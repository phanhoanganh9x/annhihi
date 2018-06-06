﻿CREATE Trigger Tr_StockManager
ON tbl_StockManager
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @ID AS INT
    DECLARE @AgentID AS INT
    DECLARE @ProductID AS INT
    DECLARE @ProductVariableID AS INT
    DECLARE @Quantity AS FLOAT
    DECLARE @QuantityCurrent AS FLOAT
    DECLARE @Type AS INT
    DECLARE @CreatedDate AS DATETIME

    SELECT TOP 1
          @ID = ID
        , @AgentID = AgentID
        , @ProductID = ProductID
        , @ProductVariableID = ProductVariableID
        , @Quantity = Quantity
        , @Type = Type
        , @CreatedDate = CreatedDate
    FROM
        tbl_StockManager AS STM
    ORDER BY
        STM.CreatedDate DESC

    SELECT
        ROW_NUMBER() OVER(ORDER BY CreatedDate DESC) AS ROWNUMBER
        , ID
        , AgentID
        , ProductID
        , ProductVariableID
        , Quantity
        , QuantityCurrent
        , [Type]
    INTO #StockTarget
    FROM
        tbl_StockManager AS STM
    WHERE
            STM.AgentID = @AgentID
        AND STM.ProductID = @ProductID
        AND STM.ProductVariableID = @ProductVariableID

    IF @@ROWCOUNT > 0
    BEGIN
        CREATE INDEX [ID_QuantityCurrent] ON #StockTarget
        (
            [ROWNUMBER] ASC
        )
    END

    SET @QuantityCurrent = 0

    SELECT
        @QuantityCurrent = (
            CASE [Type]
                WHEN 1
                    THEN STG.Quantity + STG.QuantityCurrent
                WHEN 2
                    THEN STG.QuantityCurrent - STG.Quantity
                ELSE
                    0
            END
        )
    FROM
        #StockTarget AS STG
    WHERE
        STG.ROWNUMBER = 2

    IF @Type = 2 -- Xuất kho
    BEGIN
        DECLARE @QuantityLeft AS FLOAT

        SET @QuantityLeft = @QuantityCurrent - @Quantity

        IF @QuantityLeft < 0
        BEGIN
            INSERT INTO tbl_StockManager (
                    AgentID
            ,       ProductID
            ,       ProductVariableID
            ,       Quantity
            ,       QuantityCurrent
            ,       Type
            ,       CreatedDate
            ,       CreatedBy
            ,       ModifiedDate
            ,       ModifiedBy
            ,       NoteID
            ,       OrderID
            ,       Status
            ,       SKU
            ,       MoveProID
            ,       ParentID
            )
            SELECT
                    @AgentID
            ,       @ProductID
            ,       @ProductVariableID
            ,       (-1 * @QuantityLeft)
            ,       @QuantityCurrent
            ,       1
            ,       DATEADD(Millisecond, -10, CreatedDate)
            ,       CreatedBy
            ,       NULL
            ,       NULL
            ,       N'Nhập kho bị lệch khi bán POS'
            ,       OrderID
            ,       Status
            ,       SKU
            ,       MoveProID
            ,       ParentID
            FROM
                    tbl_StockManager AS STM
            WHERE
                    ID = @ID
            AND     AgentID = @AgentID
            AND     ProductID = @ProductID
            AND     ProductVariableID = @ProductVariableID
            ;

            IF @@ROWCOUNT > 0
            BEGIN
                SET @QuantityCurrent = @Quantity
            END
        END
    END

    UPDATE tbl_StockManager
    SET QuantityCurrent = @QuantityCurrent
    WHERE
            ID = @ID
        AND AgentID = @AgentID
        AND ProductID = @ProductID
        AND ProductVariableID = @ProductVariableID
END
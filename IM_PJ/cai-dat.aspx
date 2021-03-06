﻿<%@ Page Title="Cài đặt hệ thống" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="cai-dat.aspx.cs" Inherits="IM_PJ.cai_dat" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main id="main-wrap">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div class="panel panelborderheading">
                        <div class="panel-heading clear">
                            <h3 class="page-title left not-margin-bot">Cài đặt hệ thống</h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-row">
                                <asp:Label ID="lblError" runat="server" Visible="false" ForeColor="Red"></asp:Label>
                            </div>
                            <div class="form-row">
                                <div class="row-left">
                                    Mã bảo mật
                                </div>
                                <div class="row-right">
                                    <asp:TextBox ID="txtSecurityCode" runat="server" CssClass="form-control" placeholder="Chỉ nhập khi cần thay đổi" TextMode="Password"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="row-left">
                                    Xác nhận Mã bảo mật
                                </div>
                                <div class="row-right">
                                    <asp:TextBox ID="txtConfirmSecurityCode" runat="server" CssClass="form-control" placeholder="Nhập giống phía trên" TextMode="Password"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="row-left">
                                    Số ngày được đổi trả
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="pNumOfDateToChangeProduct" ForeColor="Red"
                                        ErrorMessage="(*)" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                                <div class="row-right">
                                    <telerik:RadNumericTextBox runat="server" CssClass="form-control width-notfull" Skin="MetroTouch" ID="pNumOfDateToChangeProduct" MinValue="0" NumberFormat-GroupSizes="3" Width="100%" NumberFormat-DecimalDigits="0">
                                    </telerik:RadNumericTextBox>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="row-left">
                                    Số lượng sản phẩm được đổi trả
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="pNumOfProductCanChange" ForeColor="Red" ErrorMessage="(*)" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                                <div class="row-right">
                                    <telerik:RadNumericTextBox runat="server" CssClass="form-control width-notfull" Skin="MetroTouch" ID="pNumOfProductCanChange" MinValue="0" NumberFormat-GroupSizes="3" Width="100%" NumberFormat-DecimalDigits="0">
                                    </telerik:RadNumericTextBox>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="row-left">
                                    Phí đổi trả
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="pFeeChangeProduct" ForeColor="Red" ErrorMessage="(*)" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                                <div class="row-right">
                                    <telerik:RadNumericTextBox runat="server" CssClass="form-control width-notfull" Skin="MetroTouch" ID="pFeeChangeProduct" MinValue="0" NumberFormat-GroupSizes="3" Width="100%" NumberFormat-DecimalDigits="0">
                                    </telerik:RadNumericTextBox>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="row-left">
                                    Nội quy đổi trả hàng mua sỉ
                                </div>
                                <div class="row-right">
                                    <telerik:RadEditor runat="server" ID="pReturnRule1" Width="100%" Height="500px" ToolsFile="~/FilesResources/ToolContent.xml" Skin="Metro" DialogHandlerUrl="~/Telerik.Web.UI.DialogHandler.axd" AutoResizeHeight="True">
                                        <ImageManager ViewPaths="~/uploads/images" UploadPaths="~/uploads/images" DeletePaths="~/uploads/images" />
                                    </telerik:RadEditor>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="row-left">
                                    Nội quy đổi trả hàng mua lẻ
                                </div>
                                <div class="row-right">
                                    <telerik:RadEditor runat="server" ID="pReturnRule2" Width="100%" Height="500px" ToolsFile="~/FilesResources/ToolContent.xml" Skin="Metro" DialogHandlerUrl="~/Telerik.Web.UI.DialogHandler.axd" AutoResizeHeight="True">
                                        <ImageManager ViewPaths="~/uploads/images" UploadPaths="~/uploads/images" DeletePaths="~/uploads/images" />
                                    </telerik:RadEditor>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="row-left">
                                    CSS print barcode
                                </div>
                                <div class="row-right">
                                    <asp:TextBox ID="pCSSPrintBarcode" TextMode="MultiLine" runat="server" CssClass="form-control" Height="400px"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-row">
                                <asp:Button ID="btnLogin" runat="server" CssClass="btn primary-btn fw-btn not-fullwidth" Text="Cập nhật" OnClick="btnLogin_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:HiddenField ID="hdfTempVariable" runat="server" />
        <asp:HiddenField ID="hdfVariableFull" runat="server" />
    </main>

    <telerik:RadCodeBlock runat="server">
    </telerik:RadCodeBlock>
</asp:Content>

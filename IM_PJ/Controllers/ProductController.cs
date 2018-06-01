﻿using IM_PJ.Models;
using MB.Extensions;
using NHST.Bussiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebUI.Business;

namespace IM_PJ.Controllers
{
    public class ProductController
    {
        #region CRUD
        public static string Insert(int CategoryID, int ProductOldID, string ProductTitle, string ProductContent, string ProductSKU, double ProductStock,
            int StockStatus, bool ManageStock, double Regular_Price, double CostOfGood, double Retail_Price, string ProductImage, int ProductType,
            bool IsHidden, DateTime CreatedDate, string CreatedBy, int SupplierID, string SupplierName, string Materials,
            double MinimumInventoryLevel, double MaximumInventoryLevel, int ProductStyle)
        {
            using (var dbe = new inventorymanagementEntities())
            {
                tbl_Product ui = new tbl_Product();
                ui.CategoryID = CategoryID;
                ui.ProductOldID = ProductOldID;
                ui.ProductTitle = ProductTitle;
                ui.ProductContent = ProductContent;
                ui.ProductSKU = ProductSKU;
                ui.ProductStock = ProductStock;
                ui.StockStatus = StockStatus;
                ui.ManageStock = ManageStock;
                ui.Regular_Price = Regular_Price;
                ui.CostOfGood = CostOfGood;
                ui.Retail_Price = Retail_Price;
                ui.ProductImage = ProductImage;
                ui.ProductType = ProductType;
                ui.IsHidden = IsHidden;
                ui.CreatedDate = CreatedDate;
                ui.CreatedBy = CreatedBy;
                ui.SupplierID = SupplierID;
                ui.SupplierName = SupplierName;
                ui.Materials = Materials;
                ui.MinimumInventoryLevel = MinimumInventoryLevel;
                ui.MaximumInventoryLevel = MaximumInventoryLevel;
                ui.ProductStyle = ProductStyle;
                dbe.tbl_Product.Add(ui);
                dbe.SaveChanges();
                int kq = ui.ID;
                return kq.ToString();
            }
        }
        public static string Update(int ID, int CategoryID, int ProductOldID, string ProductTitle, string ProductContent, string ProductSKU, double ProductStock,
            int StockStatus, bool ManageStock, double Regular_Price, double CostOfGood, double Retail_Price, string ProductImage, int ProductType,
            bool IsHidden, DateTime ModifiedDate, string ModifiedBy, int SupplierID, string SupplierName, string Materials,
            double MinimumInventoryLevel, double MaximumInventoryLevel)
        {
            using (var dbe = new inventorymanagementEntities())
            {
                tbl_Product ui = dbe.tbl_Product.Where(a => a.ID == ID).SingleOrDefault();
                if (ui != null)
                {
                    ui.CategoryID = CategoryID;
                    ui.ProductOldID = ProductOldID;
                    ui.ProductTitle = ProductTitle;
                    ui.ProductContent = ProductContent;
                    ui.ProductSKU = ProductSKU;
                    if (ProductStock > 0)
                        ui.ProductStock = ProductStock;
                    if (StockStatus > 0)
                        ui.StockStatus = StockStatus;
                    ui.ManageStock = ManageStock;
                    ui.Regular_Price = Regular_Price;
                    ui.CostOfGood = CostOfGood;
                    ui.Retail_Price = Retail_Price;
                    ui.ProductImage = ProductImage;
                    ui.ProductType = ProductType;
                    ui.IsHidden = IsHidden;
                    ui.ModifiedBy = ModifiedBy;
                    ui.ModifiedDate = ModifiedDate;
                    ui.SupplierID = SupplierID;
                    ui.SupplierName = SupplierName;
                    ui.Materials = Materials;
                    ui.MinimumInventoryLevel = MinimumInventoryLevel;
                    ui.MaximumInventoryLevel = MaximumInventoryLevel;
                    int kq = dbe.SaveChanges();
                    return kq.ToString();
                }
                else
                    return null;
            }
        }

        public static string UpdateStockStatus(string SKU, int StockStatus, bool IsHidden, DateTime ModifiedDate, string ModifiedBy)
        {
            using (var dbe = new inventorymanagementEntities())
            {
                tbl_Product ui = dbe.tbl_Product.Where(a => a.ProductSKU == SKU).SingleOrDefault();
                if (ui != null)
                {

                    ui.StockStatus = StockStatus;
                    ui.IsHidden = IsHidden;
                    ui.ModifiedBy = ModifiedBy;
                    ui.ModifiedDate = ModifiedDate;
                    int kq = dbe.SaveChanges();
                    return kq.ToString();
                }
                else
                    return null;
            }
        }
        #endregion
        #region Select
        public static List<tbl_Product> GetByTextSearchIsHidden(string s, bool IsHidden)
        {
            using (var dbe = new inventorymanagementEntities())
            {
                List<tbl_Product> ags = new List<tbl_Product>();
                ags = dbe.tbl_Product.Where(p => p.ProductTitle.Contains(s) && p.IsHidden == IsHidden).OrderByDescending(o => o.ID).ToList();
                return ags;
            }
        }
        public static tbl_Product GetByID(int ID)
        {
            using (var dbe = new inventorymanagementEntities())
            {
                tbl_Product ai = dbe.tbl_Product.Where(a => a.ID == ID).SingleOrDefault();
                if (ai != null)
                {
                    return ai;
                }
                else return null;

            }
        }
        public static tbl_Product GetBySKU(string SKU)
        {
            using (var dbe = new inventorymanagementEntities())
            {
                tbl_Product ai = dbe.tbl_Product.Where(a => a.ProductSKU == SKU).SingleOrDefault();
                if (ai != null)
                {
                    return ai;
                }
                else return null;

            }
        }
        public static List<tbl_Product> GetAll(string s)
        {
            using (var dbe = new inventorymanagementEntities())
            {
                List<tbl_Product> ags = new List<tbl_Product>();
                ags = dbe.tbl_Product.Where(p => p.ProductTitle.Contains(s)).OrderByDescending(o => o.ID).ToList();
                return ags;
            }
        }
        public static List<tbl_Product> GetByCategoryID(int CategoryID)
        {
            using (var dbe = new inventorymanagementEntities())
            {
                List<tbl_Product> ags = new List<tbl_Product>();
                ags = dbe.tbl_Product.Where(p => p.CategoryID == CategoryID).OrderByDescending(o => o.ID).ToList();
                return ags;
            }
        }
        public static List<View_ProductList> View_GetByCategoryID(string s, int CategoryID)
        {
            using (var dbe = new inventorymanagementEntities())
            {
                List<View_ProductList> ags = new List<View_ProductList>();
                ags = dbe.View_ProductList.Where(p => p.ProductTitle.Contains(s) && p.CategoryID == CategoryID).OrderByDescending(o => o.ID).ToList();
                return ags;
            }
        }

        public static List<ProductSQL> GetAllSql(int categoryID, string textsearch)
        {
            var list = new List<ProductSQL>();
            var sql = @"Select p.ProductStyle as ProductStyle, c.CategoryName, p.*, i.quantiyIN as InProduct, o.quantiyIN as OutProduct, (i.quantiyIN - o.quantiyIN) as leftProduct, m.anhsanpham from tbl_product as p";
            sql += " LEFT OUTER JOIN (select ParentID, sum(quantity) as quantiyIN from tbl_InOutProductVariable where [Type]= 1 group by parentid) as i ON p.ID = i.ParentID ";
            sql += " LEFT OUTER JOIN (select ParentID, sum(quantity) as quantiyIN from tbl_InOutProductVariable where [Type]= 2 group by parentid) as o ON p.ID = o.ParentID ";

            sql += " LEFT OUTER JOIN (SELECT ProductID, MIN(ProductImage) AS anhsanpham FROM dbo.tbl_ProductImage GROUP BY ProductID) AS m ON p.ID = m.ProductID ";
            sql += " LEFT OUTER JOIN (SELECT ID, CategoryName FROM dbo.tbl_Category) AS c ON c.ID = p.CategoryID where 1 = 1 ";
            if (!string.IsNullOrEmpty(textsearch))
            {
                sql += " And (p.ProductSKU like N'%" + textsearch + "%' OR p.ProductTitle like N'%" + textsearch + "%')";
            }
            if (categoryID > 0)
            {
                sql += " AND categoryID = " + categoryID;
            }
            sql += " Order By p.ID asc";
            var reader = (IDataReader)SqlHelper.ExecuteDataReader(sql);
            int i = 1;
            while (reader.Read())
            {
                int quantityIn = reader["InProduct"].ToString().ToInt(0);
                int quantityOut = reader["OutProduct"].ToString().ToInt(0);
                int quantityLeft = quantityIn - quantityOut;
                int stockstatus = reader["StockStatus"].ToString().ToInt(0);
                var entity = new ProductSQL();
                if (reader["ID"] != DBNull.Value)
                    entity.ID = reader["ID"].ToString().ToInt(0);
                if (reader["ProductImage"] != DBNull.Value)
                    entity.ProductImage = reader["ProductImage"].ToString();
                if (reader["ProductTitle"] != DBNull.Value)
                    entity.ProductTitle = reader["ProductTitle"].ToString();
                if (reader["ProductSKU"] != DBNull.Value)
                    entity.ProductSKU = reader["ProductSKU"].ToString();
                if (quantityIn > 0)
                {
                    if (quantityLeft > 0)
                    {
                        entity.ProductInstockStatus = "<span class=\"bg-green\">Còn hàng</span>";
                        entity.StockStatus = 1;
                    }
                    else
                    {
                        entity.ProductInstockStatus = "<span class=\"bg-red\">Hết hàng</span>";
                        entity.StockStatus = 2;
                    }
                }
                else
                {
                    entity.ProductInstockStatus = "<span class=\"bg-yellow\">Đang nhập kho</span>";
                    entity.StockStatus = 3;
                }

                entity.TotalProductInstockQuantityIn = quantityIn;
                entity.TotalProductInstockQuantityOut = quantityOut;
                entity.TotalProductInstockQuantityLeft = quantityLeft;
                if (reader["Regular_Price"] != DBNull.Value)
                    entity.RegularPrice = Convert.ToDouble(reader["Regular_Price"].ToString());
                if (reader["CostOfGood"] != DBNull.Value)
                    entity.CostOfGood = Convert.ToDouble(reader["CostOfGood"].ToString());
                if (reader["Retail_Price"] != DBNull.Value)
                    entity.RetailPrice = Convert.ToDouble(reader["Retail_Price"].ToString());
                if (reader["CategoryName"] != DBNull.Value)
                    entity.CategoryName = reader["CategoryName"].ToString();
                if (reader["CategoryID"] != DBNull.Value)
                    entity.CategoryID = reader["CategoryID"].ToString().ToInt(0);
                if (reader["CreatedDate"] != DBNull.Value)
                    entity.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
                if (reader["ProductContent"] != DBNull.Value)
                    entity.ProductContent = reader["ProductContent"].ToString();
                if (reader["ProductStyle"] != DBNull.Value)
                    entity.ProductStyle = reader["ProductStyle"].ToString().ToInt(0);
                i++;
                list.Add(entity);
            }
            reader.Close();
            return list.OrderByDescending(x => x.CreatedDate).ToList();
        }

        public static List<ProductSQL> GetAllSqlView(int categoryID, string textsearch)
        {
            var list = new List<ProductSQL>();
            var sql = @"Select p.ProductStyle as ProductStyle, c.CategoryName, p.*, i.quantiyIN as InProduct, o.quantiyIN as OutProduct, (i.quantiyIN - o.quantiyIN) as leftProduct, m.anhsanpham from tbl_product as p";
            sql += " LEFT OUTER JOIN (select ParentID, sum(quantity) as quantiyIN from tbl_InOutProductVariable where [Type]= 1 group by parentid) as i ON p.ID = i.ParentID ";
            sql += " LEFT OUTER JOIN (select ParentID, sum(quantity) as quantiyIN from tbl_InOutProductVariable where [Type]= 2 group by parentid) as o ON p.ID = o.ParentID ";

            sql += " LEFT OUTER JOIN (SELECT ProductID, MIN(ProductImage) AS anhsanpham FROM dbo.tbl_ProductImage GROUP BY ProductID) AS m ON p.ID = m.ProductID ";
            sql += " LEFT OUTER JOIN (SELECT ID, CategoryName FROM dbo.tbl_Category) AS c ON c.ID = p.CategoryID where 1 = 1 ";
            if (!string.IsNullOrEmpty(textsearch))
            {
                sql += " And (p.ProductSKU like N'%" + textsearch + "%' OR p.ProductTitle like N'%" + textsearch + "%')";
            }
            if (categoryID > 0)
            {
                sql += " AND categoryID = " + categoryID;
            }
            sql += " Order By p.ID asc";
            var reader = (IDataReader)SqlHelper.ExecuteDataReader(sql);
            int i = 1;
            while (reader.Read())
            {
                int quantityIn = reader["InProduct"].ToString().ToInt(0);
                int quantityOut = reader["OutProduct"].ToString().ToInt(0);
                //int quantityLeft = reader["leftProduct"].ToString().ToInt(0);
                int quantityLeft = quantityIn - quantityOut;
                int stockstatus = reader["StockStatus"].ToString().ToInt(0);
                var entity = new ProductSQL();
                if (reader["ID"] != DBNull.Value)
                    entity.ID = reader["ID"].ToString().ToInt(0);
                if (reader["anhsanpham"] != DBNull.Value)
                    entity.ProductImage = reader["anhsanpham"].ToString();
                if (reader["ProductTitle"] != DBNull.Value)
                    entity.ProductTitle = reader["ProductTitle"].ToString();
                if (reader["ProductSKU"] != DBNull.Value)
                    entity.ProductSKU = reader["ProductSKU"].ToString();
                //if (reader["StockStatus"] != DBNull.Value)
                //entity.StockStatus = reader["StockStatus"].ToString().ToInt(0);
                if (quantityIn > 0)
                {
                    if (quantityLeft > 0)
                    {
                        entity.ProductInstockStatus = "<span class=\"bg-green\">Còn hàng</span>";
                        entity.StockStatus = 1;
                    }
                    else
                    {
                        entity.ProductInstockStatus = "<span class=\"bg-red\">Hết hàng</span>";
                        entity.StockStatus = 2;
                    }
                }
                else
                {
                    entity.ProductInstockStatus = "<span class=\"bg-yellow\">Đang nhập kho</span>";
                    entity.StockStatus = 3;
                }
                //if (stockstatus != 3)
                //{
                //    if (quantityLeft > 0)
                //        entity.ProductInstockStatus = "Còn hàng";
                //    else
                //        entity.ProductInstockStatus = "Hết hàng";
                //}
                //else
                //{
                //    entity.ProductInstockStatus = "Đang chờ nhập hàng";
                //    quantityIn = -1;
                //}

                entity.TotalProductInstockQuantityIn = quantityIn;
                entity.TotalProductInstockQuantityOut = quantityOut;
                entity.TotalProductInstockQuantityLeft = quantityLeft;
                if (reader["Regular_Price"] != DBNull.Value)
                    entity.RegularPrice = Convert.ToDouble(reader["Regular_Price"].ToString());
                if (reader["CostOfGood"] != DBNull.Value)
                    entity.CostOfGood = Convert.ToDouble(reader["CostOfGood"].ToString());
                if (reader["Retail_Price"] != DBNull.Value)
                    entity.RetailPrice = Convert.ToDouble(reader["Retail_Price"].ToString());
                if (reader["CategoryName"] != DBNull.Value)
                    entity.CategoryName = reader["CategoryName"].ToString();
                if (reader["CategoryID"] != DBNull.Value)
                    entity.CategoryID = reader["CategoryID"].ToString().ToInt(0);
                if (reader["CreatedDate"] != DBNull.Value)
                    entity.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
                if (reader["ProductContent"] != DBNull.Value)
                    entity.ProductContent = reader["ProductContent"].ToString();
                if (reader["ProductStyle"] != DBNull.Value)
                    entity.ProductStyle = reader["ProductStyle"].ToString().ToInt(0);
                i++;
                list.Add(entity);
            }
            reader.Close();
            return list.OrderByDescending(x => x.CreatedDate).ToList();
        }
        public static ProductStock GetStock(int ProductVariableID)
        {
            var entity = new ProductStock();
            var sql = @"select t.ID,t.SKU,t.ID, d.quantiyIN as InProduct, k.quantiyIN as OutProduct, (d.quantiyIN - k.quantiyIN) as leftProduct from tbl_ProductVariable as t";
            sql += " left outer join (select  ProductVariableID, sum(quantity) as quantiyIN from tbl_InOutProductVariable where [Type]= 1 group by ProductVariableID) as d ON t.ID = d.ProductVariableID ";
            sql += " left outer join (select ProductVariableID, sum(quantity) as quantiyIN from tbl_InOutProductVariable where [Type]= 2 group by ProductVariableID) as k ON t.ID = k.ProductVariableID ";
            if (ProductVariableID > 0)
            {
                sql += " Where t.ID = " + ProductVariableID;
            }
            var reader = (IDataReader)SqlHelper.ExecuteDataReader(sql);

            while (reader.Read())
            {
                int quantityIn = reader["InProduct"].ToString().ToInt(0);
                int quantityOut = reader["OutProduct"].ToString().ToInt(0);
                int quantityLeft = quantityIn - quantityOut;


                if (quantityIn > 0)
                {
                    if (quantityLeft > 0)
                    {
                        entity.ProductInstockStatus = "<span class=\"bg-green\">Còn hàng</span>";

                    }
                    else
                    {
                        entity.ProductInstockStatus = "<span class=\"bg-red\">Hết hàng</span>";
                    }
                }
                else
                {
                    entity.ProductInstockStatus = "<span class=\"bg-yellow\">Đang chờ nhập hàng</span>";

                }
                entity.quantityLeft = quantityLeft;
                //if (reader["leftProduct"] != DBNull.Value)
                //    entity.quantityLeft = reader["leftProduct"].ToString().ToInt();
            }
            reader.Close();
            return entity;
        }
        #endregion
        #region Class
        public class ProductSQL
        {
            public int ID { get; set; }
            public string ProductImage { get; set; }
            public string ProductTitle { get; set; }
            public string ProductSKU { get; set; }
            public string ProductInstockStatus { get; set; }
            public double TotalProductInstockQuantityIn { get; set; }
            public double TotalProductInstockQuantityOut { get; set; }
            public double TotalProductInstockQuantityLeft { get; set; }
            public string ProductContent { get; set; }
            public double RegularPrice { get; set; }
            public double CostOfGood { get; set; }
            public double RetailPrice { get; set; }
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }
            public DateTime CreatedDate { get; set; }
            public int StockStatus { get; set; }
            public int ProductStyle { get; set; }
        }

        public class ProductStock
        {
            public string ProductInstockStatus { get; set; }
            public int quantityLeft { get; set; }
        }
        #endregion
    }
}
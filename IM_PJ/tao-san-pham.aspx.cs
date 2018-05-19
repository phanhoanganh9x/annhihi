﻿using IM_PJ.Controllers;
using MB.Extensions;
using NHST.Bussiness;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace IM_PJ
{
    public partial class tao_san_pham : System.Web.UI.Page
    {

        public static string htmlAll = "";
        public static int element = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userLoginSystem"] != null)
                {
                    string username = Session["userLoginSystem"].ToString();
                    var acc = AccountController.GetByUsername(username);
                    if (acc != null)
                    {
                        if (acc.RoleID == 2)
                        {
                            Response.Redirect("/dang-nhap");
                        }
                    }
                }
                else
                {

                    Response.Redirect("/dang-nhap");
                }
                LoadSupplier();
                LoadData();
                LoadPDW();
                LoadCategory();
            }
        }
        public void LoadPDW()
        {
            var variablename = VariableController.GetAllIsHidden(false);
            if (variablename.Count > 0)
            {
                ddlVariablename.Items.Clear();
                ddlVariablename.Items.Insert(0, new ListItem("-- Chọn tên thuộc tính --", "0"));
                //ddlPro.Items.Insert(0, "Tỉnh/TP");
                foreach (var p in variablename)
                {
                    ListItem listitem = new ListItem(p.VariableName, p.ID.ToString());
                    ddlVariablename.Items.Add(listitem);
                }
                ddlVariablename.DataBind();

            }
            ddlVariableValue.Items.Clear();
            ddlVariableValue.Items.Insert(0, new ListItem("-- Chọn giá trị --", "0"));
        }

        public void BindVariableValue(int VariableID)
        {
            ddlVariableValue.Items.Clear();
            ddlVariableValue.Items.Insert(0, new ListItem("-- Chọn giá trị --", "0"));
            if (VariableID > 0)
            {
                var variableValue = VariableValueController.GetByVariableID(VariableID);
                if (variableValue.Count > 0)
                {
                    foreach (var p in variableValue)
                    {
                        ListItem listitem = new ListItem(p.VariableValue, p.ID.ToString());
                        ddlVariableValue.Items.Add(listitem);
                    }
                }
                //ddlVariableValue.DataSource = VariableValueController.GetByVariableID(VariableID);
                ddlVariableValue.DataBind();
            }
        }
        protected void ddlVariablename_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindVariableValue(ddlVariablename.SelectedValue.ToInt(0));
        }

        public void LoadSupplier()
        {
            var supplier = SupplierController.GetAllWithIsHidden(false);
            ddlSupplier.Items.Clear();
            ddlSupplier.Items.Insert(0, new ListItem("-- Chọn nhà cung cấp --", "0"));
            if (supplier.Count > 0)
            {
                foreach (var p in supplier)
                {
                    ListItem listitem = new ListItem(p.SupplierName, p.ID.ToString());
                    ddlSupplier.Items.Add(listitem);
                }
                ddlSupplier.DataBind();
            }
        }

        public void LoadCategory()
        {
            var category = CategoryController.GetAllWithIsHidden(false);
            ddlCategory.Items.Clear();
            ddlCategory.Items.Insert(0, new ListItem("-- Chọn danh mục --", "0"));
            if (category.Count > 0)
            {
                foreach (var p in category)
                {
                    ListItem listitem = new ListItem(p.CategoryName, p.ID.ToString());
                    ddlCategory.Items.Add(listitem);
                }
                ddlCategory.DataBind();
            }
        }
        //public void LoadCategoryParent(int ParentID)
        //{

        //    var category = CategoryController.API_GetByParentID(ParentID);
        //    ddlCategoryParent.Items.Clear();
        //    ddlCategoryParent.Items.Insert(0, new ListItem("-- Chọn danh mục --", "0"));
        //    if (category.Count > 0)
        //    {
        //        foreach (var p in category)
        //        {
        //            ListItem listitem = new ListItem(p.CategoryName, p.ID.ToString());
        //            ddlCategoryParent.Items.Add(listitem);
        //        }
        //        ddlCategoryParent.DataBind();
        //    }
        //}

        public void LoadData()
        {
            int cateid = Request.QueryString["categoryid"].ToInt(0);
            if (cateid > 0)
            {
                ///danh-sach-san-pham.aspx?categoryid=25
                ltrBack.Text = "<a href=\"/danh-sach-san-pham.aspx?categoryid=" + cateid + "\" class=\"btn primary-btn fw-btn not-fullwidth\">Trở về</a>";
            }

            //Lấy variable
            //StringBuilder htmlVariable = new StringBuilder();
            //var variables = VariableController.GetAllIsHidden(false);
            //foreach (var v in variables)
            //{
            //    htmlVariable.Append("<div class=\"form-row variable-row\">");
            //    htmlVariable.Append("   <span class=\"row-left variable-name\" data-id=\"" + v.ID + "\" data-name=\"" + v.VariableName + "\">" + v.VariableName + "</span>");
            //    htmlVariable.Append("   <div class=\"row-right\" data-name=\"" + v.VariableName + "\">");
            //    htmlVariable.Append("       <select class=\"form-control variable-value\">");
            //    htmlVariable.Append("           <option value=\"0\">Chọn thuộc tính " + v.VariableName + "</option>");
            //    var variableValue = VariableValueController.GetByVariableIDIsHidden(v.ID, false);
            //    if (variableValue.Count > 0)
            //    {
            //        foreach (var vl in variableValue)
            //        {
            //            htmlVariable.Append("           <option value=\"" + vl.ID + "\">" + vl.VariableValue + "</option>");
            //        }
            //    }
            //    htmlVariable.Append("       </select>");
            //    htmlVariable.Append("   </div>");
            //    htmlVariable.Append("</div>");
            //}
            //ltrVariable.Text = htmlVariable.ToString();


        }


        public static void DeQuyCongTu(int el, int final, string r, List<Variable> listObject)
        {
            var currentElement = listObject[el];
            var name = currentElement.VariableName;
            var childrens = currentElement.Value;
            foreach (var item in childrens)
            {
                var variableID = VariableValueController.GetByID(Convert.ToInt32(item.Value)).VariableID;
                string rprev = r;
                int leng = el + 1;
                var skutext = VariableValueController.GetByID(Convert.ToInt32(item.Value)) != null ?
                        VariableValueController.GetByID(Convert.ToInt32(item.Value)).SKUText : "";
                if (leng < final)
                {
                    rprev += variableID + "*" + name + ":" + item.Value + "," + item.Name + "," + skutext + "-";
                    DeQuyCongTu(leng, listObject.Count, rprev, listObject);
                }
                else
                {
                    string a = r;
                    a += variableID + "*" + name + ":" + item.Value + "," + item.Name + "," + skutext + "|";
                    htmlAll += a;
                }
            }
        }

        [WebMethod]
        public static string getVariable(string list)
        {
            List<Variable> listparent = new List<Variable>();
            List<VariableGetOut> vg = new List<VariableGetOut>();
            string[] value = list.Split('|');
            for (int i = 0; i < value.Length - 1; i++)
            {
                Variable vr = new Variable();
                List<VariableValue> vl = new List<VariableValue>();
                string[] t = value[i].Split(':');
                vr.VariableName = t[0];
                string[] vl1 = t[1].Split(';');
                for (int k = 0; k < vl1.Length - 1; k++)
                {
                    string[] vl2 = vl1[k].Split('-');
                    VariableValue vvl = new VariableValue();
                    //vvl.ID = vl2[0].ToInt();
                    vvl.Value = vl2[0];
                    vvl.Name = vl2[1];
                    vl.Add(vvl);
                }
                vr.Value = vl;
                listparent.Add(vr);
            }
            htmlAll = "";
            DeQuyCongTu(element, listparent.Count, "", listparent);
            string[] item = htmlAll.Split('|');
            if (item.Count() > 0)
            {

                for (int i = 0; i < item.Length - 1; i++)
                {

                    string listvalue = "";
                    string namelist = "";
                    string variablevalue = "";
                    string valuename = "";
                    string varisku = "";
                    string productvariable = "";
                    string ProductVariableName = "";
                    string[] temp = item[i].Split('-');
                    for (int j = 0; j < temp.Length; j++)
                    {
                        string[] vl1 = temp[j].Split('*');
                        listvalue += vl1[0].Trim() + "|";
                        string[] vl2 = vl1[1].Split(':');
                        namelist += vl2[0].Trim() + "|";
                        string[] vl3 = vl2[1].Split(',');
                        variablevalue += vl3[0].Trim() + "|";
                        valuename += vl3[1].Trim() + "|";
                        varisku += vl3[2].Trim();
                        productvariable += vl1[0] + ":" + vl3[0] + "|";
                        ProductVariableName += vl2[0] + ": " + vl3[1] + "|";
                    }
                    VariableGetOut v = new VariableGetOut();
                    v.VariableListValue = listvalue;
                    v.VariableNameList = namelist;
                    v.VariableValue = variablevalue;
                    v.VariableValueName = valuename;
                    v.VariableSKUText = varisku;
                    v.ProductVariable = productvariable;
                    v.ProductVariableName = ProductVariableName;
                    vg.Add(v);
                }
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(vg);


        }

        [WebMethod]
        public static string CheckSKU(string SKU)
        {
            string ProductSKU = SKU.Trim().ToUpper();

            var productcheck = ProductController.GetBySKU(ProductSKU);
            if (productcheck != null)
            {
                return "null";
            }
            else
            {
                return "ok";

            }

        }

        [WebMethod]
        public static string getParent(int parent)
        {
            //hdfParentID.Value = parent.ToString();
            List<GetOutCategory> gc = new List<GetOutCategory>();
            if (parent != 0)
            {
                var parentlist = CategoryController.API_GetByParentID(parent);
                if (parentlist != null)
                {

                    for (int i = 0; i < parentlist.Count; i++)
                    {
                        GetOutCategory go = new GetOutCategory();
                        go.ID = parentlist[i].ID;
                        go.CategoryName = parentlist[i].CategoryName;
                        go.CategoryLevel = parentlist[i].CategoryLevel.ToString();
                        gc.Add(go);
                    }
                }
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(gc);
        }

        public class GetOutCategory
        {
            public int ID { get; set; }
            public string CategoryName { get; set; }
            public string CategoryLevel { get; set; }
        }

        public class Variable
        {
            public string VariableName { get; set; }
            public List<VariableValue> Value { get; set; }
        }
        public class VariableValue
        {
            //public int ID { get; set; }
            public string Value { get; set; }
            public string Name { get; set; }
        }

        public class VariableGetOut
        {
            public string VariableListValue { get; set; }
            public string VariableNameList { get; set; }
            public string VariableValue { get; set; }
            public string VariableValueName { get; set; }
            public string VariableSKUText { get; set; }
            public string ProductVariable { get; set; }
            public string ProductVariableName { get; set; }
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = Session["userLoginSystem"].ToString();
            var acc = AccountController.GetByUsername(username);
            DateTime currentDate = DateTime.Now;
            if (acc != null)
            {
                if (acc.RoleID == 0 || acc.RoleID == 1)
                {
                    int cateID = hdfParentID.Value.ToInt();
                    if (cateID > 0)
                    {
                        string ProductSKU = txtProductSKU.Text.Trim().ToUpper();
                        var check = true;
                        var productcheck = ProductController.GetBySKU(ProductSKU);
                        if (productcheck != null)
                        {
                            check = false;
                        }
                        else
                        {
                            var productvariable = ProductVariableController.GetBySKU(ProductSKU);
                            if (productvariable != null)
                                check = false;
                        }

                        if (check == false)
                        {
                            PJUtils.ShowMessageBoxSwAlert("Trùng mã SKU vui lòng kiểm tra lại", "e", false, Page);
                        }
                        else
                        {
                            string ProductTitle = txtProductTitle.Text;
                            string ProductContent = pContent.Content;

                            double ProductStock = 0;
                            int StockStatus = 3;
                            bool ManageStock = chkManageStock.Checked;
                            double Regular_Price = Convert.ToDouble(pRegular_Price.Value);
                            double CostOfGood = Convert.ToDouble(pCostOfGood.Value);
                            double Retail_Price = Convert.ToDouble(pRetailPrice.Value);
                            int supplierID = ddlSupplier.SelectedValue.ToInt(0);
                            string supplierName = ddlSupplier.SelectedItem.ToString();
                            bool IsHidden = chkIsHidden.Checked;
                            int a = 1;
                            if (hdfsetStyle.Value == "2")
                            {
                                a = hdfsetStyle.Value.ToInt();
                            }
                            string kq = ProductController.Insert(cateID, 0, ProductTitle, ProductContent, ProductSKU, ProductStock,
                                StockStatus, ManageStock, Regular_Price, CostOfGood, Retail_Price, "", 0,
                                IsHidden, currentDate, username, supplierID, supplierName,
                                txtMaterials.Text, Convert.ToDouble(pMinimumInventoryLevel.Value), Convert.ToDouble(pMaximumInventoryLevel.Value),a);
                            if (kq.ToInt(0) > 0)
                            {
                                int ProductID = kq.ToInt(0);
                                string variable = hdfVariableListInsert.Value;
                                if (!string.IsNullOrEmpty(variable))
                                {
                                    string[] items = variable.Split(',');
                                    for (int i = 0; i < items.Length - 1; i++)
                                    {
                                        string item = items[i];
                                        string[] itemElement = item.Split(';');

                                        string datanameid = itemElement[0];
                                        string[] datavalueid = itemElement[1].Split('|');
                                        string datanametext = itemElement[2];
                                        string datavaluetext = itemElement[3];
                                        string productvariablesku = itemElement[4];
                                        string regularprice = itemElement[5];
                                        string costofgood = itemElement[6];
                                        string retailprice = itemElement[7];
                                        string[] datanamevalue = itemElement[8].Split('|');
                                        string imageUpload = itemElement[8];
                                        int max = itemElement[9].ToInt();
                                        int min = itemElement[10].ToInt();
                                     

                                        int stockstatus = itemElement[11].ToInt();
                                        HttpPostedFile postedFile = Request.Files["" + imageUpload + ""];
                                        string image = "";
                                        bool CHECK = true;
                                        if (postedFile != null && postedFile.ContentLength > 0)
                                        {
                                            string filePath = Server.MapPath("/Uploads/Images/") + Path.GetFileName(postedFile.FileName);
                                            postedFile.SaveAs(filePath);
                                            image = "/Uploads/Images/" + Path.GetFileName(postedFile.FileName);
                                        }
                                        if (a == 2)
                                        {
                                            CHECK = itemElement[12].ToBool();
                                        }


                                        string kq1 = ProductVariableController.Insert(ProductID, ProductSKU, productvariablesku, 0, stockstatus, Convert.ToDouble(regularprice),
                                            Convert.ToDouble(costofgood), Convert.ToDouble(retailprice), image, CHECK, false, currentDate, username,
                                            supplierID, supplierName, min, max);
                                        string color = "";
                                        string size = "";
                                        int ProductVariableID = 0;
                                        if (kq1.ToInt(0) > 0)
                                        {
                                            ProductVariableID = kq1.ToInt(0);
                                            //if (datanamevalue.Length - 1 > 0)
                                            //{

                                            //for (int j = 0; j < datavalueid.Length - 1; j++)
                                            //{
                                            color = datavalueid[0];
                                            size = datavalueid[1];
                                            string[] Data = datanametext.Split('|');
                                            //string[] DataID = datanameid.Split('|');
                                            string[] DataValue = datavaluetext.Split('|');
                                            for (int k = 0; k < Data.Length - 2; k++)
                                            {
                                                int variablevalueID = datavalueid[k].ToInt();
                                                string variableName = Data[k];
                                                string variableValueName = DataValue[k];
                                                ProductVariableValueController.Insert(ProductVariableID, productvariablesku, variablevalueID,
                                                        variableName, variableValueName, false, currentDate, username);
                                            }
                                            //        string variableName = "";
                                            //        string variableValueName = "";
                                            //        var variablevalueID = datavalueid[j].ToInt(0);
                                            //        var variablevalue = VariableValueController.GetByID(variablevalueID);
                                            //        if (variablevalue != null)
                                            //        {
                                            //            variableName = variablevalue.VariableName;
                                            //            variableValueName = variablevalue.VariableValue;
                                            //            if (variablevalue.VariableID == 1)
                                            //            {
                                            //                color = LeoUtils.ConvertToUnSign(variablevalue.VariableValueText.Trim());
                                            //            }
                                            //            else if (variablevalue.VariableID == 2)
                                            //            {
                                            //                size = LeoUtils.ConvertToUnSign(variablevalue.VariableValueText.Trim());
                                            //            }
                                            //        }
                                            //        ProductVariableValueController.Insert(ProductVariableID, productvariablesku, variablevalueID,
                                            //            variableName, variableValueName, false, currentDate, username);
                                            //    }
                                            //}
                                        }
                                        ProductVariableController.UpdateColorSize(ProductVariableID, color, size);
                                        
                                    }
                                   
                                }

                                //Phần thêm ảnh sản phẩm
                                string duongdan = "/Uploads/Images/";
                                string IMG = "";
                                if (hinhDaiDien.UploadedFiles.Count > 0)
                                {
                                    foreach (UploadedFile f in hinhDaiDien.UploadedFiles)
                                    {
                                        var o = duongdan + Guid.NewGuid() + f.GetExtension();
                                        try
                                        {
                                            f.SaveAs(Server.MapPath(o));
                                            IMG = o;
                                            ProductImageController.Insert(ProductID, IMG, false, currentDate, username);
                                        }
                                        catch { }
                                    }
                                }
                                //Phần thêm ảnh cho từng sản phẩm
                                //Response.Redirect("/tat-ca-san-pham.aspx");
                                //PJUtils.ShowMessageBoxSwAlert("Tạo mới sản phẩm thành công", "s", true, Page);
                                Response.Redirect("xem-san-pham.aspx?id=" + kq + "");
                            }
                        }

                    }
                   
                }
            }

        }



        //protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    LoadCategoryParent(ddlCategory.SelectedValue.ToInt(0));
        //}

        ///Lưu ảnh
        ///string duongdan = "";
        //if (hinhDaiDien.UploadedFiles.Count > 0)
        //{
        //    foreach (UploadedFile f in hinhDaiDien.UploadedFiles)
        //    {
        //        var o = duongdan + Guid.NewGuid() + f.GetExtension();
        //        try
        //        {
        //            f.SaveAs(Server.MapPath(o));
        //            IMG = o;
        //        }
        //        catch { }
        //    }
        //}


    }
}
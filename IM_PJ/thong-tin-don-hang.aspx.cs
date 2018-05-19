﻿using IM_PJ.Controllers;
using IM_PJ.Models;
using MB.Extensions;
using NHST.Bussiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static IM_PJ.Controllers.DiscountCustomerController;
using static IM_PJ.pos;

namespace IM_PJ
{
    public partial class thong_tin_don_hang : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Session["userLoginSystem"] = "admin";
                if (Session["userLoginSystem"] != null)
                {
                    string username = Session["userLoginSystem"].ToString();
                    var acc = AccountController.GetByUsername(username);
                    if (acc != null)
                    {
                        Session["refund"] = "1";
                        var agent = acc.AgentID;
                        if (agent == 1)
                        {
                            hdfIsMain.Value = "1";
                        }
                        else
                        {
                            hdfIsMain.Value = "0";
                        }

                        if (acc.RoleID == 0 || acc.RoleID == 2)
                        { }
                        else
                        {
                            Response.Redirect("/dang-nhap");
                        }
                    }
                }
                else
                {
                    Response.Redirect("/dang-nhap");
                }
                LoadData();
            }
        }
        public void LoadData()
        {
            int ID = Request.QueryString["id"].ToInt(0);
            if (ID > 0)
            {
                var order = OrderController.GetByID(ID);
                if (order != null)
                {
                    var dc = DiscountController.GetAll();
                    if (dc != null)
                    {
                        string list = "";
                        foreach (var item in dc)
                        {
                            list += item.Quantity + "-" + item.DiscountPerProduct + "|";
                        }
                        hdfChietKhau.Value = list;
                    }
                    ViewState["ID"] = ID;
                    Session["odid"] = ID;
                    int AgentID = Convert.ToInt32(order.AgentID);
                    txtPhone.Text = order.CustomerPhone;
                    txtFullname.Text = order.CustomerName;
                    txtAddress.Text = order.CustomerAddress;
                    var cus = CustomerController.GetByID(order.CustomerID.Value);
                    if (cus != null)
                    {
                        txtNick.Text = cus.Nick;
                        txtZalo.Text = cus.Zalo;
                        txtFacebook.Text = cus.Facebook;
                        if (!string.IsNullOrEmpty(cus.Facebook))
                        {
                            ltrFb.Text += "<a href =\"" + cus.Facebook + "\" class=\"btn primary-btn fw-btn not-fullwidth\" target=\"_blank\">Xem</a>";
                        }
                    }
                    int customerID = Convert.ToInt32(order.CustomerID);
                    ltrViewDetail.Text = "<a href=\"javascript:;\" class=\"btn primary-btn fw-btn not-fullwidth\" onclick=\"viewCustomerDetail('" + customerID + "')\">Xem chi tiết</a><a href=\"javascript:;\" class=\"btn primary-btn fw-btn not-fullwidth clear-btn\" onclick=\"clearCustomerDetail()\">Bỏ qua</a>";
                    var d = DiscountCustomerController.getbyCustID(customerID);
                    if (d.Count > 0)
                    {
                        var da = d[0].DiscountAmount;
                        hdfIsDiscount.Value = "1";
                        hdfDiscountAmount.Value = da.ToString();
                        ltrDiscountInfo.Text = "<strong>Khách hàng được chiết khấu: " + string.Format("{0:N0}", Convert.ToDouble(da)) + " vnđ/sản phẩm.</strong>";
                    }
                    else
                    {
                        hdfIsDiscount.Value = "0";
                        hdfDiscountAmount.Value = "0";
                    }
                    int customerType = Convert.ToInt32(order.OrderType);
                    ltrCustomerType.Text = "";
                    ltrCustomerType.Text += "<select class=\"form-control customer-type\" onchange=\"getProductPrice($(this))\">";
                    if (customerType == 1)
                    {
                        ltrCustomerType.Text += "<option value=\"2\">Khách mua sỉ</option>";
                        ltrCustomerType.Text += "<option value=\"1\" selected>Khách mua lẻ</option>";

                    }
                    else
                    {
                        ltrCustomerType.Text += "<option value=\"2\" selected>Khách mua sỉ</option>";
                        ltrCustomerType.Text += "<option value=\"1\">Khách mua lẻ</option>";

                    }
                    ltrCustomerType.Text += "</select>";
              

                    double ProductQuantity = 0;
                    double totalPrice = Convert.ToDouble(order.TotalPrice);
                    double totalPriceNotDiscount = Convert.ToDouble(order.TotalPriceNotDiscount);
                    double GuestPaid = Convert.ToDouble(order.GuestPaid);
                    hdfcheckR.Value = "";
                    int totalrefund = 0;
                    if (order.RefundsGoodsID > 0)
                    {
                        var re = RefundGoodController.GetByID(order.RefundsGoodsID.Value);
                        if (re != null)
                        {
                            totalrefund = Convert.ToInt32(re.TotalPrice);
                            hdfcheckR.Value = order.RefundsGoodsID.ToString() + "," + re.TotalPrice;
                        }
                       
                        ltrtotalpricedetail.Text = string.Format("{0:N0}", totalPrice - totalrefund);

                        ltrTotalPriceRefund.Text = string.Format("{0:N0}", totalrefund);
                    }

                    double guestChange = GuestPaid - totalPrice + totalrefund;
                  
                    ltrTotalchagne.Text = string.Format("{0:N0}", guestChange);
                    pGuestPaid.Value = GuestPaid;
                    int paymentStatus = Convert.ToInt32(order.PaymentStatus);
                    int excuteStatus = Convert.ToInt32(order.ExcuteStatus);
                    int shipping = Convert.ToInt32(order.ShippingType);
                    int paymenttype = Convert.ToInt32(order.PaymentType);
                    #region Lấy danh sách sản phẩm
                    var orderdetails = OrderDetailController.GetByOrderID(ID);
                    string html = "";
                    string Print = "";
                    if (orderdetails.Count > 0)
                    {

                        int t = 0;
                        int orderitem = 0;
                        foreach (var item in orderdetails)
                        {
                            ProductQuantity += Convert.ToDouble(item.Quantity);

                            int ProductType = Convert.ToInt32(item.ProductType);
                            int ProductID = Convert.ToInt32(item.ProductID);
                            int ProductVariableID = Convert.ToInt32(item.ProductVariableID);
                            double ItemPrice = Convert.ToDouble(item.Price);
                            string SKU = item.SKU;
                            double Giabansi = 0;
                            double Giabanle = 0;
                            string stringGiabansi = "";
                            string stringGiabanle = "";
                            double QuantityInstock = 0;
                            string ProductImageOrigin = "";
                            string ProductVariable = "";
                            string ProductName = "";
                            int PID = 0;
                            string ProductVariableName = "";
                            string ProductVariableValue = "";
                            string ProductVariableSave = "";
                            double QuantityMainInstock = 0;
                            string ProductImage = "";
                            string QuantityMainInstockString = "";
                            string QuantityInstockString = "";
                            string productVariableDescription = item.ProductVariableDescrition;

                            if (ProductType == 1)
                            {
                                PID = ProductID;
                                var product = ProductController.GetBySKU(SKU);
                                if (product != null)
                                {
                                    double total = PJUtils.TotalProductQuantityInstock(AgentID, SKU);
                                    if (customerType == 1)
                                    {
                                        Giabansi = Convert.ToDouble(product.CostOfGood);
                                        Giabanle = ItemPrice;
                                    }
                                    else
                                    {
                                        Giabansi = ItemPrice;
                                        Giabanle = Convert.ToDouble(product.Retail_Price);
                                    }
                                    stringGiabansi = string.Format("{0:N0}", Giabansi);
                                    stringGiabanle = string.Format("{0:N0}", Giabanle);
                                    string variablename = "";
                                    string variablevalue = "";
                                    string variable = "";
                                    string variablesave = "";

                                    double mainstock = PJUtils.TotalProductQuantityInstock(1, SKU);

                                    QuantityInstock = total;
                                    QuantityInstockString = string.Format("{0:N0}", total);

                                    var img = ProductImageController.GetFirstByProductID(product.ID);
                                    if (img != null)
                                    {
                                        ProductImage = "<img src=\"" + img.ProductImage + "\"  />";
                                        ProductImageOrigin = img.ProductImage;
                                    }
                                    else
                                    {
                                        ProductImage = "";
                                        ProductImageOrigin = "";
                                    }

                                    ProductVariable = variable;
                                    ProductName = product.ProductTitle;

                                    QuantityMainInstock = mainstock;
                                    QuantityMainInstockString = string.Format("{0:N0}", mainstock);
                                    ProductVariableSave = item.ProductVariableDescrition;

                                    ProductVariableName = variablename;
                                    ProductVariableValue = variablevalue;
                                }
                            }
                            else
                            {
                                PID = ProductVariableID;
                                var productvariable = ProductVariableController.GetBySKU(SKU);
                                if (productvariable != null)
                                {
                                    SKU = productvariable.SKU.Trim().ToUpper();
                                    double total = PJUtils.TotalProductQuantityInstock(AgentID, SKU);
                                    if (customerType == 1)
                                    {
                                        Giabansi = Convert.ToDouble(productvariable.CostOfGood);
                                        Giabanle = ItemPrice;
                                    }
                                    else
                                    {
                                        Giabansi = ItemPrice;
                                        Giabanle = Convert.ToDouble(productvariable.RetailPrice);
                                    }
                                    stringGiabansi = string.Format("{0:N0}", Giabansi);
                                    stringGiabanle = string.Format("{0:N0}", Giabanle);


                                    string variablename = "";
                                    string variablevalue = "";
                                    string variable = "";
                                    string variablesave = "";

                                    string[] vs = productVariableDescription.Split('|');
                                    if (vs.Length - 1 > 0)
                                    {
                                        for (int i = 0; i < vs.Length - 1; i++)
                                        {
                                            string[] items = vs[i].Split(':');
                                            variable += items[0] + ":" + items[1] + "<br/>";
                                            variablename += items[0] + "|";
                                            variablevalue += items[1] + "|";
                                        }
                                    }

                                    double mainstock = PJUtils.TotalProductQuantityInstock(1, SKU);

                                    QuantityInstock = total;
                                    QuantityInstockString = string.Format("{0:N0}", total);

                                    if (productvariable.Image != null)
                                    {
                                        ProductImage = "<img src=\"" + productvariable.Image + "\" />";
                                    }
                                    else
                                    {
                                        ProductImage = "<img src=\"/App_Themes/Ann/image/placeholder.png\" />";
                                    }

                                    ProductImageOrigin = productvariable.Image;

                                    ProductVariable = variable;
                                    var product1 = ProductController.GetByID(Convert.ToInt32(productvariable.ProductID));
                                    if (product1 != null)
                                        ProductName = product1.ProductTitle;

                                    QuantityMainInstock = mainstock;
                                    QuantityMainInstockString = string.Format("{0:N0}", mainstock);
                                    ProductVariableSave = item.ProductVariableDescrition;

                                    ProductVariableName = variablename;
                                    ProductVariableValue = variablevalue;
                                }
                            }
                            orderitem++;
                            html += "<tr class=\"product-result\" data-orderdetailid=\"" + item.ID + "\" data-giabansi=\"" + Giabansi + "\" data-giabanle=\"" + Giabanle + "\" " +
                                                "data-quantityinstock=\"" + QuantityInstock + "\" data-productimageorigin=\"" + ProductImageOrigin + "\" " +
                                                "data-productvariable=\"" + ProductVariable + "\" data-productname=\"" + ProductName + "\" " +
                                                "data-sku=\"" + SKU + "\" data-producttype=\"" + ProductType + "\" data-id=\"" + PID + "\" " +
                                                "data-productnariablename=\"" + ProductVariableName + "\" " +
                                                "data-productvariablevalue =\"" + ProductVariableValue + "\" " +
                                                "data-productvariablesave =\"" + ProductVariableSave + "\" " +
                                                "data-quantitymaininstock=\"" + QuantityMainInstock + "\">";
                            html += "   <td class=\"order-item\">" + orderitem + "";
                            html += "   <td class=\"image-item\">" + ProductImage + "";
                            html += "   <td class=\"name-item\">" + ProductName + "</td>";
                            html += "   <td class=\"sku-item\">" + SKU + "</td>";
                            html += "   <td class=\"variable-item\">" + ProductVariable + "</td>";
                            html += "   <td class=\"price-item gia-san-pham\" data-price=\"" + ItemPrice + "\">" + string.Format("{0:N0}", ItemPrice) + "</td>";
                            //html += "   <td>" + QuantityMainInstockString + "</td>";
                            html += "   <td class=\"quantity-item soluong\">" + QuantityInstockString + "</td>";
                            html += "   <td class=\"quantity-item\"><input data-quantity=\"" + item.Quantity + "\" value=\"" + item.Quantity + "\" type=\"text\" min=\"0\" max=\"" + QuantityInstock + "\" class=\"form-control in-quanlity\" value=\"1\" onkeyup=\"checkQuantiy1($(this))\" onkeypress='return event.charCode >= 48 && event.charCode <= 57'/></td>";
                            int k = Convert.ToInt32(ItemPrice) * Convert.ToInt32(item.Quantity);
                            html += "<td class=\"total-item totalprice-view\">" + string.Format("{0:N0}", k) + "</td>";
                            html += "   <td class=\"trash-item\"><a href=\"javascript:;\" class=\"link-btn\" onclick=\"deleteRow($(this))\"><i class=\"fa fa-trash\"></i></a>    </td>";

                            html += "</tr>";


                            Print += " <tr>";
                            t++;
                            Print += "<td>" + t + "</td>";
                            Print += "<td>" + SKU + " - " + ProductName + " - " + ProductVariableSave.Replace("|", ", ") + "</td> ";
                            Print += "<td>" + item.Quantity + "</td>";
                            Print += "<td>" + string.Format("{0:N0}", ItemPrice) + "</td>";

                            Print += "<td> " + string.Format("{0:N0}", k) + "</td>";

                            Print += "</tr>";

                        }
                        ltrProducts.Text = html;

                    }
                    #endregion
                    #region HiddenField
                    hdfCheckCustomer.Value = "1";
                    hdfOrderType.Value = customerType.ToString();
                    hdfTotalPrice.Value = totalPrice.ToString();
                    hdfTotalPriceNotDiscount.Value = totalPriceNotDiscount.ToString();
                    hdfPaymentStatus.Value = paymentStatus.ToString();
                    hdfExcuteStatus.Value = excuteStatus.ToString();
                    hdftotal.Value = ProductQuantity.ToString();

                    #endregion
                    ddlPaymentStatus.SelectedValue = paymentStatus.ToString();
                    ddlExcuteStatus.SelectedValue = excuteStatus.ToString();
                    ddlPaymentType.SelectedValue = paymenttype.ToString();
                    ddlShippingType.SelectedValue = shipping.ToString();
                    ltrProductQuantity.Text = string.Format("{0:N0}", ProductQuantity) + " sản phẩm";
                    ltrTotalNotDiscount.Text = string.Format("{0:N0}", Convert.ToDouble(order.TotalPriceNotDiscount));
                    ltrTotalprice.Text = string.Format("{0:N0}", Convert.ToDouble(order.TotalPrice));
                    pDiscount.Value = order.DiscountPerProduct;
                    pFeeShip.Value = Convert.ToDouble(order.FeeShipping);
                    ltrTotalAfterCK.Text = string.Format("{0:N0}", (Convert.ToDouble(order.TotalPriceNotDiscount) - Convert.ToDouble(order.TotalDiscount)));
                    ltrOrderID.Text = ID.ToString();
                    ltrCreateBy.Text = order.CreatedBy;
                    ltrCreateDate.Text = order.CreatedDate.ToString();
                    ltrDateDone.Text = order.DateDone.ToString();

                    ltrOrderQuantity.Text = ProductQuantity.ToString();
                    ltrOrderTotalPrice.Text = string.Format("{0:N0}", Convert.ToDouble(order.TotalPrice));
                    ltrOrderStatus.Text = PJUtils.OrderExcute(Convert.ToInt32(order.ExcuteStatus));
                    ltrOrderType.Text = PJUtils.OrderType(Convert.ToInt32(order.OrderType));
                    ltrOrderNote.Text = order.OrderNote;

                    #region Data Print

                    //N.a
                    string productPrint = "";
                    string shtml = "";

                    productPrint += " <div class=\"body\">";
                    productPrint += " <div class=\"table-1\">";
                    productPrint += " <table>";
                    productPrint += "<colgroup >";
                    productPrint += "<col class=\"col-left\"/>";
                    productPrint += "<col class=\"col-right\"/>";
                    productPrint += "</colgroup>";
                    productPrint += " <tbody>";
                    productPrint += " <tr>";
                    productPrint += " <td>Mã đơn hàng</td>";
                    productPrint += " <td>" + order.ID + "</td>";
                    productPrint += " </tr>";
                    productPrint += "  <tr>";
                    productPrint += "  <td>Ngày tạo</td>";
                    string date = string.Format("{0:dd/MM/yyyy HH:mm}", order.CreatedDate);
                    productPrint += " <td>" + date + "</td>";
                    productPrint += "</tr>";
                    productPrint += "<tr>";
                    productPrint += "<td>Ngày hoàn tất</td>";
                    if (!string.IsNullOrEmpty(order.DateDone.ToString()))
                    {
                        string datedone = string.Format("{0:dd/MM/yyyy HH:mm}", order.DateDone);
                        productPrint += "<td>" + datedone + "</td>";
                    }
                    else
                        productPrint += "<td></td>";
                    productPrint += " </tr>";
                    productPrint += " <tr>";
                    productPrint += "<td>Loại đơn</td>";
                    if (order.OrderType == 1)
                        productPrint += "<td>Đơn lẻ</td>";
                    if (order.OrderType == 2)
                        productPrint += "  <td>Đơn sỉ</td>";
                    productPrint += "</tr>";
                    productPrint += " <tr>";
                    productPrint += " <td>Nhân viên</td>";
                    productPrint += " <td>" + order.CreatedBy + "</td>";
                    productPrint += "</tr>";
                    productPrint += "<tr>";
                    productPrint += "  <td>Khách hàng</td>";
                    productPrint += "  <td>" + order.CustomerName + "</td>";
                    productPrint += " </tr>";
                    productPrint += "<tr>";
                    productPrint += " <td>Điện thoại</td>";
                    productPrint += " <td>" + order.CustomerPhone + "</td>";
                    productPrint += "</tr>";
                    productPrint += "<tr>";
                    productPrint += "   <td>Ghi chú</td>";
                    productPrint += "<td>" + order.OrderNote + "</td>";
                    productPrint += "</tr>";
                    productPrint += " </tbody>";
                    productPrint += "</table>";
                    productPrint += "         </div>";

                    productPrint += "<div class=\"table-2\">";
                    productPrint += " <table>";
                    productPrint += " <colgroup>";
                    productPrint += "<col class=\"stt\" />";
                    productPrint += "<col class=\"sanpham\" />";
                    productPrint += "<col class=\"soluong\" />";
                    productPrint += "<col class=\"gia\" />";
                    productPrint += "<col class=\"tong\"/>";
                    productPrint += "</colgroup>";
                    productPrint += "<thead>";
                    productPrint += " <th>#</th>";
                    productPrint += "<th>Sản phẩm</th>";
                    productPrint += "<th>Số lượng</th>";
                    productPrint += "<th>Giá</th>";
                    productPrint += "<th>Tổng</th>";
                    productPrint += " </thead>";
                    productPrint += "<tbody>";
                    productPrint += Print;
                    productPrint += "<tr>";
                    productPrint += "<td colspan =\"4\" > Số lượng </td>";
                    productPrint += "<td>" + ProductQuantity + "</td>";
                    productPrint += "</tr>";
                    productPrint += " <tr>";
                    productPrint += " <td colspan =\"4\"> Thành tiền </td>";
                    productPrint += " <td>" + string.Format("{0:N0}", Convert.ToDouble(order.TotalPriceNotDiscount)) + "</td>";
                    productPrint += " </tr>";
                    productPrint += " <tr>";
                    productPrint += "  <td colspan =\"4\"> Chiết khấu mỗi cái </td>";
                    productPrint += " <td>" + string.Format("{0:N0}", Convert.ToDouble(pDiscount.Value)) + "</td>";
                    productPrint += " </tr>";
                    productPrint += "<tr>";
                    productPrint += " <td colspan =\"4\" > Sau chiết khấu</ td >";

                    productPrint += " <td>" + string.Format("{0:N0}", Convert.ToDouble((Convert.ToDouble(order.TotalPriceNotDiscount) - (Convert.ToDouble(order.DiscountPerProduct) * Convert.ToDouble(ProductQuantity))))) + "</ td >";
                    productPrint += " </tr>";
                    productPrint += " <tr>";
                    productPrint += " <td colspan =\"4\" > Phí vận chuyển</ td >";
                    productPrint += " <td>" + string.Format("{0:N0}", Convert.ToDouble(order.FeeShipping)) + "</ td >";
                    productPrint += " </tr>";
                    productPrint += " <tr>";
                    productPrint += "<td class=\"strong\" colspan=\"4\">Tổng cộng</td>";
                    productPrint += " <td class=\"strong\">" + string.Format("{0:N0}", Convert.ToDouble(totalPrice)) + "</td>";
                    productPrint += " </tr>";
                    productPrint += "</tbody>";
                    productPrint += " </table>";
                    productPrint += "</div>";
                    productPrint += "         </div>";


                    string address = "K4C Bửu Long, Phường 15, Quận 10, TP. HCM";
                    string phone = "";
                    var agent = AgentController.GetByID(AgentID);
                    if (agent != null)
                    {
                        address = agent.AgentAddress;
                        phone = agent.AgentPhone;
                    }

                    string dateOrder = string.Format("{0:dd/MM/yyyy HH:mm}", order.DateDone);




                    shtml += "<div class=\"hoadon\">";
                    shtml += "<div class=\"all\">";
                    shtml += "<div class=\"head\">";
                    shtml += "     <div class=\"logo\"><div class=\"img\"><img src=\"App_Themes/Ann/image/logo.png\" alt=\"\" /></div></div>";
                    shtml += "<div class=\"info\">";
                    shtml += "<div class=\"ct\">";
                    shtml += "<div class=\"ct-title\">Địa chỉ:</div>";
                    shtml += "<div class=\"ct-detail\">" + address + "</div>";
                    shtml += "<div class=\"ct\">";
                    shtml += "<div class=\"ct-title\">Điện thoại/ Zalo:</div>";
                    shtml += "<div class=\"ct-detail\">";

                    shtml += "<a href = \"tel:+\" >" + phone + "</a></div>";

                    shtml += "<div class=\"ct\">";
                    shtml += "<div class=\"ct-title\">Facebook:</div>";
                    shtml += "<div class=\"ct-detail\">";
                    shtml += "<a href =\"https://facebook.com/bosiquanao.net\" target=\"_blank\" >https://facebook.com/bosiquanao.net</a>";
                    shtml += "</div>";
                    shtml += "</div>";
                    shtml += "<div class=\"ct\">";
                    shtml += "<div class=\"ct-title\">Website:</div>";
                    shtml += "<div class=\"ct-detail\">";
                    shtml += "<a href =\"\">ann.com.vn</a> ";
                    shtml += "</div> ";
                    shtml += "</div> ";
                    shtml += "</div> ";
                    shtml += "</div>";
                    shtml += "</div>";

                    shtml += productPrint;

                    shtml += "<div class=\"footer\"><h1> Cảm ơn quý khách </h ></div> ";
                    shtml += "</div>";

                    shtml += "</div>";

                    ltrPrintOrder.Text = shtml;
                    #endregion
                }
            }
        }

        [WebMethod]
        public static string getProduct(string textsearch)
        {
            List<ProductGetOut> ps = new List<ProductGetOut>();
            string username = HttpContext.Current.Session["userLoginSystem"].ToString();
            var acc = AccountController.GetByUsername(username);
            if (acc != null)
            {
                int AgentID = Convert.ToInt32(acc.AgentID);
                var products = ProductController.GetByTextSearchIsHidden(textsearch.Trim(), false);
                if (products.Count > 0)
                {
                    foreach (var product in products)
                    {
                        var productvariable = ProductVariableController.GetProductID(product.ID);
                        if (productvariable.Count > 0)
                        {
                            foreach (var pv in productvariable)
                            {
                                string SKU = pv.SKU.Trim().ToUpper();
                                var check = InOutProductVariableController.GetBySKU(AgentID, SKU);
                                if (check.Count > 0)
                                {
                                    double total = PJUtils.TotalProductQuantityInstock(AgentID, SKU);
                                    var variables = ProductVariableValueController.GetByProductVariableID(pv.ID);
                                    if (variables.Count > 0)
                                    {
                                        string variablename = "";
                                        string variablevalue = "";
                                        string variable = "";
                                        string variablesave = "";
                                        foreach (var v in variables)
                                        {
                                            variable += v.VariableName.Trim() + ": " + v.VariableValue.Trim() + "<br/>";
                                            variablesave += v.VariableName.Trim() + ": " + v.VariableValue.Trim() + "|";
                                            variablename += v.VariableName.Trim() + "|";
                                            variablevalue += v.VariableValue.Trim() + "|";
                                        }
                                        var mainstock = PJUtils.TotalProductQuantityInstock(1, SKU);
                                        ProductGetOut p = new ProductGetOut();
                                        p.ID = pv.ID;
                                        p.ProductName = product.ProductTitle;
                                        p.ProductVariable = variable;
                                        p.ProductVariableSave = variablesave;
                                        p.ProductVariableName = variablename;

                                        p.ProductVariableValue = variablevalue;
                                        p.ProductType = 2;

                                        if (p.ProductImage == null)
                                        {
                                            p.ProductImage = "<img src=\"" + pv.Image + "\" />";
                                        }
                                        else
                                        {
                                            p.ProductImage = "<img src=\"/App_Themes/Ann/image/placeholder.png\" />";
                                        }

                                        p.ProductImageOrigin = pv.Image;
                                        p.QuantityMainInstock = mainstock;
                                        p.QuantityMainInstockString = string.Format("{0:N0}", mainstock);
                                        p.QuantityInstock = total;
                                        p.QuantityInstockString = string.Format("{0:N0}", total);
                                        p.SKU = SKU;
                                        p.Giabansi = Convert.ToDouble(pv.Regular_Price);
                                        p.stringGiabansi = string.Format("{0:N0}", pv.Regular_Price);
                                        p.Giabanle = Convert.ToDouble(pv.RetailPrice);
                                        p.stringGiabanle = string.Format("{0:N0}", pv.RetailPrice);
                                        ps.Add(p);
                                    }
                                }

                            }
                        }
                        else
                        {
                            string SKU = product.ProductSKU.Trim().ToUpper();
                            var check = InOutProductVariableController.GetBySKU(AgentID, SKU);
                            if (check.Count > 0)
                            {
                                double total = PJUtils.TotalProductQuantityInstock(AgentID, SKU);
                                string variablename = "";
                                string variablevalue = "";
                                string variable = "";

                                double mainstock = PJUtils.TotalProductQuantityInstock(1, SKU);

                                ProductGetOut p = new ProductGetOut();
                                p.ID = product.ID;
                                p.ProductName = product.ProductTitle;
                                p.ProductVariable = variable;
                                p.ProductVariableSave = "";
                                p.ProductVariableName = variablename;
                                p.ProductVariableValue = variablevalue;
                                p.ProductType = 1;

                                var img = ProductImageController.GetFirstByProductID(product.ID);
                                if (img != null)
                                {
                                    p.ProductImage = "<img src=\"" + img.ProductImage + "\" />";
                                    p.ProductImageOrigin = img.ProductImage;
                                }
                                else
                                {
                                    p.ProductImage = "<img src=\"/App_Themes/Ann/image/placeholder.png\" />";
                                    p.ProductImageOrigin = "";
                                }

                                p.SKU = SKU;
                                p.QuantityMainInstock = mainstock;
                                p.QuantityMainInstockString = string.Format("{0:N0}", mainstock);
                                p.QuantityInstock = total;
                                p.QuantityInstockString = string.Format("{0:N0}", total);
                                p.Giabansi = Convert.ToDouble(product.Regular_Price);
                                p.stringGiabansi = string.Format("{0:N0}", product.Regular_Price);
                                p.Giabanle = Convert.ToDouble(product.Retail_Price);
                                p.stringGiabanle = string.Format("{0:N0}", product.Retail_Price);
                                ps.Add(p);
                            }
                        }
                    }
                }
                else
                {
                    var product = ProductController.GetBySKU(textsearch.Trim());
                    if (product != null)
                    {
                        var productvariable = ProductVariableController.GetProductID(product.ID);
                        if (productvariable.Count > 0)
                        {
                            foreach (var pv in productvariable)
                            {
                                string SKU = pv.SKU.Trim().ToUpper();
                                var check = InOutProductVariableController.GetBySKU(AgentID, SKU);
                                if (check.Count > 0)
                                {
                                    double total = PJUtils.TotalProductQuantityInstock(AgentID, SKU);
                                    var variables = ProductVariableValueController.GetByProductVariableID(pv.ID);
                                    if (variables.Count > 0)
                                    {
                                        string variablename = "";
                                        string variablevalue = "";
                                        string variable = "";
                                        string variablesave = "";
                                        foreach (var v in variables)
                                        {
                                            variable += v.VariableName.Trim() + ": " + v.VariableValue.Trim() + "<br/>";
                                            variablesave += v.VariableName.Trim() + ": " + v.VariableValue.Trim() + "|";
                                            variablename += v.VariableName.Trim() + "|";
                                            variablevalue += v.VariableValue.Trim() + "|";
                                        }
                                        var mainstock = PJUtils.TotalProductQuantityInstock(1, SKU);
                                        ProductGetOut p = new ProductGetOut();
                                        p.ID = pv.ID;
                                        p.ProductName = product.ProductTitle;
                                        p.ProductVariable = variable;
                                        p.ProductVariableSave = variablesave;
                                        p.ProductVariableName = variablename;

                                        p.ProductVariableValue = variablevalue;
                                        p.ProductType = 2;

                                        if (p.ProductImage == null)
                                        {
                                            p.ProductImage = "<img src=\"" + pv.Image + "\" />";
                                        }
                                        else
                                        {
                                            p.ProductImage = "<img src=\"/App_Themes/Ann/image/placeholder.png\" />";
                                        }

                                        p.ProductImageOrigin = pv.Image;
                                        p.QuantityMainInstock = mainstock;
                                        p.QuantityMainInstockString = string.Format("{0:N0}", mainstock);
                                        p.QuantityInstock = total;
                                        p.QuantityInstockString = string.Format("{0:N0}", total);
                                        p.SKU = SKU;
                                        p.Giabansi = Convert.ToDouble(pv.Regular_Price);
                                        p.stringGiabansi = string.Format("{0:N0}", pv.Regular_Price);
                                        p.Giabanle = Convert.ToDouble(pv.RetailPrice);
                                        p.stringGiabanle = string.Format("{0:N0}", pv.RetailPrice);
                                        ps.Add(p);
                                    }
                                }

                            }
                        }
                        else
                        {
                            string SKU = product.ProductSKU.Trim().ToUpper();
                            var check = InOutProductVariableController.GetBySKU(AgentID, SKU);
                            if (check.Count > 0)
                            {
                                double total = PJUtils.TotalProductQuantityInstock(AgentID, SKU);
                                string variablename = "";
                                string variablevalue = "";
                                string variable = "";

                                double mainstock = PJUtils.TotalProductQuantityInstock(1, SKU);

                                ProductGetOut p = new ProductGetOut();
                                p.ID = product.ID;
                                p.ProductName = product.ProductTitle;
                                p.ProductVariable = variable;
                                p.ProductVariableSave = "";
                                p.ProductVariableName = variablename;
                                p.ProductVariableValue = variablevalue;
                                p.ProductType = 1;

                                var img = ProductImageController.GetFirstByProductID(product.ID);
                                if (img != null)
                                {
                                    p.ProductImage = "<img src=\"" + img.ProductImage + "\"  />";
                                    p.ProductImageOrigin = img.ProductImage;
                                }
                                else
                                {
                                    p.ProductImage = "<img src=\"/App_Themes/Ann/image/placeholder.png\" />";
                                    p.ProductImageOrigin = "";
                                }

                                p.SKU = SKU;
                                p.QuantityMainInstock = mainstock;
                                p.QuantityMainInstockString = string.Format("{0:N0}", mainstock);
                                p.QuantityInstock = total;
                                p.QuantityInstockString = string.Format("{0:N0}", total);
                                p.Giabansi = Convert.ToDouble(product.Regular_Price);
                                p.stringGiabansi = string.Format("{0:N0}", product.Regular_Price);
                                p.Giabanle = Convert.ToDouble(product.Retail_Price);
                                p.stringGiabanle = string.Format("{0:N0}", product.Retail_Price);
                                ps.Add(p);
                            }

                        }
                    }
                    else
                    {
                        var productvariable = ProductVariableController.GetBySKU(textsearch);
                        if (productvariable != null)
                        {
                            string SKU = productvariable.SKU.Trim().ToUpper();
                            var check = InOutProductVariableController.GetBySKU(AgentID, SKU);
                            if (check.Count > 0)
                            {
                                double total = PJUtils.TotalProductQuantityInstock(AgentID, SKU);
                                var variables = ProductVariableValueController.GetByProductVariableID(productvariable.ID);

                                if (variables.Count > 0)
                                {
                                    string variablename = "";
                                    string variablevalue = "";
                                    string variable = "";
                                    string variablesave = "";

                                    foreach (var v in variables)
                                    {
                                        variable += v.VariableName + ": " + v.VariableValue + "<br/>";
                                        variablesave += v.VariableName.Trim() + ": " + v.VariableValue.Trim() + "|";
                                        variablename += v.VariableName + "|";
                                        variablevalue += v.VariableValue + "|";
                                    }
                                    double mainstock = PJUtils.TotalProductQuantityInstock(1, SKU);

                                    ProductGetOut p = new ProductGetOut();
                                    p.ID = productvariable.ID;
                                    var product1 = ProductController.GetByID(Convert.ToInt32(productvariable.ProductID));
                                    if (product1 != null)
                                        p.ProductName = product1.ProductTitle;
                                    p.ProductVariable = variable;
                                    p.ProductVariableSave = variablesave;
                                    p.ProductVariableName = variablename;
                                    p.ProductVariableValue = variablevalue;
                                    p.ProductType = 2;
                                    
                                    if (p.ProductImage == null)
                                    {
                                        p.ProductImage = "<img src=\"" + productvariable.Image + "\" />";
                                    }
                                    else
                                    {
                                        p.ProductImage = "<img src=\"/App_Themes/Ann/image/placeholder.png\" />";
                                    }

                                    p.ProductImageOrigin = productvariable.Image;
                                    p.SKU = productvariable.SKU.Trim().ToUpper();
                                    p.QuantityMainInstock = mainstock;
                                    p.QuantityMainInstockString = string.Format("{0:N0}", mainstock);
                                    p.QuantityInstock = total;
                                    p.QuantityInstockString = string.Format("{0:N0}", total);
                                    p.Giabansi = Convert.ToDouble(productvariable.Regular_Price);
                                    p.stringGiabansi = string.Format("{0:N0}", productvariable.Regular_Price);
                                    p.Giabanle = Convert.ToDouble(productvariable.RetailPrice);
                                    p.stringGiabanle = string.Format("{0:N0}", productvariable.RetailPrice);
                                    ps.Add(p);
                                }
                            }
                        }
                    }
                }
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(ps);
        }
        [WebMethod]
        public static string findcustomer(string phone)
        {
            var customer = CustomerController.GetByPhone(phone);
            if (customer != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(customer);
            }
            else
            {
                return "0";
            }

        }
        [WebMethod]
        public static string gethightdiscount(int ID)
        {
            var d = DiscountCustomerController.getbyCustID(ID);
            if (d.Count > 0)
            {
                var da = d[0].DiscountAmount;
                return da.ToString();
            }
            else
            {
                return "0";
            }

        }
        [WebMethod]
        public static string findReturnOrder(string order, string remove)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (remove.ToInt() == 0)
            {
                var or = RefundGoodController.GetOrderByID(order.ToInt());
                if (or != null)
                {
                    HttpContext.Current.Session["refund"] = or.ID + "|" + or.TotalPrice;
                    return serializer.Serialize(or);
                }
                else
                {
                    return serializer.Serialize(null);
                }
            }
            else
            {
                HttpContext.Current.Session["refund"] = 0;
                return serializer.Serialize(null);
            }
        }
        [WebMethod]
        public static string getCust(string textsearch)
        {
            var customer = CustomerController.Find(textsearch);
            if (customer != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(customer);
            }
            else
            {
                return "0";
            }

        }

        [WebMethod]
        public static string getCustomerDetail(int ID)
        {
            var customer = CustomerController.GetByID(ID);
            if (customer != null)
            {

                var ci = new CustomerInfoWithDiscount();
                if (!string.IsNullOrEmpty(customer.ProvinceID.ToString()) && customer.ProvinceID.ToString() != "0")
                {
                    var pro = ProvinceController.GetByID(customer.ProvinceID.Value);
                    ci.ProvinceName = pro.ProvinceName;
                }
                else
                {
                    ci.ProvinceName = null;
                }
                ci.Customer = customer;
                //ci.CreatedDate = string.Format("{0:dd/MM/yyyy}", customer.CreatedDate);
                ci.CreatedDate = customer.CreatedDate.ToString();
                List<DiscountCustomerGet> dc = new List<DiscountCustomerGet>();
                var d = DiscountCustomerController.getbyCustID(ID);
                if (d.Count > 0)
                {
                    dc = d;
                }
                ci.AllDiscount = dc;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(ci);
            }
            else
            {
                return "null";
            }
        }

        [WebMethod]
        public static string UpdateStatus()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string username = HttpContext.Current.Session["userLoginSystem"].ToString();
            var acc = AccountController.GetByUsername(username);
            string orderid = HttpContext.Current.Session["odid"].ToString();
            var order = OrderController.GetByID(orderid.ToInt());
            if (order != null)
            {
                int i = OrderController.UpdateExcuteStatus(order.ID, acc.Username);
                if (i > 0)
                {
                    return serializer.Serialize(i);
                }
                else
                {
                    return serializer.Serialize(null);
                }
            }
            else
                return serializer.Serialize(null);

        }

        public class ProductGetOut
        {
            public int ID { get; set; }
            public string ProductName { get; set; }
            public string ProductVariable { get; set; }
            public string ProductVariableSave { get; set; }
            public string ProductVariableName { get; set; }
            public string ProductVariableValue { get; set; }
            public int ProductType { get; set; }
            public string ProductImage { get; set; }
            public string ProductImageOrigin { get; set; }
            public string QuantityMainInstockString { get; set; }
            public double QuantityMainInstock { get; set; }
            public string QuantityInstockString { get; set; }
            public double QuantityInstock { get; set; }
            public string SKU { get; set; }
            public double Giabanle { get; set; }
            public string stringGiabanle { get; set; }
            public double Giabansi { get; set; }
            public string stringGiabansi { get; set; }
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {

        }


        [WebMethod]
        public static string Delete(string list)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            DateTime currentDate = DateTime.Now;
            string username = HttpContext.Current.Session["userLoginSystem"].ToString();
            var acc = AccountController.GetByUsername(username);
            string[] listdelete = list.Split(';');
            if (listdelete != null)
            {
                for (int i = 0; i < listdelete.Length - 1; i++)
                {
                    string[] ld = listdelete[i].Split('*');
                    if (ld != null)
                    {
                        int agentid = Convert.ToInt32(acc.AgentID);
                        int orderid = ld[0].ToInt();
                        string ordersku = ld[1];
                        double quantitynew = ld[2].ToInt();
                        string productvariablename = ld[3];
                        string productvariablevalue = ld[4];
                        string productimage = ld[5];
                        string productname = ld[6];
                        string productvariable = ld[7];
                        int id = ld[8].ToInt();
                        string ord = OrderDetailController.Delete(orderid, ordersku);


                        var parent = ProductVariableController.GetBySKU(ordersku);
                        if (parent != null)
                        {
                            var t = InOutProductVariableController.Insert(agentid, 0, 0, productvariablename, productvariablevalue, quantitynew, 0, 1, false, 1, "Xóa sản phẩm khi chỉnh sửa đơn", orderid,
                        0, 10, productname, ordersku, productimage, productvariable, currentDate, username, 0, Convert.ToInt32(parent.ProductID));
                            return serializer.Serialize(t);
                        }
                        else
                        {
                            var t = InOutProductVariableController.Insert(agentid, 0, 0, productvariablename, productvariablevalue, quantitynew, 0, 1, false, 1, "Xóa sản phẩm khi chỉnh sửa đơn", orderid,
                        0, 10, productname, ordersku, productimage, productvariable, currentDate, username, 0, Convert.ToInt32(id));
                            return serializer.Serialize(t);
                        }

                    }
                }
            }
            return serializer.Serialize(null);
        }

        protected void btnOrder_Click(object sender, EventArgs e)
        {
            DateTime currentDate = DateTime.Now;
            string username = Session["userLoginSystem"].ToString();
            var acc = AccountController.GetByUsername(username);

            if (acc != null)
            {
                if (acc.RoleID == 0 || acc.RoleID == 2)
                {
                    int OrderID = ViewState["ID"].ToString().ToInt(0);
                    if (OrderID > 0)
                    {
                        var order = OrderController.GetByID(OrderID);
                        if (order != null)
                        {
                            int AgentID = Convert.ToInt32(acc.AgentID);
                            int OrderType = hdfOrderType.Value.ToInt();
                            string AdditionFee = "0";
                            string DisCount = "0";
                            int CustomerID = 0;
                            int checkCustomer = hdfCheckCustomer.Value.ToInt();

                            string CustomerPhone = txtPhone.Text.Trim();
                            string CustomerName = txtFullname.Text.Trim();
                            string Nick = txtNick.Text.Trim();
                            string CustomerAddress = txtAddress.Text.Trim();
                            string Zalo = txtZalo.Text.Trim();
                            string Facebook = txtFacebook.Text.Trim();
                            if (checkCustomer == 0)
                            {
                                var checkphone = CustomerController.GetByPhone(CustomerPhone);
                                if (checkphone != null)
                                {
                                    CustomerID = checkphone.ID;
                                }
                                else
                                {
                                    string kq = CustomerController.Insert(CustomerName, CustomerPhone, CustomerAddress, "", 0, 0, currentDate, username, false, Zalo, Facebook, "", "", Nick);
                                    if (kq.ToInt(0) > 0)
                                    {
                                        CustomerID = kq.ToInt(0);
                                    }
                                }
                            }
                            else
                            {
                                var checkphone = CustomerController.GetByPhone(CustomerPhone);
                                if (checkphone != null)
                                {
                                    CustomerID = checkphone.ID;
                                }
                            }
                            string totalPrice = hdfTotalPrice.Value.ToString();
                            double GuestPaid = Convert.ToDouble(pGuestPaid.Value);
                            double GuestChange = Convert.ToDouble(totalPrice) - GuestPaid;

                            string totalPriceNotDiscount = hdfTotalPriceNotDiscount.Value;
                            int PaymentStatusOld = Convert.ToInt32(order.PaymentStatus);
                            int ExcuteStatusOld = Convert.ToInt32(order.ExcuteStatus);
                            int PaymentStatus = ddlPaymentStatus.SelectedValue.ToInt(0);
                            int ExcuteStatus = ddlExcuteStatus.SelectedValue.ToInt(0);
                            int PaymentType = ddlPaymentType.SelectedValue.ToInt(0);
                            int ShippingType = ddlShippingType.SelectedValue.ToInt(0);
                            string OrderNote = txtOrderNote.Text;

                            double DiscountPerProduct = 0;
                            if (!string.IsNullOrEmpty(hdfDiscountAmount.Value))
                                DiscountPerProduct = Convert.ToDouble(hdfDiscountAmount.Value);
                            string sl = hdftotal.Value;
                            if (!string.IsNullOrEmpty(hdfTotalQuantity.Value))
                            {
                                sl = hdfTotalQuantity.Value;
                            }
                            double TotalDiscount = Convert.ToDouble(pDiscount.Value) * Convert.ToDouble(sl);
                            string FeeShipping = pFeeShip.Value.ToString();

                            string datedone = "";
                            //string ret = OrderController.Update(OrderID, OrderType, AdditionFee, DisCount, CustomerID, CustomerName, CustomerPhone,
                            //    CustomerAddress, CustomerEmail, totalPrice, totalPriceNotDiscount, PaymentStatus, ExcuteStatus, currentDate, username,
                            //    DiscountPerProduct, TotalDiscount, FeeShipping);
                            if (ExcuteStatus == 2)
                            {
                                datedone = DateTime.Now.ToString();
                            }
                            string ret = OrderController.UpdateOnSystem(OrderID, OrderType, AdditionFee, DisCount, CustomerID, CustomerName, CustomerPhone,
                                CustomerAddress, "", totalPrice, totalPriceNotDiscount, PaymentStatus, ExcuteStatus, currentDate, username,
                                Convert.ToDouble(pDiscount.Value), TotalDiscount, FeeShipping, GuestPaid, GuestChange, PaymentType, ShippingType, OrderNote, datedone);
                            double totalQuantity = 0;
                            if (OrderID > 0)
                            {
                                string list = hdfListProduct.Value;
                                string[] items = list.Split(';');
                                if (items.Length - 1 > 0)
                                {
                                    for (int i = 0; i < items.Length - 1; i++)
                                    {
                                        var item = items[i];
                                        string[] itemValue = item.Split(',');

                                        int ProductID = 0;
                                        int ProductVariableID = 0;

                                        int ID = itemValue[0].ToInt();
                                        int parentID = 0;
                                        var parent = ProductVariableController.GetByID(ID);
                                        if (parent != null)
                                        {
                                            parentID = Convert.ToInt32(parent.ProductID);
                                        }
                                        string SKU = itemValue[1];
                                        int producttype = itemValue[2].ToInt();
                                        if (producttype == 1)
                                        {
                                            ProductID = ID;
                                            ProductVariableID = 0;
                                        }
                                        else
                                        {
                                            ProductID = 0;
                                            ProductVariableID = ID;
                                        }

                                        string ProductVariableName = itemValue[3];
                                        string ProductVariableValue = itemValue[4];
                                        double Quantity = Convert.ToDouble(itemValue[5]);
                                        string ProductName = itemValue[6];
                                        string ProductImageOrigin = itemValue[7];
                                        string ProductVariable = itemValue[8];
                                        string Price = itemValue[9];
                                        string ProductVariableSave = itemValue[10];
                                        int OrderDetailID = itemValue[11].ToInt(0);
                                        if (OrderDetailID > 0)
                                        {
                                            var od = OrderDetailController.GetByID(OrderDetailID);
                                            if (od != null)
                                            {
                                                if (od.IsCount == true)
                                                {
                                                    double quantityOld = Convert.ToDouble(od.Quantity);
                                                    if (producttype == 1)
                                                    {
                                                        if (quantityOld > Quantity)
                                                        {
                                                            //công vô kho
                                                            double quantitynew = quantityOld - Quantity;
                                                            InOutProductVariableController.Insert(AgentID, ID, 0, "", "", quantitynew, 0, 1, false, 1, "Nhập vô khi chỉnh sửa đơn", OrderID, 0, 4, ProductName, SKU, ProductImageOrigin, ProductVariable, currentDate, username, 0, parentID);
                                                        }
                                                        else if (quantityOld < Quantity)
                                                        {
                                                            //trừ tiếp trong kho
                                                            double quantitynew = Quantity - quantityOld;
                                                            InOutProductVariableController.Insert(AgentID, ID, 0, "", "", quantitynew, 0, 2, false, 1, "Xuất khi chỉnh sửa đơn", OrderID, 0, 3, ProductName, SKU, ProductImageOrigin, ProductVariable, currentDate, username, 0, parentID);
                                                        }

                                                    }
                                                    else
                                                    {

                                                        string parentSKU = "";
                                                        var productV = ProductVariableController.GetByID(ID);
                                                        if (productV != null)
                                                            parentSKU = productV.ParentSKU;
                                                        if (!string.IsNullOrEmpty(parentSKU))
                                                        {
                                                            var product = ProductController.GetBySKU(parentSKU);
                                                            if (product != null)
                                                                parentID = product.ID;
                                                        }
                                                        if (quantityOld > Quantity)
                                                        {
                                                            //công vô kho
                                                            double quantitynew = quantityOld - Quantity;
                                                            InOutProductVariableController.Insert(AgentID, 0, ID, ProductVariableName, ProductVariableValue, quantitynew, 0, 1, false, 2, "Nhập vô khi chỉnh sửa đơn", OrderID, 0, 4, ProductName, SKU, ProductImageOrigin, ProductVariable, currentDate, username, 0, parentID);
                                                        }
                                                        else if (quantityOld < Quantity)
                                                        {
                                                            //trừ tiếp trong kho
                                                            double quantitynew = Quantity - quantityOld;
                                                            InOutProductVariableController.Insert(AgentID, 0, ID, ProductVariableName, ProductVariableValue, quantitynew, 0, 2, false, 2, "Xuất khi chỉnh sửa đơn", OrderID, 0, 3, ProductName, SKU, ProductImageOrigin, ProductVariable, currentDate, username, 0, parentID);
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    if (ExcuteStatus == 2 && PaymentStatus == 3)
                                                    {
                                                        if (producttype == 1)
                                                        {
                                                            InOutProductVariableController.Insert(AgentID, ID, 0, "", "", Quantity, 0, 2, false, 1, "Xuất khi thêm mới sản phẩm trong chỉnh sửa đơn", OrderID, 0, 3, ProductName, SKU, ProductImageOrigin, ProductVariable, currentDate, username, 0, ID);
                                                        }
                                                        else
                                                        {

                                                            string parentSKU = "";
                                                            var productV = ProductVariableController.GetByID(ID);
                                                            if (productV != null)
                                                                parentSKU = productV.ParentSKU;
                                                            if (!string.IsNullOrEmpty(parentSKU))
                                                            {
                                                                var product = ProductController.GetBySKU(parentSKU);
                                                                if (product != null)
                                                                    parentID = product.ID;
                                                            }
                                                            InOutProductVariableController.Insert(AgentID, 0, ID, ProductVariableName, ProductVariableValue, Quantity, 0, 2,
                                                                false, 2, "Xuất khi thêm mới sản phẩm trong chỉnh sửa đơn", OrderID, 0, 3, ProductName, SKU,
                                                                ProductImageOrigin, ProductVariable, currentDate, username, 0, parentID);
                                                        }
                                                        OrderDetailController.UpdateIsCount(OrderDetailID, true);
                                                    }
                                                }
                                                OrderDetailController.UpdateQuantity(OrderDetailID, Quantity, currentDate, username);
                                            }
                                        }
                                        else
                                        {
                                            if (ExcuteStatus == 2 && PaymentStatus == 3)
                                            {
                                                OrderDetailController.Insert(AgentID, OrderID, SKU, ProductID, ProductVariableID, ProductVariableSave, Quantity, Price, 1, "0",
                                                    producttype, currentDate, username, true);
                                                if (producttype == 1)
                                                {
                                                    InOutProductVariableController.Insert(AgentID, ID, 0, "", "", Quantity, 0, 2, false, 1, "", OrderID,
                                                        0, 3, ProductName, SKU, ProductImageOrigin, ProductVariable, currentDate, username, 0, ID);
                                                }
                                                else
                                                {

                                                    string parentSKU = "";
                                                    var productV = ProductVariableController.GetByID(ID);
                                                    if (productV != null)
                                                        parentSKU = productV.ParentSKU;
                                                    if (!string.IsNullOrEmpty(parentSKU))
                                                    {
                                                        var product = ProductController.GetBySKU(parentSKU);
                                                        if (product != null)
                                                            parentID = product.ID;
                                                    }
                                                    InOutProductVariableController.Insert(AgentID, 0, ID, ProductVariableName, ProductVariableValue, Quantity, 0, 2,
                                                        false, 2, "", OrderID, 0, 3, ProductName, SKU, ProductImageOrigin, ProductVariable, currentDate,
                                                        username, 0, parentID);
                                                }
                                            }
                                            else
                                            {
                                                OrderDetailController.Insert(AgentID, OrderID, SKU, ProductID, ProductVariableID, ProductVariableSave, Quantity, Price, 1, "0",
                                                    producttype, currentDate, username, true);
                                                if (producttype == 1)
                                                {
                                                    InOutProductVariableController.Insert(AgentID, ID, 0, "", "", Quantity, 0, 2, false, 1, "", OrderID,
                                                        0, 3, ProductName, SKU, ProductImageOrigin, ProductVariable, currentDate, username, 0, ID);
                                                }
                                                else
                                                {

                                                    string parentSKU = "";
                                                    var productV = ProductVariableController.GetByID(ID);
                                                    if (productV != null)
                                                        parentSKU = productV.ParentSKU;
                                                    if (!string.IsNullOrEmpty(parentSKU))
                                                    {
                                                        var product = ProductController.GetBySKU(parentSKU);
                                                        if (product != null)
                                                            parentID = product.ID;
                                                    }
                                                    InOutProductVariableController.Insert(AgentID, 0, ID, ProductVariableName, ProductVariableValue, Quantity, 0, 2,
                                                        false, 2, "", OrderID, 0, 3, ProductName, SKU, ProductImageOrigin, ProductVariable, currentDate,
                                                        username, 0, parentID);
                                                }
                                            }
                                        }
                                        totalQuantity += Quantity;
                                    }

                                }

                                string refund = HttpContext.Current.Session["refund"].ToString();
                                if (refund == "0")
                                {
                                    if (hdfcheckR.Value != "")
                                    {
                                        string[] b = hdfcheckR.Value.Split(',');
                                        var update_returnorder = RefundGoodController.UpdateStatus(b[0].ToInt(), acc.Username, 1);
                                        var update_order = OrderController.UpdateRefund(OrderID, 0, acc.Username);
                                    }
                                }
                                else if (refund != "1")
                                {
                                    string[] RefundID = refund.Split('|');
                                    var update_returnorder = RefundGoodController.UpdateStatus(RefundID[0].ToInt(), acc.Username, 2);
                                    var update_order = OrderController.UpdateRefund(OrderID, RefundID[0].ToInt(), acc.Username);

                                    if(hdfcheckR.Value != "")
                                    {
                                        string[] k = hdfcheckR.Value.Split(',');
                                        if (k[0] != RefundID[0])
                                        {
                                            var update_oldreturnorder = RefundGoodController.UpdateStatus(k[0].ToInt(), acc.Username, 1);
                                        }
                                    }
                                    
                                }

                                HttpContext.Current.Session.Remove("refund");

                                //PJUtils.ShowMessageBoxSwAlert("Cập nhật đơn hàng thành công", "s", true, Page);
                                PJUtils.ShowMessageBoxSwAlertCallFunction("Cập nhật đơn hàng thành công", "s", true, "addPayAllClicked()", Page);
                                //Response.Redirect("/thong-tin-don-hang.aspx?id=" + OrderID);
                            }
                        }
                    }
                }
            }
        }
    }
}
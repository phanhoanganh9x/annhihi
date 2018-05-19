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

namespace IM_PJ
{
    public partial class chi_tiet_don_hang : System.Web.UI.Page
    {
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

            string username = Session["userLoginSystem"].ToString();
            var acc = AccountController.GetByUsername(username);
            if (acc != null)
            {
                int agentID = Convert.ToInt32(acc.AgentID);
                int orderType = 0;
                int paymentStatus = 0;
                int excuteStatus = 0;
                int payment = 0;
                int ship = 0;
                string s = "";
                string sku = "";

                if (Request.QueryString["o"] != null)
                {
                    orderType = Request.QueryString["o"].ToInt(0);
                }
                if (Request.QueryString["p"] != null)
                {
                    paymentStatus = Request.QueryString["p"].ToInt(0);
                }
                if (Request.QueryString["e"] != null)
                {
                    excuteStatus = Request.QueryString["e"].ToInt(0);
                }
                if (Request.QueryString["pay"] != null)
                {
                    payment = Request.QueryString["pay"].ToInt(0);
                }
                if (Request.QueryString["sh"] != null)
                {
                    ship = Request.QueryString["sh"].ToInt(0);
                }
                if (Request.QueryString["s"] != null)
                {
                    s = Request.QueryString["s"];
                }
                if (Request.QueryString["sku"] != null)
                {
                    sku = Request.QueryString["sku"];
                }
                //txtAgentName.Text = s;
                //txtSKU.Text = sku;
                //ddlOrderType.SelectedValue = orderType.ToString();
                //ddlExcuteStatus.SelectedValue = excuteStatus.ToString();
                //ddlPaymentStatus.SelectedValue = paymentStatus.ToString();
                //ddlPaymentType.SelectedValue = payment.ToString();
                //ddlShippingType.SelectedValue = ship.ToString();
                //var rs = OrderController.SearchByStatical(orderType, paymentStatus, excuteStatus, s, agentID, payment, ship, sku);
                var rs = OrderController.GetByCustomerID(ID);
               
                if (rs.Count > 0)
                {
                    pagingall(rs);
                }
                //string username = Session["userLoginSystem"].ToString();
                //var acc = AccountController.GetByUsername(username);
                //if (acc != null)
                //{
                //    int agentID = Convert.ToInt32(acc.AgentID);
                //    var orders = OrderController.GetAllByAgentID(agentID);
                //    if (orders.Count > 0)
                //    {
                //        pagingall(orders);
                //    }
                //}
            }
        }
        #region Print
        [WebMethod]
        public static string getOrder(int ID)
        {
            List<GetOutOrder> go = new List<GetOutOrder>();
            var order = OrderController.GetByID(ID);
            if (order != null)
            {
                var orderdetail = OrderDetailController.GetByOrderID(order.ID);
                if (orderdetail != null)
                {
                    GetOutOrder getOrder = new GetOutOrder();
                    getOrder.ID = order.ID;
                    getOrder.CustomerName = order.CustomerName;
                    getOrder.CustomerPhone = order.CustomerPhone;
                    getOrder.CustomerEmail = order.CustomerEmail;
                    getOrder.CustomerAddress = order.CustomerAddress;
                    getOrder.TotalPrice = order.TotalPrice;
                    getOrder.GuestPaid = order.GuestPaid;
                    getOrder.CreateDate = order.CreatedDate;

                    for (int i = 0; i < orderdetail.Count; i++)
                    {
                        getOrder.Quantity += orderdetail[i].Quantity;
                        getOrder.SKU += orderdetail[i].SKU;
                        getOrder.Price += orderdetail[i].Price;
                    }
                    go.Add(getOrder);
                }
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(go);
        }

        public class GetOutOrder
        {
            public int ID { get; set; }
            public string CustomerName { get; set; }
            public string CustomerPhone { get; set; }
            public string CustomerAddress { get; set; }
            public string CustomerEmail { get; set; }
            public string TotalPrice { get; set; }
            public double? GuestPaid { get; set; }
            public DateTime? CreateDate { get; set; }
            public string SKU { get; set; }
            public string Quantity { get; set; }
            public string Price { get; set; }
        }
        #endregion
        #region Paging
        public void pagingall(List<tbl_Order> acs)
        {
            int PageSize = 15;
            StringBuilder html = new StringBuilder();
            if (acs.Count > 0)
            {
                int TotalItems = acs.Count;
                if (TotalItems % PageSize == 0)
                    PageCount = TotalItems / PageSize;
                else
                    PageCount = TotalItems / PageSize + 1;

                Int32 Page = GetIntFromQueryString("Page");

                if (Page == -1) Page = 1;
                int FromRow = (Page - 1) * PageSize;
                int ToRow = Page * PageSize - 1;
                if (ToRow >= TotalItems)
                    ToRow = TotalItems - 1;
                for (int i = FromRow; i < ToRow + 1; i++)
                {
                    var item = acs[i];
                    html.Append("<tr>");
                    html.Append("   <td>" + item.ID + "</td>");
                    html.Append("   <td>" + PJUtils.OrderTypeStatus(Convert.ToInt32(item.OrderType)) + "</td>");
                    html.Append("   <td>" + item.CustomerPhone + "</td>");
                    html.Append("   <td>" + item.CustomerName + "</td>");
                    html.Append("   <td>" + PJUtils.OrderPaymentStatus(Convert.ToInt32(item.PaymentStatus)) + "</td>");
                    html.Append("   <td>" + PJUtils.OrderExcuteStatus(Convert.ToInt32(item.ExcuteStatus)) + "</td>");
                    html.Append("   <td>" + PJUtils.PaymentType(Convert.ToInt32(item.PaymentType)) + "</td>");
                    html.Append("   <td>" + PJUtils.ShippingType(Convert.ToInt32(item.ShippingType)) + "</td>");
                    html.Append("   <td>" + string.Format("{0:N0}", Convert.ToDouble(item.TotalPrice)) + "</td>");
                    string date = string.Format("{0:dd/MM/yyyy}", item.CreatedDate);
                    html.Append("   <td>" + date + "</td>");
                    string datedone = string.Format("{0:dd/MM/yyyy}", item.DateDone);
                    html.Append("   <td>" + datedone + "</td>");
                    html.Append("   <td>");
                    html.Append("       <a href=\"/thong-tin-don-hang.aspx?id=" + item.ID + "\" class=\"btn primary-btn h45-btn\">Chi tiết</a>");
                    html.Append("       <a href=\"javascript:;\" onclick=\"printOrder(" + item.ID + ")\" class=\"btn primary-btn h45-btn\">In</a>");
                    html.Append("   </td>");
                    html.Append("</tr>");
                }
            }
            ltrList.Text = html.ToString();
        }
        public static Int32 GetIntFromQueryString(String key)
        {
            Int32 returnValue = -1;
            String queryStringValue = HttpContext.Current.Request.QueryString[key];
            try
            {
                if (queryStringValue == null)
                    return returnValue;
                if (queryStringValue.IndexOf("#") > 0)
                    queryStringValue = queryStringValue.Substring(0, queryStringValue.IndexOf("#"));
                returnValue = Convert.ToInt32(queryStringValue);
            }
            catch
            { }
            return returnValue;
        }
        private int PageCount;
        protected void DisplayHtmlStringPaging1()
        {

            Int32 CurrentPage = Convert.ToInt32(Request.QueryString["Page"]);
            if (CurrentPage == -1) CurrentPage = 1;
            string[] strText = new string[4] { "Trang đầu", "Trang cuối", "Trang sau", "Trang trước" };
            if (PageCount > 1)
                Response.Write(GetHtmlPagingAdvanced(6, CurrentPage, PageCount, Context.Request.RawUrl, strText));

        }
        private static string GetPageUrl(int currentPage, string pageUrl)
        {
            pageUrl = Regex.Replace(pageUrl, "(\\?|\\&)*" + "Page=" + currentPage, "");
            if (pageUrl.IndexOf("?") > 0)
            {
                pageUrl += "&Page={0}";
            }
            else
            {
                pageUrl += "?Page={0}";
            }
            return pageUrl;
        }
        public static string GetHtmlPagingAdvanced(int pagesToOutput, int currentPage, int pageCount, string currentPageUrl, string[] strText)
        {
            //Nếu Số trang hiển thị là số lẻ thì tăng thêm 1 thành chẵn
            if (pagesToOutput % 2 != 0)
            {
                pagesToOutput++;
            }

            //Một nửa số trang để đầu ra, đây là số lượng hai bên.
            int pagesToOutputHalfed = pagesToOutput / 2;

            //Url của trang
            string pageUrl = GetPageUrl(currentPage, currentPageUrl);


            //Trang đầu tiên
            int startPageNumbersFrom = currentPage - pagesToOutputHalfed; ;

            //Trang cuối cùng
            int stopPageNumbersAt = currentPage + pagesToOutputHalfed; ;

            StringBuilder output = new StringBuilder();

            //Nối chuỗi phân trang
            //output.Append("<div class=\"paging\">");
            output.Append("<ul>");

            //Link First(Trang đầu) và Previous(Trang trước)
            if (currentPage > 1)
            {
                output.Append("<li><a title=\"" + strText[0] + "\" href=\"" + string.Format(pageUrl, 1) + "\">Trang đầu</a></li>");
                output.Append("<li><a title=\"" + strText[1] + "\" href=\"" + string.Format(pageUrl, currentPage - 1) + "\">Trang trước</a></li>");
                //output.Append("<li class=\"UnselectedPrev\" ><a title=\"" + strText[1] + "\" href=\"" + string.Format(pageUrl, currentPage - 1) + "\"><i class=\"fa fa-angle-left\"></i></a></li>");

                //output.Append("<span class=\"Unselect_prev\"><a href=\"" + string.Format(pageUrl, currentPage - 1) + "\"></a></span>");
            }

            /******************Xác định startPageNumbersFrom & stopPageNumbersAt**********************/
            if (startPageNumbersFrom < 1)
            {
                startPageNumbersFrom = 1;

                //As page numbers are starting at one, output an even number of pages.  
                stopPageNumbersAt = pagesToOutput;
            }

            if (stopPageNumbersAt > pageCount)
            {
                stopPageNumbersAt = pageCount;
            }

            if ((stopPageNumbersAt - startPageNumbersFrom) < pagesToOutput)
            {
                startPageNumbersFrom = stopPageNumbersAt - pagesToOutput;
                if (startPageNumbersFrom < 1)
                {
                    startPageNumbersFrom = 1;
                }
            }
            /******************End: Xác định startPageNumbersFrom & stopPageNumbersAt**********************/

            //Các dấu ... chỉ những trang phía trước  
            if (startPageNumbersFrom > 1)
            {
                output.Append("<li><a href=\"" + string.Format(GetPageUrl(currentPage - 1, pageUrl), startPageNumbersFrom - 1) + "\">&hellip;</a></li>");
            }

            //Duyệt vòng for hiển thị các trang
            for (int i = startPageNumbersFrom; i <= stopPageNumbersAt; i++)
            {
                if (currentPage == i)
                {
                    output.Append("<li class=\"current\" ><a >" + i.ToString() + "</a> </li>");
                }
                else
                {
                    output.Append("<li><a href=\"" + string.Format(pageUrl, i) + "\">" + i.ToString() + "</a> </li>");
                }
            }

            //Các dấu ... chỉ những trang tiếp theo  
            if (stopPageNumbersAt < pageCount)
            {
                output.Append("<li><a href=\"" + string.Format(pageUrl, stopPageNumbersAt + 1) + "\">&hellip;</a></li>");
            }

            //Link Next(Trang tiếp) và Last(Trang cuối)
            if (currentPage != pageCount)
            {
                //output.Append("<span class=\"Unselect_next\"><a href=\"" + string.Format(pageUrl, currentPage + 1) + "\"></a></span>");
                //output.Append("<li class=\"UnselectedNext\" ><a title=\"" + strText[2] + "\" href=\"" + string.Format(pageUrl, currentPage + 1) + "\"><i class=\"fa fa-angle-right\"></i></a></li>");
                output.Append("<li><a title=\"" + strText[2] + "\" href=\"" + string.Format(pageUrl, currentPage + 1) + "\">Trang sau</a></li>");
                output.Append("<li><a title=\"" + strText[3] + "\" href=\"" + string.Format(pageUrl, pageCount) + "\">Trang cuối</a></li>");
            }
            output.Append("</ul>");
            //output.Append("</div>");
            return output.ToString();
        }
        #endregion

        //protected void btnSearch_Click(object sender, EventArgs e)
        //{
        //    string search = txtAgentName.Text;
        //    string SKU = txtSKU.Text;
        //    Response.Redirect("/danh-sach-don-hang?s=" + search + "&o=" + ddlOrderType.SelectedValue + "&p=" + ddlPaymentStatus.SelectedValue + "&e=" + ddlExcuteStatus.SelectedValue + "&pay=" + ddlPaymentType.SelectedValue + "&sh=" + ddlShippingType.SelectedValue + "&sku=" + SKU + "");

        //}
        public class danhmuccon1
        {
            public tbl_Category cate1 { get; set; }
            public string parentName { get; set; }
        }
    }
}
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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IM_PJ
{
    public partial class chi_tiet_nha_xe : System.Web.UI.Page
    {
        private int PageCount;

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
                        if (acc.RoleID == 1)
                        {
                            Response.Redirect("/trang-chu");
                        }

                        var ID = Request.QueryString["ID"];

                        if (ID != null)
                        {
                            Initialize(Convert.ToInt32(ID));
                        }
                        else
                        {
                            Response.Redirect("/danh-sach-nha-xe");
                        }
                    }
                }
                else
                {

                    Response.Redirect("/dang-nhap");
                }
            }
        }

        /// <summary>
        /// Setting init when load page
        /// </summary>
        private void Initialize(int ID)
        {
            var company = TransportCompanyController.GetTransportCompanyByID(ID);

            if (company != null)
            {
                this.hdfID.Value = ID.ToString();
                this.txtCompanyName.Text = company.CompanyName;
                this.txtCompanyPhone.Text = company.CompanyPhone;
                this.txtCompanyAddress.Text = company.CompanyAddress;

                var transprots = TransportCompanyController.GetReceivePlace(ID);

                if (transprots.Count > 0)
                {
                    pagingall(transprots);
                }
            }
            else
            {
                Response.Redirect("/danh-sach-nha-xe");
            }
        }

        #region Paging
        public void pagingall(List<tbl_TransportCompany> transprots)
        {
            int PageSize = 15;

            StringBuilder html = new StringBuilder();

            if (transprots.Count > 0)
            {
                int TotalItems = transprots.Count;

                if (TotalItems % PageSize == 0)
                {
                    PageCount = TotalItems / PageSize;
                }
                else
                {
                    PageCount = TotalItems / PageSize + 1;
                }

                int Page = GetIntFromQueryString();

                int FromRow = (Page - 1) * PageSize;
                int ToRow = Page * PageSize - 1;

                if (ToRow >= TotalItems)
                {
                    ToRow = TotalItems - 1;
                }
                for (int i = FromRow; i < ToRow + 1; i++)
                {
                    var company = transprots[i];
                    String rowHtml = String.Empty;

                    rowHtml += Environment.NewLine + String.Format("<tr>");
                    rowHtml += Environment.NewLine + String.Format("    <td>{0}</td>", i + 1);
                    rowHtml += Environment.NewLine + String.Format("    <td>{0}</td>", company.ShipTo);
                    rowHtml += Environment.NewLine + String.Format("    <td>{0}</td>", company.Address);
                    rowHtml += Environment.NewLine + String.Format("    <td>{0}</td>", company.Prepay ? "Có" : "Không");
                    rowHtml += Environment.NewLine + String.Format("    <td>{0}</td>", company.COD ? "Có" : "Không");
                    rowHtml += Environment.NewLine + String.Format("    <td>{0:dd/MM/yyyy}</td>", company.CreatedDate);
                    rowHtml += Environment.NewLine + String.Format("    <td>");
                    rowHtml += Environment.NewLine + String.Format("        <a href=\"/chi-tiet-noi-den-nha-xe?id={0}&subid={1}\" class=\"btn primary-btn h45-btn\">Chi tiết</a>", company.ID, company.SubID);
                    rowHtml += Environment.NewLine + String.Format("    </td>");
                    rowHtml += Environment.NewLine + String.Format("</tr>");

                    html.AppendLine(rowHtml);
                }
            }

            this.ltrList.Text = html.ToString();

        }

        private static int GetIntFromQueryString()
        {
            int returnValue = 1;

            String queryStringValue = HttpContext.Current.Request.QueryString["Page"];
            try
            {
                if (queryStringValue != null)
                {
                    if (queryStringValue.IndexOf("#") > 0)
                    {
                        queryStringValue = queryStringValue.Substring(0, queryStringValue.IndexOf("#"));
                    }
                    else
                    {
                        returnValue = Convert.ToInt32(queryStringValue);
                    }
                }
            }
            catch
            {
                returnValue = 1;
            }
            return returnValue;
        }

        
        protected void DisplayHtmlStringPaging1()
        {

            int CurrentPage = Convert.ToInt32(Request.QueryString["Page"]);

            // Check min page
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            string[] strText = new string[4] { "Trang đầu", "Trang cuối", "Trang sau", "Trang trước" };

            if (PageCount > 1)
            {
                Response.Write(GetHtmlPagingAdvanced(6, CurrentPage, PageCount, Context.Request.RawUrl, strText));
            }

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

            String pageHtml = String.Empty;

            //Nối chuỗi phân trang
            pageHtml += Environment.NewLine + String.Format("<ul>");

            //Link First(Trang đầu) và Previous(Trang trước)
            if (currentPage > 1)
            {
                pageHtml += Environment.NewLine + String.Format("    <li><a title='{0}' href='{1}'>Trang đầu</a></li>", strText[0], String.Format(pageUrl, 1));
                pageHtml += Environment.NewLine + String.Format("    <li><a title='{0}' href='{1}'>Trang trước</a></li>", strText[1], String.Format(pageUrl, currentPage - 1));
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
                pageHtml += Environment.NewLine + String.Format("    <li><a href='{0}'>&hellip;</a></li>", String.Format(GetPageUrl(currentPage - 1, pageUrl), startPageNumbersFrom - 1));
            }

            //Duyệt vòng for hiển thị các trang
            for (int i = startPageNumbersFrom; i <= stopPageNumbersAt; i++)
            {
                if (currentPage == i)
                {
                    pageHtml += Environment.NewLine + String.Format("    <li class=\"current\" ><a >{0}</a></li>", i.ToString());
                }
                else
                {
                    pageHtml += Environment.NewLine + String.Format("    <li><a href='{0}'>{1}</a></li>", String.Format(pageUrl, i), i.ToString());
                }
            }

            //Các dấu ... chỉ những trang tiếp theo  
            if (stopPageNumbersAt < pageCount)
            {
                pageHtml += Environment.NewLine + String.Format("    <li><a href='{0}'>&hellip;</a></li>", String.Format(pageUrl, stopPageNumbersAt + 1));
            }

            //Link Next(Trang tiếp) và Last(Trang cuối)
            if (currentPage != pageCount)
            {
                pageHtml += Environment.NewLine + String.Format("    <li><a title='{0}' href='{1}'>Trang sau</a></li>", strText[2], String.Format(pageUrl, currentPage + 1));
                pageHtml += Environment.NewLine + String.Format("    <li><a title='{0}' href='{1}'>Trang cuối</a></li>", strText[3], String.Format(pageUrl, pageCount));
            }
            pageHtml += Environment.NewLine + String.Format("</ul>");

            return pageHtml;
        }
        #endregion

    }
}
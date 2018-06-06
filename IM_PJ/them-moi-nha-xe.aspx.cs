﻿using IM_PJ.Controllers;
using IM_PJ.Models;
using System;
using System.Web.UI;

namespace IM_PJ
{
    public partial class them_moi_nha_xe : System.Web.UI.Page
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
                        if (acc.RoleID != 0)
                        {
                            Response.Redirect("/dang-nhap");
                        }

                        // Check mode of page
                        Initialize();

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
        /// <param name="ID"></param>
        private void Initialize()
        {
            // Setting display
            this.txtCompanyName.Focus();

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string username = Session["userLoginSystem"].ToString();
            var acc = AccountController.GetByUsername(username);
            if (acc != null)
            {
                if (acc.RoleID == 0)
                {
                    tbl_TransportCompany transportCompanyNew = new tbl_TransportCompany();

                    transportCompanyNew.CompanyName = this.txtCompanyName.Text;
                    transportCompanyNew.CompanyPhone = this.txtCompanyPhone.Text;
                    transportCompanyNew.CompanyAddress = this.txtCompanyAddress.Text;
                    transportCompanyNew.Note = this.pNote.Content;
                    transportCompanyNew.CreatedBy = username;

                    TransportCompanyController.InsertTransportCompany(transportCompanyNew);

                    Response.Redirect("/danh-sach-nha-xe");
                }
            }
        }
    }
}
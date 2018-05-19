﻿<%@ Page Title="Quản lý nhân viên" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="quan-ly-nhan-vien.aspx.cs" Inherits="IM_PJ.quan_ly_nhan_vien" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main id="main-wrap">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <h3 class="page-title left">Danh sách nhân viên - <asp:Label ID="lblAgentName" runat="server"></asp:Label> </h3>
                    <div class="right above-list-btn">
                        <asp:Literal ID="ltrback" runat="server"></asp:Literal>
                        <asp:Literal ID="ltrAddnew" runat="server"></asp:Literal>
                        
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">                   
                    <div class="filter-above-wrap clear">                        
                        <div class="right">
                            <div class="filter-control right">
                                <asp:TextBox ID="txtAgentName" runat="server" CssClass="form-control" placeholder="Nhập username để tìm"
                                    Width="210px"></asp:TextBox>
                                <a href="javascript:;" onclick="searchAgent()" class="btn primary-btn h45-btn"><i class="fa fa-search"></i></a>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn primary-btn h45-btn" OnClick="btnSearch_Click" Style="display: none" />
                            </div>
                        </div>
                    </div>
                    <div class="panel-table clear">
                        <div class="responsive-table">
                            <table class="table table-checkable table-product">
                                <tbody>
                                    <tr>
                                        <th>Username</th>                                        
                                        <th>Email</th>
                                        <th>Quyền</th>
                                        <th>Trạng thái</th>
                                        <th>Ngày tạo</th>
                                        <th>Thao tác</th>
                                    </tr>
                                    <asp:Literal ID="ltrList" runat="server" EnableViewState="false"></asp:Literal>
                                </tbody>
                            </table>
                        </div>
                        <div class="panel-footer clear">
                            <div class="pagination">
                                <%this.DisplayHtmlStringPaging1();%>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <script type="text/javascript">
            function searchAgent() {
                $("#<%= btnSearch.ClientID%>").click();
            }
        </script>
    </main>
</asp:Content>

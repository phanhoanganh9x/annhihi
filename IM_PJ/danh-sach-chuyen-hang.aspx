﻿<%@ Page Title="Danh sách chuyển hàng" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="danh-sach-chuyen-hang.aspx.cs" Inherits="IM_PJ.danh_sach_chuyen_hang" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main id="main-wrap">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <h3 class="page-title left">Danh sách chuyển hàng</h3>
                    <div class="right above-list-btn">
                        <a href="/tao-lenh-chuyen-hang" class="h45-btn btn" style="background-color: #ff3f4c">Tạo lệnh chuyển hàng</a>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="filter-above-wrap clear">
                        <div class="right">
                            <div class="filter-control right">
                                <div class="col-md-5">
                                    <asp:TextBox ID="txtAgentName" runat="server" CssClass="form-control" placeholder="Nhập tên đại lý để tìm"
                                        Width="210px"></asp:TextBox>
                                </div>
                                <div class="col-md-6">
                                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="0" Text="-- Tất cả trạng thái --"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Chưa chuyển"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Đã chuyển"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="Đã hoàn tất"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-1">
                                    <a href="javascript:;" onclick="searchAgent()" class="btn primary-btn h45-btn"><i class="fa fa-search"></i></a>
                                    <asp:Button ID="btnSearch" runat="server" CssClass="btn primary-btn h45-btn" OnClick="btnSearch_Click" Style="display: none" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-table clear">
                        <div class="responsive-table">
                            <table class="table table-checkable table-product">
                                <tbody>
                                    <tr>
                                        <th>ID</th>
                                        <th>Tên ĐL chuyển</th>
                                        <th>Tên ĐL nhận</th>
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

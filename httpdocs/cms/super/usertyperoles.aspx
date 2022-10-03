<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="usertyperoles.aspx.vb" Inherits="cms_super_usertyperoles" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:Box ID="bx" runat="server" DefaultReturnButton="btnSave" IconClass="fa fa-cogs" Title="Resources.Local.egvBox.Title" Type="Warning">
        <BoxBody>
            <asp:Repeater ID="rpt" runat="server">
                <HeaderTemplate>
                    <table class="table table-hover egvGrid">
                        <tr class="bg-navy">
                            <th><%=Localization.GetResource("Resources.Local.grd.Type") %></th>
                            <th class="text-center"><%=Localization.GetResource("Resources.Local.grd.Role") %></th>
                        </tr>
                </HeaderTemplate>
                <FooterTemplate></table></FooterTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="bg-olive"><%#Eval("Title") %></td>
                        <td class="text-center" data-id='<%#Eval("UserType")%>'>
                            <egvc:EGVDropDown ID="ddlRole" runat="server" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <asp:HiddenField ID="hdnValues" runat="server" ClientIDMode="Static" />
        </BoxBody>
        <BoxFooter>
            <egvc:EGVLinkButton ID="btnSave" runat="server" Color="Primary" Text="Resources.Local.btnSave.Text" CssClass="pull-right" />
        </BoxFooter>
    </egvc:Box>
</asp:Content>
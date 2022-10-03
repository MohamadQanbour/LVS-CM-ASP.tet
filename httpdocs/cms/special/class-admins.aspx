<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="class-admins.aspx.vb" Inherits="cms_special_class_admins" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:Box  runat="server" IconClass="fa fa-user-secret" Title="Resources.Local.egvGrid.Title" Type="Primary" DefaultReturnButton="btnSave">
        <BoxBody>
            <asp:Repeater ID="rptClasses" runat="server">
                <HeaderTemplate>
                    <table class="table table-hover egvGrid">
                        <tr class="bg-navy">
                            <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.rptClasses.ClassTitle") %></th>
                            <th class="text-center"><%=Localization.GetResource("Resources.Local.rptClasses.Admin") %></th>
                        </tr>
                </HeaderTemplate>
                <FooterTemplate></table></FooterTemplate>
                <ItemTemplate>
                    <tr data-classid='<%#Eval("Id") %>'>
                        <td class="bg-olive text-center" colspan="2"><%#Eval("Title") %></td>
                    </tr>
                    <asp:Repeater ID="rptSections" runat="server" OnItemDataBound="rpt_ItemDataBound">
                        <ItemTemplate>
                            <tr data-sectionid='<%#Eval("Id") %>' id="tr" runat="server">
                                <td class="bg-olive"><%#Eval("Title") %></td>
                                <td data-type="admin">
                                    <egvc:EGVDropDown ID="ddlUsers" runat="server" Enabled='<%#CanUpdate %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </ItemTemplate>
            </asp:Repeater>
        </BoxBody>
        <BoxFooter>
            <asp:HiddenField ID="hdnValues" runat="server" ClientIDMode="Static" />
            <egvc:EGVLinkButton ID="btnSave" runat="server" Color="Primary" Text="Resources.Local.btnSave.Click" CssClass="pull-right" />
        </BoxFooter>
    </egvc:Box>
</asp:Content>
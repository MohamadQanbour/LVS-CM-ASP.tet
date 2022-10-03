<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="Settings.aspx.vb" Inherits="cms_setup_Settings" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVHyperLink ID="hypAdd" runat="server" Color="Default" NavigateUrl="settings-editor.aspx" Visible="false" AppButton="true">
        <i class="fa fa-plus"></i><%=GetLocalResourceObject("hypAdd.Text") %>
    </egvc:EGVHyperLink>
    <egvc:EGVGridView ID="egvGrid" runat="server" Title="Resources.Local.egvBox.Title" IconClass="fa fa-cogs" BusinessClass="system/Setting">
        <GridFooterTools>
            <egvc:EGVButton ID="btnSave" runat="server" Color="Primary" FlatButton="true" Text="Resources.Local.btnSave.Text" />
        </GridFooterTools>
    </egvc:EGVGridView>
</asp:Content>
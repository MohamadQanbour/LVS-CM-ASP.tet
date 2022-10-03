<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="areas.aspx.vb" Inherits="cms_lookup_areas" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVHyperLink ID="hypAdd" runat="server" Color="Default" NavigateUrl="areas-editor.aspx" AppButton="true">
        <i class="fa fa-plus"></i><%=GetLocalResourceObject("hypAdd.Text") %>
    </egvc:EGVHyperLink>
    <egvc:EGVGridView ID="egvGrid" runat="server" Title="Resources.Local.egvGrid.Title" IconClass="fa fa-map-marker" BusinessClass="lookup/Area">
        <Toolbar runat="server">
            <ButtonGroups>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnDelete" ActiveState="Multi" Color="Danger" IconClass="fa fa-trash-o" Size="Small" Text="Resources.Local.btnDelete.Text" CommandName="delete" ShouldConfirm="true" />
                    </Buttons>
                </egvc:ToolbarButtonGroup>
            </ButtonGroups>
        </Toolbar>
    </egvc:EGVGridView>
</asp:Content>
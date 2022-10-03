<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="exceptions-viewer.aspx.vb" Inherits="cms_super_exceptions_viewer" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowId.Text">
                <Content>
                    <egvc:EGVTextBox ID="txtId" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowDate.Text">
                <Content>
                    <egvc:EGVTextBox ID="txtDate" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowMessage.Text">
                <Content>
                    <egvc:EGVTextBox ID="txtMessage" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowTrace.Text">
                <Content>
                    <egvc:EGVTextBox ID="txtTrace" runat="server" ReadOnly="true" TextMode="MultiLine" Rows="10" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <div class="btn-toolbar">
                <div class="btn-group" role="group">
                    <egvc:EGVLinkButton ID="lnkDelete" runat="server" Text="Resources.Local.lnkDelete.Text" Color="Danger" Size="Small" />
                </div>
            </div>
        </FormFooter>
    </egvc:EGVInputForm>
</asp:Content>
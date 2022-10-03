<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="menus-editor.aspx.vb" Inherits="cms_super_menus_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowId.Text">
                <Content>
                    <egvc:EGVTextBox ID="txtId" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowTitle.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtTitle" runat="server" MaxLength="50" />
                    <egvc:EGVRequired ID="reqTitle" runat="server" ControlToValidate="txtTitle" ValidationGroup="valSave" 
                        ErrorMessage="Resources.Local.reqTitle.ErrorMessage" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowIcon.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtIcon" runat="server" MaxLength="50" />
                    <egvc:EGVRequired ID="reqIcon" runat="server" ControlToValidate="txtIcon" ValidationGroup="valSave"
                        ErrorMessage="Resources.Local.reqIcon.ErrorMessage" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowParent.Text">
                <Content>
                    <egvc:EGVDropDown ID="ddlParent" runat="server" DataTextField="Title" DataValueField="Id" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowPath.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtPath" runat="server" MaxLength="255" />
                    <egvc:EGVRequired ID="reqPath" runat="server" ControlToValidate="txtPath" ValidationGroup="valSave"
                        ErrorMessage="Resources.Local.reqPath.ErrorMessage" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowPageTitle.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtPageTitle" runat="server" MaxLength="50" />
                    <egvc:EGVRequired ID="reqPageTitle" runat="server" ControlToValidate="txtPageTitle" ValidationGroup="valSave"
                        ErrorMessage="Resources.Local.reqPageTitle.ErrorMessage" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowDesc.Text">
                <Content>
                    <egvc:EGVTextBox ID="txtDesc" runat="server" MaxLength="255" TextMode="MultiLine" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowPermission.Text">
                <Content>
                    <egvc:EGVDropDown ID="ddlPermission" runat="server" DataTextField="Title" DataValueField="Id" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowSuper.Text">
                <Content>
                    <div class="icheck">
                        <egvc:EGVCheckBox ID="chkSuper" runat="server" Text="Resources.Local.chkSuper.Text" />
                    </div>
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowOrder.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtOrder" runat="server" MaxLength="3" />
                    <egvc:EGVRequired ID="reqOrder" runat="server" ControlToValidate="txtOrder" ValidationGroup="valSave"
                        ErrorMessage="Resources.Local.reqOrder.Text" />
                    <egvc:EGVCompareValidator ID="compOrder" runat="server" ControlToValidate="txtOrder" Operator="DataTypeCheck"
                        Type="Integer" ValidationGroup="valSave" ErrorMessage="Resources.Local.valOrder.Text" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowPublished.Text">
                <Content>
                    <div class="icheck">
                        <egvc:EGVCheckBox ID="chkPublished" runat="server" Text="Resources.Local.chkPublished.Text" />
                    </div>
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </FormFooter>
    </egvc:EGVInputForm>
</asp:Content>
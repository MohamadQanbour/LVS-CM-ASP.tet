<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="settings-editor.aspx.vb" Inherits="cms_setup_settings_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowKey.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtKey" runat="server" />
                    <egvc:EGVRequired ID="reqKey" runat="server" ControlToValidate="txtKey" ErrorMessage="Resources.Local.txtKey.Required" 
                        ValidationGroup="valSave" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowTitle.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtTitle" runat="server" />
                    <egvc:EGVRequired ID="reqTitle" runat="server" ControlToValidate="txtTitle" ErrorMessage="Resources.Local.txtTitle.Required"
                        ValidationGroup="valSave" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowType.Text">
                <Content>
                    <egvc:egvDropDown ID="ddlType" runat="server" AllowNull="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowList.Text">
                <Content>
                    <egvc:EGVDropDown ID="ddlList" runat="server" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </FormFooter>
    </egvc:EGVInputForm>
</asp:Content>
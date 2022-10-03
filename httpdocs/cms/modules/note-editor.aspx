<%@ Page Title="" Language="VB" MasterPageFile="~/cms/CMSMaster.master" AutoEventWireup="false" CodeFile="note-editor.aspx.vb" Inherits="cms_modules_note_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<%@ Import Namespace="EGV.Business" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowId" Visible="false">
                <Content>
                    <egvc:EGVTextBox ID="txtId" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowSender">
                <Content>
                    <egvc:EGVTextBox ID="txtSender" runat="server" ReadOnly="true" />
                    <asp:HiddenField ID="hdnSender" runat="server" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowStudent">
                <Content>
                    <egvc:EGVDropDown ID="ddlStudent" runat="server" ClientIDMode="Static" AllowNull="false" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/Students" />
                    <asp:HiddenField ID="hdnSelectedStudent" runat="server" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowType">
                <Content>
                    <egvc:EGVDropDown ID="ddlNoteType" runat="server" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowDate">
                <Content>
                    <egvc:EGVTextBox ID="txtNotedate" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowNote">
                <Content>
                    <egvc:EGVTextBox ID="txtNote" runat="server" TextMode="MultiLine" />
                    <egvc:EGVRequired ID="reqNote" runat="server" ControlToValidate="txtNote"
                        ValidationGroup="valSave" ErrorMessage="Resources.Local.reqNote" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </FormFooter>
    </egvc:EGVInputForm>
</asp:Content>
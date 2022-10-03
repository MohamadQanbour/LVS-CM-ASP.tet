<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="classes-editor.aspx.vb" Inherits="cms_modules_classes_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<%@ Import Namespace="EGV.Business" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowId.Text" Visible="false">
                <Content>
                    <egvc:EGVTextBox ID="txtId" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowCode.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtCode" runat="server" MaxLength="50" />
                    <egvc:EGVRequired ID="reqCode" runat="server" ControlToValidate="txtCode" ValidationGroup="valSave"
                        ErrorMessage="Resources.Local.reqCode.ErrorMessage" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowTitle.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtTitle" runat="server" MaxLength="50" />
                    <egvc:EGVRequired ID="reqTitle" runat="server" ControlToValidate="txtTitle" ValidationGroup="valSave" 
                        ErrorMessage="Resources.Local.reqTitle.ErrorMessage" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowExamTemplate.Title">
                <Content>
                    <div class="help-block"><%=Localization.GetResource("Resources.Local.TemplateHelper") %></div>
                    <asp:repeater ID="rptTemplates" runat="server">
                        <ItemTemplate>
                            <div class="col-md-4">
                                <div class="icheck" data-template='<%#Eval("Id") %>'>
                                    <egvc:EGVCheckBox ID="chk" runat="server" Text='<%#Eval("Title") %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:repeater>
                    <asp:HiddenField ID="hdnSelectedTemplates" runat="server" ClientIDMode="Static" Value="[]" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </FormFooter>
    </egvc:EGVInputForm>
</asp:Content>
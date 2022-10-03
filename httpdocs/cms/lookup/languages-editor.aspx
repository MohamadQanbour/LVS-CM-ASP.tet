<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="languages-editor.aspx.vb" Inherits="cms_lookup_languages_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowId.Text" Visible="false">
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
            <egvc:RowItem runat="server" Title="Resources.Local.rowLanguageCode.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtLanguageCode" runat="server" MaxLength="5" />
                    <egvc:EGVRequired ID="reqLanguageCode" runat="server" ControlToValidate="txtLanguageCode" ValidationGroup="valSave" 
                        ErrorMessage="Resources.Local.reqLanguageCode.ErrorMessage" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowActive.Text">
                <Content>
                    <div class="icheck">
                        <egvc:EGVCheckBox ID="chkActive" runat="server" Text="Resources.Local.chkActive.Text" />
                    </div>
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowDefault.Text">
                <Content>
                    <div class="icheck">
                        <egvc:EGVCheckBox ID="chkDefault" runat="server" Text="Resources.Local.chkDefault.Text" />
                    </div>
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowRTL.Text">
                <Content>
                    <div class="icheck">
                        <egvc:EGVCheckBox ID="chkRTL" runat="server" Text="Resources.Local.chkRTL.Text" />
                    </div>
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowImage.Text">
                <Content>
                    <egvc:ImageSelector ID="txtImage" runat="server" CMSAssets="true" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </FormFooter>
    </egvc:EGVInputForm>
</asp:Content>
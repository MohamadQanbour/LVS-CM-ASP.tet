<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="emails-editor.aspx.vb" Inherits="cms_setup_emails_editor" %>
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
        </Rows>
        <FormFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </FormFooter>
    </egvc:EGVInputForm>
    <egvc:EGVGridView ID="egvGrid" runat="server" Title="Resources.Local.egvGrid.Title" IconClass="fa fa-envelope-o" BusinessClass="system/Email">
        <Toolbar runat="server" ID="egvToolbar">
            <ButtonGroups>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnAdd" ActiveState="Always" Color="Success" IconClass="fa fa-plus" Size="Small" Text="Resources.Local.btnAdd.Text" CommandName="add" IsHyperlink="true"  />
                    </Buttons>
                </egvc:ToolbarButtonGroup>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnActivate" ActiveState="Multi" Color="Olive" IconClass="fa fa-check" Size="Small" Text="Resources.Local.btnActivate.Text" CommandName="activate" />
                        <egvc:ToolbarButton ID="btnDeactivate" ActiveState="Multi" Color="Maroon" IconClass="fa fa-times" Size="Small" Text="Resources.Local.btnDeactivate.Text" CommandName="deactivate" />
                    </Buttons>
                </egvc:ToolbarButtonGroup>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnDelete" ActiveState="Multi" Color="Danger" IconClass="fa fa-trash-o" Size="Small" Text="Resources.Local.btnDelete.Text" CommandName="delete" ShouldConfirm="true" />
                    </Buttons>
                </egvc:ToolbarButtonGroup>
            </ButtonGroups>
        </Toolbar>
        <GridFooterTools>
            <egvc:EGVButton ID="btnSave" runat="server" Color="Primary" FlatButton="true" Text="Resources.Local.btnSave.Text" />
        </GridFooterTools>
    </egvc:EGVGridView>
    <egvc:Modal ID="modalAdd" runat="server" Title="Resources.Local.modal.Title" Size="Large">
        <Body>
            <egvc:EGVInputForm ID="egvIF2" runat="server" NoBorders="true">
                <Rows>
                    <egvc:RowItem runat="server" Title="Resources.Local.rowEmailAddress.Text" Required="true">
                        <Content>
                            <egvc:EGVTextBox ID="txtEmailAddress" runat="server" MaxLength="100" />
                            <egvc:EGVRequired ID="reqEmailAddress" runat="server" ControlToValidate="txtEmailAddress" ValidationGroup="valAdd"
                                ErrorMessage="Resources.Local.reqEmailAddress.ErrorMessage" />
                            <egvc:EGVRegxValidator ID="regEmailAddress" runat="server" ControlToValidate="txtEmailAddress" ValidationGroup="valAdd"
                                ErrorMessage="Resources.Local.regEmailAddress.ErrorMessage" ValidationType="Email" />
                        </Content>
                    </egvc:RowItem>
                    <egvc:RowItem runat="server" Title="Resources.Local.rowDisplayName.Text" Required="true">
                        <Content>
                            <egvc:EGVTextBox ID="txtDisplayName" runat="server" MaxLength="50" />
                            <egvc:EGVRequired ID="reqDisplayName" runat="server" ControlToValidate="txtDisplayName" ValidationGroup="valAdd"
                                ErrorMessage="Resources.Local.reqDisplayName.ErrorMessage" />
                        </Content>
                    </egvc:RowItem>
                    <egvc:RowItem runat="server" Title="Resources.Local.rowType.Text">
                        <Content>
                            <egvc:EGVDropDown ID="ddlType" runat="server" Size="Small" />
                        </Content>
                    </egvc:RowItem>
                </Rows>
            </egvc:EGVInputForm>
        </Body>
        <Footer>
            <egvc:EGVLinkButton ID="lnkAdd" runat="server" Color="Primary" Text="Resources.Local.lnkAdd.Text" ValidationGroup="valAdd" CssClass="pull-right" />
        </Footer>
    </egvc:Modal>
</asp:Content>
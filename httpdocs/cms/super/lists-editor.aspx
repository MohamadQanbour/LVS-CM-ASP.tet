<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="lists-editor.aspx.vb" Inherits="cms_super_lists_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowId.Text">
                <Content>
                    <egvc:EGVTextBox ID="txtId" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowName.Text" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtName" runat="server" MaxLength="50" />
                    <egvc:EGVRequired ID="reqName" runat="server" ControlToValidate="txtName" ValidationGroup="valSave" 
                        ErrorMessage="Resources.Local.reqName.ErrorMessage" />
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
    <egvc:EGVGridView ID="egvGrid" runat="server" Title="Resources.Local.egvGrid.Title" IconClass="fa fa-bars" BusinessClass="system/ListItem">
        <Toolbar runat="server" ID="egvToolbar">
            <ButtonGroups>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnAdd" ActiveState="Always" Color="Success" IconClass="fa fa-plus" Size="Small" Text="Resources.Local.btnAdd.Text" CommandName="add" IsHyperlink="true"  />
                    </Buttons>
                </egvc:ToolbarButtonGroup>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnPublish" ActiveState="Multi" Color="Olive" IconClass="fa fa-check" Size="Small" Text="Resources.Local.btnPublish.Text" CommandName="publish" />
                        <egvc:ToolbarButton ID="btnUnpublish" ActiveState="Multi" Color="Maroon" IconClass="fa fa-times" Size="Small" Text="Resources.Local.btnUnpublish.Text" CommandName="unpublish" />
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
                    <egvc:RowItem runat="server" Title="Resources.Local.rowItemText.Text" Required="true">
                        <Content>
                            <egvc:EGVTextBox ID="txtItemText" runat="server" MaxLength="50" />
                            <egvc:EGVRequired ID="reqItemText" runat="server" ControlToValidate="txtItemText" ValidationGroup="valAdd"
                                ErrorMessage="Resources.Local.reqItemText.ErrorMessage" />
                            <egvc:EGVCustomValidator ID="valItemText" runat="server" ControlToValidate="txtItemText" ValidationGroup="valAdd"
                                ErrorMessage="Resources.Local.valItemText.ErrorMessage" ClientValidationFunction="ValidateText" />
                            <asp:HiddenField ID="hdnItemText" runat="server" ClientIDMode="Static" />
                        </Content>
                    </egvc:RowItem>
                    <egvc:RowItem runat="server" Title="Resources.Local.rowItemValue.Text" Required="true">
                        <Content>
                            <egvc:EGVTextBox ID="txtItemValue" runat="server" MaxLength="255" />
                            <egvc:EGVRequired ID="reqItemValue" runat="server" ControlToValidate="txtItemValue" ValidationGroup="valAdd"
                                ErrorMessage="Resources.Local.reqItemValue.ErrorMessage" />
                            <egvc:EGVCustomValidator ID="valItemValue" runat="server" ControlToValidate="txtItemValue" ValidationGroup="valAdd"
                                ErrorMessage="Resources.Local.valItemValue.ErrorMessage" ClientValidationFunction="ValidateValue" />
                            <asp:HiddenField ID="hdnItemValue" runat="server" ClientIDMode="Static" />
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
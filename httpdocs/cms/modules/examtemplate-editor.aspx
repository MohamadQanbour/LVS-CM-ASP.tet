<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="examtemplate-editor.aspx.vb" Inherits="cms_modules_examtemplate_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Business" %>

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
            <egvc:RowItem runat="server" Title="Resources.Local.rowMaxMark.Text">
                <Content>
                    <egvc:EGVTextBox ID="txtMaxMark" runat="server" InputNumber="true" />
                    <egvc:EGVRequired ID="reqMaxMark" runat="server" ControlToValidate="txtMaxMark" ValidationGroup="valSave"
                        ErrorMessage="Resources.Local.reqMaxMark.ErrorMessage" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
            <egvc:AuditInformation ID="egvAudit" runat="server" />
        </FormFooter>
    </egvc:EGVInputForm>
    <egvc:EGVGridView ID="egvGrid" runat="server" Title="Resources.Local.egvGrid.Title" IconClass="fa fa-bars" BusinessClass="modules/ExamTemplateItem">
        <Toolbar runat="server" ID="egvToolbar">
            <ButtonGroups>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnAdd" ActiveState="Always" Color="Success" IconClass="fa fa-plus" Size="Small" Text="Resources.Local.btnAdd.Text" CommandName="add" IsHyperlink="true"  />
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
    <egvc:Modal ID="modalEdit" runat="server" Title="Resources.Local.modal.Edit" Size="Large">
        <Body DefaultButton="lnkSaveRelations">
            <egvc:EGVInputForm id="egvIF3" runat="server" NoBorders="true">
                <Rows>
                    <egvc:RowItem runat="server" Title="Resources.Local.rowRelations.Title">
                        <Content>
                            <asp:Repeater ID="rptEditChecks" runat="server">
                                <ItemTemplate>
                                    <div class="col-md-4" data-editid='<%#DirectCast(Container.DataItem, ExamTemplateItem).Id %>'>
                                        <div class="icheck">
                                            <egvc:EGVCheckBox ID="chk" runat="server" Text='<%#DirectCast(Container.DataItem, ExamTemplateItem).Title %>' />
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:HiddenField ID="hdnSelectedEditRelations" runat="server" ClientIDMode="Static" Value="[]" />
                        </Content>
                    </egvc:RowItem>
                </Rows>
            </egvc:EGVInputForm>
        </Body>
        <Footer>
            <asp:HiddenField ID="hdnEditId" runat="server" ClientIDMode="Static" Value="0" />
            <egvc:EGVLinkButton ID="lnkSaveRelations" runat="server" Color="Primary" Text="Resources.Local.lnkSaveRelations.Text" />
        </Footer>
    </egvc:Modal>
    <egvc:Modal ID="modalAdd" runat="server" Title="Resources.Local.modal.Title" Size="Large">
        <Body DefaultButton="lnkAdd">
            <egvc:EGVInputForm ID="egvIF2" runat="server" NoBorders="true">
                <Rows>
                    <egvc:RowItem runat="server" Title="Resources.Local.rowItemTitle.Text" Required="true">
                        <Content>
                            <egvc:EGVTextBox ID="txtItemTitle" runat="server" MaxLength="50" />
                            <egvc:EGVRequired ID="reqItemTitle" runat="server" ControlToValidate="txtItemTitle" ValidationGroup="valAdd"
                                ErrorMessage="Resources.Local.reqItemTitle.ErrorMessage" />
                        </Content>
                    </egvc:RowItem>
                    <egvc:RowItem runat="server" Title="Resources.Local.rowItemType.Text" Required="true">
                        <Content>
                            <egvc:EGVDropDown ID="ddlItemType" runat="server" />
                        </Content>
                    </egvc:RowItem>
                    <egvc:RowItem ID="rowRelations" runat="server" Title="Resources.Local.rowRelations.Title">
                        <Content>
                            <asp:Repeater ID="rptChecks" runat="server">
                                <ItemTemplate>
                                    <div class="col-md-4" data-id='<%#DirectCast(Container.DataItem, ExamTemplateItem).Id %>'>
                                        <div class="icheck">
                                            <egvc:EGVCheckBox ID="chk" runat="server" Text='<%#DirectCast(Container.DataItem, ExamTemplateItem).Title %>' />
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:HiddenField ID="hdnSelectedRelations" runat="server" ClientIDMode="Static" Value="[]" />
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
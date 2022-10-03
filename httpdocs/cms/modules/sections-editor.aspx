<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="sections-editor.aspx.vb" Inherits="cms_modules_sections_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hdnLanguageId" runat="server" ClientIDMode="Static" />
    <egvc:EGVInputForm ID="egvIF" runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowId.Text" Visible="false">
                <Content>
                    <egvc:EGVTextBox ID="txtId" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowYear.Text">
                <Content>
                    <egvc:EGVTextBox ID="txtYear" runat="server" ReadOnly="true" />
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
            <egvc:RowItem runat="server" Title="Resources.Local.rowClass.Text" Required="true">
                <Content>
                    <div class="input-group">
                        <egvc:EGVDropDown ID="ddlClass" runat="server" ClientIDMode="Static" AllowNull="false" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/Classes" />
                        <span class="input-group-btn">
                            <egvc:EGVHyperLink ID="hypAddClass" runat="server" NavigateUrl="#" data-toggle="modal" data-target="#egvClassModal" Color="Success">
                                <span class="fa fa-plus"></span>
                            </egvc:EGVHyperLink>
                        </span>
                    </div>
                    <asp:HiddenField ID="hdnSelectedClass" runat="server" ClientIDMode="Static" />
                    <egvc:Modal ID="egvClassModal" runat="server" Size="Large" Title="Resources.Local.egvClassModal.Title" ClientIDMode="Static">
                        <Body>
                            <egvc:EGVInputForm ID="egvIFClass" runat="server" NoBorders="true">
                                <Rows>
                                    <egvc:RowItem runat="server" Title="Resources.Local.rowClassCode.Text" Required="true">
                                        <Content>
                                            <egvc:EGVTextBox ID="txtClassCode" runat="server" MaxLength="50" ClientIDMode="Static" />
                                            <egvc:EGVRequired ID="reqClassCode" runat="server" ControlToValidate="txtClassCode" ValidationGroup="valAddClass"
                                                ErrorMessage="Resources.Local.reqClassCode.ErrorMessage" />
                                        </Content>
                                    </egvc:RowItem>
                                    <egvc:RowItem runat="server" Title="Resources.Local.rowClassTitle.Text" Required="true">
                                        <Content>
                                            <egvc:EGVTextBox ID="txtClassTitle" runat="server" MaxLength="50" ClientIDMode="Static" />
                                            <egvc:EGVRequired ID="reqClassTitle" runat="server" ControlToValidate="txtClassTitle" ValidationGroup="valAddClass"
                                                ErrorMessage="Resources.Local.reqClassTitle.ErrorMessage" />
                                        </Content>
                                    </egvc:RowItem>
                                </Rows>
                            </egvc:EGVInputForm>
                        </Body>
                        <Footer>
                            <egvc:EGVLinkButton ID="lnkAddClass" runat="server" ClientIDMode="Static" Color="Primary" Text="Resources.Local.lnkAddClass.Text" ValidationGroup="valAddClass" CssClass="pull-right" />
                        </Footer>
                    </egvc:Modal>
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowSchedule.Title">
                <Content>
                    <egvc:SingleFileUploader ID="fileSchedule" runat="server" Text="Resources.Local.fileSchedule.Text" AllowedFileTypes=".pdf" Color="Purple" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </FormFooter>
    </egvc:EGVInputForm>
</asp:Content>
<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="users-editor.aspx.vb" Inherits="cms_security_users_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVTabs ID="egvTabs" runat="server" Title="Resources.Local.egvTabs.Title" IconClass="fa fa-user">
        <Tabs>
            <egvc:EGVTabItem Title="Resources.Local.egvTabs.General" runat="server" ID="tabGeneral" />
            <egvc:EGVTabItem Title="Resources.Local.egvTabs.Password" runat="server" Id="tabPassword" />
            <egvc:EGVTabItem Title="Resources.Local.egvTabs.Image" runat="server" Id="tabImage" />
        </Tabs>
        <TabContents>
            <egvc:EGVTabContent runat="server">
                <egvc:EGVInputForm ID="egvIF" runat="server" NoBorders="true">
                    <Rows>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowId.Text" Visible="false">
                            <Content>
                                <egvc:EGVTextBox ID="txtId" runat="server" ReadOnly="true" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowUsername.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtUsername" runat="server" MaxLength="50" />
                                <egvc:EGVRequired ID="reqUsername" runat="server" ControlToValidate="txtUsername" ValidationGroup="valSave" 
                                    ErrorMessage="Resources.Local.reqUsername.ErrorMessage" />
                                <egvc:EGVCustomValidator ID="valUsername" runat="server" ControlToValidate="txtUsername" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.valUsername.ErrorMessage" OnServerValidate="valUsername_Validate" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowEmail.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtEmail" runat="server" MaxLength="255" />
                                <egvc:EGVRequired ID="reqEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.reqEmail.ErrorMessage" Visible="false" />
                                <egvc:EGVRegxValidator ID="regEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.regEmail.ErrorMessage" ValidationType="Email" />
                                <egvc:EGVCustomValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.valEmail.ErrorMessage" OnServerValidate="valEmail_Validate" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem id="rowPassword" runat="server" Title="Resources.Local.rowPassword.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtPassword" runat="server" TextMode="Password" MaxLength="24" />
                                <egvc:EGVRequired ID="reqPassword" runat="server" ControlToValidate="txtPassword" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.reqPassword.ErrorMessage" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem ID="rowRepeatPassword" runat="server" Title="Resources.Local.rowRepeatPassword.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtRepeatPassword" runat="server" TextMode="Password" MaxLength="24" />
                                <egvc:EGVRequired ID="reqRepeatPassword" runat="server" ControlToValidate="txtRepeatPassword" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.reqRepeatPassword.ErrorMessage" />
                                <egvc:EGVControlCompareValidator ID="valPassword" runat="server" ControlToValidate="txtRepeatPassword"
                                    ControlToCompare="txtPassword" ValidationGroup="valSave" ErrorMessage="Resources.Local.compPassword.ErrorMessage" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowFullName.Title" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtFullName" runat="server" MaxLength="50" />
                                <egvc:EGVRequired ID="reqFullName" runat="server" ControlToValidate="txtFullName" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.reqFullName.ErrorMessage" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem ID="rowSuper" runat="server" Title="Resources.Local.rowSuper.Text">
                            <Content>
                                <div class="icheck">
                                    <egvc:EGVCheckBox ID="chkSuper" runat="server" Text="Resources.Local.chkSuper.Text" />
                                </div>
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem ID="rowActive" runat="server" Title="Resources.Local.rowActive.Text">
                            <Content>
                                <div class="icheck">
                                    <egvc:EGVCheckBox ID="chkActive" runat="server" Text="Resources.Local.chkActive.Text" />
                                </div>
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem ID="rowAllowList" runat="server" Title="Resources.Local.rowAllowList.Text">
                            <Content>
                                <div class="icheck">
                                    <egvc:EGVCheckBox id="chkAllowList" runat="server" Text="Resources.Local.chkAllowList.Text" />
                                </div>
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem ID="rowRole" runat="server" Title="Resources.Local.rowRole.Text">
                            <Content>
                                <egvc:EGVDropDown ID="ddlRole" runat="server" ClientIDMode="Static" AllowNull="false" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/Roles" />
                                <asp:HiddenField ID="hdnSelectedRole" runat="server" ClientIDMode="Static" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem ID="rowLanguage" runat="server" Title="Resources.Local.rowLanguage.Text">
                            <Content>
                                <egvc:EGVDropDown ID="ddlLanguage" runat="server" />
                            </Content>
                        </egvc:RowItem>
                    </Rows>
                </egvc:EGVInputForm>
            </egvc:EGVTabContent>
            <egvc:EGVTabContent runat="server">
                <egvc:EGVInputForm ID="egvIF2" runat="server" NoBorders="true">
                    <Rows>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowNewPassword.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtNewPassword" runat="server" MaxLength="24" TextMode="Password" />
                                <egvc:EGVRequired ID="reqNewPassword" runat="server" ControlToValidate="txtNewPassword" ValidationGroup="valChangePassword"
                                    ErrorMessage="Resources.Local.reqNewPassword.ErrorMessage" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowRepeatNewPassword.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtRepeatNewPassword" runat="server" MaxLength="24" TextMode="Password" />
                                <egvc:EGVRequired ID="reqRepeatNewPassword" runat="server" ControlToValidate="txtRepeatNewPassword" ValidationGroup="valChangePassword"
                                    ErrorMessage="Resources.Local.reqRepeatNewPassword.ErrorMessage" />
                                <egvc:EGVControlCompareValidator ID="comRepeatNewPassword" runat="server" ControlToValidate="txtRepeatNewPassword" ValidationGroup="valChangePassword"
                                    ControlToCompare="txtNewPassword" ErrorMessage="Resources.Local.comRepeatNewPassword.ErrorMessage" />
                            </Content>
                        </egvc:RowItem>
                    </Rows>
                    <FormFooter>
                        <div class="text-right">
                            <egvc:EGVLinkButton ID="lnkChangePassword" runat="server" Color="Warning" Text="Resources.Local.lnkChangePassword.Text"
                                ValidationGroup="valChangePassword" />
                        </div>
                    </FormFooter>
                </egvc:EGVInputForm>
            </egvc:EGVTabContent>
            <egvc:EGVTabContent runat="server">
                <egvc:Toolbar ID="egvImageToolbar" runat="server">
                    <ButtonGroups>
                        <egvc:ToolbarButtonGroup runat="server">
                            <Buttons>
                                <egvc:ToolbarButton ID="btnImage" Color="Success" IconClass="fa fa-file-image-o" IsHyperlink="true" NavigateUrl="javascript:;" Size="Small" Text="Resources.Local.btnImage.Text" />
                            </Buttons>
                        </egvc:ToolbarButtonGroup>
                        <egvc:ToolbarButtonGroup runat="server">
                            <Buttons>
                                <egvc:ToolbarButton ID="btnRemoveImage" Color="Danger" IconClass="fa fa-times" IsHyperlink="true" NavigateUrl="javascript:;" Size="Small" Text="Resources.Local.btnRemoveImage.Text" />
                            </Buttons>
                        </egvc:ToolbarButtonGroup>
                    </ButtonGroups>
                </egvc:Toolbar>
                <egvc:FileUploader ID="egvFileUploader" runat="server" ClientIDMode="Static" IsCMSPath="true" UploadPath="profile-images" style="display: none;" AllowMultiple="false" />
                <egvc:ImageEditor ID="egvImageEditor" runat="server" IsCMS="true" />
            </egvc:EGVTabContent>
        </TabContents>
        <TabFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </TabFooter>
    </egvc:EGVTabs>
</asp:Content>
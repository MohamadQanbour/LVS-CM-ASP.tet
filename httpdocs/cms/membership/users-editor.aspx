<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="users-editor.aspx.vb" Inherits="cms_membership_users_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hdnLanguageId" runat="server" ClientIDMode="Static" />
    <egvc:EGVTabs ID="egvTabs" runat="server" Title="Resources.Local.egvTabs.Title" IconClass="fa fa-graduation-cap">
        <Tabs>
            <egvc:EGVTabItem Title="Resources.Local.egvTabs.General" runat="server" ID="tabGeneral" />
            <egvc:EGVTabItem Title="Resources.Local.egvTabs.Password" runat="server" Id="tabPassword" />
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
                        <egvc:RowItem runat="server" Title="Resources.Local.rowIdLogin.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtUsername" runat="server" MaxLength="50" Placeholder="Resources.Local.txtUsername.Placeholder" />
                                <egvc:EGVRequired ID="reqUsername" runat="server" ControlToValidate="txtUsername" ValidationGroup="valSave" 
                                    ErrorMessage="Resources.Local.reqUsername.ErrorMessage" />
                                <egvc:EGVCustomValidator ID="valUsername" runat="server" ControlToValidate="txtUsername" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.valUsername.ErrorMessage" OnServerValidate="valUsername_Validate" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowFullName.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtFullName" runat="server" MaxLength="50" />
                                <egvc:EGVRequired ID="reqFullName" runat="server" ControlToValidate="txtFullName" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.reqFullName.ErrorMessage" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowFatherName.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtFatherName" runat="server" MaxLength="50" />
                                <egvc:EGVRequired ID="reqFatherName" runat="server" ControlToValidate="txtFatherName" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.reqFatherName.ErrorMessage" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowMotherName.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtMotherName" runat="server" MaxLength="50" />
                                <egvc:EGVRequired ID="reqMotherName" runat="server" ControlToValidate="txtMotherName" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.reqMotherName.ErrorMessage" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowFamily.Text" Required="true">
                            <Content>
                                <div class="input-group">
                                    <egvc:EGVDropDown ID="ddlFamily" runat="server" ClientIDMode="Static" AllowNull="false" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/Families" style="width: 100%;" />
                                    <span class="input-group-btn">
                                        <egvc:EGVHyperLink ID="hypAddFamily" runat="server" NavigateUrl="#" data-toggle="modal" data-target="#egvFamilyModal" Color="Success">
                                            <span class="fa fa-plus"></span>
                                        </egvc:EGVHyperLink>
                                    </span>
                                </div>
                                <asp:HiddenField ID="hdnSelectedFamily" runat="server" ClientIDMode="Static" />
                                <egvc:Modal ID="egvFamilyModal" runat="server" Size="Large" Title="Resources.Local.egvFamilyModal.Title" ClientIDMode="Static">
                                    <Body>
                                        <egvc:EGVInputForm ID="egvIFFamily" runat="server" NoBorders="true">
                                            <Rows>
                                                <egvc:RowItem runat="server" Title="Resources.Local.rowFamilyUserName.Text" Required="true">
                                                    <Content>
                                                        <egvc:EGVTextBox ID="txtFamilyUserName" runat="server" MaxLength="50" Placeholder="Resources.Local.txtFamilyUsername.Placeholder" />
                                                        <egvc:EGVRequired ID="reqFamilyUserName" runat="server" ControlToValidate="txtFamilyUserName" ValidationGroup="valAddFamily"
                                                            ErrorMessage="Resources.Local.reqFamilyUserName.ErrorMessage" />
                                                    </Content>
                                                </egvc:RowItem>
                                            </Rows>
                                        </egvc:EGVInputForm>
                                    </Body>
                                    <Footer>
                                        <egvc:EGVLinkButton ID="lnkAddFamily" runat="server" ClientIDMode="Static" Color="Primary" Text="Resources.Local.lnkAddFamily.Text" ValidationGroup="valAddFamily" CssClass="pull-right" />
                                    </Footer>
                                </egvc:Modal>
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowArea.Text" Required="true">
                            <Content>
                                <div class="input-group">
                                    <egvc:EGVDropDown ID="ddlArea" runat="server" ClientIDMode="Static" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/Areas" />
                                    <span class="input-group-btn">
                                        <egvc:EGVHyperLink ID="hypAddArea" runat="server" NavigateUrl="#" data-toggle="modal" data-target="#egvAreaModal" Color="Success">
                                            <span class="fa fa-plus"></span>
                                        </egvc:EGVHyperLink>
                                    </span>
                                </div>
                                <asp:HiddenField ID="hdnSelectedArea" runat="server" ClientIDMode="Static" />
                                <egvc:Modal ID="egvAreaModal" runat="server" Size="Large" Title="Resources.Local.egvAreaModal.Title" ClientIDMode="Static">
                                    <Body>
                                        <egvc:EGVInputForm ID="egvIFArea" runat="server" NoBorders="true">
                                            <Rows>
                                                <egvc:RowItem runat="server" Title="Resources.Local.rowAreaTitle.Text" Required="true">
                                                    <Content>
                                                        <egvc:EGVTextBox ID="txtAreaTitle" runat="server" MaxLength="50" />
                                                        <egvc:EGVRequired ID="reqAreaTitle" runat="server" ControlToValidate="txtAreaTitle" ValidationGroup="valAddArea"
                                                            ErrorMessage="Resources.Local.reqAreaTitle.ErrorMessage" />
                                                    </Content>
                                                </egvc:RowItem>
                                            </Rows>
                                        </egvc:EGVInputForm>
                                    </Body>
                                    <Footer>
                                        <egvc:EGVLinkButton ID="lnkAddArea" runat="server" ClientIDMode="Static" Color="Primary" Text="Resources.Local.lnkAddArea.Text" ValidationGroup="valAddArea" CssClass="pull-right" />
                                    </Footer>
                                </egvc:Modal>
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowEmail.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtEmail" runat="server" MaxLength="255" />
                                <egvc:EGVRequired ID="reqEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.reqEmail.ErrorMessage" Visible="false" />
                                <egvc:EGVRegxValidator ID="regEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.regEmail.ErrorMessage" ValidationType="Email" Visible="false" />
                                <egvc:EGVCustomValidator ID="valEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.valEmail.ErrorMessage" OnServerValidate="valEmail_Validate" Visible="false" />
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
                        <egvc:RowItem runat="server" Title="Resources.Local.rowSection.Text" Required="true">
                            <Content>
                                <egvc:EGVDropDown ID="ddlSection" runat="server" ClientIDMode="Static" AllowNull="false" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/Sections" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowRecord.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtRecord" runat="server" MaxLength="100" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowReligion.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtReligion" runat="server" MaxLength="50" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowGender.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtGender" runat="server" MaxLength="50" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowBirth.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtBirth" runat="server" MaxLength="100" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowPhoneNumber.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtPhoneNumber" runat="server" MaxLength="50" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowFatherPhoneNumber.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtFatherPhoneNumber" runat="server" MaxLength="50" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowMotherPhoneNumber.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtMotherPhoneNumber" runat="server" MaxLength="50" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowLandlinePhoneNumber.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtLandlinePhoneNumber" runat="server" MaxLength="50" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowFatherWork">
                            <Content>
                                <egvc:EGVTextBox ID="txtFatherWork" runat="server" MaxLength="255" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowMotherWork">
                            <Content>
                                <egvc:EGVTextBox ID="txtMotherWork" runat="server" MaxLength="255" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowActive.Text">
                            <Content>
                                <div class="icheck">
                                    <egvc:EGVCheckBox ID="chkActive" runat="server" Text="Resources.Local.chkActive.Text" />
                                </div>
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem ID="rowLastLogin" runat="server" Title="Resources.Local.rowLastLogin.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtLastLogin" runat="server" ReadOnly="true" />
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
        </TabContents>
        <TabFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </TabFooter>
    </egvc:EGVTabs>
</asp:Content>
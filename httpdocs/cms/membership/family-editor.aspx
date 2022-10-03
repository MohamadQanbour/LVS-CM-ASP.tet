<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="family-editor.aspx.vb" Inherits="cms_membership_family_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVTabs ID="egvTabs" runat="server" Title="Resources.Local.egvTabs.Title" IconClass="fa fa-user-secret">
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
                        <egvc:RowItem runat="server" Title="Resources.Local.rowUsername.Text" Required="true">
                            <Content>
                                <egvc:EGVTextBox ID="txtUsername" runat="server" MaxLength="50" Placeholder="Resources.Local.txtUsername.Placeholder" />
                                <egvc:EGVRequired ID="reqUsername" runat="server" ControlToValidate="txtUsername" ValidationGroup="valSave" 
                                    ErrorMessage="Resources.Local.reqUsername.ErrorMessage" />
                                <egvc:EGVCustomValidator ID="valUsername" runat="server" ControlToValidate="txtUsername" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.valUsername.ErrorMessage" OnServerValidate="valUsername_Validate" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowEmail.Text">
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
                        <egvc:RowItem runat="server" Title="Resources.Local.rowFullName.Text">
                            <Content>
                                <egvc:EGVTextBox ID="txtFullName" runat="server" MaxLength="50" />
                                <egvc:EGVRequired ID="reqFullName" runat="server" ControlToValidate="txtFullName" ValidationGroup="valSave"
                                    ErrorMessage="Resources.Local.reqFullName.ErrorMessage" Visible="false" />
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
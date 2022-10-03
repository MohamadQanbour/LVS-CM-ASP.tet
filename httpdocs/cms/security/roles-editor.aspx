<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="roles-editor.aspx.vb" Inherits="cms_security_roles_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVTabs ID="egvTabs" runat="server" Title="Resources.Local.egvTabs.Title" IconClass="fa fa-shield">
        <Tabs>
            <egvc:EGVTabItem Title="Resources.Local.egvTabs.General" runat="server" ID="tabGeneral" />
            <egvc:EGVTabItem Title="Resources.Local.egvTabs.Permissions" runat="server" ID="tabPermissions" />
            <egvc:EGVTabItem Title="Resources.Local.egvTabs.Languages" runat="server" Id="tabLanguages" />
        </Tabs>
        <TabContents>
            <egvc:EGVTabContent runat="server">
                <egvc:EGVInputForm ID="egvIF" runat="server" NoBorders="true">
                    <Rows>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowId.Text">
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
                        <egvc:RowItem runat="server" Title="Resources.Local.rowSecure.Text">
                            <Content>
                                <div class="icheck">
                                    <egvc:EGVCheckBox ID="chkSecure" runat="server" Text="Resources.Local.chkSecure.Text" />
                                </div>
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowActive.Text">
                            <Content>
                                <div class="icheck">
                                    <egvc:EGVCheckBox ID="chkActive" runat="server" Text="Resources.Local.chkActive.Text" />
                                </div>
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowAdmin.Text">
                            <Content>
                                <div class="icheck">
                                    <egvc:EGVCheckBox ID="chkAdmin" runat="server" Text="Resources.Local.chkAdmin.Text" />
                                </div>
                            </Content>
                        </egvc:RowItem>
                    </Rows>
                </egvc:EGVInputForm>
            </egvc:EGVTabContent>
            <egvc:EGVTabContent runat="server">
                <asp:Repeater ID="rptPermissions" runat="server">
                    <HeaderTemplate>
                        <table class="table table-hover egvGrid">
                            <tr class="bg-navy">
                                <th><%=Localization.GetResource("Resources.Local.grdPermissions.Permission") %></th>
                                <th class="text-center"><%=Localization.GetResource("Resources.Local.grdPermissions.CanRead") %></th>
                                <th class="text-center"><%=Localization.GetResource("Resources.Local.grdPermissions.CanWrite") %></th>
                                <th class="text-center"><%=Localization.GetResource("Resources.Local.grdPermissions.CanModify") %></th>
                                <th class="text-center"><%=Localization.GetResource("Resources.Local.grdPermissions.CanPublish") %></th>
                                <th class="text-center"><%=Localization.GetResource("Resources.Local.grdPermissions.CanDelete") %></th>
                            </tr>
                    </HeaderTemplate>
                    <FooterTemplate></table></FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="bg-olive"><%#Eval("Title") %></td>
                            <td class="form-group text-center"><div class="icheck"><input type="checkbox" data-permission='<%#Eval("Id") %>' data-type="Read" id="chkRead<%#Eval("Id") %>" /></div></td>
                            <td class="form-group text-center"><div class="icheck"><input type="checkbox" data-permission='<%#Eval("Id") %>' data-type="Write" id="chkWrite<%#Eval("Id") %>" /></div></td>
                            <td class="form-group text-center"><div class="icheck"><input type="checkbox" data-permission='<%#Eval("Id") %>' data-type="Modify" id="chkModify<%#Eval("Id") %>" /></div></td>
                            <td class="form-group text-center"><div class="icheck"><input type="checkbox" data-permission='<%#Eval("Id") %>' data-type="Publish" id="chkPublish<%#Eval("Id") %>" /></div></td>
                            <td class="form-group text-center"><div class="icheck"><input type="checkbox" data-permission='<%#Eval("Id") %>' data-type="Delete" id="chkDelete<%#Eval("Id") %>" /></div></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:HiddenField ID="hdnPermissions" runat="server" ClientIDMode="Static" />
            </egvc:EGVTabContent>
            <egvc:EGVTabContent runat="server">
                <asp:Repeater ID="rptLanguages" runat="server">
                    <HeaderTemplate>
                        <table class="table table-hover egvGrid">
                            <tr class="bg-navy">
                                <th><%=Localization.GetResource("Resources.Local.grdLanguages.Language") %></th>
                                <th class="text-center"><%=Localization.GetResource("Resources.Local.grdLanguages.AllowTranslate") %></th>
                            </tr>
                    </HeaderTemplate>
                    <FooterTemplate></table></FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="bg-olive"><%#Eval("Title") %></td>
                            <td class="form-group text-center"><div class="icheck"><input type="checkbox" data-language='<%#Eval("Id") %>' id="chkLanguage<%#Eval("Id") %>" /></div></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:HiddenField ID="hdnLanguages" runat="server" ClientIDMode="Static" />
            </egvc:EGVTabContent>
        </TabContents>
        <TabFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </TabFooter>
    </egvc:EGVTabs>
</asp:Content>
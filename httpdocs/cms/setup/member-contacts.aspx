<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="member-contacts.aspx.vb" Inherits="cms_setup_member_contacts" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVTabs ID="egvTabs" runat="server" Title="Resources.Local.egvTabs.Title" IconClass="fa fa-fax">
        <Tabs>
            <egvc:EGVTabItem Title="Resources.Local.egvTabs.Families" runat="server" ID="tabFamily" />
            <egvc:EGVTabItem Title="Resources.Local.egvTabs.Students" runat="server" ID="tabStudents" />
        </Tabs>
        <TabContents>
            <egvc:EGVTabContent runat="server">
                <asp:Repeater ID="rptFamilies" runat="server">
                    <HeaderTemplate>
                        <table class="table table-hover egvGrid">
                            <tr class="bg-navy">
                                <th><%=Localization.GetResource("Resources.Local.grdRoles.RoleName") %></th>
                                <th class="text-center"><%=Localization.GetResource("Resources.Local.grdRoles.CanContact") %></th>
                                <th class="text-center"><%=Localization.GetResource("Resources.Local.grdRoles.ClassDependent") %></th>
                            </tr>
                    </HeaderTemplate>
                    <FooterTemplate></table></FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="bg-olive"><%#Eval("Title") %></td>
                            <td class="form-group text-center"><div class="icheck"><input type="checkbox" id="chkFamilyContact<%#Eval("Id") %>" data-roleid='<%#Eval("Id") %>' data-type="family-contact" /></div></td>
                            <td class="form-group text-center"><div class="icheck"><input type="checkbox" id="chkFamilyClass<%#Eval("Id") %>" data-roleid='<%#Eval("Id") %>' data-type="family-class" /></div></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:HiddenField ID="hdnFamilies" runat="server" ClientIDMode="Static" Value="[]" />
            </egvc:EGVTabContent>
            <egvc:EGVTabContent runat="server">
                <asp:Repeater ID="rptStudents" runat="server">
                    <HeaderTemplate>
                        <table class="table table-hover egvGrid">
                            <tr class="bg-navy">
                                <th><%=Localization.GetResource("Resources.Local.grdRoles.RoleName") %></th>
                                <th class="text-center"><%=Localization.GetResource("Resources.Local.grdRoles.CanContact") %></th>
                                <th class="text-center"><%=Localization.GetResource("Resources.Local.grdRoles.ClassDependent") %></th>
                            </tr>
                    </HeaderTemplate>
                    <FooterTemplate></table></FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="bg-olive"><%#Eval("Title") %></td>
                            <td class="form-group text-center"><div class="icheck"><input type="checkbox" id="chkStudentContact<%#Eval("Id") %>" data-roleid='<%#Eval("Id") %>' data-type="student-contact" /></div></td>
                            <td class="form-group text-center"><div class="icheck"><input type="checkbox" id="chkStudentClass<%#Eval("Id") %>" data-roleid='<%#Eval("Id") %>' data-type="student-class" /></div></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:HiddenField ID="hdnStudents" runat="server" ClientIDMode="Static" Value="[]" />
            </egvc:EGVTabContent>
        </TabContents>
        <TabFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </TabFooter>
    </egvc:EGVTabs>
</asp:Content>
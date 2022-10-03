<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="material-users.aspx.vb" Inherits="cms_special_material_users" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server" IconClass="fa fa-book" Title="Resources.Local.egvGrid.Title" Type="Primary" DefaultReturnButton="btnLoad">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.Class">
                <Content>
                    <egvc:EGVDropDown ID="ddlClasses" runat="server" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.Section">
                <Content>
                    <egvc:EGVDropDown ID="ddlSections" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdnSelectedSection" runat="server" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:EGVLinkButton ID="btnLoad" runat="server" Color="Warning" Text="Resources.Local.btnLoad.Text" CssClass="pull-right" />
        </FormFooter>
    </egvc:EGVInputForm>
    <egvc:Box ID="egvBox" runat="server" ClientIDMode="Static">
        <BoxBody>
            <div class="row">
                <div class="col-sm-12">
                    <asp:Repeater ID="rptMaterials" runat="server">
                        <HeaderTemplate>
                            <table class="table table-hover egvGrid">
                                <tr class="bg-navy">
                                    <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.rptMaterials.Material") %></th>
                                    <th class="text-center"><%=Localization.GetResource("Resources.Local.rptMaterials.Teacher") %></th>
                                </tr>
                        </HeaderTemplate>
                        <FooterTemplate></table></FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="bg-olive"><%#Eval("Title") %></td>
                                <td>
                                    <div class="form-group" data-materialid='<%#Eval("Id") %>'>
                                        <egvc:EGVDropDown ID="ddlTeacher" runat="server" Enabled='<%#CanUpdate %>' />
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </BoxBody>
        <BoxFooter>
            <asp:HiddenField ID="hdnValues" runat="server" ClientIDMode="Static" />
            <egvc:EGVLinkButton ID="lnkSave" runat="server" CssClass="pull-right" Text="Resources.Local.lnkSave.Text" Color="Primary" />
        </BoxFooter>
    </egvc:Box>
</asp:Content>
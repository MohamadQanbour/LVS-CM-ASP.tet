<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="classes-explorer.aspx.vb" Inherits="cms_special_classes_explorer" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server" Title="Resources.Local.egvGrid.Title" IconClass="fa fa-university" Type="Primary" DefaultReturnButton="btnLoad">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.Class">
                <Content>
                    <egvc:EGVDropDown ID="ddlClasses" runat="server" ClientIDMode="Static" />
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
                    <asp:Repeater ID="rptItems" runat="server">
                        <HeaderTemplate>
                            <table class="table table-hover egvGrid">
                                <tr class="bg-navy">
                                    <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.rptItems.Section") %></th>
                                    <th class="text-center"><%=Localization.GetResource("Resources.Local.rptItems.Material") %></th>
                                </tr>
                        </HeaderTemplate>
                        <FooterTemplate></table></FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="bg-olive"><%#Eval("Title") %></td>
                                <td>
                                    <table class="table table-hover egvGrid">
                                        <asp:Repeater ID="rptMaterials" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%#Eval("MaterialTitle") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </table>
                                    <div class="text-right">
                                        <asp:HyperLink ID="hypSchedule" runat="server" Target="_blank" CssClass="btn btn-primary"><%#Localization.GetResource("Resources.Local.ViewSchedule") %></asp:HyperLink>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </BoxBody>
    </egvc:Box>
</asp:Content>
<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="class-days.aspx.vb" Inherits="cms_special_class_days" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:Box  runat="server" IconClass="fa fa-calendar" Title="Resources.Local.egvGrid.Title" Type="Primary" DefaultReturnButton="btnSave">
        <BoxBody>
            <asp:Repeater ID="rptClasses" runat="server">
                <HeaderTemplate>
                    <table class="table table-hover egvGrid">
                        <tr class="bg-navy">
                            <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.rptClasses.ClassTitle") %></th>
                            <th class="text-center"><%=Localization.GetResource("Resources.Local.rptClasses.SchoolDays") %></th>
                            <th class="text-center"><%=Localization.GetResource("Resources.Local.rptClasses.HolidayDays") %></th>
                        </tr>
                </HeaderTemplate>
                <FooterTemplate></table></FooterTemplate>
                <ItemTemplate>
                    <tr data-classid='<%#Eval("ClassId") %>' id="tr" runat="server">
                        <td class="bg-olive"><%#Eval("ClassTitle") %></td>
                        <td data-type="school">
                            <div class="col-md-6"><egvc:EGVTextBox ID="txtSchoolDays" runat="server" InputNumber="true" Text='<%#Eval("SchoolDays") %>' Enabled='<%#CanUpdate %>' /></div>
                            <div class="col-md-6"><egvc:EGVTextBox ID="txtSchoolDays2" runat="server" InputNumber="true" Text='<%#Eval("SchoolDays2") %>' Enabled='<%#CanUpdate %>' /></div>
                        </td>
                        <td data-type="holiday">
                            <div class="col-md-6"><egvc:EGVTextBox ID="txtHolidayDays" runat="server" InputNumber="true" Text='<%#Eval("HolidayDays") %>' Enabled='<%#CanUpdate %>' /></div>
                            <div class="col-md-6"><egvc:EGVTextBox ID="txtHolidayDays2" runat="server" InputNumber="true" Text='<%#Eval("HolidayDays2") %>' Enabled='<%#CanUpdate %>' /></div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </BoxBody>
        <BoxFooter>
            <egvc:EGVLinkButton ID="btnSave" runat="server" Color="Primary" Text="Resources.Local.btnSave.Click" CssClass="pull-right" />
        </BoxFooter>
    </egvc:Box>
</asp:Content>
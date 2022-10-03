<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="attendance.aspx.vb" Inherits="cms_special_attendance" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server" IconClass="fa fa-calendar-check-o" Title="Resources.Local.egvIF.Title" Type="Primary" DefaultReturnButton="btnLoad">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowDate.Title" Required="true">
                <Content>
                    <egvc:EGVTextBox ID="txtDate" runat="server" DatePicker="true" />
                    <egvc:EGVRequired ID="reqDate" runat="server" ControlToValidate="txtDate"
                        ValidationGroup="valDate" ErrorMessage="Resources.Local.reqDate.ErrorMessage" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowClass.Title" Required="true">
                <Content>
                    <egvc:EGVDropDown ID="ddlClasses" runat="server" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowSection.Title" Required="true">
                <Content>
                    <egvc:EGVDropDown ID="ddlSections" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdnSelectedSection" runat="server" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <asp:HiddenField ID="hdnValues" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="hdnUserId" runat="server" ClientIDMode="Static" />
            <egvc:EGVLinkButton ID="btnLoad" runat="server" Color="Warning" Text="Resources.Local.btnLoad.Text" CssClass="pull-right" ValidationGroup="valDate" ClientIDMode="Static" />
        </FormFooter>
    </egvc:EGVInputForm>
    <egvc:Box ID="egvBox" runat="server" ClientIDMode="Static">
        <BoxBody>
            <div class="row">
                <div class="col-sm-12">
                    <asp:Repeater ID="rptStudents" runat="server">
                        <HeaderTemplate>
                            <table class="table table-hover egvGrid">
                                <tr class="bg-navy">
                                    <th style="width: 50%;"><%=Localization.GetResource("Resources.Local.rptStudents.Student") %></th>
                                    <th class="text-center"><%=Localization.GetResource("Resources.Local.rptStudents.Present") %></th>
                                </tr>
                        </HeaderTemplate>
                        <FooterTemplate></table></FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="bg-olive"><%#Eval("StudentName") %></td>
                                <td class="text-center">
                                    <div class="icheck">
                                        <input type="checkbox" id="chkAttend<%#Eval("StudentId") %>" data-type="attend" data-studentid='<%#Eval("StudentId") %>'<%#IIf(CanUpdate, "", " disabled=""disabled""") %> />
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </BoxBody>
        <BoxFooter>
            <egvc:EGVLinkButton ID="lnkSave" runat="server" CssClass="pull-right" Text="Resources.Local.lnkSave.Text" Color="Primary" />
        </BoxFooter>
    </egvc:Box>
</asp:Content>
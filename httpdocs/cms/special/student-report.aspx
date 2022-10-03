<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="student-report.aspx.vb" Inherits="cms_special_student_report" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hdnUserId" runat="server" ClientIDMode="Static" />
    <egvc:EGVInputForm ID="ifSections" runat="server" NoBorders="true">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.Class">
                <Content>
                    <egvc:EGVDropDown ID="ddlClass" runat="server" ClientIDMode="Static" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherClasses" />
                    <asp:HiddenField ID="hdnSelectedClass" runat="server" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.Section">
                <Content>
                    <egvc:EGVDropDown ID="ddlSection" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hdnSeletedSection" runat="server" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.ExamTemplates">
                <Content>
                    <asp:HiddenField ID="hdnSelectedTemplates" runat="server" ClientIDMode="Static" Value="[]" />
                    <div class="row" id="tblTemplates">

                    </div>
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:EGVLinkButton ID="lnkLoadStudents" runat="server" Color="Warning" ClientIDMode="Static" Text="Resources.Local.Load" CssClass="pull-right" />
        </FormFooter>
    </egvc:EGVInputForm>
    <egvc:Box ID="egvStudentBox" runat="server" ClientIDMode="Static" Type="Success" data-command="box" data-argument="student" Visible="false">
        <BoxBody>
            <div class="row">
                <div class="col-md-12 table-responsive">
                    <asp:Literal ID="litTitle" runat="server" />                 
                    <table class="table table-hover egvGrid">
                        <tr class="bg-navy" data-command="title">
                            <th style="width: 100px;"><%=Localization.GetResource("Resources.Local.Number") %></th>
                            <th><%=Localization.GetResource("Resources.Local.Student") %></th>
                            <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.Total") %></th>
                            <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.Percentage") %></th>
                        </tr>
                        <asp:Repeater ID="rptItems" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%#Container.ItemIndex + 1 %></td>
                                    <td><%#Eval("FullName") %></td>
                                    <td><%#Eval("Mark") %></td>
                                    <td><%#CDec(Eval("Percentage")).ToString("0.00") & " %" %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </div>
            </div>
        </BoxBody>
        <BoxTools>
            <asp:HiddenField ID="hdnBoxClass" runat="server" />
            <asp:HiddenField ID="hdnBoxSection" runat="server" />
            <egvc:EGVLinkButton ID="lnkExcel" runat="server" Color="Success"><i class="fa fa-file-excel-o"></i>&nbsp;Export to Excel</egvc:EGVLinkButton>
        </BoxTools>        
    </egvc:Box>
</asp:Content>
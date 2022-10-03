<%@ Page Title="" Language="VB" MasterPageFile="CMSMaster.master" AutoEventWireup="false" CodeFile="previous.aspx.vb" Inherits="cms_previous" %>
<%@ MasterType VirtualPath="CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server" IconClass="fa fa-calendar" Title="Resources.Local.egvIFTitle" DefaultReturnButton="btnLoad" Type="Primary">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowSeason">
                <Content>
                    <egvc:EGVDropDown ID="ddlSeason" runat="server" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowData">
                <Content>
                    <egvc:EGVDropDown ID="ddlData" runat="server" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:EGVLinkButton ID="btnLoad" runat="server" Color="Warning" Text="Resources.Local.btnLoad.Text" CssClass="pull-right" />
        </FormFooter>
    </egvc:EGVInputForm>
    <egvc:EGVGridView ID="egvSectionsGrid" runat="server" BusinessClass="previous/Section" Title="Resources.Local.Sections" IconClass="fa fa-bell-o" OnGridNeedDataSource="grid_DataSource" />
    <egvc:EGVGridView ID="egvSectionAdminsGrid" runat="server" BusinessClass="previous/SectionAdmins" Title="Resources.Local.SectionAdmins" IconClass="fa fa-user-secret" OnGridNeedDataSource="grid_DataSource" />
    <egvc:EGVGridView ID="egvSectionMaterialUser" runat="server" BusinessClass="previous/SectionMaterialUser" Title="Resources.Local.MaterialTeachers" IconClass="fa fa-book" OnGridNeedDataSource="grid_DataSource" />
    <egvc:EGVGridView ID="egvStudentAttendance" runat="server" BusinessClass="previous/StudentAttendance" Title="Resources.Local.Attendance" IconClass="fa fa-calendar-check-o" OnGridNeedDataSource="grid_DataSource" />
    <egvc:EGVGridView ID="egvExam" runat="server" BusinessClass="previous/Exam" Title="Resources.Local.Exams" IconClass="fa fa-file-text-o" />
</asp:Content>
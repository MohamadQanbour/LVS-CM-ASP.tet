<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="import-excel.aspx.vb" Inherits="cms_membership_import_excel" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hdnSelectedFile" runat="server" ClientIDMode="Static" />
    <egvc:EGVInputForm ID="egvIF" runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.UploadFile" Required="true">
                <Content>
                    <egvc:SingleFileUploader ID="fileExcel" runat="server" Text="Resources.Local.SelectFile" AllowedFileTypes=".csv" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:EGVLinkButton ID="lnkStartUpload" runat="server" Text="Resources.Local.StartUpload" Color="Maroon" CssClass="pull-right btn-upload" />            
        </FormFooter>
    </egvc:EGVInputForm>
    <egvc:Box ID="bxImporting" runat="server" Title="Resources.Local.bxImporting" Type="Info" Visible="false">
        <BoxBody>
            <div class="text-center">
                <b><%=Localization.GetResource("Resources.Local.FileName") %></b>&nbsp;<asp:Literal ID="litFile" runat="server" /><br />
            </div>
            <div class="text-center">
                <a href="#" data-command="startimport" class="btn btn-primary btn-block"><%=Localization.GetResource("Resources.Local.BeginImport") %></a>
            </div>
            <br />
            <div class="text-center" id="pnlProgress" style="display: none;">
                <div class="progress">
                    <div class="progress-bar progress-bar-primary progress-bar-striped active" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>
                </div>
            </div>
            <br />
            <div id="pnlErrors" style="display: none;"></div>
            <div class="text-center" id="pnlClose" style="display: none;">
                <%=Localization.GetResource("Resources.Local.Completed") %>
                <a href="payments.aspx" class="btn btn-default pull-right"><%=Localization.GetResource("Resources.Local.Close") %></a>
            </div>
        </BoxBody>
    </egvc:Box>
</asp:Content>
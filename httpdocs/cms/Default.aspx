<%@ Page Title="" Language="VB" MasterPageFile="CMSMaster.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="cms_Default" %>
<%@ MasterType VirtualPath="CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<%@ Import Namespace="EGV.Business" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="row"> 
        <div class="col-md-4">
            <div class="info-box">
                <span class="info-box-icon bg-maroon"><i class="fa fa-users"></i></span>
                <div class="info-box-content">
                    <span class="info-box-text"><%=Localization.GetResource("Resources.Local.TotalStudents") %></span>
                    <span class="info-box-number"><%=StudentController.GetTotal(MyConn).ToString("0,0") %> <small><%=Localization.GetResource("Resources.Local.Student") %></small></span>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="info-box">
                <span class="info-box-icon bg-maroon"><i class="fa fa-book"></i></span>
                <div class="info-box-content">
                    <span class="info-box-text"><%=Localization.GetResource("Resources.Local.TotalMaterials") %></span>
                    <span class="info-box-number"><%=MaterialController.GetTotal(MyConn).ToString("0,0") %> <small><%=Localization.GetResource("Resources.Local.Material") %></small></span>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="info-box">
                <span class="info-box-icon bg-maroon"><i class="fa fa-sticky-note"></i></span>
                <div class="info-box-content">
                    <span class="info-box-text"><%=Localization.GetResource("Resources.Local.TotalNotes") %></span>
                    <div class="info-box-number">
                        <div><%=NoteController.GetNegativeTotal(MyConn).ToString("0") %> <small><%=Localization.GetResource("Resources.Local.NegativeNote") %></small></div>
                        <div><%=NoteController.GetPositiveTotal(MyConn).ToString("0") %> <small><%=Localization.GetResource("Resources.Local.PositiveNote") %></small></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <egvc:Box ID="bxTools" runat="server" Type="Primary" Title="Resources.Local.ToolsBox" IconClass="fa fa-cogs">
                <BoxBody>
                    <egvc:EGVHyperLink ID="btnExportToAccess" runat="server" AppButton="true" ClientIDMode="Static" NavigateUrl="#">
                        <i class="fa fa-database"></i><%=Localization.GetResource("Resources.Local.ExportToAccess") %>
                    </egvc:EGVHyperLink>
                    <egvc:EGVHyperLink ID="hypPrevious" runat="server" AppButton="true" NavigateUrl="previous.aspx">
                        <i class="fa fa-calendar"></i><%=Localization.GetResource("Resources.Local.ViewPreviousSeasons") %>
                    </egvc:EGVHyperLink>
                </BoxBody>
            </egvc:Box>
        </div>
    </div>
    <div class="row" style="display: none;" id="pnlExport">
        <div class="col-md-12">
            <egvc:Box ID="bxExport" runat="server" Type="Primary" Title="Resources.Local.ExportBox" IconClass="fa fa-database">
                <BoxTools><div class="pull-right" style="font-weight: bold;"><span id="pnlPercentage"></span></div></BoxTools>
                <BoxBody>
                    <div class="text-center">
                        <egvc:EGVHyperLink ID="hypStartExport" runat="server" data-function="export" NavigateUrl="#" Color="Primary" Text="Resources.Local.StartExport" />
                    </div>
                    <div class="text-center" id="pnlProgress" style="display: none;">
                        <div class="progress">
                            <div class="progress-bar progress-bar-primary progress-bar-striped active" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>
                        </div>
                        <div class="help-block">Migrating <span id="litStep"></span></div>
                    </div>
                    <div class="text-center" id="pnlDownload" style="display: none;">
                        <egvc:EGVHyperLink ID="hypDownload" runat="server" data-function="download" NavigateUrl="#" Color="Primary" Target="_blank" Text="Resources.Local.DownloadAccess" />
                    </div>
                </BoxBody>
            </egvc:Box>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <egvc:Box ID="egvBoxMail" runat="server" Type="Primary" Title="Resources.Local.QuickEmail" IconClass="fa fa-envelope-o">
                <BoxBody>
                    <div class="form-group">
                        <egvc:EGVDropDown ID="ddlTo" runat="server" AllowMultiple="true" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/MessagingTo" ClientIDMode="Static" Placeholder="Resources.Local.To" />
                        <asp:HiddenField ID="hdnSelectedTo" runat="server" ClientIDMode="Static" Value="[]" />
                        <asp:HiddenField ID="hdnSetSelected" runat="server" ClientIDMode="Static" Value="[]" />
                        <egvc:EGVCustomValidator ID="valTo" runat="server" ControlToValidate="ddlTo" ValidationGroup="valMessage" ErrorMessage="Resources.Local.reqTo.ErrorMessage" OnServerValidate="validate_To" />
                    </div>
                    <div class="form-group">
                        <egvc:EGVTextBox ID="txtSubject" runat="server" Placeholder="Resources.Local.Subject" />
                        <egvc:EGVRequired ID="reqSubject" runat="server" ControlToValidate="txtSubject" ValidationGroup="valMessage" ErrorMessage="Resources.Local.reqSubject.ErrorMessage" />
                    </div>
                    <div class="form-group">
                        <egvc:HTMLEditor ID="txtBody" runat="server" />
                    </div>
                </BoxBody>
                <BoxFooter>
                    <div class="pull-right">
                        <egvc:EGVLinkButton ID="btnSend" runat="server" Color="Primary" ValidationGroup="valMessage"><i class="fa fa-envelope-o"></i> <%=Localization.GetResource("Resources.Local.Send") %></egvc:EGVLinkButton>
                    </div>
                </BoxFooter>
            </egvc:Box>
        </div>
    </div>
</asp:Content>
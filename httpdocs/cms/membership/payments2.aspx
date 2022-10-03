<%@ Page Title="" Language="VB" MasterPageFile="~/cms/CMSMaster.master" AutoEventWireup="false" CodeFile="payments2.aspx.vb" Inherits="cms_membership_payments2" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVHyperLink ID="hypImport" runat="server" NavigateUrl="#" Color="Default" AppButton="true" data-toggle="modal" data-target=".sync-accounts">
        <i class="fa fa-file-excel-o"></i><%=EGV.Utils.Localization.GetResource("Resources.Local.lnkImport") %>
    </egvc:EGVHyperLink>
    <egvc:EGVGridView ID="egvGrid" runat="server" Title="Resources.Local.egvGrid.Title" IconClass="fa fa-money" BusinessClass="membership/Payments2">
        <Toolbar runat="server">
            <ButtonGroups>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnDelete" ActiveState="Multi" Color="Danger" IconClass="fa fa-trash-o" Size="Small" Text="Resources.Local.btnDelete.Text" CommandName="delete" ShouldConfirm="true" />
                    </Buttons>
                </egvc:ToolbarButtonGroup>
            </ButtonGroups>
        </Toolbar>
    </egvc:EGVGridView>
    <egvc:Modal ID="egvModal" runat="server" CssClass="sync-accounts" Size="Default" Title="Resources.Local.egvModalTitle">
        <Body DefaultButton="lnkStartSync">
            <div id="pnlButton">
                <asp:HiddenField ID="hdnSelectedFile" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdnReload" runat="server" ClientIDMode="Static" />
                <div class="help-block"><%=Localization.GetResource("Resources.Local.SyncIntro") %></div>
                <egvc:SingleFileUploader ID="txtSyncFile" runat="server" AllowedFileTypes="*.csv" InfoText="Resources.Local.txtSyncFileInfo" Text="Resources.Local.txtSyncFile" />
            </div>
            <div id="pnlsync" style="display: none;">
                <div><%=Localization.GetResource("Resources.Local.Syncing") %>&nbsp;<span id="pnlProgress">0</span>/<span id="pnlTotal">0</span></div>
                <div id="pnlErrors" class="text-danger"></div>
                <ul id="resultErrorsList" class="text-danger"></ul>
                <div id="pnlSuccess" class="text-success"></div>
            </div>
        </Body>
        <Footer>
            <egvc:EGVLinkButton ID="lnkStartSync" runat="server" BlockButton="true" Color="Primary" FlatButton="true" data-toggle="sync">
                <span class="fa fa-refresh"></span>&nbsp;<%=Localization.GetResource("Resources.Local.hypSync") %>
            </egvc:EGVLinkButton>
        </Footer>
    </egvc:Modal>
    <asp:HiddenField ID="hdnUserId" runat="server" ClientIDMode="Static" />
</asp:Content>
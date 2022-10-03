<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="users.aspx.vb" Inherits="cms_membership_users" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVHyperLink ID="hypAdd" runat="server" Color="Default" NavigateUrl="users-editor.aspx" AppButton="true">
        <i class="fa fa-plus"></i><%=GetLocalResourceObject("hypAdd.Text") %>
    </egvc:EGVHyperLink>
    <egvc:EGVHyperLink ID="hypSync" runat="server" Color="Default" NavigateUrl="#" AppButton="true" data-toggle="modal" data-target=".sync-students-modal">
        <i class="fa fa-refresh"></i><%=Localization.GetResource("Resources.Local.hypSync") %>
    </egvc:EGVHyperLink>
    <egvc:EGVHyperLink ID="hypSyncStudent" runat="server" Color="Default" NavigateUrl="#" AppButton="true" data-toggle="modal" data-target=".sync-students-profile-modal">
        <i class="fa fa-refresh"></i><%=Localization.GetResource("Resources.Local.hypSyncStudent") %>
    </egvc:EGVHyperLink>
    <egvc:EGVGridView ID="egvGrid" runat="server" Title="Resources.Local.egvGrid.Title" IconClass="fa fa-graduation-cap" BusinessClass="membership/Student">
        <Toolbar runat="server">
            <ButtonGroups>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnMoveToClass" ActiveState="Multi" Color="Orange" IconClass="fa fa-chevron-right" Size="Small" Text="Resources.Local.btnMoveToClass.Text" CommandName="move" IsHyperlink="true" NavigateUrl="#" />
                    </Buttons>
                </egvc:ToolbarButtonGroup>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnActivate" ActiveState="Multi" Color="Olive" IconClass="fa fa-check" Size="Small" Text="Resources.Local.btnActivate.Text" CommandName="activate" />
                        <egvc:ToolbarButton ID="btnDeactivate" ActiveState="Multi" Color="Maroon" IconClass="fa fa-times" Size="Small" Text="Resources.Local.btnDeactivate.Text" CommandName="deactivate" />
                    </Buttons>
                </egvc:ToolbarButtonGroup>
                <egvc:ToolbarButtonGroup runat="server">
                    <Buttons>
                        <egvc:ToolbarButton ID="btnDelete" ActiveState="Multi" Color="Danger" IconClass="fa fa-trash-o" Size="Small" Text="Resources.Local.btnDelete.Text" CommandName="delete" ShouldConfirm="true" />
                    </Buttons>
                </egvc:ToolbarButtonGroup>
            </ButtonGroups>
        </Toolbar>
    </egvc:EGVGridView>
    <egvc:Modal ID="modalMove" runat="server" ClientIDMode="Static" Size="Small" Title="Resources.Local.modalMove.Title">
        <Body CssClass="form-group">
            <egvc:EGVDropDown ID="ddlSections" runat="server" />
        </Body>
        <Footer>
            <egvc:EGVLinkButton ID="btnMove" runat="server" Text="Resources.Local.btnMove.Text" Color="Primary" />
        </Footer>
    </egvc:Modal>
    <egvc:Modal ID="egvSyncModal" runat="server" ClientIDMode="Static" Size="Default" Title="Resources.Local.egvSyncModal" CssClass="sync-students-modal">
        <Body DefaultButton="lnkStartSync">
            <div id="pnlButton">
                <asp:HiddenField ID="hdnSelectedFile" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdnReload" runat="server" ClientIDMode="Static" />
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
    <egvc:Modal ID="egvSyncStudentModal" runat="server" Size="Default" Title="Resources.Local.egvSyncStudentModal" CssClass="sync-students-profile-modal">
        <Body DefaultButton="lnkStartStudentSync">
            <div id="pnlButton2">
                <asp:HiddenField ID="hdnSelectedStudentFile" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdnReloadStudent" runat="server" ClientIDMode="Static" />
                <egvc:SingleFileUploader ID="txtSyncStudentFile" runat="server" AllowedFileTypes="*.csv" InfoText="Resources.Local.txtSyncStudentFileInfo" Text="Resources.Local.txtSyncStudentFile" />
            </div>
            <div id="pnlsync2" style="display: none;">
                <div><%=Localization.GetResource("Resources.Local.Syncing") %>&nbsp;<span id="pnlProgress2">0</span>/<span id="pnlTotal2">0</span></div>
                <div id="pnlErrors2" class="text-danger"></div>
                <ul id="resultErrorsList2" class="text-danger"></ul>
                <div id="pnlSuccess2" class="text-success"></div>
            </div>
        </Body>
        <Footer>
            <egvc:EGVLinkButton ID="lnkStartStudentSync" runat="server" BlockButton="true" Color="Primary" FlatButton="true" data-toggle="sync-student">
                <span class="fa fa-refresh"></span>&nbsp;<%=Localization.GetResource("Resources.Local.lnkStartStudentSync") %>
            </egvc:EGVLinkButton>
        </Footer>
    </egvc:Modal>
</asp:Content>
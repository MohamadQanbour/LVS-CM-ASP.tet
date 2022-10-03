s<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="compose.aspx.vb" Inherits="cms_messages_compose" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<%@ Import Namespace="EGV.Enums" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="row">
        <div class="col-md-3">
            <a href='<%=Path.MapCMSFile("messages/list.aspx?type=") & MessageFilterTypes.Inbox %>' class="btn btn-primary btn-block margin-bottom"><%=Localization.GetResource("Resources.Local.BackToInbox") %></a>
            <div class="box box-solid">
                <div class="box-header with-border">
                    <h3 class="box-title"><%=Localization.GetResource("Resources.Local.Folders") %></h3>
                    <div class="box-tools">
                        <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                        </button>
                    </div>
                    <div class="box-body no-padding">
                        <ul class="nav nav-pills nav-stacked">
                            <li id="liInbox" runat="server">
                                <a href='<%=Path.MapCMSFile("messages/list.aspx") %>'><i class="fa fa-inbox"></i> <%=Localization.GetResource("Resources.Local.Inbox") %></a>
                            </li>
                            <li id="liUnread" runat="server">
                                <a href='<%=Path.MapCMSFile("messages/list.aspx?type=" & MessageFilterTypes.Unread) %>'>
                                    <i class="fa fa-asterisk"></i> <%=Localization.GetResource("Resources.Local.Unread") %>
                                    <span class="label label-primary pull-right"><asp:Literal ID="litUnreadCount" runat="server" /></span>
                                </a>
                            </li>
                            <li id="liStarred" runat="server" visible="false">
                                <a href='<%=Path.MapCMSFile("messages/list.aspx?type=" & MessageFilterTypes.Starred) %>'>
                                    <i class="fa fa-star"></i> <%=Localization.GetResource("Resources.Local.Starred") %>
                                    <span class="label label-warning pull-right"><asp:Literal ID="litStarredCount" runat="server" /></span>
                                </a>
                            </li>
                            <li id="liSent" runat="server">
                                <a href='<%=Path.MapCMSFile("messages/list.aspx?type=" & MessageFilterTypes.Sent) %>'><i class="fa fa-paper-plane"></i> <%=Localization.GetResource("Resources.Local.Sent") %></a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-9">
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h3 class="box-title"><%=Localization.GetResource("Resources.Local.ComposeMessage") %></h3>
                </div>
                <div class="box-body">
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
                    <div class="form-group">
                        <div class="btn btn-default btn-file">
                            <i class="fa fa-paperclip"></i> <%=Localization.GetResource("Resources.Local.Attachment") %>
                            <asp:FileUpload ID="fileAttachment" runat="server" multiple ClientIDMode="Static" />
                        </div>
                        <p class="help-block"><%=Localization.GetResource("Resources.Local.MaxFileSize") %></p>
                        <p class="help-block" id="selectedFiles"></p>
                    </div>
                </div>
                <div class="box-footer">
                    <div class="pull-right">
                        <egvc:EGVLinkButton ID="btnSend" runat="server" Color="Primary" ValidationGroup="valMessage"><i class="fa fa-envelope-o"></i> <%=Localization.GetResource("Resources.Local.Send") %></egvc:EGVLinkButton>
                    </div>
                    <egvc:EGVHyperLink ID="hypBack" runat="server" Color="Default" NavigateUrl="list.aspx"><i class="fa fa-times"></i> <%=Localization.GetResource("Resources.Local.Discard") %></egvc:EGVHyperLink>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
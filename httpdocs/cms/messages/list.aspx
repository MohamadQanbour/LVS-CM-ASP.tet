<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="list.aspx.vb" Inherits="cms_messages_list" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<%@ Import Namespace="EGV.Enums" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hdnMessageType" runat="server" ClientIDMode="Static" />
    <div class="row">
        <div class="col-md-3">
            <a href='<%=Path.MapCMSFile("messages/compose.aspx") %>' class="btn btn-primary btn-block margin-bottom"><%=Localization.GetResource("Resources.Local.Compose") %></a>
            <div class="box box-solid">
                <div class="box-header with-border">
                    <h3 class="box-title"><%=Localization.GetResource("Resources.Local.Folders") %></h3>
                    <div class="box-tools">
                        <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                        </button>
                    </div>
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
        <div class="col-md-9">
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h3 class="box-title"><%=GetTitle(True) %></h3>
                    <div class="box-tools pull-right">
                        <div class="input-group input-group-sm pull-right" style="width: 150px;">
                            <egvc:EGVTextBox ID="txtSearch" runat="server" ControlSize="Small" Placeholder="Resources.Local.SearchMessages" />
                            <div class="input-group-btn">
                                <egvc:EGVLinkButton ID="lnkSearch" runat="server" Color="Default">
                                    <span class="glyphicon glyphicon-search"></span>
                                </egvc:EGVLinkButton>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="box-body no-padding">
                    <div class="mailbox-controls">
                        <button type="button" class="btn btn-default btn-sm checkbox-toggle"><i class="fa fa-square-o"></i></button>
                        <div class="btn-group">
                            <egvc:EGVLinkButton ID="lnkDelete" runat="server" Color="Default" Size="Small" OnClientClick="return confirm('Are you sure you want to delete selected messages?');" OnClick="btnDelete_Click"><i class="fa fa-trash-o"></i></egvc:EGVLinkButton>
                            <egvc:EGVLinkButton ID="lnkReply" runat="server" Color="Default" Size="Small" OnClick="btnReply_Click"><i class="fa fa-reply"></i></egvc:EGVLinkButton>
                            <egvc:EGVLinkButton ID="lnkReplyAll" runat="server" Color="Default" Size="Small" OnClick="btnReplyAll_Click"><i class="fa fa-reply-all"></i></egvc:EGVLinkButton>
                            <egvc:EGVLinkButton ID="lnkForward" runat="server" Color="Default" Size="Small" OnClick="btnForward_Click"><i class="fa fa-share"></i></egvc:EGVLinkButton>
                        </div>
                        <egvc:EGVLinkButton ID="lnkRefresh" runat="server" Color="Default" Size="Small" OnClick="Refresh"><i class="fa fa-refresh"></i></egvc:EGVLinkButton>
                        <div class="pull-right">
                            <asp:Literal id="litFirst" runat="server" /> - <asp:Literal ID="litLast" runat="server" /> / <asp:Literal ID="litTotal" runat="server" />
                            <div class="btn-group">
                                <egvc:EGVHyperLink ID="hypPrevious" runat="server" Size="Small" Color="Default"><i class="fa fa-chevron-left"></i></egvc:EGVHyperLink>
                                <egvc:EGVHyperLink ID="hypNext" runat="server" Size="Small" Color="Default"><i class="fa fa-chevron-right"></i></egvc:EGVHyperLink>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive mailbox-messages">
                        <table class="table table-hover table-striped">
                            <tbody>
                                <asp:Repeater ID="rptMessages" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><input type="checkbox" data-id='<%#Eval("Id").ToString() %>' /></td>
                                            <td class="mailbox-star" runat="server" visible="false"><a href="#" data-id='<%#Eval("Id").ToString() %>'><i class="fa fa-<%#IIf(Eval("IsStarred"), "star", "star-o") %> text-yellow"></i></a></td>
                                            <td class="mailbox-name"><a href='<%#Path.MapCMSFile("messages/read.aspx?id=" & Eval("Id").ToString() & "&type=" & MessagesFilterType) %>'><%#GetSenderName(MyConn, Eval("SenderUserId"), Eval("SenderUserType")) %></a></td>
                                            <td class="mailbox-subject"><%#IIf(Eval("IsRead"), Eval("Title"), "<b>" & Eval("Title") & "</b>") %></td>
                                            <td class="mailbox-attachment"><%#IIf(CInt(Eval("HasAttachments").ToString()) > 0, "<i class=""fa fa-paperclip""></i>", "")  %></td>
                                            <td class="mailbox-date"><%#Helper.FormateDateDifference(CDate(Eval("MessageDate")), True) %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                        <asp:HiddenField ID="hdnSelected" runat="server" ClientIDMode="Static" />
                        <div id="pnlNoMessage" runat="server" class="no-message" visible="false"><%=Localization.GetResource("Resources.Local.NoMessages") %></div>
                    </div>
                </div>
                <div class="box-footer no-padding">
                    <div class="mailbox-controls">
                        <button type="button" class="btn btn-default btn-sm checkbox-toggle"><i class="fa fa-square-o"></i></button>
                        <div class="btn-group">
                            <egvc:EGVLinkButton ID="lnkDelete2" runat="server" Color="Default" Size="Small" OnClientClick="return confirm('Are you sure you want to delete selected messages?');" OnClick="btnDelete_Click"><i class="fa fa-trash-o"></i></egvc:EGVLinkButton>
                            <egvc:EGVLinkButton ID="lnkReply2" runat="server" Color="Default" Size="Small" OnClick="btnReply_Click"><i class="fa fa-reply"></i></egvc:EGVLinkButton>
                            <egvc:EGVLinkButton ID="lnkReplyAll2" runat="server" Color="Default" Size="Small" OnClick="btnReplyAll_Click"><i class="fa fa-reply-all"></i></egvc:EGVLinkButton>
                            <egvc:EGVLinkButton ID="lnkForward2" runat="server" Color="Default" Size="Small" OnClick="btnForward_Click"><i class="fa fa-share"></i></egvc:EGVLinkButton>
                        </div>
                        <egvc:EGVLinkButton ID="lnkRefresh2" runat="server" Color="Default" Size="Small" OnClick="Refresh"><i class="fa fa-refresh"></i></egvc:EGVLinkButton>
                        <div class="pull-right">
                            <asp:Literal id="litFirst2" runat="server" /> - <asp:Literal ID="litLast2" runat="server" /> / <asp:Literal ID="litTotal2" runat="server" />
                            <div class="btn-group">
                                <egvc:EGVHyperLink ID="hypPrevious2" runat="server" Size="Small" Color="Default"><i class="fa fa-chevron-left"></i></egvc:EGVHyperLink>
                                <egvc:EGVHyperLink ID="hypNext2" runat="server" Size="Small" Color="Default"><i class="fa fa-chevron-right"></i></egvc:EGVHyperLink>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
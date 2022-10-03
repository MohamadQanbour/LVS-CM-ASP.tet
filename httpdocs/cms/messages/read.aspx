<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="read.aspx.vb" Inherits="cms_messages_read" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<%@ Import Namespace="EGV.Enums" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
                    <h3 class="box-title"><%=Localization.GetResource("Resources.Local.ReadMail") %></h3>
                    <div class="box-tools pull-right">
                        <asp:HyperLink ID="hypPrevious" runat="server" CssClass="btn btn-box-tool" data-toggle="tooltip"><i class="fa fa-chevron-left"></i></asp:HyperLink>
                        <asp:HyperLink ID="hypNext" runat="server" CssClass="btn btn-box-tool" data-toggle="tooltip"><i class="fa fa-chevron-right"></i></asp:HyperLink>
                        <egvc:EGVHyperLink ID="hypDetails" runat="server" CssClass="btn btn-primary btn-small" data-toggle="modal" data-target=".details-modal"><span class="fa fa-eye"></span></egvc:EGVHyperLink>
                    </div>
                </div>
                <div class="box-body no-padding">
                    <div class="mailbox-read-info">
                        <h3><asp:Literal ID="litMessageTitle" runat="server" /></h3>
                        <h5>
                            <b><%=Localization.GetResource("Resources.Local.From") %>:</b> <asp:Literal ID="litSenderName" runat="server" /><br />
                            <b><%=Localization.GetResource("Resources.Local.To") %>:</b> <asp:Literal ID="litTo" runat="server" />
                            <span class="mailbox-read-time pull-right"><asp:Literal ID="litMessageDate" runat="server" /></span>
                        </h5>
                    </div>
                    <div class="mailbox-controls with-border">
                        <div class="text-center">
                            <div class="btn-group">
                                <egvc:EGVLinkButton ID="lnkDelete" runat="server" Color="Default" Size="Small" OnClientClick="return confirm('Are you sure you want to delete selected messages?');" OnClick="btnDelete_Click"><i class="fa fa-trash-o"></i></egvc:EGVLinkButton>
                                <egvc:EGVLinkButton ID="lnkReply" runat="server" Color="Default" Size="Small" OnClick="btnReply_Click"><i class="fa fa-reply"></i></egvc:EGVLinkButton>
                                <egvc:EGVLinkButton ID="lnkReplyAll" runat="server" Color="Default" Size="Small" OnClick="btnReplyAll_Click"><i class="fa fa-reply-all"></i></egvc:EGVLinkButton>
                                <egvc:EGVLinkButton ID="lnkForward" runat="server" Color="Default" Size="Small" OnClick="btnForward_Click"><i class="fa fa-share"></i></egvc:EGVLinkButton>
                            </div>
                        </div>
                        <asp:Repeater ID="rptAttachment" runat="server">
                            <HeaderTemplate><ul class="mailbox-attachments clearfix"></HeaderTemplate>
                            <FooterTemplate></ul></FooterTemplate>
                            <ItemTemplate>
                                <li>
                                    <span class="mailbox-attachment-icon" runat="server" visible='<%#IIf(IsImage(Eval("FilePath")), False, True) %>'><i class="fa fa-<%#GetIconClass(Eval("FilePath")) %>"></i></span>
                                    <span class="mailbox-attachment-icon has-img" runat="server" visible='<%#IIf(IsImage(Eval("FilePath")), True, False) %>'>
                                        <img src='<%#Helper.FormImageUrl(Eval("FilePath"), 198, 132, 198, 132, CroppingTypes.Center, 0, 0, "1", "cms") %>' alt="" />
                                    </span>
                                    <div class="mailbox-attachment-info">
                                        <a href="/download-attachment/<%#Eval("Id") %>" class="mailbox-attachment-name">
                                            <i class="fa fa-paperclip"></i> <%#Eval("FileName") %>
                                        </a>
                                        <span class="mailbox-attachment-size">
                                            <%#Helper.FormatFileSize(Eval("FileSize")) %>
                                            <a href="/download-attachment/<%#Eval("Id") %>" class="btn btn-default btn-xs pull-right"><i class="fa fa-cloud-download"></i></a>
                                        </span>
                                    </div>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                        <div class="mailbox-read-message">
                            <asp:Literal ID="litHTML" runat="server" />
                        </div>
                        <div class="text-center">
                            <div class="btn-group">
                                <egvc:EGVLinkButton ID="lnkDelete2" runat="server" Color="Default" Size="Small" OnClientClick="return confirm('Are you sure you want to delete selected messages?');" OnClick="btnDelete_Click"><i class="fa fa-trash-o"></i></egvc:EGVLinkButton>
                                <egvc:EGVLinkButton ID="lnkReply2" runat="server" Color="Default" Size="Small" OnClick="btnReply_Click"><i class="fa fa-reply"></i></egvc:EGVLinkButton>
                                <egvc:EGVLinkButton ID="lnkReplyAll2" runat="server" Color="Default" Size="Small" OnClick="btnReplyAll_Click"><i class="fa fa-reply-all"></i></egvc:EGVLinkButton>
                                <egvc:EGVLinkButton ID="lnkForward2" runat="server" Color="Default" Size="Small" OnClick="btnForward_Click"><i class="fa fa-share"></i></egvc:EGVLinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <egvc:Modal ID="egvModal" runat="server" CssClass="details-modal" Size="Large" Title="Resources.Local.ModalTitle">
        <Body>
            <div class="table-responsive">
                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th><%=Localization.GetResource("Resources.Local.Receiver") %></th>
                            <th><%=Localization.GetResource("Resources.Local.ReadDate") %></th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptDetails" runat="server" OnItemDataBound="rptDetails_ItemDataBound">
                            <ItemTemplate>
                                <tr>
                                    <td><asp:Literal ID="litUserName" runat="server" /></td>
                                    <td><asp:Literal ID="litReadDate" runat="server" /></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </Body>
        <Footer>
            <a href="#" class="btn btn-block btn-default" data-dismiss="modal"><%=Resources.CMS.Close %></a>
        </Footer>
    </egvc:Modal>
</asp:Content>
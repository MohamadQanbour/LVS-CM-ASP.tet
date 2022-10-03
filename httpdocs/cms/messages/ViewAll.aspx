<%@ Page Title="" Language="VB" MasterPageFile="~/cms/CMSMaster.master" AutoEventWireup="false" CodeFile="ViewAll.aspx.vb" Inherits="cms_messages_ViewAll" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<%@ Import Namespace="EGV.Enums" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="box box-primary">
        <div class="box-header with-border">
            <h3 class="box-title"><%=Localization.GetResource("Resources.Local.Title") %></h3>
            <div class="box-tools pull-right">
                <asp:Panel runat="server" DefaultButton="lnkSearch" CssClass="input-group input-group-sm pull-right" style="width: 150px;">
                    <egvc:EGVTextBox ID="txtSearch" runat="server" ControlSize="Small" Placeholder="Resources.Local.SearchMessages" />
                    <div class="input-group-btn">
                        <egvc:EGVLinkButton ID="lnkSearch" runat="server" Color="Default">
                            <span class="glyphicon glyphicon-search"></span>
                        </egvc:EGVLinkButton>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <div class="box-body no-padding">
            <div class="mailbox-controls">
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
                    <thead>
                        <tr>
                            <th><%=Localization.GetResource("Resources.Local.Message") %></th>
                            <th style="width: 25%;"><%=Localization.GetResource("Resources.Local.Sender") %></th>
                            <th style="width: 20%"><%=Localization.GetResource("Resources.Local.Date") %></th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptMessages" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="mailbox-name" style="padding-left: 20px;"><a href='<%#Path.MapCMSFile("messages/read_admin.aspx?id=" & Eval("Id").ToString()) %>'><%#Eval("Title")%></a></td>
                                    <td class="mailbox-subject"><%#Eval("SenderName") %></td>
                                    <td class="mailbox-date"><%#CDate(Eval("MessageDate")).ToString("MMMM d, yyyy h:mm:ss tt") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="box-footer no-padding">
            <div class="mailbox-controls">
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
</asp:Content>
<%@ Page Title="" Language="VB" MasterPageFile="~/cms/CMSMaster.master" AutoEventWireup="false" CodeFile="payments2-editor.aspx.vb" Inherits="cms_membership_payments2_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.Student">
                <Content>
                    <div id="pnlStudentIG" runat="server" class="input-group">
                        <egvc:EGVTextBox ID="txtStudent" runat="server" ReadOnly="true" />
                        <span id="pnlStudentIGB" runat="server" class="input-group-btn">
                            <egvc:EGVHyperLink ID="hypStudent" runat="server" Target="_blank" Color="Warning"><span class="fa fa-external-link"></span></egvc:EGVHyperLink>
                        </span>
                    </div>
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.Class">
                <Content>
                    <div id="pnlClassIG" runat="server" class="input-group">
                        <egvc:EGVTextBox ID="txtClass" runat="server" ReadOnly="true" />
                        <span class="input-group-btn" id="pnlClassIGB" runat="server">
                            <egvc:EGVHyperLink ID="hypClass" runat="server" Target="_blank" Color="Warning"><span class="fa fa-external-link"></span></egvc:EGVHyperLink>
                        </span>
                    </div>
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.Requested">
                <Content>
                    <egvc:EGVTextBox ID="txtRequested" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.Balance">
                <Content>
                    <egvc:EGVTextBox ID="txtBalance" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.LastUpdate">
                <Content>
                    <egvc:EGVTextBox ID="txtLastUpdate" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.LastUpdateUser">
                <Content>
                    <egvc:EGVTextBox ID="txtLastUpdateUser" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.History">
                <Content>
                    <egvc:EGVLinkButton ID="lnkDeleteAll" runat="server" BlockButton="true" Color="Maroon" Text="Resources.Local.lnkDeleteAll" OnClientClick="return confirm('Are you sure you want to delete all?!')" />
                    <div class="table-responsive">
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th></th>
                                    <th><%=Localization.GetResource("Resources.Local.Date") %></th>
                                    <th><%=Localization.GetResource("Resources.Local.Requested") %></th>
                                    <th><%=Localization.GetResource("Resources.Local.Balance") %></th>
                                    <th><%=Localization.GetResource("Resources.Local.Paid") %></th>
                                    <th><%=Localization.GetResource("Resources.Local.Payment") %></th>
                                    <th><%=Localization.GetResource("Resources.Local.AddedBy") %></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptHistory" runat="server" OnItemDataBound="rptHistory_ItemDataBound">
                                    <ItemTemplate>
                                        <tr>
                                            <td><egvc:EGVLinkButton ID="btnDelete" runat="server" OnClick="Unnamed_Click" data-id='<%#Eval("Id") %>' Color="Danger"><span class="fa fa-times"></span></egvc:EGVLinkButton></td>
                                            <td><%#CDate(Eval("LastUpdate")).ToString("MMMM d, yyyy") %></td>
                                            <td><%#CDec(Eval("RequestedAmount")).ToString("N0") %></td>
                                            <td><%#CDec(Eval("Balance")).ToString("N0") %></td>
                                            <td><%#CDec(Eval("PaidAmount")).ToString("N0") %></td>
                                            <td><%#CDec(Eval("Payment")).ToString("N0") %></td>
                                            <td><%#Eval("FullName") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" />
        </FormFooter>
    </egvc:EGVInputForm>
</asp:Content>
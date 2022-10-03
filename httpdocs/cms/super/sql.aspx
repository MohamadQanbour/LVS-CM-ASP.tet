<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="sql.aspx.vb" Inherits="cms_super_sql" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvResult" runat="server" Visible="false" Title="Resources.Local.egvBox.Title">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowResult.Title">
                <Content>
                    <egvc:EGVTextBox ID="txtResult" runat="server" ReadOnly="true" />
                </Content>
            </egvc:RowItem>
        </Rows>
    </egvc:EGVInputForm>
    <egvc:Box ID="egvBox" runat="server" Title="Resources.Local.egvBox.Title" Type="Success">
        <BoxBody>
            <asp:GridView ID="egvGrid" runat="server" CssClass="egvGrid table table-hover" AutoGenerateColumns="true">
                <HeaderStyle CssClass="bg-navy" />
            </asp:GridView>
        </BoxBody>
    </egvc:Box>
    <egvc:EGVInputForm runat="server" IconClass="fa fa-terminal" Title="Resources.Local.egvIF.Title" Type="Danger" DefaultReturnButton="lnkExecute">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.rowType.Title">
                <Content>
                    <div class="icheck">
                        <asp:RadioButtonList ID="rblType" runat="server" RepeatColumns="3" 
                            RepeatDirection="Horizontal" RepeatLayout="Table" CssClass="rbl">
                            <asp:ListItem Selected="True" Value="1" Text="Non Query" />
                            <asp:ListItem Value="2" Text="Data Table" />
                            <asp:ListItem Value="3" Text="Scalar" />
                            <asp:ListItem Value="4" Text="Insert" />
                            <asp:ListItem Value="5" Text="Update" />
                        </asp:RadioButtonList>
                    </div>
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.rowQuery.Title">
                <Content>
                    <egvc:EGVTextBox ID="txtQuery" runat="server" TextMode="MultiLine" Rows="10" />
                    <egvc:EGVRequired ID="reqQuery" runat="server" ControlToValidate="txtQuery"
                        ValidationGroup="valQuery" ErrorMessage="Resources.Local.reqQuery.ErrorMessage" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormTools>
            <egvc:EGVLinkButton ID="lnkResetDB" runat="server" Color="Danger" Text="Resources.Local.lnkResetDB.Text"
                OnClientClick="return confirm('Are you sure you want to reset the Database to its initial state?');"
                Size="XSmall" />
        </FormTools>
        <FormFooter>
            <egvc:EGVHyperLink ID="hypCancel" runat="server" NavigateUrl="Default.aspx" Color="Default" Text="Resources.Local.hypCancel.Text" />
            <egvc:EGVLinkButton ID="lnkExecute" runat="server" Color="Primary" Text="Resources.Local.lnkExecute.Text" CssClass="pull-right" ValidationGroup="valQuery" />
        </FormFooter>
    </egvc:EGVInputForm>
</asp:Content>
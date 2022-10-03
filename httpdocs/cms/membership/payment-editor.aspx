<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="payment-editor.aspx.vb" Inherits="cms_membership_payment_editor" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <egvc:EGVInputForm ID="egvIF" runat="server">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.RowStudent.Title">
                <Content>
                    <egvc:EGVDropDown ID="ddlStudent" runat="server" ClientIDMode="Static" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/Students" />
                    <egvc:EGVRequired ID="reqStudent" runat="server" ControlToValidate="ddlStudent"
                        ValidationGroup="valSave" ErrorMessage="Resources.Local.reqStudent.ErrorMessage" />
                    <egvc:EGVTextBox ID="txtStudent" runat="server" ReadOnly="true" />
                    <asp:HiddenField ID="hdnStudent" runat="server" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.RowPreviousClass.Title">
                <Content>
                    <egvc:EGVDropDown ID="ddlPreviousClass" runat="server" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/Classes" AllowNull="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.RowCurrentClass.Title">
                <Content>
                    <egvc:EGVDropDown ID="ddlCurrentClass" runat="server" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/Classes" AllowNull="false" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.RowTransportation.Title">
                <Content>
                    <div class="icheck">
                        <egvc:EGVCheckBox ID="chkTransportation" runat="server" />
                    </div>
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.TransportationValue">
                <Content>
                    <egvc:EGVTextBox ID="txtTransportationValue" runat="server" InputDecimal="true" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.RowDeposit.Title">
                <Content>
                    <egvc:EGVTextBox ID="txtDeposit" runat="server" InputDecimal="true" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.RowSubscription.Title">
                <Content>
                    <egvc:EGVTextBox ID="txtSubscription" runat="server" InputDecimal="true" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.RowTotal.Title">
                <Content>
                    <egvc:EGVTextBox ID="txtTotal" runat="server" InputDecimal="true" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.RowDiscount.Title">
                <Content>
                    <egvc:EGVTextBox ID="txtDiscount" runat="server" InputDecimal="true" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.RowNetTotal.Title">
                <Content>
                    <egvc:EGVTextBox ID="txtNetTotal" runat="server" ReadOnly="true" InputDecimal="true" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.RowPaymentsSum.Title">
                <Content>
                    <egvc:EGVTextBox ID="txtPaymentsSum" runat="server" ReadOnly="true" InputDecimal="true" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
            <egvc:RowItem runat="server" Title="Resources.Local.RowBalance.Title">
                <Content>
                    <egvc:EGVTextBox ID="txtBalance" runat="server" ReadOnly="true" InputDecimal="true" ClientIDMode="Static" />
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:SaveCancel ID="egvSaveCancel" runat="server" ValidationGroup="valSave" />
        </FormFooter>
    </egvc:EGVInputForm>
    <egvc:Box ID="egvBox" runat="server" Type="Primary" Title="Resources.Local.Payments" IconClass="fa fa-money">
        <BoxTools>
            <egvc:EGVHyperLink ID="hypAdd" runat="server" Color="Success" Size="Small" CommandName="Add"><i class="fa fa-plus"></i> <%=Localization.GetResource("Resources.Local.AddPayment")%></egvc:EGVHyperLink>
        </BoxTools>
        <BoxBody>
            <asp:HiddenField ID="hdnStudentId" runat="server" ClientIDMode="Static" />
            <table class="table table-hover egvGrid">
                <tr class="bg-navy">
                    <th><%=Localization.GetResource("Resources.Local.PaymentNumber") %></th>
                    <th><%=Localization.GetResource("Resources.Local.PaymentAmount") %></th>
                    <th><%=Localization.GetResource("Resources.Local.PaymentDate") %></th>
                    <th><%=Localization.GetResource("Resources.Local.PaymentNote") %></th>
                    <th style="width: 75px;"><%=Localization.GetResource("Resources.Local.Edit") %></th>
                    <th style="width: 75px;"><%=Localization.GetResource("Resources.Local.Delete") %></th>
                </tr>
                <asp:Repeater ID="rptPayments" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td class="bg-olive text-center"><%#Eval("PaymentNumber") %></td>
                            <td><%#CDec(Eval("PaymentAmount")).ToString("0,0") %></td>
                            <td><%#CDate(Eval("PaymentDate")).ToString("MMMM dd, yyyy") %></td>
                            <td><%#Eval("PaymentNote") %></td>
                            <td class="text-center"><a href="#" data-edit='<%#Eval("Id") %>' class="btn btn-default btn-sm"><i class="fa fa-edit"></i></a></td>
                            <td class="text-center"><egvc:EGVLinkButton ID="btnDelete" runat="server" CommandArgument='<%#Eval("Id") %>' Color="Danger" Size="Small" OnClick="btnDelete_Click" OnClientClick="return confirm('Are you sure you want to delete this payment?')"><i class="fa fa-times"></i></egvc:EGVLinkButton></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <egvc:Modal ID="egvModal" runat="server" Title="Resources.Local.ModalPayment" Size="Large" ClientIDMode="Static">
                <Body>
                    <asp:HiddenField ID="hdnId" runat="server" />
                    <egvc:EGVInputForm ID="egvEditPayment" runat="server" NoBorders="true">
                        <Rows>
                            <egvc:RowItem runat="server" Title="Resources.Local.PaymentNumber">
                                <Content>
                                    <egvc:EGVTextBox ID="txtPaymentNumber" runat="server" InputNumber="true" />
                                    <egvc:EGVRequired ID="reqPaymentNumber" runat="server" ControlToValidate="txtPaymentNumber"
                                        ValidationGroup="valPayment" ErrorMessage="Resources.Local.reqPaymentNumber" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.PaymentAmount">
                                <Content>
                                    <egvc:EGVTextBox ID="txtPaymentAmount" runat="server" InputDecimal="true" />
                                    <egvc:EGVRequired ID="reqPaymentAmount" runat="server" ControlToValidate="txtPaymentAmount"
                                        ValidationGroup="valPayment" ErrorMessage="Resources.Local.reqPaymentAmount" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.PaymentDate">
                                <Content>
                                    <egvc:EGVTextBox ID="txtPaymentDate" runat="server" DatePicker="true" />
                                    <egvc:EGVRequired ID="reqPaymentDate" runat="server" ControlToValidate="txtPaymentdate"
                                        ValidationGroup="valPayment" ErrorMessage="Resources.Local.reqPaymentDate" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.PaymentNote">
                                <Content>
                                    <egvc:EGVTextBox ID="txtPaymentNote" runat="server" />
                                </Content>
                            </egvc:RowItem>
                        </Rows>
                    </egvc:EGVInputForm>
                </Body>
                <Footer>
                    <egvc:EGVLinkButton id="btnSavePayment" runat="server" Color="Primary" FlatButton="true" Text="Resources.Local.Save" ValidationGroup="valPayment" />
                </Footer>
            </egvc:Modal>
        </BoxBody>
    </egvc:Box>
</asp:Content>
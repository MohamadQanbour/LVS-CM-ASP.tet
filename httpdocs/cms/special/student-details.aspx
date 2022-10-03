<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="student-details.aspx.vb" Inherits="cms_special_student_details" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<%@ Import Namespace="EGV.Enums" %>
<%@ Import Namespace="EGV.Business" %>
<asp:Content ID="content1" ContentPlaceHolderID="header" runat="server">
    <style>
        .student-details-notes { margin: 40px 0px; }
        .student-details-notes > .row, .internal-notes > .row { background-color: #e0e0e0; margin-bottom: 5px; }
        .internal-notes .note-header { padding-top: 10px; padding-bottom: 10px; background-color: #290a57; color: #fff; }
        .student-details-notes > .row.row-alt-bg, .internal-notes > .row.row-alt-bg { background-color: #b0b0b0; }
        .student-details-notes .bg-negative { background-color: #a94442; color: #fff; }
        .student-details-notes .bg-positive { background-color: #3c763d; color: #fff; }
        .student-details-notes .bg-negative, .student-details-notes .bg-positive { font-weight: bold; padding-top: 20px; padding-bottom: 20px; }
        .student-details-notes .note-text, .internal-notes .note-text { padding-top: 20px; padding-bottom: 20px; }
        .student-details-notes .note-date { padding-bottom: 5px; margin-bottom: 5px; border-bottom: 2px solid #fff; }
        .marginTop10 { margin-top: 10px; }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hdnUserId" runat="server" ClientIDMode="Static" />
    <egvc:EGVInputForm runat="server" Title="Resources.Local.SelectStudentTitle">
        <Rows>
            <egvc:RowItem runat="server" Title="Resources.Local.Student">
                <Content>
                    <egvc:EGVDropDown ID="ddlStudent" runat="server" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/Students" data-type="student" />
                    <asp:HiddenField ID="hdnStudent" runat="server" ClientIDMode="Static" Value="0" />
                    <span class="err" id="pnlError"><%=Localization.GetResource("Resources.Local.reqStudent") %></span>
                </Content>
            </egvc:RowItem>
        </Rows>
        <FormFooter>
            <egvc:EGVLinkButton ID="lnkLoad" runat="server" Color="Primary" BlockButton="true" ClientIDMode="Static" Text="Resources.Local.lnkLoad" data-too="load" NavigateUrl="#"/>
        </FormFooter>
    </egvc:EGVInputForm>
    <div id="pnlResults" runat="server" visible="false">
        <h4><asp:Literal ID="litStudentName" runat="server" /></h4>
        <egvc:EGVTabs ID="egvTabs" runat="server" IconClass="fa fa-graduation-cap"  Title="Resources.Local.egvTabs" >
            <Tabs>
                <egvc:EGVTabItem Id="tabProfile" Title="Resources.Local.tabProfile" />
                <egvc:EGVTabItem Id="tabNotes" Title="Resources.Local.tabNotes" />
                <egvc:EGVTabItem Id="tabPayments" Title="Resources.Local.tabPayments" />
                <egvc:EGVTabItem Id="tabInternalNotes" Title="Resources.Local.tabInternalNotes" />
                <egvc:EGVTabItem Id="tabsort" Title="Resources.Local.tabSort"/>
            </Tabs>
            <TabContents>
                <egvc:EGVTabContent runat="server" >                
                    <egvc:EGVInputForm runat="server" Title="Resources.Local.tabProfile" NoBorders="true" ID="ifProfile" >
                        <Rows>
                            <egvc:RowItem runat="server" Title="Resources.Local.Student">
                                <Content>
                                    <egvc:EGVTextBox ID="txtStudent" runat="server" ReadOnly="true" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.Class">
                                <Content>
                                    <egvc:EGVTextBox ID="txtClass" runat="server" ReadOnly="true" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.FatherName">
                                <Content>
                                    <egvc:EGVTextBox ID="txtFatherName" runat="server" ReadOnly="true" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.MotherName">
                                <Content>
                                    <egvc:EGVTextBox ID="txtMotherName" runat="server" ReadOnly="true" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.FatherWork">
                                <Content>
                                    <egvc:EGVTextBox ID="txtFatherWork" runat="server" ReadOnly="true" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.MotherWork">
                                <Content>
                                    <egvc:EGVTextBox ID="txtMotherWork" runat="server" ReadOnly="true" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.FullProfile" ID="rowFullProfile">
                                <Content>
                                    <egvc:EGVHyperLink ID="hypStudentProfile" runat="server" Color="Olive" Text="Resources.Local.hypStudentProfile" Target="_blank" /> <egvc:EGVHyperLink ID="hypFamilyProfile" runat="server" Color="Olive" Text="Resources.Local.hypFamilyProfile" Target="_blank" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.Siblings">
                                <Content>
                                    <asp:Repeater ID="rptSiblings" runat="server">
                                        <ItemTemplate>
                                            <a href='<%#Path.MapCMSFile("membership/users-editor.aspx?id=" & Eval("Id")) %>' target="_blank" class="btn btn-primary"><%#Eval("FullName") %></a>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </Content>
                            </egvc:RowItem>
                        </Rows>
                    </egvc:EGVInputForm>
                </egvc:EGVTabContent>
                <egvc:EGVTabContent runat="server">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="col-md-12 student-details-notes">
                                <h4><%=Localization.GetResource("Resources.Local.tabNotes") %></h4>
                                <asp:Repeater ID="rptNotes" runat="server">
                                    <ItemTemplate>
                                        <div class="col-md-12">
                                            <div class="row<%#IIf(Container.ItemIndex Mod 2 = 1, " row-alt-bg", "") %>">
                                                <div class="col-sm-3 bg-<%#IIf(Eval("NoteType") = NoteTypes.Negative, "negative", "positive")  %>">
                                                    <div class="note-date"><%#CDate(Eval("NoteDate")).ToString("MMMM dd, yyyy") %></div>
                                                    <%#New User(Eval("SenderId"), MyConn).Profile.FullName %>
                                                </div>
                                                <div class="col-sm-9 note-text">
                                                    <%#Eval("NoteText") %>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>                    
                </egvc:EGVTabContent>
                <egvc:EGVTabContent runat="server">
                    <egvc:EGVInputForm runat="server" Title="Resources.Local.tabPayments" NoBorders="true" ID="ifAccount">
                        <Rows>
                            <egvc:RowItem runat="server" Title="Resources.Local.rowBalance">
                                <Content>
                                    <egvc:EGVTextBox ID="txtBalance" runat="server" ReadOnly="true" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.rowPaymentSum">
                                <Content>
                                    <egvc:EGVTextBox ID="txtPayments" runat="server" ReadOnly="true" />
                                </Content>
                            </egvc:RowItem>
                        </Rows>
                    </egvc:EGVInputForm>
                    <div class="row marginTop10">
                        <div class="col-md-12">
                            <div class="col-md-12 table-responsive">
                                <asp:Repeater ID="rptPayments" runat="server">
                                    <HeaderTemplate>
                                        <table class="table table-hover egvGrid">
                                            <tr class="bg-navy">
                                                <th><%=Localization.GetResource("Resources.Local.PaymentNumber") %></th>
                                                <th><%=Localization.GetResource("Resources.Local.PaymentAmount") %></th>
                                                <th><%=Localization.GetResource("Resources.Local.PaymentDate") %></th>
                                                <th><%=Localization.GetResource("Resources.Local.PaymentNote") %></th>
                                            </tr>
                                    </HeaderTemplate>
                                    <FooterTemplate></table></FooterTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td><%#Eval("PaymentNumber") %></td>
                                            <td><%#CDec(Eval("PaymentAmount")).ToString("0,0") %></td>
                                            <td><%#CDate(Eval("PaymentDate")).ToString("MMMM dd, yyyy") %></td>
                                            <td><%#Eval("PaymentNote") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </egvc:EGVTabContent>
                <egvc:EGVTabContent runat="server">
                    <egvc:EGVInputForm ID="ifInternalNotes" runat="server" Title="Resources.Local.tabInternalNotes" NoBorders="true">
                        <Rows>
                            <egvc:RowItem runat="server" Title="Resources.Local.AddNote">
                                <Content>
                                    <egvc:EGVTextBox ID="txtNote" runat="server" TextMode="MultiLine" data-type="note" />
                                </Content>
                            </egvc:RowItem>
                            <egvc:RowItem runat="server" Title="Resources.Local.NoteDate">
                                <Content>
                                    <egvc:EGVTextBox ID="txtNoteDate" runat="server" DatePicker="true" data-type="note-date" />
                                </Content>
                            </egvc:RowItem>
                        </Rows>
                        <FormFooter>
                            <egvc:EGVHyperLink ID="hypAddNote" runat="server" data-toggle="add-note" Color="Primary" Text="Resources.Local.hypAddNote" CssClass="pull-right" />
                        </FormFooter>
                    </egvc:EGVInputForm>
                    <div class="row marginTop10">
                        <div class="col-md-12">
                            <div class="col-md-12 internal-notes">
                                <asp:Repeater ID="rptInternalNotes" runat="server">
                                    <ItemTemplate>
                                        <div class="row<%#IIf(Container.ItemIndex Mod 2 = 1, " row-alt-bg", "") %>">
                                            <div class="col-md-12">
                                                <div class="row note-header">
                                                    <div class="col-sm-6"><b><%#Eval("FullName") %></b></div>
                                                    <div class="col-sm-6 text-right"><%#CDate(Eval("NoteDate")).ToString("MMMM dd, yyyy") %></div>
                                                </div>
                                                <div class="row marginTop10">
                                                    <div class="col-sm-12 note-text"><%#Eval("Note") %></div>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </egvc:EGVTabContent>
            <egvc:EGVTabContent runat="server">
                <egvc:EGVInputForm ID="ifSort" runat="server" Title="Resources.Local.tabSort" NoBorders="True">
                </egvc:EGVInputForm>
                <div class="row marginTop10  text-right">
                    <div class="col-md-12">
                        <egvc:RowItem ID="DRYSORT" runat="server" Title="Resources.Local.DRYSort" Visible="true">
                            <Content>
                                <egvc:EGVDropDown ID="ddlYear" runat="server" ClientIDMode="Static" Visible="true"/>
                            </Content>
                        </egvc:RowItem>
                        </div>
                    </div>
                <div class="row marginTop10  text-right">
                    <div class="col-md-12">
                        <egvc:RowItem ID="DRTSort" runat="server" Title="Resources.Local.DRTSort" Visible="true" >
                            <Content>
                                <egvc:EGVDropDown ID="ddlTerm" runat="server" ClientIDMode="Static" Visible="true"/>
                                <asp:HiddenField ID="HdnClass" runat="server" ClientIDMode="Static" Value="0" />
                            </Content>
                        </egvc:RowItem>
                    </div>
                </div>
                <div class="row marginTop10">
                        <div class="col-md-12">
                            <div class="col-md-12 table-responsive">
                                <asp:Repeater ID="rptSort" runat="server">
                                    <HeaderTemplate>
                                        <table class="table table-hover egvGrid" border="1">
                                            <tr class="bg-navy">
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentNo") %></th>
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentName")%></th>
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentSum") %></th>
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentAvg") %></th>
                                            </tr>
                                    </HeaderTemplate>
                                    <FooterTemplate></table></FooterTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("NO1") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("Name1")%></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Sum1")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Avg1")).ToString("0.00%") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("NO2") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("Name2")%></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Sum2")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Avg2")).ToString("0.00%")%></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("NO3") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("Name3")%></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Sum3")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Avg3")).ToString("0.00%") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("NO4") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("Name4")%></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Sum4")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Avg4")).ToString("0.00%")%></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:Repeater ID="rptSort1" runat="server">
                                    <HeaderTemplate>
                                        <table class="table table-hover egvGrid" border="1">
                                            <tr class="bg-navy">
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentNo") %></th>
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentName")%></th>
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentSum") %></th>
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentAvg") %></th>
                                            </tr>
                                    </HeaderTemplate>
                                    <FooterTemplate></table></FooterTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("NO1") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("Name1")%></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Sum1")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Avg1")).ToString("0.00%") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("NO2") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("Name2")%></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Sum2")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Avg2")).ToString("0.00%")%></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("NO3") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("Name3")%></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Sum3")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Avg3")).ToString("0.00%") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("NO4") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("Name4")%></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Sum4")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("Avg4")).ToString("0.00%")%></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
              <%--  <div class="row marginTop10">
                        <div class="col-md-12">
                            <div class="col-md-12 table-responsive">
                                <asp:Repeater ID="rptSort1" runat="server">
                                    <HeaderTemplate>
                                        <table class="table table-hover egvGrid" border="1">
                                            <tr class="bg-navy">
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentNo") %></th>
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentName")%></th>
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentSum") %></th>
                                                <th style="text-align:center" colspan="4"><%=Localization.GetResource("Resources.Local.SortStudentAvg") %></th>
                                            </tr>
                                    </HeaderTemplate>
                                    <FooterTemplate></table></FooterTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("PaymentNumber") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("PaymentAmount")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDate(Eval("PaymentDate")).ToString("MMMM dd, yyyy") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("PaymentNote") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("PaymentNumber") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("PaymentAmount")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDate(Eval("PaymentDate")).ToString("MMMM dd, yyyy") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("PaymentNote") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("PaymentNumber") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("PaymentAmount")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDate(Eval("PaymentDate")).ToString("MMMM dd, yyyy") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("PaymentNote") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("PaymentNumber") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDec(Eval("PaymentAmount")).ToString("0,0") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#CDate(Eval("PaymentDate")).ToString("MMMM dd, yyyy") %></td>
                                            <td style="text-align:center ; border:solid ;border-width:thin"><%#Eval("PaymentNote") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>--%>
             </egvc:EGVTabContent>
            </TabContents>
        </egvc:EGVTabs>
    </div>
</asp:Content>
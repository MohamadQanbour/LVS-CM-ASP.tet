<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="student-tests2.aspx.vb" Inherits="cms_special_student_tests2" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<%@ Import Namespace="EGV.Enums" %>
<%@ Import Namespace="EGV.Business" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hdnUserId" runat="server" ClientIDMode="Static" />
    <egvc:EGVTabs ID="egvTabs" runat="server" Title="Resources.Local.TabsTitle" IconClass="fa fa-file-text-o">
        <Tabs>
            <egvc:EGVTabItem Id="tabMaterial" runat="server" Title="Resources.Local.MaterialTabTitle" />
            <egvc:EGVTabItem Id="tabStudent" runat="server" Title="Resources.Local.StudentTabTitle" />
            <egvc:EGVTabItem Id="tabPublish" runat="server" Title="Resources.Local.PublishTabTitle" />
        </Tabs>
        <TabContents>
            <egvc:EGVTabContent runat="server">
                <asp:HiddenField ID="hdnSelectedSection" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdnSelectedMaterial" runat="server" ClientIDMode="Static" />
                <egvc:EGVInputForm ID="ifMaterial" runat="server" NoBorders="true">
                    <Rows>
                        <egvc:RowItem runat="server" Title="Resources.Local.Class">
                            <Content>
                                <egvc:EGVDropDown ID="ddlClass" runat="server" ClientIDMode="Static" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherClasses" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.Section">
                            <Content>
                                <egvc:EGVDropDown ID="ddlSection" runat="server" ClientIDMode="Static" AutoComplete="false" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherSections" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.Material">
                            <Content>
                                <egvc:EGVDropDown ID="ddlMaterial" runat="server" ClientIDMode="Static" AutoComplete="false" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherMaterials" />
                            </Content>
                        </egvc:RowItem>
                    </Rows>
                    <FormFooter>
                        <egvc:EGVLinkButton ID="lnkLoad1" runat="server" Color="Warning" ClientIDMode="Static" Text="Resources.Local.Load" CssClass="pull-right" />
                    </FormFooter>
                </egvc:EGVInputForm>
                <egvc:Box ID="egvMaterialBox" runat="server" ClientIDMode="Static" Type="Success" DefaultReturnButton="lnkSaveMaterial">
                    <BoxBody>
                        <div class="row">
                            <div class="col-md-12 table-responsive">
                                <asp:HiddenField ID="hdnMaterialMaxMark" runat="server" ClientIDMode="Static" />
                                <div class="help-block"><%=Localization.GetResource("Resources.Local.MaxMark") %> <b><asp:Literal ID="litMaxMark" runat="server" /></b></div>
                                <table class="table table-hover egvGrid">
                                    <tr class="bg-navy">
                                        <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.Student") %></th>
                                        <th style="width: 53%;"><%=Localization.GetResource("Resources.Local.Exam") %></th>
                                        <th style="width: 70px;"><%=Localization.GetResource("Resources.Local.MaxMark") %></th>
                                        <th><%=Localization.GetResource("Resources.Local.User") %></th>
                                    </tr>
                                    <asp:Repeater ID="rptMaterialStudents" runat="server">
                                        <ItemTemplate>
                                            <tr data-student='<%#Eval("Id") %>'>
                                                <td class="bg-olive" style="vertical-align: top !important;">
                                                    <%#Eval("IdName") %>
                                                    <asp:HiddenField ID="hdnStudent" runat="server" Value='<%#Eval("Id") %>' />
                                                </td>
                                                <td colspan="3">
                                                    <asp:Repeater ID="rptExams" runat="server">
                                                        <ItemTemplate>
                                                            <div data-exam='<%#Eval("Id") %>' class="row">
                                                                <div class="col-md-8">
                                                                    <div><span class="control-label"><%#Eval("Title") %></span></div>
                                                                    <div>
                                                                        <input type="hidden" ID="hdn" data-type="itemrelated" value='<%#String.Join(",", ExamTemplateItemController.GetRelatedIds(Eval("Id"), MyConn)) %>' />
                                                                        <asp:HiddenField ID="hdnValue" runat="server" Value="0" />
                                                                        <egvc:EGVTextBox ID="txt" runat="server" InputDecimal="true" data-type='<%#Eval("Type") %>' data-key='<%#Eval("Id") %>' ReadOnly='<%#IIf(Eval("Type") = ExamItemTypes.Number, False, True) %>' value="0" />
                                                                    </div>
                                                                </div>
                                                                <div class="col-md-1">
                                                                    <br />
                                                                    <div class="help-block text-small"><div data-type="maxvalue"><b data-type="maxmark"><%#GetMaxMark1(MyConn, Eval("Id")) %></b></div></div>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <br />
                                                                    <div class="help-block text-small" data-type="user">&nbsp;</div>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </table>
                            </div>
                        </div>
                    </BoxBody>
                    <BoxFooter>
                        <asp:HiddenField ID="hdnResultsMaterial" runat="server" ClientIDMode="Static" Value="[]" />
                        <egvc:EGVLinkButton ID="lnkSaveMaterial" runat="server" Color="Primary" ClientIDMode="Static" Text="Resources.Local.Save" CssClass="pull-right" />
                    </BoxFooter>
                </egvc:Box>
            </egvc:EGVTabContent>
            <egvc:EGVTabContent runat="server">
                <egvc:EGVInputForm ID="ifStudent" runat="server" NoBorders="true">
                    <Rows>
                        <egvc:RowItem runat="server" Title="Resources.Local.Student">
                            <Content>
                                <egvc:EGVDropDown ID="ddlStudent" runat="server" ClientIDMode="Static" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherStudents" />
                            </Content>
                        </egvc:RowItem>
                    </Rows>
                    <FormFooter>
                        <egvc:EGVLinkButton ID="lnkLoad" runat="server" Color="Warning" ClientIDMode="Static" Text="Resources.Local.Load" CssClass="pull-right" />
                    </FormFooter>
                </egvc:EGVInputForm>
                <egvc:Box ID="egvStudentBox" runat="server" ClientIDMode="Static" Type="Success" DefaultReturnButton="lnkSaveStudent">
                    <BoxBody>
                        <asp:HiddenField ID="hdnStudentClassId" runat="server" ClientIDMode="Static" />
                        <asp:HiddenField ID="hdnStudentId" runat="server" />
                        <div class="row">
                            <div class="col-md-12 table-responsive">
                                <table class="table table-hover egvGrid">
                                    <tr class="bg-navy">
                                        <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.Material") %></th>
                                        <th style="width: 53%;"><%=Localization.GetResource("Resources.Local.Exam") %></th>
                                        <th style="width: 70px;"><%=Localization.GetResource("Resources.Local.MaxMark") %></th>
                                        <th><%=Localization.GetResource("Resources.Local.User") %></th>
                                    </tr>
                                    <asp:Repeater ID="rptMaterials" runat="server">
                                        <ItemTemplate>
                                            <tr data-maxmark='<%#Eval("MaxMark") %>' data-materialid='<%#Eval("Id") %>'>
                                                <td class="bg-olive" style="vertical-align: top !important;">
                                                    <%#Eval("MaterialTitle") %><br />
                                                    <small><%#Localization.GetResource("Resources.Local.MaxMark") %>: <b><%#Eval("MaxMark") %></b></small>
                                                    <asp:HiddenField ID="hdnMaterial" runat="server" Value='<%#Eval("Id") %>' />
                                                </td>
                                                <td colspan="3">
                                                    <asp:Repeater ID="rptExams" runat="server">
                                                        <ItemTemplate>
                                                            <div data-exam='<%#Eval("Id") %>' class="row">
                                                                <div class="col-md-8">
                                                                    <div><span class="control-label"><%#Eval("Title") %></span></div>
                                                                    <div>
                                                                        <input type="hidden" ID="hdn" data-type="itemrelated" value='<%#String.Join(",", ExamTemplateItemController.GetRelatedIds(Eval("Id"), MyConn)) %>' />
                                                                        <asp:HiddenField ID="hdnValue" runat="server" Value="0" />
                                                                        <egvc:EGVTextBox ID="txt" runat="server" InputDecimal="true" data-type='<%#Eval("Type") %>' data-key='<%#Eval("Id") %>' ReadOnly='<%#IIf(Eval("Type") = ExamItemTypes.Number, False, True) %>' value="0" />
                                                                    </div>
                                                                </div>
                                                                <div class="col-md-1">
                                                                    <br />
                                                                    <div class="help-block text-small"><div data-type="maxvalue"><b data-type="maxmark"><%#GetMaxMark(MyConn, Container, Eval("Id")) %></b></div></div>
                                                                </div>
                                                                <div class="col-md-3">
                                                                    <br />
                                                                    <div class="help-block text-small" data-type="user">&nbsp;</div>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </table>
                            </div>
                        </div>
                    </BoxBody>
                    <BoxFooter>
                        <asp:HiddenField ID="hdnResults2" runat="server" ClientIDMode="Static" Value="[]" />
                        <egvc:EGVLinkButton ID="lnkSaveStudent" runat="server" Color="Primary" ClientIDMode="Static" Text="Resources.Local.Save" CssClass="pull-right" />
                    </BoxFooter>
                </egvc:Box>
            </egvc:EGVTabContent>
            <egvc:EGVTabContent runat="server">
                <asp:HiddenField ID="hdnSelectedSectionPublish" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdnSelectedMaterialPublish" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdnSelectedExamPublish" runat="server" ClientIDMode="Static" />
                <egvc:EGVInputForm ID="ifPublish" runat="server" NoBorders="true">
                    <Rows>
                        <egvc:RowItem runat="server" Title="Resources.Local.Class">
                            <Content>
                                <egvc:EGVDropDown ID="ddlClassPublish" runat="server" ClientIDMode="Static" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherClasses" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.Section">
                            <Content>
                                <egvc:EGVDropDown ID="ddlSectionPublish" runat="server" ClientIDMode="Static" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.Material">
                            <Content>
                                <egvc:EGVDropDown ID="ddlMaterialPublish" runat="server" ClientIDMode="Static" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.Exam">
                            <Content>
                                <egvc:EGVDropDown ID="ddlExamsPublish" runat="server" ClientIDMode="Static" />
                            </Content>
                        </egvc:RowItem>
                    </Rows>
                    <FormFooter>
                        <egvc:EGVLinkButton ID="lnkPublish" runat="server" Color="Warning" ClientIDMode="Static" Text="Resources.Local.Publish" CssClass="pull-right" />
                    </FormFooter>
                </egvc:EGVInputForm>
            </egvc:EGVTabContent>
        </TabContents>
    </egvc:EGVTabs>
</asp:Content>
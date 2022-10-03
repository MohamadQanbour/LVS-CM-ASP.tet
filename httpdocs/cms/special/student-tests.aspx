<%@ Page Title="" Language="VB" MasterPageFile="~/cms/CMSMaster.master" AutoEventWireup="false" CodeFile="student-tests.aspx.vb" Inherits="cms_special_student_tests" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Enums" %>
<%@ Import Namespace="EGV.Business" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hdnUserId" runat="server" ClientIDMode="Static" />
    
    <egvc:EGVTabs ID="egvTabs" runat="server" Title="Resources.Local.egvTabs.Title" IconClass="fa fa-file-text-o">
        <Tabs>
            <egvc:EGVTabItem Id="tabMaterial" runat="server" Title="Resources.Local.egvTabs.GroupMaterial" />
            <egvc:EGVTabItem Id="tabStudent" runat="server" Title="Resources.Local.egvTabs.GroupStudent" />
        </Tabs>
        <TabContents>
            <egvc:EGVTabContent runat="server">
                <asp:HiddenField ID="hdnSelectedSection" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdnSelectedMaterial" runat="server" ClientIDMode="Static" />
                <egvc:EGVInputForm ID="ifMaterial" runat="server" NoBorders="true">
                    <Rows>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowClass.Title">
                            <Content>
                                <egvc:EGVDropDown ID="ddlClass" runat="server" ClientIDMode="Static" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherClasses" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowSection.Title">
                            <Content>
                                <egvc:EGVDropDown ID="ddlSection" runat="server" ClientIDMode="Static" AutoComplete="false" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherSections" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowMaterial.Title">
                            <Content>
                                <egvc:EGVDropDown ID="ddlMaterial" runat="server" ClientIDMode="Static" AutoComplete="false" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherMaterials" />
                            </Content>
                        </egvc:RowItem>
                    </Rows>
                    <FormFooter>
                        <egvc:EGVLinkButton ID="lnkLoad1" runat="server" Color="Warning" ClientIDMode="Static" Text="Resources.Local.lnkLoad.Text" CssClass="pull-right" />
                    </FormFooter>
                </egvc:EGVInputForm>
                <egvc:Box ID="egvBox1" runat="server" ClientIDMode="Static" Type="Success" DefaultReturnButton="lnkSave1">
                    <BoxBody>
                        <div class="row">
                            <div class="col-md-12 table-responsive">
                                <asp:HiddenField ID="hdnMaterialMaxMark" runat="server" ClientIDMode="Static" />
                                <div class="help-block"><%=Localization.GetResource("Resources.Local.MaxMark") %> <b><asp:Literal ID="litMaxMark" runat="server" /></b></div>
                                <table class="table table-hover egvGrid">
                                    <tr class="bg-navy">
                                        <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.Student") %></th>
                                        <asp:Repeater ID="rptMaterialExams1" runat="server">
                                            <ItemTemplate>
                                                <th><%#Eval("Title") %></th>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tr>
                                    <asp:Repeater ID="rptStudents" runat="server">
                                        <ItemTemplate>
                                            <tr data-student='<%#Eval("Id") %>'>
                                                <td class="bg-olive">
                                                    <%#Eval("IdName") %>
                                                    <asp:HiddenField ID="hdnStudent" runat="server" Value='<%#Eval("Id") %>' />
                                                </td>
                                                <asp:Repeater ID="rptExams" runat="server">
                                                    <ItemTemplate>
                                                        <td data-exam='<%#Eval("Id") %>'>
                                                            <div class="help-block text-small"><div data-type="maxvalue" style="display: none;"><%=Localization.GetResource("Resources.Local.MaxMark") %>&nbsp;<b data-type="maxmark"></b></div></div>
                                                            <input type="hidden" ID="hdn" data-type="itemrelated" value='<%#String.Join(",", ExamTemplateItemController.GetRelatedIds(Eval("Id"), MyConn)) %>' />
                                                            <egvc:EGVTextBox ID="txt" runat="server" InputDecimal="true" data-type='<%#Eval("Type") %>' data-key='<%#Eval("Id") %>' ReadOnly='<%#IIf(Eval("Type") = ExamItemTypes.Number, False, True) %>' />
                                                            <div class="help-block text-small" data-type="user">&nbsp;</div>
                                                            <asp:HiddenField ID="hdnValue" runat="server" />
                                                        </td>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </table>
                            </div>
                        </div>
                    </BoxBody>
                    <BoxFooter>
                        <asp:HiddenField ID="hdnResults1" runat="server" ClientIDMode="Static" Value="[]" />
                        <egvc:EGVLinkButton ID="lnkSave1" runat="server" Color="Primary" ClientIDMode="Static" Text="Resources.Local.lnkSave.Text" CssClass="pull-right" />
                    </BoxFooter>
                </egvc:Box>
            </egvc:EGVTabContent>
            <egvc:EGVTabContent runat="server">
                <egvc:EGVInputForm ID="ifStudent" runat="server" NoBorders="true">
                    <Rows>
                        <egvc:RowItem runat="server" Title="Resources.Local.rowStudent.Title">
                            <Content>
                                <egvc:EGVDropDown ID="ddlStudent" runat="server" ClientIDMode="Static" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherStudents" />
                            </Content>
                        </egvc:RowItem>
                    </Rows>
                    <FormFooter>
                        <egvc:EGVLinkButton ID="lnkLoad" runat="server" Color="Warning" ClientIDMode="Static" Text="Resources.Local.lnkLoad.Text" CssClass="pull-right" />
                    </FormFooter>
                </egvc:EGVInputForm>
                <egvc:Box ID="egvBox2" runat="server" ClientIDMode="Static" Type="Success" DefaultReturnButton="lnkSave2">
                    <BoxBody>
                        <asp:HiddenField ID="hdnStudentClassId" runat="server" ClientIDMode="Static" />
                        <asp:HiddenField ID="hdnStudentId" runat="server" />
                        <div class="row">
                            <div class="col-md-12">
                                <table class="table table-hover egvGrid">
                                    <tr class="bg-navy">
                                        <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.Material") %></th>
                                        <th><%=Localization.GetResource("Resources.Local.Exams") %></th>
                                    </tr>
                                    <asp:Repeater ID="rptMaterials" runat="server">
                                        <ItemTemplate>
                                            <tr data-maxmark='<%#Eval("MaxMark") %>'>
                                                <td class="bg-olive">
                                                    <%#Eval("MaterialTitle") %>
                                                </td>
                                                <td>
                                                    <asp:HiddenField ID="hdnMaterial" runat="server" Value='<%#Eval("Id") %>' />
                                                    <table style="width: 100%;" data-material='<%#Eval("Id") %>'>
                                                        <tr>
                                                            <asp:Repeater ID="rptExams" runat="server">
                                                                <ItemTemplate>
                                                                    <th><%#Eval("Title") %></th>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tr>
                                                        <tr>
                                                            <asp:Repeater ID="rptExamValues" runat="server">
                                                                <ItemTemplate>
                                                                    <td class="form-group" data-exam='<%#Eval("Id") %>'>
                                                                        <div class="help-block text-small"><div data-type="maxvalue" style="display: none;"><%=Localization.GetResource("Resources.Local.MaxMark") %>&nbsp;<b data-type="maxmark"></b></div></div>
                                                                        <input type="hidden" ID="hdn" data-type="itemrelated" value='<%#String.Join(",", ExamTemplateItemController.GetRelatedIds(Eval("Id"), MyConn)) %>' />
                                                                        <egvc:EGVTextBox ID="txt" runat="server" InputDecimal="true" data-type='<%#Eval("Type") %>' data-key='<%#Eval("Id") %>' ReadOnly='<%#IIf(Eval("Type") = ExamItemTypes.Number, False, True) %>' />
                                                                        <div class="help-block text-small" data-type="user">&nbsp;</div>
                                                                        <asp:HiddenField ID="hdnValue" runat="server" />
                                                                    </td>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </tr>
                                                    </table>
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
                        <egvc:EGVLinkButton ID="lnkSave2" runat="server" Color="Primary" ClientIDMode="Static" Text="Resources.Local.lnkSave.Text" CssClass="pull-right" />
                    </BoxFooter>
                </egvc:Box>
            </egvc:EGVTabContent>
        </TabContents>        
    </egvc:EGVTabs>
</asp:Content>
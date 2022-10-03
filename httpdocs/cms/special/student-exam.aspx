<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="student-exam.aspx.vb" Inherits="cms_special_student_exam" EnableViewState="false" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>
<%@ Import Namespace="EGV.Enums" %>
<%@ Import Namespace="EGV.Business" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hdnUserId" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnSaveSuccess" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnPublishSuccess" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnMaxMarkResource" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnCanUpdate" runat="server" ClientIDMode="Static" />
    <egvc:EGVTabs ID="egvTabs" runat="server" Title="Resources.Local.TabsTitle" IconClass="fa fa-file-text-o">
        <Tabs>
            <egvc:EGVTabItem Id="tabMaterial" runat="server" Title="Resources.Local.MaterialTabTitle" />
            <egvc:EGVTabItem Id="tabStudent" runat="server" Title="Resources.Local.StudentTabTitle" />
            <egvc:EGVTabItem Id="tabPublish" runat="server" Title="Resources.Local.PublishTabTitle" />
        </Tabs>
        <TabContents>
            <egvc:EGVTabContent runat="server">
                <egvc:EGVInputForm ID="ifMaterial" runat="server" NoBorders="true">
                    <Rows>
                        <egvc:RowItem runat="server" Title="Resources.Local.Class">
                            <Content>
                                <egvc:EGVDropDown ID="ddlClass" runat="server" ClientIDMode="Static" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherClasses" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.Section">
                            <Content>
                                <egvc:EGVDropDown ID="ddlSection" runat="server" ClientIDMode="Static" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.Material">
                            <Content>
                                <egvc:EGVDropDown ID="ddlMaterial" runat="server" ClientIDMode="Static" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.TemplateItem">
                            <Content>
                                <egvc:EGVDropDown ID="ddlTemplateItem" runat="server" ClientIDMode="Static" />
                            </Content>
                        </egvc:RowItem>
                    </Rows>
                    <FormFooter>
                        <egvc:EGVHyperLink ID="lnkLoadMaterials" runat="server" Color="Warning" ClientIDMode="Static" Text="Resources.Local.Load" CssClass="pull-right" data-command="load" data-argument="material" NavigateUrl="#" />
                    </FormFooter>
                </egvc:EGVInputForm>
                <div data-command="progress" data-argument="material" style="display: none;">
                    <div class="progress">
                        <div class="progress-bar progress-bar-primary progress-bar-striped active" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>
                    </div>
                </div>
                <egvc:Box ID="egvMaterialBox" runat="server" ClientIDMode="Static" Type="Success" DefaultReturnButton="lnkSaveMaterial" data-command="box" data-argument="material" style="display: none;">
                    <BoxBody>
                        <div class="row">
                            <div class="col-md-12 table-responsive">
                                <div class="help-block"><%=Localization.GetResource("Resources.Local.MaxMark") %> <b data-command="material-max-mark-text"></b></div>
                                <table class="table table-hover egvGrid">
                                    <tr class="bg-navy" data-command="title">
                                        <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.Student") %></th>
                                        <th style="width: 53%;"><%=Localization.GetResource("Resources.Local.Exam") %></th>
                                        <th style="width: 70px;"><%=Localization.GetResource("Resources.Local.MaxMark") %></th>
                                        <th><%=Localization.GetResource("Resources.Local.User") %></th>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </BoxBody>
                    <BoxFooter>
                        <egvc:EGVLinkButton ID="lnkSaveMaterial" runat="server" Color="Primary" ClientIDMode="Static" Text="Resources.Local.Save" CssClass="pull-right" data-command="save" data-argument="material" />
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
                        <egvc:EGVHyperLink NavigateUrl="#" ID="lnkLoad" runat="server" Color="Warning" ClientIDMode="Static" Text="Resources.Local.Load" CssClass="pull-right" data-command="load" data-argument="student" />
                    </FormFooter>
                </egvc:EGVInputForm>
                <div data-command="progress" data-argument="student" style="display: none;">
                    <div class="progress">
                        <div class="progress-bar progress-bar-primary progress-bar-striped active" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>
                    </div>
                </div>
                <egvc:Box ID="egvStudentBox" runat="server" ClientIDMode="Static" Type="Success" DefaultReturnButton="lnkSaveStudent" data-command="box" data-argument="student" style="display: none;">
                    <BoxBody>
                        <div class="row">
                            <div class="col-md-12 table-responsive">
                                <table class="table table-hover egvGrid">
                                    <tr class="bg-navy" data-command="title">
                                        <th style="width: 200px;"><%=Localization.GetResource("Resources.Local.Material") %></th>
                                        <th style="width: 53%;"><%=Localization.GetResource("Resources.Local.Exam") %></th>
                                        <th style="width: 70px;"><%=Localization.GetResource("Resources.Local.MaxMark") %></th>
                                        <th><%=Localization.GetResource("Resources.Local.User") %></th>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </BoxBody>
                    <BoxFooter>
                        <egvc:EGVLinkButton ID="lnkSaveStudent" runat="server" Color="Primary" ClientIDMode="Static" Text="Resources.Local.Save" CssClass="pull-right" data-command="save" data-argument="student" />
                    </BoxFooter>
                </egvc:Box>
            </egvc:EGVTabContent>
            <egvc:EGVTabContent runat="server">
                <egvc:EGVInputForm ID="ifPublish" runat="server" NoBorders="true">
                    <Rows>
                        <egvc:RowItem runat="server" Title="Resources.Local.Class">
                            <Content>
                                <egvc:EGVDropDown ID="ddlClassPublish" runat="server" ClientIDMode="Static" AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherClasses" />
                            </Content>
                        </egvc:RowItem>
                        <egvc:RowItem runat="server" Title="Resources.Local.Section">
                            <Content>
                                <egvc:EGVDropDown ID="ddlSectionPublish" runat="server" ClientIDMode="Static"  AutoComplete="true" AutoCompleteSource="/ajax/CMSAutoComplete/TeacherSections" />
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
                        <egvc:EGVHyperLink NavigateUrl="#" ID="lnkPublish" runat="server" Color="Warning" ClientIDMode="Static" Text="Resources.Local.Publish" CssClass="pull-right" data-command="publish" />
                    </FormFooter>
                </egvc:EGVInputForm>
            </egvc:EGVTabContent>
        </TabContents>
    </egvc:EGVTabs>
</asp:Content>
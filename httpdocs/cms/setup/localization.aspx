<%@ Page Title="" Language="VB" MasterPageFile="../CMSMaster.master" AutoEventWireup="false" CodeFile="localization.aspx.vb" Inherits="cms_setup_localization" %>
<%@ MasterType VirtualPath="../CMSMaster.master" %>
<%@ Import Namespace="EGV.Utils" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hdnFilter" runat="server" ClientIDMode="Static" Value="all" />
    <asp:HiddenField ID="hdnSearch" runat="server" ClientIDMode="Static" Value="" />
    <asp:HiddenField ID="hdnType" runat="server" ClientIDMode="Static" Value="dynamic" />
    <asp:HiddenField ID="hdnTypeId" runat="server" ClientIDMode="Static" Value="" />
    <asp:HiddenField ID="hdnFile" runat="server" ClientIDMode="Static" Value="LOK_Area_Res" />
    <asp:HiddenField ID="hdnUserId" runat="server" ClientIDMode="Static" Value="" />
    <egvc:Box ID="egvBox" runat="server" Title="Resources.Local.egvBox.Title" Type="Primary" IconClass="fa fa-language">
        <BoxTools>
            <div class="input-group input-group-sm pull-right" style="width:150px;">
                <egvc:EGVTextBox ID="txtSearch" runat="server" ClientIDMode="Static" CssClass="pull-right" Placeholder="Resources.Local.txtSearch.Placeholder" />
                <div class="input-group-btn">
                    <a class="btn btn-default" href="#" data-command="search">
                        <span class="fa fa-search"></span>
                    </a>
                </div>
            </div>
            <div class="btn-group pull-right" style="margin-right: 10px;">
                <a href="#" data-command="filter" data-argument="all" class="btn-sm btn btn-success disabled"><%=Localization.GetResource("Resources.Local.btnAll.Text") %></a>
                <a href="#" data-command="filter" data-argument="untranslated" class="btn-sm btn btn-success"><%=Localization.GetResource("Resources.Local.btnNotTranslated.Text") %></a>
            </div>
        </BoxTools>
        <BoxBody>
           <div class="row">
               <div class="col-md-2 unpad5 translations-list">
                   <div class="text-center">
                       <div class="btn-group">
                           <a href="#tabDynamic" data-toggle="tab" data-command="dynamic" class="btn-sm btn btn-warning disabled"><%=Localization.GetResource("Resources.Local.btnDynamic.Text") %></a>
                           <a href="#tabStatic" data-toggle="tab" data-command="static" class="btn-sm btn btn-warning"><%=Localization.GetResource("Resources.Local.btnStatic.Text") %></a>
                       </div>
                   </div>
                   <div class="tab-content">
                       <div id="tabDynamic" class="active tab-pane translation-type">
                           <asp:Repeater ID="rptDynamic" runat="server">
                               <ItemTemplate>
                                   <div class="translation-type-item">
                                       <div class="translation-type-title bg-navy"><%#Eval("Title") %></div>
                                       <ul class="translation-type-items" data-type='<%#Eval("Id") %>'>
                                           <asp:Repeater ID="rptItems" runat="server">
                                               <ItemTemplate>
                                                   <li><egvc:EGVLinkButton ID="lnkItem" runat="server" data-item='<%#Eval("Id") %>' BootstrapButton="false" OnClick="lnkItem_Click"><%#Eval("Title") %></egvc:EGVLinkButton></li>
                                               </ItemTemplate>
                                           </asp:Repeater>
                                       </ul>
                                   </div>
                               </ItemTemplate>
                           </asp:Repeater>
                       </div>
                       <div id="tabStatic" class="tab-pane translation-type">
                           <asp:Repeater ID="rptStatic" runat="server">
                               <ItemTemplate>
                                   <div class="translation-type-item">
                                       <div class="translation-type-title bg-navy"><%#Eval("Title") %></div>
                                       <ul class="translation-type-items">
                                           <asp:Repeater ID="rptItems" runat="server">
                                               <ItemTemplate>
                                                   <li><egvc:EGVLinkButton ID="lnkItem" runat="server" data-item='<%#Eval("FileName") %>' data-directory='<%#Eval("Directory") %>' data-isglobal='<%#Eval("IsGlobal") %>' BootstrapButton="false" OnClick="lnkFile_Click"><%#Eval("Title") %></egvc:EGVLinkButton></li>
                                               </ItemTemplate>
                                           </asp:Repeater>
                                       </ul>
                                   </div>
                               </ItemTemplate>
                           </asp:Repeater>
                       </div>
                   </div>
               </div>
               <div class="col-md-10 translations-container">
                    <div class="table-title">
                        <asp:Literal ID="litType" runat="server" />
                        <span class="fa fa-chevron-right"></span>
                        <asp:Literal ID="litFile" runat="server" />
                    </div>
                   <asp:Panel ID="pnlDynamic" runat="server">
                       <asp:Repeater ID="rptDynamicItems" runat="server">
                           <HeaderTemplate>
                               <table class="table table-hover egvGrid">
                                   <tr class="bg-navy">
                                       <th class="text-center"><%=Localization.GetResource("Resources.Local.Original") %></th>
                                       <th><%=Localization.GetResource("Resources.Local.Translations") %></th>
                                       <th style="width: 50px;"><%=Localization.GetResource("Resources.Local.Save")%></th>
                                   </tr>
                           </HeaderTemplate>
                           <FooterTemplate></table></FooterTemplate>
                           <ItemTemplate>
                               <tr data-original='<%#Eval("Original").ToString().ToLower() %>'>
                                   <td class="bg-olive" rowspan='<%#GetRowSpan(MyConn) %>'><%#Eval("Original") %></td>
                                    <asp:Repeater ID="rptTranslations" runat="server">
                                        <ItemTemplate>
                                            <td data-type="translation" class='<%#IIf(Eval("IsTranslated"), "", "bg-maroon") %>'>
                                                <div class="row">
                                                    <div class="col-md-2"><%#GetLanguageCode(Eval("LanguageId"), MyConn) %></div>
                                                    <div class="col-md-10"><input type="text" id="txt<%#Eval("Id") %>-<%#Eval("LanguageId") %>" class="form-control" value='<%#Eval("Text") %>' /></div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-12 text-right translate-audit" runat="server" visible='<%#IIf(Eval("IsTranslated"), True, False) %>'><%#Localization.GetResource("Resources.Local.TranslatedBy") %>&nbsp;<b><%#Eval("TranslationUser") %></b>&nbsp;<%#Localization.GetResource("Resources.Local.On") %>&nbsp;<b><%#CDate(Eval("TranslationDate")).ToString("MMMM dd, yyyy hh:mm:ss tt") %></b></div>
                                                </div>
                                            </td>
                                            <td>
                                                <a href="#" class="btn btn-primary" data-command="save" data-id='<%#Eval("Id") %>' data-language='<%#Eval("LanguageId") %>'><i class="fa fa-save"></i></a>
                                            </td>
                                        </ItemTemplate>
                                        <SeparatorTemplate></tr><tr data-original='<%#DirectCast(DirectCast(Container.NamingContainer.NamingContainer, RepeaterItem).DataItem, EGV.Structures.DynamicFileItem).Original.ToLower()%>'></SeparatorTemplate>
                                    </asp:Repeater>
                               </tr>
                           </ItemTemplate>
                       </asp:Repeater>
                   </asp:Panel>
                   <asp:Panel ID="pnlStatic" runat="server">
                       <asp:Repeater ID="rptStaticItems" runat="server">
                           <HeaderTemplate>
                               <table class="table table-hover egvGrid">
                                   <tr class="bg-navy">
                                       <th class="text-center"><%=Localization.GetResource("Resources.Local.Original") %></th>
                                       <th><%=Localization.GetResource("Resources.Local.Translations") %></th>
                                       <th style="width: 50px;"><%=Localization.GetResource("Resources.Local.Save")%></th>
                                   </tr>
                           </HeaderTemplate>
                           <FooterTemplate></table></FooterTemplate>
                           <ItemTemplate>
                               <tr data-original='<%#Eval("Key").ToString().ToLower() %>'>
                                   <td class="bg-olive" rowspan='<%#GetRowSpan(MyConn) %>'><%#Eval("Key") %></td>
                                    <asp:Repeater ID="rptTranslations" runat="server">
                                        <ItemTemplate>
                                            <td data-type="translation">
                                                <div class="row">
                                                    <div class="col-md-2"><%#GetLanguageTitleByCode(Eval("LanguageCode"), MyConn) %></div>
                                                    <div class="col-md-10"><input type="text" id="txt<%#Container.ItemIndex%>" class="form-control" value='<%#Eval("Text") %>' /></div>
                                                </div>
                                            </td>
                                            <td>
                                                <a href="#" class="btn btn-primary" data-command="savestatic" data-id='<%#Eval("Key") %>' data-language='<%#Eval("LanguageCode") %>'><i class="fa fa-save"></i></a>
                                            </td>
                                        </ItemTemplate>
                                        <SeparatorTemplate></tr><tr data-original='<%#DirectCast(DirectCast(Container.NamingContainer.NamingContainer, RepeaterItem).DataItem, EGV.Structures.StaticFileItem).Key.ToLower()%>'></SeparatorTemplate>
                                    </asp:Repeater>
                               </tr>
                           </ItemTemplate>
                       </asp:Repeater>
                   </asp:Panel>
               </div>
           </div> 
        </BoxBody>
        <BoxFooter>
            <egvc:EGVHyperLink ID="hypSaveAll" runat="server" data-command="saveall" Color="Primary" FlatButton="true" CssClass="pull-right" Text="Resources.Local.hypSaveAll.Text" />
        </BoxFooter>
    </egvc:Box>
</asp:Content>
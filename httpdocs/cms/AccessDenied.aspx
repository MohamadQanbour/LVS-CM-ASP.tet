<%@ Page Title="" Language="VB" MasterPageFile="CMSMaster.master" AutoEventWireup="false" CodeFile="AccessDenied.aspx.vb" Inherits="cms_AccessDenied" %>
<%@ MasterType VirtualPath="CMSMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="row">
        <div class="col-md-6 col-md-offset-3 text-center text-danger access-denied">
            <span class="glyphicon glyphicon-ban-circle"></span> <%=EGV.Utils.Localization.GetResource("Resources.Local.AccessDenied") %>
        </div>
    </div>
</asp:Content>
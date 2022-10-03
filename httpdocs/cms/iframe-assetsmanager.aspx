<%@ Page Language="VB" AutoEventWireup="false" CodeFile="iframe-assetsmanager.aspx.vb" Inherits="cms_iframe_assetsmanager" %>
<%@ Import Namespace="EGV.Utils" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" dir='<%=Helper.GetHTMLDirection() %>'>
<head runat="server">
    <title></title>
    <asp:Literal ID="litStyle" runat="server" />
    <asp:Literal ID="litAddStyle" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <egvc:AssetsManager ID="egvAssetsManager" runat="server" ShowModal="false" />
        <script type="text/javascript" src='<%=Path.MapCMSScript("jQuery-2.2.0.min") %>'></script>
        <asp:Literal ID="litAddScript" runat="server" />
    </div>
    </form>
</body>
</html>

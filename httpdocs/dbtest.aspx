<%@ Page Language="VB" AutoEventWireup="false" CodeFile="dbtest.aspx.vb" Inherits="dbtest" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:TextBox ID="txt" runat="server" TextMode="MultiLine" Width="500px" Height="500px" />
        <asp:Button ID="btn" runat="server" Text="Execute" />
        <asp:Button ID="btn2" runat="server" Text="Scalar" />
    </div>
    </form>
</body>
</html>

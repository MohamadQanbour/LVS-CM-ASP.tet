<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PostTest.aspx.vb" Inherits="PostTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%;" border="1" id="tbl">
            <tr>
                <th>Type</th>
                <th>Key</th>
                <th>Value</th>
            </tr>
        </table>
    </div>
        <script type="text/javascript" src="/cms/js/jQuery-2.2.0.min.js"></script>
        <script type="text/javascript">
            $(document).ready(function () {
                $.ajax("/ajax/Testing/Test?q=search", {
                    type: "POST",
                    data: { "abc": "testing" },
                    error: function (a, b, c) { alert(c); },
                    success: function (a) {
                        var ret = eval(a)[0];
                        if (ret.HasError) alert(ret.ErrorMessage);
                        else {
                            var data = ret.ReturnData;
                            var target = $('#tbl');
                            $(data).each(function () {
                                var item = $('<tr><td>' + this.Type + '</td><td>' + this.Key + '</td><td>' + this.Value + '</td></tr>');
                                target.append(item);
                            });
                        }
                    }
                });
            });
        </script>
    </form>
</body>
</html>

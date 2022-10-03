<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="cms_Login" %>

<!DOCTYPE html>

<html dir='<%=EGV.Utils.Helper.GetHTMLDirection() %>'>
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" name="viewport" />
    <title><asp:Literal ID="litPageTitle" runat="server" /></title>
    <link rel="icon" href="images/fav.ico" />
    <asp:Literal ID="litStyles" runat="server" />
   <%-- [if lt IE 9]>
      <script src="js/html5shiv.min.js"></script>
      <script src="js/respond.min.js"></script>
    [endif]--%>
</head>
<body class="hold-transition login-page">
    <form id="form1" runat="server">
    <div class="login-box">
        <egvc:Notifier ID="egvNotifier" runat="server" />
        <div class="login-logo">
            <asp:Literal ID="litTitle" runat="server" />
        </div>
        <div class="login-box-body">
            <p class="login-box-msg"><%=GetLocalResourceObject("LoginBox.Message") %></p>
            <div role="form">
                <div class="form-group has-feedback">
                    <egvc:EGVTextBox ID="txtUserName" runat="server" Placeholder="Resources.Local.txtUserName.Placeholder" />
                    <span class="glyphicon glyphicon-user form-control-feedback"></span>
                    <egvc:EGVRequired ID="reqUserName" runat="server" ControlToValidate="txtUserName" ErrorMessage="Resources.Local.reqUserName.ErrorMessage"
                        ValidationGroup="valLogin" />
                </div>
                <div class="form-group has-feedback">
                    <egvc:EGVTextBox ID="txtPassword" runat="server" TextMode="Password" Placeholder="Resources.Local.txtPassword.Placeholder" />
                    <span class="glyphicon glyphicon-lock form-control-feedback"></span>
                    <egvc:EGVRequired ID="reqPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Resources.Local.reqPassword.ErrorMessage"
                        ValidationGroup="valLogin" />
                </div>
                <div class="row">
                    <div class="col-xs-12">
                        <egvc:Notifier ID="egvResult" runat="server" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-8">
                        <div class="checkbox icheck">
                            <egvc:EGVCheckBox ID="chkRemember" runat="server" Text="Resources.Local.chkRemember.Text" />
                        </div>
                    </div>
                    <div class="col-xs-4">
                        <egvc:EGVButton ID="btnLogin" runat="server" BootstrapButton="true" Color="Primary" BlockButton="true" FlatButton="true"
                            Text="Resources.Local.btnLogin.Text" ValidationGroup="valLogin" />
                    </div>
                </div>
            </div>
            <a href="#" data-toggle="modal" data-target="#modalForgetPassword"><%=GetLocalResourceObject("btnForgetPassword.Text") %></a><br>
        </div>
    </div>
    <!--Modals-->
    <div class="modal fade" tabindex="-1" role="dialog" id="modalForgetPassword" aria-labelledby="modalTitle">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="modalTitle"><%=GetLocalResourceObject("modalForgetPassword.Header") %></h4>
                </div>
                <div class="modal-body">
                    <p class="login-box-msg"><%=GetLocalResourceObject("modalForgetPassword.Message") %></p>
                    <div role="form">
                        <div class="form-group has-feedback">
                            <egvc:EGVTextBox ID="txtPREmail" runat="server" Placeholder="Resources.Local.txtEmail.Placeholder" />
                            <span class="glyphicon glyphicon-envelope form-control-feedback"></span>
                            <egvc:EGVRequired ID="reqPREmail" runat="server" ControlToValidate="txtPREmail" 
                                ErrorMessage="Resources.Local.reqEmail.ErrorMessage" ValidationGroup="valPR" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default btn-flat" data-dismiss="modal"><%=GetLocalResourceObject("Close") %></button>
                    <egvc:EGVButton ID="btnSend" runat="server" Text="Resources.Local.btnSend.Text" BootstrapButton="true" 
                        Color="Primary" FlatButton="true" ValidationGroup="valPR" />
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript" src="js/jQuery-2.2.0.min.js"></script>
    <script type="text/javascript" src="js/bootstrap.min.js"></script>
    <script type="text/javascript" src="js/icheck.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $('input').iCheck({
                checkboxClass: 'icheckbox_square-aero',
                radioClass: 'iradio_square-aero',
                increaseArea: '20%'
            });
        });
    </script>
    </form>
</body>
</html>

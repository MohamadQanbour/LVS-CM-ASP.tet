Imports B = EGV.Business.BusinessHelper
Imports U = EGV.Utils
Imports EGV.Business
Imports EGV.Emails

Partial Class cms_Login
    Inherits CMSPageBase

#Region "Public Properties"

    Public ReadOnly Property BackURL As String
        Get
            If Request.QueryString("url") IsNot Nothing AndAlso Request.QueryString("url") <> String.Empty Then Return HttpUtility.UrlDecode(Request.QueryString("url")) Else Return "default.aspx"
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim url As String = String.Empty
            Try
                MyConn.Open()
                If U.Helper.CMSAuthUser IsNot Nothing Then
                    url = BackURL
                Else
                    Dim sitename As String = B.ReadSetting("SITENAME", MyConn)
                    litPageTitle.Text = GetLocalResourceObject("Head.Title") & " | " & sitename
                    Dim parts() As String = U.Helper.SplitString(sitename, " ")
                    parts(0) = "<b>" & parts(0) & "</b>"
                    litTitle.Text = String.Join(" ", parts)
                End If
            Catch ex As Exception
                U.ExceptionHandler.ProcessException(ex, egvNotifier, MyConn)
            Finally
                MyConn.Close()
            End Try
            If url <> String.Empty Then Response.Redirect(url)
            Dim styles() As String = {
                U.Path.MapCMSCss("bootstrap"),
                U.Path.MapCMSCss("bootstrap." & U.Helper.GetHTMLDirection()),
                U.Path.MapCMSCss("font-awesome.min"),
                U.Path.MapCMSCss("ionicons.min"),
                U.Path.MapCMSCss("arabic-fonts"),
                U.Path.MapCMSCss("styles.min." & U.Helper.GetHTMLDirection()),
                U.Path.MapCMSCss("icheck"),
                U.Path.MapCMSCss("custom"),
                U.Path.MapCMSCss("custom." & U.Helper.GetHTMLDirection())
            }
            For Each s As String In styles
                litStyles.Text &= "<link rel=""stylesheet"" href=""" & s & """ />"
            Next
        End If
    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin.Click
        If Page.IsValid Then
            Dim shouldRedirect As Boolean = True
            Try
                MyConn.Open()
                Dim username As String = txtUserName.Text
                Dim password As String = txtPassword.Text
                Try
                    Dim user = B.UserLogin(username, password, MyConn)
                    U.Helper.LoadUser(user, chkRemember.Checked)
                Catch ex As Exception
                    shouldRedirect = False
                    U.ExceptionHandler.ProcessUnrecordedException(ex, egvResult, True)
                End Try
            Catch ex As Exception
                shouldRedirect = False
                U.ExceptionHandler.ProcessException(ex, egvNotifier, MyConn)
            Finally
                MyConn.Close()
            End Try
            If shouldRedirect Then Response.Redirect(BackURL)
        End If
    End Sub

    Protected Sub btnSendPassword_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSend.Click
        If Page.IsValid Then
            Try
                MyConn.Open()
                Dim username As String = txtPREmail.Text
                Dim usr = UserController.CreateRecoveryPassword(username, MyConn)
                UserEmails.SendRecoveryPassword(usr, MyConn)
                egvResult.Success(U.Localization.GetResource("Resources.Local.RecoverSuccess"))
            Catch ex As Exception
                U.ExceptionHandler.ProcessException(ex, egvResult, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

#End Region

End Class

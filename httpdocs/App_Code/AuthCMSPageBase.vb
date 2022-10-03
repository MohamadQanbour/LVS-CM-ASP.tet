Imports System.Data.SqlClient
Imports EGV
Imports EGV.Business

Public Class AuthCMSPageBase
    Inherits CMSPageBase

#Region "Public Properties"

    Public ReadOnly Property AuthUser As Business.User
        Get
            Return Utils.Helper.CMSAuthUser
        End Get
    End Property

    Public ReadOnly Property CMD As Integer
        Get
            If Request.QueryString("cmd") IsNot Nothing AndAlso IsNumeric(Request.QueryString("cmd")) Then Return Request.QueryString("cmd") Else Return 0
        End Get
    End Property

#End Region

#Region "Public Methods"

    Public Sub SessionTimeout()
        If Not Page.IsPostBack Then
            Dim back As String = HttpUtility.UrlEncode(Request.RawUrl)
            Response.Redirect(Utils.Path.MapCMSFile("login.aspx") & IIf(back <> String.Empty, "?url=" & back, ""))
        End If
    End Sub

    Public Sub AccessDenied()
        Response.Redirect(Utils.Path.MapCMSFile("AccessDenied.aspx"))
    End Sub

    Public Sub ProcessCMD(ByVal notifier As Interfaces.INotifier)
        Select Case CMD
            Case 1
                notifier.Success(Utils.Localization.GetResource("Resources.Local.CMD.Success"))
        End Select
    End Sub

    Public Overridable Sub ProcessPermissions(ByVal usr As User, Optional ByVal pid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        If pid > 0 Then
            Dim obj As New CMSMenu(pid, conn)
            If obj.IsSuper AndAlso usr.IsSuperAdmin = False Then AccessDenied()
            If Not AuthUser.CanRead(obj.PermissionId, conn) Then AccessDenied()
        End If
    End Sub

#End Region

#Region "Overridden Methods"

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        If AuthUser Is Nothing Then
            Dim reload As Boolean = Utils.Helper.AutoLogin(MyConn)
            If Not reload Then
                SessionTimeout()
            End If
        Else
            If AuthUser.RecoveryPassword <> Nothing AndAlso AuthUser.RecoveryPassword <> String.Empty AndAlso Request.Url.LocalPath <> Utils.Path.MapCMSFile("security/users-editor.aspx") Then
                Response.Redirect(Utils.Path.MapCMSFile("security/users-editor.aspx?select-tab=2&id=" & AuthUser.Id))
            End If
        End If
    End Sub

#End Region

End Class

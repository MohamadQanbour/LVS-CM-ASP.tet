Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business

Partial Class cms_super_exceptions_viewer
    Inherits AuthCMSPageBase

#Region "Properites"

    Public ReadOnly Property Key As Integer
        Get
            If Request.QueryString("id") IsNot Nothing AndAlso Request.QueryString("id") <> String.Empty AndAlso IsNumeric(Request.QueryString("id")) Then Return Request.QueryString("id") Else Return 0
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 12, MyConn)
                Dim title As String = LoadData(MyConn)
                Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 12)
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkDelete.Click
        Dim shouldRedirect As Boolean = True
        Try
            MyConn.Open()
            EGVExceptionController.DeleteException(Key, MyConn)
        Catch ex As Exception
            shouldRedirect = False
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
        If shouldRedirect Then Response.Redirect("exceptions.aspx?cmd=1")
    End Sub

#End Region

#Region "Private Methods"

    Private Function LoadData(ByVal conn As SqlConnection) As String
        If Key > 0 Then
            Dim obj As New EGVException(Key, conn)
            txtId.Text = obj.Id
            txtMessage.Text = obj.Message
            txtTrace.Text = obj.StackTrace
            txtDate.Text = obj.RecordDate.ToString("MMMM dd, yyyy @ hh:mm:ss")
            Return obj.Id
        Else
            Return String.Empty
        End If
    End Function

#End Region

End Class

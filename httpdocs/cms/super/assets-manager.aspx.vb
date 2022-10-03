Imports EGV.Utils

Partial Class cms_super_assets_manager
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ProcessCMD(Master.Notifier)
            Try
                MyConn.Open()
                ProcessPermissions(AuthUser, PageId, MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

#End Region

End Class

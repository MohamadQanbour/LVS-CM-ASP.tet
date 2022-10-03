Imports EGV.Utils
Imports EGV.Business

Partial Class cms_setup_emails
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ProcessCMD(Master.Notifier)
            Try
                MyConn.Open()
                ProcessPermissions(AuthUser, PageId, MyConn)
                egvGrid.BindGrid(MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub grid_DataSource(ByVal sender As Object, ByVal e As EventArgs) Handles egvGrid.GridNeedDataSource
        Try
            MyConn.Open()
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub grid_ToolbarClick(ByVal cmd As String) Handles egvGrid.ToolbarButtonClick
        Try
            MyConn.Open()
            Dim ids = egvGrid.GetSelectedIds()
            Select Case cmd.ToLower()
                Case "delete"
                    Dim completed As Boolean = True
                    For Each id As Integer In ids
                        completed = completed And EmailKeyController.Delete(id, MyConn)
                    Next
                    If completed Then Master.Notifier.Success(Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(Localization.GetResource("Resources.Local.delete.Warning"))
            End Select
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub grid_DeleteRow(ByVal id As Integer) Handles egvGrid.RowDelete
        Try
            MyConn.Open()
            Dim completed As Boolean = EmailKeyController.Delete(id, MyConn)
            If completed Then Master.Notifier.Success(Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(Localization.GetResource("Resources.Local.delete.Warning"))
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

#End Region

End Class

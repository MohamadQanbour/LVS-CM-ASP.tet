Imports System.Data.SqlClient
Imports U = EGV.Utils
Imports B = EGV.Business
Imports EGV.Business

Partial Class cms_setup_Settings
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
                U.ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Try
            MyConn.Open()
            Dim lst = egvGrid.GetEditableColumns()
            For Each c As EGV.Structures.EditableColumn In From i In lst Where i.ColumnName = "SettingValue"
                Dim key As String = c.ColumnKey
                If key <> Nothing AndAlso key <> String.Empty Then
                    Dim val As String = c.ColumnValue
                    B.BusinessHelper.WriteSetting(key, val, MyConn)
                End If
            Next
            Master.Notifier.Success(U.Localization.GetResource("Resources.Local.Notifier.Success"))
        Catch ex As Exception
            U.ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub egvGrid_NeedSource(ByVal sender As Object, ByVal e As EventArgs) Handles egvGrid.GridNeedDataSource
        Try
            MyConn.Open()
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            U.ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub egvGrid_RowDelete(ByVal key As String) Handles egvGrid.RowDelete
        Try
            MyConn.Open()
            B.SettingController.DeleteSetting(key, MyConn)
            Master.Notifier.Success(String.Format(U.Localization.GetResource("Resources.Local.Notifier.DeleteSuccess"), key))
        Catch ex As Exception
            U.ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

#End Region

#Region "Private Methods"

    Public Overrides Sub ProcessPermissions(usr As User, Optional ByVal pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        MyBase.ProcessPermissions(usr, pid, conn)
        hypAdd.Visible = usr.IsSuperAdmin
    End Sub

#End Region

End Class

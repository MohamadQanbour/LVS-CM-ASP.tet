Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business

Partial Class cms_membership_payments2
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ProcessCMD(Master.Notifier)
        Try
            MyConn.Open()
            ProcessPermissions(AuthUser, PageId, MyConn)
            If Not Page.IsPostBack Then BindGrid(MyConn)
            hdnUserId.Value = AuthUser.Id
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
        hdnReload.Value = Path.MapCMSFile("membership/payments2.aspx")
        EGVScriptManager.AddScript(Path.MapCMSScript("local/payments2"))
    End Sub

    Protected Sub grid_DataSource(ByVal sender As Object, ByVal e As EventArgs) Handles egvGrid.GridNeedDataSource
        Try
            MyConn.Open()
            BindGrid(MyConn)
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
                        completed = completed And StudentAccount2Controller.Delete(id, MyConn)
                    Next
                    If completed Then Master.Notifier.Success(Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(Localization.GetResource("Resources.Local.delete.Warning"))
            End Select
            BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub grid_DeleteRow(ByVal id As Integer) Handles egvGrid.RowDelete
        Try
            MyConn.Open()
            Dim completed As Boolean = StudentAccount2Controller.Delete(id, MyConn)
            If completed Then Master.Notifier.Success(Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(Localization.GetResource("Resources.Local.delete.Warning"))
            BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkStartSync_Click(sender As Object, e As EventArgs) Handles lnkStartSync.Click
        If txtSyncFile.HasFile() Then
            Dim file = txtSyncFile.File
            Dim assets As String = Helper.AssetsPath()
            Dim dir As String = "/" & assets & "/sync-students-files"
            Dim sDir As String = Server.MapPath("~" & dir)
            If Not IO.Directory.Exists(sDir) Then IO.Directory.CreateDirectory(sDir)
            If file.FileName <> String.Empty AndAlso file.ContentLength > 0 Then
                Dim ext As String = file.FileName.Substring(file.FileName.LastIndexOf("."))
                Dim newName As String = Guid.NewGuid().ToString()
                Dim newFileName As String = newName & ext
                Dim fPath As String = dir & "/" & newFileName
                file.SaveAs(Server.MapPath("~" & fPath))
                hdnSelectedFile.Value = fPath
            End If
        End If
        EGVScriptManager.AddInlineScript("$('.sync-accounts').modal('show'); StartSync();")
    End Sub

#End Region

#Region "Private Methods"

    Private Sub BindGrid(ByVal conn As SqlConnection)
        egvGrid.AddCondition("M.LastEntry = 1")
        egvGrid.BindGrid(conn)
    End Sub

#End Region

#Region "Public Methods"

    Public Overrides Sub ProcessPermissions(usr As User, Optional pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        MyBase.ProcessPermissions(usr, pid, conn)
        Dim obj As New CMSMenu(pid, conn)
        Dim canPublish As Boolean = usr.CanPublish(obj.PermissionId, conn)
        Dim canWrite As Boolean = usr.CanWrite(obj.PermissionId, conn)
        Dim canDelete As Boolean = usr.CanDelete(obj.PermissionId, conn)
        hypImport.Visible = canWrite
        If Not canDelete Then
            egvGrid.HideToolbarButton(btnDelete.ID)
        End If
    End Sub

#End Region

End Class

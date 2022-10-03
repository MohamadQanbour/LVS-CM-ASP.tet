Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business

Partial Class cms_membership_users
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        Try
            MyConn.Open()
            For Each dr As DataRow In SectionController.GetCollection(MyConn, LanguageId,,, True).List.Rows
                ddlSections.Items.Add(New ListItem() With {
                    .Text = Helper.GetSafeDBValue(dr("ClassName")) & " - " & Helper.GetSafeDBValue(dr("Title")),
                    .Value = Helper.GetSafeDBValue(dr("Id"), EGV.Enums.ValueTypes.TypeInteger)
                })
            Next
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ProcessCMD(Master.Notifier)
        Try
            MyConn.Open()
            ProcessPermissions(AuthUser, PageId, MyConn)
            If Not Page.IsPostBack Then egvGrid.BindGrid(MyConn)
            EGVScriptManager.AddScript(Path.MapCMSScript("local/users"))
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
        hdnReload.Value = Path.MapCMSFile("membership/users.aspx")
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
                Case "activate"
                    For Each id As Integer In ids
                        StudentController.ToggleState(id, True, MyConn)
                    Next
                    Master.Notifier.Success(Localization.GetResource("Resources.Local.Activate"))
                Case "deactivate"
                    For Each id As Integer In ids
                        StudentController.ToggleState(id, False, MyConn)
                    Next
                    Master.Notifier.Success(Localization.GetResource("Resources.Local.Deactivate"))
                Case "delete"
                    Dim completed As Boolean = True
                    For Each id As Integer In ids
                        completed = completed And StudentController.Delete(id, MyConn)
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
            Dim completed As Boolean = StudentController.Delete(id, MyConn)
            If completed Then Master.Notifier.Success(Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(Localization.GetResource("Resources.Local.delete.Warning"))
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub btnMove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMove.Click
        Dim newSectionId As Integer = ddlSections.SelectedValue
        Try
            MyConn.Open()
            Dim ids = egvGrid.GetSelectedIds()
            For Each id As Integer In ids
                StudentController.MoveToSection(id, newSectionId, MyConn)
            Next
            egvGrid.BindGrid(MyConn)
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
        EGVScriptManager.AddInlineScript("$('.sync-students-modal').modal('show'); StartSync();")
    End Sub

    Protected Sub lnkStartStudentSync_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkStartStudentSync.Click
        If Request.Files(1).ContentLength > 0 Then
            Dim file = Request.Files(1)
            Dim assets As String = Helper.AssetsPath()
            Dim dir As String = "/" & assets & "/sync-students-profile-files"
            Dim sDir As String = Server.MapPath("~" & dir)
            If Not IO.Directory.Exists(sDir) Then IO.Directory.CreateDirectory(sDir)
            If file.FileName <> String.Empty AndAlso file.ContentLength > 0 Then
                Dim ext As String = file.FileName.Substring(file.FileName.LastIndexOf("."))
                Dim newName As String = Guid.NewGuid().ToString()
                Dim newFileName As String = newName & ext
                Dim fPath As String = dir & "/" & newFileName
                file.SaveAs(Server.MapPath("~" & fPath))
                hdnSelectedStudentFile.Value = fPath
            End If
        End If
        EGVScriptManager.AddInlineScript("$('.sync-students-profile-modal').modal('show'); StartStudentSync();")
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
        hypAdd.Visible = canWrite
        If Not canPublish Then
            egvGrid.HideToolbarButton(btnActivate.ID)
            egvGrid.HideToolbarButton(btnDeactivate.ID)
        End If
        If Not canDelete Then
            egvGrid.HideToolbarButton(btnDelete.ID)
        End If
    End Sub

#End Region

End Class

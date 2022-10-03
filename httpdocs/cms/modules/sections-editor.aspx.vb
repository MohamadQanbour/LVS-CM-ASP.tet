Imports EGV
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_modules_sections_editor
    Inherits AuthCMSPageBase

#Region "Properites"

    Public ReadOnly Property Key As Integer
        Get
            If Request.QueryString("id") IsNot Nothing AndAlso Request.QueryString("id") <> String.Empty AndAlso IsNumeric(Request.QueryString("id")) Then Return Request.QueryString("id") Else Return 0
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        Try
            MyConn.Open()
            ddlClass.BindToDataSource(StudyClassController.GetCollection(MyConn, LanguageId).List, "Title", "Id", True)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            egvSaveCancel.IsEditing = Key > 0
            egvSaveCancel.BackPagePath = "sections.aspx"
            egvSaveCancel.AddPagePath = "sections-editor.aspx"
            egvSaveCancel.EditPagePath = "sections-editor.aspx?id={0}"
            hdnLanguageId.Value = LanguageId
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 24, MyConn)
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 24)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 24)
                End If
            End If
            EGVScriptManager.AddScript(Path.MapCMSScript("local/sections-editor"))
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub saveClick(ByVal sender As Object, ByVal e As EventArgs, ByRef hasError As Boolean) Handles egvSaveCancel.SaveClick
        If Page.IsValid Then
            Try
                MyConn.Open()
                SaveData(MyConn)
            Catch ex As Exception
                hasError = True
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Function LoadData(ByVal conn As SqlConnection) As String
        If Key > 0 Then
            Dim obj As New Section(Key, conn, LanguageId)
            txtId.Text = obj.Id
            txtTitle.Text = obj.Title
            txtCode.Text = obj.Code
            ddlClass.SelectedValue = obj.ClassId
            hdnSelectedClass.Value = obj.ClassId
            txtYear.Text = SeasonController.GetTitle(obj.SeasonId, LanguageId, MyConn)
            fileSchedule.InfoText = obj.ScheduleFilePath.Substring(obj.ScheduleFilePath.LastIndexOf("/") + 1)
            Return obj.Title
        Else
            txtYear.Text = SeasonController.GetCurrent(MyConn, LanguageId).Title
            ddlClass.SelectedIndex = 0
            hdnSelectedClass.Value = ddlClass.SelectedItem.Value
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        If hdnSelectedClass.Value = 0 Then
            Throw New Exception(Localization.GetResource("Resources.Local.MustSelectClass"))
        Else
            Dim obj As New Section(Key, conn, LanguageId)
            obj.Title = txtTitle.Text
            obj.Code = txtCode.Text
            obj.ClassId = hdnSelectedClass.Value
            If Key = 0 Then obj.SeasonId = SeasonController.GetCurrent(conn, LanguageId).Id
            If fileSchedule.HasFile Then
                Dim ext As String = fileSchedule.File.FileName.Substring(fileSchedule.File.FileName.LastIndexOf("."))
                Dim g As Guid = Guid.NewGuid()
                Dim gu As String = g.ToString().Substring(0, g.ToString().IndexOf("-"))
                Dim newFileName As String = Helper.EscapeURL(obj.Code) & "-Season" & obj.SeasonId & "-" & gu & ext
                Dim dir As String = "~" & Path.MapPortalAsset("schedule")
                Dim tPath As String = Server.MapPath(dir)
                If Not IO.Directory.Exists(tPath) Then IO.Directory.CreateDirectory(tPath)
                Dim fPath As String = dir & "/" & newFileName
                fileSchedule.File.SaveAs(Server.MapPath(fPath))
                Dim oldFile As String = obj.ScheduleFilePath
                obj.ScheduleFilePath = fPath.Replace("~", "")
                If oldFile <> String.Empty Then IO.File.Delete(Server.MapPath("~" & oldFile))
            Else
                If fileSchedule.InfoText = String.Empty Then
                    Dim oldFile As String = obj.ScheduleFilePath
                    obj.ScheduleFilePath = String.Empty
                    If oldFile <> String.Empty Then IO.File.Delete(Server.MapPath("~" & oldFile))
                End If
            End If
            Dim trans As SqlTransaction = conn.BeginTransaction()
            Try
                obj.Save(trans)
                If Key = 0 Then egvSaveCancel.NewId = obj.Id
                trans.Commit()
                Master.Notifier.Success(String.Format(Localization.GetResource("Resources.Local.SaveSuccess"), obj.Title))
            Catch ex As Exception
                trans.Rollback()
                Throw ex
            End Try
        End If
    End Sub

#End Region

#Region "Public Methods"

    Public Overrides Sub ProcessPermissions(usr As User, Optional pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        MyBase.ProcessPermissions(usr, pid, conn)
        Dim obj As New CMSMenu(pid, conn)
        If Key > 0 AndAlso Not usr.CanModify(obj.PermissionId, conn) Then AccessDenied()
        If Key = 0 AndAlso Not usr.CanWrite(obj.PermissionId, conn) Then AccessDenied()
    End Sub

#End Region

End Class

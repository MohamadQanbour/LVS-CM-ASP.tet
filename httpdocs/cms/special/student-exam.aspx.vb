Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_special_student_exam
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If AuthUser IsNot Nothing Then
            hdnUserId.Value = AuthUser.Id
            hdnSaveSuccess.Value = Localization.GetResource("Resources.Local.SaveSuccess")
            hdnPublishSuccess.Value = Localization.GetResource("Resources.Local.PublishSuccess")
            hdnMaxMarkResource.Value = Localization.GetResource("Resources.Local.MaxMark")
            If Not Page.IsPostBack Then
                ddlClass.Items.Add(New ListItem(Localization.GetResource("Resources.Local.SelectClass"), "0"))
                ddlClassPublish.Items.Add(New ListItem(Localization.GetResource("Resources.Local.SelectClass"), "0"))
                ddlStudent.Items.Add(New ListItem(Localization.GetResource("Resources.Local.SelectStudent"), "0"))
                EGVScriptManager.AddScripts(False, {
                    Path.MapCMSScript("jquery.inputmask.js"),
                    Path.MapCMSScript("jquery.inputmask.extensions.js"),
                    Path.MapCMSScript("jquery.inputmask.numeric.extensions.js")
                })
                EGVScriptManager.AddScript(Path.MapCMSScript("local/student-exam") & "?v=3.0")
            End If
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

    Public Overrides Sub ProcessPermissions(usr As User, Optional pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        MyBase.ProcessPermissions(usr, pid, conn)
        Dim obj As New CMSMenu(pid, conn)
        Dim canPublish As Boolean = usr.CanPublish(obj.PermissionId, conn)
        Dim canDelete As Boolean = usr.CanDelete(obj.PermissionId, conn)
        Dim canModify As Boolean = usr.CanModify(obj.PermissionId, conn)
        Dim canWrite As Boolean = usr.CanWrite(obj.PermissionId, conn)
        hdnCanUpdate.Value = canModify Or canWrite
        If Not canPublish Then egvTabs.HideTab("tabPublish")
        lnkSaveMaterial.Visible = canModify Or canWrite
        lnkSaveStudent.Visible = canModify Or canWrite
    End Sub

#End Region

End Class

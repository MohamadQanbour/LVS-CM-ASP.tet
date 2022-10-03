Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_special_student_report
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If AuthUser IsNot Nothing Then
            hdnUserId.Value = AuthUser.Id
            If Not Page.IsPostBack Then
                ddlClass.Items.Add(New ListItem(Localization.GetResource("Resources.Local.SelectClass"), "0"))
                EGVScriptManager.AddScripts(False, {
                    Path.MapCMSScript("jquery.inputmask.js"),
                    Path.MapCMSScript("jquery.inputmask.extensions.js"),
                    Path.MapCMSScript("jquery.inputmask.numeric.extensions.js")
                })
                EGVScriptManager.AddScript(Path.MapCMSScript("local/student-report") & "?v=1.0")
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
    End Sub

    Protected Sub lnk_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkLoadStudents.Click
        Dim classId As Integer = hdnSelectedClass.Value
        Try
            MyConn.Open()
            Dim items As List(Of Integer) = Helper.JSDeserialize(Of List(Of Integer))(hdnSelectedTemplates.Value)
            Dim classMaxMark As Integer = StudyClassController.GetClassMaxMark(classId, MyConn, items.ToArray())
            rptItems.DataSource = StudentExamController.GetReport(hdnSeletedSection.Value, classMaxMark, MyConn, items.ToArray()).List
            rptItems.DataBind()
            egvStudentBox.Visible = True
            Dim exams As New List(Of String)
            For Each item As Integer In items
                exams.Add(New ExamTemplateItem(item, LanguageId, MyConn).Title)
            Next
            litTitle.Text = New StudyClass(classId, MyConn, LanguageId).Title & " - " & New Section(hdnSeletedSection.Value, MyConn, LanguageId).Title & " Max (" & classMaxMark & ")<br />Exams: " & String.Join(", ", exams.ToArray())
            hdnBoxClass.Value = classId
            hdnBoxSection.Value = hdnSeletedSection.Value
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkExcel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkExcel.Click
        Dim classId As Integer = hdnSelectedClass.Value
        Try
            MyConn.Open()
            Dim items As List(Of Integer) = Helper.JSDeserialize(Of List(Of Integer))(hdnSelectedTemplates.Value)
            Dim classMaxMark As Integer = StudyClassController.GetClassMaxMark(classId, MyConn, items.ToArray())
            Dim dt = StudentExamController.GetReport(hdnSeletedSection.Value, classMaxMark, MyConn, items.ToArray()).List
            Helper.DatatableToCSV(dt, "StudentReport-" & New StudyClass(hdnBoxClass.Value, MyConn, LanguageId).Code & "-" & New Section(hdnSeletedSection.Value, MyConn, LanguageId).Code & "-" & Now.ToString("yyyyMMddhhmmss"))
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

#End Region

End Class

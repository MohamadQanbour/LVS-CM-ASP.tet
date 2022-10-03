Imports EGV
Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_special_attendance
    Inherits AuthCMSPageBase

    Public Property CanUpdate As Boolean = False

#Region "Event Handlers"

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        Try
            MyConn.Open()
            Dim enables As Boolean = True
            If AuthUser Is Nothing Then Response.Redirect(Path.MapCMSFile("Login.aspx"))
            Dim classes As DBAReturnObject = StudentPresentController.GetClassesOfAdmin(MyConn, AuthUser.Id, LanguageId)
            If classes.Count > 0 Then
                ddlClasses.BindToDataSource(classes.List, "Title", "Id")
                Dim classId As Integer = ddlClasses.SelectedValue
                Dim sections As DBAReturnObject = StudentPresentController.GetSectionsOfAdmin(MyConn, AuthUser.Id, classId, LanguageId)
                If sections.Count > 0 Then
                    ddlSections.BindToDataSource(sections.List, "Title", "Id")
                    hdnSelectedSection.Value = ddlSections.SelectedValue
                Else
                    enables = False
                End If
            Else
                enables = False
            End If
            If Not enables Then
                ddlClasses.Enabled = False
                ddlSections.Enabled = False
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            hdnUserId.Value = AuthUser.Id
            ProcessPermissions(AuthUser, PageId, MyConn)
            If Not Page.IsPostBack Then
                txtDate.Text = Now.Date.ToString("yyyy-MM-dd")
                BindData(MyConn)
            End If
            EGVScriptManager.AddScript(Path.MapCMSScript("local/attendance"), False, "1.3")
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub btnLoad_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLoad.Click
        If Page.IsValid Then
            Try
                MyConn.Open()
                BindData(MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSave.Click
        Try
            MyConn.Open()
            Dim lst As List(Of SectionStudentAttendance) = Helper.JSDeserialize(Of List(Of SectionStudentAttendance))(hdnValues.Value)
            Dim d As Date = Helper.ParseDate(txtDate.Text)
            'StudentPresentController.DeleteAttendance(MyConn, d)
            For Each item As SectionStudentAttendance In lst
                StudentPresentController.AddStudentPresentDay(MyConn, item.StudentId, d, item.StudentAttend)
            Next
            BindData(MyConn)
            Master.Notifier.Success(Localization.GetResource("Resources.Local.SaveSuccess"))
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Public Overrides Sub ProcessPermissions(usr As User, Optional pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        MyBase.ProcessPermissions(usr, pid, conn)
        Dim obj As New CMSMenu(pid, conn)
        Dim canPublish As Boolean = usr.CanPublish(obj.PermissionId, conn)
        Dim canDelete As Boolean = usr.CanDelete(obj.PermissionId, conn)
        Dim canModify As Boolean = usr.CanModify(obj.PermissionId, conn)
        Dim canWrite As Boolean = usr.CanWrite(obj.PermissionId, conn)
        lnkSave.Visible = canWrite Or canModify
        CanUpdate = canWrite Or canModify
    End Sub

#End Region

#Region "Private Methods"

    Private Sub BindData(ByVal conn As SqlConnection)
        If ddlClasses.Enabled AndAlso ddlSections.Enabled Then
            Dim lst As List(Of SectionStudentAttendance) = StudentPresentController.GetSectionStudentsAttendance(conn, hdnSelectedSection.Value, Helper.ParseDate(txtDate.Text))
            rptStudents.DataSource = lst
            rptStudents.DataBind()
            hdnValues.Value = Helper.JSSerialize(lst)
            egvBox.LoadTitle(SectionController.GetTitle(conn, hdnSelectedSection.Value, LanguageId))
            ddlSections.BindToDataSource(StudentPresentController.GetSectionsOfAdmin(MyConn, AuthUser.Id, ddlClasses.SelectedValue, LanguageId).List, "Title", "Id")
            If ddlSections.Items.FindByValue(hdnSelectedSection.Value) IsNot Nothing Then
                ddlSections.SelectedValue = hdnSelectedSection.Value
            End If
        Else
            egvBox.Visible = False
        End If
    End Sub

#End Region

End Class

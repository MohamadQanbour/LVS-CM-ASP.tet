Imports EGV
Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_special_class_days
    Inherits AuthCMSPageBase

    Public Property CanUpdate As Boolean = False

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Try
                MyConn.Open()
                ProcessPermissions(AuthUser, PageId, MyConn)
                BindData(MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Try
            MyConn.Open()
            For Each item As RepeaterItem In rptClasses.Items
                If item.ItemType = ListItemType.Item OrElse item.ItemType = ListItemType.AlternatingItem Then
                    Dim tr As HtmlTableRow = item.FindControl("tr")
                    Dim txtSchool As EGVControls.EGVTextBox = item.FindControl("txtSchoolDays")
                    Dim txtHoliday As EGVControls.EGVTextBox = item.FindControl("txtHolidayDays")
                    Dim txtSchool2 As EGVControls.EGVTextBox = item.FindControl("txtSchoolDays2")
                    Dim txtHoliday2 As EGVControls.EGVTextBox = item.FindControl("txtHolidayDays2")
                    Dim classId As Integer = tr.Attributes("data-classid")
                    Dim schoolDays As Integer = Helper.GetSafeObject(txtSchool.Text, ValueTypes.TypeInteger)
                    Dim holidayDays As Integer = Helper.GetSafeObject(txtHoliday.Text, ValueTypes.TypeInteger)
                    Dim schoolDays2 As Integer = Helper.GetSafeObject(txtSchool2.Text, ValueTypes.TypeInteger)
                    Dim holidayDays2 As Integer = Helper.GetSafeObject(txtHoliday2.Text, ValueTypes.TypeInteger)
                    StudyClassController.UpdateDays(MyConn, classId, schoolDays, holidayDays, schoolDays2, holidayDays2)
                End If
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
        btnSave.Visible = canWrite Or canModify
        CanUpdate = canWrite Or canModify
    End Sub

#End Region

#Region "Private Methods"

    Private Sub BindData(ByVal conn As SqlConnection)
        rptClasses.DataSource = StudyClassController.GetClassesDays(conn, LanguageId)
        rptClasses.DataBind()
    End Sub

#End Region

End Class

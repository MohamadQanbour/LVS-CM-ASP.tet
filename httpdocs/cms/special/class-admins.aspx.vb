Imports EGV
Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_special_class_admins
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
            ClassAdminsController.DeleteClassAdmins(MyConn)
            For Each item As RepeaterItem In rptClasses.Items
                If item.ItemType = ListItemType.Item OrElse item.ItemType = ListItemType.AlternatingItem Then
                    Dim rpt As Repeater = item.FindControl("rptSections")
                    If rpt IsNot Nothing Then
                        For Each subitem As RepeaterItem In rpt.Items
                            Dim tr As HtmlTableRow = subitem.FindControl("tr")
                            Dim userId As EGVControls.EGVDropDown = subitem.FindControl("ddlUsers")
                            Dim sectionId As Integer = tr.Attributes("data-sectionid")
                            ClassAdminsController.UpdateClassAdmin(MyConn, sectionId, userId.SelectedValue)
                        Next
                    End If
                End If
            Next
            BindData(MyConn)
            Master.Notifier.Success(Localization.GetResource("Resources.Local.CMSSuccess"))
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub rptClasses_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptClasses.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim dr As DataRowView = e.Item.DataItem
            Dim sections = SectionController.GetCollection(MyConn, LanguageId, 0, String.Empty, True, dr("Id"))
            Dim rpt As Repeater = e.Item.FindControl("rptSections")
            rpt.DataSource = sections.List
            rpt.DataBind()
        End If
    End Sub

    Protected Sub rpt_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As DataRowView = e.Item.DataItem
            Dim ddl As EGVControls.EGVDropDown = e.Item.FindControl("ddlUsers")
            ddl.BindToDataSource(UserController.GetUsersOfType(MyConn, "ca").List, "FullName", "Id", True)
            ddl.SelectedValue = ClassAdminsController.GetSectionAdmin(MyConn, item("Id"))
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
        btnSave.Visible = canWrite Or canModify
        CanUpdate = canWrite Or canModify
    End Sub

#End Region

#Region "Private Methods"

    Private Sub BindData(ByVal conn As SqlConnection)
        rptClasses.DataSource = StudyClassController.GetCollection(conn, LanguageId).List
        'rptClasses.DataSource = ClassAdminsController.GetClassAdmins(conn, LanguageId)
        rptClasses.DataBind()
    End Sub

#End Region

End Class

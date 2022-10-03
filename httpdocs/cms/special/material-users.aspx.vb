Imports EGV
Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_special_material_users
    Inherits AuthCMSPageBase

    Public Property CanUpdate As Boolean = False

#Region "Event Handlers"

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        Try
            MyConn.Open()
            Dim enabled As Boolean = True
            Dim classes As DBAReturnObject = StudyClassController.GetCollection(MyConn, LanguageId)
            If classes.Count > 0 Then
                ddlClasses.BindToDataSource(classes.List, "Title", "Id")
                Dim classId As Integer = ddlClasses.SelectedValue
                Dim sections As DBAReturnObject = SectionController.GetCollection(MyConn, LanguageId, 0, "", True, classId)
                If sections.Count > 0 Then
                    ddlSections.BindToDataSource(sections.List, "Title", "Id")
                    hdnSelectedSection.Value = ddlSections.SelectedValue
                Else
                    enabled = False
                End If
            Else
                enabled = False
            End If
            If Not enabled Then
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
        ProcessPermissions(AuthUser, PageId, MyConn)
        If Not Page.IsPostBack Then
            Try
                MyConn.Open()
                BindData(MyConn)
                EGVScriptManager.AddScript(Path.MapCMSScript("local/material-users"))
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub rpt_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptMaterials.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As MaterialObject = e.Item.DataItem
            Dim ddl As EGVControls.EGVDropDown = e.Item.FindControl("ddlTeacher")
            ddl.BindToDataSource(UserController.GetUsersOfType(MyConn, "t").List, "FullName", "Id", True)
        End If
    End Sub

    Protected Sub btnLoad_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLoad.Click
        Try
            MyConn.Open()
            BindData(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSave.Click
        Try
            MyConn.Open()
            Dim lst = Helper.JSDeserialize(Of List(Of MaterialUser))(hdnValues.Value)
            SectionMaterialUserController.UpdateMaterials(MyConn, hdnSelectedSection.Value, lst)
            BindData(MyConn)
            Master.Notifier.Success(Localization.GetResource("Resources.Local.CMSSuccess"))
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
        If ddlClasses.Items.Count > 0 AndAlso ddlSections.Items.Count > 0 Then
            Dim ds = SectionController.GetSectionMaterials(MyConn, hdnSelectedSection.Value, LanguageId)
            rptMaterials.DataSource = ds
            rptMaterials.DataBind()
            Dim lst As New List(Of MaterialUser)
            For Each item As MaterialObject In ds
                lst.Add(New MaterialUser() With {
                    .MaterialId = item.Id,
                    .UserId = SectionMaterialUserController.GetMaterialUser(MyConn, hdnSelectedSection.Value, item.Id)
                })
            Next
            hdnValues.Value = Helper.JSSerialize(lst)
            egvBox.LoadTitle(SectionController.GetTitle(conn, hdnSelectedSection.Value, LanguageId))
            ddlSections.BindToDataSource(SectionController.GetCollection(MyConn, LanguageId, 0, "", True, ddlClasses.SelectedValue).List, "FullName", "Id")
            ddlSections.SelectedValue = hdnSelectedSection.Value
        Else
            egvBox.Visible = False
            ddlClasses.Enabled = False
            ddlSections.Enabled = False
        End If
    End Sub

#End Region

End Class

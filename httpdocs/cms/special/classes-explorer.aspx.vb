Imports EGV
Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_special_classes_explorer
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        Try
            MyConn.Open()
            'Dim classes As DBAReturnObject = StudyClassController.GetCollection(MyConn, LanguageId)
            Dim classes = StudyClassController.GetTeacherClasses(MyConn, AuthUser.Id, LanguageId)
            If classes.Count > 0 Then
                ddlClasses.DataTextField = "Title"
                ddlClasses.DataValueField = "Id"
                ddlClasses.DataSource = classes
                ddlClasses.DataBind()
                'ddlClasses.BindToDataSource(classes, "Title", "Id")
            Else
                ddlClasses.Enabled = False
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
            If Not Page.IsPostBack Then
                ProcessPermissions(AuthUser, PageId, MyConn)
                BindData(MyConn)
                EGVScriptManager.AddScript(Path.MapCMSScript("local/classes-explorer"))
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub rpt_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptItems.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            'Dim dr As DataRowView = e.Item.DataItem
            'Dim rpt As Repeater = e.Item.FindControl("rptMaterials")
            'rpt.DataSource = SectionMaterialUserController.GetUserMaterials(MyConn, AuthUser.Id, dr("Id"), LanguageId)
            'rpt.DataBind()
            Dim obj As SectionObject = e.Item.DataItem
            Dim rpt As Repeater = e.Item.FindControl("rptMaterials")
            rpt.DataSource = SectionMaterialUserController.GetUserMaterials(MyConn, AuthUser.Id, obj.Id, LanguageId)
            rpt.DataBind()
            Dim hyp As HyperLink = e.Item.FindControl("hypSchedule")
            If hyp IsNot Nothing Then
                Dim scheduleFile As String = New Section(obj.Id, MyConn, LanguageId).ScheduleFilePath
                If scheduleFile <> String.Empty Then
                    hyp.NavigateUrl = scheduleFile
                    hyp.Visible = True
                Else
                    hyp.Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub lnkLoad_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLoad.Click
        Try
            MyConn.Open()
            BindData(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

#End Region

#Region "Private Methods"

    Private Sub BindData(ByVal conn As SqlConnection)
        If ddlClasses.Enabled Then
            Dim classId As Integer = ddlClasses.SelectedValue
            Dim sections = SectionController.GetTeacherSections(MyConn, AuthUser.Id, LanguageId, String.Empty, classId)
            'rptItems.DataSource = SectionController.GetCollection(conn, LanguageId, 0, "", True, classId).List
            rptItems.DataSource = sections
            rptItems.DataBind()
            egvBox.LoadTitle(StudyClassController.GetTitle(conn, classId, LanguageId))
        Else
            egvBox.Visible = False
        End If
    End Sub

#End Region

End Class

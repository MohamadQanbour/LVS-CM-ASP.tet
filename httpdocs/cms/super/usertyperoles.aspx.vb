Imports EGV
Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_super_usertyperoles
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            If Not Page.IsPostBack Then
                ProcessPermissions(AuthUser, PageId, MyConn)
                EGVScriptManager.AddScript(Path.MapCMSScript("local/usertyperoles"))
                Dim lst = UserTypeRoleController.GetList(MyConn)
                rpt.DataSource = lst
                rpt.DataBind()
                hdnValues.Value = Helper.JSSerialize(lst)
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        If Page.IsValid Then
            Try
                MyConn.Open()
                SaveData(MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub rpt_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rpt.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim dr As UserTypeRole = e.Item.DataItem
            Dim ddl As EGVControls.EGVDropDown = e.Item.FindControl("ddlRole")
            ddl.BindToDataSource(RoleController.List(MyConn, False).List, "Title", "Id", True)
            ddl.SelectedValue = dr.RoleId
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim lst = Helper.JSDeserialize(Of List(Of UserTypeRole))(hdnValues.Value)
        UserTypeRoleController.Update(conn, lst)
    End Sub

#End Region

End Class

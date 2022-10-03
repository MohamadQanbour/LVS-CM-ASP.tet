Imports EGV
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_super_menus_editor
    Inherits AuthCMSPageBase

#Region "Properites"

    Public ReadOnly Property Key As Integer
        Get
            If Request.QueryString("id") IsNot Nothing AndAlso Request.QueryString("id") <> String.Empty AndAlso IsNumeric(Request.QueryString("id")) Then Return Request.QueryString("id") Else Return 0
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            egvSaveCancel.IsEditing = Key > 0
            egvSaveCancel.BackPagePath = "Menus.aspx"
            egvSaveCancel.AddPagePath = "menus-editor.aspx"
            egvSaveCancel.EditPagePath = "menus-editor.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 5, MyConn)
                ddlParent.BindToDataSource(CMSMenuController.ListParents(MyConn).List, "Title", "Id", True, 0, "Resources.Local.ddlParent.NullText")
                ddlPermission.BindToDataSource(PermissionController.List(MyConn).List, "Title", "Id", True, 0, "Resources.Local.ddlPermission.NullText")
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 5)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 5)
                End If
            End If
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
            Dim obj As New CMSMenu(Key, conn)
            txtId.Text = obj.Id
            txtTitle.Text = obj.Title
            txtIcon.Text = obj.IconClass
            ddlParent.SelectedValue = obj.ParentId
            txtPath.Text = obj.PagePath
            txtPageTitle.Text = obj.PageTitle
            txtDesc.Text = Helper.BR2NL(obj.PageDescription)
            ddlPermission.SelectedValue = obj.PermissionId
            chkSuper.Checked = obj.IsSuper
            txtOrder.Text = obj.Order
            chkPublished.Checked = obj.IsPublished
            Return obj.Title
        Else
            chkPublished.Checked = True
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim obj As New CMSMenu(Key, conn)
        obj.Title = txtTitle.Text
        obj.IconClass = txtIcon.Text
        obj.ParentId = ddlParent.SelectedValue
        obj.PagePath = txtPath.Text
        obj.PageTitle = txtPageTitle.Text
        obj.PageDescription = Helper.BR2NL(txtDesc.Text)
        obj.PermissionId = ddlPermission.SelectedValue
        obj.IsSuper = chkSuper.Checked
        obj.Order = txtOrder.Text
        obj.IsPublished = chkPublished.Checked
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
    End Sub

#End Region

End Class

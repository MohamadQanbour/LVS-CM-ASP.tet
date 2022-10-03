Imports EGV
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_security_roles_editor
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
            egvSaveCancel.BackPagePath = "roles.aspx"
            egvSaveCancel.AddPagePath = "roles-editor.aspx"
            egvSaveCancel.EditPagePath = "roles-editor.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 8, MyConn)
                EGVScriptManager.AddScript(Path.MapCMSScript("local/roles-editor"))
                rptPermissions.DataSource = PermissionController.List(MyConn).List
                rptPermissions.DataBind()
                rptLanguages.DataSource = LanguageController.List(MyConn).List
                rptLanguages.DataBind()
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 8)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 8)
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
            Dim obj As New Role(Key, conn)
            txtId.Text = obj.Id
            txtTitle.Text = obj.Title
            chkSecure.Checked = obj.IsSecure
            chkActive.Checked = obj.IsActive
            chkAdmin.Checked = obj.IsAdmin
            hdnPermissions.Value = Helper.JSSerialize(obj.Permissions)
            hdnLanguages.Value = Helper.JSSerialize(obj.Languages)
            Return obj.Title
        Else
            hdnLanguages.Value = "[]"
            hdnPermissions.Value = "[]"
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim obj As New Role(Key, conn)
        obj.Title = txtTitle.Text
        obj.IsSecure = chkSecure.Checked
        obj.IsActive = chkActive.Checked
        obj.IsAdmin = chkAdmin.Checked
        obj.Permissions = RolePermissionController.GetListFromJSON(Key, hdnPermissions.Value)
        obj.Languages = Helper.JSDeserialize(Of List(Of Integer))(hdnLanguages.Value)
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

#Region "Public Methods"

    Public Overrides Sub ProcessPermissions(usr As User, Optional pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        MyBase.ProcessPermissions(usr, pid, conn)
        Dim obj As New CMSMenu(pid, MyConn)
        If Key > 0 AndAlso Not usr.CanModify(obj.PermissionId, conn) Then AccessDenied()
        If Key = 0 AndAlso Not usr.CanWrite(obj.PermissionId, conn) Then AccessDenied()
    End Sub

#End Region

End Class

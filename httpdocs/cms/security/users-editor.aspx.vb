Imports EGV
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_security_users_editor
    Inherits AuthCMSPageBase

#Region "Properites"

    Public ReadOnly Property Key As Integer
        Get
            If Request.QueryString("id") IsNot Nothing AndAlso Request.QueryString("id") <> String.Empty AndAlso IsNumeric(Request.QueryString("id")) Then Return Request.QueryString("id") Else Return 0
        End Get
    End Property

    Public Property TotalRoles As Integer = 0

#End Region

#Region "Event Handlers"

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        Try
            MyConn.Open()
            ddlLanguage.BindToDataSource(LanguageController.List(MyConn, LanguageId, True).List, "Title", "Id")
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            If AuthUser Is Nothing Then Response.Redirect(Path.MapCMSFile("Login.aspx"))
            egvSaveCancel.IsEditing = Key > 0
            Dim pid As Integer = New CMSMenu(11, MyConn).PermissionId
            Dim usrCanRead = AuthUser.CanRead(pid, MyConn)
            egvSaveCancel.BackPagePath = IIf(usrCanRead, "users.aspx", Path.MapCMSFile("Default.aspx"))
            egvSaveCancel.AddPagePath = IIf(usrCanRead, "users-editor.aspx", Path.MapCMSFile("Default.aspx"))
            egvSaveCancel.EditPagePath = "users-editor.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 11, MyConn)
                EGVScriptManager.AddScript(Path.MapCMSScript("local/users-editor"))
                Dim roles = RoleController.List(MyConn, False)
                TotalRoles = roles.Count
                ddlRole.BindToDataSource(roles.List, "Title", "Id")
                egvImageEditor.MaxWidth = 160
                egvImageEditor.MaxHeight = 160
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(Localization.GetResource("Resources.Local.Page.EditTitle"), title), "", Localization.GetResource("Resources.Local.Page.BCEditTitle"), 11)
                Else
                    Master.LoadTitles(Localization.GetResource("Resources.Local.Page.AddTitle"), "", Localization.GetResource("Resources.Local.Page.BCAddTitle"), 11)
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
        Else
            hasError = True
        End If
    End Sub

    Protected Sub changePassword_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkChangePassword.Click
        If Page.IsValid Then
            Try
                MyConn.Open()
                ChangePassword(MyConn)
                Master.Notifier.Success(Localization.GetResource("Resources.Local.ChangePassword.Success"))
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub valUsername_Validate(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Try
            MyConn.Open()
            e.IsValid = Not UserController.UserNameExists(txtUsername.Text, Key, MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub valEmail_Validate(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Try
            MyConn.Open()
            e.IsValid = Not UserController.EmailExists(txtEmail.Text, Key, MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

#End Region

#Region "Private Methods"

    Private Function LoadData(ByVal conn As SqlConnection) As String
        If Key > 0 Then
            rowPassword.Visible = False
            rowRepeatPassword.Visible = False
            Dim obj As New User(Key, conn)
            txtId.Text = obj.Id
            txtUsername.Text = obj.UserName
            txtEmail.Text = obj.Email
            chkSuper.Checked = obj.IsSuperAdmin
            chkActive.Checked = obj.IsActive
            chkAllowList.Checked = obj.AllowList
            If obj.Roles.Count > 0 Then
                ddlRole.SelectedValue = obj.Roles(0)
                hdnSelectedRole.Value = obj.Roles(0)
            End If
            txtFullName.Text = obj.Profile.FullName
            egvImageEditor.LoadImage(obj.Profile.ProfileImageUrl)
            ddlLanguage.SelectedValue = obj.Profile.UserLanguageId
            If Not AuthUser.IsSuperAdmin AndAlso obj.IsSuperAdmin Then AccessDenied()
            Return obj.Profile.FullName
        Else
            rowPassword.Visible = True
            rowRepeatPassword.Visible = True
            egvTabs.HideTab(tabPassword.Id)
            hdnSelectedRole.Value = ddlRole.SelectedValue
            ddlLanguage.SelectedValue = LanguageController.GetDefaultId(conn)
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim obj As New User(Key, conn)
        obj.UserName = txtUsername.Text
        obj.Email = txtEmail.Text
        obj.IsSuperAdmin = chkSuper.Checked
        obj.IsActive = chkActive.Checked
        obj.AllowList = chkAllowList.Checked
        obj.Roles.Clear()
        hdnSelectedRole.Value = ddlRole.SelectedValue
        obj.Roles.Add(hdnSelectedRole.Value)
        obj.Profile.UserLanguageId = ddlLanguage.SelectedValue
        If Key = 0 Then obj.Password = txtPassword.Text
        'profile
        obj.Profile.FullName = txtFullName.Text
        obj.Profile.ProfileImageUrl = egvImageEditor.GetImage()
        obj.Profile.UserLanguageId = ddlLanguage.SelectedValue
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            obj.Save(trans)
            If Key = 0 Then egvSaveCancel.NewId = obj.Id
            trans.Commit()
            Master.Notifier.Success(String.Format(Localization.GetResource("Resources.Local.SaveSuccess"), obj.Profile.FullName))
        Catch ex As Exception
            trans.Rollback()
            Throw ex
        End Try
        Helper.ReloadUser(conn)
    End Sub

    Private Sub ChangePassword(ByVal conn As SqlConnection)
        Dim obj As New User(Key, conn)
        obj.Password = txtNewPassword.Text
        obj.RecoveryPassword = String.Empty
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            obj.Save(trans)
            trans.Commit()
        Catch ex As Exception
            trans.Rollback()
            Throw ex
        End Try
        Helper.ReloadUser(conn)
    End Sub

#End Region

#Region "Public Methods"

    Public Overrides Sub ProcessPermissions(usr As User, Optional pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        Dim obj As New CMSMenu(pid, conn)
        If Key <> usr.Id Then
            MyBase.ProcessPermissions(usr, pid, conn)
            If Key > 0 AndAlso Not usr.CanModify(obj.PermissionId, conn) Then AccessDenied()
            If Key = 0 AndAlso Not usr.CanWrite(obj.PermissionId, conn) Then AccessDenied()
        End If
        rowSuper.Visible = usr.IsSuperAdmin
        Dim userCanModify As Boolean = usr.CanModify(obj.PermissionId, conn)
        rowActive.Visible = userCanModify
        rowRole.Visible = userCanModify
    End Sub

#End Region

End Class

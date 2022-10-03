Imports EGV
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_membership_family_editor
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
            egvSaveCancel.BackPagePath = "family.aspx"
            egvSaveCancel.AddPagePath = "family-editor.aspx"
            egvSaveCancel.EditPagePath = "family-editor.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 17, MyConn)
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 17)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 17)
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
            e.IsValid = Not FamilyController.UsernameExists(txtUsername.Text, Key, MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub valEmail_Validate(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Try
            MyConn.Open()
            e.IsValid = Not FamilyController.EmailExists(txtEmail.Text, Key, MyConn)
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
            rowLastLogin.Visible = True
            Dim obj As New Family(Key, conn)
            txtId.Text = obj.Id
            txtUsername.Text = obj.SchoolId
            txtEmail.Text = obj.Email
            chkActive.Checked = obj.IsActive
            txtFullName.Text = obj.FullName
            txtLastLogin.Text = IIf(obj.LastLoginDate = DateTime.MinValue, Localization.GetResource("Resources.Local.NotLoggedIn"), obj.LastLoginDate.ToString("MMMM dd, yyyy @ hh:mm:ss"))
            Return obj.FullName
        Else
            rowPassword.Visible = True
            rowRepeatPassword.Visible = True
            rowLastLogin.Visible = False
            egvTabs.HideTab(tabPassword.Id)
            chkActive.Checked = True
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim obj As New Family(Key, conn)
        obj.SchoolId = txtUsername.Text
        obj.Email = txtEmail.Text
        obj.FullName = txtFullName.Text
        obj.IsActive = chkActive.Checked
        If Key = 0 Then obj.Password = txtPassword.Text
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            obj.Save(trans)
            If Key = 0 Then egvSaveCancel.NewId = obj.Id
            trans.Commit()
            Master.Notifier.Success(String.Format(Localization.GetResource("Resources.Local.SaveSuccess"), obj.FullName))
        Catch ex As Exception
            trans.Rollback()
            Throw ex
        End Try
    End Sub

    Private Sub ChangePassword(ByVal conn As SqlConnection)
        Dim obj As New Family(Key, conn)
        obj.Password = txtNewPassword.Text
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            obj.Save(trans)
            trans.Commit()
        Catch ex As Exception
            trans.Rollback()
            Throw ex
        End Try
    End Sub

#End Region

#Region "Public Methods"

    Public Overrides Sub ProcessPermissions(usr As User, Optional pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        If Key <> usr.Id Then
            MyBase.ProcessPermissions(usr, pid, conn)
            Dim obj As New CMSMenu(pid, conn)
            If usr.CanRead(obj.PermissionId, conn) AndAlso Not usr.CanModify(obj.PermissionId, conn) AndAlso Not usr.CanWrite(obj.PermissionId, conn) Then
                txtId.ReadOnly = True
                txtUsername.ReadOnly = True
                txtEmail.ReadOnly = True
                rowPassword.Visible = False
                rowRepeatPassword.Visible = False
                txtFullName.ReadOnly = True
                chkActive.Enabled = False
                egvTabs.HideTab("tabPassword")
                egvSaveCancel.Visible = False
            Else
                If Key > 0 AndAlso Not usr.CanModify(obj.PermissionId, conn) Then AccessDenied()
                If Key = 0 AndAlso Not usr.CanWrite(obj.PermissionId, conn) Then AccessDenied()
            End If
        End If
    End Sub

#End Region

End Class

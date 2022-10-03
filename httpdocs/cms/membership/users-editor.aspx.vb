Imports EGV
Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_membership_users_editor
    Inherits AuthCMSPageBase

#Region "Properites"

    Public ReadOnly Property Key As Integer
        Get
            If Request.QueryString("id") IsNot Nothing AndAlso Request.QueryString("id") <> String.Empty AndAlso IsNumeric(Request.QueryString("id")) Then Return Request.QueryString("id") Else Return 0
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        Try
            MyConn.Open()
            ddlArea.BindToDataSource(AreaController.GetCollection(MyConn, LanguageId).List, "Title", "Id")
            ddlFamily.BindToDataSource(FamilyController.GetCollection(MyConn, 0, "", False).List, "SchoolId", "Id")
            For Each dr As DataRow In SectionController.GetCollection(MyConn, LanguageId,,, Key = 0).List.Rows
                ddlSection.Items.Add(New ListItem(dr("ClassName") & " - " & dr("Title"), dr("Id")))
            Next
            ddlSection.Attributes.Add("data-currentyear", True)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            egvSaveCancel.IsEditing = Key > 0
            egvSaveCancel.BackPagePath = "users.aspx"
            egvSaveCancel.AddPagePath = "users-editor.aspx"
            egvSaveCancel.EditPagePath = "users-editor.aspx?id={0}"
            hdnLanguageId.Value = LanguageId
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 18, MyConn)
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(Localization.GetResource("Resources.Local.Page.EditTitle"), title), "", Localization.GetResource("Resources.Local.Page.BCEditTitle"), 18)
                Else
                    Master.LoadTitles(Localization.GetResource("Resources.Local.Page.AddTitle"), "", Localization.GetResource("Resources.Local.Page.BCAddTitle"), 18)
                End If
                EGVScriptManager.AddScript(Path.MapCMSScript("local/students-editor"))
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
            e.IsValid = Not StudentController.UsernameExists(txtUsername.Text, Key, MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub valEmail_Validate(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Try
            MyConn.Open()
            e.IsValid = Not StudentController.EmailExists(txtEmail.Text, Key, MyConn)
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
            Dim obj As New Student(Key, conn)
            txtId.Text = obj.Id
            txtUsername.Text = obj.SchoolId
            txtEmail.Text = obj.Email
            chkActive.Checked = obj.IsActive
            txtFullName.Text = obj.FullName
            txtFatherName.Text = obj.FatherName
            txtMotherName.Text = obj.MotherName
            ddlArea.SelectedValue = obj.AreaId
            hdnSelectedArea.Value = obj.AreaId
            ddlFamily.SelectedValue = obj.FamilyId
            hdnSelectedFamily.Value = obj.FamilyId
            ddlSection.SelectedValue = obj.SectionId
            txtRecord.Text = obj.RecordNumber
            txtReligion.Text = obj.Religion
            txtGender.Text = obj.Gender
            txtBirth.Text = obj.BirthInfo
            txtPhoneNumber.Text = obj.PhoneNumber
            txtFatherPhoneNumber.Text = obj.FatherPhoneNumber
            txtMotherPhoneNumber.Text = obj.MotherPhoneNumber
            txtLandlinePhoneNumber.Text = obj.LandlinePhoneNumber
            txtFatherWork.Text = obj.FatherWork
            txtMotherWork.Text = obj.MotherWork
            txtLastLogin.Text = IIf(obj.LastLoginDate = DateTime.MinValue, Localization.GetResource("Resources.Local.NotLoggedIn"), obj.LastLoginDate.ToString("MMMM dd, yyyy @ hh:mm:ss"))
            Return obj.FullName
        Else
            rowPassword.Visible = True
            rowRepeatPassword.Visible = True
            rowLastLogin.Visible = False
            egvTabs.HideTab(tabPassword.Id)
            chkActive.Checked = True
            hdnSelectedFamily.Value = ddlFamily.SelectedValue
            hdnSelectedArea.Value = ddlArea.SelectedValue
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim obj As New Student(Key, conn)
        obj.SchoolId = txtUsername.Text
        obj.Email = txtEmail.Text
        obj.FullName = txtFullName.Text
        obj.FatherName = txtFatherName.Text
        obj.MotherName = txtMotherName.Text
        obj.IsActive = chkActive.Checked
        obj.AreaId = hdnSelectedArea.Value
        obj.FamilyId = hdnSelectedFamily.Value
        obj.SectionId = ddlSection.SelectedValue
        obj.RecordNumber = txtRecord.Text
        obj.Religion = txtReligion.Text
        obj.Gender = txtGender.Text
        obj.BirthInfo = txtBirth.Text
        obj.PhoneNumber = txtPhoneNumber.Text
        obj.FatherPhoneNumber = txtFatherPhoneNumber.Text
        obj.MotherPhoneNumber = txtMotherPhoneNumber.Text
        obj.LandlinePhoneNumber = txtLandlinePhoneNumber.Text
        obj.FatherWork = txtFatherWork.Text
        obj.MotherWork = txtMotherWork.Text
        If Key = 0 Then obj.Password = txtPassword.Text
        If Key > 0 Then obj.ModifiedUser = AuthUser.Id Else obj.CreatedUser = AuthUser.Id
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
        If Key = 0 Then
            Dim acc As New StudentAccount(0, conn)
            acc.StudentId = obj.Id
            acc.PreviousClassId = 0
            acc.CurrentClassId = New Section(obj.SectionId, conn).ClassId
            acc.Transportation = False
            acc.Deposit = 0
            acc.Subscription = 0
            acc.Total = 0
            acc.Discount = 0
            acc.NetTotal = 0
            acc.PaymentsSum = 0
            acc.Balance = 0
            Dim trans2 As SqlTransaction = conn.BeginTransaction()
            Try
                acc.Save(trans2)
                trans2.Commit()
            Catch ex As Exception
                trans2.Rollback()
                Throw ex
            End Try
        End If
    End Sub

    Private Sub ChangePassword(ByVal conn As SqlConnection)
        Dim obj As New Student(Key, conn)
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
                txtFullName.ReadOnly = True
                txtFatherName.ReadOnly = True
                txtMotherName.ReadOnly = True
                ddlFamily.Enabled = False
                hypAddFamily.Enabled = False
                hypAddFamily.CssClass = hypAddFamily.CssClass & " disabled"
                ddlArea.Enabled = False
                hypAddArea.Enabled = False
                hypAddArea.CssClass = hypAddArea.CssClass & " disabled"
                ddlSection.Enabled = False
                txtRecord.ReadOnly = True
                txtReligion.ReadOnly = True
                txtGender.ReadOnly = True
                txtBirth.ReadOnly = True
                txtPhoneNumber.ReadOnly = True
                txtFatherPhoneNumber.ReadOnly = True
                txtMotherPhoneNumber.ReadOnly = True
                txtLandlinePhoneNumber.ReadOnly = True
                txtFatherWork.ReadOnly = True
                txtMotherWork.ReadOnly = True
                chkActive.Enabled = False
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

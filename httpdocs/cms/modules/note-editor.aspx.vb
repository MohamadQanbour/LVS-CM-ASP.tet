Imports EGV
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_modules_note_editor
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
            ddlStudent.BindToDataSource(StudentController.GetCollection(MyConn, 0, String.Empty, True, 0, 0, 0).List, "IdName", "Id")
            ddlNoteType.BindToEnum(Helper.GetEnumType("NoteTypes"), False)
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
            egvSaveCancel.BackPagePath = "notes.aspx"
            egvSaveCancel.AddPagePath = "note-editor.aspx"
            egvSaveCancel.EditPagePath = "note-editor.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 24, MyConn)
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 37)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 37)
                End If
            End If
            EGVScriptManager.AddScript(Path.MapCMSScript("local/notes-editor"))
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
            Dim obj As New Note(Key, conn)
            txtId.Text = obj.Id
            txtSender.Text = New User(obj.SenderId, conn).UserName
            hdnSender.Value = obj.SenderId
            ddlStudent.SelectedValue = obj.StudentId
            hdnSelectedStudent.Value = obj.StudentId
            ddlStudent.Enabled = False
            ddlNoteType.SelectedValue = obj.NoteType
            txtNotedate.Text = obj.NoteDate.ToString("yyyy-MM-dd")
            txtNote.Text = obj.NoteText
            Return obj.Id
        Else
            txtSender.Text = Helper.CMSAuthUser.UserName
            hdnSender.Value = Helper.CMSAuthUser.Id
            hdnSelectedStudent.Value = ddlStudent.SelectedValue
            txtNotedate.Text = Now.ToString("yyyy-MM-dd")
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim obj As New Note(Key, conn)
        obj.SenderId = hdnSender.Value
        obj.StudentId = hdnSelectedStudent.Value
        obj.NoteType = ddlNoteType.SelectedValue
        obj.NoteDate = txtNotedate.Text
        obj.NoteText = txtNote.Text
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            obj.Save(trans)
            If Key = 0 Then egvSaveCancel.NewId = obj.Id
            trans.Commit()
            Master.Notifier.Success(String.Format(Localization.GetResource("Resources.Local.SaveSuccess"), obj.Id))
        Catch ex As Exception
            trans.Rollback()
            Throw ex
        End Try
        If Key = 0 Then
            Dim objStudent As New Student(obj.StudentId, conn)
            Dim objFamily As New Family(objStudent.FamilyId, conn)
            Dim lst As New List(Of Structures.ShortMemberObject)
            lst.Add(New Structures.ShortMemberObject() With {.MemberId = objStudent.Id, .MemberType = MembershipTypes.Student})
            lst.Add(New Structures.ShortMemberObject() With {.MemberId = objFamily.Id, .MemberType = MembershipTypes.Family})
            WebRequests.NotificationRequest.SendNoteNotification(lst, obj.NoteText, obj.NoteType = NoteTypes.Negative, conn)
        End If
    End Sub

#End Region

#Region "Public Methods"

    Public Overrides Sub ProcessPermissions(usr As User, Optional pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        MyBase.ProcessPermissions(usr, pid, conn)
        Dim obj As New CMSMenu(pid, conn)
        If Key > 0 AndAlso Not usr.CanModify(obj.PermissionId, conn) Then AccessDenied()
        If Key = 0 AndAlso Not usr.CanWrite(obj.PermissionId, conn) Then AccessDenied()
    End Sub

#End Region

End Class

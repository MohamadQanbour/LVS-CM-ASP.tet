Imports EGV
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_modules_materials_editor
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
            ddlClass.BindToDataSource(StudyClassController.GetCollection(MyConn, LanguageId).List, "Title", "Id", True)
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
            egvSaveCancel.BackPagePath = "materials.aspx"
            egvSaveCancel.AddPagePath = "materials-editor.aspx"
            egvSaveCancel.EditPagePath = "materials-editor.aspx?id={0}"
            hdnLanguageId.Value = LanguageId
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 21, MyConn)
                hdnMaterialId.Value = Key
                Dim classId As String = ddlClass.SelectedValue
                If hdnSelectedClass.Value <> String.Empty Then
                    classId = hdnSelectedClass.Value
                End If
                If classId <> String.Empty Then
                    For Each t As ExamTemplateObject In StudyClassController.GetTemplateObjects(classId, LanguageId, MyConn)
                        ddlTemplate.Items.Add(New ListItem(t.Title, t.Id))
                    Next
                End If
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 21)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 21)
                End If
            End If
            EGVScriptManager.AddScript(Path.MapCMSScript("local/materials-editor") & "?v=2.0")
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

    Protected Sub reqTemplate_ServerValidate(source As Object, args As ServerValidateEventArgs)
        args.IsValid = hdnSelectedTemplate.Value <> String.Empty
    End Sub

#End Region

#Region "Private Methods"

    Private Function LoadData(ByVal conn As SqlConnection) As String
        If Key > 0 Then
            Dim obj As New Material(Key, conn, LanguageId)
            txtId.Text = obj.Id
            txtTitle.Text = obj.Title
            txtCode.Text = obj.Code
            ddlClass.SelectedValue = obj.ClassId
            hdnSelectedClass.Value = obj.ClassId
            txtTotalScore.Text = obj.MaxMark
            ddlTemplate.DataSource = StudyClassController.GetTemplateObjects(obj.ClassId, LanguageId, conn)
            ddlTemplate.DataBind()
            ddlTemplate.SelectedValue = obj.ExamTemplateId
            hdnSelectedTemplate.Value = obj.ExamTemplateId
            hdnItems.Value = Helper.JSSerialize(obj.Items)
            Return obj.Title
        Else
            ddlClass.SelectedIndex = 0
            hdnSelectedClass.Value = ddlClass.SelectedItem.Value
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        If hdnSelectedClass.Value = 0 Then
            Throw New Exception(Localization.GetResource("Resources.Local.MustSelectClass"))
        Else
            Dim obj As New Material(Key, conn, LanguageId)
            obj.Title = txtTitle.Text
            obj.Code = txtCode.Text
            obj.ClassId = hdnSelectedClass.Value
            obj.MaxMark = txtTotalScore.Text
            obj.ExamTemplateId = hdnSelectedTemplate.Value
            obj.Items = Helper.JSDeserialize(Of List(Of MaterialExamTemplateItem))(hdnItems.Value)
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

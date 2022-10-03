Imports EGV
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_setup_settings_editor
    Inherits AuthCMSPageBase

#Region "Properites"

    Public ReadOnly Property Key As String
        Get
            If Request.QueryString("id") IsNot Nothing AndAlso Request.QueryString("id") <> String.Empty Then Return Request.QueryString("id") Else Return String.Empty
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            egvSaveCancel.IsEditing = Key <> String.Empty
            egvSaveCancel.BackPagePath = Path.MapCMSFile(New CMSMenu(4, MyConn).PagePath)
            egvSaveCancel.AddPagePath = "settings-editor.aspx"
            egvSaveCancel.EditPagePath = "settings-editor.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 4, MyConn)
                If Key <> String.Empty Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), Key), "", GetLocalResourceObject("Page.BCEditTitle"), 4)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 4)
                End If
                ddlType.BindToEnum(GetType(ESDataTypes), False)
                ddlList.BindToDataSource(EGVListController.List(MyConn).List, "Name", "Id", True)
                LoadData(MyConn)
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
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
                hasError = True
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub LoadData(ByVal conn As SqlConnection)
        If Key <> String.Empty Then
            Dim obj As SettingObject = SettingController.GetSetting(Key, conn)
            txtKey.Text = obj.SettingKey
            txtTitle.Text = obj.Title
            ddlType.SelectedValue = obj.SettingType
            ddlList.SelectedValue = obj.SettingSourceListId
        End If
    End Sub

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim obj As New SettingObject()
        If Key <> String.Empty Then obj = SettingController.GetSetting(Key, conn)
        obj.SettingKey = txtKey.Text
        obj.Title = txtTitle.Text
        obj.SettingType = ddlType.SelectedValue
        obj.SettingSourceListId = ddlList.SelectedValue
        If Key = String.Empty Then obj.SettingValue = String.Empty
        obj.SettingSourceListId = 0
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            If Key <> String.Empty Then SettingController.UpdateSetting(obj, trans) Else SettingController.AddSetting(obj, trans)
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
        If Not usr.IsSuperAdmin Then AccessDenied()
    End Sub

#End Region

End Class

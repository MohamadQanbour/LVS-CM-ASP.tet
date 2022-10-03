Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_setup_emails_editor
    Inherits AuthCMSPageBase

#Region "Properites"

    Public ReadOnly Property Key As Integer
        Get
            If Request.QueryString("id") IsNot Nothing AndAlso Request.QueryString("id") <> String.Empty AndAlso IsNumeric(Request.QueryString("id")) Then Return Request.QueryString("id") Else Return 0
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        ddlType.BindToEnum(Helper.GetEnumType("EmailTypes"), False)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            egvSaveCancel.IsEditing = Key > 0
            egvSaveCancel.BackPagePath = "emails.aspx"
            egvSaveCancel.AddPagePath = "emails-editor.aspx"
            egvSaveCancel.EditPagePath = "emails-editor.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 14, MyConn)
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 14)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 14)
                End If
                If Key > 0 Then
                    egvGrid.AddCondition("KeyId = " & Key)
                    egvGrid.BindGrid(MyConn)
                    EGVScriptManager.AddScript(Path.MapCMSScript("local/emails-editor"))
                Else
                    egvGrid.Visible = False
                End If
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub grid_DataSource(ByVal sender As Object, ByVal e As EventArgs) Handles egvGrid.GridNeedDataSource
        Try
            MyConn.Open()
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub grid_ToolbarClick(ByVal cmd As String) Handles egvGrid.ToolbarButtonClick
        Try
            MyConn.Open()
            Dim ids = egvGrid.GetSelectedIds()
            Select Case cmd.ToLower()
                Case "activate"
                    For Each id As Integer In ids
                        EmailController.ToggleState(id, True, MyConn)
                    Next
                    Master.Notifier.Success(Localization.GetResource("Resources.Local.Activate"))
                Case "deactivate"
                    For Each id As Integer In ids
                        EmailController.ToggleState(id, False, MyConn)
                    Next
                    Master.Notifier.Success(Localization.GetResource("Resources.Local.Deactivate"))
                Case "delete"
                    Dim completed As Boolean = True
                    For Each id As Integer In ids
                        completed = completed And EmailController.Delete(id, MyConn)
                    Next
                    If completed Then Master.Notifier.Success(Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(Localization.GetResource("Resources.Local.delete.Warning"))
            End Select
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub grid_DeleteRow(ByVal id As Integer) Handles egvGrid.RowDelete
        Try
            MyConn.Open()
            Dim completed As Boolean = EmailController.Delete(id, MyConn)
            If completed Then Master.Notifier.Success(Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(Localization.GetResource("Resources.Local.delete.Warning"))
            egvGrid.BindGrid(MyConn)
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

    Protected Sub lnkSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Try
            MyConn.Open()
            Dim lst = egvGrid.GetEditableColumns()
            Dim emails = (From l In lst Group By colKey = l.ColumnKey Into grp = Group, Count())
            For Each item In emails.ToList()
                Dim id As Integer = item.colKey
                Dim email As String = (From g In item.grp Where g.ColumnName = "EmailAddress" Select g.ColumnValue).FirstOrDefault()
                Dim display As String = (From g In item.grp Where g.ColumnName = "DisplayName" Select g.ColumnValue).FirstOrDefault()
                Dim type As Integer = (From g In item.grp Where g.ColumnName = "Type" Select g.ColumnValue).FirstOrDefault()
                Dim obj As New Email()
                obj.Id = id
                obj.EmailAddress = email
                obj.DisplayName = display
                obj.Type = type
                obj.KeyId = Key
                Dim trans As SqlTransaction = MyConn.BeginTransaction()
                Try
                    EmailController.Update(obj, trans)
                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw ex
                End Try
            Next
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkAdd.Click
        If Page.IsValid Then
            Try
                MyConn.Open()
                Dim email As String = txtEmailAddress.Text
                Dim name As String = txtDisplayName.Text
                Dim type As Integer = CInt(ddlType.SelectedValue)
                Dim obj As New Email()
                obj.KeyId = Key
                obj.EmailAddress = email
                obj.DisplayName = name
                obj.Type = type
                Dim trans As SqlTransaction = MyConn.BeginTransaction()
                Try
                    EmailController.Add(obj, trans)
                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw ex
                End Try
                egvGrid.BindGrid(MyConn)
            Catch ex As Exception
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
            Dim obj As New EmailKey(Key, conn)
            txtId.Text = obj.Id
            txtTitle.Text = obj.Title
            Return obj.Title
        Else
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            If Key = 0 Then
                Dim id As Integer = EmailKeyController.Add(txtTitle.Text, trans)
                egvSaveCancel.NewId = id
            Else
                EmailKeyController.Update(Key, txtTitle.Text, trans)
            End If
            trans.Commit()
            Master.Notifier.Success(String.Format(Localization.GetResource("Resources.Local.SaveSuccess"), txtTitle.Text))
        Catch ex As Exception
            trans.Rollback()
            Throw ex
        End Try
    End Sub

#End Region

End Class

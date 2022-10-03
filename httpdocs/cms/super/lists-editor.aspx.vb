Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business

Partial Class cms_super_lists_editor
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
            egvSaveCancel.BackPagePath = "lists.aspx"
            egvSaveCancel.AddPagePath = "lists-editor.aspx"
            egvSaveCancel.EditPagePath = "lists-editor.aspx?id={0}"
            If Key > 0 Then
                Dim items As DataTable = EGVListController.GetListItems(Key, MyConn)
                Dim texts As New List(Of String)
                Dim values As New List(Of String)
                For Each dr As DataRow In items.Rows
                    texts.Add(Helper.GetSafeDBValue(dr("ItemText")))
                    values.Add(Helper.GetSafeDBValue(dr("ItemValue")))
                Next
                hdnItemText.Value = Helper.JSSerialize(texts)
                hdnItemValue.Value = Helper.JSSerialize(values)
            End If
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 13, MyConn)
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 13)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 13)
                End If
                If Key > 0 Then
                    egvGrid.AddCondition("ListId = " & Key)
                    egvGrid.BindGrid(MyConn)
                    EGVScriptManager.AddScript(Path.MapCMSScript("local/lists-editor"))
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
                Case "publish"
                    For Each id As Integer In ids
                        EGVListItemController.ToggleState(id, True, MyConn)
                    Next
                    Master.Notifier.Success(Localization.GetResource("Resources.Local.publish.Success"))
                Case "unpublish"
                    For Each id As Integer In ids
                        EGVListItemController.ToggleState(id, False, MyConn)
                    Next
                    Master.Notifier.Success(Localization.GetResource("Resources.Local.unpublish.Success"))
                Case "delete"
                    Dim completed As Boolean = True
                    For Each id As Integer In ids
                        completed = completed And EGVListItemController.Delete(id, Key, EGVListItemController.GetListValue(id, MyConn), MyConn)
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
            Dim completed As Boolean = EGVListItemController.Delete(id, Key, EGVListItemController.GetListValue(id, MyConn), MyConn)
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
            Dim textValuePairs = (From l In lst Group By colKey = l.ColumnKey Into grp = Group, Count())
            For Each item In textValuePairs.ToList()
                Dim id As Integer = item.colKey
                Dim text As String = (From g In item.grp Where g.ColumnName = "ItemText" Select g.ColumnValue).FirstOrDefault()
                Dim value As String = (From g In item.grp Where g.ColumnName = "ItemValue" Select g.ColumnValue).FirstOrDefault()
                Dim obj As New EGVListItem(id, MyConn)
                obj.ItemText = text
                obj.ItemValue = value
                Dim trans As SqlTransaction = MyConn.BeginTransaction()
                Try
                    obj.Save(trans)
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
                Dim text As String = txtItemText.Text
                Dim value As String = txtItemValue.Text
                Dim obj As New EGVListItem(0, MyConn)
                obj.ListId = Key
                obj.ItemText = text
                obj.ItemValue = value
                obj.IsPublished = True
                Dim trans As SqlTransaction = MyConn.BeginTransaction()
                Try
                    obj.Save(trans)
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
            Dim obj As New EGVList(Key, conn)
            txtId.Text = obj.Id
            txtName.Text = obj.Name
            chkPublished.Checked = obj.IsPublished
            Return obj.Name
        Else
            chkPublished.Checked = True
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim obj As New EGVList(Key, conn)
        obj.Name = txtName.Text
        obj.IsPublished = chkPublished.Checked
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            obj.Save(trans)
            If Key = 0 Then egvSaveCancel.NewId = obj.Id
            trans.Commit()
            Master.Notifier.Success(String.Format(Localization.GetResource("Resources.Local.SaveSuccess"), obj.Name))
        Catch ex As Exception
            trans.Rollback()
            Throw ex
        End Try
    End Sub

#End Region

End Class

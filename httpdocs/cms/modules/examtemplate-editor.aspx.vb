Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_modules_examtemplate_editor
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
        ddlItemType.BindToEnum(Helper.GetEnumType("ExamItemTypes"), False)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            egvSaveCancel.IsEditing = Key > 0
            egvSaveCancel.BackPagePath = "examtemplate.aspx"
            egvSaveCancel.AddPagePath = "examtemplate-editor.aspx"
            egvSaveCancel.EditPagePath = "examtemplate-editor.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 25, MyConn)
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 25)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 25)
                End If
                BindGrid(MyConn)
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
            BindGrid(MyConn)
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
                Case "delete"
                    Dim completed As Boolean = True
                    For Each id As Integer In ids
                        completed = completed And ExamTemplateItemController.Delete(id, MyConn)
                    Next
                    If completed Then Master.Notifier.Success(Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(Localization.GetResource("Resources.Local.delete.Warning"))
            End Select
            BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub grid_DeleteRow(ByVal id As Integer) Handles egvGrid.RowDelete
        Try
            MyConn.Open()
            Dim completed As Boolean = ExamTemplateItemController.Delete(id, MyConn)
            If completed Then Master.Notifier.Success(Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(Localization.GetResource("Resources.Local.delete.Warning"))
            BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub saveClick(ByVal sender As Object, ByVal e As EventArgs, ByRef hasError As Boolean) Handles egvSaveCancel.SaveClick
        If Page.IsValid Then
            Dim id As Integer = 0
            Try
                MyConn.Open()
                id = SaveData(MyConn)
            Catch ex As Exception
                hasError = True
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
            If Not hasError AndAlso id > 0 And Key = 0 Then Response.Redirect("examtemplate-editor.aspx?id=" & id)
        End If
    End Sub

    Protected Sub lnkSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Try
            MyConn.Open()
            Dim lst = egvGrid.GetEditableColumns()
            Dim textValuePairs = (From l In lst Group By colKey = l.ColumnKey Into grp = Group, Count())
            For Each item In textValuePairs.ToList()
                Dim id As Integer = item.colKey
                Dim title As String = (From g In item.grp Where g.ColumnName = "Title" Select g.ColumnValue).FirstOrDefault()
                Dim type As ExamItemTypes = (From g In item.grp Where g.ColumnName = "Type" Select g.ColumnValue).FirstOrDefault()
                Dim obj As New ExamTemplateItem(id, LanguageId, MyConn)
                obj.Title = title
                obj.Type = type
                Dim trans As SqlTransaction = MyConn.BeginTransaction()
                Try
                    obj.Save(trans)
                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw ex
                End Try
            Next
            BindGrid(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkUpdateRelations_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSaveRelations.Click
        Try
            MyConn.Open()
            Dim id As Integer = hdnEditId.Value
            Dim lst As List(Of Integer) = Helper.JSDeserialize(Of List(Of Integer))(hdnSelectedEditRelations.Value)
            Dim trans As SqlTransaction = MyConn.BeginTransaction()
            Try
                ExamTemplateItemController.DeleteRelatedIds(id, trans)
                ExamTemplateItemController.AddRelatedIds(id, lst, trans)
                trans.Commit()
            Catch ex As Exception
                trans.Rollback()
                Throw ex
            End Try
            hdnEditId.Value = 0
            hdnSelectedEditRelations.Value = "[]"
            Dim ids = ExamTemplateItemController.GetTemplateItems(Key, LanguageId, MyConn, True)
            rptChecks.DataSource = ids
            rptChecks.DataBind()
            rptEditChecks.DataSource = ids
            rptEditChecks.DataBind()
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
                Dim text As String = txtItemTitle.Text
                Dim type As Integer = ddlItemType.SelectedValue
                Dim related As List(Of Integer) = Helper.JSDeserialize(Of List(Of Integer))(hdnSelectedRelations.Value)
                Dim obj As New ExamTemplateItem(0, LanguageId, MyConn)
                obj.TemplateId = Key
                obj.Title = text
                obj.Type = type
                If related IsNot Nothing AndAlso related.Count > 0 Then
                    obj.RelatedIds = related
                End If
                Dim trans As SqlTransaction = MyConn.BeginTransaction()
                Try
                    obj.Save(trans)
                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw ex
                End Try
                BindGrid(MyConn)
                txtItemTitle.Text = String.Empty
                ddlItemType.SelectedIndex = 0
                hdnSelectedRelations.Value = "[]"
                Dim ids = ExamTemplateItemController.GetTemplateItems(Key, LanguageId, MyConn, False)
                rptChecks.DataSource = ids
                rptChecks.DataBind()
                rptEditChecks.DataSource = ids
                rptEditChecks.DataBind()
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub BindGrid(ByVal conn As SqlConnection)
        If Key > 0 AndAlso UserCanEditItems(conn) Then
            egvGrid.AddCondition("I.TemplateId = " & Key)
            egvGrid.BindGrid(conn)
            EGVScriptManager.AddScript(Path.MapCMSScript("local/examtemplate-editor"))
            Dim ids = ExamTemplateItemController.GetTemplateItems(Key, LanguageId, conn, False)
            rptChecks.DataSource = ids
            rptChecks.DataBind()
            rptEditChecks.DataSource = ids
            rptEditChecks.DataBind()
        Else
            egvGrid.Visible = False
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Function LoadData(ByVal conn As SqlConnection) As String
        If Key > 0 Then
            Dim obj As New ExamTemplate(Key, LanguageId, conn)
            txtId.Text = obj.Id
            txtTitle.Text = obj.Title
            txtMaxMark.Text = obj.MaxMark
            egvAudit.BusinessObject = obj
            'Dim ids = ExamTemplateItemController.GetTemplateItems(Key, LanguageId, MyConn, True)
            'rptChecks.DataSource = ids
            'rptChecks.DataBind()
            'rptEditChecks.DataSource = ids
            'rptEditChecks.DataBind()
            Return obj.Title
        Else
            Return String.Empty
        End If
    End Function

    Private Function SaveData(ByVal conn As SqlConnection) As Integer
        Dim obj As New ExamTemplate(Key, LanguageId, conn)
        obj.Title = txtTitle.Text
        obj.MaxMark = txtMaxMark.Text
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
        Return obj.Id
    End Function

    Private Function UserCanEditItems(ByVal conn As SqlConnection) As Boolean
        Return AuthUser.CanRead(21, conn) AndAlso AuthUser.CanWrite(21, conn) AndAlso AuthUser.CanModify(21, conn)
    End Function

#End Region

End Class

Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business

Partial Class cms_membership_payments2_editor
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
            egvSaveCancel.BackPagePath = "payments2.aspx"
            egvSaveCancel.AddPagePath = "payments2-editor.aspx"
            egvSaveCancel.EditPagePath = "payment2-editor.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 38, MyConn)
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 23)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 23)
                End If
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub Unnamed_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Empty
        Try
            MyConn.Open()
            Dim id As Integer = DirectCast(sender, LinkButton).Attributes("data-id")
            Dim studentId As Integer = StudentAccount2Controller.ReadField(id, "StudentId", EGV.Enums.ValueTypes.TypeInteger, MyConn)
            StudentAccount2Controller.Delete(id, MyConn)
            Dim lastEntryId As Integer = StudentAccount2Controller.GetLastEntry(studentId, MyConn)
            If lastEntryId > 0 Then url = "payments2-editor.aspx?id=" & lastEntryId Else url = "payments2.aspx"
        Catch ex As Exception
            Throw ex
        Finally
            MyConn.Close()
        End Try
        If url <> String.Empty Then Response.Redirect(url)
    End Sub

    Protected Sub rptHistory_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Dim btnDelete As LinkButton = e.Item.FindControl("btnDelete")
        If btnDelete IsNot Nothing Then
            btnDelete.OnClientClick = "return confirm('" & Localization.GetResource("Resources.Local.DeleteConfirmation") & "');"
            btnDelete.Visible = AuthUser.CanDelete(23, MyConn)
        End If
    End Sub

    Protected Sub lnkDeleteAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkDeleteAll.Click
        Dim url As String = String.Empty
        Try
            MyConn.Open()
            Dim studentId As Integer = StudentAccount2Controller.ReadField(Key, "StudentId", EGV.Enums.ValueTypes.TypeInteger, MyConn)
            StudentAccount2Controller.DeleteAll(studentId, MyConn)
            url = "payments2.aspx"
        Catch ex As Exception
            Throw ex
        Finally
            MyConn.Close()
        End Try
        If url <> String.Empty Then Response.Redirect(url)
    End Sub

#End Region

#Region "Private Methods"

    Private Function LoadData(ByVal conn As SqlConnection) As String
        If Key > 0 Then
            Dim obj As New StudentAccount2(Key, conn)
            Dim objStudent As New Student(obj.StudentId, conn)
            Dim objClass As New StudyClass(obj.ClassId, conn, LanguageId)
            txtStudent.Text = objStudent.FullName
            hypStudent.NavigateUrl = Path.MapCMSFile("membership/users-editor.aspx?id=" & obj.StudentId)
            If Not AuthUser.CanRead(8, conn) Then
                pnlStudentIG.Attributes.Remove("class")
                pnlStudentIGB.Visible = False
            End If
            txtClass.Text = objClass.Title
            hypClass.NavigateUrl = Path.MapCMSFile("modules/classes-editor.aspx?id=" & obj.ClassId)
            If Not AuthUser.CanRead(11, conn) Then
                pnlClassIG.Attributes.Remove("class")
                pnlClassIGB.Visible = False
            End If
            txtRequested.Text = obj.RequestedAmount.ToString("N0")
            txtBalance.Text = obj.Balance.ToString("N0")
            txtLastUpdate.Text = obj.LastUpdate.ToString("MMMM d, yyyy")
            txtLastUpdateUser.Text = obj.LastUpdateUserName
            Dim lst As New List(Of Object)
            Using dt = StudentAccount2Controller.GetCollection(conn, obj.StudentId).Tables(0)
                For Each dr In dt.Rows
                    lst.Add(New With {
                        .LastUpdate = CDate(Helper.GetSafeDBValue(dr("LastUpdate"), EGV.Enums.ValueTypes.TypeDateTime)).ToString("MMMM d, yyyy"),
                        .RequestedAmount = CDec(Helper.GetSafeDBValue(dr("RequestedAmount"), EGV.Enums.ValueTypes.TypeDecimal)).ToString("N0"),
                        .Balance = CDec(Helper.GetSafeDBValue(dr("Balance"), EGV.Enums.ValueTypes.TypeDecimal)).ToString("N0"),
                        .PaidAmount = CDec(Helper.GetSafeDBValue(dr("PaidAmount"), EGV.Enums.ValueTypes.TypeDecimal)).ToString("N0"),
                        .Payment = 0D,
                        .FullName = Helper.GetSafeDBValue(dr("FullName")),
                        .Id = Helper.GetSafeDBValue(dr("Id"), EGV.Enums.ValueTypes.TypeInteger),
                        .ClassId = Helper.GetSafeDBValue(dr("ClassId"), EGV.Enums.ValueTypes.TypeInteger)
                    })
                Next
            End Using
            For i As Integer = 0 To lst.Count - 2
                lst(i).Payment = lst(i).PaidAmount - IIf(lst(i + 1).ClassId = lst(i).ClassId, lst(i + 1).PaidAmount, 0)
            Next
            If lst.Count > 0 Then lst(lst.Count - 1).Payment = lst(lst.Count - 1).PaidAmount
            Dim classId As Integer = lst(0).ClassId
            lst = lst.Where(Function(x) x.ClassId = classId).ToList()
            rptHistory.DataSource = lst
            rptHistory.DataBind()
            Return objStudent.FullName
            Else
                Response.Redirect("payments2.aspx")
            Return String.Empty
        End If
    End Function

#End Region

End Class

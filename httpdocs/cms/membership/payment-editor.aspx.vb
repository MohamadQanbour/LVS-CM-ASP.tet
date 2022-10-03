Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business

Partial Class cms_membership_payment_editor
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
            Dim ds = StudyClassController.GetCollection(MyConn, LanguageId, 0, String.Empty).List
            ddlPreviousClass.BindToDataSource(ds, "Title", "Id", True)
            ddlCurrentClass.BindToDataSource(ds, "Title", "Id", True)
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
            egvSaveCancel.BackPagePath = "payments.aspx"
            egvSaveCancel.AddPagePath = "payment-editor.aspx"
            egvSaveCancel.EditPagePath = "payment-editor.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 38, MyConn)
                Dim title As String = LoadData(MyConn)
                If Key > 0 Then
                    Master.LoadTitles(String.Format(GetLocalResourceObject("Page.EditTitle"), title), "", GetLocalResourceObject("Page.BCEditTitle"), 23)
                Else
                    Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 23)
                End If
                EGVScriptManager.AddScript(Path.MapCMSScript("local/payments"), False, "1.3")
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim id As Integer = CType(sender, LinkButton).CommandArgument
        Dim completed As Boolean = False
        Try
            MyConn.Open()
            completed = StudentPaymentController.Delete(id, MyConn)
            Dim obj As New StudentAccount(Key, MyConn)
            obj.PaymentsSum = StudentPaymentController.GetStudentPayments(Key, MyConn)
            obj.Balance = obj.NetTotal - obj.PaymentsSum
            Dim trans As SqlTransaction = MyConn.BeginTransaction()
            Try
                obj.Save(trans)
                trans.Commit()
            Catch ex As Exception
                trans.Rollback()
                Throw ex
            End Try
        Catch ex As Exception
            completed = False
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
        If completed Then Response.Redirect(Request.RawUrl)
    End Sub

    Protected Sub saveClick(ByVal sender As Object, ByVal e As EventArgs, ByRef hasError As Boolean) Handles egvSaveCancel.SaveClick
        Try
            MyConn.Open()
            If Key = 0 AndAlso StudentAccountController.StudentIdExists(hdnStudentId.Value, MyConn) Then Throw New Exception(Localization.GetResource("Resources.Local.StudentAccountExists"))
            SaveData(MyConn)
        Catch ex As Exception
            hasError = True
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSavePayment.Click
        If Page.IsValid Then
            Dim completed As Boolean = False
            Try
                MyConn.Open()
                Dim targetId As Integer = 0
                If hdnId.Value <> String.Empty AndAlso IsNumeric(hdnId.Value) Then targetId = hdnId.Value
                Dim obj As New StudentPayment(targetId, MyConn)
                obj.StudentId = hdnStudentId.Value
                obj.PaymentNumber = txtPaymentNumber.Text
                obj.PaymentAmount = txtPaymentAmount.Text
                obj.PaymentDate = Helper.ParseDate(txtPaymentDate.Text)
                obj.PaymentNote = txtPaymentNote.Text
                Dim trans As SqlTransaction = MyConn.BeginTransaction()
                Try
                    obj.Save(trans)
                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw ex
                End Try
                Dim objAcc As New StudentAccount(Key, MyConn)
                objAcc.PaymentsSum = StudentPaymentController.GetStudentPayments(Key, MyConn)
                objAcc.Balance = objAcc.NetTotal - objAcc.PaymentsSum
                Dim trans2 As SqlTransaction = MyConn.BeginTransaction()
                Try
                    objAcc.Save(trans2)
                    trans2.Commit()
                    completed = True
                Catch ex As Exception
                    trans2.Rollback()
                    Throw ex
                End Try
                If targetId = 0 Then WebRequests.NotificationRequest.SendPaymentNotification(objAcc.StudentObject.FamilyId, objAcc.StudentId, obj.PaymentAmount, MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
            If completed Then Response.Redirect(Request.RawUrl)
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Function LoadData(ByVal conn As SqlConnection) As String
        If Key > 0 Then
            egvBox.Visible = True
            Dim obj As New StudentAccount(Key, conn)
            rptPayments.DataSource = StudentPaymentController.GetCollection(obj.StudentId, conn).List
            rptPayments.DataBind()
            txtStudent.Text = obj.StudentObject.FullName
            hdnStudentId.Value = obj.StudentId
            ddlStudent.Visible = False
            reqStudent.Visible = False
            ddlPreviousClass.SelectedValue = obj.PreviousClassId
            ddlCurrentClass.SelectedValue = obj.CurrentClassId
            chkTransportation.Checked = obj.Transportation
            txtDeposit.Text = obj.Deposit.ToString("0,0")
            txtSubscription.Text = obj.Subscription.ToString("0,0")
            txtTotal.Text = obj.Total.ToString("0,0")
            txtDiscount.Text = obj.Discount.ToString("0,0")
            txtNetTotal.Text = obj.NetTotal.ToString("0,0")
            txtPaymentsSum.Text = obj.PaymentsSum.ToString("0,0")
            txtBalance.Text = obj.Balance.ToString("0,0")
            txtTransportationValue.Text = obj.TransportationValue.ToString("0,0")
            Return obj.StudentObject.FullName
        Else
            egvBox.Visible = False
            ddlStudent.Visible = True
            reqStudent.Visible = True
            txtStudent.Visible = False
            txtDeposit.Text = 0
            txtSubscription.Text = 0
            txtTotal.Text = 0
            txtDiscount.Text = 0
            txtNetTotal.Text = 0
            txtPaymentsSum.Text = 0
            txtBalance.Text = 0
            txtTransportationValue.Text = 0
            Return String.Empty
        End If
    End Function

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim obj As New StudentAccount(Key, conn)
        If Key = 0 Then obj.StudentId = hdnStudent.Value
        obj.PreviousClassId = ddlPreviousClass.SelectedValue
        obj.CurrentClassId = ddlCurrentClass.SelectedValue
        obj.Transportation = chkTransportation.Checked
        obj.TransportationValue = txtTransportationValue.Text
        obj.Deposit = txtDeposit.Text
        obj.Subscription = txtSubscription.Text
        obj.Total = txtTotal.Text
        obj.Discount = txtDiscount.Text
        obj.NetTotal = obj.Deposit + obj.Total - obj.Discount
        If Key = 0 Then obj.PaymentsSum = 0 Else obj.PaymentsSum = StudentPaymentController.GetStudentPayments(obj.StudentId, conn)
        obj.Balance = obj.NetTotal - obj.PaymentsSum
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            obj.Save(trans)
            If Key = 0 Then egvSaveCancel.NewId = obj.Id
            trans.Commit()
            Master.Notifier.Success(String.Format(Localization.GetResource("Resources.Local.SaveSuccess"), obj.StudentObject.FullName))
        Catch ex As Exception
            trans.Rollback()
            Throw ex
        End Try
    End Sub

#End Region

End Class

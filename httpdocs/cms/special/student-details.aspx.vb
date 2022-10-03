Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Business
Imports EGV.Utils

Partial Class cms_special_student_details
            Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If AuthUser IsNot Nothing Then
            hdnUserId.Value = AuthUser.Id

            If Not Page.IsPostBack Then
                EGVScriptManager.AddScript(Path.MapCMSScript("local/student-details") & "?v=3.0")
                Try
                    MyConn.Open()
                    ProcessPermissions(AuthUser, PageId, MyConn)
                Catch ex As Exception
                    ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
                Finally
                    MyConn.Close()
                End Try
            End If
        End If
    End Sub

    Protected Sub lnkLoad_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkLoad.Click
        If hdnStudent.Value <> String.Empty AndAlso CInt(hdnStudent.Value) > 0 Then
            Try
                MyConn.Open()
                Dim obj As New Student(hdnStudent.Value, MyConn)
                litStudentName.Text = obj.SchoolId & " - " & obj.FullName
                egvTabs.Title = obj.FullName & " - " & egvTabs.Title
                txtStudent.Text = obj.FullName
                Dim objSection As New Section(obj.SectionId, MyConn, LanguageId)
                Dim objClass As New StudyClass(objSection.ClassId, MyConn, LanguageId)
                txtClass.Text = objClass.Title & " - " & objSection.Title
                txtFatherName.Text = obj.FatherName
                txtMotherName.Text = obj.MotherName
                txtFatherWork.Text = obj.FatherWork
                txtMotherWork.Text = obj.MotherWork
                hypStudentProfile.NavigateUrl = Path.MapCMSFile("membership/users-editor.aspx?id=" & obj.Id)
                hypFamilyProfile.NavigateUrl = Path.MapCMSFile("membership/family-editor.aspx?id=" & obj.FamilyId)
                Dim studentMembershipPermission As Integer = 8
                Dim familyMembershipPermission As Integer = 7
                Dim canViewStudent As Boolean = AuthUser.CanRead(studentMembershipPermission, MyConn)
                Dim canViewFamily As Boolean = AuthUser.CanRead(familyMembershipPermission, MyConn)
                hypFamilyProfile.Visible = canViewFamily
                hypStudentProfile.Visible = canViewStudent
                rowFullProfile.Visible = canViewStudent Or canViewFamily
                Dim siblings = StudentController.GetCollection(MyConn, 0, String.Empty, True, obj.FamilyId, 0, 0, obj.Id)
                If siblings.Count > 0 Then
                    rptSiblings.DataSource = siblings.List
                    rptSiblings.DataBind()
                Else
                    rptSiblings.DataSource = siblings.List
                    rptSiblings.DataBind()
                End If
                rptNotes.DataSource = NoteController.GetStudentNotes(obj.Id, MyConn).List
                rptNotes.DataBind()
                Dim objAccount = StudentAccountController.GetByStudentId(obj.Id, MyConn)
                If objAccount IsNot Nothing Then
                    txtBalance.Text = objAccount.Balance.ToString("0,0")
                    txtPayments.Text = objAccount.PaymentsSum.ToString("0,0")
                End If
                rptPayments.DataSource = StudentPaymentController.GetCollection(obj.Id, MyConn).List
                rptPayments.DataBind()
                txtNoteDate.Text = Now.ToString("yyyy-MM-dd")
                rptInternalNotes.DataSource = StudentNoteController.GetCollection(MyConn, obj.Id).List
                rptInternalNotes.DataBind()
                Dim dt As DataTable
                dt = StudentController.getSYear(MyConn, SID:=obj.Id).List
                For Each dr As DataRow In dt.Rows
                    Dim dup As String = Convert.ToString(dr("SYear"))
                    If (dt.Select(String.Format("SYear='{0}'", dup)).Count > 1) Then
                        dr.Delete()
                    End If
                Next
                ddlYear.DataSource = dt
                ddlYear.DataBind()
                ddlYear.DataTextField = "SYear"
                ddlYear.DataValueField = "SYear"
                ddlYear.DataBind()
                ddlYear.Items.Insert(0, New ListItem("----- Select Year ----", "----- Select Year ----"))
                pnlResults.Visible = True
                ifProfile.Visible = True
                ifAccount.Visible = True
                ifInternalNotes.Visible = True
                ifSort.Visible = True
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

#End Region

#Region "Public Methods"

    Public Overrides Sub ProcessPermissions(usr As User, Optional pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        MyBase.ProcessPermissions(usr, pid, conn)
        Dim obj As New CMSMenu(pid, conn)
        Dim canPublish As Boolean = usr.CanPublish(obj.PermissionId, conn)
        Dim canDelete As Boolean = usr.CanDelete(obj.PermissionId, conn)
        Dim canModify As Boolean = usr.CanModify(obj.PermissionId, conn)
        Dim canWrite As Boolean = usr.CanWrite(obj.PermissionId, conn)
    End Sub

    'Protected Sub Selected_Year(ByVal sender As Object, ByVal e As EventArgs) Handles ddlYear.TextChanged
    '    getSTerm(hdnStudent.Value, ddlYear.SelectedItem.Value, MyConn)
    '    ddlTerm.Visible = True
    '    RWI.Visible = True
    '    EVGClass.Visible = True
    '    EVGSection.Visible = True
    '    EVGClass.Text = ""
    '    EVGSection.Text = ""
    'End Sub
    'Protected Sub Selected_Term(ByVal sender As Object, ByVal e As EventArgs) Handles ddlTerm.SelectedIndexChanged
    '    EVGClass.Text = getSClass(hdnStudent.Value, ddlYear.SelectedItem.Value, ddlTerm.SelectedItem.Value, MyConn)
    '    EVGSection.Text = getSsection(ddlYear.SelectedItem.Value, ddlTerm.SelectedItem.Value, EVGClass.Text, MyConn)
    '    EVGClass.Visible = True
    '    EVGSection.Visible = True
    '    rptSort.DataSource = getSortdata(ddlYear.SelectedItem.Value, ddlTerm.SelectedItem.Value, EVGClass.Text, 1, 4, MyConn)
    '    rptSort.DataBind()
    '    rptSort1.DataSource = getSortdata(ddlYear.SelectedItem.Value, ddlTerm.SelectedItem.Value, EVGClass.Text, 5, 8, MyConn)
    '    rptSort1.DataBind()
    'End Sub
    'Public Function getSYear(ByVal Sid As Integer, conn As SqlConnection) As String
    '    Dim q As String = ("SELECT SYear FROM [MEM_StudentSort] where StudentId=" + Convert.ToString(Sid))
    '    Dim adpt = New SqlDataAdapter(q, conn)
    '    Dim dt = New DataTable()
    '    adpt.Fill(dt)
    '    For Each dr As DataRow In dt.Rows
    '        Dim dup As String = Convert.ToString(dr("SYear"))
    '        If (dt.Select(String.Format("SYear='{0}'", dup)).Count > 1) Then
    '            dr.Delete()
    '        End If
    '    Next
    '    ddlYear.DataSource = dt
    '    ddlYear.DataBind()
    '    ddlYear.DataTextField = "SYear"
    '    ddlYear.DataValueField = "SYear"
    '    ddlYear.DataBind()
    '    ddlYear.Items.Insert(0, New ListItem(Sid, "0"))
    '    Dim conm As String = ddlYear.SelectedItem.Value
    '    Return conm
    'End Function

    Public Function getSTerm(ByVal Sid As Integer, ByVal year As String, conn As SqlConnection) As String
        Dim q As String = "SELECT STerm FROM [MEM_StudentSort] Where StudentId=" + Convert.ToString(Sid) + " And SYear like '" + year + "'"
        Dim adpt = New SqlDataAdapter(q, conn)
        Dim dt = New DataTable()
        adpt.Fill(dt)
        ddlTerm.DataSource = dt
        ddlTerm.DataBind()
        ddlTerm.DataTextField = "STerm"
        ddlTerm.DataValueField = "STerm"
        ddlTerm.DataBind()
        ddlTerm.Items.Insert(0, New ListItem("-- Select Term --", "0"))
        ddlTerm.Enabled = True
        Dim conm As String = ddlTerm.SelectedItem.Value
        Return conm
    End Function
    Public Function getSClass(ByVal Sid As Integer, ByVal year As String, ByVal term As String, conn As SqlConnection) As Integer
        Dim mySClass As Integer
        Dim q As String = "SELECT SClass FROM [MEM_StudentSort] WHERE StudentId=" + Convert.ToString(Sid) + " AND SYear LIKE '" + year + "'" + " AND STerm LIKE '" + term + "'"
        Dim adpt = New SqlDataAdapter(q, conn)
        Dim dt = New DataTable()
        adpt.Fill(dt)
        MySClass = dt.Rows(0).Item(0)
        Return mySClass
    End Function

    Public Function getSsection(ByVal year As String, ByVal term As String, ByVal Sclass As Integer, conn As SqlConnection) As Integer
        Dim mySTerm As Integer
        Dim q As String = "SELECT MAX(SSection) FROM [MEM_StudentSort] WHERE SYear LIKE '" + year + "'" + " AND STerm LIKE '" + term + "'" + "AND SClass = " + Convert.ToString(Sclass)
        Dim adpt = New SqlDataAdapter(q, conn)
        Dim dt = New DataTable()
        adpt.Fill(dt)
        mySTerm = dt.Rows(0).Item(0)
        Return mySTerm
    End Function

    Public Function getSortdata(ByVal year As String, ByVal term As String, ByVal Sclass As Integer, ByVal FS As Integer, ByVal LS As Integer, conn As SqlConnection) As DataTable
        Dim Sdatatable = New DataTable
        Sdatatable.Columns.Add("No1")
        Sdatatable.Columns.Add("Name1")
        Sdatatable.Columns.Add("Sum1")
        Sdatatable.Columns.Add("Avg1")
        Sdatatable.Columns.Add("No2")
        Sdatatable.Columns.Add("Name2")
        Sdatatable.Columns.Add("Sum2")
        Sdatatable.Columns.Add("Avg2")
        Sdatatable.Columns.Add("No3")
        Sdatatable.Columns.Add("Name3")
        Sdatatable.Columns.Add("Sum3")
        Sdatatable.Columns.Add("Avg3")
        Sdatatable.Columns.Add("No4")
        Sdatatable.Columns.Add("Name4")
        Sdatatable.Columns.Add("Sum4")
        Sdatatable.Columns.Add("Avg4")
        Dim dr As DataRow
        Dim q As String
        q = "SELECT S.FullName ,SS.Fullmark , SS.Percint FROM [MEM_StudentSort] AS SS INNER JOIN [MEM_Student] AS S ON SS.StudentID = S.ID "
        q = q + " WHERE SYear LIKE '" + year + "'"
        q = q + " AND STerm LIKE '" + term + "'"
        q = q + " AND SClass = " + Convert.ToString(Sclass)
        q = q + " AND SSection =" + Convert.ToString(FS)
        q = q + " ORDER BY Percint DESC"
        Dim adpt1 = New SqlDataAdapter(q, conn)
        Dim dt1 = New DataTable()
        adpt1.Fill(dt1)
        q = "SELECT S.FullName ,SS.Fullmark , SS.Percint FROM [MEM_StudentSort] AS SS INNER JOIN [MEM_Student] AS S ON SS.StudentID = S.ID "
        q = q + " WHERE SYear LIKE '" + year + "'"
        q = q + " AND STerm LIKE '" + term + "'"
        q = q + " AND SClass = " + Convert.ToString(Sclass)
        q = q + " AND SSection =" + Convert.ToString(1 + FS)
        q = q + " ORDER BY Percint DESC"
        Dim adpt2 = New SqlDataAdapter(q, conn)
        Dim dt2 = New DataTable()
        adpt2.Fill(dt2)
        q = "SELECT S.FullName ,SS.Fullmark , SS.Percint FROM [MEM_StudentSort] AS SS INNER JOIN [MEM_Student] AS S ON SS.StudentID = S.ID "
        q = q + " WHERE SYear LIKE '" + year + "'"
        q = q + " AND STerm LIKE '" + term + "'"
        q = q + " AND SClass = " + Convert.ToString(Sclass)
        q = q + " AND SSection =" + Convert.ToString(2 + FS)
        q = q + " ORDER BY Percint DESC"
        Dim adpt3 = New SqlDataAdapter(q, conn)
        Dim dt3 = New DataTable()
        adpt3.Fill(dt3)
        q = ""
        q = "SELECT S.FullName ,SS.Fullmark , SS.Percint FROM [MEM_StudentSort] AS SS INNER JOIN [MEM_Student] AS S ON SS.StudentID = S.ID "
        q = q + " WHERE SYear LIKE '" + year + "'"
        q = q + " AND STerm LIKE '" + term + "'"
        q = q + " AND SClass = " + Convert.ToString(Sclass)
        q = q + " AND SSection =" + Convert.ToString(3 + FS)
        q = q + " ORDER BY Percint DESC"
        Dim adpt4 = New SqlDataAdapter(q, conn)
        Dim dt4 = New DataTable()
        adpt4.Fill(dt4)
        For i As Integer = 0 To 29
            dr = Sdatatable.NewRow()
            dr("No1") = i + 1
            dr("No2") = i + 1
            dr("No3") = i + 1
            dr("No4") = i + 1
            ' 1st DataTabel
            If (i < dt1.Rows.Count) Then
                dr("Name1") = dt1.Rows(i).Item(0)
            Else
                dr("Name1") = ""
            End If

            If (i < dt1.Rows.Count) Then
                dr("Sum1") = dt1.Rows(i).Item(1)
            Else
                dr("Sum1") = 0
            End If

            If (i < dt1.Rows.Count) Then
                dr("Avg1") = dt1.Rows(i).Item(2)
            Else
                dr("Avg1") = 0
            End If

            ' 2nd DataTabel
            If (i < dt2.Rows.Count) Then
                dr("Name2") = dt2.Rows(i).Item(0)
            Else
                dr("Name2") = ""
            End If

            If (i < dt2.Rows.Count) Then
                dr("Sum2") = dt2.Rows(i).Item(1)
            Else
                dr("Sum2") = 0
            End If

            If (i < dt2.Rows.Count) Then
                dr("Avg2") = dt2.Rows(i).Item(2)
            Else
                dr("Avg2") = 0
            End If

            ' 3rd DataTabel
            If (i < dt3.Rows.Count) Then
                dr("Name3") = dt3.Rows(i).Item(0)
            Else
                dr("Name3") = ""
            End If

            If (i < dt3.Rows.Count) Then
                dr("Sum3") = dt3.Rows(i).Item(1)
            Else
                dr("Sum3") = 0
            End If

            If (i < dt3.Rows.Count) Then
                dr("Avg3") = dt3.Rows(i).Item(2)
            Else
                dr("Avg3") = 0
            End If

            ' 4th DataTabel
            If (i < dt4.Rows.Count) Then
                dr("Name4") = dt4.Rows(i).Item(0)
            Else
                dr("Name4") = ""
            End If

            If (i < dt4.Rows.Count) Then
                dr("Sum4") = dt4.Rows(i).Item(1)
            Else
                dr("Sum4") = 0
            End If

            If (i < dt4.Rows.Count) Then
                dr("Avg4") = dt4.Rows(i).Item(2)
            Else
                dr("Avg4") = 0
            End If
            Sdatatable.Rows.Add(dr)
        Next
        adpt1.Dispose()
        adpt2.Dispose()
        adpt3.Dispose()
        adpt4.Dispose()
        dt1.Dispose()
        dt2.Dispose()
        dt3.Dispose()
        dt4.Dispose()
        Return Sdatatable
    End Function
#End Region

End Class
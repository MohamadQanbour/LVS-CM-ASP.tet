Imports EGV
Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_special_student_tests
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        Try
            MyConn.Open()
            LoadMainDropdowns(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        hdnUserId.Value = AuthUser.Id
        Try
            MyConn.Open()
            LoadDropDowns(MyConn)
            If Not Page.IsPostBack Then
                BindMaterialData(MyConn)
                egvBox2.Visible = False
                EGVScriptManager.AddScript(Path.MapCMSScript("local/student-tests") & "?v=1.0")
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkLoad1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkLoad1.Click
        Try
            MyConn.Open()
            BindMaterialData(MyConn)
            LoadDropDowns(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkLoad2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkLoad.Click
        Try
            MyConn.Open()
            BindStudentData(MyConn)
            LoadDropDowns(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub rptStudents_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptStudents.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim dr As DataRowView = e.Item.DataItem
            Dim rpt As Repeater = e.Item.FindControl("rptExams")
            Dim templateId As Integer = MaterialController.GetTemplateId(MyConn, hdnSelectedMaterial.Value)
            Dim templates = ExamTemplateItemController.GetTemplateItems(templateId, LanguageId, MyConn)
            rpt.DataSource = templates
            rpt.DataBind()
        End If
    End Sub

    Protected Sub rptMaterials_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptMaterials.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim dr As DataRowView = e.Item.DataItem
            Dim rpt As Repeater = e.Item.FindControl("rptExams")
            Dim templateId As Integer = MaterialController.GetTemplateId(MyConn, Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger))
            Dim templates = ExamTemplateItemController.GetTemplateItems(templateId, LanguageId, MyConn)
            rpt.DataSource = templates
            rpt.DataBind()
            Dim rpt2 As Repeater = e.Item.FindControl("rptExamValues")
            rpt2.DataSource = templates
            rpt2.DataBind()
        End If
    End Sub

    Protected Sub lnkSave2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSave2.Click
        Try
            MyConn.Open()
            Dim lst As New List(Of StudentMaterialExamItem)
            For Each item As RepeaterItem In rptMaterials.Items
                Dim hdnmaterial As HiddenField = item.FindControl("hdnMaterial")
                Dim materialId As Integer = hdnmaterial.Value
                Dim studentId As Integer = hdnStudentId.Value
                Dim rpt As Repeater = item.FindControl("rptExamValues")
                For Each exam As RepeaterItem In rpt.Items
                    Dim txt As EGVControls.EGVTextBox = exam.FindControl("txt")
                    Dim examId As Integer = txt.Attributes("data-key")
                    Dim type As String = txt.Attributes("data-type")
                    Dim mark As Decimal = 0
                    Dim markString As String = String.Empty
                    If type = Helper.GetEnumText("ExamItemTypes", ExamItemTypes.Number) Then
                        markString = txt.Text
                    Else
                        Dim hdnValue As HiddenField = exam.FindControl("hdnValue")
                        markString = hdnValue.Value
                    End If
                    If markString <> String.Empty Then
                        mark = CDec(markString)
                        lst.Add(New StudentMaterialExamItem() With {
                            .ItemId = examId,
                            .MaterialId = materialId,
                            .StudentId = studentId,
                            .Mark = mark
                        })
                    End If
                Next
            Next
            For Each m As StudentMaterialExamItem In lst
                StudentExamController.Save(MyConn, m.StudentId, m.MaterialId, m.ItemId, m.Mark, AuthUser.Id)
            Next
            Master.Notifier.Success(Localization.GetResource("Resources.Local.SaveSuccess"))
            BindStudentData(MyConn)
            LoadDropDowns(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkSave1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSave1.Click
        Try
            MyConn.Open()
            Dim lst As New List(Of StudentMaterialExamItem)
            For Each item As RepeaterItem In rptStudents.Items
                Dim hdnStudent As HiddenField = item.FindControl("hdnStudent")
                Dim studentId As Integer = hdnStudent.Value
                Dim materialId As Integer = hdnSelectedMaterial.Value
                Dim rptExams As Repeater = item.FindControl("rptExams")
                For Each exam As RepeaterItem In rptExams.Items
                    Dim txt As EGVControls.EGVTextBox = exam.FindControl("txt")
                    Dim examId As Integer = txt.Attributes("data-key")
                    Dim type As String = txt.Attributes("data-type")
                    Dim mark As Decimal = 0
                    Dim markText As String = String.Empty
                    If type = Helper.GetEnumText("ExamItemTypes", ExamItemTypes.Number) Then
                        markText = txt.Text
                    Else
                        Dim hdnValue As HiddenField = exam.FindControl("hdnValue")
                        markText = hdnValue.Value
                    End If
                    If markText <> String.Empty Then
                        mark = CDec(markText)
                        lst.Add(New StudentMaterialExamItem() With {
                            .ItemId = examId,
                            .MaterialId = materialId,
                            .StudentId = studentId,
                            .Mark = mark
                        })
                    End If
                Next
            Next
            For Each m As StudentMaterialExamItem In lst
                StudentExamController.Save(MyConn, m.StudentId, m.MaterialId, m.ItemId, m.Mark, AuthUser.Id)
            Next
            Master.Notifier.Success(Localization.GetResource("Resources.Local.SaveSuccess"))
            BindMaterialData(MyConn)
            LoadDropDowns(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

#End Region

#Region "Private Methods"

    Private Sub LoadMainDropdowns(ByVal conn As SqlConnection)
        Dim classes = StudyClassController.GetTeacherClasses(conn, AuthUser.Id, LanguageId)
        For Each c As ClassObject In classes
            ddlClass.Items.Add(New ListItem(c.Title, c.Id))
        Next
        Dim students = StudentController.GetTeacherStudents(conn, AuthUser.Id, String.Empty, True)
        If students.Count > 0 Then
            ddlStudent.BindToDataSource(students.List, "IdName", "Id")
        End If
    End Sub

    Private Sub LoadDropDowns(ByVal conn As SqlConnection)
        Dim classes = StudyClassController.GetTeacherClasses(conn, AuthUser.Id, LanguageId)
        If classes.Count > 0 Then
            Dim sections = SectionController.GetTeacherSections(conn, AuthUser.Id, LanguageId, "", ddlClass.SelectedValue)
            For Each s As SectionObject In sections
                ddlSection.Items.Add(New ListItem(s.Title, s.Id))
            Next
            If hdnSelectedSection.Value <> String.Empty AndAlso hdnSelectedSection.Value > 0 AndAlso ddlSection.Items.FindByValue(hdnSelectedSection.Value) IsNot Nothing Then ddlSection.SelectedValue = hdnSelectedSection.Value
            If sections.Count > 0 Then
                hdnSelectedSection.Value = ddlSection.SelectedValue
                Dim materials = MaterialController.GetTeacherMaterials(conn, AuthUser.Id, LanguageId, ddlClass.SelectedValue, "", hdnSelectedSection.Value)
                If materials.Count > 0 Then
                    ddlMaterial.BindToDataSource(materials.List, "MaterialTitle", "Id")
                    If hdnSelectedMaterial.Value <> String.Empty AndAlso hdnSelectedMaterial.Value > 0 AndAlso ddlMaterial.Items.FindByValue(hdnSelectedMaterial.Value) IsNot Nothing Then ddlMaterial.SelectedValue = hdnSelectedMaterial.Value
                    hdnSelectedMaterial.Value = ddlMaterial.SelectedValue
                End If
            End If
        End If
    End Sub

    Private Sub BindMaterialData(ByVal conn As SqlConnection)
        Dim classId As Integer = Helper.GetSafeObject(ddlClass.SelectedValue, ValueTypes.TypeInteger)
        Dim sectionId As Integer = Helper.GetSafeObject(hdnSelectedSection.Value)
        Dim materialId As Integer = Helper.GetSafeObject(hdnSelectedMaterial.Value)
        If classId > 0 AndAlso sectionId > 0 AndAlso materialId > 0 Then
            Dim templateId As Integer = MaterialController.GetTemplateId(conn, materialId)
            Dim lst = ExamTemplateItemController.GetTemplateItems(templateId, LanguageId, conn)
            rptMaterialExams1.DataSource = lst
            rptMaterialExams1.DataBind()
            hdnMaterialMaxMark.Value = MaterialController.GetMaxMark(materialId, conn)
            litMaxMark.Text = hdnMaterialMaxMark.Value
            rptStudents.DataSource = StudentController.GetCollection(conn, 0, String.Empty, True, 0, sectionId).List
            rptStudents.DataBind()
            hdnResults1.Value = Helper.JSSerialize(StudentExamController.GetResults(conn, materialId))
            egvBox1.Visible = True
            egvBox1.LoadTitle(SectionController.GetTitle(conn, sectionId, LanguageId) & " - " & MaterialController.GetTitle(materialId, LanguageId, conn))
            EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab1""]').click();", False)
            EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab2""]').click();", False)
            EGVScriptManager.AddInlineScript("$('[data-toggle=tab][href=""#Tab1""]').click();", False)
        Else
            egvBox1.Visible = False
        End If
    End Sub

    Private Sub BindStudentData(ByVal conn As SqlConnection)
        Dim studentId As Integer = Helper.GetSafeObject(ddlStudent.SelectedValue, ValueTypes.TypeInteger)
        If studentId > 0 Then
            hdnStudentClassId.Value = StudentController.GetClassId(studentId, conn)
            hdnStudentId.Value = studentId
            Dim materials = StudentController.GetStudentMaterials(studentId, AuthUser.Id, hdnStudentClassId.Value, LanguageId, conn)
            If materials.Count > 0 Then
                rptMaterials.DataSource = materials.List
                rptMaterials.DataBind()
                hdnResults2.Value = Helper.JSSerialize(StudentExamController.GetResults(conn, 0, studentId))
                egvBox2.Visible = True
                egvBox2.LoadTitle(StudentController.GetSchoolId(studentId, MyConn) & " - " & StudentController.GetFullName(studentId, MyConn))
                EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab1""]').click();", False)
                EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab2""]').click();", False)
                EGVScriptManager.AddInlineScript("$('[data-toggle=tab][href=""#Tab2""]').click();", False)
            Else
                egvBox2.Visible = False
            End If
        End If
    End Sub

#End Region

End Class

Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_special_student_tests2
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
            LoadDropdowns(MyConn)
            If Not Page.IsPostBack Then
                BindMaterialData(MyConn)
                egvStudentBox.Visible = False
                EGVScriptManager.AddScript(Path.MapCMSScript("local/student-tests2") & "?v=1.1")
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub rptStudents_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptMaterials.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim dr As DataRowView = e.Item.DataItem
            Dim rpt As Repeater = e.Item.FindControl("rptExams")
            Dim materialId As Integer = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger)
            Dim templateId As Integer = MaterialController.GetTemplateId(MyConn, materialId)
            Dim templates = ExamTemplateItemController.GetTemplateItems(templateId, LanguageId, MyConn)
            rpt.DataSource = templates
            rpt.DataBind()
        End If
    End Sub

    Protected Sub rptMaterials_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptMaterialStudents.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim dr As DataRowView = e.Item.DataItem
            Dim rpt As Repeater = e.Item.FindControl("rptExams")
            Dim materialId As Integer = Helper.GetSafeObject(hdnSelectedMaterial.Value)
            Dim templateId As Integer = MaterialController.GetTemplateId(MyConn, materialId)
            Dim templates = ExamTemplateItemController.GetTemplateItems(templateId, LanguageId, MyConn)
            rpt.DataSource = templates
            rpt.DataBind()
        End If
    End Sub

    Protected Sub lnkLoad1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkLoad1.Click
        Try
            MyConn.Open()
            BindMaterialData(MyConn)
            LoadDropdowns(MyConn)
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
            LoadDropdowns(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkPublish_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkPublish.Click
        Try
            MyConn.Open()
            StudentExamController.PublishExams(MyConn, hdnSelectedMaterialPublish.Value, hdnSelectedExamPublish.Value, hdnSelectedSectionPublish.Value)
            Master.Notifier.Success(Localization.GetResource("Resources.Local.PublishSuccess"))
            EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab1""]').click();", False)
            EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab2""]').click();", False)
            EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab3""]').click();", False)
            EGVScriptManager.AddInlineScript("$('[data-toggle=tab][href=""#Tab3""]').click();", False)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkSaveMaterial_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSaveMaterial.Click
        Try
            MyConn.Open()
            Dim lst As New List(Of StudentMaterialExamItem)
            For Each item As RepeaterItem In rptMaterialStudents.Items
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
                        markText = Math.Ceiling(CDec(hdnValue.Value))
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
            LoadDropdowns(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkSaveStudent_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSaveStudent.Click
        Try
            MyConn.Open()
            Dim lst As New List(Of StudentMaterialExamItem)
            For Each item As RepeaterItem In rptMaterials.Items
                Dim hdnmaterial As HiddenField = item.FindControl("hdnMaterial")
                Dim materialId As Integer = hdnmaterial.Value
                Dim studentId As Integer = hdnStudentId.Value
                Dim rpt As Repeater = item.FindControl("rptExams")
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
                        markString = Math.Ceiling(CDec(hdnValue.Value))
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
            LoadDropdowns(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

#End Region

#Region "Public Methods"

    Public Function GetMaxMark(ByVal conn As SqlConnection, ByVal item As RepeaterItem, ByVal itemId As Integer) As Integer
        Dim dr As DataRowView = DirectCast(DirectCast(DirectCast(item.Parent, Repeater).NamingContainer, RepeaterItem).DataItem, DataRowView)
        Dim materialId As Integer = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger)
        Return MaterialExamTemplateItemController.GetMaxMark(materialId, itemId, conn)
    End Function

    Public Function GetMaxMark1(ByVal conn As SqlConnection, ByVal itemId As Integer) As Integer
        Dim materialId As Integer = hdnSelectedMaterial.Value
        Return MaterialExamTemplateItemController.GetMaxMark(materialId, itemId, conn)
    End Function

#End Region

#Region "Private Methods"

    Private Sub LoadMainDropdowns(ByVal conn As SqlConnection)
        Dim classes = StudyClassController.GetTeacherClasses(conn, AuthUser.Id, LanguageId)
        ddlClass.Items.Clear()
        ddlClassPublish.Items.Clear()
        For Each c In classes
            If c.Title IsNot Nothing AndAlso c.Id <> Nothing AndAlso c.Title <> String.Empty AndAlso c.Id > 0 Then
                ddlClass.Items.Add(New ListItem(c.Title, c.Id))
                ddlClassPublish.Items.Add(New ListItem(c.Title, c.Id))
            End If
        Next
        Dim students = StudentController.GetTeacherStudents(conn, AuthUser.Id, String.Empty, True, 10)
        If students.Count > 0 Then
            ddlStudent.BindToDataSource(students.List, "IdName", "Id")
        End If
    End Sub

    Private Sub LoadDropdowns(ByVal conn As SqlConnection)
        Dim classes = StudyClassController.GetTeacherClasses(conn, AuthUser.Id, LanguageId)
        If classes.Count > 0 Then
            Dim sections = SectionController.GetTeacherSections(conn, AuthUser.Id, LanguageId, String.Empty, ddlClass.SelectedValue)
            ddlSection.Items.Clear()
            ddlSectionPublish.Items.Clear()
            For Each s In sections
                ddlSection.Items.Add(New ListItem(s.Title, s.Id))
                ddlSectionPublish.Items.Add(New ListItem(s.Title, s.Id))
            Next
            If hdnSelectedSection.Value <> String.Empty AndAlso hdnSelectedSection.Value > 0 AndAlso ddlSection.Items.FindByValue(hdnSelectedSection.Value) IsNot Nothing Then ddlSection.SelectedValue = hdnSelectedSection.Value
            If hdnSelectedSectionPublish.Value <> String.Empty AndAlso hdnSelectedSectionPublish.Value > 0 AndAlso ddlSectionPublish.Items.FindByValue(hdnSelectedSectionPublish.Value) IsNot Nothing Then ddlSectionPublish.SelectedValue = hdnSelectedSectionPublish.Value
            If sections.Count > 0 Then
                hdnSelectedSection.Value = ddlSection.SelectedValue
                hdnSelectedSectionPublish.Value = ddlSectionPublish.SelectedValue
                Dim materials = MaterialController.GetTeacherMaterials(conn, AuthUser.Id, LanguageId, ddlClass.SelectedValue, "", hdnSelectedSection.Value)
                If materials.Count > 0 Then
                    ddlMaterial.BindToDataSource(materials.List, "MaterialTitle", "Id")
                    ddlMaterialPublish.BindToDataSource(materials.List, "MaterialTitle", "Id")
                    If hdnSelectedMaterial.Value <> String.Empty AndAlso hdnSelectedMaterial.Value > 0 AndAlso ddlMaterial.Items.FindByValue(hdnSelectedMaterial.Value) IsNot Nothing Then ddlMaterial.SelectedValue = hdnSelectedMaterial.Value
                    If hdnSelectedMaterialPublish.Value <> String.Empty AndAlso hdnSelectedMaterialPublish.Value > 0 AndAlso ddlMaterialPublish.Items.FindByValue(hdnSelectedMaterialPublish.Value) IsNot Nothing Then ddlMaterialPublish.SelectedValue = hdnSelectedMaterialPublish.Value
                    hdnSelectedMaterial.Value = ddlMaterial.SelectedValue
                    hdnSelectedMaterialPublish.Value = ddlMaterialPublish.SelectedValue
                    Dim materialId = hdnSelectedMaterialPublish.Value
                    Dim templateId As Integer = MaterialController.GetTemplateId(MyConn, materialId)
                    Dim templates = ExamTemplateItemController.GetTemplateItems(templateId, LanguageId, MyConn)
                    If templates.Count > 0 Then
                        ddlExamsPublish.Items.Clear()
                        For Each e In templates
                            ddlExamsPublish.Items.Add(New ListItem(e.Title, e.Id))
                        Next
                        If hdnSelectedExamPublish.Value <> String.Empty AndAlso hdnSelectedExamPublish.Value > 0 AndAlso ddlExamsPublish.Items.FindByValue(hdnSelectedExamPublish.Value) IsNot Nothing Then ddlExamsPublish.SelectedValue = hdnSelectedExamPublish.Value
                        hdnSelectedExamPublish.Value = ddlExamsPublish.SelectedValue
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub BindMaterialData(ByVal conn As SqlConnection)
        Dim classId As Integer = Helper.GetSafeObject(ddlClass.SelectedValue, ValueTypes.TypeInteger)
        Dim sectionId As Integer = Helper.GetSafeObject(hdnSelectedSection.Value)
        Dim materialId As Integer = Helper.GetSafeObject(hdnSelectedMaterial.Value)
        If classId > 0 AndAlso sectionId > 0 AndAlso materialId > 0 Then
            hdnMaterialMaxMark.Value = MaterialController.GetMaxMark(materialId, conn)
            litMaxMark.Text = hdnMaterialMaxMark.Value
            rptMaterialStudents.DataSource = StudentController.GetCollection(conn, 0, String.Empty, True, 0, sectionId).List
            rptMaterialStudents.DataBind()
            hdnResultsMaterial.Value = Helper.JSSerialize(StudentExamController.GetResults(conn, materialId))
            egvMaterialBox.Visible = True
            egvMaterialBox.LoadTitle(SectionController.GetTitle(conn, sectionId, LanguageId) & " - " & MaterialController.GetTitle(materialId, LanguageId, conn))
            EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab1""]').click();", False)
            EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab2""]').click();", False)
            EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab3""]').click();", False)
            EGVScriptManager.AddInlineScript("$('[data-toggle=tab][href=""#Tab1""]').click();", False)
        Else
            egvMaterialBox.Visible = False
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
                egvStudentBox.Visible = True
                egvStudentBox.LoadTitle(StudentController.GetSchoolId(studentId, MyConn) & " - " & StudentController.GetFullName(studentId, MyConn))
                EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab1""]').click();", False)
                EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab2""]').click();", False)
                EGVScriptManager.RemoveInlineScript("$('[data-toggle=tab][href=""#Tab3""]').click();", False)
                EGVScriptManager.AddInlineScript("$('[data-toggle=tab][href=""#Tab2""]').click();", False)
            Else
                egvStudentBox.Visible = False
            End If
        End If
    End Sub

    Public Overrides Sub ProcessPermissions(usr As User, Optional pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        MyBase.ProcessPermissions(usr, pid, conn)
        Dim obj As New CMSMenu(pid, conn)
        Dim canPublish As Boolean = usr.CanPublish(obj.PermissionId, conn)
        Dim canDelete As Boolean = usr.CanDelete(obj.PermissionId, conn)
        Dim canModify As Boolean = usr.CanModify(obj.PermissionId, conn)
        Dim canWrite As Boolean = usr.CanWrite(obj.PermissionId, conn)
        If Not canPublish Then egvTabs.HideTab("tabPublish")
    End Sub

#End Region

End Class

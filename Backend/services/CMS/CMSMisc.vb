Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Enums
Imports EGV

Namespace Ajax

    Public Class CMSMisc
        Inherits AjaxBaseClass

#Region "Request Values"

        Public Property ExamTemplateItemId As Integer = GetSafeRequestValue("id", ValueTypes.TypeInteger)
        Public Property MaterialId As Integer = GetSafeRequestValue("materialid", ValueTypes.TypeInteger)
        Public Property TemplateId As Integer = GetSafeRequestValue("templateid", ValueTypes.TypeInteger)
        Public Property MaxMark As Integer = GetSafeRequestValue("maxmark", ValueTypes.TypeInteger)
        Public Property PaymentId As Integer = GetSafeRequestValue("paymentid", ValueTypes.TypeInteger)

#End Region

#Region "Overridden Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "TemplateItemRelations"
                    ret = GetExamTemplateItemRelations(MyConn)
                Case "MaterialExamItems"
                    ret = GetMaterialExamItems(MyConn, LanguageId)
                Case "StudentPayment"
                    ret = GetPaymentItem(MyConn)
                Case "LoadExamMaterialInfo"
                    ret = LoadExamMaterialInfo(MyConn)
                Case "LoadExamMaterialResults"
                    ret = GetMaterialExams(MyConn)
                Case "SaveExamMark"
                    ret = SaveExamMark(MyConn)
                Case "PublishMarks"
                    ret = PublishExamMarks(MyConn)
                Case "StudentExams"
                    ret = GetStudentExams(MyConn)
                Case "StudentClassId"
                    ret = GetStudentClass(MyConn)
                Case "AddInternalNote"
                    ret = AddInternalNote(MyConn)
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function AddInternalNote(ByVal conn As SqlConnection) As InternalNoteItem
            Dim userId As Integer = GetSafeRequestValue("user", ValueTypes.TypeInteger)
            Dim studentId As Integer = GetSafeRequestValue("student", ValueTypes.TypeInteger)
            Dim note As String = GetSafeRequestValue("note")
            Dim noteDate As String = GetSafeRequestValue("date")
            Dim obj As New StudentNote() With {
                .Note = note,
                .StudentId = studentId,
                .NoteDate = Helper.ParseDate(noteDate, "yyyy-MM-dd"),
                .UserId = userId
            }
            Dim trans As SqlTransaction = conn.BeginTransaction()
            Try
                obj.Save(trans)
                trans.Commit()
            Catch ex As Exception
                trans.Rollback()
                Throw ex
            End Try
            Dim ret As New InternalNoteItem() With {
                .Note = note,
                .NoteDate = obj.NoteDate.ToString("MMMM dd, yyyy"),
                .UserName = New User(userId, conn).Profile.FullName
            }
            Return ret
        End Function

        Private Function GetStudentClass(ByVal conn As SqlConnection) As Integer
            Dim studentId As Integer = GetSafeRequestValue("student", ValueTypes.TypeInteger)
            Return StudentController.GetClassId(studentId, conn)
        End Function

        Private Function GetStudentExams(ByVal conn As SqlConnection) As List(Of MaterialExamsItem)
            Dim studentId As Integer = GetSafeRequestValue("student", ValueTypes.TypeInteger)
            Dim userId As Integer = GetSafeRequestValue("user", ValueTypes.TypeInteger)
            Dim classId As Integer = GetSafeRequestValue("class", ValueTypes.TypeInteger)
            Dim lst As New List(Of MaterialExamsItem)
            Using dt = StudentController.GetStudentMaterials(studentId, userId, classId, LanguageId, conn).List
                For Each dr As DataRow In dt.Rows
                    If dr IsNot Nothing Then
                        Dim item As New MaterialExamsItem() With {
                            .Id = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                            .MaxMark = New Material(Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger), conn).MaxMark,
                            .MaterialTitle = Helper.GetSafeDBValue(dr("MaterialTitle")),
                            .Exams = New List(Of ExamItem)()
                        }
                        Dim exams = StudentExamController.GetResults(conn, item.Id, studentId)
                        Dim templateId As Integer = MaterialController.GetTemplateId(MyConn, item.Id)
                        Dim examItems = ExamTemplateItemController.GetTemplateItems(templateId, LanguageId, MyConn)
                        For Each i In examItems
                            item.Exams.Add(New ExamItem() With {
                                .Id = i.Id,
                                .Title = i.Title,
                                .Related = String.Join(",", ExamTemplateItemController.GetRelatedIds(i.Id, conn)),
                                .Type = Helper.GetEnumText("ExamItemTypes", i.Type),
                                .Mark = StudentExamController.GetMark(conn, item.Id, studentId, i.Id),
                                .MaxMark = MaterialExamTemplateItemController.GetMaxMark(item.Id, i.Id, conn)
                            })
                        Next
                        lst.Add(item)
                    End If
                Next
            End Using
            Return lst
        End Function

        Private Function PublishExamMarks(ByVal conn As SqlConnection) As Boolean
            Dim materialId As Integer = GetSafeRequestValue("material", ValueTypes.TypeInteger)
            Dim examId As Integer = GetSafeRequestValue("exam", ValueTypes.TypeInteger)
            Dim sectionId As Integer = GetSafeRequestValue("section", ValueTypes.TypeInteger)
            StudentExamController.PublishExams(conn, materialId, examId, sectionId)
            Return True
        End Function

        Private Function SaveExamMark(ByVal conn As SqlConnection) As Boolean
            Dim studentId As Integer = GetSafeRequestValue("student", ValueTypes.TypeInteger)
            Dim materialId As Integer = GetSafeRequestValue("material", ValueTypes.TypeInteger)
            Dim examId As Integer = GetSafeRequestValue("exam", ValueTypes.TypeInteger)
            Dim seasonId As Integer = SeasonController.GetCurrentId(conn)
            Dim mark As Decimal = GetSafeRequestValue("mark", ValueTypes.TypeDecimal)
            Dim userId As Integer = GetSafeRequestValue("user", ValueTypes.TypeInteger)
            StudentExamController.Save(conn, studentId, materialId, examId, mark, userId)
            Return True
        End Function

        Private Function LoadExamMaterialInfo(ByVal conn As SqlConnection) As Integer
            Dim materialId As Integer = GetSafeRequestValue("material", ValueTypes.TypeInteger)
            If materialId > 0 Then Return New Material(materialId, conn, LanguageId).MaxMark Else Return 0
        End Function

        Private Function GetPaymentItem(ByVal conn As SqlConnection) As PaymentItem
            'id
            Dim obj As New StudentPayment(PaymentId, conn)
            Return New PaymentItem() With {
                .Id = obj.Id,
                .StudentId = obj.StudentId,
                .PaymentNumber = obj.PaymentNumber,
                .PaymentAmount = obj.PaymentAmount,
                .PaymentDate = obj.PaymentDate.ToString("yyyy-MM-dd"),
                .PaymentNote = obj.PaymentNote
            }
        End Function

        Private Function GetExamTemplateItemRelations(ByVal conn As SqlConnection)
            Return ExamTemplateItemController.GetRelatedIds(ExamTemplateItemId, conn)
        End Function

        Private Function GetMaterialExamItems(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of MaterialExamTemplateItem)
            If MaterialId > 0 Then
                Return MaterialExamTemplateItemController.GetMaterialItems(MaterialId, TemplateId, conn, langId)
            Else
                Dim lst As New List(Of MaterialExamTemplateItem)
                For Each item As ExamTemplateItem In ExamTemplateItemController.GetTemplateItems(TemplateId, langId, conn, True)
                    lst.Add(New MaterialExamTemplateItem() With {
                        .MaterialId = 0,
                        .TemplateId = TemplateId,
                        .TemplateItemId = item.Id,
                        .TemplateItemTitle = item.Title,
                        .MaxMark = MaxMark
                    })
                Next
                Return lst
            End If
        End Function

        Private Function GetMaterialExams(ByVal conn As SqlConnection) As List(Of StudentExamsItem)
            Dim classId As Integer = GetSafeRequestValue("class", ValueTypes.TypeInteger)
            Dim sectionId As Integer = GetSafeRequestValue("section", ValueTypes.TypeInteger)
            Dim materialId As Integer = GetSafeRequestValue("material", ValueTypes.TypeInteger)
            Dim itemId As Integer = GetSafeRequestValue("item", ValueTypes.TypeInteger)
            Dim lst As New List(Of StudentExamsItem)()
            If classId > 0 AndAlso sectionId > 0 AndAlso materialId > 0 Then
                Using dt = StudentController.GetCollection(conn, 0, String.Empty, True, 0, sectionId).List
                    For Each dr As DataRow In dt.Rows
                        Dim item As New StudentExamsItem() With {
                            .Id = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                            .IdName = Helper.GetSafeDBValue(dr("IdName")),
                            .Exams = New List(Of ExamItem)()
                        }
                        'Dim exams = StudentExamController.GetResults(conn, materialId, item.Id)
                        Dim templateId As Integer = MaterialController.GetTemplateId(MyConn, materialId)
                        Dim examItems = ExamTemplateItemController.GetTemplateItems(templateId, LanguageId, MyConn, False, itemId)
                        For Each i In examItems
                            item.Exams.Add(New ExamItem() With {
                                .Id = i.Id,
                                .Title = i.Title,
                                .Related = String.Join(",", ExamTemplateItemController.GetRelatedIds(i.Id, conn)),
                                .Type = Helper.GetEnumText("ExamItemTypes", i.Type),
                                .Mark = StudentExamController.GetMark(conn, materialId, item.Id, i.Id),
                                .MaxMark = MaterialExamTemplateItemController.GetMaxMark(materialId, i.Id, conn)
                            })
                        Next
                        lst.Add(item)
                    Next
                End Using
            End If
            Return lst
        End Function

#End Region

#Region "Structure"

        Public Structure StudentExamsItem
            Public Property Id As Integer
            Public Property IdName As String
            Public Property Exams As List(Of ExamItem)
        End Structure

        Public Structure MaterialExamsItem
            Public Property Id As Integer
            Public Property MaxMark As Integer
            Public Property MaterialTitle As String
            Public Property Exams As List(Of ExamItem)
        End Structure

        Public Structure ExamItem
            Public Property Id As Integer
            Public Property Title As String
            Public Property Related As String
            Public Property Type As String
            Public Property Mark As ExamResultItem
            Public Property MaxMark As integer
        End Structure

#End Region

    End Class

End Namespace
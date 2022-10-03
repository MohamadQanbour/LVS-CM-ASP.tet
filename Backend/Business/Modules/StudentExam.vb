Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Structures
Imports EGV.Enums

Namespace EGV
    Namespace Business

        'controller
        Public Class StudentExamController

            Public Shared Function GetReport(ByVal sectionId As Integer, ByVal maxMark As Integer, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal items() As Integer = Nothing) As DBAReturnObject
                Dim tempItemsQuery As String = String.Empty
                If items IsNot Nothing AndAlso items.Length > 0 Then tempItemsQuery = "(" & String.Join(", ", items) & ")"
                Dim ret As New DBAReturnObject()
                Dim q As String = "SELECT *, (Mark * 100) / @MaxMark AS [Percentage] FROM (SELECT S.FullName, CASE WHEN SUM(E.Mark) IS NULL THEN 0 ELSE SUM(E.Mark) END AS Mark FROM MEM_Student S LEFT JOIN (SELECT E1.Mark, E1.StudentId FROM MOD_StudentExam E1 INNER JOIN MOD_ExamTemplateItem I1 ON E1.ExamId = I1.Id WHERE I1.[Type] = @Type" & IIf(tempItemsQuery <> String.Empty, " AND I1.Id IN " & tempItemsQuery, "") & ") E ON S.Id = E.StudentId WHERE S.SectionId = @SectionId GROUP BY S.FullName) A ORDER BY A.Mark DESC"
                If items.Length > 0 Then
                    q = "SELECT *, (Mark * 100) / @MaxMark AS [Percentage] FROM (SELECT S.FullName, CASE WHEN SUM(E.Mark) IS NULL THEN 0 ELSE SUM(E.Mark) END AS Mark FROM MEM_Student S LEFT JOIN (SELECT E1.Mark, E1.StudentId FROM MOD_StudentExam E1 INNER JOIN MOD_ExamTemplateItem I1 ON E1.ExamId = I1.Id" & IIf(tempItemsQuery <> String.Empty, " WHERE I1.Id IN " & tempItemsQuery, "") & ") E ON S.Id = E.StudentId WHERE S.SectionId = @SectionId GROUP BY S.FullName) A ORDER BY A.Mark DESC"
                End If
                ret.Query = q
                ret.List = DBA.DataTable(conn, q, DBA.CreateParameter("MaxMark", SqlDbType.Int, maxMark), DBA.CreateParameter("Type", SqlDbType.Int, ExamItemTypes.Number), DBA.CreateParameter("SectionId", SqlDbType.Int, sectionId))
                ret.Count = ret.List.Rows.Count
                Return ret
            End Function

            Public Shared Function StudentExamExists(ByVal conn As SqlConnection, ByVal studentId As Integer,
                                                ByVal materialId As Integer, ByVal examId As Integer) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_StudentExam WHERE StudentId = @SId AND MaterialId = @MId AND ExamId = @EId AND SeasonId = @NId"
                Return DBA.Scalar(conn, q,
                                  DBA.CreateParameter("SId", SqlDbType.Int, studentId),
                                  DBA.CreateParameter("MId", SqlDbType.Int, materialId),
                                  DBA.CreateParameter("EId", SqlDbType.Int, examId),
                                  DBA.CreateParameter("NId", SqlDbType.Int, SeasonController.GetCurrentId(conn))
                                  ) > 0
            End Function

            Public Shared Sub AddStudentExam(ByVal conn As SqlConnection, ByVal studentId As Integer,
                                             ByVal materialId As Integer, ByVal examId As Integer,
                                             ByVal mark As Decimal, ByVal userId As Integer)
                Dim q As String = "INSERT INTO MOD_StudentExam (StudentId, MaterialId, ExamId, SeasonId, Mark, CreatedDate, CreatedUser) VALUES (@SId, @MId, @EId, @NId, @Mark, GETDATE(), @UserId)"
                Helper.GetSafeUserId(userId)
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("SId", SqlDbType.Int, studentId),
                             DBA.CreateParameter("MId", SqlDbType.Int, materialId),
                             DBA.CreateParameter("EId", SqlDbType.Int, examId),
                             DBA.CreateParameter("NId", SqlDbType.Int, SeasonController.GetCurrentId(conn)),
                             DBA.CreateParameter("Mark", SqlDbType.Decimal, mark, 18, 2),
                             DBA.CreateParameter("UserId", SqlDbType.Int, userId)
                             )
            End Sub

            Public Shared Sub UpdateStudentExam(ByVal conn As SqlConnection, ByVal studentId As Integer,
                                                ByVal materialId As Integer, ByVal examId As Integer,
                                                ByVal mark As Decimal, ByVal userId As Integer)
                Dim q As String = "UPDATE MOD_StudentExam SET Mark = @Mark, ModifiedDate = GETDATE(), ModifiedUser = @UserId WHERE StudentId = @SId AND MaterialId = @MId AND ExamId = @EId AND SeasonId = @NId"
                Helper.GetSafeUserId(userId)
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("SId", SqlDbType.Int, studentId),
                             DBA.CreateParameter("MId", SqlDbType.Int, materialId),
                             DBA.CreateParameter("EId", SqlDbType.Int, examId),
                             DBA.CreateParameter("NId", SqlDbType.Int, SeasonController.GetCurrentId(conn)),
                             DBA.CreateParameter("Mark", SqlDbType.Decimal, mark, 18, 2),
                             DBA.CreateParameter("UserId", SqlDbType.Int, userId)
                             )
            End Sub

            Public Shared Sub Save(ByVal conn As SqlConnection, ByVal studentId As Integer,
                                   ByVal materialId As Integer, ByVal examId As Integer,
                                   ByVal mark As Decimal, ByVal userId As Integer,
                                   Optional ByVal shouldUpdate As Boolean = True)
                If StudentExamExists(conn, studentId, materialId, examId) Then
                    UpdateStudentExam(conn, studentId, materialId, examId, mark, userId)
                Else
                    AddStudentExam(conn, studentId, materialId, examId, mark, userId)
                End If
                If shouldUpdate Then UpdateResults(conn, studentId, materialId, userId)
            End Sub

            Public Shared Sub UpdateResults(ByVal conn As SqlConnection, ByVal studentId As Integer, ByVal materialId As Integer, ByVal userId As Integer)
                Dim lst As New List(Of MaterialExamsItem)
                Dim classId As Integer = StudentController.GetClassId(studentId, conn)
                Dim obj As New Material(materialId, conn, 1)
                Dim item As New MaterialExamsItem() With {
                    .Id = obj.Id,
                    .MaxMark = obj.MaxMark,
                    .MaterialTitle = obj.Title,
                    .Exams = New List(Of ExamItem)()
                }
                Dim exams = GetResults(conn, item.Id, studentId)
                Dim templateId As Integer = obj.ExamTemplateId
                Dim examItems = ExamTemplateItemController.GetTemplateItems(templateId, 1, conn)
                For Each i In examItems
                    item.Exams.Add(New ExamItem() With {
                        .Id = i.Id,
                        .Title = i.Title,
                        .Related = String.Join(",", ExamTemplateItemController.GetRelatedIds(i.Id, conn)),
                        .Type = i.Type,
                        .Mark = GetMark(conn, item.Id, studentId, i.Id),
                        .MaxMark = MaterialExamTemplateItemController.GetMaxMark(item.Id, i.Id, conn)
                    })
                Next
                'update number related
                Dim lstExams = item.Exams
                Dim calculated = (From n In lstExams Where n.Type <> ExamItemTypes.Number).ToList()
                Dim calculated2 As New List(Of ExamItem)
                For Each f In calculated
                    Dim related = Helper.SplitString(f.Related, ",")
                    Dim valid As Boolean = True
                    For Each r In related
                        valid = valid And (ExamTemplateItemController.GetItemType(r, conn) = ExamItemTypes.Number)
                    Next
                    If valid Then
                        Dim sum As Integer = 0
                        Dim affected = (From t In lstExams Where related.Contains(t.Id) AndAlso t.Type = ExamItemTypes.Number).ToList()
                        For Each a In affected
                            sum += a.Mark.Mark
                        Next
                        Select Case f.Type
                            Case ExamItemTypes.Total
                                Save(conn, studentId, materialId, f.Id, Math.Ceiling(sum), userId, False)
                                f.Mark = New ExamResultItem() With {.Mark = sum}
                            Case ExamItemTypes.Average
                                Dim count As Integer = affected.Count
                                If count = 0 Then count = 1
                                Dim avg As Decimal = sum / count
                                Save(conn, studentId, materialId, f.Id, Math.Ceiling(avg), userId, False)
                                f.Mark = New ExamResultItem() With {.Mark = avg}
                        End Select
                    Else
                        calculated2.Add(f)
                    End If
                Next
                If calculated2.Count > 0 Then
                    item.Exams.Clear()
                    exams = GetResults(conn, item.Id, studentId)
                    templateId = obj.ExamTemplateId
                    examItems = ExamTemplateItemController.GetTemplateItems(templateId, 1, conn)
                    For Each i In examItems
                        item.Exams.Add(New ExamItem() With {
                            .Id = i.Id,
                            .Title = i.Title,
                            .Related = String.Join(",", ExamTemplateItemController.GetRelatedIds(i.Id, conn)),
                            .Type = i.Type,
                            .Mark = GetMark(conn, item.Id, studentId, i.Id),
                            .MaxMark = MaterialExamTemplateItemController.GetMaxMark(item.Id, i.Id, conn)
                        })
                    Next
                    For Each f In calculated2
                        Dim related = Helper.SplitString(f.Related, ",")
                        Dim sum As Integer = 0
                        Dim affected = (From t In lstExams Where related.Contains(t.Id)).ToList()
                        For Each a In affected
                            sum += a.Mark.Mark
                        Next
                        Select Case f.Type
                            Case ExamItemTypes.Total
                                Save(conn, studentId, materialId, f.Id, Math.Ceiling(sum), userId, False)
                                f.Mark = New ExamResultItem() With {.Mark = sum}
                            Case ExamItemTypes.Average
                                Dim count As Integer = affected.Count
                                If count = 0 Then count = 1
                                Dim avg As Decimal = sum / count
                                Save(conn, studentId, materialId, f.Id, Math.Ceiling(avg), userId, False)
                                f.Mark = New ExamResultItem() With {.Mark = avg}
                        End Select
                    Next
                End If
            End Sub

            Public Shared Function GetResults(ByVal conn As SqlConnection, Optional ByVal materialId As Integer = 0,
                                              Optional ByVal studentId As Integer = 0, Optional ByVal forMaterial As Boolean = True,
                                              Optional ByVal onlyPublished As Boolean = False) As List(Of StudentMaterialExamItem)
                Dim lst As New List(Of StudentMaterialExamItem)
                Dim q As String = String.Empty
                Dim dt As DataTable = Nothing
                If studentId > 0 Then
                    'q = "SELECT E.*, CU.UserName AS CreatedUserName, MU.UserName AS ModifiedUserName FROM MOD_StudentExam E INNER JOIN SYS_User CU ON E.CreatedUser = CU.Id LEFT JOIN SYS_User MU ON E.ModifiedUser = MU.Id WHERE E.StudentId = @SId AND E.SeasonId = @NId"
                    q = "SELECT E.* FROM MOD_StudentExam E INNER JOIN SYS_User CU ON E.CreatedUser = CU.Id LEFT JOIN SYS_User MU ON E.ModifiedUser = MU.Id WHERE E.StudentId = @SId AND E.SeasonId = @NId"
                    If materialId > 0 Then q &= " AND E.MaterialId = @MId"
                    If onlyPublished Then q &= " AND E.IsPublished = 1"
                    dt = DBA.DataTable(conn, q,
                                       DBA.CreateParameter("SId", SqlDbType.Int, studentId),
                                       DBA.CreateParameter("NId", SqlDbType.Int, SeasonController.GetCurrentId(conn)),
                                       DBA.CreateParameter("MId", SqlDbType.Int, materialId)
                                       )
                ElseIf materialId > 0 Then
                    'q = "SELECT E.*, CU.UserName AS CreatedUserName, MU.UserName AS ModifiedUserName FROM MOD_StudentExam E INNER JOIN SYS_User CU ON E.CreatedUser = CU.Id LEFT JOIN SYS_User MU ON E.ModifiedUser = MU.Id WHERE E.MaterialId = @MId AND E.SeasonId = @NId"
                    q = "SELECT E.* FROM MOD_StudentExam E INNER JOIN SYS_User CU ON E.CreatedUser = CU.Id LEFT JOIN SYS_User MU ON E.ModifiedUser = MU.Id WHERE E.MaterialId = @MId AND E.SeasonId = @NId"
                    If onlyPublished Then q &= " AND E.IsPublished = 1"
                    dt = DBA.DataTable(conn, q,
                                       DBA.CreateParameter("MId", SqlDbType.Int, materialId),
                                       DBA.CreateParameter("NId", SqlDbType.Int, SeasonController.GetCurrentId(conn))
                                       )
                End If
                If dt IsNot Nothing Then
                    For Each dr As DataRow In dt.Rows
                        Dim item As New StudentMaterialExamItem() With {
                            .StudentId = Helper.GetSafeDBValue(dr("StudentId"), ValueTypes.TypeInteger),
                            .MaterialId = Helper.GetSafeDBValue(dr("MaterialId"), ValueTypes.TypeInteger),
                            .ItemId = Helper.GetSafeDBValue(dr("ExamId"), ValueTypes.TypeInteger),
                            .Mark = Helper.GetSafeDBValue(dr("Mark"), ValueTypes.TypeDecimal)
                        }
                        item.MaxMark = MaterialExamTemplateItemController.GetMaxMark(item.MaterialId, item.ItemId, conn)
                        lst.Add(item)
                    Next
                End If
                Return lst
            End Function

            Public Shared Function GetMark(ByVal conn As SqlConnection, ByVal materialId As Integer, ByVal studentId As Integer,
                                           ByVal itemId As Integer) As ExamResultItem
                Dim currentSeasonId As Integer = SeasonController.GetCurrentId(conn)
                Dim q As String = "SELECT E.Mark, E.CreatedDate, CU.FullName AS [CreatedUserName], E.ModifiedDate, MU.FullName AS [ModifiedUserName] FROM MOD_StudentExam E LEFT JOIN SYS_UserProfile CU ON E.CreatedUser = CU.UserId LEFT JOIN SYS_UserProfile MU ON E.ModifiedUser = MU.UserId WHERE E.StudentId = @SID AND E.MaterialId = @MID AND E.ExamId = @EID AND E.SeasonId = @Season"
                Dim dr = DBA.DataRow(conn, q,
                                     DBA.CreateParameter("SID", SqlDbType.Int, studentId),
                                     DBA.CreateParameter("MID", SqlDbType.Int, materialId),
                                     DBA.CreateParameter("EID", SqlDbType.Int, itemId),
                                     DBA.CreateParameter("Season", SqlDbType.Int, currentSeasonId)
                                     )
                If dr IsNot Nothing Then
                    Return New ExamResultItem() With {
                        .Mark = Helper.GetSafeDBValue(dr("Mark"), ValueTypes.TypeDecimal),
                        .CreatedDate = CDate(Helper.GetSafeDBValue(dr("CreatedDate"), ValueTypes.TypeDate)).ToString("MMMM dd, yyyy"),
                        .CreatedUser = Helper.GetSafeDBValue(dr("CreatedUserName")),
                        .ModifiedDate = CDate(Helper.GetSafeDBValue(dr("ModifiedDate"), ValueTypes.TypeDate)).ToString("MMMM dd, yyyy"),
                        .ModifiedUser = Helper.GetSafeDBValue(dr("ModifiedUserName"))
                    }
                Else
                    Return New ExamResultItem() With {.Mark = 0}
                End If
            End Function

            Public Shared Sub PublishExams(ByVal conn As SqlConnection, ByVal materialId As Integer, ByVal examId As Integer, ByVal sectionId As Integer)
                Dim q As String = "UPDATE MOD_StudentExam SET IsPublished = 1 WHERE MaterialId = @MatId AND ExamId = @ExamId AND StudentId IN (SELECT Id FROM MEM_Student WHERE SectionId = @SecId)"
                DBA.NonQuery(conn, q, DBA.CreateParameter("MatId", SqlDbType.Int, materialId), DBA.CreateParameter("ExamId", SqlDbType.Int, examId), DBA.CreateParameter("SecId", SqlDbType.Int, sectionId))
            End Sub

        End Class

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
            Public Property Type As Integer
            Public Property Mark As ExamResultItem
            Public Property MaxMark As Integer
        End Structure

    End Namespace
End Namespace
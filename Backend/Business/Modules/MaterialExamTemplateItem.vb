Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums

Namespace EGV
    Namespace Business

        Public Class MaterialExamTemplateItem

            Public Property MaterialId As Integer = 0
            Public Property TemplateId As Integer = 0
            Public Property TemplateItemId As Integer = 0
            Public Property MaxMark As Integer = 0
            Public Property TemplateItemTitle As String = String.Empty

        End Class

        Public Class MaterialExamTemplateItemController

            Public Shared Function GetMaxMark(ByVal materialId As Integer, ByVal itemId As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim type As ExamItemTypes = ExamTemplateItemController.GetItemType(itemId, conn)
                If type = ExamItemTypes.Number Then
                    Dim q As String = "SELECT MaxMark FROM MOD_MaterialExamTemplateItem WHERE MaterialId = @MId AND ExamTemplateItemId = @IId"
                    Dim val As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("MId", SqlDbType.Int, materialId), DBA.CreateParameter("IId", SqlDbType.Int, itemId)), ValueTypes.TypeInteger)
                    If val = 0 Then Return MaterialController.GetMaxMark(materialId, conn) Else Return val
                ElseIf type = ExamItemTypes.Total Then
                    Dim relatedIds = ExamTemplateItemController.GetRelatedIds(itemId, conn)
                    Dim total As Integer = 0
                    For Each item In relatedIds
                        total += GetMaxMark(materialId, item, conn)
                    Next
                    If total > 0 Then Return total Else Return MaterialController.GetMaxMark(materialId, conn)
                ElseIf type = ExamItemTypes.Average Then
                    Dim relatedIds = ExamTemplateItemController.GetRelatedIds(itemId, conn)
                    If relatedIds.Count > 0 Then
                        Dim total As Integer = 0
                        For Each item As Integer In relatedIds
                            total += GetMaxMark(materialId, item, conn)
                        Next
                        Dim avg = total / relatedIds.Count
                        If avg = 0 Then Return MaterialController.GetMaxMark(materialId, conn) Else Return avg.ToString("0,0.00")
                    Else
                        Return MaterialController.GetMaxMark(materialId, conn)
                    End If
                Else
                    Return MaterialController.GetMaxMark(materialId, conn)
                End If
            End Function

            Public Shared Function GetMaterialItems(ByVal materialId As Integer, Optional ByVal templateId As Integer = 0,
                                                    Optional ByVal conn As SqlConnection = Nothing,
                                                    Optional ByVal langId As Integer = 0) As List(Of MaterialExamTemplateItem)
                Helper.GetSafeLanguageId(langId)
                If templateId = 0 Then templateId = MaterialController.GetTemplateId(conn, materialId)
                Dim materialMaxMark As Integer = MaterialController.GetMaxMark(materialId, conn)
                Dim q As String = "SELECT R.Title, I.Id, I.TemplateId, M.MaxMark AS [ItemMaxMark] FROM MOD_ExamTemplateItem I INNER JOIN MOD_ExamTemplateItem_Res R ON I.Id = R.Id AND R.LanguageId = @LanguageId LEFT JOIN MOD_MaterialExamTemplateItem M ON I.Id = M.ExamTemplateItemId AND M.MaterialId = @MaterialId WHERE I.[Type] = @Type AND I.TemplateId = @TId"
                Dim lst As New List(Of MaterialExamTemplateItem)
                Using dt As DataTable = DBA.DataTable(conn, q,
                                                      DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                                                      DBA.CreateParameter("MaterialId", SqlDbType.Int, materialId),
                                                      DBA.CreateParameter("Type", SqlDbType.Int, ExamItemTypes.Number),
                                                      DBA.CreateParameter("TId", SqlDbType.Int, templateId)
                                                      )
                    For Each dr As DataRow In dt.Rows
                        Dim item As New MaterialExamTemplateItem() With {
                            .MaterialId = materialId,
                            .TemplateId = Helper.GetSafeDBValue(dr("TemplateId"), ValueTypes.TypeInteger),
                            .TemplateItemId = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                            .TemplateItemTitle = Helper.GetSafeDBValue(dr("Title")),
                            .MaxMark = Helper.GetSafeDBValue(dr("ItemMaxMark"), ValueTypes.TypeInteger)
                        }
                        If item.MaxMark = 0 Then item.MaxMark = materialMaxMark
                        lst.Add(item)
                    Next
                End Using
                Return lst
            End Function

            Public Shared Sub ClearMaterialItems(ByVal materialId As Integer, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "DELETE FROM MOD_MaterialExamTemplateItem WHERE MaterialId = @Id"
                If trans IsNot Nothing Then
                    DBA.NonQuery(trans, q, DBA.CreateParameter("Id", SqlDbType.Int, materialId))
                Else
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, materialId))
                End If
            End Sub

            Public Shared Sub AddMaterialItems(ByVal materialId As Integer, ByVal lst As List(Of MaterialExamTemplateItem),
                                               Optional ByVal conn As SqlConnection = Nothing, Optional ByVal trans As SqlTransaction = Nothing)
                ClearMaterialItems(materialId, conn, trans)
                Dim q As String = "INSERT INTO MOD_MaterialExamTemplateItem (MaterialId, ExamTemplateItemId, MaxMark) VALUES (@MaterialId, @ItemId, @MaxMark);"
                For Each item As MaterialExamTemplateItem In lst
                    Dim p() As SqlParameter = {
                        DBA.CreateParameter("MaterialId", SqlDbType.Int, materialId),
                        DBA.CreateParameter("ItemId", SqlDbType.Int, item.TemplateItemId),
                        DBA.CreateParameter("MaxMark", SqlDbType.Int, item.MaxMark)
                    }
                    If trans IsNot Nothing Then DBA.NonQuery(trans, q, p) Else DBA.NonQuery(conn, q, p)
                Next
            End Sub

            Public Shared Function GetTotalMaxMark(ByVal materialId As Integer, ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Decimal
                Dim q As String = "SELECT SUM(M.MaxMark) FROM MOD_MaterialExamTemplateItem M INNER JOIN MOD_TemplateItemRelation R ON M.ExamTemplateItemId = R.RelatedId WHERE R.SourceId = @TId AND M.MaterialId = @MId"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("TId", SqlDbType.Int, id), DBA.CreateParameter("MId", SqlDbType.Int, materialId)), ValueTypes.TypeDecimal)
            End Function

            Public Shared Function GetAverageMaxMark(ByVal materialId As Integer, ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Decimal
                Dim q As String = "SELECT AVG(M.MaxMark) FROM MOD_MaterialExamTemplateItem M INNER JOIN MOD_TemplateItemRelation R ON M.ExamTemplateItemId = R.RelatedId WHERE R.SourceId = @TId AND M.MaterialId = @MId"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("TId", SqlDbType.Int, id), DBA.CreateParameter("MId", SqlDbType.Int, materialId)), ValueTypes.TypeDecimal)
            End Function

        End Class

    End Namespace
End Namespace
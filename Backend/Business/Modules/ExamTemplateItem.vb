Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Interfaces

Namespace EGV
    Namespace Business

        Public Class ExamTemplateItem
            Inherits AudLocBusinessBase
            Implements ILocBusinessClass

#Region "Public Members"

            Public Property Id As Integer = 0 Implements ILocBusinessClass.Id
            Public Property TemplateId As Integer = 0
            Public Property Title As String = String.Empty Implements ILocBusinessClass.Title
            Public Property Type As ExamItemTypes = ExamItemTypes.Number

            Public Property RelatedIds As List(Of Integer)

#End Region

#Region "Filler"

            Public Overrides Sub FillObject(dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    TemplateId = Safe(dr("TemplateId"), ValueTypes.TypeInteger)
                    Title = Safe(dr("Title"))
                    Type = Safe(dr("Type"), ValueTypes.TypeInteger)
                    RelatedIds = ExamTemplateItemController.GetRelatedIds(Id, MyConn)
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal langId As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn, langId)
                RelatedIds = New List(Of Integer)()
                If tid > 0 Then
                    FillObject(DBA.SPDataRow(MyConn, "MOD_ExamTemplateItem_Get", DBA.CreateParameter("Id", SqlDbType.Int, tid), DBA.CreateParameter("LanguageId", SqlDbType.Int, MyLanguageId)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                ExamTemplateItemController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_ExamTemplateItem_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("TemplateId", SqlDbType.Int, TemplateId),
                                  DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                  DBA.CreateParameter("Type", SqlDbType.Int, Type),
                                  DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id)
                                  )
                ExamTemplateItemController.DeleteRelatedIds(Id, trans)
                If Type <> ExamItemTypes.Number AndAlso RelatedIds.Count > 0 Then
                    ExamTemplateItemController.AddRelatedIds(Id, RelatedIds, trans)
                End If
                InsertRes(trans)
            End Sub

            Public Overrides Sub InsertRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_ExamTemplateItem_AddRes"
                For Each lid As Integer In LanguageController.GetIds(MyConn, trans)
                    DBA.SPNonQuery(trans, sp,
                                 DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                 DBA.CreateParameter("Id", SqlDbType.Int, Id),
                                 DBA.CreateParameter("LanguageId", SqlDbType.Int, lid)
                                 )
                Next
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Translate(langId As Integer, userId As Integer, Optional trans As SqlTransaction = Nothing) Implements ILocBusinessClass.Translate
                Dim sp As String = "MOD_ExamTemplateItem_Translate"
                DBA.SPNonQuery(trans, sp,
                             DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                             DBA.CreateParameter("Id", SqlDbType.Int, Id),
                             DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                             DBA.CreateParameter("UserId", SqlDbType.Int, userId)
                             )
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_ExamTemplateItem_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("TemplateId", SqlDbType.Int, TemplateId),
                               DBA.CreateParameter("Type", SqlDbType.Int, Type),
                               DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
                ExamTemplateItemController.DeleteRelatedIds(Id, trans)
                If Type <> ExamItemTypes.Number AndAlso RelatedIds.Count > 0 Then
                    ExamTemplateItemController.AddRelatedIds(Id, RelatedIds, trans)
                End If
                If MyLanguageId = LanguageController.GetDefaultId(MyConn, trans) Then UpdateDefaultRes(trans)
                UpdateRes(trans)
            End Sub

            Public Overrides Sub UpdateDefaultRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_ExamTemplateItem_UpdateDefaultRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub UpdateRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_ExamTemplateItem_UpdateRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LangId", SqlDbType.Int, MyLanguageId)
                               )
            End Sub

#End Region

        End Class

        'controller
        Public Class ExamTemplateItemController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM MOD_ExamTemplateItem WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function DeleteTemplateItems(ByVal templateId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim valid As Boolean = True
                Using dt As DataTable = DBA.DataTable(conn, "SELECT Id FROM MOD_ExamTemplateItem WHERE TemplateId = @Id", DBA.CreateParameter("Id", SqlDbType.Int, templateId))
                    For Each dr As DataRow In dt.Rows
                        Dim id As Integer = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger)
                        If id > 0 Then
                            valid = valid And Delete(id, conn)
                        End If
                    Next
                End Using
                Return valid
            End Function

            Public Shared Function GetTemplateItems(ByVal templateId As Integer, Optional ByVal langId As Integer = 0, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal onlyNumber As Boolean = False, Optional ByVal targetItemId As Integer = 0, Optional ByVal onlyCalculated As Boolean = False) As List(Of ExamTemplateItem)
                Helper.GetSafeLanguageId(langId)
                Dim lst As New List(Of ExamTemplateItem)
                Using dt As DataTable = DBA.DataTable(conn, "SELECT Id FROM MOD_ExamTemplateItem WHERE TemplateId = @Id" & IIf(onlyNumber OrElse onlyCalculated, " AND [Type] " & IIf(onlyNumber, "=", "<>") & " @Type", "") & IIf(targetItemId > 0, " AND Id = " & targetItemId, ""), DBA.CreateParameter("Id", SqlDbType.Int, templateId), DBA.CreateParameter("Type", SqlDbType.Int, ExamItemTypes.Number))
                    For Each dr As DataRow In dt.Rows
                        Dim id As Integer = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger)
                        lst.Add(New ExamTemplateItem(id, langId, conn))
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function GetRelatedIds(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As List(Of Integer)
                Dim lst As New List(Of Integer)
                Using dt As DataTable = DBA.DataTable(conn, "SELECT RelatedId FROM MOD_TemplateItemRelation WHERE SourceId = @Id", DBA.CreateParameter("Id", SqlDbType.Int, id))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(Helper.GetSafeDBValue(dr("RelatedId"), ValueTypes.TypeInteger))
                    Next
                End Using
                Return lst
            End Function

            Public Shared Sub DeleteRelatedIds(ByVal id As Integer, Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "DELETE FROM MOD_TemplateItemRelation WHERE SourceId = @Id"
                DBA.NonQuery(trans, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

            Public Shared Sub AddRelatedIds(ByVal id As Integer, ByVal lst As List(Of Integer), Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "INSERT INTO MOD_TemplateItemRelation (SourceId, RelatedId) VALUES (@Id, @RelateId)"
                For Each i As Integer In lst
                    If i <> id Then
                        DBA.NonQuery(trans, q, DBA.CreateParameter("Id", SqlDbType.Int, id), DBA.CreateParameter("RelateId", SqlDbType.Int, i))
                    End If
                Next
            End Sub

            Public Shared Function GetItemType(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As ExamItemTypes
                Dim q As String = "SELECT [Type] FROM MOD_ExamTemplateItem WHERE Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

#Region "Private Methods"

            Public Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not (HasExams(id, conn))
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM MOD_ExamTemplateItem_Res WHERE Id = @Id"
                Dim p As SqlParameter = DBA.CreateParameter("Id", SqlDbType.Int, id)
                DBA.NonQuery(conn, q, p)
                q = "DELETE FROM MOD_TemplateItemRelation WHERE RelatedId = @Id"
                DBA.NonQuery(conn, q, p)
                q = "DELETE FROM MOD_TemplateItemRelation WHERE SourceId = @Id"
                DBA.NonQuery(conn, q, p)
            End Sub

            Private Shared Function HasExams(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_StudentExam WHERE ExamId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id)) > 0
            End Function

#End Region

        End Class

    End Namespace
End Namespace
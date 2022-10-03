Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Interfaces

Namespace EGV
    Namespace Business

        'object
        Public Class ExamTemplate
            Inherits AudLocBusinessBase
            Implements ILocBusinessClass

#Region "Public Properties"

            Public Property Id As Integer = 0 Implements ILocBusinessClass.Id
            Public Property Title As String = String.Empty Implements ILocBusinessClass.Title
            Public Property MaxMark As Integer = 0

            Public Property Items As List(Of ExamTemplateItem)

#End Region

#Region "Filler"

            Public Overrides Sub FillObject(dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    Title = Safe(dr("Title"))
                    MaxMark = Safe(dr("MaxMark"))
                    Items = ExamTemplateItemController.GetTemplateItems(Id, MyLanguageId, MyConn)
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal langId As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn, langId)
                Items = New List(Of ExamTemplateItem)()
                If tid > 0 Then
                    FillObject(DBA.SPDataRow(MyConn, "MOD_ExamTemplate_Get", DBA.CreateParameter("Id", SqlDbType.Int, tid), DBA.CreateParameter("LanguageId", SqlDbType.Int, MyLanguageId)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                ExamTemplateController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_ExamTemplate_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                  DBA.CreateParameter("MaxMark", SqlDbType.Int, MaxMark),
                                  DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id)
                                  )
                InsertRes(trans)
            End Sub

            Public Overrides Sub InsertRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_ExamTemplate_AddRes"
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
                Dim sp As String = "MOD_ExamTemplate_Translate"
                DBA.SPNonQuery(trans, sp,
                             DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                             DBA.CreateParameter("Id", SqlDbType.Int, Id),
                             DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                             DBA.CreateParameter("UserId", SqlDbType.Int, userId)
                             )
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_ExamTemplate_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("MaxMark", SqlDbType.Int, MaxMark),
                               DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
                If MyLanguageId = LanguageController.GetDefaultId(MyConn, trans) Then UpdateDefaultRes(trans)
                UpdateRes(trans)
            End Sub

            Public Overrides Sub UpdateDefaultRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_ExamTemplate_UpdateDefaultRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub UpdateRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_ExamTemplate_UpdateRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LangId", SqlDbType.Int, MyLanguageId)
                               )
            End Sub

#End Region

        End Class

        'controller
        Public Class ExamTemplateController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    If ExamTemplateItemController.DeleteTemplateItems(id, conn) Then
                        Dim q As String = "DELETE FROM MOD_ExamTemplate WHERE Id = @Id"
                        DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                        DeleteRelated(id, conn)
                        Return True
                    Else
                        Return False
                    End If
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetCollection(ByVal conn As SqlConnection) As DBAReturnObject
                Dim ret As New DBAReturnObject
                Dim q As String = "SELECT Id, Title, MaxMark FROM MOD_ExamTemplate"
                Dim dt As DataTable = DBA.DataTable(conn, q)
                ret.Query = q
                ret.Count = dt.Rows.Count
                ret.List = dt
                Return ret
            End Function

            Public Shared Function GetMaxMark(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Dim q As String = "SELECT MaxMark FROM MOD_ExamTemplate WHERE Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not (HasClass(id, conn) OrElse HasMaterial(id, conn))
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM MOD_ExamTemplate_Res WHERE Id = @Id"
                Dim p As SqlParameter = DBA.CreateParameter("Id", SqlDbType.Int, id)
                DBA.NonQuery(conn, q, p)
            End Sub

            Private Shared Function HasClass(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_ClassTemplate WHERE TemplateId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id)) > 0
            End Function

            Private Shared Function HasMaterial(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_Material WHERE ExamTemplateId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id)) > 0
            End Function

            Private Shared Function HasItems(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim valid As Boolean = False
                Using dt As DataTable = DBA.DataTable(conn, "SELECT Id FROM MOD_ExamTemplateItem WHERE TemplateId = @Id", DBA.CreateParameter("Id", SqlDbType.Int, id))
                    For Each dr As DataRow In dt.Rows
                        Dim tid As Integer = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger)
                        valid = valid Or ExamTemplateItemController.AllowDelete(tid, conn)
                    Next
                End Using
                Return valid
            End Function

#End Region

        End Class

    End Namespace
End Namespace
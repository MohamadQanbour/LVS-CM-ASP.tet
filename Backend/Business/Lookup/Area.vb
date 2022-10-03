Imports System.Data.SqlClient
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Utils
Imports EGV.Interfaces

Namespace EGV
    Namespace Business

        'Object
        Public Class Area
            Inherits LocBusinessBase
            Implements ILocBusinessClass

#Region "Public Properties"

            Public Property Id As Integer = 0 Implements ILocBusinessClass.Id
            Public Property Title As String = String.Empty Implements ILocBusinessClass.Title

#End Region

#Region "Filler"

            Public Overrides Sub FillObject(dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    Title = Safe(dr("Title"), ValueTypes.TypeString)
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal langId As Integer = 0)
                MyBase.New(conn, langId)
                If tid > 0 Then
                    Dim sp As String = "LOK_Area_Get"
                    FillObject(DBA.SPDataRow(MyConn, sp,
                                           DBA.CreateParameter("Id", SqlDbType.Int, tid),
                                           DBA.CreateParameter("LangId", SqlDbType.Int, MyLanguageId)
                                           ))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                AreaController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "LOK_Area_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50)
                                  )
                InsertRes(trans)
            End Sub

            Public Overrides Sub InsertRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "LOK_Area_AddRes"
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
                Dim sp As String = "LOK_Area_Translate"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                               DBA.CreateParameter("UserId", SqlDbType.Int, userId)
                               )
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                If MyLanguageId = LanguageController.GetDefaultId(MyConn, trans) Then UpdateDefaultRes(trans)
                UpdateRes(trans)
            End Sub

            Public Overrides Sub UpdateDefaultRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "LOK_Area_UpdateDefaultRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub UpdateRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "LOK_Area_UpdateRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LangId", SqlDbType.Int, MyLanguageId)
                               )
            End Sub

#End Region

        End Class

        'controller
        Public Class AreaController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    DeleteDependencies(id, conn)
                    Dim q As String = "DELETE FROM LOK_Area WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetCollection(ByVal conn As SqlConnection, ByVal langId As Integer, Optional ByVal limit As Integer = 0,
                                                 Optional ByVal search As String = "") As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Helper.GetSafeLanguageId(langId)
                Dim cq As New CustomQuery("LOK_Area_Res", "A", conn)
                cq.AddCondition("A.LanguageId = @LangId")
                cq.AddParameter(DBA.CreateParameter("LangId", SqlDbType.Int, langId))
                If limit > 0 Then
                    cq.PageSize = limit
                    cq.PageIndex = 0
                    cq.EnablePaging = True
                Else
                    cq.EnablePaging = False
                End If
                If search <> String.Empty Then
                    cq.AddCondition("A.Title LIKE '%' + @Query + '%'")
                    cq.AddParameter(DBA.CreateParameter("Query", SqlDbType.NVarChar, search, 255))
                End If
                ret.Query = cq.GetQuery()
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

            Public Shared Function GetId(ByVal title As String, ByVal langId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Dim q As String = "SELECT Id FROM LOK_Area_Res WHERE Title = @Title AND LanguageId = @LangId"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Title", SqlDbType.NVarChar, title, 50), DBA.CreateParameter("LangId", SqlDbType.Int, langId)), ValueTypes.TypeInteger)
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not HasStudents(id, conn)
            End Function

            Private Shared Sub DeleteDependencies(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM LOK_Area_Res WHERE Id = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

            Private Shared Function HasStudents(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_Student WHERE AreaId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
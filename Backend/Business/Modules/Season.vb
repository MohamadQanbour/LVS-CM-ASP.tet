Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Structures
Imports EGV.Enums
Imports EGV.Interfaces

Namespace EGV
    Namespace Business

        'object
        Public Class Season
            Inherits AudLocBusinessBase
            Implements ILocBusinessClass

#Region "Public Members"

            Public Property Id As Integer = 0 Implements ILocBusinessClass.Id
            Public Property Title As String = String.Empty Implements ILocBusinessClass.Title
            Public Property IsCurrent As Boolean = False

#End Region

#Region "Filler"

            Public Overrides Sub FillObject(dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    Title = Safe(dr("Title"))
                    IsCurrent = Safe(dr("IsCurrent"), ValueTypes.TypeBoolean)
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal langId As Integer = 0)
                MyBase.New(conn, langId)
                If tid > 0 Then
                    FillObject(DBA.SPDataRow(conn, "MOD_Season_Get", DBA.CreateParameter("Id", SqlDbType.Int, tid), DBA.CreateParameter("LanguageId", SqlDbType.Int, MyLanguageId)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                SeasonController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Season_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                  DBA.CreateParameter("IsCurrent", SqlDbType.Bit, IsCurrent),
                                  DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id)
                                  )
                InsertRes(trans)
            End Sub

            Public Overrides Sub InsertRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Season_AddRes"
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
                Dim sp As String = "MOD_Season_Translate"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                               DBA.CreateParameter("UserId", SqlDbType.Int, userId)
                               )
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Season_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("IsCurrent", SqlDbType.Bit, IsCurrent),
                               DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
                If MyLanguageId = LanguageController.GetDefaultId(MyConn, trans) Then UpdateDefaultRes(trans)
                UpdateRes(trans)
            End Sub

            Public Overrides Sub UpdateDefaultRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Season_UpdateDefaultRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub UpdateRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Season_UpdateRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LangId", SqlDbType.Int, MyLanguageId)
                               )
            End Sub

#End Region

        End Class

        'controller
        Public Class SeasonController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM MOD_Season WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Sub SetCurrent(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE MOD_Season SET IsCurrent = @IsCurrent"
                DBA.NonQuery(conn, q, DBA.CreateParameter("IsCurrent", SqlDbType.Bit, False))
                q = "UPDATE MOD_Season SET IsCurrent = @Current, ModifiedDate = GETDATE(), ModifiedUser = @UserId WHERE Id = @Id"
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("Current", SqlDbType.Bit, True),
                             DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id),
                             DBA.CreateParameter("Id", SqlDbType.Int, id)
                             )
            End Sub

            Public Shared Function GetCollection(ByVal conn As SqlConnection, ByVal langId As Integer, Optional ByVal notCurrent As Boolean = False) As DBAReturnObject
                Helper.GetSafeLanguageId(langId)
                Dim ret As New DBAReturnObject()
                Dim q As String = "SELECT R.*, S.IsCurrent FROM MOD_Season S INNER JOIN MOD_Season_Res R ON S.Id = R.Id AND R.LanguageId = @LanguageId"
                If notCurrent Then q &= " WHERE S.IsCurrent = @NotCurrent"
                Dim dt As DataTable = DBA.DataTable(conn, q,
                                                    DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                                                    DBA.CreateParameter("NotCurrent", SqlDbType.Bit, False)
                                                    )
                ret.Query = q
                ret.Count = dt.Rows.Count
                ret.List = dt
                Return ret
            End Function

            Public Shared Function GetCurrentId(ByVal conn As SqlConnection) As Integer
                Dim q As String = "SELECT Id FROM MOD_Season WHERE IsCurrent = @Current"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Current", SqlDbType.Bit, True))
            End Function

            Public Shared Function GetCurrent(ByVal conn As SqlConnection, ByVal langId As Integer) As Season
                Helper.GetSafeLanguageId(langId)
                Dim ret As Season = New Season(GetCurrentId(conn), conn, langId)
                Return ret
            End Function

            Public Shared Function GetTitle(ByVal id As Integer, ByVal langId As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim q As String = "SELECT Title FROM MOD_Season_Res WHERE Id = @Id AND LanguageId = @LanguageId"
                Return DBA.Scalar(conn, q,
                                  DBA.CreateParameter("Id", SqlDbType.Int, id),
                                  DBA.CreateParameter("LanguageId", SqlDbType.Int, langId)
                                  )
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not (HasStudentTests(id, conn) OrElse HasSections(id, conn) OrElse HasAttendance(id, conn))
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM MOD_Season_Res WHERE Id = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

            Private Shared Function HasStudentTests(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_MaterialStudentTest WHERE SeasonId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Private Shared Function HasSections(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_Section WHERE SeasonId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Private Shared Function HasAttendance(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_StudentAttendance WHERE SeasonId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
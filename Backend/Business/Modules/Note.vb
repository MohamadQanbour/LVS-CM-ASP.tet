Imports EGV.Enums
Imports EGV.Utils
Imports EGV.Structures
Imports System.Data.SqlClient

Namespace EGV
    Namespace Business

        'object
        Public Class Note
            Inherits AudBusinessBase

#Region "Public Properties"

            Public Property Id As Integer = 0
            Public Property SenderId As Integer = 0
            Public Property StudentId As Integer = 0
            Public Property NoteType As NoteTypes = NoteTypes.Negative
            Public Property NoteDate As Date
            Public Property NoteText As String = String.Empty

#End Region

#Region "Filler"

            Public Overrides Sub FillObject(dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    SenderId = Safe(dr("SenderId"), ValueTypes.TypeInteger)
                    StudentId = Safe(dr("StudentId"), ValueTypes.TypeInteger)
                    NoteType = Safe(dr("NoteType"), ValueTypes.TypeInteger)
                    NoteDate = Safe(dr("NoteDate"), ValueTypes.TypeDate)
                    NoteText = Safe(dr("NoteText"))
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    FillObject(DBA.SPDataRow(MyConn, "MOD_Note_Get", DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Note_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("SenderId", SqlDbType.Int, SenderId),
                                  DBA.CreateParameter("StudentId", SqlDbType.Int, StudentId),
                                  DBA.CreateParameter("NoteType", SqlDbType.Int, NoteType),
                                  DBA.CreateParameter("NoteDate", SqlDbType.Date, NoteDate),
                                  DBA.CreateParameter("NoteText", SqlDbType.NText, NoteText),
                                  DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id)
                                  )
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Note_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("SenderId", SqlDbType.Int, SenderId),
                               DBA.CreateParameter("StudentId", SqlDbType.Int, StudentId),
                               DBA.CreateParameter("NoteType", SqlDbType.Int, NoteType),
                               DBA.CreateParameter("NoteDate", SqlDbType.Date, NoteDate),
                               DBA.CreateParameter("NoteText", SqlDbType.NText, NoteText),
                               DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                NoteController.Delete(Id, conn)
            End Sub

#End Region

        End Class

        'Controller
        Public Class NoteController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM MOD_Note WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetStudentNotes(ByVal studentId As Integer, Optional ByVal conn As SqlConnection = Nothing) As DBAReturnObject
                Dim ret As New DBAReturnObject
                Dim q As String = "SELECT N.*, S.UserName, ST.SchoolId, ST.FullName AS [StudentName] FROM MOD_Note N INNER JOIN MEM_Student ST ON ST.Id = N.StudentId INNER JOIN SYS_User S ON S.Id = N.SenderId WHERE N.StudentId = @Id ORDER BY N.NoteDate DESC"
                ret.Query = q
                Dim dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId))
                ret.List = dt
                ret.Count = dt.Rows.Count
                Return ret
            End Function

            Public Shared Function GetFamilyNotes(ByVal familyId As Integer, Optional ByVal conn As SqlConnection = Nothing) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim q As String = "SELECT N.*, S.UserName, ST.SchoolId, ST.FullName AS [StudentName] FROM MOD_Note N INNER JOIN MEM_Student ST ON ST.Id = N.StudentId INNER JOIN SYS_User S ON S.Id = N.SenderId WHERE ST.FamilyId = @Id"
                ret.Query = q
                Dim dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, familyId))
                ret.List = dt
                ret.Count = dt.Rows.Count
                Return ret
            End Function

            Public Shared Function GetNegativeTotal(ByVal conn As SqlConnection) As Integer
                Dim q As String = "SELECT COUNT(*) FROM MOD_Note WHERE NoteType = @Type"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Type", SqlDbType.Int, NoteTypes.Negative))
            End Function

            Public Shared Function GetPositiveTotal(ByVal conn As SqlConnection) As Integer
                Dim q As String = "SELECT COUNT(*) FROM MOD_Note WHERE NoteType = @Type"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Type", SqlDbType.Int, NoteTypes.Positive))
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return True 'all notes can be deleted
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                'no related items
            End Sub

#End Region

        End Class

    End Namespace
End Namespace
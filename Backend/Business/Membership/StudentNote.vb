Imports EGV.Enums
Imports EGV.Structures
Imports System.Data.SqlClient

Namespace EGV
    Namespace Business

        Public Class StudentNote
            Inherits BusinessBase

#Region "Public Members"

            Public Property Id As Integer = 0
            Public Property StudentId As Integer = 0
            Public Property Note As String = String.Empty
            Public Property NoteDate As DateTime = Now
            Public Property UserId As Integer = 0

#End Region

#Region "Filler"

            Public Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    StudentId = Safe(dr("StudentId"), ValueTypes.TypeInteger)
                    Note = Safe(dr("Note"))
                    NoteDate = Safe(dr("NoteDate"), ValueTypes.TypeDateTime)
                    UserId = Safe(dr("UserId"), ValueTypes.TypeInteger)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    FillObject(DBA.DataRow(MyConn, "SELECT * FROM MEM_StudentNote WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                StudentNoteController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim q As String = "INSERT INTO MEM_StudentNote (StudentId, Note, NoteDate, UserId) VALUES (@StudentId, @Note, @NoteDate, @UserId);"
                Id = DBA.ScalarID(trans, q,
                                  DBA.CreateParameter("StudentId", SqlDbType.Int, StudentId),
                                  DBA.CreateParameter("Note", SqlDbType.NText, Note),
                                  DBA.CreateParameter("NoteDate", SqlDbType.Date, NoteDate),
                                  DBA.CreateParameter("UserId", SqlDbType.Int, UserId)
                                  )
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)

            End Sub

#End Region

        End Class

        Public Class StudentNoteController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    DBA.NonQuery(conn, "DELETE FROM MEM_StudentNote WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetCollection(ByVal conn As SqlConnection, ByVal studentId As Integer) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim cq As New CustomQuery("MEM_StudentNote", "S", conn)
                cq.AddColumn("Note", "S")
                cq.AddColumn("NoteDate", "S")
                cq.AddColumn("FullName", "P")
                cq.AddJoinTable("SYS_UserProfile", "P", "UserId", "UserId", TableJoinTypes.Inner)
                cq.AddCondition("S.StudentId = @StudentId")
                cq.AddParameter(DBA.CreateParameter("StudentId", SqlDbType.Int, studentId))
                cq.AddSortColumn("NoteDate", SortDirections.Descending, "S")
                ret.Query = cq.GetQuery()
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return True
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)

            End Sub

#End Region

        End Class

    End Namespace
End Namespace
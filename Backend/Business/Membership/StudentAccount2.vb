Imports EGV.Enums
Imports System.Data.SqlClient

Namespace EGV
    Namespace Business

        Public Class StudentAccount2
            Inherits BusinessBase

#Region "Properties"

            Public Property Id As Integer = 0
            Public Property StudentId As Integer = 0
            Public Property ClassId As Integer = 0
            Public Property RequestedAmount As Decimal = 0D
            Public Property Balance As Decimal = 0D
            Public Property LastUpdate As Date = Now
            Public Property LastUpdateUser As Integer = 0
            Public Property LastUpdateUserName As String = String.Empty

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    StudentId = Safe(dr("StudentId"), ValueTypes.TypeInteger)
                    ClassId = Safe(dr("ClassId"), ValueTypes.TypeInteger)
                    RequestedAmount = Safe(dr("RequestedAmount"), ValueTypes.TypeDecimal)
                    Balance = Safe(dr("Balance"), ValueTypes.TypeDecimal)
                    LastUpdate = Safe(dr("LastUpdate"), ValueTypes.TypeDateTime)
                    LastUpdateUser = Safe(dr("LastUpdateUser"), ValueTypes.TypeInteger)
                    LastUpdateUserName = Safe(dr("LastUpdateUserName"))
                End If
            End Sub

#End Region

#Region "Constructor"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    FillObject(DBA.DataRow(MyConn, "SELECT M.*, U.FullName AS [LastUpdateUserName] FROM MEM_StudentAccount2 M WITH (NOLOCK) INNER JOIN SYS_UserProfile U WITH (NOLOCK) ON M.LastUpdateUser = U.UserId WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                StudentAccount2Controller.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MEM_StudentAccount2_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("StudentId", SqlDbType.Int, StudentId),
                                  DBA.CreateParameter("ClassId", SqlDbType.Int, ClassId),
                                  DBA.CreateParameter("RequestedAmount", SqlDbType.Decimal, RequestedAmount, 18, 2),
                                  DBA.CreateParameter("Balance", SqlDbType.Decimal, Balance, 18, 2),
                                  DBA.CreateParameter("UserId", SqlDbType.Int, LastUpdateUser)
                                  )
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                Insert(trans)
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MEM_StudentAccount2_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("StudentId", SqlDbType.Int, StudentId),
                               DBA.CreateParameter("ClassId", SqlDbType.Int, ClassId),
                               DBA.CreateParameter("RequestedAmount", SqlDbType.Decimal, RequestedAmount, 18, 2),
                               DBA.CreateParameter("Balance", SqlDbType.Decimal, Balance, 18, 2),
                               DBA.CreateParameter("UserId", SqlDbType.Int, LastUpdateUser)
                               )
            End Sub

#End Region

        End Class

        Public Class StudentAccount2Controller

            Private Shared Function AllowDelete(ByVal id As Integer, ByVal conn As SqlConnection) As Boolean
                Return True
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, ByVal conn As SqlConnection)

            End Sub

            Public Shared Function ReadField(ByVal id As Integer, ByVal field As String, Optional ByVal type As ValueTypes = ValueTypes.TypeString, Optional ByVal conn As SqlConnection = Nothing) As Object
                Dim q As String = String.Format("SELECT {0} FROM {1} WITH (NOLOCK) WHERE Id = {2}", field, "MEM_StudentAccount2", id)
                Return Utils.Helper.GetSafeDBValue(DBA.Scalar(conn, q), type)
            End Function

            Public Shared Sub DeleteAll(ByVal studentId As Integer, ByVal conn As SqlConnection)
                Dim q As String = "DELETE MEM_StudentAccount2 WHERE StudentId = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId))
            End Sub

            Public Shared Function GetLastEntry(ByVal studentId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Dim q As String = "SELECT Id FROM MEM_StudentAccount2 WITH (NOLOCK) WHERE StudentId = @Id AND LastEntry = 1"
                Return Utils.Helper.GetSafeDBInteger(DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId)))
            End Function

            Public Shared Function Delete(ByVal id As Integer, ByVal conn As SqlConnection) As Boolean
                If AllowDelete(id, conn) Then
                    Dim lastEntry As Boolean = ReadField(id, "LastEntry", ValueTypes.TypeBoolean, conn)
                    If lastEntry Then
                        Dim studentId As Integer = ReadField(id, "StudentId", ValueTypes.TypeInteger, conn)
                        Dim lastId As Integer = Utils.Helper.GetSafeDBInteger(DBA.Scalar(conn, "SELECT TOP 1 Id FROM MEM_StudentAccount2 WITH (NOLOCK) WHERE StudentId = @StudentId AND Id <> @Id ORDER BY LastUpdate DESC", DBA.CreateParameter("StudentId", SqlDbType.Int, studentId), DBA.CreateParameter("Id", SqlDbType.Int, id)))
                        DBA.NonQuery(conn, "UPDATE MEM_StudentAccount2 SET LastEntry = 1 WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, lastId))
                    End If
                    DBA.NonQuery(conn, "DELETE FROM MEM_StudentAccount2 WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, id))
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function AccountAvailable(ByVal studentId As Integer, ByVal conn As SqlConnection, Optional ByVal trans As SqlTransaction = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_StudentAccount2 WITH (NOLOCK) WHERE StudentId = @Id"
                If trans IsNot Nothing Then
                    Return Utils.Helper.GetSafeDBValue(DBA.Scalar(trans, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId)), ValueTypes.TypeInteger) > 0
                Else
                    Return Utils.Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId)), ValueTypes.TypeInteger) > 0
                End If
            End Function

            Public Shared Function GetByStudentId(ByVal studentId As Integer, Optional ByVal conn As SqlConnection = Nothing) As StudentAccount2
                Dim q As String = "SELECT Id FROM MEM_StudentAccount2 WITH (NOLOCK) WHERE StudentId = @Id ORDER BY LastUpdate DESC"
                Dim id As Integer = Utils.Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId)), ValueTypes.TypeInteger)
                If id > 0 Then Return New StudentAccount2(id, conn) Else Return Nothing
            End Function

            Public Shared Function GetCollection(ByVal conn As SqlConnection, ByVal studentId As Integer) As DataSet
                Dim q As String = "SELECT S.*, U.FullName, (S.RequestedAmount - S.Balance) AS PaidAmount, ClassId FROM MEM_StudentAccount2 S WITH (NOLOCK) INNER JOIN SYS_UserProfile U WITH (NOLOCK) ON S.LastUpdateUser = U.UserId WHERE S.StudentId = @Id ORDER BY S.LastUpdate DESC"
                Return DBA.DataSet(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId))
            End Function

        End Class

    End Namespace
End Namespace
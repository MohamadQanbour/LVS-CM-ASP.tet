Imports System.Data.SqlClient
Imports System.Data

Namespace EGV
    Namespace Business

        'Object
        Public Class EGVException
            Inherits PrimeBusinessBase

#Region "Public Properties"

            Public Property Id As Integer
            Public Property Message As String
            Public Property StackTrace As String
            Public Property RecordDate As DateTime

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), Enums.ValueTypes.TypeInteger)
                    Message = Safe(dr("Message"), Enums.ValueTypes.TypeString)
                    StackTrace = Safe(dr("StackTrace"), Enums.ValueTypes.TypeString)
                    RecordDate = Safe(dr("RecordDate"), Enums.ValueTypes.TypeDateTime)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    FillObject(DBA.DataRow(MyConn, "SELECT * FROM SYS_Exception WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

        End Class

        'Controller
        Public Class EGVExceptionController

            Public Shared Sub AddException(ByVal e As Exception, ByVal trans As SqlTransaction)
                Dim sp As String = "SYS_Exception_Add"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("@Message", SqlDbType.NText, e.Message),
                               DBA.CreateParameter("@StackTrace", SqlDbType.NText, e.StackTrace)
                               )
            End Sub

            Public Shared Sub AddException(ByVal e As Exception, Optional ByVal conn As SqlConnection = Nothing)
                Dim sp As String = "SYS_Exception_Add"
                DBA.SPNonQuery(conn, sp,
                               DBA.CreateParameter("@Message", SqlDbType.NText, e.Message),
                               DBA.CreateParameter("@StackTrace", SqlDbType.NText, e.StackTrace)
                               )
            End Sub

            Public Shared Sub PurgeExceptions(Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "TRUNCATE TABLE SYS_Exception"
                DBA.NonQuery(conn, q)
            End Sub

            Public Shared Sub DeleteException(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM SYS_Exception WHERE Id = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

            Public Shared Function List(Optional ByVal conn As SqlConnection = Nothing) As Structures.DBAReturnObject
                Dim ret As New Structures.DBAReturnObject()
                Dim obj As New EntityStructure("system/Exception")
                Dim cq As CustomQuery = obj.GetCustomQuery(conn)
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

        End Class

    End Namespace
End Namespace
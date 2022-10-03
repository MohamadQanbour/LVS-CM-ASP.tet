Imports System.Data.SqlClient
Imports EGV
Imports EGV.Utils

Namespace EGV
    Namespace Business

        'Object
        Public Class EmailKey
            Inherits PrimeBusinessBase

#Region "Public Properties"

            Public Property Id As Integer = 0
            Public Property Title As String = String.Empty

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), Enums.ValueTypes.TypeInteger)
                    Title = Safe(dr("Title"), Enums.ValueTypes.TypeString)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    FillObject(DBA.DataRow(MyConn, "SELECT * FROM SYS_EmailKey WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

        End Class

        'controller
        Public Class EmailKeyController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM SYS_EmailKey WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function Add(ByVal title As String, Optional ByVal trans As SqlTransaction = Nothing) As Integer
                Dim q As String = "INSERT INTO SYS_EmailKey (Title) VALUES (@Title)"
                If trans IsNot Nothing Then
                    Return DBA.ScalarID(trans, q, DBA.CreateParameter("Title", SqlDbType.NVarChar, title, 50))
                Else
                    Return DBA.ScalarID(DBA.GetConn(), q, DBA.CreateParameter("Title", SqlDbType.NVarChar, title, 50))
                End If
            End Function

            Public Shared Sub Update(ByVal id As Integer, ByVal title As String, Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "UPDATE SYS_EmailKey SET Title = @Title WHERE Id = @Id"
                DBA.NonQuery(trans, q,
                             DBA.CreateParameter("Title", SqlDbType.NVarChar, title, 50),
                             DBA.CreateParameter("Id", SqlDbType.Int, id)
                             )
            End Sub

            Public Shared Function List(Optional ByVal conn As SqlConnection = Nothing) As Structures.DBAReturnObject
                Dim ret As New Structures.DBAReturnObject()
                Dim q As String = "SELECT * FROM SYS_EmailKey"
                Dim dt = DBA.DataTable(conn, q)
                ret.Query = q
                ret.Count = dt.Rows.Count
                ret.List = dt
                Return ret
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return True
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM SYS_Email WHERE KeyId = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

#End Region

        End Class

    End Namespace
End Namespace
Imports System.Data
Imports System.Data.SqlClient

Namespace EGV
    Namespace Business

        'Object
        Public Class Permission
            Inherits BusinessBase

#Region "Public Properties"

            Public Property Id As Integer
            Public Property Title As String

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
                    FillObject(DBA.DataRow(MyConn, "SELECT * FROM SYS_Permission WHERE Id = @Id", DBA.CreateParameter("@Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_Permission_Add"
                Id = DBA.SPScalar(trans, sp, DBA.CreateParameter("Title", SqlDbType.VarChar, Title, 50))
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_Permission_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.VarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                PermissionController.Delete(Id, conn)
            End Sub

#End Region

        End Class

        'Controller

        Public Class PermissionController

#Region "Public Methods"

            Public Shared Function List(ByVal conn As SqlConnection) As Structures.DBAReturnObject
                Dim q As String = "SELECT * FROM SYS_Permission"
                Dim ret As New Structures.DBAReturnObject()
                Dim dt As DataTable = DBA.DataTable(conn, q)
                ret.Query = q
                ret.List = dt
                ret.Count = dt.Rows.Count
                Return ret
            End Function

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELTE FROM SYS_Permission WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    Return True
                Else
                    Return False
                End If
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not (HasRoles(id, conn) OrElse HasMenus(id, conn))
            End Function

            Private Shared Function HasRoles(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM SYS_RolePermission WHERE PermissionId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id)) > 0
            End Function

            Private Shared Function HasMenus(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM SYS_Menu WHERE PermissionId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
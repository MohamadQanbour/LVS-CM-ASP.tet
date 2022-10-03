Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Enums

Namespace EGV
    Namespace Business

        'Object
        Public Class Role
            Inherits BusinessBase

#Region "Public Properties"

            Public Property Id As Integer
            Public Property Title As String
            Public Property IsSecure As Boolean
            Public Property IsActive As Boolean
            Public Property IsAdmin As Boolean

            Public Property Permissions As List(Of RolePermission)
            Public Property Languages As List(Of Integer)

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    Title = Safe(dr("Title"), ValueTypes.TypeString)
                    IsSecure = Safe(dr("IsSecure"), ValueTypes.TypeBoolean)
                    IsActive = Safe(dr("IsActive"), ValueTypes.TypeBoolean)
                    IsAdmin = Safe(dr("IsAdmin"), ValueTypes.TypeBoolean)
                    Permissions = RolePermissionController.GetRolePermissionsList(Id, MyConn)
                    Languages = RoleController.GetAllowedLanguages(Id, MyConn)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                Permissions = New List(Of RolePermission)()
                Languages = New List(Of Integer)
                If tid > 0 Then
                    FillObject(DBA.DataRow(MyConn, "SELECT * FROM SYS_Role WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_Role_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                  DBA.CreateParameter("IsSecure", SqlDbType.Bit, IsSecure),
                                  DBA.CreateParameter("IsActive", SqlDbType.Bit, IsActive),
                                  DBA.CreateParameter("IsAdmin", SqlDbType.Bit, IsAdmin)
                                  )
                RolePermissionController.SaveRolePermissionsList(Id, Permissions, trans)
                RoleController.AddAllowedLanguages(Id, Languages, trans)
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_Role_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("IsSecure", SqlDbType.Bit, IsSecure),
                               DBA.CreateParameter("IsActive", SqlDbType.Bit, IsActive),
                               DBA.CreateParameter("IsAdmin", SqlDbType.Bit, IsAdmin),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
                RolePermissionController.SaveRolePermissionsList(Id, Permissions, trans)
                RoleController.DeleteAllowedLanguages(Id, trans)
                RoleController.AddAllowedLanguages(Id, Languages, trans)
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                RoleController.Delete(Id, conn)
            End Sub

#End Region

        End Class

        'Controller
        Public Class RoleController

#Region "Public Methods"

            Public Shared Function List(Optional ByVal conn As SqlConnection = Nothing, Optional ByVal onlyNotSecure As Boolean = False,
                                        Optional ByVal search As String = "") As Structures.DBAReturnObject
                Dim ret As New Structures.DBAReturnObject
                Dim q As String = "SELECT * FROM SYS_Role WHERE IsActive = 1{0}"
                If search <> String.Empty Then q = String.Format(q, " AND Title LIKE N'%' + @Search + N'%'") Else q = String.Format(q, "")
                If onlyNotSecure Then q &= " AND IsSecure = 0"
                Dim dt = DBA.DataTable(conn, q, DBA.CreateParameter("Search", SqlDbType.NVarChar, search, 255))
                ret.Query = q
                ret.Count = dt.Rows.Count
                ret.List = dt
                Return ret
            End Function

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM SYS_Role WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Sub ToggleState(ByVal id As Integer, Optional ByVal activate As Boolean = True, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE SYS_Role SET IsActive = @Active WHERE Id = @Id"
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("Active", SqlDbType.Bit, activate),
                             DBA.CreateParameter("Id", SqlDbType.Int, id)
                             )
            End Sub

            Public Shared Function GetAllowedLanguages(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As List(Of Integer)
                Dim q As String = "SELECT LanguageId FROM SYS_RoleLanguage WHERE RoleId = @Id"
                Dim lst As New List(Of Integer)
                Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(dr("LanguageId"))
                    Next
                End Using
                Return lst
            End Function

            Public Shared Sub AddAllowedLanguages(ByVal id As Integer, ByVal languages As List(Of Integer), Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "INSERT INTO SYS_RoleLanguage (RoleId, LanguageId) VALUES (@RoleId, @LanguageId)"
                For Each i As Integer In languages
                    DBA.NonQuery(trans, q,
                                 DBA.CreateParameter("RoleId", SqlDbType.Int, id),
                                 DBA.CreateParameter("LanguageId", SqlDbType.Int, i)
                    )
                Next
            End Sub

            Public Shared Sub DeleteAllowedLanguages(ByVal id As Integer, Optional ByVal trans As SqlTransaction = Nothing, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM SYS_RoleLanguage WHERE RoleId = @Id"
                If trans IsNot Nothing Then DBA.NonQuery(trans, q, DBA.CreateParameter("Id", SqlDbType.Int, id)) Else DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not (HasUsers(id, conn))
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                RolePermissionController.DeleteRolePermissions(id, conn)
                DeleteAllowedLanguages(id, Nothing, conn)
                Dim p = DBA.CreateParameter("Id", SqlDbType.Int, id)
                Dim q As String = "DELETE FROM SYS_UserTypeRole WHERE RoleId = @Id"
                DBA.NonQuery(conn, q, p)
                'q = "DELETE FROM SYS_MemberContact WHERE RoleId = @Id"
                'DBA.NonQuery(conn, q, p)
            End Sub

            Private Shared Function HasUsers(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM SYS_UserRole WHERE RoleId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
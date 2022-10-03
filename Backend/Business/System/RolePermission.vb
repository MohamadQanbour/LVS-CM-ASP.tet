Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums

Namespace EGV
    Namespace Business

        'Object
        Public Structure RolePermission

#Region "Public Properties"

            Public Property RoleId As Integer
            Public Property PermissionId As Integer
            Public Property CanRead As Boolean
            Public Property CanWrite As Boolean
            Public Property CanModify As Boolean
            Public Property CanPublish As Boolean
            Public Property CanDelete As Boolean

#End Region

        End Structure

        'Controller
        Public Class RolePermissionController

            Public Shared Function GetRolePermissionsList(ByVal roleId As Integer, Optional ByVal conn As SqlConnection = Nothing) As List(Of RolePermission)
                Dim lst As New List(Of RolePermission)
                Using dt As DataTable = DBA.DataTable(conn, "SELECT * FROM SYS_RolePermission WHERE RoleId = @Id", DBA.CreateParameter("Id", SqlDbType.Int, roleId))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New RolePermission() With {
                            .RoleId = Helper.GetSafeDBValue(dr("RoleId"), ValueTypes.TypeInteger),
                            .PermissionId = Helper.GetSafeDBValue(dr("PermissionId"), ValueTypes.TypeInteger),
                            .CanRead = Helper.GetSafeDBValue(dr("CanRead"), ValueTypes.TypeBoolean),
                            .CanWrite = Helper.GetSafeDBValue(dr("CanWrite"), ValueTypes.TypeBoolean),
                            .CanModify = Helper.GetSafeDBValue(dr("CanModify"), ValueTypes.TypeBoolean),
                            .CanPublish = Helper.GetSafeDBValue(dr("CanPublish"), ValueTypes.TypeBoolean),
                            .CanDelete = Helper.GetSafeDBValue(dr("CanDelete"), ValueTypes.TypeBoolean)
                        })
                    Next
                End Using
                Return lst
            End Function

            Public Shared Sub SaveRolePermissionsList(ByVal roleId As Integer, ByVal lst As List(Of RolePermission), Optional ByVal trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_RolePermission_Add"
                DBA.SPNonQuery(trans, "SYS_RolePermission_Clear", DBA.CreateParameter("RoleId", SqlDbType.Int, roleId))
                For Each item As RolePermission In lst
                    With item
                        DBA.SPNonQuery(trans, sp,
                                       DBA.CreateParameter("RoleId", SqlDbType.Int, roleId),
                                       DBA.CreateParameter("PermissionId", SqlDbType.Int, .PermissionId),
                                       DBA.CreateParameter("CanRead", SqlDbType.Bit, .CanRead),
                                       DBA.CreateParameter("CanWrite", SqlDbType.Bit, .CanWrite),
                                       DBA.CreateParameter("CanModify", SqlDbType.Bit, .CanModify),
                                       DBA.CreateParameter("CanPublish", SqlDbType.Bit, .CanPublish),
                                       DBA.CreateParameter("CanDelete", SqlDbType.Bit, .CanDelete)
                                       )
                    End With
                Next
            End Sub

            Public Shared Function GetListFromJSON(ByVal roleId As Integer, ByVal json As String) As List(Of RolePermission)
                Dim lst As New List(Of RolePermission)
                If json <> String.Empty Then
                    lst = Helper.JSDeserialize(Of List(Of RolePermission))(json)
                    lst = (From item In lst Where item.CanRead = True OrElse item.CanWrite = True OrElse item.CanModify = True OrElse item.CanPublish = True OrElse item.CanDelete = True).ToList()
                End If
                Return lst
            End Function

            Public Shared Sub DeleteRolePermissions(ByVal roleId As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM SYS_RolePermission WHERE RoleId = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, roleId))
            End Sub

        End Class

    End Namespace
End Namespace
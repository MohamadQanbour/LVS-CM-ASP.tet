Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures

Namespace EGV
    Namespace Business

        Public Class UserTypeRole

            Public Property UserType As String = String.Empty
            Public Property Title As String = String.Empty
            Public Property RoleId As Integer = 0

        End Class

        Public Class UserTypeRoleController

            Public Shared Function GetList(ByVal conn As SqlConnection) As List(Of UserTypeRole)
                Dim lst As New List(Of UserTypeRole)
                Using dt As DataTable = DBA.DataTable(conn, "SELECT * FROM SYS_UserTypeRole")
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New UserTypeRole() With {
                            .RoleId = Helper.GetSafeDBValue(dr("RoleId"), ValueTypes.TypeInteger),
                            .UserType = Helper.GetSafeDBValue(dr("UserType")),
                            .Title = Helper.GetSafeDBValue(dr("Title"))
                        })
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function GetRoleOfType(ByVal conn As SqlConnection, ByVal type As String) As Integer
                Dim q As String = "SELECT RoleId FROM SYS_UserTypeRole WHERE UserType = @Type"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Type", SqlDbType.VarChar, type, 50))
            End Function

            Public Shared Sub Update(ByVal conn As SqlConnection, ByVal lst As List(Of UserTypeRole))
                Dim q As String = "UPDATE SYS_UserTypeRole SET RoleId = @Id WHERE UserType = @Type"
                For Each item As UserTypeRole In lst
                    DBA.NonQuery(conn, q,
                                 DBA.CreateParameter("Id", SqlDbType.Int, item.RoleId),
                                 DBA.CreateParameter("Type", SqlDbType.VarChar, item.UserType, 50)
                                 )
                Next
            End Sub

            Public Shared Function UserInType(ByVal userId As Integer, ByVal type As String, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM SYS_UserTypeRole T INNER JOIN SYS_UserRole R ON T.RoleId = R.RoleId WHERE R.UserId = @UserId AND T.UserType = @UserType"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("UserId", SqlDbType.Int, userId), DBA.CreateParameter("UserType", SqlDbType.VarChar, type, 50))
            End Function

        End Class

    End Namespace
End Namespace
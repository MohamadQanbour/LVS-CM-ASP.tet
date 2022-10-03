Imports System.Data.SqlClient
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Utils

Namespace EGV
    Namespace Business

        Public Class MemberContact

#Region "Public Members"

            Public Property MembershipType As MembershipTypes = MembershipTypes.Family
            Public Property RoleId As Integer = 0
            Public Property IsClassDependent As Boolean = False

#End Region

        End Class

        Public Class MemberContactController

            Public Shared Sub AddMemberContacts(ByVal conn As SqlConnection, ByVal memberType As MembershipTypes,
                                                ByVal lst As List(Of MemberContact))
                DeleteMemberContacts(conn, memberType)
                Dim q As String = "INSERT INTO LOK_MemberContact (MembershipType, RoleId, IsClassDependent) VALUES (@Type, @Role, @Class)"
                Dim trans As SqlTransaction = conn.BeginTransaction()
                Try
                    For Each item As MemberContact In lst
                        DBA.NonQuery(trans, q,
                                     DBA.CreateParameter("Type", SqlDbType.Int, memberType),
                                     DBA.CreateParameter("Role", SqlDbType.Int, item.RoleId),
                                     DBA.CreateParameter("Class", SqlDbType.Bit, item.IsClassDependent)
                                     )
                    Next
                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw ex
                End Try
            End Sub

            Public Shared Function GetMemberContacts(ByVal conn As SqlConnection, ByVal memberType As MembershipTypes) As List(Of MemberContact)
                Dim q As String = "SELECT * FROM LOK_MemberContact WHERE MembershipType = @Type"
                Dim lst As New List(Of MemberContact)
                Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("Type", SqlDbType.Int, memberType))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New MemberContact() With {
                            .RoleId = Helper.GetSafeDBValue(dr("RoleId"), ValueTypes.TypeInteger),
                            .IsClassDependent = Helper.GetSafeDBValue(dr("IsClassDependent"), ValueTypes.TypeBoolean),
                            .MembershipType = memberType
                        })
                    Next
                End Using
                Return lst
            End Function

            Public Shared Sub DeleteMemberContacts(ByVal conn As SqlConnection, ByVal memberType As MembershipTypes)
                Dim q As String = "DELETE FROM LOK_MemberContact WHERE MembershipType = @Type"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Type", SqlDbType.Int, memberType))
            End Sub

        End Class

    End Namespace
End Namespace

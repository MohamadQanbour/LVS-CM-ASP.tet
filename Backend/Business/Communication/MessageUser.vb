Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures

Namespace EGV
    Namespace Business

        'object
        Public Class MessageUser

#Region "Public Properties"

            Public Property MessageId As Guid = Nothing
            Public Property UserId As Integer = 0
            Public Property UserType As MessageUserTypes = MessageUserTypes.User
            Public Property MessageRole As MessageUserRoles = MessageUserRoles.Sender
            Public Property ReadDate As DateTime
            Public Property IsRead As Boolean = False
            Public Property IsStarred As Boolean = False
            Public Property IsViewed As Boolean = False

#End Region

        End Class

        'controller
        Public Class MessageUserController

#Region "Public Methods"

            Public Shared Sub AddMessageUsers(ByVal msgId As Guid, ByVal lst As List(Of ParsedMessageUser), Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "INSERT INTO COM_MessageUser (MessageId, UserId, UserType, MessageRole, IsRead, IsStarred, IsViewed) VALUES (@MessageId, @UserId, @UserType, @MessageRole, @IsRead, 0, 0);"
                For Each usr As ParsedMessageUser In lst
                    If usr.UserType <> MessageUserTypes.AllClass AndAlso usr.UserType <> MessageUserTypes.AllSection Then
                        If Not Exists(msgId, usr.UserId, usr.UserType, usr.UserRole, trans) Then
                            DBA.NonQuery(trans, q,
                                            DBA.CreateParameter("MessageId", SqlDbType.UniqueIdentifier, msgId),
                                            DBA.CreateParameter("UserId", SqlDbType.Int, usr.UserId),
                                            DBA.CreateParameter("UserType", SqlDbType.Int, usr.UserType),
                                            DBA.CreateParameter("MessageRole", SqlDbType.Int, usr.UserRole),
                                            DBA.CreateParameter("IsRead", SqlDbType.Bit, usr.UserRole = MessageUserRoles.Sender)
                                            )
                        End If
                    End If
                Next
            End Sub

            Public Shared Function Exists(ByVal msgId As Guid, ByVal usrId As Integer, ByVal usrType As Integer, ByVal usrRole As Integer, Optional ByVal trans As SqlTransaction = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM COM_MessageUser WHERE MessageId = @MId AND UserId = @UId AND UserType = @UType AND MessageRole = @MRole"
                Return Helper.GetSafeDBValue(DBA.Scalar(trans, q,
                                                        DBA.CreateParameter("MId", SqlDbType.UniqueIdentifier, msgId),
                                                        DBA.CreateParameter("UId", SqlDbType.Int, usrId),
                                                        DBA.CreateParameter("UType", SqlDbType.Int, usrType),
                                                        DBA.CreateParameter("MRole", SqlDbType.Int, usrRole)
                                                        )) > 0
            End Function

            Public Shared Function GetMessageUsers(ByVal msgId As Guid, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal trans As SqlTransaction = Nothing) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim q As String = "SELECT * FROM COM_MessageUser WITH (NOLOCK) WHERE MessageId = @Id ORDER BY ReadDate DESC"
                ret.Query = q
                ret.List = DBA.DataTable(conn, q, DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, msgId))
                ret.Count = ret.List.Rows.Count
                Return ret
            End Function

#End Region

        End Class

    End Namespace
End Namespace
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures

Namespace EGV
    Namespace Business

        'object
        Public Class Message

#Region "Public Members"

            Public Property Id As Guid = Nothing
            Public Property Title As String = String.Empty
            Public Property MessageDate As DateTime = Nothing
            Public Property MessageContent As String = String.Empty
            Public Property UsersJSON As String = String.Empty

            Public Property Users As List(Of ParsedMessageUser)
            Public Property Attachments As List(Of MessageAttachment)

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Guid = Nothing, Optional ByVal conn As SqlConnection = Nothing)
                DBA.GetSafeConn(conn)
                Users = New List(Of ParsedMessageUser)()
                Attachments = New List(Of MessageAttachment)()
                If tid <> Nothing Then
                    Using dt As DataTable = DBA.DataTable(conn, "SELECT * FROM COM_Message WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, tid))
                        If dt.Rows.Count = 1 Then
                            Dim dr As DataRow = dt.Rows(0)
                            Id = dr("Id")
                            Title = Helper.GetSafeDBValue(dr("Title"))
                            MessageDate = Helper.GetSafeDBValue(dr("MessageDate"), ValueTypes.TypeDateTime)
                            MessageContent = Helper.GetSafeDBValue(dr("MessageContent"))
                            UsersJSON = Helper.GetSafeDBValue(dr("Users"))
                            DeserializeUsers()
                            LoadAttachments(conn)
                        End If
                    End Using
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Sub SerializeUsers()
                UsersJSON = Helper.JSSerialize(Users)
            End Sub

#End Region

#Region "Private Methods"

            Private Sub DeserializeUsers()
                Users = Helper.JSDeserialize(Of List(Of ParsedMessageUser))(UsersJSON)
            End Sub

            Private Sub LoadAttachments(ByVal conn As SqlConnection)
                Dim q As String = "SELECT * FROM COM_MessageAttachment WHERE MessageId = @Id"
                Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, Id))
                    If dt.Rows.Count > 0 Then
                        For Each dr As DataRow In dt.Rows
                            Attachments.Add(New MessageAttachment() With {
                                .Id = Helper.GetSafeDBValue(dr("Id")),
                                .MessageId = Id,
                                .FileName = Helper.GetSafeDBValue(dr("FileName")),
                                .FilePath = Helper.GetSafeDBValue(dr("FilePath")),
                                .FileSize = Helper.GetSafeDBValue(dr("FileSize"), ValueTypes.TypeInteger)
                            })
                        Next
                    End If
                End Using
            End Sub

#End Region

        End Class

        'controller
        Public Class MessageController

#Region "Public Methods"

            Public Shared Function CreateMessage(ByVal msg As Message, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim success As Boolean = True
                msg.SerializeUsers()
                Dim sp As String = "COM_Message_Create"
                Dim lst As New List(Of ParsedMessageUser)()
                For Each usr As ParsedMessageUser In msg.Users
                    If usr.UserType <> MessageUserTypes.AllClass AndAlso usr.UserType <> MessageUserTypes.AllSection AndAlso usr.UserType <> MessageUserTypes.AllRoleUsers AndAlso usr.UserType <> MessageUserTypes.AllUsers Then
                        lst.Add(New ParsedMessageUser() With {
                            .UserId = usr.UserId, .FullName = usr.FullName, .UserRole = usr.UserRole,
                            .UserType = usr.UserType
                        })
                    Else
                        Dim classId As Integer = 0
                        Dim sectionId As Integer = 0
                        Dim roleId As Integer = 0
                        Dim allUsers As Boolean = False
                        If usr.UserType = MessageUserTypes.AllClass Then
                            classId = usr.UserId
                        ElseIf usr.UserType = MessageUserTypes.AllSection Then
                            sectionId = usr.UserId
                        ElseIf usr.UserType = MessageUserTypes.AllRoleUsers Then
                            roleId = usr.UserId
                        ElseIf usr.UserType = MessageUserTypes.AllUsers Then
                            allUsers = True
                        End If
                        If classId > 0 OrElse sectionId > 0 Then
                            Using dt As DataTable = StudentController.GetCollection(conn, 0, "", True, 0, sectionId, classId).List
                                For Each dr As DataRow In dt.Rows
                                    lst.Add(New ParsedMessageUser() With {
                                        .UserId = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                                        .UserRole = usr.UserRole,
                                        .UserType = MessageUserTypes.Student,
                                        .FullName = Helper.GetSafeDBValue(dr("IdName"))
                                    })
                                Next
                            End Using
                        End If
                        If roleId > 0 Then
                            Using dt As DataTable = UserController.GetCollection(conn, 0, String.Empty, True, {roleId}).List
                                For Each dr As DataRow In dt.Rows
                                    lst.Add(New ParsedMessageUser() With {
                                        .UserId = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                                        .UserRole = usr.UserRole,
                                        .UserType = MessageUserTypes.User,
                                        .FullName = Helper.GetSafeDBValue(dr("UserName"))
                                    })
                                Next
                            End Using
                        End If
                        If allUsers Then
                            Using dt As DataTable = UserController.GetCollection(conn, 0, String.Empty, True).List
                                For Each dr As DataRow In dt.Rows
                                    lst.Add(New ParsedMessageUser() With {
                                        .UserId = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                                        .UserRole = usr.UserRole,
                                        .UserType = MessageUserTypes.User,
                                        .FullName = Helper.GetSafeDBValue(dr("UserName"))
                                    })
                                Next
                            End Using
                            Using dt As DataTable = StudentController.GetCollection(conn, 0, String.Empty, True).List
                                For Each dr As DataRow In dt.Rows
                                    lst.Add(New ParsedMessageUser() With {
                                        .UserId = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                                        .UserRole = usr.UserRole,
                                        .UserType = MessageUserTypes.Student,
                                        .FullName = Helper.GetSafeDBValue(dr("IdName"))
                                    })
                                Next
                            End Using
                            Using dt As DataTable = FamilyController.GetCollection(conn, 0, String.Empty, True).List
                                For Each dr As DataRow In dt.Rows
                                    lst.Add(New ParsedMessageUser() With {
                                        .UserId = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                                        .UserRole = usr.UserRole,
                                        .UserType = MessageUserTypes.Family,
                                        .FullName = Helper.GetSafeDBValue(dr("FullName"))
                                    })
                                Next
                            End Using
                        End If
                    End If
                Next
                Dim trans As SqlTransaction = conn.BeginTransaction()
                Try
                    msg.Id = DBA.SPScalar(trans, sp,
                                      DBA.CreateParameter("Title", SqlDbType.NVarChar, msg.Title, 100),
                                      DBA.CreateParameter("Content", SqlDbType.NText, msg.MessageContent),
                                      DBA.CreateParameter("Users", SqlDbType.NText, msg.UsersJSON)
                                      )
                    MessageUserController.AddMessageUsers(msg.Id, lst, trans)
                    MessageAttachmentController.AddMessageAttachments(msg, trans)
                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    EGVExceptionController.AddException(ex, conn)
                    success = False
                End Try
                Try
                    WebRequests.NotificationRequest.SendMessageNotification(lst, msg.Title, conn)
                Catch ex As Exception
                    EGVExceptionController.AddException(ex, conn)
                End Try
                Return success
            End Function

            Public Shared Function ListMessages(ByVal conn As SqlConnection, ByVal userId As Integer, ByVal userType As MessageUserTypes,
                                                ByVal msgRoleType As MessageUserRoleTypes, ByVal pageSize As Integer,
                                                ByVal pageIndex As Integer, Optional ByVal searchTerm As String = "",
                                                Optional ByVal onlyUnread As Boolean = False, Optional ByVal onlyStarred As Boolean = False,
                                                Optional ByVal onlyNew As Boolean = False, Optional ByVal senderId As Integer = 0) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                'Dim sp As String = "COM_Message_List"
                'Dim ds As DataSet = DBA.SPDataSet(conn, sp,
                '                                  DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                '                                  DBA.CreateParameter("UserType", SqlDbType.Int, userType),
                '                                  DBA.CreateParameter("MessageRoleType", SqlDbType.Int, msgRoleType),
                '                                  DBA.CreateParameter("PageSize", SqlDbType.Int, pageSize),
                '                                  DBA.CreateParameter("PageIndex", SqlDbType.Int, pageIndex),
                '                                  DBA.CreateParameter("SearchTerm", SqlDbType.NVarChar, searchTerm, 255),
                '                                  DBA.CreateParameter("OnlyUnread", SqlDbType.Bit, onlyUnread),
                '                                  DBA.CreateParameter("OnlyStarred", SqlDbType.Bit, onlyStarred),
                '                                  DBA.CreateParameter("OnlyNew", SqlDbType.Bit, onlyNew),
                '                                  DBA.CreateParameter("SenderId", SqlDbType.Int, senderId)
                '                                  )
                'ret.List = ds.Tables(0)
                'ret.Count = Helper.GetSafeDBValue(ds.Tables(1).Rows(0)(0), ValueTypes.TypeInteger)
                Dim q As String = <a><![CDATA[
                    WITH OrderedTable AS (
	                    SELECT	M.*,
			                    S.UserId AS SenderUserId,
			                    S.UserType AS SenderUserType,
			                    U.IsRead,
			                    U.IsStarred,
			                    U.IsViewed,
			                    (
				                    SELECT	COUNT(*)
				                    FROM	COM_MessageAttachment A
				                    WHERE	A.MessageId = M.Id
			                    ) AS [HasAttachments],
			                    ROW_NUMBER() OVER (ORDER BY M.MessageDate DESC) AS RowNumber
	                    FROM	COM_Message M
			                    INNER JOIN COM_MessageUser S
				                    ON	S.MessageId = M.Id
					                    AND S.MessageRole = 1
			                    INNER JOIN COM_MessageUser U 
				                    ON	U.MessageId = M.Id 
					                    AND U.UserId = @UserId 
					                    AND U.UserType = @UserType
					                    AND (
						                    (@MessageRoleType = 1 AND U.MessageRole = 1)
						                    OR (@MessageRoleType = 2 AND U.MessageRole <> 1)
					                    )
	                    WHERE		(@SearchTerm = N'' OR M.Title LIKE N'%' + @SearchTerm + N'%')
			                    AND	(@OnlyUnread = 0 OR U.IsRead = 0)
			                    AND	(@OnlyStarred = 0 OR U.IsStarred = 1)
			                    AND (@OnlyNew = 0 OR U.IsViewed = 0)
			                    AND	(@SenderId = 0 OR (S.UserId = @SenderId AND S.MessageRole = 1))
			                    AND	U.IsDeleted = 0
                    )
                    SELECT	TOP (@PageSize) *
                    FROM	OrderedTable
                    WHERE	RowNumber > @PageSize * @PageIndex
                    ORDER BY RowNumber ASC
                ]]></a>
                Dim ds = DBA.DataSet(conn, q,
                    DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                    DBA.CreateParameter("UserType", SqlDbType.Int, userType),
                    DBA.CreateParameter("MessageRoleType", SqlDbType.Int, msgRoleType),
                    DBA.CreateParameter("PageSize", SqlDbType.Int, pageSize),
                    DBA.CreateParameter("PageIndex", SqlDbType.Int, pageIndex),
                    DBA.CreateParameter("SearchTerm", SqlDbType.NVarChar, searchTerm, 255),
                    DBA.CreateParameter("OnlyUnread", SqlDbType.Bit, onlyUnread),
                    DBA.CreateParameter("OnlyStarred", SqlDbType.Bit, onlyStarred),
                    DBA.CreateParameter("OnlyNew", SqlDbType.Bit, onlyNew),
                    DBA.CreateParameter("SenderId", SqlDbType.Int, senderId)
                )
                ret.Query = q
                ret.List = ds.Tables(0)
                q = <a><![CDATA[
                    SELECT	COUNT(*)
	                FROM	COM_Message M
			                INNER JOIN COM_MessageUser S
				                ON	S.MessageId = M.Id
					                AND S.MessageRole = 1
			                INNER JOIN COM_MessageUser U
				                ON	U.MessageId = M.Id
					                AND U.UserId = @UserId
					                AND U.UserType = @UserType
					                AND (
						                (@MessageRoleType = 1 AND U.MessageRole = 1)
						                OR (@MessageRoleType = 2 AND U.MessageRole <> 1)
					                )
	                WHERE		(@SearchTerm = N'' OR M.Title LIKE N'%' + @SearchTerm + N'%')
				                AND	(@OnlyUnread = 0 OR U.IsRead = 0)
				                AND	(@OnlyStarred = 0 OR U.IsStarred = 1)
				                AND (@OnlyNew = 0 OR U.IsViewed = 0)
				                AND	(@SenderId = 0 OR (S.UserId = @SenderId AND S.MessageRole = 1))
				                AND	U.IsDeleted = 0
                ]]></a>
                ret.Count = DBA.Scalar(conn, q,
                    DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                    DBA.CreateParameter("UserType", SqlDbType.Int, userType),
                    DBA.CreateParameter("MessageRoleType", SqlDbType.Int, msgRoleType),
                    DBA.CreateParameter("PageSize", SqlDbType.Int, pageSize),
                    DBA.CreateParameter("PageIndex", SqlDbType.Int, pageIndex),
                    DBA.CreateParameter("SearchTerm", SqlDbType.NVarChar, searchTerm, 255),
                    DBA.CreateParameter("OnlyUnread", SqlDbType.Bit, onlyUnread),
                    DBA.CreateParameter("OnlyStarred", SqlDbType.Bit, onlyStarred),
                    DBA.CreateParameter("OnlyNew", SqlDbType.Bit, onlyNew),
                    DBA.CreateParameter("SenderId", SqlDbType.Int, senderId)
                )
                Return ret
            End Function

            Public Shared Function GetAllMessages(ByVal conn As SqlConnection, ByVal pageSize As Integer, ByVal pageIndex As Integer, Optional ByVal searchTerm As String = "") As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim q As String = "WITH OrderTable AS (SELECT M.*, U.FullName AS [SenderName], ROW_NUMBER() OVER (ORDER BY M.MessageDate DESC) AS RowNumber FROM COM_Message M WITH (NOLOCK) INNER JOIN COM_MessageUser S ON S.MessageId = M.Id AND S.MessageRole = 1 INNER JOIN SYS_UserProfile U ON U.UserId = S.UserId WHERE (@SearchTerm = N'' OR M.Title LIKE N'%' + @SearchTerm + N'%')) SELECT TOP (@PageSize) * FROM OrderTable WHERE RowNumber > @PageSize * @PageIndex"
                Dim ds As DataSet = DBA.DataSet(conn, q, DBA.CreateParameter("SearchTerm", SqlDbType.NVarChar, searchTerm, 255), DBA.CreateParameter("PageIndex", SqlDbType.Int, pageIndex), DBA.CreateParameter("PageSize", SqlDbType.Int, pageSize))
                ret.List = ds.Tables(0)
                ret.Count = DBA.Scalar(conn, "SELECT COUNT(*) FROM COM_Message WITH (NOLOCK) WHERE (@SearchTerm = N'' OR Title LIKE N'%' + @SearchTerm + N'%')", DBA.CreateParameter("SearchTerm", SqlDbType.NVarChar, searchTerm, 255))
                ret.Query = q
                Return ret
            End Function

            Public Shared Function GetNextPrev(ByVal conn As SqlConnection, ByVal userId As Integer, ByVal userType As MessageUserTypes,
                                               ByVal msgRoleType As MessageUserRoleTypes, ByVal id As Guid) As NextPrevMessageId
                Dim ret As New NextPrevMessageId()
                Dim sp As String = "COM_Message_NextPrevious"
                Using ds As DataSet = DBA.SPDataSet(conn, sp,
                                                    DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                                                    DBA.CreateParameter("UserType", SqlDbType.Int, userType),
                                                    DBA.CreateParameter("RoleType", SqlDbType.Int, msgRoleType),
                                                    DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, id)
                                                    )
                    Dim currPos As Integer = ds.Tables(1).Rows(0)("RowNumber")
                    Dim total As Integer = ds.Tables(0).Rows.Count
                    If total > 0 Then
                        Dim prev As Integer = currPos - 1
                        If prev < 1 Then prev = 1
                        Dim nxt As Integer = currPos + 1
                        If nxt > total Then nxt = total
                        Dim dt As DataTable = ds.Tables(0)
                        ret.PrevId = dt.Rows(prev - 1)("Id")
                        ret.NextId = dt.Rows(nxt - 1)("Id")
                    Else
                        ret.PrevId = id
                        ret.NextId = id
                    End If
                End Using
                Return ret
            End Function

            Public Shared Function GetNotViewedCount(ByVal conn As SqlConnection, ByVal userId As Integer, ByVal userType As MessageUserTypes) As Integer
                Dim q As String = "SELECT COUNT(*) FROM COM_MessageUser WHERE UserId = @UserId AND UserType = @UserType AND MessageRole <> @MessageRole AND IsViewed = @IsViewed AND IsDeleted = @Deleted"
                Return DBA.Scalar(conn, q,
                                  DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                                  DBA.CreateParameter("UserType", SqlDbType.Int, userType),
                                  DBA.CreateParameter("MessageRole", SqlDbType.Int, MessageUserRoles.Sender),
                                  DBA.CreateParameter("IsViewed", SqlDbType.Bit, False),
                                  DBA.CreateParameter("Deleted", SqlDbType.Bit, False)
                                  )
            End Function

            Public Shared Function GetUnreadCount(ByVal conn As SqlConnection, ByVal userId As Integer, ByVal userType As MessageUserTypes) As Integer
                Dim q As String = "SELECT COUNT(*) FROM COM_MessageUser WHERE UserId = @UserId AND UserType = @UserType AND MessageRole <> @MessageRole AND IsRead = @IsRead AND IsDeleted = @Deleted"
                Return DBA.Scalar(conn, q,
                                  DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                                  DBA.CreateParameter("UserType", SqlDbType.Int, userType),
                                  DBA.CreateParameter("MessageRole", SqlDbType.Int, MessageUserRoles.Sender),
                                  DBA.CreateParameter("IsRead", SqlDbType.Bit, False),
                                  DBA.CreateParameter("Deleted", SqlDbType.Bit, False)
                                  )
            End Function

            Public Shared Function GetStarredCount(ByVal conn As SqlConnection, ByVal userId As Integer, ByVal userType As MessageUserTypes) As Integer
                Dim q As String = "SELECT COUNT(*) FROM COM_MessageUser WHERE UserId = @UserId AND UserType = @UserType AND MessageRole <> @MessageRole AND IsStarred = @IsStarred"
                Return DBA.Scalar(conn, q,
                                  DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                                  DBA.CreateParameter("UserType", SqlDbType.Int, userType),
                                  DBA.CreateParameter("MessageRole", SqlDbType.Int, MessageUserRoles.Sender),
                                  DBA.CreateParameter("IsStarred", SqlDbType.Bit, True)
                                  )
            End Function

            Public Shared Sub ToggleStar(ByVal messageId As String, ByVal userId As Integer, ByVal messageRoleType As MessageUserRoleTypes,
                                         Optional ByVal setStarred As Boolean = True, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE COM_MessageUser SET IsStarred = @Star WHERE MessageId = @Id AND UserId = @UserId AND UserType = @UserType AND ((@MessageRole = " & MessageUserRoleTypes.Received & " AND MessageRole > 1) OR (@MessageRole = " & MessageUserRoleTypes.Sent & " AND MessageRole = 1))"
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("Star", SqlDbType.Bit, setStarred),
                             DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, New Guid(messageId)),
                             DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                             DBA.CreateParameter("UserType", SqlDbType.Int, MessageUserTypes.User),
                             DBA.CreateParameter("MessageRole", SqlDbType.Int, messageRoleType)
                             )
            End Sub

            Public Shared Sub ViewMessages(ByVal conn As SqlConnection, ByVal userId As Integer, ByVal userType As MessageUserTypes,
                                           ByVal pageSize As Integer, ByVal pageIndex As Integer)
                Dim sp As String = "COM_Message_View"
                DBA.SPNonQuery(conn, sp,
                               DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                               DBA.CreateParameter("UserType", SqlDbType.Int, userType),
                               DBA.CreateParameter("PageSize", SqlDbType.Int, pageSize),
                               DBA.CreateParameter("PageIndex", SqlDbType.Int, pageIndex)
                               )
            End Sub

            Public Shared Sub ReadMessage(ByVal conn As SqlConnection, ByVal messageId As Guid, ByVal userId As Integer,
                                          ByVal userType As MessageUserTypes)
                Dim sp As String = "COM_Message_Read"
                DBA.SPNonQuery(conn, sp,
                               DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, messageId),
                               DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                               DBA.CreateParameter("UserType", SqlDbType.Int, userType)
                               )
            End Sub

            Public Shared Sub DeleteMessage(ByVal conn As SqlConnection, ByVal msgId As Guid, ByVal usrId As Integer,
                                            ByVal usrType As MessageUserTypes, ByVal role As MessageUserRoleTypes)
                Dim q As String = "UPDATE COM_MessageUser SET IsDeleted = 1 WHERE MessageId = @Id AND UserId = @UserId AND UserType = @Type AND MessageRole " & IIf(role = MessageUserRoleTypes.Received, " <> 1", " = 1")
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, msgId),
                             DBA.CreateParameter("UserId", SqlDbType.Int, usrId),
                             DBA.CreateParameter("Type", SqlDbType.Int, usrType),
                             DBA.CreateParameter("Role", SqlDbType.Int, role)
                             )
                q = "SELECT COUNT(*) FROM COM_MessageUser WHERE MessageId = @Id AND IsDeleted = 0"
                Dim count As Integer = DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, msgId))
                If count = 0 Then
                    q = "DELETE FROM COM_MessageUser WHERE MessageId = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, msgId))
                    q = "DELETE FROM COM_Message WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, msgId))
                    q = "SELECT FilePath FROM COM_MessageAttachment WHERE MessageId = @Id"
                    Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, msgId))
                        For Each dr As DataRow In dt.Rows
                            Dim fpath As String = dr("FilePath")
                            Dim dir As String = "~" & Path.MapCMSAsset(fpath)
                            Dim srvrDir As String = Helper.Server.MapPath(dir)
                            If IO.File.Exists(srvrDir) Then
                                Try
                                    IO.File.Delete(srvrDir)
                                Catch ex As Exception
                                End Try
                            End If
                        Next
                    End Using
                    q = "DELETE FROM COM_MessageAttachment WHERE MessageId = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.UniqueIdentifier, msgId))
                End If
            End Sub

#End Region

        End Class

    End Namespace
End Namespace
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Enums
Imports EGV
Imports System.Web

Namespace Ajax

    Public Class Messaging
        Inherits SecureAjaxBaseClass

#Region "Request Values"

        Public ReadOnly Property AccessToken As String = GetSafeRequestValue("access_token")
        Public ReadOnly Property Title As String = GetSafeRequestValue("subject")
        Public ReadOnly Property MessageRoleType As MessageUserRoleTypes = GetSafeRequestValue("message_role", ValueTypes.TypeInteger)
        Public ReadOnly Property PageSize As Integer = GetSafeRequestValue("page_size", ValueTypes.TypeInteger)
        Public ReadOnly Property PageIndex As Integer = GetSafeRequestValue("page_index", ValueTypes.TypeInteger)
        Public ReadOnly Property SearchTerm As String = GetSafeRequestValue("search_term")
        Public ReadOnly Property OnlyUnread As Boolean = GetSafeRequestValue("unread", ValueTypes.TypeBoolean)
        Public ReadOnly Property OnlyNew As Boolean = GetSafeRequestValue("new", ValueTypes.TypeBoolean)
        Public ReadOnly Property MessageId As String = GetSafeRequestValue("message_id")
        Public ReadOnly Property ForwardId As String = GetSafeRequestValue("forward")

#End Region

#Region "Overridden Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "SaveAttachment"
                    ret = SaveAttachments(MyConn, LanguageId)
                Case "Compose"
                    ret = CreateMessage(MyConn, LanguageId)
                Case "List"
                    ret = ListMessages(MyConn, LanguageId)
                Case "NextPrev"
                    ret = GetNextPrev(MyConn, LanguageId)
                Case "NotViewedCount"
                    ret = GetNotViewedCount(MyConn, LanguageId)
                Case "UnreadCount"
                    ret = GetUnreadCount(MyConn, LanguageId)
                Case "SetViewMessages"
                    ret = ViewMessages(MyConn, LanguageId)
                Case "GetMessage"
                    ret = GetMessage(MyConn, LanguageId)
                Case "Delete"
                    ret = DeleteMessage(MyConn, LanguageId)
                Case "GetReplyAllReceivers"
                    ret = GetMessageReceivers(MyConn, LanguageId)
                Case "GetReplyReceiver"
                    ret = GetMessageReceiver(MyConn, LanguageId)
                Case "GetAddressBook"
                    ret = GetMessagingTo(MyConn, LanguageId)
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function SaveAttachments(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of String)
            Dim lst As New List(Of String)
            If Helper.Request.Files.Count > 0 Then
                Dim dir As String = "~" & Path.MapCMSAsset("attachments")
                Dim tPath As String = Helper.Server.MapPath(dir)
                If Not IO.Directory.Exists(tPath) Then IO.Directory.CreateDirectory(tPath)
                For i As Integer = 0 To Helper.Request.Files.Count - 1
                    Dim file As HttpPostedFile = Helper.Request.Files(i)
                    Dim ext As String = file.FileName.Substring(file.FileName.LastIndexOf("."))
                    Dim f As String = Helper.EscapeURL(file.FileName.Replace(ext, ""))
                    f = f & ext
                    Dim fpath As String = "attachments/" & f
                    Dim targetPath As String = dir & "/" & f
                    file.SaveAs(Helper.Server.MapPath(targetPath))
                    lst.Add(fpath)
                Next
            End If
            Return lst
        End Function

        Private Function CreateMessage(ByVal conn As SqlConnection, ByVal langId As Integer) As Boolean
            Dim ret As Boolean = True
            Dim msg As New MessageCompose() With {
                .SenderToken = AccessToken,
                .Title = Title
            }
            Dim usrs As String = Helper.Request.Form("users")
            If usrs IsNot Nothing AndAlso usrs.ToString() <> String.Empty Then
                msg.Users = Helper.JSDeserialize(Of List(Of ReceiverType))(usrs)
            Else
                msg.Users = New List(Of ReceiverType)
            End If
            Dim atts As String = Helper.Request.Form("attachments")
            If atts IsNot Nothing AndAlso atts.ToString() <> String.Empty Then
                msg.Attachments = Helper.JSDeserialize(Of List(Of String))(atts)
            Else
                msg.Attachments = New List(Of String)()
            End If
            Dim contnt As String = Helper.Request.Form("html")
            If contnt IsNot Nothing AndAlso contnt <> String.Empty Then contnt = HttpUtility.UrlDecode(contnt)
            Dim objMsg As New Message()
            objMsg.Title = msg.Title
            objMsg.MessageContent = contnt
            If FamilyController.AccessTokenExists(AccessToken, conn) Then
                Dim objSndr As Family = FamilyController.GetByAccessToken(AccessToken, conn)
                objMsg.Users.Add(New ParsedMessageUser() With {
                    .FullName = objSndr.FullName,
                    .UserId = objSndr.Id,
                    .UserRole = MessageUserRoles.Sender,
                    .UserType = MessageUserTypes.Family
                })
            ElseIf StudentController.AccessTokenExists(AccessToken, conn) Then
                Dim objSndr As Student = StudentController.GetByAccessToken(AccessToken, conn)
                objMsg.Users.Add(New ParsedMessageUser() With {
                    .FullName = objSndr.SchoolId & " - " & objSndr.FullName,
                    .UserId = objSndr.Id,
                    .UserRole = MessageUserRoles.Sender,
                    .UserType = MessageUserTypes.Student
                })
            Else
                Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.UserNotFound"))
            End If
            For Each usr As ReceiverType In msg.Users
                Select Case usr.type
                    Case MessageUserTypes.Family
                        Dim fullName As String = FamilyController.GetFullName(usr.id, conn)
                        objMsg.Users.Add(New ParsedMessageUser() With {
                            .FullName = fullName,
                            .UserId = usr.id,
                            .UserRole = MessageUserRoles.RecTo,
                            .UserType = MessageUserTypes.Family
                        })
                    Case MessageUserTypes.Student
                        Dim fullName As String = StudentController.GetFullName(usr.id, MyConn)
                        objMsg.Users.Add(New ParsedMessageUser() With {
                            .FullName = fullName,
                            .UserId = usr.id,
                            .UserRole = MessageUserRoles.RecTo,
                            .UserType = MessageUserTypes.Student
                        })
                    Case MessageUserTypes.User
                        Dim obj As New User(usr.id, MyConn)
                        objMsg.Users.Add(New ParsedMessageUser() With {
                            .FullName = obj.Profile.FullName,
                            .UserId = obj.Id,
                            .UserRole = MessageUserRoles.RecTo,
                            .UserType = MessageUserTypes.User
                        })
                    Case MessageUserTypes.AllClass
                        Dim className As String = StudyClassController.GetTitle(MyConn, usr.id, LanguageId)
                        objMsg.Users.Add(New ParsedMessageUser() With {
                            .FullName = className,
                            .UserId = usr.id,
                            .UserRole = MessageUserRoles.RecTo,
                            .UserType = MessageUserTypes.AllClass
                        })
                    Case MessageUserTypes.AllSection
                        Dim sectionName As String = SectionController.GetTitle(MyConn, usr.id, LanguageId)
                        objMsg.Users.Add(New ParsedMessageUser() With {
                            .FullName = sectionName,
                            .UserId = usr.id,
                            .UserRole = MessageUserRoles.RecTo,
                            .UserType = MessageUserTypes.AllSection
                        })
                    Case Else
                        Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.UserNotFound"))
                End Select
            Next
            For Each att As String In msg.Attachments
                Dim p As String = "~" & Path.MapCMSAsset(att)
                Dim fInfo As New IO.FileInfo(Helper.Server.MapPath(p))
                objMsg.Attachments.Add(New MessageAttachment() With {
                    .FileName = fInfo.Name,
                    .FileSize = fInfo.Length,
                    .FilePath = att
                })
            Next
            If ForwardId <> String.Empty Then
                Dim obj As New Message(New Guid(ForwardId), conn)
                If obj.Attachments.Count > 0 Then
                    For Each att As MessageAttachment In obj.Attachments
                        objMsg.Attachments.Add(New MessageAttachment() With {
                            .FileName = att.FileName,
                            .FilePath = att.FilePath,
                            .FileSize = att.FileSize
                        })
                    Next
                End If
            End If
            MessageController.CreateMessage(objMsg, conn)
            Return ret
        End Function

        Private Function ListMessages(ByVal conn As SqlConnection, ByVal langId As Integer) As MessagesList
            Dim lst As New MessagesList()
            Dim usrId As UserIdInformation = GetUserInformation(conn, AccessToken)
            Dim ret As DBAReturnObject = MessageController.ListMessages(conn, usrId.UserId, usrId.UserType, MessageRoleType, PageSize, PageIndex, SearchTerm, OnlyUnread, False, OnlyNew)
            Using dt As DataTable = ret.List
                lst.Total = ret.Count
                lst.Messages = New List(Of MessageObject)()
                For Each dr As DataRow In dt.Rows
                    lst.Messages.Add(New MessageObject() With {
                        .MessageId = Helper.GetSafeDBValue(dr("Id")),
                        .Title = Helper.GetSafeDBValue(dr("Title")),
                        .SenderTitle = GetFullName(conn, Helper.GetSafeDBValue(dr("SenderUserId"), ValueTypes.TypeInteger), Helper.GetSafeDBValue(dr("SenderUserType"), ValueTypes.TypeInteger)),
                        .MessageDate = Helper.FormateDateDifference(Helper.GetSafeDBValue(dr("MessageDate"), ValueTypes.TypeDate), False),
                        .HasAttachments = Helper.GetSafeDBValue(dr("HasAttachments"), ValueTypes.TypeInteger) > 0,
                        .IsRead = Helper.GetSafeDBValue(dr("IsRead"), ValueTypes.TypeBoolean)
                    })
                Next
            End Using
            Return lst
        End Function

        Private Function GetNextPrev(ByVal conn As SqlConnection, ByVal langId As Integer) As NextPrevMessageId
            Dim usrId As UserIdInformation = GetUserInformation(conn, AccessToken)
            Return MessageController.GetNextPrev(conn, usrId.UserId, usrId.UserType, MessageRoleType, New Guid(MessageId))
        End Function

        Private Function GetNotViewedCount(ByVal conn As SqlConnection, ByVal langId As Integer) As Integer
            Dim ret As UserIdInformation = GetUserInformation(conn, AccessToken)
            Return MessageController.GetNotViewedCount(conn, ret.UserId, ret.UserType)
        End Function

        Private Function GetUnreadCount(ByVal conn As SqlConnection, ByVal langId As Integer) As Integer
            Dim ret As UserIdInformation = GetUserInformation(conn, AccessToken)
            Return MessageController.GetUnreadCount(conn, ret.UserId, ret.UserType)
        End Function

        Private Function ViewMessages(ByVal conn As SqlConnection, ByVal langId As Integer) As Boolean
            Dim ret As Boolean = True
            Dim usrId As UserIdInformation = GetUserInformation(conn, AccessToken)
            MessageController.ViewMessages(conn, usrId.UserId, usrId.UserType, PageSize, PageIndex)
            Return ret
        End Function

        Private Function GetMessage(ByVal conn As SqlConnection, ByVal langId As Integer) As FullMessageObject
            Dim ret As New FullMessageObject
            If MessageId <> String.Empty Then
                Dim usrid As UserIdInformation = GetUserInformation(conn, AccessToken)
                Dim obj As New Message(New Guid(MessageId), conn)
                ret.MessageId = obj.Id.ToString()
                ret.Title = obj.Title
                Dim sndr As ParsedMessageUser = (From u In obj.Users Where u.UserRole = MessageUserRoles.Sender).FirstOrDefault()
                ret.SenderTitle = GetFullName(conn, sndr.UserId, sndr.UserType)
                ret.MessageDate = Helper.FormateDateDifference(obj.MessageDate, True)
                ret.Body = obj.MessageContent
                ret.Attachments = New List(Of AttachmentObject)()
                If obj.Attachments.Count > 0 Then
                    For Each att As MessageAttachment In obj.Attachments
                        Dim p As String = SettingController.ReadSetting("SITELINK", conn) & "/download-attachment/" & att.Id
                        Dim extension As String = p.Substring(p.LastIndexOf(".") + 1)
                        ret.Attachments.Add(New AttachmentObject() With {
                            .FileName = att.FileName,
                            .FilePath = p,
                            .FileSize = Helper.FormatFileSize(att.FileSize),
                            .FileType = extension
                        })
                    Next
                End If
                MessageController.ReadMessage(conn, New Guid(ret.MessageId), usrid.UserId, usrid.UserType)
            Else
                Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.MessageNotSpecified"))
            End If
            Return ret
        End Function

        Private Function DeleteMessage(ByVal conn As SqlConnection, ByVal langId As Integer) As Boolean
            Dim ret As Boolean = True
            Dim usrid As UserIdInformation = GetUserInformation(conn, AccessToken)
            MessageController.DeleteMessage(conn, New Guid(MessageId), usrid.UserId, usrid.UserType, MessageRoleType)
            Return ret
        End Function

        Private Function GetMessageReceivers(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)()
            Dim obj As New Message(New Guid(MessageId), conn)
            Dim usrId As UserIdInformation = GetUserInformation(conn, AccessToken)
            Dim sndr = (From u As ParsedMessageUser In obj.Users Where u.UserRole = MessageUserRoles.Sender).FirstOrDefault()
            ret.Add(New AutoComplete() With {
                .text = GetFullName(conn, sndr.UserId, sndr.UserType),
                .id = "{""id"": " & sndr.UserId & ", ""type"": " & sndr.UserType & "}"
            })
            For Each rec In (From u In obj.Users Where u.UserRole = MessageUserRoles.RecTo)
                If rec.UserId <> usrId.UserId Then
                    ret.Add(New AutoComplete() With {
                        .text = GetFullName(conn, rec.UserId, rec.UserType),
                        .id = "{""id"": " & rec.UserId & ", ""type"": " & rec.UserType & "}"
                    })
                End If
            Next
            Return ret
        End Function

        Private Function GetMessageReceiver(ByVal conn As SqlConnection, ByVal langId As Integer) As AutoComplete
            Dim ret As New AutoComplete()
            Dim obj As New Message(New Guid(MessageId), conn)
            Dim sndr = (From u As ParsedMessageUser In obj.Users Where u.UserRole = MessageUserRoles.Sender).FirstOrDefault()
            ret.text = GetFullName(conn, sndr.UserId, sndr.UserType)
            ret.id = "{""id"": " & sndr.UserId & ", ""type"": " & sndr.UserType & "}"
            Return ret
        End Function

        Private Function GetMessagingTo(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            Dim usrId As UserIdInformation = GetUserInformation(conn, AccessToken)
            Dim roles As New List(Of MemberContact)
            Select Case usrId.UserType
                Case MessageUserTypes.Family
                    roles = MemberContactController.GetMemberContacts(conn, MembershipTypes.Family)
                    For Each roleitem As MemberContact In roles
                        If roleitem.IsClassDependent Then
                            For Each student As Integer In FamilyController.GetStudents(conn, usrId.UserId, True)
                                For Each usr As UserObject In StudentController.GetStudentTeachers(conn, student, SearchTerm)
                                    ret.Add(New AutoComplete() With {
                                        .id = "{""id"": " & usr.Id & ", ""type"": " & MessageUserTypes.User & "}",
                                        .text = usr.FullName
                                    })
                                Next
                            Next
                        Else
                            Using dt As DataTable = UserController.GetCollection(conn, 0, SearchTerm, True, {roleitem.RoleId}).List
                                For Each dr As DataRow In dt.Rows
                                    ret.Add(New AutoComplete() With {
                                        .id = "{""id"": " & Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger) & ", ""type"": " & MessageUserTypes.User & "}",
                                        .text = Helper.GetSafeDBValue(dr("FullName"))
                                    })
                                Next
                            End Using
                        End If
                    Next
                Case MessageUserTypes.Student
                    roles = MemberContactController.GetMemberContacts(conn, MembershipTypes.Student)
                    For Each roleitem As MemberContact In roles
                        If roleitem.IsClassDependent Then
                            For Each usr As UserObject In StudentController.GetStudentTeachers(conn, usrId.UserId, SearchTerm)
                                ret.Add(New AutoComplete() With {
                                    .id = "{""id"": " & usr.Id & ", ""type"": " & MessageUserTypes.User & "}",
                                    .text = usr.FullName
                                })
                            Next
                        Else
                            Using dt As DataTable = UserController.GetCollection(conn, 0, SearchTerm, True, {roleitem.RoleId}).List
                                For Each dr As DataRow In dt.Rows
                                    ret.Add(New AutoComplete() With {
                                        .id = "{""id"": " & Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger) & ", ""type"": " & MessageUserTypes.User & "}",
                                        .text = Helper.GetSafeDBValue(dr("FullName"))
                                    })
                                Next
                            End Using
                        End If
                    Next
            End Select
            Return ret
        End Function

#End Region

#Region "Helper"

        Private Function GetFullName(ByVal conn As SqlConnection, ByVal userId As Integer, ByVal userType As MessageUserTypes) As String
            Select Case userType
                Case MessageUserTypes.Family
                    Return FamilyController.GetSchoolId(userId, conn) & " - " & FamilyController.GetFullName(userId, conn)
                Case MessageUserTypes.Student
                    Return StudentController.GetSchoolId(userId, conn) & " - " & StudentController.GetFullName(userId, conn)
                Case MessageUserTypes.User
                    Return UserController.GetFullName(userId, conn)
                Case Else
                    Return "UNKNOWN"
            End Select
        End Function

        Private Function GetUserInformation(ByVal conn As SqlConnection, ByVal accessToken As String) As UserIdInformation
            Dim ret As New UserIdInformation
            If FamilyController.AccessTokenExists(accessToken, conn) Then
                Dim obj As Family = FamilyController.GetByAccessToken(accessToken, conn)
                ret.UserId = obj.Id
                ret.UserType = MessageUserTypes.Family
            ElseIf StudentController.AccessTokenExists(accessToken, conn) Then
                Dim obj As Student = StudentController.GetByAccessToken(accessToken, conn)
                ret.UserId = obj.Id
                ret.UserType = MessageUserTypes.Student
            Else
                Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.UserNotFound"))
            End If
            Return ret
        End Function

#End Region

    End Class

End Namespace
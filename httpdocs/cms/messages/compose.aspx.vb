Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Business

Partial Class cms_messages_compose
    Inherits AuthCMSPageBase

#Region "Public Properties"

    Public ReadOnly Property Reply As Guid
        Get
            If Request.QueryString("reply") IsNot Nothing AndAlso Request.QueryString("reply") <> String.Empty Then Return New Guid(Request.QueryString("reply")) Else Return Nothing
        End Get
    End Property

    Public ReadOnly Property ReplyAll As Guid
        Get
            If Request.QueryString("replyall") IsNot Nothing AndAlso Request.QueryString("replyall") <> String.Empty Then Return New Guid(Request.QueryString("replyall")) Else Return Nothing
        End Get
    End Property

    Public ReadOnly Property Forward As Guid
        Get
            If Request.QueryString("forward") IsNot Nothing AndAlso Request.QueryString("forward") <> String.Empty Then Return New Guid(Request.QueryString("forward")) Else Return Nothing
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Public Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ddlTo.Attributes.Add("data-userid", AuthUser.Id)
        ddlTo.Attributes.Add("data-usertype", MessageUserTypes.User)
        If Not Page.IsPostBack Then
            Try
                MyConn.Open()
                Master.LoadTitles(Localization.GetResource("Resources.Local.ComposeMessage"))
                Dim id As Guid = Nothing
                Dim isReply As Boolean = ReplyAll <> Nothing OrElse Reply <> Nothing
                If Forward <> Nothing Then id = Forward
                If ReplyAll <> Nothing Then id = ReplyAll
                If Reply <> Nothing Then id = Reply
                If id <> Nothing Then
                    Dim obj As New Message(id, MyConn)
                    txtSubject.Text = IIf(isReply, "RE: ", "FWD: ") & obj.Title
                    Dim sndr = (From u In obj.Users Where u.UserRole = MessageUserRoles.Sender).FirstOrDefault()
                    Dim rec = (From u In obj.Users Where u.UserRole = MessageUserRoles.RecTo)
                    Dim sb As New StringBuilder()
                    sb.Append("<div><br /></div><hr /><div style=""font-weight: bold;"">From: " & sndr.FullName & "</div>")
                    sb.Append("<div style=""font-weight: bold;"">Date: " & obj.MessageDate.ToString("MMMM dd, yyyy hh:mm:ss t") & "</div>")
                    Dim lst As New List(Of String)
                    For Each item In rec
                        lst.Add(item.FullName)
                    Next
                    sb.Append("<div style=""font-weight: bold;"">To: " & String.Join(", ", lst.ToArray()) & "</div>")
                    sb.Append("<hr />")
                    sb.Append(obj.MessageContent)
                    txtBody.Text = sb.ToString()
                    If ReplyAll <> Nothing Then
                        Dim toUsrs As New List(Of ReceiverType)
                        Dim selSelected As New List(Of AutoComplete)
                        Dim os As New ReceiverType() With {.id = sndr.UserId, .type = sndr.UserType}
                        toUsrs.Add(os)
                        selSelected.Add(New AutoComplete() With {.id = Helper.JSSerialize(os), .text = GetFullName(os)})
                        For Each item In rec
                            If item.UserId <> AuthUser.Id AndAlso item.UserType <> MessageUserTypes.User Then
                                Dim o As New ReceiverType() With {.id = item.UserId, .type = item.UserType}
                                toUsrs.Add(o)
                                selSelected.Add(New AutoComplete() With {.id = Helper.JSSerialize(o), .text = GetFullName(o)})
                            End If
                        Next
                        hdnSelectedTo.Value = Helper.JSSerialize(toUsrs)
                        For Each item In selSelected
                            ddlTo.Items.Add(New ListItem() With {
                                .Text = item.text,
                                .Value = item.id,
                                .Selected = True
                            })
                        Next
                    End If
                    If Reply <> Nothing Then
                        Dim toUsrs As New List(Of ReceiverType)
                        Dim selSelected As New List(Of AutoComplete)
                        Dim o As New ReceiverType() With {.id = sndr.UserId, .type = sndr.UserType}
                        toUsrs.Add(o)
                        selSelected.Add(New AutoComplete() With {.id = Helper.JSSerialize(o), .text = GetFullName(o)})
                        hdnSelectedTo.Value = Helper.JSSerialize(toUsrs)
                        For Each item In selSelected
                            ddlTo.Items.Add(New ListItem() With {
                                .Text = item.text,
                                .Value = item.id,
                                .Selected = True
                            })
                        Next
                    End If
                End If
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
            EGVScriptManager.AddScript(Path.MapCMSScript("local/compose"))
        End If
    End Sub

    Public Sub lnkSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSend.Click
        If Page.IsValid Then
            Dim shouldRedirect As Boolean = True
            Try
                MyConn.Open()
                Dim lst As List(Of ReceiverType) = Helper.JSDeserialize(Of List(Of ReceiverType))(hdnSelectedTo.Value)
                Dim subject As String = txtSubject.Text
                Dim html As String = txtBody.Text
                Dim msg As New Message()
                msg.Title = subject
                msg.MessageContent = HttpUtility.HtmlDecode(html)
                msg.Users.Add(New ParsedMessageUser() With {
                    .FullName = AuthUser.Profile.FullName,
                    .UserId = AuthUser.Id,
                    .UserRole = MessageUserRoles.Sender,
                    .UserType = MessageUserTypes.User
                })
                For Each usr As ReceiverType In lst
                    Select Case usr.type
                        Case MessageUserTypes.Family
                            Dim obj As New Family(usr.id, MyConn)
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = obj.FullName,
                                .UserId = obj.Id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.Family
                            })
                        Case MessageUserTypes.Student
                            Dim obj As New Student(usr.id, MyConn)
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = obj.FullName,
                                .UserId = obj.Id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.Student
                            })
                        Case MessageUserTypes.User
                            Dim obj As New User(usr.id, MyConn)
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = obj.Profile.FullName,
                                .UserId = obj.Id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.User
                            })
                        Case MessageUserTypes.AllClass
                            Dim className As String = StudyClassController.GetTitle(MyConn, usr.id, LanguageId)
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = className,
                                .UserId = usr.id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.AllClass
                            })
                        Case MessageUserTypes.AllSection
                            Dim sectionName As String = SectionController.GetTitle(MyConn, usr.id, LanguageId)
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = sectionName,
                                .UserId = usr.id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.AllSection
                            })
                        Case MessageUserTypes.AllRoleUsers
                            Dim roleName As String = New Role(usr.id, MyConn).Title
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = roleName,
                                .UserId = usr.id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.AllRoleUsers
                            })
                        Case MessageUserTypes.AllUsers
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = "All Users | جميع المستخدمين",
                                .UserId = 0,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.AllUsers
                            })
                    End Select
                Next
                msg.Attachments = SaveAttachments(MyConn)
                MessageController.CreateMessage(msg, MyConn)
            Catch ex As Exception
                shouldRedirect = False
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
            If shouldRedirect Then Response.Redirect("list.aspx?cmd=1")
        End If
    End Sub

    Protected Sub validate_To(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Dim lst As List(Of ReceiverType) = Helper.JSDeserialize(Of List(Of ReceiverType))(hdnSelectedTo.Value)
        e.IsValid = lst.Count > 0
    End Sub

#End Region

#Region "Private Methods"

    Private Function SaveAttachments(ByVal conn As SqlConnection) As List(Of MessageAttachment)
        Dim lst As New List(Of MessageAttachment)
        If fileAttachment.HasFile Then
            Dim dir As String = "~" & Path.MapCMSAsset("attachments")
            Dim tPath As String = Server.MapPath(dir)
            If Not IO.Directory.Exists(tPath) Then IO.Directory.CreateDirectory(tPath)
            For i As Integer = 0 To Request.Files.Count - 1
                Dim file As HttpPostedFile = Request.Files(i)
                Dim ext As String = file.FileName.Substring(file.FileName.LastIndexOf("."))
                Dim f As String = Helper.EscapeURL(file.FileName.Replace(ext, ""))
                f = f & ext
                Dim fpath As String = "attachments/" & f
                Dim targetPath As String = dir & "/" & f
                file.SaveAs(Server.MapPath(targetPath))
                lst.Add(New MessageAttachment() With {
                    .FileName = f,
                    .FilePath = fpath,
                    .FileSize = file.ContentLength
                })
            Next
        End If
        If Forward <> Nothing Then
            Dim obj As New Message(Forward, conn)
            If obj.Attachments.Count > 0 Then
                For Each att As MessageAttachment In obj.Attachments
                    lst.Add(New MessageAttachment() With {
                        .FileName = att.FileName,
                        .FilePath = att.FilePath,
                        .FileSize = att.FileSize
                    })
                Next
            End If
        End If
        Return lst
    End Function

    Private Function GetFullName(ByVal usr As ReceiverType) As String
        Dim name As String = String.Empty
        Select Case usr.type
            Case MessageUserTypes.Family
                name = FamilyController.ReadField(usr.id, "FullName", MyConn)
            Case MessageUserTypes.Student
                name = StudentController.ReadField(usr.id, "FullName", MyConn)
            Case MessageUserTypes.User
                name = UserController.GetFullName(usr.id, MyConn)
        End Select
        Return name
    End Function

#End Region

End Class

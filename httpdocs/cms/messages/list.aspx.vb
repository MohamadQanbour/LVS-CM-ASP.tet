Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Business

Partial Class cms_messages_list
    Inherits AuthCMSPageBase

#Region "Public Properties"

    Public ReadOnly Property MessagesFilterType As MessageFilterTypes
        Get
            If Request.QueryString("type") IsNot Nothing AndAlso IsNumeric(Request.QueryString("type")) Then Return Request.QueryString("type") Else Return MessageFilterTypes.Inbox
        End Get
    End Property

    Public ReadOnly Property PageIndex As Integer
        Get
            If Request.QueryString("page") IsNot Nothing AndAlso IsNumeric(Request.QueryString("page")) Then Return Request.QueryString("page") Else Return 0
        End Get
    End Property

    Public ReadOnly Property SearchTerm As String
        Get
            If Request.QueryString("q") IsNot Nothing Then Return HttpUtility.UrlDecode(Request.QueryString("q")) Else Return String.Empty
        End Get
    End Property

    Public Property PageSize As Integer = 25

#End Region

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        hdnMessageType.Value = IIf(MessagesFilterType = MessageFilterTypes.Sent, MessageUserRoleTypes.Sent, MessageUserRoleTypes.Received)
        If Not Page.IsPostBack Then
            ProcessCMD(Master.Notifier)
            Try
                MyConn.Open()
                Dim desc As String = String.Empty
                Dim unreadCount As Integer = MessageController.GetUnreadCount(MyConn, AuthUser.Id, MessageUserTypes.User)
                Dim starredCount As Integer = MessageController.GetStarredCount(MyConn, AuthUser.Id, MessageUserTypes.User)
                If MessagesFilterType = MessageFilterTypes.Inbox AndAlso unreadCount > 0 Then desc = unreadCount
                If MessagesFilterType = MessageFilterTypes.Starred AndAlso starredCount > 0 Then desc = starredCount
                If desc <> String.Empty Then desc &= " " & Localization.GetResource("Resources.Local.NewMessages")
                If unreadCount > 0 Then litUnreadCount.Text = unreadCount
                If starredCount > 0 Then litStarredCount.Text = starredCount
                Master.LoadTitles(GetTitle(), desc, GetTitle())
                Select Case MessagesFilterType
                    Case MessageFilterTypes.Inbox
                        liInbox.Attributes.Add("class", "active")
                    Case MessageFilterTypes.Sent
                        liSent.Attributes.Add("class", "active")
                    Case MessageFilterTypes.Starred
                        liStarred.Attributes.Add("class", "active")
                    Case MessageFilterTypes.Unread
                        liUnread.Attributes.Add("class", "active")
                End Select
                txtSearch.Text = SearchTerm
                BindMessages(MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
        EGVScriptManager.AddScript(Path.MapCMSScript("local/mailbox-list"))
    End Sub

    Protected Sub Refresh(ByVal sender As Object, ByVal e As EventArgs)
        Try
            MyConn.Open()
            BindMessages(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim shouldRedirect As Boolean = True
        Try
            MyConn.Open()
            Dim ids = Helper.SplitString(hdnSelected.Value, ",")
            For Each id As String In ids
                MessageController.DeleteMessage(MyConn, New Guid(id), AuthUser.Id, MessageUserTypes.User,
                                                IIf(MessagesFilterType = MessageFilterTypes.Sent, MessageUserRoleTypes.Sent, MessageUserRoleTypes.Received)
                                                )
            Next
        Catch ex As Exception
            shouldRedirect = False
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
        If shouldRedirect Then Response.Redirect("list.aspx?type=" & MessagesFilterType)
    End Sub

    Protected Sub btnReply_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim ids = Helper.SplitString(hdnSelected.Value, ",")
        If ids.Length > 0 Then
            Dim id = ids(0)
            Response.Redirect("compose.aspx?reply=" & id & "&type=" & MessagesFilterType)
        End If
    End Sub

    Protected Sub btnReplyAll_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim ids = Helper.SplitString(hdnSelected.Value, ",")
        If ids.Length > 0 Then
            Dim id = ids(0)
            Response.Redirect("compose.aspx?replyall=" & id & "&type=" & MessagesFilterType)
        End If
    End Sub

    Protected Sub btnForward_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim ids = Helper.SplitString(hdnSelected.Value, ",")
        If ids.Length > 0 Then
            Dim id = ids(0)
            Response.Redirect("compose.aspx?forward=" & id & "&type=" & MessagesFilterType)
        End If
    End Sub

    Protected Sub lnkSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSearch.Click
        If txtSearch.Text <> String.Empty Then
            Response.Redirect("list.aspx?type=" & MessagesFilterType & "&q=" & HttpUtility.UrlEncode(txtSearch.Text))
        Else
            Response.Redirect("list.aspx?type=" & MessagesFilterType)
        End If
    End Sub

#End Region

#Region "Public Methods"

    Public Function GetTitle(Optional ByVal withInbox As Boolean = False) As String
        Dim ret As String = IIf(withInbox, "Inbox", "Mailbox")
        Select Case MessagesFilterType
            Case MessageFilterTypes.Sent
                ret = "Sent"
            Case MessageFilterTypes.Starred
                ret = "Starred"
            Case MessageFilterTypes.Unread
                ret = "Unread"
        End Select
        Return Localization.GetResource("Resources.Local." & ret)
    End Function

    Public Function GetSenderName(ByVal conn As SqlConnection, ByVal userId As Integer, ByVal usertype As MessageUserTypes) As String
        Dim ret As String = String.Empty
        Select Case usertype
            Case MessageUserTypes.Family
                ret = FamilyController.GetSchoolId(userId, conn) & " - " & FamilyController.GetFullName(userId, conn)
            Case MessageUserTypes.Student
                ret = StudentController.GetSchoolId(userId, conn) & " - " & StudentController.GetFullName(userId, conn)
            Case MessageUserTypes.User
                ret = UserController.GetFullName(userId, conn)
        End Select
        Return ret
    End Function

#End Region

#Region "Private Methods"

    Private Sub BindMessages(ByVal conn As SqlConnection)
        litFirst.Text = PageSize * PageIndex + 1
        litFirst2.Text = PageSize * PageIndex + 1
        Dim lst As DBAReturnObject = MessageController.ListMessages(MyConn, AuthUser.Id,
                                                                    MessageUserTypes.User,
                                                                    IIf(MessagesFilterType = MessageFilterTypes.Sent, MessageUserRoleTypes.Sent, MessageUserRoleTypes.Received),
                                                                    PageSize, PageIndex, SearchTerm, MessagesFilterType = MessageFilterTypes.Unread,
                                                                    MessagesFilterType = MessageFilterTypes.Starred
                                                                    )
        litTotal.Text = lst.Count
        litTotal2.Text = lst.Count
        Dim lastRecord As Integer = PageSize * PageIndex + PageSize
        litLast.Text = IIf(lastRecord > lst.Count, lst.Count, lastRecord)
        litLast2.Text = IIf(lastRecord > lst.Count, lst.Count, lastRecord)
        Dim numOfPages As Integer = Math.Ceiling(lst.Count / PageSize)
        hypPrevious.NavigateUrl = Path.MapCMSFile("messages/list.aspx?type=" & MessagesFilterType & "&page=" & IIf(PageIndex = 0, 0, PageIndex - 1))
        hypPrevious2.NavigateUrl = hypPrevious.NavigateUrl
        hypNext.NavigateUrl = Path.MapCMSFile("messages/list.aspx?type=" & MessagesFilterType & "&page=" & IIf(PageIndex = numOfPages - 1, numOfPages - 1, PageIndex + 1))
        hypNext2.NavigateUrl = hypNext2.NavigateUrl
        If lst.Count > 0 Then
            rptMessages.DataSource = lst.List
            rptMessages.DataBind()
            pnlNoMessage.Visible = False
        Else
            pnlNoMessage.Visible = True
        End If
        If MessagesFilterType = MessageFilterTypes.Inbox OrElse MessagesFilterType = MessageFilterTypes.Unread Then MessageController.ViewMessages(conn, AuthUser.Id, MessageUserTypes.User, PageSize, PageIndex)
    End Sub

#End Region

End Class

Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Business

Partial Class cms_messages_read_admin
    Inherits AuthCMSPageBase

#Region "Public Properties"

    Public ReadOnly Property MessageId As Guid
        Get
            If Request.QueryString("id") IsNot Nothing AndAlso Request.QueryString("id").ToString() <> String.Empty Then Return New Guid(Request.QueryString("id")) Else Return Nothing
        End Get
    End Property

    Public ReadOnly Property MessageFilterType As MessageFilterTypes
        Get
            If Request.QueryString("type") IsNot Nothing AndAlso IsNumeric(Request.QueryString("type")) Then Return Request.QueryString("type") Else Return MessageFilterTypes.Inbox
        End Get
    End Property

#End Region

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Try
                MyConn.Open()
                LoadMessage(MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

#End Region

#Region "Public Methods"

    Public Function IsImage(ByVal filePath As String) As Boolean
        Dim fileInfo As New IO.FileInfo(Helper.Server.MapPath("~" & Path.MapCMSAsset(filePath)))
        Select Case fileInfo.Extension
            Case ".jpg", ".jpeg", ".png"
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Function GetIconClass(ByVal filePath As String) As String
        Dim fileInfo As New IO.FileInfo(Helper.Server.MapPath("~" & Path.MapCMSAsset(filePath)))
        Dim ret As String = String.Empty
        Select Case fileInfo.Extension
            Case ".pdf"
                ret = "file-pdf-o"
            Case ".doc", ".docx"
                ret = "file-word-o"
            Case ".xls", ".xlsx"
                ret = "file-excel-o"
        End Select
        Return ret
    End Function

#End Region

#Region "Private Methods"

    Private Sub LoadMessage(ByVal conn As SqlConnection)
        Dim msg As New Message(MessageId, conn)
        litMessageTitle.Text = msg.Title
        Dim sndr As ParsedMessageUser = (From u In msg.Users Where u.UserRole = MessageUserRoles.Sender).FirstOrDefault()
        If sndr.UserType = MessageUserTypes.Family Then
            sndr.FullName = FamilyController.GetSchoolId(sndr.UserId, conn) & " - " & sndr.FullName
        ElseIf sndr.UserType = MessageUserTypes.Student Then
            sndr.FullName = StudentController.GetSchoolId(sndr.UserId, conn) & " - " & sndr.FullName
        End If
        Dim receivers = (From u In msg.Users Where u.UserRole <> MessageUserRoles.Sender)
        Dim recLst As New List(Of String)
        For Each item As ParsedMessageUser In receivers
            recLst.Add(item.FullName)
        Next
        litTo.Text = String.Join(", ", recLst.ToArray())
        litSenderName.Text = sndr.FullName
        litMessageDate.Text = msg.MessageDate.ToString("MMMM dd, yyyy hh:mm:ss tt")
        litHTML.Text = msg.MessageContent
        Master.LoadTitles(msg.Title)
        If msg.Attachments.Count > 0 Then
            rptAttachment.DataSource = msg.Attachments
            rptAttachment.DataBind()
        Else
            rptAttachment.Visible = False
        End If
        If (sndr.UserType = MessageUserTypes.User AndAlso sndr.UserId = AuthUser.Id) OrElse AuthUser.IsSuperAdmin OrElse AuthUser.CanRead(26, conn) Then
            Dim lst As New List(Of MessageUserReadDetails)
            Using dt = MessageUserController.GetMessageUsers(MessageId, conn).List
                For Each dr In dt.Rows
                    Dim item As New MessageUserReadDetails()
                    Dim usrType As MessageUserTypes = Helper.GetSafeDBValue(dr("UserType"), ValueTypes.TypeInteger)
                    If usrType = MessageUserTypes.Student Then
                        item.UserName = StudentController.GetFullName(Helper.GetSafeDBValue(dr("UserId"), ValueTypes.TypeInteger))
                    ElseIf usrType = MessageUserTypes.Family Then
                        item.UserName = FamilyController.GetFullName(Helper.GetSafeDBValue(dr("UserId"), ValueTypes.TypeInteger))
                    ElseIf usrType = MessageUserTypes.User Then
                        item.UserName = UserController.GetFullName(Helper.GetSafeDBValue(dr("UserId"), ValueTypes.TypeInteger))
                    End If
                    If IsDBNull(dr("ReadDate")) Then
                        item.ReadDate = Localization.GetResource("Resources.Local.NotRead")
                    Else
                        item.ReadDate = CDate(Helper.GetSafeDBValue(dr("ReadDate"), ValueTypes.TypeDateTime)).ToString("MMMM d, yyyy h:mm:ss tt")
                    End If
                    lst.Add(item)
                Next
            End Using
            rptDetails.DataSource = lst
            rptDetails.DataBind()
        Else
            hypDetails.Visible = False
            egvModal.Visible = False
        End If
    End Sub

    Public Structure MessageUserReadDetails
        Public Property UserName As String
        Public Property ReadDate As String
    End Structure

#End Region

    Protected Sub rptDetails_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim i As MessageUserReadDetails = e.Item.DataItem
            Dim litUserName As Literal = e.Item.FindControl("litUserName")
            Dim litReadDate As Literal = e.Item.FindControl("litReadDate")
            litUserName.Text = i.UserName
            litReadDate.Text = i.ReadDate
        End If
    End Sub

End Class

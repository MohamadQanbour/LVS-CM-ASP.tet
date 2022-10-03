Imports System.Data.SqlClient
Imports System.Data
Imports EGV
Imports B = EGV.Business.BusinessHelper
Imports EGV.Structures
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Utils

Partial Class cms_CMSMaster
    Inherits System.Web.UI.MasterPage

#Region "Private Members"

    Private Property _MyConn As SqlConnection = Nothing
    Private _pageId As Integer = 0
    Private _parentPageId As Integer = 0

#End Region

#Region "Public Properties"

    Public ReadOnly Property Notifier As Interfaces.INotifier
        Get
            Return egvNotifier
        End Get
    End Property

    Public ReadOnly Property MyConn As SqlConnection
        Get
            If _MyConn Is Nothing Then _MyConn = DBA.GetConn()
            Return _MyConn
        End Get
    End Property

    Public ReadOnly Property PageId As Integer
        Get
            If _pageId = 0 Then _pageId = Business.CMSMenuController.GetId(Request.Url.LocalPath.Replace("/" & Utils.Helper.CMSPath & "/", ""), MyConn)
            Return _pageId
        End Get
    End Property

    Public ReadOnly Property ParentPageId As Integer
        Get
            Return New CMSMenu(PageId, MyConn).ParentId
        End Get
    End Property

#End Region

#Region "Public Methods"

    Public Sub LoadTitles(ByVal pageTitle As String, Optional ByVal pageDescription As String = "", Optional ByVal bcTitle As String = "", Optional ByVal parentId As Integer = 0)
        litTitle.Text = pageTitle & " | " & B.ReadSetting("SITENAME", MyConn)
        litPageHeaderTitle.Text = pageTitle
        If pageDescription <> String.Empty Then litPageHeaderDescription.Text = pageDescription
        litBCCurrent.Text = IIf(bcTitle <> String.Empty, bcTitle, pageTitle)
        If parentId > 0 Then
            Dim lst As New List(Of BCItem)
            Dim parent As New Business.CMSMenu(parentId, MyConn)
            lst.Add(New BCItem() With {.IconClass = parent.IconClass, .Title = parent.Title, .URL = Utils.Path.MapCMSFile(parent.PagePath)})
            rptBC.DataSource = lst
            rptBC.DataBind()
        End If
    End Sub

    Public Sub LoadTitles(ByVal pageId As Integer, Optional ByVal parentId As Integer = 0)
        Dim title As String = Localization.GetResource("Resources.Global.Menu.M" & pageId & "Title")
        Dim pagetitle As String = Localization.GetResource("Resources.Global.Menu.M" & pageId & "PageTitle")
        Dim desc As String = Localization.GetResource("Resources.Global.Menu.M" & pageId & "Desc")
        litTitle.Text = pagetitle & " | " & B.ReadSetting("SITENAME", MyConn)
        litPageHeaderTitle.Text = pagetitle
        If desc <> String.Empty Then litPageHeaderDescription.Text = desc
        litBCCurrent.Text = title
        If parentId > 0 Then
            Dim lst As New List(Of BCItem)
            Dim parent As New CMSMenu(parentId, MyConn)
            lst.Add(New BCItem() With {
                .IconClass = parent.IconClass,
                .Title = Localization.GetResource("Resources.Global.Menu.M" & parentId & "Title"),
                .URL = Path.MapCMSFile(parent.PagePath)
            })
            rptBC.DataSource = lst
            rptBC.DataBind()
        End If
    End Sub

    Public Function GetSenderImage(ByVal conn As SqlConnection, ByVal userId As Integer, ByVal userType As MessageUserTypes) As String
        Dim ret As String = String.Empty
        If userType = MessageUserTypes.User Then ret = UserController.GetImage(userId, conn) Else ret = Path.MapCMSAsset("avatar.png")
        Return ret
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

#Region "Event Handlers"

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        _MyConn = DBA.GetConn()
        If Page.IsPostBack AndAlso Not egvNotifier.HasMessage() Then
            egvNotifier.Clear()
        End If
        html.Attributes.Add("dir", IIf(Helper.Language.IsRTL, "rtl", "ltr"))
        html.Attributes.Add("lang", Helper.Language.LanguageCode)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Try
                MyConn.Open()
                'site name
                Dim sitename As String = B.ReadSetting("SITENAME", MyConn)
                Dim parts() As String = Utils.Helper.SplitString(sitename, " ")
                Dim mini As String = "<b>" & parts(0).Substring(0, 1) & "</b>"
                For i As Integer = 1 To parts.Length - 1
                    mini &= parts(i).Substring(0, 1)
                Next
                parts(0) = "<b>" & parts(0) & "</b>"
                litLogoMini.Text = mini
                litLogo.Text = String.Join(" ", parts)
                litCMSVersion.Text = Utils.Helper.CMSVersion()
                Dim usr = Utils.Helper.CMSAuthUser
                'user name
                litUserName.Text = usr.Profile.FullName
                litUserName2.Text = usr.Profile.FullName
                litUserName3.Text = usr.Profile.FullName
                'user image
                imgBtn.ImageUrl = usr.Profile.GetProfileImage()
                imgProfile.ImageUrl = usr.Profile.GetProfileImage()
                imgSide.ImageUrl = usr.Profile.GetProfileImage()
                hypProfile.NavigateUrl = Utils.Path.MapCMSFile("security/users-editor.aspx?id=" & usr.Id)
                'menu
                rptMenu.DataSource = B.CMSMenuListParent(MyConn, IIf(Not usr.IsSuperAdmin, "M.IsSuper = 0", "")).List
                rptMenu.DataBind()
                'languages
                Dim languages = LanguageController.List(MyConn, Helper.LanguageId, True)
                If languages.Count > 1 Then
                    rptLanguages.DataSource = languages.List
                    rptLanguages.DataBind()
                    liLanguages.Visible = True
                End If
                'load titles
                If PageId > 0 Then
                    Dim obj As New Business.CMSMenu(PageId, MyConn)
                    LoadTitles(PageId, obj.ParentId)
                End If
                'CSS
                litAddStyle.Text = "<link rel=""icon"" href=""" & Utils.Path.MapCMSImage("fav.ico") & """ />"
                Dim styleTag As String = "<link rel=""stylesheet"" href=""{0}"" />"
                Dim styles() As String = {
                    Path.MapCMSCss("bootstrap"),
                    Path.MapCMSCss("bootstrap." & Helper.GetHTMLDirection()),
                    Path.MapCMSCss("font-awesome.min"),
                    Path.MapCMSCss("ionicons.min"),
                    Path.MapCMSCss("fonts"),
                    Path.MapCMSCss("arabic-fonts")
                }
                For Each s In styles
                    litBeforeStyle.Text &= String.Format(styleTag, s)
                Next
                styles = {
                    Path.MapCMSCss("styles.min." & Helper.GetHTMLDirection()),
                    Path.MapCMSCss("color.min." & Helper.GetHTMLDirection()),
                    Path.MapCMSCss("icheck"),
                    Path.MapCMSCss("custom") & "?v=1.1",
                    Path.MapCMSCss("custom." & Helper.GetHTMLDirection()) & "?v=1.0"
                }
                For Each s In styles
                    litAfterStyle.Text &= String.Format(styleTag, s)
                Next
                'messages
                Dim newMessagesCount As Integer = MessageController.GetNotViewedCount(MyConn, usr.Id, MessageUserTypes.User)
                litNewMessagesCount.Text = newMessagesCount
                litNewMessagesCount2.Text = newMessagesCount
                rptMessages.DataSource = MessageController.ListMessages(MyConn, usr.Id, MessageUserTypes.User, MessageUserRoleTypes.Received, 3, 0, String.Empty, False, False, True).List
                rptMessages.DataBind()
                litUnreadMessages.Text = MessageController.GetUnreadCount(MyConn, usr.Id, MessageUserTypes.User)
                litStarredMessages.Text = MessageController.GetStarredCount(MyConn, usr.Id, MessageUserTypes.User)
                liViewAll.Visible = usr.CanRead(26, MyConn)
            Catch ex As Exception
                Utils.ExceptionHandler.ProcessException(ex, egvNotifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub btnLogout_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSignout.Click
        Utils.Helper.UnloadUser()
        Response.Redirect(Utils.Path.MapCMSFile("login.aspx"))
    End Sub

    Protected Sub rpt_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptMenu.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim dr As DataRowView = e.Item.DataItem
            If dr IsNot Nothing Then
                If Business.CMSMenuController.HasChildren(dr("Id"), MyConn) Then
                    Dim rpt As Repeater = e.Item.FindControl("rptSub")
                    rpt.DataSource = B.CMSMenuListSubs(dr("Id"), MyConn, IIf(Not Utils.Helper.CMSAuthUser.IsSuperAdmin, "M.IsSuper = 0", "")).List
                    rpt.DataBind()
                    rpt.Visible = True
                End If
            End If
        End If
    End Sub

    Protected Sub lnkChangeLanguage_Click(sender As Object, e As EventArgs)
        Dim lnk As EGVControls.EGVLinkButton = DirectCast(sender, EGVControls.EGVLinkButton)
        Dim langId As Integer = lnk.CommandArgument
        Helper.GetSafeLanguageId(langId)
        Helper.SetLanguageId(langId)
        Helper.LoadCulture()
        Dim shouldRedirect As Boolean = True
        Try
            MyConn.Open()
            Dim obj As User = Helper.CMSAuthUser
            If obj.Profile IsNot Nothing Then obj.Profile.UserLanguageId = langId
            Dim trans As SqlTransaction = MyConn.BeginTransaction()
            Try
                obj.Save(trans)
                trans.Commit()
            Catch ex As Exception
                trans.Rollback()
                Throw ex
            End Try
            Helper.ReloadUser(MyConn)
        Catch ex As Exception
            shouldRedirect = False
            ExceptionHandler.ProcessException(ex, egvNotifier, MyConn)
        Finally
            MyConn.Close()
        End Try
        If shouldRedirect Then Response.Redirect(Request.RawUrl)
    End Sub

#End Region

End Class


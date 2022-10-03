Imports EGV.Utils
Imports EGV.Business
Imports EGV.Structures
Imports System.Data.SqlClient

Partial Class cms_messages_ViewAll
    Inherits AuthCMSPageBase

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ProcessCMD(Master.Notifier)
            Master.LoadTitles(Localization.GetResource("Resources.Local.PageTitle"), String.Empty, Localization.GetResource("Resources.Local.PageBC"))
            Try
                MyConn.Open()
                txtSearch.Text = SearchTerm
                BindMessages(MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
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

    Protected Sub lnkSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSearch.Click
        If txtSearch.Text <> String.Empty Then
            Response.Redirect("ViewAll.aspx?q=" & HttpUtility.UrlEncode(txtSearch.Text))
        Else
            Response.Redirect("ViewAll.aspx")
        End If
    End Sub

    Private Sub BindMessages(ByVal conn As SqlConnection)
        litFirst.Text = PageSize * PageIndex + 1
        litFirst2.Text = PageSize * PageIndex + 1
        Dim lst As DBAReturnObject = MessageController.GetAllMessages(conn, PageSize, PageIndex, SearchTerm)
        litTotal.Text = lst.Count
        litTotal2.Text = lst.Count
        Dim lastRecord As Integer = PageSize * PageIndex + PageSize
        litLast.Text = IIf(lastRecord > lst.Count, lst.Count, lastRecord)
        litLast2.Text = IIf(lastRecord > lst.Count, lst.Count, lastRecord)
        Dim numOfPages As Integer = Math.Ceiling(lst.Count / PageSize)
        hypPrevious.NavigateUrl = Path.MapCMSFile("messages/ViewAll.aspx?page=" & IIf(PageIndex = 0, 0, PageIndex - 1))
        hypPrevious2.NavigateUrl = hypPrevious.NavigateUrl
        hypNext.NavigateUrl = Path.MapCMSFile("messages/ViewAll.aspx?page=" & IIf(PageIndex = numOfPages - 1, numOfPages - 1, PageIndex + 1))
        hypNext2.NavigateUrl = hypNext2.NavigateUrl
        rptMessages.DataSource = lst.List
        rptMessages.DataBind()
    End Sub

End Class

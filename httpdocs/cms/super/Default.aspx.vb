Imports EGV.Business
Imports EGV.Utils

Partial Class cms_super_Default
    Inherits AuthCMSPageBase

#Region "Public Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Try
                MyConn.Open()
                ProcessPermissions(AuthUser, PageId, MyConn)
                Dim obj As New CMSMenu(PageId, MyConn)
                egvListView.Title = obj.PageTitle
                egvListView.IconClass = obj.IconClass
                egvListView.BindToDataSource(CMSMenuController.ListSubs(PageId, MyConn).List, "IconClass", "Title", "PagePath")
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

#End Region

End Class

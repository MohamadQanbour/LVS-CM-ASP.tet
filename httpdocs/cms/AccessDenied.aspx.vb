Imports EGV.Utils

Partial Class cms_AccessDenied
    Inherits CMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim res As String = Localization.GetResource("Resources.Local.AccessDenied")
            Master.LoadTitles(res, res, res)
        End If
    End Sub

#End Region

End Class

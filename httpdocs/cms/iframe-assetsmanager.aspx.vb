Imports EGV.Utils
Partial Class cms_iframe_assetsmanager
    Inherits System.Web.UI.Page

    Protected Sub am_Init(ByVal sender As Object, ByVal e As EventArgs) Handles egvAssetsManager.Init
        If Request("fullpath") IsNot Nothing AndAlso Request("fullpath").ToString().ToLower() = "true" Then egvAssetsManager.ReturnFullURL = True
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Helper.LoadCulture()
        Dim styles() As String = {
            Path.MapCMSCss("bootstrap"),
            Path.MapCMSCss("bootstrap." & Helper.GetHTMLDirection()),
            Path.MapCMSCss("font-awesome.min")
        }
        For Each s As String In styles
            litStyle.Text &= "<link rel=""stylesheet"" href=""" & s & """ />"
        Next
    End Sub

End Class

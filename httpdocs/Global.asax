<%@ Application Language="VB" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="EGV" %>

<script runat="server">

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        'Register Image Crop Routes
        Dim routeHandler As New EGVControls.ImageCrop.ImageCropRouteHandler()
        RouteTable.Routes.Add("ImageCropRouteWidthHeightCropScale", New Route("files/base/{base}/width/{width}/height/{height}/crop/{cropRatio}/scale/{scale}/{*path}", routeHandler))
        RouteTable.Routes.Add("ImageCropRouteWidthHeightCrop", New Route("files/base/{base}/width/{width}/height/{height}/crop/{cropRatio}/{*path}", routeHandler))
        RouteTable.Routes.Add("ImageCropRouteWidthHeight", New Route("files/base/{base}/width/{width}/height/{height}/{*path}", routeHandler))
        RouteTable.Routes.Add("ImageCropRoute", New Route("files/base/{base}/{*path}", routeHandler))
        'Register Ajax Routes
        Dim ajaxHandler As New EGVControls.AjaxRouteHandler()
        RouteTable.Routes.Add("AjaxHandler", New Route("ajax/{PageHandler}/{FunctionName}", ajaxHandler))
        'Register Download Routes
        Dim downloadHandler As New EGVControls.DownloadRouteHandler()
        RouteTable.Routes.Add("DownloadAttachmentHandler", New Route("download-attachment/{AttachmentId}", downloadHandler))
        ' Activate URL Rewriting
        If Utils.Helper.EnableURLRewrite() Then RegisterRoutes(RouteTable.Routes)
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
    End Sub

    Private Sub RegisterRoutes(ByVal routes As RouteCollection)
        routes.Ignore("CaptchaImage.axd")
        routes.Ignore("ImageCrop.axd")
        routes.Ignore("images.axd")
        routes.Ignore("ajax.axd")
        routes.Ignore("WebResource.axd")
        routes.Ignore("ScriptResource.axd")
        routes.Ignore("download-attachment.axd")
        routes.Ignore(Utils.Helper.CMSPath & "/{*path}")
        routes.Ignore(Utils.Helper.AssetsPath & "/{*path}")
        If Utils.Helper.AdvancedURLRewrite() Then
            Dim defaultPortal As String = Utils.Helper.PortalPrefix()
            If Utils.Helper.UseLanguagePrefix() Then
                routes.MapPageRoute("WithLanguage", IIf(Utils.Helper.UsePortalPrefix(), Utils.Helper.PortalPrefix() & "/", "") & "{Locale}/{*Path}", "~/Default.aspx")
            Else
                routes.MapPageRoute("WithoutLanguage", IIf(Utils.Helper.UsePortalPrefix(), Utils.Helper.PortalPrefix() & "/", "") & "{*Path}", "~/Default.aspx")
            End If
        Else
            routes.MapPageRoute("Menu", IIf(Utils.Helper.UsePortalPrefix(), Utils.Helper.PortalPrefix() & "/", "") & "{Locale}/{MenuTitle}/{MenuId}/", "~/Default.aspx")
            routes.MapPageRoute("MenuContent", IIf(Utils.Helper.UsePortalPrefix(), Utils.Helper.PortalPrefix() & "/", "") & "{Locale}/{MenuTitle}/{MenuId}/c/{ContentTitle}/{ContentId}/", "~/Default.aspx")
            routes.MapPageRoute("Album", IIf(Utils.Helper.UsePortalPrefix(), Utils.Helper.PortalPrefix() & "/", "") & "{Locale}/{MenuTitle}/{MenuId}/a/{AlbumTitle}/{AlbumId}/", "~/Default.aspx")
            routes.MapPageRoute("MenuContentAlbum", IIf(Utils.Helper.UsePortalPrefix(), Utils.Helper.PortalPrefix() & "/", "") & "{Locale}/{MenuTitle}/{MenuId}/c/{ContentTitle}/{ContentId}/a/{AlbumTitle}/{AlbumId}/", "~/Default.aspx")
        End If
    End Sub

</script>
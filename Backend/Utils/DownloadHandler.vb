Imports System.Web
Imports System.Web.Routing
Imports System.Web.SessionState
Imports System.Reflection
Imports EGV
Imports EGV.Business
Imports System.Data.SqlClient
Imports EGV.Utils
Imports System.IO

Namespace EGVControls

#Region "Route Handler"

    Public Class DownloadRouteHandler
        Implements IRouteHandler

        Public Function GetHttpHandler(requestContext As RequestContext) As IHttpHandler Implements IRouteHandler.GetHttpHandler
            Dim handler As IHttpHandler = Activator.CreateInstance(Of DownloadHandler)()
            If TypeOf handler Is DownloadHandler Then
                CType(handler, DownloadHandler).RouteData = requestContext.RouteData
            End If
            Return handler
        End Function

    End Class

    Public Class DownloadHandler
        Implements IHttpHandler
        Implements IRequiresSessionState

#Region "Public Properties"

        Public Property RouteData As RouteData

        Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Function ReadRouteValue(ByVal key As String) As String
            If RouteData.Values(key) IsNot Nothing Then
                Return RouteData.Values(key)
            Else
                Return ""
            End If
        End Function

        Private Function fetchInstance(ByVal fullyQualifiedClassName As String) As Object
            Dim nspc As String = fullyQualifiedClassName
            Dim o As Object = Nothing
            Try
                For Each ay In Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                    If (ay.Name = nspc) Then
                        o = Assembly.Load(ay).CreateInstance(fullyQualifiedClassName)
                        Exit For
                    End If
                Next
            Catch
            End Try
            Return o
        End Function

#End Region

#Region "Public Methods"

        Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
            Dim appInstatnce As HttpApplication = context.ApplicationInstance
            Dim attId As Integer = ReadRouteValue("AttachmentId")
            If attId > 0 Then
                Dim conn As SqlConnection = DBA.GetConn()
                Try
                    conn.Open()
                    Dim obj As New MessageAttachment(attId, conn)
                    Dim filePath As String = Helper.Server.MapPath("~" & Utils.Path.MapCMSAsset(obj.FilePath))
                    Dim file As New FileInfo(filePath)
                    If file.Exists Then
                        Helper.Response.ClearContent()
                        Helper.Response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
                        Helper.Response.AddHeader("Content-Length", file.Length.ToString())
                        Helper.Response.ContentType = file.Extension
                        Helper.Response.TransmitFile(file.FullName)
                    End If
                Catch ex As Exception
                    Throw ex
                Finally
                    conn.Close()
                    Helper.Response.End()
                End Try
            Else
                appInstatnce.Response.StatusCode = 404
            End If
            appInstatnce.CompleteRequest()
        End Sub

#End Region

    End Class

#End Region

End Namespace
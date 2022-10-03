Imports System.Web
Imports System.Web.Routing
Imports System.Web.SessionState
Imports System.Reflection
Imports System.Web.UI

Namespace EGVControls

#Region "Ajax Base Class"

    Public Class AjaxFirstBaseClass

#Region "Event Handler"

        Public Overridable Function Load(ByVal tFunc As String, Optional ByVal p As HttpApplication = Nothing) As String
            Return String.Empty
        End Function

#End Region

#Region "Structures"

        Public Structure ReturnObject
            Public Property HasError As Boolean
            Public Property ErrorMessage As String
            Public Property ReturnData As Object
        End Structure

#End Region

    End Class

#End Region

#Region "Ajax Handler"

    Public Class AjaxRouteHandler
        Implements IRouteHandler

        Public Function GetHttpHandler(requestContext As RequestContext) As IHttpHandler Implements IRouteHandler.GetHttpHandler
            Dim handler As IHttpHandler = Activator.CreateInstance(Of AjaxHandler)()
            If TypeOf handler Is AjaxHandler Then
                CType(handler, AjaxHandler).RouteData = requestContext.RouteData
            End If
            Return handler
        End Function

    End Class

    Public Class AjaxHandler
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
            Dim pageHandler As String = ReadRouteValue("PageHandler")
            Dim functionName As String = ReadRouteValue("FunctionName")
            If pageHandler <> String.Empty AndAlso functionName <> String.Empty Then
                Dim o = Activator.CreateInstance(Assembly.Load("Backend").GetType("Ajax." & pageHandler))
                appInstatnce.Response.Charset = Text.Encoding.UTF8.WebName
                appInstatnce.Response.Write(CType(o, AjaxFirstBaseClass).Load(functionName, appInstatnce))
            Else
                appInstatnce.Response.StatusCode = 404
            End If
            appInstatnce.CompleteRequest()
        End Sub

#End Region

    End Class

#End Region

End Namespace
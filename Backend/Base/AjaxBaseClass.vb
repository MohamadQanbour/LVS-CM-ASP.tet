Imports System.Web
Imports EGV
Imports EGV.Enums
Imports EGV.Utils
Imports System.Data.SqlClient
Imports EGVControls

Public Class SecureAjaxBaseClass
    Inherits AjaxBaseClass

#Region "Private Members"

    Private _SecurityKey As String

#End Region

#Region "Public Properties"

    Public ReadOnly Property SecurityKey As String
        Get
            If _SecurityKey = String.Empty Then _SecurityKey = GetSafeRequestValue("security_key")
            Return _SecurityKey
        End Get
    End Property

#End Region

#Region "Event Handler"

    Public Overrides Function Load(ByVal tFunc As String, Optional ByVal p As HttpApplication = Nothing) As String
        SetTargetFunction(tFunc)
        SetMyPage(p)
        Dim retObj As New ReturnObject() With {.HasError = False, .ErrorMessage = String.Empty, .ReturnData = Nothing}
        Try
            MyConn.Open()
            If IsSecure() Then
                retObj.ReturnData = ProcessAjaxRequest(MyConn, LanguageId)
            Else
                Throw New Exception("Security Check Failed")
            End If
        Catch ex As Exception
            retObj.HasError = True
            retObj.ErrorMessage = ex.Message & IIf(Helper.DebuggingEnabled(), " : " & ex.StackTrace, "")
        Finally
            MyConn.Close()
        End Try
        Return "[" & Helper.JSSerialize(retObj) & "]"
    End Function

#End Region

#Region "Private Methods"

    Private Function IsSecure() As Boolean
        If Helper.IsRemote() Then
            Return Security.IsSecure(SecurityKey)
        Else
            Return True
        End If
    End Function

#End Region

End Class

Public Class AjaxBaseClass
    Inherits AjaxFirstBaseClass

#Region "Private Members"

    Private Property _MyConn As SqlConnection = Nothing
    Private Property _LanguageId As Integer = 0
    Private _MyPage As HttpApplication
    Private _TargetFunction As String

#End Region

#Region "Public Properties"

    Public ReadOnly Property MyConn As SqlConnection
        Get
            If _MyConn Is Nothing Then _MyConn = DBA.GetConn()
            Return _MyConn
        End Get
    End Property

    Public ReadOnly Property LanguageId As Integer
        Get
            If _LanguageId = 0 Then
                _LanguageId = GetSafeRequestValue("lang", ValueTypes.TypeInteger)
                If _LanguageId = 0 Then _LanguageId = Helper.LanguageId
            End If
            Return _LanguageId
        End Get
    End Property

    Public ReadOnly Property TargetFunction As String
        Get
            Return _TargetFunction
        End Get
    End Property

    Public ReadOnly Property MyPage As HttpApplication
        Get
            If _MyPage IsNot Nothing Then Return _MyPage Else Return HttpContext.Current.ApplicationInstance
        End Get
    End Property

#End Region

#Region "Public Methods"

    Public Function Safe(ByVal field As Object, Optional ByVal type As ValueTypes = ValueTypes.TypeString) As Object
        Return Helper.GetSafeDBValue(field, type)
    End Function

    Public Function GetSafeRequestValue(ByVal key As String, Optional ByVal type As ValueTypes = ValueTypes.TypeString) As Object
        Return Helper.GetSafeObject(MyPage.Request(key), type)
    End Function

    Public Sub SetMyPage(ByVal p As HttpApplication)
        _MyPage = p
    End Sub

    Public Sub SetTargetFunction(ByVal f As String)
        _TargetFunction = f
    End Sub

#End Region

#Region "Event Handler"

    Public Overrides Function Load(ByVal tFunc As String, Optional ByVal p As HttpApplication = Nothing) As String
        _TargetFunction = tFunc
        _MyPage = p
        Dim retObj As New ReturnObject() With {.HasError = False, .ErrorMessage = String.Empty, .ReturnData = Nothing}
        Try
            MyConn.Open()
            retObj.ReturnData = ProcessAjaxRequest(MyConn, LanguageId)
        Catch ex As Exception
            retObj.HasError = True
            retObj.ErrorMessage = ex.Message & IIf(Helper.DebuggingEnabled(), " : " & ex.StackTrace, "")
            If MyConn.State = ConnectionState.Open Then Business.EGVExceptionController.AddException(ex, MyConn)
        Finally
            MyConn.Close()
        End Try
        Return "[" & Helper.JSSerialize(retObj) & "]"
    End Function

#End Region

#Region "Overridable Methods"

    Public Overridable Function ProcessAjaxRequest(ByVal conn As SqlConnection, Optional ByVal langId As Integer = 0) As Object
        If langId = 0 Then langId = LanguageId
        Return Nothing
    End Function

#End Region

End Class

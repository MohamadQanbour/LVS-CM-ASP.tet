Imports System.Data.SqlClient
Imports EGV

Public Class CMSPageBase
    Inherits Page

#Region "Private Members"

    Private Property _MyConn As SqlConnection = Nothing
    Private _pageId As Integer = 0

#End Region

#Region "Public Properties"

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

    Public ReadOnly Property LanguageId As Integer
        Get
            Return Utils.Helper.LanguageId
        End Get
    End Property

#End Region

#Region "Public Methods"

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        _MyConn = DBA.GetConn()
        If Utils.Helper.CMSAuthUser IsNot Nothing Then
            Utils.Helper.GetSafeLanguageId(Utils.Helper.CMSAuthUser.Profile.UserLanguageId)
            Utils.Helper.SetLanguageId(Utils.Helper.CMSAuthUser.Profile.UserLanguageId)
        Else
            Utils.Helper.SetLanguageId(Business.LanguageController.GetDefaultId())
        End If
        Utils.Helper.LoadCulture()
    End Sub

#End Region

End Class

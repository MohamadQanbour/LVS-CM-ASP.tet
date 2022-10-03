Imports System.Data
Imports System.Data.SqlClient

Namespace EGV
    Namespace Business
        Public Class BusinessHelper

            'Settings
            Public Shared Function ReadSetting(ByVal key As String, Optional ByVal conn As SqlConnection = Nothing) As String
                Return SettingController.ReadSetting(key, conn)
            End Function

            Public Shared Sub WriteSetting(ByVal key As String, ByVal val As String, Optional ByVal conn As SqlConnection = Nothing)
                SettingController.WriteSetting(key, val, conn)
            End Sub

            'User
            Public Shared Function UserLogin(ByVal username As String, ByVal password As String, Optional ByVal conn As SqlConnection = Nothing) As User
                Return UserController.Login(username, password, conn)
            End Function

            'CMS Menu
            Public Shared Function CMSMenuListParent(ByVal conn As SqlConnection, ByVal ParamArray conditions() As String) As Structures.DBAReturnObject
                Return CMSMenuController.ListParents(conn, conditions)
            End Function

            Public Shared Function CMSMenuListSubs(ByVal id As Integer, ByVal conn As SqlConnection, ByVal ParamArray conditions() As String) As Structures.DBAReturnObject
                Return CMSMenuController.ListSubs(id, conn, conditions)
            End Function

        End Class
    End Namespace
End Namespace
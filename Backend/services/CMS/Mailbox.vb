Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Enums
Imports EGV

Namespace Ajax

    Public Class Mailbox
        Inherits AjaxBaseClass

#Region "Request Values"

        Private ReadOnly Property MessageId As String = GetSafeRequestValue("message_id")
        Private ReadOnly Property Star As Boolean = GetSafeRequestValue("star", ValueTypes.TypeBoolean)
        Private ReadOnly Property MessageType As MessageUserRoleTypes = GetSafeRequestValue("message_type", ValueTypes.TypeInteger)

#End Region

#Region "Public Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "StarMessage"
                    ret = StarMessage(MyConn, LanguageId)
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function StarMessage(ByVal conn As SqlConnection, ByVal langId As Integer) As Boolean
            Dim ret As Boolean = True
            MessageController.ToggleStar(MessageId, Helper.CMSAuthUser.Id, MessageType, Star, conn)
            Return ret
        End Function

#End Region

    End Class

End Namespace
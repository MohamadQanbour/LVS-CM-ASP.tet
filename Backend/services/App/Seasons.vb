Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Enums

Namespace Ajax

    Public Class Seasons
        Inherits SecureAjaxBaseClass

#Region "Request Values"



#End Region

#Region "Overridden Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "List"
                    ret = ListSeasons(MyConn, LanguageId)
                Case "GetCurrent"
                    ret = GetCurrent(MyConn, LanguageId)
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function ListSeasons(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of SeasonObject)
            Dim ret As New List(Of SeasonObject)
            Using dt As DataTable = SeasonController.GetCollection(conn, langId).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New SeasonObject() With {
                        .Id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .Title = Safe(dr("Title")),
                        .IsCurrent = Safe(dr("IsCurrent"), ValueTypes.TypeBoolean)
                    })
                Next
            End Using
            Return ret
        End Function

        Private Function GetCurrent(ByVal conn As SqlConnection, ByVal langId As Integer) As SeasonObject
            Dim obj As Season = SeasonController.GetCurrent(conn, langId)
            If obj IsNot Nothing Then
                Return New SeasonObject() With {
                    .Id = obj.Id,
                    .Title = obj.Title,
                    .IsCurrent = obj.IsCurrent
                }
            Else
                Return Nothing
            End If
        End Function

#End Region

    End Class

End Namespace
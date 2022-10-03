
Imports System.Data.SqlClient

Namespace Ajax

    Public Class Testing
        Inherits SecureAjaxBaseClass

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Dim lst As New List(Of URLItem)
            Dim request = EGV.Utils.Helper.Request
            For Each k As String In request.QueryString
                lst.Add(New URLItem() With {.Type = "GET", .Key = k, .Value = request.QueryString(k)})
            Next
            For Each k As String In request.Form
                lst.Add(New URLItem() With {.Type = "Form", .Key = k, .Value = request.Form(k)})
            Next
            ret = lst
            Return ret
        End Function

        Public Structure URLItem
            Public Property Type As String
            Public Property Key As String
            Public Property Value As String
        End Structure

    End Class

End Namespace
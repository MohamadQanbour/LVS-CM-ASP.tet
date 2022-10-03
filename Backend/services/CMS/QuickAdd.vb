Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Enums
Imports EGV

Namespace Ajax

    Public Class AutoAdd
        Inherits AjaxBaseClass

#Region "Request Values"

        Public Property Title As String = GetSafeRequestValue("title")
        Public Property UserName As String = GetSafeRequestValue("username")
        Public Property Code As String = GetSafeRequestValue("code")

#End Region

#Region "Overridden Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            If Helper.CMSAuthUser IsNot Nothing Then
                Dim ret As Object = Nothing
                Select Case TargetFunction
                    Case "Area"
                        ret = AutoAddArea(MyConn, LanguageId)
                    Case "Family"
                        ret = AutoAddFamily(MyConn, LanguageId)
                    Case "StudyClass"
                        ret = AutoAddClass(MyConn, LanguageId)
                End Select
                Return ret
            Else
                Throw New Exception("Access Denied")
            End If
        End Function

#End Region

#Region "Private Methods"

        Private Function AutoAddArea(ByVal conn As SqlConnection, ByVal langId As Integer) As AutoComplete
            Dim obj As New Area(0, conn, langId)
            obj.Title = Title
            Dim trans As SqlTransaction = conn.BeginTransaction()
            Try
                obj.Save(trans)
                trans.Commit()
            Catch ex As Exception
                trans.Rollback()
                Throw ex
            End Try
            Return New AutoComplete() With {.id = obj.Id, .text = obj.Title}
        End Function

        Private Function AutoAddFamily(ByVal conn As SqlConnection, ByVal langId As Integer) As AutoComplete
            Dim obj As New Family(0, conn)
            obj.IsActive = True
            obj.Password = Helper.GeneratePassword()
            obj.SchoolId = UserName
            Dim trans As SqlTransaction = conn.BeginTransaction()
            Try
                obj.Save(trans)
                trans.Commit()
            Catch ex As Exception
                trans.Rollback()
                Throw ex
            End Try
            Return New AutoComplete() With {.id = obj.Id, .text = obj.SchoolId}
        End Function

        Public Function AutoAddClass(ByVal conn As SqlConnection, ByVal langId As Integer) As AutoComplete
            Dim obj As New StudyClass(0, conn)
            obj.Code = Code
            obj.Title = Title
            Dim trans As SqlTransaction = conn.BeginTransaction()
            Try
                obj.Save(trans)
                trans.Commit()
            Catch ex As Exception
                trans.Rollback()
                Throw ex
            End Try
            Return New AutoComplete() With {.id = obj.Id, .text = obj.Code & " - " & obj.Title}
        End Function

#End Region

    End Class

End Namespace
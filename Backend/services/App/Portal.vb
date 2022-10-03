Imports System.Data.SqlClient
Imports EGV.Constants
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Utils
Imports EGV.Enums

Namespace Ajax
    Public Class Portal
        Inherits AjaxBaseClass

#Region "Public Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "FindStudent"
                    ret = FindStudent(conn)
                Case "FindFamily"
                    ret = FindFamily(conn)
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function FindStudent(ByVal conn As SqlConnection) As Object
            Dim studentId As String = GetSafeRequestValue("stu_id")
            Dim questionId As String = GetSafeRequestValue("question_id")
            Dim answerId As String = GetSafeRequestValue("answer_id")
            Dim objId As Integer = StudentController.GetBySchoolId(studentId, conn)
            If objId <> Nothing AndAlso objId > 0 Then
                Dim obj As New Student(objId, conn)
                Dim objSection As New Section(obj.SectionId, conn)
                Dim objClass As New StudyClass(objSection.ClassId, conn)
                If obj.IsActive Then
                    Return New With {
                        .Valid = True,
                        .QuestionId = questionId,
                        .AnswerId = answerId,
                        .Id = studentId,
                        .Name = obj.FullName,
                        .Father = obj.FatherName,
                        .ClassName = objClass.Title,
                        .SectionName = objSection.Title
                    }
                Else
                    Return New With {
                        .Valid = False,
                        .QuestionId = questionId,
                        .AnswerId = answerId
                    }
                End If
            Else
                Return New With {.Valid = False, .QuestionId = questionId, .AnswerId = answerId, .Id = studentId}
            End If
        End Function

        Private Function FindFamily(ByVal conn As SqlConnection) As Object
            Dim famId As String = GetSafeRequestValue("fam_id")
            Dim questionId As String = GetSafeRequestValue("question_id")
            Dim answerId As String = GetSafeRequestValue("answer_id")
            Dim obj = FamilyController.GetByUsername(famId, conn)
            If obj IsNot Nothing AndAlso obj.IsActive Then
                Return New With {.Valid = True, .QuestionId = questionId, .AnswerId = answerId, .Id = famId}
            Else
                Return New With {.Valid = False, .QuestionId = questionId, .AnswerId = answerId, .Id = famId}
            End If
        End Function

#End Region

    End Class
End Namespace
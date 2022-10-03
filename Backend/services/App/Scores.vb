Imports EGV.Structures
Imports System.Data.SqlClient
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Utils

Namespace Ajax

    Public Class Scores
        Inherits SecureAjaxBaseClass

#Region "Request Parameters"

        Public ReadOnly Property AccessToken As String = GetSafeRequestValue("access_token")

#End Region

#Region "Public Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "GetCurrent"
                    ret = GetStudentExamMarks(MyConn, LanguageId)
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function GetStudentExamMarks(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of StudentMaterialExamsItem)
            'Access Token
            Dim usr As UserIdInformation = GetUserInformation(conn, AccessToken)
            Dim classId As Integer = StudentController.GetClassId(usr.UserId, conn)
            Dim materials = StudentController.GetStudentMaterials(usr.UserId, 0, classId, langId, conn)
            Dim lst As New List(Of StudentMaterialExamsItem)
            If materials.Count > 0 Then
                For Each dr As DataRow In materials.List.Rows
                    Dim item As New StudentMaterialExamsItem() With {
                        .MaterialId = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .MaterialTitle = Safe(dr("MaterialTitle")),
                        .MaterialMaxMark = Safe(dr("MaxMark"), ValueTypes.TypeInteger)
                    }
                    item.Exams = New List(Of EGV.Structures.ExamItem)()
                    Dim templateId As Integer = MaterialController.GetTemplateId(conn, item.MaterialId)
                    Dim templateItems = ExamTemplateItemController.GetTemplateItems(templateId, langId, conn)
                    Dim materialItems = MaterialExamTemplateItemController.GetMaterialItems(item.MaterialId, templateId, conn, langId)
                    Dim results = StudentExamController.GetResults(conn, item.MaterialId, usr.UserId, False, True)
                    For Each template In templateItems
                        Dim aItem As New EGV.Structures.ExamItem() With {
                            .ExamTitle = template.Title,
                            .ExamType = template.Type
                        }
                        Dim target = (From r In results Where r.ItemId = template.Id)
                        If target.Count > 0 Then
                            aItem.ExamMark = target.FirstOrDefault().Mark
                        Else
                            aItem.ExamMark = 0
                        End If
                        Dim materialTarget = (From m In materialItems Where m.TemplateItemId = template.Id)
                        If materialTarget.Count > 0 Then
                            aItem.ExamMaxMark = materialTarget.FirstOrDefault().MaxMark
                        Else
                            If template.Type = ExamItemTypes.Total Then
                                aItem.ExamMaxMark = MaterialExamTemplateItemController.GetTotalMaxMark(item.MaterialId, template.Id, conn)
                            ElseIf template.Type = ExamItemTypes.Average Then
                                aItem.ExamMaxMark = MaterialExamTemplateItemController.GetAverageMaxMark(item.MaterialId, template.Id, conn)
                            Else
                                aItem.ExamMaxMark = item.MaterialMaxMark
                            End If
                            If aItem.ExamMaxMark = 0 Then aItem.ExamMaxMark = item.MaterialMaxMark
                        End If
                        item.Exams.Add(aItem)
                    Next
                    lst.Add(item)
                Next
            End If
            Return lst
        End Function

#End Region

#Region "Helper"

        Private Function GetUserInformation(ByVal conn As SqlConnection, ByVal accessToken As String) As UserIdInformation
            Dim ret As New UserIdInformation
            If StudentController.AccessTokenExists(accessToken, conn) Then
                Dim obj As Student = StudentController.GetByAccessToken(accessToken, conn)
                ret.UserId = obj.Id
                ret.UserType = MessageUserTypes.Student
            Else
                Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.UserNotFound"))
            End If
            Return ret
        End Function

#End Region

    End Class

End Namespace
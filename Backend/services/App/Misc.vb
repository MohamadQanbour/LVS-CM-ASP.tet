Imports System.Data.SqlClient
Imports EGV.Constants
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Utils
Imports EGV.Enums

Namespace Ajax

    Public Class Misc
        Inherits SecureAjaxBaseClass

#Region "Request Values"

        Private ReadOnly Property AccessToken As String = GetSafeRequestValue("access_token")

#End Region

#Region "Public Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "ScheduleFile"
                    ret = GetScheduleFile(MyConn, LanguageId)
                Case "TeachersList"
                    ret = TeachersList(MyConn, LanguageId)
                Case "StaffList"
                    ret = GetStaffList(MyConn, LanguageId)
                Case "GetNotes"
                    ret = GetNotes(MyConn, LanguageId)
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function GetScheduleFile(ByVal conn As SqlConnection, ByVal langId As Integer) As String
            Return StudentController.GetScheduleFilePath(conn, AccessToken)
        End Function

        Private Function TeachersList(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of TeacherListItem)
            Dim lst As New List(Of TeacherListItem)
            Using dt As DataTable = UserController.GetCollection(MyConn, 0, String.Empty, True, {UserTypeRoleController.GetRoleOfType(MyConn, UserTypes.Teacher)}, True).List
                For Each dr As DataRow In dt.Rows
                    Dim name As String = Helper.GetSafeDBValue(dr("FullName"))
                    If name = String.Empty Then name = Helper.GetSafeDBValue(dr("UserName"))
                    Dim item As New TeacherListItem() With {.TeacherName = name}
                    item.Materials = New List(Of String)()
                    Using mdt As DataTable = MaterialController.GetTeacherFullMaterials(MyConn, Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger), langId).List
                        For Each mdr As DataRow In mdt.Rows
                            item.Materials.Add(Helper.GetSafeDBValue(mdr("Title")))
                        Next
                    End Using
                    lst.Add(item)
                Next
            End Using
            Return lst
        End Function

        Private Function GetStaffList(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of StaffListItem)
            Dim lst As New List(Of StaffListItem)
            Dim roles() As String = {
                UserTypes.ChiefOfClasses, UserTypes.ClassAdministrator, UserTypes.ExamSupervisor,
                UserTypes.StudentAffairs, UserTypes.Supervisor, UserTypes.SystemAdmin
            }
            For Each item As String In roles
                Dim ret = UserController.GetCollection(MyConn, 0, String.Empty, True, {UserTypeRoleController.GetRoleOfType(conn, item)}, True)
                If ret.Count > 0 Then
                    Dim role As New Role(UserTypeRoleController.GetRoleOfType(conn, item), conn)
                    Dim a As New StaffListItem() With {.RoleId = role.Id, .Role = item, .RoleName = role.Title}
                    a.Staff = New List(Of String)()
                    For Each dr As DataRow In ret.List.Rows
                        Dim name As String = Helper.GetSafeDBValue(dr("FullName"))
                        If name = String.Empty Then name = Helper.GetSafeDBValue(dr("UserName"))
                        a.Staff.Add(name)
                    Next
                    lst.Add(a)
                End If
            Next
            Return lst
        End Function

        Private Function GetNotes(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of NoteItem)
            Dim lst As New List(Of NoteItem)
            Dim id As Integer = 0
            Dim isFamily As Boolean = False
            If FamilyController.AccessTokenExists(AccessToken, conn) Then
                id = FamilyController.GetByAccessToken(AccessToken, conn).Id
                isFamily = True
            ElseIf StudentController.AccessTokenExists(AccessToken, conn) Then
                id = StudentController.GetByAccessToken(AccessToken, conn).Id
                isFamily = False
            End If
            If id > 0 Then
                Dim dt As DataTable = Nothing
                If isFamily Then
                    dt = NoteController.GetFamilyNotes(id, conn).List
                Else
                    dt = NoteController.GetStudentNotes(id, conn).List
                End If
                If dt IsNot Nothing Then
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New NoteItem() With {
                            .Id = Safe(dr("Id"), ValueTypes.TypeInteger),
                            .SenderId = Safe(dr("SenderId"), ValueTypes.TypeInteger),
                            .StudentId = Safe(dr("StudentId"), ValueTypes.TypeInteger),
                            .NoteType = Safe(dr("NoteType"), ValueTypes.TypeInteger),
                            .NoteDate = CDate(Safe(dr("NoteDate"), ValueTypes.TypeDate)).ToString("MMMM dd, yyyy"),
                            .NoteText = Safe(dr("NoteText")),
                            .SenderName = Safe(dr("UserName")),
                            .StudentName = Safe(dr("StudentName")),
                            .StudentSchoolId = Safe(dr("SchoolId"))
                        })
                    Next
                End If
            End If
            Return lst
        End Function

#End Region

    End Class

End Namespace
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Enums
Imports EGV

Namespace Ajax

    Public Class Attendance
        Inherits SecureAjaxBaseClass

#Region "Request Values"

        Public ReadOnly Property AccessToken As String = GetSafeRequestValue("access_token")

#End Region

#Region "Public Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "GetAttendance"
                    ret = GetStudentAttendance(MyConn, LanguageId)
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function GetStudentAttendance(ByVal conn As SqlConnection, ByVal langId As Integer) As StudentAttendance
            Dim usrId As UserIdInformation = GetUserInformation(conn, AccessToken)
            If usrId.UserType = MembershipTypes.Student Then
                Return StudentPresentController.GetStudentAttendance(conn, usrId.UserId)
            Else
                Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.AttendanceError"))
            End If
        End Function

#End Region

#Region "Helper"

        Private Function GetFullName(ByVal conn As SqlConnection, ByVal userId As Integer, ByVal userType As MessageUserTypes) As String
            Select Case userType
                Case MessageUserTypes.Family
                    Return FamilyController.GetSchoolId(userId, conn) & " - " & FamilyController.GetFullName(userId, conn)
                Case MessageUserTypes.Student
                    Return StudentController.GetSchoolId(userId, conn) & " - " & StudentController.GetFullName(userId, conn)
                Case MessageUserTypes.User
                    Return UserController.GetFullName(userId, conn)
                Case Else
                    Return "UNKNOWN"
            End Select
        End Function

        Private Function GetUserInformation(ByVal conn As SqlConnection, ByVal accessToken As String) As UserIdInformation
            Dim ret As New UserIdInformation
            If FamilyController.AccessTokenExists(accessToken, conn) Then
                Dim obj As Family = FamilyController.GetByAccessToken(accessToken, conn)
                ret.UserId = obj.Id
                ret.UserType = MessageUserTypes.Family
            ElseIf StudentController.AccessTokenExists(accessToken, conn) Then
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
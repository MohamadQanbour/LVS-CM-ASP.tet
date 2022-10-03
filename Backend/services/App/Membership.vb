Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Enums
Imports EGV

Namespace Ajax

    Public Class Membership
        Inherits SecureAjaxBaseClass

#Region "Request Values"

        Public ReadOnly Property UserName As String = GetSafeRequestValue("username")
        Public ReadOnly Property Password As String = GetSafeRequestValue("password")
        Public ReadOnly Property AccessToken As String = GetSafeRequestValue("access_token")
        Public ReadOnly Property OldPassword As String = GetSafeRequestValue("old_password")
        Public ReadOnly Property NewPassword As String = GetSafeRequestValue("new_password")
        Public ReadOnly Property DeviceToken As String = GetSafeRequestValue("device_token")
        Public ReadOnly Property Disable As Boolean = GetSafeRequestValue("disable", ValueTypes.TypeBoolean)

#End Region

#Region "Overridden Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "Login"
                    ret = Login(MyConn, LanguageId)
                Case "Profile"
                    ret = GetProfile(MyConn, LanguageId)
                Case "ChangePassword"
                    ret = ChangePassword(MyConn, LanguageId)
                Case "StudentsOfFamily"
                    ret = ListStudentsOfFamily(MyConn, LanguageId)
                Case "RegisterDevice"
                    ret = RegisterDevice(MyConn, LanguageId)
                Case "UpdateDevice"
                    ret = UpdateDevice(MyConn, LanguageId)
                Case "Payments"
                    ret = GetPayments(MyConn)
                Case "UnregisterDevice"
                    ret = UnregisterDevice(MyConn)
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function Login(ByVal conn As SqlConnection, ByVal langId As Integer) As AppLoginToken
            Dim membership As MembershipTypes = MembershipTypes.Student
            If FamilyController.UsernameExists(UserName, 0, conn) Then membership = MembershipTypes.Family
            Dim ret As New AppLoginToken() With {.Type = membership, .Token = String.Empty}
            If membership = MembershipTypes.Family Then
                Dim token = FamilyController.Login(UserName, Password, conn)
                ret.Token = token
            ElseIf membership = MembershipTypes.Student
                Dim token = StudentController.Login(UserName, Password, conn)
                ret.Token = token
            End If
            Return ret
        End Function

        Private Function GetProfile(ByVal conn As SqlConnection, ByVal langId As Integer) As AppMemberProfile
            Dim ret As New AppMemberProfile()
            If FamilyController.AccessTokenExists(AccessToken, conn) Then
                Dim obj As Family = FamilyController.GetByAccessToken(AccessToken, conn)
                With ret
                    .Email = obj.Email
                    .FullName = obj.FullName
                    .Type = MembershipTypes.Family
                End With
            ElseIf StudentController.AccessTokenExists(AccessToken, conn) Then
                Dim obj As Student = StudentController.GetByAccessToken(AccessToken, conn)
                With ret
                    .Email = obj.Email
                    .FullName = obj.FullName
                    .Type = MembershipTypes.Student
                End With
            Else
                Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.ProfileNotFound"))
            End If
            Return ret
        End Function

        Private Function ChangePassword(ByVal conn As SqlConnection, ByVal langId As Integer) As Boolean
            Dim ret As Boolean = True
            Dim obj As BusinessBase = Nothing
            If FamilyController.AccessTokenExists(AccessToken, conn) Then
                obj = FamilyController.GetByAccessToken(AccessToken, conn)
                Dim castedObj As Family = DirectCast(obj, Family)
                If castedObj.IsActive Then
                    If castedObj.Password = OldPassword Then
                        castedObj.Password = NewPassword
                    Else
                        Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.IncorrectOldPassword"))
                    End If
                Else
                    Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.AccountInactive"))
                End If
            ElseIf StudentController.AccessTokenExists(AccessToken, conn) Then
                obj = StudentController.GetByAccessToken(AccessToken, conn)
                Dim castedObj As Student = DirectCast(obj, Student)
                If castedObj.IsActive Then
                    If castedObj.Password = OldPassword Then
                        castedObj.Password = NewPassword
                    Else
                        Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.IncorrectOldPassword"))
                    End If
                Else
                    Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.AccountInactive"))
                End If
            End If
            Dim trans As SqlTransaction = conn.BeginTransaction()
            Try
                obj.Save(trans)
                trans.Commit()
            Catch ex As Exception
                trans.Rollback()
                ret = False
                Throw ex
            End Try
            Return ret
        End Function

        Private Function ListStudentsOfFamily(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of FamilyStudent)
            Dim lst As New List(Of FamilyStudent)
            Using dt As DataTable = FamilyController.GetStudentsCollection(conn, AccessToken, True).List
                For Each dr As DataRow In dt.Rows
                    lst.Add(New FamilyStudent() With {
                        .AccessToken = StudentController.GetAccessToken(Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger), conn),
                        .FullName = Helper.GetSafeDBValue(dr("FullName")),
                        .UserName = Helper.GetSafeDBValue(dr("SchoolId"))
                    })
                Next
            End Using
            Return lst
        End Function

        Private Function RegisterDevice(ByVal conn As SqlConnection, ByVal langId As Integer) As Boolean
            'access_token, device_token, langId, disable
            Dim completed As Boolean = True
            If AccessToken <> String.Empty AndAlso DeviceToken <> String.Empty Then
                Dim memberId As Integer = 0
                Dim memberType As Integer = 0
                If AccessToken <> "0000" Then
                    Dim obj As BusinessBase = Nothing
                    If FamilyController.AccessTokenExists(AccessToken, conn) Then
                        obj = FamilyController.GetByAccessToken(AccessToken, conn)
                        Dim castedObj As Family = DirectCast(obj, Family)
                        memberId = castedObj.Id
                        memberType = MembershipTypes.Family
                    ElseIf StudentController.AccessTokenExists(AccessToken, conn) Then
                        obj = StudentController.GetByAccessToken(AccessToken, conn)
                        Dim castedObj As Student = DirectCast(obj, Student)
                        memberId = castedObj.Id
                        memberType = MembershipTypes.Student
                    End If
                End If
                DeviceTokenController.CheckDeviceToken(memberId, memberType, DeviceToken, conn)
                completed = DeviceTokenController.RegisterToken(memberId, memberType, DeviceToken, langId, Disable, conn)
            Else
                completed = False
            End If
            Return completed
        End Function

        Private Function UnregisterDevice(ByVal conn As SqlConnection) As Boolean
            'access_token, device_token
            If FamilyController.AccessTokenExists(AccessToken, conn) Then
                Dim obj As Family = FamilyController.GetByAccessToken(AccessToken, conn)
                DeviceTokenController.UnregisterToken(DeviceToken, obj.Id, MembershipTypes.Family, conn)
                Return True
            ElseIf StudentController.AccessTokenExists(AccessToken, conn) Then
                Dim obj As Student = StudentController.GetByAccessToken(AccessToken, conn)
                DeviceTokenController.UnregisterToken(DeviceToken, obj.Id, MembershipTypes.Student, conn)
                Return True
            Else
                Throw New Exception("Account not found.")
            End If
        End Function

        Private Function UpdateDevice(ByVal conn As SqlConnection, ByVal langId As Integer) As Boolean
            'access_token, device_token, langId, disable
            Dim obj As BusinessBase = Nothing
            Dim memId As Integer = 0
            Dim memType As Integer = 0
            If FamilyController.AccessTokenExists(AccessToken, conn) Then
                obj = FamilyController.GetByAccessToken(AccessToken, conn)
                Dim castedObj As Family = DirectCast(obj, Family)
                memId = castedObj.Id
                memType = MembershipTypes.Family
            ElseIf StudentController.AccessTokenExists(AccessToken, conn) Then
                obj = StudentController.GetByAccessToken(AccessToken, conn)
                Dim castedObj As Student = DirectCast(obj, Student)
                memId = castedObj.Id
                memType = MembershipTypes.Student
            End If
            If memId > 0 AndAlso memType > 0 Then
                Return DeviceTokenController.UpdateToken(memId, memType, DeviceToken, langId, Disable, conn)
            Else
                Return False
            End If
        End Function

        Private Function GetPayments(ByVal conn As SqlConnection) As List(Of StudentAccountItem)
            If FamilyController.AccessTokenExists(AccessToken, conn) Then
                Dim obj As Family = FamilyController.GetByAccessToken(AccessToken, conn)
                Dim lst As New List(Of StudentAccountItem)
                For Each id As Integer In FamilyController.GetStudents(conn, obj.Id, True)
                    Dim acc As StudentAccount2 = StudentAccount2Controller.GetByStudentId(id, conn)
                    Dim objStudent As New Student(id, conn)
                    If acc IsNot Nothing AndAlso acc.Id > 0 Then
                        Dim objClass As New StudyClass(acc.ClassId, conn, LanguageId)
                        Dim item As New StudentAccountItem() With {
                            .StudentToken = StudentController.GetAccessToken(acc.StudentId, conn),
                            .StudentName = objStudent.FullName,
                            .PreviousClass = String.Empty,
                            .CurrentClass = objClass.Title,
                            .Deposit = 0D,
                            .Subscription = 0D,
                            .Total = acc.RequestedAmount,
                            .Discount = 0D,
                            .Transportation = False,
                            .NetTotal = acc.RequestedAmount,
                            .PaymentsSum = acc.RequestedAmount - acc.Balance,
                            .Balance = acc.Balance
                        }
                        item.Payments = New List(Of StudentPaymentItem)()
                        Dim payments As New List(Of Object)
                        Using dt = StudentAccount2Controller.GetCollection(conn, acc.StudentId).Tables(0)
                            For Each dr In dt.Rows
                                payments.Add(New With {
                                    .LastUpdate = CDate(Helper.GetSafeDBValue(dr("LastUpdate"), EGV.Enums.ValueTypes.TypeDateTime)).ToString("MMMM d, yyyy"),
                                    .RequestedAmount = CDec(Helper.GetSafeDBValue(dr("RequestedAmount"), EGV.Enums.ValueTypes.TypeDecimal)).ToString("N0"),
                                    .Balance = CDec(Helper.GetSafeDBValue(dr("Balance"), EGV.Enums.ValueTypes.TypeDecimal)).ToString("N0"),
                                    .PaidAmount = CDec(Helper.GetSafeDBValue(dr("PaidAmount"), EGV.Enums.ValueTypes.TypeDecimal)).ToString("N0"),
                                    .Payment = 0D,
                                    .FullName = Helper.GetSafeDBValue(dr("FullName")),
                                    .ClassId = Helper.GetSafeDBValue(dr("ClassId"), ValueTypes.TypeInteger)
                                })
                            Next
                        End Using
                        For i As Integer = 0 To payments.Count - 2
                            payments(i).Payment = payments(i).PaidAmount - IIf(payments(i + 1).ClassId = payments(i).ClassId, payments(i + 1).PaidAmount, 0)
                        Next
                        payments(payments.Count - 1).Payment = payments(payments.Count - 1).PaidAmount
                        Dim classId As Integer = payments(0).ClassId
                        payments = payments.Where(Function(x) x.ClassId = classId).ToList()
                        Dim j As Integer = payments.Count
                        For Each p In payments
                            item.Payments.Add(New StudentPaymentItem() With {
                                .PaymentAmount = p.Payment,
                                .PaymentDate = p.LastUpdate,
                                .PaymentNumber = j
                            })
                            j = j - 1
                        Next
                        lst.Add(item)
                    Else
                        Dim objSection As New Section(objStudent.SectionId, conn, LanguageId)
                        Dim objClass As New StudyClass(objSection.ClassId, conn, LanguageId)
                        Dim item As New StudentAccountItem() With {
                            .StudentToken = StudentController.GetAccessToken(objStudent.Id, conn),
                            .StudentName = objStudent.FullName,
                            .PreviousClass = String.Empty,
                            .CurrentClass = objClass.Title,
                            .Deposit = 0D,
                            .Subscription = 0D,
                            .Total = 0D,
                            .Discount = 0D,
                            .Transportation = False,
                            .NetTotal = 0D,
                            .PaymentsSum = 0D,
                            .Balance = 0D,
                            .Payments = New List(Of StudentPaymentItem)()
                        }
                        lst.Add(item)
                    End If
                Next
                Return lst
            Else
                Throw New Exception(Localization.GetResource("Resources.Global.MobileApp.ProfileNotFound"))
            End If
        End Function

#End Region

    End Class

End Namespace
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Business
Imports EGV.Utils
Imports System.Data.SqlClient

Namespace WebRequests
    Public Class NotificationRequest

        Public Shared Function SendMessageNotification(ByVal members As List(Of ParsedMessageUser),
                                                       ByVal msgTitle As String,
                                                       Optional ByVal conn As SqlConnection = Nothing) As Boolean
            Dim completed As Boolean = False
            If Helper.IsRemote() Then
                'Dim header As String = Localization.GetResource("Resources.Global.MobileApp.MessageNotification")
                Dim header As String = "You have received a new message"
                Dim headerAr As String = "لقد وصلتكم رسالة جديدة"
                Dim content As String = msgTitle
                Dim tokens As New List(Of String)
                Dim tokensAr As New List(Of String)
                For Each member As ParsedMessageUser In members
                    If member.UserRole <> MessageUserRoles.Sender AndAlso (member.UserType = MessageUserTypes.Student OrElse member.UserType = MessageUserTypes.Family) Then
                        tokens.AddRange(DeviceTokenController.GetMemberTokens(0, member.UserId, IIf(member.UserType = MessageUserTypes.Student, MembershipTypes.Student, MembershipTypes.Family), 1, True, conn))
                        tokensAr.AddRange(DeviceTokenController.GetMemberTokens(0, member.UserId, IIf(member.UserType = MessageUserTypes.Student, MembershipTypes.Student, MembershipTypes.Family), 2, True, conn))
                    End If
                Next
                If tokens.Count > 0 Then
                    APIRequest.ProcessRequest(header, content, False, NotificationTypes.Messaging, tokens.ToArray(), "POST")
                    completed = True
                End If
                If tokensAr.Count > 0 Then
                    APIRequest.ProcessRequest(headerAr, content, False, NotificationTypes.Messaging, tokensAr.ToArray(), "POST")
                    completed = completed And True
                End If
            End If
            Return completed
        End Function

        'Public Shared Function SendMessageNotification(ByVal memberId As Integer, ByVal memberType As MembershipTypes,
        '                                               ByVal msgTitle As String, Optional ByVal conn As SqlConnection = Nothing) As Boolean
        '    Dim completed As Boolean = False
        '    If Helper.IsRemote() Then
        '        Dim header As String = Localization.GetResource("Resources.Global.MobileApp.MessageNotification")
        '        Dim content As String = msgTitle
        '        Dim tokens = DeviceTokenController.GetMemberTokens(0, memberId, memberType, 0, True, conn)
        '        If tokens.Count > 0 Then
        '            APIRequest.ProcessRequest(header, content, False, NotificationTypes.Messaging, tokens.ToArray(), "POST")
        '            completed = True
        '        End If
        '    End If
        '    Return completed
        'End Function

        Public Shared Function SendNoteNotification(ByVal lst As List(Of ShortMemberObject),
                                                    ByVal note As String, ByVal isNegativeNote As Boolean,
                                                    Optional ByVal conn As SqlConnection = Nothing) As Boolean
            Dim completed As Boolean = False
            If Helper.IsRemote() Then
                'Dim header As String = Localization.GetResource("Resources.Global.MobileApp." & IIf(isNegativeNote, "NegativeNoteNotification", "PositiveNoteNotification"))
                Dim header As String = IIf(isNegativeNote, "You have received a new negative note", "You have received a new positive note")
                Dim headerAr As String = IIf(isNegativeNote, "لقد وصلتكم ملاحظة سلبية جديدة", "لقد وصلتكم ملاحظة إيجابية جديدة")
                Dim content As String = note
                Dim tokens As New List(Of String)
                Dim tokensAr As New List(Of String)
                For Each item As ShortMemberObject In lst
                    tokens.AddRange(DeviceTokenController.GetMemberTokens(0, item.MemberId, item.MemberType, 1, True, conn))
                    tokensAr.AddRange(DeviceTokenController.GetMemberTokens(0, item.MemberId, item.MemberType, 2, True, conn))
                Next
                If tokens.Count > 0 Then
                    APIRequest.ProcessRequest(header, content, isNegativeNote, NotificationTypes.Notes, tokens.ToArray(), "POST")
                    completed = True
                End If
                If tokensAr.Count > 0 Then
                    APIRequest.ProcessRequest(headerAr, content, isNegativeNote, NotificationTypes.Notes, tokensAr.ToArray(), "POST")
                    completed = completed And True
                End If
            End If
            Return completed
        End Function

        'Public Shared Function SendNoteNotification(ByVal memberId As Integer, ByVal memberType As MembershipTypes,
        '                                            ByVal note As String, ByVal isNegativeNote As Boolean,
        '                                            Optional ByVal conn As SqlConnection = Nothing) As Boolean
        '    Dim completed As Boolean = False
        '    If Helper.IsRemote() Then
        '        Dim header As String = Localization.GetResource("Resources.Global.MobileApp." & IIf(isNegativeNote, "NegativeNoteNotification", "PositiveNoteNotification"))
        '        Dim content As String = note
        '        Dim tokens = DeviceTokenController.GetMemberTokens(0, memberId, memberType, 0, True, conn)
        '        If tokens.Count > 0 Then
        '            APIRequest.ProcessRequest(header, content, isNegativeNote, NotificationTypes.Notes, tokens.ToArray(), "POST")
        '            completed = True
        '        End If
        '    End If
        '    Return completed
        'End Function

        Public Shared Function SendPaymentNotification(ByVal familyId As Integer, ByVal studentId As Integer,
                                                       ByVal paymentAmount As Decimal,
                                                       Optional ByVal conn As SqlConnection = Nothing) As Boolean
            Dim completed As Boolean = False
            If Helper.IsRemote() Then
                Dim studentAcc = StudentAccountController.GetByStudentId(studentId, conn)
                Dim header As String = "Payment Confirmation"
                Dim content As String = String.Format("We thank you for paying a payment of {0} for student {1}. Total payments {2}, current balance {3}.", paymentAmount.ToString("0,0.00"), StudentController.GetFullName(studentId, conn), studentAcc.PaymentsSum.ToString("0,0.00"), studentAcc.Balance.ToString("0,0.00"))
                Dim headerAr As String = "تأكيد استلام دفعة"
                Dim contentAr As String = String.Format("نشكر حضرتكم على تسديد دفعة قيمتها {0} للطالب {1}. مجموع الدفعات {2}, المبلغ المتبقي {3}.", paymentAmount.ToString("0,0.00"), StudentController.GetFullName(studentId, conn), studentAcc.PaymentsSum.ToString("0,0.00"), studentAcc.Balance.ToString("0,0.00"))
                Dim tokens = DeviceTokenController.GetMemberTokens(0, familyId, MembershipTypes.Family, 1, True, conn)
                Dim tokensAr = DeviceTokenController.GetMemberTokens(0, familyId, MembershipTypes.Family, 2, True, conn)
                If tokens.Count > 0 Then
                    APIRequest.ProcessRequest(header, content, False, NotificationTypes.Payment, tokens.ToArray(), "POST")
                    completed = True
                End If
                If tokensAr.Count > 0 Then
                    APIRequest.ProcessRequest(headerAr, contentAr, False, NotificationTypes.Payment, tokensAr.ToArray(), "POST")
                    completed = completed And True
                End If
            End If
            Return completed
        End Function

        Public Shared Function SendPayment2Notification(ByVal obj As StudentAccount2, Optional ByVal conn As SqlConnection = Nothing) As Boolean
            Dim completed As Boolean = False
            If Helper.IsRemote() Then
                Dim objStudent As New Student(obj.StudentId, conn)
                Dim header As String = "Your Account Balance at Little Village School"
                Dim content As String = String.Format("Thank you for the payment at Little Village School, Your balance is {0} of {1}", obj.Balance.ToString("N0"), obj.RequestedAmount.ToString("N0"))
                Dim headerAr As String = "رصيد حسابكم في مدرسة القرية الصغيرة"
                Dim contentAr As String = String.Format("نشكركم لتسديد مبلغ لدى مدرسة القرية الصغيرة, رصيدكم اللآن {0} من أصل {1}.", obj.Balance.ToString("N0"), obj.RequestedAmount.ToString("N0"))
                Dim tokens = DeviceTokenController.GetMemberTokens(0, objStudent.FamilyId, MembershipTypes.Family, 1, True, conn)
                Dim tokensAr = DeviceTokenController.GetMemberTokens(0, objStudent.FamilyId, MembershipTypes.Family, 2, True, conn)
                If tokens.Count > 0 Then
                    APIRequest.ProcessRequest(header, content, False, NotificationTypes.Payment, tokens.ToArray(), "POST")
                    completed = True
                End If
                If tokensAr.Count > 0 Then
                    APIRequest.ProcessRequest(headerAr, contentAr, False, NotificationTypes.Payment, tokensAr.ToArray(), "POST")
                    completed = completed And True
                End If
            End If
            Return completed
        End Function

    End Class
End Namespace
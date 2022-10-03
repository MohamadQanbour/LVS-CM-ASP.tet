Imports System.Net
Imports System.Net.Mail
Imports TemplateParser
Imports System.Data.SqlClient
Imports EGV.Business
Imports EGV.Enums
Imports System.Text

Namespace EGV
    Namespace Utils

        Public Class Mailer

            Public Shared Sub SendMail(ByVal conn As SqlConnection, ByVal mailTemplate As String, ByVal subject As String, ByVal params As Hashtable,
                                   ByVal senderTitle As String, ByVal replyToAddress As String, ByVal replyToName As String, ParamArray ByVal receivers() As Email)
                SendMail(conn, mailTemplate, subject, params, Nothing, senderTitle, replyToAddress, replyToName, receivers)
            End Sub

            Public Shared Sub SendMail(ByVal conn As SqlConnection, ByVal mailTemplate As String, ByVal subject As String, ByVal params As Hashtable,
                                       ByVal attachments As AttachmentCollection, ByVal senderTitle As String, ByVal replyToAddress As String, ByVal replyToName As String,
                                       ParamArray ByVal receivers() As Email)
                Dim msg As New MailMessage()
                msg.Subject = subject
                msg.BodyEncoding = Encoding.UTF8
                msg.IsBodyHtml = True

                If attachments IsNot Nothing AndAlso attachments.Count > 0 Then
                    For Each att As Attachment In attachments
                        msg.Attachments.Add(att)
                    Next
                End If

                Dim path As String = "~/" & Helper.MailTemplatePath() & "/" & mailTemplate & ".html"
                Dim parser As New Parser(Helper.Server.MapPath(path), params)
                msg.Body = parser.Parse()
                SendMail(conn, msg, senderTitle, replyToAddress, replyToName, receivers)
            End Sub

            Public Shared Sub SendMail(ByVal conn As SqlConnection, ByVal message As MailMessage, ByVal senderTitle As String, ByVal replyToAddress As String,
                                       ByVal replyToName As String, ParamArray ByVal receivers() As Email)
                Dim shouldClose As Boolean = False
                Try
                    If conn.State <> ConnectionState.Open Then
                        shouldClose = True
                        conn.Open()
                    End If

                    If senderTitle = String.Empty Then senderTitle = SettingController.ReadSetting("SITENAME", conn)
                    Dim senderEmail As String = SettingController.ReadSetting("SenderEmail", conn)
                    Dim senderPass As String = SettingController.ReadSetting("SenderPassword", conn)
                    Dim smtp As String = SettingController.ReadSetting("SMTP", conn)
                    Dim ssl As Boolean = SettingController.ReadSetting("SSL", conn)
                    Dim port As String = SettingController.ReadSetting("PORT", conn)

                    message.From = New MailAddress(senderEmail, senderTitle)
                    For Each rec As Email In receivers
                        Select Case rec.Type
                            Case EmailTypes.To
                                message.To.Add(New MailAddress(rec.EmailAddress, rec.DisplayName))
                            Case EmailTypes.CC
                                message.CC.Add(New MailAddress(rec.EmailAddress, rec.DisplayName))
                            Case EmailTypes.BCC
                                message.Bcc.Add(New MailAddress(rec.EmailAddress, rec.DisplayName))
                        End Select
                    Next
                    message.BodyEncoding = Encoding.UTF8
                    message.IsBodyHtml = True
                    If replyToAddress <> String.Empty Then message.ReplyToList.Add(New MailAddress(replyToAddress, IIf(replyToName <> String.Empty, replyToName, replyToAddress)))

                    Dim sender As New SmtpClient() With {
                        .DeliveryMethod = SmtpDeliveryMethod.Network,
                        .EnableSsl = ssl,
                        .Host = smtp,
                        .Port = port,
                        .UseDefaultCredentials = False
                    }
                    sender.Credentials = New NetworkCredential(senderEmail, senderPass)
                    sender.Send(message)

                Catch ex As Exception
                    Throw ex
                Finally
                    If shouldClose Then conn.Close()
                End Try
            End Sub

            Public Shared Sub SendMail(ByVal conn As SqlConnection, ByVal message As MailMessage, ByVal senderTitle As String, ByVal replyToAddress As String,
                                       ByVal replyToName As String, ByVal emailKey As Integer)
                Dim lst As New List(Of Email)
                lst = EmailController.List(emailKey, conn)
                SendMail(conn, message, senderTitle, replyToAddress, replyToName, lst.ToArray())
            End Sub

            Public Shared Sub SendMail(ByVal conn As SqlConnection, ByVal mailTemplate As String, ByVal subject As String, ByVal params As Hashtable,
                                       ByVal attachments As AttachmentCollection, ByVal senderTitle As String, ByVal replyToAddress As String, ByVal replyToName As String,
                                       ByVal emailKey As Integer)
                Dim lst As New List(Of Email)
                lst = EmailController.List(emailKey, conn)
                SendMail(conn, mailTemplate, subject, params, attachments, senderTitle, replyToAddress, replyToName, lst.ToArray())
            End Sub

            Public Shared Sub SendMail(ByVal conn As SqlConnection, ByVal mailTemplate As String, ByVal subject As String, ByVal params As Hashtable,
                                   ByVal senderTitle As String, ByVal replyToAddress As String, ByVal replyToName As String, ByVal emailKey As Integer)
                SendMail(conn, mailTemplate, subject, params, Nothing, senderTitle, replyToAddress, replyToName, emailKey)
            End Sub

        End Class

    End Namespace
End Namespace
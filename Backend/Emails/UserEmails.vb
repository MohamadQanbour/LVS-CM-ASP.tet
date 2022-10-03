Imports System.Data.SqlClient
Imports EGV
Imports EGV.Business
Imports System.Net
Imports System.Net.Mail
Imports EGV.Utils

Namespace EGV
    Namespace Emails

        Public Class UserEmails

#Region "Public Methods"

            Public Shared Sub SendRecoveryPassword(ByVal usr As User, Optional ByVal conn As SqlConnection = Nothing)
                Dim template As String = "User/RecoveryPassword"
                Dim p As Hashtable = GetRecoveryPasswordParamters(usr, conn)
                Mailer.SendMail(conn, template, "Account Recovery", p, SettingController.ReadSetting("SITENAME", conn), "", "", New Email() With {
                    .DisplayName = usr.Profile.FullName,
                    .EmailAddress = usr.Email,
                    .Type = Enums.EmailTypes.To
                })
            End Sub

#End Region

#Region "Private Methods"

            Private Shared Function GetRecoveryPasswordParamters(ByVal usr As User, Optional ByVal conn As SqlConnection = Nothing) As Hashtable
                Dim sitelink As String = SettingController.ReadSetting("SITELINK", conn)
                Dim sitename As String = SettingController.ReadSetting("SITENAME", conn)
                Dim p As New Hashtable()
                p.Add("USERNAME", usr.UserName)
                p.Add("DISPLAYNAME", usr.Profile.FullName)
                p.Add("PROFILEIMAGE", sitelink & usr.Profile.ProfileImageUrl)
                p.Add("LOGINLINK", sitelink & Path.MapCMSFile("Login.aspx"))
                p.Add("SITENAME", sitename)
                p.Add("RECOVERYPASSWORD", usr.RecoveryPassword)
                Return p
            End Function

#End Region

        End Class

    End Namespace
End Namespace
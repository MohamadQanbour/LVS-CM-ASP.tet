Imports System.Data
Imports System.Data.SqlClient
Imports System.Xml

Namespace EGV
    Namespace Business

        'object
        Public Class UserProfile
            Inherits PrimeBusinessBase

#Region "Public Properties"

            Public Property UserId As Integer
            Public Property FullName As String
            Public Property ProfileImageUrl As String
            Public Property SettingsXML As String
            'Profile XML Parsed
            Public Property UserLanguageId As Integer

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal id As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If id > 0 Then
                    FillObject(DBA.DataRow(MyConn, "SELECT * FROM SYS_UserProfile WHERE UserId = @Id", DBA.CreateParameter("@Id", SqlDbType.Int, id)))
                Else
                    UserLanguageId = LanguageController.GetDefaultId(MyConn)
                End If
            End Sub

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    UserId = Safe(dr("UserId"), Enums.ValueTypes.TypeInteger)
                    FullName = Safe(dr("FullName"), Enums.ValueTypes.TypeString)
                    ProfileImageUrl = Safe(dr("ProfileImageUrl"), Enums.ValueTypes.TypeString)
                    SettingsXML = Safe(dr("SettingsXML"))
                    If SettingsXML = String.Empty Then SettingsXML = "<Settings></Settings>"
                    Deserialize()
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Function GetProfileImage() As String
                If ProfileImageUrl <> String.Empty Then Return ProfileImageUrl Else Return Utils.Helper.FormImageUrl(Utils.Path.MapCMSAsset("avatar.png"), 160, 160, 160, 160, Enums.CroppingTypes.Center, 0, 0, "1", "cms")
            End Function

            Public Sub Serialize()
                Dim doc As New XmlDocument()
                Dim root As XmlElement = doc.CreateElement("Settings")
                doc.AppendChild(root)
                Dim dashboard As XmlElement = doc.CreateElement("Dashboard")
                root.AppendChild(dashboard)
                Dim pref As XmlElement = doc.CreateElement("Preferences")
                root.AppendChild(pref)
                Dim lang As XmlElement = doc.CreateElement("LanguageId")
                lang.InnerText = UserLanguageId
                pref.AppendChild(lang)
                SettingsXML = doc.OuterXml
            End Sub

            Public Sub Deserialize()
                Dim doc As New XmlDocument()
                Dim loaded As Boolean = True
                Try
                    doc.LoadXml(SettingsXML)
                Catch
                    loaded = False
                End Try
                If loaded Then
                    Dim root = doc.SelectSingleNode("Settings")
                    Dim dashboard = root.SelectSingleNode("Dashboard")
                    If dashboard IsNot Nothing Then

                    End If
                    Dim pref = root.SelectSingleNode("Preferences")
                    If pref IsNot Nothing Then
                        Dim lang = pref.SelectSingleNode("LanguageId")
                        If lang IsNot Nothing Then UserLanguageId = lang.InnerText
                    End If
                End If
            End Sub

#End Region

        End Class

        'controller
        Public Class UserProfileController

            Public Shared Sub Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "DELETE FROM SYS_UserProfile WHERE UserId = @Id"
                If trans IsNot Nothing Then
                    DBA.NonQuery(trans, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                Else
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                End If
            End Sub

            Public Shared Sub AddProfile(ByVal profile As UserProfile, Optional ByVal trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_UserProfile_Add"
                profile.Serialize()
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("UserId", SqlDbType.Int, profile.UserId),
                               DBA.CreateParameter("FullName", SqlDbType.NVarChar, profile.FullName, 50),
                               DBA.CreateParameter("ProfileImageUrl", SqlDbType.NVarChar, profile.ProfileImageUrl, 255),
                               DBA.CreateParameter("SettingXML", SqlDbType.NText, profile.SettingsXML)
                               )
            End Sub

        End Class

    End Namespace
End Namespace
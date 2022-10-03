Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Business
Imports EGV
Imports System.Data

Partial Class cms_Default
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Public Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ddlTo.Attributes.Add("data-userid", AuthUser.Id)
        ddlTo.Attributes.Add("data-usertype", MessageUserTypes.User)
        EGVScriptManager.AddScript(Path.MapCMSScript("local/dashboard"), False, "2.1")
        Try
            MyConn.Open()
            bxTools.Visible = AuthUser.IsSuperAdmin Or UserController.UserInRole(MyConn, AuthUser.Id, 1)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Public Sub lnkSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSend.Click
        If Page.IsValid Then
            Dim shouldRedirect As Boolean = True
            Try
                MyConn.Open()
                Dim lst As List(Of ReceiverType) = Helper.JSDeserialize(Of List(Of ReceiverType))(hdnSelectedTo.Value)
                Dim subject As String = txtSubject.Text
                Dim html As String = txtBody.Text
                Dim msg As New Message()
                msg.Title = subject
                msg.MessageContent = HttpUtility.HtmlDecode(html)
                msg.Users.Add(New ParsedMessageUser() With {
                    .FullName = AuthUser.Profile.FullName,
                    .UserId = AuthUser.Id,
                    .UserRole = MessageUserRoles.Sender,
                    .UserType = MessageUserTypes.User
                })
                For Each usr As ReceiverType In lst
                    Select Case usr.type
                        Case MessageUserTypes.Family
                            Dim obj As New Family(usr.id, MyConn)
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = obj.FullName,
                                .UserId = obj.Id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.Family
                            })
                        Case MessageUserTypes.Student
                            Dim obj As New Student(usr.id, MyConn)
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = obj.FullName,
                                .UserId = obj.Id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.Student
                            })
                        Case MessageUserTypes.User
                            Dim obj As New User(usr.id, MyConn)
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = obj.Profile.FullName,
                                .UserId = obj.Id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.User
                            })
                        Case MessageUserTypes.AllClass
                            Dim className As String = StudyClassController.GetTitle(MyConn, usr.id, LanguageId)
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = className,
                                .UserId = usr.id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.AllClass
                            })
                        Case MessageUserTypes.AllSection
                            Dim sectionName As String = SectionController.GetTitle(MyConn, usr.id, LanguageId)
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = sectionName,
                                .UserId = usr.id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.AllSection
                            })
                        Case MessageUserTypes.AllRoleUsers
                            Dim roleName As String = New Role(usr.id, MyConn).Title
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = roleName,
                                .UserId = usr.id,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.AllRoleUsers
                            })
                        Case MessageUserTypes.AllUsers
                            msg.Users.Add(New ParsedMessageUser() With {
                                .FullName = "All Users | جميع المستخدمين",
                                .UserId = 0,
                                .UserRole = MessageUserRoles.RecTo,
                                .UserType = MessageUserTypes.AllUsers
                            })
                    End Select
                Next
                MessageController.CreateMessage(msg, MyConn)
            Catch ex As Exception
                shouldRedirect = False
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
            If shouldRedirect Then Response.Redirect("default.aspx")
        End If
    End Sub

    Protected Sub validate_To(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Dim lst As List(Of ReceiverType) = Helper.JSDeserialize(Of List(Of ReceiverType))(hdnSelectedTo.Value)
        e.IsValid = lst.Count > 0
    End Sub

#End Region

#Region "Private Methods"

    Private Function GetFullName(ByVal usr As ReceiverType) As String
        Dim name As String = String.Empty
        Select Case usr.type
            Case MessageUserTypes.Family
                name = FamilyController.ReadField(usr.id, "FullName", MyConn)
            Case MessageUserTypes.Student
                name = StudentController.ReadField(usr.id, "FullName", MyConn)
            Case MessageUserTypes.User
                name = UserController.GetFullName(usr.id, MyConn)
        End Select
        Return name
    End Function

#End Region

End Class

Imports System.Data.SqlClient
Imports EGV.Utils
Imports System.Text
Imports EGV.Structures
Imports EGV.Enums

Namespace EGV
    Namespace Business

        'Object
        Public Class User
            Inherits BusinessBase

#Region "Public Properties"

            Public Property Id As Integer
            Public Property UserName As String
            Public Property Password As String
            Public Property Email As String
            Public Property IsSuperAdmin As Boolean = False
            Public Property IsActive As Boolean = True
            Public Property RecoveryPassword As String = String.Empty
            Public Property AllowList As Boolean

            Public Property Profile As UserProfile
            Public Property Roles As List(Of Integer)

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tId As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                Profile = New UserProfile()
                Roles = New List(Of Integer)()
                If tId > 0 Then
                    FillObject(DBA.SPDataRow(MyConn, "SYS_User_Get", DBA.CreateParameter("@Id", SqlDbType.Int, tId)))
                End If
            End Sub

#End Region

#Region "Fill"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), Enums.ValueTypes.TypeInteger)
                    UserName = Safe(dr("UserName"), Enums.ValueTypes.TypeString)
                    Password = Safe(dr("Password"), Enums.ValueTypes.TypeString)
                    Email = Safe(dr("Email"), Enums.ValueTypes.TypeString)
                    IsSuperAdmin = Safe(dr("IsSuper"), Enums.ValueTypes.TypeBoolean)
                    IsActive = Safe(dr("IsActive"), Enums.ValueTypes.TypeBoolean)
                    RecoveryPassword = Safe(dr("RecoveryPassword"), Enums.ValueTypes.TypeString)
                    AllowList = Safe(dr("AllowList"), ValueTypes.TypeBoolean)
                    Profile = New UserProfile(Id, MyConn)
                    Roles = UserController.GetRoles(Id, MyConn)
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                UserController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_User_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("UserName", SqlDbType.NVarChar, UserName, 50),
                                  DBA.CreateParameter("Password", SqlDbType.NVarChar, Password, 24),
                                  DBA.CreateParameter("Email", SqlDbType.NVarChar, Email, 255),
                                  DBA.CreateParameter("IsSuper", SqlDbType.Bit, IsSuperAdmin),
                                  DBA.CreateParameter("IsActive", SqlDbType.Bit, IsActive),
                                  DBA.CreateParameter("AllowList", SqlDbType.Bit, AllowList)
                                  )
                Profile.UserId = Id
                UserProfileController.AddProfile(Profile, trans)
                UserController.AddRoles(Id, Roles, trans)
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_User_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("UserName", SqlDbType.NVarChar, UserName, 50),
                               DBA.CreateParameter("Password", SqlDbType.NVarChar, Password, 24),
                               DBA.CreateParameter("Email", SqlDbType.NVarChar, Email, 255),
                               DBA.CreateParameter("IsSuper", SqlDbType.Bit, IsSuperAdmin),
                               DBA.CreateParameter("IsActive", SqlDbType.Bit, IsActive),
                               DBA.CreateParameter("AllowList", SqlDbType.Bit, AllowList),
                               DBA.CreateParameter("RecoveryPassword", SqlDbType.NVarChar, RecoveryPassword, 12),
                               DBA.CreateParameter("UserId", SqlDbType.Int, Id)
                               )
                UserProfileController.Delete(Id, Nothing, trans)
                Profile.UserId = Id
                UserProfileController.AddProfile(Profile, trans)
                UserController.DeleteRoles(Id, Nothing, trans)
                UserController.AddRoles(Id, Roles, trans)
            End Sub

#End Region

#Region "Permissions"

            Public Function CanRead(ByVal permissionId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If permissionId = 0 Then
                    Return True
                Else
                    Dim sp As String = "SYS_User_CanRead"
                    Return DBA.SPScalar(conn, sp,
                                            DBA.CreateParameter("UserId", SqlDbType.Int, Id),
                                            DBA.CreateParameter("PermissionId", SqlDbType.Int, permissionId)
                                        )
                End If
            End Function

            Public Function CanWrite(ByVal permissionId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If permissionId = 0 Then
                    Return True
                Else
                    Dim sp As String = "SYS_User_CanWrite"
                    Return DBA.SPScalar(conn, sp,
                                            DBA.CreateParameter("UserId", SqlDbType.Int, Id),
                                            DBA.CreateParameter("PermissionId", SqlDbType.Int, permissionId)
                                        )
                End If
            End Function

            Public Function CanModify(ByVal permissionId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If permissionId = 0 Then
                    Return True
                Else
                    Dim sp As String = "SYS_User_CanModify"
                    Return DBA.SPScalar(conn, sp,
                                            DBA.CreateParameter("UserId", SqlDbType.Int, Id),
                                            DBA.CreateParameter("PermissionId", SqlDbType.Int, permissionId)
                                        )
                End If
            End Function

            Public Function CanPublish(ByVal permissionId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If permissionId = 0 Then
                    Return True
                Else
                    Dim sp As String = "SYS_User_CanPublish"
                    Return DBA.SPScalar(conn, sp,
                                            DBA.CreateParameter("UserId", SqlDbType.Int, Id),
                                            DBA.CreateParameter("PermissionId", SqlDbType.Int, permissionId)
                                        )
                End If
            End Function

            Public Function CanDelete(ByVal permissionId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If permissionId = 0 Then
                    Return True
                Else
                    Dim sp As String = "SYS_User_CanDelete"
                    Return DBA.SPScalar(conn, sp,
                                            DBA.CreateParameter("UserId", SqlDbType.Int, Id),
                                            DBA.CreateParameter("PermissionId", SqlDbType.Int, permissionId)
                                        )
                End If
            End Function

#End Region

        End Class

        'Controller
        Public Class UserController

#Region "Public Methods"

            Public Shared Sub ToggleState(ByVal id As Integer, Optional ByVal activate As Boolean = True, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE SYS_User SET IsActive = @Activate WHERE Id = @Id"
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("Activate", SqlDbType.Bit, activate),
                             DBA.CreateParameter("Id", SqlDbType.Int, id)
                             )
            End Sub

            Public Shared Function UserNameExists(ByVal username As String, Optional ByVal id As Integer = 0, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim sp As String = "SYS_User_UserNameExists"
                Return DBA.SPScalar(conn, sp,
                                    DBA.CreateParameter("UserName", SqlDbType.NVarChar, username, 50),
                                    DBA.CreateParameter("Id", SqlDbType.Int, id)
                                    ) > 0
            End Function

            Public Shared Function EmailExists(ByVal email As String, Optional ByVal id As Integer = 0, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim sp As String = "SYS_User_EmailExists"
                Return DBA.SPScalar(conn, sp,
                                    DBA.CreateParameter("@Email", SqlDbType.NVarChar, email, 255),
                                    DBA.CreateParameter("Id", SqlDbType.Int, id)
                                    ) > 0
            End Function

            Public Shared Function GetByUsername(ByVal username As String, Optional ByVal conn As SqlConnection = Nothing) As User
                If UserNameExists(username, 0, conn) Then
                    Return New User(DBA.Scalar(conn, "SELECT Id FROM SYS_User WHERE UserName = @UserName", DBA.CreateParameter("@UserName", SqlDbType.NVarChar, username, 50)), conn)
                Else
                    Return Nothing
                End If
            End Function

            Public Shared Function GetByEmail(ByVal email As String, Optional ByVal conn As SqlConnection = Nothing) As User
                If EmailExists(email, 0, conn) Then
                    Return New User(DBA.Scalar(conn, "SELECT Id FROM SYS_User WHERE Email = @Email", DBA.CreateParameter("Email", SqlDbType.NVarChar, email, 255)), conn)
                Else
                    Return Nothing
                End If
            End Function

            Public Shared Function Login(ByVal username As String, ByVal password As String, Optional ByVal conn As SqlConnection = Nothing) As User
                Dim obj As User = GetByUsername(username, conn)
                If obj IsNot Nothing Then
                    If obj.IsActive Then
                        If obj.Password = password Then
                            Return obj
                        ElseIf obj.RecoveryPassword = password
                            Return obj
                        Else
                            Throw New Exception(Localization.GetResource("Resources.Global.CMS.UserIncorrectPassword"))
                        End If
                    Else
                        Throw New Exception(Localization.GetResource("Resources.Global.CMS.UserInactive"))
                    End If
                Else
                    Throw New Exception(Localization.GetResource("Resources.Global.CMS.UserNotExist"))
                End If
            End Function

            Public Shared Function GetRoles(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As List(Of Integer)
                Dim q As String = "SELECT RoleId FROM SYS_UserRole WHERE UserId = @Id"
                Dim lst As New List(Of Integer)
                Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(dr("RoleId"))
                    Next
                End Using
                Return lst
            End Function

            Public Shared Sub DeleteRoles(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "DELETE FROM SYS_UserRole WHERE UserId = @Id"
                If trans IsNot Nothing Then
                    DBA.NonQuery(trans, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                Else
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                End If
            End Sub

            Public Shared Sub AddRoles(ByVal userId As Integer, ByVal lst As List(Of Integer), Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "INSERT INTO SYS_UserRole (UserId, RoleId) VALUES (@Id, @Role)"
                For Each role As Integer In lst
                    DBA.NonQuery(trans, q,
                                 DBA.CreateParameter("Id", SqlDbType.Int, userId),
                                 DBA.CreateParameter("Role", SqlDbType.Int, role)
                                 )
                Next
            End Sub

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM SYS_User WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function CreateRecoveryPassword(ByVal usernameEmail As String, Optional ByVal conn As SqlConnection = Nothing) As User
                Dim recParss As String = Helper.GeneratePassword()
                Dim obj As User = GetByUsername(usernameEmail, conn)
                If obj Is Nothing Then
                    obj = GetByUsername(usernameEmail, conn)
                End If
                If obj Is Nothing Then
                    Throw New Exception(Localization.GetResource("Resources.Global.CMS.UserNotExist"))
                Else
                    Dim trans As SqlTransaction = conn.BeginTransaction()
                    Try
                        obj.RecoveryPassword = recParss
                        obj.Save(trans)
                        trans.Commit()
                    Catch ex As Exception
                        trans.Rollback()
                        Throw ex
                    End Try
                End If
                Return obj
            End Function

            Public Shared Function GetFullName(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim q As String = "SELECT FullName FROM SYS_UserProfile WHERE UserId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Public Shared Function GetImage(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim q As String = "SELECT ProfileImageUrl FROM SYS_UserProfile WHERE UserId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Public Shared Function GetCollection(ByVal conn As SqlConnection, Optional ByVal limit As Integer = 0,
                                                 Optional ByVal search As String = "", Optional ByVal onlyActive As Boolean = False,
                                                 Optional ByVal roles() As Integer = Nothing,
                                                 Optional ByVal onlyListable As Boolean = False) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim cq As New CustomQuery("SYS_User", "U", conn)
                If limit > 0 Then
                    cq.PageSize = limit
                    cq.PageIndex = 0
                    cq.EnablePaging = True
                Else
                    cq.EnablePaging = False
                End If
                If onlyActive Then
                    cq.AddCondition("U.IsActive = @Active")
                    cq.AddParameter(DBA.CreateParameter("Active", SqlDbType.Bit, True))
                End If
                If search <> String.Empty Then
                    cq.AddCondition("(U.UserName LIKE '%' + @Query + '%' OR U.Email LIKE '%' + @Query + '%' OR P.FullName LIKE '%' + @Query + '%')")
                    cq.AddParameter(DBA.CreateParameter("Query", SqlDbType.NVarChar, search, 255))
                End If
                If roles IsNot Nothing AndAlso roles.Length > 0 Then
                    Dim lst As New List(Of String)
                    For Each roleId As Integer In roles
                        lst.Add("RoleId = " & roleId)
                    Next
                    cq.AddCondition("U.Id IN (SELECT UserId FROM SYS_UserRole WHERE " & String.Join(" OR ", lst.ToArray()) & ")")
                End If
                If onlyListable Then
                    cq.AddCondition("U.AllowList = @List")
                    cq.AddParameter(DBA.CreateParameter("List", SqlDbType.Bit, True))
                End If
                cq.AddJoinTable("SYS_UserProfile", "P", "UserId", "Id", Enums.TableJoinTypes.Left)
                ret.Query = cq.GetQuery()
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

            Public Shared Function GetUsersOfType(ByVal conn As SqlConnection, ByVal type As String) As DBAReturnObject
                Dim roleId As Integer = UserTypeRoleController.GetRoleOfType(conn, type)
                Return GetCollection(conn, 0, "", True, {roleId})
            End Function

            Public Shared Function UserInRole(ByVal conn As SqlConnection, ByVal userId As Integer,
                                              ByVal roleId As Integer) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM SYS_UserRole WHERE UserId = @UserId AND RoleId = @RoleId"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("UserId", SqlDbType.Int, userId), DBA.CreateParameter("RoleId", SqlDbType.Int, roleId)) > 0
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not (HasMessages(id, conn) OrElse HasTranslations(id, conn) OrElse HasAudit(id, conn))
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                UserProfileController.Delete(id, conn)
                DeleteRoles(id, conn)
                Dim q As String = "DELETE FROM SYS_UserGrid WHERE UserId = @Id"
                Dim p = DBA.CreateParameter("Id", SqlDbType.Int, id)
                DBA.NonQuery(conn, q, p)
            End Sub

            Private Shared Function HasMessages(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM COM_MessageUser WHERE UserId = @Id AND UserType = @UserType"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id), DBA.CreateParameter("UserType", SqlDbType.Int, MessageUserTypes.User))
            End Function

            Private Shared Function HasTranslations(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim sp As String = "SYS_User_HasTranslation"
                Return DBA.SPScalar(conn, sp, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Private Shared Function HasAudit(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim sp As String = "SYS_User_HasAudit"
                Return DBA.SPScalar(conn, sp, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Private Shared Function HasRelations(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim sp As String = "SYS_User_HasRelations"
                Return DBA.SPScalar(conn, sp, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
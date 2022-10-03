Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures

Namespace EGV
    Namespace Business

        'object
        Public Class Family
            Inherits BusinessBase

#Region "Public Members"

            Public Property Id As Integer = 0
            Public Property SchoolId As String = String.Empty
            Public Property Password As String = String.Empty
            Public Property Email As String = String.Empty
            Public Property FullName As String = String.Empty
            Public Property IsActive As Boolean = False
            Public Property LastLoginDate As DateTime

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    SchoolId = Safe(dr("SchoolId"), ValueTypes.TypeString)
                    Password = Safe(dr("Password"), ValueTypes.TypeString)
                    Email = Safe(dr("Email"), ValueTypes.TypeString)
                    FullName = Safe(dr("FullName"), ValueTypes.TypeString)
                    IsActive = Safe(dr("IsActive"), ValueTypes.TypeBoolean)
                    LastLoginDate = Safe(dr("LastLoginDate"), ValueTypes.TypeDateTime)
                End If
            End Sub

#End Region

#Region "Public Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    FillObject(DBA.DataRow(MyConn, "SELECT * FROM MEM_Family WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                FamilyController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MEM_Family_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, SchoolId, 50),
                                  DBA.CreateParameter("Password", SqlDbType.NVarChar, Password, 24),
                                  DBA.CreateParameter("Email", SqlDbType.NVarChar, Email, 255),
                                  DBA.CreateParameter("FullName", SqlDbType.NVarChar, FullName, 50),
                                  DBA.CreateParameter("IsActive", SqlDbType.Bit, IsActive)
                                  )
                FamilyController.InsertAccessToken(Id, trans)
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MEM_Family_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, SchoolId, 50),
                               DBA.CreateParameter("Password", SqlDbType.NVarChar, Password, 24),
                               DBA.CreateParameter("Email", SqlDbType.NVarChar, Email, 255),
                               DBA.CreateParameter("FullName", SqlDbType.NVarChar, FullName, 50),
                               DBA.CreateParameter("IsActive", SqlDbType.Bit, IsActive),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

#End Region

        End Class

        'Controller
        Public Class FamilyController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    DeleteRelated(id, conn)
                    Dim q As String = "DELETE FROM MEM_Family WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Sub ToggleState(ByVal id As Integer, Optional ByVal activate As Boolean = True, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE MEM_Family SET IsActive = @Activate WHERE Id = @Id"
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("Activate", SqlDbType.Bit, activate),
                             DBA.CreateParameter("Id", SqlDbType.Int, id)
                             )
            End Sub

            Public Shared Function UsernameExists(ByVal username As String, Optional ByVal id As Integer = 0, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_Family WHERE SchoolId = @SchoolId"
                If id > 0 Then q &= " AND Id <> @Id"
                Return DBA.Scalar(conn, q,
                                  DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, username, 50),
                                  DBA.CreateParameter("Id", SqlDbType.Int, id)
                                  ) > 0
            End Function

            Public Shared Function EmailExists(ByVal email As String, Optional ByVal id As Integer = 0, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_Family WHERE Email = @Email"
                If id > 0 Then q &= " AND Id <> @Id"
                Return DBA.Scalar(conn, q,
                                  DBA.CreateParameter("Email", SqlDbType.NVarChar, email, 255),
                                  DBA.CreateParameter("Id", SqlDbType.Int, id)
                                  )
            End Function

            Public Shared Function GetByUsername(ByVal username As String, Optional ByVal conn As SqlConnection = Nothing) As Family
                If UsernameExists(username, 0, conn) Then
                    Return New Family(DBA.Scalar(conn, "SELECT Id FROM MEM_Family WHERE SchoolId = @Username", DBA.CreateParameter("Username", SqlDbType.NVarChar, username, 50)), conn)
                Else
                    Return Nothing
                End If
            End Function

            Public Shared Function Login(ByVal username As String, ByVal password As String, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim obj As Family = GetByUsername(username, conn)
                If obj IsNot Nothing Then
                    If Helper.VerifyEncrypted(obj.Password, password) Then
                        If obj.IsActive Then
                            Dim q As String = "UPDATE MEM_Family SET LastLoginDate = GETDATE()"
                            DBA.NonQuery(conn, q)
                            Return GetAccessToken(obj.Id, conn)
                        Else
                            'find other accounts
                            Dim q As String = "SELECT COUNT(*) FROM MEM_Family WITH (NOLOCK) WHERE SchoolId = @SchoolId AND IsActive = 1"
                            If Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, obj.SchoolId, 50)), ValueTypes.TypeInteger) > 0 Then
                                Dim id As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, "SELECT Id FROM MEM_Family WITH (NOLOCK) WHERE SchoolId = @SchoolId AND IsActive = 1", DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, obj.SchoolId, 50)), ValueTypes.TypeInteger)
                                Return GetAccessToken(id, conn)
                            Else
                                Dim schoolId As Integer = Helper.GetSafeObject(obj.SchoolId.Substring(5), ValueTypes.TypeInteger)
                                q = "SELECT FamilyId FROM MEM_Student WHERE SchoolId = @SchoolId AND IsActive = 1"
                                Dim id As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("SchoolId", SqlDbType.Int, schoolId)), ValueTypes.TypeInteger)
                                If id > 0 Then
                                    Return GetAccessToken(id, conn)
                                Else
                                    Throw New Exception(Localization.GetResource("Resources.Global.CMS.FamilyNoActive"))
                                End If
                            End If
                        End If
                    Else
                        Throw New Exception(Localization.GetResource("Resources.Global.CMS.UserIncorrectPassword"))
                    End If
                Else
                    Throw New Exception(Localization.GetResource("Resources.Global.CMS.FamilyNotExists"))
                End If
            End Function

            Public Shared Function GetAccessToken(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim q As String = "SELECT AccessToken FROM MEM_Family WHERE Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Public Shared Function AccessTokenExists(ByVal token As String, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_Family WHERE AccessToken = @Token"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 100)), ValueTypes.TypeInteger) > 0
            End Function

            Public Shared Function GetByAccessToken(ByVal token As String, Optional ByVal conn As SqlConnection = Nothing) As Family
                If AccessTokenExists(token, conn) Then
                    Dim q As String = "SELECT Id FROM MEM_Family WHERE AccessToken = @Token"
                    Dim id As Integer = DBA.Scalar(conn, q, DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 100))
                    Return New Family(id, conn)
                Else
                    Return Nothing
                End If
            End Function

            Public Shared Sub InsertAccessToken(ByVal id As Integer, Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "UPDATE MEM_Family SET AccessToken = @AccessToken WHERE Id = @Id"
                DBA.NonQuery(trans, q, DBA.CreateParameter("AccessToken", SqlDbType.NVarChar, Helper.Encrypt("family" & id), 100), DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

            Public Shared Function GetCollection(ByVal conn As SqlConnection, Optional ByVal limit As Integer = 0,
                                                 Optional ByVal search As String = "", Optional ByVal onlyActive As Boolean = True) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim cq As New CustomQuery("MEM_Family", "F", conn)
                If limit > 0 Then
                    cq.PageSize = limit
                    cq.PageIndex = 0
                    cq.EnablePaging = True
                Else
                    cq.EnablePaging = False
                End If
                If onlyActive Then
                    cq.AddCondition("F.IsActive = @Active")
                    cq.AddParameter(DBA.CreateParameter("Active", SqlDbType.Bit, True))
                End If
                If search <> String.Empty Then
                    cq.AddCondition("(F.SchoolId LIKE '%' + @Query + '%' OR F.Email LIKE '%' + @Query + '%' OR F.FullName LIKE '%' + @Query + '%')")
                    cq.AddParameter(DBA.CreateParameter("Query", SqlDbType.NVarChar, search, 255))
                End If
                ret.Query = cq.GetQuery()
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

            Public Shared Function GetStudentsCollection(ByVal conn As SqlConnection, ByVal familyToken As String,
                                                         Optional ByVal onlyActive As Boolean = True) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim q As String = "SELECT S.* FROM MEM_Student S INNER JOIN MEM_Family F ON S.FamilyId = F.Id WHERE F.AccessToken = @Token"
                If onlyActive Then q &= " AND S.IsActive = @Active"
                Dim dt As DataTable = DBA.DataTable(conn, q,
                                                    DBA.CreateParameter("Token", SqlDbType.NVarChar, familyToken, 100),
                                                    DBA.CreateParameter("Active", SqlDbType.Bit, True)
                                                    )
                ret.Query = q
                ret.Count = dt.Rows.Count
                ret.List = dt
                Return ret
            End Function

            Public Shared Function GetStudents(ByVal conn As SqlConnection, ByVal familyId As Integer,
                                               Optional ByVal onlyActive As Boolean = True) As List(Of Integer)
                Dim lst As New List(Of Integer)
                Dim q As String = "SELECT S.Id FROM MEM_Student S WHERE S.FamilyId = @FamilyId"
                If onlyActive Then q &= " AND S.IsActive = @Active"
                Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("FamilyId", SqlDbType.Int, familyId), DBA.CreateParameter("Active", SqlDbType.Bit, True))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger))
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function ReadField(ByVal id As Integer, ByVal field As String, Optional ByVal conn As SqlConnection = Nothing) As Object
                Dim q As String = "SELECT " & field & " FROM MEM_Family WHERE Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Public Shared Function GetFullName(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim ret As String = ReadField(id, "FullName", conn)
                If ret = String.Empty Then ret = ReadField(id, "Email", conn)
                If ret = String.Empty Then ret = ReadField(id, "SchoolId", conn)
                Return ret
            End Function

            Public Shared Function GetSchoolId(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim ret As String = ReadField(id, "SchoolId", conn)
                If ret = String.Empty Then ret = "UNKNOWN"
                Return ret
            End Function

            Public Shared Function GetIdByName(ByVal name As String, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Dim q As String = "SELECT Id FROM MEM_Family WHERE FullName = @Name AND IsActive = @Active"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Name", SqlDbType.NVarChar, name, 50), DBA.CreateParameter("Active", SqlDbType.Bit, True)), ValueTypes.TypeInteger)
            End Function

            Public Shared Sub DeactivateEmptyChildren(ByVal conn As SqlConnection)
                Dim q As String = "UPDATE MEM_Family SET IsActive = @Inactive WHERE Id NOT IN (SELECT FamilyId FROM MEM_Student)"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Inactive", SqlDbType.Bit, False))
            End Sub

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not (HasStudents(id, conn) OrElse HasMessages(id, conn))
            End Function

            Private Shared Function HasStudents(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_Student WHERE FamilyId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id)) > 0
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)

            End Sub

            Private Shared Function HasMessages(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM COM_MessageUser WHERE UserId = @Id AND UserType = @UserType"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id), DBA.CreateParameter("UserType", SqlDbType.Int, MessageUserTypes.Family))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
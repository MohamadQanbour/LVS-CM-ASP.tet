Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Constants

Namespace EGV
    Namespace Business

        'object
        Public Class Student
            Inherits AudBusinessBase

#Region "Public Methods"

            Public Property Id As Integer = 0
            Public Property SchoolId As String = String.Empty
            Public Property FullName As String = String.Empty
            Public Property FatherName As String = String.Empty
            Public Property MotherName As String = String.Empty
            Public Property AreaId As Integer = 0
            Public Property Password As String = String.Empty
            Public Property FamilyId As Integer = 0
            Public Property SectionId As Integer = 0
            Public Property IsActive As Boolean = False
            Public Property Email As String = String.Empty
            Public Property RecordNumber As String = String.Empty
            Public Property Religion As String = String.Empty
            Public Property Gender As String = String.Empty
            Public Property BirthInfo As String = String.Empty
            Public Property PhoneNumber As String = String.Empty
            Public Property FatherPhoneNumber As String = String.Empty
            Public Property MotherPhoneNumber As String = String.Empty
            Public Property LandlinePhoneNumber As String = String.Empty
            Public Property LastLoginDate As DateTime
            Public Property FatherWork As String = String.Empty
            Public Property MotherWork As String = String.Empty

#End Region

#Region "Filler"

            Public Overrides Sub FillObject(dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    SchoolId = Safe(dr("SchoolId"), ValueTypes.TypeString)
                    FullName = Safe(dr("FullName"), ValueTypes.TypeString)
                    FatherName = Safe(dr("FatherName"), ValueTypes.TypeString)
                    MotherName = Safe(dr("MotherName"), ValueTypes.TypeString)
                    AreaId = Safe(dr("AreaId"), ValueTypes.TypeInteger)
                    Password = Safe(dr("Password"), ValueTypes.TypeString)
                    FamilyId = Safe(dr("FamilyId"), ValueTypes.TypeInteger)
                    SectionId = Safe(dr("SectionId"), ValueTypes.TypeInteger)
                    IsActive = Safe(dr("IsActive"), ValueTypes.TypeBoolean)
                    Email = Safe(dr("Email"), ValueTypes.TypeString)
                    RecordNumber = Safe(dr("RecordNumber"))
                    Religion = Safe(dr("Religion"))
                    Gender = Safe(dr("Gender"))
                    BirthInfo = Safe(dr("BirthInfo"))
                    PhoneNumber = Safe(dr("PhoneNumber"))
                    FatherPhoneNumber = Safe(dr("FatherPhoneNumber"))
                    MotherPhoneNumber = Safe(dr("MotherPhoneNumber"))
                    LandlinePhoneNumber = Safe(dr("LandlinePhoneNumber"))
                    LastLoginDate = Safe(dr("LastLoginDate"), ValueTypes.TypeDateTime)
                    FatherWork = Safe(dr("FatherWork"))
                    MotherWork = Safe(dr("MotherWork"))
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    FillObject(DBA.SPDataRow(MyConn, "MEM_Student_Get", DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                StudentController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MEM_Student_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, SchoolId, 50),
                                  DBA.CreateParameter("FullName", SqlDbType.NVarChar, FullName, 50),
                                  DBA.CreateParameter("FatherName", SqlDbType.NVarChar, FatherName, 50),
                                  DBA.CreateParameter("MotherName", SqlDbType.NVarChar, MotherName, 50),
                                  DBA.CreateParameter("AreaId", SqlDbType.Int, AreaId),
                                  DBA.CreateParameter("Password", SqlDbType.NVarChar, Password, 24),
                                  DBA.CreateParameter("FamilyId", SqlDbType.Int, FamilyId),
                                  DBA.CreateParameter("SectionId", SqlDbType.Int, SectionId),
                                  DBA.CreateParameter("IsActive", SqlDbType.Bit, IsActive),
                                  DBA.CreateParameter("Email", SqlDbType.NVarChar, Email, 255),
                                  DBA.CreateParameter("RecordNumber", SqlDbType.NVarChar, RecordNumber, 100),
                                  DBA.CreateParameter("Religion", SqlDbType.NVarChar, Religion, 50),
                                  DBA.CreateParameter("Gender", SqlDbType.NVarChar, Gender, 50),
                                  DBA.CreateParameter("BirthInfo", SqlDbType.NVarChar, BirthInfo, 100),
                                  DBA.CreateParameter("PhoneNumber", SqlDbType.NVarChar, PhoneNumber, 50),
                                  DBA.CreateParameter("FatherPhoneNumber", SqlDbType.NVarChar, FatherPhoneNumber, 50),
                                  DBA.CreateParameter("MotherPhoneNumber", SqlDbType.NVarChar, MotherPhoneNumber, 50),
                                  DBA.CreateParameter("LandlinePhoneNumber", SqlDbType.NVarChar, LandlinePhoneNumber, 50),
                                  DBA.CreateParameter("UserId", SqlDbType.Int, CreatedUser),
                                  DBA.CreateParameter("FatherWork", SqlDbType.NVarChar, FatherWork, 255),
                                  DBA.CreateParameter("MotherWork", SqlDbType.NVarChar, MotherWork, 255)
                                  )
                StudentController.InsertAccessToken(Id, trans)
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MEM_Student_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, SchoolId, 50),
                               DBA.CreateParameter("FullName", SqlDbType.NVarChar, FullName, 50),
                               DBA.CreateParameter("FatherName", SqlDbType.NVarChar, FatherName, 50),
                               DBA.CreateParameter("MotherName", SqlDbType.NVarChar, MotherName, 50),
                               DBA.CreateParameter("AreaId", SqlDbType.Int, AreaId),
                               DBA.CreateParameter("Password", SqlDbType.NVarChar, Password, 24),
                               DBA.CreateParameter("FamilyId", SqlDbType.Int, FamilyId),
                               DBA.CreateParameter("SectionId", SqlDbType.Int, SectionId),
                               DBA.CreateParameter("IsActive", SqlDbType.Bit, IsActive),
                               DBA.CreateParameter("Email", SqlDbType.NVarChar, Email, 255),
                               DBA.CreateParameter("RecordNumber", SqlDbType.NVarChar, RecordNumber, 100),
                               DBA.CreateParameter("Religion", SqlDbType.NVarChar, Religion, 50),
                               DBA.CreateParameter("Gender", SqlDbType.NVarChar, Gender, 50),
                               DBA.CreateParameter("BirthInfo", SqlDbType.NVarChar, BirthInfo, 100),
                               DBA.CreateParameter("PhoneNumber", SqlDbType.NVarChar, PhoneNumber, 50),
                               DBA.CreateParameter("FatherPhoneNumber", SqlDbType.NVarChar, FatherPhoneNumber, 50),
                               DBA.CreateParameter("MotherPhoneNumber", SqlDbType.NVarChar, MotherPhoneNumber, 50),
                               DBA.CreateParameter("LandlinePhoneNumber", SqlDbType.NVarChar, LandlinePhoneNumber, 50),
                               DBA.CreateParameter("UserId", SqlDbType.Int, ModifiedUser),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("FatherWork", SqlDbType.NVarChar, FatherWork, 255),
                               DBA.CreateParameter("MotherWork", SqlDbType.NVarChar, MotherWork, 255)
                               )
            End Sub

#End Region

        End Class

        'controller
        Public Class StudentController

#Region "Public Methods"

            Public Shared Function GetBySchoolId(ByVal schoolId As String, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Dim q As String = "SELECT Id FROM MEM_Student WHERE SchoolId = @SchoolId"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, schoolId, 50)), ValueTypes.TypeInteger)
            End Function

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    DeleteRelated(id, conn)
                    Dim q As String = "DELETE FROM MEM_Student WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Sub ToggleState(ByVal id As Integer, Optional ByVal activate As Boolean = True, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE MEM_Student SET IsActive = @Activate WHERE Id = @Id"
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("Activate", SqlDbType.Bit, activate),
                             DBA.CreateParameter("Id", SqlDbType.Int, id)
                             )
            End Sub

            Public Shared Function UsernameExists(ByVal username As String, Optional ByVal id As Integer = 0, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_Student WHERE SchoolId = @SchoolId"
                If id > 0 Then q &= " AND Id <> @Id"
                Return DBA.Scalar(conn, q,
                                  DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, username, 50),
                                  DBA.CreateParameter("Id", SqlDbType.Int, id)
                                  ) > 0
            End Function

            Public Shared Function EmailExists(ByVal email As String, Optional ByVal id As Integer = 0, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If email <> String.Empty Then
                    Dim q As String = "SELECT COUNT(*) FROM MEM_Student WHERE Email = @Email"
                    If id > 0 Then q &= " AND Id <> @Id"
                    Return DBA.Scalar(conn, q,
                                      DBA.CreateParameter("Email", SqlDbType.NVarChar, email, 255),
                                      DBA.CreateParameter("Id", SqlDbType.Int, id)
                                      )
                Else
                    Return True
                End If
            End Function

            Public Shared Function GetByUsername(ByVal username As String, Optional ByVal conn As SqlConnection = Nothing) As Student
                If UsernameExists(username, 0, conn) Then
                    Return New Student(DBA.Scalar(conn, "SELECT Id FROM MEM_Student WHERE SchoolId = @Username", DBA.CreateParameter("Username", SqlDbType.NVarChar, username, 50)), conn)
                Else
                    Return Nothing
                End If
            End Function

            Public Shared Function Login(ByVal username As String, ByVal password As String, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim obj As Student = GetByUsername(username, conn)
                If obj IsNot Nothing Then
                    If obj.IsActive Then
                        If Helper.VerifyEncrypted(obj.Password, password) Then
                            Dim q As String = "UPDATE MEM_Student SET LastLoginDate = GETDATE()"
                            DBA.NonQuery(conn, q)
                            Return GetAccessToken(obj.Id, conn)
                        Else
                            Throw New Exception(Localization.GetResource("Resources.Global.CMS.UserIncorrectPassword"))
                        End If
                    Else
                        Throw New Exception(Localization.GetResource("Resources.Global.CMS.StudentNoActive"))
                    End If
                Else
                    Throw New Exception(Localization.GetResource("Resources.Global.CMS.StudentNotExists"))
                End If
            End Function

            Public Shared Function GetAccessToken(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim q As String = "SELECT AccessToken FROM MEM_Student WHERE Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Public Shared Function AccessTokenExists(ByVal token As String, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_Student WHERE AccessToken = @Token"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 100)), ValueTypes.TypeInteger) > 0
            End Function

            Public Shared Function GetByAccessToken(ByVal token As String, Optional ByVal conn As SqlConnection = Nothing) As Student
                If AccessTokenExists(token, conn) Then
                    Dim q As String = "SELECT Id FROM MEM_Student WHERE AccessToken = @Token"
                    Dim id As Integer = DBA.Scalar(conn, q, DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 100))
                    Return New Student(id, conn)
                Else
                    Return Nothing
                End If
            End Function

            Public Shared Sub InsertAccessToken(ByVal id As Integer, Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "UPDATE MEM_Student SET AccessToken = @Token WHERE Id = @Id"
                DBA.NonQuery(trans, q, DBA.CreateParameter("Token", SqlDbType.NVarChar, Helper.Encrypt("student" & id), 100), DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

            Public Shared Function ReadField(ByVal id As Integer, ByVal field As String, Optional ByVal conn As SqlConnection = Nothing) As Object
                Dim q As String = "SELECT " & field & " FROM MEM_Student WHERE Id = @Id"
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

            Public Shared Function GetSectionId(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Return Helper.GetSafeDBValue(ReadField(id, "SectionId", conn), ValueTypes.TypeInteger)
            End Function

            Public Shared Function GetClassId(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Dim q As String = "SELECT ClassId FROM MOD_Section WHERE Id = @Id"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, GetSectionId(id, conn))))
            End Function

            Public Shared Sub MoveToSection(ByVal id As Integer, ByVal newSectionId As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE MEM_Student SET SectionId = @NewId, ModifiedDate = GETDATE(), ModifiedUser = @UserId WHERE Id = @Id"
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("NewId", SqlDbType.Int, newSectionId),
                             DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id),
                             DBA.CreateParameter("Id", SqlDbType.Int, id)
                             )
            End Sub

            Public Shared Function GetCollection(ByVal conn As SqlConnection, Optional ByVal limit As Integer = 0,
                                                 Optional ByVal search As String = "", Optional ByVal onlyActive As Boolean = False,
                                                 Optional ByVal familyId As Integer = 0, Optional ByVal sectionId As Integer = 0,
                                                 Optional ByVal classId As Integer = 0,
                                                 Optional ByVal excludeId As Integer = 0) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim cq As New CustomQuery("MEM_Student", "S", conn)
                cq.OnlyMainColumns = True
                cq.AddFunctionalColumn("S.SchoolId + ' - ' + S.FullName", "IdName")
                If limit > 0 Then
                    cq.PageSize = limit
                    cq.PageIndex = 0
                    cq.EnablePaging = True
                Else
                    cq.EnablePaging = False
                End If
                If onlyActive Then
                    cq.AddCondition("S.IsActive = @Active")
                    cq.AddParameter(DBA.CreateParameter("Active", SqlDbType.Bit, True))
                End If
                If search <> String.Empty Then
                    cq.AddCondition("(S.SchoolId LIKE '%' + @Query + '%' OR S.Email LIKE '%' + @Query + '%' OR S.FullName LIKE '%' + @Query + '%')")
                    cq.AddParameter(DBA.CreateParameter("Query", SqlDbType.NVarChar, search, 255))
                End If
                If sectionId > 0 Then
                    cq.AddCondition("S.SectionId = @SectionId")
                    cq.AddParameter(DBA.CreateParameter("SectionId", SqlDbType.Int, sectionId))
                End If
                If classId > 0 Then
                    cq.AddCondition("C.ClassId = @ClassId")
                    cq.AddParameter(DBA.CreateParameter("ClassId", SqlDbType.Int, classId))
                    cq.AddJoinTable("MOD_Section", "C", "Id", "SectionId")
                End If
                If familyId > 0 Then
                    cq.AddCondition("S.FamilyId = @FamilyId")
                    cq.AddParameter(DBA.CreateParameter("FamilyId", SqlDbType.Int, familyId))
                End If
                If excludeId > 0 Then
                    cq.AddCondition("S.Id <> @ExcludeId")
                    cq.AddParameter(DBA.CreateParameter("ExcludeId", SqlDbType.Int, excludeId))
                End If
                cq.AddSortColumn("FullName", SortDirections.Ascending, "S")
                ret.Query = cq.GetQuery()
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

            Public Shared Function getSYear(ByVal conn As SqlConnection, Optional ByVal search As String = "", Optional ByVal SID As Integer = 0) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim cq As New CustomQuery("MEM_StudentSort", "SS", conn)
                cq.OnlyMainColumns = False
                cq.AddFunctionalColumn("SS.SYear", "IdSYear")
                If search <> String.Empty Then
                    cq.AddCondition("(SS.SYear LIKE '%' + @Query + '%')")
                    cq.AddParameter(DBA.CreateParameter("Query", SqlDbType.NVarChar, search, 255))
                End If
                If SID > 0 Then
                    cq.AddCondition("(SS.StudentId = @SID)")
                    cq.AddParameter(DBA.CreateParameter("SID", SqlDbType.Int, SID))
                End If
                ret.Query = cq.GetQuery()
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

            Public Shared Function GetSTerm(ByVal conn As SqlConnection, Optional ByVal search As String = "", Optional ByVal Year As String = "", Optional ByVal SID As Integer = 0) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim cq As New CustomQuery("MEM_StudentSort", "SS", conn)
                cq.OnlyMainColumns = False
                cq.AddFunctionalColumn("SS.STerm", "IdSTerm")
                If search <> String.Empty Then
                    cq.AddCondition("(SS.STerm LIKE '%' + @Query + '%')")
                    cq.AddParameter(DBA.CreateParameter("Query", SqlDbType.NVarChar, search, 255))
                End If
                If Year <> String.Empty Then
                    cq.AddCondition("(SS.SYear LIKE '%' + @Year + '%')")
                    cq.AddParameter(DBA.CreateParameter("Year", SqlDbType.NVarChar, Year, 50))
                End If
                If SID > 0 Then
                    cq.AddCondition("(SS.StudentId = @SID)")
                    cq.AddParameter(DBA.CreateParameter("SID", SqlDbType.Int, SID))
                End If
                ret.Query = cq.GetQuery()
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

            Public Shared Function GetSClass(ByVal conn As SqlConnection, Optional ByVal search As String = "", Optional ByVal Year As String = "", Optional ByVal Term As String = "", Optional ByVal SID As Integer = 0) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim cq As New CustomQuery("MEM_StudentSort", "SS", conn)
                cq.OnlyMainColumns = False
                cq.AddFunctionalColumn("SS.SClass", "IdSClass")
                If search <> String.Empty Then
                    cq.AddCondition("(SS.SClass LIKE '%' + @Query + '%')")
                    cq.AddParameter(DBA.CreateParameter("Query", SqlDbType.NVarChar, search, 255))
                End If
                If Year <> String.Empty Then
                    cq.AddCondition("(SS.SYear LIKE '%' + @Year + '%')")
                    cq.AddParameter(DBA.CreateParameter("Year", SqlDbType.NVarChar, Year, 50))
                End If
                If Term <> String.Empty Then
                    cq.AddCondition("(SS.STerm LIKE '%' + @Term + '%')")
                    cq.AddParameter(DBA.CreateParameter("Term", SqlDbType.NVarChar, Term, 50))
                End If
                If SID > 0 Then
                    cq.AddCondition("(SS.StudentId = @SID)")
                    cq.AddParameter(DBA.CreateParameter("SID", SqlDbType.Int, SID))
                End If
                ret.Query = cq.GetQuery()
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

            Public Shared Function GetStudentTeachers(ByVal conn As SqlConnection, ByVal studentId As Integer,
                                                      Optional ByVal search As String = "") As List(Of UserObject)
                Dim lst As New List(Of UserObject)
                Dim q As String = "SELECT P.UserId, P.FullName FROM MEM_Student S INNER JOIN MOD_SectionMaterialUser M ON S.SectionId = M.SectionId INNER JOIN MOD_Section T ON M.SectionId = T.Id AND T.SeasonId = @SeasonId INNER JOIN SYS_User U ON U.Id = M.UserId AND U.IsActive = @Active INNER JOIN SYS_UserProfile P ON M.UserId = P.UserId WHERE P.FullName LIKE '%' + @SearchTerm + '%' AND S.Id = @StudentId GROUP BY P.UserId, P.FullName"
                Using dt As DataTable = DBA.DataTable(conn, q,
                                                      DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn)),
                                                      DBA.CreateParameter("SearchTerm", SqlDbType.NVarChar, search, 255),
                                                      DBA.CreateParameter("StudentId", SqlDbType.Int, studentId),
                                                      DBA.CreateParameter("Active", SqlDbType.Bit, True)
                                                      )
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New UserObject() With {
                            .Id = Helper.GetSafeDBValue(dr("UserId"), ValueTypes.TypeInteger),
                            .FullName = Helper.GetSafeDBValue(dr("FullName"))
                        })
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function GetTeacherStudents(ByVal conn As SqlConnection, ByVal userId As Integer,
                                                      Optional ByVal search As String = "",
                                                      Optional ByVal onlyActive As Boolean = False,
                                                      Optional ByVal limit As Integer = 0) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Dim p As New List(Of SqlParameter)
                p.Add(DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn)))
                Dim q As String = "SELECT" & IIf(limit > 0, " TOP " & limit, "") & " S.Id, (S.SchoolId + ' - ' + S.FullName) AS [IdName] FROM MEM_Student S INNER JOIN MOD_Section C ON S.SectionId = C.Id WHERE C.SeasonId = @SeasonId"
                If UserTypeRoleController.UserInType(userId, UserTypes.Teacher, conn) Then
                    q = "SELECT" & IIf(limit > 0, " TOP " & limit, "") & " (S.SchoolId + ' - ' + S.FullName) AS IdName, S.Id FROM MEM_Student S INNER JOIN ( SELECT U.SectionId FROM MOD_SectionMaterialUser U INNER JOIN MOD_Section C ON U.SectionId = C.Id AND C.SeasonId = @SeasonId WHERE U.UserId = @UserId) M ON S.SectionId = M.SectionId {0} GROUP BY S.SchoolId, S.FullName, S.Id"
                    p.Add(DBA.CreateParameter("UserId", SqlDbType.Int, userId))
                    Dim lst As New List(Of String)
                    If onlyActive Then
                        lst.Add("S.IsActive = @Active")
                        p.Add(DBA.CreateParameter("Active", SqlDbType.Bit, True))
                    End If
                    If search <> String.Empty Then
                        lst.Add("(S.SchoolId LIKE '%' + @Search + '%' OR S.FullName LIKE '%' + @Search + '%')")
                        p.Add(DBA.CreateParameter("Search", SqlDbType.NVarChar, search, 255))
                    End If
                    If lst.Count > 0 Then
                        q = String.Format(q, "WHERE " & String.Join(" AND ", lst.ToArray()))
                    End If
                Else
                    If onlyActive Then
                        q &= " AND S.IsActive = @Active"
                        p.Add(DBA.CreateParameter("Active", SqlDbType.Bit, True))
                    End If
                    If search <> String.Empty Then
                        q &= " AND (S.SchoolId LIKE '%' + @Search + '%' OR S.FullName LIKE '%' + @Search + '%')"
                        p.Add(DBA.CreateParameter("Search", SqlDbType.NVarChar, search, 255))
                    End If
                End If
                q &= " ORDER BY S.SchoolId ASC"
                ret.Query = q
                Dim dt As DataTable = DBA.DataTable(conn, q, p.ToArray())
                ret.Count = dt.Rows.Count
                ret.List = dt
                Return ret
            End Function

            Public Shared Function GetStudentMaterials(ByVal id As Integer, ByVal teacherId As Integer,
                                                       ByVal classId As Integer, Optional ByVal langId As Integer = 0,
                                                       Optional ByVal conn As SqlConnection = Nothing) As DBAReturnObject
                Helper.GetSafeLanguageId(langId)
                Dim q As String = "SELECT (M.Code + ' - ' + R.Title) AS MaterialTitle, M.Id, M.MaxMark FROM MOD_Material M INNER JOIN MOD_Material_Res R ON M.Id = R.Id AND R.LanguageId = @LanguageId WHERE M.ClassId = @ClassId"
                Dim p As New List(Of SqlParameter)
                p.Add(DBA.CreateParameter("LanguageId", SqlDbType.Int, langId))
                p.Add(DBA.CreateParameter("ClassId", SqlDbType.Int, classId))
                If UserTypeRoleController.UserInType(teacherId, UserTypes.Teacher, conn) Then
                    q = "SELECT (M.Code + ' - ' + R.Title) AS MaterialTitle, M.Id, M.MaxMark FROM MOD_Material M INNER JOIN MOD_Material_Res R ON M.Id = R.Id AND R.LanguageId = @LanguageId INNER JOIN MOD_SectionMaterialUser U ON U.MaterialId = M.Id AND U.UserId = @UserId AND SectionId = @SectionId WHERE M.ClassId = @ClassId GROUP BY M.Code, R.Title, M.Id, M.MaxMark"
                    p.Add(DBA.CreateParameter("UserId", SqlDbType.Int, teacherId))
                    Dim sectionId As Integer = StudentController.GetSectionId(id, conn)
                    p.Add(DBA.CreateParameter("SectionId", SqlDbType.Int, sectionId))
                End If
                Dim ret As New DBAReturnObject()
                ret.Query = q
                Dim dt As DataTable = DBA.DataTable(conn, q, p.ToArray())
                ret.Count = dt.Rows.Count
                ret.List = dt
                Return ret
            End Function

            Public Shared Function GetScheduleFilePath(ByVal conn As SqlConnection, ByVal accessToken As String) As String
                Dim q As String = "SELECT E.ScheduleFilePath FROM MEM_Student S INNER JOIN MOD_Section E ON S.SectionId = E.Id AND E.SeasonId = @SeasonId WHERE S.AccessToken = @AccessToken"
                Dim filePath As String = DBA.Scalar(conn, q,
                                                    DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn)),
                                                    DBA.CreateParameter("AccessToken", SqlDbType.NVarChar, accessToken, 100)
                                                    )
                Return IIf(filePath <> String.Empty, SettingController.ReadSetting("SITELINK", conn) & filePath, String.Empty)
            End Function

            Public Shared Function GetTotal(ByVal conn As SqlConnection) As Integer
                Dim q As String = "SELECT COUNT(*) FROM MEM_Student WHERE IsActive = @Active"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Active", SqlDbType.Bit, True))
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not (HasTest(id, conn) OrElse HasAttendance(id, conn) OrElse HasMessages(id, conn))
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)

            End Sub

            Private Shared Function HasTest(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_StudentExam WHERE StudentId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Private Shared Function HasAttendance(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_StudentAttendance WHERE StudentId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Private Shared Function HasMessages(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM COM_MessageUser WHERE UserId = @Id AND UserType = @UserType"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id), DBA.CreateParameter("UserType", SqlDbType.Int, MessageUserTypes.Family))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
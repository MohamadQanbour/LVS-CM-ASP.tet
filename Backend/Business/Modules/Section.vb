Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Interfaces
Imports EGV.Constants

Namespace EGV
    Namespace Business

        'object
        Public Class Section
            Inherits AudLocBusinessBase
            Implements ILocBusinessClass

#Region "Public Properties"

            Public Property Id As Integer = 0 Implements ILocBusinessClass.Id
            Public Property Code As String = String.Empty
            Public Property Title As String = String.Empty Implements ILocBusinessClass.Title
            Public Property ClassId As Integer = 0
            Public Property SeasonId As Integer = 0
            Public Property ScheduleFilePath As String

#End Region

#Region "Filler"

            Public Overrides Sub FillObject(dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    Code = Safe(dr("Code"))
                    Title = Safe(dr("Title"))
                    ClassId = Safe(dr("ClassId"), ValueTypes.TypeInteger)
                    SeasonId = Safe(dr("SeasonId"), ValueTypes.TypeInteger)
                    ScheduleFilePath = Safe(dr("ScheduleFilePath"))
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Contructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal langId As Integer = 0)
                MyBase.New(conn, langId)
                If tid > 0 Then
                    FillObject(DBA.SPDataRow(MyConn, "MOD_Section_Get", DBA.CreateParameter("Id", SqlDbType.Int, tid), DBA.CreateParameter("LanguageId", SqlDbType.Int, MyLanguageId)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                SectionController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Section_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("Code", SqlDbType.NVarChar, Code, 50),
                                  DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                  DBA.CreateParameter("ClassId", SqlDbType.Int, ClassId),
                                  DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonId),
                                  DBA.CreateParameter("ScheduleFilePath", SqlDbType.NVarChar, ScheduleFilePath, 255),
                                  DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id)
                                  )
                InsertRes(trans)
            End Sub

            Public Overrides Sub InsertRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Section_AddRes"
                For Each lid As Integer In LanguageController.GetIds(MyConn, trans)
                    DBA.SPNonQuery(trans, sp,
                                   DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                   DBA.CreateParameter("Id", SqlDbType.Int, Id),
                                   DBA.CreateParameter("LanguageId", SqlDbType.Int, lid)
                                   )
                Next
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Translate(langId As Integer, userId As Integer, Optional trans As SqlTransaction = Nothing) Implements ILocBusinessClass.Translate
                Dim sp As String = "MOD_Section_Translate"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                               DBA.CreateParameter("UserId", SqlDbType.Int, userId)
                               )
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Section_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Code", SqlDbType.NVarChar, Code, 50),
                               DBA.CreateParameter("ClassId", SqlDbType.Int, ClassId),
                               DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonId),
                               DBA.CreateParameter("ScheduleFilePath", SqlDbType.NVarChar, ScheduleFilePath, 255),
                               DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
                If MyLanguageId = LanguageController.GetDefaultId(MyConn, trans) Then UpdateDefaultRes(trans)
                UpdateRes(trans)
            End Sub

            Public Overrides Sub UpdateDefaultRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Section_UpdateDefaultRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub UpdateRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Section_UpdateRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LangId", SqlDbType.Int, MyLanguageId)
                               )
            End Sub

#End Region

        End Class

        'controller
        Public Class SectionController

#Region "Public Methods"

            Public Shared Function GetByCode(ByVal code As String, Optional ByVal currentYear As Boolean = True, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Dim q As String = "SELECT Id FROM MOD_Section WHERE Code = @Code{0}"
                If currentYear Then q = String.Format(q, " AND SeasonId = " & SeasonController.GetCurrentId(conn)) Else q = String.Format(q, "")
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Code", SqlDbType.NVarChar, code, 50)), ValueTypes.TypeInteger)
            End Function

            Public Shared Function GetByTitle(ByVal title As String, Optional ByVal langId As Integer = 0, Optional ByVal currentYear As Boolean = True, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Helper.GetSafeLanguageId(langId)
                Dim q As String = "SELECT Id FROM MOD_Section_Res WHERE Title = @Title AND LanguageId = @LangId{0}"
                If currentYear Then q = String.Format(q, " AND Id IN (SELECT Id FROM MOD_Section WHERE SeasonId = " & SeasonController.GetCurrentId(conn) & ")") Else q = String.Format(q, "")
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Title", SqlDbType.NVarChar, title, 50), DBA.CreateParameter("LangId", SqlDbType.Int, langId)), ValueTypes.TypeInteger)
            End Function

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM MOD_Section WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetCollection(ByVal conn As SqlConnection, ByVal langId As Integer, Optional ByVal limit As Integer = 0,
                                                 Optional ByVal search As String = "", Optional ByVal onlyCurrentYear As Boolean = False,
                                                 Optional ByVal classId As Integer = 0, Optional ByVal userId As Integer = 0) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Helper.GetSafeLanguageId(langId)
                Dim cq As New CustomQuery("MOD_Section", "S", conn)
                cq.AddColumn("Title", "R")
                cq.AddColumn("Id", "S")
                cq.AddColumn("Title", "C", "ClassName")
                cq.AddFunctionalColumn("C.Title + ' - ' + R.Title", "FullName")
                cq.AddCondition("R.LanguageId = @LangId")
                cq.AddParameter(DBA.CreateParameter("LangId", SqlDbType.Int, langId))
                If limit > 0 Then
                    cq.PageSize = limit
                    cq.PageIndex = 0
                    cq.EnablePaging = True
                Else
                    cq.EnablePaging = False
                End If
                If search <> String.Empty Then
                    cq.AddCondition("(R.Title LIKE N'%' + @Query + N'%' OR S.Code LIKE N'%' + @Query + N'%' OR C.Title LIKE N'%' + @Query + N'%')")
                    cq.AddParameter(DBA.CreateParameter("Query", SqlDbType.NVarChar, search, 255))
                End If
                If onlyCurrentYear Then
                    cq.AddCondition("S.SeasonId = @SeasonId")
                    cq.AddParameter(DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn)))
                End If
                If classId > 0 Then
                    cq.AddCondition("S.ClassId = @ClassId")
                    cq.AddParameter(DBA.CreateParameter("ClassId", SqlDbType.Int, classId))
                End If
                If userId > 0 Then
                    cq.AddCondition("M.UserId = @UserId")
                    cq.AddParameter(DBA.CreateParameter("userId", SqlDbType.Int, userId))
                    cq.AddJoinTable("MOD_SectionMaterialUser", "M", "SectionId", "Id")
                End If
                cq.AddJoinTable("MOD_Section_Res", "R", "Id", "Id", TableJoinTypes.Inner, True)
                cq.AddJoinTable("MOD_Class_Res", "C", "Id", "ClassId", TableJoinTypes.Left, True)
                ret.Query = cq.GetQuery()
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

            Public Shared Function GetSectionClassId(ByVal conn As SqlConnection, ByVal sectionId As Integer) As Integer
                Dim q As String = "SELECT ClassId FROM MOD_Section WHERE Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, sectionId))
            End Function

            Public Shared Function GetSectionMaterials(ByVal conn As SqlConnection, ByVal sectionId As Integer,
                                                       Optional ByVal langId As Integer = 0) As List(Of MaterialObject)
                Helper.GetSafeLanguageId(langId)
                Dim classId As Integer = GetSectionClassId(conn, sectionId)
                Dim lst As New List(Of MaterialObject)
                Dim q As String = "SELECT M.Id, R.Title FROM MOD_Material M INNER JOIN MOD_Material_Res R ON M.Id = R.Id AND R.LanguageId = @LanguageId WHERE M.ClassId = @Id"
                Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("LanguageId", SqlDbType.Int, langId), DBA.CreateParameter("Id", SqlDbType.Int, classId))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New MaterialObject() With {
                            .Id = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                            .Title = Helper.GetSafeDBValue(dr("Title"))
                        })
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function GetTitle(ByVal conn As SqlConnection, ByVal sectionId As Integer,
                                            Optional ByVal langId As Integer = 0) As String
                Helper.GetSafeLanguageId(langId)
                Dim q As String = "SELECT Title FROM MOD_Section_Res WHERE Id = @Id AND LanguageId = @LanguageId"
                Dim sectionTitle As String = DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, sectionId), DBA.CreateParameter("LanguageId", SqlDbType.Int, langId))
                Dim classId As Integer = GetSectionClassId(conn, sectionId)
                Return StudyClassController.GetTitle(conn, classId, langId) & " - " & sectionTitle
            End Function

            Public Shared Function GetTeacherSections(ByVal conn As SqlConnection, ByVal userId As Integer,
                                                      Optional ByVal langId As Integer = 0,
                                                      Optional ByVal search As String = "",
                                                      Optional ByVal classId As Integer = 0) As List(Of SectionObject)
                Helper.GetSafeLanguageId(langId)
                Dim q As String = "SELECT S.Id, R.Title FROM MOD_Section S INNER JOIN MOD_Section_Res R ON S.Id = R.Id AND R.LanguageId = @LanguageId WHERE R.Title LIKE N'%' + @SearchTerm + N'%' AND S.SeasonId = @SeasonId"
                If UserTypeRoleController.UserInType(userId, UserTypes.Teacher, conn) Then
                    q = "SELECT S.Id, R.Title FROM MOD_SectionMaterialUser M INNER JOIN MOD_Section S ON M.SectionId = S.Id INNER JOIN MOD_Section_Res R ON S.Id = R.Id AND R.LanguageId = @LanguageId WHERE M.UserId = @UserId AND R.Title LIKE N'%' + @SearchTerm + N'%' AND S.SeasonId = @SeasonId"
                End If
                If classId > 0 Then q &= " AND S.ClassId = @ClassId"
                q &= " GROUP BY S.Id, R.Title"
                Dim lst As New List(Of SectionObject)
                Dim dt As DataTable = DBA.DataTable(conn, q,
                                                    DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                                                    DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                                                    DBA.CreateParameter("SearchTerm", SqlDbType.NVarChar, search, 255),
                                                    DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn)),
                                                    DBA.CreateParameter("ClassId", SqlDbType.Int, classId)
                                                    )
                For Each dr As DataRow In dt.Rows
                    lst.Add(New SectionObject With {
                        .Id = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                        .Title = Helper.GetSafeDBValue(dr("Title"))
                    })
                Next
                Return lst
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not (HasStudents(id, conn))
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, ByVal conn As SqlConnection)
                Dim q As String = "DELETE FROM MOD_Section_Res WHERE Id = @Id"
                Dim p = DBA.CreateParameter("Id", SqlDbType.Int, id)
                DBA.NonQuery(conn, q, p)
                q = "DELETE FROM MOD_SectionMaterialUser WHERE SectionId = @Id"
                DBA.NonQuery(conn, q, p)
            End Sub

            Private Shared Function HasStudents(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_Student WHERE SectionId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
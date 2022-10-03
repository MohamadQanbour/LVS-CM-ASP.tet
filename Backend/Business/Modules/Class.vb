Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Interfaces
Imports EGV.Constants

Namespace EGV
    Namespace Business

        'object
        Public Class StudyClass
            Inherits AudLocBusinessBase
            Implements ILocBusinessClass

#Region "Public Members"

            Public Property Id As Integer = 0 Implements ILocBusinessClass.Id
            Public Property Code As String = String.Empty
            Public Property Title As String = String.Empty Implements ILocBusinessClass.Title

            Public Property Templates As List(Of Integer)

#End Region

#Region "Filler"

            Public Overrides Sub FillObject(dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    Code = Safe(dr("Code"))
                    Title = Safe(dr("Title"))
                    Templates = StudyClassController.GetTemplates(Id, MyConn)
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Contructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal langId As Integer = 0)
                MyBase.New(conn, langId)
                Templates = New List(Of Integer)()
                If tid > 0 Then
                    FillObject(DBA.SPDataRow(MyConn, "MOD_Class_Get", DBA.CreateParameter("Id", SqlDbType.Int, tid), DBA.CreateParameter("LanguageId", SqlDbType.Int, MyLanguageId)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                StudyClassController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Class_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("Code", SqlDbType.NVarChar, Code, 50),
                                  DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                  DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id)
                                  )
                StudyClassController.DeleteTemplates(Id, trans)
                StudyClassController.AddTemplates(Id, Templates, trans)
                InsertRes(trans)
            End Sub

            Public Overrides Sub InsertRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Class_AddRes"
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
                Dim sp As String = "MOD_Class_Translate"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                               DBA.CreateParameter("UserId", SqlDbType.Int, userId)
                               )
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Class_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Code", SqlDbType.NVarChar, Code, 50),
                               DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
                StudyClassController.DeleteTemplates(Id, trans)
                StudyClassController.AddTemplates(Id, Templates, trans)
                If MyLanguageId = LanguageController.GetDefaultId(MyConn, trans) Then UpdateDefaultRes(trans)
                UpdateRes(trans)
            End Sub

            Public Overrides Sub UpdateDefaultRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Class_UpdateDefaultRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub UpdateRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Class_UpdateRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LangId", SqlDbType.Int, MyLanguageId)
                               )
            End Sub

#End Region

        End Class

        'controller
        Public Class StudyClassController

#Region "Public Methods"

            Public Shared Function GetClassMaxMark(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal templateItems() As Integer = Nothing) As Integer
                Dim tempItemsQuery As String = String.Empty
                If templateItems IsNot Nothing AndAlso templateItems.Length > 0 Then tempItemsQuery = "(" & String.Join(", ", templateItems) & ")"
                'get class materials
                Dim mats As New List(Of String)
                Using dt As DataTable = DBA.DataTable(conn, "SELECT Id FROM MOD_Material WHERE ClassId = @Id", DBA.CreateParameter("Id", SqlDbType.Int, id))
                    For Each dr As DataRow In dt.Rows
                        mats.Add(Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger))
                    Next
                End Using
                Dim maxMarks As Integer = 0
                For Each m In mats
                    'material max mark
                    Dim defMatMark As Integer = MaterialController.GetMaxMark(m, conn)
                    'get material exam items
                    Dim matMax As Integer = 0
                    'get template items
                    Dim matTempId As Integer = MaterialController.GetTemplateId(conn, m)
                    If tempItemsQuery <> String.Empty Then
                        Using dtItems As DataTable = DBA.DataTable(conn, "SELECT Id FROM MOD_ExamTemplateItem WHERE TemplateId = @TemplateId" & IIf(tempItemsQuery <> String.Empty, " AND Id In " & tempItemsQuery, ""), DBA.CreateParameter("TemplateId", SqlDbType.Int, matTempId), DBA.CreateParameter("Type", SqlDbType.Int, ExamItemTypes.Number))
                            For Each drItems As DataRow In dtItems.Rows
                                Dim curMax As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, "SELECT MaxMark FROM MOD_MaterialExamTemplateItem WHERE MaterialId = @Id AND ExamTemplateItemId = @TemplateItemId", DBA.CreateParameter("Id", SqlDbType.Int, m), DBA.CreateParameter("TemplateItemId", SqlDbType.Int, Helper.GetSafeDBValue(drItems("Id"), ValueTypes.TypeInteger))), ValueTypes.TypeInteger)
                                If curMax > 0 Then matMax += curMax Else matMax += defMatMark
                            Next
                        End Using
                    Else
                        Using dtItems As DataTable = DBA.DataTable(conn, "SELECT Id FROM MOD_ExamTemplateItem WHERE TemplateId = @TemplateId AND [Type] = @Type" & IIf(tempItemsQuery <> String.Empty, " AND Id In " & tempItemsQuery, ""), DBA.CreateParameter("TemplateId", SqlDbType.Int, matTempId), DBA.CreateParameter("Type", SqlDbType.Int, ExamItemTypes.Number))
                            For Each drItems As DataRow In dtItems.Rows
                                Dim curMax As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, "SELECT MaxMark FROM MOD_MaterialExamTemplateItem WHERE MaterialId = @Id AND ExamTemplateItemId = @TemplateItemId", DBA.CreateParameter("Id", SqlDbType.Int, m), DBA.CreateParameter("TemplateItemId", SqlDbType.Int, Helper.GetSafeDBValue(drItems("Id"), ValueTypes.TypeInteger))), ValueTypes.TypeInteger)
                                If curMax > 0 Then matMax += curMax Else matMax += defMatMark
                            Next
                        End Using
                    End If
                    maxMarks += matMax
                    ''total template items
                    'Dim totalItems As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, "SELECT COUNT(*) FROM MOD_Material M INNER JOIN MOD_ExamTemplate T ON M.ExamTemplateId = T.Id INNER JOIN MOD_ExamTemplateItem I ON T.Id = I.TemplateId WHERE I.[Type] = 1 AND M.Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, m)), ValueTypes.TypeInteger)
                    ''total material template items
                    'Dim totalMaterialItems As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, "SELECT COUNT(*) FROM MOD_Material M INNER JOIN MOD_MaterialExamTemplateItem I ON M.Id = I.MaterialId WHERE M.Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, m)), ValueTypes.TypeInteger)
                    ''get material max mark
                    'Dim materialMax As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, "SELECT SUM(I.MaxMark) FROM MOD_Material M INNER JOIN MOD_MaterialExamTemplateItem I ON M.Id = I.MaterialId WHERE M.Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, m)), ValueTypes.TypeInteger)
                    'If totalItems > totalMaterialItems Then
                    '    'get template max mark
                    '    Dim templateMax As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, "SELECT T.MaxMark FROM MOD_Material M INNER JOIN MOD_ExamTemplate T ON M.ExamTemplateId = T.Id WHERE M.Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, m)), ValueTypes.TypeInteger)
                    '    'add to material max mark
                    '    materialMax = materialMax + ((totalItems - totalMaterialItems) * templateMax)
                    'End If
                    'maxMarks = maxMarks + materialMax
                Next
                Return maxMarks
            End Function

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM MOD_Class WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetCollection(ByVal conn As SqlConnection, ByVal langId As Integer, Optional ByVal limit As Integer = 0,
                                                 Optional ByVal search As String = "") As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Helper.GetSafeLanguageId(langId)
                Dim cq As New CustomQuery("MOD_Class_Res", "C", conn)
                cq.AddCondition("C.LanguageId = @LangId")
                cq.AddParameter(DBA.CreateParameter("LangId", SqlDbType.Int, langId))
                If limit > 0 Then
                    cq.PageSize = limit
                    cq.PageIndex = 0
                    cq.EnablePaging = True
                Else
                    cq.EnablePaging = False
                End If
                If search <> String.Empty Then
                    cq.AddCondition("C.Title LIKE N'%' + @Query + N'%'")
                    cq.AddParameter(DBA.CreateParameter("Query", SqlDbType.NVarChar, search, 255))
                End If
                ret.Query = cq.GetQuery()
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

            Public Shared Function GetIdByTitle(ByVal title As String, ByVal conn As SqlConnection) As Integer
                Dim q As String = "SELECT TOP 1 Id FROM MOD_Class_Res WHERE Title = @Title"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Title", SqlDbType.NVarChar, title, 255)), ValueTypes.TypeInteger)
            End Function

            Public Shared Function GetIdByCode(ByVal code As String, ByVal conn As SqlConnection) As Integer
                Dim q As String = "SELECT TOP 1 Id FROM MOD_Class WHERE Code = @Code"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Code", SqlDbType.NVarChar, code, 50)), ValueTypes.TypeInteger)
            End Function

            Public Shared Function GetTitle(ByVal conn As SqlConnection, ByVal id As Integer, ByVal langId As Integer) As String
                Dim q As String = "SELECT Title FROM MOD_Class_Res WHERE Id = @Id AND LanguageId = @LanguageId"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id), DBA.CreateParameter("LanguageId", SqlDbType.Int, langId))
            End Function

            Public Shared Function GetClassMaterials(ByVal conn As SqlConnection, ByVal classId As Integer,
                                                     Optional ByVal langId As Integer = 0) As List(Of MaterialObject)
                Helper.GetSafeLanguageId(langId)
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

            Public Shared Sub UpdateDays(ByVal conn As SqlConnection, ByVal classId As Integer,
                                         ByVal schoolDays As Integer, ByVal holidayDays As Integer,
                                         Optional ByVal schoolDays2 As Integer = 0,
                                         Optional ByVal holidayDays2 As Integer = 0)
                Dim q As String = "UPDATE MOD_Class SET SchoolDays = @School, HolidayDays = @Holiday, SchoolDays2 = @School2, HolidayDays2 = @Holiday2 WHERE Id = @Id"
                If schoolDays2 = 0 Then schoolDays2 = schoolDays
                If holidayDays2 = 0 Then holidayDays2 = holidayDays
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("School", SqlDbType.Int, schoolDays),
                             DBA.CreateParameter("Holiday", SqlDbType.Int, holidayDays),
                             DBA.CreateParameter("School2", SqlDbType.Int, schoolDays2),
                             DBA.CreateParameter("Holiday2", SqlDbType.Int, holidayDays2),
                             DBA.CreateParameter("Id", SqlDbType.Int, classId)
                             )
            End Sub

            Public Shared Function GetClassesDays(ByVal conn As SqlConnection, Optional ByVal langId As Integer = 0) As List(Of ClassDays)
                Helper.GetSafeLanguageId(langId)
                Dim lst As New List(Of ClassDays)
                Dim q As String = "SELECT C.*, R.Title AS [ClassTitle] FROM MOD_Class C INNER JOIN MOD_Class_Res R ON C.Id = R.Id AND R.LanguageId = @LanguageId"
                Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("LanguageId", SqlDbType.Int, langId))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New ClassDays() With {
                            .ClassId = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                            .ClassTitle = Helper.GetSafeDBValue(dr("Code")) & " - " & Helper.GetSafeDBValue(dr("ClassTitle")),
                            .HolidayDays = Helper.GetSafeDBValue(dr("HolidayDays"), ValueTypes.TypeInteger),
                            .SchoolDays = Helper.GetSafeDBValue(dr("SchoolDays"), ValueTypes.TypeInteger),
                            .HolidayDays2 = Helper.GetSafeDBValue(dr("HolidayDays2"), ValueTypes.TypeInteger),
                            .SchoolDays2 = Helper.GetSafeDBValue(dr("SchoolDays2"), ValueTypes.TypeInteger)
                        })
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function GetTeacherClasses(ByVal conn As SqlConnection, ByVal userId As Integer,
                                                     Optional ByVal langId As Integer = 0,
                                                     Optional ByVal search As String = "") As List(Of ClassObject)
                Helper.GetSafeLanguageId(langId)
                Dim lst As New List(Of ClassObject)
                Dim q As String = "SELECT C.Id AS [ClassId], C.Title FROM MOD_Class_Res C WHERE C.LanguageId = @LanguageId AND C.Title LIKE '%' + @SearchTerm + '%'"
                If UserTypeRoleController.UserInType(userId, UserTypes.Teacher, conn) Then
                    q = "SELECT S.ClassId, R.Title FROM MOD_SectionMaterialUser M INNER JOIN MOD_Section S ON M.SectionId = S.Id INNER JOIN MOD_Class_Res R ON S.ClassId = R.Id AND R.LanguageId = @LanguageId WHERE M.UserId = @UserId AND R.Title LIKE '%' + @SearchTerm + '%' AND S.SeasonId = @SeasonId GROUP BY S.ClassId, R.Title"
                End If
                Dim dt As DataTable = DBA.DataTable(conn, q,
                                                    DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                                                    DBA.CreateParameter("UserId", SqlDbType.Int, userId),
                                                    DBA.CreateParameter("SearchTerm", SqlDbType.NVarChar, search, 255),
                                                    DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn))
                                                    )
                For Each dr As DataRow In dt.Rows
                    lst.Add(New ClassObject With {
                        .Id = Helper.GetSafeDBValue(dr("ClassId"), ValueTypes.TypeInteger),
                        .Title = Helper.GetSafeDBValue(dr("Title"))
                    })
                Next
                Return lst
            End Function

            Public Shared Function GetTemplates(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As List(Of Integer)
                Dim lst As New List(Of Integer)
                Using dt As DataTable = DBA.DataTable(conn, "SELECT TemplateId FROM MOD_ClassTemplate WHERE ClassId = @Id", DBA.CreateParameter("Id", SqlDbType.Int, id))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(Helper.GetSafeDBValue(dr("TemplateId"), ValueTypes.TypeInteger))
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function GetTemplateObjects(ByVal id As Integer, Optional ByVal langId As Integer = 0, Optional ByVal conn As SqlConnection = Nothing) As List(Of ExamTemplateObject)
                Helper.GetSafeLanguageId(langId)
                Dim lst As New List(Of ExamTemplateObject)
                Dim q As String = "SELECT T.Id, T.Title FROM MOD_ClassTemplate C INNER JOIN MOD_ExamTemplate_Res T ON C.TemplateId = T.Id AND T.LanguageId = @LanguageId WHERE C.ClassId = @Id"
                Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("LanguageId", SqlDbType.Int, langId), DBA.CreateParameter("Id", SqlDbType.Int, id))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New ExamTemplateObject() With {
                            .Id = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                            .Title = Helper.GetSafeDBValue(dr("Title"))
                        })
                    Next
                End Using
                Return lst
            End Function

            Public Shared Sub DeleteTemplates(ByVal id As Integer, Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "DELETE FROM MOD_ClassTemplate WHERE ClassId = @Id"
                DBA.NonQuery(trans, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

            Public Shared Sub AddTemplates(ByVal id As Integer, ByVal lst As List(Of Integer), Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "INSERT INTO MOD_ClassTemplate (ClassId, TemplateId) VALUES (@Id, @Template)"
                For Each i As Integer In lst
                    DBA.NonQuery(trans, q, DBA.CreateParameter("Id", SqlDbType.Int, id), DBA.CreateParameter("Template", SqlDbType.Int, i))
                Next
            End Sub

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not (HasMaterials(id, conn) OrElse HasSections(id, conn))
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM MOD_Class_Res WHERE Id = @Id"
                Dim p = DBA.CreateParameter("Id", SqlDbType.Int, id)
                DBA.NonQuery(conn, q, p)
                'q = "DELETE FROM MOD_ClassAdmins WHERE ClassId = @Id"
                'DBA.NonQuery(conn, q, p)
                q = "DELETE FROM MOD_SectionMaterialUser WHERE ClassId = @Id"
                DBA.NonQuery(conn, q, p)
                q = "DELETE FROM MOD_ClassTemplate WHERE ClassId = @Id"
                DBA.NonQuery(conn, q, p)
            End Sub

            Private Shared Function HasMaterials(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_Material WHERE ClassId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Private Shared Function HasSections(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_Section WHERE ClassId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
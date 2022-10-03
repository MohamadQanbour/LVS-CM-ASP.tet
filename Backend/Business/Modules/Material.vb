Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Interfaces
Imports EGV.Constants

Namespace EGV
    Namespace Business

        'object
        Public Class Material
            Inherits AudLocBusinessBase
            Implements ILocBusinessClass

#Region "Public Members"

            Public Property Id As Integer = 0 Implements ILocBusinessClass.Id
            Public Property Code As String = String.Empty
            Public Property Title As String = String.Empty Implements ILocBusinessClass.Title
            Public Property ClassId As Integer = 0
            Public Property MaxMark As Integer = 0
            Public Property ExamTemplateId As Integer = 0

            Public Property Items As List(Of MaterialExamTemplateItem)

#End Region

#Region "Filler"

            Public Overrides Sub FillObject(dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    Code = Safe(dr("Code"))
                    Title = Safe(dr("Title"))
                    ClassId = Safe(dr("ClassId"), ValueTypes.TypeInteger)
                    MaxMark = Safe(dr("MaxMark"), ValueTypes.TypeInteger)
                    ExamTemplateId = Safe(dr("ExamTemplateId"), ValueTypes.TypeInteger)
                    Items = MaterialExamTemplateItemController.GetMaterialItems(Id, ExamTemplateId, MyConn, MyLanguageId)
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Contructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing, Optional ByVal langId As Integer = 0)
                MyBase.New(conn, langId)
                Items = New List(Of MaterialExamTemplateItem)()
                If tid > 0 Then
                    FillObject(DBA.SPDataRow(MyConn, "MOD_Material_Get", DBA.CreateParameter("Id", SqlDbType.Int, tid), DBA.CreateParameter("LanguageId", SqlDbType.Int, MyLanguageId)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                MaterialController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Material_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("Code", SqlDbType.NVarChar, Code, 50),
                                  DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                  DBA.CreateParameter("ClassId", SqlDbType.Int, ClassId),
                                  DBA.CreateParameter("MaxMark", SqlDbType.Int, MaxMark),
                                  DBA.CreateParameter("ExamTemplateId", SqlDbType.Int, ExamTemplateId),
                                  DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id)
                                  )
                InsertRes(trans)
                MaterialExamTemplateItemController.AddMaterialItems(Id, Items, MyConn, trans)
            End Sub

            Public Overrides Sub InsertRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Material_AddRes"
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
                Dim sp As String = "MOD_Material_Translate"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                               DBA.CreateParameter("UserId", SqlDbType.Int, userId)
                               )
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Material_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Code", SqlDbType.NVarChar, Code, 50),
                               DBA.CreateParameter("ClassId", SqlDbType.Int, ClassId),
                               DBA.CreateParameter("MaxMark", SqlDbType.Int, MaxMark),
                               DBA.CreateParameter("ExamTemplateId", SqlDbType.Int, ExamTemplateId),
                               DBA.CreateParameter("UserId", SqlDbType.Int, Helper.CMSAuthUser.Id),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
                If MyLanguageId = LanguageController.GetDefaultId(MyConn, trans) Then UpdateDefaultRes(trans)
                UpdateRes(trans)
                MaterialExamTemplateItemController.AddMaterialItems(Id, Items, MyConn, trans)
            End Sub

            Public Overrides Sub UpdateDefaultRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Material_UpdateDefaultRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub UpdateRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MOD_Material_UpdateRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LangId", SqlDbType.Int, MyLanguageId)
                               )
            End Sub

#End Region

        End Class

        'controller
        Public Class MaterialController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM MOD_Material WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetTeacherFullMaterials(ByVal conn As SqlConnection, ByVal userId As Integer,
                                                           Optional ByVal langId As Integer = 0) As DBAReturnObject
                Helper.GetSafeLanguageId(langId)
                Dim q As String = "SELECT (M.Code + ' - ' + R.Title) AS MaterialTitle, R.Title, M.Id FROM MOD_Material M INNER JOIN MOD_Material_Res R ON M.Id = R.Id AND R.LanguageId = @LangId INNER JOIN (SELECT MaterialId FROM MOD_SectionMaterialUser WHERE UserId = @UserId GROUP BY MaterialId) AS U ON U.MaterialId = M.Id"
                Dim ret As New DBAReturnObject()
                Dim dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("LangId", SqlDbType.Int, langId), DBA.CreateParameter("UserId", SqlDbType.Int, userId))
                ret.Count = dt.Rows.Count
                ret.Query = q
                ret.List = dt
                Return ret
            End Function

            Public Shared Function GetTeacherMaterials(ByVal conn As SqlConnection, ByVal userId As Integer,
                                                       Optional ByVal langId As Integer = 0,
                                                       Optional ByVal classId As Integer = 0,
                                                       Optional ByVal search As String = "",
                                                       Optional ByVal sectionId As Integer = 0) As DBAReturnObject
                Helper.GetSafeLanguageId(langId)
                Dim ret As New DBAReturnObject()
                Dim p As New List(Of SqlParameter)
                Dim lst As New List(Of String)
                Dim q As String = "SELECT (M.Code + ' - ' + R.Title) AS MaterialTitle, M.Id FROM MOD_Material M INNER JOIN MOD_Material_Res R ON M.Id = R.Id AND R.LanguageId = @LanguageId"
                p.Add(DBA.CreateParameter("LanguageId", SqlDbType.Int, langId))
                If UserTypeRoleController.UserInType(userId, UserTypes.Teacher, conn) Then
                    q &= " INNER JOIN MOD_SectionMaterialUser U ON U.MaterialId = M.Id AND U.UserId = @UserId"
                    p.Add(DBA.CreateParameter("UserId", SqlDbType.Int, userId))
                    If sectionId > 0 Then
                        q &= " AND U.SectionId = @SectionId"
                        p.Add(DBA.CreateParameter("SectionId", SqlDbType.Int, sectionId))
                    End If
                End If
                If classId > 0 Then
                    lst.Add("M.ClassId = @ClassId")
                    p.Add(DBA.CreateParameter("ClassId", SqlDbType.Int, classId))
                End If
                If search <> String.Empty Then
                    lst.Add("(M.Code LIKE '%' + @Search + '%' OR R.Title LIKE '%' + @Search + '%')")
                    p.Add(DBA.CreateParameter("Search", SqlDbType.NVarChar, search, 255))
                End If
                If lst.Count > 0 Then
                    q &= " WHERE " & String.Join(" AND ", lst.ToArray())
                End If
                ret.Query = q
                Dim dt As DataTable = DBA.DataTable(conn, q, p.ToArray())
                ret.Count = dt.Rows.Count
                ret.List = dt
                Return ret
            End Function

            Public Shared Function GetTemplateId(ByVal conn As SqlConnection, ByVal materialId As Integer) As Integer
                Dim q As String = "SELECT ExamTemplateId FROM MOD_Material WHERE Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, materialId))
            End Function

            Public Shared Function GetTitle(ByVal materialId As Integer, Optional ByVal langId As Integer = 0, Optional ByVal conn As SqlConnection = Nothing) As String
                Helper.GetSafeLanguageId(langId)
                Dim q As String = "SELECT Title FROM MOD_Material_Res WHERE LanguageId = @LanguageId AND Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("LanguageId", SqlDbType.Int, langId), DBA.CreateParameter("Id", SqlDbType.Int, materialId))
            End Function

            Public Shared Function GetMaxMark(ByVal materialId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Return DBA.Scalar(conn, "SELECT MaxMark FROM MOD_Material WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, materialId))
            End Function

            Public Shared Function GetTotal(ByVal conn As SqlConnection) As Integer
                Dim q As String = "SELECT COUNT(*) FROM MOD_Material"
                Return DBA.Scalar(conn, q)
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return False
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM MOD_Material_Res WHERE Id = @Id"
                Dim p = DBA.CreateParameter("Id", SqlDbType.Int, id)
                DBA.NonQuery(conn, q, p)
                q = "DELETE FROM MOD_SectionMaterialUser WHERE MaterialId = @Id"
                DBA.NonQuery(conn, q, p)
                MaterialExamTemplateItemController.ClearMaterialItems(id, conn)
            End Sub

#End Region

        End Class

    End Namespace
End Namespace
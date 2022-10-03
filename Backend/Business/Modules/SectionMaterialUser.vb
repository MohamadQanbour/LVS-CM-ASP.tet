Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures

Namespace EGV
    Namespace Business

        'object
        Public Class SectionMaterialUser

            Public Property ClassId As Integer = 0
            Public Property SectionId As Integer = 0
            Public Property MaterialId As Integer = 0
            Public Property UserId As Integer = 0

        End Class

        'controller
        Public Class SectionMaterialUserController

            Public Shared Function GetMaterialUser(ByVal conn As SqlConnection, ByVal sectionId As Integer,
                                                   ByVal materialId As Integer) As Integer
                Dim q As String = "SELECT UserId FROM MOD_SectionMaterialUser WHERE SectionId = @SectionId AND MaterialId = @MaterialId"
                Return DBA.Scalar(conn, q,
                                  DBA.CreateParameter("SectionId", SqlDbType.Int, sectionId),
                                  DBA.CreateParameter("MaterialId", SqlDbType.Int, materialId)
                                  )
            End Function

            Public Shared Sub DeleteSectionMaterials(ByVal conn As SqlConnection, ByVal sectionId As Integer)
                Dim q As String = "DELETE FROM MOD_SectionMaterialUser WHERE SectionId = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, sectionId))
            End Sub

            Public Shared Sub UpdateMaterials(ByVal conn As SqlConnection, ByVal sectionId As Integer, ByVal lst As List(Of MaterialUser))
                Dim q As String = "INSERT INTO MOD_SectionMaterialUser (ClassId, SectionId, MaterialId, UserId) VALUES (@ClassId, @SectionId, @MaterialId, @UserId);"
                DeleteSectionMaterials(conn, sectionId)
                Dim classId As Integer = SectionController.GetSectionClassId(conn, sectionId)
                For Each item As MaterialUser In lst
                    DBA.NonQuery(conn, q,
                                 DBA.CreateParameter("ClassId", SqlDbType.Int, classId),
                                 DBA.CreateParameter("SectionId", SqlDbType.Int, sectionId),
                                 DBA.CreateParameter("MaterialId", SqlDbType.Int, item.MaterialId),
                                 DBA.CreateParameter("UserId", SqlDbType.Int, item.UserId)
                                 )
                Next
            End Sub

            Public Shared Function GetUserMaterials(ByVal conn As SqlConnection, ByVal id As Integer,
                                                    Optional ByVal sectionId As Integer = 0,
                                                    Optional ByVal langId As Integer = 0) As List(Of UserMaterialObject)
                Helper.GetSafeLanguageId(langId)
                Dim lst As New List(Of UserMaterialObject)
                Dim q As String = "SELECT U.ClassId, U.SectionId, U.MaterialId, R.Title FROM MOD_SectionMaterialUser U INNER JOIN MOD_Material_Res R ON U.MaterialId = R.Id AND R.LanguageId = @LanguageId WHERE U.UserId = @Id"
                If sectionId > 0 Then
                    q &= " AND U.SectionId = @SectionId"
                End If
                Using dt As DataTable = DBA.DataTable(conn, q,
                                                      DBA.CreateParameter("LanguageId", SqlDbType.Int, langId),
                                                      DBA.CreateParameter("Id", SqlDbType.Int, id),
                                                      DBA.CreateParameter("SectionId", SqlDbType.Int, sectionId)
                                                      )
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New UserMaterialObject() With {
                            .ClassId = Helper.GetSafeDBValue(dr("ClassId"), ValueTypes.TypeInteger),
                            .MaterialId = Helper.GetSafeDBValue(dr("MaterialId"), ValueTypes.TypeInteger),
                            .SectionId = Helper.GetSafeDBValue(dr("SectionId"), ValueTypes.TypeInteger),
                            .MaterialTitle = Helper.GetSafeDBValue(dr("Title"))
                        })
                    Next
                End Using
                Return lst
            End Function

        End Class

    End Namespace
End Namespace
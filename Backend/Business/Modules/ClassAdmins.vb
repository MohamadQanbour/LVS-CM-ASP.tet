Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures

Namespace EGV
    Namespace Business

        'controller
        Public Class ClassAdminsController

            Public Shared Function Exists(ByVal conn As SqlConnection, ByVal classId As Integer) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_ClassAdmins WHERE ClassId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, classId)) > 0
            End Function

            Public Shared Sub UpdateClassAdmin(ByVal conn As SqlConnection, ByVal classId As Integer, ByVal userId As Integer)
                Dim q As String = "INSERT INTO MOD_ClassAdmins (ClassId, UserId) VALUES (@ClassId, @UserId)"
                DBA.NonQuery(conn, q, DBA.CreateParameter("ClassId", SqlDbType.Int, classId), DBA.CreateParameter("UserId", SqlDbType.Int, userId))
            End Sub

            Public Shared Sub DeleteClassAdmins(ByVal conn As SqlConnection)
                Dim q As String = "DELETE FROM MOD_ClassAdmins"
                DBA.NonQuery(conn, q)
            End Sub

            Public Shared Function GetClassAdmins(ByVal conn As SqlConnection, Optional ByVal langId As Integer = 0) As List(Of ClassUser)
                Helper.GetSafeLanguageId(langId)
                Dim q As String = "SELECT C.Id, C.Title, U.UserId FROM MOD_Class_Res C LEFT JOIN MOD_ClassAdmins U ON C.Id = U.ClassId WHERE C.LanguageId = @LanguageId"
                Dim lst As New List(Of ClassUser)
                Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("LanguageId", SqlDbType.Int, langId))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New ClassUser() With {
                            .ClassId = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                            .UserId = Helper.GetSafeDBValue(dr("UserId"), ValueTypes.TypeInteger),
                            .ClassTitle = Helper.GetSafeDBValue(dr("Title"))
                        })
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function GetSectionAdmin(ByVal conn As SqlConnection, ByVal sectionId As Integer) As Integer
                Dim q As String = "SELECT UserId FROM MOD_ClassAdmins WHERE ClassId = @Id"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, sectionId)), ValueTypes.TypeInteger)
            End Function

        End Class

    End Namespace
End Namespace

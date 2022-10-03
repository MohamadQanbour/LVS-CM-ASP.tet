Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV

Partial Class test
    Inherits System.Web.UI.Page

    Protected Sub page_load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim myconn As SqlConnection = DBA.GetConn()
        Try
            myconn.Open()
            Dim q As String = "SELECT * FROM MOD_ExamTemplateItem_Res WHERE LanguageId = 1"
            Using dt = DBA.DataTable(myconn, q)
                For Each dr In dt.Rows
                    Dim title As String = Utils.Helper.GetSafeDBValue(dr("Title"))
                    Dim id As Integer = Utils.Helper.GetSafeDBValue(dr("Id"), Enums.ValueTypes.TypeInteger)
                    q = "UPDATE MOD_ExamTemplateItem_Res SET Title = @Title WHERE Id = " & id & " AND LanguageId = 2"
                    DBA.NonQuery(myconn, q, DBA.CreateParameter("Title", SqlDbType.NVarChar, title, 255))
                Next
            End Using
        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            myconn.Close()
        End Try
    End Sub

    Protected Sub lnk_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btn.Click
        Dim q As String = "SELECT M.Id, C.TemplateId FROM MOD_Material M LEFT JOIN (SELECT ClassId, MIN(TemplateId) AS TemplateId FROM MOD_ClassTemplate GROUP BY ClassId) C ON M.ClassId = C.ClassId"
        Dim updateQuery As String = "UPDATE MOD_Material SET ExamTemplateId = @TemplateId WHERE Id = @Id"
        Dim myConn As SqlConnection = DBA.GetConn()
        Try
            myConn.Open()
            Using dt As DataTable = DBA.DataTable(myConn, q)
                For Each dr As DataRow In dt.Rows
                    Dim id As Integer = Helper.GetSafeDBValue(dr("Id"), EGV.Enums.ValueTypes.TypeInteger)
                    Dim templateId As Integer = Helper.GetSafeDBValue(dr("TemplateId"), EGV.Enums.ValueTypes.TypeInteger)
                    DBA.NonQuery(myConn, updateQuery, DBA.CreateParameter("TemplateId", SqlDbType.Int, templateId), DBA.CreateParameter("Id", SqlDbType.Int, id))
                Next
            End Using
            lit.Text = "Completed"
        Catch ex As Exception
            lit.Text = ex.Message
        Finally
            myConn.Close()
        End Try
    End Sub

End Class

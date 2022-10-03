Imports EGV
Imports EGV.Business
Imports EGV.Utils

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'Response.Redirect(Path.MapCMSFile("Default.aspx"))
        Dim conn = DBA.GetConn()
        Try
            conn.Open()
            Dim ret = SectionController.GetCollection(conn, 1, 0, "أول", True, 0, 1)
            Response.Write(ret.Query)
        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

End Class


Partial Class index
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim conn = EGV.DBA.GetConn()
        Try
            conn.Open()
            'Response.Write(EGV.Business.FamilyController.GetIdByName("هشام وفاطمه بزي", conn))

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

End Class

Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data
Imports EGV
Partial Class dbtest
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub

    Protected Sub btn2_click(ByVal sender As Object, ByVal e As EventArgs) Handles btn2.Click
        If txt.Text <> String.Empty Then
            Dim connStr As String = "data source=192.168.77.4\SQLEXPRESS;database=LVS;uid=LVSUsr;password=P@ssw0rd@LVS2016;"
            Dim conn As New SqlConnection(connStr)
            Try
                conn.Open()
                Response.Write(SqlHelper.ExecuteScalar(conn, Data.CommandType.Text, txt.Text))
            Catch ex As Exception
                Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try
        End If
    End Sub

    Protected Sub btn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btn.Click
        If txt.Text <> String.Empty Then
            Dim connStr As String = "data source=192.168.77.4\SQLEXPRESS;database=LVS;uid=LVSUsr;password=P@ssw0rd@LVS2016;"
            Dim conn As New SqlConnection(connStr)
            Try
                conn.Open()
                SqlHelper.ExecuteNonQuery(conn, Data.CommandType.Text, txt.Text)
            Catch ex As Exception
                Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try
        End If
    End Sub

End Class

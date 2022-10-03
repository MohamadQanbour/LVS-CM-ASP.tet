Imports EGV
Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_super_sql
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            If Not Page.IsPostBack Then
                ProcessPermissions(AuthUser, PageId, MyConn)
                egvBox.Visible = False
                egvResult.Visible = False
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkExecute_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkExecute.Click
        If Page.IsValid Then
            Try
                MyConn.Open()
                Dim query As String = txtQuery.Text
                Dim queryType As QueryTypes = rblType.SelectedValue
                Select Case queryType
                    Case QueryTypes.NonQuery
                        ExecuteNonQuery(MyConn, query)
                    Case QueryTypes.DataTable
                        ExecuteDataTable(MyConn, query)
                    Case QueryTypes.Scalar
                        ExecuteScalar(MyConn, query)
                    Case QueryTypes.Insert
                        ExecuteInsert(MyConn, query)
                    Case QueryTypes.Update
                        ExecuteUpdate(MyConn, query)
                End Select
                Master.Notifier.Success(Localization.GetResource("Resources.Local.Success"))
            Catch ex As Exception
                ExceptionHandler.ProcessUnrecordedException(ex, Master.Notifier)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub lnkReset_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkResetDB.Click
        Try
            MyConn.Open()
            Dim trans As SqlTransaction = MyConn.BeginTransaction()
            Dim sp As String = "ResetDatabase"
            Try
                DBA.SPNonQuery(trans, sp)
                trans.Commit()
            Catch ex As Exception
                trans.Rollback()
                Throw ex
            End Try
            Try
                Dim dir As String = "~" & Path.MapCMSAsset("attachments")
                Dim sDir As String = Server.MapPath(dir)
                If IO.Directory.Exists(sDir) Then
                    For Each d As String In IO.Directory.GetDirectories(sDir)
                        IO.Directory.Delete(d, True)
                    Next
                    For Each f As String In IO.Directory.GetFiles(sDir)
                        If Not f.Contains("en-us.jpg") AndAlso Not f.Contains("ar-sy.jpg") Then IO.File.Delete(f)
                    Next
                End If
                dir = "~" & Path.MapCMSAsset("profile-images")
                sDir = Server.MapPath(dir)
                If IO.Directory.Exists(sDir) Then
                    For Each d As String In IO.Directory.GetDirectories(sDir)
                        IO.Directory.Delete(d, True)
                    Next
                    For Each f As String In IO.Directory.GetFiles(sDir)
                        If Not f.Contains("371764_516757475_510961172_n.jpg") Then IO.File.Delete(f)
                    Next
                End If
                dir = "~" & Path.MapPortalAsset("images")
                sDir = Server.MapPath(dir)
                If IO.Directory.Exists(sDir) Then IO.Directory.Delete(sDir, True)
                IO.Directory.CreateDirectory(sDir)
                dir = "~" & Path.MapPortalAsset("schedule")
                sDir = Server.MapPath(dir)
                If IO.Directory.Exists(sDir) Then IO.Directory.Delete(sDir, True)
                IO.Directory.CreateDirectory(sDir)
            Catch ex As Exception
                ExceptionHandler.ProcessUnrecordedException(ex, Master.Notifier)
            End Try
            Master.Notifier.Success(Localization.GetResource("Resources.Local.ResetSuccess"))
        Catch ex As Exception
            ExceptionHandler.ProcessUnrecordedException(ex, Master.Notifier)
        Finally
            MyConn.Close()
        End Try
    End Sub

#End Region

#Region "Private Methods"

    Private Sub ExecuteNonQuery(ByVal conn As SqlConnection, ByVal query As String)
        DBA.NonQuery(conn, query)
    End Sub

    Private Sub ExecuteDataTable(ByVal conn As SqlConnection, ByVal query As String)
        Dim dt As DataTable = DBA.DataTable(conn, query)
        egvGrid.DataSource = dt
        egvGrid.DataBind()
        egvBox.Visible = True
        egvResult.Visible = False
    End Sub

    Private Sub ExecuteScalar(ByVal conn As SqlConnection, ByVal query As String)
        Dim val = DBA.Scalar(conn, query)
        txtResult.Text = val.ToString()
        egvResult.Visible = True
        egvBox.Visible = False
    End Sub

    Private Sub ExecuteInsert(ByVal conn As SqlConnection, ByVal query As String)
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            Dim ret = DBA.ScalarID(trans, query)
            trans.Commit()
            txtResult.Text = ret.ToString()
            egvResult.Visible = True
            egvBox.Visible = False
        Catch ex As Exception
            trans.Rollback()
            Throw ex
        End Try
    End Sub

    Private Sub ExecuteUpdate(ByVal conn As SqlConnection, ByVal query As String)
        Dim trans As SqlTransaction = conn.BeginTransaction()
        Try
            DBA.NonQuery(trans, query)
            trans.Commit()
        Catch ex As Exception
            trans.Rollback()
            Throw ex
        End Try
    End Sub

#End Region

End Class

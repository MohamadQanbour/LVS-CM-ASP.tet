Imports System.Data
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business

Partial Class cms_membership_import_excel
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            If Not Page.IsPostBack Then
                ProcessCMD(Master.Notifier)
                ProcessPermissions(AuthUser, 38, MyConn)
                Master.LoadTitles(GetLocalResourceObject("Page.AddTitle"), "", GetLocalResourceObject("Page.BCAddTitle"), 38)
                EGVScriptManager.AddScript(Path.MapCMSScript("local/import-excel"), False, "1.3")
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkStartUpload.Click
        If fileExcel.HasFile Then
            Dim file = fileExcel.File
            If file.ContentLength > 0 Then
                Dim assetsPath As String = Helper.AssetsPath()
                Dim ext As String = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1)
                Dim filename As String = Guid.NewGuid().ToString()
                Dim path As String = "/" & assetsPath & "/imported-excel/" & filename & "." & ext
                Dim rPath As String = Server.MapPath("~" & path)
                If Not IO.Directory.Exists(rPath.Replace(filename & "." & ext, "")) Then
                    IO.Directory.CreateDirectory(rPath.Replace(filename & "." & ext, ""))
                End If
                Dim ret As String = String.Empty
                Try
                    file.SaveAs(rPath)
                    ret = path
                Catch ex As Exception
                    Throw ex
                End Try
                hdnSelectedFile.Value = ret
                egvIF.Visible = False
                bxImporting.Visible = True
                litFile.Text = ret
            End If
        End If
    End Sub

#End Region

End Class

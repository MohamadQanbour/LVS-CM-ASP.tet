﻿Imports System.Data.SqlClient
Imports U = EGV.Utils
Imports B = EGV.Business
Imports EGV.Business

Partial Class cms_lookup_languages
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ProcessCMD(Master.Notifier)
            Try
                MyConn.Open()
                ProcessPermissions(AuthUser, PageId, MyConn)
                egvGrid.BindGrid(MyConn)
            Catch ex As Exception
                U.ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub grid_DataSource(ByVal sender As Object, ByVal e As EventArgs) Handles egvGrid.GridNeedDataSource
        Try
            MyConn.Open()
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            U.ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub grid_ToolbarClick(ByVal cmd As String) Handles egvGrid.ToolbarButtonClick
        Try
            MyConn.Open()
            Dim ids = egvGrid.GetSelectedIds()
            Select Case cmd.ToLower()
                Case "default"
                    If ids.Count = 1 Then
                        Dim id As Integer = ids(0)
                        B.LanguageController.MakeDefault(id, MyConn)
                    End If
                    Master.Notifier.Success(U.Localization.GetResource("Resources.Local.MakeDefault"))
                Case "activate"
                    For Each id As Integer In ids
                        B.LanguageController.ToggleState(id, True, MyConn)
                    Next
                    Master.Notifier.Success(U.Localization.GetResource("Resources.Local.Activate"))
                Case "deactivate"
                    For Each id As Integer In ids
                        B.LanguageController.ToggleState(id, False, MyConn)
                    Next
                    Master.Notifier.Success(U.Localization.GetResource("Resources.Local.Deactivate"))
                Case "delete"
                    Dim completed As Boolean = True
                    For Each id As Integer In ids
                        completed = completed And B.LanguageController.Delete(id, MyConn)
                    Next
                    If completed Then Master.Notifier.Success(U.Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(U.Localization.GetResource("Resources.Local.delete.Warning"))
            End Select
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            U.ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub grid_DeleteRow(ByVal id As Integer) Handles egvGrid.RowDelete
        Try
            MyConn.Open()
            Dim completed As Boolean = B.LanguageController.Delete(id, MyConn)
            If completed Then Master.Notifier.Success(U.Localization.GetResource("Resources.Local.delete.Success")) Else Master.Notifier.Warning(U.Localization.GetResource("Resources.Local.delete.Warning"))
            egvGrid.BindGrid(MyConn)
        Catch ex As Exception
            U.ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

#End Region

#Region "Public Methods"

    Public Overrides Sub ProcessPermissions(usr As User, Optional ByVal pid As Integer = 0, Optional conn As SqlConnection = Nothing)
        If pid = 0 Then pid = PageId
        MyBase.ProcessPermissions(usr, pid, conn)
        Dim obj As New CMSMenu(pid, conn)
        Dim canPublish As Boolean = usr.CanPublish(obj.PermissionId, conn)
        Dim canDelete As Boolean = usr.CanDelete(obj.PermissionId, conn)
        Dim canModify As Boolean = usr.CanModify(obj.PermissionId, conn)
        Dim canWrite As Boolean = usr.CanWrite(obj.PermissionId, conn)
        If Not canPublish Then
            egvGrid.HideToolbarButton("btnActivate")
            egvGrid.HideToolbarButton("btnDeactivate")
            egvGrid.HideToolbarButton("btnMakeDefault")
        End If
        If Not canDelete Then
            egvGrid.HideToolbarButton("btnDelete")
        End If
        hypAdd.Visible = canWrite
    End Sub

#End Region

End Class
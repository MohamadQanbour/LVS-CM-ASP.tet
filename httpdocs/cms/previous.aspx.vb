Imports EGV
Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Structures

Partial Class cms_previous
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        Try
            MyConn.Open()
            ddlSeason.BindToDataSource(SeasonController.GetCollection(MyConn, LanguageId).List, "Title", "Id")
            ddlData.Items.Add(New ListItem(Localization.GetResource("Resources.Local.Sections"), 1))
            ddlData.Items.Add(New ListItem(Localization.GetResource("Resources.Local.SectionAdmins"), 2))
            ddlData.Items.Add(New ListItem(Localization.GetResource("Resources.Local.MaterialTeachers"), 3))
            ddlData.Items.Add(New ListItem(Localization.GetResource("Resources.Local.Attendance"), 4))
            ddlData.Items.Add(New ListItem(Localization.GetResource("Resources.Local.Exams"), 5))
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Try
                MyConn.Open()
                BindData(MyConn)
                Master.LoadTitles(Localization.GetResource("Resources.Local.PageTitle"), "", Localization.GetResource("Resources.Local.PageBCEditTitle"))
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub btnLoad_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLoad.Click
        If Page.IsValid Then
            Try
                MyConn.Open()
                BindData(MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Protected Sub grid_DataSource(ByVal sender As Object, ByVal e As EventArgs)
        Try
            MyConn.Open()
            BindData(MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

#End Region

#Region "Private Methods"

    Private Sub BindData(ByVal conn As SqlConnection)
        egvSectionsGrid.Visible = False
        egvSectionAdminsGrid.Visible = False
        egvSectionMaterialUser.Visible = False
        egvStudentAttendance.Visible = False
        egvExam.Visible = False
        Select Case ddlData.SelectedValue
            Case "1"
                egvSectionsGrid.AddCondition("S.SeasonId = " & ddlSeason.SelectedValue)
                egvSectionsGrid.BindGrid(conn)
                egvSectionsGrid.Visible = True
            Case "2"
                egvSectionAdminsGrid.AddCondition("S.SeasonId = " & ddlSeason.SelectedValue)
                egvSectionAdminsGrid.BindGrid(conn)
                egvSectionAdminsGrid.Visible = True
            Case "3"
                egvSectionMaterialUser.AddCondition("S.SeasonId = " & ddlSeason.SelectedValue)
                egvSectionMaterialUser.BindGrid(conn)
                egvSectionMaterialUser.Visible = True
            Case "4"
                egvStudentAttendance.AddCondition("SEC.SeasonId = " & ddlSeason.SelectedValue)
                egvStudentAttendance.BindGrid(conn)
                egvStudentAttendance.Visible = True
            Case "5"
                egvExam.AddCondition("E.SeasonId = " & ddlSeason.SelectedValue)
                egvExam.BindGrid(conn)
                egvExam.Visible = True
        End Select
    End Sub

#End Region

End Class

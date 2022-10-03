Imports EGV
Imports System.Data.SqlClient
Imports System.Data
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums

Partial Class cms_setup_member_contacts
    Inherits AuthCMSPageBase

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            MyConn.Open()
            egvSaveCancel.IsEditing = True
            egvSaveCancel.BackPagePath = "../super/default.aspx"
            egvSaveCancel.AddPagePath = "member-contacts.aspx"
            egvSaveCancel.EditPagePath = "member-contacts.aspx?id={0}"
            If Not Page.IsPostBack Then
                ProcessPermissions(AuthUser, PageId, MyConn)
                EGVScriptManager.AddScript(Path.MapCMSScript("local/member-contacts"))
                Dim dt As DataTable = RoleController.List(MyConn).List
                rptFamilies.DataSource = dt
                rptFamilies.DataBind()
                rptStudents.DataSource = dt
                rptStudents.DataBind()
                Dim familes As List(Of MemberContact) = MemberContactController.GetMemberContacts(MyConn, MembershipTypes.Family)
                Dim students As List(Of MemberContact) = MemberContactController.GetMemberContacts(MyConn, MembershipTypes.Student)
                hdnFamilies.Value = Helper.JSSerialize(familes)
                hdnStudents.Value = Helper.JSSerialize(students)
            End If
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs, ByRef hasError As Boolean) Handles egvSaveCancel.SaveClick
        If Page.IsValid Then
            Try
                MyConn.Open()
                SaveData(MyConn)
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub SaveData(ByVal conn As SqlConnection)
        Dim families As List(Of MemberContact) = Helper.JSDeserialize(Of List(Of MemberContact))(hdnFamilies.Value)
        Dim students As List(Of MemberContact) = Helper.JSDeserialize(Of List(Of MemberContact))(hdnStudents.Value)
        MemberContactController.DeleteMemberContacts(conn, MembershipTypes.Family)
        MemberContactController.AddMemberContacts(conn, MembershipTypes.Family, families)
        MemberContactController.DeleteMemberContacts(conn, MembershipTypes.Student)
        MemberContactController.AddMemberContacts(conn, MembershipTypes.Student, students)
    End Sub

#End Region

End Class

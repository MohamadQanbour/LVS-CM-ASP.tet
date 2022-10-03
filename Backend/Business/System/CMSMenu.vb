Imports System.Data
Imports System.Data.SqlClient

Namespace EGV
    Namespace Business

        'object
        Public Class CMSMenu
            Inherits BusinessBase

#Region "Public Properties"

            Public Property Id As Integer
            Public Property Title As String
            Public Property IconClass As String
            Public Property ParentId As Integer
            Public Property PagePath As String
            Public Property PageTitle As String
            Public Property PageDescription As String
            Public Property PermissionId As Integer
            Public Property IsSuper As Boolean
            Public Property Order As Integer
            Public Property IsPublished As Boolean

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), Enums.ValueTypes.TypeInteger)
                    Title = Safe(dr("Title"), Enums.ValueTypes.TypeString)
                    IconClass = Safe(dr("IconClass"), Enums.ValueTypes.TypeString)
                    ParentId = Safe(dr("ParentId"), Enums.ValueTypes.TypeInteger)
                    PagePath = Safe(dr("PagePath"), Enums.ValueTypes.TypeString)
                    PageTitle = Safe(dr("PageTitle"), Enums.ValueTypes.TypeString)
                    PageDescription = Safe(dr("PageDescription"), Enums.ValueTypes.TypeString)
                    PermissionId = Safe(dr("PermissionId"), Enums.ValueTypes.TypeInteger)
                    IsSuper = Safe(dr("IsSuper"), Enums.ValueTypes.TypeBoolean)
                    Order = Safe(dr("Order"), Enums.ValueTypes.TypeInteger)
                    IsPublished = Safe(dr("IsPublished"), Enums.ValueTypes.TypeBoolean)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    FillObject(DBA.DataRow(MyConn, "SELECT * FROM SYS_Menu WHERE Id = @Id", DBA.CreateParameter("@Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_Menu_Add"
                Id = DBA.SPScalar(trans, sp,
                    DBA.CreateParameter("Title", SqlDbType.VarChar, Title, 50),
                    DBA.CreateParameter("IconClass", SqlDbType.VarChar, IconClass, 50),
                    DBA.CreateParameter("ParentId", SqlDbType.Int, ParentId),
                    DBA.CreateParameter("PagePath", SqlDbType.VarChar, PagePath, 255),
                    DBA.CreateParameter("PageTitle", SqlDbType.VarChar, PageTitle, 50),
                    DBA.CreateParameter("PageDescription", SqlDbType.VarChar, PageDescription, 255),
                    DBA.CreateParameter("PermissionId", SqlDbType.Int, PermissionId),
                    DBA.CreateParameter("IsSuper", SqlDbType.Bit, IsSuper),
                    DBA.CreateParameter("Order", SqlDbType.Int, Order),
                    DBA.CreateParameter("IsPublished", SqlDbType.Bit, IsPublished)
                )
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_Menu_Update"
                DBA.SPNonQuery(trans, sp,
                    DBA.CreateParameter("Title", SqlDbType.VarChar, Title, 50),
                    DBA.CreateParameter("IconClass", SqlDbType.VarChar, IconClass, 50),
                    DBA.CreateParameter("ParentId", SqlDbType.Int, ParentId),
                    DBA.CreateParameter("PagePath", SqlDbType.VarChar, PagePath, 255),
                    DBA.CreateParameter("PageTitle", SqlDbType.VarChar, PageTitle, 50),
                    DBA.CreateParameter("PageDescription", SqlDbType.VarChar, PageDescription, 255),
                    DBA.CreateParameter("PermissionId", SqlDbType.Int, PermissionId),
                    DBA.CreateParameter("IsSuper", SqlDbType.Bit, IsSuper),
                    DBA.CreateParameter("Order", SqlDbType.Int, Order),
                    DBA.CreateParameter("IsPublished", SqlDbType.Bit, IsPublished),
                    DBA.CreateParameter("Id", SqlDbType.Int, Id)
                )
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                CMSMenuController.Delete(Id, conn)
            End Sub

#End Region

        End Class

        'Controller
        Public Class CMSMenuController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If Not HasSubMenus(id, conn) Then
                    Dim q As String = "DELETE FROM SYS_Menu WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function HasSubMenus(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM SYS_Menu WHERE ParentId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id)) > 0
            End Function

            Public Shared Function List(ByVal conn As SqlConnection, ByVal ParamArray conditions() As String) As Structures.DBAReturnObject
                Dim q As String = "SELECT * FROM SYS_Menu AS M"
                conditions = (From cond In conditions Where cond <> String.Empty).ToArray()
                If conditions.Length > 0 Then q &= " WHERE " & String.Join(" AND ", conditions)
                q &= " ORDER BY M.[Order] ASC"
                Dim ret As New Structures.DBAReturnObject()
                Dim dt As DataTable = DBA.DataTable(conn, q)
                ret.Query = q
                ret.Count = dt.Rows.Count
                ret.List = dt
                Return ret
            End Function

            Public Shared Function ListParents(ByVal conn As SqlConnection, ByVal ParamArray conditions() As String) As Structures.DBAReturnObject
                Dim sp As String = "SYS_Menu_GetParents"
                Dim ret As New Structures.DBAReturnObject()
                Dim dt As DataTable = DBA.SPDataTable(conn, sp,
                                                      DBA.CreateParameter("UserId", SqlDbType.Int, Utils.Helper.CMSAuthUser.Id),
                                                      DBA.CreateParameter("Super", SqlDbType.Bit, Utils.Helper.CMSAuthUser.IsSuperAdmin)
                                                      )
                ret.Query = sp
                ret.Count = dt.Rows.Count()
                ret.List = dt
                Return ret
            End Function

            Public Shared Function ListSubs(ByVal id As Integer, ByVal conn As SqlConnection, ByVal ParamArray conditions() As String) As Structures.DBAReturnObject
                Dim sp As String = "SYS_Menu_GetSubs"
                Dim ret As New Structures.DBAReturnObject()
                Dim dt As DataTable = DBA.SPDataTable(conn, sp,
                                                      DBA.CreateParameter("UserId", SqlDbType.Int, Utils.Helper.CMSAuthUser.Id),
                                                      DBA.CreateParameter("ParentId", SqlDbType.Int, id),
                                                      DBA.CreateParameter("Super", SqlDbType.Bit, Utils.Helper.CMSAuthUser.IsSuperAdmin)
                                                      )
                ret.Query = sp
                ret.Count = dt.Rows.Count
                ret.List = dt
                Return ret
            End Function

            Public Shared Function GetId(ByVal pagePath As String, Optional ByVal conn As SqlConnection = Nothing) As Integer
                Dim q As String = "SELECT Id FROM SYS_Menu WHERE PagePath = @Path"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("@Path", SqlDbType.VarChar, pagePath, 255))
            End Function

            Public Shared Function HasChildren(ByVal pageId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM SYS_Menu WHERE ParentId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("@Id", SqlDbType.Int, pageId)) > 0
            End Function

            Public Shared Sub ToggleState(ByVal menuId As Integer, Optional ByVal published As Boolean = True, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE SYS_Menu SET IsPublished = @State WHERE Id = @Id"
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("State", SqlDbType.Bit, published),
                             DBA.CreateParameter("Id", SqlDbType.Int, menuId)
                             )
            End Sub

#End Region

        End Class

    End Namespace
End Namespace
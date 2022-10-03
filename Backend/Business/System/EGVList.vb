Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums

Namespace EGV
    Namespace Business

        'object
        Public Class EGVList
            Inherits BusinessBase

#Region "Public Properties"

            Public Property Id As Integer
            Public Property Name As String
            Public Property IsPublished As Boolean

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    Name = Safe(dr("Name"), ValueTypes.TypeString)
                    IsPublished = Safe(dr("IsPublished"), ValueTypes.TypeBoolean)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    Dim q As String = "SELECT * FROM SYS_List WHERE Id = @Id"
                    FillObject(DBA.DataRow(MyConn, q, DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                EGVListController.Delete(Id, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_List_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("Name", SqlDbType.NVarChar, Name, 50),
                                  DBA.CreateParameter("IsPublished", SqlDbType.Bit, IsPublished)
                                  )
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_List_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Name", SqlDbType.NVarChar, Name, 50),
                               DBA.CreateParameter("IsPublished", SqlDbType.Bit, IsPublished),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

#End Region

        End Class

        Public Class EGVListItem
            Inherits BusinessBase

#Region "Public Properties"

            Public Property Id As Integer
            Public Property ListId As Integer
            Public Property ItemText As String
            Public Property ItemValue As String
            Public Property IsPublished As Boolean

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    ListId = Safe(dr("ListId"), ValueTypes.TypeInteger)
                    ItemText = Safe(dr("ItemText"), ValueTypes.TypeString)
                    ItemValue = Safe(dr("ItemValue"), ValueTypes.TypeString)
                    IsPublished = Safe(dr("IsPublished"), ValueTypes.TypeBoolean)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    Dim q As String = "SELECT * FROM SYS_ListItem WHERE Id = @Id"
                    FillObject(DBA.DataRow(MyConn, q, DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                EGVListItemController.Delete(Id, ListId, ItemValue, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_ListItem_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("ListId", SqlDbType.Int, ListId),
                                  DBA.CreateParameter("ItemText", SqlDbType.NVarChar, ItemText, 50),
                                  DBA.CreateParameter("ItemValue", SqlDbType.NVarChar, ItemValue, 255),
                                  DBA.CreateParameter("IsPublished", SqlDbType.Bit, IsPublished)
                                  )
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "SYS_ListItem_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("ListId", SqlDbType.Int, ListId),
                               DBA.CreateParameter("ItemText", SqlDbType.NVarChar, ItemText, 50),
                               DBA.CreateParameter("ItemValue", SqlDbType.NVarChar, ItemValue, 255),
                               DBA.CreateParameter("IsPublished", SqlDbType.Bit, IsPublished),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

#End Region

        End Class

        'controller
        Public Class EGVListController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM SYS_List WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Sub ToggleState(ByVal id As Integer, Optional ByVal publish As Boolean = True, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE SYS_List SET IsPublished = @Publish WHERE Id = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Publish", SqlDbType.Bit, publish),
                             DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

            Public Shared Function GetListItems(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As DataTable
                Dim q As String = "SELECT ItemText, ItemValue FROM SYS_ListItem WHERE ListId = @Id"
                Return DBA.DataTable(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Public Shared Function List(Optional ByVal conn As SqlConnection = Nothing) As Structures.DBAReturnObject
                Dim ret As New Structures.DBAReturnObject()
                Dim q As String = "SELECT * FROM SYS_List"
                Dim dt = DBA.DataTable(conn, q)
                ret.Query = q
                ret.Count = dt.Rows.Count()
                ret.List = dt
                Return ret
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not HasSettings(id, conn)
            End Function

            Private Shared Function HasSettings(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM SYS_Setting WHERE SettingListSourceId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM SYS_ListItem WHERE Id = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

#End Region

        End Class

        Public Class EGVListItemController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, ByVal listid As Integer, ByVal value As String, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(listid, value, conn) Then
                    Dim q As String = "DELETE FROM SYS_ListItem WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Sub ToggleState(ByVal id As Integer, Optional ByVal publish As Boolean = True, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE SYS_ListItem SET IsPublished = @Publish WHERE Id = @Id"
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("Publish", SqlDbType.Bit, publish),
                             DBA.CreateParameter("Id", SqlDbType.Int, id)
                             )
            End Sub

            Public Shared Function GetListValue(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim q As String = "SELECT ItemValue FROM SYS_ListItem WHERE Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal listid As Integer, ByVal value As String, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not HasSetting(listid, value, conn)
            End Function

            Private Shared Function HasSetting(ByVal listid As Integer, ByVal value As String, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM SYS_Setting WHERE SettingListSourceId = @ListId AND SettingValue = @Value"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("ListId", SqlDbType.Int, listid), DBA.CreateParameter("Value", SqlDbType.NVarChar, value, 255))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
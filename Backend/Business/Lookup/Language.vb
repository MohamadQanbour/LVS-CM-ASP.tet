Imports System.Data.SqlClient
Imports EGV.Enums
Imports EGV.Interfaces
Imports EGV.Utils

Namespace EGV
    Namespace Business

        'Object
        Public Class Language
            Inherits LocBusinessBase
            Implements ILocBusinessClass

#Region "Public Properties"

            Public Property Id As Integer Implements ILocBusinessClass.Id
            Public Property Title As String Implements ILocBusinessClass.Title
            Public Property LanguageCode As String
            Public Property IsActive As Boolean
            Public Property IsDefault As Boolean
            Public Property IsRTL As Boolean
            Public Property ImageUrl As String

#End Region

#Region "Filler"

            Public Overrides Sub FillObject(dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    Title = Safe(dr("Title"), ValueTypes.TypeString)
                    LanguageCode = Safe(dr("LanguageCode"), ValueTypes.TypeString)
                    IsActive = Safe(dr("IsActive"), ValueTypes.TypeBoolean)
                    IsDefault = Safe(dr("IsDefault"), ValueTypes.TypeBoolean)
                    IsRTL = Safe(dr("IsRTL"), ValueTypes.TypeBoolean)
                    ImageUrl = Safe(dr("ImageURL"), ValueTypes.TypeString)
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal langId As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn, langId)
                If tid > 0 Then
                    Dim sp As String = "LOK_Language_Get"
                    FillObject(DBA.SPDataRow(MyConn, sp,
                                             DBA.CreateParameter("Id", SqlDbType.Int, tid),
                                             DBA.CreateParameter("LangId", SqlDbType.Int, MyLanguageId)))
                End If
            End Sub

#End Region

#Region "Public Methods"


            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "LOK_Language_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                  DBA.CreateParameter("LanguageCode", SqlDbType.Char, LanguageCode, 5),
                                  DBA.CreateParameter("IsActive", SqlDbType.Bit, IsActive),
                                  DBA.CreateParameter("IsDefault", SqlDbType.Bit, IsDefault),
                                  DBA.CreateParameter("IsRTL", SqlDbType.Bit, IsRTL),
                                  DBA.CreateParameter("ImageURL", SqlDbType.NVarChar, ImageUrl, 255)
                                  )
                InsertRes(trans)
            End Sub

            Public Overrides Sub InsertRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "LOK_Language_AddRes"
                For Each lid As Integer In LanguageController.GetIds(MyConn, trans)
                    DBA.SPNonQuery(trans, sp,
                                   DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                                   DBA.CreateParameter("Id", SqlDbType.Int, Id),
                                   DBA.CreateParameter("LanguageId", SqlDbType.Int, lid)
                                   )
                Next
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "LOK_Language_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("LanguageCode", SqlDbType.Char, LanguageCode, 5),
                               DBA.CreateParameter("IsActive", SqlDbType.Bit, IsActive),
                               DBA.CreateParameter("IsDefault", SqlDbType.Bit, IsDefault),
                               DBA.CreateParameter("IsRTL", SqlDbType.Bit, IsRTL),
                               DBA.CreateParameter("ImageURL", SqlDbType.NVarChar, ImageUrl, 255),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
                If MyLanguageId = LanguageController.GetDefaultId(MyConn, trans) Then UpdateDefaultRes(trans)
                UpdateRes(trans)
            End Sub

            Public Overrides Sub UpdateDefaultRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "LOK_Language_UpdateDefaultRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub UpdateRes(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "LOK_Language_UpdateRes"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LangId", SqlDbType.Int, MyLanguageId)
                )
            End Sub

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                LanguageController.Delete(Id, conn)
            End Sub
            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Translate(langId As Integer, userId As Integer, Optional trans As SqlTransaction = Nothing) Implements ILocBusinessClass.Translate
                Dim sp As String = "LOK_Language_Translate"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("Title", SqlDbType.NVarChar, Title, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id),
                               DBA.CreateParameter("LangId", SqlDbType.Int, langId),
                               DBA.CreateParameter("UserId", SqlDbType.Int, userId)
                               )
            End Sub

#End Region

        End Class

        'Controller
        Public Class LanguageController

#Region "Public Methods"

            Public Shared Function List(Optional ByVal conn As SqlConnection = Nothing, Optional ByVal langId As Integer = 0,
                                        Optional ByVal onlyActive As Boolean = False) As Structures.DBAReturnObject
                Helper.GetSafeLanguageId(langId)
                Dim ret As New Structures.DBAReturnObject
                Dim q As String = "SELECT R.*, L.LanguageCode, L.IsActive, L.IsDefault, L.IsRTL, L.ImageURL FROM LOK_Language L INNER JOIN LOK_Language_Res R ON L.Id = R.Id AND R.LanguageId = @LanguageId"
                Dim p As New List(Of SqlParameter)
                p.Add(DBA.CreateParameter("LanguageId", SqlDbType.Int, langId))
                If onlyActive Then
                    q &= " WHERE IsActive = @Active"
                    p.Add(DBA.CreateParameter("Active", SqlDbType.Bit, True))
                End If
                Dim dt As DataTable = DBA.DataTable(conn, q, p.ToArray())
                ret.Count = dt.Rows.Count
                ret.List = dt
                ret.Query = q
                Return ret
            End Function

            Public Shared Function GetIds(Optional ByVal conn As SqlConnection = Nothing, Optional ByVal trans As SqlTransaction = Nothing) As List(Of Integer)
                Dim q As String = "SELECT Id FROM LOK_Language"
                Dim lst As New List(Of Integer)
                Dim dt As DataTable
                If trans IsNot Nothing Then dt = DBA.DataTable(trans, q) Else dt = DBA.DataTable(conn, q)
                Using dt
                    For Each dr As DataRow In dt.Rows
                        lst.Add(dr("Id"))
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function GetDefaultId(Optional ByVal conn As SqlConnection = Nothing, Optional ByVal trans As SqlTransaction = Nothing) As Integer
                Dim q As String = "SELECT Id FROM LOK_Language WHERE IsDefault = 1"
                If trans IsNot Nothing Then Return DBA.Scalar(trans, q) Else Return DBA.Scalar(conn, q)
            End Function

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM LOK_Language WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    DeleteRelated(id, conn)
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Sub ToggleState(ByVal id As Integer, Optional ByVal activate As Boolean = True, Optional ByVal conn As SqlConnection = Nothing)
                Dim sp As String = "LOK_Language_ToggleState"
                DBA.SPNonQuery(conn, sp,
                               DBA.CreateParameter("Id", SqlDbType.Int, id),
                               DBA.CreateParameter("Activate", SqlDbType.Bit, activate)
                               )
            End Sub

            Public Shared Sub MakeDefault(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Dim sp As String = "LOK_Language_MakeDefault"
                DBA.SPNonQuery(conn, sp, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

            Public Shared Function GetLanguageCode(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim q As String = "SELECT LanguageCode FROM LOK_Language WHERE Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Return False
            End Function

            Private Shared Sub DeleteRelated(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing)
                'resources
                Dim q As String = "DELETE FROM LOK_Language_Res WHERE Id = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

#End Region

        End Class

    End Namespace
End Namespace
Imports System.Data.SqlClient
Imports System.Data

Namespace EGV
    Namespace Business

        'Object
        Public Class SettingObject
            Inherits PrimeBusinessBase

#Region "Public Properties"

            Public Property SettingKey As String = String.Empty
            Public Property Title As String = String.Empty
            Public Property SettingValue As String = String.Empty
            Public Property SettingType As Enums.ValueTypes = Enums.ValueTypes.TypeInteger
            Public Property SettingSourceListId As Integer = 0

#End Region

        End Class

        'Controller
        Public Class SettingController

            Public Shared Function AddSetting(ByVal setObj As SettingObject, ByVal trans As SqlTransaction) As String
                Dim sp As String = "SYS_Setting_Add"
                With setObj
                    DBA.SPNonQuery(trans, sp,
                                   DBA.CreateParameter("@key", SqlDbType.VarChar, .SettingKey, 50),
                                   DBA.CreateParameter("@Title", SqlDbType.NVarChar, .Title, 50),
                                   DBA.CreateParameter("@Value", SqlDbType.NVarChar, .SettingValue, 255),
                                   DBA.CreateParameter("@Type", SqlDbType.Int, .SettingType),
                                   DBA.CreateParameter("@ListId", SqlDbType.Int, .SettingSourceListId)
                                   )
                    Return .SettingKey
                End With
            End Function

            Public Shared Sub UpdateSetting(ByVal setObj As SettingObject, ByVal trans As SqlTransaction)
                Dim sp As String = "SYS_Setting_Edit"
                With setObj
                    DBA.SPNonQuery(trans, sp,
                                       DBA.CreateParameter("@Key", SqlDbType.VarChar, .SettingKey, 50),
                                       DBA.CreateParameter("@Title", SqlDbType.NVarChar, .Title, 50),
                                       DBA.CreateParameter("@Value", SqlDbType.NVarChar, .SettingValue, 255),
                                       DBA.CreateParameter("@Type", SqlDbType.Int, .SettingType),
                                       DBA.CreateParameter("@ListId", SqlDbType.Int, .SettingSourceListId)
                                    )
                End With
            End Sub

            Public Shared Function ReadSetting(ByVal key As String, Optional ByVal conn As SqlConnection = Nothing) As String
                Dim sp As String = "SYS_Setting_Read"
                Return DBA.SPScalar(conn, sp, DBA.CreateParameter("@Key", SqlDbType.VarChar, key, 50))
            End Function

            Public Shared Sub WriteSetting(ByVal key As String, ByVal value As String, Optional ByVal conn As SqlConnection = Nothing)
                Dim sp As String = "SYS_Setting_Write"
                DBA.SPNonQuery(conn, sp,
                               DBA.CreateParameter("@Key", SqlDbType.NVarChar, key, 50),
                               DBA.CreateParameter("@Value", SqlDbType.NVarChar, value, 255)
                )
            End Sub

            Public Shared Function GetSetting(ByVal key As String, Optional ByVal conn As SqlConnection = Nothing) As SettingObject
                Dim sp As String = "SYS_Setting_Get"
                Dim dr As DataRow = DBA.SPDataRow(conn, sp, DBA.CreateParameter("@Key", SqlDbType.VarChar, key, 50))
                Return New SettingObject() With {
                    .SettingKey = Utils.Helper.GetSafeDBValue(dr("SettingKey")),
                    .Title = Utils.Helper.GetSafeDBValue(dr("Title")),
                    .SettingValue = Utils.Helper.GetSafeDBValue(dr("SettingValue")),
                    .SettingType = Utils.Helper.GetSafeDBValue(dr("SettingType"), Enums.ValueTypes.TypeInteger),
                    .SettingSourceListId = Utils.Helper.GetSafeDBValue(dr("SettingListSourceId"), Enums.ValueTypes.TypeInteger)
                }
            End Function

            Public Shared Sub DeleteSetting(ByVal key As String, Optional ByVal conn As SqlConnection = Nothing)
                Dim sp As String = "SYS_Setting_Delete"
                DBA.SPNonQuery(conn, sp, DBA.CreateParameter("@Key", SqlDbType.VarChar, key, 50))
            End Sub

            Public Shared Sub Purge(Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "TRUNCATE TABLE SYS_Setting"
                DBA.NonQuery(conn, q)
            End Sub

            Public Shared Function List(Optional ByVal conn As SqlConnection = Nothing) As Structures.DBAReturnObject
                Dim ret As New Structures.DBAReturnObject()
                Dim obj As New EntityStructure("system/Setting")
                Dim cq As CustomQuery = obj.GetCustomQuery(conn)
                ret.Count = cq.ExecuteCount()
                ret.List = cq.Execute()
                Return ret
            End Function

        End Class

    End Namespace
End Namespace
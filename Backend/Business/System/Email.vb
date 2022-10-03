Imports System.Data.SqlClient
Imports EGV
Imports EGV.Structures
Imports EGV.Enums
Imports EGV.Utils

Namespace EGV
    Namespace Business

        'object
        Public Class Email
            Inherits PrimeBusinessBase

#Region "Public Members"

            Public Property Id As Integer = 0
            Public Property KeyId As Integer = 0
            Public Property EmailAddress As String = String.Empty
            Public Property DisplayName As String = String.Empty
            Public Property Type As EmailTypes = EmailTypes.To
            Public Property IsActive As Boolean = False

#End Region

        End Class

        'controller
        Public Class EmailController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "DELETE FROM SYS_Email WHERE Id = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                Return True
            End Function

            Public Shared Function Add(ByVal obj As Email, Optional ByVal trans As SqlTransaction = Nothing) As Integer
                Dim q As String = "INSERT INTO SYS_Email (KeyId, EmailAddress, DisplayName, Type, IsActive) VALUES (@KeyId, @EmailAddress, @DisplayName, @Type, @IsActive);"
                Return DBA.ScalarID(trans, q,
                                    DBA.CreateParameter("KeyId", SqlDbType.Int, obj.KeyId),
                                    DBA.CreateParameter("EmailAddress", SqlDbType.NVarChar, obj.EmailAddress, 100),
                                    DBA.CreateParameter("DisplayName", SqlDbType.NVarChar, obj.DisplayName, 50),
                                    DBA.CreateParameter("Type", SqlDbType.Int, obj.Type),
                                    DBA.CreateParameter("IsActive", SqlDbType.Bit, obj.IsActive)
                                    )
            End Function

            Public Shared Sub Update(ByVal obj As Email, Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "UPDATE SYS_Email SET EmailAddress = @Email, DisplayName = @Name, Type = @Type, KeyId = @Key, IsActive = @IsActive WHERE Id = @Id"
                DBA.NonQuery(trans, q,
                             DBA.CreateParameter("Email", SqlDbType.NVarChar, obj.EmailAddress, 100),
                             DBA.CreateParameter("Name", SqlDbType.NVarChar, obj.DisplayName, 50),
                             DBA.CreateParameter("Type", SqlDbType.Int, obj.Type),
                             DBA.CreateParameter("Key", SqlDbType.Int, obj.KeyId),
                             DBA.CreateParameter("IsActive", SqlDbType.Bit, obj.IsActive),
                             DBA.CreateParameter("Id", SqlDbType.Int, obj.Id)
                             )
            End Sub

            Public Shared Function List(ByVal keyId As Integer, Optional ByVal conn As SqlConnection = Nothing) As List(Of Email)
                Dim lst As New List(Of Email)
                Dim q As String = "SELECT * FROM SYS_Email WHERE KeyId = @Id AND IsActive = 1"
                Using dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, keyId))
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New Email() With {
                            .DisplayName = Helper.GetSafeDBValue(dr("DisplayName")),
                            .EmailAddress = Helper.GetSafeDBValue(dr("EmailAddress")),
                            .Id = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                            .KeyId = Helper.GetSafeDBValue(dr("KeyId"), ValueTypes.TypeInteger),
                            .Type = Helper.GetSafeDBValue(dr("Type"), ValueTypes.TypeInteger),
                            .IsActive = Helper.GetSafeDBValue(dr("IsActive"), ValueTypes.TypeInteger)
                        })
                    Next
                End Using
                Return lst
            End Function

            Public Shared Sub ToggleState(ByVal id As Integer, Optional ByVal activate As Boolean = True, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "UPDATE SYS_Email SET IsActive = @Activate WHERE Id = @Id"
                DBA.NonQuery(conn, q, DBA.CreateParameter("Activate", SqlDbType.Bit, activate), DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Sub

#End Region

        End Class

    End Namespace
End Namespace
Imports EGV.Enums
Imports EGV.Utils
Imports System.Data.SqlClient

Namespace EGV
    Namespace Business
        Public Class DeviceTokenController

#Region "Public Methods"

            Public Shared Function MemberTokenExists(ByVal memberId As Integer, ByVal memberType As MembershipTypes,
                                               ByVal token As String, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_DeviceToken WHERE MemberId = @Id AND MemberType = @Type AND DeviceToken = @Token"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, memberId), DBA.CreateParameter("Type", SqlDbType.Int, memberType), DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 255)) > 0
            End Function

            Public Shared Function UpdateToken(ByVal memberId As Integer, ByVal memberType As MembershipTypes,
                                               ByVal token As String, ByVal langId As Integer, ByVal disabled As Boolean,
                                               Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If MemberTokenExists(memberId, memberType, token, conn) Then
                    Dim q As String = "UPDATE MEM_DeviceToken SET LanguageId = @LangId, Disable = @Disable WHERE MemberId = @Id AND MemberType = @Type AND DeviceToken = @Token"
                    DBA.NonQuery(conn, q,
                                 DBA.CreateParameter("LangId", SqlDbType.Int, langId),
                                 DBA.CreateParameter("Disable", SqlDbType.Bit, disabled),
                                 DBA.CreateParameter("Id", SqlDbType.Int, memberId),
                                 DBA.CreateParameter("Type", SqlDbType.Int, memberType),
                                 DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 255)
                                 )
                    Return True
                Else
                    Return RegisterToken(memberId, memberType, token, langId, disabled, conn)
                End If
            End Function

            Public Shared Function TokenExists(ByVal token As String, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_DeviceToken WHERE DeviceToken = @Token"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 255)) > 0
            End Function

            Public Shared Sub CheckDeviceToken(ByVal memberId As Integer, ByVal memberType As MembershipTypes,
                                               ByVal token As String, Optional ByVal conn As SqlConnection = Nothing)
                If memberId > 0 Then
                    Dim q As String = "SELECT COUNT(*) FROM MEM_DeviceToken WHERE MemberId = 0 AND MemberType = 0 AND DeviceToken = @Token"
                    Dim total As Integer = DBA.Scalar(conn, q, DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 255))
                    If total > 0 Then
                        q = "UPDATE MEM_DeviceToken SET MemberId = @Id, MemberType = @Type WHERE MemberId = 0 AND MemberType = 0 AND DeviceToken = @Token"
                        DBA.NonQuery(conn, q,
                                     DBA.CreateParameter("Id", SqlDbType.Int, memberId),
                                     DBA.CreateParameter("Type", SqlDbType.Int, memberType),
                                     DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 255)
                                     )
                    Else
                        q = "DELETE FROM MEM_DeviceToken WHERE MemberId = 0 AND MemberType = 0 AND DeviceToken = @Token"
                        DBA.NonQuery(conn, q, DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 255))
                    End If
                End If
            End Sub

            Public Shared Function RegisterToken(ByVal memberId As Integer, ByVal memberType As MembershipTypes,
                                                 ByVal token As String, ByVal langId As Integer,
                                                 ByVal disable As Boolean, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If Not MemberTokenExists(memberId, memberType, token, conn) Then
                    Dim q As String = "INSERT INTO MEM_DeviceToken (MemberId, MemberType, DeviceToken, LanguageId, Disable) VALUES (@Id, @Type, @Token, @LangId, @Disable)"
                    Dim completed As Boolean = True
                    Dim trans As SqlTransaction = conn.BeginTransaction()
                    Try
                        DBA.NonQuery(trans, q,
                                     DBA.CreateParameter("Id", SqlDbType.Int, memberId),
                                     DBA.CreateParameter("Type", SqlDbType.Int, memberType),
                                     DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 255),
                                     DBA.CreateParameter("LangId", SqlDbType.Int, langId),
                                     DBA.CreateParameter("Disable", SqlDbType.Bit, disable)
                                     )
                        trans.Commit()
                    Catch ex As Exception
                        trans.Rollback()
                        completed = False
                        Throw ex
                    End Try
                    Return completed
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetTokenTotal(Optional ByVal langId As Integer = 0,
                                                 Optional ByVal onlyEnabled As Boolean = False,
                                                 Optional ByVal memberId As Integer = -1,
                                                 Optional ByVal memberType As Integer = -1,
                                                 Optional ByVal conn As SqlConnection = Nothing) As Integer
                Dim q As String = "SELECT COUNT(*) FROM (SELECT DeviceToken FROM MEM_DeviceToken {0}GROUP BY DeviceToken) AS A"
                Dim p As New List(Of SqlParameter)
                Dim lst As New List(Of String)
                If langId > 0 Then
                    lst.Add("LanguageId = @LangId")
                    p.Add(DBA.CreateParameter("LangId", SqlDbType.Int, langId))
                End If
                If onlyEnabled Then
                    lst.Add("Disable = @Disable")
                    p.Add(DBA.CreateParameter("Disable", SqlDbType.Bit, False))
                End If
                If memberId > -1 AndAlso memberType > -1 Then
                    lst.Add("MemberId = @Id")
                    lst.Add("MemberType = @Type")
                    p.Add(DBA.CreateParameter("Id", SqlDbType.Int, memberId))
                    p.Add(DBA.CreateParameter("Type", SqlDbType.Int, memberType))
                End If
                Dim condition As String = String.Empty
                If lst.Count > 0 Then
                    condition = "WHERE " & String.Join(" AND ", lst.ToArray()) & " "
                End If
                q = String.Format(q, condition)
                Return DBA.Scalar(conn, q, p.ToArray())
            End Function

            Public Shared Function GetMemberTokens(ByVal pageIndex As Integer, Optional ByVal memberId As Integer = -1,
                                                   Optional ByVal memberType As MembershipTypes = -1,
                                                   Optional ByVal langId As Integer = 0,
                                                   Optional ByVal onlyEnabled As Boolean = False,
                                                   Optional ByVal conn As SqlConnection = Nothing) As List(Of String)
                Dim q As String = "WITH OrderedTable AS (SELECT DeviceToken, ROW_NUMBER() OVER (ORDER BY DeviceToken ASC) AS RowNumber FROM MEM_DeviceToken{0}) SELECT TOP 2000 DeviceToken FROM OrderedTable WHERE RowNumber > {1} * 2000 GROUP BY DeviceToken"
                Dim lst As New List(Of String)
                Dim p As New List(Of SqlParameter)
                If memberId > -1 AndAlso memberType > -1 Then
                    lst.Add("MemberId = @Id")
                    lst.Add("MemberType = @Type")
                    p.Add(DBA.CreateParameter("Id", SqlDbType.Int, memberId))
                    p.Add(DBA.CreateParameter("Type", SqlDbType.Int, memberType))
                End If
                If langId > 0 Then
                    lst.Add("LanguageId = @LangId")
                    p.Add(DBA.CreateParameter("LangId", SqlDbType.Int, langId))
                End If
                If onlyEnabled Then
                    lst.Add("Disable = @Disable")
                    p.Add(DBA.CreateParameter("Disable", SqlDbType.Bit, False))
                End If
                Dim condition As String = String.Empty
                If lst.Count > 0 Then
                    condition = " WHERE " & String.Join(" AND ", lst.ToArray())
                End If
                q = String.Format(q, condition, pageIndex)
                Dim ret As New List(Of String)
                Using dt As DataTable = DBA.DataTable(conn, q, p.ToArray())
                    For Each dr As DataRow In dt.Rows
                        ret.Add(Helper.GetSafeDBValue(dr("DeviceToken")))
                    Next
                End Using
                Return ret
            End Function

            Public Shared Sub UnregisterToken(ByVal token As String, ByVal memberId As Integer,
                                                   ByVal memberType As MembershipTypes, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM MEM_DeviceToken WHERE MemberId = @MID AND MemberType = @Type AND DeviceToken = @Token"
                DBA.NonQuery(conn, q, DBA.CreateParameter("MID", SqlDbType.Int, memberId), DBA.CreateParameter("Type", SqlDbType.Int, memberType), DBA.CreateParameter("Token", SqlDbType.NVarChar, token, 255))
            End Sub

#End Region

        End Class
    End Namespace
End Namespace
Imports EGV.Enums
Imports EGV.Utils
Imports EGV.Structures
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization

Public Class APIRequest

    Public Shared Function ProcessRequest(ByVal header As String, ByVal contents As String,
                                          ByVal isNegativeNote As Boolean,
                                          ByVal notificationType As NotificationTypes,
                                          ByVal playerIds() As String,
                                          Optional ByVal protocol As String = "GET") As String
        Dim ret As String = String.Empty
        Dim numOfPages As Integer = Math.Ceiling(CDec(playerIds.Length) / 2000)
        For i As Integer = 0 To numOfPages - 1
            Dim players As List(Of String) = GetPlayers(playerIds.ToList(), i)
            Dim request As HttpWebRequest = WebRequest.Create("https://onesignal.com/api/v1/notifications")
            request.KeepAlive = True
            request.Method = "POST"
            request.ContentType = "application/json; charset=utf-8"
            request.Headers.Add("authorization", "Basic " & Helper.OneSignalAPIKey())
            Dim serializer As New JavaScriptSerializer()
            Dim data As New NotificationItem() With {
                .app_id = Helper.OneSignalAppId,
                .contents = New TextualData() With {.en = contents},
                .headings = New TextualData() With {.en = header},
                .include_player_ids = players.ToArray(),
                .ttl = 86400,
                .data = New NotificationAdditionalData() With {
                    .negative_note = isNegativeNote,
                    .notification_type = notificationType
                }
            }
            Dim params = serializer.Serialize(data)
            Dim byteArray() As Byte = Encoding.UTF8.GetBytes(params)
            Dim responseContent As String = String.Empty
            Try
                Using writer = request.GetRequestStream()
                    writer.Write(byteArray, 0, byteArray.Length)
                End Using
                Try
                    Using response As HttpWebResponse = request.GetResponse()
                        Using reader As New IO.StreamReader(response.GetResponseStream())
                            responseContent = reader.ReadToEnd()
                        End Using
                    End Using
                Catch ex As WebException
                    Using Response = ex.Response
                        Using d As IO.Stream = Response.GetResponseStream()
                            Using reader As New IO.StreamReader(d)
                                Dim Text As String = reader.ReadToEnd()
                                Dim err As OneSignalError = Helper.JSDeserialize(Of OneSignalError)(Text)
                                Throw New Exception(String.Join("<br />", err.errors.ToArray()))
                            End Using
                        End Using
                    End Using
                End Try
            Catch ex As Exception
                Throw ex
            End Try
            If responseContent <> String.Empty Then ret &= "<br />" & responseContent
        Next
        Return ret
    End Function

    Private Shared Function GetPlayers(ByVal lst As List(Of String), ByVal pageIndex As Integer) As List(Of String)
        Dim ret As New List(Of String)
        Dim startIndex As Integer = 2000 * pageIndex
        Dim count As Integer = 2000
        If startIndex > lst.Count Then startIndex = 0
        Dim index As Integer = startIndex
        While index <= lst.Count - 1 AndAlso count > 0
            ret.Add(lst(index))
            index += 1
            count -= 1
        End While
        Return ret
    End Function

    Public Structure OneSignalError
        Public Property errors As List(Of String)
    End Structure

End Class
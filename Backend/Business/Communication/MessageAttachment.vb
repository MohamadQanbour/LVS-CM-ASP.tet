Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures

Namespace EGV
    Namespace Business

        'object
        Public Class MessageAttachment

#Region "Properties"

            Public Property Id As Integer = 0
            Public Property MessageId As Guid = Nothing
            Public Property FileName As String = String.Empty
            Public Property FilePath As String = String.Empty
            Public Property FileSize As Integer = 0

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger)
                    MessageId = dr("MessageId")
                    FileName = Helper.GetSafeDBValue(dr("FileName"))
                    FilePath = Helper.GetSafeDBValue(dr("FilePath"))
                    FileSize = Helper.GetSafeDBValue(dr("FileSize"), ValueTypes.TypeInteger)
                End If
            End Sub

#End Region

#Region "Constructros"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                DBA.GetSafeConn(conn)
                If tid > 0 Then
                    Dim q As String = "SELECT * FROM COM_MessageAttachment WHERE Id = @Id"
                    FillObject(DBA.DataRow(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Public Methods"



#End Region

        End Class

        'controller
        Public Class MessageAttachmentController

#Region "Public Methods"

            Public Shared Sub AddMessageAttachments(ByVal msgId As Guid, ByVal trans As SqlTransaction, ParamArray ByVal files() As String)
                Dim q As String = "INSERT INTO COM_MessageAttachment (MessageId, FileName, FilePath, FileSize) VALUES (@MessageId, @FileName, @FilePath, @FileSize);"
                For Each f As String In files
                    Dim fileInfo As New IO.FileInfo(Helper.Server.MapPath("~" & f))
                    DBA.NonQuery(trans, q,
                                 DBA.CreateParameter("MessageId", SqlDbType.UniqueIdentifier, msgId),
                                 DBA.CreateParameter("FileName", SqlDbType.NVarChar, fileInfo.Name, 255),
                                 DBA.CreateParameter("FilePath", SqlDbType.NVarChar, fileInfo.FullName, 255),
                                 DBA.CreateParameter("FileSize", SqlDbType.Int, fileInfo.Length)
                                 )
                Next
            End Sub

            Public Shared Sub AddMessageAttachments(ByVal msg As Message, Optional ByVal trans As SqlTransaction = Nothing)
                Dim q As String = "INSERT INTO COM_MessageAttachment (MessageId, FileName, FilePath, FileSize) VALUES (@MessageId, @FileName, @FilePath, @FileSize);"
                For Each att As MessageAttachment In msg.Attachments
                    DBA.NonQuery(trans, q,
                                 DBA.CreateParameter("MessageId", SqlDbType.UniqueIdentifier, msg.Id),
                                 DBA.CreateParameter("FileName", SqlDbType.NVarChar, att.FileName, 255),
                                 DBA.CreateParameter("FilePath", SqlDbType.NVarChar, att.FilePath, 255),
                                 DBA.CreateParameter("FileSize", SqlDbType.Int, att.FileSize)
                                 )
                Next
            End Sub

#End Region

        End Class

    End Namespace
End Namespace
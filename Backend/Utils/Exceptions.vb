Imports System.Data.SqlClient

Namespace EGV
    Namespace Utils
        Public Class ExceptionHandler

            Public Shared Sub ProcessException(ByVal ex As Exception, ByVal notifier As Interfaces.INotifier, Optional ByVal conn As SqlConnection = Nothing)
                If conn.State = ConnectionState.Open Then Business.EGVExceptionController.AddException(ex, conn)
                ProcessUnrecordedException(ex, notifier)
            End Sub

            Public Shared Sub ProcessUnrecordedException(ByVal ex As Exception, ByVal notifier As Interfaces.INotifier, Optional ByVal overrideDebugging As Boolean = False)
                Dim msg As String = ex.Message & IIf(Helper.DebuggingEnabled() AndAlso Not overrideDebugging, "<br /><br /><small>" & ex.StackTrace & "</small>", "")
                notifier.Danger(msg)
            End Sub

        End Class
    End Namespace
End Namespace
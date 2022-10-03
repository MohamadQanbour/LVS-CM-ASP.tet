Imports System.Data.SqlClient

Namespace EGV
    Namespace Interfaces
        Public Interface ILocBusinessClass

            Property Id As Integer
            Property Title As String
            Sub Translate(langId As Integer, userId As Integer, Optional trans As SqlTransaction = Nothing)

        End Interface
    End Namespace
End Namespace

Imports System.Configuration
Imports System.Web.Configuration
Imports Microsoft.ApplicationBlocks.Data
Imports System.Data.SqlClient
Imports System.Data

Namespace EGV
    Public NotInheritable Class DBA

#Region "Private Methods"

        Private Shared Function GetSafeConnection(ByVal conn As SqlConnection) As SqlConnection
            If conn Is Nothing Then conn = GetConn()
            Return conn
        End Function

        Private Shared Function GetSafeTransaction(ByRef trans As SqlTransaction) As SqlTransaction
            If trans Is Nothing Then
                Dim conn As SqlConnection = GetConn()
                Return conn.BeginTransaction()
            Else
                Return trans
            End If
        End Function

        Private Shared Function GetSafeDataTable(ByRef ds As DataSet) As DataTable
            If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then Return ds.Tables(0) Else Return Nothing
        End Function

        Private Shared Function GetSafeDataRow(ByRef dt As DataTable) As DataRow
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then Return dt.Rows(0) Else Return Nothing
        End Function

#End Region

#Region "General Methods"

        Public Shared Function GetConnString() As String
            Return CType(WebConfigurationManager.ConnectionStrings(IIf(Utils.Helper.IsRemote(), "remote", "local")), ConnectionStringSettings).ConnectionString
        End Function

        Public Shared Function GetConn() As SqlConnection
            Return New SqlConnection(GetConnString())
        End Function

        Public Shared Sub GetSafeConn(ByRef conn As SqlConnection)
            If conn Is Nothing Then conn = GetConn()
        End Sub

        Public Shared Function CreateParameter(ByVal name As String, ByVal type As SqlDbType, ByVal value As Object, Optional ByVal size As Integer = 0, Optional ByVal scale As Integer = 0) As SqlParameter
            If Not name.StartsWith("@") Then name = "@" & name
            Dim ret As New SqlParameter(name, type)
            If size > 0 Then ret.Size = size
            If type = SqlDbType.Decimal Then
                If size > 0 Then ret.Precision = size Else ret.Precision = 18
                ret.Scale = scale
            End If
            ret.Value = value
            Return ret
        End Function

#End Region

#Region "Non-Query"

        Public Shared Sub NonQuery(ByVal conn As SqlConnection, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter)
            SqlHelper.ExecuteNonQuery(GetSafeConnection(conn), CommandType.Text, cmdText, cmdParams)
        End Sub

        Public Shared Sub NonQuery(ByVal trans As SqlTransaction, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter)
            SqlHelper.ExecuteNonQuery(GetSafeTransaction(trans), CommandType.Text, cmdText, cmdParams)
        End Sub

        Public Shared Sub SPNonQuery(ByVal conn As SqlConnection, ByVal spName As String, ByVal ParamArray cmdParams() As SqlParameter)
            SqlHelper.ExecuteNonQuery(GetSafeConnection(conn), spName, cmdParams)
        End Sub

        Public Shared Sub SPNonQuery(ByVal trans As SqlTransaction, ByVal spName As String, ByVal ParamArray cmdParams() As SqlParameter)
            SqlHelper.ExecuteNonQuery(GetSafeTransaction(trans), spName, cmdParams)
        End Sub

#End Region

#Region "Scalar"

        Public Shared Function Scalar(ByVal conn As SqlConnection, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As Object
            Return SqlHelper.ExecuteScalar(GetSafeConnection(conn), CommandType.Text, cmdText, cmdParams)
        End Function

        Public Shared Function Scalar(ByVal trans As SqlTransaction, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As Object
            Return SqlHelper.ExecuteScalar(GetSafeTransaction(trans), CommandType.Text, cmdText, cmdParams)
        End Function

        Public Shared Function SPScalar(ByVal conn As SqlConnection, ByVal spName As String, ByVal ParamArray cmdParams() As SqlParameter) As Object
            Return SqlHelper.ExecuteScalar(GetSafeConnection(conn), spName, cmdParams)
        End Function

        Public Shared Function SPScalar(ByVal trans As SqlTransaction, ByVal spName As String, ByVal ParamArray cmdParams() As SqlParameter) As Object
            Return SqlHelper.ExecuteScalar(GetSafeTransaction(trans), spName, cmdParams)
        End Function

#End Region

#Region "ScalarID"

        Public Shared Function ScalarID(ByVal conn As SqlConnection, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As Integer
            NonQuery(conn, cmdText, cmdParams)
            Return Scalar(conn, "SELECT @@IDENTITY")
        End Function

        Public Shared Function ScalarID(ByVal trans As SqlTransaction, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As Integer
            NonQuery(trans, cmdText, cmdParams)
            Return Scalar(trans, "SELECT @@IDENTITY")
        End Function

        Public Shared Function SPScalarID(ByVal conn As SqlConnection, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As Integer
            SPNonQuery(conn, cmdText, cmdParams)
            Return SPScalar(conn, "SELECT @@IDENTITY")
        End Function

        Public Shared Function SPScalarID(ByVal trans As SqlTransaction, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As Integer
            SPNonQuery(trans, cmdText, cmdParams)
            Return SPScalar(trans, "SELECT @@IDENTITY")
        End Function

#End Region

#Region "DataSet"

        Public Shared Function DataSet(ByVal conn As SqlConnection, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As DataSet
            Return SqlHelper.ExecuteDataset(GetSafeConnection(conn), CommandType.Text, cmdText, cmdParams)
        End Function

        Public Shared Function DataSet(ByVal trans As SqlTransaction, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As DataSet
            Return SqlHelper.ExecuteDataset(GetSafeTransaction(trans), CommandType.Text, cmdText, cmdParams)
        End Function

        Public Shared Function SPDataSet(ByVal conn As SqlConnection, ByVal spName As String, ByVal ParamArray cmdParams() As SqlParameter) As DataSet
            Return SqlHelper.ExecuteDataset(GetSafeConnection(conn), spName, cmdParams)
        End Function

        Public Shared Function SPDataSet(ByVal trans As SqlTransaction, ByVal spName As String, ByVal ParamArray cmdParams() As SqlParameter) As DataSet
            Return SqlHelper.ExecuteDataset(GetSafeTransaction(trans), spName, cmdParams)
        End Function

#End Region

#Region "DataTable"

        Public Shared Function DataTable(ByVal conn As SqlConnection, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As DataTable
            Return GetSafeDataTable(DataSet(conn, cmdText, cmdParams))
        End Function

        Public Shared Function DataTable(ByVal trans As SqlTransaction, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As DataTable
            Return GetSafeDataTable(DataSet(trans, cmdText, cmdParams))
        End Function

        Public Shared Function SPDataTable(ByVal conn As SqlConnection, ByVal spName As String, ByVal ParamArray cmdParams() As SqlParameter) As DataTable
            Return GetSafeDataTable(SPDataSet(conn, spName, cmdParams))
        End Function

        Public Shared Function SPDataTable(ByVal trans As SqlTransaction, ByVal spName As String, ByVal ParamArray cmdParams() As SqlParameter) As DataTable
            Return GetSafeDataTable(SPDataSet(trans, spName, cmdParams))
        End Function

#End Region

#Region "DataRow"

        Public Shared Function DataRow(ByVal conn As SqlConnection, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As DataRow
            Return GetSafeDataRow(DataTable(conn, cmdText, cmdParams))
        End Function

        Public Shared Function DataRow(ByVal trans As SqlTransaction, ByVal cmdText As String, ByVal ParamArray cmdParams() As SqlParameter) As DataRow
            Return GetSafeDataRow(DataTable(trans, cmdText, cmdParams))
        End Function

        Public Shared Function SPDataRow(ByVal conn As SqlConnection, ByVal spName As String, ByVal ParamArray cmdParams() As SqlParameter) As DataRow
            Return GetSafeDataRow(SPDataTable(conn, spName, cmdParams))
        End Function

        Public Shared Function SPDataRow(ByVal trans As SqlTransaction, ByVal spName As String, ByVal ParamArray cmdParams() As SqlParameter) As DataRow
            Return GetSafeDataRow(SPDataTable(trans, spName, cmdParams))
        End Function

#End Region

    End Class
End Namespace
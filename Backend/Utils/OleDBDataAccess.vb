Imports System.Data.OleDb
Imports System.Data

Public Class DataAccess

#Region "Enums"

    Public Enum Database
        Data = 1
        Album = 2
        Polls = 3
    End Enum

    Public Enum ValueType
        DBInteger = 1
        DBString = 2
        DBDate = 3
        DBDateTime = 4
        DBBoolean = 5
    End Enum

#End Region

#Region "Private Methods"

    Private Shared Sub PrepareCommand(ByVal cmd As OleDbCommand, ByVal conn As OleDbConnection, ByVal trans As OleDbTransaction, ByVal cmdType As CommandType, ByVal cmdText As String, ByVal ParamArray cmdParams() As OleDbParameter)
        cmd.Connection = conn
        cmd.CommandText = cmdText
        If trans IsNot Nothing Then
            cmd.Transaction = trans
        End If
        cmd.CommandType = cmdType
        If cmdParams IsNot Nothing Then
            For Each param As OleDbParameter In cmdParams
                cmd.Parameters.Add(param)
            Next
        End If
    End Sub

#End Region

#Region "Public Methods"

    Public Shared Function GetConnStr(ByVal fileName As String) As String
        Return "Provider=Microsoft.JET.OLEDB.4.0;Data Source=|DataDirectory|\" & fileName & ".mdb;Persist Security Info=False;"
    End Function

    Public Shared Function GetConn(ByVal fileName As String) As OleDbConnection
        Return New OleDbConnection(GetConnStr(fileName))
    End Function

    Public Shared Function CreateParameter(ByVal name As String, ByVal type As OleDbType, ByVal value As Object, Optional ByVal size As Integer = 0) As OleDbParameter
        Dim ret As New OleDbParameter(name, type)
        If size > 0 Then ret.Size = size
        ret.Value = value
        Return ret
    End Function

    Public Shared Function SetupQueryNoParameters(ByVal query As String, ByVal ParamArray cmdParams() As OleDbParameter) As String
        query = query.ToString()
        For Each p As OleDbParameter In cmdParams
            query = query.Replace(p.ParameterName, p.Value)
        Next
        Return query
    End Function

    Public Shared Sub GetSafeConnection(ByRef conn As OleDbConnection, Optional ByVal db As Database = Database.Data)
        If conn Is Nothing Then conn = GetConn(db)
    End Sub

    Public Shared Function GetSafeInteger(ByVal field As Object) As Integer
        If field IsNot Nothing AndAlso Not IsDBNull(field) AndAlso IsNumeric(field.ToString()) Then Return field Else Return 0
    End Function

    Public Shared Function GetSafeString(ByVal field As Object) As String
        If field IsNot Nothing AndAlso Not IsDBNull(field) Then Return field.ToString() Else Return String.Empty
    End Function

    Public Shared Function GetSafeDate(ByVal field As Object) As Date
        If field IsNot Nothing AndAlso Not IsDBNull(field) Then Return CDate(field) Else Return Date.MinValue
    End Function

    Public Shared Function GetSafeDatetime(ByVal field As Object) As DateTime
        If field IsNot Nothing AndAlso Not IsDBNull(field) Then Return Convert.ToDateTime(field) Else Return DateTime.MinValue
    End Function

    Public Shared Function GetSafeBoolean(ByVal field As Object) As Boolean
        If field IsNot Nothing AndAlso Not IsDBNull(field) AndAlso field.ToString().ToLower() = "true" Then Return True Else Return False
    End Function

    Public Shared Function GetSafeValue(ByVal field As Object, Optional ByVal dbType As ValueType = ValueType.DBString) As Object
        Select Case dbType
            Case ValueType.DBInteger
                Return GetSafeInteger(field)
            Case ValueType.DBString
                Return GetSafeString(field)
            Case ValueType.DBDate
                Return GetSafeDate(field)
            Case ValueType.DBDateTime
                Return GetSafeDatetime(field)
            Case ValueType.DBBoolean
                Return GetSafeBoolean(field)
            Case Else
                Return GetSafeString(field)
        End Select
    End Function

#End Region

#Region "Non Query"

    Public Shared Sub NonQuery(ByVal conn As OleDbConnection, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter)
        GetSafeConnection(conn)
        Dim close As Boolean = conn.State <> ConnectionState.Open
        Try
            If close Then conn.Open()
            Dim cmd As New OleDbCommand
            PrepareCommand(cmd, conn, Nothing, CommandType.Text, cmdText, params)
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
        Catch ex As Exception
            Throw ex
        Finally
            If close Then conn.Close()
        End Try
    End Sub

    Public Shared Sub NonQuery(ByVal connStr As String, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter)
        NonQuery(New OleDbConnection(connStr), cmdText, params)
    End Sub

    Public Shared Sub NonQuery(ByVal db As Database, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter)
        NonQuery(GetConn(db), cmdText, params)
    End Sub

    Public Shared Sub NonQuery(ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter)
        NonQuery(GetConn(Database.Data), cmdText, params)
    End Sub

    Public Shared Sub NonQuery(ByVal trans As OleDbTransaction, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter)
        If trans Is Nothing Then
            NonQuery(cmdText, params)
        Else
            Dim cmd As New OleDbCommand
            PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, cmdText, params)
            cmd.ExecuteNonQuery()
            cmd.Parameters.Clear()
        End If
    End Sub

#End Region

#Region "Scalar"

    Public Shared Function Scalar(ByVal conn As OleDbConnection, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As Object
        GetSafeConnection(conn)
        Dim close As Boolean = conn.State <> ConnectionState.Open
        Dim ret As Object = Nothing
        Try
            If close Then conn.Open()
            Dim cmd As New OleDbCommand()
            PrepareCommand(cmd, conn, Nothing, CommandType.Text, cmdText, params)
            ret = cmd.ExecuteScalar()
            cmd.Parameters.Clear()
        Catch ex As Exception
            Throw ex
        Finally
            If close Then conn.Close()
        End Try
        Return ret
    End Function

    Public Shared Function Scalar(ByVal connStr As String, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As Object
        Return Scalar(New OleDbConnection(connStr), cmdText, params)
    End Function

    Public Shared Function Scalar(ByVal db As Database, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As Object
        Return Scalar(GetConn(db), cmdText, params)
    End Function

    Public Shared Function Scalar(ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As Object
        Return Scalar(GetConn(Database.Data), cmdText, params)
    End Function

    Public Shared Function Scalar(ByVal trans As OleDbTransaction, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As Object
        If trans Is Nothing Then
            Return Scalar(cmdText, params)
        Else
            Dim cmd As New OleDbCommand
            PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, cmdText, params)
            Dim ret = cmd.ExecuteScalar()
            cmd.Parameters.Clear()
            Return ret
        End If
    End Function

#End Region

#Region "ID"

    Public Shared Function ScalarID(ByVal conn As OleDbConnection, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As Integer
        GetSafeConnection(conn)
        NonQuery(conn, cmdText, params)
        Return Scalar(conn, "SELECT @@IDENTITY")
    End Function

    Public Shared Function ScalarID(ByVal connStr As String, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As Integer
        Return ScalarID(New OleDbConnection(connStr), cmdText, params)
    End Function

    Public Shared Function ScalarID(ByVal db As Database, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As Integer
        Return ScalarID(GetConn(db), cmdText, params)
    End Function

    Public Shared Function ScalarID(ByVal cmdText As Integer, ByVal ParamArray params() As OleDbParameter) As Integer
        Return ScalarID(GetConn(Database.Data), cmdText, params)
    End Function

    Public Shared Function ScalarID(ByVal trans As OleDbTransaction, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As Integer
        If trans Is Nothing Then
            Return ScalarID(cmdText, params)
        Else
            NonQuery(trans, cmdText, params)
            Return Scalar(trans, "SELECT @@IDENTITY")
        End If
    End Function

#End Region

#Region "Reader"

    Public Shared Function Reader(ByVal conn As OleDbConnection, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As OleDbDataReader
        GetSafeConnection(conn)
        Dim close As Boolean = conn.State <> ConnectionState.Open
        Dim ret As OleDbDataReader = Nothing
        Try
            If close Then conn.Open()
            Dim cmd As New OleDbCommand
            PrepareCommand(cmd, conn, Nothing, CommandType.Text, cmdText, params)
            ret = cmd.ExecuteReader()
            cmd.Parameters.Clear()
        Catch ex As Exception
            Throw ex
        Finally
            If close Then conn.Close()
        End Try
        Return ret
    End Function

    Public Shared Function Reader(ByVal connStr As String, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As OleDbDataReader
        Return Reader(New OleDbConnection(connStr), cmdText, params)
    End Function

    Public Shared Function Reader(ByVal db As Database, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As OleDbDataReader
        Return Reader(GetConn(db), cmdText, params)
    End Function

    Public Shared Function Reader(ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As OleDbDataReader
        Return Reader(GetConn(Database.Data), cmdText, params)
    End Function

    Public Shared Function Reader(ByVal trans As OleDbTransaction, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As OleDbDataReader
        If trans Is Nothing Then
            Return Reader(cmdText, params)
        Else
            Dim cmd As New OleDbCommand
            PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, cmdText, params)
            Dim ret As OleDbDataReader = Nothing
            ret = cmd.ExecuteReader()
            cmd.Parameters.Clear()
            Return ret
        End If
    End Function

#End Region

#Region "DataSet"

    Public Shared Function Dataset(ByVal conn As OleDbConnection, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataSet
        GetSafeConnection(conn)
        Dim close As Boolean = conn.State <> ConnectionState.Open
        Dim ret As New DataSet()
        Try
            If close Then conn.Open()
            Dim cmd As New OleDbCommand
            PrepareCommand(cmd, conn, Nothing, CommandType.Text, cmdText, params)
            Dim da As New OleDbDataAdapter()
            da.SelectCommand = cmd
            da.Fill(ret)
            cmd.Parameters.Clear()
        Catch ex As Exception
            Throw ex
        Finally
            If close Then conn.Close()
        End Try
        Return ret
    End Function

    Public Shared Function Dataset(ByVal connStr As String, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataSet
        Return Dataset(New OleDbConnection(connStr), cmdText, params)
    End Function

    Public Shared Function Dataset(ByVal db As Database, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataSet
        Return Dataset(GetConn(db), cmdText, params)
    End Function

    Public Shared Function Dataset(ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataSet
        Return Dataset(GetConn(Database.Data), cmdText, params)
    End Function

    Public Shared Function Dataset(ByVal trans As OleDbTransaction, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataSet
        If trans Is Nothing Then
            Return Dataset(cmdText, params)
        Else
            Dim cmd As New OleDbCommand
            Dim ret As New DataSet
            PrepareCommand(cmd, trans.Connection, trans, CommandType.Text, cmdText, params)
            Dim da As New OleDbDataAdapter()
            da.SelectCommand = cmd
            da.Fill(ret)
            cmd.Parameters.Clear()
            Return ret
        End If
    End Function

#End Region

#Region "DataTable"

    Public Shared Function Datatable(ByVal conn As OleDbConnection, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataTable
        Dim ds = Dataset(conn, cmdText, params)
        If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then Return ds.Tables(0) Else Return Nothing
    End Function

    Public Shared Function Datatable(ByVal connStr As String, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataTable
        Return Datatable(New OleDbConnection(connStr), cmdText, params)
    End Function

    Public Shared Function Datatable(ByVal db As Database, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataTable
        Return Datatable(GetConn(db), cmdText, params)
    End Function

    Public Shared Function Datatable(ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataTable
        Return Datatable(GetConn(Database.Data), cmdText, params)
    End Function

    Public Shared Function Datatable(ByVal trans As OleDbTransaction, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataTable
        If trans Is Nothing Then
            Return Datatable(cmdText, params)
        Else
            Dim ds = Dataset(trans, cmdText, params)
            If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then Return ds.Tables(0) Else Return Nothing
        End If
    End Function

#End Region

#Region "DataRow"

    Public Shared Function Datarow(ByVal conn As OleDbConnection, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataRow
        Dim dt = Datatable(conn, cmdText, params)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then Return dt.Rows(0) Else Return Nothing
    End Function

    Public Shared Function Datarow(ByVal connStr As String, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataRow
        Return Datarow(New OleDbConnection(connStr), cmdText, params)
    End Function

    Public Shared Function Datarow(ByVal db As Database, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataRow
        Return Datarow(GetConn(db), cmdText, params)
    End Function

    Public Shared Function Datarow(ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataRow
        Return Datarow(GetConn(Database.Data), cmdText, params)
    End Function

    Public Shared Function Datarow(ByVal trans As OleDbTransaction, ByVal cmdText As String, ByVal ParamArray params() As OleDbParameter) As DataRow
        If trans Is Nothing Then
            Return Datarow(cmdText, params)
        Else
            Dim dt = Datatable(trans, cmdText, params)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then Return dt.Rows(0) Else Return Nothing
        End If
    End Function

#End Region

End Class
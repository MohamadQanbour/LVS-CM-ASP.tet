Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports EGV.Utils

Namespace EGV
    Public Class CustomQuery

#Region "Properties"

        Private Property Columns As List(Of Structures.DataColumn)
        Private Property JoinedTables As List(Of Structures.JoinedDataSource)
        Private Property SortColumns As List(Of Structures.DataSort)
        Private Property Conditions As List(Of Structures.DataCondition)
        Private Property GroupColumns As List(Of Structures.DataGroup)
        Private Property FunctionalColumns As List(Of String)
        Private Property SourceTable As Structures.DataSource

        Private Property Parameters As List(Of SqlParameter)

        Private Property MyConn As SqlConnection

#End Region

#Region "Properties"

        Public Property EnablePaging As Boolean = False
        Public Property PageSize As Integer = 10
        Public Property PageIndex As Integer = 0
        Public Property PagingCriteria As String

        Public Property CountCriteria As String = "COUNT(*)"
        Public Property ConditionsCriteria As String = "AND"
        Public Property TablePrimaryKey As String = "Id"

        Public Property IsDistinct As Boolean = False
        Public Property OnlyMainColumns As Boolean = False

#End Region

#Region "Constructors"

        Public Sub New(ByVal sourceTableName As String, Optional ByVal sourceTableAlias As String = "", Optional ByVal conn As SqlConnection = Nothing)
            MyConn = conn
            Columns = New List(Of Structures.DataColumn)
            JoinedTables = New List(Of Structures.JoinedDataSource)
            SortColumns = New List(Of Structures.DataSort)
            Conditions = New List(Of Structures.DataCondition)
            GroupColumns = New List(Of Structures.DataGroup)
            SourceTable = New Structures.DataSource() With {.TableName = sourceTableName, .TableAlias = sourceTableAlias}
            Parameters = New List(Of SqlParameter)
            FunctionalColumns = New List(Of String)()
        End Sub

#End Region

#Region "Public Methods"

        Public Function GetQuery() As String
            Return BuildQuery()
        End Function

        Public Function Execute() As DataTable
            Dim q As String = BuildQuery()
            Return DBA.DataTable(MyConn, q, Parameters.ToArray())
        End Function

        Public Function ExecuteCount() As Integer
            Dim q As String = BuildCountQuery()
            Return DBA.Scalar(MyConn, q, Parameters.ToArray())
        End Function

        Public Sub AddColumn(ByVal colName As String, Optional ByVal colAlias As String = "", Optional ByVal colRename As String = "")
            If Not colName.StartsWith("[") Then colName = "[" & colName & "]"
            Dim obj As New Structures.DataColumn() With {.ColumName = colName, .ColumnAlias = colAlias, .ColumnRename = colRename}
            If Not Columns.Contains(obj) Then
                Columns.Add(obj)
            End If
        End Sub

        Public Sub AddFunctionalColumn(ByVal func As String, ByVal rename As String)
            FunctionalColumns.Add("(" & func & ") AS " & rename)
        End Sub

        Public Sub AddJoinTable(ByVal tblName As String, ByVal tblAlias As String, ByVal tblJoinField As String, ByVal tblSourceJoinField As String, Optional ByVal tblJoinType As Enums.TableJoinTypes = Enums.TableJoinTypes.Inner, Optional ByVal addLanguageCondition As Boolean = False, Optional ByVal sourceFieldAlias As String = "")
            Dim obj As New Structures.JoinedDataSource() With {
                .TableName = tblName,
                .TableAlias = tblAlias,
                .JoinField = tblJoinField,
                .SourceJoinField = tblSourceJoinField,
                .JoinType = tblJoinType,
                .AddLanguageCondition = addLanguageCondition,
                .SourceFieldAlias = sourceFieldAlias
            }
            If Not JoinedTables.Contains(obj) Then JoinedTables.Add(obj)
        End Sub

        Public Sub AddSortColumn(ByVal colName As String, Optional ByVal colSortDirection As Enums.SortDirections = Enums.SortDirections.Ascending, Optional ByVal colAlias As String = "")
            SortColumns.Add(New Structures.DataSort() With {
                .SortColumn = colName,
                .SortColumnAlias = colAlias,
                .SortDirection = colSortDirection
            })
        End Sub

        Public Sub AddCondition(ByVal cond As String)
            If cond <> String.Empty Then Conditions.Add(New Structures.DataCondition() With {.Condition = cond})
        End Sub

        Public Sub AddGroupColumn(ByVal colName As String, Optional ByVal colAlias As String = "")
            GroupColumns.Add(New Structures.DataGroup() With {
                             .ColumnName = colName,
                             .ColumnAlias = colAlias
            })
        End Sub

        Public Sub AddParameter(ByVal p As SqlParameter)
            Parameters.Add(p)
        End Sub

        Public Sub AddParamter(ByVal name As String, ByVal type As SqlDbType, ByVal value As Object, Optional ByVal size As Integer = 0)
            Parameters.Add(DBA.CreateParameter(name, type, value, size))
        End Sub

#End Region

#Region "Private Methods"

        Private Function BuildQuery() As String
            Dim sb As New StringBuilder()
            If EnablePaging Then
                sb.AppendFormat("WITH OrderedTable AS (SELECT {0}, ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber FROM {2})", GetQueryColumns(True), GetRowNumberOrder(), GetPagingQueryBody())
                sb.AppendFormat("SELECT TOP {0} {1} FROM OrderedTable WHERE RowNumber > {2}", PageSize, GetQueryColumns(False), PageSize * PageIndex)
            Else
                sb.AppendFormat("SELECT {0} FROM {1}", GetQueryColumns(), GetQueryBody())
            End If
            Return sb.ToString()
        End Function

        Private Function BuildCountQuery() As String
            Dim sb As New StringBuilder()
            sb.AppendFormat("SELECT {0} FROM {1}", CountCriteria, GetCountQueryBody())
            Return sb.ToString()
        End Function

        Private Function GetQueryColumns(Optional ByVal addAlias As Boolean = True) As String
            Dim lst As New List(Of String)
            For Each c As Structures.DataColumn In Columns
                lst.Add(
                    IIf(addAlias AndAlso c.ColumnAlias <> String.Empty, c.ColumnAlias & ".", "") &
                    IIf(addAlias, c.ColumName, IIf(c.ColumnRename <> String.Empty, c.ColumnRename, c.ColumName)) &
                    IIf(addAlias AndAlso c.ColumnRename <> String.Empty, " AS [" & c.ColumnRename & "]", "")
                )
            Next
            Dim ret As String = IIf(OnlyMainColumns, SourceTable.TableAlias & ".", "") & "*"
            If lst.Count > 0 Then ret = String.Join(", ", lst.ToArray())
            Dim fLst As New List(Of String)
            For Each fc As String In FunctionalColumns
                If addAlias Then fLst.Add(fc) Else fLst.Add(fc.Substring(fc.LastIndexOf(" ") + 1))
            Next
            If fLst.Count > 0 Then ret &= ", " & String.Join(", ", fLst.ToArray())
            Return ret
        End Function

        Private Function GetQueryBody() As String
            Dim ret As String = ""
            ret = GetQuerySource()
            If Conditions.Count > 0 Then ret &= " WHERE " & GetQueryConditions()
            If SortColumns.Count > 0 Then ret &= " ORDER BY " & GetQueryOrder()
            If GroupColumns.Count > 0 Then ret &= " GROUP BY " & GetQueryGroup()
            Return ret
        End Function

        Private Function GetPagingQueryBody() As String
            Dim ret As String = ""
            ret = GetQuerySource()
            If Conditions.Count > 0 Then ret &= " WHERE " & GetQueryConditions()
            If GroupColumns.Count > 0 Then ret &= " GROUP BY " & GetQueryGroup()
            Return ret
        End Function

        Private Function GetCountQueryBody() As String
            Dim ret As String = ""
            ret = GetQuerySource()
            If Conditions.Count > 0 Then ret &= " WHERE " & GetQueryConditions()
            If GroupColumns.Count > 0 Then ret &= " GROUP BY " & GetQueryGroup()
            Return ret
        End Function

        Private Function GetQuerySource() As String
            Dim sb As New StringBuilder()
            sb.AppendLine(SourceTable.TableName & IIf(SourceTable.TableAlias <> String.Empty, " AS " & SourceTable.TableAlias, ""))
            For Each item As Structures.JoinedDataSource In JoinedTables
                sb.AppendFormat(" {0} {1} ON {2} = {3}{4}",
                                GetJoinType(item.JoinType),
                                item.TableName & IIf(item.TableAlias <> String.Empty, " AS " & item.TableAlias, ""),
                                IIf(item.SourceFieldAlias <> String.Empty, item.SourceFieldAlias & "." & item.SourceJoinField, IIf(SourceTable.TableAlias <> String.Empty, SourceTable.TableAlias & ".", "") & item.SourceJoinField),
                                IIf(item.TableAlias <> String.Empty, item.TableAlias & ".", "") & item.JoinField,
                                IIf(item.AddLanguageCondition, " AND " & IIf(item.TableAlias <> String.Empty, item.TableAlias & ".", "") & "LanguageId = " & Helper.LanguageId(), "")
                )
            Next
            Return sb.ToString()
        End Function

        Private Function GetQueryConditions() As String
            Dim lst As New List(Of String)
            For Each c As Structures.DataCondition In Conditions
                lst.Add(c.Condition)
            Next
            Return String.Join(" " & ConditionsCriteria & " ", lst.ToArray())
        End Function

        Private Function GetQueryOrder(Optional ByVal addAlias As Boolean = True) As String
            Dim lst As New List(Of String)
            For Each col As Structures.DataSort In SortColumns
                lst.Add(IIf(addAlias AndAlso col.SortColumnAlias <> String.Empty, col.SortColumnAlias & ".", "") & col.SortColumn & " " & GetSortType(col.SortDirection))
            Next
            Return String.Join(", ", lst.ToArray())
        End Function

        Private Function GetRowNumberOrder() As String
            Dim lst As New List(Of String)
            For Each col As Structures.DataSort In SortColumns
                lst.Add(IIf(col.SortColumnAlias <> String.Empty, col.SortColumnAlias & ".", "") & col.SortColumn & " " & GetSortType(col.SortDirection))
            Next
            If lst.Count = 0 Then lst.Add(IIf(SourceTable.TableAlias <> String.Empty, SourceTable.TableAlias & ".", "") & TablePrimaryKey & " ASC")
            Return String.Join(", ", lst.ToArray())
        End Function

        Private Function GetQueryGroup() As String
            Dim lst As New List(Of String)
            For Each g As Structures.DataGroup In GroupColumns
                lst.Add(IIf(g.ColumnAlias <> String.Empty, g.ColumnAlias & ".", "") & g.ColumnName)
            Next
            Return String.Join(", ", lst.ToArray())
        End Function

        Private Function GetJoinType(ByVal j As Enums.TableJoinTypes) As String
            Select Case j
                Case Enums.TableJoinTypes.Inner
                    Return "INNER JOIN"
                Case Enums.TableJoinTypes.Left
                    Return "LEFT JOIN"
                Case Enums.TableJoinTypes.Outer
                    Return "OUTER JOIN"
                Case Enums.TableJoinTypes.Right
                    Return "RIGHT JOIN"
                Case Else
                    Return ""
            End Select
        End Function

        Private Function GetSortType(ByVal s As Enums.SortDirections) As String
            Select Case s
                Case Enums.SortDirections.Ascending
                    Return "ASC"
                Case Enums.SortDirections.Descending
                    Return "DESC"
                Case Else
                    Return "ASC"
            End Select
        End Function

#End Region

    End Class
End Namespace
Imports System.Xml
Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Structures

Namespace EGV
    Namespace Business
        Public Class EntityStructure

#Region "Private Properties"

            Private Property UserSettings As UserGrid

#End Region

#Region "Properties"

            Public Property TableName As String
            Public Property TableAlias As String
            Public Property PrimaryKey As String
            Public Property Columns As List(Of ESColumn)
            Public Property EnableSelection As Boolean
            Public Property EnableSearch As Boolean
            Public Property EnableRefresh As Boolean
            Public Property EnableColumnSelection As Boolean
            Public Property EnableFiltering As Boolean
            Public Property EnableChangePageSize As Boolean
            Public Property EditRecordLink As String
            Public Property EnableSorting As Boolean
            Public Property EnableChangeColumnOrder As Boolean
            Public Property EnablePaging As Boolean
            Public Property PageSize As Integer
            Public Property PageIndex As Integer
            Public Property PagingPosition As Enums.PagingPositions
            Public Property OrderColumns As List(Of DataSort)
            Public Property JoinedTables As List(Of JoinedDataSource)
            Public Property ControlColumns As List(Of ESControlColumn)
            Public Property SearchColumns As List(Of ESSearchColumn)
            Public Property AppliedFilters As List(Of UserGridColumnFilter)
            Public Property Conditions As List(Of String)

            Public ReadOnly Property PrimaryKeyName As String
                Get
                    Return PrimaryKey.Replace(TableAlias & ".", "")
                End Get
            End Property

            Private Property XMLFileName As String

#End Region

#Region "Constructors"

            Public Sub New(ByVal Target As String, Optional ByVal userId As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                Columns = New List(Of ESColumn)()
                OrderColumns = New List(Of DataSort)()
                JoinedTables = New List(Of JoinedDataSource)
                ControlColumns = New List(Of ESControlColumn)
                SearchColumns = New List(Of ESSearchColumn)
                AppliedFilters = New List(Of UserGridColumnFilter)
                Conditions = New List(Of String)()
                If Not Target.EndsWith(".xml") Then Target &= ".xml"
                XMLFileName = "~" & Path.MapStructuresFile(Target)
                Deserialize()
                UserSettings = New UserGrid()
                UserSettings.UserId = userId
                UserSettings.BusinessClass = Target
                InitializeUserSettings()
                If userId > 0 Then
                    If UserGridController.Exists(userId, Target, conn) Then
                        UserSettings = UserGridController.LoadGridSettings(userId, Target, conn)
                        UpdateByUserSettings()
                    End If
                End If
            End Sub

#End Region

#Region "Private Methods"

            Private Function GetOperation(ByVal type As Enums.SearchTypes, ByVal expression As String) As String
                Dim ret As String = ""
                Select Case type
                    Case Enums.SearchTypes.Contains
                        Return " LIKE '%' + " & GetValueParam(expression) & " + '%'"
                    Case Enums.SearchTypes.EndsWith
                        Return " LIKE '%' + " & GetValueParam(expression)
                    Case Enums.SearchTypes.StartsWith
                        Return " LIKE " & GetValueParam(expression) & " + '%'"
                    Case Enums.SearchTypes.Exact
                        Return " = " & GetValueParam(expression)
                    Case Enums.SearchTypes.GreaterThan
                        Return " > " & GetValueParam(expression)
                    Case Enums.SearchTypes.GreaterThanOrEqual
                        Return " >= " & GetValueParam(expression)
                    Case Enums.SearchTypes.LessThan
                        Return " < " & GetValueParam(expression)
                    Case Enums.SearchTypes.LessThanOrEqual
                        Return " <= " & GetValueParam(expression)
                    Case Enums.SearchTypes.NotEqual
                        Return " <> " & GetValueParam(expression)
                    Case Enums.SearchTypes.Equal
                        Return " = " & GetValueParam(expression)
                    Case Else
                        Return " = " & GetValueParam(expression)
                End Select
            End Function

            Private Function GetValueParam(ByVal val As String) As String
                Return "@" & IIf(val.Contains("."), val.Replace(".", ""), val)
            End Function

            Private Function GetDBType(ByVal type As Enums.FilterTypes) As SqlDbType
                Select Case type
                    Case Enums.FilterTypes.Boolean
                        Return SqlDbType.Bit
                    Case Enums.FilterTypes.Date
                        Return SqlDbType.Date
                    Case Enums.FilterTypes.Number, Enums.FilterTypes.List
                        Return SqlDbType.Int
                    Case Enums.FilterTypes.String
                        Return SqlDbType.NVarChar
                    Case Else
                        Return SqlDbType.NVarChar
                End Select
            End Function

            Private Sub SaveUserSettings(ByVal conn As SqlConnection)
                Dim newColumns As New List(Of UserGridColumn)
                For Each c As UserGridColumn In UserSettings.Columns
                    Dim t As ESColumn = (From col In Columns Where col.Name = c.Name AndAlso col.TableAlias = c.Alias).FirstOrDefault()
                    Dim index As Integer = UserSettings.Columns.IndexOf(c)
                    c.Order = t.ColumnOrder
                    c.Visible = t.Visible
                    newColumns.Add(c)
                Next
                UserSettings.Columns = newColumns
                UserSettings.PageSize = PageSize
                UserSettings.PageIndex = PageIndex
                UserSettings.SortColumns.Clear()
                For Each c As DataSort In OrderColumns
                    UserSettings.SortColumns.Add(New UserGridSortColumn() With {
                        .Name = c.SortColumn,
                        .Alias = c.SortColumnAlias,
                        .Direction = c.SortDirection
                    })
                Next
                UserSettings.Filters.Clear()
                For Each f As UserGridColumnFilter In AppliedFilters
                    UserSettings.Filters.Add(New UserGridColumnFilter() With {
                        .Expression = f.Expression,
                        .Operation = f.Operation,
                        .Value = f.Value,
                        .DataType = f.DataType
                    })
                Next
                UserGridController.SaveGridSettings(UserSettings, conn)
            End Sub

            Private Sub InitializeUserSettings()
                UserSettings.Columns.Clear()
                For Each c As ESColumn In Columns
                    UserSettings.Columns.Add(New UserGridColumn() With {
                        .Name = c.Name,
                        .Alias = c.TableAlias,
                        .Visible = c.Visible,
                        .Order = c.ColumnOrder
                    })
                Next
                UserSettings.SearchValue = String.Empty
                UserSettings.PageSize = PageSize
                UserSettings.PageIndex = PageIndex
                UserSettings.SortColumns.Clear()
                For Each c As DataSort In OrderColumns
                    UserSettings.SortColumns.Add(New UserGridSortColumn() With {
                        .Name = c.SortColumn,
                        .Alias = c.SortColumnAlias,
                        .Direction = c.SortDirection
                    })
                Next
            End Sub

            Private Sub UpdateByUserSettings()
                For Each c As UserGridColumn In UserSettings.Columns
                    Dim t As ESColumn = (From col In Columns Where col.Name = c.Name AndAlso col.TableAlias = c.Alias).FirstOrDefault()
                    Dim index As Integer = Columns.IndexOf(t)
                    t.Visible = c.Visible
                    t.ColumnOrder = c.Order
                    Columns(index) = t
                Next
                PageSize = UserSettings.PageSize
                PageIndex = UserSettings.PageIndex
                OrderColumns.Clear()
                For Each c As UserGridSortColumn In UserSettings.SortColumns
                    OrderColumns.Add(New DataSort() With {
                        .SortColumn = c.Name,
                        .SortColumnAlias = c.Alias,
                        .SortDirection = c.Direction
                    })
                Next
                AppliedFilters.Clear()
                For Each f As UserGridColumnFilter In UserSettings.Filters
                    AppliedFilters.Add(New UserGridColumnFilter() With {
                        .Expression = f.Expression,
                        .Operation = f.Operation,
                        .Value = f.Value,
                        .DataType = f.DataType
                    })
                Next
            End Sub

            Private Sub LoadDefaultOrderColumn()
                Dim xml As New XmlDocument()
                Dim loaded As Boolean = True
                Try
                    xml.Load(Helper.Server.MapPath(XMLFileName))
                Catch ex As Exception
                    loaded = False
                End Try
                If loaded Then
                    Try
                        Dim root = xml.SelectSingleNode("Entity")
                        Dim defaults = root.SelectSingleNode("Defaults")
                        If defaults IsNot Nothing Then
                            Dim orderColumnsNode = defaults.SelectSingleNode("OrderColumns")
                            If orderColumnsNode IsNot Nothing Then
                                For Each n As XmlNode In orderColumnsNode.SelectNodes("Column")
                                    OrderColumns.Add(New DataSort() With {
                                        .SortColumn = Helper.GetSafeXML(n, "Name"),
                                        .SortDirection = Helper.GetSafeXML(n, "Order", Enums.ValueTypes.TypeInteger),
                                        .SortColumnAlias = Helper.GetSafeXML(n, "Alias")
                                    })
                                Next
                            End If
                        End If
                    Catch ex As Exception
                        Throw ex
                    End Try
                Else
                    Throw New Exception("Structure file (" & XMLFileName & ") not loaded.")
                End If
            End Sub

            Private Sub Deserialize()
                Dim xml As New XmlDocument()
                Dim loaded As Boolean = True
                Try
                    xml.Load(Helper.Server.MapPath(XMLFileName))
                Catch ex As Exception
                    loaded = False
                End Try
                If loaded Then
                    Try
                        Dim root = xml.SelectSingleNode("Entity")
                        TableName = Helper.GetSafeXML(root, "TableName")
                        PrimaryKey = Helper.GetSafeXML(root, "PrimaryKey")
                        TableAlias = Helper.GetSafeXML(root, "Alias")
                        'Defaults
                        Dim defaults = root.SelectSingleNode("Defaults")
                        If defaults IsNot Nothing Then
                            EnableSelection = Helper.GetSafeXML(defaults, "EnableSelection", Enums.ValueTypes.TypeBoolean)
                            EnableSearch = Helper.GetSafeXML(defaults, "EnableSearch", Enums.ValueTypes.TypeBoolean)
                            EnablePaging = Helper.GetSafeXML(defaults, "EnablePaging", Enums.ValueTypes.TypeBoolean)
                            EnableRefresh = Helper.GetSafeXML(defaults, "EnableRefresh", Enums.ValueTypes.TypeBoolean)
                            EnableColumnSelection = Helper.GetSafeXML(defaults, "EnableColumnSelection", Enums.ValueTypes.TypeBoolean)
                            EnableFiltering = Helper.GetSafeXML(defaults, "EnableFiltering", Enums.ValueTypes.TypeBoolean)
                            EnableChangePageSize = Helper.GetSafeXML(defaults, "EnableChangePageSize", Enums.ValueTypes.TypeBoolean)
                            EditRecordLink = Helper.GetSafeXML(defaults, "EditRecordLink")
                            EnableSorting = Helper.GetSafeXML(defaults, "EnableSorting", Enums.ValueTypes.TypeBoolean)
                            EnableChangeColumnOrder = Helper.GetSafeXML(defaults, "EnableChangeColumnOrder", Enums.ValueTypes.TypeBoolean)
                            PageSize = Helper.GetSafeXML(defaults, "PageSize", Enums.ValueTypes.TypeInteger)
                            PageIndex = Helper.GetSafeXML(defaults, "PageIndex", Enums.ValueTypes.TypeInteger)
                            PagingPosition = Helper.GetSafeXML(defaults, "PagingPosition", Enums.ValueTypes.TypeInteger)
                            Dim orderColumnsNode = defaults.SelectSingleNode("OrderColumns")
                            If orderColumnsNode IsNot Nothing Then
                                For Each n As XmlNode In orderColumnsNode.SelectNodes("Column")
                                    OrderColumns.Add(New Structures.DataSort() With {
                                        .SortColumn = Helper.GetSafeXML(n, "Name"),
                                        .SortDirection = Helper.GetSafeXML(n, "Order", Enums.ValueTypes.TypeInteger),
                                        .SortColumnAlias = Helper.GetSafeXML(n, "Alias")
                                    })
                                Next
                            End If
                        End If
                        'columns
                        Dim cols = root.SelectSingleNode("Columns")
                        If cols IsNot Nothing Then
                            For Each c As XmlNode In cols.SelectNodes("Column")
                                Dim col As New Structures.ESColumn() With {
                                    .Name = Helper.GetSafeXML(c, "Name"),
                                    .DataType = Helper.GetSafeXML(c, "DataType", Enums.ValueTypes.TypeInteger),
                                    .Visible = Helper.GetSafeXML(c, "Visible", Enums.ValueTypes.TypeBoolean),
                                    .ColumnOrder = Helper.GetSafeXML(c, "Order", Enums.ValueTypes.TypeInteger),
                                    .TableAlias = Helper.GetSafeXML(c, "Alias"),
                                    .EnumLookup = Helper.GetSafeXML(c, "EnumLookup"),
                                    .Lookup = Helper.GetSafeXML(c, "Lookup"),
                                    .Rename = Helper.GetSafeXML(c, "Rename"),
                                    .AllowSort = Helper.GetSafeXML(c, "AllowSort", Enums.ValueTypes.TypeBoolean),
                                    .AllowReorder = Helper.GetSafeXML(c, "AllowReorder", Enums.ValueTypes.TypeBoolean)
                                }
                                Dim resouce As String = Helper.GetSafeXML(c, "Resource")
                                If resouce <> String.Empty Then col.Resource = Localization.GetResource("Resources.Global.Business." & resouce) Else col.Resource = col.Name
                                Dim layout As XmlNode = c.SelectSingleNode("Layout")
                                If layout IsNot Nothing Then
                                    col.Layout = New Structures.ESColumnLayout() With {
                                        .Width = Helper.GetSafeXML(layout, "Width"),
                                        .IsPrimary = Helper.GetSafeXML(layout, "IsPrimary", Enums.ValueTypes.TypeBoolean),
                                        .EditControl = Helper.GetSafeXML(layout, "EditControl", Enums.ValueTypes.TypeBoolean),
                                        .HeaderAlign = Helper.GetSafeXML(layout, "HeaderAlign", Enums.ValueTypes.TypeInteger),
                                        .ItemAlign = Helper.GetSafeXML(layout, "ItemAlign", Enums.ValueTypes.TypeInteger),
                                        .DisplayType = Helper.GetSafeXML(layout, "DisplayType", Enums.ValueTypes.TypeInteger),
                                        .TypeLookup = Helper.GetSafeXML(layout, "TypeLookup", Enums.ValueTypes.TypeString),
                                        .EditTypeDecider = Helper.GetSafeXML(layout, "EditTypeDecider"),
                                        .ListDataSource = Helper.GetSafeXML(layout, "ListDataSource"),
                                        .ListEnumDataSource = Helper.GetSafeXML(layout, "ListEnumDataSource"),
                                        .DataTextField = Helper.GetSafeXML(layout, "DataTextField"),
                                        .DataValueField = Helper.GetSafeXML(layout, "DataValueField"),
                                        .LookupCondition = Helper.GetSafeXML(layout, "LookupCondition"),
                                        .LookupConditionValueField = Helper.GetSafeXML(layout, "LookupConditionValueField"),
                                        .ParentField = Helper.GetSafeXML(layout, "ParentField")
                                    }
                                End If
                                Dim filter As XmlNode = c.SelectSingleNode("Filter")
                                If filter IsNot Nothing Then
                                    Dim f As New ESColumnFilter() With {
                                        .FilterType = Helper.GetSafeXML(filter, "FilterType", Enums.ValueTypes.TypeInteger),
                                        .FilterDataText = Helper.GetSafeXML(filter, "FilterDataText"),
                                        .FilterDataValue = Helper.GetSafeXML(filter, "FilterDataValue"),
                                        .Expression = Helper.GetSafeXML(filter, "Expression"),
                                        .Allow = Helper.GetSafeXML(filter, "Allow")
                                    }
                                    Dim types As String = Helper.GetSafeXML(filter, "AllowedTypes")
                                    f.AllowedTypes = New List(Of Enums.SearchTypes)
                                    If types <> String.Empty Then
                                        Dim parts() As String = Helper.SplitString(types, ",")
                                        For Each i As Integer In parts
                                            f.AllowedTypes.Add(i)
                                        Next
                                    End If
                                    col.Filter = f
                                End If
                                Columns.Add(col)
                            Next
                        End If
                        'Control Columns
                        Dim ctrl = root.SelectSingleNode("ControlColumns")
                        If ctrl IsNot Nothing Then
                            For Each c As XmlNode In ctrl.SelectNodes("Column")
                                Dim col As New Structures.ESControlColumn() With {
                                    .PermissionId = Helper.GetSafeXML(c, "PermissionId", Enums.ValueTypes.TypeInteger),
                                    .Super = Helper.GetSafeXML(c, "Super", Enums.ValueTypes.TypeBoolean),
                                    .Type = Helper.GetSafeXML(c, "Type", Enums.ValueTypes.TypeInteger)
                                }
                                Dim resource As String = Helper.GetSafeXML(c, "Resource")
                                If resource <> String.Empty Then col.Resource = Localization.GetResource("Resources.Global.Business." & resource)
                                ControlColumns.Add(col)
                            Next
                        End If
                        'Search Columns
                        Dim search = root.SelectSingleNode("SearchColumns")
                        If search IsNot Nothing Then
                            For Each s As XmlNode In search.SelectNodes("Column")
                                SearchColumns.Add(New Structures.ESSearchColumn() With {
                                    .Name = Helper.GetSafeXML(s, "Name"),
                                    .Type = Helper.GetSafeXML(s, "SearchType", Enums.ValueTypes.TypeInteger)
                                })
                            Next
                        End If
                        'joined tables
                        Dim joined = root.SelectSingleNode("JoinedTables")
                        If joined IsNot Nothing Then
                            For Each j As XmlNode In joined.SelectNodes("Table")
                                JoinedTables.Add(New Structures.JoinedDataSource() With {
                                    .TableName = Helper.GetSafeXML(j, "Name"),
                                    .TableAlias = Helper.GetSafeXML(j, "Alias"),
                                    .SourceJoinField = Helper.GetSafeXML(j, "SourceField"),
                                    .JoinField = Helper.GetSafeXML(j, "JoinField"),
                                    .JoinType = Helper.GetSafeXML(j, "JoinType", Enums.ValueTypes.TypeInteger),
                                    .AddLanguageCondition = Helper.GetSafeXML(j, "AddLanguageCondition", Enums.ValueTypes.TypeBoolean),
                                    .SourceFieldAlias = Helper.GetSafeXML(j, "SourceFieldAlias")
                                })
                            Next
                        End If
                    Catch ex As Exception
                        Throw ex
                    End Try
                Else
                    Throw New Exception("Structure file (" & XMLFileName & ") not loaded.")
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Function GetCustomQuery(Optional ByVal conn As SqlConnection = Nothing) As CustomQuery
                Dim cq As New CustomQuery(TableName, TableAlias, conn)
                cq.TablePrimaryKey = PrimaryKey
                cq.EnablePaging = EnablePaging
                cq.PageSize = PageSize
                cq.PageIndex = PageIndex
                For Each c As ESColumn In From col In Columns Where col.Visible = True OrElse col.Name = PrimaryKeyName Order By col.ColumnOrder
                    cq.AddColumn("[" & c.Name & "]", c.TableAlias, c.Rename)
                    If c.Lookup <> String.Empty Then
                        Dim obj = (From jt In JoinedTables Where (jt.TableAlias & "." & jt.TableName) = (c.TableAlias & "." & c.Lookup) Take 1).First()
                        'cq.AddJoinTable(obj.TableName, obj.TableAlias, "[" & obj.JoinField & "]", "[" & obj.SourceJoinField & "]", obj.JoinType, obj.AddLanguageCondition, obj.SourceFieldAlias)
                    End If
                    If c.Layout.TypeLookup <> String.Empty Then
                        cq.AddColumn(c.Layout.TypeLookup, TableAlias)
                    End If
                    If c.Layout.ParentField <> String.Empty Then
                        Dim parentColumn = (From pc In Columns Where c.Layout.ParentField = pc.TableAlias & "." & pc.Name).FirstOrDefault()
                        If parentColumn.Lookup <> String.Empty Then
                            Dim obj = (From jt In JoinedTables Where (jt.TableAlias & "." & jt.TableName) = (parentColumn.TableAlias & "." & parentColumn.Lookup) Take 1).First()
                            'cq.AddJoinTable(obj.TableName, obj.TableAlias, "[" & obj.JoinField & "]", "[" & obj.SourceJoinField & "]", obj.JoinType, obj.AddLanguageCondition, obj.SourceFieldAlias)
                        End If
                        If parentColumn.Layout.TypeLookup <> String.Empty Then
                            cq.AddColumn(parentColumn.Layout.TypeLookup, TableAlias)
                        End If
                        cq.AddColumn(parentColumn.Name, parentColumn.TableAlias, parentColumn.Rename)
                    End If
                Next
                For Each o As DataSort In OrderColumns
                    cq.AddSortColumn("[" & o.SortColumn & "]", o.SortDirection, o.SortColumnAlias)
                Next
                If EnableFiltering Then
                    For Each f As UserGridColumnFilter In AppliedFilters
                        cq.AddCondition(IIf(f.DataType = Enums.FilterTypes.Date, "CONVERT(date, " & f.Expression & ")", f.Expression) & " " & GetOperation(f.Operation, f.Expression))
                        cq.AddParameter(DBA.CreateParameter(GetValueParam(f.Expression), GetDBType(f.DataType), f.Value))
                    Next
                End If
                For Each c As String In Conditions
                    cq.AddCondition(c)
                Next
                For Each t As JoinedDataSource In JoinedTables
                    cq.AddJoinTable(t.TableName, t.TableAlias, t.JoinField, t.SourceJoinField, t.JoinType, t.AddLanguageCondition, t.SourceFieldAlias)
                Next
                Return cq
            End Function

#End Region

#Region "Manipulation Methods"

            Public Sub AddCondition(ByVal condition As String)
                Conditions.Add(condition)
            End Sub

            Public Sub AddSearchValue(ByVal searchterm As String, Optional ByVal conn As SqlConnection = Nothing)
                UserSettings.SearchValue = searchterm
                SaveUserSettings(conn)
            End Sub

            Public Sub ChangePageSize(ByVal pagesize As Integer, Optional ByVal conn As SqlConnection = Nothing)
                Me.PageSize = pagesize
                UserSettings.PageSize = pagesize
                PageIndex = 0
                UserSettings.PageIndex = 0
                SaveUserSettings(conn)
            End Sub

            Public Sub ChangePageIndex(ByVal newPage As Integer, Optional ByVal conn As SqlConnection = Nothing)
                PageIndex = newPage
                UserSettings.PageIndex = newPage
                SaveUserSettings(conn)
            End Sub

            Public Sub ClearSort()
                OrderColumns.Clear()
            End Sub

            Public Sub AddSortColumn(ByVal col As String, ByVal colAlias As String, ByVal dir As Enums.SortDirections, Optional ByVal conn As SqlConnection = Nothing)
                Dim cols = (From c In OrderColumns Where c.SortColumn = col AndAlso c.SortColumnAlias = colAlias)
                If cols.Count() = 1 Then
                    Dim c = cols.FirstOrDefault()
                    Dim index As Integer = OrderColumns.IndexOf(c)
                    If c.SortDirection <> dir Then
                        c.SortDirection = dir
                        OrderColumns(index) = c
                    End If
                Else
                    OrderColumns.Add(New DataSort() With {
                            .SortColumn = col,
                            .SortColumnAlias = colAlias,
                            .SortDirection = dir
                        })
                End If
                SaveUserSettings(conn)
            End Sub

            Public Sub RemoveSortColumn(ByVal col As String, ByVal colAlias As String, Optional ByVal conn As SqlConnection = Nothing)
                Dim cols = (From c In OrderColumns Where c.SortColumn = col AndAlso c.SortColumnAlias = colAlias)
                If cols.Count = 1 Then
                    Dim c = cols.FirstOrDefault()
                    OrderColumns.Remove(c)
                End If
                If OrderColumns.Count = 0 Then
                    LoadDefaultOrderColumn()
                End If
                SaveUserSettings(conn)
            End Sub

            Public Sub ChangeColOrder(ByVal col As String, ByVal colAlias As String, ByVal newOrder As Integer, Optional ByVal conn As SqlConnection = Nothing)
                If newOrder > 0 Then
                    Dim lst = (From c In Columns Where c.Visible = True AndAlso c.AllowReorder = True)
                    If newOrder <= lst.Count() Then
                        Dim target = lst.Where(Function(x) x.Name = col AndAlso x.TableAlias = colAlias)
                        If target.Count() = 1 Then
                            Dim t = target.FirstOrDefault()
                            If newOrder > t.ColumnOrder Then
                                Dim swapLst = lst.Where(Function(x) x.ColumnOrder > t.ColumnOrder).Take(1)
                                If swapLst.Count() = 1 Then
                                    Dim swap = swapLst.FirstOrDefault()
                                    Dim tIndex As Integer = Columns.IndexOf(t)
                                    Dim sIndex As Integer = Columns.IndexOf(swap)
                                    Dim temp As Integer = t.ColumnOrder
                                    t.ColumnOrder = swap.ColumnOrder
                                    swap.ColumnOrder = temp
                                    Columns(tIndex) = t
                                    Columns(sIndex) = swap
                                End If
                            Else
                                Dim swapLst = lst.Where(Function(x) x.ColumnOrder < t.ColumnOrder).Take(1)
                                If swapLst.Count() = 1 Then
                                    Dim swap = swapLst.FirstOrDefault()
                                    Dim tIndex As Integer = Columns.IndexOf(t)
                                    Dim sIndex As Integer = Columns.IndexOf(swap)
                                    Dim temp As Integer = t.ColumnOrder
                                    t.ColumnOrder = swap.ColumnOrder
                                    swap.ColumnOrder = temp
                                    Columns(tIndex) = t
                                    Columns(sIndex) = swap
                                End If
                            End If
                        End If
                    End If
                End If
                SaveUserSettings(conn)
            End Sub

            Public Sub ToggleColumnVisibile(ByVal lst As List(Of JSSelectedColumn), Optional ByVal conn As SqlConnection = Nothing)
                Dim iColumns = New List(Of ESColumn)
                For Each col As ESColumn In Columns
                    Dim item As New JSSelectedColumn() With {.ColumnAlias = col.TableAlias, .ColumnName = col.Name}
                    col.Visible = lst.Contains(item)
                    iColumns.Add(col)
                Next
                Columns = iColumns
                SaveUserSettings(conn)
            End Sub

            Public Sub AddFilter(ByVal filter As UserGridColumnFilter, Optional ByVal conn As SqlConnection = Nothing)
                Dim colFilters = (From f In AppliedFilters Where f.Expression = filter.Expression)
                If colFilters.Count() = 1 Then
                    Dim item As UserGridColumnFilter = colFilters.FirstOrDefault()
                    Dim index As Integer = AppliedFilters.IndexOf(item)
                    item.Operation = filter.Operation
                    item.Value = filter.Value
                    AppliedFilters(index) = item
                Else
                    AppliedFilters.Add(New UserGridColumnFilter() With {
                        .Expression = filter.Expression,
                        .Operation = filter.Operation,
                        .DataType = filter.DataType,
                        .Value = filter.Value
                    })
                End If
                SaveUserSettings(conn)
            End Sub

            Public Sub RemoveFilter(ByVal expression As String, Optional ByVal conn As SqlConnection = Nothing)
                Dim colFilters = (From f In AppliedFilters Where f.Expression = expression)
                If colFilters.Count() = 1 Then
                    Dim item = colFilters.FirstOrDefault()
                    AppliedFilters.Remove(item)
                End If
                SaveUserSettings(conn)
            End Sub

            Public Function GetSearchValue() As String
                Return UserSettings.SearchValue
            End Function

            Public Function GetAppliedFilters() As List(Of UserGridColumnFilter)
                Return AppliedFilters
            End Function

            Public Sub ClearFilters(Optional ByVal conn As SqlConnection = Nothing)
                AppliedFilters.Clear()
                SaveUserSettings(conn)
            End Sub

            Public Sub ResetUserSettings(Optional ByVal conn As SqlConnection = Nothing)
                UserGridController.DeleteGridSettings(UserSettings.UserId, UserSettings.BusinessClass, conn)
            End Sub

#End Region

        End Class
    End Namespace
End Namespace
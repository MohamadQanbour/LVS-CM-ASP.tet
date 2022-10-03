Imports System.Data
Imports System.Data.SqlClient
Imports System.Xml
Imports EGV.Structures
Imports EGV.Utils

Namespace EGV
    Namespace Business

        'Object
        Public Class UserGrid
            Inherits PrimeBusinessBase

#Region "Public Properties"

            Public Property UserId As Integer
            Public Property BusinessClass As String

            Public Property Columns As List(Of UserGridColumn)
            Public Property Filters As List(Of UserGridColumnFilter)
            Public Property SearchValue As String
            Public Property PageSize As Integer
            Public Property PageIndex As Integer
            Public Property SortColumns As List(Of UserGridSortColumn)

#End Region

#Region "Constructors"

            Public Sub New()
                Columns = New List(Of UserGridColumn)()
                SortColumns = New List(Of UserGridSortColumn)()
                Filters = New List(Of UserGridColumnFilter)()
            End Sub

#End Region

        End Class

        'Controller
        Public Class UserGridController

#Region "Public Methods"

            Public Shared Function Exists(ByVal userId As Integer, ByVal businessClass As String, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM SYS_UserGrid WHERE UserId = @Id AND BusinessClass = @Class"
                Return DBA.Scalar(conn, q,
                                  DBA.CreateParameter("Id", SqlDbType.Int, userId),
                                  DBA.CreateParameter("Class", SqlDbType.VarChar, businessClass, 50)
                                  ) = 1
            End Function

            Public Shared Function LoadGridSettings(ByVal userId As Integer, ByVal businessClass As String, Optional ByVal conn As SqlConnection = Nothing) As UserGrid
                Dim q As String = "SELECT * FROM SYS_UserGrid WHERE UserId = @Id AND BusinessClass = @Class"
                Dim dr As DataRow = DBA.DataRow(conn, q,
                                                    DBA.CreateParameter("Id", SqlDbType.Int, userId),
                                                    DBA.CreateParameter("Class", SqlDbType.VarChar, businessClass, 50)
                                                    )
                Dim ret As UserGrid = Deserialize(Helper.GetSafeDBValue(dr("GridProperties")))
                ret.UserId = Helper.GetSafeDBValue(dr("UserId"), Enums.ValueTypes.TypeInteger)
                ret.BusinessClass = Helper.GetSafeDBValue(dr("BusinessClass"))
                Return ret
            End Function

            Public Shared Sub SaveGridSettings(ByVal obj As UserGrid, Optional ByVal conn As SqlConnection = Nothing)
                Dim sp As String = String.Empty
                If Exists(obj.UserId, obj.BusinessClass, conn) Then sp = "SYS_UserGrid_Update" Else sp = "SYS_UserGrid_Insert"
                DBA.SPNonQuery(conn, sp,
                    DBA.CreateParameter("UserId", SqlDbType.Int, obj.UserId),
                    DBA.CreateParameter("Class", SqlDbType.VarChar, obj.BusinessClass, 50),
                    DBA.CreateParameter("Properties", SqlDbType.NText, Serialize(obj))
                )
            End Sub

            Public Shared Sub DeleteGridSettings(ByVal userId As Integer, ByVal businessClass As String, Optional ByVal conn As SqlConnection = Nothing)
                Dim q As String = "DELETE FROM SYS_UserGrid WHERE UserId = @Id AND BusinessClass = @Class"
                DBA.NonQuery(conn, q,
                             DBA.CreateParameter("Id", SqlDbType.Int, userId),
                             DBA.CreateParameter("Class", SqlDbType.VarChar, businessClass, 50)
                )
            End Sub

#End Region

#Region "Private Methods"

            Private Shared Function Deserialize(ByVal settings As String) As UserGrid
                Dim ret As New UserGrid()
                Dim xml As New XmlDocument()
                Dim loaded As Boolean = True
                Try
                    xml.LoadXml(settings)
                Catch ex As Exception
                    loaded = False
                End Try
                If loaded Then
                    Dim root As XmlNode = xml.SelectSingleNode("Grid")
                    Dim columns As XmlNode = root.SelectSingleNode("Columns")
                    If columns IsNot Nothing Then
                        For Each c As XmlNode In columns.SelectNodes("Column")
                            Dim col As New UserGridColumn() With {
                                .Name = Helper.GetSafeXML(c, "Name"),
                                .Alias = Helper.GetSafeXML(c, "Alias"),
                                .Visible = Helper.GetSafeXML(c, "Visible", Enums.ValueTypes.TypeBoolean),
                                .Order = Helper.GetSafeXML(c, "Order", Enums.ValueTypes.TypeInteger)
                            }
                            ret.Columns.Add(col)
                        Next
                    End If
                    Dim nfilters As XmlNode = root.SelectSingleNode("Filters")
                    If nfilters IsNot Nothing Then
                        For Each filter As XmlNode In nfilters.SelectNodes("Filter")
                            Dim f As New UserGridColumnFilter() With {
                                .Operation = Helper.GetSafeXML(filter, "Operation", Enums.ValueTypes.TypeInteger),
                                .Value = Helper.GetSafeXML(filter, "Value"),
                                .Expression = Helper.GetSafeXML(filter, "Expression"),
                                .DataType = Helper.GetSafeXML(filter, "DataType", Enums.ValueTypes.TypeInteger)
                            }
                            ret.Filters.Add(f)
                        Next
                    End If
                    ret.SearchValue = Helper.GetSafeXML(root, "Search")
                    Dim sett As XmlNode = root.SelectSingleNode("Settings")
                    If sett IsNot Nothing Then
                        ret.PageSize = Helper.GetSafeXML(sett, "PageSize", Enums.ValueTypes.TypeInteger)
                        ret.PageIndex = Helper.GetSafeXML(sett, "PageIndex", Enums.ValueTypes.TypeInteger)
                    End If
                    Dim sort As XmlNode = root.SelectSingleNode("Sorting")
                    If sort IsNot Nothing Then
                        For Each s As XmlNode In sort.SelectNodes("Column")
                            ret.SortColumns.Add(New UserGridSortColumn() With {
                                .Name = Helper.GetSafeXML(s, "Name"),
                                .Alias = Helper.GetSafeXML(s, "Alias"),
                                .Direction = Helper.GetSafeXML(s, "Order", Enums.ValueTypes.TypeInteger)
                            })
                        Next
                    End If
                End If
                Return ret
            End Function

            Private Shared Function Serialize(ByVal obj As UserGrid) As String
                Dim xml As New XmlDocument()
                Dim root As XmlNode = xml.CreateElement("Grid")
                'columns
                Dim columns As XmlNode = xml.CreateElement("Columns")
                For Each c As UserGridColumn In obj.Columns
                    Dim col As XmlNode = xml.CreateElement("Column")
                    'name
                    Dim name As XmlNode = xml.CreateElement("Name")
                    name.InnerText = c.Name
                    col.AppendChild(name)
                    'alias
                    Dim a As XmlNode = xml.CreateElement("Alias")
                    a.InnerText = c.Alias
                    col.AppendChild(a)
                    'visible
                    Dim v As XmlNode = xml.CreateElement("Visible")
                    v.InnerText = IIf(c.Visible, "true", "false")
                    col.AppendChild(v)
                    'order
                    Dim o As XmlNode = xml.CreateElement("Order")
                    o.InnerText = c.Order
                    col.AppendChild(o)
                    'add
                    columns.AppendChild(col)
                Next
                root.AppendChild(columns)
                'filters
                Dim fs As XmlNode = xml.CreateElement("Filters")
                For Each f As UserGridColumnFilter In obj.Filters
                    Dim nf As XmlNode = xml.CreateElement("Filter")
                    'operation
                    Dim op As XmlNode = xml.CreateElement("Operation")
                    op.InnerText = f.Operation
                    nf.AppendChild(op)
                    'value
                    Dim fv As XmlNode = xml.CreateElement("Value")
                    fv.InnerText = f.Value
                    nf.AppendChild(fv)
                    'expression
                    Dim fe As XmlNode = xml.CreateElement("Expression")
                    fe.InnerText = f.Expression
                    nf.AppendChild(fe)
                    'datatype
                    Dim ft As XmlNode = xml.CreateElement("DataType")
                    ft.InnerText = f.DataType
                    nf.AppendChild(ft)
                    'add
                    fs.AppendChild(nf)
                Next
                root.AppendChild(fs)
                'search
                Dim search As XmlNode = xml.CreateElement("Search")
                search.InnerText = obj.SearchValue
                root.AppendChild(search)
                'settings
                Dim sett As XmlNode = xml.CreateElement("Settings")
                'page size
                Dim ps As XmlNode = xml.CreateElement("PageSize")
                ps.InnerText = obj.PageSize
                sett.AppendChild(ps)
                'page index
                Dim pi As XmlNode = xml.CreateElement("PageIndex")
                pi.InnerText = obj.PageIndex
                sett.AppendChild(pi)
                root.AppendChild(sett)
                'order columns
                Dim sorting As XmlNode = xml.CreateElement("Sorting")
                For Each sc As UserGridSortColumn In obj.SortColumns
                    Dim col As XmlNode = xml.CreateElement("Column")
                    'name
                    Dim cn As XmlNode = xml.CreateElement("Name")
                    cn.InnerText = sc.Name
                    col.AppendChild(cn)
                    'alias
                    Dim ca As XmlNode = xml.CreateElement("Alias")
                    ca.InnerText = sc.Alias
                    col.AppendChild(ca)
                    'direction
                    Dim cd As XmlNode = xml.CreateElement("Order")
                    cd.InnerText = sc.Direction
                    col.AppendChild(cd)
                    sorting.AppendChild(col)
                Next
                root.AppendChild(sorting)
                xml.AppendChild(root)
                Return xml.InnerXml()
            End Function

#End Region

        End Class

    End Namespace
End Namespace
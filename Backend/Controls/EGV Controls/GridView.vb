Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.ComponentModel
Imports EGV.Business
Imports EGV.Enums
Imports EGV.Utils
Imports EGV.Structures
Imports EGV
Imports System.Data.SqlClient

Namespace EGVControls

    Public Class EGVGridView
        Inherits WebControl
        Implements INamingContainer

#Region "Events"

        Public Event GridNeedDataSource(ByVal sender As Object, ByVal e As EventArgs)
        Public Event RowDelete(ByVal key As String)
        Public Event ToolbarButtonClick(ByVal cmd As String)

#End Region

#Region "Private Properties"

        Private WithEvents grid As GridView
        Private WithEvents Filter As EGVFilter
        Private Property entity As EntityStructure
        Private Property txtSearch As TextBox
        Private Property TopPager As EGVPager
        Private Property BottomPager As EGVPager
        Private Property PageSizeButtonCollection As List(Of LinkButton)
        Private Property hdnSelectedColumns As HiddenField

        Private _Conn As SqlConnection = Nothing

        Private Property SearchTerm As String
        Private Property TotalRecords As Integer

#End Region

#Region "Public Properties"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        Public Property BusinessClass As String = String.Empty

        Public Property Type As BoxTypes = BoxTypes.Default
        Public Property Collapsable As Boolean = False
        Public Property Solidate As Boolean = False
        Public Property IconClass As String = String.Empty
        Public Property Title As String = String.Empty
        Public Property HeaderBorder As Boolean = True

        Public Property AllowSelection As Boolean = True
        Public Property AllowSearch As Boolean = False
        Public Property AllowRefresh As Boolean = True
        Public Property AllowColumnSelection As Boolean = False
        Public Property AllowFiltering As Boolean = False
        Public Property AllowPaging As Boolean = False
        Public Property PagingPosition As PagingPositions = PagingPositions.Bottom
        Public Property AllowChangePageSize As Boolean = False
        Public Property AllowSorting As Boolean = False
        Public Property AllowChangeColumnOrder As Boolean = False

        Public Property EditRecordLink As String = String.Empty

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property Toolbar As Toolbar

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property GridFooterTools As Panel

#End Region

#Region "Constructors"

        Public Sub New()
            grid = New GridView()
            Toolbar = New Toolbar()
            Filter = New EGVFilter()
            PageSizeButtonCollection = New List(Of LinkButton)()
        End Sub

#End Region

#Region "Public Methods"

        Public Sub AddCondition(ByVal condition As String)
            entity.AddCondition(condition)
        End Sub

        Public Sub HideToolbarButton(ByVal id As String)
            Toolbar.Hide(id)
        End Sub

        Public Sub BindGrid(Optional ByVal conn As SqlConnection = Nothing)
            DBA.GetSafeConn(conn)
            _Conn = conn
            Dim Query As CustomQuery = entity.GetCustomQuery(conn)
            If SearchTerm <> String.Empty Then AddSearchToQuery(Query)
            TotalRecords = Query.ExecuteCount()
            grid.DataSource = Query.Execute()
            grid.DataBind()
            If TopPager IsNot Nothing Then
                TopPager.BindPager(TotalRecords, entity.PageSize, entity.PageIndex)
            End If
            If BottomPager IsNot Nothing Then
                BottomPager.BindPager(TotalRecords, entity.PageSize, entity.PageIndex)
            End If
        End Sub

        Public Function GetEditableColumns() As List(Of EditableColumn)
            Dim lst As New List(Of EditableColumn)
            Dim colIndices As New List(Of Integer)
            Dim entityIndices As New List(Of Integer)
            For Each c In From col In entity.Columns Where col.Layout.EditControl = True
                Dim fc As DataControlField = (From gc As DataControlField In grid.Columns Where gc.HeaderText = c.Resource AndAlso gc IsNot GetType(ClientSelectColumn) Select gc).FirstOrDefault()
                colIndices.Add(grid.Columns.IndexOf(fc))
                entityIndices.Add(entity.Columns.IndexOf(c))
            Next
            For Each row As GridViewRow In From r As GridViewRow In grid.Rows Where r.RowType = DataControlRowType.DataRow
                For idx As Integer = 0 To colIndices.Count - 1
                    Dim i As Integer = colIndices(idx)
                    Dim ci As Integer = entityIndices(idx)
                    Dim target As TableCell = row.Cells(i)
                    Dim col As ESColumn = entity.Columns(ci)
                    If target.HasControls() Then
                        Dim ctrl As WebControl = (From c As WebControl In target.Controls Where c.Visible = True).FirstOrDefault()
                        Select Case ctrl.GetType().Name
                            Case "TextBox", "EGVTextBox"
                                Dim txt = DirectCast(ctrl, TextBox)
                                lst.Add(New EditableColumn() With {
                                        .ColumnName = txt.Attributes("data-field").ToString(),
                                        .ColumnKey = txt.Attributes("data-key").ToString(),
                                        .ColumnValue = txt.Text
                                    })
                            Case "DropDownList", "EGVDropDown"
                                Dim ddl = DirectCast(ctrl, DropDownList)
                                lst.Add(New EditableColumn() With {
                                        .ColumnName = ddl.Attributes("data-field").ToString(),
                                        .ColumnKey = ddl.Attributes("data-key").ToString(),
                                        .ColumnValue = ddl.SelectedValue
                                    })
                            Case "CheckBox", "EGVCheckBox"
                                Dim chk = DirectCast(ctrl, CheckBox)
                                lst.Add(New EditableColumn() With {
                                        .ColumnName = chk.Attributes("data-field").ToString(),
                                        .ColumnKey = chk.Attributes("data-key").ToString(),
                                        .ColumnValue = chk.Checked.ToString().ToLower()
                                    })
                        End Select
                    End If
                Next
            Next
                Return lst
        End Function

        Public Function GetSelectedRows() As List(Of GridViewRow)
            Dim rows As New List(Of GridViewRow)
            Dim colIndex As Integer = grid.Columns.IndexOf((From col In grid.Columns Where TypeOf col Is ClientSelectColumn).FirstOrDefault())
            For Each row As GridViewRow In grid.Rows
                Dim chk As CheckBox = DirectCast(row.Cells(colIndex).FindControl("chkBxSelect"), CheckBox)
                If chk IsNot Nothing AndAlso chk.Checked Then rows.Add(row)
            Next
            Return rows
        End Function

        Public Function GetSelectedIds() As List(Of String)
            Dim ids As New List(Of String)
            Dim colIndex As Integer = grid.Columns.IndexOf((From col In grid.Columns Where TypeOf col Is ClientSelectColumn).FirstOrDefault())
            For Each row As GridViewRow In grid.Rows
                Dim chk As CheckBox = DirectCast(row.Cells(colIndex).FindControl("chkBxSelect"), CheckBox)
                If chk IsNot Nothing AndAlso chk.Checked Then ids.Add(chk.Attributes("data-key"))
            Next
            Return ids
        End Function

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            Dim classes As New List(Of String)
            classes.Add("box")
            classes.Add(GetTypeClass())
            If Solidate Then classes.Add("box-solid")
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & String.Join(" ", classes.ToArray())
            If entity Is Nothing AndAlso BusinessClass <> String.Empty Then
                Dim userId As Integer = 0
                If Helper.CMSAuthUser IsNot Nothing Then userId = Helper.CMSAuthUser.Id
                entity = New EntityStructure(BusinessClass, userId)
                SetupProperties()
            End If
            If Helper.CMSAuthUser IsNot Nothing Then EnsureChildControls()
        End Sub

        Protected Overrides Sub CreateChildControls()
            BuildHeader(Controls)
            BuildBody(Controls)
            BuildFooter(Controls)
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            EGVScriptManager.AddStyle(Path.MapCMSCss("icheck"))
            EGVScriptManager.AddScripts(False, Path.MapCMSScript("icheck.min"), Path.MapCMSScript("lib/GridView"))
            If (From c In entity.Columns Where c.Layout.DisplayType = DisplayTypes.InlineImage).Count() > 0 Then
                EGVScriptManager.AddStyle(Path.MapCMSCss("prettyPhoto"))
                EGVScriptManager.AddScript(Path.MapCMSScript("jquery.prettyPhoto"))
            End If
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
            If txtSearch IsNot Nothing Then
                Dim searchTerm As String = txtSearch.Text
                Me.SearchTerm = searchTerm
                entity.AddSearchValue(searchTerm)
                entity.PageIndex = 0
                RaiseEvent GridNeedDataSource(sender, e)
            End If
        End Sub

        Protected Sub toolbar_Click(ByVal cmd As String)
            RaiseEvent ToolbarButtonClick(cmd)
        End Sub

        Protected Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs)
            RaiseEvent GridNeedDataSource(sender, e)
        End Sub

        Protected Sub btnApplyColumnVisible_Click(ByVal sender As Object, ByVal e As EventArgs)
            If hdnSelectedColumns IsNot Nothing Then
                Dim lst As List(Of JSSelectedColumn) = Helper.JSDeserialize(Of List(Of JSSelectedColumn))(hdnSelectedColumns.Value)
                entity.ToggleColumnVisibile(lst)
            End If
            Helper.Response.Redirect(Helper.Request.RawUrl)
        End Sub

        Protected Sub btnReset_Click(ByVal sender As Object, ByVal e As EventArgs)
            entity.ResetUserSettings()
            Helper.Response.Redirect(Helper.Request.RawUrl)
        End Sub

        Protected Sub btnChangePageSize_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim size As Integer = DirectCast(sender, LinkButton).Attributes("targetpagesize")
            entity.ChangePageSize(size)
            For Each a As LinkButton In PageSizeButtonCollection
                Dim cur As Integer = a.Attributes("targetpagesize")
                Dim lbl As Label = a.FindControl("CheckLabel" & cur)
                lbl.Visible = cur = size
            Next
            entity.PageIndex = 0
            RaiseEvent GridNeedDataSource(sender, e)
        End Sub

        Protected Sub PageIndexChanged(ByVal sender As Object, ByVal e As EventArgs, ByVal newPage As Integer)
            entity.ChangePageIndex(newPage)
            RaiseEvent GridNeedDataSource(sender, e)
        End Sub

        Protected Sub DoSort(ByVal sender As Object, ByVal e As EventArgs)
            Dim lnk As LinkButton = DirectCast(sender, LinkButton)
            Dim expression As String = lnk.Attributes("sortexpression")
            Dim dir As SortDirections = lnk.Attributes("sortdirection")
            Dim sortAlias As String = lnk.Attributes("sortalias")
            Dim remove As Boolean = lnk.Attributes("removesort")
            If remove Then
                entity.RemoveSortColumn(expression, sortAlias)
            Else
                entity.ClearSort()
                entity.AddSortColumn(expression, sortAlias, dir)
            End If
            RaiseEvent GridNeedDataSource(sender, e)
        End Sub

        Protected Sub ChangeOrder(ByVal sender As Object, ByVal e As EventArgs)
            Dim lnk As LinkButton = DirectCast(sender, LinkButton)
            Dim col As String = lnk.Attributes("colname")
            Dim colAlias As String = lnk.Attributes("colalias")
            Dim newOrder As Integer = lnk.Attributes("newcolorder")
            entity.ChangeColOrder(col, colAlias, newOrder)
            Helper.Response.Redirect(Helper.Request.RawUrl)
        End Sub

        Protected Sub applyFilter(ByVal lst As List(Of UserGridColumnFilter)) Handles Filter.ApplyFilters
            entity.ClearFilters()
            For Each item As UserGridColumnFilter In lst
                entity.AddFilter(item)
            Next
            entity.PageIndex = 0
            RaiseEvent GridNeedDataSource(grid, New EventArgs())
        End Sub

#End Region

#Region "Private Methods"

        Private Function GetTypeClass()
            Select Case Type
                Case BoxTypes.Primary
                    Return "box-primary"
                Case BoxTypes.Info
                    Return "box-info"
                Case BoxTypes.Warning
                    Return "box-warning"
                Case BoxTypes.Success
                    Return "box-success"
                Case BoxTypes.Danger
                    Return "box-danger"
                Case Else
                    Return "box-default"
            End Select
        End Function

        Private Function GetSearchTypeString(ByVal type As SearchTypes)
            Select Case type
                Case SearchTypes.Contains
                    Return "LIKE N'%{0}%'"
                Case SearchTypes.StartsWith
                    Return "LIKE N'{0}%'"
                Case SearchTypes.EndsWith
                    Return "LIKE N'%{0}'"
                Case SearchTypes.Exact
                    Return "= N'{0}'"
                Case SearchTypes.Equal
                    Return "= {0}"
                Case SearchTypes.GreaterThan
                    Return "> {0}"
                Case SearchTypes.GreaterThanOrEqual
                    Return ">= {0}"
                Case SearchTypes.LessThan
                    Return "< {0}"
                Case SearchTypes.LessThanOrEqual
                    Return "<= {0}"
                Case SearchTypes.NotEqual
                    Return "<> {0}"
                Case Else
                    Return "= {0}"
            End Select
        End Function

        Private Function GetUnit(ByVal source As String) As Unit
            If source.EndsWith("%") Then Return Unit.Percentage(source.Replace("%", ""))
            If source.EndsWith("px") Then Return Unit.Pixel(source.Replace("px", ""))
        End Function

        Private Sub AddSearchToQuery(ByVal q As CustomQuery)
            Dim lst As New List(Of String)
            For Each c As ESSearchColumn In entity.SearchColumns
                lst.Add(c.Name & " " & String.Format(GetSearchTypeString(c.Type), SearchTerm))
            Next
            q.AddCondition(String.Join(" OR ", lst.ToArray()))
        End Sub

        Private Function GetAlignText(ByVal align As ControlAligns) As String
            Select Case align
                Case ControlAligns.Right
                    Return "text-right"
                Case ControlAligns.Center
                    Return "text-center"
                Case Else
                    Return "text-left"
            End Select
        End Function

#End Region

#Region "Rendering Methods"

        Private Sub SetupProperties()
            AllowSelection = entity.EnableSelection
            AllowSearch = entity.EnableSearch
            AllowPaging = entity.EnablePaging
            AllowRefresh = entity.EnableRefresh
            AllowColumnSelection = entity.EnableColumnSelection
            AllowFiltering = entity.EnableFiltering
            AllowChangePageSize = entity.EnableChangePageSize
            AllowSorting = entity.EnableSorting
            AllowChangeColumnOrder = entity.EnableChangeColumnOrder
            EditRecordLink = entity.EditRecordLink
            PagingPosition = entity.PagingPosition
            SearchTerm = entity.GetSearchValue()
            SetupFilters()
        End Sub

        Private Sub SetupFilters()
            If AllowFiltering Then
                For Each c As ESColumn In (From col In entity.Columns Where col.Filter.Allow = True)
                    Filter.AddFilter(c)
                Next
                For Each c As UserGridColumnFilter In entity.GetAppliedFilters()
                    Filter.ApplyFilter(c.Expression, c.Operation, c.Value, c.DataType)
                Next
            End If
        End Sub

        Private Sub BuildHeader(ByVal parent As ControlCollection)
            'box header
            Dim header As New Panel()
            header.CssClass = "box-header" & IIf(HeaderBorder, " with-border", "")
            If IconClass <> Nothing AndAlso IconClass <> String.Empty Then
                Dim headIcon As New HtmlGenericControl("span")
                headIcon.Attributes.Add("class", IconClass)
                header.Controls.Add(headIcon)
            End If
            If Title <> Nothing AndAlso Title <> String.Empty Then
                If Title.StartsWith("Resources.") Then Title = Localization.GetResource(Title)
                Dim titleContainer As New HtmlGenericControl("h3")
                titleContainer.Attributes.Add("class", "box-title")
                titleContainer.Controls.Add(New LiteralControl(Title))
                header.Controls.Add(titleContainer)
            End If
            BuildTools(header.Controls)
            parent.Add(header)
        End Sub

        Private Sub BuildBody(ByVal parent As ControlCollection)
            Dim body As New Panel()
            body.CssClass = "box-body table-responsive egv-grid-box-body"
            BuildGridTools(body.Controls)
            AddHandler Toolbar.ToolbarButtonClick, AddressOf toolbar_Click
            Toolbar.CssClass &= IIf(Toolbar.CssClass <> String.Empty, " ", "") & "pull-left"
            body.Controls.Add(Toolbar)
            If entity IsNot Nothing Then
                grid.CssClass = "table table-hover egvGrid"
                SetupGrid()
                If AllowFiltering Then
                    body.Controls.Add(Filter)
                End If
                body.Controls.Add(grid)
            End If
            parent.Add(body)
        End Sub

        Private Sub BuildFooter(ByVal parent As ControlCollection)
            Dim footer As New Panel()
            footer.CssClass = "box-footer clearfix"
            If AllowPaging AndAlso (PagingPosition = PagingPositions.Bottom OrElse PagingPosition = PagingPositions.TopAndBottom) Then
                BottomPager = New EGVPager()
                BottomPager.IsFullPager = True
                AddHandler BottomPager.PageIndexChanged, AddressOf PageIndexChanged
                footer.Controls.Add(BottomPager)
            End If
            If GridFooterTools IsNot Nothing Then
                footer.Controls.Add(GridFooterTools)
                GridFooterTools.CssClass = "text-right"
            End If
            If footer.HasControls() Then parent.Add(footer)
        End Sub

        Private Sub BuildGridTools(ByVal parent As ControlCollection)
            Dim tools As New Panel()
            tools.CssClass = "pull-right egv-grid-tools btn-toolbar"
            If AllowRefresh OrElse AllowFiltering OrElse AllowColumnSelection Then
                Dim pnl As New Panel()
                pnl.CssClass = "btn-group"
                pnl.Attributes.Add("role", "group")
                If AllowRefresh Then
                    BuildRefresh(pnl.Controls)
                End If
                If AllowFiltering Then
                    BuildFilter(pnl.Controls)
                End If
                If AllowColumnSelection Then
                    BuildColumnSelection(pnl.Controls)
                End If
                tools.Controls.Add(pnl)
            End If
            If AllowFiltering OrElse AllowColumnSelection OrElse AllowChangeColumnOrder OrElse AllowSorting OrElse
                    AllowPaging OrElse AllowChangePageSize Then
                Dim pnl As New Panel()
                pnl.CssClass = "btn-group"
                pnl.Attributes.Add("role", "group")
                Dim btn As New EGVLinkButton()
                btn.Color = ControlColors.Maroon
                btn.Size = ButtonSizes.Small
                btn.ToolTip = Localization.GetResource("Resources.Global.CMS.ResetUserSettings")
                btn.OnClientClick = "return confirm('" & Localization.GetResource("Resources.Global.CMS.ResetConfirm") & "');"
                AddHandler btn.Click, AddressOf btnReset_Click
                Dim icon As New Label()
                icon.CssClass = "fa fa-repeat"
                btn.Controls.Add(icon)
                pnl.Controls.Add(btn)
                tools.Controls.Add(pnl)
            End If
            If AllowChangePageSize Then
                BuildChangePageSize(tools.Controls)
            End If
            If AllowPaging AndAlso (PagingPosition = PagingPositions.Top OrElse PagingPosition = PagingPositions.TopAndBottom) Then
                TopPager = New EGVPager()
                TopPager.IsFullPager = False
                AddHandler TopPager.PageIndexChanged, AddressOf PageIndexChanged
                tools.Controls.Add(TopPager)
            End If
            parent.Add(tools)
        End Sub

        Private Sub BuildTools(ByVal parent As ControlCollection)
            Dim tools As New Panel()
            tools.CssClass = "box-tools pull-right"
            If Collapsable Then
                Dim btnCollapse As New HtmlGenericControl("button")
                btnCollapse.Attributes.Add("class", "btn btn-box-tool pull-right")
                btnCollapse.Attributes.Add("data-widget", "collapse")
                btnCollapse.Attributes.Add("data-toggle", "tooltip")
                btnCollapse.Attributes.Add("title", Localization.GetResource("Resources.Global.CMS.CollapseExpand"))
                Dim iconCollapse As New HtmlGenericControl("span")
                iconCollapse.Attributes.Add("class", "fa fa-minus")
                btnCollapse.Controls.Add(iconCollapse)
                tools.Controls.Add(btnCollapse)
            End If
            If AllowSearch Then
                BuildSearch(tools.Controls)
            End If
            parent.Add(tools)
        End Sub

        Private Sub BuildSearch(ByVal parent As ControlCollection)
            Dim search As New Panel()
            search.DefaultButton = "btnSubmitSearch"
            search.CssClass = "input-group input-group-sm pull-right"
            search.Style.Add("width", Unit.Pixel(150).ToString())
            txtSearch = New TextBox()
            txtSearch.CssClass = "form-control pull-right"
            txtSearch.Attributes.Add("placeholder", Localization.GetResource("Resources.Global.CMS.Search"))
            If SearchTerm <> String.Empty Then txtSearch.Text = SearchTerm
            search.Controls.Add(txtSearch)
            Dim btnContainer As New HtmlGenericControl("div")
            btnContainer.Attributes.Add("class", "input-group-btn")
            Dim btnSubmit As New LinkButton()
            btnSubmit.ID = "btnSubmitSearch"
            btnSubmit.CssClass = "btn btn-default"
            Dim icon As New HtmlGenericControl("span")
            icon.Attributes.Add("class", "fa fa-search")
            AddHandler btnSubmit.Click, AddressOf btnSearch_Click
            btnSubmit.Controls.Add(icon)
            btnContainer.Controls.Add(btnSubmit)
            search.Controls.Add(btnContainer)
            parent.Add(search)
        End Sub

        Private Sub AddColumn(ByVal ctrl As DataControlField, ByVal c As ESColumn)
            ctrl.HeaderText = c.Resource
            If (AllowSorting OrElse AllowChangeColumnOrder) AndAlso (c.AllowSort OrElse c.AllowReorder) Then
                ctrl.HeaderStyle.CssClass &= IIf(ctrl.HeaderStyle.CssClass <> String.Empty, " ", "") & "sortable-header data-" & c.TableAlias & "-" & c.Name
            End If
            If c.Layout.Width <> String.Empty AndAlso c.Layout.Width <> "*" Then
                ctrl.HeaderStyle.Width = GetUnit(c.Layout.Width)
                ctrl.ItemStyle.Width = GetUnit(c.Layout.Width)
            End If
            If c.Layout.IsPrimary Then ctrl.ItemStyle.CssClass = IIf(ctrl.ItemStyle.CssClass <> String.Empty, " ", "") & "bg-olive"
            If c.Layout.HeaderAlign > 0 Then
                ctrl.HeaderStyle.CssClass &= IIf(ctrl.HeaderStyle.CssClass <> String.Empty, " ", "") & GetAlignText(c.Layout.HeaderAlign)
                ctrl.ItemStyle.CssClass &= IIf(ctrl.ItemStyle.CssClass <> String.Empty, " ", "") & GetAlignText(c.Layout.ItemAlign)
            End If
            grid.Columns.Add(ctrl)
        End Sub

        Private Sub BuildRefresh(ByVal parent As ControlCollection)
            Dim btn As New LinkButton()
            btn.CssClass = "btn bg-olive btn-sm"
            btn.ToolTip = Localization.GetResource("Resources.Global.CMS.Refresh")
            AddHandler btn.Click, AddressOf btnRefresh_Click
            Dim icon As New Label()
            icon.CssClass = "fa fa-refresh"
            btn.Controls.Add(icon)
            parent.Add(btn)
        End Sub

        Private Sub BuildFilter(ByVal parent As ControlCollection)
            Dim btn As New HyperLink()
            btn.NavigateUrl = "javascript:;"
            btn.Attributes.Add("role", "toggle-filter")
            btn.CssClass = "btn btn-primary btn-sm"
            btn.ToolTip = Localization.GetResource("Resources.Global.CMS.FilterRow")
            Dim icon As New Label()
            icon.CssClass = "fa fa-filter"
            btn.Controls.Add(icon)
            parent.Add(btn)
        End Sub

        Private Sub BuildColumnSelection(ByVal parent As ControlCollection)
            Dim col As New Panel()
            col.CssClass = "btn-group"
            col.Attributes.Add("role", "group")
            Dim toggle As New HyperLink()
            toggle.NavigateUrl = "javascript:;"
            toggle.CssClass = "btn bg-purple dropdown-toggle btn-sm"
            toggle.Attributes.Add("data-toggle", "dropdown")
            toggle.Attributes.Add("aria-haspopup", "true")
            toggle.Attributes.Add("aria-expanded", "false")
            toggle.ToolTip = Localization.GetResource("Resources.Global.CMS.SelectColumns")
            Dim icon As New Label()
            icon.CssClass = "fa fa-th-list"
            toggle.Controls.Add(icon)
            col.Controls.Add(toggle)
            Dim lst As New HtmlGenericControl("UL")
            lst.Attributes.Add("class", "dropdown-menu dropdown-menu-right egv-col-select")
            Dim visibleCols As New List(Of JSSelectedColumn)
            For Each c As ESColumn In entity.Columns
                Dim item As New HtmlGenericControl("LI")
                Dim a As New HyperLink()
                a.Attributes.Add("data-columnname", c.Name)
                a.Attributes.Add("data-columnalias", c.TableAlias)
                a.NavigateUrl = "javascript:;"
                If c.Visible Then
                    a.Attributes.Add("class", "bg-olive")
                End If
                a.Text = IIf(c.Resource <> String.Empty, c.Resource, c.Name)
                item.Controls.Add(a)
                If c.Visible Then visibleCols.Add(New JSSelectedColumn() With {.ColumnAlias = c.TableAlias, .ColumnName = c.Name})
                lst.Controls.Add(item)
            Next
            Dim lastitem As New HtmlGenericControl("LI")
            Dim apply As New LinkButton()
            apply.CssClass = "btn btn-block btn-flat bg-purple"
            apply.Text = Localization.GetResource("Resources.Global.CMS.Apply")
            AddHandler apply.Click, AddressOf btnApplyColumnVisible_Click
            hdnSelectedColumns = New HiddenField()
            hdnSelectedColumns.ID = "hdnVisibleColumns"
            hdnSelectedColumns.ClientIDMode = ClientIDMode.Static
            hdnSelectedColumns.Value = Helper.JSSerialize(visibleCols)
            lastitem.Controls.Add(hdnSelectedColumns)
            lastitem.Controls.Add(apply)
            lst.Controls.Add(lastitem)
            col.Controls.Add(lst)
            parent.Add(col)
        End Sub

        Private Sub BuildChangePageSize(ByVal parent As ControlCollection)
            Dim pnl As New Panel()
            pnl.CssClass = "btn-group"
            pnl.Attributes.Add("role", "group")
            Dim subPnl As New Panel()
            subPnl.CssClass = "btn-group"
            subPnl.Attributes.Add("role", "group")
            Dim toggle As New HyperLink()
            toggle.NavigateUrl = "javascript:;"
            toggle.CssClass = "btn btn-default dropdown-toggle btn-sm"
            toggle.Attributes.Add("data-toggle", "dropdown")
            toggle.Attributes.Add("aria-haspopup", "true")
            toggle.Attributes.Add("aria-expanded", "false")
            toggle.Controls.Add(New LiteralControl(Localization.GetResource("Resources.Global.CMS.PageSize") & "&nbsp;"))
            Dim caret As New Label()
            caret.CssClass = "caret"
            toggle.Controls.Add(caret)
            subPnl.Controls.Add(toggle)
            Dim lst As New HtmlGenericControl("UL")
            lst.Attributes.Add("class", "dropdown-menu dropdown-menu-right")
            Dim allowed() As Integer = {5, 10, 25, 50, 100}
            For Each item As Integer In allowed
                Dim li As New HtmlGenericControl("LI")
                Dim a As New LinkButton()
                a.Attributes.Add("targetpagesize", item)
                a.Controls.Add(New LiteralControl(item))
                Dim lbl As New Label()
                lbl.ID = "CheckLabel" & item
                lbl.CssClass = "fa fa-check text-success egv-check"
                lbl.Visible = entity.PageSize = item
                a.Controls.Add(lbl)
                AddHandler a.Click, AddressOf btnChangePageSize_Click
                PageSizeButtonCollection.Add(a)
                li.Controls.Add(a)
                lst.Controls.Add(li)
            Next
            subPnl.Controls.Add(lst)
            pnl.Controls.Add(subPnl)
            parent.Add(pnl)
        End Sub

        Private Sub BuildSortingTools(ByVal parent As ControlCollection, ByVal c As ESColumn, Optional ByVal isFirst As Boolean = False)
            Dim orderCol = (From col In entity.OrderColumns Where (col.SortColumnAlias & "." & col.SortColumn) = (c.TableAlias & "." & c.Name))
            If orderCol.Count = 1 Then
                Dim col As DataSort = orderCol.FirstOrDefault()
                Dim lbl As New Label()
                lbl.CssClass = "egv-sort-indicator fa fa-sort-amount-" & IIf(col.SortDirection = SortDirections.Ascending, "asc", "desc")
                parent.Add(lbl)
            End If
            parent.Add(New LiteralControl(IIf(c.Resource <> String.Empty, c.Resource, c.Name)))
            Dim tools As New Panel()
            tools.CssClass = "btn-toolbar egv-sort-tools"
            Dim group As New Panel()
            group.CssClass = "btn-group"
            group.Attributes.Add("role", "group")
            tools.Controls.Add(group)
            Dim toggle As New HyperLink()
            toggle.NavigateUrl = "javascript:;"
            toggle.CssClass = "btn btn-primary btn-xs dropdown-toggle"
            toggle.Attributes.Add("data-toggle", "dropdown")
            toggle.Attributes.Add("aria-haspopup", "true")
            toggle.Attributes.Add("aria-expanded", "false")
            group.Controls.Add(toggle)
            Dim icon As New Label()
            icon.CssClass = "fa fa-sort"
            toggle.Controls.Add(icon)
            Dim dropdown As New HtmlGenericControl("UL")
            dropdown.Attributes.Add("class", "dropdown-menu" & IIf(Not isFirst, " dropdown-menu-right", ""))
            If AllowSorting AndAlso c.AllowSort Then
                'sort ascending
                Dim asc As New HtmlGenericControl("LI")
                AddSortLink(asc.Controls, c, SortDirections.Ascending, False, "fa fa-sort-amount-asc", "SortAscending")
                dropdown.Controls.Add(asc)
                'sort descending
                Dim desc As New HtmlGenericControl("LI")
                AddSortLink(desc.Controls, c, SortDirections.Descending, False, "fa fa-sort-amount-desc", "SortDescending")
                dropdown.Controls.Add(desc)
                'remove sort
                Dim r As New HtmlGenericControl("LI")
                AddSortLink(r.Controls, c, SortDirections.Ascending, True, "fa fa-times", "RemoveSort")
                dropdown.Controls.Add(r)
            End If
            If AllowSorting AndAlso AllowChangeColumnOrder AndAlso c.AllowSort AndAlso c.AllowReorder Then
                Dim sep As New HtmlGenericControl("LI")
                sep.Attributes.Add("role", "separator")
                sep.Attributes.Add("class", "divider")
                dropdown.Controls.Add(sep)
            End If
            If AllowChangeColumnOrder AndAlso c.AllowReorder Then
                'move left
                Dim moveLeft As New HtmlGenericControl("LI")
                AddOrderLink(moveLeft.Controls, c, False)
                dropdown.Controls.Add(moveLeft)
                'move right
                Dim moveRight As New HtmlGenericControl("LI")
                AddOrderLink(moveRight.Controls, c, True)
                dropdown.Controls.Add(moveRight)
            End If
            group.Controls.Add(dropdown)
            parent.Add(tools)
        End Sub

        Private Sub AddSortLink(ByVal parent As ControlCollection, ByVal c As ESColumn, ByVal dir As SortDirections, ByVal remove As Boolean,
                                ByVal iconClass As String, ByVal resource As String)
            Dim lnk As New LinkButton()
            lnk.Attributes.Add("sortexpression", c.Name)
            lnk.Attributes.Add("sortdirection", dir)
            lnk.Attributes.Add("sortalias", c.TableAlias)
            lnk.Attributes.Add("removesort", remove)
            Dim cols = (From oc In entity.OrderColumns Where oc.SortColumn = c.Name AndAlso oc.SortColumnAlias = c.TableAlias AndAlso oc.SortDirection = dir)
            If Not remove AndAlso cols.Count = 1 Then lnk.CssClass = "bg-olive"
            Dim icon As New Label()
            icon.CssClass = iconClass
            lnk.Controls.Add(icon)
            lnk.Controls.Add(New LiteralControl(Localization.GetResource("Resources.Global.CMS." & resource)))
            AddHandler lnk.Click, AddressOf DoSort
            parent.Add(lnk)
        End Sub

        Private Sub AddOrderLink(ByVal parent As ControlCollection, ByVal c As ESColumn, ByVal moveRight As Boolean)
            Dim lnk As New LinkButton()
            Dim neworder As Integer = IIf(moveRight, c.ColumnOrder + 1, c.ColumnOrder - 1)
            lnk.Attributes.Add("newcolorder", neworder)
            lnk.Attributes.Add("colname", c.Name)
            lnk.Attributes.Add("colalias", c.TableAlias)
            If neworder <= 0 OrElse neworder >= entity.Columns.Count Then lnk.CssClass = "disabled"
            Dim icon As New Label()
            icon.CssClass = "fa fa-chevron-" & IIf(moveRight, IIf(Helper.Language.IsRTL, "left", "right"), IIf(Helper.Language.IsRTL, "right", "left"))
            lnk.Controls.Add(icon)
            lnk.Controls.Add(New LiteralControl(Localization.GetResource("Resources.Global.CMS." & IIf(moveRight, "MoveColRight", "MoveColLeft"))))
            AddHandler lnk.Click, AddressOf ChangeOrder
            parent.Add(lnk)
        End Sub


#End Region

#Region "Grid Setup"

        Private Sub SetupGrid()
            'grid properties
            grid.AutoGenerateColumns = False
            grid.BorderColor = Drawing.Color.White
            grid.DataKeyNames = New String() {entity.PrimaryKeyName}
            grid.EmptyDataTemplate = New GridEmptyDataTemplate()

            'grid styles
            grid.HeaderStyle.CssClass = "bg-navy"

            'grid controls
            BuildGridColumns()
            BuildGridControlColumns()
        End Sub

        Private Sub BuildGridColumns()
            If AllowSelection Then
                Dim ctrl As New ClientSelectColumn(entity.PrimaryKeyName)
                grid.Columns.Add(ctrl)
            End If
            For Each c In From col In entity.Columns Where col.Visible = True Order By col.ColumnOrder Ascending
                If c.Layout.EditControl AndAlso c.Layout.EditTypeDecider = String.Empty Then
                    Select Case c.DataType
                        Case ESDataTypes.TypeString
                            Dim ctrl As New TextBoxEditColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name), entity.PrimaryKeyName)
                            AddColumn(ctrl, c)
                        Case ESDataTypes.TypeList
                            Dim ctrl As New DropdownEditColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name), entity.PrimaryKeyName, c.EnumLookup)
                            AddColumn(ctrl, c)
                    End Select
                ElseIf c.Layout.EditControl AndAlso c.Layout.EditTypeDecider <> String.Empty Then
                    Dim ctrl As New EditableControlColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name), entity.PrimaryKeyName,
                                                          c.Layout.EditTypeDecider, c.Layout.ListDataSource, c.Layout.ListEnumDataSource,
                                                          c.Layout.DataTextField, c.Layout.DataValueField, c.Layout.LookupCondition,
                                                          c.Layout.LookupConditionValueField, entity.TableName)
                    AddColumn(ctrl, c)
                Else
                    If c.Layout.DisplayType > 0 Then
                        Select Case c.Layout.DisplayType
                            Case DisplayTypes.BooleanIcon
                                Dim ctrl As New BooleanIconColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name))
                                AddColumn(ctrl, c)
                            Case DisplayTypes.InlineImageWithText
                                Dim ctrl As New InlineImageWithTextColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name), c.Layout.TypeLookup)
                                AddColumn(ctrl, c)
                            Case DisplayTypes.LongDateTime
                                Dim ctrl As New LongDateTimeColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name))
                                AddColumn(ctrl, c)
                            Case DisplayTypes.CombinedFields
                                Dim ctrl As New CombinedFieldsColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name), c.Layout.TypeLookup)
                                AddColumn(ctrl, c)
                            Case DisplayTypes.DecimalNumber
                                Dim ctrl As New DecimalNumberColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name))
                                AddColumn(ctrl, c)
                            Case DisplayTypes.InlineImage
                                Dim ctrl As New InlineImageColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name), entity.PrimaryKeyName)
                                AddColumn(ctrl, c)
                            Case DisplayTypes.ParentChild
                                Dim parentColumn = (From pc In entity.Columns Where c.Layout.ParentField = pc.TableAlias & "." & pc.Name).FirstOrDefault()
                                Dim parentField = IIf(parentColumn.Rename <> String.Empty, parentColumn.Rename, parentColumn.Name)
                                Dim ctrl As New ParentChildFieldColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name), parentField)
                                AddColumn(ctrl, c)
                            Case DisplayTypes.ClassName
                                Dim classIdcolumn = (From cic In entity.Columns Where c.Layout.ParentField = cic.TableAlias & "." & cic.Name).FirstOrDefault()
                                Dim classIdField = IIf(classIdcolumn.Rename <> String.Empty, classIdcolumn.Rename, classIdcolumn.Name)
                                Dim ctrl As New ClassNameColumn(IIf(c.Rename <> String.Empty, c.Rename, c.TableAlias & "." & c.Name), classIdField)
                                AddColumn(ctrl, c)
                        End Select
                    ElseIf c.EnumLookup <> String.Empty Then
                        Dim ctrl As New EnumColumn(IIf(c.Rename <> String.Empty, c.Rename, c.Name), c.EnumLookup)
                        AddColumn(ctrl, c)
                    Else
                        Dim ctrl As New BoundField()
                        ctrl.DataField = IIf(c.Rename <> String.Empty, c.Rename, c.Name)
                        AddColumn(ctrl, c)
                    End If
                End If
            Next
        End Sub

        Private Sub BuildGridControlColumns()
            For Each c In
                From col In entity.ControlColumns
                Where Helper.CMSAuthUser.IsSuperAdmin OrElse
                    (Not col.Super AndAlso col.Type = ESControlTypes.View AndAlso Helper.CMSAuthUser.CanRead(col.PermissionId)) OrElse
                    (Not col.Super AndAlso (col.Type = ESControlTypes.Edit OrElse col.Type = ESControlTypes.ModalEdit) AndAlso Helper.CMSAuthUser.CanModify(col.PermissionId)) OrElse
                    (Not col.Super AndAlso col.Type = ESControlTypes.Delete AndAlso Helper.CMSAuthUser.CanDelete(col.PermissionId))
                Select Case c.Type
                    Case ESControlTypes.Edit
                        Dim ctrl As New EditColumn(entity.PrimaryKeyName, EditRecordLink)
                        ctrl.HeaderText = c.Resource
                        grid.Columns.Add(ctrl)
                    Case ESControlTypes.Delete
                        Dim ctrl As New DeleteColumn(entity.PrimaryKeyName)
                        ctrl.HeaderText = c.Resource
                        grid.Columns.Add(ctrl)
                    Case ESControlTypes.View
                        Dim ctrl As New ViewColumn(entity.PrimaryKeyName, EditRecordLink)
                        ctrl.HeaderText = c.Resource
                        grid.Columns.Add(ctrl)
                    Case ESControlTypes.ModalEdit
                        Dim ctrl As New ModalEditColumn(entity.PrimaryKeyName)
                        ctrl.HeaderText = c.Resource
                        grid.Columns.Add(ctrl)
                End Select
            Next
        End Sub

#End Region

#Region "Grid Events"

        Protected Sub RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs) Handles grid.RowDeleting
            RaiseEvent RowDelete(e.Keys(entity.PrimaryKeyName))
            RaiseEvent GridNeedDataSource(sender, e)
        End Sub

        Protected Sub RowCreated(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grid.RowCreated
            If AllowSorting OrElse AllowChangeColumnOrder Then
                If e.Row.RowType = DataControlRowType.Header Then
                    Dim isFirst As Boolean = True
                    For Each cell As DataControlFieldHeaderCell In From col In e.Row.Cells Where DirectCast(col, DataControlFieldHeaderCell).ContainingField.HeaderStyle.CssClass.Contains("sortable-header")
                        Dim c As ESColumn = (From col In entity.Columns Where DirectCast(cell, DataControlFieldHeaderCell).ContainingField.HeaderStyle.CssClass.Contains("data-" & col.TableAlias & "-" & col.Name)).FirstOrDefault()
                        BuildSortingTools(cell.Controls, c, isFirst)
                        isFirst = False
                    Next
                End If
            End If
        End Sub

#End Region

    End Class

#Region "Columns"

    Public Class ClientSelectColumn
        Inherits TemplateField

        Public Property PrimaryKey As String = String.Empty

        Public Sub New(Optional ByVal pkey As String = "")
            PrimaryKey = pkey
            HeaderStyle.Width = Unit.Pixel(50)
            HeaderStyle.CssClass = "text-center"
            ItemStyle.CssClass = "text-center"
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            HeaderTemplate = New ClientSelectHeaderTemplate()
            ItemTemplate = New ClientSelectTemplate(PrimaryKey)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class ModalEditColumn
        Inherits TemplateField

        Public Property PrimaryKey As String = String.Empty

        Public Sub New(Optional ByVal pkey As String = "")
            HeaderStyle.Width = Unit.Pixel(50)
            HeaderStyle.CssClass = "text-center"
            ItemStyle.CssClass = "text-center"
            PrimaryKey = pkey
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New ModalEditTemplate(PrimaryKey)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class ViewColumn
        Inherits TemplateField

        Public Property PrimaryKey As String = String.Empty
        Public Property EditLink As String = String.Empty

        Public Sub New(Optional ByVal pkey As String = "", Optional ByVal link As String = "")
            HeaderStyle.Width = Unit.Pixel(50)
            HeaderStyle.CssClass = "text-center"
            ItemStyle.CssClass = "text-center"
            PrimaryKey = pkey
            EditLink = link
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New ViewColumnTemplate(PrimaryKey, EditLink)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class EditColumn
        Inherits TemplateField

        Public Property PrimaryKey As String = String.Empty
        Public Property EditLink As String = String.Empty

        Public Sub New(Optional ByVal pkey As String = "", Optional ByVal editLnk As String = "")
            HeaderStyle.Width = Unit.Pixel(50)
            HeaderStyle.CssClass = "text-center"
            ItemStyle.CssClass = "text-center"
            PrimaryKey = pkey
            EditLink = editLnk
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New EditColumnTemplate(PrimaryKey, EditLink)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class DeleteColumn
        Inherits TemplateField

        Public Property PrimaryKey As String = String.Empty

        Public Sub New(Optional ByVal pkey As String = "")
            HeaderStyle.Width = Unit.Pixel(50)
            HeaderStyle.CssClass = "text-center"
            ItemStyle.CssClass = "text-center"
            PrimaryKey = pkey
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New DeleteColumnTemplate(PrimaryKey)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class EditableControlColumn
        Inherits TemplateField

#Region "Public Methods"

        Public Property BoundFieldName As String = String.Empty
        Public Property PrimaryKey As String = String.Empty
        Public Property EditTypeDeciderField As String = String.Empty
        Public Property LookupDataSource As String = String.Empty
        Public Property LookupDataSourceCondition As String = String.Empty
        Public Property LookupDataSourceConditionValueField As String = String.Empty
        Public Property DataTextField As String = String.Empty
        Public Property DataValueField As String = String.Empty
        Public Property EnumLookupDataSource As String = String.Empty
        Public Property DataTableName As String = String.Empty

#End Region

        Public Sub New(Optional ByVal boundField As String = "", Optional ByVal pkey As String = "",
                       Optional ByVal deciderField As String = "", Optional ByVal lookup As String = "",
                       Optional ByVal enumLookup As String = "", Optional ByVal dataText As String = "",
                       Optional ByVal dataValue As String = "", Optional ByVal lookupCondition As String = "",
                       Optional ByVal lookupConditionField As String = "", Optional ByVal tableName As String = "")
            ItemStyle.CssClass = "form-group"
            BoundFieldName = boundField
            PrimaryKey = pkey
            EditTypeDeciderField = deciderField
            LookupDataSource = lookup
            LookupDataSourceCondition = lookupCondition
            LookupDataSourceConditionValueField = lookupConditionField
            DataTextField = dataText
            DataValueField = dataValue
            EnumLookupDataSource = enumLookup
            DataTableName = tableName
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New EditableColumnTemplate(BoundFieldName, PrimaryKey, EditTypeDeciderField, LookupDataSource,
                                                      EnumLookupDataSource, LookupDataSourceCondition, LookupDataSourceConditionValueField,
                                                      DataTextField, DataValueField, DataTableName)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class DropdownEditColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty
        Public Property PrimaryKey As String = String.Empty
        Public Property EnumLookupDataSource As String = String.Empty
        Public Property LookupDataSource As String = String.Empty
        Public Property DataTextField As String = String.Empty
        Public Property DataValueField As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal pkey As String = "", Optional ByVal eType As String = "", Optional ByVal ds As String = "", Optional ByVal text As String = "", Optional ByVal value As String = "")
            BoundFieldName = field
            PrimaryKey = pkey
            EnumLookupDataSource = eType
            LookupDataSource = ds
            DataTextField = text
            DataValueField = value
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New DropDownEditTemplate(BoundFieldName, PrimaryKey, EnumLookupDataSource, LookupDataSource, DataTextField, DataValueField)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class TextBoxEditColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty
        Public Property PrimaryKey As String = String.Empty

        Public Sub New(Optional ByVal boundField As String = "", Optional ByVal pkey As String = "")
            ItemStyle.CssClass = "form-group"
            BoundFieldName = boundField
            PrimaryKey = pkey
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New TextBoxEditTemplate(BoundFieldName, PrimaryKey)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class LongDateTimeColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty

        Public Sub New(Optional ByVal boundfield As String = "")
            BoundFieldName = boundfield
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New LongDateTimeTemplate(BoundFieldName)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class BooleanIconColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty

        Public Sub New(Optional ByVal boundField As String = "")
            BoundFieldName = boundField
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New BooleanIconTemplate(BoundFieldName)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class EnumColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty
        Public Property EnumTypeString As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal etString As String = "")
            BoundFieldName = field
            EnumTypeString = etString
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New EnumColumnTemplate(BoundFieldName, EnumTypeString)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class InlineImageColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty
        Public Property PrimaryKey As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal pkey As String = "")
            BoundFieldName = field
            PrimaryKey = pkey
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New InlineImageTemplate(BoundFieldName, PrimaryKey)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class InlineImageWithTextColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty
        Public Property ImageFieldName As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal image As String = "")
            BoundFieldName = field
            ImageFieldName = image
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New InlineImageWithTextTemplate(BoundFieldName, ImageFieldName)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class ParentChildFieldColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty
        Public Property ParentFieldName As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal parent As String = "")
            BoundFieldName = field
            ParentFieldName = parent
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New ParentChildFieldTemplate(BoundFieldName, ParentFieldName)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class CombinedFieldsColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty
        Public Property CombinedFieldName As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal combined As String = "")
            BoundFieldName = field
            CombinedFieldName = combined
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New CombinedFieldsTemplate(BoundFieldName, CombinedFieldName)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class DecimalNumberColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty

        Public Sub New(Optional ByVal field As String = "")
            BoundFieldName = field
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New DecimalNumberTemplate(BoundFieldName)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

    Public Class ClassNameColumn
        Inherits TemplateField

        Public Property BoundFieldName As String = String.Empty
        Public Property ClassIdFieldName As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal classIdField As String = "")
            BoundFieldName = field
            ClassIdFieldName = classIdField
        End Sub

        Public Overrides Function Initialize(sortingEnabled As Boolean, control As Control) As Boolean
            ItemTemplate = New ClassNameTemplate(BoundFieldName, ClassIdFieldName)
            Return MyBase.Initialize(sortingEnabled, control)
        End Function

    End Class

#End Region

#Region "Templates"

    Public Class ModalEditTemplate
        Implements ITemplate

        Public Property PrimaryKey As String = String.Empty

        Public Sub New(Optional ByVal pkey As String = "")
            PrimaryKey = pkey
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim lnk As New HyperLink()
            lnk.ToolTip = Localization.GetResource("Resources.Global.CMS.Edit")
            lnk.ID = "lnkModalEdit"
            lnk.CssClass = "btn btn-default btn-sm"
            AddHandler lnk.DataBinding, AddressOf OnDataBound
            Dim lbl As New Label()
            lbl.CssClass = "fa fa-edit"
            lnk.Controls.Add(lbl)
            container.Controls.Add(lnk)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If PrimaryKey <> String.Empty Then
                Dim lnk As HyperLink = DirectCast(sender, HyperLink)
                Dim container As GridViewRow = DirectCast(lnk.NamingContainer, GridViewRow)
                lnk.Attributes.Add("data-key", DirectCast(container.DataItem, DataRowView)(PrimaryKey).ToString())
            End If
        End Sub

    End Class

    Public Class ClientSelectTemplate
        Implements ITemplate

        Public Property PrimaryKey As String = String.Empty

        Public Sub New(Optional ByVal pkey As String = "")
            PrimaryKey = pkey
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim chk As New CheckBox()
            chk.ID = "chkBxSelect"
            chk.EnableViewState = True
            chk.CssClass = "egvGrid-select-row"
            AddHandler chk.DataBinding, AddressOf OnDataBound
            container.Controls.Add(chk)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If PrimaryKey <> String.Empty Then
                Dim chk As CheckBox = DirectCast(sender, CheckBox)
                Dim dr As DataRowView = DirectCast(DirectCast(chk.NamingContainer, GridViewRow).DataItem, DataRowView)
                chk.Attributes.Add("data-key", dr(PrimaryKey))
            End If
        End Sub

    End Class

    Public Class ClientSelectHeaderTemplate
        Implements ITemplate

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim chk As New CheckBox()
            chk.ID = "chkBxHeader"
            chk.Attributes.Add("onclick", "javascript: HeaderClick(this);")
            container.Controls.Add(chk)
        End Sub

    End Class

    Public Class ViewColumnTemplate
        Implements ITemplate

        Public Property PrimaryKey As String = String.Empty
        Public Property EditLink As String = String.Empty

        Public Sub New(Optional ByVal pkey As String = "", Optional ByVal link As String = "")
            PrimaryKey = pkey
            EditLink = link
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim lnk As New HyperLink()
            lnk.ToolTip = Localization.GetResource("Resources.Global.CMS.View")
            lnk.ID = "lnkView"
            lnk.CssClass = "btn btn-success btn-sm"
            AddHandler lnk.DataBinding, AddressOf OnDataBound
            Dim lbl As New Label()
            lbl.CssClass = "fa fa-eye"
            lnk.Controls.Add(lbl)
            container.Controls.Add(lnk)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If PrimaryKey <> String.Empty Then
                Dim lnk As HyperLink = DirectCast(sender, HyperLink)
                Dim container As GridViewRow = DirectCast(lnk.NamingContainer, GridViewRow)
                If EditLink <> String.Empty Then
                    lnk.NavigateUrl = String.Format(EditLink, DirectCast(container.DataItem, DataRowView)(PrimaryKey).ToString())
                Else
                    lnk.Attributes.Add("data-key", DirectCast(container.DataItem, DataRowView)(PrimaryKey).ToString())
                End If
            End If
        End Sub

    End Class

    Public Class EditColumnTemplate
        Implements ITemplate

        Public Property PrimaryKey As String = String.Empty
        Public Property EditLink As String = String.Empty

        Public Sub New(Optional ByVal pkey As String = "", Optional ByVal editLnk As String = "")
            PrimaryKey = pkey
            EditLink = editLnk
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim lnk As New HyperLink()
            lnk.ToolTip = Localization.GetResource("Resources.Global.CMS.Edit")
            lnk.ID = "lnkEdit"
            lnk.CssClass = "btn btn-sm btn-default"
            AddHandler lnk.DataBinding, AddressOf OnDataBound
            Dim lbl As New Label()
            lbl.CssClass = "fa fa-edit"
            lnk.Controls.Add(lbl)
            container.Controls.Add(lnk)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If PrimaryKey <> String.Empty Then
                Dim lnk As HyperLink = DirectCast(sender, HyperLink)
                Dim container As GridViewRow = DirectCast(lnk.NamingContainer, GridViewRow)
                If EditLink <> String.Empty Then
                    lnk.NavigateUrl = String.Format(EditLink, DirectCast(container.DataItem, DataRowView)(PrimaryKey).ToString())
                Else
                    lnk.Attributes.Add("data-key", DirectCast(container.DataItem, DataRowView)(PrimaryKey).ToString())
                End If
            End If
        End Sub

    End Class

    Public Class DeleteColumnTemplate
        Implements ITemplate

        Public Property PrimaryKey As String = String.Empty

        Public Sub New(Optional ByVal pkey As String = "")
            PrimaryKey = pkey
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim lnk As New LinkButton()
            lnk.OnClientClick = "return confirm('" & Localization.GetResource("Resources.Global.CMS.DeleteConfirm") & "');"
            lnk.ToolTip = Localization.GetResource("Resources.Global.CMS.Delete")
            lnk.ID = "lnkDelete"
            lnk.CssClass = "btn btn-sm btn-danger"
            lnk.CommandName = "Delete"
            AddHandler lnk.DataBinding, AddressOf OnDataBound
            Dim lbl As New Label()
            lbl.CssClass = "fa fa-times"
            lnk.Controls.Add(lbl)
            container.Controls.Add(lnk)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If PrimaryKey <> String.Empty Then
                Dim lnk As LinkButton = DirectCast(sender, LinkButton)
                Dim container As GridViewRow = DirectCast(lnk.NamingContainer, GridViewRow)
                lnk.CommandArgument = DirectCast(container.DataItem, DataRowView)(PrimaryKey).ToString()
            End If
        End Sub

    End Class

    Public Class EditableColumnTemplate
        Implements ITemplate

#Region "Public Properties"

        Public Property BoundFieldName As String = String.Empty
        Public Property PrimaryKey As String = String.Empty
        Public Property EditTypeDeciderField As String = String.Empty
        Public Property LookupDataSource As String = String.Empty
        Public Property LookupDataSourceCondition As String = String.Empty
        Public Property LookupDataSourceConditionValueField As String = String.Empty
        Public Property DataTextField As String = String.Empty
        Public Property DataValueField As String = String.Empty
        Public Property EnumLookupDataSource As String = String.Empty
        Public Property DataTableName As String = String.Empty

#End Region

#Region "Private Properties"

        Private Property txt As EGVTextBox
        Private Property ddl As EGVDropDown
        Private Property chk As EGVCheckBox

#End Region

#Region "Constructors"

        Public Sub New(Optional ByVal boundField As String = "", Optional ByVal pkey As String = "",
                       Optional ByVal deciderField As String = "", Optional lookup As String = "",
                       Optional ByVal enumLookup As String = "", Optional ByVal lookupCondition As String = "",
                       Optional ByVal lookupConditionValue As String = "", Optional ByVal dataText As String = "",
                       Optional ByVal dataValue As String = "", Optional ByVal tableName As String = "")
            BoundFieldName = boundField
            PrimaryKey = pkey
            EditTypeDeciderField = deciderField
            LookupDataSource = lookup
            EnumLookupDataSource = enumLookup
            LookupDataSourceCondition = lookupCondition
            LookupDataSourceConditionValueField = lookupConditionValue
            DataTextField = dataText
            DataValueField = dataValue
            DataTableName = tableName
        End Sub

#End Region

#Region "Public Methods"

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            txt = New EGVTextBox()
            txt.ID = "txtEdit" & BoundFieldName
            txt.ControlSize = ControlSizes.Small
            txt.Visible = False
            AddHandler txt.DataBinding, AddressOf OnDataBound
            container.Controls.Add(txt)
            ddl = New EGVDropDown()
            ddl.ID = "ddlEdit" & BoundFieldName
            ddl.Visible = False
            ddl.Size = ControlSizes.Small
            AddHandler ddl.DataBinding, AddressOf OnDataBound
            container.Controls.Add(ddl)
            chk = New EGVCheckBox()
            chk.ID = "chkEdit" & BoundFieldName
            chk.Visible = False
            AddHandler chk.DataBinding, AddressOf OnDataBound
            container.Controls.Add(chk)
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty Then
                Dim container As GridViewRow = DirectCast(DirectCast(sender, Control).NamingContainer, GridViewRow)
                If container.Attributes("bindcompleted") = String.Empty OrElse container.Attributes("bindcompleted") = False Then
                    Dim dataItem As DataRowView = DirectCast(container.DataItem, DataRowView)
                    If EditTypeDeciderField <> String.Empty Then
                        Dim conn As SqlConnection = DBA.GetConn()
                        Try
                            conn.Open()
                            Dim qr As String = "SELECT " & EditTypeDeciderField & ", " & LookupDataSourceConditionValueField & " FROM " & DataTableName & " WHERE " & PrimaryKey & " = '" & dataItem(PrimaryKey) & "'"
                            Using dt As DataTable = DBA.DataTable(conn, qr)
                                Dim dr As DataRow = dt.Rows(0)
                                Dim type As ESDataTypes = dr(EditTypeDeciderField)
                                Select Case type
                                    Case ESDataTypes.TypeInteger, ESDataTypes.TypeLongString, ESDataTypes.TypeString
                                        txt.Visible = True
                                        txt.Text = Helper.GetSafeDBValue(dataItem(BoundFieldName))
                                        txt.Attributes.Add("data-field", BoundFieldName)
                                        If type = ESDataTypes.TypeInteger Then txt.InputNumber = True
                                        container.Attributes.Add("bindcompleted", True)
                                        If PrimaryKey <> String.Empty Then txt.Attributes.Add("data-key", dataItem(PrimaryKey).ToString())
                                    Case ESDataTypes.TypeBoolean
                                        chk.Visible = True
                                        chk.Attributes.Add("data-field", BoundFieldName)
                                        container.Attributes.Add("bindcompleted", True)
                                        If PrimaryKey <> String.Empty Then chk.Attributes.Add("data-key", dataItem(PrimaryKey).ToString())
                                        chk.Checked = Helper.GetSafeDBValue(dataItem(BoundFieldName), ValueTypes.TypeBoolean)
                                        chk.CssClass = "icheck"
                                    Case ESDataTypes.TypeList
                                        ddl.Visible = True
                                        ddl.Attributes.Add("data-field", BoundFieldName)
                                        container.Attributes.Add("bindcompleted", True)
                                        If PrimaryKey <> String.Empty Then ddl.Attributes.Add("data-key", dataItem(PrimaryKey).ToString())
                                        If LookupDataSource <> String.Empty Then
                                            Dim q As String = "SELECT " & DataTextField & ", " & DataValueField & " FROM " & LookupDataSource & IIf(LookupDataSourceCondition <> String.Empty, " WHERE " & LookupDataSourceCondition & " = " & dr(LookupDataSourceConditionValueField), "")
                                            ddl.AddDataSourceItems(DBA.DataTable(conn, q), DataTextField, DataValueField)
                                            ddl.SelectedValue = Helper.GetSafeDBValue(dataItem(BoundFieldName), ValueTypes.TypeInteger)
                                        ElseIf EnumLookupDataSource <> String.Empty Then
                                            ddl.BindToEnum(Helper.GetEnumType(EnumLookupDataSource))
                                            ddl.SelectedValue = Helper.GetSafeDBValue(dataItem(BoundFieldName), ValueTypes.TypeInteger)
                                        End If
                                End Select
                            End Using
                        Catch ex As Exception
                            Throw ex
                        Finally
                            conn.Close()
                        End Try
                    End If
                End If
            End If
        End Sub

#End Region

    End Class

    Public Class DropDownEditTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty
        Public Property PrimaryKey As String = String.Empty
        Public Property EnumLookupDataSource As String = String.Empty
        Public Property LookupDataSource As String = String.Empty
        Public Property DataTextField As String = String.Empty
        Public Property DataValueField As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal pkey As String = "", Optional ByVal eType As String = "", Optional ByVal ds As String = "", Optional ByVal text As String = "", Optional ByVal value As String = "")
            BoundFieldName = field
            PrimaryKey = pkey
            EnumLookupDataSource = eType
            LookupDataSource = ds
            DataTextField = text
            DataValueField = value
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim ddl As New EGVDropDown()
            ddl.ID = "ddlEdit" & BoundFieldName
            ddl.Size = ControlSizes.Small
            AddHandler ddl.DataBinding, AddressOf OnDataBind
            container.Controls.Add(ddl)
        End Sub

        Protected Sub OnDataBind(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty Then
                Dim ddl As EGVDropDown = DirectCast(sender, EGVDropDown)
                Dim container As GridViewRow = DirectCast(ddl.NamingContainer, GridViewRow)
                Dim dataitem As DataRowView = DirectCast(container.DataItem, DataRowView)
                ddl.Attributes.Add("data-field", BoundFieldName)
                container.Attributes.Add("bindcompleted", True)
                If PrimaryKey <> String.Empty Then ddl.Attributes.Add("data-key", dataitem(PrimaryKey).ToString())
                If LookupDataSource <> String.Empty Then
                    Dim conn = DBA.GetConn()
                    Try
                        conn.Open()
                        Dim q As String = "SELECT " & DataTextField & ", " & DataValueField & " FROM " & LookupDataSource
                        ddl.AddDataSourceItems(DBA.DataTable(conn, q), DataTextField, DataValueField)
                        ddl.SelectedValue = Helper.GetSafeDBValue(dataitem(BoundFieldName), ValueTypes.TypeInteger)
                    Catch ex As Exception
                        Throw ex
                    Finally
                        conn.Close()
                    End Try
                ElseIf EnumLookupDataSource <> String.Empty Then
                    ddl.BindToEnum(Helper.GetEnumType(EnumLookupDataSource), False)
                    ddl.SelectedValue = Helper.GetSafeDBValue(dataitem(BoundFieldName), ValueTypes.TypeInteger)
                End If
            End If
        End Sub

    End Class

    Public Class TextBoxEditTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty
        Public Property PrimaryKey As String = String.Empty

        Public Sub New(Optional ByVal boundField As String = "", Optional ByVal pkey As String = "")
            BoundFieldName = boundField
            PrimaryKey = pkey
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim txt As New TextBox()
            txt.ID = "txtEdit" & BoundFieldName
            txt.CssClass = "form-control input-sm"
            AddHandler txt.DataBinding, AddressOf OnDataBound
            container.Controls.Add(txt)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty Then
                Dim txt As TextBox = DirectCast(sender, TextBox)
                Dim container As GridViewRow = DirectCast(txt.NamingContainer, GridViewRow)
                Dim dataItem As DataRowView = DirectCast(container.DataItem, DataRowView)
                txt.Text = Helper.GetSafeDBValue(dataItem(BoundFieldName))
                txt.Attributes.Add("data-field", BoundFieldName)
                If PrimaryKey <> String.Empty Then
                    txt.Attributes.Add("data-key", dataItem(PrimaryKey).ToString())
                End If
            End If
        End Sub

    End Class

    Public Class LongDateTimeTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty

        Public Sub New(Optional ByVal boundField As String = "")
            BoundFieldName = boundField
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim lit As New Literal()
            AddHandler lit.DataBinding, AddressOf OnDataBound
            container.Controls.Add(lit)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty Then
                Dim lit As Literal = DirectCast(sender, Literal)
                Dim container As GridViewRow = DirectCast(lit.NamingContainer, GridViewRow)
                Dim dataitem As DataRowView = DirectCast(container.DataItem, DataRowView)
                Dim dateValue As DateTime = Helper.GetSafeDBValue(dataitem(BoundFieldName), ValueTypes.TypeDateTime)
                If dateValue <> DateTime.MinValue Then
                    lit.Text = dateValue.ToString("MMMM dd, yyyy @ hh:mm:ss")
                Else
                    lit.Text = String.Empty
                End If
            End If
        End Sub

    End Class

    Public Class BooleanIconTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty

        Public Sub New(Optional ByVal boundField As String = "")
            BoundFieldName = boundField
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim lbl As New Label()
            AddHandler lbl.DataBinding, AddressOf OnDataBound
            container.Controls.Add(lbl)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty Then
                Dim lbl As Label = DirectCast(sender, Label)
                Dim container As GridViewRow = DirectCast(lbl.NamingContainer, GridViewRow)
                Dim dataitem As DataRowView = DirectCast(container.DataItem, DataRowView)
                Dim boolVal As Boolean = Helper.GetSafeDBBoolean(dataitem(BoundFieldName))
                lbl.CssClass = IIf(boolVal, "text-success fa fa-check", "text-danger fa fa-times")
            End If
        End Sub

    End Class

    Public Class InlineImageTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty
        Public Property PrimaryKey As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal pkey As String = "")
            BoundFieldName = field
            PrimaryKey = pkey
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim hyp As New HyperLink()
            hyp.Attributes.Add("data-imagepreview", "group1")
            AddHandler hyp.DataBinding, AddressOf OnDataBound
            container.Controls.Add(hyp)
            Dim img As New Image()
            img.CssClass = "egv-grid-inline-image-full"
            hyp.Controls.Add(img)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty Then
                Dim hyp As HyperLink = DirectCast(sender, HyperLink)
                Dim container As GridViewRow = DirectCast(hyp.NamingContainer, GridViewRow)
                Dim dataitem As DataRowView = DirectCast(container.DataItem, DataRowView)
                Dim path As String = Helper.GetSafeDBValue(dataitem(BoundFieldName))
                hyp.NavigateUrl = path
                hyp.ID = "hyp" & BoundFieldName & PrimaryKey
                If hyp.HasControls() Then
                    Dim img As Image = DirectCast(hyp.Controls(0), Image)
                    img.ImageUrl = Helper.FormImageUrl(dataitem(BoundFieldName))
                    img.ID = "img" & BoundFieldName & dataitem(PrimaryKey)
                End If
            End If
        End Sub

    End Class

    Public Class InlineImageWithTextTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty
        Public Property ImageFieldName As String = String.Empty

        Public Sub New(Optional ByVal boundField As String = "", Optional ByVal imageField As String = "")
            BoundFieldName = boundField
            ImageFieldName = imageField
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim div As New Panel()
            AddHandler div.DataBinding, AddressOf OnDataBound
            Dim img As New Image()
            img.Visible = False
            img.CssClass = "egv-grid-inline-image"
            div.Controls.Add(img)
            Dim lit As New Literal()
            div.Controls.Add(lit)
            container.Controls.Add(div)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty Then
                Dim div As Panel = DirectCast(sender, Panel)
                Dim container As GridViewRow = DirectCast(div.NamingContainer, GridViewRow)
                Dim dataitem As DataRowView = DirectCast(container.DataItem, DataRowView)
                Dim text As String = Helper.GetSafeDBValue(dataitem(BoundFieldName))
                If ImageFieldName <> String.Empty Then
                    Dim imageurl As String = Helper.FormImageUrl(Helper.GetSafeDBValue(dataitem(ImageFieldName)), 0, 0, 0, 0, CroppingTypes.Center, 0, 0, String.Empty, IIf(dataitem(ImageFieldName).ToString().StartsWith("/" & Helper.CMSPath()), "cms", "portal"))
                    Dim img As Image = div.Controls(0)
                    img.ImageUrl = imageurl
                    img.Visible = True
                End If
                Dim lit As Literal = div.Controls(1)
                lit.Text = text
            End If
        End Sub

    End Class

    Public Class EnumColumnTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty
        Public Property EnumTypeString As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal etString As String = "")
            BoundFieldName = field
            EnumTypeString = etString
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim lit As New Literal()
            AddHandler lit.DataBinding, AddressOf OnDataBound
            container.Controls.Add(lit)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty AndAlso EnumTypeString <> String.Empty Then
                Dim lit As Literal = DirectCast(sender, Literal)
                Dim container As GridViewRow = DirectCast(lit.NamingContainer, GridViewRow)
                Dim dataitem As DataRowView = DirectCast(container.DataItem, DataRowView)
                lit.Text = Helper.GetEnumText(EnumTypeString, Helper.GetSafeDBValue(dataitem(BoundFieldName), ValueTypes.TypeInteger))
            End If
        End Sub

    End Class

    Public Class ParentChildFieldTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty
        Public Property ParentFieldName As String = String.Empty

        Public Sub New(Optional ByVal boundField As String = "", Optional ByVal parentField As String = "")
            BoundFieldName = boundField
            ParentFieldName = parentField
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim lit As New Literal()
            lit.ID = "lit"
            AddHandler lit.DataBinding, AddressOf OnDataBound
            container.Controls.Add(lit)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty AndAlso ParentFieldName <> String.Empty Then
                Dim lit As Literal = DirectCast(sender, Literal)
                Dim container As GridViewRow = DirectCast(lit.NamingContainer, GridViewRow)
                Dim dataitem As DataRowView = DirectCast(container.DataItem, DataRowView)
                Dim child As String = Helper.GetSafeDBValue(dataitem(BoundFieldName))
                Dim parent As String = Helper.GetSafeDBValue(dataitem(ParentFieldName))
                lit.Text = parent & "&nbsp;&nbsp;<span class=""fa fa-chevron-" & Helper.GetCSSAltDirection() & """></span>&nbsp;&nbsp;" & child
            End If
        End Sub

    End Class

    Public Class CombinedFieldsTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty
        Public Property CombinedFieldName As String = String.Empty

        Public Sub New(Optional ByVal boundField As String = "", Optional ByVal combinedField As String = "")
            BoundFieldName = boundField
            CombinedFieldName = combinedField
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim div As New Panel()
            AddHandler div.DataBinding, AddressOf OnDataBound
            Dim lit As New Literal()
            lit.ID = "lit"
            div.Controls.Add(lit)
            Dim litCombined As New Literal()
            litCombined.Visible = False
            litCombined.ID = "litCombined"
            div.Controls.Add(New LiteralControl(" "))
            div.Controls.Add(litCombined)
            container.Controls.Add(div)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty Then
                Dim div As Panel = DirectCast(sender, Panel)
                Dim container As GridViewRow = DirectCast(div.NamingContainer, GridViewRow)
                Dim dataitem As DataRowView = DirectCast(container.DataItem, DataRowView)
                Dim text As String = Helper.GetSafeDBValue(dataitem(BoundFieldName))
                If CombinedFieldName <> String.Empty Then
                    Dim combinedText As String = Helper.GetSafeDBValue(dataitem(CombinedFieldName))
                    Dim litCombined As Literal = div.FindControl("litCombined")
                    If litCombined IsNot Nothing Then
                        litCombined.Text = "<small>(<b>" & combinedText & "</b>)</small>"
                        litCombined.Visible = True
                    End If
                End If
                Dim lit As Literal = div.FindControl("lit")
                lit.Text = text
            End If
        End Sub

    End Class

    Public Class DecimalNumberTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty

        Public Sub New(Optional ByVal field As String = "")
            BoundFieldName = field
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim lit As New Literal()
            AddHandler lit.DataBinding, AddressOf OnDataBound
            container.Controls.Add(lit)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty Then
                Dim lit As Literal = DirectCast(sender, Literal)
                Dim container As GridViewRow = DirectCast(lit.NamingContainer, GridViewRow)
                Dim dataitem As DataRowView = DirectCast(container.DataItem, DataRowView)
                Dim val As Decimal = Helper.GetSafeDBValue(dataitem(BoundFieldName), ValueTypes.TypeDecimal)
                lit.Text = val.ToString("0,0.00")
            End If
        End Sub

    End Class

    Public Class ClassNameTemplate
        Implements ITemplate

        Public Property BoundFieldName As String = String.Empty
        Public Property ClassIdFieldName As String = String.Empty

        Public Sub New(Optional ByVal field As String = "", Optional ByVal classIdfield As String = "")
            BoundFieldName = field
            ClassIdFieldName = classIdfield
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim lit As New Literal()
            lit.ID = "lit"
            AddHandler lit.DataBinding, AddressOf OnDataBound
            container.Controls.Add(lit)
        End Sub

        Protected Sub OnDataBound(ByVal sender As Object, ByVal e As EventArgs)
            If BoundFieldName <> String.Empty Then
                Dim lit As Literal = DirectCast(sender, Literal)
                Dim container As GridViewRow = DirectCast(lit.NamingContainer, GridViewRow)
                Dim dataitem As DataRowView = DirectCast(container.DataItem, DataRowView)
                If BoundFieldName <> String.Empty AndAlso ClassIdFieldName <> String.Empty Then
                    Dim section As String = Helper.GetSafeDBValue(dataitem(BoundFieldName))
                    Dim classId As String = Helper.GetSafeDBValue(dataitem(ClassIdFieldName), ValueTypes.TypeInteger)
                    Dim className As String = StudyClassController.GetTitle(DBA.GetConn(), classId, Helper.LanguageId)
                    lit.Text = className & " - " & section
                End If
            End If
        End Sub

    End Class

    Public Class GridEmptyDataTemplate
        Implements ITemplate

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim pnl As New Panel()
            pnl.CssClass = "egv-grid-empty-data"
            pnl.Controls.Add(New LiteralControl(Localization.GetResource("Resources.Local.egvGrid.EmptyMessage")))
            container.Controls.Add(pnl)
        End Sub

    End Class

#End Region

End Namespace
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Utils
Imports EGV

Namespace EGVControls

    <ToolboxData("<{0}:EGVFilter runat=server></{0}:EGVFilter>")>
    Public Class EGVFilter
        Inherits WebControl
        Implements INamingContainer

#Region "Public Events"

        Public Event ApplyFilters(ByVal filters As List(Of UserGridColumnFilter))

#End Region

#Region "Public Properties"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

#End Region

#Region "Private Properties"

        Private Property Filters As List(Of FilterItem)
        Private Property AppliedFilters As List(Of UserGridColumnFilter)
        Private Property hdnDefinition As HiddenField
        Private Property hdnApplied As HiddenField

#End Region

#Region "Constructors"

        Public Sub New()
            Filters = New List(Of FilterItem)()
            AppliedFilters = New List(Of UserGridColumnFilter)()
            hdnApplied = New HiddenField()
            hdnDefinition = New HiddenField()
        End Sub

#End Region

#Region "Public Methods"

        Public Sub AddFilter(ByVal filter As ESColumn)
            If filter.Filter.Allow Then
                Dim f As New FilterItem() With {
                    .DisplayTitle = IIf(filter.Resource <> String.Empty, filter.Resource, IIf(filter.Rename <> String.Empty, filter.Rename, filter.Name)),
                    .Expression = filter.Filter.Expression,
                    .Type = filter.Filter.FilterType,
                    .LookupSource = filter.Lookup,
                    .EnumLookupSource = filter.EnumLookup,
                    .LookupDataText = filter.Filter.FilterDataText,
                    .LookupDataValue = filter.Filter.FilterDataValue,
                    .AllowedTypes = filter.Filter.AllowedTypes
                }
                Filters.Add(f)
            End If
        End Sub

        Public Sub ApplyFilter(ByVal expression As String, ByVal type As SearchTypes, ByVal value As String, ByVal dataType As FilterTypes)
            AppliedFilters.Add(New UserGridColumnFilter() With {
                .Operation = type,
                .Value = value,
                .Expression = expression,
                .DataType = dataType
            })
        End Sub

#End Region

#Region "Private Methods"

        Private Function SetupJSFilterDef() As String
            Dim lst As New List(Of JSFilterDefinition)
            For Each f As FilterItem In Filters
                Dim item As New JSFilterDefinition() With {
                    .Expression = f.Expression,
                    .DataType = f.Type
                }
                item.AllowedTypes = New List(Of JSFilterAllowedType)()
                For Each i As Integer In f.AllowedTypes
                    item.AllowedTypes.Add(New JSFilterAllowedType() With {
                        .Text = Helper.GetEnumText(GetType(SearchTypes), i),
                        .Value = i
                    })
                Next
                lst.Add(item)
            Next
            Return Helper.JSSerialize(lst)
        End Function

        Private Function GetValueControl(ByVal f As FilterItem) As Control
            Select Case f.Type
                Case FilterTypes.Boolean
                    Return BuildBooleanSelect(f)
                Case FilterTypes.Number
                    Return BuildIntegerTextbox(f)
                Case FilterTypes.List
                    Return BuildListSelect(f)
                Case FilterTypes.String
                    Return BuildTextBox(f)
                Case FilterTypes.Date
                    Return BuildDateTextbox(f)
                Case Else
                    Return New HtmlGenericControl("SPAN")
            End Select
        End Function

        Private Function BuildDateTextbox(ByVal f As FilterItem) As Control
            Dim txt As New EGVTextBox()
            txt.ID = "ctrl" & f.Expression.Replace(".", "")
            txt.Attributes.Add("data-expression", f.Expression)
            txt.DatePicker = True
            txt.CssClass &= IIf(txt.CssClass <> String.Empty, " ", "") & "date"
            Return txt
        End Function

        Private Function BuildTextBox(ByVal f As FilterItem) As Control
            Dim txt As New EGVTextBox()
            txt.ID = "ctrl" & f.Expression.Replace(".", "")
            txt.Attributes.Add("data-expression", f.Expression)
            Return txt
        End Function

        Private Function BuildListSelect(ByVal f As FilterItem) As Control
            Dim ddl As New EGVDropDown()
            ddl.ID = "ctrl" & f.Expression.Replace(".", "")
            ddl.Attributes.Add("data-expression", f.Expression)
            If f.LookupSource <> String.Empty Then
                Dim q As String = "Select * FROM " & f.LookupSource
                ddl.BindToDataSource(DBA.DataTable(DBA.GetConn(), q), f.LookupDataText, f.LookupDataValue, False)
                Return ddl
            ElseIf f.EnumLookupSource <> String.Empty
                ddl.BindToEnum(Helper.GetEnumType(f.EnumLookupSource), False)
            End If
            Return ddl
        End Function

        Private Function BuildIntegerTextbox(ByVal f As FilterItem) As Control
            Dim txtInteger As New EGVTextBox()
            txtInteger.ID = "ctrl" & f.Expression.Replace(".", "")
            txtInteger.Attributes.Add("data-expression", f.Expression)
            txtInteger.InputNumber = True
            Return txtInteger
        End Function

        Private Function BuildBooleanSelect(ByVal f As FilterItem) As Control
            Dim ddlBoolean As New HtmlGenericControl("Select")
            ddlBoolean.Attributes.Add("class", "form-control")
            ddlBoolean.ID = "ctrl" & f.Expression.Replace(".", "")
            ddlBoolean.Attributes.Add("data-expression", f.Expression)
            'yes
            Dim ddlBOption1 As New HtmlGenericControl("Option")
            ddlBOption1.Attributes.Add("value", True)
            ddlBOption1.InnerText = Localization.GetResource("Resources.Global.CMS.Yes")
            ddlBoolean.Controls.Add(ddlBOption1)
            'no
            Dim ddlBOption2 As New HtmlGenericControl("Option")
            ddlBOption2.Attributes.Add("value", False)
            ddlBOption2.InnerText = Localization.GetResource("Resources.Global.CMS.No")
            ddlBoolean.Controls.Add(ddlBOption2)
            Return ddlBoolean
        End Function

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "egv-filter"
            EGVScriptManager.AddScript(Path.MapCMSScript("Lib/Filter"))
            EnsureChildControls()
        End Sub

        Protected Overrides Sub CreateChildControls()
            hdnDefinition.ID = "hdnFilterDef"
            hdnDefinition.ClientIDMode = ClientIDMode.Static
            hdnDefinition.Value = SetupJSFilterDef()
            Controls.Add(hdnDefinition)
            hdnApplied.ID = "hdnFilterApplied"
            hdnApplied.ClientIDMode = ClientIDMode.Static
            hdnApplied.Value = Helper.JSSerialize(AppliedFilters)
            Controls.Add(hdnApplied)
            'title row
            Dim titleRow As New HtmlGenericControl("DIV")
            titleRow.Attributes.Add("class", "row")
            Controls.Add(titleRow)
            Dim titleContainer As New HtmlGenericControl("DIV")
            titleContainer.Attributes.Add("class", "col-md-12")
            titleRow.Controls.Add(titleContainer)
            Dim title As New HtmlGenericControl("STRONG")
            title.InnerText = Localization.GetResource("Resources.Global.CMS.Filtering")
            titleContainer.Controls.Add(title)
            'filtering row
            Dim topRow As New HtmlGenericControl("DIV")
            topRow.Attributes.Add("class", "row")
            Controls.Add(topRow)
            'filters
            Dim fContainer As New HtmlGenericControl("DIV")
            fContainer.Attributes.Add("class", "col-md-9")
            topRow.Controls.Add(fContainer)
            'add filter field dropdown
            Dim inlineForm As New HtmlGenericControl("DIV")
            inlineForm.Attributes.Add("class", "form-inline")
            fContainer.Controls.Add(inlineForm)
            Dim fGroup As New HtmlGenericControl("DIV")
            fGroup.Attributes.Add("class", "form-group")
            inlineForm.Controls.Add(fGroup)
            Dim ddlFilter As New EGVDropDown()
            ddlFilter.ID = "ddlFilterExpression"
            ddlFilter.ClientIDMode = ClientIDMode.Static
            For Each f As FilterItem In Filters
                ddlFilter.Items.Add(New ListItem(f.DisplayTitle, f.Expression))
            Next
            fGroup.Controls.Add(ddlFilter)
            'operation
            Dim fGroup2 As New HtmlGenericControl("DIV")
            fGroup2.Attributes.Add("class", "form-group")
            inlineForm.Controls.Add(fGroup2)
            Dim ddlOperation As New HtmlGenericControl("Select")
            ddlOperation.Attributes.Add("class", "form-control")
            ddlOperation.ID = "ddlFilterOperation"
            ddlOperation.ClientIDMode = ClientIDMode.Static
            fGroup2.Controls.Add(ddlOperation)
            'value
            Dim fGroup3 As New HtmlGenericControl("DIV")
            fGroup3.Attributes.Add("class", "form-group")
            inlineForm.Controls.Add(fGroup3)
            For Each f As FilterItem In Filters
                fGroup3.Controls.Add(GetValueControl(f))
            Next
            'add button
            Dim fGroup4 As New HtmlGenericControl("DIV")
            fGroup4.Attributes.Add("class", "form-group egv-filter-button-container")
            inlineForm.Controls.Add(fGroup4)
            Dim add As New EGVHyperLink()
            add.ID = "btnFilterAdd"
            add.ClientIDMode = ClientIDMode.Static
            add.NavigateUrl = "javascript:;"
            add.Color = ControlColors.Primary
            fGroup4.Controls.Add(add)
            Dim addIcon As New HtmlGenericControl("SPAN")
            addIcon.Attributes.Add("class", "fa fa-plus")
            add.Controls.Add(addIcon)
            'apply buttons
            Dim aContainer As New HtmlGenericControl("DIV")
            aContainer.Attributes.Add("class", "col-md-3 text-right")
            topRow.Controls.Add(aContainer)
            Dim btnGroup As New HtmlGenericControl("DIV")
            btnGroup.Attributes.Add("class", "btn-group")
            btnGroup.Attributes.Add("role", "group")
            aContainer.Controls.Add(btnGroup)
            'apply
            Dim apply As New EGVLinkButton()
            apply.ID = "lnkFilterApply"
            apply.Color = ControlColors.Olive
            AddHandler apply.Click, AddressOf apply_Click
            btnGroup.Controls.Add(apply)
            Dim applyIcon As New HtmlGenericControl("SPAN")
            applyIcon.Attributes.Add("class", "fa fa-plus")
            apply.Controls.Add(applyIcon)
            apply.Controls.Add(New LiteralControl(" "))
            apply.Controls.Add(New LiteralControl(Localization.GetResource("Resources.Global.CMS.Apply")))
            'clear
            Dim clear As New EGVLinkButton()
            clear.ID = "lnkFilterClear"
            clear.Color = ControlColors.Maroon
            AddHandler clear.Click, AddressOf clear_click
            btnGroup.Controls.Add(clear)
            Dim clearIcon As New HtmlGenericControl("SPAN")
            clearIcon.Attributes.Add("class", "fa fa-times")
            clear.Controls.Add(clearIcon)
            clear.Controls.Add(New LiteralControl(" "))
            clear.Controls.Add(New LiteralControl(Localization.GetResource("Resources.Global.CMS.Clear")))
            'applied filters row
            Dim afRow As New HtmlGenericControl("DIV")
            afRow.Attributes.Add("class", "row")
            Controls.Add(afRow)
            Dim afContainer As New HtmlGenericControl("DIV")
            afContainer.Attributes.Add("class", "col-md-12")
            afContainer.ID = "pnlAppliedFilters"
            afContainer.ClientIDMode = ClientIDMode.Static
            afRow.Controls.Add(afContainer)
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub apply_Click(ByVal sender As Object, ByVal e As EventArgs)
            RaiseEvent ApplyFilters(Helper.JSDeserialize(Of List(Of UserGridColumnFilter))(hdnApplied.Value))
        End Sub

        Protected Sub clear_click(ByVal sender As Object, ByVal e As EventArgs)
            hdnApplied.Value = "[]"
            AppliedFilters.Clear()
            RaiseEvent ApplyFilters(AppliedFilters)
        End Sub

#End Region

    End Class

End Namespace
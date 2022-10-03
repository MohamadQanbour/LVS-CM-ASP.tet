Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports EGV.Utils
Imports EGV.Enums
Imports System.Collections.ObjectModel

Namespace EGVControls

    <ToolboxData("<{0}:InputForm runat=server></{0}:InputForm>"), ParseChildren(True)>
    Public Class EGVInputForm
        Inherits WebControl
        Implements INamingContainer

#Region "Properties"

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property Rows As Collection(Of RowItem)

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property FormTools As Panel = New Panel()

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property FormFooter As Panel

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        Public Property Type As BoxTypes = BoxTypes.Default
        Public Property IconClass As String = String.Empty
        Public Property Title As String = String.Empty
        Public Property DefaultReturnButton As String = String.Empty
        Public Property Collapsable As Boolean = False
        Public Property Solidate As Boolean = False
        Public Property HeaderBorder As Boolean = False
        Public Property NoBorders As Boolean = False

#End Region

#Region "Constructors"

        Public Sub New()
            Rows = New Collection(Of RowItem)
        End Sub

#End Region

#Region "Event Handlers"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            Dim classes As New List(Of String)
            classes.Add("box")
            If Not NoBorders Then classes.Add(GetTypeClass()) Else classes.Add("box-none")
            If Solidate Then classes.Add("box-solid")
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & String.Join(" ", classes.ToArray())
            EnsureChildControls()
        End Sub

        Protected Overrides Sub CreateChildControls()
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
            If FormTools IsNot Nothing Then
                FormTools.CssClass = "pull-right"
                tools.Controls.Add(FormTools)
            End If
            header.Controls.Add(tools)
            If header.HasControls() Then Controls.Add(header)
            'box body
            Dim boxBody As New Panel()
            boxBody.CssClass = "box-body"
            boxBody.EnableViewState = True
            If DefaultReturnButton <> Nothing AndAlso DefaultReturnButton <> String.Empty Then boxBody.DefaultButton = DefaultReturnButton
            Controls.Add(boxBody)
            Dim form As New HtmlGenericControl("div")
            form.Attributes.Add("class", "form-horizontal egv-input-form")
            form.Attributes.Add("role", "form")
            For Each row As RowItem In Rows
                form.Controls.Add(row)
            Next
            boxBody.Controls.Add(form)
            'box footer
            If FormFooter IsNot Nothing Then
                FormFooter.CssClass = "box-footer"
                Controls.Add(FormFooter)
            End If
            Visible = HasControls() And Visible
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

#End Region

    End Class

    <ToolboxData("<{0}:RowItem runat=server></{0}:RowItem>"), ParseChildren(True)>
    Public Class RowItem
        Inherits WebControl
        Implements INamingContainer

#Region "Public Properties"

        Public Property Title As String = String.Empty
        Public Property Required As Boolean = False

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property Content As Panel

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

#End Region

#Region "Event Handlers"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "form-group"
            EnsureChildControls()
        End Sub

        Protected Overrides Sub CreateChildControls()
            Dim lbl As New Label()
            lbl.CssClass = "col-sm-2 control-label"
            If Title <> String.Empty Then
                If Title.StartsWith("Resources.") Then Title = Localization.GetResource(Title)
                lbl.Text = Title
            End If
            If Required Then
                lbl.Text &= IIf(lbl.Text <> String.Empty, " ", "") & "<span class=""text-danger"">*</span>"
            End If
            Controls.Add(lbl)
            Dim div As New HtmlGenericControl("div")
            div.Attributes.Add("class", "col-sm-10" & IIf(Required, " egv-row-required", ""))
            If Content IsNot Nothing Then div.Controls.Add(Content)
            Controls.Add(div)
        End Sub

#End Region

    End Class

End Namespace
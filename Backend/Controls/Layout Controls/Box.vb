Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.ComponentModel
Imports System.Web.UI.HtmlControls
Imports EGV
Imports EGV.Utils

Namespace EGVControls
    <ToolboxData("<{0}:Box runat=server></{0}:Box>")>
    Public Class Box
        Inherits WebControl
        Implements INamingContainer

#Region "Public Properties"

        Public Property Type As Enums.BoxTypes = Enums.BoxTypes.Default
        Public Property IconClass As String = String.Empty
        Public Property Title As String = String.Empty
        Public Property DefaultReturnButton As String = String.Empty
        Public Property Collapsable As Boolean = False
        Public Property Solidate As Boolean = False
        Public Property HeaderBorder As Boolean = True
        Public Property ToolsTooltip As String = String.Empty

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property BoxTools As Panel = New Panel()

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property BoxBody As Panel

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property BoxFooter As Panel

#End Region

#Region "Private Properties"

        Private Property TitleContainer As HtmlGenericControl

#End Region

#Region "Constructors"

        Public Sub New()
            TitleContainer = New HtmlGenericControl("h3")
        End Sub

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
            If Title.StartsWith("Resources.") Then Title = Utils.Localization.GetResource(Title)
            TitleContainer.Attributes.Add("class", "box-title")
            TitleContainer.Controls.Add(New LiteralControl(Title))
            header.Controls.Add(TitleContainer)
            Dim tools As New Panel()
            tools.CssClass = "box-tools pull-right"
            If ToolsTooltip <> Nothing AndAlso ToolsTooltip <> String.Empty Then
                If ToolsTooltip.StartsWith("Resources.") Then ToolsTooltip = Utils.Localization.GetResource(ToolsTooltip)
                tools.Attributes.Add("data-toggle", "tooltip")
                tools.Attributes.Add("title", ToolsTooltip)
            End If
            If Collapsable Then
                Dim btnCollapse As New HtmlGenericControl("button")
                btnCollapse.Attributes.Add("class", "btn btn-box-tool pull-right")
                btnCollapse.Attributes.Add("data-widget", "collapse")
                btnCollapse.Attributes.Add("data-toggle", "tooltip")
                btnCollapse.Attributes.Add("title", Utils.Localization.GetResource("Resources.Global.CMS.CollapseExpand"))
                Dim iconCollapse As New HtmlGenericControl("span")
                iconCollapse.Attributes.Add("class", "fa fa-minus")
                btnCollapse.Controls.Add(iconCollapse)
                tools.Controls.Add(btnCollapse)
            End If
            If BoxTools IsNot Nothing Then
                BoxTools.CssClass = "pull-right"
                tools.Controls.Add(BoxTools)
            End If
            header.Controls.Add(tools)
            If header.HasControls() Then Controls.Add(header)
            'box body
            If BoxBody IsNot Nothing Then
                BoxBody.CssClass = "box-body"
                BoxBody.EnableViewState = True
                If DefaultReturnButton <> Nothing AndAlso DefaultReturnButton <> String.Empty Then BoxBody.DefaultButton = DefaultReturnButton
                Controls.Add(BoxBody)
            End If
            'box footer
            If BoxFooter IsNot Nothing Then
                BoxFooter.CssClass = "box-footer"
                Controls.Add(BoxFooter)
            End If
            Visible = HasControls() And Visible
        End Sub

#End Region

#Region "Public Methods"

        Public Sub LoadTitle(ByVal title As String)
            If title.StartsWith("Resources.") Then title = Utils.Localization.GetResource(title)
            TitleContainer.InnerText = title
        End Sub

#End Region

#Region "Private Methods"

        Private Function GetTypeClass()
            Select Case Type
                Case Enums.BoxTypes.Primary
                    Return "box-primary"
                Case Enums.BoxTypes.Info
                    Return "box-info"
                Case Enums.BoxTypes.Warning
                    Return "box-warning"
                Case Enums.BoxTypes.Success
                    Return "box-success"
                Case Enums.BoxTypes.Danger
                    Return "box-danger"
                Case Else
                    Return "box-default"
            End Select
        End Function

#End Region

    End Class
End Namespace
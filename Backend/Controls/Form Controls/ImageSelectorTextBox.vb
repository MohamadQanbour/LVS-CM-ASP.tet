Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports EGV.Enums
Imports EGV.Utils

Namespace EGVControls

    <ToolboxData("<{0}:ImageSelector runat=server></{0}:ImageSelector>")>
    Public Class ImageSelector
        Inherits WebControl
        Implements INamingContainer

#Region "Public Properties"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        Public Property Size As ControlSizes
        Public Property CMSAssets As Boolean = False

        Public Property Text As String
            Get
                Return txtPath.Text
            End Get
            Set(value As String)
                txtPath.Text = value
            End Set
        End Property


#End Region

#Region "Private Members"

        Private Property txtPath As EGVTextBox
        Private Property AssetsBasePath As String

#End Region

#Region "Constructors"

        Public Sub New()
            txtPath = New EGVTextBox()
        End Sub

#End Region

#Region "Private Methods"

        Private Function GetSizeClass() As String
            Select Case Size
                Case ControlSizes.Small
                    Return "input-group-sm"
                Case ControlSizes.Large
                    Return "input-group-lg"
                Case Else
                    Return ""
            End Select
        End Function

        Private Function GetClasses() As String
            Dim classes As New List(Of String)
            classes.Add("input-group")
            classes.Add("egv-image-selector")
            If Size <> ControlSizes.Default Then classes.Add(GetSizeClass())
            Return String.Join(" ", classes.ToArray())
        End Function

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            EnsureChildControls()
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            EGVScriptManager.AddStyle(Path.MapCMSCss("prettyPhoto"))
            EGVScriptManager.AddScript(Path.MapCMSScript("jquery.prettyPhoto"))
            EGVScriptManager.AddScript(Path.MapCMSScript("lib/egvImageSelector"))
        End Sub

        Protected Overrides Sub CreateChildControls()
            Dim container As New Panel()
            container.CssClass = GetClasses()
            txtPath.CssClass &= IIf(txtPath.CssClass <> String.Empty, " ", "") & "egv-image-selector-text"
            container.Controls.Add(txtPath)
            Dim span As New HtmlGenericControl("SPAN")
            span.Attributes.Add("class", "input-group-btn")
            container.Controls.Add(span)
            Dim preview As New HtmlGenericControl("A")
            preview.Attributes.Add("href", "javascript:;")
            preview.Attributes.Add("data-assetpreview", txtPath.ClientID)
            preview.Attributes.Add("class", "btn btn-warning btn-flat")
            span.Controls.Add(preview)
            Dim picon As New HtmlGenericControl("I")
            picon.Attributes.Add("class", "fa fa-picture-o")
            preview.Controls.Add(picon)
            AssetsBasePath = IIf(CMSAssets, Path.MapCMSAsset(""), Path.MapPortalAsset(""))
            Dim btn As New HtmlGenericControl("A")
            btn.Attributes.Add("href", "javascript:;")
            btn.Attributes.Add("class", "btn btn-info btn-flat")
            btn.Attributes.Add("data-openassets", AssetsBasePath)
            btn.Attributes.Add("data-target", "egv-image-selector-text")
            span.Controls.Add(btn)
            Dim icon As New HtmlGenericControl("I")
            icon.Attributes.Add("class", "fa fa-folder-open")
            btn.Controls.Add(icon)
            Controls.Add(container)
            Dim am As New AssetsManager()
            am.IsCMSPath = CMSAssets
            Controls.Add(am)
        End Sub

#End Region

    End Class

End Namespace
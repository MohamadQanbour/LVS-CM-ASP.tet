Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.Web
Imports EGV
Imports EGV.Enums
Imports EGV.Utils

Namespace EGVControls

    Public Class SingleFileUploader
        Inherits WebControl
        Implements INamingContainer

#Region "Public Properties"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        Public Property Color As ControlColors
        Public Property Size As ControlSizes
        Public Property Text As String

        Public ReadOnly Property File As HttpPostedFile
            Get
                Return Helper.Request.Files(0)
            End Get
        End Property

        Public ReadOnly Property HasFile As Boolean
            Get
                Return Helper.Request.Files.Count > 0 AndAlso File.ContentLength > 0
            End Get
        End Property

        Public Property InfoText As String
            Get
                Return infoContainer.InnerText
            End Get
            Set(value As String)
                infoContainer.InnerText = value
            End Set
        End Property

        Public Property AllowedFileTypes As String

#End Region

#Region "Private Properties"

        Private Property fileUpload As FileUpload
        Private Property infoContainer As HtmlGenericControl

#End Region

#Region "Constructors"

        Public Sub New()
            fileUpload = New FileUpload()
            infoContainer = New HtmlGenericControl("P")
            Color = ControlColors.Default
            Size = ControlSizes.Default
        End Sub

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
        End Sub

        Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            writer.AddAttribute("data-singlefileuploader", "true")
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If Text <> Nothing AndAlso Text.StartsWith("Resources.") Then Text = Localization.GetResource(Text)
            If InfoText <> Nothing AndAlso InfoText.StartsWith("Resources.") Then InfoText = Localization.GetResource(InfoText)
            EGVScriptManager.AddScript(Path.MapCMSScript("lib/SingleFileUploader"))
        End Sub

        Protected Overrides Sub CreateChildControls()
            Dim pnl As New Panel()
            Dim classes As New List(Of String)
            classes.Add("btn")
            classes.Add("btn-file")
            Select Case Size
                Case ButtonSizes.Large
                    classes.Add("btn-lg")
                Case ButtonSizes.Small
                    classes.Add("btn-sm")
                Case ButtonSizes.XSmall
                    classes.Add("btn-xs")
            End Select
            Select Case Color
                Case ControlColors.Danger
                    classes.Add("btn-danger")
                Case ControlColors.Info
                    classes.Add("btn-info")
                Case ControlColors.Maroon
                    classes.Add("bg-maroon")
                Case ControlColors.Navy
                    classes.Add("bg-navy")
                Case ControlColors.Olive
                    classes.Add("bg-olive")
                Case ControlColors.Orange
                    classes.Add("bg-orange")
                Case ControlColors.Primary
                    classes.Add("btn-primary")
                Case ControlColors.Purple
                    classes.Add("bg-purple")
                Case ControlColors.Warning
                    classes.Add("btn-warning")
                Case ControlColors.Success
                    classes.Add("btn-success")
                Case ControlColors.Default
                    classes.Add("btn-default")
            End Select
            If classes.Count > 0 Then pnl.CssClass = String.Join(" ", classes.ToArray())
            Controls.Add(pnl)
            Dim i As New HtmlGenericControl("I")
            i.Attributes.Add("class", "fa fa-upload")
            pnl.Controls.Add(i)
            pnl.Controls.Add(New LiteralControl(Text))
            pnl.Controls.Add(fileUpload)
            If AllowedFileTypes <> String.Empty Then fileUpload.Attributes.Add("accept", AllowedFileTypes)
            Dim removeBtn As New HtmlGenericControl("A")
            Dim removeClass As String = "btn btn-danger"
            Select Case Size
                Case ButtonSizes.Large
                    removeClass &= " btn-lg"
                Case ButtonSizes.Small
                    removeClass &= " btn-sm"
                Case ButtonSizes.XSmall
                    removeClass &= " btn-xs"
            End Select
            removeBtn.Attributes.Add("class", removeClass)
            removeBtn.Attributes.Add("href", "#")
            Controls.Add(removeBtn)
            Dim removeIcon As New HtmlGenericControl("I")
            removeIcon.Attributes.Add("class", "fa fa-times")
            removeBtn.Controls.Add(removeIcon)
            Controls.Add(infoContainer)
            infoContainer.Attributes.Add("class", "help-block")
        End Sub

#End Region

    End Class

End Namespace
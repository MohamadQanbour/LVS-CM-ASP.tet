Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.ComponentModel
Imports EGV.Utils
Imports EGV.Enums

Namespace EGVControls

    <ToolboxData("<{0}:Modal runat=server></{0}:Modal>")>
    Public Class Modal
        Inherits WebControl
        Implements INamingContainer

#Region "Public Members"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        Public Property Title As String
        Public Property Size As ControlSizes = ControlSizes.Default
        Public Property Animate As Boolean = True

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property Body As Panel

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property Footer As Panel

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "modal" & IIf(Animate, " fade", "")
        End Sub

        Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            writer.AddAttribute("id", ID)
            writer.AddAttribute("tabindex", "-1")
            writer.AddAttribute("role", "dialog")
            writer.AddAttribute("aria-labelledby", ID & "Label")
        End Sub

        Protected Overrides Sub CreateChildControls()
            Dim doc As New HtmlGenericControl("DIV")
            Dim docClasses As New List(Of String)
            docClasses.Add("modal-dialog")
            Select Case Size
                Case ControlSizes.Large
                    docClasses.Add("modal-lg")
                Case ControlSizes.Small
                    docClasses.Add("modal-sm")
            End Select
            doc.Attributes.Add("class", String.Join(" ", docClasses.ToArray()))
            doc.Attributes.Add("role", "document")
            Controls.Add(doc)
            Dim content As New HtmlGenericControl("DIV")
            content.Attributes.Add("class", "modal-content")
            doc.Controls.Add(content)
            'header
            Dim header As New HtmlGenericControl("DIV")
            content.Controls.Add(header)
            header.Attributes.Add("class", "modal-header")
            'close
            Dim btnClose As New HtmlGenericControl("BUTTON")
            btnClose.Attributes.Add("type", "button")
            btnClose.Attributes.Add("class", "close")
            btnClose.Attributes.Add("data-dismiss", "modal")
            btnClose.Attributes.Add("aria-label", "Close")
            btnClose.InnerHtml = "<span aria-hidden=""true"">&times;</span>"
            header.Controls.Add(btnClose)
            'title
            Dim litTitle As New HtmlGenericControl("H4")
            litTitle.Attributes.Add("class", "modal-title")
            litTitle.Attributes.Add("id", ID + "Label")
            If Title.StartsWith("Resources.") Then Title = Localization.GetResource(Title)
            litTitle.InnerText = Title
            header.Controls.Add(litTitle)
            If Body IsNot Nothing Then
                Body.CssClass &= IIf(Body.CssClass <> String.Empty, " ", "") & "modal-body"
                content.Controls.Add(Body)
            End If
            If Footer IsNot Nothing Then
                Footer.CssClass &= IIf(Footer.CssClass <> String.Empty, " ", "") & "modal-footer"
                content.Controls.Add(Footer)
            End If
        End Sub

#End Region

    End Class

End Namespace
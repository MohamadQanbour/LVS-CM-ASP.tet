Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports EGV.Utils
Imports EGV.Enums
Imports B = EGV.Business

Namespace EGVControls

    Public Class SaveCancel
        Inherits WebControl
        Implements INamingContainer

#Region "Events"

        Public Event SaveClick(ByVal sender As Object, ByVal e As EventArgs, ByRef hasError As Boolean)

#End Region

#Region "Public Properties"

        Public Property IsEditing As Boolean = False
        Public Property BackPagePath As String = String.Empty
        Public Property AddPagePath As String = String.Empty
        Public Property EditPagePath As String = String.Empty
        Public Property NewId As Integer = 0
        Public Property ValidationGroup As String

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

#End Region

#Region "Private Members"

        Private cancel As EGVHyperLink
        Private save As EGVLinkButton
        Private saveclose As EGVLinkButton
        Private saveadd As EGVLinkButton

#End Region

#Region "Event Handlers"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            EnsureChildControls()
        End Sub

        Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            writer.AddAttribute("class", "egv-savecancel")
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            Page.Form.DefaultButton = saveclose.UniqueID
            If IsEditing Then
                cancel.Text = "Resources.Global.CMS.Close"
                saveadd.Visible = False
            Else
                cancel.Text = "Resources.Global.CMS.Cancel"
                saveadd.Visible = True
            End If
            If BackPagePath <> String.Empty Then
                cancel.NavigateUrl = BackPagePath
                saveclose.Visible = True
            Else
                cancel.NavigateUrl = "javascript: window.history.back();"
                saveclose.Visible = False
            End If
            'hiding extra save buttons permanently
            saveadd.Visible = False
            save.Visible = False
        End Sub

        Protected Overrides Sub CreateChildControls()
            'cancel button
            cancel = New EGVHyperLink()
            cancel.Color = ControlColors.Default
            cancel.FlatButton = True
            Controls.Add(cancel)
            'save close
            saveclose = New EGVLinkButton()
            saveclose.Attributes.Add("role", "save")
            saveclose.Color = ControlColors.Primary
            saveclose.FlatButton = True
            saveclose.CssClass = "pull-right"
            saveclose.Text = "Resources.Global.CMS.SaveClose"
            saveclose.ValidationGroup = ValidationGroup
            AddHandler saveclose.Click, AddressOf saveclose_Click
            Controls.Add(saveclose)
            'save
            save = New EGVLinkButton()
            save.Attributes.Add("role", "save")
            save.Color = ControlColors.Primary
            save.FlatButton = True
            save.CssClass = "pull-right"
            save.Text = "Resources.Global.CMS.Save"
            save.ValidationGroup = ValidationGroup
            AddHandler save.Click, AddressOf save_Click
            Controls.Add(save)
            'save add another
            saveadd = New EGVLinkButton()
            saveadd.Attributes.Add("role", "save")
            saveadd.Color = ControlColors.Navy
            saveadd.FlatButton = True
            saveadd.CssClass = "pull-right"
            saveadd.Text = "Resources.Global.CMS.SaveAdd"
            saveadd.ValidationGroup = ValidationGroup
            AddHandler saveadd.Click, AddressOf saveadd_Click
            Controls.Add(saveadd)
        End Sub

        Protected Sub saveclose_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim hasError As Boolean = False
            RaiseEvent SaveClick(sender, e, hasError)
            If BackPagePath <> String.Empty AndAlso Not hasError Then Helper.Response.Redirect(BackPagePath & IIf(BackPagePath.Contains("?"), "&", "?") & "cmd=1")
        End Sub

        Protected Sub saveadd_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim hasError As Boolean = False
            RaiseEvent SaveClick(sender, e, hasError)
            If AddPagePath <> String.Empty AndAlso Not hasError Then Helper.Response.Redirect(AddPagePath & IIf(AddPagePath.Contains("?"), "&", "?") & "cmd=1")
        End Sub

        Protected Sub save_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim hasError As Boolean = False
            RaiseEvent SaveClick(sender, e, hasError)
            If NewId > 0 AndAlso EditPagePath <> String.Empty AndAlso Not hasError Then Helper.Response.Redirect(String.Format(EditPagePath, NewId) & IIf(EditPagePath.Contains("?"), "&", "?") & "cmd=1")
        End Sub

#End Region

    End Class

End Namespace
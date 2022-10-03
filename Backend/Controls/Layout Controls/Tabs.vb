Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports System.ComponentModel
Imports EGV.Utils
Imports System.Collections.ObjectModel

Namespace EGVControls

    <ToolboxData("<{0}:EGVTabs runat=server></{0}:EGVTabs>")>
    Public Class EGVTabs
        Inherits WebControl
        Implements INamingContainer

#Region "Public Properties"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        Public Property Title As String
        Public Property IconClass As String

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property Tabs As Collection(Of EGVTabItem)

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property TabContents As Collection(Of EGVTabContent)

        <Browsable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property TabFooter As Panel

#End Region

#Region "Private Members"

        Private Property RenderedTabs As Collection(Of Control)

#End Region

#Region "Constructors"

        Public Sub New()
            Tabs = New Collection(Of EGVTabItem)()
            TabContents = New Collection(Of EGVTabContent)()
            RenderedTabs = New Collection(Of Control)()
        End Sub

#End Region

#Region "Public Methods"

        Public Sub HideTab(ByVal id As String)
            Dim colTabs = (From t As EGVTabItem In Tabs Where t.Id = id)
            If colTabs.Count() = 1 Then
                Dim t = colTabs.FirstOrDefault()
                Dim index As Integer = Tabs.IndexOf(t)
                RenderedTabs(index).Visible = False
                TabContents(index).Visible = False
            End If
        End Sub

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "nav-tabs-custom"
            EnsureChildControls()
        End Sub

        Protected Overrides Sub CreateChildControls()
            Dim ul As New HtmlGenericControl("UL")
            ul.Attributes.Add("class", "nav nav-tabs pull-right")
            Dim pnlContents As New HtmlGenericControl("DIV")
            pnlContents.Attributes.Add("class", "tab-content")
            For i As Integer = 1 To Tabs.Count
                Dim cur = Tabs(i - 1)
                Dim curContent = TabContents(i - 1)
                If cur.Visible Then
                    Dim li As New HtmlGenericControl("LI")
                    If i = 1 Then li.Attributes.Add("class", "active")
                    Dim a As New HtmlGenericControl("A")
                    a.Attributes.Add("href", "#Tab" & i)
                    a.Attributes.Add("data-toggle", "tab")
                    If cur.Title.StartsWith("Resources.") Then cur.Title = Localization.GetResource(cur.Title)
                    a.Controls.Add(New LiteralControl(cur.Title))
                    li.Controls.Add(a)
                    ul.Controls.Add(li)
                    RenderedTabs.Add(a)
                    'content
                    If i = 1 Then curContent.CssClass &= IIf(curContent.CssClass <> String.Empty, " ", "") & "active"
                    curContent.ID = "Tab" & i
                    curContent.ClientIDMode = ClientIDMode.Static
                    pnlContents.Controls.Add(curContent)
                End If
            Next
            If Title <> String.Empty Then
                Dim li As New HtmlGenericControl("LI")
                li.Attributes.Add("class", "pull-left header")
                If IconClass <> String.Empty Then
                    Dim i As New HtmlGenericControl("I")
                    i.Attributes.Add("class", IconClass)
                    li.Controls.Add(i)
                    li.Controls.Add(New LiteralControl(" "))
                End If
                If Title.StartsWith("Resources.") Then Title = Localization.GetResource(Title)
                li.Controls.Add(New LiteralControl(Title))
                ul.Controls.Add(li)
            End If
            Controls.Add(ul)
            Controls.Add(pnlContents)
            If TabFooter IsNot Nothing Then
                TabFooter.CssClass &= IIf(TabFooter.CssClass <> String.Empty, " ", "") & "tab-footer"
                Controls.Add(TabFooter)
            End If
        End Sub

#End Region

    End Class

    Public Class EGVTabContent
        Inherits Panel

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "tab-pane"
        End Sub

    End Class

    Public Class EGVTabItem

#Region "Public Properties"

        Public Property Title As String
        Public Property Visible As Boolean
        Public Property Id As String

#End Region

#Region "Constructors"

        Public Sub New()
            Helper.LoadLanguage()
            Visible = True
        End Sub

#End Region

    End Class

End Namespace
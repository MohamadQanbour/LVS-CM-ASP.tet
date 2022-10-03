Imports System.Web.UI
Imports System.ComponentModel
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports EGV.Enums
Imports EGV.Utils
Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Namespace EGVControls

    Public Class Toolbar
        Inherits WebControl
        Implements INamingContainer

#Region "Events"

        Public Event ToolbarButtonClick(ByVal cmd As String)

#End Region

#Region "Public Properties"

        <Bindable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property ButtonGroups As Collection(Of ToolbarButtonGroup)

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
            ButtonGroups = New Collection(Of ToolbarButtonGroup)()
        End Sub

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "egv-toolbar btn-toolbar"
        End Sub

        Protected Overrides Sub CreateChildControls()
            Visible = ButtonGroups.Count > 0
            For Each grp As ToolbarButtonGroup In ButtonGroups
                AddHandler grp.GroupButtonClick, AddressOf click
                Controls.Add(grp)
            Next
        End Sub

#End Region

#Region "Public Methods"

        Public Sub Hide(ByVal id As String)
            For Each g As ToolbarButtonGroup In ButtonGroups
                If g.Hide(id) Then Exit For
            Next
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub click(ByVal cmd As String)
            RaiseEvent ToolbarButtonClick(cmd)
        End Sub

#End Region

    End Class

    Public Class ToolbarButtonGroup
        Inherits WebControl
        Implements INamingContainer

#Region "Event"

        Friend Event GroupButtonClick(ByVal cmd As String)

#End Region

#Region "Properties"

        <Bindable(False), PersistenceMode(PersistenceMode.InnerProperty)>
        Public Property Buttons As Collection(Of ToolbarButton)

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
            Buttons = New Collection(Of ToolbarButton)()
        End Sub

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "btn-group"
            EnsureChildControls()
        End Sub

        Protected Overrides Sub CreateChildControls()
            For Each btn As ToolbarButton In Buttons
                Dim ctrl = btn.Render()
                If Not btn.IsHyperlink Then AddHandler DirectCast(ctrl, LinkButton).Click, AddressOf btn_Click
                Controls.Add(ctrl)
            Next
        End Sub

#End Region

#Region "Public Methods"

        Public Function Hide(ByVal id As String) As Boolean
            For Each btn As ToolbarButton In Buttons
                If btn.ID = id Then
                    btn.Visible = False
                    Return True
                End If
            Next
            Return False
        End Function

#End Region

#Region "Event Handlers"

        Protected Sub btn_Click(ByVal sender As Object, ByVal e As EventArgs)
            RaiseEvent GroupButtonClick(DirectCast(sender, LinkButton).CommandName)
        End Sub

#End Region

    End Class

    Public Class ToolbarButton

#Region "Public Properties"

        Public Property Text As String = String.Empty
        Public Property IconClass As String = String.Empty
        Public Property Color As ControlColors = ControlColors.Default
        Public Property Size As ControlSizes = ControlSizes.Default
        Public Property ActiveState As ToolbarActiveStates = ToolbarActiveStates.Always
        Public Property ID As String
        Public Property Visible As Boolean = True
        Public Property ShouldConfirm As Boolean = False
        Public Property IsHyperlink As Boolean = False
        Public Property NavigateUrl As String = String.Empty
        Public Property CommandName As String = String.Empty

#End Region

#Region "Public Methods"

        Public Function Render() As WebControl
            Dim ret As WebControl
            If Not IsHyperlink Then
                Dim hyp As New LinkButton()
                If ShouldConfirm Then hyp.OnClientClick = "return confirm('" & Localization.GetResource("Resources.Global.CMS.AreYouSure") & "');"
                SetupControl(hyp)
                hyp.CommandName = CommandName
                ret = hyp
            Else
                Dim hyp As New HyperLink()
                If NavigateUrl <> String.Empty Then hyp.NavigateUrl = NavigateUrl
                SetupControl(hyp)
                ret = hyp
            End If
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Sub SetupControl(ByRef ctrl As WebControl)
            ctrl.ID = ID
            ctrl.ClientIDMode = ClientIDMode.Static
            Dim classes As New List(Of String)
            classes.Add("btn")
            classes.Add("egv-toolbar-btn")
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
                Case ControlColors.Success
                    classes.Add("btn-success")
                Case ControlColors.Warning
                    classes.Add("btn-warning")
                Case Else
                    classes.Add("btn-default")
            End Select
            Select Case Size
                Case ControlSizes.Large
                    classes.Add("btn-lg")
                Case ControlSizes.Small
                    classes.Add("btn-sm")
            End Select
            ctrl.Attributes.Add("data-activestate", System.Enum.GetName(GetType(ToolbarActiveStates), ActiveState).ToLower())
            If ActiveState <> ToolbarActiveStates.None AndAlso ActiveState <> ToolbarActiveStates.Always Then classes.Add("disabled")
            ctrl.CssClass &= IIf(ctrl.CssClass <> String.Empty, " ", "") & String.Join(" ", classes.ToArray())
            If IconClass <> String.Empty Then
                Dim lbl As New Label()
                lbl.CssClass = IconClass
                ctrl.Controls.Add(lbl)
            End If
            Dim title As String = Text
            If title <> String.Empty Then
                If title.StartsWith("Resources.") Then title = Localization.GetResource(title)
                ctrl.Controls.Add(New LiteralControl(title))
            End If
            ctrl.Visible = Visible
        End Sub

#End Region

    End Class

End Namespace
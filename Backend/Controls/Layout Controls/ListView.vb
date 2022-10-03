Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.ObjectModel
Imports EGV.Utils

Namespace EGVControls

    <ToolboxData("<{0}:EGVListViewItem runat=server></{0}:EGVListViewItem>")>
    Public Class EGVListViewItem
        Inherits EGVHyperLink
        Implements INamingContainer

#Region "Public Properties"

        Public Property IconClass As String

#End Region

#Region "Constructors"

        Public Sub New()
            AppButton = True
        End Sub

#End Region

#Region "overridden methods"

        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()
            Dim lbl As New Label()
            lbl.CssClass = IconClass
            Controls.Add(lbl)
            If Text.StartsWith("Resources.") Then Text = Localization.GetResource(Text)
            Controls.Add(New LiteralControl(Text))
        End Sub

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
        End Sub

#End Region

    End Class

    <ToolboxData("<{0}:EGVListView runat=server></{0}:EGVListView>")>
    Public Class EGVListView
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
        Public Property Items As Collection(Of EGVListViewItem)

#End Region

#Region "Constructors"

        Public Sub New()
            Items = New Collection(Of EGVListViewItem)()
        End Sub

#End Region

#Region "Public Methods"

        Public Sub BindToDataSource(ByVal lst As DataTable, ByVal iconField As String, ByVal titleField As String, ByVal linkField As String)
            For Each dr As DataRow In lst.Rows
                Items.Add(New EGVListViewItem() With {
                    .IconClass = Helper.GetSafeDBValue(dr(iconField)),
                    .Text = Helper.GetSafeDBValue(dr(titleField)),
                    .NavigateUrl = Path.MapCMSFile(Helper.GetSafeDBValue(dr(linkField)))
                })
            Next
        End Sub

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "egv-list-view"
        End Sub

        Protected Overrides Sub CreateChildControls()
            Dim box As New Box()
            box.Title = Title
            box.IconClass = IconClass
            box.Type = EGV.Enums.BoxTypes.Info
            box.BoxBody = New Panel()
            For Each item As EGVListViewItem In Items
                box.BoxBody.Controls.Add(item)
            Next
            Controls.Add(box)
        End Sub

#End Region

    End Class

End Namespace
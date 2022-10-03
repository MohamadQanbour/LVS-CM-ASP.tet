Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports EGV.Utils

Namespace EGVControls
    <ToolboxData("<{0}:EGVPager runar=server></{0}:EGVPager>")>
    Public Class EGVPager
        Inherits WebControl
        Implements INamingContainer

#Region "Events"

        Public Event PageIndexChanged(ByVal sender As Object, ByVal e As EventArgs, ByVal newPageIndex As Integer)

#End Region

#Region "Properties"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        Public Property TotalRecords As Integer
        Public Property PageIndex As Integer
        Public Property PageSize As Integer
        Public Property LimitPages As Integer = 9
        Public Property IsFullPager As Boolean = True

#End Region

#Region "Private Properties"

        Private Property litCurrentPage As LiteralControl
        Private Property litTotalPages As LiteralControl
        Private Property litTotalRecords As LiteralControl
        Private Property lnkFirst As LinkButton
        Private Property lnkLast As LinkButton
        Private Property lnkPrev As LinkButton
        Private Property lnkNext As LinkButton
        Private Property lstPages As List(Of LinkButton)

#End Region

#Region "Constructors"

        Public Sub New()
            litCurrentPage = New LiteralControl()
            litTotalPages = New LiteralControl()
            litTotalRecords = New LiteralControl()
            lnkFirst = New LinkButton()
            lnkLast = New LinkButton()
            lnkPrev = New LinkButton()
            lnkNext = New LinkButton()
            lstPages = New List(Of LinkButton)()
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub page_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim page As Integer = DirectCast(sender, LinkButton).Attributes("targetpage")
            RaiseEvent PageIndexChanged(sender, e, page)
        End Sub

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "egv-pager" & IIf(Not IsFullPager, " btn-group", " row")
            EnsureChildControls()
        End Sub

        Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If IsFullPager Then writer.AddAttribute("role", "group")
        End Sub

        Protected Overrides Sub CreateChildControls()
            Dim container As ControlCollection = Controls
            If IsFullPager Then
                Dim left As New Panel()
                left.CssClass = "col-md-5 pager-info"
                left.Controls.Add(New LiteralControl(Localization.GetResource("Resources.Global.CMS.Page") & " "))
                left.Controls.Add(litCurrentPage)
                left.Controls.Add(New LiteralControl(" " & Localization.GetResource("Resources.Global.CMS.Of") & " "))
                left.Controls.Add(litTotalPages)
                left.Controls.Add(New LiteralControl(", " & Localization.GetResource("Resources.Global.CMS.Total") & " "))
                left.Controls.Add(litTotalRecords)
                left.Controls.Add(New LiteralControl(" " & Localization.GetResource("Resources.Global.CMS.Items") & "."))
                container.Add(left)
                Dim right As New Panel()
                right.CssClass = "col-md-7"
                container.Add(right)
                Dim tool As New Panel()
                tool.CssClass = "btn-toolbar pull-right"
                right.Controls.Add(tool)
                Dim group As New Panel()
                group.CssClass = "btn-group"
                group.Attributes.Add("role", "group")
                tool.Controls.Add(group)
                container = group.Controls
            End If
            'first
            lnkFirst.CssClass = "btn btn-default btn-sm"
            lnkFirst.Attributes.Add("targetpage", 0)
            AddHandler lnkFirst.Click, AddressOf page_Click
            Dim lblFirst As New Label()
            lblFirst.CssClass = "fa fa-fast-" & IIf(Helper.Language.IsRTL, "forward", "backward")
            lnkFirst.Controls.Add(lblFirst)
            container.Add(lnkFirst)

            'previous
            lnkPrev.CssClass = "btn btn-default btn-sm"
            AddHandler lnkPrev.Click, AddressOf page_Click
            Dim lblPrev As New Label()
            lblPrev.CssClass = "fa fa-step-" & IIf(Helper.Language.IsRTL, "forward", "backward")
            lnkPrev.Controls.Add(lblPrev)
            container.Add(lnkPrev)

            'pages
            For i As Integer = 0 To LimitPages - 1
                Dim a As New LinkButton()
                a.CssClass = "btn btn-default btn-sm"
                AddHandler a.Click, AddressOf page_Click
                lstPages.Add(a)
                container.Add(a)
            Next

            'next
            lnkNext.CssClass = "btn btn-default btn-sm"
            AddHandler lnkNext.Click, AddressOf page_Click
            Dim lblNext As New Label()
            lblNext.CssClass = "fa fa-step-" & IIf(Helper.Language.IsRTL, "backward", "forward")
            lnkNext.Controls.Add(lblNext)
            container.Add(lnkNext)

            'last
            lnkLast.CssClass = "btn btn-default btn-sm"
            AddHandler lnkLast.Click, AddressOf page_Click
            Dim lblLast As New Label()
            lblLast.CssClass = "fa fa-fast-" & IIf(Helper.Language.IsRTL, "backward", "forward")
            lnkLast.Controls.Add(lblLast)
            container.Add(lnkLast)
        End Sub

#End Region

#Region "Private Methods"

        Private Function GetPagesList(ByVal curPage As Integer, ByVal limit As Integer, ByVal numOfPages As Integer) As Integer()
            Dim ret(limit - 1) As Integer
            Dim median As Integer = CInt(Math.Floor(CDec(limit - 1) / 2))
            If curPage > median AndAlso curPage < (numOfPages - median - 1) Then
                For i As Integer = 0 To limit - 1
                    ret(i) = (curPage - median) + i
                Next
            ElseIf curPage <= median Then
                For i As Integer = 0 To limit - 1
                    ret(i) = i
                Next
            Else
                For i As Integer = 0 To limit - 1
                    ret(limit - 1 - i) = numOfPages - 1 - i
                Next
            End If
            Return ret
        End Function

#End Region

#Region "Public Methods"

        Public Sub BindPager(ByVal total As Integer, ByVal selPageSize As Integer, ByVal curPageIndex As Integer)
            TotalRecords = total
            PageSize = selPageSize
            PageIndex = curPageIndex
            Dim numOfPages As Integer = CInt(Math.Ceiling(CDec(TotalRecords) / CDec(PageSize)))
            LimitPages = IIf(numOfPages > LimitPages, LimitPages, numOfPages)
            If PageIndex > numOfPages Then PageIndex = 0
            litCurrentPage.Text = (PageIndex + 1).ToString()
            litTotalPages.Text = numOfPages
            litTotalRecords.Text = TotalRecords
            'last & next
            lnkLast.Attributes.Add("targetpage", numOfPages - 1)
            lnkNext.Attributes.Add("targetpage", PageIndex + 1)
            If numOfPages <= 1 OrElse PageIndex = numOfPages - 1 Then
                lnkLast.CssClass &= IIf(lnkLast.CssClass <> String.Empty, " ", "") & "disabled"
                lnkNext.CssClass &= IIf(lnkNext.CssClass <> String.Empty, " ", "") & "disabled"
            Else
                lnkLast.CssClass = lnkLast.CssClass.Replace("disabled", "")
                lnkNext.CssClass = lnkNext.CssClass.Replace("disabled", "")
            End If
            'previous & first
            lnkPrev.Attributes.Add("targetpage", PageIndex - 1)
            If PageIndex <= 0 OrElse numOfPages <= 1 Then
                lnkPrev.CssClass &= IIf(lnkPrev.CssClass <> String.Empty, " ", "") & "disabled"
                lnkFirst.CssClass &= IIf(lnkFirst.CssClass <> String.Empty, " ", "") & "disabled"
            Else
                lnkPrev.CssClass = lnkPrev.CssClass.Replace("disabled", "")
                lnkFirst.CssClass = lnkFirst.CssClass.Replace("disabled", "")
            End If
            'pages
            Dim index As Integer = 0
            For Each a As LinkButton In lstPages
                a.Visible = index < LimitPages
                index = index + 1
            Next
            Dim pages() As Integer = GetPagesList(PageIndex, LimitPages, numOfPages)
            For i As Integer = 0 To LimitPages - 1
                Dim lnk As LinkButton = lstPages(i)
                lnk.Attributes.Add("targetpage", pages(i))
                lnk.Text = (pages(i) + 1).ToString()
                If pages(i) = PageIndex Then
                    lnk.CssClass = lnk.CssClass.Replace("btn-default", "bg-olive")
                Else
                    lnk.CssClass = lnk.CssClass.Replace("bg-olive", "btn-default")
                End If
            Next
        End Sub

#End Region

    End Class
End Namespace
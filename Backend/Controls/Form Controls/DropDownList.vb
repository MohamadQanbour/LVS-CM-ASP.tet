Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports EGV.Enums
Imports EGV.Utils

Namespace EGVControls
    <ToolboxData("<{0}:jDropDownList runat=server></{0}:jDropDownList>")>
    Public Class EGVDropDown
        Inherits DropDownList

#Region "Public Properties"

        Public Property Size As ControlSizes = ControlSizes.Default
        Public Property Bootstraped As Boolean = True
        Public Property AutoComplete As Boolean = False
        Public Property AutoCompleteSource As String = String.Empty
        Public Property ExcludeId As Integer = 0
        Public Property AllowNull As Boolean = False
        Public Property AllowMultiple As Boolean = False
        Public Property Placeholder As String = String.Empty

#End Region

#Region "Public Methods"

        Public Sub BindToEnum(ByVal enumType As Type, Optional ByVal allowNull As Boolean = True, Optional ByVal nullText As String = "Resources.Global.CMS.None",
                              Optional ByVal nullValue As Integer = 0)
            Dim values = System.Enum.GetValues(enumType)
            If allowNull Then
                If nullText.StartsWith("Resources.") Then nullText = Localization.GetResource(nullText)
                Items.Add(New ListItem(nullText, nullValue))
            End If
            For Each v As Integer In values
                Items.Add(New ListItem(System.Enum.GetName(enumType, v), v))
            Next
        End Sub

        Public Sub BindToDataSource(ByVal lst As DataTable, ByVal dataText As String, ByVal dataValue As String,
                                    Optional ByVal allowNull As Boolean = False, Optional ByVal nullValue As String = "0",
                                    Optional ByVal nullText As String = "Resources.Global.CMS.None")
            DataTextField = dataText
            DataValueField = dataValue
            DataSource = lst
            DataBind()
            If allowNull Then
                If nullText.StartsWith("Resources.") Then nullText = Localization.GetResource(nullText)
                Items.Insert(0, New ListItem(nullText, nullValue))
            End If
        End Sub

        Public Sub AddDataSourceItems(ByVal lst As DataTable, ByVal dataText As String, ByVal dataValue As String,
                                      Optional ByVal allowNull As Boolean = False, Optional ByVal nullValue As String = "0",
                                    Optional ByVal nullText As String = "Resources.Global.CMS.None")
            If allowNull Then
                If nullText.StartsWith("Resources.") Then nullText = Localization.GetResource(nullText)
                Items.Add(New ListItem(nullText, nullValue))
            End If
            For Each dr As DataRow In lst.Rows
                Items.Add(New ListItem(dr(dataText), dr(dataValue)))
            Next
        End Sub

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            Helper.LoadLanguage()
            Dim classes As New List(Of String)
            If Bootstraped Then classes.Add("form-control")
            Select Case Size
                Case ControlSizes.Large
                    classes.Add("input-lg")
                Case ControlSizes.Small
                    classes.Add("input-sm")
            End Select
            If AutoComplete Then
                classes.Add("select2")
                Style.Add("width", Unit.Percentage(100).ToString())
            End If
            If classes.Count > 0 Then CssClass &= IIf(CssClass <> String.Empty, " ", "") & String.Join(" ", classes)
            MyBase.OnInit(e)
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            EGVScriptManager.AddStyle(Path.MapCMSCss("select2.min"))
            EGVScriptManager.AddScript(Path.MapCMSScript("select2.full.min"))
        End Sub

        Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If AutoComplete Then
                writer.AddAttribute("data-autocompletesource", AutoCompleteSource)
                writer.AddAttribute("data-excludeid", ExcludeId.ToString())
                writer.AddAttribute("data-allownull", AllowNull.ToString())
            End If
            If AllowMultiple Then
                writer.AddAttribute("multiple", "multiple")
            End If
            If Placeholder <> String.Empty Then
                If Placeholder.StartsWith("Resources.") Then Placeholder = Localization.GetResource(Placeholder)
                writer.AddAttribute("data-placeholder", Placeholder)
            End If
        End Sub

#End Region

    End Class
End Namespace
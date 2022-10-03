Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports EGV.Enums
Imports EGV.Structures
Imports EGV.Utils
Imports EGV.Business
Imports EGV
Imports System.Text

Namespace EGVControls

    <ToolboxData("<{0}:MapMarkerSpecifier runat=server></{0}:MapMarkerSpecifier>")>
    Public Class MapMarkerSpecifier
        Inherits WebControl
        Implements INamingContainer

#Region "Public Members"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

#End Region

#Region "Private Members"

        Private Property hdnLatitude As HiddenField
        Private Property hdnLongitude As HiddenField

#End Region

#Region "Constructors"

        Public Sub New()
            hdnLatitude = New HiddenField()
            hdnLongitude = New HiddenField()
            hdnLatitude.ID = "hdnLatitude"
            hdnLongitude.ID = "hdnLongitude"
        End Sub

#End Region

#Region "Public Methods"

        Public Sub LoadCoordinates(ByVal latitude As String, ByVal longitude As String)
            hdnLatitude.Value = latitude
            hdnLongitude.Value = longitude
        End Sub

        Public Function GetSelectedLatitude() As String
            Return hdnLatitude.Value
        End Function

        Public Function GetSelectedLongitude() As String
            Return hdnLongitude.Value
        End Function

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            EnsureChildControls()
        End Sub

        Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            writer.AddAttribute("class", "egv-map-marker-container")
            writer.AddAttribute("data-loaded", "false")
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            Dim googleApiKey As String = SettingController.ReadSetting("MAPSAPI")
            EGVScriptManager.AddScript("https://maps.googleapis.com/maps/api/js?key=" & googleApiKey)
            EGVScriptManager.AddScript(Path.MapCMSScript("lib/egvMapMarkerSpecifier"))
            EGVScriptManager.AddInlineScript(GetScript(), False, True)
        End Sub

        Protected Overrides Sub CreateChildControls()
            Controls.Add(hdnLatitude)
            Controls.Add(hdnLongitude)
            Dim map As New HtmlGenericControl("DIV")
            map.ID = ID & "-map"
            map.ClientIDMode = ClientIDMode.Static
            Controls.Add(map)
        End Sub

#End Region

#Region "Private Methods"

        Private Function GetScript() As String
            Dim sb As New StringBuilder()
            Dim defCoordinates As String = SettingController.ReadSetting("DEFCOORD")
            Dim parts() As String = Helper.SplitString(defCoordinates, ",")
            sb.AppendFormat("$('.egv-map-marker-container').MapMarkerSpecifier({{LatitudeControl: '#{0}', LongitudeControl: '#{1}', Map: '#{2}', DefLatitude: {3}, DefLongitude: {4}}});",
                            hdnLatitude.ClientID, hdnLongitude.ClientID, ID & "-map", parts(0), parts(1))
            Return sb.ToString()
        End Function

#End Region

    End Class

End Namespace
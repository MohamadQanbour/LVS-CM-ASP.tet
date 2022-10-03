Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports EGV.Utils

Namespace EGVControls
    <ToolboxData("<{0}:EGVTextBox runat=server></{0}:EGVTextBox>")>
    Public Class EGVTextBox
        Inherits TextBox

#Region "Public Properties"

        Public Property Placeholder As String
        Public Property ControlSize As EGV.Enums.ControlSizes = EGV.Enums.ControlSizes.Default
        Public Property IsBootstrapControl As Boolean = True
        Public Property DatePicker As Boolean = False
        Public Property InputNumber As Boolean = False
        Public Property InputDecimal As Boolean = False
        Public Property InputURL As Boolean = False

#End Region

#Region "Event Handlers"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            Dim classes As New List(Of String)
            If IsBootstrapControl Then
                classes.Add("form-control")
                Select Case ControlSize
                    Case EGV.Enums.ControlSizes.Large
                        classes.Add("input-lg")
                    Case EGV.Enums.ControlSizes.Small
                        classes.Add("input-sm")
                End Select
            End If
            If DatePicker Then
                classes.Add("datepicker")
                If Helper.Language.IsRTL Then classes.Add("datepicker-rtl")
            End If
            If classes.Count > 0 Then CssClass &= IIf(CssClass <> String.Empty, " ", "") & String.Join(" ", classes)
        End Sub

        Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If Placeholder <> String.Empty Then writer.AddAttribute("placeholder", Placeholder)
            If InputNumber Then
                writer.AddAttribute("data-inputmask", "'alias': 'integer'")
                writer.AddAttribute("data-mask", "")
            ElseIf InputDecimal Then
                writer.AddAttribute("data-inputmask", "'alias': 'decimal'")
                writer.AddAttribute("data-mask", "")
            ElseIf InputURL Then
                writer.AddAttribute("data-egvinput", "url")
            End If
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If Placeholder <> Nothing AndAlso Placeholder.StartsWith("Resources.") Then
                Placeholder = EGV.Utils.Localization.GetResource(Placeholder)
            End If
            If DatePicker Then
                EGV.Utils.EGVScriptManager.AddStyle(EGV.Utils.Path.MapCMSCss("datepicker3"))
                EGV.Utils.EGVScriptManager.AddScript(EGV.Utils.Path.MapCMSScript("bootstrap-datepicker"))
            End If
            If InputNumber OrElse InputDecimal Then
                EGV.Utils.EGVScriptManager.AddScripts(False,
                                                      EGV.Utils.Path.MapCMSScript("jquery.inputmask"),
                                                      EGV.Utils.Path.MapCMSScript("jquery.inputmask.extensions"),
                                                      EGV.Utils.Path.MapCMSScript("jquery.inputmask.numeric.extensions") & "?v=1.0"
                                                      )

            End If
        End Sub

#End Region

    End Class
End Namespace
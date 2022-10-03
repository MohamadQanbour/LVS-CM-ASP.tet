Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports EGV.Utils

Namespace EGVControls
    <ToolboxData("<{0}:EGVRequired runat=server></{0}:EGVRequired>")>
    Public Class EGVRequired
        Inherits RequiredFieldValidator

#Region "Public Properties"

        Public Property AddCustomClass As Boolean = True

#End Region

#Region "Overridden methods"

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If ErrorMessage <> Nothing AndAlso ErrorMessage.StartsWith("Resources.") Then ErrorMessage = EGV.Utils.Localization.GetResource(ErrorMessage)
        End Sub

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            Display = ValidatorDisplay.Dynamic
            If AddCustomClass Then CssClass &= IIf(CssClass <> String.Empty, " ", "") & "err in"
        End Sub

#End Region

    End Class
End Namespace
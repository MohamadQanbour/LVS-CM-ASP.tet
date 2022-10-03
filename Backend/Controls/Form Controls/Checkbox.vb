Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports EGV.Utils

Namespace EGVControls
    <ToolboxData("<{0}:EGVCheckBox runat=server></{0}:EGVCheckBox>")>
    Public Class EGVCheckBox
        Inherits CheckBox

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If Text <> Nothing AndAlso Text.StartsWith("Resources.") Then Text = EGV.Utils.Localization.GetResource(Text)
        End Sub

#End Region

    End Class
End Namespace
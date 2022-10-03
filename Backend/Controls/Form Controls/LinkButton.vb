Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports EGV.Utils

Namespace EGVControls
    Public Class EGVLinkButton
        Inherits LinkButton

#Region "public properties"

        Public Property BootstrapButton As Boolean = True
        Public Property BlockButton As Boolean = False
        Public Property FlatButton As Boolean = False
        Public Property AppButton As Boolean = False
        Public Property IsDisabled As Boolean = False
        Public Property Color As EGV.Enums.ControlColors
        Public Property Size As EGV.Enums.ButtonSizes

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            Dim classes As New List(Of String)
            If BootstrapButton Then
                classes.Add("btn")
                If BlockButton Then classes.Add("btn-block")
                If FlatButton Then classes.Add("btn-flat")
                If IsDisabled Then classes.Add("disabled")
                If AppButton Then classes.Add("btn-app")
                Select Case Size
                    Case EGV.Enums.ButtonSizes.Large
                        classes.Add("btn-lg")
                    Case EGV.Enums.ButtonSizes.Small
                        classes.Add("btn-sm")
                    Case EGV.Enums.ButtonSizes.XSmall
                        classes.Add("btn-xs")
                End Select
                Select Case Color
                    Case EGV.Enums.ControlColors.Danger
                        classes.Add("btn-danger")
                    Case EGV.Enums.ControlColors.Info
                        classes.Add("btn-info")
                    Case EGV.Enums.ControlColors.Maroon
                        classes.Add("bg-maroon")
                    Case EGV.Enums.ControlColors.Navy
                        classes.Add("bg-navy")
                    Case EGV.Enums.ControlColors.Olive
                        classes.Add("bg-olive")
                    Case EGV.Enums.ControlColors.Orange
                        classes.Add("bg-orange")
                    Case EGV.Enums.ControlColors.Primary
                        classes.Add("btn-primary")
                    Case EGV.Enums.ControlColors.Purple
                        classes.Add("bg-purple")
                    Case EGV.Enums.ControlColors.Warning
                        classes.Add("btn-warning")
                    Case EGV.Enums.ControlColors.Success
                        classes.Add("btn-success")
                    Case EGV.Enums.ControlColors.Default
                        classes.Add("btn-default")
                End Select
                If classes.Count > 0 Then CssClass &= IIf(CssClass <> String.Empty, " ", "") & String.Join(" ", classes.ToArray())
            End If
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If Text <> Nothing AndAlso Text.StartsWith("Resources.") Then Text = EGV.Utils.Localization.GetResource(Text)
        End Sub

#End Region

    End Class
End Namespace

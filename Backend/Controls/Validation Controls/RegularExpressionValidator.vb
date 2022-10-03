Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports EGV.Utils

Namespace EGVControls
    <ToolboxData("<{0}:EGVRegxValidator runat=server></{0}:EGVRegxValidator>")>
    Public Class EGVRegxValidator
        Inherits RegularExpressionValidator

#Region "Public Properties"

        Public Property AddCustomClass As Boolean = True
        Public Property ValidationType As EGV.Enums.ValidationTypes = EGV.Enums.ValidationTypes.None

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
            Select Case ValidationType
                Case EGV.Enums.ValidationTypes.Email
                    ValidationExpression = "\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                Case EGV.Enums.ValidationTypes.Website
                    ValidationExpression = "(([\w]+:)?\/\/)?(([\d\w]|%[a-fA-f\d]{2,2})+(:([\d\w]|%[a-fA-f\d]{2,2})+)?@)?([\d\w][-\d\w]{0,253}[\d\w]\.)+[\w]{2,4}(:[\d]+)?(\/([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)*(\?(&?([-+_~.\d\w]|%[a-fA-f\d]{2,2})=?)*)?(#([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)?$"
                Case EGV.Enums.ValidationTypes.Date
                    ValidationExpression = "^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|[1][0-12])[-/.](19|20)\d\d$"
            End Select
        End Sub

#End Region

    End Class
End Namespace
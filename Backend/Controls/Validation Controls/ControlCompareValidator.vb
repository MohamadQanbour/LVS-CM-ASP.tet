Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports EGV.Utils

Namespace EGVControls

    Public Class EGVControlCompareValidator
        Inherits EGVCustomValidator

#Region "Public Methods"

        Public Property ControlToCompare As String

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            ClientValidationFunction = "comparePasswords"
        End Sub

        Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            writer.AddAttribute("data-controltocompare", ControlToCompare)
        End Sub

#End Region

    End Class

End Namespace
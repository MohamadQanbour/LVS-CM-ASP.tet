Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports EGV.Utils
Imports EGV.Business

Namespace EGVControls

    Public Class AuditInformation
        Inherits WebControl
        Implements INamingContainer

#Region "Public Properties"

        Public Property BusinessObject As AudBusinessBase

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "egv-audit-info"
        End Sub

        Protected Overrides Sub CreateChildControls()
            If BusinessObject IsNot Nothing Then
                Dim createdDiv As New HtmlGenericControl("DIV")
                Controls.Add(createdDiv)
                createdDiv.InnerHtml = Localization.GetResource("Resources.Global.CMS.CreatedOn") & " <b>" & BusinessObject.CreatedDate.ToString("MMMM dd, yyyy @ hh:mm:ss") & "</b> " & Localization.GetResource("Resources.Global.CMS.By") & " <b>" & BusinessObject.CreatedUserName & "</b>"
                If BusinessObject.ModifiedUserName <> String.Empty Then
                    Dim modifiedDiv As New HtmlGenericControl("DIV")
                    Controls.Add(modifiedDiv)
                    modifiedDiv.InnerHtml = Localization.GetResource("Resources.Global.CMS.ModifiedOn") & " <b>" & BusinessObject.ModifiedDate.ToString("MMMM dd, yyyy @ hh:mm:ss") & "</b> " & Localization.GetResource("Resources.Global.CMS.By") & " <b>" & BusinessObject.ModifiedUserName & "</b>"
                End If
            End If
        End Sub

#End Region

    End Class

End Namespace
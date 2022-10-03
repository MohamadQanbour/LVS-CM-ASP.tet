Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports EGV.Utils

Namespace EGVControls
    Public Class Notifier
        Inherits WebControl
        Implements EGV.Interfaces.INotifier

#Region "Properties"

        Private Property Message As String
        Private Property Type As EGV.Enums.NotifyTypes
        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
            Hide()
        End Sub

#End Region

#Region "Public Methods"

        Public Sub Clear()
            Message = String.Empty
            Hide()
        End Sub

        Public Sub Warning(ByVal msg As String) Implements EGV.Interfaces.INotifier.Warning
            If msg <> String.Empty Then
                Type = EGV.Enums.NotifyTypes.Warning
                Message = msg
                Show()
            Else
                Clear()
            End If
        End Sub

        Public Sub Success(ByVal msg As String) Implements EGV.Interfaces.INotifier.Success
            If msg <> String.Empty Then
                Type = EGV.Enums.NotifyTypes.Success
                Message = msg
                Show()
            Else
                Clear()
            End If
        End Sub

        Public Sub Danger(ByVal msg As String) Implements EGV.Interfaces.INotifier.Danger
            If msg <> String.Empty Then
                Type = EGV.Enums.NotifyTypes.Danger
                Message = msg
                Show()
            Else
                Clear()
            End If
        End Sub

        Public Sub Notify(ByVal msg As String) Implements EGV.Interfaces.INotifier.Notify
            If msg <> String.Empty Then
                Type = EGV.Enums.NotifyTypes.Notify
                Message = msg
                Show()
            Else
                Clear()
            End If
        End Sub

        Public Function HasMessage() As Boolean Implements EGV.Interfaces.INotifier.HasMessage
            Return Message <> String.Empty
        End Function

#End Region

#Region "Private Methods"

        Private Sub Show()
            Visible = True
        End Sub

        Private Sub Hide()
            Visible = False
        End Sub

        Private Function GetIconClass() As String
            Select Case Type
                Case EGV.Enums.NotifyTypes.Success
                    Return "icon fa fa-check"
                Case EGV.Enums.NotifyTypes.Danger
                    Return "icon fa fa-ban"
                Case EGV.Enums.NotifyTypes.Notify
                    Return "icon fa fa-info"
                Case Else
                    Return "icon fa fa-warning"
            End Select
        End Function

        Private Function GetTitle() As String
            Select Case Type
                Case EGV.Enums.NotifyTypes.Success
                    Return "Success"
                Case EGV.Enums.NotifyTypes.Danger
                    Return "Alert"
                Case EGV.Enums.NotifyTypes.Notify
                    Return "Information"
                Case Else
                    Return "Warning"
            End Select
        End Function

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
        End Sub

        Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            Dim lst As New List(Of String)
            lst.Add("alert")
            lst.Add("alert-dismissible")
            Select Case Type
                Case EGV.Enums.NotifyTypes.Danger
                    lst.Add("alert-danger")
                Case EGV.Enums.NotifyTypes.Notify
                    lst.Add("alert-info")
                Case EGV.Enums.NotifyTypes.Success
                    lst.Add("alert-success")
                Case EGV.Enums.NotifyTypes.Warning
                    lst.Add("alert-warning")
            End Select
            writer.AddAttribute(HtmlTextWriterAttribute.Class, String.Join(" ", lst.ToArray()))
        End Sub

        Protected Overrides Sub CreateChildControls()
            'close button
            Dim btn As New HtmlGenericControl("button")
            btn.Attributes.Add("type", "button")
            btn.Attributes.Add("class", "close")
            btn.Attributes.Add("data-dismiss", "alert")
            btn.Attributes.Add("aria-hidden", "true")
            btn.Attributes.Add("onclick", "return false;")
            btn.InnerHtml = "&times;"
            Controls.Add(btn)
            'header
            Dim h4 As New HtmlGenericControl("h4")
            h4.InnerHtml = "<i class=""" & GetIconClass() & """></i> " & GetTitle() & "!"
            Controls.Add(h4)
            'body
            Dim p As New HtmlGenericControl("p")
            p.InnerHtml = Message
            Controls.Add(p)
        End Sub

#End Region

    End Class
End Namespace
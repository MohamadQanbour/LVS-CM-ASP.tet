Imports System.Web.UI.WebControls

Namespace EGV
    Namespace Utils
        Public Class EGVScriptManager

#Region "Private Methods"

            Private Shared Function GetStylesContainer() As Literal
                Return Helper.FindControl(Helper.Page.Header, "litAddStyle")
            End Function

            Private Shared Function GetScriptContainer(Optional ByVal headerScript As Boolean = False) As Literal
                If headerScript Then Return Helper.FindControl(Helper.Page.Header, "litAddHeaderScript") Else Return Helper.FindControl(Helper.Page, "litAddScript")
            End Function

            Private Shared Function GetInlineScriptContainer(Optional ByVal headerScript As Boolean = False) As Literal
                If headerScript Then Return Helper.FindControl(Helper.Page.Header, "litAddHeaderInlineScript") Else Return Helper.FindControl(Helper.Page, "litAddInlineScript")
            End Function

#End Region

#Region "Styles"

            Private Shared Function StyleExists(ByVal style As String, ByVal container As Literal) As Boolean
                Return container.Text.Contains(style)
            End Function

            Public Shared Sub AddStyle(ByVal style As String, Optional ByVal version As String = "")
                Dim v As String = version
                If v = String.Empty Then v = Helper.StylesVersion()
                Dim container As Literal = GetStylesContainer()
                If container IsNot Nothing Then
                    If Not StyleExists(style, container) Then
                        container.Text &= "<link rel=""stylesheet"" href=""" & style & IIf(v <> String.Empty, "?v=" & v, "") & """ />"
                    End If
                Else
                    Throw New Exception("Cannot find styles container.")
                End If
            End Sub

            Public Shared Sub AddStyles(ByVal ParamArray styles() As String)
                Dim v As String = Helper.StylesVersion()
                Dim container As Literal = GetStylesContainer()
                If container IsNot Nothing Then
                    For Each item As String In styles
                        If Not StyleExists(item, container) Then
                            container.Text &= "<link rel=""stylesheet"" href=""" & item & IIf(v <> String.Empty, "?v=" & v, "") & """ />"
                        End If
                    Next
                Else
                    Throw New Exception("Cannot find styles container.")
                End If
            End Sub

#End Region

#Region "Scripts"

            Public Shared Function ScriptExists(ByVal script As String, ByVal container As Literal) As Boolean
                Return container.Text.Contains(script)
            End Function

            Public Shared Sub AddScript(ByVal script As String, Optional ByVal headerScript As Boolean = False, Optional ByVal version As String = "")
                Dim v As String = version
                If v = String.Empty Then v = Helper.ScriptsVersion()
                Dim container As Literal = GetScriptContainer(headerScript)
                If container IsNot Nothing Then
                    If Not ScriptExists(script, container) Then
                        container.Text &= "<script type=""text/javascript"" src=""" & script & IIf(v <> String.Empty, "?v=" & v, "") & """></script>"
                    End If
                Else
                    Throw New Exception("Cannot find scripts container.")
                End If
            End Sub

            Public Shared Sub AddScripts(ByVal headerScripts As Boolean, ByVal ParamArray scripts() As String)
                Dim v As String = Helper.ScriptsVersion()
                Dim container As Literal = GetScriptContainer(headerScripts)
                If container IsNot Nothing Then
                    For Each item As String In scripts
                        If Not ScriptExists(item, container) Then
                            container.Text &= "<script type=""text/javascript"" src=""" & item & IIf(v <> String.Empty, "?v=" & v, "") & """></script>"
                        End If
                    Next
                Else
                    Throw New Exception("Cannot find scripts container.")
                End If
            End Sub

#End Region

#Region "Inline Scripts"

            Public Shared Function InlineScriptExists(ByVal script As String, ByVal container As Literal) As Boolean
                Return container.Text.Contains(script)
            End Function

            Public Shared Sub RemoveInlineScript(ByVal script As String, Optional ByVal headerScript As Boolean = False, Optional ByVal addOnLoad As Boolean = True)
                Dim scr As String = "<script type=""text/javascript"">" & IIf(addOnLoad, "$(document).ready(function(){", "") & script & IIf(addOnLoad, "});", "") & "</script>"
                Dim container As Literal = GetInlineScriptContainer(headerScript)
                If container IsNot Nothing Then
                    container.Text = container.Text.Replace(scr, "")
                Else
                    Throw New Exception("Cannot find inline scripts container.")
                End If
            End Sub

            Public Shared Sub ClearInlineScript(Optional ByVal headerScript As Boolean = False)
                Dim container As Literal = GetInlineScriptContainer(headerScript)
                container.Text = ""
            End Sub

            Public Shared Sub AddInlineScript(ByVal script As String, Optional ByVal headerScript As Boolean = False, Optional ByVal addOnLoad As Boolean = True)
                Dim scr As String = "<script type=""text/javascript"">" & IIf(addOnLoad, "$(document).ready(function(){", "") & script & IIf(addOnLoad, "});", "") & "</script>"
                Dim container As Literal = GetInlineScriptContainer(headerScript)
                If container IsNot Nothing Then
                    If Not InlineScriptExists(script, container) Then container.Text &= scr
                Else
                    Throw New Exception("Cannot find inline scripts container.")
                End If
            End Sub

#End Region

        End Class
    End Namespace
End Namespace
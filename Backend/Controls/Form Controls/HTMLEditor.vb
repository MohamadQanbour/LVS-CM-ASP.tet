Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports InnovaStudio
Imports EGV.Utils
Imports System.Xml

Namespace EGVControls

    Public Class HTMLEditor
        Inherits WYSIWYGEditor
        Implements INamingContainer

#Region "Public Properties"

        Public Property FrontPageCss As String
        Public Property ReturnFullPath As Boolean = False

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            'add scripts
            EGVScriptManager.AddScript(Path.MapCMSScript("scripts/innovaeditor.js"), True)
            EGVScriptManager.AddScript(Path.MapCMSScript("lib/egvEditor"))
            'set properties
            ApplySettings()
            Width = Unit.Percentage(100)
            Height = Unit.Pixel(350)
            EncodeIO = True
            InternalImageWidth = "16"
            InternalImageHeight = "16"
            InternalLink = Path.MapCMSFile("iframe-links.aspx")
            InternalLinkHeight = "400"
            InternalLinkWidth = "800"
            scriptPath = Path.MapCMSFile("js/scripts")
            onFullScreen = "egvDoFullScreen()"
            onNormalScreen = "egvDoNormalScreen()"
            AssetManager = Path.MapCMSFile("iframe-assetsmanager.aspx" & IIf(ReturnFullPath, "?fullpath=true", ""))
            AssetManagerHeight = "600"
            AssetManagerWidth = "1024"
            UseBR = False
            UseDIV = True
            If FrontPageCss <> String.Empty Then
                Css = Path.MapPortalStyle(FrontPageCss)
                btnStyles = True
            Else
                btnStyles = False
            End If
            'toolbar tabs
            Dim homeTab As New ISTab() With {
                .Id = "tabHome",
                .Text = Localization.GetResource("Resources.Global.CMS.Home")
                }
            homeTab.Groups.AddRange({
                New ISGroup() With {.Id = "grpEdit", .Buttons = New String() {"XHTMLSource", "FullScreen", "Search", "Preview", "BRK", "Undo", "Redo", "Cut", "Copy", "Paste", "PasteWord", "PasteText", "RemoveFormat"}},
                New ISGroup() With {.Id = "grpFont", .Buttons = New String() {"Strikethrough", "Superscript", "|", "ForeColor", "BackColor", "BRK", "Bold", "Italic", "Underline"}},
                New ISGroup() With {.Id = "grpPara", .Buttons = New String() {"Paragraph", "Indent", "Outdent", "LTR", "RTL", "|", "Styles", "StyleAndFormatting", "BRK", "JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyFull", "Numbering", "Bullets"}}
                                    })
            ToolbarTabs.Add(homeTab)
            Dim insertTab As New ISTab() With {
                .Id = "tabInsert",
                .Text = Localization.GetResource("Resources.Global.CMS.Insert")
                }
            insertTab.Groups.AddRange({
                New ISGroup() With {.Id = "grpInsert", .Buttons = New String() {"Hyperlink", "Bookmark", "BRK", "Image", "InternalLink"}},
                New ISGroup() With {.Id = "grpTable", .Buttons = New String() {"Table", "BRK", "Guidelines", "AutoTable"}},
                New ISGroup() With {.Id = "grpMedia", .Buttons = New String() {"Media", "YoutubeVideo", "BRK", "Characters", "Line"}}
                                      })
            ToolbarTabs.Add(insertTab)
        End Sub

#End Region

#Region "Private Methods"

        Private Sub ApplySettings()
            Dim doc As New XmlDocument()
            Dim loaded As Boolean = True
            Try
                doc.Load(Helper.Server.MapPath("~" & Path.MapPortalFile("css/HTMLEditorSettings.xml")))
            Catch ex As Exception
                loaded = False
            End Try
            If loaded Then
                'front page css
                Dim cssText As String = Helper.GetSafeXML(doc.SelectSingleNode("Settings"), "FrontPageCSS")
                If cssText <> String.Empty Then FrontPageCss = cssText
                If Helper.Language.IsRTL Then
                    Dim rtlCssText As String = Helper.GetSafeXML(doc.SelectSingleNode("Settings"), "RTLFrontPageCss")
                    If rtlCssText <> String.Empty Then FrontPageCss = rtlCssText
                End If
                'custom colors
                Dim colors As New List(Of String)
                For Each c As XmlNode In doc.SelectNodes("Settings/CustomColors/Color")
                    Dim cText As String = Helper.GetSafeXML(c, "Code")
                    If cText <> String.Empty Then colors.Add(cText)
                Next
                CustomColors = colors.ToArray()
            End If
        End Sub

#End Region

    End Class

End Namespace
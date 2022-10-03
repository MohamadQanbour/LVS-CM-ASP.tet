Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports EGV.Utils
Imports EGV.Business
Imports System.Text

Namespace EGVControls

    Public Class AssetsManager
        Inherits WebControl
        Implements INamingContainer

#Region "Public Properties"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        Public Property IsCMSPath As Boolean = False
        Public Property ShowModal As Boolean = True
        Public Property ReturnFullURL As Boolean = False

#End Region

#Region "Private Properties"

        Private ReadOnly Property ImageResolution As Integer
            Get
                Return SettingController.ReadSetting("imageresolution")
            End Get
        End Property

        Private ReadOnly Property MaxWidth As Integer
            Get
                Select Case ImageResolution
                    Case 1
                        Return 800
                    Case 2
                        Return 1024
                    Case 3
                        Return 1200
                    Case 4
                        Return 1920
                    Case Else
                        Return 0
                End Select
            End Get
        End Property

        Private ReadOnly Property MaxHeight As String
            Get
                Select Case ImageResolution
                    Case 1
                        Return 600
                    Case 2
                        Return 768
                    Case 3
                        Return 900
                    Case 4
                        Return 1440
                    Case Else
                        Return 0
                End Select
            End Get
        End Property

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            If ShowModal Then CssClass &= IIf(CssClass <> String.Empty, " ", "") & "modal fade"
            EnsureChildControls()
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            EGVScriptManager.AddStyles(
                Path.MapCMSCss("assets-manager"),
                Path.MapCMSCss("assets-manager." & Helper.GetHTMLDirection()),
                Path.MapCMSCss("jquery.fileupload"),
                Path.MapCMSCss("jquery.fileupload-ui")
            )
            EGVScriptManager.AddScripts(False,
                Path.MapCMSScript("lib/fileupload/jquery.ui.widget"),
                Path.MapCMSScript("lib/fileupload/tmpl.min"),
                Path.MapCMSScript("lib/fileupload/load-image.all.min"),
                Path.MapCMSScript("lib/fileupload/canvas-to-blob.min"),
                Path.MapCMSScript("lib/fileupload/jquery.iframe-transport"),
                Path.MapCMSScript("lib/fileupload/jquery.fileupload"),
                Path.MapCMSScript("lib/fileupload/jquery.fileupload-process"),
                Path.MapCMSScript("lib/fileupload/jquery.fileupload-image"),
                Path.MapCMSScript("lib/fileupload/jquery.fileupload-validate"),
                Path.MapCMSScript("lib/fileupload/jquery.fileupload-ui"),
                Path.MapCMSScript("lib/egvAssetsManager")
            )
        End Sub

        Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
            MyBase.AddAttributesToRender(writer)
            If ShowModal Then
                writer.AddAttribute("role", "dialog")
                writer.AddAttribute("tabindex", "-1")
            End If
            writer.AddAttribute("egvcommand", "assetsmanager")
        End Sub

        Protected Overrides Sub CreateChildControls()
            'hiddne fields
            Dim hdnAMP As New HiddenField()
            hdnAMP.ID = "hdnAMP"
            hdnAMP.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnAMP)
            hdnAMP.Value = "/ajax/AssetsManager/"
            Dim hdnAF As New HiddenField()
            hdnAF.ID = "hdnAF"
            hdnAF.ClientIDMode = ClientIDMode.Static
            hdnAF.Value = Helper.AssetsPath()
            Controls.Add(hdnAF)
            Dim hdnCP As New HiddenField()
            hdnCP.ID = "hdnCP"
            hdnCP.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnCP)
            hdnCP.Value = Helper.CMSPath()
            Dim hdnLoadingImage As New HiddenField()
            hdnLoadingImage.ID = "hdnLoadingImage"
            hdnLoadingImage.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnLoadingImage)
            hdnLoadingImage.Value = Path.MapCMSImage("loading.gif")
            Dim hdnResolution As New HiddenField()
            hdnResolution.ID = "hdnResolution"
            hdnResolution.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnResolution)
            hdnResolution.Value = ImageResolution
            Dim hdnMaxWidth As New HiddenField()
            hdnMaxWidth.ID = "hdnMaxWidth"
            hdnMaxWidth.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnMaxWidth)
            hdnMaxWidth.Value = MaxWidth
            Dim hdnMaxHeight As New HiddenField()
            hdnMaxHeight.ID = "hdnMaxHeight"
            hdnMaxHeight.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnMaxHeight)
            hdnMaxHeight.Value = MaxHeight
            Dim hdnIsCMS As New HiddenField()
            hdnIsCMS.ID = "hdnIsCMS"
            hdnIsCMS.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnIsCMS)
            hdnIsCMS.Value = IsCMSPath.ToString().ToLower()
            Dim hdnShowModal As New HiddenField()
            hdnShowModal.ID = "hdnShowModal"
            hdnShowModal.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnShowModal)
            hdnShowModal.Value = ShowModal.ToString().ToLower()
            Dim hdnReturnFullURL As New HiddenField()
            hdnReturnFullURL.ID = "hdnReturnFullURL"
            hdnReturnFullURL.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnReturnFullURL)
            hdnReturnFullURL.Value = ReturnFullURL.ToString().ToLower()
            Dim hdnRequestPath As New HiddenField()
            hdnRequestPath.ID = "hdnRequestPath"
            hdnRequestPath.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnRequestPath)
            hdnRequestPath.Value = "http://" & Helper.Request.Url.Host & IIf(Helper.Request.Url.Port <> 80, ":" & Helper.Request.Url.Port, "")
            'rendering
            Dim body As New HtmlGenericControl("DIV")
            Dim footer As New HtmlGenericControl("DIV")
            If ShowModal Then
                Dim dialog As New HtmlGenericControl("DIV")
                dialog.Attributes.Add("class", "modal-dialog modal-lg")
                Controls.Add(dialog)
                Dim content As New HtmlGenericControl("DIV")
                content.Attributes.Add("class", "modal-content")
                dialog.Controls.Add(content)
                'header
                Dim header As New HtmlGenericControl("DIV")
                header.Attributes.Add("class", "modal-header")
                content.Controls.Add(header)
                Dim closeBtn As New HtmlGenericControl("BUTTON")
                closeBtn.Attributes.Add("type", "button")
                closeBtn.Attributes.Add("class", "close")
                closeBtn.Attributes.Add("data-dismiss", "modal")
                closeBtn.Attributes.Add("aria-label", "Close")
                header.Controls.Add(closeBtn)
                Dim closeIcon As New HtmlGenericControl("SPAN")
                closeIcon.Attributes.Add("aria-hidden", "true")
                closeIcon.InnerHtml = "&times;"
                closeBtn.Controls.Add(closeIcon)
                Dim h4 As New HtmlGenericControl("H4")
                h4.Attributes.Add("class", "modal-title")
                header.Controls.Add(h4)
                h4.InnerText = Localization.GetResource("Resources.Global.AssetsManager.HeaderTitle")
                'body
                body.Attributes.Add("class", "modal-body egv-assets-modal-body")
                content.Controls.Add(body)
                'footer
                footer.Attributes.Add("class", "modal-footer")
                content.Controls.Add(footer)
            Else
                Controls.Add(body)
                Controls.Add(footer)
            End If
            Dim displayPanel As New HtmlGenericControl("DIV")
            displayPanel.Attributes.Add("class", "egv-assets-display-panel")
            body.Controls.Add(displayPanel)
            'toolbar
            Dim toolbar As New HtmlGenericControl("DIV")
            toolbar.Attributes.Add("class", "egv-assets-toolbar")
            displayPanel.Controls.Add(toolbar)
            'upload btn
            Dim btnUpload As New HtmlGenericControl("A")
            btnUpload.Attributes.Add("selectmode", "0")
            btnUpload.Attributes.Add("href", "javascript:;")
            btnUpload.Attributes.Add("egvcommand", "upload")
            toolbar.Controls.Add(btnUpload)
            btnUpload.InnerHtml = "<span class=""fa fa-cloud-upload""></span> " & Localization.GetResource("Resources.Global.AssetsManager.UploadFiles")
            'add folder
            Dim btnAddFolder As New HtmlGenericControl("A")
            btnAddFolder.Attributes.Add("selectmode", "0")
            btnAddFolder.Attributes.Add("href", "javascript:;")
            btnAddFolder.Attributes.Add("egvcommand", "newfolder")
            toolbar.Controls.Add(btnAddFolder)
            btnAddFolder.InnerHtml = "<span class=""fa fa-plus-circle""></span> " & Localization.GetResource("Resources.Global.AssetsManager.AddFolder")
            'separator
            Dim sep1 As New HtmlGenericControl("SPAN")
            sep1.Attributes.Add("class", "sep")
            toolbar.Controls.Add(sep1)
            'rename
            Dim btnRename As New HtmlGenericControl("A")
            btnRename.Attributes.Add("selectmode", "1")
            btnRename.Attributes.Add("href", "javascript:;")
            btnRename.Attributes.Add("egvcommand", "rename")
            toolbar.Controls.Add(btnRename)
            btnRename.InnerHtml = "<span class=""fa fa-edit""></span> " & Localization.GetResource("Resources.Global.AssetsManager.Rename")
            'delete
            Dim btnDelete As New HtmlGenericControl("A")
            btnDelete.Attributes.Add("selectmode", "2")
            btnDelete.Attributes.Add("href", "javascript:;")
            btnDelete.Attributes.Add("egvcommand", "delete")
            toolbar.Controls.Add(btnDelete)
            btnDelete.InnerHtml = "<span class=""fa fa-times""></span> " & Localization.GetResource("Resources.Global.AssetsManager.Delete")
            'reload
            Dim btnReload As New HtmlGenericControl("A")
            btnReload.Attributes.Add("selectmode", "0")
            btnReload.Attributes.Add("href", "javascript:;")
            btnReload.Attributes.Add("egvcommand", "refresh")
            toolbar.Controls.Add(btnReload)
            btnReload.InnerHtml = "<span class=""fa fa-refresh""></span> " & Localization.GetResource("Resources.Global.AssetsManager.Reload")
            'breadcrumb
            Dim breadcrumb As New HtmlGenericControl("DIV")
            breadcrumb.Attributes.Add("class", "egv-assets-breadcrumb")
            breadcrumb.Attributes.Add("egvcommand", "breadcrumb")
            displayPanel.Controls.Add(breadcrumb)
            'search panel
            Dim pnlSearch As New HtmlGenericControl("DIV")
            pnlSearch.Attributes.Add("class", "egv-assets-search-panel")
            displayPanel.Controls.Add(pnlSearch)
            Dim txtSearch As New HtmlGenericControl("INPUT")
            txtSearch.Attributes.Add("type", "text")
            txtSearch.Attributes.Add("egvcommand", "txtsearch")
            txtSearch.Attributes.Add("placeholder", Localization.GetResource("Resources.Global.AssetsManager.SearchPlaceholder"))
            pnlSearch.Controls.Add(txtSearch)
            'clear search
            Dim btnClearSearch As New HtmlGenericControl("A")
            btnClearSearch.Attributes.Add("href", "javascript:;")
            btnClearSearch.Attributes.Add("egvcommand", "clearsearch")
            pnlSearch.Controls.Add(btnClearSearch)
            btnClearSearch.InnerHtml = "<span class=""fa fa-times""></span>"
            'search
            Dim btnSearch As New HtmlGenericControl("A")
            btnSearch.Attributes.Add("href", "javascript:;")
            btnSearch.Attributes.Add("egvcommand", "search")
            pnlSearch.Controls.Add(btnSearch)
            btnSearch.InnerHtml = "<span class=""fa fa-search""></span>"
            'file list
            Dim pnlFileList As New HtmlGenericControl("DIV")
            pnlFileList.Attributes.Add("class", "egv-assets-files-list")
            pnlFileList.Attributes.Add("egvcommand", "filelist")
            displayPanel.Controls.Add(pnlFileList)
            Dim flSection As New HtmlGenericControl("SECTION")
            pnlFileList.Controls.Add(flSection)
            Dim table As New HtmlGenericControl("TABLE")
            flSection.Controls.Add(table)
            Dim trHeader As New HtmlGenericControl("TR")
            trHeader.Attributes.Add("class", "egv-assets-header")
            table.Controls.Add(trHeader)
            'col select
            Dim colSelect As New HtmlGenericControl("TH")
            colSelect.Attributes.Add("class", "egv-assets-th-select")
            trHeader.Controls.Add(colSelect)
            'col name
            Dim colName As New HtmlGenericControl("TH")
            colName.Attributes.Add("class", "egv-assets-th-name")
            trHeader.Controls.Add(colName)
            Dim colNameSort As New HtmlGenericControl("A")
            colNameSort.Attributes.Add("href", "javascript:;")
            colNameSort.Attributes.Add("egvcommand", "sort")
            colNameSort.Attributes.Add("egvargument", "name")
            colNameSort.InnerText = Localization.GetResource("Resources.Global.AssetsManager.Name")
            colName.Controls.Add(colNameSort)
            'col date
            Dim colDate As New HtmlGenericControl("TH")
            colDate.Attributes.Add("class", "egv-assets-th-date")
            trHeader.Controls.Add(colDate)
            Dim colDateSort As New HtmlGenericControl("A")
            colDateSort.Attributes.Add("href", "javascript:;")
            colDateSort.Attributes.Add("egvcommand", "sort")
            colDateSort.Attributes.Add("egvargument", "date")
            colDateSort.InnerText = Localization.GetResource("Resources.Global.AssetsManager.Date")
            colDate.Controls.Add(colDateSort)
            'col size
            Dim colSize As New HtmlGenericControl("TH")
            colSize.Attributes.Add("class", "egv-assets-th-size")
            trHeader.Controls.Add(colSize)
            Dim colSizeSort As New HtmlGenericControl("A")
            colSizeSort.Attributes.Add("href", "javascript:;")
            colSizeSort.Attributes.Add("egvcommand", "sort")
            colSizeSort.Attributes.Add("egvargument", "size")
            colSizeSort.InnerText = Localization.GetResource("Resources.Global.AssetsManager.Size")
            colSize.Controls.Add(colSizeSort)
            'preview
            Dim pnlPreview As New HtmlGenericControl("DIV")
            pnlPreview.Attributes.Add("class", "egv-assets-preview")
            pnlPreview.Attributes.Add("egvcommand", "preview")
            displayPanel.Controls.Add(pnlPreview)
            Dim btnClosePreview As New HtmlGenericControl("A")
            btnClosePreview.Attributes.Add("href", "javascript:;")
            btnClosePreview.Attributes.Add("egvcommand", "closepreview")
            btnClosePreview.InnerHtml = "<span class=""fa fa-times""></span>"
            pnlPreview.Controls.Add(btnClosePreview)
            Dim sectionPreview As New HtmlGenericControl("SECTION")
            pnlPreview.Controls.Add(sectionPreview)
            'pager
            Dim pnlPager As New HtmlGenericControl("DIV")
            pnlPager.Attributes.Add("class", "egv-assets-pager")
            pnlPager.Attributes.Add("egvcommand", "pager")
            displayPanel.Controls.Add(pnlPager)
            'content info
            Dim pnlContentInfo As New HtmlGenericControl("DIV")
            pnlContentInfo.Attributes.Add("class", "egv-assets-content-info")
            pnlContentInfo.Attributes.Add("egvcommand", "contentinfo")
            displayPanel.Controls.Add(pnlContentInfo)
            'file upload
            Dim pnlUpload As New HtmlGenericControl("DIV")
            pnlUpload.Attributes.Add("egvcommand", "uploadpanel")
            pnlUpload.Style.Add("display", "none")
            displayPanel.Controls.Add(pnlUpload)
            Dim hdnCurFilePath As New HtmlGenericControl("INPUT")
            hdnCurFilePath.Attributes.Add("type", "hidden")
            hdnCurFilePath.Attributes.Add("id", "hdnCurFilePath")
            hdnCurFilePath.Attributes.Add("name", "hdnCurFilePath")
            pnlUpload.Controls.Add(hdnCurFilePath)
            Dim rowUpload As New HtmlGenericControl("DIV")
            rowUpload.Attributes.Add("class", "row fileupload-buttonbar")
            pnlUpload.Controls.Add(rowUpload)
            Dim pnlLarge As New HtmlGenericControl("DIV")
            pnlLarge.Attributes.Add("class", "col-lg-7")
            rowUpload.Controls.Add(pnlLarge)
            Dim btnAddFiles As New HtmlGenericControl("SPAN")
            btnAddFiles.Attributes.Add("class", "btn btn-success fileinput-button")
            btnAddFiles.InnerHtml = "<i class=""fa fa-plus""></i><span>" & Localization.GetResource("Resources.Global.AssetsManager.AddFiles") & "</span><input type=""file"" name=""files[]"" multiple />"
            pnlLarge.Controls.Add(btnAddFiles)
            Dim btnStartUpload As New HtmlGenericControl("BUTTON")
            btnStartUpload.Attributes.Add("type", "submit")
            btnStartUpload.Attributes.Add("class", "btn btn-primary start")
            btnStartUpload.InnerHtml = "<i class=""fa fa-cloud-upload""></i><span>" & Localization.GetResource("Resources.Global.AssetsManager.StartUpload") & "</span>"
            pnlLarge.Controls.Add(btnStartUpload)
            Dim btnCancelUpload As New HtmlGenericControl("BUTTON")
            btnCancelUpload.Attributes.Add("type", "reset")
            btnCancelUpload.Attributes.Add("class", "btn btn-warning cancel")
            btnCancelUpload.InnerHtml = "<i class=""fa fa-ban""></i><span>" & Localization.GetResource("Resources.Global.AssetsManager.CancelUpload") & "</span>"
            pnlLarge.Controls.Add(btnCancelUpload)
            Dim spanUploadProcess As New HtmlGenericControl("SPAN")
            spanUploadProcess.Attributes.Add("class", "fileupload-process")
            pnlLarge.Controls.Add(spanUploadProcess)
            Dim pnlSmall As New HtmlGenericControl("DIV")
            pnlSmall.Attributes.Add("class", "col-lg-5 fileupload-progress fade")
            rowUpload.Controls.Add(pnlSmall)
            pnlSmall.InnerHtml = "<div class=""progress progress-striped active"" role=""progressbar"" aria-valuemin=""0"" aria-valuemax=""100""><div class=""progress-bar progress-bar-success"" style=""width:0%;""></div></div><div class=""progress-extended"">&nbsp;</div>"
            Dim pnlPresentation As New HtmlGenericControl("TABLE")
            pnlPresentation.Attributes.Add("role", "presentation")
            pnlPresentation.Attributes.Add("class", "table table-striped")
            pnlPresentation.InnerHtml = "<tbody class=""files""></tbody>"
            pnlUpload.Controls.Add(pnlPresentation)
            'footer
            Dim fInputGroup As New HtmlGenericControl("DIV")
            fInputGroup.Attributes.Add("class", "input-group")
            footer.Controls.Add(fInputGroup)
            Dim txtSelectedFile As New HtmlGenericControl("input")
            txtSelectedFile.Attributes.Add("type", "text")
            txtSelectedFile.Attributes.Add("class", "form-control")
            txtSelectedFile.Attributes.Add("readonly", "readonly")
            txtSelectedFile.Attributes.Add("egvcommand", "selectedfile")
            fInputGroup.Controls.Add(txtSelectedFile)
            Dim btnAddOn As New HtmlGenericControl("SPAN")
            btnAddOn.Attributes.Add("class", "input-group-btn")
            fInputGroup.Controls.Add(btnAddOn)
            Dim okBtn As New HtmlGenericControl("A")
            okBtn.Attributes.Add("class", "btn btn-info")
            okBtn.Attributes.Add("egvcommand", "ok")
            okBtn.InnerText = Localization.GetResource("Resources.Global.AssetsManager.OK")
            btnAddOn.Controls.Add(okBtn)
            Dim uploadScript As New HtmlGenericControl("SCRIPT")
            uploadScript.Attributes.Add("id", "template-upload")
            uploadScript.Attributes.Add("type", "text/x-tmpl")
            uploadScript.InnerHtml = GetUploadScript()
            Controls.Add(uploadScript)
            Dim downloadScript As New HtmlGenericControl("SCRIPT")
            downloadScript.Attributes.Add("id", "template-download")
            downloadScript.Attributes.Add("type", "text/x-tmpl")
            downloadScript.InnerHtml = GetDownloadScript()
            Controls.Add(downloadScript)
        End Sub

#End Region

#Region "Private Methods"

        Private Function GetUploadScript() As String
            Dim sb As New StringBuilder()
            sb.AppendLine("	{% for (var i=0, file; file=o.files[i]; i++) { %}")
            sb.AppendLine("		<tr class=""template-upload fade"">")
            sb.AppendLine("			<td>")
            sb.AppendLine("				<span class=""preview""></span>")
            sb.AppendLine("			</td>")
            sb.AppendLine("			<td>")
            sb.AppendLine("				<p class=""name"">{%=file.name%}</p>")
            sb.AppendLine("				<strong class=""error text-danger""></strong>")
            sb.AppendLine("			</td>")
            sb.AppendLine("			<td>")
            sb.AppendLine("				<p class=""size"">Processing...</p>")
            sb.AppendLine("				<div class=""progress progress-striped active"" role=""progressbar"" aria-valuemin=""0"" aria-valuemax=""100"" aria-valuenow=""0""><div class=""progress-bar progress-bar-success"" style=""width:0%;""></div></div>")
            sb.AppendLine("			</td>")
            sb.AppendLine("			<td>")
            sb.AppendLine("				{% if (!i && !o.options.autoUpload) { %}")
            sb.AppendLine("					<button class=""btn btn-primary start"" disabled>")
            sb.AppendLine("						<i class=""glyphicon glyphicon-upload""></i>")
            sb.AppendLine("						<span>" & Localization.GetResource("Resources.Global.AssetsManager.Start") & "</span>")
            sb.AppendLine("					</button>")
            sb.AppendLine("				{% } %}")
            sb.AppendLine("				{% if (!i) { %}")
            sb.AppendLine("					<button class=""btn btn-warning cancel"">")
            sb.AppendLine("						<i class=""glyphicon glyphicon-ban-circle""></i>")
            sb.AppendLine("						<span>" & Localization.GetResource("Resources.Global.AssetsManager.Cancel") & "</span>")
            sb.AppendLine("					</button>")
            sb.AppendLine("				{% } %}")
            sb.AppendLine("			</td>")
            sb.AppendLine("		</tr>")
            sb.AppendLine("	{% } %}")
            Return sb.ToString()
        End Function

        Private Function GetDownloadScript()
            Dim sb As New StringBuilder()
            sb.AppendLine("	{% for (var i=0, file; file=o.files[i]; i++) { %}")
            sb.AppendLine("		<tr class=""template-download fade"">")
            sb.AppendLine("			<td>")
            sb.AppendLine("				<span class=""preview"">")
            sb.AppendLine("					{% if (file.thumbnailUrl) { %}")
            sb.AppendLine("						<a href=""{%=file.url%}"" title=""{%=file.name%}"" download=""{%=file.name%}"" data-gallery><img src=""{%=file.thumbnailUrl%}""></a>")
            sb.AppendLine("					{% } %}")
            sb.AppendLine("				</span>")
            sb.AppendLine("			</td>")
            sb.AppendLine("			<td>")
            sb.AppendLine("				<p class=""name"">")
            sb.AppendLine("					{% if (file.url) { %}")
            sb.AppendLine("						<a href=""{%=file.url%}"" title=""{%=file.name%}"" download=""{%=file.name%}"" {%=file.thumbnailUrl?'data-gallery':''%}>{%=file.name%}</a>")
            sb.AppendLine("					{% } else { %}")
            sb.AppendLine("						<span>{%=file.name%}</span>")
            sb.AppendLine("					{% } %}")
            sb.AppendLine("				</p>")
            sb.AppendLine("				{% if (file.error) { %}")
            sb.AppendLine("					<div><span class=""label label-danger"">Error</span> {%=file.error%}</div>")
            sb.AppendLine("				{% } %}")
            sb.AppendLine("			</td>")
            sb.AppendLine("			<td>")
            sb.AppendLine("				<span class=""size"">{%=o.formatFileSize(file.size)%}</span>")
            sb.AppendLine("			</td>")
            sb.AppendLine("			<td>")
            sb.AppendLine("				{% if (file.deleteUrl) { %}")
            sb.AppendLine("					")
            sb.AppendLine("				{% } else { %}")
            sb.AppendLine("					<button class=""btn btn-warning cancel"">")
            sb.AppendLine("						<i class=""glyphicon glyphicon-ban-circle""></i>")
            sb.AppendLine("						<span>" & Localization.GetResource("Resources.Global.AssetsManager.Cancel") & "</span>")
            sb.AppendLine("					</button>")
            sb.AppendLine("				{% } %}")
            sb.AppendLine("			</td>")
            sb.AppendLine("		</tr>")
            sb.AppendLine("	{% } %}")
            Return sb.ToString()
        End Function

#End Region

    End Class

End Namespace
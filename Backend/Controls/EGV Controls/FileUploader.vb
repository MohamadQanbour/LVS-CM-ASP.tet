Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports EGV.Business
Imports EGV.Utils
Imports System.Text

Namespace EGVControls

    Public Class FileUploader
        Inherits WebControl
        Implements INamingContainer

#Region "Public Properties"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        Public Property IsCMSPath As Boolean = False
        Public Property UploadPath As String = String.Empty
        Public Property AllowDocumentsUpload As Boolean = False
        Public Property AllowMediaUpload As Boolean = False
        Public Property AllowFilesUpload As Boolean = False
        Public Property AllowMultiple As Boolean = True

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

#Region "Overridden methods"

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "egv-file-uploader-container"
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            EGVScriptManager.AddStyles(
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
                Path.MapCMSScript("lib/egvFileUploader")
            )
        End Sub

        Protected Overrides Sub CreateChildControls()
            'hidden Fields
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
            Dim hdnLimitNumOfFiles As New HiddenField()
            hdnLimitNumOfFiles.ID = "hdnLimit"
            hdnLimitNumOfFiles.ClientIDMode = ClientIDMode.Static
            hdnLimitNumOfFiles.Value = IIf(Not AllowMultiple, 1, 0)
            Controls.Add(hdnLimitNumOfFiles)
            'Rendering
            Dim pnlUpload As New HtmlGenericControl("DIV")
            pnlUpload.Attributes.Add("egvcommand", "fileuploader")
            Controls.Add(pnlUpload)
            Dim hdnCurFilePath As New HtmlGenericControl("INPUT")
            hdnCurFilePath.Attributes.Add("type", "hidden")
            hdnCurFilePath.Attributes.Add("id", "hdnCurFilePath")
            hdnCurFilePath.Attributes.Add("name", "hdnCurFilePath")
            pnlUpload.Controls.Add(hdnCurFilePath)
            hdnCurFilePath.Attributes.Add("value", UploadPath)
            Dim rowUpload As New HtmlGenericControl("DIV")
            rowUpload.Attributes.Add("class", "row fileupload-buttonbar")
            pnlUpload.Controls.Add(rowUpload)
            Dim pnlLarge As New HtmlGenericControl("DIV")
            pnlLarge.Attributes.Add("class", "col-lg-7")
            rowUpload.Controls.Add(pnlLarge)
            Dim btnAddFiles As New HtmlGenericControl("SPAN")
            btnAddFiles.Attributes.Add("class", "btn btn-success fileinput-button")
            btnAddFiles.InnerHtml = "<i class=""fa fa-plus""></i><span>" & Localization.GetResource("Resources.Global.AssetsManager.AddFiles") & "</span><input type=""file"" name=""files[]"" " & IIf(AllowMultiple, "multiple", "") & " accept=""" & GetAllowedTypes() & """ />"
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
            'scripts
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

        Private Function GetAllowedTypes() As String
            Dim lst As New List(Of String)
            lst.Add("image/*")
            If AllowDocumentsUpload Then lst.Add(".pdf,.xlsx,.xls,.docx,.doc,.ppsx,.ppt,.pptx")
            If AllowMediaUpload Then lst.Add("audio/*,video/*")
            If AllowFilesUpload Then lst.Add(".zip,.rar,.gzip")
            Return String.Join(",", lst.ToArray())
        End Function

#End Region

    End Class

End Namespace
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Business

Namespace EGVControls

    <ToolboxData("<{0}:ImageEditor runat=server></{0}:ImageEditor>")>
    Public Class ImageEditor
        Inherits WebControl
        Implements INamingContainer

#Region "Public Methods"

        Protected Overrides ReadOnly Property TagKey As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        Public Property TargetImage As String
        Public Property MaxWidth As String
        Public Property MaxHeight As String
        Public Property CropType As CroppingTypes
        Public Property CropX As String
        Public Property CropY As String
        Public Property Scale As String
        Public Property IsCMS As Boolean = False

#End Region

#Region "Private Properties"

        Private Property hdnImage As HiddenField
        Private Property hdnMaxWidth As HiddenField
        Private Property hdnMaxHeight As HiddenField
        Private Property hdnCropType As HiddenField
        Private Property hdnCropX As HiddenField
        Private Property hdnCropY As HiddenField
        Private Property hdnScale As HiddenField
        Private Property ctrlImage As Image
        Private Property ddl As EGVDropDown

#End Region

#Region "Constuctors"

        Public Sub New()
            CropType = CroppingTypes.Manual
            hdnImage = New HiddenField()
            hdnMaxWidth = New HiddenField()
            hdnMaxHeight = New HiddenField()
            hdnCropType = New HiddenField()
            hdnCropX = New HiddenField()
            hdnCropY = New HiddenField()
            hdnScale = New HiddenField()
            ctrlImage = New Image()
            ddl = New EGVDropDown()
        End Sub

#End Region

#Region "Overridden Methods"

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            EGVScriptManager.AddStyle(Path.MapCMSCss("jquery-ui.min"))
            EGVScriptManager.AddScript(Path.MapCMSScript("jquery-ui.min"))
            EGVScriptManager.AddScript(Path.MapCMSScript("lib/egvImageEditor"))
        End Sub

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
            Helper.LoadLanguage()
            CssClass &= IIf(CssClass <> String.Empty, " ", "") & "egv-image-editor"
        End Sub

        Protected Overrides Sub CreateChildControls()
            'hidden fields
            hdnImage.ID = "hdnImageEditorImage"
            hdnImage.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnImage)
            hdnMaxWidth.ID = "hdnImageEditorMaxWidth"
            hdnMaxWidth.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnMaxWidth)
            hdnMaxHeight.ID = "hdnImageEditorMaxHeight"
            hdnMaxHeight.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnMaxHeight)
            hdnCropType.ID = "hdnImageEditorCropType"
            hdnCropType.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnCropType)
            hdnCropX.ID = "hdnImageEditorCropX"
            hdnCropX.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnCropX)
            hdnCropY.ID = "hdnImageEditorCropY"
            hdnCropY.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnCropY)
            hdnScale.ID = "hdnImageEditorScale"
            hdnScale.ClientIDMode = ClientIDMode.Static
            Controls.Add(hdnScale)
            'rendering
            Dim row As New HtmlGenericControl("DIV")
            row.Attributes.Add("class", "row")
            Controls.Add(row)
            Dim container As New HtmlGenericControl("DIV")
            container.Attributes.Add("class", "col-xs-12 col-sm-8 col-md-6 col-sm-offset-2 col-md-offset-3")
            row.Controls.Add(container)
            'cropping type
            Dim inlineForm As New HtmlGenericControl("DIV")
            inlineForm.Attributes.Add("class", "form-horizontal egv-crop-type-selector")
            container.Controls.Add(inlineForm)
            Dim formGroup As New HtmlGenericControl("DIV")
            formGroup.Attributes.Add("class", "form-group")
            inlineForm.Controls.Add(formGroup)
            Dim lbl As New HtmlGenericControl("SPAN")
            lbl.Attributes.Add("class", "col-sm-4 control-label")
            lbl.InnerText = Localization.GetResource("Resources.Global.CMS.CropType")
            formGroup.Controls.Add(lbl)
            Dim selectContainer As New HtmlGenericControl("DIV")
            selectContainer.Attributes.Add("class", "col-sm-8")
            formGroup.Controls.Add(selectContainer)
            ddl.ID = "ddlCropType"
            ddl.ClientIDMode = ClientIDMode.Static
            ddl.Size = ControlSizes.Small
            ddl.BindToEnum(GetType(CroppingTypes), False)
            ddl.Attributes.Add("egvcommand", "croptype")
            selectContainer.Controls.Add(ddl)
            'into
            Dim intro As New HtmlGenericControl("DIV")
            intro.Attributes.Add("class", "egv-intro")
            intro.InnerHtml = "<p>" & Localization.GetResource("Resources.Global.CMS.ImageEditorIntro") & "</p>"
            container.Controls.Add(intro)
            'toolbar
            Dim toolbar As New HtmlGenericControl("DIV")
            toolbar.Attributes.Add("class", "btn-toolbar text-right")
            container.Controls.Add(toolbar)
            'zoom button
            Dim zoomGroup As New HtmlGenericControl("DIV")
            zoomGroup.Attributes.Add("class", "btn-group")
            zoomGroup.Attributes.Add("role", "group")
            toolbar.Controls.Add(zoomGroup)
            Dim zoomin As New HtmlGenericControl("A")
            zoomin.Attributes.Add("href", "javascript:;")
            zoomin.Attributes.Add("class", "btn btn-default btn-sm")
            zoomin.Attributes.Add("egvcommand", "zoomin")
            zoomin.InnerHtml = "<span class=""fa fa-plus""></span>"
            zoomGroup.Controls.Add(zoomin)
            Dim zoomout As New HtmlGenericControl("A")
            zoomout.Attributes.Add("href", "javascript:;")
            zoomout.Attributes.Add("class", "btn btn-default btn-sm")
            zoomout.Attributes.Add("egvcommand", "zoomout")
            zoomout.InnerHtml = "<span class=""fa fa-minus""></span>"
            zoomGroup.Controls.Add(zoomout)
            'move buttons
            Dim moveGroup As New HtmlGenericControl("DIV")
            moveGroup.Attributes.Add("class", "btn-group")
            moveGroup.Attributes.Add("role", "group")
            toolbar.Controls.Add(moveGroup)
            Dim moveLeft As New HtmlGenericControl("A")
            moveLeft.Attributes.Add("href", "javascript:;")
            moveLeft.Attributes.Add("class", "btn btn-default btn-sm")
            moveLeft.Attributes.Add("egvcommand", "moveleft")
            moveLeft.InnerHtml = String.Format("<span class=""fa fa-arrow-{0}""></span>", IIf(Helper.Language.IsRTL, "right", "left"))
            moveGroup.Controls.Add(moveLeft)
            Dim moveRight As New HtmlGenericControl("A")
            moveRight.Attributes.Add("href", "javascript:;")
            moveRight.Attributes.Add("class", "btn btn-default btn-sm")
            moveRight.Attributes.Add("egvcommand", "moveright")
            moveRight.InnerHtml = String.Format("<span class=""fa fa-arrow-{0}""></span>", IIf(Helper.Language.IsRTL, "left", "right"))
            moveGroup.Controls.Add(moveRight)
            Dim moveUp As New HtmlGenericControl("A")
            moveUp.Attributes.Add("href", "javascript:;")
            moveUp.Attributes.Add("class", "btn btn-default btn-sm")
            moveUp.Attributes.Add("egvcommand", "moveup")
            moveUp.InnerHtml = "<span class=""fa fa-arrow-up""></span>"
            moveGroup.Controls.Add(moveUp)
            Dim moveDown As New HtmlGenericControl("A")
            moveDown.Attributes.Add("href", "javascript:;")
            moveDown.Attributes.Add("class", "btn btn-default btn-sm")
            moveDown.Attributes.Add("egvcommand", "movedown")
            moveDown.InnerHtml = "<span class=""fa fa-arrow-down""></span>"
            moveGroup.Controls.Add(moveDown)
            'image viewer
            Dim viewer As New HtmlGenericControl("DIV")
            viewer.Attributes.Add("class", "egv-viewer")
            container.Controls.Add(viewer)
            Dim wrapper As New HtmlGenericControl("DIV")
            wrapper.Attributes.Add("class", "egv-image-wrapper")
            viewer.Controls.Add(wrapper)
            wrapper.Controls.Add(ctrlImage)
        End Sub

#End Region

#Region "Private Methods"

        Private Sub ProcessImage()
            If TargetImage.StartsWith("/files/") Then
                Dim parts() = Helper.SplitString(TargetImage, "/")
                For i As Integer = 0 To parts.Length - 1
                    If parts(i) = "width" Then MaxWidth = parts(i + 1)
                    If parts(i) = "height" Then MaxHeight = parts(i + 1)
                    If parts(i) = "scale" Then Scale = parts(i + 1)
                    If parts(i) = "crop" Then
                        Dim cropParts() = Helper.SplitString(parts(i + 1), "x")
                        If cropParts.Length >= 3 Then
                            CropType = cropParts(2)
                        End If
                        If cropParts.Length = 4 Then
                            Dim coord() = Helper.SplitString(cropParts(3), ",")
                            CropX = coord(0)
                            CropY = coord(1)
                        End If
                    End If
                Next
                TargetImage = Helper.DeformImageUrl(TargetImage)
            End If
        End Sub

        Private Sub LoadHiddenFields()
            hdnCropX.Value = CropX
            hdnCropY.Value = CropY
            hdnImage.Value = TargetImage
            hdnCropType.Value = CropType
            hdnMaxHeight.Value = MaxHeight
            hdnMaxWidth.Value = MaxWidth
            hdnScale.Value = Scale
            ctrlImage.ImageUrl = TargetImage
            ddl.SelectedValue = CropType
        End Sub

#End Region

#Region "Public Methods"

        Public Sub LoadImage(ByVal image As String)
            TargetImage = image
            ProcessImage()
            LoadHiddenFields()
        End Sub

        Public Function GetImage() As String
            TargetImage = hdnImage.Value
            If TargetImage <> String.Empty Then
                If TargetImage.StartsWith("/files/") Then TargetImage = Helper.DeformImageUrl(TargetImage)
                Dim x As Integer = 0
                Dim y As Integer = 0
                Dim scale As String = "1"
                If ddl.SelectedValue = CroppingTypes.Manual Then
                    x = Helper.GetSafeObject(hdnCropX.Value)
                    y = Helper.GetSafeObject(hdnCropY.Value)
                    scale = Helper.GetSafeObject(hdnScale.Value, ValueTypes.TypeDecimal)
                    If scale = String.Empty Then scale = "1"
                End If
                Return Helper.FormImageUrl(
                    TargetImage,
                    hdnMaxWidth.Value,
                    hdnMaxHeight.Value,
                    hdnMaxWidth.Value,
                    hdnMaxHeight.Value,
                    ddl.SelectedValue,
                    x, y, scale,
                    IIf(IsCMS, "cms", "portal")
                )
            Else
                Return String.Empty
            End If
        End Function

        Public Function GetUnprocessedImage() As String
            Return hdnImage.Value
        End Function

        Public Function GetCropType() As Integer
            Return ddl.SelectedValue
        End Function

        Public Function GetCropX() As String
            Return hdnCropX.Value
        End Function

        Public Function GetCropY() As String
            Return hdnCropY.Value
        End Function

        Public Function GetScale() As String
            Return hdnScale.Value
        End Function

#End Region

    End Class

End Namespace
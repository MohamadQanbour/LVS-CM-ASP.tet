Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Web.Routing
Imports System.Web.SessionState
Imports EGV
Imports EGV.Structures

Namespace EGVControls
    Namespace ImageCrop

#Region "Handler"

        Public Class ImageCropRouteHandler
            Implements IRouteHandler

            Public Function GetHttpHandler(requestContext As RequestContext) As IHttpHandler Implements IRouteHandler.GetHttpHandler
                Dim handler As IHttpHandler = Activator.CreateInstance(Of ImageCropHandler)()
                If TypeOf handler Is ImageCropHandler Then
                    CType(handler, ImageCropHandler).RouteData = requestContext.RouteData
                End If
                Return handler
            End Function

        End Class

        Public Class ImageCropHandler
            Implements IHttpHandler
            Implements IRequiresSessionState

            Public Property RouteData As RouteData

            Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
                Get
                    Return True
                End Get
            End Property

            Private Function ReadRouteValue(ByVal key As String) As String
                If RouteData.Values(key) IsNot Nothing Then
                    Return RouteData.Values(key)
                Else
                    Return ""
                End If
            End Function

            Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
                Dim appInstatnce As HttpApplication = context.ApplicationInstance
                Dim path As String = HttpUtility.UrlDecode(ReadRouteValue("path"))
                Dim maxWidth As String = ReadRouteValue("width")
                Dim maxHeight As String = ReadRouteValue("height")
                Dim cropRatio As String = ReadRouteValue("cropRatio")
                Dim scaleRatio As String = ReadRouteValue("scale")
                Dim basePath As String = ReadRouteValue("base")
                Dim assetsPath As String = Utils.Helper.AssetsPath()
                path = IIf(basePath = "cms", Utils.Helper.CMSPath() & "/", "") & assetsPath & "/" & path
                Dim cropper As New ImageCrop(appInstatnce, path, maxWidth, maxHeight, cropRatio, scaleRatio)
                Dim processedFile As String = cropper.GetProcessedImage()
                If processedFile <> "noimage" Then
                    Dim extension As String = processedFile.Substring(processedFile.LastIndexOf(".") + 1)
                    Dim processedPath As String = processedFile.Substring(0, processedFile.LastIndexOf("."))
                    If File.Exists(processedPath) Then
                        appInstatnce.Response.WriteFile(processedPath)
                        appInstatnce.Response.ContentType = "image/" & extension
                        appInstatnce.Response.StatusCode = 200
                    Else
                        appInstatnce.Response.StatusCode = 404
                    End If
                Else
                    appInstatnce.Response.StatusCode = 404
                End If
                context.ApplicationInstance.CompleteRequest()
            End Sub

        End Class

#End Region

#Region "Image Crop"

        <Serializable()>
        Public Class ImageCrop

#Region "Public Properties"

            Public Property AppInstance As HttpApplication
            Public Property Path As String
            Public Property MaxWidth As String = ""
            Public Property MaxHeight As String = ""
            Public Property Crop As String = ""
            Public Property Scale As String = ""

#End Region

#Region "Private Properties"

            Private Property Server As HttpServerUtility
            Private Property SourceImageFile As FileInfo
            Private Property SourceImage As Bitmap

            Private Property cropWidth As String
            Private Property cropHeight As String
            Private Property cropRatio As String
            Private Property cropMethod As Enums.CroppingTypes
            Private Property cropX As Single
            Private Property cropY As Single
            Private Property ScaleRatio As Single

#End Region

#Region "Constructors"

            Public Sub New(ByVal appInstance As HttpApplication, ByVal path As String, Optional ByVal maxWidth As String = "",
                       Optional ByVal maxHeight As String = "", Optional ByVal cropRatio As String = "", Optional ByVal scaleRatio As String = "")
                Me.AppInstance = appInstance
                Me.Path = path
                Me.MaxWidth = maxWidth
                Me.MaxHeight = maxHeight
                Crop = cropRatio

                Server = appInstance.Server

                If scaleRatio = Nothing OrElse scaleRatio = String.Empty OrElse scaleRatio = "0" Then
                    Scale = "1"
                    Me.ScaleRatio = 1
                Else
                    Scale = scaleRatio
                    Me.ScaleRatio = scaleRatio
                End If

                If Crop <> String.Empty Then
                    Dim parts = Crop.Split("x")
                    If parts.Length >= 2 Then
                        cropWidth = parts(0)
                        cropHeight = parts(1)
                        Me.cropRatio = cropWidth / cropHeight
                        If parts.Length >= 3 Then
                            cropMethod = parts(2)
                        Else
                            cropMethod = Enums.CroppingTypes.Center
                        End If
                        If cropMethod = 0 Then cropMethod = Enums.CroppingTypes.Center
                        If parts.Length >= 4 Then
                            Dim xy = parts(3).Split(",")
                            If xy.Length = 2 Then
                                cropX = xy(0)
                                cropY = xy(1)
                            Else
                                cropX = 0
                                cropY = 0
                            End If
                        Else
                            cropX = 0
                            cropY = 0
                        End If
                    Else
                        cropWidth = String.Empty
                        cropHeight = String.Empty
                        Me.cropRatio = String.Empty
                        cropMethod = Enums.CroppingTypes.Center
                        cropX = 0
                        cropY = 0
                    End If
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Function GetProcessedImage() As String
                Dim retFileName As String = "noimage"
                Dim serverPath As String = Server.MapPath("~/" & Path)
                If File.Exists(serverPath) Then
                    'Get Source Image
                    Try
                        SourceImageFile = New FileInfo(serverPath)
                        SourceImage = New Bitmap(serverPath)
                        If ShouldProcess() AndAlso ShouldProcessImage() Then
                            Dim enc As String = ""
                            Using md5Hash As MD5 = MD5.Create()
                                Dim data As Byte() = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Path & SourceImageFile.CreationTime.ToString("yyyyMMddhhmmss") & MaxWidth & MaxHeight & Crop & Scale))
                                Dim sb As New StringBuilder()
                                For i As Integer = 0 To data.Length - 1
                                    sb.Append(data(i).ToString("x2"))
                                Next
                                enc = sb.ToString()
                            End Using
                            Dim processedFilePath As String
                            Dim serverProcessedFilePath As String
                            If enc <> String.Empty Then
                                processedFilePath = "~/" & Utils.Helper.CropCachePath() & "/" & enc
                                serverProcessedFilePath = Server.MapPath(processedFilePath)
                                If File.Exists(serverProcessedFilePath) Then
                                    retFileName = serverProcessedFilePath & SourceImageFile.Extension
                                Else
                                    retFileName = BeginProcessing() & SourceImageFile.Extension
                                End If
                            End If
                        Else
                            retFileName = serverPath & SourceImageFile.Extension
                        End If
                        If SourceImage IsNot Nothing Then
                            SourceImage.Dispose()
                        End If
                    Catch ex As Exception
                        retFileName = "noimage"
                    End Try
                End If
                Return retFileName
            End Function

#End Region

#Region "Processing Methods"

            Private Function BeginProcessing() As String
                Dim serverPath As String = Server.MapPath("~/" & Path)
                Dim newPath As String = String.Empty
                Dim renderedImage As RenderedImage = GetRenderedImage()
                SourceImage.Dispose()
                Using ms As New MemoryStream()
                    Using fs As New FileStream(serverPath, FileMode.Open)
                        Dim bmp As Bitmap = Image.FromStream(fs)
                        'Scaling and Cropping
                        bmp = ScaleImage(bmp, renderedImage)
                        bmp = CropImage(bmp, renderedImage)
                        SaveTemporarly(bmp, ms, Utils.Helper.ImageCropQuality())
                        If bmp IsNot Nothing Then
                            bmp.Dispose()
                        End If
                    End Using
                    'save image to file
                    Dim enc As String = ""
                    Using md5Hash As MD5 = MD5.Create()
                        Dim data As Byte() = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Path & SourceImageFile.CreationTime.ToString("yyyyMMddhhmmss") & MaxWidth & MaxHeight & Crop & Scale))
                        Dim sb As New StringBuilder()
                        For j As Integer = 0 To data.Length - 1
                            sb.Append(data(j).ToString("x2"))
                        Next
                        enc = sb.ToString()
                    End Using
                    newPath = Server.MapPath("~/" & Utils.Helper.CropCachePath() & "/" & enc)
                    Dim imgData() As Byte = ms.ToArray()
                    Using dfs As New FileStream(newPath, FileMode.Create)
                        dfs.Write(imgData, 0, imgData.Length)
                    End Using
                End Using
                RecycleOldCache()
                Return newPath
            End Function

            Private Sub SaveTemporarly(ByVal bmp As Bitmap, ByVal ms As MemoryStream, ByVal quality As Integer)
                Dim qualityParam As New EncoderParameter(Imaging.Encoder.Quality, quality)
                Dim codec = GetImageCodecInfo()
                Dim encoderParams As New EncoderParameters(1)
                encoderParams.Param(0) = qualityParam
                If codec IsNot Nothing Then
                    bmp.Save(ms, codec, encoderParams)
                Else
                    bmp.Save(ms, GetImageFormat())
                End If
            End Sub

            Private Function ScaleImage(ByVal img As Bitmap, ByVal renderedImage As RenderedImage) As Bitmap
                Dim pFormat As PixelFormat = IIf(img.PixelFormat = PixelFormat.Indexed OrElse img.PixelFormat = PixelFormat.Format8bppIndexed OrElse img.PixelFormat = PixelFormat.Format4bppIndexed OrElse img.PixelFormat = PixelFormat.Format1bppIndexed, PixelFormat.Format32bppRgb, img.PixelFormat)
                Dim result As Bitmap = New Bitmap(renderedImage.Width, renderedImage.Height, pFormat)
                result.SetResolution(img.HorizontalResolution, img.VerticalResolution)
                Using g As Graphics = Graphics.FromImage(result)
                    g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                    g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                    g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                    g.DrawImage(img, 0, 0, renderedImage.Width, renderedImage.Height)
                End Using
                Return result
            End Function

            Private Function CropImage(ByVal img As Bitmap, ByVal renderedImage As RenderedImage) As Bitmap
                If renderedImage.CropWidth > 0D AndAlso renderedImage.CropHeight > 0D Then
                    Dim pFormat As PixelFormat = IIf(img.PixelFormat = PixelFormat.Indexed OrElse img.PixelFormat = PixelFormat.Format8bppIndexed OrElse img.PixelFormat = PixelFormat.Format4bppIndexed OrElse img.PixelFormat = PixelFormat.Format1bppIndexed, PixelFormat.Format32bppRgb, img.PixelFormat)
                    Dim result As Bitmap = New Bitmap(renderedImage.CropWidth, renderedImage.CropHeight, pFormat)
                    result.SetResolution(img.HorizontalResolution, img.VerticalResolution)
                    Using g As Graphics = Graphics.FromImage(result)
                        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                        g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                        g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                        g.DrawImage(img, New Rectangle(0, 0, renderedImage.CropWidth, renderedImage.CropHeight), GetDestRectange(renderedImage), GraphicsUnit.Pixel)
                    End Using
                    Return result
                Else
                    Return img
                End If
            End Function

            Private Function GetDestRectange(ByRef renderedImage As RenderedImage) As RectangleF
                Dim x As Decimal = 0
                Dim y As Decimal = 0
                Select Case cropMethod
                    Case Enums.CroppingTypes.Center
                        x = (renderedImage.Width - renderedImage.CropWidth) / 2
                        y = (renderedImage.Height - renderedImage.CropHeight) / 2
                    Case Enums.CroppingTypes.TopLeft
                        x = 0
                        y = 0
                    Case Enums.CroppingTypes.TopRight
                        x = renderedImage.Width - renderedImage.CropWidth
                        y = 0
                    Case Enums.CroppingTypes.BottomLeft
                        x = 0
                        y = renderedImage.Height - renderedImage.CropHeight
                    Case Enums.CroppingTypes.BottomRight
                        x = renderedImage.Width - renderedImage.CropWidth
                        y = renderedImage.Height - renderedImage.CropHeight
                    Case Enums.CroppingTypes.Manual
                        Dim newWidth = MaxWidth
                        Dim newHeight = newWidth * (renderedImage.Height / renderedImage.Width)
                        If newHeight < MaxHeight Then
                            newHeight = MaxHeight
                            newWidth = newHeight * (renderedImage.Width / renderedImage.Height)
                        End If
                        If Scale <> 1 Then
                            newWidth = Math.Round(newWidth * Scale)
                            newHeight = Math.Round(newHeight * Scale)
                        End If
                        x = cropX * (renderedImage.Width / newWidth)
                        y = cropY * (renderedImage.Height / newHeight)
                    Case Else
                        x = (renderedImage.Width - renderedImage.CropWidth) / 2
                        y = (renderedImage.Height - renderedImage.CropHeight) / 2
                End Select
                Return New RectangleF(x, y, renderedImage.CropWidth, renderedImage.CropHeight)
            End Function

            Private Function GetRenderedImage() As RenderedImage
                Dim sourceRatio As Decimal = SourceImage.Width / SourceImage.Height
                Dim targetCropRatio As Decimal = cropRatio
                Dim setCropHeight As Decimal = 0D
                Dim setCropWidth As Decimal = 0D
                'Set Crop Parameters
                If targetCropRatio > sourceRatio Then
                    'image too tall, crop top and bottom
                    setCropHeight = SourceImage.Width / targetCropRatio
                    setCropWidth = SourceImage.Width
                Else
                    'image too wide, crop sides
                    setCropWidth = SourceImage.Height * targetCropRatio
                    setCropHeight = SourceImage.Height
                End If
                Dim ret As New RenderedImage()
                'Set Resize Parameters
                If ShouldResizeBasedOnWidth(cropWidth) Then
                    Dim resizeFactor = GetResizeWidthFactor(cropWidth)
                    ret.Height = Math.Round(resizeFactor * SourceImage.Height)
                    ret.Width = Math.Round(resizeFactor * SourceImage.Width)
                    If IsCroppingNeeded() Then
                        ret.CropHeight = Math.Round(resizeFactor * setCropHeight)
                        ret.CropWidth = Math.Round(resizeFactor * setCropWidth)
                    End If
                ElseIf ShouldResizeBasedOnHeight(cropHeight) Then
                    Dim resizeFactor = GetResizeHeightFactor(cropHeight)
                    ret.Width = Math.Round(resizeFactor * SourceImage.Width)
                    ret.Height = Math.Round(resizeFactor * SourceImage.Height)
                    If IsCroppingNeeded() Then
                        ret.CropHeight = Math.Round(resizeFactor * setCropHeight)
                        ret.CropWidth = Math.Round(resizeFactor * setCropWidth)
                    End If
                ElseIf IsCroppingNeeded() Then
                    Dim ratio = IIf(GetResizeUncroppedWidthFactor() > GetResizeUncroppedHeightFactor(), GetResizeUncroppedWidthFactor(), GetResizeUncroppedHeightFactor())
                    ret.Width = Math.Round(ratio * SourceImage.Width)
                    ret.Height = Math.Round(ratio * SourceImage.Height)
                    ret.CropWidth = Math.Round(ratio * setCropWidth)
                    ret.CropHeight = Math.Round(ratio * setCropHeight)
                Else
                    ret.Width = IIf(SourceImage.Width < MaxWidth, SourceImage.Width, MaxWidth)
                    ret.Height = IIf(SourceImage.Height < MaxHeight, SourceImage.Height, MaxHeight)
                    ret.CropWidth = IIf(SourceImage.Width < MaxWidth, SourceImage.Width, MaxWidth)
                    ret.CropHeight = IIf(SourceImage.Height < MaxHeight, SourceImage.Height, MaxHeight)
                End If
                If Scale <> 1 Then
                    Dim scaledHeight = Math.Round(ret.Height * ScaleRatio)
                    Dim scaledWidth = Math.Round(ret.Width * ScaleRatio)
                    If scaledHeight > MaxHeight AndAlso scaledWidth > MaxWidth Then
                        ret.Height = scaledHeight
                        ret.Width = scaledWidth
                    End If
                End If
                Return ret
            End Function

#End Region

#Region "Recycling Methods"

            Private Sub RecycleOldCache()
                Dim cachePath As String = Utils.Helper.CropCachePath()
                Dim filePath As String = Server.MapPath("~/" & cachePath & "/config.sys")
                If Not File.Exists(filePath) Then
                    File.Create(filePath)
                End If
                Dim d As String = File.ReadAllText(filePath)
                Dim lastDate As Date
                If Date.TryParse(d, lastDate) Then
                    If DateDiff(DateInterval.Day, Now, lastDate) >= Utils.Helper.CropRecyclePeriod() Then
                        Dim dir As DirectoryInfo = New DirectoryInfo(Server.MapPath("~/" & cachePath))
                        For Each f As FileInfo In dir.GetFiles()
                            If DateDiff(DateInterval.Day, Now, f.CreationTime) >= 30 Then
                                Try
                                    If f.Name <> "config.sys" Then
                                        f.Delete()
                                    End If
                                Catch
                                End Try
                            End If
                        Next
                        UpdateRecycleDate(filePath)
                    End If
                Else
                    UpdateRecycleDate(filePath)
                End If
            End Sub

            Private Sub UpdateRecycleDate(ByVal filePath As String)
                Dim sw As StreamWriter = Nothing
                Try
                    sw = New StreamWriter(filePath)
                    sw.Write(Now.ToString("M/d/yyyy"))
                Catch ex As Exception
                Finally
                    If sw IsNot Nothing Then
                        sw.Close()
                    End If
                End Try
            End Sub

#End Region

#Region "Checking Processing Properties"

            Private Function ShouldProcess() As Boolean
                Return (MaxWidth <> String.Empty AndAlso MaxHeight <> String.Empty) _
                OrElse (MaxWidth <> String.Empty AndAlso MaxHeight <> String.Empty AndAlso Crop <> String.Empty)
            End Function

            Private Function IsWidthDifferent() As Boolean
                Return MaxWidth < SourceImage.Width
            End Function

            Private Function IsHeightDifferent() As Boolean
                Return MaxHeight < SourceImage.Height
            End Function

            Private Function IsCroppingNeeded() As Boolean
                Return cropWidth <> String.Empty AndAlso cropHeight <> String.Empty AndAlso cropRatio <> String.Empty AndAlso (cropRatio <> (SourceImage.Width / SourceImage.Height) OrElse ScaleRatio <> 1)
            End Function

            Private Function ShouldProcessImage() As Boolean
                Return IsWidthDifferent() OrElse IsHeightDifferent() OrElse IsCroppingNeeded()
            End Function

            Private Function ShouldResizeBasedOnWidth(ByVal cropWidth As Decimal) As Boolean
                Return Math.Floor(GetResizeWidthFactor(cropWidth) * SourceImage.Height) <= MaxHeight
            End Function

            Private Function ShouldResizeBasedOnHeight(ByVal cropHeight As Decimal) As Boolean
                Return Math.Floor(GetResizeHeightFactor(cropHeight) * SourceImage.Width) <= MaxWidth
            End Function

#End Region

#Region "Cropping and Resizing Methods"

            Private Function GetResizeWidthFactor(ByVal cropWidth As Decimal) As Decimal
                If cropWidth <> 0 Then
                    Return GetResizeCroppedWidthFactor(cropWidth)
                Else
                    Return GetResizeUncroppedWidthFactor()
                End If
            End Function

            Private Function GetResizeHeightFactor(ByVal cropHeight As Decimal) As Decimal
                If cropHeight <> 0 Then
                    Return GetResizeCroppedHeightFactor(cropHeight)
                Else
                    Return GetResizeUncroppedHeightFactor()
                End If
            End Function

            Private Function GetResizeCroppedWidthFactor(ByVal cropWidth As Decimal) As Decimal
                Return CDec(MaxWidth) / cropWidth
            End Function

            Private Function GetResizeCroppedHeightFactor(ByVal cropHeight As Decimal) As Decimal
                Return CDec(MaxHeight) / cropHeight
            End Function

            Private Function GetResizeUncroppedWidthFactor() As Decimal
                Return CDec(MaxWidth) / SourceImage.Width
            End Function

            Private Function GetResizeUncroppedHeightFactor() As Decimal
                Return CDec(MaxHeight) / SourceImage.Height
            End Function

#End Region

#Region "Check Image Properties"

            Private Function GetImageFormat() As ImageFormat
                Dim ext = SourceImageFile.Extension
                Select Case ext
                    Case ".gif"
                        Return ImageFormat.Gif
                    Case ".png"
                        Return ImageFormat.Png
                    Case ".jpg", ".jpeg"
                        Return ImageFormat.Jpeg
                    Case ".bmp"
                        Return ImageFormat.Bmp
                    Case Else
                        Return ImageFormat.Jpeg
                End Select
            End Function

            Private Function GetImageCodecInfo() As ImageCodecInfo
                Dim ext = SourceImageFile.Extension
                Select Case ext
                    Case ".bmp"
                        Return ImageCodecInfo.GetImageEncoders()(0)
                    Case ".jpg", ".jpeg"
                        Return ImageCodecInfo.GetImageEncoders()(1)
                    Case ".gif"
                        Return ImageCodecInfo.GetImageEncoders()(2)
                    Case ".tiff"
                        Return ImageCodecInfo.GetImageEncoders()(3)
                    Case ".png"
                        Return ImageCodecInfo.GetImageEncoders()(4)
                    Case Else
                        Return Nothing
                End Select
            End Function

#End Region

        End Class

#End Region

    End Namespace
End Namespace
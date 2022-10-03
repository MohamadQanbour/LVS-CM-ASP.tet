Imports System.Web
Imports System.Web.UI
Imports System.Web.SessionState
Imports System.Web.Configuration
Imports System.Globalization
Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.Script.Serialization
Imports EGV.Enums
Imports EGV.Business
Imports System.Threading
Imports System.Reflection
Imports System.Text.RegularExpressions

Namespace EGV
    Namespace Utils
        Public Class Helper

#Region "Public Properties"

            Public Shared ReadOnly Property Session As HttpSessionState
                Get
                    Return HttpContext.Current.Session
                End Get
            End Property

            Public Shared ReadOnly Property ViewState As StateBag
                Get
                    Dim viewStateProp As Object = Nothing
                    If TypeOf Page Is Page AndAlso Page IsNot Nothing Then
                        Dim vsp = Page.GetType().GetProperty("ViewState", BindingFlags.FlattenHierarchy Or BindingFlags.Instance Or BindingFlags.NonPublic)
                        If vsp IsNot Nothing Then
                            viewStateProp = DirectCast(vsp.GetValue(Page), StateBag)
                        End If
                    End If
                    Return viewStateProp
                End Get
            End Property

            Public Shared ReadOnly Property Request As HttpRequest
                Get
                    Return HttpContext.Current.Request
                End Get
            End Property

            Public Shared ReadOnly Property Response As HttpResponse
                Get
                    Return HttpContext.Current.Response
                End Get
            End Property

            Public Shared ReadOnly Property Server As HttpServerUtility
                Get
                    Return HttpContext.Current.Server
                End Get
            End Property

            Public Shared ReadOnly Property Page As Page
                Get
                    If TypeOf HttpContext.Current.Handler Is Page Then Return CType(HttpContext.Current.Handler, Page) Else Return Nothing
                End Get
            End Property

            Public Shared ReadOnly Property CMSAuthUser As Business.User
                Get
                    If Session(GetSessionId("CMSUser")) IsNot Nothing Then
                        Return Session(GetSessionId("CMSUser"))
                    Else
                        If AutoLogin() Then Return Session(GetSessionId("CMSUser")) Else Return Nothing
                    End If
                End Get
            End Property

            Public Shared ReadOnly Property LanguageId As Integer
                Get
                    Dim target = Session(GetSessionId("LanguageId"))
                    If target Is Nothing OrElse Not IsNumeric(target) Then
                        SetLanguageId(GetDefaultLanguageId())
                    End If
                    Return Session(GetSessionId("LanguageId"))
                End Get
            End Property

            Public Shared ReadOnly Property Language As Language
                Get
                    If Session(GetSessionId("Langauge")) Is Nothing OrElse DirectCast(Session(GetSessionId("Language")), Language).Id <> LanguageId Then Session(GetSessionId("Language")) = New Language(LanguageId, LanguageId)
                    Return Session(GetSessionId("Language"))
                End Get
            End Property

#End Region

#Region "Public Methods"

#Region "Application Settings"

            Public Shared Function ReadAppSetting(ByVal key As String) As String
                Return WebConfigurationManager.AppSettings(key).ToString()
            End Function

            Public Shared Function IsRemote() As Boolean
                Return ReadAppSetting("IsRemote")
            End Function

            Public Shared Function CMSPath() As String
                Return ReadAppSetting("CMSPath")
            End Function

            Public Shared Function StructuresPath() As String
                Return ReadAppSetting("StructuresFilePath")
            End Function

            Public Shared Function DebuggingEnabled() As Boolean
                Return ReadAppSetting("Debug")
            End Function

            Public Shared Function StylesVersion() As String
                Return ReadAppSetting("StylesVersion")
            End Function

            Public Shared Function ScriptsVersion() As String
                Return ReadAppSetting("ScriptsVersion")
            End Function

            Public Shared Function SessionPrefix() As String
                Return ReadAppSetting("SessionPrefix")
            End Function

            Public Shared Function EncryptionKey() As String
                Return ReadAppSetting("EncryptionKey")
            End Function

            Public Shared Function CMSVersion() As String
                Return ReadAppSetting("CMSVersion")
            End Function

            Public Shared Function AssetsPath() As String
                Return ReadAppSetting("AssetsPath")
            End Function

            Public Shared Function CropCachePath() As String
                Return ReadAppSetting("ImageCropCachePath")
            End Function

            Public Shared Function ImageCropQuality() As String
                Return ReadAppSetting("ImageCropQuality")
            End Function

            Public Shared Function CropRecyclePeriod() As Integer
                Return ReadAppSetting("CropRecyclePeriod")
            End Function

            Public Shared Function EnableURLRewrite() As Boolean
                Return ReadAppSetting("EnableURLRewriting")
            End Function

            Public Shared Function AdvancedURLRewrite() As Boolean
                Return ReadAppSetting("AdvancedURLRewriting")
            End Function

            Public Shared Function UsePortalPrefix() As Boolean
                Return ReadAppSetting("UsePortalPrefix")
            End Function

            Public Shared Function UseLanguagePrefix() As Boolean
                Return ReadAppSetting("UseLanguagePrefix")
            End Function

            Public Shared Function PortalPrefix() As String
                Return ReadAppSetting("DefaultPortal")
            End Function

            Public Shared Function MailTemplatePath() As String
                Return ReadAppSetting("MailTemplate")
            End Function

            Public Shared Function PrivateKey() As String
                Return ReadAppSetting("SecureKey")
            End Function

            Public Shared Function OneSignalAPIKey() As String
                Return ReadAppSetting("OneSignalAPIKey")
            End Function

            Public Shared Function OneSignalAppId() As String
                Return ReadAppSetting("OneSignalAppId")
            End Function

#End Region

#Region "Web Controls"

            Public Shared Function FindControl(ByVal parent As Control, ByVal targetId As String) As Control
                Dim ret As Control = Nothing
                If parent.HasControls() Then
                    ret = parent.FindControl(targetId)
                    If ret Is Nothing Then
                        Dim i As Integer = 0
                        While ret Is Nothing AndAlso i < parent.Controls.Count
                            ret = FindControl(parent.Controls(i), targetId)
                            i += 1
                        End While
                    End If
                End If
                Return ret
            End Function

#End Region

#Region "Textual"

            Public Shared Function NL2BR(ByVal source As String) As String
                Return source.Replace(Environment.NewLine, "<br />").Replace("\n", "<br />").Replace("\r", "<br />")
            End Function

            Public Shared Function BR2NL(ByVal source As String) As String
                Return source.Replace("<br />", Environment.NewLine)
            End Function

            Public Shared Function SplitString(ByVal source As String, ByVal splitter As String) As String()
                Return source.Split(New String() {splitter}, StringSplitOptions.RemoveEmptyEntries)
            End Function

#End Region

#Region "Misc"

            Public Shared Function GetSessionId(ByVal id As String) As String
                Return (SessionPrefix() & id).ToLower().Replace(" ", "")
            End Function

            Public Shared Function GetCookieId(ByVal id As String) As String
                Return GetSessionId(id)
            End Function

#End Region

#Region "Cookies"

            Public Shared Sub WriteCookie(ByVal cookieId As String, ByVal expiry As Date, ByVal ParamArray values() As Structures.CookieItem)
                Dim aCookie As New HttpCookie(GetCookieId(cookieId))
                For Each v As Structures.CookieItem In values
                    aCookie(v.Id) = v.Value
                Next
                aCookie.Expires = expiry
                Response.Cookies.Add(aCookie)
            End Sub

            Public Shared Function ReadCookie(ByVal cookieId As String) As HttpCookie
                If Request.Cookies(GetCookieId(cookieId)) IsNot Nothing Then Return Request.Cookies(GetCookieId(cookieId)) Else Return Nothing
            End Function

            Public Shared Sub RemoveCookie(ByVal cookieId As String)
                Dim c = ReadCookie(cookieId)
                If c IsNot Nothing Then
                    c.Expires = Now.AddDays(-1D)
                    c.Value = String.Empty
                    Response.Cookies.Add(c)
                End If
            End Sub

#End Region

#Region "Languages"

            Public Shared Sub GetSafeLanguageId(ByRef langId As Integer)
                If langId = 0 Then langId = LanguageId
            End Sub

            Public Shared Sub SetLanguageId(ByVal langId As Integer)
                Session(GetSessionId("LanguageId")) = langId
            End Sub

            Public Shared Function GetDefaultLanguageId(Optional ByVal conn As SqlConnection = Nothing) As Integer
                Return LanguageController.GetDefaultId(conn)
            End Function

            Public Shared Sub LoadCulture()
                Thread.CurrentThread.CurrentUICulture = New CultureInfo(Language.LanguageCode)
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Language.LanguageCode)
            End Sub

            Public Shared Function GetCulture() As CultureInfo
                Return Thread.CurrentThread.CurrentUICulture
            End Function

            Public Shared Function GetHTMLDirection() As String
                Return IIf(Language.IsRTL, "rtl", "ltr")
            End Function

            Public Shared Function GetHTMLAltDirection() As String
                Return IIf(Language.IsRTL, "ltr", "rtl")
            End Function

            Public Shared Function GetCSSDirection() As String
                Return IIf(Language.IsRTL, "right", "left")
            End Function

            Public Shared Function GetCSSAltDirection() As String
                Return IIf(Language.IsRTL, "left", "right")
            End Function

            Public Shared Sub LoadLanguage(Optional ByVal conn As SqlConnection = Nothing)
                Dim langId As Integer = 0
                If CMSAuthUser IsNot Nothing Then
                    GetSafeLanguageId(CMSAuthUser.Profile.UserLanguageId)
                    langId = CMSAuthUser.Profile.UserLanguageId
                End If
                GetSafeLanguageId(langId)
                If langId <> LanguageId Then
                    SetLanguageId(langId)
                    LoadCulture()
                End If
            End Sub

#End Region

#Region "Casting"

            Public Shared Function GetSafeObject(ByVal obj As Object, Optional ByVal type As Enums.ValueTypes = Enums.ValueTypes.TypeInteger) As Object
                Select Case type
                    Case ValueTypes.TypeInteger
                        Return GetObjectAsInteger(obj)
                    Case ValueTypes.TypeDecimal
                        Return GetObjectAsDecimal(obj)
                    Case ValueTypes.TypeString
                        Return GetObjectAsString(obj)
                    Case ValueTypes.TypeDate
                        Return GetObjectAsDate(obj)
                    Case ValueTypes.TypeDateTime
                        Return GetObjectAsDatetime(obj)
                    Case ValueTypes.TypeBoolean
                        Return GetObjectAsBoolean(obj)
                    Case Else
                        Return GetObjectAsString(obj)
                End Select
            End Function

            Public Shared Function GetSafeDBValue(ByVal field As Object, Optional ByVal dbType As Enums.ValueTypes = Enums.ValueTypes.TypeString) As Object
                Select Case dbType
                    Case ValueTypes.TypeInteger
                        Return GetSafeDBInteger(field)
                    Case ValueTypes.TypeDecimal
                        Return GetSafeDBDecimal(field)
                    Case ValueTypes.TypeString
                        Return GetSafeDBString(field)
                    Case ValueTypes.TypeDate
                        Return GetSafeDBDate(field)
                    Case ValueTypes.TypeDateTime
                        Return GetSafeDBDatetime(field)
                    Case ValueTypes.TypeBoolean
                        Return GetSafeDBBoolean(field)
                    Case Else
                        Return GetSafeDBString(field)
                End Select
            End Function

#End Region

#Region "XML"

            Public Shared Function GetSafeXML(ByVal n As Xml.XmlNode, ByVal target As String, Optional ByVal type As Enums.ValueTypes = Enums.ValueTypes.TypeString) As Object
                Select Case type
                    Case Enums.ValueTypes.TypeBoolean
                        Return GetSafeXMLBoolean(n, target)
                    Case Enums.ValueTypes.TypeInteger
                        Return GetSafeXMLInteger(n, target)
                    Case Else
                        Return GetSafeXMLText(n, target)
                End Select
            End Function

            Public Shared Function GetSafeXMLText(ByVal n As Xml.XmlNode, ByVal target As String) As String
                Dim t = n.SelectSingleNode(target)
                If t IsNot Nothing Then Return t.InnerText Else Return String.Empty
            End Function

            Public Shared Function GetSafeXMLBoolean(ByVal n As Xml.XmlNode, ByVal target As String) As Boolean
                Dim t = n.SelectSingleNode(target)
                If t IsNot Nothing Then Return GetObjectAsBoolean(t.InnerText) Else Return False
            End Function

            Public Shared Function GetSafeXMLInteger(ByVal n As Xml.XmlNode, ByVal target As String) As Integer
                Dim t = n.SelectSingleNode(target)
                If t IsNot Nothing Then Return GetObjectAsInteger(t.InnerText) Else Return 0
            End Function

#End Region

#Region "User"

            Public Shared Sub LoadUser(ByVal usr As Business.User, Optional ByVal saveCookie As Boolean = False)
                Session.Add(GetSessionId("CMSUser"), usr)
                If saveCookie Then
                    Dim lst As New List(Of Structures.CookieItem)
                    lst.Add(New Structures.CookieItem() With {.Id = "username", .Value = usr.UserName})
                    lst.Add(New Structures.CookieItem() With {.Id = "password", .Value = Encrypt(usr.Password)})
                    WriteCookie("CMSUserAutoLogin", Now.AddDays(3), lst.ToArray())
                End If
            End Sub

            Public Shared Sub UnloadUser()
                Session.Remove(GetSessionId("CMSUser"))
                RemoveCookie("CMSUserAutoLogin")
            End Sub

            Public Shared Sub ReloadUser(Optional ByVal conn As SqlConnection = Nothing)
                LoadUser(New Business.User(CMSAuthUser.Id, conn), True)
            End Sub

            Public Shared Function AutoLogin(Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim reload As Boolean = False
                Dim c = ReadCookie("CMSUserAutoLogin")
                If c IsNot Nothing Then
                    Dim username As String = c.Values().Item("username")
                    Dim pass As String = c.Values().Item("password")
                    DBA.GetSafeConn(conn)
                    Try
                        conn.Open()
                        Dim usr As Business.User = Business.UserController.GetByUsername(username, conn)
                        If usr IsNot Nothing Then
                            reload = VerifyEncrypted(usr.Password, pass)
                            If reload Then LoadUser(usr)
                        End If
                    Catch ex As Exception
                        reload = False
                    Finally
                        conn.Close()
                    End Try
                End If
                Return reload
            End Function

            Public Shared Sub GetSafeUserId(ByRef usrId As Integer)
                If usrId = 0 Then usrId = CMSAuthUser.Id
            End Sub

            Public Shared Function GeneratePassword() As String
                Dim allowedChars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
                Dim r As New Random()
                Dim sb As New StringBuilder()
                For i As Integer = 1 To 8
                    Dim idx As Integer = r.Next(0, allowedChars.Length - 1)
                    sb.Append(allowedChars.Substring(idx, 1))
                Next
                Return sb.ToString()
            End Function

#End Region

#Region "Security"

            Public Shared Function Encrypt(ByVal source As String)
                Return GetMD5Hash(source & EncryptionKey())
            End Function

            Public Shared Function VerifyEncrypted(ByVal source As String, ByVal encrypted As String) As Boolean
                Return VerifyMD5Hash(source & EncryptionKey(), encrypted)
            End Function

#End Region

#Region "Java Script"

            Public Shared Function JSSerialize(ByVal obj As Object) As String
                Dim js As New JavaScriptSerializer()
                Return js.Serialize(obj)
            End Function

            Public Shared Function JSDeserialize(Of T)(ByVal json As String) As T
                Dim js = New JavaScriptSerializer()
                Return js.Deserialize(Of T)(json)
            End Function

#End Region

#Region "URL Formation"

            Public Shared Function FormImageUrl(ByVal imagePath As String, Optional ByVal width As Integer = 0, Optional ByVal height As Integer = 0,
                                                Optional ByVal cropWidth As Integer = 0, Optional ByVal cropHeight As Integer = 0,
                                                Optional ByVal cropMethod As CroppingTypes = CroppingTypes.Center, Optional ByVal cropX As Integer = 0,
                                                Optional ByVal cropY As Integer = 0, Optional ByVal scale As String = "",
                                                Optional ByVal basePath As String = "portal") As String
                Dim ret As String = "/files/base/" & basePath & "/"
                If width > 0 Then ret &= "width/" & width & "/"
                If height > 0 Then ret &= "height/" & height & "/"
                If cropWidth > 0 AndAlso cropHeight > 0 Then
                    ret &= "crop/" & cropWidth & "x" & cropHeight & "x" & cropMethod
                    If cropMethod = CroppingTypes.Manual Then ret &= "x" & cropX & "," & cropY
                    ret &= "/"
                End If
                If scale <> String.Empty Then ret &= "scale/" & scale & "/"
                If imagePath <> String.Empty Then
                    Dim targetpath As String = imagePath
                    If basePath = "cms" Then targetpath = targetpath.Replace(CMSPath() & "/", "")
                    targetpath = targetpath.Replace(AssetsPath() & "/", "")
                    If targetpath.StartsWith("/") Then targetpath = targetpath.Substring(1)
                    ret &= targetpath
                Else
                    ret &= "/no-image.jpg"
                End If
                Return ret
            End Function

            Public Shared Function DeformImageUrl(ByVal fullPath As String) As String
                Dim rep As String = "/files/base/"
                Dim parts() = SplitString(fullPath, "/")
                Dim base As String = String.Empty
                Dim width As String = String.Empty
                Dim height As String = String.Empty
                Dim crop As String = String.Empty
                Dim scale As String = String.Empty
                For i As Integer = 0 To parts.Length - 1
                    If parts(i) = "base" Then base = parts(i + 1)
                    If parts(i) = "width" Then width = parts(i + 1)
                    If parts(i) = "height" Then height = parts(i + 1)
                    If parts(i) = "crop" Then crop = parts(i + 1)
                    If parts(i) = "scale" Then scale = parts(i + 1)
                Next
                rep &= base
                If width <> String.Empty Then rep &= "/width/" & width
                If height <> String.Empty Then rep &= "/height/" & height
                If crop <> String.Empty Then rep &= "/crop/" & crop
                If scale <> String.Empty Then rep &= "/scale/" & scale
                Dim ret As String = ""
                If base = "cms" Then ret &= "/" & CMSPath()
                ret &= "/" & AssetsPath()
                ret &= fullPath.Replace(rep, "")
                Return ret
            End Function

            Public Shared Function EscapeURL(ByVal source As String) As String
                source = source.Replace(" ", "-")
                Dim parts() As Char = source.ToCharArray()
                Dim allowedChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_"
                Dim ret As String = ""
                For Each item As Char In parts
                    If allowedChars.IndexOf(item) >= 0 Then ret &= item
                Next
                If ret = String.Empty Then
                    Dim g = Guid.NewGuid().ToString()
                    ret = Now.ToString("yyyyMMddhhmmss") & "-" & g.Substring(g.LastIndexOf("-") + 1)
                End If
                Dim rgx As New Regex("\-+")
                ret = rgx.Replace(ret, "-")
                If ret.ToCharArray()(0) = "-" Then ret = ret.Substring(1)
                If ret.ToCharArray()(ret.Length - 1) = "-" Then ret = ret.Substring(0, ret.Length - 1)
                Return ret
            End Function

#End Region

#Region "Enums"

            Public Shared Function GetEnumText(ByVal enumType As Type, ByVal value As Integer) As String
                Return System.Enum.GetName(enumType, value)
            End Function

            Public Shared Function GetEnumText(ByVal enumStringType As String, ByVal value As Integer) As String
                Return GetEnumText(GetEnumType(enumStringType), value)
            End Function

            Public Shared Function GetEnumValue(ByVal enumType As Type, ByVal name As String) As Integer
                Return System.Enum.Parse(enumType, name)
            End Function

            Public Shared Function GetEnumValue(ByVal enumStringType As String, ByVal name As String) As Integer
                Return GetEnumValue(GetEnumType(enumStringType), name)
            End Function

            Public Shared Function GetEnumType(ByVal type As String) As Type
                Dim exAssembly = Assembly.GetExecutingAssembly()
                Return exAssembly.GetTypes().First(Function(f) f.Name = type)
            End Function

#End Region

#Region "Date"

            Public Shared Function FormateDateDifference(ByVal d As DateTime, Optional ByVal addPostfix As Boolean = False) As String
                Dim ret As String = ""
                Dim diff As Long = DateDiff(DateInterval.Second, d, Now)
                Dim original As Long = diff
                Dim unit As String = "Seconds"
                If original > 60 Then
                    diff = Math.Floor(diff / 60)
                    unit = "Minutes"
                End If
                If original > 60 * 60 Then
                    diff = Math.Floor(diff / 60)
                    unit = "Hours"
                End If
                If original > 60 * 60 * 24 Then
                    diff = Math.Floor(diff / 24)
                    unit = "Days"
                End If
                ret = diff & " " & Localization.GetResource("Resources.Global.CMS." & unit) & " " & IIf(addPostfix, Localization.GetResource("Resources.Global.CMS.Ago"), "")
                If original > 60 * 60 * 24 * 30 Then
                    ret = d.ToString("MMMM dd, yyyy")
                End If
                Return ret
            End Function

            Public Shared Function ParseDate(ByVal source As String, Optional ByVal format As String = "yyyy-MM-dd") As Date
                Return DateTime.ParseExact(source, format, CultureInfo.InvariantCulture)
            End Function

#End Region

#Region "General"

            Public Shared Sub DatatableToCSV(ByVal tbl As DataTable, ByVal fileName As String)
                Dim writer As IO.StreamWriter = Nothing
                Dim flag As Boolean = False
                Dim dirPath As String = Server.MapPath("~" & Utils.Path.MapCMSAsset("excel/"))
                If Not IO.Directory.Exists(dirPath) Then
                    IO.Directory.CreateDirectory(dirPath)
                End If
                Dim path As String = dirPath & fileName & ".csv"
                Try
                    writer = New IO.StreamWriter(path, True, Encoding.UTF8)
                    Dim sep As String = ""
                    Dim builder As New StringBuilder()
                    'add table captions
                    builder = New StringBuilder()
                    For Each col As System.Data.DataColumn In tbl.Columns
                        builder.Append(sep).Append(col.Caption)
                        sep = ","
                    Next
                    writer.WriteLine(builder.ToString())
                    'add rows
                    For Each row As DataRow In tbl.Rows
                        sep = ""
                        builder = New StringBuilder()
                        For Each col As System.Data.DataColumn In tbl.Columns
                            If row(col.ColumnName).ToString.Contains(",") Then
                                builder.Append(sep).Append("""" & row(col.ColumnName) & """")
                            Else
                                builder.Append(sep).Append(row(col.ColumnName))
                            End If
                            sep = ","
                        Next
                        Dim line As String = builder.ToString()
                        line = line.Replace(Environment.NewLine, " ")
                        writer.WriteLine(line)
                    Next
                Catch ex As Exception
                    flag = True
                    Throw ex
                Finally
                    If Not writer Is Nothing Then writer.Close()
                End Try
                If Not flag Then
                    HttpContext.Current.Response.Clear()
                    HttpContext.Current.Response.ClearContent()
                    HttpContext.Current.Response.ClearHeaders()
                    HttpContext.Current.Response.ContentType = "application/vnd.ms-excel; charset=UTF-8"
                    HttpContext.Current.Response.Charset = "UTF-8"
                    HttpContext.Current.Response.ContentEncoding = Encoding.UTF8
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" & fileName & ".csv")
                    HttpContext.Current.Response.WriteFile(path)
                    HttpContext.Current.Response.End()
                End If
            End Sub

            Public Shared Function FormatFileSize(ByVal size As Integer) As String
                Dim s As Decimal = size
                If s < 1024 Then Return s.ToString("N2") & Localization.GetResource("Resources.Global.CMS.Bytes")
                s = s / 1024
                If s < 1024 Then Return s.ToString("N2") & Localization.GetResource("Resources.Global.CMS.KBytes")
                s = s / 1024
                If s < 1024 Then Return s.ToString("N2") & Localization.GetResource("Resources.Global.CMS.MBytes")
                s = s / 1024
                Return s.ToString("N2") & Localization.GetResource("Resources.Global.CMS.GBytes")
            End Function

            Public Shared Function FetchInstance(ByVal fullyQualifiedClassName As String) As Object
                Dim nspc As String = fullyQualifiedClassName
                Dim o As Object = Nothing
                Try
                    For Each ay In Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                        If (ay.Name = nspc) Then
                            o = Assembly.Load(ay).CreateInstance(fullyQualifiedClassName)
                            Exit For
                        End If
                    Next
                Catch
                End Try
                Return o
            End Function

#End Region

#End Region

#Region "Private Methods"

#Region "Casting"

            Private Shared Function GetObjectAsInteger(ByVal obj As Object) As Integer
                If obj IsNot Nothing AndAlso IsNumeric(obj.ToString()) Then Return obj Else Return 0
            End Function

            Private Shared Function GetObjectAsDecimal(ByVal obj As Object) As Decimal
                If obj IsNot Nothing AndAlso IsNumeric(obj.ToString()) Then Return obj Else Return 0D
            End Function

            Private Shared Function GetObjectAsString(ByVal obj As Object) As String
                If obj IsNot Nothing Then Return obj Else Return String.Empty
            End Function

            Private Shared Function GetObjectAsBoolean(ByVal obj As Object) As Boolean
                Dim ret As Boolean = False
                If obj IsNot Nothing Then
                    If obj.ToString().ToLower() = "true" Then ret = True
                End If
                Return ret
            End Function

            Private Shared Function GetObjectAsDate(ByVal obj As Object) As Date
                Dim ret As Date = Date.MinValue
                If obj IsNot Nothing Then
                    Try
                        ret = Date.ParseExact(obj.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    Catch ex As Exception
                        Throw ex
                    End Try
                End If
                Return ret
            End Function

            Private Shared Function GetObjectAsDatetime(ByVal obj As Object) As DateTime
                Dim ret As DateTime = DateTime.MinValue
                If obj IsNot Nothing Then
                    Try
                        ret = DateTime.ParseExact(obj.ToString(), "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture)
                    Catch ex As Exception
                        Throw ex
                    End Try
                End If
                Return ret
            End Function

            Public Shared Function GetSafeDBInteger(ByVal field As Object) As Integer
                If field IsNot Nothing AndAlso Not IsDBNull(field) AndAlso IsNumeric(field.ToString()) Then Return field Else Return 0
            End Function

            Public Shared Function GetSafeDBDecimal(ByVal field As Object) As Decimal
                If field IsNot Nothing AndAlso Not IsDBNull(field) AndAlso IsNumeric(field.ToString()) Then Return field Else Return 0D
            End Function

            Public Shared Function GetSafeDBString(ByVal field As Object) As String
                If field IsNot Nothing AndAlso Not IsDBNull(field) Then Return field.ToString() Else Return String.Empty
            End Function

            Public Shared Function GetSafeDBDate(ByVal field As Object) As Date
                If field IsNot Nothing AndAlso Not IsDBNull(field) Then Return CDate(field) Else Return Date.MinValue
            End Function

            Public Shared Function GetSafeDBDatetime(ByVal field As Object) As DateTime
                If field IsNot Nothing AndAlso Not IsDBNull(field) Then Return Convert.ToDateTime(field) Else Return DateTime.MinValue
            End Function

            Public Shared Function GetSafeDBBoolean(ByVal field As Object) As Boolean
                If field IsNot Nothing AndAlso Not IsDBNull(field) AndAlso field.ToString().ToLower() = "true" Then Return True Else Return False
            End Function

#End Region

#Region "Security"

            Private Shared Function GetMD5Hash(ByVal input As String, Optional ByVal md As MD5 = Nothing) As String
                If md Is Nothing Then md = MD5.Create()
                Using md
                    Dim data As Byte() = md.ComputeHash(Encoding.UTF8.GetBytes(input))
                    Dim sBuilder As New StringBuilder()
                    Dim i As Integer
                    For i = 0 To data.Length - 1
                        sBuilder.Append(data(i).ToString("x2"))
                    Next i
                    Return sBuilder.ToString()
                End Using
            End Function

            Private Shared Function VerifyMD5Hash(ByVal input As String, ByVal hash As String) As Boolean
                Using md As MD5 = MD5.Create()
                    Dim hashOfInput As String = GetMD5Hash(input, md)
                    Dim comparer As StringComparer = StringComparer.OrdinalIgnoreCase
                    If 0 = comparer.Compare(hashOfInput, hash) Then Return True Else Return False
                End Using
            End Function

#End Region

#End Region

        End Class
    End Namespace
End Namespace
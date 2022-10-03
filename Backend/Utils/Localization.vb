Imports System.Web
Imports System.Xml
Imports EGV.Structures
Imports EGV.Business

Namespace EGV
    Namespace Utils
        Public Class Localization

#Region "Private Members"

            Private Shared Property TargetLocalFile As LocalizationItem
                Get
                    If Helper.ViewState IsNot Nothing Then
                        If Helper.ViewState("LocalResourceFile") IsNot Nothing Then Return Helper.ViewState("LocalResourceFile") Else Return New LocalizationItem()
                    Else
                        If Helper.Session("LocalResourceFile") IsNot Nothing Then Return Helper.Session("LocalResourceFile") Else Return New LocalizationItem()
                    End If
                End Get
                Set(value As LocalizationItem)
                    If Helper.ViewState IsNot Nothing Then Helper.ViewState("LocalResourceFile") = value Else Helper.Session("LocalResourceFile") = value
                End Set
            End Property

            Private Shared Property TargetGlobalFile As LocalizationItem
                Get
                    If Helper.ViewState IsNot Nothing Then
                        If Helper.ViewState("GlobalResourceFile") IsNot Nothing Then Return Helper.ViewState("GlobalResourceFile") Else Return New LocalizationItem()
                    Else
                        If Helper.Session("GlobalResourceFile") IsNot Nothing Then Return Helper.Session("GlobalResourceFile") Else Return New LocalizationItem()
                    End If
                End Get
                Set(value As LocalizationItem)
                    If Helper.ViewState IsNot Nothing Then Helper.ViewState("GlobalResourceFile") = value Else Helper.Session("GlobalResourceFile") = value
                End Set
            End Property

#End Region

#Region "Private Methods"

            Private Shared Sub LoadTargetLocalResourceFile(ByVal virtualPath As String, ByVal langId As Integer)
                Dim shouldLoad As Boolean = False
                Dim fileName As String = GetLocalResourceFileDirectory(virtualPath, langId)
                If TargetLocalFile.FileName = String.Empty OrElse TargetLocalFile.FileName <> fileName OrElse TargetLocalFile.LanguageId <> langId Then shouldLoad = True
                If shouldLoad Then
                    Dim obj As New LocalizationItem() With {
                        .FileName = fileName,
                        .LanguageId = langId,
                        .Resoruces = New List(Of LocalizationResourceItem)()
                    }
                    Dim loaded As Boolean = True
                    Dim doc As New XmlDocument()
                    Try
                        doc.Load(Helper.Server.MapPath(fileName))
                    Catch ex As Exception
                        loaded = False
                    End Try
                    If loaded Then
                        For Each n As XmlNode In (From x As XmlNode In doc.SelectNodes("root/data") Where x.NodeType <> XmlNodeType.Comment)
                            Dim value As String = Helper.GetSafeXML(n, "value")
                            Dim key As String = n.Attributes("name").Value
                            obj.Resoruces.Add(New LocalizationResourceItem() With {.Key = key, .Value = value})
                        Next
                        TargetLocalFile = obj
                    Else
                        Throw New Exception("Could not load resource file " & fileName)
                    End If
                End If
            End Sub

            Private Shared Sub LoadTargetGlobalResourceFile(ByVal fileName As String, ByVal langId As Integer)
                Dim shouldLoad As Boolean = False
                Dim targetFileName As String = GetGlobalResourceFileDirectory(fileName, langId)
                If TargetGlobalFile.FileName = String.Empty OrElse TargetGlobalFile.FileName <> fileName OrElse TargetGlobalFile.LanguageId <> langId Then shouldLoad = True
                If shouldLoad Then
                    Dim obj As New LocalizationItem() With {
                        .FileName = targetFileName,
                        .LanguageId = langId,
                        .Resoruces = New List(Of LocalizationResourceItem)()
                    }
                    Dim loaded As Boolean = True
                    Dim doc As New XmlDocument()
                    Try
                        doc.Load(Helper.Server.MapPath(targetFileName))
                    Catch ex As Exception
                        loaded = False
                    End Try
                    If loaded Then
                        For Each n As XmlNode In (From x As XmlNode In doc.SelectNodes("root/data") Where x.NodeType <> XmlNodeType.Comment)
                            Dim value As String = Helper.GetSafeXML(n, "value")
                            Dim key As String = n.Attributes("name").Value
                            obj.Resoruces.Add(New LocalizationResourceItem() With {.Key = key, .Value = value})
                        Next
                        TargetGlobalFile = obj
                    Else
                        Throw New Exception("Could not load resource file " & fileName)
                    End If
                End If
            End Sub

            Private Shared Function GetLocalResourceFileDirectory(ByVal virtualPath As String, ByVal langId As Integer) As String
                Helper.GetSafeLanguageId(langId)
                Dim langCode As String = LanguageController.GetLanguageCode(langId)
                Dim fileName As String = virtualPath.Substring(virtualPath.LastIndexOf("/") + 1)
                Dim directory As String = virtualPath.Replace(fileName, "")
                Dim resourcePath As String = directory & "App_LocalResources/" & fileName & "." & langCode & ".resx"
                If IO.File.Exists(Helper.Server.MapPath(resourcePath)) Then
                    Return resourcePath
                Else
                    resourcePath = resourcePath.Replace("." & langCode, "")
                    If IO.File.Exists(Helper.Server.MapPath(resourcePath)) Then
                        Return resourcePath
                    Else
                        Throw New Exception("Local Resource File Not Found")
                    End If
                End If
            End Function

            Private Shared Function GetGlobalResourceFileDirectory(ByVal fileName As String, ByVal langId As Integer) As String
                Helper.GetSafeLanguageId(langId)
                Dim langcode As String = LanguageController.GetLanguageCode(langId)
                Dim resourcePath As String = "~/App_GlobalResources/" & fileName & "." & langcode & ".resx"
                If IO.File.Exists(Helper.Server.MapPath(resourcePath)) Then
                    Return resourcePath
                Else
                    resourcePath = resourcePath.Replace("." & langcode, "")
                    If IO.File.Exists(Helper.Server.MapPath(resourcePath)) Then
                        Return resourcePath
                    Else
                        Throw New Exception("Local Resource File Not Found")
                    End If
                End If
            End Function

            Private Shared Function GetDirectory(ByVal directory As String, Optional ByVal isGlobal As Boolean = False) As String
                Dim dir As String = "~"
                If isGlobal Then
                    dir &= "/App_GlobalResources/"
                Else
                    dir &= "/" & Helper.CMSPath() & directory & "/App_LocalResources/"
                End If
                dir = dir.Replace("//", "/")
                Return dir
            End Function

            Private Shared Function GetFile(ByVal fileName As String, ByVal directory As String,
                                            Optional ByVal languageCode As String = "",
                                            Optional ByVal isGlobal As Boolean = False,
                                            Optional ByVal strict As Boolean = True) As String
                fileName = fileName.Replace("_", "-")
                Dim file As String = directory & fileName & IIf(isGlobal, "", ".aspx") & IIf(languageCode <> String.Empty, "." & languageCode, "") & ".resx"
                If strict AndAlso Not IO.File.Exists(Helper.Server.MapPath(file)) Then file = file.Replace("." & languageCode, "")
                Return file
            End Function

#End Region

#Region "Public Methods"

            Public Shared Function GetResource(ByVal fullQualifiedResourceName As String) As String
                Helper.LoadCulture()
                Dim key As String = fullQualifiedResourceName.Replace("Resources.", "")
                Dim type As String = key.Substring(0, key.IndexOf("."))
                key = key.Replace(type & ".", "")
                If type.ToLower() = "local" Then
                    Dim vp As String = Helper.Page.AppRelativeVirtualPath
                    LoadTargetLocalResourceFile(vp, Helper.LanguageId)
                    Dim target = (From r In TargetLocalFile.Resoruces Where r.Key.ToLower() = key.ToLower())
                    If target.Count > 0 Then Return target.FirstOrDefault().Value Else Throw New Exception(String.Format("Resource ({0}) not found in local resouces file ({1}).", key, vp))
                ElseIf type.ToLower() = "global" Then
                    Dim vp As String = key.Substring(0, key.IndexOf("."))
                    key = key.Replace(vp & ".", "")
                    LoadTargetGlobalResourceFile(vp, Helper.LanguageId)
                    Dim target = (From r In TargetGlobalFile.Resoruces Where r.Key.ToLower() = key.ToLower())
                    If target.Count > 0 Then Return target.FirstOrDefault().Value Else Throw New Exception(String.Format("Resource ({0}) not found in local resouces file ({1}).", key, vp))
                Else
                    Throw New Exception(String.Format("Resource type not specified. ({0})", fullQualifiedResourceName))
                End If
            End Function

            Public Shared Function LoadResourceFile(ByVal fileName As String, ByVal filePath As String, ByVal languageCode As String, Optional ByVal isGlobal As Boolean = False) As Hashtable
                Dim dir As String = GetDirectory(filePath, isGlobal)
                Dim ret As New Hashtable()
                If IO.Directory.Exists(Helper.Server.MapPath(dir)) Then
                    Dim file As String = GetFile(fileName, dir, languageCode, isGlobal)
                    Dim sfile As String = Helper.Server.MapPath(file)
                    If IO.File.Exists(sfile) Then
                        Dim d As New XmlDocument()
                        Dim loaded As Boolean = True
                        Try
                            d.Load(sfile)
                        Catch ex As Exception
                            loaded = False
                        End Try
                        If loaded Then
                            For Each n As XmlNode In (From x As XmlNode In d.SelectNodes("root/data") Where x.NodeType <> XmlNodeType.Comment)
                                Dim value As String = Helper.GetSafeXML(n, "value")
                                Dim key As String = n.Attributes("name").Value
                                If Not ret.ContainsKey(key) Then
                                    ret.Add(key, value)
                                End If
                            Next
                        End If
                    End If
                End If
                Return ret
            End Function

            Public Shared Function ResourceExists(ByVal filePath As String, ByVal key As String) As Boolean
                Dim ret As Boolean = False
                Dim file As String = Helper.Server.MapPath(filePath)
                If IO.File.Exists(file) Then
                    Dim doc As New XmlDocument()
                    Dim loaded As Boolean = True
                    Try
                        doc.Load(file)
                    Catch ex As Exception
                        loaded = False
                    End Try
                    If loaded Then
                        ret = doc.SelectNodes("root/data[@name='" & key & "']").Count > 0
                    End If
                End If
                Return ret
            End Function

            Public Shared Function SaveResource(ByVal fileName As String, ByVal directory As String,
                                                ByVal key As String, ByVal value As String,
                                                ByVal languageCode As String, Optional ByVal isGlobal As Boolean = False) As Boolean
                Dim dir As String = GetDirectory(directory, isGlobal)
                Dim filePath As String = GetFile(fileName, dir, languageCode, isGlobal, False)
                Dim file As String = Helper.Server.MapPath(filePath)
                Dim ret As Boolean = False
                If Not IO.File.Exists(file) Then
                    file = file.Replace("." & languageCode, "")
                    filePath = filePath.Replace("." & languageCode, "")
                End If
                If IO.File.Exists(file) Then
                    If ResourceExists(filePath, key) Then
                        Dim doc As New XmlDocument()
                        Dim loaded As Boolean = True
                        Try
                            doc.Load(file)
                        Catch ex As Exception
                            loaded = False
                        End Try
                        If loaded Then
                            Dim n As XmlNode = doc.SelectSingleNode("root/data[@name='" & key & "']/value")
                            If n IsNot Nothing Then
                                n.InnerText = value
                                doc.Save(file)
                                ret = True
                            End If
                        End If
                    End If
                End If
                Return ret
            End Function

#End Region

        End Class
    End Namespace
End Namespace
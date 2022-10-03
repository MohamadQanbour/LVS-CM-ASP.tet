Imports System.Data.SqlClient
Imports System.IO
Imports EGV.Utils
Imports EGV.Enums
Imports System.Web

Namespace Ajax

    Public Class AssetsManager
        Inherits AjaxBaseClass

#Region "Request Values"

        Public ReadOnly Property RootPath As String
            Get
                Dim path As String = GetSafeRequestValue("folder")
                Dim assets As String = Helper.AssetsPath()
                Return path.Replace("/" & assets & "/", "").Replace(assets & "/", "").Replace("/" & assets, "")
            End Get
        End Property
        Public Property IsCMS As Boolean = GetSafeRequestValue("cms", ValueTypes.TypeBoolean)
        Public Property SearchTerm As String = GetSafeRequestValue("q")
        Public Property SortColumn As String = GetSafeRequestValue("col")
        Public Property SortOrder As String = GetSafeRequestValue("ord")
        Public Property PageIndex As Integer = GetSafeRequestValue("pageindex", ValueTypes.TypeInteger)
        Public Property Name As String = GetSafeRequestValue("name")
        Public Property NewName As String = GetSafeRequestValue("newname")
        Public Property UploadPath As String = GetSafeRequestValue("UploadFilePath")

#End Region

#Region "Overridden Methods"

        Public Overrides Function Load(tFunc As String, Optional p As HttpApplication = Nothing) As String
            If Helper.Request.Files.Count > 0 Then
                Return SaveFile(UploadPath)
            Else
                SetTargetFunction(tFunc)
                SetMyPage(p)
                Dim retObj As New ReturnObject() With {.HasError = False, .ErrorMessage = String.Empty, .ReturnData = Nothing}
                Try
                    MyConn.Open()
                    retObj.ReturnData = ProcessAjaxRequest(MyConn, LanguageId)
                Catch ex As Exception
                    retObj.HasError = True
                    retObj.ErrorMessage = ex.Message & IIf(Helper.DebuggingEnabled(), " : " & ex.StackTrace, "")
                Finally
                    MyConn.Close()
                End Try
                Return "[" & Helper.JSSerialize(retObj) & "]"
            End If
        End Function

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            If Helper.CMSAuthUser IsNot Nothing Then
                Dim ret As Object = Nothing
                Select Case TargetFunction
                    Case "FolderList"
                        ret = GetFolderList()
                    Case "FileList"
                        ret = GetFileList()
                    Case "AddFolder"
                        AddFolder()
                    Case "DeleteFolder"
                        DeleteFolder()
                    Case "DeleteFile"
                        DeleteFile()
                    Case "RenameFolder"
                        RenameFolder()
                    Case "RenameFile"
                        RenameFile()
                    Case "FolderCount"
                        ret = GetTotalFolderContents()
                End Select
                Return ret
            Else
                Throw New Exception("Access Denied")
            End If
        End Function

#End Region

#Region "Private Methods"

        Private Function GetFolderList() As List(Of Folder)
            Dim RootFolder As String = RootPath
            If RootFolder.EndsWith("/") Then RootFolder = RootFolder.Substring(0, RootFolder.LastIndexOf("/"))
            Dim assetsPath As String = Helper.AssetsPath()
            Dim cmsPath As String = Helper.CMSPath()
            Dim targetPath As String = "~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & RootFolder
            Dim lst As New List(Of Folder)
            For Each sdr As DirectoryInfo In New DirectoryInfo(Helper.Server.MapPath(targetPath)).GetDirectories()
                lst.Add(New Folder() With {
                    .Name = sdr.Name,
                    .Path = RootFolder & "/" & sdr.Name
                })
            Next
            Return lst
        End Function

        Private Function GetFileList() As List(Of FileFolder)
            Dim RootFolder As String = RootPath
            If RootFolder.EndsWith("/") Then RootFolder = RootFolder.Substring(0, RootFolder.LastIndexOf("/"))
            If RootFolder.StartsWith("/") Then RootFolder = RootFolder.Substring(1)
            Dim assetsPath As String = Helper.AssetsPath()
            Dim cmsPath As String = Helper.CMSPath()
            Dim targetPath As String = "~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & RootFolder
            Dim lst As New List(Of FileFolder)
            Dim dir As New DirectoryInfo(Helper.Server.MapPath(targetPath))
            If dir.Exists Then
                Dim dirs() As DirectoryInfo = dir.GetDirectories()
                If SortColumn <> String.Empty AndAlso SortOrder <> String.Empty Then
                    Array.Sort(dirs, GetSortingFunction(SortColumn, SortOrder))
                End If
                For Each item As DirectoryInfo In (From d In dirs Where SearchTerm = String.Empty OrElse d.Name.ToLower().Contains(SearchTerm.ToLower()))
                    lst.Add(New FileFolder() With {
                        .Name = item.Name,
                        .Path = RootFolder & "/" & item.Name,
                        .Type = FileFolderTypes.Folder,
                        .FileDate = String.Empty,
                        .FileSize = String.Empty
                    })
                Next
                Dim files() As FileInfo = dir.GetFiles()
                If SortColumn.Length > 0 AndAlso SortOrder.Length > 0 Then
                    Array.Sort(files, GetSortingFunction(SortColumn, SortOrder))
                End If
                For Each item As FileInfo In (From f In files Where SearchTerm = String.Empty OrElse f.Name.ToLower().Contains(SearchTerm.ToLower()))
                    lst.Add(New FileFolder() With {
                        .Name = item.Name,
                        .Path = RootFolder & "/" & item.Name,
                        .Type = FileFolderTypes.File,
                        .FileDate = item.CreationTime.ToString("MMMM dd, yyyy"),
                        .FileSize = FormatFileSize(item.Length)
                    })
                Next
                Dim pagesize As Integer = 50
                If lst.Count > pagesize Then
                    Dim total = lst.Count
                    Dim start As Integer = 0
                    Dim limit As Integer = 0
                    Dim numOfPages = Math.Ceiling(CDec(total) / CDec(pagesize))
                    If PageIndex * pagesize < total Then
                        start = PageIndex * pagesize
                        limit = IIf(total - start > pagesize, pagesize, total - start)
                        lst = lst.GetRange(start, limit)
                    End If
                End If
                Return lst
            Else
                Return New List(Of FileFolder)()
            End If
        End Function

        Private Sub AddFolder()
            Dim RootFolder As String = RootPath
            Name = EscapeName(Name)
            If RootFolder.EndsWith("/") Then RootFolder = RootFolder.Substring(0, RootFolder.LastIndexOf("/"))
            If RootFolder.StartsWith("/") Then RootFolder = RootFolder.Substring(1)
            Dim assetsPath As String = Helper.AssetsPath()
            Dim cmsPath As String = Helper.CMSPath()
            Dim targetPath As String = "~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & RootFolder & "/" & Name
            Dim fullPath As String = Helper.Server.MapPath(targetPath)
            If Not Directory.Exists(fullPath) Then
                Directory.CreateDirectory(fullPath)
            End If
        End Sub

        Private Sub DeleteFolder()
            Dim RootFolder As String = RootPath
            RootFolder = RootFolder.Replace(Name, "")
            If RootFolder.EndsWith("/") Then RootFolder = RootFolder.Substring(0, RootFolder.LastIndexOf("/"))
            If RootFolder.StartsWith("/") Then RootFolder = RootFolder.Substring(1)
            Dim assetsPath As String = Helper.AssetsPath()
            Dim cmsPath As String = Helper.CMSPath()
            Dim targetPath As String = "~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & RootFolder & "/" & Name
            Dim fullPath As String = Helper.Server.MapPath(targetPath)
            If Directory.Exists(fullPath) Then
                Try
                    Directory.Delete(fullPath)
                Catch
                End Try
            End If
        End Sub

        Private Sub DeleteFile()
            Dim RootFolder As String = RootPath
            If RootFolder.EndsWith("/") Then RootFolder = RootFolder.Substring(0, RootFolder.LastIndexOf("/"))
            If RootFolder.StartsWith("/") Then RootFolder = RootFolder.Substring(1)
            Dim assetsPath As String = Helper.AssetsPath()
            Dim cmsPath As String = Helper.CMSPath()
            Dim targetPath As String = "~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & RootFolder & "/" & Name
            targetPath = Helper.Server.MapPath(targetPath)
            If File.Exists(targetPath) Then
                Try
                    File.Delete(targetPath)
                Catch
                End Try
            End If
        End Sub

        Private Sub RenameFolder()
            Dim RootFolder As String = RootPath
            RootFolder = RootFolder.Replace(Name, "")
            If RootFolder.EndsWith("/") Then RootFolder = RootFolder.Substring(0, RootFolder.LastIndexOf("/"))
            If RootFolder.StartsWith("/") Then RootFolder = RootFolder.Substring(1)
            Dim assetsPath As String = Helper.AssetsPath()
            Dim cmsPath As String = Helper.CMSPath()
            Dim targetPath As String = "~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & RootFolder & "/" & Name
            targetPath = Helper.Server.MapPath(targetPath)
            If Directory.Exists(targetPath) Then
                NewName = EscapeName(NewName)
                Dim newTarget = Helper.Server.MapPath("~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & RootFolder & "/" & NewName)
                If Not Directory.Exists(newTarget) Then
                    Directory.Move(targetPath, newTarget)
                End If
            End If
        End Sub

        Private Sub RenameFile()
            Dim RootFolder As String = RootPath
            If RootFolder.EndsWith("/") Then RootFolder = RootFolder.Substring(0, RootFolder.LastIndexOf("/"))
            If RootFolder.StartsWith("/") Then RootFolder = RootFolder.Substring(1)
            Dim assetsPath As String = Helper.AssetsPath()
            Dim cmsPath As String = Helper.CMSPath()
            Dim targetPath As String = "~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & RootFolder & "/" & Name
            targetPath = Helper.Server.MapPath(targetPath)
            If File.Exists(targetPath) Then
                Dim ext = NewName.Substring(NewName.LastIndexOf("."))
                NewName = EscapeName(NewName.Replace(ext, ""))
                Dim newTarget = Helper.Server.MapPath("~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & RootFolder & "/" & NewName & ext)
                If Not File.Exists(newTarget) Then
                    File.Move(targetPath, newTarget)
                End If
            End If
        End Sub

        Private Function GetTotalFolderContents() As Integer
            Dim RootFolder As String = RootPath
            If RootFolder.EndsWith("/") Then RootFolder = RootFolder.Substring(0, RootFolder.LastIndexOf("/"))
            If RootFolder.StartsWith("/") Then RootFolder = RootFolder.Substring(1)
            Dim assetsPath As String = Helper.AssetsPath()
            Dim cmsPath As String = Helper.CMSPath()
            Dim targetPath As String = "~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & RootFolder
            Dim count As Integer = 0
            Dim dir As New DirectoryInfo(Helper.Server.MapPath(targetPath))
            If dir.Exists Then
                count += (From d In dir.GetDirectories() Where SearchTerm = String.Empty OrElse d.Name.ToLower().Contains(SearchTerm.ToLower())).Count()
                count += (From f In dir.GetFiles() Where SearchTerm = String.Empty OrElse f.Name.ToLower().Contains(SearchTerm.ToLower())).Count()
            End If
            Return count
        End Function

        Public Function SaveFile(ByVal path As String) As String
            If Helper.CMSAuthUser IsNot Nothing Then
                Dim assetsPath As String = Helper.AssetsPath()
                path = path.Replace("/" & assetsPath & "/", "").Replace("/" & assetsPath, "").Replace(assetsPath & "/", "")
                If path.EndsWith("/") Then path = path.Substring(0, path.LastIndexOf("/"))
                If path.StartsWith("/") Then path = path.Substring(1)
                Dim files As HttpFileCollection = Helper.Request.Files
                Dim cmsPath As String = Helper.CMSPath()
                Dim directory As String = "~" & IIf(IsCMS, "/" & cmsPath, "") & "/" & assetsPath & "/" & path
                Dim tpath As String = Helper.Server.MapPath(directory)
                If Not System.IO.Directory.Exists(tpath) Then
                    System.IO.Directory.CreateDirectory(tpath)
                End If

                Dim ret As New List(Of UploadedFile)
                For i As Integer = 0 To files.Count - 1
                    Dim f As HttpPostedFile = files(i)
                    Try
                        If f.FileName <> String.Empty Then
                            Dim ext As String = f.FileName.Substring(f.FileName.LastIndexOf("."))
                            Dim newName = EscapeName(f.FileName.Replace(ext, ""))
                            Dim newFileName As String = newName & ext
                            Dim fPath As String = directory & "/" & newFileName
                            f.SaveAs(Helper.Server.MapPath(fPath))
                            ret.Add(New UploadedFile() With {
                                .name = fPath.Replace("~", ""),
                                .size = f.ContentLength,
                                .url = fPath.Replace("~", ""),
                                .thumbnailUrl = fPath.Replace("~", ""),
                                .deleteUrl = fPath.Replace("~", ""),
                                .deleteType = "DELETE"
                            })
                        Else
                            Throw New Exception("File is empty...")
                        End If
                    Catch ex As Exception
                        ret.Add(New UploadedFile() With {
                            .name = f.FileName,
                            .size = f.ContentLength,
                            .error = ex.Message
                        })
                    End Try
                Next
                Helper.Response.Clear()
                Helper.Response.ContentType = "application/json"
                Return Helper.JSSerialize(New UploadResponse() With {.files = ret})
            Else
                Return Helper.JSSerialize(New UploadResponse())
            End If
        End Function

#End Region

#Region "Structures"

        Protected Enum FileFolderTypes
            Folder = 1
            File = 2
        End Enum

        Protected Structure Folder
            Public Property Name As String
            Public Property Path As String
        End Structure

        Protected Structure FileFolder
            Public Property Name As String
            Public Property Path As String
            Public Property Type As FileFolderTypes
            Public Property FileDate As String
            Public Property FileSize As String
        End Structure

        Protected Structure UploadedFile
            Public Property name As String
            Public Property size As Integer
            Public Property url As String
            Public Property thumbnailUrl As String
            Public Property deleteUrl As String
            Public Property deleteType As String
            Public Property [error] As String
        End Structure

        Protected Structure UploadResponse
            Public Property files As List(Of UploadedFile)
        End Structure

#End Region

#Region "Helping Methods"

        Public Shared Function FormatFileSize(ByVal size As Long) As String
            If size < 1024 Then
                Return Math.Round(size, 2, MidpointRounding.AwayFromZero) & " b"
            ElseIf size < (1024 * 1024) Then
                Return Math.Round((size / 1024), 2, MidpointRounding.AwayFromZero) & " kb"
            ElseIf size < (1024 * 1024 * 1024) Then
                Return Math.Round((size / (1024 * 1024)), 2, MidpointRounding.AwayFromZero) & " mb"
            Else
                Return Math.Round((size / (1024 * 1024 * 1024)), 2, MidpointRounding.AwayFromZero) & " gb"
            End If
        End Function

        Private Function GetSortingFunction(ByVal sortColumn As String, ByVal sortOrder As String) As IComparer
            sortColumn = sortColumn.ToLower()
            sortOrder = sortOrder.ToLower()
            Select Case sortColumn
                Case "date"
                    If sortOrder = "asc" Then
                        Return New IODateComparer()
                    Else
                        Return New IOReverseDateComparer()
                    End If
                Case "size"
                    If sortOrder = "asc" Then
                        Return New IOSizeComparer()
                    Else
                        Return New IOReverseSizeComparer()
                    End If
                Case Else
                    If sortOrder = "asc" Then
                        Return New IONameComparer()
                    Else
                        Return New IOReverseNameComparer()
                    End If
            End Select
        End Function

        Public Function EscapeName(ByVal source As String) As String
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
            Return ret
        End Function

#End Region

#Region "Comparers"

        Public Class IONameComparer
            Implements IComparer

            Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
                If TypeOf x Is DirectoryInfo Then
                    Dim objX As DirectoryInfo = x
                    Dim objY As DirectoryInfo = y
                    Return New CaseInsensitiveComparer().Compare(objX.Name, objY.Name)
                Else
                    Dim objX As FileInfo = x
                    Dim objY As FileInfo = y
                    Return New CaseInsensitiveComparer().Compare(objX.Name, objY.Name)
                End If
            End Function

        End Class

        Public Class IODateComparer
            Implements IComparer

            Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
                If TypeOf x Is DirectoryInfo Then
                    Dim objX As DirectoryInfo = x
                    Dim objY As DirectoryInfo = y
                    Return New CaseInsensitiveComparer().Compare(objX.Name, objY.Name)
                Else
                    Dim objX As FileInfo = x
                    Dim objY As FileInfo = y
                    Return Date.Compare(objX.CreationTime, objY.CreationTime)
                End If
            End Function

        End Class

        Public Class IOSizeComparer
            Implements IComparer

            Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
                If TypeOf x Is DirectoryInfo Then
                    Dim objX As DirectoryInfo = x
                    Dim objY As DirectoryInfo = y
                    Return New CaseInsensitiveComparer().Compare(objX.Name, objY.Name)
                Else
                    Dim objX As FileInfo = x
                    Dim objY As FileInfo = y
                    If objX.Length > objY.Length Then
                        Return 1
                    End If
                    If objX.Length < objY.Length Then
                        Return -1
                    Else
                        Return 0
                    End If
                End If
            End Function

        End Class

        Public Class IOReverseNameComparer
            Implements IComparer

            Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
                If TypeOf x Is DirectoryInfo Then
                    Dim objX As DirectoryInfo = x
                    Dim objY As DirectoryInfo = y
                    Return New CaseInsensitiveComparer().Compare(objY.Name, objX.Name)
                Else
                    Dim objX As FileInfo = x
                    Dim objY As FileInfo = y
                    Return New CaseInsensitiveComparer().Compare(objY.Name, objX.Name)
                End If
            End Function

        End Class

        Public Class IOReverseDateComparer
            Implements IComparer

            Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
                If TypeOf x Is DirectoryInfo Then
                    Dim objX As DirectoryInfo = x
                    Dim objY As DirectoryInfo = y
                    Return New CaseInsensitiveComparer().Compare(objX.Name, objY.Name)
                Else
                    Dim objX As FileInfo = x
                    Dim objY As FileInfo = y
                    Return Date.Compare(objY.CreationTime, objX.CreationTime)
                End If
            End Function

        End Class

        Public Class IOReverseSizeComparer
            Implements IComparer

            Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
                If TypeOf x Is DirectoryInfo Then
                    Dim objX As DirectoryInfo = x
                    Dim objY As DirectoryInfo = y
                    Return New CaseInsensitiveComparer().Compare(objX.Name, objY.Name)
                Else
                    Dim objX As FileInfo = x
                    Dim objY As FileInfo = y
                    If objY.Length > objX.Length Then
                        Return 1
                    End If
                    If objY.Length < objX.Length Then
                        Return -1
                    Else
                        Return 0
                    End If
                End If
            End Function

        End Class

#End Region

    End Class

End Namespace
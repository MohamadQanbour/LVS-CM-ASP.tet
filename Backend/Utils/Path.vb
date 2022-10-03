Namespace EGV
    Namespace Utils
        Public Class Path

#Region "CMS"

            Public Shared Function MapCMSFile(ByVal fileName As String) As String
                Return "/" & Helper.CMSPath() & "/" & fileName
            End Function

            Public Shared Function MapStructuresFile(ByVal filename As String) As String
                Return MapCMSFile(Helper.StructuresPath() & "/" & filename)
            End Function

            Public Shared Function MapCMSCss(ByVal filename As String) As String
                If Not filename.EndsWith(".css") Then filename &= ".css"
                Return MapCMSFile("css/" & filename)
            End Function

            Public Shared Function MapCMSScript(ByVal filename As String) As String
                If Not filename.EndsWith(".js") Then filename &= ".js"
                Return MapCMSFile("js/" & filename)
            End Function

            Public Shared Function MapCMSImage(ByVal filename As String) As String
                Return MapCMSFile("images/" & filename)
            End Function

            Public Shared Function MapCMSAsset(ByVal filename As String) As String
                Return MapCMSFile(Helper.AssetsPath() & "/" & filename)
            End Function

#End Region

#Region "Portal"

            Public Shared Function MapPortalFile(ByVal filename As String) As String
                Return "/" & filename
            End Function

            Public Shared Function MapPortalAsset(ByVal filename As String) As String
                Return MapPortalFile(Helper.AssetsPath() & "/" & filename)
            End Function

            Public Shared Function MapPortalScript(ByVal filename As String) As String
                If Not filename.EndsWith(".js") Then filename &= ".js"
                Return MapPortalFile("js/" & filename)
            End Function

            Public Shared Function MapPortalImage(ByVal filename As String) As String
                Return MapPortalFile("images/" & filename)
            End Function

            Public Shared Function MapPortalStyle(ByVal filename As String) As String
                If Not filename.EndsWith(".css") Then filename &= ".css"
                Return MapPortalFile("css/" & filename)
            End Function

#End Region

        End Class
    End Namespace
End Namespace
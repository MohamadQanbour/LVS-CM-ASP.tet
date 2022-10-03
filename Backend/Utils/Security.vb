Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text

Namespace EGV
    Namespace Utils

        Public Class Security

#Region "Public Methods"

            Public Shared Function IsSecure(ByVal securityKey As String) As Boolean
                Dim valid As Boolean = False
                If securityKey <> String.Empty Then
                    Dim privateKey As String = Helper.PrivateKey()
                    Dim tData As New SortedList(New APCStringComparer)
                    Dim queryCheck As String = Helper.GetSafeObject(Helper.Request.Form("query"), Enums.ValueTypes.TypeString)
                    If privateKey <> String.Empty AndAlso queryCheck <> String.Empty Then
                        For Each item In Helper.Request.QueryString
                            If item IsNot Nothing AndAlso item <> String.Empty AndAlso Not item.Equals("security_key") Then
                                Dim str = Helper.Request.QueryString(item).ToString().Replace(" ", "+")
                                str = str.Replace("%20", "+")
                                tData.Add(item, str)
                            End If
                        Next
                        tData.Add("query", queryCheck)
                        Dim rawHash As String = privateKey
                        For Each item As DictionaryEntry In tData
                            rawHash &= item.Value.ToString().ToLower()
                        Next
                        Dim signature As String = HashString(rawHash)
                        valid = securityKey.Contains(signature)
                    End If
                End If
                Return valid
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function HashString(ByVal str As String) As String
                Dim strHash As String = String.Empty
                Dim md5 As New MD5CryptoServiceProvider()
                Dim arr(), arrHashed() As Byte
                arr = UTF8Encoding.UTF8.GetBytes(str)
                arrHashed = md5.ComputeHash(arr)
                For Each b As Byte In arrHashed
                    strHash &= b.ToString("x2")
                Next
                Return strHash.ToLower()
            End Function

#End Region

        End Class

        Public Class APCStringComparer
            Implements IComparer

            Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
                If x = y Then Return 0
                If x Is Nothing Then Return -1
                If y Is Nothing Then Return 1
                Dim sx As String = TryCast(x, String)
                Dim sy As String = TryCast(y, String)
                Dim myComparer As CompareInfo = CompareInfo.GetCompareInfo("en-US")
                If sx <> Nothing AndAlso sy <> String.Empty Then Return myComparer.Compare(sx, sy, CompareOptions.Ordinal)
                Throw New Exception("x and y should be strings.")
            End Function

        End Class

    End Namespace
End Namespace
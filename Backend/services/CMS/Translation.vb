Imports System.Web
Imports System.Data.SqlClient
Imports System.Xml
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Enums
Imports EGV.Interfaces
Imports System.Reflection
Imports System.Runtime.Serialization
Imports EGV

Namespace Ajax

    Public Class Translation
        Inherits AjaxBaseClass

#Region "Request Values"

        Public ReadOnly Property UserId As Integer = GetSafeRequestValue("userid", ValueTypes.TypeInteger)
        Public ReadOnly Property Type As TranslationTypes = GetSafeRequestValue("type", ValueTypes.TypeInteger)
        Public ReadOnly Property File As String = GetSafeRequestValue("file")
        Public ReadOnly Property TargetLanguageId As Integer = GetSafeRequestValue("lang", ValueTypes.TypeInteger)
        Public ReadOnly Property TargetId As Integer = GetSafeRequestValue("id", ValueTypes.TypeInteger)
        Public ReadOnly Property Translation As String = GetSafeRequestValue("value")
        Public ReadOnly Property Key As String = GetSafeRequestValue("key")
        Public ReadOnly Property TargetLanguageCode As String = GetSafeRequestValue("langcode")

#End Region

#Region "Overridden Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "Translate"
                    Select Case Type
                        Case TranslationTypes.Dynamic
                            ret = Translate(MyConn)
                        Case Else
                            ret = UpdateResource()
                    End Select
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function UpdateResource() As Boolean
            'File, Key, Translation, TargetLanguageCode
            Dim statics As List(Of StaticControllerItem) = LoadStaticInfo()
            Dim target = (From n In statics Where n.File = File)
            If target.Count > 0 Then
                Dim obj = target.FirstOrDefault()
                Dim dir As String = obj.Directory
                Dim isGlobal As Boolean = obj.IsGlobal
                Return Localization.SaveResource(File, dir, Key, HttpUtility.HtmlDecode(Translation), TargetLanguageCode, isGlobal)
            Else
                Return False
            End If
        End Function

        Private Function Translate(ByVal conn As SqlConnection) As Boolean
            'UserId, File, TargetLanguageId, TargetId, Translation
            Dim dynamics As List(Of DynamicControllerItem) = LoadDynamicController()
            Dim target = (From a In dynamics Where a.Id = File)
            If target.Count > 0 Then
                Dim activatorName As String = target.FirstOrDefault().Controller
                Dim activatorType As Type = System.Type.GetType("EGV.Business." & activatorName)
                If activatorType IsNot Nothing Then
                    Dim instance As ILocBusinessClass = DirectCast(FormatterServices.GetUninitializedObject(activatorType), ILocBusinessClass)
                    instance.Id = TargetId
                    instance.Title = Translation
                    Dim trans As SqlTransaction = conn.BeginTransaction()
                    Try
                        instance.Translate(TargetLanguageId, UserId, trans)
                        trans.Commit()
                    Catch ex As Exception
                        trans.Rollback()
                        Throw ex
                    End Try
                End If
            End If
            Return True
        End Function

#End Region

#Region "Helper Methods"

        Private Function LoadDynamicController() As List(Of DynamicControllerItem)
            Dim lst As New List(Of DynamicControllerItem)
            Dim file As String = "~" & Path.MapCMSFile(Helper.StructuresPath() & "/localization/Dynamic.xml")
            Dim doc As New XmlDocument()
            Dim loaded As Boolean = True
            Try
                doc.Load(Helper.Server.MapPath(file))
            Catch ex As Exception
                loaded = False
            End Try
            If loaded Then
                For Each n As XmlNode In doc.SelectNodes("List/Group/Items/Item")
                    lst.Add(New DynamicControllerItem() With {
                        .Id = Helper.GetSafeXML(n, "Id"),
                        .Controller = Helper.GetSafeXML(n, "Controller")
                    })
                Next
            End If
            Return lst
        End Function

        Private Function LoadStaticInfo() As List(Of StaticControllerItem)
            Dim lst As New List(Of StaticControllerItem)
            Dim file As String = "~" & Path.MapCMSFile(Helper.StructuresPath() & "/localization/Static.xml")
            Dim doc As New XmlDocument()
            Dim loaded As Boolean = True
            Try
                doc.Load(Helper.Server.MapPath(file))
            Catch ex As Exception
                loaded = False
            End Try
            If loaded Then
                For Each n As XmlNode In doc.SelectNodes("List/Group")
                    Dim isGlobal As Boolean = Helper.GetSafeXML(n, "IsGlobal", ValueTypes.TypeBoolean)
                    Dim dir As String = Helper.GetSafeXML(n, "Directory")
                    For Each i As XmlNode In n.SelectNodes("Items/Item")
                        lst.Add(New StaticControllerItem() With {
                            .Directory = dir,
                            .IsGlobal = isGlobal,
                            .File = i.InnerText
                        })
                    Next
                Next
            End If
            Return lst
        End Function

#End Region

    End Class

End Namespace
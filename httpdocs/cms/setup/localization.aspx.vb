Imports EGV
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures
Imports System.Data.SqlClient
Imports System.Data
Imports System.Xml
Imports EGV.Business

Partial Class cms_setup_localization
    Inherits AuthCMSPageBase

#Region "Public Properties"

    Public Property DynamicList As List(Of DynamicGroupItem)
    Public Property StaticList As List(Of StaticGroupItem)

#End Region

#Region "Event Handlers"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        DynamicList = LoadDynamics()
        StaticList = LoadStatics()
        hdnUserId.Value = AuthUser.Id
        If Not Page.IsPostBack Then
            ProcessCMD(Master.Notifier)
            Try
                MyConn.Open()
                ProcessPermissions(AuthUser, PageId, MyConn)
                LoadDynamicList()
                LoadStaticList()
                LoadDynamicData(hdnFile.Value, MyConn)
                EGVScriptManager.AddScript(Path.MapCMSScript("local/localization"))
            Catch ex As Exception
                ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
            Finally
                MyConn.Close()
            End Try
        End If
    End Sub

    Public Function GetLanguageCode(ByVal langId As Integer, Optional ByVal conn As SqlConnection = Nothing) As String
        Dim q As String = "SELECT Title FROM LOK_Language_Res WHERE Id = @Id AND LanguageId = @LanguageId"
        Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, langId), DBA.CreateParameter("LanguageId", SqlDbType.Int, LanguageId))
    End Function

    Public Function GetLanguageTitleByCode(ByVal langCode As String, Optional ByVal conn As SqlConnection = Nothing) As String
        Dim q As String = "SELECT R.Title FROM LOK_Language L INNER JOIN LOK_Language_Res R ON L.Id = R.Id AND R.LanguageId = @LanguageId WHERE L.LanguageCode = @Code"
        Return DBA.Scalar(conn, q, DBA.CreateParameter("LanguageId", SqlDbType.Int, LanguageId), DBA.CreateParameter("Code", SqlDbType.Char, langCode, 5))
    End Function

    Public Function GetRowSpan(Optional ByVal conn As SqlConnection = Nothing) As Integer
        Dim q As String = "SELECT COUNT(*) FROM LOK_Language"
        Return DBA.Scalar(conn, q)
    End Function

    Protected Sub lnkItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim file As String = DirectCast(sender, LinkButton).Attributes("data-item")
        hdnFile.Value = file
        Try
            MyConn.Open()
            LoadDynamicData(file, MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub lnkFile_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim directory As String = DirectCast(sender, LinkButton).Attributes("data-directory")
        Dim filename As String = DirectCast(sender, LinkButton).Attributes("data-item")
        Dim isGlobalStr As String = DirectCast(sender, LinkButton).Attributes("data-isglobal")
        Dim isGlobal As Boolean = IIf(isGlobalStr.ToLower() = "true", True, False)
        hdnFile.Value = filename
        Try
            MyConn.Open()
            LoadStaticData(filename, directory, isGlobal, MyConn)
        Catch ex As Exception
            ExceptionHandler.ProcessException(ex, Master.Notifier, MyConn)
        Finally
            MyConn.Close()
        End Try
    End Sub

    Protected Sub rptDynamic_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptDynamic.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As DynamicGroupItem = e.Item.DataItem
            If item.Items.Count > 0 Then
                Dim rpt As Repeater = e.Item.FindControl("rptItems")
                rpt.DataSource = item.Items
                rpt.DataBind()
            End If
        End If
    End Sub

    Protected Sub rptStatic_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptStatic.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As StaticGroupItem = e.Item.DataItem
            If item.Items.Count > 0 Then
                Dim rpt As Repeater = e.Item.FindControl("rptItems")
                rpt.DataSource = item.Items
                rpt.DataBind()
            End If
        End If
    End Sub

    Protected Sub rptDynamicFile_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptDynamicItems.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As DynamicFileItem = e.Item.DataItem
            If item.Translations.Count > 0 Then
                Dim rpt As Repeater = e.Item.FindControl("rptTranslations")
                rpt.DataSource = item.Translations
                rpt.DataBind()
            End If
        End If
    End Sub

    Protected Sub rptStaticFile_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptStaticItems.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As StaticFileItem = e.Item.DataItem
            If item.Translations.Count > 0 Then
                Dim rpt As Repeater = e.Item.FindControl("rptTranslations")
                rpt.DataSource = item.Translations
                rpt.DataBind()
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub LoadDynamicData(ByVal file As String, Optional ByVal conn As SqlConnection = Nothing)
        litType.Text = GetTypeTitle()
        litFile.Text = GetFileTitle(file)
        pnlDynamic.Visible = True
        pnlStatic.Visible = False
        rptDynamicItems.DataSource = LoadDynamicFile(file, conn)
        rptDynamicItems.DataBind()
        hdnTypeId.Value = TranslationTypes.Dynamic
    End Sub

    Private Sub LoadStaticData(ByVal file As String, ByVal directory As String, ByVal isGlobal As Boolean, Optional ByVal conn As SqlConnection = Nothing)
        litType.Text = GetTypeTitle()
        litFile.Text = GetFileTitle(file)
        pnlDynamic.Visible = False
        pnlStatic.Visible = True
        Dim lst As New List(Of StaticFileItem)
        Dim defaultLanguageId As Integer = LanguageController.GetDefaultId(conn)
        Dim defaultLanguageCode As String = LanguageController.GetLanguageCode(defaultLanguageId, conn)
        For Each item As DictionaryEntry In Localization.LoadResourceFile(file, directory, defaultLanguageCode, isGlobal)
            Dim l As New StaticFileItem() With {
                .Key = item.Key
            }
            l.Translations = New List(Of StaticFileTranslationItem)()
            l.Translations.Add(New StaticFileTranslationItem() With {
                .LanguageCode = defaultLanguageCode,
                .Text = HttpUtility.HtmlEncode(item.Value),
                .Key = item.Key
            })
            lst.Add(l)
        Next
        For Each lid As Integer In LanguageController.GetIds(conn)
            If lid <> defaultLanguageId Then
                Dim code As String = LanguageController.GetLanguageCode(lid, conn)
                For Each item As DictionaryEntry In Localization.LoadResourceFile(file, directory, code, isGlobal)
                    Dim target = (From l In lst Where l.Key = item.Key)
                    If target.Count = 1 Then
                        target.FirstOrDefault().Translations.Add(New StaticFileTranslationItem() With {
                            .LanguageCode = code,
                            .Text = HttpUtility.HtmlEncode(item.Value),
                            .Key = item.Key
                        })
                    End If
                Next
            End If
        Next
        rptStaticItems.DataSource = lst.OrderBy(Function(x) x.Key).ToList()
        rptStaticItems.DataBind()
        hdnTypeId.Value = TranslationTypes.Static
    End Sub

    Private Sub LoadDynamicList()
        rptDynamic.DataSource = DynamicList
        rptDynamic.DataBind()
    End Sub

    Private Sub LoadStaticList()
        rptStatic.DataSource = StaticList
        rptStatic.DataBind()
    End Sub

    Private Function GetTypeTitle() As String
        Return IIf(hdnType.Value = "static", "Static", "Dynamic")
    End Function

    Private Function GetFileTitle(ByVal file As String) As String
        If hdnType.Value = "static" Then
            Return Localization.GetResource("Resources.Global.Translations." & file)
        Else
            Dim target = DynamicList.SelectMany(Function(x) x.Items.Where(Function(y) y.Id = file))
            If target.Count > 0 Then
                Return target.FirstOrDefault().Title
            Else
                Return "Not Found"
            End If
        End If
    End Function

#End Region

#Region "Load Methods"

    Private Function LoadDynamics() As List(Of DynamicGroupItem)
        Dim file As String = "~" & Path.MapCMSFile(Helper.StructuresPath() & "/localization/Dynamic.xml")
        Dim doc As New XmlDocument()
        Dim lst As New List(Of DynamicGroupItem)
        Dim loaded As Boolean = True
        Try
            doc.Load(Server.MapPath(file))
        Catch ex As Exception
            loaded = False
        End Try
        If loaded Then
            Dim root = doc.SelectSingleNode("List")
            For Each n As XmlNode In root.SelectNodes("Group")
                Dim g As New DynamicGroupItem() With {
                    .Title = Localization.GetResource("Resources.Global.Translations." & Helper.GetSafeXML(n, "Name")),
                    .Id = Helper.GetSafeXML(n, "Id")
                }
                g.Items = New List(Of DynamicListItem)()
                Dim ni = n.SelectSingleNode("Items")
                For Each nn As XmlNode In ni.SelectNodes("Item")
                    g.Items.Add(New DynamicListItem() With {
                        .Id = Helper.GetSafeXML(nn, "Id"),
                        .Title = Localization.GetResource("Resources.Global.Translations." & Helper.GetSafeXML(nn, "Title")),
                        .Controller = Helper.GetSafeXML(nn, "Controller")
                    })
                Next
                lst.Add(g)
            Next
        End If
        Return lst
    End Function

    Private Function LoadStatics() As List(Of StaticGroupItem)
        Dim file As String = "~" & Path.MapCMSFile(Helper.StructuresPath() & "/localization/Static.xml")
        Dim doc As New XmlDocument()
        Dim lst As New List(Of StaticGroupItem)
        Dim loaded As Boolean = True
        Try
            doc.Load(Server.MapPath(file))
        Catch ex As Exception
            loaded = False
        End Try
        If loaded Then
            Dim root = doc.SelectSingleNode("List")
            For Each n As XmlNode In root.SelectNodes("Group")
                Dim g As New StaticGroupItem() With {
                    .Title = Localization.GetResource("Resources.Global.Translations." & Helper.GetSafeXML(n, "Title")),
                    .IsGlobal = Helper.GetSafeXML(n, "IsGlobal", ValueTypes.TypeBoolean),
                    .Directory = Helper.GetSafeXML(n, "Directory")
                }
                g.Items = New List(Of StaticListItem)()
                Dim ni = n.SelectSingleNode("Items")
                For Each nn As XmlNode In ni.SelectNodes("Item")
                    Dim filename As String = nn.InnerText
                    g.Items.Add(New StaticListItem() With {
                                .Title = Localization.GetResource("Resources.Global.Translations." & filename),
                                .FileName = filename,
                                .Directory = g.Directory,
                                .IsGlobal = g.IsGlobal
                    })
                Next
                lst.Add(g)
            Next
        End If
        Return lst
    End Function

    Private Function LoadDynamicFile(ByVal file As String, ByVal conn As SqlConnection) As List(Of DynamicFileItem)
        Dim originalFile As String = file.Replace("_Res", "")
        Dim oq As String = "SELECT Id, Title FROM {0}"
        Dim q As String = "SELECT R.LanguageId, R.Title, R.IsTranslated, R.TranslateDate, U.UserName FROM {0} R LEFT JOIN SYS_User U ON R.TranslateUser = U.Id WHERE R.Id = {1}"
        oq = String.Format(oq, originalFile)
        Dim lst As New List(Of DynamicFileItem)
        Using dt As DataTable = DBA.DataTable(conn, oq)
            For Each dr As DataRow In dt.Rows
                Dim item As New DynamicFileItem() With {
                    .Id = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                    .Original = Helper.GetSafeDBValue(dr("Title"))
                }
                item.Translations = New List(Of DynamicFileTranslationItem)()
                Dim nq = String.Format(q, file, item.Id)
                Using sdt As DataTable = DBA.DataTable(conn, nq)
                    For Each sdr As DataRow In sdt.Rows
                        item.Translations.Add(New DynamicFileTranslationItem() With {
                            .Id = item.Id,
                            .LanguageId = Helper.GetSafeDBValue(sdr("LanguageId"), ValueTypes.TypeInteger),
                            .Text = Helper.GetSafeDBValue(sdr("Title")),
                            .IsTranslated = Helper.GetSafeDBValue(sdr("IsTranslated"), ValueTypes.TypeBoolean),
                            .TranslationDate = Helper.GetSafeDBValue(sdr("TranslateDate"), ValueTypes.TypeDateTime),
                            .TranslationUser = Helper.GetSafeDBValue(sdr("UserName"))
                        })
                    Next
                End Using
                lst.Add(item)
            Next
        End Using
        Return lst
    End Function

#End Region

End Class

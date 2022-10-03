Imports EGV.Enums

Namespace EGV
    Namespace Structures

#Region "Data Access"

        Public Structure DataColumn
            Public Property ColumName As String
            Public Property ColumnAlias As String
            Public Property ColumnRename As String
        End Structure

        Public Structure DataSource
            Public Property TableName As String
            Public Property TableAlias As String
        End Structure

        Public Structure JoinedDataSource
            Public Property TableName As String
            Public Property TableAlias As String
            Public Property JoinType As Enums.TableJoinTypes
            Public Property SourceJoinField As String
            Public Property JoinField As String
            Public Property AddLanguageCondition As Boolean
            Public Property SourceFieldAlias As String
        End Structure

        Public Structure DataCondition
            Public Property Condition As String
        End Structure

        Public Structure DataSort
            Public Property SortColumn As String
            Public Property SortColumnAlias As String
            Public Property SortDirection As Enums.SortDirections
        End Structure

        Public Structure DataGroup
            Public Property ColumnName As String
            Public Property ColumnAlias As String
        End Structure

        Public Structure DBAReturnObject
            Public Property Count As Integer
            Public Property List As DataTable
            Public Property Query As String
        End Structure

#End Region

#Region "Entity Structure"

        Public Structure ESColumn
            Public Property Name As String
            Public Property TableAlias As String
            Public Property DataType As Enums.ESDataTypes
            Public Property Visible As Boolean
            Public Property ColumnOrder As Integer
            Public Property EnumLookup As String
            Public Property Lookup As String
            Public Property Rename As String
            Public Property Resource As String
            Public Property AllowSort As Boolean
            Public Property AllowReorder As Boolean
            Public Property Layout As ESColumnLayout
            Public Property Filter As ESColumnFilter
            Public Property FilterOperation As SearchTypes
            Public Property FilterValue As String
        End Structure

        Public Structure ESColumnLayout
            Public Property Width As String
            Public Property IsPrimary As Boolean
            Public Property EditControl As Boolean
            Public Property HeaderAlign As ControlAligns
            Public Property ItemAlign As ControlAligns
            Public Property DisplayType As DisplayTypes
            Public Property TypeLookup As String
            Public Property EditTypeDecider As String
            Public Property ListDataSource As String
            Public Property ListEnumDataSource As String
            Public Property DataTextField As String
            Public Property DataValueField As String
            Public Property LookupCondition As String
            Public Property LookupConditionValueField As String
            Public Property ParentField As String
        End Structure

        Public Structure ESColumnFilter
            Public Property FilterType As Enums.FilterTypes
            Public Property FilterDataText As String
            Public Property FilterDataValue As String
            Public Property AllowedTypes As List(Of SearchTypes)
            Public Property Expression As String
            Public Property Allow As Boolean
        End Structure

        Public Structure ESControlColumn
            Public Property Resource As String
            Public Property PermissionId As Integer
            Public Property Super As Boolean
            Public Property Type As Enums.ESControlTypes
        End Structure

        Public Structure ESSearchColumn
            Public Property Name As String
            Public Property Type As Enums.SearchTypes
        End Structure

#End Region

#Region "HTTP"

        Public Structure CookieItem
            Public Property Id As String
            Public Property Value As Object
        End Structure

#End Region

#Region "Controls"

        Public Structure AutoComplete
            Public Property id As String
            Public Property text As String
        End Structure

        Public Structure RenderedImage
            Public Property Width As Decimal
            Public Property Height As Decimal
            Public Property CropWidth As Decimal
            Public Property CropHeight As Decimal
            Public Property ZoomScale As Decimal
        End Structure

        Public Structure EditableColumn
            Public Property ColumnName As String
            Public Property ColumnValue As String
            Public Property ColumnKey As String
        End Structure

        Public Structure JSSelectedColumn
            Public Property ColumnAlias As String
            Public Property ColumnName As String
        End Structure

        Public Structure FilterItem
            Public Property DisplayTitle As String
            Public Property Expression As String
            Public Property Type As FilterTypes
            Public Property LookupSource As String
            Public Property EnumLookupSource As String
            Public Property LookupDataText As String
            Public Property LookupDataValue As String
            Public Property AllowedTypes As List(Of SearchTypes)
        End Structure

        Public Structure JSFilterDefinition
            Public Property Expression As String
            Public Property DataType As FilterTypes
            Public Property AllowedTypes As List(Of JSFilterAllowedType)
        End Structure

        Public Structure JSFilterAllowedType
            Public Property Text As String
            Public Property Value As Integer
        End Structure

#End Region

#Region "Pages"

        Public Structure BCItem
            Public Property URL As String
            Public Property IconClass As String
            Public Property Title As String
        End Structure

#End Region

#Region "User Grid"

        Public Structure UserGridColumn
            Public Property Name As String
            Public Property [Alias] As String
            Public Property Visible As Boolean
            Public Property Order As Integer
        End Structure

        Public Structure UserGridSortColumn
            Public Property Name As String
            Public Property [Alias] As String
            Public Property Direction As Enums.SortDirections
        End Structure

        Public Structure UserGridColumnFilter
            Public Property Operation As SearchTypes
            Public Property Value As String
            Public Property Expression As String
            Public Property DataType As FilterTypes
        End Structure

#End Region

#Region "Mobile Application"

        Public Structure AppLoginToken
            Public Property Token As String
            Public Property Type As MembershipTypes
        End Structure

        Public Structure AppMemberProfile
            Public Property FullName As String
            Public Property Email As String
            Public Property Type As MembershipTypes
        End Structure

        Public Structure FamilyStudent
            Public Property AccessToken As String
            Public Property FullName As String
            Public Property UserName As String
        End Structure

        Public Structure SeasonObject
            Public Property Id As Integer
            Public Property Title As String
            Public Property IsCurrent As Boolean
        End Structure

        Public Structure MessageCompose
            Public Property Title As String
            Public Property Users As List(Of ReceiverType)
            Public Property SenderToken As String
            Public Property Attachments As List(Of String)
        End Structure

        Public Structure MessageObject
            Public Property MessageId As String
            Public Property Title As String
            Public Property MessageDate As String
            Public Property SenderTitle As String
            Public Property HasAttachments As Boolean
            Public Property IsRead As Boolean
        End Structure

        Public Structure FullMessageObject
            Public Property MessageId As String
            Public Property Title As String
            Public Property MessageDate As String
            Public Property SenderTitle As String
            Public Property Body As String
            Public Property Attachments As List(Of AttachmentObject)
        End Structure

        Public Structure MessagesList
            Public Property Total As Integer
            Public Property Messages As List(Of MessageObject)
        End Structure

        Public Structure UserIdInformation
            Public Property UserId As Integer
            Public Property UserType As MembershipTypes
        End Structure

        Public Structure AttachmentObject
            Public Property FileName As String
            Public Property FilePath As String
            Public Property FileSize As String
            Public Property FileType As String
        End Structure

        Public Structure StudentAttendance
            Public Property PresentDays As Integer
            Public Property AbsentDays As Integer
            Public Property AbsentDates As List(Of String)
        End Structure

        Public Structure StudentMaterialExamsItem
            Public Property MaterialId As Integer
            Public Property MaterialTitle As String
            Public Property MaterialMaxMark As Integer
            Public Property Exams As List(Of ExamItem)
        End Structure

        Public Structure ExamItem
            Public Property ExamTitle As String
            Public Property ExamMark As String
            Public Property ExamType As ExamItemTypes
            Public Property ExamMaxMark As Integer
        End Structure

        Public Structure TeacherListItem
            Public Property TeacherName As String
            Public Property Materials As List(Of String)
        End Structure

        Public Structure StaffListItem
            Public Property Role As String
            Public Property RoleId As Integer
            Public Property RoleName As String
            Public Property Staff As List(Of String)
        End Structure

        Public Structure NotificationItem
            Public Property app_id As String
            Public Property contents As TextualData
            Public Property headings As TextualData
            Public Property include_player_ids As String()
            Public Property ttl As Integer
            Public Property data As NotificationAdditionalData
        End Structure

        Public Structure TextualData
            Public Property en As String
        End Structure

        Public Structure NotificationData
            Public Property contents As TextualData
            Public Property headings As TextualData
        End Structure

        Public Structure NotificationAdditionalData
            Public Property negative_note As Boolean
            Public Property notification_type As NotificationTypes
        End Structure

        Public Structure StudentAccountItem
            Public Property StudentToken As String
            Public Property StudentName As String
            Public Property PreviousClass As String
            Public Property CurrentClass As String
            Public Property Transportation As Boolean
            Public Property Deposit As Decimal
            Public Property Subscription As Decimal
            Public Property Total As Decimal
            Public Property Discount As Decimal
            Public Property NetTotal As Decimal
            Public Property PaymentsSum As Decimal
            Public Property Balance As Decimal
            Public Property Payments As List(Of StudentPaymentItem)
        End Structure

        Public Structure StudentPaymentItem
            Public Property PaymentNumber As String
            Public Property PaymentAmount As Decimal
            Public Property PaymentDate As String
        End Structure

#End Region

#Region "Messaging"

        Public Structure ParsedMessageUser
            Public Property UserId As Integer
            Public Property UserType As MessageUserTypes
            Public Property UserRole As MessageUserRoles
            Public Property FullName As String
        End Structure

        Public Structure NextPrevMessageId
            Public Property NextId As Guid
            Public Property PrevId As Guid
        End Structure

        Public Structure ReceiverType
            Public Property id As Integer
            Public Property type As MessageUserTypes
        End Structure

        Public Structure ShortMemberObject
            Public Property MemberId As Integer
            Public Property MemberType As MembershipTypes
        End Structure

#End Region

#Region "Modules"

        Public Structure InternalNoteItem
            Public Property UserName As String
            Public Property NoteDate As String
            Public Property Note As String
        End Structure

        Public Structure ClassObject
            Public Property Id As Integer
            Public Property Title As String
        End Structure

        Public Structure SectionObject
            Public Property Id As Integer
            Public Property Title As String
        End Structure

        Public Structure UserObject
            Public Property Id As Integer
            Public Property FullName As String
        End Structure

        Public Structure MaterialObject
            Public Property Id As Integer
            Public Property Title As String
        End Structure

        Public Structure MaterialUser
            Public Property MaterialId As Integer
            Public Property UserId As Integer
        End Structure

        Public Structure UserMaterialObject
            Public Property ClassId As Integer
            Public Property SectionId As Integer
            Public Property MaterialId As Integer
            Public Property MaterialTitle As String
        End Structure

        Public Structure ClassDays
            Public Property ClassId As Integer
            Public Property ClassTitle As String
            Public Property SchoolDays As Integer
            Public Property HolidayDays As Integer
            Public Property SchoolDays2 As Integer
            Public Property HolidayDays2 As Integer
        End Structure

        Public Structure ClassUser
            Public Property ClassId As Integer
            Public Property ClassTitle As String
            Public Property UserId As Integer
        End Structure

        Public Structure SectionStudentAttendance
            Public Property StudentId As Integer
            Public Property StudentName As String
            Public Property StudentAttend As Boolean
        End Structure

        Public Structure ExamTemplateObject
            Public Property Id As Integer
            Public Property Title As String
        End Structure

        Public Structure StudentMaterialExamItem
            Public Property StudentId As Integer
            Public Property MaterialId As Integer
            Public Property ItemId As Integer
            Public Property Mark As Decimal
            Public Property MaxMark As Decimal
            'Public Property CreatedDate As String
            'Public Property CreatedUser As String
            'Public Property ModifiedDate As String
            'Public Property ModifiedUser As String
        End Structure

        Public Structure NoteItem
            Public Property Id As Integer
            Public Property SenderId As Integer
            Public Property SenderName As String
            Public Property StudentId As Integer
            Public Property StudentSchoolId As String
            Public Property StudentName As String
            Public Property NoteType As Integer
            Public Property NoteDate As String
            Public Property NoteText As String
        End Structure

        Public Structure PaymentItem
            Public Property Id As Integer
            Public Property StudentId As Integer
            Public Property PaymentNumber As Integer
            Public Property PaymentAmount As Decimal
            Public Property PaymentDate As String
            Public Property PaymentNote As String
        End Structure

        Public Structure ExamResultItem
            Public Property Mark As Decimal
            Public Property CreatedDate As String
            Public Property CreatedUser As String
            Public Property ModifiedDate As String
            Public Property ModifiedUser As String
        End Structure

#End Region

#Region "Translations"

        Public Structure DynamicGroupItem
            Public Property Title As String
            Public Property Id As String
            Public Property Items As List(Of DynamicListItem)
        End Structure

        Public Structure DynamicListItem
            Public Property Id As String
            Public Property Title As String
            Public Property Controller As String
        End Structure

        Public Structure DynamicControllerItem
            Public Property Id As String
            Public Property Controller As String
        End Structure

        Public Structure StaticControllerItem
            Public Property File As String
            Public Property Directory As String
            Public Property IsGlobal As Boolean
        End Structure

        Public Structure StaticGroupItem
            Public Property Title As String
            Public Property Directory As String
            Public Property IsGlobal As Boolean
            Public Property Items As List(Of StaticListItem)
        End Structure

        Public Structure StaticListItem
            Public Property Title As String
            Public Property FileName As String
            Public Property IsGlobal As Boolean
            Public Property Directory As String
        End Structure

        Public Structure DynamicFileItem
            Public Property Id As Integer
            Public Property Original As String
            Public Property Translations As List(Of DynamicFileTranslationItem)
        End Structure

        Public Structure DynamicFileTranslationItem
            Public Property LanguageId As Integer
            Public Property Id As Integer
            Public Property Text As String
            Public Property IsTranslated As Boolean
            Public Property TranslationDate As DateTime
            Public Property TranslationUser As String
        End Structure

        Public Structure StaticFileItem
            Public Property Key As String
            Public Property Translations As List(Of StaticFileTranslationItem)
        End Structure

        Public Structure StaticFileTranslationItem
            Public Property LanguageCode As String
            Public Property Key As String
            Public Property Text As String
        End Structure

        <Serializable()>
        Public Structure LocalizationItem
            Public Property FileName As String
            Public Property LanguageId As Integer
            Public Property Resoruces As List(Of LocalizationResourceItem)
        End Structure

        <Serializable()>
        Public Structure LocalizationResourceItem
            Public Property Key As String
            Public Property Value As String
        End Structure

#End Region

    End Namespace
End Namespace
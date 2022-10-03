Namespace EGV
    Namespace Enums

#Region "Query Builder"

        Public Enum QueryTypes
            NonQuery = 1
            DataTable = 2
            Scalar = 3
            Insert = 4
            Update = 5
        End Enum

        Public Enum ValueTypes
            TypeInteger = 1
            TypeString = 2
            TypeDate = 3
            TypeDateTime = 4
            TypeBoolean = 5
            TypeDecimal = 6
        End Enum

        Public Enum TableJoinTypes
            Left = 1
            Right = 2
            Inner = 3
            Outer = 4
        End Enum

        Public Enum SortDirections
            Ascending = 1
            Descending = 2
        End Enum

        Public Enum ESDataTypes
            TypeInteger = 1
            TypeString = 2
            TypeLongString = 3
            TypeDateTime = 4
            TypeDate = 5
            TypeBoolean = 6
            TypeList = 7
            TypeDecimal = 8
        End Enum

        Public Enum ESControlTypes
            Edit = 1
            Delete = 2
            View = 3
            ModalEdit = 4
        End Enum

        Public Enum SearchTypes
            None = 0
            Contains = 1
            StartsWith = 2
            EndsWith = 3
            Exact = 4
            Equal = 5
            GreaterThan = 6
            GreaterThanOrEqual = 7
            LessThan = 8
            LessThanOrEqual = 9
            NotEqual = 10
        End Enum

        Public Enum FilterTypes
            [String] = 1
            Number = 2
            [Boolean] = 3
            [Date] = 4
            List = 5
        End Enum

#End Region

#Region "Controls"

        Public Enum NotifyTypes
            Warning = 1
            Success = 2
            Danger = 3
            Notify = 4
        End Enum

        Public Enum ControlSizes
            [Default] = 1
            Small = 2
            Large = 3
        End Enum

        Public Enum ButtonSizes
            Normal = 1
            Large = 2
            Small = 3
            XSmall = 4
        End Enum

        Public Enum ControlColors
            [Default] = 1
            Primary = 2
            Warning = 3
            Danger = 4
            Info = 5
            Maroon = 6
            Purple = 7
            Navy = 8
            Orange = 9
            Olive = 10
            Success = 11
        End Enum

        Public Enum ValidationTypes
            None = 0
            Email = 1
            Website = 2
            [Date] = 3
        End Enum

        Public Enum CroppingTypes
            Center = 1
            TopLeft = 2
            TopRight = 3
            BottomLeft = 4
            BottomRight = 5
            Manual = 6
        End Enum

        Public Enum BoxTypes
            [Default] = 1
            Primary = 2
            Success = 3
            Danger = 4
            Warning = 5
            Info = 6
        End Enum

        Public Enum PagingPositions
            Top = 1
            Bottom = 2
            TopAndBottom = 3
        End Enum

        Public Enum ControlAligns
            Left = 1
            Right = 2
            Center = 3
        End Enum

        Public Enum DisplayTypes
            BooleanIcon = 1
            InlineImageWithText = 2
            LongDateTime = 3
            CombinedFields = 4
            DecimalNumber = 5
            InlineImage = 6
            ParentChild = 7
            ClassName = 8
        End Enum

        Public Enum ToolbarActiveStates
            None = 1
            Multi = 2
            [Single] = 3
            Always = 4
        End Enum

#End Region

#Region "Emails"

        Public Enum EmailTypes
            [To] = 1
            CC = 2
            BCC = 3
        End Enum

#End Region

#Region "Membership"

        Public Enum MembershipTypes
            Student = 1
            Family = 2
        End Enum

#End Region

#Region "Messaging"

        Public Enum MessageUserTypes
            Student = 1
            Family = 2
            User = 3
            AllSection = 4
            AllClass = 5
            AllRoleUsers = 6
            AllUsers = 7
        End Enum

        Public Enum MessageUserRoles
            Sender = 1
            RecTo = 2
            RecCC = 3
            RecBCC = 4
        End Enum

        Public Enum MessageUserRoleTypes
            Sent = 1
            Received = 2
        End Enum

        Public Enum MessageFilterTypes
            Inbox = 1
            Starred = 2
            Unread = 3
            Sent = 4
        End Enum

#End Region

#Region "Translations"

        Public Enum TranslationTypes
            Dynamic = 1
            [Static] = 2
        End Enum

#End Region

#Region "Modules"

        Public Enum ExamItemTypes
            Number = 1
            Total = 2
            Average = 3
        End Enum

        Public Enum NoteTypes
            Negative = 1
            Positive = 2
        End Enum

        Public Enum NotificationTypes
            Messaging = 1
            Notes = 2
            Payment = 3
        End Enum

#End Region

    End Namespace
End Namespace
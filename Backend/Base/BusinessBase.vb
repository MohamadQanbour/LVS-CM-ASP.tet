Imports System.Data.SqlClient

Namespace EGV
    Namespace Business

        'Prime Business Base
        Public MustInherit Class PrimeBusinessBase

#Region "Private Members"

            Private Property _MyConn As SqlConnection

#End Region

#Region "Public Members"

            Public Property MyConn As SqlConnection
                Get
                    If _MyConn Is Nothing Then _MyConn = DBA.GetConn()
                    Return _MyConn
                End Get
                Set(value As SqlConnection)
                    _MyConn = value
                End Set
            End Property

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal conn As SqlConnection = Nothing)
                _MyConn = conn
            End Sub

#End Region

#Region "Public Methods"

            Public Function Safe(ByVal field As Object, Optional ByVal type As Enums.ValueTypes = Enums.ValueTypes.TypeString) As Object
                Return Utils.Helper.GetSafeDBValue(field, type)
            End Function

#End Region

        End Class

        'Busniess Base
        Public MustInherit Class BusinessBase
            Inherits PrimeBusinessBase

#Region "Constructors"

            Public Sub New(Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
            End Sub

#End Region

#Region "Overridden Methods"

            Public MustOverride Sub Insert(Optional ByVal trans As SqlTransaction = Nothing)
            Public MustOverride Sub Update(Optional ByVal trans As SqlTransaction = Nothing)
            Public MustOverride Sub Save(Optional ByVal trans As SqlTransaction = Nothing)
            Public MustOverride Sub Delete(Optional ByVal conn As SqlConnection = Nothing)

#End Region

        End Class

        'Localized Prime Business Base
        Public MustInherit Class LocPrimeBusinessBase
            Inherits PrimeBusinessBase

#Region "Private Members"

            Private Property _MyLanguageId As Integer

#End Region

#Region "Public Members"

            Public Property MyLanguageId As Integer
                Get
                    Utils.Helper.GetSafeLanguageId(_MyLanguageId)
                    Return _MyLanguageId
                End Get
                Set(value As Integer)
                    _MyLanguageId = value
                End Set
            End Property

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal conn As SqlConnection = Nothing, Optional ByVal langId As Integer = 0)
                MyBase.New(conn)
                MyLanguageId = langId
            End Sub

#End Region

#Region "Overloaded Methods"

            Public Overridable Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    MyLanguageId = Safe(dr("LanguageId"), Enums.ValueTypes.TypeInteger)
                End If
            End Sub

#End Region

        End Class

        'Localized Business Base
        Public MustInherit Class LocBusinessBase
            Inherits BusinessBase

#Region "Private Members"

            Private Property _MyLanguageId As Integer

#End Region

#Region "Public Members"

            Public Property MyLanguageId As Integer
                Get
                    Utils.Helper.GetSafeLanguageId(_MyLanguageId)
                    Return _MyLanguageId
                End Get
                Set(value As Integer)
                    _MyLanguageId = value
                End Set
            End Property

            Public Property IsTranslated As Boolean
            Public Property TranslateDate As Date
            Public Property TranslateUser As Integer
            Public Property TranslateUserName As String

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal conn As SqlConnection = Nothing, Optional ByVal langId As Integer = 0)
                MyBase.New(conn)
                MyLanguageId = langId
            End Sub

#End Region

#Region "Overloaded Methods"

            Public Overridable Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    MyLanguageId = Safe(dr("LanguageId"), Enums.ValueTypes.TypeInteger)
                    IsTranslated = Safe(dr("IsTranslated"), Enums.ValueTypes.TypeBoolean)
                    TranslateDate = Safe(dr("TranslateDate"), Enums.ValueTypes.TypeDateTime)
                    TranslateUser = Safe(dr("TranslateUser"), Enums.ValueTypes.TypeInteger)
                    TranslateUserName = Safe(dr("TranslateUserName"), Enums.ValueTypes.TypeString)
                End If
            End Sub

#End Region

#Region "Overridden Methods"

            Public MustOverride Sub InsertRes(Optional ByVal trans As SqlTransaction = Nothing)
            Public MustOverride Sub UpdateRes(Optional ByVal trans As SqlTransaction = Nothing)
            Public MustOverride Sub UpdateDefaultRes(Optional ByVal trans As SqlTransaction = Nothing)
            Public MustOverride Sub Translate(ByVal langId As Integer, ByVal userId As Integer, Optional ByVal trans As SqlTransaction = Nothing)

#End Region

        End Class

        'Audited Business Base
        Public MustInherit Class AudBusinessBase
            Inherits BusinessBase

#Region "Public Members"

            Public Property CreatedDate As Date
            Public Property CreatedUser As Integer
            Public Property CreatedUserName As String
            Public Property ModifiedDate As Date
            Public Property ModifiedUser As Integer
            Public Property ModifiedUserName As String

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
            End Sub

#End Region

#Region "Overloaded Methods"

            Public Overridable Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    CreatedDate = Safe(dr("CreatedDate"), Enums.ValueTypes.TypeDateTime)
                    CreatedUser = Safe(dr("CreatedUser"), Enums.ValueTypes.TypeInteger)
                    CreatedUserName = Safe(dr("CreatedUserName"), Enums.ValueTypes.TypeString)
                    ModifiedDate = Safe(dr("ModifiedDate"), Enums.ValueTypes.TypeDateTime)
                    ModifiedUser = Safe(dr("ModifiedUser"), Enums.ValueTypes.TypeInteger)
                    ModifiedUserName = Safe(dr("ModifiedUserName"), Enums.ValueTypes.TypeString)
                End If
            End Sub

#End Region

        End Class

        'Localized Audited Business Base
        Public MustInherit Class AudLocBusinessBase
            Inherits AudBusinessBase

#Region "Private Members"

            Private Property _MyLanguageId As Integer

#End Region

#Region "Public Members"

            Public Property MyLanguageId As Integer
                Get
                    Utils.Helper.GetSafeLanguageId(_MyLanguageId)
                    Return _MyLanguageId
                End Get
                Set(value As Integer)
                    _MyLanguageId = value
                End Set
            End Property

            Public Property IsTranslated As Boolean
            Public Property TranslateDate As Date
            Public Property TranslateUser As Integer
            Public Property TranslateUserName As String

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal conn As SqlConnection = Nothing, Optional ByVal langId As Integer = 0)
                MyBase.New(conn)
                MyLanguageId = langId
            End Sub

#End Region

#Region "Overloaded Methods"

            Public Overrides Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    MyLanguageId = Safe(dr("LanguageId"), Enums.ValueTypes.TypeInteger)
                    IsTranslated = Safe(dr("IsTranslated"), Enums.ValueTypes.TypeBoolean)
                    TranslateDate = Safe(dr("TranslateDate"), Enums.ValueTypes.TypeDateTime)
                    TranslateUser = Safe(dr("TranslateUser"), Enums.ValueTypes.TypeInteger)
                    TranslateUserName = Safe(dr("TranslateUserName"), Enums.ValueTypes.TypeString)
                    MyBase.FillObject(dr)
                End If
            End Sub

#End Region

#Region "Overridden Methods"

            Public MustOverride Sub InsertRes(Optional ByVal trans As SqlTransaction = Nothing)
            Public MustOverride Sub UpdateRes(Optional ByVal trans As SqlTransaction = Nothing)
            Public MustOverride Sub UpdateDefaultRes(Optional ByVal trans As SqlTransaction = Nothing)
            Public MustOverride Sub Translate(ByVal langId As Integer, ByVal userId As Integer, Optional ByVal trans As SqlTransaction = Nothing)

#End Region

        End Class

    End Namespace
End Namespace
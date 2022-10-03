Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Business
Imports EGV.Structures
Imports EGV.Enums
Imports EGV.Constants

Namespace Ajax

    Public Class CMSAutoComplete
        Inherits AjaxBaseClass

#Region "Request Values"

        Public Property SearchTerm As String = GetSafeRequestValue("q")
        Public Property ExcludeId As Integer = GetSafeRequestValue("exclude", ValueTypes.TypeInteger)
        Public Property AllowNull As Boolean = GetSafeRequestValue("allownull", ValueTypes.TypeBoolean)
        Public Property UserType As MessageUserTypes = GetSafeRequestValue("usertype", ValueTypes.TypeInteger)
        Public Property UserId As Integer = GetSafeRequestValue("userid", ValueTypes.TypeInteger)
        Public Property SID As Integer = GetSafeRequestValue("SID", ValueTypes.TypeInteger)
        Public Property Year As String = GetSafeRequestValue("Year")
        Public Property Term As String = GetSafeRequestValue("Term")
        Public Property ClassId As Integer = GetSafeRequestValue("classid", ValueTypes.TypeInteger)
        Public Property CurrentYear As Boolean = GetSafeRequestValue("currentyear", ValueTypes.TypeBoolean)
        Public Property SectionId As Integer = GetSafeRequestValue("sectionid", ValueTypes.TypeInteger)
        Public Property TemplateId As Integer = GetSafeRequestValue("templateid", ValueTypes.TypeInteger)

#End Region

#Region "Overridden Methods"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "Families"
                    ret = GetFamilies(MyConn, LanguageId)
                Case "Areas"
                    ret = GetAreas(MyConn, LanguageId)
                Case "Roles"
                    ret = GetRoles(MyConn, LanguageId)
                Case "Classes"
                    ret = GetClasses(MyConn, LanguageId)
                Case "Sections"
                    ret = GetSections(MyConn, LanguageId)
                Case "SectionsAttendance"
                    ret = GetSectionsAttendance(MyConn, LanguageId)
                Case "MessagingTo"
                    ret = GetMessagingTo(MyConn, LanguageId)
                Case "Students"
                    ret = GetStudents(MyConn, LanguageId)
                Case "ClassTemplates"
                    ret = GetClassTemplates(MyConn, LanguageId)
                Case "TemplateMaxMark"
                    ret = GetTemplateMaxMark(MyConn, LanguageId)
                Case "TeacherClasses"
                    ret = GetTeacherClasses(MyConn, LanguageId)
                Case "TeacherSections"
                    ret = GetTeacherSections(MyConn, LanguageId)
                Case "TeacherStudents"
                    ret = GetTeacherStudents(MyConn, LanguageId)
                Case "TeacherMaterials"
                    ret = GetTeacherMaterials(MyConn, LanguageId)
                Case "MaterialExams"
                    ret = GetMaterialExams(MyConn, LanguageId)
                Case "ClassExamItems"
                    ret = GetClassExamTemplateItems(MyConn, LanguageId)
                Case "SYear"
                    ret = GetSortYear(MyConn, LanguageId)
                Case "STerm"
                    ret = GetSortTerm(MyConn, LanguageId)
                Case "SClass"
                    ret = GetSortClass(MyConn, LanguageId)
            End Select
            Return ret
        End Function


#End Region

#Region "Private Methods"

        Private Function GetFamilies(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            If AllowNull Then
                ret.Add(New AutoComplete() With {
                    .id = "0",
                    .text = Localization.GetResource("Resources.Global.CMS.None")
                })
            End If
            Using dt As DataTable = FamilyController.GetCollection(conn, 0, SearchTerm, False).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New AutoComplete() With {
                        .id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .text = Safe(dr("SchoolId")) & " - " & Safe(dr("FullName")) & " <" & Safe(dr("Email")) & ">"
                    })
                Next
            End Using
            Return ret
        End Function

        Private Function GetAreas(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            If AllowNull Then
                ret.Add(New AutoComplete() With {
                    .id = "0",
                    .text = Localization.GetResource("Resources.Global.CMS.None")
                })
            End If
            Using dt As DataTable = AreaController.GetCollection(conn, langId, 0, SearchTerm).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New AutoComplete() With {
                        .id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .text = Safe(dr("Title"))
                    })
                Next
            End Using
            Return ret
        End Function

        Private Function GetRoles(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            If AllowNull Then
                ret.Add(New AutoComplete() With {
                    .id = "0",
                    .text = Localization.GetResource("Resources.Global.CMS.None")
                })
            End If
            Using dt As DataTable = RoleController.List(conn, False).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New AutoComplete() With {
                        .id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .text = Safe(dr("Title"))
                    })
                Next
            End Using
            Return ret
        End Function

        Private Function GetClasses(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            If AllowNull Then
                ret.Add(New AutoComplete() With {
                    .id = "0",
                    .text = Localization.GetResource("Resources.Global.CMS.None")
                })
            End If
            Using dt As DataTable = StudyClassController.GetCollection(conn, langId, 0, SearchTerm).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New AutoComplete() With {
                        .id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .text = Safe(dr("Title"))
                    })
                Next
            End Using
            Return ret
        End Function

        Private Function GetSectionsAttendance(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            If AllowNull Then
                ret.Add(New AutoComplete() With {
                    .id = "0",
                    .text = Localization.GetResource("Resources.Global.CMS.None")
                })
            End If
            Using dt As DataTable = StudentPresentController.GetSectionsOfAdmin(conn, UserId, ClassId, langId).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New AutoComplete() With {
                        .id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .text = Safe(dr("Title"))
                    })
                Next
            End Using
            Return ret
        End Function

        Private Function GetSections(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            If AllowNull Then
                ret.Add(New AutoComplete() With {
                    .id = "0",
                    .text = Localization.GetResource("Resources.Global.CMS.None")
                })
            End If
            Using dt As DataTable = SectionController.GetCollection(conn, langId, 0, SearchTerm, CurrentYear, ClassId, UserId).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New AutoComplete() With {
                        .id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .text = Safe(dr("FullName"))
                    })
                Next
            End Using
            Return ret
        End Function

        Private Function GetStudents(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            If AllowNull Then
                ret.Add(New AutoComplete() With {
                    .id = "0",
                    .text = Localization.GetResource("Resources.Global.CMS.None")
                })
            End If
            Using dt As DataTable = StudentController.GetCollection(conn, 0, SearchTerm, False, 0, SectionId).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New AutoComplete() With {
                        .id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .text = Safe(dr("IdName"))
                    })
                Next
            End Using
            Return ret
        End Function

        Private Function GetSortYear(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            If AllowNull Then
                ret.Add(New AutoComplete() With {
                    .id = "0",
                    .text = Localization.GetResource("Resources.Global.CMS.None")
                })
            End If
            Using dt As DataTable = StudentController.getSYear(conn, SearchTerm, SID).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New AutoComplete() With {
                        .id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .text = Safe(dr("IdSYear"))
                    })
                Next
            End Using

            Return ret
        End Function
        Private Function GetSortTerm(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            Using dt As DataTable = StudentController.GetSTerm(conn, SearchTerm, Year, SID).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New AutoComplete() With {
                        .id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .text = Safe(dr("IdSTerm"))
                    })
                Next
            End Using
            Return ret
        End Function
        Private Function GetSortClass(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            Using dt As DataTable = StudentController.GetSClass(conn, SearchTerm, Year, Term, SID).List
                For Each dr As DataRow In dt.Rows
                    ret.Add(New AutoComplete() With {
                        .id = Safe(dr("Id"), ValueTypes.TypeInteger),
                        .text = Safe(dr("IdSClass"))
                    })
                Next
            End Using
            Return ret
        End Function
        Private Function GetMessagingTo(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim ret As New List(Of AutoComplete)
            Select Case UserType
                Case MessageUserTypes.Family

                Case MessageUserTypes.Student

                Case MessageUserTypes.User
                    If Not UserTypeRoleController.UserInType(UserId, UserTypes.Teacher, conn) AndAlso Not UserTypeRoleController.UserInType(UserId, UserTypes.ExamSupervisor, conn) AndAlso Not UserTypeRoleController.UserInType(UserId, UserTypes.EmployeesSupervisor, conn) Then
                        'family
                        Using dt As DataTable = FamilyController.GetCollection(conn, 0, SearchTerm, True).List
                            For Each dr As DataRow In dt.Rows
                                If Not ret.Exists(Function(x) x.id = "") Then
                                    ret.Add(New AutoComplete() With {
                                    .text = dr("SchoolId") & " - " & dr("FullName"),
                                        .id = "{""id"": " & dr("Id") & ", ""type"": " & MessageUserTypes.Family & "}"
                                    })
                                End If
                            Next
                        End Using

                    End If
                    If Not UserTypeRoleController.UserInType(UserId, UserTypes.ChiefOfClasses, conn) AndAlso Not UserTypeRoleController.UserInType(UserId, UserTypes.StudentAffairs, conn) AndAlso Not UserTypeRoleController.UserInType(UserId, UserTypes.TransportationAdmin, conn) AndAlso Not UserTypeRoleController.UserInType(UserId, UserTypes.ExamSupervisor, conn) AndAlso Not UserTypeRoleController.UserInType(UserId, UserTypes.EmployeesSupervisor, conn) Then
                        'student
                        Using dt As DataTable = StudentController.GetCollection(conn, 0, SearchTerm, True).List
                            For Each dr As DataRow In dt.Rows
                                ret.Add(New AutoComplete() With {
                                    .text = dr("SchoolId") & " - " & dr("FullName"),
                                    .id = "{""id"": " & dr("Id")  & ", ""type"": " & MessageUserTypes.Student & "}"
                                })
                            Next
                        End Using
                        'classes & sections
                        Dim classes As New List(Of ClassObject)
                        Dim sections As New List(Of SectionObject)
                        Dim teacherRoleId As Integer = UserTypeRoleController.GetRoleOfType(conn, "t")
                        If UserController.UserInRole(conn, UserId, teacherRoleId) Then
                            classes = StudyClassController.GetTeacherClasses(conn, UserId, langId, SearchTerm)
                            sections = SectionController.GetTeacherSections(conn, UserId, langId, SearchTerm)
                        Else
                            '++Class
                            Using dt As DataTable = StudyClassController.GetCollection(conn, langId, 0, SearchTerm).List
                                For Each dr As DataRow In dt.Rows
                                    classes.Add(New ClassObject() With {
                                        .Id = Safe(dr("Id"), ValueTypes.TypeInteger),
                                        .Title = Safe(dr("Title"))
                                    })
                                Next
                            End Using
                            '++Sections
                            Using dt As DataTable = SectionController.GetCollection(conn, langId, 0, SearchTerm, True, 0).List
                                For Each dr As DataRow In dt.Rows
                                    If Not sections.Exists(Function(x) x.Id = Safe(dr("Id"), ValueTypes.TypeInteger)) Then
                                        sections.Add(New SectionObject() With {
                                            .Id = Safe(dr("Id"), ValueTypes.TypeInteger),
                                            .Title = Safe(dr("ClassName")) & " - " & Safe(dr("Title"))
                                        })
                                    End If
                                Next
                            End Using
                        End If
                        For Each item As ClassObject In classes
                            ret.Add(New AutoComplete() With {
                                .id = "{""id"": " & item.Id & ", ""type"": " & MessageUserTypes.AllClass & "}",
                                .text = Localization.GetResource("Resources.Global.CMS.Class") & " - " & item.Title
                            })
                        Next
                        For Each item As SectionObject In sections
                            If Not ret.Exists(Function(x) x.id = "{""id"": " & item.Id & ", ""type"": " & MessageUserTypes.AllSection & "") Then
                                ret.Add(New AutoComplete() With {
                                    .id = "{""id"": " & item.Id & ", ""type"": " & MessageUserTypes.AllSection & "}",
                                    .text = Localization.GetResource("Resources.Global.CMS.Section") & " - " & item.Title
                                })
                            End If
                        Next
                    End If
                    ''user admin
                    Using dt As DataTable = UserController.GetCollection(conn, 0, SearchTerm, True, {1}).List
                        For Each dr As DataRow In dt.Rows
                            ret.Add(New AutoComplete() With {
                                .text = dr("UserName"),
                                .id = "{""id"": " & dr("Id") & ", ""type"": " & MessageUserTypes.User & "}"
                            })
                        Next
                    End Using

                    ''user cheif of classes
                    Using dt As DataTable = UserController.GetCollection(conn, 0, SearchTerm, True, {2}).List
                        For Each dr As DataRow In dt.Rows
                            ret.Add(New AutoComplete() With {
                                .text = dr("UserName"),
                                .id = "{""id"": " & dr("Id") & ", ""type"": " & MessageUserTypes.User & "}"
                            })
                        Next
                    End Using

                    ''user class admins
                    If Not UserTypeRoleController.UserInType(UserId, UserTypes.StudentAffairs, conn) Then
                        Using dt As DataTable = UserController.GetCollection(conn, 0, SearchTerm, True, {3}).List
                            For Each dr As DataRow In dt.Rows
                                ret.Add(New AutoComplete() With {
                                    .text = dr("UserName"),
                                    .id = "{""id"": " & dr("Id") & ", ""type"": " & MessageUserTypes.User & "}"
                                })
                            Next
                        End Using
                    End If

                    ''user student affair
                    If Not UserTypeRoleController.UserInType(UserId, UserTypes.Teacher, conn) Then
                        Using dt As DataTable = UserController.GetCollection(conn, 0, SearchTerm, True, {6}).List
                            For Each dr As DataRow In dt.Rows
                                ret.Add(New AutoComplete() With {
                                    .text = dr("UserName"),
                                    .id = "{""id"": " & dr("Id") & ", ""type"": " & MessageUserTypes.User & "}"
                                })
                            Next
                        End Using
                    End If

                    ''teachers
                    If Not UserTypeRoleController.UserInType(UserId, UserTypes.StudentAffairs, conn) Then
                        Using dt As DataTable = UserController.GetCollection(conn, 0, SearchTerm, True, {7}).List
                            For Each dr As DataRow In dt.Rows
                                ret.Add(New AutoComplete() With {
                                    .text = dr("UserName"),
                                    .id = "{""id"": " & dr("Id") & ", ""type"": " & MessageUserTypes.User & "}"
                                })
                            Next
                        End Using
                    End If

                    ''transportation
                    If Not UserTypeRoleController.UserInType(UserId, UserTypes.ExamSupervisor, conn) Then
                        Using dt As DataTable = UserController.GetCollection(conn, 0, SearchTerm, True, {10}).List
                            For Each dr As DataRow In dt.Rows
                                ret.Add(New AutoComplete() With {
                                    .text = dr("UserName"),
                                    .id = "{""id"": " & dr("Id") & ", ""type"": " & MessageUserTypes.User & "}"
                                })
                            Next
                        End Using
                    End If

                    ''examination
                    If Not UserTypeRoleController.UserInType(UserId, UserTypes.StudentAffairs, conn) AndAlso Not UserTypeRoleController.UserInType(UserId, UserTypes.EmployeesSupervisor, conn) Then
                        Using dt As DataTable = UserController.GetCollection(conn, 0, SearchTerm, True, {11}).List
                            For Each dr As DataRow In dt.Rows
                                ret.Add(New AutoComplete() With {
                                    .text = dr("UserName"),
                                    .id = "{""id"": " & dr("Id") & ", ""type"": " & MessageUserTypes.User & "}"
                                })
                            Next
                        End Using
                    End If

                    ''employees affair
                    If Not UserTypeRoleController.UserInType(UserId, UserTypes.StudentAffairs, conn) AndAlso Not UserTypeRoleController.UserInType(UserId, UserTypes.ExamSupervisor, conn) AndAlso Not UserTypeRoleController.UserInType(UserId, UserTypes.EmployeesSupervisor, conn) Then
                        Using dt As DataTable = UserController.GetCollection(conn, 0, SearchTerm, True, {12}).List
                            For Each dr As DataRow In dt.Rows
                                ret.Add(New AutoComplete() With {
                                    .text = dr("UserName"),
                                    .id = "{""id"": " & dr("Id") & ", ""type"": " & MessageUserTypes.User & "}"
                                })
                            Next
                        End Using
                    End If

                    ''to all users by role
                    If UserTypeRoleController.UserInType(UserId, UserTypes.SystemAdmin) Then
                        Using dt As DataTable = RoleController.List(conn, False, SearchTerm).List
                            For Each dr As DataRow In dt.Rows
                                ret.Add(New AutoComplete() With {
                                    .text = dr("Title"),
                                    .id = "{""id"": " & dr("Id") & ", ""type"": " & MessageUserTypes.AllRoleUsers & "}"
                                })
                            Next
                        End Using
                        ret.Add(New AutoComplete() With {
                            .text = "All Users | جميع المستخدمين",
                            .id = "{""id"": 0, ""type"": " & MessageUserTypes.AllUsers & "}"
                        })
                    End If
            End Select
            Return ret
        End Function

        Private Function GetClassTemplates(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim lst As New List(Of AutoComplete)
            For Each item As ExamTemplateObject In StudyClassController.GetTemplateObjects(ClassId, langId, conn)
                lst.Add(New AutoComplete() With {
                    .id = item.Id,
                    .text = item.Title
                })
            Next
            Return lst
        End Function

        Private Function GetTemplateMaxMark(ByVal conn As SqlConnection, ByVal langId As Integer) As Integer
            Return ExamTemplateController.GetMaxMark(TemplateId, conn)
        End Function

        Private Function GetTeacherClasses(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            'UserId, Search
            Dim lst As New List(Of AutoComplete)
            For Each i As ClassObject In StudyClassController.GetTeacherClasses(conn, UserId, langId, SearchTerm)
                lst.Add(New AutoComplete() With {
                    .id = i.Id,
                    .text = i.Title
                })
            Next
            Return lst
        End Function

        Private Function GetTeacherSections(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim lst As New List(Of AutoComplete)
            For Each i As SectionObject In SectionController.GetTeacherSections(conn, UserId, langId, "", ClassId)
                lst.Add(New AutoComplete() With {
                    .id = i.Id,
                    .text = i.Title
                })
            Next
            Return lst
        End Function

        Private Function GetTeacherStudents(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim lst As New List(Of AutoComplete)
            Using dt As DataTable = StudentController.GetTeacherStudents(conn, UserId, SearchTerm, True).List
                For Each dr As DataRow In dt.Rows
                    lst.Add(New AutoComplete() With {
                        .id = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                        .text = Helper.GetSafeDBValue(dr("IdName"))
                    })
                Next
            End Using
            Return lst
        End Function

        Private Function GetTeacherMaterials(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim lst As New List(Of AutoComplete)
            Using dt As DataTable = MaterialController.GetTeacherMaterials(conn, UserId, langId, ClassId, SearchTerm, SectionId).List
                For Each dr As DataRow In dt.Rows
                    lst.Add(New AutoComplete() With {
                        .id = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                        .text = Helper.GetSafeDBValue(dr("MaterialTitle"))
                    })
                Next
            End Using
            Return lst
        End Function

        Private Function GetMaterialExams(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim lst As New List(Of AutoComplete)
            Dim materialid As Integer = GetSafeRequestValue("material_id", ValueTypes.TypeInteger)
            Dim allowNull As Boolean = GetSafeRequestValue("allownull", ValueTypes.TypeBoolean)
            Dim numbers As Boolean = GetSafeRequestValue("onlynumbers", ValueTypes.TypeBoolean)
            If allowNull Then
                lst.Add(New AutoComplete() With {.id = "0", .text = Localization.GetResource("Resources.Global.CMS.All")})
            End If
            Dim templateId As Integer = MaterialController.GetTemplateId(MyConn, materialid)
            Dim templates = ExamTemplateItemController.GetTemplateItems(templateId, LanguageId, MyConn, numbers)
            For Each i In templates
                lst.Add(New AutoComplete() With {.id = i.Id, .text = i.Title})
            Next
            Return lst
        End Function

        Private Function GetClassExamTemplateItems(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of AutoComplete)
            Dim lst As New List(Of AutoComplete)
            Dim classId As Integer = GetSafeRequestValue("class_id", ValueTypes.TypeInteger)
            Using materials As DataTable = MaterialController.GetTeacherMaterials(conn, 0, langId, classId).List
                For Each material As DataRow In materials.Rows
                    Dim templateId As Integer = MaterialController.GetTemplateId(conn, Safe(material("Id"), ValueTypes.TypeInteger))
                    For Each templateitem In ExamTemplateItemController.GetTemplateItems(templateId, langId, conn, False, 0, True)
                        If Not lst.Exists(Function(x) x.id = templateitem.Id) Then
                            lst.Add(New AutoComplete() With {.id = templateitem.Id, .text = templateitem.Title})
                        End If
                    Next
                Next
            End Using
            Return lst
        End Function

#End Region

    End Class

End Namespace
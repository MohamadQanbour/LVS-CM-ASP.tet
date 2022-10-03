Imports EGV.Utils
Imports EGV.Business
Imports EGV.Enums
Imports System.Data.SqlClient
Imports Microsoft.Office.Interop
Imports System.Data.OleDb
Imports EGV
Imports System.IO
Imports System.IO.Compression

Namespace Ajax
    Public Class Import
        Inherits AjaxBaseClass

#Region "Event Handlers"

        Public Overrides Function ProcessAjaxRequest(conn As SqlConnection, Optional langId As Integer = 0) As Object
            MyBase.ProcessAjaxRequest(conn, langId)
            Dim ret As Object = Nothing
            Select Case TargetFunction
                Case "PaymentsTotalRecords"
                    ret = GetPaymentTotalRecords(MyConn, LanguageId)
                Case "PaymentsImport"
                    ret = ImportPaymentRecords(MyConn, LanguageId)
                Case "PaymentFileDelete"
                    ret = DeletePaymentFile(MyConn, LanguageId)
                Case "CreateNewMDBFile"
                    ret = CreateNewMDBFile()
                Case "Migrate"
                    ret = Migrate(MyConn)
                Case "MigrateList"
                    ret = GetMigrateList(MyConn)
                Case "StudentFileRecords"
                    ret = GetStudentFileTotal()
                Case "SyncStudent"
                    ret = SyncStudent(MyConn)
                Case "StudentProfileFileRecords"
                    ret = GetStudentProfileFileTotal()
                Case "SyncStudentProfile"
                    ret = SyncStudentProfile(MyConn)
                Case "StudentAccountsRecords"
                    ret = GetStudentAccounts2Total(MyConn)
                Case "SyncStudentAccount2"
                    ret = SyncStudentAccount2(MyConn)
            End Select
            Return ret
        End Function

#End Region

#Region "Private Methods"

        Private Function GetStudentAccounts2Total(ByVal conn As SqlConnection) As Object
            Dim filePath As String = GetSafeRequestValue("file")
            Dim rPath As String = Helper.Server.MapPath("~" & filePath)
            Dim ret As Integer = 0
            If File.Exists(rPath) Then
                Using tfp As New FileIO.TextFieldParser(rPath)
                    tfp.TextFieldType = FileIO.FieldType.Delimited
                    tfp.Delimiters = {","}
                    tfp.HasFieldsEnclosedInQuotes = True
                    tfp.ReadLine()
                    Dim i As Integer = 0
                    While Not tfp.EndOfData
                        tfp.ReadLine()
                        i = i + 1
                    End While
                    ret = i
                End Using
            End If
            Return ret
        End Function

        Private Function SyncStudentAccount2(ByVal conn As SqlConnection) As Integer
            Dim filePath As String = GetSafeRequestValue("file")
            Dim pageIndex As Integer = GetSafeRequestValue("page", ValueTypes.TypeInteger)
            Dim numOfRecords As Integer = GetSafeRequestValue("step", ValueTypes.TypeInteger)
            Dim userId As Integer = GetSafeRequestValue("user", ValueTypes.TypeInteger)
            Dim rPath As String = Helper.Server.MapPath("~" & filePath)
            Dim added As Integer = 0
            Dim students As New List(Of Object)
            If File.Exists(rPath) Then
                Dim lines As New List(Of String)
                Using tfp As New FileIO.TextFieldParser(rPath)
                    tfp.TextFieldType = FileIO.FieldType.Delimited
                    tfp.Delimiters = {","}
                    tfp.HasFieldsEnclosedInQuotes = True
                    tfp.ReadLine()
                    Dim startIndex As Integer = pageIndex * numOfRecords
                    If startIndex <= 0 Then startIndex = 0
                    Dim endIndex As Integer = startIndex + numOfRecords
                    Dim i As Integer = 0
                    While Not tfp.EndOfData AndAlso i < startIndex
                        tfp.ReadLine()
                        i = i + 1
                    End While
                    i = startIndex
                    While Not tfp.EndOfData AndAlso i < endIndex
                        Dim currFields() As String = tfp.ReadFields()
                        students.Add(New With {
                            .StudentId = currFields(0),
                            .StudentName = currFields(1),
                            .ClassId = currFields(2),
                            .Requested = currFields(3),
                            .Balance = currFields(4),
                            .SendNotification = IIf(Safe(currFields(5)).ToString() = "1", True, False)
                        })
                        i = i + 1
                    End While
                End Using
                For Each s In students
                    Dim id As Integer = StudentController.GetBySchoolId(s.StudentId, conn)
                    If id > 0 Then
                        Dim classId As Integer = StudyClassController.GetIdByCode(s.ClassId, conn)
                        If classId = 0 Then classId = s.ClassId
                        Dim trans As SqlTransaction = conn.BeginTransaction()
                        Dim obj As New StudentAccount2() With {
                            .Balance = s.Balance,
                            .ClassId = classId,
                            .LastUpdateUser = userId,
                            .RequestedAmount = s.Requested,
                            .StudentId = id
                        }
                        Try
                            obj.Save(trans)
                            trans.Commit()
                        Catch ex As Exception
                            trans.Rollback()
                            Throw ex
                        End Try
                        If s.SendNotification Then
                            WebRequests.NotificationRequest.SendPayment2Notification(obj, conn)
                        End If
                    Else
                        Throw New Exception("Student " & s.StudentId & " Not Found!")
                    End If
                Next
                Return added
            Else
                Throw New Exception("File not found")
            End If
        End Function

        Private Function GetStudentFileTotal() As Integer
            Dim filePath As String = GetSafeRequestValue("file")
            Dim rPath As String = Helper.Server.MapPath("~" & filePath)
            Dim ret As Integer = 0
            If File.Exists(rPath) Then
                Using tfp As New FileIO.TextFieldParser(rPath)
                    tfp.TextFieldType = FileIO.FieldType.Delimited
                    tfp.Delimiters = {","}
                    tfp.HasFieldsEnclosedInQuotes = True
                    tfp.ReadLine()
                    Dim i As Integer = 0
                    While Not tfp.EndOfData
                        tfp.ReadLine()
                        i = i + 1
                    End While
                    ret = i
                End Using
            End If
            Return ret
        End Function

        Private Function GetStudentProfileFileTotal() As Integer
            Dim filePath As String = GetSafeRequestValue("file")
            Dim rPath As String = Helper.Server.MapPath("~" & filePath)
            Dim ret As Integer = 0
            If File.Exists(rPath) Then
                Using tfp As New FileIO.TextFieldParser(rPath)
                    tfp.TextFieldType = FileIO.FieldType.Delimited
                    tfp.Delimiters = {","}
                    tfp.HasFieldsEnclosedInQuotes = True
                    tfp.ReadLine()
                    Dim i As Integer = 0
                    While Not tfp.EndOfData
                        tfp.ReadLine()
                        i = i + 1
                    End While
                    ret = i
                End Using
            End If
            Return ret
        End Function

        Private Function SyncStudent(ByVal conn As SqlConnection) As Integer
            Dim filePath As String = GetSafeRequestValue("file")
            Dim pageIndex As Integer = GetSafeRequestValue("page", ValueTypes.TypeInteger)
            Dim numOfRecords As Integer = GetSafeRequestValue("step", ValueTypes.TypeInteger)
            Dim rPath As String = Helper.Server.MapPath("~" & filePath)
            Dim added As Integer = 0
            Dim students As New Dictionary(Of String, String)
            If File.Exists(rPath) Then
                Dim lines As New List(Of String)
                Using tfp As New FileIO.TextFieldParser(rPath)
                    tfp.TextFieldType = FileIO.FieldType.Delimited
                    tfp.Delimiters = {","}
                    tfp.HasFieldsEnclosedInQuotes = True
                    tfp.ReadLine()
                    Dim startIndex As Integer = pageIndex * numOfRecords
                    If startIndex <= 0 Then startIndex = 0
                    Dim endIndex As Integer = startIndex + numOfRecords
                    Dim i As Integer = 0
                    While Not tfp.EndOfData AndAlso i < startIndex
                        tfp.ReadLine()
                        i = i + 1
                    End While
                    i = startIndex
                    While Not tfp.EndOfData AndAlso i < endIndex
                        Dim currFields() As String = tfp.ReadFields()
                        students.Add(currFields(0), currFields(1))
                        i = i + 1
                    End While
                End Using
                For Each s In students
                    Dim id As Integer = StudentController.GetBySchoolId(s.Key, conn)
                    If id > 0 Then
                        Dim sectionId As Integer = SectionController.GetByCode(s.Value, True, conn)
                        If sectionId > 0 Then
                            StudentController.MoveToSection(id, sectionId, conn)
                            added += 1
                        Else
                            Throw New Exception("Could not find section " & s.Value)
                        End If
                    Else
                        Throw New Exception("Student " & s.Key & " Not Found!")
                    End If
                Next
                Return added
            Else
                Throw New Exception("File not found")
            End If
        End Function

        Private Function SyncStudentProfile(ByVal conn As SqlConnection) As Integer
            Dim filePath As String = GetSafeRequestValue("file")
            Dim pageIndex As Integer = GetSafeRequestValue("page", ValueTypes.TypeInteger)
            Dim numOfRecords As Integer = GetSafeRequestValue("step", ValueTypes.TypeInteger)
            Dim rPath As String = Helper.Server.MapPath("~" & filePath)
            Dim added As Integer = 0
            Dim students As New List(Of StudentProfileStruct)
            If File.Exists(rPath) Then
                Using tfp As New FileIO.TextFieldParser(rPath)
                    tfp.TextFieldType = FileIO.FieldType.Delimited
                    tfp.Delimiters = {","}
                    tfp.HasFieldsEnclosedInQuotes = True
                    tfp.ReadLine()
                    Dim startIndex As Integer = pageIndex * numOfRecords
                    If startIndex <= 0 Then startIndex = 0
                    Dim endIndex As Integer = startIndex + numOfRecords
                    Dim i As Integer = 0
                    While Not tfp.EndOfData AndAlso i < startIndex
                        tfp.ReadLine()
                        i = i + 1
                    End While
                    i = startIndex
                    While Not tfp.EndOfData AndAlso i < endIndex
                        Dim currFields() As String = tfp.ReadFields()
                        students.Add(New StudentProfileStruct() With {
                            .SchoolId = currFields(0).Trim(),
                            .FirstName = currFields(1).Trim(),
                            .LastName = currFields(2).Trim(),
                            .FatherName = currFields(3).Trim(),
                            .MotherName = currFields(4).Trim(),
                            .Area = currFields(5).Trim(),
                            .Class = currFields(6).Trim(),
                            .Section = currFields(7).Trim(),
                            .Email = currFields(8).Trim(),
                            .RecordNumber = currFields(9).Trim(),
                            .Religion = currFields(10).Trim(),
                            .Gender = currFields(11).Trim(),
                            .BirthInfo = currFields(12).Trim(),
                            .PhoneNumber = currFields(13).Trim(),
                            .FatherPhoneNumber = currFields(14).Trim(),
                            .MotherPhoneNumber = currFields(15).Trim(),
                            .LandlinePhoneNumber = currFields(16).Trim(),
                            .FatherWork = currFields(17).Trim(),
                            .MotherWork = currFields(18).Trim()
                        })
                        i = i + 1
                    End While
                End Using
                For Each s In students
                    Dim id As Integer = StudentController.GetBySchoolId(s.SchoolId, conn)
                    If id > 0 Then
                        'update
                        Dim obj As New Student(id, conn)
                        obj.AreaId = AreaController.GetId(s.Area, LanguageId, conn)
                        obj.BirthInfo = s.BirthInfo
                        obj.Email = s.Email
                        obj.FatherName = s.FatherName
                        obj.FatherPhoneNumber = s.FatherPhoneNumber
                        obj.FullName = s.FirstName & " " & s.LastName
                        obj.Gender = s.Gender
                        obj.LandlinePhoneNumber = s.LandlinePhoneNumber
                        obj.ModifiedUser = Helper.CMSAuthUser.Id
                        obj.MotherName = s.MotherName
                        obj.MotherPhoneNumber = s.MotherPhoneNumber
                        obj.PhoneNumber = s.PhoneNumber
                        obj.RecordNumber = s.RecordNumber
                        obj.Religion = s.Religion
                        obj.FatherWork = s.FatherWork
                        obj.MotherWork = s.MotherWork
                        obj.SectionId = SectionController.GetByCode(s.Section, True, conn)
                        If obj.SectionId = 0 Then obj.SectionId = SectionController.GetByTitle(s.Section, LanguageId, True, conn)
                        obj.FamilyId = FamilyController.GetIdByName(s.FatherName & " و" & s.MotherName & " " & s.LastName, conn)
                        If obj.FamilyId = 0 Then
                            Dim q As String = "SELECT Id FROM MEM_Family WHERE SchoolId = @SchoolId AND IsActive = 1"
                            Dim famid As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, "Fam-" & obj.SchoolId, 50)), ValueTypes.TypeInteger)
                            If famid = 0 Then
                                Dim objFamily As New Family() With {
                                    .Email = "",
                                    .FullName = s.FatherName & " و" & s.MotherName & " " & s.LastName,
                                    .IsActive = True,
                                    .Password = "Fam-" & s.SchoolId,
                                    .SchoolId = "Fam-" & s.SchoolId
                                }
                                Dim t1 As SqlTransaction = conn.BeginTransaction()
                                Try
                                    objFamily.Save(t1)
                                    t1.Commit()
                                Catch ex As Exception
                                    t1.Rollback()
                                    Throw ex
                                End Try
                                obj.FamilyId = objFamily.Id
                            Else
                                obj.FamilyId = famid
                            End If
                        End If
                        Dim t As SqlTransaction = conn.BeginTransaction()
                        Try
                            obj.Save(t)
                            t.Commit()
                        Catch ex As Exception
                            t.Rollback()
                            Throw ex
                        End Try
                    Else
                        'insert
                        Dim obj As New Student(id, conn)
                        obj.CreatedUser = Helper.CMSAuthUser.Id
                        obj.Password = s.SchoolId
                        obj.SchoolId = s.SchoolId
                        obj.AreaId = AreaController.GetId(s.Area, LanguageId, conn)
                        obj.BirthInfo = s.BirthInfo
                        obj.Email = s.Email
                        obj.FatherName = s.FatherName
                        obj.FatherPhoneNumber = s.FatherPhoneNumber
                        obj.FullName = s.FirstName & " " & s.LastName
                        obj.Gender = s.Gender
                        obj.LandlinePhoneNumber = s.LandlinePhoneNumber
                        obj.MotherName = s.MotherName
                        obj.MotherPhoneNumber = s.MotherPhoneNumber
                        obj.PhoneNumber = s.PhoneNumber
                        obj.RecordNumber = s.RecordNumber
                        obj.Religion = s.Religion
                        obj.FatherWork = s.FatherWork
                        obj.MotherWork = s.MotherWork
                        obj.IsActive = True
                        obj.SectionId = SectionController.GetByCode(s.Section, True, conn)
                        If obj.SectionId = 0 Then obj.SectionId = SectionController.GetByTitle(s.Section, LanguageId, True, conn)
                        obj.FamilyId = FamilyController.GetIdByName(s.FatherName & " و" & s.MotherName & " " & s.LastName, conn)
                        If obj.FamilyId = 0 Then
                            Dim q As String = "SELECT Id FROM MEM_Family WHERE SchoolId = @SchoolId AND IsActive = 1"
                            Dim famid As Integer = Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("SchoolId", SqlDbType.NVarChar, "Fam-" & obj.SchoolId, 50)), ValueTypes.TypeInteger)
                            If famid = 0 Then
                                Dim objFamily As New Family() With {
                                    .Email = "",
                                    .FullName = s.FatherName & " و" & s.MotherName & " " & s.LastName,
                                    .IsActive = True,
                                    .Password = "Fam-" & s.SchoolId,
                                    .SchoolId = "Fam-" & s.SchoolId
                                }
                                Dim t1 As SqlTransaction = conn.BeginTransaction()
                                Try
                                    objFamily.Save(t1)
                                    t1.Commit()
                                Catch ex As Exception
                                    t1.Rollback()
                                    Throw ex
                                End Try
                                obj.FamilyId = objFamily.Id
                            Else
                                obj.FamilyId = famid
                            End If
                        End If
                        Dim t As SqlTransaction = conn.BeginTransaction()
                        Try
                            obj.Save(t)
                            t.Commit()
                        Catch ex As Exception
                            t.Rollback()
                            Throw ex
                        End Try
                        added = added + 1
                    End If
                Next
                Return added
            Else
                Throw New Exception("File not found")
            End If
        End Function

        Private Function GetMigrateList(ByVal conn As SqlConnection) As List(Of String)
            Dim lst As New List(Of String)
            Dim seasonId As Integer = SeasonController.GetCurrentId(conn)
            lst.AddRange({"Areas", "ClassAdmins", "Classes", "ClassTemplates", "ExamTemplateItems", "ExamTemplates", "Families", "MaterialExamTemplateItems", "Materials", "Notes", "Seasons", "SectionMaterialUsers", "Sections", "StudentAccounts"})
            Dim count As Integer = DBA.Scalar(conn, "SELECT COUNT(*) FROM MOD_StudentAttendance WHERE SeasonId = " & seasonId)
            lst.Add("StudentAttendances")
            If count > 1000 Then
                Dim pages As Integer = Math.Ceiling(count / 1000)
                For i As Integer = 1 To pages
                    lst.Add("StudentAttendances-" & i)
                Next
            End If
            count = DBA.Scalar(conn, "SELECT COUNT(*) FROM MOD_StudentExam WHERE SeasonId = " & seasonId)
            lst.Add("StudentExams")
            If count > 1000 Then
                Dim pages As Integer = Math.Ceiling(count / 1000)
                For i As Integer = 1 To pages
                    lst.Add("StudentExams-" & i)
                Next
            End If
            count = DBA.Scalar(conn, "SELECT COUNT(*) FROM MEM_StudentPayment")
            lst.Add("StudentPayments")
            If count > 1000 Then
                Dim pages As Integer = Math.Ceiling(count / 1000)
                For i As Integer = 1 To pages
                    lst.Add("StudentPayments-" & i)
                Next
            End If
            lst.AddRange({"Students", "TemplateItemRelations", "Users"})
            Return lst
        End Function

        Private Function Migrate(ByVal conn As SqlConnection) As String
            Dim stepItem As String = GetSafeRequestValue("step")
            Dim fileName As String = GetSafeRequestValue("file")
            Dim page As Integer = 0
            If stepItem.Contains("-") Then
                Dim item As String = stepItem
                stepItem = stepItem.Substring(0, stepItem.IndexOf("-"))
                page = CInt(item.Replace(stepItem & "-", "")) - 1
            End If
            Dim returnFileName As String = String.Empty
            Dim oConn As OleDbConnection = DataAccess.GetConn(fileName)
            Dim seasonId As Integer = SeasonController.GetCurrentId(conn)
            Try
                oConn.Open()
                Select Case stepItem
                    Case "Areas"
                        DataAccess.NonQuery(oConn, "DELETE FROM Areas")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT Id, Title FROM LOK_Area_Res WHERE LanguageId = @Id", DBA.CreateParameter("Id", SqlDbType.Int, LanguageId))
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO Areas (Id, Title) VALUES (@Id, @Title)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@Title", OleDb.OleDbType.VarWChar, dr("Title"), 255)
                                                    )
                            Next
                        End Using
                    Case "ClassAdmins"
                        DataAccess.NonQuery(oConn, "DELETE FROM ClassAdmins")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT ClassId, UserId FROM MOD_ClassAdmins")
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO ClassAdmins (ClassId, UserId) VALUES (@CID, @UID)",
                                                    DataAccess.CreateParameter("@CID", OleDb.OleDbType.Integer, dr("ClassId")),
                                                    DataAccess.CreateParameter("@UID", OleDb.OleDbType.Integer, dr("UserId"))
                                                    )
                            Next
                        End Using
                    Case "Classes"
                        DataAccess.NonQuery(oConn, "DELETE FROM Classes")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT C.Id, C.Code, R.Title, C.SchoolDays, C.HolidayDays FROM MOD_Class C INNER JOIN MOD_Class_Res R ON C.Id = R.Id AND R.LanguageId = @Id", DBA.CreateParameter("@Id", SqlDbType.Int, LanguageId))
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO Classes (Id, Code, Title, SchoolDays, HolidayDays) VALUES (@Id, @Code, @Title, @SD, @HD)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@Code", OleDb.OleDbType.VarWChar, dr("Code"), 255),
                                                    DataAccess.CreateParameter("@Title", OleDb.OleDbType.VarWChar, dr("Title"), 255),
                                                    DataAccess.CreateParameter("@SD", OleDb.OleDbType.Integer, dr("SchoolDays")),
                                                    DataAccess.CreateParameter("@HD", OleDb.OleDbType.Integer, dr("HolidayDays"))
                                                    )
                            Next
                        End Using
                    Case "ClassTemplates"
                        DataAccess.NonQuery(oConn, "DELETE FROM ClassTemplates")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT ClassId, TemplateId FROM MOD_ClassTemplate")
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO ClassTemplates (ClassId, TemplateId) VALUES (@CID, @TID)",
                                                    DataAccess.CreateParameter("@CID", OleDb.OleDbType.Integer, dr("ClassId")),
                                                    DataAccess.CreateParameter("@TID", OleDb.OleDbType.Integer, dr("TemplateId"))
                                                    )
                            Next
                        End Using
                    Case "ExamTemplateItems"
                        DataAccess.NonQuery(oConn, "DELETE FROM ExamTemplateItems")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT T.Id, T.TemplateId, R.Title, T.Type FROM MOD_ExamTemplateItem T INNER JOIN MOD_ExamTemplateItem_Res R ON T.Id = R.Id AND R.LanguageId = @Id", DBA.CreateParameter("Id", SqlDbType.Int, LanguageId))
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO ExamTemplateItems (Id, TemplateId, Title, Type) VALUES (@Id, @TemplateId, @Title, @Type)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@TemplateId", OleDb.OleDbType.Integer, dr("TemplateId")),
                                                    DataAccess.CreateParameter("@Title", OleDb.OleDbType.VarWChar, dr("Title"), 255),
                                                    DataAccess.CreateParameter("@Type", OleDb.OleDbType.Integer, dr("Type"))
                                                    )
                            Next
                        End Using
                    Case "ExamTemplates"
                        DataAccess.NonQuery(oConn, "DELETE FROM ExamTemplates")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT T.Id, R.Title, T.MaxMark FROM MOD_ExamTemplate T INNER JOIN MOD_ExamTemplate_Res R ON T.Id = R.Id AND R.LanguageId = @Id", DBA.CreateParameter("@Id", SqlDbType.Int, LanguageId))
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO ExamTemplates (Id, Title, MaxMark) VALUES (@Id, @Title, @MaxMark)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@Title", OleDb.OleDbType.VarWChar, dr("Title"), 255),
                                                    DataAccess.CreateParameter("@MaxMark", OleDb.OleDbType.Integer, dr("MaxMark"))
                                                    )
                            Next
                        End Using
                    Case "Families"
                        DataAccess.NonQuery(oConn, "DELETE FROM Families")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT Id, SchoolId, Email, FullName, IsActive FROM MEM_Family")
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO Families (Id, SchoolId, Email, FullName, IsActive) VALUES (@Id, @SID, @Email, @FullName, @IsActive)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@SID", OleDb.OleDbType.VarWChar, dr("SchoolId"), 255),
                                                    DataAccess.CreateParameter("@Email", OleDb.OleDbType.VarWChar, dr("Email"), 255),
                                                    DataAccess.CreateParameter("@FullName", OleDb.OleDbType.VarWChar, dr("FullName"), 255),
                                                    DataAccess.CreateParameter("@IsActive", OleDb.OleDbType.Boolean, dr("IsActive"))
                                                    )
                            Next
                        End Using
                    Case "MaterialExamTemplateItems"
                        DataAccess.NonQuery(oConn, "DELETE FROM MaterialExamTemplateItems")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT MaterialId, ExamTemplateItemId, MaxMark FROM MOD_MaterialExamTemplateItem")
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO MaterialExamTemplateItems (MaterialId, ExamTemplateItemId, MaxMark) VALUES (@MId, @EId, @MaxMark)",
                                                    DataAccess.CreateParameter("@MId", OleDb.OleDbType.Integer, dr("MaterialId")),
                                                    DataAccess.CreateParameter("@EId", OleDb.OleDbType.Integer, dr("ExamTemplateItemId")),
                                                    DataAccess.CreateParameter("@MaxMark", OleDb.OleDbType.Integer, dr("MaxMark"))
                                                    )
                            Next
                        End Using
                    Case "Materials"
                        DataAccess.NonQuery(oConn, "DELETE FROM Materials")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT M.Id, M.Code, R.Title, M.ClassId, M.MaxMark, M.ExamTemplateId FROM MOD_Material M INNER JOIN MOD_Material_Res R ON M.Id = R.Id AND R.LanguageId = @Id", DBA.CreateParameter("@Id", SqlDbType.Int, LanguageId))
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO Materials (Id, Code, Title, ClassId, MaxMark, ExamTemplateId) VALUES (@Id, @Code, @Title, @ClassId, @MaxMark, @ExamTemplateId)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@Code", OleDb.OleDbType.VarWChar, dr("Code"), 255),
                                                    DataAccess.CreateParameter("@Title", OleDb.OleDbType.LongVarWChar, dr("Title"), 255),
                                                    DataAccess.CreateParameter("@ClassId", OleDb.OleDbType.Integer, dr("ClassId")),
                                                    DataAccess.CreateParameter("@MaxMark", OleDb.OleDbType.Integer, dr("MaxMark")),
                                                    DataAccess.CreateParameter("@ExamTemplateId", OleDb.OleDbType.Integer, dr("ExamTemplateId"))
                                                    )
                            Next
                        End Using
                    Case "Notes"
                        DataAccess.NonQuery(oConn, "DELETE FROM Notes")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT Id, SenderId, StudentId, NoteType, NoteDate, NoteText FROM MOD_Note")
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO Notes (Id, SenderId, StudentId, NoteType, NoteDate, NoteText) VALUES (@Id, @SenderId, @StudentId, @NoteType, @NoteDate, @NoteText)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@SenderId", OleDb.OleDbType.Integer, dr("SenderId")),
                                                    DataAccess.CreateParameter("@StudentId", OleDb.OleDbType.Integer, dr("StudentId")),
                                                    DataAccess.CreateParameter("@NoteType", OleDb.OleDbType.Integer, dr("NoteType")),
                                                    DataAccess.CreateParameter("@NoteDate", OleDb.OleDbType.Date, dr("NoteDate")),
                                                    DataAccess.CreateParameter("@NoteText", OleDb.OleDbType.VarWChar, dr("NoteText"), 255)
                                                    )
                            Next
                        End Using
                    Case "Seasons"
                        DataAccess.NonQuery(oConn, "DELETE FROM Seasons")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT S.Id, R.Title, S.IsCurrent FROM MOD_Season S INNER JOIN MOD_Season_Res R ON S.Id = R.Id AND R.LanguageId = @Id", DBA.CreateParameter("@Id", SqlDbType.Int, LanguageId))
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO Seasons (Id, Title, IsCurrent) VALUES (@Id, @Title, @IsCurrent)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@Title", OleDb.OleDbType.VarWChar, dr("Title"), 255),
                                                    DataAccess.CreateParameter("@IsCurrent", OleDb.OleDbType.Boolean, dr("IsCurrent"))
                                                    )
                            Next
                        End Using
                    Case "SectionMaterialUsers"
                        DataAccess.NonQuery(oConn, "DELETE FROM SectionMaterialUsers")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT ClassId, SectionId, MaterialId, UserId FROM MOD_SectionMaterialUser")
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO SectionMaterialUsers (ClassId, SectionId, MaterialId, UserId) VALUES (@ClassId, @SectionId, @MaterialId, @UserId)",
                                                    DataAccess.CreateParameter("@ClassId", OleDb.OleDbType.Integer, dr("ClassId")),
                                                    DataAccess.CreateParameter("@SectionId", OleDb.OleDbType.Integer, dr("SectionId")),
                                                    DataAccess.CreateParameter("@MaterialId", OleDb.OleDbType.Integer, dr("MaterialId")),
                                                    DataAccess.CreateParameter("@UserId", OleDb.OleDbType.Integer, dr("UserId"))
                                                    )
                            Next
                        End Using
                    Case "Sections"
                        DataAccess.NonQuery(oConn, "DELETE FROM Sections")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT S.Id, S.Code, R.Title, S.ClassId, S.SeasonId, S.ScheduleFilePath FROM MOD_Section S INNER JOIN MOD_Section_Res R ON S.Id = R.Id AND R.LanguageId = @Id WHERE S.SeasonId = " & seasonId, DBA.CreateParameter("@Id", SqlDbType.Int, LanguageId))
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO Sections (Id, Code, Title, ClassId, SeasonId, ScheduleFilePath) VALUES (@Id, @Code, @Title, @ClassId, @SeasonId, @ScheduleFilePath)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@Code", OleDb.OleDbType.VarWChar, dr("Code"), 255),
                                                    DataAccess.CreateParameter("@Title", OleDb.OleDbType.VarWChar, dr("Title"), 255),
                                                    DataAccess.CreateParameter("@ClassId", OleDb.OleDbType.Integer, dr("ClassId")),
                                                    DataAccess.CreateParameter("@SeasonId", OleDb.OleDbType.Integer, dr("SeasonId")),
                                                    DataAccess.CreateParameter("@ScheduleFilePath", OleDb.OleDbType.VarWChar, dr("ScheduleFilePath"), 255)
                                                    )
                            Next
                        End Using
                    Case "StudentAccounts"
                        DataAccess.NonQuery(oConn, "DELETE FROM StudentAccounts")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT Id, StudentId, PreviousClassId, CurrentClassId, Transportation, Deposit, Subscription, Total, Discount, NetTotal, PaymentsSum, Balance FROM MEM_StudentAccount")
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO StudentAccounts (Id, StudentId, PreviousClassid, CurrentClassId, Transportation, Deposit, Subscription, Total, Discount, NetTotal, PaymentsSum, Balance) VALUES (@Id, @StudentId, @PreviousClassId, @CurrentClassId, @Transportation, @Deposit, @Subscription, @Total, @Discount, @NetTotal, @PaymentsSum, @Balance)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@StudentId", OleDb.OleDbType.Integer, dr("StudentId")),
                                                    DataAccess.CreateParameter("@PreviousClassId", OleDb.OleDbType.Integer, dr("PreviousClassId")),
                                                    DataAccess.CreateParameter("@CurrentClassId", OleDb.OleDbType.Integer, dr("CurrentClassId")),
                                                    DataAccess.CreateParameter("@Transportation", OleDb.OleDbType.Boolean, dr("Transportation")),
                                                    DataAccess.CreateParameter("@Deposit", OleDb.OleDbType.Currency, dr("Deposit")),
                                                    DataAccess.CreateParameter("@Subscription", OleDb.OleDbType.Currency, dr("Subscription")),
                                                    DataAccess.CreateParameter("@Total", OleDb.OleDbType.Currency, dr("Total")),
                                                    DataAccess.CreateParameter("@Discount", OleDb.OleDbType.Currency, dr("Discount")),
                                                    DataAccess.CreateParameter("@NetTotal", OleDb.OleDbType.Currency, dr("NetTotal")),
                                                    DataAccess.CreateParameter("@PaymentsSum", OleDb.OleDbType.Currency, dr("PaymentsSum")),
                                                    DataAccess.CreateParameter("@Balance", OleDb.OleDbType.Currency, dr("Balance"))
                                                    )
                            Next
                        End Using
                    Case "StudentAttendances"
                        If page = 0 Then DataAccess.NonQuery(oConn, "DELETE FROM StudentAttendances")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT TOP 1000 * FROM (SELECT StudentId, SchoolDate, SeasonId, IsPresent, ROW_NUMBER() OVER (ORDER BY StudentId ASC, SchoolDate ASC) AS C FROM MOD_StudentAttendance WHERE SeasonId = " & seasonId & ") A WHERE A.C > " & (1000 * page).ToString())
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO StudentAttendances (StudentId, SchoolDate, SeasonId, IsPresent) VALUES (@StudentId, @SchoolDate, @SeasonId, @IsPresent)",
                                                        DataAccess.CreateParameter("@StudentId", OleDb.OleDbType.Integer, dr("StudentId")),
                                                        DataAccess.CreateParameter("@SchoolDate", OleDb.OleDbType.Date, dr("SchoolDate")),
                                                        DataAccess.CreateParameter("@SeasonId", OleDb.OleDbType.Integer, dr("SeasonId")),
                                                        DataAccess.CreateParameter("@IsPresent", OleDb.OleDbType.Boolean, dr("IsPresent"))
                                                        )
                            Next
                        End Using
                    Case "StudentExams"
                        Threading.Thread.Sleep(1000)
                        If page = 0 Then DataAccess.NonQuery(oConn, "DELETE FROM StudentExams")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT TOP 1000 * FROM (SELECT StudentId, MaterialId, ExamId, SeasonId, Mark, ROW_NUMBER() OVER (ORDER BY StudentId ASC, MaterialId ASC, ExamId ASC) AS C FROM MOD_StudentExam WHERE SeasonId = " & seasonId & ") A WHERE A.C > " & (1000 * page).ToString())
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO StudentExams (StudentId, MaterialId, ExamId, SeasonId, Mark) VALUES (@StudentId, @MaterialId, @ExamId, @SeasonId, @Mark)",
                                                    DataAccess.CreateParameter("@StudentId", OleDb.OleDbType.Integer, dr("StudentId")),
                                                    DataAccess.CreateParameter("@MaterialId", OleDb.OleDbType.Integer, dr("MaterialId")),
                                                    DataAccess.CreateParameter("@ExamId", OleDb.OleDbType.Integer, dr("ExamId")),
                                                    DataAccess.CreateParameter("@SeasonId", OleDb.OleDbType.Integer, dr("SeasonId")),
                                                    DataAccess.CreateParameter("@Mark", OleDb.OleDbType.Integer, dr("Mark"))
                                                    )
                            Next
                        End Using
                    Case "StudentPayments"
                        If page = 0 Then DataAccess.NonQuery(oConn, "DELETE FROM StudentPayments")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT TOP 1000 * FROM (SELECT Id, StudentId, PaymentNumber, PaymentAmount, PaymentDate, ROW_NUMBER() OVER (ORDER BY Id ASC) AS C FROM MEM_StudentPayment) A WHERE A.C > " & (1000 * page).ToString())
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO StudentPayments (Id, StudentId, PaymentNumber, PaymentAmount, PaymentDate) VALUES (@Id, @StudentId, @PaymentNumber, @PaymentAmount, @PaymentDate)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@StudentId", OleDb.OleDbType.Integer, dr("StudentId")),
                                                    DataAccess.CreateParameter("@PaymentNumber", OleDb.OleDbType.Integer, dr("PaymentNumber")),
                                                    DataAccess.CreateParameter("@PaymentAmount", OleDb.OleDbType.Currency, dr("PaymentAmount")),
                                                    DataAccess.CreateParameter("@PaymentDate", OleDb.OleDbType.Date, dr("PaymentDate"))
                                                    )
                            Next
                        End Using
                    Case "Students"
                        DataAccess.NonQuery(oConn, "DELETE FROM Students")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT Id, SchoolId, FullName, FatherName, MotherName, AreaId, FamilyId, SectionId, IsActive, Email, RecordNumber, Religion, Gender, BirthInfo, PhoneNumber, FatherPhoneNumber, MotherPhoneNumber, LandlinePhoneNumber FROM MEM_Student")
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO Students (Id, SchoolId, FullName, FatherName, MotherName, AreaId, FamilyId, SectionId, IsActive, Email, RecordNumber, Religion, Gender, BirthInfo, PhoneNumber, FatherPhoneNumber, MotherPhoneNumber, LandlinePhoneNumber) VALUES (@Id, @StudentId, @FullName, @FatherName, @MotherName, @AreaId, @FamilyId, @SectionId, @IsActive, @Email, @RecordNumber, @Religion, @Gender, @BirthInfo, @PhoneNumber, @FatherPhoneNumber, @MotherPhoneNumber, @LandlinePhoneNumber)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@SchoolId", OleDb.OleDbType.VarWChar, dr("SchoolId"), 255),
                                                    DataAccess.CreateParameter("@FullName", OleDb.OleDbType.VarWChar, dr("FullName"), 255),
                                                    DataAccess.CreateParameter("@FatherName", OleDb.OleDbType.VarWChar, dr("FatherName"), 255),
                                                    DataAccess.CreateParameter("@MotherName", OleDb.OleDbType.VarWChar, dr("MotherName"), 255),
                                                    DataAccess.CreateParameter("@AreaId", OleDb.OleDbType.Integer, dr("AreaId")),
                                                    DataAccess.CreateParameter("@FamilyId", OleDb.OleDbType.Integer, dr("FamilyId")),
                                                    DataAccess.CreateParameter("@SectionId", OleDb.OleDbType.Integer, dr("SectionId")),
                                                    DataAccess.CreateParameter("@IsActive", OleDb.OleDbType.Boolean, dr("IsActive")),
                                                    DataAccess.CreateParameter("@Email", OleDb.OleDbType.VarWChar, dr("Email"), 255),
                                                    DataAccess.CreateParameter("@RecordNumber", OleDb.OleDbType.VarWChar, dr("RecordNumber"), 255),
                                                    DataAccess.CreateParameter("@Religion", OleDb.OleDbType.VarWChar, dr("Religion"), 255),
                                                    DataAccess.CreateParameter("@Gender", OleDb.OleDbType.VarWChar, dr("Gender"), 255),
                                                    DataAccess.CreateParameter("@BirthInfo", OleDb.OleDbType.VarWChar, dr("BirthInfo"), 255),
                                                    DataAccess.CreateParameter("@PhoneNumber", OleDb.OleDbType.VarWChar, dr("PhoneNumber"), 255),
                                                    DataAccess.CreateParameter("@FatherPhoneNumber", OleDb.OleDbType.VarWChar, dr("FatherPhoneNumber"), 255),
                                                    DataAccess.CreateParameter("@MotherPhoneNumber", OleDb.OleDbType.VarWChar, dr("MotherPhoneNumber"), 255),
                                                    DataAccess.CreateParameter("@LandlinePhoneNumber", OleDb.OleDbType.VarWChar, dr("LandlinePhoneNumber"), 255)
                                                    )
                            Next
                        End Using
                    Case "TemplateItemRelations"
                        DataAccess.NonQuery(oConn, "DELETE FROM TemplateItemRelations")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT SourceId, RelatedId FROM MOD_TemplateItemRelation")
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO TemplateItemRelations (SourceId, RelatedId) VALUES (@SID, @RID)",
                                                    DataAccess.CreateParameter("@SID", OleDb.OleDbType.Integer, dr("SourceId")),
                                                    DataAccess.CreateParameter("@RID", OleDb.OleDbType.Integer, dr("RelatedId"))
                                                    )
                            Next
                        End Using
                    Case "Users"
                        DataAccess.NonQuery(oConn, "DELETE FROM Users")
                        Using dt As DataTable = DBA.DataTable(conn, "SELECT U.Id, U.UserName, U.IsActive, P.FullName FROM SYS_User U LEFT JOIN SYS_UserProfile P ON U.Id = P.UserId")
                            For Each dr As DataRow In dt.Rows
                                DataAccess.NonQuery(oConn, "INSERT INTO Users (Id, UserName, IsActive, FullName) VALUES (@Id, @UserName, @IsActive, @FullName)",
                                                    DataAccess.CreateParameter("@Id", OleDb.OleDbType.Integer, dr("Id")),
                                                    DataAccess.CreateParameter("@UserName", OleDb.OleDbType.VarWChar, dr("UserName"), 255),
                                                    DataAccess.CreateParameter("@IsActive", OleDb.OleDbType.Boolean, dr("IsActive")),
                                                    DataAccess.CreateParameter("@FullName", OleDb.OleDbType.VarWChar, dr("FullName"), 255)
                                                    )
                            Next
                        End Using
                    Case "Close"
                        Try
                            IO.File.Copy(Helper.Server.MapPath("~/App_Data/" & fileName & ".mdb"), Helper.Server.MapPath("~/assets/LVS.mdb"), True)
                            Dim fileToCompress As New FileInfo(Helper.Server.MapPath("~/assets/LVS.mdb"))
                            Using orignalFileStream As FileStream = fileToCompress.OpenRead()
                                Using compressedFileStream As FileStream = File.Create(fileToCompress.FullName.Replace(".mdb", "") & ".gz")
                                    Using compressionStream As New GZipStream(compressedFileStream, CompressionMode.Compress)
                                        orignalFileStream.CopyTo(compressionStream)
                                    End Using
                                End Using
                            End Using
                            returnFileName = "/assets/LVS.gz"
                        Catch ex As Exception
                        End Try
                End Select
            Catch ex As Exception
                Throw ex
            Finally
                oConn.Close()
            End Try
            Return returnFileName
        End Function

        Private Function CreateNewMDBFile() As String
            For Each f In IO.Directory.GetFiles(Helper.Server.MapPath("~/App_Data/"))
                If f <> Helper.Server.MapPath("~/App_Data/LVS.mdb") Then
                    Try
                        IO.File.Delete(f)
                    Catch ex As Exception
                    End Try
                End If
            Next
            Dim fileName As String = "LVS" & Now.ToString("yyyyMMddhhmmss")
            Dim shouldReturn As Boolean = True
            Try
                IO.File.Copy(Helper.Server.MapPath("~/App_Data/LVS.mdb"), Helper.Server.MapPath("~/App_Data/" & fileName & ".mdb"))
            Catch ex As Exception
                shouldReturn = False
                Throw ex
            End Try
            Return fileName
        End Function

        Private Function DeletePaymentFile(ByVal conn As SqlConnection, ByVal langId As Integer) As Boolean
            Dim filePath As String = GetSafeRequestValue("file")
            Dim rPath As String = Helper.Server.MapPath("~" & filePath)
            Try
                IO.File.Delete(rPath)
            Catch ex As Exception
            End Try
            Return True
        End Function

        Private Function GetPaymentTotalRecords(ByVal conn As SqlConnection, ByVal langId As Integer) As Integer
            Dim filePath As String = GetSafeRequestValue("file")
            Dim rPath As String = Helper.Server.MapPath("~" & filePath)
            Dim ret As Integer = 0
            If IO.File.Exists(rPath) Then
                Try
                    Using sr As New IO.StreamReader(rPath)
                        sr.ReadLine()
                        Dim i As Integer = 0
                        While Not sr.EndOfStream
                            sr.ReadLine()
                            i = i + 1
                        End While
                        ret = i
                    End Using
                Catch ex As Exception
                    Throw ex
                End Try
            End If
            Return ret
        End Function

        Private Function ImportPaymentRecords(ByVal conn As SqlConnection, ByVal langId As Integer) As List(Of String)
            Threading.Thread.Sleep(1000)
            Dim filePath As String = GetSafeRequestValue("file")
            Dim numOfRecords As Integer = 10
            Dim pageIndex As Integer = GetSafeRequestValue("page", ValueTypes.TypeInteger)
            Dim rPath As String = Helper.Server.MapPath("~" & filePath)
            Dim lst As New List(Of String)
            Dim items As New List(Of StudentAccountStruct)
            If IO.File.Exists(rPath) Then
                Try
                    Dim lines As New List(Of String)
                    Using tfp As New FileIO.TextFieldParser(rPath)
                        tfp.TextFieldType = FileIO.FieldType.Delimited
                        tfp.Delimiters = {","}
                        tfp.HasFieldsEnclosedInQuotes = True
                        tfp.ReadLine()
                        Dim startIndex As Integer = pageIndex * numOfRecords
                        If startIndex <= 0 Then startIndex = 0
                        Dim endIndex As Integer = startIndex + numOfRecords
                        Dim i As Integer = 0
                        While Not tfp.EndOfData AndAlso i < startIndex
                            tfp.ReadLine()
                            i = i + 1
                        End While
                        i = startIndex
                        While Not tfp.EndOfData AndAlso i < endIndex
                            Dim curr() As String = tfp.ReadFields()
                            Dim std As New StudentAccountStruct()
                            Dim schoolId As String = Safe(curr(0))
                            If schoolId <> String.Empty Then
                                Dim objStudent = GetStudent(schoolId, conn)
                                If objStudent IsNot Nothing AndAlso objStudent.Id > 0 Then
                                    std.StudentId = objStudent.Id
                                    Dim oldClass As String = Safe(curr(3))
                                    Dim curClass As String = Safe(curr(4))
                                    std.CurrentClassId = StudyClassController.GetIdByTitle(curClass, conn)
                                    std.PreviousClassId = StudyClassController.GetIdByTitle(oldClass, conn)
                                    If std.CurrentClassId = 0 Then std.CurrentClassId = StudyClassController.GetIdByCode(curClass, conn)
                                    If std.PreviousClassId = 0 Then std.PreviousClassId = StudyClassController.GetIdByCode(oldClass, conn)
                                    With std
                                        .TransportationValue = Safe(curr(5), ValueTypes.TypeDecimal)
                                        .Transportation = Safe(curr(6), ValueTypes.TypeInteger) > 0
                                        .Deposit = 0D
                                        .Subscription = Safe(curr(7), ValueTypes.TypeDecimal)
                                        .Total = Safe(curr(8), ValueTypes.TypeDecimal)
                                        .Discount = Safe(curr(9), ValueTypes.TypeDecimal)
                                        .NetTotal = Safe(curr(10), ValueTypes.TypeDecimal)
                                    End With
                                    std.Payments = New List(Of StudentPaymentStruct)()
                                    For j As Integer = 1 To 4
                                        Dim base As Integer = 11 + ((j - 1) * 3) - 1
                                        Try
                                            Dim payment As Decimal = Safe(curr(base + 1), ValueTypes.TypeDecimal)
                                            If payment > 0 Then
                                                Dim pitem As New StudentPaymentStruct()
                                                pitem.PaymentAmount = payment
                                                If Safe(curr(base + 3)) <> String.Empty Then pitem.PaymentDate = Helper.ParseDate(Safe(curr(base + 3)), "M/d/yyyy") Else pitem.PaymentDate = Now
                                                pitem.PaymentNote = Safe(curr(base + 2))
                                                pitem.PaymentNumber = j
                                                std.Payments.Add(pitem)
                                            End If
                                        Catch ex As Exception
                                            lst.Add(String.Format("Could not read payment number {0} of student account {1}", j, schoolId))
                                        End Try
                                    Next
                                    items.Add(std)
                                Else
                                    lst.Add(String.Format("Could not find student account {0}", schoolId))
                                End If
                            End If
                        End While
                    End Using
                    For Each item As StudentAccountStruct In items
                        Dim acc As New StudentAccount()
                        If Not StudentAccountController.StudentIdExists(item.StudentId, conn) Then
                            acc = New StudentAccount(0, conn)
                            acc.StudentId = item.StudentId
                        Else
                            acc = StudentAccountController.GetByStudentId(item.StudentId, conn)
                        End If
                        acc.CurrentClassId = item.CurrentClassId
                        acc.PreviousClassId = item.PreviousClassId
                        acc.Transportation = item.Transportation
                        acc.TransportationValue = item.TransportationValue
                        acc.Deposit = item.Deposit
                        acc.Subscription = item.Subscription
                        acc.Total = item.Total
                        acc.Discount = item.Discount
                        acc.NetTotal = item.NetTotal
                        acc.PaymentsSum = 0
                        Dim trans As SqlTransaction = conn.BeginTransaction()
                        Try
                            acc.Save(trans)
                            trans.Commit()
                        Catch ex As Exception
                            trans.Rollback()
                            lst.Add(String.Format("Could not insert or update student account {0}", acc.StudentObject.SchoolId))
                        End Try
                        Dim totalNewPayments As Decimal = 0D
                        For Each itemPay As StudentPaymentStruct In item.Payments
                            If Not StudentPaymentController.PaymentExists(acc.StudentId, itemPay.PaymentDate, itemPay.PaymentAmount, conn) Then
                                Dim pay As New StudentPayment(0, conn)
                                pay.PaymentAmount = itemPay.PaymentAmount
                                totalNewPayments += itemPay.PaymentAmount
                                pay.PaymentDate = itemPay.PaymentDate
                                pay.PaymentNote = itemPay.PaymentNote
                                pay.PaymentNumber = itemPay.PaymentNumber
                                pay.StudentId = acc.StudentId
                                Dim trans2 As SqlTransaction = conn.BeginTransaction()
                                Try
                                    pay.Save(trans2)
                                    trans2.Commit()
                                Catch ex As Exception
                                    trans2.Rollback()
                                    lst.Add(String.Format("Could not insert payment number {0} for student account {1}", pay.PaymentNumber, acc.StudentObject.SchoolId))
                                End Try
                            End If
                        Next
                        acc.PaymentsSum = StudentPaymentController.GetStudentPayments(acc.StudentId, conn)
                        acc.Balance = acc.NetTotal - acc.PaymentsSum
                        Dim trans3 As SqlTransaction = conn.BeginTransaction()
                        Dim paymentSaved As Boolean = False
                        Try
                            acc.Save(trans3)
                            trans3.Commit()
                            paymentSaved = True
                        Catch ex As Exception
                            trans3.Rollback()
                            lst.Add(String.Join("Could not recalculate balance of account number {0}", acc.StudentObject.SchoolId))
                        End Try
                        If paymentSaved AndAlso totalNewPayments > 0 Then
                            'send notification
                            WebRequests.NotificationRequest.SendPaymentNotification(acc.StudentObject.FamilyId, acc.StudentId, totalNewPayments, MyConn)
                        End If
                    Next
                Catch ex As Exception
                    Throw ex
                End Try
            End If
            Return lst
        End Function

#End Region

#Region "Structure"

        Public Structure StudentAccountStruct
            Public Property Id As Integer
            Public Property StudentId As Integer
            Public Property PreviousClassId As Integer
            Public Property CurrentClassId As Integer
            Public Property Transportation As Boolean
            Public Property TransportationValue As Decimal
            Public Property Deposit As Decimal
            Public Property Subscription As Decimal
            Public Property Total As Decimal
            Public Property Discount As Decimal
            Public Property NetTotal As Decimal
            Public Property PaymentsSum As Decimal
            Public Property Balance As Decimal
            Public Property Payments As List(Of StudentPaymentStruct)
        End Structure

        Public Structure StudentPaymentStruct
            Public Property PaymentNumber As String
            Public Property PaymentAmount As Decimal
            Public Property PaymentDate As Date
            Public Property PaymentNote As String
        End Structure

        Public Structure StudentProfileStruct
            Public Property SchoolId As String
            Public Property FirstName As String
            Public Property LastName As String
            Public Property FatherName As String
            Public Property MotherName As String
            Public Property Area As String
            Public Property [Class] As String
            Public Property Section As String
            Public Property Email As String
            Public Property RecordNumber As String
            Public Property Religion As String
            Public Property Gender As String
            Public Property BirthInfo As String
            Public Property PhoneNumber As String
            Public Property FatherPhoneNumber As String
            Public Property MotherPhoneNumber As String
            Public Property LandlinePhoneNumber As String
            Public Property FatherWork As String
            Public Property MotherWork As String
        End Structure

#End Region

#Region "Helper Functions"

        Private Function GetStudent(ByVal studentId As String, ByVal conn As SqlConnection) As Student
            If studentId.Length < 10 Then
                Dim ret As Student = StudentController.GetByUsername(studentId, conn)
                If ret IsNot Nothing Then Return ret Else Return GetStudent("0" & studentId, conn)
            Else
                Return Nothing
            End If
        End Function

#End Region

    End Class
End Namespace
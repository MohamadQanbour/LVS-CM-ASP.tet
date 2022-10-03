Imports System.Data.SqlClient
Imports EGV.Utils
Imports EGV.Enums
Imports EGV.Structures

Namespace EGV
    Namespace Business

        'controller
        Public Class StudentPresentController

            Public Shared Sub AddStudentPresentDay(ByVal conn As SqlConnection, ByVal studentId As Integer,
                                                   ByVal schoolDay As Date, ByVal isPresent As Boolean)
                Dim q As String = "INSERT INTO MOD_StudentAttendance (StudentId, SchoolDate, SeasonId, IsPresent) VALUES (@StudentId, @SchoolDate, @SeasonId, @IsPresent)"
                If Not Exists(conn, studentId, schoolDay) Then
                    DBA.NonQuery(conn, q,
                                 DBA.CreateParameter("StudentId", SqlDbType.Int, studentId),
                                 DBA.CreateParameter("SchoolDate", SqlDbType.Date, schoolDay),
                                 DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn)),
                                 DBA.CreateParameter("IsPresent", SqlDbType.Bit, isPresent)
                                 )
                Else
                    q = "UPDATE MOD_StudentAttendance SET IsPresent = @IsPresent, SeasonId = @SeasonId WHERE StudentId = @StudentId AND SchoolDate = @SchoolDate"
                    DBA.NonQuery(conn, q,
                                 DBA.CreateParameter("IsPresent", SqlDbType.Bit, isPresent),
                                 DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn)),
                                 DBA.CreateParameter("StudentId", SqlDbType.Int, studentId),
                                 DBA.CreateParameter("SchoolDate", SqlDbType.Date, schoolDay)
                                 )
                End If
            End Sub

            Public Shared Sub DeleteAttendance(ByVal conn As SqlConnection, ByVal schoolDay As Date)
                Dim q As String = "DELETE FROM MOD_StudentAttendance WHERE SeasonId = @SeasonId AND SchoolDate = @Date"
                DBA.NonQuery(conn, q, DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn)), DBA.CreateParameter("Date", SqlDbType.Date, schoolDay))
            End Sub

            Public Shared Function GetAttendentStudents(ByVal conn As SqlConnection, ByVal schoolDay As Date) As List(Of Integer)
                Dim q As String = "SELECT StudentId FROM MOD_StudentAttendance WHERE SeasonId = @SeasonId AND SchoolDate = @Date AND IsPresent = @IsPresent"
                Dim lst As New List(Of Integer)
                Using dt As DataTable = DBA.DataTable(conn, q,
                                                      DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn)),
                                                      DBA.CreateParameter("Date", SqlDbType.Date, schoolDay),
                                                      DBA.CreateParameter("IsPresent", SqlDbType.Bit, True)
                                                      )
                    For Each dr As DataRow In dt.Rows
                        lst.Add(Helper.GetSafeDBValue(dr("StudentId")))
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function Exists(ByVal conn As SqlConnection, ByVal studentId As Integer,
                                          ByVal schoolDay As Date) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MOD_StudentAttendance WHERE StudentId = @StudentId AND SchoolDate = @Date"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("StudentId", SqlDbType.Int, studentId), DBA.CreateParameter("Date", SqlDbType.Date, schoolDay)) > 0
            End Function

            Public Shared Function GetStudentAttendance(ByVal conn As SqlConnection, ByVal studentId As Integer) As StudentAttendance
                Dim obj As New StudentAttendance()
                Dim q As String = "SELECT COUNT(*) FROM MOD_StudentAttendance WHERE SeasonId = @SeasonId AND StudentId = @StudentId AND IsPresent = @IsPresent"
                Dim currentSeasonId As Integer = SeasonController.GetCurrentId(conn)
                obj.PresentDays = DBA.Scalar(conn, q,
                                             DBA.CreateParameter("SeasonId", SqlDbType.Int, currentSeasonId),
                                             DBA.CreateParameter("StudentId", SqlDbType.Int, studentId),
                                             DBA.CreateParameter("IsPresent", SqlDbType.Bit, True)
                                             )
                obj.AbsentDays = DBA.Scalar(conn, q,
                                            DBA.CreateParameter("SeasonId", SqlDbType.Int, currentSeasonId),
                                            DBA.CreateParameter("StudentId", SqlDbType.Int, studentId),
                                            DBA.CreateParameter("IsPresent", SqlDbType.Bit, False)
                                            )
                q = "SELECT SchoolDate FROM MOD_StudentAttendance WHERE SeasonId = @SeasonId AND StudentId = @StudentId AND IsPresent = @IsPresent"
                obj.AbsentDates = New List(Of String)()
                Using dt As DataTable = DBA.DataTable(conn, q,
                                                      DBA.CreateParameter("SeasonId", SqlDbType.Int, currentSeasonId),
                                                      DBA.CreateParameter("StudentId", SqlDbType.Int, studentId),
                                                      DBA.CreateParameter("IsPresent", SqlDbType.Bit, False)
                                                      )
                    For Each dr As DataRow In dt.Rows
                        obj.AbsentDates.Add(CDate(Helper.GetSafeDBValue(dr("SchoolDate"), ValueTypes.TypeDate)).ToString("MMMM dd, yyyy"))
                    Next
                End Using
                Return obj
            End Function

            Public Shared Function GetClassesOfAdmin(ByVal conn As SqlConnection, ByVal userId As Integer,
                                                     Optional ByVal langId As Integer = 0) As DBAReturnObject
                Dim ret As New DBAReturnObject
                Helper.GetSafeLanguageId(langId)
                Dim isAdmin As Boolean = UserController.UserInRole(conn, userId, 1)
                Dim q As String = "SELECT C.Id, C.Title FROM MOD_Class_Res C WHERE Id IN (SELECT S.ClassId FROM MOD_Section S RIGHT JOIN MOD_ClassAdmins U ON S.Id = U.ClassId WHERE U.UserId = @Id) AND C.LanguageId = @LanguageId"
                If isAdmin Then q = "SELECT C.Id, C.Title FROM MOD_Class_Res C WHERE C.LanguageId = @LanguageId"
                Dim dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("LanguageId", SqlDbType.Int, langId), DBA.CreateParameter("Id", SqlDbType.Int, userId))
                ret.Query = q
                ret.List = dt
                ret.Count = dt.Rows.Count
                Return ret
            End Function

            Public Shared Function GetSectionsOfAdmin(ByVal conn As SqlConnection, ByVal userId As Integer, Optional ByVal classId As Integer = 0,
                                                      Optional ByVal langId As Integer = 0) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                Helper.GetSafeLanguageId(langId)
                Dim isAdmin As Boolean = UserController.UserInRole(conn, userId, 1)
                Dim q As String = "SELECT S.Id, R.Title FROM MOD_Section S LEFT JOIN MOD_Section_Res R ON S.Id = R.Id AND R.LanguageId = @LanguageId WHERE S.SeasonId = @CurSeason AND S.Id IN (SELECT ClassId FROM MOD_ClassAdmins WHERE UserId = @Id)"
                If isAdmin Then q = "SELECT S.Id, R.Title FROM MOD_Section S LEFT JOIN MOD_Section_Res R ON S.Id = R.ID AND R.LanguageId = @LanguageId WHERE S.SeasonId = @CurSeason"
                If classId > 0 Then q = q & " AND S.ClassId = @ClassId"
                Dim dt As DataTable = DBA.DataTable(conn, q, DBA.CreateParameter("LanguageId", SqlDbType.Int, langId), DBA.CreateParameter("Id", SqlDbType.Int, userId), DBA.CreateParameter("ClassId", SqlDbType.Int, classId), DBA.CreateParameter("CurSeason", SqlDbType.Int, SeasonController.GetCurrentId(conn)))
                ret.Query = q
                ret.List = dt
                ret.Count = dt.Rows.Count
                Return ret
            End Function

            Public Shared Function GetSectionStudentsAttendance(ByVal conn As SqlConnection, ByVal sectionId As Integer, ByVal targetDate As Date) As List(Of SectionStudentAttendance)
                Dim lst As New List(Of SectionStudentAttendance)
                Dim q As String = "SELECT S.Id, S.SchoolId, S.FullName, CASE WHEN A.IsPresent IS NULL THEN CONVERT(BIT, 1) ELSE A.IsPresent END AS IsPresent FROM MEM_Student S LEFT JOIN MOD_StudentAttendance A ON S.Id = A.StudentId AND A.SchoolDate = @Date AND A.SeasonId = @SeasonId WHERE S.SectionId = @SectionId ORDER BY S.FullName"
                Using dt As DataTable = DBA.DataTable(conn, q,
                                                      DBA.CreateParameter("Date", SqlDbType.Date, targetDate),
                                                      DBA.CreateParameter("SeasonId", SqlDbType.Int, SeasonController.GetCurrentId(conn)),
                                                      DBA.CreateParameter("SectionId", SqlDbType.Int, sectionId)
                                                      )
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New SectionStudentAttendance() With {
                            .StudentId = Helper.GetSafeDBValue(dr("Id"), ValueTypes.TypeInteger),
                            .StudentName = Helper.GetSafeDBValue(dr("SchoolId")) & " - " & Helper.GetSafeDBValue(dr("FullName")),
                            .StudentAttend = Helper.GetSafeDBValue(dr("IsPresent"), ValueTypes.TypeBoolean)
                        })
                    Next
                End Using
                Return lst
            End Function

        End Class

    End Namespace
End Namespace
Imports System.Data.SqlClient
Imports EGV.Enums
Imports EGV.Utils
Imports EGV.Structures

Namespace EGV
    Namespace Business

        'object
        Public Class StudentPayment
            Inherits BusinessBase

#Region "Public Properties"

            Public Property Id As Integer = 0
            Public Property StudentId As Integer = 0
            Public Property PaymentNumber As Integer = 0
            Public Property PaymentAmount As Decimal = 0D
            Public Property PaymentDate As Date
            Public Property PaymentNote As String = String.Empty

#End Region

#Region "Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    FillObject(DBA.DataRow(MyConn, "SELECT * FROM MEM_StudentPayment WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                End If
            End Sub

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    StudentId = Safe(dr("StudentId"), ValueTypes.TypeInteger)
                    PaymentNumber = Safe(dr("PaymentNumber"), ValueTypes.TypeInteger)
                    PaymentAmount = Safe(dr("PaymentAmount"), ValueTypes.TypeDecimal)
                    PaymentDate = Safe(dr("PaymentDate"), ValueTypes.TypeDate)
                    PaymentNote = Safe(dr("PaymentNote"))
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MEM_StudentPayment_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("StudentId", SqlDbType.Int, StudentId),
                                  DBA.CreateParameter("PaymentNumber", SqlDbType.Int, PaymentNumber),
                                  DBA.CreateParameter("PaymentAmount", SqlDbType.Decimal, PaymentAmount, 18, 2),
                                  DBA.CreateParameter("PaymentDate", SqlDbType.Date, PaymentDate),
                                  DBA.CreateParameter("PaymentNote", SqlDbType.NVarChar, PaymentNote, 50)
                                  )
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MEM_StudentPayment_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("PaymentNumber", SqlDbType.Int, PaymentNumber),
                               DBA.CreateParameter("PaymentAmount", SqlDbType.Decimal, PaymentAmount, 18, 2),
                               DBA.CreateParameter("PaymentDate", SqlDbType.Date, PaymentDate),
                               DBA.CreateParameter("PaymentNote", SqlDbType.NVarChar, PaymentNote, 50),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                StudentPaymentController.Delete(Id, conn)
            End Sub

#End Region

        End Class

        'controller
        Public Class StudentPaymentController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM MEM_StudentPayment WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetStudentPayments(ByVal studentId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Decimal
                Dim q As String = "SELECT SUM(PaymentAmount) FROM MEM_StudentPayment WHERE StudentId = @Id"
                Return Helper.GetSafeDBDecimal(DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId)))
            End Function

            Public Shared Function GetCollection(ByVal studentId As Integer, Optional ByVal conn As SqlConnection = Nothing) As DBAReturnObject
                Dim ret As New DBAReturnObject()
                ret.Query = "SELECT * FROM MEM_StudentPayment WHERE StudentId = @Id ORDER BY PaymentNumber ASC"
                ret.List = DBA.DataTable(conn, ret.Query, DBA.CreateParameter("Id", SqlDbType.Int, studentId))
                ret.Count = ret.List.Rows.Count
                Return ret
            End Function

            Public Shared Function GetItemCollections(ByVal studentId As Integer, Optional ByVal conn As SqlConnection = Nothing) As List(Of StudentPaymentItem)
                Dim lst As New List(Of StudentPaymentItem)
                Using dt As DataTable = GetCollection(studentId, conn).List
                    For Each dr As DataRow In dt.Rows
                        lst.Add(New StudentPaymentItem() With {
                            .PaymentNumber = Helper.GetSafeDBValue(dr("PaymentNumber"), ValueTypes.TypeInteger),
                            .PaymentAmount = Helper.GetSafeDBValue(dr("PaymentAmount"), ValueTypes.TypeDecimal),
                            .PaymentDate = DirectCast(Helper.GetSafeDBValue(dr("PaymentDate"), ValueTypes.TypeDate), Date).ToString("MMMM dd, yyyy")
                        })
                    Next
                End Using
                Return lst
            End Function

            Public Shared Function PaymentExists(ByVal studentId As Integer, ByVal paymentDate As DateTime, ByVal paymentAmount As Decimal, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_StudentPayment WHERE StudentId = @Id AND PaymentAmount = @Amount AND PaymentDate = @Date"
                Return Helper.GetSafeDBValue(DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId), DBA.CreateParameter("Amount", SqlDbType.Decimal, paymentAmount, 18, 2), DBA.CreateParameter("Date", SqlDbType.Date, paymentDate)), ValueTypes.TypeInteger) > 0
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return True 'no dependents
            End Function

#End Region

        End Class

    End Namespace
End Namespace
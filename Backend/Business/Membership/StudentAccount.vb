Imports EGV.Enums
Imports System.Data.SqlClient

Namespace EGV
    Namespace Business

        'object
        Public Class StudentAccount
            Inherits BusinessBase

#Region "Public Members"

            Public Property Id As Integer
            Public Property StudentId As Integer
            Public Property PreviousClassId As Integer
            Public Property CurrentClassId As Integer
            Public Property Transportation As Boolean
            Public Property Deposit As Decimal
            Public Property Subscription As Decimal
            Public Property Total As Decimal
            Public Property Discount As Decimal
            Public Property NetTotal As Decimal
            Public Property PaymentsSum As Decimal
            Public Property Balance As Decimal
            Public Property TransportationValue As Decimal = 0D

            Public Property StudentObject As Student
            Public Property PreviousClassObject As StudyClass
            Public Property CurrentClassObject As StudyClass

#End Region

#Region "Public Constructors"

            Public Sub New(Optional ByVal tid As Integer = 0, Optional ByVal conn As SqlConnection = Nothing)
                MyBase.New(conn)
                If tid > 0 Then
                    FillObject(DBA.DataRow(conn, "SELECT * FROM MEM_StudentAccount WHERE Id = @Id", DBA.CreateParameter("Id", SqlDbType.Int, tid)))
                Else
                    StudentObject = New Student(0, MyConn)
                    PreviousClassObject = New StudyClass(0, MyConn)
                    CurrentClassObject = New StudyClass(0, MyConn)
                End If
            End Sub

#End Region

#Region "Filler"

            Private Sub FillObject(ByVal dr As DataRow)
                If dr IsNot Nothing Then
                    Id = Safe(dr("Id"), ValueTypes.TypeInteger)
                    StudentId = Safe(dr("StudentId"), ValueTypes.TypeInteger)
                    PreviousClassId = Safe(dr("PreviousClassId"), ValueTypes.TypeInteger)
                    CurrentClassId = Safe(dr("CurrentClassId"), ValueTypes.TypeInteger)
                    Transportation = Safe(dr("Transportation"), ValueTypes.TypeBoolean)
                    Deposit = Safe(dr("Deposit"), ValueTypes.TypeDecimal)
                    Subscription = Safe(dr("Subscription"), ValueTypes.TypeDecimal)
                    Total = Safe(dr("Total"), ValueTypes.TypeDecimal)
                    Discount = Safe(dr("Discount"), ValueTypes.TypeDecimal)
                    NetTotal = Safe(dr("NetTotal"), ValueTypes.TypeDecimal)
                    PaymentsSum = Safe(dr("PaymentsSum"), ValueTypes.TypeDecimal)
                    Balance = Safe(dr("Balance"), ValueTypes.TypeDecimal)
                    TransportationValue = Safe(dr("TransportationValue"), ValueTypes.TypeDecimal)

                    StudentObject = New Student(StudentId, MyConn)
                    PreviousClassObject = New StudyClass(PreviousClassId, MyConn)
                    CurrentClassObject = New StudyClass(CurrentClassId, MyConn)
                End If
            End Sub

#End Region

#Region "Public Methods"

            Public Overrides Sub Delete(Optional conn As SqlConnection = Nothing)
                StudentAccountController.Delete(StudentId, conn)
            End Sub

            Public Overrides Sub Insert(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MEM_StudentAccount_Add"
                Id = DBA.SPScalar(trans, sp,
                                  DBA.CreateParameter("StudentId", SqlDbType.Int, StudentId),
                                  DBA.CreateParameter("PreviousClassId", SqlDbType.Int, PreviousClassId),
                                  DBA.CreateParameter("CurrentClassId", SqlDbType.Int, CurrentClassId),
                                  DBA.CreateParameter("Transportation", SqlDbType.Bit, Transportation),
                                  DBA.CreateParameter("Deposit", SqlDbType.Decimal, Deposit, 18, 2),
                                  DBA.CreateParameter("Subscription", SqlDbType.Decimal, Subscription, 18, 2),
                                  DBA.CreateParameter("Total", SqlDbType.Decimal, Total, 18, 2),
                                  DBA.CreateParameter("Discount", SqlDbType.Decimal, Discount, 18, 2),
                                  DBA.CreateParameter("NetTotal", SqlDbType.Decimal, NetTotal, 18, 2),
                                  DBA.CreateParameter("PaymentsSum", SqlDbType.Decimal, PaymentsSum, 18, 2),
                                  DBA.CreateParameter("Balance", SqlDbType.Decimal, Balance, 18, 2),
                                  DBA.CreateParameter("TransportationValue", SqlDbType.Decimal, TransportationValue, 18, 2)
                                  )
            End Sub

            Public Overrides Sub Save(Optional trans As SqlTransaction = Nothing)
                If Id > 0 Then Update(trans) Else Insert(trans)
            End Sub

            Public Overrides Sub Update(Optional trans As SqlTransaction = Nothing)
                Dim sp As String = "MEM_StudentAccount_Update"
                DBA.SPNonQuery(trans, sp,
                               DBA.CreateParameter("PreviousClassId", SqlDbType.Int, PreviousClassId),
                               DBA.CreateParameter("CurrentClassId", SqlDbType.Int, CurrentClassId),
                               DBA.CreateParameter("Transportation", SqlDbType.Bit, Transportation),
                               DBA.CreateParameter("Deposit", SqlDbType.Decimal, Deposit, 18, 2),
                               DBA.CreateParameter("Subscription", SqlDbType.Decimal, Subscription, 18, 2),
                               DBA.CreateParameter("Total", SqlDbType.Decimal, Total, 18, 2),
                               DBA.CreateParameter("Discount", SqlDbType.Decimal, Discount, 18, 2),
                               DBA.CreateParameter("NetTotal", SqlDbType.Decimal, NetTotal, 18, 2),
                               DBA.CreateParameter("PaymentsSum", SqlDbType.Decimal, PaymentsSum, 18, 2),
                               DBA.CreateParameter("Balance", SqlDbType.Decimal, Balance, 18, 2),
                               DBA.CreateParameter("TransportationValue", SqlDbType.Decimal, TransportationValue, 18, 2),
                               DBA.CreateParameter("Id", SqlDbType.Int, Id)
                               )
            End Sub

#End Region

        End Class

        'controller
        Public Class StudentAccountController

#Region "Public Methods"

            Public Shared Function Delete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                If AllowDelete(id, conn) Then
                    Dim q As String = "DELETE FROM MEM_StudentAccount WHERE Id = @Id"
                    DBA.NonQuery(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Shared Function GetByStudentId(ByVal studentId As Integer, Optional ByVal conn As SqlConnection = Nothing) As StudentAccount
                Dim q As String = "SELECT Id FROM MEM_StudentAccount WHERE StudentId = @Id"
                Dim id As Integer = DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId))
                If id <> Nothing AndAlso id > 0 Then
                    Return New StudentAccount(id, conn)
                Else
                    Return Nothing
                End If
            End Function

            Public Shared Function StudentIdExists(ByVal studentId As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_StudentAccount WHERE StudentId = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, studentId)) > 0
            End Function

#End Region

#Region "Private Methods"

            Private Shared Function AllowDelete(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Return Not HasPayments(id, conn)
            End Function

            Private Shared Function HasPayments(ByVal id As Integer, Optional ByVal conn As SqlConnection = Nothing) As Boolean
                Dim q As String = "SELECT COUNT(*) FROM MEM_StudentPayment WHERE Id = @Id"
                Return DBA.Scalar(conn, q, DBA.CreateParameter("Id", SqlDbType.Int, id))
            End Function

#End Region

        End Class

    End Namespace
End Namespace
Namespace EGV
    Namespace Interfaces
        Public Interface INotifier

            Sub Warning(ByVal msg As String)
            Sub Success(ByVal msg As String)
            Sub Danger(ByVal msg As String)
            Sub Notify(ByVal msg As String)
            Function HasMessage() As Boolean

        End Interface
    End Namespace
End Namespace
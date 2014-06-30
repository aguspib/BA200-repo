'AG 02/06/2014 - #1644 implements a class with semaphores used by application

Imports System.Threading


Namespace Biosystems.Ax00.Global

    Public Class GlobalSemaphores

        '***************************
        '**** GLOBAL SEMAPHOREs **** (it is a variable, not a constant)
        '***************************
        Public Shared createWSExecutionsSemaphore As New Semaphore(0, 1) 'AG 02/06/2014 - #1644 semaphore that assures the CreateWSProcess is executed only by 1 thread at any time
        '                                                                 when the semaphrore is in use the ANSPHR instruction is not got from FIFO in this cycle machine

        Public Shared createWSExecutionsQueue As Integer = 0 '0 means semaphore FREE, > 0 means semaphore BUSY (some thread is executing createWSExecutions process)
    End Class

End Namespace
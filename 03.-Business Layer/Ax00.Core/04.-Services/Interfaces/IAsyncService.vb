Namespace Biosystems.Ax00.Core.Services

    Public Interface IAsyncService
        Inherits IDisposable

        Property OnServiceStatusChange As Action(Of IServiceStatusCallback)

        Property Status As ServiceStatusEnum

        Function StartService() As Boolean
        Sub PauseService()
        Sub RestartService()

    End Interface

End Namespace

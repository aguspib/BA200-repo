Namespace Biosystems.Ax00.Core.Services

    Public Interface IAsyncService
        Inherits IDisposable

        Property OnServiceStatusChange As Action(Of IServiceStatusCallback)

        Function StartService() As Boolean

    End Interface

End Namespace

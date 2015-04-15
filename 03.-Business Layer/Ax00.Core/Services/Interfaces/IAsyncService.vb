Imports Biosystems.Ax00.Core.Services.Enums
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Services.Interfaces

    Public Interface IAsyncService
        Inherits IDisposable

        Property OnServiceStatusChange As Action(Of IServiceStatusCallback)
        Property Status As ServiceStatusEnum

        Function StartService() As Boolean
        Sub PauseService()
        Sub RestartService()

        Sub UpdateFlags(ByVal flagsDs As AnalyzerManagerFlagsDS)
        Function ExistsBottleAlarmsOrRotorIsMissing() As Boolean

    End Interface

End Namespace

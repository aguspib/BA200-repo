Namespace Biosystems.Ax00.Core.Services
    Public Enum ServiceStatusEnum

        ''' <summary>
        ''' The service has ended successfuly
        ''' </summary>
        ServiceSuccess

        ''' <summary>
        ''' The service has ended because error or exception was met
        ''' </summary>
        ServiceAborted

        ''' <summary>
        ''' The service is currently paused
        ''' </summary>
        ServicePaused

        ''' <summary>
        ''' The service has started execution
        ''' </summary>
        ServiceStarted

        ''' <summary>
        ''' The service has started execution after a Pause
        ''' </summary>
        ServiceResumed

    End Enum

End Namespace


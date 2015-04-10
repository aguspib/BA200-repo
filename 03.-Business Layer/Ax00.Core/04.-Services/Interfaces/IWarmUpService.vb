Imports Biosystems.Ax00.Core.Services.Enums

Namespace Biosystems.Ax00.Core.Services.Interfaces
    Public Interface IWarmUpService
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function StartService() As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Sub PauseService()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Sub RestartService()

        ''' <summary>
        ''' Recovers the system to a stable point after close and start application during change rotor process 'in course'
        ''' </summary>
        ''' <returns>TRUE process recovered | FALSE process could not be recovered</returns>
        ''' <remarks>
        ''' </remarks>
        Function RecoverProcess() As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Sub FinalizeProcess()

        Property ReuseRotorContentsForBaseLine As Action(Of BaseLineService.ReuseRotorResponse)
        ReadOnly Property NextStep As WarmUpStepsEnum
    End Interface
End Namespace
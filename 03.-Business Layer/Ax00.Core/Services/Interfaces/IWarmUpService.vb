Imports Biosystems.Ax00.Core.Services.Enums

Namespace Biosystems.Ax00.Core.Services.Interfaces
    Public Interface IWarmUpService
        Inherits IAsyncService

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

        Property ReuseContentsForBaseLineCallback As Action(Of BaseLineService.ReuseRotorResponse)
        ReadOnly Property NextStep As WarmUpStepsEnum
    End Interface
End Namespace
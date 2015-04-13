Imports Biosystems.Ax00.Core.Services.Enums

Namespace Biosystems.Ax00.Core.Services.Interfaces
    Public Interface IBaseLineService
        Inherits IAsyncService

        Sub EmptyAndFinalizeProcess()
        Function RecoverProcess() As Boolean
        Sub RepeatDynamicBaseLineReadStep()

        ''' <summary>
        ''' If this function is set to True, if the rotor is full of clean water that can be used to perform a FLIGHT, the process starts directly from the Read step.
        ''' This happens when a 551 status (alarm) is present and rotor contents hasn't lapsed.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property DecideToReuseRotorContents As Action(Of BaseLineService.ReuseRotorResponse)
        Property CurrentStep As BaseLineStepsEnum

    End Interface
End Namespace
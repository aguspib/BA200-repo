Namespace Biosystems.Ax00.Core.Services.Interfaces
    Public Interface IRotorChangeService
        Inherits IAsyncService
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function ContinueProcess() As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Sub RepeatDynamicBaseLineReadStep()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Sub EmptyAndFinalizeProcess()

        ''' <summary>
        ''' Recovers the system to a stable point after close and start application during change rotor process 'in course'
        ''' </summary>
        ''' <returns>TRUE process recovered | FALSE process could not be recovered</returns>
        ''' <remarks>
        ''' Modified by:  AG 20/01/2015 - BA-2216
        ''' </remarks>
        Function RecoverProcess() As Boolean
    End Interface
End Namespace

Namespace Biosystems.Ax00.Data.Interfaces
    Public Interface IvWSExecutionsDAO
        ''' <summary>
        ''' Retrieves the required washing solution, used in a given WRUN instruction
        ''' If the washing solution is distilled water, it returns 'DISTW'
        ''' Otherwise, it returns the Washing solution code.
        ''' </summary>
        ''' <param name="ExecutionID"></param>
        ''' <param name="AnalyzerID"></param>
        ''' <param name="WorkSessionID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetWashingSolution(ExecutionID As Integer, AnalyzerID As String, WorkSessionID As String) As vWSExecutionsDS
    End Interface
End Namespace
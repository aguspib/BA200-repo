Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Public Interface ItwksWSAnalyzerAlarms

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerAlarmsDS"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Create(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerAlarmsDS As WSAnalyzerAlarmsDS) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerAlarmsDS"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Update(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerAlarmsDS As WSAnalyzerAlarmsDS) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pWorkSessionID"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetByWS(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, Optional ByVal pAnalyzerID As String = "") As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <param name="pWorkSessionID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetByAnalyzer(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, Optional ByVal pWorkSessionID As String = "") As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <param name="pWorkSessionID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetCurrentActiveAlarms(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pInitialDate"></param>
    ''' <param name="pFinalDate"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetByTime(ByVal pDBConnection As SqlConnection, ByVal pInitialDate As Date, ByVal pFinalDate As Date, Optional ByVal pAnalyzerID As String = "") As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAlarmID"></param>
    ''' <param name="pInitialDate"></param>
    ''' <param name="pFinalDate"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <param name="pWorkSessionID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetByAlarmID(ByVal pDBConnection As SqlConnection, ByVal pAlarmID As String, Optional ByVal pInitialDate As Date = Nothing, Optional ByVal pFinalDate As Date = Nothing, Optional ByVal pAnalyzerID As String = Nothing, Optional ByVal pWorkSessionID As String = Nothing) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <param name="pWorkSessionID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetAlarmsMonitor(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, Optional ByVal pWorkSessionID As String = "") As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <param name="pWorkSessionID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO

End Interface

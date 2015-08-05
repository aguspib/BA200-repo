Imports Biosystems.Ax00.Global

Public Interface ItfmwAlarms

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAlarmID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmID As String) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pLanguageID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function UpdateLanguageResource(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLanguageID As String) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAlarmID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ReadManagementAlarm(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmID As String) As GlobalDataTO
End Interface

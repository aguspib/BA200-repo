Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Public Interface ItcfgAnalyzers

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
    ''' <param name="pAnalyzerID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ReadByAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzer"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzer As AnalyzersDS.tcfgAnalyzersRow) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pGeneric"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ReadByAnalyzerGeneric(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pGeneric As Boolean) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ReadByAnalyzerActive(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function DeleteConnectedAnalyzersNotActive(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function UpdateAnalyzerActive(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO

End Interface

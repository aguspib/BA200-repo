Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Public Interface ItfmwSwParameters

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pParameterName"></param>
    ''' <param name="pAnalyzerModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ReadByParameterName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pParameterName As String, ByVal pAnalyzerModel As String) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerModel"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ReadByAnalyzerModel(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerModel As String, Optional ByVal pAnalyzerID As String = "") As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <param name="pParameterName"></param>
    ''' <param name="pDependOnModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetParameterByAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pParameterName As String, ByVal pDependOnModel As Boolean) As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetAllList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pAnalyzerModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetAllISEList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pDBConnection"></param>
    ''' <param name="pParameterRow"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pParameterRow As ParametersDS.tfmwSwParametersRow) As GlobalDataTO

End Interface

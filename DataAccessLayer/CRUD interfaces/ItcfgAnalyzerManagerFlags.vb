Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Public Interface ItcfgAnalyzerManagerFlags

    Function ReadValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pFlagID As String) As GlobalDataTO

    Function ReadByAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO

    Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerFlagsDS As AnalyzerManagerFlagsDS) As GlobalDataTO

    Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerFlagsDS As AnalyzerManagerFlagsDS) As GlobalDataTO

    Function UpdateFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pFlagID As String, ByVal pValue As String) As GlobalDataTO

    Function ResetFlags(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pLeaveConnectFlag As Boolean) As GlobalDataTO

    Function ReadByStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pValue As String, ByVal pReadWithSameValue As Boolean) As GlobalDataTO

    Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO

End Interface

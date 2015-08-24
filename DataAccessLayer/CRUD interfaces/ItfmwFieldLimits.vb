Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Public Interface ItfmwFieldLimits

    Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLimitID As GlobalEnumerates.FieldLimitsEnum, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO

    Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerModel As String = "") As GlobalDataTO

    Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLimitRow As FieldLimitsDS.tfmwFieldLimitsRow) As GlobalDataTO
End Interface

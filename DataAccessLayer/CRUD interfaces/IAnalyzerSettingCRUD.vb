Imports Biosystems.Ax00.Global

''' <summary>
''' This is the interface to be implemented in any CRUD that provides data access to analyzerID based data.
''' </summary>
''' <typeparam name="TDataKind"></typeparam>
''' <remarks></remarks>
Public Interface IAnalyzerSettingCRUD(Of TDataKind As DataSet)

    Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As TypedGlobalDataTo(Of TDataKind)

    Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pEntryDS As TDataKind) As GlobalDataTO

    Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pEntryDS As TDataKind) As GlobalDataTO

    Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO

End Interface

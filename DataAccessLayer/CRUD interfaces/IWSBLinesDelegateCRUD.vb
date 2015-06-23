Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Public Interface IWSBLinesDelegateCRUD(Of T As DataSet)

    Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBaseLinesDS As T) As GlobalDataTO


    Function Read(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                  ByVal pBaseLineID As Integer, ByVal pWellUsed As Integer, ByVal pType As String) As TypedGlobalDataTo(Of T)


    Function Update(ByVal pDBConnection As SqlConnection, ByVal pBaseLinesDS As T) As GlobalDataTO

    Function Delete(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
              ByVal pBaseLineID As Integer, ByVal pWellUsed As Integer, ByVal pType As String) As TypedGlobalDataTo(Of T)


End Interface

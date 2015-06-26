Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Public Interface IWSBLinesDelegateCRUD(Of TDataKind As DataSet)

    Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBaseLinesDS As TDataKind) As GlobalDataTO


    Function Read(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                  ByVal pBaseLineID As Integer, ByVal pWellUsed As Integer, ByVal pType As String) As TypedGlobalDataTo(Of TDataKind)


    Function Update(ByVal pDBConnection As SqlConnection, ByVal pBaseLinesDS As TDataKind) As GlobalDataTO

    Function Delete(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
              ByVal pBaseLineID As Integer, ByVal pWellUsed As Integer, ByVal pType As String) As TypedGlobalDataTo(Of TDataKind)


End Interface

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Core.Entities.WorkSession
    Public interface IWSExecutionCreator
        Function CreateWS(ByVal ppDBConnection As SqlConnection, ByVal ppAnalyzerID As String, ByVal ppWorkSessionID As String, _
                                 ByVal ppWorkInRunningMode As Boolean, Optional ByVal ppOrderTestID As Integer = -1, _
                                 Optional ByVal ppPostDilutionType As String = "", Optional ByVal ppIsISEModuleReady As Boolean = False, _
                                 Optional ByVal ppISEElectrodesList As List(Of String) = Nothing, Optional ByVal ppPauseMode As Boolean = False, _
                                 Optional ByVal ppManualRerunFlag As Boolean = True) As GlobalDataTO

    End Interface
End NameSpace
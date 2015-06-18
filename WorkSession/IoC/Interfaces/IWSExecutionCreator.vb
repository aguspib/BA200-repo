Imports System.Data.SqlClient
Imports Biosystems.Ax00.Core.Entities.Worksession.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.Worksession.Interfaces
    Public Interface IWSExecutionCreator
        ''' <summary>
        ''' Returns the current active Analyzer ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>This property is ReadOnly</remarks>
        ReadOnly Property AnalyzerID As String

        ''' <summary>
        ''' Returns the current active WorkSession Id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>This property is ReadOnly</remarks>
        ReadOnly Property WorksesionID As String

        ''' <summary>
        ''' Property that defines the contamination specifications
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Implements the IAnalyzerContaminationsSpecificiation interface</remarks>
        Property ContaminationsSpecification As IAnalyzerContaminationsSpecification

        ''' <summary>
        ''' Method that creates the WorkSession to run
        ''' </summary>
        ''' <param name="ppDBConnection"></param>
        ''' <param name="ppAnalyzerID"></param>
        ''' <param name="ppWorkSessionID"></param>
        ''' <param name="ppWorkInRunningMode"></param>
        ''' <param name="ppOrderTestID"></param>
        ''' <param name="ppPostDilutionType"></param>
        ''' <param name="ppIsISEModuleReady"></param>
        ''' <param name="ppISEElectrodesList"></param>
        ''' <param name="ppPauseMode"></param>
        ''' <param name="ppManualRerunFlag"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function CreateWS(ByVal ppDBConnection As SqlConnection, ppAnalyzerID As String, ByVal ppWorkSessionID As String, _
                                 ByVal ppWorkInRunningMode As Boolean, Optional ByVal ppOrderTestID As Integer = -1, _
                                 Optional ByVal ppPostDilutionType As String = "", Optional ByVal ppIsISEModuleReady As Boolean = False, _
                                 Optional ByVal ppISEElectrodesList As List(Of String) = Nothing, Optional ByVal ppPauseMode As Boolean = False, _
                                 Optional ByVal ppManualRerunFlag As Boolean = True) As GlobalDataTO

        Function GetContaminationNumber(calculateinrunning As Boolean, previousReagentID As List(Of Integer), ByVal orderTests As IEnumerable(Of ExecutionsDS.twksWSExecutionsRow)) As Integer
    End Interface
End Namespace
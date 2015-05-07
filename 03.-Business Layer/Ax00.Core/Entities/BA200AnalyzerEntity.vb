Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports System.Data
Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Public Class BA200AnalyzerEntity
        Inherits AnalyzerManager


        Public Sub New(assemblyName As String, analyzerModel As String, baseLine As IBaseLineEntity)
            MyBase.New(assemblyName, analyzerModel, baseLine)
            WSExecutionCreator.Instance.ContaminationsSpecification = New BA200ContaminationsSpecification()
        End Sub

#Region "Overridden methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pWell"></param>
        ''' <param name="pBaseLineWithAdjust"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 29/10/2014 BA-2064 adapt method to read the static or dynamic base line
        ''' </remarks>
        Public Overrides Function GetCurrentBaseLineID(ByVal pdbConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                     ByVal pWorkSessionID As String, ByVal pWell As Integer, ByVal pBaseLineWithAdjust As Boolean) As GlobalDataTO

            Return GetCurrentBaseLineIDByType(pdbConnection, pAnalyzerID, pWorkSessionID, pWell, pBaseLineWithAdjust, BaseLineTypeForCalculations.ToString())
        End Function

#End Region

    End Class

End Namespace

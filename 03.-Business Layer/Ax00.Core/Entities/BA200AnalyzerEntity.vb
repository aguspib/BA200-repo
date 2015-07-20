Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports System.Data
Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Services.BaseLine

Namespace Biosystems.Ax00.Core.Entities

    Public Class BA200AnalyzerEntity
        Inherits AnalyzerManager
        Private BL_expListener As BaseLineExpirationListener

        Public Sub New(assemblyName As String, analyzerModel As String, baseLine As IBaseLineEntity)
            MyBase.New(assemblyName, analyzerModel, baseLine)

            WSExecutionCreator.Instance.ContaminationsSpecification = New BA200ContaminationsSpecification(Me)
            _currentAnalyzer = Me

            BL_expListener = New BaseLineExpirationListener(Me, New BaseLineEntityExpiration(_currentAnalyzer), New AnalyzerAlarms(_currentAnalyzer))

        End Sub

        ''' <summary>
        ''' Method that start The expiration listener at the same time we create and instance of one Analyzer and we connect with one instrument
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub startListenerExpiration()
            MyBase.startListenerExpiration()
            BL_expListener.StartToListening()
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

        'Public Overrides Function ContaminationsSpecification() As IAnalyzerContaminationsSpecification
        '    Return WSExecutionCreator.Instance.ContaminationsSpecification
        'End Function

        Public Overrides Function ExistBaseLineFinished() As Boolean
            Return SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "END" AndAlso
                SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "END" AndAlso
                SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "END"
        End Function

        Public Overrides Function BaseLineNotStarted() As Boolean
            Return SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "" AndAlso
                SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "" AndAlso
                SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = ""
        End Function
#End Region

        Public Overrides ReadOnly Property WashingIDRequired As Boolean
            Get
                Return True
            End Get
        End Property



    End Class

End Namespace

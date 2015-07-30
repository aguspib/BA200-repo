Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports System.Data
Imports Biosystems.Ax00.Core.Entities.WorkSession
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Services.BaseLine

Namespace Biosystems.Ax00.Core.Entities

    Public Class BA400AnalyzerEntity
        Inherits AnalyzerManager

        Public Sub New(assemblyName As String, analyzerModel As String, baseLine As IBaseLineEntity)
            MyBase.New(assemblyName, analyzerModel, baseLine)
            WSExecutionCreator.Instance.ContaminationsSpecification = New BA400ContaminationsSpecification(Me)
            _currentAnalyzer = Me

            'Dim BL_expListener = New BaseLineExpirationListener(Me)

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

            Return MyBase.GetCurrentBaseLineIDByType(pdbConnection, pAnalyzerID, pWorkSessionID, pWell, pBaseLineWithAdjust, BaseLineTypeForCalculations.ToString())
        End Function

        'Public Overrides Function ContaminationsSpecification() As IAnalyzerContaminationsSpecification
        '    Return WSExecutionCreator.Instance.ContaminationsSpecification
        'End Function

        Public Overrides Function ExistBaseLineFinished() As Boolean
            Return SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "END"
        End Function

        Public Overrides Function BaseLineNotStarted() As Boolean
            Return SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = ""
        End Function

        Public Overrides ReadOnly Property GetModelValue(ByVal pAnalyzerID As String) As String
            'AJG Pendiente de poner lo que le toca
            Get
                Dim returnValue As String = ""

                If pAnalyzerID.Length > 0 Then

                    If (pAnalyzerID = GenericDefaultAnalyzer()) Then
                        returnValue = "A400"
                    Else
                        Dim strTocompare = GetUpperPartSN(pAnalyzerID)

                        If (strTocompare = BA400ModelID) Then
                            returnValue = "A400"
                        End If
                    End If
                End If

                Return returnValue
            End Get
        End Property

        Public Overrides ReadOnly Property GenericDefaultAnalyzer() As String
            Get
                Return "SN0000099999_Ax400"
            End Get
        End Property

#End Region

        Public Overrides ReadOnly Property WashingIDRequired As Boolean
            Get
                Return False
            End Get
        End Property

    End Class

End Namespace

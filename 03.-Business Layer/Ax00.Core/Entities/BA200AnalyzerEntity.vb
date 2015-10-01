Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports System.Data
Imports Biosystems.Ax00.BL
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

            'MultilanguageResourcesDelegate.RegisterKeyword("AX00", Function() Model)

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

        Public Overrides ReadOnly Property GetModelValue(ByVal pAnalyzerID As String) As String
            Get
                Dim returnValue As String = "A400"  'AJG. Assigned a default value to avoid a MasterData error while loading an analyzer

                If pAnalyzerID.Length > 0 Then

                    If (pAnalyzerID = GenericDefaultAnalyzer()) Then
                        returnValue = "A200"
                    Else
                        Dim strTocompare = GetUpperPartSN(pAnalyzerID)

                        If (strTocompare = BA200ModelID) Then
                            returnValue = "A200"
                        End If
                    End If
                End If

                Return returnValue
            End Get
        End Property

        Public Overrides ReadOnly Property GenericDefaultAnalyzer() As String
            Get
                Return "SN0000099999_Ax200"
            End Get
        End Property

        Public Overrides ReadOnly Property GetModelCode() As String
            Get
                Return BA200ModelID
            End Get
        End Property

#End Region

        Public Overrides ReadOnly Property WashingIDRequired As Boolean
            Get
                Return True
            End Get
        End Property



        Public Overrides ReadOnly Property FirmwareFileExtension As String
            Get
                Return "*.ba2"
            End Get
        End Property

        Public Overrides Function CommercialModelName() As String
            Return "BA200"
        End Function
    End Class

End Namespace

Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.App
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities

Public Class CommunicationsTest
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Definitions"

    Private WithEvents analyzer As IAnalyzerManager = AnalyzerController.Instance.Analyzer '#REFACTORING

#End Region

    
#Region "Frame Receiving"


    ''' <summary>
    ''' Simulate instruction reception
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' 
    ''' </remarks>
    Private Sub BsButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsReceive.Click

        If (AnalyzerController.IsAnalyzerInstantiated) Then
            If AnalyzerController.Instance.Analyzer.CommThreadsStarted Then '#REFACTORING
                'Short instructions
                AnalyzerController.Instance.Analyzer.SimulateInstructionReception(SimulatedFrameReceived.Text.Trim)
            End If
        End If

    End Sub

#End Region

#Region "Alarms"

    Private Sub BsButton1_Click(sender As Object, e As EventArgs) Handles btnWaterDepositErrAlarm.Click
        Biosystems.Ax00.Core.Entities.AnalyzerManager.SimulatingFillWaterAlarm = Not Biosystems.Ax00.Core.Entities.AnalyzerManager.SimulatingFillWaterAlarm

        If Biosystems.Ax00.Core.Entities.AnalyzerManager.SimulatingFillWaterAlarm Then
            btnWaterDepositErrAlarm.Text = "Disable Simulate Alarm"
        Else
            btnWaterDepositErrAlarm.Text = "Enable Simulate Alarm"
        End If

    End Sub

#End Region


End Class
Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.App
Imports Biosystems.Ax00.Core.Interfaces

Public Class CommunicationsTest
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Definitions"

    Private WithEvents analyzer As IAnalyzerManager = AnalyzerController.Instance.Analyzer '#REFACTORING

#End Region

#Region "Receiving"


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


End Class
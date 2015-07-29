Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.App
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Public Class Ax00MainMenuForm
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    Dim LocalAnalizerDS As New AnalyzersDS

    Private Sub PosititioningToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PosititioningToolStripMenuItem.Click
        Dim myReagSampPosForm As New UiWSRotorPositions
        myReagSampPosForm.ActiveWorkSession = Me.TextBox1.Text
        If Not GetFormFromList(myReagSampPosForm.Name) Then
            myReagSampPosForm.MdiParent = Me
            myReagSampPosForm.Show()
        End If
    End Sub

    Private Sub PositioningBCNToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim myReagSampPosForm As New UiWSRotorPositions
        myReagSampPosForm.ActiveWorkSession = Me.TextBox1.Text
        If Not GetFormFromList(myReagSampPosForm.Name) Then
            myReagSampPosForm.MdiParent = Me
            myReagSampPosForm.Show()
        End If
    End Sub

    Private Sub TestProgrammingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestProgrammingToolStripMenuItem.Click
        Dim myTestProgrammingForm As New UiProgTest
        If LocalAnalizerDS.tcfgAnalyzers.Rows.Count > 0 Then
            myTestProgrammingForm.AnalyzerModel = LocalAnalizerDS.tcfgAnalyzers(0).AnalyzerModel
            myTestProgrammingForm.AnalyzerID = LocalAnalizerDS.tcfgAnalyzers(0).AnalyzerID
            myTestProgrammingForm.WorkSessionID = ""
        End If
        If Not GetFormFromList(myTestProgrammingForm.Name) Then
            myTestProgrammingForm.MdiParent = Me
            myTestProgrammingForm.Show()
        End If
    End Sub

    Private Sub WSPreparationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WSPreparationToolStripMenuItem.Click
        Dim myWSPreparationForm As New UiWSSampleRequest
        myWSPreparationForm.ActiveAnalyzer = "SN0000099999_Ax400"
        myWSPreparationForm.ActiveWorkSession = Me.TextBox1.Text

        If Not GetFormFromList(myWSPreparationForm.Name) Then
            myWSPreparationForm.MdiParent = Me
            myWSPreparationForm.Show()
        End If
    End Sub

    ''' <summary>
    ''' Fill a local structute withe information related
    ''' to the Analyzer info.
    ''' </summary>
    ''' <remarks>
    ''' CREATE BY: TR 22/03/2010.
    ''' </remarks>
    Private Sub GetAnalyzerInfo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myAnalyzerDelegate As New AnalyzersDelegate

            ' XBC 07/06/2012
            'myGlobalDataTO = myAnalyzerDelegate.GetAnalyzer(Nothing)
            myGlobalDataTO = myAnalyzerDelegate.CheckAnalyzer(Nothing)
            ' XBC 07/06/2012

            If Not myGlobalDataTO.HasError Then
                LocalAnalizerDS = CType(myGlobalDataTO.SetDatos, AnalyzersDS)
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetAnalyzerInfo ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", "SYSTEM_ERROR", ex.Message)
        End Try
    End Sub


    Private Sub Ax00MainMenuForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        GetAnalyzerInfo()
    End Sub

    'Private Sub WSPreparationTestToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Dim myWSPreparationForm As New WSPreparation_GDS 'WS_Preparation
    '    myWSPreparationForm.ActiveWorkSession = Me.TextBox1.Text

    '    If Not GetFormFromList(myWSPreparationForm.Name) Then
    '        myWSPreparationForm.MdiParent = Me
    '        myWSPreparationForm.Show()
    '    End If
    'End Sub

    Private Sub ResetWSToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetWSToolStripMenuItem.Click
        'Show the error message
        If (Me.TextBox1.Text <> "") Then
            If (MsgBox("WorkSession " & Me.TextBox1.Text & " will be delete. Continue?", MsgBoxStyle.YesNo, "Reset WS") = Windows.Forms.DialogResult.Yes) Then
                Dim myWS As New WorkSessionsDelegate
                Dim myGlobal As New GlobalDataTO

                myGlobal = myWS.ResetWS(Nothing, AnalyzerController.Instance.Analyzer.GenericDefaultAnalyzer(), TextBox1.Text, AnalyzerController.Instance.Analyzer.GetModelValue(AnalyzerController.Instance.Analyzer.GenericDefaultAnalyzer()))
                If (Not myGlobal.HasError) Then
                    MsgBox("OK ")
                Else
                    MsgBox("KO " & myGlobal.ErrorMessage)
                End If
            End If
        End If
    End Sub

    Private Sub PatientDataToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PatientDataToolStripMenuItem.Click
        If Not GetFormFromList(UiProgPatientData.Name) Then
            UiProgPatientData.MdiParent = Me
            UiProgPatientData.Show()
        End If
    End Sub

    Private Sub PatientDataSearchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PatientDataSearchToolStripMenuItem.Click
        Dim myPatientSearchForm As New UiProgPatientData

        myPatientSearchForm.EntryMode = "SEARCH"
        myPatientSearchForm.PatientID = "PAT00007"
        myPatientSearchForm.ShowDialog()
    End Sub
End Class

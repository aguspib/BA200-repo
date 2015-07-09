Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Services.BaseLine
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Services
Imports Biosystems.Ax00.BL

Public Class Form_MR

    Private Shared _analyzer As IAnalyzerManager
    Private _analyzerAlarmsManager As IAnalyzerAlarms
    Private bsExpired As BaseLineExpirationListener


    Private Sub Btn_create_refresh_Click(sender As Object, e As EventArgs) Handles Btn_create_refresh.Click


        _analyzerAlarmsManager.ActionAlarm(0, AlarmEnumerates.Alarms.BASELINE_EXPIRED)
    End Sub

    Private Sub BsDelete_refresh_Click(sender As Object, e As EventArgs) Handles BsDelete_refresh.Click
        _analyzerAlarmsManager.ActionAlarm(1, AlarmEnumerates.Alarms.BASELINE_EXPIRED)

    End Sub

    Private Sub Form_MR_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        _analyzer = AnalyzerManager.GetCurrentAnalyzerManager()
        bsExpired = New BaseLineExpirationListener(_analyzer, New BaseLineEntityExpiration(_analyzer), New AnalyzerAlarms(_analyzer))
        _analyzerAlarmsManager = New AnalyzerAlarms(AnalyzerManager.GetCurrentAnalyzerManager())

    End Sub

    Private Sub BsBtnUpdteBLexpirationdate_Click(sender As Object, e As EventArgs) Handles BsBtnUpdteBLexpirationdate.Click

        Dim blService As New BaseLineService(AnalyzerManager.GetCurrentAnalyzerManager(), New AnalyzerManagerFlagsDelegate)
        blService.UpdateAnalyzerSettings()

    End Sub
End Class
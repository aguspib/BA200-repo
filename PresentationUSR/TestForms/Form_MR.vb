Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Services.BaseLine
Imports Biosystems.Ax00.Global

Public Class Form_MR

    Private Shared _analyzer As IAnalyzerManager
    Private _analyzerAlarmsManager As IAnalyzerAlarms
    Private bsExpired As BaseLineExpirationListener


    Private Sub Btn_create_refresh_Click(sender As Object, e As EventArgs) Handles Btn_create_refresh.Click


        bsExpired.ActionAlarm(BaseLineExpirationListener.TypeActionAlarm.Creation, AlarmEnumerates.Alarms.BL_EXPIRED)
    End Sub

    Private Sub BsDelete_refresh_Click(sender As Object, e As EventArgs) Handles BsDelete_refresh.Click
        bsExpired.ActionAlarm(BaseLineExpirationListener.TypeActionAlarm.Delete, AlarmEnumerates.Alarms.BL_EXPIRED)
    End Sub

    Private Sub Form_MR_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        _analyzer = AnalyzerManager.GetCurrentAnalyzerManager()
        bsExpired = New BaseLineExpirationListener(_analyzer)

    End Sub
End Class
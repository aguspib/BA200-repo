Imports System.IO
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


        _analyzerAlarmsManager.ActionAlarm(True, AlarmEnumerates.Alarms.BASELINE_EXPIRED)
    End Sub

    Private Sub BsDelete_refresh_Click(sender As Object, e As EventArgs) Handles BsDelete_refresh.Click
        _analyzerAlarmsManager.ActionAlarm(False, AlarmEnumerates.Alarms.BASELINE_EXPIRED)

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

    Private Sub btnLogActivity_Click(sender As Object, e As EventArgs) Handles btnLogActivity.Click
        Try

            CreateLogActivity("Test Create Log Activity.", EventLogEntryType.Information)

        Catch ex As Exception
            CreateLogActivity(ex)
        End Try

        
        'CheckTemperaturesAlarms(Nothing, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE), _
        '                pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS), _
        '                pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION), _
        '                pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1), _
        '                pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2), myAlarmList, myAlarmStatusList)




    End Sub

    Private Sub BsDecription_Click(sender As Object, e As EventArgs) Handles BsDecryption.Click
        Dim mySecurity As New Security.Security
        Dim pswd As String = String.Empty
        Dim passwordText As String = String.Empty

        pswd += mySecurity.Decryption("f7fD8xuz2mzX6LDP87SigQ==")
        pswd += " / " + mySecurity.Decryption("7dfCJTp87OCx0M3xFOxlAQ==")
        pswd += " / " + mySecurity.Decryption("nFPqtr0Aa9i/VdDPVVfLhA==")
        pswd += " / " + mySecurity.Decryption("f7fD8xuz2mzX6LDP87SigQ==")
        pswd += " / " + mySecurity.Decryption("iketN8FGcjjK3EGXFTIk1A==")

        BsDecryption.Text = pswd
    End Sub
End Class
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports System.Globalization

Public Class BaseLineEntityExpiration
    Implements IBaseLineExpiration

    'Public Property AnalyzerID As String Implements IBaseLineExpiration.AnalyzerID
    Private _analyzer As IAnalyzerManager

    Sub New(analyzer As IAnalyzerManager)
        Me._analyzer = analyzer
    End Sub


    Public ReadOnly Property IsBlExpired As Boolean Implements IBaseLineExpiration.IsBlExpired
        Get
            IsBlExpired = getBLParameter()
        End Get
    End Property

    Public Function getBLParameter() As Boolean
        Dim resultData As GlobalDataTO = Nothing
        Dim myAnalyzerSettingsDs As New AnalyzerSettingsDS
        Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow
        Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
        Dim myDate As String = ""
        Dim dtStartDate As Date

        Try

            Dim AnalyzerID As String = ""
            resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(AnalyzerID, _
                                                                       GlobalEnumerates.AnalyzerSettingsEnum.DYNAMICBLDATETIME).GetCompatibleGlobalDataTO()

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myAnalyzerSettingsDs = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                If (myAnalyzerSettingsDs.tcfgAnalyzerSettings.Rows.Count = 1) Then
                    myDate = myAnalyzerSettingsDs.tcfgAnalyzerSettings.First.CurrentValue
                End If

                If String.Equals(myDate.Trim, String.Empty) Then
                    Exit Function
                Else
                    dtStartDate = DateTime.Parse(myAnalyzerSettingsDs.tcfgAnalyzerSettings.First.CurrentValue, CultureInfo.InvariantCulture)
                End If
            End If





        Catch ex As Exception
            Throw ex
        Finally
            myAnalyzerSettingsDs = Nothing
        End Try
    End Function



    'Public Sub CreationAlarm(ByRef alarm As AlarmEnumerates.Alarms) Implements IBaseLine.CreationAlarm
    '    Dim myGlobal As New GlobalDataTO
    '    Dim myAlarmList As New List(Of AlarmEnumerates.Alarms)
    '    Dim myAlarmStatusList As New List(Of Boolean)

    '    Try
    '        myAlarmList.Add(alarm)
    '        myAlarmStatusList.Add(True)

    '        If myAlarmList.Count > 0 Then

    '            Dim currentAlarms = New AnalyzerAlarms(Analyzer)
    '            myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)

    '        End If
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub









End Class
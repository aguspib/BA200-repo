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
        Dim myAnalyzerSettingsDs As New AnalyzerSettingsDelegate
        'Dim myDate As String = ""
        'Dim dtStartDate As Date
        Dim result As Boolean = False
        Try




            'Dim AnalyzerID2 As String = ""
            'resultData = myAnalyzerSettingsDs.GetAnalyzerSetting(Nothing, _
            '                                                           _analyzer.ActiveAnalyzer.ToString(), _
            '                                                           GlobalEnumerates.AnalyzerSettingsEnum.DYNAMIC_BL_LIFETIME.ToString())
            'Dim dtr As ParametersDS.tfmwSwParametersRow() = CType(_analyzer.AnalyzerSwParameters.tfmwSwParameters.Select("ParameterName= '" & GlobalEnumerates.AnalyzerSettingsEnum.BL_DATETIME.ToString() & "' "), ParametersDS.tfmwSwParametersRow())
            'If dtr.Length > 1 Then

            'End If
            ' Dim dtR As DataRow = _analyzer.ActiveAnalyzer

            'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
            '    myAnalyzerSettingsDs = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

            '    If (myAnalyzerSettingsDs.tcfgAnalyzerSettings.Rows.Count = 1) Then
            '        myDate = myAnalyzerSettingsDs.tcfgAnalyzerSettings.First.CurrentValue
            '    End If

            '    If String.Equals(myDate.Trim, String.Empty) Then
            '        'Is Empty for this analyzer, create new BL. 
            '        result = True
            '    Else
            '        dtStartDate = DateTime.Parse(myAnalyzerSettingsDs.tcfgAnalyzerSettings.First.CurrentValue, CultureInfo.InvariantCulture)

            '        ' Read W-Up full time configuration
            '        resultData = SwParametersDelegate.ReadNumValueByParameterName(_analyzer, GlobalEnumerates.SwParameters.WUPFULLTIME.ToString, _analyzer.a)

            '        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
            '            'Dim myParametersDS As New ParametersDS
            '            Dim myParametersDS As ParametersDS

            '            myParametersDS = CType(resultData.SetDatos, ParametersDS)
            '            'Dim myList As New List(Of ParametersDS.tfmwSwParametersRow)
            '            Dim myList As List(Of ParametersDS.tfmwSwParametersRow)

            '            myList = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
            '                      Where String.Equals(a.ParameterName, GlobalEnumerates.SwParameters.WUPFULLTIME.ToString) _
            '                      Select a).ToList

            '            If myList.Count > 0 Then WUPFullTime = myList(0).ValueNumeric
            '        End If
            'End If

        Catch ex As Exception
            Throw ex
        Finally
            'myAnalyzerSettingsDs = Nothing
        End Try
        Return result
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
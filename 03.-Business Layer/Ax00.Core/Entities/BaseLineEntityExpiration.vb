﻿Imports System.Diagnostics.Eventing.Reader
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
    ''' <summary>
    ''' Property that returns if BL is expired.Calculation based on difference of dates.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsBlExpired As Boolean Implements IBaseLineExpiration.IsBlExpired
        Get
            Dim result As Boolean = True
            Try
                result = IsBLExpiredFunction()
            Catch e As Exception
                CreateLogActivity(e)
            Finally
                _analyzer.IsBlExpired = result
            End Try
            Return result
        End Get
    End Property

    ''' <summary>
    ''' Function called from IsBlExpired get property, this function check if the BaseLine need to be recalculated.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsBLExpiredFunction() As Boolean
        Dim result = False
        Dim mins As Integer = -1
        Dim datelastBl As Date
        Dim datelastBLstr As String = String.Empty

        Dim dtrSwParam = (From a As ParametersDS.tfmwSwParametersRow In _analyzer.AnalyzerSwParameters.tfmwSwParameters
                          Where a.ParameterName = GlobalEnumerates.SwParameters.BL_LIFETIME.ToString()).ToList()

        If dtrSwParam.Any() Then mins = CInt(dtrSwParam(0).ValueNumeric)

        Dim dtrAnalyzerSettings = (From a As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In _analyzer.AnalyzerSettings.tcfgAnalyzerSettings
                                   Where a.SettingID = GlobalEnumerates.AnalyzerSettingsEnum.BL_DATETIME.ToString()).ToList()

        If dtrAnalyzerSettings.Any() Then

            If Not dtrAnalyzerSettings(0).IsCurrentValueNull() Then
                datelastBLstr = CStr(dtrAnalyzerSettings(0).CurrentValue)
            End If

            If datelastBLstr.Equals(String.Empty) Then
                result = True
            Else
                datelastBl = DateTime.Parse(dtrAnalyzerSettings(0).CurrentValue, CultureInfo.InvariantCulture)
                If DateDiff(DateInterval.Minute, datelastBl, Now) > mins Then
                    result = True
                End If
            End If
        Else
            'Analyzer can't loaded the default parameters, means are nothing.
            result = True
        End If
        Return result
    End Function

End Class
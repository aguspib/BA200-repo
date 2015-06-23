Imports System.Diagnostics.Eventing.Reader
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
            Try
                IsBlExpired = GetBlParameter()
            Catch e As Exception
                CreateLogActivity(e)
                Return True
            End Try
        End Get
    End Property

    ''' <summary>
    ''' Function called from IsBlExpired get property, this function check if the BaseLine need to be recalculated.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetBlParameter() As Boolean
        Dim result = False
        Dim mins As Integer = -1
        Dim datelastBl As Date
        Dim datelastBLstr As String = String.Empty

            Dim dtrSwParam = (From a As ParametersDS.tfmwSwParametersRow In _analyzer.AnalyzerSwParameters.tfmwSwParameters
                              Where a.ParameterName = GlobalEnumerates.SwParameters.BL_LIFETIME.ToString()).First()

            If dtrSwParam IsNot Nothing Then mins = CInt(dtrSwParam.ValueNumeric)

            Dim dtrAnalyzerSettings = (From a As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In _analyzer.AnalyzerSettings.tcfgAnalyzerSettings
                                       Where a.SettingID = GlobalEnumerates.AnalyzerSettingsEnum.BL_DATETIME.ToString()).First()


            If dtrAnalyzerSettings IsNot Nothing Then

                If Not dtrAnalyzerSettings.IsCurrentValueNull() Then
                    datelastBLstr = CStr(dtrAnalyzerSettings.CurrentValue)
                End If

                If datelastBLstr.Equals(String.Empty) Then
                    result = True
                Else
                    datelastBl = DateTime.Parse(dtrAnalyzerSettings.CurrentValue, CultureInfo.InvariantCulture)
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
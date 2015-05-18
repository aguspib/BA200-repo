Imports Logger
Imports Biosystems.Ax00.Global

Public Interface IStatisticsUpkeepDelegate
    ''' <summary>
    ''' Logs changes of rotor
    ''' </summary>
    ''' <remarks>Created By AC</remarks>
    Sub LogRotorChangeConsum(analyzerId As String)
End Interface

''' <summary>
''' Class Manager to instance the interface of the StatisticsUpkeepDelegate.
''' </summary>
''' <remarks></remarks>
Public Class StatisticsUpKeepManager
    Implements IStatisticsUpkeepDelegate

    Public Sub LogRotorChangeConsum(analyzerId As String) Implements IStatisticsUpkeepDelegate.LogRotorChangeConsum

        Try
            Dim logRotorChanges As New MessageLogger("LogConsum", "AnalyzerSN", analyzerId & "_RotorConsumption")
            Dim message = analyzerId.ToString
            logRotorChanges.AddLog(message)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "StatisticsUpkeepDelegate.LogRotorChangeConsum", EventLogEntryType.Error, False)
        End Try

    End Sub

End Class
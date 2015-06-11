Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.Core.Entities.UpdateVersion

    Public Class ExecutionResults
        Public Property Success As Boolean = True
        Public Property LastErrorRelease As Release
        Public Property LastErrorCommonRevision As CommonRevision
        Public Property LastErrorDataRevision As DataRevision
        Public Property LastErrorStep As ErrorStep = ErrorStep.NoErrors
        Public Property Exception() As Exception
        Public Enum ErrorStep
            NoErrors
            PrerequisiteScript
            StructureScript
            DataScript
            IntegrityCheckScript
        End Enum

        Public Sub WriteLog(debugLogger As DebugLogger)

            'Dim debugLogger As DebugLogger = New DebugLogger()

            DebugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            DebugLogger.AddLog(" Execution Results: ", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            DebugLogger.AddLog(String.Format(" - Process finished successfully: {0}", Success), GlobalBase.UpdateVersionDatabaseProcessLogFileName)

            If Not Success Then
                DebugLogger.AddLog(String.Format(" - Release Error: {0}", LastErrorRelease.Version), GlobalBase.UpdateVersionDatabaseProcessLogFileName)

                If (Not LastErrorCommonRevision Is Nothing) Then
                    DebugLogger.AddLog(String.Format(" - Commom Revision Error: {0}", LastErrorCommonRevision.RevisionNumber), GlobalBase.UpdateVersionDatabaseProcessLogFileName)
                End If

                If (Not LastErrorDataRevision Is Nothing) Then
                    DebugLogger.AddLog(String.Format(" - Data Revision Error: {0}", LastErrorDataRevision.RevisionNumber), GlobalBase.UpdateVersionDatabaseProcessLogFileName)
                End If

                DebugLogger.AddLog(String.Format(" - Step Error: {0}", LastErrorStep.ToString()), GlobalBase.UpdateVersionDatabaseProcessLogFileName)

            End If

            If (Not Exception Is Nothing) Then
                DebugLogger.AddLog(String.Format(" - Exception: {0}", Exception.Message & " " & Exception.InnerException.ToString()), GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            End If
            DebugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)

        End Sub

    End Class
End Namespace
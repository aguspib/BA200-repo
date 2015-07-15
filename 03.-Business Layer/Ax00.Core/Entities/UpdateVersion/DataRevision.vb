Imports System.Globalization
Imports System.Text
Imports System.Xml.Serialization
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global
Imports Microsoft.SqlServer.Management.Smo

' ReSharper disable once CheckNamespace
Namespace Biosystems.Ax00.Core.Entities.UpdateVersion

    <Serializable()>
    Public Class DataRevision
        Inherits Revision
        Implements iComparable

#Region "Serializable elements"

        Public Property DataScript As String

#End Region

#Region "Public members"

        Public Overrides Function Run(result As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String, ByVal packageId As String) As Boolean

            RunPrerequisites(result, server, dataBaseName)

            If result.Success Then
                RunData(result, server, dataBaseName)
                If result.Success Then
                    RunIntegrity(result, server, dataBaseName)
                End If
            Else
                RunIntegrity(result, server, dataBaseName)
            End If

            If result.Success Then
                UpdateDatabaseVersion(dataBaseName, server, packageId)
            Else
                result.LastErrorDataRevision = Me
            End If

            Return result.Success
        End Function

        Public Sub WriteLog(debugLogger As DebugLogger)

            'Dim debugLogger As DebugLogger = New DebugLogger()
            Dim content As New StringBuilder()

            content.AppendLine(String.Format(" Data Revision JiraId: {0} RevisionNumber: {1}", JiraId, RevisionNumber))
            content.AppendLine(String.Format("      DataScript: {0}", DataScript))

            debugLogger.AddLog(content.ToString(), GlobalBase.UpdateVersionDatabaseProcessLogFileName)

        End Sub

#End Region

#Region "Private members"

        Private Sub RunData(result As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String)

            If DataScript Is Nothing OrElse DataScript.Trim = String.Empty Then Return
            result.Success = DBManager.ExecuteScripts(server, dataBaseName, DataScript, result.Exception)
            If result.Success = False Then result.LastErrorStep = ExecutionResults.ErrorStep.DataScript

        End Sub

        Private Sub UpdateDatabaseVersion(ByVal pDataBaseName As String, ByRef pServer As Server, ByVal packageId As String)

            Dim myVersionsDelegate As New VersionsDelegate

            ' update the new Database version. pass the server connection contex because we are inside a transaction that can affect the table.
            myVersionsDelegate.SaveDBSoftwareVersion(pServer.ConnectionContext.SqlConnectionObject(), packageId, String.Empty, 0, RevisionNumber)

        End Sub

#End Region

    End Class
End Namespace
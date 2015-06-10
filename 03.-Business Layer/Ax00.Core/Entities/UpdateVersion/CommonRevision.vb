Imports System.ComponentModel
Imports System.Text
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global
Imports Microsoft.SqlServer.Management.Smo

' ReSharper disable once CheckNamespace
Namespace Biosystems.Ax00.Core.Entities.UpdateVersion

    <Serializable()>
    Public Class CommonRevision
        Inherits Revision
        Implements iComparable

#Region "Serializable elements"

        Public Property StructureScript As String
        Public Property DataScript As String

#End Region

#Region "Public members"

        Public Overrides Function Run(result As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String, ByVal packageId As String) As Boolean

            RunPrerequisites(result, server, dataBaseName)

            If (result.Success) Then
                RunStructure(result, server, dataBaseName)
                If result.Success Then
                    RunData(result, server, dataBaseName)
                    If result.Success Then
                        RunIntegrity(result, server, dataBaseName)
                    End If
                End If
            Else
                RunIntegrity(result, server, dataBaseName)
            End If

            If result.Success Then
                UpdateDatabaseVersion(dataBaseName, server, packageId)
            Else
                result.LastErrorCommonRevision = Me
            End If

            Return result.Success
        End Function

        Public Sub WriteLog()

            Dim debugLogger As DebugLogger = New DebugLogger()
            Dim content As New StringBuilder()

            content.AppendLine(String.Format(" Common Revision JiraId: {0} RevisionNumber: {1}", JiraId, RevisionNumber))
            content.AppendLine(String.Format("      StructureScript: {0}", StructureScript))
            content.AppendLine(String.Format("      DataScript: {0}", DataScript))

            debugLogger.AddLog(content.ToString(), GlobalBase.UpdateVersionDatabaseProcessLogFileName)

        End Sub

#End Region

#Region "Private members"

        Private Sub RunStructure(result As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String)

            If StructureScript Is Nothing OrElse StructureScript.Trim = String.Empty Then Return
            result.Success = DBManager.ExecuteScripts(server, dataBaseName, StructureScript, result.Exception)
            If result.Success = False Then result.LastErrorStep = ExecutionResults.ErrorStep.StructureScript

        End Sub

        Private Sub RunData(result As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String)

            If DataScript Is Nothing OrElse DataScript.Trim = String.Empty Then Return
            result.Success = DBManager.ExecuteScripts(server, dataBaseName, DataScript, result.Exception)
            If result.Success = False Then result.LastErrorStep = ExecutionResults.ErrorStep.DataScript

        End Sub

        Private Sub UpdateDatabaseVersion(ByVal dataBaseName As String, ByRef server As Server, ByVal packageId As String)

            Dim myVersionsDelegate As New VersionsDelegate

            ' update the new Database version. pass the server connection contex because we are inside a transaction that can affect the table.
            myVersionsDelegate.SaveDBSoftwareVersion(server.ConnectionContext.SqlConnectionObject(), packageId, String.Empty, RevisionNumber, String.Empty)

        End Sub

#End Region

    End Class
End Namespace
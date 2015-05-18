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

        Public Overrides Function Run(executionResults As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String, ByVal packageId As String) As Boolean

            Me.Results = executionResults

            RunPrerequisites(server, dataBaseName)

            If (executionResults.Success) Then
                RunStructure(server, dataBaseName)
                If executionResults.Success Then
                    RunData(server, dataBaseName)
                    If executionResults.Success Then
                        RunIntegrity(server, dataBaseName)
                    End If
                End If
            Else
                RunIntegrity(server, dataBaseName)
            End If

            If executionResults.Success Then
                UpdateDatabaseVersion(dataBaseName, server, packageId)
            Else
                executionResults.LastErrorCommonRevision = Me
            End If

            'WriteLog(Me.ToString())

            Return executionResults.Success
        End Function

        Public Sub WriteLog()

            Dim content As New StringBuilder()

            content.AppendLine(String.Format(" Common Revision JiraId: {0} RevisionNumber: {1}", JiraId, RevisionNumber))
            content.AppendLine(String.Format("      StructureScript: {0}", StructureScript))
            content.AppendLine(String.Format("      DataScript: {0}", DataScript))
            If (Not Results Is Nothing) Then
                content.AppendLine(String.Format("      Success: {0}", Results.Success))
                If Not Results.Success Then
                    content.AppendLine(String.Format("      Step Error: {0}", Results.LastErrorStep.ToString()))
                End If
                If (Not Results.Exception Is Nothing) Then
                    content.AppendLine(String.Format("      Exception: {0}", Results.Exception.Message & " " & Results.Exception.InnerException.ToString()))
                End If
            End If

            DebugLogger.AddLog(content.ToString(), GlobalBase.UpdateVersionDatabaseProcessLogFileName)

        End Sub

#End Region

#Region "Private members"

        Private Sub RunStructure(ByRef server As Server, ByVal dataBaseName As String)

            If StructureScript Is Nothing OrElse StructureScript.Trim = String.Empty Then Return
            Results.Success = DBManager.ExecuteScripts(server, dataBaseName, StructureScript, Results.Exception)
            If Results.Success = False Then Results.LastErrorStep = ExecutionResults.ErrorStep.StructureScript

        End Sub

        Private Sub RunData(ByRef server As Server, ByVal dataBaseName As String)

            If DataScript Is Nothing OrElse DataScript.Trim = String.Empty Then Return
            Results.Success = DBManager.ExecuteScripts(server, dataBaseName, DataScript, Results.Exception)
            If Results.Success = False Then Results.LastErrorStep = ExecutionResults.ErrorStep.DataScript

        End Sub

        Private Sub UpdateDatabaseVersion(ByVal dataBaseName As String, ByRef server As Server, ByVal packageId As String)

            Dim myVersionsDelegate As New VersionsDelegate

            ' update the new Database version. pass the server connection contex because we are inside a transaction that can affect the table.
            myVersionsDelegate.SaveDBSoftwareVersion(server.ConnectionContext.SqlConnectionObject(), packageId, String.Empty, RevisionNumber, String.Empty)

        End Sub

#End Region

    End Class
End Namespace
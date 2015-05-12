Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
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

        Public Overrides Function Run(results As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String, ByVal packageId As String) As Boolean
            Debug.WriteLine("Executting Common Revision JiraId:" & JiraId & " RevisionNumber:" & RevisionNumber)

            If results.Success Then RunPrerequisites(results, server, dataBaseName)
            If results.Success Then RunStructure(results, server, dataBaseName)
            If results.Success Then RunData(results, server, dataBaseName)
            If results.Success Then RunIntegrity(results, server, dataBaseName)

            If results.Success Then
                UpdateDatabaseVersion(results, dataBaseName, server, packageId)
            Else
                results.LastErrorCommonRevision = Me
            End If

            Debug.WriteLine("Execution result:" & results.Success)

            Return results.Success
        End Function


        Private Sub UpdateDatabaseVersion(results As ExecutionResults, ByVal dataBaseName As String, ByRef server As Server, ByVal packageId As String)

            Dim myVersionsDelegate As New VersionsDelegate

            ' update the new Database version. pass the server connection contex because we are inside a transaction that can affect the table.
            myVersionsDelegate.SaveDBSoftwareVersion(server.ConnectionContext.SqlConnectionObject(), packageId, String.Empty, RevisionNumber, String.Empty)

        End Sub

#End Region

#Region "Private members"

        Private Sub RunStructure(result As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String)

            If StructureScript Is Nothing OrElse StructureScript.Trim = String.Empty Then Return
            Debug.WriteLine("Running StructureScript script")
            result.Success = DBManager.ExecuteScripts(server, dataBaseName, StructureScript)
            If result.Success = False Then result.LastErrorStep = ExecutionResults.ErrorStep.StructureScript

        End Sub

        Private Sub RunData(result As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String)

            If DataScript Is Nothing OrElse DataScript.Trim = String.Empty Then Return
            Debug.WriteLine("Running data script")
            result.Success = DBManager.ExecuteScripts(server, dataBaseName, DataScript)
            If result.Success = False Then result.LastErrorStep = ExecutionResults.ErrorStep.DataScript

        End Sub

#End Region

    End Class
End Namespace
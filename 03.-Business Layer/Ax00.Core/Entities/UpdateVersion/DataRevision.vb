Imports System.Globalization
Imports System.Xml.Serialization
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
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

        Public Overrides Function Run(results As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String, ByVal packageId As String) As Boolean
            Debug.WriteLine("Executting Data Revision JiraId:" & JiraId & " RevisionNumber:" & RevisionNumber)

            If results.Success Then RunPrerequisites(results, server, dataBaseName)
            'If results.Success Then RunStructure(result)
            If results.Success Then RunData(results, server, dataBaseName)
            If results.Success Then RunIntegrity(results, server, dataBaseName)

            If results.Success Then
                UpdateDatabaseVersion(results, dataBaseName, server, packageId)
            Else
                results.LastErrorDataRevision = Me
            End If

            Debug.WriteLine("Execution result:" & results.Success)

            Return results.Success
        End Function


        Private Sub UpdateDatabaseVersion(results As ExecutionResults, ByVal pDataBaseName As String, ByRef pServer As Server, ByVal packageId As String)

            Dim myVersionsDelegate As New VersionsDelegate

            ' update the new Database version. pass the server connection contex because we are inside a transaction that can affect the table.
            myVersionsDelegate.SaveDBSoftwareVersion(pServer.ConnectionContext.SqlConnectionObject(), packageId, String.Empty, String.Empty, RevisionNumber)

        End Sub

#End Region

#Region "Private members"

        Private Sub RunData(result As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String)

            If DataScript Is Nothing OrElse DataScript.Trim = String.Empty Then Return
            Debug.WriteLine("Running data script")
            result.Success = DBManager.ExecuteScripts(server, dataBaseName, DataScript)
            If result.Success = False Then result.LastErrorStep = ExecutionResults.ErrorStep.DataScript

        End Sub

#End Region

    End Class
End Namespace
Imports System.Globalization
Imports System.Xml.Serialization
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Microsoft.SqlServer.Management.Smo

' ReSharper disable once CheckNamespace
Namespace Biosystems.Ax00.Core.Entities.UpdateVersion

    <Serializable()>
    Public MustInherit Class Revision
        Implements iComparable

#Region "Serializable elements"

        ''' <summary>
        ''' This number can be used to sort nodes exection when there are several execution nodes registered to be run in the same day.
        ''' </summary>
        <XmlAttribute("SequenceNumber")>
        Public Property RevisionNumber As String = "1"

        <XmlAttribute("JiraID")>
        Public Property JiraId As String
        Public Property PrerequisiteScript As String
        Public Property IntegrityScript As String

#End Region

        <XmlIgnoreAttribute()>
        Public Property Results As ExecutionResults

#Region "Public members"

        Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            Dim compared = TryCast(obj, Revision)
            If compared IsNot Nothing Then

                'If compared.RevisionDate < RevisionDate Then
                '    Return 1
                'ElseIf compared.RevisionDate > RevisionDate Then
                '    Return -1
                'Else
                If compared.RevisionNumber < RevisionNumber Then
                    Return 1
                ElseIf compared.RevisionNumber = RevisionNumber Then
                    Return 0
                Else
                    Return -1
                End If
                'End If
            Else
                Throw (New Exception("Objects can't be compared"))
            End If
        End Function

        Public MustOverride Function Run(result As ExecutionResults, ByRef server As Server, ByVal dataBaseName As String, ByVal packageId As String) As Boolean

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return CType(obj, Revision).RevisionNumber = RevisionNumber
        End Function

        Public Function Validate() As Boolean
            'TODO: Implement this method to validate if values is valid or not
            Return True
        End Function

#End Region

#Region "Private members"


        Public Sub RunPrerequisites(ByRef server As Server, ByVal dataBaseName As String)

            If PrerequisiteScript Is Nothing OrElse PrerequisiteScript.Trim = String.Empty Then Return
            Results.Success = DBManager.ExecuteBooleanScript(server, dataBaseName, PrerequisiteScript, Results.Exception)
            If Results.Success = False Then Results.LastErrorStep = ExecutionResults.ErrorStep.PrerequisiteScript

        End Sub

        Public Sub RunIntegrity(ByRef server As Server, ByVal dataBaseName As String)

            If IntegrityScript Is Nothing OrElse IntegrityScript.Trim = String.Empty Then Return
            Results.Success = DBManager.ExecuteBooleanScript(server, dataBaseName, IntegrityScript, Results.Exception)
            If Results.Success = False Then Results.LastErrorStep = ExecutionResults.ErrorStep.IntegrityCheckScript

        End Sub

#End Region
    End Class
End Namespace
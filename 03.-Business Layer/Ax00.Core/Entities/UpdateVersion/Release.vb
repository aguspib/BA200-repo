Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.SqlServer.Management.Smo
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global

' ReSharper disable once CheckNamespace
Namespace Biosystems.Ax00.Core.Entities.UpdateVersion

    <Serializable()>
    Public Class Release
        Implements Icomparable

        Dim _version As String = "00.00.00"
        <XmlAttribute("Version")>
        Public Property Version As String
            Get
                Return _version
            End Get
            Set(value As String)
                Dim haserror = False
                Dim validation = value.Split("."c)
                If validation.Count <> 3 Then
                    haserror = True
                Else
                    For Each versNumber In validation
                        If Not IsNumeric(versNumber) OrElse versNumber.Length <> 2 OrElse CInt(versNumber) < 0 Then
                            haserror = True
                            Exit For
                        End If
                    Next
                End If
                If Not haserror Then
                    _version = value
                Else
                    Throw (New Exception("Incorrect version value for a Release. Version format has to be ##.##.##"))
                End If
            End Set
        End Property

        Public Property CommonRevisions As New List(Of CommonRevision)
        Public Property DataRevisions As New List(Of DataRevision)

        Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
            Dim compared = TryCast(obj, Release)
            If compared IsNot Nothing Then
                If compared.Version < Version Then
                    Return 1
                ElseIf compared.Version > Version Then
                    Return -1
                Else
                    Return 0
                End If
            Else
                Throw (New Exception("Object are not comparable"))
            End If

        End Function

        Public Sub SortContents()
            CommonRevisions.Sort()
            DataRevisions.Sort()
        End Sub

        Function GenerateRevisionPack(fromCommonRevisionNumber As String, fromDataRevisionNumber As String) As Release

            Dim release As New Release
            release.Version = Version

            For Each revision In CommonRevisions
                If revision.RevisionNumber <= fromCommonRevisionNumber Then
                    Continue For
                Else
                    release.CommonRevisions.Add(revision)
                End If
            Next

            For Each revision In DataRevisions
                If revision.RevisionNumber <= fromDataRevisionNumber Then
                    Continue For
                Else
                    release.DataRevisions.Add(revision)
                End If
            Next

            Return release

        End Function

        Sub RunScripts(results As ExecutionResults, ByVal dataBaseName As String, ByRef server As Server, ByVal packageId As String)

            For Each revision In CommonRevisions
                revision.Run(results, server, dataBaseName, packageId)

                If Not results.Success Then
                    Exit For
                End If
            Next

            For Each revision In DataRevisions
                revision.Run(results, server, dataBaseName, packageId)

                If Not results.Success Then
                    Exit For
                End If
            Next

            If results.Success Then
                UpdateDatabaseVersion(results, dataBaseName, server, packageId)
            Else
                results.LastErrorRelease = Me
            End If

        End Sub

        Public Sub Merge(release As Release)

            CommonRevisions.AddRange(release.CommonRevisions)
            DataRevisions.AddRange(release.DataRevisions)

        End Sub


        Public Function Validate() As Boolean

            Dim valid = CommonRevisions.Select(Function(x) x.RevisionNumber).Distinct().Count() = CommonRevisions.Count

            If (valid) Then
                For Each revision In CommonRevisions
                    valid = revision.Validate()
                    If (Not valid) Then Exit For
                Next
            End If

            If (valid) Then
                For Each revision In DataRevisions
                    valid = revision.Validate()
                    If (Not valid) Then Exit For
                Next
            End If

            Return valid

        End Function


        Private Sub UpdateDatabaseVersion(results As ExecutionResults, ByVal pDataBaseName As String, ByRef pServer As Server, ByVal packageId As String)

            Dim myVersionsDelegate As New VersionsDelegate

            ' update the new Database version. pass the server connection contex because we are inside a transaction that can affect the table.
            myVersionsDelegate.SaveDBSoftwareVersion(pServer.ConnectionContext.SqlConnectionObject(), packageId, Version, String.Empty, String.Empty)

        End Sub

        Public Sub WriteLog(debugLogger As DebugLogger)

            debugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            debugLogger.AddLog(String.Format(" Release: {0}", Version), GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            debugLogger.AddLog(" 1.- Common scripts", GlobalBase.UpdateVersionDatabaseProcessLogFileName)

            For Each revision As CommonRevision In CommonRevisions
                revision.WriteLog(debugLogger)
            Next

            debugLogger.AddLog(" 2.- Data scripts", GlobalBase.UpdateVersionDatabaseProcessLogFileName)

            For Each revision As DataRevision In DataRevisions
                revision.WriteLog(debugLogger)
            Next

            debugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)

        End Sub

    End Class

End Namespace
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.SqlServer.Management.Smo

Namespace Biosystems.Ax00.Core.Entities.UpdateVersion

    <Serializable()>
    Public Class DatabaseUpdatesManager

        Public Property Releases As New List(Of Release)

        Public Sub SortContents()
            Releases.Sort()
            For Each release In Releases
                release.SortContents()
            Next
        End Sub

        Public Sub Serialize(stream As Stream)
            SortContents()
            Dim x As New XmlSerializer(Me.GetType)
            x.Serialize(stream, Me)
        End Sub

        Public Sub Serialize(filename As String)
            Dim file As New StreamWriter(filename, False)
            Serialize(file.BaseStream)
            file.Close()
        End Sub

        Public Shared Function Deserialize(filename As String) As DatabaseUpdatesManager
            Dim file As New StreamReader(filename)
            Dim result = Deserialize(file.BaseStream)
            file.Close()
            Return result
        End Function

        Shared Function Deserialize(stream As Stream) As DatabaseUpdatesManager
            'Deserialize text file to a new object.
            Dim objStreamReader As New StreamReader(stream)


            Dim auxVariable As New DatabaseUpdatesManager()
            Dim x As New XmlSerializer(auxVariable.GetType)

            auxVariable = TryCast(x.Deserialize(objStreamReader), DatabaseUpdatesManager)
            objStreamReader.Close()

            Return auxVariable
        End Function

        Shared Function Deserialize(xml As XmlTextReader) As DatabaseUpdatesManager

            Dim auxVariable As New DatabaseUpdatesManager()
            Dim x As New XmlSerializer(auxVariable.GetType)

            auxVariable = TryCast(x.Deserialize(xml), DatabaseUpdatesManager)

            Return auxVariable
        End Function


        Shared Function Deserialize(stream As StreamReader) As DatabaseUpdatesManager
            'Deserialize text file to a new object.

            Dim auxVariable As New DatabaseUpdatesManager()
            Dim x As New XmlSerializer(auxVariable.GetType)

            auxVariable = TryCast(x.Deserialize(stream), DatabaseUpdatesManager)
            stream.Close()

            Return auxVariable
        End Function


        Function GenerateUpdatePack(fromVersion As String, fromCommonRevisionNumber As String, fromDataRevisionNumber As String) As DatabaseUpdatesManager

            Dim updatesManager As New DatabaseUpdatesManager

            For Each release In Releases
                If release.Version < fromVersion Then
                    'Ignore previous versions
                    Continue For
                ElseIf release.Version = fromVersion Then
                    'Ignore previous subversions, but get required subversions
                    updatesManager.Releases.Add(release.GenerateRevisionPack(fromCommonRevisionNumber, fromDataRevisionNumber))
                Else
                    'Add newer versions
                    updatesManager.Releases.Add(release)
                End If
            Next

            updatesManager.SortContents()

            Return updatesManager

        End Function

        Public Function RunScripts(ByVal dataBaseName As String, ByRef server As Server, ByVal packageId As String) As ExecutionResults
            Dim results As New ExecutionResults

            For Each release In Releases
                release.RunScripts(results, dataBaseName, server, packageId)
                If results.Success = False Then Exit For
            Next
            Return results
        End Function

        Public Sub Merge(databaseUpdatesManager As DatabaseUpdatesManager)

            For Each item As Release In databaseUpdatesManager.Releases
                Dim release = item
                If (Releases.Where(Function(x) x.Version.Equals(release.Version)).Count = 0) Then
                    Releases.Add(item)
                Else
                    Releases.FirstOrDefault(Function(x) x.Version.Equals(release.Version)).Merge(release)
                End If
            Next

        End Sub

        Public Function Validate() As Boolean

            Dim valid As Boolean = True

            For Each release In Releases
                valid = release.Validate()
                If (Not valid) Then Exit For
            Next

            Return valid

        End Function


    End Class

End Namespace
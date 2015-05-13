Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.SqlServer.Management.Smo

Namespace Biosystems.Ax00.Core.Entities.UpdateVersion

    <Serializable()>
    Public Class DatabaseUpdatesManager

        Public Property Releases As New List(Of Release)

        <XmlIgnoreAttribute()>
        Public Property FromVersion As String
        <XmlIgnoreAttribute()>
        Public Property FromCommonRevisionNumber As String
        <XmlIgnoreAttribute()>
        Public Property FromDataRevisionNumber As String

        <XmlIgnoreAttribute()>
        Public Property Results As ExecutionResults


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


        Function GenerateUpdatePack(ByVal version As String, commonRevisionNumber As String, dataRevisionNumber As String) As DatabaseUpdatesManager

            Dim updatesManager As New DatabaseUpdatesManager
            updatesManager.FromVersion = version
            updatesManager.FromCommonRevisionNumber = commonRevisionNumber
            updatesManager.FromDataRevisionNumber = dataRevisionNumber

            For Each release In Releases
                If release.Version < version Then
                    'Ignore previous versions
                    Continue For
                ElseIf release.Version = version Then
                    'Ignore previous subversions, but get required subversion
                    updatesManager.Releases.Add(release.GenerateRevisionPack(commonRevisionNumber, dataRevisionNumber))
                Else
                    'Add newer versions
                    updatesManager.Releases.Add(release)
                End If

            Next

            updatesManager.SortContents()

            Return updatesManager

        End Function

        Public Function RunScripts(ByVal dataBaseName As String, ByRef server As Server, ByVal packageId As String) As ExecutionResults

            Results = New ExecutionResults
            For Each release In Releases
                release.RunScripts(results, dataBaseName, server, packageId)
                If results.Success = False Then Exit For
            Next

            WriteLog()
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

        Private Sub WriteLog()

            DebugLogger.AddLog(" --------------------------------------------", "UpdateVersion")
            DebugLogger.AddLog(" Update Version: Generated Update Pack (INI)", "UpdateVersion")
            DebugLogger.AddLog(String.Format(" From Version: {0} Common Revision: {1} Data Revision: {2}", FromVersion, FromCommonRevisionNumber, FromDataRevisionNumber), "UpdateVersion")
            For Each release In Releases
                DebugLogger.AddLog(String.Format(" Added Release Version: {0}", release.Version), "UpdateVersion")
                DebugLogger.AddLog(String.Format(" - Total Common Revisions: {0}", release.CommonRevisions.Count), "UpdateVersion")
                DebugLogger.AddLog(String.Format(" - Total Data Revisions: {0}", release.DataRevisions.Count), "UpdateVersion")
            Next
            DebugLogger.AddLog(String.Format(" Total Releases: {0}", Releases.Count), "UpdateVersion")
            DebugLogger.AddLog(" Update Version: Generating Update Pack (END)", "UpdateVersion")
            DebugLogger.AddLog(" --------------------------------------------", "UpdateVersion")


            DebugLogger.AddLog(" --------------------------------------------", "UpdateVersion")
            DebugLogger.AddLog(" Update Version: Run Scripts (INI)", "UpdateVersion")
            For Each release In Releases
                release.WriteLog()
            Next
            DebugLogger.AddLog(String.Format(" - Process finished successfully: {0}", Results.Success), "UpdateVersion")
            DebugLogger.AddLog(" Update Version: Run Scripts (END)", "UpdateVersion")
            DebugLogger.AddLog(" --------------------------------------------", "UpdateVersion")

        End Sub

    End Class

End Namespace
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports Biosystems.Ax00.Global
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
        Public Property ToVersion As String

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


        Function GenerateUpdatePack(pFromVersion As String, pFromCommonRevisionNumberFrom As String, pFromDataRevisionNumber As String, pToVersion As String) As DatabaseUpdatesManager

            Dim updatesManager As New DatabaseUpdatesManager
            updatesManager.FromVersion = pFromVersion
            updatesManager.FromCommonRevisionNumber = pFromCommonRevisionNumberFrom
            updatesManager.FromDataRevisionNumber = pFromDataRevisionNumber
            updatesManager.ToVersion = pToVersion

            For Each release In Releases
                If release.Version < pFromVersion Then
                    'Ignore previous versions
                    Continue For
                    'ElseIf release.Version = pFromVersion Then
                    '    'Ignore previous subversions, but get required subversion
                    '    updatesManager.Releases.Add(release.GenerateRevisionPack(pFromCommonRevisionNumberFrom, pFromDataRevisionNumber))
                    'Else
                    '    'Add newer versions
                    '    updatesManager.Releases.Add(release)
                ElseIf ((release.Version >= pFromVersion) And (release.Version <= pToVersion)) Then
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

            Dim debugLogger As DebugLogger = New DebugLogger()

            DebugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            DebugLogger.AddLog(" Update Version: Generated Update Pack (INI)", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            DebugLogger.AddLog(String.Format(" From Version: {0}  Common Revision: {1} Data Revision: {2} To Version: {3}", FromVersion, FromCommonRevisionNumber, FromDataRevisionNumber, ToVersion), GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            For Each release In Releases
                DebugLogger.AddLog(String.Format(" Added Release Version: {0}", release.Version), GlobalBase.UpdateVersionDatabaseProcessLogFileName)
                DebugLogger.AddLog(String.Format(" - Total Common Revisions: {0}", release.CommonRevisions.Count), GlobalBase.UpdateVersionDatabaseProcessLogFileName)
                DebugLogger.AddLog(String.Format(" - Total Data Revisions: {0}", release.DataRevisions.Count), GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            Next
            DebugLogger.AddLog(String.Format(" Total Releases: {0}", Releases.Count), GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            DebugLogger.AddLog(" Update Version: Generating Update Pack (END)", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            DebugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)


            DebugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            DebugLogger.AddLog(" Update Version: Run Scripts (INI)", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            For Each release In Releases
                release.WriteLog(debugLogger)
            Next

            Results.WriteLog(debugLogger)

            DebugLogger.AddLog(" Update Version: Run Scripts (END)", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            DebugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)

        End Sub

    End Class

End Namespace
Imports System.Windows.Forms
Imports System.IO
Imports Biosystems.Ax00.Global.TO
Imports System.Runtime.InteropServices

Public Class DebugLogger

    Private Const Extension = ".log"
    Private Const FolderName = "DebugLog\"

    Public Shared Sub AddLog(message As String, Optional ByVal prefix As String = Nothing)
#If DEBUG Then
        Dim fullFile As String = getFileNameAndPath(prefix)

        Dim fileFs As New FileStream(fullFile, FileMode.Append, FileAccess.Write)

        If Not fileFs Is Nothing And fileFs.CanWrite Then

            Dim objStream As New StreamWriter(fileFs)
            objStream.WriteLine("[" + Date.Now.TimeOfDay.ToString & "]" + message)
            objStream.Close()
            fileFs.Close()
        End If
#End If
    End Sub

    Private Shared Function getFileNameAndPath(Optional ByVal prefix As String = Nothing) As String
        Dim solutionPath = System.IO.Path.GetFullPath(Application.StartupPath & "\..\..\..\..\")
        If Not Directory.Exists(solutionPath + FolderName) Then
            Directory.CreateDirectory(solutionPath + FolderName)
        End If
        Return solutionPath + FolderName + prefix + DateTime.Now.ToString("yyyyMMdd") + Extension
    End Function

End Class

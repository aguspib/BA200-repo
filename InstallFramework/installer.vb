'Imports System.Net.Mime
Imports System.Security.Policy

Module installer

    Const GitRemoteLocation = "http://stash.ginper.local:7990/scm/ba/bax00framework.git"
    Const GitRemoteLocation2 = "sourcetree://cloneRepo?type=stash&baseWebUrl=http%3A%2F%2Fstash.ginper.local%3A7990&cloneUrl=http%3A%2F%2Fmiba%25C3%25B1ez%40stash.ginper.local%3A7990%2Fscm%2Fba%2Fbax00framework.git&user=miba%C3%B1ez"
    Sub Main()

        ' "C:\Users\manel ibañez\AppData\Local\Atlassian\SourceTree\git_local\cmd"


        Dim gitFolder = AtlassianGitFolder()

        Dim appFolder = AppDomain.CurrentDomain.BaseDirectory()
        Dim success = False
        While Not success
            Dim P As New Process
            P.StartInfo.FileName = gitFolder & "\git.exe"
            P.StartInfo.Arguments = "clone --recursive " & GitRemoteLocation
            P.StartInfo.WindowStyle = ProcessWindowStyle.Maximized
            P.Start()
            While P.HasExited = False

            End While
            success = P.ExitCode = 0
            If Not success Then
                Console.WriteLine("Error downloading git repository. Press 'Y' to retry")
                If Console.ReadKey(True).KeyChar <> "y"c Then
                    End
                End If
            End If
        End While

        If System.IO.File.Exists(appFolder & "\bax00framework\.gitignore") Then

            Dim file = GetSourceTreeFolder() & "\SourceTree.exe"
            'Dim command = """" & file & """ -f """ & appFolder & """\bax00framework\ status"
            Dim repoLocation = System.IO.Path.GetFullPath(appFolder & "\bax00framework\")
            Dim command = " -f """ & repoLocation & """ status"
            Dim sheller = """" & file & """ " & command
            Shell(sheller)

            Console.Write("Press enter to exit")
        Else
            Console.Write("Something went wrong...")

        End If

        Console.ReadLine()
    End Sub

    Private Function AtlassianGitFolder() As String

        Const GitSubfolder = "\Atlassian\SourceTree\git_local\cmd"

        Dim gitFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & GitSubfolder

        If System.IO.Directory.Exists(gitFolder) = False Then
            Console.WriteLine("Can't find Attalasian GIT version installed. Prcess aborted")
            End
        End If
        Return System.IO.Path.GetFullPath(gitFolder)
    End Function

    Private Function GetSourceTreeFolder() As String
        Dim sourceFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) & "\Atlassian\SourceTree"
        If System.IO.Directory.Exists(sourceFolder) Then
            Return sourceFolder
        Else
            Return ""
        End If
    End Function
End Module

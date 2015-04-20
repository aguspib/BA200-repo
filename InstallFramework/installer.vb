'Imports System.Net.Mime
Imports System.Security.Policy

Module Installer

    Const FrameworkLocationInStash = "http://stash.ginper.local:7990/scm/ba/bax00framework.git"

    Sub Main()

        Dim gitFolder = AtlassianGitFolder()
        Const cr = Chr(13) & Chr(10)
        Console.ForegroundColor = ConsoleColor.White
        Console.BackgroundColor = ConsoleColor.Red
        Const niceHeader As String =
            "                                       " & cr &
            "     BAx00 framework download tool     " & cr &
            "                                       "
        Console.WriteLine(niceHeader)
        Console.ForegroundColor = ConsoleColor.Gray
        Console.BackgroundColor = ConsoleColor.Black
        Console.WriteLine("Framework download tool")
        Dim success = False
        While Not success
            Dim gitProcess As New Process
            gitProcess.StartInfo.FileName = gitFolder & "\git.exe"
            gitProcess.StartInfo.Arguments = "clone --recursive " & FrameworkLocationInStash
            gitProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized
            gitProcess.Start()
            Dim t = Now.AddSeconds(1)
            Console.Write("Waiting for GIT authentication and download.")
            While gitProcess.HasExited = False
                If Now > t Then
                    t = Now.AddSeconds(1)
                    Console.Write(".")
                End If
            End While
            Console.WriteLine("")
            success = gitProcess.ExitCode = 0
            If Not success Then
                Console.Write("Error downloading git repository. Press 'Y' to retry>")
                If Console.ReadKey(True).KeyChar <> "y"c Then
                    End
                Else
                    Console.WriteLine("")
                End If
            End If
        End While
        Console.WriteLine("")
        Console.ForegroundColor = ConsoleColor.White
        Console.WriteLine("Important:")
        Console.WriteLine("Framework repository has been downloaded into BAx00Framework folder.")
        Console.WriteLine("Remember to add this repository to SourceTree too")
        Console.ForegroundColor = ConsoleColor.Gray

        Console.WriteLine("")
        Console.Write("Press ENTER to close this window>")

        Console.ReadLine()

    End Sub

    Private Function AtlassianGitFolder() As String

        Const gitSubfolder = "\Atlassian\SourceTree\git_local\cmd"

        Dim gitFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & GitSubfolder

        If IO.Directory.Exists(gitFolder) = False Then
            Console.WriteLine("Can't find Attalasian GIT version installed. Prcess aborted")
            End
        End If
        Return IO.Path.GetFullPath(gitFolder)
    End Function

End Module


'Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.Global

Public NotInheritable Class StartupUSER

    'Private RunningBackGround As Boolean = False
    'Private Ax00StartUpVisible As Boolean = False
    'Dim iconPath As String = IconsPath
    Private Ax00StartUp As IAx00StartUp


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'SGM 01/02/2012 - Set that is User Assembly - Bug #1112
        GlobalBase.IsServiceAssembly = False

        ' Add any initialization after the InitializeComponent() call.

        'SGM 09/01/2012 - activation of compatibility between Framework 4.5 and Mixed Mode Assemblies
        'Dim myLogAcciones As New ApplicationLogManager()

        'AG 21/02/2014 - #1516 at this point services can be stopped (move to the Load event)
        'IMPORTANT!!! Leave the call to x because otherwise some processes like load rsat fails
        If RuntimePolicyHelper.LegacyV2RuntimeEnabledSuccessfully Then
            'myLogAcciones.CreateLogActivity(My.Application.Info.ProductName & " - Application STARTUP", "StartupUSER.New", EventLogEntryType.Information, False)
        Else
            'myLogAcciones.CreateLogActivity(My.Application.Info.ProductName & " - LegacyV2RuntimeEnabled error", "StartupUSER.New", EventLogEntryType.Error, False)
        End If
        'end SGM 09/01/2012

        ''SGM 07/11/2012 - log Application Startup
        'Dim myLogAcciones As New ApplicationLogManager()
        'myLogAcciones.CreateLogActivity(My.Application.Info.ProductName & " - Application STARTUP", "StartupUSER.New", EventLogEntryType.Information, False)
        ''end SGM 07/11/2012

    End Sub

    Private Sub Startup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Set up the dialog text at runtime according to the application's assembly information.  

        'TODO: Customize the application's assembly information in the "Application" pane of the project 
        '  properties dialog (under the "Project" menu).

        'Application title
        If My.Application.Info.Title <> "" Then
            ApplicationTitle.Text = My.Application.Info.Title
        Else
            'If the application title is missing, use the application name, without the extension
            ApplicationTitle.Text = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If

        'Format the version information using the text set into the Version control at design time as the
        '  formatting string.  This allows for effective localization if desired.
        '  Build and revision information could be included by using the following code and changing the 
        '  Version control's designtime text to "Version {0}.{1:00}.{2}.{3}" or something similar.  See
        '  String.Format() in Help for more information.
        '
        '    Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision)

        Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor)

        'Copyright info
        Copyright.Text = My.Application.Info.Copyright

        'Note that IAx00Login validates DB existence, create it if concern, and loads Application Current Language

        Using myLoginForm As New IAx00Login()
            If myLoginForm.ShowDialog() = DialogResult.OK Then
                'Ax00StartUp.Show()
                'Application.DoEvents()

                Dim ShowBackground As Boolean
#If DEBUG Then
                ShowBackground = False
#Else
                ShowBackground = True
#End If
                Ax00StartUp = New IAx00StartUp(Nothing) With { _
                            .Title = "Loading...", _
                            .WaitText = "", _
                            .Background = ""}

                Dim myBackForm As New IBackground(IAx00MainMDI, Ax00StartUp)
                Application.DoEvents()

                myBackForm.ShowMDI(ShowBackground)

                'Ax00StartUp.Close()
            End If
        End Using

        'AG 21/02/2014 - #1516 at this point services are running. Do not use here the method RuntimePolicyHelper.LegacyV2RuntimeEnabledSuccessfully because fails
        Dim myLogAcciones As New ApplicationLogManager()
        myLogAcciones.CreateLogActivity(My.Application.Info.ProductName & " - ApplicationUSR STARTUP", "Startup_Load (User)", EventLogEntryType.Information, False)

        If Not Ax00StartUp Is Nothing Then
            Ax00StartUp.Close()
            Ax00StartUp = Nothing
        End If

        Me.Close()
    End Sub

    '    Private Sub Startup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    '        'Set up the dialog text at runtime according to the application's assembly information.  

    '        'TODO: Customize the application's assembly information in the "Application" pane of the project 
    '        '  properties dialog (under the "Project" menu).

    '        'Application title
    '        If My.Application.Info.Title <> "" Then
    '            ApplicationTitle.Text = My.Application.Info.Title
    '        Else
    '            'If the application title is missing, use the application name, without the extension
    '            ApplicationTitle.Text = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
    '        End If

    '        'Format the version information using the text set into the Version control at design time as the
    '        '  formatting string.  This allows for effective localization if desired.
    '        '  Build and revision information could be included by using the following code and changing the 
    '        '  Version control's designtime text to "Version {0}.{1:00}.{2}.{3}" or something similar.  See
    '        '  String.Format() in Help for more information.
    '        '
    '        '    Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision)

    '        Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor)

    '        'Copyright info
    '        Copyright.Text = My.Application.Info.Copyright

    '        'Note that IAx00Login validates DB existence, create it if concern, and loads Application Current Language

    '        Using myLoginForm As New IAx00Login()
    '            If myLoginForm.ShowDialog() = DialogResult.OK Then
    '                RunningBackGround = True

    '                Ax00StartUp.Show()
    '                Application.DoEvents()

    '                Ax00StartUpVisible = True
    '                bwPreload.RunWorkerAsync()

    '                Dim ShowBackground As Boolean
    '#If DEBUG Then
    '                ShowBackground = False
    '#Else
    '                ShowBackground = True
    '#End If
    '                Dim myBackForm As New IBackground(IAx00MainMDI, Ax00StartUp)

    '                Ax00StartUp.RefreshLoadingImage()
    '                Application.DoEvents()

    '                myBackForm.ShowMDI(ShowBackground)

    '                RunningBackGround = False
    '            End If
    '        End Using

    '        'Wait for Ax00StartUp to be closed
    '        While Ax00StartUpVisible
    '            System.Threading.Thread.Sleep(100)
    '        End While

    '        Ax00StartUp.Close()

    '        Me.Close()
    '    End Sub

    'Private Sub bwPreload_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwPreload.DoWork
    '    While RunningBackGround
    '        Ax00StartUp.UIThread(New Action(AddressOf Ax00StartUp.RefreshLoadingImage))
    '        Application.DoEvents()
    '        System.Threading.Thread.Sleep(100)
    '    End While

    '    Ax00StartUpVisible = False
    'End Sub

End Class

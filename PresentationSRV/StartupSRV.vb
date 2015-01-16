Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.Global

Public NotInheritable Class StartupSRV

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        'SGM 01/02/2012 - Set that is Service Assembly - Bug #1112
        GlobalBase.IsServiceAssembly = True

        ' Add any initialization after the InitializeComponent() call.

        'SGM 09/01/2012 - activation of compatibility between Framework 4.5 and Mixed Mode Assemblies
        'AG 21/02/2014 - #1516 at this point services can be stopped (move to the Load event)
        'IMPORTANT!!! Leave the call to x because otherwise some processes like load rsat fails
        ''Dim myLogAcciones As New ApplicationLogManager()
        If RuntimePolicyHelper.LegacyV2RuntimeEnabledSuccessfully Then
            'GlobalBase.CreateLogActivity(My.Application.Info.ProductName & " - Application STARTUP", "StartupSRV.New", EventLogEntryType.Information, False)
        Else
            'GlobalBase.CreateLogActivity(My.Application.Info.ProductName & " - LegacyV2RuntimeEnabled error", "StartupSRV.New", EventLogEntryType.Error, False)
        End If
        'end SGM 09/01/2012

        ''SGM 07/11/2012 - log Application Startup
        ''Dim myLogAcciones As New ApplicationLogManager()
        'GlobalBase.CreateLogActivity(My.Application.Info.ProductName & " - Application STARTUP", "StartupSRV.New", EventLogEntryType.Information, False)
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
                Dim ShowBackground As Boolean
#If DEBUG Then
                ShowBackground = False
#Else
                ShowBackground = True
#End If
                Dim myBackForm As New IBackground(Ax00ServiceMainMDI, Nothing)
                myBackForm.ShowMDI(ShowBackground)
            End If
        End Using

        'AG 21/02/2014 - #1516 at this point services are running. Do not use here the method RuntimePolicyHelper.LegacyV2RuntimeEnabledSuccessfully because fails
        'Dim myLogAcciones As New ApplicationLogManager()
        GlobalBase.CreateLogActivity(My.Application.Info.ProductName & " - ApplicationSRV STARTUP", "Startup_Load (service)", EventLogEntryType.Information, False)

        Me.Close()
    End Sub

End Class

Imports System.Windows.Forms

Public Interface IMainMDI

    Property ErrorStatusLabelDisplayStyle() As ToolStripItemDisplayStyle
    Property ErrorStatusLabelText As String
    WriteOnly Property CurrentLanguage() As String

    Sub InitializeMarqueeProgreesBar()
    Sub StopMarqueeProgressBar()
    Sub ReleaseLIS()
    Sub InitializeAnalyzerAndWorkSession(ByVal pStartingApplication As Boolean)
    Sub EnableButtonAndMenus(ByVal pEnabled As Boolean, Optional ByVal pForceValue As Boolean = False)
    Sub SetActionButtonsEnableProperty(ByVal pEnable As Boolean)
    Sub OpenMonitorForm(ByRef FormToClose As Form, Optional ByVal pAutomaticProcessFlag As Boolean = False)
    Sub CloseForm(FormToClose As Biosystems.Ax00.PresentationCOM.BSBaseForm)
    Sub CheckAnalyzerCompatibility()
End Interface
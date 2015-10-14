Imports System.Windows.Forms

Public Class EmptyMainMDI
    Implements IMainMDI

    Public WriteOnly Property CurrentLanguage() As String Implements IMainMDI.CurrentLanguage
        Set(ByVal value As String)

        End Set
    End Property



    Public Property ErrorStatusLabelDisplayStyle() As ToolStripItemDisplayStyle Implements IMainMDI.ErrorStatusLabelDisplayStyle
        Get
            Return Nothing
        End Get
        Set(ByVal value As ToolStripItemDisplayStyle)

        End Set
    End Property

    Public Property ErrorStatusLabelText() As String Implements IMainMDI.ErrorStatusLabelText
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)

        End Set
    End Property

    Public Sub InitializeMarqueeProgreesBar() Implements IMainMDI.InitializeMarqueeProgreesBar

    End Sub

    Public Sub StopMarqueeProgressBar() Implements IMainMDI.StopMarqueeProgressBar

    End Sub

    Public Sub ReleaseLIS() Implements IMainMDI.ReleaseLIS

    End Sub

    Public Sub InitializeAnalyzerAndWorkSession(ByVal pStartingApplication As Boolean) Implements IMainMDI.InitializeAnalyzerAndWorkSession

    End Sub

    Public Sub EnableButtonAndMenus(ByVal pEnabled As Boolean, Optional ByVal pForceValue As Boolean = False) Implements IMainMDI.EnableButtonAndMenus

    End Sub

    Public Sub SetActionButtonsEnableProperty(ByVal pEnable As Boolean) Implements IMainMDI.SetActionButtonsEnableProperty

    End Sub

    Public Sub OpenMonitorForm(ByRef FormToClose As Form, Optional ByVal pAutomaticProcessFlag As Boolean = False) Implements IMainMDI.OpenMonitorForm

    End Sub

    Public Sub CloseForm(FormToClose As Biosystems.Ax00.PresentationCOM.BSBaseForm) Implements IMainMDI.CloseForm
        If FormToClose IsNot Nothing Then
            FormToClose.Close()
        End If
    End Sub

    Public Function CheckAnalyzerCompatibility() As Boolean Implements IMainMDI.CheckAnalyzerCompatibility

    End Function
End Class


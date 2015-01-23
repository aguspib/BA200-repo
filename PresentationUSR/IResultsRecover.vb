Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL

Public Class UiResultsRecover
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
    End Sub
#End Region

#Region "Declarations"

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 01/08/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            'Get the current Language from the current Application Session
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            bsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RESULTS_RECOVER_TITLE", currentLanguage)
            bsRecoverLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RESULTS_RECOVERING", currentLanguage)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization according the information received
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 01/08/2012
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 70)


            'Set multilanguage texts for all screen labels and tooltips...
            GetScreenLabels()
            Me.Opacity = 100


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".InitializeScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 05/06/2012
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_WINDOWPOSCHANGING Then
            Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, _
                                                                                             GetType(WINDOWPOS)),  _
                                                                                             WINDOWPOS)
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
            pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 70
            Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
        End If

        MyBase.WndProc(m)

    End Sub


#Region "Public Methods"

#End Region

#Region "Events"

    ''' <summary>
    ''' Load form
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 01/08/2012
    ''' </remarks>
    Private Sub IResultsRecover_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '
        Try
            InitializeScreen()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IResultsRecover_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IResultsRecover_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        ''
    End Sub


#End Region


#End Region

End Class
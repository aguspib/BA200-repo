Imports System.Drawing
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
'Imports Biosystems.Ax00.PresentationCOM
Imports System.Windows.Forms
Imports Biosystems.Ax00.CommunicationsSwFw  ' XBC 05/06/2012

Public NotInheritable Class UiInstrumentInfo
    Inherits BSBaseForm

#Region "Declarations"

    Private LanguageID As String
    Private mdiAnalyzerCopy As AnalyzerManager ' XBC 05/06/2012

#End Region

#Region "Events"

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 14/05/2012
    ''' </remarks>
    Private Sub IInstrumentInfo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        Try
            If (e.KeyCode = Keys.Escape) Then
                OKButton.PerformClick()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IInstrumentInfo_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IInstrumentInfo_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub IInstrumentInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'DL 28/07/2011
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)
            'END DL 28/07/2011

            ' XBC 05/06/2012
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
            End If
            ' XBC 05/06/2012

            'Get the current Language from the current Application Session
            'Dim LanguageIDGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage

            GetScreenLabels()
            PrepareButtons()
            LoadImageLogo()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IInstrumentInfo_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IInstrumentInfo_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
        Me.Close()
    End Sub

#End Region

#Region "Methods"

    ''' <summary>
    ''' Sets all label texts in form
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 14/03/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            GroupControl1.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_InstrumentInfo", LanguageID)

            'ToDo: Get the actual value
            Me.SerialNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SERIAL", LanguageID)

            ' XBC 05/06/2012
            'Me.SerialNumberLabel.Text = String.Format("{0}: {1}", Me.SerialNumberLabel.Text, "1234-567-890")
            Me.SerialNumberLabel.Text = String.Format("{0}: {1}", Me.SerialNumberLabel.Text, mdiAnalyzerCopy.ActiveAnalyzer)
            ' XBC 05/06/2012

            'ToDo: Get the actual value
            FirmwareLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirmwareVersion", LanguageID)

            ' XBC 05/06/2012
            'FirmwareLabel.Text = String.Format("{0}: {1}", FirmwareLabel.Text, GlobalBase.FirmwareVersion)
            FirmwareLabel.Text = String.Format("{0}: {1}", FirmwareLabel.Text, mdiAnalyzerCopy.ActiveFwVersion)
            ' XBC 05/06/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the form image icon
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 14/03/2012
    ''' </remarks>
    Private Sub LoadImageLogo()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            auxIconName = GetIconName("INFO")
            If (auxIconName <> "") Then LogoPictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadImageLogo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadImageLogo ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get icons and multilanguage text of tooltip in the active language for all graphical buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 03/11/2011
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then OKButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            bsScreenToolTips.SetToolTip(OKButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", LanguageID))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

End Class

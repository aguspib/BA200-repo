Imports System.Drawing
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
'Imports Biosystems.Ax00.PresentationCOM
Imports System.Windows.Forms

Public NotInheritable Class IAboutBox
    Inherits BSBaseForm

#Region "Declarations"

    Private LanguageID As String

#End Region

#Region "Fields"

    Private IsUserField As Boolean = True

#End Region

#Region "Properties"

    Public Property IsUser() As Boolean
        Get
            Return IsUserField
        End Get
        Set(ByVal value As Boolean)
            IsUserField = value
        End Set
    End Property

#End Region

#Region "Events"
    ''' <summary>
    ''' When the screen is in ADD or EDIT Mode and the ESC Key is pressed, code of Cancel Button is executed;
    ''' in other case, the screen is closed when ESC Key is pressed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/05/2012
    ''' </remarks>
    Private Sub IAboutBox_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then

                'Escape key should do exactly the same operations as bsExitButton_Click()
                OKButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ProgControls_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgControls_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub IUserAboutBox_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'DL 28/07/2011
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)
            'END DL 28/07/2011

            'Get the current Language from the current Application Session
            'Dim LanguageIDGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage

            GetScreenLabels()
            PrepareButtons()
            LoadImageLogo()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IUserAboutBox_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IUserAboutBox_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
        Me.Close()
    End Sub

#End Region

#Region "Methods"

    ''' <summary>
    ''' Get the Window title in the active language
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 03/11/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            'Form Title
            Dim applicationTitle As String
            If (My.Application.Info.Title <> "") Then
                applicationTitle = My.Application.Info.Title
            Else
                applicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
            End If

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            GroupControl1.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_About", LanguageID)
            GroupControl1.Text &= " " & applicationTitle

            Me.LabelVersion.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Version", LanguageID)

            Me.AppTitle.Text = applicationTitle
            Me.LabelCopyright.Text = My.Application.Info.Copyright
            Me.RightLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RightsReserved", LanguageID)

            'Get the Application Version from the assembly information
            'Dim myUtil As New Utilities.
            Dim myGlobalDataTO As GlobalDataTO

            myGlobalDataTO = Utilities.GetSoftwareVersion()
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Me.LabelVersion.Text = String.Format("{0}: {1}", LabelVersion.Text, myGlobalDataTO.SetDatos)
            Else
                'Error getting value of the Current App. Language. Show it.
                ShowMessage(Name & ".GetLabelsValues", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the image Logo
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 14/03/2012
    ''' </remarks>
    Private Sub LoadImageLogo()
        Try
            Dim auxIconName As String = String.Empty
            Dim iconPath As String = IconsPath

            If IsUser Then
                auxIconName = GetIconName("ABT_BA4")
            Else
                auxIconName = GetIconName("ABT_BA4SRV")
            End If

            If (auxIconName <> String.Empty) Then
                LogoPictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadImageLogo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

End Class

Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Public Class IWSTestSelectionWarning

#Region "Declarations"

    Private LanguageID As String = "ENG"

#End Region

#Region "Fields"

    Private MessageField As String

#End Region

#Region "Properties"

    Public Property Message() As String
        Get
            Return MessageField
        End Get

        Set(ByVal value As String)
            MessageField = value
        End Set
    End Property

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Screen initialization
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 24/05/2012 
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'DL 28/07/2011
            Dim mySize As Size = IAx00MainMDI.Size
            Dim myLocation As Point = IAx00MainMDI.Location
            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            'END DL 28/07/2011

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage

            PrepareButtons()

            LoadTitleLabel()

            MemoEdit1.EditValue = Message

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the images for the form buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 24/05/2012
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'EXIT Button
            auxIconName = GetIconName("ACCEPT1")
            If auxIconName <> "" Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'For Tooltips...
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", LanguageID))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Writes the text of the Title label
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 24/05/2012
    ''' </remarks>
    Private Sub LoadTitleLabel()
        Try
            Dim Messages As New MessageDelegate
            Dim myGlobalDataTO As GlobalDataTO

            myGlobalDataTO = Messages.GetMessageDescription(Nothing, GlobalEnumerates.Messages.FACTORY_VALUES.ToString())

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMessagesDS As MessagesDS
                myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                    bsTitleLabel.Text = myMessagesDS.tfmwMessages(0).MessageText
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadTitleLabel", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadTitleLabel", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ReleaseElements()

        Try
            '--- Detach variable defined using WithEvents ---
            bsExitButton = Nothing
            bsScreenToolTips = Nothing
            bsNotPosWarningGroupBox = Nothing
            bsTitleLabel = Nothing
            MemoEdit1 = Nothing
            '-----------------------------------------------
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

#End Region

#Region "Events"
    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 24/05/2012
    ''' </remarks>
    Private Sub IWSTestSelectionWarning_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                bsExitButton.PerformClick()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WSNotPosWarning_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WSNotPosWarning_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IWSTestSelectionWarning_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ScreenLoad()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WSNotPosWarning_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WSNotPosWarning_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If m.Msg = WM_WINDOWPOSCHANGING Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)
                Dim myLocation As Point = IAx00MainMDI.Location
                Dim mySize As Size = IAx00MainMDI.Size

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WndProc ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

End Class
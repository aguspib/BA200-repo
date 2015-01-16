Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.BL.Framework

Imports System.Drawing 'SG 03/12/10
Imports System.Windows.Forms 'SG 03/12/10


Public Class IConfigLanguage

#Region "Declarations"
    Private OriginalLanguage As String = ""
    Public CurrentLanguage As String 'SG 03/12/10
#End Region

#Region "Constructor" 'SG 03/12/10
    'Public Sub New(ByRef myMDI As Form)
    Public Sub New()
        'RH 12/04/2012 myMDI not needed.

        'MyBase.New()
        'MyBase.SetParentMDI(myMDI)
        InitializeComponent()

    End Sub
#End Region

#Region "Methods"
    ''' <summary>
    ''' Screen initialization (code moved here from the screen load event)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 17/09/2010
    ''' Modified by: PG 07/10/2010 - Get the current Language
    ''' </remarks>
    Private Sub ScreenLoad()
        Try

            'DL 28/07/2011
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)
            'END DL 28/07/2011

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            PrepareButtons()
            GetScreenLabels(currentLanguage)

            GetLanguages()
            GetCurrentLanguage()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_WINDOWPOSCHANGING Then
            Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, _
                                                                                             GetType(WINDOWPOS)),  _
                                                                                             WINDOWPOS)
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
            pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60
            Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
        End If

        MyBase.WndProc(m)

    End Sub

    ''' <summary>
    ''' Get the list of available Languages and load the ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by : VR 14/05/2010 
    ''' Modified by: SG 09/12/2010 - Filter the Service Languages
    ''' </remarks>
    Private Sub GetLanguages()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.LANGUAGES)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreLoadedMasterDataDS As New PreloadedMasterDataDS
                myPreLoadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)


                If (myPreLoadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then

                    'XBC 03/11/2011
                    ''SG 09/12/2010
                    'Select Case My.Application.Info.ProductName.ToUpper
                    '    Case "BAX00"
                    '        Me.bsLangComboBox.DataSource = myPreLoadedMasterDataDS.tfmwPreloadedMasterData
                    '    Case "AX00 SERVICE"
                    '        Dim srvLangs As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                    '        srvLangs = (From a In myPreLoadedMasterDataDS.tfmwPreloadedMasterData Where a.ItemID = "ENG" Or a.ItemID = "SPA" Select a).ToList
                    '        Me.bsLangComboBox.DataSource = srvLangs

                    'End Select

                    'SGM 01/02/2012 - Set that is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        Dim srvLangs As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                        srvLangs = (From a In myPreLoadedMasterDataDS.tfmwPreloadedMasterData Where a.ItemID = "ENG" Or a.ItemID = "SPA" Select a).ToList
                        Me.bsLangComboBox.DataSource = srvLangs
                    Else
                        Me.bsLangComboBox.DataSource = myPreLoadedMasterDataDS.tfmwPreloadedMasterData
                    End If
                    'XBC 03/11/2011

                    bsLangComboBox.ValueMember = "ItemID"
                    bsLangComboBox.DisplayMember = "FixedItemDesc"
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetLanguages", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetLanguages", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get the current application language and set it as value selected in the ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 25/05/2010 
    ''' </remarks>
    Private Sub GetCurrentLanguage()
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            Dim myUserSettingDelegate As New UserSettingsDelegate

            ' XBC 12/11/2012 - distinct between Service and User Software
            'myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE.ToString)

            'SGM 01/02/2012 - Set that is Service Assembly - Bug #1112
            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If GlobalBase.IsServiceAssembly Then
                myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE_SRV.ToString)
            Else
                myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE.ToString)
            End If
            ' XBC 12/11/2012 

            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                bsLangComboBox.SelectedValue = CType(myGlobalDataTO.SetDatos, String)
                OriginalLanguage = bsLangComboBox.SelectedValue.ToString
            Else
                'Error getting the list of available Languages, show it
                ShowMessage(Me.Name & ".SaveCurrentLanguage", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetCurrentLanguage", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetCurrentLanguage", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get the Icons for the screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 03/11/2010 - Load Icon in the Image Property instead of in BackgroundImage Property
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1") 'AG 16/06/2010 auxIconName = GetIconName("ACCEPT")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsCancelButton.Image = Image.FromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the selected language has been changed, save it as current language and execute the changes needed 
    ''' to refresh labels of all opened forms
    ''' </summary>
    ''' <remarks>
    ''' Created by:  VR  14/05/2010
    ''' Modified by: SA  06/07/2010 - Changed the way of calling the Update function
    '''              SA  06/10/2010 - Update the ApplicationSessionInfo
    '''              XBC 19/10/2010 - Translates the text of the Alarms with the selected language
    '''              SA  02/11/2010 - Inform value of property CurrentLanguage of the Main MDI with the selected
    '''                               Language to change values of menu options and button tooltips in it
    ''' </remarks>
    Private Sub SaveCurrentLanguage()
        Try
            Me.Cursor = Cursors.WaitCursor 'AG 13/10/2010

            Dim myResult As New GlobalDataTO
            Dim myUserSettings As New UserSettingsDelegate

            ' XBC 12/11/2012 - distinct between Service and User Software
            'myResult = myUserSettings.Update(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE.ToString, bsLangComboBox.SelectedValue.ToString)

            'SGM 01/02/2012 - Set that is Service Assembly - Bug #1112
            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If GlobalBase.IsServiceAssembly Then
                myResult = myUserSettings.Update(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE_SRV.ToString, bsLangComboBox.SelectedValue.ToString)
            Else
                myResult = myUserSettings.Update(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE.ToString, bsLangComboBox.SelectedValue.ToString)
            End If
            ' XBC 12/11/2012 

            If (Not myResult.HasError) Then
                'Update the Current Language property in session object ApplicationInfoSessionTO
                SetApplicationSessionInfo(bsLangComboBox.SelectedValue.ToString)

                'AG 21/06/2010 - No message
                'ShowMessage(Me.Name & ".SaveChanges", "SUCCESSFUL_DB_ACTION")

                'XBC 19/10/2010 - Translates the text of the Alarms with the selected language
                Dim myAlarmsDelegate As New AlarmsDelegate
                myResult = myAlarmsDelegate.UpdateLanguageResource(Nothing, bsLangComboBox.SelectedValue.ToString)
                If (Not myResult.HasError) Then
                    'Set value of the Language Property of the Main MDIForm
                    MyClass.CurrentLanguage = bsLangComboBox.SelectedValue.ToString
                Else
                    'Error updating the Alarms descriptions; show it
                    Me.Cursor = Cursors.Default
                    ShowMessage(Me.Name & ".SaveCurrentLanguage", myResult.ErrorCode, myResult.ErrorMessage)
                End If
                'XBC 19/10/2010 - Translates the text of the Alarms with the selected language
            Else
                'Error updating the current Application Language, show it
                Me.Cursor = Cursors.Default
                ShowMessage(Me.Name & ".SaveCurrentLanguage", myResult.ErrorCode, myResult.ErrorMessage)
            End If

        Catch ex As Exception
            Me.Cursor = Cursors.Default  'AG 13/10/2010
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveCurrentLanguage", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveCurrentLanguage", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            'RH 13/04/2012 Move this line here because we want to restore the default cursor anyway,
            'whenever there is a raised exception or not.
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' To set the Application Session Info.  Create the New Session and, if session already exists then reset 
    ''' the session and create a new one with the new selected Language
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 06/10/2010 
    ''' </remarks>
    Private Sub SetApplicationSessionInfo(ByVal pNewLanguageID As String)
        Try
            'Get current Session Values before reset the Session
            'Dim currentSession As New GlobalBase
            Dim currentUserID As String = GlobalBase.GetSessionInfo().UserName.Trim.ToString
            Dim currentUserLevel As String = GlobalBase.GetSessionInfo().UserLevel.Trim.ToString

            Dim resetStatus As Boolean = False
            Dim myApplicationSessionManager As New ApplicationSessionManager

            If (myApplicationSessionManager.SessionExist) Then
                resetStatus = ResetApplicationInfoSession()
                If (resetStatus) Then
                    InitializeApplicationInfoSession(currentUserID, currentUserLevel, pNewLanguageID)
                Else
                    ShowMessage(Me.Name & ".SetApplicationSessionInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, , Me)
                End If
            Else
                InitializeApplicationInfoSession(currentUserID, currentUserLevel, pNewLanguageID)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetApplicationSessionInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetApplicationSessionInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by: PG 06/10/10
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsLangConfigTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_LangConfig", pLanguageID)
            bsLanguagesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LangConfig_SelectLang", pLanguageID) + ":"

            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel&Close", pLanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"

    Private Sub LanguageConfig_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            ScreenLoad()

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Config Language LOAD (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IConfigLanguage.LanguageConfig_Load", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LanguageConfig_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LanguageConfig_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 08/11/2010 
    ''' </remarks>
    Private Sub LanguageConfig_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'Close()
                'RH 01/07/2011 Escape key should do exactly the same operations as bsCancelButton_Click()
                bsCancelButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LanguageConfig_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LanguageConfig_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'Save the selected Language as the current one and refresh opened screens
            If (bsLangComboBox.SelectedValue.ToString <> OriginalLanguage) Then
                SaveCurrentLanguage()
            End If
            Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
            Me.Close()


            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity(" Change Lang and Close Screen (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IConfigLanguage.bsAcceptButton_Click", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsAcceptButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsAcceptButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            'AG 20/09/2010 - Cancel button closes the screen without messages and without business
            'If (bsLangComboBox.SelectedValue.ToString <> OriginalLanguage) Then
            '    'Show Save Pending question message
            '    If (ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SAVE_PENDING.ToString) = Windows.Forms.DialogResult.Yes) Then
            '        'Then works as the Accept Button
            '        SaveCurrentLanguage()
            '    End If
            'End If
            'END AG 20/09/2010
            Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
            Me.Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

End Class

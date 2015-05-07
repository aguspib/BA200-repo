Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Imports System.Drawing 'SG 03/12/10
Imports System.Windows.Forms 'SG 03/12/10

Public Class UiPassword

#Region "Definitions"
    'Private changesMade As Boolean = False
    Public UserPassword As String 'SG 03/12/10
    Private IsService As Boolean = False
    'TR 28/11/2011 -Variable use to indicate if it's a user change.
    Public IsUserChange As Boolean = False

#End Region

#Region "Methods"
    'SG 03/12/10
    'Public Sub New(ByRef myMDI As Form)
    Public Sub New()
        'RH 12/04/2012 myMDI not needed.

        MyBase.New()
        'SetParentMDI(myMDI)
        InitializeComponent()

        'SGM 01/02/2012 - Set that is Service Assembly - Bug #1112
        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
        If GlobalBase.IsServiceAssembly Then
            Me.bsUserIDTextBox.Text = "SERVICE" 'QUITAR
            MyClass.IsService = True
        End If

    End Sub

    ''' <summary>
    ''' Method incharge to load the button images
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 05/05/2010
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' To Load the Change password Details using the structure tcfgUserData
    ''' </summary>
    ''' <returns>Dataset with structure of table tcfgUserData</returns>
    ''' <remarks>
    ''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls 
    '''                              to the generic function ShowMessage
    ''' </remarks>
    Private Function LoadChangePwdDetailsDS() As UserDataDS
        Dim userDetailsData As New UserDataDS

        Try
            Dim secObject As New Security.Security
            Dim userDetailsRow As UserDataDS.tcfgUserDataRow
            userDetailsRow = userDetailsData.tcfgUserData.NewtcfgUserDataRow

            userDetailsRow.UserName = bsUserIDTextBox.Text.Trim
            userDetailsRow.Password = secObject.Encryption(bsCurPasswordTextBox.Text.Trim)
            userDetailsRow.NewPassword = secObject.Encryption(bsNewPasswordTextBox.Text.Trim)

            userDetailsData.tcfgUserData.Rows.Add(userDetailsRow)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadChangePwdDetailsDS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadChangePwdDetailsDS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return userDetailsData
    End Function

    ''' <summary>
    ''' Load the Change Password Screen
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls 
    '''                              to the generic function ShowMessage
    '''              PG 08/10/2010 - Get the current Language   
    ''' </remarks>
    Private Sub ChangePasswordLoad()
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            Dim dataSessionTO As ApplicationInfoSessionTO
            dataSessionTO = GetApplicationInfoSession()

            If (dataSessionTO.UserName.Length > 0) Then
                bsUserIDTextBox.Text = dataSessionTO.UserName
                bsUserIDTextBox.BackColor = Color.LightGray

                'Load the multilanguage texts for all Screen Labels
                GetScreenLabels(currentLanguage)
            Else
                ShowMessage(Name & ".ChangePasswordLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString)
                Close()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangePasswordLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangePasswordLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save the New Password if it is valid
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              SA 23/06/2010 - Removed confirmation message; changed function calls due to they use now the new template
    ''' </remarks>
    Private Sub SaveChanges()
        'Using LoginObject As New IAx00Login(myParentMDI, IsUserChange)
        Dim returnData As New GlobalDataTO()
        Try
            'Verify if all fields have correct values
            If (ValidateSavingConditions()) Then
                Dim mySecurity As New Security.Security()
                Dim myConfig As New UserConfigurationDelegate()
                returnData = myConfig.UserValidation(Nothing, bsUserIDTextBox.Text.Trim, mySecurity.Encryption(bsCurPasswordTextBox.Text.Trim), IsService)
                If (Not returnData.HasError And Not returnData.SetDatos Is Nothing) Then
                    If DirectCast(returnData.SetDatos, Boolean) Then
                        'Informed User/current Pwd is correct, update the new Password
                        Dim returnPwdData As New GlobalDataTO()
                        returnPwdData = myConfig.ChangePassword(Nothing, bsUserIDTextBox.Text.Trim, _
                                                                mySecurity.Encryption(bsNewPasswordTextBox.Text.Trim), IsService)
                        If (Not returnPwdData.HasError) Then
                            '    'AG 21/06/2010 - No messages
                            'LoginObject.UserPassword = bsPwdConfirmTextBox.Text.Trim
                            DialogResult = Windows.Forms.DialogResult.OK
                        Else
                            '    'Show the error message
                            ShowMessage(Name & ".SaveChanges", returnPwdData.ErrorCode, returnPwdData.ErrorMessage)
                            DialogResult = Windows.Forms.DialogResult.None
                        End If
                        'DL 10/06/2010
                        Close()
                    End If
                Else
                    ' SG 29/07/2010
                    bsScreenErrorProvider.SetError(bsCurPasswordTextBox, GetMessageText(GlobalEnumerates.Messages.WRONG_USER_PASSWORD.ToString))
                    DialogResult = Windows.Forms.DialogResult.None
                End If
            Else
                DialogResult = Windows.Forms.DialogResult.None
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        'End Using
    End Sub

    ''' <summary>
    ''' Verify that all mandatory screen fields are informed and the values are correct
    ''' </summary>
    ''' <returns>True if all mandatory fields are informed and the values are correct;
    '''          otherwise, False</returns>
    ''' <remarks>
    ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Function ValidateSavingConditions() As Boolean
        Dim fieldsOK As Boolean = True

        Try
            bsScreenErrorProvider.Clear()

            If (bsCurPasswordTextBox.TextLength = 0) Then
                fieldsOK = False
                bsScreenErrorProvider.SetError(bsCurPasswordTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            End If

            If (bsNewPasswordTextBox.TextLength = 0) Then
                fieldsOK = False
                bsScreenErrorProvider.SetError(bsNewPasswordTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            End If

            If (bsPwdConfirmTextBox.TextLength = 0) Then
                fieldsOK = False
                bsScreenErrorProvider.SetError(bsPwdConfirmTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            End If

            'TR 27/03/2012 -Validate is not usign the default Admin password.
            If bsUserIDTextBox.Text = "Admin" AndAlso IsDefaultAdminPass() Then
                fieldsOK = False
                bsScreenErrorProvider.SetError(bsNewPasswordTextBox, GetMessageText(GlobalEnumerates.Messages.PWR_NOT_ALLOW.ToString))
            End If


            If (fieldsOK) Then
                'If (bsCurPasswordTextBox.Text.Trim.ToUpper = bsNewPasswordTextBox.Text.Trim.ToUpper) Then
                If (bsCurPasswordTextBox.Text.Trim.ToUpperBS = bsNewPasswordTextBox.Text.Trim.ToUpperBS) Then
                    fieldsOK = False
                    bsScreenErrorProvider.SetError(bsNewPasswordTextBox, GetMessageText(GlobalEnumerates.Messages.PASSWORD_DUPLICATED.ToString))
                    'ElseIf (bsNewPasswordTextBox.Text.Trim.ToUpper <> bsPwdConfirmTextBox.Text.Trim.ToUpper) Then
                ElseIf (bsNewPasswordTextBox.Text.Trim.ToUpperBS <> bsPwdConfirmTextBox.Text.Trim.ToUpperBS) Then
                    fieldsOK = False
                    bsScreenErrorProvider.SetError(bsPwdConfirmTextBox, GetMessageText(GlobalEnumerates.Messages.PASSWORD_CONFIRMATION.ToString))
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateSavingConditions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateSavingConditions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return fieldsOK
    End Function


    ''' <summary>
    ''' Validate if the Admin user is using the default password
    ''' </summary>
    ''' <returns>true if </returns>
    ''' <remarks>CREATED BY: TR 27/03/2012</remarks>
    Private Function IsDefaultAdminPass() As Boolean
        Dim myResult As Boolean = False
        Try
            Dim myGlobalDataTO As GlobalDataTO

            myGlobalDataTO = GeneralSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.INITIAL_KEY)

            If Not myGlobalDataTO.HasError Then
                If bsNewPasswordTextBox.Text = myGlobalDataTO.SetDatos.ToString() Then
                    myResult = True
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateAdminPass", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateAdminPass", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function



    ''' <summary>
    ''' Cancel the Edition
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    '''              by calls to the generic function ShowMessage
    '''              DL 10/06/2010 - If user want to discard changes then Close screeno	Else Continue edition
    '''              SG 29/07/2010
    ''' </remarks>
    Private Sub CancelChangePasswordEdition()
        Try
            'TR 12/01/2010 - Call directly the showMessage Method 
            'If ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes Then
            'Me.Close()
            'Else

            'AG 27/09/2010 - Dont ask and close the screen
            ''SG 29/07/2010
            'If changesMade Then
            '    If ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes Then
            '        Me.DialogResult = Windows.Forms.DialogResult.Cancel
            '        Me.Close()
            '    End If
            'Else
            '    Me.DialogResult = Windows.Forms.DialogResult.Cancel
            '    Me.Close()
            'End If
            ''END SG 29/07/2010
            Close()

            'End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CancelChangePasswordEdition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CancelChangePasswordEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application</param>
    ''' <remarks>
    ''' Created by: PG 08/10/10
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsChangePwdDefLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ChangePassword", pLanguageID)
            bsCurPasswordLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ChgPwd_Current", pLanguageID) + ":"
            bsNewPasswordLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ChgPwd_New", pLanguageID) + ":"
            bsPwdConfirmLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PasswordConfirmation", pLanguageID) + ":"
            bsUserNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_UserID", pLanguageID) + ":"

            'For button ToolTips            
            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", pLanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel&Close", pLanguageID))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

#Region "Events"
    ''' <summary>
    ''' Save the New Password if the Current Password is Valid and After changes open the Main application. 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    '''                              by calls to the generic function ShowMessage
    ''' </remarks> 
    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            SaveChanges()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BsSaveButton_Click " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsSaveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Cancel the Edition
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    '''                              by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub BsCancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            CancelChangePasswordEdition()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BsCancelButton_Click " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010 
    ''' </remarks>
    Private Sub ChangePassword_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                CancelChangePasswordEdition()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangePassword_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangePassword_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the Change password Screen with the Informed valid UserID
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    '''                              by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub ChangePassword_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            PrepareButtons()
            ChangePasswordLoad()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ChangePassword_Load " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangePassword_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the TextBox showing the error is changed
    ''' and to control changes made in fields New and Confirm New Password
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 29/07/2010
    ''' Modified by: SA 10/11/2010 - Added the cleaning of the Error Provider
    ''' </remarks>
    Private Sub TextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCurPasswordTextBox.TextChanged, _
                                                                                                        bsNewPasswordTextBox.TextChanged, _
                                                                                                        bsPwdConfirmTextBox.TextChanged
        Try
            Dim myTextBox As New TextBox
            myTextBox = CType(sender, TextBox)

            If (myTextBox.TextLength > 0) Then
                If (bsScreenErrorProvider.GetError(myTextBox) <> "") Then
                    bsScreenErrorProvider.SetError(myTextBox, String.Empty)
                End If
            End If

            'changesMade = (myTextBox Is bsNewPasswordTextBox Or myTextBox Is bsPwdConfirmTextBox)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "PassFields_TextChanged " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PassFields_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

#Region "TO DELETE??"

    '''' <summary>
    '''' Enable the Save Button if all the mandatory fields are informed
    '''' </summary>
    '''' <remarks>
    '''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls 
    ''''                              to the generic function ShowMessage
    '''' </remarks>
    'Private Sub SaveButtonEnable(ByVal Sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
    '    Try
    '        'If Me.bsUserIDTextBox.Text.Trim.Length <> 0 And Me.bsCurPasswordTextBox.Text.Trim.Length <> 0 And _
    '        '  Me.bsNewPasswordTextBox.Text.Trim.Length <> 0 And Me.bsPwdConfirmTextBox.Text.Trim.Length <> 0 Then
    '        'Me.bsAcceptButton.Enabled = True
    '        'Else
    '        'Me.bsAcceptButton.Enabled = False
    '        'End If

    '        If e.KeyCode = Keys.Enter And Sender Is bsCurPasswordTextBox Then
    '            bsNewPasswordTextBox.Focus()

    '        ElseIf e.KeyCode = Keys.Enter And Sender Is bsNewPasswordTextBox Then
    '            bsPwdConfirmTextBox.Focus()

    '        ElseIf e.KeyCode = Keys.Enter And Sender Is bsPwdConfirmTextBox Then
    '            bsAcceptButton.Focus()
    '            bsAcceptButton.PerformClick()
    '        End If
    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveButtonEnable", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".SaveButtonEnable", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    '''' <summary>
    '''' Enable the Save Button if all the mandatary fields are informed
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>
    '''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    ''''              by calls to the generic function ShowMessage
    '''' </remarks>
    'Private Sub BsPwdConfirmTextBox_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsPwdConfirmTextBox.KeyUp
    '    Try
    '        SaveButtonEnable(sender, e)
    '    Catch ex As Exception
    '        'Write error SYSTEM_ERROR in the Application Log
    '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "BsPwdConfirmTextBox_KeyUp " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        'Show error message
    '        ShowMessage(Me.Name & ".BsPwdConfirmTextBox_KeyUp", "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    '''' <summary>
    '''' Enable the Save Button if all the mandatary fields are informed
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>
    '''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    ''''              by calls to the generic function ShowMessage
    '''' </remarks>

    'Private Sub bsCurPasswordTextBox_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsCurPasswordTextBox.KeyUp
    '    Try
    '        SaveButtonEnable(sender, e)
    '    Catch ex As Exception
    '        'Write error SYSTEM_ERROR in the Application Log
    '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "bsCurPasswordTextBox_KeyUp " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        'Show error message
    '        ShowMessage(Me.Name & ".bsCurPasswordTextBox_KeyUp", "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

    '''' <summary>
    '''' Enable the Save Button if all the mandatary fields are informed
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>
    '''' Modified by: BK 30/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    ''''              by calls to the generic function ShowMessage
    '''' </remarks>
    'Private Sub bsNewPasswordTextBox_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsNewPasswordTextBox.KeyUp
    '    Try
    '        SaveButtonEnable(sender, e)
    '    Catch ex As Exception
    '        'Write error SYSTEM_ERROR in the Application Log
    '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "bsNewPasswordTextBox_KeyUp " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        'Show error message
    '        ShowMessage(Me.Name & ".bsNewPasswordTextBox_KeyUp", "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub
#End Region

End Class
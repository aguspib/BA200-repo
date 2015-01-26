Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.BL.Framework
Imports System.IO
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.BL.UpdateVersion
Imports System.Drawing 'SG 03/12/10
Imports System.Windows.Forms 'SG 03/12/10
Imports Biosystems.Ax00.Types

Public Class UiAx00Login

#Region "Declarations"
    'Private CountDownValue As Long = 0
    Private CurrentAppLanguage As String = "ENG"
    Protected Ax00StartUp As UiAx00StartUp
    Private RunningPreload As Boolean = False
    Private DBServerError As Boolean
    Private IsService As Boolean = False
    Private IsUserChange As Boolean = False  'TR 28/11/2011 - Variable used to indicate if it's an User change
    Private IsPcReaderLaunch As Boolean = False 'AG 27/11/2012 - Assure the Pc info reader is launch only 1 time when app starts

    'Private myNewLocation As New Point 'DL 28/03/2012

#End Region

#Region "Fields"
    Private UserPasswordField As String
#End Region

#Region "Properties"
    ''' <summary>
    ''' To get or set the User Password
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Property UserPassword() As String
        Get
            Return UserPasswordField
        End Get
        Set(ByVal value As String)
            UserPasswordField = value
        End Set
    End Property
#End Region

#Region "Constructor"

    ''' <remarks>
    ''' </remarks>
    Public Sub New(Optional ByVal pIsUserChange As Boolean = False)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        IsUserChange = pIsUserChange

        'DL 28/03/2012
        If IsUserChange Then
            Me.ShowInTaskbar = False
            'Me.Text = " "
        End If

    End Sub

#End Region

#Region "Methods"
    ''' <remarks>
    ''' Validate SQL Server engine and DB availability
    ''' DL 29/08/2011 - Check minimun resolution must be 1024 x 768
    ''' </remarks>
    Private Function CheckMinimunResolution() As Boolean
        Dim boolValidation As Boolean = False

        Try
            Dim intX As Integer = Screen.PrimaryScreen.Bounds.Width
            Dim intY As Integer = Screen.PrimaryScreen.Bounds.Height

            If intX < 1024 Or intY < 768 Then
                boolValidation = True

                Dim myMultiLangDelegate As New MultilanguageResourcesDelegate
                MessageBox.Show(myMultiLangDelegate.GetResourceText(Nothing, "MSG_MINIMAL_RESOLUTION", CurrentAppLanguage), "AX00 User Software", MessageBoxButtons.OK, MessageBoxIcon.Stop)

                'DL 05/12/2012. Exit to application when resolution minimal is not correct. BEGIN
                Application.Exit()
                'DL 05/12/2012. Exit to application when resolution minimal is not correct. END

            End If
        Catch ex As Exception
            boolValidation = True

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CheckDataBaseAvailability ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CheckDataBaseAvailability ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return boolValidation
    End Function

    ''' <remarks>
    ''' DL 29/08/2011 - Check initial validation
    ''' </remarks>
    Private Function CheckInitialValidation() As Boolean
        Dim returnvalue As Boolean = False

        Try
            If Not CheckMinimunResolution() Then
                If Not DBServerError Then
                    returnvalue = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CheckInitialValidation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CheckInitialValidation ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return returnvalue
    End Function

    ''' <remarks>
    ''' Validate SQL Server engine and DB availability
    ''' AG 22/11/2010 - create by copy the code in constructor into this new method
    ''' </remarks>
    Private Sub CheckDataBaseAvailability()
        'Move the RH code into this new method a new worker
        Try
            Dim MyDabaseManagerDelegate As New DataBaseManagerDelegate()

            DBServerError = False
            If Not MyDabaseManagerDelegate.StartSQLService(DAOBase.DBServer) Then
                'wfPreload.Close()

                'RH 27/05/2011 Make this method run in the Main Thread
                Ax00StartUp.UIThread(New Action(AddressOf Ax00StartUp.Close))
                Ax00StartUp = Nothing

                'It is not possible to connect to the DB. Show the error. It is not possible to use the generic
                'ShowMessage function because there is no connection to the DB.
                MessageBox.Show( _
                    String.Format("SQL Server '{0}' not found. {1}Can not connect to the SQL Server engine. The application will exit now. {1}Please, install SQL Server again using BA400 distribution package.", _
                                  DAOBase.DBServer, vbCrLf), _
                                  "AX00 User Software", MessageBoxButtons.OK, MessageBoxIcon.Error)
                DBServerError = True
            Else
                Dim resultData As New GlobalDataTO

                'RH Here the SQL Server engine and Browser services are OK.

                'wfPreload.Title = "Checking database availability and application version..." 'AG - Not multilanguage text
                'wfPreload.WaitText = "Please wait..." 'AG - Not multilanguage text

                'RH 27/05/2011 Make this assignment run in the Main Thread
                Ax00StartUp.SetPropertyThreadSafe("Title", "Checking database availability and application version...")
                Ax00StartUp.SetPropertyThreadSafe("WaitText", "Please wait...")

                'TR 22/06/2010 -belong to the installation proccess
                'AG 16/01/2013 v1.0.1 - InstallUpdateProcess returns a globaldatato
                resultData = InstallUpdateProcess() 'TR Installation

                If Not resultData.HasError Then
                    'AG 16/01/2013 v1.0.1 note:
                    'Keep previous code already developed in v1.0.0 without changes

                    'RH 18/11/2010 ToDo: Validate here what happend if InstallUpdateProcess()
                    'failed and the DB is empty because the restore operation failed

                    'RH 18/04/2012 Try to open a DB connection to verify there is no problem with the DB.
                    'Change the operation order. First try to connect to the DB. Second try to put it in MultiUser state.
                    Dim dbConnection As SqlClient.SqlConnection = Nothing
                    resultData = DAOBase.GetOpenDBConnection(Nothing)

                    If (resultData.HasError) Then
                        'wfPreload.Close()

                        'RH 27/05/2011 Make this method run in the Main Thread
                        Ax00StartUp.UIThread(New Action(AddressOf Ax00StartUp.Close))
                        Ax00StartUp = Nothing

                        'It is not possible to connect to the DB. Show the error. It is not possible to use the generic
                        'ShowMessage function because there is no connection to the DB.
                        MessageBox.Show( _
                            String.Format("Database '{0}' not found. {1}Can not connect to the database. {1}The application will exit now. {1}Please, install Database again using BA400 distribution package.", _
                                          DAOBase.CurrentDB, vbCrLf), _
                                          "BA400 Software", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        DBServerError = True
                    Else
                        'DB Connection is OK, get the opened connection and close it.
                        dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                        dbConnection.Close()

                        'RH 15/07/2011 Now set DB to MultiUser
                        If Not MyDabaseManagerDelegate.SetDataBaseMultiUser(DAOBase.DBServer, DAOBase.CurrentDB, DAOBase.DBLogin, DAOBase.DBPassword) Then
                            'RH 27/05/2011 Make this method run in the Main Thread
                            Ax00StartUp.UIThread(New Action(AddressOf Ax00StartUp.Close))
                            Ax00StartUp = Nothing

                            'It is not possible to connect to the DB. Show the error. It is not possible to use the generic
                            'ShowMessage function because there is no connection to the DB.
                            MessageBox.Show( _
                                String.Format("Database '{0}' is SingleUser. {1}Can not connect to the database. {1}The application will exit now.", _
                                              DAOBase.CurrentDB, vbCrLf), _
                                              "BA400 Software", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            DBServerError = True
                        End If
                    End If
                    'END RH 18/04/2012 

                Else
                    Ax00StartUp.UIThread(New Action(AddressOf Ax00StartUp.Close))
                    Ax00StartUp = Nothing
                    DBServerError = True

                    Dim appVersion As String = ""
                    'TR 21/02/2013 -Get the application Version for the installed Database.
                    Dim myVersionDelegate As New VersionsDelegate
                    Dim myGlobalDataTO As New GlobalDataTO
                    myGlobalDataTO = myVersionDelegate.GetVersionsData(Nothing)
                    If Not myGlobalDataTO.HasError Then
                        If DirectCast(myGlobalDataTO.SetDatos, VersionsDS).tfmwVersions.Count > 0 Then
                            appVersion = DirectCast(myGlobalDataTO.SetDatos, VersionsDS).tfmwVersions(0).UserSoftware
                        End If
                    End If
                    'TR 21/02/2013 -END.
                    If resultData.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_VERSION.ToString Then
                        'Database version higher than application version
                        'Show the next text but not in multilanguage resources. English embedded:
                        Dim MessageBoxResult As DialogResult
                        'ShowMessage function because there is no connection to the DB.

                        MessageBoxResult = MessageBox.Show( _
                            String.Format("BA400 software version {0} cannot work with a higher Database version {1} " & _
                                          "(This situation appears only after installation a previous software version " & _
                                          "without uninstalling database, because automatic downgrade is not possible). {2}" & _
                                          "If you want to load a Restore Point of database version {0}, press the 'Accept' button " & _
                                          "and choose restore point file and restart application and continue working. {2} " & _
                                          "If you want to maintain your database information, press the 'Cancel' button. " & _
                                          "After this you must install BA400 software version {1} (using related distribution package).", _
                                          Application.ProductVersion, appVersion, vbCrLf), "BA400 Software", MessageBoxButtons.OKCancel, MessageBoxIcon.Error)


                        'MessageBoxResult = MessageBox.Show( _
                        '    String.Format("Database can not be downloaded to a previous version. We recommend you to update your application to version {0} {1}If you still want to continue working with old version you have installed you must load the proper restore point labeled with 'End version' that Software had generated automatically", _
                        '                  appVersion, vbCrLf), "BA400 Software", MessageBoxButtons.OKCancel, MessageBoxIcon.Error)

                        If MessageBoxResult = Windows.Forms.DialogResult.OK Then
                            MessageBoxResult = CallRestorePointScreen()
                            If MessageBoxResult = Windows.Forms.DialogResult.OK Then
                                MessageBox.Show("Please, restart the application to apply changes.")
                            End If
                        End If

                    ElseIf resultData.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString Then

                        'Update version process failed
                        'Show the next text but not in multilanguage resources. English embedded:
                        'The updating process to BA400 software version y.y.y has been cancelled due to some error. User database version x.x.x remains without changes. {3}
                        'Please, send us SATreport located on: “Previous Folder Path”, in order to analyze and fix this issue.
                        'Until problem is solved, you can continue working with previous BA400 software versions x.x.x  (same version than user database).  Please, uninstall current BA400 software and install BA400 software version x.x.x (using related distribution package).

                        MessageBox.Show( _
                            String.Format("The updating process to BA400 software version {0} has been cancelled due to some error. User database version {1} remains without changes. {3}" & _
                                          "Please, send us SATreport located on: {2}, in order to analyze and fix this issue. {3}" & _
                                          "Until problem is solved, you can continue working with previous BA400 software versions {1} (same version than user database).  " &
                                          "Please, uninstall current BA400 software and install BA400 software version {1} (using related distribution package){3} ", _
                                          Application.ProductVersion, appVersion, Application.StartupPath & GlobalBase.PreviousFolder, vbCrLf), "BA400 Software", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        ''appVersion, vbCrLf, vbCrLf, vbCrLf, Application.StartupPath & GlobalBase.PreviousFolder, vbCrLf), "BA400 Software", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    ElseIf resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString Then
                        MessageBox.Show(resultData.ErrorMessage, "BA400 Software", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    End If
                End If
                'AG 16/01/2013 v1.0.1
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CheckDataBaseAvailability ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CheckDataBaseAvailability ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Call the Restore point screen for user to load the correspnding 
    ''' Restore point version. Only shows current application version restore points.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR  14/06/2013</remarks>
    Private Function CallRestorePointScreen() As DialogResult
        Dim myResult As DialogResult
        Try
            Dim myIRestPointUpdateProc As New UiRestPointUpdateProc
            myIRestPointUpdateProc.AllowDrop = False 'TR 08/07/2013 -Set the allow Drop to false to avoid exception (bugTracking 1232)
            myIRestPointUpdateProc.RestorePointMode = True
            myResult = myIRestPointUpdateProc.ShowDialog()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CallRestorePointScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CallRestorePointScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function



    ''' <summary>
    ''' Method incharge to load the button images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/05/2010
    ''' Modified by: DL 03/11/2010 - Load the Icon in the Image property instead of in BackgroundImage 
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim iconPath As String = IconsPath
            Dim auxIconName As String = ""

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsLoginButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CHANGE PASSWORD Button
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                bsChangePwdButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To Check the validation of the login screen
    ''' </summary>
    ''' <returns>Validation True or Validation False </returns>
    ''' <remarks>
    ''' Created by:  VR
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls
    '''                              to the generic function ShowMessage
    '''              SA 04/11/2010 - Inform the current language when calling function GetMessageText
    ''' </remarks>
    Private Function Validation() As Boolean
        Dim fieldsOK As Boolean = False
        Dim returnData As GlobalDataTO

        Try
            Dim mySecurity As New Security.Security
            Dim myconfig As New UserConfigurationDelegate

            returnData = myconfig.UserValidation(Nothing, bsUserIDTextBox.Text.Trim, mySecurity.Encryption(bsPasswordTextBox.Text.Trim), IsService)

            If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) AndAlso bsPasswordTextBox.Text.Trim <> "" Then
                If (DirectCast(returnData.SetDatos, Boolean)) Then
                    'Informed User/Pwd is correct
                    SetApplicationSessionInfo(bsUserIDTextBox.Text.Trim, returnData.SetUserLevel.ToString)
                    fieldsOK = True
                End If
            Else
                'TR 21/10/2011 -Implement message unification.
                'If (bsUserIDTextBox.Text.Trim.Length = 0 Or returnData.ErrorCode = GlobalEnumerates.Messages.INVALID_APPLICATION_USER.ToString) Then
                '    BsErrorProvider1.SetError(bsUserIDTextBox, GetMessageText(GlobalEnumerates.Messages.INVALID_APPLICATION_USER.ToString, CurrentAppLanguage))

                '    If (bsPasswordTextBox.Text.Length = 0) Then
                '        BsErrorProvider1.SetError(bsPasswordTextBox, GetMessageText(GlobalEnumerates.Messages.INVALID_APPLICATION_USER.ToString, CurrentAppLanguage))
                '    End If
                'Else
                '    BsErrorProvider1.SetError(bsPasswordTextBox, GetMessageText(GlobalEnumerates.Messages.INVALID_APPLICATION_USER.ToString, CurrentAppLanguage))
                'End If
                If (Not returnData.ErrorCode = String.Empty) Then
                    BsErrorProvider1.SetError(LoginErrorLabel, GetMessageText(GlobalEnumerates.Messages.INVALID_APPLICATION_USER.ToString, CurrentAppLanguage))
                End If
                'TR 21/10/2011 -END.
                fieldsOK = False
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".Validation", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".Validation", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return fieldsOK
    End Function

    ''' <summary>
    ''' To set the Application Session Info.  Create the New Session and, if session already exists then reset 
    ''' the session and create a new one.
    ''' </summary>
    ''' <param name="pUserID">User Identifier</param>
    ''' <param name="pUserLevel">User Level</param>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls
    '''                              to the generic function ShowMessage
    ''' </remarks>
    Private Sub SetApplicationSessionInfo(ByVal pUserID As String, ByVal pUserLevel As String)
        Try
            Dim resetStatus As Boolean = False
            Dim myApplicationSessionManager As New ApplicationSessionManager

            If (myApplicationSessionManager.SessionExist) Then
                resetStatus = ResetApplicationInfoSession()
                If (resetStatus) Then
                    InitializeApplicationInfoSession(pUserID, pUserLevel, CurrentAppLanguage)
                Else
                    ShowMessage(Name & ".SetApplicationSessionInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, , Me)
                End If
            Else
                InitializeApplicationInfoSession(pUserID, pUserLevel, CurrentAppLanguage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetApplicationSessionInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetApplicationSessionInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 05/10/2010
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangDelegate As New MultilanguageResourcesDelegate

            bsUserIDLabel.Text = myMultiLangDelegate.GetResourceText(Nothing, "LBL_UserID", CurrentAppLanguage) + ":"
            bsPasswordLabel.Text = myMultiLangDelegate.GetResourceText(Nothing, "LBL_Password", CurrentAppLanguage) + ":"

            bsScreenToolTips.SetToolTip(bsChangePwdButton, myMultiLangDelegate.GetResourceText(Nothing, "LBL_ChangePassword", CurrentAppLanguage))
            If (Not IsUserChange) Then
                bsScreenToolTips.SetToolTip(bsLoginButton, myMultiLangDelegate.GetResourceText(Nothing, "BTN_Login_Login", CurrentAppLanguage))
                bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangDelegate.GetResourceText(Nothing, "BTN_Login_Exit", CurrentAppLanguage))
            Else
                bsScreenToolTips.SetToolTip(bsLoginButton, myMultiLangDelegate.GetResourceText(Nothing, "BTN_AcceptSelection", CurrentAppLanguage))
                bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangDelegate.GetResourceText(Nothing, "BTN_CancelSelection", CurrentAppLanguage))
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the Change User Password screen if the Informed UserID is valid
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls
    '''                              to the generic function ShowMessage
    '''              SA 04/11/2010 - Inform the current language when calling function GetMessageText
    ''' </remarks>
    Private Sub ChangeUserPassword()
        Try
            Dim returnData As GlobalDataTO
            Dim myConfig As New UserConfigurationDelegate

            If (bsUserIDTextBox.Text.Trim.Length > 0) Then
                returnData = myConfig.UserValidation(Nothing, bsUserIDTextBox.Text.Trim, "", IsService)

                If (Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing) Then
                    If (DirectCast(returnData.SetDatos, Boolean)) Then
                        'Informed User/Pwd is correct
                        SetApplicationSessionInfo(bsUserIDTextBox.Text.Trim, returnData.SetUserLevel.ToString)

                        Using objChangePwd As New UiPassword() ' dl 08/06/2011
                            objChangePwd.IsUserChange = IsUserChange 'TR 28/11/2011
                            objChangePwd.ShowDialog()
                            If (objChangePwd.DialogResult = DialogResult.OK) Then
                                DialogResult = DialogResult.OK
                                Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                                Close()
                            Else
                                bsPasswordTextBox.Focus()
                            End If
                        End Using
                    End If
                Else
                    'Show the error message
                    BsErrorProvider1.Clear()
                    BsErrorProvider1.SetError(bsUserIDTextBox, GetMessageText(GlobalEnumerates.Messages.WRONG_USERNAME.ToString, CurrentAppLanguage))
                    bsUserIDTextBox.Focus()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeUserPassword ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeUserPassword", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Login in the application if the informed UserID and Passsword are valid
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 01/02/2012
    ''' Modified by: XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Sub LoginApplication()
        Try
            If (Validation()) Then
                'TR 27/03/2012 -If validation is OK then validate if User is Admin and Password is the initial. if true the open change pass windows.
                'If bsUserIDTextBox.Text.ToUpper() = "ADMIN" AndAlso IsDefaultAdminPass() Then
                If bsUserIDTextBox.Text.ToUpperBS() = "ADMIN" AndAlso IsDefaultAdminPass() Then
                    ShowMessage(Name, GlobalEnumerates.Messages.CHANGE_ADMIN_PWR.ToString())
                    ChangeUserPassword()
                    bsPasswordTextBox.Clear()
                Else
                    Me.DialogResult = DialogResult.OK
                    Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                    Close()

                    'RH 16/04/2012
                    If (Not IsUserChange) Then
                        ReleaseUnManageControls(Me.Controls)
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoginApplication", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoginApplication", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the Admin user is using the default password
    ''' </summary>
    ''' <returns>true if </returns>
    ''' <remarks>CREATED BY: TR 27/03/2012</remarks>
    Private Function IsDefaultAdminPass() As Boolean
        Dim myResult As Boolean = False
        Try
            Dim myGlobalDataTO As GlobalDataTO
            'Dim myGeneralSettingDS As New GeneralSettingsDS
            Dim myGeneralSettingDelegate As New GeneralSettingsDelegate

            myGlobalDataTO = myGeneralSettingDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.INITIAL_KEY.ToString())

            If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then  'DL 28/03/2012 Add check if setdatos is nothing
                If bsPasswordTextBox.Text = myGlobalDataTO.SetDatos.ToString() Then
                    myResult = True
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateAdminPass", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateAdminPass", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function



    ''' <summary>
    ''' To close the application
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls
    '''                              to the generic function ShowMessage
    ''' </remarks>
    Private Sub ExitApplication(Optional ByVal Ask As Boolean = True)
        Try
            'TR 28/11/2011 -Validate if user change then do not close the application.
            If Not IsUserChange Then
                SetApplicationSessionInfo("", "") 'AG 08/10/2010
                If Not Ask OrElse (ShowMessage(Name & ".ExitApplication", GlobalEnumerates.Messages.EXIT_PROGRAM.ToString, , Me) = DialogResult.Yes) Then
                    'RH 25/10/2010 Wait until Ax00MDBackGround.RunWorkerAsync() is completed
                    'bsLoginButton.Enabled = False
                    'bsExitButton.Enabled = False
                    'AG - Not multilanguage text
                    'AG - Not multilanguage text


                    'DL 20/04/2012
                    'Dim myWaitScreen = New WaitScreen()


                    'DL 17/04/2012
                    'Dim myBackground As String = IconsPath

                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    'myBackground &= "Embedded\ServiceSplash.png"
                    'Else
                    '   myBackground &= "Embedded\UserSplash.png"
                    'End If

                    'Ax00StartUp = New IAx00StartUp(Me) With { _
                    '                .Title = "Waiting ongoing processes completion...", _
                    '                .WaitText = "Please wait...", _
                    '                .Background = myBackground}

                    Dim myStartUp As UiWaitScreen
                    Me.Enabled = False
                    myStartUp = New UiWaitScreen(Me) With { _
                                    .Title = "Waiting ongoing processes completion...", _
                                    .WaitText = "Please wait..."}

                    ''Dim iconPath As String = IconsPath
                    ''Ax00StartUp = New IAx00StartUp(Me) With {.Title = "Waiting ongoing processes completion...", _
                    ''                                         .WaitText = "Please wait...", _
                    ''                                         .Background = iconPath & "Embedded\ServiceSplash.png"} ' dl 08/06/2011

                    'DL 17/04/2012
                    myStartUp.Show() 'dl 20/04/2012
                    'Ax00StartUp.Show()
                    While RunningAx00MDBackGround
                        Application.DoEvents()
                        'Ax00StartUp.RefreshLoadingImage()
                        System.Threading.Thread.Sleep(100)
                    End While

                    'TR 21/10/2011 -Make sure the DetectorForm get close.
                    If Application.OpenForms.Count > 0 Then
                        For Each myform As Form In Application.OpenForms
                            If myform.GetType().Name = "DetectorForm" Then
                                myform.Close()
                                Exit For
                            End If
                        Next
                    End If
                    'TR 21/10/2011 -END.

                    'dl 20/04/2012
                    myStartUp.Close()
                    myStartUp = Nothing
                    'Ax00StartUp.Close()
                    'Ax00StartUp = Nothing
                    'dl 20/04/2012

                    Me.DialogResult = DialogResult.Cancel

                    'SGM 07/11/2012 - log app ends
                    'Dim myLogAcciones As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity(My.Application.Info.ProductName & " - Application END", Name & ".ExitApplication", EventLogEntryType.Information, False)

                    Close()
                End If
            Else
                Me.DialogResult = DialogResult.OK
                Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                Me.Close()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitApplication", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitApplication", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To enable the Change Password button if the UserID is informed.
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls
    '''                              to the generic function ShowMessage
    ''' </remarks>
    Private Sub ChangePasswordButtonStatus()
        Try
            If bsUserIDTextBox.Text.Trim.Length > 0 Then
                bsChangePwdButton.Enabled = True
            Else
                bsChangePwdButton.Enabled = False
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangePasswordButtonStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangePasswordButtonStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    '''  To enable the Change Password button if the Password is informed
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls
    '''                              to the generic function ShowMessage
    ''' </remarks>
    Private Sub LoginButtonStatus(ByVal e As KeyEventArgs)
        Try
            If (e.KeyCode = Keys.Enter) Then
                bsLoginButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoginButtonStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoginButtonStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '''' <summary>
    '''' To disable the Timer and the Login screen after some interval of time
    '''' </summary>
    '''' <remarks>
    '''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls
    ''''                              to the generic function ShowMessage
    '''' </remarks>
    'Private Sub LoginTimerTick()
    '    Try
    '        CountDownValue = CountDownValue - 1 'Decrement the value
    '        If CountDownValue = 0 Then
    '            bsLoginTimer.Enabled = False
    '            myParentMDI.Show()
    '            Hide()
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoginTimerTick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".LoginTimerTick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub

    ''' <summary>
    ''' To activate the Access Control 
    ''' </summary>
    ''' <param name="pValue"></param>
    ''' <remarks>
    ''' Created by:  VR 
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls 
    '''                              to the generic function ShowMessage
    ''' </remarks>
    Private Sub ActivateAccessControl(ByVal pValue As Boolean)
        Try
            'Me.BsAccessControlGroupBox.Visible = pValue
            bsLoginButton.Visible = pValue
            bsExitButton.Visible = pValue

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ActivateAccessControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ActivateAccessControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"
    '''' <summary>
    '''' Send the focus to the Password textbox after load the screen
    '''' </summary>
    '''' <remarks>
    '''' Created by: AG 21/06/2010
    '''' </remarks>
    'Private Sub Ax00Login_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
    '    Try
    '        If bsPasswordTextBox.Enabled And bsPasswordTextBox.Visible Then bsPasswordTextBox.Select()
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".Login_Activated", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".Login_Activated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub


    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010 
    ''' </remarks>
    Private Sub IAx00Login_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                ExitApplication()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IAx00Login_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IAx00Login_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    Private Sub GetFileLogSize()

        Try
            Dim resultdata As New GlobalDataTO
            Dim mySwParameters As New SwParametersDelegate
            Dim myParameterDS As New ParametersDS

            resultdata = mySwParameters.ReadByParameterName(Nothing, _
                                                            GlobalEnumerates.SwParameters.MAX_FILE_LOG_SIZE.ToString, _
                                                            Nothing)

            If Not resultdata.HasError Then
                myParameterDS = DirectCast(resultdata.SetDatos, ParametersDS)
                GlobalConstants.MaxFileLogSize = CType(myParameterDS.tfmwSwParameters.First.ValueNumeric, Long)
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetFileLogSize", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetFileLogSize", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the Login screen
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls 
    '''                              to the generic function ShowMessage
    '''              SA 14/07/2010 - Try to open a DB Connection to verify everything is correct before open the application
    '''                              (DL code was removed)
    '''              SA 05/10/2010 - Get value of the User Setting for the current application Language
    '''              RH 17/11/2010 - SQL Server and DB validation refactoring. 
    ''' </remarks>
    Private Sub Login_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try

#If DEBUG Then
            'TR set the bio user
            bsUserIDTextBox.Text = "BIOSYSTEMS"

            'SGM 01/02/2012 - Set that is Service Assembly - Bug #1112
            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If GlobalBase.IsServiceAssembly Then
                bsUserIDTextBox.Text = "SERVICE"
                bsPasswordTextBox.Text = "BA400"
            Else
                bsPasswordTextBox.Text = "CostaBrava"
            End If


#Else
            bsUserIDTextBox.Clear()
            bsPasswordTextBox.Clear()
#End If

            'TR 28/11/2011 -Validate if is a User Change.
            If Not IsUserChange Then

                'DL 17/04/2012
                Dim myBackground As String = IconsPath

                'SGM 01/02/2012 - Set that is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    myBackground &= "Embedded\ServiceSplash.png"
                Else
                    myBackground &= "Embedded\UserSplash.png"
                End If

                Ax00StartUp = New UiAx00StartUp(Me) With { _
                                .Title = "Starting system required services...", _
                                .WaitText = "Please wait...", _
                                .Background = myBackground}
                'DL 17/04/2012

                Ax00StartUp.Show()

                'AG 22/11/2010 - Start process for checking Database availability & application version
                RunningPreload = True
                bwPreload.RunWorkerAsync()
                'AG - Not multilanguage text

                ''AG 16/01/2013 v1.0.1 - AG 27/11/2012 Move to the Shown event
                ''To activate correction bugsTracking n. 942 you has to comment the next code line

                ''Put these lines here so, both processes go in paralell (bwPreload and Ax00MDBackGround). => A quicker start up.
                ''TR 02/07/2010 -Start a new process to get the ports information; this is made to improve the ports load
                'Ax00MDBackGround.RunWorkerAsync()
                ''TR 02/07/2010 -End
                ''AG 16/01/2013 v1.0.1

                While RunningPreload
                    Application.DoEvents()
                    'Ax00StartUp.RefreshLoadingImage()
                    System.Threading.Thread.Sleep(100)
                End While

            End If

            'SGM 01/02/2012 - Set that is Service Assembly - Bug #1112
            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
            If GlobalBase.IsServiceAssembly Then
                MyClass.IsService = True

                LogoBAUserPicture.Visible = False       'DL 28/03/2012
                LogoBAServicePicture.Visible = True     'DL 28/03/2012
            Else
                LogoBAUserPicture.Visible = True        'DL 28/03/2012
                LogoBAServicePicture.Visible = False    'DL 28/03/2012
            End If

            ' DL 29/08/2011 Check that minimun resolution are 1024x768
            If Not CheckInitialValidation() Then
                ExitApplication(False)
                Return
            Else
                ' dl 24/12/2010
                GetFileLogSize() 'DL 27/01/2012

                bsSoftwareLabel.Text = DirectCast(DirectCast(Biosystems.Ax00.PresentationCOM.My.MyProject.Application,  _
                                                             Biosystems.Ax00.PresentationCOM.My.MyApplication).Info,  _
                                                             Microsoft.VisualBasic.ApplicationServices.AssemblyInfo).Title
                ' END DL

                'TR 07/02/2012 -Get the version information from the assembly information.
                'Dim myUtil As New Utilities.
                Dim myGlobalDataTO As GlobalDataTO
                myGlobalDataTO = Utilities.GetSoftwareVersion()
                If Not myGlobalDataTO.HasError Then
                    bsVersionLabel.Text = "Version: " & myGlobalDataTO.SetDatos.ToString()
                Else
                    'Error getting value of the Current App. Language. Show it.
                    ShowMessage(Name & ".Login_Load", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
                'TR 07/02/2012 -END.

                'RH 16/11/2010 DB Connection is OK. Validated previously.

                'SG 13/10/10 Check if version file exists. If not create it
                'Dim myFolder As String = ConfigurationManager.AppSettings("PCOSInfoFilePath").ToString()
                If Not IsUserChange Then
                    'TR 25/01/2011 -Replace by corresponding value on global base.
                    Dim myFolder As String = GlobalBase.PCOSInfoFilePath
                    'Dim myVersionFileName As String = ConfigurationManager.AppSettings("VersionFileName").ToString()

                    'TR 25/01/2011 -Replace by corresponding value on global base.
                    Dim myVersionFileName As String = GlobalBase.VersionFileName

                    If Not File.Exists(myFolder & myVersionFileName) Then
                        Utilities.CreateVersionFile(myFolder & myVersionFileName)
                    End If

                End If

                'Get value of the Current Application Language to set the global variable
                Dim myUserSettingsDelegate As New UserSettingsDelegate

                ' XBC 12/11/2012 - distinct between Service and User Software
                'myGlobalDataTO = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE.ToString)
                'SGM 01/02/2012 - Set that is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then
                    myGlobalDataTO = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE_SRV.ToString)
                Else
                    myGlobalDataTO = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE.ToString)
                End If
                ' XBC 12/11/2012 

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    CurrentAppLanguage = DirectCast(myGlobalDataTO.SetDatos, String)

                    ' XBC 12/11/2012 - Translates the text of the Alarms with the selected language
                    Dim myAlarmsDelegate As New AlarmsDelegate
                    myGlobalDataTO = myAlarmsDelegate.UpdateLanguageResource(Nothing, CurrentAppLanguage)
                    If myGlobalDataTO.HasError Then
                        'Error getting value of the Current App. Language. Show it.
                        ShowMessage(Name & ".Login_Load", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        Exit Try
                    End If
                    ' XBC 12/11/2012

                    MultilanguageResourcesDelegate.SetCurrentLanguage(CurrentAppLanguage)

                    'Make more initializations
                    PrepareButtons()
                    GetScreenLabels()
                    ActivateAccessControl(True)
                    ChangePasswordButtonStatus()

                    'TR 28/11/2011  validate to close the preloades
                    If Not IsUserChange Then
                        'Close Preload form
                        Ax00StartUp.Close()
                        Ax00StartUp = Nothing
                    End If
                Else
                    'Close Preload form
                    Ax00StartUp.Close()
                    Ax00StartUp = Nothing

                    'Error getting value of the Current App. Language. Show it.
                    ShowMessage(Name & ".Login_Load", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If

                bsUserIDTextBox.Select()
            End If

            'DL 28/03/2012
            If IsUserChange Then
                Dim myMultiLangDelegate As New MultilanguageResourcesDelegate
                TestSortingLabel.Text = myMultiLangDelegate.GetResourceText(Nothing, "MENU_CHANGE_USER", CurrentAppLanguage)
                TestSortingLabel.Size = New Size(280, 20)
                '
                bsUserIDLabel.Location = New Point(12, 46)
                bsUserIDTextBox.Location = New Point(12, 62)
                bsPasswordLabel.Location = New Point(12, 94)
                bsPasswordTextBox.Location = New Point(12, 110)
                '
                bsChangePwdButton.Location = New Point(255, 103)
                '
                TestSortingGB.Size = New Size(300, 145)
                bsLoginButton.Location = New Point(240, 160)
                bsExitButton.Location = New Point(278, 160)
                '
                LogoBAServicePicture.Visible = False
                LogoBAUserPicture.Visible = False
                bsVersionLabel.Visible = False
                bsSoftwareLabel.Visible = False
                '
                LoginErrorLabel.Location = New Point(225, 85)

                Me.Size = New Size(325, 205)

                'RH 30/03/2012
                If Not Me.ParentForm Is Nothing Then
                    Dim NewX As Integer = CInt((Me.ParentForm.Width - Me.Width) / 2)
                    Dim NewY As Integer = CInt((Me.ParentForm.Height - Me.Height) / 2) - 60
                    Me.Location = New Point(NewX, NewY)
                End If

                AcceptButton = bsLoginButton
                'END RH 30/03/2012

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".Login_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".Login_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Open the Change User Password screen
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    '''                              by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub BsChangePwdButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsChangePwdButton.Click
        Try
            ChangeUserPassword()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsChangePwdButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsChangePwdButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Login the Main Application, if the Informed UserID and Passsword are Valid 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    '''                              by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub BsLoginButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsLoginButton.Click
        Try
            LoginApplication()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsLoginButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsLoginButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '''' <summary>
    '''' To disable the Timer and the Login screen after some interval of time
    '''' </summary>
    '''' <remarks>
    '''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    ''''                              by calls to the generic function ShowMessage
    '''' </remarks>
    'Private Sub BsLoginTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsLoginTimer.Tick
    '    Try
    '        LoginTimerTick()
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsLoginTimer_Tick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".BsLoginTimer_Tick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub

    ''' <summary>
    ''' To close the Application
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    '''                              by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub BsExitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try

            ExitApplication()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#Region "Workers"

    ''' <summary>
    ''' To read the computer ports
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Ax00MDBackGround_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles Ax00MDBackGround.DoWork
        'RH 25/10/2010 tells this thread is running...
        RunningAx00MDBackGround = True

        'RH 20/03/2012
        'Register CommAX00.exe COM server
        System.Diagnostics.Process.Start("CommAX00.exe", "/regserver")

        PCInfoReader.GetSystemInfo(True)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 08/10/2010
    ''' Modified by: RH 14/10/2010
    ''' </remarks>
    Private Sub Ax00MDBackGround_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles Ax00MDBackGround.RunWorkerCompleted
        Try
            'RH 14/10/2010
            If (Not e.Error Is Nothing) Then
                CreateLogActivity(e.Error.Message, Name & ".Ax00MDBackGround_DoWork ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
                ShowMessage(Name & ".Ax00MDBackGround_DoWork ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), e.Error.Message)
            End If

            'RH 14/10/2010
            'Long story short: if you dropped a BGW on a form then everything is taken care of automatically, you don't have to help.
            'If you didn't drop it on a form then it isn't an element in a components collection and nothing needs to be done.
            'You don't have to call Dispose().
            'http://stackoverflow.com/questions/2542326/proper-way-to-dispose-of-a-backgroundworker
            'Also: http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/2e691893-8e01-43b1-94e7-a34d78b4012e
            'Ax00MDBackGround.Dispose()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".Ax00MDBackGround_RunWorkerCompleted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".Ax00MDBackGround_RunWorkerCompleted ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            'RH 25/10/2010 tells this thread is finished...
            RunningAx00MDBackGround = False
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 22/11/2010</remarks>
    Private Sub bwPreload_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bwPreload.DoWork
        Try
            CheckDataBaseAvailability()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bwPreload_DoWork ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bwPreload_DoWork ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            RunningPreload = False

        End Try
    End Sub

#End Region

#Region "Installation Methods"

    ''' <summary>
    ''' Update and installation process.
    ''' Modified by: RH 17/11/2010
    ''' Modified by AG 16/01/2013 Define as a Function and return a GlobalDataTo
    ''' </summary>
    Private Shared Function InstallUpdateProcess() As GlobalDataTO
        'Private Shared Sub InstallUpdateProcess()
        ''Dim myLogAcciones As New ApplicationLogManager()
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            Dim mydbmngDelegate As New DataBaseManagerDelegate()

            'GlobalBase.CreateLogActivity(Me.Name & ".Updateprocess -Validating if Data Base exists ", "Installation validation", EventLogEntryType.Information, False)
            myGlobalDataTO = mydbmngDelegate.InstallUpdateProcess(DAOBase.DBServer, DAOBase.CurrentDB, DAOBase.DBLogin, DAOBase.DBPassword)

        Catch ex As Exception
            MessageBox.Show(ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
        Return myGlobalDataTO
        'End Sub
    End Function

#End Region

    ''' <summary>
    ''' To enable the Change Password button if the UserID is informed.
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    '''                              by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub bsUserIDTextBox_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsUserIDTextBox.KeyUp
        Try
            ChangePasswordButtonStatus()

            If (e.KeyCode = Keys.Enter) Then
                bsPasswordTextBox.Focus()
                bsPasswordTextBox.SelectAll()
                e.Handled = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsUserIDTextBox_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsUserIDTextBox_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Move focus to bsLoginButton when rhe user presses Enter User TextBox has the focus
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 09/03/2012
    ''' </remarks>
    Private Sub bsPasswordTextBox_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsPasswordTextBox.KeyUp
        Try
            If (e.KeyCode = Keys.Enter) Then
                bsLoginButton.Focus()
                e.Handled = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsPasswordTextBox_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsPasswordTextBox_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsUserIDTextBox_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsUserIDTextBox.Enter
        BsErrorProvider1.Clear()
    End Sub

    Private Sub bsPasswordTextBox_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPasswordTextBox.Enter
        BsErrorProvider1.Clear()
    End Sub

    'AG 27/11/2012 -To activate correction bugsTracking n. 942 you has to uncomment this method
    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>Created AG 27/11/2012 - Move code from Load and add a new flag IsPcReaderLaunch condition</remarks>
    'Private Sub IAx00Login_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
    '    Try
    '        'Moved from Load event
    '        If Not IsUserChange AndAlso Not IsPcReaderLaunch Then
    '            IsPcReaderLaunch = True

    '            'Put these lines here so, both processes go in paralell (bwPreload and Ax00MDBackGround). => A quicker start up.
    '            'TR 02/07/2010 -Start a new process to get the ports information; this is made to improve the ports load
    '            Ax00MDBackGround.RunWorkerAsync()
    '            'TR 02/07/2010 -End
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IAx00Login_Shown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".IAx00Login_Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

    '    End Try

    'End Sub
    'AG 27/11/2012


    'Private Sub IAx00Login_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
    '    If IsUserChange Then Me.Location = myNewLocation

    '    Try
    '        If IsUserChange Then Me.Location = myNewLocation

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IAx00Login_Move", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".IAx00Login_Move", Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try

    'End Sub
#End Region

    ''' <summary>
    ''' Launch the personal computer info reader
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 16/01/2013 </remarks>
    Private Sub IAx00Login_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            'AG 16/01/2013 v1.0.1 - AG 27/11/2012 Move to the Shown event
            'To activate correction bugsTracking n. 942 you has to comment the next code line

            'Put these lines here so, both processes go in paralell (bwPreload and Ax00MDBackGround). => A quicker start up.
            'TR 02/07/2010 -Start a new process to get the ports information; this is made to improve the ports load
            Ax00MDBackGround.RunWorkerAsync()
            'TR 02/07/2010 -End
            'AG 16/01/2013 v1.0.1

        Catch ex As Exception

        End Try
    End Sub
End Class
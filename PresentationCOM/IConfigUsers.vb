Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global.TO

Imports System.Drawing 'SG 03/12/10
Imports System.Windows.Forms 'SG 03/12/10

Public Class UiConfigUsers

#Region "Declarations"

    Private ModeType As String = "QUERY"
    Private ReadOnly EditionMode As Boolean
    Private ReadOnly ValidationError As Boolean 'SG 29/07/2010

    'Global variable to control in which grid row is the User currently loaded for edition
    Private SelectedUserRow As Integer = -1

    'SA 21/09/2010 - Global variable for the DS linked to the Users DataGridView
    Private UsersDS As New UserDataDS

    'SA 20/09/2010 - Global variables to store the UserName and UserLevel of the connected User,
    '                and also the flag indicating if it is an Internal User
    Private CurrentUserID As String = ""
    Private CurrentUserIsInternal As Boolean = False

    ' XBC 09/06/2011
    Private IsService As Boolean = GlobalBase.IsServiceAssembly ''SGM 01/02/2012 - Set that is Service Assembly - Bug #1112

#End Region

#Region "Methods"

    'SG 03/12/10
    'Public Sub New(ByRef myMDI As Form)
    Public Sub New()
        'RH 12/04/2012 myMDI not needed.

        'MyBase.New()
        'SetParentMDI(myMDI)
        InitializeComponent()

        ' XBC 09/06/2011
        ''SGM 04/04/11
        'If myMDI.Name.ToUpper.Contains("SERVICE") Then
        '    Me.StartPosition = FormStartPosition.Manual
        '    Me.Location = New Point(0, 0)
        '    Me.Size = New Size(978, 593) '978; 593

        '    bsUserDetailsGroupBox.Visible = False
        '    'bsUsersListGroupBox.Height = bsUserDetailsGroupBox.Top - bsUsersListGroupBox.Top + bsUserDetailsGroupBox.Height

        'End If
        ''END SGM 04/04/11

        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
        '    IsService = True
        'End If
        ' XBC 09/06/2011

    End Sub


    ''' <summary>
    ''' Method incharge to load the button images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/04/2010
    ''' Modified by: DL 03/11/2010 - Use the Image property to load the icon instead of property BackgroundImage 
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'ADD Button
            auxIconName = GetIconName("ADD")
            If (auxIconName <> "") Then
                bsNewButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'EDIT Button
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                bsEditButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'DELETE Button
            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                bsDeleteButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'SAVE Button
            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                bsSaveButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Create all columns needed in Users DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 21/09/2010
    ''' Modified by: PG 06/10/2010 - Added pLanguageID as parameter; get the MultiLanguage text for all 
    '''                              visible columns
    ''' </remarks>
    Private Sub InitializeUsersDataGrid(ByVal pLanguageID As String)
        Try
            'Initialize 
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Initialize the grid
            'bsUserDataGridView.RowHeadersVisible = True
            bsUserDataGridView.ColumnHeadersVisible = True
            bsUserDataGridView.AllowUserToAddRows = False
            bsUserDataGridView.AllowUserToDeleteRows = False
            bsUserDataGridView.EditMode = DataGridViewEditMode.EditOnEnter

            bsUserDataGridView.Rows.Clear()
            bsUserDataGridView.Columns.Clear()

            Dim columnName As String
            columnName = "UserLevel"
            bsUserDataGridView.Columns.Add(columnName, "LevelCode")
            bsUserDataGridView.Columns(columnName).Visible = False
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            columnName = "UserLevelDesc"
            bsUserDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_UserLevel", pLanguageID))
            bsUserDataGridView.Columns(columnName).Visible = True
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            columnName = "UserName"
            bsUserDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_UserID", pLanguageID))
            bsUserDataGridView.Columns(columnName).Visible = True
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            columnName = "UserFirstName"
            bsUserDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirstName", pLanguageID))
            bsUserDataGridView.Columns(columnName).Visible = True
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            columnName = "UserLastName"
            bsUserDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LastName", pLanguageID))
            bsUserDataGridView.Columns(columnName).Visible = True
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            columnName = "Password"
            bsUserDataGridView.Columns.Add(columnName, "Password")
            bsUserDataGridView.Columns(columnName).Visible = False
            bsUserDataGridView.Columns(columnName).Width = 0
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            columnName = "NewPassword"
            bsUserDataGridView.Columns.Add(columnName, "NewPassword")
            bsUserDataGridView.Columns(columnName).Visible = False
            bsUserDataGridView.Columns(columnName).Width = 0
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            columnName = "MaxTestsNum"
            bsUserDataGridView.Columns.Add(columnName, "MaxTestsNum")
            bsUserDataGridView.Columns(columnName).Visible = False
            bsUserDataGridView.Columns(columnName).Width = 0
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            columnName = "InternalUser"
            bsUserDataGridView.Columns.Add(columnName, "InternalUser")
            bsUserDataGridView.Columns(columnName).Visible = False
            bsUserDataGridView.Columns(columnName).Width = 0
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            columnName = "TS_User"
            bsUserDataGridView.Columns.Add(columnName, "TS_User")
            bsUserDataGridView.Columns(columnName).Visible = False
            bsUserDataGridView.Columns(columnName).Width = 0
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            columnName = "TS_DateTime"
            bsUserDataGridView.Columns.Add(columnName, "TS_DateTime")
            bsUserDataGridView.Columns(columnName).Visible = False
            bsUserDataGridView.Columns(columnName).Width = 0
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'DL 17/10/2011
            columnName = "IsService"
            bsUserDataGridView.Columns.Add(columnName, "IsService")
            'TR 09/11/2011Set value to false this column is for internal Use.
            bsUserDataGridView.Columns(columnName).Visible = False
            bsUserDataGridView.Columns(columnName).DataPropertyName = columnName
            bsUserDataGridView.Columns(columnName).ReadOnly = True
            bsUserDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            'DL 17/10/2011

            bsUserDataGridView.Columns("UserLevelDesc").Width = CInt((bsUserDataGridView.Width / 6))
            bsUserDataGridView.Columns("Username").Width = CInt(bsUserDataGridView.Width / 4.5)
            bsUserDataGridView.Columns("UserFirstName").Width = CInt(bsUserDataGridView.Width / 3.5)
            bsUserDataGridView.Columns("UserLastName").Width = CInt(bsUserDataGridView.Width / 3.5) + 17
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeUsersDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeUsersDataGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the list of non Internal Users currently defined in the Users DataGridView
    ''' </summary>
    ''' <param name="pCurrentUser">When informed, it indicates this User has to be selected in the grid</param>
    ''' <remarks>
    ''' Modified by: SA 20/09/2010 - Changed the method logic and the way of loading the DataGridView
    ''' </remarks>
    Private Sub FillUsersDataGrid(Optional ByVal pCurrentUser As String = "")
        Try
            'Get the list of non internal Users currently defined
            Dim resultData As New GlobalDataTO
            Dim myUsersDelegate As New UserConfigurationDelegate

            resultData = myUsersDelegate.GetNotInternalList(Nothing, Me.IsService)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                UsersDS = DirectCast(resultData.SetDatos, UserDataDS)

                'Bind the UsersDS as SourceData of the Users DataGridView
                Dim viewUsers As DataView = New DataView
                viewUsers = UsersDS.tcfgUserData.DefaultView
                viewUsers.Sort = "UserLevel DESC, UserName ASC"

                Dim bsUsersBindingSource As BindingSource = New BindingSource() With {.DataSource = viewUsers}
                bsUserDataGridView.DataSource = bsUsersBindingSource

                If (pCurrentUser <> "") Then
                    For i As Integer = 0 To bsUserDataGridView.Rows.Count - 1
                        If (bsUserDataGridView.Rows(i).Cells("UserName").Value.ToString = pCurrentUser) Then
                            bsUserDataGridView.Rows(i).Selected = True
                        ElseIf (bsUserDataGridView.Rows(i).Selected) Then
                            bsUserDataGridView.Rows(i).Selected = False
                        End If
                    Next
                End If
            Else
                'Error getting the list of non internal Users currently defined, show it
                ShowMessage(Me.Name & ".FillUsersDataGrid", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillUsersDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillUsersDataGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get value of General Setting MAX NEW TEST ALLOWED and assign it as Max Limit for the correspondent numeric Up/down
    ''' </summary>
    ''' <remarks>
    ''' Modified by : VR 29/12/2009 - Change the Constant Value to Enum Value
    '''               SA 01/07/2010 - Code changed: it is enough a call to function GetGeneralSettingValue; currently 
    '''                               they are three calls to get the same value
    ''' </remarks>
    Private Sub FillMaxTests()
        Try
            Dim resultData As New GlobalDataTO

            resultData = GeneralSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.MAX_NEW_TESTS_ALLOWED.ToString)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim mySettingValue As String = DirectCast(resultData.SetDatos, String)
                If (mySettingValue <> "") Then
                    bsTestNumberUpDown.Maximum = Convert.ToDecimal(mySettingValue)
                End If
            Else
                'Error getting the value of the General Setting, show it
                ShowMessage(Me.Name & ".FillMaxTests", resultData.ErrorCode, resultData.ErrorMessage)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillMaxTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillMaxTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBox of User Levels
    ''' </summary>
    ''' <remarks>
    ''' Modified by : VR 29/12/2009 - Change the Constant value to Enum value
    '''               SA 19/01/2010 - Change call to PreloadedMasterDataDelegate.GetList for a call to UserLevelsDelegate.GetLevels
    '''               SA 21/09/2010 - Changed the function logic, ComboBox was bad loaded (hardcode values
    '''                               instead of Code+Description got from the UserLevels table in the DB)
    '''               XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub FillUserLevels()
        Try
            'Get the list of existing levels
            Dim myGlobalDataTO As New GlobalDataTO
            Dim userLevelsList As New UsersLevelDelegate

            myGlobalDataTO = userLevelsList.GetLevelsByInternalUseFlag(Nothing, False)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim userLevelsDS As New UsersLevelDS
                userLevelsDS = DirectCast(myGlobalDataTO.SetDatos, UsersLevelDS)

                If (userLevelsDS.tfmwUsersLevel.Rows.Count > 0) Then
                    'Create an additional DS containing only the Operator Level
                    Using operatorLevelDS As New UsersLevelDS()
                        For Each userLevel As UsersLevelDS.tfmwUsersLevelRow In userLevelsDS.tfmwUsersLevel.Rows
                            If (userLevel.UserLevel = "OPERATOR") Then
                                operatorLevelDS.tfmwUsersLevel.ImportRow(userLevel)
                            End If
                        Next
                        'Verify if there is already an User with SUPERVISOR Level loaded in the grid
                        Dim lstUserData As New List(Of UserDataDS.tcfgUserDataRow)()
                        lstUserData = (From a As UserDataDS.tcfgUserDataRow In UsersDS.tcfgUserData _
                        Where a.UserLevel = "SUPERVISOR" _
                        Select a).ToList()
                        'Where a.UserLevel.ToUpper = "SUPERVISOR" _
                        Dim supervisorLoaded As Boolean = (lstUserData.Count = 1)
                        bsLevelComboBox.DataSource = Nothing
                        bsLevelComboBox.Items.Clear()
                        If (Not supervisorLoaded) OrElse (ModeType = "QUERY") Then
                            'There is not a SUPERVISOR, or the data is loaded in ReadOnly Mode:
                            '...all User Levels are loaded in the ComboBox
                            bsLevelComboBox.DataSource = userLevelsDS.tfmwUsersLevel
                        Else
                            If (ModeType = "ADD") Then
                                'There is a SUPERVISOR, only it is possible add new OPERATORS
                                bsLevelComboBox.DataSource = operatorLevelDS.tfmwUsersLevel
                            ElseIf (ModeType = "EDIT") Then
                                'If the selected User is the SUPERVISOR, then this value is also loaded in the ComboBox
                                If (bsUserDataGridView.SelectedRows.Count = 1) Then
                                    'If (bsUserDataGridView.SelectedRows(0).Cells("UserLevel").Value.ToString.ToUpper = "SUPERVISOR") Then
                                    If (bsUserDataGridView.SelectedRows(0).Cells("UserLevel").Value.ToString = "SUPERVISOR") Then
                                        bsLevelComboBox.DataSource = userLevelsDS.tfmwUsersLevel
                                    Else
                                        'Then the selected User is an OPERATOR and only that value has to be loaded in the ComboBox
                                        bsLevelComboBox.DataSource = operatorLevelDS.tfmwUsersLevel
                                    End If
                                Else
                                    'No User is selected in the grid, both Levels are loaded in the ComboBox
                                    bsLevelComboBox.DataSource = userLevelsDS.tfmwUsersLevel
                                End If
                            End If
                        End If

                        ' XBC 09/06/2011
                        If IsService Then
                            'For SERVICE just selected User is an OPERATOR and only that value has to be loaded in the ComboBox
                            bsLevelComboBox.DataSource = operatorLevelDS.tfmwUsersLevel
                        End If
                        ' XBC 09/06/2011

                    End Using
                    bsLevelComboBox.ValueMember = "UserLevel"
                    bsLevelComboBox.DisplayMember = "FixedUserLevelDesc"
                End If
            Else
                'Error getting the list of non internal User Levels, show it
                ShowMessage(Name & ".FillUserLevels ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillUserLevels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillUserLevels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load data of the selected User in the details area
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 21/09/2010 - Changed the method logic
    ''' Modified by: XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Sub FillUserToControls()
        Dim myUserConfiguration As New UserConfigurationDelegate
        Try
            If (bsUserDataGridView.SelectedRows.Count > 0) Then
                'Get data of the currently selected User
                Dim selectedUser As String = bsUserDataGridView.SelectedRows(0).Cells("UserName").Value.ToString
                Dim lstUserData As New List(Of UserDataDS.tcfgUserDataRow)
                lstUserData = (From a As UserDataDS.tcfgUserDataRow In UsersDS.tcfgUserData _
                              Where a.UserName.ToUpperBS = selectedUser.ToUpperBS _
                             Select a).ToList()
                'Where a.UserName.ToUpper = selectedUser.ToUpper _

                If (lstUserData.Count = 1) Then
                    bsUserIDTextBox.Text = lstUserData(0).UserName
                    bsLevelComboBox.SelectedValue = lstUserData(0).UserLevel

                    bsFirstNameTextBox.Text = ""
                    If (Not lstUserData(0).IsUserFirstNameNull) Then
                        bsFirstNameTextBox.Text = lstUserData(0).UserFirstName
                    End If

                    bsLastNameTextBox.Text = ""
                    If (Not lstUserData(0).IsUserLastNameNull) Then
                        bsLastNameTextBox.Text = lstUserData(0).UserLastName
                    End If

                    If (Not lstUserData(0).IsMaxTestsNumNull AndAlso lstUserData(0).MaxTestsNum <> -1) Then
                        bsTestNumberUpDown.Value = lstUserData(0).MaxTestsNum
                    End If

                    Dim mySecurity As New Security.Security
                    Dim passwordText As String = mySecurity.Decryption(lstUserData(0).Password)
                    bsPasswordTextBox.Text = passwordText
                    bsPasswordConfirmTextBox.Text = passwordText
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillUserToControls", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillUserToControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get data of the connected User and shown it as selected in the DataGridView if it is not 
    ''' one of the Internal Users (due to in this case it is not loaded in the grid)
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 21/09/2010 - Changed the method logic
    ''' Modified by: XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Sub UserDetailsLoad()
        Try
            Dim dataSessionTO As New ApplicationInfoSessionTO
            dataSessionTO = GetApplicationInfoSession()

            'Inform the global variables containing the ID and Level of the connected User
            'CurrentUserID = dataSessionTO.UserName.ToUpper 'AG 20/12/2010 - add toUpper
            CurrentUserID = dataSessionTO.UserName.ToUpperBS
            CurrentUserLevel = dataSessionTO.UserLevel

            'Verify if the connected User is an Internal one
            CurrentUserIsInternal = False
            Dim resultData As New GlobalDataTO
            Dim myUserConfiguration As New UserConfigurationDelegate

            ' XBC 09/06/2011
            'resultData = myUserConfiguration.GetAllList(Nothing, dataSessionTO.UserName)
            resultData = myUserConfiguration.GetAllList(Nothing, dataSessionTO.UserName, IsService)
            ' XBC 09/06/2011
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim usersDS As New UserDataDS
                usersDS = DirectCast(resultData.SetDatos, UserDataDS)
                If (usersDS.tcfgUserData.Rows.Count = 1) Then CurrentUserIsInternal = usersDS.tcfgUserData(0).InternalUser
            Else
                'Error getting value of the InternalUser flag for the connected User; show it
                ShowMessage(Name & ".UserDetailsLoad", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

            If (bsUserDataGridView.RowCount > 0) Then
                If (Not CurrentUserIsInternal) Then
                    For i As Integer = 0 To bsUserDataGridView.RowCount - 1
                        'If (bsUserDataGridView.Rows(i).Cells(2).Value.ToString.ToUpper = CurrentUserID.ToUpper) Then
                        If (bsUserDataGridView.Rows(i).Cells(2).Value.ToString.ToUpperBS = CurrentUserID.ToUpperBS) Then
                            bsUserDataGridView.SelectedRows(0).Selected = False
                            bsUserDataGridView.Rows(i).Selected = True
                            Exit For
                        End If
                    Next
                Else
                    'The selected User is the first record in the grid
                    bsUserDataGridView.SelectedRows(0).Selected = False
                    bsUserDataGridView.Rows(0).Selected = True
                End If

                'Load data of the selected User in the details area and shown it in ReadOnlyMode
                FillUserToControls()
            End If

            'Screen is initialized
            InitialModeScreenStatus()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UserDetailsLoad ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UserDetailsLoad ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 20/09/2010
    ''' Modified by: PG 06/10/2010 - Get the current Language 
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Get screen Icons and Labels
            PrepareButtons()
            GetScreenLabels(currentLanguage)

            'Drawn columns of the Users DataGridView and fill it with the list of non Internal Users currently defined
            InitializeUsersDataGrid(currentLanguage)
            FillUsersDataGrid()

            'Fill the ComboBox of User Levels and limits for field maximum number of new Tests
            FillUserLevels()
            FillMaxTests()

            'Get data of the connected User and select it in the grid if it is not an Internal User
            UserDetailsLoad()

            ' XBC 09/06/2011
            If IsService Then
                Me.bsTestNumberLabel.Visible = False
                Me.bsTestNumberUpDown.Visible = False
                ResetBorderSRV()
            End If
            ' XBC 09/06/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenLoad ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Clear all values in the details area
    ''' </summary>
    ''' <remarks>
    ''' Modified by:  SA 20/09/2010 - Removed instructions RemoveHandler/AddHandler
    ''' </remarks>
    Private Sub ClearDetailsArea()
        Try
            bsUserIDTextBox.Text = ""

            If (bsUserDataGridView.SelectedRows.Count > 1) Then
                bsLevelComboBox.SelectedIndex = -1
            Else
                bsLevelComboBox.SelectedIndex = 0
            End If

            bsTestNumberUpDown.Value = bsTestNumberUpDown.Minimum
            bsFirstNameTextBox.Text = ""
            bsLastNameTextBox.Text = ""
            bsPasswordTextBox.Text = ""
            bsPasswordConfirmTextBox.Text = ""

            bsScreenErrorProvider.Clear()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ClearDetailsArea ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ClearDetailsArea ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Change the backcolor of all controls in the details area according the informed Status and the
    ''' current screen mode (Add or Edit)
    ''' </summary>
    ''' <param name="pStatus">True to enable controls in the details area; otherwise False</param>
    ''' <remarks>
    ''' Modified by: SA 22/09/2010 - Changed the method logic. Added parameter pStatus to enable/disable 
    '''                              controls in the details area according its value
    ''' </remarks>
    Private Sub ChangeBackColor(ByVal pStatus As Boolean)
        Try
            bsLevelComboBox.Enabled = pStatus
            bsFirstNameTextBox.Enabled = pStatus
            bsLastNameTextBox.Enabled = pStatus
            bsPasswordTextBox.Enabled = pStatus
            bsPasswordConfirmTextBox.Enabled = pStatus

            If (pStatus) Then
                If (ModeType = "ADD") Then
                    bsUserIDTextBox.Enabled = True
                    bsUserIDTextBox.BackColor = Color.Khaki
                    bsPasswordTextBox.BackColor = Color.Khaki
                    bsPasswordConfirmTextBox.BackColor = Color.Khaki

                ElseIf (ModeType = "EDIT") Then
                    bsUserIDTextBox.Enabled = False
                    bsUserIDTextBox.BackColor = SystemColors.MenuBar
                    bsPasswordTextBox.BackColor = Color.White
                    bsPasswordConfirmTextBox.BackColor = Color.White
                End If

                bsFirstNameTextBox.BackColor = Color.White
                bsLastNameTextBox.BackColor = Color.White

                If (bsLevelComboBox.SelectedIndex = -1) Then
                    bsLevelComboBox.BackColor = Color.Khaki

                    bsTestNumberUpDown.Enabled = False
                    bsTestNumberUpDown.BackColor = SystemColors.MenuBar
                Else
                    bsLevelComboBox.BackColor = Color.White

                    If (bsLevelComboBox.SelectedValue.ToString = "SUPERVISOR") Then
                        bsTestNumberUpDown.Enabled = True
                        bsTestNumberUpDown.BackColor = Color.White
                    Else
                        bsTestNumberUpDown.Enabled = False
                        bsTestNumberUpDown.BackColor = SystemColors.MenuBar
                    End If

                End If
            Else
                bsUserIDTextBox.Enabled = False
                bsUserIDTextBox.BackColor = SystemColors.MenuBar
                bsLevelComboBox.BackColor = SystemColors.MenuBar
                bsTestNumberUpDown.Enabled = False
                bsTestNumberUpDown.BackColor = SystemColors.MenuBar
                bsFirstNameTextBox.BackColor = SystemColors.MenuBar
                bsLastNameTextBox.BackColor = SystemColors.MenuBar
                bsPasswordTextBox.BackColor = SystemColors.MenuBar
                bsPasswordConfirmTextBox.BackColor = SystemColors.MenuBar
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeBackColor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeBackColor ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify  if there are changes pending to save when the user requests a different action
    ''' </summary>
    ''' <returns>True if there are changes pending to Save; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  SA 22/09/2010
    ''' </remarks>
    Private Function ChangesPending() As Boolean
        Dim pendingToSave As Boolean = False
        Try
            If (ModeType = "ADD") Then
                pendingToSave = (bsUserIDTextBox.Text <> "") Or (bsTestNumberUpDown.Value <> 0) Or _
                                (bsFirstNameTextBox.Text <> "") Or (bsLastNameTextBox.Text <> "") Or _
                                (bsPasswordTextBox.Text <> "") Or (bsPasswordConfirmTextBox.Text <> "")
            ElseIf (ModeType = "EDIT") Then
                Dim mySecurity As New Security.Security
                Dim passwordText As String = mySecurity.Encryption(bsPasswordTextBox.Text.ToString)

                pendingToSave = (bsLevelComboBox.SelectedValue.ToString <> bsUserDataGridView.Rows(SelectedUserRow).Cells("UserLevel").Value.ToString) Or _
                                (bsTestNumberUpDown.Value.ToString <> bsUserDataGridView.Rows(SelectedUserRow).Cells("MaxTestsNum").Value.ToString) Or _
                                (bsFirstNameTextBox.Text.ToString <> bsUserDataGridView.Rows(SelectedUserRow).Cells("UserFirstName").Value.ToString) Or _
                                (bsLastNameTextBox.Text.ToString <> bsUserDataGridView.Rows(SelectedUserRow).Cells("UserLastName").Value.ToString) Or _
                                (passwordText <> bsUserDataGridView.Rows(SelectedUserRow).Cells("Password").Value.ToString)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangesPending ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangesPending", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return pendingToSave
    End Function

    ''' <summary>
    ''' Configure the screen controls to set the INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 21/09/2010 - Changed the method logic
    ''' Modified by: XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Sub InitialModeScreenStatus()
        Try
            If (bsUserDataGridView.Rows.Count = 0) Then
                'If there are not Users in the grid it means the Conected User is an Internal one and unique buttons 
                'available are ADD and EXIT - Details area is empty and disable
                bsNewButton.Enabled = True
                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
                bsPrintButton.Enabled = False

                ClearDetailsArea()
                ChangeBackColor(False)

                bsSaveButton.Enabled = False
                bsCancelButton.Enabled = False
            Else
                Dim selectedUser As String = ""
                If (bsUserDataGridView.SelectedRows.Count = 1) Then
                    'selectedUser = bsUserDataGridView.SelectedRows(0).Cells("UserName").Value.ToString.ToUpper 'AG 20/12/2010 - add toUpper
                    selectedUser = bsUserDataGridView.SelectedRows(0).Cells("UserName").Value.ToString.ToUpperBS
                Else
                    ClearDetailsArea()
                End If
                ChangeBackColor(False)

                'If there are Users in the grid, buttons availability will depend on the Connected User
                If (CurrentUserIsInternal) Then
                    'Not Internal Users only can edit their own data
                    bsNewButton.Enabled = True
                    bsEditButton.Enabled = (selectedUser <> "")
                    'bsDeleteButton.Enabled = (selectedUser <> "")
                    bsDeleteButton.Enabled = True
                    bsPrintButton.Enabled = True
                Else
                    bsNewButton.Enabled = False
                    bsEditButton.Enabled = (selectedUser = CurrentUserID)
                    bsDeleteButton.Enabled = False
                    bsPrintButton.Enabled = True
                End If

                'Details area is in ReadOnly mode, buttons are always disabled
                bsSaveButton.Enabled = False
                bsCancelButton.Enabled = False
            End If
            ModeType = "QUERY"
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitialModeScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitialModeScreenStatus ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Code for event click of button bsNewButton
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 22/09/2010
    ''' </remarks>
    Private Sub AddUser()
        Try
            'Verify if a different user was in edition and there are changes pending to save
            Dim cancelAdd As Boolean = False
            If (ChangesPending()) Then
                cancelAdd = (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, "", Me) = Windows.Forms.DialogResult.No)
            End If

            'Prepare the details area to add a new User
            If (Not cancelAdd) Then
                ModeType = "ADD"

                'Reload the ComboBox of User Levels
                FillUserLevels()

                ClearDetailsArea()
                bsLevelComboBox.SelectedIndex = 0
                ChangeBackColor(True)

                bsSaveButton.Enabled = True
                bsCancelButton.Enabled = True

                'Disable buttons EDIT, DELETE and PRINT...
                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
                bsPrintButton.Enabled = False
            End If

            bsUserIDTextBox.Focus()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AddUser ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AddUser", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Code for event click of button bsEditButton
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 22/09/2010
    ''' </remarks>
    Private Sub EditUser()
        Try
            'If several Users are selected in the grid, unselect all except the first one
            If (bsUserDataGridView.SelectedRows.Count > 1) Then
                For i As Integer = bsUserDataGridView.SelectedRows.Count - 1 To 1 Step -1
                    bsUserDataGridView.SelectedRows(i).Selected = False
                Next
            End If

            'Verify if a different user was in edition and there are changes pending to save
            Dim cancelEdit As Boolean = False
            cancelEdit = (ModeType = "EDIT" And bsUserIDTextBox.Text = bsUserDataGridView.SelectedRows(0).Cells("UserName").Value.ToString)

            If (Not cancelEdit) Then
                If (ChangesPending()) Then
                    cancelEdit = (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, "", Me) = Windows.Forms.DialogResult.No)
                End If
            End If

            'Prepare the details area to edit the selected User
            If (Not cancelEdit) Then
                ModeType = "EDIT"
                SelectedUserRow = bsUserDataGridView.CurrentRow.Index

                'Reload the ComboBox of User Levels
                FillUserLevels()
                bsLevelComboBox.SelectedValue = bsUserDataGridView.SelectedRows(0).Cells("UserLevel").Value.ToString

                FillUserToControls()
                ChangeBackColor(True)

                bsSaveButton.Enabled = True
                bsCancelButton.Enabled = True

                'Disable buttons EDIT, DELETE and PRINT...
                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
                bsPrintButton.Enabled = False

                ScreenStatusByUserLevel() 'TR 20/04/2012
            End If

            bsUserIDTextBox.Focus()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EditUser ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EditUser", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load data of the User selected in the grid in the details area, after verifying if there are
    ''' changes pending to save for a previous selected User 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 22/09/2010
    ''' Modified by: SA 19/11/2010 - Before load User's details in the grid, reload the Levels ComboBox
    ''' </remarks>
    Private Sub QueryUser()
        Try
            Dim cancelAction As Boolean = (bsUserIDTextBox.Text = bsUserDataGridView.SelectedRows(0).Cells("UserName").Value.ToString)
            If (Not cancelAction) Then
                If (ChangesPending()) Then
                    If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, "", Me) = Windows.Forms.DialogResult.No) Then
                        cancelAction = True
                    End If
                End If
            End If

            If (Not cancelAction) Then
                ModeType = "QUERY"
                If (bsUserDataGridView.SelectedRows.Count > 1) Then
                    ClearDetailsArea()
                    ChangeBackColor(False)
                    bsEditButton.Enabled = False
                Else
                    'Reload the ComboBox of User Levels
                    FillUserLevels()

                    FillUserToControls()
                    InitialModeScreenStatus()
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".QueryUser ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QueryUser", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load data in controls of the details area in the structure needed to add or modify
    ''' the User in the Database
    ''' </summary>
    ''' <returns>Typed DataSet with structure of table tcfgUsers</returns>
    ''' <remarks></remarks>
    Private Function LoadUserDetailsDS() As UserDataDS
        Dim userDetailsData As New UserDataDS

        Try
            Dim userDetailsRow As UserDataDS.tcfgUserDataRow
            userDetailsRow = userDetailsData.tcfgUserData.NewtcfgUserDataRow

            userDetailsRow.UserName = bsUserIDTextBox.Text.Trim
            userDetailsRow.UserLevel = bsLevelComboBox.SelectedValue.ToString
            userDetailsRow.MaxTestsNum = CInt(bsTestNumberUpDown.Value)
            userDetailsRow.UserFirstName = bsFirstNameTextBox.Text.Trim
            userDetailsRow.UserLastName = bsLastNameTextBox.Text.Trim
            userDetailsRow.InternalUser = False

            Dim secObject As New Security.Security
            userDetailsRow.Password = secObject.Encryption(bsPasswordTextBox.Text.Trim)
            userDetailsData.tcfgUserData.Rows.Add(userDetailsRow)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadUserDetailsDS ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadUserDetailsDS ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return userDetailsData
    End Function

    ''' <summary>
    ''' Load all Users selected in the DataGridView in the structure needed to delete them in the DB
    ''' </summary>
    ''' <returns>Typed DataSet with structure of table tcfgUsers</returns>
    ''' <remarks>
    ''' Modified by: SA 22/09/2010 - Changed the method logic
    ''' </remarks>
    Private Function LoadUsersToDeleteDS() As UserDataDS
        Dim userDetailsData As New UserDataDS

        Try
            Dim userDetailsRow As UserDataDS.tcfgUserDataRow
            For i As Integer = 0 To bsUserDataGridView.SelectedRows.Count - 1
                userDetailsRow = userDetailsData.tcfgUserData.NewtcfgUserDataRow
                userDetailsRow.UserName = bsUserDataGridView.SelectedRows(i).Cells("UserName").Value.ToString.Trim
                userDetailsData.tcfgUserData.Rows.Add(userDetailsRow)
            Next
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadUsersToDeleteDS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadUsersToDeleteDS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return userDetailsData
    End Function

    ''' <summary>
    ''' Validate all fields in the details area are correct before saving changes
    ''' </summary>
    ''' <returns>True if all fields are valid; otherwise, it returns False</returns>
    ''' <remarks>
    ''' Modified by: SA 22/09/2010 - Changed from method to function returning a boolean value
    '''              XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Function ValidateSavingConditions() As Boolean
        Dim fieldsOK As Boolean = True
        Try
            bsScreenErrorProvider.Clear()

            If (bsUserIDTextBox.TextLength = 0) Then
                fieldsOK = False
                bsScreenErrorProvider.SetError(bsUserIDTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                bsUserIDTextBox.Focus()
            ElseIf (bsPasswordTextBox.TextLength = 0) Then
                fieldsOK = False
                bsScreenErrorProvider.SetError(bsPasswordTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                bsPasswordTextBox.Focus()
            ElseIf (bsPasswordConfirmTextBox.TextLength = 0) Then
                fieldsOK = False
                bsScreenErrorProvider.SetError(bsPasswordConfirmTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                bsPasswordConfirmTextBox.Focus()
                'ElseIf (bsPasswordTextBox.Text.Trim.ToUpper <> bsPasswordConfirmTextBox.Text.Trim.ToUpper) Then
            ElseIf (bsPasswordTextBox.Text.Trim.ToUpperBS <> bsPasswordConfirmTextBox.Text.Trim.ToUpperBS) Then
                fieldsOK = False
                bsScreenErrorProvider.SetError(bsPasswordTextBox, GetMessageText(GlobalEnumerates.Messages.PASSWORD_CONFIRMATION.ToString))
                bsPasswordTextBox.Focus()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateSavingConditions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateSavingConditions ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return fieldsOK
    End Function

    ''' <summary>
    ''' Validate if all fields in the details area have a correct value and save (add or modify) the User.
    ''' For event click of bsSaveButton
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SG 29/07/2010 - Reserved UserName message implemented
    '''              SA 22/09/2010 - Changed the method logic
    ''' </remarks>
    Private Sub SaveChanges()
        Try
            Cursor = Cursors.WaitCursor

            If (ValidateSavingConditions()) Then
                Dim returnedData As New GlobalDataTO
                Dim myUserDataToSave As New UserConfigurationDelegate

                Dim userData As New UserDataDS
                userData = LoadUserDetailsDS()

                ' XBC 09/06/2011
                'If (ModeType = "ADD") Then
                '    returnedData = myUserDataToSave.Add(Nothing, userData)
                'ElseIf (ModeType = "EDIT") Then
                '    returnedData = myUserDataToSave.Modify(Nothing, userData)
                'End If
                If (ModeType = "ADD") Then
                    returnedData = myUserDataToSave.Add(Nothing, userData, IsService)
                ElseIf (ModeType = "EDIT") Then
                    returnedData = myUserDataToSave.Modify(Nothing, userData, IsService)
                End If
                ' XBC 09/06/2011

                If (Not returnedData.HasError) Then
                    'Reload the Users Grid, marking as selected the one added or modified
                    SelectedUserRow = -1

                    FillUsersDataGrid(userData.tcfgUserData(0).UserName)
                    FillUserToControls()
                    InitialModeScreenStatus()
                Else
                    'Controlled Errors...UserName already exists or it is one of the used for Internal Users
                    If (returnedData.ErrorCode = GlobalEnumerates.Messages.DUPLICATED_USERNAME.ToString Or _
                        returnedData.ErrorCode = GlobalEnumerates.Messages.RESERVED_USERNAME.ToString) Then
                        bsScreenErrorProvider.Clear()
                        bsScreenErrorProvider.SetError(bsUserIDTextBox, GetMessageText(returnedData.ErrorCode))
                    Else
                        'There was a system error adding or modifying the User; shown it
                        Cursor = Cursors.Default
                        ShowMessage(Name & ".SaveUserDetails", returnedData.ErrorCode, returnedData.ErrorMessage, Me)
                    End If

                    bsUserIDTextBox.Focus()
                End If
            End If
            Cursor = Cursors.Default
        Catch ex As Exception
            Cursor = Cursors.Default
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveChanges ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Cancel the adding/editing of an User. For event click of bsCancelButton
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 22/09/2010 - Changed the method logic
    ''' </remarks>
    Private Sub CancelEdition()
        Try
            Dim cancelAction As Boolean = False
            If (ChangesPending()) Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, "", Me) = Windows.Forms.DialogResult.No) Then
                    cancelAction = True
                End If
            End If

            If (Not cancelAction) Then
                SelectedUserRow = -1

                ClearDetailsArea()
                FillUserToControls()
                InitialModeScreenStatus()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CancelEdition ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CancelEdition ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete the selected Users
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 22/09/2010 - Changed the method logic
    ''' </remarks>
    Private Sub DeleteUsers()
        Dim returnedData As GlobalDataTO
        Try
            If (ShowMessage(Name & ".DeleteUsers ", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString, "", Me) = Windows.Forms.DialogResult.Yes) Then
                Dim userData As New UserDataDS
                userData = LoadUsersToDeleteDS()

                Dim userToDelete As New UserConfigurationDelegate
                ' XBC 22/05/2012
                'returnedData = userToDelete.Delete(Nothing, userData)
                returnedData = userToDelete.Delete(Nothing, userData, IsService)
                ' XBC 22/05/2012
                If (Not returnedData.HasError) Then
                    FillUsersDataGrid()
                    FillUserToControls()
                    InitialModeScreenStatus()
                Else
                    'Error deleting the Users; shown it
                    ShowMessage(Name & ".DeleteUsers", returnedData.ErrorCode, returnedData.ErrorMessage, Me)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteUsers", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteUsers", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
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
            bsFirstNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirstName", pLanguageID) + ":"
            bsLastNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LastName", pLanguageID) + ":"
            bsLevelLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_UserLevel", pLanguageID) + ":"
            bsUserListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Users_List", pLanguageID)
            bsPasswordLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Password", pLanguageID) + ":"
            bsPasswordConfirmLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PasswordConfirmation", pLanguageID) + ":"
            bsTestNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_TestLeft", pLanguageID) + ":" 'DL 20/11/2012 myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Users_TestsNumber", pLanguageID) + ":"
            bsUserDetailLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Users_Details", pLanguageID)
            bsUserNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_UserID", pLanguageID) + ":"

            bsScreenToolTips.SetToolTip(bsNewButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            bsScreenToolTips.SetToolTip(bsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", pLanguageID))
            bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", pLanguageID))
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Enable or disable functionallity by user level.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: TR 20/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    bsTestNumberUpDown.Enabled = False
                    Exit Select

                Case "OPERATOR"
                    bsTestNumberUpDown.Enabled = False
                    Exit Select
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Events"

    ''' <summary>
    ''' Load the screen of Users Configuration
    ''' </summary>
    Private Sub ConfigUsers_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'TR 20/04/2012 get the current user level
            'Dim myGlobalbase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo().UserLevel
            'TR 20/04/2012 -END.

            ScreenLoad()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ConfigUsers_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ConfigUsers_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the screen is in ADD or EDIT mode and ESC key is pressed, code of Cancel Button is executed;
    ''' in other case, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 08/11/2010
    ''' Modified by: RH 01/07/2011 - Escape key should do exactly the same operations as bsExitButton_Click()
    ''' </remarks>
    Private Sub ConfigUsers_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (bsCancelButton.Enabled) Then
                    CancelEdition()
                Else
                    bsExitButton.PerformClick()
                End If

            ElseIf (e.KeyCode = Keys.Enter) Then
                If (bsUserDataGridView.Focused AndAlso bsUserDataGridView.SelectedRows.Count = 1) Then
                    If bsEditButton.Enabled Then EditUser()
                    e.Handled = True
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ConfigUsers_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ConfigUsers_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure the details area to allow enter data for a new User
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsNewButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNewButton.Click
        Try
            AddUser()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsNewButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsNewButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure the details area to allow edit data of the User selected in the grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsEditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsEditButton.Click
        Try
            EditUser()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "bsEditButton_Click" & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsEditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Delete the selected Users
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDeleteButton.Click
        Try
            DeleteUsers()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsDeleteButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDeleteButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save the new or modified User
    ''' </summary>
    ''' <remarks></remarks>    
    Private Sub bsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSaveButton.Click
        Try
            SaveChanges()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSaveButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSaveButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Cancel the adding/modifying of an User
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            CancelEdition()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsCancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen after verifying if there are changes pending to save
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            Dim cancelClose As Boolean = False
            If (ChangesPending()) Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, "", Me) = Windows.Forms.DialogResult.No) Then
                    cancelClose = True
                End If
            End If

            If Not cancelClose Then
                Me.Enabled = False 'RH 13/04/2012 Disable the form to avoid pressing buttons on closing
                Close()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExitButton_Click " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Enabled/disabled the UpDown control for the Maximum Number of Tests depending on the selected UserLevel
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsLevelComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsLevelComboBox.SelectionChangeCommitted
        Try
            bsTestNumberUpDown.Enabled = (bsLevelComboBox.SelectedValue.ToString = "SUPERVISOR")
            If (Not bsTestNumberUpDown.Enabled) Then
                bsTestNumberUpDown.Value = bsTestNumberUpDown.Minimum
                bsTestNumberUpDown.BackColor = SystemColors.MenuBar
            Else
                bsTestNumberUpDown.BackColor = Color.White
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsLevelComboBox_SelectionChangeCommitted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsLevelComboBox_SelectionChangeCommitted ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To select an User in the grid and show its data in details area in ReadOnly mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 23/11/2010 - Code changed from Click event to CellMouseClick event to avoid row selection
    '''                              when the click in the empty grid area or in the header row
    ''' </remarks>
    Private Sub bsUserDataGridView_CellMouseClick(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles bsUserDataGridView.CellMouseClick
        Try
            If (e.RowIndex > -1) Then QueryUser()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsUserDataGridView_CellMouseClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsUserDataGridView_CellMouseClick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To select an User un the grid and show its data in details area in Edit mode
    ''' If the connected User does not have enough rights to edit the selected User, then 
    ''' it is shows in ReadOnly mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 23/11/2010 - Code changed from DoubleClick event to CellMouseDoubleClick event to avoid row selection
    '''                              when the click in the empty grid area or in the header row
    ''' </remarks>
    Private Sub bsUserDataGridView_CellMouseDoubleClick(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles bsUserDataGridView.CellMouseDoubleClick
        Try
            If (e.RowIndex > -1) Then
                If (bsEditButton.Enabled) Then
                    EditUser()
                Else
                    QueryUser()
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsUserDataGridView_CellMouseDoubleClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsUserDataGridView_CellMouseDoubleClick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the user details while moving up and down arrows on rows of grid.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsUserDataGridView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsUserDataGridView.KeyUp
        Try
            If (e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down) Then
                FillUserToControls()
                InitialModeScreenStatus()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsUserDataGridView_KeyUp ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsUserDataGridView_KeyUp ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete the selected Users when key SUPR is pressed.  If the connected User does not have
    ''' enough rights to delete them, then the method do nothing
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsUserDataGridView_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsUserDataGridView.PreviewKeyDown
        Try
            If (e.KeyCode = Keys.Delete) Then
                If (bsDeleteButton.Enabled) Then DeleteUsers()
            ElseIf e.KeyCode = Keys.Enter Then
                If bsUserDataGridView.SelectedRows.Count > 1 Then bsEditButton.Enabled = False
                If bsEditButton.Enabled Then EditUser()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsUserDataGridView_PreviewKeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsUserDataGridView_PreviewKeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the TextBox showing the error is changed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 22/09/2010
    ''' </remarks>
    Private Sub bsTextbox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsUserIDTextBox.TextChanged, _
                                                                                                   bsPasswordTextBox.TextChanged, _
                                                                                                   bsPasswordConfirmTextBox.TextChanged
        Try
            'Dim myTextBox As New TextBox
            Dim myTextBox As TextBox 'RH 02/12/2011 Remove New
            myTextBox = CType(sender, TextBox)

            'If (myTextBox.TextLength > 0) Then
            '    If (bsScreenErrorProvider.GetError(myTextBox) <> "") Then
            '        bsScreenErrorProvider.SetError(myTextBox, String.Empty)
            '    End If
            'End If

            'RH 02/12/2011 Just clear the Error text value in any case
            bsScreenErrorProvider.SetError(myTextBox, String.Empty)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTextbox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTextbox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prints the Users' Report
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 16/11/2010
    ''' </remarks>
    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            XRManager.ShowUsersReport() 'RH 02/12/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsPrintButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsPrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

End Class
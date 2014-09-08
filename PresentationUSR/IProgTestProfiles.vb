Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.PresentationCOM

Public Class IProgTestProfiles
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declarations"
    Private editionMode As Boolean                          'To control if the screen is in Add or Edit Mode and execute the control of Pending Changes
    Private validationError As Boolean                      'To control if errors have been shown
    Private selectedTestProfileID As Integer                'To store the ID of the selected Test Profile 
    Private originalTestProfileName As String = ""          'To store the Name of the selected Test Profile  and control Pending Changes
    Private originalSelectedIndex As Integer = -1           'To store the index of the selected Test Profile and control Pending Changes
    Private originalSelectedTests As New TestProfileTestsDS 'To store the list of Tests included originally in a selected Test Profile and control Pending Changes
#End Region

#Region "Constructor"
    Public Sub New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub
#End Region

#Region "Methods"
    ''' <summary>
    ''' Configure the screen controls to set the ADD MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              DL 13/10/2010 - Filter LoadSelectableTestsList by the selected Test Type
    '''              SA 20/10/2010 - Changes due to new implementation of function LoadSelectableTestsList
    '''              DL 19/07/2011 - Initialized new DoubleList property EnableDisableControls to False
    ''' </remarks>
    Private Sub AddModeScreenStatus()
        Try
            'Area of Test Profiles List
            If (bsTestProfilesListView.Items.Count > 0) Then
                bsTestProfilesListView.Enabled = True
                bsTestProfilesListView.SelectedItems.Clear()
            End If

            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = False 'AG 05/09/2014 - BA-1869
            bsPrintButton.Enabled = False

            'Area of Test Profile Definition
            bsNameTextBox.Text = ""
            bsNameTextBox.Enabled = True
            bsNameTextBox.BackColor = Color.White

            bsTestsSelectionDoubleList.EnableDisableControls = True
            bsTestsSelectionDoubleList.InitializeControl = True

            'The first Sample Type in the list is selected as default
            bsSampleTypesComboBox.SelectedIndex = 0
            bsSampleTypesComboBox.Enabled = True
            bsSampleTypesComboBox.BackColor = Color.White

            'The first Test Type in the list is selected as default
            bsTestsTypesComboBox.SelectedIndex = 0
            bsTestsTypesComboBox.Enabled = True
            bsTestsTypesComboBox.BackColor = Color.White

            'The list of Tests belonging to the selected Sample Type are loaded in the selectable Tests List filtered by STD Tests
            LoadSelectableTestsList(bsSampleTypesComboBox.SelectedValue.ToString)
            bsTestsSelectionDoubleList.TypeToShow = bsTestsTypesComboBox.SelectedValue.ToString

            'Enable Save and Cancel buttons
            bsSaveButton.Enabled = True
            bsCancelButton.Enabled = True

            'Put Focus in the first enabled field
            bsNameTextBox.Focus()

            'Initialize global variables
            CleanGlobalValues()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AddModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AddModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the screen for adding a new Test Profile.  If there are changes pending to save, the Discard Pending Changes Verification 
    ''' is executed
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BA 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage.    
    '''                              Remove '_ForTESTING suffix' from the function name and fix the error name in the call
    ''' </remarks>
    Private Sub AddTestProfile()
        Try
            editionMode = True
            If (PendingChangesVerification()) Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                    AddModeScreenStatus()
                Else
                    If (originalSelectedIndex <> -1) Then
                        'Return focus to the Test Profile that has been edited
                        bsTestProfilesListView.Items(originalSelectedIndex).Selected = True
                        bsTestProfilesListView.Select()
                    End If
                End If
            Else
                'Set Screen Status to ADD MODE
                AddModeScreenStatus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AddTestProfile", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AddTestProfile", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Cancel the adding/edition of a Test Profile
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BA 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage    
    '''                            - Remove '_ForTESTING suffix' from the function name and fix the error name in the call. 
    '''              SA 19/10/2010 - When there are changes pending to save and the user's answer to the shown warning is NOT discard 
    '''                              them, then the screen remains in the same state as before; removed call to SaveChanges
    ''' </remarks>
    Private Sub CancelTestProfileEdition()
        Dim setScreenToInitial As Boolean = False

        Try
            If (PendingChangesVerification()) Then
                If (ShowMessage(Name & ".CancelTestProfileEdition", GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                    setScreenToInitial = True
                Else
                    If (originalSelectedIndex <> -1) Then
                        'Return focus to the Test Profile that has been edited
                        bsTestProfilesListView.Items(originalSelectedIndex).Selected = True
                        bsTestProfilesListView.Select()
                    End If
                End If
            Else
                setScreenToInitial = True
            End If

            If (setScreenToInitial) Then
                bsScreenErrorProvider.Clear()

                If (bsTestProfilesListView.Items.Count = 0) Then
                    'Set screen status to Initial Mode
                    InitialModeScreenStatus()
                Else
                    'Select the first Test Profile in the list and show it in Query Mode or in ReadOnly Mode if it is a InUse Profile
                    bsTestsTypesComboBox.SelectedIndex = 0
                    If (Not LoadDataOfSelectedTestProfile()) Then
                        'Set screen status to Query Mode
                        QueryModeScreenStatus()
                    Else
                        'Set screen status to Read Only Mode
                        ReadOnlyModeScreenStatus()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CancelTestProfileEdition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CancelTestProfileEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the Global Variables used in the screen to store the selected Test Profile and/or to control when there 
    ''' are changes pending to save
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BA 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage    
    ''' </remarks>
    Private Sub CleanGlobalValues()
        Try
            'Initialization of global variables
            selectedTestProfileID = 0
            originalSelectedIndex = -1
            originalTestProfileName = ""

            originalSelectedTests = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CleanGlobalValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CleanGlobalValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete the selected Test Profile
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BA 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''                              Remove 'ForTESTING' for Confirm Deletion 
    '''              SA 27/06/2010 - Removed control of Concurrency Error    
    ''' </remarks>
    Private Sub DeleteTestProfile()
        Dim resultData As New GlobalDataTO
        Try
            If (ShowMessage(Name, GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                Dim setScreenToInitial As Boolean = False
                If (bsTestProfilesListView.SelectedItems.Count = 1) Then
                    'Fill Test Profile basic data
                    Dim testProfileData As New TestProfilesDS
                    testProfileData = LoadTestProfileDS()

                    Dim testProfileToDelete As New TestProfilesDelegate
                    resultData = testProfileToDelete.Delete(Nothing, testProfileData)

                    If (Not resultData.HasError) Then
                        'Refresh the list of Test Profiles and set screen status to Initial Mode
                        CleanGlobalValues()
                        LoadTestProfilesList()
                        setScreenToInitial = True
                    Else
                        'Error deleting the selected Test Profile, show the error message
                        ShowMessage(Name & ".DeleteTestProfile", resultData.ErrorCode, resultData.ErrorMessage, Me)
                    End If

                ElseIf (bsTestProfilesListView.SelectedItems.Count > 1) Then
                    Dim testProfileData As TestProfilesDS = MultiLoadTestProfileDS()

                    Dim testProfileToDelete As New TestProfilesDelegate
                    resultData = testProfileToDelete.Delete(Nothing, testProfileData)

                    If (Not resultData.HasError) Then
                        'Refresh the list of Test Profiles and set screen status to Initial Mode
                        CleanGlobalValues()
                        LoadTestProfilesList()
                        setScreenToInitial = True
                    Else
                        'Error deleting the list of selected Test Profiles, show the error message
                        ShowMessage(Name & ".DeleteTestProfile", resultData.ErrorCode, resultData.ErrorMessage, Me)
                    End If
                End If

                If (setScreenToInitial) Then
                    If (bsTestProfilesListView.Items.Count = 0) Then
                        'Set screen status to Initial Mode
                        InitialModeScreenStatus()
                    Else
                        'Select the first Test Profile in the list and show it in Query Mode or in ReadOnly Mode if it is a InUse Profile
                        bsTestsTypesComboBox.SelectedIndex = 0
                        If (Not LoadDataOfSelectedTestProfile()) Then
                            'Set screen status to Query Mode
                            QueryModeScreenStatus()
                        Else
                            'Set screen status to Read Only Mode
                            ReadOnlyModeScreenStatus()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteTestProfile", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteTestProfile", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure the screen controls to set the EDIT MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              DL 19/07/2011 - Initialized new DoubleList property EnableDisableControls to False
    ''' </remarks>
    Private Sub EditModeScreenStatus()
        Try
            'Area of Test Profiles List
            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = False 'AG 05/09/2014 - BA-1869
            bsPrintButton.Enabled = False

            'Area of Test Profile Definition
            bsNameTextBox.Enabled = True
            bsNameTextBox.BackColor = Color.White

            bsSampleTypesComboBox.Enabled = False
            bsSampleTypesComboBox.BackColor = SystemColors.MenuBar

            bsTestsTypesComboBox.Enabled = True
            bsTestsTypesComboBox.BackColor = Color.White

            bsTestsSelectionDoubleList.EnableDisableControls = True

            bsSaveButton.Enabled = True
            bsCancelButton.Enabled = True

            'Put Focus in the first enabled field
            bsNameTextBox.Focus()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EditModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EditModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load list of selectable Tests and set the screen status to Edit Mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BA 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage    
    ''' </remarks>
    Private Sub EditTestProfileByButtonClick()
        Try
            editionMode = True
            If (Not bsNameTextBox.Enabled) Then
                'Set screen status to Edit Mode
                EditModeScreenStatus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EditTestProfileByButtonClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EditTestProfileByButtonClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load data of the selected Test Profile and the list of selectable Tests, and set the screen status to Edit Mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BA 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage.
    '''                              Remove '_ForTESTING suffix' from the function name and fix the error name in the call. 
    ''' </remarks>
    Private Sub EditTestProfileByDoubleClick()
        Dim setScreenToEdition As Boolean = False

        Try
            'If Not CurrentUserLevel = "OPERATOR" Then ' TR 23/04/2012 -Validate user level.
            editionMode = True
            If (bsTestProfilesListView.SelectedIndices(0) <> originalSelectedIndex) Then
                If (PendingChangesVerification()) Then
                    If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                        setScreenToEdition = True
                    Else
                        If (originalSelectedIndex <> -1) Then
                            'Return focus to the Test Profile that has been edited
                            bsTestProfilesListView.Items(originalSelectedIndex).Selected = True
                            bsTestProfilesListView.Select()
                        End If
                    End If
                Else
                    setScreenToEdition = True
                End If
            Else
                'If the screen was in Read Only Mode, it is changed to Edit Mode although the User
                'double-clicking in the same Test Profile
                If (Not bsNameTextBox.Enabled) Then setScreenToEdition = True
            End If

            If (setScreenToEdition) Then
                'Load screen fields with all data of the selected Test Profile
                If Not LoadDataOfSelectedTestProfile() Then
                    'Set screen status to Edit Mode
                    EditModeScreenStatus()
                Else
                    'Set screen status to Read Only Mode
                    ReadOnlyModeScreenStatus()
                End If
            End If
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EditTestProfileByDoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EditTestProfileByDoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen when there are not changes pending to save or there are some but the User decides discard them
    ''' </summary>
    Private Sub ExitScreen()
        Dim screenClose As Boolean = False

        Try
            If (editionMode) Then
                If (PendingChangesVerification()) Then
                    If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                        screenClose = True
                    End If
                Else
                    screenClose = True
                End If
            Else
                screenClose = True
            End If

            If screenClose Then
                'TR 11/04/2012 -Disable form on close to avoid any button press.
                Me.Enabled = False
                If Not Tag Is Nothing Then
                    'A PerformClick() method was executed
                    Close()
                Else
                    'Normal button click - Open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method for collecting all multilanguage texts for the DoubleList UserControl
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 07/07/2010
    ''' Modified by: PG 08/10/2010 - Add the LanguageID parameter
    '''              TR 09/03/2011 - Set text of message for Calibrators with factory values to property FactoryValueMessage 
    '''                              of the DoubleList UserControl 
    ''' </remarks>
    Private Sub GetDoubleListLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Titles for listviews included in the DoubleList User Control
            bsTestsSelectionDoubleList.SelectableElementsTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Profiles_SelectableTests", pLanguageID) + ":"
            bsTestsSelectionDoubleList.SelectedElementsTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Profiles_SelectedTests", pLanguageID) + ":"

            'Tooltips for buttons included in the DoubleList User Control
            bsTestsSelectionDoubleList.SelectAllToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Profiles_SelectAll", pLanguageID)
            bsTestsSelectionDoubleList.SelectSomeToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Profiles_SelectSome", pLanguageID)
            bsTestsSelectionDoubleList.UnselectSomeToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Profiles_UnSelectSome", pLanguageID)
            bsTestsSelectionDoubleList.UnselectAllToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Profiles_UnSelectAll", pLanguageID)

            'Inform the factory value property 
            bsTestsSelectionDoubleList.FactoryValueMessage = GetMessageText(GlobalEnumerates.Messages.FACTORY_VALUES.ToString(), pLanguageID)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetDoubleListLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetDoubleListLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application</param>
    ''' <remarks>
    ''' Created by: PG 07/10/10
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels and ToolTips of Graphical Buttons.....
            bsNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", pLanguageID) + ":"
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID) + ":"
            bsTestTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Profiles_TestType", pLanguageID) + ":"
            bsTestProfileDefinitionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Profiles_Definition", pLanguageID)
            bsTestProfileLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Profiles_List", pLanguageID)
            bsTestsSelectionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestsSelection", pLanguageID)

            bsScreenToolTips.SetToolTip(bsNewButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            bsScreenToolTips.SetToolTip(bsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", pLanguageID))
            bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", pLanguageID))
            'bsScreenToolTips.SetToolTip(BsCustomOrderButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", pLanguageID)) 'AG 05/09/2014 - BA-1869

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure and initialize the ListView of Test Profiles 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              AG 16/06/2010 - Multiselection allowed in the ListView (needed for deletion)
    '''              PG 08/10/2010 - Add the LanguageID parameter
    ''' </remarks>
    Private Sub InitializeTestProfilesList(ByVal pLanguageID As String)
        Try
            'Initialization of Test Profiles list
            bsTestProfilesListView.Items.Clear()

            bsTestProfilesListView.Alignment = ListViewAlignment.Left
            bsTestProfilesListView.FullRowSelect = True
            bsTestProfilesListView.MultiSelect = True
            bsTestProfilesListView.Scrollable = True
            bsTestProfilesListView.View = View.Details
            bsTestProfilesListView.HideSelection = False

            'List columns definition  --> only column containing the Test Profile Name will be visible
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            bsTestProfilesListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Profiles_ListName", pLanguageID), -2, HorizontalAlignment.Left)

            bsTestProfilesListView.Columns.Add("TestProfileID", 0, HorizontalAlignment.Left)
            bsTestProfilesListView.Columns.Add("SampleTypeCode", 0, HorizontalAlignment.Left)
            bsTestProfilesListView.Columns.Add("TestProfilePosition", 0, HorizontalAlignment.Left)
            bsTestProfilesListView.Columns.Add("TSUser", 0, HorizontalAlignment.Left)
            bsTestProfilesListView.Columns.Add("TSDateTime", 0, HorizontalAlignment.Left)
            bsTestProfilesListView.Columns.Add("InUse", 0, HorizontalAlignment.Left)
            bsTestProfilesListView.Columns.Add("TestType", 0, HorizontalAlignment.Left)

            'Fill ListView with the list of existing Test Profiles
            LoadTestProfilesList()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeTestProfilesList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeTestProfilesList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure the screen controls to set the INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              DL 13/10/2010 - Filter LoadSelectableTestsList by the selected Test Type
    '''              DL 19/07/2011 - Initialized new DoubleList property EnableDisableControls to False
    ''' </remarks>
    Private Sub InitialModeScreenStatus()
        Try
            'Area of Test Profiles List
            If (bsTestProfilesListView.Items.Count > 0) Then
                bsTestProfilesListView.Enabled = True
                bsTestProfilesListView.SelectedItems.Clear()
            End If

            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = False 'AG 05/09/2014 - BA-1869
            bsPrintButton.Enabled = False

            'Area of Test Profile Definition
            bsNameTextBox.Text = ""
            bsNameTextBox.Enabled = False
            bsNameTextBox.BackColor = SystemColors.MenuBar

            bsSampleTypesComboBox.SelectedIndex = -1
            bsSampleTypesComboBox.SelectedText = ""
            bsSampleTypesComboBox.Enabled = False
            bsSampleTypesComboBox.BackColor = SystemColors.MenuBar

            bsTestsTypesComboBox.SelectedIndex = -1
            bsTestsTypesComboBox.SelectedText = ""
            bsTestsTypesComboBox.Enabled = False
            bsTestsTypesComboBox.BackColor = SystemColors.MenuBar

            'Area of Test Selection
            bsTestsSelectionDoubleList.MultiSelection = True
            bsTestsSelectionDoubleList.EnableDisableControls = False
            bsTestsSelectionDoubleList.InitializeControl = True
            bsTestsSelectionDoubleList.ListViewsIcons = PrepareImageList()

            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False

            'Initialize global variables
            CleanGlobalValues()

            'Focus to button Add
            bsNewButton.Focus()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitialModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitialModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill screen fields with data of the selected Test Profile
    ''' </summary>
    ''' <returns>Boolean value indicating if the selected Test Profile is InUse in the active Work Session or not</returns>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              DL 15/10/2010 - Filter LoadSelectableTestsList by the selected Test Type
    '''              SA 20/10/2010 - Changes due to new implementation of function LoadSelectableTestsList
    '''              SA 05/11/2010 - Removed changes of AG of 04/11/2010
    ''' </remarks>
    Private Function LoadDataOfSelectedTestProfile() As Boolean
        Dim inUse As Boolean = False

        Try
            'If there is not a selected Test Profile, select the first
            If (bsTestProfilesListView.SelectedItems.Count = 0) Then bsTestProfilesListView.Items(0).Selected = True

            'Fill screen controls with data of the selected Test Profile
            selectedTestProfileID = Convert.ToInt32(bsTestProfilesListView.SelectedItems(0).SubItems(1).Text)
            originalTestProfileName = bsTestProfilesListView.SelectedItems(0).SubItems(0).Text
            originalSelectedIndex = bsTestProfilesListView.SelectedIndices(0)

            bsNameTextBox.Text = bsTestProfilesListView.SelectedItems(0).Text
            bsSampleTypesComboBox.SelectedValue = bsTestProfilesListView.SelectedItems(0).SubItems(2).Text

            'Load list of selectable Tests for the selected Test Profile and filter it for the selected TestType
            bsTestsTypesComboBox.SelectedIndex = 0
            LoadSelectableTestsList(bsSampleTypesComboBox.SelectedValue.ToString, selectedTestProfileID)
            bsTestsSelectionDoubleList.TypeToShow = bsTestsTypesComboBox.SelectedValue.ToString

            'Load list of selected Tests for the selected Test Profile
            LoadSelectedTestsList(selectedTestProfileID)

            'Get value of InUse flag
            inUse = CType(bsTestProfilesListView.SelectedItems(0).SubItems(6).Text, Boolean)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadDataOfSelectedTestProfile", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadDataOfSelectedTestProfile", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return inUse
    End Function

    ''' <summary>
    ''' Load the ComboBox of Sample Types with the list of existing ones.
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              VR 29/12/2009 - Change the Constant Value to Enum Value
    ''' </remarks>
    Private Sub LoadSampleTypesList()
        Try
            'Get the list of existing Sample Types
            Dim myGlobalDataTO As New GlobalDataTO
            Dim masterDataConfig As New MasterDataDelegate

            myGlobalDataTO = masterDataConfig.GetList(Nothing, MasterDataEnum.SAMPLE_TYPES.ToString)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim masterDataDS As MasterDataDS = DirectCast(myGlobalDataTO.SetDatos, MasterDataDS)

                If (masterDataDS.tcfgMasterData.Rows.Count > 0) Then
                    'Fill ComboBox of Sample Types
                    bsSampleTypesComboBox.DataSource = masterDataDS.tcfgMasterData
                    bsSampleTypesComboBox.DisplayMember = "ItemIDDesc"
                    bsSampleTypesComboBox.ValueMember = "ItemID"
                End If
            Else
                'Error getting the list of Preloaded SampleTypes
                ShowMessage(Name & ".LoadSampleTypeList", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadSampleTypesList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadSampleTypesList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load list of selectable Tests according the selected Sample Type and Test Profile
    ''' </summary>
    ''' <param name="pSampleType">Code of the selected SampleType (to filter the Tests)</param>
    ''' <param name="pTestProfileID">Optional parameter. Informed when an existing Test Profile is being edited</param>
    ''' <remarks>
    ''' Created by:  SA 20/10/2010 - New implementation: all Tests (of all available TestTypes) are passed to the 
    '''                              DoubleList UserControl, and it is this control who filter which type is shown 
    '''                              according the value selected in the Test Types ComboBox
    ''' Modified by: DL 26/11/2010 - Added case for OffSystem Tests
    '''              TR 09/03/2011 - Insert FactoryCalib as column in the list of Tests and fill it for each Standard Test
    ''' AG 01/09/2014 - BA-1869 Insert Available as column in the list of Tests and fill it for each Test (std, ise, calc, off)
    ''' </remarks>
    Private Sub LoadSelectableTestsList(ByVal pSampleType As String, Optional ByVal pTestProfileID As Integer = 0)
        Try
            'Get the list of selectable Tests
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myTestProfilesDelegate As New TestProfilesDelegate

            myGlobalDataTO = myTestProfilesDelegate.GetTests(Nothing, pTestProfileID, False, pSampleType)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                originalSelectedTests = DirectCast(myGlobalDataTO.SetDatos, TestProfileTestsDS)

                'Create the needed DataSet
                Dim auxTable As New DataTable
                auxTable.Columns.Add("MainPos", System.Type.GetType("System.Int32"))
                auxTable.Columns.Add("Type")
                auxTable.Columns.Add("Code", System.Type.GetType("System.Int32"))
                auxTable.Columns.Add("Description")
                auxTable.Columns.Add("SecondaryPos", System.Type.GetType("System.Int32"))
                auxTable.Columns.Add("Icon")
                auxTable.Columns.Add("FactoryCalib", System.Type.GetType("System.Boolean"))
                auxTable.Columns.Add("Available", System.Type.GetType("System.Boolean")) 'AG 01/09/2014 - BA-1869

                'Load the list of selectable Tests
                Dim auxTableRow As DataRow
                For i As Integer = 0 To originalSelectedTests.tparTestProfileTests.Rows.Count - 1
                    auxTableRow = auxTable.NewRow()
                    Select Case originalSelectedTests.tparTestProfileTests(i).TestType
                        Case "STD"
                            auxTableRow("MainPos") = 1
                        Case "CALC"
                            auxTableRow("MainPos") = 2
                        Case "ISE"
                            auxTableRow("MainPos") = 3
                        Case "OFFS"
                            auxTableRow("MainPos") = 4
                        Case Else
                            auxTableRow("MainPos") = 5
                    End Select

                    auxTableRow("Type") = originalSelectedTests.tparTestProfileTests(i).TestType
                    auxTableRow("Code") = originalSelectedTests.tparTestProfileTests(i).TestID
                    auxTableRow("Description") = originalSelectedTests.tparTestProfileTests(i).TestName
                    auxTableRow("SecondaryPos") = originalSelectedTests.tparTestProfileTests(i).TestPosition
                    auxTableRow("Icon") = originalSelectedTests.tparTestProfileTests(i).IconPath
                    auxTableRow("FactoryCalib") = CBool(originalSelectedTests.tparTestProfileTests(i)("FactoryCalib").ToString())
                    auxTableRow("Available") = CBool(originalSelectedTests.tparTestProfileTests(i)("Available").ToString()) 'AG 01/09/2014 - BA-1869
                    auxTable.Rows.Add(auxTableRow)
                Next

                Dim selectableTests As New DataSet
                selectableTests.Tables.Add(auxTable)

                bsTestsSelectionDoubleList.SelectableElements = selectableTests
            Else
                'Error getting the list of Tests that can be added to a new Profile or to the informed one; shown it
                ShowMessage(Name & ".LoadSelectedTestsList", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadSelectableTestsList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadSelectableTestsList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load list of Tests included in the selected Test Profile
    ''' </summary>
    ''' <param name="pTestProfileID">Identifier of the selected Test Profile</param>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              DL 14/10/2010 - Added column TestType to the DataSet
    '''              DL 26/11/2010 - Added case for OffSystem Tests
    '''              TR 09/03/2011 - Insert FactoryCalib as column in the list of Tests and fill it for each Standard Test
    ''' AG 01/09/2014 - BA-1869 Insert Available as column in the list of Tests and fill it for each TEST (std, calc, ise, off)
    ''' </remarks>
    Private Sub LoadSelectedTestsList(ByVal pTestProfileID As Integer)
        Try
            'Get the list of selected Tests
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myTestProfilesDelegate As New TestProfilesDelegate

            myGlobalDataTO = myTestProfilesDelegate.GetTests(Nothing, pTestProfileID, True)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                originalSelectedTests = DirectCast(myGlobalDataTO.SetDatos, TestProfileTestsDS)

                'Create the needed DataSet
                Dim auxTable As New DataTable
                auxTable.Columns.Add("MainPos", System.Type.GetType("System.Int32"))
                auxTable.Columns.Add("Type")
                auxTable.Columns.Add("Code", System.Type.GetType("System.Int32"))
                auxTable.Columns.Add("Description")
                auxTable.Columns.Add("SecondaryPos", System.Type.GetType("System.Int32"))
                auxTable.Columns.Add("Icon")
                auxTable.Columns.Add("FactoryCalib", System.Type.GetType("System.Boolean"))
                auxTable.Columns.Add("Available", System.Type.GetType("System.Boolean")) 'AG 01/09/2014 - BA-1869

                'Load the list of selected Tests
                Dim auxTableRow As DataRow
                For i As Integer = 0 To originalSelectedTests.tparTestProfileTests.Rows.Count - 1
                    auxTableRow = auxTable.NewRow()
                    Select Case originalSelectedTests.tparTestProfileTests(i).TestType
                        Case "STD"
                            auxTableRow("MainPos") = 1
                        Case "CALC"
                            auxTableRow("MainPos") = 2
                        Case "ISE"
                            auxTableRow("MainPos") = 3
                        Case "OFFS"
                            auxTableRow("MainPos") = 4
                    End Select

                    auxTableRow("Type") = originalSelectedTests.tparTestProfileTests(i).TestType
                    auxTableRow("Code") = originalSelectedTests.tparTestProfileTests(i).TestID
                    auxTableRow("Description") = originalSelectedTests.tparTestProfileTests(i).TestName
                    auxTableRow("SecondaryPos") = originalSelectedTests.tparTestProfileTests(i).TestPosition
                    auxTableRow("Icon") = originalSelectedTests.tparTestProfileTests(i).IconPath
                    auxTableRow("FactoryCalib") = CBool(originalSelectedTests.tparTestProfileTests(i)("FactoryCalib").ToString())
                    auxTableRow("Available") = CBool(originalSelectedTests.tparTestProfileTests(i)("Available").ToString()) 'AG 01/09/2014 - BA-1869
                    auxTable.Rows.Add(auxTableRow)
                Next

                Dim selectedTests As New DataSet
                selectedTests.Tables.Add(auxTable)
                bsTestsSelectionDoubleList.SelectedElements = selectedTests
            Else
                'Error getting the list of Tests included in the selected Profile; shown it
                ShowMessage(Name & ".LoadSelectedTestsList", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadSelectedTestsList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadSelectedTestsList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill Test Profile basic data in the correspondent structure
    ''' </summary>
    ''' <returns>A dataset with structure of table tparTestProfiles</returns>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Function LoadTestProfileDS() As TestProfilesDS
        Dim testProfileData As New TestProfilesDS
        Try
            Dim testProfileRow As TestProfilesDS.tparTestProfilesRow
            testProfileRow = testProfileData.tparTestProfiles.NewtparTestProfilesRow

            testProfileRow.TestProfileID = selectedTestProfileID
            testProfileRow.TestProfileName = bsNameTextBox.Text.Trim
            testProfileRow.SampleType = bsSampleTypesComboBox.SelectedValue.ToString

            If (selectedTestProfileID = 0) Then
                'When adding a new Test Profile, gets from the Session the Username of the connected User
                'and the current datetime
                Dim currentSession As New GlobalBase()

                testProfileRow.SetTestProfilePositionNull() 'Set to null, to get the value from the database.
                testProfileRow.TS_User = currentSession.GetSessionInfo().UserName
                testProfileRow.TS_DateTime = Now
            Else
                'When editing or deleting an existing Test Profile, to concurrence control, the informed Username and Datetime
                'to send will be the same the Test Profile had when it was read from the database
                testProfileRow.TestProfilePosition = Convert.ToInt32(bsTestProfilesListView.SelectedItems(0).SubItems(3).Text)
                testProfileRow.TS_User = bsTestProfilesListView.SelectedItems(0).SubItems(4).Text.ToString
                testProfileRow.TS_DateTime = CDate(bsTestProfilesListView.SelectedItems(0).SubItems(5).Text)
            End If

            testProfileData.tparTestProfiles.Rows.Add(testProfileRow)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadTestProfileDS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadTestProfileDS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return testProfileData
    End Function

    ''' <summary>
    ''' Load the ListView of Test Profiles with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              SA 27/06/2010 - Changed the way of getting the needed icons
    '''              SG 07/07/2010 - Changed the way of updating the item selected; sort Profiles by Position before load the ListView
    ''' </remarks>
    Private Sub LoadTestProfilesList()
        Try
            'Get the list of existing Test Profiles
            Dim resultData As New GlobalDataTO
            Dim myTestProfilesDelegate As New TestProfilesDelegate

            resultData = myTestProfilesDelegate.GetListByPosition(Nothing)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myTestProfilesDS As TestProfilesDS = DirectCast(resultData.SetDatos, TestProfilesDS)

                If (myTestProfilesDS.tparTestProfiles.Rows.Count > 0) Then
                    Dim testProfIcon As New ImageList
                    Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

                    'Get the Icon for not In Use Test Profiles
                    resultData = myPreloadedMasterDataDelegate.GetSubTableItem(Nothing, PreloadedMasterDataEnum.ICON_PATHS, "TPROFILES")
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myPreMasterDataDS As PreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count = 1) Then
                            If (IO.File.Exists(IconsPath & myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc)) Then
                                testProfIcon.Images.Add(myPreMasterDataDS.tfmwPreloadedMasterData(0).ItemID, _
                                                        Image.FromFile(IconsPath & myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc))
                            End If
                        End If
                    End If

                    'Get the Icon for In Use Test Profiles
                    resultData = myPreloadedMasterDataDelegate.GetSubTableItem(Nothing, PreloadedMasterDataEnum.ICON_PATHS, "INUSETPROF")
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myPreMasterDataDS As PreloadedMasterDataDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                        If (myPreMasterDataDS.tfmwPreloadedMasterData.Rows.Count = 1) Then
                            If (IO.File.Exists(IconsPath & myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc)) Then
                                testProfIcon.Images.Add(myPreMasterDataDS.tfmwPreloadedMasterData(0).ItemID, _
                                                        Image.FromFile(IconsPath & myPreMasterDataDS.tfmwPreloadedMasterData(0).FixedItemDesc))
                            End If
                        End If
                    End If

                    bsTestProfilesListView.Items.Clear()
                    bsTestProfilesListView.Enabled = True
                    bsTestProfilesListView.SmallImageList = testProfIcon

                    'Sort Profiles by Position before load the ListView
                    Dim qTestProfilesRowsByPosition As List(Of TestProfilesDS.tparTestProfilesRow) = (From a In myTestProfilesDS.tparTestProfiles _
                                                                                                    Select a Order By a.TestProfilePosition).ToList()

                    For Each profilesRow As TestProfilesDS.tparTestProfilesRow In qTestProfilesRowsByPosition
                        bsTestProfilesListView.Items.Add(profilesRow.TestProfileID.ToString, _
                                                         profilesRow.TestProfileName.ToString, _
                                                         profilesRow.IconPath.ToString).Tag = profilesRow.InUse

                        Dim last As Integer = bsTestProfilesListView.Items.Count - 1

                        bsTestProfilesListView.Items(last).SubItems.Add(profilesRow.TestProfileID.ToString)
                        bsTestProfilesListView.Items(last).SubItems.Add(profilesRow.SampleType.ToString)
                        bsTestProfilesListView.Items(last).SubItems.Add(profilesRow.TestProfilePosition.ToString)
                        bsTestProfilesListView.Items(last).SubItems.Add(profilesRow.TS_User.ToString)
                        bsTestProfilesListView.Items(last).SubItems.Add(profilesRow.TS_DateTime.ToString)
                        bsTestProfilesListView.Items(last).SubItems.Add(profilesRow.InUse.ToString)

                        If (selectedTestProfileID <> 0) Then
                            'The selected Profile can be in a new position in the list: reselect it
                            If (Convert.ToInt32(bsTestProfilesListView.Items(last).SubItems(1).Text) = selectedTestProfileID) Then
                                originalSelectedIndex = bsTestProfilesListView.Items(last).Index
                            End If
                        End If
                    Next

                    If (bsTestProfilesListView.Items.Count > 0) Then
                        If (originalSelectedIndex = -1) Then originalSelectedIndex = bsTestProfilesListView.Items(0).Index
                        If (bsTestProfilesListView.SelectedIndices.Count > 0) Then
                            bsTestProfilesListView.Items(bsTestProfilesListView.SelectedIndices(0)).Selected = False
                        End If
                        bsTestProfilesListView.Items(originalSelectedIndex).Selected = True
                        bsTestProfilesListView.Items(originalSelectedIndex).Focused = True
                        bsTestProfilesListView.Select()
                    Else
                        'There was not a selected Test Profile or the selected one is not in the list; the global variables containing 
                        'information of the selected Test Profile is initializated
                        CleanGlobalValues()
                    End If
                Else
                    CleanGlobalValues()

                    bsTestProfilesListView.Items.Clear()
                    bsTestProfilesListView.Enabled = False
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadTestProfilesList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadTestProfilesList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBox of Test Types with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 13/10/2010
    ''' </remarks>
    Private Sub LoadTestTypesList()
        Try
            'Get the list of existing Tests Types
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.TEST_TYPES)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                If (myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                    'Fill ComboBox of Tests Types
                    bsTestsTypesComboBox.DataSource = myPreloadedMasterDataDS.tfmwPreloadedMasterData
                    bsTestsTypesComboBox.DisplayMember = "FixedItemDesc"
                    bsTestsTypesComboBox.ValueMember = "ItemID"
                End If
            Else
                'Error getting the list of Preloaded SampleTypes
                ShowMessage(Name & ".LoadTestTypesList", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadTestTypesList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadTestTypesList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill Test Profile basic data in the correspondent structure
    ''' </summary>
    ''' <returns>A dataset with structure of table tparTestProfiles</returns>
    ''' <remarks>
    ''' </remarks>
    Private Function MultiLoadTestProfileDS() As TestProfilesDS
        Dim testProfileData As New TestProfilesDS

        Try
            For Each mySelectedItem As ListViewItem In bsTestProfilesListView.SelectedItems
                Dim testProfileRow As TestProfilesDS.tparTestProfilesRow
                testProfileRow = testProfileData.tparTestProfiles.NewtparTestProfilesRow

                testProfileRow.TestProfileID = CInt(mySelectedItem.Name)
                testProfileData.tparTestProfiles.Rows.Add(testProfileRow)
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MultiLoadTestProfileDS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MultiLoadTestProfileDS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return testProfileData
    End Function

    ''' <summary>
    ''' Configure the screen controls when there are several Test Profiles selected in the list
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 27/06/2010 - Added Try/Catch
    '''              DL 13/10/2010 - Select the first TestType in the list as default value, and filter LoadSelectableTestsList 
    '''                              by the selected Test Type
    '''              SA 20/10/2010 - Changes due to new implementation of function LoadSelectableTestsList
    '''              SA 24/11/2010 - Set fields BackColor to MenuBar instead of to Gainsboro
    '''              DL 19/07/2011 - Initialized new DoubleList property EnableDisableControls to False
    ''' </remarks>
    Private Sub MultiSelect()
        Try
            'Area of Test Profiles List
            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
            bsPrintButton.Enabled = True

            'Area of Test Profile Definition
            bsNameTextBox.Text = ""
            bsNameTextBox.Enabled = False
            bsNameTextBox.BackColor = SystemColors.MenuBar

            'The first Sample Type in the list is  selected as default
            bsSampleTypesComboBox.SelectedIndex = 0
            bsSampleTypesComboBox.Enabled = False
            bsSampleTypesComboBox.BackColor = SystemColors.MenuBar

            'The first Test Type in the list is  selected as default
            bsTestsTypesComboBox.SelectedIndex = 0
            bsTestsTypesComboBox.Enabled = False
            bsTestsTypesComboBox.BackColor = SystemColors.MenuBar

            'Initialize global variables
            CleanGlobalValues()

            'The list of Standard Tests for Serum is shown in the Selectable Elements ListView
            bsTestsSelectionDoubleList.InitializeControl = True
            LoadSelectableTestsList(bsSampleTypesComboBox.SelectedValue.ToString)
            bsTestsSelectionDoubleList.TypeToShow = bsTestsTypesComboBox.SelectedValue.ToString
            bsTestsSelectionDoubleList.EnableDisableControls = False

            'Disable Save and Cancel Buttons
            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False

            'Put Focus in the first enabled field
            bsNameTextBox.Focus()

            'Initialize global variables
            CleanGlobalValues()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MultiSelect", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MultiSelect", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if there is at least one User's change pending to save
    ''' </summary>
    ''' <returns>True if there are changes pending to save: otherwise, False</returns>
    ''' <remarks>
    ''' Modified by: BA 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage   
    '''              SG 08/07/2010 - Checks if there is any test added in order to determine pending changes 
    '''              SA 05/11/2010 - Changed verification of pending changes when the list of Selected Tests
    '''                              has the same number of elements than the list of Original Selected Tests
    ''' </remarks>
    Public Function PendingChangesVerification() As Boolean
        Dim pendingToSaveChanges As Boolean = False

        Try
            If (bsNameTextBox.Enabled) Then
                If (selectedTestProfileID = 0) Then
                    'In Add Mode, there are always changes pending to save if the name was informed 
                    'and/or some Tests were selected
                    pendingToSaveChanges = (bsNameTextBox.Text.Trim <> "") OrElse _
                                           (bsTestsSelectionDoubleList.SelectedElements.Tables(0).Rows.Count > 0)
                Else
                    If (originalSelectedTests.tparTestProfileTests.Rows.Count > 0) Then
                        'In Edit Mode, loading values are compared against current values
                        If (bsNameTextBox.Text.Trim <> originalTestProfileName) Then
                            pendingToSaveChanges = True
                        Else
                            'Get the list of selected elements and compare it againts the original list
                            Dim auxTestsList As DataSet = bsTestsSelectionDoubleList.SelectedElements
                            If (auxTestsList.Tables.Count > 0) Then
                                'If the number of Tests is different, the list has been changed
                                If (auxTestsList.Tables(0).Rows.Count <> originalSelectedTests.tparTestProfileTests.Rows.Count) Then
                                    pendingToSaveChanges = True
                                Else
                                    'If both lists have the same number of Tests, they are compared one by one
                                    For Each selectedTest As DataRow In auxTestsList.Tables(0).Rows
                                        Dim testType As String = selectedTest.Item("Type").ToString
                                        Dim testID As Integer = Convert.ToInt32(selectedTest.Item("Code"))

                                        'Search if the selected TestType/TestID was in the list of original selected Tests
                                        Dim lstOriginalTest As List(Of TestProfileTestsDS.tparTestProfileTestsRow)
                                        lstOriginalTest = (From a As TestProfileTestsDS.tparTestProfileTestsRow In originalSelectedTests.tparTestProfileTests _
                                                          Where a.TestType = testType _
                                                        AndAlso a.TestID = testID _
                                                         Select a).ToList

                                        If (lstOriginalTest.Count = 0) Then
                                            pendingToSaveChanges = True
                                            Exit For
                                        End If
                                    Next
                                End If
                            Else
                                pendingToSaveChanges = True
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PendingChangesVerification", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PendingChangesVerification", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return pendingToSaveChanges
    End Function

    ''' <summary>
    ''' Method incharge to load the button images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/04/2010
    ''' Modified by: DL 03/11/2010 - Set value of Image Property instead of BackgroundImage Property
    '''              DL 12/11/2010 - Get Icons for movement buttons in the UserControl BSDoubleList
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'ADD Button
            auxIconName = GetIconName("ADD")
            If (auxIconName <> "") Then
                bsNewButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'EDIT Button
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                bsEditButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'DELETE Button
            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                bsDeleteButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CUSTOMSORT Button 'AG 05/09/2014 - BA-1869
            auxIconName = GetIconName("ADJUSTMENT")
            If (auxIconName <> "") Then
                BsCustomOrderButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'SAVE Button
            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                bsSaveButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                bsCancelButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'BSDoubleList Buttons
            auxIconName = GetIconName("FORWARDL")
            If (auxIconName <> "") Then
                bsTestsSelectionDoubleList.SelectAllButtonImage = Image.FromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("BACKWARDL")
            If (auxIconName <> "") Then
                bsTestsSelectionDoubleList.UnselectAllButtonImage = Image.FromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("RIGHT")
            If (auxIconName <> "") Then
                bsTestsSelectionDoubleList.SelectChosenButtonImage = Image.FromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("LEFT")
            If (auxIconName <> "") Then
                bsTestsSelectionDoubleList.UnselectChosenButtonImage = Image.FromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get icons for all Test Types that can be included in a Test Profile and load them in an ImageList
    ''' </summary>
    ''' <returns>ImageList containing the icons for the different Test Types</returns>
    ''' <remarks>
    ''' Created by:  SA 20/10/2010
    ''' Modified by: DL 26/11/2010 - Get Icon for OffSystem Tests and add it to the ImageList
    '''              SA 02/12/2010 - Name of Icon for OffSystem Tests was bad written
    ''' </remarks>
    Private Function PrepareImageList() As ImageList
        Dim myTestTypeImageList As New ImageList
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            auxIconName = GetIconName("TESTICON")
            If (auxIconName <> "") Then
                myTestTypeImageList.Images.Add(iconPath & auxIconName, Image.FromFile(iconPath & auxIconName))
            End If

            auxIconName = GetIconName("USERTEST")
            If (auxIconName <> "") Then
                myTestTypeImageList.Images.Add(iconPath & auxIconName, Image.FromFile(iconPath & auxIconName))
            End If

            auxIconName = GetIconName("TCALC")
            If (auxIconName <> "") Then
                myTestTypeImageList.Images.Add(iconPath & auxIconName, Image.FromFile(iconPath & auxIconName))
            End If

            auxIconName = GetIconName("TISE_SYS")
            If (auxIconName <> "") Then
                myTestTypeImageList.Images.Add(iconPath & auxIconName, Image.FromFile(iconPath & auxIconName))
            End If

            auxIconName = GetIconName("TOFF_SYS")
            If (auxIconName <> "") Then
                myTestTypeImageList.Images.Add(iconPath & auxIconName, Image.FromFile(iconPath & auxIconName))
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareImageList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareImageList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myTestTypeImageList
    End Function

    ''' <summary>
    ''' Configure the screen controls to set the READ-ONLY MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              DL 13/10/2010 - Added ComboBox of Test Types
    '''              DL 19/07/2011 - Initialized new DoubleList property EnableDisableControls to False
    ''' </remarks>
    Private Sub QueryModeScreenStatus()
        Try
            'Area of Test Profiles List
            bsNewButton.Enabled = True
            bsEditButton.Enabled = True
            bsDeleteButton.Enabled = True
            BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
            bsPrintButton.Enabled = True

            'Area of Test Profile Definition
            bsNameTextBox.Enabled = False
            bsNameTextBox.BackColor = SystemColors.MenuBar

            bsSampleTypesComboBox.Enabled = False
            bsSampleTypesComboBox.BackColor = SystemColors.MenuBar

            bsTestsTypesComboBox.SelectedIndex = 0
            bsTestsTypesComboBox.Enabled = False
            bsTestsTypesComboBox.BackColor = SystemColors.MenuBar

            bsTestsSelectionDoubleList.EnableDisableControls = False

            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".QueryModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QueryModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get data of the selected Test Profile, fill the correspondent variables and controls and
    ''' set the screen status to Read-only Mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BA 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage 
    '''                            - Remove '_ForTESTING suffix' from the function name and fix the error name in the call. 
    ''' </remarks>
    Private Sub QueryTestProfile()
        Dim setScreenToQuery As Boolean = False

        Try
            If (bsTestProfilesListView.SelectedIndices(0) <> originalSelectedIndex) Then
                If (PendingChangesVerification()) Then
                    If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                        setScreenToQuery = True
                    Else
                        If (originalSelectedIndex <> -1) Then
                            bsTestProfilesListView.SelectedItems.Clear() 'AG 04/11/2010

                            'Return focus to the Test Profile that has been edited
                            bsTestProfilesListView.Items(originalSelectedIndex).Selected = True
                            bsTestProfilesListView.Select()
                        End If
                    End If
                Else
                    setScreenToQuery = True
                End If

                If (setScreenToQuery) Then
                    'Load screen fields with all data of the selected Test Profile
                    If (bsTestsTypesComboBox.SelectedIndex = -1) Then bsTestsTypesComboBox.SelectedIndex = 0
                    If (Not LoadDataOfSelectedTestProfile()) Then
                        'Set screen status to Query Mode
                        QueryModeScreenStatus()
                    Else
                        'Set screen status to Read Only Mode
                        ReadOnlyModeScreenStatus()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".QueryTestProfile", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QueryTestProfile", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen display in query mode when the KeyUp or KeyDown key is pressed
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 24/11/2010 - Included also PageDown and PageUp keys 
    ''' </remarks>
    Private Sub QueryTestProfilesByMoveUpDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        Try
            Select Case e.KeyCode
                Case Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp
                    QueryTestProfile()
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".QueryTestProfilesByMoveUpDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QueryTestProfilesByMoveUpDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure the screen controls to set the READ-ONLY MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by: GDS 18/05/2010
    '''             DL  13/10/2010 - Filter LoadSelectableTestsList by the selected Test Type
    '''             DL  19/07/2011 - Initialized new DoubleList property EnableDisableControls to False
    ''' </remarks>
    Private Sub ReadOnlyModeScreenStatus()
        Try
            'Area of Test Profiles List
            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
            bsPrintButton.Enabled = True

            'Area of Test Profile Definition
            bsNameTextBox.Enabled = False
            bsNameTextBox.BackColor = SystemColors.MenuBar

            bsSampleTypesComboBox.Enabled = False
            bsSampleTypesComboBox.BackColor = SystemColors.MenuBar

            bsTestsTypesComboBox.Enabled = False
            bsTestsTypesComboBox.BackColor = SystemColors.MenuBar

            bsTestsSelectionDoubleList.EnableDisableControls = False

            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ReadOnlyModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ReadOnlyModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save data of the added or updated Test Profile
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              SG 07/07/2010 - Added ErrorProvider functionality
    '''              AG 02/09/2014 - BA1869 get also the Available column in order to calculate the profile availability
    ''' </remarks>
    Private Sub SaveChanges()
        Try
            Cursor = Cursors.WaitCursor

            validationError = False
            ValidateSavingConditions()

            If (Not validationError) Then
                'Fill Test Profile basic data
                Dim testProfileData As TestProfilesDS = LoadTestProfileDS()

                'Fill the list of selected Tests to include in the Test Profile
                Dim auxTestsList As DataSet = bsTestsSelectionDoubleList.SelectedElements

                If (Not IsNothing(auxTestsList)) Then
                    Dim selectedTestsList As New TestProfileTestsDS
                    Dim testProfileTestRow As TestProfileTestsDS.tparTestProfileTestsRow

                    For i As Integer = 0 To auxTestsList.Tables(0).Rows.Count - 1
                        testProfileTestRow = selectedTestsList.tparTestProfileTests.NewtparTestProfileTestsRow
                        testProfileTestRow.TestProfileID = selectedTestProfileID
                        testProfileTestRow.TestID = CInt(auxTestsList.Tables(0).Rows(i).Item("Code"))
                        testProfileTestRow.TestType = auxTestsList.Tables(0).Rows(i).Item("Type").ToString

                        'AG 02/09/2014 - BA-1869 - If some component not available then the profile is also not available
                        If Not testProfileData Is Nothing AndAlso testProfileData.tparTestProfiles.Rows.Count > 0 Then
                            If Not auxTestsList.Tables(0).Rows(i).Item("Available") Is DBNull.Value Then
                                If testProfileData.tparTestProfiles(0).IsAvailableNull Then
                                    testProfileData.tparTestProfiles(0).Available = CBool(auxTestsList.Tables(0).Rows(i).Item("Available")) 'Initialize value
                                ElseIf Not CBool(auxTestsList.Tables(0).Rows(i).Item("Available")) AndAlso testProfileData.tparTestProfiles(0).Available Then
                                    testProfileData.tparTestProfiles(0).Available = False
                                End If

                            Else
                                'Do nothing: Add method will use the default 1 value / Modify method wont modify Available column
                            End If
                        End If
                        'AG 02/09/2014 - BA-1869

                        selectedTestsList.tparTestProfileTests.Rows.Add(testProfileTestRow)
                    Next i

                    Dim returnedData As GlobalDataTO
                    Dim testProfileToSave As New TestProfilesDelegate
                    If (selectedTestProfileID = 0) Then
                        'Insert a new Test Profile
                        Dim myTestProfilesDelegate As New TestProfilesDelegate
                        returnedData = myTestProfilesDelegate.Add(Nothing, testProfileData, selectedTestsList)
                    Else
                        'Modify values of an existing Test Profile
                        returnedData = testProfileToSave.Modify(Nothing, testProfileData, selectedTestsList)
                    End If

                    If (Not returnedData.HasError) Then
                        If (selectedTestProfileID = 0) Then
                            'Show data of the added Test Profile in Read Only Mode
                            selectedTestProfileID = CInt(CType(returnedData.SetDatos, TestProfilesDS).tparTestProfiles(0).TestProfileID)

                            'Refresh the Test Profiles List 
                            LoadTestProfilesList()

                            'Show the added Test Profile in Read Only mode
                            LoadDataOfSelectedTestProfile()
                            QueryModeScreenStatus()
                        Else
                            'Refresh the Test Profiles List and the details area 
                            LoadTestProfilesList()
                            LoadDataOfSelectedTestProfile()
                            QueryModeScreenStatus()
                        End If
                    Else
                        'Error adding or updating the Test Profile; show error message
                        bsScreenErrorProvider.SetError(bsNameTextBox, GetMessageText(GlobalEnumerates.Messages.DUPLICATED_TEST_PROFILE_NAME.ToString)) 'SG 07/07/2010
                    End If
                End If
            End If

            Cursor = Cursors.Default
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization. For event TestProfilesManagement_Load
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 19/10/2010 - Code moved from the event
    ''' Modified by: PG 07/07/2010 - Load the multilanguage texts for all Screen Labels 
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the list of available Sample Types and Test Types
            LoadSampleTypesList()
            LoadTestTypesList()

            'Load the list of existing Test Profiles
            InitialModeScreenStatus()
            InitializeTestProfilesList(currentLanguage)

            'Load the multilanguage texts for all Screen Labels 
            GetScreenLabels(currentLanguage)
            GetDoubleListLabels(currentLanguage)

            'Load all icons used in graphical buttons
            PrepareButtons()

            If (bsTestProfilesListView.Items.Count > 0) Then
                'Select the first Test Profile in the list and show it in Query Mode or in ReadOnly Mode if it is a InUse Profile
                bsTestsTypesComboBox.SelectedIndex = 0
                If (Not LoadDataOfSelectedTestProfile()) Then
                    'Set screen status to Query Mode
                    QueryModeScreenStatus()
                Else
                    'Set screen status to Read Only Mode
                    ReadOnlyModeScreenStatus()
                End If
            End If
            ResetBorder()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Updates in DataBase the positions of the test profiles according to the sorting of the listview
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 07/07/2010
    ''' </remarks>
    Private Sub UpdateTestProfilesPositions()
        Try
            'Take all the items in the ListView and copy them to a local DataSet
            Using testProfileData As New TestProfilesDS()
                For Each item As ListViewItem In bsTestProfilesListView.Items
                    Dim testProfileRow As TestProfilesDS.tparTestProfilesRow

                    testProfileRow = testProfileData.tparTestProfiles.NewtparTestProfilesRow
                    testProfileRow.TestProfileID = CInt(item.SubItems(1).Text)
                    testProfileRow.TestProfileName = bsNameTextBox.Text.Trim
                    testProfileRow.SampleType = bsSampleTypesComboBox.SelectedValue.ToString
                    testProfileRow.TestProfilePosition = item.Index + 1
                    testProfileRow.TS_User = item.SubItems(4).Text
                    testProfileRow.TS_DateTime = CDate(item.SubItems(5).Text)
                    testProfileData.tparTestProfiles.Rows.Add(testProfileRow)
                Next

                'Update field TestProfilePosition for all Profiles according their current position in the ListView
                If (testProfileData.tparTestProfiles.Rows.Count > 1) Then
                    Dim myGlobalDataTO As New GlobalDataTO()
                    Dim myTestProfilesDelegate As New TestProfilesDelegate()

                    myGlobalDataTO = myTestProfilesDelegate.UpdateTestProfilePosition(Nothing, testProfileData)
                    If (myGlobalDataTO.HasError) Then
                        'Error updating field TestProfilePosition; shown it
                        ShowMessage(Name & ".UpdateTestProfilesPositions", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                End If
            End Using
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateTestProfilesPositions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpdateTestProfilesPositions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate that all required fields have been informed
    ''' </summary>
    ''' <returns>True if all required fields have a value; otherwise, False</returns>
    ''' <remarks>
    ''' Modified by: SG 07/07/2010 - Add ErrorProvider functionality
    ''' </remarks>
    Private Function ValidateSavingConditions() As Boolean
        Dim fieldsOK As Boolean = False
        Try
            bsScreenErrorProvider.Clear()
            If (bsNameTextBox.TextLength = 0) And (bsTestsSelectionDoubleList.SelectedElements.Tables(0).Rows.Count = 0) Then
                validationError = True
                bsScreenErrorProvider.SetError(bsNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                bsScreenErrorProvider.SetError(bsTestsSelectionDoubleList, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            ElseIf (bsNameTextBox.TextLength = 0) Then
                validationError = True
                bsScreenErrorProvider.SetError(bsNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            ElseIf (bsTestsSelectionDoubleList.SelectedElements.Tables(0).Rows.Count = 0) Then
                validationError = True
                bsScreenErrorProvider.SetError(bsTestsSelectionDoubleList, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            Else
                fieldsOK = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateSavingConditions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateSavingConditions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return fieldsOK
    End Function

    ''' <summary>
    ''' Enable or disable functionallity by user level.
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: TR 23/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    Exit Select
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region


#Region "Events"
    '*************************'
    '* EVENTS FOR THE SCREEN *'
    '*************************'
    ''' <summary>
    ''' When the screen is in ADD or EDIT Mode and the ESC key is pressed, the code for Cancel Button Click is executed;
    ''' in other case, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 05/11/2010 
    ''' Modified by: RH 04/07/2011 - Escape key should do exactly the same operations as bsCancelButton_Click()
    ''' </remarks>
    Private Sub ProgTestProfiles_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (bsCancelButton.Enabled) Then
                    bsCancelButton.PerformClick()
                Else
                    bsExitButton.PerformClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ProgTestProfiles_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProgTestProfiles_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Form initialization when loading
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BA 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              PG 08/10/2010 - Get the current language     
    '''              SA 19/10/2010 - Code moved to ScreenLoad method
    ''' </remarks>
    Private Sub TestProfilesManagement_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'TR 20/04/2012 get the current user level
            Dim MyGlobalBase As New GlobalBase
            CurrentUserLevel = MyGlobalBase.GetSessionInfo().UserLevel
            'TR 20/04/2012 -END.
            ScreenLoad()

            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".TestProfilesManagement_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".TestProfilesManagement_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '****************************************'
    '* EVENTS FOR LISTVIEW OF TEST PROFILES *'
    '****************************************'
    ''' <summary>
    ''' Show data of the selected Test Profile in Read-Only Mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: AG 16/06/2010 - Add code for multiselection (copied from ProgCalculatedTest screen)
    ''' </remarks>
    Private Sub bsTestProfilesListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsTestProfilesListView.Click
        Try
            If (bsTestProfilesListView.SelectedItems.Count = 1) Then
                QueryTestProfile()
            Else
                Dim bEnabled As Boolean = True
                For Each mySelectedItem As ListViewItem In bsTestProfilesListView.SelectedItems
                    'If several Profiles have been selected but at least one of them is InUse, then EDIT and DELETE actions are disabled 
                    If (Convert.ToBoolean(mySelectedItem.SubItems(6).Text)) Then
                        bEnabled = False
                        Exit For
                    End If
                Next mySelectedItem
                MultiSelect()
                bsEditButton.Enabled = False
                BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
                bsDeleteButton.Enabled = bEnabled
            End If

            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestProfilesListView_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestProfilesListView_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Allow re-sort the list of Test Profiles ascending/descending when clicking in the column header
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 07/07/2010
    ''' Modified by: SA 19/10/2010 - Added the Try/Catch
    ''' </remarks>
    Private Sub bsTestProfilesListView_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles bsTestProfilesListView.ColumnClick
        Try
            Select Case bsTestProfilesListView.Sorting
                Case SortOrder.None
                    bsTestProfilesListView.Sorting = SortOrder.Descending
                    Exit Select
                Case SortOrder.Ascending
                    bsTestProfilesListView.Sorting = SortOrder.Descending
                    Exit Select
                Case SortOrder.Descending
                    bsTestProfilesListView.Sorting = SortOrder.Ascending
                    Exit Select
            End Select
            bsTestProfilesListView.Sort()
            UpdateTestProfilesPositions()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestProfilesListView_ColumnClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestProfilesListView_ColumnClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Don not allow Users resizing of the visible ListView columm
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsTestProfilesListView_ColumnWidthChanging(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles bsTestProfilesListView.ColumnWidthChanging
        e.Cancel = True
        If (e.ColumnIndex = 0) Then
            e.NewWidth = 210
        Else
            e.NewWidth = 0
        End If
    End Sub

    ''' <summary>
    ''' Load data of the selected Test Profile and set the screen status to Edit Mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BA 24/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage    
    ''' </remarks>
    Private Sub bsTestProfilesListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsTestProfilesListView.DoubleClick
        Try
            EditTestProfileByDoubleClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestProfilesListView_DoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestProfilesListView_DoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Allow consultation of elements in Test Profiles list using the keyboard, and also deletion using SUPR key
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 28/06/2010
    ''' </remarks>
    Private Sub bsTestProfilesListView_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsTestProfilesListView.KeyUp
        Try
            If (bsTestProfilesListView.SelectedItems.Count > 0) Then
                If (bsTestProfilesListView.SelectedItems.Count = 1) Then
                    QueryTestProfilesByMoveUpDown(e)
                Else
                    Dim bEnabled As Boolean = True
                    For Each mySelectedItem As ListViewItem In bsTestProfilesListView.SelectedItems
                        'If several Profiles have been selected but at least one of them is InUse, then EDIT and DELETE actions are disabled 
                        If (Convert.ToBoolean(mySelectedItem.SubItems(6).Text)) Then
                            bEnabled = False
                            Exit For
                        End If
                    Next

                    'Configure details area for the case of multiple Profiles selected
                    MultiSelect()

                    bsEditButton.Enabled = False
                    BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
                    bsDeleteButton.Enabled = bEnabled
                End If
            End If
            ScreenStatusByUserLevel() 'TR 23/04/2012
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestProfilesListView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestProfilesListView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Allow deletion of Test Profiles using the SUPR key
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsTestProfilesListView_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsTestProfilesListView.PreviewKeyDown
        Try
            If (bsTestProfilesListView.SelectedItems.Count > 0) Then
                If (e.KeyCode = Keys.Delete And bsDeleteButton.Enabled) Then
                    DeleteTestProfile()
                ElseIf (e.KeyCode = Keys.Enter And bsEditButton.Enabled) Then
                    If (bsTestProfilesListView.SelectedItems.Count = 1) Then EditTestProfileByButtonClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestProfilesListView_PreviewKeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestProfilesListView_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*************************************'
    '* EVENTS FOR FIELDS IN DETAILS AREA *'
    '*************************************'
    ''' <summary>
    ''' Load list of Selectable Tests when a new Sample Type is chosen.
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 13/10/2010 - Filter LoadSelectableTestsList by the selected Test Type
    '''              SA 20/10/2010 - Changes due to new implementation of function LoadSelectableTestsList
    ''' </remarks>
    Private Sub bsSampleTypesComboBox_SelectionChangeCommitted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSampleTypesComboBox.SelectionChangeCommitted
        Try
            If (bsSampleTypesComboBox.SelectedIndex > -1) Then
                bsTestsSelectionDoubleList.InitializeControl = True

                LoadSelectableTestsList(bsSampleTypesComboBox.SelectedValue.ToString)
                bsTestsSelectionDoubleList.TypeToShow = bsTestsTypesComboBox.SelectedValue.ToString
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSampleTypesComboBox_SelectionChangeCommitted", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSampleTypesComboBox_SelectionChangeCommitted", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the list of Selectable Tests according the selected TestProfile, SampleType and TestType
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 13/10/2010
    ''' Modified by: SA 20/10/2010 - Inform property TestTypeToShow of the DoubleList control with the selected value
    ''' </remarks>
    Private Sub bsTestsTypesComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsTestsTypesComboBox.SelectionChangeCommitted
        Try
            If (bsTestsTypesComboBox.SelectedIndex > -1) Then
                bsTestsSelectionDoubleList.InitializeControl = False
                bsTestsSelectionDoubleList.TypeToShow = bsTestsTypesComboBox.SelectedValue.ToString
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestsTypesComboBox_SelectionChangeCommitted", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestsTypesComboBox_SelectionChangeCommitted", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the TextBox showing the error is changed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 07/07/2010
    ''' </remarks>
    Private Sub bsTextbox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsNameTextBox.TextChanged
        Try
            Dim myTextBox As New TextBox
            myTextBox = CType(sender, TextBox)

            If (myTextBox.TextLength > 0) Then
                If (bsScreenErrorProvider.GetError(myTextBox) <> "") Then
                    bsScreenErrorProvider.SetError(myTextBox, String.Empty)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTextbox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTextbox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '**********************'
    '* EVENTS FOR BUTTONS *'
    '**********************'
    ''' <summary>
    ''' Set the screen status to Add Mode
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsNewButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNewButton.Click
        Try
            AddTestProfile()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsNewButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsNewButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen status to Edit Mode for the selected Test Profile
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsEditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsEditButton.Click
        Try
            EditTestProfileByButtonClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsEditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsEditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Executes process of deletion the selected Test Profile
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsDeleteButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsDeleteButton.Click
        Try
            DeleteTestProfile()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsDeleteButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDeleteButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prints the Profiles Report
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 02/12/2011
    ''' </remarks>
    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            If (bsTestProfilesListView.SelectedItems.Count > 0) Then
                Dim Profiles As New List(Of Integer)

                For Each mySelectedItem As ListViewItem In bsTestProfilesListView.SelectedItems
                    Profiles.Add(CInt(mySelectedItem.Name))
                Next

                XRManager.ShowTestProfilesReport(Profiles)
            Else
                XRManager.ShowTestProfilesReport()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsPrintButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsPrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save changes (add or updation) in the database
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSaveButton.Click
        Try
            SaveChanges()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSaveButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSaveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Execute the cancelling of a Test Profile adding or edition
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            CancelTestProfileEdition()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen when there are not changes pending to save or there are some but the User decides discard them
    ''' </summary>
    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            ExitScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the customize order and availability for profiles tests selection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 04/09/2014 - BA-1869</remarks>
    Private Sub BsCustomOrderButton_Click(sender As Object, e As EventArgs) Handles BsCustomOrderButton.Click
        Try
            'Shown the Positioning Warnings Screen
            Using AuxMe As New ISortingTestsAux()
                AuxMe.openMode = "TESTSELECTION"
                AuxMe.screenID = "PROFILE"
                AuxMe.ShowDialog()
            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsCustomOrderButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsCustomOrderButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

End Class
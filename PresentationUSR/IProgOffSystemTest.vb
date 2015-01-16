Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Public Class IProgOffSystemTest

#Region "Declarations"
    Private EditionMode As Boolean = False                 'To control when the selected Calculated Test is in Edition Mode

    Private SelectedOffSystemTestID As Integer = 0         'To store the ID of the selected OffSystem Test
    Private OriginalOffSystemTestName As String = ""       'To store the Name of the selected OFF-SYSTEM Test and control Pending Changes
    Private OriginalSelectedIndex As Integer = -1          'To store the index of the selected OFF-SYSTEM Test and control Pending Changes

    Private SelectedTestSampleTypesDS As New OffSystemTestSamplesDS 'To manage the list of Sample Types defined for the selected OFF-SYSTEM Test
    Private SelectedTestRefRangesDS As New TestRefRangesDS          'To manage the Reference Ranges for the selected OFF-SYSTEM Test

    Private UpdateHistoryRequired As Boolean 'To indicate if data of the OFF-SYSTEM Test has to be also updated in Historic Module
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Set the screen controls to ADD MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/11/2010
    ''' Modified by: SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control
    ''' </remarks>
    Private Sub AddModeScreenStatus()
        Try
            'Area of Off-System Test List
            If (bsOffSystemTestListView.Items.Count > 0) Then
                bsOffSystemTestListView.Enabled = True
                bsOffSystemTestListView.SelectedItems.Clear()
            End If

            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = False 'AG 04/09/2014 - BA-1869
            '            bsPrintButton.Enabled = False DL 11/05/2012

            'Area of Calculated Test Definition
            bsFullNameTextbox.Text = ""
            bsFullNameTextbox.Enabled = True

            bsShortNameTextbox.Text = ""
            bsShortNameTextbox.Enabled = True
            bsShortNameTextbox.BackColor = Color.Khaki 'TR 19/03/2012 Set the required color.

            bsSampleTypeComboBox.SelectedIndex = -1
            bsSampleTypeComboBox.Enabled = True
            bsSampleTypeComboBox.BackColor = Color.Khaki

            bsUnitComboBox.SelectedIndex = -1
            bsUnitComboBox.Enabled = True
            bsUnitComboBox.BackColor = Color.Khaki

            bsResultTypeComboBox.SelectedIndex = -1
            bsResultTypeComboBox.Enabled = True
            bsResultTypeComboBox.BackColor = Color.Khaki

            'bsDefaultValueComboBox.SelectedIndex = -1
            'bsDe/faultValueComboBox.Enabled = True
            'bsDefaultValueComboBox.BackColor = Color.White

            'bsDefaultValueUpDown.Value = 0
            'bsDefaultValueUpDown.Enabled = True
            'bsDefaultValueUpDown.BackColor = Color.White

            bsDecimalsUpDown.Value = bsDecimalsUpDown.Minimum
            bsDecimalsUpDown.Enabled = True
            bsDecimalsUpDown.BackColor = Color.White

            'Area of Reference Ranges
            SelectedTestRefRangesDS = New TestRefRangesDS

            bsTestRefRanges.TestID = -1
            bsTestRefRanges.ActiveRangeType = ""
            bsTestRefRanges.MeasureUnit = bsUnitComboBox.Text.ToString
            bsTestRefRanges.DefinedTestRangesDS = SelectedTestRefRangesDS
            bsTestRefRanges.SampleType = "" 'TR-SA Before implementing the sample type need to initialize the DataSet

            Dim decimalsNumber As Integer = 0
            If (IsNumeric(bsDecimalsUpDown.Text)) Then decimalsNumber = Convert.ToInt32(bsDecimalsUpDown.Text)
            bsTestRefRanges.RefNumDecimals = decimalsNumber

            bsTestRefRanges.ClearReferenceRanges()
            bsTestRefRanges.isEditing = False

            'Clean error provider 
            bsScreenErrorProvider.Clear()

            'Buttons in details area
            bsSaveButton.Enabled = True
            bsCancelButton.Enabled = True

            'Initialize global variables
            CleanGlobalValues()

            'Put Focus in the first enabled field
            bsFullNameTextbox.Focus()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AddModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AddModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the screen for adding a new Off-System Test.  If there are changes pending
    ''' to save, the Discard Pending Changes Verification is executed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/11/2010
    ''' Modified by: SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control 
    ''' </remarks>
    Private Sub AddOffSystemTest()
        Try
            If (PendingChangesVerification()) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    'Set Screen Status to ADD MODE.
                    EditionMode = True
                    AddModeScreenStatus()
                    bsTestRefRanges.ChangesMade = False 'TR 13/10/2011 -Set the change made = false on the RefRanges.
                Else
                    If (OriginalSelectedIndex <> -1) Then
                        'Return focus to the Off-System Test that has been edited
                        bsOffSystemTestListView.Items(OriginalSelectedIndex).Selected = True
                        bsOffSystemTestListView.Select()
                    End If
                End If
            Else
                'Set Screen Status to ADD MODE
                EditionMode = True
                AddModeScreenStatus()
                bsTestRefRanges.ChangesMade = False 'TR 13/10/2011 -Set the change made = false on the RefRanges.
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AddOffSystemTest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AddOffSystemTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Execute the cancelling of a Off-System Test edition
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 26/11/2010 
    ''' Modified by: SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control 
    '''              TR 05/09/2012 - Set to false value of global variable UpdateHistoryRequired when the edition is cancelled
    ''' </remarks>
    Private Sub CancelOffSystemTestEdition()
        Try
            Dim setScreenToInitial As Boolean = False
            If (PendingChangesVerification()) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    EditionMode = False
                    UpdateHistoryRequired = False

                    SelectedTestRefRangesDS.Clear()
                    setScreenToInitial = True
                Else
                    If (OriginalSelectedIndex <> -1) Then
                        'Return focus to the Off-System Test that has been edited
                        bsOffSystemTestListView.Items(OriginalSelectedIndex).Selected = True
                        bsOffSystemTestListView.Select()
                    End If
                End If
            Else
                setScreenToInitial = True
            End If

            If (setScreenToInitial) Then
                UpdateHistoryRequired = False

                If (String.Compare(bsScreenErrorProvider.GetError(bsUnitComboBox), "", False) <> 0) Then
                    bsScreenErrorProvider.SetError(bsUnitComboBox, String.Empty)
                End If

                If (OriginalSelectedIndex <> -1) Then
                    bsOffSystemTestListView.Items(OriginalSelectedIndex).Selected = True
                Else
                    'Set screen status to Initial Mode
                    InitialModeScreenStatus()
                    If (bsOffSystemTestListView.Items.Count > 0) Then
                        'Select the first Off-System Test in the list
                        bsOffSystemTestListView.Items(0).Selected = True
                    End If
                End If

                If (bsOffSystemTestListView.SelectedItems.Count > 0) Then
                    'Load screen fields with all data of the selected OFF-SYSTEM Test 
                    Dim inUseOffSystemTest As Boolean = LoadDataOfOffSystemTest()

                    'Get the SampleType the OFF-SYSTEM Test is using currently and select it in the ComboBox of Sample Types
                    GetOffSystemTestSamples(SelectedOffSystemTestID)
                    bsSampleTypeComboBox.SelectedValue = SelectedTestSampleTypesDS.tparOffSystemTestSamples.First.SampleType

                    'Get the Reference Ranges defined for the OFF-SYSTEM Test and shown them
                    GetRefRanges(SelectedOffSystemTestID)
                    LoadRefRangesData()

                    If (Not inUseOffSystemTest) Then
                        'Set screen status to Query Mode
                        QueryModeScreenStatus()
                    Else
                        'Set screen status to Read Only Mode 
                        ReadOnlyModeScreenStatus()
                    End If
                    bsOffSystemTestListView.Focus()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CancelOffSystemTestEdition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CancelOffSystemTestEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the Global Variables used in the screen to store the selected offsystem Test
    ''' and/or to control when there are changes pending to save
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 25/11/2010
    ''' </remarks>
    Private Sub CleanGlobalValues()
        Try
            'Initialization of global variables
            SelectedOffSystemTestID = 0
            OriginalSelectedIndex = -1
            OriginalOffSystemTestName = ""
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CleanGlobalValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CleanGlobalValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Delete the selected OffSystem Test(s).
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 01/12/2010
    ''' Modified by: SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control
    '''              SA 04/01/2011 - Added the validation of affected elements before deletion
    '''              TR 05/09/2012 - Set to false value of global variable UpdateHistoryRequired once the deleted has been executed
    ''' </remarks>
    Private Sub DeleteOffSystemTests()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim continueDeleting As Boolean = False

            If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                'Load all selected OffSystem Tests in a typed DataSet OffSystemTestsDS
                Dim offSytemTestDataDS As New OffSystemTestsDS
                Dim offSystemTestRow As OffSystemTestsDS.tparOffSystemTestsRow

                For Each mySelectedItem As ListViewItem In bsOffSystemTestListView.SelectedItems
                    offSystemTestRow = offSytemTestDataDS.tparOffSystemTests.NewtparOffSystemTestsRow

                    offSystemTestRow.OffSystemTestID = CInt(mySelectedItem.Name)
                    offSystemTestRow.Name = mySelectedItem.Text
                    offSytemTestDataDS.tparOffSystemTests.Rows.Add(offSystemTestRow)
                Next

                If (offSytemTestDataDS.tparOffSystemTests.Rows.Count > 0) Then
                    'Verify if there other elements affected for the deletion ...
                    Dim myDependeciesElementsDS As New DependenciesElementsDS

                    myGlobalDataTO = ValidateDependencies(offSytemTestDataDS)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myDependeciesElementsDS = DirectCast(myGlobalDataTO.SetDatos, DependenciesElementsDS)

                        If (myDependeciesElementsDS.DependenciesElements.Count > 0) Then
                            Using AffectedElement As New IWarningAfectedElements
                                AffectedElement.AffectedElements = myDependeciesElementsDS
                                AffectedElement.ShowDialog()

                                continueDeleting = (AffectedElement.DialogResult = Windows.Forms.DialogResult.OK)
                            End Using
                        Else
                            continueDeleting = True
                        End If
                    Else
                        'Error getting the list of affected elements; show it
                        ShowMessage(Name & ".SaveChanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                End If

                If (continueDeleting) Then
                    Dim myOffSystemTestDelegate As New OffSystemTestsDelegate

                    myGlobalDataTO = myOffSystemTestDelegate.Delete(Nothing, offSytemTestDataDS)
                    If (Not myGlobalDataTO.HasError) Then
                        UpdateHistoryRequired = False

                        'Refresh the list of Off System Tests and set screen status to Initial Mode
                        LoadOffSystemTestList()
                        InitialModeScreenStatus()

                        If (bsOffSystemTestListView.Items.Count > 0) Then
                            'Select the first Off System Test in the list
                            bsOffSystemTestListView.Items(0).Selected = True
                            QueryOffSystemTest()
                            bsOffSystemTestListView.Focus()
                        End If
                    Else
                        'Error deleting the selected OffSystem Tests, show the error message
                        ShowMessage(Me.Name & ".DeleteOffSystemTest", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteOffSystemTest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteOffSystemTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to EDIT MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 25/11/2010 
    ''' Modified by: SA 03/01/2011 - Changes due to new implementation of Reference Ranges Control
    ''' </remarks>
    Private Sub EditModeScreenStatus()
        Try
            'Area of OFF-SYSTEM Test List
            'Disable ListView in EDIT Mode IR 02/10/2012
            bsOffSystemTestListView.MultiSelect = False

            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = False 'AG 04/09/2014 - BA-1869
            'bsPrintButton.Enabled = False DL 11/05/2012

            bsFullNameTextbox.Enabled = True
            bsFullNameTextbox.BackColor = Color.White

            bsShortNameTextbox.Enabled = True
            bsShortNameTextbox.BackColor = Color.White

            bsSampleTypeComboBox.Enabled = True
            bsSampleTypeComboBox.BackColor = Color.White

            bsResultTypeComboBox.Enabled = True
            bsResultTypeComboBox.BackColor = Color.White

            bsSaveButton.Enabled = True
            bsCancelButton.Enabled = True

            'DL 27/01/2011 - Status of fields MeasureUnit and DecimalsNumber will depend on the ResultType of the selected Test
            If (String.Compare(bsResultTypeComboBox.SelectedValue.ToString, "QUALTIVE", False) = 0) Then
                bsUnitComboBox.SelectedValue = -1
                bsUnitComboBox.Enabled = False
                bsUnitComboBox.BackColor = SystemColors.MenuBar

                bsDecimalsUpDown.Value = 0
                bsDecimalsUpDown.Enabled = False
                bsDecimalsUpDown.BackColor = SystemColors.MenuBar

                bsTestRefRanges.isEditing = False

            ElseIf (bsResultTypeComboBox.SelectedValue.ToString = "QUANTIVE") Then
                bsUnitComboBox.Enabled = True
                bsUnitComboBox.BackColor = Color.White

                bsDecimalsUpDown.Enabled = True
                bsDecimalsUpDown.BackColor = Color.White

                bsTestRefRanges.isEditing = True
            End If

            'TR 14/10/2011 -Set focus.
            bsFullNameTextbox.Focus()
            'TR 14/10/2011 -END.


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the Edit Button is clicked, set the screen status to EDIT MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 29/11/2010
    ''' Modified by: SA 03/01/2011 - Changes due to new implementation of Reference Ranges Control
    ''' </remarks>
    Private Sub EditOffSystemTestByButtonClick()
        Try
            If (Not bsShortNameTextbox.Enabled) Then
                'Set screen status to Edit Mode
                EditionMode = True
                EditModeScreenStatus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EditOffSystemTestByButtonClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditOffSystemTestByButtonClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When double clicking in a OffSystem Test in the ListView, verify if there are changes pending 
    ''' to save and set the screen status to EDIT MODE if it is not InUse, or to READ ONLY MODE if it 
    ''' is InUse in the active Work Session
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 20/10/2010
    '''              SA 03/01/2011 - Changes due to new implementation of Reference Ranges Control
    ''' </remarks>
    Private Sub EditOffSystemTestByDoubleClick()
        Dim setScreenToEdition As Boolean = False
        Try
            If (bsOffSystemTestListView.SelectedIndices(0) <> OriginalSelectedIndex) Then
                If (PendingChangesVerification()) Then
                    If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                        SelectedTestRefRangesDS.Clear()
                        setScreenToEdition = True
                    Else
                        If (OriginalSelectedIndex <> -1) Then
                            'Return focus to the OFF-SYSTEM Test that has been edited
                            bsOffSystemTestListView.Items(OriginalSelectedIndex).Selected = True
                            bsOffSystemTestListView.Select()
                        End If
                    End If
                Else
                    setScreenToEdition = True
                End If
            Else
                'If the screen was in Read Only Mode, it is changed to Edit Mode although the User
                'double-clicking in the same OFF-SYSTEM Test 
                If (Not bsShortNameTextbox.Enabled) Then setScreenToEdition = True
            End If

            EditionMode = setScreenToEdition
            If (setScreenToEdition) Then
                'Load screen fields with all data of the selected OFF-SYSTEM Test
                Dim inUseOffSystemTest As Boolean = LoadDataOfOffSystemTest()

                'Get the SampleType the OFF-SYSTEM Test is using currently and select it in the ComboBox of Sample Types
                GetOffSystemTestSamples(SelectedOffSystemTestID)
                bsSampleTypeComboBox.SelectedValue = SelectedTestSampleTypesDS.tparOffSystemTestSamples.First.SampleType

                'Get the Reference Ranges defined for the OFF-SYSTEM Test and shown them
                GetRefRanges(SelectedOffSystemTestID)
                LoadRefRangesData()

                If (Not inUseOffSystemTest) Then
                    'Set screen status to Query Mode
                    EditModeScreenStatus()
                Else
                    'Set screen status to Read Only Mode 
                    ReadOnlyModeScreenStatus()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditOffSystemTestByDoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditOffSystemTestByDoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen, verifying first if there are changes pending to save
    ''' </summary>
    Private Sub ExitScreen()
        Dim screenClose As Boolean = True
        Try
            If (EditionMode) Then
                If (PendingChangesVerification()) Then
                    If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No) Then
                        screenClose = False
                    End If
                End If
            End If

            If (screenClose) Then
                'TR 11/04/2012 -Disable form on close to avoid any button press.
                Me.Enabled = False

                'RH 16/12/2010
                If (Not Tag Is Nothing) Then
                    'A PerformClick() method was executed
                    Close()
                Else
                    'Normal button click - open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
            Else
                bsFullNameTextbox.Focus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Gets all defined Age Units to pass to the Ref Ranges Control
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 01/09/2010
    ''' </remarks>
    Private Function GetAgeUnits() As PreloadedMasterDataDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate()

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.AGE_UNITS)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS

                myPreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Return myPreloadedMasterDataDS
            Else
                'Error getting the list of defined Age Units; shown it
                ShowMessage(Name & ".GetAgeUnits", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetAgeUnits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetAgeUnits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Gets all defined SubTypes for Detailed Reference Ranges to pass to the Ref Ranges Control
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 14/12/2010 (as copy of GetGenders)
    ''' </remarks>
    Private Function GetDetailedRangesSubTypes() As PreloadedMasterDataDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate()

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.RANGE_SUBTYPES)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS

                myPreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Return myPreloadedMasterDataDS
            Else
                'Error getting the list of available Detailed Reference Ranges Subtypes; shown it
                ShowMessage(Name & ".GetDetailedRangesSubTypes", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetDetailedRangesSubTypes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetDetailedRangesSubTypes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Gets all defined Genders to pass to the Ref Ranges Control
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 01/09/2010
    ''' </remarks>
    Private Function GetGenders() As PreloadedMasterDataDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate()

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.SEX_LIST)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreloadedMasterDataDS As PreloadedMasterDataDS

                myPreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Return myPreloadedMasterDataDS
            Else
                'Error getting the list of available Genders; shown it
                ShowMessage(Name & ".GetGenders", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetGenders ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetGenders", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Gets all defined field limits to pass to the Ref Ranges Control
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 01/09/2010
    ''' </remarks>
    Private Function GetLimits() As FieldLimitsDS
        Dim myFieldLimitsDS As New FieldLimitsDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()

            myGlobalDataTO = myFieldLimitsDelegate.GetAllList(Nothing)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                myFieldLimitsDS = DirectCast(myGlobalDataTO.SetDatos, FieldLimitsDS)
            Else
                'Error getting the defined Field Limits; shown it
                ShowMessage(Name & ".GetLimits", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetLimits ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myFieldLimitsDS
    End Function

    ''' <summary>
    ''' Get all defined Messages to pass to the Ref Ranges Control
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 01/09/2010
    ''' </remarks>
    Private Function GetMessages() As MessagesDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myMessageDelegate As New MessageDelegate()

            myGlobalDataTO = myMessageDelegate.GetAllMessageDescription(Nothing)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMessagesDS As New MessagesDS

                myMessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)
                Return myMessagesDS
            Else
                'Error getting the list of defined Messages; shown it
                ShowMessage(Name & ".GetMessages", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetMessages", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetMessages", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Get the list of Sample Types currently used for the selected OFF-SYSTEM Test
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 25/11/2010
    ''' </remarks>
    Private Sub GetOffSystemTestSamples(ByVal pOffSystemTestID As Integer)
        Try
            Dim myGlobalDataTo As New GlobalDataTO
            Dim myOffSystemTestSampleDelegate As New OffSystemTestSamplesDelegate

            myGlobalDataTo = myOffSystemTestSampleDelegate.GetListByOffSystemTestID(Nothing, pOffSystemTestID)
            If (Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing) Then
                SelectedTestSampleTypesDS = DirectCast(myGlobalDataTo.SetDatos, OffSystemTestSamplesDS)
            Else
                'Error getting the list of SampleTypes currently defined for the selected OFF-SYSTEM Test 
                ShowMessage(Name & ".GetOffSystemTestSamples", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetOffSystemTestSamples", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetOffSystemTestSamples", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Gets the ref ranges defined for the specified OFF-SYSTEM test
    ''' </summary>
    ''' <param name="pOffSystemTestID">OffSystem Test Identifier</param>
    ''' <remarks>
    ''' Modified by: DL 25/11/2010
    ''' </remarks>
    Private Sub GetRefRanges(ByVal pOffSystemTestID As Integer)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestRefRangesDelegate As New TestRefRangesDelegate

            myGlobalDataTO = myTestRefRangesDelegate.ReadByTestID(Nothing, pOffSystemTestID, , , "OFFS")
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                SelectedTestRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)
            Else
                SelectedTestRefRangesDS = New TestRefRangesDS
                ShowMessage(Me.Name & ".GetRefRanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetRefRanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetRefRanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by:  SG 20/10/2010
    ''' Modified by: SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control 
    '''              AG 05/09/2014 - BA-1869 ==> Added ToolTip for new button used to open the auxiliary screen that allow sort and set the  
    '''                                          availability of OFFS Tests (Custom Order Button)
    '''              SA 17/11/2014 - BA-2125 ==> Added ToolTip for new button used to open the auxiliary screen that allow sort and set the  
    '''                                          availability of OFFS Tests (Custom Order Button) - previous change was not really done; code 
    '''                                          was commented and the label was not the correct one. Commented code to get ToolTip for Print 
    '''                                          Button due to it is not visible.
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsOffSystemTestListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_OffSystemTests_List", pLanguageID)
            bsOffSystemTestDefLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_OffSystemTests_Definition", pLanguageID)

            bsFullNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", pLanguageID) + ":"
            bsNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ShortName", pLanguageID) + ":"

            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID) + ":"
            bsUnitLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", pLanguageID) + ":"
            bsDecimalsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Decimals", pLanguageID) + ":"
            bsResultTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ResultType", pLanguageID) + ":"
            'bsDefaultValueLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "????", pLanguageID) + ":"

            'For bsTestRefRanges
            RefRangesTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Long", pLanguageID)

            bsTestRefRanges.TextForGenericRadioButton = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Generic", pLanguageID)
            bsTestRefRanges.TextForNormalityLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Normality", pLanguageID) & ":"
            bsTestRefRanges.TextForMinValueLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MinValue", pLanguageID)
            bsTestRefRanges.TextForMaxValueLabel = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MaxValue", pLanguageID)

            bsTestRefRanges.TextForDetailedRadioButton = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DetailedReferenceRange", pLanguageID)
            bsTestRefRanges.TextForGenderColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Gender", pLanguageID)
            bsTestRefRanges.TextForAgeColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Age", pLanguageID)
            bsTestRefRanges.TextForFromColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_From", pLanguageID)
            bsTestRefRanges.TextForToColumn = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_To", pLanguageID)

            bsTestRefRanges.ToolTipForDetailDeleteButton = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DelReferenceRange", pLanguageID)

            'For Tooltips
            bsScreenToolTips.SetToolTip(bsNewButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", pLanguageID))
            bsScreenToolTips.SetToolTip(bsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            bsScreenToolTips.SetToolTip(BsCustomOrderButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TEST_SORTING_SELECTION", pLanguageID))
            'bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))

            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure and initialize the ListView of OFF-SYSTEM Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 25/11/2010 
    ''' </remarks>
    Private Sub InitializeOffSystemTestList(ByVal pLanguageID As String)
        Try
            'Initialization of OFF-SYSTEM Tests List
            bsOffSystemTestListView.Items.Clear()

            bsOffSystemTestListView.Alignment = ListViewAlignment.Left
            bsOffSystemTestListView.FullRowSelect = True
            bsOffSystemTestListView.MultiSelect = True
            bsOffSystemTestListView.Scrollable = True
            bsOffSystemTestListView.View = View.Details
            bsOffSystemTestListView.HideSelection = False
            bsOffSystemTestListView.HeaderStyle = ColumnHeaderStyle.Clickable

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            bsOffSystemTestListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestNames", pLanguageID), -2, HorizontalAlignment.Left)

            bsOffSystemTestListView.Columns.Add("OffSystemTestID", 0, HorizontalAlignment.Left)
            bsOffSystemTestListView.Columns.Add("Name", 0, HorizontalAlignment.Left)
            bsOffSystemTestListView.Columns.Add("ShortName", 0, HorizontalAlignment.Left)
            bsOffSystemTestListView.Columns.Add("Units", 0, HorizontalAlignment.Left)
            bsOffSystemTestListView.Columns.Add("Decimals", 0, HorizontalAlignment.Left)
            bsOffSystemTestListView.Columns.Add("ResultType", 0, HorizontalAlignment.Left)
            bsOffSystemTestListView.Columns.Add("InUse", 0, HorizontalAlignment.Left)

            'Fill ListView with the list of existing Off-System Tests
            LoadOffSystemTestList()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeOffSystemTestList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeOffSystemTestList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the Reference Ranges User control, passing to it all DB data it neededs to work
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 16/12/2010
    ''' </remarks>
    Private Sub InitializeReferenceRangesControl()
        Try
            'Control will be shown using the "big" layout and used for Off System Tests
            bsTestRefRanges.SmallLayout = False
            bsTestRefRanges.TestType = "OFFS"

            'Load the necessary data to the Reference Ranges User Control
            Dim myAllFieldLimitsDS As FieldLimitsDS = GetLimits()
            Dim myGendersMasterDataDS As PreloadedMasterDataDS = GetGenders()
            Dim myAgeUnitsMasterDataDS As PreloadedMasterDataDS = GetAgeUnits()
            Dim myDetailedRangeSubTypesDS As PreloadedMasterDataDS = GetDetailedRangesSubTypes()
            Dim myAllMessagesDS As MessagesDS = GetMessages()

            bsTestRefRanges.LoadFrameworkData(myAllFieldLimitsDS, myGendersMasterDataDS, myAgeUnitsMasterDataDS, _
                                              myDetailedRangeSubTypesDS, myAllMessagesDS, SystemInfoManager.OSDecimalSeparator)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeReferenceRangesControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeReferenceRangesControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control  
    ''' </remarks>
    Private Sub InitialModeScreenStatus(Optional ByVal pInitializeListView As Boolean = True)
        Try
            If (pInitializeListView) Then
                'Area of OFF-SYSTEM Tests List
                If (bsOffSystemTestListView.Items.Count > 0) Then
                    bsOffSystemTestListView.Enabled = True
                    bsOffSystemTestListView.SelectedItems.Clear()
                End If

                bsNewButton.Enabled = True
                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
                BsCustomOrderButton.Enabled = False 'AG 04/09/2014 - BA-1869
                'bsPrintButton.Enabled = False DL 11/05/2012
            End If

            'Area of OFF-SYSTEM Test Definition
            bsFullNameTextbox.Text = String.Empty
            bsFullNameTextbox.Enabled = False
            bsFullNameTextbox.BackColor = SystemColors.MenuBar

            bsShortNameTextbox.Text = String.Empty
            bsShortNameTextbox.Enabled = False
            bsShortNameTextbox.BackColor = SystemColors.MenuBar

            bsSampleTypeComboBox.SelectedIndex = -1
            bsSampleTypeComboBox.Enabled = False
            bsSampleTypeComboBox.BackColor = SystemColors.MenuBar

            bsUnitComboBox.SelectedIndex = -1
            bsUnitComboBox.Enabled = False
            bsUnitComboBox.BackColor = SystemColors.MenuBar

            bsDecimalsUpDown.Value = bsDecimalsUpDown.Minimum
            bsDecimalsUpDown.Enabled = False
            bsDecimalsUpDown.BackColor = SystemColors.MenuBar

            bsResultTypeComboBox.SelectedIndex = -1
            bsResultTypeComboBox.Enabled = False
            bsResultTypeComboBox.BackColor = SystemColors.MenuBar

            'bsDefaultValueComboBox.SelectedIndex = -1
            'bsDefaultValueComboBox.Visible = True
            'bsDefaultValueComboBox.BackColor = SystemColors.MenuBar
            'bsDefaultValueComboBox.Enabled = False

            'bsDefaultValueUpDown.Visible = False
            'bsDefaultValueUpDown.Value = bsDefaultValueUpDown.Minimum
            'bsDefaultValueUpDown.BackColor = SystemColors.MenuBar
            'bsDefaultValueUpDown.Enabled = False

            'Area of Reference Ranges
            InitializeReferenceRangesControl()
            bsTestRefRanges.isEditing = False

            'Buttons in details area
            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False

            'Initialize global variables and controls
            bsScreenErrorProvider.Clear()
            CleanGlobalValues()

            'Focus to button Add
            If (pInitializeListView) Then bsNewButton.Focus()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitialModeScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitialModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill screen fields with data of the selected Off System Test
    ''' </summary>
    ''' <returns>True if the Off System Test is In Use; otherwise False</returns>
    ''' <remarks>
    ''' Created by: SA 24/12/2010
    ''' </remarks>
    Private Function LoadDataOfOffSystemTest() As Boolean
        Dim inUse As Boolean = False

        Try
            SelectedOffSystemTestID = CInt(bsOffSystemTestListView.SelectedItems(0).SubItems(1).Text)
            OriginalOffSystemTestName = bsOffSystemTestListView.SelectedItems(0).SubItems(0).Text
            OriginalSelectedIndex = bsOffSystemTestListView.SelectedIndices(0)

            'Fill screen controls with data of the selected Off System Test
            bsFullNameTextbox.Text = bsOffSystemTestListView.SelectedItems(0).SubItems(0).Text
            bsShortNameTextbox.Text = bsOffSystemTestListView.SelectedItems(0).SubItems(2).Text

            bsUnitComboBox.SelectedValue = bsOffSystemTestListView.SelectedItems(0).SubItems(3).Text
            bsDecimalsUpDown.Value = Convert.ToDecimal(bsOffSystemTestListView.SelectedItems(0).SubItems(4).Text)
            bsResultTypeComboBox.SelectedValue = bsOffSystemTestListView.SelectedItems(0).SubItems(5).Text

            inUse = Convert.ToBoolean(bsOffSystemTestListView.SelectedItems(0).SubItems(6).Text)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadDataOfOffSystemTest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadDataOfOffSystemTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return inUse
    End Function

    ''' <summary>
    ''' Read the limits defined for field Number of Decimals 
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 13/12/2010
    ''' </remarks>
    Private Sub LoadDecimalsLimit()
        Try
            'Get the Field Limits for control Num Decimals
            Dim myGlobalDataTo As New GlobalDataTO
            Dim fieldLimitsConfig As New FieldLimitsDelegate

            myGlobalDataTo = fieldLimitsConfig.GetList(Nothing, FieldLimitsEnum.CTEST_NUM_DECIMALS)
            If (Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing) Then
                Dim fieldDS As New FieldLimitsDS
                fieldDS = CType(myGlobalDataTo.SetDatos, FieldLimitsDS)

                If (fieldDS.tfmwFieldLimits.Rows.Count > 0) Then
                    'Inform limit values for the NumericUpdown of Decimals Values
                    bsDecimalsUpDown.Minimum = CType(fieldDS.tfmwFieldLimits(0).MinValue, Integer)
                    bsDecimalsUpDown.Maximum = CType(fieldDS.tfmwFieldLimits(0).MaxValue, Integer)
                    bsDecimalsUpDown.Value = CType(fieldDS.tfmwFieldLimits(0).DefaultValue, Integer)
                End If
            Else
                'Error getting the limits defined, show the error message
                ShowMessage(Name & ".LoadDecimalsLimit", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadDecimalsLimit", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadDecimalsLimit", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ListView of Off-System Tests with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 25/11/2010
    ''' Modified by: SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control 
    '''              SA 28/01/2011 - Verify fields Unit and Decimals are not NULL before adding them to the ListView
    ''' </remarks>
    Private Sub LoadOffSystemTestList()
        Try
            Dim myIcons As New ImageList

            'Get the Icon defined for OFF-SYSTEM Tests that are not in use in the current Work Session
            Dim notInUseIcon As String = GetIconName("TOFF_SYS")
            If (String.Compare(notInUseIcon, "", False) <> 0) Then
                myIcons.Images.Add("TOFF_SYS", Image.FromFile(IconsPath & notInUseIcon))
            End If

            'Get the Icon defined for OFF-SYSTEM Tests that are not in use in the current Work Session
            Dim inUseIcon As String = GetIconName("INUSEOFFS")
            If (String.Compare(inUseIcon, "", False) <> 0) Then
                myIcons.Images.Add("INUSEOFFS", Image.FromFile(IconsPath & inUseIcon))
            End If

            'Assign the Icons to the OFF-SYSTEM Tests List View
            bsOffSystemTestListView.Items.Clear()
            bsOffSystemTestListView.SmallImageList = myIcons

            'Get the list of existing OFF-SYSTEM Tests
            Dim resultData As New GlobalDataTO
            Dim myOffSystemTestList As New OffSystemTestsDelegate

            resultData = myOffSystemTestList.GetList(Nothing)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim myOffsystemTestDS As New OffSystemTestsDS
                myOffsystemTestDS = DirectCast(resultData.SetDatos, OffSystemTestsDS)

                'Sort the returned OFF-SYSTEM Tests
                Dim qOffSystemTests As List(Of OffSystemTestsDS.tparOffSystemTestsRow)
                Select Case bsOffSystemTestListView.Sorting
                    Case SortOrder.Ascending
                        qOffSystemTests = (From a In myOffsystemTestDS.tparOffSystemTests _
                                         Select a _
                                       Order By a.Name Ascending).ToList()

                    Case SortOrder.Descending
                        qOffSystemTests = (From a In myOffsystemTestDS.tparOffSystemTests _
                                         Select a _
                                       Order By a.Name Descending).ToList()

                    Case SortOrder.None
                        qOffSystemTests = (From a In myOffsystemTestDS.tparOffSystemTests _
                                         Select a).ToList()
                    Case Else
                        qOffSystemTests = (From a In myOffsystemTestDS.tparOffSystemTests _
                                         Select a).ToList()
                End Select

                'Fill the List View with all existing OFF-SYSTEM Tests
                Dim i As Integer = 0
                Dim rowToSelect As Integer = -1

                For Each offSystemTest As OffSystemTestsDS.tparOffSystemTestsRow In qOffSystemTests
                    Dim iconNameVar As String = "TOFF_SYS"
                    If (offSystemTest.InUse) Then iconNameVar = "INUSEOFFS"

                    bsOffSystemTestListView.Items.Add(offSystemTest.OffSystemTestID.ToString, _
                                                      offSystemTest.Name.ToString, _
                                                      iconNameVar).Tag = offSystemTest.InUse

                    bsOffSystemTestListView.Items(i).SubItems.Add(offSystemTest.OffSystemTestID.ToString)
                    bsOffSystemTestListView.Items(i).SubItems.Add(offSystemTest.ShortName.ToString)

                    Dim measureUnit As String = ""
                    If (Not offSystemTest.IsUnitsNull) Then measureUnit = offSystemTest.Units.ToString
                    bsOffSystemTestListView.Items(i).SubItems.Add(measureUnit)

                    Dim decimals As String = "0"
                    If (Not offSystemTest.IsDecimalsNull) Then decimals = offSystemTest.Decimals.ToString
                    bsOffSystemTestListView.Items(i).SubItems.Add(decimals)

                    bsOffSystemTestListView.Items(i).SubItems.Add(offSystemTest.ResultType.ToString)
                    bsOffSystemTestListView.Items(i).SubItems.Add(offSystemTest.InUse.ToString)

                    'If there is a selected Off System Test and it is still in the list, its position is stored to re-select 
                    'the same Off System Test once the list is loaded
                    If (SelectedOffSystemTestID = CLng(offSystemTest.OffSystemTestID)) Then rowToSelect = i
                    i += 1
                Next

                If (rowToSelect = -1) Then
                    'There was not a selected OFF-SYSTEM Test or the selected one is not in the list; the global variables containing 
                    'information of the selected OFF-SYSTEM Test is initializated
                    CleanGlobalValues()
                Else
                    'If there is a selected OFF-SYSTEM Test, focus is put in the correspondent element in the Test OFF-SYSTEM List
                    bsOffSystemTestListView.Items(rowToSelect).Selected = True
                    bsOffSystemTestListView.Select()

                    'The global variable containing the index of the selected OffSystem Test is updated
                    OriginalSelectedIndex = bsOffSystemTestListView.SelectedIndices(0)
                End If
            End If

            'An error has happened getting data from the Database
            If (resultData.HasError) Then
                ShowMessage(Me.Name & ".LoadOffSystemTestList", resultData.ErrorCode, resultData.ErrorMessage)

                CleanGlobalValues()
                bsOffSystemTestListView.Enabled = False
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadOffSystemTestList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadOffSystemTestList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBox of Measure Units with the list of existing ones
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadMeasureUnits()
        Try
            'Get the list of existing Measure Units
            Dim myGlobalDataTo As New GlobalDataTO
            Dim masterDataConfig As New MasterDataDelegate

            myGlobalDataTo = masterDataConfig.GetList(Nothing, MasterDataEnum.TEST_UNITS.ToString)
            If (Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing) Then
                Dim masterDataDS As New MasterDataDS
                masterDataDS = DirectCast(myGlobalDataTo.SetDatos, MasterDataDS)
                If (masterDataDS.tcfgMasterData.Rows.Count > 0) Then
                    'Fill ComboBox of Measure Units in area of Off System Test details
                    bsUnitComboBox.DataSource = masterDataDS.tcfgMasterData
                    bsUnitComboBox.DisplayMember = "FixedItemDesc"
                    bsUnitComboBox.ValueMember = "ItemID"
                End If
            Else
                'Error getting the list of defined Measure Units, show the error message
                ShowMessage(Me.Name & ".LoadMeasureUnits", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadMeasureUnits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadMeasureUnits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Shows the Reference Ranges defined for the selected OffSystem Test
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 24/12/2010
    ''' </remarks>
    Private Sub LoadRefRangesData()
        Try
            If (bsOffSystemTestListView.SelectedItems.Count = 1) Then
                bsTestRefRanges.TestID = CInt(bsOffSystemTestListView.SelectedItems(0).SubItems(1).Text)
                bsTestRefRanges.SampleType = SelectedTestSampleTypesDS.tparOffSystemTestSamples.First.SampleType

                If (SelectedTestSampleTypesDS.tparOffSystemTestSamples.First.IsActiveRangeTypeNull) Then
                    bsTestRefRanges.ActiveRangeType = String.Empty
                Else
                    bsTestRefRanges.ActiveRangeType = SelectedTestSampleTypesDS.tparOffSystemTestSamples.First.ActiveRangeType
                End If

                bsTestRefRanges.MeasureUnit = bsUnitComboBox.Text.ToString
                bsTestRefRanges.DefinedTestRangesDS = SelectedTestRefRangesDS

                Dim decimalsNumber As Integer = 0
                If (IsNumeric(bsDecimalsUpDown.Text)) Then decimalsNumber = Convert.ToInt32(bsDecimalsUpDown.Text)
                bsTestRefRanges.RefNumDecimals = decimalsNumber

                If (SelectedTestRefRangesDS IsNot Nothing) Then
                    bsTestRefRanges.LoadReferenceRanges()
                Else
                    bsTestRefRanges.ClearReferenceRanges()
                End If

                bsTestRefRanges.isEditing = bsFullNameTextbox.Enabled
            Else
                bsTestRefRanges.ClearData()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadRefRangesData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadRefRangesData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBox of Result Types with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 26/11/2010
    ''' Modified by: SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control 
    ''' </remarks>
    Private Sub LoadResultTypeCombo()
        Try
            'Get the list of existing Result Types
            Dim myGlobalDataTo As New GlobalDataTO
            Dim masterDataConfig As New PreloadedMasterDataDelegate

            myGlobalDataTo = masterDataConfig.GetList(Nothing, PreloadedMasterDataEnum.RESULT_TYPE)
            If (Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing) Then
                Dim masterDataDS As New PreloadedMasterDataDS
                masterDataDS = DirectCast(myGlobalDataTo.SetDatos, PreloadedMasterDataDS)

                If (masterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                    'Fill ComboBox of Sample Types in area of Off System Test details
                    bsResultTypeComboBox.DataSource = masterDataDS.tfmwPreloadedMasterData
                    bsResultTypeComboBox.DisplayMember = "FixedItemDesc"
                    bsResultTypeComboBox.ValueMember = "ItemID"
                End If
            Else
                'Error getting the list of defined Result Types, show the error message
                ShowMessage(Name & ".LoadResultTypeCombo", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadResultTypeCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadResultTypeCombo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBox of Sample Types with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 26/11/2010
    ''' </remarks>
    Private Sub LoadSampleTypesList()
        Try
            'Get the list of existing Sample Types
            Dim myGlobalDataTo As New GlobalDataTO
            Dim masterDataConfig As New MasterDataDelegate

            myGlobalDataTo = masterDataConfig.GetList(Nothing, MasterDataEnum.SAMPLE_TYPES.ToString)
            If (Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing) Then
                Dim masterDataDS As New MasterDataDS
                masterDataDS = DirectCast(myGlobalDataTo.SetDatos, MasterDataDS)

                If (masterDataDS.tcfgMasterData.Rows.Count > 0) Then
                    'Fill ComboBox of Sample Types in area of Off System Test details
                    bsSampleTypeComboBox.DataSource = masterDataDS.tcfgMasterData
                    bsSampleTypeComboBox.DisplayMember = "ItemIDDesc"
                    bsSampleTypeComboBox.ValueMember = "ItemID"
                End If
            Else
                'Error getting the list of defined Sample Types, show the error message
                ShowMessage(Name & ".LoadSampleTypesList", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadSampleTypesList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadSampleTypesList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Verify if there is at least one User's change pending to save
    ''' </summary>
    ''' <returns>True if there are changes pending to save; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  DL 25/11/2010 
    ''' Modified by: SA 03/01/2011 - Changes due to new implementation of Reference Ranges Control  
    ''' </remarks>
    Public Function PendingChangesVerification() As Boolean
        Dim pendingToSaveChanges As Boolean = False

        Try
            If (EditionMode) Then
                'When changes have been made in the Test Reference Ranges or there are errors still unsolved
                If (bsTestRefRanges.ChangesMade Or bsTestRefRanges.ValidationError) Then
                    pendingToSaveChanges = True
                    SelectedTestRefRangesDS = DirectCast(bsTestRefRanges.DefinedTestRangesDS, TestRefRangesDS) 'bsTestRefRanges.DefinedTestRangesDS

                ElseIf (SelectedOffSystemTestID = 0) Then
                    'In Add Mode, if at least one of the detail values has been changed, then there are changes pending to save
                    If (String.Compare(bsFullNameTextbox.Text.Trim, "", False) <> 0 Or bsShortNameTextbox.Text.Trim <> "" Or _
                        bsSampleTypeComboBox.SelectedIndex <> -1 Or bsUnitComboBox.SelectedIndex <> -1 Or _
                        Convert.ToInt32(bsDecimalsUpDown.Value) <> 0 Or bsResultTypeComboBox.SelectedIndex <> -1) Then
                        pendingToSaveChanges = True
                    End If

                Else
                    If (bsOffSystemTestListView.SelectedItems.Count = 0) Then
                        'If there is an OFF-SYSTEM Test in edition and a click is made out of the list of Tests,
                        'the Test in edition is selected to avoid errors if the screen is closed or the edition cancelled
                        If (OriginalSelectedIndex > 0) Then
                            'Return focus to the Off-System Test that has been edited
                            bsOffSystemTestListView.Items(OriginalSelectedIndex).Selected = True
                            bsOffSystemTestListView.Select()
                        End If
                    End If

                    'In Edit Mode, loading values are compared against current values
                    If (bsOffSystemTestListView.SelectedIndices(0) = OriginalSelectedIndex) Then
                        pendingToSaveChanges = (bsFullNameTextbox.Text.Trim <> OriginalOffSystemTestName) OrElse _
                                               (bsShortNameTextbox.Text.Trim <> bsOffSystemTestListView.SelectedItems(0).SubItems(2).Text) OrElse _
                                               (CDbl(bsDecimalsUpDown.Value) <> CDbl(bsOffSystemTestListView.SelectedItems(0).SubItems(4).Text)) OrElse _
                                               (bsResultTypeComboBox.SelectedValue.ToString <> bsOffSystemTestListView.SelectedItems(0).SubItems(5).Text) OrElse _
                                               (bsSampleTypeComboBox.SelectedValue.ToString <> SelectedTestSampleTypesDS.tparOffSystemTestSamples(0).SampleType)

                        If (Not pendingToSaveChanges) Then
                            'If the Test continues as Quantitative, verify if the MeasureUnit has been changed
                            If (bsResultTypeComboBox.SelectedValue.ToString = bsOffSystemTestListView.Items(0).SubItems(5).Text) And _
                               (bsResultTypeComboBox.SelectedValue.ToString = "QUANTIVE") Then
                                pendingToSaveChanges = (bsUnitComboBox.SelectedValue.ToString <> bsOffSystemTestListView.Items(0).SubItems(3).Text)
                            End If
                        End If
                    Else
                        pendingToSaveChanges = (bsFullNameTextbox.Text.Trim <> OriginalOffSystemTestName) OrElse _
                                               (bsShortNameTextbox.Text.Trim <> bsOffSystemTestListView.Items(OriginalSelectedIndex).SubItems(2).Text) OrElse _
                                               (CDbl(bsDecimalsUpDown.Value) <> CDbl(bsOffSystemTestListView.Items(OriginalSelectedIndex).SubItems(4).Text)) OrElse _
                                               (bsResultTypeComboBox.SelectedValue.ToString <> bsOffSystemTestListView.Items(OriginalSelectedIndex).SubItems(5).Text) OrElse _
                                               (bsSampleTypeComboBox.SelectedValue.ToString <> SelectedTestSampleTypesDS.tparOffSystemTestSamples(0).SampleType)

                        If (Not pendingToSaveChanges) Then
                            'If the Test continues as Quantitative, verify if the MeasureUnit has been changed
                            If (bsResultTypeComboBox.SelectedValue.ToString = bsOffSystemTestListView.Items(OriginalSelectedIndex).SubItems(5).Text) And _
                               (bsResultTypeComboBox.SelectedValue.ToString = "QUANTIVE") Then
                                pendingToSaveChanges = (bsUnitComboBox.SelectedValue.ToString <> bsOffSystemTestListView.Items(OriginalSelectedIndex).SubItems(3).Text)
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PendingChangesVerification", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PendingChangesVerification", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return pendingToSaveChanges
    End Function

    ''' <summary>
    ''' Method incharge to load the image for each button
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 25/11/2010
    ''' Modified by: SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control  
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'NEW Button
            auxIconName = GetIconName("ADD")
            If auxIconName <> "" Then
                bsNewButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'DELETE Button
            auxIconName = GetIconName("REMOVE")
            If auxIconName <> "" Then
                bsDeleteButton.Image = Image.FromFile(iconPath & auxIconName)
                bsTestRefRanges.DeleteButtonImage = Image.FromFile(iconPath & auxIconName)
            End If

            'EDIT Button
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                bsEditButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = Image.FromFile(iconPath & auxIconName)
            End If
            'JB 30/08/2012 - Hide Print button
            bsPrintButton.Visible = False

            'CUSTOM SORT Button AG 04/09/2014 - BA-1869
            auxIconName = GetIconName("ORDER_TESTS")
            If (auxIconName <> "") Then
                BsCustomOrderButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'SAVE Button
            auxIconName = GetIconName("SAVE")
            If (String.Compare(auxIconName, "", False) <> 0) Then
                bsSaveButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("UNDO")
            If (String.Compare(auxIconName, "", False) <> 0) Then
                bsCancelButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CLOSE Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to QUERY MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 25/11/2010
    ''' Modified by: SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control  
    ''' </remarks>
    Private Sub QueryModeScreenStatus()
        Try
            bsEditButton.Enabled = True
            '            bsPrintButton.Enabled = True DL 11/05/2012
            bsDeleteButton.Enabled = True
            BsCustomOrderButton.Enabled = True 'AG 04/09/2014 - BA-1869

            bsFullNameTextbox.Enabled = False
            bsFullNameTextbox.BackColor = SystemColors.MenuBar

            bsShortNameTextbox.Enabled = False
            bsShortNameTextbox.BackColor = SystemColors.MenuBar

            bsSampleTypeComboBox.Enabled = False
            bsSampleTypeComboBox.BackColor = SystemColors.MenuBar

            bsUnitComboBox.Enabled = False
            bsUnitComboBox.BackColor = SystemColors.MenuBar

            bsDecimalsUpDown.Enabled = False
            bsDecimalsUpDown.BackColor = SystemColors.MenuBar

            bsResultTypeComboBox.Enabled = False
            bsResultTypeComboBox.BackColor = SystemColors.MenuBar

            'bsDefaultValueComboBox.Enabled = False
            'bsDefaultValueComboBox.BackColor = SystemColors.MenuBar

            'bsDefaultValueUpDown.Enabled = False
            'bsDefaultValueUpDown.BackColor = SystemColors.MenuBar

            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False

            bsTestRefRanges.isEditing = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".QueryModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QueryModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get data of the selected OffSystem Test, fill the correspondent variables and controls and set the screen status 
    ''' to Read-only Mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 25/11/2010 
    '''              SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control 
    ''' </remarks>
    Private Sub QueryOffSystemTest()
        Dim setScreenToQuery As Boolean = False

        Try
            If (bsOffSystemTestListView.SelectedIndices(0) <> OriginalSelectedIndex) Then
                If (PendingChangesVerification()) Then
                    If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                        EditionMode = False
                        SelectedTestRefRangesDS.Clear()
                        setScreenToQuery = True
                    Else
                        If (OriginalSelectedIndex <> -1) Then
                            bsOffSystemTestListView.SelectedItems.Clear()

                            'Return focus to the Off-System Test that has been edited
                            bsOffSystemTestListView.Items(OriginalSelectedIndex).Selected = True
                            bsOffSystemTestListView.Select()
                        End If
                    End If
                Else
                    SelectedTestRefRangesDS.Clear()
                    setScreenToQuery = True
                End If

                If (setScreenToQuery) Then
                    'Load screen fields with all data of the selected OFF-SYSTEM Test
                    Dim inUseOffSystemTest As Boolean = LoadDataOfOffSystemTest()

                    'Get the SampleType the OFF-SYSTEM Test is using currently and select it in the ComboBox of Sample Types
                    GetOffSystemTestSamples(SelectedOffSystemTestID)
                    bsSampleTypeComboBox.SelectedValue = SelectedTestSampleTypesDS.tparOffSystemTestSamples.First.SampleType

                    'Get the Reference Ranges defined for the OFF-SYSTEM Test and shown them
                    GetRefRanges(SelectedOffSystemTestID)
                    LoadRefRangesData()

                    If (Not inUseOffSystemTest) Then
                        'Set screen status to Query Mode
                        QueryModeScreenStatus()
                    Else
                        'Set screen status to Read Only Mode 
                        ReadOnlyModeScreenStatus()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".QueryOffSystemTest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QueryOffSystemTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Screen display in query mode when the KeyUp, KeyDown, PageDown or PageUp key is pressed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 25/11/2010 
    ''' Modified by: TR 13/10/2011 - Do not validate the pressed key, just query the selected OffSystem Test
    ''' </remarks>
    Private Sub QueryOffSystemTestByMoveUpDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        Try
            QueryOffSystemTest()

            'TR 13/10/2011 -Commented
            'Select Case e.KeyCode
            '    Case Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp
            '        QueryOffSystemTest()
            'End Select
            'TR 13/10/2011 end Commented.
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryOffSystemTestByMoveUpDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryOffSystemTestByMoveUpDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to READ ONLY MODE (for InUse OFF-SYSTEM Tests)
    ''' </summary>
    ''' <remarks>
    ''' Modified by:  DL 25/11/2010
    ''' </remarks>
    Private Sub ReadOnlyModeScreenStatus()
        Try
            'Set all controls to QUERY MODE
            EditionMode = False
            QueryModeScreenStatus()

            'Disable all buttons that cannot be used in Read Only Mode
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = True 'AG 04/09/2014 - BA-1869
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReadOnlyModeScreenStatus " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ReadOnlyModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save data of the added/updated OFF-SYSTEM Test
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 26/11/2010
    ''' Modified by: SA 03/01/2011 - New function SaveOffSystemTest to save all data of the OFF-SYSTEM Test; added 
    '''                              validation of affected dependencies when the SampleType has been changed
    ''' </remarks>
    Private Sub SaveChanges_OLD()
        Try
            Dim continueSaving As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myDependeciesElementsDS As New DependenciesElementsDS

            'Verify if the OFF-SYSTEM Test can be saved
            If (ValidateSavingConditions()) Then
                'If the SampleType has been changed, verify if there are affected Profiles
                If (SelectedOffSystemTestID > 0 AndAlso String.Compare(bsSampleTypeComboBox.SelectedValue.ToString, SelectedTestSampleTypesDS.tparOffSystemTestSamples(0).SampleType, False) <> 0) Then
                    'Load the selected OFF-SYSTEM Test in a typed DataSet OffSystemTestsDS
                    Dim offSytemTestDataDS As New OffSystemTestsDS
                    Dim offSystemTestRow As OffSystemTestsDS.tparOffSystemTestsRow

                    offSystemTestRow = offSytemTestDataDS.tparOffSystemTests.NewtparOffSystemTestsRow
                    offSystemTestRow.OffSystemTestID = SelectedOffSystemTestID
                    offSystemTestRow.Name = OriginalOffSystemTestName
                    offSytemTestDataDS.tparOffSystemTests.Rows.Add(offSystemTestRow)

                    myGlobalDataTO = ValidateDependencies(offSytemTestDataDS)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myDependeciesElementsDS = DirectCast(myGlobalDataTO.SetDatos, DependenciesElementsDS)

                        If (myDependeciesElementsDS.DependenciesElements.Count > 0) Then
                            Using AffectedElement As New IWarningAfectedElements
                                AffectedElement.AffectedElements = myDependeciesElementsDS
                                AffectedElement.ShowDialog()

                                continueSaving = (AffectedElement.DialogResult = Windows.Forms.DialogResult.OK)
                            End Using
                        Else
                            continueSaving = True
                        End If
                    Else
                        'Error getting the list of affected elements; show it
                        ShowMessage(Name & ".SaveChanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                Else
                    continueSaving = True
                End If
            End If

            If (continueSaving) Then
                'Save the OFF-SYSTEM Test..
                If (SaveOffSystemTest(myDependeciesElementsDS)) Then
                    If (SelectedOffSystemTestID > 0) Then
                        'Refresh the OFF_SYSTEM Tests List and show the Test in Query Mode
                        EditionMode = False
                        LoadOffSystemTestList()
                        QueryModeScreenStatus()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Save data of the added/updated OFF-SYSTEM Test
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 17/10/2012
    ''' Modified by: WE 21/11/2014 - RQ00035C (BA-1867): beside Profiles extend with Calculated Tests as possible affected elements.
    ''' </remarks>
    Private Sub SaveChanges_NEW()
        Try
            Dim continueSaving As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myDependeciesElementsDS As New DependenciesElementsDS

            'Verify if the OFF-SYSTEM Test can be saved
            If (ValidateSavingConditions()) Then
                ' If the SampleType has been changed, verify if there are affected Profiles and Calculated Tests to delete.
                If (SelectedOffSystemTestID > 0 AndAlso bsSampleTypeComboBox.SelectedValue.ToString <> SelectedTestSampleTypesDS.tparOffSystemTestSamples(0).SampleType) Then
                    'Load the selected OFF-SYSTEM Test in a typed DataSet OffSystemTestsDS
                    Dim offSytemTestDataDS As New OffSystemTestsDS
                    Dim offSystemTestRow As OffSystemTestsDS.tparOffSystemTestsRow

                    offSystemTestRow = offSytemTestDataDS.tparOffSystemTests.NewtparOffSystemTestsRow
                    offSystemTestRow.OffSystemTestID = SelectedOffSystemTestID
                    offSystemTestRow.Name = OriginalOffSystemTestName
                    offSytemTestDataDS.tparOffSystemTests.Rows.Add(offSystemTestRow)

                    myGlobalDataTO = ValidateDependencies(offSytemTestDataDS)
                    If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myDependeciesElementsDS = DirectCast(myGlobalDataTO.SetDatos, DependenciesElementsDS)

                        If (myDependeciesElementsDS.DependenciesElements.Count > 0) Then
                            Using AffectedElement As New IWarningAfectedElements
                                AffectedElement.AffectedElements = myDependeciesElementsDS
                                AffectedElement.ShowDialog()

                                continueSaving = (AffectedElement.DialogResult = Windows.Forms.DialogResult.OK)
                            End Using
                        Else
                            continueSaving = True
                        End If
                    Else
                        'Error getting the list of affected elements; show it
                        ShowMessage(Name & ".SaveChanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                Else
                    continueSaving = True
                End If
            End If

            'Create ReferenceRanges DataSets needed for the Save method
            'Dim newRefRangesDS As New TestRefRangesDS
            'Dim updatedRefRangesDS As New TestRefRangesDS
            'Dim deletedRefRangesDS As New TestRefRangesDS

            If (continueSaving) Then
                'Get from the User Control of Reference Ranges the defined ones
                SelectedTestRefRangesDS = DirectCast(bsTestRefRanges.DefinedTestRangesDS, TestRefRangesDS) 'bsTestRefRanges.DefinedTestRangesDS

                'Dim myGlobalBase As New GlobalBase
                ''Dim myTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)
                'Dim myTestRefRanges As List(Of TestRefRangesDS.tparTestRefRangesRow)

                ''CREATE: Get all added Reference Ranges
                'myTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                '                   Where a.IsNew = True _
                '                   Select a).ToList()

                'For i As Integer = 0 To myTestRefRanges.Count - 1
                '    myTestRefRanges(0).BeginEdit()
                '    myTestRefRanges(0).TS_User = GlobalBase.GetSessionInfo.UserName
                '    myTestRefRanges(0).TS_DateTime = Now
                '    myTestRefRanges(0).EndEdit()

                '    newRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                'Next

                ''UPDATE: Get all updated Reference Ranges
                'myTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                '                   Where a.IsNew = False _
                '                   And a.IsDeleted = False _
                '                   Select a).ToList()

                'For i As Integer = 0 To myTestRefRanges.Count - 1
                '    myTestRefRanges(0).BeginEdit()
                '    myTestRefRanges(0).TS_User = GlobalBase.GetSessionInfo.UserName
                '    myTestRefRanges(0).TS_DateTime = Now
                '    myTestRefRanges(0).EndEdit()

                '    updatedRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                'Next

                ''DELETE: Get all Reference Ranges marked to delete
                'myTestRefRanges = (From a In SelectedTestRefRangesDS.tparTestRefRanges _
                '                   Where a.IsDeleted _
                '                   Select a).ToList()

                'For i As Integer = 0 To myTestRefRanges.Count - 1
                '    deletedRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                'Next


                'Save the OFF-SYSTEM Test..
                If (SaveOffSystemTest(myDependeciesElementsDS)) Then
                    If (SelectedOffSystemTestID > 0) Then
                        'Refresh the OFF_SYSTEM Tests List and show the Test in Query Mode
                        EditionMode = False
                        LoadOffSystemTestList()
                        QueryModeScreenStatus()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save (add/update) all data of the OFF-SYSTEM Test: definition and Reference Ranges 
    ''' </summary>
    ''' <param name="pAffectedElementsDS">Typed DataSet DependenciesElementsDS containing the list of 
    '''                                   Profiles that will be changed/deleted when the SampleType of 
    '''                                   an existing OFF-SYSTEM Test has been changed</param>
    ''' <returns>True if the OFF-SYSTEM Test was correctly added/updated; otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  SA 03/01/2011
    ''' Modified by: TR 04/09/2012 - Use value of global variables UpdateHistoryRequired and CloseHistoryRequired as value of 
    '''                              new optional parameters defined for function Modify in OffSystemTestsDelegate
    '''              TR 05/09/2012 - Set to false value of global variable UpdateHistoryRequired once the saving has been executed
    '''              SA 17/09/2012 - Changed call to function Modify in OffSystemTestsDelegate: global variable CloseHistoryRequired 
    '''                              and the corresponding optional parameter have been deleted because they are not needed
    ''' </remarks>
    Private Function SaveOffSystemTest(ByVal pAffectedElementsDS As DependenciesElementsDS) As Boolean
        Dim savingOK As Boolean = False
        Try
            'Gets from the Session the Username of the connected User
            Dim currentSession As New ApplicationSessionManager
            Dim currentUser As String = GlobalBase.GetSessionInfo().UserName

            'Fill OffSystem Test basic data
            Dim offSystemTestData As New OffSystemTestsDS
            Dim offSystemTestRow As OffSystemTestsDS.tparOffSystemTestsRow

            offSystemTestRow = offSystemTestData.tparOffSystemTests.NewtparOffSystemTestsRow()
            With offSystemTestRow
                .OffSystemTestID = SelectedOffSystemTestID
                .Name = bsFullNameTextbox.Text.Trim
                .ShortName = bsShortNameTextbox.Text.Trim
                .Decimals = bsDecimalsUpDown.Value
                If (bsUnitComboBox.SelectedIndex <> -1) Then .Units = bsUnitComboBox.SelectedValue.ToString.Trim
                .ResultType = bsResultTypeComboBox.SelectedValue.ToString.Trim
                .InUse = False
                .TS_User = currentUser
                .TS_DateTime = Now
            End With
            offSystemTestData.tparOffSystemTests.Rows.Add(offSystemTestRow)

            'Fill OffSystem Test data for the selected Sample Type
            Dim offSystemTestSampleData As New OffSystemTestSamplesDS
            Dim offSystemTestSampleRow As OffSystemTestSamplesDS.tparOffSystemTestSamplesRow

            offSystemTestSampleRow = offSystemTestSampleData.tparOffSystemTestSamples.NewtparOffSystemTestSamplesRow()
            With offSystemTestSampleRow
                .OffSystemTestID = SelectedOffSystemTestID
                .SampleType = bsSampleTypeComboBox.SelectedValue.ToString.Trim
                .SetDefaultValueNull()

                'If Reference Ranges have been defined, get value of the selected Range Type
                Dim selectedRangeType As String = bsTestRefRanges.ActiveRangeType
                If (selectedRangeType = String.Empty Or bsResultTypeComboBox.SelectedValue.ToString = "QUALTIVE") Then
                    .SetActiveRangeTypeNull()
                Else
                    .ActiveRangeType = selectedRangeType
                End If

                .TS_User = currentUser
                .TS_DateTime = Now
            End With
            offSystemTestSampleData.tparOffSystemTestSamples.AddtparOffSystemTestSamplesRow(offSystemTestSampleRow)

            'Get from the User Control of Reference Ranges the defined ones
            SelectedTestRefRangesDS = DirectCast(bsTestRefRanges.DefinedTestRangesDS, TestRefRangesDS)

            'Add/Update the OFF-SYSTEM Test
            Dim returnedData As New GlobalDataTO
            Dim offSystemTestToSave As New OffSystemTestsDelegate
            If (SelectedOffSystemTestID = 0) Then
                'Insert a new OFF-SYSTEM Test
                returnedData = offSystemTestToSave.Add(Nothing, offSystemTestData, offSystemTestSampleData, SelectedTestRefRangesDS)
            Else
                'Modify values of an existing OFF-SYSTEM Test
                returnedData = offSystemTestToSave.Modify(Nothing, offSystemTestData, offSystemTestSampleData, _
                                                          SelectedTestRefRangesDS, pAffectedElementsDS, UpdateHistoryRequired)
            End If

            If (Not returnedData.HasError) Then
                savingOK = True
                UpdateHistoryRequired = False

                If (SelectedOffSystemTestID = 0) Then
                    'Show data of the added OFF-SYSTEM Test in Read Only Mode
                    SelectedOffSystemTestID = DirectCast(returnedData.SetDatos, OffSystemTestsDS).tparOffSystemTests(0).OffSystemTestID
                End If
            Else
                'Error adding or updating the OFF-SYSTEM Test, show error message
                If (String.Compare(returnedData.ErrorCode, GlobalEnumerates.Messages.DUPLICATED_TEST_NAME.ToString, False) = 0) Then
                    bsScreenErrorProvider.SetError(bsFullNameTextbox, GetMessageText(returnedData.ErrorCode))
                    bsFullNameTextbox.Focus()
                ElseIf (returnedData.ErrorCode = GlobalEnumerates.Messages.DUPLICATED_TEST_SHORTNAME.ToString) Then
                    bsScreenErrorProvider.SetError(bsShortNameTextbox, GetMessageText(returnedData.ErrorCode))
                    bsShortNameTextbox.Focus()
                Else
                    ShowMessage(Name & ".SaveOffSystemTest", returnedData.ErrorCode, returnedData.ErrorMessage)
                    bsFullNameTextbox.Focus()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveOffSystemTest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveOffSystemTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return savingOK
    End Function

    ''' <summary>
    ''' Load all data needed for the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 25/11/2010
    ''' Modified by: RH 15/12/2010 - Added call to new ResetBorder to avoid flickering when opening
    '''              SA 24/12/2010 - Changes due to new implementation of Reference Ranges Control  
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Get Icons for graphical buttons
            PrepareButtons()

            'Load ComboBoxes and fix limits of Numeric UpDown controls
            LoadMeasureUnits()
            'LoadDefaultValues()
            LoadSampleTypesList()
            LoadResultTypeCombo()
            LoadDecimalsLimit()

            'Load the list of existing Off-System Tests
            InitializeOffSystemTestList(currentLanguage)

            'Load the multilanguage texts for all Screen Labels
            GetScreenLabels(currentLanguage)

            'Set Screen Status to INITIAL MODE
            InitialModeScreenStatus()

            If (bsOffSystemTestListView.Items.Count > 0) Then
                'Select the first OFF-SYSTEM Test in the list
                bsOffSystemTestListView.Items(0).Selected = True
                QueryOffSystemTest()
            End If

            'To avoid flickering when opening
            ResetBorder()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Enable or disable functionallity by user level.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 23/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    bsNewButton.Enabled = False
                    bsEditButton.Enabled = False
                    bsDeleteButton.Enabled = False
                    BsCustomOrderButton.Enabled = True 'AG 04/09/2014 - BA-1869
                    'bsPrintButton.Enabled = False DL 11/05/2012
                    Exit Select
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if there are affected elements - Profiles that have to be changed and/or Calculated Tests that
    ''' have to be deleted due to the deletion of the selected OFF-SYSTEM Test(s) or change of the SampleType
    ''' of an updated OFF-SYSTEM Test.
    ''' </summary>
    ''' <param name="pOffSystemTestDataDS">Typed DataSet OffSystemTestsDS containing the list of OFF-SYSTEM Test(s)
    '''                                    selected to be deleted, or containing the OFF-SYSTEM Test to be updated.</param>
    ''' <returns>GlobalDataTO containing a typed DataSet DependenciesElementsDS with the list of affected elements</returns>
    ''' <remarks>
    ''' Created by:  SA 04/01/2011
    ''' Modified by: WE 21/11/2014 - RQ00035C (BA-1867): Updated Summary and Parameter description (added Calculated Tests as possible cause).
    ''' </remarks>
    Private Function ValidateDependencies(ByVal pOffSystemTestDataDS As OffSystemTestsDS) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            Dim myOffSystemTestDelegate As New OffSystemTestsDelegate
            myGlobalDataTO = myOffSystemTestDelegate.ValidatedDependencies(pOffSystemTestDataDS)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateDependencies", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateDependencies", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' When Reference Ranges have been defined for the selected OFF-SYSTEM Test; validate they are correct
    ''' before saving the OFF-SYSTEM Test
    ''' </summary>
    ''' <returns>True if not Ranges have been defined or if the defined ones are correct; otherwise, it
    '''          returns False</returns>
    ''' <remarks>
    ''' Created by: SA 03/01/2011 
    ''' </remarks>
    Private Function ValidateRefRanges() As Boolean
        Try
            If (String.Compare(bsTestRefRanges.ActiveRangeType, String.Empty, False) <> 0) Then
                bsTestRefRanges.ValidateRefRangesLimits(False, True)
                If (bsTestRefRanges.ValidationError) Then Return False
            End If
            Return True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateRefRanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateRefRanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Validate that all mandatory fields have been informed with a correct value
    ''' </summary>
    ''' <returns>True if all fields have a correct value; otherwise, False</returns>
    ''' <remarks>
    ''' Modified by: DL 26/11/2010
    '''              SA 04/01/2010 - Added validation of empty SampleType
    ''' </remarks>
    Private Function ValidateSavingConditions() As Boolean
        Dim fieldsOK As Boolean = True
        Try
            Dim setFocusTo As Integer = -1

            bsScreenErrorProvider.Clear()
            If (bsFullNameTextbox.TextLength = 0) Then
                'Validate the Long Name is not empty; otherwise inform the missing data
                bsScreenErrorProvider.SetError(bsFullNameTextbox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                setFocusTo = 0
            End If

            If (bsShortNameTextbox.TextLength = 0) Then
                'Validate the Short Name is not empty; otherwise inform the missing data
                bsScreenErrorProvider.SetError(bsShortNameTextbox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                If (setFocusTo = -1) Then setFocusTo = 1
            End If

            If (bsSampleTypeComboBox.SelectedIndex = -1) Then
                'Validate the Sample Type is not empty; otherwise inform the missing data
                bsScreenErrorProvider.SetError(bsSampleTypeComboBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                If (setFocusTo = -1) Then setFocusTo = 2
            End If

            If (bsResultTypeComboBox.SelectedIndex = -1) Then
                'Validate the Short Name is not empty; otherwise inform the missing data
                bsScreenErrorProvider.SetError(bsResultTypeComboBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                If (setFocusTo = -1) Then setFocusTo = 3
            Else
                'DL 27/01/2011 - For QUANTITATIVE Tests, validate field MeasureUnit is informed
                If (String.Compare(bsResultTypeComboBox.SelectedValue.ToString, "QUANTIVE", False) = 0) Then
                    If (bsUnitComboBox.SelectedIndex = -1) Then
                        'Validate the Measure Unit is not empty; otherwise inform the missing data
                        bsScreenErrorProvider.SetError(bsUnitComboBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                        If (setFocusTo = -1) Then setFocusTo = 4
                    End If
                    'Else
                    '    If (Not ValidateRefRanges()) Then
                    '        If (setFocusTo = -1) Then setFocusTo = 5
                    '    End If
                End If
                If (Not ValidateRefRanges()) Then
                    If (setFocusTo = -1) Then setFocusTo = 5
                End If
            End If

            'Select the proper field to put the focus
            If (setFocusTo >= 0) Then
                fieldsOK = False

                If (setFocusTo = 0) Then
                    bsFullNameTextbox.Focus()
                ElseIf (setFocusTo = 1) Then
                    bsShortNameTextbox.Focus()
                ElseIf (setFocusTo = 2) Then
                    bsSampleTypeComboBox.Focus()
                ElseIf (setFocusTo = 3) Then
                    bsResultTypeComboBox.Focus()
                ElseIf (setFocusTo = 4) Then
                    bsUnitComboBox.Focus()
                ElseIf (setFocusTo = 5) Then
                    Me.bsOffSystemTabControl.SelectTab(0)
                End If
            Else
                'All mandatory fields are informed, verify the informed Name and ShortName are unique
                Dim resultData As New GlobalDataTO
                Dim myOffSystemTestDelegate As New OffSystemTestsDelegate

                resultData = myOffSystemTestDelegate.ExistsOffSystemTest(Nothing, bsFullNameTextbox.Text, "FNAME", SelectedOffSystemTestID)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    Dim duplicatedTest As Boolean = False
                    duplicatedTest = CType(resultData.SetDatos, Boolean)

                    If (duplicatedTest) Then
                        bsScreenErrorProvider.SetError(bsFullNameTextbox, GetMessageText(GlobalEnumerates.Messages.DUPLICATED_TEST_NAME.ToString))
                        setFocusTo = 0
                    End If
                End If

                resultData = myOffSystemTestDelegate.ExistsOffSystemTest(Nothing, bsShortNameTextbox.Text, "NAME", SelectedOffSystemTestID)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    Dim duplicatedTest As Boolean = False
                    duplicatedTest = CType(resultData.SetDatos, Boolean)

                    If (duplicatedTest) Then
                        bsScreenErrorProvider.SetError(bsShortNameTextbox, GetMessageText(GlobalEnumerates.Messages.DUPLICATED_TEST_SHORTNAME.ToString))
                        If (setFocusTo = -1) Then setFocusTo = 1
                    End If
                End If

                If (setFocusTo >= 0) Then
                    fieldsOK = False

                    If (setFocusTo = 0) Then
                        bsFullNameTextbox.Focus()
                    ElseIf (setFocusTo = 1) Then
                        bsShortNameTextbox.Focus()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateSavingConditions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateSavingConditions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return fieldsOK
    End Function

#End Region

#Region "Events"
    '*************************'
    '* EVENTS FOR THE SCREEN *'
    '*************************'
    ''' <summary>
    ''' Close the screen when key ESC is pressed
    ''' </summary>
    Private Sub ProgOffSystemTest_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (bsCancelButton.Enabled) Then
                    'RH 04/07/2011 Escape key should do exactly the same operations as bsCancelButton_Click()
                    bsCancelButton.PerformClick()
                Else
                    'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                    bsExitButton.PerformClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ProgOffSystemTest_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgOffSystemTest_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Form initialization when loading: set the screen status to INITIAL MODE
    ''' </summary>
    Private Sub ProgOffSystemTest_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'TR 20/04/2012 - Get the Level of the Current User
            Dim MyGlobalBase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo().UserLevel

            ScreenLoad()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ProgOffSystemTest_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgOffSystemTest_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Enable/disable screen functionalities according the level of the current User
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 23/04/2012 
    ''' </remarks>
    Private Sub IProgOffSystemTest_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            ScreenStatusByUserLevel()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IProgOffSystemTest_Shown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IProgOffSystemTest_Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '*******************************************'
    '* EVENTS FOR LISTVIEW OF OFF-SYSTEM TESTS *'
    '*******************************************'
    ''' <summary>
    ''' Show data of the selected OFF-SYSTEM Test in READ-ONLY MODE
    ''' </summary>
    Private Sub bsOffSystemTestListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsOffSystemTestListView.Click
        Try
            If (bsOffSystemTestListView.SelectedItems.Count = 1) Then
                QueryOffSystemTest()
            Else
                Dim bEnabled As Boolean = True
                For Each mySelectedItem As ListViewItem In bsOffSystemTestListView.SelectedItems
                    'If there is an item InUse
                    If (CBool(mySelectedItem.SubItems(6).Text)) Then
                        bEnabled = False
                        Exit For
                    End If
                Next mySelectedItem

                'Set screen status to the case when there are several selected OFF-SYSTEM Tests
                InitialModeScreenStatus(False)

                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = bEnabled
                BsCustomOrderButton.Enabled = True 'AG 04/09/2014 - BA-1869
            End If

            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsOffSystemTestListView_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsOffSystemTestListView_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Sort ascending/descending the list of offsystem Tests when clicking in the list header
    ''' </summary>
    Private Sub bsOffSystemTestListView_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles bsOffSystemTestListView.ColumnClick
        Try
            Select Case bsOffSystemTestListView.Sorting
                Case SortOrder.None
                    bsOffSystemTestListView.Sorting = SortOrder.Ascending
                    Exit Select
                Case SortOrder.Ascending
                    bsOffSystemTestListView.Sorting = SortOrder.Descending
                    Exit Select
                Case SortOrder.Descending
                    bsOffSystemTestListView.Sorting = SortOrder.Ascending
                    Exit Select
                Case Else
                    Exit Select
            End Select
            bsOffSystemTestListView.Sort()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsOffSystemTestListView_ColumnClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsOffSystemTestListView_ColumnClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Don't allow Users to resize the visible ListView column
    ''' </summary>
    Private Sub bsOffSystemTestListView_ColumnWidthChanging(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles bsOffSystemTestListView.ColumnWidthChanging
        Try
            e.Cancel = True
            If (e.ColumnIndex = 0) Then
                e.NewWidth = 210
            Else
                e.NewWidth = 0
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsOffSystemTestListView_ColumnWidthChanging", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsOffSystemTestListView_ColumnWidthChanging", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load data of the selected OFF-SYSTEM Test and set the screen status to Edit Mode
    ''' </summary>
    Private Sub bsOffSystemTestListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsOffSystemTestListView.DoubleClick
        Try
            If Not String.Compare(CurrentUserLevel, "OPERATOR", False) = 0 Then 'TR 23/04/2012 -Validate user level.
                EditOffSystemTestByDoubleClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsOffSystemTestListView_DoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsOffSystemTestListView_DoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Allow consultation of elements in OFF-SYSTEM Test list using the keyboard
    ''' </summary>
    Private Sub bsOffSystemTestListView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsOffSystemTestListView.KeyUp
        Try
            If (bsOffSystemTestListView.SelectedItems.Count = 1) Then
                QueryOffSystemTestByMoveUpDown(e)
            Else
                Dim bEnabled As Boolean = True
                For Each mySelectedItem As ListViewItem In bsOffSystemTestListView.SelectedItems
                    'If there is an item InUse
                    If (CBool(mySelectedItem.SubItems(6).Text)) Then
                        bEnabled = False
                        Exit For
                    End If
                Next

                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = bEnabled
                BsCustomOrderButton.Enabled = True 'AG 04/09/2014 - BA-1869
            End If

            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsOffSystemTestListView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsOffSystemTestListView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Allow deletion of OFF-SYSTEM Tests using the SUPR key; allow set the edition Mode for the selected OFF-SYSTEM Test
    ''' when Enter is pressed
    ''' </summary>
    Private Sub bsOffSystemTestListView_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsOffSystemTestListView.PreviewKeyDown
        Try
            If (e.KeyCode = Keys.Delete And bsDeleteButton.Enabled) Then
                DeleteOffSystemTests()

            ElseIf (e.KeyCode = Keys.Enter And bsEditButton.Enabled) Then
                EditOffSystemTestByButtonClick()
            End If

            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsOffSystemTestListView_PreviewKeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsOffSystemTestListView_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*************************************'
    '* EVENTS FOR FIELDS IN DETAILS AREA *'
    '*************************************'
    ''' <summary>
    ''' Clean the Error Provider when a value is selected in the ComboBox; for Measure Units ComboBox:
    ''' inform the selected unit in the Reference Ranges Control
    ''' </summary>
    Private Sub bsComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSampleTypeComboBox.SelectedIndexChanged, _
                                                                                                                    bsUnitComboBox.SelectedIndexChanged, _
                                                                                                                    bsResultTypeComboBox.SelectedIndexChanged
        Try
            Dim myComboBox As New ComboBox
            myComboBox = CType(sender, ComboBox)

            If (myComboBox.SelectedIndex <> -1) Then
                If (String.Compare(bsScreenErrorProvider.GetError(myComboBox), "", False) <> 0) Then
                    bsScreenErrorProvider.SetError(myComboBox, String.Empty)
                End If

                If (myComboBox Is bsUnitComboBox) Then
                    'Show the new selected Measure Unit in the area of Reference Ranges
                    bsTestRefRanges.MeasureUnit = bsUnitComboBox.Text
                End If
            End If

            'TR 04/09/2012 - When the event is raised for whatever of the ComboBoxes (SampleType, MeasureUnit or ResultType)
            '                flag UpdateHistoryRequired is set to TRUE
            If (EditionMode) Then UpdateHistoryRequired = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsComboBox_SelectedIndexChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsComboBox_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Change BackColor of the ComboBox according if there is an item selected or not; Reference Ranges Control
    ''' will be available only in Edit Mode and when SampleType and ResultType have been informed and this last
    ''' one has QUANTITATIVE as value.
    ''' </summary>
    Private Sub bsComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSampleTypeComboBox.SelectionChangeCommitted, _
                                                                                                                 bsUnitComboBox.SelectionChangeCommitted, _
                                                                                                                 bsResultTypeComboBox.SelectionChangeCommitted
        Try
            Dim myComboBox As New ComboBox
            myComboBox = CType(sender, ComboBox)

            If (myComboBox.SelectedIndex = -1) Then
                myComboBox.BackColor = Color.Khaki
            Else
                myComboBox.BackColor = Color.White
            End If

            If (myComboBox Is bsSampleTypeComboBox) Then
                If (myComboBox.SelectedIndex = -1) Then
                    bsTestRefRanges.isEditing = False
                Else
                    bsTestRefRanges.SampleType = bsSampleTypeComboBox.SelectedValue.ToString
                    bsTestRefRanges.isEditing = (bsResultTypeComboBox.SelectedIndex <> -1 AndAlso bsResultTypeComboBox.SelectedValue.ToString = "QUANTIVE") And _
                                                 EditionMode
                End If

            ElseIf (myComboBox Is bsResultTypeComboBox) Then
                'When the Result Type is changed to QUALITATIVE, the Reference Ranges are emptied and disabled
                Select Case (bsResultTypeComboBox.SelectedValue.ToString)
                    Case "QUALTIVE"
                        If (bsTestRefRanges.ActiveRangeType <> String.Empty) Then
                            If (ShowMessage(Name & ".bsComboBox_SelectionChangeCommitted", GlobalEnumerates.Messages.DELETE_REFERENCE_RANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                                bsTestRefRanges.ClearData()
                                bsTestRefRanges.isEditing = False

                                'DL 27/01/2011 - For Qualitative Results: fields MeasureUnit and Decimals are empty and disabled
                                bsUnitComboBox.SelectedValue = -1
                                If (bsScreenErrorProvider.GetError(bsUnitComboBox) <> "") Then bsScreenErrorProvider.SetError(bsUnitComboBox, String.Empty)

                                bsUnitComboBox.Enabled = False
                                bsUnitComboBox.BackColor = SystemColors.MenuBar

                                bsDecimalsUpDown.Value = 0
                                bsDecimalsUpDown.Enabled = False
                                bsDecimalsUpDown.BackColor = SystemColors.MenuBar
                            Else
                                bsResultTypeComboBox.SelectedValue = "QUANTIVE"
                            End If
                        Else
                            bsTestRefRanges.ClearData()
                            bsTestRefRanges.isEditing = False

                            'DL 27/01/2011 - For Qualitative Results: fields MeasureUnit and Decimals are empty and disabled
                            bsUnitComboBox.SelectedValue = -1
                            If (String.Compare(bsScreenErrorProvider.GetError(bsUnitComboBox), "", False) <> 0) Then bsScreenErrorProvider.SetError(bsUnitComboBox, String.Empty)

                            bsUnitComboBox.Enabled = False
                            bsUnitComboBox.BackColor = SystemColors.MenuBar

                            bsDecimalsUpDown.Value = 0
                            bsDecimalsUpDown.Enabled = False
                            bsDecimalsUpDown.BackColor = SystemColors.MenuBar
                        End If

                    Case "QUANTIVE"
                        If (EditionMode) Then
                            If (String.Compare(bsUnitComboBox.Text, String.Empty, False) <> 0) Then bsTestRefRanges.MeasureUnit = bsUnitComboBox.Text
                            bsTestRefRanges.isEditing = (bsSampleTypeComboBox.SelectedIndex > -1)
                        End If

                        'DL 27/01/2011 - For Quantitative Results: MeasureUnit is enabled and mandatory, and Decimals is enabled
                        bsUnitComboBox.Enabled = True
                        bsUnitComboBox.BackColor = Color.Khaki
                        bsDecimalsUpDown.Enabled = True
                        bsDecimalsUpDown.BackColor = Color.White
                    Case Else
                        bsTestRefRanges.isEditing = False
                End Select
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsComboBox_SelectionChangeCommitted", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsComboBox_SelectionChangeCommitted", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' If value entered as number of decimals is out of the allowed limits, set the minimum or the maximum as value
    ''' </summary>
    Private Sub bsDecimalsUpDown_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsDecimalsUpDown.Validated
        Try
            Dim decimalsNumber As Integer = 0
            If (IsNumeric(bsDecimalsUpDown.Text)) Then decimalsNumber = Convert.ToInt32(bsDecimalsUpDown.Text)

            decimalsNumber = Convert.ToInt32(IIf(decimalsNumber < bsDecimalsUpDown.Minimum, bsDecimalsUpDown.Minimum, decimalsNumber))
            decimalsNumber = Convert.ToInt32(IIf(decimalsNumber > bsDecimalsUpDown.Maximum, bsDecimalsUpDown.Maximum, decimalsNumber))
            bsDecimalsUpDown.Text = decimalsNumber.ToString

            bsTestRefRanges.RefNumDecimals = decimalsNumber
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsDecimalsUpDown_Validated", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDecimalsUpDown_Validated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the number of decimals allowed for the OFF-SYSTEM Test changes, flag UpdateHistoryRequired is set to TRUE
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 04/09/2012
    ''' </remarks>
    Private Sub bsDecimalsUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDecimalsUpDown.ValueChanged
        Try
            If (EditionMode) Then UpdateHistoryRequired = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsDecimalsUpDown_ValueChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDecimalsUpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate characters entered in both TextBoxes used for Names 
    ''' </summary>
    Private Sub bsTextbox_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles bsFullNameTextbox.KeyPress, _
                                                                                                                      bsShortNameTextbox.KeyPress
        Try
            If (ValidateSpecialCharacters(e.KeyChar, "[@#~$%&/()-+><_.:,;!¿?=·ªº'¡|}^{]")) Then e.Handled = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTextbox_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTextbox_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the TextBox showing the error is changed
    ''' </summary>
    Private Sub bsTextbox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsFullNameTextbox.TextChanged, _
                                                                                                   bsShortNameTextbox.TextChanged
        Try
            Dim myTextBox As New TextBox
            myTextBox = CType(sender, TextBox)

            If (myTextBox.TextLength > 0) Then
                If (String.Compare(bsScreenErrorProvider.GetError(myTextBox), "", False) <> 0) Then
                    bsScreenErrorProvider.SetError(myTextBox, String.Empty)
                End If
            End If

            'TR 04/09/2012 - When the event is raised for the field containing the OFF-SYSTEM Test long name, 
            '                flag UpdateHistoryRequired is set to TRUE
            If (EditionMode AndAlso String.Compare(myTextBox.Name, bsFullNameTextbox.Name, False) = 0) Then UpdateHistoryRequired = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTextbox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTextbox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '**********************'
    '* EVENTS FOR BUTTONS *'
    '**********************'
    ''' <summary>
    ''' Set the screen status to ADD MODE
    ''' </summary>
    Private Sub bsNewButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNewButton.Click
        Try
            AddOffSystemTest()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsNewButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsNewButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen status to EDIT MODE for the selected OFF-SYSTEM Test
    ''' </summary>
    Private Sub bsEditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsEditButton.Click
        Try
            'EditOffSystemTestByButtonClick() 'DL 17/10/2012
            EditOffSystemTestByDoubleClick()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsEditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsEditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Executes process of deletion the selected OFF-SYSTEM Tests
    ''' </summary>
    Private Sub bsDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDeleteButton.Click
        Try
            DeleteOffSystemTests()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsDeleteButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDeleteButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Saves data of the added/updated OFF-SYSTEM Test
    ''' </summary>
    Private Sub bsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSaveButton.Click
        Try
            SaveChanges_NEW()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSaveButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSaveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Execute the cancelling of an OFF-SYSTEM Test edition
    ''' </summary>  
    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            CancelOffSystemTestEdition()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen, verifying first if there are changes pending to save
    ''' </summary>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            ExitScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Open the customize order and availability for OFFS tests selection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 04/09/2014 - BA-1869</remarks>
    Private Sub BsCustomOrderButton_Click(sender As Object, e As EventArgs) Handles BsCustomOrderButton.Click
        Try
            'Shown the Positioning Warnings Screen
            Using AuxMe As New ISortingTestsAux()
                AuxMe.openMode = "TESTSELECTION"
                AuxMe.screenID = "OFFS"
                AuxMe.ShowDialog()
            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsCustomOrderButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsCustomOrderButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region


End Class
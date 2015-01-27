Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.PresentationCOM

Public Class UiProgPatientData

#Region "Constructor"
    Public Sub New()
        'This call is required by the Windows Form Designer
        InitializeComponent()
    End Sub
#End Region

#Region "Declarations"
    Private EnableDeleteButton As Boolean = False  'SA 30/11/2012 - Created only for V1, while problem described in BugTracking Num 952 is not solved         

    Private MDIChildHeight As Integer = 654                 'Screen height when it is opened as MDIChild from the Main Menu
    Private PopUpHeight As Integer = 690                    'Screen height when it is opened as a PopUp from WS Preparation
    Private ProcessEvent As Boolean = True                  'To avoid event raising when some controls are initialized
    Private GridInProcess As Boolean = False                '??

    Private GenderListDS As PreloadedMasterDataDS           'DataSource for Genders ComboBox in details area  
    Private FilterGenderListDS As PreloadedMasterDataDS     'DataSource for Genders ComboBox in searching area
    Private AgeUnitsListDS As PreloadedMasterDataDS         'DataSource for Age Units ComboBox in searching area  

    Private EditMode As String = ""                         'To know if the screen is in mode NEW or EDIT
    Private ValidationError As Boolean = False              'To know if at least one field has an error and the Patient cannot be saved
    Private PendingToSaveChanges As Boolean = False         'To store whether the field is been edited by the user

    Private FiltersInformed As Boolean = False              'To know if the list of Patients if filtered by at least one search criteria
    Private EditingPatientData As DataGridViewRow           'To store Patient details before edition 

    'JB 08/11/2012 - Not needed: updated to a DateTime control
    'Private MaskedTextEmpty As String = String.Empty        'To manage the mask used in the DateOfBirth TextBox - RH 23/02/2012

    Private myNewLocation As Point                          'To avoid the screen movement when the screen is opened as a PopUp from WS Preparation
#End Region

#Region "Attributes"
    Private entryModeAttribute As String = "MENU"
    Private patientIDAttribute As String = ""
    Private patientNamesAttribute As String = ""
    Private patientsListAttribute As New List(Of String)
#End Region

#Region "Properties"
    'Indicates if the screen was opened from Menu or from the screen of WS Preparation
    Public WriteOnly Property EntryMode() As String
        Set(ByVal value As String)
            entryModeAttribute = value
        End Set
    End Property

    'To recibe a PatientID to be used as filter; to return the ID of the selected Patient
    'Used only when Property EntryMode is SEARCH
    Public Property PatientID() As String
        Get
            Return patientIDAttribute
        End Get
        Set(ByVal value As String)
            patientIDAttribute = value
        End Set
    End Property

    'To return the First and Last Names of the selected Patient
    'Used only when Property EntryMode is SEARCH
    Public ReadOnly Property PatientNames() As String
        Get
            Return patientNamesAttribute
        End Get
    End Property

    Public WriteOnly Property PatientsList() As List(Of String)
        Set(ByVal value As List(Of String))
            patientsListAttribute = value
        End Set
    End Property
#End Region

#Region "Methods"
    ''' <summary>
    ''' Manages the cancelling of adding/editing a Patient
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010
    ''' Modified by: SA 16/11/2010 - Removed disable of fields by GroupBox
    ''' </remarks>
    Private Sub CancelSearchEdition()
        Try
            bsExitButton.Visible = True
            If (Not PendingToSaveChanges OrElse _
               (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes)) Then
                bsScreenErrorProvider.Clear()

                DataGridViewSelectionChanged()

                bsSearchCriteriaPanel.Visible = False
                bsGridPanel.Visible = True
                bsDetailsPanel.Visible = True

                If (bsPatientListDataGridView.RowCount > 0) Then
                    bsEditButton.Enabled = True
                    bsDeleteButton.Enabled = True And EnableDeleteButton
                    bsPrintButton.Enabled = True
                End If
                bsOpenSearchButton.Enabled = True

                EditingPatientData = CType(LoadSelectedPatientDataFields(), DataGridViewRow)
                UpdateDetailsColors(False)
                bsPatientListDataGridView.Focus()

                PendingToSaveChanges = False
            Else
                PendingToSaveChanges = True
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CancelSearchEdition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CancelSearchEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Add/Update the Patient data in details area and refresh the Patients grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 12/07/2010
    ''' Modified by: SA 16/11/2010 - Removed disable of fields by GroupBox; enabled Clear Filters Button if some
    '''                              filters have been informed. Update also the selected GenderCode in the grid 
    '''              SA 12/01/2011 - Inform property attributes for PatientID and PatientNames after save changes
    '''              TR 26/07/2011 - Enable the Accept button if there is a Patient selected (only when the screen 
    '''                              has been opened from the screen of WS Samples Requests
    ''' </remarks>
    Private Sub ChangesToSave()
        Try
            'Verify all mandatory fields have been informed
            ValidationError = False
            bsScreenErrorProvider.Clear()

            ValidatePatientID()
            ValidateFirstName()
            ValidateLastName()

            ValidateDateOfBirthMaskedTextBox() 'RH 23/02/2012

            If ValidationError Then Return

            If (EditMode = "EDIT") Then
                'Updation of an existing Patient
                Dim row As DataGridViewRow = EditingPatientData
                row.Cells("FirstName").Value = bsDetailsFirstNameTextBox.Text.Trim()
                row.Cells("LastName").Value = bsDetailsLastNameTextBox.Text.Trim()
                row.Cells("GenderCode").Value = bsGenderDetailsComboBox.SelectedValue
                row.Cells("Gender").Value = bsGenderDetailsComboBox.Text

                If (bsDetailsAgeByTextBox.Text <> String.Empty) Then
                    row.Cells("DateOfBirth").Value = bsDateOfBirthMaskedTextBox.DateTime
                    row.Cells("Age").Value = bsDetailsAgeByTextBox.Text
                Else
                    row.Cells("DateOfBirth").Value = String.Empty
                    row.Cells("Age").Value = String.Empty
                End If

                row.Cells("PerformedBy").Value = bsPerformedByTextBox.Text.Trim()
                row.Cells("Comments").Value = bsCommentsTextBox.Text.Trim()

                SaveChanges()
                PendingToSaveChanges = False

                bsEditButton.Enabled = True
                bsDeleteButton.Enabled = True And EnableDeleteButton
                bsPrintButton.Enabled = True

                bsOpenSearchButton.Enabled = True
                bsClearButton.Enabled = FiltersInformed

                patientIDAttribute = bsDetailsPatientIDTextBox.Text
                patientNamesAttribute = bsDetailsFirstNameTextBox.Text & " " & bsDetailsLastNameTextBox.Text

                UpdateDetailsColors(False)
                bsPatientListDataGridView.Focus()
            Else
                'Add new Patient
                If (SaveNewPatient()) Then
                    ProcessEvent = False
                    bsPatientListDataGridView.Rows.Add(1)
                    ProcessEvent = True

                    Dim row As DataGridViewRow = New DataGridViewRow
                    row = bsPatientListDataGridView.Rows(bsPatientListDataGridView.Rows.Count - 1)

                    row.Cells("PatientID").Value = bsDetailsPatientIDTextBox.Text.Trim()
                    row.Cells("FirstName").Value = bsDetailsFirstNameTextBox.Text.Trim()
                    row.Cells("LastName").Value = bsDetailsLastNameTextBox.Text.Trim()
                    row.Cells("GenderCode").Value = bsGenderDetailsComboBox.SelectedValue
                    row.Cells("Gender").Value = bsGenderDetailsComboBox.Text

                    If (bsDetailsAgeByTextBox.Text <> String.Empty) Then
                        row.Cells("DateOfBirth").Value = bsDateOfBirthMaskedTextBox.DateTime
                        row.Cells("Age").Value = bsDetailsAgeByTextBox.Text
                    Else
                        row.Cells("DateOfBirth").Value = String.Empty
                        row.Cells("Age").Value = String.Empty
                    End If

                    row.Cells("PerformedBy").Value = bsPerformedByTextBox.Text.Trim()
                    row.Cells("Comments").Value = bsCommentsTextBox.Text.Trim()

                    bsPatientListDataGridView.MultiSelect = True

                    PendingToSaveChanges = False
                    For i As Integer = (bsPatientListDataGridView.SelectedRows.Count - 1) To 0 Step -1
                        bsPatientListDataGridView.SelectedRows(i).Selected = False
                    Next

                    row.Selected = True
                    bsPatientListDataGridView.FirstDisplayedScrollingRowIndex = bsPatientListDataGridView.Rows.Count - 1

                    bsSearchCriteriaPanel.Visible = False
                    bsGridPanel.Visible = True
                    bsDetailsPanel.Visible = True

                    bsEditButton.Enabled = True
                    bsDeleteButton.Enabled = True And EnableDeleteButton
                    bsPrintButton.Enabled = True

                    bsOpenSearchButton.Enabled = True
                    bsClearButton.Enabled = FiltersInformed

                    patientIDAttribute = bsDetailsPatientIDTextBox.Text
                    patientNamesAttribute = bsDetailsFirstNameTextBox.Text & " " & bsDetailsLastNameTextBox.Text

                    UpdateDetailsColors(False)
                    bsPatientListDataGridView.Focus()
                End If
            End If

            If (bsSelectPatientButton.Visible) AndAlso (bsPatientListDataGridView.RowCount > 0 AndAlso bsPatientListDataGridView.SelectedRows.Count = 1) Then
                bsSelectPatientButton.Enabled = True
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ChangesToSave", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ChangesToSave", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Clears all the Patient Details fields
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 06/21/2010
    ''' </remarks>
    Private Sub ClearPatientDetails()
        Try
            bsDetailsPatientIDTextBox.Text = String.Empty
            bsDetailsFirstNameTextBox.Text = String.Empty
            bsDetailsLastNameTextBox.Text = String.Empty
            bsGenderDetailsComboBox.SelectedIndex = 0

            bsDateOfBirthMaskedTextBox.Text = String.Empty
            bsDetailsAgeByTextBox.Text = String.Empty

            bsPerformedByTextBox.Text = String.Empty
            bsCommentsTextBox.Text = String.Empty

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ClearPatientDetails", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ClearPatientDetails", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the changing of selected Patient
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 16/11/2010 - Removed disable of fields by GroupBox
    ''' </remarks>
    Private Sub DataGridViewSelectionChanged()
        Try
            If (Not ProcessEvent) Then Return
            If (GridInProcess) Then Return

            If (bsPatientListDataGridView.SelectedRows.Count = 0) Then
                ClearPatientDetails()

                bsDeleteButton.Enabled = False
                bsEditButton.Enabled = False
                bsPrintButton.Enabled = False

                patientNamesAttribute = String.Empty
                patientIDAttribute = String.Empty

                EditingPatientData = Nothing
            Else
                If (bsSaveButton.Enabled) Then
                    If (DiscardPendingChanges("CANCEL")) Then
                        EditingPatientData = CType(LoadSelectedPatientDataFields(), DataGridViewRow)
                        PendingToSaveChanges = False
                    Else
                        GridInProcess = True
                        If (EditMode = "EDIT") Then
                            Dim index As Integer = EditingPatientData.Index
                            For Each dgr As DataGridViewRow In bsPatientListDataGridView.Rows
                                dgr.Selected = False
                            Next
                            bsPatientListDataGridView.Rows(index).Selected = True
                        End If
                        GridInProcess = False
                    End If
                Else
                    EditingPatientData = CType(LoadSelectedPatientDataFields(), DataGridViewRow)
                    PendingToSaveChanges = False
                End If
            End If
            UpdateDetailsColors(False)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DataGridViewSelectionChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DataGridViewSelectionChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete the selected Patients
    ''' </summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              RH 22/06/2010 - Finished the logic programming
    '''              SA 19/07/2010 - LoadFilteredPatientList was called once for each selected row; one time is enough
    '''              DL 15/09/2010 - If there is at least an InUse Patient, deletion action is not allowed
    '''              SA 16/11/2010 - Some changes to improve the logic flow
    ''' </remarks>
    Private Sub DeletePatient()
        Dim returnedData As GlobalDataTO
        Try
            If (bsPatientListDataGridView.SelectedRows.Count > 0) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                    'Load all selected Patients in a DataSet
                    Dim myPatientList As New PatientsDS
                    Dim row As PatientsDS.tparPatientsRow

                    For i As Integer = 0 To bsPatientListDataGridView.SelectedRows.Count - 1
                        row = myPatientList.tparPatients.NewtparPatientsRow()
                        row.PatientID = bsPatientListDataGridView.SelectedRows(i).Cells("PatientID").Value.ToString()
                        myPatientList.tparPatients.AddtparPatientsRow(row)
                    Next i

                    'Delete all selected Patients
                    Dim patientsDetails As New PatientDelegate

                    returnedData = patientsDetails.Delete(Nothing, myPatientList)
                    If (Not returnedData.HasError) Then
                        'Reload the Patients Grid and select the first one 
                        LoadFilteredPatientList()
                        If (bsPatientListDataGridView.Rows.Count > 0) Then
                            bsPatientListDataGridView.MultiSelect = True
                            bsPatientListDataGridView.Rows(0).Selected = True
                            bsPatientListDataGridView.FirstDisplayedScrollingRowIndex = 0
                        End If
                        PendingToSaveChanges = False
                        UpdateDetailsColors(False)
                    Else
                        'Error deleting the list of selected Patients...; shown it
                        ShowMessage(Me.Name & ".DeletePatient", returnedData.ErrorCode, returnedData.ErrorMessage)
                    End If
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeletePatient", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeletePatient", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Check pending changes while editing if any other action different of Save is executed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 12/07/2010
    ''' </remarks>
    Private Function DiscardPendingChanges(Optional ByVal pSource As String = "") As Boolean
        Try
            If (pSource <> "") Then
                If (Not PendingToSaveChanges) Then
                    patientNamesAttribute = ""
                    patientIDAttribute = ""
                    Return True
                End If
            Else
                If (Not PendingToSaveChanges OrElse _
                   (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes)) Then
                    patientNamesAttribute = ""
                    patientIDAttribute = ""
                    Return True
                End If
            End If
            Return False
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DiscardPendingChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DiscardPendingChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Prepare the Patient's Details area for editing data of the selected Patient
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 02/08/2010
    ''' Modified by: SA 16/11/2010 - Removed disable of fields by GroupBox
    '''              TR 26/07/2011 - Set value of the EditingPatientData to avoid Null Reference Exception
    ''' </remarks>
    Private Sub EditPatientData()
        Try
            If (Not Convert.ToBoolean(bsPatientListDataGridView.SelectedRows(0).Cells("InUse").Value)) Then
                EditMode = "EDIT"
                EditingPatientData = CType(LoadSelectedPatientDataFields(), DataGridViewRow)
                bsSearchCriteriaPanel.Visible = False
                bsClearButton.Enabled = False
                bsOpenSearchButton.Enabled = False
                bsNewButton.Enabled = True
                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
                bsPrintButton.Enabled = False

                bsDetailsFirstNameTextBox.Focus()

                UpdateDetailsColors(True)
            Else
                UpdateDetailsColors(False)
            End If
            PendingToSaveChanges = False
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditPatientData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditPatientData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Determines the availability of the Reset Filters button in Search Criteria area
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 12/07/2010
    ''' </remarks>
    Private Function EnableResetButton() As Boolean
        Dim enable As Boolean = False
        Try
            enable = BsDateBirthRadioButton.Checked
            enable = enable Or BsAgeRadioButton.Checked
            enable = enable Or (bsPatientIDTextbox.Text <> "") _
                            Or (bsLastNameTextBox.Text <> "") _
                            Or (bsFirstNameTextBox.Text <> "") _
                            Or (bsGenderComboBox.Text <> "")
            Me.bsResetButton.Enabled = enable
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EnableResetButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EnableResetButton ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return enable
    End Function

    ''' <summary>
    ''' Enabled/Disabled all controls inside the specified GroupBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 09/07/2010
    ''' </remarks>
    Private Sub EnablingGroupBoxItems(ByRef pGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox, ByVal pStatus As Boolean)
        Try
            For Each ctr As Control In pGroupBox.Controls
                ctr.Enabled = pStatus
            Next
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EnablingGroupBoxItems ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EnablingGroupBoxItems ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Closes the form and discards the Patient data
    ''' </summary>
    Private Sub ExitScreen()
        Try
            If (Not PendingToSaveChanges OrElse _
               (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes)) Then
                patientNamesAttribute = ""
                patientIDAttribute = ""

                bsScreenErrorProvider.Clear()

                'RemoveHandler bsDateOfBirthMaskedTextBox.Validating, AddressOf bsDateOfBirthMaskedTextBox_Validating

                'TR 11/04/2012 -Disable form on close to avoid any button press.
                Me.Enabled = False

                If (Me.MdiParent Is Nothing) Then
                    Me.Close()
                Else 'It is a MDI child
                    If (Not Me.Tag Is Nothing) Then
                        'A PerformClick() method was executed
                        Me.Close()
                    Else
                        'Normal button click - Open the WS Monitor form and close this one
                        UiAx00MainMDI.OpenMonitorForm(Me)
                    End If
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID">The current Language of Application </param>
    ''' <remarks>
    ''' Created by:  PG 07/10/2010
    ''' Modified by: SA 16/11/2010 - Added : for labels in the search criteria area. Get multilanguage text for ToolTip 
    '''                              of Reset Filters in search criteria area
    '''              PG 12/01/2011 - Change Text ToolTip in Search Criteria area
    ''' </remarks>
    ''' 
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Screen titles...
            bsPatientsListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Summary_PatientsList", pLanguageID)
            bsPatientDetailsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Patients_Details", pLanguageID)
            bsPatientSearchLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Patients_Search", pLanguageID)

            'Labels in Patient's Details area
            bsDetailsPatientIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", pLanguageID) & ":"
            bsDetailsFirstNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirstName", pLanguageID) & ":"
            bsDetailsLastNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LastName", pLanguageID) & ":"
            bsDetailsGenderLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Gender", pLanguageID) & ":"
            bsDetailsDoBLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DateOfBirth", pLanguageID) & ":"
            bsDetailsAgeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Age", pLanguageID) & ":"
            bsPerformedbyLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Patients_PerformedBy", pLanguageID) & ":"
            bsCommentsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", pLanguageID) & ":"

            'Labels in Search Criteria area
            bsPatientIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", pLanguageID) & ":"
            bsFirstNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirstName", pLanguageID) & ":"
            bsLastNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LastName", pLanguageID) & ":"
            bsGenderLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Gender", pLanguageID) & ":"

            BsDateBirthRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DateOfBirth", pLanguageID)
            bsDOBFromLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_From", pLanguageID) & ":"
            bsDOBToLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_To", pLanguageID) & ":"

            BsAgeRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Age", pLanguageID)
            bsSearchAgeUnitLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", pLanguageID) & ":"
            bsAgeFromLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_From", pLanguageID) & ":"
            bsAgeToLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_To", pLanguageID) & ":"

            'ToolTips in Patients' List area
            bsScreenToolTips.SetToolTip(bsNewButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", pLanguageID))
            bsScreenToolTips.SetToolTip(bsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))
            bsScreenToolTips.SetToolTip(bsOpenSearchButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Patients_OpenSearch", pLanguageID))
            bsScreenToolTips.SetToolTip(bsClearButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CleanFilters", pLanguageID))

            'ToolTips in Patient's Details area
            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))

            'ToolTips in Search Criteria area
            bsScreenToolTips.SetToolTip(bsSearchButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Search", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCloseSearchButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Patients_CloseSearch", pLanguageID))
            bsScreenToolTips.SetToolTip(bsResetButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CleanFilters", pLanguageID))

            'ToolTips for general screen buttons
            If (entryModeAttribute = "SEARCH") Then
                bsScreenToolTips.SetToolTip(bsSelectPatientButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AcceptSelection", pLanguageID))
                bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CancelSelection", pLanguageID))
            Else
                bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Configure and initialize the GridView of Patient
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    '''                              by calls to the generic function ShowMessage
    '''              SG 09/07/2010 - DataGrid column properties
    '''              SA 19/07/2010 - Hide the HIS/LIS column
    '''              PG 07/10/2010 - Added the LanguageID parameter and get the multilanguage texts for all
    '''                              visible column headers
    '''              SA 16/11/2010 - Added hidden column for the Gender Code
    '''              JC 13/11/2012 - Modified width column patiendID
    ''' </remarks>
    Private Sub InitializePatientsGrid(ByVal pLanguageID As String)
        Try
            'Initialize 
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Dim gridWidth As Integer = bsPatientListDataGridView.Width - 23

            bsPatientListDataGridView.Columns.Clear()
            bsPatientListDataGridView.Columns.Add("PatientID", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", pLanguageID))
            bsPatientListDataGridView.Columns.Add("LastName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LastName", pLanguageID))
            bsPatientListDataGridView.Columns.Add("FirstName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirstName", pLanguageID))
            bsPatientListDataGridView.Columns.Add("Gender", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Gender", pLanguageID))
            bsPatientListDataGridView.Columns.Add("DateOfBirth", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DateOfBirth", pLanguageID))
            bsPatientListDataGridView.Columns.Add("Age", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Age", pLanguageID))

            bsPatientListDataGridView.Columns("PatientID").Width = CInt(15 * gridWidth / 100) ' CInt(15 * gridWidth / 100)
            bsPatientListDataGridView.Columns("LastName").Width = CInt(25 * gridWidth / 100)
            bsPatientListDataGridView.Columns("FirstName").Width = CInt(25 * gridWidth / 100)
            bsPatientListDataGridView.Columns("Gender").Width = CInt(10 * gridWidth / 100) 'CInt(10 * gridWidth / 100)
            bsPatientListDataGridView.Columns("DateOfBirth").Width = CInt(15 * gridWidth / 100)
            bsPatientListDataGridView.Columns("Age").Width = CInt(10 * gridWidth / 100)

            bsPatientListDataGridView.Columns("Gender").SortMode = DataGridViewColumnSortMode.NotSortable
            bsPatientListDataGridView.Columns("LastName").SortMode = DataGridViewColumnSortMode.NotSortable
            bsPatientListDataGridView.Columns("FirstName").SortMode = DataGridViewColumnSortMode.NotSortable
            bsPatientListDataGridView.Columns("Gender").SortMode = DataGridViewColumnSortMode.NotSortable
            bsPatientListDataGridView.Columns("DateOfBirth").SortMode = DataGridViewColumnSortMode.NotSortable
            bsPatientListDataGridView.Columns("Age").SortMode = DataGridViewColumnSortMode.NotSortable

            bsPatientListDataGridView.Columns("Gender").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsPatientListDataGridView.Columns("DateOfBirth").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsPatientListDataGridView.Columns("Age").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsPatientListDataGridView.Columns("Gender").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsPatientListDataGridView.Columns("DateOfBirth").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsPatientListDataGridView.Columns("Age").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            Dim dataGridCheckBoxColumn As New DataGridViewCheckBoxColumn
            With dataGridCheckBoxColumn
                .Name = "HIS/LIS"
                .HeaderText = "HIS/LIS"
            End With
            bsPatientListDataGridView.Columns.Add(dataGridCheckBoxColumn)
            bsPatientListDataGridView.Columns("HIS/LIS").Width = CInt(10 * gridWidth / 100)
            bsPatientListDataGridView.Columns("HIS/LIS").SortMode = DataGridViewColumnSortMode.NotSortable
            bsPatientListDataGridView.Columns("HIS/LIS").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsPatientListDataGridView.Columns("HIS/LIS").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsPatientListDataGridView.Columns("HIS/LIS").Visible = False

            'Grid columns definition  --> Following columns are invisible as per the design
            bsPatientListDataGridView.Columns.Add("PatientType", "PatientType")
            bsPatientListDataGridView.Columns.Add("GenderCode", "GenderCode")
            bsPatientListDataGridView.Columns.Add("ExternalPID", "ExternalPID")
            bsPatientListDataGridView.Columns.Add("ExternalArrivalDate", "ExternalArrivalDate")
            bsPatientListDataGridView.Columns.Add("PerformedBy", "PerformedBy")
            bsPatientListDataGridView.Columns.Add("Comments", "Comments")
            bsPatientListDataGridView.Columns.Add("TS_User", "TS_User")
            bsPatientListDataGridView.Columns.Add("TS_DateTime", "TS_DateTime")
            bsPatientListDataGridView.Columns.Add("InUse", "InUse")

            bsPatientListDataGridView.Columns("PatientType").Visible = False
            bsPatientListDataGridView.Columns("GenderCode").Visible = False
            bsPatientListDataGridView.Columns("ExternalPID").Visible = False
            bsPatientListDataGridView.Columns("ExternalArrivalDate").Visible = False
            bsPatientListDataGridView.Columns("PerformedBy").Visible = False
            bsPatientListDataGridView.Columns("Comments").Visible = False
            bsPatientListDataGridView.Columns("TS_User").Visible = False
            bsPatientListDataGridView.Columns("TS_DateTime").Visible = False
            bsPatientListDataGridView.Columns("InUse").Visible = False

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializePatientsGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializePatientsGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the search criteria fields
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 09/07/2010
    ''' </remarks>
    Private Sub InitializeSearchCriteria()
        Try
            bsPatientIDTextbox.ResetText()
            bsFirstNameTextBox.ResetText()
            bsLastNameTextBox.ResetText()
            bsGenderComboBox.SelectedIndex = -1

            BsDateBirthRadioButton.Checked = False
            bsFromDateTimePicker.Value = DateTime.Now
            bsFromDateTimePicker.Checked = False
            bsToDateTimePicker.Value = DateTime.Now
            bsToDateTimePicker.Checked = False
            EnablingGroupBoxItems(Me.bsDateOfBirthGroupBox, BsDateBirthRadioButton.Checked)

            BsAgeRadioButton.Checked = False
            bsAgeUnitsComboBox.SelectedIndex = 0

            bsAgeFromNumericUpDown.Value = bsAgeFromNumericUpDown.Minimum
            bsAgeToNumericUpDown.Value = bsAgeToNumericUpDown.Maximum
            EnablingGroupBoxItems(bsAgeGroupbox, BsAgeRadioButton.Checked)

            bsCloseSearchButton.Enabled = True
            bsPatientIDTextbox.Focus()

            'RH 18/11/2011 Create mask for Date in the current culture
            Dim DateSeparator As String = SystemInfoManager.OSDateSeparator
            Dim Mask As System.Text.StringBuilder = New System.Text.StringBuilder(SystemInfoManager.OSDateFormat)

            For i As Integer = 0 To Mask.Length - 1
                If Mask(i) <> DateSeparator Then Mask(i) = "a"c
            Next

            'JB 08/11/2012
            'bsDateOfBirthMaskedTextBox.Mask = Mask.ToString()
            'MaskedTextEmpty = bsDateOfBirthMaskedTextBox.Text 'RH 23/02/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeSearchCriteria", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeSearchCriteria", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get minimum and maximum limits for Age From/To Numeric Up/Downs according the current Age Unit
    ''' </summary>
    ''' <param name="pLimitID">Code of the LimitID to get</param>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              SA 19/07/2010 - Added error control after calling the function in the delegate 
    ''' </remarks>
    Private Sub LoadAgeLimitValues(ByVal pLimitID As FieldLimitsEnum)
        Try
            Dim resultData As New GlobalDataTO
            Dim fieldLimitsConfig As New FieldLimitsDelegate

            resultData = fieldLimitsConfig.GetList(Nothing, pLimitID)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim fieldDS As New FieldLimitsDS
                fieldDS = CType(resultData.SetDatos, FieldLimitsDS)

                If (fieldDS.tfmwFieldLimits.Rows.Count > 0) Then
                    bsAgeFromNumericUpDown.Minimum = Convert.ToDecimal(fieldDS.tfmwFieldLimits(0).MinValue)
                    bsAgeFromNumericUpDown.Maximum = Convert.ToDecimal(fieldDS.tfmwFieldLimits(0).MaxValue)
                    bsAgeFromNumericUpDown.Value = bsAgeFromNumericUpDown.Minimum

                    bsAgeToNumericUpDown.Minimum = Convert.ToDecimal(fieldDS.tfmwFieldLimits(0).MinValue)
                    bsAgeToNumericUpDown.Maximum = Convert.ToDecimal(fieldDS.tfmwFieldLimits(0).MaxValue)
                    bsAgeToNumericUpDown.Value = bsAgeToNumericUpDown.Maximum
                End If
            Else
                'Error getting the informed Age Limits; shown it
                ShowMessage(Me.Name & ".LoadAgeLimitValues", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadAgeLimitValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadAgeLimitValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the available Age Units from the DB and load the ComboBox of Age Units in area of Search Criteria
    ''' </summary>
    ''' <remarks>
    ''' Modified by: VR 29/12/2009 - Change the  Constant Value 
    '''              BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              SA 19/07/2010 - Added error control after calling the function in the delegate
    '''              SA 16/11/2010 - Load a global PreloadedMasterDataDS instead of a local one
    ''' </remarks>
    Private Sub LoadAgeUnits()
        Try
            Dim resultData As New GlobalDataTO
            Dim preloadedMasterConfig As New PreloadedMasterDataDelegate

            resultData = preloadedMasterConfig.GetList(Nothing, PreloadedMasterDataEnum.AGE_UNITS)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                AgeUnitsListDS = CType(resultData.SetDatos, PreloadedMasterDataDS)

                If (AgeUnitsListDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                    AgeUnitsListDS.tfmwPreloadedMasterData.DefaultView.Sort = "Position"

                    bsAgeUnitsComboBox.DataSource = AgeUnitsListDS.tfmwPreloadedMasterData
                    bsAgeUnitsComboBox.DisplayMember = "FixedItemDesc"
                    bsAgeUnitsComboBox.ValueMember = "ItemID"
                End If
            Else
                'Error getting the list of available Age Units; shown it
                ShowMessage(Me.Name & ".LoadAgeUnits", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadAgeUnits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadAgeUnits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fills Patient Grid View with data according to the search criteria.
    ''' </summary>
    ''' <remarks>
    ''' Modified by RH            - A lot of refactoring and enhancements
    '''             SG 13/07/2010 - Management of the RadioButtons Age and DateOfBirth
    '''                             Enhancements of the data query
    '''             SG 30/07/2010 - Add a row to the DS containing the searching filters only when at least a filter
    '''                             has been informed
    '''             SA 16/11/2010 - Inform also the Gender Code in the correspondent grid column
    '''             SA 12/01/2011 - For not InUse Patients, verify if there is in the list of Patients with OPEN requested
    '''                             Order Tests and in this case, mark it as InUse
    '''             XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Sub LoadFilteredPatientList()
        Try
            'Fill the DS with the informed filters
            Dim myPatientFiltersDS As New PatientFilterCriteriaDS
            Dim getPatientsDataRow As PatientFilterCriteriaDS.tparPatientFilterCriteriaRow
            getPatientsDataRow = myPatientFiltersDS.tparPatientFilterCriteria.NewtparPatientFilterCriteriaRow

            Dim hasData As Boolean = False
            If (bsPatientIDTextbox.Text <> "") Then
                getPatientsDataRow.PatientID = bsPatientIDTextbox.Text
                hasData = True
            Else
                getPatientsDataRow.SetPatientIDNull()
            End If

            If (bsGenderComboBox.Text <> "") Then
                getPatientsDataRow.Gender = bsGenderComboBox.SelectedValue.ToString
                hasData = True
            Else
                getPatientsDataRow.SetGenderNull()
            End If

            If (bsFirstNameTextBox.Text <> "") Then
                getPatientsDataRow.FirstName = bsFirstNameTextBox.Text
                hasData = True
            Else
                getPatientsDataRow.SetFirstNameNull()
            End If

            If (bsLastNameTextBox.Text <> "") Then
                getPatientsDataRow.LastName = bsLastNameTextBox.Text
                hasData = True
            Else
                getPatientsDataRow.SetLastNameNull()
            End If

            If (Me.BsDateBirthRadioButton.Checked) Then
                If (bsFromDateTimePicker.Checked And bsToDateTimePicker.Checked) Then
                    getPatientsDataRow.DobFrom = bsFromDateTimePicker.Value
                    getPatientsDataRow.DobTo = bsToDateTimePicker.Value
                    hasData = True
                ElseIf (bsFromDateTimePicker.Checked And Not bsToDateTimePicker.Checked) Then
                    getPatientsDataRow.DobFrom = bsFromDateTimePicker.Value
                    hasData = True
                ElseIf (Not bsFromDateTimePicker.Checked And bsToDateTimePicker.Checked) Then
                    getPatientsDataRow.DobTo = bsToDateTimePicker.Value
                    hasData = True
                End If
            End If

            If (Me.BsAgeRadioButton.Checked) Then
                If (bsAgeUnitsComboBox.SelectedValue.ToString <> "") Then
                    If (bsAgeFromNumericUpDown.Text <> "") Then
                        Dim ageFrom As Integer = SetAgeToYears(Convert.ToInt32(bsAgeFromNumericUpDown.Text), bsAgeUnitsComboBox.SelectedValue.ToString)
                        getPatientsDataRow.AgeFrom = ageFrom
                        hasData = True
                    Else
                        getPatientsDataRow.SetAgeFromNull()
                    End If

                    If (bsAgeToNumericUpDown.Text <> "") Then
                        Dim ageTo As Integer = SetAgeToYears(Convert.ToInt32(bsAgeToNumericUpDown.Text), bsAgeUnitsComboBox.SelectedValue.ToString)
                        getPatientsDataRow.AgeTo = ageTo
                        hasData = True
                    Else
                        getPatientsDataRow.SetAgeToNull()
                    End If
                End If
            End If
            If (hasData = True) Then myPatientFiltersDS.tparPatientFilterCriteria.AddtparPatientFilterCriteriaRow(getPatientsDataRow)
            FiltersInformed = hasData

            'Search all Patients that fullfil the informed criteria
            Dim resultData As GlobalDataTO
            Dim patientsList As New PatientDelegate

            resultData = patientsList.GetListWithFilters(Nothing, myPatientFiltersDS)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim patientListDS As New PatientsDS
                patientListDS = DirectCast(resultData.SetDatos, PatientsDS)

                bsPatientListDataGridView.Rows.Clear()
                If (patientListDS.tparPatients.Rows.Count > 0) Then
                    'Populate the grid with filtered records
                    ProcessEvent = False
                    bsPatientListDataGridView.Rows.Add(patientListDS.tparPatients.Rows.Count)
                    ProcessEvent = True

                    Dim i As Integer = 0
                    For Each rowtparPatient As PatientsDS.tparPatientsRow In patientListDS.tparPatients.Rows
                        bsPatientListDataGridView("PatientID", i).Value = rowtparPatient.PatientID
                        bsPatientListDataGridView("LastName", i).Value = rowtparPatient.LastName
                        bsPatientListDataGridView("FirstName", i).Value = rowtparPatient.FirstName

                        If (Not rowtparPatient.IsGenderNull) Then
                            'Get the Gender description from the DS used to load the Gender ComboBoxes
                            Dim genderRows As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                            genderRows = (From grow In GenderListDS.tfmwPreloadedMasterData _
                                         Where grow.ItemID = rowtparPatient.Gender _
                                        Select grow).ToList()
                            If (genderRows.Count = 1) Then
                                bsPatientListDataGridView("GenderCode", i).Value = rowtparPatient.Gender
                                bsPatientListDataGridView("Gender", i).Value = genderRows(0).FixedItemDesc
                            Else
                                bsPatientListDataGridView("GenderCode", i).Value = String.Empty
                                bsPatientListDataGridView("Gender", i).Value = String.Empty
                            End If
                        Else
                            bsPatientListDataGridView("GenderCode", i).Value = String.Empty
                            bsPatientListDataGridView("Gender", i).Value = String.Empty
                        End If

                        If (rowtparPatient.IsDateOfBirthNull) Then
                            bsPatientListDataGridView("DateOfBirth", i).Value = String.Empty
                            bsPatientListDataGridView("Age", i).Value = String.Empty
                        Else
                            bsPatientListDataGridView("DateOfBirth", i).Value = rowtparPatient.DateOfBirth.ToString(SystemInfoManager.OSDateFormat)
                            bsPatientListDataGridView("Age", i).Value = Utilities.GetAgeUnits(rowtparPatient.DateOfBirth, AgeUnitsListDS)
                        End If

                        bsPatientListDataGridView("HIS/LIS", i).Value = rowtparPatient.ExternalPID <> String.Empty
                        bsPatientListDataGridView("PatientType", i).Value = rowtparPatient.PatientType
                        bsPatientListDataGridView("ExternalPID", i).Value = rowtparPatient.ExternalPID

                        If (rowtparPatient.IsExternalArrivalDateNull) Then
                            bsPatientListDataGridView("ExternalArrivalDate", i).Value = String.Empty
                        Else
                            bsPatientListDataGridView("ExternalArrivalDate", i).Value = rowtparPatient.ExternalArrivalDate.ToString(SystemInfoManager.OSDateFormat)
                        End If

                        bsPatientListDataGridView("PerformedBy", i).Value = rowtparPatient.PerformedBy
                        bsPatientListDataGridView("Comments", i).Value = rowtparPatient.Comments
                        bsPatientListDataGridView("TS_User", i).Value = rowtparPatient.TS_User
                        bsPatientListDataGridView("TS_DateTime", i).Value = rowtparPatient.TS_DateTime
                        bsPatientListDataGridView("InUse", i).Value = rowtparPatient.InUse

                        'If the Patient is InUse in the active WorkSession it is shown with a different background
                        If (rowtparPatient.InUse) Then
                            bsPatientListDataGridView.Rows(i).DefaultCellStyle.BackColor = Color.White
                            bsPatientListDataGridView.Rows(i).DefaultCellStyle.ForeColor = Color.Gray
                        Else
                            'Verify if there are OPEN but not saved Order Tests requested for the Patient in the active WS 
                            If (patientsListAttribute.Count > 0) Then
                                For Each row As String In patientsListAttribute
                                    'If (row.ToUpper = rowtparPatient.PatientID.ToUpper) Then
                                    If (row.ToUpperBS = rowtparPatient.PatientID.ToUpperBS) Then
                                        bsPatientListDataGridView("InUse", i).Value = True
                                        bsPatientListDataGridView.Rows(i).DefaultCellStyle.BackColor = Color.White
                                        bsPatientListDataGridView.Rows(i).DefaultCellStyle.ForeColor = Color.Gray
                                    End If
                                Next
                            End If
                        End If
                        i += 1
                    Next

                    Application.DoEvents() 'RH 16/12/2010

                    bsEditButton.Enabled = True
                    bsDeleteButton.Enabled = True And EnableDeleteButton
                    bsPrintButton.Enabled = True
                    If (bsSelectPatientButton.Visible) Then bsSelectPatientButton.Enabled = True

                    'First Patient in the grid is selected
                    bsPatientListDataGridView.Rows(0).Selected = True
                    LoadSelectedPatientDataFields()
                Else
                    'bsEditButton.Enabled = False
                    bsDeleteButton.Enabled = False
                    bsPrintButton.Enabled = False

                    bsSelectPatientButton.Enabled = False

                    'dl 11/10/2011
                    ClearPatientDetails()

                    bsDeleteButton.Enabled = False
                    bsEditButton.Enabled = False
                    bsPrintButton.Enabled = False

                    patientNamesAttribute = String.Empty
                    patientIDAttribute = String.Empty

                    EditingPatientData = Nothing
                    'dl 11/10/2011
                End If

                'Status of other buttons and visibility of screen panels
                bsNewButton.Enabled = True
                bsOpenSearchButton.Enabled = True
                bsClearButton.Enabled = FiltersInformed

                bsSearchCriteriaPanel.Visible = False
                bsGridPanel.Visible = True
                bsDetailsPanel.Visible = True

                'bsPatientListDataGridView_SelectionChanged(Nothing, Nothing)
                PendingToSaveChanges = False
            Else
                'Error getting the list of Patients...; shown it
                ShowMessage(Me.Name & ".LoadFilteredPatientList", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadFilteredPatientList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadFilteredPatientList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Get the available Genders from the DB and load the ComboBox of Genders in area of Search Criteria 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 30/07/2010
    ''' </remarks>
    Private Sub LoadFilterGenders()
        Try
            Dim resultData As New GlobalDataTO
            Dim preloadedMasterConfig As New PreloadedMasterDataDelegate

            resultData = preloadedMasterConfig.GetList(Nothing, PreloadedMasterDataEnum.SEX_LIST)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                FilterGenderListDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                If (FilterGenderListDS.tfmwPreloadedMasterData.Rows.Count > 0) Then

                    'Insert an empy item
                    Dim emptyGenderRow As DataRow = FilterGenderListDS.tfmwPreloadedMasterData.NewRow
                    emptyGenderRow(0) = "SEX_LIST"
                    emptyGenderRow(1) = " "
                    emptyGenderRow(2) = ""
                    emptyGenderRow(3) = ""
                    emptyGenderRow(4) = 0
                    emptyGenderRow(5) = True
                    emptyGenderRow(6) = ""

                    FilterGenderListDS.tfmwPreloadedMasterData.Rows.Add(emptyGenderRow)
                    FilterGenderListDS.tfmwPreloadedMasterData.DefaultView.Sort = "Position"

                    bsGenderComboBox.DataSource = FilterGenderListDS.tfmwPreloadedMasterData.DefaultView
                    bsGenderComboBox.DisplayMember = "FixedItemDesc"
                    bsGenderComboBox.ValueMember = "ItemID"
                End If

            Else
                'Error getting the list of available Genders; shown it
                ShowMessage(Me.Name & ".LoadFilterGenders", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadFilterGenders", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadFilterGenders", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the available Genders from the DB and load the ComboBox of Genders in area of Patient's Details 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: VR 29/12/2009 - Change the Constant Value to Enum Value
    '''              BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              RH            - Added a new DS for a new Gender Combo Box
    '''              SA 19/07/2010 - Added error control after calling the function in the delegate
    '''              DL 23/07/2010 - Added an empty item to list of Genders
    ''' </remarks>
    Private Sub LoadGenders()
        Try
            Dim resultData As New GlobalDataTO
            Dim preloadedMasterConfig As New PreloadedMasterDataDelegate

            resultData = preloadedMasterConfig.GetList(Nothing, PreloadedMasterDataEnum.SEX_LIST)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                GenderListDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                If (GenderListDS.tfmwPreloadedMasterData.Rows.Count > 0) Then

                    'Insert an empy item
                    Dim emptyGenderRow As DataRow = GenderListDS.tfmwPreloadedMasterData.NewRow
                    emptyGenderRow(0) = "SEX_LIST"
                    emptyGenderRow(1) = " "
                    emptyGenderRow(2) = ""
                    emptyGenderRow(3) = ""
                    emptyGenderRow(4) = 0
                    emptyGenderRow(5) = True
                    emptyGenderRow(6) = ""

                    GenderListDS.tfmwPreloadedMasterData.Rows.Add(emptyGenderRow)
                    GenderListDS.tfmwPreloadedMasterData.DefaultView.Sort = "Position"

                    'Load Gender ComboBox in details frame
                    bsGenderDetailsComboBox.DataSource = GenderListDS.tfmwPreloadedMasterData.DefaultView
                    bsGenderDetailsComboBox.DisplayMember = "FixedItemDesc"
                    bsGenderDetailsComboBox.ValueMember = "ItemID"
                End If

            Else
                'Error getting the list of available Genders; shown it
                ShowMessage(Me.Name & ".LoadGenders", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadGenders", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadGenders", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load data of the selected patient into the edition fields
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 12/07/2010
    ''' Modified by: AG 04/11/2010 - Execute code only when there is a selected row
    '''              SA 16/11/2010 - Select the Patient Gender in the correspondent ComboBox using the GenderCode 
    '''                              from the grid
    ''' </remarks>
    Private Function LoadSelectedPatientDataFields() As DataGridViewRow
        Try
            If (bsPatientListDataGridView.SelectedRows.Count > 0) Then
                Dim row As DataGridViewRow = bsPatientListDataGridView.SelectedRows(0)

                bsDetailsPatientIDTextBox.Text = row.Cells("PatientID").Value.ToString()
                bsDetailsFirstNameTextBox.Text = row.Cells("FirstName").Value.ToString()
                bsDetailsLastNameTextBox.Text = row.Cells("LastName").Value.ToString()

                Dim genderCode As String = Convert.ToString(row.Cells("GenderCode").Value)
                If (genderCode.Length > 0) Then
                    bsGenderDetailsComboBox.SelectedValue = genderCode
                Else
                    bsGenderDetailsComboBox.SelectedIndex = 0
                End If

                Dim DateOfBirthStr As String = Convert.ToString(row.Cells("DateOfBirth").Value)
                If (String.IsNullOrEmpty(DateOfBirthStr)) Then
                    bsDateOfBirthMaskedTextBox.Text = String.Empty
                    bsDetailsAgeByTextBox.Text = String.Empty
                Else
                    Dim DateOfBirth As Date = Convert.ToDateTime(DateOfBirthStr)
                    bsDateOfBirthMaskedTextBox.DateTime = DateOfBirth
                    bsDetailsAgeByTextBox.Text = Utilities.GetAgeUnits(DateOfBirth, AgeUnitsListDS)
                End If

                bsPerformedByTextBox.Text = row.Cells("PerformedBy").Value.ToString()
                bsCommentsTextBox.Text = row.Cells("Comments").Value.ToString()

                If (Convert.ToBoolean(row.Cells("InUse").Value)) Then
                    row.DefaultCellStyle.SelectionBackColor = Color.LightGray
                    row.DefaultCellStyle.SelectionForeColor = Color.White
                End If

                bsDeleteButton.Enabled = Not Convert.ToBoolean(row.Cells("InUse").Value) And EnableDeleteButton
                bsEditButton.Enabled = Not Convert.ToBoolean(row.Cells("InUse").Value)
                bsPrintButton.Enabled = True

                patientIDAttribute = bsDetailsPatientIDTextBox.Text
                patientNamesAttribute = bsDetailsFirstNameTextBox.Text & " " & bsDetailsLastNameTextBox.Text

                UpdateDetailsColors(False)
                Return row
            Else
                Return Nothing
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadSelectedPatientDataFields", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadSelectedPatientDataFields", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Prepare the Patient's Details area for adding a new Patient
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 02/08/2010
    ''' Modified by: SA 16/11/2010 - Removed disable of fields by GroupBox; changed the way of verify if there are changes pending
    '''                              to save. Unselect all rows selected in the Patients' grid
    ''' </remarks>
    Private Sub NewPatientData()
        Try
            Dim cancelAdd As Boolean = False
            If (bsSaveButton.Enabled) Then
                cancelAdd = (Not DiscardPendingChanges())
            End If

            If (Not cancelAdd) Then
                EditMode = "ADD"
                bsDetailsPatientIDTextBox.Enabled = True
                UpdateDetailsColors(True)

                bsClearButton.Enabled = False
                bsOpenSearchButton.Enabled = False
                bsNewButton.Enabled = True
                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
                bsPrintButton.Enabled = False

                bsSearchCriteriaPanel.Visible = False
                bsGridPanel.Visible = True
                bsDetailsPanel.Visible = True

                ClearPatientDetails()
                PendingToSaveChanges = False
                bsDetailsPatientIDTextBox.Focus()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".NewPatientData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".NewPatientData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen loading: fill lists of existing Genders and Age Limits, initialize and load the Patients grid and 
    ''' set the screen status to initial mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              PG 07/10/2010 - Get the current Language by change labels of screen  
    '''              SA 16/11/2010 - Some changes in logic; move some code from InitialModeScreenStatus
    ''' </remarks>
    Private Sub PatientSearchLoad()
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels
            GetScreenLabels(currentLanguage)

            'Load ComboBoxes and Field Limits
            LoadGenders()
            LoadFilterGenders()
            LoadAgeUnits()
            LoadAgeLimitValues(FieldLimitsEnum.AGE_FROM_TO_YEARS)

            Application.DoEvents() 'RH 16/12/2010

            'Set initial values to screen controls (Details and Search areas)
            bsGenderDetailsComboBox.SelectedIndex = 0
            InitializeSearchCriteria()

            'Configure the screen Layout according the informed EntryMode
            If (entryModeAttribute = "SEARCH") Then
                'Screen was opened from WS Preparation; it is then opened as a PopUp 
                Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedDialog
                Me.Height = PopUpHeight

                bsSelectPatientButton.Visible = True
                bsPatientIDTextbox.Text = patientIDAttribute
            Else
                bsSelectPatientButton.Enabled = False
                bsSelectPatientButton.Visible = False
            End If

            Application.DoEvents() 'RH 16/12/2010

            'Initialize the Patient Grid and load it with the list of existing Patients
            InitializePatientsGrid(currentLanguage)
            LoadFilteredPatientList()

            'Put Details area in Query Mode
            UpdateDetailsColors(False)

            Application.DoEvents() 'RH 16/12/2010

            'TR 09/11/2011 -Set the acceptButton the CloseSearchButton
            Me.AcceptButton = bsCloseSearchButton

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PatientSearchLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PatientSearchLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to load the button images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/04/2010
    ''' Modified by: RH 22/06/2010 - Added new icons
    '''              DL 03/11/2010 - Load Icon in Image Property instead of in BackgroundImage Property
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'NEW Button
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

            'CANCEL EDITION and CANCEL SEARCH Buttons
            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsSelectPatientButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'FIND Button
            auxIconName = GetIconName("FIND")
            If (auxIconName <> "") Then
                bsSearchButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL") '("ACCEPT")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                bsCloseSearchButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'FILTER Button
            auxIconName = GetIconName("FILTER")
            If (auxIconName <> "") Then
                bsOpenSearchButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CLEAR FILTER Button
            auxIconName = GetIconName("DELFILTER")
            If (auxIconName <> "") Then
                bsClearButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'RESET FIELDS IN FILTER FRAME Button SG 12/07/2010
            auxIconName = GetIconName("RESETFIELD")
            If (auxIconName <> "") Then
                bsResetButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Update data of the selected Patient
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' Modified by: SA 16/11/2010 - Get the SelectedValue from Genders ComboBox, not the first character of the description!!
    ''' </remarks>
    Private Sub SaveChanges()
        Try
            Dim patientData As New PatientsDS
            Dim row As DataGridViewRow = bsPatientListDataGridView.SelectedRows(0)
            Dim patientRow As PatientsDS.tparPatientsRow = patientData.tparPatients.NewtparPatientsRow()

            With patientRow
                .PatientType = Convert.ToString(row.Cells("PatientType").Value)
                .PatientID = row.Cells("PatientID").Value.ToString()
                .FirstName = bsDetailsFirstNameTextBox.Text.Trim()
                .LastName = bsDetailsLastNameTextBox.Text.Trim()
                .InUse = False

                If (bsGenderDetailsComboBox.Text.Length > 0) Then .Gender = bsGenderDetailsComboBox.SelectedValue.ToString
                If (bsDetailsAgeByTextBox.Text <> String.Empty) Then .DateOfBirth = bsDateOfBirthMaskedTextBox.DateTime
                If (bsPerformedByTextBox.Text.Trim() <> "") Then .PerformedBy = bsPerformedByTextBox.Text.Trim()
                If (bsCommentsTextBox.Text.Trim() <> "") Then .Comments = bsCommentsTextBox.Text.Trim()

                Dim currentSession As New ApplicationSessionManager
                .TS_User = GlobalBase.GetSessionInfo().UserName
                .TS_DateTime = Date.Now
            End With
            patientData.tparPatients.AddtparPatientsRow(patientRow)

            'Modify values of an existing Patient
            Dim returnedData As GlobalDataTO
            Dim patientToSave As New PatientDelegate
            returnedData = patientToSave.Modify(Nothing, patientData)

            If (Not returnedData.HasError) Then
                PendingToSaveChanges = False
            Else
                'Error updating the Patient data; shown it
                ShowMessage(Me.Name & ".SaveChanges", returnedData.ErrorCode, returnedData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Add a new Patient
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' Modified by: SA 16/11/2010 - Get the SelectedValue from Genders ComboBox, not the first character of the description!!
    ''' </remarks>
    Private Function SaveNewPatient() As Boolean
        Dim patientCreated As Boolean = False
        Try
            Dim patientData As New PatientsDS
            Dim patientRow As PatientsDS.tparPatientsRow = patientData.tparPatients.NewtparPatientsRow()

            With patientRow
                .PatientType = "MAN"
                .PatientID = bsDetailsPatientIDTextBox.Text.Trim()
                .FirstName = bsDetailsFirstNameTextBox.Text.Trim()
                .LastName = bsDetailsLastNameTextBox.Text.Trim()
                .InUse = False

                If (bsGenderDetailsComboBox.Text.Length > 0) Then .Gender = bsGenderDetailsComboBox.SelectedValue.ToString
                If (bsDetailsAgeByTextBox.Text <> String.Empty) Then .DateOfBirth = bsDateOfBirthMaskedTextBox.DateTime

                If (bsPerformedByTextBox.Text.Trim() <> "") Then .PerformedBy = bsPerformedByTextBox.Text.Trim()
                If (bsCommentsTextBox.Text.Trim() <> "") Then .Comments = bsCommentsTextBox.Text.Trim()

                Dim currentSession As New ApplicationSessionManager
                .TS_User = GlobalBase.GetSessionInfo().UserName
                .TS_DateTime = Date.Now
            End With
            patientData.tparPatients.AddtparPatientsRow(patientRow)

            'Saves values of a new Patient
            Dim returnedData As GlobalDataTO
            Dim patientToSave As New PatientDelegate

            returnedData = patientToSave.Add(Nothing, patientData, False) 'AG 13/03/2013 add parameter False (patient not from xml message)
            If (Not returnedData.HasError) Then
                patientCreated = True
                PendingToSaveChanges = False

            ElseIf (returnedData.ErrorCode = GlobalEnumerates.Messages.DUPLICATED_PATIENT_ID.ToString) Then
                'Informed PatientID already exists for another Patient
                Me.bsScreenErrorProvider.Clear()
                Me.bsScreenErrorProvider.SetError(bsDetailsPatientIDTextBox, GetMessageText(returnedData.ErrorCode)) 'SG 12/07/2010

                bsDetailsPatientIDTextBox.Focus()
            Else
                'Error creating the new Patient
                ShowMessage(Me.Name & ".SaveNewPatient", returnedData.ErrorCode, returnedData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveNewPatient ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveNewPatient", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return patientCreated
    End Function

    ''' <summary>
    ''' Enable or disable functionallity by user level.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 23/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' Modified by SGM 31/05/2013 - Not to allow Operator to Create nor Print data
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    bsDeleteButton.Enabled = False
                    bsNewButton.Enabled = False 'SGM 31/05/2013
                    bsPrintButton.Enabled = False 'SGM 31/05/2013
                    bsEditButton.Enabled = False 'SGM 31/05/2013
                    Exit Select
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Converts any age value to its corresponding value in years
    ''' </summary>
    ''' <param name="pValue">Age Value to convert</param>
    ''' <param name="pUnit">Current Age Unit</param>
    ''' <remarks>
    ''' Created by:  SG 13/07/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Private Function SetAgeToYears(ByVal pValue As Integer, ByVal pUnit As String) As Integer
        Try
            'Select Case (pUnit.ToUpper)
            Select Case (pUnit.ToUpperBS)
                Case "Y"
                    Return pValue
                Case "M"
                    Return CInt(pValue / 12)
                Case "D"
                    Return CInt(pValue / 365.25)
                Case Else
                    Return pValue
            End Select
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetAgeToYears ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetAgeToYears ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

            Return pValue
        End Try
    End Function

    ''' <summary>
    ''' Updates the BackColor of all the Patient Details fields
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' Modified by: SA 16/11/2010 - Changed the logic; added parameter for the status to set for the controls
    ''' </remarks>
    Private Sub UpdateDetailsColors(ByVal pStatus As Boolean)
        Try
            If (pStatus) Then
                'Mandatory fields...
                If (EditMode <> "EDIT") Then
                    bsDetailsPatientIDTextBox.BackColor = Color.Khaki
                    bsDetailsFirstNameTextBox.BackColor = Color.Khaki
                    bsDetailsLastNameTextBox.BackColor = Color.Khaki
                Else
                    bsDetailsPatientIDTextBox.BackColor = SystemColors.MenuBar
                    bsDetailsFirstNameTextBox.BackColor = Color.White
                    bsDetailsLastNameTextBox.BackColor = Color.White
                End If

                'Optional fields...
                bsDetailsAgeByTextBox.BackColor = Color.Gainsboro
                bsPerformedByTextBox.BackColor = Color.White
                bsCommentsTextBox.BackColor = Color.White
            Else
                bsDetailsPatientIDTextBox.BackColor = SystemColors.MenuBar
                bsDetailsFirstNameTextBox.BackColor = SystemColors.MenuBar
                bsDetailsLastNameTextBox.BackColor = SystemColors.MenuBar

                bsDetailsAgeByTextBox.BackColor = SystemColors.MenuBar
                bsPerformedByTextBox.BackColor = SystemColors.MenuBar
                bsCommentsTextBox.BackColor = SystemColors.MenuBar
            End If

            bsDetailsPatientIDTextBox.Enabled = (pStatus And EditMode <> "EDIT")
            bsDetailsFirstNameTextBox.Enabled = pStatus
            bsDetailsLastNameTextBox.Enabled = pStatus
            bsGenderDetailsComboBox.Enabled = pStatus

            bsDateOfBirthMaskedTextBox.Enabled = pStatus

            bsPerformedByTextBox.Enabled = pStatus
            bsCommentsTextBox.Enabled = pStatus

            bsSaveButton.Enabled = pStatus
            bsCancelButton.Enabled = pStatus

            bsOpenSearchButton.Enabled = (Not pStatus)
            bsClearButton.Enabled = (Not pStatus And FiltersInformed)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateDetailsColors", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateDetailsColors", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validates bsDateOfBirthMaskedTextBox value
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 23/02/2012
    ''' Modified by: JB 08/11/2012 - Using the DevExpress DateEdit
    ''' </remarks>
    Private Sub ValidateDateOfBirthMaskedTextBox()
        Try
            If String.IsNullOrEmpty(bsDateOfBirthMaskedTextBox.Text) Then
                bsDetailsAgeByTextBox.Text = String.Empty
                Return
            End If

            Dim NewDate As DateTime = bsDateOfBirthMaskedTextBox.DateTime

            If (Today.AddYears(-150) < NewDate) AndAlso (NewDate <= Today) Then
                'Date is well formed and in the valid range (Today - 150 < v <= Today)
                bsDetailsAgeByTextBox.Text = Utilities.GetAgeUnits(NewDate, AgeUnitsListDS)
            Else
                bsDetailsAgeByTextBox.Text = String.Empty

                bsScreenErrorProvider.SetError(bsDateOfBirthMaskedTextBox, GetMessageText(GlobalEnumerates.Messages.INVALIDDATE.ToString()))
                bsDateOfBirthMaskedTextBox.Focus()

                ValidationError = True
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateDateOfBirthMaskedTextBox ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateDateOfBirthMaskedTextBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate the First Name is informed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/28/2010
    ''' </remarks>
    Private Sub ValidateFirstName()
        Try
            If (bsDetailsFirstNameTextBox.Text.Trim.Length = 0) Then
                ValidationError = True
                bsScreenErrorProvider.SetError(bsDetailsFirstNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateFirstName ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateFirstName ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate the Last Name is informed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/28/2010
    ''' Modified by: SA 17/11/2010 - Reduce the control width to allow correct visualization of the Error Provider simbol
    ''' </remarks>
    Private Sub ValidateLastName()
        Try
            If (bsDetailsLastNameTextBox.Text.Trim.Length = 0) Then
                ValidationError = True
                bsDetailsLastNameTextBox.SetBounds(bsDetailsLastNameTextBox.Location.X, bsDetailsLastNameTextBox.Location.Y, _
                                                   314, bsDetailsLastNameTextBox.Size.Height)
                bsScreenErrorProvider.SetError(bsDetailsLastNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateLastName ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateLastName ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate the PatientID is informed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/23/2010
    ''' </remarks>
    Private Sub ValidatePatientID()
        Try
            If (bsDetailsPatientIDTextBox.Text.Trim.Length = 0) Then
                ValidationError = True
                bsScreenErrorProvider.SetError(bsDetailsPatientIDTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidatePatientID ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidatePatientID ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"

    '*************************'
    '* EVENTS FOR THE SCREEN *'
    '*************************'
    ''' <summary>
    ''' When the screen is in ADD or EDIT mode and ESC key is pressed, code of Cancel Button is executed;
    ''' in other case, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010
    ''' Modified by: AG 20/12/2010 - When there is a Patient selected in the grid and the Enter Key is pressed, Patient
    '''                              data is loaded in details area in edit mode
    ''' </remarks>
    Private Sub IProgPatientData_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (bsCancelButton.Enabled) Then
                    'CancelSearchEdition()

                    'RH 04/07/2011 Escape key should do exactly the same operations as bsCancelButton_Click()
                    bsCancelButton.PerformClick()

                ElseIf bsCloseSearchButton.Enabled AndAlso bsCloseSearchButton.Visible Then
                    bsCloseSearchButton.PerformClick()

                Else
                    'ExitScreen()
                    'RH 04/07/2011 Escape key should do exactly the same operations as bsExitButton_Click()
                    bsExitButton.PerformClick()
                End If

            ElseIf (e.KeyCode = Keys.Enter) Then
                If (bsPatientListDataGridView.Focused AndAlso bsPatientListDataGridView.SelectedRows.Count = 1) Then
                    EditPatientData()
                    e.Handled = True
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IProgPatientData_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IProgPatientData_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Form initialization when loading
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' Modified by: TR 09/11/2011 - Set the Exit button as the ACCEPT Screen Button
    '''              TR 20/04/2012 - Get the Level of the connected User
    '''              SA 08/06/2012 - When the screen is opened from WSSamplesRequest, center the screen and avoid movement
    ''' </remarks>
    Private Sub IProgPatientData_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'Get Level of the connected User
            'Dim myGlobalbase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo().UserLevel

            PrepareButtons()
            PatientSearchLoad()

            Me.AcceptButton = bsExitButton

            If (entryModeAttribute = "MENU") Then
                ResetBorder()
            Else
                Dim mySize As Size = UiAx00MainMDI.Size
                Dim myLocation As Point = UiAx00MainMDI.Location

                If (Not Me.MdiParent Is Nothing) Then
                    mySize = Me.Parent.Size
                    myLocation = Me.Parent.Location
                End If

                myNewLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
                Me.Location = myNewLocation
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IProgPatientData_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IProgPatientData_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IProgPatientData_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            ScreenStatusByUserLevel() 'TR 23/04/2012
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IProgPatientData_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IProgPatientData_Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                If (entryModeAttribute = "SEARCH") Then
                    Dim mySize As Size = UiAx00MainMDI.Size
                    Dim myLocation As Point = UiAx00MainMDI.Location
                    If (Not Me.MdiParent Is Nothing) Then
                        mySize = Me.Parent.Size
                        myLocation = Me.Parent.Location
                    End If

                    pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                    pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
                    Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
                End If
            End If
            MyBase.WndProc(m)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '***************************************'
    '* EVENTS FOR DATAGRIDVIEW OF PATIENTS *'
    '***************************************'
    ''' <summary>
    ''' To select a Patient in the grid and show its data in details area in ReadOnly mode
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 17/11/2010
    ''' Modified by: SA 23/11/2010 - Code changed from Click event to CellMouseClick event to avoid row selection
    '''                              when the click in the empty grid area or in the header row
    ''' </remarks>
    Private Sub bsPatientListDataGridView_CellMouseClick(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles bsPatientListDataGridView.CellMouseClick
        Try
            If (e.RowIndex > -1) Then
                'Verify if there are changes pending to save
                Dim cancelClick As Boolean = False
                If (bsSaveButton.Enabled) Then
                    cancelClick = (Not DiscardPendingChanges())
                End If

                If (cancelClick) Then
                    'The screen remains in its previous state, including the selected row
                    GridInProcess = True

                    Dim index As Integer = EditingPatientData.Index
                    For Each dgr As DataGridViewRow In bsPatientListDataGridView.Rows
                        dgr.Selected = False
                    Next

                    bsPatientListDataGridView.Rows(index).Selected = True
                    GridInProcess = False
                    'IR 02/10/2012 Begin change: Disable edit and delete button when editing a patient, when user makes a row change in patient list and discard pending changes
                    bsEditButton.Enabled = False
                    bsDeleteButton.Enabled = False
                Else
                    PendingToSaveChanges = False
                    If (bsPatientListDataGridView.SelectedRows.Count = 1) Then
                        'Load data of the selected Patient in the details area
                        EditingPatientData = CType(LoadSelectedPatientDataFields(), DataGridViewRow)
                    Else
                        'Clear the Details Area when there are several Patients selected and disable the Edit Button
                        ClearPatientDetails()
                        bsEditButton.Enabled = False
                    End If
                    UpdateDetailsColors(False)
                End If

                'Always control Status of Delete Button: if at least a InUse Patient is selected the button is disabled
                'IR 02/10/2012 End change: Disable edit and delete button when editing a patient, when user makes a row change in patient list and discard pending changes
                'bsDeleteButton.Enabled = True
                'bsEditButton.Enabled = True
                For Each row As DataGridViewRow In bsPatientListDataGridView.SelectedRows
                    If (Convert.ToBoolean(row.Cells("InUse").Value)) Then
                        bsDeleteButton.Enabled = False
                        bsEditButton.Enabled = False
                        Exit For
                    End If
                Next

                If bsPatientListDataGridView.SelectedRows.Count > 1 Then bsEditButton.Enabled = False

            Else
                'DL 11/10/2011
                If bsPatientListDataGridView.SelectedRows.Count > 1 Then
                    ClearPatientDetails()

                    bsDeleteButton.Enabled = False
                    bsEditButton.Enabled = False
                    bsPrintButton.Enabled = False

                    patientNamesAttribute = String.Empty
                    patientIDAttribute = String.Empty

                    EditingPatientData = Nothing
                End If
                'DL 11/10/2011
            End If

            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsPatientListDataGridView_CellMouseClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsPatientListDataGridView_CellMouseClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enables editing of the Patient Details
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' Modified by: SG 12/07/2010 - Patient selection from WSPreparation
    '''              SA 19/07/2010 - When the selected Patient is No InUse, it has to be show in ReadOnly mode
    '''              AG 04/11/2010 - Execute code only if there is a selected Patient
    '''              SA 23/11/2010 - Code changed from DoubleClick event to CellMouseDoubleClick event to avoid row selection
    '''                              when the click in the empty grid area or in the header row
    ''' </remarks>
    Private Sub bsPatientListDataGridView_CellMouseDoubleClick(ByVal sender As Object, ByVal e As DataGridViewCellMouseEventArgs) Handles bsPatientListDataGridView.CellMouseDoubleClick
        Try
            If (e.RowIndex > -1) Then
                If (entryModeAttribute = "SEARCH") Then
                    Me.Close()
                Else
                    If (bsPatientListDataGridView.SelectedRows.Count = 1) Then
                        EditPatientData()
                    End If
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsPatientListDataGridView_CellMouseDoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsPatientListDataGridView_CellMouseDoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the patient details while moving up and down arrows on rows of grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 17/11/2010 
    ''' Modified by: AG 20/12/2010 - Disable Edit Button when more than one Patient is selected
    ''' </remarks>
    Private Sub bsPatientListDataGridView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsPatientListDataGridView.KeyUp
        Try
            If (e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down) Then
                'Load data of the selected Patient in the details area
                EditingPatientData = CType(LoadSelectedPatientDataFields(), DataGridViewRow)
                UpdateDetailsColors(False)

                bsDeleteButton.Enabled = True And EnableDeleteButton
                bsEditButton.Enabled = True
                For Each row As DataGridViewRow In bsPatientListDataGridView.SelectedRows
                    If (Convert.ToBoolean(row.Cells("InUse").Value)) Then
                        bsDeleteButton.Enabled = False
                        bsEditButton.Enabled = False
                        Exit For
                    End If
                Next
                If (bsPatientListDataGridView.SelectedRows.Count > 1) Then bsEditButton.Enabled = False
            End If

            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsPatientListDataGridView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsPatientListDataGridView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete Patient when pressing Supr key 
    ''' </summary>
    ''' <remarks>
    ''' Create by:   SG 13/07/2010
    ''' Modified by: AG 20/12/2010 - When the Enter Key is pressed, Patient data is loaded in details area in edit mode
    ''' </remarks>
    Private Sub bsPatientListDataGridView_PreviewKeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsPatientListDataGridView.PreviewKeyDown
        Try
            If (e.KeyCode = Keys.Delete And Me.bsDeleteButton.Enabled) Then
                DeletePatient()
            ElseIf (e.KeyCode = Keys.Enter And Me.bsEditButton.Enabled) Then
                EditPatientData()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsPatientListDataGridView_PreviewKeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsPatientListDataGridView_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*************************************'
    '* EVENTS FOR FIELDS IN DETAILS AREA *'
    '*************************************'
    ''' <summary>
    ''' Updates the Age Units limit values
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 06/21/2010
    ''' </remarks>
    Private Sub bsAgeUnitsComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAgeUnitsComboBox.SelectedIndexChanged
        Try
            Select Case bsAgeUnitsComboBox.SelectedValue.ToString()
                Case "D"
                    LoadAgeLimitValues(FieldLimitsEnum.AGE_FROM_TO_DAYS)
                Case "M"
                    LoadAgeLimitValues(FieldLimitsEnum.AGE_FROM_TO_MONTHS)
                Case "Y"
                    LoadAgeLimitValues(FieldLimitsEnum.AGE_FROM_TO_YEARS)
            End Select
            PatientSearchData_Changed(sender, e)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsAgeUnitsComboBox_SelectedIndexChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsAgeUnitsComboBox_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsDateOfBirthMaskedTextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles bsDateOfBirthMaskedTextBox.Validating
        Try
            PendingToSaveChanges = True

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsDateOfBirthMaskedTextBox_Validating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsDateOfBirthMaskedTextBox_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the TextBox showing the error is changed
    ''' </summary>
    Private Sub bsMandatoryTextbox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsDetailsPatientIDTextBox.TextChanged, _
                                                                                                            bsDetailsFirstNameTextBox.TextChanged, _
                                                                                                            bsDetailsLastNameTextBox.TextChanged
        Try
            'Dim myTextBox As New TextBox
            Dim myTextBox As TextBox
            myTextBox = CType(sender, TextBox)

            If (myTextBox.TextLength > 0) Then
                If (bsScreenErrorProvider.GetError(myTextBox) <> String.Empty) Then
                    bsScreenErrorProvider.SetError(myTextBox, String.Empty)

                    If (myTextBox Is bsDetailsLastNameTextBox) Then
                        'Return the LastName TextBox to its original width
                        myTextBox.SetBounds(myTextBox.Location.X, myTextBox.Location.Y, myTextBox.Size.Width + 10, myTextBox.Size.Height)
                    End If
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsMandatoryTextbox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsMandatoryTextbox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To avoid entered following characters in the Numeric Up/Downs for Age From/to:
    '''   ** Minus sign
    '''   ** Dot or Comma as decimal point
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 19/07/2010
    ''' </remarks>
    Private Sub bsNumericUpDown_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles bsAgeFromNumericUpDown.KeyPress, _
                                                                                                                            bsAgeToNumericUpDown.KeyPress
        Try
            If (e.KeyChar = CChar("-") Or e.KeyChar = CChar(".") Or e.KeyChar = CChar(",")) Then e.Handled = True
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNumericUpDown_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNumericUpDown_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Controls characters not allowed in PatientID field
    ''' </summary>
    ''' <remarks>
    ''' Created by:  VR 23/04/2010
    ''' Modified by: AG 27/04/2010 - Add new parameter into ValidateSpecialCharacters method)
    '''              RH 23/06/2010 - Modified logic. Only allow alphanumeric characters and the Backspace key
    ''' </remarks>
    Private Sub bsPatientIDTextbox_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles bsPatientIDTextbox.KeyPress, bsDetailsPatientIDTextBox.KeyPress
        Try
            If (Not ValidateSpecialCharacters(e.KeyChar, "[\w\]")) Then e.Handled = True
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsPatientIDTextBox_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsPatientIDTextBox_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set variable for pending to save changes to True when there is a change in whatever of the controls in the 
    ''' details area
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 24/11/2010 
    '''              RH 18/11/2011 Remove unneeded New instructions
    ''' </remarks>
    Private Sub ControlValue_Changed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPerformedByTextBox.TextChanged, _
                                                                                                         bsGenderDetailsComboBox.SelectedIndexChanged, _
                                                                                                         bsDetailsPatientIDTextBox.TextChanged, _
                                                                                                         bsDetailsLastNameTextBox.TextChanged, _
                                                                                                         bsDetailsFirstNameTextBox.TextChanged, _
                                                                                                         bsCommentsTextBox.TextChanged
        Try
            If (Not sender Is Nothing) AndAlso (EditMode <> "") Then
                If (sender Is bsGenderDetailsComboBox) Then
                    'Dim myComboBox As New ComboBox
                    Dim myComboBox As ComboBox
                    myComboBox = CType(sender, ComboBox)
                    If (myComboBox.Enabled) Then PendingToSaveChanges = True
                Else
                    'Dim myTextBox As New TextBox
                    Dim myTextBox As TextBox
                    myTextBox = CType(sender, TextBox)
                    If (myTextBox.Enabled) Then PendingToSaveChanges = True
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ControlValue_Changed ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ControlValue_Changed", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Control availability of fields in area of search criteria
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 12/07/2010
    ''' </remarks>
    Private Sub PatientSearchData_Changed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPatientIDTextbox.TextChanged, _
                                                                                                              bsLastNameTextBox.TextChanged, _
                                                                                                              bsFirstNameTextBox.TextChanged, _
                                                                                                              bsGenderComboBox.SelectedIndexChanged, _
                                                                                                              BsDateBirthRadioButton.CheckedChanged, _
                                                                                                              BsAgeRadioButton.CheckedChanged, _
                                                                                                              bsToDateTimePicker.ValueChanged, _
                                                                                                              bsFromDateTimePicker.ValueChanged, _
                                                                                                              bsAgeToNumericUpDown.ValueChanged, _
                                                                                                              bsAgeFromNumericUpDown.ValueChanged
        Try
            Dim newData As Boolean = EnableResetButton()
            EnablingGroupBoxItems(bsDateOfBirthGroupBox, BsDateBirthRadioButton.Checked)
            EnablingGroupBoxItems(bsAgeGroupbox, BsAgeRadioButton.Checked)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PatientSearchData_Changed", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PatientSearchData_Changed", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '**********************'
    '* EVENTS FOR BUTTONS *'
    '**********************'
    ''' <summary>
    ''' Cancels edition of Patient Details data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' Modified by: DL 27/07/2010 - Change code for call to a specific function
    ''' </remarks>
    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            CancelSearchEdition()
            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Clears all the searching filters
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' </remarks>
    Private Sub bsClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsClearButton.Click
        Try
            If (Not PendingToSaveChanges OrElse _
                ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                'Area of Searching Criteria.
                bsPatientIDTextbox.Text = String.Empty
                bsGenderComboBox.SelectedIndex = 0
                bsFirstNameTextBox.Text = String.Empty
                bsLastNameTextBox.Text = String.Empty

                BsDateBirthRadioButton.Checked = False
                bsFromDateTimePicker.Checked = False
                bsToDateTimePicker.Checked = False

                BsAgeRadioButton.Checked = False
                bsAgeUnitsComboBox.SelectedIndex = 0

                bsAgeFromNumericUpDown.Value = bsAgeFromNumericUpDown.Minimum
                bsAgeToNumericUpDown.Value = bsAgeToNumericUpDown.Maximum

                bsClearButton.Enabled = False
                LoadFilteredPatientList()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsClearButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsClearButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Hides the Search panel and shows the others
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 06/21/2010
    ''' Modified by: RH 29/11/2011 When a PerfomClick() is executed, close the form, else close the search window
    ''' </remarks>
    Private Sub bsCloseSearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCloseSearchButton.Click
        Try
            If (Not Me.Tag Is Nothing) Then
                'A PerformClick() method was executed
                bsExitButton_Click(sender, e)
            Else
                'Normal button click - Execute Close Search Button code
                bsSelectPatientButton.Visible = (bsSelectPatientButton.Enabled)
                bsExitButton.Visible = True

                bsSearchCriteriaPanel.Visible = False
                bsGridPanel.Visible = True
                bsDetailsPanel.Visible = True

                'TR 09/11/2011 -Set the accept button.
                Me.AcceptButton = bsExitButton
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCloseSearchButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCloseSearchButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Deletes one or more selected Patients
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' </remarks>
    Private Sub bsDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDeleteButton.Click
        Try
            DeletePatient()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsDeleteButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsDeleteButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepares the screen for editing the selected Patient
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' Modified by: SG 02/07/2010 - Change code for call to a specific function
    ''' </remarks>
    Private Sub bsEditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsEditButton.Click
        Try
            EditPatientData()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsEditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsEditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Closes the form and discards the Patient data
    ''' </summary>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            ExitScreen()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the screen for adding a new Patient
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' Modified by: SG 02/08/2010 - Change code for call to a specific function
    ''' </remarks>
    Private Sub bsNewButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNewButton.Click
        Try
            NewPatientData()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNewButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNewButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Shows the Search panel and hides the others
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 06/21/2010
    ''' Modified by: SG 02/08/2010
    ''' </remarks>
    Private Sub bsOpenSearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsOpenSearchButton.Click
        Try
            bsGridPanel.Visible = False
            bsDetailsPanel.Visible = False

            bsSearchCriteriaPanel.Visible = True
            bsPatientIDTextbox.Focus()

            bsSelectPatientButton.Visible = False
            bsExitButton.Visible = False

            'TR 09/11/2011 -Set the accept button.
            Me.AcceptButton = bsCloseSearchButton

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsOpenSearchButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsOpenSearchButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prints the Patients Report
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 02/12/2011
    ''' </remarks>
    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            XRManager.ShowPatientsReport()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsPrintButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsPrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Clear filter fields when the search frame is open
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 09/07/2010
    ''' </remarks>
    Private Sub bsResetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsResetButton.Click
        Try
            'Area of Searching Criteria.
            bsPatientIDTextbox.Text = String.Empty
            bsGenderComboBox.SelectedIndex = 0
            bsFirstNameTextBox.Text = String.Empty
            bsLastNameTextBox.Text = String.Empty

            BsDateBirthRadioButton.Checked = False
            bsFromDateTimePicker.Checked = False
            bsToDateTimePicker.Checked = False

            BsAgeRadioButton.Checked = False
            bsAgeUnitsComboBox.SelectedIndex = 0

            bsAgeFromNumericUpDown.Value = bsAgeFromNumericUpDown.Minimum
            bsAgeToNumericUpDown.Value = bsAgeToNumericUpDown.Maximum

            FiltersInformed = False
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsResetButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsResetButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Saves data of the updated or new created Patient
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 06/21/2010
    ''' </remarks>
    Private Sub bsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSaveButton.Click
        Try
            ChangesToSave()
            ScreenStatusByUserLevel() 'TR 23/04/2012.
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSaveButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSaveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the Patient Grid according to the searching criteria
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced 
    '''                              by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub bsSearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSearchButton.Click
        Try
            bsExitButton.Visible = True
            bsClearButton.Enabled = True

            LoadFilteredPatientList()

            'RH 29/11/2011 -Set the accept button.
            Me.AcceptButton = bsExitButton

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSearchButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSearchButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Closes the form and keeps the Patient data
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 06/21/2010
    ''' </remarks>
    Private Sub bsSelectPatientButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSelectPatientButton.Click
        Try
            If (Not PendingToSaveChanges OrElse _
               (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes)) Then
                Me.Close()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSelectPatientButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSelectPatientButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region
End Class
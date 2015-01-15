Option Explicit On
Option Strict On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.CommunicationsSwFw


Public Class IWSIncompleteSamplesAuxScreen
    Inherits BSBaseForm

#Region "Declarations"
    'Global variable to store value of General Setting containing the maximum number of Patient Order Tests that can be created
    Private maxPatientOrderTests As Integer = -1

    'Global variable to store the list of Tests selected using the auxiliary screen of Tests Searching
    Private mySelectedTestsDS As New SelectedTestsDS

    'Global variable for the full path of the Import from LIMS file
    Private ReadOnly importFile As String = LIMSImportFilePath & LIMS_IMPORT_FILE_NAME

    'Global variable used to centered the screen
    Private myNewLocation As New Point

    'Global variable used to control the screen closing
    Private continueClosing As Boolean = True

    Private mdiAnalyzerCopy As AnalyzerManager
#End Region

#Region "Attributes"
    Private AnalyzerIDAttribute As String = ""
    Private WorkSessionIDAttribute As String = ""
    Private WSStatusAttribute As String = ""
    Private SourceScreenAttribute As GlobalEnumerates.SourceScreen
    Private WorkSessionResultDSAttribute As New WorkSessionResultDS
    Private ChangesMadeAttribute As Boolean = False
#End Region

#Region "Properties"
    ''' <summary>
    ''' Identifier of the Analyzer in which the WorkSession is executed
    ''' </summary>
    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Identifier of the active Work Session
    ''' </summary>
    Public WriteOnly Property WorkSessionID() As String
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Status of the active Work Session
    ''' </summary>
    Public Property WorkSessionStatus() As String
        Get
            Return WSStatusAttribute
        End Get
        Set(ByVal value As String)
            WSStatusAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' To set the screen who opened this auxiliary form. Possible values:
    '''  ** START_BUTTON   --> When the scanning was executed from button START in Session Button Bar
    '''  ** ROTOR_POS      --> When the scanning was executed from button SCANNING in WS Rotor Positioning Screen
    '''                        When the screen was opened from clicking in the correspondent button in WS Rotor Positioning Screen 
    '''  ** SAMPLE_REQUEST --> When the scanning was executed from button SCANNING in WS Samples Request Screen
    ''' </summary>
    Public WriteOnly Property SourceScreen() As GlobalEnumerates.SourceScreen
        Set(ByVal value As GlobalEnumerates.SourceScreen)
            SourceScreenAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When value of property SourceScreen is SAMPLE_REQUEST, this property contains all the Order Tests
    ''' currently requested in the active WorkSession
    ''' </summary>
    Public Property WSOrderTests() As WorkSessionResultDS
        Get
            Return WorkSessionResultDSAttribute
        End Get
        Set(ByVal value As WorkSessionResultDS)
            WorkSessionResultDSAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When value of property SourceScreen is SAMPLE_REQUEST, this property indicates if at least one incomplete Patient
    ''' Sample has been completed, to allow refreshing of the correspondent variable in the screen of WS Samples Request
    ''' </summary>
    Public ReadOnly Property ChangesMade() As Boolean
        Get
            Return ChangesMadeAttribute
        End Get
    End Property
#End Region

#Region "Methods"
    ''' <summary>
    ''' Control availability of fields in area of Manual Entering of Samples Details
    ''' </summary>
    ''' <param name="pStatus">True to set enabled fields; false to disabled them</param>
    ''' <remarks>
    ''' Created by:  SA 13/09/2011
    ''' </remarks>
    Private Sub EnterSamplesDetailsEnabled(ByVal pStatus As Boolean)
        Try
            bsSampleTypeComboBox.Enabled = pStatus
            bsSearchTestsButton.Enabled = pStatus
            bsStatCheckbox.Enabled = pStatus
            bsSaveButton.Enabled = pStatus
            bsCancelButton.Enabled = pStatus

            If (Not pStatus) Then
                bsSampleTypeComboBox.SelectedIndex = -1
                mySelectedTestsDS.Clear()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EnterSamplesDetailsEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EnterSamplesDetailsEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill a typed DataSet MaxOrderTestsValuesDS with all values required to calculate if the maximum number of Patient Order 
    ''' Tests has been exceeded in a WorkSession:
    '''   ** Max number of allowed Patient Order Tests (loaded from an Application General Setting)
    '''   ** Number of Patient Order Tests that have been requested (number of Patient Order Tests currently included in the WorkSession) 
    '''   ** Total number of Orders that has been requested (number of Incomplete Patient Samples currently selected in the grid)
    ''' </summary>
    ''' <param name="pMaxOrderTestsDS">Typed DataSet MaxOrderTestsValuesDS where the values needed to calculate if the maximum
    '''                                number of Patient Order Tests has been exceeded will be loaded</param>
    ''' <remarks>
    ''' Created by:  DL 31/08/2011
    ''' Modified by: SA 13/09/2011 - The way of getting the number of Order Tests currently requested is different depending on the 
    '''                              screen from which this auxiliary screen was opened
    ''' </remarks>
    Private Sub FillMaxOrderTestValues(ByVal pMaxOrderTestsDS As MaxOrderTestsValuesDS)
        Try
            Dim currentRequestedOrders As Integer = 0

            If (SourceScreenAttribute = GlobalEnumerates.SourceScreen.SAMPLE_REQUEST) Then
                currentRequestedOrders = WorkSessionResultDSAttribute.Patients.Rows.Count
            Else
                Dim resultData As GlobalDataTO
                Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate

                resultData = myWSOrderTestsDelegate.CountPatientOrderTests(Nothing, WorkSessionIDAttribute)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    currentRequestedOrders = DirectCast(resultData.SetDatos, Integer)
                Else
                    'Error getting the number of Patient OrderTests currently requested in the WorkSession
                    ShowMessage(Name & ".FillMaxOrderTestValues", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If
            End If

            'Fill the DS with the values needed from the auxiliary screen of Test Searching
            Dim newMaxOrderTestsRow As MaxOrderTestsValuesDS.MaxOrderTestsValuesRow

            newMaxOrderTestsRow = pMaxOrderTestsDS.MaxOrderTestsValues.NewMaxOrderTestsValuesRow
            newMaxOrderTestsRow.MaxRowsAllowed = maxPatientOrderTests
            newMaxOrderTestsRow.CurrentNumOrdersValue = bsIncompleteSamplesDataGridView.SelectedRows.Count
            newMaxOrderTestsRow.CurrentRowsNumValue = currentRequestedOrders

            pMaxOrderTestsDS.MaxOrderTestsValues.Rows.Add(newMaxOrderTestsRow)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillMaxOrderTestValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillMaxOrderTestValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the list of selected Tests for an specific PatientID/SampleID and SampleType but with the
    ''' opposite StatFlag and fill the list of Locked Tests. A Patient cannot have the same Tests/SampleTypes
    ''' requested for Stat and also for Routine 
    ''' </summary>
    ''' <param name="pPatientID">PatientID/SampleID of the selected Order</param>
    ''' <param name="pStatFlag">StatFlag of the selected Order</param>
    ''' <param name="pSampleType">Code of the SampleType currently selected</param>
    ''' <param name="pLockedTestsDS">Typed DataSet SelectedTestsDS where the list of locked Tests for the informed 
    '''                              PatientID/SampleID and SampleType and the opposite StatFlag will be loaded</param>
    ''' <remarks>
    ''' Created by:  SA 19/09/2011 - Copied from IWSSampleRequest screen and adapted
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub FillSelectedTestsForDifPriority(ByVal pPatientID As String, ByVal pStatFlag As Boolean, ByVal pSampleType As String, _
                                                ByRef pLockedTestsDS As SelectedTestsDS, ByRef pCurrentSelTestsDS As SelectedTestsDS)
        Try
            'Get all Tests requested for the same PatientID/SampleID, SampleType and the opposite StatFlag
            Dim lstWSPatientDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSPatientDS = (From a In WorkSessionResultDSAttribute.Patients _
                             Where a.SampleClass = "PATIENT" _
                           AndAlso a.SampleType = pSampleType _
                           AndAlso a.SampleID = pPatientID _
                           AndAlso a.StatFlag <> pStatFlag _
                            Select a).ToList()
            'AndAlso a.SampleID.ToUpper = pPatientID.ToUpper _

            'Load the DataSet with the list of locked Tests
            Dim newTestRow As SelectedTestsDS.SelectedTestTableRow
            Dim lstToDelete As List(Of SelectedTestsDS.SelectedTestTableRow)

            For Each patientOrderTest As WorkSessionResultDS.PatientsRow In lstWSPatientDS
                newTestRow = pLockedTestsDS.SelectedTestTable.NewSelectedTestTableRow

                newTestRow.TestType = patientOrderTest.TestType
                newTestRow.SampleType = patientOrderTest.SampleType
                newTestRow.TestID = patientOrderTest.TestID
                newTestRow.OTStatus = patientOrderTest.OTStatus

                If (Not patientOrderTest.IsTestProfileIDNull) Then
                    If (patientOrderTest.TestProfileID > 0 AndAlso patientOrderTest.TestProfileName <> "") Then
                        newTestRow.TestProfileID = patientOrderTest.TestProfileID
                        newTestRow.TestProfileName = patientOrderTest.TestProfileName
                    End If
                End If

                If (Not patientOrderTest.IsCalcTestIDNull) Then
                    If (patientOrderTest.CalcTestID <> "" AndAlso patientOrderTest.CalcTestName = "") Then
                        newTestRow.CalcTestIDs = patientOrderTest.CalcTestID
                        newTestRow.CalcTestNames = patientOrderTest.CalcTestName
                    End If
                End If
                pLockedTestsDS.SelectedTestTable.Rows.Add(newTestRow)

                'If the Test is in the list of Selected Tests, delete it (due to a problem when manages several tubes for the 
                'same Patient and without a Sample Type informed, when same SampleType but different priority is assigned to 
                'each tube)
                lstToDelete = (From b As SelectedTestsDS.SelectedTestTableRow In pCurrentSelTestsDS.SelectedTestTable _
                              Where b.SampleType = patientOrderTest.SampleType _
                            AndAlso b.TestType = patientOrderTest.TestType _
                            AndAlso b.TestID = patientOrderTest.TestID _
                             Select b).ToList

                For Each row As SelectedTestsDS.SelectedTestTableRow In lstToDelete
                    row.Delete()
                Next
                pCurrentSelTestsDS.SelectedTestTable.AcceptChanges()
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillSelectedTestsForDifPriority", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillSelectedTestsForDifPriority", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the list of currently selected Tests for an specific PatientID/SampleID, StatFlag and SampleType
    ''' </summary>
    ''' <param name="pPatientID">PatientID/SampleID of the selected Order</param>
    ''' <param name="pStatFlag">StatFlag of the selected Order</param>
    ''' <param name="pSampleType">Code of the SampleType currently selected</param>
    ''' <param name="pCurrentTestsDS">Typed DataSet SelectedTestsDS where the list of selected Tests for the 
    '''                               informed PatientID/SampleID, StatFlag and SampleType will be loaded</param>
    ''' <remarks>
    ''' Created by:  SA 19/09/2011 - Copied from IWSSampleRequest screen and adapted 
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub FillSelectedTestsForPatient(ByVal pPatientID As String, ByVal pStatFlag As Boolean, ByVal pSampleType As String, _
                                            ByRef pCurrentTestsDS As SelectedTestsDS)
        Try
            'Get all Tests requested for the same PatientID/SampleID, StatFlag and SampleType
            Dim lstWSPatientDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSPatientDS = (From a In WorkSessionResultDSAttribute.Patients _
                             Where a.SampleClass = "PATIENT" _
                           AndAlso a.SampleType = pSampleType _
                           AndAlso a.SampleID = pPatientID _
                           AndAlso a.StatFlag = pStatFlag _
                            Select a).ToList()
            'AndAlso a.SampleID.ToUpper = pPatientID.ToUpper _

            'Load the Selected Tests DataSet with the list of Patient Order Tests (for the selected SampleID/PatientID, StatFlag
            'and SampleType) currently loaded in the grid of Patients
            Dim newTestRow As SelectedTestsDS.SelectedTestTableRow
            For Each patientOrderTest As WorkSessionResultDS.PatientsRow In lstWSPatientDS
                newTestRow = pCurrentTestsDS.SelectedTestTable.NewSelectedTestTableRow

                newTestRow.Selected = True
                newTestRow.TestType = patientOrderTest.TestType
                newTestRow.SampleType = patientOrderTest.SampleType
                newTestRow.TestID = patientOrderTest.TestID
                newTestRow.OTStatus = patientOrderTest.OTStatus

                If (Not patientOrderTest.IsTestProfileIDNull) Then
                    If (patientOrderTest.TestProfileID > 0 AndAlso patientOrderTest.TestProfileName <> "") Then
                        newTestRow.TestProfileID = patientOrderTest.TestProfileID
                        newTestRow.TestProfileName = patientOrderTest.TestProfileName
                    End If
                End If

                If (Not patientOrderTest.IsCalcTestIDNull) Then
                    If (patientOrderTest.CalcTestID <> "" AndAlso patientOrderTest.CalcTestName <> "") Then
                        newTestRow.CalcTestIDs = patientOrderTest.CalcTestID
                        newTestRow.CalcTestNames = patientOrderTest.CalcTestName
                    End If
                End If
                pCurrentTestsDS.SelectedTestTable.Rows.Add(newTestRow)
            Next patientOrderTest
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillSelectedTestsForPatient", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillSelectedTestsForPatient", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application</param>
    ''' <remarks>
    ''' Created by: SA 04/08/2011
    ''' Modify by : DL 31/08/2011
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsSamplesDetailsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples_Details", pLanguageID)
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID) + ":"
            bsStatCheckbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Stat", pLanguageID)
            bsSearchTestsButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", pLanguageID) 'JB 01/10/2012 - Resource String unification

            bsSamplesListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Incomplete_Samples_List", pLanguageID)
            bsImportLISLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Import_LIS", pLanguageID)

            'For button Tooltips...
            bsScreenToolTips.SetToolTip(bsSearchTestsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestsSelection", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            bsScreenToolTips.SetToolTip(bsLIMSImportButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSPrep_LIMSImport", pLanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Execute the process of import details of incomplete Patient Samples from an external LIMS system
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 13/09/2011
    ''' </remarks>
    Private Sub ImportSamplesDetailsFromLIMS()
        Try
            Cursor = Cursors.WaitCursor

            Dim resultData As GlobalDataTO = Nothing
            Dim myOrdersDelegate As New OrdersDelegate

            'Excepting when the Source Screen is WS Samples Request, the imported data has to be added to the active WS in DB
            Dim updateWSInDB As Boolean = (SourceScreenAttribute <> GlobalEnumerates.SourceScreen.SAMPLE_REQUEST)

            resultData = myOrdersDelegate.ImportFromLIMS(Nothing, importFile, WorkSessionResultDSAttribute, AnalyzerIDAttribute, _
                                                         LIMSImportMemoPath, WorkSessionIDAttribute, WSStatusAttribute, updateWSInDB)
            If (Not resultData.HasError) Then
                If (Not resultData.SetDatos Is Nothing AndAlso updateWSInDB) Then
                    'Update status of WSStatusAttribute with content of the WS DS returned
                    Dim myWS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                    If (myWS.twksWorkSessions.Rows.Count = 1) Then WSStatusAttribute = myWS.twksWorkSessions(0).WorkSessionStatus
                End If
                ChangesMadeAttribute = True
            Else
                'Unexpected error in the import from LIMS process, show the message
                Cursor = Cursors.Default
                ShowMessage(Name & ".ImportSamplesDetailsFromLIMS", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

            Cursor = Cursors.Default
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ImportSamplesDetailsFromLIMS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ImportSamplesDetailsFromLIMS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' The value selected by default in sample type combo will depend on following conditions:
    ''' •	All selected incomplete Patient Samples do not have Sample Type: no Sample Type is selected
    '''     ** Exception: if for all the selected Incomplete Patient Samples there are requested Tests for the same SampleType, the Sample Type is selected
    ''' •	All selected incomplete Patient Samples have the same  Sample Type: the Sample Type is selected 
    ''' •	The selected incomplete Patient Samples have different Sample Types or do not have a Sample Type assigned: no Sample Type is selected in the field
    ''' This sample type combo is enabled in following conditions:
    ''' •	All selected incomplete Patient Samples do not have the Sample Type informed
    ''' •   In whatever other case, sample type combo is disabled
    ''' The value selected by default for a Stat check will depend on following conditions:
    ''' •	All selected incomplete Patient Samples are not for Stat: unchecked and enabled
    ''' •	All selected incomplete Patient Samples are for Stat: checked and enabled
    ''' •	Some of selected  incomplete Patient Samples are for Routine and other are for Stat: unchecked and disabled
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/09/2011 - Code moved from the Event. Some changes in the method logic
    ''' Modified by: TR 19/09/2011 - Clear content of the Error Provider. 
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub IncompletePatientSamplesCellMouseUp()
        Try
            Dim myStat As Boolean
            Dim mySampleType As String
            Dim dgvRow As DataGridViewRow

            bsScreenErrorProvider.Clear()

            LIMSImportButtonEnabled()
            If (bsIncompleteSamplesDataGridView.SelectedRows.Count = 0) Then
                EnterSamplesDetailsEnabled(False)
            Else
                'Verify if all selected incomplete Patient Samples have the same SampleType 
                Dim sampleTypeDifferent As Boolean = False

                mySampleType = bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("SampleType").Value.ToString
                For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                    If (mySampleType <> dgvRow.Cells("SampleType").Value.ToString) Then
                        sampleTypeDifferent = True
                        Exit For
                    End If
                Next dgvRow

                If (sampleTypeDifferent) Then
                    'It is not possible to select Tests when there are different Sample Types selected
                    bsSampleTypeComboBox.SelectedIndex = -1
                    bsSampleTypeComboBox.Enabled = False
                    bsSearchTestsButton.Enabled = False
                Else
                    If (mySampleType = String.Empty) Then
                        'Verify if there are requested Tests in the active WorkSession for the selected Incomplete Patient Samples. If all of them have 
                        'requested Tests for just one SampleType and it is the same for all, then the Sample Type is selected and the ComboBox is enabled
                        Dim mySampleID As String
                        Dim lstWSPatientDS As List(Of String)

                        For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                            If (dgvRow.Cells("PatientID").Value.ToString = String.Empty) Then
                                mySampleID = dgvRow.Cells("ExternalPID").Value.ToString
                            Else
                                mySampleID = dgvRow.Cells("PatientID").Value.ToString
                            End If

                            'Search if there are requested Tests for this Patient in the active WorkSession. Get all different SampleTypes
                            lstWSPatientDS = (From a In WorkSessionResultDSAttribute.Patients _
                                             Where a.SampleClass = "PATIENT" _
                                           AndAlso a.SampleID = mySampleID _
                                            Select a.SampleType Distinct).ToList()
                            'AndAlso a.SampleID.ToUpper = mySampleID.ToUpper _

                            If (lstWSPatientDS.Count = 1) Then
                                If (mySampleType = String.Empty) Then
                                    mySampleType = lstWSPatientDS.First.ToString

                                ElseIf (lstWSPatientDS.First.ToString <> mySampleType) Then
                                    'If a different SampleType is found, then it will not be a default value in the ComboBox
                                    mySampleType = String.Empty
                                    Exit For
                                End If
                            Else
                                'There are not requested Tests, or there are some but for different SampleTypes
                                mySampleType = String.Empty
                                Exit For
                            End If
                        Next dgvRow

                        If (mySampleType = String.Empty) Then
                            'Not all the selected Patient Samples have requested Tests for the same SampleType, no Sample Type is selected and the  ComboBox is enabled
                            bsSampleTypeComboBox.SelectedIndex = -1
                            bsSampleTypeComboBox.Enabled = True
                        Else
                            'All the selected Patient Samples have requested Tests for the same SampleType, it is selected and the ComboBox is disabled
                            bsSampleTypeComboBox.SelectedValue = mySampleType
                            bsSampleTypeComboBox.Enabled = True
                        End If
                    Else
                        'If the selected Patient Samples have a SampleType informed, it is selected and the ComboBox is disabled
                        bsSampleTypeComboBox.SelectedValue = mySampleType
                        bsSampleTypeComboBox.Enabled = False
                    End If
                    bsSearchTestsButton.Enabled = True
                End If

                'Verify if all selected incomplete Patient Samples have the same StatFlag
                Dim statDifferent As Boolean = False

                myStat = CType(bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("StatFlag").Value, Boolean)
                For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                    If (myStat <> CType(dgvRow.Cells("StatFlag").Value, Boolean)) Then
                        statDifferent = True
                        Exit For
                    End If
                Next dgvRow

                If (statDifferent) Then
                    'If the selected Patient Samples have different StatFlag values, it is not possible to change it
                    bsStatCheckbox.Checked = False
                    bsStatCheckbox.Enabled = False
                Else
                    'If the selected Patient Samples have the same StatFlag value, it is possible to change it o
                    bsStatCheckbox.Checked = myStat
                    bsStatCheckbox.Enabled = True
                End If
            End If

            bsSaveButton.Enabled = (bsSearchTestsButton.Enabled OrElse bsStatCheckbox.Enabled)
            bsCancelButton.Enabled = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsIncompleteSamplesDataGridView_MouseUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsIncompleteSamplesDataGridView_MouseUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the Incomplete Samples DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/08/2011
    ''' Modified by: SA 13/09/2011 - Removed column CellNumber from the grid due to it is possible to have more than one 
    '''                              tube with the same PatientSample, but each Patient Sample has to be shown once in the grid
    '''              TR 19/09/2011 - Add new column showing Code and Description of the Sample Type (when informed). Hide the
    '''                              column for the SampleType Code
    '''              SA 19/09/2011 - Included again the CellNumber column as hidden
    ''' </remarks>
    Private Sub InitializeIncompleteSamplesGrid(ByVal pLanguageID As String)
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsIncompleteSamplesDataGridView.AutoGenerateColumns = False

            'Rotor Type
            columnName = "RotorType"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, columnName)
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False

            'Cell Number
            columnName = "CellNumber"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, columnName)
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False

            'External Sample ID
            columnName = "ExternalPID"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_External_SampleID", pLanguageID))
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).ReadOnly = True
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 200
            bsIncompleteSamplesDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsIncompleteSamplesDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable

            'Sample Type
            columnName = "SampleType"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, columnName)
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False

            'TR 19/09/2011 -Sample Type Description
            columnName = "SampleTypeDesc"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID))
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).ReadOnly = True
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 200
            bsIncompleteSamplesDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsIncompleteSamplesDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable

            'Stat column
            Dim checkBoxColumnCheckValue As New DataGridViewCheckBoxColumn
            columnName = "StatFlag"
            checkBoxColumnCheckValue.Name = columnName
            checkBoxColumnCheckValue.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Stat", pLanguageID) '"Check On/Off"
            checkBoxColumnCheckValue.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            bsIncompleteSamplesDataGridView.Columns.Add(checkBoxColumnCheckValue)
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 70
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).ReadOnly = False
            bsIncompleteSamplesDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsIncompleteSamplesDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Patient Identifier
            columnName = "PatientID"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, columnName)
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeIncompleteSamplesGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeIncompleteSamplesGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill ComboBox of Sample Types
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeSampleTypeComboBox()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myMasterDataDelegate As New MasterDataDelegate

            myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMasterDataDS As MasterDataDS = DirectCast(myGlobalDataTO.SetDatos, MasterDataDS)
                If (myMasterDataDS.tcfgMasterData.Rows.Count > 0) Then
                    Dim lstSorted As List(Of MasterDataDS.tcfgMasterDataRow) = (From a In myMasterDataDS.tcfgMasterData _
                                                                            Order By a.Position _
                                                                              Select a).ToList()
                    bsSampleTypeComboBox.DataSource = lstSorted
                    bsSampleTypeComboBox.DisplayMember = "ItemIDDesc"
                    bsSampleTypeComboBox.ValueMember = "ItemID"
                    lstSorted = Nothing

                    'No element is selected by default in the ComboBox
                    bsSampleTypeComboBox.SelectedIndex = -1
                End If
            Else
                'Error getting the list of available Sample Types
                ShowMessage(Name & ".InitializeSampleTypeComboBox", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeSampleTypeComboBox", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeSampleTypeComboBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if button for Import from LIMS can be enabled - For basic LIMS implementation
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/09/2011
    ''' </remarks>
    Private Sub LIMSImportButtonEnabled()
        Try
            bsLIMSImportButton.Enabled = (IO.File.Exists(importFile)) AndAlso _
                                         (bsIncompleteSamplesDataGridView.SelectedRows.Count = 0 OrElse _
                                          bsIncompleteSamplesDataGridView.SelectedRows.Count = bsIncompleteSamplesDataGridView.Rows.Count)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LIMSImportButtonEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LIMSImportButtonEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the list of Patient Samples marked as incompleted after scanning
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/08/2011
    ''' Modified by: TR 19/09/2011 - Get the Sample Type Description
    '''              SA 11/06/2012 - When the grid is empty, set attribute ChangesMade to True to avoid errors 
    '''                              when the WS is saved in the screen of WSSamplesRequest
    ''' </remarks>
    Private Sub LoadIncompleteSamplesGrid()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myBarcodePositionsWithNoRequestsDelegate As New BarcodePositionsWithNoRequestsDelegate

            myGlobalDataTO = myBarcodePositionsWithNoRequestsDelegate.ReadDistinctPatientSamples(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS = DirectCast(myGlobalDataTO.SetDatos, BarcodePositionsWithNoRequestsDS)

                'TR 19/09/2011
                Dim myMasterDataDelegate As New MasterDataDelegate
                myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myMasterDataDS As MasterDataDS = DirectCast(myGlobalDataTO.SetDatos, MasterDataDS)
                    Dim qMasterData As New List(Of MasterDataDS.tcfgMasterDataRow)
                    'Get the description by LINQ
                    For Each bcpRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow _
                                    In myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows
                        qMasterData = (From a In myMasterDataDS.tcfgMasterData _
                                      Where a.ItemID = bcpRow.SampleType Select a).ToList()

                        If (qMasterData.Count > 0) Then bcpRow.SampleTypeDesc = qMasterData.First().ItemIDDesc
                    Next
                End If
                'TR 19/09/2011 -END

                bsIncompleteSamplesDataGridView.DataSource = myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests

                If (myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows.Count > 0) Then
                    'Unselect first row in DataGridView
                    bsIncompleteSamplesDataGridView.ClearSelection()
                Else
                    'When the grid is empty, set attribute ChangesMade to True to avoid errors when the WS is saved
                    'in the screen of WSSamplesRequest
                    ChangesMadeAttribute = True
                End If
            Else
                'Error getting the list of Incomplete Patient Samples
                ShowMessage(Name & ".LoadIncompleteSamplesGrid", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadIncompleteSamplesGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadIncompleteSamplesGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to load the image for each graphical button 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'IMPORT FROM LIMS Button
            auxIconName = GetIconName("LIMSIMPORT")
            If (auxIconName <> "") Then
                bsLIMSImportButton.Image = Image.FromFile(iconPath & auxIconName)
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

            'CLOSE Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Assign Stat Flag, SampleType and list of Tests to the selected Incomplete Patient Samples. 
    '''   ** If the screen has been opened from the screen of WS Sample Request (SourceScreen = SAMPLE_REQUEST), the Patient
    '''      Order Tests and the needed Blanks, Calibrators and Controls are added to the WorkSessionResultsDS used to load
    '''      the grids in WS Sample Request screen.  All scanned tubes of Patient Samples are saved as NotInUse rotor positions
    '''   ** If the screen has been opened from the screen of WS Rotor Positioning (SourceScreen = ROTOR_POS) or from the Start 
    '''      Button (SourceScreen = START_BUTTON), the Patient Order Tests and the needed Blanks, Calibrators and Controls are 
    '''      added to the existing WorkSession
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/08/2011
    ''' Modified by: SA 13/09/2011 - The saving process is different according value of property SourceScreen. Changed to function
    '''                              returning a Boolean to refresh the screen only when the saving finishes successfully
    '''              TR 19/09/2011 - Validations of missing required field improved
    '''              SA 20/09/2011 - Changes to save data when SourceScreen is IWSSampleRequest
    '''              SA 11/10/2011 - When for the selected Sample Type and StatFlag there are previously requested Tests, it is not 
    '''                              mandatory to select more (it is not needed to open the auxiliary screen of Tests Selection)
    '''              SA 11/06/2012 - When there are several selected SampleIDs and for all of them there are previously requested
    '''                              Tests for the selected Sample Type and StatFlag, it is not mandatory to select more (it is not
    '''                              needed to open the auxiliary screen of Tests Selection)
    ''' </remarks>
    Private Function SaveChanges() As Boolean
        Dim continueSaving As Boolean = False

        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myBarcodePositionsWithNoRequests As New BarcodePositionsWithNoRequestsDelegate
            Dim myBarcodePositionsWithNoRequestsDS As New BarcodePositionsWithNoRequestsDS

            Cursor = Cursors.WaitCursor
            bsScreenErrorProvider.Clear()

            If (bsSearchTestsButton.Enabled) Then
                If (bsSampleTypeComboBox.SelectedValue Is Nothing) Then
                    'It is mandatory to inform a Sample Type...
                    bsScreenErrorProvider.SetError(bsSampleTypeComboBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                    myGlobalDataTO.HasError = True
                Else
                    If (mySelectedTestsDS Is Nothing OrElse mySelectedTestsDS.Tables(0).Rows.Count = 0) Then
                        Dim mySampleID As String = bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("ExternalPID").Value.ToString

                        'Verify if all selected rows have the same ExternalID
                        For Each dgvRow As DataGridViewRow In bsIncompleteSamplesDataGridView.SelectedRows
                            If (dgvRow.Cells("ExternalPID").Value.ToString <> mySampleID) Then
                                mySampleID = String.Empty
                                Exit For
                            End If
                        Next dgvRow

                        If (mySampleID <> String.Empty) Then
                            If (bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("PatientID").Value.ToString <> String.Empty) Then
                                mySampleID = bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("PatientID").Value.ToString
                            End If

                            FillSelectedTestsForPatient(mySampleID, bsStatCheckbox.Checked, bsSampleTypeComboBox.SelectedValue.ToString, mySelectedTestsDS)
                            If (mySelectedTestsDS Is Nothing OrElse mySelectedTestsDS.Tables(0).Rows.Count = 0) Then
                                'It is mandatory to select at least a Standard or ISE Test
                                bsScreenErrorProvider.SetError(bsSearchTestsButton, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                                myGlobalDataTO.HasError = True
                            Else
                                'It is mandatory to select at least a Standard or ISE Test
                                Dim lstValidTests As List(Of SelectedTestsDS.SelectedTestTableRow) = (From a As SelectedTestsDS.SelectedTestTableRow In mySelectedTestsDS.SelectedTestTable _
                                                                                                     Where a.TestType <> "OFFS" Select a).ToList

                                If (lstValidTests.Count = 0) Then
                                    'It is mandatory to select at least a Standard or ISE Test
                                    bsScreenErrorProvider.SetError(bsSearchTestsButton, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                                    myGlobalDataTO.HasError = True
                                Else
                                    continueSaving = True
                                End If
                                lstValidTests = Nothing
                            End If
                        Else
                            If (Not VerifyAtLeastATestBySampleID()) Then
                                'It is mandatory to select at least a Standard or ISE Test for each selected Patient
                                bsScreenErrorProvider.SetError(bsSearchTestsButton, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                                myGlobalDataTO.HasError = True
                            Else
                                continueSaving = True
                            End If
                        End If
                    Else
                        'It is mandatory to select at least a Standard or ISE Test
                        Dim lstValidTests As List(Of SelectedTestsDS.SelectedTestTableRow) = (From a As SelectedTestsDS.SelectedTestTableRow In mySelectedTestsDS.SelectedTestTable _
                                                                                             Where a.TestType <> "OFFS" Select a).ToList

                        If (lstValidTests.Count = 0) Then
                            'It is mandatory to select at least a Standard or ISE Test
                            bsScreenErrorProvider.SetError(bsSearchTestsButton, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                            myGlobalDataTO.HasError = True
                        Else
                            continueSaving = True
                        End If
                        lstValidTests = Nothing
                    End If
                End If
            Else
                If (bsStatCheckbox.Enabled) Then
                    'Update the StatFlag of the selected Incomplete Patient Samples
                    Dim dgvRow As DataGridViewRow
                    Dim barcodePositionsWithNoRequestsRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow

                    For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                        'Add data to myBarcodePositionsWithNoRequestsDS
                        barcodePositionsWithNoRequestsRow = myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.NewtwksWSBarcodePositionsWithNoRequestsRow
                        barcodePositionsWithNoRequestsRow.WorkSessionID = WorkSessionIDAttribute
                        barcodePositionsWithNoRequestsRow.AnalyzerID = AnalyzerIDAttribute
                        barcodePositionsWithNoRequestsRow.RotorType = dgvRow.Cells("RotorType").Value.ToString
                        barcodePositionsWithNoRequestsRow.ExternalPID = dgvRow.Cells("ExternalPID").Value.ToString

                        If (dgvRow.Cells("SampleType").Value.ToString = String.Empty) Then
                            barcodePositionsWithNoRequestsRow.SetSampleTypeNull()
                        Else
                            barcodePositionsWithNoRequestsRow.SampleType = dgvRow.Cells("SampleType").Value.ToString
                        End If

                        If (Convert.ToInt32(dgvRow.Cells("CellNumber").Value) = 0) Then
                            barcodePositionsWithNoRequestsRow.SetCellNumberNull()
                        Else
                            barcodePositionsWithNoRequestsRow.CellNumber = Convert.ToInt32(dgvRow.Cells("CellNumber").Value)
                        End If

                        barcodePositionsWithNoRequestsRow.StatFlag = bsStatCheckbox.Checked
                        myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows.Add(barcodePositionsWithNoRequestsRow)
                    Next dgvRow

                    myGlobalDataTO = myBarcodePositionsWithNoRequests.UpdateStatFlag(Nothing, myBarcodePositionsWithNoRequestsDS)
                    If (myGlobalDataTO.HasError) Then
                        'Error updating the StatFlag of the selected Incomplete Patient Samples
                        ShowMessage(Name & ".SaveChanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                End If
            End If

            If (continueSaving) Then
                If (Not myGlobalDataTO.HasError) Then
                    'Get the selected StatFlag and SampleType 
                    Dim myStatFlag As Boolean = bsStatCheckbox.Checked
                    Dim mySampleType As String = bsSampleTypeComboBox.SelectedValue.ToString

                    Dim myPID As String
                    Dim myPIDType As String
                    Dim dgvRow As DataGridViewRow
                    Dim myOrderTestsDelegate As New OrderTestsDelegate
                    Dim barcodePositionsWithNoRequestsRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow

                    For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                        If (dgvRow.Cells("PatientID").Value.ToString = String.Empty) Then
                            myPIDType = "MAN"
                            myPID = dgvRow.Cells("ExternalPID").Value.ToString
                        Else
                            myPIDType = "DB"
                            myPID = dgvRow.Cells("PatientID").Value.ToString
                        End If

                        If (Not mySelectedTestsDS Is Nothing) Then
                            If (SourceScreenAttribute = GlobalEnumerates.SourceScreen.SAMPLE_REQUEST) Then
                                'Delete all Open Patient Order Tests that are not in the list of selected Tests for the active StatFlag, Sample Type, SampleID
                                myGlobalDataTO = myOrderTestsDelegate.DeletePatientOrderTests("NOT_IN_LIST", mySelectedTestsDS, WorkSessionResultDSAttribute, _
                                                                                              mySampleType, myPID, myStatFlag)
                                'If there are Order Tests to add...
                                If (Not myGlobalDataTO.HasError AndAlso mySelectedTestsDS.SelectedTestTable.Rows.Count > 0) Then
                                    'Add all Patient Order Tests of the selected Tests for the active Sample Type
                                    myGlobalDataTO = myOrderTestsDelegate.AddPatientOrderTests(mySelectedTestsDS, WorkSessionResultDSAttribute, AnalyzerIDAttribute, _
                                                                                               myPID, myStatFlag, myPIDType)
                                End If
                            Else
                                'Add all Patient Order Tests of the selected Tests for the active Sample Type
                                myGlobalDataTO = myOrderTestsDelegate.AddPatientOrderTests(mySelectedTestsDS, WorkSessionResultDSAttribute, AnalyzerIDAttribute, _
                                                                                           myPID, myStatFlag, myPIDType)

                            End If
                        End If
                        If (myGlobalDataTO.HasError) Then Exit For

                        'Add data to myBarcodePositionsWithNoRequestsDS
                        barcodePositionsWithNoRequestsRow = myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.NewtwksWSBarcodePositionsWithNoRequestsRow
                        barcodePositionsWithNoRequestsRow.WorkSessionID = WorkSessionIDAttribute
                        barcodePositionsWithNoRequestsRow.AnalyzerID = AnalyzerIDAttribute
                        barcodePositionsWithNoRequestsRow.RotorType = dgvRow.Cells("RotorType").Value.ToString
                        barcodePositionsWithNoRequestsRow.CellNumber = Convert.ToInt32(dgvRow.Cells("CellNumber").Value)
                        barcodePositionsWithNoRequestsRow.ExternalPID = dgvRow.Cells("ExternalPID").Value.ToString
                        barcodePositionsWithNoRequestsRow.PatientID = dgvRow.Cells("PatientID").Value.ToString
                        barcodePositionsWithNoRequestsRow.NotSampleType = (dgvRow.Cells("SampleType").Value.ToString = String.Empty)
                        barcodePositionsWithNoRequestsRow.SampleType = mySampleType

                        If (Not bsStatCheckbox.Enabled) Then myStatFlag = DirectCast(dgvRow.Cells("StatFlag").Value, Boolean)
                        barcodePositionsWithNoRequestsRow.StatFlag = myStatFlag

                        myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows.Add(barcodePositionsWithNoRequestsRow)
                    Next dgvRow
                End If

                If (Not myGlobalDataTO.HasError) Then
                    If (SourceScreenAttribute <> GlobalEnumerates.SourceScreen.SAMPLE_REQUEST) Then
                        myGlobalDataTO = myBarcodePositionsWithNoRequests.ProcessCompletedPatientSamples(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, WSStatusAttribute, _
                                                                                                         WorkSessionResultDSAttribute, myBarcodePositionsWithNoRequestsDS)

                        'Update status of WSStatusAttribute with content of the WS DS returned
                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myWS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)
                            If (myWS.twksWorkSessions.Rows.Count = 1) Then WSStatusAttribute = myWS.twksWorkSessions(0).WorkSessionStatus

                            ChangesMadeAttribute = True
                        End If
                    Else
                        myGlobalDataTO = myBarcodePositionsWithNoRequests.ProcessCompletedPatientSamples(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, WSStatusAttribute, _
                                                                                                         Nothing, myBarcodePositionsWithNoRequestsDS)
                        ChangesMadeAttribute = True
                    End If

                    If (myGlobalDataTO.HasError) Then
                        'Error adding all new elements to the Work Session
                        ShowMessage(Name & ".SaveChanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                Else
                    'Error adding the requested Patient Order Tests and related elements
                    ShowMessage(Name & ".SaveChanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            End If

            continueSaving = (Not myGlobalDataTO.HasError)
            Cursor = Cursors.Default
        Catch ex As Exception
            continueSaving = False
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return continueSaving
    End Function

    ''' <summary>
    ''' Screen initialization
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 29/08/2012 - Inform the AnalyzerID when calling function GetOrderTestsForWS in WorkSessionsDelegate
    ''' </remarks>
    Private Sub ScreenLoad()
        Dim myGlobalDataTO As GlobalDataTO = Nothing
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

            mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) 'AG 16/06/2011 - Use the same AnalyzerManager as the MDI
            mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS) = 0

            'Load buttons images
            PrepareButtons()

            'Get multilanguage labels for all screen controls
            GetScreenLabels(currentLanguage)

            'Load ComboBox of Sample Types
            InitializeSampleTypeComboBox()

            'Initializes the Incomplete Samples DataGridView
            InitializeIncompleteSamplesGrid(currentLanguage)

            'Get the list of Incomplete Samples to load the DataGridView
            LoadIncompleteSamplesGrid()

            'Disable all controls in both areas: Import from LIS and Enter Samples Details
            LIMSImportButtonEnabled()
            EnterSamplesDetailsEnabled(False)

            'Get value of General Setting containing the maximum number of Patient Order Tests that can be created
            Dim myUserSettingsDelegate As New GeneralSettingsDelegate
            myGlobalDataTO = myUserSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.MAX_PATIENT_ORDER_TESTS.ToString)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                'Save value in global variable maxPatientOrderTests
                maxPatientOrderTests = CType(myGlobalDataTO.SetDatos, Integer)
            Else
                'Error getting value of General Setting for the max quantity of allowed Patient Order Tests in a WorkSession
                ShowMessage(Name & ".ScreenLoad", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

            'If there is an active WS with requested Order Tests, then get all of them
            If (Not myGlobalDataTO.HasError) Then
                If (SourceScreenAttribute <> GlobalEnumerates.SourceScreen.SAMPLE_REQUEST) Then
                    If (WSStatusAttribute.Trim <> "EMPTY") Then
                        Dim myWSDelegate As New WorkSessionsDelegate

                        myGlobalDataTO = myWSDelegate.GetOrderTestsForWS(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            WorkSessionResultDSAttribute = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)
                        Else
                            'Error getting the list of OrderTests currently included in the WorkSession
                            ShowMessage(Name & ".ScreenLoad", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow select/unselect Tests for the informed SampleType
    ''' for the selected Incomplete Patient Samples.
    ''' For event bsSearchTests_Click
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 
    ''' Modified by: TR 19/09/2011 - Validate if there's a Sample Type selected before open the auxiliary screen
    '''                              If not, show the Required error message
    '''              SA 19/09/2011 - When there is an unique Patient/Sample Type selected verify also if there are tests
    '''                              locked due to they have been requested for the same Patient/Sample Type but with 
    '''                              different priority  
    '''              SA 21/09/2011 - When the auxiliary screen of Tests selection is closed, verify if there are Tests 
    '''                              already requested for the same Patient/SampleType and StatFlag and in that case,
    '''                              disable the StatFlag checkbox. This is to avoid following situation: 
    '''                               ** There are two tubes for the same Patient but without SampleType 
    '''                               ** Select a tube, assign SampleType, set Stat=True, select some tests and Save
    '''                               ** Select the another tube and assign the same SampleType and Stat than the previous, and
    '''                                  change the list of selected Tests (the ones selected for the first tube are shown marked
    '''                                  and selected and the user can unmark some of them and also add more tests)
    '''                               ** Try to change now the StatFlag --> This has to be avoided; possibilities:
    '''                                  1) Don't allow change the priority (disable the field) - IMPLEMENTED
    '''                                  2) Allow change the priority and change it also for the previous tube
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub SearchTests()
        Try
            bsScreenErrorProvider.Clear()
            If (bsSampleTypeComboBox.SelectedIndex >= 0) Then
                Dim lockedTests As New SelectedTestsDS
                Dim mySampleID As String = bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("ExternalPID").Value.ToString

                'Verify if all selected rows have the same ExternalID
                For Each dgvRow As DataGridViewRow In bsIncompleteSamplesDataGridView.SelectedRows
                    If (dgvRow.Cells("ExternalPID").Value.ToString <> mySampleID) Then
                        mySampleID = String.Empty
                        Exit For
                    End If
                Next dgvRow

                If (mySampleID <> String.Empty) Then
                    If (bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("PatientID").Value.ToString <> String.Empty) Then
                        mySampleID = bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("PatientID").Value.ToString
                    End If

                    If (mySelectedTestsDS Is Nothing OrElse mySelectedTestsDS.SelectedTestTable.Rows.Count = 0) Then
                        FillSelectedTestsForPatient(mySampleID, bsStatCheckbox.Checked, bsSampleTypeComboBox.SelectedValue.ToString, mySelectedTestsDS)
                    End If
                    FillSelectedTestsForDifPriority(mySampleID, bsStatCheckbox.Checked, bsSampleTypeComboBox.SelectedValue.ToString, lockedTests, mySelectedTestsDS)
                End If

                'Load the Typed DataSet MaxOrderTestsValuesDS before opening the auxiliary screen
                Dim myMaxOrderTestsDS As New MaxOrderTestsValuesDS
                FillMaxOrderTestValues(myMaxOrderTestsDS)

                'Inform properties and open the screen of Tests Selection
                Using myForm As New IWSTestSelectionAuxScreen()
                    myForm.SampleClass = "PATIENT"
                    myForm.SampleType = bsSampleTypeComboBox.SelectedValue.ToString()
                    myForm.SampleTypeName = bsSampleTypeComboBox.Text
                    myForm.ListOfSelectedTests = mySelectedTestsDS
                    myForm.MaxValues = myMaxOrderTestsDS

                    If (mySampleID <> String.Empty) Then
                        myForm.PatientID = mySampleID
                        myForm.SelectedTestsInDifPriority = lockedTests
                    End If

                    myForm.ShowDialog()
                    If (myForm.DialogResult = Windows.Forms.DialogResult.OK) Then
                        mySelectedTestsDS = myForm.ListOfSelectedTests

                        'Get all Tests requested for the same PatientID/SampleID, StatFlag and SampleType
                        Dim lstWSPatientDS As List(Of WorkSessionResultDS.PatientsRow)
                        lstWSPatientDS = (From a In WorkSessionResultDSAttribute.Patients _
                                          Where a.SampleClass = "PATIENT" _
                                          AndAlso a.SampleType = bsSampleTypeComboBox.SelectedValue.ToString() _
                                          AndAlso a.SampleID = mySampleID _
                                          AndAlso a.StatFlag = bsStatCheckbox.Checked _
                                          Select a).ToList()
                        'AndAlso a.SampleID.ToUpper = mySampleID.ToUpper _

                        'If there are OrderTests requested for the same Patient/SampleType/StatFlag, then the StatFlag
                        'cannot be changed (this case is only for management of several tubes for the same Patient but
                        'without SampleType informed)
                        If (lstWSPatientDS.Count > 0) Then bsStatCheckbox.Enabled = False

                        'Disable the grid
                        bsIncompleteSamplesDataGridView.Enabled = False
                    End If

                End Using
            Else
                bsScreenErrorProvider.SetError(bsSampleTypeComboBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SearchTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SearchTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if, for all SampleIDs selected in the grid, there is at least an Standard or ISE Test requested 
    ''' for the selected SampleType and StatFlag. Used to avoid showing the error message of pending Tests selection
    ''' when the auxiliary screen has not been opened but for all SampleID/StatFlag/SampleType they are 
    ''' already Tests requested in the WorkSession. Besides, fill in the dataset SelectedTestsDS all Tests requested
    ''' for each one of the selected SampleIDs 
    ''' </summary>
    ''' <returns>True if there is at least a requested Standard or ISE Test for all selected SampleID/StatFlag/SampleType;
    '''          otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  SA 11/06/2012
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function VerifyAtLeastATestBySampleID() As Boolean
        Dim atLeastATest As Boolean = False

        Try
            Dim mySampleID As String = String.Empty
            Dim lstPatientTests As List(Of SelectedTestsDS.SelectedTestTableRow)

            For Each dgvRow As DataGridViewRow In bsIncompleteSamplesDataGridView.SelectedRows
                mySampleID = dgvRow.Cells("ExternalPID").Value.ToString
                If (dgvRow.Cells("PatientID").Value.ToString <> String.Empty) Then
                    mySampleID = dgvRow.Cells("PatientID").Value.ToString
                End If

                'Load all Tests selected for the SampleID in mySelectedTestsDS
                FillSelectedTestsForPatient(mySampleID, bsStatCheckbox.Checked, bsSampleTypeComboBox.SelectedValue.ToString(), _
                                            mySelectedTestsDS)

                'Verify if at least an STD or ISE Test have been requested for the SampleID/StatFlag with the selected SampleType
                lstPatientTests = (From a As SelectedTestsDS.SelectedTestTableRow In mySelectedTestsDS.SelectedTestTable _
                                  Where a.SampleType = bsSampleTypeComboBox.SelectedValue.ToString() _
                                AndAlso a.SampleID = mySampleID _
                                AndAlso a.StatFlag = bsStatCheckbox.Checked _
                                AndAlso a.TestType <> "OFFS" _
                                 Select a).ToList()
                'AndAlso a.SampleID.ToUpper = mySampleID.ToUpper _

                atLeastATest = (lstPatientTests.Count > 0)
                If (Not atLeastATest) Then Exit For
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".VerifyAtLeastATestBySampleID", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".VerifyAtLeastATestBySampleID", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return atLeastATest
    End Function

#End Region

#Region "Events"
    '*************************************************************
    '* EVENTS FOR BUTTONS FOR MANUAL ENTERING OF SAMPLES DETAILS *
    '*************************************************************
    ''' <summary>
    ''' Open the auxiliary screen for Tests selection
    ''' </summary>
    Private Sub bsSearchTestsButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSearchTestsButton.Click
        Try
            SearchTests()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSearchTestsButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSearchTestsButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save the manually entered values for StatFlag, Sample Type and list of Tests for all selected Incomplete Patient Samples
    ''' </summary>
    Private Sub bsSaveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSaveButton.Click
        Try
            If (SaveChanges()) Then
                'Get the list of Incomplete Samples to load the DataGridView
                LoadIncompleteSamplesGrid()

                'Disable all controls in both areas: Import from LIS and Enter Samples Details
                LIMSImportButtonEnabled()
                EnterSamplesDetailsEnabled(False)

                'Enable again the grid
                bsIncompleteSamplesDataGridView.Enabled = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSaveButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSaveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Cancel the manually entered values for all selected Incomplete Patient Samples
    ''' </summary>
    Private Sub bsCancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            If (Not mySelectedTestsDS Is Nothing AndAlso mySelectedTestsDS.SelectedTestTable.Rows.Count > 0) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    'Disable all controls in both areas: Import from LIS and Enter Samples Details
                    LIMSImportButtonEnabled()
                    EnterSamplesDetailsEnabled(False)
                    bsIncompleteSamplesDataGridView.ClearSelection()

                    'Enable again the grid
                    bsIncompleteSamplesDataGridView.Enabled = True
                Else
                    bsSaveButton.Focus()
                End If
            Else
                bsScreenErrorProvider.Clear()

                'Disable all controls in both areas: Import from LIS and Enter Samples Details
                LIMSImportButtonEnabled()
                EnterSamplesDetailsEnabled(False)
                bsIncompleteSamplesDataGridView.ClearSelection()

                'Enable again the grid
                bsIncompleteSamplesDataGridView.Enabled = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*************************************
    '* EVENTS FOR BUTTON IMPORT FROM LIS *
    '*************************************
    ''' <summary>
    ''' Execute the process of import details of the selected Incomplete Patient Samples from LIS
    ''' </summary>
    Private Sub bsLIMSImportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsLIMSImportButton.Click
        Try
            EnterSamplesDetailsEnabled(False)
            ImportSamplesDetailsFromLIMS()

            LoadIncompleteSamplesGrid()
            LIMSImportButtonEnabled()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsLIMSImportButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsLIMSImportButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*****************************************************
    '* EVENTS FOR THE GRID OF INCOMPLETE PATIENT SAMPLES *
    '*****************************************************
    ''' <summary>
    ''' Select/unselect all rows in the grid of Incomplete Patient Samples when click/double click in the ExternalID column
    ''' </summary>
    Private Sub bsIncompleteSamplesDataGridView_ColumnHeaderMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsIncompleteSamplesDataGridView.ColumnHeaderMouseClick, _
                                                                                                                                                                       bsIncompleteSamplesDataGridView.ColumnHeaderMouseDoubleClick
        Try
            If (bsIncompleteSamplesDataGridView.Rows.Count = 0) Then Exit Sub
            If (bsIncompleteSamplesDataGridView.Rows(0).Cells("ExternalPID").ColumnIndex = e.ColumnIndex) Then
                If (bsIncompleteSamplesDataGridView.SelectedRows.Count = 0) Then
                    bsIncompleteSamplesDataGridView.SelectAll()
                Else
                    bsIncompleteSamplesDataGridView.ClearSelection()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_ColumnHeaderMouseClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_ColumnHeaderMouseClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the selection/unselection of all 
    ''' </summary>
    Private Sub bsIncompleteSamplesDataGridView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsIncompleteSamplesDataGridView.KeyUp
        Try
            Select Case (e.KeyCode)
                Case Keys.Up
                    If (bsIncompleteSamplesDataGridView.CurrentRow.Index >= 0) Then
                        bsIncompleteSamplesDataGridView_CellMouseUp(Nothing, Nothing)
                    End If
                Case Keys.Down
                    If (bsIncompleteSamplesDataGridView.CurrentRow.Index < bsIncompleteSamplesDataGridView.Rows.Count) Then
                        bsIncompleteSamplesDataGridView_CellMouseUp(Nothing, Nothing)
                    End If
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the select/unselect of Incomplete Patient Samples in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/08/2011
    ''' Modified by: SA 13/09/2011 - Code moved to a function
    ''' </remarks>
    Private Sub bsIncompleteSamplesDataGridView_CellMouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsIncompleteSamplesDataGridView.CellMouseUp
        Try
            IncompletePatientSamplesCellMouseUp()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_CellMouseUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_CellMouseUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Change the BackColor of rows in the grid of Incomplete Patient Samples according if they are
    ''' enabled or disabled
    ''' </summary>
    Private Sub bsIncompleteSamplesDataGridView_EnabledChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsIncompleteSamplesDataGridView.EnabledChanged
        Try
            Dim backColor As New Color
            Dim letColor As New Color

            If (Not bsIncompleteSamplesDataGridView.Enabled) Then
                backColor = SystemColors.MenuBar
                letColor = Color.DarkGray
            Else
                backColor = Color.White
                letColor = Color.Black
            End If

            For Each row As DataGridViewRow In bsIncompleteSamplesDataGridView.Rows
                row.DefaultCellStyle.BackColor = backColor
                row.DefaultCellStyle.ForeColor = letColor
            Next row
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_EnabledChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_EnabledChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*****************
    '* OTHER EVENTS *
    '*****************
    ''' <summary>
    ''' If continueClosing has been set to False, the form closing is cancelled
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/09/2011 
    ''' </remarks>
    Private Sub IWSIncompleteSamplesAuxScreen_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            e.Cancel = (Not continueClosing)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IWSIncompleteSamplesAuxScreen_FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IWSIncompleteSamplesAuxScreen_FormClosing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 14/09/2011 
    ''' </remarks>
    Private Sub IWSIncompleteSamplesAuxScreen_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then bsExitButton.PerformClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IWSIncompleteSamplesAuxScreen_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IWSIncompleteSamplesAuxScreen_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen loading
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/08/2011
    ''' Modified by: SA 14/09/2011 - Added the screen centering
    ''' </remarks>
    Private Sub IWSIncompleteSamplesAuxScreen_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'The screen should appear always centered regarding the Main MDI
            Dim myLocation As Point = IAx00MainMDI.PointToScreen(Point.Empty)
            Dim mySize As Size = IAx00MainMDI.Size

            myNewLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 20) 'AG + RH 03/04/2012 - add - 20
            Me.Location = myNewLocation

            ScreenLoad()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IWSIncompleteSamplesAuxScreen_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IWSIncompleteSamplesAuxScreen_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Avoid moving the screen from its centered position
    ''' </summary>
    Private Sub IWSIncompleteSamplesAuxScreen_Move(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Move
        Me.Location = myNewLocation
    End Sub

    ''' <summary>
    ''' Close the screen. If the screen was opening from the START_BUTTON, the screen of Rotor Positioning is 
    ''' opening
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 
    ''' Modified by: SA 15/09/2011 - Verify if there are pending changes to show the correspondent warning message
    ''' </remarks>
    Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            continueClosing = True
            If (Not mySelectedTestsDS Is Nothing AndAlso mySelectedTestsDS.SelectedTestTable.Rows.Count > 0) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No) Then
                    continueClosing = False
                End If
            End If

            If (continueClosing) Then
                Me.Close()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CloseButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CloseButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

End Class
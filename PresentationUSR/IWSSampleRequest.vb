Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.PresentationCOM

Public Class IWSSampleRequest

#Region "Declarations"
    'Global variable for all Blank, Calibrator, Control and Patient Order Tests shown in the different grids
    Public myWorkSessionResultDS As New WorkSessionResultDS

    'Global variables to control if the SampleID corresponds to an existing PatientID
    Private sampleIDType As String = "AUTO"
    Private validatedSampleID As Boolean

    'Global variables to control checked/unchecked of all rows in Blanks and Calibrators grid
    Private isHeaderBlkCalCheckBoxClicked As Boolean
    Private isHeaderControlCheckBoxClicked As Boolean
    Private isHeaderPatientCheckBoxClicked As Boolean

    ' XB 26/08/2014 - BT #1868
    'Private totalBlkCalCheckBoxes As Integer
    Private totalBlankCheckBoxes As Integer
    Private totalCalibCheckBoxes As Integer

    Private totalControlCheckBoxes As Integer
    Private totalPatientCheckBoxes As Integer

    ' XB 26/08/2014 - BT #1868
    'Private totalBlkCalCheckedCheckBoxes As Integer
    Private totalBlankCheckedCheckBoxes As Integer
    Private totalCalibCheckedCheckBoxes As Integer

    Private totalControlCheckedCheckBoxes As Integer
    Private totalPatientCheckedCheckBoxes As Integer

    'Global variables to control the event SelectValueChange in ComboBox columns in DataGrids, and the SampleID edition in Patients grid
    Private textBoxOriginal As String = ""

    'Global variables to store the icon image byte arrays for Stat and Routine
    Private myIconStatFlag As Byte() = Nothing
    Private myIconNonStatFlag As Byte() = Nothing

    'Global variables to control selection/unselection of rows in grid of Patient Order Tests
    Private myPatientRow As Integer = -1
    Private myCountPatientClicks As Integer
    Private patientEventSource As String = ""

    'Global variables to control selection/unselection of rows in grid of Control Order Tests
    Private myControlRow As Integer = -1
    Private myCountControlClicks As Integer
    Private controlEventSource As String = ""

    'Global variables to control selection/unselection of rows in grid of Blank&Calibrator Order Tests
    Private myBlkCalibRow As Integer = -1
    Private myCountBlKCalibClicks As Integer
    Private blkCalibEventSource As String = ""

    'Global variable to store value of General Setting containing the maximum number of Patient Order Tests 
    'that can be created
    Private maxPatientOrderTests As Integer = -1

    'Global variable to control when the change of a PatientID/SampleID in a row in Patients' grid has been due
    'to a change in the case of one or more letters
    Private patientCaseChanged As Boolean

    'Global variable for the full path of the Import from LIMS file
    Private ReadOnly importFile As String = LIMSImportFilePath & LIMS_IMPORT_FILE_NAME

    'Global variable for the typed DataSet containing the Import Errors Log
    Private myImportErrorsLogDS As New ImportErrorsLogDS

    'Global variables needed to control edition of Calibration Factor in Blanks&Calibrators DataGridView
    Private myFactorTextBox As TextBox
    'Private OldFactorValue As Single = 0
    'Private Const NumFactorDecimals As Integer = 4
    Private Const CalibFactorFormat As String = "0.0000"

    'Global variables for Regular and Strike Fonts used for Calibrators
    Private ReadOnly StrikeFont As Font = New Font("Verdana", 8.25!, FontStyle.Strikeout, GraphicsUnit.Point, CType(0, Byte))
    Private ReadOnly RegularFont As Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))

    'Global variables to control the Progress Bar
    Private CreateOpenWS As Boolean = False
    Private SavingWS As Boolean = False
    Private ErrorOnSavingWS As String = String.Empty
    Private IsOpenRotor As Boolean = False
    'TR 15/09/2011
    'Dim isScanningProcess As Boolean = False

    Private mdiAnalyzerCopy As AnalyzerManager 'AG 23/09/2011

    Private RowPostPaintEnabled As Boolean = True 'RH 08/05/2012 Bugtracking 544

    'TR - Variable used to indicate if LIS is Online or works with Files
    Private LISWithFilesMode As Boolean = False

#End Region

#Region "Attributes"


    'Global variable to control if there have been changes in the WorkSession. There are changes when:
    ' ** New Blank, Calibrator, Control and/or Patient Order Tests have been requested
    ' ** Some Blank, Calibrator, Control and/or Patient Order Tests have been deleted
    ' ** Blank, Calibrator, Control and/or Patient Order Tests have been selected/unselected
    ' ** Number of Replicates and/or TubeType is changed for Blank, Calibrator, Control and/or Patient Order Tests  
    ' ** PatientID/SampleID is changed for Patient Order Tests
    ' ** StatFlag is changed for Patient Order Tests
    ' ** Results are informed for Off-System Tests requested in the WS
    ' ** A Blank or Calibrator Order Test having previous results is marked to be executed in the WS instead or reusing the previous value
    ' ** The value of the Calibration Factor for a Calibrator Order Test having a previous result to reused is changed (only for Single Point Calibrator)
    ' ** A saved WS is loaded
    ' ** A WS is imported from LIMS
    ' ** Order Tests (whatever SampleClass) are selected/unselected (due to the CreationOrder has to be changed)
    'Private ChangesMade As Boolean = False
    Private ChangesMadeAttribute As Boolean = False

    Private AnalyzerIDAttribute As String = ""
    Private WorkSessionIDAttribute As String = ""
    Private WSStatusAttribute As String = ""
    'Private PendingToSaveOTAttribute As Boolean
    'TR 03/04/2012 -Indicate if Worksession will be reset.
    Private isWSResetAttribute As Boolean = False

    'Attributes to manage Properties related with loading of a Saved WS from a Menu option in 
    'the MDIForm (case in which this screen is still not opened)
    Private WSLoadedIDAttribute As Integer = -1
    Private WSLoadedNameAttribute As String = ""

    'Attribute to manage Property related with executing the Import from LIMS process from a 
    'Menu option in the MDIForm (in the case this screen is still not opened)
    Private OpenForLIMSImportAttribute As Boolean
    Private OpenByAutomaticProcessAttribute As Boolean = False 'AG 11/07/2013

#End Region

#Region "Properties"
    Public WriteOnly Property ActiveAnalyzer() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    Public Property ActiveWorkSession() As String
        Get
            Return WorkSessionIDAttribute
        End Get
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

    Public Property ActiveWSStatus() As String
        Get
            Return WSStatusAttribute
        End Get
        Set(ByVal value As String)
            WSStatusAttribute = value
        End Set
    End Property

    Public Property WSLoadedID() As Integer
        Get
            Return WSLoadedIDAttribute
        End Get
        Set(ByVal value As Integer)
            WSLoadedIDAttribute = value
        End Set
    End Property

    Public Property WSLoadedName() As String
        Get
            Return WSLoadedNameAttribute
        End Get
        Set(ByVal value As String)
            WSLoadedNameAttribute = value
        End Set
    End Property

    Public WriteOnly Property OpenForLIMSImport() As Boolean
        Set(ByVal value As Boolean)
            OpenForLIMSImportAttribute = value
        End Set
    End Property

    Public WriteOnly Property ChangesMade() As Boolean
        Set(ByVal value As Boolean)
            ChangesMadeAttribute = value
        End Set
    End Property
    'TR 16/05/2012 -This property is not implemented any more. Tals to SA.
    'Public ReadOnly Property PendingToSaveOrderTests() As Boolean
    '    Get
    '        PendingToSaveOTAttribute = (bsPatientOrdersDataGridView.RowCount > 0 OrElse _
    '                                    bsBlkCalibDataGridView.RowCount > 0 OrElse _
    '                                    bsControlOrdersDataGridView.RowCount > 0)
    '        Return PendingToSaveOTAttribute
    '    End Get
    'End Property

    Public Property isWSReset() As Boolean
        Get
            Return isWSResetAttribute
        End Get
        Set(ByVal value As Boolean)
            isWSResetAttribute = value
            SavingWS = value 'AG 30/05/2013
        End Set
    End Property

    ''' <summary>
    ''' Indicates when the screen has been open by user (FALSE) or when by the automatic WS creation with LIS process (TRUE)
    ''' AG 11/07/2013
    ''' </summary>
    Public WriteOnly Property OpenByAutomaticProcess() As Boolean
        Set(ByVal value As Boolean)
            OpenByAutomaticProcessAttribute = value
        End Set
    End Property
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Execute the process of Import Samples from a LIMS file.Method is public to allow execute the LIMS process 
    ''' from the option in submenu Work Session in the Main MDI Form 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/09/2010
    ''' </remarks>
    Public Sub ExecuteImportFromLIMSProcess()
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Cursor = Cursors.WaitCursor

            Dim resultData As New GlobalDataTO
            Dim myOrdersDelegate As New OrdersDelegate

            resultData = myOrdersDelegate.ImportFromLIMS(Nothing, importFile, myWorkSessionResultDS, AnalyzerIDAttribute, _
                                                         LIMSImportMemoPath, WorkSessionIDAttribute, WSStatusAttribute, False)
            If (Not resultData.HasError) Then
                bsLIMSImportButton.Enabled = False
                LIMSErrorsButtonEnabled()

                ChangesMadeAttribute = True
            Else
                'Unexpected error in the import from LIMS process, show the message
                Cursor = Cursors.Default
                ShowMessage(Name & ".bsLIMSImportButton_Click", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

            Cursor = Cursors.Default

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("IWSampleRequest Execute Import (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IWSampleRequest.ExecuteImportFromLIMSProcess", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExecuteImportFromLIMSProcess", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExecuteImportFromLIMSProcess", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 'On-line' Refresh WS Samples Request screen when Barcode results are received
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG/TR 23/09/2011
    ''' Modified by: AG    15/03/2012 - When FREEZE appears while UI is disabled because screen is working Sw must reactivate UI
    '''              AG    28/03/2012 - When new alarms appears, check the covers to activate or not the Scan BarCode button
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed
            RefreshDoneField = False

            If (pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ)) Then
                ScreenWorkingProcess = False
                Me.Enabled = True
                ValidateBCWarningButtonEnabled()
                RefreshDoneField = True
            End If

            'When FREEZE appears while UI is disabled because screen is working Sw must reactivate UI
            If mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.FREEZE) = 1 Then
                ScreenWorkingProcess = False 'Process finished
                Me.Enabled = True
                IAx00MainMDI.EnableButtonAndMenus(True)
                IAx00MainMDI.SetActionButtonsEnableProperty(True)
                Cursor = Cursors.Default
                RefreshDoneField = True
            End If

            'Scan Barcode button must be disabled if cover open while the cover detection is enabled
            If pRefreshEventType.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then
                ValidateScanningButtonEnabled()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Create a new WorkSession or update the active one but sending the selected Order Tests to positioning in the Analyzer 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 02/11/2010 - Function changed to PUBLIC to allow calling it from the MDI
    '''              SA 21/02/2011 - Added code to show the ProgressBar while the WS is being saved
    '''              SA 28/09/2011 - When there are not Order Tests selected to be positioned but some Order Tests have been deleted,
    '''                              or added but not selected to be positioned, the Work Session has to be saved before opening the 
    '''                              screen of Rotor Positioning
    '''              SA 30/09/2011 - When User answer NO to the sending selected elements to positioning, the Rotor Positioning screen
    '''                              should not be opened
    '''              TR 11/04/2012 - Disable the form to avoid User's actions while the form is closing
    '''              XB 28/05/2013 - Threads are causing malfunctions on datagridview events so are deleted and related functionalities are synchronous called (bugstrankig #544 and #1102)
    ''' </remarks>
    Public Sub SaveWSWithPositioning()
        Try
            Dim showQuestion As Boolean = True

            Cursor = Cursors.WaitCursor

            'Verify if there are Open Order Tests (whatever SampleClass) selected to be sent to the Analyzer
            '*** Verify selected Patient Order Tests
            Dim lstWSPatients As List(Of WorkSessionResultDS.PatientsRow)
            lstWSPatients = (From a In myWorkSessionResultDS.Patients _
                            Where a.SampleClass = "PATIENT" _
                          AndAlso a.OTStatus = "OPEN" _
                          AndAlso a.Selected = True _
                           Select a).ToList()
            showQuestion = (lstWSPatients.Count > 0)
            lstWSPatients = Nothing

            If (Not showQuestion) Then
                '*** Verify selected Control Order Tests
                Dim lstWSControls As List(Of WorkSessionResultDS.ControlsRow)
                lstWSControls = (From a In myWorkSessionResultDS.Controls _
                                Where a.SampleClass = "CTRL" _
                              AndAlso a.OTStatus = "OPEN" _
                              AndAlso a.Selected = True _
                               Select a).ToList()
                showQuestion = (lstWSControls.Count > 0)
                lstWSControls = Nothing
            End If

            If (Not showQuestion) Then
                '*** Verify selected Blank and/or Calibrator Order Tests
                Dim lstWSBlkCalibs As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                lstWSBlkCalibs = (From a In myWorkSessionResultDS.BlankCalibrators _
                                 Where a.OTStatus = "OPEN" _
                               AndAlso a.Selected = True _
                                Select a).ToList()
                showQuestion = (lstWSBlkCalibs.Count > 0)
                lstWSBlkCalibs = Nothing
            End If

            Cursor = Cursors.Default
            IAx00MainMDI.EnableButtonAndMenus(False)

            Dim continueSaving As Boolean = False
            Dim openRotorScreen As Boolean = False

            If (showQuestion) Then
                ScreenWorkingProcess = True 'AG 08/11/2012 - inform this flag because the MDI requires it to avoid blinking in vertical bar

                'Show message to confirm that selected Order Tests will be sent to the Analyzer... continue only after confirmation
                If (ShowMessage(bsPrepareWSLabel.Text, GlobalEnumerates.Messages.POSITIONING_CONFIRMATION.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                    continueSaving = True
                End If
            Else
                'If some tests were deleted, or some tests were added but not selected to be positioned, it is needed to save the Work Session
                'before opening the Rotor Positioning screen
                If (ChangesMadeAttribute) Then
                    continueSaving = True
                Else
                    If (WorkSessionIDAttribute.Trim <> "") Then openRotorScreen = True
                End If
            End If

            If (continueSaving) Then
                RowPostPaintEnabled = False

                'Deactivate the Screen Controls
                'IAx00MainMDI.InitializeMarqueeProgreesBar()  ' XB 28/05/2013
                Application.DoEvents()

                ChangeControlsStatusByProgressBar(False)

                SavingWS = True
                ScreenWorkingProcess = True 'AG 08/11/2012 - inform this flag because the MDI requires it
                CreateOpenWS = False

                ' XB 28/05/2013
                'Dim workingThread As New Threading.Thread(AddressOf PrepareOrderTestsForWS)
                'workingThread.Start()

                'While SavingWS
                '    IAx00MainMDI.InitializeMarqueeProgreesBar()
                '    Application.DoEvents()
                'End While

                'workingThread = Nothing
                Cursor = Cursors.WaitCursor
                Application.DoEvents()
                PrepareOrderTestsForWS()
                ' XB 28/05/2013

                If (ErrorOnSavingWS = String.Empty) Then
                    openRotorScreen = True
                    IAx00MainMDI.SetWSActiveData(AnalyzerIDAttribute, WorkSessionIDAttribute, WSStatusAttribute)
                End If

            Else
                ScreenWorkingProcess = False 'AG 08/11/2012
            End If

            'IAx00MainMDI.StopMarqueeProgressBar()  ' XB 28/05/2013

            Cursor = Cursors.Default

            If (ErrorOnSavingWS <> String.Empty) Then
                'In case of Error, set Enabled=True for all controls that can be activated
                ChangeControlsStatusByProgressBar(True)

                Dim myErrorData As String() = ErrorOnSavingWS.Split(CChar("|"))
                ErrorOnSavingWS = String.Empty 'Reset the value after using it
                ShowMessage(Name & ".SaveWSWithPositioning", myErrorData(0), myErrorData(1), Me)

            ElseIf (openRotorScreen) Then
                'Disable the form while saving
                Me.Enabled = False

                'Update global variables in the main MDI Form
                IAx00MainMDI.SetWSActiveData(AnalyzerIDAttribute, WorkSessionIDAttribute, WSStatusAttribute)

                ' XB 27/11/2013 - Inform to MDI that this screen is closing aims to open next screen - Task #1303
                ExitingScreen()
                IAx00MainMDI.EnableButtonAndMenus(True)
                Application.DoEvents()

                'Open the RotorPositions form and close this one
                IAx00MainMDI.OpenRotorPositionsForm(Me)

            End If

            IAx00MainMDI.SetStatusRotorPosOptions(True)
        Catch ex As Exception
            IAx00MainMDI.StopMarqueeProgressBar()
            Application.DoEvents()

            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveWSWithPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveWSWithPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Cursor = Cursors.Default
            IAx00MainMDI.EnableButtonAndMenus(True)
        End Try
    End Sub
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' After loading the different grids in the screen, apply styles to special columns of each grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 
    ''' Modified by: DL 15/04/2011 - Removed column for field ControlNumber
    ''' </remarks>
    Private Sub ApplyStylesToSpecialGridColumns()
        Try
            'Apply styles to special columns
            bsPatientOrdersDataGridView.Columns("NumReplicates").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            bsControlOrdersDataGridView.Columns("NumReplicates").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsControlOrdersDataGridView.Columns("ExpirationDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            bsBlkCalibDataGridView.Columns("NumberOfCalibrators").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsBlkCalibDataGridView.Columns("NumReplicates").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsBlkCalibDataGridView.Columns("ABSValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsBlkCalibDataGridView.Columns("ABSDateTime").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsBlkCalibDataGridView.Columns("FactorValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ApplyStylesToSpecialGridColumns", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ApplyStylesToSpecialGridColumns", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Make the bind of the DS to the form Grids. Set the sort order for each grid
    ''' </summary>
    ''' <remarks>
    ''' Created by: GDS 12/04/2010
    ''' </remarks>
    Private Sub BindDSToGrids()
        Try
            'Sort the Blanks and Calibrators grid by SampleClass, CreationOrder and OTStatus (sent Order Tests are show first)
            Dim viewBlkCalib As DataView = myWorkSessionResultDS.BlankCalibrators.DefaultView
            viewBlkCalib.Sort = "SampleClass ASC, CreationOrder ASC, OTStatus ASC"

            Dim bsBlkCalibBindingSource As BindingSource = New BindingSource() With {.DataSource = viewBlkCalib}
            bsBlkCalibDataGridView.DataSource = bsBlkCalibBindingSource

            'Verify availability of controls related with grid of Blank and Calibrators
            CheckAddRemoveBlkCalRows()

            'Sort the Controls grid by CreationOrder and OTStatus (sent Order Tests are show first)
            Dim viewControl As DataView = myWorkSessionResultDS.Controls.DefaultView
            viewControl.Sort = "CreationOrder ASC, OTStatus ASC"

            Dim bsControlBindingSource As BindingSource = New BindingSource() With {.DataSource = viewControl}
            bsControlOrdersDataGridView.DataSource = bsControlBindingSource

            'Verify availability of controls related with grid of Controls
            CheckAddRemoveControlsRows()

            'Sort the Patients grid by CreationOrder and OTStatus (sent Order Tests are show first)
            Dim viewPatient As DataView = myWorkSessionResultDS.Patients.DefaultView
            viewPatient.Sort = "CreationOrder ASC, OTStatus ASC "

            Dim bsPatientBindingSource As BindingSource = New BindingSource() With {.DataSource = viewPatient}
            bsPatientOrdersDataGridView.DataSource = bsPatientBindingSource

            'Verify availability of controls related with grid of Patient Samples
            CheckAddRemovePatientsRows()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BindDSToGrids", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BindDSToGrids", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manage the selection/deselection of Blanks and/or Calibrators in the grid of Blank and Calibrator Order Tests
    ''' For event bsBlkCalibDataGridView_CellMouseUp
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 29/03/2010
    ''' Modified by: SA 07/04/2010 - Changes to avoid error when clicking in an empty grid
    '''              SA 12/04/2010 - When the click is done in an editable column the select/unselect of the full row does not apply 
    ''' </remarks>
    Private Sub BlkCalibOrderTestsCellMouseUp()
        Try
            If (Not bsBlkCalibDataGridView.CurrentRow Is Nothing) Then
                If (bsBlkCalibDataGridView.CurrentCell.ReadOnly) Then
                    blkCalibEventSource = "Left_Click"
                    If (bsBlkCalibDataGridView.SelectedRows.Count <= 1) Then
                        If (bsBlkCalibDataGridView.CurrentRow.Index = myBlkCalibRow) Then
                            If (myCountBlKCalibClicks = 1) Then
                                'Row is unmarked; controls in header area remains without changes 
                                blkCalibEventSource = "Left_Click_Disabled"
                            Else
                                'Row is marked as selected; 
                                blkCalibEventSource = "Left_Click_Enabled"

                                'SampleClas of the selected row is selected in ComboBox of Sample Classes, and the selected SampleType in the ComboBox of SampleTypes
                                bsSampleClassComboBox.SelectedValue = bsBlkCalibDataGridView("SampleClass", bsBlkCalibDataGridView.CurrentCell.RowIndex).Value
                                bsSampleTypeComboBox.SelectedValue = bsBlkCalibDataGridView("SampleType", bsBlkCalibDataGridView.CurrentCell.RowIndex).Value

                                HeaderStatusForBlankCalibrators(bsBlkCalibDataGridView("SampleClass", bsBlkCalibDataGridView.CurrentCell.RowIndex).Value.ToString)
                            End If

                            myBlkCalibRow = -1
                        Else
                            'A different row has been selected... 
                            myBlkCalibRow = bsBlkCalibDataGridView.CurrentRow.Index
                            myCountBlKCalibClicks = 1

                            'SampleClas of the selected row is selected in ComboBox of Sample Classes, and the selected SampleType in the ComboBox of SampleTypes
                            bsSampleClassComboBox.SelectedValue = bsBlkCalibDataGridView("SampleClass", bsBlkCalibDataGridView.CurrentCell.RowIndex).Value
                            bsSampleTypeComboBox.SelectedValue = bsBlkCalibDataGridView("SampleType", bsBlkCalibDataGridView.CurrentCell.RowIndex).Value

                            HeaderStatusForBlankCalibrators(bsBlkCalibDataGridView("SampleClass", bsBlkCalibDataGridView.CurrentCell.RowIndex).Value.ToString)
                        End If
                    Else
                        'When more than one row has been selected, verify if all of them have the same SampleClass
                        If (IsTheSameSampleClassSelection()) Then
                            'The common SampleClas of the selected rows is selected in ComboBox of Sample Classes
                            bsSampleClassComboBox.SelectedValue = bsBlkCalibDataGridView("SampleClass", bsBlkCalibDataGridView.CurrentCell.RowIndex).Value
                            'Else
                            'Controls in the header area remains without changes
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BlkCalibOrderTestsClickRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BlkCalibOrderTestsClickRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Re-sort all SampleIDs automatically generated that have not been sent to an Analyzer.  These SampleIDs have
    ''' to be resequenced after adding or deleting Patient Order Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 19/04/2010
    ''' Modified by: SA 30/09/2011 - Changed the function logic due to bad calculation when adding more tests to auto SampleIDs with 
    '''                              Order Tests already positioned and adding new Order Tests for one or more auto SampleIDs 
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode
    ''' </remarks>
    Private Sub CalculateAllAutonumeric()
        Try
            'Get all Patient Order Tests that correspond to an automatically generated SampleID and already sent to the Analyzer
            Dim sampleIDsSent As List(Of String)
            sampleIDsSent = (From a In myWorkSessionResultDS.Patients _
                            Where a.SampleClass = "PATIENT" _
                          AndAlso a.SampleIDType = "AUTO" _
                          AndAlso a.OTStatus <> "OPEN" _
                         Order By a.StatFlag Descending, a.SampleID _
                           Select a.SampleID).Distinct.ToList()

            Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
            If (sampleIDsSent.Count > 0) Then
                'Get all Open Patient Order Tests that correspond to a automatically generated SampleID
                Dim sampleIDsList As List(Of String)
                sampleIDsList = (From a In myWorkSessionResultDS.Patients _
                                Where a.SampleClass = "PATIENT" _
                              AndAlso a.SampleIDType = "AUTO" _
                              AndAlso a.OTStatus = "OPEN" _
                             Order By a.StatFlag Descending, a.SampleID _
                               Select a.SampleID).Distinct.ToList()

                If (sampleIDsList.Count > 0) Then
                    Dim nextSeq As Integer = 1

                    For Each sampleID As String In sampleIDsList
                        If (Not sampleIDsSent.Contains(sampleID)) Then
                            Dim currentSeq As Integer = Convert.ToInt32(sampleID.Substring(9))
                            If (currentSeq = nextSeq) Then
                                nextSeq += 1

                            Else
                                Dim notFound As Boolean = True
                                Do While (notFound)
                                    Dim searchedSampleID As String = sampleID.Substring(0, 9) & Format(nextSeq, "0000")

                                    'Search if value for nextSeq is assigned to a Patient Order Test already sent to the Analyzer
                                    lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                                      Where a.SampleClass = "PATIENT" _
                                                    AndAlso a.SampleIDType = "AUTO" _
                                                    AndAlso a.OTStatus <> "OPEN" _
                                                    AndAlso a.SampleID.ToUpperInvariant = searchedSampleID.ToUpperInvariant _
                                                     Select a).ToList()
                                    If (lstWSPatientsDS.Count > 0) Then
                                        nextSeq += 1
                                    Else
                                        notFound = False

                                        'Search all Order Tests with the current SampleID and assign to them the Searched SampleID
                                        lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                                          Where a.SampleClass = "PATIENT" _
                                                        AndAlso a.SampleIDType = "AUTO" _
                                                        AndAlso a.OTStatus = "OPEN" _
                                                        AndAlso a.SampleID.ToUpperInvariant = sampleID.ToUpperInvariant _
                                                         Select a).ToList()


                                        For Each patientRow As WorkSessionResultDS.PatientsRow In lstWSPatientsDS
                                            patientRow.SampleID = searchedSampleID.Substring(1, 12)
                                        Next

                                        currentSeq = nextSeq
                                        nextSeq += 1
                                    End If
                                Loop
                            End If
                        End If
                    Next sampleID
                End If
                sampleIDsList = Nothing

                'Add the # character to all the re-sequenced automatic SampleIDs
                lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                  Where a.SampleClass = "PATIENT" _
                                AndAlso a.SampleIDType = "AUTO" _
                                AndAlso a.OTStatus = "OPEN" _
                                AndAlso a.SampleID.Substring(0, 1) <> "#" _
                                 Select a).ToList()

                For Each patientRow As WorkSessionResultDS.PatientsRow In lstWSPatientsDS
                    patientRow.SampleID = "#" & patientRow.SampleID
                Next
                myWorkSessionResultDS.Patients.AcceptChanges()
            Else
                'There are not Patient Order Tests with SampleID automatically generated already sent to the Analyzer
                'Automatic SampleID of Open Patient Order Tests are re-sorted...
                lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                  Where a.SampleClass = "PATIENT" _
                                AndAlso a.SampleIDType = "AUTO" _
                                AndAlso a.OTStatus = "OPEN" _
                               Order By a.StatFlag Descending, a.SampleID _
                                 Select a).ToList()

                Dim nextSeq As Integer = 1
                Dim newSampleID As String = ""
                Dim currentSampleID As String = ""
                For Each patientRow As WorkSessionResultDS.PatientsRow In lstWSPatientsDS
                    If (patientRow.SampleID.ToUpperInvariant <> currentSampleID.ToUpperInvariant) Then
                        currentSampleID = patientRow.SampleID
                        newSampleID = patientRow.SampleID.Substring(0, 9) & Format(nextSeq, "0000")

                        nextSeq += 1
                    End If

                    patientRow.SampleID = newSampleID
                Next
                myWorkSessionResultDS.Patients.AcceptChanges()
            End If

            sampleIDsSent = Nothing
            lstWSPatientsDS = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CalculateAllAutonumeric", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CalculateAllAutonumeric", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Calculate the last generated autonumeric Patient Identifier
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 22/03/2010
    ''' Modified by: SA 31/03/2010 - Implement LINQ to filter Patient Order Tests with PatientID automatically generated 
    ''' </remarks>
    Private Function CalculateAutonumeric() As String
        Dim maxValue As String = ""
        Try
            'Verify if there are Patient Order Tests for autonumeric SampleIDs for the current date 
            Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                              Where a.SampleClass = "PATIENT" _
                            AndAlso a.SampleIDType = "AUTO" _
                           Order By a.SampleID Descending _
                             Select a).ToList()

            If (lstWSPatientsDS.Count > 0) Then maxValue = lstWSPatientsDS.First.SampleID
            lstWSPatientsDS = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CalculateAutonumeric", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CalculateAutonumeric", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return maxValue
    End Function

    ''' <summary>
    ''' If the NewCheck column for a Blank or Calibrator is selected/unselected, the correspondent Selected Check is selected/unselected
    ''' For event bsBlkCalibDataGridView_CellValueChanged
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/04/2010
    ''' Modified by: SA 25/11/2010 - For Calibrators, enable/disable FactorValue cell when New CheckBox is unchecked/checked
    ''' </remarks>
    Private Sub ChangeBlankCalibratorNewCheckColumn()
        Try
            If (Not bsBlkCalibDataGridView.CurrentRow.Cells("NewCheck").ReadOnly) Then
                bsBlkCalibDataGridView.CurrentRow.Cells("Selected").Value = bsBlkCalibDataGridView.CurrentRow.Cells("NewCheck").Value

                If (bsBlkCalibDataGridView.CurrentRow.Cells("SampleClass").Value.ToString = "CALIB") Then
                    'If the current row corresponds to a Calibrator, cell for FactorValue can be changed in following cases:
                    ' ** The Calibrator Order Test does not have a status different of OPEN
                    ' ** The Order Test is for a single point Calibrator
                    ' ** There is a previous result for the Calibrator Order Test and New check remains unselected
                    ' ** Value of Calibrator Factor is informed
                    bsBlkCalibDataGridView.CurrentRow.Cells("FactorValue").ReadOnly = (bsBlkCalibDataGridView.CurrentRow.Cells("OTStatus").Value.ToString <> "OPEN") OrElse _
                                                                                      (Convert.ToBoolean(bsBlkCalibDataGridView.CurrentRow.Cells("NewCheck").Value)) OrElse _
                                                                                      (Convert.ToInt32(bsBlkCalibDataGridView.CurrentRow.Cells("NumberOfCalibrators").Value) > 1) OrElse _
                                                                                      String.IsNullOrEmpty(bsBlkCalibDataGridView.CurrentRow.Cells("PreviousOrderTestID").Value.ToString) OrElse _
                                                                                      String.IsNullOrEmpty(bsBlkCalibDataGridView.CurrentRow.Cells("OriginalFactorValue").Value.ToString)

                End If
                ChangesMadeAttribute = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeBlankCalibratorNewCheckColumn", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeBlankCalibratorNewCheckColumn", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' If a Calibrator is selected, select also the required Blank
    ''' If a Blank or Calibrator is unselected, verify if it is needed for another Order Test to allow the unselection
    ''' For event bsBlkCalibDataGridView_CellValueChanged
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 09/04/2010 - Code move from the event; fixed error when calling function VerifyUnselectedOrderTest: parameters
    '''                              where got from grid of Controls instead of from grid of Blanks and Calibrators
    '''              SA 15/04/2010 - When a Calibrator used as alternative for different SampleTypes is unselected, verify that there are
    '''                              not Patient or Control Order Tests requested for any of the requested Sample Types
    '''              SA 09/11/2012 - When function VerifyUnselectedOrderTest is called to validate if there are Patient or Control Order Tests 
    '''                              requested for any of the requested Sample Types, parameter with the "real" SampleType (the one for which
    '''                              the Calibrator was defined) is informed (due to when validation of existing previous results for the 
    '''                              Calibrator is done, the informed SampleType has to be always the one for which the Calibrator was defined)
    '''              XB 26/08/2014 - remove parameter on the call RowBlkCalCheckBoxClick function - BT #1868
    ''' </remarks>
    Private Sub ChangeBlankCalibratorSelectedColumn()
        Try
            If (Not isHeaderBlkCalCheckBoxClicked) Then
                If (Not bsBlkCalibDataGridView.CurrentRow.Cells("Selected").ReadOnly) Then
                    Dim myOrderTestsDelegate As New OrderTestsDelegate
                    If (CBool(bsBlkCalibDataGridView.CurrentRow.Cells("Selected").Value)) Then
                        If (bsBlkCalibDataGridView.CurrentRow.Cells("SampleClass").Value.ToString = "CALIB") Then
                            'Select also the required Blank in case it is unselected, excepting when it has a previous result; in 
                            'that case the Blank remains in the same state
                            myOrderTestsDelegate.SelectAllNeededOrderTests(bsBlkCalibDataGridView.CurrentRow.Cells("SampleClass").Value.ToString, _
                                                                           bsBlkCalibDataGridView.CurrentRow.Cells("TestType").Value.ToString, _
                                                                           CInt(bsBlkCalibDataGridView.CurrentRow.Cells("TestID").Value), _
                                                                           bsBlkCalibDataGridView.CurrentRow.Cells("SampleType").Value.ToString, _
                                                                           myWorkSessionResultDS)

                        End If

                        'Verify if the CheckBox for select/unselect all Blanks and Calibrators have to be checked
                        RowBlkCalCheckBoxClick()
                    Else
                        Dim myVerification As Boolean = False
                        If (bsBlkCalibDataGridView.CurrentRow.Cells("PreviousOrderTestID").Value.ToString = "") Then
                            myVerification = myOrderTestsDelegate.VerifyUnselectedOrderTest(bsBlkCalibDataGridView.CurrentRow.Cells("SampleClass").Value.ToString, _
                                                                                            bsBlkCalibDataGridView.CurrentRow.Cells("TestType").Value.ToString, _
                                                                                            CInt(bsBlkCalibDataGridView.CurrentRow.Cells("TestID").Value), _
                                                                                            bsBlkCalibDataGridView.CurrentRow.Cells("SampleType").Value.ToString, _
                                                                                            myWorkSessionResultDS)

                            If (bsBlkCalibDataGridView.CurrentRow.Cells("SampleClass").Value.ToString = "CALIB") Then
                                'If the unselected Order Test is a Calibrator used as alternative for other Sample Types, 
                                'verify also that there are not requested Control and/or Patient Order Test for the Test 
                                'and each requested Sample Type
                                If (bsBlkCalibDataGridView.CurrentRow.Cells("RequestedSampleTypes").Value.ToString <> _
                                    bsBlkCalibDataGridView.CurrentRow.Cells("SampleType").Value.ToString) Then

                                    Dim additionalSampleTypes() As String = Split(bsBlkCalibDataGridView.CurrentRow.Cells("RequestedSampleTypes").Value.ToString.Trim)
                                    For Each reqSampleType As String In additionalSampleTypes
                                        myVerification = myOrderTestsDelegate.VerifyUnselectedOrderTest(bsBlkCalibDataGridView.CurrentRow.Cells("SampleClass").Value.ToString, _
                                                                                                       bsBlkCalibDataGridView.CurrentRow.Cells("TestType").Value.ToString, _
                                                                                                       Convert.ToInt32(bsBlkCalibDataGridView.CurrentRow.Cells("TestID").Value), _
                                                                                                       reqSampleType.ToString, myWorkSessionResultDS, _
                                                                                                       bsBlkCalibDataGridView.CurrentRow.Cells("SampleType").Value.ToString)
                                        If (Not myVerification) Then Exit For
                                    Next
                                End If
                            End If
                        Else
                            'If there is a previous result for the current Blank or Calibrator it can be always unselected
                            myVerification = True
                        End If

                        If (Not myVerification) Then
                            'The Blank or Calibrator cannot be unselected due to is required for another OrderTest
                            bsBlkCalibDataGridView.CurrentRow.Cells("Selected").Value = True
                            myWorkSessionResultDS.BlankCalibrators.AcceptChanges()
                        Else
                            'If a Calibrator was unselected, the related Blank is also unselected, excepting when it has a previous result;
                            'in that case the Blank remains in the same state
                            If (bsBlkCalibDataGridView.CurrentRow.Cells("SampleClass").Value.ToString = "CALIB") Then
                                Dim lstWSBlankDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                                lstWSBlankDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                                               Where a.SampleClass = "BLANK" _
                                             AndAlso a.TestType = bsBlkCalibDataGridView.CurrentRow.Cells("TestType").Value.ToString _
                                             AndAlso a.TestID = CInt(bsBlkCalibDataGridView.CurrentRow.Cells("TestID").Value) _
                                             AndAlso a.SampleType = bsBlkCalibDataGridView.CurrentRow.Cells("SampleType").Value.ToString _
                                             AndAlso (a.IsPreviousOrderTestIDNull OrElse a.PreviousOrderTestID.ToString = "") _
                                              Select a).ToList()

                                If (lstWSBlankDS.Count = 1) Then
                                    lstWSBlankDS(0).Selected = False
                                    myWorkSessionResultDS.BlankCalibrators.AcceptChanges()
                                End If
                            End If
                        End If
                    End If

                    'Control check/uncheck of New CheckBox when there is a previous result for the Blank or Calibrator in the current row
                    If (bsBlkCalibDataGridView.CurrentRow.Cells("PreviousOrderTestID").Value.ToString <> "") Then
                        bsBlkCalibDataGridView.CurrentRow.Cells("NewCheck").Value = bsBlkCalibDataGridView.CurrentRow.Cells("Selected").Value
                    End If

                    RefreshGrids()
                    ChangesMadeAttribute = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeBlankCalibratorSelectedColumn", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeBlankCalibratorSelectedColumn", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When a TubeType is changed, it has to be changed for all requested Calibrator Order Tests using the same Calibrator
    ''' For event bsBlkCalibDataGridView_CellValueChanged
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 09/04/2010 - Code moved from the event; changes to improve the code
    ''' Modified by: RH 07/06/2011 - Add logic for BLANK rows.
    ''' </remarks>
    Private Sub ChangeBlankCalibratorTubeTypeColumn()
        Try
            If (Not bsBlkCalibDataGridView.CurrentRow.Cells("TubeType").ReadOnly) Then
                If (bsBlkCalibDataGridView.CurrentRow.Cells("SampleClass").Value.ToString = "CALIB") Then
                    'The TubeType can be changed, find all rows with the same Calibrator
                    Dim lstWSCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                    lstWSCalibratorsDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                                         Where a.SampleClass = "CALIB" _
                                       AndAlso Not a.IsCalibratorIDNull _
                                       AndAlso a.CalibratorID = Convert.ToInt32(bsBlkCalibDataGridView.CurrentRow.Cells("CalibratorID").Value) _
                                        Select a).ToList()

                    For Each calibratorRow As WorkSessionResultDS.BlankCalibratorsRow In lstWSCalibratorsDS
                        calibratorRow.TubeType = bsBlkCalibDataGridView.CurrentRow.Cells("TubeType").Value.ToString
                    Next
                    lstWSCalibratorsDS = Nothing
                Else 'CurrentRow.SampleClass it is "BLANK"
                    'The TubeType can be changed, find all rows with the same BlankMode
                    Dim lstWSCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                    lstWSCalibratorsDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                                         Where a.SampleClass = "BLANK" _
                                       AndAlso Not a.IsBlankModeNull _
                                       AndAlso a.BlankMode = bsBlkCalibDataGridView.CurrentRow.Cells("BlankMode").Value.ToString _
                                        Select a).ToList()

                    For Each blankRow As WorkSessionResultDS.BlankCalibratorsRow In lstWSCalibratorsDS
                        blankRow.TubeType = bsBlkCalibDataGridView.CurrentRow.Cells("TubeType").Value.ToString
                    Next
                    lstWSCalibratorsDS = Nothing
                End If
                myWorkSessionResultDS.BlankCalibrators.AcceptChanges()

                ChangesMadeAttribute = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeCalibratorTubeTypeColumn", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeCalibratorTubeTypeColumn", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' After changing value of Selected column for a Blank or Calibrator, verify the state of the CheckBox that
    ''' allows select/unselect all the selectable elements in the grid
    ''' For event bsBlkCalibDataGridView_CurrentCellDirtyStateChanged
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/04/2010 - Code moved from the event
    ''' Modified by: XB 26/08/2014 - remove parameter on the call RowBlkCalCheckBoxClick function - BT #1868
    ''' </remarks>
    Private Sub ChangeBlkCalibSelectedState()
        Try
            If (TypeOf bsBlkCalibDataGridView.CurrentCell Is DataGridViewCheckBoxCell) OrElse _
               (TypeOf bsBlkCalibDataGridView.CurrentCell Is DataGridViewComboBoxCell) Then
                bsBlkCalibDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)

                If (bsBlkCalibDataGridView.CurrentCell.OwningColumn.Name = "Selected") Then
                    RowBlkCalCheckBoxClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeBlkCalibSelectedState", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeBlkCalibSelectedState", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the Number of Replicates is changed, it has to be changed for all the Controls needed for the same TestType, TestID and SampleType
    ''' For event bsControlOrdersDataGridView_CellValueChanged
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/04/2010 - Code move from the event; changes to improve the code
    ''' Modified by: SA 19/06/2012 - Get also the TestType and filter the Linq also by this field
    ''' </remarks>
    Private Sub ChangeControlNumReplicatesColumn()
        Try
            If (Not bsControlOrdersDataGridView.CurrentRow.Cells("NumReplicates").ReadOnly) Then
                Dim myTestType As String = bsControlOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString
                Dim myTestID As Integer = Convert.ToInt32(bsControlOrdersDataGridView.CurrentRow.Cells("TestID").Value)
                Dim mySampleType As String = bsControlOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString
                Dim myNumReplicates As Integer = Convert.ToInt32(bsControlOrdersDataGridView.CurrentRow.Cells("NumReplicates").Value)

                'The Num of Replicates can be changed, find all rows having the same TestType, Test and SampleType
                Dim lstWSControlsDS As List(Of WorkSessionResultDS.ControlsRow)
                lstWSControlsDS = (From a In myWorkSessionResultDS.Controls _
                                  Where a.SampleClass = "CTRL" _
                                AndAlso a.TestType = myTestType _
                                AndAlso a.TestID = myTestID _
                                AndAlso a.SampleType = mySampleType _
                                 Select a).ToList()

                For Each controlRow As WorkSessionResultDS.ControlsRow In lstWSControlsDS
                    controlRow.NumReplicates = myNumReplicates
                Next
                myWorkSessionResultDS.Controls.AcceptChanges()
                lstWSControlsDS = Nothing

                ChangesMadeAttribute = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeControlNumReplicatesColumn", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeControlNumReplicatesColumn", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' If a Control is selected, select also the required Calibrator and Blank
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/04/2010 - Code move from the event
    ''' Modified by: SA 12/07/2010 - Added code to manage the unselection of a Control
    '''              SA 19/06/2012 - Select/Unselect of related Blanks and Calibrators has to be executed only for Standard Tests
    ''' </remarks>
    Private Sub ChangeControlSelectedColumn()
        Try
            If (Not isHeaderControlCheckBoxClicked) Then
                If (bsControlOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString = "STD") Then
                    Dim myOrderTestsDelegate As New OrderTestsDelegate
                    If CBool(bsControlOrdersDataGridView.CurrentRow.Cells("Selected").Value) Then
                        myOrderTestsDelegate.SelectAllNeededOrderTests(bsControlOrdersDataGridView.CurrentRow.Cells("SampleClass").Value.ToString, _
                                                                       bsControlOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString, _
                                                                       CInt(bsControlOrdersDataGridView.CurrentRow.Cells("TestID").Value), _
                                                                       bsControlOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString, _
                                                                       myWorkSessionResultDS)
                    Else
                        'Unselect the Calibrator and Blank if it is possible
                        UnselectBlkCalibrator("STD", bsControlOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString, _
                                              CInt(bsControlOrdersDataGridView.CurrentRow.Cells("TestID").Value))
                    End If
                    RefreshGrids()
                    ChangesMadeAttribute = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeControlSelectedColumn", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeControlSelectedColumn", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' After changing value of Selected column for a Control, verify the state of the CheckBox that
    ''' allows select/unselect all the selectable elements in the grid
    ''' For event bsControlOrdersDataGridView_CurrentCellDirtyStateChanged
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/04/2010 - Code moved from the event; changes to improve the code
    ''' </remarks>
    Private Sub ChangeControlSelectedState()
        Try
            If (TypeOf bsControlOrdersDataGridView.CurrentCell Is DataGridViewCheckBoxCell) OrElse _
               (TypeOf bsControlOrdersDataGridView.CurrentCell Is DataGridViewComboBoxCell) Then
                bsControlOrdersDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)

                If (bsControlOrdersDataGridView.CurrentCell.OwningColumn.Name = "Selected") Then
                    RowControlCheckBoxClick(Nothing)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeControlSelectedState", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeControlSelectedState", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Activate/desactivate screen controls when ProgressBar is hidden/shown
    ''' </summary>
    ''' <param name="pStatus">Flag indicating if screen controls have to be enabled or disabled</param>
    ''' <remarks>
    ''' Created by:  SA 23/02/2011
    ''' </remarks>
    Private Sub ChangeControlsStatusByProgressBar(ByVal pStatus As Boolean)
        Try
            bsOrderDetailsGroupBox.Enabled = pStatus
            bsSampleClassesTabControl.Enabled = pStatus

            If (Not pStatus) Then
                bsLoadWSButton.Enabled = False
                bsSaveWSButton.Enabled = False
                bsOffSystemResultsButton.Enabled = False
                bsScanningButton.Enabled = False 'TR 29/09/2011 Disable scanning button.
                bsLIMSImportButton.Enabled = False
                bsLIMSErrorsButton.Enabled = False
                bsOpenRotorButton.Enabled = False
                bsAcceptButton.Enabled = False
            Else
                SaveLoadWSButtonsEnabled()
                OffSystemResultsButtonEnabled()

                'SA 16/09/2011 
                'LIMSImportButtonEnabled()
                'LIMSErrorsButtonEnabled()

                OpenRotorButtonEnabled()
                bsAcceptButton.Enabled = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeControlsStatusByProgressBar", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeControlsStatusByProgressBar", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When a TubeType is changed, it has to be changed for all requested Control Order Tests using the same Control
    ''' For event bsControlOrdersDataGridView_CellValueChanged
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/04/2010 - Code move from the event; changes to improve the code
    ''' </remarks>
    Private Sub ChangeControlTubeTypeColumn()
        Try
            If (Not bsControlOrdersDataGridView.CurrentRow.Cells("TubeType").ReadOnly) Then
                Dim myControlID As Integer = Convert.ToInt32(bsControlOrdersDataGridView.CurrentRow.Cells("ControlID").Value)
                Dim myTubeType As String = bsControlOrdersDataGridView.CurrentRow.Cells("TubeType").Value.ToString

                'The TubeType can be changed, find all rows with the same Control
                Dim lstWSControlsDS As List(Of WorkSessionResultDS.ControlsRow)
                lstWSControlsDS = (From a In myWorkSessionResultDS.Controls _
                                  Where a.SampleClass = "CTRL" _
                                AndAlso a.ControlID = myControlID _
                                 Select a).ToList()

                For Each controlRow As WorkSessionResultDS.ControlsRow In lstWSControlsDS
                    controlRow.TubeType = myTubeType
                Next
                myWorkSessionResultDS.Controls.AcceptChanges()
                lstWSControlsDS = Nothing

                ChangesMadeAttribute = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeControlTubeTypeColumn", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeControlTubeTypeColumn", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the Number of Replicates is changed, it is changed for all rows having the same SampleID and TestID but
    ''' different Sample Type, due to the number of replicates is defined by Test, not by Test/SampleType
    ''' For event bsPatientOrdersDataGridView_CellValueChanged
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 27/04/2010 
    ''' Modified by: SA 06/10/2011 - Get the TestType for the selected grid row and filter the linq to get the rest 
    '''                              of affected rows by TestType
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode
    ''' </remarks> 
    Private Sub ChangePatientNumReplicatesColumn()
        Try
            If (Not bsPatientOrdersDataGridView.CurrentRow.Cells("NumReplicates").ReadOnly) Then
                Dim mySampleID As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value.ToString
                Dim myTestType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString
                Dim myTestID As Integer = Convert.ToInt32(bsPatientOrdersDataGridView.CurrentRow.Cells("TestID").Value)
                Dim mySampleType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString
                Dim myNumReplicates As Integer = Convert.ToInt32(bsPatientOrdersDataGridView.CurrentRow.Cells("NumReplicates").Value)

                'The Num of Replicates can be changed, find all rows having the same SampleID and Test but with a different Sample Type
                '(the Number of Replicates is defined by Test, not by Test/SampleType)
                Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
                lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                  Where a.SampleClass = "PATIENT" _
                                AndAlso a.SampleID.ToUpperInvariant = mySampleID.ToUpperInvariant _
                                AndAlso a.TestType = myTestType _
                                AndAlso a.TestID = myTestID _
                                AndAlso a.SampleType <> mySampleType _
                                 Select a).ToList()

                If (lstWSPatientsDS.Count > 0) Then
                    For Each patientRow As WorkSessionResultDS.PatientsRow In lstWSPatientsDS
                        patientRow.NumReplicates = myNumReplicates
                    Next
                    myWorkSessionResultDS.Patients.AcceptChanges()
                End If
                lstWSPatientsDS = Nothing

                ChangesMadeAttribute = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangePatientNumReplicatesColumn", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangePatientNumReplicatesColumn", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When user DoubleClicking in the cell showing the Stat/Routine Icon, the process to change priority from Routine to Stat (or the opposite) is 
    ''' executed.  For event bsPatientOrdersDataGridView_CellMouseDoubleClick
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 25/03/2010
    ''' Modified by: SA 06/04/2010 - Changes: confirmation message was missing; priority change has to be replicated to all Order Tests with the same
    '''                              PatientID/SampleID and StatFlag, and finally verify if a merge of duplicated Order Tests is needed
    '''              SA 23/04/2010 - Priority change is possible only when there are not sent Order Tests for the selected PatientID/SampleID 
    '''                              and StatFlag
    '''              SA 11/05/2010 - When verify duplicates, sort records by Status to delete the opened ones
    '''              SA 13/05/2010 - Test merge is not needed any more due to now it is not possible for Patient Samples select a Test that is 
    '''                              requested for the same Patient and different Priority
    '''              SA 09/07/2010 - If the Priority was changed for the selected Patient, change value of Stat CheckBox in the 
    '''                              header area
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode.
    '''              TR 11/03/2013 - Execute function only when OTStatus = OPEN AndAlso LISRequest = FALSE
    '''                              In the first LINQ, change filter OTStatus <> OPEN by the following one
    '''                              (OTStatus <> OPEN OrElse LISRequest = TRUE)   
    ''' </remarks>
    Private Sub ChangePatientOrderPriority()
        Try
            If (bsPatientOrdersDataGridView.Columns(bsPatientOrdersDataGridView.CurrentCell.ColumnIndex).Name = "SampleClassIcon") Then
                If (bsPatientOrdersDataGridView.CurrentRow.Cells("OTStatus").Value.ToString = "OPEN" AndAlso _
                    bsPatientOrdersDataGridView.CurrentRow.Cells("LISRequest").Value.ToString = "False") Then

                    'Get values of selected PatientID/SampleID and StatFlag
                    Dim selectedSampleID As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value.ToString
                    Dim selectedStatFlag As Boolean = CBool(bsPatientOrdersDataGridView.CurrentRow.Cells("StatFlag").Value)

                    'Verify if there are sent Patient Samples for the same PatientID/SampleID and StatFlag
                    Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
                    lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                      Where a.SampleID = selectedSampleID _
                                        AndAlso a.StatFlag = selectedStatFlag _
                                        AndAlso (a.OTStatus <> "OPEN" OrElse a.LISRequest = True) _
                                     Select a).ToList()

                    'Only if there are not sent Order Tests for the selected PatientID/SampleID and StatFlag the process is executed
                    If (lstWSPatientsDS.Count = 0) Then
                        'Show the proper confirmation message
                        Dim iconToShow As Byte() = Nothing
                        Dim executePriorityChange As Boolean = False

                        If (selectedStatFlag) Then
                            iconToShow = myIconNonStatFlag
                            executePriorityChange = (ShowMessage(bsPrepareWSLabel.Text, GlobalEnumerates.Messages.CHANGE_STAT_TO_ROUTINE.ToString, , Me) = Windows.Forms.DialogResult.Yes)
                        Else
                            iconToShow = myIconStatFlag
                            executePriorityChange = (ShowMessage(bsPrepareWSLabel.Text, GlobalEnumerates.Messages.CHANGE_ROUTINE_TO_STAT.ToString, , Me) = Windows.Forms.DialogResult.Yes)
                        End If

                        'Process continues only after confirmation
                        If (executePriorityChange) Then
                            Dim mergeTests As Boolean = False
                            Dim myFinalID As String = ""

                            'Verify if there are Order Tests requested for the same PatientID/SampleID and the opposite StatFlag in order to
                            'execute the merge of Tests after change the priority
                            lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                              Where a.SampleID.ToUpperInvariant = selectedSampleID.ToUpperInvariant _
                                            AndAlso a.StatFlag = (Not selectedStatFlag) _
                                             Select a).ToList()

                            If (lstWSPatientsDS.Count > 0) Then
                                mergeTests = True
                                myFinalID = lstWSPatientsDS.First.SampleID
                            Else
                                myFinalID = selectedSampleID
                            End If

                            'Get all Order Tests belonging to the selected PatientID/SampleID and StatFlag
                            lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                              Where a.SampleID.ToUpperInvariant = selectedSampleID.ToUpperInvariant _
                                            AndAlso a.StatFlag = selectedStatFlag _
                                             Select a).ToList()

                            'Change priority of all OrderTests of the selected Patient/StatFlag and accept changes
                            For Each patientRow As WorkSessionResultDS.PatientsRow In lstWSPatientsDS
                                patientRow.SampleID = myFinalID
                                patientRow.SampleClassIcon = iconToShow
                                patientRow.StatFlag = (Not selectedStatFlag)

                                myWorkSessionResultDS.Patients.AcceptChanges()
                            Next

                            'If it is possible that some records can be duplicated, execute the merge
                            If (mergeTests) Then
                                MergePatientOrderTests(selectedSampleID, Not selectedStatFlag)
                                myWorkSessionResultDS.Patients.AcceptChanges()
                            End If

                            'Re-sort the Patients grid
                            Dim myOrderTestsDelegate As New OrderTestsDelegate
                            myOrderTestsDelegate.SortPatients(myWorkSessionResultDS)

                            'Re-Calculate autonumeric SampleIDs
                            CalculateAllAutonumeric()

                            'Change the Priority also in the Stat CheckBox in the header area if the Patient was selected
                            If (bsPatientIDTextBox.Text.Trim <> "") Then bsStatCheckbox.Checked = (Not selectedStatFlag)

                            ChangesMadeAttribute = True
                        End If
                        iconToShow = Nothing
                    End If
                    lstWSPatientsDS = Nothing
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangePatientOrderPriority", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangePatientOrderPriority", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the changing of the SampleID for the selected Patient Order Test:
    ''' ** The informed SampleID is validated to known if it corresponds to a Patient that exists in the
    '''    DB or if it is a manual one
    ''' ** If for the informed SampleID and StatFlag there are requested Order Tests, a merge is executed
    '''    to avoid duplicates by TestID/SampleType
    ''' ** If the changed SampleID was an automatically generated one, then the rest of autonumeric SampleID
    '''    in the grid are re-calculated
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 16/04/2010 - Code moved from the event; changes to improve the code
    ''' Modified by: SA 11/05/2010 - Added changes needed to manage also Calculated Tests (updation of TubeType only for Standard
    '''                              Tests; when merge Tests, use also the Test Type to control equal rows).  Once the change finishes,
    '''                              update also value in the PatientID textbox in the header are
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode
    '''              XB 08/03/2013 - Add ISE case into condition to get the selected TubeType
    '''              SA 29/08/2013 - For the new informed PatientID/SampleID, verify if there is at least an Order Test requested by an external LIS system for the same
    '''                              Patient and SampleType, and in this case, get value of the SpecimenID and assign it to all the affected Order Tests
    ''' </remarks>
    Private Sub ChangePatientSampleIDColumn()
        Try
            Dim autoIDRecalculation As Boolean = False
            If (Not bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").ReadOnly) Then
                If (bsPatientOrdersDataGridView.CurrentCell.Value.ToString = "") Then
                    'If cell SampleID is empty... the original SampleID value is recovered
                    bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value = textBoxOriginal
                Else
                    'If the SampleID to change was an autonumeric one, then the rest of autonumeric SampleIDs 
                    'in the grid has to be recalculated
                    Dim mySampleIDType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleIDType").Value.ToString
                    autoIDRecalculation = (mySampleIDType = "AUTO")

                    Dim newSampleID As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value.ToString
                    If (newSampleID.Substring(0, 1) = "#") Then
                        newSampleID = newSampleID.Substring(1)
                    End If

                    'Verify if records have been already added for the informed SampleID
                    Dim executeChg As Boolean = True
                    Dim lstPatientExist As List(Of WorkSessionResultDS.PatientsRow)
                    lstPatientExist = (From a In myWorkSessionResultDS.Patients _
                                      Where a.SampleID.ToUpperInvariant = newSampleID.ToUpperInvariant _
                                    AndAlso a.SampleID <> newSampleID _
                                     Select a).ToList()

                    If (lstPatientExist.Count > 0) Then
                        'To be sure all records will have the same format in the SampleID (upper/lower case)
                        newSampleID = lstPatientExist.First.SampleID
                        mySampleIDType = lstPatientExist.First.SampleIDType
                    Else
                        'Verify if the informed SampleID corresponds a PatientID that exists in the DB
                        Dim myGlobalDataTO As New GlobalDataTO

                        Dim myPatientDelegate As New PatientDelegate
                        myGlobalDataTO = myPatientDelegate.GetPatientData(Nothing, newSampleID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myPatientDS As PatientsDS = DirectCast(myGlobalDataTO.SetDatos, PatientsDS)

                            mySampleIDType = "MAN"
                            If (myPatientDS.tparPatients.Rows.Count > 0) Then mySampleIDType = "DB"
                        Else
                            'Show the error message
                            ShowMessage(Name & ".ChangePatientSampleIDColumn", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            executeChg = False
                        End If
                    End If
                    lstPatientExist = Nothing

                    If (executeChg) Then
                        'Get values of StatFlag, TestType, TestID and SampleType of the selected row
                        Dim myStatFlag As Boolean = Convert.ToBoolean(bsPatientOrdersDataGridView.CurrentRow.Cells("StatFlag").Value)
                        Dim myTestType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("Testtype").Value.ToString
                        'Dim myTestID As Integer = Convert.ToInt32(bsPatientOrdersDataGridView.CurrentRow.Cells("TestID").Value)
                        Dim mySampleType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString

                        'Only if the current row corresponds to an Standard/ISE Test, get the selected TubeType
                        Dim myTubeType As String = ""
                        If (myTestType = "STD") Or (myTestType = "ISE") Then myTubeType = bsPatientOrdersDataGridView.CurrentRow.Cells("TubeType").Value.ToString

                        Dim lstWSPatient As List(Of WorkSessionResultDS.PatientsRow)

                        'Get all patients with the same newSampleID and mySampleType in order to obtain and set a unique TubeType for all of them
                        'Use only Standard Tests (Calculated Tests have the TubeType column empty)
                        lstWSPatient = (From a In myWorkSessionResultDS.Patients _
                                       Where a.SampleClass = "PATIENT" _
                                     AndAlso a.SampleID.ToUpperInvariant = newSampleID.ToUpperInvariant _
                                     AndAlso a.SampleType = mySampleType _
                                     AndAlso (a.TestType = "STD" OrElse a.TestType = "ISE") _
                                      Select a).ToList()

                        If (lstWSPatient.Count > 0) Then myTubeType = lstWSPatient.First.TubeType

                        'SA 29/08/2013 - See comments about this code in the function header
                        'Verify if there are already Order Tests requested for LIS for the same Patient and SampleType to get the SpecimenID (Barcode) for it
                        Dim mySpecimenID As String = String.Empty
                        lstWSPatient = (From a In myWorkSessionResultDS.Patients _
                                       Where a.SampleClass = "PATIENT" _
                                     AndAlso a.SampleID.ToUpperInvariant = newSampleID.ToUpperInvariant _
                                     AndAlso a.SampleType = mySampleType _
                                     AndAlso a.LISRequest _
                                 AndAlso Not a.IsSpecimenIDNull _
                                      Select a).ToList()
                        If (lstWSPatient.Count > 0) Then mySpecimenID = lstWSPatient.First.SpecimenID

                        'Search all OPEN Patient Order Tests having the same SampleID (the one before change) and StatFlag, and update values of columns
                        'SampleID and SampleIDType for the new ones
                        lstWSPatient = (From a In myWorkSessionResultDS.Patients _
                                       Where a.SampleClass = "PATIENT" _
                                     AndAlso a.OTStatus = "OPEN" _
                                     AndAlso a.SampleID = textBoxOriginal _
                                     AndAlso a.StatFlag = myStatFlag _
                                      Select a).ToList()

                        For Each samePatientStat As WorkSessionResultDS.PatientsRow In lstWSPatient
                            samePatientStat.SampleID = newSampleID
                            samePatientStat.SampleIDType = mySampleIDType

                            'SA 29/08/2013 - Assign the SpecimenID when informed...
                            If (mySpecimenID <> String.Empty) Then samePatientStat.SpecimenID = mySpecimenID
                        Next

                        If ((myTestType = "STD" OrElse myTestType = "ISE") AndAlso myTubeType.Trim <> "") Then
                            'The TubeType should be changed. Find all rows corresponding to Standard Tests, with the same PatientID/SampleID and SampleType
                            lstWSPatient = (From a In myWorkSessionResultDS.Patients _
                                           Where a.SampleClass = "PATIENT" _
                                         AndAlso a.OTStatus = "OPEN" _
                                         AndAlso a.SampleID.ToUpperInvariant = newSampleID.ToUpperInvariant _
                                         AndAlso a.SampleType = mySampleType _
                                         AndAlso (a.TestType = "STD" OrElse a.TestType = "ISE") _
                                          Select a).ToList()

                            For Each patientRow As WorkSessionResultDS.PatientsRow In lstWSPatient
                                patientRow.TubeType = myTubeType
                            Next
                        End If

                        'Change value of field SampleIDType for the row that has been changed
                        lstWSPatient = (From a In myWorkSessionResultDS.Patients _
                                       Where a.SampleClass = "PATIENT" _
                                     AndAlso a.OTStatus = "OPEN" _
                                     AndAlso a.SampleID.ToUpperInvariant = newSampleID.ToUpperInvariant _
                                     AndAlso a.StatFlag = myStatFlag _
                                     AndAlso a.SampleIDType <> mySampleIDType _
                                      Select a).ToList()

                        If (lstWSPatient.Count = 1) Then lstWSPatient.First.SampleIDType = mySampleIDType
                        myWorkSessionResultDS.Patients.AcceptChanges()
                        lstWSPatient = Nothing

                        If (bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value.ToString <> newSampleID) Then
                            patientCaseChanged = True
                            bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value = newSampleID
                            patientCaseChanged = False
                        End If
                        bsPatientOrdersDataGridView.CurrentRow.Cells("SampleIDType").Value = mySampleIDType

                        'SA 29/08/2013 - Assign the SpecimenID when informed...
                        If (mySpecimenID <> String.Empty) Then bsPatientOrdersDataGridView.CurrentRow.Cells("SpecimenID").Value = mySpecimenID
                        bsPatientOrdersDataGridView.UpdateCellValue(bsPatientOrdersDataGridView.CurrentCell.ColumnIndex, bsPatientOrdersDataGridView.CurrentCell.RowIndex)

                        'Search if after the change there are duplicated Order Tests (same TestType, TestID and SampleType) for the new SampleID and StatFlag and 
                        'execute the merge
                        MergePatientOrderTests(newSampleID, myStatFlag)
                        myWorkSessionResultDS.Patients.AcceptChanges()

                        'If the changed SampleID was an automatically generated one, then the rest of autonumeric SampleIDs 
                        'in the grid has to be recalculated
                        If (autoIDRecalculation) Then
                            CalculateAllAutonumeric()
                        End If

                        'Re-sort the Patients grid
                        Dim myOrderTestDelegate As New OrderTestsDelegate
                        myOrderTestDelegate.SortPatients(myWorkSessionResultDS)

                        'Change Sample value also in the Patient TextBox in the header area
                        If (bsPatientIDTextBox.Text.Trim <> "") Then bsPatientIDTextBox.Text = newSampleID
                        ChangesMadeAttribute = True
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangePatientSampleIDColumn", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangePatientSampleIDColumn", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' If a Patient is selected, select also the required Calibrator and Blank
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 16/04/2010 - Code moved from the event
    ''' Modified by: SA 12/07/2010 - Modified code to manage the unselection of a Patient Sample
    '''              SA 19/06/2012 - When a requested Test for a Patient Sample is unselected, verify if the needed Control 
    '''                              can be also unselected also for ISE Tests
    ''' </remarks>
    Private Sub ChangePatientSelectedColumn()
        Try
            Dim selSampleID As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value.ToString
            Dim selStatFlag As Boolean = Convert.ToBoolean(bsPatientOrdersDataGridView.CurrentRow.Cells("StatFlag").Value)
            Dim selSampleType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString
            Dim selTestID As Integer = CInt(bsPatientOrdersDataGridView.CurrentRow.Cells("TestID").Value)

            Dim myOrderTestsDelegate As New OrderTestsDelegate
            If CBool(bsPatientOrdersDataGridView.CurrentRow.Cells("Selected").Value) Then
                If (bsPatientOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString = "CALC") Then
                    myOrderTestsDelegate.SelectCalcTestComponents(selSampleID, selStatFlag, selTestID, _
                                                                  myWorkSessionResultDS)

                ElseIf (bsPatientOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString = "STD") Then
                    myOrderTestsDelegate.SelectAllNeededOrderTests(bsPatientOrdersDataGridView.CurrentRow.Cells("SampleClass").Value.ToString, _
                                                                   bsPatientOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString, _
                                                                   selTestID, selSampleType, _
                                                                   myWorkSessionResultDS)
                End If
            Else
                'If a Test that is part of the formula of some requested Calculated Tests is unselected, 
                'then the Calculated Tests are also unselected
                If (bsPatientOrdersDataGridView.CurrentRow.Cells("CalcTestID").Value.ToString <> "") Then
                    UnSelectRelatedCalcTest(selSampleID, selStatFlag, _
                                            bsPatientOrdersDataGridView.CurrentRow.Cells("CalcTestID").Value.ToString)
                End If

                'For Standard and ISE Tests, if it is possible, unselect also the Control for the same TestType/TestID and SampleType
                'For Standard Tests, if it is possible, unselect also the Calibrator and Blank for the same TestType/TestID and SampleType
                Dim myTestType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString
                If (myTestType = "STD" OrElse myTestType = "ISE") Then
                    'Verify if there is a request for the same TestType/TestID/SampleType for a different SampleID
                    'or for the same SampleID with a different StatFlag
                    Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
                    lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                      Where a.SampleClass = "PATIENT" _
                                    AndAlso (a.SampleID <> selSampleID OrElse _
                                            (a.SampleID = selSampleID AndAlso a.StatFlag <> selStatFlag)) _
                                    AndAlso a.TestType = myTestType _
                                    AndAlso a.SampleType = selSampleType _
                                    AndAlso a.TestID = selTestID _
                                     Select a).ToList

                    If (lstWSPatientsDS.Count = 0) Then
                        'Unselect Controls used for the same TestType/TestID/SampleType
                        Dim lstWSControlsS As List(Of WorkSessionResultDS.ControlsRow)
                        lstWSControlsS = (From a In myWorkSessionResultDS.Controls _
                                         Where a.SampleClass = "CTRL" _
                                       AndAlso a.TestType = myTestType _
                                       AndAlso a.SampleType = selSampleType _
                                       AndAlso a.TestID = selTestID _
                                        Select a).ToList

                        If (lstWSControlsS.Count > 0) Then
                            For Each ctrl As WorkSessionResultDS.ControlsRow In lstWSControlsS
                                ctrl.Selected = False
                            Next
                            myWorkSessionResultDS.Controls.AcceptChanges()
                        End If
                        lstWSControlsS = Nothing

                        If (myTestType = "STD") Then
                            'Unselect the Calibrator and Blank if it is possible
                            UnselectBlkCalibrator("STD", selSampleType, selTestID)
                        End If
                    End If
                    lstWSPatientsDS = Nothing
                End If
            End If
            RefreshGrids()
            ChangesMadeAttribute = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangePatientSelectedColumn", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangePatientSelectedColumn", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' After changing value of Selected column for a Patient, verify the state of the CheckBox that
    ''' allows select/unselect all the selectable elements in the grid
    ''' For event bsPatientOrdersDataGridView_CurrentCellDirtyStateChanged
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/04/2010 - Code moved from the event; changes to improve the code
    ''' </remarks>
    Private Sub ChangePatientSelectedState()
        If (TypeOf bsPatientOrdersDataGridView.CurrentCell Is DataGridViewCheckBoxCell) OrElse _
           (TypeOf bsPatientOrdersDataGridView.CurrentCell Is DataGridViewComboBoxCell) Then
            bsPatientOrdersDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)

            If (bsPatientOrdersDataGridView.CurrentCell.OwningColumn.Name = "Selected") Then
                RowPatientCheckBoxClick(Nothing)
            End If
        End If
    End Sub

    ''' <summary>
    ''' When a TubeType is changed, it has to be changed for all requested Patient Order Tests having the same 
    ''' 'PatientID/SampleID and SampleType. For event bsPatientOrdersDataGridView_CellValueChanged
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 16/04/2010 - Code moved from the event; changes to improve the code
    ''' Modified by: SA 11/05/2010 - The Tube Type change has not to be applied for rows containing Calculated Tests
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode
    ''' </remarks>
    Private Sub ChangePatientTubeTypeColumn()
        Try
            If (Not bsPatientOrdersDataGridView.CurrentRow.Cells("TubeType").ReadOnly) Then
                Dim mySampleID As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value.ToString
                Dim mySampleType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString
                Dim myTubeType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("TubeType").Value.ToString

                'The TubeType can be changed, find all rows containing Standard and/or ISE Tests with the same PatientID/SampleID and SampleType 
                Dim lstWSPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
                lstWSPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                  Where a.SampleClass = "PATIENT" _
                                AndAlso a.SampleID.ToUpperInvariant = mySampleID.ToUpperInvariant _
                                AndAlso a.SampleType = mySampleType _
                                AndAlso (a.TestType = "STD" OrElse a.TestType = "ISE") _
                                 Select a).ToList()

                For Each patientRow As WorkSessionResultDS.PatientsRow In lstWSPatientsDS
                    patientRow.TubeType = myTubeType
                Next
                myWorkSessionResultDS.Patients.AcceptChanges()
                lstWSPatientsDS = Nothing

                ChangesMadeAttribute = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangePatientTubeTypeColumn", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangePatientTubeTypeColumn", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' For event bsBlkCalibDataGridView_RowsAdded
    ''' </summary>
    ''' <remarks>
    ''' Created by:  GDS
    ''' Modified by: DL 09/03/2010
    '''              SA 17/03/2010 - CheckBox for select/unselect all Blank and Calibrators is only for those with Status OPEN                              
    '''              XB 26/08/2014 - changes to control availability of both Check Boxes - BT #1868
    ''' </remarks>
    Private Sub CheckAddRemoveBlkCalRows()
        Try
            If (Not isHeaderBlkCalCheckBoxClicked) Then
                'Check how many OPEN Blanks and Calibrators are currently in the grid of Blank and Calibrator Order Tests
                Dim lstWSOpenDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)

                ' Blanks
                lstWSOpenDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                               Where a.SampleClass = "BLANK" _
                               AndAlso a.OTStatus = "OPEN" _
                               Select a).ToList()
                totalBlankCheckBoxes = lstWSOpenDS.Count

                ' Calibrators
                lstWSOpenDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                               Where a.SampleClass = "CALIB" _
                               AndAlso a.OTStatus = "OPEN" _
                               Select a).ToList()
                totalCalibCheckBoxes = lstWSOpenDS.Count


                'Check how many of the opened Blanks and Calibrators are currently selected
                Dim lstWSSelectedDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)

                ' Blanks
                lstWSSelectedDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                               Where a.SampleClass = "BLANK" _
                               AndAlso a.OTStatus = "OPEN" _
                                AndAlso a.Selected = True _
                                 Select a).ToList()
                totalBlankCheckedCheckBoxes = lstWSSelectedDS.Count

                ' Calibrators
                lstWSSelectedDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                               Where a.SampleClass = "CALIB" _
                               AndAlso a.OTStatus = "OPEN" _
                                AndAlso a.Selected = True _
                                 Select a).ToList()
                totalCalibCheckedCheckBoxes = lstWSSelectedDS.Count

                'Delete button is enabled only if there is at least a Blank or Calibrator with status OPEN
                bsDelCalibratorsButton.Enabled = (totalBlankCheckBoxes + totalCalibCheckBoxes > 0)

                'Control for Check/Uncheck all Blanks is enabled only when there is at least a Blank with status OPEN
                bsAllBlanksCheckBox.Enabled = (totalBlankCheckBoxes > 0)
                'Control for Check/Uncheck all Calibrators is enabled only when there is at least a Calibrator with status OPEN
                bsAllCalibsCheckBox.Enabled = (totalCalibCheckBoxes > 0)

                '...additionally, it is Checked when all Blanks with Status OPEN are selected
                bsAllBlanksCheckBox.Checked = (totalBlankCheckBoxes > 0) AndAlso _
                                              (totalBlankCheckedCheckBoxes = totalBlankCheckBoxes)
                '...additionally, it is Checked when all Calibrators with Status OPEN are selected
                bsAllCalibsCheckBox.Checked = (totalCalibCheckBoxes > 0) AndAlso _
                                              (totalCalibCheckedCheckBoxes = totalCalibCheckBoxes)

                lstWSOpenDS = Nothing
                lstWSSelectedDS = Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CheckAddRemoveBlkCalRows", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CheckAddRemoveBlkCalRows", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' For events bsControlDataGridView_RowsAdded and bsControlDataGridView_RowsRemoved
    ''' </summary>
    ''' <remarks>
    ''' Created by:  GDS
    ''' Modified by: DL 09/03/2010
    '''              SA 17/03/2010 - CheckBox for select/unselect all Controls is only for those with Status OPEN      
    '''              TR 12/03/2013 - Add filter a.LISRequest = False  on lsWSOpenDS to disable Delete button on OT Requested by LIS.     
    ''' </remarks>
    Private Sub CheckAddRemoveControlsRows()
        Try
            'Check how many OPEN Controls are currently in the grid of Control Order Tests
            Dim lstWSOpenDS As List(Of WorkSessionResultDS.ControlsRow)
            lstWSOpenDS = (From a In myWorkSessionResultDS.Controls _
                          Where a.OTStatus = "OPEN" _
                          Select a).ToList()
            totalControlCheckBoxes = lstWSOpenDS.Count

            'Check how many of the opened Controls are currently selected
            Dim lstWSSelectedDS As List(Of WorkSessionResultDS.ControlsRow)
            lstWSSelectedDS = (From a In myWorkSessionResultDS.Controls _
                              Where a.OTStatus = "OPEN" _
                            AndAlso a.Selected = True _
                             Select a).ToList()
            totalControlCheckedCheckBoxes = lstWSSelectedDS.Count



            'Control for Check/Uncheck all Controls is enabled only when there is at least a Control with status OPEN
            bsAllCtrlsCheckBox.Enabled = (lstWSOpenDS.Count > 0)

            '...additionally, it is Checked when all Controls with Status OPEN are selected
            bsAllCtrlsCheckBox.Checked = (lstWSOpenDS.Count > 0) AndAlso _
                                         (totalControlCheckedCheckBoxes = totalControlCheckBoxes)


            'Check how many OPEN Controls are currently in the grid of Control Order Tests
            lstWSOpenDS = (From a In myWorkSessionResultDS.Controls _
                          Where a.OTStatus = "OPEN" _
                          AndAlso a.LISRequest = False
                         Select a).ToList()
            'Delete button is enabled only if there is at least a Control with status OPEN
            bsDelControlsButton.Enabled = (lstWSOpenDS.Count > 0)

            lstWSOpenDS = Nothing
            lstWSSelectedDS = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CheckAddRemoveControlsRows", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CheckAddRemoveControlsRows", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' For events bsPatientDataGridView_RowsAdded and bsPatientDataGridView_RowsRemoved 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  GDS
    ''' Modified by: DL 09/03/2010
    '''              SA 17/03/2010 - CheckBox for select/unselect all Patient Samples is only for those with Status OPEN           
    '''              TR 12/03/2013 - Add filter a.LISRequest = False on lsWSOpenDS to disable Delete button on OT Requested by LIS.
    ''' </remarks>
    Private Sub CheckAddRemovePatientsRows()
        Try
            'Check how many OPEN Patient Samples are currently in the grid of Patient Order Tests
            '(Off-System Tests are not counted due to they can not be selected/unselected)
            Dim lstWSOpenDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSOpenDS = (From a In myWorkSessionResultDS.Patients _
                          Where a.OTStatus = "OPEN" _
                            AndAlso a.TestType <> "OFFS" _
                         Select a).ToList()
            totalPatientCheckBoxes = lstWSOpenDS.Count

            'Check how many of the opened Patient Samples are currently selected
            Dim lstWSSelectedDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSSelectedDS = (From a In myWorkSessionResultDS.Patients _
                              Where a.OTStatus = "OPEN" _
                            AndAlso a.Selected = True _
                             Select a).ToList()
            totalPatientCheckedCheckBoxes = lstWSSelectedDS.Count


            'Control for Check/Uncheck all Patient Samples is enabled only when there is at least a Patient Sample with status OPEN
            bsAllPatientsCheckBox.Enabled = (lstWSOpenDS.Count > 0)

            '...additionally, it is Checked when all Patient Samples with Status OPEN are selected
            bsAllPatientsCheckBox.Checked = (lstWSOpenDS.Count > 0) AndAlso _
                                            (totalPatientCheckedCheckBoxes = totalPatientCheckBoxes)

            'TR Set new filter to validate if enable the 
            lstWSOpenDS = (From a In myWorkSessionResultDS.Patients _
              Where a.OTStatus = "OPEN" _
                AndAlso a.TestType <> "OFFS" _
                AndAlso a.LISRequest = False _
             Select a).ToList()
            'Delete button is enabled only if there is at least a Patient with status OPEN
            bsDelPatientsButton.Enabled = (lstWSOpenDS.Count > 0)


            lstWSOpenDS = Nothing
            lstWSSelectedDS = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CheckAddRemovePatientsRows", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CheckAddRemovePatientsRows", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the value entered is numeric. String values are not allowed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 25/11/2010
    ''' Modified by: SA 21/02/2012 - Implementation changed, the previous one did not work properly
    ''' </remarks>
    Private Sub CheckNumericCell(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Try
            If (e.KeyChar = CChar("") OrElse e.KeyChar = ChrW(Keys.Back)) Then
                e.Handled = False
            Else
                Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
                If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                    If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                        e.KeyChar = CChar(myDecimalSeparator)
                    End If

                    If (CType(sender, TextBox).Text.Contains(".") Or CType(sender, TextBox).Text.Contains(",")) Then
                        e.Handled = True
                    Else
                        e.Handled = False
                    End If
                Else
                    If (Not IsNumeric(e.KeyChar)) Then
                        e.Handled = True
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CheckNumericCell ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CheckNumericCell ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ' XB 26/08/2014 - Comment function ClickAllBlkCalibCheckBox, it will not be used anymore - BT #1868
    ' ''' <summary>
    ' ''' Manage the check/uncheck of all selectable rows in the grid of Blanks and Calibrators
    ' ''' For event bsAllBlkCalCheckBox_MouseClick
    ' ''' </summary>
    ' ''' <remarks>
    ' ''' Created by:  SA 12/04/2010 - Code moved from the event; changes to fix errors
    ' ''' Modified by: SA 15/04/2010 - When a Calibrator used as alternative for different SampleTypes is unselected, verify that there are
    ' '''                              not Patient or Control Order Tests requested for any of the requested Sample Types
    ' '''              SA 09/11/2012 - When function VerifyUnselectedOrderTest is called to validate if there are Patient or Control Order Tests 
    ' '''                              requested for any of the requested Sample Types, parameter with the "real" SampleType (the one for which
    ' '''                              the Calibrator was defined) is informed (due to when validation of existing previous results for the 
    ' '''                              Calibrator is done, the informed SampleType has to be always the one for which the Calibrator was defined)
    ' ''' </remarks>
    'Private Sub ClickAllBlkCalibCheckBox()
    '    Try
    '        Dim numLocked As Integer = 0
    '        isHeaderBlkCalCheckBoxClicked = True

    '        'Empty the collection of selected rows in the grid of Blank and Calibrators
    '        bsBlkCalibDataGridView.ClearSelection()

    '        Dim checkedValue As Boolean = bsAllBlanksCheckBox.Checked
    '        Dim myOrderTestsDelegate As New OrderTestsDelegate

    '        'Get all Calibrators that can be selected/unselected
    '        Dim lstWSOpenCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
    '        lstWSOpenCalibratorsDS = (From a In myWorkSessionResultDS.BlankCalibrators _
    '                                 Where a.SampleClass = "CALIB" _
    '                               AndAlso a.OTStatus = "OPEN" _
    '                                Select a).ToList()

    '        Dim verifyUnCheck As Boolean
    '        For Each openCalibRow As WorkSessionResultDS.BlankCalibratorsRow In lstWSOpenCalibratorsDS
    '            verifyUnCheck = True
    '            If (Not checkedValue) Then
    '                'Verify if the Calibrator can be unselected: there are not selected Patient and/or Control Order Tests using it
    '                verifyUnCheck = myOrderTestsDelegate.VerifyUnselectedOrderTest(openCalibRow.SampleClass.ToString, openCalibRow.TestType.ToString, _
    '                                                                               CInt(openCalibRow.TestID), openCalibRow.SampleType.ToString, _
    '                                                                               myWorkSessionResultDS)
    '            End If

    '            If (verifyUnCheck) Then
    '                If (Not checkedValue) Then
    '                    If (openCalibRow.RequestedSampleTypes <> openCalibRow.SampleType) Then
    '                        'Verify also if there are not selected Patient and/or Control Order Tests using any of the requested Sample Types
    '                        Dim additionalSampleTypes() As String = Split(openCalibRow.RequestedSampleTypes.Trim)

    '                        For Each reqSampleType As String In additionalSampleTypes
    '                            verifyUnCheck = myOrderTestsDelegate.VerifyUnselectedOrderTest(openCalibRow.SampleClass.ToString, openCalibRow.TestType.ToString, _
    '                                                                                           CInt(openCalibRow.TestID), reqSampleType.ToString, _
    '                                                                                           myWorkSessionResultDS, openCalibRow.SampleType)
    '                            If (Not verifyUnCheck) Then Exit For
    '                        Next
    '                    End If
    '                End If

    '                If (verifyUnCheck) Then
    '                    openCalibRow.Selected = checkedValue
    '                    If (Not openCalibRow.IsPreviousOrderTestIDNull) Then openCalibRow.NewCheck = checkedValue
    '                Else
    '                    If (Not checkedValue) Then numLocked += 1
    '                End If
    '            Else
    '                If (Not checkedValue) Then numLocked += 1
    '            End If
    '        Next
    '        myWorkSessionResultDS.BlankCalibrators.AcceptChanges()
    '        lstWSOpenCalibratorsDS = Nothing

    '        'Get all Blanks that can be selected/unselected
    '        Dim lstWSOpenBlanksDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
    '        lstWSOpenBlanksDS = (From a In myWorkSessionResultDS.BlankCalibrators _
    '                            Where a.SampleClass = "BLANK" _
    '                          AndAlso a.OTStatus = "OPEN" _
    '                           Select a).ToList()

    '        For Each openBlankRow As WorkSessionResultDS.BlankCalibratorsRow In lstWSOpenBlanksDS
    '            verifyUnCheck = True
    '            If (Not checkedValue) Then
    '                'Verify if the Blank can be unselected: there are not selected Patient, Control and/or Calibrator Order Tests using it
    '                verifyUnCheck = myOrderTestsDelegate.VerifyUnselectedOrderTest(openBlankRow.SampleClass.ToString, openBlankRow.TestType.ToString, _
    '                                                                               CInt(openBlankRow.TestID), openBlankRow.SampleType.ToString, _
    '                                                                               myWorkSessionResultDS)
    '            End If

    '            If (verifyUnCheck) Then
    '                openBlankRow.Selected = checkedValue
    '                If (Not openBlankRow.IsPreviousOrderTestIDNull) Then openBlankRow.NewCheck = checkedValue
    '            Else
    '                If (Not checkedValue) Then numLocked += 1
    '            End If
    '        Next
    '        myWorkSessionResultDS.BlankCalibrators.AcceptChanges()
    '        lstWSOpenBlanksDS = Nothing

    '        bsBlkCalibDataGridView.RefreshEdit()
    '        totalBlkCalCheckedCheckBoxes = If(checkedValue, totalBlkCalCheckBoxes, 0)

    '        'If all rows remained checked and the CheckBox in the header was unchecked, it is checked again
    '        If (Not checkedValue) AndAlso (numLocked = bsBlkCalibDataGridView.Rows.Count) Then
    '            bsAllBlanksCheckBox.Checked = True
    '        End If

    '        'Verify if OpenRotor button can be enabled
    '        OpenRotorButtonEnabled()

    '        isHeaderBlkCalCheckBoxClicked = False
    '        ChangesMadeAttribute = True
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ClickAllBlkCalibCheckBox", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".ClickAllBlkCalibCheckBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub


    ''' <summary>
    ''' Manage the check/uncheck of all selectable rows in the grid just for the Blanks
    ''' For event bsAllBlanksCheckBox_MouseClick
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 26/08/2014 - from older function ClickAllBlkCalCheckBox with the code for SampleClass BLANKS extracted - BT #1868
    ''' </remarks>
    Private Sub ClickAllBlanksCheckBox()
        Try
            Dim numLocked As Integer = 0
            isHeaderBlkCalCheckBoxClicked = True

            'Empty the collection of selected rows in the grid of Blank and Calibrators
            bsBlkCalibDataGridView.ClearSelection()

            Dim checkedValue As Boolean = bsAllBlanksCheckBox.Checked
            Dim myOrderTestsDelegate As New OrderTestsDelegate

            Dim verifyUnCheck As Boolean

            'Get all Blanks that can be selected/unselected
            Dim lstWSOpenBlanksDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
            lstWSOpenBlanksDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                                Where a.SampleClass = "BLANK" _
                              AndAlso a.OTStatus = "OPEN" _
                               Select a).ToList()

            Dim OpenBlanksNum As Integer = lstWSOpenBlanksDS.Count

            For Each openBlankRow As WorkSessionResultDS.BlankCalibratorsRow In lstWSOpenBlanksDS
                verifyUnCheck = True
                If (Not checkedValue) Then
                    'Verify if the Blank can be unselected: there are not selected Patient, Control and/or Calibrator Order Tests using it
                    verifyUnCheck = myOrderTestsDelegate.VerifyUnselectedOrderTest(openBlankRow.SampleClass.ToString, openBlankRow.TestType.ToString, _
                                                                                   CInt(openBlankRow.TestID), openBlankRow.SampleType.ToString, _
                                                                                   myWorkSessionResultDS)
                End If

                If (verifyUnCheck) Then
                    openBlankRow.Selected = checkedValue
                    If (Not openBlankRow.IsPreviousOrderTestIDNull) Then openBlankRow.NewCheck = checkedValue
                Else
                    If (Not checkedValue) Then numLocked += 1
                End If
            Next
            myWorkSessionResultDS.BlankCalibrators.AcceptChanges()
            lstWSOpenBlanksDS = Nothing

            bsBlkCalibDataGridView.RefreshEdit()
            totalBlankCheckedCheckBoxes = If(checkedValue, totalBlankCheckBoxes, 0)

            'If all rows remained checked and the CheckBox in the header was unchecked, it is checked again
            If (Not checkedValue) AndAlso (numLocked = OpenBlanksNum) Then
                bsAllBlanksCheckBox.Checked = True
            End If

            'Verify if OpenRotor button can be enabled
            OpenRotorButtonEnabled()

            isHeaderBlkCalCheckBoxClicked = False
            ChangesMadeAttribute = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ClickAllBlanksCheckBox", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ClickAllBlanksCheckBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Manage the check/uncheck of all selectable rows in the grid just for the Calibrators
    ''' For event bsAllCalibsCheckBox_MouseClick
    ''' </summary>
    ''' <remarks>
    ''' Created by:  XB 26/08/2014 - from older function ClickAllBlkCalCheckBox with the code for SampleClass CALIBS extracted - BT #1868
    ''' </remarks>
    Private Sub ClickAllCalibsCheckBox()
        Try
            Dim numLocked As Integer = 0
            isHeaderBlkCalCheckBoxClicked = True

            'Empty the collection of selected rows in the grid of Blank and Calibrators
            bsBlkCalibDataGridView.ClearSelection()

            Dim checkedValue As Boolean = bsAllCalibsCheckBox.Checked
            Dim myOrderTestsDelegate As New OrderTestsDelegate

            'Get all Calibrators that can be selected/unselected
            Dim lstWSOpenCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
            lstWSOpenCalibratorsDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                                     Where a.SampleClass = "CALIB" _
                                   AndAlso a.OTStatus = "OPEN" _
                                    Select a).ToList()

            Dim OpenCalibsNum As Integer = lstWSOpenCalibratorsDS.Count

            Dim verifyUnCheck As Boolean
            For Each openCalibRow As WorkSessionResultDS.BlankCalibratorsRow In lstWSOpenCalibratorsDS
                verifyUnCheck = True
                If (Not checkedValue) Then
                    'Verify if the Calibrator can be unselected: there are not selected Patient and/or Control Order Tests using it
                    verifyUnCheck = myOrderTestsDelegate.VerifyUnselectedOrderTest(openCalibRow.SampleClass.ToString, openCalibRow.TestType.ToString, _
                                                                                   CInt(openCalibRow.TestID), openCalibRow.SampleType.ToString, _
                                                                                   myWorkSessionResultDS)
                End If

                If (verifyUnCheck) Then
                    If (Not checkedValue) Then
                        If (openCalibRow.RequestedSampleTypes <> openCalibRow.SampleType) Then
                            'Verify also if there are not selected Patient and/or Control Order Tests using any of the requested Sample Types
                            Dim additionalSampleTypes() As String = Split(openCalibRow.RequestedSampleTypes.Trim)

                            For Each reqSampleType As String In additionalSampleTypes
                                verifyUnCheck = myOrderTestsDelegate.VerifyUnselectedOrderTest(openCalibRow.SampleClass.ToString, openCalibRow.TestType.ToString, _
                                                                                               CInt(openCalibRow.TestID), reqSampleType.ToString, _
                                                                                               myWorkSessionResultDS, openCalibRow.SampleType)
                                If (Not verifyUnCheck) Then Exit For
                            Next
                        End If
                    End If

                    If (verifyUnCheck) Then
                        openCalibRow.Selected = checkedValue
                        If (Not openCalibRow.IsPreviousOrderTestIDNull) Then openCalibRow.NewCheck = checkedValue
                    Else
                        If (Not checkedValue) Then numLocked += 1
                    End If
                Else
                    If (Not checkedValue) Then numLocked += 1
                End If
            Next
            myWorkSessionResultDS.BlankCalibrators.AcceptChanges()
            lstWSOpenCalibratorsDS = Nothing


            'Get all different TestID and Sample Type for all selected Calibrators in order to search all needed Blanks
            Dim listOfDifElem As List(Of String)
            listOfDifElem = (From a In myWorkSessionResultDS.BlankCalibrators _
                            Where a.SampleClass = "CALIB" _
                            AndAlso a.OTStatus = "OPEN" _
                            Select String.Format("{0}|{1}|{2}", a.TestType, a.TestID, a.SampleType) Distinct).ToList()

            For Each difElement As String In listOfDifElem
                Dim elements As String() = difElement.Split(CChar("|"))
                myOrderTestsDelegate.SelectAllNeededOrderTests("CALIB", elements(0), CInt(elements(1)), elements(2), myWorkSessionResultDS)

                isHeaderBlkCalCheckBoxClicked = False
                ChangeBlankCalibratorSelectedColumn()
                isHeaderBlkCalCheckBoxClicked = True
            Next
            listOfDifElem = Nothing


            bsBlkCalibDataGridView.RefreshEdit()
            totalCalibCheckedCheckBoxes = If(checkedValue, totalCalibCheckBoxes, 0)

            'If all rows remained checked and the CheckBox in the header was unchecked, it is checked again
            If (Not checkedValue) AndAlso (numLocked = OpenCalibsNum) Then
                bsAllCalibsCheckBox.Checked = True
            End If

            'Verify if OpenRotor button can be enabled
            OpenRotorButtonEnabled()

            isHeaderBlkCalCheckBoxClicked = False
            ChangesMadeAttribute = True

            RowBlkCalCheckBoxClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ClickAllCalibsCheckBox", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ClickAllCalibsCheckBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manage the check/uncheck of all selectable rows in the grid of Controls
    ''' For event bsAllCtrlsCheckBox_MouseClick
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/04/2010 - Code moved from the event; changes to fix errors
    ''' Modified by: SA 28/04/2010 - Changed the way of calling function SelectAllNeededOrderTests: execute it for 
    '''                              each different TestType, TestID and SampleType instead of for each row in the 
    '''                              grid (due to long time execution when the grid is loaded with lot of rows)
    ''' </remarks>
    Private Sub ClickAllControlCheckBox()
        Try
            isHeaderControlCheckBoxClicked = True

            'Empty the collection of selected rows in the grid of Controls
            bsControlOrdersDataGridView.ClearSelection()
            Dim checkedValue As Boolean = bsAllCtrlsCheckBox.Checked

            'Get all Controls that can be selected/unselected
            Dim lstWSOpenControlsDS As List(Of WorkSessionResultDS.ControlsRow)
            lstWSOpenControlsDS = (From a In myWorkSessionResultDS.Controls _
                                  Where a.SampleClass = "CTRL" _
                                AndAlso a.OTStatus = "OPEN" _
                                 Select a).ToList()

            For Each controlRow As WorkSessionResultDS.ControlsRow In lstWSOpenControlsDS
                controlRow.Selected = checkedValue
            Next
            myWorkSessionResultDS.Controls.AcceptChanges()
            lstWSOpenControlsDS = Nothing

            If (checkedValue) Then
                Dim listOfDifElem As List(Of String)
                listOfDifElem = (From a In myWorkSessionResultDS.Controls _
                                Where a.SampleClass = "CTRL" _
                              AndAlso a.OTStatus = "OPEN" _
                               Select String.Format("{0}|{1}|{2}", a.TestType, a.TestID, a.SampleType) Distinct).ToList()

                Dim myOrderTestsDelegate As New OrderTestsDelegate
                For Each difElement As String In listOfDifElem
                    Dim elements As String() = difElement.Split(CChar("|"))
                    myOrderTestsDelegate.SelectAllNeededOrderTests("CTRL", elements(0), CInt(elements(1)), elements(2), myWorkSessionResultDS)
                Next
                listOfDifElem = Nothing
            End If

            bsControlOrdersDataGridView.RefreshEdit()
            totalControlCheckedCheckBoxes = If(checkedValue, totalControlCheckedCheckBoxes, 0)

            'Verify if OpenRotor button can be enabled
            OpenRotorButtonEnabled()

            isHeaderControlCheckBoxClicked = False
            ChangesMadeAttribute = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ClickAllControlCheckBox", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ClickAllControlCheckBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manage the check/uncheck of all selectable rows in the grid of Patients
    ''' For event bsAllPatientsCheckBox_MouseClick
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/04/2010 - Code moved from the event; changes to fix errors
    ''' Modified by: SA 28/04/2010 - Changed the way of calling function SelectAllNeededOrderTests: execute it for 
    '''                              each different TestType, TestID and SampleType instead of for each row in the 
    '''                              grid (due to long time execution when the grid is loaded with lot of rows)
    '''              SA 12/07/2010 - When all Open Patient Samples are unchecked, all related Controls, Calibrators 
    '''                              and Blanks are also unchecked when it is possible
    '''              SA 18/01/2011 - Exclude Off-System Tests from the list of Patients that can be unselected
    ''' </remarks>
    Private Sub ClickAllPatientCheckBox()
        Try
            isHeaderPatientCheckBoxClicked = True

            'Empty the collection of selected rows in the grid of Patients
            bsPatientOrdersDataGridView.ClearSelection()
            Dim checkedValue As Boolean = bsAllPatientsCheckBox.Checked

            'Get all Patients that can be selected/unselected 
            Dim lstWSOpenPatientsDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSOpenPatientsDS = (From a In myWorkSessionResultDS.Patients _
                                  Where a.SampleClass = "PATIENT" _
                                AndAlso a.OTStatus = "OPEN" _
                                AndAlso a.TestType <> "OFFS" _
                                 Select a).ToList()

            For Each patientRow As WorkSessionResultDS.PatientsRow In lstWSOpenPatientsDS
                patientRow.Selected = checkedValue
            Next
            lstWSOpenPatientsDS = Nothing

            'Verify if related elements can be also selected/unselected
            Dim listOfDifElem As List(Of String)
            listOfDifElem = (From a In myWorkSessionResultDS.Patients _
                            Where a.SampleClass = "PATIENT" _
                          AndAlso a.OTStatus = "OPEN" _
                          AndAlso a.TestType = "STD" _
                           Select String.Format("{0}|{1}|{2}", a.TestType, a.TestID, a.SampleType) Distinct).ToList()

            Dim myOrderTestsDelegate As New OrderTestsDelegate
            For Each difElement As String In listOfDifElem
                Dim elements As String() = difElement.Split(CChar("|"))

                If (checkedValue) Then
                    myOrderTestsDelegate.SelectAllNeededOrderTests("PATIENT", elements(0), CInt(elements(1)), elements(2), myWorkSessionResultDS)
                Else
                    'Unselect Controls used for the same SampleType/TestID
                    Dim lstWSControlsS As List(Of WorkSessionResultDS.ControlsRow)
                    lstWSControlsS = (From a In myWorkSessionResultDS.Controls _
                                     Where a.SampleClass = "CTRL" _
                                   AndAlso a.TestType = elements(0) _
                                   AndAlso a.SampleType = elements(2) _
                                   AndAlso a.TestID = Convert.ToInt32(elements(1)) _
                                    Select a).ToList

                    If (lstWSControlsS.Count > 0) Then
                        For Each ctrl As WorkSessionResultDS.ControlsRow In lstWSControlsS
                            ctrl.Selected = False
                        Next
                        myWorkSessionResultDS.Controls.AcceptChanges()
                    End If

                    'Unselect the Calibrator and Blank if it is possible
                    UnselectBlkCalibrator("STD", elements(2), Convert.ToInt32(elements(1)))
                End If
            Next
            myWorkSessionResultDS.Patients.AcceptChanges()
            listOfDifElem = Nothing

            bsPatientOrdersDataGridView.RefreshEdit()
            totalPatientCheckedCheckBoxes = If(checkedValue, totalPatientCheckedCheckBoxes, 0)

            'Verify if OpenRotor button can be enabled
            OpenRotorButtonEnabled()

            isHeaderPatientCheckBoxClicked = False
            ChangesMadeAttribute = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ClickAllPatientCheckBox", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ClickAllPatientCheckBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manage the selection/deselection of Controls in the grid of Control Order Tests
    ''' For event bsControlOrdersDataGridView_CellMouseUp
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 29/03/2010
    ''' Modified by: SA 07/04/2010 - Changes to avoid error when clicking in an empty grid
    ''' </remarks>
    Private Sub ControlOrderTestsCellMouseUp()
        Try
            If (Not bsControlOrdersDataGridView.CurrentRow Is Nothing) Then
                'Dim columnClicked As Integer = bsControlOrdersDataGridView.CurrentCell.ColumnIndex
                If (bsControlOrdersDataGridView.CurrentCell.ReadOnly) Then
                    controlEventSource = "Left_Click"
                    If (bsControlOrdersDataGridView.SelectedRows.Count <= 1) Then
                        If (bsControlOrdersDataGridView.CurrentRow.Index = myControlRow) Then
                            If (myCountControlClicks = 1) Then
                                'Row is unmarked; controls in header area remains without changes 
                                controlEventSource = "Left_Click_Disabled"
                            Else
                                'Row is marked as selected; 
                                controlEventSource = "Left_Click_Enabled"

                                'CTRL is selected in ComboBox of Sample Classes and the selected SampleType in the ComboBox of SampleTypes
                                bsSampleClassComboBox.SelectedValue = "CTRL"
                                bsSampleTypeComboBox.SelectedValue = bsControlOrdersDataGridView("SampleType", bsControlOrdersDataGridView.CurrentCell.RowIndex).Value

                                HeaderStatusForControls()
                            End If

                            myControlRow = -1
                        Else
                            'A different row has been selected... 
                            myControlRow = bsControlOrdersDataGridView.CurrentRow.Index
                            myCountControlClicks = 1

                            'CTRL is selected in ComboBox of Sample Classes and the selected SampleType in the ComboBox of SampleTypes
                            bsSampleClassComboBox.SelectedValue = "CTRL"
                            bsSampleTypeComboBox.SelectedValue = bsControlOrdersDataGridView("SampleType", bsControlOrdersDataGridView.CurrentCell.RowIndex).Value

                            HeaderStatusForControls()
                        End If
                    Else
                        'When more than one row has been selected, select the selected SampleType in the correspondent ComboBox in header area
                        bsSampleTypeComboBox.SelectedValue = bsControlOrdersDataGridView("SampleType", bsControlOrdersDataGridView.CurrentCell.RowIndex).Value
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ControlOrderTestsClickRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ControlOrderTestsClickRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete selected Blank and Calibrators Order Tests - For event bsDelCalibrators_Click
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 09/03/2010
    '''              SA 28/04/2010 - Show warning message when not all selected Blanks and/or Calibrators were deleted
    '''                              (due to they are needed for other requested elements)
    ''' </remarks>
    Private Sub DeleteBlankCalibrators()
        Try
            Dim mySelectedBlanksDS As New SelectedTestsDS
            Dim mySelectedCalibsDS As New SelectedTestsDS
            Dim rowTestsDS As SelectedTestsDS.SelectedTestTableRow

            'If there is no selected row, return...
            Dim SelectedRowsCount As Integer = 0
            For Each SelectedRow As DataGridViewRow In bsBlkCalibDataGridView.SelectedRows
                If SelectedRow.Cells("OTStatus").Value.ToString = "OPEN" Then SelectedRowsCount += 1
            Next
            If SelectedRowsCount = 0 Then Return

            'Show message to confirm the deletion... continue only after confirmation
            If (ShowMessage(bsPrepareWSLabel.Text, GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                Cursor = Cursors.WaitCursor

                'Save current number of requested Calibrators 
                Dim lstWSCalibs As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                lstWSCalibs = (From a In myWorkSessionResultDS.BlankCalibrators _
                               Where a.SampleClass = "CALIB" _
                               Select a).ToList()
                Dim totalCalibs As Integer = lstWSCalibs.Count

                'Prepare the DataSet with the list of selected Blank and Calibrators to delete
                For Each rowBlkCalibDataGridView As DataGridViewRow In bsBlkCalibDataGridView.SelectedRows
                    If (rowBlkCalibDataGridView.Cells("SampleClass").Value.ToString = "BLANK") Then
                        rowTestsDS = mySelectedBlanksDS.SelectedTestTable.NewSelectedTestTableRow

                        rowTestsDS.TestType = rowBlkCalibDataGridView.Cells("TestType").Value.ToString
                        rowTestsDS.SampleType = rowBlkCalibDataGridView.Cells("SampleType").Value.ToString
                        rowTestsDS.TestID = DirectCast(rowBlkCalibDataGridView.Cells("TestID").Value, Integer)

                        mySelectedBlanksDS.SelectedTestTable.Rows.Add(rowTestsDS)

                    Else
                        rowTestsDS = mySelectedCalibsDS.SelectedTestTable.NewSelectedTestTableRow

                        rowTestsDS.TestType = rowBlkCalibDataGridView.Cells("TestType").Value.ToString
                        rowTestsDS.SampleType = rowBlkCalibDataGridView.Cells("SampleType").Value.ToString
                        rowTestsDS.TestID = DirectCast(rowBlkCalibDataGridView.Cells("TestID").Value, Integer)

                        mySelectedCalibsDS.SelectedTestTable.Rows.Add(rowTestsDS)
                    End If
                Next

                'If some Calibrators have been selected to delete....
                Dim showWarning As Boolean = False
                Dim myGlobalDataTO As New GlobalDataTO
                Dim myOrderTestsDelegate As New OrderTestsDelegate

                If (mySelectedCalibsDS.SelectedTestTable.Rows.Count > 0) Then
                    Dim selectedCalibs As Integer = mySelectedCalibsDS.SelectedTestTable.Rows.Count

                    myGlobalDataTO = myOrderTestsDelegate.DeleteCalibratorOrderTests("IN_LIST", mySelectedCalibsDS, myWorkSessionResultDS)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        'Refresh the global WorkSessionResultDS
                        myWorkSessionResultDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)

                        'Get the new number of requested Calibrators after deleting the selected ones
                        lstWSCalibs = (From a In myWorkSessionResultDS.BlankCalibrators _
                                       Where a.SampleClass = "CALIB" _
                                       Select a).ToList()

                        'Verify if all selected Calibrators were deleted
                        If ((totalCalibs - lstWSCalibs.Count) < selectedCalibs) Then showWarning = True
                    Else
                        'Show the message error
                        Cursor = Cursors.Default
                        ShowMessage(Name & ".DeleteBlankCalibrators", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                End If
                lstWSCalibs = Nothing

                If (Not myGlobalDataTO.HasError) Then
                    'If some Blanks have been selected to delete....
                    If (mySelectedBlanksDS.SelectedTestTable.Rows.Count > 0) Then
                        'Delete selected Blanks
                        myGlobalDataTO = myOrderTestsDelegate.DeleteBlankOrderTests("IN_LIST", mySelectedBlanksDS, myWorkSessionResultDS)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            'Refresh the global WorkSessionResultDS
                            myWorkSessionResultDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)

                            If (Not showWarning) Then
                                'Verify if all selected Blanks have been deleted
                                Dim testType As String
                                Dim sampleType As String
                                Dim testID As Integer
                                Dim lstWSBlank As List(Of WorkSessionResultDS.BlankCalibratorsRow)

                                For Each selectedBlank As SelectedTestsDS.SelectedTestTableRow In mySelectedBlanksDS.SelectedTestTable.Rows
                                    testType = selectedBlank.TestType
                                    testID = selectedBlank.TestID
                                    sampleType = selectedBlank.SampleType

                                    lstWSBlank = (From a In myWorkSessionResultDS.BlankCalibrators _
                                                 Where a.SampleClass = "BLANK" _
                                               AndAlso a.TestType = testType _
                                               AndAlso a.TestID = testID _
                                               AndAlso a.SampleType = sampleType _
                                                Select a).ToList()
                                    If (lstWSBlank.Count = 1) Then
                                        'Blank was not deleted...
                                        showWarning = True
                                        Exit For
                                    End If
                                Next
                                lstWSBlank = Nothing
                            End If
                        Else
                            'Show the message error
                            Cursor = Cursors.Default
                            ShowMessage(Name & ".DeleteBlankCalibrators", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        End If
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    ChangesMadeAttribute = True
                    If (showWarning) Then
                        'Notify user that some selected Blanks and/or Calibrators were not deleted because are needed for another requested elements
                        Cursor = Cursors.Default
                        ShowMessage(Name, GlobalEnumerates.Messages.BKL_CALIB_REQUIRED.ToString, , Me)
                    End If

                    CheckAddRemoveBlkCalRows()
                End If

                'Verify availability of screen buttons
                OpenRotorButtonEnabled()
                SaveLoadWSButtonsEnabled()
                'LISImportButtonEnabled()

                'No row has to appear as selected in the grid of Blanks and Calibrators
                bsBlkCalibDataGridView.ClearSelection()
                Cursor = Cursors.Default
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteBlankCalibrators", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteBlankCalibrators", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete selected Controls Order Tests - For event bsDelControlsButton_Click
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 09/03/2010
    '''              SA 17/03/2010 - Included ControlNumber of selected Controls to delete in the DataSet SelectedTestsDS
    '''              DL 15/04/2011 - Informed field ControlID instead of ControlNumber when load the selected records in the lists of Selected Tests
    '''              TR 11/03/2013 - Set existSelectedRows = True only when in the list of selected rows there is at least one with OTStatus = OPEN 
    '''                              AndAlso LISRequest = FALSE (improve this For/Next; write it as the one in function DeletePatients set the exit for)
    ''' </remarks>
    Private Sub DeleteControls()
        Try
            Dim mySelectedCtrlsDS As New SelectedTestsDS
            Dim rowTestsDS As SelectedTestsDS.SelectedTestTableRow

            'If there is no selected and deletable row, return...
            Dim SelectedRowsCount As Integer = 0
            For Each SelectedRow As DataGridViewRow In bsControlOrdersDataGridView.SelectedRows
                If SelectedRow.Cells("OTStatus").Value.ToString = "OPEN" AndAlso _
                    SelectedRow.Cells("LISRequest").Value.ToString = "False" Then
                    SelectedRowsCount += 1
                    Exit For
                End If

            Next
            If SelectedRowsCount = 0 Then Return

            'Show message to confirm the deletion... continue only after confirmation
            If (ShowMessage(bsPrepareWSLabel.Text, GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                Cursor = Cursors.WaitCursor

                'Prepare the DataSet with the list of selected Controls to delete
                For Each rowControlDataGridView As DataGridViewRow In bsControlOrdersDataGridView.SelectedRows
                    rowTestsDS = mySelectedCtrlsDS.SelectedTestTable.NewSelectedTestTableRow

                    rowTestsDS.TestType = rowControlDataGridView.Cells("TestType").Value.ToString
                    rowTestsDS.SampleType = rowControlDataGridView.Cells("SampleType").Value.ToString
                    rowTestsDS.TestID = DirectCast(rowControlDataGridView.Cells("TestID").Value, Integer)
                    rowTestsDS.ControlID = DirectCast(rowControlDataGridView.Cells("ControlID").Value, Integer)

                    mySelectedCtrlsDS.SelectedTestTable.Rows.Add(rowTestsDS)
                Next

                'If some Controls have been selected to delete....
                If (mySelectedCtrlsDS.SelectedTestTable.Rows.Count > 0) Then
                    Dim myGlobalDataTO As New GlobalDataTO
                    Dim myOrderTestsDelegate As New OrderTestsDelegate

                    myGlobalDataTO = myOrderTestsDelegate.DeleteControlOrderTests("IN_LIST", mySelectedCtrlsDS, myWorkSessionResultDS)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        ChangesMadeAttribute = True

                        'Refresh the global WorkSessionResultDS
                        myWorkSessionResultDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)

                        CheckAddRemoveControlsRows()
                        CheckAddRemoveBlkCalRows()
                    Else
                        Cursor = Cursors.Default
                        ShowMessage(Name & ".DeleteControls", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                End If

                'Verify availability of screen buttons
                OpenRotorButtonEnabled()
                SaveLoadWSButtonsEnabled()
                'LISImportButtonEnabled()

                Cursor = Cursors.Default
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteControls", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete selected Patient Order Tests - For event bsDelPatientsButton_Click
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 09/03/2010
    '''              TR 11/03/2013 Implement functionality for LIS Set existSelectedRows = True 
    '''                            only when in the list of selected rows there is at least one with 
    '''                            OTStatus = OPEN AndAlso LISRequest = FALSE.
    ''' </remarks>
    Private Sub DeletePatients()
        Try
            Dim mySelectedPatientDS As New SelectedTestsDS
            Dim rowTestsDS As SelectedTestsDS.SelectedTestTableRow

            'If there is not selected and deletable row, return...
            Dim existSelectedRows As Boolean = False
            For Each SelectedRow As DataGridViewRow In bsPatientOrdersDataGridView.SelectedRows
                If (SelectedRow.Cells("OTStatus").Value.ToString = "OPEN" _
                    AndAlso SelectedRow.Cells("LISRequest").Value.ToString = "False") Then
                    existSelectedRows = True
                    Exit For
                End If
            Next
            If Not existSelectedRows Then Return

            'Show message to confirm the deletion... continue only after confirmation
            If (ShowMessage(bsPrepareWSLabel.Text, GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim StartTime As DateTime = Now
                Dim myLogAcciones As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                Cursor = Cursors.WaitCursor

                'Prepare the DataSet with the list of selected Patients to delete
                For Each rowPatientDataGridView As DataGridViewRow In bsPatientOrdersDataGridView.SelectedRows

                    'TR 12/03/2013 -Exclude the Request by LIS  And Open status 
                    If Not DirectCast(rowPatientDataGridView.Cells("LISRequest").Value, Boolean) AndAlso _
                        rowPatientDataGridView.Cells("OTStatus").Value.ToString = "OPEN" Then

                        rowTestsDS = mySelectedPatientDS.SelectedTestTable.NewSelectedTestTableRow

                        rowTestsDS.SampleID = rowPatientDataGridView.Cells("SampleID").Value.ToString
                        rowTestsDS.StatFlag = DirectCast(rowPatientDataGridView.Cells("StatFlag").Value, Boolean)
                        rowTestsDS.TestType = rowPatientDataGridView.Cells("TestType").Value.ToString
                        rowTestsDS.SampleType = rowPatientDataGridView.Cells("SampleType").Value.ToString
                        rowTestsDS.TestID = DirectCast(rowPatientDataGridView.Cells("TestID").Value, Integer)
                        rowTestsDS.SampleID = DirectCast(rowPatientDataGridView.Cells("SampleID").Value, String)
                        rowTestsDS.LISRequest = DirectCast(rowPatientDataGridView.Cells("LISRequest").Value, Boolean)

                        'If the Test to delete belongs to a Test Profile, it has to be informed to remove the Test Profile from 
                        'the rest of the Tests for the same SampleID/StatFlag
                        If (Not IsDBNull(rowPatientDataGridView.Cells("TestProfileID").Value)) Then
                            rowTestsDS.TestProfileID = DirectCast(rowPatientDataGridView.Cells("TestProfileID").Value, Integer)
                        End If

                        'If the Test to delete is included in one or more Calculated Tests, the field containing the list of 
                        'Calculated Test IDs has to be informed to delete the Calculated Test and remove the link with the rest of
                        'Test in their formulas
                        If (Not IsDBNull(rowPatientDataGridView.Cells("CalcTestID").Value)) Then
                            rowTestsDS.CalcTestIDs = rowPatientDataGridView.Cells("CalcTestID").Value.ToString
                        End If

                        mySelectedPatientDS.SelectedTestTable.Rows.Add(rowTestsDS)
                    End If
                Next

                'If some Patient Order Tests have been selected to delete....
                If (mySelectedPatientDS.SelectedTestTable.Rows.Count > 0) Then
                    Dim myGlobalDataTO As New GlobalDataTO
                    Dim myOrderTestsDelegate As New OrderTestsDelegate

                    myGlobalDataTO = myOrderTestsDelegate.DeletePatientOrderTests("IN_LIST", mySelectedPatientDS, myWorkSessionResultDS)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        ChangesMadeAttribute = True

                        'Refresh the global WorkSessionResultDS
                        myWorkSessionResultDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)

                        CheckAddRemovePatientsRows()
                        CheckAddRemoveBlkCalRows()

                        'Initialize values in area of criteria selection
                        SetFieldStatusForPatient(True)
                    Else
                        'Show the message error
                        Cursor = Cursors.Default
                        ShowMessage(Name & ".DeletePatients", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                End If
                CalculateAllAutonumeric()

                'Verify availability of screen buttons
                OpenRotorButtonEnabled()
                SaveLoadWSButtonsEnabled()
                OffSystemResultsButtonEnabled()
                'LISImportButtonEnabled()

                Cursor = Cursors.Default

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                myLogAcciones.CreateLogActivity("IWSampleRequest Delete Patient Request(Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                "IWSampleRequest.DeletePatients", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeletePatients", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeletePatients", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Disable all fields in bsOrderDetailsGroupBox when the status of the current WS is ABORTED
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 12/03/2012
    ''' </remarks>
    Private Sub DisableFieldsForAbortedWS()
        Try
            bsStatCheckbox.Enabled = False
            bsPatientIDTextBox.Enabled = False
            bsPatientIDTextBox.BackColor = SystemColors.MenuBar
            bsPatientSearchButton.Enabled = False
            bsNumOrdersNumericUpDown.Enabled = False
            bsNumOrdersNumericUpDown.BackColor = SystemColors.MenuBar
            bsSampleTypeComboBox.Enabled = False
            bsSampleTypeComboBox.BackColor = SystemColors.MenuBar
            bsSearchTestsButton.Enabled = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DisableFieldsForAbortedWS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DisableFieldsForAbortedWS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill a typed DataSet MaxOrderTestsValuesDS with all values required to calculate if the maximum number of Patient Order 
    ''' Tests has been exceeded:
    '''   ** Max number of allowed Patient Order Tests (loaded from an Application General Setting)
    '''   ** Number of Patient Order Tests that have been requested (number of rows currently in grid of Patient Order Tests) 
    '''   ** Total number of Orders that has been requested (from the correspondent NumericUpDown control in header area)
    ''' </summary>
    ''' <param name="pMaxOrderTestsDS">Typed DataSet MaxOrderTestsValuesDS where the values needed to calculate if the maximum
    '''                                number of Patient Order Tests has been exceeded will be loaded</param>
    ''' <remarks>
    ''' Created by:  SA 06/04/2010
    ''' </remarks>
    Private Sub FillMaxOrderTestValues(ByVal pMaxOrderTestsDS As MaxOrderTestsValuesDS)
        Try
            Dim newMaxOrderTestsRow As MaxOrderTestsValuesDS.MaxOrderTestsValuesRow

            newMaxOrderTestsRow = pMaxOrderTestsDS.MaxOrderTestsValues.NewMaxOrderTestsValuesRow
            newMaxOrderTestsRow.MaxRowsAllowed = maxPatientOrderTests
            newMaxOrderTestsRow.CurrentNumOrdersValue = CType(bsNumOrdersNumericUpDown.Value, Integer)
            newMaxOrderTestsRow.CurrentRowsNumValue = bsPatientOrdersDataGridView.Rows.Count
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
    ''' Created by:  SA 12/05/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode
    ''' </remarks>
    Private Sub FillSelectedTestsForDifPriority(ByVal pPatientID As String, ByVal pStatFlag As Boolean, _
                                                ByVal pSampleType As String, ByRef pLockedTestsDS As SelectedTestsDS)
        Try
            'Get all Tests requested for the same PatientID/SampleID, SampleType and the opposite StatFlag
            Dim lstWSPatientDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSPatientDS = (From a In myWorkSessionResultDS.Patients _
                              Where a.SampleClass = "PATIENT" _
                            AndAlso a.SampleType = pSampleType _
                            AndAlso a.SampleID.ToUpperInvariant = pPatientID.ToUpperInvariant _
                            AndAlso a.StatFlag <> pStatFlag _
                             Select a).ToList()

            'Load the DataSet with the list of locked Tests
            Dim newTestRow As SelectedTestsDS.SelectedTestTableRow
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
            Next
            lstWSPatientDS = Nothing
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
    ''' Created by:  DL
    ''' Modified by: SA 29/03/2010 - Changed name from ListPatient to FillSelectedTestsForPatient; added the Thrown ex when error
    '''              SA 12/05/2010 - Inform fields for Calculated Tests in the DataSet
    '''              SA 02/11/2010 - Changed from Sub to Function returning a boolean value indicating if the process of adding
    '''                              Tests has to continue (when True) or has to be stopped (when False)
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode
    '''              TR 11/03/2013 - Inform LISRequest in SelectedTestsDS with the same value it has for the corresponding Order Test.
    ''' </remarks>
    Private Function FillSelectedTestsForPatient(ByVal pPatientID As String, ByVal pStatFlag As Boolean, ByVal pSampleType As String, _
                                                 ByRef pCurrentTestsDS As SelectedTestsDS) As Boolean
        Dim openTestSelection As Boolean = True
        Try
            'Verify if there are Order Tests that have been sent to the Analyzer Rotor for the selected PatientID/SampleID and StatFlag
            Dim lstWSPatientDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSPatientDS = (From a In myWorkSessionResultDS.Patients _
                              Where a.SampleClass = "PATIENT" _
                                AndAlso a.SampleID.ToUpperInvariant = pPatientID.ToUpperInvariant _
                                AndAlso a.StatFlag = pStatFlag _
                                AndAlso a.OTStatus <> "OPEN" _
                             Select a).ToList()

            If (lstWSPatientDS.Count > 0) Then
                'Show message to confirm the adding of more Order Tests for this Patient... continue only after confirmation
                openTestSelection = (ShowMessage(bsPrepareWSLabel.Text, GlobalEnumerates.Messages.ADD_MORE_TESTS_TO_PATIENT.ToString, , Me) = Windows.Forms.DialogResult.Yes)
            End If

            If (openTestSelection) Then
                'Get all Tests requested for the same PatientID/SampleID, StatFlag and SampleType
                lstWSPatientDS = (From a In myWorkSessionResultDS.Patients _
                                 Where a.SampleClass = "PATIENT" _
                                   AndAlso a.SampleType = pSampleType _
                                   AndAlso a.SampleID.ToUpperInvariant = pPatientID.ToUpperInvariant _
                                   AndAlso a.StatFlag = pStatFlag _
                                Select a).ToList()

                'Load the Selected Tests DataSet with the list of Patient Order Tests (for the selected SampleID/PatientID, StatFlag
                'and SampleType) currently loaded in the grid of Patients
                Dim newTestRow As SelectedTestsDS.SelectedTestTableRow
                For Each patientOrderTest As WorkSessionResultDS.PatientsRow In lstWSPatientDS
                    newTestRow = pCurrentTestsDS.SelectedTestTable.NewSelectedTestTableRow

                    newTestRow.TestType = patientOrderTest.TestType
                    newTestRow.SampleType = patientOrderTest.SampleType
                    newTestRow.TestID = patientOrderTest.TestID
                    newTestRow.OTStatus = patientOrderTest.OTStatus
                    newTestRow.LISRequest = patientOrderTest.LISRequest

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
                Next
            End If
            lstWSPatientDS = Nothing
        Catch ex As Exception
            openTestSelection = False

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillSelectedTestsForPatient", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillSelectedTestsForPatient", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return openTestSelection
    End Function

    ''' <summary>
    ''' Get the default limits for the different Num. Replicates controls in all the grids  
    ''' ** Patients grid: TEST_NUM_REPLICATES
    ''' ** Controls grid: CONTROL_REPLICATES
    ''' ** Blanks and Calibrators grid: BLK_CALIB_REPLICATES 
    ''' </summary>
    ''' <param name="pLimitID">Limit Identifier</param>
    ''' <returns>Array of Integers containing each one of the values for the informed limit
    '''          (Min, Max, Decimals Allowed, Step and Default Value)</returns>
    ''' <remarks></remarks>
    Private Function GetNumReplicatesLimits(ByVal pLimitID As FieldLimitsEnum) As Integer()
        Dim resultData(4) As Integer
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim fieldLimitsConfig As New FieldLimitsDelegate

            myGlobalDataTO = fieldLimitsConfig.GetList(Nothing, pLimitID)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim fieldDS As FieldLimitsDS = DirectCast(myGlobalDataTO.SetDatos, FieldLimitsDS)

                If (fieldDS.tfmwFieldLimits.Rows.Count > 0) Then
                    'Return the different limits values in each array position
                    resultData(0) = Convert.ToInt32(fieldDS.tfmwFieldLimits(0).MinValue)
                    resultData(1) = Convert.ToInt32(fieldDS.tfmwFieldLimits(0).MaxValue)
                    resultData(2) = Convert.ToInt32(fieldDS.tfmwFieldLimits(0).DefaultValue)
                    resultData(3) = Convert.ToInt32(fieldDS.tfmwFieldLimits(0).DecimalsAllowed)

                    If (Not fieldDS.tfmwFieldLimits(0).IsStepValueNull) Then
                        resultData(4) = Convert.ToInt32(fieldDS.tfmwFieldLimits(0).StepValue)
                    End If
                End If
            Else
                'Show the error Message
                ShowMessage(Name & ".GetNumReplicatesLimits", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetNumReplicatesLimits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetNumReplicatesLimits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get values to load Combo Box of Tube Types in the different grids
    ''' </summary>
    ''' <returns>GlobalDataTO containing the list of Sample Tube Types sorted by position</returns>
    ''' <remarks></remarks>
    Private Function GetSampleTubeTypes() As GlobalDataTO
        Dim myGlobalDataTO As GlobalDataTO

        Try
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.TUBE_TYPES_SAMPLES)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreLoadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                If (myPreLoadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                    Dim lstSorted As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                    lstSorted = (From a In myPreLoadedMasterDataDS.tfmwPreloadedMasterData _
                                 Order By (a.Position) _
                                 Select a).ToList()

                    'Return the sorted list
                    myGlobalDataTO.SetDatos = lstSorted
                    lstSorted = Nothing 'TR 05/08/2011 -set value to nothing to release memory
                End If
            End If

        Catch ex As Exception
            myGlobalDataTO = New GlobalDataTO()
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSampleTubeTypes", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by:  PG 07/10/2010
    ''' Modified by: SA 18/01/2010 - Get multilanguage text for tooltip of new button to opening the auxiliary 
    '''                              screen to add results for requested Off-System Tests
    '''              XB 26/08/2014 - Use Multilanguage resource LBL_Blanks instead of LBL_WSPrep_AllBlanksCalibs - BT #1868
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsNumOrdersLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Long", pLanguageID) + ":"
            'EF 29/08/2013 - Bugtracking 1272 - Change label text by 'Patient/sample'  Wrong change of DL is canceled (DL 12/06/2013. Work Sessions Module Changes to shown LIS Specimen ID
            bsPatientIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientSample", pLanguageID) + ":"
            'bsPatientIDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Patient", pLanguageID) + ":"
            'DL 12/06/2013
            'EF 29/08/2013

            bsSampleClassLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_SampleClass", pLanguageID) + ":"
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID) + ":"
            bsPrepareWSLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_WSPreparation", pLanguageID)

            ' XB 26/08/2014 - BT #1868
            bsAllBlanksCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Blanks", pLanguageID)
            bsAllCalibsCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrators", pLanguageID)

            bsAllCtrlsCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Controls", pLanguageID)
            bsAllPatientsCheckBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_AllPatients", pLanguageID)
            bsStatCheckbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Stat", pLanguageID)
            bsSearchTestsButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", pLanguageID) 'JB 01/10/2012 - Resource String unification

            OtherSamplesTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_BlanksCalibsCtrls", pLanguageID)
            PatientsTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_PatientSamples", pLanguageID)

            'For button Tooltips...
            bsScreenToolTips.SetToolTip(bsPatientSearchButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSPrep_OpenSelPatient", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSearchTestsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestsSelection", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDelPatientsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDelCalibratorsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDelControlsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            bsScreenToolTips.SetToolTip(bsLoadWSButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSPrep_OpenLoadWS", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSaveWSButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSPrep_OpenSaveWS", pLanguageID))
            bsScreenToolTips.SetToolTip(bsOffSystemResultsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_OffSystem_Tests_Results", pLanguageID))
            bsScreenToolTips.SetToolTip(bsScanningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RotorPos_ReadBarcode", pLanguageID))
            bsScreenToolTips.SetToolTip(bsBarcodeWarningButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_BARCODE_WARN", pLanguageID))
            'bsScreenToolTips.SetToolTip(bsLIMSImportButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSPrep_LIMSImport", pLanguageID))
            bsScreenToolTips.SetToolTip(bsLIMSErrorsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSPrep_OpenLIMSErrors", pLanguageID))
            bsScreenToolTips.SetToolTip(bsOpenRotorButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSPrep_OpenRotorPos", pLanguageID))
            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", pLanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set status of controls in the header area when the selected SampleClass is BLANK or CALIB
    ''' </summary>
    ''' <param name="pSampleClass">Sample Class code: BLANK or CALIB</param>
    ''' <remarks>
    ''' Modified by: SA 12/03/2012 - Do not enable the SampleType ComboBox when status of the current WS is ABORTED
    ''' </remarks>
    Private Sub HeaderStatusForBlankCalibrators(ByVal pSampleClass As String)
        Try
            bsStatCheckbox.Checked = False
            bsStatCheckbox.Enabled = False

            bsPatientIDTextBox.Text = ""
            bsPatientIDTextBox.ReadOnly = True
            bsPatientIDTextBox.BackColor = SystemColors.MenuBar
            bsPatientIDTextBox.Enabled = False

            bsPatientSearchButton.Enabled = False

            bsNumOrdersNumericUpDown.Value = 1
            bsNumOrdersNumericUpDown.Enabled = False
            bsNumOrdersNumericUpDown.BackColor = SystemColors.MenuBar

            If (pSampleClass = "BLANK") Then
                bsSampleTypeComboBox.SelectedIndex = -1
                bsSampleTypeComboBox.Enabled = False
                bsSampleTypeComboBox.BackColor = SystemColors.MenuBar

            ElseIf (WSStatusAttribute <> "ABORTED") Then
                bsSampleTypeComboBox.Enabled = True
                bsSampleTypeComboBox.BackColor = Color.White
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".HeaderStatusForBlankCalibrators", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".HeaderStatusForBlankCalibrators", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set status of controls in the header area when the selected SampleClass is CTRL
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 12/03/2012 - Do not enable the SampleType ComboBox when status of the current WS is ABORTED
    ''' </remarks>
    Private Sub HeaderStatusForControls()
        Try
            bsStatCheckbox.Checked = False
            bsStatCheckbox.Enabled = False

            If (WSStatusAttribute <> "ABORTED") Then
                bsSampleTypeComboBox.Enabled = True
                bsSampleTypeComboBox.BackColor = Color.White
            End If

            bsPatientIDTextBox.Text = ""
            bsPatientIDTextBox.ReadOnly = True
            bsPatientIDTextBox.BackColor = SystemColors.MenuBar
            bsPatientIDTextBox.Enabled = False

            bsPatientSearchButton.Enabled = False

            bsNumOrdersNumericUpDown.Value = 1
            bsNumOrdersNumericUpDown.Enabled = False
            bsNumOrdersNumericUpDown.BackColor = SystemColors.MenuBar
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".HeaderStatusForControls", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".HeaderStatusForControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set status of controls in the header area when the selected SampleClass is PATIENT
    ''' </summary>
    ''' <param name="pPatientSelected">Indicate if there is a Patient selected or not</param>
    ''' <remarks>
    ''' Modified by: SA 09/07/2010 - Do not change the current checked status of the Stat CheckBox only enable/disable it
    '''              SA 12/03/2012 - Do not enable the PatientID TextBox when status of the current WS is ABORTED
    ''' </remarks>
    Private Sub HeaderStatusForPatients(ByVal pPatientSelected As Boolean)
        Try
            If (Not pPatientSelected AndAlso WSStatusAttribute <> "ABORTED") Then
                bsStatCheckbox.Enabled = True

                bsPatientIDTextBox.Text = ""
                bsPatientIDTextBox.ReadOnly = False
                bsPatientIDTextBox.BackColor = Color.White

                bsPatientSearchButton.Enabled = True

                bsNumOrdersNumericUpDown.Value = 1
                bsNumOrdersNumericUpDown.Enabled = True
                bsNumOrdersNumericUpDown.BackColor = Color.White
            Else
                bsStatCheckbox.Enabled = False

                bsPatientIDTextBox.ReadOnly = True
                bsPatientIDTextBox.BackColor = Color.Gainsboro

                bsPatientSearchButton.Enabled = False

                bsNumOrdersNumericUpDown.Value = 1
                bsNumOrdersNumericUpDown.Enabled = False
                bsNumOrdersNumericUpDown.BackColor = SystemColors.MenuBar
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".HeaderStatusForPatients", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".HeaderStatusForPatients", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialization of Blank and Calibrators Order Tests grid
    ''' </summary>
    ''' <param name="pTubeTypesList">List of Sample TubeTypes sorted by default</param>
    ''' <remarks>
    ''' Modified by: DL 09/03/2010
    '''              SA 17/03/2010 - Set SortMode of visible columns as Programmatically
    '''              PG 13/10/2010 - Get the current language and add the languageID parameter
    '''              SA 24/11/2010 - Added new hidden column for Factor value originally loaded. Changed column FactorValue
    '''                              for a TextBox that can be editable
    '''              RH 07/06/2011 - Added column for BlankMode 
    '''              SA 28/08/2012 - Added a hidden column for field Previous WorkSession ID (for those Blanks and Calibrators having
    '''                              previous results, this field and PreviousOrderTestID are the identifiers of the result in Historic Module) 
    '''              SA 23/03/2013 - Set to false grid property AutoGenerateColumns to avoid shown new columns added to the source DS for LIS management
    ''' </remarks>
    Private Sub InitializeBlkCalibGrid(ByVal pTubeTypesList As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow), ByVal pLanguageID As String)
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsBlkCalibDataGridView.AutoGenerateColumns = False
            bsBlkCalibDataGridView.AllowUserToAddRows = False
            bsBlkCalibDataGridView.AllowUserToDeleteRows = False

            'Blanks and Calibrator Order Tests
            bsBlkCalibDataGridView.Rows.Clear()
            bsBlkCalibDataGridView.Columns.Clear()
            bsBlkCalibDataGridView.EditMode = DataGridViewEditMode.EditOnEnter

            'Selected column
            Dim checkBoxColumnCalib As New DataGridViewCheckBoxColumn
            columnName = "Selected"
            checkBoxColumnCalib.Name = columnName
            checkBoxColumnCalib.HeaderText = ""

            bsBlkCalibDataGridView.Columns.Add(checkBoxColumnCalib)
            bsBlkCalibDataGridView.Columns(columnName).Width = 20
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).Resizable = DataGridViewTriState.False
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'SampleClass column
            columnName = "SampleClass"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'SampleClassIcon column
            Dim iconSampleClassColumn As New DataGridViewImageColumn
            columnName = "SampleClassIcon"
            iconSampleClassColumn.Name = columnName
            iconSampleClassColumn.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleClass_Short", pLanguageID)
            iconSampleClassColumn.DataPropertyName = "BlankCalibrators.SampleClassIcon"

            bsBlkCalibDataGridView.Columns.Add(iconSampleClassColumn)
            bsBlkCalibDataGridView.Columns(columnName).Width = 55
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'OrderID column
            columnName = "OrderID"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'OrderTestID column
            columnName = "OrderTestID"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'CalibratorID column
            columnName = "CalibratorID"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'CalibratorName column
            'JC 13/11/2012
            columnName = "CalibratorName"
            bsBlkCalibDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_Calibrator", pLanguageID))
            bsBlkCalibDataGridView.Columns(columnName).Width = 125
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'LotNumber column
            columnName = "LotNumber"
            bsBlkCalibDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", pLanguageID))
            bsBlkCalibDataGridView.Columns(columnName).Width = 80
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'NumberOfCalibrators column
            columnName = "NumberOfCalibrators"
            bsBlkCalibDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_#Calibrators", pLanguageID))
            bsBlkCalibDataGridView.Columns(columnName).Width = 100
            bsBlkCalibDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'TestType column
            columnName = "TestType"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'TestID column
            columnName = "TestID"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'TestName column
            columnName = "TestName"
            bsBlkCalibDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", pLanguageID))
            bsBlkCalibDataGridView.Columns(columnName).Width = 130
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'RequestedSampleTypes column
            columnName = "RequestedSampleTypes"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'SampleType column
            columnName = "SampleType"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'ToShowSampleType column
            columnName = "ToShowSampleType"
            bsBlkCalibDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", pLanguageID))
            bsBlkCalibDataGridView.Columns(columnName).Width = 60
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'NumReplicates column
            'JC 13/11/2012
            Dim numUpDownReplicates As New DataGridViewNumericUpDownColumn
            Dim numReplicatesLimits(4) As Integer
            numReplicatesLimits = GetNumReplicatesLimits(FieldLimitsEnum.BLK_CALIB_REPLICATES)

            columnName = "NumReplicates"
            numUpDownReplicates.DataPropertyName = columnName
            numUpDownReplicates.Name = columnName
            numUpDownReplicates.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_#Replicates_Short", pLanguageID)
            numUpDownReplicates.Minimum = numReplicatesLimits(0)
            numUpDownReplicates.Maximum = numReplicatesLimits(1)
            numUpDownReplicates.DecimalPlaces = numReplicatesLimits(3)
            numUpDownReplicates.Increment = numReplicatesLimits(4)

            bsBlkCalibDataGridView.Columns.Add(numUpDownReplicates)
            bsBlkCalibDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsBlkCalibDataGridView.Columns(columnName).Width = 60
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'Tube Types Column
            Dim comboBoxCol As New DataGridViewComboBoxColumn

            columnName = "TubeType"
            comboBoxCol.DataPropertyName = columnName
            comboBoxCol.Name = columnName
            comboBoxCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tube", pLanguageID)
            comboBoxCol.DataSource = pTubeTypesList
            comboBoxCol.DisplayMember = "FixedItemDesc"
            comboBoxCol.ValueMember = "ItemID"

            bsBlkCalibDataGridView.Columns.Add(comboBoxCol)
            bsBlkCalibDataGridView.Columns(columnName).Width = 165
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'NewCheck column
            'JC 13/11/2012
            Dim checkBoxColumnNew As New DataGridViewCheckBoxColumn
            columnName = "NewCheck"
            checkBoxColumnNew.Name = columnName
            checkBoxColumnNew.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_New", pLanguageID)
            bsBlkCalibDataGridView.Columns.Add(checkBoxColumnNew)
            bsBlkCalibDataGridView.Columns(columnName).Width = 55
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'PreviousOrderTestID column
            columnName = "PreviousOrderTestID"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'ABSValue column

            columnName = "ABSValue"
            bsBlkCalibDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", pLanguageID))
            bsBlkCalibDataGridView.Columns(columnName).Width = 80
            bsBlkCalibDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).DefaultCellStyle.WrapMode = DataGridViewTriState.True
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'ABSDateTime column
            columnName = "ABSDateTime"
            bsBlkCalibDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", pLanguageID))
            bsBlkCalibDataGridView.Columns(columnName).Width = 120
            bsBlkCalibDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'FactorValue column
            Dim textBoxColumn As New DataGridViewTextBoxColumn
            columnName = "FactorValue"
            textBoxColumn.Name = columnName
            textBoxColumn.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibFactor", pLanguageID)
            textBoxColumn.MaxInputLength = 10

            bsBlkCalibDataGridView.Columns.Add(textBoxColumn)
            bsBlkCalibDataGridView.Columns(columnName).Width = 90
            bsBlkCalibDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True
            bsBlkCalibDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'OriginalFactorValue column
            columnName = "OriginalFactorValue"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'Order Test Status column
            columnName = "OTStatus"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'CreationOrder column (needed to sort the records)
            columnName = "CreationOrder"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'ToSendFlag column
            columnName = "TOSendFlag"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            'BlankMode column
            columnName = "BlankMode"
            bsBlkCalibDataGridView.Columns.Add(columnName, "")
            bsBlkCalibDataGridView.Columns(columnName).Width = 0
            bsBlkCalibDataGridView.Columns(columnName).Visible = False
            bsBlkCalibDataGridView.Columns(columnName).DataPropertyName = columnName
            bsBlkCalibDataGridView.Columns(columnName).ReadOnly = True

            bsBlkCalibDataGridView.ScrollBars = ScrollBars.Both
            bsBlkCalibDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeBlkCalibGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeBlkCalibGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialization of Control Order Tests grid
    ''' </summary>
    ''' <param name="pTubeTypesList">List of Sample TubeTypes sorted by default</param>
    ''' <remarks>
    ''' Modified by: DL 09/03/2010
    '''              SA 17/03/2010 - Set SortMode of visible columns as Programmatically
    '''              PG 13/10/2010 - Get the current Language and add the LanguageID parameter
    '''              DL 15/04/2011 - Removed column for field ControlNumber
    '''              SA 18/06/2012 - Added hide column TestType and a visible column for TestType Icon (due to Controls 
    '''                              can be defined also for ISE Tests)
    '''              JC 13/11/2012 - Modified column width
    '''              SA 23/03/2013 - Set to false grid property AutoGenerateColumns to avoid shown new columns added to the source DS for LIS management
    ''' </remarks>
    Private Sub InitializeControlGrid(ByVal pTubeTypesList As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow), _
                                      ByVal pLanguageID As String)
        Try
            Dim columnText As String
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsControlOrdersDataGridView.AutoGenerateColumns = False
            bsControlOrdersDataGridView.AllowUserToAddRows = False
            bsControlOrdersDataGridView.AllowUserToDeleteRows = False
            bsControlOrdersDataGridView.Rows.Clear()
            bsControlOrdersDataGridView.Columns.Clear()
            bsControlOrdersDataGridView.EditMode = DataGridViewEditMode.EditOnEnter

            'Selected column
            Dim checkBoxColumnCalib As New DataGridViewCheckBoxColumn
            columnName = "Selected"
            checkBoxColumnCalib.Name = columnName
            checkBoxColumnCalib.HeaderText = ""

            bsControlOrdersDataGridView.Columns.Add(checkBoxColumnCalib)
            bsControlOrdersDataGridView.Columns(columnName).Width = 20
            bsControlOrdersDataGridView.Columns(columnName).Visible = True
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).Resizable = DataGridViewTriState.False
            bsControlOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'SampleClass column
            columnName = "SampleClass"
            bsControlOrdersDataGridView.Columns.Add(columnName, "")
            bsControlOrdersDataGridView.Columns(columnName).Width = 0
            bsControlOrdersDataGridView.Columns(columnName).Visible = False
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True

            'OrderID column
            columnName = "OrderID"
            bsControlOrdersDataGridView.Columns.Add("OrderID", "")
            bsControlOrdersDataGridView.Columns(columnName).Width = 0
            bsControlOrdersDataGridView.Columns(columnName).Visible = False
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True

            'OrderTestID column
            columnName = "OrderTestID"
            bsControlOrdersDataGridView.Columns.Add("OrderTestID", "")
            bsControlOrdersDataGridView.Columns(columnName).Width = 0
            bsControlOrdersDataGridView.Columns(columnName).Visible = False
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True

            'ControlID column
            columnName = "ControlID"
            bsControlOrdersDataGridView.Columns.Add("ControlID", "")
            bsControlOrdersDataGridView.Columns(columnName).Width = 0
            bsControlOrdersDataGridView.Columns(columnName).Visible = False
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True

            'ControlName column
            columnName = "ControlName"
            columnText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_Control", pLanguageID)
            bsControlOrdersDataGridView.Columns.Add("ControlName", columnText)
            bsControlOrdersDataGridView.Columns(columnName).Width = 195
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsControlOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'LotNumber column
            columnName = "LotNumber"
            columnText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", pLanguageID)
            bsControlOrdersDataGridView.Columns.Add("LotNumber", columnText)
            bsControlOrdersDataGridView.Columns(columnName).Width = 90
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsControlOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'TestType column
            columnName = "TestType"
            bsControlOrdersDataGridView.Columns.Add("TestType", "")
            bsControlOrdersDataGridView.Columns(columnName).Width = 0
            bsControlOrdersDataGridView.Columns(columnName).Visible = False
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True

            'TR 11/03/2013
            'LIS Required ICON
            Dim LISRequiredIconColumn As New DataGridViewImageColumn
            columnName = "LISRequestIcon"
            LISRequiredIconColumn.Name = columnName
            LISRequiredIconColumn.HeaderText = String.Empty

            bsControlOrdersDataGridView.Columns.Add(LISRequiredIconColumn)
            bsControlOrdersDataGridView.Columns(columnName).Width = 40
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsControlOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'TestTypeIcon column - TestType Icon
            'JC 13/11/2012
            Dim iconTestTypeColumn As New DataGridViewImageColumn
            columnName = "TestTypeIcon"
            iconTestTypeColumn.Name = columnName
            iconTestTypeColumn.HeaderText = String.Empty

            bsControlOrdersDataGridView.Columns.Add(iconTestTypeColumn)
            bsControlOrdersDataGridView.Columns(columnName).Width = 40
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsControlOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'TestID column
            columnName = "TestID"
            bsControlOrdersDataGridView.Columns.Add("TestID", "")
            bsControlOrdersDataGridView.Columns(columnName).Width = 0
            bsControlOrdersDataGridView.Columns(columnName).Visible = False
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName

            'TestName column
            columnName = "TestName"
            columnText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", pLanguageID)
            bsControlOrdersDataGridView.Columns.Add("TestName", columnText)
            bsControlOrdersDataGridView.Columns(columnName).Width = 130
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsControlOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'SampleType column
            'JC 13/11/2012
            columnName = "SampleType"
            columnText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", pLanguageID)
            bsControlOrdersDataGridView.Columns.Add("SampleType", columnText)
            bsControlOrdersDataGridView.Columns(columnName).Width = 70
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True

            'NumReplicates column
            'JC 13/11/2012
            Dim numUpDownReplicates As New DataGridViewNumericUpDownColumn
            Dim numReplicatesLimits(4) As Integer
            numReplicatesLimits = GetNumReplicatesLimits(FieldLimitsEnum.CONTROL_REPLICATES)

            columnName = "NumReplicates"
            numUpDownReplicates.DataPropertyName = columnName
            numUpDownReplicates.Name = columnName
            numUpDownReplicates.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_#Replicates_Short", pLanguageID)
            numUpDownReplicates.Minimum = numReplicatesLimits(0)
            numUpDownReplicates.Maximum = numReplicatesLimits(1)
            numUpDownReplicates.DecimalPlaces = numReplicatesLimits(3)
            numUpDownReplicates.Increment = numReplicatesLimits(4)

            bsControlOrdersDataGridView.Columns.Add(numUpDownReplicates)
            bsControlOrdersDataGridView.Columns(columnName).Width = 80
            bsControlOrdersDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsControlOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'Tube Types Column
            Dim comboBoxCol As New DataGridViewComboBoxColumn

            columnName = "TubeType"
            comboBoxCol.DataPropertyName = columnName
            comboBoxCol.Name = "TubeType"
            comboBoxCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tube", pLanguageID)
            comboBoxCol.DataSource = pTubeTypesList
            comboBoxCol.DisplayMember = "FixedItemDesc"
            comboBoxCol.ValueMember = "ItemID"

            bsControlOrdersDataGridView.Columns.Add(comboBoxCol)
            bsControlOrdersDataGridView.Columns(columnName).Width = 150
            bsControlOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'Order Test Status column
            columnName = "OTStatus"
            bsControlOrdersDataGridView.Columns.Add("OTStatus", "")
            bsControlOrdersDataGridView.Columns(columnName).Width = 0
            bsControlOrdersDataGridView.Columns(columnName).Visible = False
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True

            'ExpirationDate column
            'JC 13/11/2012
            columnName = "ExpirationDate"
            columnText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Short", pLanguageID)
            bsControlOrdersDataGridView.Columns.Add("ExpirationDate", columnText)
            bsControlOrdersDataGridView.Columns(columnName).Width = 200
            bsControlOrdersDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsControlOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'CreationOrder column (needed to sort the records)
            columnName = "CreationOrder"
            bsControlOrdersDataGridView.Columns.Add(columnName, "")
            bsControlOrdersDataGridView.Columns(columnName).Width = 0
            bsControlOrdersDataGridView.Columns(columnName).Visible = False 'True
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True

            bsControlOrdersDataGridView.ScrollBars = ScrollBars.Both

            'TR 11/03/2013
            'LIS LISRequest
            columnName = "LISRequest"
            bsControlOrdersDataGridView.Columns.Add("LISRequest", "")
            bsControlOrdersDataGridView.Columns(columnName).Width = 0
            bsControlOrdersDataGridView.Columns(columnName).Visible = False
            bsControlOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsControlOrdersDataGridView.Columns(columnName).ReadOnly = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeControlGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeControlGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialization of Patient Order Tests grid
    ''' </summary>
    ''' <param name="pTubeTypesList">List of Sample TubeTypes sorted by default</param>
    ''' <remarks>
    ''' Modified by: DL 04/03/2010
    '''              SA 17/03/2010 - Set SortMode of visible columns as Programmatically
    '''              SA 11/05/2010 - Add columns needed to show information of Calculated Tests
    '''              PG 13/10/2010 - Get the current language and add the languageID parameter
    '''              SA 21/10/2010 - Added new column for the Formula of Calculated Tests
    '''              DL 13/12/2010 - Removed HeaderText of column showing the Test Type icon
    '''              JC 13/11/2012 - Modified column width
    '''              SA 23/03/2013 - Set to false grid property AutoGenerateColumns to avoid shown new columns added to the source DS for LIS management
    ''' </remarks>
    Private Sub InitializePatientGrid(ByVal pTubeTypesList As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow), _
                                      ByVal pLanguageID As String)
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsPatientOrdersDataGridView.AutoGenerateColumns = False
            bsPatientOrdersDataGridView.AllowUserToAddRows = False
            bsPatientOrdersDataGridView.AllowUserToDeleteRows = False
            bsPatientOrdersDataGridView.EditMode = DataGridViewEditMode.EditOnEnter

            bsPatientOrdersDataGridView.Rows.Clear()
            bsPatientOrdersDataGridView.Columns.Clear()

            'Selected column
            Dim checkBoxColumnSelec As New DataGridViewCheckBoxColumn
            columnName = "Selected"
            checkBoxColumnSelec.Name = columnName
            checkBoxColumnSelec.HeaderText = ""

            bsPatientOrdersDataGridView.Columns.Add(checkBoxColumnSelec)
            bsPatientOrdersDataGridView.Columns(columnName).Width = 20
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).Resizable = DataGridViewTriState.False
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'SampleClass column
            columnName = "SampleClass"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            'OrderID column
            columnName = "OrderID"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            'OrderTestID column
            columnName = "OrderTestID"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            Dim textBoxColumnSampleID As New DataGridViewTextBoxColumn
            columnName = "SampleID"
            textBoxColumnSampleID.Name = columnName
            textBoxColumnSampleID.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientSample", pLanguageID)
            textBoxColumnSampleID.MaxInputLength = 30

            bsPatientOrdersDataGridView.Columns.Add(textBoxColumnSampleID)
            bsPatientOrdersDataGridView.Columns(columnName).Width = 170
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'DL 12/06/2013
            Dim textBoxColumnSpecimenID As New DataGridViewTextBoxColumn
            columnName = "SpecimenID"
            textBoxColumnSpecimenID.Name = columnName
            'EF 29/08/2013 - Bugtracking 1272 - Change label text by 'Codigo barras' (smaller size = all visible)
            'textBoxColumnSpecimenID.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_Barcode", pLanguageID)
            textBoxColumnSpecimenID.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BARCODE", pLanguageID)
            'EF 29/08/2013

            bsPatientOrdersDataGridView.Columns.Add(textBoxColumnSpecimenID)
            bsPatientOrdersDataGridView.Columns(columnName).Width = 140
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            'DL 12/06/2013

            'TR 11/03/2013 
            Dim LisRequestIconColumn As New DataGridViewImageColumn
            columnName = "LisRequestIcon"
            LisRequestIconColumn.Name = columnName
            LisRequestIconColumn.HeaderText = "" 'myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Stat", pLanguageID)

            bsPatientOrdersDataGridView.Columns.Add(LisRequestIconColumn)
            bsPatientOrdersDataGridView.Columns(columnName).Width = 50
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'StatFlag column
            Dim checkBoxColumnStat As New DataGridViewCheckBoxColumn
            columnName = "StatFlag"
            checkBoxColumnStat.Name = columnName
            checkBoxColumnStat.HeaderText = "StatFlag"

            bsPatientOrdersDataGridView.Columns.Add(checkBoxColumnStat)
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            'SampleClassIcon column - StatFlag Icon
            'JC 13/11/2012
            Dim iconSampleClassColumn As New DataGridViewImageColumn
            columnName = "SampleClassIcon"
            iconSampleClassColumn.Name = columnName
            iconSampleClassColumn.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Stat", pLanguageID)

            bsPatientOrdersDataGridView.Columns.Add(iconSampleClassColumn)
            bsPatientOrdersDataGridView.Columns(columnName).Width = 52
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'SampleIDType column
            columnName = "SampleIDType"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            'TestType column
            columnName = "TestType"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            'TestTypeIcon column - TestType Icon
            Dim iconTestTypeColumn As New DataGridViewImageColumn
            columnName = "TestTypeIcon"
            iconTestTypeColumn.Name = columnName
            iconTestTypeColumn.HeaderText = "" 'myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_CalcFlag", pLanguageID)

            bsPatientOrdersDataGridView.Columns.Add(iconTestTypeColumn)
            bsPatientOrdersDataGridView.Columns(columnName).Width = 40
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'TestID column
            columnName = "TestID"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            'TestName column
            columnName = "TestName"
            bsPatientOrdersDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", pLanguageID))
            bsPatientOrdersDataGridView.Columns(columnName).Width = 170
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'SampleType column
            columnName = "SampleType"
            bsPatientOrdersDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", pLanguageID))
            bsPatientOrdersDataGridView.Columns(columnName).Width = 70
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'NumReplicates column
            'JC 13/11/2012
            Dim numUpDownReplicates As New DataGridViewNumericUpDownColumn
            Dim numReplicatesLimits(4) As Integer
            numReplicatesLimits = GetNumReplicatesLimits(FieldLimitsEnum.TEST_NUM_REPLICATES)

            columnName = "NumReplicates"
            numUpDownReplicates.DataPropertyName = columnName
            numUpDownReplicates.Name = columnName
            numUpDownReplicates.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_#Replicates_Short", pLanguageID)
            numUpDownReplicates.Minimum = numReplicatesLimits(0)
            numUpDownReplicates.Maximum = numReplicatesLimits(1)
            numUpDownReplicates.DecimalPlaces = numReplicatesLimits(3)
            numUpDownReplicates.Increment = numReplicatesLimits(4)
            numUpDownReplicates.ValueType = GetType(Decimal)

            bsPatientOrdersDataGridView.Columns.Add(numUpDownReplicates)
            bsPatientOrdersDataGridView.Columns(columnName).Width = 60
            bsPatientOrdersDataGridView.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'Tube Types Column
            columnName = "TubeType"
            Dim comboBoxColumnTubeType As New DataGridViewComboBoxColumn()
            comboBoxColumnTubeType.DataPropertyName = columnName
            comboBoxColumnTubeType.Name = columnName
            comboBoxColumnTubeType.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tube", pLanguageID)
            comboBoxColumnTubeType.DataSource = pTubeTypesList
            comboBoxColumnTubeType.DisplayMember = "FixedItemDesc"
            comboBoxColumnTubeType.ValueMember = "ItemID"

            bsPatientOrdersDataGridView.Columns.Add(comboBoxColumnTubeType)
            bsPatientOrdersDataGridView.Columns(columnName).Width = 180
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'CalcTestID column (if the Standard Test is included in more than one CalculatedTest, this column
            'will have the list of Calculated Test IDs divided by commas)
            columnName = "CalcTestID"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            'CalcTestName column (if the Standard Test is included in more than one CalculatedTest, this column
            'will have the list of Calculated Test IDs divided by commas)
            'JC 13/11/2012
            columnName = "CalcTestName"
            bsPatientOrdersDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_CalcTests_Short", pLanguageID))
            bsPatientOrdersDataGridView.Columns(columnName).Width = 120
            bsPatientOrdersDataGridView.Columns(columnName).Visible = True
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'CalcTestFormula column 
            columnName = "CalcTestFormula"
            bsPatientOrdersDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_Formula", pLanguageID))
            bsPatientOrdersDataGridView.Columns(columnName).Width = 160
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'TestProfileID column
            columnName = "TestProfileID"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            'TestProfileName column 
            columnName = "TestProfileName"
            bsPatientOrdersDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_Profile", pLanguageID))
            bsPatientOrdersDataGridView.Columns(columnName).Width = 160
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True
            bsPatientOrdersDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'Order Test Status column
            columnName = "OTStatus"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            'CreationOrder column (needed to sort the records)
            columnName = "CreationOrder"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            'OffSystemResult column (only for requested Off-System Tests)
            columnName = "OffSystemResult"
            bsPatientOrdersDataGridView.Columns.Add(columnName, "")
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True


            Dim checkBoxColumnLISRequest As New DataGridViewCheckBoxColumn
            columnName = "LISRequest"
            checkBoxColumnLISRequest.Name = columnName
            checkBoxColumnLISRequest.HeaderText = "LISRequest"

            bsPatientOrdersDataGridView.Columns.Add(checkBoxColumnLISRequest)
            bsPatientOrdersDataGridView.Columns(columnName).Width = 0
            bsPatientOrdersDataGridView.Columns(columnName).Visible = False
            bsPatientOrdersDataGridView.Columns(columnName).DataPropertyName = columnName
            bsPatientOrdersDataGridView.Columns(columnName).ReadOnly = True

            bsPatientOrdersDataGridView.ScrollBars = ScrollBars.Both
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializePatientGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializePatientGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill ComboBox of Sample Classes
    ''' </summary>
    ''' <remarks>
    ''' Modified by: TR 05/08/2011 - Set to Nothing value of declared List once it has been used to release memory
    ''' </remarks>
    Private Sub InitializeSampleClassComboBox()
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.SAMPLE_CLASSES)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreLoadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                If (myPreLoadedMasterDataDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                    Dim lstSorted As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                    lstSorted = (From a In myPreLoadedMasterDataDS.tfmwPreloadedMasterData _
                             Order By (a.Position) _
                               Select a).ToList()

                    bsSampleClassComboBox.DataSource = lstSorted
                    bsSampleClassComboBox.DisplayMember = "FixedItemDesc"
                    bsSampleClassComboBox.ValueMember = "ItemID"

                    'The first SampleClass is selected by default
                    bsSampleClassComboBox.SelectedIndex = 0
                    lstSorted = Nothing
                End If
            Else
                'Show the error Message
                ShowMessage(Name & ".InitializeSampleClassComboBox", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeSampleClassComboBox", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeSampleClassComboBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill ComboBox of Sample Types
    ''' </summary>
    ''' <remarks>
    ''' Modified by: TR 05/08/2011 - Set to Nothing value of declared List once it has been used to release memory
    ''' </remarks>
    Private Sub InitializeSampleTypeComboBox()
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            Dim myMasterDataDelegate As New MasterDataDelegate

            myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMasterDataDS As MasterDataDS = DirectCast(myGlobalDataTO.SetDatos, MasterDataDS)
                If (myMasterDataDS.tcfgMasterData.Rows.Count > 0) Then
                    Dim lstSorted As List(Of MasterDataDS.tcfgMasterDataRow)
                    lstSorted = (From a In myMasterDataDS.tcfgMasterData _
                                 Order By (a.Position) _
                                 Select a).ToList()

                    bsSampleTypeComboBox.DataSource = lstSorted
                    bsSampleTypeComboBox.DisplayMember = "ItemIDDesc"
                    bsSampleTypeComboBox.ValueMember = "ItemID"
                    lstSorted = Nothing
                End If
            Else
                'Show the error Message
                ShowMessage(Name & ".InitializeSampleTypeComboBox", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeSampleTypeComboBox", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeSampleTypeComboBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Check if all selected rows have the same PatientID/SampleID, StatFlag and SampleType
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/03/2010
    ''' Modified by: SA 29/03/2010 - Implementation changed; it is enough just one loop
    ''' </remarks>
    Private Function IsTheSamePatientSelection() As Boolean
        Dim returnData As Boolean = True
        Try
            Dim currentSampleID As String = ""
            Dim currentStatFlag As String = ""

            For iRow As Integer = 0 To bsPatientOrdersDataGridView.SelectedRows.Count - 1
                If (iRow = 0) Then
                    'Store in variables the values of PatientID/SampleID and StatFlag of the first selected row to compare
                    'if the rest of selected rows have the same values
                    currentSampleID = bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("SampleID").Value.ToString()
                    currentStatFlag = bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("StatFlag").Value.ToString
                Else
                    'Compare values against the reference ones 
                    If (currentSampleID <> bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("SampleID").Value.ToString() OrElse _
                        currentStatFlag <> bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("StatFlag").Value.ToString) Then
                        returnData = False
                        Exit For
                    End If
                End If
            Next iRow
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IsTheSamePatientSelection", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IsTheSamePatientSelection", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return returnData
    End Function

    ''' <summary>
    ''' Verify if all rows selected in the grid of Blank and Calibrator Order Tests have the same SampleClass
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Private Function IsTheSameSampleClassSelection() As Boolean
        Dim returnData As Boolean = True
        Try
            Dim mySampleClass As String = ""
            For iRow As Integer = 0 To bsBlkCalibDataGridView.SelectedRows.Count - 1
                If (iRow = 0) Then
                    'Store in a variable the value of SampleClass of the first selected row to compare if the rest of 
                    'selected rows have the same values
                    mySampleClass = bsBlkCalibDataGridView.SelectedRows(iRow).Cells("SampleClass").Value.ToString()
                Else
                    'Compare values against the reference ones 
                    If (mySampleClass <> bsBlkCalibDataGridView.SelectedRows(iRow).Cells("SampleClass").Value.ToString()) Then
                        returnData = False
                        Exit For
                    End If
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IsTheSameSampleClassSelection", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IsTheSameSampleClassSelection", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return returnData
    End Function

    ''' <summary>
    ''' Verify if all rows selected in the grid of Patient Order Tests have the same SampleType
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 24/03/2010
    ''' Modified by: SA 29/03/2010 - Implementation changed; it is enough just one loop
    ''' </remarks>
    Private Function IsTheSameSampleTypeSelection() As Boolean
        Dim returnData As Boolean = True
        Try
            Dim mySampleType As String = ""
            For iRow As Integer = 0 To bsPatientOrdersDataGridView.SelectedRows.Count - 1
                If (iRow = 0) Then
                    'Store in a variable the value of SampleType of the first selected row to compare if the rest of 
                    'selected rows have the same values
                    mySampleType = bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("SampleType").Value.ToString()

                Else
                    'Compare values against the reference ones 
                    If (mySampleType <> bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("SampleType").Value.ToString()) Then
                        returnData = False
                        Exit For
                    End If
                End If
            Next iRow
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IsTheSameSampleTypeSelection", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IsTheSameSampleTypeSelection", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return returnData
    End Function

    ''' <summary>
    ''' Verify if button for shown the errors in the last executed Import from LIMS process can be enabled
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/09/2010
    ''' Modified by: TR 05/04/2013 - Enable or disable LIS and Barcode Buttons depending LISWithFilesMode parameter
    ''' </remarks>
    Private Sub LIMSErrorsButtonEnabled()
        Try
            If (Not LISWithFilesMode) Then
                'Button is not visible when LIS is working with ES
                bsLIMSErrorsButton.Visible = False
            Else
                Dim resultData As New GlobalDataTO
                Dim myImportErrorsLogDelegate As New ImportErrorsLogDelegate

                resultData = myImportErrorsLogDelegate.GetAll(Nothing)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    myImportErrorsLogDS = DirectCast(resultData.SetDatos, ImportErrorsLogDS)

                    'Buttons is enabled and visible only when there are LIS Import Errors to shown
                    bsLIMSErrorsButton.Enabled = (myImportErrorsLogDS.twksImportErrorsLog.Rows.Count > 0)
                    bsLIMSErrorsButton.Visible = (myImportErrorsLogDS.twksImportErrorsLog.Rows.Count > 0)
                Else
                    'Error getting the list of Import Errors... the error message is shown
                    ShowMessage(Name & ".LIMSErrorsButtonEnabled", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LIMSErrorsButtonEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LIMSErrorsButtonEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the default limits defined for field Num. Orders
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadNumOrdersLimits()
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            Dim fieldLimitsConfig As New FieldLimitsDelegate

            myGlobalDataTO = fieldLimitsConfig.GetList(Nothing, FieldLimitsEnum.NUM_ORDERS)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim fieldDS As FieldLimitsDS = DirectCast(myGlobalDataTO.SetDatos, FieldLimitsDS)
                If (fieldDS.tfmwFieldLimits.Rows.Count > 0) Then
                    'Set the Limits for NumericUpDown
                    bsNumOrdersNumericUpDown.Minimum = Convert.ToInt32(fieldDS.tfmwFieldLimits(0).MinValue)
                    bsNumOrdersNumericUpDown.Maximum = Convert.ToInt32(fieldDS.tfmwFieldLimits(0).MaxValue)
                    bsNumOrdersNumericUpDown.Value = Convert.ToInt32(fieldDS.tfmwFieldLimits(0).DefaultValue)
                    bsNumOrdersNumericUpDown.DecimalPlaces = fieldDS.tfmwFieldLimits(0).DecimalsAllowed
                    bsNumOrdersNumericUpDown.Increment = Convert.ToInt32(fieldDS.tfmwFieldLimits(0).StepValue)
                End If
            Else
                'Show the error Message
                ShowMessage(Name & ".LoadNumOrdersLimits", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadNumOrdersLimits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadNumOrdersLimits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manage the loading of a selected Saved WorkSession. For events bsLoadWSButton_Click and/or
    ''' ScreenShown when the form is opened from menu option Load WorkSession in the main MDI.
    ''' Verify if the selected Saved WS can be loaded, show the ProgressBar and execute the loading
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 22/02/2011
    ''' Modified by: SA 22/09/2011 - Do not shown the warning message when status of active WS is empty
    '''              XB 28/05/2013 - Threads are causing malfunctions on datagridview events so are deleted and related functionalities are synchronous called (bugstrankig #544 and #1102)
    ''' </remarks>
    Private Sub LoadSavedWS()
        Try
            Dim executeLoad As Boolean = True
            If (bsPatientOrdersDataGridView.Rows.Count > 0 OrElse bsControlOrdersDataGridView.Rows.Count > 0 OrElse _
                bsBlkCalibDataGridView.Rows.Count > 0 OrElse (Not String.IsNullOrEmpty(WorkSessionIDAttribute) AndAlso WSStatusAttribute <> "EMPTY")) Then
                executeLoad = (ShowMessage(bsPrepareWSLabel.Text, GlobalEnumerates.Messages.REPLACE_SAMPLES_WITH_SAVED_WS.ToString, , Me) = Windows.Forms.DialogResult.Yes)
            End If

            If (executeLoad) Then
                Cursor = Cursors.WaitCursor
                'IAx00MainMDI.InitializeMarqueeProgreesBar()  ' XB 28/05/2013
                Application.DoEvents()

                'Deactivate the Screen Controls
                ChangeControlsStatusByProgressBar(False)

                Application.DoEvents()
                SavingWS = True
                ScreenWorkingProcess = True 'AG 08/11/2012 - inform this flag because the MDI requires it

                ' XB 28/05/2013
                'Dim workingThread As New Threading.Thread(AddressOf LoadSavedWSOrderTests)
                'workingThread.Start()

                'While SavingWS
                '    IAx00MainMDI.InitializeMarqueeProgreesBar()
                '    Application.DoEvents()
                'End While

                'IAx00MainMDI.StopMarqueeProgressBar()
                Cursor = Cursors.WaitCursor
                Application.DoEvents()
                LoadSavedWSOrderTests()
                ' XB 28/05/2013


                'Bind the DS with the screen grids and set Enabled=True for all controls that can be activated
                BindDSToGrids()

                'Apply styles to special columns and activate again the screen controls
                ApplyStylesToSpecialGridColumns()
                ChangeControlsStatusByProgressBar(True)

                If (ErrorOnSavingWS <> String.Empty) Then
                    'When the selected Saved WS could not be fully loaded (definition of some Tests have been changed) 
                    'or could not be loaded, the warning is shown
                    Dim myErrorData As String() = ErrorOnSavingWS.Split(CChar("|"))
                    ErrorOnSavingWS = String.Empty 'Reset the value after using it

                    IAx00MainMDI.StopMarqueeProgressBar()
                    Application.DoEvents()
                    Cursor = Cursors.Default
                    ShowMessage(Name & ".LoadSavedWS", myErrorData(0), myErrorData(1), Me)
                End If

                ChangesMadeAttribute = True
                Cursor = Cursors.Default
            Else
                'Initialize the attributes for ID and Name of the Saved WS
                WSLoadedIDAttribute = -1
                WSLoadedNameAttribute = ""
            End If
        Catch ex As Exception
            IAx00MainMDI.StopMarqueeProgressBar()
            Application.DoEvents()

            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadSavedWS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadSavedWS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Execute the process of getting all Order Tests in a selected Saved WS and load them in the screen grids
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 22/02/2011
    ''' </remarks>
    Private Sub LoadSavedWSOrderTests()

        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myOrderTests As New OrderTestsDelegate

            myGlobalDataTO = myOrderTests.LoadFromSavedWS(Nothing, WSLoadedIDAttribute, AnalyzerIDAttribute)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myWorkSessionResultDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)
                If (myGlobalDataTO.ErrorCode <> "") Then
                    'Some Order Tests could not be loaded due to changes in Test Programming...
                    ErrorOnSavingWS = String.Format("{0}|{1}", myGlobalDataTO.ErrorCode, "")
                End If
            Else
                'Error loading the Saved WS...
                ErrorOnSavingWS = String.Format("{0}|{1}", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadSavedWSOrderTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".LoadSavedWSOrderTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.UIThread(Function() ShowMessage(Name & ".LoadSavedWSOrderTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))"))
            'DL 15/05/2013
        Finally
            SavingWS = False
            ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it
        End Try
    End Sub

    ''' <summary>
    ''' After changing manually a SampleID or after change the priority of a Patient Order, verify if there are duplicated records 
    ''' and in this case, merge them properly
    ''' </summary>
    ''' <param name="pSampleID">PatientID/SampleID for which the merge is executed</param>
    ''' <param name="pStatFlag">StatFlag for which the merge is executed</param>
    ''' <remarks>
    ''' Created by:  SA 19/05/2010
    ''' Modified by: TR 01/08/2012 - Declare LINQ lists outside loops
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode
    ''' </remarks>
    Private Sub MergePatientOrderTests(ByVal pSampleID As String, ByVal pStatFlag As Boolean)
        Try
            'Get all different TestType, TestID and SampleType for the informed SampleID/StatFlag
            Dim lstDistinctElements As List(Of String)
            lstDistinctElements = (From a In myWorkSessionResultDS.Patients _
                                  Where a.SampleID.ToUpperInvariant = pSampleID.ToUpperInvariant _
                                AndAlso a.StatFlag = pStatFlag _
                                 Select String.Format("{0}|{1}|{2}", a.TestType, a.TestID, a.SampleType) Distinct).ToList

            Dim lstSameOTs As List(Of WorkSessionResultDS.PatientsRow)
            For Each distinctElem As String In lstDistinctElements
                'Get current values:  values(0) --> TestType
                '                     values(1) --> TestID
                '                     values(2) --> SampleType
                Dim values() As String = distinctElem.Split(CChar("|"))

                'Get all Patient Order Tests having same SampleID, StatFlag, TestType, TestID and SampleType, sorted by Status
                'Dim lstSameOTs As List(Of WorkSessionResultDS.PatientsRow)
                lstSameOTs = (From b In myWorkSessionResultDS.Patients _
                              Where b.SampleID.ToUpperInvariant = pSampleID.ToUpperInvariant _
                              AndAlso b.StatFlag = pStatFlag _
                              AndAlso b.TestType = values(0) _
                              AndAlso b.TestID = Convert.ToInt32(values(1)) _
                              AndAlso b.SampleType = values(2) _
                              Order By b.OTStatus _
                              Select b).ToList

                If (lstSameOTs.Count = 2) Then
                    'Duplicated Order Tests... merge fields that can be different

                    'Case 1 - TEST PROFILE...
                    If (Not lstSameOTs(0).IsTestProfileIDNull) Then
                        'If the Test is linked to a TestProfile in the first row, nothing to do...

                    ElseIf (Not lstSameOTs(1).IsTestProfileIDNull) Then
                        'Update Test Profile fields in the first row with values of the second one
                        lstSameOTs(0).TestProfileID = lstSameOTs(1).TestProfileID
                        lstSameOTs(0).TestProfileName = lstSameOTs(1).TestProfileName
                    End If

                    'Case 2 - CALCULATED TESTS...
                    If (lstSameOTs(1).IsCalcTestIDNull) Then
                        'If the Test in the second row is not linked to any Calculated Test, nothing to do...

                    Else
                        If (lstSameOTs(0).IsCalcTestIDNull) Then
                            'Update Calculated Test fields in the first row with values of the second one
                            lstSameOTs(0).CalcTestID = lstSameOTs(1).CalcTestID
                            lstSameOTs(0).CalcTestName = lstSameOTs(1).CalcTestName

                        ElseIf (lstSameOTs(0).CalcTestID = lstSameOTs(1).CalcTestID) Then
                            'Nothing to do...

                        Else
                            'Get the list of Calculated Tests in both rows
                            Dim calcIDsFst() As String = lstSameOTs(0).CalcTestID.Split(CChar(", "))
                            Dim calcIDsSnd() As String = lstSameOTs(1).CalcTestID.Split(CChar(", "))
                            Dim calcNamesSnd() As String = lstSameOTs(1).CalcTestName.Split(CChar(", "))

                            'Merge the list of Calculated Tests
                            For i As Integer = 0 To calcIDsSnd.Length - 1
                                Dim testFound As Boolean = False
                                For j As Integer = 0 To calcIDsFst.Length - 1
                                    If (calcIDsSnd(i) = calcIDsFst(j)) Then
                                        testFound = True
                                        Exit For
                                    End If
                                Next

                                'Add the Calculated Tests to the list of Calculated Tests in the first row
                                If (Not testFound) Then
                                    lstSameOTs(0).CalcTestID &= ", " & calcIDsSnd(i)
                                    lstSameOTs(0).CalcTestName &= ", " & calcNamesSnd(i)
                                End If
                            Next
                        End If
                    End If

                    'Finally, delete the second row...
                    lstSameOTs.Last.Delete()
                    myWorkSessionResultDS.Patients.AcceptChanges()
                End If
            Next
            lstSameOTs = Nothing
            lstDistinctElements = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MergePatientOrderTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MergePatientOrderTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if button for open screen to add results for requested Off-System Tests can be enabled
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/01/2011
    ''' Modified by: SA 12/03/2012 - When the status of the current WS is ABORTED, disable the OffSystemResults button
    ''' </remarks>
    Private Sub OffSystemResultsButtonEnabled()
        Try
            Dim lstRequestedOFFSystems As List(Of WorkSessionResultDS.PatientsRow)
            lstRequestedOFFSystems = (From a As WorkSessionResultDS.PatientsRow In myWorkSessionResultDS.Patients _
                                      Where a.TestType = "OFFS" _
                                      Select a).ToList()

            bsOffSystemResultsButton.Enabled = (lstRequestedOFFSystems.Count > 0) AndAlso (WSStatusAttribute <> "ABORTED")
            lstRequestedOFFSystems = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OffSystemResultsButtonEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OffSystemResultsButtonEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the list of requested Off-System Tests and open the auxiliary screen that allow enter results
    ''' for them. When the auxiliary screen is closed, the results are updated in the local Patients DS 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/01/2011
    ''' </remarks>
    Private Sub OpenOffSystemResultsScreen()
        Try
            Cursor = Cursors.WaitCursor

            'Get all requested Off-System Tests
            Dim lstRequestedOFFSystems As List(Of WorkSessionResultDS.PatientsRow)
            lstRequestedOFFSystems = (From a As WorkSessionResultDS.PatientsRow In myWorkSessionResultDS.Patients _
                                     Where a.TestType = "OFFS" _
                                  Order By a.TestID _
                                    Select a).ToList()

            Dim myCurrentUnit As String = ""
            Dim myCurrentTest As Integer = -1
            Dim myCurrentDecimals As Integer = 0
            Dim myCurrentResultType As String = ""

            Dim stopProcess As Boolean = False
            Dim resultData As New GlobalDataTO
            Dim myOffSystemTestDS As OffSystemTestsDS
            Dim myOffSystemTestDelegate As New OffSystemTestsDelegate
            Dim myOffSystemTestsResultsDS As New OffSystemTestsResultsDS
            Dim myOFFSTestResultRow As OffSystemTestsResultsDS.OffSystemTestsResultsRow

            For Each reqOFFS As WorkSessionResultDS.PatientsRow In lstRequestedOFFSystems
                If (reqOFFS.TestID <> myCurrentTest) Then
                    resultData = myOffSystemTestDelegate.ReadWithUnitDesc(Nothing, reqOFFS.TestID)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myOffSystemTestDS = DirectCast(resultData.SetDatos, OffSystemTestsDS)

                        If (myOffSystemTestDS.tparOffSystemTests.Rows.Count = 1) Then
                            myCurrentResultType = myOffSystemTestDS.tparOffSystemTests(0).ResultType

                            If (myCurrentResultType = "QUANTIVE") Then
                                myCurrentUnit = myOffSystemTestDS.tparOffSystemTests(0).Units
                                myCurrentDecimals = Convert.ToInt32(myOffSystemTestDS.tparOffSystemTests(0).Decimals)
                            Else
                                myCurrentUnit = ""
                                myCurrentDecimals = 0
                            End If
                        End If
                        myCurrentTest = reqOFFS.TestID
                    Else
                        'Error getting data of the OffSystem Test; shown it
                        Cursor = Cursors.Default
                        stopProcess = True
                        ShowMessage(Me.Name & ".OpenOffSystemResultsScreen", resultData.ErrorCode, resultData.ErrorMessage, Me)
                        Exit For
                    End If
                End If

                myOFFSTestResultRow = myOffSystemTestsResultsDS.OffSystemTestsResults.NewOffSystemTestsResultsRow
                If (Not reqOFFS.IsOrderTestIDNull) Then
                    myOFFSTestResultRow.OrderTestID = reqOFFS.OrderTestID
                End If

                myOFFSTestResultRow.SampleID = reqOFFS.SampleID
                myOFFSTestResultRow.StatFlag = reqOFFS.StatFlag
                myOFFSTestResultRow.StatFlagIcon = reqOFFS.SampleClassIcon
                myOFFSTestResultRow.SampleType = reqOFFS.SampleType
                myOFFSTestResultRow.TestID = reqOFFS.TestID
                myOFFSTestResultRow.TestName = reqOFFS.TestName
                myOFFSTestResultRow.Unit = myCurrentUnit
                myOFFSTestResultRow.AllowedDecimals = myCurrentDecimals
                myOFFSTestResultRow.ResultType = myCurrentResultType
                myOFFSTestResultRow.QuantitativeFlag = (myOFFSTestResultRow.ResultType = "QUANTIVE")

                If (Not reqOFFS.IsOffSystemResultNull) Then
                    myOFFSTestResultRow.ResultValue = reqOFFS.OffSystemResult
                End If
                myOffSystemTestsResultsDS.OffSystemTestsResults.AddOffSystemTestsResultsRow(myOFFSTestResultRow)
            Next
            myOffSystemTestsResultsDS.AcceptChanges()

            If (Not stopProcess) Then
                Cursor = Cursors.Default

                'Open the auxiliary screen that allow inform a result for each requested OffSystem Test
                Dim myAuxOFFSResultsScreen As New IResultsOffSystemsTest
                myAuxOFFSResultsScreen.OffSystemTestsList = myOffSystemTestsResultsDS

                myAuxOFFSResultsScreen.ShowDialog()
                If (myAuxOFFSResultsScreen.DialogResult = Windows.Forms.DialogResult.OK) Then
                    Cursor = Cursors.WaitCursor

                    'Recovery the list of updated results
                    Dim lStrNewResult As List(Of OffSystemTestsResultsDS.OffSystemTestsResultsRow)

                    myOffSystemTestsResultsDS = myAuxOFFSResultsScreen.OffSystemTestsList
                    For Each reqOFFS As WorkSessionResultDS.PatientsRow In lstRequestedOFFSystems
                        'Search the Result Value for each different SampleID/StatFlag/SampleType/TestID
                        lStrNewResult = (From b As OffSystemTestsResultsDS.OffSystemTestsResultsRow In myOffSystemTestsResultsDS.OffSystemTestsResults _
                                        Where b.SampleID = reqOFFS.SampleID _
                                      AndAlso b.StatFlag = reqOFFS.StatFlag _
                                      AndAlso b.SampleType = reqOFFS.SampleType _
                                      AndAlso b.TestID = reqOFFS.TestID _
                                  AndAlso Not b.IsResultValueNull _
                                       Select b).ToList

                        reqOFFS.BeginEdit()
                        If (lStrNewResult.Count = 0) Then
                            reqOFFS.SetOffSystemResultNull()
                        Else
                            If (Not lStrNewResult(0).QuantitativeFlag) Then
                                reqOFFS.OffSystemResult = lStrNewResult(0).ResultValue
                            Else
                                If (String.IsNullOrEmpty(lStrNewResult(0).ResultValue)) Then
                                    reqOFFS.SetOffSystemResultNull()
                                Else
                                    'Format the numeric value according the number of decimals allowed for the OFF-SYSTEM Test
                                    reqOFFS.OffSystemResult = CType(lStrNewResult(0).ResultValue, Double).ToString("F" & lStrNewResult(0).AllowedDecimals.ToString)
                                End If
                            End If
                        End If
                        reqOFFS.AcceptChanges()
                    Next
                    lStrNewResult = Nothing

                    Cursor = Cursors.Default
                    ChangesMadeAttribute = True
                End If
            End If
            lstRequestedOFFSystems = Nothing
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OpenOffSystemResultsScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenOffSystemResultsScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if button OpenRotor can be enabled or not
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 02/11/2010 - Set the same status to the menu option and button in the MDI
    '''                              by calling function SetStatusRotorPosOptions in the MDI
    ''' </remarks>
    Private Sub OpenRotorButtonEnabled()
        Try
            If (Not WorkSessionIDAttribute.Trim = "") AndAlso (WSStatusAttribute <> "OPEN") Then
                bsOpenRotorButton.Enabled = True
            Else
                'If there are selected rows in whatever grid then the button is enabled
                Dim lstrWSSelectedRowsDS As List(Of WorkSessionResultDS.PatientsRow)
                lstrWSSelectedRowsDS = (From a In myWorkSessionResultDS.Patients _
                                       Where a.Selected = True _
                                     AndAlso a.OTStatus = "OPEN" _
                                      Select a).ToList()

                If (lstrWSSelectedRowsDS.Count > 0) Then
                    bsOpenRotorButton.Enabled = True
                Else
                    Dim lstWSBlankCalibsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                    lstWSBlankCalibsDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                                         Where a.Selected = True _
                                       AndAlso a.OTStatus = "OPEN" _
                                        Select a).ToList()

                    If (lstWSBlankCalibsDS.Count > 0) Then
                        bsOpenRotorButton.Enabled = True
                    Else
                        Dim lstWSControlDS As List(Of WorkSessionResultDS.ControlsRow)
                        lstWSControlDS = (From a In myWorkSessionResultDS.Controls _
                                         Where a.Selected = True _
                                       AndAlso a.OTStatus = "OPEN" _
                                        Select a).ToList()

                        bsOpenRotorButton.Enabled = (lstWSControlDS.Count > 0)
                        lstWSControlDS = Nothing
                    End If
                    lstWSBlankCalibsDS = Nothing
                End If
                lstrWSSelectedRowsDS = Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OpenRotorButtonEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenRotorButtonEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set Gold Background to rows containing Blank and Calibrator Order Tests that have been already
    ''' sent to positioning in an Analyzer Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 10/03/2010
    ''' Modified by: DL 28/03/2010 - Changed BackColor of not open Patient Order Tests 
    ''' </remarks>
    Private Sub PaintBlkCalibratRowsExists()
        Try
            For Each row As DataGridViewRow In bsBlkCalibDataGridView.Rows
                If (UCase(row.Cells("OTStatus").Value.ToString) <> "OPEN") Then
                    row.DefaultCellStyle.BackColor = Color.LightGray 'Color.FromArgb(255, 255, 128) 'Color.Gold
                Else
                    row.DefaultCellStyle.BackColor = Color.White
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PaintBlkCalibratRowsExists", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PaintBlkCalibratRowsExists", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set Gold Background to rows containing Control Order Tests that have been already
    ''' sent to positioning in an Analyzer Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 10/03/2010
    ''' Modified by: DL 28/03/2010 - Changed BackColor of not open Patient Order Tests 
    ''' </remarks>
    Private Sub PaintControlRowsExists()
        Try
            For Each Row As DataGridViewRow In bsControlOrdersDataGridView.Rows
                If (UCase(Row.Cells("OTStatus").Value.ToString) <> "OPEN") Then
                    Row.DefaultCellStyle.BackColor = Color.LightGray 'Color.FromArgb(255, 255, 128) 'color.Gold
                Else
                    Row.DefaultCellStyle.BackColor = Color.White
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PaintControlRowsExists", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PaintControlRowsExists", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set Gold Background to rows containing Patient Order Tests that have been already
    ''' sent to positioning in an Analyzer Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 19/04/2010
    ''' Modified by: DL 28/03/2010 - Changed BackColor of not open Patient Order Tests 
    ''' </remarks>
    Private Sub PaintPatientRowExists()
        Try
            If (SavingWS) Then Return
            For Each Row As DataGridViewRow In bsPatientOrdersDataGridView.Rows
                If (UCase(Row.Cells("OTStatus").Value.ToString) <> "OPEN") Then
                    Row.DefaultCellStyle.BackColor = Color.LightGray ' Color.FromArgb(255, 255, 128) 'Color.Gold
                Else
                    Row.DefaultCellStyle.BackColor = Color.White
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PaintPatientRowExists", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PaintPatientRowExists", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages changes in the screen when a PatientID/SampleID is informed in the correspondent TextBox in the
    ''' header area. For event bsPatientIDTextBox_TextChanged
    ''' </summary>
    ''' <remarks>
    ''' Modified by: TR 25/07/2011 - If the Patient value changed, clear the ToolTip
    ''' </remarks>
    Private Sub PatientIDChangedInTextBox()
        Try
            'Clear the ToolTip
            bsScreenToolTips.SetToolTip(bsPatientIDTextBox, "")

            'Number of Orders is enabled only when a PatientID/SampleID has not been informed
            bsNumOrdersNumericUpDown.Enabled = String.IsNullOrEmpty(bsPatientIDTextBox.Text)
            If (Not bsNumOrdersNumericUpDown.Enabled) Then bsNumOrdersNumericUpDown.Value = 1

            'If the first character is #, it is removed due to it is a special character used for 
            'SampleID automatically generated - although it is not posible write the character, this is to
            'avoid the character when the field is informed through copy/paste
            If (Not String.IsNullOrEmpty(bsPatientIDTextBox.Text)) Then
                If (Not bsPatientIDTextBox.ReadOnly AndAlso bsPatientIDTextBox.Text.Substring(0, 1) = "#") Then bsPatientIDTextBox.Text = bsPatientIDTextBox.Text.Substring(1)

                'If a PatientID/SampleID has not been validated, the it is marked as Manual
                If (sampleIDType <> "DB") Then
                    sampleIDType = "MAN"
                    validatedSampleID = False  'The SampleID is pending to be validated...
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PatientIDChangedInTextBox", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PatientIDChangedInTextBox", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manage the selection/deselection of Patient Samples in the grid of Patient Order Tests
    ''' For event bsPatientOrdersDataGridView_CellMouseUp
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 10/03/2010
    ''' Modified by: SA 12/03/2010 - Load also StatFlag and SampleType and disable Patient fields
    '''              SA 29/03/2010 - Changes in the implementation when multiple rows have been selected; Select Case replace by If/Else
    '''                              due to problems when no row was selected 
    ''' </remarks>
    Private Sub PatientOrderTestsCellMouseUp()
        Try
            If (Not bsPatientOrdersDataGridView.CurrentRow Is Nothing) Then
                'Dim columnClicked As Integer = bsPatientOrdersDataGridView.CurrentCell.ColumnIndex

                If Not bsPatientOrdersDataGridView.CurrentCell.IsInEditMode Then
                    patientEventSource = "Left_Click"

                    'Get PatientID/SampleID, StatFlag and SampleType of the last clicked row in grid of Patient Order Tests
                    Dim mySampleID As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value.ToString
                    Dim mySampleIDType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleIDType").Value.ToString
                    Dim myStatFlag As Boolean = CBool(bsPatientOrdersDataGridView.CurrentRow.Cells("StatFlag").Value)
                    Dim mySampleType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString

                    'Case 1
                    If (bsPatientOrdersDataGridView.SelectedRows.Count <= 1) Then
                        'clickSource = True
                        If (bsPatientOrdersDataGridView.CurrentRow.Index = myPatientRow) Then
                            If (myCountPatientClicks = 1) Then
                                'Row is unmarked and controls in the header area are enabled
                                patientEventSource = "Left_Click_Disabled"
                                bsPatientOrdersDataGridView.ClearSelection()

                                myPatientRow = -1

                                HeaderStatusForPatients(False)

                                sampleIDType = "MAN"
                                validatedSampleID = False
                            Else
                                'Row is marked as selected and values of PatientID/SampleID, StatFlag and SampleType are loaded
                                'in the header area controls
                                patientEventSource = "Left_Click_Enabled"

                                bsStatCheckbox.Checked = myStatFlag
                                bsSampleTypeComboBox.SelectedValue = mySampleType
                                sampleIDType = mySampleIDType

                                HeaderStatusForPatients(True)

                                'For AUTO SampleIDs, it is needed that this sentence be here, after calling HeaderStatusForPatients
                                bsPatientIDTextBox.Text = mySampleID
                            End If
                        Else
                            'A different row has been selected... controls in the header area are loaded with values of the new selected rows
                            bsStatCheckbox.Checked = myStatFlag
                            bsSampleTypeComboBox.SelectedValue = mySampleType
                            sampleIDType = mySampleIDType

                            HeaderStatusForPatients(True)

                            'For AUTO SampleIDs, it is needed that this sentence be here, after calling HeaderStatusForPatients
                            bsPatientIDTextBox.Text = mySampleID

                            myPatientRow = bsPatientOrdersDataGridView.CurrentRow.Index
                            myCountPatientClicks = 1
                        End If
                    Else
                        'Case Is > 1
                        'Dim hasDiference As Boolean = False

                        'Verify if data has to be loaded in the header area
                        If (IsTheSamePatientSelection()) Then
                            'If all selected rows have the same PatientID/SampleID, StatFlag and SampleType, controls in the header are are loaded
                            'with selected values (in the same way than when just one row has been selected) - Values are get from the first selected row
                            'In this case is possible to add more Tests (using the selected SampleType) to the selected PatientID/SampleID and StatFlag
                            bsStatCheckbox.Checked = DirectCast(bsPatientOrdersDataGridView.SelectedRows(0).Cells("StatFlag").Value, Boolean)
                            bsSampleTypeComboBox.SelectedValue = bsPatientOrdersDataGridView.SelectedRows(0).Cells("SampleType").Value.ToString

                            HeaderStatusForPatients(True)

                            bsPatientIDTextBox.Text = bsPatientOrdersDataGridView.SelectedRows(0).Cells("SampleID").Value.ToString

                        ElseIf (IsTheSameSampleTypeSelection()) Then
                            'If all selected rows have the same SampleType (although they have different PatientID/SampleID and StatFlag), the SampleType
                            'is selected in the correspondent ComboBox in the header area; Patient controls in the header area are empty and disable - Value
                            'is get from the first selected row.  In this case is possible to add several Tests using the selected SampleType to the group of 
                            'selected PatientID/SampleID and StatFlag
                            bsStatCheckbox.Checked = False
                            bsSampleTypeComboBox.SelectedValue = bsPatientOrdersDataGridView.SelectedRows(0).Cells("SampleType").Value.ToString
                            bsPatientIDTextBox.Text = ""

                            HeaderStatusForPatients(True)
                        Else
                            'If all selected rows have different PatientID/SampleID, StatFlag and SampleType, no information is loaded in the controls
                            'in the header area
                            HeaderStatusForPatients(False)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PatientOrderTestsClickRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PatientOrderTestsClickRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' After adding a row in the grid of Blanks and Calibrators, assign the BackColor and set the availability of all editable columns
    ''' For event bsBlkCalibDataGridView_RowPostPaint
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/04/2010 - Code moved from the event
    '''              SA 24/11/2010 - Added control of availability of cell FactorValue
    ''' Modified by  RH 03/06/2011 - Now Blank rows are not ReadOnly
    ''' </remarks>
    Private Sub PostPaintBlkCalibratorRow()
        Try
            'RH 07/06/2011
            Dim dgvr As DataGridViewRow = bsBlkCalibDataGridView.CurrentRow

            'Editable columns are ReadOnly for Blanks and Calibrators having OrderTestStatus different of OPEN 
            Dim readOnlyColumns As Boolean = (dgvr.Cells("OTStatus").Value.ToString() <> "OPEN")

            dgvr.Cells("Selected").ReadOnly = readOnlyColumns
            dgvr.Cells("NumReplicates").ReadOnly = readOnlyColumns
            dgvr.Cells("NewCheck").ReadOnly = (readOnlyColumns OrElse String.IsNullOrEmpty(dgvr.Cells("PreviousOrderTestID").Value.ToString()))

            If (dgvr.Cells("SampleClass").Value.ToString() = "CALIB") Then
                'If the current row corresponds to a Calibrator, search all rows having the same one.  If at least one of the rows has 
                'OrderTestStatus different to OPEN, then the TubeType column will be read only --> that means, when a Calibrator has been 
                'already positioned in the Analyzer, the TubeType used for it can not be changed from this screen
                Dim currentCalibratorID As Integer = Convert.ToInt32(dgvr.Cells("CalibratorID").Value)

                Dim lstWSSameCalibDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                lstWSSameCalibDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                                   Where a.SampleClass = "CALIB" _
                                 AndAlso (Not a.IsCalibratorIDNull AndAlso a.CalibratorID = currentCalibratorID) _
                                 AndAlso a.OTStatus <> "OPEN" _
                                  Select a).ToList()

                bsBlkCalibDataGridView.CurrentRow.Cells("TubeType").ReadOnly = (lstWSSameCalibDS.Count > 0)
                lstWSSameCalibDS = Nothing

                'If the current row corresponds to a Calibrator, cell for FactorValue can be changed in following cases:
                ' ** The Calibrator Order Test does not have a status different of OPEN
                ' ** The Order Test is for a single point Calibrator
                ' ** There is a previous result for the Calibrator Order Test and New check remains unselected
                ' ** Value of Calibrator Factor is informed
                dgvr.Cells("FactorValue").ReadOnly = readOnlyColumns OrElse _
                                                     (Convert.ToBoolean(dgvr.Cells("NewCheck").Value)) OrElse _
                                                     (Convert.ToInt32(dgvr.Cells("NumberOfCalibrators").Value) > 1) OrElse _
                                                      String.IsNullOrEmpty(dgvr.Cells("PreviousOrderTestID").Value.ToString()) OrElse _
                                                      String.IsNullOrEmpty(dgvr.Cells("OriginalFactorValue").Value.ToString)
            Else
                'The current row corresponds to a Blank, column FactorValue is always locked; column TubeType is ReadOnly when BlankMode is Reagent
                dgvr.Cells("TubeType").ReadOnly = (readOnlyColumns OrElse (dgvr.Cells("BlankMode").Value.ToString() = "REAGENT"))
                dgvr.Cells("FactorValue").ReadOnly = True
            End If

            'Blanks and Calibrators having OrderTestStatus different of OPEN are shown with special background color (Gold)
            PaintBlkCalibratRowsExists()

            'Set the Wrap mode of cell AbsValue for multipoint Calibrators having previous results
            SetWrapCells()
            Select Case blkCalibEventSource
                Case "Load"
                    bsBlkCalibDataGridView.ClearSelection()

                Case "Left_Click", "Left_Click_Enabled"
                    bsBlkCalibDataGridView.Rows(bsBlkCalibDataGridView.CurrentRow.Index).Selected = True

                Case "Left_Click_Disabled"
                    bsBlkCalibDataGridView.Rows(bsBlkCalibDataGridView.CurrentRow.Index).Selected = False
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsBlkCalibDataGridView_RowPostPaint", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsBlkCalibDataGridView_RowPostPaint", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' After adding a row in the grid of Controls, assign the BackColor and set the availability of all editable columns
    ''' For event bsControlOrdersDataGridView_RowPostPaint
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/04/2010 - Code moved from the event
    ''' Modified by: SA 19/06/2012 - Validations to set the ReadOnly status of the NumReplicates will depend not only in the 
    '''                              Order Test Status, but also in the TestType; the linq is filtered by TestType
    '''              TR 11/03/2013 - Set readOnlyColumns = TRUE when (OTStatus <> OPEN OrElse LISRequest = TRUE).
    '''                              Set selected column as ReadOnly when (OTStatus <> OPEN).
    '''                              Change filter OTStatus <> OPEN in both LINQs by the following one: (OTStatus <> OPEN OrElse LISRequest = TRUE)
    ''' </remarks>
    Private Sub PostPaintControlRow()
        Try
            'Editable columns are read only for Controls having Order Test Status different of OPEN 
            Dim readOnlyColumns As Boolean = (bsControlOrdersDataGridView.CurrentRow.Cells("OTStatus").Value.ToString <> "OPEN" OrElse
                                              bsControlOrdersDataGridView.CurrentRow.Cells("LISRequest").Value.ToString <> "True")

            bsControlOrdersDataGridView.CurrentRow.Cells("Selected").ReadOnly = _
                                        bsControlOrdersDataGridView.CurrentRow.Cells("OTStatus").Value.ToString <> "OPEN"

            'Search all rows having the same Control than the selected one.  If at least one of the rows has 
            'OrderTestStatus different to OPEN, then the TubeType column will be read only --> that means, when a Control has been 
            'already positioned in the Analyzer, the TubeType used for it can not be changed from this screen
            Dim currentControlID As Integer = Convert.ToInt32(bsControlOrdersDataGridView.CurrentRow.Cells("ControlID").Value)

            Dim lstWSSameCtrlDS As List(Of WorkSessionResultDS.ControlsRow)
            lstWSSameCtrlDS = (From a In myWorkSessionResultDS.Controls _
                              Where a.SampleClass = "CTRL" _
                                AndAlso a.ControlID = currentControlID _
                                AndAlso (a.OTStatus <> "OPEN" OrElse a.LISRequest = True) _
                             Select a).ToList()

            bsControlOrdersDataGridView.CurrentRow.Cells("TubeType").ReadOnly = (lstWSSameCtrlDS.Count > 0)

            'Search all rows containing the Controls required for the selected TestType, Test and SampleType. If at least one of the rows has 
            'OrderTestStatus different to OPEN, then the NumReplicates column will be read only --> that means, when for a TestType, Test and SampleType
            'with two Controls (Q1 and Q2), the Q1 has been already positioned in the Analyzer, field NumReplicates for Q2 can not be changed
            Dim currentTestType As String = bsControlOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString
            Dim currentTestID As Integer = Convert.ToInt32(bsControlOrdersDataGridView.CurrentRow.Cells("TestID").Value)
            Dim currentSampleType As String = bsControlOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString

            lstWSSameCtrlDS = (From a In myWorkSessionResultDS.Controls _
                              Where a.SampleClass = "CTRL" _
                            AndAlso a.TestType = currentTestType _
                            AndAlso a.TestID = currentTestID _
                            AndAlso a.SampleType = currentSampleType _
                            AndAlso (a.OTStatus <> "OPEN" OrElse a.LISRequest = True) _
                             Select a).ToList()

            bsControlOrdersDataGridView.CurrentRow.Cells("NumReplicates").ReadOnly = (lstWSSameCtrlDS.Count > 0)
            lstWSSameCtrlDS = Nothing

            PaintControlRowsExists()
            Select Case controlEventSource
                Case "Load"
                    bsControlOrdersDataGridView.ClearSelection()

                Case "Left_Click", "Left_Click_Enabled"
                    bsControlOrdersDataGridView.Rows(bsControlOrdersDataGridView.CurrentRow.Index).Selected = True

                Case "Left_Click_Disabled"
                    bsControlOrdersDataGridView.Rows(bsControlOrdersDataGridView.CurrentRow.Index).Selected = False
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsControlOrdersDataGridView_RowPostPaint", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsControlOrdersDataGridVieww_RowPostPaint", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' After adding a row in the grid of Patients, assign the BackColor and set the availability of all editable columns
    ''' For event bsPatientOrdersDataGridView_RowPostPaint
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/04/2010 - Code moved from the event
    ''' Modified by: SA 27/04/2010 - Num Replicates can't be modified when there are not OPEN Order Test for the same 
    '''                              SampleID/PatientID and TestID
    '''              SA 03/11/2010 - Num Replicates can't be modified when the Order Test is for a Test Type different of STANDARD
    '''              SA 02/12/2010 - Tube Type can't be modified when the Order Test is for a Test Type different of STANDARD
    '''              SA 13/12/2010 - Allow modification of Num Replicates also for ISE Tests
    '''              SA 18/01/2011 - Column selected will be always read only for requested Off-System Tests
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode
    '''              TR 11/03/2013 - Set readOnlyColumns = TRUE when (OTStatus <> OPEN OrElse LISRequest = TRUE).
    '''                            - Set selected column as ReadOnly when (OTStatus <> OPEN OrElse TestType = OFFS)
    '''                            - Change filter OTStatus <> OPEN in both LINQs by the following one:
    '''                               (OTStatus <> OPEN OrElse LISRequest = TRUE)  
    ''' </remarks>
    Private Sub PostPaintPatientRow()
        Try
            'Editable columns are read only for Patients having Order Test Status different of OPEN 
            Dim readOnlyColumns As Boolean = (bsPatientOrdersDataGridView.CurrentRow.Cells("OTStatus").Value.ToString <> "OPEN" _
                                              OrElse bsPatientOrdersDataGridView.CurrentRow.Cells("LISRequest").Value.ToString = "True")

            bsPatientOrdersDataGridView.CurrentRow.Cells("Selected").ReadOnly = _
                                                    bsPatientOrdersDataGridView.CurrentRow.Cells("OTStatus").Value.ToString <> "OPEN" OrElse _
                                                    (bsPatientOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString = "OFFS")

            'Search all rows having the same Patient and SampleType than the selected one.  If at least one of the rows has 
            'OrderTestStatus different to OPEN, then: 
            '  ** The SampleID column will be read only --> if we allow changing the SampleID, there are problems with Tests belonging
            '     to Test Profiles and/or Calculated Tests
            '  ** The TubeType column will be read only --> that means, when a SampleType for a Patient has been 
            '     already positioned in the Analyzer, the TubeType used for it can not be changed from this screen
            Dim currentSampleID As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value.ToString
            Dim currentSampleType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString

            Dim lstWSSamePatientDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSSamePatientDS = (From a In myWorkSessionResultDS.Patients _
                                 Where a.SampleClass = "PATIENT" _
                                   AndAlso a.SampleID.ToUpperInvariant = currentSampleID.ToUpperInvariant _
                                   AndAlso a.SampleType = currentSampleType _
                                   AndAlso (a.OTStatus <> "OPEN" OrElse a.LISRequest = True) _
                                 Select a).ToList()

            bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").ReadOnly = readOnlyColumns OrElse _
                                                                                (bsPatientOrdersDataGridView.SelectedRows.Count > 1) OrElse _
                                                                                (lstWSSamePatientDS.Count > 0)

            bsPatientOrdersDataGridView.CurrentRow.Cells("TubeType").ReadOnly = (lstWSSamePatientDS.Count > 0) OrElse _
                                                                                (bsPatientOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString = "CALC") OrElse _
                                                                                (bsPatientOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString = "OFFS")
            'Search all rows having the same Patient and Test than the selected one.  If at least one of the rows has 
            'OrderTestStatus different to OPEN, then the NumReplicates column will be read only (because the Number of Replicates 
            'is set by Test, no matter which SampleType is required)
            Dim currentTest As Integer = Convert.ToInt32(bsPatientOrdersDataGridView.CurrentRow.Cells("TestID").Value)
            lstWSSamePatientDS = (From a In myWorkSessionResultDS.Patients _
                                 Where a.SampleClass = "PATIENT" _
                                   AndAlso a.SampleID.ToUpperInvariant = currentSampleID.ToUpperInvariant _
                                   AndAlso a.TestType = "STD" _
                                   AndAlso a.TestID = currentTest _
                                   AndAlso (a.OTStatus <> "OPEN" OrElse a.LISRequest = True) _
                                Select a).ToList()

            bsPatientOrdersDataGridView.CurrentRow.Cells("NumReplicates").ReadOnly = (lstWSSamePatientDS.Count > 0) OrElse _
                                                                                     (bsPatientOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString = "CALC") OrElse _
                                                                                     (bsPatientOrdersDataGridView.CurrentRow.Cells("TestType").Value.ToString = "OFFS")
            lstWSSamePatientDS = Nothing
            PaintPatientRowExists()

            Select Case patientEventSource
                Case "Load", "Change_SampleClass", "Search_Tests"
                    bsPatientOrdersDataGridView.ClearSelection()

                Case "Left_Click"
                    bsPatientOrdersDataGridView.Rows(bsPatientOrdersDataGridView.CurrentRow.Index).Selected = True

                Case "Left_Click_Disabled"
                    bsPatientOrdersDataGridView.Rows(bsPatientOrdersDataGridView.CurrentRow.Index).Selected = False

                Case "Left_Click_Enabled"
                    bsPatientOrdersDataGridView.Rows(bsPatientOrdersDataGridView.CurrentRow.Index).Selected = True

                Case "Right_Click"
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PostPaintPatientRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PostPaintPatientRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to load the image for each graphical button 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/05/2010
    ''' Modified by: SA 03/11/2010 - Set value of Image Property instead of BackgroundImage Property; removed Icon in button Search Tests
    '''              SA 18/01/2011 - Get OffSystem Tests Icon for the new button to opening the auxiliary screen to add results for this 
    '''                              type of Tests
    '''              TR 16/09/2011 - Get Barcode Warnings Icon for the new button to opening the auxiliary screen to check incomplete Patient Samples
    '''              AG 03/10/2011 - Added code to enabling/disabling the Scanning Button depending on the Analyzer status
    '''              AG 25/11/2011 - Get value of Analyzer Setting SAMPLE_BARCODE_DISABLED to verify if the Barcode is enabled for the Analyzer
    '''              AG 06/02/2012 - Added condition mdiAnalyzerCopy.Connected to the group of conditions that have to be fulfilled to enabling
    '''                              the Scanning Button
    '''              SA 12/03/2012 - Code to enabling/disabling the Scanning Button moved to a new sub ValidateScanningButtonEnabled; this function is 
    '''                              only to load images for graphical buttons
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            'LOAD SAVED WS Button
            auxIconName = GetIconName("OPEN")
            If Not String.Equals(auxIconName, String.Empty) Then bsLoadWSButton.Image = Image.FromFile(iconPath & auxIconName)

            'SAVE WS Button
            auxIconName = GetIconName("SAVE")
            If Not String.Equals(auxIconName, String.Empty) Then bsSaveWSButton.Image = Image.FromFile(iconPath & auxIconName)

            'DELETE Buttons (Patient Samples, Controls, Blanks&Calibrators)
            auxIconName = GetIconName("REMOVE")
            If Not String.Equals(auxIconName, String.Empty) Then
                bsDelPatientsButton.Image = Image.FromFile(iconPath & auxIconName)
                bsDelCalibratorsButton.Image = Image.FromFile(iconPath & auxIconName)
                bsDelControlsButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'OFF SYSTEM TESTS RESULTS Button
            auxIconName = GetIconName("OFFSYSTEMBUT") 'auxIconName = GetIconName("TOFF_SYS")
            If Not String.Equals(auxIconName, String.Empty) Then
                bsOffSystemResultsButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'POSITIONING Button
            auxIconName = GetIconName("SENDTOPOS")
            If Not String.Equals(auxIconName, String.Empty) Then bsOpenRotorButton.Image = Image.FromFile(iconPath & auxIconName)

            'SCANNING SAMPLES Rotor
            auxIconName = GetIconName("BARCODE")
            If Not String.Equals(auxIconName, String.Empty) Then bsScanningButton.Image = Image.FromFile(iconPath & auxIconName)

            'BARCODE WARNINGS Button
            auxIconName = GetIconName("BCWARNING")
            If Not String.Equals(auxIconName, String.Empty) Then bsBarcodeWarningButton.Image = Image.FromFile(iconPath & auxIconName)

            'IMPORT FROM LIMS Button
            auxIconName = GetIconName("LIMSIMPORT")
            If Not String.Equals(auxIconName, String.Empty) Then bsLIMSImportButton.Image = Image.FromFile(iconPath & auxIconName)

            'SHOW LIMS ERRORS Button
            auxIconName = GetIconName("STUS_WITHERRS") ' "WARNING") dl 23/03/2012
            If Not String.Equals(auxIconName, String.Empty) Then bsLIMSErrorsButton.Image = Image.FromFile(iconPath & auxIconName)

            'If the LIS is working with ES, these buttons are not visible
            If (Not LISWithFilesMode) Then
                bsScanningButton.Visible = False
                bsLIMSImportButton.Visible = False
                bsBarcodeWarningButton.Visible = False
            End If

            'SAVE & EXIT Button
            auxIconName = GetIconName("ACCEPT1")
            If Not String.Equals(auxIconName, String.Empty) Then bsAcceptButton.Image = Image.FromFile(iconPath & auxIconName)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Update all local activity in the DB; add a new WorkSession or update the active one
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SG 30/07/2010 - If there are not items in the three grids, the WS is resetted
    '''              SA 30/07/2010 - After Reset WS, initialize WS properties in the Main MDI
    '''              SA 30/11/2010 - After create/update the WS, verify if some recalculations are needed 
    '''              SA 21/02/2011 - Changed from function returning a boolean value to SUB. Removed parameter pCreateOpenWS.
    '''                              These changes were due to this method has to be called in a different thread to allow 
    '''                              show a ProgressBar while it is in execution
    '''              RH 27/05/2011 - Make the Cursor set run in the Main Thread
    '''              TR 21/09/2011 - If there are not requested OrderTests but the status of the active WorkSession is EMPTY,
    '''                              verify if changes are needed in table of Incomplete Patient Samples
    '''              SA 22/09/2011 - If there are not requested OrderTests and the status of the active WorkSession is OPEN,
    '''                              update the WS Status to EMPTY instead reset it
    '''              SA 25/04/2013 - When all grids are empty and the current WS Status is EMPTY, call to function UpdateRelatedIncompletedSamples
    '''                              in BarcodePositionsWithNoRequestsDelegate has to be done only when LISWithFilesMode is TRUE
    '''              SA 21/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions. When value of global flag 
    '''                                           NEWAddWorkSession is TRUE, call new version of function AddWorkSession 
    ''' </remarks>
    Private Sub PrepareOrderTestsForWS()
        Dim myGlobalDataTO As GlobalDataTO
        Dim myWSDelegate As New WorkSessionsDelegate

        Try
            'If there are not items in the three grids, then the WS is resetted
            SetPropertyThreadSafe("Cursor", Cursors.WaitCursor)

            If (bsBlkCalibDataGridView.Rows.Count = 0 AndAlso bsControlOrdersDataGridView.Rows.Count = 0 AndAlso _
                bsPatientOrdersDataGridView.Rows.Count = 0) Then

                If (WorkSessionIDAttribute <> String.Empty) Then
                    If (WSStatusAttribute = "EMPTY") Then
                        If (LISWithFilesMode) Then
                            'Verify if changes are needed in table of Incomplete Patient Samples
                            Dim myBCPosWithNoRequestDelegate As New BarcodePositionsWithNoRequestsDelegate
                            myGlobalDataTO = myBCPosWithNoRequestDelegate.UpdateRelatedIncompletedSamples(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, _
                                                                                                          myWorkSessionResultDS, False)

                            If (myGlobalDataTO.HasError) Then
                                'Show the error message
                                SetPropertyThreadSafe("Cursor", Cursors.Default)
                                ErrorOnSavingWS = String.Format("{0}|{1}", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                            End If
                        End If

                    ElseIf (String.Equals(WSStatusAttribute, "OPEN")) Then
                        'Change the WS Status to EMPTY
                        myGlobalDataTO = myWSDelegate.ChangeWSStatusFromOpenToEmpty(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)

                        If (Not myGlobalDataTO.HasError) Then
                            'Update screen attributes and  global variables in the main MDI Form
                            WSStatusAttribute = "EMPTY"
                        Else
                            'Show the error message
                            SetPropertyThreadSafe("Cursor", Cursors.Default)
                            ErrorOnSavingWS = String.Format("{0}|{1}", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                        End If
                    Else
                        'The WS is reset: THIS CASE NEVER HAPPENS!!
                        myGlobalDataTO = myWSDelegate.ResetWSNEW(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)

                        If (Not myGlobalDataTO.HasError) Then
                            'Update screen attributes and  global variables in the main MDI Form
                            WorkSessionIDAttribute = ""
                            WSStatusAttribute = ""
                        Else
                            'Show the error message
                            SetPropertyThreadSafe("Cursor", Cursors.Default)
                            ErrorOnSavingWS = String.Format("{0}|{1}", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                        End If
                    End If
                End If
            Else
                'Save the Work Session 
                If (NEWAddWorkSession) Then
                    'BT #1545
                    myGlobalDataTO = myWSDelegate.PrepareOrderTestsForWS_NEW(myWorkSessionResultDS, WorkSessionIDAttribute, AnalyzerIDAttribute, CreateOpenWS, _
                                                                             WSStatusAttribute, ChangesMadeAttribute, LISWithFilesMode)
                Else
                    myGlobalDataTO = myWSDelegate.PrepareOrderTestsForWS(Nothing, myWorkSessionResultDS, CreateOpenWS, WorkSessionIDAttribute, _
                                                                         AnalyzerIDAttribute, WSStatusAttribute, ChangesMadeAttribute, LISWithFilesMode)
                End If

                'TR 28/06/2013 -Prepare and send results to LIS.
                If (Not myGlobalDataTO.HasError) Then
                    If myWSDelegate.LastExportedResults.twksWSExecutions.Rows.Count > 0 Then 'AG 21/02/2014 - #1505 call mdi threat only when needed
                        CreateLogActivity("Current Results automatic upload (OFFS)", Me.Name & ".PrepareOrderTestsForWS ", EventLogEntryType.Information, False) 'AG 02/01/2014 - BT #1433 (v211 patch2)
                        IAx00MainMDI.AddResultsIntoQueueToUpload(myWSDelegate.LastExportedResults)
                        IAx00MainMDI.InvokeUploadResultsLIS(False)
                    End If 'AG 21/02/2014 - #1505
                End If
                'TR 28/06/2013 -END.

                '//JVV 02/10/13
                If (Not myGlobalDataTO.HasError) Then
                    Dim printGlobalTo As New GlobalDataTO
                    Dim myautoreportdelg As New AutoReportsDelegate
                    Dim sType As String = String.Empty
                    printGlobalTo = myautoreportdelg.ManageAutoReportCreationOFFSys(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, sType)
                    If (Not printGlobalTo.HasError AndAlso Not printGlobalTo.SetDatos Is Nothing) Then
                        Dim oList As New List(Of String)
                        oList = DirectCast(printGlobalTo.SetDatos, List(Of String))
                        If oList.Count > 0 Then
                            If (String.Equals(sType, "COMPACT")) Then
                                XRManager.PrintCompactPatientsReport(AnalyzerIDAttribute, WorkSessionIDAttribute, oList, True)
                            ElseIf (String.Equals(sType, "INDIVIDUAL")) Then
                                XRManager.PrintPatientsFinalReport(AnalyzerIDAttribute, WorkSessionIDAttribute, oList)
                            End If
                        End If
                    End If
                End If
                '//JVV 02/10/13


                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim wsDS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)

                    If (wsDS.twksWorkSessions.Rows.Count = 1) Then
                        If (WorkSessionIDAttribute.Trim = "") Then WorkSessionIDAttribute = wsDS.twksWorkSessions(0).WorkSessionID
                        If (Not wsDS.twksWorkSessions(0).IsWorkSessionStatusNull) Then WSStatusAttribute = wsDS.twksWorkSessions(0).WorkSessionStatus

                        'Execute recalculations for changes in Calibrator Factor when needed
                        myGlobalDataTO = VerifyChangesInCalibrationFactor()
                        If (myGlobalDataTO.HasError) Then
                            'Inform the error message
                            SetPropertyThreadSafe("Cursor", Cursors.Default)
                            ErrorOnSavingWS = String.Format("{0}|{1}", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                        End If
                    End If
                Else
                    'Inform the error message
                    SetPropertyThreadSafe("Cursor", Cursors.Default)
                    ErrorOnSavingWS = String.Format("{0}|{1}", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                End If
            End If

        Catch ex As Exception
            SetPropertyThreadSafe("Cursor", Cursors.Default)
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareOrderTestsForWS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Name & ".PrepareOrderTestsForWS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.UIThread(Function() ShowMessage(Name & ".PrepareOrderTestsForWS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))"))
            'DL 15/05/2013
        Finally
            SavingWS = False
            ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it
            SetPropertyThreadSafe("Cursor", Cursors.Default)
        End Try
    End Sub

    ''' <summary>
    ''' Override the BaseForm function to allow the special management of field PatientID ENTER key should validate the 
    ''' informed Patient and open the auxiliary screen of Tests selection)
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR/SA 12/03/2012
    ''' </remarks>
    Protected Overrides Function ProcessDialogKey(ByVal keyData As System.Windows.Forms.Keys) As Boolean
        Try

            If (Me.ActiveControl IsNot Nothing) Then
                If (keyData = Keys.Return) Then
                    If (Me.ActiveControl Is bsPatientIDTextBox) Then
                        If (bsPatientIDTextBox.Text.Trim <> "") Then
                            PatientIDChangedInTextBox()

                            'If a SampleID/PatientID is informed, then verify if it corresponds to an existing Patient
                            Dim myGlobalDataTO As New GlobalDataTO
                            Dim myPatientDelegate As New PatientDelegate
                            myGlobalDataTO = myPatientDelegate.GetPatientData(Nothing, bsPatientIDTextBox.Text.Trim)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myPatientDS As PatientsDS = DirectCast(myGlobalDataTO.SetDatos, PatientsDS)
                                If (myPatientDS.tparPatients.Rows.Count > 0) Then
                                    'Patient found
                                    '...The PatientID is refreshed with the one in DB to show the exact case
                                    bsPatientIDTextBox.Text = myPatientDS.tparPatients(0).PatientID

                                    '... Patient names are show as ToolTip
                                    Dim sPatientName As String = ""
                                    sPatientName = String.Format("{0}, {1}", myPatientDS.tparPatients(0).LastName, myPatientDS.tparPatients(0).FirstName)
                                    bsScreenToolTips.SetToolTip(bsPatientIDTextBox, sPatientName)

                                    sampleIDType = "DB"
                                End If

                                'Initialize values in area of criteria selection
                                bsNumOrdersNumericUpDown.Value = 1
                                bsNumOrdersNumericUpDown.Enabled = False
                                bsNumOrdersNumericUpDown.BackColor = SystemColors.MenuBar

                                'Click in SearchTests button to open the auxiliary screen of Tests selection
                                bsSearchTests_Click(Nothing, Nothing)
                            Else
                                'Error validating the informed Patient ID - Initialize values in area of criteria selection
                                bsPatientIDTextBox.Text = String.Empty
                                SetFieldStatusForPatient(True)

                                'Show the error message
                                ShowMessage(Name & ".SearchPatient", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                        End If
                    ElseIf (Me.ActiveControl Is bsSampleClassComboBox OrElse Me.ActiveControl Is bsNumOrdersNumericUpDown OrElse Me.ActiveControl Is bsSampleTypeComboBox) Then
                        'For the rest of controls (excepting buttons), ENTER key works as TAB key 
                        SendKeys.Send("{Tab}")
                        Return True
                    End If
                End If
            End If
            Return MyBase.ProcessDialogKey(keyData)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ProcessDialogKey", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ProcessDialogKey", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Function

    ''' <summary>
    ''' Refresh all the DataGridViews
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 03/03/2010 
    ''' </remarks>
    Private Sub RefreshGrids()
        Try
            bsPatientOrdersDataGridView.Refresh()
            bsBlkCalibDataGridView.Refresh()
            bsControlOrdersDataGridView.Refresh()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RefreshGrids", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RefreshGrids", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Release elements not handle by the Garbage Collector
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 04/08/2011
    ''' Modified by: TR 01/08/2012 - Added more elements to release
    ''' AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called
    ''' </remarks>
    Private Sub ReleaseElements()
        Try
            isClosingFlag = True 'AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called

            'LOAD SAVED WS Button
            bsLoadWSButton.Image = Nothing
            'SAVE WS Button
            bsSaveWSButton.Image = Nothing
            'DELETE Buttons (Patient Samples, Controls, Blanks&Calibrators)
            bsDelPatientsButton.Image = Nothing
            bsDelCalibratorsButton.Image = Nothing
            bsDelControlsButton.Image = Nothing
            'OFF SYSTEM TESTS RESULTS Button
            bsOffSystemResultsButton.Image = Nothing
            'POSITIONING Button
            bsOpenRotorButton.Image = Nothing
            'SCANNING SAMPLES Rotor
            bsScanningButton.Image = Nothing
            'BARCODE WARNINGS Button
            bsBarcodeWarningButton.Image = Nothing
            'IMPORT FROM LIMS Button
            bsLIMSImportButton.Image = Nothing
            'SHOW LIMS ERRORS Button
            bsLIMSErrorsButton.Image = Nothing
            'SAVE & EXIT Button
            bsAcceptButton.Image = Nothing

            'Global variable for all Blank, Calibrator, Control and Patient Order Tests shown in the different grids
            myWorkSessionResultDS = Nothing
            myImportErrorsLogDS = Nothing

            'Global variables to store the icon image byte arrays for Stat and Routine
            myIconStatFlag = Nothing
            myIconNonStatFlag = Nothing
            myFactorTextBox = Nothing


            'mdiAnalyzerCopy = Nothing 'not this variable

            '--- Detach variable defined using WithEvents ---
            bsOrderDetailsGroupBox = Nothing
            bsNumOrdersNumericUpDown = Nothing
            bsNumOrdersLabel = Nothing
            bsStatCheckbox = Nothing
            bsSampleClassComboBox = Nothing
            bsSampleClassLabel = Nothing
            bsPatientSearchButton = Nothing
            bsPatientIDTextBox = Nothing
            bsPatientIDLabel = Nothing
            bsSampleTypeLabel = Nothing
            bsSampleTypeComboBox = Nothing
            bsSearchTestsButton = Nothing
            bsOpenRotorButton = Nothing
            bsSampleClassesTabControl = Nothing
            PatientsTab = Nothing
            OtherSamplesTab = Nothing
            bsSaveWSButton = Nothing
            bsLoadWSButton = Nothing
            bsPrepareWSLabel = Nothing
            bsAcceptButton = Nothing
            bsDelPatientsButton = Nothing
            bsDelControlsButton = Nothing
            bsDelCalibratorsButton = Nothing
            bsLIMSImportButton = Nothing
            bsLIMSErrorsButton = Nothing
            bsScreenToolTips = Nothing
            bsAllPatientsCheckBox = Nothing
            bsAllBlanksCheckBox = Nothing
            bsAllCalibsCheckBox = Nothing
            bsAllCtrlsCheckBox = Nothing
            bsBlkCalibDataGridView = Nothing
            bsControlOrdersDataGridView = Nothing
            bsPatientOrdersDataGridView = Nothing
            bsOffSystemResultsButton = Nothing
            bsScanningButton = Nothing
            bsBarcodeWarningButton = Nothing
            bsErrorProvider1 = Nothing
            '------------------------------------------------

            'GC.Collect()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if the CheckBox for select/unselect all Blanks and Calibrators have to be checked or unchecked
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 09/03/2010
    '''              XB 26/08/2014 - Remove parameter and the block of code executed when parameter was not Nothing (due to the function is called just once using Nothing) - BT #1868
    '''                              Get value of cell SampleClass for the current row and divide code according the SampleClass - BT #1868
    ''' </remarks>
    Private Sub RowBlkCalCheckBoxClick()
        Try
            ChangesMadeAttribute = True

            ' XB 26/08/2014 - BT #1868
            'If (Not pRowCheckBox Is Nothing) Then
            '    'Modify the global counter
            '    If (CBool(pRowCheckBox.Value) AndAlso totalBlkCalCheckedCheckBoxes < totalBlkCalCheckBoxes) Then
            '        totalBlkCalCheckedCheckBoxes += 1

            '    ElseIf (totalBlkCalCheckedCheckBoxes > 0) Then
            '        totalBlkCalCheckedCheckBoxes -= 1
            '    End If

            '    'Change state of the header CheckBox...
            '    If (totalBlkCalCheckedCheckBoxes < totalBlkCalCheckBoxes) Then
            '        bsAllBlanksCheckBox.Checked = False

            '    ElseIf (totalBlkCalCheckedCheckBoxes = totalBlkCalCheckBoxes) Then
            '        bsAllBlanksCheckBox.Checked = True
            '    End If
            'Else
            ''Count the number of selected rows in grid of Blanks and Calibrators
            'Dim lstWSSelectedRowsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
            'lstWSSelectedRowsDS = (From a In myWorkSessionResultDS.BlankCalibrators _
            '                      Where a.Selected = True _
            '                     Select a).ToList()


            ''If all rows are selected, then the CheckBox for select/unselect all Blanks and Calibrators is checked
            'bsAllBlanksCheckBox.Checked = (lstWSSelectedRowsDS.Count = bsBlkCalibDataGridView.Rows.Count)
            'lstWSSelectedRowsDS = Nothing
            'End If

            CheckAddRemoveBlkCalRows()
            ' XB 26/08/2014 - BT #1868

            'Verify if button for Positioning should be enabled
            OpenRotorButtonEnabled()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RowBlkCalCheckBoxClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RowBlkCalCheckBoxClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if the CheckBox for select/unselect all Controls have to be checked or unchecked
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 09/03/2010
    ''' </remarks>
    Private Sub RowControlCheckBoxClick(ByVal pRowCheckBox As DataGridViewCheckBoxCell)
        Try
            ChangesMadeAttribute = True
            If (Not pRowCheckBox Is Nothing) Then
                'Modify the global counter
                If (CBool(pRowCheckBox.Value) AndAlso totalControlCheckedCheckBoxes < totalControlCheckBoxes) Then
                    totalControlCheckedCheckBoxes += 1

                ElseIf (totalControlCheckedCheckBoxes > 0) Then
                    totalControlCheckedCheckBoxes -= 1
                End If

                'Change state of the header CheckBox...
                If (totalControlCheckedCheckBoxes < totalControlCheckBoxes) Then
                    bsAllCtrlsCheckBox.Checked = False

                ElseIf (totalControlCheckedCheckBoxes = totalControlCheckBoxes) Then
                    bsAllCtrlsCheckBox.Checked = True
                End If
            Else
                'Count the number of selected rows in grid of Controls
                Dim lstWSSelectedRowsDS As List(Of WorkSessionResultDS.ControlsRow)
                lstWSSelectedRowsDS = (From a In myWorkSessionResultDS.Controls _
                                      Where a.Selected = True _
                                     Select a).ToList()

                'If all rows are selected, then the CheckBox for select/unselect all Controls is checked
                bsAllCtrlsCheckBox.Checked = (lstWSSelectedRowsDS.Count = bsControlOrdersDataGridView.Rows.Count)
                lstWSSelectedRowsDS = Nothing
            End If

            'Verify if button for Positioning should be enabled
            OpenRotorButtonEnabled()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RowControlCheckBoxClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RowControlCheckBoxClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if the CheckBox for select/unselect all Patients have to be checked or unchecked
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 09/03/2010
    ''' </remarks>
    Private Sub RowPatientCheckBoxClick(ByVal pRowCheckBox As DataGridViewCheckBoxCell)
        Try
            ChangesMadeAttribute = True
            If (Not pRowCheckBox Is Nothing) Then
                'Modify the global counter
                If (CBool(pRowCheckBox.Value) AndAlso totalPatientCheckedCheckBoxes < totalPatientCheckBoxes) Then
                    totalPatientCheckedCheckBoxes += 1

                ElseIf (totalPatientCheckedCheckBoxes > 0) Then
                    totalPatientCheckedCheckBoxes -= 1
                End If

                'Change state of the header CheckBox...
                If (totalPatientCheckedCheckBoxes < totalPatientCheckBoxes) Then
                    bsAllPatientsCheckBox.Checked = False

                ElseIf (totalPatientCheckedCheckBoxes = totalPatientCheckBoxes) Then
                    bsAllPatientsCheckBox.Checked = True
                End If
            Else
                'Count the number of selected rows in grid of Patients
                Dim lstWSSelectedRowsDS As List(Of WorkSessionResultDS.PatientsRow)
                lstWSSelectedRowsDS = (From a In myWorkSessionResultDS.Patients _
                                      Where a.Selected = True _
                                    AndAlso a.OTStatus = "OPEN" _
                                     Select a).ToList()

                'Count the number of OPEN Patient OrderTests (excepting Off-System ones) in the grid of Patients
                Dim lstWSOpenRowsDS As List(Of WorkSessionResultDS.PatientsRow)
                lstWSOpenRowsDS = (From a In myWorkSessionResultDS.Patients _
                                  Where a.OTStatus = "OPEN" _
                                AndAlso a.TestType <> "OFFS" _
                                 Select a).ToList()

                'If all rows are selected, then the CheckBox for select/unselect all Patients is checked
                bsAllPatientsCheckBox.Checked = (lstWSSelectedRowsDS.Count = lstWSOpenRowsDS.Count)

                lstWSOpenRowsDS = Nothing
                lstWSSelectedRowsDS = Nothing
            End If

            'Verify if button for Positioning should be enabled
            OpenRotorButtonEnabled()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RowPatientCheckBoxClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RowPatientCheckBoxClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization after a new SampleClass is selected 
    ''' For event bsSampleClassComboBox_SelectionChangeCommitted
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 09/03/2010
    '''              SA 16/09/2010 - Do not set selected SampleType to the first one when selected SampleClass is changed
    ''' </remarks>
    Private Sub SampleClassSelectionChange()
        Try
            If (bsSampleClassComboBox.SelectedIndex > -1) Then
                Dim patientCtrlsEnabled As Boolean = (bsSampleClassComboBox.SelectedValue.ToString = "PATIENT" AndAlso _
                                                      WSStatusAttribute <> "ABORTED")

                bsStatCheckbox.Checked = False
                bsStatCheckbox.Enabled = patientCtrlsEnabled

                bsPatientIDTextBox.Text = String.Empty
                bsPatientIDTextBox.Enabled = patientCtrlsEnabled
                bsPatientIDTextBox.ReadOnly = False

                bsNumOrdersNumericUpDown.Value = 1
                If (patientCtrlsEnabled) Then
                    bsPatientIDTextBox.BackColor = Color.White

                    bsNumOrdersNumericUpDown.Enabled = True
                    bsNumOrdersNumericUpDown.BackColor = Color.White
                Else
                    bsPatientIDTextBox.BackColor = SystemColors.MenuBar

                    bsNumOrdersNumericUpDown.Enabled = False
                    bsNumOrdersNumericUpDown.BackColor = SystemColors.MenuBar
                End If

                bsScreenToolTips.SetToolTip(bsPatientIDTextBox, "")
                bsPatientSearchButton.Enabled = patientCtrlsEnabled

                If (bsSampleClassComboBox.SelectedValue.ToString = "BLANK") Then
                    bsSampleTypeComboBox.SelectedIndex = -1
                    bsSampleTypeComboBox.Enabled = False
                    bsSampleTypeComboBox.BackColor = SystemColors.MenuBar

                Else
                    If (WSStatusAttribute <> "ABORTED") Then
                        If (bsSampleTypeComboBox.SelectedIndex = -1) Then bsSampleTypeComboBox.SelectedIndex = 0

                        bsSampleTypeComboBox.Enabled = True
                        bsSampleTypeComboBox.BackColor = Color.White

                        bsPatientIDTextBox.Focus()
                    End If
                End If

                If (bsSampleClassComboBox.SelectedValue.ToString = "PATIENT") Then
                    patientEventSource = "Change_SampleClass"
                    bsPatientOrdersDataGridView.ClearSelection()

                    bsSampleClassesTabControl.SelectedTabPage = PatientsTab
                Else
                    blkCalibEventSource = "Change_SampleClass"
                    controlEventSource = "Change_SampleClass"

                    bsBlkCalibDataGridView.ClearSelection()
                    bsControlOrdersDataGridView.ClearSelection()

                    bsSampleClassesTabControl.SelectedTabPage = OtherSamplesTab
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SampleClassSelectionChange", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SampleClassSelectionChange", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if buttons Save WorkSession and Load WorkSession can be enabled
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 22/09/2011 - Enable Load button also when WSStatus=EMPTY
    ''' Modified by: DL 07/05/2013 - Disable Load button  when count (Patienst.listRequest = true) + count (Controls.listRequest = true) > 0
    ''' </remarks>
    Private Sub SaveLoadWSButtonsEnabled()
        Try
            bsSaveWSButton.Enabled = (myWorkSessionResultDS.Patients.Rows.Count > 0 OrElse _
                                      myWorkSessionResultDS.Controls.Rows.Count > 0 OrElse _
                                      myWorkSessionResultDS.BlankCalibrators.Rows.Count > 0)

            'DL 07/05/2013. Begin
            Dim countLISRequestPatients As Integer = 0
            Dim countLISRequestControls As Integer = 0

            If myWorkSessionResultDS.Patients.Rows.Count > 0 Then
                countLISRequestPatients = (From row In myWorkSessionResultDS.Patients Where row.LISRequest = True Select row).Count
            End If

            If myWorkSessionResultDS.Controls.Rows.Count > 0 Then
                countLISRequestControls = (From row In myWorkSessionResultDS.Controls Where row.LISRequest = True Select row).Count
            End If

            'bsLoadWSButton.Enabled = (WorkSessionIDAttribute = "") OrElse (WSStatusAttribute = "EMPTY") OrElse (WSStatusAttribute = "OPEN")
            bsLoadWSButton.Enabled = ((WorkSessionIDAttribute = "") OrElse (WSStatusAttribute = "EMPTY") OrElse (WSStatusAttribute = "OPEN")) AndAlso ((countLISRequestControls + countLISRequestPatients) = 0)

            'DL 07/05/2013. End

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveLoadWSButtonsEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveLoadWSButtonsEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow create/update a Saved WorkSession, adding to it the list of Order Tests
    ''' that are currently in the different grids. For event bsSaveWSButton_Click
    ''' </summary>
    ''' <remarks>
    ''' Created by:  GDS 07/04/2010
    ''' Modified by: SA  07/04/2010 - Code moved from the button event to this function
    '''              RH  19/10/2010 - Introduced the Using statement               
    '''              DL  27/07/2011 - If a WS was saved, enable button LoadSavedWS
    '''              SA  08/03/2012 - Button LoadSavedWS cannot be always enabled; it is enabled only when WSStatus is EMPTY or OPEN
    ''' </remarks>
    Private Sub SaveWorkSession()
        Try
            Dim mySavedWS As New SavedWSDelegate
            Dim myGlobalDataTo As New GlobalDataTO

            'The same Form used to Save Virtual Rotors is reused to manage the saving of WorkSessions
            Using myWSSelection As New IWSLoadSaveAuxScreen
                'Assign the required properties of the auxiliary screen and open it as a DialogForm
                myWSSelection.ScreenUse = "SAVEDWS"
                myWSSelection.SourceButton = "SAVE"
                myWSSelection.NameProperty = WSLoadedNameAttribute.Trim

                If (myWSSelection.ShowDialog() = Windows.Forms.DialogResult.OK) Then
                    myGlobalDataTo = mySavedWS.Save(Nothing, myWSSelection.NameProperty, myWorkSessionResultDS, myWSSelection.IDProperty)

                    If (Not myGlobalDataTo.HasError) Then
                        bsLoadWSButton.Enabled = (WSStatusAttribute = "EMPTY" OrElse WSStatusAttribute = "OPEN")
                    Else
                        ShowMessage(Name & ".SaveWorkSession", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage, Me)
                    End If
                End If
            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveWorkSession", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveWorkSession", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Create a new WorkSession or update the active one but without send Order Tests to 
    ''' positioning in the Analyzer 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 21/02/2011 - Added code to show the ProgressBar while the WS is being saved
    '''              TR 03/08/2011 - Called sub EnableMenusBar to disable the MainMenu while the saving process is executing and enable 
    '''                              it again once the saving finishes
    '''              TR 05/08/2011 - Set to nothing all Lists once they have been used to release memory
    '''              TR 04/10/2011 - Changed call to sub EnableMenusBar for a call to a new button EnableButtonAndMenus
    '''              TR 03/04/2012 - Validade if is reseting the current worksession.
    '''              XB 28/05/2013 - Threads are causing malfunctions on datagridview events so are deleted and related functionalities are synchronous called (bugstrankig #544 and #1102)
    ''' </remarks>
    Private Sub SaveWSWithoutPositioning()
        Try
            'TR 11/04/2012 - Disable form on close to avoid clicking in any screen button 
            Me.Enabled = False

            'Validate if not WS Reset
            If (Not isWSResetAttribute) Then
                RowPostPaintEnabled = False 'RH 08/05/2012 Bugtracking 544

                Cursor = Cursors.WaitCursor
                IAx00MainMDI.EnableButtonAndMenus(False)

                Dim openOrderTests As Boolean = True
                If (Not ChangesMadeAttribute) Then
                    openOrderTests = False
                Else
                    If (WorkSessionIDAttribute.Trim = "" OrElse ActiveWSStatus = "EMPTY") Then
                        'Verify if there are Open Order Tests (whatever SampleClass) 
                        '*** Verify Open Patient Order Tests
                        Dim lstWSPatients As List(Of WorkSessionResultDS.PatientsRow)
                        lstWSPatients = (From a In myWorkSessionResultDS.Patients _
                                        Where a.SampleClass = "PATIENT" _
                                      AndAlso a.OTStatus = "OPEN" _
                                       Select a).ToList()
                        openOrderTests = (lstWSPatients.Count > 0)

                        If (Not openOrderTests) Then
                            '*** Verify Open Control Order Tests
                            Dim lstWSControls As List(Of WorkSessionResultDS.ControlsRow)
                            lstWSControls = (From a In myWorkSessionResultDS.Controls _
                                            Where a.SampleClass = "CTRL" _
                                          AndAlso a.OTStatus = "OPEN" _
                                           Select a).ToList()
                            openOrderTests = (lstWSControls.Count > 0)
                            lstWSControls = Nothing
                        End If

                        If (Not openOrderTests) Then
                            '*** Verify Open Blank and/or Calibrator Order Tests
                            Dim lstWSBlkCalibs As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                            lstWSBlkCalibs = (From a In myWorkSessionResultDS.BlankCalibrators _
                                             Where a.OTStatus = "OPEN" _
                                            Select a).ToList()
                            openOrderTests = (lstWSBlkCalibs.Count > 0)
                            lstWSBlkCalibs = Nothing
                        End If
                        lstWSPatients = Nothing
                    End If
                End If

                If (openOrderTests) Then
                    'IAx00MainMDI.InitializeMarqueeProgreesBar()  ' XB 28/05/2013
                    Application.DoEvents()

                    'Deactivate the Screen Controls
                    ChangeControlsStatusByProgressBar(False)

                    'Save the WorkSession
                    SavingWS = True
                    ScreenWorkingProcess = True 'AG 08/11/2012 - inform this flag because the MDI requires it
                    CreateOpenWS = True

                    ' XB 28/05/2013
                    'Dim workingThread As New Threading.Thread(AddressOf PrepareOrderTestsForWS)
                    'workingThread.Start()

                    'While SavingWS
                    '    IAx00MainMDI.InitializeMarqueeProgreesBar()
                    '    Application.DoEvents()
                    'End While

                    'workingThread = Nothing
                    'IAx00MainMDI.StopMarqueeProgressBar()
                    Cursor = Cursors.WaitCursor
                    Application.DoEvents()
                    PrepareOrderTestsForWS()
                    ' XB 28/05/2013

                    If (ErrorOnSavingWS = String.Empty) Then
                        'Update global variables in the main MDI Form
                        IAx00MainMDI.SetWSActiveData(AnalyzerIDAttribute, WorkSessionIDAttribute, WSStatusAttribute)
                        If (Not Tag Is Nothing) Then
                            'A PerformClick() method was executed
                            Close()
                        Else
                            ' XB 27/11/2013 - Inform to MDI that this screen is closing aims to open next screen - Task #1303
                            ExitingScreen()
                            IAx00MainMDI.EnableButtonAndMenus(True)
                            Application.DoEvents()

                            'Normal button click
                            'Open the WS Monitor form and close this one
                            IAx00MainMDI.OpenMonitorForm(Me)
                        End If
                        'GC.Collect()
                    Else
                        'In case of Error, set Enabled=True for all controls that can be activated
                        ChangeControlsStatusByProgressBar(True)

                        IAx00MainMDI.StopMarqueeProgressBar()
                        Application.DoEvents()

                        Dim myErrorData As String() = ErrorOnSavingWS.Split(CChar("|"))
                        ErrorOnSavingWS = String.Empty 'Reset the value after using it
                        Cursor = Cursors.Default
                        ShowMessage(Name & ".SaveWSWithoutPositioning", myErrorData(0), myErrorData(1), Me)
                    End If
                Else
                    IAx00MainMDI.StopMarqueeProgressBar()

                    If (Not Tag Is Nothing) Then
                        'A PerformClick() method was executed
                        Close()
                    Else
                        ' XB 27/11/2013 - Inform to MDI that this screen is closing aims to open next screen - Task #1303
                        ExitingScreen()
                        IAx00MainMDI.EnableButtonAndMenus(True)
                        Application.DoEvents()

                        'Normal button click
                        'Open the WS Monitor form and close this one
                        IAx00MainMDI.OpenMonitorForm(Me)
                    End If
                End If
                IAx00MainMDI.SetStatusRotorPosOptions(True)
            Else
                Close()
            End If

        Catch ex As Exception
            IAx00MainMDI.StopMarqueeProgressBar()
            Application.DoEvents()

            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveWSWithoutPositioning", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)

            'DL 15/05/2013
            'ShowMessage(Name & ".SaveWSWithoutPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.UIThread(Function() ShowMessage(Name & ".SaveWSWithoutPositioning", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))"))
            'DL 15/05/2013
        Finally
            Cursor = Cursors.Default
            IAx00MainMDI.EnableButtonAndMenus(True)
        End Try
    End Sub

    ''' <summary>
    ''' Start the scanning process of Samples Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:
    ''' Modified by: AG 12/07/2011 - Disable all vertical actions ButtonBar and initialize Barcode read with NO running involved
    ''' </remarks>
    Private Sub ScanningBarCode()
        Try
            Dim resultdata As New GlobalDataTO
            Dim BarCodeDS As New AnalyzerManagerDS
            Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow

            rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
            With rowBarCode
                .RotorType = "SAMPLES"
                .Action = GlobalEnumerates.Ax00CodeBarAction.FULL_ROTOR
                .Position = 0
            End With
            BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
            BarCodeDS.AcceptChanges()

            'Send the Barcode Scanning instruction
            If (Not mdiAnalyzerCopy Is Nothing) Then
                ScreenWorkingProcess = True

                mdiAnalyzerCopy.BarCodeProcessBeforeRunning = AnalyzerManager.BarcodeWorksessionActions.NO_RUNNING_REQUEST
                resultdata = mdiAnalyzerCopy.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS, "")
                If (resultdata.HasError OrElse Not mdiAnalyzerCopy.Connected) Then
                    ScreenWorkingProcess = False
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScanningBarCode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage(Me.Name & ".ScanningBarCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.UIThread(Function() ShowMessage(Name & ".ScanningBarCode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))"))
            'DL 15/05/2013
            ScreenWorkingProcess = False
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL  03/03/2010 - Added binding source for Controls grid
    '''              SA  08/03/2010 - Added binding source for Patients grid
    '''              GDS 12/04/2010 - Bind code was placed in a separated routine (BindDSToGrids)
    '''              TR  05/05/2010 - Call function PrepareButtons to load images for all graphical buttons
    '''              RH  27/05/2010 - CurrentAnalyzer initialization
    '''              PG  13/10/2010 - Get the current language
    '''              SA  22/02/2011 - Loading of a selected SavedWS (when WSLoadedIDAttribute is informed) moved to event 
    '''                               Form_Shown to allow showing the ProgressBar while the loading is in execution
    '''              AG  16/06/2011 - Added code to get the same AnalyzerManager used in the MDI
    '''              SA  16/09/2011 - Remove code for enabling/disabling the LIMS Import button due to this button has been deleted from the screen
    '''              DL  01/12/2011 - Set focus to field PatientID
    '''              SA  12/03/2012 - Added call to new sub ValidateScanningButtonEnabled (code in that function was included before in function 
    '''                               PrepareButtons but has been moved to a new sub). When Status of the current WS is ABORTED, call function 
    '''                               DisableFieldsForAbortedWS to disable all fields in bsOrderDetailsGroupBox (excepting the SampleClass ComboBox)
    '''              SA  28/08/2012 - Inform parameter AnalyzerID when calling function GetOrderTestsForWS in OrderTestsDelegate
    ''' </remarks>
    Private Sub ScreenLoad()
        Dim myGlobalDataTO As GlobalDataTO
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

            If (Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
            End If

            'SA-TR 30/05/2012 - Get the WorkSession status
            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate
            myGlobalDataTO = myWSAnalyzersDelegate.GetByWorkSession(Nothing, WorkSessionIDAttribute)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                WSStatusAttribute = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS).twksWSAnalyzers(0).WSStatus
            End If


            If (Not myGlobalDataTO.HasError) Then
                'TR 05/04/2013 - Hide buttons depending on the parameter value LIS_WithFiles_Mode.
                Dim myUserSettingsDelegate As New UserSettingsDelegate
                myGlobalDataTO = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WITHFILES_MODE.ToString())

                If (Not myGlobalDataTO.HasError) Then
                    'Set the value to my local variable
                    LISWithFilesMode = CBool(myGlobalDataTO.SetDatos)
                End If
            End If

            'Prepare all buttons
            PrepareButtons()
            IAx00MainMDI.bsTSInfoButton.Enabled = True

            'Disable the context menu shown with mouse right-button click in all editable fields
            Dim emptyContextMenu As New ContextMenuStrip
            bsPatientIDTextBox.ContextMenuStrip = emptyContextMenu
            bsNumOrdersNumericUpDown.ContextMenuStrip = emptyContextMenu

            'Get multilanguage labels for all screen controls
            GetScreenLabels(currentLanguage)

            'Load ComboBoxes of Sample Types and Sample Classes
            InitializeSampleTypeComboBox()
            InitializeSampleClassComboBox()

            'Get values for the NumericUpDown control used to set the num of Orders
            LoadNumOrdersLimits()

            'Get the list of Sample Tube Types (for the ComboBox column of TubeType in all the grids)
            myGlobalDataTO = GetSampleTubeTypes()
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim lstSorted As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
                lstSorted = DirectCast(myGlobalDataTO.SetDatos, List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow))

                InitializePatientGrid(lstSorted, currentLanguage)
                InitializeControlGrid(lstSorted, currentLanguage)
                InitializeBlkCalibGrid(lstSorted, currentLanguage)
                lstSorted = Nothing
            Else
                'Show the error Message
                ShowMessage(Name & ".ScreenLoad", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

            Application.DoEvents()
            If (Not myGlobalDataTO.HasError) Then
                'Get value of General Setting containing the maximum number of Patient Order Tests that can be created
                Dim myGeneralSettingsDelegate As New GeneralSettingsDelegate
                myGlobalDataTO = myGeneralSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.MAX_PATIENT_ORDER_TESTS.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    'Save value in global variable maxPatientOrderTests
                    maxPatientOrderTests = CType(myGlobalDataTO.SetDatos, Integer)
                End If
            End If

            If (Not myGlobalDataTO.HasError) Then
                'If a WorkSession Identifier was informed, get the list of Order Tests currently linked to it
                '(both, OPEN and IN PROCESS ones of whatever Sample Class)
                If Not String.Equals(WorkSessionIDAttribute.Trim, String.Empty) Then
                    Dim myWSDelegate As New WorkSessionsDelegate

                    myGlobalDataTO = myWSDelegate.GetOrderTestsForWS(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myWorkSessionResultDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)
                    Else
                        'Error getting the list of previously requested Order Tests
                        ShowMessage(Name & ".ScreenLoad", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                End If

                'Bind DS to the form grids
                BindDSToGrids()

                'Apply styles to special columns
                ApplyStylesToSpecialGridColumns()

                'If there are Patients and/or Controls requested by LIS and unselected, unselect also the related 
                'Controls, Calibrators and Blanks when it is possible
                UnselectLISOrderTests()
            End If

            patientEventSource = "Load"
            controlEventSource = "Load"
            blkCalibEventSource = "Load"

            'When loading the screen any row should be selected in any DataGridView
            bsPatientOrdersDataGridView.ClearSelection()
            bsControlOrdersDataGridView.ClearSelection()
            bsBlkCalibDataGridView.ClearSelection()

            Application.DoEvents()

            'Get the Icons for Stat and Routine
            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
            myIconStatFlag = preloadedDataConfig.GetIconImage("STATS")
            myIconNonStatFlag = preloadedDataConfig.GetIconImage("ROUTINES")

            'Verify availability of screen buttons
            OpenRotorButtonEnabled()
            SaveLoadWSButtonsEnabled()
            OffSystemResultsButtonEnabled()

            'Verify if there are errors to shown
            LIMSErrorsButtonEnabled()

            'Verify if the Scanning Button and the button for the auxiliary screen of incomplete Patient Samples have to be enabled
            ValidateScanningButtonEnabled()
            ValidateBCWarningButtonEnabled()

            'Disable all fields in bsOrderDetailsGroupBox if the status of the WS is ABORTED
            If (WSStatusAttribute = "ABORTED") Then
                DisableFieldsForAbortedWS()
                bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
            End If
            ResetBorder()
            If (bsPatientIDTextBox.Enabled) Then bsPatientIDTextBox.Focus()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if the informed PatientID corresponds to an existing Patient, and in that case, show
    ''' the Patient names in the ToolTip - For event bsPatientSearchButton_Click
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 12/01/2010 - Before opening the Patients Search Screen, get the list of Patients saved in DB  from which
    '''                              there are requested Order Tests with status OPEN and inform screen property PatientsList
    ''' </remarks>
    Private Sub SearchPatient()
        Try
            Dim patientExits As Boolean = False
            Dim myGlobalDataTO As New GlobalDataTO

            If (bsPatientIDTextBox.Text.Trim <> "") Then
                'If a SampleID/PatientID is informed, then verify if it corresponds to an existing Patient
                Dim myPatientDelegate As New PatientDelegate
                myGlobalDataTO = myPatientDelegate.GetPatientData(Nothing, bsPatientIDTextBox.Text.Trim)

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myPatientDS As PatientsDS = DirectCast(myGlobalDataTO.SetDatos, PatientsDS)

                    If (myPatientDS.tparPatients.Rows.Count > 0) Then
                        patientExits = True

                        'Patient found
                        '...The PatientID is refreshed with the one in DB to show the exact case
                        bsPatientIDTextBox.Text = myPatientDS.tparPatients(0).PatientID

                        '... Patient names are show as ToolTip
                        Dim sPatientName As String = ""
                        sPatientName = String.Format("{0}, {1}", myPatientDS.tparPatients(0).LastName, myPatientDS.tparPatients(0).FirstName)
                        bsScreenToolTips.SetToolTip(bsPatientIDTextBox, sPatientName)

                        sampleIDType = "DB"

                        'Initialize values in area of criteria selection
                        bsNumOrdersNumericUpDown.Value = 1
                        bsNumOrdersNumericUpDown.Enabled = False
                        bsNumOrdersNumericUpDown.BackColor = SystemColors.MenuBar
                    End If
                Else
                    'Error validating the informed Patient ID - Initialize values in area of criteria selection
                    bsPatientIDTextBox.Text = String.Empty
                    SetFieldStatusForPatient(True)

                    'Show the error message
                    ShowMessage(Name & ".SearchPatient", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            End If

            If (Not myGlobalDataTO.HasError AndAlso Not patientExits) Then
                'Get the list of Patients existing in DB with requested Order Tests with status OPEN
                '(to control the InUse status of Patients when the Order Tests have not been saved)
                Dim lstPatientExist As List(Of String)
                lstPatientExist = (From a In myWorkSessionResultDS.Patients _
                                  Where a.SampleIDType = "DB" _
                                AndAlso a.OTStatus = "OPEN" _
                                 Select a.SampleID Distinct).ToList()

                'Open screen of Patient's Search passing to it the informed PatientID 
                'RH 19/10/2010 Introduce the Using statement
                Using myPatientSearchForm As New IProgPatientData()
                    myPatientSearchForm.EntryMode = "SEARCH"
                    myPatientSearchForm.PatientID = bsPatientIDTextBox.Text
                    myPatientSearchForm.PatientsList = lstPatientExist
                    myPatientSearchForm.StartPosition = FormStartPosition.CenterParent
                    myPatientSearchForm.ShowDialog()

                    If (myPatientSearchForm.PatientNames.Trim <> "") Then
                        bsPatientIDTextBox.Text = myPatientSearchForm.PatientID
                        bsScreenToolTips.SetToolTip(bsPatientIDTextBox, myPatientSearchForm.PatientNames)

                        sampleIDType = "DB"

                        'Initialize values in area of criteria selection
                        bsNumOrdersNumericUpDown.Value = 1
                        bsNumOrdersNumericUpDown.Enabled = False
                        bsNumOrdersNumericUpDown.BackColor = SystemColors.MenuBar
                    Else
                        sampleIDType = "MAN"

                        'Initialize values in area of criteria selection
                        SetFieldStatusForPatient(True)
                    End If
                End Using
                lstPatientExist = Nothing 'TR 05/08/2011 - Set value to nothing to release memory
            End If

            'If button for Search Patient was clicked, then the Patient was validated
            validatedSampleID = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SearchPatient", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SearchPatient", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow select/unselect Tests for an specific Sample Class
    ''' For event bsSearchTests_Click
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SearchTests()
        Try
            Select Case bsSampleClassComboBox.SelectedValue.ToString
                Case "BLANK"
                    SearchTestsForBlanks()

                    blkCalibEventSource = "Search_Tests"
                    bsBlkCalibDataGridView.ClearSelection()

                Case "CALIB"
                    SearchTestsForCalibrators()

                    blkCalibEventSource = "Search_Tests"
                    bsBlkCalibDataGridView.ClearSelection()

                Case "CTRL"
                    SearchTestsForControls()

                    controlEventSource = "Search_Tests"
                    bsControlOrdersDataGridView.ClearSelection()

                Case "PATIENT"
                    SearchTestsForPatientSamples()

                    patientEventSource = "Search_Tests"
                    bsPatientOrdersDataGridView.ClearSelection()
            End Select

            'Verify availability of screen buttons
            OpenRotorButtonEnabled()
            SaveLoadWSButtonsEnabled()
            'LISImportButtonEnabled()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SearchTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SearchTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow select/unselect Tests when the selected SampleClass is BLANK
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 19/10/2010 - Introduced the Using statement
    ''' </remarks>
    Private Sub SearchTestsForBlanks()
        Try
            'Get the list of Blank Order Tests currently in the grid of Blanks&Calibrators 
            Dim lstWSBlanksDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
            lstWSBlanksDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                             Where a.SampleClass = "BLANK" _
                             Select a).ToList()

            'Load the list of Blank Order Tests currently loaded in the grid of Blanks&Calibrators
            'in a the DataSet of Selected Tests needed for the auxiliary screen of Tests Selection 
            Dim currentTestsDS As New SelectedTestsDS
            Dim newTestRow As SelectedTestsDS.SelectedTestTableRow

            For Each blankOrderTest As WorkSessionResultDS.BlankCalibratorsRow In lstWSBlanksDS
                newTestRow = currentTestsDS.SelectedTestTable.NewSelectedTestTableRow

                newTestRow.TestType = blankOrderTest.TestType
                newTestRow.SampleType = blankOrderTest.SampleType
                newTestRow.TestID = blankOrderTest.TestID
                newTestRow.OTStatus = blankOrderTest.OTStatus

                currentTestsDS.SelectedTestTable.Rows.Add(newTestRow)
            Next blankOrderTest
            lstWSBlanksDS = Nothing

            'Inform properties of the auxiliary screen of Tests Selection and open it
            Using myForm As New IWSTestSelectionAuxScreen()
                myForm.SampleClass = "BLANK"
                myForm.SampleType = ""
                myForm.SampleTypeName = ""
                myForm.ListOfSelectedTests = currentTestsDS

                myForm.ShowDialog()
                If (myForm.DialogResult = DialogResult.OK) Then
                    Cursor = Cursors.WaitCursor

                    If (Not myForm.ListOfSelectedTests Is Nothing) Then
                        Dim myGlobalDataTO As GlobalDataTO
                        Dim myOrderTestsDelegate As New OrderTestsDelegate

                        'Delete all Open Blank Order Tests that are not in the list of selected Tests
                        myGlobalDataTO = myOrderTestsDelegate.DeleteBlankOrderTests("NOT_IN_LIST", myForm.ListOfSelectedTests, myWorkSessionResultDS)

                        If (Not myGlobalDataTO.HasError AndAlso myForm.ListOfSelectedTests.SelectedTestTable.Rows.Count > 0) Then
                            'Add all Blank Order Tests for the selected Tests
                            myGlobalDataTO = myOrderTestsDelegate.AddBlankOrderTests(myForm.ListOfSelectedTests, myWorkSessionResultDS, AnalyzerIDAttribute)
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            ChangesMadeAttribute = True
                        Else
                            'Show the error message
                            Cursor = Cursors.Default
                            ShowMessage(Name & "SearchTestsForBlanks", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        End If
                    End If
                End If
            End Using

            'No row should appear as selected in the grid of Blanks and Calibrators
            bsBlkCalibDataGridView.ClearSelection()
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SearchTestsForBlanks", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SearchTestsForBlanks", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow select/unselect Tests when the selected SampleClass is CALIB
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 19/10/2010 - Introduced the Using statement
    '''              TR 28/04/2014 - BT #1494 ==> When the auxiliary screen that allow select/unselect Tests is closed by clicking in OK Button, validates if Screen Property 
    '''                                           IncompleteTest has been set to TRUE (which means that at least one of the selected STD Tests has the Calibration programming 
    '''                                           incomplete) and in this case shows a warning message
    ''' </remarks>
    Private Sub SearchTestsForCalibrators()
        Try
            'Get the list of Calibrator Order Tests (for the selected SampleType) currently in the grid of Blanks&Calibrators 
            Dim lstWSCalibDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
            lstWSCalibDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                           Where a.SampleClass = "CALIB" _
                         AndAlso (a.SampleType = bsSampleTypeComboBox.SelectedValue.ToString _
                          OrElse a.RequestedSampleTypes.Contains(bsSampleTypeComboBox.SelectedValue.ToString)) _
                          Select a).ToList()

            'Load the list of Calibrator Order Tests currently loaded in the grid of Blanks&Calibrators (for the selected SampleType)
            'in a the DataSet of Selected Tests needed for the auxiliary screen of Tests Selection 
            Dim currentTestsDS As New SelectedTestsDS
            Dim newTestRow As SelectedTestsDS.SelectedTestTableRow
            For Each calibOrderTest As WorkSessionResultDS.BlankCalibratorsRow In lstWSCalibDS
                newTestRow = currentTestsDS.SelectedTestTable.NewSelectedTestTableRow

                newTestRow.TestType = calibOrderTest.TestType
                newTestRow.SampleType = bsSampleTypeComboBox.SelectedValue.ToString
                newTestRow.TestID = calibOrderTest.TestID
                newTestRow.OTStatus = calibOrderTest.OTStatus

                currentTestsDS.SelectedTestTable.Rows.Add(newTestRow)
            Next calibOrderTest
            lstWSCalibDS = Nothing

            'Inform properties of the auxiliary screen of Tests Selection and open it
            Using myForm As New IWSTestSelectionAuxScreen()
                myForm.SampleClass = bsSampleClassComboBox.SelectedValue.ToString()
                myForm.SampleType = bsSampleTypeComboBox.SelectedValue.ToString()
                myForm.SampleTypeName = bsSampleTypeComboBox.Text
                myForm.ListOfSelectedTests = currentTestsDS
                myForm.ShowDialog()

                If (myForm.DialogResult = DialogResult.OK) Then
                    Cursor = Cursors.WaitCursor

                    'BT#1494 - Validate if there was selected STD Tests with incomplete Calibration programming to show the warning message
                    '          that notify the User they was removed from the list of Selected Tests
                    If (myForm.IncompleteTest) Then ShowMessage(Name & ".SearchTestsForCalibrators", GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString)

                    If (Not myForm.ListOfSelectedTests Is Nothing) Then
                        Dim myGlobalDataTO As GlobalDataTO
                        Dim myOrderTestsDelegate As New OrderTestsDelegate
                        'Delete all Open Calibrator Order Tests that are not in the list of selected Tests for the active Sample Type
                        myGlobalDataTO = myOrderTestsDelegate.DeleteCalibratorOrderTests("NOT_IN_LIST", myForm.ListOfSelectedTests, myWorkSessionResultDS, _
                                                                                         bsSampleTypeComboBox.SelectedValue.ToString)

                        If (Not myGlobalDataTO.HasError AndAlso myForm.ListOfSelectedTests.SelectedTestTable.Rows.Count > 0) Then
                            'Add all Calibrator Order Tests for the selected Tests for the active Sample Type
                            myGlobalDataTO = myOrderTestsDelegate.AddCalibratorOrderTests(myForm.ListOfSelectedTests, myWorkSessionResultDS, AnalyzerIDAttribute)

                            'Set the Wrap mode of cell AbsValue for multipoint Calibrators having previous results
                            SetWrapCells()
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            ChangesMadeAttribute = True
                        Else
                            'Show the error message
                            Cursor = Cursors.Default
                            ShowMessage(Name & "SearchTestsForCalibrators", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        End If
                    End If
                End If
            End Using

            'No row should appear as selected in the grid of Blanks and Calibrators
            bsBlkCalibDataGridView.ClearSelection()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SearchTestsForCalibrators", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SearchTestsForCalibrators", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow select/unselect Tests when the selected SampleClass is CTRL
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA
    ''' Modified by: DL 15/04/2011 - Removed use of field ControlNumber
    '''              RH 19/10/2010 - Introduced the Using statement
    '''              TR 11/03/2013 - Inform LISRequest in SelectedTestsDS with the same value it has for the
    '''                              corresponding Order Test.
    '''              TR 17/05/2013 - Inform optional parameters when calling function AddControlOrderTests
    '''              TR 28/04/2014 - BT #1494 ==> When the auxiliary screen that allow select/unselect Tests is closed by clicking in OK Button, validates if Screen Property 
    '''                                           IncompleteTest has been set to TRUE (which means that at least one of the selected STD Tests has the Calibration programming 
    '''                                           incomplete) and in this case shows a warning message
    ''' </remarks>
    Private Sub SearchTestsForControls()
        Try
            'Get the list of Control Order Tests (for the selected SampleType) currently loaded in the grid of Controls 
            Dim lstWSCtrlDS As List(Of WorkSessionResultDS.ControlsRow)
            lstWSCtrlDS = (From a In myWorkSessionResultDS.Controls _
                          Where a.SampleClass = "CTRL" _
                            AndAlso a.SampleType = bsSampleTypeComboBox.SelectedValue.ToString _
                         Order By a.TestType, a.TestID _
                         Select a).ToList()

            'Load the list of Control Order Tests currently loaded in the grid of Controls (for the selected SampleType)
            'in a the DataSet of Selected Tests needed for the auxiliary screen of Tests Selection 
            Dim currentTestsDS As New SelectedTestsDS
            Dim newTestRow As SelectedTestsDS.SelectedTestTableRow

            Dim i As Integer
            Dim testType As String = ""
            Dim testID As Integer = 0
            Dim sampleType As String = ""

            For Each ctrlOrderTest As WorkSessionResultDS.ControlsRow In lstWSCtrlDS
                If (testType <> ctrlOrderTest.TestType) OrElse (testID <> ctrlOrderTest.TestID) OrElse (sampleType <> ctrlOrderTest.SampleType) Then
                    newTestRow = currentTestsDS.SelectedTestTable.NewSelectedTestTableRow

                    newTestRow.TestType = ctrlOrderTest.TestType
                    newTestRow.SampleType = ctrlOrderTest.SampleType
                    newTestRow.TestID = ctrlOrderTest.TestID
                    newTestRow.OTStatus = ctrlOrderTest.OTStatus
                    newTestRow.NumberOfControls = 1
                    newTestRow.LISRequest = ctrlOrderTest.LISRequest

                    currentTestsDS.SelectedTestTable.Rows.Add(newTestRow)

                    'Set value of variables used to control the loop --> When more than one Control is required for a Test it appears as
                    'n rows in the grid, but it has to be added to Selected Tests DataSet just once
                    testType = ctrlOrderTest.TestType
                    testID = ctrlOrderTest.TestID
                    sampleType = ctrlOrderTest.SampleType
                Else
                    'Update field NumberOfControls of the last added row --> When more than one Control is required for a Test it appears as
                    'n rows in the grid but it is added only once to the Selected Tests DataSet. However, it is needed pass to the auxiliary 
                    'screen the number of controls currently in the grid due to it needs that information to mark the Test as fully or 
                    'partially selected
                    i = currentTestsDS.SelectedTestTable.Rows.Count - 1
                    currentTestsDS.SelectedTestTable(i).BeginEdit()
                    currentTestsDS.SelectedTestTable(i).NumberOfControls += 1
                    currentTestsDS.SelectedTestTable(i).EndEdit()
                End If
            Next ctrlOrderTest
            lstWSCtrlDS = Nothing

            'Inform properties of the auxiliary screen of Tests Selection and open it
            Using myForm As New IWSTestSelectionAuxScreen()
                myForm.SampleClass = bsSampleClassComboBox.SelectedValue.ToString()
                myForm.SampleType = bsSampleTypeComboBox.SelectedValue.ToString()
                myForm.SampleTypeName = bsSampleTypeComboBox.Text
                myForm.ListOfSelectedTests = currentTestsDS
                myForm.ShowDialog()

                If (myForm.DialogResult = DialogResult.OK) Then
                    Cursor = Cursors.WaitCursor

                    'BT#1494 - Validate if there was selected STD Tests with incomplete Calibration programming to show the warning message
                    '          that notify the User they was removed from the list of Selected Tests
                    If (myForm.IncompleteTest) Then ShowMessage(Name & ".SearchTestsForControls", GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString)


                    If (Not myForm.ListOfSelectedTests Is Nothing) Then
                        Dim myGlobalDataTO As GlobalDataTO
                        Dim myOrderTestsDelegate As New OrderTestsDelegate

                        'Delete all Open Control Order Tests that are not in the list of selected Tests for the active Sample Type
                        myGlobalDataTO = myOrderTestsDelegate.DeleteControlOrderTests("NOT_IN_LIST", myForm.ListOfSelectedTests, myWorkSessionResultDS, _
                                                                                      bsSampleTypeComboBox.SelectedValue.ToString)
                        If (Not myGlobalDataTO.HasError AndAlso myForm.ListOfSelectedTests.SelectedTestTable.Rows.Count > 0) Then
                            'Add all Control Order Tests for the selected Tests for the active Sample Type
                            myGlobalDataTO = myOrderTestsDelegate.AddControlOrderTests(myForm.ListOfSelectedTests, myWorkSessionResultDS, AnalyzerIDAttribute, False, False, "CTRL")
                        End If

                        If (Not myGlobalDataTO.HasError) Then
                            ChangesMadeAttribute = True
                        Else
                            'Show the error message
                            Cursor = Cursors.Default
                            ShowMessage(Name & "SearchTestsForControls", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        End If
                    End If
                End If
            End Using

            'No row should appear as selected in the grid of Controls
            bsControlOrdersDataGridView.ClearSelection()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SearchTestsForControls", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SearchTestsForControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Cursor = Cursors.Default()
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow select/unselect Tests when the selected SampleClass is PATIENT
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 19/10/2010 - Introduced the Using statement
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode
    '''              TR 29/04/2014 - BT #1494 ==> When the auxiliary screen that allow select/unselect Tests is closed by clicking in OK Button, validates if Screen Property 
    '''                                           IncompleteTest has been set to TRUE (which means that at least one of the selected STD Tests has the Calibration programming 
    '''                                           incomplete) and in this case shows a warning message
    '''              SA 20/05/2014 - BT #1633 ==> Added validation of Screen Property IncompleteTest for patientCase = 3 (n rows selected in Patient Samples grid having 
    '''                                           different Tests and belonging to different Patients). This change should have been included in BT #1494
    ''' </remarks>
    Private Sub SearchTestsForPatientSamples()
        Try
            'Get value of field PatientID/SampleID in the header area, and verify if it corresponds to a one with
            'requested Order Tests; in that case, set value of the SampleID in the grid to be sure the format is
            'the same (Upper/Lower case)
            Dim myPatientID As String = bsPatientIDTextBox.Text.Trim

            If (myPatientID <> String.Empty) Then
                Dim lstPatientExist As List(Of String)
                lstPatientExist = (From a In myWorkSessionResultDS.Patients _
                                  Where a.SampleID.ToUpperInvariant = myPatientID.ToUpperInvariant _
                                 Select a.SampleID Distinct).ToList()

                If (lstPatientExist.Count > 0) Then myPatientID = lstPatientExist.First
                lstPatientExist = Nothing
            End If

            'Get the number of lines currently selected in the grid of Patient Order Test
            Dim linesSelected As Integer = bsPatientOrdersDataGridView.SelectedRows.Count

            'Determine the way of opening the auxiliary screen of Tests Selection and the way of process the selected/unselected Tests 
            'after closing it
            Dim patientCase As Byte = 0
            If (linesSelected = 0 AndAlso myPatientID = "") OrElse (linesSelected = 1 AndAlso myPatientID = "") Then
                'Any row selected in the grid of Patient Order Tests + PatientID/SampleID non informed --> Add Tests for one or more
                'anonymus (unknown) Patients (PatientCase = 1)
                patientCase = 1

            ElseIf (linesSelected = 0 AndAlso myPatientID <> "") OrElse (linesSelected = 1) Then
                'Any row selected in the grid of Patient Order Tests + PatientID/SampleID informed, or just one row selected in 
                'the grid --> Add Tests to the selected Patient (PatientCase = 2)
                patientCase = 2

            ElseIf (linesSelected > 1) Then
                If (myPatientID <> "") Then
                    'n rows selected in the grid of Patient Order Tests having all the same PatientID/SampleID and StatFlag; the SampleType of the
                    'different selected rows is not important --> Same behaviour that when linesSelected=1; the SampleType loaded in the header area
                    'is the one of the last selected row (PatientCase = 2)
                    patientCase = 2

                Else
                    'n rows selected in the grid of Patient Order Tests having different PatientID/SampleID and StatFlag; the SampleType of the
                    'different selected rows is not important (the SampleType loaded in the header area is the one of the last selected row) --> The
                    'auxiliary screen of Tests Selection is opened with all Test as unselected; all selected Tests will be added to all the different
                    'PatientID/SampleID and StatFlag selected, ignoring those Tests previouly added
                    patientCase = 3
                End If
            End If

            'Execute the Tests searching according the obtained PatientCase
            Dim myGlobalDataTO As New GlobalDataTO
            Dim currentTestsDS As New SelectedTestsDS
            Dim myMaxOrderTestsDS As New MaxOrderTestsValuesDS

            'Get the selected StatFlag and SampleType in the header area
            Dim mySampleType As String = bsSampleTypeComboBox.SelectedValue.ToString
            Dim myStatFlag As Boolean = bsStatCheckbox.Checked

            Dim actionCancelled As Boolean = True
            Select Case (patientCase)
                Case 1
                    'Load the Typed DataSet SelectedTestsDS before opening the auxiliary screen
                    If (FillSelectedTestsForPatient(myPatientID, myStatFlag, mySampleType, currentTestsDS)) Then
                        'Load the Typed DataSet MaxOrderTestsValuesDS before opening the auxiliary screen
                        FillMaxOrderTestValues(myMaxOrderTestsDS)

                        'Inform properties and open the screen of Tests Selection
                        Using myForm As New IWSTestSelectionAuxScreen()
                            myForm.SampleClass = bsSampleClassComboBox.SelectedValue.ToString()
                            myForm.SampleType = bsSampleTypeComboBox.SelectedValue.ToString()
                            myForm.SampleTypeName = bsSampleTypeComboBox.Text
                            myForm.ListOfSelectedTests = currentTestsDS
                            myForm.MaxValues = myMaxOrderTestsDS

                            myForm.ShowDialog()
                            If (myForm.DialogResult = Windows.Forms.DialogResult.OK) Then
                                Cursor = Cursors.WaitCursor
                                actionCancelled = False

                                'BT#1494 - Validate if there was selected STD Tests with incomplete Calibration programming to show the warning message
                                '          that notify the User they was removed from the list of Selected Tests
                                If (myForm.IncompleteTest) Then ShowMessage(Name & ".SearchTestsForPatientSamples", GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString)

                                'Calculate the maximun AutoNumeric PatientSample currently in the grid when the Order(s) to add 
                                'are for unknown Patients
                                sampleIDType = "AUTO"

                                Dim lastSampleID As String = CalculateAutonumeric()
                                Dim lastDate As String = DateTime.Now.ToString("yyyyMMdd")
                                Dim maxAutoNumericValue As Integer = -1

                                If (lastSampleID.Trim <> "") Then
                                    lastDate = Mid(lastSampleID, 2, 8)
                                    maxAutoNumericValue = CInt(Mid(lastSampleID, Len(lastSampleID) - 3))
                                End If

                                'Add all Patient Order Tests of the selected Tests for the active Sample Type
                                Dim myOrderTestsDelegate As New OrderTestsDelegate
                                myGlobalDataTO = myOrderTestsDelegate.AddPatientOrderTests(myForm.ListOfSelectedTests, myWorkSessionResultDS, AnalyzerIDAttribute, _
                                                                                           "", myStatFlag, sampleIDType, CInt(bsNumOrdersNumericUpDown.Value), _
                                                                                           maxAutoNumericValue, lastDate)

                                'Resort the autonumeric SampleID when it is needed
                                CalculateAllAutonumeric()
                                Cursor = Cursors.Default
                            End If
                        End Using
                    End If
                Case 2
                    'If there is a selected PatientID/SampleID, get the type of SampleID
                    If (linesSelected > 0) Then sampleIDType = bsPatientOrdersDataGridView.SelectedRows(0).Cells("SampleIDType").Value.ToString

                    'Load the Typed DataSet SelectedTestsDS before opening the auxiliary screen
                    If (FillSelectedTestsForPatient(myPatientID, myStatFlag, mySampleType, currentTestsDS)) Then

                        'Load a Typed DataSet SelectedTestsDS with the list of Tests selected for the same Patient but different priority
                        Dim lockedTestsDS As New SelectedTestsDS
                        FillSelectedTestsForDifPriority(myPatientID, myStatFlag, mySampleType, lockedTestsDS)

                        'Load the Typed DataSet MaxOrderTestsValuesDS before opening the auxiliary screen
                        FillMaxOrderTestValues(myMaxOrderTestsDS)

                        'Inform properties and open the screen of Tests Selection
                        Using myForm As New IWSTestSelectionAuxScreen()
                            myForm.SampleClass = bsSampleClassComboBox.SelectedValue.ToString()
                            myForm.SampleType = bsSampleTypeComboBox.SelectedValue.ToString()
                            myForm.SampleTypeName = bsSampleTypeComboBox.Text
                            myForm.ListOfSelectedTests = currentTestsDS
                            myForm.SelectedTestsInDifPriority = lockedTestsDS
                            myForm.PatientID = myPatientID
                            myForm.MaxValues = myMaxOrderTestsDS

                            myForm.ShowDialog()
                            If (myForm.DialogResult = Windows.Forms.DialogResult.OK) Then
                                Cursor = Cursors.WaitCursor
                                actionCancelled = False

                                'BT#1494 - Validate if there was selected STD Tests with incomplete Calibration programming to show the warning message
                                '          that notify the User they was removed from the list of Selected Tests
                                If (myForm.IncompleteTest) Then ShowMessage(Name & ".SearchTestsForPatientSamples", GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString)

                                If (Not myForm.ListOfSelectedTests Is Nothing) Then
                                    'Delete all Open Patient Order Tests that are not in the list of selected Tests for the active StatFlag, Sample Type, SampleID
                                    Dim myOrderTestsDelegate As New OrderTestsDelegate
                                    myGlobalDataTO = myOrderTestsDelegate.DeletePatientOrderTests("NOT_IN_LIST", myForm.ListOfSelectedTests, myWorkSessionResultDS, _
                                                                                                  bsSampleTypeComboBox.SelectedValue.ToString, myPatientID, myStatFlag)

                                    'If there are Order Tests to add...
                                    If (Not myGlobalDataTO.HasError AndAlso myForm.ListOfSelectedTests.SelectedTestTable.Rows.Count > 0) Then
                                        'If the SampleID was manually informed but it has not been validated...
                                        If (linesSelected = 0 AndAlso sampleIDType = "MAN" AndAlso Not validatedSampleID) Then
                                            'Verify if the informed Patient/Sample corresponds to an existing PatientID 

                                            Dim myPatientDelegate As New PatientDelegate
                                            myGlobalDataTO = myPatientDelegate.GetPatientData(Nothing, myPatientID)

                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                Dim myPatientDS As New PatientsDS
                                                myPatientDS = DirectCast(myGlobalDataTO.SetDatos, PatientsDS)

                                                If (myPatientDS.tparPatients.Rows.Count > 0) Then sampleIDType = "DB"
                                                validatedSampleID = True
                                            End If
                                        End If

                                        'Add all Patient Order Tests of the selected Tests for the active Sample Type
                                        myGlobalDataTO = myOrderTestsDelegate.AddPatientOrderTests(myForm.ListOfSelectedTests, myWorkSessionResultDS, _
                                                                                                   AnalyzerIDAttribute, myPatientID, myStatFlag, sampleIDType)
                                    Else
                                        'If all Tests was deleted for an automatic PatientID/SampleID it is needed re-calculate value of the rest of the automatic IDs
                                        If (sampleIDType = "AUTO") Then CalculateAllAutonumeric()
                                    End If
                                End If
                                Cursor = Cursors.Default
                            End If
                        End Using
                    End If
                Case 3
                    Dim openTestSelection As Boolean = True
                    For Each myRow As DataGridViewRow In bsPatientOrdersDataGridView.SelectedRows
                        If (myRow.Cells("OTStatus").Value.ToString() <> "OPEN") Then
                            'Show message to confirm the adding of more Order Tests for this Patient... continue only after confirmation
                            openTestSelection = (ShowMessage(bsPrepareWSLabel.Text, GlobalEnumerates.Messages.ADD_MORE_TESTS_TO_PATIENT.ToString, , Me) = Windows.Forms.DialogResult.Yes)
                            Exit For
                        End If
                    Next

                    If (openTestSelection) Then
                        'Load the Typed DataSet MaxOrderTestsValuesDS before opening the auxiliary screen
                        FillMaxOrderTestValues(myMaxOrderTestsDS)

                        'Inform properties and open the screen of Tests Selection
                        Using myForm As New IWSTestSelectionAuxScreen()
                            myForm.SampleClass = bsSampleClassComboBox.SelectedValue.ToString()
                            myForm.SampleType = bsSampleTypeComboBox.SelectedValue.ToString()
                            myForm.SampleTypeName = bsSampleTypeComboBox.Text
                            myForm.ListOfSelectedTests = Nothing
                            myForm.MaxValues = myMaxOrderTestsDS

                            myForm.ShowDialog()

                            If (myForm.DialogResult = Windows.Forms.DialogResult.OK) Then
                                Cursor = Cursors.WaitCursor
                                actionCancelled = False

                                'BT #1633 - Validate if there was selected STD Tests with incomplete Calibration programming to show the warning message
                                '           that notify the User they was removed from the list of Selected Tests
                                If (myForm.IncompleteTest) Then ShowMessage(Name & ".SearchTestsForPatientSamples", GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString)

                                Dim myOrderTestsDelegate As New OrderTestsDelegate
                                If (Not myForm.ListOfSelectedTests Is Nothing) Then
                                    Dim currentSampleID As String = ""
                                    Dim currentStatFlag As Boolean = False

                                    'Save the list of different selected PatientID/SampleID and StatFlag 
                                    Dim myOrdersDS As New OrdersDS
                                    Dim newOrderRow As OrdersDS.twksOrdersRow

                                    For iRow As Integer = 0 To bsPatientOrdersDataGridView.SelectedRows.Count - 1
                                        If (currentSampleID <> bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("SampleID").Value.ToString OrElse _
                                            currentStatFlag <> CBool(bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("StatFlag").Value)) Then

                                            newOrderRow = myOrdersDS.twksOrders.NewtwksOrdersRow

                                            newOrderRow.SampleID = bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("SampleID").Value.ToString
                                            newOrderRow.StatFlag = CBool(bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("StatFlag").Value)
                                            newOrderRow.SampleIDType = bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("SampleIDType").Value.ToString
                                            myOrdersDS.twksOrders.Rows.Add(newOrderRow)

                                            'Update values of variables used to control the loop
                                            currentSampleID = bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("SampleID").Value.ToString
                                            currentStatFlag = CBool(bsPatientOrdersDataGridView.SelectedRows(iRow).Cells("StatFlag").Value)
                                        End If
                                    Next

                                    'For each different selected PatientID/SampleID and StatFlag, add the list of selected Tests 
                                    For Each selectedOrder As OrdersDS.twksOrdersRow In myOrdersDS.twksOrders.Rows
                                        'Add the new Tests to the PatientID/SampleID currently selected
                                        myGlobalDataTO = myOrderTestsDelegate.AddPatientOrderTests(myForm.ListOfSelectedTests, myWorkSessionResultDS, AnalyzerIDAttribute, _
                                                                                                   selectedOrder.SampleID, selectedOrder.StatFlag, selectedOrder.SampleIDType)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    Next
                                End If
                                Cursor = Cursors.Default
                            End If
                        End Using
                    End If
            End Select

            If (Not actionCancelled) Then
                If (Not myGlobalDataTO.HasError) Then
                    Cursor = Cursors.WaitCursor
                    ChangesMadeAttribute = True

                    'Update the global DataSet
                    If (Not myGlobalDataTO.SetDatos Is Nothing) Then myWorkSessionResultDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)

                    'Verify if there are unselected OrderTests having a Test included in a Calculated Test Formula to unselect also the correspondent Order Test
                    Dim lstWSPatients As List(Of WorkSessionResultDS.PatientsRow)
                    lstWSPatients = (From a In myWorkSessionResultDS.Patients _
                                     Where a.Selected = False _
                                     AndAlso (Not a.IsCalcTestIDNull) _
                                     Order By a.SampleID, a.StatFlag _
                                     Select a).ToList

                    For Each testInFormula As WorkSessionResultDS.PatientsRow In lstWSPatients
                        UnSelectRelatedCalcTest(testInFormula.SampleID, testInFormula.StatFlag, testInFormula.CalcTestID)
                    Next
                    lstWSPatients = Nothing

                    'Initialize values in area of criteria selection
                    SetFieldStatusForPatient(True)

                    'Apply styles to special columns, and apply colors
                    ApplyStylesToSpecialGridColumns()

                    'Verify if button for adding results of Off-System Tests can be enabled 
                    OffSystemResultsButtonEnabled()

                    Cursor = Cursors.Default
                    bsPatientIDTextBox.Focus()
                Else
                    'Show the error message
                    Cursor = Cursors.Default
                    ShowMessage(Name & ".SearchTestsForPatientSamples", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            Else
                'Initialize values in area of criteria selection
                SetFieldStatusForPatient(True)
                bsPatientIDTextBox.Focus()
                Cursor = Cursors.Default
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SearchTestsForPatientSamples", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            Cursor = Cursors.Default
            ShowMessage(Name & ".SearchTestsForPatientSamples", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Clear area of criteria selection after adding or deleting Patient Order Tests, or after select an existing Patient
    ''' </summary>
    ''' <param name="pInitializeFields">When True, it indicates fields in the criteria selection area has to be initialized</param>
    ''' <remarks>
    ''' Modified by: SA 09/07/2010 - Do not change the current checked status of the Stat CheckBox, only enable/disable it
    ''' </remarks>
    Private Sub SetFieldStatusForPatient(ByVal pInitializeFields As Boolean)
        Try
            If (pInitializeFields) Then
                bsStatCheckbox.Enabled = True

                bsPatientIDTextBox.Text = ""
                bsPatientIDTextBox.ReadOnly = False
                bsPatientIDTextBox.BackColor = Color.White
                bsPatientSearchButton.Enabled = True

                bsNumOrdersNumericUpDown.Value = 1
                bsNumOrdersNumericUpDown.Enabled = True
                bsNumOrdersNumericUpDown.BackColor = Color.White

                'Initialize also global variables needed to determine the type of Sample 
                sampleIDType = "MAN"
                validatedSampleID = False
            Else
                bsPatientIDTextBox.ReadOnly = True
                bsPatientIDTextBox.BackColor = Color.Gainsboro
                bsPatientSearchButton.Enabled = False

                bsNumOrdersNumericUpDown.Value = 1
                bsNumOrdersNumericUpDown.Enabled = False
                bsNumOrdersNumericUpDown.BackColor = SystemColors.MenuBar
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetFieldStatusForPatient", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetFieldStatusForPatient", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' For Multipoint Calibrators having previous results, set property WrapMode=True for the cell containing
    ''' the Absorbance Value of each point
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 14/04/2010
    ''' Modified by: SA 30/11/2010 - For SinglePoint Calibrators having OriginalFactorValue different FactorValue, set Font
    '''                              of ABSValue to Strike
    '''              RH 15/06/2011 - Code optimization
    ''' </remarks>
    Private Sub SetWrapCells()
        Try
            Dim myCalibsDGRows As List(Of DataGridViewRow) = (From a In bsBlkCalibDataGridView.Rows.Cast(Of DataGridViewRow)() _
                                                             Where a.Cells("SampleClass").Value.Equals("CALIB") _
                                                           AndAlso Not a.Cells("NumberOfCalibrators").Value Is Nothing _
                                                           AndAlso Not a.Cells("PreviousOrderTestID").Value Is DBNull.Value _
                                                            Select a).ToList()
            Dim myNumOfCalibrators As Integer
            For Each row As DataGridViewRow In myCalibsDGRows
                myNumOfCalibrators = Convert.ToInt32(row.Cells("NumberOfCalibrators").Value)
                If (myNumOfCalibrators) = 1 Then
                    If (row.Cells("FactorValue").FormattedValue.ToString <> row.Cells("OriginalFactorValue").FormattedValue.ToString) Then
                        row.Cells("ABSValue").Style.Font = StrikeFont
                    Else
                        row.Cells("ABSValue").Style.Font = RegularFont
                    End If
                ElseIf (myNumOfCalibrators > 1) Then
                    row.DefaultCellStyle.WrapMode = DataGridViewTriState.True
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetWrapCells", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetWrapCells", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 'Shown the LIMS Import Errors Screen when click in the LIMS Errors button
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/09/2010
    ''' Modified by: RH 19/10/2010 - Introduced the Using statement
    ''' </remarks>
    Private Sub ShowLIMSImportErrors()
        Try
            Using LIMSImportErrorsDialog As New IWSImportLIMSErrors()
                LIMSImportErrorsDialog.ListOfImportErrors = myImportErrorsLogDS
                LIMSImportErrorsDialog.ShowDialog()
            End Using
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ShowLIMSImportErrors", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowLIMSImportErrors", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Unselect the Calibrator defined for the specified SampleType/Test if it is possible
    ''' Unselect the Blank defined for the specified Test if it is possible
    ''' </summary>
    ''' <param name="pTestType">Test Type Code</param>
    ''' <param name="pSampleType">Sample Type Code</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <remarks>
    ''' Created by:  SA 12/07/2010 
    ''' </remarks>
    Private Sub UnselectBlkCalibrator(ByVal pTestType As String, ByVal pSampleType As String, ByVal pTestID As Integer)
        Try
            'Search the Calibrator needed for the SampleType/TestID (if any)
            Dim lstWSCalibDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
            lstWSCalibDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                           Where a.SampleClass = "CALIB" _
                         AndAlso a.TestType = pTestType _
                         AndAlso (a.SampleType = pSampleType OrElse _
                                  a.RequestedSampleTypes.Contains(pSampleType)) _
                         AndAlso a.TestID = pTestID _
                          Select a).ToList()

            'There is a selected Calibrator for the SampleType/TestID
            Dim myVerification As Boolean = True
            If (lstWSCalibDS.Count = 1) Then
                If (lstWSCalibDS(0).Selected) Then
                    Dim myOrderTestsDelegate As New OrderTestsDelegate
                    myVerification = myOrderTestsDelegate.VerifyUnselectedOrderTest("CALIB", pTestType, pTestID, lstWSCalibDS(0).SampleType, _
                                                                                    myWorkSessionResultDS)
                    If (myVerification) Then
                        'Unselect also the Calibrator
                        lstWSCalibDS(0).Selected = False
                        myWorkSessionResultDS.BlankCalibrators.AcceptChanges()
                    End If
                Else
                    'The needed Calibrator is already unselected
                    myVerification = False
                End If
            End If
            lstWSCalibDS = Nothing

            If (myVerification) Then
                'Search the Blank for the Test and unselect also it
                Dim lstWSBlankDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
                lstWSBlankDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                               Where a.SampleClass = "BLANK" _
                             AndAlso a.TestType = pTestType _
                             AndAlso a.TestID = pTestID _
                             AndAlso a.Selected = True _
                              Select a).ToList()

                If (lstWSBlankDS.Count = 1) Then
                    lstWSBlankDS(0).Selected = False
                    myWorkSessionResultDS.BlankCalibrators.AcceptChanges()
                End If
                lstWSBlankDS = Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UnselectBlkCalibrator", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UnselectBlkCalibrator", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When a Patient Order Test of a Test included in the formula of a Calculated Test is unselected, the Order Test
    ''' of the corresponding Calculated Tests are also unselected 
    ''' </summary>
    ''' <param name="pSampleID">PatientID/SampleID of the unselected Order Test</param>
    ''' <param name="pStatFlag">StatFlag of the unselected Order Test</param>
    ''' <param name="pCalcTestIDs">List of IDs of the Calculated Tests in which the unselected Test is included</param>
    ''' <remarks>
    ''' Created by:  SA 17/05/2010
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              XB 06/03/2013 - Implement again ToUpper because is not redundant but using invariant mode
    ''' </remarks>
    Private Sub UnSelectRelatedCalcTest(ByVal pSampleID As String, ByVal pStatFlag As Boolean, ByVal pCalcTestIDs As String)
        Try
            Dim calcIDs() As String = pCalcTestIDs.Split(CChar(", "))
            Dim lstCalcTest As List(Of WorkSessionResultDS.PatientsRow)

            For i As Integer = 0 To calcIDs.Length - 1
                'Search the Calculated Test to unselect it - 
                lstCalcTest = (From a In myWorkSessionResultDS.Patients _
                              Where a.SampleID.ToUpperInvariant = pSampleID.ToUpperInvariant _
                            AndAlso a.StatFlag = pStatFlag _
                            AndAlso a.TestType = "CALC" _
                            AndAlso a.TestID = Convert.ToInt32(calcIDs(i)) _
                             Select a).ToList()

                If (lstCalcTest.Count = 1) Then
                    lstCalcTest.First.Selected = False
                    If (Not lstCalcTest.First.IsCalcTestIDNull AndAlso lstCalcTest.First.CalcTestID <> "") Then
                        'If the Calculated Test is part of the Formula of another Calculated Test, then it has to be also unselected
                        UnSelectRelatedCalcTest(pSampleID, pStatFlag, lstCalcTest.First.CalcTestID)
                    End If
                    myWorkSessionResultDS.Patients.AcceptChanges()
                End If
            Next
            lstCalcTest = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UnSelectRelatedCalcTest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UnSelectRelatedCalcTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if there are Samples Barcode Positions with no requested Tests and Enable/Disable Button BarcodeWarningButton.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 16/09/2011
    ''' Modified by: SA 25/04/2013 - Validation is executed only when variable LISWithFilesMode is TRUE; when it is FALSE, it has not
    '''                              sense due to the button is always hide
    ''' </remarks>
    Private Sub ValidateBCWarningButtonEnabled()
        Try
            If (LISWithFilesMode) Then
                Dim myGlobalDataTO As New GlobalDataTO
                Dim myBarcodePositionsWithNoRequestsDelegate As New BarcodePositionsWithNoRequestsDelegate

                myGlobalDataTO = myBarcodePositionsWithNoRequestsDelegate.ReadByAnalyzerAndWorkSession(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS = DirectCast(myGlobalDataTO.SetDatos, BarcodePositionsWithNoRequestsDS)
                    If (myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows.Count > 0) Then
                        bsBarcodeWarningButton.Enabled = True
                    Else
                        bsBarcodeWarningButton.Enabled = False
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateBCWarningButtonEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateBCWarningButtonEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if Scanning Barcode Button has to be enabled (depending on the current Analyzer status and also on current value of 
    ''' Analyzer Setting SAMPLE_BARCODE_DISABLED)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 12/03/2012 - Code moved from sub PrepareButtons
    ''' Modified by: SA 25/04/2013 - Validation is executed only when variable LISWithFilesMode is TRUE; when it is FALSE, it has not
    '''                              sense due to the button is always hide
    '''              SA 16/10/2013 - BT #1334 ==> Changes in button activation rule due to new Analyzer mode PAUSE in RUNNING: the Scanning button 
    '''                              can be available not only when the Analyzer is in STAND BY, but also when it is in RUNNING but it is currently 
    '''                              in PAUSE mode - NOTE: this change has not been tested due to the option for LIS with files has been disabled 
    '''                              from version 2.0.0
    ''' </remarks>
    Private Sub ValidateScanningButtonEnabled()
        Try
            If (IsDisposed) Then Exit Sub 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            If (LISWithFilesMode) Then
                Dim statusScanningButton As Boolean = False

                If (Not mdiAnalyzerCopy Is Nothing) Then
                    'Verify the Analyzer is not Warming Up
                    Dim sensorValue As Single = mdiAnalyzerCopy.GetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED)

                    'Get value of the Analyzer Setting indicating if the Samples Barcode is enabled or not
                    Dim resultData As New GlobalDataTO
                    Dim analyzerSettings As New AnalyzerSettingsDelegate

                    Dim sampleBarcodeReaderOFF As Boolean = False
                    resultData = analyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myDataSet As AnalyzerSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                        If (myDataSet.tcfgAnalyzerSettings.Rows.Count > 0) Then
                            sampleBarcodeReaderOFF = CType(myDataSet.tcfgAnalyzerSettings(0).CurrentValue, Boolean)
                        End If
                    End If

                    'If the Analyzer is Connected and Ready, the WarmUp maneuvers have finished, the Barcode Reader is available and not disabled....
                    If (mdiAnalyzerCopy.Connected AndAlso mdiAnalyzerCopy.AnalyzerIsReady AndAlso sensorValue = 1 AndAlso _
                        mdiAnalyzerCopy.BarCodeProcessBeforeRunning = AnalyzerManager.BarcodeWorksessionActions.BARCODE_AVAILABLE AndAlso _
                       (Not sampleBarcodeReaderOFF)) Then
                        'If the Analyzer is in STAND BY or if it is in RUNNING but has been PAUSED...
                        If (mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY OrElse _
                           (mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso mdiAnalyzerCopy.AllowScanInRunning)) Then
                            If (Not IAx00MainMDI Is Nothing) Then  'This condition is to be sure a new instance of the MDI is not created 
                                'Verify if the Scanning Button can be available by checking Alarms and another Analyzer states
                                statusScanningButton = IAx00MainMDI.ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.READ_BARCODE)
                            End If
                        End If
                    End If
                End If

                bsScanningButton.Enabled = statusScanningButton
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateScanningButtonEnabled ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateScanningButtonEnabled ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' For all single point Calibrators having a previous result included in the WS and for which the Calibrator 
    ''' Factor has been changed, update fields ManualResultFlag and ManualResult in Results table and execute all
    ''' needed recalculations
    ''' </summary>
    ''' <returns>GlobalDataTO containing sucess/error information</returns>
    ''' <remarks>
    ''' Created by:  AG/SA 30/11/2010
    ''' Modified by: SA    24/07/2012 - Call function UpdateManualCalibrationFactorNEW in RecalculateResultsDelegate instead of 
    '''                                 call function UpdateManualCalibrationFactor in the same delegate 
    ''' </remarks>
    Private Function VerifyChangesInCalibrationFactor() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            'Get all single point Calibrators having previous results and with CalibratorFactor informed
            Dim lstWSCalibratorsDS As List(Of WorkSessionResultDS.BlankCalibratorsRow)
            lstWSCalibratorsDS = (From a In myWorkSessionResultDS.BlankCalibrators _
                                 Where a.SampleClass = "CALIB" _
                               AndAlso (Not a.IsNumberOfCalibratorsNull AndAlso a.NumberOfCalibrators = 1) _
                               AndAlso (Not a.IsPreviousOrderTestIDNull) _
                               AndAlso (Not a.IsFactorValueNull) _
                                Select a).ToList()

            Dim myResultsDelegate As New ResultsDelegate
            Dim myHisWSResultsDelegate As New HisWSResultsDelegate
            For Each calibratorOrderTest As WorkSessionResultDS.BlankCalibratorsRow In lstWSCalibratorsDS
                'Update value of Manual Factor or Experimental Factor depending if FactorValue <> OriginalFactorValue
                Dim myManualResultFlag As Boolean = (calibratorOrderTest.FactorValue <> calibratorOrderTest.OriginalFactorValue)

                'Check the current Factor value; if it has been changed, then call recalculations
                myGlobalDataTO = myResultsDelegate.GetAcceptedResults(Nothing, calibratorOrderTest.PreviousOrderTestID)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myResultsDS As ResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)

                    If (myResultsDS.twksResults.Rows.Count > 0) Then
                        Dim recalculate As Boolean = False
                        If (Not myResultsDS.twksResults(0).ManualResultFlag) Then
                            If (Not myResultsDS.twksResults(0).IsCalibratorFactorNull) Then
                                Dim myValue As Single = Convert.ToSingle(myResultsDS.twksResults(0).CalibratorFactor.ToString(GlobalConstants.CALIBRATOR_FACTOR_FORMAT))
                                recalculate = (myValue <> calibratorOrderTest.FactorValue)
                            End If
                        Else
                            If (Not myResultsDS.twksResults(0).IsManualResultNull) Then
                                Dim myValue As Single = Convert.ToSingle(myResultsDS.twksResults(0).ManualResult.ToString(GlobalConstants.CALIBRATOR_FACTOR_FORMAT))
                                recalculate = (myValue <> calibratorOrderTest.FactorValue)
                            End If
                        End If

                        If (recalculate) Then

                            'Load needed data in a vwksWSExecutionsResultsRow in ExecutionsDS
                            Dim myExecutionDS As New ExecutionsDS
                            Dim selectedExecRow As ExecutionsDS.vwksWSExecutionsResultsRow

                            selectedExecRow = myExecutionDS.vwksWSExecutionsResults.NewvwksWSExecutionsResultsRow
                            selectedExecRow.AnalyzerID = AnalyzerIDAttribute
                            selectedExecRow.WorkSessionID = WorkSessionIDAttribute
                            selectedExecRow.ExecutionID = -1
                            selectedExecRow.OrderTestID = calibratorOrderTest.PreviousOrderTestID
                            selectedExecRow.RerunNumber = -1
                            selectedExecRow.MultiItemNumber = 1
                            selectedExecRow.ReplicateNumber = 1
                            selectedExecRow.TestID = calibratorOrderTest.TestID
                            selectedExecRow.SampleType = calibratorOrderTest.SampleType
                            selectedExecRow.ExecutionStatus = "CLOSED"

                            Dim myRecalcDelegate As New RecalculateResultsDelegate
                            myRecalcDelegate.AnalyzerModel = AnalyzerModel
                            myGlobalDataTO = myRecalcDelegate.UpdateManualCalibrationFactorNEW(selectedExecRow, myManualResultFlag, calibratorOrderTest.FactorValue)
                            If (myGlobalDataTO.HasError) Then Exit For
                        End If
                    End If
                End If
            Next
            lstWSCalibratorsDS = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".VerifyChangesInCalibrationFactor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".VerifyChangesInCalibrationFactor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' For all not positioned Patient and Controls requested by LIS, verify which related elements can be also unselected  
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 03/04/2013
    ''' </remarks>
    Private Sub UnselectLISOrderTests()
        Try
            'Verify if related elements for Patients requested by LIS can be also selected/unselected
            Dim listOfDifElem As List(Of String)
            listOfDifElem = (From a In myWorkSessionResultDS.Patients _
                            Where a.SampleClass = "PATIENT" _
                          AndAlso a.OTStatus = "OPEN" _
                          AndAlso (a.TestType = "STD" OrElse a.TestType = "ISE") _
                          AndAlso a.Selected = False _
                          AndAlso a.LISRequest = True _
                           Select String.Format("{0}|{1}|{2}", a.TestType, a.TestID, a.SampleType) Distinct).ToList()

            Dim myOrderTestsDelegate As New OrderTestsDelegate
            For Each difElement As String In listOfDifElem
                Dim elements As String() = difElement.Split(CChar("|"))

                'Unselect Controls used for the same SampleType/TestID
                Dim lstWSControlsS As List(Of WorkSessionResultDS.ControlsRow)
                lstWSControlsS = (From a In myWorkSessionResultDS.Controls _
                                     Where a.SampleClass = "CTRL" _
                                   AndAlso a.TestType = elements(0) _
                                   AndAlso a.SampleType = elements(2) _
                                   AndAlso a.TestID = Convert.ToInt32(elements(1)) _
                                   AndAlso a.OTStatus = "OPEN"
                                    Select a).ToList

                If (lstWSControlsS.Count > 0) Then
                    For Each ctrl As WorkSessionResultDS.ControlsRow In lstWSControlsS
                        ctrl.Selected = False
                    Next
                    myWorkSessionResultDS.Controls.AcceptChanges()
                End If

                'Unselect the Calibrator and Blank if it is possible
                If (elements(0) = "STD") Then UnselectBlkCalibrator("STD", elements(2), Convert.ToInt32(elements(1)))
            Next

            'Verify if related elements for Controls requested by LIS can be also selected/unselected
            listOfDifElem = (From a In myWorkSessionResultDS.Controls _
                            Where a.SampleClass = "CTRL" _
                          AndAlso a.OTStatus = "OPEN" _
                          AndAlso a.TestType = "STD" _
                          AndAlso a.Selected = False _
                          AndAlso a.LISRequest = True _
                           Select String.Format("{0}|{1}|{2}", a.TestType, a.TestID, a.SampleType) Distinct).ToList()

            For Each difElement As String In listOfDifElem
                Dim elements As String() = difElement.Split(CChar("|"))

                'Unselect the Calibrator and Blank if it is possible
                UnselectBlkCalibrator("STD", elements(2), Convert.ToInt32(elements(1)))
            Next
            listOfDifElem = Nothing

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".Prueba", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".Prueba", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Events"

    '******************************************
    '* EVENTS FOR BLANKS AND CALIBRATORS GRID *
    '******************************************

    ''' <summary>
    ''' Manages changes in editable cells in the grid of Blanks and Calibrators: Tube Type, Selected Check and/or New Check  
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsBlkCalibDataGridView_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsBlkCalibDataGridView.CellValueChanged
        Try
            If (SavingWS) Then Return
            Select Case (bsBlkCalibDataGridView.Columns(e.ColumnIndex).Name)
                Case "TubeType"
                    ChangeBlankCalibratorTubeTypeColumn()

                Case "Selected"
                    ChangeBlankCalibratorSelectedColumn()

                Case "NewCheck"
                    ChangeBlankCalibratorNewCheckColumn()
            End Select

            'Set the Wrap mode of cell AbsValue for multipoint Calibrators having previous results
            SetWrapCells()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsBlkCalibDataGridView_CellValueChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsBlkCalibDataGridView_CellValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsBlkCalibDataGridView_CellMouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsBlkCalibDataGridView.CellMouseUp
        Try
            If (SavingWS) Then Return
            BlkCalibOrderTestsCellMouseUp()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsBlkCalibDataGridView_CellMouseUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsBlkCalibDataGridView_CellMouseUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsBlkCalibDataGridView_CurrentCellDirtyStateChanged(ByVal sender As Object, ByVal e As EventArgs) Handles bsBlkCalibDataGridView.CurrentCellDirtyStateChanged
        Try
            If (SavingWS) Then Return
            ChangeBlkCalibSelectedState()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsBlkCalibDataGridView_CurrentCellDirtyStateChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsBlkCalibDataGridView_CurrentCellDirtyStateChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    '''              AG 30/05/2013 - also add condition If (SavingWS) Then Return 'Bug #1102
    ''' </remarks>
    Private Sub bsBlkCalibDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles bsBlkCalibDataGridView.RowPostPaint
        Try
            'RH 08/05/2012 Bugtracking 544
            If (Not RowPostPaintEnabled) Then Return
            If (SavingWS) Then Return 'Bug #1102
            PostPaintBlkCalibratorRow()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsBlkCalibDataGridView_RowPostPaint", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsBlkCalibDataGridView_RowPostPaint", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsBlkCalibDataGridView_RowsAdded(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowsAddedEventArgs) Handles bsBlkCalibDataGridView.RowsAdded
        Try
            If (SavingWS) Then Return
            CheckAddRemoveBlkCalRows()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsBlkCalibDataGridView_RowsAdded", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsBlkCalibDataGridView_RowsAdded", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When 'Supr' key is pressed having at least a selected row in the grid of Blanks and Calibrators, function 
    ''' DeleteBlankCalibrators is called to delete selected rows
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 10/03/2010
    ''' Modified by: RH 29/04/2010 - DEL Key should do exactly the same work as bsDelCalibratorsButton button, so we call
    '''                              the bsDelCalibratorsButton handle but only in case bsDelCalibratorsButton is Enabled.
    ''' </remarks>
    Private Sub bsBlkCalibDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsBlkCalibDataGridView.KeyDown
        Try
            If (SavingWS) Then Return
            If (e.KeyCode = Keys.Delete) Then
                If bsDelCalibratorsButton.Enabled Then bsDelCalibratorsButton_Click(Nothing, Nothing)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsBlkCalibDataGridView_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsBlkCalibDataGridView_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Format the informed CalibrationFactor using the number of allowed decimals
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 04/09/2012
    ''' </remarks>
    Private Sub bsBlkCalibDataGridView_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles bsBlkCalibDataGridView.CellFormatting
        Try
            If (SavingWS) Then Return
            If (e.RowIndex >= 0) Then
                If (e.ColumnIndex = bsBlkCalibDataGridView.Columns("FactorValue").Index) AndAlso _
                   (Not bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("FactorValue").Value Is DBNull.Value) Then
                    e.Value = Convert.ToDouble(bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("FactorValue").Value).ToString(CalibFactorFormat)

                ElseIf (e.ColumnIndex = bsBlkCalibDataGridView.Columns("OriginalFactorValue").Index) AndAlso _
                       (Not bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("OriginalFactorValue").Value Is DBNull.Value) Then
                    e.Value = Convert.ToDouble(bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("OriginalFactorValue").Value).ToString(CalibFactorFormat)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsBlkCalibDataGridView_CellFormatting", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsBlkCalibDataGridView_CellFormatting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Before begin to edit the Calibration Factor Value, save the current value in a global variable, to allow restoring it in case this is needed
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 30/11/2010 - If the click is in an ABSValue cell with Strike Font, then value of the original Calibrator Factor is recovered
    '''              TR 29/11/2011 - Validate the Row Index has to be greater or equal to Zero (0) to avoid Index out of range error.
    ''' </remarks>
    Private Sub bsBlkCalibDataGridView_CellMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsBlkCalibDataGridView.CellMouseClick
        Try
            If (SavingWS) Then Return
            If (e.RowIndex >= 0) Then
                If (e.ColumnIndex = bsBlkCalibDataGridView.Columns("ABSValue").Index) Then
                    If (bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("ABSValue").Style.Font Is StrikeFont) Then
                        bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("ABSValue").Style.Font = RegularFont
                        bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("FactorValue").Value = bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("OriginalFactorValue").Value
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsBlkCalibDataGridView_CellMouseClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsBlkCalibDataGridView_CellMouseClick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' If the value entered as Calibrator Factor Value is empty or zero, restore the previous value. Changes the Font of the correspondent
    ''' ABSValue to Strike when the Original Factor Value is changed
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsBlkCalibDataGridView_CellValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles bsBlkCalibDataGridView.CellValidating
        Try
            If (SavingWS) Then Return
            If (e.ColumnIndex = bsBlkCalibDataGridView.Columns("FactorValue").Index) AndAlso (Not bsBlkCalibDataGridView.CurrentCell.ReadOnly) Then
                If (Not IsNumeric(e.FormattedValue)) Then
                    bsBlkCalibDataGridView.CancelEdit()
                Else
                    Dim currentValue As Double = Convert.ToDouble(e.FormattedValue)
                    If (currentValue = 0) Then
                        bsBlkCalibDataGridView.CancelEdit()
                    Else
                        If (currentValue.ToString(CalibFactorFormat) <> Convert.ToDouble(bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("OriginalFactorValue").Value).ToString(CalibFactorFormat)) Then
                            bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("ABSValue").Style.Font = StrikeFont
                        Else
                            bsBlkCalibDataGridView.Rows(e.RowIndex).Cells("ABSValue").Style.Font = RegularFont
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsBlkCalibDataGridView_CellValidating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsBlkCalibDataGridView_CellValidating ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When a Calibrator Factor is changed, format value to shown a maximum of 4 decimals
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsBlkCalibDataGridView_CellValidated(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsBlkCalibDataGridView.CellValidated
        Try
            If (SavingWS) Then Return
            If (bsBlkCalibDataGridView.Columns(e.ColumnIndex).Name = "FactorValue") AndAlso _
               (Not bsBlkCalibDataGridView.CurrentRow.Cells("FactorValue").ReadOnly) AndAlso _
               (Not bsBlkCalibDataGridView.CurrentRow.Cells("FactorValue").Value Is DBNull.Value) Then
                Dim myValue As String = Convert.ToDouble(bsBlkCalibDataGridView.CurrentRow.Cells("FactorValue").FormattedValue).ToString(CalibFactorFormat)
                bsBlkCalibDataGridView.CurrentRow.Cells("FactorValue").Value = myValue
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsBlkCalibDataGridView_CellValidated", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsBlkCalibDataGridView_CellValidated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When a Calibrator Factor cell is beign edited, adds the handler to control the key pressed is allowed  
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 
    ''' Modified by: SA 12/01/2011 - When the editing cell is the one for Number of Replicates, raise the event
    '''                              to avoid entering decimal separators and/or minus sign
    '''              SA 22/05/2012 - Disable the context menu that appear by default when the right mouse button is
    '''                              clicked in a cell that is in edition mode
    ''' </remarks>
    Private Sub bsBlkCalibDataGridView_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles bsBlkCalibDataGridView.EditingControlShowing
        Try
            If (SavingWS) Then Return
            If (bsBlkCalibDataGridView.Columns(bsBlkCalibDataGridView.CurrentCell.ColumnIndex).Name = "FactorValue") AndAlso _
               (bsBlkCalibDataGridView.IsCurrentCellInEditMode) Then
                DirectCast(e.Control, DataGridViewTextBoxEditingControl).ShortcutsEnabled = False

                If (bsBlkCalibDataGridView.SelectedRows.Count = 1) Then
                    If (Not myFactorTextBox Is Nothing) Then
                        RemoveHandler myFactorTextBox.KeyPress, AddressOf CheckNumericCell
                        myFactorTextBox = Nothing
                    End If

                    myFactorTextBox = CType(e.Control, TextBox)
                    AddHandler myFactorTextBox.KeyPress, AddressOf CheckNumericCell

                    ChangesMadeAttribute = True
                Else
                    bsBlkCalibDataGridView.CancelEdit()
                    bsBlkCalibDataGridView.CurrentCell.ReadOnly = True
                End If
            ElseIf (bsBlkCalibDataGridView.Columns(bsBlkCalibDataGridView.CurrentCell.ColumnIndex).Name = "NumReplicates") AndAlso _
                   (bsBlkCalibDataGridView.IsCurrentCellInEditMode) Then
                Dim editingUpDownCell As NumericUpDown = CType(e.Control, NumericUpDown)
                If (Not editingUpDownCell Is Nothing) Then
                    editingUpDownCell.ContextMenuStrip = New ContextMenuStrip

                    RemoveHandler editingUpDownCell.KeyPress, AddressOf editingGridUpDown_KeyPress
                    AddHandler editingUpDownCell.KeyPress, AddressOf editingGridUpDown_KeyPress
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsBlkCalibDataGridView_EditingControlShowing", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsBlkCalibDataGridView_EditingControlShowing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ' XB 26/08/2014 - rename bsAllBlkCalCheckBox as bsAllBlanksCheckBox - BT #1868
    Private Sub bsAllBlanksCheckBox_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsAllBlanksCheckBox.MouseClick
        Try
            ClickAllBlanksCheckBox()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsAllBlanksCheckBox_MouseClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsAllBlanksCheckBox_MouseClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' bsAllCalibsCheckBox event click
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by XB 26/08/2014 - BT #1868
    ''' </remarks>
    Private Sub bsAllCalibsCheckBox_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsAllCalibsCheckBox.MouseClick
        Try
            ClickAllCalibsCheckBox()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsAllCalibsCheckBox_MouseClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsAllCalibsCheckBox_MouseClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsDelCalibratorsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDelCalibratorsButton.Click
        Try
            If (bsBlkCalibDataGridView.SelectedRows.Count > 0) Then DeleteBlankCalibrators()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsDelCalibratorsButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsDelCalibratorsButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    '******************************************
    '* EVENTS FOR CONTROLS GRID               *
    '******************************************
    ''' <summary>
    ''' Manages changes in editable cells in the grid of Controls: Tube Type, Number of Replicates and/or Selected Check 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bsControlOrdersDataGridView_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsControlOrdersDataGridView.CellValueChanged
        Try
            If (SavingWS) Then Return
            Select Case (bsControlOrdersDataGridView.Columns(e.ColumnIndex).Name)
                Case "TubeType"
                    ChangeControlTubeTypeColumn()

                Case "NumReplicates"
                    ChangeControlNumReplicatesColumn()

                Case "Selected"
                    ChangeControlSelectedColumn()
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsControlOrdersDataGridView_CellValueChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsControlOrdersDataGridView_CellValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsControlOrdersDataGridView_CellMouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsControlOrdersDataGridView.CellMouseUp
        Try
            If (SavingWS) Then Return
            ControlOrderTestsCellMouseUp()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsControlOrdersDataGridView_CellMouseUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsControlOrdersDataGridView_CellMouseUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsControlOrdersDataGridView_CurrentCellDirtyStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsControlOrdersDataGridView.CurrentCellDirtyStateChanged
        Try
            If (SavingWS) Then Return
            ChangeControlSelectedState()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsControlOrdersDataGridView_CurrentCellDirtyStateChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsControlOrdersDataGridView_CurrentCellDirtyStateChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To avoid entering decimal separators and/or minus sign in the cell for Number of Replicates
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 12/01/2011
    ''' Modified by: SA 22/05/2012 - Disable the context menu that appear by default when the right mouse button is
    '''                              clicked in a cell that is in edition mode
    ''' </remarks>
    Private Sub bsControlOrdersDataGridView_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles bsControlOrdersDataGridView.EditingControlShowing
        Try
            If (SavingWS) Then Return
            If (bsControlOrdersDataGridView.Columns(bsControlOrdersDataGridView.CurrentCell.ColumnIndex).Name = "NumReplicates") AndAlso _
               (bsControlOrdersDataGridView.IsCurrentCellInEditMode) Then
                Dim editingUpDownCell As NumericUpDown = CType(e.Control, NumericUpDown)
                If (Not editingUpDownCell Is Nothing) Then
                    editingUpDownCell.ContextMenuStrip = New ContextMenuStrip

                    RemoveHandler editingUpDownCell.KeyPress, AddressOf editingGridUpDown_KeyPress
                    AddHandler editingUpDownCell.KeyPress, AddressOf editingGridUpDown_KeyPress
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsControlOrdersDataGridView_EditingControlShowing", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsControlOrdersDataGridView_EditingControlShowing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When 'Supr' key is pressed having at least a selected row in the grid of Controls, function 
    ''' DeleteControls is called to delete selected rows
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 10/03/2010
    ''' Modified by: RH 04/29/2010 - DEL Key should do exactly the same work as bsDelControlsButton button,
    '''                              so we call the bsDelControlsButton handle but only in case bsDelControlsButton is Enabled.
    ''' </remarks>
    Private Sub bsControlOrdersDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsControlOrdersDataGridView.KeyDown
        Try
            If (SavingWS) Then Return
            If (e.KeyCode = Keys.Delete) Then
                If bsDelControlsButton.Enabled Then bsDelControlsButton_Click(Nothing, Nothing)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsControlOrdersDataGridView_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsControlOrdersDataGridView_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    '''              AG 30/05/2013 - also add condition If (SavingWS) Then Return 'Bug #1102
    ''' </remarks>
    Private Sub bsControlOrdersDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles bsControlOrdersDataGridView.RowPostPaint
        Try
            'RH 08/05/2012 Bugtracking 544
            If (Not RowPostPaintEnabled) Then Return
            If (SavingWS) Then Return 'Bug #1102
            PostPaintControlRow()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsControlOrdersDataGridView_RowPostPaint", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsControlOrdersDataGridView_RowPostPaint", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsControlOrdersDataGridView_RowsAdded(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowsAddedEventArgs) Handles bsControlOrdersDataGridView.RowsAdded
        Try
            If (SavingWS) Then Return
            CheckAddRemoveControlsRows()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsControlOrdersDataGridView_RowsAdded", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsControlOrdersDataGridView_RowsAdded", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsAllCtrlsCheckBox_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsAllCtrlsCheckBox.MouseClick
        Try
            ClickAllControlCheckBox()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsAllCtrlsCheckBox_MouseClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsAllCtrlsCheckBox_MouseClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsDelControlsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDelControlsButton.Click
        Try
            If bsControlOrdersDataGridView.SelectedRows.Count > 0 Then DeleteControls()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsDelControlsButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsDelControlsButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '******************************************
    '* EVENTS FOR PATIENTS GRID               *
    '******************************************

    Private Sub bsPatientOrdersDataGridView_CellMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsPatientOrdersDataGridView.CellMouseDoubleClick
        Try
            If (SavingWS) Then Return
            If (bsPatientOrdersDataGridView.Rows.Count > 0) Then
                Dim columnClicked As Integer = bsPatientOrdersDataGridView.CurrentCell.ColumnIndex

                'If the double-click was in the Stat cell...
                If (bsPatientOrdersDataGridView.Columns(columnClicked).Name = "SampleClassIcon") Then
                    ChangePatientOrderPriority()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsPatientOrdersDataGridView_CellMouseDoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsPatientOrdersDataGridView_CellMouseDoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages changes in editable cells in the grid of Patients: SampleID, Tube Type, and/or Selected Check 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/03/2010
    ''' </remarks>
    Private Sub bsPatientOrdersDataGridView_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsPatientOrdersDataGridView.CellValueChanged
        Try
            If (SavingWS) Then Return
            Select Case bsPatientOrdersDataGridView.Columns(e.ColumnIndex).Name
                Case "SampleID"
                    'To avoid double execution of ChangePatientSampleIDColumn
                    If (Not patientCaseChanged) Then ChangePatientSampleIDColumn()

                Case "TubeType"
                    ChangePatientTubeTypeColumn()

                Case "NumReplicates"
                    ChangePatientNumReplicatesColumn()

                Case "Selected"
                    ChangePatientSelectedColumn()
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsPatientOrdersDataGridView_CellValueChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsPatientOrdersDataGridView_CellValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsPatientOrdersDataGridView_CellMouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsPatientOrdersDataGridView.CellMouseUp
        Try
            If (SavingWS) Then Return
            PatientOrderTestsCellMouseUp()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsPatientOrdersDataGridView_CellMouseUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsPatientOrdersDataGridView_CellMouseUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When 'Supr' key is pressed having at least a selected row in the grid of Patients, function 
    ''' DeletePatients is called to delete selected rows
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 10/03/2010
    ''' Modified by: RH 04/29/2010 - DEL Key should do exactly the same work as bsDelPatientsButton button, so we call 
    '''                              the bsDelPatientsButton handle but only in case bsDelPatientsButton is Enabled
    ''' </remarks>
    Private Sub bsPatientOrdersDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsPatientOrdersDataGridView.KeyDown
        Try
            If (SavingWS) Then Return
            If (e.KeyCode = Keys.Delete) Then
                If bsDelPatientsButton.Enabled Then bsDelPatientsButton_Click(Nothing, Nothing)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsPatientOrdersDataGridView_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsPatientOrdersDataGridView_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsPatientOrdersDataGridView_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsPatientOrdersDataGridView.KeyUp
        Try
            If (SavingWS) Then Return
            If (bsPatientOrdersDataGridView.CurrentRow Is Nothing) OrElse _
               (bsPatientOrdersDataGridView.CurrentRow.Index = myPatientRow) Then Return

            Select Case e.KeyCode
                Case Keys.Up
                    If (bsPatientOrdersDataGridView.CurrentRow.Index >= 0) Then
                        myPatientRow = -1
                        PatientOrderTestsCellMouseUp()
                    End If
                Case Keys.Down
                    If (bsPatientOrdersDataGridView.CurrentRow.Index < bsPatientOrdersDataGridView.Rows.Count) Then
                        myPatientRow = -1
                        PatientOrderTestsCellMouseUp()
                    End If
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsPatientOrdersDataGridView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsPatientOrdersDataGridView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsPatientOrdersDataGridView_CurrentCellDirtyStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsPatientOrdersDataGridView.CurrentCellDirtyStateChanged
        Try
            If (SavingWS) Then Return
            ChangePatientSelectedState()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsPatientOrdersDataGridView_CurrentCellDirtyStateChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsPatientOrdersDataGridView_CurrentCellDirtyStateChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by:  DL 04/03/2010
    ''' Modified by: RH 04/30/2010
    '''              SA 12/01/2011 - When the editing cell is the one for Number of Replicates, raise the event
    '''                              to avoid entering decimal separators and/or minus sign
    '''              SA 22/05/2012 - Disable the context menu that appear by default when the right mouse button is
    '''                              clicked in a cell that is in edition mode
    ''' </remarks>
    Private Sub bsPatientOrdersDataGridView_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles bsPatientOrdersDataGridView.EditingControlShowing
        Try
            If (SavingWS) Then Return
            If (bsPatientOrdersDataGridView.Columns(bsPatientOrdersDataGridView.CurrentCell.ColumnIndex).Name = "SampleID") AndAlso _
               (bsPatientOrdersDataGridView.IsCurrentCellInEditMode) Then
                DirectCast(e.Control, DataGridViewTextBoxEditingControl).ShortcutsEnabled = False

                If (bsPatientOrdersDataGridView.SelectedRows.Count = 1) Then
                    'Get PatientID/SampleID, StatFlag and SampleType of the clicked row in grid of Patient Order Tests
                    Dim mySampleID As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleID").Value.ToString()
                    Dim mySampleIDType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleIDType").Value.ToString()
                    Dim myStatFlag As Boolean = CBool(bsPatientOrdersDataGridView.CurrentRow.Cells("StatFlag").Value)
                    Dim mySampleType As String = bsPatientOrdersDataGridView.CurrentRow.Cells("SampleType").Value.ToString()

                    bsStatCheckbox.Checked = myStatFlag
                    bsSampleTypeComboBox.SelectedValue = mySampleType
                    sampleIDType = mySampleIDType

                    'For ChangeSampleIDColumn
                    textBoxOriginal = mySampleID

                    HeaderStatusForPatients(True)

                    'For AUTO SampleIDs, it is needed that this sentence be here, after calling HeaderStatusForPatients
                    bsPatientIDTextBox.Text = mySampleID

                    'Needed for next call to PatientOrderTestsCellMouseUp()
                    myPatientRow = -1
                    myCountPatientClicks = 1
                    patientEventSource = "Left_Click_Enabled"
                Else
                    bsPatientOrdersDataGridView.CancelEdit()
                    bsPatientOrdersDataGridView.CurrentCell.ReadOnly = True
                End If

            ElseIf (bsPatientOrdersDataGridView.Columns(bsPatientOrdersDataGridView.CurrentCell.ColumnIndex).Name = "NumReplicates") AndAlso _
                   (bsPatientOrdersDataGridView.IsCurrentCellInEditMode) Then
                Dim editingUpDownCell As NumericUpDown = CType(e.Control, NumericUpDown)
                If (Not editingUpDownCell Is Nothing) Then
                    editingUpDownCell.ContextMenuStrip = New ContextMenuStrip

                    RemoveHandler editingUpDownCell.KeyPress, AddressOf editingGridUpDown_KeyPress
                    AddHandler editingUpDownCell.KeyPress, AddressOf editingGridUpDown_KeyPress
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsPatientOrdersDataGridView_EditingControlShowing", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsPatientOrdersDataGridView_EditingControlShowing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsPatientOrdersDataGridView_RowsAdded(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowsAddedEventArgs) Handles bsPatientOrdersDataGridView.RowsAdded
        Try
            If (SavingWS) Then Return
            CheckAddRemovePatientsRows()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsPatientOrdersDataGridView_RowsAdded", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsPatientOrdersDataGridView_RowsAdded", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    '''              AG 30/05/2013 - also add condition If (SavingWS) Then Return 'Bug #1102
    ''' </remarks>
    Private Sub bsPatientOrdersDataGridView_RowPostPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowPostPaintEventArgs) Handles bsPatientOrdersDataGridView.RowPostPaint
        Try
            'RH 08/05/2012 Bugtracking 544
            If Not RowPostPaintEnabled Then Return
            If (SavingWS) Then Return 'Bug #1102
            PostPaintPatientRow()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsPatientOrdersDataGridView_RowPostPaint", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsPatientOrdersDataGridView_RowPostPaint", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsAllPatientsCheckBox_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsAllPatientsCheckBox.MouseClick
        Try
            ClickAllPatientCheckBox()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsAllPatientsCheckBox_MouseClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsAllPatientsCheckBox_MouseClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsDelPatientsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDelPatientsButton.Click
        Try
            If (bsPatientOrdersDataGridView.SelectedRows.Count > 0) Then DeletePatients()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsDelPatientsButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "bsDelPatientsButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '******************************************
    '* EVENTS FOR SCREEN BUTTONS              *
    '******************************************
    ''' <summary>
    ''' Open the auxiliary screen that allow select/unselect Tests according the selected Sample Class and Sample Type
    ''' </summary>
    Private Sub bsSearchTests_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSearchTestsButton.Click
        Try
            SearchTests()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSearchTests_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSearchTests_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' If a Patient/Sample has been informed, validate if it corresponds to a Patient that exists in DB. If no Patient/Sample
    ''' has been informed, or the informed is not an existing one, then open the Patients Programming screen 
    ''' </summary>
    Private Sub bsPatientSearchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsPatientSearchButton.Click
        Try
            SearchPatient()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsPatientSearchButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsPatientSearchButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow to save the current WorkSession
    ''' </summary>
    Private Sub bsSaveWSButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSaveWSButton.Click
        Try
            SaveWorkSession()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSaveWSButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSaveWSButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow selecting a SavedWS to load, and call the function to execute the loading
    ''' </summary>
    ''' <remarks>
    ''' Modified by: RH 19/10/2010 - Introduce the Using statement
    '''              SA 22/02/2011 - Inform the correspondent Screen Properties with values of ID and Name of the selected Saved WS
    ''' </remarks>
    Private Sub bsLoadWSButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsLoadWSButton.Click
        Try
            Using myWSSelection As New IWSLoadSaveAuxScreen
                'Assign the required properties of the auxiliary screen and open it as a DialogForm
                myWSSelection.ScreenUse = "SAVEDWS"
                myWSSelection.SourceButton = "LOAD"

                If (myWSSelection.ShowDialog() = Windows.Forms.DialogResult.OK) Then
                    WSLoadedIDAttribute = myWSSelection.IDProperty
                    WSLoadedNameAttribute = myWSSelection.NameProperty

                    LoadSavedWS()
                End If
            End Using
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsLoadWSButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsLoadWSButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow inform results for requested Off-System Tests
    ''' </summary>
    Private Sub bsOffSystemResultsButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsOffSystemResultsButton.Click
        Try
            OpenOffSystemResultsScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsOffSystemResultsButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsOffSystemResultsButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Execute the scanning of the Samples Rotor
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 26/09/2011
    ''' Modified by: SA 10/05/2012 - Once the scanning finishes, if there are incomplete Patient Samples, enable button
    '''                              BarCode Warnings
    '''              SA 16/10/2013 - BT #1334 ==> Changes due to new Analyzer mode PAUSE in RUNNING: the Scanning process will be called not only
    '''                              when the Analyzer is in STAND BY, but also when it is in RUNNING but stopped (PAUSE) - NOTE: this change has
    '''                              not been tested due to the option for LIS with files has been disabled from version 2.0.0
    ''' </remarks>
    Private Sub bsScanningButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsScanningButton.Click
        Try
            CreateLogActivity("Btn Scanning", Me.Name & ".bsScanningButton_Click", EventLogEntryType.Information, False) 'JV #1360 24/10/2013
            IAx00MainMDI.SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted) 'AG 10/07/2013
            If (Not mdiAnalyzerCopy Is Nothing) Then
                'Call the Barcode read process only if the Analyzer is connected and the Barcode is available
                If (mdiAnalyzerCopy.Connected AndAlso mdiAnalyzerCopy.BarCodeProcessBeforeRunning = AnalyzerManager.BarcodeWorksessionActions.BARCODE_AVAILABLE) Then
                    'Call the Barcode read process only if the Analyzer Status is STANDBY or if it is PAUSE in RUNNING
                    If (mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY) OrElse _
                       (mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso mdiAnalyzerCopy.AllowScanInRunning) Then
                        Cursor = Cursors.WaitCursor

                        IAx00MainMDI.DisabledMdiForms = Me
                        IAx00MainMDI.EnableButtonAndMenus(False)
                        IAx00MainMDI.SetActionButtonsEnableProperty(False)
                        IAx00MainMDI.ShowStatus(Messages.BARCODE_READING)
                        IAx00MainMDI.SinglecanningRequested = True 'AG 06/11/2013 - Task #1375 - Inform scanning requested from rotorposition screen started

                        'Disable the screen while the scanning is executed
                        Me.Enabled = False
                        ScreenWorkingProcess = True

                        IAx00MainMDI.InitializeMarqueeProgreesBar()
                        'Dim prevMessage As String = IAx00MainMDI.bsAnalyzerStatus.Text
                        Application.DoEvents()

                        Dim workingThread As New Threading.Thread(AddressOf ScanningBarCode)
                        workingThread.Start()

                        While ScreenWorkingProcess
                            IAx00MainMDI.InitializeMarqueeProgreesBar()
                            Application.DoEvents()
                        End While
                        workingThread = Nothing
                        IAx00MainMDI.StopMarqueeProgressBar()

                        IAx00MainMDI.ShowStatus(Messages._NONE)
                        IAx00MainMDI.SetActionButtonsEnableProperty(True)

                        'If there are incomplete Patient Samples, enable button BCWarnings
                        ValidateBCWarningButtonEnabled()
                    End If
                End If
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsScanningButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsScanningButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            IAx00MainMDI.SinglecanningRequested = False 'AG 06/11/2013 - Task #1375 - Inform scanning requested from rotorposition screen finished

            IAx00MainMDI.EnableButtonAndMenus(True)
            Cursor = Cursors.Default
        End Try

        Cursor = Cursors.Default
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen of Incomplete Patient Samples
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA 22/09/2011 - Verify if changes are needed in table of Incomplete Patient Samples
    '''              TR 04/04/2013 - NOT USED, THE BUTTON IS HIDE.
    '''              AG 07/01/2014 - BT #1436 protection, if recover results INPROCESS do not open the HQ barcode neither the incomplete samples screens
    ''' </remarks>
    Private Sub bsBarcodeWarningButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsBarcodeWarningButton.Click
        Try
            'AG 07/01/2013 - BT #1436 - put all code inside this IF
            If mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) <> "INPROCESS" Then
                'Verify if changes are needed in table of Incomplete Patient Samples
                Dim resultData As GlobalDataTO = Nothing
                Dim myBCPosWithNoRequestDelegate As New BarcodePositionsWithNoRequestsDelegate

                resultData = myBCPosWithNoRequestDelegate.UpdateRelatedIncompletedSamples(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, _
                                                                                          myWorkSessionResultDS, False)
                If (Not resultData.HasError) Then
                    'Open the auxiliary screen for Incomplete Patient Samples
                    Using myForm As New IWSIncompleteSamplesAuxScreen()
                        myForm.AnalyzerID = AnalyzerIDAttribute
                        myForm.WorkSessionID = WorkSessionIDAttribute
                        myForm.WorkSessionStatus = WSStatusAttribute
                        myForm.SourceScreen = SourceScreen.SAMPLE_REQUEST
                        myForm.WSOrderTests = myWorkSessionResultDS
                        myForm.StartPosition = FormStartPosition.CenterParent
                        myForm.ShowDialog()

                        If (myForm.DialogResult = Windows.Forms.DialogResult.OK) Then
                            myWorkSessionResultDS = myForm.WSOrderTests
                            WSStatusAttribute = myForm.WorkSessionStatus

                            ChangesMadeAttribute = myForm.ChangesMade
                        End If
                    End Using

                    'Update global variables in the main MDI Form
                    IAx00MainMDI.SetWSActiveData(AnalyzerIDAttribute, WorkSessionIDAttribute, WSStatusAttribute)
                Else
                    'Error updating the CompletedFlag; shown it
                    ShowMessage(Me.Name & ".bsBarcodeWarningButton_Click", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If

                'Verify if the button has to remains active
                ValidateBCWarningButtonEnabled()

            Else
                bsBarcodeWarningButton.Enabled = False
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsBarcodeWarningButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsBarcodeWarningButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Execute the Import from LIS process when the needed file exists in the path 
    ''' - NOT USED, THE BUTTON IS HIDE
    ''' </summary>
    Private Sub bsLIMSImportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsLIMSImportButton.Click
        Try
            ExecuteImportFromLIMSProcess()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsLIMSImportButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsLIMSImportButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen used to shown errors after execution of an Import from LIS process 
    ''' </summary>
    Private Sub bsLIMSErrorsButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsLIMSErrorsButton.Click
        Try
            ShowLIMSImportErrors()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsLIMSErrorsButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsLIMSErrorsButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save the WorkSession and sent to positioning all selected Order Tests. Finally, open the screen of Rotor Positioning
    ''' </summary>
    Private Sub bsOpenRotor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsOpenRotorButton.Click
        Try

            'AG 24/02/2014 - use parameter MAX_APP_MEMORYUSAGE into performance counters (but do not show message here!) - ' XB 18/02/2014 BT #1499
            Dim PCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage) 'AG 24/02/2014 - #1520 add parameter
            PCounters.GetAllCounters()
            PCounters = Nothing
            ' XB 18/02/2014 BT #1499

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            IsOpenRotor = True
            SaveWSWithPositioning()

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("IWSampleRequest Send to Position And Close (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IWSampleRequest.bsOpenRotor_Click", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsOpenRotor_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsOpenRotor_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save the WorkSession but without sent selected Order Tests to positioning. Finally, the screen is closed
    ''' </summary>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try

            'AG 24/02/2014 - use parameter MAX_APP_MEMORYUSAGE into performance counters (but do not show message here!) - ' XB 18/02/2014 BT #1499
            Dim PCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage) 'AG 24/02/2014 - #1520 add parameter
            PCounters.GetAllCounters()
            PCounters = Nothing
            ' XB 18/02/2014 BT #1499

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            SaveWSWithoutPositioning()

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("IWSampleRequest Save Without POS WS And Close (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IWSampleRequest.bsExitButton_Click", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    '******************************************
    '* SCREEN EVENTS                          *
    '******************************************

    'Private Sub WS_Preparation_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
    '    Try
    '        ReleaseElements()
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".WS_Preparation_FormClosed", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".WS_Preparation_FormClosed", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub

    Private Sub WS_Preparation_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            ScreenLoad()

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("IWSampleRequest.LOAD (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IWSampleRequest.WS_Preparation_Load", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".WS_Preparation_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".WS_Preparation_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010
    ''' Modified by: RH 30/06/2011 - Escape key should do exactly the same operations as bsExitButton_Click()
    ''' </remarks>
    Private Sub WS_Preparation_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then bsAcceptButton.PerformClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".WS_Preparation_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".WS_Preparation_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Show Warning Message for LIMS Import file pending to load when the correspondent button is enabled and the screen has
    ''' not been opened by selecting the LIMS Import option in the WorkSession submenu of the Main MDI Form; in this last case
    ''' the Import from LIMS process is executed by calling function ExecuteImportFromLIMSProcess
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/09/2010 
    ''' Modified by: SA 22/02/2011 - When attribute WSLoadedIDAttribute is informed, it means a SavedWS to load was selected before
    '''                              opening the screen
    ''' </remarks>
    Private Sub WS_Preparation_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            If (WSLoadedIDAttribute <> -1) Then
                LoadSavedWS()
            Else
                If (Not OpenForLIMSImportAttribute) Then
                    'SA 16/09/2011 -Temporary commented due to the lims import process is executed in the incomplete patients samples Form.
                    'If (bsLIMSImportButton.Enabled) Then
                    '    ShowMessage(Name & ".WS_Preparation_Shown", GlobalEnumerates.Messages.LIMS_FILE_PENDING_TO_IMPORT.ToString, "", Me)
                    '    bsLIMSImportButton.Focus()
                    'End If
                    'SA 16/09/2011 -END
                Else
                    ExecuteImportFromLIMSProcess()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".WS_Preparation_Shown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".WS_Preparation_Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if the character '#' is introduced as first character in cell SampleID in Patients' grid, and in this case, remove it
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 06/04/2010
    ''' Modified by: SA 09/04/2010 - Verify if the grid of Patients has rows to avoid errors.  Additionally, the character # is forbbiden 
    '''                              only as first character in a SampleID manually informed. Added Try/Catch.
    '''              SA 21/04/2010 - Character # is forbbiden in any editable control in this screen
    ''' </remarks>
    Private Sub WS_Preparation_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
        Try
            If (e.KeyChar = CChar("#")) Then e.Handled = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".WS_Preparation_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".WS_Preparation_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '******************************************
    '* EVENTS FOR OTHER SCREEN CONTROLS       *
    '******************************************
    Private Sub bsSampleClassComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSampleClassComboBox.SelectionChangeCommitted
        Try
            SampleClassSelectionChange()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSampleClassComboBox_SelectionChangeCommitted", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSampleClassComboBox_SelectionChangeCommitted", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsPatientIDTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPatientIDTextBox.TextChanged
        Try
            PatientIDChangedInTextBox()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsPatientIDTextBox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsPatientIDTextBox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Change the selected Sample Class when the selected Tab is changed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 16/03/2010
    ''' </remarks>
    Private Sub bsSampleClassesTabControl_Selected(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSampleClassesTabControl.SelectedPageChanged
        Try
            If (bsSampleClassesTabControl.SelectedTabPage Is PatientsTab) Then
                bsSampleClassComboBox.SelectedValue = "PATIENT"
            Else
                If (bsSampleClassComboBox.SelectedValue.ToString = "PATIENT") Then bsSampleClassComboBox.SelectedValue = "BLANK"
            End If
            bsSampleClassComboBox_SelectionChangeCommitted(sender, e)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSampleClassesTabControl_Selected", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSampleClassesTabControl_Selected", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To avoid entered following characters in the Numeric Up/Down for Num of Orders:
    '''   ** Minus sign
    '''   ** Dot, Apostrophe or Comma as decimal point
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 09/07/2010
    ''' </remarks>
    Private Sub bsNumOrdersNumericUpDown_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles bsNumOrdersNumericUpDown.KeyPress
        Try
            If (e.KeyChar = CChar("-") OrElse e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",") OrElse e.KeyChar = "'") Then e.Handled = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsNumOrdersNumericUpDown_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsNumOrdersNumericUpDown_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' To avoid entering decimal separators in the NumericUpDown cells used for Number of Replicates in the screen grids
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 12/01/2011
    ''' </remarks>
    Private Sub editingGridUpDown_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Try
            If (e.KeyChar = "." OrElse e.KeyChar = "," OrElse e.KeyChar = "'" OrElse e.KeyChar = "-") Then
                e.Handled = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".editingGridUpDown_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".editingGridUpDown_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "NOT USED"
    ''' <summary>
    ''' Verify if button for Import from LIMS can be enabled - NOT USED, button is hide
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/09/2010
    ''' </remarks>
    Private Sub LIMSImportButtonEnabled()
        Try
            bsLIMSImportButton.Enabled = (IO.File.Exists(importFile))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LIMSImportButtonEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LIMSImportButtonEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manage the loading of a selected Saved WorkSession. For events bsLoadWSButton_Click and/or
    ''' Screen loading when the form is opened from menu option Load WorkSession in the main MDI
    ''' </summary>
    ''' <param name="pSavedWSID">Identifier of the Saved WS selected to load</param>
    ''' <param name="pSavedWSName">Name of the Saved WS selected to load</param>
    ''' <remarks>
    ''' Created by:  GDS 08/04/2010
    ''' Modified by: GDS 12/04/2010 - Added bind code and buttons enable control
    '''              SA  28/05/2010 - Removed opening of the auxiliary screen (code moved to the event) to allow reusing
    '''                               this function when the load is executed in the screen load
    '''              SG 09/07/2010  - If the WS has not any element show message and delete the WS
    '''              SA 12/07/2010  - Deletion of the empty WS was moved to the Delegate due to it has to be executed
    '''                               in the same DB Transaction of the elements deletion
    ''' </remarks>
    Private Sub LoadWorkSession(ByVal pSavedWSID As Integer, ByVal pSavedWSName As String)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myOrderTests As New OrderTestsDelegate

            Dim executeLoad As Boolean = True
            If (bsPatientOrdersDataGridView.Rows.Count > 0 OrElse bsControlOrdersDataGridView.Rows.Count > 0 OrElse _
                bsBlkCalibDataGridView.Rows.Count > 0 OrElse Not String.IsNullOrEmpty(ActiveWorkSession)) Then
                executeLoad = (ShowMessage(bsPrepareWSLabel.Text, GlobalEnumerates.Messages.REPLACE_SAMPLES_WITH_SAVED_WS.ToString, , Me) = Windows.Forms.DialogResult.Yes)
            End If

            If (executeLoad) Then
                Cursor = Cursors.WaitCursor

                myGlobalDataTO = myOrderTests.LoadFromSavedWS(Nothing, pSavedWSID, AnalyzerIDAttribute)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myWorkSessionResultDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)
                    WSLoadedNameAttribute = pSavedWSName

                    BindDSToGrids()

                    OpenRotorButtonEnabled()
                    SaveLoadWSButtonsEnabled()
                    'LISImportButtonEnabled()

                    'When the selected Saved WS could not be fully loaded (definition of some Tests have been changed) 
                    'or could not be loaded, the warning is shown
                    If (myGlobalDataTO.ErrorCode <> "") Then
                        Cursor = Cursors.Default
                        ShowMessage(Name & ".LoadWorkSession", myGlobalDataTO.ErrorCode, , Me)
                    End If
                    ChangesMadeAttribute = True
                Else
                    'Error loading the selected WS, show it
                    Cursor = Cursors.Default
                    ShowMessage(Name & ".LoadWorkSession", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If

                Cursor = Cursors.Default
            Else
                WSLoadedIDAttribute = -1
                WSLoadedNameAttribute = ""
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadWorkSession", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadWorkSession", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' When some Patient Samples are deleted, search if there are Incomplete Patient Samples marked as completed to mark them as incompleted
    ''' When some Patient Samples are added manually, search if there are Incomplete Patient Samples marked as incomplete to mark them as completed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 20/09/2011
    ''' </remarks>
    Private Sub UpdateRelatedIncompletedSamples(ByVal pWithPositioning As Boolean)
        Try
            Dim resultData As GlobalDataTO = Nothing
            Dim myBCPosWithNoRequestDelegate As New BarcodePositionsWithNoRequestsDelegate

            resultData = myBCPosWithNoRequestDelegate.ReadByAnalyzerAndWorkSession(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "SAMPLES")
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim bcPosWithNoRequestDS As BarcodePositionsWithNoRequestsDS = DirectCast(resultData.SetDatos, BarcodePositionsWithNoRequestsDS)

                Dim myPID As String = String.Empty
                Dim lstRequestedPatients As List(Of WorkSessionResultDS.PatientsRow)

                For Each row As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In bcPosWithNoRequestDS.twksWSBarcodePositionsWithNoRequests
                    myPID = row.ExternalPID
                    If (Not row.IsPatientIDNull) Then myPID = row.PatientID

                    'Search if there are requested Tests for the Incompleted Sample (value of StatFlag is ignored due to the same Sample tube is used)
                    lstRequestedPatients = (From a In myWorkSessionResultDS.Patients _
                                           Where a.SampleClass = "PATIENT" _
                                         AndAlso a.SampleID = myPID _
                                         AndAlso a.SampleType = row.SampleType _
                                         AndAlso (a.TestType = "STD" OrElse a.TestType = "ISE") _
                                          Select a).ToList()

                    If (row.CompletedFlag AndAlso lstRequestedPatients.Count = 0) Then
                        'Tests have been removed, mark the Incomplete Sample as not Completed
                        row.CompletedFlag = False
                    ElseIf (Not row.CompletedFlag AndAlso lstRequestedPatients.Count > 0) Then
                        'Tests have been added, mark the Incomplete Sample as Completed
                        row.CompletedFlag = True
                    Else
                        'No update is needed; set Null in the DS to exclude that incomplete samples from the updation process
                        row.SetCompletedFlagNull()
                    End If
                    row.AcceptChanges()
                Next row

                'Update the affected records in table of Incompleted Patient Samples
                resultData = myBCPosWithNoRequestDelegate.UpdateCompletedFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, _
                                                                              bcPosWithNoRequestDS, pWithPositioning)
            End If

            If (resultData.HasError) Then
                'Show the error Message
                ShowMessage(Name & ".UpdateRelatedIncompletedSamples", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateRelatedIncompletedSamples", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpdateRelatedIncompletedSamples", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    'Private Sub RefreshGrids()
    '    SavingWS = True
    '    CreateOpenWS = True

    '    PrepareOrderTestsForWS()

    '    bsPatientOrdersDataGridView.DataSource = Nothing
    '    bsControlOrdersDataGridView.DataSource = Nothing
    '    bsBlkCalibDataGridView.DataSource = Nothing

    '    'Get the current Language from the current Application Session
    '    Dim currentLanguageGlobal As New GlobalBase
    '    Dim currentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

    '    'Get the list of Sample Tube Types (for the ComboBox column of TubeType in all the grids)
    '    Dim myGlobalDataTO As New GlobalDataTO
    '    myGlobalDataTO = GetSampleTubeTypes()
    '    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
    '        Dim lstSorted As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)
    '        lstSorted = DirectCast(myGlobalDataTO.SetDatos, List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow))

    '        InitializePatientGrid(lstSorted, currentLanguage)
    '        InitializeControlGrid(lstSorted, currentLanguage)
    '        InitializeBlkCalibGrid(lstSorted, currentLanguage)
    '        lstSorted = Nothing
    '    Else
    '        'Show the error Message
    '        ShowMessage(Name & ".ScreenLoad", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
    '    End If

    '    Dim myWSDelegate As New WorkSessionsDelegate

    '    myGlobalDataTO = myWSDelegate.GetOrderTestsForWS(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute)
    '    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
    '        myWorkSessionResultDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)
    '    Else
    '        'Error getting the list of previously requested Order Tests
    '        ShowMessage(Name & ".PRUEBA", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
    '    End If

    '    'Bind DS to the form grids
    '    BindDSToGrids()

    '    'Apply styles to special columns
    '    ApplyStylesToSpecialGridColumns()

    '    bsPatientOrdersDataGridView.Refresh()
    '    bsControlOrdersDataGridView.Refresh()
    '    bsBlkCalibDataGridView.Refresh()

    '    SavingWS = False
    'End Sub
#End Region

End Class
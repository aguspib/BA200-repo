Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.CommunicationsSwFw
Imports LIS.Biosystems.Ax00.LISCommunications

Public Class HQBarcode

#Region "Declarations"
    'Global variable to store value of General Setting containing the maximum number of Patient Order Tests that can be created
    Private maxPatientOrderTests As Integer = -1

    'Global variable to store the list of Tests selected using the auxiliary screen of Tests Searching
    Private mySelectedTestsDS As New SelectedTestsDS

    'Global variable for the full path of the Import from LIMS file
    'Private ReadOnly importFile As String = LIMSImportFilePath & LIMS_IMPORT_FILE_NAME

    'Global variable used to centered the screen
    Private myNewLocation As New Point

    'Global variable used to control the screen closing
    Private continueClosing As Boolean = True

    Private mdiAnalyzerCopy As AnalyzerManager
    Private mdiESWrapperCopy As ESWrapper 'AG 26/04/2013

    'JCID 25/03/2013 -Load the icons used for STAT and Normal  
    Dim STATIcon As Byte()
    Dim NORMALIcon As Byte()
    Dim anyRowWithError As Boolean = True
    Dim sampleTypes As DataTable
    Dim bsHQSelectAllCheckBx_EventEnabled As Boolean = True  ' Flag bsHQSelectAll CheckBox Event Changed enabled 
    Dim bsGridSelectedChanged_EventEnabled As Boolean = True  ' Flag Grid Selected Changed Event Changed enabled 
    Dim bsGridComboBoxCollapsed As Boolean = True

    Dim cboBxCell As New DataGridViewComboBoxColumn
    Dim errorBarcodeRepeated As String

    Dim bsStatCheckboxChecked As Boolean = False
    Dim bsStatCheckboxEnabled As Boolean = False
    Dim bsSearchTestsButtonEnabled As Boolean = False
    Dim bsSampleTypeComboBoxEnabled As Boolean = False
    Dim bsSampleTypeComboBoxSelectedValue As String = ""
    Dim bsSampleTypeComboBoxSelectedIndex As Integer = -1
    Dim bsCancelButtonEnabled As Boolean = False
    Dim bsSaveButtonEnabled As Boolean = False

    Dim previousValueStat As Boolean    'Useful to know previous value before enter sample edit
    'Dim hideRows As List(Of Integer)   'Useful to Hide Samples With Orders. 
    '                                    #BUG:1259 On version 2.1.1 the functionality requires that all 
    '                                    all samples (with or without orders) must been shown on the screen

    Dim clearSelection As Boolean

    Private currentLanguage As String 'SGM 29/05/2013

#End Region

#Region "Attributes"
    Private AnalyzerIDAttribute As String = ""
    Private WorkSessionIDAttribute As String = ""
    Private WSStatusAttribute As String = ""
    Private SourceScreenAttribute As GlobalEnumerates.SourceScreen
    Private WorkSessionResultDSAttribute As New WorkSessionResultDS
    Private OpenSelectedCellSamples As List(Of Integer)
    Private OpenByAutomaticProcessAttribute As Boolean = False    'AG 08/07/2013
    Private AutoWSCreationWithLISModeAttribute As Boolean = False 'XB 17/07/2013
    Private HQButtonUserClickAttribute As Boolean = False         'SA 25/07/2013
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
    ''' If a group of cells in Samples Rotor has been selected before opening this screen, these cells will appear  
    ''' selected by default in the grid
    ''' </summary>
    Public Property OpenSelectedCells() As List(Of Integer)
        Get
            Return OpenSelectedCellSamples
        End Get
        Set(ByVal value As List(Of Integer))
            OpenSelectedCellSamples = value
        End Set
    End Property

    ''' <summary>
    ''' AG 08/07/2013 - Indicates when the screen has been open by user (FALSE) or when by the automatic WS creation with LIS process (TRUE)
    ''' </summary>
    Public WriteOnly Property OpenByAutomaticProcess() As Boolean
        Set(ByVal value As Boolean)
            OpenByAutomaticProcessAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' XB 17/07/2013 - Auto WS process
    ''' </summary>
    Public WriteOnly Property AutoWSCreationWithLISMode() As Boolean
        Set(ByVal value As Boolean)
            AutoWSCreationWithLISModeAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' It indicates if a final User has clicked in HostQuery Button
    ''' </summary>
    Public ReadOnly Property HQButtonUserClick() As Boolean
        Get
            Return HQButtonUserClickAttribute
        End Get
    End Property
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Close screen business, moved to a method from the event code
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 09/07/2013
    ''' Modified by: SA 01/08/2013 - Due to new functionality for wait synchronously until LIS sends an answer for all queried Specimens (or until the maximum
    '''                              waiting time expires), manual changes are now saved individually when clicking in SAVE button, instead of saving all of them 
    '''                              here when the screen closes 
    ''' </remarks>
    Public Sub CloseScreen()
        Try
            continueClosing = True
            If (Not mySelectedTestsDS Is Nothing AndAlso mySelectedTestsDS.SelectedTestTable.Rows.Count > 0) OrElse _
               (bsSampleTypeComboBox.Enabled AndAlso Not bsSampleTypeComboBox.SelectedValue Is Nothing) OrElse _
               (previousValueStat <> bsStatCheckbox.Checked AndAlso (bsSearchTestsButton.Enabled OrElse bsStatCheckbox.Enabled)) Then
                'AG 09/07/2013 - Special mode for create WS from LIS with an unique click
                If (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute) Then
                    'Nothing
                Else
                    If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No) Then
                        continueClosing = False
                    End If
                End If
                'AG 09/07/2013
            End If

            If (continueClosing) Then
                'Dim myGlobalDataTO As New GlobalDataTO
                'Dim myWSDelegate As New WorkSessionsDelegate
                'myGlobalDataTO = myWSDelegate.PrepareOrderTestsForWS(Nothing, WorkSessionResultDSAttribute, False, WorkSessionIDAttribute, AnalyzerIDAttribute, WSStatusAttribute, True, False)

                ''Update status of WSStatusAttribute with content of the WS DS returned
                'If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                '    Dim myWS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)
                '    If (myWS.twksWorkSessions.Rows.Count = 1) Then WSStatusAttribute = myWS.twksWorkSessions(0).WorkSessionStatus
                'Else
                '    'Error saving changes in the active Work Session 
                '    ShowMessage(Name & ".SaveChanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                'End If

                Me.Close()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CloseScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CloseScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Cursor = Cursors.Default
    End Sub

    ''' <summary>
    ''' Refresh the Screen when 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC 
    ''' Modified by: SA 24/07/2013 - Do not execute the Refresh in following cases: when the screen has been opened during the
    '''                              process of automatic Work Session creation with LIS, or when the User has clicked in HQ Button
    '''                              and it is waiting for the LIS answer.
    '''              SA 25/07/2013 - Changed the function that get data to fill the grid: WSRotorContentByPositionDelegate.GetAllPatientTubesForHQ
    '''                              instead of BarcodePositionsWithNoRequestsDelegate.ReadByAnalyzerAndWorkSession
    ''' </remarks>
    Public Overrides Sub RefreshScreen(pRefreshEventType As List(Of UI_RefreshEvents), pRefreshDS As UIRefreshDS)
        Try
            'If the screen is closing, the Refresh is not executed
            If (isClosingFlag) Then Return

            'If the screen has been opened during the process of Automatic WS Creation with LIS, the Refresh is not executed
            If (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute) Then Return

            'If the user has clicked in HQ Button and the screen is waiting for the LIS answer, the Refresh is not executed
            If (HQButtonUserClickAttribute) Then Return

            Dim myGlobalDataTO As New GlobalDataTO
            Dim rotorPosDlg As New WSRotorContentByPositionDelegate
            myGlobalDataTO = rotorPosDlg.GetAllPatientTubesForHQ(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, False)

            'Dim myBarcodePositionsWithNoRequestsDelegate As New BarcodePositionsWithNoRequestsDelegate
            'myGlobalDataTO = myBarcodePositionsWithNoRequestsDelegate.ReadByAnalyzerAndWorkSession(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS = DirectCast(myGlobalDataTO.SetDatos, BarcodePositionsWithNoRequestsDS)

                'Rows on DB where LISStatus <> "ASKING"
                Dim cellNumberNotAskingDB As List(Of Integer) = (From A In myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows _
                                                                Where CType(A, BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow).LISStatus <> "ASKING"
                                                               Select CType(A, BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow).CellNumber).ToList()

                'Rows on GRID must UPDATE
                Dim gdVwRowsMustUpdate As List(Of Object) = (From A In bsIncompleteSamplesDataGridView.Rows _
                                                            Where CType(A, DataGridViewRow).Cells("LISStatus").Value.ToString = "ASKING" _
                                                          AndAlso cellNumberNotAskingDB.Contains(CType(CType(A, DataGridViewRow).Cells("CellNumber").Value, Integer)) _
                                                           Select A).ToList()

                For Each gdRow As Object In gdVwRowsMustUpdate
                    CType(gdRow, DataGridViewRow).Cells("LISStatus").Value = "PENDING"
                    RepaintGridRow(CType(gdRow, DataGridViewRow))
                Next
            End If

            If (bsIncompleteSamplesDataGridView.Enabled) Then
                bsIncompleteSamplesDataGridView_SelectionChanged(Nothing, Nothing)
                MyClass.LIMSImportButtonEnabled() 'SG 15/05/2013 - Evaluate availability of the HQ button
            Else
                bsLIMSImportButton.Enabled = False
                EditButton.Enabled = False
            End If
        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: JC 06/05/2013 - Copied from IBarCodeEdit
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                Dim mySize As Size = IAx00MainMDI.Size
                Dim myLocation As Point = IAx00MainMDI.Location
                If (Not Me.MdiParent Is Nothing) Then
                    mySize = Me.Parent.Size
                    myLocation = Me.Parent.Location
                End If

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Disable all action buttons (when the status of the current WS is ABORTED or when the screen is waiting 
    ''' for LIS Orders after a Host Query)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 24/07/2013
    ''' </remarks>
    Private Sub DisableAllFields()
        Try
            bsLIMSImportButton.Enabled = False
            EditButton.Enabled = False
            bsHQSellectAllCheckBx.Checked = False
            bsHQSellectAllCheckBx.Enabled = False
            bsIncompleteSamplesDataGridView.ReadOnly = True

            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DisableAllFields", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DisableAllFields", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Control availability of EDIT button and when it is enabled, also control availability of fields in area of 
    ''' Manual Entering of Samples Details
    ''' </summary>
    ''' <param name="pStatus">True to enable the EDIT button; otherwise, false</param>
    ''' <remarks>
    ''' Created by: JC 06/05/2013 
    ''' </remarks>
    Private Sub EditButtonEnabled(ByVal pStatus As Boolean)
        Try
            EditButton.Enabled = pStatus
            If (Not pStatus) Then EnterSamplesDetailsEnabled(pStatus)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EditButtonEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EditButtonEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Control availability of fields in area of Manual Entering of Samples Details
    ''' </summary>
    ''' <param name="pStatus">True to set enabled fields; false to disabled them</param>
    ''' <remarks>
    ''' Created by:  SA 13/09/2011
    ''' Modified by: SA 24/07/2013 - CheckBox for select all rows in the grid is disabled when the 
    '''                              status of the active Work Session is ABORTED
    ''' </remarks>
    Private Sub EnterSamplesDetailsEnabled(ByVal pStatus As Boolean)
        Try
            If (pStatus) Then
                bsStatCheckbox.Checked = bsStatCheckboxChecked
                bsStatCheckbox.Enabled = bsStatCheckboxEnabled
                bsSearchTestsButton.Enabled = bsSearchTestsButtonEnabled
                bsSampleTypeComboBox.Enabled = bsSampleTypeComboBoxEnabled
                bsCancelButton.Enabled = bsCancelButtonEnabled
                bsSaveButton.Enabled = bsSaveButtonEnabled

                bsIncompleteSamplesDataGridView.Enabled = False
                bsLIMSImportButton.Enabled = False
            Else
                bsSampleTypeComboBox.Enabled = pStatus
                bsSearchTestsButton.Enabled = pStatus
                bsStatCheckbox.Enabled = pStatus
                bsSaveButton.Enabled = pStatus
                bsCancelButton.Enabled = pStatus

                bsSampleTypeComboBox.SelectedIndex = -1
                mySelectedTestsDS.Clear()
                bsStatCheckbox.Checked = False
            End If

            bsHQSelectAllCheckBx_EventEnabled = False
            bsHQSellectAllCheckBx.Enabled = (Not pStatus AndAlso WSStatusAttribute <> "ABORTED")
            bsHQSelectAllCheckBx_EventEnabled = True
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
    '''              SA 28/05/2013 - This screen is not opened from IWSSamplesRequest; related code has been removed
    ''' </remarks>
    Private Sub FillMaxOrderTestValues(ByVal pMaxOrderTestsDS As MaxOrderTestsValuesDS)
        Try
            Dim resultData As GlobalDataTO
            Dim currentRequestedOrders As Integer = 0
            Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate

            resultData = myWSOrderTestsDelegate.CountPatientOrderTests(Nothing, WorkSessionIDAttribute)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                currentRequestedOrders = DirectCast(resultData.SetDatos, Integer)
            Else
                'Error getting the number of Patient OrderTests currently requested in the WorkSession
                ShowMessage(Name & ".FillMaxOrderTestValues", resultData.ErrorCode, resultData.ErrorMessage, Me)
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
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid 
    '''                              Regional Settings problems (Bugs tracking #1112)
    '''              SA 08/05/2013 - When Order Tests are found for the SpecimenID/opposite StatFlag/SampleType or for the PatientID/opposite StatFlag/SampleType, 
    '''                              get current value of fields SampleID and SampleIDType for them and load these values in the SelectedTestsDS to return
    ''' </remarks>
    Private Sub FillSelectedTestsForDifPriority(ByVal pPatientID As String, ByVal pStatFlag As Boolean, ByVal pSampleType As String, ByVal pBarCodeInfo As String, _
                                                ByRef pLockedTestsDS As SelectedTestsDS, ByRef pCurrentSelTestsDS As SelectedTestsDS)
        Try
            'Get all Tests requested for the same PatientID/SampleID, SampleType and the opposite StatFlag
            Dim lstWSPatientDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSPatientDS = (From a As WorkSessionResultDS.PatientsRow In WorkSessionResultDSAttribute.Patients _
                             Where a.SampleClass = "PATIENT" _
                           AndAlso a.SampleType = pSampleType _
                           AndAlso a.SpecimenID = pBarCodeInfo _
                           AndAlso a.StatFlag <> pStatFlag _
                            Select a).ToList()

            If (lstWSPatientDS.Count > 0) Then
                'Get all Tests requested for the same PatientID/SampleID, StatFlag and SampleType
                lstWSPatientDS = (From a As WorkSessionResultDS.PatientsRow In WorkSessionResultDSAttribute.Patients _
                                 Where a.SampleClass = "PATIENT" _
                               AndAlso a.SampleType = pSampleType _
                               AndAlso a.SampleID = lstWSPatientDS(0).SampleID _
                               AndAlso a.StatFlag <> pStatFlag _
                                Select a).ToList()
            Else
                'Get all Tests requested for the same PatientID/SampleID, StatFlag and SampleType
                lstWSPatientDS = (From a As WorkSessionResultDS.PatientsRow In WorkSessionResultDSAttribute.Patients _
                                 Where a.SampleClass = "PATIENT" _
                               AndAlso a.SampleType = pSampleType _
                               AndAlso a.SampleID = pPatientID _
                               AndAlso a.StatFlag <> pStatFlag _
                                Select a).ToList()
            End If

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

                newTestRow.SampleID = patientOrderTest.SampleID
                newTestRow.SampleIDType = patientOrderTest.SampleIDType

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
    ''' <param name="pPatientID">Sample Identifier</param>
    ''' <param name="pStatFlag">Stat/Routine flag</param>
    ''' <param name="pSampleType">Sample Type code</param>
    ''' <param name="pBarCodeInfo">Full BarCode</param>
    ''' <param name="pCurrentTestsDS">Typed DataSet SelectedTestsDS where the list of selected Tests for the 
    '''                               informed PatientID/SampleID, StatFlag and SampleType will be loaded</param>
    ''' <remarks>
    ''' Created by:  SA 19/09/2011 - Copied from IWSSampleRequest screen and adapted 
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings 
    '''                              problems (Bugs tracking #1112)
    '''              SA 08/05/2013 - When Order Tests are found for the SpecimenID/StatFlag/SampleType or for the PatientID/StatFlag/SampleType, 
    '''                              get current value of fields SampleID, SampleIDType and LISRequest for them and load these values in the SelectedTestsDS to return.
    ''' </remarks>
    Private Sub FillSelectedTestsForPatient(ByVal pPatientID As String, _
                                            ByVal pStatFlag As Boolean, _
                                            ByVal pSampleType As String, _
                                            ByVal pBarCodeInfo As String, _
                                            ByRef pCurrentTestsDS As SelectedTestsDS)
        Try
            'First search Tests by SpecimentID + StatFlag + SampleType
            Dim lstWSPatientDS As List(Of WorkSessionResultDS.PatientsRow)
            lstWSPatientDS = (From a As WorkSessionResultDS.PatientsRow In WorkSessionResultDSAttribute.Patients _
                             Where a.SampleClass = "PATIENT" _
                           AndAlso a.SpecimenID = pBarCodeInfo _
                           AndAlso a.SampleType = pSampleType _
                           AndAlso a.StatFlag = pStatFlag _
                            Select a).ToList()

            If (lstWSPatientDS.Count > 0) Then
                'Get all Tests requested for the same PatientID/SampleID, StatFlag and SampleType
                lstWSPatientDS = (From a As WorkSessionResultDS.PatientsRow In WorkSessionResultDSAttribute.Patients _
                                 Where a.SampleClass = "PATIENT" _
                               AndAlso a.SampleType = pSampleType _
                               AndAlso a.SampleID = lstWSPatientDS(0).SampleID _
                               AndAlso a.StatFlag = pStatFlag _
                                Select a).ToList()
            Else
                'Get all Tests requested for the same PatientID/SampleID, StatFlag and SampleType
                lstWSPatientDS = (From a As WorkSessionResultDS.PatientsRow In WorkSessionResultDSAttribute.Patients _
                                 Where a.SampleClass = "PATIENT" _
                               AndAlso a.SampleType = pSampleType _
                               AndAlso a.SampleID = pPatientID _
                               AndAlso a.StatFlag = pStatFlag _
                                Select a).ToList()
            End If

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

                newTestRow.SampleID = patientOrderTest.SampleID
                newTestRow.SampleIDType = patientOrderTest.SampleIDType
                newTestRow.LISRequest = patientOrderTest.LISRequest

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
    ''' Created by:   SA 04/08/2011
    ''' Modified by : DL 31/08/2011
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
            bsHQSellectAllCheckBx.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SELECT_ALL", pLanguageID)

            'For button Tooltips...
            bsScreenToolTips.SetToolTip(bsSearchTestsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestsSelection", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            bsScreenToolTips.SetToolTip(EditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", currentLanguage))

            'AG 03/05/2013 - New tooltip when working with LIS online (different that the used when working with files)
            bsScreenToolTips.SetToolTip(bsLIMSImportButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_LISHostQuery", pLanguageID))

            'Error text
            errorBarcodeRepeated = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_BARCODE_REPEATED_SEL_SAMPTYPE", pLanguageID) & " "
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' The value selected by default in sample type combo will depend on following conditions:
    ''' •	All selected incomplete Patient Samples do not have Sample Type: no Sample Type is selected
    '''     ** Exception: if for all the selected Incomplete Patient Samples there are requested Tests for the same SampleType, the Sample Type is selected
    ''' •	All selected incomplete Patient Samples have the same  Sample Type: the Sample Type is selected 
    ''' •	The selected incomplete Patient Samples have different Sample Types or do not have a Sample Type assigned: no Sample Type is selected in the field
    ''' This sample type combo is enabled in following conditions:
    ''' •	All selected incomplete Patient Samples do not have the Sample Type informed, excepting when the LIS Status of at least one of the selected 
    '''     incomplete Patient Samples is ASKING, in which case sample type combo is disabled 
    ''' •   In whatever other case, sample type combo is disabled
    ''' The value selected by default for a Stat check will depend on following conditions:
    ''' •	All selected incomplete Patient Samples are not for Stat: unchecked and enabled
    ''' •	All selected incomplete Patient Samples are for Stat: checked and enabled
    ''' •	Some of selected  incomplete Patient Samples are for Routine and other are for Stat: unchecked and disabled
    ''' •	Some of selected  incomplete Patient Samples have LIS Status = ASKING: unchecked and disabled
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 15/09/2011 - Code moved from the Event. Some changes in the method logic
    ''' Modified by: TR 19/09/2011 - Clear content of the Error Provider. 
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    '''              SA 24/07/2013 - It is not allowed to change the priority not only for Cells with LIS Status ASKING, but also for Cells containing IN USE Patient Samples
    '''                              If the status of the active Work Session is ABORTED, or if the User has clicked in HQ button, EDIT button is disabled.
    ''' </remarks>
    Private Sub IncompletePatientSamplesCellMouseUp()
        Try
            Dim myStat As Boolean
            Dim mySampleType As String
            Dim dgvRow As DataGridViewRow

            bsScreenErrorProvider.Clear()
            If (bsIncompleteSamplesDataGridView.SelectedRows.Count = 0) Then
                EditButtonEnabled(False)

            ElseIf (WSStatusAttribute = "ABORTED" OrElse HQButtonUserClickAttribute) Then
                EditButtonEnabled(False)

            Else
                'Verify if all selected incomplete Patient Samples have LISSTATUS <> "ASKING", otherwise, fields in area of  
                'Enter Manual Details are all disabled
                Dim sampleStatASKING As Boolean = False

                'Verify if all selected incomplete Patient Samples have the same SampleType 
                Dim sampleTypeDifferent As Boolean = False

                mySampleType = bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("SampleType").Value.ToString
                For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                    If (Not sampleTypeDifferent AndAlso mySampleType <> dgvRow.Cells("SampleType").Value.ToString) Then
                        sampleTypeDifferent = True
                    End If

                    If (Not sampleStatASKING) AndAlso (dgvRow.Cells("LISStatus").Value.ToString = "ASKING") Then
                        sampleStatASKING = True
                    End If
                    If (sampleStatASKING And sampleTypeDifferent) Then Exit For
                Next dgvRow


                If (sampleTypeDifferent Or sampleStatASKING) Then
                    'It is not possible to select Tests when there are different Sample Types selected
                    bsSampleTypeComboBox.SelectedIndex = -1
                    bsSampleTypeComboBoxEnabled = False
                    bsSearchTestsButtonEnabled = False
                Else
                    If (mySampleType = String.Empty) Then
                        'Verify if there are requested Tests in the active WorkSession for the selected Incomplete Patient Samples. If all of them have 
                        'requested Tests for just one SampleType and it is the same for all, then the Sample Type is selected and the ComboBox is enabled
                        Dim mySampleID As String
                        Dim lstWSPatientDS As List(Of String)

                        For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                            mySampleID = dgvRow.Cells("ExternalPID").Value.ToString

                            'Search if there are requested Tests for this Patient in the active WorkSession. Get all different SampleTypes
                            lstWSPatientDS = (From a In WorkSessionResultDSAttribute.Patients _
                                             Where a.SampleClass = "PATIENT" _
                                           AndAlso a.SampleID = mySampleID _
                                            Select a.SampleType Distinct).ToList()

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
                            bsSampleTypeComboBoxEnabled = True
                        Else
                            'All the selected Patient Samples have requested Tests for the same SampleType, it is selected and the ComboBox is disabled
                            bsSampleTypeComboBoxSelectedValue = mySampleType
                            bsSampleTypeComboBox.SelectedValue = mySampleType
                            bsSampleTypeComboBoxEnabled = True
                        End If
                    Else
                        'If the selected Patient Samples have a SampleType informed, it is selected and the ComboBox is disabled
                        bsSampleTypeComboBoxSelectedValue = mySampleType
                        bsSampleTypeComboBox.SelectedValue = mySampleType
                        bsSampleTypeComboBoxEnabled = False
                    End If
                    bsSearchTestsButtonEnabled = True
                End If

                Dim statDifferent As Boolean = False
                ''TR 27/08/2013 -Comment here to implement solution
                ''Verify if all selected incomplete Patient Samples have the same StatFlag and that all of them are NOT IN USE
                'If (Not sampleStatASKING) Then
                '    myStat = CType(bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("StatFlag").Value, Boolean)
                '    For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                '        If (myStat <> CType(dgvRow.Cells("StatFlag").Value, Boolean) OrElse (dgvRow.Cells("CellStatus").Value.ToString <> "NO_INUSE")) Then
                '            statDifferent = True
                '            Exit For
                '        End If
                '    Next dgvRow
                'End If

                'If (statDifferent Or sampleStatASKING) Then
                '    'If the selected Patient Samples have different StatFlag values, it is not possible to change it
                '    bsStatCheckboxChecked = False
                '    bsStatCheckbox.Checked = False
                '    bsStatCheckboxEnabled = False
                'Else
                '    'If the selected Patient Samples have the same StatFlag value, it is possible to change it o
                '    bsStatCheckboxChecked = myStat
                '    bsStatCheckbox.Checked = myStat
                '    bsStatCheckboxEnabled = True
                'End If
                'TR 27/08/2013 - End Comment here uncoment line unther.

                'TR 27/08/2013  -Solution for binding and enable controls un comment to apply changes and comment the lines above
                'Verify if all selected incomplete Patient Samples have the same StatFlag and that all of them are NOT IN USE
                Dim allNotInUse As Boolean = True
                If (Not sampleStatASKING) Then

                    myStat = CType(bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("StatFlag").Value, Boolean)
                    If bsIncompleteSamplesDataGridView.SelectedRows.Count > 1 Then 'TR 
                        For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                            If (myStat <> CType(dgvRow.Cells("StatFlag").Value, Boolean)) Then
                                statDifferent = True
                                Exit For
                            End If
                            If (dgvRow.Cells("CellStatus").Value.ToString <> "NO_INUSE") Then 'TR
                                allNotInUse = False
                            End If
                        Next dgvRow
                    End If

                End If

                If (statDifferent Or sampleStatASKING) Then
                    'If the selected Patient Samples have different StatFlag values, it is not possible to change it
                    bsStatCheckboxChecked = False
                    bsStatCheckbox.Checked = False
                    bsStatCheckboxEnabled = False
                Else
                    'If the selected Patient Samples have the same StatFlag value, it is possible to change it o
                    bsStatCheckboxChecked = myStat
                    bsStatCheckbox.Checked = myStat
                    bsStatCheckboxEnabled = allNotInUse 'TR 
                End If
                'TR 27/08/2013 - END.
            End If

            bsSaveButtonEnabled = (bsSearchTestsButtonEnabled OrElse bsStatCheckboxEnabled)
            bsCancelButtonEnabled = True

            LIMSImportButtonEnabled()
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
    '''              TR 19/09/2011 - Added new column showing Code and Description of the Sample Type (when informed). Hide the
    '''                              column for the SampleType Code
    '''              SA 19/09/2011 - Included again the CellNumber column as hidden
    '''              SA 18/04/2013 - Hide column PatientID and show instead the ExternalPID column
    '''              SA 29/05/2013 - Added new hidden column for field MessageID 
    '''              SA 24/07/2013 - Added new hidden column for the Cell Status
    ''' </remarks>
    Private Sub InitializeIncompleteSamplesGrid(ByVal pLanguageID As String)
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim preloadedDataConfig As New PreloadedMasterDataDelegate

            bsIncompleteSamplesDataGridView.AutoGenerateColumns = False

            'Rotor Type
            columnName = "RotorType"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, columnName)
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False

            ''Selected for HQ CheckBox
            'Dim checkBoxColumnCheckValue As New DataGridViewCheckBoxColumn
            'columnName = "Selected"
            'checkBoxColumnCheckValue.Name = columnName
            'checkBoxColumnCheckValue.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LIS_MODE_RERUNS_LIS", pLanguageID)
            'checkBoxColumnCheckValue.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'bsIncompleteSamplesDataGridView.Columns.Add(checkBoxColumnCheckValue)
            'bsIncompleteSamplesDataGridView.Columns(columnName).Width = 38
            'bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            'bsIncompleteSamplesDataGridView.Columns(columnName).ReadOnly = False
            'bsIncompleteSamplesDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            'bsIncompleteSamplesDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Cell Number
            columnName = "CellNumber"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorCell", pLanguageID))
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = True
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 35
            bsIncompleteSamplesDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Barcode
            columnName = "BarCodeInfo"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_BARCODE", pLanguageID))
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).ReadOnly = False
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 170
            bsIncompleteSamplesDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            'External Sample ID
            columnName = "ExternalPID"
            'EF 29/08/2013 - Bugtracking 1272 - Change label text by 'Sample' in v2.1.1
            'bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_SampleID", pLanguageID))
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", pLanguageID))
            'EF 29/08/2013
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).ReadOnly = True
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 170
            bsIncompleteSamplesDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsIncompleteSamplesDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = True

            'Patient Identifier
            columnName = "PatientID"
            'EF 29/08/2013 - Bugtracking 1272 - Change label text by 'Sample' in v2.1.1
            'bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_SampleID", pLanguageID))
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", pLanguageID))
            'EF 29/08/2013
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 0
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False

            'Sample Type
            cboBxCell.DataSource = sampleTypes.Copy
            cboBxCell.DisplayMember = "ItemIDDesc"
            cboBxCell.ValueMember = "ItemID"
            cboBxCell.ReadOnly = False
            cboBxCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing

            columnName = "SampleType"
            cboBxCell.Name = columnName
            cboBxCell.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", pLanguageID)

            bsIncompleteSamplesDataGridView.Columns.Add(cboBxCell)
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).ReadOnly = False
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 178
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = True

            'Routine/Stat ICON
            Dim iconSampleClassColumn As New DataGridViewImageColumn
            columnName = "SampleClassIcon"
            iconSampleClassColumn.Name = columnName
            iconSampleClassColumn.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Stat", pLanguageID)

            bsIncompleteSamplesDataGridView.Columns.Add(iconSampleClassColumn)
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 52
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).ReadOnly = True
            bsIncompleteSamplesDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic

            'StatFlag column
            columnName = "StatFlag"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Stat", pLanguageID))
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 0
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).ReadOnly = False
            bsIncompleteSamplesDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsIncompleteSamplesDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'LIS Status Code
            columnName = "LISStatus"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Status", pLanguageID))
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 92

            'LIS Status Description
            columnName = "LISStatusShown"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Status", pLanguageID))
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = True
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 95

            'Flag of SampleType manually informed
            columnName = "NotSampleType"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, columnName)
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 0

            'Flag of editable SampleType
            columnName = "EditableSampleType"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, columnName)
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 0
            bsIncompleteSamplesDataGridView.Columns(columnName).ValueType = GetType(System.Boolean)

            'Cell selected in Samples Rotor in IWSRotorPositions screen, and is the order DESC (value 3 shows higher than value 0)
            columnName = "ShowOnPosition"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, columnName)
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False
            bsIncompleteSamplesDataGridView.Columns(columnName).Width = 0
            bsIncompleteSamplesDataGridView.Columns(columnName).ValueType = GetType(System.Int32)

            'HQ Message Identifier
            columnName = "MessageID"
            bsIncompleteSamplesDataGridView.Columns.Add(columnName, columnName)
            bsIncompleteSamplesDataGridView.Columns(columnName).DataPropertyName = columnName
            bsIncompleteSamplesDataGridView.Columns(columnName).Visible = False

            'Rotor Cell Status
            columnName = "CellStatus"
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

                    sampleTypes = myMasterDataDS.tcfgMasterData.Copy()
                    sampleTypes.Rows.InsertAt(sampleTypes.NewRow(), 0)

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
    ''' Verify if button for send Host Query requests to LIS can be enabled
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/09/2011
    ''' Modified by: AG 26/04/2013 - Activation rules have been moved to ESBusiness class
    '''              SA 24/07/2013 - Button is disabled if the status of the active WS is ABORTED, or if the user has clicked in HQ 
    '''                              button and the screen is waiting for the LIS answer
    ''' </remarks>
    Public Sub LIMSImportButtonEnabled()
        Try
            If (WSStatusAttribute <> "ABORTED" AndAlso Not HQButtonUserClickAttribute) Then
                Dim allRowCheckedAreOkForLIMS As Boolean
                Dim lsSelectedRowsLIMS As IEnumerable(Of Object)
                Dim lsSelectedNoOkRowsLIMS As IEnumerable(Of Object)
                Dim lsSelectable As IEnumerable(Of Object)

                allRowCheckedAreOkForLIMS = False

                'LIS BUTTON can be enabled when all rows with Selected CheckBox checked have LISStatus different of ASKING 
                lsSelectedRowsLIMS = (From a In bsIncompleteSamplesDataGridView.SelectedRows _
                                     Where (Not String.IsNullOrEmpty(CType(a, DataGridViewRow).Cells("SampleType").ErrorText) _
                                    OrElse CType(a, DataGridViewRow).Cells("LISStatus").Value.ToString <> "ASKING")
                                    Select a)

                lsSelectable = (From a In bsIncompleteSamplesDataGridView.Rows _
                               Where (Not String.IsNullOrEmpty(CType(a, DataGridViewRow).Cells("SampleType").ErrorText) _
                              OrElse CType(a, DataGridViewRow).Cells("LISStatus").Value.ToString <> "ASKING")
                              Select a)

                'LIS BUTTON has to be disabled when at least one of the Selected rows have the Error Icon for required Sample Type
                lsSelectedNoOkRowsLIMS = (From a In lsSelectedRowsLIMS
                                         Where Not String.IsNullOrEmpty(CType(a, DataGridViewRow).Cells("SampleType").ErrorText) _
                                          Take 1 _
                                        Select a)

                If (lsSelectedNoOkRowsLIMS.Count > 0) Then
                    'There is at least a row that disable the LIS Button
                    allRowCheckedAreOkForLIMS = False

                ElseIf (lsSelectedRowsLIMS.Count > 0) Then
                    'There are not rows that disable the LIS Button and there is at least one that allow enable it
                    allRowCheckedAreOkForLIMS = True
                End If

                If (allRowCheckedAreOkForLIMS AndAlso Not mdiAnalyzerCopy Is Nothing AndAlso Not mdiESWrapperCopy Is Nothing) Then
                    Dim myESBusinessDlg As New ESBusiness
                    Dim runningFlag As Boolean = CBool(IIf(mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.RUNNING, True, False))
                    Dim connectingFlag As Boolean = CBool(IIf(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS", True, False))

                    bsLIMSImportButton.Enabled = myESBusinessDlg.AllowLISAction(Nothing, LISActions.HostQuery, runningFlag, connectingFlag, mdiESWrapperCopy.Status, mdiESWrapperCopy.Storage)
                Else
                    bsLIMSImportButton.Enabled = False
                End If

                'Update CheckState of SelectAll CheckBox
                bsHQSelectAllCheckBx_EventEnabled = False
                If (lsSelectable.Count = bsIncompleteSamplesDataGridView.SelectedRows.Count AndAlso lsSelectable.Count > 0) Then
                    bsHQSellectAllCheckBx.CheckState = CheckState.Checked
                Else
                    bsHQSellectAllCheckBx.CheckState = CheckState.Unchecked
                End If
                bsHQSelectAllCheckBx_EventEnabled = True
            Else
                'Disable HQ button when Status of the active WorkSession is ABORTED or when a HQ has been sent and the screen is
                'waiting for the LIS answer
                bsLIMSImportButton.Enabled = False
            End If

            'XB 01/08/2013 - Add functionality to disable LIS buttons
            If (IAx00MainMDI.DisableLISButtons()) Then
                bsLIMSImportButton.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LIMSImportButtonEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LIMSImportButtonEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ' ''' <summary>
    ' ''' Hide the Patient Samples edited or Patient Samples with the same Barcode and the same Type
    ' ''' </summary>
    ' ''' <remarks>
    ' ''' Created by:  JC 09/05/2013
    ' ''' </remarks>
    'Private Sub HideEditedRows()
    '    Try
    '        'Hide Rows manually edited and currently pending to position when the form closed
    '        If (Not hideRows Is Nothing) Then
    '            Dim rowsToHide As List(Of Object) = (From a In bsIncompleteSamplesDataGridView.Rows _
    '                                                Where hideRows.Contains(CType(CType(a, DataGridViewRow).Cells("CellNumber").Value, Integer))
    '                                               Select a).ToList()

    '            Dim similarRw As List(Of Integer)
    '            For Each rw As Object In rowsToHide
    '                'Select Samples With Same BarCode and Sample Type than current to hide
    '                similarRw = (From a In bsIncompleteSamplesDataGridView.Rows _
    '                            Where CStr(CType(a, DataGridViewRow).Cells("BarCodeInfo").Value) = CStr(CType(rw, DataGridViewRow).Cells("BarCodeInfo").Value) _
    '                          AndAlso Not IsDBNull(CType(a, DataGridViewRow).Cells("SampleType").Value) _
    '                          AndAlso CStr(CType(a, DataGridViewRow).Cells("SampleType").Value) = CStr(CType(rw, DataGridViewRow).Cells("SampleType").Value) _
    '                          AndAlso CStr(CType(a, DataGridViewRow).Cells("StatFlag").Value) = CStr(CType(rw, DataGridViewRow).Cells("StatFlag").Value) _
    '                          Order By CType(a, DataGridViewRow).Index Descending _
    '                           Select CType(a, DataGridViewRow).Index).ToList()

    '                If bsIncompleteSamplesDataGridView.Rows.Count > 0 Then
    '                    Dim cm As CurrencyManager = CType(BindingContext(bsIncompleteSamplesDataGridView.DataSource), CurrencyManager)


    '                    For Each rwDelete As Integer In similarRw
    '                        cm.SuspendBinding()
    '                        bsIncompleteSamplesDataGridView.Rows.RemoveAt(rwDelete)
    '                    Next
    '                End If

    '            Next
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".HideEditedRows", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".HideEditedRows", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Load the list of Patient Samples marked as incompleted after scanning
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/08/2011
    ''' Modified by: TR 19/09/2011 - Get the Sample Type Description
    '''              SA 11/06/2012 - When the grid is empty, set attribute ChangesMade to True to avoid errors 
    '''                              when the WS is saved in the screen of WSSamplesRequest
    '''              AG 11/07/2013 - Added changes for automatic WS creation with LIS
    '''              SA 24/07/2013 - Added changes to always load the grid with all Patient Samples in Samples Rotor (not only with the incomplete ones) 
    ''' </remarks>
    Private Sub LoadIncompleteSamplesGrid(Optional ShowRowPosition As Integer = Nothing, Optional CheckAllToRequestLis As Boolean = True)
        Try
            'If AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute Then 'Show all patient tubes read
            '    Dim rotorPosDlg As New WSRotorContentByPositionDelegate
            '    myGlobalDataTO = rotorPosDlg.GetAllPatientTubesForHQ(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)
            'Else ' Classical business
            '    Dim myBarcodePositionsWithNoRequestsDelegate As New BarcodePositionsWithNoRequestsDelegate
            '    myGlobalDataTO = myBarcodePositionsWithNoRequestsDelegate.ReadByAnalyzerAndWorkSession(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute)
            'End If

            Dim myGlobalDataTO As New GlobalDataTO
            Dim rotorPosDlg As New WSRotorContentByPositionDelegate
            Dim excludeDuplicates As Boolean = (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute)

            myGlobalDataTO = rotorPosDlg.GetAllPatientTubesForHQ(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, excludeDuplicates)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myBarcodePositionsWithNoRequestsDS As BarcodePositionsWithNoRequestsDS = DirectCast(myGlobalDataTO.SetDatos, BarcodePositionsWithNoRequestsDS)

                Dim myMasterDataDelegate As New MasterDataDelegate
                myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myMasterDataDS As MasterDataDS = DirectCast(myGlobalDataTO.SetDatos, MasterDataDS)

                    'Get the description by LINQ
                    Dim qMasterData As New List(Of MasterDataDS.tcfgMasterDataRow)
                    For Each bcpRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows
                        qMasterData = (From a In myMasterDataDS.tcfgMasterData _
                                      Where a.ItemID = bcpRow.SampleType Select a).ToList()

                        If (qMasterData.Count > 0) Then
                            bcpRow.SampleTypeDesc = qMasterData.First().ItemIDDesc
                        End If
                    Next
                End If

                'JC 11/04/2013 Sort Grid by columns "Selected, CellNumber" if screens is opened from IWSRotorPosition with some cells selected,
                '              or by "CellNumber" when it is opened from IWSRotorPosition without any cell selected
                Dim viewSamples As DataView = myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.DefaultView
                If (Not IsNothing(OpenSelectedCellSamples) AndAlso OpenSelectedCellSamples.Count > 0) Then
                    viewSamples.Sort = "ShowOnPosition DESC, CellNumber ASC"

                    Dim orderSelected As Integer = 0
                    Dim numSamplesSelectedUser As Integer = OpenSelectedCellSamples.Count
                    For Each dtRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow In myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows
                        orderSelected = OpenSelectedCellSamples.IndexOf(dtRow.CellNumber)
                        If (orderSelected >= 0) Then
                            SetCheckHQRow(dtRow, True)
                            dtRow.ShowOnPosition = numSamplesSelectedUser - orderSelected
                        End If
                    Next

                    Dim bsSamplesBindingSource As BindingSource = New BindingSource() With {.DataSource = viewSamples}
                    bsIncompleteSamplesDataGridView.DataSource = bsSamplesBindingSource

                    Dim rowsToSelect As List(Of Object) = (From a In bsIncompleteSamplesDataGridView.Rows _
                                                          Where OpenSelectedCells.Contains(CInt(CType(a, DataGridViewRow).Cells("CellNumber").Value))
                                                         Select a).ToList()

                    For Each row As Object In rowsToSelect
                        CType(row, DataGridViewRow).Selected = True
                    Next

                    ' JC 09-05-2013
                    'Hide Rows manually edited and current pending to position when form close
                    'HideEditedRows()
                    SetErrorDuplicatedBarcode()
                Else
                    viewSamples.Sort = "CellNumber ASC"

                    Dim bsSamplesBindingSource As BindingSource = New BindingSource() With {.DataSource = viewSamples}
                    bsIncompleteSamplesDataGridView.DataSource = bsSamplesBindingSource

                    'JC 09-05-2013
                    'Hide Rows manually edited and current pending to position when form close
                    'HideEditedRows()

                    'If no selected samples from rotor position then scroll grid to First Barcode Repeated
                    Dim firstRowWithWarning As Integer = SetErrorDuplicatedBarcode()
                    If (firstRowWithWarning <> -1 AndAlso IsNothing(ShowRowPosition)) Then
                        bsIncompleteSamplesDataGridView.FirstDisplayedScrollingRowIndex = firstRowWithWarning
                    End If

                    If (myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.Rows.Count > 0) Then
                        'Unselect first row in DataGridView
                        bsIncompleteSamplesDataGridView.ClearSelection()
                    End If
                End If

                If (Not IsNothing(ShowRowPosition) AndAlso bsIncompleteSamplesDataGridView.RowCount > 0) Then
                    bsIncompleteSamplesDataGridView.FirstDisplayedScrollingRowIndex = CInt(IIf(ShowRowPosition < bsIncompleteSamplesDataGridView.RowCount AndAlso _
                                                                                               ShowRowPosition >= 0, ShowRowPosition, 0))
                End If
                RepaintGridRows()
            Else
                'Error getting the list of Incomplete Patient Samples
                ShowMessage(Name & ".LoadIncompleteSamplesGrid", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

            IncompletePatientSamplesCellMouseUp()
            EnterSamplesDetailsEnabled(False)

            'When there are only 1 distinct Sample Type, set this Sample Type on Combox of the edit area. 
            Dim distinctSamplesTypes As IEnumerable(Of Object)
            distinctSamplesTypes = From c In bsIncompleteSamplesDataGridView.SelectedRows
                                                 Select CType(c, DataGridViewRow).Cells("SampleType").Value
                                                 Distinct
            If distinctSamplesTypes.Count = 1 Then
                bsIncompleteSamplesDataGridView_SelectionChanged(Nothing, Nothing)
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

            'LIS Button
            auxIconName = GetIconName("HQ")
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

            'EDIT Button
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                EditButton.Image = Image.FromFile(iconPath & auxIconName)
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
    '''              SA 08/05/2013 - Removed references to PatientID field; it is not used. Removed code used when the screen was opened 
    '''                              from IWSSamplesRequest screen. Before calling function to add selected Tests to the DS containing all data in
    '''                              the active WS, get the SampleID and the SampleIDType from the first row in SelectedTestsDS and call function 
    '''                              that remove from the DS WorkSessionResultDSAttribute all Tests that have been deleted  
    '''              SA 14/06/2013 - Before calling function to add selected Tests to the DS containing all data in the active WS, the SampleID and
    '''                              SampleIDType are obtained from the first row in SelectedTestsDS only when they are informed (all selected Barcodes
    '''                              have the same SampleID); otherwise, the SampleID is obtained from the selected row in process, and SampleIDType
    '''                              is always MANUAL
    '''              SA 01/08/2013 - Due to new functionality for wait synchronously until LIS sends an answer for all queried Specimens (or until the maximum
    '''                              waiting time expires), manual changes are now saved individually in this point, instead of saving all of them when the 
    '''                              screen closes 
    '''              SA 21/03/2014 - BT #1545 ==> Changes to divide AddWorkSession process in several DB Transactions. When value of global flag 
    '''                                           NEWAddWorkSession is TRUE, call new version of function PrepareOrderTestsForWS
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
                        Dim myBarCode As String = bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("BarCodeInfo").Value.ToString

                        'Verify if all selected rows have the same ExternalID
                        For Each dgvRow As DataGridViewRow In bsIncompleteSamplesDataGridView.SelectedRows
                            If (dgvRow.Cells("ExternalPID").Value.ToString <> mySampleID) Then
                                mySampleID = String.Empty
                                Exit For
                            End If
                        Next dgvRow

                        If (mySampleID <> String.Empty) Then
                            FillSelectedTestsForPatient(mySampleID, bsStatCheckbox.Checked, bsSampleTypeComboBox.SelectedValue.ToString, myBarCode, mySelectedTestsDS)
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
                'Button Search is not enabled, which mean several Incomplete Patient Samples are selected; in this case, only the StatFlag can be changed
                If (bsStatCheckbox.Enabled) Then
                    Dim dgvRow As DataGridViewRow
                    Dim barcodePositionsWithNoRequestsRow As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow

                    For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                        'Add data to myBarcodePositionsWithNoRequestsDS
                        barcodePositionsWithNoRequestsRow = myBarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequests.NewtwksWSBarcodePositionsWithNoRequestsRow
                        barcodePositionsWithNoRequestsRow.WorkSessionID = WorkSessionIDAttribute
                        barcodePositionsWithNoRequestsRow.AnalyzerID = AnalyzerIDAttribute
                        barcodePositionsWithNoRequestsRow.RotorType = dgvRow.Cells("RotorType").Value.ToString
                        barcodePositionsWithNoRequestsRow.ExternalPID = dgvRow.Cells("ExternalPID").Value.ToString

                        If (dgvRow.Cells("SampleType").Value.ToString = String.Empty OrElse _
                            dgvRow.Cells("SampleType").Value.ToString <> dgvRow.Cells("SampleType").GetEditedFormattedValue(dgvRow.Index, DataGridViewDataErrorContexts.InitialValueRestoration).ToString) Then
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

                    Dim myPID As String = String.Empty
                    Dim myPIDType As String = String.Empty

                    Dim dgvRow As DataGridViewRow
                    Dim myOrderTestsDelegate As New OrderTestsDelegate

                    For Each dgvRow In bsIncompleteSamplesDataGridView.SelectedRows
                        'JCID 26/03/2013  If several samples with distinct Stat have been selected, must check the current row Stat, else use the generic stat choosed on edit control botom
                        If (Not bsStatCheckbox.Enabled) Then myStatFlag = DirectCast(dgvRow.Cells("StatFlag").Value, Boolean)

                        If (Not mySelectedTestsDS Is Nothing AndAlso mySelectedTestsDS.SelectedTestTable.Rows.Count > 0) Then
                            'Get values of SampleID and SampleIDType from first row in DS of Selected Tests if they are informed
                            If (Not mySelectedTestsDS.SelectedTestTable.First.IsSampleIDNull AndAlso mySelectedTestsDS.SelectedTestTable.First.SampleID <> String.Empty) Then
                                myPID = mySelectedTestsDS.SelectedTestTable.First.SampleID
                            Else
                                myPID = dgvRow.Cells("ExternalPID").Value.ToString
                            End If

                            myPIDType = "MAN"
                            If (Not mySelectedTestsDS.SelectedTestTable.First.IsSampleIDTypeNull AndAlso mySelectedTestsDS.SelectedTestTable.First.SampleIDType <> String.Empty) Then
                                myPIDType = mySelectedTestsDS.SelectedTestTable.First.SampleIDType
                            End If

                            'Delete all Open Patient Order Tests that are not in the list of selected Tests for the active StatFlag, Sample Type, SampleID
                            myGlobalDataTO = myOrderTestsDelegate.DeletePatientOrderTests("NOT_IN_LIST", mySelectedTestsDS, WorkSessionResultDSAttribute, _
                                                                                          mySampleType, myPID, myStatFlag)
                            'If there are Order Tests to add...
                            If (Not myGlobalDataTO.HasError) Then
                                'Add all Patient Order Tests of the selected Tests for the active Sample Type
                                myGlobalDataTO = myOrderTestsDelegate.AddPatientOrderTests(mySelectedTestsDS, WorkSessionResultDSAttribute, AnalyzerIDAttribute, _
                                                                                           myPID, myStatFlag, myPIDType)
                            End If
                        End If
                        If (myGlobalDataTO.HasError) Then Exit For

                        'Update values of StatFlag and SampleType in the grid, and update them also in the table of Incomplete Patient Samples
                        dgvRow.Cells("StatFlag").Value = myStatFlag
                        dgvRow.Cells("SampleType").Value = mySampleType

                        myGlobalDataTO = myBarcodePositionsWithNoRequests.HQUpdatePatientRequestedSamples(Nothing, CType(CType(dgvRow.DataBoundItem, DataRowView).Row,  _
                                                                                                          BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow))
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next dgvRow

                    If (myGlobalDataTO.HasError) Then
                        'Error adding all new elements to the Work Session or updating StatFlag and SampleType for the selected Incomplete Patient Samples
                        ShowMessage(Name & ".SaveChanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    Else
                        'Save Changes in the active WorkSession
                        Dim myWSDelegate As New WorkSessionsDelegate
                        If (NEWAddWorkSession) Then
                            'BT #1545
                            myGlobalDataTO = myWSDelegate.PrepareOrderTestsForWS_NEW(WorkSessionResultDSAttribute, WorkSessionIDAttribute, AnalyzerIDAttribute, False, WSStatusAttribute, True, False)
                        Else
                            myGlobalDataTO = myWSDelegate.PrepareOrderTestsForWS(Nothing, WorkSessionResultDSAttribute, False, WorkSessionIDAttribute, AnalyzerIDAttribute, WSStatusAttribute, True, False)
                        End If

                        'Update status of WSStatusAttribute with content of the WS DS returned
                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myWS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)
                            If (myWS.twksWorkSessions.Rows.Count = 1) Then WSStatusAttribute = myWS.twksWorkSessions(0).WorkSessionStatus
                        Else
                            'Error saving changes in the active Work Session 
                            ShowMessage(Name & ".SaveChanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        End If
                    End If
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
    '''              JC 25/03/2013 - Get the icons used for STAT and Normal  
    '''              AG 08/07/2013 - When the screen is opened during the automatic WS creation process, all rows in the grid are selected
    '''              SA 24/07/2013 - Added changes to disable all action buttons if the Status of the active WorkSession is ABORTED
    ''' </remarks>
    Private Sub ScreenLoad()
        Dim myGlobalDataTO As GlobalDataTO = Nothing
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim TotalStartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            MyClass.currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage

            If (Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) 'AG 16/06/2011 - Use the same AnalyzerManager as the MDI
                mdiAnalyzerCopy.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS) = 0
            End If

            If (Not AppDomain.CurrentDomain.GetData("GlobalLISManager") Is Nothing) Then
                mdiESWrapperCopy = CType(AppDomain.CurrentDomain.GetData("GlobalLISManager"), ESWrapper) ' Use the same ESWrapper as the MDI
            End If

            'Load buttons images
            PrepareButtons()

            'Get multilanguage labels for all screen controls
            GetScreenLabels(currentLanguage)

            'Load ComboBox of Sample Types
            InitializeSampleTypeComboBox()

            'Initializes the Incomplete Samples DataGridView
            InitializeIncompleteSamplesGrid(currentLanguage)

            'Get the icons used for STAT and Normal  
            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
            STATIcon = preloadedDataConfig.GetIconImage("STATS")
            NORMALIcon = preloadedDataConfig.GetIconImage("ROUTINES")

            'Get the list of Incomplete Samples to load the DataGridView
            LoadIncompleteSamplesGrid()

            'Disable all controls in Enter Samples Details; verify if LIS Button can be enabled
            LIMSImportButtonEnabled()
            If (IsNothing(OpenSelectedCellSamples) OrElse OpenSelectedCellSamples.Count = 0) Then
                bsHQSellectAllCheckBx.Checked = True
                bsHQSellectAllCheckBx.CheckState = CheckState.Checked
            End If

            'Special mode for working with LIS with automatic actions (select all specimens)
            If (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute) Then
                bsHQSellectAllCheckBx.Checked = True
                bsHQSellectAllCheckBx.CheckState = CheckState.Checked
                bsExitButton.Enabled = False
            End If

            'If the screen is opened when the active Work Session is ABORTED, all buttons and fields in the screen are disabled
            If (WSStatusAttribute = "ABORTED") Then
                DisableAllFields()
                bsScreenErrorProvider.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
            Else
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
                    If (WSStatusAttribute.Trim <> "EMPTY") Then
                        Dim myWSDelegate As New WorkSessionsDelegate

                        myGlobalDataTO = myWSDelegate.GetOrderTestsForWS(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, False)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            WorkSessionResultDSAttribute = DirectCast(myGlobalDataTO.SetDatos, WorkSessionResultDS)
                        Else
                            'Error getting the list of OrderTests currently included in the WorkSession
                            ShowMessage(Name & ".ScreenLoad", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        End If
                    End If
                End If
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Total function time = " & Now.Subtract(TotalStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            Me.Name & "." & (New System.Diagnostics.StackTrace()).GetFrame(0).GetMethod().Name, EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
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
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional 
    '''                              Settings problems (Bugs tracking #1112)
    '''              SA 08/05/2013 - Removed references to PatientID field; it is not used. If the SelectedTestsDS returned by the auxiliary screen
    '''                              of Tests selection does not contain the SampleID and SampleIDType, inform these fields in all DS rows with  
    '''                              value of field ExternalPID and assume the SampleID Type is Manual
    '''              TR 29/04/2014 - BT #1494 Validate when the auxiliari Test selection screen is close if there are any incomplete programming test
    '''                              from selection.
    ''' </remarks>
    Private Sub SearchTests()
        Try
            bsScreenErrorProvider.Clear()
            If (bsSampleTypeComboBox.SelectedIndex >= 0) Then
                Dim lockedTests As New SelectedTestsDS
                Dim mySampleID As String = bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("ExternalPID").Value.ToString
                Dim myBarCodeInfo As String = bsIncompleteSamplesDataGridView.SelectedRows(0).Cells("BarCodeInfo").Value.ToString

                'Verify if all selected rows have the same ExternalID
                For Each dgvRow As DataGridViewRow In bsIncompleteSamplesDataGridView.SelectedRows
                    If (dgvRow.Cells("ExternalPID").Value.ToString <> mySampleID) Then
                        mySampleID = String.Empty
                        Exit For
                    End If
                Next dgvRow

                If (mySampleID <> String.Empty) Then
                    FillSelectedTestsForPatient(mySampleID, bsStatCheckbox.Checked, bsSampleTypeComboBox.SelectedValue.ToString, myBarCodeInfo, mySelectedTestsDS)
                    If (mySelectedTestsDS.SelectedTestTable.Rows.Count > 0) Then
                        If (Not mySelectedTestsDS.SelectedTestTable.First.IsSampleIDNull) Then mySampleID = mySelectedTestsDS.SelectedTestTable.First.SampleID
                    End If

                    FillSelectedTestsForDifPriority(mySampleID, bsStatCheckbox.Checked, bsSampleTypeComboBox.SelectedValue.ToString, myBarCodeInfo, lockedTests, mySelectedTestsDS)
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

                    'JC 09/05/2013 Screen sometimes appear on bacground of MDIForm, and app looks like has hang out
                    '#If Not Debug Then
                    Me.TopMost = False
                    '#End If

                    myForm.ShowDialog()
                    If (myForm.DialogResult = Windows.Forms.DialogResult.OK) Then
                        mySelectedTestsDS = myForm.ListOfSelectedTests

                        ' TR 29/04/2014 BT#1494 -Validate if there was incomplete programming test to show message
                        If myForm.IncompleteTest Then
                            ShowMessage("Error", GlobalEnumerates.Messages.INCOMPLETE_TESTSAMPLE.ToString)
                        End If
                        ' TR 29/04/2014 BT#1494 -END.

                        If (mySelectedTestsDS.SelectedTestTable.Rows.Count > 0) Then
                            If (mySelectedTestsDS.SelectedTestTable.First.IsSampleIDNull) Then
                                For Each row As SelectedTestsDS.SelectedTestTableRow In mySelectedTestsDS.SelectedTestTable
                                    row.SampleID = mySampleID
                                    row.SampleIDType = "MAN"
                                Next
                            End If

                            'Get all Tests requested for the same PatientID/SampleID, StatFlag and SampleType
                            Dim lstWSPatientDS As List(Of WorkSessionResultDS.PatientsRow)
                            lstWSPatientDS = (From a As WorkSessionResultDS.PatientsRow In WorkSessionResultDSAttribute.Patients _
                                             Where a.SampleClass = "PATIENT" _
                                           AndAlso a.SampleType = bsSampleTypeComboBox.SelectedValue.ToString() _
                                           AndAlso a.SampleID = mySelectedTestsDS.SelectedTestTable.First.SampleID _
                                           AndAlso a.StatFlag = bsStatCheckbox.Checked _
                                            Select a).ToList()

                            'If there are OrderTests requested for the same Patient/SampleType/StatFlag, then the StatFlag
                            'cannot be changed (this case is only for management of several tubes for the same Patient but
                            'without SampleType informed)
                            If (lstWSPatientDS.Count > 0) Then bsStatCheckbox.Enabled = False
                        End If
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
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid 
    '''                              Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Function VerifyAtLeastATestBySampleID() As Boolean
        Dim atLeastATest As Boolean = False

        Try
            Dim mySampleID As String = String.Empty
            Dim mySampleIDType As String = String.Empty
            Dim lstPatientTests As List(Of SelectedTestsDS.SelectedTestTableRow)
            Dim myBarCodeInfo As String = String.Empty

            For Each dgvRow As DataGridViewRow In bsIncompleteSamplesDataGridView.SelectedRows
                mySampleID = dgvRow.Cells("ExternalPID").Value.ToString
                myBarCodeInfo = dgvRow.Cells("BarCodeInfo").Value.ToString

                'Load all Tests selected for the SampleID in mySelectedTestsDS
                FillSelectedTestsForPatient(mySampleID, bsStatCheckbox.Checked, bsSampleTypeComboBox.SelectedValue.ToString(), myBarCodeInfo, mySelectedTestsDS)

                'Verify if at least an STD or ISE Test have been requested for the SampleID/StatFlag with the selected SampleType
                lstPatientTests = (From a As SelectedTestsDS.SelectedTestTableRow In mySelectedTestsDS.SelectedTestTable _
                                  Where a.SampleType = bsSampleTypeComboBox.SelectedValue.ToString() _
                                AndAlso a.SampleID = mySampleID _
                                AndAlso a.StatFlag = bsStatCheckbox.Checked _
                                AndAlso a.TestType <> "OFFS" _
                                 Select a).ToList()

                atLeastATest = (lstPatientTests.Count > 0)
                If (Not atLeastATest) Then Exit For
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".VerifyAtLeastATestBySampleID", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".VerifyAtLeastATestBySampleID", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return atLeastATest
    End Function

    ''' <summary>
    ''' Check/Uncheck a Patient Sample Row if requirements are met
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC 25/03/2013 
    ''' Modified by: SG 29/05/2013 - Get Multilanguage description for LIS Status ASKING
    ''' </remarks>
    Private Sub SetRowAsking(gdRow As DataGridViewRow)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Paint Only specified Row
            gdRow.DefaultCellStyle.BackColor = Color.LightGray
            gdRow.ReadOnly = True
            gdRow.Cells("LISStatus").Value = "ASKING"
            gdRow.Cells("LISStatusShown").Value = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_STATUS_ASKED", MyClass.currentLanguage)
            gdRow.Selected = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetRowAsking", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetRowAsking", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Check/Uncheck a Patient Sample Row if requirements are met
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC 25/03/2013 
    ''' </remarks>
    Private Sub RepaintGridRows()
        Try
            For Each gdVwRow As DataGridViewRow In bsIncompleteSamplesDataGridView.Rows
                RepaintGridRow(gdVwRow)
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RepaintGridRows", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RepaintGridRows", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Check/Uncheck a Patient Sample Row if requirements are met
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC 25/03/2013 
    ''' Modified by: SG 29/05/2013 - Get Multilanguage description for LIS Status ASKING and NOINFO; show Required Error Message in Sample Type cell 
    '''                              when in the active WS there are Tests of several Sample Types for an unique SpecimenID
    ''' </remarks>
    Private Sub RepaintGridRow(Row As DataGridViewRow)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            If (Row.Cells("LISStatus").Value.ToString = "ASKING") Then
                Row.ReadOnly = True
                If (bsIncompleteSamplesDataGridView.Enabled) Then Row.DefaultCellStyle.BackColor = Color.LightGray
                Row.Cells("LISStatusShown").Value = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_STATUS_ASKED", MyClass.currentLanguage)

            ElseIf (Row.Cells("LISStatus").Value.ToString = "NOINFO") Then
                Row.ReadOnly = False
                If (bsIncompleteSamplesDataGridView.Enabled) Then Row.DefaultCellStyle.BackColor = Color.White
                Row.Cells("LISStatusShown").Value = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_STATUS_NOINFO", MyClass.currentLanguage)

            Else 'PENDING, UNDELIVERED, REJECTED, UNRESPONDED
                Row.ReadOnly = False
                If (bsIncompleteSamplesDataGridView.Enabled) Then Row.DefaultCellStyle.BackColor = Color.White
                Row.Cells("LISStatusShown").Value = String.Empty

                'Show Required Error Message in Sample Type cell when in the active WS there are Tests of several Sample Types for an unique SpecimenID
                If (Not IsDBNull(Row.Cells("MessageID").Value) AndAlso CStr(Row.Cells("MessageID").Value) = "PROCESSED") Then
                    If (IsDBNull(Row.Cells("SampleType").Value) Or String.IsNullOrEmpty(Row.Cells("SampleType").Value.ToString) And (String.IsNullOrEmpty(Row.Cells("SampleType").ErrorText))) Then
                        Row.Cells("SampleType").ErrorText = MyBase.GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)
                        Row.ReadOnly = True
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RepaintGridRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RepaintGridRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Check/Uncheck a Patient Sample Row if requirements are met
    ''' </summary>
    ''' <remarks>
    ''' Created by: JC 25/03/2013 
    ''' </remarks>
    Private Function SetCheckHQRow(SampleInfo As DataGridViewRow, Value As Boolean) As Boolean
        Dim finalValue As Boolean

        Try
            finalValue = (Value And Not SampleInfo.Cells("LISStatus").Value.ToString = "ASKING")
            SampleInfo.Selected = finalValue
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetCheckHQRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetCheckHQRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return finalValue
    End Function

    ''' <summary>
    ''' Check/Uncheck a Patient Sample Row if requirements are met
    ''' </summary>
    ''' <remarks>
    ''' Created by: JC 25/03/2013 
    ''' </remarks>
    Private Function SetCheckHQRow(SampleInfo As BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow, Value As Boolean) As Boolean
        Dim finalValue As Boolean

        Try
            finalValue = (Value And Not SampleInfo.LISStatus = "ASKING")
            SampleInfo.Selected = finalValue
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetCheckHQRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetCheckHQRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return finalValue
    End Function

    ''' <summary>
    ''' Change a Patient Sample Urgency Flag and Icon 
    ''' </summary>
    ''' <remarks>
    ''' Created by: JC 25/03/2013 
    ''' </remarks>
    Private Sub ChangeStatusFlagRow(SampleInfo As DataGridViewRow)
        Try
            SampleInfo.Cells("StatFlag").Value = Not CBool(SampleInfo.Cells("StatFlag").Value)
            SampleInfo.Cells("SampleClassIcon").Value = IIf(CBool(SampleInfo.Cells("StatFlag").Value), STATIcon, NORMALIcon)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeStatusFlagRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeStatusFlagRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Clear error alerts on Repeated Barcodes and Sample Type are not Void
    ''' </summary>
    ''' <param name="Barcode">Optional: if Barcode is void, clear all error alerts.
    '''                                 if Barcode has value, clear all errors for samples with this Barcode</param>
    ''' <remarks>
    ''' Created by: JC 25/03/2013 
    ''' </remarks>
    Private Sub UnsetErrorDuplicatedBarcode(Optional Barcode As String = "")
        Try
            Dim lsDuplicatedBarcodes As IEnumerable(Of Object)
            If (String.IsNullOrEmpty(Barcode)) Then
                lsDuplicatedBarcodes = From a In bsIncompleteSamplesDataGridView.Rows _
                                      Where Not String.IsNullOrEmpty(CType(a, DataGridViewRow).Cells("BarCodeInfo").Value.ToString) _
                                     Select a _
                                      Group a By CType(a, DataGridViewRow).Cells("BarCodeInfo").Value Into Group _
                                     Select Group.ToList()
            Else
                lsDuplicatedBarcodes = From a In bsIncompleteSamplesDataGridView.Rows _
                                      Where CType(a, DataGridViewRow).Cells("BarCodeInfo").Value.ToString = Barcode _
                                     Select a _
                                      Group a By CType(a, DataGridViewRow).Cells("BarCodeInfo").Value Into Group _
                                     Select Group.ToList()
            End If

            For i As Integer = 0 To lsDuplicatedBarcodes.Count - 1
                For Each row As Object In CType(lsDuplicatedBarcodes(i), List(Of Object))
                    CType(row, DataGridViewRow).Cells("SampleType").ErrorText = String.Empty
                Next
            Next

            lsDuplicatedBarcodes = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UnsetErrorDuplicatedBarcode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UnsetErrorDuplicatedBarcode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set error alerts on Repeated Barcodes and Sample Type Void
    ''' </summary>
    ''' <param name="Barcode">Optional: if Barcode is void, clear all error alerts.
    '''                                 if Barcode has value, clear all errors for samples with this Barcode
    ''' </param>
    ''' <remarks>
    ''' Created by:  JC 25/03/2013 
    ''' </remarks>
    Private Function SetErrorDuplicatedBarcode(Optional Barcode As String = "") As Integer
        Dim firstRowWithRepeatedBarcode As Integer = -1

        Try
            If (WSStatusAttribute <> "ABORTED") Then
                Dim barcode_repeated As String
                Dim gdVwRwsSampleTypeVoid As IEnumerable(Of Object)
                Dim gdVwRwsBarcodeRepeated As IEnumerable(Of Object)

                anyRowWithError = False
                If (String.IsNullOrEmpty(Barcode)) Then
                    'List of all Barcodes repeated
                    gdVwRwsBarcodeRepeated = From a In bsIncompleteSamplesDataGridView.Rows _
                                            Where Not String.IsNullOrEmpty(CType(a, DataGridViewRow).Cells("BarCodeInfo").Value.ToString) _
                                           OrElse (CBool(CType(a, DataGridViewRow).Cells("NotSampleType").Value) AndAlso CType(a, DataGridViewRow).Cells("SampleType").Value.ToString() <> "") _
                                            Group a By CType(a, DataGridViewRow).Cells("BarCodeInfo").Value Into Group _
                                            Where Group.Count > 1 _
                                           Select Group.ToList()
                Else
                    'List of Barcodes
                    gdVwRwsBarcodeRepeated = From a In bsIncompleteSamplesDataGridView.Rows _
                                            Where CType(a, DataGridViewRow).Cells("BarCodeInfo").Value.ToString = Barcode _
                                           OrElse (CBool(CType(a, DataGridViewRow).Cells("NotSampleType").Value) AndAlso CType(a, DataGridViewRow).Cells("SampleType").Value.ToString() <> "")
                                            Group a By CType(a, DataGridViewRow).Cells("BarCodeInfo").Value Into Group _
                                            Where Group.Count > 1 AndAlso Value.ToString = Barcode _
                                           Select Group.ToList()
                End If

                For i As Integer = 0 To (gdVwRwsBarcodeRepeated.Count - 1)
                    If (String.IsNullOrEmpty(Barcode)) Then
                        barcode_repeated = CType(CType(gdVwRwsBarcodeRepeated(i), List(Of Object)).Item(0), DataGridViewRow).Cells("BarCodeInfo").Value.ToString()
                    Else
                        barcode_repeated = Barcode
                    End If

                    gdVwRwsSampleTypeVoid = From a In bsIncompleteSamplesDataGridView.Rows _
                                           Where (String.IsNullOrEmpty(CType(a, DataGridViewRow).Cells("SampleType").Value.ToString) _
                                          OrElse (CBool(CType(a, DataGridViewRow).Cells("NotSampleType").Value) AndAlso CType(a, DataGridViewRow).Cells("SampleType").Value.ToString() <> "")) _
                                         AndAlso CType(a, DataGridViewRow).Cells("BarCodeInfo").Value.ToString = barcode_repeated _
                                          Select a

                    For Each row As Object In gdVwRwsSampleTypeVoid
                        If (CType(row, DataGridViewRow).Cells("LisStatus").Value.ToString <> "ASKING") Then
                            anyRowWithError = True

                            CType(row, DataGridViewRow).Cells("EditableSampleType").Value = "TRUE"
                            If (IsDBNull(CType(row, DataGridViewRow).Cells("SampleType").Value.ToString()) OrElse _
                                String.IsNullOrEmpty(CType(row, DataGridViewRow).Cells("SampleType").Value.ToString())) Then
                                CType(row, DataGridViewRow).Cells("SampleType").ErrorText = errorBarcodeRepeated

                                If (firstRowWithRepeatedBarcode = -1) Then firstRowWithRepeatedBarcode = CType(row, DataGridViewRow).Index
                            End If
                            CType(CType(row, DataGridViewRow).Cells("SampleType"), DataGridViewComboBoxCell).DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                        End If
                    Next

                    gdVwRwsSampleTypeVoid = Nothing
                Next
                gdVwRwsBarcodeRepeated = Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetErrorDuplicatedBarcode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetErrorDuplicatedBarcode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return firstRowWithRepeatedBarcode
    End Function
#End Region

#Region "Events"
    '*************************************************************
    '* EVENTS FOR BUTTONS FOR MANUAL ENTERING OF SAMPLES DETAILS *
    '*************************************************************
    ''' <summary>
    ''' Open the auxiliary screen for Tests selection
    ''' </summary>
    ''' 
    Private Sub bsSearchTestsButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSearchTestsButton.Click
        Try
            SearchTests()
            '#If Not Debug Then
            Me.TopMost = True
            '#End If
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
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim TotalStartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            If (SaveChanges()) Then
                'Enable again the grid
                bsIncompleteSamplesDataGridView.Enabled = True

                'JC 13/05/2013
                'Check If current Rows Selected has test assigneds, then  Hide Sample of the Grid. 
                'The Samples are positioned on Form Close 
                Dim currentTestsDS As New SelectedTestsDS

                Dim mySampleID As String
                Dim myBarCodeInfo As String
                Dim myUrgency As Boolean
                Dim mySampleType As String
                For Each gdVwRow As DataGridViewRow In bsIncompleteSamplesDataGridView.SelectedRows
                    mySampleType = CStr(IIf(IsNothing(bsSampleTypeComboBox.SelectedValue), gdVwRow.Cells("SampleType").Value.ToString, bsSampleTypeComboBox.SelectedValue))

                    'If current Samples has not a Sample Type, this Samples will not hidden
                    If (String.IsNullOrEmpty(mySampleType)) Then
                        Continue For
                    End If

                    mySampleID = gdVwRow.Cells("ExternalPID").Value.ToString
                    myBarCodeInfo = gdVwRow.Cells("BarCodeInfo").Value.ToString
                    myUrgency = CBool(IIf(bsStatCheckbox.Enabled, bsStatCheckbox.Checked, gdVwRow.Cells("StatFlag").Value))

                    If (Not currentTestsDS Is Nothing) Then currentTestsDS.Clear()

                    'For this samples get all Test assinged for the same priority
                    FillSelectedTestsForPatient(mySampleID, myUrgency, mySampleType, myBarCodeInfo, currentTestsDS)

                    ''Check if there are any Sample Request for this SampleID and Priority
                    ''or user has selected some one Tests Window
                    'If (currentTestsDS.SelectedTestTable.Rows.Count > 0 OrElse _
                    '   (Not IsNothing(mySelectedTestsDS.SelectedTestTable) AndAlso mySelectedTestsDS.SelectedTestTable.Rows.Count > 0)) Then
                    '    If (hideRows Is Nothing) Then
                    '        hideRows = New List(Of Integer)
                    '    End If

                    '    'If has any Test, mark grid row as completed
                    '    hideRows.Add(CType(gdVwRow.Cells("CellNumber").Value, Integer))
                    'End If
                Next

                'Get the list of Incomplete Samples to load the DataGridView
                LoadIncompleteSamplesGrid()

                'Disable all controls in both areas: Import from LIS and Enter Samples Details
                EditButtonEnabled(False)
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Total function time = " & Now.Subtract(TotalStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            Me.Name & "." & (New System.Diagnostics.StackTrace()).GetFrame(0).GetMethod().Name, EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
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
            If (Not mySelectedTestsDS Is Nothing AndAlso mySelectedTestsDS.SelectedTestTable.Rows.Count > 0) OrElse _
               (bsSampleTypeComboBox.Enabled AndAlso Not bsSampleTypeComboBox.SelectedValue Is Nothing) OrElse _
                previousValueStat <> bsStatCheckbox.Checked Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    'Disable all controls in both areas: Import from LIS and Enter Samples Details
                    LIMSImportButtonEnabled()
                    EditButtonEnabled(False)
                    bsIncompleteSamplesDataGridView.ClearSelection()

                    'Enable again the grid
                    bsIncompleteSamplesDataGridView.Enabled = True

                    RepaintGridRows()
                    If (Not mySelectedTestsDS Is Nothing) Then mySelectedTestsDS.Clear()
                Else
                    bsSaveButton.Focus()
                End If
            Else
                bsScreenErrorProvider.Clear()

                'Disable all controls in both areas: Import from LIS and Enter Samples Details
                LIMSImportButtonEnabled()
                EditButtonEnabled(False)
                bsIncompleteSamplesDataGridView.ClearSelection()

                'Enable again the grid
                bsIncompleteSamplesDataGridView.Enabled = True

                RepaintGridRows()
                If (Not mySelectedTestsDS Is Nothing) Then mySelectedTestsDS.Clear()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    '*****************************************************
    '* EVENTS FOR THE GRID OF INCOMPLETE PATIENT SAMPLES *
    '*****************************************************
    ''' <summary>
    ''' Event for Select All Check Clicked
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC 25/03/2013 
    ''' </remarks>
    Private Sub bsHQSellectAllCheckBx_CheckedChanged(sender As Object, e As EventArgs) Handles bsHQSellectAllCheckBx.CheckedChanged
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim TotalStartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'If flag raise event Enabled
            If (bsHQSelectAllCheckBx_EventEnabled) Then

                If bsHQSellectAllCheckBx.CheckState = CheckState.Checked Then
                    Dim selectableRows As List(Of Object)
                    selectableRows = (From A In bsIncompleteSamplesDataGridView.Rows _
                                      Where CType(A, DataGridViewRow).Cells("LISStatus").Value.ToString <> "ASKING" _
                                    Select A).ToList()

                    For Each slRow As Object In selectableRows
                        CType(slRow, DataGridViewRow).Selected = True
                    Next

                Else
                    For Each dr As DataGridViewRow In bsIncompleteSamplesDataGridView.SelectedRows
                        dr.Selected = False
                    Next
                    bsHQSellectAllCheckBx.CheckState = CheckState.Unchecked
                End If

                LIMSImportButtonEnabled()
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Total function time = " & Now.Subtract(TotalStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            Me.Name & "." & (New System.Diagnostics.StackTrace()).GetFrame(0).GetMethod().Name, EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsHQSellectAllCheckBx_CheckedChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsHQSellectAllCheckBx_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsIncompleteSamplesDataGridView_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles bsIncompleteSamplesDataGridView.CellMouseDown
        Try
            If (e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0) Then

                If IsNothing(bsIncompleteSamplesDataGridView.CurrentCell) Then
                    ' Return
                    bsIncompleteSamplesDataGridView.CurrentCell = bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex)
                End If

                If (bsIncompleteSamplesDataGridView.Columns.Item(e.ColumnIndex).Name = "SampleType") Then
                    Dim currentValueSampleType As String = bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
                    Dim isDuplicated As Boolean = Not IsNothing(bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells("EditableSampleType").Value)

                    If (isDuplicated AndAlso bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells("LISStatus").Value.ToString() <> "ASKING") Then
                        bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).ReadOnly = False
                        bsIncompleteSamplesDataGridView.BeginEdit(True)
                    Else
                        If bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Selected AndAlso bsIncompleteSamplesDataGridView.SelectedRows.Count = 1 Then
                            clearSelection = True
                        End If
                    End If
                Else
                    If bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Selected AndAlso bsIncompleteSamplesDataGridView.SelectedRows.Count = 1 Then
                        clearSelection = True
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_CellMouseDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_CellMouseDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsIncompleteSamplesDataGridView_CellMouseUp(sender As Object, e As DataGridViewCellMouseEventArgs) Handles bsIncompleteSamplesDataGridView.CellMouseUp
        If clearSelection Then
            bsIncompleteSamplesDataGridView.ClearSelection()
            clearSelection = False
        End If
    End Sub

    ''' <summary>
    ''' Manages the selection/unselection of all 
    ''' </summary>
    Private Sub bsIncompleteSamplesDataGridView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsIncompleteSamplesDataGridView.KeyUp
        Try
            Select Case (e.KeyCode)
                Case Keys.Up
                    If (bsIncompleteSamplesDataGridView.CurrentRow.Index >= 0) Then
                        bsIncompleteSamplesDataGridView_SelectionChanged(Nothing, Nothing)
                    End If
                Case Keys.Down
                    If (bsIncompleteSamplesDataGridView.CurrentRow.Index < bsIncompleteSamplesDataGridView.Rows.Count) Then
                        bsIncompleteSamplesDataGridView_SelectionChanged(Nothing, Nothing)
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
    '''              JC 13/05/2013 - Disable Edit Button when there are not selected Rows or all the selected Rows have LISStatus = ASKING
    '''              SA 25/07/2013 - Disable Edit Button also when the user has clicked in HQ Button and the screen is waiting for the LIS answer
    ''' </remarks>
    Private Sub bsIncompleteSamplesDataGridView_SelectionChanged(sender As Object, e As EventArgs) Handles bsIncompleteSamplesDataGridView.SelectionChanged
        Try
            IncompletePatientSamplesCellMouseUp()

            Dim _selectedRowsAsking As List(Of Object)
            _selectedRowsAsking = (From A In bsIncompleteSamplesDataGridView.SelectedRows _
                                  Where CType(A, DataGridViewRow).Cells("LISStatus").Value.ToString = "ASKING" _
                                 Select A).ToList()

            For Each gdRow As Object In _selectedRowsAsking
                CType(gdRow, DataGridViewRow).Selected = False
            Next

            If (Not HQButtonUserClickAttribute) Then
                EditButtonEnabled(bsIncompleteSamplesDataGridView.SelectedRows.Count > 0 AndAlso _
                                 (bsStatCheckboxChecked OrElse bsStatCheckboxEnabled OrElse bsSearchTestsButtonEnabled OrElse _
                                  bsSampleTypeComboBoxEnabled OrElse bsSaveButtonEnabled))
            Else
                EditButtonEnabled(False)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_SelectionChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_SelectionChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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

    ''' <summary>
    ''' Change the value of Grid depending of Column 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC 25/03/2013 
    ''' Modified by: SA 24/07/2013 - It is not allowed to change the priority not only for Cells with LIS Status ASKING, but also for Cells containing IN USE Patient Samples.
    '''                              Action is not allowed neither when the Status of the active Work Session is ABORTED
    ''' </remarks>
    Private Sub bsIncompleteSamplesDataGridView_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles bsIncompleteSamplesDataGridView.CellMouseDoubleClick
        Try
            If (WSStatusAttribute <> "ABORTED" AndAlso Not HQButtonUserClickAttribute) Then
                If (e.RowIndex >= 0) Then
                    Select Case bsIncompleteSamplesDataGridView.Columns.Item(e.ColumnIndex).Name
                        Case "SampleClassIcon"
                            If (bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells("LISStatus").Value.ToString <> "ASKING") AndAlso _
                               (bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells("CellStatus").Value.ToString = "NO_INUSE") Then
                                Dim msgText As String
                                If (CBool(bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells("StatFlag").Value)) Then
                                    msgText = GlobalEnumerates.Messages.CHANGE_STAT_TO_ROUTINE.ToString
                                Else
                                    msgText = GlobalEnumerates.Messages.CHANGE_ROUTINE_TO_STAT.ToString
                                End If

                                If (ShowMessage(bsSamplesListLabel.Text, msgText, , Me) = Windows.Forms.DialogResult.Yes) Then
                                    ChangeStatusFlagRow(bsIncompleteSamplesDataGridView.Rows(e.RowIndex))

                                    Dim resultData As New GlobalDataTO
                                    Dim myBarcodePositionsWithNoRequests As New BarcodePositionsWithNoRequestsDelegate
                                    resultData = myBarcodePositionsWithNoRequests.HQUpdatePatientRequestedSamples(Nothing, CType(CType(bsIncompleteSamplesDataGridView.Rows(e.RowIndex).DataBoundItem, DataRowView).Row,  _
                                                                                                                  BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow))

                                    bsStatCheckbox.Checked = CBool(bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells("StatFlag").Value)
                                    bsStatCheckboxChecked = CBool(bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells("StatFlag").Value)
                                End If

                            End If
                    End Select
                    clearSelection = False
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_CellMouseDoubleClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_CellMouseDoubleClick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Force Refresh HQButton Enabled/Disabled without have to unfocus GridComboSampleType
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC 08/04/2013 
    ''' </remarks>
    Private Sub bsIncompleteSamplesDataGridView_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles bsIncompleteSamplesDataGridView.CurrentCellDirtyStateChanged
        Try
            If (Not IsNothing(bsIncompleteSamplesDataGridView.CurrentCell) AndAlso _
                bsIncompleteSamplesDataGridView.Columns(bsIncompleteSamplesDataGridView.CurrentCell.ColumnIndex).Name = "SampleType") Then
                bsIncompleteSamplesDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)

                'Error Protection
                If (IsNothing(bsIncompleteSamplesDataGridView.CurrentCell) _
                     OrElse bsIncompleteSamplesDataGridView.Columns(bsIncompleteSamplesDataGridView.CurrentCell.ColumnIndex).Name <> "SampleType") Then
                    Return
                End If

                If (String.IsNullOrEmpty(CType(bsIncompleteSamplesDataGridView.CurrentCell, DataGridViewComboBoxCell).Value.ToString)) Then
                    bsIncompleteSamplesDataGridView.CurrentCell.ErrorText = errorBarcodeRepeated
                Else
                    bsIncompleteSamplesDataGridView.CurrentCell.ErrorText = String.Empty
                End If

                Dim resultData As New GlobalDataTO
                Dim myBarcodePositionsWithNoRequests As New BarcodePositionsWithNoRequestsDelegate
                resultData = myBarcodePositionsWithNoRequests.HQUpdatePatientRequestedSamples(Nothing, CType(CType(bsIncompleteSamplesDataGridView.CurrentRow.DataBoundItem, DataRowView).Row,  _
                                                                                              BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow))

                LIMSImportButtonEnabled()
                IncompletePatientSamplesCellMouseUp()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_CurrentCellDirtyStateChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_CurrentCellDirtyStateChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Change the value of Grid depending of Column 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC 25/03/2013 
    ''' </remarks>
    Private Sub bsIncompleteSamplesDataGridView_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles bsIncompleteSamplesDataGridView.CellMouseClick
        Try
            If (e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0) Then
                If (bsIncompleteSamplesDataGridView.Columns.Item(e.ColumnIndex).Name = "SampleType") Then
                    Dim currentValueSampleType As String = bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString
                    Dim isDuplicated As Boolean = Not IsNothing(bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells("EditableSampleType").Value)

                    If (isDuplicated AndAlso bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells("LISStatus").Value.ToString() <> "ASKING") Then
                        anyRowWithError = True

                        bsIncompleteSamplesDataGridView.BeginEdit(True)
                        bsIncompleteSamplesDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).ReadOnly = False
                        DirectCast(bsIncompleteSamplesDataGridView.EditingControl, DataGridViewComboBoxEditingControl).DroppedDown = True
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_CellMouseClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_CellMouseClick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' After SampleType Changed update the Error's Grid 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC 25/03/2013 
    ''' </remarks>
    Private Sub bsIncompleteSamplesDataGridView_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles bsIncompleteSamplesDataGridView.CellEndEdit
        Try
            Dim barcode As String = bsIncompleteSamplesDataGridView.CurrentRow.Cells("BarCodeInfo").Value.ToString

            UnsetErrorDuplicatedBarcode(barcode)
            SetErrorDuplicatedBarcode(barcode)
            LIMSImportButtonEnabled()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_CellEndEdit ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_CellEndEdit ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Mark with error warning as repeated Grid ComboBox 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC 09/04/2013 - When user sort again clicking on header cell, error alerts and combobox disappear
    ''' </remarks>
    Private Sub bsIncompleteSamplesDataGridView_Sorted(sender As Object, e As EventArgs) Handles bsIncompleteSamplesDataGridView.Sorted
        Try
            SetErrorDuplicatedBarcode()
            RepaintGridRows()
            LIMSImportButtonEnabled()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsIncompleteSamplesDataGridView_Sorted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsIncompleteSamplesDataGridView_Sorted ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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
    Private Sub HQBarcode_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            e.Cancel = (Not continueClosing)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".HQBarcode_FormClosing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".HQBarcode_FormClosing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 14/09/2011 
    ''' </remarks>
    Private Sub HQBarcode_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then bsExitButton.PerformClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".HQBarcode_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".HQBarcode_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen loading
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/08/2011
    ''' Modified by: SA 14/09/2011 - Added the screen centering
    ''' </remarks>
    Private Sub HQBarcode_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'The screen should appear always centered regarding the Main MDI
            Dim myLocation As Point = IAx00MainMDI.PointToScreen(Point.Empty)
            Dim mySize As Size = IAx00MainMDI.Size

            myNewLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 20) 'AG + RH 03/04/2012 - add - 20
            Me.Location = myNewLocation

            ScreenLoad()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".HQBarcode_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".HQBarcode_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' If the screen is opened during the automatic process of LIS WS creation, sent a HostQuery for all selected specimen automatically
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 12/07/2013
    ''' </remarks>
    Private Sub HQBarcode_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            If (AutoWSCreationWithLISModeAttribute AndAlso OpenByAutomaticProcessAttribute) Then
                Dim autoProcessUserAnswer As DialogResult = DialogResult.OK

                'Check if there are some patient tube in samples rotor
                autoProcessUserAnswer = IAx00MainMDI.CheckForExceptionsInAutoCreateWSWithLISProcess(2, Me, bsIncompleteSamplesDataGridView.Rows.Count)
                If (autoProcessUserAnswer = DialogResult.Yes) Then
                    'Positive case. No exceptions

                    'Then evaluate the 2on level exception rule
                    'Before query by specimen check the LIS status
                    autoProcessUserAnswer = IAx00MainMDI.CheckForExceptionsInAutoCreateWSWithLISProcess(1, Me)
                    If (autoProcessUserAnswer = DialogResult.Yes) Then
                        If Not bsLIMSImportButton.Enabled Then bsLIMSImportButton.Enabled = True 'AG 19/11/2013 - #1396-c (Protection against bar sample barcodes)

                        'Positive case. No exceptions
                        bsLIMSImportButton.PerformClick()
                        bsHQSellectAllCheckBx.Enabled = False
                        bsExitButton.Enabled = False

                        'TR 16/07/2013 - Send the parameter with the number of selected specimens to activate the waiting timer
                        IAx00MainMDI.EnableLISWaitTimer(True, bsIncompleteSamplesDataGridView.Rows.Count)
                        Cursor = Cursors.WaitCursor

                    ElseIf (autoProcessUserAnswer = DialogResult.OK) Then 'User answers stops the automatic process
                        'Automatic process aborted
                        IAx00MainMDI.SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitAutomaticProcesAndStop)
                        IAx00MainMDI.InitializeAutoWSFlags()
                        CloseScreen()

                    Else 'User answers 'Cancel' -> stop process but continue executing WorkSession
                        IAx00MainMDI.SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitHostQueryNotAvailableButGoToRunning)
                        CloseScreen()
                    End If

                ElseIf (autoProcessUserAnswer = DialogResult.OK) Then 'User answers stops the automatic process
                    'Automatic process aborted
                    IAx00MainMDI.SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitAutomaticProcesAndStop)
                    IAx00MainMDI.InitializeAutoWSFlags()
                    CloseScreen()

                Else 'User answers 'Cancel' -> stop process but continue executing WorkSession
                    IAx00MainMDI.SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitHostQueryNotAvailableButGoToRunning)
                    CloseScreen()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".HQBarcode_Shown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".HQBarcode_Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Execute the process of import details of the selected Incomplete Patient Samples from LIS
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JC
    ''' Modified by: SA 25/07/2013 - If the button click was executed by a final User, all fields and buttons are disabled
    '''                              and the screen waits until LIS sent Orders for all queried Specimens or until the wait
    '''                              interval finishes
    '''              XB 26/08/2013 - Sort bsIncompleteSamplesDataGridView.SelectedRows in ascending order (rotor position) instead default sorting (LIFO)  
    '''                              when the selected items are requested to LIS - bugstracking #1264
    ''' </remarks>
    Private Sub bsLIMSImportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsLIMSImportButton.Click
        Try

            'AG 24/02/2014 - use parameter MAX_APP_MEMORYUSAGE into performance counters (but do not show message here!!!) - ' XB 18/02/2014 BT #1499
            Dim PCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage) 'AG 24/02/2014 - #1520 add parameter
            PCounters.GetAllCounters()
            PCounters = Nothing
            ' XB 18/02/2014 BT #1499

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim TotalStartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Dim listBarcodesSentToLis As New List(Of String)
            Dim myBarcodePositionsWithNoRequests As New BarcodePositionsWithNoRequestsDelegate

            ' XB 26/08/2013
            'Get Selected Samples to Send
            'For Each gdVwRow As Object In bsIncompleteSamplesDataGridView.SelectedRows
            '    Dim row As DataGridViewRow = CType(gdVwRow, DataGridViewRow)

            '    SetRowAsking(row)
            '    If (Not myBarcodePositionsWithNoRequests.HQUpdatePatientRequestedSamples(Nothing, CType(DirectCast(row.DataBoundItem, System.Data.DataRowView).Row,  _
            '                                                                             Biosystems.Ax00.Types.BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow)).HasError) Then
            '        If (Not listBarcodesSentToLis.Contains(row.Cells("BarCodeInfo").Value.ToString)) Then
            '            listBarcodesSentToLis.Add(row.Cells("BarCodeInfo").Value.ToString)
            '        End If
            '    End If
            'Next

            Dim query As IEnumerable(Of DataGridViewRow) = _
                  From item As DataGridViewRow In bsIncompleteSamplesDataGridView.SelectedRows.Cast(Of DataGridViewRow)() _
                  Order By item.Cells("CellNumber").Value Ascending _
                  Select item

            Dim rows As List(Of DataGridViewRow) = query.ToList()

            For Each row As DataGridViewRow In rows
                SetRowAsking(row)
                If (Not myBarcodePositionsWithNoRequests.HQUpdatePatientRequestedSamples(Nothing, CType(DirectCast(row.DataBoundItem, System.Data.DataRowView).Row,  _
                                                                                         Biosystems.Ax00.Types.BarcodePositionsWithNoRequestsDS.twksWSBarcodePositionsWithNoRequestsRow)).HasError) Then
                    If (Not listBarcodesSentToLis.Contains(row.Cells("BarCodeInfo").Value.ToString)) Then
                        listBarcodesSentToLis.Add(row.Cells("BarCodeInfo").Value.ToString)
                    End If
                End If
            Next
            ' XB 26/08/2013

            'Request to LIS, AND SET AS ASKING
            IAx00MainMDI.autoWSCreationWithLISMode = True  'SA 30/07/2013
            IAx00MainMDI.InvokeLISHostQuery(listBarcodesSentToLis)

            'Refresh Grid and Controls
            RepaintGridRows()
            LIMSImportButtonEnabled()
            bsIncompleteSamplesDataGridView_SelectionChanged(Nothing, Nothing)
            bsIncompleteSamplesDataGridView_SelectionChanged(sender, e)

            HQButtonUserClickAttribute = (Not OpenByAutomaticProcessAttribute AndAlso Not AutoWSCreationWithLISModeAttribute)
            If (HQButtonUserClickAttribute) Then
                DisableAllFields()
                bsExitButton.Enabled = False
                bsHQSellectAllCheckBx.Enabled = False

                'Activate the Main MDI variable that indicates a HQ was sent by a final User
                IAx00MainMDI.SetHQProcessByUserFlag(True)
                IAx00MainMDI.SetAutomateProcessStatusValue(GlobalEnumerates.LISautomateProcessSteps.subProcessAskBySpecimen)

                'Send the parameter with the number of selected specimens to activate the waiting timer
                IAx00MainMDI.EnableLISWaitTimer(True, bsIncompleteSamplesDataGridView.SelectedRows.Count)
                IWSRotorPositions.HQButtonUserClick = True
                Cursor = Cursors.WaitCursor
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Total function time = " & Now.Subtract(TotalStartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            Me.Name & "." & (New System.Diagnostics.StackTrace()).GetFrame(0).GetMethod().Name, EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsLIMSImportButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsLIMSImportButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Start Edition Of Manual Edition of Sample
    ''' </summary>
    Private Sub EditButton_Click(sender As Object, e As EventArgs) Handles EditButton.Click
        Try
            EnterSamplesDetailsEnabled(EditButton.Enabled)
            EditButton.Enabled = False
            previousValueStat = bsStatCheckbox.Checked
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen closing
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 08/05/2013 - Before close the screen, call the function that updates the active WS  
    ''' </remarks>
    Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            'AG 24/02/2014 - use parameter MAX_APP_MEMORYUSAGE into performance counters (but do not show message here!!!) - ' XB 18/02/2014 BT #1499
            Dim PCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage) 'AG 24/02/2014 - #1520 add parameter
            PCounters.GetAllCounters()
            PCounters = Nothing
            ' XB 18/02/2014 BT #1499

            CloseScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CloseButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CloseButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region
End Class



Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.CommunicationsSwFw

Public Class IWSTestSelectionAuxScreen
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Constants"

    'Width of Test cells in the screen grids
    Private Const GRID_CELLS_WIDTH As Integer = 135
    Private Const COLUMN_COUNT As Integer = 5 'RH 12/03/2012

    'RH 19/06/2012 Use constants for colors used in the grids
    Private INPROCESS_BACK_COLOR As Color = Color.Gold
    Private INPROCESS_FORE_COLOR As Color = Color.Black

    Private SELECTED_BACK_COLOR As Color = Color.LightSlateGray
    Private SELECTED_FORE_COLOR As Color = Color.White

    Private LOCKED_BACK_COLOR As Color = Color.CornflowerBlue
    Private LOCKED_FORE_COLOR As Color = Color.White

    Private NOTSELECTED_BACK_COLOR As Color = Color.White
    Private NOTSELECTED_FORE_COLOR As Color = Color.Black

    Private UNSELECTED_BACK_COLOR As Color = Color.IndianRed
    Private UNSELECTED_FORE_COLOR As Color = Color.Black

    'SG 13/03/2013
    Private LIS_REQUESTED_BACK_COLOR As Color = Color.MediumPurple
    Private LIS_REQUESTED_FORE_COLOR As Color = Color.White

#End Region

#Region "Declarations"

    'To load all Test Profiles with their Tests
    Private testProfilesList As New TestProfileTestsDS

    'DataSet for all Standard, Calculated, ISE and Off-System Tests
    Private standardTestList As New SelectedTestsDS
    Private calculatedTestList As New SelectedTestsDS
    Private iseTestList As New SelectedTestsDS
    Private offSystemTestList As New SelectedTestsDS

    Private calcTestComponents As New FormulasDS

    'To control when a Profile Node is selected/unselected
    Private chkChange As Boolean = False
    'Private checkedValue As Boolean = False

    'To control when clear the warning area
    Private numSTDRed As Integer = 0

    'To control original selection
    Private selectTestList As New SelectedTestsDS()

    'To control if maximum number of Patient Order Tests has been reached
    Private maxPatientOTsReached As Boolean = False

    'To control multiple Tests selection in grid of Standard Tests
    Private lastRowClickedSTD As Integer = -1
    Private lastColClickedSTD As Integer = -1
    Private lastSelStatusSTD As Boolean = False
    Private shiftKeyIsPressedSTD As Boolean = False

    'To control multiple Tests selection in grid of Calculated Tests
    Private lastRowClickedCALC As Integer = -1
    Private lastColClickedCALC As Integer = -1
    Private lastSelStatusCALC As Boolean = False
    Private shiftKeyIsPressedCALC As Boolean = False

    'To control multiple Tests selection in grid of ISE Tests
    Private lastRowClickedISE As Integer = -1
    Private lastColClickedISE As Integer = -1
    Private lastSelStatusISE As Boolean = False
    Private shiftKeyIsPressedISE As Boolean = False

    'To control multiple Tests selection in grid of OffSystem Tests
    Private lastRowClickedOffSystem As Integer = -1
    Private lastColClickedOffSystem As Integer = -1
    Private lastSelStatusOffSystem As Boolean = False
    Private shiftKeyIsPressedOffSystem As Boolean = False

    'To get data of the Analyzer
    Private mdiAnalyzerCopy As AnalyzerManager

    'To avoid the screen movement
    Dim myNewLocation As Point

#End Region

#Region "Attributes"
    Private SampleClassAttribute As String
    Private SampleTypeAttribute As String
    Private SampleTypeNameAttribute As String
    Private ListOfSelectedTestsAttribute As New SelectedTestsDS()
    Private SelectedTestsInDifPriorityAttribute As New SelectedTestsDS
    Private PatientIDAttribute As String
    Private MaxValuesAttribute As New MaxOrderTestsValuesDS()
    Private WorkingModelAttribute As String = GlbSourceScreen.STANDARD.ToString
    Private ControlIDAttribute As Integer = -1

    'BT #1494 - Used to indicate if at least one of the selected STANDARD Tests have incomplete programming
    Private IncompleteTestAttribute As Boolean = False
#End Region

#Region "Properties"
    ''' <summary>
    ''' Screen property to indicate if at least one of the selected STANDARD Tests have incomplete programming
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 28/04/2014 - BT #1494
    ''' </remarks>
    Public ReadOnly Property IncompleteTest() As Boolean
        Get
            Return IncompleteTestAttribute
        End Get
    End Property

    ''' <summary>
    ''' Code of the SampleClass (sent from the previous screen).  Needed to filter the Tests to shown
    ''' </summary>
    ''' <remarks></remarks>
    Public WriteOnly Property SampleClass() As String
        Set(ByVal value As String)
            SampleClassAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Indicates if the screen is opened from WS Preparation (Standard mode) or from the Controls
    ''' Programming screen
    ''' </summary>
    ''' <remarks></remarks>
    Public WriteOnly Property WorkingModel() As String
        Set(ByVal value As String)
            WorkingModelAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When the screen is used to manage the linking of Tests/SampleTypes to a Control, this property
    ''' contains the Control Identifier to get the Tests/SampleTypes currently linked to an existing Control
    ''' </summary>
    ''' <remarks></remarks>
    Public WriteOnly Property ControlID() As Integer
        Set(ByVal value As Integer)
            ControlIDAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Code of the SampleType for which Tests have to be loaded (sent from the previous screen)
    ''' </summary>
    ''' <remarks></remarks>
    Public Property SampleType() As String
        Get
            Return SampleTypeAttribute
        End Get
        Set(ByVal value As String)
            SampleTypeAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Name of the SampleType for which Tests have to be loaded (sent from the previous screen)
    ''' </summary>
    ''' <remarks></remarks>
    Public WriteOnly Property SampleTypeName() As String
        Set(ByVal value As String)
            SampleTypeNameAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When the screen is opened, it contains the list of Tests (linked to the informed Sample Type) that have 
    ''' to be marked as selected (sent from the previous screen)
    ''' </summary>
    ''' <remarks></remarks>
    Public Property ListOfSelectedTests() As SelectedTestsDS
        Get
            Return ListOfSelectedTestsAttribute
        End Get
        Set(ByVal value As SelectedTestsDS)
            ListOfSelectedTestsAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' When the screen is opened to select Tests for Patients, it contains the list of Tests (linked to the informed
    ''' SampleType) that have to be marked as locked (sent from the previous screen). A Test will be locked when it has 
    ''' been requested for the same Patient with a different Stat Flag o priority
    ''' </summary>
    ''' <remarks></remarks>
    Public WriteOnly Property SelectedTestsInDifPriority() As SelectedTestsDS
        Set(ByVal value As SelectedTestsDS)
            SelectedTestsInDifPriorityAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Identifier of the PatientID/SampleID. Informed only when SampleClass = PATIENT, but not mandatory
    ''' </summary>
    ''' <remarks></remarks>
    Public WriteOnly Property PatientID() As String
        Set(ByVal value As String)
            PatientIDAttribute = value
        End Set
    End Property

    ''' <summary>
    ''' Values required to verify if it is needed to show the warning of maximum number
    ''' of Patient Order Tests reached 
    ''' </summary>
    ''' <remarks></remarks>
    Public WriteOnly Property MaxValues() As MaxOrderTestsValuesDS
        Set(ByVal value As MaxOrderTestsValuesDS)
            MaxValuesAttribute = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Set the opacity to 0 to hide the form; this is because there are some validations 
        'to do before showing the form.
        Me.Opacity = 0
    End Sub
#End Region

#Region "Methods for Standard Screen Model"
    ''' <summary>
    ''' When a Test (whatever TestType) included in a selected Test Profile is clicked to unselect it, besides unselect the Test Profile in the TreeView,
    ''' the Profile information (ID and Name) has to be removed for the rest of Tests in the affected Profile (these Tests remain selected, but they are not 
    ''' linked to the unselected Test Profile anymore)
    ''' </summary>
    ''' <param name="pTestType">TestType Code</param>
    ''' <param name="pTestID">Test Identifier</param>
    ''' <param name="pTestProfileID">Identifier of the unselected Test Profile</param>
    ''' <remarks>
    ''' Created by: SA 21/05/2014 - BT #1633
    ''' </remarks>
    Private Sub DeleteProfileInformation(ByVal pTestType As String, pTestID As Integer, ByVal pTestProfileID As Integer)
        Try
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow) = Nothing

            'Search the Test in the proper SelectedTestsDS according its type
            Select Case (pTestType)
                Case "STD"
                    qSelectedTest = (From a As SelectedTestsDS.SelectedTestTableRow In standardTestList.SelectedTestTable _
                                    Where a.TestID = pTestID AndAlso Not a.IsTestProfileIDNull AndAlso a.TestProfileID = pTestProfileID _
                                   Select a).ToList()
                Case "ISE"
                    qSelectedTest = (From a As SelectedTestsDS.SelectedTestTableRow In iseTestList.SelectedTestTable _
                                    Where a.TestID = pTestID AndAlso Not a.IsTestProfileIDNull AndAlso a.TestProfileID = pTestProfileID _
                                   Select a).ToList()
                Case "CALC"
                    qSelectedTest = (From a As SelectedTestsDS.SelectedTestTableRow In calculatedTestList.SelectedTestTable _
                                    Where a.TestID = pTestID AndAlso Not a.IsTestProfileIDNull AndAlso a.TestProfileID = pTestProfileID _
                                   Select a).ToList()
                Case "OFFS"
                    qSelectedTest = (From a As SelectedTestsDS.SelectedTestTableRow In offSystemTestList.SelectedTestTable _
                                    Where a.TestID = pTestID AndAlso Not a.IsTestProfileIDNull AndAlso a.TestProfileID = pTestProfileID _
                                   Select a).ToList()
            End Select

            If (qSelectedTest.Count > 0) Then
                'If the Test is included in another selected Test Profile, inform in the SelectedTestsDS the ID and Name of this Profile;
                'otherwise, set both fields to Null
                Dim myTreeNode As New TreeNode()
                myTreeNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)

                If (myTreeNode.Name <> String.Empty) Then
                    If (myTreeNode.Parent Is Nothing) Then
                        qSelectedTest.First.TestProfileID = CType(myTreeNode.Name, Integer)
                        qSelectedTest.First.TestProfileName = myTreeNode.Text
                    Else
                        qSelectedTest.First.TestProfileID = CType(myTreeNode.Parent.Name, Integer)
                        qSelectedTest.First.TestProfileName = myTreeNode.Parent.Text
                    End If
                Else
                    qSelectedTest.First.SetTestProfileIDNull()
                    qSelectedTest.First.SetTestProfileNameNull()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteProfileInformation", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteProfileInformation", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization when the screen is opened from the screen of WS Preparation
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 09/02/2010
    ''' Modified by: SA 21/04/2010 - Close screen after shown the error message when an exception happens
    '''              SA 04/05/2010 - Added the getting of available Calculated Tests
    '''              SA 12/05/2010 - Add fields to show the PatientID when it has been informed
    '''              PG 13/10/2010 - Get the current language
    '''              SA 22/10/2010 - Function GetListForTestProfiles changed by GetBySampleType
    '''              SA 02/11/2010 - When the screen is opened to select Tests for Patient Samples, call
    '''                              function MarkSelectedProfilesOnTreeView to mark in the Profiles TreeView
    '''                              all selected Profiles (if any)
    '''              DL 21/10/2010 - Added the getting of available ISE Tests
    '''              DL 29/11/2010 - Added the getting of available OffSystem Tests
    '''              SA 01/02/2011 - When selected SampleClass is PATIENT, the screen has to be opened if there is 
    '''                              at least a Test (whatever Test Type) defined for the selected SampleType (currently the 
    '''                              screen is not opened if there are not Standard Tests, and it is wrong)
    '''              RH 12/03/2012 - When an error happens, execute Return to prevent the execution of the following lines
    '''              SA 19/06/2012 - Get ISE Tests also when SampleClass is CTRL
    '''              AG 29/08/2014 - BA-1869 customize test selection visibility and order (new parameter TRUE for GetList and GetBySampleType methods)
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Get Multilanguage Texts for all Screen Controls
            GetScreenLabels(currentLanguage)

            'Get Icons for Screen Buttons
            PrepareButtons()

            bsSampleTypeTextBox.BackColor = Color.LightGray
            bsSampleTypeTextBox.Text = SampleTypeNameAttribute

            If (SampleClassAttribute = "PATIENT") Then
                'Show Label and TextBox for PatientID and inform it with the value received
                bsPatientLabel.Visible = True
                bsPatientTextBox.Visible = True

                bsPatientTextBox.BackColor = Color.LightGray
                bsPatientTextBox.Text = PatientIDAttribute

                'Show Label and TreeView for Test Profiles
                bsTestProfilesLabel.Visible = True
                bsProfilesTreeView.Visible = True
            Else
                'Hide Label and TextBox for PatientID
                bsPatientLabel.Visible = False
                bsPatientTextBox.Visible = False

                'Hide Label and TreeView for Test Profiles
                bsTestProfilesLabel.Visible = False
                bsProfilesTreeView.Visible = False
            End If

            Dim numOfTests As Integer = 0
            Dim myGlobalDataTO As GlobalDataTO
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)

            Dim customizedTestSelection As Boolean = True 'AG 01/09/2014 - BA-1869 set TRUE to final code / leave FALSE during develop
            'Get the list of available Standard Tests according the selected SampleClass and SampleType
            Dim myTestsDelegate As New TestsDelegate()
            If (SampleClassAttribute = "BLANK") Then
                'All Tests have to be loaded, without filtering them by Sample Type
                myGlobalDataTO = myTestsDelegate.GetList(Nothing, customizedTestSelection) 'AG 29/08/2014 BA-1869 pCustomizedTestSelection
            Else
                'For CALIBRATORS --> Only Tests using for the SampleType an Experimental Calibrator, or an Alternative one based
                'in an Experimental, will be loaded
                'For CONTROLS --> Only Tests having a Control defined for the SampleType will be loaded 
                'For PATIENTS --> All Tests using the SampleType will be loaded 
                myGlobalDataTO = myTestsDelegate.GetBySampleType(Nothing, SampleTypeAttribute, SampleClassAttribute, customizedTestSelection) 'AG 29/08/2014 BA-1869 pCustomizedTestSelection
            End If

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myTestsDS As TestsDS = DirectCast(myGlobalDataTO.SetDatos, TestsDS)

                If (myTestsDS.tparTests.Rows.Count > 0) Then
                    numOfTests += myTestsDS.tparTests.Rows.Count

                    'If value is Patient then fill the TreeView of Test Profiles
                    If (SampleClassAttribute = "PATIENT") Then
                        'Get the data for the TreeView
                        Dim myTestProfileDelegate As New TestProfilesDelegate()
                        myGlobalDataTO = myTestProfileDelegate.GetProfilesBySampleType(Nothing, SampleTypeAttribute, customizedTestSelection) 'AG 29/08/2014 BA-1869 pCustomizedTestSelection

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            'Set value of global variable testProfileList, needed to fill the TreeView of Test Profiles
                            testProfilesList = DirectCast(myGlobalDataTO.SetDatos, TestProfileTestsDS)
                            FillTreeView(testProfilesList)
                        Else
                            'Show the Error Message
                            ShowMessage(Me.Name & ".InitializeScreen", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            Me.Close()
                            Return
                        End If
                    End If

                    'Load the Standard Tests in bsTestListDataGridView
                    FillAndMarkTestListGridView(myTestsDS)

                    'Initialize the list of Selected Tests
                    qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                     Where a.Selected = True _
                                     Select a).ToList()

                    'Add all selected Tests to the DataSet to return
                    selectTestList = New SelectedTestsDS
                    For Each mySelectedRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                        selectTestList.SelectedTestTable.ImportRow(mySelectedRow)
                    Next
                End If
            Else
                'Error getting the list of available Standard Tests
                ShowMessage(Me.Name & ".InitializeScreen", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Me.Close()
                Return
            End If

            'Get the list of available Calculated Tests...
            If (Not myGlobalDataTO.HasError) Then
                If (SampleClassAttribute = "PATIENT") Then
                    Dim myCalcTestDelegate As New CalculatedTestsDelegate
                    myGlobalDataTO = myCalcTestDelegate.GetBySampleType(Nothing, SampleTypeAttribute, customizedTestSelection) 'AG 29/08/2014 BA-1869 pCustomizedTestSelection

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myCalcTestsDS As CalculatedTestsDS = DirectCast(myGlobalDataTO.SetDatos, CalculatedTestsDS)
                        If (myCalcTestsDS.tparCalculatedTests.Rows.Count > 0) Then
                            numOfTests += myCalcTestsDS.tparCalculatedTests.Rows.Count

                            'Load the Calculated Tests in bsCalcTestDataGridView
                            FillAndMarkCalcTestListGridView(myCalcTestsDS)

                            qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                                            Where a.Selected = True _
                                           Select a).ToList()

                            'Add all selected Calculated Tests to the DataSet 
                            For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                selectTestList.SelectedTestTable.ImportRow(selectedRow)
                            Next
                        End If
                    Else
                        'Error getting the list of available Calculated Tests
                        ShowMessage(Me.Name & ".InitializeScreen", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        Me.Close()
                        Return
                    End If
                End If
            End If

            'Get the list of available ISE Tests...
            If (Not myGlobalDataTO.HasError) Then
                If (SampleClassAttribute = "PATIENT" OrElse SampleClassAttribute = "CTRL") Then
                    Dim myISETestDelegate As New ISETestsDelegate
                    myGlobalDataTO = myISETestDelegate.GetBySampleType(Nothing, SampleTypeAttribute, (SampleClassAttribute = "CTRL"), customizedTestSelection) 'AG 01/09/2014 BA-1869 pCustomizedTestSelection

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myISETestsDS As ISETestsDS = DirectCast(myGlobalDataTO.SetDatos, ISETestsDS)
                        If (myISETestsDS.tparISETests.Rows.Count > 0) Then
                            numOfTests += myISETestsDS.tparISETests.Rows.Count

                            'Load the ISE Tests in bsISETestDataGridView
                            FillAndMarkISETestListGridView(myISETestsDS)

                            qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                             Where a.Selected = True _
                                             Select a).ToList()

                            'Add all selected ISE Tests to the DataSet 
                            For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                selectTestList.SelectedTestTable.ImportRow(selectedRow)
                            Next
                        End If
                    Else
                        'Error getting the list of available ISE Tests
                        ShowMessage(Me.Name & ".InitializeScreen", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        Me.Close()
                        Return
                    End If
                End If
            End If

            'Get the list of available Off-System Tests...
            If (Not myGlobalDataTO.HasError) Then
                If (SampleClassAttribute = "PATIENT") Then
                    Dim myOffSystemTestDelegate As New OffSystemTestsDelegate
                    myGlobalDataTO = myOffSystemTestDelegate.GetBySampleType(Nothing, SampleTypeAttribute, customizedTestSelection) 'AG 01/09/2014 BA-1869 pCustomizedTestSelection

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myOffSystemTestsDS As OffSystemTestsDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)
                        If (myOffSystemTestsDS.tparOffSystemTests.Rows.Count > 0) Then
                            numOfTests += myOffSystemTestsDS.tparOffSystemTests.Rows.Count

                            'Load the OffSystem Tests in bsOffSystemTestDataGridView
                            FillAndMarkOffSystemTestListGridView(myOffSystemTestsDS)

                            qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                                             Where a.Selected = True _
                                             Select a).ToList()

                            'Add all selected Off-System Tests to the DataSet 
                            For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                selectTestList.SelectedTestTable.ImportRow(selectedRow)
                            Next
                        End If
                    Else
                        'Error getting the list of available Off-System Tests
                        ShowMessage(Me.Name & ".InitializeScreen", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                        Me.Close()
                        Return
                    End If
                End If
            End If

            If (Not myGlobalDataTO.HasError) Then
                If (numOfTests = 0) Then
                    'Show the Error Message acording the SampleClass 
                    Dim errorMsg As String = ""
                    If (SampleClassAttribute = "PATIENT") Then
                        '...PATIENTS - There are not Tests for the specified Sample Type
                        errorMsg = GlobalEnumerates.Messages.TESTS_NOT_FOUND.ToString
                    ElseIf (SampleClassAttribute = "CALIB") Then
                        '...CALIBRATORS - There are not Tests using Experimental Calibrators for the specified SampleType
                        errorMsg = GlobalEnumerates.Messages.NO_CALIBRATORS_FOR_SAMPLETYPE.ToString
                    ElseIf (SampleClassAttribute = "CTRL") Then
                        '...CONTROLS - There are not Tests with Control definition for the specified SampleType
                        errorMsg = GlobalEnumerates.Messages.NO_CONTROLS_FOR_SAMPLETYPE.ToString
                    End If
                    ShowMessage(Me.Name & ".InitializeScreen", errorMsg, , Me)
                    Me.Close()
                    Return
                Else
                    'Mark all selected Profiles in the TreeView and verify if some Tests have to be locked for selection 
                    If (SampleClassAttribute = "PATIENT") Then
                        MarkSelectedProfilesOnTreeView()
                        VerifyTestsLocking()
                    End If

                    'Set the opacity to 100 to make visible the form.
                    Me.Opacity = 100
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Search Icons for screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 12/05/2010 
    ''' Modified by: SA 03/11/2010 - Set value of Image Property instead of BackgroundImage Property
    '''              DL 23/03/2012 - Changed the Icon used for the Warning Area
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'ACCEPT Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'Warning Icon in Warnings area
            auxIconName = GetIconName("STUS_WITHERRS")
            If (auxIconName <> "") Then
                bsWarningIconPictureBox.ImageLocation = iconPath & auxIconName
                bsWarningIconPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Verify if there are Tests to mark as locked for selection
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 12/05/2010
    ''' Modified by: SA 29/10/2010 - Added locking verification for ISE Tests; for Calculated Tests, add verification
    '''                              to lock Profiles in which the locked Calculated Tests are included
    '''              SA 02/12/2010 - Added locking verification for Off-System Tests
    ''' </remarks>
    Private Sub VerifyTestsLocking()
        Try
            If (SelectedTestsInDifPriorityAttribute.SelectedTestTable.Rows.Count > 0) Then
                'Search each Test in the list of Tests to mark it as locked
                For Each lockedTest As SelectedTestsDS.SelectedTestTableRow In SelectedTestsInDifPriorityAttribute.SelectedTestTable.Rows
                    Dim currentTestID As Integer = lockedTest.TestID
                    Dim currentSampleType As String = lockedTest.SampleType
                    Dim currentTestType As String = lockedTest.TestType

                    If (currentTestType = "STD") Then
                        'Process for locked Standard Tests...
                        Dim lstStandardTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                        lstStandardTest = (From a In standardTestList.SelectedTestTable _
                                          Where a.TestID = currentTestID _
                                        AndAlso a.SampleType = currentSampleType _
                                         Select a).ToList

                        If (lstStandardTest.Count = 1) Then
                            'Change the OTStatus to indicate the Test is locked
                            lstStandardTest(0).OTStatus = "STATLOCK"

                            'Change the BackColor/ForeColor of the grid cell containing the Test to indicate that it cannot be selected/unselected
                            bsTestListDataGridView.Rows(lstStandardTest(0).Row).Cells(lstStandardTest(0).Col).Style.BackColor = LOCKED_BACK_COLOR
                            bsTestListDataGridView.Rows(lstStandardTest(0).Row).Cells(lstStandardTest(0).Col).Style.SelectionBackColor = LOCKED_BACK_COLOR
                            bsTestListDataGridView.Rows(lstStandardTest(0).Row).Cells(lstStandardTest(0).Col).Style.ForeColor = LOCKED_FORE_COLOR
                            bsTestListDataGridView.Rows(lstStandardTest(0).Row).Cells(lstStandardTest(0).Col).Style.SelectionForeColor = LOCKED_FORE_COLOR

                            'If the locked Test is included in a Test Profile, check it in the TreeView
                            If (Not lockedTest.IsTestProfileIDNull) Then
                                If (bsProfilesTreeView.Nodes.Find(lockedTest.TestProfileID.ToString(), False).Length <> 0) Then
                                    Dim myTreeNode As New TreeNode
                                    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(lockedTest.TestProfileID.ToString(), True).First, TreeNode)
                                    myTreeNode.Checked = True
                                    myTreeNode.ForeColor = Color.CornflowerBlue

                                    For Each myNode As TreeNode In myTreeNode.Nodes
                                        myNode.Checked = True
                                        myNode.ForeColor = Color.CornflowerBlue
                                    Next
                                End If
                            End If

                            'Lock all Test Profiles that include the locked Test
                            LockTestProfile(lstStandardTest(0).TestKey)
                        End If

                    ElseIf (currentTestType = "CALC") Then
                        'Process for locked Calculated Tests...
                        Dim lstCalcTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                        lstCalcTest = (From a In calculatedTestList.SelectedTestTable _
                                      Where a.TestID = currentTestID _
                                    AndAlso a.SampleType = currentSampleType _
                                     Select a).ToList

                        If (lstCalcTest.Count = 1) Then
                            'Change the OTStatus to indicate the Test is locked
                            lstCalcTest(0).OTStatus = "STATLOCK"

                            'Change the BackColor/ForeColor of the grid cell containing the Test to indicate that it cannot be selected/unselected
                            bsCalcTestDataGridView.Rows(lstCalcTest(0).Row).Cells(lstCalcTest(0).Col).Style.BackColor = LOCKED_BACK_COLOR
                            bsCalcTestDataGridView.Rows(lstCalcTest(0).Row).Cells(lstCalcTest(0).Col).Style.SelectionBackColor = LOCKED_BACK_COLOR
                            bsCalcTestDataGridView.Rows(lstCalcTest(0).Row).Cells(lstCalcTest(0).Col).Style.ForeColor = LOCKED_FORE_COLOR
                            bsCalcTestDataGridView.Rows(lstCalcTest(0).Row).Cells(lstCalcTest(0).Col).Style.SelectionForeColor = LOCKED_FORE_COLOR

                            'If the locked Test is included in a Test Profile, check it in the TreeView
                            If (Not lockedTest.IsTestProfileIDNull) Then
                                If (bsProfilesTreeView.Nodes.Find(lockedTest.TestProfileID.ToString(), False).Length <> 0) Then
                                    Dim myTreeNode As New TreeNode
                                    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(lockedTest.TestProfileID.ToString(), True).First, TreeNode)
                                    myTreeNode.Checked = True
                                    myTreeNode.ForeColor = Color.CornflowerBlue

                                    For Each myNode As TreeNode In myTreeNode.Nodes
                                        myNode.Checked = True
                                        myNode.ForeColor = Color.CornflowerBlue
                                    Next
                                End If
                            End If

                            'Lock all Test Profiles that include the locked Test
                            LockTestProfile(lstCalcTest(0).TestKey)
                        End If

                    ElseIf (currentTestType = "ISE") Then
                        'Process for locked ISE Tests...
                        Dim lstISETest As List(Of SelectedTestsDS.SelectedTestTableRow)
                        lstISETest = (From a In iseTestList.SelectedTestTable _
                                     Where a.TestID = currentTestID _
                                   AndAlso a.SampleType = currentSampleType _
                                    Select a).ToList

                        If (lstISETest.Count = 1) Then
                            'Change the OTStatus to indicate the Test is locked
                            lstISETest(0).OTStatus = "STATLOCK"

                            'Change the BackColor/ForeColor of the grid cell containing the Test to indicate that it cannot be selected/unselected
                            bsISETestDataGridView.Rows(lstISETest(0).Row).Cells(lstISETest(0).Col).Style.BackColor = LOCKED_BACK_COLOR
                            bsISETestDataGridView.Rows(lstISETest(0).Row).Cells(lstISETest(0).Col).Style.SelectionBackColor = LOCKED_BACK_COLOR
                            bsISETestDataGridView.Rows(lstISETest(0).Row).Cells(lstISETest(0).Col).Style.ForeColor = LOCKED_FORE_COLOR
                            bsISETestDataGridView.Rows(lstISETest(0).Row).Cells(lstISETest(0).Col).Style.SelectionForeColor = LOCKED_FORE_COLOR

                            'If the locked Test is included in a Test Profile, check it in the TreeView
                            If (Not lockedTest.IsTestProfileIDNull) Then
                                If (bsProfilesTreeView.Nodes.Find(lockedTest.TestProfileID.ToString(), False).Length <> 0) Then
                                    Dim myTreeNode As New TreeNode
                                    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(lockedTest.TestProfileID.ToString(), True).First, TreeNode)
                                    myTreeNode.Checked = True
                                    myTreeNode.ForeColor = Color.CornflowerBlue

                                    For Each myNode As TreeNode In myTreeNode.Nodes
                                        myNode.Checked = True
                                        myNode.ForeColor = Color.CornflowerBlue
                                    Next
                                End If
                            End If

                            'Lock all Test Profiles that include the locked Test
                            LockTestProfile(lstISETest(0).TestKey)
                        End If

                    ElseIf (currentTestType = "OFFS") Then
                        'Process for locked Off-System Tests...
                        'Dim lstOffSystemTest As New List(Of SelectedTestsDS.SelectedTestTableRow)
                        Dim lstOffSystemTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                        lstOffSystemTest = (From a In offSystemTestList.SelectedTestTable _
                                            Where a.TestID = currentTestID _
                                            AndAlso a.SampleType = currentSampleType _
                                            Select a).ToList

                        If (lstOffSystemTest.Count = 1) Then
                            'Change the OTStatus to indicate the Test is locked
                            lstOffSystemTest(0).OTStatus = "STATLOCK"

                            'Change the BackColor/ForeColor of the grid cell containing the Test to indicate that it cannot be selected/unselected
                            bsOffSystemTestDataGridView.Rows(lstOffSystemTest(0).Row).Cells(lstOffSystemTest(0).Col).Style.BackColor = LOCKED_BACK_COLOR
                            bsOffSystemTestDataGridView.Rows(lstOffSystemTest(0).Row).Cells(lstOffSystemTest(0).Col).Style.SelectionBackColor = LOCKED_BACK_COLOR
                            bsOffSystemTestDataGridView.Rows(lstOffSystemTest(0).Row).Cells(lstOffSystemTest(0).Col).Style.ForeColor = LOCKED_FORE_COLOR
                            bsOffSystemTestDataGridView.Rows(lstOffSystemTest(0).Row).Cells(lstOffSystemTest(0).Col).Style.SelectionForeColor = LOCKED_FORE_COLOR

                            'If the locked Test is included in a Test Profile, check it in the TreeView
                            If (Not lockedTest.IsTestProfileIDNull) Then
                                If (bsProfilesTreeView.Nodes.Find(lockedTest.TestProfileID.ToString(), False).Length <> 0) Then
                                    Dim myTreeNode As New TreeNode
                                    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(lockedTest.TestProfileID.ToString(), True).First, TreeNode)
                                    myTreeNode.Checked = True
                                    myTreeNode.ForeColor = Color.CornflowerBlue

                                    For Each myNode As TreeNode In myTreeNode.Nodes
                                        myNode.Checked = True
                                        myNode.ForeColor = Color.CornflowerBlue
                                    Next
                                End If
                            End If

                            'Lock all Test Profiles that include the locked Test
                            LockTestProfile(lstOffSystemTest(0).TestKey)
                        End If
                    End If

                    'Lock also all Calculated Tests including the locked Test
                    Dim lstCalculates As List(Of Integer)
                    lstCalculates = (From b In calcTestComponents.tparFormulas _
                                    Where b.TestType = currentTestType _
                                  AndAlso b.Value = currentTestID.ToString _
                                  AndAlso b.SampleType = currentSampleType _
                                   Select b.CalcTestID).ToList

                    For Each calcTestID As Integer In lstCalculates
                        'Search position of the Calculated Test in the list 
                        Dim lstCalcTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                        lstCalcTest = (From a In calculatedTestList.SelectedTestTable _
                                      Where a.TestID = calcTestID _
                                     Select a).ToList

                        If (lstCalcTest.Count = 1) Then
                            'Change the OTStatus to indicate the Test is locked
                            lstCalcTest(0).OTStatus = "STATLOCK"

                            'Change the BackColor/ForeColor of the grid cell containing the Test to indicate that it cannot be selected/unselected
                            bsCalcTestDataGridView.Rows(lstCalcTest(0).Row).Cells(lstCalcTest(0).Col).Style.BackColor = LOCKED_BACK_COLOR
                            bsCalcTestDataGridView.Rows(lstCalcTest(0).Row).Cells(lstCalcTest(0).Col).Style.SelectionBackColor = LOCKED_BACK_COLOR
                            bsCalcTestDataGridView.Rows(lstCalcTest(0).Row).Cells(lstCalcTest(0).Col).Style.ForeColor = LOCKED_FORE_COLOR
                            bsCalcTestDataGridView.Rows(lstCalcTest(0).Row).Cells(lstCalcTest(0).Col).Style.SelectionForeColor = LOCKED_FORE_COLOR
                        End If
                    Next
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".VerifyTestsLocking", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".VerifyTestsLocking", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When a Test is locked, all Test Profiles in which it is included are also locked
    ''' </summary>
    ''' <param name="pTestKey">TestType|TestID of the locked Tests</param>
    ''' <param name="pTestProfileID">Optional parameter. When informed, it means that all Test Profiles in which
    '''                              the informed Test is included have to be locked, excepting the identified 
    '''                              with this ID</param>
    ''' <remarks>
    ''' Created by:  SA 10/06/2010
    ''' Modified by: SA 02/11/2010 - Changed parameter pTestID by pTestKey to allow search Tests of whatever 
    '''                              type in the TreeView of Test Profiles
    ''' </remarks>
    Private Sub LockTestProfile(ByVal pTestKey As String, Optional ByVal pTestProfileID As Integer = -1)
        Try
            For Each myTreeNode As TreeNode In bsProfilesTreeView.Nodes
                Dim verifyLocking As Boolean = True
                If (pTestProfileID <> -1) Then
                    If (myTreeNode.Name <> pTestProfileID.ToString) Then
                        verifyLocking = (myTreeNode.Name <> pTestProfileID.ToString)
                    End If
                End If

                If (verifyLocking AndAlso myTreeNode.ForeColor <> Color.Blue) Then
                    'Search if the locked Test is included in the TestProfile
                    If (myTreeNode.Nodes.Find(pTestKey.ToString, False).Length <> 0) Then
                        'Lock all the TestProfile
                        For Each myChildNode As TreeNode In myTreeNode.Nodes
                            myChildNode.ForeColor = Color.CornflowerBlue
                        Next
                        myTreeNode.ForeColor = Color.CornflowerBlue
                    End If
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LockTestProfile", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LockTestProfile", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Search a Node in Node Collection of the TreeView
    ''' </summary>
    ''' <param name="pNodeID">Node Identifier</param>
    ''' <param name="pTreeNodeList">Node collection to search the node</param>
    ''' <returns>Return the Node with the resul values if found. If not return an empty node.
    ''' </returns>
    ''' <remarks>
    ''' Created by:  TR 10/02/2010 
    ''' </remarks>
    Private Function SearchNode(ByVal pNodeID As String, ByVal pTreeNodeList As TreeNodeCollection) As TreeNode
        Dim myTreeNode As New TreeNode
        Try
            'Go through the node collection
            For Each myNode As TreeNode In pTreeNodeList
                'Validate if the node has children 
                If (myNode.Nodes.Count > 0) Then
                    'Recall the method, implementing recursivity to search on all NodeCollection levels
                    myTreeNode = SearchNode(pNodeID, myNode.Nodes)

                    'If the returned value on the name property is not empty, it means that the node was found on this label
                    If myTreeNode.Name <> "" Then Exit For
                Else
                    'Validate if the Node name is the same as the Node we are looking, and validate that the Node is checked
                    If (myNode.Name = pNodeID AndAlso myNode.Checked) Then
                        'Set the found Node to our result variable
                        myTreeNode = myNode
                        Exit For
                    End If
                End If
            Next myNode
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SearchNode", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SearchNode", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myTreeNode
    End Function

    ''' <summary>
    ''' Fill the Profiles TreeView 
    ''' </summary>
    ''' <param name="pTestsProfileTestDS">Typed DataSet containing the Test Profiles with their Tests</param>
    ''' <remarks>
    ''' Created by:  TR 08/02/2010
    ''' Modified by: SA 03/07/2010 - Error: if one Profile has a Test which ID is the same than the ID of another
    '''                              Profile, the Tests in this Profile appears as child of the Test in the first
    '''                              Profile
    '''              SA 29/10/2010 - Changed the Key of child nodes: use TestKey instead of TestID, where TestKey 
    '''                              is TestType|TestID
    ''' </remarks>
    Private Sub FillTreeView(ByVal pTestsProfileTestDS As TestProfileTestsDS)
        Try
            Dim profileID As Integer = 0
            Dim myTreeNode As New TreeNode
            For Each testProfileTestRow As TestProfileTestsDS.tparTestProfileTestsRow In pTestsProfileTestDS.tparTestProfileTests.Rows
                If (testProfileTestRow.TestProfileID <> profileID) Then
                    'New Test Profile, new main node in the Tree
                    profileID = testProfileTestRow.TestProfileID

                    bsProfilesTreeView.Nodes.Add(testProfileTestRow.TestProfileID.ToString(), testProfileTestRow.TestProfileName)
                    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(testProfileTestRow.TestProfileID.ToString(), True).First, TreeNode)
                    myTreeNode.Nodes.Add(testProfileTestRow.TestKey.ToString(), testProfileTestRow.TestName)
                Else
                    'Same Profile, add the Tests as child Nodes
                    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(testProfileTestRow.TestProfileID.ToString(), True).First, TreeNode)
                    myTreeNode.Nodes.Add(testProfileTestRow.TestKey.ToString(), testProfileTestRow.TestName)
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillTreeView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillTreeView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the selection/unselection of Standard Tests 
    ''' </summary>
    ''' <param name="pRowIndex">Test position in the grid of Standard Tests - Row number</param>
    ''' <param name="pColIndex">Test position in the grid of Standard Tests - Column number</param>
    ''' <param name="pCalcTestID">Optional parameter. When informed (its value is greater than zero), it means
    '''                           the Test is included in a Calculated Test that has been unselected. This information
    '''                           is needed because if additionally, the Test is included in a selected Test Profile
    '''                           it has to remain unselected</param>
    ''' <param name="pCalcTestName">Optional parameter. When informed, it contains the name of the Calculated Test that has 
    '''                             been unselected. It is informed always pCalcTestID is informed</param>
    ''' <returns>A boolean value indicating if the Test was selected (True) or unselected (False). If the Test could not
    '''          be selected, returns also False</returns>
    ''' <remarks>
    ''' Created by:  TR 09/02/2010 - Tested: OK
    ''' Modified by: SA 06/04/2010 - Added control for maximum number of allowed Patient Order Tests reached
    '''              SA 04/05/2010 - Added control for unselection of Tests that are included in a Test Profile and it is also
    '''                              included in the Formula of at least a Calculated Tests
    '''              SA 22/07/2010 - Changed from Sub to Function that returns a boolean value indicating if the Test was
    '''                              selected or unselected
    '''              SA 29/10/2010 - Use TestKey instead TestID when search if the Test is included in another Profile
    '''              SA 02/05/2012 - When a STD Test is unselected due to a linked Calculated Test has been unselected (pCalcTestID is 
    '''                              informed), check if the rest of Calculated Tests to which the STD is linked remain selected, removing
    '''                              from fields CalcTestIDs and CalcTestNames those that are currently unselected
    '''              SG 13/03/2013 - Do not allow unselect Tests that have been requested by LIS
    '''              SA 21/05/2014 - BT #1633 ==> When a Test is clicked to unselect it, if it is included in a selected TestProfile, the TestProfile
    '''                                           is unselected, and fields TestProfileID and TestProfileName have to be cleared also for the rest of 
    '''                                           Tests included in the affected Profile (the Tests remain selected, but the Profile data is cleared 
    '''                                           because the Test Profile is not selected anymore). To clear those fields, a new function  
    '''                                           DeleteProfileInformation is called for each Tests included in the Profile (excepting the unselected one)
    ''' </remarks>
    Private Function MarkUnMarkSelectedCell(ByVal pRowIndex As Integer, ByVal pColIndex As Integer, Optional ByVal pCalcTestID As Integer = 0, _
                                            Optional ByVal pCalcTestName As String = "") As Boolean
        Dim selectedTest As Boolean = False
        Try
            Dim myTreeNode As New TreeNode()

            'Search on structure of Selected Tests by Col and Row, but only if the clicked Test is not Locked
            'SGM 13/03/2013
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
            qSelectedTest = (From a In standardTestList.SelectedTestTable _
                            Where a.Row = pRowIndex _
                            AndAlso a.Col = pColIndex _
                            AndAlso a.OTStatus <> "LISLOCK" _
                            AndAlso a.OTStatus <> "STATLOCK" _
                           Select a).ToList()

            If (qSelectedTest.Count = 1) Then
                'Validate if the clicked Test is selected or not to set the next status
                If (Not qSelectedTest.First.Selected) Then
                    'If not selected, then the selected status is set to true                    
                    qSelectedTest.First.Selected = True
                    selectedTest = True

                    'If the Test is selected due to a Calculated Test was selected, link the CalcTest to it (ID and Name)
                    If (pCalcTestID <> 0) Then
                        If (qSelectedTest.First.CalcTestIDs.Trim = "") Then
                            qSelectedTest.First.CalcTestIDs = pCalcTestID.ToString
                            qSelectedTest.First.CalcTestNames = pCalcTestName
                        Else
                            qSelectedTest.First.CalcTestIDs &= ", " & pCalcTestID.ToString
                            qSelectedTest.First.CalcTestNames &= ", " & pCalcTestName
                        End If
                    End If
                Else
                    qSelectedTest.First.Selected = False
                    Dim canBeUnselected As Boolean = False

                    'Test can be unselected only if it has not been sent to the Analyzer
                    If (qSelectedTest.First.OTStatus = "OPEN") Then
                        If (Not qSelectedTest.First().IsTestProfileIDNull AndAlso qSelectedTest.First().TestProfileID > 0) Then
                            'If the Test is linked to a selected Test Profile, the Profile has to be also unselected in the TreeView, 
                            'but this process is executed only when the Test is unselected by clicking in it, not when it is unselected
                            'by unselect the Calculated Test in which Formula the Test is included 
                            If (pCalcTestID = 0) Then
                                'BT #1633 - Local variables needed to call new function DeleteProfileInformation
                                Dim myTestID As Integer = -1
                                Dim myTestType As String = String.Empty
                                Dim testProfileID As Integer = qSelectedTest.First().TestProfileID

                                'Search the Profile on the TreeView to unchecked it
                                If (bsProfilesTreeView.Nodes.Find(testProfileID.ToString, False).Length <> 0) Then
                                    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(testProfileID.ToString, True).First, TreeNode)
                                    myTreeNode.Checked = False

                                    'Unselect also all the Tests in the Profile
                                    For Each myNode As TreeNode In myTreeNode.Nodes
                                        myNode.Checked = myTreeNode.Checked

                                        'BT #1633 - If it not the clicked Test, remove Profile Information from the SelectedTestsDS according 
                                        '           the TestType and TestID
                                        If (myNode.Name.Trim <> qSelectedTest.First().TestKey.Trim) Then
                                            myTestType = myNode.Name.Split(CChar("|"))(0)
                                            myTestID = Convert.ToInt32(myNode.Name.Split(CChar("|"))(1))

                                            DeleteProfileInformation(myTestType, myTestID, testProfileID)
                                        End If
                                    Next

                                    'Search if the Test is also in another Test Profile 
                                    myTreeNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)
                                    If (myTreeNode.Name <> String.Empty) Then
                                        If (myTreeNode.Parent Is Nothing) Then
                                            qSelectedTest.First.TestProfileID = CType(myTreeNode.Name, Integer)
                                            qSelectedTest.First.TestProfileName = myTreeNode.Text
                                        Else
                                            qSelectedTest.First.TestProfileID = CType(myTreeNode.Parent.Name, Integer)
                                            qSelectedTest.First.TestProfileName = myTreeNode.Parent.Text
                                        End If
                                    Else
                                        qSelectedTest.First.TestProfileID = 0
                                        qSelectedTest.First.TestProfileName = ""

                                        canBeUnselected = True
                                    End If
                                End If

                                'If (bsProfilesTreeView.Nodes.Find(qSelectedTest.First().TestProfileID.ToString(), False).Length <> 0) Then
                                '    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(qSelectedTest.First().TestProfileID.ToString(), True).First, TreeNode)
                                '    myTreeNode.Checked = False

                                '    'Unselect also all the Tests in the Profile
                                '    For Each myNode As TreeNode In myTreeNode.Nodes
                                '        myNode.Checked = myTreeNode.Checked
                                '    Next

                                '    'Search if the Test is also in another Test Profile 
                                '    myTreeNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)
                                '    If (Not myTreeNode.Name = "") Then
                                '        If (myTreeNode.Parent Is Nothing) Then
                                '            qSelectedTest.First.TestProfileID = CType(myTreeNode.Name, Integer)
                                '            qSelectedTest.First.TestProfileName = myTreeNode.Text
                                '        Else
                                '            qSelectedTest.First.TestProfileID = CType(myTreeNode.Parent.Name, Integer)
                                '            qSelectedTest.First.TestProfileName = myTreeNode.Parent.Text
                                '        End If
                                '    Else
                                '        qSelectedTest.First.TestProfileID = 0
                                '        qSelectedTest.First.TestProfileName = ""

                                '        canBeUnselected = True
                                '    End If
                                'End If
                            Else
                                'Nothing to do, the Test remains selected because is linked to a selected Test Profile...
                                'Search the Calculated Test ID in field CalcTestIDs to remove it
                                'Search the Calculated Test Name in field CalcTestNames to remove it
                                qSelectedTest.First.CalcTestIDs = RebuildStringList(qSelectedTest.First.CalcTestIDs, pCalcTestID.ToString)
                                qSelectedTest.First.CalcTestNames = RebuildStringList(qSelectedTest.First.CalcTestNames, pCalcTestName)
                            End If
                        Else
                            canBeUnselected = True
                        End If

                        If (canBeUnselected) Then
                            If (pCalcTestID <> 0) Then
                                'Test is unselected due to a linked Calculated Test has been unselected
                                'Search the Calculated Test ID in field CalcTestIDs to remove it
                                'Search the Calculated Test Name in field CalcTestNames to remove it
                                qSelectedTest.First.CalcTestIDs = RebuildStringList(qSelectedTest.First.CalcTestIDs, pCalcTestID.ToString)
                                qSelectedTest.First.CalcTestNames = RebuildStringList(qSelectedTest.First.CalcTestNames, pCalcTestName)

                                'If the STD Test is linked to other Calculated Tests, verify if they remain selected
                                If (qSelectedTest.First.CalcTestIDs.Trim <> "") Then
                                    Dim calcTests() As String = qSelectedTest.First.CalcTestIDs.Trim.Split(CChar(", "))
                                    For i = 0 To calcTests.Count - 1
                                        ' ReSharper disable once InconsistentNaming
                                        Dim aux_i = i
                                        'Get position of the Calculated Test in the correspondent array
                                        Dim lstCalPos As List(Of SelectedTestsDS.SelectedTestTableRow)
                                        lstCalPos = (From a In calculatedTestList.SelectedTestTable _
                                                     Where a.TestID = Convert.ToInt32(calcTests(aux_i).Trim) _
                                                     Select a).ToList()
                                        If (lstCalPos.Count = 1) Then
                                            'If the Calculated Test is currently unselected, remove it from fields CalcTestIDs and CalcTestNames
                                            If (Not lstCalPos.First.Selected) Then
                                                qSelectedTest.First.CalcTestIDs = RebuildStringList(qSelectedTest.First.CalcTestIDs, lstCalPos.First.TestID.ToString)
                                                qSelectedTest.First.CalcTestNames = RebuildStringList(qSelectedTest.First.CalcTestNames, lstCalPos.First.TestName)
                                            End If
                                        End If
                                    Next i
                                End If

                                'El Test can be unselected when it is not linked to another selected Calculated Test
                                canBeUnselected = (qSelectedTest.First.CalcTestIDs.Trim = "")
                            Else
                                'Test is unselected by clicking in it
                                If (qSelectedTest.First.CalcTestIDs.Trim <> "") Then
                                    'Get all different selected Calculated Tests to which the Test is linked
                                    Dim calcTests() As String = qSelectedTest.First.CalcTestIDs.Trim.Split(CChar(", "))
                                    For i = 0 To calcTests.Count - 1
                                        Dim aux_i = i
                                        'Get position of the Calculated Test in the correspondent array
                                        Dim lstCalPos As List(Of SelectedTestsDS.SelectedTestTableRow)
                                        lstCalPos = (From a In calculatedTestList.SelectedTestTable _
                                                     Where a.TestID = Convert.ToInt32(calcTests(aux_i).Trim) _
                                                     Select a).ToList()
                                        If (lstCalPos.Count = 1) Then
                                            'Only if the Calculated Test is currently selected, unselect it but without unselect its components
                                            If (lstCalPos.First.Selected) Then
                                                MarkUnMarkCalculatedTestCell(lstCalPos.First.Row, lstCalPos.First.Col, False)
                                            End If
                                        End If
                                    Next i
                                    qSelectedTest.First.CalcTestIDs = ""
                                    qSelectedTest.First.CalcTestNames = ""
                                End If
                            End If
                        End If

                        If (canBeUnselected) Then
                            'Finally, the Test is unselected
                            qSelectedTest.First.Selected = False
                            selectedTest = False
                        Else
                            'The Test recovers its previous status
                            qSelectedTest.First.Selected = True
                            selectedTest = True
                        End If
                    Else
                        'Only for Controls... if not all Controls needed for Test are included in the WorkSession
                        If (qSelectedTest.First.PartiallySelected) Then
                            qSelectedTest.First.Selected = False
                            selectedTest = False
                        End If

                    End If
                End If

                'Set the color for Selected and Not Selected cells
                Dim bFind As Boolean = False
                Dim maxNumberValidation As Boolean = False
                If (qSelectedTest.First.OTStatus = "OPEN") Then
                    If (qSelectedTest.First().Selected) Then
                        If (bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.IndianRed) Then numSTDRed -= 1

                        bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.LightSlateGray
                        bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.LightSlateGray
                        bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.White
                        bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.White

                        maxNumberValidation = (Not maxPatientOTsReached)
                    Else
                        bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.White
                        bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.White

                        If (Not qSelectedTest.First.PartiallySelected) Then
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Black
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Black
                        Else
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Red
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Red
                        End If

                        'If the warning of maximum number of Tests have been shown, when a Test is unselected the verification is executed again
                        If (maxPatientOTsReached) Then
                            maxNumberValidation = True
                            maxPatientOTsReached = False
                        End If

                        For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In selectTestList.SelectedTestTable.Rows
                            If (qSelectedTest.First.TestType = selectedRow.TestType AndAlso qSelectedTest.First.TestID = selectedRow.TestID) Then
                                bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.IndianRed
                                bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.IndianRed
                                bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Black
                                bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Black

                                numSTDRed += 1
                                bFind = True
                                Exit For
                            End If
                        Next selectedRow
                    End If
                Else
                    'Only for Controls... if the Control is included in a WorkSession but only partially
                    If (qSelectedTest.First.PartiallySelected) Then
                        'Allow selecting the Test
                        If (qSelectedTest.First().Selected) Then
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.LightSlateGray
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.LightSlateGray
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.White
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.White

                        Else
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.White
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.White
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Red
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Red
                        End If
                    End If
                End If

                'Verify if the maximum number of allowed selected Tests has been reached
                If (maxNumberValidation) Then ControlMsgMaxNumberReached()

                'Verify if a Test previously selected have been unselected
                If (Not maxPatientOTsReached) Then ControlMSGDeleteTests(bFind)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkSelectedCell", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkSelectedCell", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return selectedTest
    End Function

    ''' <summary>
    ''' Manages selection/unselection of Calculated Tests
    ''' </summary>
    ''' <param name="pRowIndex">Test position in the grid of Calculated Tests - Row number</param>
    ''' <param name="pColIndex">Test position in the grid of Calculated Tests - Column number</param>
    ''' <param name="pVerifyComponents">Optional parameters to indicate if, when the Calculated Test is unselected,
    '''                                 all the Tests included in its Formula have to be also unchecked</param>
    ''' <returns>A boolean value indicating if the Test was selected (True) or unselected (False). If the Test could not
    '''          be selected, returns also False</returns>
    ''' <remarks>
    ''' Created by:  SA 04/05/2010
    ''' Modified by: SA 22/07/2010 - Changed from Sub to Function that returns a boolean value indicating if the Test was
    '''                              selected or unselected
    '''              SA 29/10/2010 - When a Calculated Test included in a selected Profile is unselected, the Profile has to
    '''                              be also unselected; use TestKey instead TestID when search if the Test is included in 
    '''                              another Profile 
    '''              TR 10/03/2011 - Added changes to validate if the selected Calculated Tests are composed for Standard
    '''                              Tests that are still using Factory Calibration values
    '''              RH 23/05/2012 - Use StringBuilder for massive String concatenations; message for Factory Calibration 
    '''                              change is shown in an auxiliary screen instead of a MsgBox
    '''              RH 24/05/2012 - New parameter pTestToValidate
    '''              SG 13/03/2013 - Do not allow unselect Tests that have been requested by LIS
    '''              SA 21/05/2014 - BT #1633 ==> When a Test is clicked to unselect it, if it is included in a selected TestProfile, the TestProfile
    '''                                           is unselected, and fields TestProfileID and TestProfileName have to be cleared also for the rest of 
    '''                                           Tests included in the affected Profile (the Tests remain selected, but the Profile data is cleared 
    '''                                           because the Test Profile is not selected anymore). To clear those fields, a new function  
    '''                                           DeleteProfileInformation is called for each Tests included in the Profile (excepting the unselected one)
    '''              XB 02/12/2014 - Add functionality cases for ISE and OFFS tests included into a CALC test - BA-1867
    ''' </remarks>
    Private Function MarkUnMarkCalculatedTestCell(ByVal pRowIndex As Integer, ByVal pColIndex As Integer, _
                                                  Optional ByVal pVerifyComponents As Boolean = True, _
                                                  Optional ByVal pTestToValidate As List(Of SelectedTestsDS.SelectedTestTableRow) = Nothing) As Boolean
        Dim selectedTest As Boolean = False
        Try
            Dim myTreeNode As New TreeNode()

            'To store the list of STD Tests also selected (due to they are included in the Formula
            'of selected Calculated Tests) to validate the use of Factory Calibration values
            Dim myTestToValidate As List(Of SelectedTestsDS.SelectedTestTableRow)
            If (pTestToValidate Is Nothing) Then
                myTestToValidate = New List(Of SelectedTestsDS.SelectedTestTableRow)
            Else
                myTestToValidate = pTestToValidate
            End If

            'Search on structure of Selected Tests by Col and Row, but only if the clicked Test is not Locked
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
            qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                            Where a.Row = pRowIndex _
                          AndAlso a.Col = pColIndex _
                          AndAlso a.OTStatus <> "LISLOCK" _
                          AndAlso a.OTStatus <> "STATLOCK" _
                           Select a).ToList()

            If (qSelectedTest.Count = 1) Then
                'Validate if it's selected or not to set the proper status
                Dim selectionStatus As Boolean = (Not qSelectedTest.First.Selected)
                Dim continueProcess As Boolean = False

                Dim myCurrentID As Integer = qSelectedTest.First.TestID
                Dim myCurrentName As String = qSelectedTest.First.TestName

                'Validate if the clicked Test is selected or not to set the next status
                If (Not qSelectedTest.First.Selected) Then
                    'If not selected, then the selected status is set to true                    
                    qSelectedTest.First.Selected = True
                    selectedTest = True

                    qSelectedTest.First.CalcTestIDs = ""
                    qSelectedTest.First.CalcTestNames = ""

                    continueProcess = True
                Else
                    'Test can be unselected only if it has not been sent to the Analyzer
                    If (qSelectedTest.First.OTStatus = "OPEN") Then
                        If (Not qSelectedTest.First().IsTestProfileIDNull AndAlso qSelectedTest.First().TestProfileID > 0) Then
                            'BT #1633 - Local variables needed to call new function DeleteProfileInformation
                            Dim myTestID As Integer = -1
                            Dim myTestType As String = String.Empty
                            Dim testProfileID As Integer = qSelectedTest.First().TestProfileID

                            'If the Test is linked to a selected Test Profile, the Profile has to be also unselected in the TreeView
                            'Search the Profile on the TreeView to unchecked it
                            If (bsProfilesTreeView.Nodes.Find(testProfileID.ToString, False).Length <> 0) Then
                                myTreeNode = CType(bsProfilesTreeView.Nodes.Find(testProfileID.ToString(), True).First, TreeNode)
                                myTreeNode.Checked = False

                                'Unselect also all the Tests in the Profile
                                For Each myNode As TreeNode In myTreeNode.Nodes
                                    myNode.Checked = myTreeNode.Checked

                                    'BT #1633 - If it not the clicked Test, remove Profile Information from the SelectedTestsDS according 
                                    '           the TestType and TestID
                                    If (myNode.Name.Trim <> qSelectedTest.First().TestKey.Trim) Then
                                        myTestType = myNode.Name.Split(CChar("|"))(0)
                                        myTestID = Convert.ToInt32(myNode.Name.Split(CChar("|"))(1))

                                        DeleteProfileInformation(myTestType, myTestID, testProfileID)
                                    End If
                                Next

                                'Search if the Test is also in another Test Profile 
                                'myTreeNode = SearchNode(qSelectedTest.First.TestID.ToString(), bsProfilesTreeView.Nodes)
                                myTreeNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)
                                If (myTreeNode.Name <> String.Empty) Then
                                    If (myTreeNode.Parent Is Nothing) Then
                                        qSelectedTest.First.TestProfileID = CType(myTreeNode.Name, Integer)
                                        qSelectedTest.First.TestProfileName = myTreeNode.Text
                                    Else
                                        qSelectedTest.First.TestProfileID = CType(myTreeNode.Parent.Name, Integer)
                                        qSelectedTest.First.TestProfileName = myTreeNode.Parent.Text
                                    End If
                                Else
                                    qSelectedTest.First.TestProfileID = 0
                                    qSelectedTest.First.TestProfileName = ""

                                    continueProcess = True
                                End If
                            End If

                            'If (bsProfilesTreeView.Nodes.Find(qSelectedTest.First().TestProfileID.ToString(), False).Length <> 0) Then
                            '    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(qSelectedTest.First().TestProfileID.ToString(), True).First, TreeNode)
                            '    myTreeNode.Checked = False

                            '    'Unselect also all the Tests in the Profile
                            '    For Each myNode As TreeNode In myTreeNode.Nodes
                            '        myNode.Checked = myTreeNode.Checked
                            '    Next

                            '    'Search if the Test is also in another Test Profile 
                            '    'myTreeNode = SearchNode(qSelectedTest.First.TestID.ToString(), bsProfilesTreeView.Nodes)
                            '    myTreeNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)
                            '    If (Not myTreeNode.Name = "") Then
                            '        If (myTreeNode.Parent Is Nothing) Then
                            '            qSelectedTest.First.TestProfileID = CType(myTreeNode.Name, Integer)
                            '            qSelectedTest.First.TestProfileName = myTreeNode.Text
                            '        Else
                            '            qSelectedTest.First.TestProfileID = CType(myTreeNode.Parent.Name, Integer)
                            '            qSelectedTest.First.TestProfileName = myTreeNode.Parent.Text
                            '        End If
                            '    Else
                            '        qSelectedTest.First.TestProfileID = 0
                            '        qSelectedTest.First.TestProfileName = ""

                            '        continueProcess = True
                            '    End If
                            'End If
                        Else
                            continueProcess = True
                        End If

                        If (continueProcess) Then
                            'The Calculated Test is unselected
                            qSelectedTest.First.Selected = False
                            selectedTest = False

                            'Verify if the unselected Calculated Test is included in the formula of another selected Calculated Test
                            If (qSelectedTest.First.CalcTestIDs <> "") Then
                                Dim lstSelected As List(Of SelectedTestsDS.SelectedTestTableRow)
                                lstSelected = (From a In calculatedTestList.SelectedTestTable _
                                               Join b In calcTestComponents.tparFormulas _
                                                 On a.TestID Equals b.CalcTestID _
                                              Where b.TestType = "CALC" _
                                            AndAlso b.SampleType = SampleTypeAttribute _
                                            AndAlso b.Value = myCurrentID.ToString _
                                            AndAlso a.Selected = True _
                                             Select a).ToList

                                For Each calcTest As SelectedTestsDS.SelectedTestTableRow In lstSelected
                                    'Unmark also all Calculated Tests in which formula is included
                                    'MarkUnMarkCalculatedTestCell(calcTest.Row, calcTest.Col)

                                    'RH 24/05/2012
                                    MarkUnMarkCalculatedTestCell(calcTest.Row, calcTest.Col, pVerifyComponents, myTestToValidate)
                                Next

                                qSelectedTest.First.CalcTestIDs = ""
                                qSelectedTest.First.CalcTestNames = ""
                            End If
                        End If
                    End If
                End If

                If (continueProcess) Then
                    'Get the list of Tests included in the Formula defined for the Calculated Test having the same SampleType
                    Dim lstFormulaTests As List(Of FormulasDS.tparFormulasRow)
                    lstFormulaTests = (From a In calcTestComponents.tparFormulas _
                                      Where a.CalcTestID = myCurrentID _
                                    AndAlso a.SampleType = SampleTypeAttribute _
                                   Order By a.TestType _
                                     Select a).ToList()

                    For Each testInFormula As FormulasDS.tparFormulasRow In lstFormulaTests
                        If (testInFormula.TestType = "STD") Then
                            'Search position and current selection status of the Test in the list of Standard Tests
                            Dim lstStandardTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                            lstStandardTest = (From b In standardTestList.SelectedTestTable _
                                              Where b.TestID = Convert.ToInt32(testInFormula.Value) _
                                             Select b).ToList

                            If (lstStandardTest.Count = 1) Then
                                If (pVerifyComponents) Then
                                    'Only if the selection Status is different from the one that has been set for the Calculated Test...
                                    If (lstStandardTest.First.Selected <> selectionStatus) Then
                                        If (lstStandardTest.First.OTStatus = "OPEN") Then
                                            MarkUnMarkSelectedCell(lstStandardTest.First.Row, lstStandardTest.First.Col, myCurrentID, myCurrentName)
                                        Else
                                            'The Standard Test is PENDING... if the Calculated Test was unselected, then remove it 
                                            'from the list of Calculated Test in which the Standard Tests is included
                                            If (Not selectionStatus) Then
                                                lstStandardTest.First.CalcTestIDs = RebuildStringList(lstStandardTest.First.CalcTestIDs, myCurrentID.ToString)
                                                lstStandardTest.First.CalcTestNames = RebuildStringList(lstStandardTest.First.CalcTestNames, myCurrentName.ToString)
                                            End If
                                        End If

                                        If (selectionStatus) Then
                                            'TR 10/03/2011 - Use of Factory Calibration values has to be validated for the selected STD Test
                                            myTestToValidate.Add(lstStandardTest.First())
                                        End If
                                    Else
                                        'Link the Standard Test to the Calculated informing fields CalcTestIDs and CalcTestNames (when selecting)
                                        If (selectionStatus) Then
                                            If (lstStandardTest.First.CalcTestIDs.Trim = "") Then
                                                lstStandardTest.First.CalcTestIDs = myCurrentID.ToString
                                                lstStandardTest.First.CalcTestNames = myCurrentName.ToString
                                            Else
                                                lstStandardTest.First.CalcTestIDs &= ", " & myCurrentID.ToString
                                                lstStandardTest.First.CalcTestNames &= ", " & myCurrentName.ToString
                                            End If
                                        End If
                                    End If
                                Else
                                    'The component Test remains selected, but the link with the Calculated Test is removed
                                    lstStandardTest.First.CalcTestIDs = RebuildStringList(lstStandardTest.First.CalcTestIDs, myCurrentID.ToString)
                                    lstStandardTest.First.CalcTestNames = RebuildStringList(lstStandardTest.First.CalcTestNames, myCurrentName.ToString)
                                End If
                            End If
                        ElseIf (testInFormula.TestType = "CALC") Then
                            'Search position and current selection status of the Test in the list of Calculated Tests
                            Dim lstCalculatedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                            lstCalculatedTest = (From b In calculatedTestList.SelectedTestTable _
                                                Where b.TestID = Convert.ToInt32(testInFormula.Value) _
                                                Select b).ToList

                            If (lstCalculatedTest.Count = 1) Then
                                If (pVerifyComponents) Then
                                    'Only if the selection Status is different from the one that has been set for the Calculated Test...
                                    If (lstCalculatedTest.First.Selected <> selectionStatus) Then
                                        If (lstCalculatedTest.First.OTStatus = "OPEN") Then
                                            'MarkUnMarkCalculatedTestCell(lstCalculatedTest.First.Row, lstCalculatedTest.First.Col)

                                            'RH 24/05/2012
                                            MarkUnMarkCalculatedTestCell(lstCalculatedTest.First.Row, lstCalculatedTest.First.Col, pVerifyComponents, myTestToValidate)
                                        Else
                                            'The Standard Test is PENDING... if the Calculated Test was unselected, then remove it 
                                            'from the list of Calculated Test in which the Standard Tests is included
                                            If (Not selectionStatus) Then
                                                lstCalculatedTest.First.CalcTestIDs = RebuildStringList(lstCalculatedTest.First.CalcTestIDs, myCurrentID.ToString)
                                                lstCalculatedTest.First.CalcTestNames = RebuildStringList(lstCalculatedTest.First.CalcTestNames, myCurrentName.ToString)
                                            End If
                                        End If
                                    End If

                                    'Link the Calculated Test to the Calculated selected informing fields CalcTestIDs and CalcTestNames (when selecting)
                                    If (selectionStatus) Then
                                        If (lstCalculatedTest.First.CalcTestIDs.Trim = "") Then
                                            lstCalculatedTest.First.CalcTestIDs = myCurrentID.ToString
                                            lstCalculatedTest.First.CalcTestNames = myCurrentName.ToString
                                        Else
                                            lstCalculatedTest.First.CalcTestIDs &= ", " & myCurrentID.ToString
                                            lstCalculatedTest.First.CalcTestNames &= ", " & myCurrentName.ToString
                                        End If
                                    End If
                                Else
                                    'The component Test remains selected, but the link with the Calculated Test is removed
                                    lstCalculatedTest.First.CalcTestIDs = RebuildStringList(lstCalculatedTest.First.CalcTestIDs, myCurrentID.ToString)
                                    lstCalculatedTest.First.CalcTestNames = RebuildStringList(lstCalculatedTest.First.CalcTestNames, myCurrentName.ToString)
                                End If
                            End If

                            ' XB 02/12/2014 - BA-1867
                        ElseIf (testInFormula.TestType = "ISE") Then
                            'Search position and current selection status of the Test in the list of ISE Tests
                            Dim lstISETest As List(Of SelectedTestsDS.SelectedTestTableRow)
                            lstISETest = (From b In iseTestList.SelectedTestTable _
                                         Where b.TestID = Convert.ToInt32(testInFormula.Value) _
                                        Select b).ToList

                            If (lstISETest.Count = 1) Then
                                If (pVerifyComponents) Then
                                    'Only if the selection Status is different from the one that has been set for the Calculated Test...
                                    If (lstISETest.First.Selected <> selectionStatus) Then
                                        If (lstISETest.First.OTStatus = "OPEN") Then
                                            MarkUnMarkISETestCell(lstISETest.First.Row, lstISETest.First.Col, myCurrentID, myCurrentName)
                                        Else
                                            'The ISE Test is PENDING... if the Calculated Test was unselected, then remove it 
                                            'from the list of Calculated Test in which the ISE Tests is included
                                            If (Not selectionStatus) Then
                                                lstISETest.First.CalcTestIDs = RebuildStringList(lstISETest.First.CalcTestIDs, myCurrentID.ToString)
                                                lstISETest.First.CalcTestNames = RebuildStringList(lstISETest.First.CalcTestNames, myCurrentName.ToString)
                                            End If
                                        End If
                                    Else
                                        'Link the ISE Test to the Calculated informing fields CalcTestIDs and CalcTestNames (when selecting)
                                        If (selectionStatus) Then
                                            If (lstISETest.First.CalcTestIDs.Trim = "") Then
                                                lstISETest.First.CalcTestIDs = myCurrentID.ToString
                                                lstISETest.First.CalcTestNames = myCurrentName.ToString
                                            Else
                                                lstISETest.First.CalcTestIDs &= ", " & myCurrentID.ToString
                                                lstISETest.First.CalcTestNames &= ", " & myCurrentName.ToString
                                            End If
                                        End If
                                    End If
                                Else
                                    'The component Test remains selected, but the link with the Calculated Test is removed
                                    lstISETest.First.CalcTestIDs = RebuildStringList(lstISETest.First.CalcTestIDs, myCurrentID.ToString)
                                    lstISETest.First.CalcTestNames = RebuildStringList(lstISETest.First.CalcTestNames, myCurrentName.ToString)
                                End If
                            End If

                        ElseIf (testInFormula.TestType = "OFFS") Then
                            'Search position and current selection status of the Test in the list of OFFS Tests
                            Dim lstOFFSTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                            lstOFFSTest = (From b In offSystemTestList.SelectedTestTable _
                                           Where b.TestID = Convert.ToInt32(testInFormula.Value) _
                                           Select b).ToList

                            If (lstOFFSTest.Count = 1) Then
                                If (pVerifyComponents) Then
                                    'Only if the selection Status is different from the one that has been set for the Calculated Test...
                                    If (lstOFFSTest.First.Selected <> selectionStatus) Then
                                        If (lstOFFSTest.First.OTStatus = "OPEN") Then
                                            MarkUnMarkOffSystemTestCell(lstOFFSTest.First.Row, lstOFFSTest.First.Col, myCurrentID, myCurrentName)
                                        Else
                                            'The OFFS Test is PENDING... if the Calculated Test was unselected, then remove it 
                                            'from the list of Calculated Test in which the ISE Tests is included
                                            If (Not selectionStatus) Then
                                                lstOFFSTest.First.CalcTestIDs = RebuildStringList(lstOFFSTest.First.CalcTestIDs, myCurrentID.ToString)
                                                lstOFFSTest.First.CalcTestNames = RebuildStringList(lstOFFSTest.First.CalcTestNames, myCurrentName.ToString)
                                            End If
                                        End If
                                    Else
                                        'Link the OFFS Test to the Calculated informing fields CalcTestIDs and CalcTestNames (when selecting)
                                        If (selectionStatus) Then
                                            If (lstOFFSTest.First.CalcTestIDs.Trim = "") Then
                                                lstOFFSTest.First.CalcTestIDs = myCurrentID.ToString
                                                lstOFFSTest.First.CalcTestNames = myCurrentName.ToString
                                            Else
                                                lstOFFSTest.First.CalcTestIDs &= ", " & myCurrentID.ToString
                                                lstOFFSTest.First.CalcTestNames &= ", " & myCurrentName.ToString
                                            End If
                                        End If
                                    End If
                                Else
                                    'The component Test remains selected, but the link with the Calculated Test is removed
                                    lstOFFSTest.First.CalcTestIDs = RebuildStringList(lstOFFSTest.First.CalcTestIDs, myCurrentID.ToString)
                                    lstOFFSTest.First.CalcTestNames = RebuildStringList(lstOFFSTest.First.CalcTestNames, myCurrentName.ToString)
                                End If
                            End If
                            ' XB 02/12/2014 - BA-1867


                        End If
                    Next

                    'Set the color for Selected and Not Selected cells
                    Dim bFind As Boolean = False
                    Dim maxNumberValidation As Boolean = False
                    If (qSelectedTest.First.OTStatus = "OPEN") Then
                        If (qSelectedTest.First().Selected) Then
                            'If (bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.IndianRed) Then numCALCRed -= 1

                            bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.LightSlateGray
                            bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.LightSlateGray
                            bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.White
                            bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.White

                            maxNumberValidation = (Not maxPatientOTsReached)
                        Else
                            bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.White
                            bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.White
                            bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Black
                            bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Black

                            'If the warning of maximum number of Tests have been shown, when a Test is unselected the verification is executed again
                            If (maxPatientOTsReached) Then
                                maxNumberValidation = True
                                maxPatientOTsReached = False
                            End If

                            For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In selectTestList.SelectedTestTable.Rows
                                If (qSelectedTest.First.TestType = selectedRow.TestType AndAlso qSelectedTest.First.TestID = selectedRow.TestID) Then
                                    bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.IndianRed
                                    bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.IndianRed
                                    bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Black
                                    bsCalcTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Black

                                    'numCALCRed += 1
                                    bFind = True
                                    Exit For
                                End If
                            Next selectedRow
                        End If
                    End If

                    'Verify if the maximum number of allowed selected Tests has been reached
                    If (maxNumberValidation) Then ControlMsgMaxNumberReached()

                    'Verify if a Test previously selected have been unselected
                    If (Not maxPatientOTsReached) Then ControlMSGDeleteTests(bFind)
                End If
            End If

            'Verify if the selected STD Tests are using the Factory Calibration values
            If (pTestToValidate Is Nothing AndAlso myTestToValidate.Count > 0) Then
                Dim testWithFactoryValues As New System.Text.StringBuilder()
                For Each mySelTest As SelectedTestsDS.SelectedTestTableRow In myTestToValidate
                    If (mySelTest.Selected) Then testWithFactoryValues.Append(ValidateFactoryValues(mySelTest.Row, mySelTest.Col))
                Next

                If (testWithFactoryValues.Length > 0) Then
                    Using AuxForm As New IWSTestSelectionWarning()
                        AuxForm.Message = testWithFactoryValues.ToString()
                        AuxForm.ShowDialog()
                    End Using
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkCalculatedTestCell", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkCalculatedTestCell", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return selectedTest
    End Function

    ''' <summary>
    ''' Manages selection/unselection of ISE Tests
    ''' </summary>
    ''' <param name="pRowIndex">Test position in the grid of ISE Tests - Row number</param>
    ''' <param name="pColIndex">Test position in the grid of ISE Tests - Column number</param>
    ''' <returns>A boolean value indicating if the Test was selected (True) or unselected (False). If the Test could not
    '''          be selected, returns also False</returns>
    ''' <remarks>
    ''' Created by:  DL 21/10/2010
    ''' Modified by: SA 29/10/2010 - When an ISE Test included in a selected Profile is unselected, the Profile has to
    '''                              be also unselected; use TestKey instead TestID when search if the Test is included 
    '''                              in another Profile 
    '''              SA 19/06/2012 - Added management of Controls partially selected in the same way it is done for Standard Tests
    '''              SG 13/03/2013 - Do not allow unselect Tests that have been requested by LIS
    '''              SA 21/05/2014 - BT #1633 ==> When a Test is clicked to unselect it, if it is included in a selected TestProfile, the TestProfile
    '''                                           is unselected, and fields TestProfileID and TestProfileName have to be cleared also for the rest of 
    '''                                           Tests included in the affected Profile (the Tests remain selected, but the Profile data is cleared 
    '''                                           because the Test Profile is not selected anymore). To clear those fields, a new function  
    '''                                           DeleteProfileInformation is called for each Tests included in the Profile (excepting the unselected one)
    '''              XB 02/12/2014 - Add functionality cases for ISE and OFFS tests included into a CALC test - BA-1867
    ''' </remarks>
    Private Function MarkUnMarkISETestCell(ByVal pRowIndex As Integer, ByVal pColIndex As Integer, Optional ByVal pCalcTestID As Integer = 0, _
                                           Optional ByVal pCalcTestName As String = "") As Boolean
        Dim selectedTest As Boolean = False
        Try
            Dim myTreeNode As New TreeNode()

            'Search on structure of Selected Tests by Col and Row, but only if the clicked Test is not Locked
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
            qSelectedTest = (From a In iseTestList.SelectedTestTable _
                            Where a.Row = pRowIndex _
                          AndAlso a.Col = pColIndex _
                          AndAlso a.OTStatus <> "LISLOCKED" _
                          AndAlso a.OTStatus <> "STATLOCK" _
                           Select a).ToList()

            If (qSelectedTest.Count = 1) Then
                'Validate if the clicked Test is selected or not to set the next status
                If (Not qSelectedTest.First.Selected) Then
                    'If not selected, then the selected status is set to true                    
                    qSelectedTest.First.Selected = True
                    selectedTest = True

                    ' XB 02/12/2014 - BA-1867
                    'If the Test is selected due to a Calculated Test was selected, link the CalcTest to it (ID and Name)
                    If (pCalcTestID <> 0) Then
                        If (qSelectedTest.First.CalcTestIDs.Trim = "") Then
                            qSelectedTest.First.CalcTestIDs = pCalcTestID.ToString
                            qSelectedTest.First.CalcTestNames = pCalcTestName
                        Else
                            qSelectedTest.First.CalcTestIDs &= ", " & pCalcTestID.ToString
                            qSelectedTest.First.CalcTestNames &= ", " & pCalcTestName
                        End If
                    End If
                    ' XB 02/12/2014 - BA-1867
                Else
                    Dim canBeUnselected As Boolean = False

                    'Test can be unselected only if it has not been sent to the Analyzer
                    If (qSelectedTest.First.OTStatus = "OPEN") Then
                        If (Not qSelectedTest.First().IsTestProfileIDNull AndAlso qSelectedTest.First().TestProfileID > 0) Then
                            ' XB 02/12/2014 - BA-1867
                            'If the Test is linked to a selected Test Profile, the Profile has to be also unselected in the TreeView, 
                            'but this process is executed only when the Test is unselected by clicking in it, not when it is unselected
                            'by unselect the Calculated Test in which Formula the Test is included 
                            If (pCalcTestID = 0) Then
                                ' XB 02/12/2014 - BA-1867
                                'BT #1633 - Local variables needed to call new function DeleteProfileInformation
                                Dim myTestID As Integer = -1
                                Dim myTestType As String = String.Empty
                                Dim testProfileID As Integer = qSelectedTest.First().TestProfileID

                                'If the Test is linked to a selected Test Profile, the Profile has to be also unselected in the TreeView
                                'Search the Profile on the TreeView to unchecked it
                                If (bsProfilesTreeView.Nodes.Find(testProfileID.ToString, False).Length <> 0) Then
                                    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(testProfileID.ToString, True).First, TreeNode)
                                    myTreeNode.Checked = False

                                    'Unselect also all the Tests in the Profile
                                    For Each myNode As TreeNode In myTreeNode.Nodes
                                        myNode.Checked = myTreeNode.Checked

                                        'BT #1633 - If it not the clicked Test, remove Profile Information from the SelectedTestsDS according 
                                        '           the TestType and TestID
                                        If (myNode.Name.Trim <> qSelectedTest.First().TestKey.Trim) Then
                                            myTestType = myNode.Name.Split(CChar("|"))(0)
                                            myTestID = Convert.ToInt32(myNode.Name.Split(CChar("|"))(1))

                                            DeleteProfileInformation(myTestType, myTestID, testProfileID)
                                        End If
                                    Next

                                    'Search if the Test is also in another Test Profile 
                                    myTreeNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)
                                    If (myTreeNode.Name <> String.Empty) Then
                                        If (myTreeNode.Parent Is Nothing) Then
                                            qSelectedTest.First.TestProfileID = CType(myTreeNode.Name, Integer)
                                            qSelectedTest.First.TestProfileName = myTreeNode.Text
                                        Else
                                            qSelectedTest.First.TestProfileID = CType(myTreeNode.Parent.Name, Integer)
                                            qSelectedTest.First.TestProfileName = myTreeNode.Parent.Text
                                        End If
                                    Else
                                        qSelectedTest.First.TestProfileID = 0
                                        qSelectedTest.First.TestProfileName = ""

                                        canBeUnselected = True
                                    End If
                                End If

                                ' XB 02/12/2014 - BA-1867
                            Else
                                'Nothing to do, the Test remains selected because is linked to a selected Test Profile...
                                'Search the Calculated Test ID in field CalcTestIDs to remove it
                                'Search the Calculated Test Name in field CalcTestNames to remove it
                                qSelectedTest.First.CalcTestIDs = RebuildStringList(qSelectedTest.First.CalcTestIDs, pCalcTestID.ToString)
                                qSelectedTest.First.CalcTestNames = RebuildStringList(qSelectedTest.First.CalcTestNames, pCalcTestName)
                            End If
                            ' XB 02/12/2014 - BA-1867



                            'If (bsProfilesTreeView.Nodes.Find(qSelectedTest.First().TestProfileID.ToString(), False).Length <> 0) Then
                            '    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(qSelectedTest.First().TestProfileID.ToString(), True).First, TreeNode)
                            '    myTreeNode.Checked = False

                            '    'Unselect also all the Tests in the Profile
                            '    For Each myNode As TreeNode In myTreeNode.Nodes
                            '        myNode.Checked = myTreeNode.Checked
                            '    Next

                            '    'Search if the Test is also in another Test Profile 
                            '    myTreeNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)
                            '    If (Not myTreeNode.Name = "") Then
                            '        If (myTreeNode.Parent Is Nothing) Then
                            '            qSelectedTest.First.TestProfileID = CType(myTreeNode.Name, Integer)
                            '            qSelectedTest.First.TestProfileName = myTreeNode.Text
                            '        Else
                            '            qSelectedTest.First.TestProfileID = CType(myTreeNode.Parent.Name, Integer)
                            '            qSelectedTest.First.TestProfileName = myTreeNode.Parent.Text
                            '        End If
                            '    Else
                            '        qSelectedTest.First.TestProfileID = 0
                            '        qSelectedTest.First.TestProfileName = ""

                            '        canBeUnselected = True
                            '    End If
                            'End If
                        Else
                            canBeUnselected = True
                        End If

                        'Finally, the Test is unselected
                        If (canBeUnselected) Then
                            ' XB 02/12/2014 - BA-1867
                            'qSelectedTest.First.Selected = False
                            'selectedTest = False

                            If (pCalcTestID <> 0) Then
                                'Test is unselected due to a linked Calculated Test has been unselected
                                'Search the Calculated Test ID in field CalcTestIDs to remove it
                                'Search the Calculated Test Name in field CalcTestNames to remove it
                                qSelectedTest.First.CalcTestIDs = RebuildStringList(qSelectedTest.First.CalcTestIDs, pCalcTestID.ToString)
                                qSelectedTest.First.CalcTestNames = RebuildStringList(qSelectedTest.First.CalcTestNames, pCalcTestName)

                                'If the ISE Test is linked to other Calculated Tests, verify if they remain selected
                                If (qSelectedTest.First.CalcTestIDs.Trim <> "") Then
                                    Dim calcTests() As String = qSelectedTest.First.CalcTestIDs.Trim.Split(CChar(", "))
                                    For i2 = 0 To calcTests.Count - 1
                                        Dim i = i2
                                        'Get position of the Calculated Test in the correspondent array
                                        Dim lstCalPos As List(Of SelectedTestsDS.SelectedTestTableRow)
                                        lstCalPos = (From a In calculatedTestList.SelectedTestTable _
                                                     Where a.TestID = Convert.ToInt32(calcTests(i).Trim) _
                                                     Select a).ToList()
                                        If (lstCalPos.Count = 1) Then
                                            'If the Calculated Test is currently unselected, remove it from fields CalcTestIDs and CalcTestNames
                                            If (Not lstCalPos.First.Selected) Then
                                                qSelectedTest.First.CalcTestIDs = RebuildStringList(qSelectedTest.First.CalcTestIDs, lstCalPos.First.TestID.ToString)
                                                qSelectedTest.First.CalcTestNames = RebuildStringList(qSelectedTest.First.CalcTestNames, lstCalPos.First.TestName)
                                            End If
                                        End If
                                    Next
                                End If

                                'El Test can be unselected when it is not linked to another selected Calculated Test
                                canBeUnselected = (qSelectedTest.First.CalcTestIDs.Trim = "")
                            Else
                                'Test is unselected by clicking in it
                                If (qSelectedTest.First.CalcTestIDs.Trim <> "") Then
                                    'Get all different selected Calculated Tests to which the Test is linked
                                    Dim calcTests() As String = qSelectedTest.First.CalcTestIDs.Trim.Split(CChar(", "))
                                    For i = 0 To calcTests.Count - 1
                                        ' ReSharper disable once InconsistentNaming
                                        Dim aux_i = i
                                        'Get position of the Calculated Test in the correspondent array
                                        Dim lstCalPos As List(Of SelectedTestsDS.SelectedTestTableRow)
                                        lstCalPos = (From a In calculatedTestList.SelectedTestTable _
                                                     Where a.TestID = Convert.ToInt32(calcTests(aux_i).Trim) _
                                                     Select a).ToList()
                                        If (lstCalPos.Count = 1) Then
                                            'Only if the Calculated Test is currently selected, unselect it but without unselect its components
                                            If (lstCalPos.First.Selected) Then
                                                MarkUnMarkCalculatedTestCell(lstCalPos.First.Row, lstCalPos.First.Col, False)
                                            End If
                                        End If
                                    Next i
                                    qSelectedTest.First.CalcTestIDs = ""
                                    qSelectedTest.First.CalcTestNames = ""
                                End If
                            End If
                        End If

                        'Finally, the Test is unselected
                        If (canBeUnselected) Then
                            qSelectedTest.First.Selected = False
                            selectedTest = False
                        Else
                            'The Test recovers its previous status
                            qSelectedTest.First.Selected = True
                            selectedTest = True
                        End If
                        ' XB 02/12/2014 - BA-1867
                    Else
                        'Only for Controls... if not all Controls needed for Test are included in the WorkSession
                        If (qSelectedTest.First.PartiallySelected) Then
                            qSelectedTest.First.Selected = False
                            selectedTest = False
                        End If
                    End If
                End If

                'Set the color for Selected and Not Selected cells
                Dim bFind As Boolean = False
                Dim maxNumberValidation As Boolean = False
                If (qSelectedTest.First.OTStatus = "OPEN") Then
                    If (qSelectedTest.First().Selected) Then
                        If (bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.IndianRed) Then numSTDRed -= 1

                        bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.LightSlateGray
                        bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.LightSlateGray
                        bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.White
                        bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.White

                        maxNumberValidation = (Not maxPatientOTsReached)
                    Else
                        bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.White
                        bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.White

                        If (Not qSelectedTest.First.PartiallySelected) Then
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Black
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Black
                        Else
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Red
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Red
                        End If

                        'If the warning of maximum number of Tests have been shown, when a Test is unselected the verification is executed again
                        If (maxPatientOTsReached) Then
                            maxNumberValidation = True
                            maxPatientOTsReached = False
                        End If

                        For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In selectTestList.SelectedTestTable.Rows
                            If (qSelectedTest.First.TestID = selectedRow.TestID) Then
                                bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.IndianRed
                                bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.IndianRed
                                bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Black
                                bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Black

                                numSTDRed += 1
                                bFind = True
                                Exit For
                            End If
                        Next selectedRow
                    End If
                Else
                    'Only for Controls... if the Control is included in a WorkSession but only partially
                    If (qSelectedTest.First.PartiallySelected) Then
                        'Allow selecting the Test
                        If (qSelectedTest.First().Selected) Then
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.LightSlateGray
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.LightSlateGray
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.White
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.White

                        Else
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.White
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.White
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Red
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Red
                        End If
                    End If
                End If

                'Verify if the maximum number of allowed selected Tests has been reached
                If (maxNumberValidation) Then ControlMsgMaxNumberReached()

                'Verify if a Test previously selected have been unselected
                If (Not maxPatientOTsReached) Then ControlMSGDeleteTests(bFind)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkISETestCell", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkISETestCell", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return selectedTest
    End Function

    ''' <summary>
    ''' Manages selection/unselection of offsystem Tests
    ''' </summary>
    ''' <param name="pRowIndex">Test position in the grid of offsystem Tests - Row number</param>
    ''' <param name="pColIndex">Test position in the grid of offsystem Tests - Column number</param>
    ''' <returns>A boolean value indicating if the Test was selected (True) or unselected (False). If the Test could not
    '''          be selected, returns also False</returns>
    ''' <remarks>
    ''' Created by:  DL 29/11/2010
    ''' Modified by: SG 13/03/2013 - Do not allow unselect Tests that have been requested by LIS
    '''              SA 21/05/2014 - BT #1633 ==> When a Test is clicked to unselect it, if it is included in a selected TestProfile, the TestProfile
    '''                                           is unselected, and fields TestProfileID and TestProfileName have to be cleared also for the rest of 
    '''                                           Tests included in the affected Profile (the Tests remain selected, but the Profile data is cleared 
    '''                                           because the Test Profile is not selected anymore). To clear those fields, a new function  
    '''                                           DeleteProfileInformation is called for each Tests included in the Profile (excepting the unselected one)
    '''              XB 02/12/2014 - Add functionality cases for ISE and OFFS tests included into a CALC test - BA-1867
    ''' </remarks>
    Private Function MarkUnMarkOffSystemTestCell(ByVal pRowIndex As Integer, ByVal pColIndex As Integer, Optional ByVal pCalcTestID As Integer = 0, _
                                                 Optional ByVal pCalcTestName As String = "") As Boolean
        Dim selectedTest As Boolean = False
        Try
            Dim myTreeNode As New TreeNode()

            'Search on structure of Selected Tests by Col and Row, but only if the clicked Test is not Locked
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
            qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                            Where a.Row = pRowIndex _
                          AndAlso a.Col = pColIndex _
                          AndAlso a.OTStatus <> "LISLOCK" _
                          AndAlso a.OTStatus <> "STATLOCK" _
                           Select a).ToList()

            If (qSelectedTest.Count = 1) Then
                'Validate if the clicked Test is selected or not to set the next status
                If (Not qSelectedTest.First.Selected) Then
                    'If not selected, then the selected status is set to true                    
                    qSelectedTest.First.Selected = True
                    selectedTest = True

                    ' XB 02/12/2014 - BA-1867
                    'If the Test is selected due to a Calculated Test was selected, link the CalcTest to it (ID and Name)
                    If (pCalcTestID <> 0) Then
                        If (qSelectedTest.First.CalcTestIDs.Trim = "") Then
                            qSelectedTest.First.CalcTestIDs = pCalcTestID.ToString
                            qSelectedTest.First.CalcTestNames = pCalcTestName
                        Else
                            qSelectedTest.First.CalcTestIDs &= ", " & pCalcTestID.ToString
                            qSelectedTest.First.CalcTestNames &= ", " & pCalcTestName
                        End If
                    End If
                    ' XB 02/12/2014 - BA-1867
                Else
                    Dim canBeUnselected As Boolean = False

                    'Test can be unselected only if it has not been sent to the Analyzer
                    If (qSelectedTest.First.OTStatus = "OPEN") Then
                        If (Not qSelectedTest.First().IsTestProfileIDNull AndAlso qSelectedTest.First().TestProfileID > 0) Then

                            ' XB 02/12/2014 - BA-1867
                            'If the Test is linked to a selected Test Profile, the Profile has to be also unselected in the TreeView, 
                            'but this process is executed only when the Test is unselected by clicking in it, not when it is unselected
                            'by unselect the Calculated Test in which Formula the Test is included 
                            If (pCalcTestID = 0) Then
                                ' XB 02/12/2014 - BA-1867
                                'BT #1633 - Local variables needed to call new function DeleteProfileInformation
                                Dim myTestID As Integer = -1
                                Dim myTestType As String = String.Empty
                                Dim testProfileID As Integer = qSelectedTest.First().TestProfileID

                                'If the Test is linked to a selected Test Profile, the Profile has to be also unselected in the TreeView
                                'Search the Profile on the TreeView to unchecked it
                                If (bsProfilesTreeView.Nodes.Find(testProfileID.ToString, False).Length <> 0) Then
                                    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(testProfileID.ToString, True).First, TreeNode)
                                    myTreeNode.Checked = False

                                    'Unselect also all the Tests in the Profile
                                    For Each myNode As TreeNode In myTreeNode.Nodes
                                        myNode.Checked = myTreeNode.Checked

                                        'BT #1633 - If it not the clicked Test, remove Profile Information from the SelectedTestsDS according 
                                        '           the TestType and TestID
                                        If (myNode.Name.Trim <> qSelectedTest.First().TestKey.Trim) Then
                                            myTestType = myNode.Name.Split(CChar("|"))(0)
                                            myTestID = Convert.ToInt32(myNode.Name.Split(CChar("|"))(1))

                                            DeleteProfileInformation(myTestType, myTestID, testProfileID)
                                        End If
                                    Next

                                    'Search if the Test is also in another Test Profile 
                                    myTreeNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)
                                    If (myTreeNode.Name <> String.Empty) Then
                                        If (myTreeNode.Parent Is Nothing) Then
                                            qSelectedTest.First.TestProfileID = CType(myTreeNode.Name, Integer)
                                            qSelectedTest.First.TestProfileName = myTreeNode.Text
                                        Else
                                            qSelectedTest.First.TestProfileID = CType(myTreeNode.Parent.Name, Integer)
                                            qSelectedTest.First.TestProfileName = myTreeNode.Parent.Text
                                        End If
                                    Else
                                        qSelectedTest.First.TestProfileID = 0
                                        qSelectedTest.First.TestProfileName = ""

                                        canBeUnselected = True
                                    End If
                                End If

                                ' XB 02/12/2014 - BA-1867
                            Else
                                'Nothing to do, the Test remains selected because is linked to a selected Test Profile...
                                'Search the Calculated Test ID in field CalcTestIDs to remove it
                                'Search the Calculated Test Name in field CalcTestNames to remove it
                                qSelectedTest.First.CalcTestIDs = RebuildStringList(qSelectedTest.First.CalcTestIDs, pCalcTestID.ToString)
                                qSelectedTest.First.CalcTestNames = RebuildStringList(qSelectedTest.First.CalcTestNames, pCalcTestName)
                            End If
                            ' XB 02/12/2014 - BA-1867



                            'If (bsProfilesTreeView.Nodes.Find(qSelectedTest.First().TestProfileID.ToString(), False).Length <> 0) Then
                            '    myTreeNode = CType(bsProfilesTreeView.Nodes.Find(qSelectedTest.First().TestProfileID.ToString(), True).First, TreeNode)
                            '    myTreeNode.Checked = False

                            '    'Unselect also all the Tests in the Profile
                            '    For Each myNode As TreeNode In myTreeNode.Nodes
                            '        myNode.Checked = myTreeNode.Checked
                            '    Next

                            '    'Search if the Test is also in another Test Profile 
                            '    myTreeNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)
                            '    If (Not myTreeNode.Name = "") Then
                            '        If (myTreeNode.Parent Is Nothing) Then
                            '            qSelectedTest.First.TestProfileID = CType(myTreeNode.Name, Integer)
                            '            qSelectedTest.First.TestProfileName = myTreeNode.Text
                            '        Else
                            '            qSelectedTest.First.TestProfileID = CType(myTreeNode.Parent.Name, Integer)
                            '            qSelectedTest.First.TestProfileName = myTreeNode.Parent.Text
                            '        End If
                            '    Else
                            '        qSelectedTest.First.TestProfileID = 0
                            '        qSelectedTest.First.TestProfileName = ""

                            '        canBeUnselected = True
                            '    End If
                            'End If
                        Else
                            canBeUnselected = True
                        End If

                        'Finally, the Test is unselected
                        If (canBeUnselected) Then
                            ' XB 02/12/2014 - BA-1867
                            'qSelectedTest.First.Selected = False
                            'selectedTest = False

                            If (pCalcTestID <> 0) Then
                                'Test is unselected due to a linked Calculated Test has been unselected
                                'Search the Calculated Test ID in field CalcTestIDs to remove it
                                'Search the Calculated Test Name in field CalcTestNames to remove it
                                qSelectedTest.First.CalcTestIDs = RebuildStringList(qSelectedTest.First.CalcTestIDs, pCalcTestID.ToString)
                                qSelectedTest.First.CalcTestNames = RebuildStringList(qSelectedTest.First.CalcTestNames, pCalcTestName)

                                'If the ISE Test is linked to other Calculated Tests, verify if they remain selected
                                If (qSelectedTest.First.CalcTestIDs.Trim <> "") Then
                                    Dim calcTests() As String = qSelectedTest.First.CalcTestIDs.Trim.Split(CChar(", "))
                                    For i As Integer = 0 To calcTests.Count - 1
                                        Dim aux_i = i
                                        'Get position of the Calculated Test in the correspondent array
                                        Dim lstCalPos As List(Of SelectedTestsDS.SelectedTestTableRow)
                                        lstCalPos = (From a In calculatedTestList.SelectedTestTable _
                                                     Where a.TestID = Convert.ToInt32(calcTests(aux_i).Trim) _
                                                     Select a).ToList()
                                        If (lstCalPos.Count = 1) Then
                                            'If the Calculated Test is currently unselected, remove it from fields CalcTestIDs and CalcTestNames
                                            If (Not lstCalPos.First.Selected) Then
                                                qSelectedTest.First.CalcTestIDs = RebuildStringList(qSelectedTest.First.CalcTestIDs, lstCalPos.First.TestID.ToString)
                                                qSelectedTest.First.CalcTestNames = RebuildStringList(qSelectedTest.First.CalcTestNames, lstCalPos.First.TestName)
                                            End If
                                        End If
                                    Next i
                                End If

                                'El Test can be unselected when it is not linked to another selected Calculated Test
                                canBeUnselected = (qSelectedTest.First.CalcTestIDs.Trim = "")
                            Else
                                'Test is unselected by clicking in it
                                If (qSelectedTest.First.CalcTestIDs.Trim <> "") Then
                                    'Get all different selected Calculated Tests to which the Test is linked
                                    Dim calcTests() As String = qSelectedTest.First.CalcTestIDs.Trim.Split(CChar(", "))
                                    For i As Integer = 0 To calcTests.Count - 1
                                        Dim aux_i = i
                                        'Get position of the Calculated Test in the correspondent array
                                        Dim lstCalPos As List(Of SelectedTestsDS.SelectedTestTableRow)
                                        lstCalPos = (From a In calculatedTestList.SelectedTestTable _
                                                     Where a.TestID = Convert.ToInt32(calcTests(aux_i).Trim) _
                                                     Select a).ToList()
                                        If (lstCalPos.Count = 1) Then
                                            'Only if the Calculated Test is currently selected, unselect it but without unselect its components
                                            If (lstCalPos.First.Selected) Then
                                                MarkUnMarkCalculatedTestCell(lstCalPos.First.Row, lstCalPos.First.Col, False)
                                            End If
                                        End If
                                    Next
                                    qSelectedTest.First.CalcTestIDs = ""
                                    qSelectedTest.First.CalcTestNames = ""
                                End If
                            End If
                        End If

                        'Finally, the Test is unselected
                        If (canBeUnselected) Then
                            qSelectedTest.First.Selected = False
                            selectedTest = False
                        Else
                            'The Test recovers its previous status
                            qSelectedTest.First.Selected = True
                            selectedTest = True
                        End If
                        ' XB 02/12/2014 - BA-1867

                    End If
                End If

                'Set the color for Selected and Not Selected cells
                Dim bFind As Boolean = False
                Dim maxNumberValidation As Boolean = False
                If (qSelectedTest.First.OTStatus = "OPEN") Then
                    If (qSelectedTest.First().Selected) Then
                        If (bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.IndianRed) Then numSTDRed -= 1

                        bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.LightSlateGray
                        bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.LightSlateGray
                        bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.White
                        bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.White

                        maxNumberValidation = (Not maxPatientOTsReached)
                    Else
                        bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.White
                        bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.White

                        If (Not qSelectedTest.First.PartiallySelected) Then
                            bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Black
                            bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Black
                        Else
                            bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Red
                            bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Red
                        End If

                        'If the warning of maximum number of Tests have been shown, when a Test is unselected the verification is executed again
                        If (maxPatientOTsReached) Then
                            maxNumberValidation = True
                            maxPatientOTsReached = False
                        End If

                        For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In selectTestList.SelectedTestTable.Rows
                            If (qSelectedTest.First.TestType = selectedRow.TestType AndAlso qSelectedTest.First.TestID = selectedRow.TestID) Then
                                bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.IndianRed
                                bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.IndianRed
                                bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Black
                                bsOffSystemTestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Black

                                numSTDRed += 1
                                bFind = True
                                Exit For
                            End If
                        Next selectedRow
                    End If
                End If

                'Verify if the maximum number of allowed selected Tests has been reached
                If (maxNumberValidation) Then ControlMsgMaxNumberReached()

                'Verify if a Test previously selected have been unselected
                If (Not maxPatientOTsReached) Then ControlMSGDeleteTests(bFind)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkOffSystemTestCell", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkOffSystemTestCell", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return selectedTest
    End Function

    ''' <summary>
    ''' Verify if the maximum number of allowed Tests have been reached to show/clear the correspondent warning
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 30/04/2010
    ''' </remarks>
    Private Sub ControlMsgMaxNumberReached()
        Try
            If (VerifyMaxNumberReached()) Then
                Dim myGlobalDataTO As New GlobalDataTO
                Dim myMessages As New MessageDelegate

                myGlobalDataTO = myMessages.GetMessageDescription(Nothing, GlobalEnumerates.Messages.MAX_PATIENT_ORDERTESTS.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myMessagesDS As MessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                        bsWarningMessageLabel.Text = myMessagesDS.tfmwMessages(0).MessageText
                    End If
                End If

                maxPatientOTsReached = True
                bsAdviceGroupBox.Visible = True

                bsAcceptButton.Enabled = False
            Else
                bsAdviceGroupBox.Visible = False
                bsAcceptButton.Enabled = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ControlMsgMaxNumberReached", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ControlMsgMaxNumberReached", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' If a Test that was selected is unselected, shows the corresponding warning
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 30/04/2010
    ''' </remarks>
    Private Sub ControlMSGDeleteTests(ByVal pShowMessage As Boolean)
        Try
            If (pShowMessage Or numSTDRed > 0) Then
                Dim myGlobalDataTO As New GlobalDataTO
                Dim myMessages As New MessageDelegate

                myGlobalDataTO = myMessages.GetMessageDescription(Nothing, GlobalEnumerates.Messages.DELETE_TESTS.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myMessagesDS As MessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                    If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                        bsWarningMessageLabel.Text = myMessagesDS.tfmwMessages(0).MessageText
                    End If
                End If
                bsAdviceGroupBox.Visible = True

            ElseIf (numSTDRed = 0) Then
                bsAdviceGroupBox.Visible = False
            End If
            bsTestListDataGridView.Refresh()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ControlMSGDeleteTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ControlMSGDeleteTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' If the volume of Reagents needed for ISE Tests are not enough (low volume), a warning message is shown when the screen is loaded
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 20/03/2012
    ''' </remarks>
    Private Sub ShowMSGIseReagentsNotEnough()
        Try
            Dim myMessages As New MessageDelegate
            Dim myGlobalDataTO As New GlobalDataTO

            myGlobalDataTO = myMessages.GetMessageDescription(Nothing, GlobalEnumerates.Messages.ISE_NOT_ENOUGH_REAGENTS_VOL.ToString)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMessagesDS As MessagesDS = DirectCast(myGlobalDataTO.SetDatos, MessagesDS)

                If (myMessagesDS.tfmwMessages.Rows.Count > 0) Then
                    bsWarningMessageLabel.Text = myMessagesDS.tfmwMessages(0).MessageText
                    bsAdviceGroupBox.Visible = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ShowMSGIseReagentsNotEnough", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ShowMSGIseReagentsNotEnough", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load Standard Tests in the corresponding GridView, and mark as selected all
    ''' Tests loaded from the previous screen
    ''' </summary>
    ''' <param name="pTestsDS">Typed DataSet containing all Tests that have to be shown</param>
    ''' <remarks>
    ''' Created by:  TR 08/02/2010 
    ''' Modified by: SA 02/11/2010 - Removed call to MarkSelectedProfilesOnTreeView
    '''              TR 10/03/2011 - Inform also field FactoryCalib in the DS of STD Tests
    '''              RH 12/03/2012 - Introduce COLUMN_COUNT const
    '''              SG 13/03/2013 - Set OTStatus = "LISLOCK" if the Test was requested by LIS
    ''' </remarks>
    Private Sub FillAndMarkTestListGridView(ByVal pTestsDS As TestsDS)
        Try
            Dim maxSTDTests As Integer = pTestsDS.tparTests.Rows.Count

            Dim qStandardTests As List(Of TestsDS.tparTestsRow)
            qStandardTests = (From a In pTestsDS.tparTests Select a).ToList()

            If (maxSTDTests > 0) Then
                'Set the number of rows required for the GridView
                Dim numGridRows As Integer = (maxSTDTests \ COLUMN_COUNT)

                If (maxSTDTests Mod COLUMN_COUNT) = 0 Then numGridRows -= 1
                bsTestListDataGridView.Rows.Add(numGridRows + 1)

                Dim testPosition As Integer = 0
                Dim standarPos As Integer = 0
                Dim newTestRow As SelectedTestsDS.SelectedTestTableRow

                For j As Integer = 0 To numGridRows
                    For k As Integer = 0 To COLUMN_COUNT - 1
                        testPosition = (COLUMN_COUNT * j) + k
                        If (testPosition >= maxSTDTests) Then Exit For
                        bsTestListDataGridView.Rows(j).Cells(k).Value = qStandardTests(standarPos).ShortName
                        bsTestListDataGridView.Rows(j).Cells(k).ToolTipText = qStandardTests(standarPos).TestName

                        newTestRow = standardTestList.SelectedTestTable.NewSelectedTestTableRow
                        newTestRow.Row = j
                        newTestRow.Col = k
                        newTestRow.SampleType = SampleTypeAttribute
                        newTestRow.TestType = "STD"
                        newTestRow.TestID = qStandardTests(standarPos).TestID
                        newTestRow.TestName = qStandardTests(standarPos).TestName
                        newTestRow.TestProfileName = ""
                        newTestRow.Selected = False
                        newTestRow.OTStatus = "OPEN"

                        If (qStandardTests(standarPos).IsFactoryCalibNull) Then
                            newTestRow.FactoryCalib = False
                        Else
                            newTestRow.FactoryCalib = qStandardTests(standarPos).FactoryCalib
                        End If

                        'BT1494 - Inform the field that indicates if the Calibration Programming is completed for the Test/SampleType
                        newTestRow.EnableStatus = qStandardTests(standarPos).EnableStatus

                        'Only for Controls: needed to verify Controls partially selected
                        If (Not qStandardTests(standarPos).IsNumberOfControlsNull) Then
                            newTestRow.NumberOfControls = qStandardTests(standarPos).NumberOfControls
                        End If

                        newTestRow.PartiallySelected = False
                        standardTestList.SelectedTestTable.Rows.Add(newTestRow)

                        'Increase the next position for the list of standard Tests
                        standarPos += 1
                    Next k
                Next j

                'Set the width of each column
                For k As Integer = 0 To COLUMN_COUNT - 1
                    bsTestListDataGridView.Columns(k).Width = GRID_CELLS_WIDTH
                    bsTestListDataGridView.Columns(k).Resizable = DataGridViewTriState.False
                Next

                'If the list of selected Tests have records then mark the selected Tests on the screen
                If (Not ListOfSelectedTestsAttribute Is Nothing) Then
                    'Dim qSelectedTest As New List(Of SelectedTestsDS.SelectedTestTableRow)
                    Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                    If (SampleClassAttribute = "BLANK") Then
                        qSelectedTest = (From a In ListOfSelectedTestsAttribute.SelectedTestTable _
                                        Where a.TestType = "STD" _
                                       Select a).ToList()
                    Else
                        qSelectedTest = (From a In ListOfSelectedTestsAttribute.SelectedTestTable _
                                        Where a.TestType = "STD" And a.SampleType = SampleTypeAttribute _
                                       Select a).ToList()
                    End If

                    For Each selTestRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                        Dim myCurrentTest As Integer = selTestRow.TestID

                        Dim qSearchTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                        qSearchTest = (From a In standardTestList.SelectedTestTable _
                                      Where a.TestID = myCurrentTest _
                                     Select a).ToList()

                        If (qSearchTest.Count = 1) Then
                            qSearchTest.First.Selected = True

                            'SG 13/03/2013
                            If (Not selTestRow.IsLISRequestNull AndAlso selTestRow.LISRequest) Then
                                qSearchTest.First.OTStatus = "LISLOCK"
                            Else
                                qSearchTest.First.OTStatus = selTestRow.OTStatus
                            End If

                            'For PATIENTS, set value of fields containing Test Profile and Calculated Tests information
                            If (SampleClassAttribute = "PATIENT") Then
                                If (Not selTestRow.IsTestProfileIDNull) Then
                                    qSearchTest.First.TestProfileID = selTestRow.TestProfileID
                                    qSearchTest.First.TestProfileName = selTestRow.TestProfileName
                                End If

                                If (Not selTestRow.IsCalcTestIDsNull) Then
                                    qSearchTest.First.CalcTestIDs = selTestRow.CalcTestIDs
                                    qSearchTest.First.CalcTestNames = selTestRow.CalcTestNames
                                End If
                            End If

                            'For CONTROLS, verify if it is partially selected
                            Dim markAsSelected As Boolean = True
                            If (SampleClassAttribute = "CTRL") Then
                                'Test with more than one Control for which not all of them are selected, are marked
                                'as not selected (and then the unique possible action is select it to add the rest of 
                                'Controls, but the delete has to be done using the DEL button in the WS Preparation screen)
                                markAsSelected = (qSearchTest.First.NumberOfControls = selTestRow.NumberOfControls)
                                qSearchTest.First.Selected = markAsSelected
                                qSearchTest.First.PartiallySelected = (Not markAsSelected)
                            End If

                            Dim row As Integer = qSearchTest.First.Row
                            Dim col As Integer = qSearchTest.First.Col
                            If (markAsSelected) Then
                                If (selTestRow.OTStatus = "OPEN" AndAlso Not selTestRow.LISRequest) Then
                                    'Change the Background Color and the Foreground Color to indicate that this Test is selected
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.BackColor = SELECTED_BACK_COLOR
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.SelectionBackColor = SELECTED_BACK_COLOR
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.ForeColor = SELECTED_FORE_COLOR
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.SelectionForeColor = SELECTED_FORE_COLOR
                                ElseIf (selTestRow.LISRequest) Then 'SGM 13/03/2013
                                    'Change the Background Color and the Foreground Color to indicate that this Test requested by LIS
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.BackColor = LIS_REQUESTED_BACK_COLOR
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.SelectionBackColor = LIS_REQUESTED_BACK_COLOR
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.ForeColor = LIS_REQUESTED_FORE_COLOR
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.SelectionForeColor = LIS_REQUESTED_FORE_COLOR
                                Else
                                    'Change the Background Color and the Foreground Color to indicate that this Test is selected and cannot be unselected
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.BackColor = INPROCESS_BACK_COLOR
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.SelectionBackColor = INPROCESS_BACK_COLOR
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.ForeColor = INPROCESS_FORE_COLOR
                                    bsTestListDataGridView.Rows(row).Cells(col).Style.SelectionForeColor = INPROCESS_FORE_COLOR
                                End If

                            ElseIf (qSearchTest.First.PartiallySelected) Then  'Only for CONTROLS
                                'Change the Foreground Color to indicate that not all Controls needed for the Test are selected
                                bsTestListDataGridView.Rows(row).Cells(col).Style.ForeColor = Color.Red
                                bsTestListDataGridView.Rows(row).Cells(col).Style.SelectionForeColor = Color.Red
                            End If
                        End If
                    Next
                End If

                'MarkSelectedProfilesOnTreeView()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillAndMarkTestListGridView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillTestListGridView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load available Calculated Tests in the corresponding GridView, and mark as selected all
    ''' Tests loaded from the previous screen
    ''' </summary>
    ''' <param name="pCalcTestsDS">Typed DataSet containing all Calculated Tests that have to be shown</param>
    ''' <remarks>
    ''' Created by:  SA 04/05/2010
    ''' Modified by: SA 02/11/2010 - Inform TestProfileID and TestProfileName when the previously selected Calculated
    '''                              Tests have these fields informed
    '''              RH 12/03/2012 - Introduce COLUMN_COUNT const
    '''              SG 13/03/2013 - Set OTStatus = "LISLOCK" if the Test was requested by LIS
    ''' </remarks>
    Private Sub FillAndMarkCalcTestListGridView(ByVal pCalcTestsDS As CalculatedTestsDS)
        Try
            Dim maxCALCTests As Integer = pCalcTestsDS.tparCalculatedTests.Rows.Count

            Dim qCalculatedTests As List(Of CalculatedTestsDS.tparCalculatedTestsRow)
            qCalculatedTests = (From a In pCalcTestsDS.tparCalculatedTests Select a).ToList()

            If (maxCALCTests > 0) Then
                'Set the number of rows required for the GridView
                Dim numGridRows As Integer = (maxCALCTests \ COLUMN_COUNT)

                If (maxCALCTests Mod COLUMN_COUNT) = 0 Then numGridRows -= 1
                bsCalcTestDataGridView.Rows.Add(numGridRows + 1)

                Dim testPosition As Integer = 0
                Dim standarPos As Integer = 0
                'Dim resultData As New GlobalDataTO
                Dim resultData As GlobalDataTO = Nothing
                Dim myFormulaDelegate As New FormulasDelegate
                Dim newTestRow As SelectedTestsDS.SelectedTestTableRow

                For j As Integer = 0 To numGridRows
                    For k As Integer = 0 To COLUMN_COUNT - 1
                        testPosition = (COLUMN_COUNT * j) + k
                        If (testPosition >= maxCALCTests) Then Exit For

                        bsCalcTestDataGridView.Rows(j).Cells(k).Value = pCalcTestsDS.tparCalculatedTests(standarPos).CalcTestName
                        bsCalcTestDataGridView.Rows(j).Cells(k).ToolTipText = pCalcTestsDS.tparCalculatedTests(standarPos).CalcTestLongName & _
                                                                              "=" & pCalcTestsDS.tparCalculatedTests(standarPos).FormulaText

                        newTestRow = calculatedTestList.SelectedTestTable.NewSelectedTestTableRow
                        newTestRow.Row = j
                        newTestRow.Col = k
                        newTestRow.SampleType = SampleTypeAttribute
                        newTestRow.TestType = "CALC"
                        newTestRow.TestID = pCalcTestsDS.tparCalculatedTests(standarPos).CalcTestID
                        newTestRow.TestName = pCalcTestsDS.tparCalculatedTests(standarPos).CalcTestLongName
                        newTestRow.TestProfileName = ""
                        newTestRow.Selected = False
                        newTestRow.OTStatus = "OPEN"
                        newTestRow.PartiallySelected = False
                        calculatedTestList.SelectedTestTable.Rows.Add(newTestRow)

                        'Increase the next position for the list of calculated Tests
                        standarPos += 1
                    Next
                Next

                'Set the width of each column
                For k As Integer = 0 To COLUMN_COUNT - 1
                    bsCalcTestDataGridView.Columns(k).Width = GRID_CELLS_WIDTH
                    bsCalcTestDataGridView.Columns(k).Resizable = DataGridViewTriState.False
                Next

                For Each calculatedTest As SelectedTestsDS.SelectedTestTableRow In calculatedTestList.SelectedTestTable.Rows
                    'Get the list of Tests included in the Formula of the Calculated Test
                    resultData = myFormulaDelegate.GetTestsInFormula(Nothing, calculatedTest.TestID)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myFormulaDS As FormulasDS = DirectCast(resultData.SetDatos, FormulasDS)

                        For Each testRow As FormulasDS.tparFormulasRow In myFormulaDS.tparFormulas.Rows
                            'For Calculated Tests, verify it has not been included yet
                            Dim lstIncludedTests As List(Of FormulasDS.tparFormulasRow)
                            lstIncludedTests = (From a In calcTestComponents.tparFormulas _
                                               Where a.CalcTestID = testRow.CalcTestID _
                                             AndAlso a.TestType = testRow.TestType _
                                             AndAlso a.SampleType = testRow.SampleType _
                                             AndAlso a.Value = testRow.Value _
                                              Select a).ToList()
                            If (lstIncludedTests.Count = 0) Then calcTestComponents.tparFormulas.ImportRow(testRow)
                        Next
                    Else
                        'Error getting the list of Tests/SampleTypes included in the Calculated Test Formula
                        ShowMessage(Me.Name & ".FillAndMarkCalcTestListGridView", resultData.ErrorCode, resultData.ErrorMessage, Me)
                        Exit For
                    End If
                Next

                If (Not resultData.HasError) Then
                    'If the list of selected Tests have records then mark the selected Tests on the screen
                    If (Not ListOfSelectedTestsAttribute Is Nothing) Then
                        'Verify Calculated Tests previously selected
                        Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                        qSelectedTest = (From a In ListOfSelectedTestsAttribute.SelectedTestTable _
                                         Where a.TestType = "CALC" _
                                         AndAlso a.SampleType = SampleTypeAttribute _
                                         Select a).ToList()

                        For Each selTestRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                            For Each calcTestROW As SelectedTestsDS.SelectedTestTableRow In calculatedTestList.SelectedTestTable.Rows
                                If (calcTestROW.TestID = selTestRow.TestID) Then
                                    calcTestROW.Selected = True

                                    'SG 13/03/2013
                                    If (Not selTestRow.IsLISRequestNull AndAlso selTestRow.LISRequest) Then
                                        calcTestROW.OTStatus = "LISLOCK"
                                    Else
                                        calcTestROW.OTStatus = selTestRow.OTStatus
                                    End If

                                    If (Not selTestRow.IsTestProfileIDNull) Then
                                        calcTestROW.TestProfileID = selTestRow.TestProfileID
                                        calcTestROW.TestProfileName = selTestRow.TestProfileName
                                    End If

                                    If (Not selTestRow.IsCalcTestIDsNull AndAlso Not selTestRow.CalcTestIDs.Trim = "") Then
                                        calcTestROW.CalcTestIDs = selTestRow.CalcTestIDs
                                        calcTestROW.CalcTestNames = selTestRow.CalcTestNames
                                    End If

                                    If (selTestRow.OTStatus = "OPEN" AndAlso Not selTestRow.LISRequest) Then
                                        'Change the Background Color and the Foreground Color to indicate that this Test is selected
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.BackColor = SELECTED_BACK_COLOR
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.SelectionBackColor = SELECTED_BACK_COLOR
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.ForeColor = SELECTED_FORE_COLOR
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.SelectionForeColor = SELECTED_FORE_COLOR
                                    ElseIf (selTestRow.LISRequest) Then 'SGM 13/03/2013
                                        'Change the Background Color and the Foreground Color to indicate that this Test is requested by LIS
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.BackColor = LIS_REQUESTED_BACK_COLOR
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.SelectionBackColor = LIS_REQUESTED_BACK_COLOR
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.ForeColor = LIS_REQUESTED_FORE_COLOR
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.SelectionForeColor = LIS_REQUESTED_FORE_COLOR
                                    Else
                                        'Change the Background Color and the Foreground Color to indicate that this Test is selected and cannot be unselected
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.BackColor = INPROCESS_BACK_COLOR
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.SelectionBackColor = INPROCESS_BACK_COLOR
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.ForeColor = INPROCESS_FORE_COLOR
                                        bsCalcTestDataGridView.Rows(calcTestROW.Row).Cells(calcTestROW.Col).Style.SelectionForeColor = INPROCESS_FORE_COLOR
                                    End If
                                    Exit For
                                End If
                            Next
                        Next
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillAndMarkCalcTestListGridView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillAndMarkCalcTestListGridView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load available ISE Tests in the corresponding GridView, and mark as selected all
    ''' Tests loaded from the previous screen
    ''' </summary>
    ''' <param name="pISETestsDS">Typed DataSet containing all ISE Tests that have to be shown</param>
    ''' <remarks>
    ''' Created by:  DL 21/10/2010
    ''' Modified by: SA 02/11/2010 - Inform TestProfileID and TestProfileName when the previously selected ISE
    '''                              Tests have these fields informed
    '''              RH 12/03/2012 - Introduced COLUMN_COUNT const
    '''              SA 20/03/2012 - When ISE Module is installed, if the current volume of the needed Reagents is marked
    '''                              as not enough, the ISE Tests grid is disabled and a warning message is shown in the
    '''                              warning bottom bar
    '''              SA 19/06/2012 - When informed, save value of field NumberOfControls in the DS containing all ISE Tests
    '''                              (needed to verify ISE Controls partially selected)
    '''              XB 27/07/2012 - ISE Tests Disabled by volume not enough functionallity is canceled
    '''              SG 13/03/2013 - Set OTStatus = "LISLOCK" if the Test was requested by LIS
    '''              XB 14/01/2015 - Add management when unselect tests for ISE tests included on CALC test - BA-1867
    ''' </remarks>
    Private Sub FillAndMarkISETestListGridView(ByVal pISETestsDS As ISETestsDS)
        Try
            Dim maxISETests As Integer = pISETestsDS.tparISETests.Rows.Count

            Dim qISETests As List(Of ISETestsDS.tparISETestsRow)
            qISETests = pISETestsDS.tparISETests.ToList()

            If (maxISETests > 0) Then
                'Set the number of rows required for the GridView
                Dim numGridRows As Integer = (maxISETests \ COLUMN_COUNT)

                If (maxISETests Mod COLUMN_COUNT) = 0 Then numGridRows -= 1
                bsISETestDataGridView.Rows.Add(numGridRows + 1)

                Dim testPosition As Integer = 0
                Dim standarPos As Integer = 0

                'Dim resultData As New GlobalDataTO
                Dim newTestRow As SelectedTestsDS.SelectedTestTableRow

                For j As Integer = 0 To numGridRows
                    For k As Integer = 0 To COLUMN_COUNT - 1
                        testPosition = (COLUMN_COUNT * j) + k
                        If (testPosition >= maxISETests) Then Exit For

                        bsISETestDataGridView.Rows(j).Cells(k).Value = pISETestsDS.tparISETests(standarPos).ShortName
                        bsISETestDataGridView.Rows(j).Cells(k).ToolTipText = pISETestsDS.tparISETests(standarPos).Name

                        newTestRow = iseTestList.SelectedTestTable.NewSelectedTestTableRow
                        newTestRow.Row = j
                        newTestRow.Col = k
                        newTestRow.SampleType = SampleTypeAttribute
                        newTestRow.TestType = "ISE"
                        newTestRow.TestID = pISETestsDS.tparISETests(standarPos).ISETestID
                        newTestRow.TestName = pISETestsDS.tparISETests(standarPos).Name
                        newTestRow.TestProfileName = ""
                        newTestRow.Selected = False
                        newTestRow.OTStatus = "OPEN"
                        newTestRow.PartiallySelected = False
                        iseTestList.SelectedTestTable.Rows.Add(newTestRow)

                        'Only for Controls: needed to verify Controls partially selected
                        If (Not qISETests(standarPos).IsNumberOfControlsNull) Then
                            newTestRow.NumberOfControls = qISETests(standarPos).NumberOfControls
                        End If
                        newTestRow.PartiallySelected = False

                        'Increase the next position for the list of ISE Tests
                        standarPos += 1
                    Next k
                Next j

                'Set the width of each column
                For k As Integer = 0 To COLUMN_COUNT - 1
                    bsISETestDataGridView.Columns(k).Width = GRID_CELLS_WIDTH
                    bsISETestDataGridView.Columns(k).Resizable = DataGridViewTriState.False
                Next

                'If the list of selected Tests have records then mark the selected Tests on the screen
                If (Not ListOfSelectedTestsAttribute Is Nothing) Then
                    'Verify ISE Tests previously selected
                    Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                    qSelectedTest = (From a In ListOfSelectedTestsAttribute.SelectedTestTable _
                                    Where a.TestType = "ISE" _
                                  AndAlso a.SampleType = SampleTypeAttribute _
                                   Select a).ToList()

                    For Each selTestRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                        For Each iseTestROW As SelectedTestsDS.SelectedTestTableRow In iseTestList.SelectedTestTable.Rows
                            If (iseTestROW.TestID = selTestRow.TestID) Then
                                iseTestROW.Selected = True

                                'SG 13/03/2013
                                If (Not selTestRow.IsLISRequestNull AndAlso selTestRow.LISRequest) Then
                                    iseTestROW.OTStatus = "LISLOCK"
                                Else
                                    iseTestROW.OTStatus = selTestRow.OTStatus
                                End If

                                ' XB - 14/01/2015 - BA-1867
                                'If (Not selTestRow.IsTestProfileIDNull) Then
                                '    iseTestROW.TestProfileID = selTestRow.TestProfileID
                                '    iseTestROW.TestProfileName = selTestRow.TestProfileName
                                'End If

                                'For PATIENTS, set value of fields containing Test Profile and Calculated Tests information
                                If (SampleClassAttribute = "PATIENT") Then
                                    If (Not selTestRow.IsTestProfileIDNull) Then
                                        iseTestROW.TestProfileID = selTestRow.TestProfileID
                                        iseTestROW.TestProfileName = selTestRow.TestProfileName
                                    End If

                                    If (Not selTestRow.IsCalcTestIDsNull) Then
                                        iseTestROW.CalcTestIDs = selTestRow.CalcTestIDs
                                        iseTestROW.CalcTestNames = selTestRow.CalcTestNames
                                    End If
                                End If
                                ' XB - 14/01/2015 - BA-1867

                                Dim markAsSelected As Boolean = True
                                If (SampleClassAttribute = "CTRL") Then
                                    'ISE Tests with more than one Control for which not all of them are selected, are marked as not selected
                                    '(and then the unique possible action is select it to add the rest of Controls, but the delete has to be 
                                    'done using the DEL button in the WS Samples Request screen)
                                    markAsSelected = (iseTestROW.NumberOfControls = selTestRow.NumberOfControls)
                                    iseTestROW.Selected = markAsSelected
                                    iseTestROW.PartiallySelected = (Not markAsSelected)
                                End If

                                If (markAsSelected) Then
                                    If (selTestRow.OTStatus = "OPEN" AndAlso Not selTestRow.LISRequest) Then
                                        'Change the Background Color and the Foreground Color to indicate that this Test is selected
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.BackColor = SELECTED_BACK_COLOR
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.SelectionBackColor = SELECTED_BACK_COLOR
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.ForeColor = SELECTED_FORE_COLOR
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.SelectionForeColor = SELECTED_FORE_COLOR
                                    ElseIf (selTestRow.LISRequest) Then 'SGM 13/03/2013
                                        'Change the Background Color and the Foreground Color to indicate that this Test is requested by LIS
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.BackColor = LIS_REQUESTED_BACK_COLOR
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.SelectionBackColor = LIS_REQUESTED_BACK_COLOR
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.ForeColor = LIS_REQUESTED_FORE_COLOR
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.SelectionForeColor = LIS_REQUESTED_FORE_COLOR
                                    Else
                                        'Change the Background Color and the Foreground Color to indicate that this Test is selected and cannot be unselected
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.BackColor = INPROCESS_BACK_COLOR
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.SelectionBackColor = INPROCESS_BACK_COLOR
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.ForeColor = INPROCESS_FORE_COLOR
                                        bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.SelectionForeColor = INPROCESS_FORE_COLOR
                                    End If
                                ElseIf (iseTestROW.PartiallySelected) Then  'Only for CONTROLS
                                    'Change the Foreground Color to indicate that not all Controls needed for the Test are selected
                                    bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.ForeColor = Color.Red
                                    bsISETestDataGridView.Rows(iseTestROW.Row).Cells(iseTestROW.Col).Style.SelectionForeColor = Color.Red
                                End If
                                Exit For
                            End If
                        Next iseTestROW
                    Next selTestRow
                End If

                'Get the object needed to get the Analyzer properties
                If (Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing) Then
                    mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                End If

                ' XB 27/07/2012 - This functionallity is canceled - spec 20/07/2012
                ''If the connected Analyzer has not an ISE Module installed, or it is installed but there are not enough volume of the 
                ''needed Reagents, then the grid containing the ISE Tests is disabled, and an information message is shown
                'If (Not mdiAnalyzerCopy Is Nothing AndAlso Not mdiAnalyzerCopy.ISE_Manager Is Nothing AndAlso mdiAnalyzerCopy.ISE_Manager.IsISEModuleReady) Then
                '    If (Not mdiAnalyzerCopy.ISE_Manager.MonitorDataTO.RP_IsEnoughVolA OrElse Not mdiAnalyzerCopy.ISE_Manager.MonitorDataTO.RP_IsEnoughVolB) Then
                '        bsISETestsLabel.Enabled = False
                '        bsISETestDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Sunken
                '        bsISETestDataGridView.BackgroundColor = SystemColors.MenuBar

                '        For j As Integer = 0 To numGridRows
                '            For k As Integer = 0 To (iseTestList.SelectedTestTable.Rows.Count - 1)
                '                bsISETestDataGridView.Rows(j).Cells(k).Style.ForeColor = Color.Gray
                '                bsISETestDataGridView.Rows(j).Cells(k).Style.SelectionForeColor = Color.Gray
                '                bsISETestDataGridView.Rows(j).Cells(k).ReadOnly = True
                '            Next
                '        Next
                '        bsISETestDataGridView.ReadOnly = True
                '        bsISETestDataGridView.Enabled = False

                '        'Show a message in the bottom bar
                '        ShowMSGIseReagentsNotEnough()
                '    End If
                'End If
                ' XB 27/07/2012
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillAndMarkISETestListGridView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillAndMarkISETestListGridView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load available OffSystem Tests in the corresponding GridView, and mark as selected all
    ''' Tests loaded from the previous screen
    ''' </summary>
    ''' <param name="pOffSystemTestsDS">Typed DataSet containing all OffSystem Tests that have to be shown</param>
    ''' <remarks>
    ''' Created by:  DL 29/11/2010
    '''              RH 12/03/2012 - Introduce COLUMN_COUNT const
    '''              SG 13/03/2013 - Set OTStatus = "LISLOCK" if the Test was requested by LIS
    '''              XB 14/01/2015 - Add management when unselect tests for OFFS tests included on CALC test - BA-1867
    ''' </remarks>
    Private Sub FillAndMarkOffSystemTestListGridView(ByVal pOffSystemTestsDS As OffSystemTestsDS)
        Try
            Dim maxOffSystemTests As Integer = pOffSystemTestsDS.tparOffSystemTests.Rows.Count

            Dim qOffSystemsTests As List(Of OffSystemTestsDS.tparOffSystemTestsRow)
            qOffSystemsTests = (From a In pOffSystemTestsDS.tparOffSystemTests _
                                Select a).ToList()

            If (maxOffSystemTests > 0) Then
                'Set the number of rows required for the GridView
                Dim numGridRows As Integer = (maxOffSystemTests \ COLUMN_COUNT)

                If (maxOffSystemTests Mod COLUMN_COUNT) = 0 Then numGridRows -= 1
                bsOffSystemTestDataGridView.Rows.Add(numGridRows + 1)

                Dim testPosition As Integer = 0
                Dim standarPos As Integer = 0

                'Dim resultData As New GlobalDataTO
                'Dim resultData As GlobalDataTO
                Dim newTestRow As SelectedTestsDS.SelectedTestTableRow

                For j As Integer = 0 To numGridRows
                    For k As Integer = 0 To COLUMN_COUNT - 1
                        testPosition = (COLUMN_COUNT * j) + k
                        If (testPosition >= maxOffSystemTests) Then Exit For

                        bsOffSystemTestDataGridView.Rows(j).Cells(k).Value = pOffSystemTestsDS.tparOffSystemTests(standarPos).ShortName
                        bsOffSystemTestDataGridView.Rows(j).Cells(k).ToolTipText = pOffSystemTestsDS.tparOffSystemTests(standarPos).Name

                        newTestRow = offSystemTestList.SelectedTestTable.NewSelectedTestTableRow
                        newTestRow.Row = j
                        newTestRow.Col = k
                        newTestRow.SampleType = SampleTypeAttribute
                        newTestRow.TestType = "OFFS"
                        newTestRow.TestID = pOffSystemTestsDS.tparOffSystemTests(standarPos).OffSystemTestID
                        newTestRow.TestName = pOffSystemTestsDS.tparOffSystemTests(standarPos).Name
                        newTestRow.TestProfileName = ""
                        newTestRow.Selected = False
                        newTestRow.OTStatus = "OPEN"
                        newTestRow.PartiallySelected = False
                        offSystemTestList.SelectedTestTable.Rows.Add(newTestRow)

                        'Increase the next position for the list of calculated Tests
                        standarPos += 1
                    Next k
                Next j

                'Set the width of each column
                For k As Integer = 0 To COLUMN_COUNT - 1
                    bsOffSystemTestDataGridView.Columns(k).Width = GRID_CELLS_WIDTH
                    bsOffSystemTestDataGridView.Columns(k).Resizable = DataGridViewTriState.False
                Next

                'If the list of selected Tests have records then mark the selected Tests on the screen
                If (Not ListOfSelectedTestsAttribute Is Nothing) Then
                    'Verify OffSystems Tests previously selected
                    Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                    qSelectedTest = (From a In ListOfSelectedTestsAttribute.SelectedTestTable _
                                     Where a.TestType = "OFFS" _
                                     AndAlso a.SampleType = SampleTypeAttribute _
                                     Select a).ToList()

                    For Each selTestRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                        For Each offsystemTestROW As SelectedTestsDS.SelectedTestTableRow In offSystemTestList.SelectedTestTable.Rows
                            If (offsystemTestROW.TestID = selTestRow.TestID) Then
                                offsystemTestROW.Selected = True

                                'SG 13/03/2013
                                If (Not selTestRow.IsLISRequestNull AndAlso selTestRow.LISRequest) Then
                                    offsystemTestROW.OTStatus = "LISLOCK"
                                Else
                                    offsystemTestROW.OTStatus = selTestRow.OTStatus
                                End If

                                ' XB 14/01/2015 - BA-1867
                                'If (Not selTestRow.IsTestProfileIDNull) Then
                                '    offsystemTestROW.TestProfileID = selTestRow.TestProfileID
                                '    offsystemTestROW.TestProfileName = selTestRow.TestProfileName
                                'End If
                                
                                'For PATIENTS, set value of fields containing Test Profile and Calculated Tests information
                                If (SampleClassAttribute = "PATIENT") Then
                                    If (Not selTestRow.IsTestProfileIDNull) Then
                                        offsystemTestROW.TestProfileID = selTestRow.TestProfileID
                                        offsystemTestROW.TestProfileName = selTestRow.TestProfileName
                                    End If

                                    If (Not selTestRow.IsCalcTestIDsNull) Then
                                        offsystemTestROW.CalcTestIDs = selTestRow.CalcTestIDs
                                        offsystemTestROW.CalcTestNames = selTestRow.CalcTestNames
                                    End If
                                End If
                                ' XB 14/01/2015 - BA-1867

                                If (selTestRow.OTStatus = "OPEN" AndAlso Not selTestRow.LISRequest) Then
                                    'Change the Background Color and the Foreground Color to indicate that this Test is selected
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.BackColor = SELECTED_BACK_COLOR
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.SelectionBackColor = SELECTED_BACK_COLOR
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.ForeColor = SELECTED_FORE_COLOR
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.SelectionForeColor = SELECTED_FORE_COLOR
                                ElseIf (selTestRow.LISRequest) Then 'SGM 13/03/2013
                                    'Change the Background Color and the Foreground Color to indicate that this Test is requested by LIS
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.BackColor = LIS_REQUESTED_BACK_COLOR
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.SelectionBackColor = LIS_REQUESTED_BACK_COLOR
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.ForeColor = LIS_REQUESTED_FORE_COLOR
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.SelectionForeColor = LIS_REQUESTED_FORE_COLOR
                                Else
                                    'Change the Background Color and the Foreground Color to indicate that this Test is selected and cannot be unselected
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.BackColor = INPROCESS_BACK_COLOR
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.SelectionBackColor = INPROCESS_BACK_COLOR
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.ForeColor = INPROCESS_FORE_COLOR
                                    bsOffSystemTestDataGridView.Rows(offsystemTestROW.Row).Cells(offsystemTestROW.Col).Style.SelectionForeColor = INPROCESS_FORE_COLOR
                                End If
                                Exit For
                            End If
                        Next offsystemTestROW
                    Next selTestRow
                End If

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillAndMarkOffSystemTestListGridView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillAndMarkOffSystemTestListGridView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Mark a selected Profile on the TreeView 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 09/02/2010
    ''' Modified by: SA 02/11/2010 - Verify Tests having field TestProfileID informed for all Test Types,
    '''                              not only Standard Tests
    ''' </remarks>
    Private Sub MarkSelectedProfilesOnTreeView()
        Try
            If (Not ListOfSelectedTestsAttribute Is Nothing) Then
                Dim qTestProfiles As List(Of TestProfileTestsDS.tparTestProfileTestsRow)
                Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)

                'Get all selected TestProfiles from the list of selected Tests (whatever type) 
                Dim qSelectedTestDistinct As List(Of Integer)
                qSelectedTestDistinct = (From a In ListOfSelectedTestsAttribute.SelectedTestTable _
                                    Where Not a.IsTestProfileIDNull _
                                       Select a.TestProfileID Distinct).ToList()

                'Validate if there are any test profile selected
                For Each testProfileID As Integer In qSelectedTestDistinct
                    Dim mytestProfileID As Integer = testProfileID
                    'Get all Tests included in the Profile
                    qTestProfiles = (From a In testProfilesList.tparTestProfileTests _
                                    Where a.TestProfileID = mytestProfileID _
                                   Select a).ToList()

                    'Get all Selected Tests belonging to the Profile
                    qSelectedTest = (From b In ListOfSelectedTests.SelectedTestTable _
                                    Where (Not b.IsTestProfileIDNull AndAlso b.TestProfileID = mytestProfileID) _
                                   Select b).ToList()

                    If (qTestProfiles.Count = qSelectedTest.Count) Then
                        'Search the Profile Node to set the mark
                        If (bsProfilesTreeView.Nodes.Find(testProfileID.ToString(), False).Length <> 0) Then
                            Dim myTreeNode As New TreeNode()
                            myTreeNode = CType(bsProfilesTreeView.Nodes.Find(testProfileID.ToString(), True).First, TreeNode)
                            myTreeNode.Checked = True

                            For Each myNode As TreeNode In myTreeNode.Nodes
                                myNode.Checked = True
                            Next
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkSelectedProfilesOnTreeView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkSelectedProfilesOnTreeView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the structure to return with the list of selected Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA 04/05/2010 - Load also selected Calculated Tests in the list of selected elements that will be returned
    '''              DL 21/10/2010 - Load also selected ISE Tests in the list of selected elements that will be returned
    '''              DL 29/11/2010 - Load also selected Off-System Tests in the list of selected elements that will be returned
    '''              SA 08/05/2013 - If fields SampleID and SampleIDType are informed in the entry SelectedTestsDS, save them also 
    '''                              in the returned SelectedTestsDS (this functionality is required when this screen is used from 
    '''                              HQBarcode screen; in any other case, those fields are not informed)
    '''              TR 28/04/2014 - BT #1494 ==> For each STANDARD Test marked as selected, validate if it has a completed Calibration programming 
    '''                                           before add it to the final list of selected Tests. Besides, set value of screen attribute 
    '''                                           IncompleteTestAttribute to TRUE to notify the User that some of the selected Tests were removed 
    '''                                           due to they have an incomplete Calibration programming 
    '''              XB 02/12/2014 - Add functionality cases for ISE and OFFS tests included into a CALC test - BA-1867
    ''' </remarks>
    Private Sub AcceptTestSelection()
        Try
            'Hide the Form to avoid ugly downloading effects while in WS Preparation screen the grid is loaded
            Me.Opacity = 0

            'If fields SampleID and SampleIDType are informed in the entry SelectedTestsDS, save them also in the returned SelectedTestsDS
            Dim mySampleID As String = String.Empty
            Dim mySampleIDType As String = String.Empty
            If (Not ListOfSelectedTestsAttribute Is Nothing) Then
                If (ListOfSelectedTestsAttribute.SelectedTestTable.Rows.Count > 0) Then
                    If (Not ListOfSelectedTestsAttribute.SelectedTestTable.First.IsSampleIDNull AndAlso Not ListOfSelectedTestsAttribute.SelectedTestTable.First.IsSampleIDTypeNull) Then
                        mySampleID = ListOfSelectedTestsAttribute.SelectedTestTable.First.SampleID
                        mySampleIDType = ListOfSelectedTestsAttribute.SelectedTestTable.First.SampleIDType
                    End If
                End If
            End If

            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
            ListOfSelectedTestsAttribute = New SelectedTestsDS


            'BT #1494 - Get the list of Standard Tests that have been selected but have an incomplete Calibration programming to unselect them
            Dim qSelStdTests As List(Of SelectedTestsDS.SelectedTestTableRow)
            Dim qTestInFormula As List(Of FormulasDS.tparFormulasRow)
            Dim qSelCalcTests As List(Of SelectedTestsDS.SelectedTestTableRow)
            Dim lstCalculatedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
            Dim myCalTestName As String = String.Empty
            Dim belongCalcTest As Boolean = False 'Indicate the Test is included in the Formula of one or more Calculated Tests

            'Get the list of selected STD Tests having EnableStatus = False (those marked with incomplete Calibration programming)
            qSelStdTests = (From a As SelectedTestsDS.SelectedTestTableRow In standardTestList.SelectedTestTable _
                           Where a.Selected = True AndAlso a.EnableStatus = False _
                          Select a).ToList()

            If (qSelStdTests.Count > 0) Then
                For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelStdTests
                    belongCalcTest = False

                    If (Not selectedRow.IsCalcTestIDsNull) Then
                        'The STD Test is included in the Formula of one or more selected Calculated Tests
                        myCalTestName = selectedRow.CalcTestNames
                        belongCalcTest = True
                    End If

                    If (myCalTestName <> String.Empty) Then
                        'Unmark all Calculated Tests in which Formula the STD Test is included
                        For Each calcTestName As String In myCalTestName.Split(CChar(","))
                            lstCalculatedTest = (From b In calculatedTestList.SelectedTestTable _
                                                Where b.TestType = "CALC" AndAlso b.TestName = calcTestName _
                                               Select b).ToList

                            If (lstCalculatedTest.Count > 0) Then
                                MarkUnMarkCalculatedTestCell(lstCalculatedTest.First().Row, lstCalculatedTest.First().Col, True, lstCalculatedTest)
                            End If
                        Next
                    End If

                    'Set Selected = TRUE to allow unmark the STD Tests (needed because in the previous action it is possible than the Selected 
                    'value of this Test was set to FALSE)
                    If (belongCalcTest) Then selectedRow.Selected = True

                    'Finally, unmark the STD Test to remove it from the list of selected Tests
                    MarkUnMarkSelectedCell(selectedRow.Row, selectedRow.Col)
                Next

                'At least an STD Test with incomplete Calibration programming was selected and removed. Set to TRUE the property that allow the WS Samples 
                'Requests Screen to notify the final User that some of the chosen Tests cannot be selected 
                IncompleteTestAttribute = True
            Else
                IncompleteTestAttribute = False
            End If

            'Get the list of Standard Tests that have been selected...
            qSelStdTests = (From a As SelectedTestsDS.SelectedTestTableRow In standardTestList.SelectedTestTable _
                           Where a.Selected = True OrElse a.PartiallySelected = True _
                          Select a).ToList()

            'Add all selected Standard Tests to the DataSet to return
            For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelStdTests
                'Verify if it has been already included in the list of Selected Tests
                Dim myTestID As Integer = selectedRow.TestID
                Dim mySampleType As String = selectedRow.SampleType

                qSelectedTest = (From a As SelectedTestsDS.SelectedTestTableRow In ListOfSelectedTestsAttribute.SelectedTestTable _
                                Where a.TestType = "STD" _
                              AndAlso a.SampleType = mySampleType _
                              AndAlso a.TestID = myTestID _
                               Select a).ToList()

                If (qSelectedTest.Count = 0) Then
                    If (mySampleID <> String.Empty) Then
                        selectedRow.SampleID = mySampleID
                        selectedRow.SampleIDType = mySampleIDType
                    End If

                    'Add the Standard Test to the list of Selected Tests
                    ListOfSelectedTestsAttribute.SelectedTestTable.ImportRow(selectedRow)
                End If
            Next selectedRow

            ' XB 02/12/2014 - BA-1867
            'Get the list of ISE Tests that have been selected...
            Dim qSelISETests As List(Of SelectedTestsDS.SelectedTestTableRow)
            qSelISETests = (From a As SelectedTestsDS.SelectedTestTableRow In iseTestList.SelectedTestTable _
                           Where a.Selected = True OrElse a.PartiallySelected = True _
                          Select a).ToList()

            'Add all selected ISE Tests to the DataSet to return
            For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelISETests
                'Verify if it has been already included in the list of Selected Tests
                Dim myTestID As Integer = selectedRow.TestID
                Dim mySampleType As String = selectedRow.SampleType

                qSelectedTest = (From a As SelectedTestsDS.SelectedTestTableRow In ListOfSelectedTestsAttribute.SelectedTestTable _
                                Where a.TestType = "ISE" _
                              AndAlso a.SampleType = mySampleType _
                              AndAlso a.TestID = myTestID _
                               Select a).ToList()

                If (qSelectedTest.Count = 0) Then
                    If (mySampleID <> String.Empty) Then
                        selectedRow.SampleID = mySampleID
                        selectedRow.SampleIDType = mySampleIDType
                    End If

                    'Add the ISE Test to the list of Selected Tests
                    ListOfSelectedTestsAttribute.SelectedTestTable.ImportRow(selectedRow)
                End If
            Next selectedRow

            'Get the list of OffSystem Tests that have been selected...
            Dim qSelOffSystemTests As List(Of SelectedTestsDS.SelectedTestTableRow)
            qSelOffSystemTests = (From a As SelectedTestsDS.SelectedTestTableRow In offSystemTestList.SelectedTestTable _
                                 Where a.Selected = True OrElse a.PartiallySelected = True _
                                Select a).ToList()

            'Add all selected OffSystem Tests to the DataSet to return
            For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelOffSystemTests
                'Verify if it has been already included in the list of Selected Tests
                Dim myTestID As Integer = selectedRow.TestID
                Dim mySampleType As String = selectedRow.SampleType

                qSelectedTest = (From a As SelectedTestsDS.SelectedTestTableRow In ListOfSelectedTestsAttribute.SelectedTestTable _
                                Where a.TestType = "OFFS" _
                              AndAlso a.SampleType = mySampleType _
                              AndAlso a.TestID = myTestID _
                               Select a).ToList()

                If (qSelectedTest.Count = 0) Then
                    If (mySampleID <> String.Empty) Then
                        selectedRow.SampleID = mySampleID
                        selectedRow.SampleIDType = mySampleIDType
                    End If

                    'Add the OffSystem Test to the list of Selected Tests
                    ListOfSelectedTestsAttribute.SelectedTestTable.ImportRow(selectedRow)
                End If
            Next selectedRow
            ' XB 02/12/2014 - BA-1867

            'Get the list of Calculated Tests that have been selected...
            If (SampleClassAttribute = "PATIENT") Then
                'Dim qSelCalcTests As List(Of SelectedTestsDS.SelectedTestTableRow)
                qSelCalcTests = (From a As SelectedTestsDS.SelectedTestTableRow In calculatedTestList.SelectedTestTable _
                                Where a.Selected = True _
                               Select a).ToList()

                'Add all selected Calculated Tests to the DataSet to return
                For Each selCalcTestRow As SelectedTestsDS.SelectedTestTableRow In qSelCalcTests
                    If (mySampleID <> String.Empty) Then
                        selCalcTestRow.SampleID = mySampleID
                        selCalcTestRow.SampleIDType = mySampleIDType
                    End If
                    ListOfSelectedTestsAttribute.SelectedTestTable.ImportRow(selCalcTestRow)

                    'Get the list of Tests included in the selected Calculated Test but having a different SampleType
                    Dim currentCalcTestID As Integer = selCalcTestRow.TestID
                    Dim currentCalcTestName As String = selCalcTestRow.TestName

                    'Dim qTestInFormula As List(Of FormulasDS.tparFormulasRow)
                    qTestInFormula = (From a As FormulasDS.tparFormulasRow In calcTestComponents.tparFormulas _
                                     Where a.CalcTestID = currentCalcTestID _
                                   AndAlso a.SampleType <> SampleTypeAttribute _
                                    Select a).ToList()

                    For Each formulaTest As FormulasDS.tparFormulasRow In qTestInFormula
                        Dim myValue As String = formulaTest.Value
                        Dim myTestType As String = formulaTest.TestType
                        Dim mySampleType As String = formulaTest.SampleType

                        'Verify if it has been already included in the list of Selected Tests
                        qSelectedTest = (From a As SelectedTestsDS.SelectedTestTableRow In ListOfSelectedTestsAttribute.SelectedTestTable _
                                        Where a.TestType = myTestType _
                                      AndAlso a.SampleType = mySampleType _
                                      AndAlso a.TestID = Convert.ToInt32(myValue) _
                                       Select a).ToList()

                        If (qSelectedTest.Count = 0) Then
                            'Add the Test to the list of Selected Tests
                            Dim newTestRow As SelectedTestsDS.SelectedTestTableRow

                            newTestRow = ListOfSelectedTestsAttribute.SelectedTestTable.NewSelectedTestTableRow()
                            newTestRow.TestID = Convert.ToInt32(formulaTest.Value)
                            newTestRow.SampleType = formulaTest.SampleType
                            newTestRow.TestType = formulaTest.TestType
                            newTestRow.CalcTestIDs = currentCalcTestID.ToString
                            newTestRow.CalcTestNames = currentCalcTestName
                            newTestRow.OTStatus = "OPEN"
                            newTestRow.Selected = True
                            newTestRow.PartiallySelected = False

                            If (mySampleID <> String.Empty) Then
                                newTestRow.SampleID = mySampleID
                                newTestRow.SampleIDType = mySampleIDType
                            End If
                            ListOfSelectedTestsAttribute.SelectedTestTable.AddSelectedTestTableRow(newTestRow)
                        End If
                    Next formulaTest
                Next selCalcTestRow
            End If

            ' XB 02/12/2014 - BA-1867
            ''Get the list of ISE Tests that have been selected...
            ''Dim qSelISETests As New List(Of SelectedTestsDS.SelectedTestTableRow)
            'Dim qSelISETests As List(Of SelectedTestsDS.SelectedTestTableRow)
            'qSelISETests = (From a As SelectedTestsDS.SelectedTestTableRow In iseTestList.SelectedTestTable _
            '               Where a.Selected = True _
            '              Select a).ToList()

            ''Add all selected ISE Tests to the DataSet to return
            'For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelISETests
            '    'Verify if it has been already included in the list of Selected Tests
            '    Dim myTestID As Integer = selectedRow.TestID
            '    Dim mySampleType As String = selectedRow.SampleType

            '    qSelectedTest = (From a As SelectedTestsDS.SelectedTestTableRow In ListOfSelectedTestsAttribute.SelectedTestTable _
            '                    Where a.TestType = "ISE" _
            '                  AndAlso a.SampleType = mySampleType _
            '                  AndAlso a.TestID = myTestID _
            '                   Select a).ToList()

            '    If (qSelectedTest.Count = 0) Then
            '        If (mySampleID <> String.Empty) Then
            '            selectedRow.SampleID = mySampleID
            '            selectedRow.SampleIDType = mySampleIDType
            '        End If

            '        'Add the ISE Test to the list of Selected Tests
            '        ListOfSelectedTestsAttribute.SelectedTestTable.ImportRow(selectedRow)
            '    End If
            'Next selectedRow

            ''Get the list of OffSystem Tests that have been selected...
            'Dim qSelOffSystemTests As List(Of SelectedTestsDS.SelectedTestTableRow)
            'qSelOffSystemTests = (From a As SelectedTestsDS.SelectedTestTableRow In offSystemTestList.SelectedTestTable _
            '                     Where a.Selected = True _
            '                    Select a).ToList()

            ''Add all selected OffSystem Tests to the DataSet to return
            'For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelOffSystemTests
            '    'Verify if it has been already included in the list of Selected Tests
            '    Dim myTestID As Integer = selectedRow.TestID
            '    Dim mySampleType As String = selectedRow.SampleType

            '    qSelectedTest = (From a As SelectedTestsDS.SelectedTestTableRow In ListOfSelectedTestsAttribute.SelectedTestTable _
            '                    Where a.TestType = "OFFS" _
            '                  AndAlso a.SampleType = mySampleType _
            '                  AndAlso a.TestID = myTestID _
            '                   Select a).ToList()

            '    If (qSelectedTest.Count = 0) Then
            '        If (mySampleID <> String.Empty) Then
            '            selectedRow.SampleID = mySampleID
            '            selectedRow.SampleIDType = mySampleIDType
            '        End If

            '        'Add the OffSystem Test to the list of Selected Tests
            '        ListOfSelectedTestsAttribute.SelectedTestTable.ImportRow(selectedRow)
            '    End If
            'Next selectedRow
            ' XB 02/12/2014 - BA-1867

            'Close the screen
            Me.Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AcceptTestSelection", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AcceptTestSelection", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Verify if the maximum number of Patient Order Tests have been reached.  The maximum number is reached when:
    '''   (Number of Patient Order Tests in the correspondent grid + (Number of requested Patient Orders * Number of selected Tests)) > Total Number allowed
    ''' where the total number allowed is set in the General Setting MAX_PATIENT_ORDER_TESTS
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA
    ''' Modified by: SA 04/05/2010 - Count also the number of selected Calculated Tests
    '''              SA 22/10/2010 - Count also the number of selected ISE Tests
    '''              SA 02/12/2010 - Count also the number of selected Off-System Tests
    ''' </remarks>
    Private Function VerifyMaxNumberReached() As Boolean
        Dim maxNumberReached As Boolean = False
        Try
            If (SampleClassAttribute = "PATIENT") Then
                'Get total number of Tests currently selected
                Dim totalSelectedTests As Integer = 0

                'Count number of Standard Tests currently selected
                Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                Where a.Selected = True _
                               Select a).ToList()
                totalSelectedTests += qSelectedTest.Count

                'Count number of Calculated Tests currently selected
                qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                                Where a.Selected = True _
                               Select a).ToList()
                totalSelectedTests += qSelectedTest.Count

                'Count number of ISE Tests currently selected 
                qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                Where a.Selected = True _
                               Select a).ToList()
                totalSelectedTests += qSelectedTest.Count

                'Count number of Off-System Tests currently selected 
                qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                                Where a.Selected = True _
                               Select a).ToList()
                totalSelectedTests += qSelectedTest.Count

                'Verify if maximum number of Patient Order Tests have been reached when there is at least a selected Test
                If (totalSelectedTests > 0) Then
                    Dim total As Integer = (MaxValuesAttribute.MaxOrderTestsValues(0).CurrentRowsNumValue - selectTestList.SelectedTestTable.Rows.Count) + _
                                           (MaxValuesAttribute.MaxOrderTestsValues(0).CurrentNumOrdersValue * totalSelectedTests)
                    maxNumberReached = (total > MaxValuesAttribute.MaxOrderTestsValues(0).MaxRowsAllowed)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".VerifyMaxNumberReached", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".VerifyMaxNumberReached", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return maxNumberReached
    End Function

    ''' <summary>
    ''' Rebuild a String List by removind from it an specific value
    ''' </summary>
    ''' <param name="pSourceList">String list to rebuild by deleting from it the specified value</param>
    ''' <param name="pValueToDelete">Value to remove from the informed String List</param>
    ''' <returns>The entry String without the value specified to be deleted</returns>
    ''' <remarks>
    ''' Created by:  SA 11/05/2010
    ''' </remarks>
    Private Function RebuildStringList(ByVal pSourceList As String, ByVal pValueToDelete As String) As String
        Dim finalList As String = ""

        Try
            If (pSourceList.Trim <> "") Then
                Dim elements() As String = pSourceList.Trim.Split(CChar(", "))
                For i As Integer = 0 To elements.Count - 1
                    If (elements(i).Trim <> pValueToDelete) Then
                        If (finalList <> "") Then finalList &= ", "
                        finalList &= elements(i).Trim
                    End If
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RebuildStringList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RebuildStringList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return finalList
    End Function

    ''' <summary>
    ''' Manages the multi selection/unselection of Standard Tests when the shift key remains pressed
    ''' </summary>
    ''' <param name="pRow">Row of the clicked cell</param>
    ''' <param name="pCol">Column of the clicked cell</param>
    ''' <remarks>
    ''' Created by:  SA 22/07/2010
    ''' Modified by: TR 10/03/2011 - Added changes to validate which of the selected Standard Tests
    '''                              are still using Factory Calibration values
    '''              RH 23/05/2012 - Use StringBuilder for massive String concatenations; message for Factory Calibration 
    '''                              change is shown in an auxiliary screen instead of a MsgBox
    ''' </remarks>
    Private Sub MarkUnMarkSTDIntervals(ByVal pRow As Integer, ByVal pCol As Integer)
        Try
            'Verify if the Standard Test clicked has status OPEN and if it is currently selected or not
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
            qSelectedTest = (From a In standardTestList.SelectedTestTable _
                            Where a.Row = pRow _
                          AndAlso a.Col = pCol _
                          AndAlso a.OTStatus = "OPEN" _
                           Select a).ToList()

            Dim selectedState As Boolean = False

            If (qSelectedTest.Count = 1) Then
                selectedState = (qSelectedTest.First.Selected)

                If (lastRowClickedSTD = -1) Then
                    'Click in a Test when no one has been selected before, mark all Tests placed 
                    'previously in the grid
                    qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                    Where a.Row <= pRow _
                                  AndAlso a.OTStatus = "OPEN" _
                                  AndAlso a.Selected = selectedState _
                                   Select a).ToList()

                    For Each stdTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                        If (stdTest.Row = pRow AndAlso stdTest.Col > pCol) Then Exit For
                        MarkUnMarkSelectedCell(stdTest.Row, stdTest.Col)
                    Next
                Else
                    'There is an interval of Tests to mark/unmark, but only if the action in both Tests forming the 
                    'interval limits is the same (if the last clicked was selected the current clicked has to be also
                    'to select)
                    If (selectedState <> lastSelStatusSTD) Then
                        If (lastRowClickedSTD < pRow) Then
                            'Ascending row selection 
                            qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                            Where (a.Row >= lastRowClickedSTD And a.Row <= pRow) _
                                          AndAlso a.OTStatus = "OPEN" _
                                          AndAlso a.Selected = selectedState _
                                           Select a).ToList()

                            For Each stdTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (stdTest.Row = lastRowClickedSTD AndAlso stdTest.Col > lastColClickedSTD) OrElse _
                                   (stdTest.Row > lastRowClickedSTD AndAlso stdTest.Row < pRow) OrElse _
                                   (stdTest.Row = pRow AndAlso stdTest.Col <= pCol) Then
                                    MarkUnMarkSelectedCell(stdTest.Row, stdTest.Col)
                                End If
                            Next

                        ElseIf (lastRowClickedSTD > pRow) Then
                            'Descending row selection 
                            qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                            Where (a.Row >= pRow AndAlso a.Row <= lastRowClickedSTD) _
                                          AndAlso a.OTStatus = "OPEN" _
                                          AndAlso a.Selected = selectedState _
                                           Select a).ToList()

                            For Each stdTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (stdTest.Row = pRow AndAlso stdTest.Col >= pCol) OrElse _
                                   (stdTest.Row > pRow AndAlso stdTest.Row < lastRowClickedSTD) OrElse _
                                   (stdTest.Row = lastRowClickedSTD AndAlso stdTest.Col < lastColClickedSTD) Then
                                    MarkUnMarkSelectedCell(stdTest.Row, stdTest.Col)
                                End If
                            Next
                        Else
                            'Same row selecting
                            If (lastColClickedSTD < pCol) Then
                                'Ascending column selection
                                qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                                Where a.Row = pRow _
                                              AndAlso (a.Col > lastColClickedSTD AndAlso a.Col <= pCol) _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.Selected = selectedState _
                                               Select a).ToList()

                                For Each stdTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (stdTest.Col > lastColClickedSTD AndAlso stdTest.Col <= pCol) Then
                                        MarkUnMarkSelectedCell(stdTest.Row, stdTest.Col)
                                    End If
                                Next

                            ElseIf (lastColClickedSTD > pCol) Then
                                'Descending column selection
                                qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                                Where a.Row = pRow _
                                              AndAlso (a.Col >= pCol AndAlso a.Col < lastColClickedSTD) _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.Selected = selectedState _
                                               Select a).ToList()

                                For Each stdTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (stdTest.Col >= pCol AndAlso stdTest.Col < lastColClickedSTD) Then
                                        MarkUnMarkSelectedCell(stdTest.Row, stdTest.Col)
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If

            'Verify if the selected STD Tests are using the Factory Calibration values
            If (qSelectedTest.Count > 0) Then
                Dim testWithFactoryValues As New System.Text.StringBuilder()
                For Each mySelTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                    If (mySelTest.Selected AndAlso Not selectedState) Then testWithFactoryValues.Append(ValidateFactoryValues(mySelTest.Row, mySelTest.Col))
                Next

                If (testWithFactoryValues.Length > 0) Then
                    Using AuxForm As New IWSTestSelectionWarning()
                        AuxForm.Message = testWithFactoryValues.ToString()
                        AuxForm.ShowDialog()
                    End Using
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkSTDIntervals", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkSTDIntervals", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the multi selection/unselection of ISE Tests when the shift key remains pressed
    ''' </summary>
    ''' <param name="pRow">Row of the clicked cell</param>
    ''' <param name="pCol">Column of the clicked cell</param>
    ''' <remarks>
    ''' Created by:  DL 22/10/2010
    ''' </remarks>
    Private Sub MarkUnMarkISEIntervals(ByVal pRow As Integer, ByVal pCol As Integer)
        Try
            'Verify if the ISE Test clicked has status OPEN and if it is currently selected or not
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
            qSelectedTest = (From a In iseTestList.SelectedTestTable _
                            Where a.Row = pRow _
                          AndAlso a.Col = pCol _
                          AndAlso a.OTStatus = "OPEN" _
                           Select a).ToList()

            Dim selectedState As Boolean = False

            If (qSelectedTest.Count = 1) Then
                selectedState = (qSelectedTest.First.Selected)

                If (lastRowClickedISE = -1) Then
                    'Click in a Test when no one has been selected before, mark all Tests placed 
                    'previously in the grid
                    qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                    Where a.Row <= pRow _
                                  AndAlso a.OTStatus = "OPEN" _
                                  AndAlso a.Selected = selectedState _
                                   Select a).ToList()

                    For Each iseTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                        If (iseTest.Row = pRow AndAlso iseTest.Col > pCol) Then Exit For
                        MarkUnMarkISETestCell(iseTest.Row, iseTest.Col)
                    Next
                Else
                    'There is an interval of Tests to mark/unmark, but only if the action in both Tests forming the 
                    'interval limits is the same (if the last clicked was selected the current clicked has to be also
                    'to select)
                    If (selectedState <> lastSelStatusISE) Then
                        If (lastRowClickedISE < pRow) Then
                            'Ascending row selection 
                            qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                            Where (a.Row >= lastRowClickedISE AndAlso a.Row <= pRow) _
                                          AndAlso a.OTStatus = "OPEN" _
                                          AndAlso a.Selected = selectedState _
                                           Select a).ToList()

                            For Each iseTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (iseTest.Row = lastRowClickedISE AndAlso iseTest.Col > lastColClickedISE) OrElse _
                                   (iseTest.Row > lastRowClickedISE AndAlso iseTest.Row < pRow) OrElse _
                                   (iseTest.Row = pRow AndAlso iseTest.Col <= pCol) Then
                                    MarkUnMarkISETestCell(iseTest.Row, iseTest.Col)
                                End If
                            Next

                        ElseIf (lastRowClickedISE > pRow) Then
                            'Descending row selection 
                            qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                            Where (a.Row >= pRow AndAlso a.Row <= lastRowClickedISE) _
                                          AndAlso a.OTStatus = "OPEN" _
                                          AndAlso a.Selected = selectedState _
                                           Select a).ToList()

                            For Each iseTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (iseTest.Row = pRow AndAlso iseTest.Col >= pCol) OrElse _
                                   (iseTest.Row > pRow AndAlso iseTest.Row < lastRowClickedISE) OrElse _
                                   (iseTest.Row = lastRowClickedISE AndAlso iseTest.Col < lastColClickedISE) Then
                                    MarkUnMarkISETestCell(iseTest.Row, iseTest.Col)
                                End If
                            Next
                        Else
                            'Same row selecting
                            If (lastColClickedISE < pCol) Then
                                'Ascending column selection
                                qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                                Where a.Row = pRow _
                                              AndAlso (a.Col > lastColClickedISE AndAlso a.Col <= pCol) _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.Selected = selectedState _
                                               Select a).ToList()

                                For Each iseTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (iseTest.Col > lastColClickedISE AndAlso iseTest.Col <= pCol) Then
                                        MarkUnMarkISETestCell(iseTest.Row, iseTest.Col)
                                    End If
                                Next

                            ElseIf (lastColClickedISE > pCol) Then
                                'Descending column selection
                                qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                                Where a.Row = pRow _
                                              AndAlso (a.Col >= pCol AndAlso a.Col < lastColClickedISE) _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.Selected = selectedState _
                                               Select a).ToList()

                                For Each iseTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (iseTest.Col >= pCol AndAlso iseTest.Col < lastColClickedISE) Then
                                        MarkUnMarkISETestCell(iseTest.Row, iseTest.Col)
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkISEIntervals", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkISEIntervals", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the multi selection/unselection of OffSystem Tests when the shift key remains pressed
    ''' </summary>
    ''' <param name="pRow">Row of the clicked cell</param>
    ''' <param name="pCol">Column of the clicked cell</param>
    ''' <remarks>
    ''' Created by:  SA 02/12/2010
    ''' </remarks>
    Private Sub MarkUnMarkOffSystemIntervals(ByVal pRow As Integer, ByVal pCol As Integer)
        Try
            'Verify if the Off-System Test clicked has status OPEN and if it is currently selected or not
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)

            qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                            Where a.Row = pRow _
                          AndAlso a.Col = pCol _
                          AndAlso a.OTStatus = "OPEN" _
                           Select a).ToList()

            If (qSelectedTest.Count = 1) Then
                Dim selectedState As Boolean = (qSelectedTest.First.Selected)
                If (lastRowClickedOffSystem = -1) Then
                    'Click in a Test when no one has been selected before, mark all Tests placed 
                    'previously in the grid
                    qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                                    Where a.Row <= pRow _
                                  AndAlso a.OTStatus = "OPEN" _
                                  AndAlso a.Selected = selectedState _
                                   Select a).ToList()

                    For Each offSystemTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                        If (offSystemTest.Row = pRow AndAlso offSystemTest.Col > pCol) Then Exit For
                        MarkUnMarkOffSystemTestCell(offSystemTest.Row, offSystemTest.Col)
                    Next
                Else
                    'There is an interval of Tests to mark/unmark, but only if the action in both Tests forming the 
                    'interval limits is the same (if the last clicked was selected the current clicked has to be also
                    'to select)
                    If (selectedState <> lastSelStatusOffSystem) Then
                        If (lastRowClickedOffSystem < pRow) Then
                            'Ascending row selection 
                            qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                                            Where (a.Row >= lastRowClickedOffSystem AndAlso a.Row <= pRow) _
                                          AndAlso a.OTStatus = "OPEN" _
                                          AndAlso a.Selected = selectedState _
                                           Select a).ToList()

                            For Each offSystemTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (offSystemTest.Row = lastRowClickedOffSystem AndAlso offSystemTest.Col > lastColClickedOffSystem) OrElse _
                                   (offSystemTest.Row > lastRowClickedOffSystem AndAlso offSystemTest.Row < pRow) OrElse _
                                   (offSystemTest.Row = pRow AndAlso offSystemTest.Col <= pCol) Then
                                    MarkUnMarkOffSystemTestCell(offSystemTest.Row, offSystemTest.Col)
                                End If
                            Next

                        ElseIf (lastRowClickedOffSystem > pRow) Then
                            'Descending row selection 
                            qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                                            Where (a.Row >= pRow AndAlso a.Row <= lastRowClickedOffSystem) _
                                          AndAlso a.OTStatus = "OPEN" _
                                          AndAlso a.Selected = selectedState _
                                           Select a).ToList()

                            For Each offSystemTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (offSystemTest.Row = pRow AndAlso offSystemTest.Col >= pCol) OrElse _
                                   (offSystemTest.Row > pRow AndAlso offSystemTest.Row < lastRowClickedOffSystem) OrElse _
                                   (offSystemTest.Row = lastRowClickedOffSystem AndAlso offSystemTest.Col < lastColClickedOffSystem) Then
                                    MarkUnMarkOffSystemTestCell(offSystemTest.Row, offSystemTest.Col)
                                End If
                            Next
                        Else
                            'Same row selecting
                            If (lastColClickedOffSystem < pCol) Then
                                'Ascending column selection
                                qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                                                Where a.Row = pRow _
                                              AndAlso (a.Col > lastColClickedOffSystem AndAlso a.Col <= pCol) _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.Selected = selectedState _
                                               Select a).ToList()

                                For Each OffSystemTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (OffSystemTest.Col > lastColClickedOffSystem AndAlso OffSystemTest.Col <= pCol) Then
                                        MarkUnMarkOffSystemTestCell(OffSystemTest.Row, OffSystemTest.Col)
                                    End If
                                Next

                            ElseIf (lastColClickedOffSystem > pCol) Then
                                'Descending column selection
                                qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                                                Where a.Row = pRow _
                                              AndAlso (a.Col >= pCol AndAlso a.Col < lastColClickedOffSystem) _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.Selected = selectedState _
                                               Select a).ToList()

                                For Each OffSystemTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (OffSystemTest.Col >= pCol AndAlso OffSystemTest.Col < lastColClickedOffSystem) Then
                                        MarkUnMarkOffSystemTestCell(OffSystemTest.Row, OffSystemTest.Col)
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkOffSystemIntervals", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkOffSystemIntervals", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the multi selection/unselection of Calculated Tests when the shift key remains pressed
    ''' </summary>
    ''' <param name="pRow">Row of the clicked cell</param>
    ''' <param name="pCol">Column of the clicked cell</param>
    ''' <remarks>
    ''' Created by:  SA 22/07/2010 - Function is done, but the event is deactivated due to some problems when
    '''                              mark/unmark this grid. Grid has few elements, then it is not possible 
    '''                              multiselect in it now
    ''' Modified by: RH 24/05/2012 - Use StringBuilder for massive String concatenations; message for Factory Calibration 
    '''                              change is shown in an auxiliary screen instead of a MsgBox
    '''                            - Changed calls to function MarkUnMarkCalculatedTestCell due to this has additional parameters
    ''' </remarks>
    Private Sub MarkUnMarkCALCIntervals(ByVal pRow As Integer, ByVal pCol As Integer)
        Try
            Dim myTestToValidate As New List(Of SelectedTestsDS.SelectedTestTableRow) 'RH 24/05/2012

            'Verify if the Calculated Test clicked has status OPEN and if it is currently selected or not
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)

            qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                            Where a.Row = pRow _
                          AndAlso a.Col = pCol _
                          AndAlso a.OTStatus = "OPEN" _
                           Select a).ToList()

            Dim selectedState As Boolean = False

            If (qSelectedTest.Count = 1) Then
                selectedState = (qSelectedTest.First.Selected)

                If (lastRowClickedCALC = -1) Then
                    'Click in a Test when no one has been selected before, mark all Tests placed 
                    'previously in the grid
                    qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                                    Where a.Row <= pRow _
                                  AndAlso a.OTStatus = "OPEN" _
                                  AndAlso a.Selected = selectedState _
                                   Select a).ToList()

                    For Each calcTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                        If (calcTest.Row = pRow AndAlso calcTest.Col > pCol) Then Exit For
                        MarkUnMarkCalculatedTestCell(calcTest.Row, calcTest.Col, True, myTestToValidate)
                    Next
                Else
                    'There is an interval of Tests to mark/unmark, but only if the action in both Tests forming the 
                    'interval limits is the same (if the last clicked was selected the current clicked has to be also
                    'to select)
                    If (selectedState <> lastSelStatusCALC) Then
                        If (lastRowClickedCALC < pRow) Then
                            'Ascending row selection 
                            qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                                            Where (a.Row >= lastRowClickedCALC And a.Row <= pRow) _
                                          AndAlso a.OTStatus = "OPEN" _
                                          AndAlso a.Selected = selectedState _
                                           Select a).ToList()

                            For Each calcTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (calcTest.Row = lastRowClickedCALC AndAlso calcTest.Col > lastColClickedCALC) OrElse _
                                   (calcTest.Row > lastRowClickedCALC AndAlso calcTest.Row < pRow) OrElse _
                                   (calcTest.Row = pRow AndAlso calcTest.Col <= pCol) Then
                                    MarkUnMarkCalculatedTestCell(calcTest.Row, calcTest.Col, True, myTestToValidate)
                                End If
                            Next

                        ElseIf (lastRowClickedCALC > pRow) Then
                            'Descending row selection 
                            qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                                            Where (a.Row >= pRow AndAlso a.Row <= lastRowClickedCALC) _
                                          AndAlso a.OTStatus = "OPEN" _
                                          AndAlso a.Selected = selectedState _
                                           Select a).ToList()

                            For Each calcTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (calcTest.Row = pRow AndAlso calcTest.Col >= pCol) OrElse _
                                   (calcTest.Row > pRow AndAlso calcTest.Row < lastRowClickedCALC) OrElse _
                                   (calcTest.Row = lastRowClickedCALC AndAlso calcTest.Col < lastColClickedCALC) Then
                                    MarkUnMarkCalculatedTestCell(calcTest.Row, calcTest.Col, True, myTestToValidate)
                                End If
                            Next
                        Else
                            'Same row selecting
                            If (lastColClickedSTD < pCol) Then
                                'Ascending column selection
                                qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                                                Where a.Row = pRow _
                                              AndAlso (a.Col > lastColClickedCALC AndAlso a.Col <= pCol) _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.Selected = selectedState _
                                               Select a).ToList()

                                For Each calcTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (calcTest.Col > lastColClickedCALC AndAlso calcTest.Col <= pCol) Then
                                        MarkUnMarkCalculatedTestCell(calcTest.Row, calcTest.Col, True, myTestToValidate)
                                    End If
                                Next

                            ElseIf (lastColClickedSTD > pCol) Then
                                'Descenting column selection
                                qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                                                Where a.Row = pRow _
                                              AndAlso (a.Col >= pCol AndAlso a.Col < lastColClickedCALC) _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.Selected = selectedState _
                                               Select a).ToList()

                                For Each calcTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (calcTest.Col >= pCol AndAlso calcTest.Col < lastColClickedCALC) Then
                                        MarkUnMarkCalculatedTestCell(calcTest.Row, calcTest.Col, True, myTestToValidate)
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If

                'Verify if the selected STD Tests are using the Factory Calibration values
                If (myTestToValidate.Count > 0) Then
                    Dim testWithFactoryValues As New System.Text.StringBuilder()
                    For Each mySelTest As SelectedTestsDS.SelectedTestTableRow In myTestToValidate
                        If (mySelTest.Selected AndAlso Not selectedState) Then testWithFactoryValues.Append(ValidateFactoryValues(mySelTest.Row, mySelTest.Col))
                    Next

                    If (testWithFactoryValues.Length > 0) Then
                        Using AuxForm As New IWSTestSelectionWarning()
                            AuxForm.Message = testWithFactoryValues.ToString()
                            AuxForm.ShowDialog()
                        End Using
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkCALCIntervals", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkCALCIntervals", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by:  PG 13/10/2010 
    ''' Modified by: SA 22/10/2010 - Get multilanguage text for label of the ISE Tests grid 
    '''              SG 14/03/2013 - Get multilanguage text for label of new colour for LIS Requested Tests in Legend area
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons...
            bsTestSelectionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestsSelection", pLanguageID)

            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID) + ":"
            bsPatientLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientSample", pLanguageID) + ":"
            bsTestProfilesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_Profiles", pLanguageID) + ":"

            bsStandardTestsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_StandardTests", pLanguageID) + ":"
            bsCalcTestsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_Long", pLanguageID) + ":"
            bsISETestsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_ISETests", pLanguageID) + ":"
            bsOffSystemTestsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_OffSystemTests", pLanguageID) + ":"

            bsLegendLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Legend", pLanguageID)
            bsSelectedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_Selected", pLanguageID)
            bsDeletedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_DelSelTest", pLanguageID)
            bsInProcessLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_InProcess", pLanguageID)
            bsDifPriorityLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_DifPriority", pLanguageID)
            bsPartialSelectedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_PartialControl", pLanguageID)
            bsUnselectedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_Unselected", pLanguageID)
            bsLISRequestedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Requested_By_LIS", pLanguageID)

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AcceptSelection", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CancelSelection", pLanguageID))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the selected STD Test has Factory Calibration values
    ''' </summary>
    ''' <returns>True if the selected STD Test has Factory Calibration values; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  TR 10/03/2011
    ''' Modified by: DL 09/04/2011 - Added optional parameter to filter the LINQ by SampleType when the screen is used from Controls Programming Screen 
    '''              RH 23/05/2012 - Use StringBuilder for massive String concatenations
    ''' </remarks>
    Private Function ValidateFactoryValues(ByVal pRowIndex As Integer, ByVal pColumnIndex As Integer, Optional ByVal pSampleType As String = "") As String
        Dim myResult As New System.Text.StringBuilder()

        Try
            Dim qTestWithFactValue As List(Of SelectedTestsDS.SelectedTestTableRow)
            If (pSampleType <> "") Then
                qTestWithFactValue = (From a In standardTestList.SelectedTestTable _
                                     Where a.Row = pRowIndex _
                                   AndAlso a.Col = pColumnIndex _
                                   AndAlso a.SampleType = pSampleType _
                                   AndAlso a.FactoryCalib _
                                   AndAlso a.Selected _
                                    Select a).ToList()
            Else
                qTestWithFactValue = (From a In standardTestList.SelectedTestTable _
                                     Where a.Row = pRowIndex _
                                   AndAlso a.Col = pColumnIndex _
                                   AndAlso a.FactoryCalib _
                                   AndAlso a.Selected _
                                    Select a).ToList()
            End If

            For Each selectedTest As SelectedTestsDS.SelectedTestTableRow In qTestWithFactValue
                myResult.AppendFormat("● {0}{1}", selectedTest.TestName, vbCrLf)
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateFactoryValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateFactoryValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult.ToString()
    End Function

    Private Sub ReleaseElements()

        Try
            '--- Detach variable defined using WithEvents ---
            bsAcceptButton = Nothing
            bsAdviceGroupBox = Nothing
            bsWarningMessageLabel = Nothing
            bsWarningIconPictureBox = Nothing
            bsTestSelectionLabel = Nothing
            bsSampleTypeTextBox = Nothing
            bsSampleTypeLabel = Nothing
            bsTestProfilesLabel = Nothing
            bsCalcTestsLabel = Nothing
            bsStandardTestsLabel = Nothing
            bsTestListDataGridView = Nothing
            bsCalcTestDataGridView = Nothing
            bsProfilesTreeView = Nothing
            bsPatientTextBox = Nothing
            bsPatientLabel = Nothing
            bsTestSelectionAreaGroupBox = Nothing
            bsLegendGroupBox = Nothing
            bsPartialSelectedLabel = Nothing
            bsDifPriorityLabel = Nothing
            bsInProcessLabel = Nothing
            bsDeletedLabel = Nothing
            bsUnselectedLabel = Nothing
            bsSelectedLabel = Nothing
            bsDifPriorityButton = Nothing
            bsSelectButton = Nothing
            bsPartialSelectedButton = Nothing
            bsUnselectedButton = Nothing
            bsDeletedButton = Nothing
            bsInProcessButton = Nothing
            bsScreenToolTips = Nothing
            bsCancelButton = Nothing
            bsISETestDataGridView = Nothing
            bsISETestsLabel = Nothing
            bsOffSystemTestDataGridView = Nothing
            bsOffSystemTestsLabel = Nothing
            bsLegendLabel = Nothing
            bsLegendModel2GroupBox = Nothing
            bsLegendModel2Label = Nothing
            bsUnselectedModel2Label = Nothing
            bsSelectedInUseModel2Label = Nothing
            bsDeletedModel2Label = Nothing
            bsSelectedModel2Label = Nothing
            bsSelectModel2Button = Nothing
            bsUnselectedModel2Button = Nothing
            bsDeletedModel2Button = Nothing
            bsSelectedInUseModel2Button = Nothing
            bsSampleTypeComboBox = Nothing
            Column1 = Nothing
            Column2 = Nothing
            Column3 = Nothing
            Column4 = Nothing
            Column5 = Nothing
            DataGridViewButtonColumn9 = Nothing
            DataGridViewButtonColumn10 = Nothing
            DataGridViewButtonColumn11 = Nothing
            DataGridViewButtonColumn12 = Nothing
            DataGridViewButtonColumn13 = Nothing
            DataGridViewButtonColumn1 = Nothing
            DataGridViewButtonColumn2 = Nothing
            DataGridViewButtonColumn3 = Nothing
            DataGridViewButtonColumn4 = Nothing
            DataGridViewButtonColumn14 = Nothing
            DataGridViewButtonColumn5 = Nothing
            DataGridViewButtonColumn6 = Nothing
            DataGridViewButtonColumn7 = Nothing
            DataGridViewButtonColumn8 = Nothing
            DataGridViewButtonColumn15 = Nothing
            bsLISRequestedLabel = Nothing
            bsLISRequestedButton = Nothing
            '-----------------------------------------------
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

#End Region

#Region "Methods for Second Screen Model"
    ''' <summary>
    ''' Get texts in the current application language for all screen controls (when the application is open
    ''' from the screen of Controls Programming)
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  DL 06/04/2011
    ''' Modified by: SA 10/05/2011 - Get labels also for button tooltips
    ''' </remarks>
    Private Sub GetScreenLabelsModel2(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsTestSelectionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestsSelection", pLanguageID)
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID) + ":"
            bsStandardTestsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_StandardTests", pLanguageID) + ":"

            'For Labels, CheckBox, RadioButtons...
            bsLegendModel2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Legend", pLanguageID)
            bsSelectedModel2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_Selected", pLanguageID)
            bsUnselectedModel2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_Unselected", pLanguageID)
            bsDeletedModel2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_DelSelTest", pLanguageID)
            bsSelectedInUseModel2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_REAGENT_POS_STATUS_INUSE", pLanguageID)

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AcceptSelection", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CancelSelection", pLanguageID))

            'RH 13/06/2012
            bsISETestsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_ISETests", pLanguageID) + ":"

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabelsModel2", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabelsModel2", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization when the screen is opened from the screen of Controls Programming
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 06/04/2011
    ''' Modified by: SA 10/05/2011 - Some code improvements
    '''              RH 12/03/2012 - When an error happens, execute Return to prevent the execution of the following lines
    ''' </remarks>
    Private Sub InitializeScreenModel2()
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Get Multilanguage Texts for all Screen Controls
            GetScreenLabelsModel2(currentLanguage)

            'Get Icons for Screen Buttons
            PrepareButtons()

            'Hide fields visible only in Standard Mode
            bsSampleTypeTextBox.Visible = False
            bsPatientLabel.Visible = False
            bsPatientTextBox.Visible = False
            bsTestProfilesLabel.Visible = False
            bsProfilesTreeView.Visible = False
            bsLegendGroupBox.Visible = False
            bsCalcTestsLabel.Visible = False
            bsCalcTestDataGridView.Visible = False

            bsISETestsLabel.Top = bsCalcTestsLabel.Top - 24
            bsISETestDataGridView.Top = bsCalcTestDataGridView.Top - 24
            'bsISETestsLabel.Visible = False
            'bsISETestDataGridView.Visible = False

            bsOffSystemTestsLabel.Visible = False
            bsOffSystemTestDataGridView.Visible = False
            bsAdviceGroupBox.Visible = False

            'Show fields visible only in this mode
            bsSampleTypeComboBox.Visible = True
            bsLegendModel2GroupBox.Visible = True

            'Change size of screen and controls
            Me.Size = New Size(Me.Width, 490)

            bsTestSelectionAreaGroupBox.Size = New Size(bsTestSelectionAreaGroupBox.Width, 405)

            bsLegendModel2GroupBox.Location = New Point(bsLegendModel2GroupBox.Left, bsTestListDataGridView.Top - 8)
            bsTestListDataGridView.Height = bsTestListDataGridView.Height - 24

            'Get the list of all Tests with QCActive = 1. If a control is in edition (ControlIDAttribute <> -1),
            'all Tests linked to it have to be returned, including those with QCActive = 0
            Dim resultData As GlobalDataTO
            Dim myTestsDelegate As New TestsDelegate()
            Dim myMasterData As New MasterDataDelegate
            Dim qDistinctSampleType As New List(Of String)

            resultData = myTestsDelegate.GetForControlsProgramming(Nothing, ControlIDAttribute)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myTestDS As TestsDS = DirectCast(resultData.SetDatos, TestsDS)

                'Get the list of different Sample Types 
                qDistinctSampleType = (From a In myTestDS.tparTests _
                                     Select a.SampleType Distinct).ToList()

                'Load all Tests using QC in the DS of Standard Tests, show the ones using the selected SampleType,
                'and mark as selected those that have been informed from the previous screen
                FillStandardTests(myTestDS)

                'Save in a DS the Tests originally linked to the Control
                Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                 Where a.Selected = True _
                                 Select a).ToList()

                selectTestList = New SelectedTestsDS
                For Each mySelectedRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                    selectTestList.SelectedTestTable.ImportRow(mySelectedRow)
                Next
            End If

            'Get the list of available ISE Tests...
            If (Not resultData.HasError) Then
                Dim myISETestDelegate As New ISETestsDelegate

                resultData = myISETestDelegate.GetForControlsProgramming(Nothing, ControlIDAttribute)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myISETestsDS As ISETestsDS = DirectCast(resultData.SetDatos, ISETestsDS)

                    If (myISETestsDS.tparISETests.Rows.Count > 0) Then
                        'Get the list of different Sample Types and add them to the list of different Sample Types for STD Tests
                        qDistinctSampleType.AddRange((From a In myISETestsDS.tparISETests _
                                                      Select a.SampleType Distinct).ToList())

                        'Remove duplicated values
                        qDistinctSampleType = (From a In qDistinctSampleType _
                                             Select a Distinct).ToList()

                        'Load the ISE Tests in bsISETestDataGridView
                        FillISETests(myISETestsDS)

                        Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                        qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                         Where a.Selected _
                                         Select a).ToList()

                        'Add all selected ISE Tests to the DataSet 
                        For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                            selectTestList.SelectedTestTable.ImportRow(selectedRow)
                        Next
                    End If
                End If
            End If

            If (Not resultData.HasError) Then
                If (qDistinctSampleType.Count > 0) Then
                    'Get the list of all available Sample Types with the multilanguage description
                    resultData = myMasterData.GetList(Nothing, MasterDataEnum.SAMPLE_TYPES.ToString)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim mySampleTypes As MasterDataDS = DirectCast(resultData.SetDatos, MasterDataDS)

                        'Remove from the list all SampleTypes that do not have Tests with QCActive 
                        Dim myNewSampleTypes As New MasterDataDS

                        For Each qcSampleType As String In qDistinctSampleType
                            'Search the SampleType in the list that contains all the available SampleTypes
                            Dim sampleTypeInList As List(Of MasterDataDS.tcfgMasterDataRow)
                            sampleTypeInList = (From b As MasterDataDS.tcfgMasterDataRow In mySampleTypes.tcfgMasterData _
                                               Where b.ItemID = qcSampleType _
                                              Select b).ToList()

                            If (sampleTypeInList.Count = 1) Then myNewSampleTypes.tcfgMasterData.ImportRow(sampleTypeInList.First)
                        Next
                        myNewSampleTypes.AcceptChanges()

                        If (myNewSampleTypes.tcfgMasterData.Rows.Count > 0) Then
                            'Fill ComboBox of Sample Types
                            bsSampleTypeComboBox.DataSource = myNewSampleTypes.tcfgMasterData
                            bsSampleTypeComboBox.DisplayMember = "ItemIDDesc"
                            bsSampleTypeComboBox.ValueMember = "ItemID"
                        End If

                        If (SampleTypeAttribute = String.Empty) Then
                            'If a Sample Type has not been selected for the Control, select the first SampleType in the list
                            bsSampleTypeComboBox.SelectedIndex = 0
                            SampleTypeAttribute = bsSampleTypeComboBox.SelectedValue.ToString()
                        Else
                            bsSampleTypeComboBox.SelectedValue = SampleTypeAttribute

                            'If the SampleType selected for the Control is not loaded in the ComboBox, select the first one
                            If (bsSampleTypeComboBox.SelectedValue Is Nothing) Then
                                bsSampleTypeComboBox.SelectedIndex = 0
                                SampleTypeAttribute = bsSampleTypeComboBox.SelectedValue.ToString()
                            End If
                        End If

                        LoadTestsBySampleType(bsSampleTypeComboBox.SelectedValue.ToString())
                        LoadISETestsBySampleType(bsSampleTypeComboBox.SelectedValue.ToString())
                    End If
                Else
                    'There are not Tests/SampleTypes with QC active; the screen cannot be opened
                    ShowMessage(Me.Name & ".InitializeScreenModel2", GlobalEnumerates.Messages.NO_TESTS_WITH_QC_ACTIVE.ToString(), , Me)
                    Me.Close()
                    Return
                End If

            End If

            'Set the opacity to 100 to make visible the form.
            Me.Opacity = 100

            'If an error has happened getting the list of Tests or the list of available Sample Types, show it and close the screen
            If (resultData.HasError) Then
                ShowMessage(Me.Name & ".InitializeScreenModel2", resultData.ErrorCode, resultData.ErrorMessage)
                Me.Close()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScreenModel2", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreenModel2", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Load the list of Standard Tests to shown and assign the status to each one of them
    ''' </summary>
    ''' <param name="pTestsDS">Typed DataSet containing all Tests that have to be shown</param>
    ''' <remarks>
    ''' Created by:  DL 06/04/2011
    ''' Modified by: SA 10/05/2011 - Removed use of status STATLOCK; when a Test linked to the Control is InUse (but the
    '''                              Control is not), it is shown with status OPEN. Inform also field ShortName in the DS 
    '''                              loaded. Some code improvements 
    ''' </remarks>
    Private Sub FillStandardTests(ByVal pTestsDS As TestsDS)
        Try
            Dim maxSTDTests As Integer

            'Get the list of different SampleTypes 
            Dim qDistinctSampleType As List(Of String) = (From a In pTestsDS.tparTests _
                                                        Select a.SampleType).Distinct.ToList
            For Each mySampleType As String In qDistinctSampleType
                'Get the list of Tests to shown for each different Sample Type
                Dim qStandardTests As List(Of TestsDS.tparTestsRow) = (From b In pTestsDS.tparTests _
                                                                      Where b.SampleType = mySampleType _
                                                                     Select b).ToList()

                maxSTDTests = qStandardTests.Count
                If (maxSTDTests > 0) Then
                    'Set the number of rows required for the GridView for this SampleType
                    Dim numGridRows As Integer = (maxSTDTests \ COLUMN_COUNT)
                    If (maxSTDTests Mod COLUMN_COUNT) = 0 Then numGridRows -= 1

                    Dim testPosition As Integer = 0
                    Dim standarPos As Integer = 0
                    Dim newTestRow As SelectedTestsDS.SelectedTestTableRow

                    For j As Integer = 0 To numGridRows
                        For k As Integer = 0 To COLUMN_COUNT - 1
                            testPosition = (COLUMN_COUNT * j) + k
                            If (testPosition >= maxSTDTests) Then Exit For

                            newTestRow = standardTestList.SelectedTestTable.NewSelectedTestTableRow
                            newTestRow.Row = j
                            newTestRow.Col = k
                            newTestRow.SampleType = mySampleType
                            newTestRow.TestType = "STD"
                            newTestRow.TestID = qStandardTests(standarPos).TestID
                            newTestRow.TestName = qStandardTests(standarPos).TestName
                            newTestRow.ShortName = qStandardTests(standarPos).ShortName
                            newTestRow.ActiveControl = qStandardTests(standarPos).ActiveControl
                            newTestRow.RejectionCriteria = qStandardTests(standarPos).RejectionCriteria
                            newTestRow.DecimalsAllowed = CShort(qStandardTests(standarPos).DecimalsAllowed)
                            newTestRow.MeasureUnit = qStandardTests(standarPos).MeasureUnit
                            newTestRow.PreloadedTest = qStandardTests(standarPos).PreloadedTest
                            newTestRow.TestPosition = qStandardTests(standarPos).TestPosition
                            newTestRow.InUse = qStandardTests(standarPos).InUse
                            newTestRow.OTStatus = CStr(IIf(qStandardTests(standarPos).InUse, "INPROCESS", "OPEN"))
                            newTestRow.Selected = False

                            If (qStandardTests(standarPos).IsFactoryCalibNull) Then
                                newTestRow.FactoryCalib = False
                            Else
                                newTestRow.FactoryCalib = qStandardTests(standarPos).FactoryCalib
                            End If
                            standardTestList.SelectedTestTable.Rows.Add(newTestRow)

                            'Increase the next position for the list of standard Tests
                            standarPos += 1
                        Next k
                    Next j

                    'If the list of selected Tests have records then mark the selected Tests on the screen
                    If (Not ListOfSelectedTestsAttribute Is Nothing) Then
                        Dim qSelectedTest As New List(Of SelectedTestsDS.SelectedTestTableRow)
                        qSelectedTest = (From a In ListOfSelectedTestsAttribute.SelectedTestTable _
                                         Where a.TestType = "STD" And a.SampleType = mySampleType _
                                         Select a).ToList()

                        For Each selTestRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                            Dim myCurrentTest As Integer = selTestRow.TestID

                            Dim qSearchTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                            qSearchTest = (From a In standardTestList.SelectedTestTable _
                                           Where a.TestID = myCurrentTest And a.SampleType = mySampleType _
                                           Select a).ToList()

                            If (qSearchTest.Count = 1) Then
                                qSearchTest.First.Selected = True
                                If (qSearchTest.First.OTStatus = "INPROCESS") Then qSearchTest.First.OTStatus = "OPEN"
                            End If
                        Next
                    End If
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillStandardTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillStandardTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the list of ISE Tests to show and assigns the status to each one of them
    ''' </summary>
    ''' <param name="pISETestsDS">Typed DataSet containing all ISE Tests that have to be shown</param>
    ''' <remarks>
    ''' Created by: RH 19/06/2012
    ''' </remarks>
    Private Sub FillISETests(ByVal pISETestsDS As ISETestsDS)
        Try
            Dim maxTests As Integer

            'Get the list of different SampleTypes 
            Dim qDistinctSampleType As List(Of String) = (From a In pISETestsDS.tparISETests _
                                                        Select a.SampleType Distinct).ToList()

            For Each mySampleType As String In qDistinctSampleType
                'Get the list of Tests to show for each different Sample Type
                Dim qISETests As List(Of ISETestsDS.tparISETestsRow) = (From b In pISETestsDS.tparISETests _
                                                                       Where b.SampleType = mySampleType _
                                                                      Select b).ToList()

                maxTests = qISETests.Count
                If (maxTests > 0) Then
                    'Set the number of rows required for the GridView for this SampleType
                    Dim numGridRows As Integer = (maxTests \ COLUMN_COUNT)
                    If (maxTests Mod COLUMN_COUNT) = 0 Then numGridRows -= 1

                    Dim testPosition As Integer = 0
                    Dim isePos As Integer = 0
                    Dim newTestRow As SelectedTestsDS.SelectedTestTableRow

                    For j As Integer = 0 To numGridRows
                        For k As Integer = 0 To COLUMN_COUNT - 1
                            testPosition = (COLUMN_COUNT * j) + k
                            If (testPosition >= maxTests) Then Exit For

                            newTestRow = iseTestList.SelectedTestTable.NewSelectedTestTableRow
                            newTestRow.Row = j
                            newTestRow.Col = k
                            newTestRow.SampleType = mySampleType
                            newTestRow.TestType = "ISE"
                            newTestRow.TestID = qISETests(isePos).ISETestID
                            newTestRow.TestName = qISETests(isePos).Name
                            newTestRow.ShortName = qISETests(isePos).ShortName
                            newTestRow.ActiveControl = qISETests(isePos).ActiveControl
                            newTestRow.RejectionCriteria = qISETests(isePos).RejectionCriteria
                            newTestRow.DecimalsAllowed = CShort(qISETests(isePos).Decimals)
                            newTestRow.MeasureUnit = qISETests(isePos).Units
                            newTestRow.PreloadedTest = True
                            newTestRow.TestPosition = qISETests(isePos).TestPosition
                            newTestRow.InUse = qISETests(isePos).InUse
                            newTestRow.OTStatus = CStr(IIf(qISETests(isePos).InUse, "INPROCESS", "OPEN"))
                            newTestRow.Selected = False
                            newTestRow.FactoryCalib = False
                            iseTestList.SelectedTestTable.Rows.Add(newTestRow)

                            'Increase the next position for the list of standard Tests
                            isePos += 1
                        Next k
                    Next j

                    'If the list of selected Tests have records then mark the selected Tests on the screen
                    If (Not ListOfSelectedTestsAttribute Is Nothing) Then
                        Dim qSelectedTest As New List(Of SelectedTestsDS.SelectedTestTableRow)
                        qSelectedTest = (From a In ListOfSelectedTestsAttribute.SelectedTestTable _
                                         Where a.TestType = "ISE" AndAlso a.SampleType = mySampleType _
                                         Select a).ToList()

                        For Each selTestRow As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                            Dim myCurrentTest As Integer = selTestRow.TestID

                            Dim qSearchTest As List(Of SelectedTestsDS.SelectedTestTableRow)
                            qSearchTest = (From a In iseTestList.SelectedTestTable _
                                           Where a.TestID = myCurrentTest AndAlso a.SampleType = mySampleType _
                                           Select a).ToList()

                            If (qSearchTest.Count = 1) Then
                                qSearchTest.First.Selected = True
                                If (qSearchTest.First.OTStatus = "INPROCESS") Then qSearchTest.First.OTStatus = "OPEN"
                            End If
                        Next
                    End If
                End If
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillISETests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillISETests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the grid with all Tests for the Sample Type selected in the ComboBox
    ''' </summary>
    ''' <param name="pSampleType">Sample Type identifier</param>
    ''' <remarks>
    ''' Created by:  DL 06/04/2011
    ''' Modified by: SA 10/05/2011 - Removed use of STATLOCK status. Shown the Test ShortName as button text
    '''              RH 12/03/2012 - Introduce COLUMN_COUNT const
    ''' </remarks>
    Private Sub LoadTestsBySampleType(ByVal pSampleType As String)
        Try
            'Clean the GridView from previous Tests
            bsTestListDataGridView.Rows.Clear()

            'Get the list of Tests for the selected SampleType
            Dim qSearchTest As List(Of SelectedTestsDS.SelectedTestTableRow) = (From a In standardTestList.SelectedTestTable _
                                                                               Where a.SampleType = pSampleType _
                                                                              Select a).ToList()

            'Load the Tests in the Grid with the correspondent Colors according the Status of each one
            Dim maxSTDTests As Integer = qSearchTest.Count
            If (maxSTDTests > 0) Then
                'Set the number of rows required for the GridView for this SampleType
                Dim numGridRows As Integer = (maxSTDTests \ COLUMN_COUNT)

                If (maxSTDTests Mod COLUMN_COUNT) = 0 Then numGridRows -= 1
                bsTestListDataGridView.Rows.Add(numGridRows + 1)

                Dim standarPos As Integer = 0
                Dim testPosition As Integer = 0

                For j As Integer = 0 To numGridRows
                    For k As Integer = 0 To COLUMN_COUNT - 1
                        testPosition = (COLUMN_COUNT * j) + k
                        If (testPosition >= maxSTDTests) Then Exit For
                        bsTestListDataGridView.Rows(j).Cells(k).Value = qSearchTest(standarPos).ShortName
                        bsTestListDataGridView.Rows(j).Cells(k).ToolTipText = qSearchTest(standarPos).TestName

                        If (qSearchTest(standarPos).OTStatus = "INPROCESS") Then
                            'Change the Background Color and the Foreground Color to indicate that this Test is InUse and cannot be selected
                            bsTestListDataGridView.Rows(j).Cells(k).Style.BackColor = INPROCESS_BACK_COLOR
                            bsTestListDataGridView.Rows(j).Cells(k).Style.SelectionBackColor = INPROCESS_BACK_COLOR
                            bsTestListDataGridView.Rows(j).Cells(k).Style.ForeColor = INPROCESS_FORE_COLOR
                            bsTestListDataGridView.Rows(j).Cells(k).Style.SelectionForeColor = INPROCESS_FORE_COLOR
                        ElseIf (qSearchTest(standarPos).Selected) Then
                            'Change the Background Color and the Foreground Color to indicate that this Test is selected
                            bsTestListDataGridView.Rows(j).Cells(k).Style.BackColor = SELECTED_BACK_COLOR
                            bsTestListDataGridView.Rows(j).Cells(k).Style.SelectionBackColor = SELECTED_BACK_COLOR
                            bsTestListDataGridView.Rows(j).Cells(k).Style.ForeColor = SELECTED_FORE_COLOR
                            bsTestListDataGridView.Rows(j).Cells(k).Style.SelectionForeColor = SELECTED_FORE_COLOR
                        End If

                        'Increase the next position for the list of standard Tests
                        standarPos += 1
                    Next k
                Next j

                'Set the width of each column
                For k As Integer = 0 To COLUMN_COUNT - 1
                    bsTestListDataGridView.Columns(k).Width = GRID_CELLS_WIDTH
                    bsTestListDataGridView.Columns(k).Resizable = DataGridViewTriState.False
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadTestsBySampleType", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadTestsBySampleType", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the grid with all ISE Tests for the Sample Type selected in the ComboBox
    ''' </summary>
    ''' <param name="pSampleType">Sample Type identifier</param>
    ''' <remarks>
    ''' Created by: RH 19/06/2012
    ''' </remarks>
    Private Sub LoadISETestsBySampleType(ByVal pSampleType As String)
        Try
            'Clean the GridView from previous Tests
            bsISETestDataGridView.Rows.Clear()

            'Get the list of Tests for the selected SampleType
            Dim qSearchTest As List(Of SelectedTestsDS.SelectedTestTableRow) = (From a In iseTestList.SelectedTestTable _
                                                                               Where a.SampleType = pSampleType _
                                                                              Select a).ToList()

            'Load the Tests in the Grid with the correspondent Colors according the Status of each one
            Dim maxISETests As Integer = qSearchTest.Count
            If (maxISETests > 0) Then
                'Set the number of rows required for the GridView for this SampleType
                Dim numGridRows As Integer = (maxISETests \ COLUMN_COUNT)

                If (maxISETests Mod COLUMN_COUNT) = 0 Then numGridRows -= 1
                bsISETestDataGridView.Rows.Add(numGridRows + 1)

                Dim isePos As Integer = 0
                Dim testPosition As Integer = 0

                For j As Integer = 0 To numGridRows
                    For k As Integer = 0 To COLUMN_COUNT - 1
                        testPosition = (COLUMN_COUNT * j) + k

                        If (testPosition >= maxISETests) Then Exit For

                        bsISETestDataGridView.Rows(j).Cells(k).Value = qSearchTest(isePos).ShortName
                        bsISETestDataGridView.Rows(j).Cells(k).ToolTipText = qSearchTest(isePos).TestName

                        If (qSearchTest(isePos).OTStatus = "INPROCESS") Then
                            'Change the Background Color and the Foreground Color to indicate that this Test is InUse and cannot be selected
                            bsISETestDataGridView.Rows(j).Cells(k).Style.BackColor = INPROCESS_BACK_COLOR
                            bsISETestDataGridView.Rows(j).Cells(k).Style.SelectionBackColor = INPROCESS_BACK_COLOR
                            bsISETestDataGridView.Rows(j).Cells(k).Style.ForeColor = INPROCESS_FORE_COLOR
                            bsISETestDataGridView.Rows(j).Cells(k).Style.SelectionForeColor = INPROCESS_FORE_COLOR
                        ElseIf (qSearchTest(isePos).Selected) Then
                            'Change the Background Color and the Foreground Color to indicate that this Test is selected
                            bsISETestDataGridView.Rows(j).Cells(k).Style.BackColor = SELECTED_BACK_COLOR
                            bsISETestDataGridView.Rows(j).Cells(k).Style.SelectionBackColor = SELECTED_BACK_COLOR
                            bsISETestDataGridView.Rows(j).Cells(k).Style.ForeColor = SELECTED_FORE_COLOR
                            bsISETestDataGridView.Rows(j).Cells(k).Style.SelectionForeColor = SELECTED_FORE_COLOR
                        End If

                        'Increase the next position for the list of standard Tests
                        isePos += 1
                    Next k
                Next j

                'Set the width of each column
                For k As Integer = 0 To COLUMN_COUNT - 1
                    bsISETestDataGridView.Columns(k).Width = GRID_CELLS_WIDTH
                    bsISETestDataGridView.Columns(k).Resizable = DataGridViewTriState.False
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadISETestsBySampleType", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadISETestsBySampleType", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the selection/unselection of Standard Tests when the screen is opened from the Controls Programming Screen
    ''' </summary>
    ''' <param name="pRowIndex">Test position in the grid of Standard Tests - Row number</param>
    ''' <param name="pColIndex">Test position in the grid of Standard Tests - Column number</param>
    ''' <param name="pSampleType">Sample Type currently shown</param> 
    ''' <remarks>
    ''' Created by:  DL 06/04/2011
    ''' Modified by: RH 19/06/2012 - Search Test filtering by TestType, because now we have more than one TestTypes
    ''' </remarks>
    Private Function MarkUnMarkSelectedCellModel2(ByVal pRowIndex As Integer, ByVal pColIndex As Integer, ByVal pSampleType As String) As Boolean
        Dim selectedTest As Boolean = False

        Try
            'Search on structure of Selected Tests by SampleType, Col and Row, but only if the clicked Test is not Locked
            Dim qOriginalSelTest As List(Of SelectedTestsDS.SelectedTestTableRow)

            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
            qSelectedTest = (From a In standardTestList.SelectedTestTable _
                            Where a.SampleType = pSampleType _
                          AndAlso a.Row = pRowIndex _
                          AndAlso a.Col = pColIndex _
                          AndAlso a.OTStatus <> "INPROCESS" _
                           Select a).ToList()

            If (qSelectedTest.Count = 1) Then
                'Validate if the clicked Test is selected or not to set the next status
                If (Not qSelectedTest.First.Selected) Then
                    'If not selected, then the selected status is set to true                    
                    qSelectedTest.First.Selected = True
                    selectedTest = True
                    bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.LightSlateGray
                    bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.LightSlateGray
                    bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.White
                    bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.White
                Else
                    'Test can be unselected only if it is NOT IN USE 
                    If (qSelectedTest.First.OTStatus = "OPEN") Then
                        qSelectedTest.First.Selected = False

                        'Search if the Test was initially selected
                        qOriginalSelTest = (From a In selectTestList.SelectedTestTable _
                                           Where a.TestID = qSelectedTest.First.TestID _
                                         AndAlso a.SampleType = qSelectedTest.First.SampleType _
                                         AndAlso a.TestType = qSelectedTest.First.TestType _
                                          Select a).ToList()

                        If (qOriginalSelTest.Count = 0) Then
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.White
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.White
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Black
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Black
                        Else
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = Color.IndianRed
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = Color.IndianRed
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = Color.Black
                            bsTestListDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = Color.Black
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkSelectedCellModel2", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkSelectedCellModel2", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return selectedTest
    End Function

    ''' <summary>
    ''' Manages the selection/unselection of ISE Tests when the screen is opened from the Controls Programming Screen
    ''' </summary>
    ''' <param name="pRowIndex">Test position in the grid of Standard Tests - Row number</param>
    ''' <param name="pColIndex">Test position in the grid of Standard Tests - Column number</param>
    ''' <param name="pSampleType">Sample Type currently shown</param> 
    ''' <remarks>
    ''' Created by: RH 19/06/2012
    ''' </remarks>
    Private Function MarkUnMarkISETestCellModel2(ByVal pRowIndex As Integer, ByVal pColIndex As Integer, ByVal pSampleType As String) As Boolean
        Dim selectedTest As Boolean = False

        Try
            'Search on structure of Selected Tests by SampleType, Col and Row, but only if the clicked Test is not Locked
            Dim qOriginalSelTest As List(Of SelectedTestsDS.SelectedTestTableRow)

            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)
            qSelectedTest = (From a In iseTestList.SelectedTestTable _
                             Where a.SampleType = pSampleType _
                             AndAlso a.Row = pRowIndex _
                             AndAlso a.Col = pColIndex _
                             AndAlso a.OTStatus <> "INPROCESS" _
                             Select a).ToList()

            If (qSelectedTest.Count = 1) Then
                'Validate if the clicked Test is selected or not to set the next status
                If (Not qSelectedTest.First.Selected) Then
                    'If not selected, then the selected status is set to true                    
                    qSelectedTest.First.Selected = True
                    selectedTest = True
                    bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = SELECTED_BACK_COLOR
                    bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = SELECTED_BACK_COLOR
                    bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = SELECTED_FORE_COLOR
                    bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = SELECTED_FORE_COLOR
                Else
                    'Test can be unselected only if it is NOT IN USE 
                    If (qSelectedTest.First.OTStatus = "OPEN") Then
                        qSelectedTest.First.Selected = False

                        'Search if the Test was initially selected
                        qOriginalSelTest = (From a In selectTestList.SelectedTestTable _
                                            Where a.TestID = qSelectedTest.First.TestID _
                                            AndAlso a.SampleType = qSelectedTest.First.SampleType _
                                            AndAlso a.TestType = qSelectedTest.First.TestType _
                                            Select a).ToList()

                        If (qOriginalSelTest.Count = 0) Then
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = NOTSELECTED_BACK_COLOR
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = NOTSELECTED_BACK_COLOR
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = NOTSELECTED_FORE_COLOR
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = NOTSELECTED_FORE_COLOR
                        Else
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.BackColor = UNSELECTED_BACK_COLOR
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionBackColor = UNSELECTED_BACK_COLOR
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.ForeColor = UNSELECTED_FORE_COLOR
                            bsISETestDataGridView.Rows(pRowIndex).Cells(pColIndex).Style.SelectionForeColor = UNSELECTED_FORE_COLOR
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkISETestCellModel2", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkISETestCellModel2", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        Return selectedTest
    End Function

    ''' <summary>
    ''' Manages selection/unselection of STD Tests
    ''' </summary>
    ''' <param name="pRowIndex">Test position in the grid of std Tests - Row number</param>
    ''' <param name="pColIndex">Test position in the grid of std Tests - Column number</param>
    ''' <param name="pSampleType">Sample Type currently shown</param>
    ''' <remarks>
    ''' Created by:  DL 06/04/2011
    ''' </remarks>
    Private Sub MarkUnMarkSTDIntervalsModel2(ByVal pRowIndex As Integer, ByVal pColIndex As Integer, ByVal pSampleType As String)
        Try
            'Verify if the Standard Test clicked has status OPEN and if it is currently selected or not
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)

            qSelectedTest = (From a In standardTestList.SelectedTestTable _
                            Where a.SampleType = pSampleType _
                          AndAlso a.Row = pRowIndex _
                          AndAlso a.Col = pColIndex _
                          AndAlso a.OTStatus = "OPEN" _
                           Select a).ToList()

            Dim selectedState As Boolean = False

            If (qSelectedTest.Count = 1) Then
                selectedState = (qSelectedTest.First.Selected)

                If (lastRowClickedSTD = -1) Then
                    'Click in a Test when no one has been selected before, mark all Tests placed 
                    'previously in the grid
                    qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                    Where a.SampleType = pSampleType _
                                  AndAlso a.Row <= pRowIndex _
                                  AndAlso a.OTStatus = "OPEN" _
                                  AndAlso a.Selected = selectedState _
                                   Select a).ToList()

                    For Each stdTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                        If (stdTest.Row = pRowIndex AndAlso stdTest.Col > pColIndex) Then Exit For
                        MarkUnMarkSelectedCellModel2(stdTest.Row, stdTest.Col, stdTest.SampleType)
                    Next
                Else
                    'There is an interval of Tests to mark/unmark, but only if the action in both Tests forming the 
                    'interval limits is the same (if the last clicked was selected the current clicked has to be also
                    'to select)
                    If (selectedState <> lastSelStatusSTD) Then
                        If (lastRowClickedSTD < pRowIndex) Then
                            'Ascending row selection 
                            qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                            Where a.SampleType = pSampleType _
                                          AndAlso (a.Row >= lastRowClickedSTD And a.Row <= pRowIndex) _
                                          AndAlso a.OTStatus = "OPEN" _
                                          AndAlso a.Selected = selectedState _
                                           Select a).ToList()

                            For Each stdTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (stdTest.Row = lastRowClickedSTD AndAlso stdTest.Col > lastColClickedSTD) OrElse _
                                   (stdTest.Row > lastRowClickedSTD AndAlso stdTest.Row < pRowIndex) OrElse _
                                   (stdTest.Row = pRowIndex AndAlso stdTest.Col <= pColIndex) Then
                                    MarkUnMarkSelectedCellModel2(stdTest.Row, stdTest.Col, stdTest.SampleType)
                                End If
                            Next

                        ElseIf (lastRowClickedSTD > pRowIndex) Then
                            'Descending row selection 
                            qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                            Where a.SampleType = pSampleType _
                                          AndAlso (a.Row >= pRowIndex AndAlso a.Row <= lastRowClickedSTD) _
                                          AndAlso a.OTStatus = "OPEN" _
                                          AndAlso a.Selected = selectedState _
                                           Select a).ToList()

                            For Each stdTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (stdTest.Row = pRowIndex AndAlso stdTest.Col >= pColIndex) OrElse _
                                   (stdTest.Row > pRowIndex AndAlso stdTest.Row < lastRowClickedSTD) OrElse _
                                   (stdTest.Row = lastRowClickedSTD AndAlso stdTest.Col < lastColClickedSTD) Then
                                    MarkUnMarkSelectedCellModel2(stdTest.Row, stdTest.Col, stdTest.SampleType)
                                End If
                            Next
                        Else
                            'Same row selecting
                            'If (lastColClickedSTD < pRowIndex) Then
                            If (lastColClickedSTD < pColIndex) Then 'RH 31/05/2012
                                'Ascending column selection
                                qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                                Where a.SampleType = pSampleType _
                                              AndAlso a.Row = pRowIndex _
                                              AndAlso (a.Col > lastColClickedSTD AndAlso a.Col <= pColIndex) _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.Selected = selectedState _
                                               Select a).ToList()

                                For Each stdTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (stdTest.Col > lastColClickedSTD AndAlso stdTest.Col <= pColIndex) Then
                                        MarkUnMarkSelectedCellModel2(stdTest.Row, stdTest.Col, stdTest.SampleType)
                                    End If
                                Next

                            ElseIf (lastColClickedSTD > pColIndex) Then
                                'Descending column selection
                                qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                                Where a.SampleType = pSampleType _
                                              AndAlso a.Row = pRowIndex _
                                              AndAlso (a.Col >= pColIndex AndAlso a.Col < lastColClickedSTD) _
                                              AndAlso a.OTStatus = "OPEN" _
                                              AndAlso a.Selected = selectedState _
                                               Select a).ToList()

                                For Each stdTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (stdTest.Col >= pColIndex AndAlso stdTest.Col < lastColClickedSTD) Then
                                        MarkUnMarkSelectedCellModel2(stdTest.Row, stdTest.Col, stdTest.SampleType)
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If

            'Verify Tests with Factory Calibration values
            If (qSelectedTest.Count > 0) Then
                'Dim testWithFactoryValues As String = ""
                'For Each mySelTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                '    testWithFactoryValues &= ValidateFactoryValues(mySelTest.Row, mySelTest.Col, mySelTest.SampleType)
                'Next

                'If (testWithFactoryValues <> "") Then
                '    ShowMessage(Me.Name & ".MarkUnMarkSTDIntervalsModel2", GlobalEnumerates.Messages.FACTORY_VALUES.ToString, vbCrLf & testWithFactoryValues)
                'End If

                'RH 23/05/2012 Use StringBuilder for massive String concatenations.
                Dim testWithFactoryValues As New System.Text.StringBuilder()
                For Each mySelTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                    If mySelTest.Selected AndAlso Not selectedState Then
                        testWithFactoryValues.Append(ValidateFactoryValues(mySelTest.Row, mySelTest.Col, mySelTest.SampleType))
                    End If
                Next

                If (testWithFactoryValues.Length > 0) Then
                    Using AuxForm As New IWSTestSelectionWarning()
                        AuxForm.Message = testWithFactoryValues.ToString()
                        AuxForm.ShowDialog()
                    End Using
                End If
                'RH 23/05/2012 END
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkSTDIntervalsModel2", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkSTDIntervalsModel2", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages selection/unselection of ISE Tests
    ''' </summary>
    ''' <param name="pRowIndex">Test position in the grid of std Tests - Row number</param>
    ''' <param name="pColIndex">Test position in the grid of std Tests - Column number</param>
    ''' <param name="pSampleType">Sample Type currently shown</param>
    ''' <remarks>
    ''' Created by: RH 19/06/2012
    ''' </remarks>
    Private Sub MarkUnMarkISEIntervalsModel2(ByVal pRowIndex As Integer, ByVal pColIndex As Integer, ByVal pSampleType As String)
        Try
            'Verify if the Standard Test clicked has status OPEN and if it is currently selected or not
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)

            qSelectedTest = (From a In iseTestList.SelectedTestTable _
                             Where a.SampleType = pSampleType _
                             AndAlso a.Row = pRowIndex _
                             AndAlso a.Col = pColIndex _
                             AndAlso a.OTStatus = "OPEN" _
                             Select a).ToList()

            Dim selectedState As Boolean = False

            If (qSelectedTest.Count = 1) Then
                selectedState = (qSelectedTest.First.Selected)

                If (lastRowClickedISE = -1) Then
                    'Click in a Test when none has been selected before, mark all Tests placed 
                    'previously in the grid
                    qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                     Where a.SampleType = pSampleType _
                                     AndAlso a.Row <= pRowIndex _
                                     AndAlso a.OTStatus = "OPEN" _
                                     AndAlso a.Selected = selectedState _
                                     Select a).ToList()

                    For Each iseTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                        If (iseTest.Row = pRowIndex AndAlso iseTest.Col > pColIndex) Then Exit For
                        MarkUnMarkISETestCellModel2(iseTest.Row, iseTest.Col, iseTest.SampleType)
                    Next
                Else
                    'There is an interval of Tests to mark/unmark, but only if the action in both Tests forming the 
                    'interval limits is the same (if the last clicked was selected the current clicked has to be also
                    'to select)
                    If (selectedState <> lastSelStatusISE) Then
                        If (lastRowClickedISE < pRowIndex) Then
                            'Ascending row selection 
                            qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                             Where a.SampleType = pSampleType _
                                             AndAlso (a.Row >= lastRowClickedISE AndAlso a.Row <= pRowIndex) _
                                             AndAlso a.OTStatus = "OPEN" _
                                             AndAlso a.Selected = selectedState _
                                             Select a).ToList()

                            For Each iseTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (iseTest.Row = lastRowClickedISE AndAlso iseTest.Col > lastColClickedISE) OrElse _
                                   (iseTest.Row > lastRowClickedISE AndAlso iseTest.Row < pRowIndex) OrElse _
                                   (iseTest.Row = pRowIndex AndAlso iseTest.Col <= pColIndex) Then
                                    MarkUnMarkISETestCellModel2(iseTest.Row, iseTest.Col, iseTest.SampleType)
                                End If
                            Next

                        ElseIf (lastRowClickedISE > pRowIndex) Then
                            'Descending row selection 
                            qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                             Where a.SampleType = pSampleType _
                                             AndAlso (a.Row >= pRowIndex AndAlso a.Row <= lastRowClickedISE) _
                                             AndAlso a.OTStatus = "OPEN" _
                                             AndAlso a.Selected = selectedState _
                                             Select a).ToList()

                            For Each iseTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                If (iseTest.Row = pRowIndex AndAlso iseTest.Col >= pColIndex) OrElse _
                                   (iseTest.Row > pRowIndex AndAlso iseTest.Row < lastRowClickedISE) OrElse _
                                   (iseTest.Row = lastRowClickedISE AndAlso iseTest.Col < lastColClickedISE) Then
                                    MarkUnMarkISETestCellModel2(iseTest.Row, iseTest.Col, iseTest.SampleType)
                                End If
                            Next
                        Else
                            'Same row selecting
                            If (lastColClickedISE < pColIndex) Then
                                'Ascending column selection
                                qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                                 Where a.SampleType = pSampleType _
                                                 AndAlso a.Row = pRowIndex _
                                                 AndAlso (a.Col > lastColClickedISE AndAlso a.Col <= pColIndex) _
                                                 AndAlso a.OTStatus = "OPEN" _
                                                 AndAlso a.Selected = selectedState _
                                                 Select a).ToList()

                                For Each iseTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (iseTest.Col > lastColClickedISE AndAlso iseTest.Col <= pColIndex) Then
                                        MarkUnMarkISETestCellModel2(iseTest.Row, iseTest.Col, iseTest.SampleType)
                                    End If
                                Next

                            ElseIf (lastColClickedISE > pColIndex) Then
                                'Descending column selection
                                qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                                 Where a.SampleType = pSampleType _
                                                 AndAlso a.Row = pRowIndex _
                                                 AndAlso (a.Col >= pColIndex AndAlso a.Col < lastColClickedISE) _
                                                 AndAlso a.OTStatus = "OPEN" _
                                                 AndAlso a.Selected = selectedState _
                                                 Select a).ToList()

                                For Each iseTest As SelectedTestsDS.SelectedTestTableRow In qSelectedTest
                                    If (iseTest.Col >= pColIndex AndAlso iseTest.Col < lastColClickedISE) Then
                                        MarkUnMarkISETestCellModel2(iseTest.Row, iseTest.Col, iseTest.SampleType)
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".MarkUnMarkISEIntervalsModel2", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".MarkUnMarkISEIntervalsModel2", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the structure to return with the list of selected Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 06/04/2011
    ''' Modified by: SA 10/05/2011 - 
    ''' Modified by RH: 14/06/2012 Adapted for ISE TestType
    ''' </remarks>
    Private Sub AcceptTestSelectionModel2()
        Try
            'Hide the Form to avoid ugly downloading effects while in WS Preparation screen the grid is loaded
            Me.Opacity = 0

            ListOfSelectedTestsAttribute = New SelectedTestsDS

            'Get the list of Tests that have been selected...
            Dim qSelStdAndISETests As List(Of SelectedTestsDS.SelectedTestTableRow)

            'Get Std Tests
            qSelStdAndISETests = (From a In standardTestList.SelectedTestTable _
                                  Where a.Selected _
                                  Select a).ToList()

            'Add ISE Tests
            qSelStdAndISETests.AddRange((From a In iseTestList.SelectedTestTable _
                                         Where a.Selected _
                                         Select a).ToList())

            'Add all selected Tests to the DataSet to return
            Dim qSelectedTest As List(Of SelectedTestsDS.SelectedTestTableRow)

            For Each selectedRow As SelectedTestsDS.SelectedTestTableRow In qSelStdAndISETests
                'Verify if the Test has been selected now or if it was originally selected when the screen was opened
                Dim myTestID As Integer = selectedRow.TestID
                Dim mySampleType As String = selectedRow.SampleType
                Dim myTestType As String = selectedRow.TestType

                qSelectedTest = (From a In selectTestList.SelectedTestTable _
                                 Where a.TestType = myTestType _
                                 AndAlso a.SampleType = mySampleType _
                                 AndAlso a.TestID = myTestID _
                                 Select a).ToList()

                If (qSelectedTest.Count = 0) Then
                    'Add the Standard Test to the list of Selected Tests
                    selectedRow.OTStatus = "NEW"
                Else
                    qSelectedTest.First.OTStatus = "EQUAL"
                    selectedRow.OTStatus = "EQUAL"
                End If

                ListOfSelectedTestsAttribute.SelectedTestTable.ImportRow(selectedRow)
            Next

            'Search if some Tests have been deleted
            Dim qOriginalTests As List(Of SelectedTestsDS.SelectedTestTableRow)

            qOriginalTests = (From a In selectTestList.SelectedTestTable _
                              Where a.OTStatus <> "EQUAL" _
                              Select a).ToList()

            For Each originalRow As SelectedTestsDS.SelectedTestTableRow In qOriginalTests
                'Set the Status to DELETED and import the row to the DS to return
                originalRow.OTStatus = "DELETED"
                ListOfSelectedTestsAttribute.SelectedTestTable.ImportRow(originalRow)
            Next originalRow

            'Close the screen
            Me.Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AcceptTestSelectionModel2", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AcceptTestSelectionModel2", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Events"
    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010 
    ''' </remarks>
    Private Sub WSTestSelectionAuxScreen_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                bsCancelButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WSTestSelectionAuxScreen_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WSTestSelectionAuxScreen_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen loading
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 28/07/2011 - Open the screen centered regarding the Main MDI form
    ''' </remarks>
    Private Sub WSTestSelectionAuxScreen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If (WorkingModelAttribute = GlbSourceScreen.STANDARD.ToString) Then
                InitializeScreen()
            ElseIf (WorkingModelAttribute = GlbSourceScreen.MODEL2.ToString) Then
                InitializeScreenModel2()
            End If

            Dim mySize As Size = IAx00MainMDI.Size
            Dim myLocation As Point = IAx00MainMDI.Location

            If (Not Me.MdiParent Is Nothing) Then
                mySize = Me.Parent.Size
                myLocation = Me.Parent.Location
            End If

            myNewLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            Me.Location = myNewLocation

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WSTestSelectionAuxScreen_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WSTestSelectionAuxScreen_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
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
    ''' Manages the simple or multiple selection of Standard Tests
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 22/07/2010 - Added management of multiple selection using the Shift Key
    '''              TR 10/03/2011 - When the Test was selected, validate if it has Factory Calibration 
    '''                              values to show the Warning message
    '''              DL 06/04/2011 - Added code needed when the screen is opened from Controls Programming Screen 
    '''              RH 24/05/2012 - Message for Factory Calibration change is shown in an auxiliary screen instead of a MsgBox
    ''' </remarks>
    Private Sub bsTestListDataGridView_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsTestListDataGridView.CellClick
        Try
            If (WorkingModelAttribute = GlbSourceScreen.STANDARD.ToString) Then
                If (Not shiftKeyIsPressedSTD) Then

                    Dim selectedAction As Boolean = MarkUnMarkSelectedCell(e.RowIndex, e.ColumnIndex)

                    'Set value of global variables containing information of the last clicked/unclicked cell
                    lastRowClickedSTD = e.RowIndex
                    lastColClickedSTD = e.ColumnIndex
                    lastSelStatusSTD = selectedAction

                    If (selectedAction) Then
                        Dim testWithFactoryValues As String = ValidateFactoryValues(e.RowIndex, e.ColumnIndex)
                        If (testWithFactoryValues.Length > 0) Then
                            Using AuxForm As New IWSTestSelectionWarning()
                                AuxForm.Message = testWithFactoryValues
                                AuxForm.ShowDialog()
                            End Using
                        End If
                    End If
                Else
                    MarkUnMarkSTDIntervals(e.RowIndex, e.ColumnIndex)
                End If
                shiftKeyIsPressedSTD = False

            ElseIf (WorkingModelAttribute = GlbSourceScreen.MODEL2.ToString) Then
                If (Not shiftKeyIsPressedSTD) Then
                    Dim selectedAction As Boolean = MarkUnMarkSelectedCellModel2(e.RowIndex, e.ColumnIndex, SampleTypeAttribute)

                    'Set value of global variables containing information of the last clicked/unclicked cell
                    lastRowClickedSTD = e.RowIndex
                    lastColClickedSTD = e.ColumnIndex
                    lastSelStatusSTD = selectedAction

                    If (selectedAction) Then
                        Dim testWithFactoryValues As String = ValidateFactoryValues(e.RowIndex, e.ColumnIndex, SampleTypeAttribute)
                        If (testWithFactoryValues.Length > 0) Then
                            Using AuxForm As New IWSTestSelectionWarning()
                                AuxForm.Message = testWithFactoryValues
                                AuxForm.ShowDialog()
                            End Using
                        End If
                    End If
                Else
                    MarkUnMarkSTDIntervalsModel2(e.RowIndex, e.ColumnIndex, SampleTypeAttribute)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTestListDataGridView_CellClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListDataGridView_CellClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set value True to the global variable that indicates the Shift Key has been pressed for Standard Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 22/07/2010
    ''' </remarks>
    Private Sub bsTestListDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsTestListDataGridView.KeyDown
        If (e.KeyCode = Keys.ShiftKey) Then shiftKeyIsPressedSTD = True
    End Sub

    ''' <summary>
    ''' Set value False to the global variable that indicates the Shift Key has been loosen for Standard Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 22/07/2010
    ''' </remarks>
    Private Sub bsTestListDataGridView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsTestListDataGridView.KeyUp
        If (e.KeyCode = Keys.ShiftKey) Then shiftKeyIsPressedSTD = False
    End Sub

    ''' <summary>
    ''' Manages the simple or multiple selection of Calculated Tests
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 22/07/2010 - Added management of multiple selection using the Shift Key 
    '''                              (not available yet, it need some review, it doesn't work fine always)
    '''              SA 02/05/2012 - Management of multiple selection using the Shift Key
    ''' </remarks>
    Private Sub bsCalcTestDataGridView_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsCalcTestDataGridView.CellClick
        Try
            If (Not shiftKeyIsPressedCALC) Then
                Dim selectedAction As Boolean = MarkUnMarkCalculatedTestCell(e.RowIndex, e.ColumnIndex)

                'Set value of global variables containing information of the last clicked/unclicked cell
                lastRowClickedCALC = e.RowIndex
                lastColClickedCALC = e.ColumnIndex
                lastSelStatusCALC = selectedAction
            Else
                MarkUnMarkCALCIntervals(e.RowIndex, e.ColumnIndex)
                shiftKeyIsPressedCALC = False 'RH 31/05/2012
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCalcTestDataGridView_CellClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCalcTestDataGridView_CellClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set value True to the global variable that indicates the Shift Key has been pressed for Calculated Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 22/07/2010
    ''' </remarks>
    Private Sub bsCalcTestDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsCalcTestDataGridView.KeyDown
        If (e.KeyCode = Keys.ShiftKey) Then shiftKeyIsPressedCALC = True
    End Sub

    ''' <summary>
    ''' Set value True to the global variable that indicates the Shift Key has been loosen for Calculated  Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 22/07/2010
    ''' </remarks>
    Private Sub bsCalcTestDataGridView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsCalcTestDataGridView.KeyUp
        If (e.KeyCode = Keys.ShiftKey) Then shiftKeyIsPressedCALC = False
    End Sub

    ''' <summary>
    ''' Manages the simple or multiple selection of ISE Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 26/10/2010
    ''' Modified by: RH 19/06/2012
    ''' </remarks>
    Private Sub bsISETestDataGridView_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsISETestDataGridView.CellClick
        Try
            If (WorkingModelAttribute = GlbSourceScreen.STANDARD.ToString()) Then
                If (Not shiftKeyIsPressedISE) Then
                    Dim selectedAction As Boolean = MarkUnMarkISETestCell(e.RowIndex, e.ColumnIndex)

                    'Set value of global variables containing information of the last clicked/unclicked cell
                    lastRowClickedISE = e.RowIndex
                    lastColClickedISE = e.ColumnIndex
                    lastSelStatusISE = selectedAction
                Else
                    MarkUnMarkISEIntervals(e.RowIndex, e.ColumnIndex)
                End If

            ElseIf (WorkingModelAttribute = GlbSourceScreen.MODEL2.ToString()) Then
                If (Not shiftKeyIsPressedISE) Then
                    Dim selectedAction As Boolean = MarkUnMarkISETestCellModel2(e.RowIndex, e.ColumnIndex, SampleTypeAttribute)

                    'Set value of global variables containing information of the last clicked/unclicked cell
                    lastRowClickedISE = e.RowIndex
                    lastColClickedISE = e.ColumnIndex
                    lastSelStatusISE = selectedAction
                Else
                    MarkUnMarkISEIntervalsModel2(e.RowIndex, e.ColumnIndex, SampleTypeAttribute)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsISETestDataGridView_CellClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsISETestDataGridView_CellClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set value True to the global variable that indicates the Shift Key has been pressed for ISE Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 26/07/2010
    ''' </remarks>
    Private Sub bsISETestDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsISETestDataGridView.KeyDown
        If (e.KeyCode = Keys.ShiftKey) Then shiftKeyIsPressedISE = True
    End Sub

    ''' <summary>
    ''' Set value True to the global variable that indicates the Shift Key has been loosen for ISE  Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 26/10/2010
    ''' </remarks>
    Private Sub bsISETestDataGridView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsISETestDataGridView.KeyUp
        If (e.KeyCode = Keys.ShiftKey) Then shiftKeyIsPressedISE = False
    End Sub

    ''' <summary>
    ''' Manages the simple or multiple selection of Off-System Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 29/11/2010
    ''' </remarks>
    Private Sub bsOffSystemTestDataGridView_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsOffSystemTestDataGridView.CellClick

        Try
            If (Not shiftKeyIsPressedOffSystem) Then
                Dim selectedAction As Boolean = MarkUnMarkOffSystemTestCell(e.RowIndex, e.ColumnIndex)

                'Set value of global variables containing information of the last clicked/unclicked cell
                lastRowClickedOffSystem = e.RowIndex
                lastColClickedOffSystem = e.ColumnIndex
                lastSelStatusOffSystem = selectedAction

            Else
                MarkUnMarkOffSystemIntervals(e.RowIndex, e.ColumnIndex)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsOffSystemTestDataGridView_CellClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsOffSystemTestDataGridView_CellClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Set value True to the global variable that indicates the Shift Key has been pressed for Off-System Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 29/11/2010
    ''' </remarks>
    Private Sub bsOffSystemTestDataGridView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsOffSystemTestDataGridView.KeyDown
        If (e.KeyCode = Keys.ShiftKey) Then shiftKeyIsPressedOffSystem = True
    End Sub

    ''' <summary>
    ''' Set value True to the global variable that indicates the Shift Key has been loosen for Off-System  Tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 29/11/2010
    ''' </remarks>
    Private Sub bsOffSystemTestDataGridView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsOffSystemTestDataGridView.KeyUp
        If (e.KeyCode = Keys.ShiftKey) Then shiftKeyIsPressedOffSystem = False
    End Sub

    Private Sub bsProfilesTreeView_AfterCheck(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles bsProfilesTreeView.AfterCheck
        If (e.Action <> TreeViewAction.Unknown) Then chkChange = True
    End Sub

    Private Sub bsProfilesTreeView_BeforeCheck(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles bsProfilesTreeView.BeforeCheck
        If (e.Node.ForeColor = Color.CornflowerBlue) Then e.Cancel = True
    End Sub

    Private Sub bsProfilesTreeView_BeforeSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewCancelEventArgs) Handles bsProfilesTreeView.BeforeSelect
        If (e.Node.ForeColor = Color.CornflowerBlue) Then e.Cancel = True
    End Sub

    ''' <summary>
    ''' Control select/unselect of a Test Profile in the TreeView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 
    ''' Modified by: DL 10/03/2010
    '''              SA 04/05/2010 - Before unselect each Test in the Profile, verify it is included in a selected Calculated
    '''                              Test, because in this case it has to remain selected
    '''              DL 21/10/2010 - Diferences between Test Types
    '''              SA 29/10/2010 - Use TestKey instead TestID when search the Node in the TreeView. Call functions for Mark/Unmark
    '''                              Tests included in the selected/unselected Profile
    '''              SA 02/12/2010 - Added management of OffSystem Tests included in the selected Profile
    '''              TR 10/03/2011 - Added changes to validate if the selected Test Profile is composed for Standard
    '''                              Tests that are still using Factory Calibration values
    '''              SA 21/05/2014 - BT #1633 ==> Fixed an old error in the code that validates if a Test Node of an unselected Profile belongs to another Profile:
    '''                                           result of function SearchNode is assigned to variable tmpNode, but the checkings are executed using variable 
    '''                                           myTreeNode; this is wrong, checkings have to be executed using tmpNode
    ''' </remarks>
    Private Sub bsProfilesTreeView_NodeMouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles bsProfilesTreeView.NodeMouseClick
        Try
            Dim myTreeNode As New TreeNode()
            Dim tmpNode As New TreeNode()

            'List to store the STD Tests included in the selected Profile to validate if they have Factory Calibration value
            Dim myTestToValidate As New List(Of SelectedTestsDS.SelectedTestTableRow)

            If (chkChange) Then
                If (e.Node.Parent Is Nothing) Then
                    'If the selected Node is the parent, then assign the value to our myTreeNode variable
                    myTreeNode = e.Node
                Else
                    'If the selected Node is not the parent, then assign the parent value of this Node to our myTreeNode variable
                    myTreeNode = e.Node.Parent
                    myTreeNode.Checked = e.Node.Checked
                End If

                'Set the Mark/UnMark to the child nodes.
                For Each myNode As TreeNode In myTreeNode.Nodes
                    myNode.Checked = myTreeNode.Checked
                Next myNode

                If (myTreeNode.Checked) Then
                    Dim qSelectedTest As New List(Of SelectedTestsDS.SelectedTestTableRow)
                    For Each myNode As TreeNode In myTreeNode.Nodes
                        'Get the TestType of the Node
                        Dim testType As String = myNode.Name.Trim.Split(CChar("|"))(0)

                        Dim myNodeName As String = myNode.Name
                        Select Case (testType)
                            Case "STD"
                                'Search the Standard Test included in the selected Profile to select it
                                qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                                 Where a.TestKey = myNodeName _
                                                 Select a).ToList()

                            Case "CALC"
                                'Search the Calculated Test included in the selected Profile to select it
                                qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                                                 Where a.TestKey = myNodeName _
                                                 Select a).ToList()

                            Case "ISE"
                                'Search the ISE Test included in the selected Profile to select it
                                qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                                 Where a.TestKey = myNodeName _
                                                 Select a).ToList()

                            Case "OFFS"
                                'Search the Off System Test included in the selected Profile to select it
                                qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                                                 Where a.TestKey = myNodeName _
                                                 Select a).ToList()
                        End Select

                        If (qSelectedTest.Count > 0) Then
                            If (Not qSelectedTest.First.Selected) Then
                                'Validate if the Test in the Node belongs to another Profile
                                tmpNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)

                                'If the Test is not assigned to another TestProfile
                                If (Not tmpNode.Parent Is Nothing AndAlso tmpNode.Parent.Name = myTreeNode.Name) Then
                                    qSelectedTest.First().TestProfileID = CType(myTreeNode.Name, Integer)
                                    qSelectedTest.First().TestProfileName = myTreeNode.Text

                                    If (qSelectedTest.First().OTStatus = "OPEN") Then
                                        If (qSelectedTest.First().TestType = "STD") Then
                                            MarkUnMarkSelectedCell(qSelectedTest.First().Row, qSelectedTest.First.Col)

                                            'TR 10/03/2011 - Add the selected Test to the list for validation of Factory Calibration values
                                            myTestToValidate.Add(qSelectedTest.First())

                                        ElseIf (qSelectedTest.First().TestType = "CALC") Then
                                            MarkUnMarkCalculatedTestCell(qSelectedTest.First().Row, qSelectedTest.First.Col)

                                        ElseIf (qSelectedTest.First().TestType = "ISE") Then
                                            MarkUnMarkISETestCell(qSelectedTest.First().Row, qSelectedTest.First.Col)

                                        ElseIf (qSelectedTest.First().TestType = "OFFS") Then
                                            MarkUnMarkOffSystemTestCell(qSelectedTest.First().Row, qSelectedTest.First.Col)
                                        End If
                                    End If
                                End If
                            Else
                                'If the Test is selected (due to is linked to a selected Calculated Test), just fill Profile fields
                                qSelectedTest.First().TestProfileID = CType(myTreeNode.Name, Integer)
                                qSelectedTest.First().TestProfileName = myTreeNode.Text
                            End If
                        End If
                    Next
                Else
                    Dim qSelectedTest As New List(Of SelectedTestsDS.SelectedTestTableRow)
                    For Each myNode As TreeNode In myTreeNode.Nodes
                        'Get the TestType of the Node
                        Dim testType As String = myNode.Name.Trim.Split(CChar("|"))(0)

                        Dim myNodeName As String = myNode.Name
                        Dim remainSelected As Boolean = False
                        Select Case (testType)
                            Case "STD"
                                'Search all Standard Tests included in the selected Profile to verify if they can be unselected
                                qSelectedTest = (From a In standardTestList.SelectedTestTable _
                                                Where a.TestKey = myNodeName _
                                               Select a).ToList()
                                remainSelected = (qSelectedTest.First.CalcTestIDs.Trim <> "")

                            Case "CALC"
                                'Search the Standard Test included in the selected Profile to select it
                                qSelectedTest = (From a In calculatedTestList.SelectedTestTable _
                                                Where a.TestKey = myNodeName _
                                               Select a).ToList()
                                remainSelected = (qSelectedTest.First.CalcTestIDs.Trim <> "")

                            Case "ISE"
                                'Search the ISE Test included in the selected Profile to select it
                                qSelectedTest = (From a In iseTestList.SelectedTestTable _
                                                Where a.TestKey = myNodeName _
                                               Select a).ToList()
                                remainSelected = (qSelectedTest.First.CalcTestIDs.Trim <> "") ' False

                            Case "OFFS"
                                'Search the Off System Test included in the selected Profile to select it
                                qSelectedTest = (From a In offSystemTestList.SelectedTestTable _
                                                Where a.TestKey = myNodeName _
                                               Select a).ToList()
                                remainSelected = (qSelectedTest.First.CalcTestIDs.Trim <> "") ' False
                        End Select

                        If (qSelectedTest.Count > 0) Then
                            'Validate if the Node belong to another Profile
                            tmpNode = SearchNode(qSelectedTest.First.TestKey.ToString(), bsProfilesTreeView.Nodes)

                            'BT #1633 - Execute the checking using tmpNode instead of myTreeNode
                            If (tmpNode.Name <> String.Empty) Then
                                'The Test belong to another selected Test Profile: it is re-linked to it
                                If (tmpNode.Parent Is Nothing) Then
                                    qSelectedTest.First.TestProfileID = CType(tmpNode.Name, Integer)
                                    qSelectedTest.First.TestProfileName = tmpNode.Text
                                Else
                                    qSelectedTest.First.TestProfileID = CType(tmpNode.Parent.Name, Integer)
                                    qSelectedTest.First.TestProfileName = tmpNode.Parent.Text
                                End If
                            Else
                                'Remove the link to the unselected Test Profile
                                qSelectedTest.First().TestProfileID = 0
                                qSelectedTest.First().TestProfileName = ""

                                'Before unselect the Test, verify that it is not included in a Selected Calculated Test
                                If (Not remainSelected) Then
                                    'Modified by: DL 11/03/2010
                                    If (qSelectedTest.First().OTStatus = "OPEN") Then
                                        If (qSelectedTest.First().TestType = "STD") Then
                                            MarkUnMarkSelectedCell(qSelectedTest.First().Row, qSelectedTest.First.Col)

                                        ElseIf (qSelectedTest.First().TestType = "CALC") Then
                                            MarkUnMarkCalculatedTestCell(qSelectedTest.First().Row, qSelectedTest.First.Col)

                                        ElseIf (qSelectedTest.First().TestType = "ISE") Then
                                            MarkUnMarkISETestCell(qSelectedTest.First().Row, qSelectedTest.First.Col)

                                        ElseIf (qSelectedTest.First().TestType = "OFFS") Then
                                            MarkUnMarkOffSystemTestCell(qSelectedTest.First().Row, qSelectedTest.First.Col)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
                chkChange = False
            End If

            ''TR 10/03/2011 
            'If (myTestToValidate.Count > 0) Then
            '    Dim testWithFactoryValues As String = ""
            '    For Each mySelTest As SelectedTestsDS.SelectedTestTableRow In myTestToValidate
            '        testWithFactoryValues &= ValidateFactoryValues(mySelTest.Row, mySelTest.Col)
            '    Next

            '    If (testWithFactoryValues <> "") Then
            '        ShowMessage(Me.Name & ".bsProfilesTreeView_NodeMouseClick", GlobalEnumerates.Messages.FACTORY_VALUES.ToString, vbCrLf & testWithFactoryValues)
            '    End If
            'End If

            'RH 24/05/2012 Use StringBuilder for massive String concatenations.
            If myTestToValidate.Count > 0 Then
                Dim testWithFactoryValues As New System.Text.StringBuilder()
                For Each mySelTest As SelectedTestsDS.SelectedTestTableRow In myTestToValidate
                    If mySelTest.Selected Then
                        testWithFactoryValues.Append(ValidateFactoryValues(mySelTest.Row, mySelTest.Col))
                    End If
                Next

                If (testWithFactoryValues.Length > 0) Then
                    Using AuxForm As New IWSTestSelectionWarning()
                        AuxForm.Message = testWithFactoryValues.ToString()
                        AuxForm.ShowDialog()
                    End Using
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsProfilesTreeView_NodeMouseClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsProfilesTreeView_NodeMouseClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        If (WorkingModelAttribute = GlbSourceScreen.STANDARD.ToString()) Then
            AcceptTestSelection()
        ElseIf (WorkingModelAttribute = GlbSourceScreen.MODEL2.ToString()) Then
            AcceptTestSelectionModel2()
        End If
    End Sub

    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Me.Close()
    End Sub

    Private Sub bsSampleTypeComboBox_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsSampleTypeComboBox.SelectionChangeCommitted
        Try
            If (bsSampleTypeComboBox.SelectedValue.ToString <> SampleTypeAttribute) Then
                SampleTypeAttribute = bsSampleTypeComboBox.SelectedValue.ToString
                LoadTestsBySampleType(SampleTypeAttribute)
                LoadISETestsBySampleType(SampleTypeAttribute)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSampleTypeComboBox_SelectionChangeCommitted", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSampleTypeComboBox_SelectionChangeCommitted", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

End Class
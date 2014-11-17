Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types.AllowedTestsDS

Public Class IProgCalculatedTest
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declarations"
    Private selectedCalTestID As Integer                    '= 0 Redundant field initialization. To store the ID of the selected Calculated Test
    Private originalCalTestName As String = ""              'To store the Name of the selected calculated Test and control Pending Changes
    Private originalSelectedIndex As Integer = -1           'To store the index of the selected calculated Test and control Pending Changes
    Private myOriginalFormulaValue As String = ""           'To store the Formula of the selected Calculated Test and control Pending Changes 

    Private originalFormulaValues As New FormulasDS         'To store original values of the Formula of the selected Calculated Test
    Private SelectedTestRefRangesDS As New TestRefRangesDS  'To manage the Reference Ranges for the selected Calculated Test
    Private EditionMode As Boolean                          'To control when the selected Calculated Test is in Edition Mode

    'To manage Calculated Tests having a Formula composed for at least a deleted Test (this case is not possible
    'anymore due to now all elements affected by a deletion are also deleted after User's confirmation)
    Private myEnableStatus As Boolean

    Private UpdateHistoryRequired As Boolean 'To indicate if data of the Calculated Test has to be also updated in Historic Module
#End Region

#Region "Constructor"
    Public Sub New()
        'This call is required by the Windows Form Designer
        InitializeComponent()
    End Sub
#End Region

#Region "Methods"
    ''' <summary>
    ''' Prepare the screen for adding a new Calculated Test.  If there are changes pending
    ''' to save, the Discard Pending Changes Verification is executed
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Call to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              TR 12/01/2010 - Call the ShowMessage method directly and validate the User's answer
    '''              SA 16/12/2010 - Changes due new implementation of Reference Ranges control
    ''' </remarks>
    Private Sub AddCalculatedTest()
        Try
            If (PendingChangesVerification()) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    'Set Screen Status to ADD MODE
                    AddModeScreenStatus()
                Else
                    If (originalSelectedIndex <> -1) Then
                        'Return focus to the Caculated Test that has been edited
                        bsCalTestListView.Items(originalSelectedIndex).Selected = True
                        bsCalTestListView.Select()
                    End If
                End If
            Else
                'Set Screen Status to ADD MODE
                AddModeScreenStatus()
            End If

            EditionMode = True
            originalFormulaValues = Nothing
            bsCalTestFormula.FormulaValuesList = originalFormulaValues
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AddCalculatedTest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AddCalculatedTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to ADD MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic 
    '''                              function ShowMessage
    '''              SA 01/07/2010 - Set khaki BackColor for mandatory fields
    '''              SG 02/09/2010 - Added code to initialize tab of Reference Ranges
    '''              SA 16/12/2010 - Changed code for initialization of area of Reference Ranges
    ''' </remarks>
    Private Sub AddModeScreenStatus()
        Try
            'Area of Calculated Test List
            If (bsCalTestListView.Items.Count > 0) Then
                bsCalTestListView.Enabled = True
                bsCalTestListView.SelectedItems.Clear()
            End If

            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = False 'AG 05/09/2014 - BA-1869
            'bsPrintButton.Enabled = False DL 11/05/2012

            'Area of Calculated Test Definition
            bsFullNameTextbox.Text = ""
            bsFullNameTextbox.Enabled = True

            bsNameTextbox.Text = ""
            bsNameTextbox.Enabled = True

            bsUniqueRadioButton.Checked = True
            bsUniqueRadioButton.Enabled = True

            bsMultipleRadioButton.Checked = False
            bsMultipleRadioButton.Enabled = True

            bsSampleComboBox.SelectedIndex = 0
            bsSampleComboBox.Enabled = True
            bsSampleComboBox.BackColor = Color.White

            bsUnitComboBox.SelectedIndex = 0
            bsUnitComboBox.Enabled = True
            bsUnitComboBox.BackColor = Color.White

            bsDecimalsNumericUpDown.Value = bsDecimalsNumericUpDown.Minimum
            bsDecimalsNumericUpDown.Enabled = True
            bsDecimalsNumericUpDown.BackColor = Color.White

            bsPrintExpTestCheckbox.Checked = False
            bsPrintExpTestCheckbox.Enabled = True

            'Area of Formula Definition
            InitializeFormula()
            bsCalTestFormula.SelectedSampleType = bsSampleComboBox.SelectedValue.ToString
            bsCalTestFormula.EnableDisableControls = True

            'Area of Reference Ranges
            SelectedTestRefRangesDS = New TestRefRangesDS

            bsTestRefRanges.TestID = -1
            bsTestRefRanges.SampleType = ""
            bsTestRefRanges.ActiveRangeType = ""
            bsTestRefRanges.MeasureUnit = bsUnitComboBox.Text.ToString
            bsTestRefRanges.DefinedTestRangesDS = SelectedTestRefRangesDS

            Dim decimalsNumber As Integer = 0
            If (IsNumeric(bsDecimalsNumericUpDown.Text)) Then decimalsNumber = Convert.ToInt32(bsDecimalsNumericUpDown.Text)
            bsTestRefRanges.RefNumDecimals = decimalsNumber

            bsTestRefRanges.ClearReferenceRanges()
            bsTestRefRanges.isEditing = True

            'Clean error provider and put visible the tab for the Formula definition
            CalculatedTestTabControl.SelectTab(0)
            bsScreenErrorProvider.Clear()

            'Buttons in details area
            bsSaveButton.Enabled = True
            bsCancelButton.Enabled = True

            'Initialize global variables
            CleanGlobalValues()

            'Put Focus in the first enabled field
            bsFullNameTextbox.Focus()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AddModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AddModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Execute the cancelling of a Calculated Test adding or edition
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              TR 12/01/2010 - Calls the ShowMessage method directly and validate the User's answer
    '''              SA 16/12/2010 - Changes due new implementation of Reference Ranges control
    ''' </remarks>
    Private Sub CancelCalcTestEdition()
        Dim setScreenToInitial As Boolean = False

        Try
            If (PendingChangesVerification()) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    EditionMode = False
                    SelectedTestRefRangesDS.Clear()
                    setScreenToInitial = True
                Else
                    If (originalSelectedIndex <> -1) Then
                        'Return focus to the Calculated Test that has been edited
                        bsCalTestListView.Items(originalSelectedIndex).Selected = True
                        bsCalTestListView.Select()
                    End If
                End If
            Else
                setScreenToInitial = True
            End If

            If (setScreenToInitial) Then
                If (originalSelectedIndex <> -1) Then
                    bsCalTestListView.Items(originalSelectedIndex).Selected = True
                Else
                    'Set screen status to Initial Mode
                    InitialModeScreenStatus()
                    If (bsCalTestListView.Items.Count > 0) Then
                        'Select the first Calculated Test in the list
                        bsCalTestListView.Items(0).Selected = True
                    End If
                End If

                If (bsCalTestListView.SelectedItems.Count > 0) Then
                    'Load screen fields with all data of the selected Calculated Test Formula
                    Dim inUseCalcTest As Boolean = LoadDataOfCalculatedTestFormula()

                    'Get the Reference Ranges defined for the Calculated Test and shown them
                    GetRefRanges(selectedCalTestID)
                    LoadRefRangesData()

                    If (Not inUseCalcTest) Then
                        'Set screen status to Query Mode
                        QueryModeScreenStatus()
                    Else
                        'Set screen status to Read Only Mode 
                        ReadOnlyModeScreenStatus()
                    End If

                    bsCalTestListView.Focus()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CancelCalTestEdition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CancelCalTestEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the Global Variables used in the screen to store the selected Calculated Test
    ''' and/or to control when there are changes pending to save
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub CleanGlobalValues()
        Try
            'Initialization of global variables
            selectedCalTestID = 0
            originalSelectedIndex = -1
            originalCalTestName = ""
            originalFormulaValues = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CleanGlobalValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CleanGlobalValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Delete all selected Calculated Tests
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              TR 12/01/2010 - Call the ShowMessage method directly to shown the Delete Confirmation Message and verify the User's answer
    '''              SA 21/06/2010 - Functionality for multiple deletion was bad implemented, code has been changed to fix errors.
    '''                              Control of Concurrency Error has been removed
    '''              TR 25/11/2010 - Before deleting, verify if there are affected elements to shown the warning auxiliary pop up
    '''              SA 17/12/2010 - Deletion of Reference Ranges has been removed from here; it is executed now in the Delegate function
    ''' </remarks>
    Private Sub DeleteCalculatedTest()
        Dim returnedData As New GlobalDataTO

        Try
            If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                'Get the current User from the Appliction Session
                Dim currentSession As New ApplicationSessionManager
                Dim loggedUser As String = currentSession.GetSessionInfo().UserName

                'Load all selected Calculated Tests in a typed DataSet CalculatedTestsDS
                Dim calTestDataDS As New CalculatedTestsDS
                Dim calTestRow As CalculatedTestsDS.tparCalculatedTestsRow

                For Each mySelectedItem As ListViewItem In bsCalTestListView.SelectedItems
                    calTestRow = calTestDataDS.tparCalculatedTests.NewtparCalculatedTestsRow

                    calTestRow.CalcTestID = CInt(mySelectedItem.Name)
                    calTestRow.CalcTestName = mySelectedItem.Text
                    calTestRow.TS_User = loggedUser
                    calTestRow.TS_DateTime = Now
                    calTestDataDS.tparCalculatedTests.Rows.Add(calTestRow)
                Next

                If (calTestDataDS.tparCalculatedTests.Rows.Count > 0) Then
                    'Verify if there other elements affected for the deletion ...
                    If (ValidateDependenciesOnDeletedElements(calTestDataDS) = Windows.Forms.DialogResult.OK) Then
                        Dim calTestToDelete As New CalculatedTestsDelegate
                        returnedData = calTestToDelete.Delete(Nothing, calTestDataDS)

                        If (Not returnedData.HasError) Then
                            'Every time the list of Calculated Tests is reloaded, the correspondent list in the Formula Control
                            'is also reloaded
                            LoadCalculatedTests()

                            'Refresh the list of Calulated Test and set screen status to Initial Mode
                            LoadCalculatedTestList()
                            InitialModeScreenStatus()

                            If (bsCalTestListView.Items.Count > 0) Then
                                'Select the first Calculated Test in the list
                                bsCalTestListView.Items(0).Selected = True
                                QueryCalculatedTest()
                                bsCalTestListView.Focus()
                            End If
                        Else
                            'Error deleting the selected Calculated Tests, show the error message
                            ShowMessage(Me.Name & ".DeleteCalculatedTest", returnedData.ErrorCode, returnedData.ErrorMessage)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteCalculatedTest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteCalculatedTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the Edit Button is clicked, set the screen status to EDIT MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              SA 16/12/2010 - Changes due new implementation of Reference Ranges control
    ''' </remarks>
    Private Sub EditCalTestByButtonClick()
        Try
            EditionMode = True

            bsCalTestFormula.EnabledStatus = True
            myOriginalFormulaValue = bsCalTestFormula.OriginalFormulaValue

            If (Not bsNameTextbox.Enabled) Then
                'Set screen status to Edit Mode
                EditModeScreenStatus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditCalTestByButtonClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditCalTestByButtonClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When double clicking in a Calculated Test in the ListView, verify if there are changes pending 
    ''' to save and set the screen status to EDIT MODE if it is not InUse, or to READ ONLY MODE if it 
    ''' is InUse in the active Work Session
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              TR 12/01/2010 - Calls the ShowMessage method directly and validate the User's answer
    '''              SA 16/12/2010 - Changes due new implementation of Reference Ranges control
    ''' </remarks>
    Private Sub EditCalTestByDoubleClick()
        Dim setScreenToEdition As Boolean = False
        Try
            EditionMode = True
            If (bsCalTestListView.SelectedIndices(0) <> originalSelectedIndex) Then
                If (PendingChangesVerification()) Then
                    If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                        SelectedTestRefRangesDS.Clear()
                        setScreenToEdition = True
                    Else
                        If (originalSelectedIndex <> -1) Then
                            'Return focus to the Calculted Test that has been edited
                            bsCalTestListView.Items(originalSelectedIndex).Selected = True
                            bsCalTestListView.Select()
                        End If
                    End If
                Else
                    setScreenToEdition = True
                End If
            Else
                'If the screen was in Read Only Mode, it is changed to Edit Mode when the User
                'double-clicking in the same Calulated Test 
                If (Not bsNameTextbox.Enabled) Then setScreenToEdition = True 'GDS - 14/05/2010
            End If

            If (setScreenToEdition) Then
                'Load screen fields with all data of the selected Calculated Test Formula
                Dim inUseCalcTest As Boolean = LoadDataOfCalculatedTestFormula()

                'Get the Reference Ranges defined for the Calculated Test and shown them
                GetRefRanges(selectedCalTestID)
                LoadRefRangesData()

                If (Not inUseCalcTest) Then
                    'Set screen status to Query Mode
                    EditModeScreenStatus()
                Else
                    'Set screen status to Read Only Mode 
                    ReadOnlyModeScreenStatus()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditCalTestByDoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditCalTestByDoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to EDIT MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic 
    '''                              function ShowMessage
    ''' </remarks>
    Private Sub EditModeScreenStatus()
        Try
            'Area of Calculated Test List
            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = False 'AG 05/09/2014 - BA-1869
            'bsPrintButton.Enabled = False DL 11/05/2012

            'Area of Caculated Test Definition
            bsFullNameTextbox.Enabled = True
            bsFullNameTextbox.BackColor = Color.White
            bsNameTextbox.Enabled = True
            bsNameTextbox.BackColor = Color.White

            bsSampleComboBox.Enabled = True
            bsSampleComboBox.BackColor = Color.White
            bsSampleComboBox.Enabled = bsUniqueRadioButton.Checked

            bsUnitComboBox.Enabled = True
            bsUnitComboBox.BackColor = Color.White
            bsDecimalsNumericUpDown.Enabled = True
            bsDecimalsNumericUpDown.BackColor = Color.White

            bsUniqueRadioButton.Enabled = True
            bsMultipleRadioButton.Enabled = True
            bsPrintExpTestCheckbox.Enabled = True

            bsCalTestFormula.EnableDisableControls = True
            bsTestRefRanges.isEditing = True

            bsSaveButton.Enabled = True
            bsCancelButton.Enabled = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
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
                    If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.No) Then
                        screenClose = False
                    End If
                End If
            End If

            If (screenClose) Then
                'TR 11/04/2012 -Disable form on close to avoid any button press
                Me.Enabled = False
                If (Not Me.Tag Is Nothing) Then
                    'A PerformClick() method was executed
                    Me.Close()
                Else
                    'Normal button click - Open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
            Else
                bsFullNameTextbox.Focus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
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
                ShowMessage(Me.Name & ".GetAgeUnits", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetAgeUnits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetAgeUnits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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
                ShowMessage(Me.Name & ".GetDetailedRangesSubTypes", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetDetailedRangesSubTypes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetDetailedRangesSubTypes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Get multilanguage labels needed for the Formula User Control
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 07/07/2010
    ''' Modified by: PG 07/10/2010 - Added a LanguageID Parameter to get the multilanguage text of 
    '''                              each label.
    '''              TR 09/03/2011 - Set the Factory Calib value message.
    ''' </remarks>
    Private Sub GetFormulaLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsCalTestFormula.FormulaTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_Formula", pLanguageID) & ":"
            bsCalTestFormula.SampleTypeTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID) & ":"
            bsCalTestFormula.TestsTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", pLanguageID) & ":"
            bsCalTestFormula.CalculatedTestsTitle = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_Long", pLanguageID) & ":"

            bsCalTestFormula.DelFormulaMemberToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_DelLastMember", pLanguageID)
            bsCalTestFormula.ClearFormulaToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CalcTests_ClearFormula", pLanguageID)

            bsCalTestFormula.FactoryValueMessage = GetMessageText(GlobalEnumerates.Messages.FACTORY_VALUES.ToString(), pLanguageID) 'TR 09/03/2011

            ' WE 07/11/2014 - RQ00035C (BA-1867) - Set the Product Name as Factory values message caption (Not to be translated!!!)
            bsCalTestFormula.FactoryValueCaption = My.Application.Info.Title

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetFormulaLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetFormulaLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

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
                ShowMessage(Me.Name & ".GetGenders", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetGenders ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetGenders", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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
                ShowMessage(Me.Name & ".GetLimits", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetLimits ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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
                ShowMessage(Me.Name & ".GetMessages", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetMessages", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetMessages", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Gets the Reference Ranges defined for the specified Calculated Test
    ''' </summary>
    ''' <param name="pCalcTestID">Calculated Test Identifier</param>
    ''' <remarks>
    ''' Created by: SG 02/09/2010
    ''' </remarks>
    Private Sub GetRefRanges(ByVal pCalcTestID As Integer)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestRefRangesDelegate As New TestRefRangesDelegate

            myGlobalDataTO = myTestRefRangesDelegate.ReadByTestID(Nothing, pCalcTestID, , , "CALC")
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                SelectedTestRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)
            Else
                'Error getting the Reference Ranges defined for the selected Calculated Test; shown it
                SelectedTestRefRangesDS = New TestRefRangesDS
                ShowMessage(Me.Name & ".GetRefRanges", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetRefRanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetRefRanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application</param>
    ''' <remarks>
    ''' Created by:  PG 07/10/10
    ''' Modified by: SA 09/12/2010 - Changes due to new implementation of Reference Ranges control
    '''              AG 05/09/2014 - BA-1869 ==> Added ToolTip for new button used to open the auxiliary screen that allow sort and set the  
    '''                                          availability of CALC Tests (Custom Order Button)
    '''              SA 17/11/2014 - BA-2125 ==> Added ToolTip for new button used to open the auxiliary screen that allow sort and set the  
    '''                                          availability of CALC Tests (Custom Order Button) - previous change was not really done; code 
    '''                                          was commented and the label was not the correct one. Commented code to get ToolTip for Print 
    '''                                          Button due to it is not visible.
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsCalTestDefLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_CalcTests_Definition", pLanguageID)
            bsCalcTestListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_CalcTests_List", pLanguageID)
            bsDecimalsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Decimals", pLanguageID) + ":"
            FormulaTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_FormulaDef", pLanguageID)
            bsMultipleRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_MultiSampleType", pLanguageID)
            bsFullNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", pLanguageID) + ":"
            bsPrintExpTestCheckbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_PrintExpTests", pLanguageID)
            RefRangesTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Long", pLanguageID)
            bsSampleGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", pLanguageID)
            bsNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ShortName", pLanguageID) + ":"
            bsUniqueRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcTests_UniqueSampleType", pLanguageID)
            bsUnitLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", pLanguageID) + ":"

            'For Tooltips
            bsScreenToolTips.SetToolTip(bsNewButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", pLanguageID))
            bsScreenToolTips.SetToolTip(bsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            bsScreenToolTips.SetToolTip(BsCustomOrderButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TEST_SORTING_SELECTION", pLanguageID))
            'bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))

            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", pLanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))

            'For bsTestRefRanges
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
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure and initialize the ListView of Calculated Tests
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              PG 08/10/2010 - Added the LanguageID parameter and the multilanguage text for the list header title
    ''' </remarks>
    Private Sub InitializeCalculatedTestList(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Initialization of Calculated Tests List
            bsCalTestListView.Items.Clear()

            bsCalTestListView.Alignment = ListViewAlignment.Left
            bsCalTestListView.FullRowSelect = True
            bsCalTestListView.MultiSelect = True
            bsCalTestListView.Scrollable = True
            bsCalTestListView.View = View.Details
            bsCalTestListView.HideSelection = False
            bsCalTestListView.HeaderStyle = ColumnHeaderStyle.Clickable

            'List columns definition  --> only column containing the Calculated Test Name will be visible
            bsCalTestListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestNames", pLanguageID), -2, HorizontalAlignment.Left)

            bsCalTestListView.Columns.Add("CalcTestID", 0, HorizontalAlignment.Left)
            bsCalTestListView.Columns.Add("SampleType", 0, HorizontalAlignment.Left)
            bsCalTestListView.Columns.Add("CalcTestLongName", 0, HorizontalAlignment.Left)
            bsCalTestListView.Columns.Add("MeasureUnit", 0, HorizontalAlignment.Left)
            bsCalTestListView.Columns.Add("UniqueSampleType", 0, HorizontalAlignment.Left)
            bsCalTestListView.Columns.Add("Decimals", 0, HorizontalAlignment.Left)
            bsCalTestListView.Columns.Add("PrintExpTests", 0, HorizontalAlignment.Left)
            bsCalTestListView.Columns.Add("FormulaText", 0, HorizontalAlignment.Left)
            bsCalTestListView.Columns.Add("InUse", 0, HorizontalAlignment.Left)
            bsCalTestListView.Columns.Add("EnableStatus", 0, HorizontalAlignment.Left)
            bsCalTestListView.Columns.Add("ActiveRangeType", 0, HorizontalAlignment.Left)

            'Fill ListView with the list of existing Calculated Test.
            LoadCalculatedTestList()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeCalculatedTestList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeCalculatedTestList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initialize properties of the Formula User Control
    ''' </summary>
    ''' <remarks>
    ''' Createb by:  SA 07/07/2010
    ''' </remarks>
    Private Sub InitializeFormula()
        Try
            bsCalTestFormula.CalcTestID = 0
            bsCalTestFormula.SelectedSampleType = Nothing
            bsCalTestFormula.TestSampleTypeList = Nothing
            bsCalTestFormula.TestCalculatedSampleTypeList = Nothing
            bsCalTestFormula.FormulaValuesList = Nothing
            bsCalTestFormula.GenerateFormula = String.Empty
            bsCalTestFormula.GenerateFormulaValueList = False
            bsCalTestFormula.EditionMode = False
            bsCalTestFormula.ShowErrorProvider(String.Empty)
            bsCalTestFormula.DecimalSeparator = SystemInfoManager.OSDecimalSeparator
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeFormula", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeFormula", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
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
            'Control will be shown using the "big" layout and used for Calculated Tests
            bsTestRefRanges.SmallLayout = False
            bsTestRefRanges.TestType = "CALC"

            'Load the necessary data to the Reference Ranges User Control
            Dim myAllFieldLimitsDS As FieldLimitsDS = GetLimits()
            Dim myGendersMasterDataDS As PreloadedMasterDataDS = GetGenders()
            Dim myAgeUnitsMasterDataDS As PreloadedMasterDataDS = GetAgeUnits()
            Dim myDetailedRangeSubTypesDS As PreloadedMasterDataDS = GetDetailedRangesSubTypes()
            Dim myAllMessagesDS As MessagesDS = GetMessages()

            bsTestRefRanges.LoadFrameworkData(myAllFieldLimitsDS, myGendersMasterDataDS, myAgeUnitsMasterDataDS, _
                                              myDetailedRangeSubTypesDS, myAllMessagesDS, SystemInfoManager.OSDecimalSeparator)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeReferenceRangesControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeReferenceRangesControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic 
    '''                              function ShowMessage
    '''              SG 02/09/2010 - Added code to initialize tab of Reference Ranges
    '''              SA 16/12/2010 - Changes due new implementation of Reference Ranges control
    ''' </remarks>
    Private Sub InitialModeScreenStatus(Optional ByVal pInitializeListView As Boolean = True)
        Try
            If (pInitializeListView) Then
                'Area of Calculated Test List
                If (bsCalTestListView.Items.Count > 0) Then
                    bsCalTestListView.Enabled = True
                    bsCalTestListView.SelectedItems.Clear()
                End If

                bsNewButton.Enabled = True
                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
                BsCustomOrderButton.Enabled = False 'AG 05/09/2014 - BA-1869
                'bsPrintButton.Enabled = False DL 11/05/2012
            End If

            'Area of Calculated Test Definition
            bsFullNameTextbox.Text = ""
            bsFullNameTextbox.Enabled = False
            bsFullNameTextbox.BackColor = SystemColors.MenuBar

            bsNameTextbox.Text = ""
            bsNameTextbox.Enabled = False
            bsNameTextbox.BackColor = SystemColors.MenuBar

            bsUniqueRadioButton.Checked = False
            bsMultipleRadioButton.Checked = False
            bsUniqueRadioButton.Enabled = False
            bsMultipleRadioButton.Enabled = False

            bsSampleComboBox.SelectedIndex = 0 '-1
            bsSampleComboBox.Enabled = False
            bsSampleComboBox.BackColor = SystemColors.MenuBar

            bsUnitComboBox.SelectedIndex = 0 '-1
            bsUnitComboBox.Enabled = False
            bsUnitComboBox.BackColor = SystemColors.MenuBar

            bsDecimalsNumericUpDown.Value = Me.bsDecimalsNumericUpDown.Minimum
            bsDecimalsNumericUpDown.Enabled = False
            bsDecimalsNumericUpDown.BackColor = SystemColors.MenuBar

            bsPrintExpTestCheckbox.Checked = False
            bsPrintExpTestCheckbox.Enabled = False

            'Area of Formula definition
            InitializeFormula()
            bsCalTestFormula.EnableDisableControls = False

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
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitialModeScreenStatus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitialModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the list of Calculated Tests in the Formula User Control
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 13/05/2010 
    ''' Modified by: SA 21/06/2010 - Added error control after calls to functions in Delegate Classes
    ''' </remarks>
    Private Sub LoadCalculatedTests()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myCalTestList As New CalculatedTestsDelegate

            myGlobalDataTO = myCalTestList.GetAllowedTestList(Nothing, "CALC")
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim allowedTestDataDS As New AllowedTestsDS
                allowedTestDataDS = DirectCast(myGlobalDataTO.SetDatos, AllowedTestsDS)

                For Each allowedTest As AllowedTestsDS.tparAllowedTestsRow In allowedTestDataDS.tparAllowedTests.Rows
                    allowedTest.BeginEdit()
                    allowedTest.IconPath = MyBase.IconsPath & allowedTest.IconPath
                    allowedTest.EndEdit()
                Next

                If (allowedTestDataDS.tparAllowedTests.Rows.Count > 0) Then
                    bsCalTestFormula.TestCalculatedList = allowedTestDataDS
                End If
            Else
                'Error getting the list of allowed Calculated Tests, show the error message
                ShowMessage(Me.Name & ".LoadCalculatedTests", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadCalculatedTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadCalculatedTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill Calculated Test basic data in the correspondent structure
    ''' </summary>
    ''' <returns>A typed DataSet CalculatedTestsDS containing all data of the Calculated Test</returns>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic 
    '''                              function ShowMessage
    '''              SA 16/12/2010 - Get also the selected type of Reference Ranges
    ''' </remarks>
    Private Function LoadCalculatedTestDS() As CalculatedTestsDS
        Dim calTestRow As CalculatedTestsDS.tparCalculatedTestsRow
        Dim calTestData As New CalculatedTestsDS

        Try
            calTestRow = calTestData.tparCalculatedTests.NewtparCalculatedTestsRow

            calTestRow.CalcTestID = selectedCalTestID
            calTestRow.CalcTestName = bsNameTextbox.Text.Trim
            calTestRow.CalcTestLongName = bsFullNameTextbox.Text.Trim
            calTestRow.MeasureUnit = bsUnitComboBox.SelectedValue.ToString

            If (Me.bsUniqueRadioButton.Checked) Then
                calTestRow.UniqueSampleType = Me.bsUniqueRadioButton.Checked
                calTestRow.SampleType = bsSampleComboBox.SelectedValue.ToString
            Else
                calTestRow.UniqueSampleType = Me.bsUniqueRadioButton.Checked
                calTestRow.SampleType = ""
            End If

            calTestRow.Decimals = Convert.ToInt32(bsDecimalsNumericUpDown.Value)
            calTestRow.PrintExpTests = Me.bsPrintExpTestCheckbox.Checked
            calTestRow.EnableStatus = True

            If (calTestRow.EnableStatus) Then
                calTestRow.FormulaText = bsCalTestFormula.FormulaString.Replace("'", "''").Replace(",", ".") 'IT 08/10/2014: BA-1991
            Else
                calTestRow.FormulaText = myOriginalFormulaValue.Replace("'", "''").Replace(",", ".") 'IT 08/10/2014: BA-1991
            End If

            'If Reference Ranges have been defined, get value of the selected Range Type
            Dim selectedRangeType As String = bsTestRefRanges.ActiveRangeType
            If (selectedRangeType = String.Empty) Then
                calTestRow.SetActiveRangeTypeNull()
            Else
                calTestRow.ActiveRangeType = selectedRangeType
            End If

            'Gets from the Session the Username of the connected User; get also the current datetime
            Dim currentSession As New ApplicationSessionManager
            calTestRow.TS_User = currentSession.GetSessionInfo().UserName
            calTestRow.TS_DateTime = Now

            calTestData.tparCalculatedTests.Rows.Add(calTestRow)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadCalculatedTestDS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadCalculatedTestDS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return calTestData
    End Function

    ''' <summary>
    ''' Load the ListView of Calculated Test with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic 
    '''                              function ShowMessage
    '''              SA 21/06/2010 - Changed the way of get the screen Icons; show message when there is an error
    '''              SG 28/07/2010 - Sort the list of Calculated Tests depending its sorting currently active 
    '''              SA 30/07/2010 - Changed the way of get the ListView Icons and load them in the ImageList
    '''              SG 03/09/2010 - Load field ActiveRangeType as hide column in the ListView
    ''' </remarks>
    Private Sub LoadCalculatedTestList()
        Try
            Dim myIcons As New ImageList

            'Get the Icon defined for Calculated Tests that are not in use in the current Work Session
            Dim notInUseIcon As String = GetIconName("TCALC")
            If (notInUseIcon <> "") Then
                myIcons.Images.Add("TCALC", Image.FromFile(MyBase.IconsPath & notInUseIcon))
            End If

            'Get the Icon defined for Calculated Tests that are not in use in the current Work Session
            Dim inUseIcon As String = GetIconName("INUSETCALC")
            If (inUseIcon <> "") Then
                myIcons.Images.Add("INUSETCALC", Image.FromFile(MyBase.IconsPath & inUseIcon))
            End If

            'Assign the Icons to the Calculated Tests List View
            bsCalTestListView.Items.Clear()
            bsCalTestListView.SmallImageList = myIcons

            'Get the list of existing Calculated Tests
            Dim resultData As New GlobalDataTO
            Dim calTestList As New CalculatedTestsDelegate

            resultData = calTestList.GetList(Nothing)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim calTestDS As New CalculatedTestsDS
                calTestDS = DirectCast(resultData.SetDatos, CalculatedTestsDS)

                'Sort the returned Calculated Tests
                Dim qCalcTests As List(Of CalculatedTestsDS.tparCalculatedTestsRow)
                Select Case bsCalTestListView.Sorting
                    Case SortOrder.Ascending
                        qCalcTests = (From a In calTestDS.tparCalculatedTests _
                                    Select a Order By a.CalcTestLongName Ascending).ToList()
                    Case SortOrder.Descending
                        qCalcTests = (From a In calTestDS.tparCalculatedTests _
                                    Select a Order By a.CalcTestLongName Descending).ToList()
                    Case SortOrder.None
                        qCalcTests = (From a In calTestDS.tparCalculatedTests _
                                    Select a).ToList()
                    Case Else
                        qCalcTests = (From a In calTestDS.tparCalculatedTests _
                                    Select a).ToList()
                End Select

                'Fill the List View with the list of sorted Calculated Tests
                Dim i As Integer = 0
                Dim rowToSelect As Integer = -1

                For Each calculatedTest As CalculatedTestsDS.tparCalculatedTestsRow In qCalcTests
                    bsCalTestListView.Items.Add(calculatedTest.CalcTestID.ToString, _
                                                calculatedTest.CalcTestLongName.ToString, _
                                                calculatedTest.IconPath.ToString).Tag = calculatedTest.InUse

                    bsCalTestListView.Items(i).SubItems.Add(calculatedTest.CalcTestID.ToString)
                    bsCalTestListView.Items(i).SubItems.Add(calculatedTest.SampleType.ToString)
                    bsCalTestListView.Items(i).SubItems.Add(calculatedTest.CalcTestName.ToString)
                    bsCalTestListView.Items(i).SubItems.Add(calculatedTest.MeasureUnit.ToString)
                    bsCalTestListView.Items(i).SubItems.Add(calculatedTest.UniqueSampleType.ToString)
                    bsCalTestListView.Items(i).SubItems.Add(calculatedTest.Decimals.ToString)
                    bsCalTestListView.Items(i).SubItems.Add(calculatedTest.PrintExpTests.ToString)
                    bsCalTestListView.Items(i).SubItems.Add(calculatedTest.FormulaText.ToString)
                    bsCalTestListView.Items(i).SubItems.Add(calculatedTest.InUse.ToString)
                    bsCalTestListView.Items(i).SubItems.Add(calculatedTest.EnableStatus.ToString)

                    If (calculatedTest.IsActiveRangeTypeNull) Then
                        bsCalTestListView.Items(i).SubItems.Add("")
                    Else
                        bsCalTestListView.Items(i).SubItems.Add(calculatedTest.ActiveRangeType)
                    End If

                    'If there is a selected Calculated Test and it is still in the list, its position is stored to re-select 
                    'the same Calculated Test once the list is loaded
                    If (selectedCalTestID = CLng(calculatedTest.CalcTestID)) Then rowToSelect = i
                    i += 1
                Next

                If (rowToSelect = -1) Then
                    'There was not a selected Calculated Test or the selected one is not in the list; the global variables containing 
                    'information of the selected Calculated Test is initializated
                    CleanGlobalValues()
                Else
                    'If there is a selected Calculated Test, focus is put in the correspondent element in the Test Calculated List
                    bsCalTestListView.Items(rowToSelect).Selected = True
                    bsCalTestListView.Select()

                    'The global variable containing the index of the selected Calculated Test is updated
                    originalSelectedIndex = bsCalTestListView.SelectedIndices(0)
                End If
            End If

            'An error has happened getting data from the Database
            If (resultData.HasError) Then
                ShowMessage(Me.Name & ".LoadCalculatedTestList", resultData.ErrorCode, resultData.ErrorMessage)

                CleanGlobalValues()
                bsCalTestListView.Enabled = False
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadCalculatedTestList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadCalculatedTestList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill screen fields with data of the selected Calculated Test
    ''' </summary>
    ''' <returns>True if the Calculated Test is In Use; otherwise False</returns>
    ''' <remarks>
    ''' Modified by: BK  29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              GDS 17/05/2010 - Changed from Sub to Function to return value of inUse flag for the selected Calculated Test
    ''' </remarks>
    Private Function LoadDataOfCalculatedTestFormula() As Boolean
        Dim inUse As Boolean = False

        Try
            selectedCalTestID = CInt(bsCalTestListView.SelectedItems(0).SubItems(1).Text)
            originalCalTestName = bsCalTestListView.SelectedItems(0).SubItems(0).Text
            originalSelectedIndex = bsCalTestListView.SelectedIndices(0)

            'Fill screen controls with data of the selected Calculated Test
            bsFullNameTextbox.Text = bsCalTestListView.SelectedItems(0).SubItems(0).Text
            bsNameTextbox.Text = bsCalTestListView.SelectedItems(0).SubItems(3).Text

            If (bsCalTestListView.SelectedItems(0).SubItems(2).Text <> "") Then
                bsSampleComboBox.SelectedValue = bsCalTestListView.SelectedItems(0).SubItems(2).Text
            Else
                bsSampleComboBox.SelectedValue = bsSampleComboBox.Items(0).ToString
            End If

            bsUnitComboBox.SelectedValue = bsCalTestListView.SelectedItems(0).SubItems(4).Text
            bsDecimalsNumericUpDown.Value = Convert.ToDecimal(bsCalTestListView.SelectedItems(0).SubItems(6).Text)

            If (Convert.ToBoolean(bsCalTestListView.SelectedItems(0).SubItems(5).Text)) Then
                bsUniqueRadioButton.Checked = True

                bsCalTestFormula.SelectedSampleType = bsCalTestListView.SelectedItems(0).SubItems(2).Text
                bsCalTestFormula.EnableSampleType(False)
            Else
                bsMultipleRadioButton.Checked = True
                bsCalTestFormula.EnableSampleType(True)
            End If

            If (Convert.ToBoolean(bsCalTestListView.SelectedItems(0).SubItems(7).Text)) Then
                bsPrintExpTestCheckbox.Checked = True
            Else
                bsPrintExpTestCheckbox.Checked = False
            End If

            inUse = Convert.ToBoolean(bsCalTestListView.SelectedItems(0).SubItems(9).Text)
            bsCalTestFormula.CalcTestID = selectedCalTestID
            myEnableStatus = Convert.ToBoolean(bsCalTestListView.SelectedItems(0).SubItems(10).Text)

            If (Not myEnableStatus) Then
                bsCalTestFormula.WarningSintaxStatus()
                bsCalTestFormula.FormulaValuesList = Nothing
                bsCalTestFormula.GenerateFormula = bsCalTestListView.SelectedItems(0).SubItems(8).Text
            Else
                'Load members of the Calculated Test Formula
                LoadFormulaValues(selectedCalTestID)
            End If

            'TR 04/09/2012 -Set the original formula values to the global variable.
            myOriginalFormulaValue = bsCalTestFormula.OriginalFormulaValue

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadDataOfCalculatedTestFormula", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadDataOfCalculatedTestFormula", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return inUse
    End Function

    ''' <summary>
    ''' Read the limits defined for field Number of Decimals 
    ''' </summary>
    ''' <remarks>
    ''' Modified by: VR 29/12/2009 - Change the Constant Value To Enum Value
    '''              BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic 
    '''                              function ShowMessage
    '''              SA 21/06/2010 - Added error control after calls to functions in Delegate Classes
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
                    bsDecimalsNumericUpDown.Minimum = CType(fieldDS.tfmwFieldLimits(0).MinValue, Integer)
                    bsDecimalsNumericUpDown.Maximum = CType(fieldDS.tfmwFieldLimits(0).MaxValue, Integer)
                    bsDecimalsNumericUpDown.Value = CType(fieldDS.tfmwFieldLimits(0).DefaultValue, Integer)
                End If
            Else
                'Error getting the limits defined, show the error message
                ShowMessage(Me.Name & ".LoadDecimalsLimit", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadDecimalsLimit", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadDecimalsLimit", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get members of the Formula defined for the specified Calculated Test
    ''' </summary>
    ''' <param name="pCalcTestID">Calculated Test Identifier</param>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              SA 21/06/2010 - To fix some errors: now the name of the standard and calculated Tests that are formula members 
    '''                              for the specified Calculated Test is returned by function GetFormulaValues in FormulaDelegate
    ''' </remarks>
    Private Sub LoadFormulaValues(ByVal pCalcTestID As Integer)
        Try
            Dim resultData As New GlobalDataTO
            Dim formulaData As New FormulasDelegate

            resultData = formulaData.GetFormulaValues(Nothing, pCalcTestID)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                'Get the components of the Formula defined for the Calculated Test
                originalFormulaValues = CType(resultData.SetDatos, FormulasDS)

                'Inform properties of the Formula User Control
                bsCalTestFormula.ShowErrorProvider(String.Empty)
                bsCalTestFormula.FormulaValuesList = originalFormulaValues.tparFormulas.DataSet
                bsCalTestFormula.EditionMode = True
                bsCalTestFormula.GenerateFormulaValueList = True
            Else
                'Error getting the Formula members of the selected Calculated Test
                ShowMessage(Me.Name & ".LoadFormulaValues", resultData.ErrorCode, resultData.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadFormulaValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadFormulaValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBox of Measure Units with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Modified by: VR 29/12/2009 - Changes from Constant Value to Enum Value
    '''              BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic 
    '''                              function ShowMessage
    '''              SA 21/06/2010 - Added error control after calls to functions in Delegate Classes
    ''' </remarks>
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
                    'Fill ComboBox of Measure Units in area of Calculated Test details
                    bsUnitComboBox.DataSource = masterDataDS.tcfgMasterData
                    bsUnitComboBox.DisplayMember = "FixedItemDesc"
                    bsUnitComboBox.ValueMember = "ItemID"
                End If
            Else
                'Error getting the list of defined Measure Units, show the error message
                ShowMessage(Me.Name & ".LoadMeasureUnits", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadMeasureUnits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadMeasureUnits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Shows the Reference Ranges defined for the selected Calculated Test
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 02/09/2010
    ''' Modified by: AG 27/10/2010 - If there are ranges in the DataSet, do not read them from the DB
    '''              SA 16/12/2010 - Changes due to new implementation of Reference Ranges control
    ''' </remarks>
    Private Sub LoadRefRangesData()
        Try
            If (bsCalTestListView.SelectedItems.Count = 1) Then
                bsTestRefRanges.TestID = CInt(bsCalTestListView.SelectedItems(0).SubItems(1).Text)
                bsTestRefRanges.SampleType = ""
                bsTestRefRanges.ActiveRangeType = (bsCalTestListView.SelectedItems(0).SubItems(11).Text)
                bsTestRefRanges.MeasureUnit = bsUnitComboBox.Text.ToString
                bsTestRefRanges.DefinedTestRangesDS = SelectedTestRefRangesDS

                Dim decimalsNumber As Integer = 0
                If (IsNumeric(bsDecimalsNumericUpDown.Text)) Then decimalsNumber = Convert.ToInt32(bsDecimalsNumericUpDown.Text)
                bsTestRefRanges.RefNumDecimals = decimalsNumber

                If (SelectedTestRefRangesDS IsNot Nothing) Then
                    bsTestRefRanges.LoadReferenceRanges()
                Else
                    bsTestRefRanges.ClearReferenceRanges()
                End If

                bsTestRefRanges.isEditing = bsCalTestFormula.Enabled
            Else
                bsTestRefRanges.ClearData() 'TR 13/01/2011 
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadRefRangesData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadRefRangesData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBoxes of Sample Types with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Modified by: VR 29/12/2009 - Changes from Constant Value to Enum Value 
    '''              BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic 
    '''                              function ShowMessage
    '''              SA 21/06/2010 - Added error control after calls to functions in Delegate Classes
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
                    'Fill ComboBox of Sample Types in area of Calculated Test details
                    bsSampleComboBox.DataSource = masterDataDS.tcfgMasterData
                    bsSampleComboBox.DisplayMember = "ItemIDDesc"
                    bsSampleComboBox.ValueMember = "ItemID"

                    'Inform fields Code and Description in the DS to load also the SampleTypes ComboBox 
                    'in the Formula User Control
                    For Each myMasterRow As MasterDataDS.tcfgMasterDataRow In masterDataDS.tcfgMasterData.Rows
                        myMasterRow.BeginEdit()
                        myMasterRow.Code = myMasterRow.ItemID
                        myMasterRow.Description = myMasterRow.ItemIDDesc
                        myMasterRow.EndEdit()
                        myMasterRow.AcceptChanges()
                    Next
                    bsCalTestFormula.SampleTypesList = masterDataDS
                End If
            Else
                'Error getting the list of defined Sample Types, show the error message
                ShowMessage(Me.Name & ".LoadSampleTypesList", myGlobalDataTo.ErrorCode, myGlobalDataTo.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadSampleTypesList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadSampleTypesList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ' ''' <summary>
    ' ''' Load the list of Standard Tests in the Formula User Control   [NOT USED ANYMORE (OBSOLETE) REPLACED BY NEW METHOD LoadTests]
    ' ''' </summary>
    ' ''' <remarks>
    ' ''' Created by:  DL 13/05/2010 
    ' ''' Modified by: SA 21/06/2010 - Added error control after calls to functions in Delegate Classes
    ' ''' </remarks>
    'Private Sub LoadStandardTests()
    '    Try
    '        Dim myGlobalDataTO As New GlobalDataTO
    '        Dim myCalTestList As New CalculatedTestsDelegate

    '        myGlobalDataTO = myCalTestList.GetAllowedTestList(Nothing, "STD")
    '        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
    '            Dim allowedTestDataDS As New AllowedTestsDS
    '            allowedTestDataDS = DirectCast(myGlobalDataTO.SetDatos, AllowedTestsDS)

    '            For Each allowedTest As AllowedTestsDS.tparAllowedTestsRow In allowedTestDataDS.tparAllowedTests.Rows
    '                allowedTest.BeginEdit()
    '                allowedTest.IconPath = MyBase.IconsPath & allowedTest.IconPath
    '                allowedTest.EndEdit()
    '            Next

    '            If (allowedTestDataDS.tparAllowedTests.Rows.Count > 0) Then
    '                bsCalTestFormula.TestList = allowedTestDataDS
    '            End If
    '        Else
    '            'Error getting the list of allowed Standard Tests, show the error message
    '            ShowMessage(Me.Name & ".LoadStandardTests", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
    '        End If
    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadStandardTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".LoadStandardTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub


    ''' <summary>
    ''' Load the list of Tests (Standard, ISE and Off-System) in the Formula User Control
    ''' </summary>
    ''' <remarks>
    ''' Created by:  WE 07/11/2014 - RQ00035C (BA-1867).
    ''' Modified by: SA 21/06/2010 - Added error control after calls to functions in Delegate Classes
    ''' </remarks>
    Private Sub LoadTests()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myCalTestList As New CalculatedTestsDelegate

            'myGlobalDataTO = myCalTestList.GetAllowedTestList(Nothing, "STD")
            myGlobalDataTO = myCalTestList.GetAllowedTestList(Nothing, "STD_ISE_OFFS")
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim allowedTestDataDS As New AllowedTestsDS
                allowedTestDataDS = DirectCast(myGlobalDataTO.SetDatos, AllowedTestsDS)

                For Each allowedTest As AllowedTestsDS.tparAllowedTestsRow In allowedTestDataDS.tparAllowedTests.Rows
                    allowedTest.BeginEdit()
                    allowedTest.IconPath = MyBase.IconsPath & allowedTest.IconPath
                    allowedTest.EndEdit()
                Next

                If (allowedTestDataDS.tparAllowedTests.Rows.Count > 0) Then
                    bsCalTestFormula.TestList = allowedTestDataDS
                End If
            Else
                'Error getting the list of allowed Tests, show the error message
                ShowMessage(Me.Name & ".LoadTests", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub




    ''' <summary>
    ''' Verify if there is at least one User's change pending to save
    ''' </summary>
    ''' <returns>True if there are changes pending to save; otherwise, False</returns>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              AG 02/11/2010 - If SampleType is changed from Unique to Multiple (or the opposite), mark also flag of pending changes
    '''              SA 16/12/2010 - Changes due to new implementation of Reference Ranges control
    ''' </remarks>
    Public Function PendingChangesVerification() As Boolean
        Dim pendingToSaveChanges As Boolean = False

        Try
            If (EditionMode) Then
                'When changes have been made in the Test Reference Ranges or there are errors still unsolved
                If (bsTestRefRanges.ChangesMade Or bsTestRefRanges.ValidationError) Then
                    pendingToSaveChanges = True
                    SelectedTestRefRangesDS = DirectCast(bsTestRefRanges.DefinedTestRangesDS, TestRefRangesDS) 'bsTestRefRanges.DefinedTestRangesDS

                ElseIf (selectedCalTestID = 0) Then ' Or bsUnitComboBox.SelectedIndex <> -1 
                    'In Add Mode, if at least one of the default values has been changed, then there are changes pending to save
                    If (bsFullNameTextbox.Text.Trim <> "" Or bsNameTextbox.Text.Trim <> "" Or _
                        bsMultipleRadioButton.Checked Or _
                        Convert.ToInt32(bsDecimalsNumericUpDown.Value) <> 0 Or bsPrintExpTestCheckbox.Checked Or _
                        bsCalTestFormula.FormulaString <> "") Then
                        pendingToSaveChanges = True
                    End If

                Else
                    If (bsCalTestListView.SelectedItems.Count = 0) Then
                        'If there is a Calculated Test in edition and a click is made out of the list of Tests,
                        'the Test in edition is selected to avoid errors if the screen is closed or the edition cancelled
                        If (originalSelectedIndex > 0) Then
                            'Return focus to the Calculated Test that has been edited
                            bsCalTestListView.Items(originalSelectedIndex).Selected = True
                            bsCalTestListView.Select()
                        End If
                    End If

                    'SampleType changed from Unique to Multiple or from Multiple to Unique
                    If (bsUniqueRadioButton.Checked <> Convert.ToBoolean(bsCalTestListView.Items(originalSelectedIndex).SubItems(5).Text)) Then
                        pendingToSaveChanges = True

                    ElseIf (Not originalFormulaValues Is Nothing) Then ' AndAlso originalFormulaValues.tparFormulas.Rows.Count > 0) Then
                        Dim selectedSample As String = ""
                        If (Not bsSampleComboBox.SelectedValue Is Nothing) Then
                            selectedSample = bsSampleComboBox.SelectedValue.ToString
                        End If

                        'In Edit Mode, loading values are compared against current values
                        If (bsCalTestListView.SelectedIndices(0) = originalSelectedIndex) Then
                            If (bsFullNameTextbox.Text.Trim <> originalCalTestName) Or _
                               (bsUniqueRadioButton.Checked AndAlso selectedSample <> bsCalTestListView.SelectedItems(0).SubItems(2).Text) Or _
                               (bsNameTextbox.Text.Trim <> bsCalTestListView.SelectedItems(0).SubItems(3).Text) Or _
                               (bsUniqueRadioButton.Checked <> CBool(bsCalTestListView.SelectedItems(0).SubItems(5).Text)) Or _
                               (CDbl(bsDecimalsNumericUpDown.Value) <> CDbl(bsCalTestListView.SelectedItems(0).SubItems(6).Text)) Or _
                               (bsPrintExpTestCheckbox.Checked <> CBool(bsCalTestListView.SelectedItems(0).SubItems(7).Text)) Then
                                'If (bsFullNameTextbox.Text.Trim <> originalCalTestName) Or _
                                '    (bsUniqueRadioButton.Checked AndAlso selectedSample <> bsCalTestListView.SelectedItems(0).SubItems(2).Text) Or _
                                '    (bsNameTextbox.Text.Trim <> bsCalTestListView.SelectedItems(0).SubItems(3).Text) Or _
                                '    (bsUnitComboBox.SelectedValue.ToString <> bsCalTestListView.SelectedItems(0).SubItems(4).Text) Or _
                                '    (bsUniqueRadioButton.Checked <> CBool(bsCalTestListView.SelectedItems(0).SubItems(5).Text)) Or _
                                '    (CDbl(bsDecimalsNumericUpDown.Value) <> CDbl(bsCalTestListView.SelectedItems(0).SubItems(6).Text)) Or _
                                '    (bsPrintExpTestCheckbox.Checked <> CBool(bsCalTestListView.SelectedItems(0).SubItems(7).Text)) Then
                                pendingToSaveChanges = True
                            Else
                                bsCalTestFormula.GenerateFormulaValueList = True
                                If (bsCalTestFormula.FormulaString <> bsCalTestListView.SelectedItems(0).SubItems(8).Text) Then
                                    pendingToSaveChanges = True
                                End If
                            End If

                        Else
                            If (bsFullNameTextbox.Text.Trim <> originalCalTestName) Or _
                               (bsUniqueRadioButton.Checked AndAlso selectedSample <> bsCalTestListView.Items(originalSelectedIndex).SubItems(2).Text) Or _
                               (bsNameTextbox.Text.Trim <> bsCalTestListView.Items(originalSelectedIndex).SubItems(3).Text) Or _
                               (bsUniqueRadioButton.Checked <> CBool(bsCalTestListView.Items(originalSelectedIndex).SubItems(5).Text)) Or _
                               (bsDecimalsNumericUpDown.Value <> CDbl(bsCalTestListView.Items(originalSelectedIndex).SubItems(6).Text)) Or _
                               (bsPrintExpTestCheckbox.Checked <> CBool(bsCalTestListView.Items(originalSelectedIndex).SubItems(7).Text)) Then
                                'If (bsFullNameTextbox.Text.Trim <> originalCalTestName) Or _
                                '(bsUniqueRadioButton.Checked AndAlso selectedSample <> bsCalTestListView.Items(originalSelectedIndex).SubItems(2).Text) Or _
                                '(bsNameTextbox.Text.Trim <> bsCalTestListView.Items(originalSelectedIndex).SubItems(3).Text) Or _
                                '(bsUnitComboBox.SelectedValue.ToString <> bsCalTestListView.Items(originalSelectedIndex).SubItems(4).Text) Or _
                                '(bsUniqueRadioButton.Checked <> CBool(bsCalTestListView.Items(originalSelectedIndex).SubItems(5).Text)) Or _
                                '(bsDecimalsNumericUpDown.Value <> CDbl(bsCalTestListView.Items(originalSelectedIndex).SubItems(6).Text)) Or _
                                '(bsPrintExpTestCheckbox.Checked <> CBool(bsCalTestListView.Items(originalSelectedIndex).SubItems(7).Text)) Then
                                pendingToSaveChanges = True
                            Else
                                bsCalTestFormula.GenerateFormulaValueList = True
                                'DL 09/05/2013. Check system decimal separator
                                'If (bsCalTestFormula.FormulaString <> bsCalTestListView.Items(originalSelectedIndex).SubItems(8).Text) Then
                                If (bsCalTestFormula.FormulaString <> bsCalTestListView.Items(originalSelectedIndex).SubItems(8).Text.Replace(".", SystemInfoManager.OSDecimalSeparator)) Then
                                    pendingToSaveChanges = True
                                End If
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
    ''' Method incharge of loading the image for each button
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 11/06/2010
    ''' Modified by: SG 03/09/2010 - Get image for Delete Button in the Reference Ranges User Control
    '''              DL 03/11/2010 - Load icon in Image Property instead of in BackGroundImage one
    '''              SA 09/12/2010 - Load image for Add Button in the Reference Ranges User Control; code to get icons
    '''                              for the status of the Calculated Test Formula moved here from sub ScreenLoad
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim iconPath As String = MyBase.IconsPath
            Dim auxIconName As String = ""

            'NEW Button
            auxIconName = GetIconName("ADD")
            If (auxIconName <> "") Then
                bsNewButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'EDIT Button
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                bsEditButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'DELETE Buttons
            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                bsDeleteButton.Image = Image.FromFile(iconPath & auxIconName)
                bsTestRefRanges.DeleteButtonImage = Image.FromFile(iconPath & auxIconName)
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = Image.FromFile(iconPath & auxIconName)
            End If
            'JB 30/08/2012 - Hide Print button
            bsPrintButton.Visible = False

            'CUSTOM SORT Button 'AG 05/09/2014 - BA-1869
            auxIconName = GetIconName("ORDER_TESTS")
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

            'CLOSE Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'Buttons for status of the Calculated Test Formula
            auxIconName = GetIconName("ACCEPTF")
            If (auxIconName <> "") Then
                bsCalTestFormula.CheckImage = iconPath & auxIconName
            End If

            auxIconName = GetIconName("CANCELF")
            If (auxIconName <> "") Then
                bsCalTestFormula.CancelImage = iconPath & auxIconName
            End If

            auxIconName = GetIconName("WARNINGF")
            If (auxIconName <> "") Then
                bsCalTestFormula.WarningImage = iconPath & auxIconName
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get data of the selected Calculated Test, fill the correspondent variables and controls and set the screen status 
    ''' to Read-only Mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              SA 16/12/2010 - Changes due new implementation of Reference Ranges control
    ''' </remarks>
    Private Sub QueryCalculatedTest()
        Dim setScreenToQuery As Boolean = False

        Try
            If (bsCalTestListView.SelectedIndices(0) <> originalSelectedIndex) Then
                If (PendingChangesVerification()) Then
                    If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                        EditionMode = False
                        SelectedTestRefRangesDS.Clear()
                        setScreenToQuery = True
                    Else
                        If (originalSelectedIndex <> -1) Then
                            bsCalTestListView.SelectedItems.Clear()

                            'Return focus to the Calculated Test that has been edited
                            bsCalTestListView.Items(originalSelectedIndex).Selected = True
                            bsCalTestListView.Select()
                        End If
                    End If
                Else
                    SelectedTestRefRangesDS.Clear()
                    setScreenToQuery = True
                End If

                If (setScreenToQuery) Then
                    'Load screen fields with all data of the selected Calculated Test Formula
                    Dim inUseCalcTest As Boolean = LoadDataOfCalculatedTestFormula()

                    'Get the Reference Ranges defined for the Calculated Test and shown them
                    GetRefRanges(selectedCalTestID)
                    LoadRefRangesData()

                    If (Not inUseCalcTest) Then
                        'Set screen status to Query Mode
                        QueryModeScreenStatus()
                    Else
                        'Set screen status to Read Only Mode 
                        ReadOnlyModeScreenStatus()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryCalculatedTest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryCalculatedTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Screen display in query mode when the KeyUp or KeyDown key is pressed
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' SA 19/11/2010 - Include also PageDown and PageUp keys
    ''' </remarks>
    Private Sub QueryCalTestByMoveUpDown()
        ' Private Sub QueryCalTestByMoveUpDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        Try
            'TR 13/10/2011 -Do not validate the press key and send the query
            QueryCalculatedTest()

            'TR 13/10/2011 -Commented
            'Select Case e.KeyCode
            '    Case Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp
            '        QueryCalculatedTest()
            'End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryCalTestByMoveUpDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryCalTestByMoveUpDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to QUERY MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic 
    '''                              function ShowMessage
    ''' </remarks>
    Private Sub QueryModeScreenStatus()
        Try
            bsNewButton.Enabled = True
            bsEditButton.Enabled = True
            bsDeleteButton.Enabled = True
            BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
            'bsPrintButton.Enabled = True  DL 11/05/2012

            bsFullNameTextbox.Enabled = False
            bsFullNameTextbox.BackColor = SystemColors.MenuBar

            bsNameTextbox.Enabled = False
            bsNameTextbox.BackColor = SystemColors.MenuBar

            bsSampleComboBox.Enabled = False
            bsSampleComboBox.BackColor = SystemColors.MenuBar

            bsUnitComboBox.Enabled = False
            bsUnitComboBox.BackColor = SystemColors.MenuBar
            bsDecimalsNumericUpDown.Enabled = False
            bsDecimalsNumericUpDown.BackColor = SystemColors.MenuBar

            bsUniqueRadioButton.Enabled = False
            bsMultipleRadioButton.Enabled = False
            bsPrintExpTestCheckbox.Enabled = False

            bsCalTestFormula.EnableDisableControls = False
            bsTestRefRanges.isEditing = False

            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to READ ONLY MODE (for InUse Calculated Tests)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  GDS 17/05/2010
    ''' </remarks>
    Private Sub ReadOnlyModeScreenStatus()
        Try
            'Set all controls to QUERY MODE
            QueryModeScreenStatus()

            'Disable all buttons that cannot be used in Read Only Mode
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ReadOnlyModeScreenStatus " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReadOnlyModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save (add/update) all data of the Calculated Test: definition, Formula and Reference Ranges 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 16/12/2010
    ''' Modified by: TR 13/01/2011 - Before modifying a Calculated Test, verify if there are other elements affected 
    '''                              (Calculated Tests and/or Test Profiles), and in this case, shown the warning message
    '''              SA 14/01/2011 - If the Affected Elements warning screen is shown and the User cancelled the updation,
    '''                              the screen has to remain in EditionMode; changed from Sub to Function to allow returning 
    '''                              if the saving was executed or not
    '''              TR 04/09/2012 - Set to false value of global variable UpdateHistoryRequired once the saving has been executed
    '''              SA 17/09/2012 - Changed call to function Modify in CalculatedTestsDelegate: global variable CloseHistoryRequired 
    '''                              and the corresponding optional parameter have been deleted because they are not needed
    '''              AG 02/09/2014 - BA-1869 calculated test available TRUE when all his components are available, else FALSE
    ''' </remarks>
    Private Function SaveCalculatedTest() As Boolean
        Dim savingExecuted As Boolean = True
        Try
            'Fill Calculated Test basic data
            Dim calTestData As New CalculatedTestsDS
            calTestData = LoadCalculatedTestDS()

            'Fill the Values of Formula to include in the Calculated Test Formula 
            Dim selectedFormulaValue As New FormulasDS
            Dim calTestFormulaRow As FormulasDS.tparFormulasRow

            Dim auxFormula As New DataSet
            auxFormula = bsCalTestFormula.FormulaValuesList

            If (Not auxFormula Is Nothing) Then
                For i As Integer = 0 To auxFormula.Tables(0).Rows.Count - 1
                    calTestFormulaRow = selectedFormulaValue.tparFormulas.NewtparFormulasRow

                    calTestFormulaRow.CalcTestID = selectedCalTestID
                    calTestFormulaRow.Position = CInt(auxFormula.Tables(0).Rows(i).Item("Position"))
                    calTestFormulaRow.ValueType = UCase(auxFormula.Tables(0).Rows(i).Item("ValueType").ToString)
                    calTestFormulaRow.Value = auxFormula.Tables(0).Rows(i).Item("Value").ToString

                    If (UCase(auxFormula.Tables(0).Rows(i).Item("ValueType").ToString) = "TEST") Then
                        calTestFormulaRow.TestType = auxFormula.Tables(0).Rows(i).Item("TestType").ToString
                        calTestFormulaRow.SampleType = auxFormula.Tables(0).Rows(i).Item("SampleType").ToString
                        calTestFormulaRow.TestName = auxFormula.Tables(0).Rows(i).Item("TestName").ToString

                        'AG 02/09/2014 - BA-1869 - If some component not available then the calculated test is also not available
                        If Not calTestData Is Nothing AndAlso calTestData.tparCalculatedTests.Rows.Count > 0 Then
                            If Not auxFormula.Tables(0).Rows(i).Item("Available") Is DBNull.Value Then
                                If calTestData.tparCalculatedTests(0).IsAvailableNull Then
                                    calTestData.tparCalculatedTests(0).Available = CBool(auxFormula.Tables(0).Rows(i).Item("Available")) 'Initialize value
                                ElseIf Not CBool(auxFormula.Tables(0).Rows(i).Item("Available")) AndAlso calTestData.tparCalculatedTests(0).Available Then
                                    calTestData.tparCalculatedTests(0).Available = False
                                End If

                            Else
                                'Do nothing: Add method will use the default 1 value / Modify method wont modify Available column
                            End If
                        End If
                        'AG 02/09/2014 - BA-1869

                    End If
                    selectedFormulaValue.tparFormulas.Rows.Add(calTestFormulaRow)
                Next i
            End If


            'Get from the User Control of Reference Ranges the defined ones
            SelectedTestRefRangesDS = DirectCast(bsTestRefRanges.DefinedTestRangesDS, TestRefRangesDS)

            'Add/Update the Calculated Test
            Dim returnedData As New GlobalDataTO
            Dim calcTestToSave As New CalculatedTestsDelegate
            If (selectedCalTestID = 0) Then
                'Insert a new Calculated Test
                returnedData = calcTestToSave.Add(Nothing, calTestData, selectedFormulaValue, SelectedTestRefRangesDS)
            Else
                'Validate if there are affected elements...
                If (ValidateDependenciesOnUpdate(calTestData, selectedFormulaValue) = Windows.Forms.DialogResult.OK) Then
                    'Modify values of an existing Calculated Test
                    returnedData = calcTestToSave.Modify(Nothing, calTestData, selectedFormulaValue, SelectedTestRefRangesDS, True, UpdateHistoryRequired)
                    If (Not returnedData.HasError) Then UpdateHistoryRequired = False
                Else
                    savingExecuted = False
                End If
            End If

            If (savingExecuted) Then
                If (Not returnedData.HasError) Then
                    If (selectedCalTestID = 0) Then
                        'Show data of the added Calculated Test in Read Only Mode
                        selectedCalTestID = DirectCast(returnedData.SetDatos, CalculatedTestsDS).tparCalculatedTests(0).CalcTestID
                    End If
                Else
                    'Error adding/updating the Calculated Test; shown it
                    ShowMessage(Me.Name & ".SaveCalculatedTest", returnedData.ErrorCode, returnedData.ErrorMessage)
                    bsFullNameTextbox.Focus()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveCalculatedTest", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveCalculatedTest", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            savingExecuted = False
        End Try
        Return savingExecuted
    End Function

    ''' <summary>
    ''' Save data of the added or updated Calculated Test
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              AG 28/10/2010 - SaveFormulaChanges return a boolean True (OK, continue saving), False (Something failed, stop save)
    '''              SA 16/12/2010 - New function SaveCalculatedTest to save all data of the Calculated Test
    ''' </remarks>
    Private Sub SaveChanges()
        Try
            'Verify if the Calculated Test can be saved
            If (ValidateSavingConditions()) Then
                If (SaveCalculatedTest()) Then

                    If (selectedCalTestID > 0) Then
                        'Every time the list of Calculated Tests is reloaded, the correspondent list in the Formula Control is also reloaded
                        EditionMode = False
                        LoadCalculatedTests()

                        'Refresh the Calculated Test List and show the Test in Query Mode
                        LoadCalculatedTestList()
                        QueryModeScreenStatus()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load all data needed for the screen
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic 
    '''                              function ShowMessage
    '''              SA 21/06/2010 - Delete calls to LoadTestTypes and LoadCalculatedTests; they are not used
    '''              PG 07/10/2010 - Get the current Language for change language information of screen 
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            Dim currentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Get Icons for graphical buttons
            PrepareButtons()

            'Load the list of available Sample Types, Measure Units and Num of Allowed Decimals Limits
            LoadSampleTypesList()
            LoadMeasureUnits()
            LoadDecimalsLimit()

            'Load the list of existing Calculated Tests
            InitializeCalculatedTestList(currentLanguage) 'PG 08/10/2010

            'Load the multilanguage texts for all Screen Labels
            GetScreenLabels(currentLanguage)
            GetFormulaLabels(currentLanguage)

            'Set Screen Status to INITIAL MODE
            InitialModeScreenStatus()

            'Load the list of Standard and Calculated Tests in the Formula Control
            LoadTests()
            LoadCalculatedTests()

            If (bsCalTestListView.Items.Count > 0) Then
                'Select the first Calculated Test in the list
                bsCalTestListView.Items(0).Selected = True
                QueryCalculatedTest()
            End If

            'To avoid flickering while the screen is opening
            ResetBorder()
            UpdateHistoryRequired = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalculatedTestLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalculatedTestLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
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
                    BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
                    'bsPrintButton.Enabled = False   DL 11/05/2012
                    Exit Select
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the Calculated Test uses an unique SampleType, property SelectedSampleType of Formula User Control is informed
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              SG 30/07/2010 - Clear the Formula in case of changing the SampleType to Unique
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub SelectFormulaSampleType()
        Try
            If (bsUniqueRadioButton.Checked) Then
                If (Not bsSampleComboBox.SelectedValue Is Nothing) Then
                    'If (bsCalTestFormula.SelectedSampleType.ToUpper <> CStr(bsSampleComboBox.SelectedValue).ToUpper) Then
                    If (bsCalTestFormula.SelectedSampleType <> CStr(bsSampleComboBox.SelectedValue)) Then
                        bsCalTestFormula.GenerateFormula = String.Empty
                        bsCalTestFormula.FormulaValuesList.Clear()
                    End If
                    bsCalTestFormula.SelectedSampleType = bsSampleComboBox.SelectedValue.ToString
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SelectFormulaSampleType", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SelectFormulaSampleType", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the Calculated Test uses multiple Sample Types, the SampleTypes ComboBox is disabled in 
    ''' details area, while the same ComboBox in the Formula User Control is enabled
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub SelectMultipleSampleType()
        Try
            If (bsMultipleRadioButton.Checked) Then
                bsSampleComboBox.Enabled = False
                bsSampleComboBox.BackColor = SystemColors.MenuBar
                bsCalTestFormula.EnableSampleType(True)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SelectMultipleSampleType", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SelectMultipleSampleType", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the Calculated Test uses an unique Sample Type, the SampleTypes ComboBox is enabled in 
    ''' details area, while the same ComboBox in the Formula User Control is disabled
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub SelectUniqueSampleType()
        Try
            If (bsUniqueRadioButton.Checked) Then
                bsSampleComboBox.Enabled = True
                bsSampleComboBox.BackColor = Color.White
                bsCalTestFormula.EnableSampleType(False)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SelectUniqueSampleType", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SelectUniqueSampleType", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate if there are affected elements that have to be changed/deleted due to deletion of the selected 
    ''' Calculated Tests. These elements can be Profiles or another Calculated Tests
    ''' </summary>
    ''' <param name="pCalcTestDataDS">Typed DataSet CalculatedTestsDS containing the list of Calculated Tests selected to be deleted</param>
    ''' <returns>If the warning screen is shown, it returns the User's answer: continue deletion or stop the process.
    '''          If there are not other affected elements, it returns the answer to continue process</returns>
    ''' <remarks>
    ''' Created by:  TR 25/11/2010
    ''' </remarks>
    Private Function ValidateDependenciesOnDeletedElements(ByVal pCalcTestDataDS As CalculatedTestsDS) As DialogResult
        Dim myResult As New DialogResult
        myResult = Windows.Forms.DialogResult.Cancel

        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myCalculateTestDelegate As New CalculatedTestsDelegate

            myGlobalDataTO = myCalculateTestDelegate.ValidatedDependencies(pCalcTestDataDS)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myDependeciesElementsDS As New DependenciesElementsDS
                myDependeciesElementsDS = DirectCast(myGlobalDataTO.SetDatos, DependenciesElementsDS)

                If (myDependeciesElementsDS.DependenciesElements.Count > 0) Then
                    Using AffectedElement As New IWarningAfectedElements
                        AffectedElement.AffectedElements = myDependeciesElementsDS

                        AffectedElement.ShowDialog()
                        myResult = AffectedElement.DialogResult
                    End Using
                Else
                    'If there are not affected elements, then return OK to continue deletion process
                    myResult = Windows.Forms.DialogResult.OK
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateDependenciesOnDeletedElements", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateDependenciesOnDeletedElements", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Validate if there are affected elements that have to be changed/deleted due to updation of the currently edited 
    ''' Calculated Test. These elements can be Profiles or another Calculated Tests
    ''' </summary>
    ''' <param name="pCalcTestDataDS">Typed DataSet CalculatedTestsDS containing the ID and Name of the Calculated Test</param>
    ''' <param name="pFormulaDS">Typed DataSet FormulasDS containing all members of the Formula defined for the Calculated Test</param>
    ''' <returns>If the warning screen is shown, it returns the User's answer: continue updation or stop the process.
    '''          If there are not other affected elements, it returns the answer to continue process</returns>
    ''' <remarks>
    ''' Created by:  TR 13/01/2011
    ''' Modified by: SA 14/01/2011 - Use DISTINCT to get the list of different Sample Types used in the Formula of the Calculated Test
    ''' </remarks>
    Private Function ValidateDependenciesOnUpdate(ByVal pCalcTestDataDS As CalculatedTestsDS, ByVal pFormulaDS As FormulasDS) As DialogResult
        Dim myResult As New DialogResult
        myResult = Windows.Forms.DialogResult.Cancel

        Try
            'Get all Sample Types of the Tests included in the Formula defined for the Calculated Test
            Dim qFormulaList As New List(Of String)
            qFormulaList = (From a In pFormulaDS.tparFormulas _
                           Where a.ValueType = "TEST" _
                          Select a.SampleType).Distinct.ToList()

            'Build a String List containing all different SampleTypes linked by commas
            Dim SampleTypeList As String = ""
            For Each diffSampleType As String In qFormulaList
                SampleTypeList &= "'" & diffSampleType & "',"
            Next

            'Finally, remove the last comma 
            If (SampleTypeList.Length > 0) Then
                SampleTypeList = SampleTypeList.Remove(SampleTypeList.Count - 1, 1)
            End If

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myCalculateTestDelegate As New CalculatedTestsDelegate
            Dim myDependeciesElementsDS As New DependenciesElementsDS

            'Get all affected Profiles and Calculated Tests:
            '  *** Profiles: those including the Calculated Test defined for a SampleType different from the ones in SampleTypeList
            '  *** Calculated Test: those including the Calculated Test as Formula member but with a SampleType different from the 
            '                       ones in SampleTypeList
            myGlobalDataTO = myCalculateTestDelegate.ValidatedDependenciesOnUpdate(pCalcTestDataDS, SampleTypeList, Nothing)
            If (Not myGlobalDataTO.HasError) Then
                myDependeciesElementsDS = DirectCast(myGlobalDataTO.SetDatos, DependenciesElementsDS)
                If (myDependeciesElementsDS.DependenciesElements.Count > 0) Then
                    'If there are dependencies, then shown the warning screen with the list 
                    Using AffectedElement As New IWarningAfectedElements
                        AffectedElement.AffectedElements = myDependeciesElementsDS

                        AffectedElement.ShowDialog()
                        myResult = AffectedElement.DialogResult
                    End Using
                Else
                    'If there are not affected elements, then return OK to continue updating process
                    myResult = Windows.Forms.DialogResult.OK
                End If
            Else
                'Error getting the list of affected elements; shown it
                ShowMessage(Me.Name & ".ValidateDependenciesOnUpdate", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateDependenciesOnUpdate", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateDependenciesOnUpdate", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Validate a Formula with correct sintax has been defined for the Calculated Test before saving it
    ''' </summary>
    ''' <returns>True if the Formula has been defined and its sintax is correct; otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by: SG 03/09/2010
    ''' </remarks>
    Private Function ValidateFormula() As Boolean
        Try
            If (bsCalTestFormula.FormulaString.Trim.Length = 0) Then
                'Validate the Formula is not empty
                bsCalTestFormula.ShowErrorProvider(GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                Return False
            ElseIf (Not bsCalTestFormula.FormulaSintaxStatus) Then
                'Validate the sintax of the Formula is correct
                bsCalTestFormula.ShowErrorProvider(GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                Return False
            End If
            Return True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateFormula", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateFormula", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' When Reference Ranges have been defined for the selected Calculated Test; validate they are correct
    ''' before saving the Calculated Test (or before allow changing the selected Tab)
    ''' </summary>
    ''' <returns>True if not Ranges have been defined or if the defined ones are correct; otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  SG 03/09/2010
    ''' Modified by: SA 16/12/2010 - Changes due to new implementation of Reference Ranges control
    ''' </remarks>
    Private Function ValidateRefRanges() As Boolean
        Try
            If (bsTestRefRanges.ActiveRangeType <> String.Empty) Then
                bsTestRefRanges.ValidateRefRangesLimits(False, True)
                If (bsTestRefRanges.ValidationError) Then Return False
            End If
            Return True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateRefRanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateRefRanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Validate that all mandatory fields have been informed with a correct value
    ''' </summary>
    ''' <returns>True if all fields have a correct value; otherwise, False</returns>
    ''' <remarks>
    ''' Modified by: SA 07/07/2010 - Use the Error Provider to show Messages
    '''              SA 14/01/2011 - Validate also that informed Name and ShortName are unique 
    '''              DL 06/02/2012 - Select a value in ComboBox of MeasureUnits is not mandatory due to there are Calculated Tests without Measure Unit
    '''              TR 29/05/2012 - Added again validation of ComboBox of MeasureUnits as mandatory
    '''              SA 13/09/2012 - Removed last TR change: ComboBox of MeasureUnits is NOT mandatory
    ''' </remarks>
    Private Function ValidateSavingConditions() As Boolean
        Dim fieldsOK As Boolean = True
        Try
            Dim setFocusTo As Integer = -1

            bsScreenErrorProvider.Clear()
            If (bsFullNameTextbox.TextLength = 0) Then
                'Validate the Long Name is not empty, otherwise inform the missing data
                bsScreenErrorProvider.SetError(bsFullNameTextbox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                setFocusTo = 0
            End If

            If (bsNameTextbox.TextLength = 0) Then
                'Validate the Short Name is not empty, otherwise inform the missing data
                bsScreenErrorProvider.SetError(bsNameTextbox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                If (setFocusTo = -1) Then setFocusTo = 1
            End If

            'Validate the Test Formula
            If (Not ValidateFormula()) Then
                If (setFocusTo = -1) Then setFocusTo = 2
            End If

            'Validate the Test Reference Ranges
            If (Not ValidateRefRanges()) Then
                If (setFocusTo = -1) Then setFocusTo = 3
            End If

            'Select the proper field to put the focus
            If (setFocusTo >= 0) Then
                fieldsOK = False

                If (setFocusTo = 0) Then
                    bsFullNameTextbox.Focus()
                ElseIf (setFocusTo = 1) Then
                    bsNameTextbox.Focus()
                ElseIf (setFocusTo = 2) Then
                    Me.CalculatedTestTabControl.SelectTab(0)
                ElseIf (setFocusTo = 3) Then
                    Me.CalculatedTestTabControl.SelectTab(1)
                End If
            Else
                'All mandatory fields are informed, verify the informed Name and ShortName are unique
                Dim resultData As New GlobalDataTO
                Dim myCalcTestDelegate As New CalculatedTestsDelegate

                resultData = myCalcTestDelegate.ExistsCalculatedTest(Nothing, bsFullNameTextbox.Text, "FNAME", selectedCalTestID)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    Dim duplicatedTest As Boolean = False
                    duplicatedTest = CType(resultData.SetDatos, Boolean)

                    If (duplicatedTest) Then
                        bsScreenErrorProvider.SetError(bsFullNameTextbox, GetMessageText(GlobalEnumerates.Messages.DUPLICATED_TEST_NAME.ToString))
                        setFocusTo = 0
                    End If
                End If

                resultData = myCalcTestDelegate.ExistsCalculatedTest(Nothing, bsNameTextbox.Text, "NAME", selectedCalTestID)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    Dim duplicatedTest As Boolean = False
                    duplicatedTest = CType(resultData.SetDatos, Boolean)

                    If (duplicatedTest) Then
                        bsScreenErrorProvider.SetError(bsNameTextbox, GetMessageText(GlobalEnumerates.Messages.DUPLICATED_TEST_SHORTNAME.ToString))
                        If (setFocusTo = -1) Then setFocusTo = 1
                    End If
                End If

                If (setFocusTo >= 0) Then
                    fieldsOK = False

                    If (setFocusTo = 0) Then
                        bsFullNameTextbox.Focus()
                    ElseIf (setFocusTo = 1) Then
                        bsNameTextbox.Focus()
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
    ''' When the screen is in ADD or EDIT Mode and the ESC Key is pressed, code of Cancel Button is executed;
    ''' in other case, the screen is closed when ESC Key is pressed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 08/11/2010
    ''' Modified by: RH 04/07/2011 - Escape key should do exactly the same operations as bsCancelButton_Click()
    ''' </remarks>
    Private Sub IProgCalculatedTest_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (bsCancelButton.Enabled) Then
                    bsCancelButton.PerformClick()
                Else
                    bsExitButton.PerformClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ProgCalculatedTest_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgCalculatedTest_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Form initialization when loading: set the screen status to INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              TR 23/04/2012 - Get level of the current User
    ''' </remarks>
    Private Sub IProgCalculatedTest_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim MyGlobalBase As New GlobalBase
            CurrentUserLevel = MyGlobalBase.GetSessionInfo().UserLevel

            ScreenLoad()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ProgCalculatedTest_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgCalculatedTest_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Enable/disable screen functionalities according the level of the current User
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 23/04/2012 
    ''' </remarks>
    Private Sub IProgCalculatedTest_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            ScreenStatusByUserLevel()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IProgCalculatedTest_Shown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IProgCalculatedTest_Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    '*******************************************'
    '* EVENTS FOR LISTVIEW OF CALCULATED TESTS *'
    '*******************************************'
    ''' <summary>
    ''' Show data of the selected Calculated Test in READ-ONLY MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009  - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              GDS 25/05/2010 - Added code for multi selection
    '''              SA  16/12/2010 - Changes due to new implementation of Reference Ranges control
    ''' </remarks>
    Private Sub bsCalTestListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsCalTestListView.Click
        Try
            If (bsCalTestListView.SelectedItems.Count = 1) Then
                QueryCalculatedTest()
            Else
                Dim bEnabled As Boolean = True
                For Each mySelectedItem As ListViewItem In bsCalTestListView.SelectedItems
                    'If there is an item InUse
                    If (CBool(mySelectedItem.SubItems(9).Text)) Then
                        bEnabled = False
                        Exit For
                    End If
                Next mySelectedItem

                'Set screen status to the case when there are several selected Calculated Tests
                InitialModeScreenStatus(False)

                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = bEnabled
                BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
            End If
            ScreenStatusByUserLevel() 'TR 23/04/2012
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCalTestListView_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCalTestListView_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Sort ascending/descending the list of Calculated Tests when clicking in the list header
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 08/07/2010
    ''' </remarks>
    Private Sub bsCalTestListView_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles bsCalTestListView.ColumnClick
        Try
            Select Case bsCalTestListView.Sorting
                Case SortOrder.None
                    bsCalTestListView.Sorting = SortOrder.Ascending
                    Exit Select
                Case SortOrder.Ascending
                    bsCalTestListView.Sorting = SortOrder.Descending
                    Exit Select
                Case SortOrder.Descending
                    bsCalTestListView.Sorting = SortOrder.Ascending
                    Exit Select
                Case Else
                    Exit Select
            End Select
            bsCalTestListView.Sort()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCalTestListView_ColumnClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCalTestListView_ColumnClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Don't allow Users to resize the visible ListView column
    ''' </summary>
    Private Sub bsCalTestListView_ColumnWidthChanging(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles bsCalTestListView.ColumnWidthChanging
        Try
            e.Cancel = True
            If (e.ColumnIndex = 0) Then
                e.NewWidth = 210
            Else
                e.NewWidth = 0
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCalTestListView_ColumnWidthChanging", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCalTestListView_ColumnWidthChanging", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load data of the selected Calculated Test and set the screen status to Edit Mode
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub bsCalTestListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsCalTestListView.DoubleClick
        Try
            'Validate user level.
            If Not CurrentUserLevel = "OPERATOR" Then
                EditCalTestByDoubleClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCalTestListView_DoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCalTestListView_DoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Allow consultation of elements in Calculated Test list using the keyboard, and also deletion using SUPR key
    ''' </summary>
    ''' <remarks>
    ''' Modified by:  BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub bsCalTestListView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsCalTestListView.KeyUp
        Try
            If (bsCalTestListView.SelectedItems.Count = 1) Then
                QueryCalTestByMoveUpDown()
            Else
                Dim bEnabled As Boolean = True
                For Each mySelectedItem As ListViewItem In bsCalTestListView.SelectedItems
                    'If there is an item InUse
                    If (CBool(mySelectedItem.SubItems(9).Text)) Then
                        bEnabled = False
                        Exit For
                    End If
                Next

                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = bEnabled
                BsCustomOrderButton.Enabled = True 'AG 05/09/2014 - BA-1869
            End If

            ScreenStatusByUserLevel() 'TR 23/04/2012
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCalTestListView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCalTestListView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Allow deletion of Calculated Tests using the SUPR key; allow set the edition Mode for the selected Calculated Test
    ''' when Enter is pressed
    ''' </summary>
    Private Sub bsCalTestListView_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsCalTestListView.PreviewKeyDown
        Try
            If (e.KeyCode = Keys.Delete And bsDeleteButton.Enabled) Then
                DeleteCalculatedTest()

            ElseIf (e.KeyCode = Keys.Enter And bsEditButton.Enabled) Then
                EditCalTestByButtonClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCalTestListView_PreviewKeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCalTestListView_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    '*************************************'
    '* EVENTS FOR FIELDS IN DETAILS AREA *'
    '*************************************'
    ''' <summary>
    ''' When the Formula of the Calculated Test is changed, flag CloseHistoryRequired is set to TRUE; otherwise it is set to FALSE
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 04/09/2012
    ''' </remarks>
    Private Sub bsCalTestFormula_FormulaChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCalTestFormula.FormulaChanged
        Try
            If (EditionMode) Then
                If (bsCalTestFormula.FormulaSintaxStatus AndAlso myOriginalFormulaValue <> bsCalTestFormula.FormulaString) Then
                    UpdateHistoryRequired = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCalTestFormula_FormulaChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCalTestFormula_FormulaChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set value of the selected Number of Decimals for the Reference Ranges control
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 04/10/2010
    ''' </remarks>
    Private Sub bsDecimalsNumericUpDown_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDecimalsNumericUpDown.Validated
        Try
            Dim decimalsNumber As Integer = 0
            If IsNumeric(bsDecimalsNumericUpDown.Text) Then decimalsNumber = CInt(bsDecimalsNumericUpDown.Text)

            decimalsNumber = CInt(IIf(decimalsNumber < bsDecimalsNumericUpDown.Minimum, bsDecimalsNumericUpDown.Minimum, decimalsNumber))
            decimalsNumber = CInt(IIf(decimalsNumber > bsDecimalsNumericUpDown.Maximum, bsDecimalsNumericUpDown.Maximum, decimalsNumber))
            bsDecimalsNumericUpDown.Text = decimalsNumber.ToString

            'Set the new selected Number of Decimals for controls in the area of Reference Ranges
            bsTestRefRanges.RefNumDecimals = decimalsNumber
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsDecimalsNumericUpDown_Validated", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsDecimalsNumericUpDown_Validated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the number of decimals allowed for the Calculated Test changes, flag UpdateHistoryRequired is set to TRUE
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 04/09/2012
    ''' </remarks>
    Private Sub bsDecimalsNumericUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDecimalsNumericUpDown.ValueChanged
        Try
            If (EditionMode) Then UpdateHistoryRequired = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsDecimalsNumericUpDown_ValueChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsDecimalsNumericUpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Clear the formula when the SampleType for the Calculated Test is switched from Multiple to Unique
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 30/07/2010
    ''' </remarks>
    Private Sub bsMultipleRadioButton_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsMultipleRadioButton.CheckedChanged
        Try
            If (Not bsMultipleRadioButton.Checked And EditionMode) Then
                bsCalTestFormula.GenerateFormula = String.Empty
                bsCalTestFormula.FormulaValuesList.Clear()
                bsSampleComboBox.SelectedIndex = 0
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsMultipleRadioButton_CheckedChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsMultipleRadioButton_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' To select the Multiple Sample Type
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub bsMultipleRadioButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsMultipleRadioButton.Click
        Try
            SelectMultipleSampleType()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsMultipleRadioButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsMultipleRadioButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load list of Tests when a new Sample Type is chosen.
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub bsSampleComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSampleComboBox.SelectedIndexChanged
        Try
            SelectFormulaSampleType()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSampleComboBox_SelectedIndexChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSampleComboBox_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the TextBox showing the error is changed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 07/07/2010
    ''' Modified by: TR 04/09/2012 - When the event is raised for the field containing the Calculated Test long name, 
    '''                              flag UpdateHistoryRequired is set to TRUE
    ''' </remarks>
    Private Sub bsTextbox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsFullNameTextbox.TextChanged, _
                                                                                                   bsNameTextbox.TextChanged
        Try
            Dim myTextBox As New TextBox
            myTextBox = CType(sender, TextBox)

            If (myTextBox.TextLength > 0) Then
                If (bsScreenErrorProvider.GetError(myTextBox) <> "") Then
                    bsScreenErrorProvider.SetError(myTextBox, String.Empty)
                End If
            End If

            If (EditionMode AndAlso myTextBox.Name = bsFullNameTextbox.Name) Then UpdateHistoryRequired = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTextbox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTextbox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' To Select the Unique Sample Type
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub bsUniqueRadioButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsUniqueRadioButton.Click
        Try
            SelectUniqueSampleType()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsUniqueRadioButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsUniqueRadioButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Clean the Error Provider when a value is selected in the ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 07/07/2010
    ''' Modified by: TR 04/09/2012 - When the Measure Unit of the Calculated Test changes, flag UpdateHistoryRequired is set to TRUE
    ''' </remarks>
    Private Sub bsUnitComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsUnitComboBox.SelectedIndexChanged
        Try
            If (bsUnitComboBox.SelectedIndex <> -1) Then
                'Show the new selected Measure Unit in the area of Reference Ranges
                bsTestRefRanges.MeasureUnit = bsUnitComboBox.Text
            End If

            If (EditionMode) Then UpdateHistoryRequired = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsUnitComboBox_SelectedIndexChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsUnitComboBox_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '**********************'
    '* EVENTS FOR BUTTONS *'
    '**********************'
    ''' <summary>
    ''' Set the screen status to ADD MODE
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub bsNewButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNewButton.Click
        Try
            AddCalculatedTest()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNewButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNewButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen status to EDIT MODE for the selected Calculated Test
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub bsEditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsEditButton.Click
        Try
            'EditCalTestByButtonClick()
            EditCalTestByDoubleClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsEditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsEditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Executes deletion of all the selected Calculated Tests
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>
    Private Sub bsDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDeleteButton.Click
        Try
            DeleteCalculatedTest()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsDeleteButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsDeleteButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen, verifying first if there are changes pending to save
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 09/07/2010 - Added the dispose of Formula User Control before close the screen
    '''              TR 12/01/2010 - Call directly the ShowMessage method
    '''              RH 18/10/2010 - Removed the dispose of Formula User Control before close the screen
    ''' </remarks>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            ExitScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save changes (add or updation) in the database
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    ''' </remarks>    
    Private Sub bsSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSaveButton.Click
        Try
            SaveChanges()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSaveButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSaveButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Execute the cancelling of a Calculated Test adding or edition
    ''' </summary>
    ''' <remarks>
    ''' Modified by: BK 29/12/2009 - Calls to ApplicationLog_ForTESTING were replaced by calls to the generic function ShowMessage
    '''              TR 29/05/2012 - Flag EditionMode is set to FALSE
    '''              TR 04/09/2012 - Flag UpdateHistoryRequired is set to FALSE
    ''' </remarks>    
    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            CancelCalcTestEdition()
            EditionMode = False
            UpdateHistoryRequired = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Open the customize order and availability for OFFS tests selection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>AG 05/09/2014 - BA-1869</remarks>
    Private Sub BsCustomOrderButton_Click(sender As Object, e As EventArgs) Handles BsCustomOrderButton.Click
        Try
            'Shown the Positioning Warnings Screen
            Using AuxMe As New ISortingTestsAux()
                AuxMe.openMode = "TESTSELECTION"
                AuxMe.screenID = "CALC"
                AuxMe.ShowDialog()
            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BsCustomOrderButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BsCustomOrderButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


#End Region

End Class

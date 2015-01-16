Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
'Imports System.Configuration
Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Controls.UserControls

Public Class IProgControls
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Attributes"
    Private SourceScreenAttribute As String = GlbSourceScreen.STANDARD.ToString
    Private WorkSessionIDAttribute As String
    Private AnalyzerIDAttribute As String
#End Region

#Region "Properties"
    Public WriteOnly Property SourceScreen() As String
        Set(ByVal value As String)
            SourceScreenAttribute = value
        End Set
    End Property

    Public Property AnalyzerID() As String
        Get
            Return AnalyzerIDAttribute
        End Get
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    Public Property WorkSessionID() As String
        Get
            Return WorkSessionIDAttribute
        End Get
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property
#End Region

#Region "Declarations"
    Private currentLanguage As String                      'To store the current application language  

    Private selectedControlID As Integer = -1              'To store the ID of the selected Control
    Private originalSelectedIndex As Integer = -1          'To store the index of the selected Control to verify Pending Changes

    Private EditionMode As Boolean = False                 'To validate when the selected Control is in Edition Mode
    Private ChangesMade As Boolean = False                 'To validate if there are changes pending to save when cancelling 

    Private LotChanged As Boolean = False                  'To indicate the Lot has been changed for a new one
    Private SaveCurrentLotAsPrevious As Boolean = False    'To indicate if the current Lot has to be saved as Previous when updating a Control (and the Lot has been changed) 
    Private TestControlsListDS As New TestControlsDS       'To store the list of Tests/SampleTypes linked to the Control with their Min/Max Concentration values 
    Private DeletedTestsListDS As New SelectedTestsDS      'To store the list of Tests/SampleTypes that have been unlinked from the Control

    Private MinAllowedConcentration As Single              'To store the minimum allowed value for Min/Max Concentration fields
    Private MaxAllowedConcentration As Single              'To store the maximum allowed value for Min/Max Concentration fields
#End Region

#Region "Constructor"
    Public Sub New()
        'This call is required by the Windows Form Designer
        InitializeComponent()
    End Sub
#End Region

#Region "Methods"
    ''' <summary>
    ''' Prepare the screen for adding a new Control. If there are changes pending
    ''' to save, the Discard Pending Changes Verification is executed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 11/05/2011 - Search Tests button has not to be disabled
    ''' </remarks>
    Private Sub AddControl()
        Try
            If ExistPendingChanges() Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    'Set Screen Status to ADD MODE
                    AddModeScreenStatus()
                Else
                    If (originalSelectedIndex <> -1) Then
                        'Return focus to the control that has been edited
                        bsControlsListView.Items(originalSelectedIndex).Selected = True
                        bsControlsListView.Select()
                    End If
                End If
            Else
                'Set Screen Status to ADD MODE
                AddModeScreenStatus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AddControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AddControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to ADD MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 11/05/2011 - Min allowed ExpirationDate will be ActivationDate + 1 day. ActivationDate
    '''                              has to be shown with the date/time format currently active in the OS
    '''              TR 24/07/2013 - Disabel button Add new control lot (bsNewLotButton) to avoid error (bug #1130)
    '''              XB 01/09/2014 - add Level field - BA #1868
    ''' </remarks>
    Private Sub AddModeScreenStatus()
        Try
            'Set variable for control of changes pending to save to FALSE
            ChangesMade = False
            EditionMode = True

            'Area of Control List
            If (bsControlsListView.Items.Count > 0) Then
                bsControlsListView.SelectedItems.Clear()
            End If

            'Buttons
            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            '            bsPrintButton.Enabled = False DL 11/05/2012

            bsNewLotButton.Enabled = False 'TR 24/07/2013 disable add new control lot button.

            'Buttons in details area
            bsSaveButton.Enabled = True
            bsCancelButton.Enabled = True

            'Area of control Definition
            bsControlNameTextbox.Enabled = True
            bsControlNameTextbox.Text = String.Empty
            bsControlNameTextbox.BackColor = Color.Khaki

            bsSampleTypeComboBox.Enabled = True
            bsSampleTypeComboBox.SelectedValue = -1
            bsSampleTypeComboBox.BackColor = Color.Khaki

            bsLotNumberTextBox.Enabled = True
            bsLotNumberTextBox.Text = String.Empty
            bsLotNumberTextBox.BackColor = Color.Khaki

            bsActivationTextBox.Text = Now.ToString(SystemInfoManager.OSDateFormat) & " " & Now.ToString(SystemInfoManager.OSShortTimeFormat)

            bsExpDatePickUpCombo.Enabled = True
            bsExpDatePickUpCombo.MinDate = CDate(bsActivationTextBox.Text).AddDays(1)
            bsExpDatePickUpCombo.Value = Now.AddMonths(3)

            bsLevelUpDown.Enabled = True
            bsLevelUpDown.Value = 1
            bsLevelUpDown.BackColor = Color.White

            'Initialize global variables
            bsScreenErrorProvider.Clear()
            CleanGlobalValues()

            'Clean and enabled Tests/SampleTypes DS
            DeletedTestsListDS = New SelectedTestsDS

            TestControlsListDS = New TestControlsDS
            bsTestListGrid.DataSource = TestControlsListDS.tparTestControls

            'Show/hide the Tests/SampleTypes area according the screen use
            If (SourceScreenAttribute = GlbSourceScreen.STANDARD.ToString) Then
                bsTestListGrid.Enabled = True
                bsSearchTestsButton.Enabled = True
            Else
                'When screen was opened from Tests Programming Screen, this area is hidden
                bsTestControlPanel.Visible = False
            End If

            'Put Focus in the first enabled field
            bsControlNameTextbox.Focus()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".AddModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".AddModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Calculate values of fields TargetMean and TargetSD when Min/Max values are informed
    ''' </summary>
    ''' <param name="pRow">Current Row index</param>
    ''' <param name="pCol">Current Column Index</param>
    ''' <remarks>
    ''' Created by:  TR 06/04/2011
    ''' Modified by: SA 12/05/2011 - When Target Values are calculated, apply the numeric formats to them and
    '''                              also the Min/Max cells
    ''' </remarks>
    Private Sub CalculatedTarget(ByVal pRow As Integer, ByVal pCol As Integer)
        Try
            'Clear all errors 
            bsTestListGrid.Rows(pRow).Cells("MinConcentration").ErrorText = String.Empty
            bsTestListGrid.Rows(pRow).Cells("MaxConcentration").ErrorText = String.Empty

            'Apply Styles
            bsTestListGrid.Rows(pRow).Cells(pCol).Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListGrid.Rows(pRow).Cells("MinConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListGrid.Rows(pRow).Cells("MaxConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Validate there are not null, zero values or only the decimal separator in the cells
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

            If (Not bsTestListGrid.Rows(pRow).Cells("MinConcentration").Value Is Nothing) AndAlso _
               (Not bsTestListGrid.Rows(pRow).Cells("MaxConcentration").Value Is Nothing) AndAlso _
               (Not bsTestListGrid.Rows(pRow).Cells("MinConcentration").Value Is DBNull.Value) AndAlso _
               (Not bsTestListGrid.Rows(pRow).Cells("MaxConcentration").Value Is DBNull.Value) AndAlso _
               (bsTestListGrid.Rows(pRow).Cells("MinConcentration").Value.ToString <> "") AndAlso _
               (bsTestListGrid.Rows(pRow).Cells("MaxConcentration").Value.ToString <> "") AndAlso _
               (bsTestListGrid.Rows(pRow).Cells("MinConcentration").Value.ToString <> myDecimalSeparator) AndAlso _
               (bsTestListGrid.Rows(pRow).Cells("MaxConcentration").Value.ToString <> myDecimalSeparator) Then

                Dim minValue As Single = Convert.ToSingle(bsTestListGrid.Rows(pRow).Cells("MinConcentration").Value)
                Dim maxValue As Single = Convert.ToSingle(bsTestListGrid.Rows(pRow).Cells("MaxConcentration").Value)
                Dim kSDValue As Single = Convert.ToSingle(bsTestListGrid.Rows(pRow).Cells("RejectionCriteria").Value)

                'Validate before calculating
                If (minValue < maxValue) Then
                    'Calculate the TargetMean as (Min+Max)/2 
                    bsTestListGrid.Rows(pRow).Cells("TargetMean").Value = (minValue + maxValue) / 2

                    'Calculate the TargetSD as (Max-Min)/(2*kSD)
                    bsTestListGrid.Rows(pRow).Cells("TargetSD").Value = (maxValue - minValue) / (2 * kSDValue)

                    'Apply numeric formats
                    Dim myValue As Single
                    Dim myDecimals As Integer = Convert.ToInt32(bsTestListGrid.Rows(pRow).Cells("DecimalsAllowed").Value)

                    myValue = Convert.ToSingle(bsTestListGrid.Rows(pRow).Cells("MinConcentration").Value)
                    bsTestListGrid.Rows(pRow).Cells("MinConcentration").Value = myValue.ToString("F" & myDecimals)

                    myValue = Convert.ToSingle(bsTestListGrid.Rows(pRow).Cells("MaxConcentration").Value)
                    bsTestListGrid.Rows(pRow).Cells("MaxConcentration").Value = myValue.ToString("F" & myDecimals)

                    myValue = Convert.ToSingle(bsTestListGrid.Rows(pRow).Cells("TargetMean").Value)
                    bsTestListGrid.Rows(pRow).Cells("TargetMean").Value = myValue.ToString("F" & myDecimals)

                    myDecimals += 1
                    myValue = Convert.ToSingle(bsTestListGrid.Rows(pRow).Cells("TargetSD").Value)
                    bsTestListGrid.Rows(pRow).Cells("TargetSD").Value = myValue.ToString("F" & myDecimals)
                Else
                    'Show error message
                    bsTestListGrid.Rows(pRow).Cells(pCol).Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    bsTestListGrid.Rows(pRow).Cells(pCol).ErrorText = GetMessageText(GlobalEnumerates.Messages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)

                    'Clean cells 
                    bsTestListGrid.Rows(pRow).Cells("TargetMean").Value = DBNull.Value
                    bsTestListGrid.Rows(pRow).Cells("TargetSD").Value = DBNull.Value
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalculatedTarget ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalculatedTarget ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Execute the cancelling of a Control adding or edition
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 21/06/2012 - After loading data of the first Control in the list in details are, 
    '''                              set focus to the List of Controls
    ''' </remarks>
    Private Sub CancelControlEdition()
        Dim setScreenToInitial As Boolean = False

        Try
            If (ExistPendingChanges()) Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    setScreenToInitial = True
                    bsScreenErrorProvider.Clear()
                Else
                    If (originalSelectedIndex <> -1) Then
                        'Return focus to the Controls that has been edited
                        bsControlsListView.Items(originalSelectedIndex).Selected = True
                        bsControlsListView.Select()
                    End If
                End If
            Else
                setScreenToInitial = True
            End If

            If (setScreenToInitial) Then
                If (bsControlsListView.Items.Count = 0) Then
                    'Set screen status to Initial Mode
                    InitialModeScreenStatus()
                Else
                    'Select the first Control in the list and show it in Query Mode or in ReadOnly Mode if it is a InUse Control
                    If (Not LoadDataOfControl()) Then
                        'Set screen status to Query Mode
                        QueryModeScreenStatus()
                    Else
                        'Set screen status to Read Only Mode
                        ReadOnlyModeScreenStatus()
                    End If

                    bsControlsListView.Focus()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CancelControlEdition", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CancelControlEdition", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow changing the Lot for the Control
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 12/05/2011 - Code moved here from the Click Event of New Lot button
    '''                              Implementation changed
    ''' Modified by  RH 16/06/2012 - Adapted for TestType. Code optimization.
    ''' </remarks>
    Private Sub ChangeLot()
        Try
            'Search if there is a previous saved Lot for the Control
            Dim resultData As GlobalDataTO
            Dim myPreviousLotDelegate As New PreviousControlLotsDelegate

            resultData = myPreviousLotDelegate.GetPreviousLot(Nothing, selectedControlID)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myPreviousControlLotDS As ControlsDS = DirectCast(resultData.SetDatos, ControlsDS)

                Using myForm As New ILotChangeAuxScreen
                    myForm.CurrentLot = bsLotNumberTextBox.Text.Trim
                    myForm.OriginalLot = bsControlsListView.SelectedItems(0).SubItems(4).Text
                    myForm.PreviusLotDataDS = myPreviousControlLotDS
                    myForm.ShowDialog()

                    If (myForm.DialogResult = DialogResult.OK) Then
                        Dim cancelLotChange As Boolean = False

                        'Validate if there are QC Results pending to cumulate for the changed Lot
                        If (ValidateDependenciesOnLotChange() = DialogResult.OK) Then
                            If (myForm.PreviousLotRecovered) Then
                                Dim myPreviousTestLotDelegate As New PreviousTestControlLotsDelegate
                                resultData = myPreviousTestLotDelegate.GetPreviousLotTests(Nothing, selectedControlID)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    Dim myPreviousLotTestsDS As TestControlsDS = DirectCast(resultData.SetDatos, TestControlsDS)

                                    Dim lstPreviousLotValues As List(Of TestControlsDS.tparTestControlsRow)
                                    For Each selectedTest As TestControlsDS.tparTestControlsRow In TestControlsListDS.tparTestControls
                                        'Search if there are values for the Test/SampleType for the Previous Lot
                                        lstPreviousLotValues = (From a As TestControlsDS.tparTestControlsRow In myPreviousLotTestsDS.tparTestControls _
                                                               Where a.TestType = selectedTest.TestType _
                                                             AndAlso a.TestID = selectedTest.TestID _
                                                             AndAlso a.SampleType = selectedTest.SampleType _
                                                              Select a).ToList()

                                        If (lstPreviousLotValues.Count = 0) Then
                                            'There is not information, clean Min/Max and Target values
                                            selectedTest.SetMinConcentrationNull()
                                            selectedTest.SetMaxConcentrationNull()
                                            selectedTest.SetTargetMeanNull()
                                            selectedTest.SetTargetSDNull()
                                        Else
                                            'There is information, update Min/Max values 
                                            selectedTest.MinConcentration = lstPreviousLotValues.First.MinConcentration
                                            selectedTest.MaxConcentration = lstPreviousLotValues.First.MaxConcentration
                                        End If
                                    Next
                                Else
                                    'Error getting Tests/Samples information for the Previous saved Lot for the Control; show it
                                    ShowMessage(Me.Name & ".ChangeLot", resultData.ErrorCode, resultData.ErrorMessage, Me)
                                    cancelLotChange = True
                                End If

                                'Recalculate Target Values (use Column 7 - MaxConcentration to call the function) 
                                For i As Integer = 0 To bsTestListGrid.Rows.Count - 1
                                    CalculatedTarget(i, 7)
                                Next
                            Else
                                For Each selectedTest As TestControlsDS.tparTestControlsRow In TestControlsListDS.tparTestControls
                                    'Clean Min/Max and Target values
                                    selectedTest.SetMinConcentrationNull()
                                    selectedTest.SetMaxConcentrationNull()
                                    selectedTest.SetTargetMeanNull()
                                    selectedTest.SetTargetSDNull()
                                Next
                            End If
                        Else
                            'Lot Change cancelled by User due to there are QC Results pending to cumulate
                            cancelLotChange = True
                        End If

                        If (Not cancelLotChange) Then
                            bsLotNumberTextBox.Text = myForm.NewLotNumber
                            bsLotNumberTextBox.BackColor = SystemColors.MenuBar

                            bsActivationTextBox.Text = Now.ToString(SystemInfoManager.OSDateFormat) & " " & Now.ToString(SystemInfoManager.OSShortTimeFormat)
                            bsExpDatePickUpCombo.Value = Convert.ToDateTime(myForm.NewLotExpirationDate)

                            LotChanged = True
                            SaveCurrentLotAsPrevious = myForm.SaveCurrentLotAsPrevious
                        End If
                    End If
                End Using
            End If

            If (resultData.HasError) Then
                'Error getting data of the Previous saved Lot for the Control; show it
                ShowMessage(Me.Name & ".ChangeLot", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ChangeLot", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ChangeLot", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the value enter is numeric or one of the allowed decimals separators 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/05/2011
    ''' Modified by: SA 18/05/2011 - Allow characters "." and "," as decimal separators. Avoid more than one decimal
    '''                              separator in a cell
    '''              TR 28/07/2011 - Set ChangeMade=True when the screen is in edition mode and the pressed Key is not Tab or BackSpace
    '''              SA 21/02/2012 - If the pressed key is BackSpace, allow it (e.Handle=false) and stop
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

            If (EditionMode AndAlso Not ChangesMade AndAlso Not e.KeyChar = ChrW(Keys.Tab) AndAlso Not e.KeyChar = ChrW(Keys.Back)) Then
                ChangesMade = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CheckNumericCell ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CheckNumericCell ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the Global Variables used in the screen to store the selected controls
    ''' and/or to control when there are changes pending to save
    ''' </summary>
    ''' <remarks>
    ''' Modified by: DL 30/03/2011
    ''' </remarks>
    Private Sub CleanGlobalValues()
        Try
            'Initialization of global variables
            selectedControlID = -1
            originalSelectedIndex = -1
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CleanGlobalValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CleanGlobalValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Delete all selected Controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 13/05/2011 - Implementation changed, the previous one was wrong
    ''' Modified by: DL 11/10/2012
    ''' </remarks>
    Private Sub DeleteControls()
        Dim returnedData As New GlobalDataTO

        Try
            If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                'Get the current User from the Application Session
                Dim currentSession As New ApplicationSessionManager
                Dim loggedUser As String = GlobalBase.GetSessionInfo().UserName

                'Load all selected Controls in a typed DataSet controlDS
                Dim myControlsDS As New ControlsDS
                Dim controlRow As ControlsDS.tparControlsRow

                For Each mySelectedItem As ListViewItem In bsControlsListView.SelectedItems
                    controlRow = myControlsDS.tparControls.NewtparControlsRow
                    controlRow.ControlID = CInt(mySelectedItem.Name)
                    controlRow.ControlName = mySelectedItem.Text
                    controlRow.TS_User = loggedUser
                    controlRow.TS_DateTime = Now
                    myControlsDS.tparControls.Rows.Add(controlRow)
                Next

                'Verify if there other elements affected for the deletion ...(QC Results pending to cumulate)
                If (ValidateDependenciesOnDeletedElements(myControlsDS) = Windows.Forms.DialogResult.OK) Then
                    Dim resultData As New GlobalDataTO
                    Dim myControlsDelegate As New ControlsDelegate

                    If (Not resultData.HasError) Then
                        resultData = myControlsDelegate.DeleteControl(Nothing, myControlsDS, AnalyzerIDAttribute, WorkSessionIDAttribute)
                        If (Not resultData.HasError) Then
                            'Refresh the list of Controls and set screen status to Initial Mode
                            LoadControlsList()
                            InitialModeScreenStatus()

                            If (bsControlsListView.Items.Count > 0) Then
                                'Select the first control in the list
                                bsControlsListView.Items(0).Selected = True
                                QueryControl()
                                bsControlsListView.Focus()
                            End If
                        Else
                            'Error deleting the selected Controls; show it
                            ShowMessage(Me.Name & ".DeleteControls", returnedData.ErrorCode, returnedData.ErrorMessage, Me)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteControls", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteControls", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Delete all selected Tests/Sample Types
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/05/2011 - Code moved here from the grid event. Implementation changed
    ''' </remarks>
    Private Sub DeleteTests()
        Try
            If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString()) = DialogResult.Yes) Then
                For Each row As DataGridViewRow In bsTestListGrid.SelectedRows
                    'Get the Test/Sample Type and search them in the DS of Test Controls and delete it
                    Dim qTestControlToRemove As List(Of TestControlsDS.tparTestControlsRow)

                    qTestControlToRemove = (From a In TestControlsListDS.tparTestControls _
                                            Where a.TestID = Convert.ToInt32(row.Cells("TestID").Value) _
                                            AndAlso a.SampleType = row.Cells("SampleType").Value.ToString() _
                                            AndAlso a.TestType = row.Cells("TestType").Value.ToString() _
                                            Select a).ToList()

                    If (qTestControlToRemove.Count = 1) Then
                        'Save the Test/SampleType in a global DS of deleted elements
                        Dim newTestRow As SelectedTestsDS.SelectedTestTableRow
                        newTestRow = DeletedTestsListDS.SelectedTestTable.NewSelectedTestTableRow
                        newTestRow.TestType = qTestControlToRemove.First.TestType
                        newTestRow.TestID = qTestControlToRemove.First.TestID
                        newTestRow.SampleType = qTestControlToRemove.First.SampleType
                        newTestRow.ControlID = selectedControlID
                        DeletedTestsListDS.SelectedTestTable.Rows.Add(newTestRow)
                    End If
                Next

                'Delete deleted Tests/SampleTypes from the global DS used as DataSource of the Tests grid
                For Each deletedTest As SelectedTestsDS.SelectedTestTableRow In DeletedTestsListDS.SelectedTestTable
                    Dim qTestControlToRemove As List(Of TestControlsDS.tparTestControlsRow)

                    qTestControlToRemove = (From a In TestControlsListDS.tparTestControls _
                                            Where a.TestID = deletedTest.TestID _
                                            AndAlso a.SampleType = deletedTest.SampleType _
                                            AndAlso a.TestType = deletedTest.TestType _
                                            Select a).ToList()

                    If (qTestControlToRemove.Count = 1) Then
                        'Delete the Test/SampleType from the global TestControls DS
                        qTestControlToRemove.First().Delete()
                        TestControlsListDS.AcceptChanges()
                    End If
                Next

                'Set the BackColor of the grid columns and enable/disable the DeleteTest Button according the number of selected Tests
                UnselectTestsSampleTypes()

                bsDelTest.Enabled = (TestControlsListDS.tparTestControls.Rows.Count > 0 AndAlso _
                                     bsTestListGrid.SelectedRows.Count > 0)

                ChangesMade = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Sets the screen status to EDIT MODE if it is not InUse, or to READ ONLY MODE if it is InUse in the active Work Session
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 14/06/2012
    ''' </remarks>
    Private Sub EditSelectedControl()
        Try
            'Load screen fields with all data of the selected control
            Dim inUseControl As Boolean = LoadDataOfControl()

            If (Not inUseControl) Then
                'Set screen status to Query Mode
                EditModeScreenStatus()
            Else
                'Set screen status to Read Only Mode 
                ReadOnlyModeScreenStatus()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditSelectedControl ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditSelectedControl ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to EDIT MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by  XB 01/09/2014 - Add Level field - BA #1868
    ''' </remarks>
    Private Sub EditModeScreenStatus()
        Try
            'Set variable for control of changes pending to save to FALSE
            ChangesMade = False
            EditionMode = True

            LotChanged = False 'RH 15/06/2012

            'Buttons
            bsNewButton.Enabled = True
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
            'bsPrintButton.Enabled = False DL 11/05/2012

            'bsNewLotButton.Enabled = True

            bsSearchTestsButton.Enabled = True
            bsSaveButton.Enabled = True
            bsCancelButton.Enabled = True

            'Fields in details areas
            bsControlNameTextbox.Enabled = True
            bsControlNameTextbox.BackColor = Color.White

            bsSampleTypeComboBox.Enabled = True
            bsSampleTypeComboBox.BackColor = Color.White

            'bsLotNumberTextBox.Enabled = True

            bsExpDatePickUpCombo.Enabled = True
            bsExpDatePickUpCombo.BackColor = Color.White

            bsTestListGrid.Enabled = True

            If (bsTestListGrid.Rows.Count > 0) Then
                bsLotNumberTextBox.Enabled = False
                bsLotNumberTextBox.BackColor = SystemColors.MenuBar
                bsNewLotButton.Enabled = True
            Else
                bsLotNumberTextBox.Enabled = True
                bsLotNumberTextBox.BackColor = Color.White
                bsNewLotButton.Enabled = False
            End If

            bsDelTest.Enabled = False

            bsLevelUpDown.Enabled = True
            bsLevelUpDown.BackColor = Color.White

            UnselectTestsSampleTypes()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".EditModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EditModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen, verifying first if there are changes pending to save
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 21/06/2011
    ''' Modified by: RH 14/06/2012
    ''' </remarks>
    Private Sub ExitScreen()
        Try
            If EditionMode AndAlso ExistPendingChanges() Then
                If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString()) = DialogResult.No) Then
                    bsControlNameTextbox.Focus()
                    Return
                End If
            End If

            'TR 11/04/2012 -Disable form on close to avoid any button press.
            Me.Enabled = False

            If (Not Me.Tag Is Nothing) Then
                'A PerformClick() method was executed
                Me.Close()
            Else
                'Normal button click - Open the WS Monitor form and close this one
                IAx00MainMDI.OpenMonitorForm(Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExitScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 11/05/2011 - Set ToolTip for New Lot button, it was missing
    '''              XB 01/09/2014 - Add Level field - BA #1868
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsControlsListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Controls_List", currentLanguage)
            bsControlDefLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Controls_Definition", currentLanguage)
            bsNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", currentLanguage) + ":"
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", currentLanguage) + ":"
            bsLotNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", currentLanguage) + ":"
            bsActivationDateLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ActivationDate", currentLanguage) + ":"
            bsExpirationDateLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Full", currentLanguage) + ":"
            bsControlTestLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage)
            bsSearchTestsButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", currentLanguage) '01/10/2012 - JB: Resource String unification

            'For Tooltips
            bsScreenToolTips.SetToolTip(bsNewButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", currentLanguage))
            bsScreenToolTips.SetToolTip(bsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", currentLanguage))
            bsScreenToolTips.SetToolTip(bsDeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", currentLanguage))
            bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", currentLanguage))

            bsScreenToolTips.SetToolTip(bsNewLotButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", currentLanguage))
            bsScreenToolTips.SetToolTip(bsSearchTestsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestsSelection", currentLanguage))
            bsScreenToolTips.SetToolTip(bsSaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

            ' XB 01/09/2014 - BA #1868
            bsLevelLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_UserLevel", currentLanguage) + ":"
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure and initialize the ListView of Controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/03/2011
    ''' Modified by: SA 11/05/2011 - Removed columns for Previous Control Lot information
    '''              XB 01/09/2014 - Add Level field - BA #1868
    ''' </remarks>
    Private Sub InitializeControlsList()
        Try
            'Initialization of control List
            bsControlsListView.Items.Clear()

            bsControlsListView.Alignment = ListViewAlignment.Left
            bsControlsListView.FullRowSelect = True
            bsControlsListView.MultiSelect = True
            bsControlsListView.Scrollable = True
            bsControlsListView.View = View.Details
            bsControlsListView.HideSelection = False
            bsControlsListView.HeaderStyle = ColumnHeaderStyle.Clickable

            'List columns definition  --> only column containing the Control Name will be visible
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            bsControlsListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Names", currentLanguage), -2, HorizontalAlignment.Left)

            bsControlsListView.Columns.Add("ControlID", 0, HorizontalAlignment.Left)
            bsControlsListView.Columns.Add("ControlName", 0, HorizontalAlignment.Left)
            bsControlsListView.Columns.Add("SampleType", 0, HorizontalAlignment.Left)
            bsControlsListView.Columns.Add("LotNumber", 0, HorizontalAlignment.Left)
            bsControlsListView.Columns.Add("ActivationDate", 0, HorizontalAlignment.Left)
            bsControlsListView.Columns.Add("ExpirationDate", 0, HorizontalAlignment.Left)
            bsControlsListView.Columns.Add("InUse", 0, HorizontalAlignment.Left)
            ' XB 01/09/2014 - BA #1868
            bsControlsListView.Columns.Add("Level", 0, HorizontalAlignment.Left)

            'Fill ListView with the list of existing Control.
            LoadControlsList()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeControlsList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeControlsList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Setup the limits and step increment of all Numeric UpDown controls 
    ''' </summary>
    ''' <remarks>
    ''' Created by: XB 01/09/2014 - BA #1868
    ''' </remarks>
    Private Sub SetUpControlsLimits()
        Try
            Dim myFieldLimitsDS As New FieldLimitsDS()

            myFieldLimitsDS = GetControlsLimits(FieldLimitsEnum.CONTROLS_NUMBER)
            If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                bsLevelUpDown.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                bsLevelUpDown.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                bsLevelUpDown.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                bsLevelUpDown.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetUpControlsLimits", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetUpControlsLimits", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Method in charge to get the controls limits value. 
    ''' </summary>
    ''' <param name="pLimitsID">Limit to get</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: XB 01/09/2014 - BA #1868
    ''' </remarks>
    Private Function GetControlsLimits(ByVal pLimitsID As FieldLimitsEnum, Optional ByVal pAnalyzerModel As String = "") As FieldLimitsDS
        Dim myFieldLimitsDS As New FieldLimitsDS
        Try
            Dim myGlobalDataTO As New GlobalDataTO

            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
            'Load the time Cycles control
            myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, pLimitsID, pAnalyzerModel)

            If Not myGlobalDataTO.HasError Then
                myFieldLimitsDS = CType(myGlobalDataTO.SetDatos, FieldLimitsDS)
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetControlsLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return myFieldLimitsDS

    End Function

    ''' <summary>
    ''' Set the screen controls to INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/03/2011
    ''' Modified by: SA 11/05/2011 - Min allowed ExpirationDate will be ActivationDate + 1 day
    '''                              ActivationDate should be shown with the format defined in the OSCultureInfo
    '''                              Use function Now instead of Today, due to the formated hour returns always 
    '''              XB 01/09/2014 - Add Level field - BA #1868
    ''' </remarks> 
    Private Sub InitialModeScreenStatus(Optional ByVal pInitializeListView As Boolean = True)
        Try
            'Set variable for control of changes pending to save to FALSE
            ChangesMade = False
            EditionMode = False

            If (pInitializeListView) Then
                'Area of Controls List
                If (bsControlsListView.Items.Count > 0) Then
                    bsControlsListView.SelectedItems.Clear()
                End If

                bsNewButton.Enabled = True
                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
                '                bsPrintButton.Enabled = False DL 11/05/2012
            End If

            'Area of Controls Definition
            bsControlNameTextbox.Text = ""
            bsControlNameTextbox.Enabled = False
            bsControlNameTextbox.BackColor = SystemColors.MenuBar

            bsSampleTypeComboBox.Enabled = False
            bsSampleTypeComboBox.SelectedValue = -1
            bsSampleTypeComboBox.BackColor = SystemColors.MenuBar

            bsLotNumberTextBox.Text = ""
            bsLotNumberTextBox.Enabled = False
            bsLotNumberTextBox.BackColor = SystemColors.MenuBar

            bsNewLotButton.Enabled = False
            bsActivationTextBox.Text = Now.ToString(SystemInfoManager.OSDateFormat) & " " & Now.ToString(SystemInfoManager.OSShortTimeFormat)
            bsActivationTextBox.BackColor = SystemColors.MenuBar ' dl 18/07/2011 Color.Gainsboro

            bsExpDatePickUpCombo.MinDate = CDate(bsActivationTextBox.Text).AddDays(1)
            bsExpDatePickUpCombo.Enabled = False
            bsExpDatePickUpCombo.Value = Now.AddMonths(3)
            bsExpDatePickUpCombo.BackColor = SystemColors.MenuBar

            bsLevelUpDown.Enabled = False
            bsLevelUpDown.BackColor = SystemColors.MenuBar

            DeletedTestsListDS = New SelectedTestsDS
            TestControlsListDS = New TestControlsDS
            bsTestListGrid.DataSource = TestControlsListDS.tparTestControls
            bsTestListGrid.Enabled = False
            bsSearchTestsButton.Enabled = False
            bsDelTest.Enabled = False

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
    ''' Fill Control data in the correspondent structure
    ''' </summary>
    ''' <returns>A typed DataSet ControlsDS containing all data of the Controls</returns>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: XB 01/09/2014 - inform field ControlLevel with the value of the added NumericUpDown - BA #1868
    ''' </remarks>
    Private Function LoadControlsDS() As ControlsDS
        Dim controlRow As ControlsDS.tparControlsRow
        Dim controlData As New ControlsDS

        Try
            controlRow = controlData.tparControls.NewtparControlsRow

            controlRow.ControlID = selectedControlID
            controlRow.ControlName = bsControlNameTextbox.Text.Trim
            controlRow.SampleType = bsSampleTypeComboBox.SelectedValue.ToString
            controlRow.LotNumber = bsLotNumberTextBox.Text.Trim
            controlRow.ActivationDate = CDate(bsActivationTextBox.Text)
            controlRow.ExpirationDate = CDate(bsExpDatePickUpCombo.Value)
            ' XB 01/09/2014 - BA #1868
            controlRow.ControlLevel = CType(bsLevelUpDown.Value, Integer)

            'Gets from the Session the Username of the connected User; get also the current datetime
            Dim currentSession As New ApplicationSessionManager
            controlRow.TS_User = GlobalBase.GetSessionInfo().UserName
            controlRow.TS_DateTime = Now

            controlData.tparControls.Rows.Add(controlRow)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadControlsDS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadControlsDS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return controlData
    End Function

    ''' <summary>
    ''' Load the ListView of Controls with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/03/2011
    ''' Modified by: SA 11/05/2011 - Removed columns for Previous Control Lot information
    '''              XB 01/09/2014 - Add Level field - BA #1868
    ''' </remarks>
    Private Sub LoadControlsList()
        Try
            Dim myIcons As New ImageList

            'Get the Icon defined for controls that are not in use in the current Work Session
            Dim notInUseIcon As String = GetIconName("CTRL")
            If (notInUseIcon <> "") Then
                myIcons.Images.Add("CTRL", Image.FromFile(MyBase.IconsPath & notInUseIcon))
            End If

            'Get the Icon defined for Controls that are not in use in the current Work Session
            Dim inUseIcon As String = GetIconName("INUSECTRL")
            If (inUseIcon <> "") Then
                myIcons.Images.Add("INUSECTRL", Image.FromFile(MyBase.IconsPath & inUseIcon))
            End If

            'Assign the Icons to the Controls
            bsControlsListView.Items.Clear()
            bsControlsListView.SmallImageList = myIcons

            'Get the list of existing Controls
            Dim resultData As New GlobalDataTO
            Dim controlList As New ControlsDelegate

            resultData = controlList.GetAll(Nothing)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim myControlDS As ControlsDS = DirectCast(resultData.SetDatos, ControlsDS)

                'Sort the returned Controls
                Dim qControls As List(Of ControlsDS.tparControlsRow)
                Select Case bsControlsListView.Sorting
                    Case SortOrder.Ascending
                        qControls = (From a In myControlDS.tparControls _
                                     Select a Order By a.ControlName Ascending).ToList()
                    Case SortOrder.Descending
                        qControls = (From a In myControlDS.tparControls _
                                     Select a Order By a.ControlName Descending).ToList()
                    Case SortOrder.None
                        qControls = (From a In myControlDS.tparControls _
                                     Select a).ToList()
                    Case Else
                        qControls = (From a In myControlDS.tparControls _
                                     Select a).ToList()
                End Select

                'Fill the List View with the list of sorted controls
                Dim i As Integer = 0
                Dim rowToSelect As Integer = -1

                For Each controlRow As ControlsDS.tparControlsRow In qControls
                    bsControlsListView.Items.Add(controlRow.ControlID.ToString, _
                                                 controlRow.ControlName.ToString, _
                                                 controlRow.IconPath.ToString).Tag = controlRow.InUse

                    bsControlsListView.Items(i).SubItems.Add(controlRow.ControlID.ToString)
                    bsControlsListView.Items(i).SubItems.Add(controlRow.ControlName.ToString)
                    bsControlsListView.Items(i).SubItems.Add(controlRow.SampleType.ToString)
                    bsControlsListView.Items(i).SubItems.Add(controlRow.LotNumber.ToString)
                    bsControlsListView.Items(i).SubItems.Add(controlRow.ActivationDate.ToString)
                    bsControlsListView.Items(i).SubItems.Add(controlRow.ExpirationDate.ToString)
                    bsControlsListView.Items(i).SubItems.Add(controlRow.InUse.ToString)
                    ' XB 01/09/2014 - BA #1868
                    bsControlsListView.Items(i).SubItems.Add(controlRow.ControlLevel.ToString)

                    'If there is a selected control and it is still in the list, its position is stored to re-select 
                    'the same control once the list is loaded
                    If (selectedControlID = Convert.ToInt32(controlRow.ControlID)) Then rowToSelect = i
                    i += 1
                Next controlRow

                ChangesMade = False
                If (rowToSelect = -1) Then
                    'There was not a selected control or the selected one is not in the list; the global variables containing 
                    'information of the selected control is initializated
                    CleanGlobalValues()
                Else
                    'If there is a selected controls, focus is put in the correspondent element in the controls List
                    bsControlsListView.Items(rowToSelect).Selected = True
                    bsControlsListView.Select()

                    'The global variable containing the index of the selected controls is updated
                    originalSelectedIndex = bsControlsListView.SelectedIndices(0)
                End If
            End If

            'An error has happened getting data from the Database
            If (resultData.HasError) Then
                ShowMessage(Me.Name & ".LoadControlsList", resultData.ErrorCode, resultData.ErrorMessage)
                CleanGlobalValues()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadControlsList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadControlsList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill screen fields with data of the selected Control
    ''' </summary>
    ''' <returns>True if the control is In Use; otherwise False</returns>
    ''' <remarks>
    ''' Created by:  DL 30/03/2011
    ''' Modified by: SA 11/05/2011 - Removed load of Previous Control Lot values
    '''              XB 01/09/2014 - Add Level field - BA #1868
    ''' </remarks>
    Private Function LoadDataOfControl() As Boolean
        Dim inUse As Boolean = False

        Try
            If (bsControlsListView.SelectedItems.Count = 0) Then bsControlsListView.Items(0).Selected = True

            selectedControlID = CInt(bsControlsListView.SelectedItems(0).SubItems(1).Text)
            originalSelectedIndex = bsControlsListView.SelectedIndices(0)

            'Fill screen controls with data of the selected controls
            bsControlNameTextbox.Text = bsControlsListView.SelectedItems(0).SubItems(0).Text
            bsSampleTypeComboBox.SelectedValue = bsControlsListView.SelectedItems(0).SubItems(3).Text.Trim
            bsLotNumberTextBox.Text = bsControlsListView.SelectedItems(0).SubItems(4).Text

            Dim activationDate As Date = Convert.ToDateTime(bsControlsListView.SelectedItems(0).SubItems(5).Text)
            bsActivationTextBox.Text = activationDate.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                     activationDate.ToString(SystemInfoManager.OSShortTimeFormat)

            bsExpDatePickUpCombo.MinDate = CDate(bsActivationTextBox.Text)
            bsExpDatePickUpCombo.Value = CDate(bsControlsListView.SelectedItems(0).SubItems(6).Text)
            inUse = Convert.ToBoolean(bsControlsListView.SelectedItems(0).SubItems(7).Text)

            ' XB 01/09/2014 - BA #1868
            bsLevelUpDown.Value = CInt(bsControlsListView.SelectedItems(0).SubItems(8).Text)

            'Load the list of Tests/SampleTypes linked to the selected Control
            LoadTestControlsGrid()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadDataOfControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadDataOfControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return inUse
    End Function

    ''' <summary>
    ''' Load the GridView of Test Controls with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 01/04/2011
    ''' Modified by: SA 11/05/2011 - Removed the sorting 
    ''' </remarks>
    Private Sub LoadTestControlsGrid()
        Try
            If (bsTestControlPanel.Visible) Then
                'Get the list of existing test controls
                Dim resultData As New GlobalDataTO
                Dim myTestControlsDelegate As New TestControlsDelegate

                resultData = myTestControlsDelegate.GetTestControlsByControlIDNEW(Nothing, selectedControlID)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    TestControlsListDS = DirectCast(resultData.SetDatos, TestControlsDS)

                    'Sort the Tests/SampleTypes by TestPosition
                    Dim viewTests As DataView = New DataView
                    viewTests = TestControlsListDS.tparTestControls.DefaultView
                    viewTests.Sort = "TestType DESC, TestPosition, SampleType "

                    'Set the sorted DS as DataSource of Tests/SampleTypes grid
                    Dim bsTestsBindingSource As BindingSource = New BindingSource
                    bsTestsBindingSource.DataSource = viewTests
                    bsTestListGrid.DataSource = bsTestsBindingSource
                Else
                    'Error getting the list of Tests/SampleTypes linked to the Control
                    ShowMessage(Me.Name & ".LoadTestControlsGrid", resultData.ErrorCode, resultData.ErrorMessage)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadTestControlsGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadTestControlsGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBox of Sample Types with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/03/2011
    ''' Modified by: SA 11/05/2011 - Removed not used code 
    ''' </remarks>
    Private Sub LoadSampleTypesList()
        Try
            'Get the list of existing Sample Types
            Dim myGlobalDataTo As New GlobalDataTO
            Dim masterDataConfig As New MasterDataDelegate
            myGlobalDataTo = masterDataConfig.GetList(Nothing, MasterDataEnum.SAMPLE_TYPES.ToString)

            If (Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing) Then
                Dim masterDataDS As MasterDataDS = DirectCast(myGlobalDataTo.SetDatos, MasterDataDS)

                If (masterDataDS.tcfgMasterData.Rows.Count > 0) Then
                    'Fill ComboBox of Sample Types in area of Calculated Test details
                    bsSampleTypeComboBox.DataSource = masterDataDS.tcfgMasterData
                    bsSampleTypeComboBox.DisplayMember = "ItemIDDesc"
                    bsSampleTypeComboBox.ValueMember = "ItemID"
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

    ''' <summary>
    ''' Verify if there is at least one User's change pending to save
    ''' </summary>
    ''' <returns>True if there are changes pending to save; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 12/05/2011 - Implementation changed; removed verification of changes in Tests/SampleTypes grid
    '''              XB 01/09/2014 - Add Level field - BA #1868
    ''' </remarks> 
    Public Function ExistPendingChanges() As Boolean
        Try
            If EditionMode AndAlso Not ChangesMade Then
                If (selectedControlID <= 0) Then
                    'In Add Mode, if at least one of the values has been changed, then there are changes pending to save
                    If (bsControlNameTextbox.Text.Trim <> "") OrElse (bsSampleTypeComboBox.Text <> "") OrElse _
                       (bsLotNumberTextBox.Text.Trim <> "") OrElse (bsTestListGrid.Rows.Count > 0) Then
                        ChangesMade = True
                    End If
                Else
                    If (bsControlsListView.SelectedItems.Count = 0) Then
                        'If there is a control in edition and a click is made out of the list of Tests,
                        'the Test in edition is selected to avoid errors if the screen is closed or the edition cancelled
                        If (originalSelectedIndex > 0) Then
                            bsControlsListView.Items(originalSelectedIndex).Selected = True
                            bsControlsListView.Select()
                        End If
                    End If

                    Dim selectedSample As String = ""

                    If (Not bsSampleTypeComboBox.SelectedValue Is Nothing) Then
                        selectedSample = bsSampleTypeComboBox.SelectedValue.ToString()
                    End If

                    If (bsControlNameTextbox.Text.Trim <> bsControlsListView.Items(originalSelectedIndex).Text) OrElse _
                       (selectedSample <> bsControlsListView.Items(originalSelectedIndex).SubItems(3).Text.Trim) OrElse _
                       (bsLotNumberTextBox.Text.Trim <> bsControlsListView.Items(originalSelectedIndex).SubItems(4).Text.Trim) OrElse _
                       (bsExpDatePickUpCombo.Value.ToString("yyyyMMdd") <> Convert.ToDateTime(bsControlsListView.Items(originalSelectedIndex).SubItems(6).Text).ToString("yyyyMMdd")) OrElse _
                       (bsLevelUpDown.Text.Trim <> bsControlsListView.Items(originalSelectedIndex).SubItems(8).Text.Trim) Then
                        ChangesMade = True
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExistPendingChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExistPendingChanges ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try

        Return ChangesMade
    End Function

    ''' <summary>
    ''' Method incharge of loading the image for each button
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/03/2011
    ''' Modified by: SA 11/05/2011 - Removed getting of Icons for Tests (Preloaded and User Defined, InUse or Not)
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim iconPath As String = MyBase.IconsPath
            Dim auxIconName As String = ""

            'NEW Button
            auxIconName = GetIconName("ADD")
            If (auxIconName <> "") Then
                bsNewButton.Image = Image.FromFile(iconPath & auxIconName)
                bsNewLotButton.Image = Image.FromFile(iconPath & auxIconName)
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
                bsDelTest.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = Image.FromFile(iconPath & auxIconName)
            End If
            'JB 30/08/2012 - Hide Print button
            bsPrintButton.Visible = False


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
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure the grid for details of Tests/SampleTypes linked to a Control
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 05/04/2011
    ''' Modified by: SA 11/05/2011 - Removed not needed columns; set alignment for all visible columns.
    ''' </remarks>
    Private Sub PrepareTestsSamplesGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsTestListGrid.Rows.Clear()
            bsTestListGrid.Columns.Clear()

            bsTestListGrid.AutoGenerateColumns = False
            bsTestListGrid.AllowUserToAddRows = False
            bsTestListGrid.AllowUserToResizeColumns = True
            bsTestListGrid.AllowUserToDeleteRows = False
            bsTestListGrid.EditMode = DataGridViewEditMode.EditOnEnter

            'TestType Icon (Preloaded or User Defined)
            Dim ImageCol As New DataGridViewImageColumn
            ImageCol.Name = "TestTypeIcon"
            ImageCol.DataPropertyName = "TestTypeIcon"
            ImageCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", currentLanguage)
            bsTestListGrid.Columns.Add(ImageCol)
            bsTestListGrid.Columns("TestTypeIcon").Width = 40
            bsTestListGrid.Columns("TestTypeIcon").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns("TestTypeIcon").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Test Identifier
            Dim TestIDCol As New DataGridViewTextBoxColumn
            TestIDCol.Name = "TestID"
            TestIDCol.DataPropertyName = "TestID"
            TestIDCol.Visible = False
            bsTestListGrid.Columns.Add(TestIDCol)

            'Test Name
            Dim TestNameCol As New DataGridViewTextBoxColumn
            TestNameCol.Name = "TestName"
            TestNameCol.DataPropertyName = "TestName"
            TestNameCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", currentLanguage)
            TestNameCol.ReadOnly = True
            bsTestListGrid.Columns.Add(TestNameCol)
            bsTestListGrid.Columns("TestName").Width = 145 '150 SA '156 JB - 09/11/2012
            bsTestListGrid.Columns("TestName").SortMode = DataGridViewColumnSortMode.Automatic
            bsTestListGrid.Columns("TestName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Sample Type Code
            Dim SampleCol As New DataGridViewTextBoxColumn
            SampleCol.Name = "SampleType"
            SampleCol.DataPropertyName = "SampleType"
            SampleCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", currentLanguage)
            SampleCol.ReadOnly = True
            bsTestListGrid.Columns.Add(SampleCol)
            bsTestListGrid.Columns("SampleType").Width = 65
            bsTestListGrid.Columns("SampleType").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns("SampleType").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Test Measure Unit
            Dim UnitCol As New DataGridViewTextBoxColumn
            UnitCol.Name = "MeasureUnit"
            UnitCol.DataPropertyName = "MeasureUnit"
            UnitCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", currentLanguage)
            UnitCol.ReadOnly = True
            bsTestListGrid.Columns.Add(UnitCol)
            bsTestListGrid.Columns("MeasureUnit").Width = 66 '70 JB 09/11/2012
            bsTestListGrid.Columns("MeasureUnit").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns("MeasureUnit").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Rejection Criteria (kSD) defined for the Test/SampleType
            Dim kSDCol As New DataGridViewTextBoxColumn
            kSDCol.Name = "RejectionCriteria"
            kSDCol.DataPropertyName = "RejectionCriteria"
            kSDCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_kSD", currentLanguage)
            kSDCol.ReadOnly = True
            bsTestListGrid.Columns.Add(kSDCol)
            bsTestListGrid.Columns("RejectionCriteria").Width = 50
            bsTestListGrid.Columns("RejectionCriteria").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns("RejectionCriteria").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Min Concentration
            Dim MinCol As New DataGridViewTextBoxColumn
            MinCol.Name = "MinConcentration"
            MinCol.DataPropertyName = "MinConcentration"
            MinCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Min", currentLanguage)
            MinCol.MaxInputLength = 11
            bsTestListGrid.Columns.Add(MinCol)
            bsTestListGrid.Columns("MinConcentration").Width = 52 '55 SA
            bsTestListGrid.Columns("MinConcentration").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns("MinConcentration").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Max Concentration
            Dim MaxCol As New DataGridViewTextBoxColumn
            MaxCol.Name = "MaxConcentration"
            MaxCol.DataPropertyName = "MaxConcentration"
            MaxCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Max", currentLanguage)
            MaxCol.MaxInputLength = 11
            bsTestListGrid.Columns.Add(MaxCol)
            bsTestListGrid.Columns("MaxConcentration").Width = 52  '55 SA
            bsTestListGrid.Columns("MaxConcentration").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns("MaxConcentration").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Calculated Target Mean
            Dim TargetMeanCol As New DataGridViewTextBoxColumn
            TargetMeanCol.Name = "TargetMean"
            TargetMeanCol.DataPropertyName = "TargetMean"
            TargetMeanCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TargetMean", currentLanguage)
            TargetMeanCol.ReadOnly = True
            bsTestListGrid.Columns.Add(TargetMeanCol)
            bsTestListGrid.Columns("TargetMean").Width = 85 '105 SA '85 JB - 09/11/2012
            bsTestListGrid.Columns("TargetMean").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns("TargetMean").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Calculated Target SD
            Dim TargetSDCol As New DataGridViewTextBoxColumn
            TargetSDCol.Name = "TargetSD"
            TargetSDCol.DataPropertyName = "TargetSD"
            TargetSDCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TargetSD", currentLanguage)
            TargetSDCol.ReadOnly = True
            bsTestListGrid.Columns.Add(TargetSDCol)
            bsTestListGrid.Columns("TargetSD").Width = 79
            bsTestListGrid.Columns("TargetSD").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns("TargetSD").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight


            'Number of Decimals allowed for the Test
            Dim DecimalsAllowedCol As New DataGridViewTextBoxColumn
            DecimalsAllowedCol.Name = "DecimalsAllowed"
            DecimalsAllowedCol.DataPropertyName = "DecimalsAllowed"
            DecimalsAllowedCol.Visible = False
            bsTestListGrid.Columns.Add(DecimalsAllowedCol)

            'Flag indicating if the Control is marked as Active for the Test/SampleType
            Dim ActiveControlChkBoxCol As New DataGridViewCheckBoxColumn
            ActiveControlChkBoxCol.Name = "ActiveControl"
            ActiveControlChkBoxCol.DataPropertyName = "ActiveControl"
            ActiveControlChkBoxCol.Visible = False
            bsTestListGrid.Columns.Add(ActiveControlChkBoxCol)

            'Test Type
            Dim TestTypeCol As New DataGridViewTextBoxColumn
            TestTypeCol.Name = "TestType"
            TestTypeCol.DataPropertyName = "TestType"
            TestTypeCol.Visible = False
            bsTestListGrid.Columns.Add(TestTypeCol)

            bsTestListGrid.Columns("TestTypeIcon").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestListGrid.Columns("TestName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsTestListGrid.Columns("SampleType").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestListGrid.Columns("MeasureUnit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestListGrid.Columns("RejectionCriteria").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListGrid.Columns("MinConcentration").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListGrid.Columns("MaxConcentration").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListGrid.Columns("TargetMean").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListGrid.Columns("TargetSD").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareTestsSamplesGrid " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestsSamplesGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get data of the selected control, fill the correspondent variables and controls and set the screen status 
    ''' to Read-only Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 30/03/2011
    ''' </remarks>
    Private Sub QueryControl()
        Dim setScreenToQuery As Boolean = False

        Try
            If (bsControlsListView.SelectedIndices(0) <> originalSelectedIndex) Then
                If ExistPendingChanges() Then
                    If (ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                        setScreenToQuery = True
                    Else
                        If (originalSelectedIndex <> -1) Then
                            bsControlsListView.SelectedItems.Clear()

                            'Return focus to the control that has been edited
                            bsControlsListView.Items(originalSelectedIndex).Selected = True
                            bsControlsListView.Select()
                        End If
                    End If
                Else
                    setScreenToQuery = True
                End If

                If (setScreenToQuery) Then
                    'Load screen fields with all data of the selected control
                    Dim inUseControl As Boolean = LoadDataOfControl()
                    If (Not inUseControl) Then
                        'Set screen status to Query Mode
                        QueryModeScreenStatus()
                    Else
                        'Set screen status to Read Only Mode 
                        ReadOnlyModeScreenStatus()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Screen display in query mode when the KeyUp or KeyDown key is pressed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: TR 13/10/2011 - Do not validate the key pressed
    ''' </remarks>
    Private Sub QueryControlByMoveUpDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        Try
            QueryControl()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryControlByMoveUpDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryControlByMoveUpDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to QUERY MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: XB 01/09/2014 - Add Level field - BA #1868
    ''' </remarks>
    Private Sub QueryModeScreenStatus()
        Try
            EditionMode = False
            ChangesMade = False

            'Buttons Edit and Delete Controls are enabled only if the screen is opened from Menu
            If (SourceScreenAttribute = GlbSourceScreen.TEST_QCTAB.ToString) Then
                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
            Else
                bsEditButton.Enabled = True
                bsDeleteButton.Enabled = True
            End If

            'New and Print Buttons are always active
            bsNewButton.Enabled = True
            '            bsPrintButton.Enabled = True DL 11/05/2012

            'Buttons in details areas are always disabled in Query Mode
            bsNewLotButton.Enabled = False
            bsDelTest.Enabled = False
            bsSearchTestsButton.Enabled = False
            bsSaveButton.Enabled = False
            bsCancelButton.Enabled = False

            'All controls in details areas are disabled
            bsControlNameTextbox.Enabled = False
            bsControlNameTextbox.BackColor = SystemColors.MenuBar

            bsSampleTypeComboBox.Enabled = False
            bsSampleTypeComboBox.BackColor = SystemColors.MenuBar

            bsLotNumberTextBox.Enabled = False
            bsLotNumberTextBox.BackColor = SystemColors.MenuBar

            bsExpDatePickUpCombo.Enabled = False
            bsExpDatePickUpCombo.BackColor = SystemColors.MenuBar

            bsLevelUpDown.Enabled = False
            bsLevelUpDown.BackColor = SystemColors.MenuBar

            'Tests/SampleTypes grid is disabled
            bsTestControlPanel.Visible = True
            bsTestListGrid.Enabled = False
            UnselectTestsSampleTypes()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen controls to READ ONLY MODE (for InUse Controls)
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL
    ''' </remarks>
    Private Sub ReadOnlyModeScreenStatus()
        Try
            'Set all controls to QUERY MODE
            QueryModeScreenStatus()

            'Disable all buttons that cannot be used in Read Only Mode
            bsEditButton.Enabled = False
            bsDeleteButton.Enabled = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReadOnlyModeScreenStatus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReadOnlyModeScreenStatus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save data of the added or updated Controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 24/05/2011 - Validate if there are affected Elements before save changes
    ''' </remarks>
    Private Sub SaveChanges()
        Try
            'Verify if the Control Test can be saved
            If (ValidateSavingConditions()) Then
                'Validate if there are affected Elements
                If (ValidateDependenciesOnUpdate() = Windows.Forms.DialogResult.OK) Then
                    'Fill basic data of the Control in a ControlsDS and save
                    Dim controlDataDS As ControlsDS = LoadControlsDS()
                    If (SaveControl(controlDataDS)) Then
                        If (selectedControlID > 0) Then
                            'Reload the Controls List 
                            LoadControlsList()
                            QueryModeScreenStatus()
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveChanges", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveChanges", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Save (add/update) the Control being edited
    ''' </summary>
    ''' <param name="pControlDS">Typed DataSet ControlsDS containing basic data of the Control to add/update</param>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' Modified by: SA 12/05/2011 - New implementation
    ''' Modified by: RH 15/06/2012
    ''' </remarks>
    Private Function SaveControl(ByVal pControlDS As ControlsDS) As Boolean
        Dim savingExecuted As Boolean = True

        Try
            Dim returnedData As New GlobalDataTO
            Dim controlToSave As New ControlsDelegate

            If (selectedControlID <= 0) Then
                'Insert a new Control
                returnedData = controlToSave.AddControlNEW(Nothing, pControlDS, TestControlsListDS, True)
            Else
                'Update the Control
                Dim changedLotNumber As String = ""
                If (LotChanged) Then changedLotNumber = bsControlsListView.SelectedItems(0).SubItems(4).Text

                returnedData = controlToSave.ModifyControlNEW(Nothing, pControlDS, TestControlsListDS, DeletedTestsListDS, SaveCurrentLotAsPrevious, changedLotNumber)
            End If

            If (Not returnedData.HasError) Then
                If (selectedControlID <= 0) Then
                    'Get the generated ControlID and show data of the added control in Read Only Mode
                    selectedControlID = DirectCast(returnedData.SetDatos, ControlsDS).tparControls(0).ControlID
                End If
            Else
                'Error adding/updating the Control; shown it
                ShowMessage(Me.Name & ".SaveControl", returnedData.ErrorCode, returnedData.ErrorMessage)
                bsControlNameTextbox.Focus()
                savingExecuted = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            savingExecuted = False
        End Try

        Return savingExecuted
    End Function

    ''' <summary>
    ''' Load all data needed for the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/03/2011 
    ''' Modified by: SA 18/05/2011 - Get limits (min and max allowed values) for Min/Max Concentration fields 
    '''              DL 28/07/2011 - Set the screen location when it is opened
    '''              XB 01/09/2014 - Call SetUpControlsLimits funcion - BA #1868
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            If (String.Compare(SourceScreenAttribute, GlbSourceScreen.TEST_QCTAB.ToString, False) = 0) Then
                Dim myLocation As Point = IAx00MainMDI.Location
                Dim mySize As Size = IAx00MainMDI.Size

                Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            End If

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels
            GetScreenLabels()

            'Get Icons for graphical buttons
            PrepareButtons()

            'Get allowed limits for fields Min/Max Concentration
            Dim resultData As New GlobalDataTO
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate

            resultData = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.CONTROL_MIN_MAX_CONC)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myFieldLimitsDS As FieldLimitsDS = DirectCast(resultData.SetDatos, FieldLimitsDS)
                If (myFieldLimitsDS.tfmwFieldLimits.Rows.Count = 1) Then
                    MinAllowedConcentration = myFieldLimitsDS.tfmwFieldLimits(0).MinValue
                    MaxAllowedConcentration = myFieldLimitsDS.tfmwFieldLimits(0).MaxValue
                End If
            Else
                'Error getting limits for fields Min/Max Concentration
                ShowMessage(Me.Name & ".ScreenLoad", resultData.ErrorCode, resultData.ErrorMessage)
            End If

            If (Not resultData.HasError) Then
                'Load the list of available Sample Types
                LoadSampleTypesList()

                'Configure and load the list of existing Controls
                InitializeControlsList()

                SetUpControlsLimits()   ' XB 01/09/2014 - BA #1868

                'Configure the grid for Tests/SampleTypes
                PrepareTestsSamplesGrid()
                bsTestControlPanel.Visible = True

                'Reset the Screen border when the screen is not opened as pop up
                Select Case (SourceScreenAttribute)
                    Case GlbSourceScreen.STANDARD.ToString
                        ResetBorder() 'To avoid flickering while the screen is opening
                End Select

                'Set Screen Status to INITIAL MODE
                InitialModeScreenStatus()

                If (bsControlsListView.Items.Count > 0) Then
                    'Select the first Control in the list
                    bsControlsListView.Items(0).Selected = True
                    QueryControl()
                    selectedControlID = CInt(bsControlsListView.SelectedItems(0).SubItems(1).Text)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Enable or disable functionality according the Level of the connected User 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 23/04/2012
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
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
                    '                    bsPrintButton.Enabled = False DL 11/05/2012
                    bsDelTest.Enabled = False
                    bsSaveButton.Enabled = False
                    bsCancelButton.Enabled = False
                    Exit Select
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen of Tests selection
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 11/05/2011 - Code moved here from the button event
    ''' Modified by: SA 12/05/2011 - Removed the not needed code. Change the way of load the grid 
    ''' Modified by: RH 14/06/2012 - Adapted for ISE TestType. Code optimization.
    ''' </remarks>
    Private Sub SearchTests()
        Try
            'If there are Tests/SampleTypes in the grid, load them in a DS of Selected Elements
            Dim myTests As New SelectedTestsDS
            Dim newTestRow As SelectedTestsDS.SelectedTestTableRow

            For Each selectedTest As TestControlsDS.tparTestControlsRow In TestControlsListDS.tparTestControls.Rows
                newTestRow = myTests.SelectedTestTable.NewSelectedTestTableRow
                newTestRow.TestType = selectedTest.TestType
                newTestRow.TestID = selectedTest.TestID
                newTestRow.SampleType = selectedTest.SampleType
                newTestRow.OTStatus = "OPEN"
                newTestRow.TestPosition = selectedTest.TestPosition
                myTests.SelectedTestTable.Rows.Add(newTestRow)
            Next

            'Inform Properties and open the auxiliary screen of Tests selection
            Using myForm As New IWSTestSelectionAuxScreen
                myForm.WorkingModel = GlbSourceScreen.MODEL2.ToString()
                myForm.ControlID = selectedControlID
                myForm.ListOfSelectedTests = myTests

                If Not String.IsNullOrEmpty(bsSampleTypeComboBox.Text) Then
                    myForm.SampleType = bsSampleTypeComboBox.SelectedValue.ToString()
                End If

                myForm.ShowDialog()

                If (myForm.DialogResult = DialogResult.OK) AndAlso (Not myForm.ListOfSelectedTests Is Nothing) Then
                    'Get all Tests/SampleTypes that have been deleted 
                    Dim lstDeletedTests As List(Of SelectedTestsDS.SelectedTestTableRow)
                    lstDeletedTests = (From a In myForm.ListOfSelectedTests.SelectedTestTable _
                                       Where a.OTStatus = "DELETED").ToList()

                    If (lstDeletedTests.Count > 0) Then
                        ChangesMade = True

                        For Each myTest As TestControlsDS.tparTestControlsRow In TestControlsListDS.tparTestControls
                            'Search if the Test/SampleType is in the list of deleted ones
                            Dim myTestSample As List(Of SelectedTestsDS.SelectedTestTableRow)
                            myTestSample = (From b In lstDeletedTests _
                                            Where b.TestID = myTest.TestID _
                                            AndAlso b.TestType = myTest.TestType _
                                            AndAlso String.Compare(b.SampleType, myTest.SampleType, False) = 0).ToList()

                            If (myTestSample.Count = 1) Then
                                myTest.Delete()

                                'Save the deleted Test/SampleType in a global DS
                                Dim deletedTestRow As SelectedTestsDS.SelectedTestTableRow
                                deletedTestRow = DeletedTestsListDS.SelectedTestTable.NewSelectedTestTableRow
                                deletedTestRow.TestType = myTestSample.First.TestType
                                deletedTestRow.TestID = myTestSample.First.TestID
                                deletedTestRow.SampleType = myTestSample.First.SampleType
                                deletedTestRow.ControlID = selectedControlID
                                DeletedTestsListDS.SelectedTestTable.Rows.Add(deletedTestRow)
                            End If
                        Next

                        TestControlsListDS.AcceptChanges()
                    End If

                    'Get all new selected Tests/SampleTypes
                    Dim lstAddedTests As List(Of SelectedTestsDS.SelectedTestTableRow)
                    lstAddedTests = (From a In myForm.ListOfSelectedTests.SelectedTestTable _
                                     Where a.OTStatus = "NEW").ToList()

                    If (lstAddedTests.Count > 0) Then
                        ChangesMade = True

                        'Get Icons for Preloaded and User-defined Standard Tests
                        Dim preloadedDataConfig As New PreloadedMasterDataDelegate
                        Dim imageTest As Byte() = preloadedDataConfig.GetIconImage("TESTICON")
                        Dim imageUserTest As Byte() = preloadedDataConfig.GetIconImage("USERTEST")
                        Dim imageISETest As Byte() = preloadedDataConfig.GetIconImage("TISE_SYS")

                        Dim addedTestRow As TestControlsDS.tparTestControlsRow

                        For Each addedTest As SelectedTestsDS.SelectedTestTableRow In lstAddedTests
                            addedTestRow = TestControlsListDS.tparTestControls.NewtparTestControlsRow
                            addedTestRow.TestID = addedTest.TestID
                            addedTestRow.TestName = addedTest.TestName
                            addedTestRow.SampleType = addedTest.SampleType
                            addedTestRow.TestType = addedTest.TestType
                            addedTestRow.DecimalsAllowed = addedTest.DecimalsAllowed
                            addedTestRow.RejectionCriteria = addedTest.RejectionCriteria
                            addedTestRow.MeasureUnit = addedTest.MeasureUnit
                            addedTestRow.PreloadedTest = addedTest.PreloadedTest
                            addedTestRow.TestPosition = addedTest.TestPosition
                            addedTestRow.ControlID = selectedControlID

                            If (addedTest.TestType = "STD") Then
                                If (addedTest.PreloadedTest) Then
                                    addedTestRow.TestTypeIcon = imageTest
                                Else
                                    addedTestRow.TestTypeIcon = imageUserTest
                                End If
                            ElseIf (addedTestRow.TestType = "ISE") Then
                                addedTestRow.TestTypeIcon = imageISETest
                            End If
                            TestControlsListDS.tparTestControls.Rows.Add(addedTestRow)
                        Next

                        TestControlsListDS.AcceptChanges()
                    End If

                    'Set the BackColor of the grid columns and enable/disable the DeleteTest Button according the number of selected Tests
                    UnselectTestsSampleTypes()

                    bsDelTest.Enabled = (TestControlsListDS.tparTestControls.Rows.Count > 0 AndAlso _
                                         bsTestListGrid.SelectedRows.Count > 0)
                End If

            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SearchTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SearchTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' If the value informed is less than the allowed, set the minimum allowed as cell value
    ''' If the value informed is greater than the allowed, set the maximum allowed as cell value
    ''' Finally, apply format to numeric cells:
    ''' ** Min, Max and TargetMean are shown with the number of decimals defined for the Test
    ''' ** TargetSD is shown with the number of decimals defined for the Test + 1
    ''' </summary>
    ''' <param name="pRow">Current Row index</param>
    ''' <param name="pCol">Current Column Index</param>
    ''' <remarks>
    ''' Created by:  SA 18/05/2011
    ''' Modified by: TR 20/01/2012 - If value entered is equal to Minimum Allowed Concentration it is valid (which means that 
    '''                              zero is allowed)
    '''              TR 19/04/2012 - Use the EdtitedFormattedValue instead of Value because Value still contains the previous 
    '''                              value and the other has the new value entered by User that has to be validated
    '''              SA 22/06/2012 - Use of the EditedFormattedValue recovered. Besides, when the cell has been emptied, shown 
    '''                              the Error icon indicating it is a required value
    ''' </remarks>
    Private Sub SetLimitValues(ByVal pRow As Integer, ByVal pCol As Integer)
        Try
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
            Dim numDecimals As Integer = Convert.ToInt32(bsTestListGrid.Rows(pRow).Cells("DecimalsAllowed").Value)

            If (Not bsTestListGrid.Rows(pRow).Cells(pCol).EditedFormattedValue Is Nothing) AndAlso _
               (Not bsTestListGrid.Rows(pRow).Cells(pCol).EditedFormattedValue Is DBNull.Value) AndAlso _
               (bsTestListGrid.Rows(pRow).Cells(pCol).EditedFormattedValue.ToString <> "") AndAlso _
               (bsTestListGrid.Rows(pRow).Cells(pCol).EditedFormattedValue.ToString <> myDecimalSeparator) Then
                Dim myValue As String = bsTestListGrid.Rows(pRow).Cells(pCol).EditedFormattedValue.ToString()

                If (CSng(myValue) < MinAllowedConcentration) Then
                    Dim numToAdd As Single = 1
                    If (Convert.ToInt32(bsTestListGrid.Rows(pRow).Cells("DecimalsAllowed").Value) = 0) Then
                        myValue = (MinAllowedConcentration + numToAdd).ToString
                    Else
                        numToAdd = 0
                        numToAdd = CSng(numToAdd.ToString("F" & (numDecimals - 1).ToString) & "1")

                        myValue = (MinAllowedConcentration + numToAdd).ToString
                    End If
                ElseIf (CSng(myValue) > MaxAllowedConcentration) Then
                    myValue = MaxAllowedConcentration.ToString
                End If

                'Format the value
                If (bsTestListGrid.Columns(pCol).Name = "TargetSD") Then
                    numDecimals += 1
                End If
                bsTestListGrid.Rows(pRow).Cells(pCol).Value = CSng(myValue).ToString("F" & numDecimals.ToString)
            Else
                'If the cell was emptied, shown the Error icon indicating it is a required value
                bsTestListGrid.Rows(pRow).Cells(pCol).Value = DBNull.Value
                bsTestListGrid.Rows(pRow).Cells(pCol).Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                bsTestListGrid.Rows(pRow).Cells(pCol).ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SetLimitValues", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SetLimitValues", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Update the Style of all cells in the grid of Tests/SampleTypes depending if it is enabled or not
    ''' </summary>
    Private Sub UnselectTestsSampleTypes()
        Try
            If (bsTestListGrid.Rows.Count > 0) Then
                For r As Integer = 0 To bsTestListGrid.Rows.Count - 1 Step 1
                    bsTestListGrid.Rows(r).Selected = False

                    If (EditionMode) Then
                        'Change to White the BackColor of the editable cells; the rest remain as MenuBar
                        bsTestListGrid.Rows(r).Cells("TestTypeIcon").Style.BackColor = SystemColors.MenuBar
                        bsTestListGrid.Rows(r).Cells("TestName").Style.BackColor = SystemColors.MenuBar
                        bsTestListGrid.Rows(r).Cells("SampleType").Style.BackColor = SystemColors.MenuBar
                        bsTestListGrid.Rows(r).Cells("MeasureUnit").Style.BackColor = SystemColors.MenuBar
                        bsTestListGrid.Rows(r).Cells("RejectionCriteria").Style.BackColor = SystemColors.MenuBar
                        bsTestListGrid.Rows(r).Cells("TargetMean").Style.BackColor = SystemColors.MenuBar
                        bsTestListGrid.Rows(r).Cells("TargetSD").Style.BackColor = SystemColors.MenuBar

                        bsTestListGrid.Rows(r).Cells("MinConcentration").Style.BackColor = Color.White
                        bsTestListGrid.Rows(r).Cells("MaxConcentration").Style.BackColor = Color.White

                        bsTestListGrid.Rows(r).DefaultCellStyle.ForeColor = Color.Black
                    Else
                        bsTestListGrid.Rows(r).DefaultCellStyle.BackColor = SystemColors.MenuBar

                        bsTestListGrid.Rows(r).Cells("MinConcentration").Style.BackColor = SystemColors.MenuBar
                        bsTestListGrid.Rows(r).Cells("MaxConcentration").Style.BackColor = SystemColors.MenuBar

                        bsTestListGrid.Rows(r).DefaultCellStyle.ForeColor = Color.DarkGray
                    End If
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UnselectTestsSampleTypes", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UnselectTestsSampleTypes", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate if there are QC Results pending to cumulate for the group of Controls selected to be deleted,
    ''' and in this case, shown the warning of affected elements. This function is used also to validate if 
    ''' there are affected elements before confirm a Lot change
    ''' </summary>
    ''' <param name="pControlsDS">Typed DataSet ControlsDS containing the list of Controls selected
    '''                           to be deleted</param>
    ''' <returns>If the warning screen is shown, it returns the User's answer: continue deletion or stop the process.
    '''          If there are not other affected elements, it returns the answer to continue process</returns>
    ''' <remarks>
    ''' Created by:  SA 13/05/2011
    ''' </remarks>
    Private Function ValidateDependenciesOnDeletedElements(ByVal pControlsDS As ControlsDS) As DialogResult
        Dim myResult As New DialogResult
        myResult = Windows.Forms.DialogResult.Cancel

        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myControlsDelegate As New ControlsDelegate

            myGlobalDataTO = myControlsDelegate.ValidateDependenciesOnDelete(pControlsDS)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myDependeciesElementsDS As DependenciesElementsDS = DirectCast(myGlobalDataTO.SetDatos, DependenciesElementsDS)

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
            Else
                'Error getting the list of affected elements, show it
                ShowMessage(Me.Name & ".ValidateDependenciesOnDeletedElements", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateDependenciesOnDeletedElements", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateDependenciesOnDeletedElements", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Validate if there are affected elements before confirm a requested Lot change
    ''' </summary>
    ''' <returns>If the warning screen is shown, it returns the User's answer: continue with the Lot Changing or stop the process.
    '''          If there are not other affected elements, it returns the answer to continue process</returns>
    ''' <remarks>
    ''' Created by:  SA 24/05/2011
    ''' </remarks>
    Private Function ValidateDependenciesOnLotChange() As DialogResult
        Dim myResult As New DialogResult
        myResult = Windows.Forms.DialogResult.Cancel

        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myControlsDelegate As New ControlsDelegate

            myGlobalDataTO = myControlsDelegate.ValidatedDependenciesOnUpdate(selectedControlID)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myDependeciesElementsDS As DependenciesElementsDS = DirectCast(myGlobalDataTO.SetDatos, DependenciesElementsDS)

                If (myDependeciesElementsDS.DependenciesElements.Count > 0) Then
                    Using AffectedElement As New IWarningAfectedElements
                        AffectedElement.AffectedElements = myDependeciesElementsDS
                        AffectedElement.ShowDialog()

                        myResult = AffectedElement.DialogResult
                    End Using
                Else
                    'If there are not affected elements, then return OK to change the Lot
                    myResult = Windows.Forms.DialogResult.OK
                End If
            Else
                'Error getting the list of affected elements, show it
                ShowMessage(Me.Name & ".ValidateDependenciesOnLotChange", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateDependenciesOnLotChange", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateDependenciesOnLotChange", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' When an existing Control is updated, if at least one of its linked Test/SampleType has been deleted, validate if there are 
    ''' QC Results pending to cumulate for them and in this case, shown the warning of affected elements
    ''' </summary>
    ''' <returns>If the warning screen is shown, it returns the User's answer: continue deletion or stop the process.
    '''          If there are not other affected elements, it returns the answer to continue process</returns>
    ''' <remarks>
    ''' Created by:  SA 13/05/2011
    ''' </remarks>
    Private Function ValidateDependenciesOnUpdate() As DialogResult
        Dim myResult As DialogResult = Windows.Forms.DialogResult.Cancel
        Try
            If (selectedControlID <= 0) Then
                'When adding a new Control there is nothing to validate
                myResult = Windows.Forms.DialogResult.OK
            Else
                If (DeletedTestsListDS.SelectedTestTable.Rows.Count = 0) Then
                    'When updating an existing Control, if none of the linked Tests/SampleTypes where deleted, there is nothing to validate 
                    myResult = Windows.Forms.DialogResult.OK
                ElseIf (LotChanged) Then
                    'When the Lot used for the Control has been changed, there is nothing to validate (validation was done when the auxiliary
                    'screen for Change the Lot was closed)
                    myResult = Windows.Forms.DialogResult.OK
                Else
                    Dim resultData As New GlobalDataTO
                    Dim myControlsDelegate As New ControlsDelegate

                    resultData = myControlsDelegate.ValidatedDependenciesOnUpdate(selectedControlID, DeletedTestsListDS)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myDependeciesElementsDS As DependenciesElementsDS = DirectCast(resultData.SetDatos, DependenciesElementsDS)

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
                    Else
                        'Error getting the list of affected elements, show it
                        ShowMessage(Me.Name & ".ValidateDependenciesOnUpdate", resultData.ErrorCode, resultData.ErrorMessage, Me)
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateDependenciesOnUpdate", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateDependenciesOnUpdate", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Validate if there are errors in values informed in the grid of Tests/SampleTypes
    ''' </summary>
    ''' <returns>True if there are errors; otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 12/05/2011 - Validation of required value for MaxConcentration was missing
    '''              SA 18/05/2011 - Implementation changed
    ''' </remarks>
    Private Function ValidateErrorOnTestsValues() As Boolean
        Dim errorFound As Boolean = False
        Try
            For i As Integer = 0 To bsTestListGrid.Rows.Count - 1
                errorFound = ValidateTestGridRow(i)
                If errorFound Then Exit For
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateErrorOnTestsValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateErrorOnTestsValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return errorFound
    End Function

    ''' <summary>
    ''' Validate that all mandatory fields have been informed with a correct value
    ''' </summary>
    ''' <returns>True if all fields have a correct value; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 17/05/2011 - Changed the way of getting the multilanguage text for duplicated Control
    '''                              message: the message should exist in tfmwMessage, the resource should exist
    '''                              in tfmwMultiLanguageResources, the enumerate for the message should exist in
    '''                              GlobalEnumerates.Messages, and then, the GetMessageText function should be called
    '''              XB 01/09/2014 - add Level field - BA #1868
    ''' </remarks>
    Private Function ValidateSavingConditions() As Boolean
        Dim fieldsOK As Boolean = True
        Try
            Dim setFocusTo As Integer = -1

            bsScreenErrorProvider.Clear()
            If (bsControlNameTextbox.TextLength = 0) Then
                bsScreenErrorProvider.SetError(bsControlNameTextbox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                setFocusTo = 0
            End If

            If (bsSampleTypeComboBox.SelectedValue Is Nothing) Then
                bsScreenErrorProvider.SetError(bsSampleTypeComboBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                If (setFocusTo = -1) Then setFocusTo = 1
            End If

            If (bsLotNumberTextBox.TextLength = 0) Then
                bsScreenErrorProvider.SetError(bsLotNumberTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                If (setFocusTo = -1) Then setFocusTo = 2
            End If

            If (bsExpDatePickUpCombo.Value < Now) Then
                bsScreenErrorProvider.SetError(bsExpDatePickUpCombo, GetMessageText(GlobalEnumerates.Messages.INVALIDDATE.ToString))
                If (setFocusTo = -1) Then setFocusTo = 3
            End If

            If (ValidateErrorOnTestsValues()) Then
                If (setFocusTo = -1) Then setFocusTo = 4
            End If

            ' XB 01/09/2014 - BA #1868
            If bsLevelUpDown.Text = "" Then
                bsScreenErrorProvider.SetError(bsLevelUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                If (setFocusTo = -1) Then setFocusTo = 5
            End If



            'Select the proper field to put the focus
            If (setFocusTo >= 0) Then
                fieldsOK = False

                If (setFocusTo = 0) Then
                    bsControlNameTextbox.Focus()
                ElseIf (setFocusTo = 1) Then
                    bsSampleTypeComboBox.Focus()
                ElseIf (setFocusTo = 2) Then
                    bsLotNumberTextBox.Focus()
                ElseIf (setFocusTo = 3) Then
                    bsExpDatePickUpCombo.Focus()
                ElseIf (setFocusTo = 4) Then
                    bsTestListGrid.Focus()
                ElseIf (setFocusTo = 5) Then
                    bsLevelUpDown.Focus()   ' XB 01/09/2014 - BA #1868
                End If
            Else
                'All mandatory fields are informed, verify the informed Name is unique
                Dim resultData As New GlobalDataTO
                Dim myControlDelegate As New ControlsDelegate

                resultData = myControlDelegate.ExistsControl(Nothing, bsControlNameTextbox.Text, selectedControlID)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    Dim duplicatedTest As Boolean = DirectCast(resultData.SetDatos, Boolean)
                    If (duplicatedTest) Then
                        bsScreenErrorProvider.SetError(bsControlNameTextbox, GetMessageText(GlobalEnumerates.Messages.DUPLICATED_CONTROL_NAME.ToString))

                        fieldsOK = False
                        bsControlNameTextbox.Focus()
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateSavingConditions", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateSavingConditions", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return fieldsOK
    End Function

    ''' <summary>
    ''' For the informed grid row, verify all mandatory values have been informed with an allowed 
    ''' value. In case of wrong value, show the Icon Error with the correspondent message in the 
    ''' right side of the cell
    ''' </summary>
    ''' <param name="pCurrentRow">Current row index</param>
    ''' <returns>Boolean value: True if at least one of the cells in the row is wrong; 
    '''          otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  SA 18/05/2011
    ''' </remarks>
    Private Function ValidateTestGridRow(ByVal pCurrentRow As Integer) As Boolean
        Dim rowWithErrors As Boolean = False
        Try
            Dim errorFound As Boolean = False
            'Clear previous errors...
            bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").ErrorText = String.Empty
            bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight

            bsTestListGrid.Rows(pCurrentRow).Cells("MaxConcentration").ErrorText = String.Empty
            bsTestListGrid.Rows(pCurrentRow).Cells("MaxConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Min/Max values have to be informed and Min < Max
            If (bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").Value Is Nothing) OrElse _
               (bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").Value Is DBNull.Value) OrElse _
               (bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").Value.ToString = String.Empty) Then
                errorFound = True
                rowWithErrors = True
                bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)
            End If
            If (bsTestListGrid.Rows(pCurrentRow).Cells("MaxConcentration").Value Is Nothing) OrElse _
               (bsTestListGrid.Rows(pCurrentRow).Cells("MaxConcentration").Value Is DBNull.Value) OrElse _
               (bsTestListGrid.Rows(pCurrentRow).Cells("MaxConcentration").Value.ToString = String.Empty) Then
                errorFound = True
                rowWithErrors = True
                bsTestListGrid.Rows(pCurrentRow).Cells("MaxConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                bsTestListGrid.Rows(pCurrentRow).Cells("MaxConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)
            End If

            If (Not errorFound) Then
                If (CSng(bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").Value.ToString) >= _
                    CSng(bsTestListGrid.Rows(pCurrentRow).Cells("MaxConcentration").Value.ToString)) Then
                    errorFound = True
                    rowWithErrors = True
                    bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").ErrorText = GetMessageText(GlobalEnumerates.Messages.MIN_MUST_BE_LOWER_THAN_MAX.ToString)
                End If
            End If

            If (Not rowWithErrors) Then
                'If there are not errors in the grid row, clean all Error symbols
                bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").ErrorText = String.Empty
                bsTestListGrid.Rows(pCurrentRow).Cells("MinConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight

                bsTestListGrid.Rows(pCurrentRow).Cells("MaxConcentration").ErrorText = String.Empty
                bsTestListGrid.Rows(pCurrentRow).Cells("MaxConcentration").Style.Alignment = DataGridViewContentAlignment.MiddleRight
            Else
                bsTestListGrid.Rows(pCurrentRow).Cells("TargetMean").Value = DBNull.Value
                bsTestListGrid.Rows(pCurrentRow).Cells("TargetSD").Value = DBNull.Value
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ValidateTestGridRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ValidateTestGridRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return rowWithErrors
    End Function

    ''' <summary>
    ''' Not allow moving the form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)
                If (SourceScreenAttribute = GlbSourceScreen.TEST_QCTAB.ToString) Then
                    Dim myLocation As Point = IAx00MainMDI.Location
                    Dim mySize As Size = IAx00MainMDI.Size

                    pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2) 'Me.Left
                    pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) ' Me.Top
                    Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
                End If
            End If
            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".WndProc", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
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
    ''' Created by:  DL 13/04/2011
    ''' Modified by: RH 04/07/2011 - Escape key should do exactly the same operations as bsCancelButton_Click() or bsExitButton_Click()
    ''' </remarks>
    Private Sub ProgControls_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (bsCancelButton.Enabled) Then
                    'Escape key should do exactly the same operations as bsCancelButton_Click()
                    bsCancelButton.PerformClick()
                Else
                    'Escape key should do exactly the same operations as bsExitButton_Click()
                    bsExitButton.PerformClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ProgControls_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgControls_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Form initialization when loading: set the screen status to INITIAL MODE
    ''' </summary>
    ''' <remarks>
    ''' Created  by: DL 30/03/2011 
    ''' </remarks>
    Private Sub ProgControls_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'TR 20/04/2012 get the current user level
            Dim MyGlobalBase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo().UserLevel
            'TR 20/04/2012 -END.

            ScreenLoad()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ProgControls_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgControls_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Change the BackColor of rows loaded in the grid of Tests/SampleTypes
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/05/2011
    ''' </remarks>
    Private Sub ProgControls_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            UnselectTestsSampleTypes()

            ScreenStatusByUserLevel() 'TR 23/04/2012

            'RH 29/06/2012 Bugtracking 660
            If (SourceScreenAttribute = GlbSourceScreen.TEST_QCTAB.ToString) Then
                bsNewButton.PerformClick()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ProgControls_Shown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ProgControls_Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '***********************************'
    '* EVENTS FOR LISTVIEW OF CONTROLS *'
    '***********************************'
    ''' <summary>
    ''' Show data of the selected Control in READ-ONLY MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 30/03/2011
    ''' </remarks>
    Private Sub bsControlsListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsControlsListView.Click
        Try
            If (bsControlsListView.SelectedItems.Count = 0) Then
                'If there is not Control selected, set screen status to initial
                InitialModeScreenStatus(False)

                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = False
                bsDelTest.Enabled = False

            ElseIf (bsControlsListView.SelectedItems.Count = 1) Then
                'If there is only a selected Control, show its data in details area
                QueryControl()
            Else
                'If there are several Controls selected, verify if at least one of the selected Controls is InUse
                Dim bEnabled As Boolean = True
                For Each mySelectedItem As ListViewItem In bsControlsListView.SelectedItems
                    'If there is an item InUse
                    If (CBool(mySelectedItem.SubItems(7).Text)) Then
                        bEnabled = False
                        Exit For
                    End If
                Next mySelectedItem

                'Set screen status to the case when there are several selected Controls
                InitialModeScreenStatus(False)

                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = bEnabled
                bsDelTest.Enabled = False
            End If

            ScreenStatusByUserLevel() 'TR 23/04/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsControlsListView_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsControlsListView_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Sort ascending/descending the list of Controls when clicking in the list header
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/03/2011
    ''' </remarks>
    Private Sub bsControlsListView_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles bsControlsListView.ColumnClick
        Try
            Select Case bsControlsListView.Sorting
                Case SortOrder.None
                    bsControlsListView.Sorting = SortOrder.Ascending
                    Exit Select
                Case SortOrder.Ascending
                    bsControlsListView.Sorting = SortOrder.Descending
                    Exit Select
                Case SortOrder.Descending
                    bsControlsListView.Sorting = SortOrder.Ascending
                    Exit Select
                Case Else
                    Exit Select
            End Select
            bsControlsListView.Sort()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsControlsListView_ColumnClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsControlsListView_ColumnClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Don't allow Users to resize the visible ListView column
    ''' </summary>
    Private Sub bsControlsListView_ColumnWidthChanging(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles bsControlsListView.ColumnWidthChanging
        Try
            e.Cancel = True
            If (e.ColumnIndex = 0) Then
                e.NewWidth = 210
            Else
                e.NewWidth = 0
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsControlsListView_ColumnWidthChanging", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsControlsListView_ColumnWidthChanging", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load data of the selected Control and set the screen status to Edit Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/03/2011
    ''' Modified by RH 14/06/2012
    ''' </remarks>
    Private Sub bsControlsListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsControlsListView.DoubleClick
        Try
            If (SourceScreenAttribute <> GlbSourceScreen.TEST_QCTAB.ToString()) Then
                If (bsEditButton.Enabled) Then
                    EditSelectedControl()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsControlsListView_DoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsControlsListView_DoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Allow consultation of elements in the Controls list using the keyboard, and also deletion using SUPR key
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 30/03/2011
    ''' </remarks>
    Private Sub bsControlsListView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsControlsListView.KeyUp
        Try
            If (bsControlsListView.SelectedItems.Count = 1) Then
                QueryControlByMoveUpDown(e)
            Else
                Dim bEnabled As Boolean = True
                For Each mySelectedItem As ListViewItem In bsControlsListView.SelectedItems
                    'If there is an item InUse
                    If (CBool(mySelectedItem.SubItems(7).Text)) Then
                        bEnabled = False
                        Exit For
                    End If
                Next

                'Set screen status to the case when there are several selected Controls
                InitialModeScreenStatus(False)

                bsEditButton.Enabled = False
                bsDeleteButton.Enabled = bEnabled
                bsDelTest.Enabled = False
            End If
            ScreenStatusByUserLevel() 'TR 23/04/2012
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsControlsListView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsControlsListView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Allow deletion of Controls using the SUPR key; allow set the edition Mode for the selected Control
    ''' when Enter is pressed
    ''' </summary>
    Private Sub bsControlsListView_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsControlsListView.PreviewKeyDown
        Try
            If (e.KeyCode = Keys.Delete AndAlso bsDeleteButton.Enabled) Then
                DeleteControls()

            ElseIf (e.KeyCode = Keys.Enter AndAlso bsEditButton.Enabled) Then
                bsEditButton.PerformClick()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsControlsListView_PreviewKeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsControlsListView_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '*************************************'
    '* EVENTS FOR FIELDS IN DETAILS AREA *'
    '*************************************'
    ''' <summary>
    ''' Set the proper BackColor to ComboBox of Sample Types when an item is selected 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 11/05/2011 - Removed the enabled/disabled of Search Tests button. 
    '''                              Removed relation with value of Control Name field.
    '''                              Clean the ErrorProvider when a value is selected.
    ''' </remarks>
    Private Sub bsSampleTypeComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSampleTypeComboBox.SelectedIndexChanged
        Try
            If (bsSampleTypeComboBox.SelectedValue Is Nothing) Then
                bsSampleTypeComboBox.BackColor = Color.Khaki
            Else
                bsSampleTypeComboBox.BackColor = Color.White
                If (bsScreenErrorProvider.GetError(bsSampleTypeComboBox) <> "") Then
                    bsScreenErrorProvider.SetError(bsSampleTypeComboBox, String.Empty)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSampleTypeComboBox_SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSampleTypeComboBox_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Generic event to clean the Error Provider when the value in the TextBox showing the error is changed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 11/05/2011 - Removed the enabled/disabled of Search Tests button. 
    '''                              Manages also the TextChange of LotNumber TextBox
    ''' </remarks>
    Private Sub bsTextbox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsControlNameTextbox.TextChanged, _
                                                                                                   bsLotNumberTextBox.TextChanged
        Try
            Dim myTextBox As TextBox = CType(sender, TextBox)
            If (myTextBox.TextLength > 0) Then
                If (bsScreenErrorProvider.GetError(myTextBox) <> "") Then
                    bsScreenErrorProvider.SetError(myTextBox, String.Empty)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTextbox_TextChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTextbox_TextChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '******************************************'
    '* EVENTS FOR GRID OF TESTS/SAMPLES TYPES *'
    '******************************************'
    ''' <summary>
    ''' Enable/Disable the Delete Test Button according the number of Tests selected in the grid
    ''' </summary>
    Private Sub bsTestListGrid_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsTestListGrid.CellClick
        Try
            bsDelTest.Enabled = (bsTestListGrid.SelectedRows.Count > 0)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTestListGrid_CellClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListGrid_CellClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Recalculate values of Target Mean and SD when Min and/or Max are changed
    ''' </summary>
    ''' <remarks>
    ''' Modified by: SA 18/05/2011 - Before calculate the Tartget Values, verify if the value is in the range of
    '''                              allowed ones and format data according the number of decimals defined for the Test
    ''' </remarks>
    Private Sub bsTestListGrid_CellLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsTestListGrid.CellLeave, bsTestListGrid.CellEndEdit
        Try
            If (bsTestListGrid.Columns(e.ColumnIndex).Name = "MinConcentration" OrElse bsTestListGrid.Columns(e.ColumnIndex).Name = "MaxConcentration") Then
                SetLimitValues(e.RowIndex, e.ColumnIndex)
                CalculatedTarget(e.RowIndex, e.ColumnIndex)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTestListGrid_CellLeave ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListGrid_CellLeave", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Apply a numeric format according the column 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 08/06/2011 
    ''' </remarks>
    Private Sub bsTestListGrid_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles bsTestListGrid.CellFormatting
        Try
            Dim myDecimals As Integer = Convert.ToInt32(bsTestListGrid.Rows(e.RowIndex).Cells("DecimalsAllowed").Value)
            If (bsTestListGrid.Columns(e.ColumnIndex).Name = "MinConcentration" OrElse bsTestListGrid.Columns(e.ColumnIndex).Name = "MaxConcentration" OrElse _
                bsTestListGrid.Columns(e.ColumnIndex).Name = "TargetMean") Then
                If (Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing) Then
                    e.Value = DirectCast(e.Value, Single).ToString("F" & myDecimals)
                End If
            ElseIf (bsTestListGrid.Columns(e.ColumnIndex).Name = "TargetSD") Then
                If (Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing) Then
                    'Show the decimals allowed + 1 
                    e.Value = DirectCast(e.Value, Single).ToString("F" & (myDecimals + 1))
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " bsTestListGrid_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListGrid_CellFormatting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Avoid to leave the Min/Max Concentration cells when the cell content is only the decimal separator (is not a valid number)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/05/2011 
    ''' </remarks>
    Private Sub bsTestListGrid_CellValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles bsTestListGrid.CellValidating
        Try
            If (bsTestListGrid.Columns(e.ColumnIndex).Name = "MinConcentration" OrElse bsTestListGrid.Columns(e.ColumnIndex).Name = "MaxConcentration") Then
                If (Not e.FormattedValue Is DBNull.Value AndAlso Not e.FormattedValue Is Nothing AndAlso e.FormattedValue.ToString.Trim <> "" AndAlso _
                    Not IsNumeric(e.FormattedValue)) Then
                    e.Cancel = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTestListGrid_CellValidating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListGrid_CellValidating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate that only numbers and decimals characters are entered in cells Min and Max
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 11/05/2011 - Removed verification of InUse Test/SampleType
    ''' </remarks>
    Private Sub bsTestListGrid_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles bsTestListGrid.EditingControlShowing
        Try
            Dim currentCol As Integer = bsTestListGrid.CurrentCell.ColumnIndex
            If (bsTestListGrid.CurrentRow.Index >= 0) AndAlso _
               (bsTestListGrid.Columns(currentCol).Name = "MinConcentration" OrElse bsTestListGrid.Columns(currentCol).Name = "MaxConcentration") Then
                DirectCast(e.Control, DataGridViewTextBoxEditingControl).ShortcutsEnabled = False

                AddHandler e.Control.KeyPress, AddressOf CheckNumericCell
            Else
                RemoveHandler e.Control.KeyPress, AddressOf CheckNumericCell
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTestListGrid_EditingControlShowing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListGrid_EditingControlShowing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Allow deletion of selected Tests/SampleTypes by pressing SUPR key
    ''' </summary>
    Private Sub bsTestListGrid_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsTestListGrid.PreviewKeyDown
        Try
            If (e.KeyCode = Keys.Delete And bsDelTest.Enabled) Then
                DeleteTests()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTestListGrid_PreviewKeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListGrid_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate Min/Max cells have been informed with allowed values
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/05/2011 
    ''' </remarks>
    Private Sub bsTestListGrid_RowValidating(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles bsTestListGrid.RowValidating
        Try
            ValidateTestGridRow(e.RowIndex)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsTestListGrid_RowValidating", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListGrid_RowValidating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '**********************'
    '* EVENTS FOR BUTTONS *'
    '**********************'
    ''' <summary>
    ''' Set the screen status to ADD MODE
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' </remarks>
    Private Sub bsNewButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNewButton.Click
        Try
            AddControl()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNewButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNewButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen status to EDIT MODE for the selected Controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011 
    ''' Modified by RH: 14/06/2012
    ''' </remarks>
    Private Sub bsEditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsEditButton.Click
        Try
            EditSelectedControl()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsEditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsEditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Executes deletion of all the selected Controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' </remarks>
    Private Sub bsDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDeleteButton.Click
        Try
            DeleteControls()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsDeleteButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsDeleteButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Execute the process of Lot Change for the selected Control
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 11/05/2011 - Code moved to a private Sub
    ''' </remarks>
    Private Sub bsNewLotButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNewLotButton.Click
        Try
            ChangeLot()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNewLotButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNewLotButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Opens the auxiliary screen of Test Selection
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 11/05/2011 - Code moved to a private Sub
    ''' </remarks>
    Private Sub bsSearchTestsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSearchTestsButton.Click
        Try
            SearchTests()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSearchTestsButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSearchTestsButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete from grid of Tests/SampleTypes all selected records
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 11/05/2011 - Code moved to a private Sub
    ''' </remarks>
    Private Sub bsDelTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDelTest.Click
        Try
            DeleteTests()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsDelTest_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsDelTest_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save changes (add or updation) in the database
    ''' </summary>
    ''' <remarks>
    ''' Create by: DL 30/03/2011
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
    ''' Execute the cancelling of a control adding or edition
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 31/03/2011
    ''' </remarks>    
    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            CancelControlEdition()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen, verifying first if there are changes pending to save
    ''' </summary>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            ExitScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Activate the Change made variable when some value of the corresponding numericupdown control is change by text (not only by arrows)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  XB 01/09/2014 - BA #1868
    ''' </remarks>
    Private Sub TextChange(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsLevelUpDown.TextChanged
        Try
            If EditionMode Then
                Dim myNewValue As String = DirectCast(sender, BSNumericUpDown).Text
                Dim myMaxValue As Single = DirectCast(sender, BSNumericUpDown).Maximum
                Dim myMinValue As Single = DirectCast(sender, BSNumericUpDown).Minimum

                If myNewValue.Length > 0 AndAlso myNewValue.Length > myMaxValue.ToString.Length Then
                    If CSng(myNewValue) > CSng(myMaxValue) Then
                        DirectCast(sender, Biosystems.Ax00.Controls.UserControls.BSNumericUpDown).Text = myMaxValue.ToString
                    ElseIf CSng(myNewValue) < myMinValue Then
                        If IsNumeric(Microsoft.VisualBasic.Left(myNewValue, 1)) Then
                            DirectCast(sender, Biosystems.Ax00.Controls.UserControls.BSNumericUpDown).Text = myMinValue.ToString
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".TextChange", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".TextChange", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    'Private Sub SlopeAUpDown_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles SlopeAUpDown.Validating

    '    Try
    '        BsErrorProvider1.Clear()
    '        ValidationError = False
    '        If Not SlopeAUpDown.Text = "" AndAlso SlopeAUpDown.Value = 0 Then
    '            BsErrorProvider1.SetError(SlopeAUpDown, GetMessageText(GlobalEnumerates.Messages.ZERO_NOTALLOW.ToString)) 'AG 07/07/2010("ZERO_NOTALLOW"))
    '            ValidationError = True
    '            SlopeAUpDown.Select()
    '        ElseIf Not SlopeAUpDown.Text = "" AndAlso SlopeBUpDown.Text = "" Then
    '            BsErrorProvider1.SetError(SlopeBUpDown, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString)) 'AG 07/07/2010("REQUIRED_VALUE"))
    '            ValidationError = True
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "SlopeAUpDown_Validating " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Name & ".SlopeAUpDown_Validating", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub


    ''' <summary>
    ''' generic event handler to capture the NumericUpDown content deletion
    ''' </summary>
    ''' <remarks>Created by:  XB 01/09/2014 - BA #1868</remarks>
    Private Sub NumericUpDown_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsLevelUpDown.KeyUp
        Try
            Dim miNumericUpDown As NumericUpDown = CType(sender, NumericUpDown)

            If miNumericUpDown.Text <> "" Then

            Else
                miNumericUpDown.Value = miNumericUpDown.Minimum
                miNumericUpDown.ResetText()
            End If

            If EditionMode Then
                ChangesMade = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "NumericUpDown_KeyUp " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".NumericUpDown_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Handle for NumericUpDown controls allowing numbers with decimals. Only numbers and the decimal separator are allowed
    ''' </summary>
    ''' <remarks>Created by:  XB 01/09/2014 - BA #1868</remarks>
    Private Sub IntegerNumericUpDown_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles bsLevelUpDown.KeyPress
        Try
            If (e.KeyChar = CChar("-") OrElse e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",") OrElse e.KeyChar = "'") Then e.Handled = True
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IntegerNumericUpDown_KeyPress", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IntegerNumericUpDown_KeyPress", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

End Class

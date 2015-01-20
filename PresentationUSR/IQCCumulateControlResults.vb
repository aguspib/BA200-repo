Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
'Imports System.Configuration
'Imports Biosystems.Ax00.BL.Framework

Public Class IQCCumulateControlResults

#Region "Declarations"
    Private isLoading As Boolean = False                 'To avoid show warning message of not QC Results pending to cumulate when the is screen is still loading    
    Private currentLanguage As String                    'To store the current application language  
    Private selectedQCControlLotID As Integer = -1       'To store the ID of the selected Control/Lot
    Private originalSelectedIndex As Integer = -1        'To store the index of the selected Control/Lot to verify Pending Changes
    Private QCSummaryDS As New QCSummaryByTestSampleDS   'To store the list of Tests/SampleTypes linked to the selected Control and having non cumulated QC Results
    Private QCDataToCumDS As New QCSummaryByTestSampleDS 'To store the list of Tests/SampleTypes selected to Cumulate
#End Region

#Region "Attributes"
    Private AnalyzerIDAttribute As String = String.Empty
    Private IQCResultReviewGoneAttribute As Boolean = False
#End Region

#Region "Properties"
    Public WriteOnly Property IQCResultReviewGone() As Boolean
        Set(ByVal value As Boolean)
            IQCResultReviewGoneAttribute = value
        End Set
    End Property

    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property
#End Region

#Region "Methods"
    ''' <summary>
    ''' Change the status of the details area to ReadOnly (when pEnableFields=False) or Edition Mode (when pEnableFields=True)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 07/07/2011 
    ''' Modified by: SA 15/07/2011 - Set also status for the TextBox of LotNumber and for the new grid of data to Cumulate
    ''' </remarks>
    Private Sub ChangeStatusDetailsArea(ByVal pEnableFields As Boolean)
        Try
            bsCurrentLotNumberTextBox.Enabled = pEnableFields
            bsTestListGrid.Enabled = pEnableFields
            bsTestListToCumGrid.Enabled = pEnableFields

            bsCumulateButton.Enabled = pEnableFields
            bsCancelButton.Enabled = pEnableFields

            If (bsControlsListView.Items.Count > 0) Then
                bsEditButton.Enabled = (Not pEnableFields)
                bsPrintButton.Enabled = (Not pEnableFields)
            Else
                bsEditButton.Enabled = False
                bsPrintButton.Enabled = False
            End If
            ChangeTestSamplesGridsBackColor(pEnableFields)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ChangeStatusDetailsArea", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ChangeStatusDetailsArea", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Update the Style of all cells in the grids of Tests/SampleTypes depending if it is enabled or not
    ''' </summary>
    Private Sub ChangeTestSamplesGridsBackColor(ByVal pEnabled As Boolean)
        Try
            Dim myBackColor As Color = Color.White
            Dim myForeColor As Color = Color.Black

            If (Not pEnabled) Then
                myBackColor = SystemColors.MenuBar
                myForeColor = Color.Gray
            End If

            If (bsTestListGrid.Rows.Count > 0) Then
                For r As Integer = 0 To bsTestListGrid.Rows.Count - 1 Step 1
                    bsTestListGrid.Rows(r).Selected = False
                    bsTestListGrid.Rows(r).DefaultCellStyle.BackColor = myBackColor
                    bsTestListGrid.Rows(r).DefaultCellStyle.ForeColor = myForeColor

                    'Set BackColor of all cells
                    bsTestListGrid.Rows(r).Cells("Selected").Style.BackColor = myBackColor
                    bsTestListGrid.Rows(r).Cells("TestTypeIcon").Style.BackColor = myBackColor
                    bsTestListGrid.Rows(r).Cells("TestName").Style.BackColor = myBackColor
                    bsTestListGrid.Rows(r).Cells("SampleType").Style.BackColor = myBackColor
                    bsTestListGrid.Rows(r).Cells("RejectionCriteria").Style.BackColor = myBackColor
                    bsTestListGrid.Rows(r).Cells("MeasureUnit").Style.BackColor = myBackColor
                    bsTestListGrid.Rows(r).Cells("CumulatedMean").Style.BackColor = myBackColor
                    bsTestListGrid.Rows(r).Cells("CumulatedSD").Style.BackColor = myBackColor
                    bsTestListGrid.Rows(r).Cells("CumulatedRange").Style.BackColor = myBackColor

                    'Set ForeColor of all cells
                    bsTestListGrid.Rows(r).Cells("TestName").Style.ForeColor = myForeColor
                    bsTestListGrid.Rows(r).Cells("SampleType").Style.ForeColor = myForeColor
                    bsTestListGrid.Rows(r).Cells("RejectionCriteria").Style.ForeColor = myForeColor
                    bsTestListGrid.Rows(r).Cells("MeasureUnit").Style.ForeColor = myForeColor
                    bsTestListGrid.Rows(r).Cells("CumulatedMean").Style.ForeColor = myForeColor
                    bsTestListGrid.Rows(r).Cells("CumulatedSD").Style.ForeColor = myForeColor
                    bsTestListGrid.Rows(r).Cells("CumulatedRange").Style.ForeColor = myForeColor
                Next
            End If

            If (bsTestListToCumGrid.Rows.Count > 0) Then
                For r As Integer = 0 To bsTestListToCumGrid.Rows.Count - 1 Step 1
                    bsTestListToCumGrid.Rows(r).Selected = False
                    bsTestListToCumGrid.Rows(r).DefaultCellStyle.BackColor = myBackColor
                    bsTestListToCumGrid.Rows(r).DefaultCellStyle.ForeColor = myForeColor

                    'Set BackColor of all cells
                    bsTestListToCumGrid.Rows(r).Cells("TestTypeIcon").Style.BackColor = myBackColor
                    bsTestListToCumGrid.Rows(r).Cells("TestName").Style.BackColor = myBackColor
                    bsTestListToCumGrid.Rows(r).Cells("SampleType").Style.BackColor = myBackColor
                    bsTestListToCumGrid.Rows(r).Cells("n").Style.BackColor = myBackColor
                    bsTestListToCumGrid.Rows(r).Cells("Mean").Style.BackColor = myBackColor
                    bsTestListToCumGrid.Rows(r).Cells("AlarmsIconPath").Style.BackColor = myBackColor
                    bsTestListToCumGrid.Rows(r).Cells("MeasureUnit").Style.BackColor = myBackColor
                    bsTestListToCumGrid.Rows(r).Cells("SD").Style.BackColor = myBackColor
                    bsTestListToCumGrid.Rows(r).Cells("CV").Style.BackColor = myBackColor
                    bsTestListToCumGrid.Rows(r).Cells("Ranges").Style.BackColor = myBackColor

                    'Set ForeColor of all cells
                    bsTestListToCumGrid.Rows(r).Cells("TestName").Style.ForeColor = myForeColor
                    bsTestListToCumGrid.Rows(r).Cells("SampleType").Style.ForeColor = myForeColor
                    bsTestListToCumGrid.Rows(r).Cells("n").Style.ForeColor = myForeColor
                    bsTestListToCumGrid.Rows(r).Cells("Mean").Style.ForeColor = myForeColor
                    bsTestListToCumGrid.Rows(r).Cells("MeasureUnit").Style.ForeColor = myForeColor
                    bsTestListToCumGrid.Rows(r).Cells("SD").Style.ForeColor = myForeColor
                    bsTestListToCumGrid.Rows(r).Cells("CV").Style.ForeColor = myForeColor
                    bsTestListToCumGrid.Rows(r).Cells("Ranges").Style.ForeColor = myForeColor
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ChangeTestSamplesGridsBackColor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ChangeTestSamplesGridsBackColor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' For the selected Control, execute the cumulation of QC results of the selected Tests/Sample Types
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/06/2011 
    ''' </remarks>
    Private Sub CumulateSelectedTests()
        Try
            Dim resultData As New GlobalDataTO
            Dim myQCResultsDelegate As New QCResultsDelegate

            'resultData = myQCResultsDelegate.CumulateControlResults(Nothing, QCSummaryDS, selectedQCControlLotID)
            resultData = myQCResultsDelegate.CumulateControlResultsNEW(Nothing, QCSummaryDS, selectedQCControlLotID, AnalyzerIDAttribute)
            If (Not resultData.HasError) Then
                'Reload the list of Controls and select the first element in it
                LoadControlsList()
                If (bsControlsListView.Items.Count > 0) Then
                    'Select the first Control in the list
                    bsControlsListView.Items(0).Selected = True
                    selectedQCControlLotID = Convert.ToInt32(bsControlsListView.SelectedItems(0).SubItems(1).Text)
                    originalSelectedIndex = -1

                    QueryControl()
                Else
                    selectedQCControlLotID = -1
                    originalSelectedIndex = -1

                    bsCurrentLotNumberTextBox.Text = String.Empty
                    QCSummaryDS.Clear()
                    QCDataToCumDS.Clear()

                    Me.bsEditButton.Enabled = False
                    Me.bsPrintButton.Enabled = False
                End If
                ChangeStatusDetailsArea(False)
            Else
                'Error creating the cumulated serie for the selected Control/Lot - Tests/SampleTypes
                ShowMessage(Me.Name & "CumulateSelectedTests", resultData.ErrorCode, resultData.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CumulateSelectedTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CumulateSelectedTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen
    ''' </summary>
    Private Sub ExitScreen()
        Try
            If (IQCResultReviewGoneAttribute) Then
                Me.Close()
                If Not IAx00MainMDI.ActiveMdiChild Is Nothing Then
                    If (TypeOf IAx00MainMDI.ActiveMdiChild Is IQCResultsReview) Then
                        Dim CurrentMdiChild As IQCResultsReview = CType(IAx00MainMDI.ActiveMdiChild, IQCResultsReview)
                        CurrentMdiChild.ReloadScreen()
                    End If
                End If
            Else
                If (Not Me.Tag Is Nothing) Then
                    'A PerformClick() method was executed
                    Me.Close()
                Else
                    'Normal button click
                    'Open the WS Monitor form and close this one
                    IAx00MainMDI.OpenMonitorForm(Me)
                End If
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
    ''' Created by:  SA 10/06/2011 
    ''' Modified by: SA 14/07/2011 - Get multilanguage text for new labels (Last Cumulated Values and Data to Cumulate)
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons.....
            bsControlsListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Controls_List", currentLanguage)
            bsResultsByTestSampleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_QCResultsByTestSampleType", currentLanguage)
            bsLotNumberLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", currentLanguage) + ":"
            bsLastCumValuesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_QCLastCumValues", currentLanguage) + ":"
            bsDataToCumLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_QCDataToCum", currentLanguage) + ":"

            'For Tooltips
            bsScreenToolTips.SetToolTip(bsEditButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Edit", currentLanguage))
            bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", currentLanguage))
            bsScreenToolTips.SetToolTip(bsCumulateButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CumulateQCResults", currentLanguage))
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure and initialize the ListView of Controls/Lots
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 10/06/2011
    ''' </remarks>
    Private Sub InitializeControlsList()
        Try
            'Get the Icon defined for controls that are not in use in the current Work Session
            Dim myIcons As New ImageList
            Dim notInUseIcon As String = GetIconName("CTRL")
            If (notInUseIcon <> "") Then
                myIcons.Images.Add("CTRL", ImageUtilities.ImageFromFile(MyBase.IconsPath & notInUseIcon))
            End If

            'Initialization of control List
            bsControlsListView.Items.Clear()
            bsControlsListView.Alignment = ListViewAlignment.Left
            bsControlsListView.FullRowSelect = True
            bsControlsListView.MultiSelect = False
            bsControlsListView.Scrollable = True
            bsControlsListView.View = View.Details
            bsControlsListView.HideSelection = False
            bsControlsListView.HeaderStyle = ColumnHeaderStyle.Clickable
            bsControlsListView.SmallImageList = myIcons

            'List columns definition  --> only column containing the Control Name will be visible
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            bsControlsListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Names", currentLanguage), -2, HorizontalAlignment.Left)

            bsControlsListView.Columns.Add("QCControlLotID", 0, HorizontalAlignment.Left)
            bsControlsListView.Columns.Add("ControlName", 0, HorizontalAlignment.Left)
            bsControlsListView.Columns.Add("LotNumber", 0, HorizontalAlignment.Left)

            'Fill ListView with the list of existing Controls/Lots
            LoadControlsList()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeControlsList", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeControlsList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load the ListView of Controls with the list of existing ones
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 10/06/2011
    ''' </remarks>
    Private Sub LoadControlsList()
        Try
            'Clear current content of the list view
            bsControlsListView.Items.Clear()

            'Get the list of existing Controls
            Dim resultData As New GlobalDataTO
            Dim controlList As New QCResultsDelegate

            'resultData = controlList.GetControlsToCumulate(Nothing)
            resultData = controlList.GetControlsToCumulateNEW(Nothing, AnalyzerIDAttribute)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                Dim myHistControlLotDS As HistoryControlLotsDS = DirectCast(resultData.SetDatos, HistoryControlLotsDS)

                'Sort the returned Controls
                Dim qControls As List(Of HistoryControlLotsDS.tqcHistoryControlLotsRow)
                Select Case bsControlsListView.Sorting
                    Case SortOrder.Ascending
                        qControls = (From a In myHistControlLotDS.tqcHistoryControlLots _
                                     Select a Order By a.ControlName Ascending).ToList()
                    Case SortOrder.Descending
                        qControls = (From a In myHistControlLotDS.tqcHistoryControlLots _
                                     Select a Order By a.ControlName Descending).ToList()
                    Case SortOrder.None
                        qControls = (From a In myHistControlLotDS.tqcHistoryControlLots _
                                     Select a).ToList()
                    Case Else
                        qControls = (From a In myHistControlLotDS.tqcHistoryControlLots _
                                     Select a).ToList()
                End Select

                'Fill the List View with the list of sorted controls
                Dim i As Integer = 0
                For Each controlRow As HistoryControlLotsDS.tqcHistoryControlLotsRow In qControls
                    bsControlsListView.Items.Add(controlRow.QCControlLotID.ToString, controlRow.ControlName.ToString, "CTRL")

                    bsControlsListView.Items(i).SubItems.Add(controlRow.QCControlLotID.ToString)
                    bsControlsListView.Items(i).SubItems.Add(controlRow.ControlName.ToString)
                    bsControlsListView.Items(i).SubItems.Add(controlRow.LotNumber.ToString)

                    i += 1
                Next controlRow

                'The global variables containing information of the selected control are initializated
                selectedQCControlLotID = -1
                originalSelectedIndex = -1
            Else
                'An error has happened getting data from the Database
                ShowMessage(Me.Name & ".LoadControlsList", resultData.ErrorCode, resultData.ErrorMessage)

                selectedQCControlLotID = -1
                originalSelectedIndex = -1
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadControlsList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadControlsList", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get the list of Tests/SampleTypes linked to the selected Control/Lot and having QC Results pending
    ''' to accumulate and load them in the DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 10/06/2011
    ''' Modified by: SA 15/07/2011 - Added loading of the new grid with data of the Tests/SampleTypes selected to Cumulate
    '''              SA 06/06/2012 - Sort the Tests/SampleTypes by TestType DESC (Standard ones first), PreloadedTest DESC (Biosystems 
    '''                              Tests first) and TestName 
    ''' </remarks>
    Private Sub LoadTestControlsGrid()
        Try
            'Get the list of Test/Sample Types 
            Dim resultData As New GlobalDataTO
            Dim myQCResultsDelegate As New QCResultsDelegate

            'resultData = myQCResultsDelegate.GetByQCControlLotID(Nothing, selectedQCControlLotID)
            resultData = myQCResultsDelegate.GetByQCControlLotIDNEW(Nothing, selectedQCControlLotID, AnalyzerIDAttribute)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                QCSummaryDS = DirectCast(resultData.SetDatos, QCSummaryByTestSampleDS)

                'Build the Ranges fields 
                QCDataToCumDS.Clear()
                For Each testSampleRow As QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow In QCSummaryDS.QCSummaryByTestSampleTable
                    If (Not testSampleRow.IsCumulatedMinRangeNull AndAlso Not testSampleRow.IsCumulatedMaxRangeNull) Then
                        testSampleRow.CumulatedRange = testSampleRow.CumulatedMinRange.ToString("F" & testSampleRow.DecimalsAllowed.ToString()) & " - " & _
                                                       testSampleRow.CumulatedMaxRange.ToString("F" & testSampleRow.DecimalsAllowed.ToString())
                    End If

                    If (Not testSampleRow.IsMinRangeNull AndAlso Not testSampleRow.IsMaxRangeNull) Then
                        testSampleRow.Ranges = testSampleRow.MinRange.ToString("F" & testSampleRow.DecimalsAllowed.ToString()) & " - " & _
                                               testSampleRow.MaxRange.ToString("F" & testSampleRow.DecimalsAllowed.ToString())
                    End If
                    QCDataToCumDS.QCSummaryByTestSampleTable.ImportRow(testSampleRow)
                Next
                QCSummaryDS.QCSummaryByTestSampleTable.AcceptChanges()

                'Sort the Tests/SampleTypes by TestType/TestName
                Dim viewTests As DataView = New DataView
                viewTests = QCSummaryDS.QCSummaryByTestSampleTable.DefaultView
                viewTests.Sort = " TestType DESC, PreloadedTest DESC, TestName ASC "

                'Set the sorted DS as DataSource of Tests/SampleTypes grid
                Dim bsTestsBindingSource As BindingSource = New BindingSource
                bsTestsBindingSource.DataSource = viewTests
                bsTestListGrid.DataSource = bsTestsBindingSource

                'Sort the Tests/SampleTypes by TestType/TestName
                Dim viewTestsToCum As DataView = New DataView
                viewTestsToCum = QCDataToCumDS.QCSummaryByTestSampleTable.DefaultView
                viewTestsToCum.Sort = " TestType DESC, PreloadedTest DESC, TestName ASC "

                'Set the sorted DS as DataSource of the grid containing the Tests to cumulate
                Dim bsTestsToCumBindingSource As BindingSource = New BindingSource
                bsTestsToCumBindingSource.DataSource = viewTestsToCum
                bsTestListToCumGrid.DataSource = bsTestsToCumBindingSource
            Else
                'Error getting the list of Tests/SampleTypes linked to the Control
                ShowMessage(Me.Name & ".LoadTestControlsGrid", resultData.ErrorCode, resultData.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadTestControlsGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadTestControlsGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Opens the screen of QC Results Review for the selected Test/SampleType
    ''' </summary>
    ''' <param name="pQCTestSampleID">Identifier of the selected Test/SampleType in QC Module</param>
    ''' <remarks>
    ''' Created by:  SA 13/06/2011
    ''' Modified by: SA 06/06/2012 - When screen of QCResultsReview is opened, property AnalyzerID is informed
    ''' </remarks>
    Private Sub OpenQCResultsReview(ByVal pQCTestSampleID As Integer)
        Try
            Dim myForm As New IQCResultsReview

            myForm.QCTestSampleIDValue = pQCTestSampleID
            myForm.AnalyzerID = AnalyzerIDAttribute
            myForm.MdiParent = IAx00MainMDI
            myForm.Show()

            Me.Close()
            Application.DoEvents()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".OpenQCResultsReview", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".OpenQCResultsReview", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge of loading the image for each button
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 10/06/2011
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim iconPath As String = MyBase.IconsPath
            Dim auxIconName As String = ""

            'EDIT Button
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                bsEditButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CUMULATE Button
            auxIconName = GetIconName("QCCUM")
            If (auxIconName <> "") Then
                bsCumulateButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CLOSE Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
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
    ''' Created by:  SA 10/06/2011
    ''' Modified by: SA 06/06/2012 - Added a hide column for TestType code
    ''' </remarks>
    Private Sub PrepareTestsSamplesGrid()
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsTestListGrid.Rows.Clear()
            bsTestListGrid.Columns.Clear()

            bsTestListGrid.AutoGenerateColumns = False
            bsTestListGrid.AllowUserToAddRows = False
            bsTestListGrid.AllowUserToDeleteRows = False
            bsTestListGrid.EditMode = DataGridViewEditMode.EditOnEnter

            'Selected column
            Dim selectCheckBoxCol As New DataGridViewCheckBoxColumn
            columnName = "Selected"
            selectCheckBoxCol.Name = columnName
            selectCheckBoxCol.HeaderText = ""

            bsTestListGrid.Columns.Add(selectCheckBoxCol)
            bsTestListGrid.Columns(columnName).Width = 20
            bsTestListGrid.Columns(columnName).DataPropertyName = columnName
            bsTestListGrid.Columns(columnName).Resizable = DataGridViewTriState.False
            bsTestListGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'TestType Icon (Preloaded or User Defined)
            Dim typeIconCol As New DataGridViewImageColumn
            columnName = "TestTypeIcon"
            typeIconCol.Name = columnName
            typeIconCol.DataPropertyName = "TestTypeIcon"
            typeIconCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", currentLanguage)

            bsTestListGrid.Columns.Add(typeIconCol)
            bsTestListGrid.Columns(columnName).Width = 45
            bsTestListGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Test/Sample Type Identifier in QC Module -Not visible column
            Dim qcTestSampleIDCol As New DataGridViewTextBoxColumn
            columnName = "QCTestSampleID"
            qcTestSampleIDCol.Name = columnName
            qcTestSampleIDCol.DataPropertyName = "QCTestSampleID"
            qcTestSampleIDCol.Visible = False
            bsTestListGrid.Columns.Add(qcTestSampleIDCol)

            'Runs Group Number -Not visible column
            Dim runsGroupNumCol As New DataGridViewTextBoxColumn
            columnName = "RunsGroupNumber"
            runsGroupNumCol.Name = columnName
            runsGroupNumCol.DataPropertyName = "RunsGroupNumber"
            runsGroupNumCol.Visible = False
            bsTestListGrid.Columns.Add(runsGroupNumCol)

            'Indicator of PreloadedTest -Not visible column
            Dim preloadedCol As New DataGridViewTextBoxColumn
            columnName = "PreloadedTest"
            preloadedCol.Name = columnName
            preloadedCol.DataPropertyName = "PreloadedTest"
            preloadedCol.Visible = False
            bsTestListGrid.Columns.Add(preloadedCol)

            'TestType -Not visible column
            Dim testTypeCol As New DataGridViewTextBoxColumn
            columnName = "TestType"
            testTypeCol.Name = columnName
            testTypeCol.DataPropertyName = "TestType"
            testTypeCol.Visible = False
            bsTestListGrid.Columns.Add(testTypeCol)

            'Test Name
            Dim testNameCol As New DataGridViewTextBoxColumn
            columnName = "TestName"
            testNameCol.Name = columnName
            testNameCol.DataPropertyName = "TestName"
            testNameCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", currentLanguage)
            testNameCol.ReadOnly = True

            bsTestListGrid.Columns.Add(testNameCol)
            bsTestListGrid.Columns(columnName).Width = 150
            bsTestListGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.Automatic
            bsTestListGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Sample Type Code
            Dim sampleTypeCol As New DataGridViewTextBoxColumn
            columnName = "SampleType"
            sampleTypeCol.Name = columnName
            sampleTypeCol.DataPropertyName = "SampleType"
            sampleTypeCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", currentLanguage)
            sampleTypeCol.ReadOnly = True

            bsTestListGrid.Columns.Add(sampleTypeCol)
            bsTestListGrid.Columns(columnName).Width = 60
            bsTestListGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Rejection Criteria (kSD) defined for the Test/SampleType
            Dim kSDCol As New DataGridViewTextBoxColumn
            columnName = "RejectionCriteria"
            kSDCol.Name = columnName
            kSDCol.DataPropertyName = "RejectionCriteria"
            kSDCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_kSD", currentLanguage)
            kSDCol.ReadOnly = True

            bsTestListGrid.Columns.Add(kSDCol)
            bsTestListGrid.Columns(columnName).Width = 70
            bsTestListGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Number of Decimals allowed for the Test -Not visible column
            Dim decimalsAllowed As New DataGridViewTextBoxColumn
            decimalsAllowed.Name = "DecimalsAllowed"
            decimalsAllowed.DataPropertyName = "DecimalsAllowed"
            decimalsAllowed.Visible = False
            bsTestListGrid.Columns.Add(decimalsAllowed)

            'Cumulated Mean
            Dim cumMeanCol As New DataGridViewTextBoxColumn
            columnName = "CumulatedMean"
            cumMeanCol.Name = columnName
            cumMeanCol.DataPropertyName = "CumulatedMean"
            cumMeanCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", currentLanguage)
            'cumMeanCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CumulatedMean_Short", currentLanguage)
            cumMeanCol.ReadOnly = True

            bsTestListGrid.Columns.Add(cumMeanCol)
            bsTestListGrid.Columns(columnName).Width = 95
            bsTestListGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Test Measure Unit
            Dim unitCol As New DataGridViewTextBoxColumn
            columnName = "MeasureUnit"
            unitCol.Name = columnName
            unitCol.DataPropertyName = "MeasureUnit"
            unitCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", currentLanguage)
            unitCol.ReadOnly = True

            bsTestListGrid.Columns.Add(unitCol)
            bsTestListGrid.Columns(columnName).Width = 60
            bsTestListGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Cumulated SD
            Dim cumSDCol As New DataGridViewTextBoxColumn
            columnName = "CumulatedSD"
            cumSDCol.Name = columnName
            cumSDCol.DataPropertyName = "CumulatedSD"
            cumSDCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", currentLanguage)
            'cumSDCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CumulatedSD_Short", currentLanguage)
            cumSDCol.ReadOnly = True

            bsTestListGrid.Columns.Add(cumSDCol)
            bsTestListGrid.Columns(columnName).Width = 65
            bsTestListGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Min Concentration -Not visible column
            Dim minRangeCol As New DataGridViewTextBoxColumn
            columnName = "CumulatedMinRange"
            minRangeCol.Name = columnName
            minRangeCol.DataPropertyName = "CumulatedMinRange"
            minRangeCol.Visible = False
            bsTestListGrid.Columns.Add(minRangeCol)

            'Max Concentration -Not visible column
            Dim maxRangeCol As New DataGridViewTextBoxColumn
            columnName = "CumulatedMaxRange"
            maxRangeCol.Name = columnName
            maxRangeCol.DataPropertyName = "CumulatedMaxRange"
            maxRangeCol.Visible = False
            bsTestListGrid.Columns.Add(maxRangeCol)

            'Cumulated Range
            Dim rangeCumRangeCol As New DataGridViewTextBoxColumn
            columnName = "CumulatedRange"
            rangeCumRangeCol.Name = columnName
            rangeCumRangeCol.DataPropertyName = "CumulatedRange"
            rangeCumRangeCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Ranges", currentLanguage)
            'rangeCumRangeCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Range_Cum", currentLanguage)
            rangeCumRangeCol.ReadOnly = True

            bsTestListGrid.Columns.Add(rangeCumRangeCol)
            bsTestListGrid.Columns(columnName).Width = 130
            bsTestListGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            bsTestListGrid.Columns("TestTypeIcon").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestListGrid.Columns("TestName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsTestListGrid.Columns("SampleType").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestListGrid.Columns("RejectionCriteria").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListGrid.Columns("CumulatedMean").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListGrid.Columns("MeasureUnit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestListGrid.Columns("CumulatedSD").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListGrid.Columns("CumulatedRange").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareTestsSamplesGrid " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestsSamplesGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Configure the grid for details of data of all Tests/SampleTypes selected to cumulate
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 15/07/2011
    ''' Modified by: SA 06/06/2012 - Added a hide column for TestType code 
    ''' </remarks>
    Private Sub PrepareTestToCumulateGrid()
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsTestListToCumGrid.Rows.Clear()
            bsTestListToCumGrid.Columns.Clear()

            bsTestListToCumGrid.AutoGenerateColumns = False
            bsTestListToCumGrid.AllowUserToAddRows = False
            bsTestListToCumGrid.AllowUserToDeleteRows = False
            bsTestListToCumGrid.EditMode = DataGridViewEditMode.EditOnEnter

            'TestType Icon (Preloaded or User Defined)
            Dim typeIconCol As New DataGridViewImageColumn
            columnName = "TestTypeIcon"
            typeIconCol.Name = columnName
            typeIconCol.DataPropertyName = "TestTypeIcon"
            typeIconCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", currentLanguage)

            bsTestListToCumGrid.Columns.Add(typeIconCol)
            bsTestListToCumGrid.Columns(columnName).Width = 40
            bsTestListToCumGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Test/Sample Type Identifier in QC Module -Not visible column
            Dim qcTestSampleIDCol As New DataGridViewTextBoxColumn
            columnName = "QCTestSampleID"
            qcTestSampleIDCol.Name = columnName
            qcTestSampleIDCol.DataPropertyName = "QCTestSampleID"
            qcTestSampleIDCol.Visible = False
            bsTestListToCumGrid.Columns.Add(qcTestSampleIDCol)

            'Runs Group Number -Not visible column
            Dim runsGroupNumCol As New DataGridViewTextBoxColumn
            columnName = "RunsGroupNumber"
            runsGroupNumCol.Name = columnName
            runsGroupNumCol.DataPropertyName = "RunsGroupNumber"
            runsGroupNumCol.Visible = False
            bsTestListToCumGrid.Columns.Add(runsGroupNumCol)

            'Indicator of PreloadedTest -Not visible column
            Dim preloadedCol As New DataGridViewTextBoxColumn
            columnName = "PreloadedTest"
            preloadedCol.Name = columnName
            preloadedCol.DataPropertyName = "PreloadedTest"
            preloadedCol.Visible = False
            bsTestListToCumGrid.Columns.Add(preloadedCol)

            'TestType -Not visible column
            Dim testTypeCol As New DataGridViewTextBoxColumn
            columnName = "TestType"
            testTypeCol.Name = columnName
            testTypeCol.DataPropertyName = "TestType"
            testTypeCol.Visible = False
            bsTestListGrid.Columns.Add(testTypeCol)

            'Test Name
            Dim testNameCol As New DataGridViewTextBoxColumn
            columnName = "TestName"
            testNameCol.Name = columnName
            testNameCol.DataPropertyName = "TestName"
            testNameCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", currentLanguage)
            testNameCol.ReadOnly = True

            bsTestListToCumGrid.Columns.Add(testNameCol)
            bsTestListToCumGrid.Columns(columnName).Width = 150
            bsTestListToCumGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.Automatic
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Sample Type Code
            Dim sampleTypeCol As New DataGridViewTextBoxColumn
            columnName = "SampleType"
            sampleTypeCol.Name = columnName
            sampleTypeCol.DataPropertyName = "SampleType"
            sampleTypeCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", currentLanguage)
            sampleTypeCol.ReadOnly = True

            bsTestListToCumGrid.Columns.Add(sampleTypeCol)
            bsTestListToCumGrid.Columns(columnName).Width = 60
            bsTestListToCumGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Number of Decimals allowed for the Test -Not visible column
            Dim decimalsAllowed As New DataGridViewTextBoxColumn
            decimalsAllowed.Name = "DecimalsAllowed"
            decimalsAllowed.DataPropertyName = "DecimalsAllowed"
            decimalsAllowed.Visible = False
            bsTestListToCumGrid.Columns.Add(decimalsAllowed)

            'Number of non cumulated QC Results (n)
            Dim nCol As New DataGridViewTextBoxColumn
            columnName = "n"
            nCol.Name = columnName
            nCol.DataPropertyName = "n"
            nCol.HeaderText = "n"
            nCol.ReadOnly = True

            bsTestListToCumGrid.Columns.Add(nCol)
            bsTestListToCumGrid.Columns(columnName).Width = 35
            bsTestListToCumGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Mean
            Dim meanCol As New DataGridViewTextBoxColumn
            columnName = "Mean"
            meanCol.Name = columnName
            meanCol.DataPropertyName = "CalcMean"
            meanCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", currentLanguage)
            meanCol.ReadOnly = True

            bsTestListToCumGrid.Columns.Add(meanCol)
            bsTestListToCumGrid.Columns(columnName).Width = 95
            bsTestListToCumGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Alarm Icon 
            Dim alarmIconCol As New DataGridViewImageColumn
            columnName = "AlarmsIconPath"
            alarmIconCol.Name = columnName
            alarmIconCol.DataPropertyName = "AlarmsIconPath"
            alarmIconCol.HeaderText = ""

            bsTestListToCumGrid.Columns.Add(alarmIconCol)
            bsTestListToCumGrid.Columns(columnName).Width = 20
            bsTestListToCumGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestListToCumGrid.Columns(columnName).DefaultCellStyle.NullValue = Nothing

            'Test Measure Unit
            Dim unitCol As New DataGridViewTextBoxColumn
            columnName = "MeasureUnit"
            unitCol.Name = columnName
            unitCol.DataPropertyName = "MeasureUnit"
            unitCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", currentLanguage)
            unitCol.ReadOnly = True

            bsTestListToCumGrid.Columns.Add(unitCol)
            bsTestListToCumGrid.Columns(columnName).Width = 60
            bsTestListToCumGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'SD
            Dim sdCol As New DataGridViewTextBoxColumn
            columnName = "SD"
            sdCol.Name = columnName
            sdCol.DataPropertyName = "CalcSD"
            sdCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", currentLanguage)
            sdCol.ReadOnly = True

            bsTestListToCumGrid.Columns.Add(sdCol)
            bsTestListToCumGrid.Columns(columnName).Width = 60
            bsTestListToCumGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'CV
            Dim cvCol As New DataGridViewTextBoxColumn
            columnName = "CV"
            cvCol.Name = columnName
            cvCol.DataPropertyName = "CalcCV"
            cvCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CV", currentLanguage)
            cvCol.ReadOnly = True

            bsTestListToCumGrid.Columns.Add(cvCol)
            bsTestListToCumGrid.Columns(columnName).Width = 40
            bsTestListToCumGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight

            'Min Concentration -Not visible column
            Dim minRangeCol As New DataGridViewTextBoxColumn
            columnName = "MinRange"
            minRangeCol.Name = columnName
            minRangeCol.DataPropertyName = "MinRange"
            minRangeCol.Visible = False
            bsTestListToCumGrid.Columns.Add(minRangeCol)

            'Max Concentration -Not visible column
            Dim maxRangeCol As New DataGridViewTextBoxColumn
            columnName = "MaxRange"
            maxRangeCol.Name = columnName
            maxRangeCol.DataPropertyName = "MaxRange"
            maxRangeCol.Visible = False
            bsTestListToCumGrid.Columns.Add(maxRangeCol)

            'Range
            Dim rangeCumRangeCol As New DataGridViewTextBoxColumn
            columnName = "Ranges"
            rangeCumRangeCol.Name = columnName
            rangeCumRangeCol.DataPropertyName = "Ranges"
            rangeCumRangeCol.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Ranges", currentLanguage)
            rangeCumRangeCol.ReadOnly = True

            bsTestListToCumGrid.Columns.Add(rangeCumRangeCol)
            bsTestListToCumGrid.Columns(columnName).Width = 140
            bsTestListToCumGrid.Columns(columnName).SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestListToCumGrid.Columns(columnName).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            bsTestListToCumGrid.Columns("TestTypeIcon").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestListToCumGrid.Columns("TestName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsTestListToCumGrid.Columns("SampleType").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestListToCumGrid.Columns("n").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListToCumGrid.Columns("Mean").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListToCumGrid.Columns("MeasureUnit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestListToCumGrid.Columns("SD").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListToCumGrid.Columns("CV").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsTestListToCumGrid.Columns("Ranges").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareTestToCumulateGrid " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestToCumulateGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get data of the selected Control/Lot and fill the correspondent variables and controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 10/06/2011
    ''' </remarks>
    Private Sub QueryControl()
        Try
            If (bsControlsListView.SelectedIndices(0) <> originalSelectedIndex) Then
                If (bsControlsListView.SelectedItems.Count = 0) Then bsControlsListView.Items(0).Selected = True

                selectedQCControlLotID = Convert.ToInt32(bsControlsListView.SelectedItems(0).SubItems(1).Text)
                originalSelectedIndex = bsControlsListView.SelectedIndices(0)

                'Fill screen controls with data of the selected control
                bsCurrentLotNumberTextBox.Text = bsControlsListView.SelectedItems(0).SubItems(3).Text

                'Load the list of Tests/SampleTypes linked to the selected Control
                LoadTestControlsGrid()
                bsCumulateButton.Enabled = (bsTestListGrid.RowCount > 0)
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
    ''' Created by:   SA 10/06/2011
    ''' </remarks>
    Private Sub QueryControlByMoveUpDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        Try
            Select Case e.KeyCode
                Case Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp
                    QueryControl()
                    ChangeStatusDetailsArea(False)
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".QueryControlByMoveUpDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".QueryControlByMoveUpDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Load all data needed for the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 10/06/2011 
    ''' Modified by: SA 15/07/2011 - Added preparation of the new grid of data to Cumulate
    ''' </remarks>
    Private Sub ScreenLoad()
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels
            GetScreenLabels()

            'Get Icons for graphical buttons
            PrepareButtons()

            'Configure and load the list of existing Controls
            InitializeControlsList()

            'Configure the grids for Tests/SampleTypes
            PrepareTestsSamplesGrid()
            PrepareTestToCumulateGrid()

            If (bsControlsListView.Items.Count > 0) Then
                'Select the first Control in the list
                bsControlsListView.Items(0).Selected = True
                QueryControl()

                selectedQCControlLotID = Convert.ToInt32(bsControlsListView.SelectedItems(0).SubItems(1).Text)
            End If
            bsCurrentLotNumberTextBox.BackColor = Color.Gainsboro
            ChangeStatusDetailsArea(False)

            'To avoid flickering when the screen is opened
            ResetBorder()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ScreenLoad", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ScreenLoad", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub ReleaseElements()

        Try
            '--- Detach variable defined using WithEvents ---
            bsControlListGroupBox = Nothing
            bsControlsListLabel = Nothing
            bsControlsListView = Nothing
            bsExitButton = Nothing
            bsControlGroupBox = Nothing
            bsLotNumberLabel = Nothing
            bsResultsByTestSampleLabel = Nothing
            bsTestListGrid = Nothing
            bsScreenToolTips = Nothing
            bsCurrentLotNumberTextBox = Nothing
            bsPrintButton = Nothing
            bsEditButton = Nothing
            bsCumulateButton = Nothing
            bsCancelButton = Nothing
            bsTestListToCumGrid = Nothing
            bsDataToCumLabel = Nothing
            bsLastCumValuesLabel = Nothing
            '-----------------------------------------------
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

#End Region

#Region "Events"
    '*************************'
    '* EVENTS FOR THE SCREEN *'
    '*************************'
    ''' <summary>
    ''' Close the screen when ESC Key is pressed
    ''' </summary>
    Private Sub IQCCumulateControlResults_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (bsCancelButton.Enabled) Then
                    bsCancelButton.PerformClick()
                Else
                    bsExitButton.PerformClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IQCCumulateControlResults_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IQCCumulateControlResults_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Form initialization when loading
    ''' </summary>
    Private Sub IQCCumulateControlResults_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            isLoading = True
            ScreenLoad()
            bsPrintButton.Visible = False 'JV BT #1248
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IQCCumulateControlResults_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IQCCumulateControlResults_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Change the BackColor of the dummy column used to separating the data of the last Cumulated for each
    ''' Test/SampleType loaded in the grid
    ''' </summary>
    Private Sub IQCCumulateControlResults_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Try
            ChangeTestSamplesGridsBackColor(False)
            isLoading = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IQCCumulateControlResults_Shown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IQCCumulateControlResults_Shown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '***********************************'
    '* EVENTS FOR LISTVIEW OF CONTROLS *'
    '***********************************'
    ''' <summary>
    ''' Show data of the selected Control
    ''' </summary>
    Private Sub bsControlsListView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsControlsListView.Click
        Try
            If (bsControlsListView.SelectedItems.Count = 0) Then
                'If there is not Control selected, set screen status to initial
                bsCurrentLotNumberTextBox.Text = ""
                QCSummaryDS.Clear()
            ElseIf (bsControlsListView.SelectedItems.Count = 1) Then
                'If there is only a selected Control, show its data in details area
                QueryControl()
            End If
            ChangeStatusDetailsArea(False)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsControlsListView_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsControlsListView_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Sort ascending/descending the list of Controls when clicking in the list header
    ''' </summary>
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
    Private Sub bsControlsListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsControlsListView.DoubleClick
        Try
            If (bsControlsListView.SelectedItems.Count = 0) Then
                'If there is not Control selected, set screen status to initial
                bsCurrentLotNumberTextBox.Text = ""
                QCSummaryDS.Clear()
                ChangeStatusDetailsArea(False)
            ElseIf (bsControlsListView.SelectedItems.Count = 1) Then
                'If there is only a selected Control, show its data in details area
                QueryControl()
                ChangeStatusDetailsArea(True)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsControlsListView_DoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsControlsListView_DoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Allow consultation of elements in the Controls list using the keyboard, and also deletion using SUPR key
    ''' </summary>
    Private Sub bsControlsListView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsControlsListView.KeyUp
        Try
            If (bsControlsListView.SelectedItems.Count = 1) Then
                QueryControlByMoveUpDown(e)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsControlsListView_KeyUp", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsControlsListView_KeyUp", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Allow set the edition Mode for the selected Control when Enter is pressed
    ''' </summary>
    Private Sub bsControlsListView_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles bsControlsListView.PreviewKeyDown
        Try
            If (e.KeyCode = Keys.Enter And bsEditButton.Enabled) Then
                ChangeStatusDetailsArea(True)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsControlsListView_PreviewKeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsControlsListView_PreviewKeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '******************************************'
    '* EVENTS FOR GRID OF TESTS/SAMPLES TYPES *'
    '******************************************'
    ''' <summary>
    ''' Apply a numeric format according the column 
    ''' </summary>
    Private Sub bsTestListGrid_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles bsTestListGrid.CellFormatting
        Try
            Dim myDecimals As Integer = Convert.ToInt32(bsTestListGrid.Rows(e.RowIndex).Cells("DecimalsAllowed").Value)

            If (bsTestListGrid.Columns(e.ColumnIndex).Name = "RejectionCriteria") Then
                'Show one decimal
                e.Value = DirectCast(e.Value, Single).ToString("F1")
            ElseIf (bsTestListGrid.Columns(e.ColumnIndex).Name = "CumulatedMean") Then
                'Show the decimals allowed for the Test
                If (Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing) Then
                    e.Value = DirectCast(e.Value, Double).ToString("F" & myDecimals.ToString())
                End If
            ElseIf (bsTestListGrid.Columns(e.ColumnIndex).Name = "CumulatedSD") Then
                If (Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing) Then
                    'Show the decimals allowed for the Test + 1 
                    e.Value = DirectCast(e.Value, Double).ToString("F" & (myDecimals + 1).ToString())
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " bsTestListGrid_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListGrid_CellFormatting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Opens the screen of QC Results Review for the selected Test/SampleType
    ''' </summary>
    Private Sub bsTestListGrid_CellMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsTestListGrid.CellMouseDoubleClick
        Try
            If (e.RowIndex >= 0) Then OpenQCResultsReview(Convert.ToInt32(bsTestListGrid.Rows(e.RowIndex).Cells("QCTestSampleID").Value))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " bsTestListGrid_CellMouseDoubleClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListGrid_CellMouseDoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When Selected column of a Test/Sample Type is checked/unchecked, update the state also in the DS used as DataSource for the grid 
    ''' </summary>
    Private Sub bsTestListGrid_CurrentCellDirtyStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsTestListGrid.CurrentCellDirtyStateChanged
        Try
            If (TypeOf bsTestListGrid.CurrentCell Is DataGridViewCheckBoxCell) Then
                bsTestListGrid.CommitEdit(DataGridViewDataErrorContexts.Commit)

                Dim selectedTests As List(Of QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow)
                selectedTests = (From a As QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow In QCSummaryDS.QCSummaryByTestSampleTable _
                                Where a.Selected = True Select a).ToList

                QCDataToCumDS.Clear()
                For Each row As QCSummaryByTestSampleDS.QCSummaryByTestSampleTableRow In selectedTests
                    QCDataToCumDS.QCSummaryByTestSampleTable.ImportRow(row)
                Next
                QCDataToCumDS.AcceptChanges()

                bsCumulateButton.Enabled = (selectedTests.Count > 0)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestListGrid_CurrentCellDirtyStateChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestListGrid_CurrentCellDirtyStateChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '******************************************************'
    '* EVENTS FOR GRID OF TESTS/SAMPLES TYPES TO CUMULATE *'
    '******************************************************'
    ''' <summary>
    ''' Apply a numeric format according the column in the grid of data to cumulate
    ''' </summary>
    Private Sub bsTestListToCumGrid_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles bsTestListToCumGrid.CellFormatting
        Try
            Dim myDecimals As Integer = Convert.ToInt32(bsTestListGrid.Rows(e.RowIndex).Cells("DecimalsAllowed").Value)

            If (bsTestListToCumGrid.Columns(e.ColumnIndex).Name = "Mean") Then
                'Show the decimals allowed for the Test
                If (Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing) Then
                    e.Value = DirectCast(e.Value, Double).ToString("F" & myDecimals.ToString())
                End If
            ElseIf (bsTestListToCumGrid.Columns(e.ColumnIndex).Name = "SD") Then
                If (Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing) Then
                    'Show the decimals allowed for the Test + 1 
                    e.Value = DirectCast(e.Value, Double).ToString("F" & (myDecimals + 1).ToString())
                End If
            ElseIf (bsTestListToCumGrid.Columns(e.ColumnIndex).Name = "CV") Then
                If (Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing) Then
                    'Show two decimals 
                    e.Value = DirectCast(e.Value, Double).ToString("F2")
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " bsTestListGrid_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsTestListGrid_CellFormatting", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '**********************'
    '* EVENTS FOR BUTTONS *'
    '**********************'
    ''' <summary>
    ''' Set the screen status to edition mode 
    ''' </summary>
    Private Sub bsEditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsEditButton.Click
        Try
            ChangeStatusDetailsArea(True)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsEditButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsEditButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' For the selected Control, execute the cumulation of QC results of the selected Tests/Sample Types
    ''' </summary>
    Private Sub bsCumulateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCumulateButton.Click
        Try
            CumulateSelectedTests()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCumulateButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCumulateButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the screen status to read-only mode
    ''' </summary>
    Private Sub bsCancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            ChangeStatusDetailsArea(False)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close the screen
    ''' </summary>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            ExitScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

End Class

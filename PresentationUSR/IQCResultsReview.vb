Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.PresentationCOM

Public Class IQCResultsReview

#Region "Declaration"
    Private currentLanguage As String = ""
    Private ChangeMade As Boolean = False
    Private LoadingData As Boolean = False
    Private LocalDecimalAllow As Integer
    Private LocalOpenQCResultsDS As New OpenQCResultsDS
    Private LocalQCResultsDS As New QCResultsDS
    Private FilterQCResultsList As New List(Of QCResultsDS.tqcResultsRow)
    Private PrevSelectedControlName As New List(Of String)


#End Region

#Region "Attributes"

    Private QCTestSampleIDAttribute As Integer = 0
    Private AnalyzerIDAttribute As String = String.Empty
#End Region

#Region "Properties"
    Public WriteOnly Property QCTestSampleIDValue() As Integer
        Set(ByVal value As Integer)
            QCTestSampleIDAttribute = value
        End Set
    End Property

    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Reload the screen when auxiliary screen IQCCumulateControlsResults is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 02/07/2012
    ''' Modified by: SA 13/11/2014 - BA-1885 ==> Replaced current code by a call to function LoadScreen
    ''' </remarks>
    Public Sub ReloadScreen()
        Try
            LoadScreen(False)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ReloadScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ReloadScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Open the auxiliary screen that allow adding of new manual QC Results
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 09/06/2011
    ''' Modified by: SA 05/06/2012 - Informed field AnalyzerID in the DS sent to the auxiliary screen
    '''              SA 08/06/2012 - Changes due to new functionality of the auxiliary screen for adding new QC Results
    ''' </remarks>
    Private Sub AddResultValues(ByVal pQCTestSampleID As Integer)
        Try
            'Get the maximum Run Number between all Controls/Lots linked to the TestType/TestID/SampleType
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQCResultsDelegate As New QCResultsDelegate

            myGlobalDataTO = myQCResultsDelegate.GetMaxRunNumberByTestSample(Nothing, pQCTestSampleID, AnalyzerIDAttribute)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myRunNumber As Integer = CType(myGlobalDataTO.SetDatos, Integer)

                'Get the maximum DateTime between all linked Controls/Lots having a QC Result for the maximum RunNumber
                Dim myDate As DateTime = (From a In LocalQCResultsDS.tqcResults _
                                         Where a.RunNumber = myRunNumber _
                                        Select a.ResultDateTime).Max

                'Load the list of all different Controls/Lots linked to the TestType/TestID/SampleType in a QCResultsDS
                Dim myQCResultsDS As New QCResultsDS
                Dim myQCResultsRow As QCResultsDS.tqcResultsRow
                For Each controlLotRow As OpenQCResultsDS.tOpenResultsRow In LocalOpenQCResultsDS.tOpenResults.Rows
                    myQCResultsRow = myQCResultsDS.tqcResults.NewtqcResultsRow
                    myQCResultsRow.QCTestSampleID = pQCTestSampleID
                    myQCResultsRow.QCControlLotID = controlLotRow.QCControlLotID
                    myQCResultsRow.AnalyzerID = AnalyzerIDAttribute
                    myQCResultsRow.RunsGroupNumber = controlLotRow.RunsGroupNumber
                    myQCResultsRow.ControlName = controlLotRow.ControlName
                    myQCResultsRow.LotNumber = controlLotRow.LotNumber
                    myQCResultsRow.ResultDateTime = DateAdd(DateInterval.Day, 1, myDate.Date)
                    myQCResultsRow.ResultTime = myDate
                    myQCResultsDS.tqcResults.AddtqcResultsRow(myQCResultsRow)
                Next

                'Open the auxiliary screen to add new results
                Using myManualQCResultForm As New IQCAddManualResultsAux
                    myManualQCResultForm.LanguageID = currentLanguage
                    myManualQCResultForm.MaxRunNumber = myRunNumber + 1
                    myManualQCResultForm.DecimalAllowed = LocalDecimalAllow
                    myManualQCResultForm.AllQCResultsDS = LocalQCResultsDS
                    myManualQCResultForm.NewQCResultDS = myQCResultsDS
                    myManualQCResultForm.ShowDialog()

                    'Validate if the Min Date has to be updated
                    If (Not myManualQCResultForm.MinDate = DateTime.MinValue AndAlso myManualQCResultForm.MinDate < bsDateFromDateTimePick.MinDate) Then
                        bsDateFromDateTimePick.MinDate = myManualQCResultForm.MinDate
                        bsDateFromDateTimePick.Value = myManualQCResultForm.MinDate
                    End If

                    'Validate if the Max Date has to be updated
                    If (Not myManualQCResultForm.MaxDate = DateTime.MaxValue AndAlso myManualQCResultForm.MaxDate > bsDateToDateTimePick.Value) Then
                        bsDateToDateTimePick.MaxDate = myManualQCResultForm.MaxDate
                        bsDateToDateTimePick.Value = myManualQCResultForm.MaxDate
                    End If
                End Using

                'Recalculate Results By Control/Lot after add the new Results 
                BindResultsControlLot(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text), bsDateFromDateTimePick.Value, bsDateToDateTimePick.Value)
            Else
                'Error getting the max Run Number; shown it...
                ShowMessage(Name & ".AddResultValues ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

            'Dim myRunNumber As Integer
            'Dim myQCResultsDS As New QCResultsDS
            'Dim myQCResultsRow As QCResultsDS.tqcResultsRow

            'For Each controlLotRow As OpenQCResultsDS.tOpenResultsRow In LocalOpenQCResultsDS.tOpenResults.Rows
            '    myRunNumber = 0
            '    myQCResultsRow = myQCResultsDS.tqcResults.NewtqcResultsRow

            '    myQCResultsRow.QCTestSampleID = pQCTestSampleID
            '    myQCResultsRow.QCControlLotID = controlLotRow.QCControlLotID
            '    myQCResultsRow.AnalyzerID = AnalyzerIDAttribute
            '    myQCResultsRow.RunsGroupNumber = controlLotRow.RunsGroupNumber
            '    myQCResultsRow.ControlName = controlLotRow.ControlName
            '    myQCResultsRow.LotNumber = controlLotRow.LotNumber
            '    myQCResultsRow.ResultDateTime = DateTime.Now.Date
            '    myQCResultsRow.ResultTime = DateTime.Now

            '    'Get the max RunNumber for the Test/SampleType and Control/Lot
            '    myRunNumber = GetMaxRunNumber(pQCTestSampleID, controlLotRow.QCControlLotID, controlLotRow.RunsGroupNumber)
            '    If (myRunNumber > 0) Then
            '        myQCResultsRow.RunNumber = myRunNumber + 1
            '    Else
            '        'Error getting the max Run Number... the auxiliary screen is not opened
            '        Exit Try
            '    End If
            '    myQCResultsDS.tqcResults.AddtqcResultsRow(myQCResultsRow)
            'Next

            ''Open screen to add new results
            'Using myManualQCResultForm As New IQCAddManualResultsAux
            '    myManualQCResultForm.LanguageID = currentLanguage
            '    myManualQCResultForm.DecimalAllowed = LocalDecimalAllow
            '    myManualQCResultForm.NewQCResultDS = myQCResultsDS
            '    myManualQCResultForm.ShowDialog()

            '    'Validate if the Min Date has to be updated
            '    If (Not myManualQCResultForm.MinDate = DateTime.MinValue AndAlso myManualQCResultForm.MinDate < bsDateFromDateTimePick.MinDate) Then
            '        bsDateFromDateTimePick.MinDate = myManualQCResultForm.MinDate
            '        bsDateFromDateTimePick.Value = myManualQCResultForm.MinDate
            '    End If

            '    'Validate if the Max Date has to be updated
            '    If (Not myManualQCResultForm.MaxDate = DateTime.MaxValue AndAlso myManualQCResultForm.MaxDate > bsDateToDateTimePick.Value) Then
            '        bsDateToDateTimePick.MaxDate = myManualQCResultForm.MaxDate
            '        bsDateToDateTimePick.Value = myManualQCResultForm.MaxDate
            '    End If
            'End Using

            ''Recalculate Results By Control/Lot after add the new Results 
            'BindResultsControlLot(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text), bsDateFromDateTimePick.Value, bsDateToDateTimePick.Value)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AddResultValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AddResultValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' For the selected Test/SampleType, load QC data applying the specified criteria
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/11/2014 - BA-1885 ==> Code moved from Click event of SearchButton
    ''' </remarks>
    Private Sub ApplySearchCriteria()
        Try
            Me.Enabled = False
            Cursor = Cursors.WaitCursor

            If (bsTestSampleListView.SelectedItems.Count > 0) Then
                If (Not ValidateErrorRequiredValues()) Then
                    If (ChangeMade) Then
                        SaveUpdatedValues(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text), _
                                          bsTestSampleListView.SelectedItems(0).SubItems(5).Text, _
                                          CInt(bsTestSampleListView.SelectedItems(0).SubItems(3).Text), _
                                          bsTestSampleListView.SelectedItems(0).SubItems(1).Text)
                        ChangeMade = False
                    End If

                    'Clear previous filters
                    PrevSelectedControlName.Clear()
                    BindCalculationCriteria(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text), False)
                End If
            End If

            Cursor = Cursors.Default
            Me.Enabled = True
        Catch ex As Exception
            Cursor = Cursors.Default
            Me.Enabled = True

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ApplySearchCriteria ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ApplySearchCriteria ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Bind data read from DB with fields on the Calculation Criteria area
    ''' </summary>
    ''' <param name="pQCTestSampleID">Identifier (in the history QC tables) of the selected Test/SampleType</param>
    ''' <remarks>
    ''' Created by:  TR 27/05/2011
    ''' Modified by: SA 07/10/2011 - Added parameter to indicate if From/To controls have to be initialized or not
    '''              SA 25/01/2012 - Enable the Multirules area also when there is only a linked Control/Lot with non cumulated QC Results
    '''              SA 23/04/2012 - Changed the way of selecting Controls to apply the Westgard Multirules to solve some cases that didn't work
    '''              SA 05/06/2012 - Informed parameter AnalyzerID when calling functions MinResultDateTime and MaxResultDateTime in 
    '''                              QCResultsDelegate, and GetAllControlsLinkedToTestSampleType in HistoryTestControlLotsDelegate
    ''' </remarks>
    Private Sub BindCalculationCriteria(ByVal pQCTestSampleID As Integer, Optional ByVal pSetDefaultDates As Boolean = True)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQCResultDelegate As New QCResultsDelegate
            Dim myHistoryTestSamplesDelegate As New HistoryTestSamplesDelegate
            Dim myHistoryTestControlLotsDelegate As New HistoryTestControlLotsDelegate

            'Clear the Error Provider and all controls in the area
            bsResultErrorProv.Clear()
            ClearControls(pSetDefaultDates)

            'Get the information from the History Tests/Sample Types table
            myGlobalDataTO = myHistoryTestSamplesDelegate.Read(Nothing, pQCTestSampleID)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                'Fill fields in area of Calculation Criteria
                Dim myHistoryTestSampleDS As HistoryTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)

                If (myHistoryTestSampleDS.tqcHistoryTestSamples.Count > 0) Then
                    '** Bind Calculation Mode, Number of Series and Rejection Criteria...
                    bsCalculationModeCombo.SelectedValue = myHistoryTestSampleDS.tqcHistoryTestSamples(0).CalculationMode
                    If (bsCalculationModeCombo.SelectedValue = "MANUAL") Then
                        bsNumberOfSeriesNumeric.ResetText()
                    Else
                        bsNumberOfSeriesNumeric.Text = myHistoryTestSampleDS.tqcHistoryTestSamples(0).NumberOfSeries
                    End If
                    bsRejectionNumeric.Text = myHistoryTestSampleDS.tqcHistoryTestSamples(0).RejectionCriteria

                    'Bind Multirules...
                    BindMultirulesControls(pQCTestSampleID)

                    'If there is not an informed date FROM, get date of the oldest non cumulate QC Result for the selected Test/SampleType
                    If (pSetDefaultDates) Then
                        'myGlobalDataTO = myQCResultDelegate.GetMinResultDateTime(Nothing, pQCTestSampleID)
                        myGlobalDataTO = myQCResultDelegate.GetMinResultDateTimeNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute)
                        If (Not myGlobalDataTO.HasError) Then
                            If (Not myGlobalDataTO.SetDatos Is DBNull.Value) Then
                                bsDateFromDateTimePick.MinDate = DirectCast(myGlobalDataTO.SetDatos, DateTime)
                                bsDateFromDateTimePick.Value = DirectCast(myGlobalDataTO.SetDatos, DateTime)
                            End If
                        Else
                            'Error getting the Min QC Result Date for the selected Test/SampleType; shown it
                            ShowMessage(Name & ".BindCalculationCriteria ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                        End If
                    End If

                    'If there is not an informed date TO, get date of the more recent non cumulate QC Result for the selected Test/SampleType
                    If (pSetDefaultDates) Then
                        'myGlobalDataTO = myQCResultDelegate.GetMaxResultDateTime(Nothing, pQCTestSampleID)
                        myGlobalDataTO = myQCResultDelegate.GetMaxResultDateTimeNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute)
                        If Not myGlobalDataTO.HasError Then
                            If Not myGlobalDataTO.SetDatos Is DBNull.Value Then
                                bsDateToDateTimePick.MaxDate = DirectCast(myGlobalDataTO.SetDatos, DateTime)
                                bsDateToDateTimePick.Value = DirectCast(myGlobalDataTO.SetDatos, DateTime)

                                'TR 02/07/2012 -Set the max value to from date control.
                                bsDateFromDateTimePick.MaxDate = bsDateToDateTimePick.Value
                            End If
                        Else
                            'Error getting the Max QC Result Date for the selected Test/SampleType; shown it
                            ShowMessage(Name & ".BindCalculationCriteria ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                        End If
                    End If

                    If (Not myGlobalDataTO.HasError) Then
                        'Get all active Controls linked to the selected Test/SampleType to load ComboBoxes in Westgard area
                        'myGlobalDataTO = myHistoryTestControlLotsDelegate.GetAllControlsLinkedToTestSampleType(Nothing, pQCTestSampleID)
                        myGlobalDataTO = myHistoryTestControlLotsDelegate.GetAllControlsLinkedToTestSampleTypeNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myHistoryTestControlLotsDS As HistoryTestControlLotsDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestControlLotsDS)

                            If (myHistoryTestControlLotsDS.tqcHistoryTestControlLots.Count >= 1) Then
                                'There is at least one Control linked to the selected Test/SampleType: enable and load the Multirules area
                                bsRulesGroupbox.Enabled = True

                                bsMultirulesApplication1Combo.Enabled = True
                                Dim myHistoryTestControlLotsList1 As List(Of HistoryTestControlLotsDS.tqcHistoryTestControlLotsRow) = (From a In myHistoryTestControlLotsDS.tqcHistoryTestControlLots _
                                                                                                                                     Select a).ToList()
                                bsMultirulesApplication1Combo.DataSource = myHistoryTestControlLotsList1
                                bsMultirulesApplication1Combo.DisplayMember = "ControlName"
                                bsMultirulesApplication1Combo.ValueMember = "QCControlLotID"

                                If (myHistoryTestControlLotsDS.tqcHistoryTestControlLots.Count > 1) Then
                                    bsMultirulesApplication2Combo.Enabled = True
                                    Dim myHistoryTestControlLotsList2 As List(Of HistoryTestControlLotsDS.tqcHistoryTestControlLotsRow) = (From a In myHistoryTestControlLotsDS.tqcHistoryTestControlLots _
                                                                                                                                         Select a).ToList()
                                    bsMultirulesApplication2Combo.DataSource = myHistoryTestControlLotsList2
                                    bsMultirulesApplication2Combo.DisplayMember = "ControlName"
                                    bsMultirulesApplication2Combo.ValueMember = "QCControlLotID"

                                    myHistoryTestControlLotsList2 = Nothing
                                End If

                                'Select all linked Controls not selected to apply Westgard
                                Dim firstAlreadySelected As Boolean = False
                                Dim myNotWestgardControls As List(Of HistoryTestControlLotsDS.tqcHistoryTestControlLotsRow) = (From a In myHistoryTestControlLotsDS.tqcHistoryTestControlLots _
                                                                                                                              Where a.IsWestgardControlNumNull OrElse a.WestgardControlNum = 0 _
                                                                                                                             Select a).ToList()


                                'Search if there is a Control with WestgardControlNum = 1
                                Dim myHistoryTestControlLotsList As List(Of HistoryTestControlLotsDS.tqcHistoryTestControlLotsRow) = (From a In myHistoryTestControlLotsDS.tqcHistoryTestControlLots _
                                                                                                                                     Where a.WestgardControlNum = 1 _
                                                                                                                                    Select a).ToList()
                                If (myHistoryTestControlLotsList.Count > 0) Then
                                    'If found, then selected it on the first combo 
                                    bsMultirulesApplication1Combo.SelectedValue = myHistoryTestControlLotsList.First().QCControlLotID
                                Else
                                    If (myNotWestgardControls.Count > 0) Then
                                        'Select the first between all Controls not previously selected to apply Westgard rules
                                        bsMultirulesApplication1Combo.SelectedValue = myNotWestgardControls(0).QCControlLotID
                                        firstAlreadySelected = True
                                    End If
                                End If

                                If (myHistoryTestControlLotsDS.tqcHistoryTestControlLots.Count > 1) Then
                                    'Search if there is a Control with WestgardControlNum = 2
                                    myHistoryTestControlLotsList = (From a In myHistoryTestControlLotsDS.tqcHistoryTestControlLots _
                                                                   Where a.WestgardControlNum = 2 _
                                                                  Select a).ToList()

                                    If (myHistoryTestControlLotsList.Count > 0) Then
                                        'If found, then selected it on the second combo 
                                        bsMultirulesApplication2Combo.SelectedValue = myHistoryTestControlLotsList.First().QCControlLotID
                                    Else
                                        If (myNotWestgardControls.Count > 0) Then
                                            If (Not firstAlreadySelected) Then
                                                'If the first Control in the list of not selected for Westgard was not selected in the first ComboBox,
                                                'then it is selected in the second one
                                                bsMultirulesApplication2Combo.SelectedValue = myNotWestgardControls(0).QCControlLotID

                                            ElseIf (myNotWestgardControls.Count > 1) Then
                                                'Otherwise, if there are more Controls in the list of not selected for Westgard, then the second in the
                                                'list is selected in the second ComboBox
                                                bsMultirulesApplication2Combo.SelectedValue = myNotWestgardControls(1).QCControlLotID
                                            End If
                                        End If
                                    End If
                                Else
                                    bsMultirulesApplication2Combo.DataSource = Nothing
                                    bsMultirulesApplication2Combo.Enabled = False
                                End If

                                'Bind data to the grid of Results by Control/Lot
                                BindResultsControlLot(pQCTestSampleID, bsDateFromDateTimePick.Value, bsDateToDateTimePick.Value)

                                myHistoryTestControlLotsList1 = Nothing
                                myHistoryTestControlLotsList = Nothing

                            ElseIf (myHistoryTestControlLotsDS.tqcHistoryTestControlLots.Count = 0) Then
                                'Clear both grids of results (by Control/Lot and Detailed)
                                bsResultsDetailsGridView.DataSource = Nothing
                                bsResultControlLotGridView.DataSource = Nothing

                                'Show message no QC Results Pending to Cumulate
                                ShowMessage(Name & ".BindCalculationCriteria ", GlobalEnumerates.Messages.NOCUMULATE_QCRESULT.ToString())
                            End If
                        Else
                            'Error getting the list of active Controls linked to the selected Test/SampleType; shown it
                            ShowMessage(Name & ".BindCalculationCriteria ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                        End If
                    End If
                Else
                    'Show message no QC Results Pending to Cumulate
                    ShowMessage(Name & ".BindCalculationCriteria ", GlobalEnumerates.Messages.NOCUMULATE_QCRESULT.ToString(), _
                                GetMessageText(GlobalEnumerates.Messages.NOCUMULATE_QCRESULT.ToString(), currentLanguage))
                End If
            Else
                'Error getting Calculation Data of the selected Test/SampleType; shown it
                ShowMessage(Name & ".BindCalculationCriteria ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

            'After binding all controls, set ChangeMade to false
            ChangeMade = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BindCalculationCriteria ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BindCalculationCriteria ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Bind Multirules data read from DB with fields on the correspondent subarea of Calculation Criteria area
    ''' </summary>
    ''' <param name="pQCTestSampleID">Identifier (in the history QC tables) of the selected Test/SampleType</param>
    ''' <remarks>
    ''' Created by:  TR 27/05/2011
    ''' </remarks>
    Private Sub BindMultirulesControls(ByVal pQCTestSampleID As Integer)
        Try
            'Clear rules values before binding
            bs12SCheckBox.Checked = False
            bs13SCheckBox.Checked = False
            bs22SCheckBox.Checked = False
            bsR4SCheckBox.Checked = False
            bs41SCheckBox.Checked = False
            bs10XmCheckBox.Checked = False

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myHistoryTestSampRulesDelegate As New HistoryTestSamplesRulesDelegate

            'Get the Westgard rules selected for the Test/SampleType
            myGlobalDataTO = myHistoryTestSampRulesDelegate.ReadByQCTestSampleID(Nothing, pQCTestSampleID)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myHistoryTestSamRulesDS As HistoryTestSamplesRulesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesRulesDS)

                Dim qTestSampleMultiList As List(Of HistoryTestSamplesRulesDS.tqcHistoryTestSamplesRulesRow)
                qTestSampleMultiList = (From a In myHistoryTestSamRulesDS.tqcHistoryTestSamplesRules _
                                      Select a).ToList()

                If (qTestSampleMultiList.Count > 0) Then
                    For Each ruleRow As HistoryTestSamplesRulesDS.tqcHistoryTestSamplesRulesRow In qTestSampleMultiList
                        Select Case ruleRow.RuleID
                            Case "WESTGARD_1-2s"
                                bs12SCheckBox.Checked = True
                                Exit Select
                            Case "WESTGARD_1-3s"
                                bs13SCheckBox.Checked = True
                                Exit Select
                            Case "WESTGARD_2-2s"
                                bs22SCheckBox.Checked = True
                                Exit Select
                            Case "WESTGARD_R-4s"
                                bsR4SCheckBox.Checked = True
                                Exit Select
                            Case "WESTGARD_4-1s"
                                bs41SCheckBox.Checked = True
                                Exit Select
                            Case "WESTGARD_10X"
                                bs10XmCheckBox.Checked = True
                                Exit Select
                        End Select
                    Next
                End If
                qTestSampleMultiList = Nothing
            Else
                'Error getting the list of Multirules linked to the Test/SampleType; shown it
                ShowMessage(Name & ".BindMultirulesControls ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BindMultirulesControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BindMultirulesControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Bind QC Results read from DB to the DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 31/05/2011
    ''' Modified by: SA 30/11/2011 - Changed the way of getting the information that have to be shown in the DataGridView 
    '''                              of Results by Control/Lot:
    '''                                ** When CalculationMode=MANUAL, call function GetResultsByControlLotForManualMode
    '''                                ** When CalculationMode=STATISTICS, call function GetResultsByControlLotForStatisticsMode
    '''              SA 25/01/2012 - When verifying if there are Control/Lots selected for application of Multirules, manage the case when
    '''                              only one Control/Lot has been selected
    '''              SA 05/06/2012 - ID of the connected Analyzer informed when calling functions UnmarkStatisticResults, SetResultsForStatistics,
    '''                              GetResultsByControlLotForManualMode and GetResultsByControlLotForStatisticsMode in QCResultsDelegate
    ''' </remarks>
    Private Sub BindResultsControlLot(ByVal pQCTestSampleID As Integer, ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime)
        Try
            EnableDisableControls(True)

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQCResultsDelegate As New QCResultsDelegate

            If (bsCalculationModeCombo.SelectedValue.ToString = "MANUAL") Then
                'Set IncludedInMean = FALSE for all open QC Results for the specified Test/SampleType (to manage the case when 
                'CalculationMode has been changed from STATISTIC to MANUAL)
                'myGlobalDataTO = myQCResultsDelegate.UnmarkStatisticResults(Nothing, pQCTestSampleID)
                myGlobalDataTO = myQCResultsDelegate.UnmarkStatisticResultsNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute)
                If (Not myGlobalDataTO.HasError) Then
                    'Get all Control/Lots linked to the specified Test/SampleType
                    'myGlobalDataTO = myQCResultsDelegate.GetResultsByControlLotForManualMode(Nothing, pQCTestSampleID, pDateFrom, pDateTo)
                    myGlobalDataTO = myQCResultsDelegate.GetResultsByControlLotForManualModeNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute, pDateFrom, pDateTo)
                End If

            ElseIf (bsCalculationModeCombo.SelectedValue.ToString = "STATISTIC") Then
                'Set IncludedInMean = TRUE for the first NumberOfSeries QC Results for the specified Test/SampleType, and IncludedInMean = FALSE
                'for the rest of its QC Results (to manage the case when CalculationMode has been changed from MANUAL to STATISTIC, and also the 
                'case in which the Calculation Mode remains as STATISTIC but the NumberOfSeries has been changed)
                'myGlobalDataTO = myQCResultsDelegate.SetResultsForStatistics(Nothing, pQCTestSampleID)
                myGlobalDataTO = myQCResultsDelegate.SetResultsForStatisticsNEW(Nothing, AnalyzerIDAttribute, pQCTestSampleID)

                If (Not myGlobalDataTO.HasError) Then
                    'Get all Control/Lots linked to the specified Test/SampleType
                    'myGlobalDataTO = myQCResultsDelegate.GetResultsByControlLotForStatisticsMode(Nothing, pQCTestSampleID, pDateFrom, pDateTo)
                    myGlobalDataTO = myQCResultsDelegate.GetResultsByControlLotForStatisticsModeNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute, pDateFrom, pDateTo)
                End If
            End If

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                LocalOpenQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, OpenQCResultsDS)

                For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In LocalOpenQCResultsDS.tOpenResults.Rows
                    If (Not openQCResultRow.IsMinRangeNull AndAlso Not openQCResultRow.IsMaxRangeNull) Then
                        openQCResultRow.Ranges = openQCResultRow.MinRange.ToString("F" & LocalDecimalAllow.ToString()) & "-" & _
                                                 openQCResultRow.MaxRange.ToString("F" & LocalDecimalAllow.ToString())
                    End If
                Next
                bsResultControlLotGridView.DataSource = LocalOpenQCResultsDS.tOpenResults

                'Before getting the open QC Results, make sure there is at least a Control/Lot selected
                If (LocalOpenQCResultsDS.tOpenResults.Count > 0) Then
                    If (bsMultirulesApplication1Combo.SelectedIndex > -1) Then
                        'If there are Controls selected for application of the selected Multirules, mark them as selected in the Control/Lot Results DataGridView
                        If (LocalOpenQCResultsDS.tOpenResults.ToList.Where(Function(a) a.QCControlLotID = CInt(bsMultirulesApplication1Combo.SelectedValue)).Count > 0) Then
                            LocalOpenQCResultsDS.tOpenResults.ToList.Where(Function(a) a.QCControlLotID = CInt(bsMultirulesApplication1Combo.SelectedValue)).First().WestgardControlNum = 1
                            LocalOpenQCResultsDS.tOpenResults.ToList.Where(Function(a) a.QCControlLotID = CInt(bsMultirulesApplication1Combo.SelectedValue)).First().Selected = True

                            If (bsMultirulesApplication2Combo.Enabled AndAlso _
                                LocalOpenQCResultsDS.tOpenResults.ToList.Where(Function(a) a.QCControlLotID = CInt(bsMultirulesApplication2Combo.SelectedValue)).Count > 0) Then
                                LocalOpenQCResultsDS.tOpenResults.ToList.Where(Function(a) a.QCControlLotID = CInt(bsMultirulesApplication2Combo.SelectedValue)).First().WestgardControlNum = 2
                                LocalOpenQCResultsDS.tOpenResults.ToList.Where(Function(a) a.QCControlLotID = CInt(bsMultirulesApplication2Combo.SelectedValue)).First().Selected = True
                            End If
                        Else
                            LocalOpenQCResultsDS.tOpenResults(0).Selected = True
                        End If
                    Else
                        LocalOpenQCResultsDS.tOpenResults(0).Selected = True
                    End If

                    'Get non cumulate QC Results for all Control/Lots
                    GetNonCumulateResultForAllControls(pQCTestSampleID, LocalOpenQCResultsDS, pDateFrom, pDateTo)

                    If (LocalQCResultsDS.tqcResults.Count > 0) Then
                        'Get Results for the first Selected Control/Lot
                        Dim myQCControlLotID1 As Integer = (LocalOpenQCResultsDS.tOpenResults.ToList.Where(Function(a) Not a.IsSelectedNull _
                                                                                                           AndAlso a.Selected).First()).QCControlLotID

                        Dim myQCControlLotID2 As Integer = 0
                        If (LocalOpenQCResultsDS.tOpenResults.ToList.Where(Function(a) Not a.IsSelectedNull _
                                                                           AndAlso a.Selected _
                                                                           AndAlso a.QCControlLotID <> myQCControlLotID1).Count > 0) Then
                            'Get Results for the second selected Control/Lot
                            myQCControlLotID2 = LocalOpenQCResultsDS.tOpenResults.ToList.Where(Function(a) Not a.IsSelectedNull _
                                                                                               AndAlso a.Selected _
                                                                                               AndAlso a.QCControlLotID <> myQCControlLotID1).Last().QCControlLotID
                        End If

                        'Shown only QC Results of selected Control/Lots
                        FilterQCResultsList.Clear()
                        FilterQCResultsBySelectedControls(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text))
                    End If
                Else
                    bsResultsDetailsGridView.DataSource = Nothing

                    'Show Message no QC Result pending to cummulate.
                    ShowMessage(Name & ".BindResultsControlLot ", GlobalEnumerates.Messages.NOCUMULATE_QCRESULT.ToString())
                End If
            Else
                'Error updating field IncludedInMean for all open QC Results for the specified Test/SampleType or getting the list 
                'of non cumulated QC Results for all Control/Lots linked to the selected Test/SampleType; shown it
                ShowMessage(Name & ".BindResultsControlLot ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BindResultsControlLot ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BindResultsControlLot ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Commit the changes made whe user click on the Selected CheckBox on ResultControlLotGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 08/06/2011
    ''' Modified by: SA 23/12/2011 - If there are three Control/Lots selected, it is not allowed selecting a new one
    ''' </remarks>
    Private Sub ChangeControlLotSelectedState()
        Try
            If (TypeOf bsResultControlLotGridView.CurrentCell Is DataGridViewCheckBoxCell) Then
                If (ValidateActiveControls()) Then
                    bsResultControlLotGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)
                Else
                    bsResultControlLotGridView.CancelEdit()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeControlLotSelectedState", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeControlLotSelectedState", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Clear controls values.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 27/05/2011
    ''' Modified by: SA 07/10/2011 - Added parameter to indicate if From/To controls have to be initialized or not
    ''' </remarks>
    Private Sub ClearControls(Optional ByVal pSetDefaultDates As Boolean = True)
        Try
            If (pSetDefaultDates) Then
                bsDateFromDateTimePick.Value = bsDateFromDateTimePick.MinDate
                bsDateToDateTimePick.Value = bsDateToDateTimePick.MaxDate
            End If

            bsMultirulesApplication1Combo.SelectedIndex = -1
            bsMultirulesApplication2Combo.SelectedIndex = -1
            bsCalculationModeCombo.SelectedIndex = -1
            bsNumberOfSeriesNumeric.ResetText()
            bsRejectionNumeric.ResetText()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ClearControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ClearControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete all selected QC Results
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 
    ''' Modified by: SA 05/06/2012 - Field AnalyzerID informed in the typed DS QCResultsDS containing all QC Results to delete
    ''' </remarks>
    Private Sub DeleteSelecteQCResultValues()
        Try
            If (bsResultsDetailsGridView.SelectedRows.Count > 0) Then
                'Show delete confirmation message 
                If (ShowMessage(Name & ".DeleteSelecteQCResultValues ", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                    Dim myQCResultsDS As New QCResultsDS
                    Dim myQCResultsRow As QCResultsDS.tqcResultsRow

                    'Load all selected Results to delete in a QCResultsDS typed DataSet
                    For Each selQCResult As DataGridViewRow In bsResultsDetailsGridView.SelectedRows
                        myQCResultsRow = myQCResultsDS.tqcResults.NewtqcResultsRow()
                        myQCResultsRow.QCTestSampleID = CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text)
                        myQCResultsRow.QCControlLotID = CInt(selQCResult.Cells("QCControlLotID").Value)
                        myQCResultsRow.AnalyzerID = AnalyzerIDAttribute
                        myQCResultsRow.RunsGroupNumber = CInt(selQCResult.Cells("RunsGroupNumber").Value)
                        myQCResultsRow.RunNumber = CInt(selQCResult.Cells("RunNumber").Value)

                        myQCResultsDS.tqcResults.AddtqcResultsRow(myQCResultsRow)
                    Next

                    Dim myGlobalDataTO As New GlobalDataTO
                    Dim myQCResultsDelegate As New QCResultsDelegate

                    'myGlobalDataTO = myQCResultsDelegate.Delete(Nothing, myQCResultsDS)
                    myGlobalDataTO = myQCResultsDelegate.DeleteNEW(Nothing, myQCResultsDS)
                    If (myGlobalDataTO.HasError) Then
                        'Error deleting the selected QC Results; show it
                        ShowMessage(Name & ".DeleteSelecteQCResultValues ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    Else
                        'Recalculate Results By Control/Lot after deleting the selected Results 
                        BindResultsControlLot(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text), bsDateFromDateTimePick.Value, bsDateToDateTimePick.Value)
                        If (bsResultsDetailsGridView.RowCount > 0) Then
                            bsDeleteButtom.Enabled = True
                            bsGraphsButton.Enabled = True
                            bsAddButtom.Enabled = True
                        Else
                            bsDeleteButtom.Enabled = False
                            bsGraphsButton.Enabled = False
                            bsAddButtom.Enabled = False
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteSelecteQCResultValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteSelecteQCResultValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open the auxiliary screen that allow edition of the selected QC Result
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 08/06/211
    ''' Modified by: SA 05/06/2012 - Informed field AnalyzerID in the DS sent to the auxiliary screen
    ''' </remarks>
    Private Sub EditResultValues()
        Try
            If (bsResultsDetailsGridView.SelectedRows.Count = 1) Then
                Using myQCResultEdition As New IQCResultsEditionAux()
                    Dim myResultInformationDS As New ResultInformationDS
                    Dim myResultInformationRow As ResultInformationDS.tResultInformationRow

                    'Fill dataset with selected result data.
                    myResultInformationRow = myResultInformationDS.tResultInformation.NewtResultInformationRow
                    myResultInformationRow.TestName = bsTestSampleListView.SelectedItems(0).Text
                    myResultInformationRow.SampleTypeCode = bsTestSampleListView.SelectedItems(0).SubItems(1).Text
                    myResultInformationRow.TestMeasureUnit = bsResultsDetailsGridView.SelectedRows(0).Cells("MeasureUnit").Value.ToString()
                    myResultInformationRow.DecimalsAllowed = LocalDecimalAllow
                    myResultInformationRow.ControlName = bsResultsDetailsGridView.SelectedRows(0).Cells("ControlName").Value.ToString()
                    myResultInformationRow.LotNumber = bsResultsDetailsGridView.SelectedRows(0).Cells("LotNumber").Value.ToString()
                    myResultInformationRow.QCTestSampleID = CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text)
                    myResultInformationRow.QCControlID = CInt(bsResultsDetailsGridView.SelectedRows(0).Cells("QCControlLotID").Value)
                    myResultInformationRow.AnalyzerID = AnalyzerIDAttribute
                    myResultInformationRow.RunsGroupNumber = CInt(bsResultControlLotGridView.SelectedRows(0).Cells("RunsGroupNumber").Value)
                    myResultInformationRow.NumberOfSeries = CInt(bsResultsDetailsGridView.SelectedRows(0).Cells("RunNumber").Value)
                    myResultInformationRow.CalNumberSeries = CInt(bsResultsDetailsGridView.SelectedRows(0).Cells("CalcRunNumber").Value)
                    myResultInformationRow.ResultValue = CDbl(bsResultsDetailsGridView.SelectedRows(0).Cells("ResultValue").Value)
                    myResultInformationRow.Excluded = CBool(bsResultsDetailsGridView.SelectedRows(0).Cells("Excluded").Value)

                    If (Not bsResultsDetailsGridView.SelectedRows(0).Cells("ResultComment").Value Is DBNull.Value) Then
                        myResultInformationRow.ResultComment = bsResultsDetailsGridView.SelectedRows(0).Cells("ResultComment").Value
                    End If
                    myResultInformationRow.ManualResultValue = 0
                    myResultInformationDS.tResultInformation.AddtResultInformationRow(myResultInformationRow)

                    myQCResultEdition.ResultInformationDS = myResultInformationDS
                    myQCResultEdition.LanguageID = currentLanguage
                    myQCResultEdition.ShowDialog()
                End Using

                'Recalculate Results By Control/Lot after edition 
                BindResultsControlLot(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text), bsDateFromDateTimePick.Value, bsDateToDateTimePick.Value)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EditResultValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EditResultValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enable/disable all screen fields
    ''' </summary>
    ''' <param name="pEnable">True to enable the fields; otherwise, False</param>
    ''' <remarks>
    ''' Created by:  TR 18/07/2011
    ''' Modified by: SA 16/11/2011 - Multirules fields are not enabled/disabled in this method due to it works wrong
    '''                              after edition of a QC Result in the Results Details DataGridView
    '''              TR 20/04/2012 - Set status of buttons according level of the connected User
    ''' </remarks>
    Private Sub EnableDisableControls(ByVal pEnable As Boolean)
        Try
            '** Fields in Calculation Criteria Area
            bsDateFromDateTimePick.Enabled = pEnable
            bsDateToDateTimePick.Enabled = pEnable

            bsCalculationModeCombo.Enabled = pEnable
            bsRejectionNumeric.Enabled = pEnable
            If (pEnable AndAlso bsCalculationModeCombo.SelectedValue.ToString() = "MANUAL") Then
                bsNumberOfSeriesNumeric.Enabled = Not pEnable
            Else
                bsNumberOfSeriesNumeric.Enabled = pEnable
            End If
            bsSearchButton.Enabled = pEnable

            '** Fields in Results by Control/Lot Area
            bsResultControlLotGridView.Enabled = pEnable

            '** Fields in Results Details Area
            bsResultsDetailsGridView.Enabled = pEnable

            '** Buttons in General Area
            bsCumulateButton.Enabled = pEnable
            bsPrintButton.Enabled = pEnable

            ScreenStatusByUserLevel()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EnableDisableControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EnableDisableControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the ComboBox of Calculation Mode
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 27/05/2011
    ''' </remarks>
    Private Sub FillCalculationModeCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            'Get available QC Calculation Mode from table of Preloaded Master Data
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.QC_CALC_MODES)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myPreMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                Dim calculationModeList As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow) = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                                                                                                    Order By a.Position Select a).ToList()

                bsCalculationModeCombo.DataSource = calculationModeList
                bsCalculationModeCombo.DisplayMember = "FixedItemDesc"
                bsCalculationModeCombo.ValueMember = "ItemID"

                calculationModeList = Nothing
            Else
                'Error getting the list of available QC Calculation Mode; show it
                ShowMessage(Name & ".FillCalculationModeCombo ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillCalculationModeCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillCalculationModeCombo ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the QC Test/Sample Types ListView with all open Test/SampleTypes that exist in the history
    ''' QC table for Test/Sample Types
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 26/05/2011 
    ''' Modified by: SA 05/06/2012 - Selected the Icon to shown depending on the TestType; add also field TestType as ListView column 
    '''                            - Informed ID of the connected Analyzer when calling function ReadAll in HistoryTestSamplesDelegate
    ''' </remarks>
    Private Sub FillTestSampleListView()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQCHistoryTestSampleDelegate As New HistoryTestSamplesDelegate

            'myGlobalDataTO = myQCHistoryTestSampleDelegate.ReadAll(Nothing, True)
            myGlobalDataTO = myQCHistoryTestSampleDelegate.ReadAllNEW(Nothing, AnalyzerIDAttribute, True)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myQCHistoryTestSampleDS As HistoryTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)

                'Show first the Standard Biosystems Tests, then the User-defined Standard Tests and finally the ISE Tests
                Dim myQCHistoryTestSampleList As List(Of HistoryTestSamplesDS.tqcHistoryTestSamplesRow) = (From a In myQCHistoryTestSampleDS.tqcHistoryTestSamples _
                                                                                                       Order By a.TestType Descending, a.PreloadedTest Descending, a.TestName).ToList()
                Dim myIndex As Integer = 0
                Dim myIconNameVar As String = ""
                For Each historyTestSampRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow In myQCHistoryTestSampleList
                    'Select icon depending on value of the Preloaded Test flag
                    If (historyTestSampRow.TestType = "STD") Then
                        If (historyTestSampRow.PreloadedTest) Then
                            'Biosystem Test
                            myIconNameVar = "TESTICON"
                        Else
                            'User Test
                            myIconNameVar = "USERTEST"
                        End If
                    ElseIf (historyTestSampRow.TestType = "ISE") Then
                        myIconNameVar = "TISE_SYS"
                    End If

                    'Add the Test/Sample Type to the ListView
                    bsTestSampleListView.Items.Add(historyTestSampRow.TestName, myIconNameVar).SubItems.Add(historyTestSampRow.SampleType)
                    bsTestSampleListView.Items(myIndex).SubItems.Add(historyTestSampRow.QCTestSampleID)
                    bsTestSampleListView.Items(myIndex).SubItems.Add(historyTestSampRow.TestID)
                    bsTestSampleListView.Items(myIndex).SubItems.Add(historyTestSampRow.DecimalsAllowed)
                    bsTestSampleListView.Items(myIndex).SubItems.Add(historyTestSampRow.TestType)

                    myIndex += 1
                Next
                myQCHistoryTestSampleList = Nothing
            Else
                'Error getting the list of open Test/SampleTypes in the history QC table; show it
                ShowMessage(Name & ".FillTestSampleListView ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillTestSampleListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillTestSampleListView ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Filter by the selected Control/Lots when user select/unselect Control/Lots on ResultControlLotGridView
    ''' </summary>
    ''' <param name="pQCTestSampleID">Identifier (in the history QC tables) of the selected Test/SampleType</param>
    ''' <remarks>
    ''' Created by:  TR 08/06/2011
    ''' Modified by: SA 05/12/2011 - When Calculation Mode is Statistical and for the selected Controls there are results marked
    '''                              as included in the Mean, fill column IncludeInMean with symbol Xm for those results and set
    '''                              Visible = True for the column
    '''                            - Sort results by CalcRunNumber instead of by RunNumber
    ''' </remarks>
    Private Sub FilterQCResultsBySelectedControls(ByVal pQCTestSampleID As Integer)
        Try
            If (PrevSelectedControlName.Count > 0) Then
                For Each openQCResultRow As OpenQCResultsDS.tOpenResultsRow In LocalOpenQCResultsDS.tOpenResults.Rows
                    'Search if the element was selected on my previous selected list to mark as selected
                    If ((From a In PrevSelectedControlName Where a = openQCResultRow.ControlName Select a).Count > 0) Then
                        openQCResultRow.Selected = True
                    Else
                        openQCResultRow.Selected = False
                    End If
                Next
            Else
                PrevSelectedControlName = (From a In LocalOpenQCResultsDS.tOpenResults _
                                      Where Not a.IsSelectedNull AndAlso a.Selected _
                                         Select a.ControlName).ToList
            End If

            FilterQCResultsList = (From a In LocalQCResultsDS.tqcResults Join b In LocalOpenQCResultsDS.tOpenResults _
                                                                           On a.QCControlLotID Equals b.QCControlLotID _
                               Where Not b.IsSelectedNull AndAlso b.Selected _
                                 AndAlso a.QCTestSampleID = pQCTestSampleID _
                                  Select a Order By a.CalcRunNumber Descending).ToList()
            bsResultsDetailsGridView.DataSource = FilterQCResultsList

            'The column containing the mark of Result included in calculation of Statistics Mean is shown only for 
            'Test/SampleTypes with Calculation Mode = STATISTICS
            bsResultsDetailsGridView.Columns("IncludedInMean").Visible = False
            If (bsCalculationModeCombo.SelectedValue.ToString = "STATISTIC") Then
                Dim i As Integer = 0
                Dim resultInMean As List(Of QCResultsDS.tqcResultsRow)

                For Each row As DataGridViewRow In bsResultsDetailsGridView.Rows
                    resultInMean = (From a In LocalQCResultsDS.tqcResults _
                                   Where a.QCControlLotID = Convert.ToInt32(row.Cells("QCControlLotID").Value) _
                                 AndAlso a.RunNumber = Convert.ToInt32(row.Cells("RunNumber").Value) _
                             AndAlso Not a.IsIncludedInMeanNull AndAlso a.IncludedInMean = True _
                                  Select a).ToList()

                    If (resultInMean.Count = 1) Then
                        i += 1
                        row.Cells("IncludedInMean").Value = "Xm"
                    End If
                Next
                bsResultsDetailsGridView.Columns("IncludedInMean").Visible = (i > 0)
            End If
            bsResultsDetailsGridView.Refresh()

            'Validate if there are Results for the selected Control/Lots to enable/disable buttons in Results area
            If (bsResultsDetailsGridView.RowCount > 0) Then
                bsEditButtom.Enabled = True
                bsDeleteButtom.Enabled = True
                bsGraphsButton.Enabled = True
            Else
                bsEditButtom.Enabled = False
                bsDeleteButtom.Enabled = False
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FilterQCResultsBySelectedControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FilterQCResultsBySelectedControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the the maximum Run Number inside the informed Runs Group for the Test/SampleType and Control/Lot
    ''' </summary>
    ''' <param name="pQCTestSampleID">Identifier (in the history QC tables) of the selected Test/SampleType</param>
    ''' <param name="pQCControlLotID">Identifier (in the history QC tables) of the selected Control/Lot</param>
    ''' <param name="pRunsGroupNumber">Number of the Runs Group</param>
    ''' <returns>Integer value containing the maximum Run Number inside the informed Runs Group for the Test/SampleType 
    '''          and Control/Lot</returns>
    ''' <remarks>
    ''' Created by:  TR 10/06/2011
    ''' </remarks>
    Private Function GetMaxRunNumber(ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, ByVal pRunsGroupNumber As Integer) As Integer
        Dim myMaxRunNumber As Integer = 0
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQCResultsDelegate As New QCResultsDelegate

            myGlobalDataTO = myQCResultsDelegate.GetMaxRunNumberNEW(Nothing, pQCTestSampleID, pQCControlLotID, AnalyzerIDAttribute, pRunsGroupNumber)
            If (Not myGlobalDataTO.HasError) Then
                myMaxRunNumber = CInt(myGlobalDataTO.SetDatos)
            Else
                'Error getting the maximum Run Number; show it
                ShowMessage(Name & ".GetMaxRunNumber ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetMaxRunNumber ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetMaxRunNumber ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myMaxRunNumber
    End Function

    ''' <summary>
    ''' Get non cumulate QC Results inside the informed date interval, for all Control/Lots linked to the 
    ''' selected Test/SampleType
    ''' </summary>
    ''' <param name="pQCTestSampleID">Identifier (in the history QC tables) of the selected Test/SampleType</param>
    ''' <param name="pOpenQCResultsDS">Typed Dataset OpenQCResultsDS containing the list of Control/Lots linked to the 
    '''                                informed Test/SampleType</param>
    ''' <param name="pDateFrom">Min QC Result date</param>
    ''' <param name="pDateTo">Max QC Result data</param>
    ''' <remarks>
    ''' Created by:  TR 03/06/2011
    ''' Modified by: SA 05/06/2012 - ID of the connected Analyzer informed when calling function GetNonCumulateResultForAllControlLots 
    '''                              in QCResultsDelegate
    ''' </remarks>
    Private Sub GetNonCumulateResultForAllControls(ByVal pQCTestSampleID As Integer, ByVal pOpenQCResultsDS As OpenQCResultsDS, _
                                                   ByVal pDateFrom As DateTime, ByVal pDateTo As DateTime)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQCResultDel As New QCResultsDelegate

            'myGlobalDataTO = myQCResultDel.GetNonCumulateResultForAllControlLots(Nothing, pQCTestSampleID, pOpenQCResultsDS, pDateFrom, pDateTo)
            myGlobalDataTO = myQCResultDel.GetNonCumulateResultForAllControlLotsNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute, pOpenQCResultsDS, pDateFrom, pDateTo)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                LocalQCResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCResultsDS)
            Else
                'Error getting the list of non cumulated QC Results; show it
                ShowMessage(Name & ".GetNonCumulateResultForAllControls ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetNonCumulateResultForAllControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetNonCumulateResultForAllControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls.
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  TR 26/05/2011
    ''' Modified by: SA 26/01/2012 - Get also label for new Title label for area of Results in the Controls/Lots section
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsTestSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", pLanguageID)
            bsCalculationCriteriaLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Calculation_Criteria", pLanguageID)
            bsDateFromLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date_From", pLanguageID) & ":"
            bsDateToLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date_To", pLanguageID) & ":"
            bsMultirulesApplicationLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Multirules_Application", pLanguageID) & ":"
            bsCalculationModeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calculation_Mode", pLanguageID) & ":"
            bsRejectionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rejection", pLanguageID) & ":"
            bsSDLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", pLanguageID)

            bsControlLotLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Controls_List", pLanguageID)
            bsControlLotResultsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Results", pLanguageID)
            bsIndividualResultDetLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Individual_Results", pLanguageID)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 26/05/2011
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString()

            GetScreenLabels(currentLanguage)
            PrepareButtons(currentLanguage)
            PrepareTestListView(currentLanguage)
            PrepareResultControlLotGrid(currentLanguage)
            PrepareResultDetailsGrid(currentLanguage)
            FillCalculationModeCombo()
            SetQCLimits()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the same Control has been selected as first and second Control for application of the 
    ''' selected Westgard Multirules
    ''' </summary>
    ''' <param name="pMultiruleCboName">Name of the Multirules ComboBox that is validated</param>
    ''' <remarks>
    ''' Created by:  TR
    ''' </remarks>
    Private Sub IsMultirulesApplicationEqual(ByVal pMultiruleCboName As String)
        Try
            If (bsMultirulesApplication1Combo.SelectedValue.ToString = bsMultirulesApplication2Combo.SelectedValue.ToString()) Then
                If (pMultiruleCboName = bsMultirulesApplication1Combo.Name) Then
                    bsMultirulesApplication2Combo.SelectedIndex = -1
                Else
                    bsMultirulesApplication1Combo.SelectedIndex = -1
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IsMultirulesApplicationEqual ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IsMultirulesApplicationEqual ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When a new Test/SampleType is selected, get and load all non-cumulated QC Results for all linked Control/Lots
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 13/11/2014 - BA-1885 ==> Code moved from SelectedIndexChanged event of the ListView containing the Tests/SampleTypes.
    '''                                          Disable the screen and show the hourglass cursor while details areas are beign loaded 
    ''' </remarks>
    Private Sub LoadDataOfSelectedTestSample()
        Try
            If (bsTestSampleListView.SelectedItems.Count = 1) Then
                Me.Enabled = False
                Cursor = Cursors.WaitCursor

                LoadingData = True

                'Clean previous filter control; clean the Error Provider control
                PrevSelectedControlName.Clear()
                bsResultErrorProv.Clear()

                'Set the decimals allowed to my local variable to format numeric values
                LocalDecimalAllow = CInt(bsTestSampleListView.SelectedItems(0).SubItems(4).Text)
                BindCalculationCriteria(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text), True)
                LoadingData = False

                Me.Enabled = True
                Cursor = Cursors.Default
            Else
                'TR 23/07/2012 - If there is not a Test/SampleType selected in the list, then all Controls are cleared
                ClearControls()
            End If
        Catch ex As Exception
            Me.Enabled = True
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadDataOfSelectedTestSample ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadDataOfSelectedTestSample ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize and configure all Screen controls and load data for the first Test/Sample Type in the list when the 
    ''' screen is opened
    ''' </summary>
    ''' <param name="pInitialScreenLoad">True if the method is called for initial screen loading; False if the method is 
    '''                                  called to refresh the screen after closing the auxiliary screen of Cumulate Control Results</param>
    ''' <remarks>
    ''' Created by:  SA 13/11/2014 - BA-1885 ==> Code moved from the Screen Load Event
    ''' </remarks>
    Private Sub LoadScreen(ByVal pInitialScreenLoad As Boolean)
        Try
            InitializeScreen()
            FillTestSampleListView()

            If (pInitialScreenLoad) Then
                'TR 20/04/2012 - Get level of the connected User
                Dim myGlobalBase As New GlobalBase
                CurrentUserLevel = myGlobalBase.GetSessionInfo().UserLevel
            End If

            If (bsTestSampleListView.Items.Count = 0) Then
                'Disable screen controls
                EnableDisableControls(False)

                bsAddButtom.Enabled = False
                bsEditButtom.Enabled = False
                bsDeleteButtom.Enabled = False
                bsGraphsButton.Enabled = False
            Else
                If (pInitialScreenLoad) Then
                    If (QCTestSampleIDAttribute > 0) Then
                        'Select the Test/SampleType on the ListView
                        SelectQCTestSampleID(QCTestSampleIDAttribute)
                    Else
                        'Select the first Test/SampleType on the ListView
                        bsTestSampleListView.Items(0).Selected = True
                    End If
                Else
                    'Select the first Test/SampleType on the ListView
                    bsTestSampleListView.Items(0).Selected = True
                End If

                'Enable screen controls
                EnableDisableControls(True)

                bsAddButtom.Enabled = True
                bsEditButtom.Enabled = True
                bsDeleteButtom.Enabled = True
                bsGraphsButton.Enabled = True

                'TR 20/04/2012 - Validate the user level to activate/desactivate functionalities
                If (pInitialScreenLoad) Then ScreenStatusByUserLevel()
            End If

            If (pInitialScreenLoad) Then
                ResetBorder()
                Application.DoEvents()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get icons and tooltip texts of all screen buttons
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  TR 26/05/2011
    ''' </remarks>
    Private Sub PrepareButtons(ByVal pLanguageID As String)
        Try
            Dim auxIconName As String = ""
            Dim myToolTipsControl As New ToolTip
            Dim iconPath As String = MyBase.IconsPath
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'SEARCH Button
            auxIconName = GetIconName("FIND")
            If (auxIconName <> "") Then
                bsSearchButton.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsSearchButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Search", pLanguageID))
            End If

            'NEW Button
            auxIconName = GetIconName("ADD")
            If (auxIconName <> "") Then
                bsAddButtom.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsAddButtom, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AddNew", pLanguageID))
            End If

            'EDIT Button
            auxIconName = GetIconName("EDIT")
            If (auxIconName <> "") Then
                bsEditButtom.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsEditButtom, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_EDIT", pLanguageID))
            End If

            'DELETE Button
            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                bsDeleteButtom.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsDeleteButtom, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))
            End If

            'LJ & YOUDEN GRAPHS Button
            auxIconName = GetIconName("ABS_GRAPH")
            If (auxIconName <> "") Then
                bsGraphsButton.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsGraphsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Graph", pLanguageID))
            End If

            'CUMULATE RESULTS Button
            auxIconName = GetIconName("QCCUM")
            If (auxIconName <> "") Then
                bsCumulateButton.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsCumulateButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Cumulate_Results", pLanguageID))
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the Results by Control/Lot DataGridView 
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  TR 01/05/2011
    ''' Modified by: SA 30/11/2011 - Added three new visible columns for Calculated values of Mean, SD and CV. Added
    '''                              a dummy column to separate the assigned values and the result values     
    '''              SA 25/01/2012 - Column LotNumber is deleted due to column ControlName will shown ControlName (LotNumber)     
    '''                              Changed labels used for columns containing calculated Mean, SD and CV         
    '''              JC 12/11/2012 - Added columns Mean, CV and CalcCV     
    '''              SA 12/11/2014 - BA-1885 ==> Extend the width of column "n" to allow show numbers of three digits. Reduce width
    '''                                          of columns CalcMean and CalcSD proportionally
    ''' </remarks>
    Private Sub PrepareResultControlLotGrid(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsResultControlLotGridView.AutoSize = False
            bsResultControlLotGridView.MultiSelect = False
            bsResultControlLotGridView.AutoGenerateColumns = False
            bsResultControlLotGridView.Columns.Clear()

            'CheckBox to Select/Unselect the Control/Lot to show the details of its Results
            Dim ActiveControlColChkBox As New DataGridViewCheckBoxColumn
            ActiveControlColChkBox.Width = 20
            ActiveControlColChkBox.Name = "ActiveControl"
            ActiveControlColChkBox.HeaderText = ""
            ActiveControlColChkBox.DataPropertyName = "Selected"
            ActiveControlColChkBox.Resizable = DataGridViewTriState.False
            bsResultControlLotGridView.Columns.Add(ActiveControlColChkBox)
            bsResultControlLotGridView.Columns("ActiveControl").ReadOnly = False

            'Control Name (Lot Number)
            bsResultControlLotGridView.Columns.Add("ControlName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", pLanguageID))
            bsResultControlLotGridView.Columns("ControlName").Width = 165
            bsResultControlLotGridView.Columns("ControlName").DataPropertyName = "ControlNameLotNum"
            bsResultControlLotGridView.Columns("ControlName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsResultControlLotGridView.Columns("ControlName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsResultControlLotGridView.Columns("ControlName").ReadOnly = True

            'Assigned Mean for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("Mean", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pLanguageID))
            bsResultControlLotGridView.Columns("Mean").Width = 50
            bsResultControlLotGridView.Columns("Mean").DataPropertyName = "Mean"
            bsResultControlLotGridView.Columns("Mean").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("Mean").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("Mean").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("Mean").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("Mean").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("Mean").ReadOnly = True

            'Test Measure Unit
            bsResultControlLotGridView.Columns.Add("Unit", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", pLanguageID).TrimEnd())
            bsResultControlLotGridView.Columns("Unit").Width = 60
            bsResultControlLotGridView.Columns("Unit").DataPropertyName = "MeasureUnit"
            bsResultControlLotGridView.Columns("Unit").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("Unit").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("Unit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("Unit").ReadOnly = True

            'Assigned Standard Deviation for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("SD", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", pLanguageID))
            bsResultControlLotGridView.Columns("SD").Width = 55
            bsResultControlLotGridView.Columns("SD").DataPropertyName = "SD"
            bsResultControlLotGridView.Columns("SD").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("SD").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("SD").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("SD").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("SD").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("SD").ReadOnly = True

            'Assigned Coefficient of Variation for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("CV", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CV", pLanguageID))
            bsResultControlLotGridView.Columns("CV").Width = 45
            bsResultControlLotGridView.Columns("CV").DataPropertyName = "CV"
            bsResultControlLotGridView.Columns("CV").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("CV").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("CV").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("CV").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CV").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CV").ReadOnly = True

            'Assigned Min/Max Concentration values for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("Ranges", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Ranges", pLanguageID))
            bsResultControlLotGridView.Columns("Ranges").Width = 100
            bsResultControlLotGridView.Columns("Ranges").DataPropertyName = "Ranges"
            bsResultControlLotGridView.Columns("Ranges").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("Ranges").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("Ranges").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("Ranges").ReadOnly = True

            'Separator
            bsResultControlLotGridView.Columns.Add("Dummy", "")
            bsResultControlLotGridView.Columns("Dummy").Width = 2
            bsResultControlLotGridView.Columns("Dummy").ReadOnly = True

            'Number of open QC Results (not included in the statistical calculation of the Mean) for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("n", "n")
            bsResultControlLotGridView.Columns("n").Width = 35
            bsResultControlLotGridView.Columns("n").DataPropertyName = "n"
            bsResultControlLotGridView.Columns("n").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("n").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("n").ReadOnly = True
            bsResultControlLotGridView.Columns("n").SortMode = DataGridViewColumnSortMode.NotSortable

            'Additional columns to show Mean, SD and CV calculated from all obtained open QC Results
            'Calculated Mean of open QC Results for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("CalcMean", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pLanguageID))
            bsResultControlLotGridView.Columns("CalcMean").Width = 45
            bsResultControlLotGridView.Columns("CalcMean").DataPropertyName = "CalcMean"
            bsResultControlLotGridView.Columns("CalcMean").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("CalcMean").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("CalcMean").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("CalcMean").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcMean").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcMean").ReadOnly = True

            'Assigned Standard Deviation for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("CalcSD", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", pLanguageID))
            bsResultControlLotGridView.Columns("CalcSD").Width = 50
            bsResultControlLotGridView.Columns("CalcSD").DataPropertyName = "CalcSD"
            bsResultControlLotGridView.Columns("CalcSD").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("CalcSD").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("CalcSD").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("CalcSD").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcSD").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcSD").ReadOnly = True

            'Assigned Coefficient of Variation for the Test/SampleType and Control/Lot
            bsResultControlLotGridView.Columns.Add("CalcCV", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CV", pLanguageID))
            bsResultControlLotGridView.Columns("CalcCV").Width = 50
            bsResultControlLotGridView.Columns("CalcCV").DataPropertyName = "CalcCV"
            bsResultControlLotGridView.Columns("CalcCV").DefaultCellStyle.NullValue = Nothing
            bsResultControlLotGridView.Columns("CalcCV").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("CalcCV").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsResultControlLotGridView.Columns("CalcCV").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcCV").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CalcCV").ReadOnly = True

            'Not visible columns...
            '** Identifier of the Control/Lot in QC Module
            bsResultControlLotGridView.Columns.Add("QCControlLotID", "QCControlLotID")
            bsResultControlLotGridView.Columns("QCControlLotID").DataPropertyName = "QCControlLotID"
            bsResultControlLotGridView.Columns("QCControlLotID").Visible = False

            '** Runs Group Number
            bsResultControlLotGridView.Columns.Add("RunsGroupNumber", "RunsGroupNumber")
            bsResultControlLotGridView.Columns("RunsGroupNumber").DataPropertyName = "RunsGroupNumber"
            bsResultControlLotGridView.Columns("RunsGroupNumber").Visible = False

            '** Number of the last Run included in the statistical calculation (only when Calculation Mode = STATISTICS)
            bsResultControlLotGridView.Columns.Add("LastMeanRunNumber", "LastMeanRunNumber")
            bsResultControlLotGridView.Columns("LastMeanRunNumber").DataPropertyName = "LastMeanRunNumber"
            bsResultControlLotGridView.Columns("LastMeanRunNumber").Visible = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareResultControlLotGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareResultControlLotGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the Result Details DataGridView
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  TR 10/06/2011
    ''' Modified by: SA 01/12/2011 - Added visible column for Relative Error expressed as percentage
    '''                              Hide column Relative Error (SDi)
    ''' </remarks>
    Private Sub PrepareResultDetailsGrid(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsResultsDetailsGridView.AutoGenerateColumns = False
            bsResultsDetailsGridView.AutoSize = False
            bsResultsDetailsGridView.ReadOnly = True
            bsResultsDetailsGridView.MultiSelect = True
            bsResultsDetailsGridView.Columns.Clear()

            'Included in Mean Mark
            bsResultsDetailsGridView.Columns.Add("IncludedInMean", "")
            bsResultsDetailsGridView.Columns("IncludedInMean").Width = 30
            bsResultsDetailsGridView.Columns("IncludedInMean").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultsDetailsGridView.Columns("IncludedInMean").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Run Number (in screen)
            bsResultsDetailsGridView.Columns.Add("CalcRunNumber", "n")
            bsResultsDetailsGridView.Columns("CalcRunNumber").Width = 35
            bsResultsDetailsGridView.Columns("CalcRunNumber").DataPropertyName = "CalcRunNumber"
            bsResultsDetailsGridView.Columns("CalcRunNumber").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultsDetailsGridView.Columns("CalcRunNumber").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            'Control Name
            bsResultsDetailsGridView.Columns.Add("ControlName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", pLanguageID))
            bsResultsDetailsGridView.Columns("ControlName").Width = 130
            bsResultsDetailsGridView.Columns("ControlName").DataPropertyName = "ControlName"
            bsResultsDetailsGridView.Columns("ControlName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopLeft
            bsResultsDetailsGridView.Columns("ControlName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft

            'Result DateTime
            bsResultsDetailsGridView.Columns.Add("ResultDate", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", pLanguageID))
            bsResultsDetailsGridView.Columns("ResultDate").Width = 140
            bsResultsDetailsGridView.Columns("ResultDate").DataPropertyName = "ResultDateTime"
            bsResultsDetailsGridView.Columns("ResultDate").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopLeft
            bsResultsDetailsGridView.Columns("ResultDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft

            'ResultValue 
            bsResultsDetailsGridView.Columns.Add("ResultValue", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Result", pLanguageID))
            bsResultsDetailsGridView.Columns("ResultValue").Width = 70
            bsResultsDetailsGridView.Columns("ResultValue").DataPropertyName = "VisibleResultValue"
            bsResultsDetailsGridView.Columns("ResultValue").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("ResultValue").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopRight
            bsResultsDetailsGridView.Columns("ResultValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopRight

            'Icon to indicate the Result has been edited
            Dim myDataGridViewImageColumn As New DataGridViewImageColumn
            myDataGridViewImageColumn.Name = "EditResultIcon"
            myDataGridViewImageColumn.HeaderText = ""
            myDataGridViewImageColumn.DataPropertyName = "IconPath"
            myDataGridViewImageColumn.Width = 20
            myDataGridViewImageColumn.DefaultCellStyle.NullValue = Nothing
            myDataGridViewImageColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter
            myDataGridViewImageColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
            myDataGridViewImageColumn.ImageLayout = DataGridViewImageCellLayout.Normal

            bsResultsDetailsGridView.Columns.Add(myDataGridViewImageColumn)
            bsResultsDetailsGridView.Columns("EditResultIcon").DefaultCellStyle.BackColor = Color.White
            bsResultsDetailsGridView.Columns("EditResultIcon").SortMode = DataGridViewColumnSortMode.NotSortable

            'Measure Unit
            bsResultsDetailsGridView.Columns.Add("MeasureUnit", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", pLanguageID))
            bsResultsDetailsGridView.Columns("MeasureUnit").Width = 65
            bsResultsDetailsGridView.Columns("MeasureUnit").DataPropertyName = "MeasureUnit"
            bsResultsDetailsGridView.Columns("MeasureUnit").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
            bsResultsDetailsGridView.Columns("MeasureUnit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter
            bsResultsDetailsGridView.Columns("MeasureUnit").SortMode = DataGridViewColumnSortMode.NotSortable

            'Absolute Error
            bsResultsDetailsGridView.Columns.Add("ABSError", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ABS_Error", pLanguageID))
            bsResultsDetailsGridView.Columns("ABSError").Width = 70
            bsResultsDetailsGridView.Columns("ABSError").DataPropertyName = "ABSError"
            bsResultsDetailsGridView.Columns("ABSError").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("ABSError").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopRight
            bsResultsDetailsGridView.Columns("ABSError").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopRight

            'Relative Error (%)
            bsResultsDetailsGridView.Columns.Add("RELErrorPercent", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rel_Error", pLanguageID))
            bsResultsDetailsGridView.Columns("RELErrorPercent").Width = 90
            bsResultsDetailsGridView.Columns("RELErrorPercent").DataPropertyName = "RELErrorPercent"
            bsResultsDetailsGridView.Columns("RELErrorPercent").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("RELErrorPercent").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopRight
            bsResultsDetailsGridView.Columns("RELErrorPercent").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopRight

            'List of Results Alarms
            bsResultsDetailsGridView.Columns.Add("AlarmsList", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Alarms", pLanguageID)) 'JB 01/10/2012 - Resource String unification
            bsResultsDetailsGridView.Columns("AlarmsList").Width = 250
            bsResultsDetailsGridView.Columns("AlarmsList").DataPropertyName = "AlarmsList"
            bsResultsDetailsGridView.Columns("AlarmsList").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopLeft
            bsResultsDetailsGridView.Columns("AlarmsList").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft

            'Not visible columns...
            '** Identifier of ControlID/LotNumber in QC Module 
            bsResultsDetailsGridView.Columns.Add("QCControlLotID", "QCControlLotID")
            bsResultsDetailsGridView.Columns("QCControlLotID").DataPropertyName = "QCControlLotID"
            bsResultsDetailsGridView.Columns("QCControlLotID").Visible = False

            '** RunNumber (value in DB)
            bsResultsDetailsGridView.Columns.Add("RunNumber", "RunNumber")
            bsResultsDetailsGridView.Columns("RunNumber").DataPropertyName = "RunNumber"
            bsResultsDetailsGridView.Columns("RunNumber").Visible = False

            '** Excluded flag
            bsResultsDetailsGridView.Columns.Add("Excluded", "Excluded")
            bsResultsDetailsGridView.Columns("Excluded").DataPropertyName = "Excluded"
            bsResultsDetailsGridView.Columns("Excluded").Visible = False

            '** Lot Number
            bsResultsDetailsGridView.Columns.Add("LotNumber", "LotNumber")
            bsResultsDetailsGridView.Columns("LotNumber").DataPropertyName = "LotNumber"
            bsResultsDetailsGridView.Columns("LotNumber").Visible = False

            '** Runs Group Number
            bsResultsDetailsGridView.Columns.Add("RunsGroupNumber", "RunsGroupNumber")
            bsResultsDetailsGridView.Columns("RunsGroupNumber").DataPropertyName = "RunsGroupNumber"
            bsResultsDetailsGridView.Columns("RunsGroupNumber").Visible = False

            '** Result Comment
            bsResultsDetailsGridView.Columns.Add("ResultComment", "ResultComment")
            bsResultsDetailsGridView.Columns("ResultComment").DataPropertyName = "ResultComment"
            bsResultsDetailsGridView.Columns("ResultComment").Visible = False

            '** Relative Error (SDi)
            bsResultsDetailsGridView.Columns.Add("RELError", "RELError")
            bsResultsDetailsGridView.Columns("RELError").DataPropertyName = "RELError"
            bsResultsDetailsGridView.Columns("RELError").Visible = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareResultDetailsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareResultDetailsGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the QC Test/Sample Types ListView
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  TR 27/05/2011
    ''' Modified by: SA 05/06/2012 - Load in the Image list the icon for ISE Tests 
    ''' </remarks>
    Private Sub PrepareTestListView(ByVal pLanguageID As String)
        Try
            Dim myTestIconList As New ImageList
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            myTestIconList.Images.Add("TESTICON", Image.FromFile(MyBase.IconsPath & GetIconName("TESTICON")))
            myTestIconList.Images.Add("USERTEST", Image.FromFile(MyBase.IconsPath & GetIconName("USERTEST")))
            myTestIconList.Images.Add("TISE_SYS", Image.FromFile(MyBase.IconsPath & GetIconName("TISE_SYS")))

            bsTestSampleListView.Items.Clear()
            bsTestSampleListView.Scrollable = True
            bsTestSampleListView.MultiSelect = False
            bsTestSampleListView.View = View.Details
            bsTestSampleListView.FullRowSelect = True
            bsTestSampleListView.AllowColumnReorder = False
            bsTestSampleListView.AutoArrange = False
            bsTestSampleListView.HideSelection = False
            bsTestSampleListView.SmallImageList = myTestIconList
            bsTestSampleListView.Alignment = ListViewAlignment.Left
            bsTestSampleListView.HeaderStyle = ColumnHeaderStyle.Clickable

            bsTestSampleListView.Columns.Add(myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestNames", pLanguageID), 168, HorizontalAlignment.Left)
            bsTestSampleListView.Columns.Add("", 50, HorizontalAlignment.Center)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareTestListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareTestListView ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set elements to Nothing to release memory when the screen is closed
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ReleaseElements()
        Try
            bsTestSampleListView = Nothing
            bsTestSampleGroupBox = Nothing
            bsTestSampleTypeLabel = Nothing
            bsCalculationCriteriaGroupBox = Nothing
            bsDateFromDateTimePick = Nothing
            bsDateFromLabel = Nothing
            bsCalculationCriteriaLabel = Nothing
            bsMultirulesApplication2Combo = Nothing
            bsMultirulesApplication1Combo = Nothing
            bsMultirulesApplicationLabel = Nothing
            bsDateToDateTimePick = Nothing
            bsDateToLabel = Nothing
            bsNumberOfSeriesNumeric = Nothing
            bsSearchButton = Nothing
            bsCalculationModeCombo = Nothing
            bsCalculationModeLabel = Nothing
            bsRejectionNumeric = Nothing
            bsRejectionLabel = Nothing
            bsRulesGroupbox = Nothing
            bs22SCheckBox = Nothing
            bs13SCheckBox = Nothing
            bs12SCheckBox = Nothing
            bsSDLabel = Nothing
            bs10XmCheckBox = Nothing
            bsR4SCheckBox = Nothing
            bsExitButton = Nothing
            bs41SCheckBox = Nothing
            bsResultsByCtrlGroupBox = Nothing
            bsControlLotLabel = Nothing
            bsResultControlLotGridView = Nothing
            bsResultsDetailsGridView = Nothing
            bsIndividualResultDetLabel = Nothing
            bsResultErrorProv = Nothing
            bsDeleteButtom = Nothing
            bsAddButtom = Nothing
            bsEditButtom = Nothing
            bsCumulateButton = Nothing
            bsPrintButton = Nothing
            bsGraphsButton = Nothing
            bsControlLotResultsLabel = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When values of CalculationMode, NumberOfSeries, RejectionCriteria and selected Westgard Multirules are
    ''' changed, they are also changed in the correspondent tables in Parameters Programming Module 
    ''' (tparTestSamples and tparTestSamplesMultirules)
    ''' </summary>
    ''' <returns>True if the update finished sucessfully; otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  TR 30/05/2011
    ''' Modified by: SA 25/01/2012 - Verify if ComboBox for second Westgard Control is enabled before get the selected value
    '''              SA 05/06/2012 - Added parameter for TestType and informed when calling function UpdateTestSampleMultiRules
    '''                            - ID of the connected Analyzer informed when calling function UpdateChangedValues in QCResultsDelegate
    ''' </remarks>
    Private Function SaveUpdatedValues(ByVal pQCTestSampleID As Integer, ByVal pTestType As String, ByVal pTestID As Integer, ByVal pSampleType As String) As Boolean
        Dim myResult As Boolean = True

        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQCResultsDelegate As New QCResultsDelegate

            'Get current values of CalculationMode, NumberOfSeries, and RejectionCriteria
            Dim mycalcMode As String = bsCalculationModeCombo.SelectedValue.ToString
            Dim myNumSeries As Integer = 0
            If (Not bsNumberOfSeriesNumeric.Text = "") Then myNumSeries = CInt(bsNumberOfSeriesNumeric.Text)
            Dim myRejecNumeric As Single = bsRejectionNumeric.Value

            'Get selected Westgard Multirules
            Dim myQCControlLotIDForWESG1 As String = String.Empty
            Dim myQCControlLotIDForWESG2 As String = String.Empty
            Dim myTestSampleMultirulesDS As New TestSamplesMultirulesDS
            If (bsRulesGroupbox.Enabled) Then
                'If the Multirule area is enable then update values
                '** Used to update the tqcHistoryTestControlLots
                myQCControlLotIDForWESG1 = bsMultirulesApplication1Combo.SelectedValue.ToString()
                If (bsMultirulesApplication2Combo.Enabled) Then myQCControlLotIDForWESG2 = bsMultirulesApplication2Combo.SelectedValue.ToString()

                '** Used to update the Parameters Programming table
                myTestSampleMultirulesDS = UpdateTestSampleMultiRules(pTestType, pTestID, pSampleType)
            End If

            'Finally execute the updates
            myGlobalDataTO = myQCResultsDelegate.UpdateChangedValuesNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute, mycalcMode, myNumSeries, myRejecNumeric, _
                                                                        myQCControlLotIDForWESG1, myQCControlLotIDForWESG2, myTestSampleMultirulesDS)
            If (myGlobalDataTO.HasError) Then
                myResult = False
                ShowMessage(Name & ".SaveUpdatedValues ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            myResult = False
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveUpdatedValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveUpdatedValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Enable or disable functionality according Level of the connected User
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 20/04/2012
    ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select
                Case "OPERATOR"
                    bsCumulateButton.Enabled = False
                    Exit Select
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Select a item on the List View by the QCTestSampleID
    ''' </summary>
    ''' <param name="pQCTestSampleID">Identifier (in the history QC tables) of the selected Test/SampleType</param>
    ''' <remarks>
    ''' Created by:  TR 14/06/2011
    ''' </remarks>
    Private Sub SelectQCTestSampleID(ByVal pQCTestSampleID As Integer)
        Try
            Dim myItemList As List(Of ListViewItem) = (From list In Me.bsTestSampleListView.Items.Cast(Of ListViewItem)() _
                                                      Where CInt(list.SubItems(2).Text) = pQCTestSampleID _
                                                     Select list).ToList()
            If (myItemList.Count > 0) Then
                Me.bsTestSampleListView.Items(myItemList.First().Index).Selected = True
                Me.bsTestSampleListView.Select()
            End If
            myItemList = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SelectTestSampleID ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SelectTestSampleID ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get limit values for all screen fields
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/05/2011
    ''' </remarks>
    Private Sub SetQCLimits()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myFieldLimitsDS As New FieldLimitsDS
            Dim myFieldLimitsDelegate As New FieldLimitsDelegate()

            'Get limit values for Rejection Criteria Numeric UpDown
            myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.CONTROL_REJECTION, "")
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myFieldLimitsDS = DirectCast(myGlobalDataTO.SetDatos, FieldLimitsDS)

                If (myFieldLimitsDS.tfmwFieldLimits.Count > 0) Then
                    bsRejectionNumeric.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                    bsRejectionNumeric.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                    If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull) Then
                        bsRejectionNumeric.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                    End If
                    If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsDecimalsAllowedNull) Then
                        bsRejectionNumeric.DecimalPlaces = myFieldLimitsDS.tfmwFieldLimits(0).DecimalsAllowed
                    End If
                End If
            Else
                'Error getting the limit value; shown it
                ShowMessage(Name & ".SetQCLimits ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If

            If (Not myGlobalDataTO.HasError) Then
                'Get limit values for Number of Series Numeric UpDown
                myGlobalDataTO = myFieldLimitsDelegate.GetList(Nothing, FieldLimitsEnum.CONTROL_MIN_NUM_SERIES, "")
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myFieldLimitsDS = DirectCast(myGlobalDataTO.SetDatos, FieldLimitsDS)

                    If (myFieldLimitsDS.tfmwFieldLimits.Count > 0) Then
                        bsNumberOfSeriesNumeric.Minimum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Decimal)
                        bsNumberOfSeriesNumeric.Maximum = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Decimal)
                        If (Not myFieldLimitsDS.tfmwFieldLimits(0).IsStepValueNull) Then
                            bsNumberOfSeriesNumeric.Increment = CType(myFieldLimitsDS.tfmwFieldLimits(0).StepValue, Decimal)
                        End If
                    End If
                Else
                    'Error getting the limit value; shown it
                    ShowMessage(Name & ".SetQCLimits ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetQCLimits ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetQCLimits ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the dataset needed to update the Westgard Multirules for the specified TestType/TestID/SampleType
    ''' in Parameters Programming Module
    ''' </summary>
    ''' <param name="pTestType">Test Type Code</param>
    ''' <param name="pTestID">Test Identifier in Parameters Programming Module</param>
    ''' <param name="pSampleType">Sample Type Code</param>
    ''' <returns>Typed Dataset TestSamplesMultirulesDS</returns>
    ''' <remarks>
    ''' Created by:  TR 30/05/2011
    ''' Modified by: SA 05/06/2012 - Added parameter for TestType
    ''' </remarks>
    Private Function UpdateTestSampleMultiRules(ByVal pTestType As String, ByVal pTestID As Integer, ByVal pSampleType As String) As TestSamplesMultirulesDS
        Dim myTestSampleMultirulesDS As New TestSamplesMultirulesDS

        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestSampleMultiDelegate As New TestSamplesMultirulesDelegate

            'Get the status of the Westgard Multirules for the selected Test/SampleType in DB
            myGlobalDataTO = myTestSampleMultiDelegate.GetByTestIDAndSampleTypeNEW(Nothing, pTestType, pTestID, pSampleType)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myTestSampleMultirulesDS = DirectCast(myGlobalDataTO.SetDatos, TestSamplesMultirulesDS)

                If (myTestSampleMultirulesDS.tparTestSamplesMultirules.Count = 0) Then
                    'Add all Multirules...
                    Dim myPreloadedDataDelegate As New PreloadedMasterDataDelegate

                    myGlobalDataTO = myPreloadedDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.QC_MULTIRULES)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myPreloadedMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                        Dim myTestSampleMultiRulesRow As TestSamplesMultirulesDS.tparTestSamplesMultirulesRow
                        For Each multiRule As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myPreloadedMasterDataDS.tfmwPreloadedMasterData.Rows
                            myTestSampleMultiRulesRow = myTestSampleMultirulesDS.tparTestSamplesMultirules.NewtparTestSamplesMultirulesRow
                            myTestSampleMultiRulesRow.TestType = pTestType
                            myTestSampleMultiRulesRow.TestID = pTestID
                            myTestSampleMultiRulesRow.SampleType = pSampleType
                            myTestSampleMultiRulesRow.RuleID = multiRule.ItemID
                            myTestSampleMultirulesDS.tparTestSamplesMultirules.AddtparTestSamplesMultirulesRow(myTestSampleMultiRulesRow)
                        Next
                    Else
                        'Error getting the list of available Westgard Multirules; show it
                        ShowMessage(Me.Name & ".UpdateTestSampleMultiRules ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobalDataTO.ErrorMessage)
                    End If
                End If

                If (Not myGlobalDataTO.HasError) Then
                    Dim qTestSampleMultiList As List(Of TestSamplesMultirulesDS.tparTestSamplesMultirulesRow)
                    qTestSampleMultiList = (From a In myTestSampleMultirulesDS.tparTestSamplesMultirules _
                                          Select a).ToList()

                    qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_1-2s").First().SelectedRule = bs12SCheckBox.Checked
                    qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_1-3s").First().SelectedRule = bs13SCheckBox.Checked
                    qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_2-2s").First().SelectedRule = bs22SCheckBox.Checked
                    qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_R-4s").First().SelectedRule = bsR4SCheckBox.Checked
                    qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_4-1s").First().SelectedRule = bs41SCheckBox.Checked
                    qTestSampleMultiList.Where(Function(a) a.RuleID = "WESTGARD_10X").First().SelectedRule = bs10XmCheckBox.Checked
                End If
            Else
                'Error getting the status of the Westgard Multirules for the selected Test/SampleType in DB; show it
                ShowMessage(Me.Name & ".UpdateTestSampleMultiRules ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateTestSampleMultiRules " & Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateTestSampleMultiRules ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myTestSampleMultirulesDS
    End Function

    ''' <summary>
    ''' Validate no more than three Control/Lots are selected in the grid of Results by Control/Lot
    ''' </summary>
    ''' <returns>True if the number of Controls selected is correct; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  SA 23/12/2011
    ''' </remarks>
    Private Function ValidateActiveControls() As Boolean
        Dim myResult As Boolean = False
        Try
            myResult = ((From a In LocalOpenQCResultsDS.tOpenResults _
                    Where Not a.IsSelectedNull AndAlso a.Selected _
                       Select a).Count <= 3)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateActiveControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateActiveControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Validate if all required files have a correct value</summary>
    ''' <returns>True if at least one of the required fields is not informed</returns>
    ''' <remarks>
    ''' Created by:  TR 29/06/2011
    ''' </remarks>
    Private Function ValidateErrorRequiredValues() As Boolean
        Dim ValidationError As Boolean = False

        Try
            bsResultErrorProv.Clear()
            If (bsNumberOfSeriesNumeric.Enabled AndAlso bsNumberOfSeriesNumeric.Text = String.Empty) Then
                bsResultErrorProv.SetError(bsNumberOfSeriesNumeric, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ValidationError = True
            End If

            If (bsRejectionNumeric.Text = String.Empty) Then
                bsResultErrorProv.SetError(bsRejectionNumeric, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ValidationError = True
            End If

            If (bsMultirulesApplication1Combo.Enabled AndAlso bsMultirulesApplication1Combo.SelectedIndex = -1) Then
                bsResultErrorProv.SetError(bsMultirulesApplication1Combo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                bsResultErrorProv.SetIconAlignment(bsMultirulesApplication1Combo, ErrorIconAlignment.MiddleLeft)
                ValidationError = True
            End If

            If (bsMultirulesApplication2Combo.Enabled AndAlso bsMultirulesApplication2Combo.SelectedIndex = -1) Then
                bsResultErrorProv.SetError(bsMultirulesApplication2Combo, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                ValidationError = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateErrorRequiredValues ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateErrorRequiredValues ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return ValidationError
    End Function

    ''' <summary>
    ''' Method that verifies if changes have been made
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 06/07/2011
    ''' </remarks>
    Private Sub ValuesChanges()
        Try
            If (Not LoadingData) Then
                If (Not ChangeMade) Then
                    ChangeMade = True

                    'Clean all grids
                    bsResultControlLotGridView.DataSource = Nothing
                    bsResultsDetailsGridView.DataSource = Nothing

                    'Disable all buttons in Result Details area
                    bsAddButtom.Enabled = False
                    bsEditButtom.Enabled = False
                    bsDeleteButtom.Enabled = False
                    bsGraphsButton.Enabled = False
                End If
            Else
                ChangeMade = False
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValuesChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValuesChanges ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Events"
    '*******************
    '** SCREEN EVENTS **
    '*******************
    Private Sub IQCResultsReview_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (Not bsRejectionNumeric.Focused AndAlso Not bsNumberOfSeriesNumeric.Focused) Then
                    bsExitButton.PerformClick()

                ElseIf (bsRejectionNumeric.Focused AndAlso bsRejectionNumeric.Text = String.Empty) Then
                    bsRejectionNumeric.Text = bsRejectionNumeric.Value.ToString()
                    bsResultErrorProv.Clear()

                ElseIf (bsNumberOfSeriesNumeric.Focused AndAlso bsNumberOfSeriesNumeric.Text = String.Empty) Then
                    bsNumberOfSeriesNumeric.Text = bsNumberOfSeriesNumeric.Value.ToString()
                    bsResultErrorProv.Clear()
                Else
                    bsExitButton.PerformClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IQCResultsReview_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IQCResultsReview_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load screen event
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: SA 13/11/2014 - BA-1885 ==> Code for the event Shown has been moved to this event to avoid show the screen while it is 
    '''                                          still loading (the time needed for loading can be larger now due to until 300 results by Control/Lot
    '''                                          are allowed). Additionally, new function LoadScreen has been created and the final code has been
    '''                                          moved to it. 
    ''' </remarks>
    Private Sub QCResultsReview_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            LoadScreen(True)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".QCResultsReview_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QCResultsReview_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*******************
    '** BUTTON EVENTS **
    '*******************
    Private Sub AddButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAddButtom.Click
        Try
            If (bsTestSampleListView.Items.Count > 0 AndAlso bsTestSampleListView.SelectedItems.Count = 1) Then
                AddResultValues(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text))
                ChangeMade = False
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AddButtom_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AddButtom_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub EditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsEditButtom.Click
        Try
            EditResultValues()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EditButtom_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".EditButtom_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub DeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDeleteButtom.Click
        Try
            DeleteSelecteQCResultValues()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteButtom_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteButtom_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CumulateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCumulateButton.Click
        Try
            Dim myCumulateForm As New IQCCumulateControlResults
            myCumulateForm.MdiParent = Me.MdiParent
            myCumulateForm.AnalyzerID = AnalyzerIDAttribute
            myCumulateForm.IQCResultReviewGone = True
            myCumulateForm.Show()
            Application.DoEvents()


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CumulateButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CumulateButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            If (Not Tag Is Nothing) Then
                'A PerformClick() method was executed
                Close()
            Else
                'Normal button click - Open the WS Monitor form and close this one
                IAx00MainMDI.OpenMonitorForm(Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' For the selected Test/SampleType, load QC data applying the specified criteria
    ''' </summary>
    ''' <remarks>
    ''' Created by:
    ''' Modified by: SA 13/11/2014 - BA-1885 ==> Disable the screen and show the hourglass cursor while details areas are beign loaded
    '''                                          Code moved to a new method ApplySearchCriteria
    ''' </remarks>
    Private Sub SearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSearchButton.Click
        Try
            ApplySearchCriteria()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SearchButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SearchButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub GraphsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsGraphsButton.Click
        Try
            Using myIQCGraph As New IQCGraphs
                myIQCGraph.DateTo = bsDateToDateTimePick.Value
                myIQCGraph.DateFrom = bsDateFromDateTimePick.Value
                myIQCGraph.TestSampleID = CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text)
                myIQCGraph.QCResultsByControlDS = LocalQCResultsDS
                myIQCGraph.LocalOpenQCResultsDS = LocalOpenQCResultsDS
                myIQCGraph.DecimalAllowed = LocalDecimalAllow
                myIQCGraph.TestName = bsTestSampleListView.SelectedItems(0).SubItems(0).Text
                myIQCGraph.SampleType = bsTestSampleListView.SelectedItems(0).SubItems(1).Text
                myIQCGraph.RejectionCriteria = Me.bsRejectionNumeric.Value.ToString()

                myIQCGraph.ShowDialog()

                bsResultControlLotGridView.Refresh()
                FilterQCResultsBySelectedControls(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text))
            End Using
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GraphsButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GraphsButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            If (bsTestSampleListView.SelectedItems.Count = 1) AndAlso (Not bsResultControlLotGridView.DataSource Is Nothing) AndAlso _
               (Not bsResultsDetailsGridView.DataSource Is Nothing) Then
                Dim testSampleId As Integer = CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text)
                Dim decAllow As Integer = CInt(bsTestSampleListView.SelectedItems(0).SubItems(4).Text)

                Cursor = Cursors.WaitCursor
                XRManager.ShowQCIndividualResultsByTestReport(testSampleId, bsDateFromDateTimePick.Value, bsDateToDateTimePick.Value, _
                                                              LocalOpenQCResultsDS, LocalQCResultsDS, decAllow, REPORT_QC_GRAPH_TYPE.NO_GRAPH)
                Cursor = Cursors.Default
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsPrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsPrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*******************************************
    '** EVENTS FOR TEST/SAMPLE TYPES LISTVIEW **
    '*******************************************
    Private Sub TestSampleListView_ColumnWidthChanging(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles bsTestSampleListView.ColumnWidthChanging
        Try
            e.Cancel = True
            If (e.ColumnIndex = 0) Then
                e.NewWidth = 168
            ElseIf (e.ColumnIndex = 1) Then
                e.NewWidth = 50
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".TestSampleListView_ColumnWidthChanging ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".TestSampleListView_ColumnWidthChanging ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' When a new Test/SampleType is selected in the ListView, load its QC data
    ''' </summary>
    ''' <remarks>
    ''' Created by:
    ''' Modified by: SA 13/11/2014 - BA-1885 ==> Disable the screen and show the hourglass cursor while details areas are beign loaded
    '''                                          Code moved to a new method LoadDataOfSelectedTestSample
    ''' </remarks>
    Private Sub TestSampleListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTestSampleListView.SelectedIndexChanged
        Try
            LoadDataOfSelectedTestSample()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".TestSampleListView_SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".TestSampleListView_SelectedIndexChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '****************************************************
    '** EVENTS FOR FIELDS IN CALCULATION CRITERIA AREA **
    '****************************************************
    Private Sub DateFromDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDateFromDateTimePick.ValueChanged, _
                                                                                                                      bsDateToDateTimePick.ValueChanged
        Try
            If (DirectCast(sender, BSDateTimePicker).Name = bsDateFromDateTimePick.Name) Then
                bsDateToDateTimePick.MaxDate = DateTime.FromBinary(946707264000000000)
                bsDateToDateTimePick.MinDate = bsDateFromDateTimePick.Value
            End If
            ValuesChanges()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DateFromDateTimePick_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DateFromDateTimePick_ValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub MultirulesApplicationCombo_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsMultirulesApplication1Combo.SelectedValueChanged, _
                                                                                                                                    bsMultirulesApplication2Combo.SelectedValueChanged
        Try
            If (Not bsMultirulesApplication1Combo.SelectedValue Is Nothing AndAlso Not bsMultirulesApplication2Combo.SelectedValue Is Nothing) Then
                IsMultirulesApplicationEqual(DirectCast(sender, BSComboBox).Name)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MultirulesApplicationCombo_SelectedValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MultirulesApplicationCombo_SelectedValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub Controls_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNumberOfSeriesNumeric.ValueChanged, _
                                                                                                          bsRejectionNumeric.ValueChanged, _
                                                                                                          bsCalculationModeCombo.TextChanged, _
                                                                                                          bs10XmCheckBox.CheckedChanged, _
                                                                                                          bs22SCheckBox.CheckedChanged, _
                                                                                                          bs41SCheckBox.CheckedChanged, _
                                                                                                          bs13SCheckBox.CheckedChanged, _
                                                                                                          bs12SCheckBox.CheckedChanged, _
                                                                                                          bsR4SCheckBox.CheckedChanged, _
                                                                                                          bsMultirulesApplication1Combo.SelectedIndexChanged, _
                                                                                                          bsMultirulesApplication2Combo.SelectedIndexChanged
        Try
            ValuesChanges()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".Controls_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".Controls_ValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CalculationModeCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCalculationModeCombo.SelectedIndexChanged
        Try
            If (Not bsCalculationModeCombo.SelectedValue Is Nothing) Then
                If (bsCalculationModeCombo.SelectedValue.ToString() = "MANUAL") Then
                    bsNumberOfSeriesNumeric.Minimum = 0
                    bsNumberOfSeriesNumeric.Maximum = 1
                    bsNumberOfSeriesNumeric.ResetText()
                    bsNumberOfSeriesNumeric.Enabled = False
                ElseIf (bsCalculationModeCombo.SelectedValue.ToString() = "STATISTIC") Then
                    bsNumberOfSeriesNumeric.Enabled = True
                    SetQCLimits()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CalculationModeCombo_SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CalculationModeCombo_SelectedIndexChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub NumberOfSeriesNumeric_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles bsNumberOfSeriesNumeric.Validating
        Try
            bsResultErrorProv.Clear()
            If (bsNumberOfSeriesNumeric.Enabled AndAlso bsNumberOfSeriesNumeric.Text = String.Empty) Then
                bsResultErrorProv.SetError(bsNumberOfSeriesNumeric, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                bsNumberOfSeriesNumeric.Focus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".NumberOfSeriesNumeric_Validating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".NumberOfSeriesNumeric_Validating ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub RejectionNumeric_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles bsRejectionNumeric.Validating
        Try
            bsResultErrorProv.Clear()
            If (bsRejectionNumeric.Text = String.Empty) Then
                bsResultErrorProv.SetError(bsRejectionNumeric, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString))
                bsRejectionNumeric.Focus()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RejectionNumeric_Validating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RejectionNumeric_Validating ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '****************************************************
    '** EVENTS FOR RESULTS BY CONTROL/LOT DATAGRIDVIEW **
    '****************************************************
    Private Sub ResultControlLotGridView_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) _
                                                        Handles bsResultControlLotGridView.CellFormatting
        Try
            If (Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing) Then
                If (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "Mean") OrElse _
                   (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "CalcMean") Then
                    'Assigned Mean / Calculated Mean are shown with the number of decimals programmed for the Test
                    e.Value = DirectCast(e.Value, Double).ToString("F" & LocalDecimalAllow.ToString())

                ElseIf (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "SD") OrElse _
                       (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "CalcSD") Then
                    'Assigned SD / Calculated SD are shown with the number of decimals programmed for the Test plus one
                    e.Value = DirectCast(e.Value, Double).ToString("F" & (LocalDecimalAllow + 1).ToString())

                ElseIf (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "CV") OrElse _
                       (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "CalcCV") Then
                    'Assigned CV / Assigned CV are shown with two decimals
                    e.Value = DirectCast(e.Value, Double).ToString("F2")
                End If
            ElseIf (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "Dummy") Then
                e.CellStyle.BackColor = SystemColors.MenuBar
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultControlLotGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResultControlLotGridView_CellFormatting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ResultControlLotGridView_CellValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
                                                          Handles bsResultControlLotGridView.CellValueChanged
        Try
            If (bsResultControlLotGridView.CurrentRow.Index >= 0 AndAlso Me.bsResultControlLotGridView.CurrentCell.ColumnIndex = 0) Then
                If bsTestSampleListView.SelectedItems.Count > 0 Then
                    PrevSelectedControlName.Clear()
                    FilterQCResultsBySelectedControls(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text))
                End If

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultControlLotGridView_CellValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResultControlLotGridView_CellValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ResultControlLotGridView_CurrentCellDirtyStateChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                                      Handles bsResultControlLotGridView.CurrentCellDirtyStateChanged
        Try
            ChangeControlLotSelectedState()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultControlLotGridView_CurrentCellDirtyStateChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResultControlLotGridView_CurrentCellDirtyStateChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*********************************************
    '** EVENTS FOR RESULTS DETAILS DATAGRIDVIEW **
    '*********************************************
    Private Sub ResultsDetailsGridView_CellDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
                                                       Handles bsResultsDetailsGridView.CellDoubleClick
        Try
            If (e.RowIndex >= 0 AndAlso bsResultsDetailsGridView.SelectedRows.Count = 1) Then
                If (bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "EditResultIcon") Then
                    EditResultValues()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultsDetailsGridView_CellDoubleClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResultsDetailsGridView_CellDoubleClick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ResultsDetailsGridView_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) _
                                                      Handles bsResultsDetailsGridView.CellFormatting
        Try
            If (bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "EditResultIcon" AndAlso e.Value IsNot Nothing) Then
                Dim file As String = e.Value.ToString()

                If (System.IO.File.Exists(file)) Then
                    e.Value = Image.FromFile(file)
                Else
                    e.Value = Nothing
                End If
            End If

            If (bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "ResultValue" OrElse bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "ABSError") Then
                If (Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing) Then
                    e.Value = CDbl(e.Value).ToString("F" & LocalDecimalAllow.ToString())

                    If (bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "ResultValue") Then
                        'Validate if there are Alarms to change the ForeColor of the row to red
                        If (bsResultsDetailsGridView.Rows(e.RowIndex).Cells("AlarmsList").Value.ToString() = String.Empty) Then
                            e.CellStyle.ForeColor = Color.Black
                        Else
                            bsResultsDetailsGridView.Rows(e.RowIndex).DefaultCellStyle.ForeColor = Color.Red
                        End If

                        'Validate if result is excluded to change the BackColor to gray and the FontStyle to Strikeout for all the row
                        If (Not bsResultsDetailsGridView.Rows(e.RowIndex).Cells("Excluded").Value Is DBNull.Value AndAlso _
                            DirectCast(bsResultsDetailsGridView.Rows(e.RowIndex).Cells("Excluded").Value, Boolean)) Then

                            Dim f As Font = bsResultsDetailsGridView.DefaultCellStyle.Font
                            e.CellStyle.Font = New Font(f, FontStyle.Strikeout)
                            e.CellStyle.ForeColor = Color.Gray

                            bsResultsDetailsGridView.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.LightGray
                            bsResultsDetailsGridView.Rows(e.RowIndex).DefaultCellStyle.ForeColor = Color.Gray
                            bsResultsDetailsGridView.Rows(e.RowIndex).DefaultCellStyle.Font = New Font(f, FontStyle.Strikeout)
                        End If
                    End If
                End If

            ElseIf (bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "RELError" OrElse bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "RELErrorPercent") Then
                e.Value = CDbl(e.Value).ToString("F2")

            ElseIf (bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "ResultDate") Then
                e.Value = DirectCast(e.Value, DateTime).ToString(SystemInfoManager.OSDateFormat).ToString() & " " & _
                          DirectCast(e.Value, DateTime).ToString(SystemInfoManager.OSLongTimeFormat).ToString()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultsDetailsGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResultsDetailsGridView_CellFormatting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ResultsDetailsGridView_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsResultsDetailsGridView.KeyDown
        Try
            If (e.KeyCode = Keys.Delete) Then DeleteSelecteQCResultValues()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultsDetailsGridView_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResultsDetailsGridView_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub ResultsDetailsGridView_RowEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
                                                Handles bsResultsDetailsGridView.RowEnter
        Try
            If (bsResultsDetailsGridView.SelectedRows.Count > 1) Then
                bsAddButtom.Enabled = False
                bsEditButtom.Enabled = False
            Else
                bsAddButtom.Enabled = True
                bsEditButtom.Enabled = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultsDetailsGridView_RowEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResultsDetailsGridView_RowEnter ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region
End Class


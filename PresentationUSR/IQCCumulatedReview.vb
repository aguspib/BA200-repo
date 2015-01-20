Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.PresentationCOM
Imports DevExpress.XtraCharts

Public Class IQCCumulatedReview

#Region "Declarations"
    Private currentLanguage As String = ""

    Private ChangeMade As Boolean = False
    Private LocalDecimalsAllowed As Integer

    Private LocalQCCumulateResultsDS As New QCCumulatedSummaryDS
    Private LocalCumulateResultsDS As New CumulatedResultsDS

    Private LocalPoint As Point
    Private PrevSelectedControlName As New List(Of String)

    Private LabelSD As String = ""
    Private LabelMEAN As String = ""
#End Region

#Region "Attributes"
    Private AnalyzerIDAttribute As String = String.Empty
#End Region

#Region "Properties"
    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property
#End Region

#Region "Methods"

    ''' <summary>
    ''' Load a constant line into the graphic
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 28/06/2011
    ''' </remarks>
    Private Sub CreateConstantLine(ByVal pName As String, ByVal pDiagram As XYDiagram, ByVal pValue As Single, ByVal pColor As Color, _
                                   ByVal pDashStyle As DashStyle)
        Try
            'Create a constant line
            Dim constantLine As New ConstantLine(pName)
            pDiagram.AxisY.ConstantLines.Add(constantLine)

            'Define its axis value
            constantLine.AxisValue = pValue

            'Customize the behavior of the constant line
            constantLine.Visible = True
            constantLine.ShowInLegend = True
            constantLine.LegendText = pName
            constantLine.ShowBehind = False

            'Customize the constant line's title
            constantLine.Title.Visible = True
            constantLine.Title.Text = pName
            constantLine.Title.TextColor = pColor
            constantLine.Title.Antialiasing = False
            constantLine.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
            constantLine.Title.ShowBelowLine = True
            constantLine.Title.Alignment = ConstantLineTitleAlignment.Far

            'Customize the appearance of the constant line.
            constantLine.Color = pColor
            constantLine.LineStyle.DashStyle = pDashStyle
            constantLine.LineStyle.Thickness = 1
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CreateConstantLine", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CreateConstantLine", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the point value contains an error or a Warning and change the icon 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' </remarks>
    Private Sub CustomDrawSeriesPoint(ByVal sender As Object, ByVal e As DevExpress.XtraCharts.CustomDrawSeriesPointEventArgs)
        Try
            Dim mySerieName As String = e.Series.Name

            CType(e.SeriesDrawOptions, PointDrawOptions).Marker.FillStyle.FillMode = FillMode.Solid
            CType(e.SeriesDrawOptions, PointDrawOptions).Marker.Kind = MarkerKind.Cross
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CustomDrawSeriesPoint ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CustomDrawSeriesPoint ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete selected Cumulated Series
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 
    ''' Modified by: SA 06/06/2012 - Inform also field AnalyzerID in the CumulatedResultsDS containing all cumulated series to delete
    ''' </remarks>
    Private Sub DeleteSelectedResult()
        Try
            If (bsResultsDetailsGridView.SelectedRows.Count > 0) Then
                If (ShowMessage("Warning", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                    Dim myCumulateResultsDS As New CumulatedResultsDS
                    Dim myCumulateResultsRow As CumulatedResultsDS.tqcCumulatedResultsRow

                    For Each SelQCResult As DataGridViewRow In bsResultsDetailsGridView.SelectedRows
                        myCumulateResultsRow = CType(myCumulateResultsDS.tqcCumulatedResults.NewRow, CumulatedResultsDS.tqcCumulatedResultsRow)

                        myCumulateResultsRow.QCTestSampleID = CInt(SelQCResult.Cells("QCTestSampleID").Value)
                        myCumulateResultsRow.QCControlLotID = CInt(SelQCResult.Cells("QCControlLotID").Value)
                        myCumulateResultsRow.AnalyzerID = AnalyzerIDAttribute
                        myCumulateResultsRow.CumResultsNum = CInt(SelQCResult.Cells("CumResultsNum").Value)
                        myCumulateResultsRow.XMLFileName = SelQCResult.Cells("XMLFileName").Value.ToString()

                        myCumulateResultsDS.tqcCumulatedResults.AddtqcCumulatedResultsRow(myCumulateResultsRow)
                    Next

                    Dim myGlobalDataTO As New GlobalDataTO
                    Dim myCumulateResultsDelegate As New CumulatedResultsDelegate

                    'myGlobalDataTO = myCumulateResultsDelegate.Delete(Nothing, myCumulateResultsDS)
                    myGlobalDataTO = myCumulateResultsDelegate.DeleteNEW(Nothing, myCumulateResultsDS)
                    If (Not myGlobalDataTO.HasError) Then
                        GetControlsLotsWithCumulatedSeries(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text), False)
                    Else
                        'Error deleting the Cumulated Series; shown it
                        ShowMessage(Name & ".DeleteSelectedResult", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteSelectedResult ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteSelectedResult ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the Tests/Sample Types ListView with all the Tests/Sample Types from tqcHistoryTestSample table
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 22/06/2011 
    ''' Modified by: SA 03/01/2012 - Added a column containing the status (deleted or not) of each Test/SampleType in the ListView.
    '''                              Show a different Icon for deleted Test/SampleTypes  
    '''              SA 06/06/2012 - Sort Tests/SampleTypes by TestType DESC/PreloadedTest DESC/TestName; in the ListView, shown the 
    '''                              proper icon according the TestType, and add the TestType to the ListView
    ''' </remarks>
    Private Sub FillTestSampleListView()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQCHistoryTestSampleDelegate As New HistoryTestSamplesDelegate

            'Get all Tests/Sample Types in tqcHistoryTestSample table
            myGlobalDataTO = myQCHistoryTestSampleDelegate.ReadAllNEW(Nothing, AnalyzerIDAttribute, False)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myQCHistoryTestSampleDS As HistoryTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)

                'Sort data to shown first STD Biosystems Tests, STD User Tests and finally ISE Tests
                Dim myQCHistoryTestSampleList As List(Of HistoryTestSamplesDS.tqcHistoryTestSamplesRow) = (From a In myQCHistoryTestSampleDS.tqcHistoryTestSamples _
                                                                                                       Order By a.TestType Descending, a.PreloadedTest Descending, a.TestName).ToList()

                Dim myIndex As Integer = 0
                Dim myIconNameVar As String = ""
                Dim activeTestSample As Boolean = True
                For Each historyTestSampRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow In myQCHistoryTestSampleList
                    'Verify if the Test is active (Test or Test/SampleType are marked as deleted)
                    activeTestSample = (Not historyTestSampRow.DeletedSampleType) AndAlso (Not historyTestSampRow.DeletedTest)

                    'Select the Test Icon depending of TestType and also depending if it is a Preloaded Test or a Used-defined one 
                    'and if it is active or has been deleted
                    If (historyTestSampRow.TestType = "STD") Then
                        If (historyTestSampRow.PreloadedTest) Then
                            If (activeTestSample) Then
                                myIconNameVar = "TESTICON"
                            Else
                                myIconNameVar = "INUSETEST"
                            End If
                        Else
                            If (activeTestSample) Then
                                myIconNameVar = "USERTEST"
                            Else
                                myIconNameVar = "INUSUSTEST"
                            End If
                        End If

                    ElseIf (historyTestSampRow.TestType = "ISE") Then
                        If (activeTestSample) Then
                            myIconNameVar = "TISE_SYS"
                        Else
                            myIconNameVar = "INUSETISE"
                        End If
                    End If

                    'Add the Test/Sample Type to the ListView
                    bsTestSampleListView.Items.Add(historyTestSampRow.TestName, myIconNameVar).SubItems.Add(historyTestSampRow.SampleType)
                    bsTestSampleListView.Items(myIndex).SubItems.Add(historyTestSampRow.QCTestSampleID.ToString)
                    bsTestSampleListView.Items(myIndex).SubItems.Add(historyTestSampRow.TestID.ToString)
                    bsTestSampleListView.Items(myIndex).SubItems.Add(historyTestSampRow.DecimalsAllowed.ToString)
                    bsTestSampleListView.Items(myIndex).SubItems.Add(activeTestSample.ToString)
                    bsTestSampleListView.Items(myIndex).SubItems.Add(historyTestSampRow.TestType)

                    myIndex += 1
                Next
            Else
                'Error getting all Tests/Sample Types in tqcHistoryTestSample table; shown it
                ShowMessage(Name & ".FillTestSampleListView", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillTestSampleListView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillTestSampleListView ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Filter the cumulated series results by the selected Control/Lot on the ResultControlLot DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 28/06/2011
    ''' Modified by: TR 24/04/2012 - Added call to function ScreenStatusByUserLevel
    ''' </remarks>
    Private Sub FilterCumulatedSeries(ByVal pQCTestSampleID As Integer)
        Try
            Dim myQCCumulateResultsList As New List(Of CumulatedResultsDS.tqcCumulatedResultsRow)

            If (PrevSelectedControlName.Count > 0) Then
                For Each qcCumResultRow As QCCumulatedSummaryDS.QCCumulatedSummaryTableRow In LocalQCCumulateResultsDS.QCCumulatedSummaryTable.Rows
                    If ((From a In PrevSelectedControlName Where a = qcCumResultRow.ControlName Select a).Count > 0) AndAlso _
                       ((From a In PrevSelectedControlName Where a = qcCumResultRow.ControlName Select a).Count < 4) Then
                        qcCumResultRow.Selected = True
                    Else
                        qcCumResultRow.Selected = False
                    End If
                Next
            Else
                PrevSelectedControlName = (From a In LocalQCCumulateResultsDS.QCCumulatedSummaryTable _
                                      Where Not a.IsSelectedNull AndAlso a.Selected _
                                         Select a.ControlName).ToList()
            End If

            'Make a join between DS LocalCumulateResultsDS to show only results of the selected Control/Lot on LocalQCCumulateResultsDS
            myQCCumulateResultsList = (From a In LocalCumulateResultsDS.tqcCumulatedResults _
                                       Join b In LocalQCCumulateResultsDS.QCCumulatedSummaryTable On a.QCControlLotID Equals b.QCControlLotID _
                                  Where Not b.IsSelectedNull _
                                    AndAlso b.Selected _
                                    AndAlso a.QCTestSampleID = pQCTestSampleID _
                                     Select a _
                                   Order By a.CumResultsNum Descending, a.ControlName, a.LotNumber).ToList()

            'Set as datasource for Results DataGridView
            bsResultsDetailsGridView.DataSource = myQCCumulateResultsList

            'Load graphics 
            LoadMeanGraphic()

            'Enable/disable Delete button 
            If (myQCCumulateResultsList.Count > 0) Then
                bsDeleteCumulateSeries.Enabled = True
            Else
                bsDeleteCumulateSeries.Enabled = False
            End If
            ScreenStatusByUserLevel()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FilterCumulatedSeries ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FilterCumulatedSeries ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get all the Controls/Lots with Cumulated Series saved for the specified Test/SampleType and bind data to the 
    ''' correspondent screen fields
    ''' </summary>
    ''' <param name="pQCTestSampleID">Identifier of the Test/SampleType in QC Module</param>
    ''' <remarks>
    ''' Created by:  TR 22/06/2011
    ''' Modified by: SA 03/01/2012 - If the selected Test/SampleType is still active (column Save as Target is visible), verify for which
    '''                              linked Control/Lots the button in the correspondent cell can be enabled. Button is enabled when following
    '''                              conditions are fulfilled (both of them):
    '''                                ** The Control/Lot is still active 
    '''                                ** The link between the Test/Sample and the Control is still active (it exists in table tparTestControls)
    '''                                ** There are at least two cumulated series saved for the Control/Lot (cumulated values have been calculated)   
    '''              TR 20/04/2012 - The target button on the grid will be disabled if the level of the connected User is Operator
    '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub GetControlsLotsWithCumulatedSeries(ByVal pQCTestSampleID As Integer, Optional ByVal pSearchButton As Boolean = False)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myCumulateResultsDelegate As New CumulatedResultsDelegate

            'Before getting all the cumulated series, get the date and time of the oldest Cumulated Serie for the Test/SampleType
            myGlobalDataTO = myCumulateResultsDelegate.GetMinCumDateTimeNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute)
            If (Not myGlobalDataTO.HasError) Then
                If (Not pSearchButton AndAlso Not myGlobalDataTO.SetDatos Is DBNull.Value) Then
                    bsDateFromDateTimePick.MinDate = DirectCast(myGlobalDataTO.SetDatos, DateTime)
                    bsDateFromDateTimePick.Value = DirectCast(myGlobalDataTO.SetDatos, DateTime)
                End If

                'TR 02/07/2012 - Set the maximum value for DateTo DateTimePicker
                myGlobalDataTO = myCumulateResultsDelegate.GetMaxCumDateTime(Nothing, pQCTestSampleID, AnalyzerIDAttribute)
                If (Not myGlobalDataTO.HasError) Then
                    If (Not pSearchButton AndAlso Not myGlobalDataTO.SetDatos Is DBNull.Value) Then
                        bsDateToDateTimePick.MaxDate = DirectCast(myGlobalDataTO.SetDatos, DateTime)
                        bsDateToDateTimePick.Value = DirectCast(myGlobalDataTO.SetDatos, DateTime)
                    End If
                End If

                'Set the min and max values to date time controls.
                bsDateFromDateTimePick.MaxDate = bsDateToDateTimePick.Value
                bsDateToDateTimePick.MinDate = bsDateFromDateTimePick.Value

                'Get all Cumulated Series for all Controls/Lots linked to the selected Test/SampleType in the specified range of dates
                myGlobalDataTO = myCumulateResultsDelegate.GetCumulatedSeriesNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute, _
                                                                                 bsDateFromDateTimePick.Value, bsDateToDateTimePick.Value)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    LocalCumulateResultsDS = DirectCast(myGlobalDataTO.SetDatos, CumulatedResultsDS)

                    'Set value of field Range and bind the DS to the grid of Cumulated Series Details
                    For Each myCumRow As CumulatedResultsDS.tqcCumulatedResultsRow In LocalCumulateResultsDS.tqcCumulatedResults.Rows
                        If (myCumRow.MinRange < 0) Then myCumRow.MinRange = 0
                        If (myCumRow.MaxRange < 0) Then myCumRow.MaxRange = 0

                        myCumRow.Range = myCumRow.MinRange.ToString("F" & LocalDecimalsAllowed.ToString()) & " - " & _
                                         myCumRow.MaxRange.ToString("F" & LocalDecimalsAllowed.ToString())
                    Next

                    'Get statistics values for all Controls/Lots having Cumulated Series for the selected Test/SampleType in the specified range of dates
                    myGlobalDataTO = myCumulateResultsDelegate.GetControlsLotsWithCumulatedSeriesNEW(Nothing, pQCTestSampleID, AnalyzerIDAttribute, _
                                                                                                     bsDateFromDateTimePick.Value, bsDateToDateTimePick.Value, _
                                                                                                     LocalCumulateResultsDS)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        LocalQCCumulateResultsDS = DirectCast(myGlobalDataTO.SetDatos, QCCumulatedSummaryDS)

                        If (LocalQCCumulateResultsDS.QCCumulatedSummaryTable.Count > 0) Then
                            'Set value of fields DateRange and Range, and bind the DS to the grid of Cumulated Results by Control/Lot
                            For Each myQCCumSumRow As QCCumulatedSummaryDS.QCCumulatedSummaryTableRow In LocalQCCumulateResultsDS.QCCumulatedSummaryTable.Rows
                                If (Not myQCCumSumRow.IsMinDateNull AndAlso Not myQCCumSumRow.IsMaxDateNull) Then
                                    'Set the dates Range between MinDate and MaxDate.
                                    myQCCumSumRow.DatesRange = myQCCumSumRow.MinDate.Date.ToShortDateString() & " - " & _
                                                               myQCCumSumRow.MaxDate.Date.ToShortDateString()
                                End If

                                'Set the Range betwen MinRange and Max Range
                                If (Not myQCCumSumRow.IsMinRangeNull AndAlso Not myQCCumSumRow.IsMaxRangeNull) Then
                                    If (myQCCumSumRow.MinRange < 0) Then myQCCumSumRow.MinRange = 0
                                    If (myQCCumSumRow.MaxRange < 0) Then myQCCumSumRow.MaxRange = 0

                                    myQCCumSumRow.Range = myQCCumSumRow.MinRange.ToString("F" & LocalDecimalsAllowed.ToString()) & " - " & _
                                                          myQCCumSumRow.MaxRange.ToString("F" & LocalDecimalsAllowed.ToString())
                                End If
                            Next
                            LocalQCCumulateResultsDS.QCCumulatedSummaryTable.DefaultView.Sort = "ControlName"
                            bsResultControlLotGridView.DataSource = LocalQCCumulateResultsDS.QCCumulatedSummaryTable.DefaultView

                            'Get the current Language from the current Application Session and get the text for the Button in the DataGridView
                            'Dim currentLanguageGlobal As New GlobalBase
                            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

                            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString()
                            Dim myButtonText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CumReview_SaveAsTarget", currentLanguage)

                            'For each Control/Lot, verify if Button for Save values as Target has to be enabled or disabled
                            Dim myButtonDisabled As Boolean = True
                            Dim myTestControlsDelegate As New TestControlsDelegate

                            Dim buttonCell As DataGridViewDisableButtonCell
                            For i As Integer = 0 To bsResultControlLotGridView.Rows.Count - 1
                                myButtonDisabled = Convert.ToBoolean(bsResultControlLotGridView.Rows(i).Cells("DeletedControlLot").Value) OrElse _
                                                   bsResultControlLotGridView.Rows(i).Cells("Ranges").Value Is Nothing OrElse _
                                                   bsResultControlLotGridView.Rows(i).Cells("Ranges").Value Is DBNull.Value

                                If (Not myButtonDisabled) Then
                                    'Verify if the link between the Test/SampleType and the Control still exists in the application (table tparTestControls)
                                    myGlobalDataTO = myTestControlsDelegate.VerifyLinkByQCModuleIDsNEW(Nothing, pQCTestSampleID, _
                                                                                                       Convert.ToInt32(bsResultControlLotGridView.Rows(i).Cells("QCControlLotID").Value))
                                    If (myGlobalDataTO.HasError) Then Exit For

                                    If (CurrentUserLevel <> "OPERATOR") Then
                                        myButtonDisabled = Not DirectCast(myGlobalDataTO.SetDatos, Boolean)
                                    Else
                                        myButtonDisabled = True
                                    End If
                                End If

                                buttonCell = CType(bsResultControlLotGridView.Rows(i).Cells("SaveAsTarget"), DataGridViewDisableButtonCell)
                                buttonCell.Enabled = (Not myButtonDisabled)

                                buttonCell.Value = "Xm"
                                buttonCell.ToolTipText = myButtonText
                            Next

                            If (Not myGlobalDataTO.HasError) Then
                                'After getting the Cumulated Results then filter by selected elements
                                FilterCumulatedSeries(pQCTestSampleID)
                            Else
                                'Error verifying if link between the selected Test/SampleType and the Controls already exists in tparTestControls; shown it
                                ShowMessage(Name & ".GetControlsLotsWithCumulatedSeries ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                            End If
                        Else
                            bsResultControlLotGridView.DataSource = Nothing
                            bsResultsDetailsGridView.DataSource = Nothing

                            'Clear all the graphs
                            GetLegendsLabels(currentLanguage)
                            bsMeanChartControl.Series.Clear()
                            bsMeanChartControl.ClearCache()

                            'Show Message: not Cumulated Series for the selected Test/Sample Type
                            ShowMessage(Name & ".GetControlsLotsWithCumulatedSeries ", GlobalEnumerates.Messages.NOCUMULATE_QCSERIESVALUE.ToString())
                        End If
                    Else
                        'Error getting the Cumulated Values for all Controls/Lots linked to the selected Test/Sample Type; shown it
                        ShowMessage(Name & ".GetControlsLotsWithCumulatedSeries ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                    End If
                Else
                    'Error getting  all Controls/Lots linked to the selected Test/SampleType; shown it
                    ShowMessage(Name & ".GetControlsLotsWithCumulatedSeries ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetControlsLotsWithCumulatedSeries ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetControlsLotsWithCumulatedSeries ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the Cumulated CV value for the ToolTip of a point in the graph
    ''' </summary>
    ''' <param name="pCumResultNumber"></param>
    ''' <param name="pControlLotNumber"></param>
    ''' <returns>The CV value</returns>
    ''' <remarks>
    ''' Created by:  TR
    ''' </remarks>
    Private Function GetCVValue(ByVal pCumResultNumber As Integer, ByVal pControlLotNumber As String) As Double
        Dim myCVValue As Double = 0
        Try
            If ((From a In LocalCumulateResultsDS.tqcCumulatedResults Join b In LocalQCCumulateResultsDS.QCCumulatedSummaryTable _
                   On a.QCControlLotID Equals b.QCControlLotID _
            Where Not b.IsSelectedNull AndAlso b.Selected _
              AndAlso a.VisibleCumResultNumber = pCumResultNumber _
              AndAlso a.LotNumber = pControlLotNumber _
               Select a.CV).Count > 0) Then

                'Get the CV 
                myCVValue = (From a In LocalCumulateResultsDS.tqcCumulatedResults Join b In LocalQCCumulateResultsDS.QCCumulatedSummaryTable _
                               On a.QCControlLotID Equals b.QCControlLotID _
                        Where Not b.IsSelectedNull AndAlso b.Selected _
                          AndAlso a.VisibleCumResultNumber = pCumResultNumber _
                          AndAlso a.LotNumber = pControlLotNumber _
                           Select a.CV).Max
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetCVValue ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetCVValue ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myCVValue
    End Function

    ''' <summary>
    ''' Get the Legend labels
    ''' </summary>
    ''' <param name="pLanguageID">Current application Language</param>
    ''' <remarks>
    ''' Created by:  TR 21/06/2011
    ''' </remarks>
    Private Sub GetLegendsLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsFirstCtrlLotLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirstCtrlLot", pLanguageID)
            bsFirstCtrlLotLabel.Enabled = False
            bsFirstCtrlLotPictureBox.Enabled = False

            bsSecondCtrlLotLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SecondCtrlLot", pLanguageID)
            bsSecondCtrlLotLabel.Enabled = False
            bsSecondCtrlLotPictureBox.Enabled = False

            bsThirdCtrlLotLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ThirdCtrlLot", pLanguageID)
            bsThirdCtrlLotLabel.Enabled = False
            bsThirdCtrlLotPictureBox.Enabled = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetLegendsLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetLegendsLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID">Current application language</param>
    ''' <remarks>
    ''' Created by: TR 22/06/2011
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsTestSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", pLanguageID)
            bsCumulatedResultsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_CumulatedResults", pLanguageID)
            bsDateFromLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date_From", pLanguageID) & ":"
            bsDateToLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date_To", pLanguageID) & ":"
            bsCumulatedSeriesLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_CumulatedSeries", pLanguageID)
            ValuesXtraTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Values", pLanguageID)
            GraphXtraTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Graph", pLanguageID)
            bsLegendGB.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Legend", pLanguageID)

            LabelSD = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", pLanguageID)
            LabelMEAN = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", pLanguageID)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize all elements on the screen 
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 22/06/2011
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString()

            GetScreenLabels(currentLanguage)
            GetLegendsLabels(currentLanguage)
            PrepareButtons(currentLanguage)

            PrepareTestListView(currentLanguage)
            PrepareResultControlLotGrid(currentLanguage)
            PrepareResultDetailsGrid(currentLanguage)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the Mean Graphic control
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 28/06/2011
    ''' Modified by: SA 02/05/2012 - Added labels for +/- SD and Mean below the dotted lines on the right side of the graphic for 
    '''                              both cases: one or more Control/Lots plotted
    ''' </remarks>
    Private Sub LoadMeanGraphic()
        Try
            GetLegendsLabels(currentLanguage)

            bsMeanChartControl.Series.Clear()
            bsMeanChartControl.ClearCache()
            bsMeanChartControl.Legend.Visible = False
            bsMeanChartControl.BackColor = Color.White
            bsMeanChartControl.AppearanceName = "Light"

            Dim myDiagram As New XYDiagram
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            For Each qcCumResultRow As QCCumulatedSummaryDS.QCCumulatedSummaryTableRow In LocalQCCumulateResultsDS.QCCumulatedSummaryTable.Rows
                If (Not qcCumResultRow.IsMeanNull AndAlso qcCumResultRow.Selected) Then
                    'Set up the series
                    bsMeanChartControl.Series.Add(qcCumResultRow.QCControlLotID.ToString(), ViewType.Line)
                    bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).ShowInLegend = False
                    bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).Label.Visible = False
                    bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).PointOptions.PointView = PointView.ArgumentAndValues

                    'Set Serie Color
                    If (bsMeanChartControl.Series.Count = 1) Then
                        bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).View.Color = Color.Green

                        bsFirstCtrlLotLabel.Text = qcCumResultRow.ControlName & Environment.NewLine
                        bsFirstCtrlLotLabel.Text &= qcCumResultRow.LotNumber
                        bsFirstCtrlLotLabel.Enabled = True

                        bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).Tag = qcCumResultRow.ControlName & Environment.NewLine &
                            qcCumResultRow.LotNumber.ToString

                        bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).LegendText = qcCumResultRow.LotNumber

                    ElseIf (bsMeanChartControl.Series.Count = 2) Then
                        bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).View.Color = Color.Blue

                        bsSecondCtrlLotLabel.Text = qcCumResultRow.ControlName & Environment.NewLine
                        bsSecondCtrlLotLabel.Text &= qcCumResultRow.LotNumber
                        bsSecondCtrlLotLabel.Enabled = True

                        bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).Tag = qcCumResultRow.ControlName & Environment.NewLine &
                            qcCumResultRow.LotNumber
                        bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).LegendText = qcCumResultRow.LotNumber

                    ElseIf (bsMeanChartControl.Series.Count = 3) Then
                        bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).View.Color = Color.DarkViolet

                        bsThirdCtrlLotLabel.Text = qcCumResultRow.ControlName & Environment.NewLine
                        bsThirdCtrlLotLabel.Text &= qcCumResultRow.LotNumber
                        bsThirdCtrlLotLabel.Enabled = True

                        bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).Tag = qcCumResultRow.ControlName & Environment.NewLine &
                            qcCumResultRow.LotNumber
                        bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).LegendText = qcCumResultRow.LotNumber
                    End If

                    If ((From a In LocalQCCumulateResultsDS.QCCumulatedSummaryTable _
                         Where Not a.IsSelectedNull AndAlso a.Selected Select a).Count = 1) Then
                        'Create points
                        For Each cumResultRow As CumulatedResultsDS.tqcCumulatedResultsRow In LocalCumulateResultsDS.tqcCumulatedResults.Rows
                            If (Not cumResultRow.IsMeanNull AndAlso cumResultRow.QCControlLotID = qcCumResultRow.QCControlLotID) Then
                                bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).Points.Add(New SeriesPoint(cumResultRow.VisibleCumResultNumber, cumResultRow.Mean))
                            End If
                        Next
                    Else
                        'Create points
                        For Each cumResultRow As CumulatedResultsDS.tqcCumulatedResultsRow In LocalCumulateResultsDS.tqcCumulatedResults.Rows
                            If (Not cumResultRow.IsMeanNull AndAlso cumResultRow.QCControlLotID = qcCumResultRow.QCControlLotID) Then
                                bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).Points.Add(New SeriesPoint(cumResultRow.VisibleCumResultNumber, _
                                                                                                                            (cumResultRow.Mean - qcCumResultRow.Mean) / qcCumResultRow.SD))
                                'Set the mean value into the Tag property
                                bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).Points( _
                                                        bsMeanChartControl.Series(qcCumResultRow.QCControlLotID.ToString()).Points.Count - 1).Tag = cumResultRow.Mean
                            End If
                        Next
                    End If

                ElseIf (qcCumResultRow.Selected) Then
                    If (qcCumResultRow.IsMeanNull) Then
                        bsMeanChartControl.Series.Clear()
                        bsMeanChartControl.ClearCache()

                        bsCumulatedXtraTab.SelectedTabPage = bsCumulatedXtraTab.TabPages(0)
                        Exit For
                    End If
                End If
            Next

            If (Not bsMeanChartControl.Diagram Is Nothing) Then
                myDiagram = CType(bsMeanChartControl.Diagram, XYDiagram)
                myDiagram.AxisY.GridLines.Visible = False
                myDiagram.AxisX.GridLines.Visible = True

                'Remove all Constant lines
                myDiagram.AxisY.ConstantLines.Clear()
                myDiagram.AxisX.ConstantLines.Clear()
                myDiagram.AxisX.Title.Visible = False
                myDiagram.AxisY.Title.Visible = False

                'Create constans lines depending the number of selected controls
                If ((From a In LocalQCCumulateResultsDS.QCCumulatedSummaryTable _
                     Where Not a.IsSelectedNull AndAlso a.Selected AndAlso _
                           Not a.IsMeanNull AndAlso Not a.IsCVNull Select a).Count = 1) Then
                    Dim myMean As Single = 0
                    Dim mySD As Single = 0

                    myMean = (From a In LocalQCCumulateResultsDS.QCCumulatedSummaryTable _
                         Where Not a.IsSelectedNull AndAlso a.Selected Select a).First().Mean

                    mySD = (From a In LocalQCCumulateResultsDS.QCCumulatedSummaryTable _
                       Where Not a.IsSelectedNull AndAlso a.Selected Select a).First().SD

                    'Draw contants lines 1,0,-1
                    CreateConstantLine(LabelMEAN, myDiagram, myMean, Color.Black, DashStyle.Solid)
                    CreateConstantLine("+1 " & LabelSD, myDiagram, myMean + (1 * mySD), Color.Black, DashStyle.Dash)
                    CreateConstantLine("-1 " & LabelSD, myDiagram, myMean - (1 * mySD), Color.Black, DashStyle.Dash)

                    'Set limits for Axis Y
                    myDiagram.AxisY.Range.SetMinMaxValues(myMean - (1 * mySD) - (myMean - (myMean - mySD)) / 2, _
                                                          myMean + (1 * mySD) + (myMean - (myMean - mySD)) / 2)
                Else
                    'More than one control selected
                    CreateConstantLine("+1 " & LabelSD, myDiagram, 1, Color.Black, DashStyle.Dash)
                    CreateConstantLine(LabelMEAN, myDiagram, 0, Color.Black, DashStyle.Solid)
                    CreateConstantLine("-1 " & LabelSD, myDiagram, -1, Color.Black, DashStyle.Dash)
                    myDiagram.AxisY.Range.SetMinMaxValues(-1.5, 1.5)

                    'Get the max value for X Axis
                    Dim MaxValue As Integer = (From a In LocalCumulateResultsDS.tqcCumulatedResults _
                                               Join b In LocalQCCumulateResultsDS.QCCumulatedSummaryTable On a.QCControlLotID Equals b.QCControlLotID _
                                          Where Not b.IsSelectedNull _
                                            AndAlso b.Selected _
                                             Select a.CumResultsNum).Max

                    myDiagram.AxisX.Range.SetMinMaxValues(0, MaxValue)
                End If
            End If

            RemoveHandler bsMeanChartControl.CustomDrawSeriesPoint, AddressOf CustomDrawSeriesPoint
            AddHandler bsMeanChartControl.CustomDrawSeriesPoint, AddressOf CustomDrawSeriesPoint

            RemoveHandler bsMeanChartControl.ObjectHotTracked, AddressOf ObjectHotTracked
            AddHandler bsMeanChartControl.ObjectHotTracked, AddressOf ObjectHotTracked
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadMeanGraphic", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadMeanGraphic", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the ToolTips for points in the graph
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' </remarks>
    Private Sub ObjectHotTracked(ByVal sender As Object, ByVal e As DevExpress.XtraCharts.HotTrackEventArgs)
        Try
            Dim myPoint As SeriesPoint = TryCast(e.AdditionalObject, SeriesPoint)

            If (Not myPoint Is Nothing) Then
                'Get the RunNumber
                Dim myArgumentValue As String = DirectCast(e.AdditionalObject, DevExpress.XtraCharts.SeriesPoint).Argument.ToString()
                Dim mySerie As Series = TryCast(e.Object, DevExpress.XtraCharts.Series)

                Dim myToolTip As String = ""
                myToolTip = mySerie.Tag.ToString() & Environment.NewLine
                myToolTip &= "N: "
                myToolTip &= myPoint.NumericalArgument.ToString("F0") & " "

                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                myToolTip &= Environment.NewLine
                myToolTip &= myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", currentLanguage)

                If (myPoint.Tag Is Nothing) Then
                    myToolTip &= ": " & myPoint.Values(0).ToString("F2")
                Else
                    myToolTip &= ": " & CDbl(myPoint.Tag).ToString("F2")
                End If

                'Get CV Value
                myToolTip &= Environment.NewLine
                myToolTip &= myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CV", currentLanguage)
                myToolTip &= ": " & GetCVValue(CInt(myPoint.NumericalArgument), mySerie.LegendText).ToString("F2")

                bsScreenToolTips.ShowHint(myToolTip, LocalPoint)
            Else
                bsScreenToolTips.HideHint()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ObjectHotTracked ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ObjectHotTracked ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the icon and tooltip text of all the screen buttons
    ''' </summary>
    ''' <param name="pLanguageID">Current application language</param>
    ''' <remarks>
    ''' Created by:  TR 22/06/2011
    ''' </remarks>
    Private Sub PrepareButtons(ByVal pLanguageID As String)
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath
            Dim myToolTipsControl As New ToolTip
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'SEARCH Button
            auxIconName = GetIconName("FIND")
            If (auxIconName <> "") Then
                bsSearchButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsSearchButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Search", pLanguageID))
            End If

            'DELETE Button
            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                bsDeleteCumulateSeries.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsDeleteCumulateSeries, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", pLanguageID))
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", pLanguageID))
            End If

            'ICONS for the Graph Legend GroupBox
            auxIconName = GetIconName("GREEN_CIRCLE")
            If (auxIconName <> "") Then bsFirstCtrlLotPictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            auxIconName = GetIconName("BLUE_CIRCLE")
            If (auxIconName <> "") Then bsSecondCtrlLotPictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            auxIconName = GetIconName("VIOLET_CIRCLE")
            If (auxIconName <> "") Then bsThirdCtrlLotPictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the DataGridView of Cumulated Results by Control/Lot
    ''' </summary>
    ''' <param name="pLanguageID">Current application language</param>
    ''' <remarks>
    ''' Created by:  TR 22/06/2011
    ''' Modified by: SA 02/01/2012 - Added new column to allow save last cumulated values of each Control as 
    '''                              its Target Values for the selected Test/SampleType.  Added hidden columns
    '''                              for Min/Max Range values
    '''              DL 24/02/2012 - Changed the width of several columns
    ''' </remarks>
    Private Sub PrepareResultControlLotGrid(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsResultControlLotGridView.AutoSize = False
            bsResultControlLotGridView.MultiSelect = False
            bsResultControlLotGridView.AutoGenerateColumns = False
            bsResultControlLotGridView.Columns.Clear()

            'CheckBox to shown/hide the details of the Cumulated Series for the Control/Lot
            Dim ActiveControlColChkBox As New DataGridViewCheckBoxColumn
            ActiveControlColChkBox.Width = 20
            ActiveControlColChkBox.Name = "ActiveControl"
            ActiveControlColChkBox.HeaderText = ""
            ActiveControlColChkBox.DataPropertyName = "Selected"
            ActiveControlColChkBox.Resizable = DataGridViewTriState.False
            bsResultControlLotGridView.Columns.Add(ActiveControlColChkBox)
            bsResultControlLotGridView.Columns("ActiveControl").ReadOnly = False

            'Control Name
            bsResultControlLotGridView.Columns.Add("ControlName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", pLanguageID))
            bsResultControlLotGridView.Columns("ControlName").Width = 125
            bsResultControlLotGridView.Columns("ControlName").DataPropertyName = "ControlName"
            bsResultControlLotGridView.Columns("ControlName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopLeft
            bsResultControlLotGridView.Columns("ControlName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft
            bsResultControlLotGridView.Columns("ControlName").ReadOnly = True

            'Lot Number
            bsResultControlLotGridView.Columns.Add("LotNumber", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", pLanguageID))
            bsResultControlLotGridView.Columns("LotNumber").Width = 115
            bsResultControlLotGridView.Columns("LotNumber").DataPropertyName = "LotNumber"
            bsResultControlLotGridView.Columns("LotNumber").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsResultControlLotGridView.Columns("LotNumber").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsResultControlLotGridView.Columns("LotNumber").ReadOnly = True

            'Number of Cumulated Series created for the Test/Sample Type and Control/Lot
            bsResultControlLotGridView.Columns.Add("N", "N")
            bsResultControlLotGridView.Columns("N").Width = 25
            bsResultControlLotGridView.Columns("N").DataPropertyName = "N"
            bsResultControlLotGridView.Columns("N").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("N").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("N").ReadOnly = True

            'Cumulated Mean
            bsResultControlLotGridView.Columns.Add("Mean", LabelMEAN)
            bsResultControlLotGridView.Columns("Mean").Width = 70
            bsResultControlLotGridView.Columns("Mean").DataPropertyName = "Mean"
            bsResultControlLotGridView.Columns("Mean").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("Mean").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("Mean").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("Mean").ReadOnly = True

            'Test Measure Unit
            bsResultControlLotGridView.Columns.Add("Unit", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", pLanguageID).TrimEnd())
            bsResultControlLotGridView.Columns("Unit").Width = 70
            bsResultControlLotGridView.Columns("Unit").DataPropertyName = "MeasureUnit"
            bsResultControlLotGridView.Columns("Unit").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("Unit").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
            bsResultControlLotGridView.Columns("Unit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter
            bsResultControlLotGridView.Columns("Unit").ReadOnly = True

            'Cumulated SD
            bsResultControlLotGridView.Columns.Add("SD", LabelSD)
            bsResultControlLotGridView.Columns("SD").Width = 60
            bsResultControlLotGridView.Columns("SD").DataPropertyName = "SD"
            bsResultControlLotGridView.Columns("SD").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("SD").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("SD").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("SD").ReadOnly = True

            'Cumulated CV
            bsResultControlLotGridView.Columns.Add("CV", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CV", pLanguageID))
            bsResultControlLotGridView.Columns("CV").Width = 60
            bsResultControlLotGridView.Columns("CV").DataPropertyName = "CV"
            bsResultControlLotGridView.Columns("CV").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("CV").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CV").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultControlLotGridView.Columns("CV").ReadOnly = True

            'Cumulated Range
            bsResultControlLotGridView.Columns.Add("Ranges", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Ranges", pLanguageID))
            bsResultControlLotGridView.Columns("Ranges").Width = 130
            bsResultControlLotGridView.Columns("Ranges").DataPropertyName = "Range"
            bsResultControlLotGridView.Columns("Ranges").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("Ranges").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("Ranges").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("Ranges").ReadOnly = True

            'Button to save last Cumulated Control values as the Target ones for the selected Test/Sample Type
            Dim SaveAsTargetColButton As New DataGridViewDisableButtonColumn
            SaveAsTargetColButton.Width = 35
            SaveAsTargetColButton.Name = "SaveAsTarget"
            SaveAsTargetColButton.HeaderText = ""
            SaveAsTargetColButton.DataPropertyName = ""
            SaveAsTargetColButton.Resizable = DataGridViewTriState.False
            bsResultControlLotGridView.Columns.Add(SaveAsTargetColButton)
            bsResultControlLotGridView.Columns("SaveAsTarget").ReadOnly = True

            'Range of dates of created Cumulated Series
            bsResultControlLotGridView.Columns.Add("DateRanges", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", pLanguageID))
            bsResultControlLotGridView.Columns("DateRanges").Width = 170
            bsResultControlLotGridView.Columns("DateRanges").DataPropertyName = "DatesRange"
            bsResultControlLotGridView.Columns("DateRanges").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultControlLotGridView.Columns("DateRanges").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("DateRanges").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultControlLotGridView.Columns("DateRanges").ReadOnly = True

            'Hidden Columns
            bsResultControlLotGridView.Columns.Add("QCControlLotID", "QCControlLotID")
            bsResultControlLotGridView.Columns("QCControlLotID").DataPropertyName = "QCControlLotID"
            bsResultControlLotGridView.Columns("QCControlLotID").Visible = False

            bsResultControlLotGridView.Columns.Add("RunsGroupNumber", "RunsGroupNumber")
            bsResultControlLotGridView.Columns("RunsGroupNumber").DataPropertyName = "RunsGroupNumber"
            bsResultControlLotGridView.Columns("RunsGroupNumber").Visible = False

            bsResultControlLotGridView.Columns.Add("MinRange", "MinRange")
            bsResultControlLotGridView.Columns("MinRange").DataPropertyName = "MinRange"
            bsResultControlLotGridView.Columns("MinRange").Visible = False

            bsResultControlLotGridView.Columns.Add("MaxRange", "MaxRange")
            bsResultControlLotGridView.Columns("MaxRange").DataPropertyName = "MaxRange"
            bsResultControlLotGridView.Columns("MaxRange").Visible = False

            bsResultControlLotGridView.Columns.Add("DeletedControlLot", "DeletedControlLot")
            bsResultControlLotGridView.Columns("DeletedControlLot").DataPropertyName = "DeletedControlLot"
            bsResultControlLotGridView.Columns("DeletedControlLot").Visible = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareResultControlLotGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareResultControlLotGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the DataGridView of Cumulated Series Details
    ''' </summary>
    ''' <param name="pLanguageID">Current application language</param>
    ''' <remarks>
    ''' Created by:  TR 22/06/2011
    ''' </remarks>
    Private Sub PrepareResultDetailsGrid(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsResultsDetailsGridView.AutoSize = False
            bsResultsDetailsGridView.ReadOnly = True
            bsResultsDetailsGridView.MultiSelect = True
            bsResultsDetailsGridView.AutoGenerateColumns = False
            bsResultsDetailsGridView.Columns.Clear()

            'Number of the Cumulated Serie for the Test/SampleType and Control/Lot
            bsResultsDetailsGridView.Columns.Add("VisibleCumResultsNum", "")
            bsResultsDetailsGridView.Columns("VisibleCumResultsNum").Width = 30
            bsResultsDetailsGridView.Columns("VisibleCumResultsNum").DataPropertyName = "VisibleCumResultNumber"
            bsResultsDetailsGridView.Columns("VisibleCumResultsNum").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("VisibleCumResultsNum").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopLeft
            bsResultsDetailsGridView.Columns("VisibleCumResultsNum").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft

            'Control Name
            bsResultsDetailsGridView.Columns.Add("ControlName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", pLanguageID))
            bsResultsDetailsGridView.Columns("ControlName").Width = 125
            bsResultsDetailsGridView.Columns("ControlName").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("ControlName").DataPropertyName = "ControlName"
            bsResultsDetailsGridView.Columns("ControlName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopLeft
            bsResultsDetailsGridView.Columns("ControlName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft

            'Lot Number
            bsResultsDetailsGridView.Columns.Add("LotNumber", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", pLanguageID))
            bsResultsDetailsGridView.Columns("LotNumber").Width = 115
            bsResultsDetailsGridView.Columns("LotNumber").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("LotNumber").DataPropertyName = "LotNumber"
            bsResultsDetailsGridView.Columns("LotNumber").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsResultsDetailsGridView.Columns("LotNumber").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsResultsDetailsGridView.Columns("LotNumber").ReadOnly = True

            'Date in which the Cumulated Serie was created
            bsResultsDetailsGridView.Columns.Add("Date", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", pLanguageID))
            bsResultsDetailsGridView.Columns("Date").Width = 140
            bsResultsDetailsGridView.Columns("Date").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("Date").DataPropertyName = "CumDateTime"
            bsResultsDetailsGridView.Columns("Date").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("Date").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultsDetailsGridView.Columns("Date").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultsDetailsGridView.Columns("Date").ReadOnly = True

            'Number of individual results included in the Cumulated Serie
            bsResultsDetailsGridView.Columns.Add("n", "n")
            bsResultsDetailsGridView.Columns("n").Width = 30
            bsResultsDetailsGridView.Columns("n").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("n").DataPropertyName = "TotalRuns"
            bsResultsDetailsGridView.Columns("n").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopRight
            bsResultsDetailsGridView.Columns("n").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopRight

            'Mean of all the individual results included in the Cumulated Serie 
            bsResultsDetailsGridView.Columns.Add("Mean", LabelMEAN)
            bsResultsDetailsGridView.Columns("Mean").Width = 70
            bsResultsDetailsGridView.Columns("Mean").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("Mean").DataPropertyName = "Mean"
            bsResultsDetailsGridView.Columns("Mean").DefaultCellStyle.NullValue = Nothing
            bsResultsDetailsGridView.Columns("Mean").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("Mean").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultsDetailsGridView.Columns("Mean").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultsDetailsGridView.Columns("Mean").ReadOnly = True

            'Test Measure Unit
            bsResultsDetailsGridView.Columns.Add("Unit", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", pLanguageID).TrimEnd())
            bsResultsDetailsGridView.Columns("Unit").Width = 70
            bsResultsDetailsGridView.Columns("Unit").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("Unit").DataPropertyName = "MeasureUnit"
            bsResultsDetailsGridView.Columns("Unit").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("Unit").HeaderCell.Style.Alignment = DataGridViewContentAlignment.TopCenter
            bsResultsDetailsGridView.Columns("Unit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter
            bsResultsDetailsGridView.Columns("Unit").ReadOnly = True

            'SD of all the individual results included in the Cumulated Serie 
            bsResultsDetailsGridView.Columns.Add("SD", LabelSD)
            bsResultsDetailsGridView.Columns("SD").Width = 70
            bsResultsDetailsGridView.Columns("SD").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("SD").DataPropertyName = "SD"
            bsResultsDetailsGridView.Columns("SD").DefaultCellStyle.NullValue = Nothing
            bsResultsDetailsGridView.Columns("SD").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("SD").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultsDetailsGridView.Columns("SD").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultsDetailsGridView.Columns("SD").ReadOnly = True

            'CV of all the individual results included in the Cumulated Serie 
            bsResultsDetailsGridView.Columns.Add("CV", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CV", pLanguageID))
            bsResultsDetailsGridView.Columns("CV").Width = 70
            bsResultsDetailsGridView.Columns("CV").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("CV").DataPropertyName = "CV"
            bsResultsDetailsGridView.Columns("CV").DefaultCellStyle.NullValue = Nothing
            bsResultsDetailsGridView.Columns("CV").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("CV").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultsDetailsGridView.Columns("CV").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsResultsDetailsGridView.Columns("CV").ReadOnly = True

            'Min/Max Concentration Values used to validate the individual results included in the Cumulated Serie
            bsResultsDetailsGridView.Columns.Add("Ranges", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Ranges", pLanguageID))
            bsResultsDetailsGridView.Columns("Ranges").Width = 135
            bsResultsDetailsGridView.Columns("Ranges").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("Ranges").DataPropertyName = "Range"
            bsResultsDetailsGridView.Columns("Ranges").SortMode = DataGridViewColumnSortMode.NotSortable
            bsResultsDetailsGridView.Columns("Ranges").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultsDetailsGridView.Columns("Ranges").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultsDetailsGridView.Columns("Ranges").ReadOnly = True

            'Hidden Columns
            bsResultsDetailsGridView.Columns.Add("QCTestSampleID", "QCTestSampleID")
            bsResultsDetailsGridView.Columns("QCTestSampleID").Width = 135
            bsResultsDetailsGridView.Columns("QCTestSampleID").DataPropertyName = "QCTestSampleID"
            bsResultsDetailsGridView.Columns("QCTestSampleID").Visible = False

            bsResultsDetailsGridView.Columns.Add("QCControlLotID", "QCControlLotID")
            bsResultsDetailsGridView.Columns("QCControlLotID").Width = 135
            bsResultsDetailsGridView.Columns("QCControlLotID").DataPropertyName = "QCControlLotID"
            bsResultsDetailsGridView.Columns("QCControlLotID").Visible = False

            bsResultsDetailsGridView.Columns.Add("CumResultsNum", "")
            bsResultsDetailsGridView.Columns("CumResultsNum").Width = 30
            bsResultsDetailsGridView.Columns("CumResultsNum").DataPropertyName = "CumResultsNum"
            bsResultsDetailsGridView.Columns("CumResultsNum").Visible = False

            bsResultsDetailsGridView.Columns.Add("XMLFileName", "")
            bsResultsDetailsGridView.Columns("XMLFileName").Width = 30
            bsResultsDetailsGridView.Columns("XMLFileName").DataPropertyName = "XMLFileName"
            bsResultsDetailsGridView.Columns("XMLFileName").Visible = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareResultDetailsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareResultDetailsGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the Tests/Sample Types ListView
    ''' </summary>
    ''' <param name="pLanguageID">Current application language</param>
    ''' <remarks>
    ''' Created by:  TR 22/06/2011
    ''' Modified by: SA 04/01/2012 - Load in the ImageList of Icons that can be shown in the ListView, the pair of icons for 
    '''                              Standard Tests/SampleTypes deleted
    '''              SA 06/06/2012 - Load in the ImageList of Icons that can be shown in the ListView, the icons for ISE Tests/SampleTypes 
    '''                              active and deleted
    ''' </remarks>
    Private Sub PrepareTestListView(ByVal pLanguageID As String)
        Try
            Dim myTestIconList As New ImageList
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            myTestIconList.Images.Add("TESTICON", ImageUtilities.ImageFromFile(MyBase.IconsPath & GetIconName("TESTICON")))
            myTestIconList.Images.Add("USERTEST", ImageUtilities.ImageFromFile(MyBase.IconsPath & GetIconName("USERTEST")))
            myTestIconList.Images.Add("TISE_SYS", ImageUtilities.ImageFromFile(MyBase.IconsPath & GetIconName("TISE_SYS")))
            myTestIconList.Images.Add("INUSETEST", ImageUtilities.ImageFromFile(MyBase.IconsPath & GetIconName("INUSETEST")))
            myTestIconList.Images.Add("INUSUSTEST", ImageUtilities.ImageFromFile(MyBase.IconsPath & GetIconName("INUSUSTEST")))
            myTestIconList.Images.Add("INUSETISE", ImageUtilities.ImageFromFile(MyBase.IconsPath & GetIconName("INUSETISE")))

            'Initialization of Tests/Sample Types list
            bsTestSampleListView.Items.Clear()
            bsTestSampleListView.Scrollable = True
            bsTestSampleListView.MultiSelect = False
            bsTestSampleListView.View = View.Details
            bsTestSampleListView.FullRowSelect = True
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
            bsCumulatedResultsLabel = Nothing
            bsDateToDateTimePick = Nothing
            bsDateToLabel = Nothing
            bsSearchButton = Nothing
            bsExitButton = Nothing
            bsCumulatedSeriesLabel = Nothing
            bsScreenErrorProvider = Nothing
            bsPrintButton = Nothing
            bsResultControlLotGridView = Nothing
            bsCumulatedXtraTab = Nothing
            ValuesXtraTab = Nothing
            bsResultsDetailsGridView = Nothing
            GraphXtraTab = Nothing
            bsDeleteCumulateSeries = Nothing
            bsMeanChartControl = Nothing
            bsLegendGB = Nothing
            bsFirstCtrlLotPictureBox = Nothing
            bsFirstCtrlLotLabel = Nothing
            bsSecondCtrlLotLabel = Nothing
            bsSecondCtrlLotPictureBox = Nothing
            bsThirdCtrlLotLabel = Nothing
            bsThirdCtrlLotPictureBox = Nothing
            bsScreenToolTips = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Update Min/Max Concentration values for the specified QCTestSampleID and QCControlLotID in table tqcHistoryTestControlLots
    ''' in QC Module, and for the correspondent TestID/SampleType in table tparTestControls
    ''' </summary>
    ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC</param>
    ''' <param name="pQCControlLotID">Identifier of Control/Lot in QC</param>
    ''' <param name="pMinValue">Min concentration value</param>
    ''' <param name="pMaxValue">Max concentration value</param>
    ''' <remarks>
    ''' Created by:  SA 02/01/2012
    ''' </remarks>
    Private Sub SaveCumValuesAsTarget(ByVal pQCTestSampleID As Integer, ByVal pQCControlLotID As Integer, ByVal pMinValue As Single, ByVal pMaxValue As Single)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myHistTestControlsDelegate As New HistoryTestControlLotsDelegate

            myGlobalDataTO = myHistTestControlsDelegate.SaveLastCumulatedAsTargetNEW(Nothing, pQCTestSampleID, pQCControlLotID, AnalyzerIDAttribute, pMinValue, pMaxValue)
            If (myGlobalDataTO.HasError) Then
                'Error updating the Target Min/Max values for the Test/SampleType and Control/Lot; show it
                ShowMessage(Me.Name & ".SaveCumValuesAsTarget", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveCumValuesAsTarget ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveCumValuesAsTarget ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Enable/disable availability of button for deleting Cumulated Series according the Level of the connected User
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 20/04/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select
                Case "OPERATOR"
                    bsDeleteCumulateSeries.Enabled = False
                    Exit Select
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Validate the number of active (selected) Controls
    ''' </summary>
    ''' <returns>Return true if the number of controls selected is lower than 3</returns>
    ''' <remarks>
    ''' Created by:  TR 05/07/2011
    ''' </remarks>
    Private Function ValidateActiveControls() As Boolean
        Dim myResult As Boolean = False

        Try
            'Validate if there are more than two controls selected
            myResult = ((From a In LocalQCCumulateResultsDS.QCCumulatedSummaryTable _
                    Where Not a.IsSelectedNull AndAlso a.Selected _
                       Select a).Count < 4)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ValidateActiveControls ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ValidateActiveControls ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function
#End Region

#Region "Events"
    '*****************'
    '* SCREEN EVENTS *'
    '*****************'
    Private Sub IQCCumulatedReview_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then bsExitButton.PerformClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IQCCumulatedReview_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IQCCumulatedReview_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen loading and initialization
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR
    ''' Modified by: TR 20/04/2012 - Get the Level of the connected User and save it in the global variable CurrentUserLevel
    ''' </remarks>
    Private Sub IQCCumulatedReview_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'Dim myGlobalbase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo().UserLevel

            InitializeScreen()
            FillTestSampleListView()

            ResetBorder()
            Application.DoEvents()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IQCCumulatedReview_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IQCCumulatedReview_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IQCCumulatedReview_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Try
            If (bsTestSampleListView.Items.Count > 0) Then
                'Select the first element on the list
                bsTestSampleListView.Items(0).Selected = True
            Else
                'If there are not selected Tests/SampleTypes, disable all screen controls
                bsCalculationCriteriaGroupBox.Enabled = False
                bsPrintButton.Enabled = False
            End If
            ScreenStatusByUserLevel()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IQCCumulatedReview_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IQCCumulatedReview_Shown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*****************'
    '* BUTTON EVENTS *'
    '*****************'
    Private Sub bsDeleteCumulateSeries_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDeleteCumulateSeries.Click
        Try
            DeleteSelectedResult()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsDeleteCumulateSeries_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDeleteCumulateSeries_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsSearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSearchButton.Click
        Try
            If (bsTestSampleListView.Items.Count > 0 AndAlso bsTestSampleListView.SelectedItems.Count > 0) Then
                PrevSelectedControlName.Clear()
                GetControlsLotsWithCumulatedSeries(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text), True)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSearchButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSearchButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            If (Not Me.Tag Is Nothing) Then
                'A PerformClick() method was executed
                Me.Close()
            Else
                'Normal button click - Open the WS Monitor form and close this one
                IAx00MainMDI.OpenMonitorForm(Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsExitButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            If (bsTestSampleListView.SelectedItems.Count = 1) AndAlso (Not bsResultControlLotGridView.DataSource Is Nothing) AndAlso _
               (Not bsResultsDetailsGridView.DataSource Is Nothing) Then
                Dim testSampleId As Integer = CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text)
                Dim decAllow As Integer = CInt(bsTestSampleListView.SelectedItems(0).SubItems(4).Text)

                Cursor = Cursors.WaitCursor
                XRManager.ShowQCAccumulatedResultsByTestReport(testSampleId, bsDateFromDateTimePick.Value, bsDateToDateTimePick.Value, _
                                                               LocalQCCumulateResultsDS, LocalCumulateResultsDS, decAllow)
                Cursor = Cursors.Default
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsPrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsPrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '**********************************************'
    '* EVENTS FOR LIST VIEW OF TESTS/SAMPLE TYPES *'
    '**********************************************'
    Private Sub bsTestSampleListView_ColumnWidthChanging(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles bsTestSampleListView.ColumnWidthChanging
        Try
            e.Cancel = True
            If (e.ColumnIndex = 0) Then
                e.NewWidth = 168
            ElseIf (e.ColumnIndex = 1) Then
                e.NewWidth = 50
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestSampleListView_ColumnWidthChanging", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestSampleListView_ColumnWidthChanging", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsTestSampleListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTestSampleListView.SelectedIndexChanged
        Try
            If (bsTestSampleListView.Items.Count > 0 AndAlso bsTestSampleListView.SelectedItems.Count > 0) Then
                PrevSelectedControlName.Clear()
                LocalDecimalsAllowed = CInt(bsTestSampleListView.SelectedItems(0).SubItems(4).Text)

                'Load data for the selected Test/Sample Type
                GetControlsLotsWithCumulatedSeries(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text))

                'If the Test or the Test/SampleType is marked as deleted, column Save as Target is hidden
                bsResultControlLotGridView.Columns("SaveAsTarget").Visible = Convert.ToBoolean(bsTestSampleListView.SelectedItems(0).SubItems(5).Text)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestSampleListView_SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsTestSampleListView_SelectedIndexChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    '***************************************************'
    '* EVENTS FOR DATA GRID VIEW OF LINKED CONTROL/LOT *'
    '***************************************************'
    Private Sub bsResultControlLotGridView_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsResultControlLotGridView.CellClick
        Try
            If (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "SaveAsTarget") Then
                If (CType(bsResultControlLotGridView.Rows(e.RowIndex).Cells("SaveAsTarget"), DataGridViewDisableButtonCell).Enabled) Then

                    'TR 17/01/2012 - Show DialogBox to confirm the modification of the Ranges
                    If (ShowMessage("", GlobalEnumerates.Messages.TARGET_CAL_UPDATE.ToString()) = Windows.Forms.DialogResult.Yes) Then
                        SaveCumValuesAsTarget(Convert.ToInt32(bsTestSampleListView.SelectedItems(0).SubItems(2).Text), _
                                                                  Convert.ToInt32(bsResultControlLotGridView.Rows(e.RowIndex).Cells("QCControlLotID").Value), _
                                                                  Convert.ToSingle(bsResultControlLotGridView.Rows(e.RowIndex).Cells("MinRange").Value), _
                                                                  Convert.ToSingle(bsResultControlLotGridView.Rows(e.RowIndex).Cells("MaxRange").Value))
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsResultControlLotGridView_CellClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsResultControlLotGridView_CellClick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsResultControlLotGridView_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) _
                                                          Handles bsResultControlLotGridView.CellFormatting
        Try
            If (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "Mean") OrElse _
               (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "SD") OrElse _
               (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "CV") Then
                If (Not e.Value Is Nothing AndAlso Not e.Value Is DBNull.Value) Then
                    If (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "SD") Then
                        e.Value = DirectCast(e.Value, Single).ToString("F" & (LocalDecimalsAllowed + 1))

                    ElseIf (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "Mean") Then
                        e.Value = DirectCast(e.Value, Single).ToString("F" & LocalDecimalsAllowed)

                    ElseIf (bsResultControlLotGridView.Columns(e.ColumnIndex).Name = "CV") Then
                        e.Value = DirectCast(e.Value, Single).ToString("F2")
                    End If
                Else
                    'If there are not cumulated values for the Control, button for save them as target values is disabled
                    bsResultControlLotGridView.Rows(e.RowIndex).Cells("SaveAsTarget").ReadOnly = True
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsResultControlLotGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsResultControlLotGridView_CellFormatting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsResultControlLotGridView_CellValueChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
                                                            Handles bsResultControlLotGridView.CellValueChanged
        Try
            If (bsResultControlLotGridView.CurrentRow.Index >= 0 AndAlso Me.bsResultControlLotGridView.CurrentCell.ColumnIndex = 0) Then
                If bsTestSampleListView.SelectedItems.Count > 0 Then
                    If (CBool(bsResultControlLotGridView.CurrentCell.Value)) Then
                        If (ValidateActiveControls()) Then
                            PrevSelectedControlName.Clear()
                            FilterCumulatedSeries(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text))
                        Else
                            bsResultControlLotGridView.CurrentCell.Value = False
                            bsResultControlLotGridView.RefreshEdit()
                        End If
                    Else
                        PrevSelectedControlName.Clear()
                        FilterCumulatedSeries(CInt(bsTestSampleListView.SelectedItems(0).SubItems(2).Text))
                    End If
                Else
                    'TR 24/07/2012 -No TestSample Selected then clear.
                    bsResultControlLotGridView.DataSource = Nothing
                    bsResultsDetailsGridView.DataSource = Nothing
                    'Clear Graphics controls
                    bsMeanChartControl.Series.Clear()
                End If


            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsResultControlLotGridView_CellValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsResultControlLotGridView_CellValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsResultControlLotGridView_CurrentCellDirtyStateChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsResultControlLotGridView.CurrentCellDirtyStateChanged
        Try
            If (TypeOf bsResultControlLotGridView.CurrentCell Is DataGridViewCheckBoxCell) Then
                bsResultControlLotGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsResultControlLotGridView_CurrentCellDirtyStateChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsResultControlLotGridView_CurrentCellDirtyStateChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    '*************************************************'
    '* EVENTS FOR DATA GRID VIEW OF CUMULATED SERIES *'
    '*************************************************'
    Private Sub bsResultsDetailsGridView_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) _
                                                      Handles bsResultsDetailsGridView.CellFormatting
        Try
            If (Not e.Value Is DBNull.Value AndAlso Not e.Value Is Nothing) Then
                If (bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "SD") Then
                    e.Value = DirectCast(e.Value, Single).ToString("F" & (LocalDecimalsAllowed + 1))

                ElseIf (bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "Mean") Then
                    e.Value = DirectCast(e.Value, Single).ToString("F" & LocalDecimalsAllowed)

                ElseIf (bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "CV" AndAlso Not e.Value Is DBNull.Value) Then
                    e.Value = DirectCast(e.Value, Single).ToString("F2")

                ElseIf (bsResultsDetailsGridView.Columns(e.ColumnIndex).Name = "Date") Then
                    e.Value = DirectCast(e.Value, DateTime).ToString(SystemInfoManager.OSDateFormat) & " " & DirectCast(e.Value, DateTime).ToString(SystemInfoManager.OSLongTimeFormat)
                End If
            End If

        Catch ex As DataException
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsResultsDetailsGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsResultsDetailsGridView_CellFormatting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsResultsDetailsGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsResultsDetailsGridView_CellFormatting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsResultsDetailsGridView_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsResultsDetailsGridView.KeyDown
        Try
            If (e.KeyCode = Keys.Delete) Then DeleteSelectedResult()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsResultsDetailsGridView_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsResultsDetailsGridView_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    '************************************'
    '* EVENTS FOR OTHER SCREEN CONTROLS *'
    '************************************'
    Private Sub DateFromDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                                                  Handles bsDateFromDateTimePick.ValueChanged, bsDateToDateTimePick.ValueChanged
        Try
            If (DirectCast(sender, BSDateTimePicker).Name = "DateFromDateTimePick") Then
                'Set the min allowed value for the control 
                bsDateToDateTimePick.MinDate = bsDateFromDateTimePick.Value
            End If

            'Clear the grids 
            bsResultControlLotGridView.DataSource = Nothing
            bsResultsDetailsGridView.DataSource = Nothing

            'Clear Graphics controls
            bsMeanChartControl.Series.Clear()
            bsMeanChartControl.ClearCache()
            bsMeanChartControl.Legend.Visible = False
            bsMeanChartControl.BackColor = Color.White
            bsMeanChartControl.AppearanceName = "Light"
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DateFromDateTimePick_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DateFromDateTimePick_ValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub CumulatedXtraTab_Selecting(ByVal sender As System.Object, ByVal e As DevExpress.XtraTab.TabPageCancelEventArgs) _
                                           Handles bsCumulatedXtraTab.Selecting
        Try
            If (e.Page.Name = "GraphXtraTab") Then
                If (LocalQCCumulateResultsDS.QCCumulatedSummaryTable.Where(Function(a) a.Selected AndAlso a.IsMeanNull).Count > 0) Then
                    e.Cancel = True
                    ShowMessage("", GlobalEnumerates.Messages.CUM_SERIES_LACK.ToString())
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CumulatedXtraTab_Selecting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CumulatedXtraTab_Selecting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub MeanChartControl_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsMeanChartControl.MouseMove
        Try
            LocalPoint = bsMeanChartControl.PointToScreen(New Point(e.X, e.Y))
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".MeanChartControl_MouseMove ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".MeanChartControl_MouseMove ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region
End Class


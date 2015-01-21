Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraCharts
Imports System.Drawing

Public Class QCIndividualResultsByTestReport

#Region "Declarations"
    Private mTestSampleData As HistoryTestSamplesDS.tqcHistoryTestSamplesRow
    Private mDateRangeText As String
    Private mIncludeGraph As Boolean
    Private mControlsDS As OpenQCResultsDS
    Private mLabelMEAN As String = ""
    Private mLabelSD As String = ""
    Private mResultsDS As QCResultsDS
#End Region

#Region "Public Methods"
    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 25/09/2014 - BA-1608 ==> ** Added some changes required after activation of Option Strict On
    '''                                          ** Added changes to remove all Excluded Results from the entry pResultsDS
    '''                                          ** Added changes to verify for each Selected Control if there are not Excluded Results for it
    '''                                          ** Changed from Sub to Function that returns a Boolean value: TRUE if the Report can be generated, 
    '''                                             and FALSE if the Report cannot be generated (if none of the linked Controls are Selected, or if 
    '''                                             all results for the selected Controls are marked as Excluded)
    ''' </remarks>
    Public Function SetControlsAndResultsDatasource(ByVal pTestSampleRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow, ByVal pControlsDS As OpenQCResultsDS, _
                                                    ByVal pResultsDS As QCResultsDS, ByVal pLocalDecimalAllow As Integer, ByVal pDateRangeText As String, _
                                                    ByVal pGraphType As REPORT_QC_GRAPH_TYPE) As Boolean
        Dim generateReport As Boolean = False

        'Get all selected Controls
        Dim lstSelectedControls As List(Of OpenQCResultsDS.tOpenResultsRow) = (From c As OpenQCResultsDS.tOpenResultsRow In pControlsDS.tOpenResults _
                                                                              Where c.Selected = True _
                                                                             Select c).ToList()

        If (lstSelectedControls.Count > 0) Then
            'BA-1608 - Get all Results marked as Excluded and remove them from the DataSet
            Dim myLocalResultsDS As QCResultsDS = DirectCast(pResultsDS.Copy(), QCResultsDS)
            Dim myRow() As QCResultsDS.tqcResultsRow = DirectCast(myLocalResultsDS.tqcResults.Select("Excluded = True"), QCResultsDS.tqcResultsRow())
            For i As Integer = 0 To UBound(myRow)
                myLocalResultsDS.tqcResults.Rows.Remove(myRow(i))
            Next
            myLocalResultsDS.AcceptChanges()

            If (myLocalResultsDS.tqcResults.Rows.Count > 0) Then
                'Add the SubReports
                For Each elem As OpenQCResultsDS.tOpenResultsRow In lstSelectedControls
                    'BA-1608 - Verify if there are not Excluded Results for the Control - If all Results have been excluded, the Control is just ignored
                    If (myLocalResultsDS.tqcResults.ToList.Where(Function(a) a.QCControlLotID = elem.QCControlLotID).Count > 0) Then
                        'The Report will be generated only when there are Results to print for at least one of the selected Controls
                        generateReport = True

                        Dim mQCRep As New QCIndividualResultsByTestControlReport
                        mQCRep.ControlLotID.Value = elem.QCControlLotID
                        mQCRep.SetControlsAndResultsDatasource(pControlsDS, myLocalResultsDS, pLocalDecimalAllow, pGraphType, pTestSampleRow.RejectionCriteria)
                        'mQCRep.SetControlsAndResultsDatasource(pControlsDS, pResultsDS, pLocalDecimalAllow, pGraphType, pTestSampleRow.RejectionCriteria)

                        Dim mSubReport As New XRSubreport()
                        mSubReport.Name = "SubReport" & elem.QCControlLotID.ToString
                        mSubReport.ReportSource = mQCRep
                        Me.Detail1.Controls.Add(mSubReport)
                        mSubReport.TopF = Me.Detail1.HeightF
                        mSubReport.LeftF = 0
                        Me.Detail1.HeightF += mSubReport.HeightF
                    End If
                Next

                'BA-1608 - Inform the global variables used for LJ and Youden Reports
                If (generateReport) Then
                    mTestSampleData = pTestSampleRow
                    mDateRangeText = pDateRangeText
                    mControlsDS = pControlsDS
                    mResultsDS = myLocalResultsDS
                    mIncludeGraph = (pGraphType = REPORT_QC_GRAPH_TYPE.YOUDEN_GRAPH)
                End If
            End If
        End If
        lstSelectedControls = Nothing

        Return generateReport
    End Function
#End Region

#Region "Private Methods"
    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 23/09/2014 - BA-1608 ==> If ReportName is informed (field TestLongName is not Null nor empty), use it as Test Name in the report
    ''' </remarks>
    Private Sub QCIndividualResultsByTestReport_BeforePrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles Me.BeforePrint

        If Me.DesignMode Then Exit Sub

        'Multilanguage support
        'Dim currentLanguageGlobal As New GlobalBase
        Dim CurrentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        'Multilanguage. Get texts from DB.
        XrHeaderLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_QCResultsByTestSampleType", CurrentLanguage)
        XrLabelTestName.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", CurrentLanguage)
        XrLabelSample.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", CurrentLanguage)
        XrLabelCalcultationMode.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calculation_Mode", CurrentLanguage)
        XrLabelDateRange.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DateRange", CurrentLanguage)
        XrLabelControls.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Controls_List", CurrentLanguage)

        'BA-1608 - If ReportName is informed (field TestLongName is not Null nor empty), use it as Test Name in the report
        Dim myTestName As String = mTestSampleData.TestName
        If (Not mTestSampleData.IsTestLongNameNull AndAlso mTestSampleData.TestLongName <> String.Empty) Then myTestName = mTestSampleData.TestLongName
        XrTestName.Text = myTestName

        'Rest of the TestSample data
        XrSample.Text = mTestSampleData.SampleType
        XrMode.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_QC_CALC_MODES_" & mTestSampleData.CalculationMode, CurrentLanguage)
        XrDateRange.Text = mDateRangeText

        'Show/Hide the graph
        GroupFooter1.Visible = mIncludeGraph

        If mIncludeGraph Then
            'Header: Control Level1 Name (Lot) + Control Level2 Name (Lot)
            XrGraphHeaderLabel.Text = ""
            For Each ctrl In From elem In mControlsDS.tOpenResults Where elem.Selected
                If (Not String.IsNullOrEmpty(XrGraphHeaderLabel.Text)) Then XrGraphHeaderLabel.Text &= " + "
                XrGraphHeaderLabel.Text &= ctrl.ControlName & " (" & ctrl.LotNumber & ")"
            Next
            PrepareYoudenGraph()
        End If

    End Sub
#End Region

#Region "Youden Graph"
    ''' <summary>
    ''' Draw a constant line in X-Axis of a Youden Graph
    ''' </summary>
    ''' <param name="pName">Title for the Constant line</param>
    ''' <param name="pDiagram">Diagram in which the Constant line will be added</param>
    ''' <param name="pValue">Value in X-Axis in which the Constant line will be drawn</param>
    ''' <param name="pColor">Color for the Constant line</param>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' </remarks>
    Private Sub CreateConstantLineAxisX(ByVal pName As String, ByVal pDiagram As XYDiagram, ByVal pValue As Double, ByVal pColor As Color)
        Try
            'Create the constant line
            Dim constantLine As New ConstantLine(pName)
            pDiagram.AxisX.ConstantLines.Add(constantLine)

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
            constantLine.Title.ShowBelowLine = False
            constantLine.Title.Alignment = ConstantLineTitleAlignment.Far
            constantLine.Title.Font = New Font("Verdana", 8, FontStyle.Bold)

            'Customize the appearance of the constant line.
            constantLine.Color = pColor
            constantLine.LineStyle.Thickness = 1
            constantLine.LineStyle.DashStyle = DashStyle.Solid
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Draw a constant line in Y-Axis of a Youden Graph
    ''' </summary>
    ''' <param name="pName">Title for the Constant line</param>
    ''' <param name="pDiagram">Diagram in which the Constant line will be added</param>
    ''' <param name="pValue">Value in Y-Axis in which the Constant line will be drawn</param>
    ''' <param name="pColor">Color for the Constant line</param>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' </remarks>
    Private Sub CreateConstantLineAxisY(ByVal pName As String, ByVal pDiagram As XYDiagram, ByVal pValue As Single, ByVal pColor As Color)
        Try
            'Create the constant line
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
            constantLine.Title.ShowBelowLine = False
            constantLine.Title.Antialiasing = False
            constantLine.Title.Alignment = ConstantLineTitleAlignment.Far
            constantLine.Title.Font = New Font("Verdana", 8, FontStyle.Bold)

            'Customize the appearance of the constant line.
            constantLine.Color = pColor
            constantLine.LineStyle.Thickness = 1
            constantLine.LineStyle.DashStyle = DashStyle.Solid
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Create the SD Squares in a Youden Graph
    ''' </summary>
    ''' <param name="pControl1Mean">Mean of the first selected Control</param>
    ''' <param name="pControl1SD">SD of the first selected Control</param>
    ''' <param name="pControl2Mean">Mean of the second selected Control</param>
    ''' <param name="pControl2SD">SD of the second selected Control</param>
    ''' <remarks>
    ''' Created by:  TR 17/06/2011
    ''' </remarks>
    Private Sub CreateSquares(ByVal pControl1Mean As Double, ByVal pControl1SD As Double, _
                              ByVal pControl2Mean As Double, ByVal pControl2SD As Double)
        Try
            Dim myLineSeriesView As LineSeriesView

            '****************'
            '*  Square SD1  *'
            '****************'
            Dim series4 As New Series("SD1L1", ViewType.Line)
            series4.Points.Add(New SeriesPoint((pControl1Mean + pControl1SD), (pControl2Mean + pControl2SD)))
            series4.Points.Add(New SeriesPoint((pControl1Mean - pControl1SD), (pControl2Mean + pControl2SD)))

            myLineSeriesView = CType(series4.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series4.Label.TextPattern = "{V}"
            series4.ArgumentScaleType = ScaleType.Numerical
            series4.ValueScaleType = ScaleType.Numerical
            series4.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            Dim series41 As New Series("SD1L2", ViewType.Line)
            series41.Points.Add(New SeriesPoint(pControl1Mean - (pControl1SD), pControl2Mean + (pControl2SD)))
            series41.Points.Add(New SeriesPoint(pControl1Mean - (pControl1SD), (pControl2Mean - (pControl2SD))))

            myLineSeriesView = CType(series41.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series41.Label.TextPattern = "{V}"
            series41.ArgumentScaleType = ScaleType.Numerical
            series41.ValueScaleType = ScaleType.Numerical
            series41.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            Dim series42 As New Series("SD1L3", ViewType.Line)
            series42.Points.Add(New SeriesPoint(pControl1Mean - (pControl1SD), pControl2Mean - (pControl2SD)))
            series42.Points.Add(New SeriesPoint(pControl1Mean + (pControl1SD), (pControl2Mean - (pControl2SD))))

            myLineSeriesView = CType(series42.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series42.Label.TextPattern = "{V}"
            series42.ArgumentScaleType = ScaleType.Numerical
            series42.ValueScaleType = ScaleType.Numerical
            series42.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            Dim series43 As New Series("SD1L4", ViewType.Line)
            series43.Points.Add(New SeriesPoint(pControl1Mean + (pControl1SD), pControl2Mean - (pControl2SD)))
            series43.Points.Add(New SeriesPoint(pControl1Mean + (pControl1SD), (pControl2Mean + (pControl2SD))))

            myLineSeriesView = CType(series43.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series43.Label.TextPattern = "{V}"
            series43.ArgumentScaleType = ScaleType.Numerical
            series43.ValueScaleType = ScaleType.Numerical
            series43.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            '****************'
            '*  Square SD2  *'
            '****************'
            Dim series40 As New Series("SD2L1", ViewType.Line)
            series40.Points.Add(New SeriesPoint(pControl1Mean + 2 * (pControl1SD), pControl2Mean + 2 * (pControl2SD)))
            series40.Points.Add(New SeriesPoint(pControl1Mean - 2 * (pControl1SD), (pControl2Mean + 2 * (pControl2SD))))

            myLineSeriesView = CType(series40.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series40.Label.TextPattern = "{V}"
            series40.ArgumentScaleType = ScaleType.Numerical
            series40.ValueScaleType = ScaleType.Numerical
            series40.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            Dim series410 As New Series("SD2L2", ViewType.Line)
            series410.Points.Add(New SeriesPoint(pControl1Mean - 2 * (pControl1SD), pControl2Mean + 2 * (pControl2SD)))
            series410.Points.Add(New SeriesPoint(pControl1Mean - 2 * (pControl1SD), (pControl2Mean - 2 * (pControl2SD))))

            myLineSeriesView = CType(series410.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series410.Label.TextPattern = "{V}"
            series410.ArgumentScaleType = ScaleType.Numerical
            series410.ValueScaleType = ScaleType.Numerical
            series410.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            Dim series420 As New Series("SD2L3", ViewType.Line)
            series420.Points.Add(New SeriesPoint(pControl1Mean - 2 * (pControl1SD), pControl2Mean - 2 * (pControl2SD)))
            series420.Points.Add(New SeriesPoint(pControl1Mean + 2 * (pControl1SD), (pControl2Mean - 2 * (pControl2SD))))

            myLineSeriesView = CType(series420.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash
            myLineSeriesView.LineStyle.Thickness = 2

            series420.Label.TextPattern = "{V}"
            series420.ArgumentScaleType = ScaleType.Numerical
            series420.ValueScaleType = ScaleType.Numerical
            series420.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            Dim series430 As New Series("SD2L4", ViewType.Line)
            series430.Points.Add(New SeriesPoint(pControl1Mean + 2 * (pControl1SD), pControl2Mean - 2 * (pControl2SD)))
            series430.Points.Add(New SeriesPoint(pControl1Mean + 2 * (pControl1SD), (pControl2Mean + 2 * (pControl2SD))))

            myLineSeriesView = CType(series430.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.Thickness = 2
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash

            series430.Label.TextPattern = "{V}"
            series430.ArgumentScaleType = ScaleType.Numerical
            series430.ValueScaleType = ScaleType.Numerical
            series430.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            '****************'
            '*  Square SD3  *'
            '****************'
            Dim series50 As New Series("SD3L1", ViewType.Line)
            series50.Points.Add(New SeriesPoint(pControl1Mean + 3 * (pControl1SD), pControl2Mean + 3 * (pControl2SD)))
            series50.Points.Add(New SeriesPoint(pControl1Mean - 3 * (pControl1SD), (pControl2Mean + 3 * (pControl2SD))))

            myLineSeriesView = CType(series50.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.Thickness = 2
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash

            series50.Label.TextPattern = "{V}"
            series50.ArgumentScaleType = ScaleType.Numerical
            series50.ValueScaleType = ScaleType.Numerical
            series50.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            Dim series51 As New Series("SD3L2", ViewType.Line)
            series51.Points.Add(New SeriesPoint(pControl1Mean - 3 * (pControl1SD), pControl2Mean + 3 * (pControl2SD)))
            series51.Points.Add(New SeriesPoint(pControl1Mean - 3 * (pControl1SD), (pControl2Mean - 3 * (pControl2SD))))

            myLineSeriesView = CType(series51.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.Thickness = 2
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash

            series51.Label.TextPattern = "{V}"
            series51.ArgumentScaleType = ScaleType.Numerical
            series51.ValueScaleType = ScaleType.Numerical
            series51.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            Dim series52 As New Series("SD3L3", ViewType.Line)
            series52.Points.Add(New SeriesPoint(pControl1Mean - 3 * (pControl1SD), pControl2Mean - 3 * (pControl2SD)))
            series52.Points.Add(New SeriesPoint(pControl1Mean + 3 * (pControl1SD), (pControl2Mean - 3 * (pControl2SD))))

            myLineSeriesView = CType(series52.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.Thickness = 2
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash

            series52.Label.TextPattern = "{V}"
            series52.ArgumentScaleType = ScaleType.Numerical
            series52.ValueScaleType = ScaleType.Numerical
            series52.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            Dim series53 As New Series("SD3L4", ViewType.Line)
            series53.Points.Add(New SeriesPoint(pControl1Mean + 3 * (pControl1SD), pControl2Mean - 3 * (pControl2SD)))
            series53.Points.Add(New SeriesPoint(pControl1Mean + 3 * (pControl1SD), (pControl2Mean + 3 * (pControl2SD))))

            myLineSeriesView = CType(series53.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray
            myLineSeriesView.LineStyle.Thickness = 2
            myLineSeriesView.LineStyle.DashStyle = DashStyle.Dash

            series53.Label.TextPattern = "{V}"
            series53.ArgumentScaleType = ScaleType.Numerical
            series53.ValueScaleType = ScaleType.Numerical
            series53.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False

            '*******************'
            '*  Diagonal Line  *'
            '*******************'
            Dim series60 As New Series("tan45º", ViewType.Line)
            series60.Points.Add(New SeriesPoint(pControl1Mean - 7 * (pControl1SD), pControl2Mean - 7 * (pControl2SD)))
            series60.Points.Add(New SeriesPoint(pControl1Mean + 7 * (pControl1SD), (pControl2Mean + 7 * (pControl2SD))))

            myLineSeriesView = CType(series60.View, LineSeriesView)
            myLineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.False
            myLineSeriesView.Color = Color.Gray   'Color.Red
            myLineSeriesView.LineStyle.Thickness = 2

            series60.Label.TextPattern = "{V}"
            series60.ArgumentScaleType = ScaleType.Numerical
            series60.ValueScaleType = ScaleType.Numerical
            series60.LabelsVisibility = DevExpress.Utils.DefaultBoolean.False


            XrYoudenGraph.Series.AddRange(New Series() {series4, series41, series42, series43, series40, series410, _
                                                        series420, series430, series50, series51, series52, series53, series60})
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 23/09/2014 - BA-1608 ==> ** Added some changes required after activation of Option Strict On (ConvertToSingle)
    '''                                          ** Before drawn the graph, verify the selected Controls have at least a not exclude result
    ''' </remarks>
    Private Sub PrepareYoudenGraph()
        'Multilanguage support
        'Dim currentLanguageGlobal As New GlobalBase
        Dim mCurrentLanguage As String = GlobalBase.GetSessionInfo().ApplicationLanguage
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

        mLabelSD = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SD", mCurrentLanguage)
        mLabelMEAN = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Mean", mCurrentLanguage)
        XrLabelLast.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LAST_RUNPOINT", mCurrentLanguage)

        'The Graph
        XrYoudenGraph.Series.Clear()
        XrYoudenGraph.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False
        XrYoudenGraph.SeriesTemplate.ValueScaleType = ScaleType.Numerical
        XrYoudenGraph.BackColor = Color.White

        Try
            'Get the list of selected Controls
            'Dim mySelectecControlLotList As List(Of OpenQCResultsDS.tOpenResultsRow) = (From a In mControlsDS.tOpenResults _
            '                                                                            Where Not a.IsSelectedNull AndAlso a.Selected _
            '                                                                            Select a).ToList()
            'Dim numOfSelectedCtrls As Integer = mySelectecControlLotList.Count

            'Get the list of selected Controls
            Dim mySelectecControlLotList As List(Of OpenQCResultsDS.tOpenResultsRow) = (From a As OpenQCResultsDS.tOpenResultsRow In mControlsDS.tOpenResults _
                                                                                   Where Not a.IsSelectedNull AndAlso a.Selected = True _
                                                                                      Select a).ToList()
            Dim numOfSelectedCtrls As Integer = mySelectecControlLotList.Count

            'BA-1608 - Verify for each selected Control that it has at least a not excluded result to plot and unselect Controls that not fulfill this conditions
            '          If the number of selected Controls changes, get again the selected Controls (the ones that remain selected)
            If (numOfSelectedCtrls > 0) Then
                Dim validResults As Boolean = False
                For Each selControl As OpenQCResultsDS.tOpenResultsRow In mySelectecControlLotList
                    'Verify if the Control has at least a not excluded result to plot; otherwise, set Selected = False for it
                    validResults = (mResultsDS.tqcResults.ToList.Where(Function(a) a.QCControlLotID = selControl.QCControlLotID AndAlso a.Excluded = False).Count > 0)
                    If (Not validResults) Then selControl.Selected = False
                Next


                If (mControlsDS.tOpenResults.ToList.Where(Function(b) Not b.IsSelectedNull AndAlso b.Selected = True).Count <> numOfSelectedCtrls) Then
                    'The number of selected Controls has changed, get the group of Controls that remains selected (if any) and count them
                    mySelectecControlLotList = (From a As OpenQCResultsDS.tOpenResultsRow In mControlsDS.tOpenResults _
                                           Where Not a.IsSelectedNull AndAlso a.Selected = True _
                                              Select a).ToList()
                    numOfSelectedCtrls = mySelectecControlLotList.Count
                End If
            End If

            If (numOfSelectedCtrls > 0) Then
                If (numOfSelectedCtrls > 2) Then
                    'If there are more than two Control/Lots selected, the last one is unselected
                    mySelectecControlLotList.Last.Selected = False
                    numOfSelectedCtrls -= 1
                End If

                XrYoudenGraph.Series.Add(mySelectecControlLotList.First().ControlNameLotNum, ViewType.Point)
                XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).ShowInLegend = True
                XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).LabelsVisibility = DevExpress.Utils.DefaultBoolean.False
                XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).Label.TextPattern = "{V}"
                XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).ArgumentScaleType = ScaleType.Numerical
                XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).ValueScaleType = ScaleType.Numerical
                XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).View.Color = Color.Black

                Dim myDiagram As XYDiagram = CType(XrYoudenGraph.Diagram, XYDiagram)
                myDiagram.AxisY.ConstantLines.Clear()
                myDiagram.AxisX.ConstantLines.Clear()
                myDiagram.AxisY.WholeRange.Auto = False
                myDiagram.AxisX.WholeRange.Auto = False
                myDiagram.AxisY.VisualRange.Auto = False
                myDiagram.AxisX.VisualRange.Auto = False

                Dim XResultValues As New List(Of Single)
                If (numOfSelectedCtrls = 1) Then
                    If (mySelectecControlLotList.First().IsSDNull) Then
                        'There are not enough Results to drawn the graph
                        mySelectecControlLotList.First().Selected = False
                        XrYoudenGraph.Series.Clear()
                    Else
                        'Drawn the Youden Graph for the selected Control...

                        'Set Margins
                        myDiagram.Margins.Right = 40

                        'Set values for X-Axis and Y-Axis
                        XResultValues = (From a In mResultsDS.tqcResults _
                                         Where a.QCControlLotID = mySelectecControlLotList.First().QCControlLotID AndAlso _
                                               a.ControlNameLotNum = mySelectecControlLotList.First().ControlNameLotNum AndAlso _
                                               Not a.Excluded _
                                         Select a.VisibleResultValue).ToList()

                        myDiagram.AxisX.WholeRange.SetMinMaxValues(Math.Round(mySelectecControlLotList.First().Mean - (3 * mySelectecControlLotList.First().SD), 3) - 1, _
                                                                                      Math.Round(mySelectecControlLotList.First().Mean + (3 * mySelectecControlLotList.First().SD), 3) + 1)
                        myDiagram.AxisX.VisualRange.SetMinMaxValues(Math.Round(mySelectecControlLotList.First().Mean - (3 * mySelectecControlLotList.First().SD), 3) - 1, _
                                                              Math.Round(mySelectecControlLotList.First().Mean + (3 * mySelectecControlLotList.First().SD), 3) + 1)
                        myDiagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True
                        myDiagram.AxisX.Title.Antialiasing = False
                        myDiagram.AxisX.Title.TextColor = Color.Black
                        myDiagram.AxisX.Title.Alignment = StringAlignment.Center
                        myDiagram.AxisX.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
                        myDiagram.AxisX.Title.Text = mySelectecControlLotList.First().ControlName

                        myDiagram.AxisY.WholeRange.SetMinMaxValues(Math.Round(mySelectecControlLotList.First().Mean - (3 * mySelectecControlLotList.First().SD), 3) - 1, _
                                                                                      Math.Round(mySelectecControlLotList.First().Mean + (3 * mySelectecControlLotList.First().SD), 3) + 1)
                        myDiagram.AxisY.VisualRange.SetMinMaxValues(Math.Round(mySelectecControlLotList.First().Mean - (3 * mySelectecControlLotList.First().SD), 3) - 1, _
                                                              Math.Round(mySelectecControlLotList.First().Mean + (3 * mySelectecControlLotList.First().SD), 3) + 1)
                        myDiagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True
                        myDiagram.AxisY.Title.Antialiasing = False
                        myDiagram.AxisY.Title.TextColor = Color.Black
                        myDiagram.AxisY.Title.Alignment = StringAlignment.Center
                        myDiagram.AxisY.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
                        myDiagram.AxisY.Title.Text = mySelectecControlLotList.Last().ControlName

                        'Create the graph squares
                        CreateSquares(mySelectecControlLotList.First().Mean, mySelectecControlLotList.First().SD, _
                                      mySelectecControlLotList.Last().Mean, mySelectecControlLotList.Last().SD)

                        'Drawn the points in the graph
                        For Each qcResultRow As QCResultsDS.tqcResultsRow In From elem In mResultsDS.tqcResults _
                                                                             Where Not elem.Excluded AndAlso _
                                                                                   elem.ControlNameLotNum = mySelectecControlLotList.First().ControlNameLotNum
                            XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).Points.Add(New SeriesPoint(qcResultRow.VisibleResultValue, _
                                                                                                                                qcResultRow.VisibleResultValue))
                            XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).Points(XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).Points.Count - 1).Tag = qcResultRow.CalcRunNumber
                        Next

                        'Create cross lines with the Control Mean
                        CreateConstantLineAxisX(mySelectecControlLotList.First().Mean.ToString("F2"), myDiagram, mySelectecControlLotList.First().Mean, Color.Gray)
                        CreateConstantLineAxisY(mySelectecControlLotList.First().Mean.ToString("F2"), myDiagram, Convert.ToSingle(mySelectecControlLotList.First().Mean), Color.Gray)
                    End If

                ElseIf (numOfSelectedCtrls = 2) Then
                    If (mySelectecControlLotList.First().IsSDNull OrElse mySelectecControlLotList.Last().IsSDNull) Then
                        'There are not enough Results to drawn the graph...

                        mySelectecControlLotList.First().Selected = False
                        mySelectecControlLotList.Last().Selected = False

                        XrYoudenGraph.Series.Clear()
                    Else
                        'Drawn the Youden Graph for the pair of selected Controls...

                        'Set Margins
                        myDiagram.Margins.Right = 40

                        'Set values to X-Axis
                        XResultValues = (From a In mResultsDS.tqcResults _
                                         Where a.QCControlLotID = mySelectecControlLotList.First().QCControlLotID AndAlso _
                                               a.ControlNameLotNum = mySelectecControlLotList.First().ControlNameLotNum AndAlso _
                                               Not a.Excluded _
                                         Select a.VisibleResultValue).ToList()

                        Dim MinValue As Single = Convert.ToSingle(Math.Round(mySelectecControlLotList.First().Mean - (3 * mySelectecControlLotList.First().SD), 3))
                        If (MinValue > XResultValues.Min) Then MinValue = XResultValues.Min

                        Dim MaxValue As Single = Convert.ToSingle(Math.Round(mySelectecControlLotList.First().Mean + (3 * mySelectecControlLotList.First().SD), 3))
                        If (MaxValue < XResultValues.Max) Then MaxValue = XResultValues.Max

                        myDiagram.AxisX.WholeRange.SetMinMaxValues(MinValue - 1, MaxValue + 1)
                        myDiagram.AxisX.VisualRange.SetMinMaxValues(MinValue - 1, MaxValue + 1)
                        myDiagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True
                        myDiagram.AxisX.Title.Antialiasing = False
                        myDiagram.AxisX.Title.TextColor = Color.Black
                        myDiagram.AxisX.Title.Alignment = StringAlignment.Center
                        myDiagram.AxisX.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
                        myDiagram.AxisX.Title.Text = mySelectecControlLotList.First().ControlName

                        'Set values to Y-Axis
                        Dim YResultValues As List(Of Single) = (From a In mResultsDS.tqcResults _
                                                                Where a.QCControlLotID = mySelectecControlLotList.Last().QCControlLotID AndAlso _
                                                                      a.ControlName = mySelectecControlLotList.Last().ControlName AndAlso _
                                                                      Not a.Excluded _
                                                                Select a.VisibleResultValue).ToList()

                        MinValue = Convert.ToSingle(Math.Round(mySelectecControlLotList.Last().Mean - (3 * mySelectecControlLotList.Last().SD), 3))
                        If (MinValue > YResultValues.Min) Then MinValue = YResultValues.Min

                        MaxValue = Convert.ToSingle(Math.Round(mySelectecControlLotList.Last().Mean + (3 * mySelectecControlLotList.Last().SD), 3))
                        If (MaxValue < YResultValues.Max) Then MaxValue = YResultValues.Max

                        myDiagram.AxisY.WholeRange.SetMinMaxValues(MinValue - 1, MaxValue + 1)
                        myDiagram.AxisY.VisualRange.SetMinMaxValues(MinValue - 1, MaxValue + 1)
                        myDiagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True
                        myDiagram.AxisY.Title.Antialiasing = False
                        myDiagram.AxisY.Title.TextColor = Color.Black
                        myDiagram.AxisY.Title.Alignment = StringAlignment.Center
                        myDiagram.AxisY.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
                        myDiagram.AxisY.Title.Text = mySelectecControlLotList.Last().ControlName

                        'Create cross lines with the Mean of selected Controls
                        CreateConstantLineAxisX(mySelectecControlLotList.First().Mean.ToString("F2"), myDiagram, mySelectecControlLotList.First().Mean, Color.Gray)
                        CreateConstantLineAxisY(mySelectecControlLotList.Last().Mean.ToString("F2"), myDiagram, Convert.ToSingle(mySelectecControlLotList.Last().Mean), Color.Gray)

                        'Create the graph squares
                        CreateSquares(mySelectecControlLotList.First().Mean, mySelectecControlLotList.First().SD, _
                                      mySelectecControlLotList.Last().Mean, mySelectecControlLotList.Last().SD)

                        'Drawn the points in the graph
                        Dim SecondControlValue As New List(Of QCResultsDS.tqcResultsRow)
                        For Each qcResultRow As QCResultsDS.tqcResultsRow In From elem In mResultsDS.tqcResults _
                                                                             Where Not elem.Excluded AndAlso _
                                                                                   elem.ControlNameLotNum = mySelectecControlLotList.First().ControlNameLotNum
                            'Get result values for second control.
                            SecondControlValue = (From a In mResultsDS.tqcResults _
                                                  Where a.QCControlLotID = mySelectecControlLotList.Last().QCControlLotID AndAlso _
                                                        a.CalcRunNumber = qcResultRow.CalcRunNumber _
                                                  Select a).ToList()

                            If (SecondControlValue.Count > 0) Then
                                XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).Points.Add(New SeriesPoint(qcResultRow.VisibleResultValue, _
                                                                                                                                    SecondControlValue.First().VisibleResultValue))
                                XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).Points(XrYoudenGraph.Series(mySelectecControlLotList.First().ControlNameLotNum).Points.Count - 1).Tag = qcResultRow.CalcRunNumber
                            End If
                        Next
                    End If
                Else
                    myDiagram = CType(XrYoudenGraph.Diagram, XYDiagram)
                    myDiagram.AxisY.ConstantLines.Clear()
                    myDiagram.AxisX.ConstantLines.Clear()

                    myDiagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.False
                    myDiagram.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.False

                End If
            End If

        Catch ex As Exception
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 23/09/2014 - BA-1608 ==> Added some changes required after activation of Option Strict On (ConvertToInt32)
    ''' </remarks>
    Private Sub XrYoudenGraph_CustomDrawSeriesPoint(ByVal sender As System.Object, ByVal e As DevExpress.XtraCharts.CustomDrawSeriesPointEventArgs) Handles XrYoudenGraph.CustomDrawSeriesPoint
        Try
            Dim myArgumentValue As String = e.SeriesPoint.Argument.ToString

            'Youden Graph
            Dim myPoint As SeriesPoint = TryCast(e.SeriesPoint, SeriesPoint)
            If (Not myPoint Is Nothing AndAlso Not myPoint.Tag Is Nothing) Then
                e.SeriesDrawOptions.Color = Color.Black
                With CType(e.SeriesDrawOptions, PointDrawOptions).Marker
                    'Validate if the n on the tag property is the last to change the icon
                    If (Convert.ToInt32(e.Series.Points(e.Series.Points.Count - 1).Tag) = DirectCast(myPoint.Tag, Integer)) Then
                        '.FillStyle.FillMode = FillMode.Solid
                        .Kind = MarkerKind.Star
                        .Size = 14
                    Else
                        '.FillStyle.FillMode = FillMode.Solid
                        .Kind = MarkerKind.Cross
                        .Size = 7
                    End If
                End With
            End If
        Catch ex As Exception
        End Try
    End Sub
#End Region
End Class
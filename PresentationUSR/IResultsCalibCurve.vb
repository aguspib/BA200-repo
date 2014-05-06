'Created by AG 10/08/2010 - Based on ResultFormAG

Option Explicit On
Option Strict On

Imports System.Text
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Calculations 'AG 26/07/2010
Imports Biosystems.Ax00.PresentationCOM
Imports DevExpress.XtraCharts
Imports Biosystems.Ax00.CommunicationsSwFw

Public Class IResultsCalibCurve

#Region "Declarations"

    Private NoImage As Byte() = Nothing
    Private ClassImage As Byte() = Nothing
    Private ABS_GRAPH As Byte() = Nothing
    Private AVG_ABS_GRAPH As Byte() = Nothing

    ' DL 28/06/2011
    Private m_HotTrackedPoint As SeriesPoint
    Private myLabelText As String = String.Empty
    ' DL 28/06/2011

    Dim SampleIconList As ImageList

    Private StrikeFont As Font = New Font("Verdana", 8.25!, FontStyle.Strikeout, GraphicsUnit.Point, CType(0, Byte))
    Private RegularFont As Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
    'Private SeeRemsFont As Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte)) ' DL 8/6/2011 Private SeeRemsFont As Font = New Font("Verdana", 18.0!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))

    Private RegularBkColor As Color = Color.White
    Private StrikeBkColor As Color = Color.LightGray

    Private RegularForeColor As Color = Color.Black
    Private StrikeForeColor As Color = Color.Gray

    Private CollapseColName As String = "Collapse"
    Dim IsColCollapsed As New Dictionary(Of String, Boolean)
    Dim ProcessEvent As Boolean = True

    Private CurveResultsID As Integer = 0 'For current WS results use
    Private HistOrderTestID As Integer = 0 'AG 16/10/2012 - For historical mode use

    Private ChartRerunNumber As Integer = 0
    Private ChartTestName As String = String.Empty
    Private ChartSampleType As String = String.Empty
    Private CurveGrowthType As String = String.Empty
    Private CurveType As String = String.Empty
    Private CurveAxisXType As String = String.Empty
    Private CurveAxisYType As String = String.Empty
    Private DecimalsAllowed As Integer = 0
    Private MonotonousCurve As Boolean = True

    'Private WithEvents ResultsChart As New bsResultsChart() 'SG 30/08/2010
    Private WithEvents ResultsChart As bsResultsChart 'RH 13/12/2010 Remove New because it creates an object that wont be used.

    Private PointAbsorbance As New List(Of Single)
    Private PointConcentration As New List(Of Single)

    Dim ComboSelectedIndex As New Dictionary(Of String, Integer)

    Private LanguageID As String

    Private labelOpenAbsorbanceCurve As String = String.Empty
    Private labelSampleClasessBlank As String = String.Empty
    Private labelCurveCalibRerunNumber As String = String.Empty
    Private labelCalibCurveNotCalculated As String = String.Empty

    Private mdiAnalyzerCopy As AnalyzerManager 'AG 22/06/2012

#End Region

#Region "Attributes"
    Private WorkSessionIDField As String
    Private AnalyzerIDField As String
    Private AverageResultsDSField As New ResultsDS
    Private ExecutionsResultsDSField As New ExecutionsDS
    Private TestSelectedTextField As String = String.Empty
    Private LotSelectedTextField As String = String.Empty
    Private FullTestNameField As String = String.Empty
    Private CalibratorSelectedTextField As String = String.Empty
    Private SampleTypeSelectedTextField As String = String.Empty
    Private AcceptedRerunNumberField As Integer = 0
    Private HistoricalModeField As Boolean = False 'AG 16/10/2012

#End Region

#Region "Properties"
    Public Property ActiveWorkSession() As String
        Get
            Return WorkSessionIDField
        End Get
        Set(ByVal value As String)
            WorkSessionIDField = value
        End Set
    End Property

    Public Property ActiveAnalyzer() As String
        Get
            Return AnalyzerIDField
        End Get
        Set(ByVal value As String)
            AnalyzerIDField = value
        End Set
    End Property

    Public Property AverageResults() As ResultsDS
        Get
            Return AverageResultsDSField
        End Get
        Set(ByVal value As ResultsDS)
            AverageResultsDSField = value
        End Set
    End Property

    Public Property ExecutionResults() As ExecutionsDS
        Get
            Return ExecutionsResultsDSField
        End Get
        Set(ByVal value As ExecutionsDS)
            ExecutionsResultsDSField = value
        End Set
    End Property

    Public Property SelectedTestName() As String
        Get
            Return TestSelectedTextField
        End Get
        Set(ByVal value As String)
            TestSelectedTextField = value
        End Set
    End Property

    Public Property SelectedFullTestName() As String
        Get
            Return FullTestNameField
        End Get
        Set(ByVal value As String)
            FullTestNameField = value
        End Set
    End Property


    Public Property SelectedLot() As String
        Get
            Return LotSelectedTextField
        End Get
        Set(ByVal value As String)
            LotSelectedTextField = value
        End Set
    End Property

    Public Property SelectedCalibrator() As String
        Get
            Return CalibratorSelectedTextField
        End Get
        Set(ByVal value As String)
            CalibratorSelectedTextField = value
        End Set
    End Property

    Public Property SelectedSampleType() As String
        Get
            Return SampleTypeSelectedTextField
        End Get
        Set(ByVal value As String)
            SampleTypeSelectedTextField = value
        End Set
    End Property

    Public Property AcceptedRerunNumber() As Integer
        Get
            Return AcceptedRerunNumberField
        End Get
        Set(ByVal value As Integer)
            AcceptedRerunNumberField = value
        End Set
    End Property

    Public WriteOnly Property HistoricalMode() As Boolean 'AG 16/10/2012
        Set(ByVal value As Boolean)
            HistoricalModeField = value
        End Set
    End Property

#End Region

#Region "Events"

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 10/11/2010 
    ''' </remarks>
    Private Sub CurveResultForm_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                bsExitButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CurveResultForm_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CurveResultForm_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen loading event
    ''' </summary>
    Private Sub CurveResultForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            InitializeScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CurveResultForm_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CurveResultForm_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen closing event, executed when Exit button is clicked
    ''' </summary>
    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            Me.Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Add tooltip to No column
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 22/07/2011
    ''' </remarks>
    Private Sub CalibratorsDataGridView_CellFormatting(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles CalibratorsDataGridView.CellFormatting

        Try
            If e.RowIndex > -1 Then
                Dim ColIndexNo As Integer = CalibratorsDataGridView.Columns("No").Index
                If e.ColumnIndex = ColIndexNo AndAlso (e.Value IsNot Nothing) Then
                    Dim ColIndexRemarks As Integer = CalibratorsDataGridView.Columns("Remarks").Index

                    If Not CalibratorsDataGridView.Rows(e.RowIndex).Cells(ColIndexRemarks).Value Is Nothing _
                    AndAlso Not CalibratorsDataGridView.Rows(e.RowIndex).Cells(ColIndexRemarks).Value Is String.Empty Then
                        CalibratorsDataGridView.Rows(e.RowIndex).Cells(e.ColumnIndex).ToolTipText = CalibratorsDataGridView.Rows(e.RowIndex).Cells(ColIndexRemarks).Value.ToString
                    End If

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CalibratorsDataGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Change the cursor image according the type of cell in which the mouse has been placed
    ''' </summary>
    Private Sub CalibratorsDataGridView_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles CalibratorsDataGridView.CellMouseEnter
        Dim dgv As BSDataGridView = CType(sender, BSDataGridView)

        Try
            Select Case (e.ColumnIndex)
                Case (dgv.Columns(CollapseColName).Index)
                    If Not HistoricalModeField Then 'AG 17/10/2012
                        If (Not CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).IsEnabled) Then Return
                        If (e.RowIndex < 0) Then
                            dgv.Cursor = Cursors.Hand
                        Else
                            If (IsSubHeader(dgv, e.RowIndex)) Then
                                dgv.Cursor = Cursors.Hand
                            Else
                                dgv.Cursor = Cursors.Default
                            End If
                        End If
                    End If

                Case (dgv.Columns("Graph").Index)
                    If (e.RowIndex < 0) Then
                        dgv.Cursor = Cursors.Default
                    Else
                        If (Not dgv("Graph", e.RowIndex).Value Is NoImage) Then
                            dgv.Cursor = Cursors.Hand
                        End If
                    End If

                Case Else
                    dgv.Cursor = Cursors.Default
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalibratorsDataGridView_CellMouseEnter", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalibratorsDataGridView_CellMouseEnter", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Change the cursor image when the mouse leaves a grid cell
    ''' </summary>
    Private Sub CalibratorsDataGridView_CellMouseLeave(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles CalibratorsDataGridView.CellMouseLeave
        Dim dgv As BSDataGridView = CType(sender, BSDataGridView)
        Try
            If (e.RowIndex > 0) Then
                dgv.Cursor = Cursors.Default
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalibratorsDataGridView_CellMouseLeave", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalibratorsDataGridView_CellMouseLeave", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Manages the click in a grid cell according its type 
    ''' </summary>
    ''' <remarks>
    ''' Created by:
    ''' Modified by: AG 02/08/2010
    '''              AG 08/08/2010
    '''              SG 30/08/2010 - Show the Results Graph when the cell clicked is the one containing the Graph icon
    '''              SA 15/11/2010 - Message with Spanish text has been commented
    ''' </remarks>
    Private Sub CalibratorsDataGridView_CellMouseClick(ByVal sender As System.Object, _
                                                       ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles CalibratorsDataGridView.CellMouseClick
        'Dim key As String = String.Empty
        Dim key As String = SelectedTestName & "_CALIB"
        Dim resultRow As ResultsDS.vwksResultsRow
        Dim executionRow As ExecutionsDS.vwksWSExecutionsResultsRow

        Try
            If sender Is Nothing Then Return

            Dim dgv As BSDataGridView = CType(sender, BSDataGridView)

            'Select Case dgv.Name
            '    Case CalibratorsDataGridView.Name
            '        key = TestSelectedText & "_CALIB"
            '    Case Else
            '        Return
            'End Select

            If (e.RowIndex = -1) AndAlso (e.ColumnIndex = dgv.Columns(CollapseColName).Index) Then
                If Not HistoricalModeField Then 'AG 17/10/2012
                    IsColCollapsed(key) = CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).IsCollapsed
                    'AG 02/08/2010 - Update collapse (general)
                    If Not HistoricalModeField Then 'AG 17/10/2012
                        If (Not Me.UpdateCollapse(True, IsColCollapsed(key), dgv.Name)) Then
                            IsColCollapsed(key) = Not IsColCollapsed(key)
                        End If
                    Else
                        'Nothing to do in historical mode
                    End If
                    'END AG 02/08/2010
                End If
                Return
            End If

            If e.RowIndex < 0 Then Return
            Dim changeInUseValue As Boolean = False 'AG 08/08/2010

            Select Case (e.ColumnIndex)
                Case (dgv.Columns(CollapseColName).Index)
                    If Not HistoricalModeField Then 'AG 17/10/2012
                        If (IsSubHeader(dgv, e.RowIndex) And (Not dgv.Rows(e.RowIndex).Tag Is Nothing)) Then
                            resultRow = CType(dgv.Rows(e.RowIndex).Tag, ResultsDS.vwksResultsRow)
                            resultRow.Collapsed = CType(dgv(CollapseColName, e.RowIndex), bsDataGridViewCollapseCell).IsCollapsed
                            IsColCollapsed.Remove(key)

                            If Not HistoricalModeField Then 'AG 17/10/2012
                                'AG 02/08/2010 - update field collapse (ESPECIFIC OrderTestID - RerunNumber - MultiPointNumber)
                                If (Not Me.UpdateCollapse(False, resultRow.Collapsed, Nothing, resultRow)) Then
                                    resultRow.Collapsed = Not resultRow.Collapsed
                                End If
                                'END AG 02/08/2010
                            Else
                                'Nothing to do in historical mode
                            End If
                        End If
                    End If

                Case (dgv.Columns("Graph").Index)
                    'Dim myGlobalDataTO As New GlobalDataTO
                    'Dim myExecutionsDelegate As New ExecutionsDelegate

                    If (Not IsSubHeader(dgv, e.RowIndex)) Then
                        executionRow = CType(dgv.Rows(e.RowIndex).Tag, ExecutionsDS.vwksWSExecutionsResultsRow)

                        If executionRow.RowState <> DataRowState.Deleted Then
                            If Not HistoricalModeField Then 'AG 17/10/2012
                                ShowResultsChart(executionRow.ExecutionID, executionRow.OrderTestID, executionRow.RerunNumber, executionRow.MultiItemNumber, executionRow.ReplicateNumber)
                            Else
                                'In historical the replicates are not shown - v1
                            End If

                        End If


                    Else
                        resultRow = CType(dgv.Rows(e.RowIndex).Tag, ResultsDS.vwksResultsRow)
                        If resultRow.RowState <> DataRowState.Deleted Then
                            If Not HistoricalModeField Then
                                ShowResultsChart(-1, resultRow.OrderTestID, resultRow.RerunNumber, resultRow.MultiPointNumber, -1)
                            Else
                                'In  historical the abs(t) graf is not adapted yet - v1
                            End If
                        End If

                        'End If

                    End If

                Case Else
                    If (Not IsSubHeader(dgv, e.RowIndex)) Then
                        'AG 22/06/2012 - TO CONFIRM
                        'If mdiAnalyzerCopy.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then changeInUseValue = True 'AG 22/06/2012 - change in use replicates value is allowed only out of running
                        changeInUseValue = True
                    End If
            End Select

            'AG 08/08/2010 - Depending the clicked cell then call the change in use value
            If (changeInUseValue) Then
                If (ProcessEvent) Then 'If not current process
                    'ChangeInUseFlagReplicate(dgv, e.RowIndex)
                    If Not HistoricalModeField Then 'AG 16/10/2012
                        ChangeInUseFlagReplicateNEW(dgv, e.RowIndex)
                    End If
                End If
            End If
            'END AG 08/08/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CalibratorsDataGridView_CellMouseClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CalibratorsDataGridView_CellMouseClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Recalculates and redraw the Calibration Curve when the type of curve, the type of X-Axis or the type of
    ''' Y-Axis is changed
    ''' </summary>
    Private Sub CurveCombos_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CalibrationCurveCombo.SelectedIndexChanged, _
                                                                                                                     XAxisCombo.SelectedIndexChanged, _
                                                                                                                     YAxisCombo.SelectedIndexChanged
        Try
            'AG 03/09/2010
            If Not ProcessEvent Then Return

            Dim myCombo As ComboBox = CType(sender, ComboBox)
            Dim SelectedIndex As Integer = myCombo.SelectedIndex

            If ComboSelectedIndex(myCombo.Name) = SelectedIndex Then Return
            ComboSelectedIndex(myCombo.Name) = SelectedIndex

            'Recalculate the curve
            Dim resultData As GlobalDataTO
            'resultData = Me.RecalculateCurveAfterDefinitionChanges()
            If Not HistoricalModeField Then 'AG 16/10/2012
                resultData = Me.RecalculateCurveAfterDefinitionChangesNEW()
            End If
            'END AG 03/09/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CurveCombos_SelectedIndexChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CurveCombos_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Recalculates and redraw the Calibration Curve when the type of reaction curve is changed
    ''' </summary>
    Private Sub CurveReactionType_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsIncreasingRadioButton.CheckedChanged

        Try
            'AG 03/09/2010
            If Not ProcessEvent Then Return

            'Recalculate the curve
            Dim resultData As New GlobalDataTO
            'resultData = Me.RecalculateCurveAfterDefinitionChanges()
            If Not HistoricalModeField Then 'AG 16/10/2012
                resultData = Me.RecalculateCurveAfterDefinitionChangesNEW()
            End If
            'END AG 03/09/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CurveReactionType_CheckedChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CurveReactionType_CheckedChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Remove the Result Graph from the graph area
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 30/08/2010
    ''' </remarks>
    Private Sub ResultsChart_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResultsChart.Validated
        Try
            RemoveResultsChart()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ResultsChart_Validated ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ResultsChart_Validated", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Remove the Result Graph from the graph area
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 30/08/2010
    ''' </remarks>
    Private Sub ResultsChart_Exit() Handles ResultsChart.ExitRequest
        Try
            RemoveResultsChart()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ResultsChart_Exit ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ResultsChart_Exit", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Generate the report when the Print Button is clicked
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 20/10/2010
    ''' </remarks>
    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            'AG 20/12/2010 - Temporally commented
            'If (CalibratorsDataGridView.Rows.Count > 1) Then 'Take the Row Header into account
            '    Dim Printer As DGVPrinter = New DGVPrinter
            '    Printer.Title = Me.Text
            '    Printer.SubTitle = Today.ToShortDateString()
            '    Printer.SubTitleFormatFlags = StringFormatFlags.LineLimit Or StringFormatFlags.NoClip
            '    Printer.PageNumbers = True
            '    Printer.PageNumberInHeader = False
            '    Printer.PorportionalColumns = True
            '    Printer.HeaderCellAlignment = StringAlignment.Near
            '    Printer.Footer = "*Biosystems AX00 Automatic Analyser*"
            '    Printer.FooterSpacing = 15

            '    Printer.TitleSpacing = 10
            '    Printer.SubTitleSpacing = 40
            '    Printer.ShowTotalPageNumber = True

            '    Printer.printDocument.DocumentName = Printer.Title
            '    Printer.ColumnWidths.Add("SeeRems", 0)
            '    Printer.ColumnWidths.Add("Remarks", 250)

            '    'Convert CalibratorsDataGridView to DataGridView and print it
            '    Dim TmpDataGridView As DataGridView = GetNewCalibratorsDataGridView()

            '    For i As Integer = 1 To CalibratorsDataGridView.Columns.Count - 1
            '        If Not TypeOf (CalibratorsDataGridView.Columns(i)) Is DataGridViewImageColumn Then
            '            TmpDataGridView.Columns.Add(CType(CalibratorsDataGridView.Columns(i).Clone(), DataGridViewColumn))
            '        End If
            '    Next

            '    Dim k As Integer = -1

            '    For Each row As DataGridViewRow In CalibratorsDataGridView.Rows
            '        TmpDataGridView.Rows.Add()
            '        k += 1
            '        Dim j As Integer = -1
            '        For i As Integer = 1 To CalibratorsDataGridView.Columns.Count - 1
            '            If Not TypeOf (CalibratorsDataGridView.Columns(i)) Is DataGridViewImageColumn Then
            '                j += 1
            '                TmpDataGridView(j, k).Value = CalibratorsDataGridView(i, k).Value
            '            End If
            '        Next

            '        TmpDataGridView.Rows(k).DefaultCellStyle.BackColor = CalibratorsDataGridView.Rows(k).DefaultCellStyle.BackColor
            '        TmpDataGridView.Rows(k).DefaultCellStyle.ForeColor = CalibratorsDataGridView.Rows(k).DefaultCellStyle.ForeColor
            '    Next

            '    'The chart image
            '    'chart1.Printing.Print(False)
            '    Dim ChartImage As DGVPrinter.ImbeddedImage = New DGVPrinter.ImbeddedImage
            '    Dim bmp As Bitmap = New Bitmap(chart1.Width, chart1.Height)
            '    chart1.DrawToBitmap(bmp, New Rectangle(0, 0, chart1.Width, chart1.Height))
            '    ChartImage.theImage = bmp
            '    ChartImage.ImageX = 160
            '    ChartImage.ImageY = 140
            '    ChartImage.ImageLocation = DGVPrinter.Location.Absolute
            '    Printer.TitleSpacing = 10
            '    Printer.SubTitleSpacing = 550
            '    Printer.ImbeddedImageList.Add(ChartImage)

            '    Printer.PrintDataGridView(TmpDataGridView)
            'Else
            '    ShowMessage(Me.Name & ".PrintReport", GlobalEnumerates.Messages.NO_DATA_TO_PRINT.ToString())
            'End If

            'AG 30/04/2014 - #1608 if informed use ReportName instead of TestName
            'XRManager.ShowResultsCalibCurveReport(ActiveAnalyzer, ActiveWorkSession, SelectedTestName, AcceptedRerunNumber)
            Dim myList As List(Of ResultsDS.vwksResultsRow) = (From row In AverageResultsDSField.vwksResults _
                                           Where row.TestName = SelectedTestName AndAlso row.SampleType = SampleTypeSelectedTextField _
                                           AndAlso Not row.IsTestLongNameNull Select row).ToList
            Dim myTestReportName As String = ""
            If myList.Count > 0 AndAlso Not myList(0).IsTestLongNameNull Then
                myTestReportName = myList(0).TestLongName
            End If
            XRManager.ShowResultsCalibCurveReport(ActiveAnalyzer, ActiveWorkSession, SelectedTestName, AcceptedRerunNumber, myTestReportName)
            myList.Clear()
            myList = Nothing

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsPrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsPrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "General Private Methods"

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>    ''' 
    ''' <remarks>
    ''' '<param name="pLanguageID"> The current Language of Application</param>
    ''' Created by:  PG 14/10/2010
    ''' Modified by: PG 18/10/2010 - Added the Text for Graph axis
    '''              RH 20/10/2010 - Removed the LanguageID parameter. Now it is a class property.
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels.....
            ' DL 28/06/2011
            bsCurveResultTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibration_Results", LanguageID)
            bsCalibrationCurveTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibrationCurve", LanguageID)

            'bsCalibrationCurveLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibration_Results", LanguageID) & ":"
            bsCalibrationCurveLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveType", LanguageID) & ":" 'AG 18/10/2012

            bsCalibratorNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibratorName", LanguageID) & ":"
            bsLotLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", LanguageID) & ":"
            bsTestNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", LanguageID) & ":"
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", LanguageID) & ":"
            bsXAxisLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AxisX", LanguageID) & ":"
            bsYAxisLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AxisY", LanguageID) & ":"
            bsDecreasingRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Decreasing", LanguageID)
            bsIncreasingRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Increasing", LanguageID)
            TypeGroupbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", LanguageID)

            ' For Tooltips...
            bsProgTestToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", LanguageID))
            bsProgTestToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", LanguageID))

            'PG 18/10/10 For Graph Axis
            'chart1.ChartAreas(0).Axes(0).Title = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Concentration_Long", LanguageID)
            'chart1.ChartAreas(0).Axes(1).Title = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", LanguageID)


            Dim diagram As XYDiagram = CType(DXChartControl.Diagram, XYDiagram)
            diagram.AxisY.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", LanguageID)
            diagram.AxisX.Title.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Concentration_Long", LanguageID)

            'Me.Text = "*Calibration Curve Results*"
            'Me.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_CurveResultsScreen", LanguageID)

            'RH 30/05/2011
            labelOpenAbsorbanceCurve = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ABSORBANCE_CURVE", LanguageID) '"Open Absorbance Curve"
            labelSampleClasessBlank = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_BLANK", LanguageID)
            labelCurveCalibRerunNumber = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveCalib_RerunNumber", LanguageID)
            labelCalibCurveNotCalculated = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_CALIBCURVE_NOT_CALCULATED", LanguageID)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the list of Order Tests Results from the Executions
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/15/2010
    ''' Modified by: AG 04/09/2010 - Added optional parameter connection
    ''' Modified by RH 27/05/2011 Remove connection parameter
    ''' </remarks>
    Private Sub LoadExecutionsResults()
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            Dim resultData As GlobalDataTO
            resultData = DAOBase.GetOpenDBConnection(Nothing)

            If (Not resultData.HasError) Then
                dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                If (Not dbConnection Is Nothing) Then
                    Dim myExecutionDelegate As New ExecutionsDelegate
                    resultData = myExecutionDelegate.GetWSExecutionsResults(dbConnection, ActiveAnalyzer, ActiveWorkSession)

                    If (Not resultData.HasError) Then
                        ExecutionResults = CType(resultData.SetDatos, ExecutionsDS)

                        AverageResults.vwksResults.Clear()
                        AverageResults.vwksResultsAlarms.Clear()         'AG 28/07/2010 - Clear average alarms
                        ExecutionResults.vwksWSExecutionsAlarms.Clear() 'AG 28/07/2010 - Clear executions alarms

                        Dim OrderTestList As List(Of Integer) = (From row In ExecutionResults.vwksWSExecutionsResults _
                                                               Select row.OrderTestID Distinct).ToList()

                        Dim myResultsDelegate As New ResultsDelegate
                        For Each orderTestID As Integer In OrderTestList
                            'Get all results for current OrderTestID.
                            resultData = myResultsDelegate.GetResults(dbConnection, orderTestID)

                            If (Not resultData.HasError) Then
                                For Each resultRow As ResultsDS.vwksResultsRow In CType(resultData.SetDatos, ResultsDS).vwksResults.Rows
                                    AverageResults.vwksResults.ImportRow(resultRow)
                                Next
                            Else
                                ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
                                Exit For
                            End If
                        Next

                        'Get Average Result Alarms
                        resultData = myResultsDelegate.GetResultAlarms(dbConnection)
                        If (Not resultData.HasError) Then
                            For Each resultRow As ResultsDS.vwksResultsAlarmsRow In CType(resultData.SetDatos, ResultsDS).vwksResultsAlarms.Rows
                                AverageResults.vwksResultsAlarms.ImportRow(resultRow)
                            Next
                        Else
                            ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
                        End If

                        'Get Execution Result Alarms
                        resultData = myExecutionDelegate.GetWSExecutionResultAlarms(dbConnection)
                        If (Not resultData.HasError) Then
                            For Each resultRow As ExecutionsDS.vwksWSExecutionsAlarmsRow In CType(resultData.SetDatos, ExecutionsDS).vwksWSExecutionsAlarms.Rows
                                ExecutionResults.vwksWSExecutionsAlarms.ImportRow(resultRow)
                            Next
                        Else
                            ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
                        End If
                    Else
                        ShowMessage(Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
                    End If
                End If
            Else
                ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadExecutionsResults", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadExecutionsResults", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            If (Not dbConnection Is Nothing) Then dbConnection.Close()

        End Try
    End Sub

    ''' <summary>
    ''' Initialize all the controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/12/2010
    ''' Modified by: PG 14/10/2010 - Get current Language
    '''              RH 20/10/2010 - Removed the currentLanguage local variable/parameter.
    '''                              Initialized the LanguageID new class property. 
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            LanguageID = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) 'AG 22/06/2012 - Use the same AnalyzerManager as the MDI
            End If

            GetScreenLabels()
            PrepareButtons()

            'LoadExecutionsResults()
            InitializeCalibratorsGrid()

            'RH 27/05/2011 Remove every reference to a connection
            ''AG 06/09/2010 - Open and use only one connection
            ''UpdateCalibratorsMuliItemDataGrid(TestSelectedName)
            ''LoadCombos()
            ''DrawChart()
            'resultData = DAOBase.GetOpenDBConnection(Nothing)
            'If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
            '    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
            '    If (Not dbConnection Is Nothing) Then
            '        'UpdateCalibratorsMuliItemDataGrid(TestSelectedName)
            '        UpdateCurrentResultsGrid(dbConnection, False)
            '        LoadCombos(dbConnection)
            '        DrawChart() 'PG 18/10/2010 
            '    End If
            'End If
            ''END AG 06/09/2010

            TestDescriptionTextBox.Text = SelectedFullTestName
            BsLotTextBox.Text = LotSelectedTextField
            BsCalibratorNameTextBox.Text = CalibratorSelectedTextField
            bsSampleTypeTextBox.Text = SelectedSampleType

            'RH 27/05/2011
            UpdateCurrentResultsGrid(False)
            LoadCombos()
            DrawChart()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    Private Sub DXChartControl_ObjectHotTracked(ByVal sender As Object, ByVal e As HotTrackEventArgs) Handles DXChartControl.ObjectHotTracked
        Try

            Dim mySerie As Series = TryCast(e.Object, Series)

            If Not mySerie Is Nothing AndAlso mySerie.Name = "Series 2" Then
                Dim myPoint As SeriesPoint = TryCast(e.AdditionalObject, SeriesPoint)

                If Not myPoint Is Nothing Then
                    Me.Cursor = Cursors.Hand
                    Dim myToolTip As String = myPoint.Values(0).ToString("#0.####") 'myPoint.NumericalArgument.ToString & ":" & myPoint.Values(0).ToString("#0.####")

                    ToolTipController1.ShowHint(myToolTip)
                Else
                    ToolTipController1.HideHint()
                End If
            Else
                Cursor = Cursors.Default
                ToolTipController1.HideHint()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DXChartControl_ObjectHotTracked", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DXChartControl_ObjectHotTracked", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    Private Sub DXChartControl_CustomDrawSeriesPoint(ByVal sender As Object, ByVal e As CustomDrawSeriesPointEventArgs) Handles DXChartControl.CustomDrawSeriesPoint

        If e.Series.Name = "Series 2" Then
            If e.SeriesPoint Is m_HotTrackedPoint Then
                e.LabelText = myLabelText
            Else
                If Not e.SeriesPoint.Tag Is Nothing Then e.LabelText = e.SeriesPoint.Tag.ToString
            End If
        End If

    End Sub

    ''' <summary>
    ''' Draws the curve results chart
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 28/06/2011
    ''' </remarks>
    Private Sub DrawChart()
        Try
            Dim resultData As GlobalDataTO

            If Not HistoricalModeField Then
                Dim myCurveResultsDelegate As New CurveResultsDelegate
                resultData = myCurveResultsDelegate.GetResults(Nothing, CurveResultsID)
            Else
                Dim myHistCurve As New HisWSCurveResultsDelegate
                resultData = myHistCurve.GetResults(Nothing, HistOrderTestID, AnalyzerIDField, WorkSessionIDField)
            End If

            myLabelText = String.Empty
            m_HotTrackedPoint = New SeriesPoint

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                TitleChartLabel.Visible = False
                DXChartControl.Series(0).Points.Clear()
                DXChartControl.Series(1).Points.Clear()

                If (MonotonousCurve) Then
                    Dim curveDS As CurveResultsDS
                    curveDS = DirectCast(resultData.SetDatos, CurveResultsDS)

                    'Plot curve points
                    RemoveHandler DXChartControl.CustomDrawSeriesPoint, AddressOf DXChartControl_CustomDrawSeriesPoint
                    For i As Integer = 0 To curveDS.twksCurveResults.Rows.Count - 1
                        With curveDS.twksCurveResults(i)
                            DXChartControl.Series(0).Points.Add(New SeriesPoint(.CONCValue, .ABSValue))
                        End With
                    Next i

                    RemoveHandler DXChartControl.CustomDrawSeriesPoint, AddressOf DXChartControl_CustomDrawSeriesPoint
                    AddHandler DXChartControl.CustomDrawSeriesPoint, AddressOf DXChartControl_CustomDrawSeriesPoint

                    If curveDS.twksCurveResults.Rows.Count > 0 Then  ' dl 08/06/2011
                        'Plot CalibratorBlankAbsUsed
                        m_HotTrackedPoint = New SeriesPoint(PointConcentration(0), PointAbsorbance(0))
                        myLabelText = labelSampleClasessBlank
                        DXChartControl.Series(1).Points.Add(m_HotTrackedPoint)
                        DXChartControl.Series(1).Points(0).Tag = myLabelText
                        'Plot Result Points
                        For i As Integer = 1 To PointAbsorbance.Count - 1
                            myLabelText = i.ToString()

                            m_HotTrackedPoint = New SeriesPoint(PointConcentration(i), PointAbsorbance(i))
                            DXChartControl.Series(1).Points.Add(m_HotTrackedPoint)
                            DXChartControl.Series(1).Points(i).Tag = myLabelText
                        Next i

                        TitleChartLabel.Visible = False

                        If CurveType = "LINEAR" Then

                            Dim AverageList As List(Of ResultsDS.vwksResultsRow) = _
                                                (From row In AverageResults.vwksResults _
                                                 Where row.TestName = ChartTestName _
                                                 AndAlso row.SampleType = ChartSampleType _
                                                 AndAlso row.RerunNumber = AcceptedRerunNumber _
                                                 AndAlso row.CurveType = "LINEAR" _
                                                 AndAlso Not row.IsCurveOffsetNull AndAlso Not row.IsCurveSlopeNull AndAlso Not row.IsCurveCorrelationNull _
                                                 Order By row.CurveSlope Descending _
                                                 Select row).ToList()

                            If AverageList.Count > 0 Then
                                AbsLabelControl.Visible = True

                                AbsLabelControl.Text = ""
                                AbsLabelControl.Text &= "Abs = " & AverageList(0).CurveSlope.ToString("#0.####") & "*Conc"

                                If AverageList(0).CurveOffset < 0 Then
                                    AbsLabelControl.Text &= " - " & ((-1) * AverageList(0).CurveOffset).ToString("#0.####")
                                Else
                                    AbsLabelControl.Text &= " + " & AverageList(0).CurveOffset.ToString("#0.####")
                                End If

                                AbsLabelControl.Text &= "  r = " & AverageList(0).CurveCorrelation.ToString("#0.####")
                                AbsLabelControl.Text &= "  r² = " & Math.Pow(AverageList(0).CurveCorrelation, 2).ToString("#0.####")

                            Else
                                AbsLabelControl.Visible = False
                            End If
                        Else
                            AbsLabelControl.Visible = False
                        End If

                    Else
                        AbsLabelControl.Visible = False
                        TitleChartLabel.Visible = True
                        TitleChartLabel.Text = Environment.NewLine & labelCalibCurveNotCalculated & Environment.NewLine & " "
                    End If

                Else
                    AbsLabelControl.Visible = False
                    TitleChartLabel.Visible = True
                    TitleChartLabel.Text = Environment.NewLine & labelCalibCurveNotCalculated & Environment.NewLine & " "
                End If
            End If

            ProcessEvent = False

            XAxisCombo.SelectedValue = CurveAxisXType
            ComboSelectedIndex(XAxisCombo.Name) = XAxisCombo.SelectedIndex

            YAxisCombo.SelectedValue = CurveAxisYType
            ComboSelectedIndex(YAxisCombo.Name) = YAxisCombo.SelectedIndex

            CalibrationCurveCombo.SelectedValue = CurveType
            ComboSelectedIndex(CalibrationCurveCombo.Name) = CalibrationCurveCombo.SelectedIndex

            If (CurveGrowthType = "DEC") Then
                bsDecreasingRadioButton.Checked = True
            Else
                bsIncreasingRadioButton.Checked = True
            End If

            ProcessEvent = True

            If HistoricalModeField Then 'AG 16/10/2012
                XAxisCombo.Enabled = False
                YAxisCombo.Enabled = False
                CalibrationCurveCombo.Enabled = False
                bsIncreasingRadioButton.Enabled = False
                bsDecreasingRadioButton.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " DrawChart ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Load the ComboBoxes for X and Y Axis Types and for Types of Calibration Curves 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 31/08/2010
    '''              SA 12/11/2010 - Changed code in case of error: the message returned for GetList function
    '''                              has to be shown.
    ''' </remarks>
    Private Sub LoadCombos()
        Dim result As GlobalDataTO

        Try
            ProcessEvent = False

            Dim preloadedMasterConfig As New PreloadedMasterDataDelegate
            result = preloadedMasterConfig.GetList(Nothing, PreloadedMasterDataEnum.CURVE_AXIS_TYPES)

            If (Not result.HasError AndAlso Not result.SetDatos Is Nothing) Then
                Dim XAxisDS As PreloadedMasterDataDS
                XAxisDS = DirectCast(result.SetDatos, PreloadedMasterDataDS)

                If (XAxisDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                    'Fill ComboBox of Axes Types for X-Axe
                    XAxisCombo.DataSource = XAxisDS.tfmwPreloadedMasterData
                    XAxisCombo.DisplayMember = "FixedItemDesc"
                    XAxisCombo.ValueMember = "ItemID"
                    ComboSelectedIndex(XAxisCombo.Name) = XAxisCombo.SelectedIndex
                End If
            Else
                'Error getting the list of Curve Axis Types; shown it
                ShowMessage(Me.Name & ".LoadCombos", result.ErrorCode, result.ErrorMessage, Me)
            End If

            result = preloadedMasterConfig.GetList(Nothing, PreloadedMasterDataEnum.CURVE_AXIS_TYPES)

            If (Not result.HasError And Not result.SetDatos Is Nothing) Then
                Dim YAxisDS As PreloadedMasterDataDS
                YAxisDS = CType(result.SetDatos, PreloadedMasterDataDS)

                If (YAxisDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                    'Fill ComboBox of Axis Types
                    YAxisCombo.DataSource = YAxisDS.tfmwPreloadedMasterData
                    YAxisCombo.DisplayMember = "FixedItemDesc"
                    YAxisCombo.ValueMember = "ItemID"
                    ComboSelectedIndex(YAxisCombo.Name) = YAxisCombo.SelectedIndex
                Else
                    ShowMessage(Me.Name & ".LoadCombos", result.ErrorCode, result.ErrorMessage, Me)
                End If
            End If

            If (Not result.HasError) Then
                result = preloadedMasterConfig.GetList(Nothing, PreloadedMasterDataEnum.CURVE_TYPES)
                If (Not result.HasError AndAlso Not result.SetDatos Is Nothing) Then
                    Dim CalibrationCurveDS As PreloadedMasterDataDS
                    CalibrationCurveDS = CType(result.SetDatos, PreloadedMasterDataDS)

                    If (CalibrationCurveDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                        'Fill ComboBox of Axis Types
                        CalibrationCurveCombo.DataSource = CalibrationCurveDS.tfmwPreloadedMasterData
                        CalibrationCurveCombo.DisplayMember = "FixedItemDesc"
                        CalibrationCurveCombo.ValueMember = "ItemID"
                        ComboSelectedIndex(CalibrationCurveCombo.Name) = CalibrationCurveCombo.SelectedIndex
                    End If
                Else
                    'Error getting the list of Calibration Curve Types; shown it
                    ShowMessage(Me.Name & ".LoadCombos", result.ErrorCode, result.ErrorMessage, Me)
                End If
            End If

            bsIncreasingRadioButton.Checked = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "LoadCombos " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadCombos", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            ProcessEvent = True

        End Try
    End Sub

    ''' <summary>
    ''' Returns Alarm Description (Remmark) associated to a Result
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 19/07/2010
    ''' Modified by: AG 28/07/2010
    '''              RH 29/07/2010
    ''' </remarks>
    Private Function GetResultAlarmDescription(ByVal OrderTestID As Integer, ByVal RerunNumber As Integer, ByVal MultiPointNumber As Integer) As String
        Try
            Dim Descriptions As List(Of String) = (From row In AverageResults.vwksResultsAlarms _
                                                   Where row.OrderTestID = OrderTestID _
                                                   And row.RerunNumber = RerunNumber _
                                                   And row.MultiPointNumber = MultiPointNumber _
                                                   Select row.Description Distinct).ToList()  'AG 28/07/2010 - Add Distinct

            If (Descriptions.Count > 0) Then
                'AG 28/07/2010
                'Return Descriptions(0)
                'Dim myTotalAlarmsDescription As String = ""
                'For i As Integer = 0 To Descriptions.Count - 1
                '    myTotalAlarmsDescription += Descriptions(i)
                '    If i < Descriptions.Count - 1 Then myTotalAlarmsDescription += ", "
                'Next
                'Return myTotalAlarmsDescription
                'END AG 28/07/2010

                'RH 29/07/2010
                'Just a little enhancement, because in .NET, Strings are inmutables.
                'It is better to do Strings concatenations inside a loop using StringBuilders
                'They were designed for that task
                Dim myTotalAlarmsDescription As New StringBuilder()
                For i As Integer = 0 To Descriptions.Count - 1
                    myTotalAlarmsDescription.Append(Descriptions(i))
                    If i < Descriptions.Count - 1 Then myTotalAlarmsDescription.Append(", ")
                Next
                Return myTotalAlarmsDescription.ToString()
                'END RH 29/07/2010
            Else
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetResultAlarmDescription ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Returns Alarm Description (Remmark) associated to an Execution
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 19/07/2010
    ''' Modified by: AG 28/07/2010
    '''              RH 29/07/2010
    ''' </remarks>
    Private Function GetExecutionAlarmDescription(ByVal ExecutionID As Integer) As String
        Try
            Dim Descriptions As List(Of String) = (From row In ExecutionResults.vwksWSExecutionsAlarms _
                                                   Where row.ExecutionID = ExecutionID _
                                                   Select row.Description Distinct).ToList() 'AG 28/07/2010 - Add distinct

            If (Descriptions.Count > 0) Then
                'RH 29/07/2010
                'Just a little enhancement, because in .NET, Strings are inmutables.
                'It is better to do Strings concatenations inside a loop using StringBuilders
                'They were designed for that task
                Dim myTotalAlarmsDescription As New StringBuilder()
                For i As Integer = 0 To Descriptions.Count - 1
                    myTotalAlarmsDescription.Append(Descriptions(i))
                    If i < Descriptions.Count - 1 Then myTotalAlarmsDescription.Append(", ")
                Next
                Return myTotalAlarmsDescription.ToString()
                'END RH 29/07/2010
            Else
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetExecutionAlarmDescription ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetExecutionAlarmDescription ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            Return Nothing
        End Try
    End Function


    ''' <summary>
    ''' Returns Alarm Description (Remmark) associated to an Historical Execution (HistOrderTestID)
    ''' </summary>
    ''' <param name="pHistOrderTestID"></param>
    ''' <param name="pMultiItemNumber"></param>
    ''' <param name="pReplicateNumber"></param>
    ''' <remarks>
    ''' Created by:  AG 17/10/2012 - based on GetExecutionAlarmDescription
    ''' </remarks>
    Private Function HIST_GetExecutionAlarmDescription(ByVal pHistOrderTestID As Integer, ByVal pMultiItemNumber As Integer, ByVal pReplicateNumber As Integer) As String
        Try
            Dim Descriptions As List(Of String) = (From row In ExecutionResults.vwksWSExecutionsAlarms _
                                                   Where row.HistOrderTestID = pHistOrderTestID _
                                                   And row.MultiPointNumber = pMultiItemNumber _
                                                   And row.ReplicateNumber = pReplicateNumber _
                                                   Select row.Description Distinct).ToList() 'AG 28/07/2010 - Add distinct

            If (Descriptions.Count > 0) Then
                'Just a little enhancement, because in .NET, Strings are inmutables.
                'It is better to do Strings concatenations inside a loop using StringBuilders
                'They were designed for that task
                Dim myTotalAlarmsDescription As New StringBuilder()
                For i As Integer = 0 To Descriptions.Count - 1
                    myTotalAlarmsDescription.Append(Descriptions(i))
                    If i < Descriptions.Count - 1 Then myTotalAlarmsDescription.Append(", ")
                Next
                Return myTotalAlarmsDescription.ToString()
            Else
                Return Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".HIST_GetExecutionAlarmDescription ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".HIST_GetExecutionAlarmDescription ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Refresh grid or clear it
    ''' </summary>
    ''' <param name="pClearGrid"></param>
    ''' <remarks>
    ''' Created by:  AG 26/07/2010
    ''' Modified by  AG Added new conection parameter
    ''' Modified by: RH 27/05/2011 Remove connection parameter
    ''' </remarks>
    Private Sub UpdateCurrentResultsGrid(ByVal pClearGrid As Boolean)
        'Dim resultData As New GlobalDataTO
        'Dim dbConnection As New SqlClient.SqlConnection

        Try
            'resultData = DAOBase.GetOpenDBConnection(pDBConnection)
            'If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
            '    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
            '    If (Not dbConnection Is Nothing) Then
            '        If Not pClearGrid Then
            '            UpdateCalibratorsMuliItemDataGrid(dbConnection, TestSelectedText)
            '        Else
            '            'RH 13/10/2010
            '            'CalibratorsDataGridView.Rows.Clear()
            '            For j As Integer = CalibratorsDataGridView.Rows.Count - 1 To 0 Step -1
            '                CalibratorsDataGridView.Rows(j).Visible = False
            '            Next
            '        End If
            '    End If
            'End If

            If Not pClearGrid Then
                UpdateCalibratorsMuliItemDataGrid(SelectedTestName, SampleTypeSelectedTextField)
            Else
                'RH 13/10/2010
                'CalibratorsDataGridView.Rows.Clear()
                For j As Integer = CalibratorsDataGridView.Rows.Count - 1 To 0 Step -1
                    CalibratorsDataGridView.Rows(j).Visible = False
                Next
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".UpdateCurrentGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateCurrentGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            'Finally
            '    If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to load the buttons images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/04/2010
    ''' Modified by: DL 21/06/2010 - Load the Icon in Image Property instead of in BackgroundImage
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = String.Empty
            Dim iconPath As String = MyBase.IconsPath
            Dim preloadedDataConfig As New PreloadedMasterDataDelegate

            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("FREECELL")
            If (auxIconName <> "") Then
                NoImage = preloadedDataConfig.GetIconImage("FREECELL")
            End If

            auxIconName = GetIconName("BLANK")
            If (auxIconName <> "") Then
                ClassImage = preloadedDataConfig.GetIconImage("BLANK")
            End If

            auxIconName = GetIconName("ABS_GRAPH")
            If (auxIconName <> "") Then
                ABS_GRAPH = preloadedDataConfig.GetIconImage("ABS_GRAPH")
            End If

            auxIconName = GetIconName("AVG_ABS_GRAPH")
            If (auxIconName <> "") Then
                AVG_ABS_GRAPH = preloadedDataConfig.GetIconImage("AVG_ABS_GRAPH")
            End If

            SampleIconList = New ImageList()

            auxIconName = GetIconName("STATS")
            If (auxIconName <> "") Then
                AddIconToImageList(SampleIconList, auxIconName)
            End If

            auxIconName = GetIconName("ROUTINES")
            If (auxIconName <> "") Then
                AddIconToImageList(SampleIconList, auxIconName)
            End If

            If HistoricalModeField Then 'AG 17/10/2012
                bsPrintButton.Enabled = False
                bsPrintButton.Visible = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Update collapse in database
    ''' - pGeneral (True): All results shows in current grid
    ''' - pGeneral (False): Only the OrderTestID - RerunNumber - MultiItemNumber informed in pResultRow
    ''' </summary>
    ''' <returns >Boolean indicating if an error has been occurred</returns>
    ''' <remarks>Created by AG 03/08/2010</remarks>
    Private Function UpdateCollapse(ByVal pGeneral As Boolean, ByVal pNewValue As Boolean, Optional ByVal pGridName As String = Nothing, _
                                    Optional ByVal pResultRow As ResultsDS.vwksResultsRow = Nothing) As Boolean
        Dim finalResultOK As Boolean = True

        Try
            Dim myResultsDS As New ResultsDS
            Dim myResultsDelegate As New ResultsDelegate
            Dim OrderTestIdList As List(Of Integer)

            Dim Setdatos As Boolean = True

            If pGeneral Then
                'Update collapse value for all results in current grid
                '1st get all ordertestid shown in current grid
                Select Case pGridName
                    Case CalibratorsDataGridView.Name
                        OrderTestIdList = (From row In ExecutionResults.vwksWSExecutionsResults _
                                           Where row.TestName = SelectedTestName AndAlso row.SampleClass = "CALIB" _
                                           Select row.OrderTestID Distinct).ToList()

                    Case Else
                        Return False
                End Select

                '2on create dataset
                For Each myOrderTest As Integer In OrderTestIdList
                    Dim newResultsRow As ResultsDS.twksResultsRow
                    newResultsRow = myResultsDS.twksResults.NewtwksResultsRow()
                    With newResultsRow
                        .Collapsed = pNewValue
                        .OrderTestID = myOrderTest
                        .RerunNumber = Nothing
                        .MultiPointNumber = Nothing
                    End With
                    newResultsRow.EndEdit()
                    myResultsDS.twksResults.AddtwksResultsRow(newResultsRow)
                Next
                myResultsDS.AcceptChanges()

            Else
                If Not pResultRow.IsOrderTestIDNull Then 'DL 16/05/2012

                    'Update collapse value only for the selected result
                    Dim newResultsRow As ResultsDS.twksResultsRow
                    newResultsRow = myResultsDS.twksResults.NewtwksResultsRow()
                    With newResultsRow
                        .Collapsed = pNewValue
                        .OrderTestID = pResultRow.OrderTestID
                        .RerunNumber = pResultRow.RerunNumber
                        .MultiPointNumber = pResultRow.MultiPointNumber
                    End With
                    newResultsRow.EndEdit()
                    myResultsDS.twksResults.AddtwksResultsRow(newResultsRow)
                    myResultsDS.AcceptChanges()
                Else 'DL 16/05/2012
                    Setdatos = False 'DL 16/05/2012
                End If 'DL 16/05/2012
            End If

            If Setdatos Then 'DL 16/05/2012
                Dim myGlobal As New GlobalDataTO
                myGlobal = myResultsDelegate.UpdateCollapse(Nothing, myResultsDS)

                If (myGlobal.HasError) Then
                    ShowMessage(Name & ".UpdateCollapse", myGlobal.ErrorCode, myGlobal.ErrorMessage)
                    finalResultOK = False
                End If
            End If 'DL 16/05/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateCollapse ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".UpdateCollapse", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
            finalResultOK = False
        End Try
        Return finalResultOK
    End Function
#End Region

#Region "CalibratorsDataGridView Methods"
    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>    ''' 
    ''' <remarks>
    ''' '<param name="pLanguageID"> The current Language of Application </param>
    ''' Created by: AG 03/08/2010 (based on InitializeCalibratorsDataGrid)
    ''' Modified by PG 14/10/10
    ''' Modified by: RH 20/10/2010 - Remove the LanguageID parameter. Now it is a class property.
    ''' </remarks>
    Private Sub InitializeCalibratorsGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            CalibratorsDataGridView.Columns.Clear()

            'Dim CollapseColumn As New bsDataGridViewCollapseColumn
            'With CollapseColumn
            '    .Name = CollapseColName
            '    AddHandler .HeaderClickEventHandler, AddressOf CalibratorsDataGridView_CellMouseClick
            '    If HistoricalModeField Then .Visible = False 'AG 17/10/2012 - Try but it does not work because the '+' appears in header!!!
            'End With
            'CalibratorsDataGridView.Columns.Add(CollapseColumn)

            If Not HistoricalModeField Then 'AG 17/10/2012
                Dim CollapseColumn As New bsDataGridViewCollapseColumn
                With CollapseColumn
                    .Name = CollapseColName
                    AddHandler .HeaderClickEventHandler, AddressOf CalibratorsDataGridView_CellMouseClick
                End With
                CalibratorsDataGridView.Columns.Add(CollapseColumn)
            Else 'Not show replicates in historical mode
                Dim CollapseColumn As New DataGridViewTextBoxSpanColumn
                With CollapseColumn
                    .Name = CollapseColName
                    .HeaderText = ""
                    .Width = 25
                    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                    .HeaderCell.Style.WrapMode = DataGridViewTriState.False
                End With
                CalibratorsDataGridView.Columns.Add(CollapseColumn)
            End If

            Dim MultiItemColumn As New DataGridViewTextBoxSpanColumn
            With MultiItemColumn
                .Name = "ItemNumber"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Kit", LanguageID)
                .Width = 30
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
            End With
            CalibratorsDataGridView.Columns.Add(MultiItemColumn)

            'CalibratorsDataGridView.Columns.Add("SeeRems", "")
            'CalibratorsDataGridView.Columns("SeeRems").Width = 33
            'CalibratorsDataGridView.Columns("SeeRems").SortMode = DataGridViewColumnSortMode.NotSortable
            'CalibratorsDataGridView.Columns("SeeRems").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            Dim GraphColumn As New DataGridViewImageColumn
            With GraphColumn
                .Name = "Graph"
                .HeaderText = "" 'myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Graph", LanguageID)
                .Width = 30

                If HistoricalModeField Then .Visible = False 'AG 23/10/2012 - Not visible in v1

            End With
            CalibratorsDataGridView.Columns.Add(GraphColumn)

            'PG 14/10/2010
            'CalibratorsDataGridView.Columns.Add("No", "No.")
            CalibratorsDataGridView.Columns.Add("No", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", LanguageID))
            'PG 14/10/2010
            CalibratorsDataGridView.Columns("No").Width = 33
            CalibratorsDataGridView.Columns("No").SortMode = DataGridViewColumnSortMode.NotSortable
            CalibratorsDataGridView.Columns("No").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            'PG 14/10/2010
            'CalibratorsDataGridView.Columns.Add("ABSValue", "Abs.")
            CalibratorsDataGridView.Columns.Add("ABSValue", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", LanguageID))
            'PG 14/10/2010
            CalibratorsDataGridView.Columns("ABSValue").Width = 55
            CalibratorsDataGridView.Columns("ABSValue").SortMode = DataGridViewColumnSortMode.NotSortable
            CalibratorsDataGridView.Columns("ABSValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            Dim ConcentrationColumn As New DataGridViewTextBoxSpanColumn
            With ConcentrationColumn
                .Name = "TheorConc"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TheoricalConc_Short", LanguageID)
                .Width = 50
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
            End With
            CalibratorsDataGridView.Columns.Add(ConcentrationColumn)


            CalibratorsDataGridView.Columns.Add("CONCValue", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", LanguageID))
            CalibratorsDataGridView.Columns("CONCValue").Width = 65 'AG 25/10/2012 - increment width because now the conc is formated with 4 decimals (old value 50)
            CalibratorsDataGridView.Columns("CONCValue").SortMode = DataGridViewColumnSortMode.NotSortable
            CalibratorsDataGridView.Columns("CONCValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            Dim UnitColumn As New DataGridViewTextBoxSpanColumn
            With UnitColumn
                .Name = "Unit"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", LanguageID)
                .Width = 45
            End With
            CalibratorsDataGridView.Columns.Add(UnitColumn)

            CalibratorsDataGridView.Columns.Add("Error", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_%Error", LanguageID))
            CalibratorsDataGridView.Columns("Error").Width = 65
            CalibratorsDataGridView.Columns("Error").SortMode = DataGridViewColumnSortMode.NotSortable
            CalibratorsDataGridView.Columns("Error").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            CalibratorsDataGridView.Columns.Add("Remarks", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", LanguageID))

            CalibratorsDataGridView.Columns("Remarks").Width = 300
            CalibratorsDataGridView.Columns("Remarks").SortMode = DataGridViewColumnSortMode.NotSortable
            CalibratorsDataGridView.Columns("Remarks").DefaultCellStyle.WrapMode = DataGridViewTriState.True

            CalibratorsDataGridView.Columns.Add("Date", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", LanguageID))
            CalibratorsDataGridView.Columns("Date").Width = 140 'AG 15/08/2010 (120)
            CalibratorsDataGridView.Columns("Date").SortMode = DataGridViewColumnSortMode.NotSortable
            CalibratorsDataGridView.Columns("Date").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter


            ' dl 28/07/2011
            CalibratorsDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeCalibratorsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fill the CalibratorsDataGridView with test name associated data when the calibrator is a multi item element
    ''' </summary>
    ''' <param name="pTestName"></param>
    ''' <param name="pSampleType"></param>
    ''' <remarks>
    ''' Created by: AG 03/08/2010 (based on UpdateCalibratorsDataGrid)
    ''' Modified by RH 08/10/2010
    ''' AG 18/072012 - filter also by sample type
    ''' </remarks>
    Private Sub UpdateCalibratorsMuliItemDataGrid(ByVal pTestName As String, ByVal pSampleType As String)
        Try
            Dim dgv As BSDataGridView = CalibratorsDataGridView
            Dim Remark As String = String.Empty

            Me.Enabled = False
            Cursor = Cursors.WaitCursor

            'dgv.Visible = False

            'dgv.Rows.Clear()
            'Set the rows count for the Collapse Column
            If Not HistoricalModeField Then 'AG 17/10/2012 - collapse only when current WS results
                CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = 0
            End If

            Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                        (From row In ExecutionResults.vwksWSExecutionsResults _
                         Where row.TestName = pTestName AndAlso row.SampleClass = "CALIB" _
                         AndAlso row.SampleType = pSampleType AndAlso row.RerunNumber = AcceptedRerunNumber _
                         Select row).ToList()

            If TestList.Count = 0 Then
                For j As Integer = 0 To dgv.Rows.Count - 1
                    dgv.Rows(j).Visible = False
                Next
                'dgv.Visible = True
                Return
            End If

            'RH 31/05/2011 Update SelectedSampleType
            SelectedSampleType = TestList.First().SampleType

            Dim TheoreticalConcList As List(Of Single) = _
                            (From row In AverageResults.vwksResults _
                             Where row.OrderTestID = TestList(0).OrderTestID _
                             Select row.TheoricalConcentration Distinct).ToList()

            If TheoreticalConcList.Count = 0 Then
                For j As Integer = 0 To dgv.Rows.Count - 1
                    dgv.Rows(j).Visible = False
                Next
                'dgv.Visible = True
                Return
            End If

            Dim i As Integer = 0
            Dim IsAverageDone As New Dictionary(Of Integer, Boolean)

            'First point is CalibratorBlankAbsUsed. Concentration is set to Zero.
            PointAbsorbance.Clear()
            PointConcentration.Clear()
            PointAbsorbance.Add(0)
            PointConcentration.Add(0)

            Dim itempoint As Integer = 0 'TheoreticalConcList.Count + 1
            For Each myTheoreticalConc As Single In TheoreticalConcList
                'itempoint -= 1
                itempoint += 1

                Dim AverageList As List(Of ResultsDS.vwksResultsRow) = _
                                (From row In AverageResults.vwksResults _
                                 Where row.OrderTestID = TestList(0).OrderTestID _
                                 AndAlso row.SampleType = pSampleType _
                                 AndAlso row.TheoricalConcentration = myTheoreticalConc _
                                 AndAlso row.MultiPointNumber = itempoint _
                                 AndAlso row.RerunNumber = AcceptedRerunNumber _
                                 Select row).ToList()

                'END AG 08/08/2010

                With AverageList(0)
                    PointAbsorbance(0) = .CalibratorBlankAbsUsed

                    'AG 16/10/2012
                    'CurveResultsID = .CurveResultsID
                    If Not HistoricalModeField Then
                        CurveResultsID = .CurveResultsID
                    Else
                        HistOrderTestID = .OrderTestID
                    End If

                    ChartTestName = .TestName
                    ChartSampleType = .SampleType
                    ChartRerunNumber = .RerunNumber
                    PointAbsorbance.Add(.ABSValue)
                    PointConcentration.Add(.TheoricalConcentration)
                    CurveGrowthType = .CurveGrowthType
                    CurveType = .CurveType
                    CurveAxisXType = .CurveAxisXType
                    CurveAxisYType = .CurveAxisYType
                    DecimalsAllowed = .DecimalsAllowed
                    MonotonousCurve = String.IsNullOrEmpty(.CalibrationError)
                End With

                For Each resultRow As ResultsDS.vwksResultsRow In AverageList
                    If Not IsAverageDone.ContainsKey(resultRow.MultiPointNumber) Then
                        IsAverageDone(resultRow.MultiPointNumber) = True
                        'dgv.Rows.Add()
                        If i >= dgv.Rows.Count Then
                            dgv.Rows.Add()
                        Else
                            dgv.Rows(i).Visible = True
                        End If

                        'SetSubHeaderColors(dgv, i)
                        dgv.Rows(i).DefaultCellStyle.BackColor = Color.LightSlateGray 'Color.DodgerBlue
                        dgv.Rows(i).DefaultCellStyle.SelectionBackColor = Color.LightSlateGray 'Color.DodgerBlue
                        dgv.Rows(i).DefaultCellStyle.ForeColor = Color.White
                        dgv.Rows(i).DefaultCellStyle.SelectionForeColor = Color.White

                        If Not HistoricalModeField Then 'AG 17/10/2012 - collapse only when current WS results
                            CType(dgv(CollapseColName, i), bsDataGridViewCollapseCell).IsSubHeader = True
                        End If

                        'RH 03/06/2011
                        MergeCells(dgv, "TheorConc", i, 1)
                        MergeCells(dgv, "Unit", i, 1)

                        dgv("ItemNumber", i).Value = resultRow.MultiPointNumber
                        dgv("Graph", i).Value = AVG_ABS_GRAPH ' NoImage dl 21/02/2011
                        dgv("Graph", i).ToolTipText = labelOpenAbsorbanceCurve
                        dgv("No", i).Value = Nothing
                        dgv("ABSValue", i).Value = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                        dgv("TheorConc", i).Value = resultRow.TheoricalConcentration.ToStringWithDecimals(resultRow.DecimalsAllowed)

                        dgv("Error", i).Value = Nothing

                        If Not resultRow.IsCONC_ValueNull Then
                            Dim hasConcentrationError As Boolean = False
                            If Not resultRow.IsCONC_ErrorNull Then
                                hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error) 'AG 15/09/2010
                            End If

                            If Not hasConcentrationError Then
                                'AG 18/10/2012 - 4 decimals, otherwise the %Error has no sense
                                'dgv("CONCValue", i).Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                dgv("CONCValue", i).Value = resultRow.CONC_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)

                                If Not resultRow.IsRelativeErrorCurveNull Then
                                    dgv("Error", i).Value = resultRow.RelativeErrorCurve.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                End If
                            Else
                                dgv("CONCValue", i).Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                            End If
                        Else
                            dgv("CONCValue", i).Value = Nothing
                        End If

                        dgv("Unit", i).Value = resultRow.MeasureUnit

                        'AG 15/09/2010 - Special case when Absorbance has error
                        If Not resultRow.IsABS_ErrorNull Then
                            If Not String.IsNullOrEmpty(resultRow.ABS_Error) Then
                                dgv("ABSValue", i).Value = GlobalConstants.ABSORBANCE_ERROR
                                dgv("CONCValue", i).Value = GlobalConstants.CONC_DUE_ABS_ERROR
                            End If
                        End If
                        'END AG 15/09/2010

                        Remark = GetResultAlarmDescription(resultRow.OrderTestID, resultRow.RerunNumber, resultRow.MultiPointNumber)
                        'dgv("SeeRems", i).Style.Font = SeeRemsFont
                        'If Not String.IsNullOrEmpty(Remark) Then
                        '    dgv("SeeRems", i).Value = "*"
                        'Else
                        '    dgv("SeeRems", i).Value = Nothing
                        'End If
                        'dgv("SeeRems", i).ToolTipText = Remark

                        'AG 25/10/2012 - In order to when remarks the No col shows '*' + tooltip (avg level) this code must be activated but it was commented but with no explanation
                        'Sw Area meeting -> Activate it if no causes any problem
                        If Not String.IsNullOrEmpty(Remark) Then
                            dgv("No", i).Value = "*"
                        Else
                            dgv("No", i).Value = Nothing
                        End If
                        'AG 25/10/2012

                        dgv("Remarks", i).Value = Remark

                        'dgv("Date", i).Value = resultRow.ResultDateTime.ToShortDateString() & _
                        '                                            " " & resultRow.ResultDateTime.ToShortTimeString()
                        dgv("Date", i).Value = resultRow.ResultDateTime.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                               resultRow.ResultDateTime.ToString(SystemInfoManager.OSLongTimeFormat)

                        dgv.Rows(i).Tag = resultRow

                        Dim OrderTestID As Integer = resultRow.OrderTestID
                        Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010
                        Dim Striked As Boolean = False

                        'AG 04/08/2010 - add rerunnumber condition
                        TestList = (From row In ExecutionResults.vwksWSExecutionsResults _
                                    Where row.OrderTestID = OrderTestID _
                                    AndAlso row.RerunNumber = myRerunNumber _
                                    AndAlso row.MultiItemNumber = itempoint _
                                    Select row Distinct).ToList()

                        'dgv.Rows.Add(TestList.Count)

                        For j As Integer = 0 To TestList.Count - 1
                            Dim k As Integer = j + i + 1

                            If k >= dgv.Rows.Count Then
                                dgv.Rows.Add()
                                dgv.Rows(k).Visible = Not resultRow.Collapsed 'RH 13/12/2010
                            Else
                                'dgv.Rows(k).Visible = True
                                dgv.Rows(k).Visible = Not resultRow.Collapsed 'RH 13/12/2010
                            End If

                            MergeCells(dgv, "TheorConc", k, 1)
                            MergeCells(dgv, "Unit", k, 1)

                            If TestList(j).InUse Then
                                dgv.Rows(k).DefaultCellStyle.Font = RegularFont
                                dgv.Rows(k).DefaultCellStyle.BackColor = RegularBkColor
                                dgv.Rows(k).DefaultCellStyle.SelectionBackColor = RegularBkColor
                                dgv.Rows(k).DefaultCellStyle.ForeColor = RegularForeColor
                                dgv.Rows(k).DefaultCellStyle.SelectionForeColor = RegularForeColor
                            Else
                                dgv.Rows(k).DefaultCellStyle.Font = StrikeFont
                                dgv.Rows(k).DefaultCellStyle.BackColor = StrikeBkColor
                                dgv.Rows(k).DefaultCellStyle.SelectionBackColor = StrikeBkColor
                                dgv.Rows(k).DefaultCellStyle.ForeColor = StrikeForeColor
                                dgv.Rows(k).DefaultCellStyle.SelectionForeColor = StrikeForeColor
                                Striked = True
                            End If

                            If Not HistoricalModeField Then 'AG 17/10/2012 - collapse only when current WS results
                                CType(dgv(CollapseColName, k), bsDataGridViewCollapseCell).IsSubHeader = False
                            End If

                            'RH 03/06/2011
                            dgv("ItemNumber", k).Value = Nothing

                            dgv("Graph", k).Value = ABS_GRAPH
                            dgv("Graph", k).ToolTipText = labelOpenAbsorbanceCurve
                            'dgv("No", k).Value = TestList(j).ReplicateNumber.ToString()
                            dgv("ABSValue", k).Value = TestList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                            dgv("TheorConc", k).Value = resultRow.TheoricalConcentration.ToStringWithDecimals(resultRow.DecimalsAllowed)

                            dgv("Error", k).Value = Nothing

                            If Not TestList(j).IsCONC_ValueNull Then
                                Dim hasConcentrationError As Boolean = False
                                If Not TestList(j).IsCONC_ErrorNull Then
                                    hasConcentrationError = Not String.IsNullOrEmpty(TestList(j).CONC_Error) 'AG 15/09/2010
                                End If

                                If Not hasConcentrationError Then
                                    'AG 18/10/2012 - 4 decimals, otherwise the %Error has no sense
                                    'dgv("CONCValue", k).Value = TestList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                    dgv("CONCValue", k).Value = TestList(j).CONC_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)

                                    If Not TestList(j).IsCONC_CurveErrorNull Then
                                        dgv("Error", k).Value = TestList(j).CONC_CurveError.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                    End If
                                Else
                                    dgv("CONCValue", k).Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                End If
                            Else
                                dgv("CONCValue", k).Value = Nothing
                            End If

                            'AG 15/09/2010 - Special case when Absorbance has error
                            If Not TestList(j).IsABS_ErrorNull Then
                                If Not String.IsNullOrEmpty(TestList(j).ABS_Error) Then
                                    dgv("ABSValue", k).Value = GlobalConstants.ABSORBANCE_ERROR
                                    dgv("CONCValue", k).Value = GlobalConstants.CONC_DUE_ABS_ERROR
                                End If
                            End If
                            'END AG 15/09/2010

                            If Not HistoricalModeField Then 'AG 17/10/2012
                                Remark = GetExecutionAlarmDescription(TestList(j).ExecutionID)
                            Else
                                Remark = HIST_GetExecutionAlarmDescription(TestList(j).OrderTestID, TestList(j).MultiItemNumber, TestList(j).ReplicateNumber)
                            End If

                            'dgv("SeeRems", k).Style.Font = SeeRemsFont
                            'If Not String.IsNullOrEmpty(Remark) Then
                            '    dgv("SeeRems", k).Value = "*"
                            'Else
                            '    dgv("SeeRems", k).Value = Nothing
                            'End If
                            'dgv("SeeRems", k).ToolTipText = Remark

                            If Not String.IsNullOrEmpty(Remark) Then
                                dgv("No", k).Value = "* " & TestList(j).ReplicateNumber
                            Else
                                dgv("No", k).Value = TestList(j).ReplicateNumber.ToString()
                            End If

                            dgv("Remarks", k).Value = Remark

                            'dgv("Date", k).Value = TestList(j).ResultDate.ToShortDateString() & _
                            '                                        " " & TestList(j).ResultDate.ToShortTimeString()
                            dgv("Date", k).Value = TestList(j).ResultDate.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                   TestList(j).ResultDate.ToString(SystemInfoManager.OSLongTimeFormat)

                            dgv.Rows(k).Tag = TestList(j)

                            dgv("TheorConc", k).Value = dgv("TheorConc", i).Value
                            dgv("Unit", k).Value = dgv("Unit", i).Value
                        Next

                        If Not Striked Then
                            MergeCells(dgv, "TheorConc", i + 1, TestList.Count)
                            MergeCells(dgv, "Unit", i + 1, TestList.Count)
                        End If

                        If Not HistoricalModeField Then 'AG 17/10/2012 - collapse only when current WS results
                            If Not IsColCollapsed.ContainsKey(SelectedTestName & "_CALIB") Then
                                CType(dgv(CollapseColName, i), bsDataGridViewCollapseCell).IsCollapsed = resultRow.Collapsed
                            End If
                        End If

                        i += TestList.Count + 1
                    End If
                Next

                If Not HistoricalModeField Then 'AG 17/10/2012 - collapse only when current WS results
                    If IsColCollapsed.ContainsKey(SelectedTestName & "_CALIB") Then
                        If IsColCollapsed(SelectedTestName & "_CALIB") Then
                            CollapseAll(dgv)
                        Else
                            ExpandAll(dgv)
                        End If
                    End If
                End If

            Next myTheoreticalConc ' AG 08/08/2010

            If i < dgv.Rows.Count Then
                For j As Integer = i To dgv.Rows.Count - 1
                    dgv.Rows(j).Visible = False
                Next
            End If

            'Set the rows count for the Collapse Column
            If Not HistoricalModeField Then 'AG 17/10/2012 - collapse only when current WS results
                CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = i
            End If
            'dgv.Visible = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateCalibratorsMuliItemDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Me.Enabled = True
            Cursor = Cursors.Default

        End Try
    End Sub
#End Region

#Region "Generic DataGridView Methods"
    ''' <summary>
    ''' Displays the result chart
    ''' </summary>
    ''' <param name="pExecutionNumber">execution identifier</param>
    ''' <param name="pOrderTestID">order test id</param>
    ''' <param name="pMultiItemNumber">multiitem number</param>
    ''' <param name="pRerunNumber">rerun number</param>
    ''' <remarks>
    ''' Created by dl 18/02/2011
    ''' </remarks>
    Private Sub ShowResultsChart(ByVal pExecutionNumber As Integer, _
                                 ByVal pOrderTestID As Integer, _
                                 ByVal pRerunNumber As Integer, _
                                 ByVal pMultiItemNumber As Integer, _
                                 ByVal pReplicate As Integer)

        Try

            Using myForm As New IResultsAbsCurve
                myForm.AnalyzerID = AnalyzerIDField
                myForm.WorkSessionID = WorkSessionIDField
                myForm.MultiItemNumber = pMultiItemNumber
                myForm.ReRun = pRerunNumber
                myForm.ExecutionID = pExecutionNumber
                myForm.OrderTestID = pOrderTestID
                myForm.SourceForm = GlobalEnumerates.ScreenCallsGraphical.CURVEFRM
                myForm.Replicate = pReplicate
                'If Not IsHead Then
                'myForm.SourceCalled = GraphicalAbsScreenCallMode.CURVE_RESULTS_SINGLE
                'Else
                'myForm.SourceCalled = GraphicalAbsScreenCallMode.CURVE_RESULTS_MULTIPLE
                'End If

                IAx00MainMDI.AddNoMDIChildForm = myForm 'Inform the MDI the curve calib results is shown
                'myForm.ShowInfo()  ' 25/05/2011
                myForm.ShowDialog()
                IAx00MainMDI.RemoveNoMDIChildForm = myForm 'Inform the MDI the curve calib results is closed
            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ShowResultsChart ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' RemoveResultsChart
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 30/08/2010
    ''' </remarks>
    Private Sub RemoveResultsChart()
        Try
            Dim father As Control = ResultsChart.Parent
            If father IsNot Nothing Then
                father.Controls.Remove(ResultsChart)

                'RH: It is not a good idea to dispose managed resources!
                'Let the Garbage Collector do that work
                'ResultsChart.Dispose()

                'RH 13/12/2010
                ResultsChart = Nothing
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RemoveResultsChart ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RemoveResultsChart", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Spans a DataGridViewCell into "RowSpan" rows
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 07/16/2010
    ''' </remarks>
    Private Sub MergeCells(ByRef dgv As BSDataGridView, ByVal SpanColName As String, ByVal RowIndex As Integer, ByVal RowSpan As Integer)
        Try
            CType(dgv(SpanColName, RowIndex), DataGridViewTextBoxSpanCell).RowSpan = RowSpan

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " MergeCells ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Collapses all the rows in a DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 07/16/2010
    ''' </remarks>
    Private Sub CollapseAll(ByRef dgv As BSDataGridView)
        Try
            CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).CollapseAll()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CollapseAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Expands all the rows in a DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 07/16/2010
    ''' </remarks>
    Private Sub ExpandAll(ByRef dgv As BSDataGridView)
        Try
            CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).ExpandAll()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ExpandAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Returns True if the row is a SubHeader. That is, it is an Average row. Returns False otherwise.
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 07/16/2010
    ''' </remarks>
    Private Function IsSubHeader(ByRef dgv As BSDataGridView, ByVal RowIndex As Integer) As Boolean
        Try
            If Not HistoricalModeField Then 'AG 17/10/2012 - collapse only when current WS results
                Return CType(dgv(CollapseColName, RowIndex), bsDataGridViewCollapseCell).IsSubHeader
            Else
                Return True ' In historical only show the sub-headers (avg results)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " IsSubHeader ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            Return False
        End Try
    End Function
#End Region

#Region "Public Methods"

    'Private DSNumber As Integer = 0

    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            'RH 30/05/2011 Not needed, because Executions and Results are loaded before calling RefreshScreen()
            'Me.LoadExecutionsResults()

            'RH Save UIRefreshDS
            'pRefreshDS.WriteXml(String.Format("UIRefreshDS{0}.xml", DSNumber))

            'RH Load UIRefreshDS
            'pRefreshDS.ReadXml(String.Format("UIRefreshDS{0}.xml", DSNumber))

            'DSNumber += 1

            If isClosingFlag Then Return 'AG 03/08/2012
            If HistoricalModeField Then Return 'AG 16/10/2012

            'Check refresh conditions
            Dim ChangedExecutions As List(Of UIRefreshDS.ExecutionStatusChangedRow) = _
                    (From row In pRefreshDS.ExecutionStatusChanged _
                     Where row.ExecutionStatus = "CLOSED" _
                     Select row).ToList()

            If ChangedExecutions.Count > 0 Then 'Only CLOSED Executions
                Dim IsRefreshAvailable As Boolean = False

                For Each row As UIRefreshDS.ExecutionStatusChangedRow In ChangedExecutions
                    Dim ResultRow As ExecutionsDS.vwksWSExecutionsResultsRow = _
                            (From ExecutionResultRow In ExecutionResults.vwksWSExecutionsResults _
                             Where ExecutionResultRow.ExecutionID = row.ExecutionID _
                             Select ExecutionResultRow).ToList().First()

                    If (ResultRow.TestName = SelectedTestName) AndAlso (ResultRow.SampleType = SelectedSampleType) AndAlso _
                            ((ResultRow.SampleClass = "BLANK") OrElse (ResultRow.SampleClass = "CALIB")) Then

                        If ResultRow.SampleClass = "BLANK" Then
                            IsRefreshAvailable = True
                            Exit For
                        Else 'It is CALIB
                            If ResultRow.RerunNumber = AcceptedRerunNumber Then
                                IsRefreshAvailable = True
                                Exit For
                            End If
                        End If
                    End If
                Next

                If IsRefreshAvailable Then
                    Me.UpdateCurrentResultsGrid(False)
                    Me.DrawChart()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RefreshScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try

    End Sub

#End Region


#Region "METHODS REPLACED FOR NEW ONES DUE TO PERFORMANCE ISSUES"
    ''' <summary>
    ''' Changes the InUse flag of a replicate and updates the DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 07/16/2010
    ''' Modified by AG 26/07/2010 - Use re calculations class
    ''' Modified by RH - 07/28/2010 - Renamed from SetStrike()
    ''' Modified by AG - 03/09/2010 - re draw chart and use only 1 connection
    '''             PG - 18/10/2010 - Get the current language
    ''' Modified by: RH 20/10/2010 - Remove the currentLanguage local variable/parameter.
    ''' </remarks>
    Private Sub ChangeInUseFlagReplicate(ByRef dgv As BSDataGridView, ByVal RowIndex As Integer)
        Try
            Me.Cursor = Cursors.WaitCursor 'RH 20/10/2010

            Dim ExecutionResultRow As ExecutionsDS.vwksWSExecutionsResultsRow

            If Not dgv.Rows(RowIndex).Tag Is Nothing Then
                ExecutionResultRow = CType(dgv.Rows(RowIndex).Tag, ExecutionsDS.vwksWSExecutionsResultsRow)
            Else
                Return
            End If

            'AG 15/09/2010 - dont allow discard replicates with ErrorAbs (same as A25)
            If Not ExecutionResultRow.IsABS_ErrorNull Then
                If ExecutionResultRow.ABS_Error <> "" Then Return
            End If
            'END AG 15/09/2010

            Dim myRecalDelegate As New RecalculateResultsDelegate
            myRecalDelegate.AnalyzerModel = AnalyzerModel
            Dim myGlobal As GlobalDataTO

            'Me.Cursor = Cursors.WaitCursor   'RH 20/10/2010  'AG 14/10/2010
            myGlobal = myRecalDelegate.ChangeInUseFlagReplicate(Nothing, AnalyzerIDField, WorkSessionIDField, ExecutionResultRow.ExecutionID, Not ExecutionResultRow.InUse)

            Dim actionAllowed As Boolean = False
            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                actionAllowed = CType(myGlobal.SetDatos, Boolean)

                If actionAllowed Then
                    'Update screen global DS with the affected results
                    Me.LoadExecutionsResults()
                    Me.UpdateCurrentResultsGrid(False)
                    Me.DrawChart() 'PG 18/10/2010  'AG 030/09/2010 - redraw curve after recalculations
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ChangeInUseFlagReplicate ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            'Me.Cursor = Cursors.Default 'RH 20/10/2010   'AG 14/10/2010

        Finally
            Me.Cursor = Cursors.Default  'RH 20/10/2010

        End Try

    End Sub

    ''' <summary>
    ''' User changes some curve definition parameter
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: RH 27/05/2011 Based on a previous version by AG 03/09/2010
    ''' AG 03/07/2012 - Use Nothing instead of Connection
    ''' </remarks>
    Private Function RecalculateCurveAfterDefinitionChanges() As GlobalDataTO
        Dim resultData As GlobalDataTO
        'Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            Dim OperationSuccess As Boolean = False

            Cursor = Cursors.WaitCursor

            'resultData = DAOBase.GetOpenDBTransaction(Nothing)
            'If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
            '    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

            '1) Get the current TestId and SampleType
            Dim testInformation As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                        (From row In ExecutionResults.vwksWSExecutionsResults _
                         Where row.TestName = SelectedTestName AndAlso row.SampleClass = "CALIB" _
                         And row.RerunNumber = AcceptedRerunNumber _
                         AndAlso row.SampleType = SelectedSampleType _
                         Select row).ToList()

            If testInformation.Count > 0 Then
                Dim myTestID As Integer = testInformation.First.TestID
                Dim mySampleType As String = testInformation.First.SampleType
                Dim myOrderTestID As Integer = testInformation.First.OrderTestID

                '2) Get the test calibration programming current definition
                Dim myTestCalibratorsDelegate As New TestCalibratorsDelegate
                resultData = myTestCalibratorsDelegate.GetTestCalibratorByTestID(Nothing, myTestID, mySampleType)

                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then '(1)
                    Dim myLocalTestCalibDS As New TestCalibratorsDS
                    myLocalTestCalibDS = CType(resultData.SetDatos, TestCalibratorsDS)

                    If myLocalTestCalibDS.tparTestCalibrators.Rows.Count > 0 Then
                        With myLocalTestCalibDS.tparTestCalibrators(0)
                            .BeginEdit()
                            .CurveType = CalibrationCurveCombo.SelectedValue.ToString()
                            .CurveAxisXType = XAxisCombo.SelectedValue.ToString()
                            .CurveAxisYType = YAxisCombo.SelectedValue.ToString()
                            If bsDecreasingRadioButton.Checked Then
                                .CurveGrowthType = "DEC"
                            Else
                                .CurveGrowthType = "INC"
                            End If
                            .EndEdit()
                        End With

                        '3) Update test calibration programming
                        Dim mypTestCalibratorsDelegate As New TestCalibratorsDelegate
                        resultData = mypTestCalibratorsDelegate.Update(Nothing, myLocalTestCalibDS)
                        If Not resultData.HasError Then '(2)

                            '4) Call recalculations class (we need the executions owner of the max replicate and max multipointnumber
                            'Get the maximum replicate belongs the selected OrderTestID, RerunNumber 
                            Dim maxItemNumber As Integer = _
                                            (From row In ExecutionResults.vwksWSExecutionsResults _
                                             Where row.OrderTestID = myOrderTestID _
                                             And row.RerunNumber = AcceptedRerunNumber _
                                             Select row.MultiItemNumber).Max

                            Dim maxReplicate As Integer = _
                                        (From row In ExecutionResults.vwksWSExecutionsResults _
                                         Where row.OrderTestID = myOrderTestID _
                                         And row.RerunNumber = AcceptedRerunNumber _
                                         Select row.ReplicateNumber).Max

                            Dim executionToRecalculate As Integer = _
                                        (From row In ExecutionsResultsDSField.vwksWSExecutionsResults _
                                         Where row.OrderTestID = myOrderTestID _
                                         And row.RerunNumber = AcceptedRerunNumber _
                                         And row.MultiItemNumber = maxItemNumber _
                                         And row.ReplicateNumber = maxReplicate _
                                         Select row.ExecutionID).Max

                            Dim myRecalDelegate As New RecalculateResultsDelegate
                            myRecalDelegate.AnalyzerModel = AnalyzerModel
                            resultData = myRecalDelegate.RecalculateResults(Nothing, AnalyzerIDField, WorkSessionIDField, executionToRecalculate, True)

                            If Not resultData.HasError Then
                                OperationSuccess = True
                            End If
                        End If 'If Not resultData.HasError Then (3)
                    End If 'If Not resultData.HasError Then (2)
                End If 'If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then (1)

                If OperationSuccess Then
                    'DAOBase.CommitTransaction(dbConnection)
                Else
                    'If Not dbConnection Is Nothing Then DAOBase.RollbackTransaction(dbConnection)
                End If

            End If
            'End If

            'RH 27/05/2011
            If OperationSuccess Then
                'Update screen global DS with the affected results if no error
                If Not resultData.HasError Then
                    Me.LoadExecutionsResults()
                    Me.UpdateCurrentResultsGrid(False)
                    Me.DrawChart()
                End If
            End If

        Catch ex As Exception
            'If Not dbConnection Is Nothing Then DAOBase.RollbackTransaction(dbConnection)

            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "RecalculateResultsDelegate.RecalculateCurveAfterDefinitionChanges", EventLogEntryType.Error, False)

        Finally
            'If Not dbConnection Is Nothing Then dbConnection.Close()
            Me.Cursor = Cursors.Default

        End Try

        Return resultData
    End Function
#End Region

#Region "NEW METHODS FOR PERFORMANCE IMPROVEMENTS"
    ''' <summary>
    ''' Changes the InUse flag of a replicate and updates the DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 24/07/2012 - Based in ChangeInUseFlagReplicate
    ''' </remarks>
    Private Sub ChangeInUseFlagReplicateNEW(ByRef dgv As BSDataGridView, ByVal RowIndex As Integer)
        Try
            Me.Cursor = Cursors.WaitCursor

            Dim executionResultRow As ExecutionsDS.vwksWSExecutionsResultsRow
            If (Not dgv.Rows(RowIndex).Tag Is Nothing) Then
                executionResultRow = CType(dgv.Rows(RowIndex).Tag, ExecutionsDS.vwksWSExecutionsResultsRow)
            Else
                Return
            End If

            'Discard Replicates with ErrorABS is not allowed (same as A25)
            If (Not executionResultRow.IsABS_ErrorNull AndAlso executionResultRow.ABS_Error <> String.Empty) Then Return

            Dim myGlobal As GlobalDataTO
            Dim myRecalDelegate As New RecalculateResultsDelegate
            myRecalDelegate.AnalyzerModel = AnalyzerModel

            myGlobal = myRecalDelegate.ChangeInUseFlagReplicateNEW(executionResultRow, Not executionResultRow.InUse)
            'myGlobal = myRecalDelegate.ChangeInUseFlagReplicate(Nothing, AnalyzerIDField, WorkSessionIDField, executionResultRow.ExecutionID, Not executionResultRow.InUse)
            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                Dim actionAllowed As Boolean = CType(myGlobal.SetDatos, Boolean)

                If (actionAllowed) Then
                    'Update screen global DS with the affected results
                    Me.LoadExecutionsResults()
                    Me.UpdateCurrentResultsGrid(False)
                    Me.DrawChart()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ChangeInUseFlagReplicateNEW ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ChangeInUseFlagReplicateNEW", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Executes recalculations when the User changes at least one of Curve Parameters
    ''' </summary>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by:  SA - Based in RecalculateCurveAfterDefinitionChanges
    ''' </remarks>
    Private Function RecalculateCurveAfterDefinitionChangesNEW() As GlobalDataTO
        Dim resultData As GlobalDataTO

        Try
            Dim operationSuccess As Boolean = False
            Cursor = Cursors.WaitCursor

            'Get the current TestID and SampleType
            Dim selectedExecRow As ExecutionsDS.vwksWSExecutionsResultsRow = (From row As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionResults.vwksWSExecutionsResults _
                                                                             Where row.TestName = SelectedTestName _
                                                                           AndAlso row.SampleClass = "CALIB" _
                                                                           AndAlso row.RerunNumber = AcceptedRerunNumber _
                                                                           AndAlso row.SampleType = SelectedSampleType _
                                                                            Select row).First

            Dim myTestID As Integer = selectedExecRow.TestID
            Dim mySampleType As String = selectedExecRow.SampleType
            Dim myOrderTestID As Integer = selectedExecRow.OrderTestID

            'Get the Calibration Programming data defined for the TestID/SampleType
            Dim myTestCalibratorsDelegate As New TestCalibratorsDelegate

            resultData = myTestCalibratorsDelegate.GetTestCalibratorByTestID(Nothing, myTestID, mySampleType)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myLocalTestCalibDS As TestCalibratorsDS = DirectCast(resultData.SetDatos, TestCalibratorsDS)

                If myLocalTestCalibDS.tparTestCalibrators.Rows.Count > 0 Then
                    With myLocalTestCalibDS.tparTestCalibrators(0)
                        .BeginEdit()
                        .CurveType = CalibrationCurveCombo.SelectedValue.ToString()
                        .CurveAxisXType = XAxisCombo.SelectedValue.ToString()
                        .CurveAxisYType = YAxisCombo.SelectedValue.ToString()
                        If (bsDecreasingRadioButton.Checked) Then
                            .CurveGrowthType = "DEC"
                        Else
                            .CurveGrowthType = "INC"
                        End If
                        .EndEdit()
                    End With

                    'Update calibration programming data for the TestID/SampleType
                    resultData = myTestCalibratorsDelegate.Update(Nothing, myLocalTestCalibDS)
                    If (Not resultData.HasError) Then
                        'Get the maximum MultiItemNumber for the selected OrderTestID/RerunNumber 
                        Dim maxItemNumber As Integer = (From row As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionResults.vwksWSExecutionsResults _
                                                       Where row.OrderTestID = myOrderTestID _
                                                     AndAlso row.RerunNumber = AcceptedRerunNumber _
                                                      Select row.MultiItemNumber).Max

                        'Get all data of the Execution for the maximum MultiItemNumber and ReplicateNumber 
                        Dim executionRowToRecalculate As ExecutionsDS.vwksWSExecutionsResultsRow
                        executionRowToRecalculate = (From row As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionResults.vwksWSExecutionsResults _
                                                    Where row.OrderTestID = myOrderTestID _
                                                  AndAlso row.RerunNumber = AcceptedRerunNumber _
                                                  AndAlso row.MultiItemNumber = maxItemNumber _
                                                 Order By row.ReplicateNumber Descending _
                                                   Select row).First

                        Dim myRecalDelegate As New RecalculateResultsDelegate
                        myRecalDelegate.AnalyzerModel = AnalyzerModel
                        resultData = myRecalDelegate.RecalculateResultsNEW(Nothing, selectedExecRow, executionRowToRecalculate, True, False)
                        'resultData = myRecalDelegate.RecalculateResults(Nothing, AnalyzerIDField, WorkSessionIDField, executionToRecalculate, True)

                        operationSuccess = (Not resultData.HasError)
                    End If
                End If
            End If

            If (operationSuccess) Then
                'Update screen global DS with the affected results if no error
                Me.LoadExecutionsResults()
                Me.UpdateCurrentResultsGrid(False)
                Me.DrawChart()
            End If
        Catch ex As Exception
            resultData = New GlobalDataTO()
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "RecalculateResultsDelegate.RecalculateCurveAfterDefinitionChangesNEW", EventLogEntryType.Error, False)
        Finally
            Me.Cursor = Cursors.Default
        End Try
        Return resultData
    End Function

#End Region
End Class

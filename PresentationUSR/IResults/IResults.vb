Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.PresentationCOM
Imports System.Text
Imports System.ComponentModel
Imports LIS.Biosystems.Ax00.LISCommunications

Public Class IResults
    'RH 13/12/2010 Substitute every "And" by "AndAlso" (Only in boolean expressions, not in bitwise expressions!)
    '              Substitute every "Or" by "OrElse" (Only in boolean expressions, not in bitwise expressions!)
    'To evaluate the boolean expressions in short circuit and speed up mean processing velocity

    'http://msdn.microsoft.com/en-us/library/8067cy78(v=VS.90).aspx
    'In Visual Basic 2008, the And, Or, Not, and Xor operators still evaluate all expressions contributing
    'to their operands. Visual Basic 2008 also introduces two new operators, AndAlso and OrElse, that can
    'reduce execution time by short-circuiting logical evaluations. If the first operand of an AndAlso
    'operator evaluates to False, the second operand is not evaluated. Similarly, if the first operand of
    'an OrElse operator evaluates to True, the second operand is not evaluated.

    'Description of "short-circuit" evaluation in Visual Basic
    'http://support.microsoft.com/kb/817250/en-us

#Region "Declarations"
    Private Const CollapseColName As String = "Collapse"
    Private XtraCollapseColName As String = String.Empty 'It should be initialized
    Private RepImage As Byte() = Nothing
    Private OKImage As Byte() = Nothing
    Private UnCheckImage As Byte() = Nothing
    Private KOImage As Byte() = Nothing
    Private NoImage As Byte() = Nothing
    Private ClassImage As Byte() = Nothing
    Private ABS_GRAPHImage As Byte() = Nothing
    Private AVG_ABS_GRAPHImage As Byte() = Nothing
    Private CURVE_GRAPHImage As Byte() = Nothing
    Private INC_NEW_REPImage As Byte() = Nothing
    Private RED_NEW_REPImage As Byte() = Nothing
    Private EQ_NEW_REPImage As Byte() = Nothing
    Private NO_NEW_REPImage As Byte() = Nothing
    Private INC_SENT_REPImage As Byte() = Nothing
    Private RED_SENT_REPImage As Byte() = Nothing
    Private EQ_SENT_REPImage As Byte() = Nothing
    Private REP_INCImage As Byte() = Nothing
    Private EQUAL_REPImage As Byte() = Nothing
    Private RED_REPImage As Byte() = Nothing
    Private PATIENT_STATImage As Byte() = Nothing
    Private PATIENT_ROUTINEImage As Byte() = Nothing
    Private SampleIconList As ImageList
    Private TestTypeIconList As ImageList
    Private XtraGridIconList As ImageList
    Private CollapseIconList As ImageList
    Private XtraVerticalBar As Image

    Private StrikeFont As Font = New Font("Verdana", 8.25!, FontStyle.Strikeout, GraphicsUnit.Point, 0)
    Private RegularFont As Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, 0)
    Private XtraStrikeFont As Font = New Font("Tahoma", 8.25!, FontStyle.Strikeout, GraphicsUnit.Point, 0)

    Private RegularBkColor As Color = Color.White
    Private RegularForeColor As Color = Color.Black
    Private StrikeBkColor As Color = Color.LightGray
    Private StrikeForeColor As Color = Color.Gray
    Private AverageBkColor As Color = Color.LightSlateGray
    Private AverageForeColor As Color = Color.White
    Private AverageResultsDS As New ResultsDS
    Private ExecutionsResultsDS As ExecutionsDS
    Private IsColCollapsed As New Dictionary(Of String, Boolean)
    Private IsTestSTD As New Dictionary(Of String, Boolean)
    Private SamplesListViewText As String = String.Empty
    Private SamplesListViewIndex As Integer = -1
    Private TestsListViewText As String = String.Empty
    Private LotListViewText As String = String.Empty
    Private CalibratorListViewText As String = String.Empty
    Private ExperimentalSampleIndex As Integer = -1
    Private FullTestName As String = String.Empty
    Private BlankTestName As String = String.Empty
    Private CalibratorTestName As String = String.Empty
    Private ControlTestName As String = String.Empty
    Private SampleTestName As String = String.Empty
    Private ProcessEvent As Boolean = True

    'Indicates if the patient list must be refreshed after recalculations or new result received
    Private UpdatePatientList As Boolean = True

    Private WithEvents ResultsChart As bsResultsChart 'RH 13/12/2010 Remove New because it creates an object that wont be used.
    Private PrintImage As Image

    Private CurveToolTip As String
    Private CreatingXlsResults As Boolean
    Private ChangeWS As Boolean = False

    Enum SortType
        ASC
        DESC
    End Enum

    Private ExperimentalSortType As New Dictionary(Of String, SortType)
    Private CalibratorSortType As New Dictionary(Of String, SortType)
    Private BlankSortType As New Dictionary(Of String, SortType)
    Private ControlSortType As New Dictionary(Of String, SortType)
    Private SampleSortType As New Dictionary(Of String, SortType)
    Private IsOrderPrinted As New Dictionary(Of String, Boolean)
    Private IsOrderHISSent As New Dictionary(Of String, Boolean)
    Private LanguageID As String
    Private NewFactorValue As Single = Single.NaN
    Private OldFactorValue As Single = 0
    Private labelReportPrintAvailable As String = String.Empty
    Private labelReportPrintNOTAvailable As String = String.Empty
    Private labelHISSent As String = String.Empty
    Private labelHISNOTSent As String = String.Empty
    Private labelOpenAbsorbanceCurve As String = String.Empty

    Private PrintPictureBox As New PictureBox()
    Private HISPictureBox As New PictureBox()

    Private LISSubHeaderImage As Byte() = Nothing
    Private LISHeadImage As Image = Nothing
    Private LISHeadCheckImage As Image = Nothing 'IT 21/10/2014: BA-2036
    Private PrintHeadImage As Image = Nothing
    Private LISExperimentalHeadImage As Image = Nothing
    Private LISControlHeadImage As Image = Nothing
    Private LISSamplesHeadImage As Image = Nothing
    Private HeadImageSide As Integer = 16 'Set here the dimensions of the head images
    Private HeadRect As Rectangle = Nothing
    
    Private labelManualRerunIncrease As String = String.Empty
    Private labelManualRerunDecrease As String = String.Empty
    Private labelManualRerunEqual As String = String.Empty
    Private labelPatient As String = String.Empty
    Private labelRerun As String = String.Empty
    Private labelConcentration As String = String.Empty
    Private labelUnit As String = String.Empty
    Private labelType As String = String.Empty

    Private SamplesAverageList As List(Of ResultsDS.vwksResultsRow)
    Private HeadersCount As Integer = 0
    Private HeaderIndex As Integer = 0

    Private tblXtraSamples As ResultsDS.XtraSamplesDataTable

    Private UpdateSubHeaders As Boolean = False
    Private XtraSamplesCollapsed As Boolean = False
    Private mdiAnalyzerCopy As AnalyzerManager 'AG 19/01/2012

    'BA-1927: added to verify if it is possible to execute the Export process
    Private mdiESWrapperCopy As ESWrapper

    Private Shared OpenForms As Integer = 0
    Private copyRefreshDS As UIRefreshDS = Nothing

    Private LISNameForColumnHeaders As String = "LIS"
#End Region

#Region "Fields (attributes)"

    Private WorkSessionIDField As String = ""
    Private AnalyzerIDField As String = ""
    Private WSStatusField As String = "" 'AG 19/03/2012
    Private LISWorkingModeRerunsAttr As String = "BOTH" 'SGM 17/04/2013

    Private SampleClassField As String      ' XB 26/02/2014 - task #1529
    Private SampleOrTestNameField As String ' XB 26/02/2014 - task #1529
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

    'XB 26/02/2014 - task #1529
    Public Property SampleClass As String
        Get
            Return SampleClassField
        End Get
        Set(value As String)
            SampleClassField = value
        End Set
    End Property
    Public Property SampleOrTestName As String
        Get
            Return SampleOrTestNameField
        End Get
        Set(value As String)
            SampleOrTestNameField = value
        End Set
    End Property
    ' XB 26/02/2014 - task #1529

    'AG 24/05/2011 - used for refresh calibration curve screen (real time refresh)
    Public ReadOnly Property AverageResults() As ResultsDS
        Get
            Return AverageResultsDS
        End Get
    End Property

    'AG 24/05/2011 - used for refresh calibration curve screen  (real time refresh)
    Public ReadOnly Property ExecutionResults() As ExecutionsDS
        Get
            Return ExecutionsResultsDS
        End Get
    End Property

    'AG 24/05/2011  - used for refresh calibration curve screen (real time refresh)
    Public ReadOnly Property SelectedTestName() As String
        Get
            Return TestsListViewText
        End Get
    End Property

    'AG 24/05/2011  - used for refresh calibration curve screen (real time refresh)
    Public ReadOnly Property AcceptedRerunNumber() As Integer
        Get
            Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                 (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                Where String.Compare(row.TestName, TestsListViewText, False) = 0 AndAlso row.SampleClass = "CALIB" _
                Select row).ToList()

            If TestList.Count > 0 Then
                Return (From row In AverageResultsDS.vwksResults _
                                Where row.OrderTestID = TestList(0).OrderTestID _
                                AndAlso row.AcceptedResultFlag = True _
                                Select row.RerunNumber).First
            Else
                Return 1
            End If
        End Get
    End Property

    Public Property ActiveWSStatus() As String
        Get
            Return WSStatusField
        End Get
        Set(ByVal value As String)
            WSStatusField = value
        End Set
    End Property

    ''SGM 17/04/2013
    Private ReadOnly Property LISWorkingModeReruns As String
        Get
            Return LISWorkingModeRerunsAttr
        End Get
    End Property

#End Region

#Region "Events"
    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 11/11/2010
    ''' </remarks>
    Private Sub ResultForm_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then ExitButton.PerformClick()
    End Sub

    ''' <summary>
    ''' Substracts one to OpenForms when the form is closed.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 11/04/2012
    ''' </remarks>
    Private Sub IResults_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If OpenForms > 0 Then
            OpenForms -= 1
        End If
        'ReleaseElement()
    End Sub

    ''' <summary>
    ''' Adds one to OpenForms when the form is shown. Avoids more than one instances open at the same time.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 11/04/2012
    ''' </remarks>
    Private Sub IResults_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        OpenForms += 1
        If OpenForms > 1 Then Close()
    End Sub

    ''' <summary>
    ''' Screen load 
    ''' </summary>
    ''' <remarks>
    ''' Created by:
    ''' Modified by: SA 19/09/2014 - BA-1927 ==> Get a copy of the ESWrapper to check the status of the LIS Connection before execute the 
    '''                                          Export to LIS process
    ''' </remarks>
    Private Sub ResultForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            'startTime = Now 'AG 04/06/2012
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'TR 16/05/2012 -Get the current level
            'Dim myGlobalbase As New GlobalBase
            CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel
            'TR 16/05/2012 -END

            'TR 11/07/2012 -Get the current Language from the current Application Session
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage


            WorkSessionIDField = UiAx00MainMDI.ActiveWorkSession
            AnalyzerIDField = UiAx00MainMDI.ActiveAnalyzer
            'AnalyzerModelField = IAx00MainMDI.AnalyzerModel 'AG 03/07/2012 - comment because causes AnalyzerModelField = ""

            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) 'AG 19/01/2012 - Use the same AnalyzerManager as the MDI
            End If

            'BA-1927: added to verify if it is possible to execute the Export process
            If (Not AppDomain.CurrentDomain.GetData("GlobalLISManager") Is Nothing) Then
                mdiESWrapperCopy = CType(AppDomain.CurrentDomain.GetData("GlobalLISManager"), ESWrapper) 'AG 11/03/2013 - Use the same ESWrapper as the MDI
            End If

            InitializeScreen()

            'RH 15/12/2010
            ResetBorder()

            'AG 21/03/2012 - If WS aborted then show message in the app status bar
            If String.Equals(WSStatusField, "ABORTED") Then bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
            'AG 21/03/2012

            'TR 16/05/2012 -Validate the user level.
            ScreenStatusByUserLevel()

            CollapseAll(bsExperimentalsDataGridView) 'dl 23/07/2013 

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("IResults LOAD (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.ResultForm_Load", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultForm_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    Private Sub bsTestsListDataGridView_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsTestsListDataGridView.SelectionChanged
        Try
            If Not ProcessEvent Then Return

            If bsTestsListDataGridView.SelectedRows.Count > 0 Then
                TestsListViewText = bsTestsListDataGridView.SelectedRows(0).Cells("TestName").Value.ToString()
                RepaintCurrentResultsGrid()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestsListDataGridView_SelectionChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Manage the tab change (selection between Patients/Tests views)
    ''' </summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 18/09/2014 - BA-1927 ==> Code moved to a function
    ''' </remarks>
    Private Sub bsTestDetailsTabs_Selected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles bsTestDetailsTabControl.Selected
        Try
            TestDetailsTabSelectedEvent()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsTestDetailsTabs_Selected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Manage the Sample Class tab change in Tests View
    ''' </summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 18/09/2014 - BA-1927 ==> Code moved to a function
    ''' </remarks>
    Private Sub bsResultsTabControl_Selected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles bsResultsTabControl.Selected
        Try
            ResultsTabControlSelectedEvent()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsResultsTabControl_Selected ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 19/09/2014 - BA-1927 ==> Fixed errors raised when Option Strict On for the screen was activated (Graph column)
    ''' </remarks>
    Private Sub GenericDataGridView_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
            Handles bsExperimentalsDataGridView.CellMouseEnter, bsBlanksDataGridView.CellMouseEnter, bsCalibratorsDataGridView.CellMouseEnter, _
                    bsControlsDataGridView.CellMouseEnter

        Try
            Dim dgv As BSDataGridView = CType(sender, BSDataGridView)
            Dim ColumnName As String = dgv.Columns(e.ColumnIndex).Name

            If e.RowIndex < 0 Then
                Select Case dgv.Name
                    Case bsExperimentalsDataGridView.Name
                        If ExperimentalSortType.ContainsKey(ColumnName) Then
                            dgv.Cursor = Cursors.Hand
                            Return
                        End If

                    Case bsBlanksDataGridView.Name
                        If BlankSortType.ContainsKey(ColumnName) Then
                            dgv.Cursor = Cursors.Hand
                            Return
                        End If

                    Case bsCalibratorsDataGridView.Name
                        If CalibratorSortType.ContainsKey(ColumnName) Then
                            dgv.Cursor = Cursors.Hand
                            Return
                        End If

                    Case bsControlsDataGridView.Name
                        If ControlSortType.ContainsKey(ColumnName) Then
                            dgv.Cursor = Cursors.Hand
                            Return
                        End If

                End Select
            End If

            Select Case ColumnName
                Case CollapseColName
                    If e.RowIndex < 0 Then
                        If Not CType(dgv.Columns(e.ColumnIndex), bsDataGridViewCollapseColumn).IsEnabled Then Return

                        dgv.Cursor = Cursors.Hand
                    Else
                        If Not CType(dgv(e.ColumnIndex, e.RowIndex), bsDataGridViewCollapseCell).IsEnabled Then Return

                        If IsSubHeader(dgv, e.RowIndex) Then
                            dgv.Cursor = Cursors.Hand
                        Else
                            dgv.Cursor = Cursors.Default
                        End If
                    End If

                Case "NewRep"
                    If e.RowIndex < 0 Then
                        dgv.Cursor = Cursors.Default
                    Else
                        'If IsSubHeader(dgv, e.RowIndex) AndAlso _
                        '  Not dgv(e.ColumnIndex, e.RowIndex).Value Is NoImage Then
                        If Not (dgv("NewRep", e.RowIndex).Value Is NoImage OrElse dgv("NewRep", e.RowIndex).Value Is INC_SENT_REPImage _
                                OrElse dgv("NewRep", e.RowIndex).Value Is RED_SENT_REPImage OrElse dgv("NewRep", e.RowIndex).Value Is EQ_SENT_REPImage) Then
                            dgv.Cursor = Cursors.Hand
                        Else
                            dgv.Cursor = Cursors.Default
                        End If
                    End If

                Case "Ok"
                    If e.RowIndex < 0 Then
                        dgv.Cursor = Cursors.Default
                    Else
                        If IsSubHeader(dgv, e.RowIndex) Then
                            dgv.Cursor = Cursors.Hand
                        Else
                            dgv.Cursor = Cursors.Default
                        End If
                    End If

                Case "Graph"
                    If e.RowIndex < 0 Then
                        dgv.Cursor = Cursors.Default
                    Else
                        If Not dgv(e.ColumnIndex, e.RowIndex).Value Is NoImage Then
                            dgv.Cursor = Cursors.Hand
                        Else
                            dgv.Cursor = Cursors.Default
                        End If
                    End If

                    'AG 18/07/2012 - column removed
                    ' dl 23/03/2011
                    'Case "Curve"
                    '    If e.RowIndex < 0 Then
                    '        dgv.Cursor = Cursors.Default
                    '    Else
                    '        If Not dgv(e.ColumnIndex, e.RowIndex).Value Is NoImage Then
                    '            dgv.Cursor = Cursors.Hand
                    '        Else
                    '            dgv.Cursor = Cursors.Default
                    '        End If
                    '    End If
                    '    ' end dl 23/03/2011

                Case "Factor"
                    If e.RowIndex < 0 Then
                        dgv.Cursor = Cursors.Default
                    Else
                        'BA-1927: Added ToString to compare value of Tag property in Graph column, but checking previously that it is not Nothing
                        If IsSubHeader(dgv, e.RowIndex) AndAlso (dgv("Graph", e.RowIndex).Tag Is Nothing OrElse dgv("Graph", e.RowIndex).Tag.ToString <> "CURVE") Then ' bsCurveButton.Visible dl 23/03/2011
                            dgv.Cursor = Cursors.Hand
                        Else
                            dgv.Cursor = Cursors.Default
                        End If
                    End If

                Case Else
                    dgv.Cursor = Cursors.Default
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GenericDataGridView_CellMouseEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub GenericDataGridView_CellMouseLeave(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
        Handles bsBlanksDataGridView.CellMouseLeave, bsCalibratorsDataGridView.CellMouseLeave, bsControlsDataGridView.CellMouseLeave, _
                 bsExperimentalsDataGridView.CellMouseLeave, bsSamplesListDataGridView.CellMouseLeave

        Try
            Dim dgv As BSDataGridView = CType(sender, BSDataGridView)

            If e.RowIndex >= 0 Then
                dgv.Cursor = Cursors.Default
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GenericDataGridView_CellMouseLeave ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 19/09/2014 - BA-1927 ==> Fixed errors raised when Option Strict On for the screen was activated (Graph column)
    ''' </remarks>
    Private Sub GenericDataGridView_CellMouseClick(ByVal sender As System.Object, _
                                                   ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsExperimentalsDataGridView.CellMouseClick, _
                                                                                                                           bsBlanksDataGridView.CellMouseClick, _
                                                                                                                           bsCalibratorsDataGridView.CellMouseClick, _
                                                                                                                           bsControlsDataGridView.CellMouseClick

        Dim resultRow As ResultsDS.vwksResultsRow = Nothing
        Dim key As String = String.Empty
        Dim executionRow As ExecutionsDS.vwksWSExecutionsResultsRow 'SG 30/08/2010
        Dim ParentControl As System.Windows.Forms.Control

        Cursor = Cursors.WaitCursor

        Try
            If sender Is Nothing Then Return
            Dim dgv As BSDataGridView = CType(sender, BSDataGridView)
            Dim myGridSampleClass As String = ""

            Select Case dgv.Name
                Case bsExperimentalsDataGridView.Name
                    myGridSampleClass = "PATIENT"
                    key = SamplesListViewText & "_SAMPLE"
                    ParentControl = bsExperimentalsTabPage

                Case bsBlanksDataGridView.Name
                    myGridSampleClass = "BLANK"
                    key = TestsListViewText & "_BLANK"
                    ParentControl = bsBlanksTabPage

                Case bsCalibratorsDataGridView.Name
                    myGridSampleClass = "CALIB"
                    If (e.RowIndex >= 0) Then
                        If e.ColumnIndex = dgv.Columns("Factor").Index AndAlso IsSubHeader(dgv, e.RowIndex) Then
                            'AG 22/06/2012 - TO CONFIRM
                            'If Not HasCurve AndAlso mdiAnalyzerCopy.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then 'bsCurveButton.Visible Then 'AG note - Allow manual factor only when calibrator 1 point

                            'BA-1927: Added ToString to compare value of Tag property in Graph column, but checking previously that it is not Nothing
                            If (dgv("Graph", e.RowIndex).Tag Is Nothing OrElse dgv("Graph", e.RowIndex).Tag.ToString <> "CURVE") Then 'bsCurveButton.Visible Then 
                                dgv.CurrentCell.Style.BackColor = Color.White
                                dgv.CurrentCell.Style.ForeColor = Color.Black
                                dgv.CurrentCell.Style.Font = RegularFont
                                NewFactorValue = Single.NaN

                                'AG 09/11/2010 - Remember the original factor value
                                If IsNumeric(dgv("Factor", e.RowIndex).Value) Then
                                    OldFactorValue = CType(dgv("Factor", e.RowIndex).Value, Single)
                                    NewFactorValue = OldFactorValue
                                End If
                                'END AG 09/11/2010

                                dgv.BeginEdit(True)
                            End If

                            Return
                        End If

                    End If

                    key = TestsListViewText & "_CALIB"
                    ParentControl = bsCalibratorsTabPage

                Case bsControlsDataGridView.Name
                    myGridSampleClass = "CTRL"
                    key = TestsListViewText & "_CTRL"
                    ParentControl = bsControlsTabPage

                Case Else
                    Return
            End Select

            If (e.RowIndex = -1) AndAlso (e.ColumnIndex = dgv.Columns(CollapseColName).Index) Then
                IsColCollapsed(key) = _
                            CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).IsCollapsed
                'AG 02/08/2010 - Update collapse (general)
                If Not UpdateCollapse(True, IsColCollapsed(key), dgv.Name) Then
                    IsColCollapsed(key) = Not IsColCollapsed(key)
                End If
                'END AG 02/08/2010

                'RH 26/11/2010 Completely out. NOT needed. The collapse column does the work by itself.
                'AG 22/11/2010 - repaint results (temporal solution)
                'If Not IsColCollapsed(key) Then
                '    ExpandAll(dgv) 'RH 25/11/2010
                '    'If bsTestDetailsTabControl.SelectedTab.Name.Equals("bsSamplesTab") Then
                '    '    UpdateExperimentalsDataGrid()
                '    'Else
                '    '    RepaintCurrentResultsGrid()
                '    'End If
                'End If
                'END AG 22/11/2010

                Return
            End If

            If e.RowIndex < 0 Then
                Dim ColName As String = dgv.Columns(e.ColumnIndex).Name
                Select Case dgv.Name
                    Case bsExperimentalsDataGridView.Name
                        If ExperimentalSortType.ContainsKey(ColName) Then
                            If ExperimentalSortType(ColName) = SortType.ASC Then
                                ExperimentalSortType(ColName) = SortType.DESC
                            Else
                                ExperimentalSortType(ColName) = SortType.ASC
                            End If
                            SortDataGrigView(dgv, e.ColumnIndex, ExperimentalSortType(ColName))
                        End If

                    Case bsBlanksDataGridView.Name
                        If BlankSortType.ContainsKey(ColName) Then
                            If BlankSortType(ColName) = SortType.ASC Then
                                BlankSortType(ColName) = SortType.DESC
                            Else
                                BlankSortType(ColName) = SortType.ASC
                            End If
                            SortDataGrigView(dgv, e.ColumnIndex, BlankSortType(ColName))
                        End If

                    Case bsCalibratorsDataGridView.Name
                        If CalibratorSortType.ContainsKey(ColName) Then
                            If CalibratorSortType(ColName) = SortType.ASC Then
                                CalibratorSortType(ColName) = SortType.DESC
                            Else
                                CalibratorSortType(ColName) = SortType.ASC
                            End If
                            SortDataGrigView(dgv, e.ColumnIndex, CalibratorSortType(ColName))
                        End If

                    Case bsControlsDataGridView.Name
                        If ControlSortType.ContainsKey(ColName) Then
                            If ControlSortType(ColName) = SortType.ASC Then
                                ControlSortType(ColName) = SortType.DESC
                            Else
                                ControlSortType(ColName) = SortType.ASC
                            End If
                            SortDataGrigView(dgv, e.ColumnIndex, ControlSortType(ColName))
                        End If

                End Select

                Return
            End If

            Dim changeInUseValue As Boolean = False 'AG 08/08/2010
            Select Case e.ColumnIndex
                Case dgv.Columns(CollapseColName).Index
                    If IsSubHeader(dgv, e.RowIndex) AndAlso (Not dgv.Rows(e.RowIndex).Tag Is Nothing) Then
                        resultRow = CType(dgv.Rows(e.RowIndex).Tag, ResultsDS.vwksResultsRow)
                        resultRow.Collapsed = CType(dgv(CollapseColName, e.RowIndex), bsDataGridViewCollapseCell).IsCollapsed
                        IsColCollapsed.Remove(key)

                        'AG 02/08/2010 - update field collapse (ESSPECIFIC OrderTestID - RerunNumber - MultiPointNumber)
                        If Not Me.UpdateCollapse(False, resultRow.Collapsed, Nothing, resultRow) Then
                            resultRow.Collapsed = Not resultRow.Collapsed
                        End If
                        'END AG 02/08/2010

                        'RH 13/12/2010 Completely out. NOT needed. The collapse column does the work by itself.
                        'AG 22/11/2010 - repaint results (temporal solution)
                        'If Not resultRow.Collapsed Then
                        '    If bsTestDetailsTabControl.SelectedTab.Name.Equals("bsSamplesTab") Then
                        '        UpdateExperimentalsDataGrid()
                        '    Else
                        '        RepaintCurrentResultsGrid()
                        '    End If
                        'End If
                        'END AG 22/11/2010

                        'AG 08/08/2010
                        'Else
                        '    ChangeInUseFlagReplicate(dgv, e.RowIndex)

                        'RH 04/06/2012
                        If bsTestDetailsTabControl.SelectedTab.Name = bsSamplesTab.Name Then
                            SampleTestName = String.Empty
                        End If
                    End If

                Case dgv.Columns("NewRep").Index
                    If IsSubHeader(dgv, e.RowIndex) Then

                        'If Not dgv("NewRep", e.RowIndex).Value Is NoImage Then 'AG 14/08/2010 - Only the last rerun can select the new repetition criterion
                        If Not (dgv("NewRep", e.RowIndex).Value Is NoImage OrElse dgv("NewRep", e.RowIndex).Value Is INC_SENT_REPImage _
                                OrElse dgv("NewRep", e.RowIndex).Value Is RED_SENT_REPImage OrElse dgv("NewRep", e.RowIndex).Value Is EQ_SENT_REPImage) Then
                            'AG 28/07/2010
                            'Select the new repetition criterion
                            resultRow = CType(dgv.Rows(e.RowIndex).Tag, ResultsDS.vwksResultsRow)
                            Dim manualCriteria As GlobalEnumerates.PostDilutionTypes
                            manualCriteria = Me.ShowManualRepetitions(resultRow.OrderTestID, resultRow.TestType, myGridSampleClass)
                            'RH: 27/08/2010
                            If Not manualCriteria = PostDilutionTypes.UNDEFINED Then
                                resultRow.PostDilutionType = manualCriteria.ToString()
                                SetRepPostDilutionImage(dgv, "NewRep", e.RowIndex, resultRow.PostDilutionType, True)
                            End If
                            'END AG 28/07/2010
                        End If 'END AG 14/08/2010

                        'AG 08/08/2010
                        'Else
                        '    ChangeInUseFlagReplicate(dgv, e.RowIndex)
                    End If

                Case dgv.Columns("Graph").Index
                    'BA-1927: Added ToString to compare value of Tag property in Graph column, but checking previously that it is not Nothing
                    If (dgv("Graph", e.RowIndex).Tag Is Nothing OrElse dgv("Graph", e.RowIndex).Tag.ToString <> "CURVE") Then 'AG 18/07/2012
                        'SG 30/08/2010
                        If Not IsSubHeader(dgv, e.RowIndex) Then '(1)

                            'Dim myTest As String = ""
                            'Dim mySample As String = ""

                            If Not dgv("Graph", e.RowIndex).Value Is NoImage Then '(1.2) - AG 22/12/2010 - If no graph image then do not execute following code
                                executionRow = CType(dgv.Rows(e.RowIndex).Tag, ExecutionsDS.vwksWSExecutionsResultsRow)

                                If String.Compare(executionRow.ExecutionType, "PREP_STD", False) = 0 Then '(1.3) - AG 22/12/2010 - ISE results dont have graphical Abs(t)

                                    ShowResultsChart(executionRow.ExecutionID, _
                                                     executionRow.OrderTestID, _
                                                     executionRow.RerunNumber, _
                                                     executionRow.MultiItemNumber, _
                                                     executionRow.ReplicateNumber)
                                End If '(1.3)
                            End If '(1.2)

                        Else '(1)
                            If Not dgv("Graph", e.RowIndex).Value Is NoImage Then '(2.1)
                                resultRow = CType(dgv.Rows(e.RowIndex).Tag, ResultsDS.vwksResultsRow)
                                If resultRow.TestType = "STD" Then '(2.2)
                                    ' DL 11/02/2011
                                    ShowResultsChart(-1, resultRow.OrderTestID, resultRow.RerunNumber, resultRow.MultiPointNumber, -1)
                                End If '(2.2)

                            End If '(2.1)
                        End If '(1)
                        'END SG 30/08/2010
                    End If

                Case dgv.Columns("Ok").Index
                    If Not IsSubHeader(dgv, e.RowIndex) Then
                        'AG 22/06/2012 - TO CONFIRM
                        'If mdiAnalyzerCopy.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then changeInUseValue = True 'AG 22/06/2012 - change replicate inuse allowed only out of Running
                        changeInUseValue = True
                    Else
                        resultRow = CType(dgv.Rows(e.RowIndex).Tag, ResultsDS.vwksResultsRow)
                        If Not resultRow.AcceptedResultFlag Then
                            'AG 22/06/2012 - TO CONFIRM
                            'If mdiAnalyzerCopy.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then ChangeAcceptedResult(dgv, resultRow, myGridSampleClass) 'AG 22/06/2012 - Do not allow change accepted results in RUNNING
                            'ChangeAcceptedResult(dgv, resultRow, myGridSampleClass)
                            'ChangeAcceptedResult(resultRow, myGridSampleClass)
                            ChangeAcceptedResultNEW(resultRow)
                        Else
                            If myGridSampleClass = "PATIENT" OrElse myGridSampleClass = "CTRL" Then
                                RejectResult(dgv, resultRow, myGridSampleClass) 'AG 17/02/2011 - Reject results only for patients
                            End If

                        End If
                        'END AG 04/08/2010
                    End If

                Case Else
                    If Not IsSubHeader(dgv, e.RowIndex) Then
                        'AG 22/06/2012 - TO CONFIRM
                        'If mdiAnalyzerCopy.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then changeInUseValue = True 'AG 22/06/2012 - change replicate inuse allowed only out of Running
                        changeInUseValue = True
                    End If
            End Select

            'AG 18/07/2012 Graph and curve use the same column 'DL 23/03/2011
            'If myGridSampleClass = "CALIB" AndAlso e.ColumnIndex = dgv.Columns("Curve").Index Then

            'BA-1927: Added ToString to compare value of Tag property in Graph column, but checking previously that it is not Nothing
            If (myGridSampleClass = "CALIB") AndAlso (e.ColumnIndex = dgv.Columns("Graph").Index) AndAlso (Not dgv("Graph", e.RowIndex).Tag Is Nothing) AndAlso (dgv("Graph", e.RowIndex).Tag.ToString = "CURVE") Then
                If IsSubHeader(dgv, e.RowIndex) Then '(1)
                    resultRow = CType(dgv.Rows(e.RowIndex).Tag, ResultsDS.vwksResultsRow)
                    LotListViewText = resultRow.CalibratorLotNumber
                    CalibratorListViewText = resultRow.CalibratorName
                    'JC 12/11/2012
                    FullTestName = resultRow.TestName ' & " (" & resultRow.AnalysisMode & ")" '  - " & resultRow.SampleType
                    ShowCurve(resultRow.SampleType)
                End If
            End If
            'AG 18/07/2012 'END DL 23/03/2011

            'AG 09/11/2010
            Dim recoverExperimentalFactor As Boolean = False
            'AG 22/06/2012 - TO CONFIRM
            'If Not changeInUseValue AndAlso mdiAnalyzerCopy.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then 'AG 22/06/2012 - recover experimental factor allowed only out of Running
            If Not changeInUseValue Then
                If dgv.Name = bsCalibratorsDataGridView.Name AndAlso IsSubHeader(dgv, e.RowIndex) AndAlso e.ColumnIndex <> dgv.Columns("Ok").Index Then
                    resultRow = CType(dgv.Rows(e.RowIndex).Tag, ResultsDS.vwksResultsRow)
                    If resultRow.ManualResultFlag Then
                        recoverExperimentalFactor = True
                    End If
                End If
            End If
            'END AG 09/11/2010

            'AG 08/08/2010 - Depending the clicked cell then call the change in use value
            If changeInUseValue Then
                ChangeInUseFlagReplicate(dgv, e.RowIndex)

            ElseIf (recoverExperimentalFactor) Then 'AG 09/11/2010
                'ChangeCalibrationType(False, dgv, resultRow, 1)
                ChangeCalibrationTypeNEW(False, resultRow, 0, dgv)

            End If
            'END AG 08/08/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GenericDataGridView_CellMouseClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub

    Private Sub bsCalibratorsDataGridView_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles bsCalibratorsDataGridView.EditingControlShowing
        Try
            If Not TypeOf e.Control Is TextBox Then Return

            Dim tb As TextBox = CType(e.Control, TextBox)

            If Not tb Is Nothing Then
                RemoveHandler tb.KeyPress, AddressOf dgvTextBox_KeyPress
                AddHandler tb.KeyPress, AddressOf dgvTextBox_KeyPress
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCalibratorsDataGridView_EditingControlShowing ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    Private Sub dgvTextBox_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Try
            If Not ValidateSpecialCharacters(e.KeyChar, "[\d\" & SystemInfoManager.OSDecimalSeparator & "\]") Then e.Handled = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".dgvTextBox_KeyPress ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: SA 19/09/2014 - BA-1927 ==> Fixed errors raised when Option Strict On for the screen was activated (Graph column)
    ''' </remarks>
    Private Sub bsCalibratorsDataGridView_CellValidating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellValidatingEventArgs) Handles bsCalibratorsDataGridView.CellValidating
        Try
            If sender Is Nothing Then Return

            Dim dgv As BSDataGridView = CType(sender, BSDataGridView)
            If dgv.EditingControl Is Nothing Then Return

            'BA-1927: Added ToString to compare value of Tag property in Graph column, but checking previously that it is not Nothing
            If (e.ColumnIndex = dgv.Columns("Factor").Index) AndAlso (IsSubHeader(dgv, e.RowIndex)) AndAlso (dgv("Graph", e.RowIndex).Tag Is Nothing OrElse dgv("Graph", e.RowIndex).Tag.ToString <> "CURVE") Then ' bsCurveButton.Visible Then
                Try
                    Dim FormattedValue As String = e.FormattedValue.ToString()
                    Dim SingleValue As Single = Single.Parse(FormattedValue)

                    'AG 09/11/2010 - If wrong value cancel cell edition
                    If SingleValue > 0 Then
                        NewFactorValue = SingleValue
                    Else
                        dgv.CancelEdit()
                    End If

                Catch ex As Exception
                    'AG 09/11/2010
                    'e.Cancel = True
                    'dgv.EditingControl.BackColor = Color.DarkRed
                    'dgv.EditingControl.ForeColor = Color.White
                    'dgv.EditingPanel.BackColor = Color.DarkRed
                    'ShowMessage("Error", "WRONG_DATATYPE", ex.Message + " ((" + ex.HResult.ToString + "))")
                    NewFactorValue = OldFactorValue
                    dgv.CancelEdit()
                End Try
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCalibratorsDataGridView_CellValidating ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub bsCalibratorsDataGridView_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsCalibratorsDataGridView.CellEndEdit
        Try
            If sender Is Nothing Then Return

            Dim dgv As BSDataGridView = CType(sender, BSDataGridView)
            dgv.CurrentCell.Style.BackColor = AverageBkColor
            dgv.CurrentCell.Style.ForeColor = AverageForeColor

            'If Not Single.IsNaN(NewFactorValue) Then 'We have a new value
            If Not Single.IsNaN(NewFactorValue) AndAlso NewFactorValue <> OldFactorValue _
              AndAlso NewFactorValue > 0 Then 'AG 09/11/2010 - We have a new value and it's different from the original one and > 0
                'AG 09/11/2010
                'dgv.CurrentRow.DefaultCellStyle.Font = StrikeFont
                'dgv.CurrentCell.Style.Font = RegularFont
                'MessageBox.Show("Albert, the New Factor Value is: " & NewFactorValue)
                Dim resultRow As ResultsDS.vwksResultsRow

                resultRow = CType(dgv.Rows(e.RowIndex).Tag, ResultsDS.vwksResultsRow)
                'Me.ChangeCalibrationType(True, dgv, resultRow, NewFactorValue)
                ChangeCalibrationTypeNEW(True, resultRow, NewFactorValue, dgv)
                'END AG 09/11/2010
            Else
                dgv.CancelEdit()
            End If

            'Finally
            NewFactorValue = Single.NaN
            OldFactorValue = 0 'AG 09/11/2010 - reset value
            bsTestDetailsTabControl.Focus()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsCalibratorsDataGridView_CellEndEdit ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: SG 30/08/2010
    ''' </remarks>
    Private Sub ResultsChart_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResultsChart.Validated
        Try
            RemoveResultsChart()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ResultsChart_LostFocus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: SG 30/08/2010
    ''' </remarks>
    Private Sub ResultsChart_Exit() Handles ResultsChart.ExitRequest
        Try
            RemoveResultsChart()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ResultsChart_LostFocus ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prints the Patients Final Report
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 15/05/2012
    ''' </remarks>
    Private Sub PrintReportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintReportButton.Click
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            XRManager.ShowPatientsFinalReport(ActiveAnalyzer, ActiveWorkSession)

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Patients Final Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.PrintReportButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrintReportButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prints the compact version of the patient results report. 
    ''' </summary>
    ''' <remarks>
    ''' Created by: CF 26/09/2013
    ''' </remarks>
    Private Sub PrintCompactReportButton_Click(sender As Object, e As EventArgs) Handles PrintCompactReportButton.Click
        Try
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'Stub
            XRManager.PrintCompactPatientsReport(ActiveAnalyzer, ActiveWorkSession, Nothing, False)

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Patients Compact Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.PrintCompactReportButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrintCompactReportButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prints Work Session Test Results report
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 10/01/2012
    ''' </remarks>
    Private Sub PrintTestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintTestButton.Click
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            XRManager.ShowResultsByTestReport(ActiveAnalyzer, ActiveWorkSession)

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Test Results Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.PrintTestButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrintTestButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by:  JV 21/02/2014 - BT #1502
    ''' </remarks>
    Private Sub PrintTestBlankButton_Click(sender As Object, e As EventArgs) Handles PrintTestBlankButton.Click
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'XRManager.ShowResultsByTestReportCompactBySampleType(ActiveAnalyzer, ActiveWorkSession, "BLANK")

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Test Results Blank Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.PrintTestBlankButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrintTestBlankButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by:  JV 21/02/2014 - BT #1502
    ''' Modified by: IT 01/10/2014 - #BA-1864
    ''' </remarks>
    Private Sub PrintTestCtrlButton_Click(sender As Object, e As EventArgs) Handles PrintTestCtrlButton.Click
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            XRManager.ShowControlsCompactReport(ActiveAnalyzer, ActiveWorkSession, False) 'IT 01/10/2014 - #BA-1864

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Ctrl Test Results Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.PrintTestCtrlButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrintTestCtrlButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prints Work Session "Marked Patients for printing" Results report
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 04/01/2012
    ''' </remarks>
    Private Sub PrintSampleButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintSampleButton.Click
        Try
            XRManager.ShowResultsByPatientSampleReport(ActiveAnalyzer, ActiveWorkSession)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrintSampleButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click
        Try
            'AG 28/05/2014 - New trace
            CreateLogActivity("Start Closing IResults", "IResults.bsExitButton_Click", EventLogEntryType.Information, False)

            'AG 24/02/2014 - use parameter MAX_APP_MEMORYUSAGE into performance counters (but do not show message here!!!) ' XB 18/02/2014 BT #1499
            Dim PCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage)
            PCounters.GetAllCounters()
            PCounters = Nothing
            ' XB 18/02/2014 BT #1499

            ' XB 26/11/2013 - Inform to MDI that this screen is closing aims to open next screen - Task #1303
            ExitingScreen()
            UiAx00MainMDI.EnableButtonAndMenus(True)
            Application.DoEvents()

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'SGM 17/04/2013
            Me.ExitButton.Enabled = False
            Me.ExportButton.Enabled = False
            Me.PrintReportButton.Enabled = False
            Me.bsXlsresults.Enabled = False

            'TR 27/09/2013 -disable new controls
            SummaryButton.Enabled = False
            PrintSampleButton.Enabled = False
            PrintCompactReportButton.Enabled = False

            bsSamplesListDataGridView.Enabled = False
            bsTestsListDataGridView.Enabled = False
            bsBlanksDataGridView.Enabled = False
            bsExperimentalsDataGridView.Enabled = False
            bsCalibratorsDataGridView.Enabled = False
            bsControlsDataGridView.Enabled = False
            SamplesXtraGrid.Enabled = False
            'TR 27/09/2013 -END.

            'When screen closes all ordertests in table repetitionsToAdd are automatically added to WS
            Me.SendManRepButton.Tag = "Closing"
            Me.SendManRepButton.PerformClick()
            'end SGM 17/04/2013

            'RH 16/12/2010
            If Not Tag Is Nothing Then
                'A PerformClick() method was executed
                Close()
            Else
                'TR 18/05/2012 - indicate to monitor form there was a change on the worsession.
                IMonitor.WorkSessionChange = ChangeWS
                'Normal button click
                'Open the WS Monitor form and close this one
                UiAx00MainMDI.OpenMonitorForm(Me)
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("IResults CLOSED (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.ExitButton_Click", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Shows the summary table
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 21/09/2010
    ''' </remarks>
    Private Sub SummaryButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SummaryButton.Click
        Try
            'RH 19/10/2010 Introduce the Using statement
            Using SummaryResult As New IResultsSummaryTable
                SummaryResult.AverageResultsDS = AverageResultsDS
                SummaryResult.ExecutionsResultsDS = ExecutionsResultsDS
                SummaryResult.ActiveAnalyzer = ActiveAnalyzer
                SummaryResult.ActiveWorkSession = ActiveWorkSession
                SummaryResult.ShowDialog(Me)
            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSumaryButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by:  AG 11/05/2011
    ''' Modified by: SA 19/09/2014 - BA-1927 ==> Call new function ActivateDeactivateAllButtons to deactivate/activate buttons when the 
    '''                                          process starts/finishes 
    ''' </remarks>
    Private Sub bsXlsresults_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsXlsresults.Click
        Try
            'Disable all screen buttons, and disable also all buttons and menus in the MainMDI
            ActivateDeactivateAllButtons(False)

            Cursor = Cursors.WaitCursor

            'Get the analyzer status due this method is only allowed in StandBy or Sleep
            Dim myAnalyzerManager As AnalyzerManager
            myAnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)

            If myAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY OrElse myAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING OrElse Not myAnalyzerManager.Connected Then
                bsXlsresults.Enabled = False
                UiAx00MainMDI.SetActionButtonsEnableProperty(False) 'Disable all action button bar

                Dim workingThread As New Threading.Thread(AddressOf ExportResults)
                CreatingXlsResults = True
                ScreenWorkingProcess = True 'AG 08/11/2012 - inform this flag because the MDI requires it
                workingThread.Start()

                While CreatingXlsResults
                    UiAx00MainMDI.InitializeMarqueeProgreesBar()
                    Cursor = Cursors.WaitCursor
                    Application.DoEvents()
                End While

                workingThread = Nothing
                UiAx00MainMDI.StopMarqueeProgressBar()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsXlsresults_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            CreatingXlsResults = False
            ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it
            bsXlsresults.Enabled = True
            UiAx00MainMDI.SetActionButtonsEnableProperty(True) 'Activate action button bar depending Ax00 status, alarms, ...
            Cursor = Cursors.Default

            'Enable all screen buttons, and enable also all buttons and menus in the MainMDI
            ActivateDeactivateAllButtons(True)
        End Try
    End Sub

    ''' <summary>
    ''' Manually LIS export
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 22/03/2011
    ''' Modified by: SG 10/04/2013 - Call method ExportToLISManualNEW with parameters pIncludeSent = TRUE
    '''              AG 13/02/2014 - BT #1505 ==> Results screen export only the still not sent results
    '''              AG 17/02/2014 - BT #1505 ==> Use merge instead of loops
    '''              SA 19/08/2014 - BA-1927  ==> Code moved to new function ExportResultsToLIS
    ''' </remarks>
    Private Sub ExportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportButton.Click
        Try
            ExportResultsToLIS()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExportButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExportButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        ''*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        'Dim StartTime As DateTime = Now
        ''Dim myLogAcciones As New ApplicationLogManager()
        ''*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        'Try

        '    'AG 24/02/2014 - use parameter MAX_APP_MEMORYUSAGE into performance counters (but do not shown message here!!!) ' XB 18/02/2014 BT #1499
        '    Dim PCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage)
        '    PCounters.GetAllCounters()
        '    PCounters = Nothing
        '    ' XB 18/02/2014 BT #1499

        '    Dim resultData As New GlobalDataTO

        '    'TR 04/08/2011 -Enable menu bar.
        '    'IAx00MainMDI.EnableMenusBar(False)
        '    IAx00MainMDI.EnableButtonAndMenus(False) 'TR 04/10/2011 -Implement new method.
        '    ExitButton.Enabled = False
        '    bsXlsresults.Enabled = False
        '    OffSystemResultsButton.Enabled = False
        '    SendManRepButton.Enabled = False
        '    PrintTestButton.Enabled = False
        '    PrintSampleButton.Enabled = False
        '    SummaryButton.Enabled = False
        '    PrintReportButton.Enabled = False
        '    'TR 04/08/2011 -END.

        '    Dim myExport As New ExportDelegate
        '    'resultData = myExport.ExportToLISManual(AnalyzerIDField, WorkSessionIDField)

        '    'AG 29/07/2014 - #1887 - Re-send results
        '    'Export by Patient view uses the OrderToExport field
        '    'Export by Test view uses ??? - PENDING CONFIRM
        '    'resultData = myExport.ExportToLISManualNEW(AnalyzerIDField, WorkSessionIDField, True) 'AG 13/02/2014 - #1505 (results screen export only the still not sent results)
        '    If String.Compare(bsTestDetailsTabControl.SelectedTab.Name, bsSamplesTab.Name, False) = 0 Then
        '        'PATIENTs view re-send process
        '        resultData = myExport.ExportToLISManualNEW(AnalyzerIDField, WorkSessionIDField, True, True) 'Include the already exported results
        '    Else
        '        'TESTs view re-send process
        '        resultData = myExport.ExportToLISManualNEW(AnalyzerIDField, WorkSessionIDField, True) 'From test view user export only the still not sent results!!! (SPECIFICATION: NO changes in export from tests view)
        '    End If
        '    'AG 29/07/2014

        '    'AG 18/03/2013 - upload results to LIS
        '    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '        Dim exportResults As New ExecutionsDS
        '        exportResults = CType(resultData.SetDatos, ExecutionsDS)

        '        'AG 29/07/2014 #1887 Max number of results to export each time 100
        '        Dim maxResultsToExport As Integer = 100 'Default value
        '        Dim swParamDlg As New SwParametersDelegate
        '        resultData = swParamDlg.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_RESULTSTOEXPORT_HIST.ToString, Nothing)
        '        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '            Dim myDS As New ParametersDS
        '            myDS = DirectCast(resultData.SetDatos, ParametersDS)
        '            If myDS.tfmwSwParameters.Rows.Count > 0 Then
        '                If Not myDS.tfmwSwParameters(0).IsValueNumericNull Then
        '                    maxResultsToExport = CInt(myDS.tfmwSwParameters(0).ValueNumeric)
        '                End If
        '            End If
        '        End If

        '        If exportResults.twksWSExecutions.Rows.Count > maxResultsToExport Then
        '            'Show message and leave method - MESSAGE NOT IN DATABASE (same as in historical results!!!)
        '            MessageBox.Show(Me, "Please, export to LIS in groups of " & maxResultsToExport & " results (maximum)", My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Information)
        '            Return
        '        End If
        '        'AG 29/07/2014

        '        If exportResults.twksWSExecutions.Rows.Count > 0 Then 'AG 17/02/2014 - #1505
        '            'Inform the new results to be updated into MDI property
        '            IAx00MainMDI.AddResultsIntoQueueToUpload(exportResults)

        '            'Call assynchronously to a synchronous process using another thread but not wait
        '            'AG + SG 05/04/2013
        '            'Do not use the same AverageResultsDS as 2 different parameters because the Dataset always are treated as ByRef
        '            'and the system loss information
        '            'Solution: create a new variable only with alarms information

        '            'IAx00MainMDI.InvokeUploadResultsLIS(False, AverageResultsDS, AverageResultsDS, Nothing)
        '            Dim myResultsAlarmsDS As New ResultsDS

        '            'AG 17/02/2017 - #1505 improvement, copy DS using Merge instead of loops
        '            'For Each row As ResultsDS.vwksResultsAlarmsRow In AverageResultsDS.vwksResultsAlarms
        '            '    myResultsAlarmsDS.vwksResultsAlarms.ImportRow(row)
        '            'Next
        '            'myResultsAlarmsDS.vwksResultsAlarms.AcceptChanges()

        '            myResultsAlarmsDS.vwksResultsAlarms.Merge(AverageResultsDS.vwksResultsAlarms)
        '            myResultsAlarmsDS.vwksResultsAlarms.AcceptChanges()
        '            'AG 17/02/2014 - #1505

        '            CreateLogActivity("Current Results manual upload", Me.Name & ".ExportButton_Click ", EventLogEntryType.Information, False) 'AG 02/01/2014 - BT #1433 (v211 patch2)
        '            IAx00MainMDI.InvokeUploadResultsLIS(False, AverageResultsDS, myResultsAlarmsDS, Nothing)
        '            'AG + SG 05/04/2013
        '        End If 'AG 17/02/2014 - #1505
        '    End If
        '    'AG 18/03/2013

        'Catch ex As Exception
        '    CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ExportButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
        '    ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        'Finally



        '    'DL 30/09/2011. Improvement + BUGS. ID: 162
        '    UpdateScreenGlobalDSWithAffectedResults()

        '    UpdateSamplesListDataGrid()
        '    If (bsTestDetailsTabControl.SelectedTab.Name = bsTestsTabTage.Name) Then

        '        'If bsTestDetailsTabControl.SelectedTab.Name = bsSamplesTab.Name Then
        '        '    UpdateSamplesListDataGrid()
        '        'Else
        '        Select Case bsResultsTabControl.SelectedTab.Name
        '            Case bsBlanksTabPage.Name
        '                'UpdateBlanksDataGrid()
        '            Case bsCalibratorsTabPage.Name
        '                'UpdateCalibratorsDataGrid()
        '            Case bsControlsTabPage.Name
        '                UpdateControlsDataGrid()
        '            Case XtraSamplesTabPage.Name
        '                UpdateSamplesXtraGrid()
        '        End Select
        '        Application.DoEvents()
        '    End If
        '    'END DL 24/09/2011

        '    'TR 04/08/2011 -Enable menu bar.
        '    'IAx00MainMDI.EnableMenusBar(True)
        '    IAx00MainMDI.EnableButtonAndMenus(True) 'TR 04/10/2011 -Implement new method.
        '    ExitButton.Enabled = True
        '    bsXlsresults.Enabled = True
        '    'OffSystemResultsButton.Enabled = True
        '    'TR 11/07/2012 -Use function that activate offSystems results button.
        '    OffSystemResultsButton.Enabled = OffSystemResultsButtonEnabled()

        '    'AG 19/03/2012
        '    'SendManRepButton.Enabled = True
        '    SendManRepButton.Enabled = IIf(WSStatusField = "ABORTED", False, True)

        '    PrintTestButton.Enabled = True
        '    PrintSampleButton.Enabled = True
        '    SummaryButton.Enabled = True
        '    PrintReportButton.Enabled = True
        '    'TR 04/08/2011 -END.

        '    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '    GlobalBase.CreateLogActivity("Manual Export of Results: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
        '                                    "IResults.ExportButton_Click", EventLogEntryType.Information, False)
        '    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        '    'for refreshing data when ack from lis
        '    ExperimentalSampleIndex = -1 'SGM 16/03/2013

        'End Try

    End Sub

    Private Sub OffSystemResultsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OffSystemResultsButton.Click
        Try
            OpenOffSystemResultsScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OffSystemResultsButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OffSystemResultsButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Add to the Work Session all the Repetitions pending to add
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 08/08/2010
    ''' Modified by: SA 20/03/2012 - Changed verification of ISE Module installed; added verification of ISE Module available and ready. In both cases
    '''                              (not installed or not ready), all pending ISE Executions are marked as locked
    '''              SA 26/07/2012 - Before calling the function to add requested Reruns, verify if the ISE Module is ready to inform the optional parameter
    '''                              that will allow blocking all ISE Executions when the module is not ready
    '''              XB 04/09/2012 - Correction : add 'iseModuleReady' parameter to the function call 'AddManualRepetitions'
    ''' </remarks>
    Private Sub SendManRepButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SendManRepButton.Click
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Dim mySendManRepButton As BSButton = TryCast(sender, BSButton)

            Cursor = Cursors.WaitCursor 'RH 17/05/2012
            mySendManRepButton.Enabled = False 'RH 17/05/2012

            If (String.Compare(AnalyzerIDField, "", False) <> 0 AndAlso WorkSessionIDField <> "") Then
                'Verify if the Analyzer is connected, has an ISE Module installed, and if it is available and ready
                Dim iseModuleReady As Boolean = False
                If (Not mdiAnalyzerCopy Is Nothing AndAlso mdiAnalyzerCopy.Connected) Then
                    iseModuleReady = (Not mdiAnalyzerCopy.ISE_Manager Is Nothing AndAlso mdiAnalyzerCopy.ISE_Manager.IsISEModuleReady)
                End If

                Dim myGlobal As GlobalDataTO
                Dim myRep As New RepetitionsDelegate

                'Verify is the current Analyzer Status is RUNNING
                Dim runningMode As Boolean = (mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.RUNNING)

                'AG 31/03/2014 - #1565 inform new runningmode parameter in the proper position
                myGlobal = myRep.AddManualRepetitions(Nothing, AnalyzerIDField, WorkSessionIDField, runningMode, iseModuleReady)
                'AG 31/03/2014 - #1565

                If (myGlobal.HasError) Then
                    ShowMessage(Me.Name & ".SendManRepButton_Click ", myGlobal.ErrorCode, myGlobal.ErrorCode, Me)
                Else
                    ''If there is an Analyzer connected...
                    'If (Not mdiAnalyzerCopy Is Nothing AndAlso mdiAnalyzerCopy.Connected) Then
                    '    If (mdiAnalyzerCopy.ISE_Manager Is Nothing OrElse Not mdiAnalyzerCopy.ISE_Manager.IsISEModuleReady) Then
                    '        'ISE Module cannot be used; all pending ISE Preparations are LOCKED
                    '        Dim myExecutionDelegate As New ExecutionsDelegate
                    '        myGlobal = myExecutionDelegate.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDField, AnalyzerIDField, _
                    '                                                                            "PREP_ISE", "PENDING", "LOCKED")
                    '        'TR 18/05/2012 indicate there was a change on the worksession.
                    '        ChangeWS = True

                    '    End If
                    'End If

                    ''AG 19/01/2012 - If ISE not installed lock all pending ISE executions after add manual repetitions
                    'If Not mdiAnalyzerCopy Is Nothing AndAlso mdiAnalyzerCopy.Connected Then
                    '    Dim adjustValue As String = ""
                    '    Dim iseInstalledFlag As Boolean = False
                    '    adjustValue = mdiAnalyzerCopy.ReadAdjustValue(GlobalEnumerates.Ax00Adjustsments.ISEINS)
                    '    If adjustValue <> "" AndAlso IsNumeric(adjustValue) Then
                    '        iseInstalledFlag = CType(adjustValue, Boolean)
                    '        If Not iseInstalledFlag Then
                    '            Dim myExecutions As New ExecutionsDelegate
                    '            myGlobal = myExecutions.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDField, AnalyzerIDField, "PREP_ISE", "PENDING", "LOCKED")
                    '        End If
                    '    End If
                    'End If
                    ''AG 19/01/2012

                    'SGM 17/04/2013 - not to refresh grids when screen closing
                    If (mySendManRepButton.Tag Is Nothing) Then
                        UpdateScreenGlobalDSWithAffectedResults()


                        'AG 24/11/2010 - If new patient repetitions are sent maybe the HIS export icon maybe changes
                        If (bsTestDetailsTabControl.SelectedTab.Name = bsSamplesTab.Name) Then
                            UpdateSamplesListDataGrid()
                        Else
                            UpdatePatientList = True
                        End If
                        'AG 24/11/2010 
                    End If
                    'end SGM 17/04/2013

                    'IAx00MainMDI.SetActionButtonsEnableProperty(True) 'AG 18/07/2011 - Activate vertical buttons depending the executions available
                End If
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Send Manual Repetition: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.SendmanRepButton_Click", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SendManRepButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SendManRepButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        Finally
            'SGM 17/04/2013
            If (SendManRepButton.Tag Is Nothing) Then
                UiAx00MainMDI.SetActionButtonsEnableProperty(True) 'RH 17/05/2012
                SendManRepButton.Enabled = True 'RH 17/05/2012
                Cursor = Cursors.Default 'RH 17/05/2012
            End If
            SendManRepButton.Tag = Nothing
            'end SGM 17/04/2013
        End Try
    End Sub

    ''' <summary>
    ''' Draws the icon header for column LISExperimental in bsExperimentalsDataGridView
    ''' </summary>
    ''' <remarks>Created by RH 19/10/2011</remarks>
    Private Sub bsExperimentalsDataGridView_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles bsExperimentalsDataGridView.CellPainting
        Exit Sub 'Not to draw the header icon - SGM 16/04/2013

        'If e.RowIndex = -1 AndAlso e.ColumnIndex = 2 Then
        '    HeadRect = New Rectangle(Convert.ToInt32(e.CellBounds.Left + (e.CellBounds.Width - HeadImageSide) / 2), _
        '                             Convert.ToInt32(e.CellBounds.Top + (e.CellBounds.Height - HeadImageSide) / 2), _
        '                             HeadImageSide, HeadImageSide)

        '    e.Paint(HeadRect, DataGridViewPaintParts.All And Not DataGridViewPaintParts.ContentForeground)
        '    e.Graphics.DrawImage(LISExperimentalHeadImage, HeadRect)
        '    e.Handled = True
        'End If
    End Sub

    ''' <summary>
    ''' Draws the icon header for column LISControl in bsControlsDataGridView
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by RH 19/10/2011</remarks>
    Private Sub bsControlsDataGridView_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles bsControlsDataGridView.CellPainting
        Exit Sub 'Not to draw the header icon - SGM 16/04/2013

        'If e.RowIndex = -1 AndAlso e.ColumnIndex = 2 Then
        '    HeadRect = New Rectangle(Convert.ToInt32(e.CellBounds.Left + (e.CellBounds.Width - HeadImageSide) / 2), _
        '                             Convert.ToInt32(e.CellBounds.Top + (e.CellBounds.Height - HeadImageSide) / 2), _
        '                             HeadImageSide, HeadImageSide)

        '    e.Paint(HeadRect, DataGridViewPaintParts.All And Not DataGridViewPaintParts.ContentForeground)
        '    e.Graphics.DrawImage(LISControlHeadImage, HeadRect)
        '    e.Handled = True
        'End If
    End Sub

    ''' <summary>
    ''' Draws the icon header for column LISSamples in bsSamplesDataGridView
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by RH 19/10/2011</remarks>
    Private Sub bsSamplesDataGridView_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs)
        Exit Sub 'Not to draw the header icon - SGM 16/04/2013

        'If e.RowIndex = -1 AndAlso e.ColumnIndex = 2 Then
        '    HeadRect = New Rectangle(Convert.ToInt32(e.CellBounds.Left + (e.CellBounds.Width - HeadImageSide) / 2), _
        '                             Convert.ToInt32(e.CellBounds.Top + (e.CellBounds.Height - HeadImageSide) / 2), _
        '                             HeadImageSide, HeadImageSide)

        '    e.Paint(HeadRect, DataGridViewPaintParts.All And Not DataGridViewPaintParts.ContentForeground)
        '    e.Graphics.DrawImage(LISSamplesHeadImage, HeadRect)
        '    e.Handled = True
        'End If
    End Sub

    ''' <summary>
    ''' Draws the icon header for column LISExperimental in bsExperimentalsDataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 04/06/2012
    ''' Modified by: SA 19/09/2014 - BA-1927 ==> Fixed errors raised when Option Strict On for the screen was activated (values have to be converted to Integer)
    '''                                          Added Try/Catch block
    ''' </remarks>
    Private Sub bsSamplesListDataGridView_CellPainting(ByVal sender As Object, ByVal e As DataGridViewCellPaintingEventArgs) Handles bsSamplesListDataGridView.CellPainting
        Try
            If (e.RowIndex = -1) Then
                Dim OrderToPrintIndex As Integer = bsSamplesListDataGridView.Columns("OrderToPrint").Index
                Dim OrderToExportIndex As Integer = bsSamplesListDataGridView.Columns("OrderToExport").Index

                If (e.ColumnIndex = OrderToExportIndex OrElse e.ColumnIndex = OrderToPrintIndex) Then
                    'BA-1927 ==> Convert values to Integer
                    HeadRect = New Rectangle(Convert.ToInt32(e.CellBounds.Left + (e.CellBounds.Width - HeadImageSide) / 2), _
                                             Convert.ToInt32(e.CellBounds.Top + (e.CellBounds.Height - HeadImageSide) / 2), _
                                             HeadImageSide, HeadImageSide)

                    e.Paint(HeadRect, DataGridViewPaintParts.All And Not DataGridViewPaintParts.ContentForeground)

                    If (e.ColumnIndex = OrderToPrintIndex) Then
                        e.Graphics.DrawImage(PrintHeadImage, HeadRect)
                        e.Handled = True
                    Else
                        'Not to draw the header icon - SGM 16/04/2013
                        e.Graphics.DrawImage(LISHeadCheckImage, HeadRect) 'IT 21/10/2014: BA-2036
                        e.Handled = True 'IT 21/10/2014: BA-2036
                    End If
                    'e.Handled = True SGM 16/04/2013
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSamplesListDataGridView_CellPainting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

#Region "General Private Methods"
    ''' <summary>
    ''' Gets the list of Order Tests Results from the Executions
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 15/07/2010
    ''' Modified by: AG 01/12/2010 - Adapted for ISE and OFF-SYSTEM tests
    '''              SA 25/01/2011 - Changed the way of getting the Reference Range Intervals, also get all Patients
    '''                              having only OFF-SYSTEM Order Tests and add them to ExecutionsResultsDS.vwksWSExecutionsResults
    '''              RH 31/01/2011 - Remove the previous code for getting OffSystem info because does not work properly.
    '''                              We need executionRow.OrderStatus, which do not exist in ResultsDS.vwksResultsRow.
    '''                              Also breakes the logic of the method. Now all the executions (STD, ISE and OFFS) come
    '''                              at the beginning from myExecutionDelegate.GetWSExecutionsResults() as it was planned before,
    '''                              and not in slices
    '''              AG 25/06/2012 - Improve speed. Do not AverageResultsDS.vwksResults.Clear and then call results order by order test and import
    '''                              Open directly the contents of the new view vwksCompleteAvgResults
    '''              SA 22/09/2014 - BA-1927 ==>
    ''' </remarks>
    Private Sub LoadExecutionsResults()
        Dim StartTime As DateTime = Now 'AG 21/06/2012 - time estimation

        Try
            Dim resultData As GlobalDataTO
            Dim myExecutionDelegate As New ExecutionsDelegate

            resultData = myExecutionDelegate.GetWSExecutionsResults(Nothing, ActiveAnalyzer, ActiveWorkSession)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                ExecutionsResultsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                'Clear Average and Execution Alarms
                AverageResultsDS.vwksResultsAlarms.Clear()
                ExecutionsResultsDS.vwksWSExecutionsAlarms.Clear()

                'Get all results for current Analyzer & WorkSession
                Dim myResultsDelegate As New ResultsDelegate
                resultData = myResultsDelegate.GetCompleteResults(Nothing, ActiveAnalyzer, ActiveWorkSession)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    AverageResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
                Else
                    ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If

                'Read Reference Range Limits
                Dim minimunValue As Nullable(Of Single) = Nothing
                Dim maximunValue As Nullable(Of Single) = Nothing

                Dim panicMinimunValue As Nullable(Of Single) = Nothing
                Dim panicMaximunValue As Nullable(Of Single) = Nothing

                Dim mySampleType As String = String.Empty
                Dim myTestRefRangesDS As TestRefRangesDS
                Dim myOrderTestsDelegate As New OrderTestsDelegate

                For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                    If (Not resultRow.IsActiveRangeTypeNull) Then
                        If (resultRow.TestID = 1000) Then Console.Write("stop")

                        mySampleType = String.Empty
                        If (resultRow.TestType <> "CALC") Then mySampleType = resultRow.SampleType

                        'Get the Reference Range for the Test/SampleType according the TestType and the Type of Range
                        resultData = myOrderTestsDelegate.GetReferenceRangeInterval(Nothing, resultRow.OrderTestID, resultRow.TestType, _
                                                                                    resultRow.TestID, mySampleType, resultRow.ActiveRangeType)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            myTestRefRangesDS = DirectCast(resultData.SetDatos, TestRefRangesDS)

                            If (myTestRefRangesDS.tparTestRefRanges.Rows.Count = 1) Then
                                minimunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalLowerLimit
                                maximunValue = myTestRefRangesDS.tparTestRefRanges(0).NormalUpperLimit

                                'Validate if Panic Limits are not NULL 
                                If (Not myTestRefRangesDS.tparTestRefRanges(0).IsBorderLineLowerLimitNull) AndAlso _
                                   (Not myTestRefRangesDS.tparTestRefRanges(0).IsBorderLineUpperLimitNull) Then
                                    panicMinimunValue = myTestRefRangesDS.tparTestRefRanges(0).BorderLineLowerLimit
                                    panicMaximunValue = myTestRefRangesDS.tparTestRefRanges(0).BorderLineUpperLimit
                                End If
                            End If
                        End If

                        If (minimunValue.HasValue AndAlso maximunValue.HasValue) Then
                            If (minimunValue <> -1 AndAlso maximunValue <> -1) Then
                                resultRow.NormalLowerLimit = minimunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                resultRow.NormalUpperLimit = maximunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                            End If

                            If (panicMinimunValue <> -1 AndAlso panicMaximunValue <> -1) Then
                                resultRow.PanicLowerLimit = panicMinimunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                resultRow.PanicUpperLimit = panicMaximunValue.Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                            End If
                        End If
                    End If
                Next resultRow

                'Get Average Result Alarms
                resultData = myResultsDelegate.GetResultAlarms(Nothing)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    'AG 26/02/2014 - BT #1521: Remove loops and use Merge
                    AverageResultsDS.vwksResultsAlarms.Merge(DirectCast(resultData.SetDatos, ResultsDS).vwksResultsAlarms)
                    AverageResultsDS.vwksResultsAlarms.AcceptChanges()
                Else
                    ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If

                'Get Execution Result Alarms
                resultData = myExecutionDelegate.GetWSExecutionResultAlarms(Nothing)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    'AG 26/02/2014 - BT #1521: Remove loops and use Merge
                    ExecutionsResultsDS.vwksWSExecutionsAlarms.Merge(DirectCast(resultData.SetDatos, ExecutionsDS).vwksWSExecutionsAlarms)
                    ExecutionsResultsDS.vwksWSExecutionsAlarms.AcceptChanges()
                Else
                    ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If

                'Set the Panic Limit Alarm if it exists
                GetPanicRangeValuesExecutions()
                GetPanicRangeValuesAverage()

                Dim hisSent As Integer = 0
                Dim rowIndex As Integer = 0
                Dim printedFalse As Integer = 0
                Dim relatedResults As Integer = 0
                Dim lstResultsByOrderID As List(Of ResultsDS.vwksResultsRow)

                IsOrderPrinted.Clear() 'RH 06/10/2010 Very important!
                IsOrderHISSent.Clear()

                For Each executionRow As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults
                    If (executionRow.SampleClass = "PATIENT") Then
                        If (IsOrderPrinted.ContainsKey(executionRow.OrderID)) Then 'Already computed
                            executionRow.PrintAvailable = Not IsOrderPrinted(executionRow.OrderID)
                            executionRow.HIS_Sent = IsOrderHISSent(executionRow.OrderID)
                        Else
                            hisSent = 0
                            printedFalse = 0
                            relatedResults = 0

                            lstResultsByOrderID = (From a As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults _
                                                  Where a.OrderID = executionRow.OrderID _
                                                 Select a).ToList()

                            For Each resultRow As ResultsDS.vwksResultsRow In lstResultsByOrderID
                                If (Not resultRow.Printed) Then printedFalse += 1

                                If (resultRow.AcceptedResultFlag) Then
                                    relatedResults += 1
                                    If (resultRow.ExportStatus = "SENT") Then hisSent += 1
                                End If

                                resultRow.PatientName = executionRow.PatientName
                                resultRow.PatientID = executionRow.PatientID
                            Next

                            'For Each resultRow As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                            '    If String.Compare(resultRow.OrderID, executionRow.OrderID, False) = 0 Then
                            '        If Not resultRow.Printed Then printedFalse += 1

                            '        'AG 18/11/2010
                            '        'relatedResults += 1
                            '        'If resultRow.ExportStatus = "SENT" Then HISSent += 1
                            '        If resultRow.AcceptedResultFlag = True Then
                            '            relatedResults += 1
                            '            If String.Compare(resultRow.ExportStatus, "SENT", False) = 0 Then hisSent += 1
                            '        End If
                            '        'END AG 18/11/2010

                            '        'Take advantage of this loop for setting Result Patient Name
                            '        resultRow.PatientName = executionRow.PatientName
                            '        resultRow.PatientID = executionRow.PatientID
                            '    End If
                            'Next resultRow

                            executionRow.PrintAvailable = (printedFalse > 0)
                            executionRow.HIS_Sent = (hisSent = relatedResults)

                            IsOrderPrinted(executionRow.OrderID) = Not executionRow.PrintAvailable
                            IsOrderHISSent(executionRow.OrderID) = executionRow.HIS_Sent
                        End If
                    End If

                    executionRow.RowIndex = rowIndex 'IMPORTANT!!! The index starts in zero
                    rowIndex += 1
                Next executionRow

                myTestRefRangesDS = Nothing
                lstResultsByOrderID = Nothing
            Else
                ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadExecutionsResults", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Debug.Print("IResults.LoadExecutionsResults: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 21/06/2012 - time estimation
    End Sub

    ''' <summary>
    ''' For Each Execution validate the alarm ID to add the panic reference values.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 06/06/2012
    ''' </remarks>
    Private Sub GetPanicRangeValuesExecutions()
        Try
            Dim ExecutionID As Integer = 0
            'Validate each execcution Alarm.
            Dim myExecutionResultsList As New List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
            For Each resultRow As ExecutionsDS.vwksWSExecutionsAlarmsRow In _
                                        ExecutionsResultsDS.vwksWSExecutionsAlarms.Rows
                ExecutionID = 0
                Select Case resultRow.AlarmID
                    Case "CONC_REMARK9", "CONC_REMARK10"
                        ExecutionID = resultRow.ExecutionID
                        Exit Select
                End Select

                If Not ExecutionID = 0 Then
                    'If Execution Found then search the orderTestID, RerunNumber, MultipointNumber on execution results 
                    myExecutionResultsList = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In _
                                                       ExecutionsResultsDS.vwksWSExecutionsResults _
                                              Where a.ExecutionID = ExecutionID Select a).ToList()
                    If myExecutionResultsList.Count > 0 Then
                        '
                        Dim myAverageResultList As New List(Of ResultsDS.vwksResultsRow)
                        myAverageResultList = (From a As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults _
                                               Where a.OrderTestID = myExecutionResultsList.First().OrderTestID _
                                               AndAlso a.RerunNumber = myExecutionResultsList.First().RerunNumber _
                                               AndAlso a.MultiPointNumber = myExecutionResultsList.First().MultiItemNumber _
                                               Select a).ToList
                        If myAverageResultList.Count > 0 Then
                            If Not myAverageResultList.First.IsPanicLowerLimitNull AndAlso _
                               Not myAverageResultList.First().IsPanicUpperLimitNull Then
                                resultRow.Description &= " [" & myAverageResultList.First().PanicLowerLimit & _
                                                         "-" & myAverageResultList.First().PanicUpperLimit & "] "
                            End If

                        End If
                    End If
                End If
            Next

            'TR 25/09/2013 #memory
            myExecutionResultsList = Nothing
            'TR 25/09/2013 #memory

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " GetPanicRangeValuesExecutions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' For Each Average validate the alarm ID to add the panic reference values.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 06/6/2012
    ''' </remarks>
    Private Sub GetPanicRangeValuesAverage()
        Try
            For Each resultRow As ResultsDS.vwksResultsAlarmsRow In AverageResultsDS.vwksResultsAlarms.Rows
                Select Case resultRow.AlarmID
                    Case "CONC_REMARK9", "CONC_REMARK10"

                        Dim myAverageResultList As New List(Of ResultsDS.vwksResultsRow)
                        myAverageResultList = (From a As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults _
                                               Where a.OrderTestID = resultRow.OrderTestID _
                                               AndAlso a.RerunNumber = resultRow.RerunNumber _
                                               AndAlso a.MultiPointNumber = resultRow.MultiPointNumber _
                                               Select a).ToList
                        If myAverageResultList.Count > 0 Then
                            If Not myAverageResultList.First.IsPanicLowerLimitNull AndAlso _
                               Not myAverageResultList.First().IsPanicUpperLimitNull Then
                                resultRow.Description &= " [" & myAverageResultList.First().PanicLowerLimit & _
                                                         "-" & myAverageResultList.First().PanicUpperLimit & "] "
                            End If
                        End If
                        Exit Select
                End Select
            Next resultRow

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " GetPanicRangeValuesAverage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get the Alarm letter depending on the alarm type
    ''' Get the AlarmID form the AverageResults.
    ''' </summary>
    ''' <param name="pOrderTestID">Order Test ID</param>
    ''' <param name="pRerunNumber">Rerun Number</param>
    ''' <param name="pMultipoinNumber">Multipoint Number</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 07/06/2012
    ''' </remarks>
    Private Function GetRangeAlarmsLetter(ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, ByVal pMultipoinNumber As Integer) As String
        Dim myResult As String = String.Empty
        Try
            'AverageResultsDS.vwksResultsAlarms
            Dim myAverageResultList As New List(Of ResultsDS.vwksResultsAlarmsRow)
            myAverageResultList = (From a As ResultsDS.vwksResultsAlarmsRow In AverageResultsDS.vwksResultsAlarms _
                                   Where a.OrderTestID = pOrderTestID _
                                   And a.RerunNumber = pRerunNumber _
                                   And a.MultiPointNumber = pMultipoinNumber _
                                   Select a).ToList()

            For Each myAlarm As ResultsDS.vwksResultsAlarmsRow In myAverageResultList
                If Not myAlarm.IsAlarmIDNull Then
                    Select Case myAlarm.AlarmID
                        'EF 03/06/2014 #1650  (usar constantes no texto fijo)
                        Case "CONC_REMARK7"
                            myResult = GlobalConstants.LOW   '"L"
                            Exit Select
                        Case "CONC_REMARK8"
                            myResult = GlobalConstants.HIGH   '"H"
                            Exit Select
                        Case "CONC_REMARK9"
                            myResult = GlobalConstants.PANIC_LOW  '"PL"
                            Exit Select
                        Case "CONC_REMARK10"
                            myResult = GlobalConstants.PANIC_HIGH  '"PH"
                            Exit Select
                        Case Else
                            myResult = ""
                            Exit Select
                            'EF 03/06/2014 #1650  END
                    End Select
                End If
            Next
            'TR 02/08/2012 set value to nothin to release memory.
            myAverageResultList = Nothing

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " GetRangeAlarmsLetter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Get the Alarm letter depending on the alarm type
    ''' Get the alarm ID from the ExecutionsResultsDS.vwksWSExecutionsAlarms.
    ''' </summary>
    ''' <param name="pExecutionID"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  TR 08/06/2012
    ''' </remarks>
    Private Function GetReplicatesRangeAlarmsLetter(ByVal pExecutionID As Integer) As String
        Dim myResult As String = String.Empty
        Try
            Dim myExecutionsAlarmsList As New List(Of ExecutionsDS.vwksWSExecutionsAlarmsRow)
            myExecutionsAlarmsList = (From row In ExecutionsResultsDS.vwksWSExecutionsAlarms _
                             Where row.ExecutionID = pExecutionID _
                             Select row).ToList()
            If Not myExecutionsAlarmsList.Count = 0 Then
                For Each ExecRow As ExecutionsDS.vwksWSExecutionsAlarmsRow In myExecutionsAlarmsList
                    If Not ExecRow.IsAlarmIDNull Then
                        Select Case ExecRow.AlarmID
                            'EF 03/06/2014 #1650  (usar constantes no texto fijo)
                            Case "CONC_REMARK7"
                                myResult = GlobalConstants.LOW   '"L"
                                Exit Select
                            Case "CONC_REMARK8"
                                myResult = GlobalConstants.HIGH   '"H"
                                Exit Select
                            Case "CONC_REMARK9"
                                myResult = GlobalConstants.PANIC_LOW  '"PL"
                                Exit Select
                            Case "CONC_REMARK10"
                                myResult = GlobalConstants.PANIC_HIGH  '"PH"
                                Exit Select
                            Case Else
                                myResult = ""
                                Exit Select
                                'EF 03/06/2014 #1650  END
                        End Select
                    End If
                Next
            End If
            'TR 02/08/2012 set value to nothin to release memory.
            myExecutionsAlarmsList = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " GetRangeAlarmsLetter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Initializes all the controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 12/07/2010
    ''' Modified by: PG 14/10/2010 - Get the current language
    ''' Modified by: RH 18/10/2010 - Remove the currentLanguage local variable/parameter. Initialize the LanguageID new class property.
    '''              SG 12/04/2013 - Added code to get value of Software Parameter LIS_NAME
    '''              SG 17/04/2013 - Added code to get value of User Setting LIS_WORKING_MODE_RERUNS
    '''              XB 26/02/2014 - BT #1529 ==> Change the way to open the screen using OpenMDIChildForm generic method to avoid OutOfMem errors when back to Monitor
    '''              SA 19/09/2014 - BA-1927  ==> Removed code to verify if button SendManRepButton has to be enabled or disabled due to that checking is done 
    '''                                           previously in function PrepareButtons
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            LoadExecutionsResults()

            InitializeSamplesListGrid()
            InitializeTestsListGrid()

            InitializeUserSelectionStep1()   'XB 26/02/2014 - task #1529

            GetScreenLabels()
            PrepareButtons()

            'Get value of Software Parameter LIS Name for headers
            Dim myGlobal As New GlobalDataTO
            Dim myParams As New SwParametersDelegate

            myGlobal = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.LIS_NAME.ToString, Nothing)
            If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                Dim myParametersDS As ParametersDS = DirectCast(myGlobal.SetDatos, ParametersDS)

                If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then MyClass.LISNameForColumnHeaders = myParametersDS.tfmwSwParameters.Item(0).ValueText
            End If

            'Get value of User Setting LIS_WORKING_MODE_RERUNS
            Dim myRerunLISMode As String = "LIS"
            Dim myUsersSettingsDelegate As New UserSettingsDelegate

            myGlobal = myUsersSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WORKING_MODE_RERUNS.ToString)
            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                MyClass.LISWorkingModeRerunsAttr = TryCast(myGlobal.SetDatos, String)
            End If

            'Experimentals Grid
            InitializeExperimentalsGrid()
            DefineExperimentalsSortedColumns()

            'Blanks Grid
            InitializeBlanksGrid()
            DefineBlanksSortedColumns()

            'Blanks Grid
            InitializeCalibratorsGrid()
            DefineCalibratorsSortedColumns()

            'Controls Grid
            InitializeControlsGrid()
            DefineControlsSortedColumns()

            'Samples XtraGrid
            InitializeSamplesXtraGrid()
            DefineXtraSamplesSortedColumns()

            UpdateTestsListDataGrid()
            UpdateSamplesListDataGrid()

            InitializeUserSelectionStep2()   'XB 26/02/2014 - task #1529

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Preparations to shows SampleClass and SampleOrTestName info
    ''' </summary>
    ''' <remarks>
    ''' Created by: XB 26/02/2014 - Change the way to open the screen using OpenMDIChildForm generic method to avoid OutOfMem errors when back to Monitor - task #1529
    ''' </remarks>
    Private Sub InitializeUserSelectionStep1()
        Try
            If Not SampleClass Is Nothing Then
                Select Case SampleClass
                    Case "PATIENT"
                        SamplesListViewText = SampleOrTestName

                    Case "BLANK"
                        TestsListViewText = SampleOrTestName

                    Case "CALIB"
                        TestsListViewText = SampleOrTestName

                    Case "CTRL"
                        TestsListViewText = SampleOrTestName
                End Select
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " InitializeUserSelectionStep1 ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Preparations to shows SampleClass and SampleOrTestName info
    ''' </summary>
    ''' <remarks>
    ''' Created by: XB 26/02/2014 - Change the way to open the screen using OpenMDIChildForm generic method to avoid OutOfMem errors when back to Monitor - task #1529
    ''' </remarks>
    Private Sub InitializeUserSelectionStep2()
        Try
            If (Not SampleClass Is Nothing) Then
                Select Case SampleClass
                    Case "PATIENT"
                        bsTestDetailsTabControl.SelectedTab = bsSamplesTab

                        bsSamplesResultsTabControl.Visible = True
                        bsSamplesPanel.Visible = True
                        ExportButton.Visible = True

                        bsTestPanel.Visible = False
                        bsResultsTabControl.Visible = False

                    Case "BLANK"
                        bsTestDetailsTabControl.SelectedTab = bsTestsTabTage

                        bsTestPanel.Visible = True
                        bsResultsTabControl.Visible = True
                        bsResultsTabControl.SelectedTab = bsBlanksTabPage
                        ExportButton.Visible = False

                        bsSamplesResultsTabControl.Visible = False
                        bsSamplesPanel.Visible = False

                    Case "CALIB"
                        bsTestDetailsTabControl.SelectedTab = bsTestsTabTage

                        bsTestPanel.Visible = True
                        bsResultsTabControl.Visible = True
                        bsResultsTabControl.SelectedTab = bsCalibratorsTabPage
                        ExportButton.Visible = False

                        bsSamplesResultsTabControl.Visible = False
                        bsSamplesPanel.Visible = False

                    Case "CTRL"
                        bsTestDetailsTabControl.SelectedTab = bsTestsTabTage

                        bsTestPanel.Visible = True
                        bsResultsTabControl.Visible = True

                        bsResultsTabControl.SelectedTab = bsControlsTabPage
                        ExportButton.Visible = True

                        bsSamplesResultsTabControl.Visible = False
                        bsSamplesPanel.Visible = False
                End Select
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " InitializeUserSelectionStep2 ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Returns Alarm Description (Remmark) associated to a Result
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 19/07/2010
    ''' Modified by AG 28/07/2010
    ''' Modified by RH 05/10/2010
    ''' </remarks>
    Private Function GetResultAlarmDescription(ByVal OrderTestID As Integer, ByVal RerunNumber As Integer, ByVal MultiPointNumber As Integer) As String
        Try
            Dim Descriptions As List(Of String) = _
                    (From row In AverageResultsDS.vwksResultsAlarms _
                             Where row.OrderTestID = OrderTestID _
                             AndAlso row.RerunNumber = RerunNumber _
                             AndAlso row.MultiPointNumber = MultiPointNumber _
                             Select row.Description Distinct).ToList()  'AG 28/07/2010 - Add Distinct

            If Descriptions.Count > 0 Then
                'AG 28/07/2010
                'Return Descriptions(0)
                'Dim myTotalAlarmsDescription As String = ""
                'For i As Integer = 0 To Descriptions.Count - 1
                '    myTotalAlarmsDescription += Descriptions(i)
                '    If i < Descriptions.Count - 1 Then myTotalAlarmsDescription += ", "
                'Next
                'Return myTotalAlarmsDescription
                'END AG 28/07/2010

                'RH 05/10/2010
                'Just a little enhancement, because in .NET, Strings are inmutables.
                'It is better to do Strings concatenations inside a loop using StringBuilders
                'They were designed for that task           
                Dim myTotalAlarmsDescription As New StringBuilder()
                For Each Description As String In Descriptions
                    myTotalAlarmsDescription.AppendFormat("{0}, ", Description)
                Next

                Return myTotalAlarmsDescription.Remove(myTotalAlarmsDescription.Length - 2, 2).ToString()
                'END RH 05/10/2010
            Else
                Return String.Empty
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetResultAlarmDescription ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
            Return Nothing

        End Try
    End Function

    ''' <summary>
    ''' Returns Alarm Description (Remmark) associated to an Execution
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 19/07/2010
    ''' Modified by AG 28/07/2010
    ''' Modified by RH 05/10/2010
    ''' </remarks>
    Private Function GetExecutionAlarmDescription(ByVal ExecutionID As Integer) As String
        Try
            Dim Descriptions As List(Of String) = _
                    (From row In ExecutionsResultsDS.vwksWSExecutionsAlarms _
                             Where row.ExecutionID = ExecutionID _
                             Select row.Description Distinct).ToList() 'AG 28/07/2010 - Add distinct

            If Descriptions.Count > 0 Then
                'AG 28/07/2010
                'Return Descriptions(0)
                'Dim myTotalAlarmsDescription As String = ""
                'For i As Integer = 0 To Descriptions.Count - 1
                '    myTotalAlarmsDescription += Descriptions(i)
                '    If i < Descriptions.Count - 1 Then myTotalAlarmsDescription += ", "
                'Next
                'Return myTotalAlarmsDescription
                'END AG 28/07/2010

                'RH 05/10/2010
                'Just a little enhancement, because in .NET, Strings are inmutables.
                'It is better to do Strings concatenations inside a loop using StringBuilders
                'They were designed for that task           
                Dim myTotalAlarmsDescription As New StringBuilder()
                For Each Description As String In Descriptions
                    myTotalAlarmsDescription.AppendFormat("{0}, ", Description)
                Next

                Return myTotalAlarmsDescription.Remove(myTotalAlarmsDescription.Length - 2, 2).ToString()
                'END RH 05/10/2010
            Else
                Return String.Empty
            End If

            'TR 02/08/2012 
            Descriptions = Nothing

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetExecutionAlarmDescription ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            Return Nothing

        End Try
    End Function

    ''' <summary>
    ''' Refresh grid.
    ''' </summary>
    ''' <remarks>
    ''' Created by AG 26/07/2010
    ''' Modified by: RH 08/10/2010
    ''' </remarks>
    Private Sub UpdateCurrentResultsGrid()
        Try
            If isClosingFlag Then Exit Sub ' XB 24/02/2014 - #1496 No refresh if screen is closing

            Select Case bsResultsTabControl.SelectedTab.Name
                Case Me.bsBlanksTabPage.Name
                    UpdateBlanksDataGrid()
                Case Me.bsCalibratorsTabPage.Name
                    UpdateCalibratorsDataGrid()
                Case Me.bsControlsTabPage.Name
                    UpdateControlsDataGrid()
                Case Me.XtraSamplesTabPage.Name
                    UpdateSamplesXtraGrid()
            End Select

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateCurrentResultsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Actually repaints the Result Grid.
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 08/09/2010
    ''' </remarks>
    Private Sub RepaintCurrentResultsGrid()
        Try
            Dim StartTime As DateTime = Now 'AG 21/06/2012 - time estimation
            Select Case bsResultsTabControl.SelectedTab.Name
                Case Me.bsBlanksTabPage.Name
                    'AG 21/06/2012
                    'BlankTestName = String.Empty
                    If copyRefreshDS Is Nothing Then BlankTestName = String.Empty
                    UpdateBlanksDataGrid()
                    Debug.Print("IResults.RepaintCurrentResultsGrid (UpdateBlanksDataGrid): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 21/06/2012 - time estimation

                Case Me.bsCalibratorsTabPage.Name
                    'AG 21/06/2012
                    'CalibratorTestName = String.Empty
                    If copyRefreshDS Is Nothing Then CalibratorTestName = String.Empty
                    UpdateCalibratorsDataGrid()
                    Debug.Print("IResults.RepaintCurrentResultsGrid (UpdateCalibratorsDataGrid): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 21/06/2012 - time estimation

                Case Me.bsControlsTabPage.Name
                    'ControlTestName = String.Empty
                    'AG 21/06/2012
                    'ControlTestName = String.Empty
                    If copyRefreshDS Is Nothing Then ControlTestName = String.Empty
                    UpdateControlsDataGrid()
                    Debug.Print("IResults.RepaintCurrentResultsGrid (UpdateControlsDataGrid): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 21/06/2012 - time estimation

                Case Me.XtraSamplesTabPage.Name
                    'AG 21/06/2012
                    'SampleTestName = String.Empty
                    If copyRefreshDS Is Nothing Then SampleTestName = String.Empty
                    UpdateSamplesXtraGrid()
                    Debug.Print("IResults.RepaintCurrentResultsGrid (UpdateSamplesXtraGrid): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 21/06/2012 - time estimation

            End Select

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log & show error message
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " RepaintCurrentResultsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))") 'AG 07/07/2010 GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to load the buttons image.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/04/2010
    ''' Modified by: DL 21/06/2010
    '''              RH 23/08/2010 - Modified the way of loading the icons
    '''              SA 20/01/2011 - Load Icon for new button for opening screen of results for Off-System Order Tests;
    '''                              control the availability of this new button when the screen is loaded
    '''              SA 19/09/2014 - BA-1927 ==> Call new function SendManRepButtonEnabled to get the availability of button for Send Manual Reruns
    '''              WE 13/01/2015 - BA-2153: •	Changed the initialization order of the ImageList 'TestTypeIconList'. Note that this sequence must always
    '''                                         be in-sync with array 'TestType' in Sub UpdateTestsListDataGrid (Class IResultsTestsListDataGridView).
    ''' </remarks>
    Private Sub PrepareButtons()
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Dim StartTime As DateTime = Now
        'Dim myLogAcciones As New ApplicationLogManager()
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Try
            Dim preloadedDataConfig As New PreloadedMasterDataDelegate

            Dim auxIconName As String = String.Empty
            Dim iconPath As String = MyBase.IconsPath

            'dl 12/05/2011
            'Excel Export Button
            'auxIconName = GetIconName("BTN_EXCELEXPORT")
            'If Not String.Equals(auxIconName, "") Then bsXlsresults.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'TEST()
            'PRINT REPORT Button
            auxIconName = GetIconName("FINALPRINT")
            If Not String.Equals(auxIconName, String.Empty) Then PrintReportButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'cf 26/09/2013
            'Print compact report button
            auxIconName = GetIconName("COMPACTPRINT")
            If Not String.Equals(auxIconName, String.Empty) Then PrintCompactReportButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            auxIconName = GetIconName("COMPACTPRINTCTR")
            If Not String.Equals(auxIconName, String.Empty) Then PrintTestCtrlButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'SUMMARY Button
            auxIconName = GetIconName("GRID")
            If Not String.Equals(auxIconName, String.Empty) Then SummaryButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'PRINT Buttons: PrintSampleButton, PrintTestButton
            auxIconName = GetIconName("PRINT")
            If Not String.Equals(auxIconName, String.Empty) Then
                PrintSampleButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                '                bsPrintTestButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                PrintTestButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'SEND MANUAL REPETITION Button
            auxIconName = GetIconName("MANUAL_REP")
            If Not String.Equals(auxIconName, String.Empty) Then SendManRepButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'OFF SYSTEM TESTS RESULTS Button
            auxIconName = GetIconName("OFFSYSTEMBUT")
            If Not String.Equals(auxIconName, String.Empty) Then OffSystemResultsButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            ' Export Button
            auxIconName = GetIconName("MANUAL_EXP")
            If Not String.Equals(auxIconName, String.Empty) Then ExportButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            auxIconName = GetIconName("CANCEL")
            If Not String.Equals(auxIconName, String.Empty) Then ExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

            'Icons to be used in Grids
            auxIconName = GetIconName("PRINTL")
            If Not String.Equals(auxIconName, String.Empty) Then PrintImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            auxIconName = GetIconName("LOGIN")
            If Not String.Equals(auxIconName, String.Empty) Then RepImage = preloadedDataConfig.GetIconImage("LOGIN")

            auxIconName = GetIconName("INC_NEW_REP")
            If (auxIconName <> String.Empty) Then
                INC_NEW_REPImage = preloadedDataConfig.GetIconImage("INC_NEW_REP")
            End If

            auxIconName = GetIconName("RED_NEW_REP")
            If (auxIconName <> String.Empty) Then
                RED_NEW_REPImage = preloadedDataConfig.GetIconImage("RED_NEW_REP")
            End If

            '
            bsXlsresults.Image = ImageUtilities.ImageFromFile(iconPath & "BTN_EXCELEXPORT.png")
            PrintReportButton.Image = ImageUtilities.ImageFromFile(iconPath & "FinalPrint.png")
            SummaryButton.Image = ImageUtilities.ImageFromFile(iconPath & "Grid_16.png")
            '
            PrintSampleButton.Image = ImageUtilities.ImageFromFile(iconPath & "Print.png")
            '            bsPrintTestButton.Image = ImageUtilities.ImageFromFile(iconPath & "Print.png")
            PrintTestButton.Image = ImageUtilities.ImageFromFile(iconPath & "Print.png")
            '
            SendManRepButton.Image = ImageUtilities.ImageFromFile(iconPath & "Manual_Repeat.png")
            OffSystemResultsButton.Image = ImageUtilities.ImageFromFile(iconPath & "OFFSTestButton.png")

            ExportButton.Image = ImageUtilities.ImageFromFile(iconPath & "Export.png")
            ExitButton.Image = ImageUtilities.ImageFromFile(iconPath & "Cancel.png")

            PrintImage = ImageUtilities.ImageFromFile(iconPath & "PrintL.png")

            'RepImage = preloadedDataConfig.GetIconImage("LOGIN")
            auxIconName = GetIconName("LOGIN")
            If Not String.Equals(auxIconName, String.Empty) Then RepImage = preloadedDataConfig.GetIconImage("LOGIN")

            auxIconName = GetIconName("EQ_NEW_REP")
            If (auxIconName <> String.Empty) Then
                EQ_NEW_REPImage = preloadedDataConfig.GetIconImage("EQ_NEW_REP")
            End If

            auxIconName = GetIconName("NO_NEW_REP")
            If (auxIconName <> String.Empty) Then
                NO_NEW_REPImage = preloadedDataConfig.GetIconImage("NO_NEW_REP")
            End If

            auxIconName = GetIconName("REP_INC")
            If (auxIconName <> String.Empty) Then
                REP_INCImage = preloadedDataConfig.GetIconImage("REP_INC")
            End If

            auxIconName = GetIconName("EQUAL_REP")
            If (auxIconName <> String.Empty) Then
                EQUAL_REPImage = preloadedDataConfig.GetIconImage("EQUAL_REP")
            End If

            auxIconName = GetIconName("RED_REP")
            If (auxIconName <> String.Empty) Then
                RED_REPImage = preloadedDataConfig.GetIconImage("RED_REP")
            End If

            auxIconName = GetIconName("CHECKL")
            If (auxIconName <> String.Empty) Then
                OKImage = preloadedDataConfig.GetIconImage("CHECKL")
            End If

            'DL 23/09/2011
            auxIconName = GetIconName("UNCHECKL")
            If (auxIconName <> String.Empty) Then
                UnCheckImage = preloadedDataConfig.GetIconImage("UNCHECKL")
            End If
            'END DL 23/09/2011

            auxIconName = GetIconName("STATS")
            If (auxIconName <> String.Empty) Then
                KOImage = preloadedDataConfig.GetIconImage("STATS")
            End If

            auxIconName = GetIconName("FREECELL")
            If (auxIconName <> String.Empty) Then
                NoImage = preloadedDataConfig.GetIconImage("FREECELL")
            End If

            auxIconName = GetIconName("BLANK")
            If (auxIconName <> String.Empty) Then
                ClassImage = preloadedDataConfig.GetIconImage("BLANK")
            End If

            auxIconName = GetIconName("ABS_GRAPH")
            If auxIconName <> String.Empty Then
                ABS_GRAPHImage = preloadedDataConfig.GetIconImage("ABS_GRAPH")
            End If

            auxIconName = GetIconName("AVG_ABS_GRAPH")
            If (auxIconName <> String.Empty) Then
                AVG_ABS_GRAPHImage = preloadedDataConfig.GetIconImage("AVG_ABS_GRAPH")
                CURVE_GRAPHImage = preloadedDataConfig.GetIconImage("AVG_ABS_GRAPH")
            End If

            'OPEN CURVE GRAPH Button
            'auxIconName = GetIconName("CURVEGRAPH") ' CURVE
            'If (auxIconName <> String.Empty) Then
            'CURVE_GRAPHImage = preloadedDataConfig.GetIconImage("CURVEGRAPH")
            'End If

            'IMAGES FOR Samples TreeView
            SampleIconList = New ImageList()

            auxIconName = GetIconName("STATS")
            If (auxIconName <> String.Empty) Then
                AddIconToImageList(SampleIconList, auxIconName)
                PATIENT_STATImage = preloadedDataConfig.GetIconImage("STATS")
            End If

            auxIconName = GetIconName("ROUTINES")
            If (auxIconName <> String.Empty) Then
                AddIconToImageList(SampleIconList, auxIconName)
                PATIENT_ROUTINEImage = preloadedDataConfig.GetIconImage("ROUTINES")
            End If

            'IMAGES FOR Tests TreeView
            TestTypeIconList = New ImageList()

            ' WE 13/01/2015 (BA-2153) - Sync order of Test Types with array 'TestType' in Sub UpdateTestsListDataGrid (Class IResultsTestsListDataGridView).
            auxIconName = GetIconName("TESTICON") 'STD Tests Icon
            If (auxIconName <> String.Empty) Then
                AddIconToImageList(TestTypeIconList, auxIconName)
            End If

            'auxIconName = GetIconName("TCALC") 'CALC Tests Icon
            'If (auxIconName <> String.Empty) Then
            '    TestTypeIconList.Images.Add(ImageUtilities.ImageFromFile(iconPath & auxIconName))
            'End If

            auxIconName = GetIconName("TISE_SYS") 'ISE Tests Icon
            If (String.Compare(auxIconName, String.Empty, False) <> 0) Then
                TestTypeIconList.Images.Add(ImageUtilities.ImageFromFile(iconPath & auxIconName))
            End If

            auxIconName = GetIconName("TOFF_SYS") 'OFFS Tests Icon
            If (auxIconName <> String.Empty) Then
                TestTypeIconList.Images.Add(ImageUtilities.ImageFromFile(iconPath & auxIconName))
            End If

            auxIconName = GetIconName("TCALC") 'CALC Tests Icon
            If (auxIconName <> String.Empty) Then
                TestTypeIconList.Images.Add(ImageUtilities.ImageFromFile(iconPath & auxIconName))
            End If
            ' WE 13/01/2015 (BA-2153) - End.

            auxIconName = GetIconName("INC_SENT_REP")
            If (auxIconName <> String.Empty) Then
                INC_SENT_REPImage = preloadedDataConfig.GetIconImage("INC_SENT_REP")
            End If

            auxIconName = GetIconName("RED_SENT_REP")
            If (auxIconName <> String.Empty) Then
                RED_SENT_REPImage = preloadedDataConfig.GetIconImage("RED_SENT_REP")
            End If

            auxIconName = GetIconName("EQ_SENT_REP")
            If (auxIconName <> String.Empty) Then
                EQ_SENT_REPImage = preloadedDataConfig.GetIconImage("EQ_SENT_REP")
            End If

            '' dl 23/03/2011
            'auxIconName = GetIconName("ROUTINES")
            'If (auxIconName <> "") Then
            '    PATIENT = preloadedDataConfig.GetIconImage("ROUTINES")
            'End If

            'Control availability and visibility of special buttons
            SendManRepButton.Enabled = SendManRepButtonEnabled()
            OffSystemResultsButton.Enabled = OffSystemResultsButtonEnabled()

            auxIconName = GetIconName("PRINTHEAD")
            If (String.Compare(auxIconName, String.Empty, False) <> 0) Then
                'PrintPictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                PrintHeadImage = ImageUtilities.ImageFromFile(iconPath & auxIconName) 'RH 04/06/2012
            End If

            auxIconName = GetIconName("EXPORTHEAD")
            If (auxIconName <> String.Empty) Then
                'HISSentImage = preloadedDataConfig.GetIconImage("MANUAL_EXP")
                'LISHeadImage = preloadedDataConfig.GetIconImage("EXPORTHEAD")
                'HISPictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)

                'RH 19/10/2011
                LISExperimentalHeadImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                LISControlHeadImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                LISSamplesHeadImage = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                LISSubHeaderImage = preloadedDataConfig.GetIconImage("EXPORTHEAD")
                'END RH 19/10/2011

                LISHeadImage = ImageUtilities.ImageFromFile(iconPath & auxIconName) 'RH 04/06/2012
            End If

            'IT 21/10/2014: INI BA-2036
            auxIconName = GetIconName("EXPORTHEADCHECK")
            If (auxIconName <> String.Empty) Then
                LISHeadCheckImage = ImageUtilities.ImageFromFile(iconPath & GetIconName("EXPORTHEADCHECK"))
            End If
            'IT 21/10/2014: END BA-2036


            Const PicLeft As Integer = 7
            Const PicTop As Integer = 2

            'PrintPictureBox
            PrintPictureBox.BackColor = Color.Transparent
            PrintPictureBox.Name = "PrintPictureBox"
            PrintPictureBox.SizeMode = PictureBoxSizeMode.AutoSize
            PrintPictureBox.TabStop = False

            Dim X As Integer = 0

            'TR 30/09/2013 -Execute the same as order print. I thisk this part require more analisys
            '               cause here is where fails.
            For i As Integer = 0 To bsSamplesListDataGridView.Columns("OrderToPrint1").Index - 1
                If bsSamplesListDataGridView.Columns(i).Visible Then
                    X += bsSamplesListDataGridView.Columns(i).Width
                End If
            Next

            For i As Integer = 0 To bsSamplesListDataGridView.Columns("OrderToPrint").Index - 1
                If bsSamplesListDataGridView.Columns(i).Visible Then
                    X += bsSamplesListDataGridView.Columns(i).Width
                End If
            Next

            PrintPictureBox.Left = X + PicLeft
            PrintPictureBox.Top = PicTop
            bsSamplesListDataGridView.Controls.Add(PrintPictureBox)

            ''HISPictureBox
            'HISPictureBox.BackColor = Color.Transparent
            'HISPictureBox.Name = "HISPictureBox"
            'HISPictureBox.SizeMode = PictureBoxSizeMode.AutoSize
            'HISPictureBox.TabStop = False

            'X = 0
            'For i As Integer = 0 To bsSamplesListDataGridView.Columns("OrderToExport").Index - 1
            '    If bsSamplesListDataGridView.Columns(i).Visible Then
            '        X += bsSamplesListDataGridView.Columns(i).Width
            '    End If
            'Next

            'HISPictureBox.Left = X + PicLeft
            'HISPictureBox.Top = PicTop
            'bsSamplesListDataGridView.Controls.Add(HISPictureBox)

            'RH 24/10/2011
            XtraGridIconList = New ImageList()

            auxIconName = GetIconName("EXPORTHEAD")
            If (auxIconName <> String.Empty) Then
                AddIconToImageList(XtraGridIconList, auxIconName)
            End If

            auxIconName = GetIconName("FREECELL")
            If (auxIconName <> String.Empty) Then
                AddIconToImageList(XtraGridIconList, auxIconName)
            End If

            CollapseIconList = New ImageList()
            CollapseIconList.ImageSize = New Size(9, 9)

            auxIconName = GetIconName("EXPAND")
            'auxIconName = "expand.png"
            If (auxIconName <> String.Empty) Then
                AddIconToImageList(CollapseIconList, auxIconName)
            End If

            auxIconName = GetIconName("COLLAPSE")
            'auxIconName = "collapse.png"
            If (auxIconName <> String.Empty) Then
                AddIconToImageList(CollapseIconList, auxIconName)
            End If

            auxIconName = GetIconName("XTRAVERTICALBAR")
            'auxIconName = "XtraVerticalBar.png"
            If (auxIconName <> String.Empty) Then
                XtraVerticalBar = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        GlobalBase.CreateLogActivity("IResults PrepareButtons (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0) & _
                                        " OPEN TAB: " & bsTestDetailsTabControl.SelectedTab.Name, _
                                        "IResults.PrepareButtons", EventLogEntryType.Information, False)
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

    End Sub

    ''' <summary>
    ''' Verify if button for open screen to add results for requested Off-System Tests can be enabled
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 20/01/2011
    ''' </remarks>
    Private Function OffSystemResultsButtonEnabled() As Boolean
        Dim numOffSystems As Integer = 0
        Try
            Dim resultData As GlobalDataTO
            Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate

            resultData = myWSOrderTestsDelegate.CountWSOffSystemTests(Nothing, WorkSessionIDField)
            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                numOffSystems = DirectCast(resultData.SetDatos, Integer)
            Else
                'Error getting the number of requested OFF-SYSTEM OrderTests in the WorkSession
                ShowMessage(Name & ".OffSystemResultsButtonEnabled", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OffSystemResultsButtonEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OffSystemResultsButtonEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return (numOffSystems > 0)
    End Function

    ''' <summary>
    ''' Get the list of requested Off-System Tests and open the auxiliary screen that allow enter results
    ''' for them. When the auxiliary screen is closed, the results are updated in ??? 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 20/01/2011
    ''' Modified by: AG 30/09/2014 - BA-1440 ==> Inform that is an automatic exportation when call method InvokeUploadResultsLIS
    '''              XB 28/11/2014 - BA-1867 ==> Recalculates calculated tests also for OFFS tests
    '''              SA 14/01/2015 - BA-2153 ==> Undo changes made for BA-1867. Recalculation of affected Calculated Tests is moved to function 
    '''                                          SaveOffSystemResults to execute it also when results of OffSystem Tests are saved from WS Samples  
    '''                                          Requests Screen  
    ''' </remarks>
    Private Sub OpenOffSystemResultsScreen()
        Try
            Dim resultData As GlobalDataTO
            Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate

            resultData = myWSOrderTestsDelegate.GetWSOffSystemTests(Nothing, WorkSessionIDField)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myOffSystemTestsResultsDS As OffSystemTestsResultsDS
                myOffSystemTestsResultsDS = DirectCast(resultData.SetDatos, OffSystemTestsResultsDS)

                'Open the auxiliary screen for results of OffSystem OrderTests
                Using myAuxOFFSResultsScreen As New IResultsOffSystemsTest
                    myAuxOFFSResultsScreen.OffSystemTestsList = myOffSystemTestsResultsDS
                    myAuxOFFSResultsScreen.ShowDialog()
                    If (myAuxOFFSResultsScreen.DialogResult = DialogResult.OK) Then
                        'Recovery the list of updated results
                        myOffSystemTestsResultsDS = myAuxOFFSResultsScreen.OffSystemTestsList

                        'Add/update/delete the results of OffSystem OrderTests
                        Dim myResultsDelegate As New ResultsDelegate(MyBase.AnalyzerModel)
                        resultData = myResultsDelegate.SaveOffSystemResults(Nothing, myOffSystemTestsResultsDS, AnalyzerIDField, WorkSessionIDField)

                        ' ''XB 28/11/2014 - BA-1867
                        'Dim myCalcTestsDelegate As New OperateCalculatedTestDelegate
                        '' recalculate calculated tests
                        'For Each offSystemTestResult As OffSystemTestsResultsDS.OffSystemTestsResultsRow In myOffSystemTestsResultsDS.OffSystemTestsResults
                        '    myCalcTestsDelegate.AnalyzerID = AnalyzerIDField
                        '    myCalcTestsDelegate.WorkSessionID = WorkSessionIDField
                        '    resultData = myCalcTestsDelegate.ExecuteCalculatedTest(Nothing, offSystemTestResult.OrderTestID, True)
                        'Next
                        ''XB 28/11/2014 - BA-1867


                        'TR 28/06/2013 - Prepare and send results to LIS.
                        If (Not resultData.HasError) Then
                            If myResultsDelegate.LastExportedResults.twksWSExecutions.Rows.Count > 0 Then 'AG 21/02/2014 - #1505 call mdi threat only when needed
                                CreateLogActivity("Current Results automatic upload (OFFS)", Me.Name & ".OpenOffSystemResultsScreen ", EventLogEntryType.Information, False) 'AG 02/01/2014 - BT #1433 (v211 patch2)
                                UiAx00MainMDI.AddResultsIntoQueueToUpload(myResultsDelegate.LastExportedResults)
                                UiAx00MainMDI.InvokeUploadResultsLIS(False, True) 'AG 30/09/2014 - BA-1440 inform that is an automatic exportation

                                'Clear the executions
                                myResultsDelegate.ClearLastExportedResults()
                            End If 'AG 21/02/2014 - #1505
                        End If
                        'TR 28/06/2013 -END.


                        '//JVV 02/10/13
                        If (Not resultData.HasError) Then
                            Dim printGlobalTo As New GlobalDataTO
                            Dim myautoreportdelg As New AutoReportsDelegate
                            Dim sType As String = String.Empty
                            printGlobalTo = myautoreportdelg.ManageAutoReportCreationOFFSys(Nothing, AnalyzerIDField, WorkSessionIDField, sType)
                            If (Not printGlobalTo.HasError AndAlso Not printGlobalTo.SetDatos Is Nothing) Then
                                Dim oList As New List(Of String)
                                oList = DirectCast(printGlobalTo.SetDatos, List(Of String))
                                If oList.Count > 0 Then
                                    If (String.Equals(sType, "COMPACT")) Then
                                        XRManager.PrintCompactPatientsReport(AnalyzerIDField, WorkSessionIDField, oList, True)
                                    ElseIf (String.Equals(sType, "INDIVIDUAL")) Then
                                        XRManager.PrintPatientsFinalReport(AnalyzerIDField, WorkSessionIDField, oList)
                                    End If
                                End If
                            End If
                        End If
                        '//JVV 02/10/13

                        If (Not resultData.HasError) Then
                            'RH 27/01/2011
                            bsTestsListDataGridView.Rows.Clear()
                            bsSamplesListDataGridView.Rows.Clear()

                            'SamplesListViewIndex = -1
                            RefreshScreen(Nothing, Nothing)
                        End If
                    End If
                End Using
            End If

            If (resultData.HasError) Then
                'Error getting the list of requested OffSystem OrderTests or saving the results; shown it
                ShowMessage(Me.Name & ".OpenOffSystemResultsScreen", resultData.ErrorCode, resultData.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OpenOffSystemResultsScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenOffSystemResultsScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Open the manual repetition screen
    ''' </summary>
    ''' <param name="pOrderTestID" ></param>
    ''' <param name="pTestType" ></param>
    ''' <param name="pSampleClass" ></param>
    ''' <returns>String indicating the selected repetition criteria</returns>
    ''' <remarks>
    ''' Created by:  AG 28/07/2010
    ''' Modified by: AG 02/12/2010 - Add pTestType parameter
    ''' Modified by: AG 11/03/2011 - Add pSampleClass parameter
    ''' </remarks>
    Private Function ShowManualRepetitions(ByVal pOrderTestID As Integer, ByVal pTestType As String, ByVal pSampleClass As String) As GlobalEnumerates.PostDilutionTypes
        Dim myReturnValue As GlobalEnumerates.PostDilutionTypes = PostDilutionTypes.WITHOUT
        Try
            'RH 19/10/2010 Introduce the Using statement
            Using myScreen As New IWSManualRepetition
                myScreen.ActiveAnalyzer = AnalyzerIDField
                myScreen.ActiveWorkSession = WorkSessionIDField
                myScreen.ActiveOrderTestID = pOrderTestID
                myScreen.TestType = pTestType
                myScreen.SampleClass = pSampleClass
                myScreen.ShowDialog()
                myReturnValue = myScreen.SelectedManualCriteria
            End Using

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ShowManualRepetitions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
        Return myReturnValue
    End Function

    ''' <summary>
    ''' Update collapse in database
    ''' - pGeneral (True): All results shows in current grid
    ''' - pGeneral (False): Only the OrderTestID - RerunNumber - MultiItemNumber informed in pResultRow
    ''' </summary>
    ''' <param name="pGeneral"></param>
    ''' <param name="pNewValue"></param>
    ''' <param name="pGridName"></param>
    ''' <param name="pResultRow"></param>
    ''' <returns >Boolean indicating if an error has been occurred</returns>
    ''' <remarks>Created by AG 03/08/2010</remarks>
    Private Function UpdateCollapse(ByVal pGeneral As Boolean, ByVal pNewValue As Boolean, _
                                    Optional ByVal pGridName As String = Nothing, _
                               Optional ByVal pResultRow As ResultsDS.vwksResultsRow = Nothing) As Boolean

        Dim finalResultOK As Boolean = True
        Try
            Dim myResultsDelegate As New ResultsDelegate
            Dim myResultsDS As New ResultsDS
            Dim OrderTestIdList As List(Of Integer)

            If pGeneral Then
                'Update collapse value for all results in current grid

                '1st get all ordertestid shown in current grid
                Select Case pGridName
                    Case bsExperimentalsDataGridView.Name
                        OrderTestIdList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                            Where String.Compare(row.PatientID, SamplesListViewText, False) = 0 AndAlso String.Compare(row.SampleClass, "PATIENT", False) = 0 _
                                            Select row.OrderTestID Distinct).ToList()

                    Case bsBlanksDataGridView.Name
                        OrderTestIdList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                            Where row.TestName = TestsListViewText AndAlso row.SampleClass = "BLANK" _
                                            Select row.OrderTestID Distinct).ToList()

                    Case bsCalibratorsDataGridView.Name
                        OrderTestIdList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                            Where String.Compare(row.TestName, TestsListViewText, False) = 0 AndAlso String.Compare(row.SampleClass, "CALIB", False) = 0 _
                                            Select row.OrderTestID Distinct).ToList()

                    Case bsControlsDataGridView.Name
                        OrderTestIdList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                            Where row.TestName = TestsListViewText AndAlso row.SampleClass = "CTRL" _
                                            Select row.OrderTestID Distinct).ToList()

                    Case Else
                        Return False
                End Select

                '2on create dataset
                Dim newResultsRow As ResultsDS.twksResultsRow
                For Each myOrderTest As Integer In OrderTestIdList
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
            End If

            Dim myGlobal As GlobalDataTO
            myGlobal = myResultsDelegate.UpdateCollapse(Nothing, myResultsDS)

            If myGlobal.HasError Then
                ShowMessage("Error", myGlobal.ErrorCode, myGlobal.ErrorMessage)
                finalResultOK = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateCollapse ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            finalResultOK = False

        End Try

        Return finalResultOK
    End Function

    ''' <summary>
    ''' The manual Repetition icon will be available in following conditions: 
    ''' ** For STD and ISE Tests 
    ''' ** For the last Rerun and only when the status of the OrderTest is CLOSED (all Reruns are finished)
    ''' ** For PATIENT Results, only if value of setting LIS RERUN WORKING MODE is different of LIS ONLY
    ''' </summary>
    ''' <param name="pRow">Row of typed DataSet ResultsDS.vwksResultsRow</param>
    ''' <param name="pMaxRerunNumber">Number of the last Rerun requested for the OrderTest</param>
    ''' <param name="pRepetitionCriteriaSent"></param>
    ''' <returns>True if a manual Repetition is allowed; otherwise, False</returns>
    ''' <remarks>
    ''' Created by:  AG 15/08/2010
    ''' Modified by: AG 31/08/2010 - Do not use Count ... get the RerunNumber of pRow and search and compare the OrderTestID max rerun number
    '''              AG 08/03/2011 -
    '''              RH 23/04/2012 - Pass pMaxRerunNumber instead
    '''              SG 17/04/2013 - Lock Manual Repetitions in case of Working Mode for Reruns = "LIS"
    '''              JC 14/05/2013 - Lock Manual Repetitions in case of Working Mode for Reruns = "LIS" but only if SampleClass is PATIENT
    ''' </remarks>
    Private Function AllowManualRepetitions(ByVal pRow As ResultsDS.vwksResultsRow, ByVal pMaxRerunNumber As Integer, _
                                            ByVal pSampleClass As String, ByRef pRepetitionCriteriaSent As String) As Boolean
        Dim manualRepAllowed As Boolean = False

        Try
            'SG 17/04/2013 / JC 14/05/2013
            If (MyClass.LISWorkingModeReruns = "LIS" AndAlso pSampleClass = "PATIENTS") Then Return False

            '1st condition: Manual Repetitions are allowed only for STANDARD and ISE Tests
            If (pRow.TestType = "STD" OrElse pRow.TestType = "ISE") Then
                '2nd condition: Average belongs to the max orderTest rerun number (the last requested)
                If (pRow.RerunNumber = pMaxRerunNumber) Then
                    '3rd condition: All reruns are finished ... the OrderTest is CLOSED
                    If (pRow.OrderTestStatus = "CLOSED") Then
                        manualRepAllowed = True
                    End If
                End If

                If (Not manualRepAllowed) Then
                    Dim myRow As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = (From row As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                                                    Where row.OrderTestID = pRow.OrderTestID _
                                                                                  AndAlso row.RerunNumber = pRow.RerunNumber _
                                                                                  AndAlso row.MultiItemNumber = pRow.MultiPointNumber _
                                                                                   Select row).ToList()

                    If (myRow.Count > 0) Then
                        If (Not myRow(0).IsSentNewRerunPostdilutionNull) Then
                            pRepetitionCriteriaSent = myRow(0).SentNewRerunPostdilution
                        End If
                    End If
                    myRow = Nothing
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " AllowManualRepetitions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            manualRepAllowed = False
        End Try
        Return manualRepAllowed
    End Function

    ''' <summary>
    ''' Sorts a List(Of ExecutionsDS.vwksWSExecutionsResultsRow) by a field, ascending or descending
    ''' </summary>
    ''' <param name="theList"></param>
    ''' <param name="byField"></param>
    ''' <param name="Asc"></param>
    ''' <remarks>
    '''Created by: RH - 09/09/2010
    ''' </remarks>
    Private Sub SortList(ByRef theList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow), ByVal byField As String, Optional ByVal Asc As SortType = SortType.ASC)
        If String.IsNullOrEmpty(byField) Then Return

        If (theList Is Nothing) OrElse theList.Count < 2 Then Return

        Dim prop As System.Reflection.PropertyInfo = GetType(ExecutionsDS.vwksWSExecutionsResultsRow).GetProperty(byField)

        If Asc = SortType.ASC Then
            theList = theList.OrderBy(Function(x) prop.GetValue(x, Nothing)).ToList()
        Else
            theList = theList.OrderByDescending(Function(x) prop.GetValue(x, Nothing)).ToList()
        End If
    End Sub

    ''' <summary>
    ''' Sorts a List(Of ResultsDS.vwksResultsRow) by a field, ascending or descending
    ''' </summary>
    ''' <param name="theList"></param>
    ''' <param name="byField"></param>
    ''' <param name="Asc"></param>
    ''' <remarks>
    '''Created by: RH - 09/09/2010
    ''' </remarks>
    Private Sub SortList(ByRef theList As List(Of ResultsDS.vwksResultsRow), ByVal byField As String, Optional ByVal Asc As SortType = SortType.ASC)
        If String.IsNullOrEmpty(byField) Then Return

        If (theList Is Nothing) OrElse theList.Count < 2 Then Return

        Dim prop As System.Reflection.PropertyInfo = GetType(ResultsDS.vwksResultsRow).GetProperty(byField)

        If Asc = SortType.ASC Then
            theList = theList.OrderBy(Function(x) prop.GetValue(x, Nothing)).ToList()
        Else
            theList = theList.OrderByDescending(Function(x) prop.GetValue(x, Nothing)).ToList()
        End If
    End Sub

    ''' <summary>
    ''' Release elements not handle by the GC.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 01/08/2012
    ''' Modified by: AG 10/02/2014 - BT #1496 ==> Mark screen closing when ReleaseElement is called
    ''' </remarks>
    Private Sub ReleaseElements()
        Try
            isClosingFlag = True 'AG 10/02/2014 - #1496 Mark screen closing when ReleaseElement is called

            'Class variables
            RepImage = Nothing
            OKImage = Nothing
            UnCheckImage = Nothing
            KOImage = Nothing
            NoImage = Nothing
            ClassImage = Nothing
            ABS_GRAPHImage = Nothing
            AVG_ABS_GRAPHImage = Nothing
            CURVE_GRAPHImage = Nothing
            INC_NEW_REPImage = Nothing
            RED_NEW_REPImage = Nothing
            EQ_NEW_REPImage = Nothing
            NO_NEW_REPImage = Nothing
            INC_SENT_REPImage = Nothing
            RED_SENT_REPImage = Nothing
            EQ_SENT_REPImage = Nothing

            REP_INCImage = Nothing
            EQUAL_REPImage = Nothing
            RED_REPImage = Nothing
            PATIENT_STATImage = Nothing
            PATIENT_ROUTINEImage = Nothing
            SampleIconList = Nothing
            TestTypeIconList = Nothing
            XtraGridIconList = Nothing
            CollapseIconList = Nothing
            XtraVerticalBar = Nothing

            AverageResultsDS = Nothing
            ExecutionsResultsDS = Nothing
            IsColCollapsed = Nothing
            IsTestSTD = Nothing
            ResultsChart = Nothing
            PrintImage = Nothing

            ExperimentalSortType = Nothing
            CalibratorSortType = Nothing
            BlankSortType = Nothing
            ControlSortType = Nothing
            SampleSortType = Nothing
            IsOrderPrinted = Nothing
            IsOrderHISSent = Nothing
            LISSubHeaderImage = Nothing
            LISHeadImage = Nothing
            PrintHeadImage = Nothing
            LISExperimentalHeadImage = Nothing
            LISControlHeadImage = Nothing
            LISSamplesHeadImage = Nothing
            HeadRect = Nothing
            SamplesAverageList = Nothing
            tblXtraSamples = Nothing
            'mdiAnalyzerCopy = Nothing 'not this variable
            copyRefreshDS = Nothing

            'Buttons with images

            PrintReportButton.Image = Nothing
            PrintCompactReportButton.Image = Nothing
            SummaryButton.Image = Nothing
            PrintSampleButton.Image = Nothing
            PrintTestButton.Image = Nothing
            SendManRepButton.Image = Nothing
            OffSystemResultsButton.Image = Nothing
            ExportButton.Image = Nothing
            ExitButton.Image = Nothing
            bsXlsresults.Image = Nothing

            'TR 25/09/2013 #memory
            StrikeFont = Nothing
            RegularFont = Nothing
            XtraStrikeFont = Nothing
            RegularBkColor = Nothing
            StrikeBkColor = Nothing
            StrikeForeColor = Nothing
            AverageBkColor = Nothing
            AverageForeColor = Nothing
            PrintPictureBox.Image = Nothing
            HISPictureBox.Image = Nothing
            'TR 25/09/2013 #memory

            With CollapseColumnControls
                .Name = CollapseColName
                RemoveHandler .HeaderClickEventHandler, AddressOf GenericDataGridView_CellMouseClick
            End With

            With CollapseColumnExperimentals
                .Name = CollapseColName
                RemoveHandler .HeaderClickEventHandler, AddressOf GenericDataGridView_CellMouseClick
            End With

            With CollapseColumnCalibrators
                .Name = CollapseColName
                RemoveHandler .HeaderClickEventHandler, AddressOf GenericDataGridView_CellMouseClick
            End With

            With CollapseColumnBlanks
                .Name = CollapseColName
                RemoveHandler .HeaderClickEventHandler, AddressOf GenericDataGridView_CellMouseClick
            End With

            '--- Detach variable defined using WithEvents ---
            bsErrorProvider1 = Nothing
            bsProgTestToolTips = Nothing
            bsPanel2 = Nothing
            bsPanel4 = Nothing
            Cycle = Nothing
            Abs1 = Nothing
            Abs2 = Nothing
            Diff = Nothing
            OrderToExportCheckBox = Nothing
            OrderToPrintCheckBox = Nothing
            STATImage = Nothing
            ExitButton = Nothing
            bsXlsresults = Nothing
            ExportButton = Nothing
            OffSystemResultsButton = Nothing
            SendManRepButton = Nothing
            bsResultFormGroupBox = Nothing
            bsSamplesResultsTabControl = Nothing
            bsExperimentalsTabPage = Nothing
            bsExperimentalsDataGridView = Nothing
            bsResultsTabControl = Nothing
            bsBlanksTabPage = Nothing
            bsBlanksDataGridView = Nothing
            bsCalibratorsTabPage = Nothing
            bsCalibratorsDataGridView = Nothing
            bsControlsTabPage = Nothing
            bsControlsDataGridView = Nothing
            bsTestDetailsTabControl = Nothing
            bsSamplesTab = Nothing
            bsSamplesListDataGridView = Nothing
            bsTestsTabTage = Nothing
            bsTestsListDataGridView = Nothing
            bsResultsFormLabel = Nothing
            bsTestPanel = Nothing
            PrintTestButton = Nothing
            bsSamplesPanel = Nothing
            PrintReportButton = Nothing
            SummaryButton = Nothing
            PrintSampleButton = Nothing
            XtraSamplesTabPage = Nothing
            SamplesXtraGrid = Nothing
            SamplesXtraGridView = Nothing
            ToolTipController1 = Nothing
            AlarmsDS1 = Nothing
            PrintCompactReportButton = Nothing
            PrintTestBlankButton = Nothing
            PrintTestCtrlButton = Nothing
            '------------------------------------------------

            'GC.Collect() 

        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElement", EventLogEntryType.Error, False)
        End Try
    End Sub
#End Region

#Region "Generic DataGridView Methods"
    ''' <summary>
    ''' View TEST-PATIENT results: Update grid when selected test has patient (or calibrator or blank) results changes
    ''' View TEST-CONTROL results: Update grid when selected test has control (or calibrator or blank) results changes
    ''' View TEST-CALIBRATOR results: Update grid when selected test has calibrator (or blank) results changes
    ''' View TEST-BLANK results: Update grid when selected test has blank results changes
    ''' View PATIENT results: Update grid when selected patient results changes (or new calibrator or blank affects the results of the selected patient)
    ''' </summary>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' Created by:  AG 22/06/2012
    ''' Modified by: SA 19/09/2014 - BA-1927 ==> Fixed errors raised when Option Strict On for the screen was activated (linqs to search new results by SampleClass)
    ''' </remarks>
    Private Sub PrepareFlagsForRefreshAffectedGrids(ByVal pRefreshDS As UIRefreshDS)
        Try
            If Not pRefreshDS Is Nothing Then 'New results received
                Dim newBlanks As List(Of Integer) 'new blank results OrderTestID
                Dim newCalibs As List(Of Integer) 'new calibrator results OrderTestID
                Dim newControls As List(Of Integer) 'new control results OrderTestID
                Dim newPatients As List(Of Integer) 'new patient results OrderTestID
                Dim AverageList As List(Of ResultsDS.vwksResultsRow)

                'Search for new results by sample class
                'BA-1927: Changed the way the Linq was written to avoid errors raised due to Option Strict On has been activated
                newBlanks = (From a In pRefreshDS.ExecutionStatusChanged _
                            Where a.SampleClass = "BLANK" _
                           Select a.OrderTestID Distinct).ToList

                newCalibs = (From a In pRefreshDS.ExecutionStatusChanged _
                            Where a.SampleClass = "CALIB" _
                           Select a.OrderTestID Distinct).ToList

                newControls = (From a In pRefreshDS.ExecutionStatusChanged _
                              Where a.SampleClass = "CTRL" _
                             Select a.OrderTestID Distinct).ToList

                newPatients = (From a In pRefreshDS.ExecutionStatusChanged _
                              Where a.SampleClass = "PATIENT" _
                             Select a.OrderTestID Distinct).ToList


                Dim testViewRefreshRequired As Boolean = False
                Dim patientViewRefreshRequired As Boolean = False

                '1) Check if selected test has received a new blank results. Affects all tabs
                If Not testViewRefreshRequired OrElse Not patientViewRefreshRequired Then

                    'TR 11/07/2012 -Declare Outside the for
                    Dim myTestID As String = String.Empty
                    Dim myTestType As String = String.Empty
                    Dim mySelectedPatientID As String = String.Empty

                    For Each item As Integer In newBlanks
                        AverageList = (From row In AverageResultsDS.vwksResults _
                               Where row.OrderTestID = item Select row).ToList()

                        If AverageList.Count > 0 Then
                            'Analyze for test view refresh
                            If Not testViewRefreshRequired AndAlso BlankTestName <> String.Empty Then
                                If AverageList(0).TestName = BlankTestName Then
                                    testViewRefreshRequired = True
                                    BlankTestName = String.Empty
                                    CalibratorTestName = String.Empty
                                    ControlTestName = String.Empty
                                    SampleTestName = String.Empty

                                    ''Simplification!! Optimal case: We must evaluate if the patient selected has results of the recalculated blank test
                                    'ExperimentalSampleIndex = -1
                                    'SamplesListViewIndex = -1
                                    'Exit For
                                End If
                            End If

                            'Analyze for patient view refresh (We must evaluate if the patient selected has results of the recalculated blank test)
                            If Not patientViewRefreshRequired AndAlso SamplesListViewIndex <> -1 AndAlso bsSamplesListDataGridView.SelectedRows.Count > 0 Then

                                'Dim myTestID As String = String.Empty
                                'Dim myTestType As String = String.Empty
                                myTestID = String.Empty
                                myTestType = String.Empty
                                If Not AverageList(0).IsTestIDNull Then myTestID = AverageList(0).TestID.ToString
                                If Not AverageList(0).IsTestTypeNull Then myTestType = AverageList(0).TestType

                                If (Not myTestID = String.Empty AndAlso Not myTestType = String.Empty) Then
                                    'Dim mySelectedPatientID As String = bsSamplesListDataGridView.SelectedRows(0).Cells("PatientID").Value.ToString()
                                    mySelectedPatientID = bsSamplesListDataGridView.SelectedRows(0).Cells("PatientID").Value.ToString()

                                    AverageList = (From row In AverageResultsDS.vwksResults _
                                                   Where row.PatientID = mySelectedPatientID AndAlso row.SampleClass = "PATIENT" AndAlso _
                                                   row.TestID = Convert.ToInt32(myTestID) AndAlso row.TestType = myTestType Select row).ToList()

                                    If AverageList.Count > 0 Then
                                        patientViewRefreshRequired = True
                                        ExperimentalSampleIndex = -1
                                        SamplesListViewIndex = -1
                                    End If
                                End If

                            ElseIf Not patientViewRefreshRequired _
                                    AndAlso bsSamplesListDataGridView.SelectedRows.Count = 0 _
                                    AndAlso newPatients.Count > 0 Then

                                patientViewRefreshRequired = True
                                ExperimentalSampleIndex = -1
                                SamplesListViewIndex = -1
                            End If

                            If testViewRefreshRequired AndAlso patientViewRefreshRequired Then
                                Exit For
                            End If

                        End If

                    Next
                End If

                '2) Check if selected test has received a new calibrator results. Affects all tabs but the blanks
                If Not testViewRefreshRequired OrElse Not patientViewRefreshRequired Then
                    'TR 11/07/2012 -Declare OutSide the For
                    Dim myTestID As String = String.Empty
                    Dim myTestType As String = String.Empty
                    Dim mySelectedPatientID As String = String.Empty

                    For Each item As Integer In newCalibs
                        AverageList = (From row In AverageResultsDS.vwksResults _
                                       Where row.OrderTestID = item Select row).ToList()

                        If AverageList.Count > 0 Then
                            'Analyze for test view refresh
                            If Not testViewRefreshRequired AndAlso CalibratorTestName <> String.Empty Then
                                If AverageList(0).TestName = CalibratorTestName Then
                                    testViewRefreshRequired = True
                                    CalibratorTestName = String.Empty
                                    ControlTestName = String.Empty
                                    SampleTestName = String.Empty

                                    ''Simplification!! Optimal case: We must evaluate if the patient selected has results of the recalculated calibrator test
                                    'ExperimentalSampleIndex = -1
                                    'SamplesListViewIndex = -1
                                    'Exit For
                                End If
                            End If

                            'Analyze for patient view refresh (We must evaluate if the patient selected has results of the recalculated calibrator test)
                            If Not patientViewRefreshRequired AndAlso SamplesListViewIndex <> -1 AndAlso bsSamplesListDataGridView.SelectedRows.Count > 0 Then

                                'Dim myTestID As String = String.Empty
                                'Dim myTestType As String = String.Empty
                                myTestID = String.Empty
                                myTestType = String.Empty

                                If Not AverageList(0).IsTestIDNull Then myTestID = AverageList(0).TestID.ToString
                                If Not AverageList(0).IsTestTypeNull Then myTestType = AverageList(0).TestType

                                If Not myTestID = String.Empty AndAlso Not myTestType = String.Empty Then
                                    'Dim mySelectedPatientID As String = bsSamplesListDataGridView.SelectedRows(0).Cells("PatientID").Value.ToString()
                                    mySelectedPatientID = bsSamplesListDataGridView.SelectedRows(0).Cells("PatientID").Value.ToString()

                                    AverageList = (From row In AverageResultsDS.vwksResults _
                                                   Where row.PatientID = mySelectedPatientID _
                                                   AndAlso row.SampleClass = "PATIENT" _
                                                   AndAlso row.TestID = Convert.ToInt32(myTestID) _
                                                   AndAlso row.TestType = myTestType _
                                                   Select row).ToList()

                                    If AverageList.Count > 0 Then
                                        patientViewRefreshRequired = True
                                        ExperimentalSampleIndex = -1
                                        SamplesListViewIndex = -1
                                    End If
                                End If

                            ElseIf Not patientViewRefreshRequired AndAlso bsSamplesListDataGridView.SelectedRows.Count = 0 AndAlso newPatients.Count > 0 Then
                                patientViewRefreshRequired = True
                                ExperimentalSampleIndex = -1
                                SamplesListViewIndex = -1
                            End If

                            If testViewRefreshRequired AndAlso patientViewRefreshRequired Then
                                Exit For
                            End If

                        End If
                    Next
                End If

                '3) Check if selected test has received a new control results. Affects only the controls tab
                If Not testViewRefreshRequired Then
                    For Each item As Integer In newControls
                        AverageList = (From row In AverageResultsDS.vwksResults _
                                       Where row.OrderTestID = item Select row).ToList()

                        'Analyze for test view refresh 
                        If AverageList.Count > 0 AndAlso ControlTestName <> String.Empty Then
                            If AverageList(0).TestName = ControlTestName Then
                                testViewRefreshRequired = True
                                ControlTestName = String.Empty
                                Exit For

                            End If
                        End If

                        'Analyze for patient view refresh 
                        'Not required
                    Next
                End If


                '4) Check if selected test has received a new patient results. Affects only the patients tab
                If Not testViewRefreshRequired OrElse Not patientViewRefreshRequired Then 'If no refresh required found then analyze the patients 
                    For Each item As Integer In newPatients
                        AverageList = (From row In AverageResultsDS.vwksResults _
                               Where row.OrderTestID = item Select row).ToList()

                        If AverageList.Count > 0 Then
                            'Analyze for test view refresh 
                            If Not testViewRefreshRequired AndAlso String.Compare(SampleTestName, String.Empty, False) <> 0 Then
                                If String.Compare(AverageList(0).TestName, SampleTestName, False) = 0 Then
                                    testViewRefreshRequired = True
                                    SampleTestName = String.Empty

                                    'Simplification!! Optimal case: We must evaluate if the patient selected it is the same that has been recalculated
                                    'ExperimentalSampleIndex = -1
                                    'SamplesListViewIndex = -1
                                    'Exit For
                                End If
                            End If

                            'Analyze for patient view refresh (We must evaluate if the patient selected it is the same that has been recalculated)
                            If Not patientViewRefreshRequired AndAlso SamplesListViewIndex <> -1 _
                                AndAlso Not AverageList(0).IsPatientIDNull _
                                AndAlso bsSamplesListDataGridView.SelectedRows.Count > 0 _
                                AndAlso SamplesListViewIndex = bsSamplesListDataGridView.SelectedRows(0).Index _
                                AndAlso AverageList(0).PatientID = bsSamplesListDataGridView.SelectedRows(0).Cells("PatientID").Value.ToString() Then

                                patientViewRefreshRequired = True
                                ExperimentalSampleIndex = -1
                                SamplesListViewIndex = -1

                            ElseIf Not patientViewRefreshRequired AndAlso bsSamplesListDataGridView.SelectedRows.Count = 0 Then
                                patientViewRefreshRequired = True
                                ExperimentalSampleIndex = -1
                                SamplesListViewIndex = -1
                            End If

                        End If

                        If testViewRefreshRequired AndAlso patientViewRefreshRequired Then
                            Exit For
                        End If

                    Next
                End If

                newBlanks = Nothing
                newCalibs = Nothing
                newControls = Nothing
                newPatients = Nothing
                AverageList = Nothing

            Else 'Recalculations
                Dim AverageList As List(Of ResultsDS.vwksResultsRow)
                Dim evaluatePatientViewFlag As Boolean = True

                If bsTestDetailsTabControl.SelectedTab.Name = bsSamplesTab.Name Then 'View PATIENT results
                    ExperimentalSampleIndex = -1
                    SamplesListViewIndex = -1
                    'SampleTestName = String.Empty 'Simplification!! Optimal case: We must evaluate if the selected test has the patient result recalculated 

                    'Analyze for test view refresh (We must evaluate if the selected test has the patient result recalculated)
                    If bsTestsListDataGridView.SelectedRows.Count > 0 Then

                        AverageList = (From row In AverageResultsDS.vwksResults _
                                       Where row.PatientID = SamplesListViewText AndAlso row.SampleClass = "PATIENT" _
                                       AndAlso String.Compare(row.TestName, TestsListViewText, False) = 0 Select row).ToList()
                        If AverageList.Count > 0 Then
                            SampleTestName = String.Empty
                        End If
                    End If


                Else 'View TEST results
                    Select Case bsResultsTabControl.SelectedTab.Name
                        Case Me.bsBlanksTabPage.Name
                            'Blank affects all tabs
                            BlankTestName = String.Empty
                            CalibratorTestName = String.Empty
                            ControlTestName = String.Empty
                            SampleTestName = String.Empty
                            'ExperimentalSampleIndex = -1 'Simplification!! Optimal case: We must evaluate if the patient selected has results of the recalculated blank test

                        Case Me.bsCalibratorsTabPage.Name
                            'Calibrator affects all tabs but the blanks 
                            CalibratorTestName = String.Empty
                            ControlTestName = String.Empty
                            SampleTestName = String.Empty
                            'ExperimentalSampleIndex = -1 'Simplification!! Optimal case: We must evaluate if the patient selected has the results of the recalculated calibrator test

                        Case Me.bsControlsTabPage.Name
                            'Control affects only the control tab
                            ControlTestName = String.Empty
                            evaluatePatientViewFlag = False

                        Case Me.XtraSamplesTabPage.Name
                            'Patient affects only the patients tab
                            SampleTestName = String.Empty
                            'ExperimentalSampleIndex = -1 'Simplification!! Optimal case: We must evaluate if the patient selected it is the same that has been recalculated
                    End Select

                    'Analyze for patient view refresh
                    If evaluatePatientViewFlag Then
                        If bsSamplesListDataGridView.SelectedRows.Count > 0 Then
                            AverageList = (From row In AverageResultsDS.vwksResults _
                                           Where String.Compare(row.PatientID, SamplesListViewText, False) = 0 _
                                           AndAlso row.SampleClass = "PATIENT" _
                                           AndAlso row.TestName = TestsListViewText _
                                           Select row).ToList()
                            If AverageList.Count > 0 Then
                                ExperimentalSampleIndex = -1
                            End If
                        End If
                    End If

                End If

                AverageList = Nothing

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " PrepareFlagsForRefreshAffectedGrids ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Loads the Result datasets and updates the data grid views
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 30/08/2010
    ''' Modified by: AG 10/02/2014 - BT #1496 ==> Avoid Screen Refresh when it is closing
    '''              SA 22/09/2014 - BA-1927  ==> Added Try/Catch block
    ''' </remarks>
    Public Sub UpdateScreenGlobalDSWithAffectedResults(Optional ByVal ReloadExecutions As Boolean = True)
        Try
            If (isClosingFlag) Then Exit Sub

            'Update screen global DS with the affected results
            If (ReloadExecutions) Then LoadExecutionsResults()

            'Evaluate if it is needed to refresh the current grids
            PrepareFlagsForRefreshAffectedGrids(Nothing)

            If (bsTestDetailsTabControl.SelectedTab.Name = bsSamplesTab.Name) Then
                UpdateExperimentalsDataGrid()
            Else
                RepaintCurrentResultsGrid()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " UpdateScreenGlobalDSWithAffectedResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Refresh the Results Screen after Export Results to LIS (this function is executed when the LIS delivered notification is received) 
    ''' </summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: AG 10/02/2014 - BT #1496 ==> Avoid Screen Refresh when it is closing
    '''              SA 22/09/2014 - BA-1927 ==> * Refresh the list of Patients also when the Export to LIS has been executed from Tests View to update CheckBox 
    '''                                            ExportToLIS for all Patients which results have been exported 
    '''                                          * Added Try/Catch block
    ''' </remarks>
    Public Sub RefreshExportStatusChanged()
        Try
            If (isClosingFlag) Then Exit Sub
            MyClass.UpdateScreenGlobalDSWithAffectedResults(True)

            'BA-1927: The list of Patients have to be refreshed also when the Export to LIS has been executed from Tests View 
            '         to update CheckBox ExportToLIS for all Patients which results have been exported 
            UpdateSamplesListDataGrid()
            If (bsTestDetailsTabControl.SelectedTab.Name = bsTestsTabTage.Name) Then
                Select Case bsResultsTabControl.SelectedTab.Name
                    Case bsBlanksTabPage.Name
                        'UpdateBlanksDataGrid()
                    Case bsCalibratorsTabPage.Name
                        'UpdateCalibratorsDataGrid()
                    Case bsControlsTabPage.Name
                        UpdateControlsDataGrid()
                    Case XtraSamplesTabPage.Name
                        UpdateSamplesXtraGrid()
                End Select
                Application.DoEvents()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " RefreshExportStatusChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Changes the InUse flag of a replicate and updates the DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 16/07/2010
    ''' Modified by: AG 26/07/2010 - Use recalculations class
    '''              RH 28/07/2010 - Renamed from SetStrike()
    '''              AG 15/09/2010 - Do not allow discard replicates with ErrorABS (same as A25)
    '''              AG 24/11/2010 - After recalculations the icon of LIS Export maybe has changed 
    '''              SA 12/07/2012 - Call to function ChangeInUseFlagReplicate in RecalculateResultsDelegate changed by a call to
    '''                              new function ChangeInUseFlagReplicateNEW in the same Delegate Class
    ''' </remarks>
    Private Sub ChangeInUseFlagReplicate(ByRef dgv As BSDataGridView, ByVal RowIndex As Integer)
        Try
            Dim ContinuePro As Boolean = True

            Dim ExecutionResultRow As ExecutionsDS.vwksWSExecutionsResultsRow = Nothing
            If (Not dgv.Rows(RowIndex).Tag Is Nothing) Then
                ExecutionResultRow = CType(dgv.Rows(RowIndex).Tag, ExecutionsDS.vwksWSExecutionsResultsRow)
            Else
                ContinuePro = False
            End If

            If (ContinuePro) Then
                'AG 15/09/2010 - Don't allow discard replicates with ErrorAbs (same as A25)
                If (Not ExecutionResultRow.IsABS_ErrorNull) Then
                    If (ExecutionResultRow.ABS_Error <> String.Empty) Then ContinuePro = False
                End If
                'END AG 15/09/2010

                If (ContinuePro) Then
                    Dim myGlobal As GlobalDataTO
                    Dim myRecalDelegate As New RecalculateResultsDelegate

                    myRecalDelegate.AnalyzerModel = AnalyzerModel()
                    'myGlobal = myRecalDelegate.ChangeInUseFlagReplicate(Nothing, AnalyzerIDField, WorkSessionIDField, ExecutionResultRow.ExecutionID, Not ExecutionResultRow.InUse)
                    myGlobal = myRecalDelegate.ChangeInUseFlagReplicateNEW(ExecutionResultRow, Not ExecutionResultRow.InUse)

                    Dim actionAllowed As Boolean = False
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        actionAllowed = CType(myGlobal.SetDatos, Boolean)

                        If (actionAllowed) Then
                            UpdateScreenGlobalDSWithAffectedResults()

                            'AG 24/11/2010 - After recalculations the LIS export icon maybe has changed
                            If (String.Compare(bsTestDetailsTabControl.SelectedTab.Name, bsSamplesTab.Name, False) = 0) Then
                                UpdateSamplesListDataGrid()
                            Else
                                UpdatePatientList = True
                            End If
                            'AG 24/11/2010 
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeInUseFlagReplicate ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeInUseFlagReplicate ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub UpdateExecutionAndAverageFromDB(ByRef ExecutionRow As ResultsDS.XtraSamplesRow, ByVal TestName As String)
        LoadExecutionsResults()

        Dim CalcTestInserted As Dictionary(Of String, Boolean) = Nothing
        Dim ISEOFFSTestInserted As Dictionary(Of String, Boolean) = Nothing
        Dim PatientInserted As Dictionary(Of String, Boolean) = Nothing

        CreateAverageList(TestName, CalcTestInserted, ISEOFFSTestInserted, PatientInserted)

        Dim theGroup As String = ExecutionRow.Group

        Dim AverageRow As ResultsDS.XtraSamplesRow = _
                (From aRow In tblXtraSamples _
                 Where aRow.Group = theGroup AndAlso aRow.IsSubHeader _
                 Select aRow).First()

        Dim resultRow As ResultsDS.vwksResultsRow = SamplesAverageList(AverageRow.TagRowIndex)

        'ToDo: Preguntar a Albert qué campos hay que actualizar!!!

        'Update Average row
        If Not resultRow.IsABSValueNull Then
            AverageRow.ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
        Else
            AverageRow.ABSValue = String.Empty
        End If

        If Not resultRow.IsCONC_ValueNull Then
            Dim hasConcentrationError As Boolean = False

            If Not resultRow.IsCONC_ErrorNull Then
                hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error)
            End If

            If Not hasConcentrationError Then
                AverageRow.Concentration = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
            Else
                AverageRow.Concentration = GlobalConstants.CONCENTRATION_NOT_CALCULATED
            End If
        Else
            If (Not resultRow.IsManualResultTextNull) Then
                AverageRow.Concentration = resultRow.ManualResultText
            Else
                AverageRow.Concentration = GlobalConstants.CONCENTRATION_NOT_CALCULATED
            End If
        End If

        If Not resultRow.IsExportStatusNull Then
            If String.Compare(resultRow.ExportStatus, "SENT", False) = 0 Then
                AverageRow.ExportStatus = LISSubHeaderImage
            Else
                AverageRow.ExportStatus = NoImage
            End If
        Else
            AverageRow.ExportStatus = NoImage
        End If

        AverageRow.ResultDate = resultRow.ResultDateTime.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                 resultRow.ResultDateTime.ToString(SystemInfoManager.OSLongTimeFormat)

        'Update Execution row
        Dim ExecutionResultRow As ExecutionsDS.vwksWSExecutionsResultsRow = _
                                    ExecutionsResultsDS.vwksWSExecutionsResults(ExecutionRow.TagRowIndex)

        ExecutionRow.InUse = ExecutionResultRow.InUse
        PrepareFlagsForRefreshAffectedGrids(Nothing) 'AG 26/06/2012 - Evaluate which grids are affected with this recalculation
    End Sub

    ''' <summary>
    ''' Changes the InUse flag of a replicate and updates the DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 07/11/2011
    ''' Modified by: AG 15/09/2010 - Do not allow discard replicates with ErrorABS (same as A25)
    '''              AG 24/11/2010 - After recalculations the icon of LIS Export maybe has changed
    '''              SA 12/07/2012 - Call to function ChangeInUseFlagReplicate in RecalculateResultsDelegate changed by a call to
    '''                              new function ChangeInUseFlagReplicateNEW in the same Delegate Class
    ''' </remarks>
    Private Sub ChangeInUseFlagReplicate(ByRef Row As ResultsDS.XtraSamplesRow)
        Try
            Dim ContinuePro As Boolean = True
            Dim ExecutionResultRow As ExecutionsDS.vwksWSExecutionsResultsRow

            ExecutionResultRow = ExecutionsResultsDS.vwksWSExecutionsResults(Row.TagRowIndex)
            Dim TestName As String = ExecutionResultRow.TestName

            'AG 15/09/2010 - Don't allow discard replicates with ErrorAbs (same as A25)
            If (Not ExecutionResultRow.IsABS_ErrorNull) Then
                If (String.Compare(ExecutionResultRow.ABS_Error, "", False) <> 0) Then ContinuePro = False
            End If
            'END AG 15/09/2010

            If (ContinuePro) Then
                Dim myGlobal As GlobalDataTO
                Dim myRecalDelegate As New RecalculateResultsDelegate

                myRecalDelegate.AnalyzerModel = AnalyzerModel()
                'myGlobal = myRecalDelegate.ChangeInUseFlagReplicate(Nothing, AnalyzerIDField, WorkSessionIDField, ExecutionResultRow.ExecutionID, Not ExecutionResultRow.InUse)
                myGlobal = myRecalDelegate.ChangeInUseFlagReplicateNEW(ExecutionResultRow, Not ExecutionResultRow.InUse)

                Dim actionAllowed As Boolean = False
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    actionAllowed = CType(myGlobal.SetDatos, Boolean)

                    If (actionAllowed) Then
                        UpdateExecutionAndAverageFromDB(Row, TestName)

                        'AG 24/11/2010 - After recalculations the icon of LIS Export maybe has changed
                        If (String.Compare(bsTestDetailsTabControl.SelectedTab.Name, bsSamplesTab.Name, False) = 0) Then
                            UpdateSamplesListDataGrid()
                        Else
                            UpdatePatientList = True
                        End If
                        'AG 24/11/2010 
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeInUseFlagReplicate ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeInUseFlagReplicate ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Strikes (Marks) or unstrikes (Unmarks) a row in a DataGridView control according to some rules
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 28/07/2010
    ''' </remarks>
    Private Sub SetStrike(ByRef dgv As BSDataGridView, ByVal RowIndex As Integer, ByVal InUse As Boolean)
        Try
            If InUse Then
                For Col As Integer = 0 To dgv.ColumnCount - 1
                    If Not dgv.Columns(Col).GetType() Is GetType(DataGridViewTextBoxSpanColumn) Then
                        'If Col <> dgv.Columns("SeeRems").Index Then dgv(Col, RowIndex).Style.Font = RegularFont
                        dgv(Col, RowIndex).Style.Font = RegularFont
                        dgv(Col, RowIndex).Style.BackColor = RegularBkColor
                        dgv(Col, RowIndex).Style.SelectionBackColor = RegularBkColor
                        dgv(Col, RowIndex).Style.ForeColor = RegularForeColor
                        dgv(Col, RowIndex).Style.SelectionForeColor = RegularForeColor
                    End If
                Next
            Else
                For Col As Integer = 0 To dgv.ColumnCount - 1
                    If Not dgv.Columns(Col).GetType() Is GetType(DataGridViewTextBoxSpanColumn) Then
                        'If Col <> dgv.Columns("SeeRems").Index Then dgv(Col, RowIndex).Style.Font = StrikeFont
                        dgv(Col, RowIndex).Style.Font = StrikeFont
                        dgv(Col, RowIndex).Style.BackColor = StrikeBkColor
                        dgv(Col, RowIndex).Style.SelectionBackColor = StrikeBkColor
                        dgv(Col, RowIndex).Style.ForeColor = StrikeForeColor
                        dgv(Col, RowIndex).Style.SelectionForeColor = StrikeForeColor
                    End If
                Next
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " SetStrike ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Spans a DataGridViewCell into "RowSpan" rows
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 16/07/2010
    ''' </remarks>
    Private Sub MergeCells(ByRef dgv As BSDataGridView, ByVal SpanColName As String, ByVal RowIndex As Integer, ByVal RowSpan As Integer)
        Try
            CType(dgv(SpanColName, RowIndex), DataGridViewTextBoxSpanCell).RowSpan = RowSpan

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " MergeCells ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Collapses all the rows in a DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 16/07/2010
    ''' </remarks>
    Private Sub CollapseAll(ByRef dgv As BSDataGridView)
        Try
            CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).CollapseAll()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " CollapseAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Expands all the rows in a DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 16/07/2010
    ''' </remarks>
    Private Sub ExpandAll(ByRef dgv As BSDataGridView)
        Try
            CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).ExpandAll()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ExpandAll ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Updates the Average rows colors
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 16/07/2010
    ''' </remarks>
    Private Sub SetSubHeaderColors(ByRef dgv As BSDataGridView, ByVal RowIndex As Integer)
        Try
            dgv.Rows(RowIndex).DefaultCellStyle.BackColor = AverageBkColor
            dgv.Rows(RowIndex).DefaultCellStyle.SelectionBackColor = AverageBkColor
            dgv.Rows(RowIndex).DefaultCellStyle.ForeColor = AverageForeColor
            dgv.Rows(RowIndex).DefaultCellStyle.SelectionForeColor = AverageForeColor

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " SetSubHeaderColors ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Returns True if the row is striked, False otherwise
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 16/07/2010
    ''' </remarks>
    Private Function IsStriked(ByRef dgv As BSDataGridView, ByVal RowIndex As Integer) As Boolean
        Try
            Return dgv(dgv.ColumnCount - 1, RowIndex).Style.BackColor = StrikeBkColor

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " IsStriked ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Returns True if the row is a SubHeader. That is, it is an Average row. Returns False otherwise.
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 16/07/2010
    ''' </remarks>
    Private Function IsSubHeader(ByRef dgv As BSDataGridView, ByVal RowIndex As Integer) As Boolean
        Try
            Return CType(dgv(CollapseColName, RowIndex), bsDataGridViewCollapseCell).IsSubHeader

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " IsSubHeader ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Enables a Collapse Column
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 16/07/2010
    ''' </remarks>
    Private Sub EnableCollapseColumn(ByRef dgv As BSDataGridView)
        Try
            CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).IsEnabled = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " EnableCollapseColumn ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Disables a Collapse Column
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 16/07/2010
    ''' </remarks>
    Private Sub DisableCollapseColumn(ByRef dgv As BSDataGridView)
        Try
            CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).IsEnabled = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " DisableCollapseColumn ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Sets Repetition PostDilution icon image on the DataGridView
    ''' </summary>
    ''' <param name="dgv"></param>
    ''' <param name="ColName"></param>
    ''' <param name="RowIndex"></param>
    ''' <param name="PostDilutionType"></param>
    ''' <param name="pAllowManualRepetition" ></param>
    ''' <remarks>
    ''' Created by:  RH 27/08/2010
    ''' Modified by: AG 08/03/2011 - Added new parameter pAllowManualRepetition
    ''' </remarks>
    Private Sub SetRepPostDilutionImage(ByRef dgv As BSDataGridView, ByVal ColName As String, ByVal RowIndex As Integer, ByVal PostDilutionType As String, ByVal pAllowManualRepetition As Boolean)
        If pAllowManualRepetition Then
            Select Case PostDilutionType
                Case "INC"
                    dgv(ColName, RowIndex).Value = INC_NEW_REPImage

                Case "RED"
                    dgv(ColName, RowIndex).Value = RED_NEW_REPImage

                Case "NONE"
                    dgv(ColName, RowIndex).Value = EQ_NEW_REPImage

                Case Else
                    dgv(ColName, RowIndex).Value = NO_NEW_REPImage
            End Select
        Else
            Select Case PostDilutionType
                Case "INC"
                    dgv(ColName, RowIndex).Value = INC_SENT_REPImage

                Case "RED"
                    dgv(ColName, RowIndex).Value = RED_SENT_REPImage

                Case "NONE"
                    dgv(ColName, RowIndex).Value = EQ_SENT_REPImage

                Case Else
                    dgv(ColName, RowIndex).Value = NoImage
            End Select
        End If
    End Sub

    ''' <summary>
    ''' Sets Repetition PostDilution icon image on the DataRow
    ''' </summary>
    ''' <param name="Row"></param>
    ''' <param name="PostDilutionType"></param>
    ''' <param name="pAllowManualRepetition" ></param>
    ''' <remarks>
    ''' Created by: RH 02/11/2011
    ''' </remarks>
    Private Sub SetRepPostDilutionImage(ByRef Row As ResultsDS.XtraSamplesRow, ByVal PostDilutionType As String, ByVal pAllowManualRepetition As Boolean)
        If pAllowManualRepetition Then
            Select Case PostDilutionType
                Case "INC"
                    Row.NewRep = INC_NEW_REPImage

                Case "RED"
                    Row.NewRep = RED_NEW_REPImage

                Case "NONE"
                    Row.NewRep = EQ_NEW_REPImage

                Case Else
                    Row.NewRep = NO_NEW_REPImage
            End Select
        Else
            Select Case PostDilutionType
                Case "INC"
                    Row.NewRep = INC_SENT_REPImage

                Case "RED"
                    Row.NewRep = RED_SENT_REPImage

                Case "NONE"
                    Row.NewRep = EQ_SENT_REPImage

                Case Else
                    Row.NewRep = Nothing 'NoImage
            End Select
        End If
    End Sub

    ''' <summary>
    ''' Sets PostDilution icon image on the DataGridView
    ''' </summary>
    ''' <param name="dgv"></param>
    ''' <param name="ColName"></param>
    ''' <param name="RowIndex"></param>
    ''' <param name="PostDilutionType"></param>
    ''' <remarks>
    ''' Created by: RH 30/08/2010
    ''' </remarks>
    Private Sub SetPostDilutionImage(ByRef dgv As BSDataGridView, ByVal ColName As String, ByVal RowIndex As Integer, ByVal PostDilutionType As String)
        Select Case PostDilutionType
            Case "INC"
                dgv(ColName, RowIndex).Value = REP_INCImage

            Case "RED"
                dgv(ColName, RowIndex).Value = RED_REPImage

            Case "EQUAL"
                dgv(ColName, RowIndex).Value = EQUAL_REPImage

            Case Else
                dgv(ColName, RowIndex).Value = EQUAL_REPImage

        End Select
    End Sub

    ''' <summary>
    ''' Sets PostDilution text on the DataGridView
    ''' </summary>
    ''' <param name="dgv"></param>
    ''' <param name="ColName"></param>
    ''' <param name="RowIndex"></param>
    ''' <param name="PostDilutionType"></param>
    ''' <remarks>
    ''' Created by:  DL 23/09/2011
    ''' Modified by: RH 20/10/2011 Code optimization
    ''' </remarks>
    Private Sub SetPostDilutionText(ByRef dgv As BSDataGridView, _
                                    ByVal ColName As String, _
                                    ByVal RowIndex As Integer, _
                                    ByVal PostDilutionType As String, _
                                    ByVal pRerun As Integer)

        If pRerun > 1 Then
            'Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Select Case PostDilutionType
                Case "INC"
                    'mytext = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ManualRerun_Increase", LanguageID)
                    dgv(ColName, RowIndex).Value = labelManualRerunIncrease

                Case "RED"
                    'mytext = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ManualRerun_Decrease", LanguageID)
                    dgv(ColName, RowIndex).Value = labelManualRerunDecrease
                Case Else
                    'mytext = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ManualRerun_Equal", LanguageID)
                    dgv(ColName, RowIndex).Value = labelManualRerunEqual
            End Select
        Else
            dgv(ColName, RowIndex).Value = String.Empty
        End If

    End Sub

    ''' <summary>
    ''' Sets PostDilution text on the DataGridView
    ''' </summary>
    ''' <param name="Row"></param>
    ''' <param name="PostDilutionType"></param>
    ''' <param name="pRerun"></param>
    ''' <remarks>
    ''' Created by: RH 02/11/2011
    ''' </remarks>
    Private Sub SetPostDilutionText(ByRef Row As ResultsDS.XtraSamplesRow, ByVal PostDilutionType As String, ByVal pRerun As Integer)
        If pRerun > 1 Then
            Select Case PostDilutionType
                Case "INC"
                    Row.Rep = labelManualRerunIncrease

                Case "RED"
                    Row.Rep = labelManualRerunDecrease
                Case Else
                    Row.Rep = labelManualRerunEqual
            End Select
        Else
            Row.Rep = String.Empty
        End If
    End Sub

    ''' <summary>
    ''' Removes the old SortGlyph from the DataGridView
    ''' </summary>
    ''' <param name="dgv"></param>
    ''' <remarks>
    ''' Created by: RH 10/09/2010
    ''' </remarks>
    Private Sub RemoveOldSortGlyph(ByRef dgv As BSDataGridView)
        'Remove the old SortGlyph
        For Each Col As DataGridViewColumn In dgv.Columns
            Col.HeaderCell.SortGlyphDirection = SortOrder.None
        Next
    End Sub

    ''' <summary>
    ''' Sorts a DataGridView on a given column and sort type (ascending or descending)
    ''' </summary>
    ''' <param name="dgv"></param>
    ''' <param name="ColIndex"></param>
    ''' <param name="Asc"></param>
    ''' <remarks>
    ''' Created by: RH 10/09/2010
    ''' </remarks>
    Private Sub SortDataGrigView(ByRef dgv As BSDataGridView, ByVal ColIndex As Integer, Optional ByVal Asc As SortType = SortType.ASC)
        Try
            If dgv Is Nothing OrElse dgv.Rows.Count = 0 Then Return

            Const SortingCol As String = "_SortingCol"
            Dim direction As ListSortDirection = ListSortDirection.Ascending

            'Add fake column for sorting
            dgv.Columns.Add(SortingCol, String.Empty)
            dgv.Columns(SortingCol).SortMode = DataGridViewColumnSortMode.Programmatic

            Dim NewValue As Object = Nothing

            RemoveOldSortGlyph(dgv)

            'Build the fake column with proper data to be sorted
            If Asc = SortType.DESC Then
                direction = ListSortDirection.Descending

                'Put the new SortGlyph
                dgv.Columns(ColIndex).HeaderCell.SortGlyphDirection = SortOrder.Descending

                For i As Integer = 0 To dgv.Rows.Count - 1
                    If IsSubHeader(dgv, i) Then NewValue = dgv(ColIndex, i).Value
                    dgv(SortingCol, i).Value = NewValue.ToString + (dgv.Rows.Count - i).ToString("0000")
                Next
            Else
                'Put the new SortGlyph
                dgv.Columns(ColIndex).HeaderCell.SortGlyphDirection = SortOrder.Ascending

                For i As Integer = 0 To dgv.Rows.Count - 1
                    If IsSubHeader(dgv, i) Then NewValue = dgv(ColIndex, i).Value
                    dgv(SortingCol, i).Value = NewValue.ToString + i.ToString("0000")
                Next
            End If

            'Sort the fake column
            dgv.Sort(dgv.Columns(SortingCol), direction)
            dgv.Columns.Remove(SortingCol)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SortDataGrigView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Loads Average values from Database for a given Test and Creates a new Average List for it.
    ''' </summary>
    ''' <param name="TestName"></param>
    ''' <remarks>
    ''' Created by: RH 
    ''' </remarks>
    Private Sub UpdateAverageFromDB(ByVal TestName As String)
        Try
            LoadExecutionsResults()

            Dim CalcTestInserted As Dictionary(Of String, Boolean) = Nothing
            Dim ISEOFFSTestInserted As Dictionary(Of String, Boolean) = Nothing
            Dim PatientInserted As Dictionary(Of String, Boolean) = Nothing

            CreateAverageList(TestName, CalcTestInserted, ISEOFFSTestInserted, PatientInserted)

            If Not tblXtraSamples Is Nothing Then
                Dim SubHeaders As List(Of ResultsDS.XtraSamplesRow) = _
                                (From aRow In tblXtraSamples _
                                Where aRow.IsSubHeader _
                                Select aRow).ToList()

                If SamplesAverageList.Count = SubHeaders.Count Then
                    For i As Integer = 0 To SamplesAverageList.Count - 1
                        'ToDo: Preguntar a Albert qué campos hay que actualizar!!!
                        If SamplesAverageList(i).AcceptedResultFlag Then
                            SubHeaders(i).Ok = OKImage
                        Else
                            SubHeaders(i).Ok = UnCheckImage
                        End If

                        If Not SamplesAverageList(i).IsABSValueNull Then
                            SubHeaders(i).ABSValue = SamplesAverageList(i).ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                        Else
                            SubHeaders(i).ABSValue = String.Empty
                        End If

                        If Not SamplesAverageList(i).IsCONC_ValueNull Then
                            Dim hasConcentrationError As Boolean = False

                            If Not SamplesAverageList(i).IsCONC_ErrorNull Then
                                hasConcentrationError = Not String.IsNullOrEmpty(SamplesAverageList(i).CONC_Error)
                            End If

                            If Not hasConcentrationError Then
                                SubHeaders(i).Concentration = SamplesAverageList(i).CONC_Value.ToStringWithDecimals(SamplesAverageList(i).DecimalsAllowed)
                            Else
                                SubHeaders(i).Concentration = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                            End If
                        Else
                            If (Not SamplesAverageList(i).IsManualResultTextNull) Then
                                SubHeaders(i).Concentration = SamplesAverageList(i).ManualResultText
                            Else
                                SubHeaders(i).Concentration = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                            End If
                        End If

                        If Not SamplesAverageList(i).IsExportStatusNull Then
                            If String.Compare(SamplesAverageList(i).ExportStatus, "SENT", False) = 0 Then
                                SubHeaders(i).ExportStatus = LISSubHeaderImage
                            Else
                                SubHeaders(i).ExportStatus = NoImage
                            End If
                        Else
                            SubHeaders(i).ExportStatus = NoImage
                        End If

                        SubHeaders(i).ResultDate = SamplesAverageList(i).ResultDateTime.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                 SamplesAverageList(i).ResultDateTime.ToString(SystemInfoManager.OSLongTimeFormat)
                    Next

                    PrepareFlagsForRefreshAffectedGrids(Nothing) 'AG 26/06/2012 - Evaluate which grids are affected with this recalculation
                End If
            End If

            If bsTestDetailsTabControl.SelectedTab.Name = bsSamplesTab.Name OrElse _
               (bsTestDetailsTabControl.SelectedTab.Name <> bsSamplesTab.Name AndAlso String.Compare(bsResultsTabControl.SelectedTab.Name, XtraSamplesTabPage.Name, False) <> 0) Then
                UpdateScreenGlobalDSWithAffectedResults(False)
            End If

            'TR 25/09/2013 #memory
            CalcTestInserted = Nothing
            ISEOFFSTestInserted = Nothing
            PatientInserted = Nothing
            'TR 25/09/2013 #memory

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " UpdateAverageFromDB ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Displays the result chart
    ''' </summary>
    ''' <param name="pExecutionNumber">execution identifier</param> 
    ''' <param name="pOrderTestID">order test id</param> 
    ''' <param name="pRerunNumber">rerun number</param>
    ''' <param name="pMultiItemNumber">multiitem number</param> 
    ''' <param name="pReplicate">rerun number</param>
    ''' <remarks>
    ''' Created by: DL 11/02/2011
    ''' </remarks>
    Private Sub ShowResultsChart(ByVal pExecutionNumber As Integer, _
                                 ByVal pOrderTestID As Integer, _
                                 ByVal pRerunNumber As Integer, _
                                 ByVal pMultiItemNumber As Integer, _
                                 ByVal pReplicate As Integer)

        Try
            Using myForm As New IResultsAbsCurve
                myForm.SourceForm = GlobalEnumerates.ScreenCallsGraphical.RESULTSFRM
                myForm.AnalyzerID = AnalyzerIDField
                myForm.WorkSessionID = WorkSessionIDField
                myForm.MultiItemNumber = pMultiItemNumber
                myForm.ReRun = pRerunNumber
                myForm.ExecutionID = pExecutionNumber
                myForm.OrderTestID = pOrderTestID
                myForm.Replicate = pReplicate

                UiAx00MainMDI.AddNoMDIChildForm = myForm 'Inform the MDI the curve calib results is shown
                myForm.ShowDialog()
                UiAx00MainMDI.RemoveNoMDIChildForm = myForm 'Inform the MDI the curve calib results is closed
            End Using

        Catch ex As Exception
            Cursor = Cursors.Default
            Application.DoEvents()

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ShowResultsChart ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: SG 30/08/2010
    ''' </remarks>
    Private Sub RemoveResultsChart()
        Try
            Dim father As System.Windows.Forms.Control = ResultsChart.Parent
            If father IsNot Nothing Then
                father.Controls.Remove(ResultsChart)

                'RH 18/10/2010 Don't call Dispose() on Managed Resources.
                'Let the Garbage Collector do that work.
                'http://visualbasic.about.com/od/usingvbnet/a/disposeobj.htm
                'ResultsChart.Dispose()

                'RH 13/12/2010
                ResultsChart = Nothing

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " RemoveResultsChart ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Reject a result (NO results accepts for all reruns) for a OrderTestID
    ''' </summary>
    ''' <param name="dgv"></param>
    ''' <param name="RowValues"></param>
    ''' <param name="pSampleClass" ></param>
    ''' <remarks>
    ''' Created by:  AG 17/02/2011 
    ''' AG 16/10/2014 BA-2011: 
    '''    Re-evaluate calculated tests always that changes in patient results (remove condition test type = 'STD')
    '''    For patient and controls re-evaluate the OrderToExport value
    ''' </remarks>
    Private Sub RejectResult(ByRef dgv As BSDataGridView, ByVal RowValues As ResultsDS.vwksResultsRow, ByVal pSampleClass As String)
        Try
            Dim myGlobal As New GlobalDataTO


            Cursor = Cursors.WaitCursor     'AG 14/10/2010
            Dim myResDelegate As New ResultsDelegate
            myGlobal = myResDelegate.UpdateAcceptedResult(Nothing, RowValues.OrderTestID, RowValues.RerunNumber, False)

            If Not myGlobal.HasError Then
                'When the OrderTestID belongs to a CALC_TEST Sw has to call recalculations for CALC_TEST
                'AG 16/10/2014 BA-2011
                'If String.Compare(RowValues.TestType, "STD", False) = 0 And String.Compare(pSampleClass, "PATIENT", False) = 0 Then
                If String.Compare(pSampleClass, "PATIENT", False) = 0 Then
                    Dim myCalcTestsDelegate As New OperateCalculatedTestDelegate
                    myCalcTestsDelegate.AnalyzerID = AnalyzerIDField
                    myCalcTestsDelegate.WorkSessionID = WorkSessionIDField
                    myGlobal = myCalcTestsDelegate.ExecuteCalculatedTest(Nothing, RowValues.OrderTestID, True)
                End If
            End If

            'AG 16/10/2014 BA-2011
            If Not myGlobal.HasError AndAlso (pSampleClass = "CTRL" OrElse pSampleClass = "PATIENT") Then
                Dim ordersDlg As New OrdersDelegate
                myGlobal = ordersDlg.SetNewOrderToExportValue(Nothing, , RowValues.OrderTestID)
            End If
            'AG 16/10/2014 BA-2011

            If Not myGlobal.HasError Then
                UpdateScreenGlobalDSWithAffectedResults()

                'AG 24/11/2010 - After change accepted result the HIS export icon maybe changes
                If bsTestDetailsTabControl.SelectedTab.Name = bsSamplesTab.Name Then
                    UpdateSamplesListDataGrid()
                Else
                    UpdatePatientList = True
                End If
                'AG 24/11/2010 

            End If
            Cursor = Cursors.Default    'AG 14/10/2010

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " RejectResult ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            Me.Cursor = Cursors.Default    'AG 14/10/2010

        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Reject a result (NO results accepts for all reruns) for a OrderTestID
    ''' </summary>
    ''' <param name="RowValues"></param>
    ''' <param name="pSampleClass" ></param>
    '''<remarks>
    ''' Created by:  AG 17/02/2011 
    ''' AG 16/10/2014 BA-2011
    '''    Re-evaluate calculated tests always that changes in patient results (remove condition test type = 'STD')
    '''    For patient and controls re-evaluate the OrderToExport value
    ''' </remarks>
    Private Sub RejectResult(ByVal RowValues As ResultsDS.vwksResultsRow, ByVal pSampleClass As String)
        Try
            Dim myGlobal As GlobalDataTO

            Cursor = Cursors.WaitCursor     'AG 14/10/2010

            Dim myResDelegate As New ResultsDelegate
            myGlobal = myResDelegate.UpdateAcceptedResult(Nothing, RowValues.OrderTestID, RowValues.RerunNumber, False)

            If Not myGlobal.HasError Then
                'When the OrderTestID belongs to a CALC_TEST Sw has to call recalculations for CALC_TEST
                'AG 16/10/2014 BA-2011
                'If RowValues.TestType = "STD" AndAlso String.Compare(pSampleClass, "PATIENT", False) = 0 Then
                If String.Compare(pSampleClass, "PATIENT", False) = 0 Then
                    Dim myCalcTestsDelegate As New OperateCalculatedTestDelegate
                    myCalcTestsDelegate.AnalyzerID = AnalyzerIDField
                    myCalcTestsDelegate.WorkSessionID = WorkSessionIDField
                    myGlobal = myCalcTestsDelegate.ExecuteCalculatedTest(Nothing, RowValues.OrderTestID, True)
                End If
            End If

            'AG 16/10/2014 BA-2011
            If Not myGlobal.HasError AndAlso (pSampleClass = "CTRL" OrElse pSampleClass = "PATIENT") Then
                Dim ordersDlg As New OrdersDelegate
                myGlobal = ordersDlg.SetNewOrderToExportValue(Nothing, , RowValues.OrderTestID)
            End If
            'AG 16/10/2014 BA-2011

            If Not myGlobal.HasError Then
                'UpdateScreenGlobalDSWithAffectedResults()

                UpdateAverageFromDB(RowValues.TestName)

                'AG 24/11/2010 - After change accepted result the HIS export icon maybe changes
                If String.Compare(bsTestDetailsTabControl.SelectedTab.Name, bsSamplesTab.Name, False) = 0 Then
                    UpdateSamplesListDataGrid()
                Else
                    UpdatePatientList = True
                End If
                'AG 24/11/2010 
            End If

        Catch ex As Exception
            Me.Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " RejectResult ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub

#End Region

#Region "MultiLanguage"
    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' '<param name="pLanguageID"> The current Language of Application </param>
    ''' Created by:  PG 14/10/2010
    ''' Modified by: RH 18/10/2010 - Remove the LanguageID parameter. Now it is a class property
    '''              SA 20/01/2011 - Get multilanguage tooltip for new button to open the screen for results of 
    '''                              Off-System Order Tests
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels.....
            bsResultsFormLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Results", LanguageID)
            bsBlanksTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Blanks", LanguageID)
            bsCalibratorsTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrators", LanguageID)
            bsControlsTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Controls", LanguageID)
            'DL 23/05/2012
            'bsExperimentalsTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Experimentals", LanguageID)
            bsExperimentalsTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", LanguageID)
            'bsSamplesTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", LanguageID)
            bsSamplesTab.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_AllPatients", LanguageID)
            'XtraSamplesTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Samples", LanguageID)
            XtraSamplesTabPage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSPrep_AllPatients", LanguageID)
            'DL 23/05/2012
            bsTestsTabTage.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", LanguageID) 'JB 01/10/2012 - Resource string unification

            ' For Tooltips...
            bsProgTestToolTips.SetToolTip(ExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", LanguageID))
            bsProgTestToolTips.SetToolTip(ExportButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_ManualExport", LanguageID))
            bsProgTestToolTips.SetToolTip(PrintSampleButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_PrintPatient", LanguageID))
            bsProgTestToolTips.SetToolTip(PrintTestButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_PrintTest", LanguageID)) 'DL 26/07/2012
            bsProgTestToolTips.SetToolTip(PrintTestCtrlButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_PrintControls", LanguageID)) 'IT 01/10/2014 - #BA-1864
            bsProgTestToolTips.SetToolTip(PrintReportButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_IndividualReport", LanguageID))
            bsProgTestToolTips.SetToolTip(SendManRepButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_ManualRerun", LanguageID))
            bsProgTestToolTips.SetToolTip(SummaryButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_OpenSummary", LanguageID))
            CurveToolTip = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_OpenCurve", LanguageID)
            bsProgTestToolTips.SetToolTip(OffSystemResultsButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_OffSystem_Tests_Results", LanguageID))
            bsProgTestToolTips.SetToolTip(bsXlsresults, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Xls_File_Results", LanguageID))
            bsProgTestToolTips.SetToolTip(PrintPictureBox, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_PrintPatient", LanguageID))
            bsProgTestToolTips.SetToolTip(HISPictureBox, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_ManualExport", LanguageID))
            bsProgTestToolTips.SetToolTip(PrintCompactReportButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_CompactReport", LanguageID))
            'cf - printcompactreportbuttontooltip

            'AG 31/12/2010
            labelReportPrintAvailable = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Final_Print", LanguageID)
            labelReportPrintNOTAvailable = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Not_Final_Print", LanguageID)
            labelHISSent = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_HIS_Sent", LanguageID)
            labelHISNOTSent = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Not_HIS_Sent", LanguageID)
            'AG 31/12/2010

            labelOpenAbsorbanceCurve = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ABSORBANCE_CURVE", LanguageID) '"Open Absorbance Curve"

            'RH 20/10/2011
            labelManualRerunIncrease = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ManualRerun_Increase", LanguageID)
            labelManualRerunDecrease = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ManualRerun_Decrease", LanguageID)
            labelManualRerunEqual = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ManualRerun_Equal", LanguageID)
            labelPatient = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Patient", LanguageID)
            labelRerun = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rerun", LanguageID)
            labelConcentration = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", LanguageID)
            labelUnit = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", LanguageID)
            labelConcentration = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", LanguageID)
            labelType = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", LanguageID)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub
#End Region

#Region "Other methods"

    ''' <summary>
    ''' Click on the patient list (header or item)
    ''' - Click on item (name) select item and load results
    ''' - Click on item (column OrderToPrint) change current field value (screen and database)
    ''' - Click on item (column OrderToExport): checks if complies with rule (7) for sending to LIS; if not show warning; if true => change current field value (screen and database). {BA-2018}
    ''' 
    ''' - Click on header (name) sort alphabetical
    ''' - Click on header (columns OrderToExport /OrderToPrint) select all (if any not selected) or deselect all (if all selected) -> New 31/07/2014 #1187
    ''' - Click on header (column OrderToExport): If some or all are not selected => All checkboxes will be selected that comply with rule (7) for sending to LIS;
    '''                                                                              if at least 1 does not comply => show warning; change current field value (screen and database) accordingly. {BA-2018}
    '''                                           If all are selected => All checkboxes will be unchecked.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 04/06/2012
    ''' Modified by: AG 31/07/2014 - BT #1887 ==> Click on header select all / deselect all
    '''              WE 17/10/2014 - BA-2018 Req.7
    '''              AG 22/10/2014 - BA-2011 inform new parameter required pOnlyPatientsFlag = False (it can apply for both patients or controls)
    ''' </remarks>
    Private Sub bsSamplesListDataGridView_CellMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsSamplesListDataGridView.CellMouseClick
        Try
            ' AG 31/07/2014 - #1887
            Dim updateDSRequired As Boolean = False

            ' Define variables common used for click on item and also on header.
            Dim updateColumnOrderToExport As Boolean = False    ' OrderToExport column (True) or OrderToPrint (False).

            Dim myPatientID As String = ""  ' Patient to refresh, "" -> all.
            Dim myOrdersDelegate As New OrdersDelegate
            Dim myOrderToExportValue As Boolean = False
            Dim myOrderToPrintValue As Boolean = False
            Dim resultData As GlobalDataTO = Nothing
            Dim linqRes As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
            Dim dgv As BSDataGridView = bsSamplesListDataGridView
            Dim warningShown As Boolean = False
            Dim ShowWarning As Boolean = False
            Dim currentPatientID As String

            If e.RowIndex >= 0 Then
                ' Click on item.

                ' TR 02/12/2013 bt #1300, set the patientID to search instead the patient id.
                myPatientID = dgv("PatientIDToSearch", e.RowIndex).Value.ToString()

                ' ## Part below only execute for OrderToExport AND Only if Send to LIS checkbox has just been checked (from state unchecked to checked).
                If String.Compare(bsSamplesListDataGridView.Columns(e.ColumnIndex).Name, "OrderToExport", False) = 0 AndAlso Not CBool(dgv(e.ColumnIndex, e.RowIndex).Value) Then

                    ' Determine OrderID(s) for selected Patient.
                    Dim linqOrderID As List(Of String)
                    linqOrderID = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults _
                    Where a.SampleClass = "PATIENT" AndAlso a.PatientID = myPatientID Select a.OrderID Distinct).ToList

                    updateDSRequired = False

                    ' Check if all results are not valid/accepted or all related Tests not mapped to LIS.
                    Dim myResultsDlg As New ResultsDelegate
                    If myResultsDlg.AllResultsNotAcceptedOrAllTestsNotMapped(Nothing, linqOrderID, "PATIENT") Then
                        ' Display message "Results cannot be sent".
                        warningShown = True
                        CreateLogActivity("Results cannot be sent to LIS", Me.Name & " bsSamplesListDataGridView_CellMouseClick ", EventLogEntryType.Warning, False)
                        ShowMessage(Me.Name, GlobalEnumerates.Messages.RESULTS_CANNOT_BE_SENT.ToString, , Me)
                    End If
                End If

                ' ## Execute part below for OrderToExport (but only in case "Results cannot be sent" message is not displayed) and for OrderToPrint.
                If (String.Compare(bsSamplesListDataGridView.Columns(e.ColumnIndex).Name, "OrderToExport", False) = 0 AndAlso Not warningShown) OrElse _
                     String.Compare(bsSamplesListDataGridView.Columns(e.ColumnIndex).Name, "OrderToPrint", False) = 0 Then

                    Cursor = Cursors.WaitCursor

                    ' DS refresh at the end is required.
                    updateDSRequired = True
                    updateColumnOrderToExport = (String.Compare(bsSamplesListDataGridView.Columns(e.ColumnIndex).Name, "OrderToExport", False) = 0)

                    ' Refresh list.
                    Dim myField As String = dgv.Columns(e.ColumnIndex).Name
                    dgv(myField, e.RowIndex).Value = Not CBool(dgv(e.ColumnIndex, e.RowIndex).Value) 'Assign new value = Not current

                    myOrderToExportValue = Convert.ToBoolean(dgv("OrderToExport", e.RowIndex).Value)
                    myOrderToPrintValue = Convert.ToBoolean(dgv("OrderToPrint", e.RowIndex).Value)
                    resultData = myOrdersDelegate.UpdateOutputBySampleID(Nothing, myPatientID, myOrderToPrintValue, myOrderToExportValue)
                End If

            Else
                ' Click on header.
                If String.Compare(bsSamplesListDataGridView.Columns(e.ColumnIndex).Name, "OrderToPrint", False) = 0 Then
                    ' ORDER TO PRINT
                    ' Search new value: some NOT selected -> SELECT ALL // all selected -> SELECT NONE

                    Cursor = Cursors.WaitCursor

                    ' DS refresh at the end is required.
                    updateDSRequired = True
                    updateColumnOrderToExport = False
                    linqRes = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults _
                               Where a.SampleClass = "PATIENT" AndAlso a.OrderToPrint = False Select a).ToList
                    If linqRes.Count > 0 Then
                        ' Some not selected.
                        myOrderToPrintValue = True
                    Else
                        myOrderToPrintValue = False
                    End If

                    resultData = myOrdersDelegate.UpdateOrderToPrint(Nothing, myOrderToPrintValue)

                    ' Refresh list.
                    If Not resultData.HasError Then
                        For i As Integer = 0 To bsSamplesListDataGridView.Rows.Count - 1
                            bsSamplesListDataGridView("OrderToPrint", i).Value = myOrderToPrintValue
                        Next
                    End If

                ElseIf String.Compare(bsSamplesListDataGridView.Columns(e.ColumnIndex).Name, "OrderToExport", False) = 0 Then
                    ' ORDER TO EXPORT
                    ' Search new value: Some or All NOT selected -> SELECT ALL // All selected -> SELECT NONE

                    ' Inform DS refresh at the end of this procedure is not required. It will be done inside the main loop.
                    updateDSRequired = False
                    updateColumnOrderToExport = True

                    linqRes = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults _
                               Where a.SampleClass = "PATIENT" AndAlso a.OrderToExport = False Select a).ToList
                    If linqRes.Count > 0 Then
                        ' Previous state:   Some or All checkboxes are NOT selected.
                        ' New state:        All checkboxes are selected that comply with rule (7) for sending to LIS.

                        Cursor = Cursors.WaitCursor

                        ' FOR EACH Patient Row in column OrderToExport.
                        For item As Integer = 0 To bsSamplesListDataGridView.Rows.Count - 1

                            ' Check if for current Patient Row the checkbox was NOT selected.
                            If Not CBool(bsSamplesListDataGridView("OrderToExport", item).Value) Then
                                ' Complies with rule 7 for sending to LIS ==> Select this checkbox.

                                currentPatientID = bsSamplesListDataGridView("PatientIDToSearch", item).Value.ToString()

                                ' Determine OrderID(s) for selected Patient(i).
                                Dim linqOrderID As List(Of String)
                                linqOrderID = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults _
                                Where a.SampleClass = "PATIENT" AndAlso a.PatientID = currentPatientID Select a.OrderID Distinct).ToList

                                ' Look for more orders related to the same patient as in linqOrder.
                                If linqOrderID.Count = 1 Then
                                    Dim ordersDlg As New OrdersDelegate
                                    resultData = ordersDlg.ReadRelatedOrdersByOrderID(Nothing, linqOrderID(0), "PATIENT")
                                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then

                                        For Each row As OrdersDS.twksOrdersRow In DirectCast(resultData.SetDatos, OrdersDS).twksOrders
                                            If Not linqOrderID.Contains(row.OrderID) Then
                                                linqOrderID.Add(row.OrderID)
                                            End If
                                        Next
                                    End If
                                End If

                                ' Check if for Patient(i) all results are NOT valid/accepted OR all related Tests NOT mapped to LIS.
                                Dim myResultsDlg As New ResultsDelegate
                                If myResultsDlg.AllResultsNotAcceptedOrAllTestsNotMapped(Nothing, linqOrderID, "PATIENT") Then
                                    ' After processing of this loop display message "Results cannot be sent".
                                    ShowWarning = True
                                    ' Don´t allow State (OrderToExport) to change because current Patient Row does NOT comply with rule 7.
                                    ' => myOrderToExportValue stays False.
                                    myOrderToExportValue = False
                                Else
                                    ' Set New state (OrderToExport).
                                    myOrderToExportValue = True
                                End If

                                ' Call UpdateOrderToExport for current Patient's Orders - inform only first element, UpdateOrderToExport seeks all related orders.
                                resultData = myOrdersDelegate.UpdateOrderToExport(Nothing, myOrderToExportValue, True, linqOrderID(0).ToString)

                                ' Refresh the current row in the list.
                                If Not resultData.HasError Then
                                    bsSamplesListDataGridView("OrderToExport", item).Value = myOrderToExportValue
                                End If

                                ' Update DS for the linqOrder elements.
                                If Not resultData.HasError Then
                                    For Each myOrderID As String In linqOrderID
                                        linqRes = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                   Where a.SampleClass = "PATIENT" AndAlso a.OrderID = myOrderID Select a).ToList

                                        For Each row As ExecutionsDS.vwksWSExecutionsResultsRow In linqRes
                                            row.BeginEdit()
                                            row.OrderToExport = myOrderToExportValue
                                            row.EndEdit()
                                        Next
                                        ExecutionsResultsDS.vwksWSExecutionsResults.AcceptChanges()
                                    Next
                                End If

                            Else
                                ' For current Patient Row the checkbox was selected:
                                ' -> Do nothing...
                            End If

                        Next    ' FOR EACH Patient Row in column OrderToExport.

                        ' Show warning if at least 1 Patient Row does NOT comply with rule 7 for sending to LIS.
                        If ShowWarning Then
                            CreateLogActivity("Results cannot be sent to LIS", Me.Name & " bsSamplesListDataGridView_CellMouseClick ", EventLogEntryType.Warning, False)
                            ShowMessage(Me.Name, GlobalEnumerates.Messages.RESULTS_CANNOT_BE_SENT.ToString, , Me)
                        End If

                    Else
                        ' Previous state:   All checkboxes were selected
                        ' New state:        Deselect all checkboxes
                        myOrderToExportValue = False

                        Cursor = Cursors.WaitCursor

                        updateDSRequired = True
                        updateColumnOrderToExport = True

                        ' Force the value for OrderToExport because it is the user desire!!
                        resultData = myOrdersDelegate.UpdateOrderToExport(Nothing, myOrderToExportValue, True)

                        ' Refresh list.
                        If Not resultData.HasError Then
                            For i As Integer = 0 To bsSamplesListDataGridView.Rows.Count - 1
                                bsSamplesListDataGridView("OrderToExport", i).Value = myOrderToExportValue
                            Next
                        End If

                    End If
                End If

            End If

            ' Finally update DS if required. Applies for:
            ' OrderToPrint:  click on item
            '                click on header
            '
            ' OrderToExport: click on item 
            '                click on header only when new state = unselect all
            '                (NOTE: click on header when new state = select all the DataSet refresh will be done in previous loop)
            If updateDSRequired Then
                If Not resultData Is Nothing AndAlso Not resultData.HasError Then
                    If myPatientID <> "" Then 'Click on item
                        linqRes = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults _
                                   Where a.SampleClass = "PATIENT" AndAlso a.PatientID = myPatientID Select a).ToList

                    Else 'Click on header
                        linqRes = (From a As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults _
                                   Where a.SampleClass = "PATIENT" Select a).ToList
                    End If

                    For Each row As ExecutionsDS.vwksWSExecutionsResultsRow In linqRes
                        row.BeginEdit()
                        If Not updateColumnOrderToExport Then
                            row.OrderToPrint = myOrderToPrintValue
                        Else
                            row.OrderToExport = myOrderToExportValue
                        End If
                        row.EndEdit()
                    Next

                    ExecutionsResultsDS.vwksWSExecutionsResults.AcceptChanges()
                End If
            End If
            linqRes = Nothing ' Release memory.

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " bsSamplesListDataGridView_CellMouseClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            ' If Cursor <> Cursors.Default Then Cursor = Cursors.Default

            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub bsSamplesListDataGridView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSamplesListDataGridView.Click
        Try
            If isClosingFlag Then Exit Sub ' XB 26/03/2014 - #1496 No refresh if screen is closing

            If Not ProcessEvent Then Return

            If bsSamplesListDataGridView.RowCount > 0 AndAlso SamplesListViewIndex <> bsSamplesListDataGridView.SelectedRows(0).Index Then
                'AG 29/08/2013 - Use the tag instead of the Value
                'SamplesListViewText = bsSamplesListDataGridView.SelectedRows(0).Cells("PatientID").Value.ToString()
                SamplesListViewText = bsSamplesListDataGridView.SelectedRows(0).Cells("PatientID").Tag.ToString()
                'AG 29/08/2013
                SamplesListViewIndex = bsSamplesListDataGridView.SelectedRows(0).Index
                ExperimentalSampleIndex = -1
                Dim StartTime As DateTime = Now 'AG 21/06/2012 - time estimation
                UpdateExperimentalsDataGrid()
                Debug.Print("IResults.UpdateExperimentalsDataGrid: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 21/06/2012 - time estimation
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSamplesListDataGridView_SelectionChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Manages the index when grid is manually sorted
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 10/07/2013
    ''' </remarks>
    Private Sub bsSamplesListDataGridView_Sorted(sender As Object, e As EventArgs) Handles bsSamplesListDataGridView.Sorted
        Try
            If bsSamplesListDataGridView.SelectedRows.Count > 0 Then
                SamplesListViewIndex = bsSamplesListDataGridView.SelectedRows(0).Index
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "bsSamplesListDataGridView_Sorted ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSamplesListDataGridView_Sorted ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Open curve form
    ''' </summary>
    ''' <param name="pSampleType"></param>
    ''' <remarks>
    ''' Created by:  DL 23/03/2011
    ''' Modified by: AG 18/07/2012 - Added parameter pSampleType and apply it to linq
    ''' </remarks>
    Private Sub ShowCurve(ByVal pSampleType As String)
        Try
            Cursor = Cursors.WaitCursor

            'RH 19/10/2010 Introduce the Using statement
            Using myCurveForm As New IResultsCalibCurve

                'Inform the properties
                Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                 (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                  Where String.Compare(row.TestName, TestsListViewText, False) = 0 AndAlso String.Compare(row.SampleClass, "CALIB", False) = 0 AndAlso row.SampleType = pSampleType _
                  Select row).ToList()

                With myCurveForm
                    .ActiveAnalyzer = AnalyzerIDField
                    .ActiveWorkSession = WorkSessionIDField
                    .AnalyzerModel = AnalyzerModel()
                    .AverageResults = AverageResultsDS
                    .ExecutionResults = ExecutionsResultsDS
                    .SelectedTestName = TestsListViewText
                    .SelectedSampleType = pSampleType 'TestList(0).SampleType
                    .SelectedFullTestName = FullTestName
                    .SelectedLot = LotListViewText
                    .SelectedCalibrator = CalibratorListViewText

                    .AcceptedRerunNumber = (From row In AverageResultsDS.vwksResults _
                                            Where row.OrderTestID = TestList(0).OrderTestID _
                                            AndAlso row.AcceptedResultFlag = True _
                                            Select row.RerunNumber).First
                End With

                UiAx00MainMDI.AddNoMDIChildForm = myCurveForm 'Inform the MDI the curve calib results is shown
                myCurveForm.ShowDialog()
                UpdateScreenGlobalDSWithAffectedResults()
                UiAx00MainMDI.RemoveNoMDIChildForm = myCurveForm 'Inform the MDI the curve calib results is closed

                'TR 25/09/2013 #memory
                TestList = Nothing

            End Using

        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCurveButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub bsSamplesListDataGridView_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles bsSamplesListDataGridView.KeyUp
        'TR 11/07/2012 -Validate the keys PageUp and PageDown.
        If (e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down _
            OrElse e.KeyCode = Keys.PageUp OrElse e.KeyCode = Keys.PageDown OrElse e.KeyCode = Keys.Enter) Then
            bsSamplesListDataGridView_Click(Nothing, Nothing)
        End If
    End Sub
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Creates a new instance of this class
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 13/01/2011
    ''' </remarks>
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    ''' <summary>
    ''' Updates the Results gridview
    ''' </summary>
    ''' <param name="pRefreshEventType">RefreshEventType</param>
    ''' <param name="pRefreshDS">UIRefreshDS with info to update</param>
    ''' <remarks>
    ''' Created by:  RH 10/01/2011
    ''' Modified by: AG 12/04/2011 - Rename for RefreshScreen method (use the same method name in all screen)
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS)
        Try
            Dim startTime As DateTime = Now 'AG 26/06/2012 - time estimation

            If isClosingFlag Then Exit Sub 'AG 03/08/2012 - #1496 No refresh if screen is closing

            'AG 21/06/2012
            If Not pRefreshDS Is Nothing Then
                Dim lockThis As New Object()
                SyncLock lockThis
                    copyRefreshDS = CType(pRefreshDS.Copy(), UIRefreshDS)
                End SyncLock
            Else
                copyRefreshDS = Nothing
            End If
            'AG 21/06/2012

            LoadExecutionsResults()

            'AG 22/06/2012 - evaluate if refresh current grid is required
            'RH 03/08/2011 This forces the methods UpdateXXXXDataGrid() to repaint de grid.
            'BlankTestName = String.Empty
            'CalibratorTestName = String.Empty
            'ControlTestName = String.Empty
            'SampleTestName = String.Empty
            'ExperimentalSampleIndex = -1
            'SamplesListViewIndex = -1
            PrepareFlagsForRefreshAffectedGrids(pRefreshDS)

            UpdateTestsListDataGrid()
            UpdateSamplesListDataGrid()
            Debug.Print("IResults.RefreshScreen (TOTAL): " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 26/06/2012 - time estimation
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".RefreshScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
        copyRefreshDS = Nothing 'AG 21/06/2012
    End Sub

    ''' <summary>
    ''' Old function to Export Results to LIS using files (NOT USED)
    ''' </summary>
    Public Sub ExportResults()
        Try
            Dim resultData As GlobalDataTO
            Dim swParams As New SwParametersDelegate
            resultData = swParams.ReadByAnalyzerModel(Nothing, AnalyzerModel)

            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                Dim myDS As ParametersDS
                myDS = CType(resultData.SetDatos, ParametersDS)

                Dim myList As List(Of ParametersDS.tfmwSwParametersRow)
                myList = (From a As ParametersDS.tfmwSwParametersRow In myDS.tfmwSwParameters _
                          Where String.Compare(a.ParameterName, GlobalEnumerates.SwParameters.XLS_RESULTS_DOC.ToString(), False) = 0 _
                          Select a).ToList()

                Dim filename As String = ""
                If myList.Count > 0 Then filename = myList(0).ValueText

                myList = (From a As ParametersDS.tfmwSwParametersRow In myDS.tfmwSwParameters _
                          Where String.Compare(a.ParameterName, GlobalEnumerates.SwParameters.XLS_PATH.ToString(), False) = 0 _
                          Select a).ToList()

                Dim pathname As String = ""
                If myList.Count > 0 Then pathname = myList(0).ValueText

                If pathname.StartsWith("\") AndAlso Not pathname.StartsWith("\\") Then
                    pathname = Application.StartupPath & pathname & DateTime.Now.ToString("dd-MM-yyyy HH-mm") & "\"
                End If

                If String.Compare(filename, "", False) <> 0 AndAlso String.Compare(pathname, "", False) <> 0 Then
                    Dim xlsResults As New ResultsFileDelegate
                    resultData = xlsResults.ExportXLS(WorkSessionIDField, pathname, filename, AnalyzerIDField)

                    If resultData.HasError Then
                        'Dim myLogAcciones As New ApplicationLogManager()
                        GlobalBase.CreateLogActivity(resultData.ErrorMessage, "ExportCalculations.ExportResults. ExportXLS", EventLogEntryType.Error, False)

                        'DL 15/05/2013
                        'ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), resultData.ErrorMessage, Me)
                        Me.UIThread(Function() ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), resultData.ErrorMessage, Me))
                        'DL 15/05/2013
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExportResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            Me.UIThread(Function() ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))"))
            'DL 15/05/2013

        Finally
            CreatingXlsResults = False
            ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it
        End Try
    End Sub

    Public Function ExportResultsWithParameters(ByVal pFileName As String, ByVal pPath As String, ByVal pWorkSessionID As String) As GlobalDataTO
        Dim resultData As GlobalDataTO = Nothing

        Try

            If String.Compare(pFileName, "", False) <> 0 AndAlso String.Compare(pPath, "", False) <> 0 Then
                Dim xlsResults As New ResultsFileDelegate
                resultData = xlsResults.ExportXLS(pWorkSessionID, pPath, pFileName, AnalyzerIDField)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ExportResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

        Return resultData

    End Function

    ''' <summary>
    ''' Enable or disable functionallity by user level.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 16/05/2012
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    bsXlsresults.Visible = False
                    Exit Select

                Case "OPERATOR"
                    bsXlsresults.Visible = False
                    Exit Select

                Case "ADMINISTRATOR"
                    bsXlsresults.Visible = False
                    Exit Select

            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & "ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "NEW METHODS"
    ''' <summary>
    ''' Change accepted result (different rerun result) for an OrderTestID
    ''' </summary>
    ''' <param name="RowValues">Row of a typed DataSet ResultsDS containing data of the result (OrderTestID/RerunNumber) selected to be accepted</param>
    ''' <remarks>
    ''' Created by:  SA 16/07/2012 - Based on ChangeAcceptedResult
    ''' AG 16/10/2014 BA-2011 For ISE, CALC and OFFS inform test type and sample class in dataset before call RecalculateResultsDelegate
    ''' </remarks>
    Private Sub ChangeAcceptedResultNEW(ByVal RowValues As ResultsDS.vwksResultsRow)
        Try
            Dim myGlobal As New GlobalDataTO
            Dim myRecalDelegate As New RecalculateResultsDelegate

            myRecalDelegate.AnalyzerModel = AnalyzerModel()
            Dim executionRowToRecalculate As ExecutionsDS.vwksWSExecutionsResultsRow

            If String.Equals(RowValues.TestType, "STD") Then
                'Get the maximum MultiItemNumber and ReplicateNumber for the selected OrderTestID/RerunNumber 
                Dim maxItemNumber As Integer = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                               Where row.OrderTestID = RowValues.OrderTestID _
                                             AndAlso row.RerunNumber = RowValues.RerunNumber _
                                              Select row.MultiItemNumber).Max

                'Get all data of the Execution for the maximum MultiItemNumber and ReplicateNumber 
                executionRowToRecalculate = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                            Where row.OrderTestID = RowValues.OrderTestID _
                                          AndAlso row.RerunNumber = RowValues.RerunNumber _
                                          AndAlso row.MultiItemNumber = maxItemNumber _
                                         Order By row.ReplicateNumber Descending _
                                           Select row).First
            Else
                'ISE, OFFS, CALC - Inform the data needed to update the AcceptedResultFlag of the results
                executionRowToRecalculate = ExecutionsResultsDS.vwksWSExecutionsResults.NewvwksWSExecutionsResultsRow 'DL 19/07/2012 
                executionRowToRecalculate.BeginEdit()
                executionRowToRecalculate.AnalyzerID = RowValues.AnalyzerID
                executionRowToRecalculate.WorkSessionID = RowValues.WorkSessionID
                executionRowToRecalculate.OrderTestID = RowValues.OrderTestID
                executionRowToRecalculate.RerunNumber = RowValues.RerunNumber

                'AG 16/10/2014 BA-2011
                executionRowToRecalculate.SampleClass = RowValues.SampleClass
                executionRowToRecalculate.TestType = RowValues.TestType
                'AG 16/10/2014 BA-2011

                executionRowToRecalculate.EndEdit()
            End If

            Cursor = Cursors.WaitCursor

            'AG 16/10/2014 BA-2011
            'myGlobal = myRecalDelegate.ChangeAcceptedResultNEW(executionRowToRecalculate)
            Dim myExportStatus As String = ""
            If Not RowValues.IsExportStatusNull Then myExportStatus = RowValues.ExportStatus
            myGlobal = myRecalDelegate.ChangeAcceptedResultNEW(executionRowToRecalculate, RowValues.ExportStatus)
            'AG 15/10/2014 BA-2011

            If (Not myGlobal.HasError) Then
                UpdateScreenGlobalDSWithAffectedResults()

                If (String.Compare(bsTestDetailsTabControl.SelectedTab.Name, bsSamplesTab.Name, False) = 0) Then
                    UpdateSamplesListDataGrid()
                Else
                    UpdatePatientList = True
                End If
            End If

            Cursor = Cursors.Default
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeAcceptedResultNEW ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeAcceptedResultNEW ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

            Me.Cursor = Cursors.Default
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Change Calibration Type from Experimental to Factor when the Manual Factor is informed OR
    ''' Change Calibration Type from Factor to Experimental when the Manual Factor is deleted
    ''' This change can be done in Calibrators DataGridView in Results Screen and in WS Preparation Screen
    ''' </summary>
    ''' <param name="pUseManualFactor">When TRUE, it indicates the Calibration will be changed from Experimental to Manual
    '''                                When FALSE, it indicates the Calibration will be changed from Manual to Experimental</param>
    ''' <param name="RowValues">Row of typed DataSet ResultsDS (subtable vwksResultsRow) containing all data of the selected 
    '''                         single-point Calibrator</param>
    ''' <param name="pManualFactorValue">When pUseManualFactor is TRUE, the entered Manual Calibration Factor
    '''                                  When pUseManualFactot is FALSE, zero</param>
    ''' <param name="pDgv">Calibrators DataGridView</param>
    ''' <remarks>
    ''' Created by:  SA 16/07/2012 - Based on ChangeCalibrationType
    ''' </remarks>
    Private Sub ChangeCalibrationTypeNEW(ByVal pUseManualFactor As Boolean, ByVal RowValues As ResultsDS.vwksResultsRow, _
                                         ByVal pManualFactorValue As Single, ByRef pDgv As BSDataGridView)
        Try
            Dim myGlobal As New GlobalDataTO
            Dim myRecalDelegate As New RecalculateResultsDelegate

            myRecalDelegate.AnalyzerModel = AnalyzerModel()
            Dim executionRowToRecalculate As ExecutionsDS.vwksWSExecutionsResultsRow

            'Get the maximum MultiItemNumber for the selected OrderTestID/RerunNumber 
            Dim maxItemNumber As Integer = (From row As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionsResultsDS.vwksWSExecutionsResults _
                                           Where row.OrderTestID = RowValues.OrderTestID _
                                         AndAlso row.RerunNumber = RowValues.RerunNumber _
                                          Select row.MultiItemNumber).Max
            If (maxItemNumber = 1) Then
                Cursor = Cursors.WaitCursor

                'Get all data of the Execution for the maximum MultiItemNumber and ReplicateNumber 
                executionRowToRecalculate = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                            Where row.OrderTestID = RowValues.OrderTestID _
                                          AndAlso row.RerunNumber = RowValues.RerunNumber _
                                          AndAlso row.MultiItemNumber = maxItemNumber _
                                       Order By row.ReplicateNumber Descending _
                                         Select row).First

                myGlobal = myRecalDelegate.UpdateManualCalibrationFactorNEW(executionRowToRecalculate, pUseManualFactor, pManualFactorValue)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    Dim actionAllowed As Boolean = CType(myGlobal.SetDatos, Boolean)

                    If (actionAllowed) Then
                        UpdateScreenGlobalDSWithAffectedResults()

                        'After manual factor recalculations the HIS export icon maybe changes
                        If (String.Compare(bsTestDetailsTabControl.SelectedTab.Name, bsSamplesTab.Name, False) = 0) Then
                            UpdateSamplesListDataGrid()
                        End If

                        'AG 26/07/2012 - new versions set the strike row when grid is repainted
                        'If (pUseManualFactor) Then
                        '    pDgv.CurrentRow.DefaultCellStyle.Font = StrikeFont
                        '    pDgv.CurrentCell.Style.Font = RegularFont
                        'Else
                        '    pDgv.CurrentRow.DefaultCellStyle.Font = RegularFont
                        'End If
                    End If
                End If
                Cursor = Cursors.Default
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeCalibrationTypeNEW ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeCalibrationTypeNEW", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

            Cursor = Cursors.Default
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub
#End Region

#Region "METHODS ADDED FOR v3.1.0"
    ''' <summary>
    ''' Activate/Deactivate all screen buttons and also buttons and menu entries in the MainMDI, depending of value of parameter pStatusToSet
    ''' </summary>
    ''' <param name="pStatusToSet">True to activate buttons; False to deactivate them</param>
    ''' <remarks>
    ''' Created by:  SA 19/09/2014 - BA-1927 ==> Created due to this code was executed twice (to deactivate and then to activate) in several
    '''                                          functions. Added PrintCompactReportButton to the list of buttons disabled/enabled (it was missing)
    ''' </remarks>
    Private Sub ActivateDeactivateAllButtons(ByVal pStatusToSet As Boolean)
        Try
            'Disable all screen buttons, and disable also all buttons and menus in the MainMDI
            UiAx00MainMDI.EnableButtonAndMenus(pStatusToSet)
            PrintReportButton.Enabled = pStatusToSet
            PrintCompactReportButton.Enabled = pStatusToSet
            SummaryButton.Enabled = pStatusToSet
            PrintSampleButton.Enabled = pStatusToSet
            PrintTestButton.Enabled = pStatusToSet
            bsXlsresults.Enabled = pStatusToSet
            ExitButton.Enabled = pStatusToSet

            'Buttons with special activation rules...
            If (Not pStatusToSet) Then
                OffSystemResultsButton.Enabled = pStatusToSet
                SendManRepButton.Enabled = pStatusToSet
            Else
                OffSystemResultsButton.Enabled = OffSystemResultsButtonEnabled()
                SendManRepButton.Enabled = SendManRepButtonEnabled()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ActivateDeactivateAllButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Manage the process of manual export of results to LIS 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 18/09/2014 - BA-1927 ==> * Code moved from event ExportButton.Click
    '''                                          * Before start the process, verify if LIS Connection is enabled. If it is not available, stop the process
    '''                                          * Count the number of results to Export before start the process. If it is greater than 100, show the warning 
    '''                                            message (using multilanguage) and stop the process
    '''                                          * Call new function ActivateDeactivateAllButtons to deactivate/activate buttons when the process starts/finishes
    '''                                          * Refresh the list of Patients also when the Export to LIS has been executed from Tests View to update CheckBox 
    '''                                            ExportToLIS for all Patients which results have been exported 
    ''' Modified by: AG 30/09/2014 - BA-1440 ==> Inform that is a manual exportation when call method InvokeUploadResultsLIS
    ''' </remarks>
    Private Sub ExportResultsToLIS()
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Dim StartTime As DateTime = Now
        'Dim myLogAcciones As New ApplicationLogManager()
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Try
            'If LIS Connection is not available, do nothing
            If (Not VerifyExportToLISAllowed()) Then Return

            'BT #1499 - Use parameter MAX_APP_MEMORYUSAGE into performance counters (but do not shown message here) 
            Dim pCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage)
            pCounters.GetAllCounters()
            pCounters = Nothing

            'Disable all screen buttons, and disable also all buttons and menus in the MainMDI
            ActivateDeactivateAllButtons(False)

            'BA-1887: Get value of parameter for the maximum number of results that can be exported in a group (default value = 100)
            Dim resultData As New GlobalDataTO
            Dim swParamDlg As New SwParametersDelegate
            Dim maxResultsToExport As Integer = 100

            resultData = swParamDlg.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_RESULTSTOEXPORT_HIST.ToString, Nothing)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim myDS As ParametersDS = DirectCast(resultData.SetDatos, ParametersDS)

                If (myDS.tfmwSwParameters.Rows.Count > 0) AndAlso (Not myDS.tfmwSwParameters(0).IsValueNumericNull) Then
                    maxResultsToExport = CInt(myDS.tfmwSwParameters(0).ValueNumeric)
                End If
            End If

            'When the current screen view is by Patient, Patient results that have been already exported to LIS will be exported again;
            'When the current screen view is by Test, only Patient and Control results with ExportStatus <> SENT will be exported to LIS
            Dim includeExportedResults As Boolean = (bsTestDetailsTabControl.SelectedTab.Name = bsSamplesTab.Name)

            'BA-1927: Count the number of results to Export to check if it is greater than the maximum allowed
            Dim stopProcess As Boolean = False
            Dim myResultsDelegate As New ResultsDelegate

            resultData = myResultsDelegate.CountTotalResultsToExportToLIS(Nothing, AnalyzerIDField, WorkSessionIDField, includeExportedResults)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                If (CInt(resultData.SetDatos) > maxResultsToExport) Then
                    'Show the warning message and stop the process 
                    stopProcess = True
                    ShowMessage(Me.Name, GlobalEnumerates.Messages.MAX_RESULTS_FOR_LISEXPORT.ToString, , Me)

                ElseIf (CInt(resultData.SetDatos) = 0) Then
                    'If there is nothing to Export, stop the process
                    stopProcess = True
                End If
            Else
                'If an error has happened, stop the export process
                stopProcess = True
            End If

            If (Not stopProcess) Then
                'AG 29/07/2014 - BA-1887 (if current screen view is Patient, includeExportedResults = True, which means that Patient Results already exported will be re-sent)
                Dim myExport As New ExportDelegate
                resultData = myExport.ExportToLISManualNEW(AnalyzerIDField, WorkSessionIDField, True, includeExportedResults)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim exportResults As ExecutionsDS = DirectCast(resultData.SetDatos, ExecutionsDS)

                    'AG 17/02/2014 - BT #1505
                    If (exportResults.twksWSExecutions.Rows.Count > 0) Then
                        'Pass the list of Results to be exported to LIS to the MainMDI
                        UiAx00MainMDI.AddResultsIntoQueueToUpload(exportResults)

                        'AG 17/02/2014 - BT #1505: improvement, copy DS using Merge instead of loops
                        Dim myResultsAlarmsDS As New ResultsDS
                        myResultsAlarmsDS.vwksResultsAlarms.Merge(AverageResultsDS.vwksResultsAlarms)
                        myResultsAlarmsDS.vwksResultsAlarms.AcceptChanges()

                        'AG 02/01/2014 - BT #1433 (v211 patch2)
                        CreateLogActivity("Current Results manual upload", Me.Name & ".ExportResultsToLIS ", EventLogEntryType.Information, False)

                        UiAx00MainMDI.InvokeUploadResultsLIS(False, False, AverageResultsDS, myResultsAlarmsDS, Nothing) 'AG 30/09/2014 - BA-1440 inform that is a manual exportation
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExportResultsToLIS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExportResultsToLIS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        Finally
            UpdateScreenGlobalDSWithAffectedResults()

            'BA-1927: The list of Patients have to be refreshed also when the Export to LIS has been executed from Tests View 
            '         to update CheckBox ExportToLIS for all Patients which results have been exported 
            UpdateSamplesListDataGrid()
            If (bsTestDetailsTabControl.SelectedTab.Name = bsTestsTabTage.Name) Then
                Select Case bsResultsTabControl.SelectedTab.Name
                    Case bsBlanksTabPage.Name
                        'UpdateBlanksDataGrid()
                    Case bsCalibratorsTabPage.Name
                        'UpdateCalibratorsDataGrid()
                    Case bsControlsTabPage.Name
                        UpdateControlsDataGrid()
                    Case XtraSamplesTabPage.Name
                        UpdateSamplesXtraGrid()
                End Select
                Application.DoEvents()
            End If

            'Enable screen buttons, and enable also all buttons and menus in the MainMDI
            ActivateDeactivateAllButtons(True)

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Manual Export of Results: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.ExportResultsToLIS", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'for refreshing data when ack from lis
            ExperimentalSampleIndex = -1 'SG 16/03/2013
        End Try
    End Sub

    ''' <summary>
    ''' Control the availability of screen controls (panels, tab and buttons) according the view selected: by Patient or by Test
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 18/09/2014 - BA-1927 ==> Code moved from event bsTestDetailsTabControl.Selected
    '''                                         Changed the way of set the visibility of ExportButton (there was an error in the previous code)
    '''                                         Call new function SendManRepButtonEnabled to get the availability of button for Send Manual Reruns
    ''' </remarks>
    Private Sub TestDetailsTabSelectedEvent()
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Select Case (bsTestDetailsTabControl.SelectedTab.Name)
                Case bsSamplesTab.Name
                    'View by PATIENT
                    bsSamplesResultsTabControl.Visible = True
                    bsSamplesPanel.Visible = True

                    bsTestPanel.Visible = False
                    bsResultsTabControl.Visible = False

                    'BA-1927: The ExportButton will be always visible in Patients View
                    ExportButton.Visible = True

                Case bsTestsTabTage.Name
                    'View by TEST
                    bsSamplesResultsTabControl.Visible = False
                    bsSamplesPanel.Visible = False

                    bsTestPanel.Visible = True
                    bsResultsTabControl.Visible = True

                    'BA-1927: The ExportButton will be visible if the Selected Tab is Controls or Patients
                    '         The previous comparation by Name was wrong (bsResultsTabControl.SelectedTab.Name = "XtraSamplesTabPage",
                    '         while bsSamplesTab.Name = "bsSamplesTab")
                    ExportButton.Visible = (bsResultsTabControl.SelectedTab.Name = bsControlsTabPage.Name OrElse _
                                            bsResultsTabControl.SelectedTab.Name = XtraSamplesTabPage.Name)

                    'BT #1502 - This two buttons are hide because the test of the new reports have not been executed
                    PrintTestBlankButton.Visible = False
            End Select

            'BA-1927: Call new function SendManRepButtonEnabled to get the availability of button for Send Manual Reruns
            SendManRepButton.Enabled = SendManRepButtonEnabled()

            If (Not ProcessEvent) Then Return
            If (bsTestDetailsTabControl.SelectedTab.Name = bsSamplesTab.Name) Then
                'RH 13/12/2010 
                'Change the order of the instructions: first update the Samples list, and then update the experimental grid.
                'That is the proper way to avoid calling UpdateExperimentalsDataGrid() twice
                If (UpdatePatientList) Then
                    UpdateSamplesListDataGrid()
                    UpdatePatientList = False
                End If
                Application.DoEvents()
                UpdateExperimentalsDataGrid()
            Else
                UpdateCurrentResultsGrid()
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("IResults TAB CHANGE (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0) & _
                                            " OPEN TAB: " & bsTestDetailsTabControl.SelectedTab.Name, _
                                            "IResults.TestDetailsTabSelectedEvent", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".TestDetailsTabSelectedEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Control the availability of screen buttons in view by Test according the tab of SampleClass selected 
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 18/09/2014 - BA-1927 ==> Code moved from event bsResultsTabControl.Selected
    '''                                         Changed the way of set the visibility of ExportButton (there was an error in the previous code)
    '''                                         Call new function SendManRepButtonEnabled to get the availability of button for Send Manual Reruns
    ''' </remarks>
    Private Sub ResultsTabControlSelectedEvent()
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'BA-1927: The ExportButton will be visible if the Selected Tab is Controls or Patients
            '         The previous comparation was incomplete due to for Controls the button remained hidden
            ExportButton.Visible = (bsResultsTabControl.SelectedTab.Name = bsControlsTabPage.Name OrElse _
                                    bsResultsTabControl.SelectedTab.Name = XtraSamplesTabPage.Name)

            'BA-1927: Call new function SendManRepButtonEnabled to get the availability of button for Send Manual Reruns
            SendManRepButton.Enabled = SendManRepButtonEnabled()

            If (Not ProcessEvent) Then Return
            UpdateCurrentResultsGrid()

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("IResults TAB CHANGE (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0) _
                                            & " OPEN TAB: " & bsResultsTabControl.SelectedTab.Name, _
                                            "IResults.ResultsTabControlSelectedEvent", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultsTabControlSelectedEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Control the availability of button for Send Manual Repetition. The button has to be disabled in following conditions:
    ''' (1) If the status of the active Work Session is ABORTED
    ''' (2) In Patients View, if setting LIS Working Mode for Reruns has been set to LIS ONLY
    ''' (3) In Tests View, if the active Tab is Patients and setting LIS Working Mode for Reruns has been set to LIS ONLY
    ''' </summary>
    ''' <returns>True is the button can be enabled; otherwise False</returns>
    ''' <remarks>
    ''' Created by: SA 18/09/2014 - BA-1927
    ''' </remarks>
    Private Function SendManRepButtonEnabled() As Boolean
        Dim buttonEnabled As Boolean = True

        Try
            If (WSStatusField = "ABORTED") Then
                '(1) If the Work Session is ABORTED, the button is not available
                buttonEnabled = False
            Else
                If (bsTestDetailsTabControl.SelectedTab.Name = bsSamplesTab.Name) Then
                    '(2) In Patients View, the button availability depends on value of setting LIS Rerun Mode
                    buttonEnabled = (MyClass.LISWorkingModeReruns <> "LIS")
                Else
                    '(3) Tests View
                    If (bsResultsTabControl.SelectedTab.Name = XtraSamplesTabPage.Name) Then
                        'For PATIENTS, the button availability depends on value of setting LIS Rerun Mode
                        buttonEnabled = (MyClass.LISWorkingModeReruns <> "LIS")
                    Else
                        'For BLANKS, CALIBRATORS and CONTROLS, the button is always available
                        buttonEnabled = True
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SendManRepButtonEnabled", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SendManRepButtonEnabled", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return buttonEnabled
    End Function

    ''' <summary>
    ''' Check the status of the LIS Connection (before execute the process of Export Results to LIS). 
    ''' </summary>
    ''' <returns>True is the Export Results to LIS can be executed; otherwise False</returns>
    ''' <remarks>
    ''' Created by: SA 18/09/2014 - BA-1927
    ''' </remarks>
    Private Function VerifyExportToLISAllowed() As Boolean
        Dim connectEnabled As Boolean = True
        Try
            If (Not mdiAnalyzerCopy Is Nothing AndAlso Not mdiESWrapperCopy Is Nothing) Then
                Dim myESBusinessDlg As New ESBusiness

                Dim runningFlag As Boolean = CBool(IIf(mdiAnalyzerCopy.AnalyzerStatus = AnalyzerManagerStatus.RUNNING, True, False))
                Dim connectingFlag As Boolean = CBool(IIf(mdiAnalyzerCopy.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS", True, False))

                connectEnabled = myESBusinessDlg.AllowLISAction(Nothing, LISActions.HostQuery, runningFlag, connectingFlag, mdiESWrapperCopy.Status, mdiESWrapperCopy.Storage)
            Else
                connectEnabled = False
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".VerifyExportToLISAllowed", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".VerifyExportToLISAllowed", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return connectEnabled
    End Function
#End Region

#Region "TO DELETE"
    ' XB 26/02/2014 - Change the way to open the screen using OpenMDIChildForm generic method to avoid OutOfMem errors when back to Monitor - task #1529
    ' ''' <summary>
    ' ''' Creates a new instance of this class and shows SampleClass and SampleOrTestName info
    ' ''' </summary>
    ' ''' <param name="SampleClass">SampleClass to show</param>
    ' ''' <param name="SampleOrTestName">Sample or Test name to show</param>
    ' ''' <remarks>
    ' ''' Created by: RH 13/01/2011
    ' ''' </remarks>
    'Public Sub New(ByVal SampleClass As String, ByVal SampleOrTestName As String)

    '    ' This call is required by the Windows Form Designer.
    '    InitializeComponent()

    '    ' Add any initialization after the InitializeComponent() call.

    '    Select Case SampleClass
    '        Case "PATIENT"
    '            SamplesListViewText = SampleOrTestName
    '            bsTestDetailsTabControl.SelectedTab = bsSamplesTab

    '            ExportButton.Visible = True
    '            bsSamplesResultsTabControl.Visible = True
    '            bsSamplesPanel.Visible = True

    '            bsTestPanel.Visible = False
    '            bsResultsTabControl.Visible = False

    '        Case "BLANK"
    '            TestsListViewText = SampleOrTestName
    '            bsTestDetailsTabControl.SelectedTab = bsTestsTabTage
    '            bsResultsTabControl.SelectedTab = bsBlanksTabPage

    '            'If UCase(bsResultsTabControl.SelectedTab.Text) = "CONTROLS" Or UCase(bsResultsTabControl.SelectedTab.Text) = "SAMPLES" Then
    '            '    ExportButton.Visible = True
    '            'Else
    '            '    ExportButton.Visible = False
    '            'End If

    '            ExportButton.Visible = False

    '            bsSamplesResultsTabControl.Visible = False
    '            bsSamplesPanel.Visible = False

    '            bsTestPanel.Visible = True
    '            bsResultsTabControl.Visible = True

    '        Case "CALIB"
    '            TestsListViewText = SampleOrTestName
    '            bsTestDetailsTabControl.SelectedTab = bsTestsTabTage
    '            bsResultsTabControl.SelectedTab = bsCalibratorsTabPage

    '            'If UCase(bsResultsTabControl.SelectedTab.Text) = "CONTROLS" Or UCase(bsResultsTabControl.SelectedTab.Text) = "SAMPLES" Then
    '            '    ExportButton.Visible = True
    '            'Else
    '            '    ExportButton.Visible = False
    '            'End If

    '            ExportButton.Visible = False

    '            bsSamplesResultsTabControl.Visible = False
    '            bsSamplesPanel.Visible = False

    '            bsTestPanel.Visible = True
    '            bsResultsTabControl.Visible = True

    '        Case "CTRL"
    '            TestsListViewText = SampleOrTestName
    '            bsTestDetailsTabControl.SelectedTab = bsTestsTabTage
    '            bsResultsTabControl.SelectedTab = bsControlsTabPage

    '            'If UCase(bsResultsTabControl.SelectedTab.Text) = "CONTROLS" Or UCase(bsResultsTabControl.SelectedTab.Text) = "SAMPLES" Then
    '            '    ExportButton.Visible = True
    '            'Else
    '            '    ExportButton.Visible = False
    '            'End If

    '            ExportButton.Visible = True

    '            bsSamplesResultsTabControl.Visible = False
    '            bsSamplesPanel.Visible = False

    '            bsTestPanel.Visible = True
    '            bsResultsTabControl.Visible = True

    '    End Select

    'End Sub
    ' XB 26/02/2014 - task #1529
#End Region
End Class

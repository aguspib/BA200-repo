'Put here your business code for the tab StatesTab inside Monitor Form

Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Controls.UserControls

Partial Public Class IMonitor

#Region "Declaration"

    Private SampleIconList As ImageList
    Private NoImage As Byte() = Nothing
    Private SatImage As Byte() = Nothing
    Private NoSatImage As Byte() = Nothing
    Private FinishedImage As Byte() = Nothing
    Private LockedImage As Byte() = Nothing
    Private FinishedWithErrorsImage As Byte() = Nothing
    Private PrintAvailableImage As Byte() = Nothing
    Private HISSentImage As Byte() = Nothing
    Private PausedImage As Byte() = Nothing
    Private PlayImage As Byte() = Nothing
    Private GraphImage As Byte() = Nothing

    Private STDTestIcon As Byte() = Nothing
    Private CALCTestIcon As Byte() = Nothing
    Private ISETestIcon As Byte() = Nothing
    Private OFFSTestIcon As Byte() = Nothing

    Private BlankIcon As Byte() = Nothing
    Private CalibratorIcon As Byte() = Nothing
    Private ControlIcon As Byte() = Nothing

    Private ReadOnly HeaderGreen As Color = Color.FromArgb(255, 96, 233, 75) 'Color.FromArgb(204, 255, 204) 'Color.FromArgb(51, 204, 51)
    Private ReadOnly HeaderYellow As Color = Color.FromArgb(255, 255, 128)  'Color.FromArgb(255, 255, 153) 
    Private ReadOnly HeaderOrange As Color = Color.FromArgb(255, 255, 201, 14) 'Color.FromArgb(255, 127, 39) 'Color.FromArgb(255, 204, 153) 'Color.FromArgb(255, 204, 0)

    'Private ReadOnly HeaderForeColor As Color = Color.Black

    Private ReadOnly RowGreen As Color = Color.FromArgb(255, 96, 233, 75) 'Color.FromArgb(204, 255, 204)
    Private ReadOnly RowYellow As Color = Color.FromArgb(255, 255, 128) ' Color.FromArgb(255, 255, 153)
    Private ReadOnly RowOrange As Color = Color.FromArgb(255, 255, 201, 14) 'Color.FromArgb(255, 127, 39) 'Color.FromArgb(255, 201, 14) 'Color.FromArgb(255, 204, 153)

    Private myWSGridDS As ExecutionsDS 'AG 27/02/2014 - #1524 replace myExecutionsDS -> myWSGridDS
    Private AverageResultsDS As ResultsDS
    Private myExecutions As List(Of ExecutionsDS.vwksWSExecutionsMonitorRow) 'RH 07/02/2011

    Private resultData As GlobalDataTO
    Private ReadOnly myExecutionsDelegate As New ExecutionsDelegate()

    'TR 25/09/2013 -Remove ReadOnly status, to release #memory.
    Private PausePictureBox As New PictureBox()
    Private PrintPictureBox As New PictureBox()
    Private HISSentPictureBox As New PictureBox()
    Private GraphPictureBox As New PictureBox()
    'TR 25/09/2013 -END.


    Private StatText As String = String.Empty
    Private RoutineText As String = String.Empty
    Private labelReportPrintAvailable As String = String.Empty
    Private labelReportPrintNOTAvailable As String = String.Empty
    Private labelHISSent As String = String.Empty
    Private labelHISNOTSent As String = String.Empty

    'RH 02/05/2011
    Private labelName As String = String.Empty
    Private labelType As String = String.Empty
    Private labelState As String = String.Empty
    Private labelAbsorbanceCurve As String = String.Empty
    Private labelBLANK As String = String.Empty
    Private labelLocked As String = String.Empty
    Private labelFinishedRemarks As String = String.Empty
    Private labelFinishedOpticalKO As String = String.Empty
    Private labelSamplePosStatusFinished As String = String.Empty
    Private labelWSActionPause As String = String.Empty
    Private labelTitleResult As String = String.Empty

    'RH 25/07/2011
    Private labelWSActionResume As String = String.Empty

    'RH 01/02/2011 For statistics
    Private Statistics As DataTable

    Private HeaderFont As Font = New Font("Verdana", 8.25!, FontStyle.Bold, GraphicsUnit.Point, 0)
    Private RowFont As Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, 0)

    Private CurrentRowColor As Color = RowGreen
    Private CurrentHeaderColor As Color = HeaderGreen
    Private MaxRow As Integer = 0
    Private HeaderIndex As Integer = 0

    'AG 12/03/2014 - #1524 alarms icon for header
    Private Enum AlarmsIconValues
        None = 0
        Finished_OK = 1
        With_Remarks = 2
        With_OpticalErrors = 3
        Locked = 4
    End Enum
    Private currentHeaderAlarmsIcon As AlarmsIconValues = AlarmsIconValues.None 'Determines the alarms icon shown in each header (calculated using their children)
    Private currentHeaderShowPrintedIconFlag As Boolean = False 'Determines the printed icon shown in each header (calculated using their children)
    Private currentHeaderShowExportIconFlag As Boolean = False 'Determines the export status icon shown in each header (calculated using their children)
    'AG 12/03/2014 - #1524

    Private LISNameForColumnHeaders As String = "LIS" 'SGM 16/04/2012

#End Region

#Region "Fields"

    'Private Const CollapseColName As String = "Collapse"   ' XB 14/03/2014 - Delete Collapse functionality due System Out of memory cause errors - Task #1543

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Updates the WS Status gridview
    ''' </summary>
    ''' <param name="pRefreshDS">UIRefreshDS with info to update</param>
    ''' <param name="pNewCalculationsFlag">when true the results alarms must be reloaded</param>
    ''' <param name="pAutoRerunAddedFlag">when true means a new auto rerun has been added to WS</param>
    ''' <remarks>
    ''' Created by: RH 10/01/2011
    ''' Modified: AG 27/02/2014 + 14/03/2014 - #1524 Complete re-design query (new parameters required pNewCalculationsFlag, pAutoRerunAddedFlag)
    ''' </remarks>
    Public Sub UpdateWSState(ByVal pRefreshDS As UIRefreshDS, Optional ByVal pNewCalculationsFlag As Boolean = False, Optional ByVal pAutoRerunAddedFlag As Boolean = False)
        Try

            'AG 26/02/2014 - #1521
            If pRefreshDS Is Nothing Then Return
            If (IsDisposed) Then Return 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            'Evaluate if grid has to create rows (slow) or only update rows colors (quick)
            Dim PrevExecutionNumber As Integer = 0
            Dim NewExecutionNumber As Integer = 0
            If Not myWSGridDS Is Nothing Then
                PrevExecutionNumber = myWSGridDS.vwksWSExecutionsMonitor.Rows.Count
            End If

            'Fake refreshs or new automatic reruns added ====> Load data!!
            If pRefreshDS.ExecutionStatusChanged.Rows.Count = 0 OrElse pAutoRerunAddedFlag Then
                LoadExecutionsDS()
            Else
                'Debug.Print(Name & ".UpdateWSState rows to refresh: " & pRefreshDS.ExecutionStatusChanged.Rows.Count)

                If pNewCalculationsFlag Then
                    'If new results are calculated we must update the results & executions alarms information
                    LoadAlarms()
                End If
            End If


            If Not myWSGridDS Is Nothing Then
                NewExecutionNumber = myWSGridDS.vwksWSExecutionsMonitor.Rows.Count
            End If
            Dim CreateDGWRows As Boolean = (NewExecutionNumber - PrevExecutionNumber > 0)

            'Keep this line after making actual updates
            'StartTime = Now 'AG 06/06/2012 - time estimation
            If Not myWSGridDS Is Nothing Then
                UpdateExecutionsDataGridView(CreateDGWRows, pRefreshDS)
            End If
            'Debug.Print("iMonitor.UpdateWSState.UpdateExecutionsDataGridView: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 06/06/2012 - time estimation
            'AG 26/02/2014 - #1521

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateWSState", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpdateWSState", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub

#End Region

#Region "Private Methods"

    Private Sub InitializeStatesTab()
        'Put your initialization code here. It will be executed in the Monitor OnLoad event

        Try
            GetStateTabLabels()
            InitializeStatesGrid()
            LoadImages()
            LoadExecutionsDS()
            UpdateExecutionsDataGridView(True, Nothing)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeStatesTab", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeStatesTab", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    '''  
    ''' </summary>
    ''' <remarks>
    ''' Created by:
    ''' </remarks>
    Private Sub InitializeStatesGrid()
        Try
            'Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim columnName As String

            Const PicLeft As Integer = 9
            Const PicTop As Integer = 8

            'bsWSExecutionsDataGridView.Rows.Clear()
            'bsWSExecutionsDataGridView.Columns.Clear()

            ' XB 14/03/2014 - Delete Collapse functionality due System Out of memory cause errors - Task #1543
            ''Collapse Column
            'Dim CollapseColumn As New bsDataGridViewCollapseColumn()
            'CollapseColumn.Name = CollapseColName
            'bsWSExecutionsDataGridView.Columns.Add(CollapseColumn)

            'StatFlag column
            Dim STATColumn As New DataGridViewImageColumn
            With STATColumn
                .Name = "StatFlag"
                .HeaderText = String.Empty
                .Width = 33
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With
            bsWSExecutionsDataGridView.Columns.Add(STATColumn)

            'Test type column
            'Dim TestTypeColumn As New DataGridViewImageColumn
            'With TestTypeColumn
            '    .Name = "TestType"
            '    .HeaderText = String.Empty
            '    .Width = 30
            '    .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            'End With
            'bsWSExecutionsDataGridView.Columns.Add(TestTypeColumn)

            'PausedFlag column
            Dim PausedColumn As New DataGridViewImageColumn
            With PausedColumn
                .Name = "PausedFlag"
                .HeaderText = String.Empty
                .Width = 33
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With
            bsWSExecutionsDataGridView.Columns.Add(PausedColumn)

            'ElementName column
            columnName = "ElementName"
            bsWSExecutionsDataGridView.Columns.Add(columnName, labelName)

            ' XB 14/03/2014 - Delete Collapse functionality due System Out of memory cause errors - Task #1543
            'bsWSExecutionsDataGridView.Columns(columnName).Width = 180
            bsWSExecutionsDataGridView.Columns(columnName).Width = 205
            ' XB 14/03/2014

            'SampleType column
            columnName = "SampleType"
            bsWSExecutionsDataGridView.Columns.Add(columnName, labelType)
            bsWSExecutionsDataGridView.Columns(columnName).Width = 45
            bsWSExecutionsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Alarms column
            Dim AlarmsColumn As New DataGridViewImageColumn
            With AlarmsColumn
                .Name = "Alarms"
                .HeaderText = labelState
                .Width = 50
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With
            bsWSExecutionsDataGridView.Columns.Add(AlarmsColumn)

            'Print available column
            Dim PrintAvailableColumn As New DataGridViewImageColumn
            With PrintAvailableColumn
                .Name = "PrintAvailable"
                .HeaderText = String.Empty
                .Width = 33
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With
            bsWSExecutionsDataGridView.Columns.Add(PrintAvailableColumn)

            'HISSent column
            Dim HISSentColumn As New DataGridViewImageColumn
            With HISSentColumn
                .Name = "Exported"
                .HeaderText = MyClass.LISNameForColumnHeaders ' String.Empty SGM 16/04/2013
                .Width = 33
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With
            bsWSExecutionsDataGridView.Columns.Add(HISSentColumn)

            'Graph column
            Dim GraphColumn As New DataGridViewImageColumn
            With GraphColumn
                .Name = "Graph"
                .HeaderText = String.Empty
                .Width = 33
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With
            bsWSExecutionsDataGridView.Columns.Add(GraphColumn)

            bsWSExecutionsDataGridView.ScrollBars = ScrollBars.Both

            For i As Integer = 0 To bsWSExecutionsDataGridView.ColumnCount - 1
                bsWSExecutionsDataGridView.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable
            Next

            Const HeadImageSide As Integer = 16

            'PausePictureBox
            'PausePictureBox.BackColor = Color.DarkGray
            PausePictureBox.Name = "PausePictureBox"
            PausePictureBox.SizeMode = PictureBoxSizeMode.StretchImage
            PausePictureBox.Size = New Size(HeadImageSide, HeadImageSide)
            PausePictureBox.TabStop = False

            Dim X As Integer = 0
            For i As Integer = 0 To PausedColumn.Index - 1
                X += bsWSExecutionsDataGridView.Columns(i).Width
            Next

            PausePictureBox.Left = X + PicLeft
            PausePictureBox.Top = PicTop
            bsWSExecutionsDataGridView.Controls.Add(PausePictureBox)

            'PrintPictureBox
            'PrintPictureBox.BackColor = Color.DarkGray
            PrintPictureBox.Name = "PrintPictureBox"
            PrintPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
            PrintPictureBox.Size = New Size(HeadImageSide, HeadImageSide)
            PrintPictureBox.TabStop = False

            X = 0
            For i As Integer = 0 To PrintAvailableColumn.Index - 1
                X += bsWSExecutionsDataGridView.Columns(i).Width
            Next

            PrintPictureBox.Left = X + PicLeft
            PrintPictureBox.Top = PicTop
            bsWSExecutionsDataGridView.Controls.Add(PrintPictureBox)

            'SGM 16/04/2013 - hide Export Icon. Leave as text "LIS"
            ''HISSentPictureBox
            ''HISSentPictureBox.BackColor = Color.DarkGray
            'HISSentPictureBox.Name = "HISSentPictureBox"
            'HISSentPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
            'HISSentPictureBox.Size = New Size(HeadImageSide, HeadImageSide)
            'HISSentPictureBox.TabStop = False

            'X = 0
            'For i As Integer = 0 To HISSentColumn.Index - 1
            '    X += bsWSExecutionsDataGridView.Columns(i).Width
            'Next

            'HISSentPictureBox.Left = X + PicLeft
            'HISSentPictureBox.Top = PicTop
            'bsWSExecutionsDataGridView.Controls.Add(HISSentPictureBox)
            'end SGM 16/04/2013


            'GraphPictureBox
            'GraphPictureBox.BackColor = Color.DarkGray
            GraphPictureBox.Name = "GraphPictureBox"
            GraphPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
            GraphPictureBox.Size = New Size(HeadImageSide, HeadImageSide)
            GraphPictureBox.TabStop = False

            X = 0
            For i As Integer = 0 To GraphColumn.Index - 1
                X += bsWSExecutionsDataGridView.Columns(i).Width
            Next

            GraphPictureBox.Left = X + PicLeft
            GraphPictureBox.Top = PicTop
            bsWSExecutionsDataGridView.Controls.Add(GraphPictureBox)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeStatesGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeStatesGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub


    ''' <summary>
    ''' Creates and initializes the Statistics Data Table
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 01/02/2011
    ''' AG 03/03/2014 - #1524 - define only the statitics shown in presentation
    ''' </remarks>
    Private Sub InitializeStatisticsTable()
        Try
            Statistics = New DataTable("Statistics")

            Statistics.BeginInit()

            'AG 03/03/2014 - #1524
            'Statistics.Columns.Add("TESTTYPE", GetType(String))
            'Statistics.Columns.Add("CLOSEDNOK", GetType(Integer))
            'Statistics.Columns.Add("LOCKED", GetType(Integer))
            'Statistics.Columns.Add("PENDING", GetType(Integer))
            'Statistics.Columns.Add("INPROCESS", GetType(Integer))
            'Statistics.Columns.Add("CLOSED", GetType(Integer))
            'Statistics.Columns.Add("TOTAL", GetType(Integer))

            'Statistics.Rows.Add("STD", 0, 0, 0, 0, 0, 0)
            'Statistics.Rows.Add("CALC", 0, 0, 0, 0, 0, 0)
            'Statistics.Rows.Add("ISE", 0, 0, 0, 0, 0, 0)
            'Statistics.Rows.Add("OFFS", 0, 0, 0, 0, 0, 0)
            'Statistics.Rows.Add("Total", 0, 0, 0, 0, 0, 0)

            Statistics.Columns.Add("CLOSEDNOK", GetType(Integer))
            Statistics.Columns.Add("LOCKED", GetType(Integer))
            Statistics.Columns.Add("PENDING", GetType(Integer))
            Statistics.Columns.Add("INPROCESS", GetType(Integer))
            Statistics.Columns.Add("CLOSED", GetType(Integer))
            Statistics.Columns.Add("TOTAL", GetType(Integer))

            Statistics.Rows.Add(0, 0, 0, 0, 0, 0)
            'AG 03/03/2014

            Statistics.EndInit()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeStatisticsTable", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeStatisticsTable", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub

    ''' <summary>
    ''' Loads the ExecutionsDS with details of the created WorkSession Executions for monitoring
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 02/12/2010
    ''' Modified: AG 27/02/2014 - #1524 Complete re-design query
    ''' </remarks>
    Private Sub LoadExecutionsDS()
        Try
            'Get data for STD and ISE tests
            resultData = myExecutionsDelegate.GetDataForMonitorTabWS(Nothing, ActiveAnalyzer, ActiveWorkSession, True)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim stdIseTestsDS As ExecutionsDS
                stdIseTestsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                resultData.SetDatos = Nothing

                'Read data for CALC and OFFS tests
                resultData = myExecutionsDelegate.GetDataForMonitorTabWS(Nothing, ActiveAnalyzer, ActiveWorkSession, False)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim offsCalcTestsDS As ExecutionsDS
                    offsCalcTestsDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                    resultData.SetDatos = Nothing

                    'Build the final DS myWSGridDS with Headers + ISE-Std tests + Calc-Offs tests
                    If myWSGridDS Is Nothing Then
                        'Create
                        myWSGridDS = New ExecutionsDS
                    Else
                        'Clear current datatables
                        myWSGridDS.vwksWSExecutionsMonitor.Clear()
                        myWSGridDS.vwksWSExecutionsAlarms.Clear()
                    End If

                    If Not stdIseTestsDS Is Nothing Then
                        BuildWSGridDataSet(stdIseTestsDS, True, offsCalcTestsDS)

                    ElseIf Not offsCalcTestsDS Is Nothing Then
                        'WS only with OFFS or CALC tests
                        BuildWSGridDataSet(offsCalcTestsDS, False, Nothing)
                    End If
                End If

                'Get Execution Result Alarms
                'RH 28/01/2011 Get Average Result Alarms for recovering OffSystem test errors
                LoadAlarms()
                resultData.SetDatos = Nothing

                'Finally fill the list of executions but do not use the view
                If Not myExecutions Is Nothing Then myExecutions.Clear()
                If Not resultData.HasError Then
                    resultData = myExecutionsDelegate.GetWSExecutionsMonitor(Nothing, ActiveAnalyzer, ActiveWorkSession)
                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        myExecutions = (From row In DirectCast(resultData.SetDatos, ExecutionsDS).vwksWSExecutionsMonitor _
                                        Order By row.ExecutionID _
                                        Select row).ToList()
                    End If
                End If

            Else
                'Error getting the information of the WS Executions; show it
                ShowMessage(Name & ".LoadExecutionsDS", resultData.ErrorCode, resultData.ErrorMessage, MsgParent)
            End If
            resultData.SetDatos = Nothing


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadExecutionsDS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadExecutionsDS", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub

    ''' <summary>
    ''' Loads the Alarms for the local ExecutionsDS
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 10/01/2011
    ''' </remarks>
    Private Sub LoadAlarms()
        Try
            myWSGridDS.vwksWSExecutionsAlarms.Clear()

            'Get Execution Result Alarms
            resultData = myExecutionsDelegate.GetWSExecutionResultAlarms(Nothing)
            If (Not resultData.HasError) Then
                'AG 26/02/2014 - #1521 - remove wide loops and use Merge
                'For Each resultRow As ExecutionsDS.vwksWSExecutionsAlarmsRow In CType(resultData.SetDatos, ExecutionsDS).vwksWSExecutionsAlarms.Rows
                '    myWSGridDS.vwksWSExecutionsAlarms.ImportRow(resultRow)
                'Next
                myWSGridDS.vwksWSExecutionsAlarms.Merge(DirectCast(resultData.SetDatos, ExecutionsDS).vwksWSExecutionsAlarms)
                myWSGridDS.vwksWSExecutionsAlarms.AcceptChanges()
                'AG 26/02/2014 - #1521
            Else
                'Error getting the information of the Alarms; show it
                ShowMessage(Name & ".LoadAlarms", resultData.ErrorCode, resultData.ErrorMessage, MsgParent)
            End If

            'RH 28/01/2011 Get Average Result Alarms for recovering OffSystem test errors
            AverageResultsDS = New ResultsDS
            Dim myResultsDelegate As New ResultsDelegate
            resultData = myResultsDelegate.GetResultAlarms(Nothing)
            If (Not resultData.HasError) Then
                'AG 26/02/2014 - #1521 - remove wide loops and use Merge
                'For Each resultRow As ResultsDS.vwksResultsAlarmsRow In CType(resultData.SetDatos, ResultsDS).vwksResultsAlarms.Rows
                '    AverageResultsDS.vwksResultsAlarms.ImportRow(resultRow)
                'Next
                AverageResultsDS.vwksResultsAlarms.Merge(DirectCast(resultData.SetDatos, ResultsDS).vwksResultsAlarms)
                AverageResultsDS.vwksResultsAlarms.AcceptChanges()
                'AG 26/02/2014 - #1521
            Else
                ShowMessage(Me.Name, resultData.ErrorCode, resultData.ErrorMessage, MsgParent)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LoadAlarms", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LoadAlarms", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub


    ''' <summary>
    ''' Build the DS for the grid -> insert headers, tests,...
    ''' </summary>
    ''' <param name="pMainTestsDS"></param>
    ''' <param name="pTestsWithExecutionsFlag">TRUE -> MainTestDS contains STD/ISE tests and SecondaryTestDS contains CALC/OFFS
    '''                                        FALSE -> MainTestDS contains CALC/OFFS tests and SecondaryTestDS is nothing</param>
    ''' <param name="pSecondaryTestDS"></param>
    ''' <remarks>AG 28/02/2014 - Created #1524
    ''' AG 24/03/2014 - #1550 - One patient with several sample types --> several headers (1 for each sample type)</remarks>
    Private Sub BuildWSGridDataSet(ByVal pMainTestsDS As ExecutionsDS, ByVal pTestsWithExecutionsFlag As Boolean, Optional ByVal pSecondaryTestDS As ExecutionsDS = Nothing)
        Try
            Dim previousElementName As String = String.Empty 'We can also evaluate when a new header must be added using the previous OrderID but I think is clearer using ElementName (header NAME)
            Dim previousSampleClass As String = String.Empty 'SampleClass for the previous ElementName
            Dim previousType As String = String.Empty 'AG 24/03/2014 - #1550

            Dim headerWithOFFSCALCList As New List(Of String)

            'Get the list of order tests with OFFS / CALC tests
            If pTestsWithExecutionsFlag AndAlso Not pSecondaryTestDS Is Nothing Then
                headerWithOFFSCALCList = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In pSecondaryTestDS.vwksWSExecutionsMonitor Where Not a.IsElementNameNull Select a.ElementName Distinct).ToList
            End If

            'add to WS grid the tests of pMainTestsDS (insert headers when required)
            For Each DataSetRow As ExecutionsDS.vwksWSExecutionsMonitorRow In pMainTestsDS.vwksWSExecutionsMonitor
                'Check if is required add new header row 
                'AG 24/03/2014- #1550 (only for patients: same element name with different type means different header!!!)
                'If DataSetRow.ElementName <> previousElementName Then
                If DataSetRow.ElementName <> previousElementName OrElse (DataSetRow.SampleClass = "PATIENT" AndAlso DataSetRow.SampleType <> previousType) Then
                    'AG 24/03/2014- #1550

                    'When we are adding tests with executions before add new header insert, if any, evaluate if there are some CALC / OFFS tests for the previous elementName
                    'Only for patients
                    If pTestsWithExecutionsFlag AndAlso (previousElementName <> String.Empty) _
                        AndAlso (previousSampleClass = "PATIENT") _
                        AndAlso (headerWithOFFSCALCList.Count > 0 AndAlso headerWithOFFSCALCList.Contains(previousElementName)) Then

                        'AG 24/03/2014 - #1550 
                        'InsertCALCandOFFSTests(previousElementName, pSecondaryTestDS)
                        'headerWithOFFSCALCList.Remove(previousElementName)
                        If InsertCALCandOFFSTests(previousElementName, previousType, pSecondaryTestDS) Then
                            headerWithOFFSCALCList.Remove(previousElementName)
                        End If
                        'AG 24/03/2014 - #1550 add new parameter
                    End If

                    'Add header row into myWSGridDS!!
                    DataSetRow.BeginEdit()
                    DataSetRow.IsHeader = True
                    DataSetRow.Index = myWSGridDS.vwksWSExecutionsMonitor.Rows.Count
                    DataSetRow.EndEdit()
                    myWSGridDS.vwksWSExecutionsMonitor.ImportRow(DataSetRow)
                    previousElementName = DataSetRow.ElementName  'Set the new header value
                    If Not DataSetRow.IsSampleTypeNull Then previousType = DataSetRow.SampleType Else previousType = String.Empty 'AG 20/03/2014 - #1550
                    If Not DataSetRow.IsSampleClassNull Then previousSampleClass = DataSetRow.SampleClass Else previousSampleClass = String.Empty
                End If

                'Add the test row  ... after add the header (if required)
                DataSetRow.BeginEdit()
                If DataSetRow.IsIsHeaderNull OrElse DataSetRow.IsHeader Then DataSetRow.IsHeader = False
                DataSetRow.Index = myWSGridDS.vwksWSExecutionsMonitor.Rows.Count
                DataSetRow.EndEdit()
                myWSGridDS.vwksWSExecutionsMonitor.ImportRow(DataSetRow)
            Next

            'Once the loop has finished
            'Check if the last patient with executions has also CALC/OFFS tests and add them
            If pTestsWithExecutionsFlag AndAlso (previousElementName <> String.Empty) _
                AndAlso previousSampleClass = "PATIENT" _
                AndAlso (headerWithOFFSCALCList.Count > 0 AndAlso headerWithOFFSCALCList.Contains(previousElementName)) Then

                'AG 24/03/2014 - #1550
                'InsertCALCandOFFSTests(previousElementName, pSecondaryTestDS)
                'headerWithOFFSCALCList.Remove(previousElementName)
                If InsertCALCandOFFSTests(previousElementName, previousType, pSecondaryTestDS) Then
                    headerWithOFFSCALCList.Remove(previousElementName)
                End If
                'AG 24/03/2014 - #1550 

            End If
            myWSGridDS.vwksWSExecutionsMonitor.AcceptChanges()

            'When we have added all tests with executions maybe exists some patients with only OFFS tests. So add them now
            If pTestsWithExecutionsFlag AndAlso headerWithOFFSCALCList.Count > 0 Then
                BuildWSGridDataSet(pSecondaryTestDS, False, Nothing)
            End If

            headerWithOFFSCALCList.Clear()
            headerWithOFFSCALCList = Nothing

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BuildWSGridDataSet", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".BuildWSGridDataSet", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub


    ''' <summary>
    ''' Add the calculated and offsystem tests into DS used for complete the WS grid
    ''' Function returns a boolean: TRUE means this header has no more CALC/OFFS and it can be deleted
    '''                             FALSE means this header (name( has other CALC/OFFS for others sample types, it cannot be deleted
    ''' </summary>
    ''' <param name="pCurrentHeaderName"></param>
    ''' <param name="pCurrentType">AG 24/03/2014 - #1550</param>
    ''' <param name="pOFFSCALCTestsDS"></param>
    ''' <remarks>Created: AG 27/02/2014 - #1524
    ''' AG 24/03/2014 - #1550 - One patient with several sample types --> several headers (1 for each sample type)
    '''                         Transform into function instead of sub </remarks>
    Private Function InsertCALCandOFFSTests(ByVal pCurrentHeaderName As String, ByVal pCurrentType As String, ByVal pOFFSCALCTestsDS As ExecutionsDS) As Boolean
        Dim headerNameCompleted As Boolean = False 'AG 24/03/2014 - #1550
        Try
            Dim myList As New List(Of ExecutionsDS.vwksWSExecutionsMonitorRow)

            'Get the CALC / OFFS tests of currentheader (1st CALC, later OFFS)
            'AG 24/03/2014 - #1550
            'myList = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In pOFFSCALCTestsDS.vwksWSExecutionsMonitor Where a.ElementName = pCurrentHeaderName Select a Distinct Order By a.TestType Ascending).ToList 
            myList = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In pOFFSCALCTestsDS.vwksWSExecutionsMonitor _
                      Where a.ElementName = pCurrentHeaderName AndAlso a.SampleType = pCurrentType Select a Distinct Order By a.TestType Ascending).ToList
            'AG 24/03/2014 - #1550

            'AG 04/04/2014 - #1576
            Dim myOtCalcDelg As New OrderCalculatedTestsDelegate
            Dim myOtCalcDS As New OrderCalculatedTestsDS
            Dim returnedData As GlobalDataTO
            Dim addCalcOffTestFlag As Boolean = True
            Dim skipBT1550 As Boolean = False 'When a calcTest is not add to monitor grid because the patient has several headers and current is not the correct then leave headerNameCompleted = False
            Dim myOTFormPartList As New List(Of ExecutionsDS.vwksWSExecutionsMonitorRow)
            'AG 04/04/2014 - #1576

            If myList.Count > 0 Then
                For Each row As ExecutionsDS.vwksWSExecutionsMonitorRow In myList
                    'AG 04/04/2014 - #1576 add the CALC test only when all STD/ISE test that form part are added into myWSGridDS
                    'row.BeginEdit()
                    'row.Index = myWSGridDS.vwksWSExecutionsMonitor.Rows.Count
                    'row.EndEdit()
                    'myWSGridDS.vwksWSExecutionsMonitor.ImportRow(row)
                    'row.Delete() 'Delete row from pOFFSCALCTestsDS, in order not to add these CALC / OFFS tests more than 1 time

                    addCalcOffTestFlag = True
                    returnedData = myOtCalcDelg.GetByCalcOrderTestID(Nothing, row.OrderTestID, False, True)
                    If Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing Then
                        myOtCalcDS = DirectCast(returnedData.SetDatos, OrderCalculatedTestsDS)

                        For Each otThatFormsPart As OrderCalculatedTestsDS.twksOrderCalculatedTestsRow In myOtCalcDS.twksOrderCalculatedTests
                            myOTFormPartList = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In myWSGridDS.vwksWSExecutionsMonitor _
                                                Where a.OrderTestID = otThatFormsPart.OrderTestID Select a).ToList
                            If myOTFormPartList.Count = 0 Then
                                addCalcOffTestFlag = False
                                Exit For
                            End If
                        Next
                    End If

                    If addCalcOffTestFlag Then
                        row.BeginEdit()
                        row.Index = myWSGridDS.vwksWSExecutionsMonitor.Rows.Count
                        row.EndEdit()
                        myWSGridDS.vwksWSExecutionsMonitor.ImportRow(row)
                        row.Delete() 'Delete row from pOFFSCALCTestsDS, in order not to add these CALC / OFFS tests more than 1 time
                    Else
                        skipBT1550 = True
                    End If
                    'AG 04/04/2014 - #1576

                Next

                myWSGridDS.vwksWSExecutionsMonitor.AcceptChanges()
                pOFFSCALCTestsDS.vwksWSExecutionsMonitor.AcceptChanges()
            End If

            'AG 24/04/2014 - #1550 - Look for more CALC/OFFS tests for this patient but using other sample types
            If Not skipBT1550 Then
                myList = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In pOFFSCALCTestsDS.vwksWSExecutionsMonitor _
                          Where a.ElementName = pCurrentHeaderName AndAlso a.SampleType <> pCurrentType Select a Distinct Order By a.TestType Ascending).ToList
                If myList.Count > 0 Then headerNameCompleted = False Else headerNameCompleted = True
            End If
            'AG 24/04/2014 - #1550

            myList = Nothing
            myOTFormPartList = Nothing 'AG 04/04/2014 - #1576

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InsertCALCandOFFSTests", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InsertCALCandOFFSTests", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try

        Return headerNameCompleted 'AG 24/03/2014 - #1550

    End Function


    ''' <summary>
    ''' Draw and fill a new row
    ''' </summary>
    ''' <param name="dgv"></param>
    ''' <param name="pNewRow"></param>
    ''' <remarks>
    ''' Created by:  AG 28/02/2014 - BT #1524
    ''' Modified by: AG 24/03/2014 - BT #1550 ==> New business for Alarms icons
    '''              SA 16/06/2014 - BT #1667 ==> Changes in the construction of ToolTips with Patient data:
    '''                                           ** If PatientID = PatientName, only PatientID is shown 
    '''                                           ** If First Character of PatientName is "-" and Last Character of PatientName is "-", remove both  
    '''                                           ** If PatientName is empty, only PatientID is shown
    ''' </remarks>
    Private Sub DrawNewRow(ByVal dgv As BSDataGridView, ByVal pNewRow As ExecutionsDS.vwksWSExecutionsMonitorRow)
        Try
            Dim auxString As String = String.Empty
            CurrentRowColor = AssignRowColor(pNewRow)
            SetRowColor(MaxRow, CurrentRowColor)

            ' XB 14/03/2014 - Delete Collapse functionality due System Out of memory cause errors - Task #1543
            ''Collapse column
            'If pNewRow.IsHeader Then HeaderIndex = MaxRow 'Set the index (in grid and in myWSGridDS) of the last header
            'CType(dgv(CollapseColName, MaxRow), bsDataGridViewCollapseCell).IsSubHeader = pNewRow.IsHeader

            'StatFlag Image column (only header)
            If pNewRow.IsHeader Then
                Select Case pNewRow.SampleClass
                    Case "BLANK"
                        dgv("StatFlag", MaxRow).Value = BlankIcon

                    Case "CALIB"
                        dgv("StatFlag", MaxRow).Value = CalibratorIcon

                    Case "CTRL"
                        dgv("StatFlag", MaxRow).Value = ControlIcon

                    Case "PATIENT"
                        dgv("StatFlag", MaxRow).Value = If(pNewRow.StatFlag, SampleIconList.Images(0), SampleIconList.Images(1))
                End Select

                'StatFlag Image column ToolTip
                If pNewRow.SampleClass <> "BLANK" Then
                    If pNewRow.StatFlag Then
                        dgv("StatFlag", MaxRow).ToolTipText = StatText
                    Else
                        dgv("StatFlag", MaxRow).ToolTipText = RoutineText
                    End If
                End If
            Else 'Child
                dgv("StatFlag", MaxRow).Value = NoImage
                dgv("StatFlag", MaxRow).ToolTipText = String.Empty
            End If

            'Paused flag image column 
            If CurrentRowColor = RowYellow OrElse CurrentRowColor = HeaderYellow Then
                If pNewRow.SampleClass = "PATIENT" And (pNewRow.TestType = "STD" Or pNewRow.TestType = "ISE") Then
                    If Not pNewRow.IsPausedNull AndAlso pNewRow.Paused Then 'First replicate
                        dgv("PausedFlag", MaxRow).Value = PausedImage
                        dgv("PausedFlag", MaxRow).ToolTipText = labelWSActionResume

                        'If one child paused then the header is paused too!!!
                        If Not dgv("PausedFlag", HeaderIndex).Value.Equals(PausedImage) Then
                            dgv("PausedFlag", HeaderIndex).Value = PausedImage 'If some test is paused set the header also with the pause icon
                            dgv("PausedFlag", HeaderIndex).ToolTipText = labelWSActionResume

                            'Update the dataset myWSGridDS
                            myWSGridDS.vwksWSExecutionsMonitor(HeaderIndex).BeginEdit()
                            myWSGridDS.vwksWSExecutionsMonitor(HeaderIndex).Paused = True
                            myWSGridDS.vwksWSExecutionsMonitor(HeaderIndex).EndEdit()
                            myWSGridDS.vwksWSExecutionsMonitor.AcceptChanges()
                        End If

                    Else
                        dgv("PausedFlag", MaxRow).Value = PlayImage
                        dgv("PausedFlag", MaxRow).ToolTipText = labelWSActionPause
                    End If
                End If
            End If

            'ElementName column
            If pNewRow.IsHeader Then
                dgv("ElementName", MaxRow).Style.Font = HeaderFont 'Element name: Font - header name - specimenID - ...
                If Not pNewRow.IsSpecimenIDListNull Then
                    'With barcode
                    auxString = pNewRow.SpecimenIDList
                    If Not pNewRow.IsElementNameNull Then auxString &= " (" & pNewRow.ElementName & ")"
                Else
                    'Without barcode
                    If pNewRow.SampleClass <> "BLANK" Then
                        If Not pNewRow.IsElementNameNull Then auxString = pNewRow.ElementName
                    Else
                        auxString = labelBLANK
                    End If
                End If
            Else
                dgv("ElementName", MaxRow).Style.Font = RowFont
                If Not pNewRow.IsTestNameNull Then auxString = pNewRow.TestName Else auxString = ""
            End If

            If Not pNewRow.IsRerunNumberNull AndAlso pNewRow.RerunNumber > 1 Then 'Add the rerun number (if > 1)
                auxString = String.Format("{0} ({1})", auxString, pNewRow.RerunNumber)
            End If
            dgv("ElementName", MaxRow).Value = auxString

            'ElementName ToolTip column
            If CurrentRowColor = RowGreen OrElse CurrentRowColor = HeaderGreen Then
                dgv("ElementName", MaxRow).ToolTipText = labelTitleResult

                If (pNewRow.SampleClass = "PATIENT") Then
                    'Get value of the Patient Identifier
                    auxString = String.Empty
                    If (Not pNewRow.IsElementNameNull) Then auxString = pNewRow.ElementName

                    'BT #1667 - Get value of Patient First and Last Name (if both of them have a hyphen as value, ignore these 
                    '           fields and shown only the PatientID
                    Dim hyphenIndex As Integer = -1
                    Dim myPatientName As String = pNewRow.PatientName.Trim

                    If (pNewRow.PatientName.Trim.StartsWith("-") AndAlso pNewRow.PatientName.Trim.EndsWith("-")) Then
                        hyphenIndex = pNewRow.PatientName.Trim.IndexOf("-")
                        myPatientName = pNewRow.PatientName.Trim.Remove(hyphenIndex, 1)

                        hyphenIndex = myPatientName.Trim.LastIndexOf("-")
                        myPatientName = myPatientName.Trim.Remove(hyphenIndex, 1)
                    End If

                    If (Not pNewRow.IsSpecimenIDListNull) Then
                        'When the Barcode is informed, the ToolTip will be "BC (PatientID) - FirstName LastName" or, using the variables defined in  
                        'code: "SpecimenIDList (auxString) - PatientName", but with following exceptions (BT #1667):
                        '** If PatientName = "" OR auxString = PatientName, then the ToolTip will be "SpecimenIDList (auxString)"
                        If (auxString.Trim <> myPatientName.Trim AndAlso myPatientName.Trim <> String.Empty) Then
                            dgv("ElementName", MaxRow).ToolTipText = String.Format("{0} ({1}) - {2}", pNewRow.SpecimenIDList, auxString, myPatientName)
                        Else
                            dgv("ElementName", MaxRow).ToolTipText = String.Format("{0} ({1})", pNewRow.SpecimenIDList, auxString)
                        End If
                    Else
                        'When the Barcode is NOT informed, the ToolTip will be "PatientID - FirstName LastName" or, using the variables defined in  
                        'code: "auxString - PatientName", but with following exceptions (BT #1667):
                        '** If PatientName = "" OR auxString = PatientName, then the ToolTip will be "auxString"
                        If (auxString.Trim <> myPatientName.Trim AndAlso myPatientName.Trim <> String.Empty) Then
                            dgv("ElementName", MaxRow).ToolTipText = String.Format("{0} - {1}", auxString, myPatientName)
                        Else
                            dgv("ElementName", MaxRow).ToolTipText = auxString
                        End If
                    End If
                End If
            Else
                dgv("ElementName", MaxRow).ToolTipText = String.Empty
            End If

            'SampleTye column
            If pNewRow.IsHeader Then
                dgv("SampleType", MaxRow).Style.Font = HeaderFont 'SampleType
                If pNewRow.SampleClass = "PATIENT" Then
                    auxString = pNewRow.SampleType
                Else
                    'BLANKS, CALIB, CTRLS has not sample type in header
                    auxString = String.Empty
                End If
            Else
                dgv("SampleType", MaxRow).Style.Font = RowFont
                If pNewRow.SampleClass <> "BLANK" AndAlso pNewRow.TestType <> "CALC" Then
                    auxString = pNewRow.SampleType
                Else
                    'BLANKS has not sample type in header
                    auxString = String.Empty
                End If
            End If
            dgv("SampleType", MaxRow).Value = auxString

            'Alarms column
            If pNewRow.IsHeader Then 'Initialize Header with no alarms. This column is updated with their children
                dgv("Alarms", HeaderIndex).Value = NoImage
                dgv("Alarms", HeaderIndex).ToolTipText = String.Empty

            Else 'Child 
                If CurrentRowColor = RowGreen OrElse CurrentRowColor = HeaderGreen Then 'Finished

                    Dim myExecutionID As Integer = -1
                    Dim myRerun As Integer = -1
                    If Not pNewRow.IsExecutionIDNull Then myExecutionID = pNewRow.ExecutionID 'This column is NULL for CALC/OFFS
                    If Not pNewRow.IsRerunNumberNull Then myRerun = pNewRow.RerunNumber 'This column is NULL for CALC/OFFS

                    If IsExecutionWithAlarms(myExecutionID, pNewRow.OrderTestID, pNewRow.TestType, myRerun) Then
                        dgv("Alarms", MaxRow).Value = FinishedWithErrorsImage
                        dgv("Alarms", MaxRow).ToolTipText = labelFinishedRemarks '"*Finished With Remarks*"
                    ElseIf Not pNewRow.IsExecutionStatusNull AndAlso pNewRow.ExecutionStatus = "CLOSEDNOK" Then
                        dgv("Alarms", MaxRow).Value = FinishedWithErrorsImage
                        dgv("Alarms", MaxRow).ToolTipText = labelFinishedOpticalKO  '"*Finished With Optical Errors*"
                    ElseIf Not pNewRow.IsExecutionStatusNull AndAlso pNewRow.ExecutionStatus = "LOCKED" Then
                        dgv("Alarms", MaxRow).Value = LockedImage
                        dgv("Alarms", MaxRow).ToolTipText = labelLocked 'Locked
                    Else
                        If (Not dgv("Alarms", MaxRow).Value.Equals(FinishedWithErrorsImage)) AndAlso (Not dgv("Alarms", MaxRow).Value.Equals(LockedImage)) Then
                            'Change to avoid memory issues - do not use icons if not necessary
                            'dgv("Alarms", MaxRow).Value = FinishedImage
                            'dgv("Alarms", MaxRow).ToolTipText = labelSamplePosStatusFinished '*Finished*"
                        End If
                    End If

                ElseIf Not pNewRow.IsExecutionStatusNull AndAlso pNewRow.ExecutionStatus <> "LOCKED" Then 'Evaluate using the replicate 1 ExecutionStatus
                    dgv("Alarms", MaxRow).Value = NoImage
                    dgv("Alarms", MaxRow).ToolTipText = String.Empty

                ElseIf Not pNewRow.IsExecutionStatusNull AndAlso pNewRow.ExecutionStatus = "LOCKED" Then 'Evaluate using the replicate 1 ExecutionStatus
                    dgv("Alarms", MaxRow).Value = LockedImage
                    dgv("Alarms", MaxRow).ToolTipText = labelLocked
                End If

                'AG 24/03/2014 - #1550 New business - 'AG 12/03/2014 - #1524 Calculate value for header column
                Select Case dgv("Alarms", MaxRow).ToolTipText
                    Case labelLocked 'child LOCKED
                        'All childs lock -> Header shows lock icon
                        If currentHeaderAlarmsIcon <> AlarmsIconValues.With_OpticalErrors AndAlso currentHeaderAlarmsIcon <> AlarmsIconValues.With_Remarks _
                            AndAlso currentHeaderAlarmsIcon <> AlarmsIconValues.Finished_OK Then
                            currentHeaderAlarmsIcon = AlarmsIconValues.Locked
                        End If

                    Case labelFinishedOpticalKO 'child CLOSED NOK
                        'At least 1 child finished with optical errors then header shows warning icon
                        currentHeaderAlarmsIcon = AlarmsIconValues.With_OpticalErrors

                    Case labelFinishedRemarks 'child CLOSED OK with remarks
                        'If there is not child with optical error -> header shows with Remarks
                        If currentHeaderAlarmsIcon <> AlarmsIconValues.With_OpticalErrors Then
                            currentHeaderAlarmsIcon = AlarmsIconValues.With_Remarks
                        End If


                    Case labelSamplePosStatusFinished 'child CLOSED OK withour remarks
                        'All finished children with no optical errors neither remarks
                        If currentHeaderAlarmsIcon <> AlarmsIconValues.With_Remarks AndAlso currentHeaderAlarmsIcon <> AlarmsIconValues.With_OpticalErrors Then
                            currentHeaderAlarmsIcon = AlarmsIconValues.Finished_OK
                        End If

                    Case String.Empty
                        If currentHeaderAlarmsIcon = AlarmsIconValues.Locked Then currentHeaderAlarmsIcon = AlarmsIconValues.None
                End Select
                'AG 24/03/2014 - #1550 New business 'AG 12/03/2014 - #1524

            End If


            'AG 12/03/2014 - #1524 - PrintAvailable
            If pNewRow.SampleClass = "PATIENT" Then
                If pNewRow.IsHeader Then
                    'Initialize empty, it will be calculated using their children
                    dgv("PrintAvailable", MaxRow).Value = NoImage
                    dgv("PrintAvailable", MaxRow).ToolTipText = labelReportPrintNOTAvailable
                    currentHeaderShowPrintedIconFlag = False

                Else
                    If pNewRow.IsPrintedNull Then pNewRow.Printed = False 'AG 19/03/2014 - Protection against DBNULL
                    If CurrentRowColor = RowGreen AndAlso Not pNewRow.Printed Then
                        dgv("PrintAvailable", MaxRow).Value = PrintAvailableImage
                        dgv("PrintAvailable", MaxRow).ToolTipText = labelReportPrintAvailable
                        currentHeaderShowPrintedIconFlag = True 'There is at least one child available to print

                    Else
                        dgv("PrintAvailable", MaxRow).Value = NoImage
                        dgv("PrintAvailable", MaxRow).ToolTipText = labelReportPrintNOTAvailable
                    End If
                End If

            Else 'Blanks, Calibs, Ctrols do not show available print icon
                currentHeaderShowPrintedIconFlag = False
            End If

            'Export icons columns
            If pNewRow.SampleClass = "PATIENT" OrElse pNewRow.SampleClass = "CTRL" Then
                If pNewRow.IsHeader Then
                    'Initialize empty, it will be calculated using their children
                    dgv("Exported", MaxRow).Value = NoImage
                    dgv("Exported", MaxRow).ToolTipText = labelHISNOTSent
                    currentHeaderShowExportIconFlag = False

                Else
                    If CurrentRowColor = RowGreen AndAlso pNewRow.ExportStatus = "SENT" Then
                        dgv("Exported", MaxRow).Value = HISSentImage
                        dgv("Exported", MaxRow).ToolTipText = labelHISSent
                        currentHeaderShowExportIconFlag = True 'There is at least one child sent to LIS

                    Else
                        dgv("Exported", MaxRow).Value = NoImage
                        dgv("Exported", MaxRow).ToolTipText = labelHISNOTSent
                    End If
                End If

            Else 'Blanks, Calibs do not show the sent to LIS icon
                currentHeaderShowExportIconFlag = False
            End If
            'AG 12/03/2014 - #1524

            'Absorbance time graph
            If pNewRow.TestType = "STD" Then
                dgv("Graph", MaxRow).Value = GraphImage
                dgv("Graph", MaxRow).ToolTipText = labelAbsorbanceCurve '"*Open Absorbance Curve*"
            Else
                dgv("Graph", MaxRow).Value = NoImage
                dgv("Graph", MaxRow).ToolTipText = String.Empty
            End If

            auxString = String.Empty

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DrawNewRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DrawNewRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub


    ''' <summary>
    ''' Draw a updated/refreshed row
    ''' </summary>
    ''' <param name="dgv"></param>
    ''' <param name="pRefreshRow"></param>
    ''' <param name="indexRow"></param>
    ''' <remarks>AG 28/02/2014 - Creation #1524
    ''' AG 24/03/2014 - #1550 new business for alarms icon</remarks>
    Private Sub DrawRefreshRow(ByVal dgv As BSDataGridView, ByVal pRefreshRow As ExecutionsDS.vwksWSExecutionsMonitorRow, ByVal indexRow As Integer)
        Try
            Dim auxString As String = String.Empty

            CurrentRowColor = AssignRowColor(pRefreshRow)

            'I refresh mode the headers rows color is set once all his children has been evaluated -Except for green color
            If Not pRefreshRow.IsHeader OrElse CurrentRowColor = HeaderGreen Then SetRowColor(indexRow, CurrentRowColor)

            'Collapse column - Not refresh required
            'StatFlag Image column (only header) - Not refresh required

            'Paused flag image column - Same as creation mode
            If CurrentRowColor = RowYellow OrElse CurrentRowColor = HeaderYellow Then
                If pRefreshRow.SampleClass = "PATIENT" And (pRefreshRow.TestType = "STD" Or pRefreshRow.TestType = "ISE") Then
                    If Not pRefreshRow.IsPausedNull AndAlso pRefreshRow.Paused Then 'First replicate
                        dgv("PausedFlag", indexRow).Value = PausedImage
                        dgv("PausedFlag", indexRow).ToolTipText = labelWSActionResume

                    Else
                        dgv("PausedFlag", indexRow).Value = PlayImage
                        dgv("PausedFlag", indexRow).ToolTipText = labelWSActionPause
                    End If
                End If
            End If

            'ElementName column - No refresh required

            'ElementName tooltip column - Same as creation mode
            If CurrentRowColor = RowGreen OrElse CurrentRowColor = HeaderGreen Then
                dgv("ElementName", indexRow).ToolTipText = labelTitleResult

                If pRefreshRow.SampleClass = "PATIENT" Then
                    If Not pRefreshRow.IsElementNameNull Then auxString = pRefreshRow.ElementName
                    If Not pRefreshRow.IsSpecimenIDListNull Then
                        dgv("ElementName", indexRow).ToolTipText = String.Format("{0} ({1}) - {2}", pRefreshRow.SpecimenIDList, auxString, pRefreshRow.PatientName)
                    Else
                        dgv("ElementName", indexRow).ToolTipText = String.Format("{0} - {1}", auxString, pRefreshRow.PatientName)
                    End If
                End If
            Else
                dgv("ElementName", indexRow).ToolTipText = String.Empty
            End If

            'SampleTye column - No refresh required

            'Alarms column - Same as creation mode
            If pRefreshRow.IsHeader Then 'Initialize Header with no alarms. This column is updated with their children
                dgv("Alarms", HeaderIndex).Value = NoImage
                dgv("Alarms", HeaderIndex).ToolTipText = String.Empty
                CurrentHeaderAlarmsIcon = AlarmsIconValues.None 'AG 12/03/2014 - #1524

            Else 'Child 
                If CurrentRowColor = RowGreen OrElse CurrentRowColor = HeaderGreen Then 'Finished
                    Dim myExecutionID As Integer = -1
                    Dim myRerun As Integer = -1
                    If Not pRefreshRow.IsExecutionIDNull Then myExecutionID = pRefreshRow.ExecutionID 'This column is NULL for CALC/OFFS
                    If Not pRefreshRow.IsRerunNumberNull Then myRerun = pRefreshRow.RerunNumber 'This column is NULL for CALC/OFFS

                    If IsExecutionWithAlarms(myExecutionID, pRefreshRow.OrderTestID, pRefreshRow.TestType, myRerun) Then
                        dgv("Alarms", indexRow).Value = FinishedWithErrorsImage
                        dgv("Alarms", indexRow).ToolTipText = labelFinishedRemarks '"*Finished With Remarks*"
                    ElseIf Not pRefreshRow.IsExecutionStatusNull AndAlso pRefreshRow.ExecutionStatus = "CLOSEDNOK" Then
                        dgv("Alarms", indexRow).Value = FinishedWithErrorsImage
                        dgv("Alarms", indexRow).ToolTipText = labelFinishedOpticalKO  '"*Finished With Optical Errors*"
                    ElseIf Not pRefreshRow.IsExecutionStatusNull AndAlso pRefreshRow.ExecutionStatus = "LOCKED" Then
                        dgv("Alarms", indexRow).Value = LockedImage
                        dgv("Alarms", indexRow).ToolTipText = labelLocked 'Locked
                    Else
                        If (Not dgv("Alarms", indexRow).Value.Equals(FinishedWithErrorsImage)) AndAlso (Not dgv("Alarms", indexRow).Value.Equals(LockedImage)) Then
                            'AG 12/03/2014 - Change to avoid memory issues - do not use icons if not necessary
                            'dgv("Alarms", indexRow).Value = FinishedImage
                            'dgv("Alarms", indexRow).ToolTipText = labelSamplePosStatusFinished '*Finished*"
                        End If
                    End If

                ElseIf Not pRefreshRow.IsExecutionStatusNull AndAlso pRefreshRow.ExecutionStatus <> "LOCKED" Then 'Evaluate using the replicate 1 ExecutionStatus
                    dgv("Alarms", indexRow).Value = NoImage
                    dgv("Alarms", indexRow).ToolTipText = String.Empty

                ElseIf Not pRefreshRow.IsExecutionStatusNull AndAlso pRefreshRow.ExecutionStatus = "LOCKED" Then 'Evaluate using the replicate 1 ExecutionStatus
                    dgv("Alarms", indexRow).Value = LockedImage
                    dgv("Alarms", indexRow).ToolTipText = labelLocked
                End If

                'AG 24/03/2014 - #1550 New business - 'AG 12/03/2014 - #1524 Calculate value for header column
                Select Case dgv("Alarms", indexRow).ToolTipText
                    Case labelLocked 'child LOCKED
                        'All childs lock -> Header shows lock icon
                        If currentHeaderAlarmsIcon <> AlarmsIconValues.With_OpticalErrors AndAlso currentHeaderAlarmsIcon <> AlarmsIconValues.With_Remarks _
                            AndAlso currentHeaderAlarmsIcon <> AlarmsIconValues.Finished_OK Then
                            currentHeaderAlarmsIcon = AlarmsIconValues.Locked
                        End If

                    Case labelFinishedOpticalKO 'child CLOSED NOK
                        'At least 1 child finished with optical errors then header shows warning icon
                        currentHeaderAlarmsIcon = AlarmsIconValues.With_OpticalErrors

                    Case labelFinishedRemarks 'child CLOSED OK with remarks
                        'If there is not child with optical error -> header shows with Remarks
                        If currentHeaderAlarmsIcon <> AlarmsIconValues.With_OpticalErrors Then
                            currentHeaderAlarmsIcon = AlarmsIconValues.With_Remarks
                        End If


                    Case labelSamplePosStatusFinished 'child CLOSED OK withour remarks
                        'All finished children with no optical errors neither remarks
                        If currentHeaderAlarmsIcon <> AlarmsIconValues.With_Remarks AndAlso currentHeaderAlarmsIcon <> AlarmsIconValues.With_OpticalErrors Then
                            currentHeaderAlarmsIcon = AlarmsIconValues.Finished_OK
                        End If


                    Case String.Empty
                        If currentHeaderAlarmsIcon = AlarmsIconValues.Locked Then currentHeaderAlarmsIcon = AlarmsIconValues.None

                End Select
                'AG 24/03/2014 - #1550 New business 'AG 12/03/2014 - #1524

            End If

            'AG 12/03/2014 - #1524 - PrintAvailable
            If pRefreshRow.SampleClass = "PATIENT" Then
                If pRefreshRow.IsHeader Then
                    'Initialize empty, it will be calculated using their children
                    dgv("PrintAvailable", indexRow).Value = NoImage
                    dgv("PrintAvailable", indexRow).ToolTipText = labelReportPrintNOTAvailable
                    currentHeaderShowPrintedIconFlag = False

                Else
                    If pRefreshRow.IsPrintedNull Then pRefreshRow.Printed = False 'AG 19/03/2014 - Protection against DBNULL
                    If CurrentRowColor = RowGreen AndAlso Not pRefreshRow.Printed Then
                        dgv("PrintAvailable", indexRow).Value = PrintAvailableImage
                        dgv("PrintAvailable", indexRow).ToolTipText = labelReportPrintAvailable
                        currentHeaderShowPrintedIconFlag = True 'There is at least one child available to print

                    Else
                        dgv("PrintAvailable", indexRow).Value = NoImage
                        dgv("PrintAvailable", indexRow).ToolTipText = labelReportPrintNOTAvailable
                    End If
                End If

            Else 'Blanks, Calibs, Ctrols do not show available print icon
                currentHeaderShowPrintedIconFlag = False
            End If

            'Export icons columns
            If pRefreshRow.SampleClass = "PATIENT" OrElse pRefreshRow.SampleClass = "CTRL" Then
                If pRefreshRow.IsHeader Then
                    'Initialize empty, it will be calculated using their children
                    dgv("Exported", indexRow).Value = NoImage
                    dgv("Exported", indexRow).ToolTipText = labelHISNOTSent
                    currentHeaderShowExportIconFlag = False

                Else
                    If CurrentRowColor = RowGreen AndAlso pRefreshRow.ExportStatus = "SENT" Then
                        dgv("Exported", indexRow).Value = HISSentImage
                        dgv("Exported", indexRow).ToolTipText = labelHISSent
                        currentHeaderShowExportIconFlag = True 'There is at least one child sent to LIS

                    Else
                        dgv("Exported", indexRow).Value = NoImage
                        dgv("Exported", indexRow).ToolTipText = labelHISNOTSent
                    End If
                End If

            Else 'Blanks, Calibs do not show the sent to LIS icon
                currentHeaderShowExportIconFlag = False
            End If
            'AG 12/03/2014 - #1524

            'Absorbance time graph - No refresh required

            auxString = String.Empty

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DrawRefreshRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DrawRefreshRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub


    ''' <summary>
    ''' Updates the current header with values calculated after evaluate all their children
    ''' - Header row color
    ''' - Header alarms icon
    ''' - Header LIS icon -> Header finished (green) and at least 1 child sent to LIS
    ''' - Header FinalPrint icon -> Header finished (green) and at least 1 child available to print
    ''' </summary>
    ''' <remarks>AG 12/03/2014 - creation #1524</remarks>
    Private Sub DrawCurrentHeaderRow()
        Try
            'Get the control grid
            Dim dgv As BSDataGridView = bsWSExecutionsDataGridView

            'Set header color with the row color calculated to the last Header (if different)
            If bsWSExecutionsDataGridView.Rows(HeaderIndex).DefaultCellStyle.BackColor <> CurrentHeaderColor Then
                SetRowColor(HeaderIndex, CurrentHeaderColor)
            End If

            'Set header alarms icon
            Select Case CurrentHeaderAlarmsIcon
                Case AlarmsIconValues.Locked
                    dgv("Alarms", HeaderIndex).Value = LockedImage
                    dgv("Alarms", HeaderIndex).ToolTipText = labelLocked

                Case AlarmsIconValues.With_OpticalErrors
                    dgv("Alarms", HeaderIndex).Value = FinishedWithErrorsImage
                    dgv("Alarms", HeaderIndex).ToolTipText = labelFinishedOpticalKO

                Case AlarmsIconValues.With_Remarks
                    dgv("Alarms", HeaderIndex).Value = FinishedWithErrorsImage
                    dgv("Alarms", HeaderIndex).ToolTipText = labelFinishedRemarks

                    'Change to avoid memory issues - do not use icons if not necessary
                    'Case AlarmsIconValues.Finished_OK
                    '    dgv("Alarms", HeaderIndex).Value = FinishedImage
                    '    dgv("Alarms", HeaderIndex).ToolTipText = labelSamplePosStatusFinished

                Case AlarmsIconValues.Finished_OK, AlarmsIconValues.None
                    dgv("Alarms", HeaderIndex).Value = NoImage
                    dgv("Alarms", HeaderIndex).ToolTipText = String.Empty
            End Select

            'Set header LIS icon
            'If order is CLOSED and at least one child sent to LIS ... header shows icon too
            If currentHeaderShowExportIconFlag AndAlso CurrentHeaderColor = HeaderGreen Then
                dgv("Exported", HeaderIndex).Value = HISSentImage
                dgv("Exported", HeaderIndex).ToolTipText = labelHISSent
            End If

            'Set header final PRINT available icon
            If currentHeaderShowPrintedIconFlag AndAlso CurrentHeaderColor = HeaderGreen Then
                dgv("PrintAvailable", HeaderIndex).Value = PrintAvailableImage
                dgv("PrintAvailable", HeaderIndex).ToolTipText = labelReportPrintAvailable
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DrawCurrentHeaderRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DrawCurrentHeaderRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub

    ''' <summary>
    ''' Evaluate the color that must be assigned to the current row
    ''' HEADER:
    '''     Green -> Closed or All children are (closed or pending or locked)
    '''     Orange -> One child is inprocess
    '''     Yellow -> All child are pending or locked
    ''' CHILD:
    '''     Green -> Closed or all replicated closed or locked
    '''     Orange -> One replicate inprocess
    '''     Yellow -> All replicates pending or locked
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <returns></returns>
    ''' <remarks>AG 28/02/2014 - Creation #1524</remarks>
    Private Function AssignRowColor(ByVal pRow As ExecutionsDS.vwksWSExecutionsMonitorRow) As System.Drawing.Color
        Dim value As System.Drawing.Color = RowYellow
        Try

            'HEADERs ROWs
            If pRow.IsHeader Then

                'pRow is the new header and at this moment HeaderIndex points to the LAST Header in grid update color is required
                If HeaderIndex <> -1 Then
                    DrawCurrentHeaderRow()
                    CurrentHeaderAlarmsIcon = AlarmsIconValues.None
                End If

                'Start evaluate the new header
                If Not pRow.IsIndexNull AndAlso HeaderIndex <> pRow.Index Then HeaderIndex = pRow.Index 'Set the index (in grid and in myWSGridDS) of the last header
                value = HeaderYellow 'Initialize header row color as yellow!!
                If Not pRow.IsOrderStatusNull AndAlso pRow.OrderStatus = "CLOSED" Then value = HeaderGreen 'CLOSED!!! green
                CurrentHeaderColor = value 'Set the current header

                'children ROWs
            Else
                Dim doQueryFlag As Boolean = False 'Value is set to TRUE when Sw must execute query for evaluate color
                If Not pRow.IsOrderTestStatusNull AndAlso pRow.OrderTestStatus = "CLOSED" Then
                    value = RowGreen 'CLOSED!!! green

                ElseIf (pRow.TestType = "STD" OrElse pRow.TestType = "ISE") AndAlso Not pRow.IsExecutionStatusNull Then 'Evaluate using the replicate 1 ExecutionStatus
                    Select Case pRow.ExecutionStatus
                        Case "INPROCESS" '1st replicate INPROCESS
                            value = RowOrange

                        Case "PENDING", "LOCKED" '1st replicate PENDING/ LOCKED (but case LOCEKD:: STD tests with auto predilution could have 1st replicate locked but inprocess the others
                            '                    in this case the row will change from Yellow -> Green when the OT is closed)
                            value = RowYellow
                            If pRow.ExecutionStatus = "LOCKED" Then doQueryFlag = True

                        Case Else '1st replicate CLOSED but the OT is not closed -> it could mean: not all replicates closed or reruns added
                            doQueryFlag = True
                    End Select
                ElseIf (pRow.TestType = "CALC" OrElse pRow.TestType = "OFFS") Then
                    'CALC/OFFS tests: Not apply (YELLOW or GREEN)
                    value = RowYellow
                End If

                If doQueryFlag Then
                    'STD or ISE tests: read all executions by OT - Rerun and evaluate color
                    Dim exDlg As New ExecutionsDelegate
                    Dim resultData As GlobalDataTO
                    resultData = exDlg.GetByOrderTest(Nothing, WorkSessionIDField, AnalyzerIDField, pRow.OrderTestID)
                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        Dim totalReplicates As Integer = 0
                        Dim counter As Integer = 0

                        'Get all replicates number
                        totalReplicates = (From a As ExecutionsDS.twksWSExecutionsRow In DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions _
                                           Where a.OrderTestID = pRow.OrderTestID AndAlso a.RerunNumber = pRow.RerunNumber Select a).Count

                        'all pending/locked -> yellow color
                        counter = (From a As ExecutionsDS.twksWSExecutionsRow In DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions _
                                   Where a.OrderTestID = pRow.OrderTestID AndAlso a.RerunNumber = pRow.RerunNumber _
                                   AndAlso (a.ExecutionStatus = "PENDING" OrElse a.ExecutionStatus = "LOCKED") Select a).Count
                        If counter = totalReplicates Then
                            value = RowYellow 'set color only for current row. Not for header!!! (1 test pending/locked does not mean header color)
                        Else
                            'one inprocess -> orange color
                            counter = (From a As ExecutionsDS.twksWSExecutionsRow In DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions _
                                       Where a.OrderTestID = pRow.OrderTestID AndAlso a.RerunNumber = pRow.RerunNumber _
                                       AndAlso a.ExecutionStatus = "INPROCESS" Select a).Count
                            If counter > 0 Then
                                value = RowOrange
                            Else
                                'all (closed/closedNOK+ pending/locked) -> green color
                                value = RowGreen 'set color only for current row. Not for header!!! (1 test pending/locked does not mean header color)
                            End If
                        End If

                    ElseIf Not pRow.IsIndexNull Then
                        'Keep the same
                        value = bsWSExecutionsDataGridView.Rows(pRow.Index).DefaultCellStyle.BackColor
                    End If
                    resultData.SetDatos = Nothing
                End If

                'AG 12/03/2014 - Calculate the color for the header row depending the child color
                Select Case value
                    Case RowOrange
                        CurrentHeaderColor = HeaderOrange 'At least 1 child in process the header is inprocess
                    Case RowGreen
                        If CurrentHeaderColor <> HeaderOrange Then CurrentHeaderColor = HeaderGreen 'If no children in process then set green color
                    Case RowYellow
                        'No changes, keeps the initial header row
                End Select
                'AG 12/03/2014

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".AssignRowColor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".AssignRowColor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

            If pRow.IsHeader Then value = HeaderYellow Else value = RowYellow
        End Try
        Return value
    End Function


    ''' <summary>
    ''' Calculate WS statitics #TestsNumber (Total, Closed, InProcess, Pending. Locked, with Alarms 'ClosedNok' )
    ''' For STD / ISE tests count the table twksWSExecutions
    ''' For CALC / OFFS count the table twksOrderTests (not direct, they have special business)
    ''' </summary>
    ''' <remarks>AG 03/03/2014 - #1524</remarks>
    Private Sub CalculateStatisticsCharts()
        Try

            'Statistics using executions (STD & ISE tests)
            Dim execDlg As New ExecutionsDelegate
            Dim myGlobal As New GlobalDataTO

            Dim stadisticStatusExecutions() As String = {"CLOSEDNOK", "LOCKED", "PENDING", "INPROCESS", "CLOSED", ""} 'The status "" is renamed for statistics as "TOTAL"

            For Each status As String In stadisticStatusExecutions
                myGlobal = execDlg.CountByExecutionStatus(Nothing, AnalyzerIDField, WorkSessionIDField, status)
                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                    If status = "" Then status = "TOTAL"
                    Statistics(0)(status) = DirectCast(myGlobal.SetDatos, Integer)
                End If
            Next
            Erase stadisticStatusExecutions

            'Statistics adding tests with no executions (CALC & OFFS tests)
            Dim otDlg As New OrderTestsDelegate
            Dim statiticsOrderTestsStatus() As String = {"PENDING", "CLOSED", ""} 'The status "" is renamed for statistics as "TOTAL"
            'inProcess: Add Count orderTestID with (TestType = 'CALC' and OrderStatus = 'PENDING') or (TestType = 'OFFS' and OrderStatus = 'OPEN')
            'Total: Add Count orderTestID with TestType = 'CALC' or 'OFFS'
            'Closed: Add Count orderTestID with TestType = 'CALC' or 'OFFS' and OrderStatus = 'CLOSED'

            'InProcess: For OFFS not apply, for CALC is difficult (some of his component tests has a replicate inprocess) -> By now NOT APPLY
            'Locked: For OFFS not apply, for CALC is difficult (some of his component tests has all replicates locked) -> By now NOT APPLY
            'Alarms: For OFFS, CALC not apply

            Dim currentValue As Integer
            For Each status As String In statiticsOrderTestsStatus
                myGlobal = otDlg.CountByOrderTestStatus(Nothing, AnalyzerIDField, WorkSessionIDField, status, True)
                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                    If status = "" Then status = "TOTAL"
                    currentValue = CInt(Statistics(0)(status))
                    Statistics(0)(status) = currentValue + DirectCast(myGlobal.SetDatos, Integer)
                End If
            Next
            Erase statiticsOrderTestsStatus



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CalculateStatisticsCharts", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CalculateStatisticsCharts", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub


    ''' <summary>
    ''' Fills the grid with details of the created WorkSession Executions for monitoring
    ''' Method has 2 modes: CreateGrid and rows used when the screen is loaded
    '''                     Refresh mode: No rows are added, only change colors and icons - used in communications events
    ''' </summary>
    ''' <param name="CreateDGWRows">Tells if new rows have to be added or just make it visible</param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' Created by:  RH 29/11/2010
    ''' Modified by: RH 05/10/2011 New algorithm implementation with new rules. Flow simplification.
    ''' AG 06/06/2012 - Improve execution speed wide work sessions case few patients lots of replicates (50 patients, 1 test with 50 replicates)
    ''' AG 13/06/2012 - Improve execution speed wide work sessions case lots patients few replicates (2000 patients, 1 replicate)
    '''                 Declare optional parameter pRefreshDS
    ''' AG 14/06/2013 - when exists shows the barcode value. Between brackets
    ''' AG 01/10/2013 - Bug #1305 and also fix several issues related with the screen refresh using calculated tests (1306, 1307 and 1275)
    ''' AG 28/02/2014 - #1524
    ''' </remarks>
    Private Sub UpdateExecutionsDataGridView(ByVal CreateDGWRows As Boolean, ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            'Dim StartTime As DateTime = Now
            Dim dgv As BSDataGridView = bsWSExecutionsDataGridView

            'Initialize variables!!
            MaxRow = -1 'MaxRow is used in mode creation 'dgv.Rows.Count - 1
            CurrentRowColor = RowYellow
            HeaderIndex = -1 'Header position for the current child test (creation or update)
            CurrentHeaderColor = HeaderYellow
            CurrentHeaderAlarmsIcon = AlarmsIconValues.None

            'Statistics
            InitializeStatisticsTable()
            If myWSGridDS.vwksWSExecutionsMonitor.Rows.Count > 0 Then
                CalculateStatisticsCharts()
                UpdateStatisticsCharts()
            Else
                ResetStatisticsCharts()
            End If

            'If mode CreateDGWRows or fake refresh then create grid!!!
            If CreateDGWRows OrElse (Not pRefreshDS Is Nothing AndAlso pRefreshDS.ExecutionStatusChanged.Rows.Count = 0) Then
                If myWSGridDS.vwksWSExecutionsMonitor.Rows.Count > 0 Then
                    dgv.Rows.Clear()
                    'dgv.DataSource = myWSGridDS.vwksWSExecutionsMonitor
                    For Each wsGridRow As ExecutionsDS.vwksWSExecutionsMonitorRow In myWSGridDS.vwksWSExecutionsMonitor
                        'Add and initialize row into grid
                        dgv.Rows.Add()
                        MaxRow = dgv.Rows.Count - 1
                        'MaxRow += 1
                        InitWSExecutionsDataGridRow(MaxRow) 'Initializes the new row in grid
                        DrawNewRow(dgv, wsGridRow) 'Fill it
                        dgv.Rows(MaxRow).Visible = True
                    Next

                    'At this moment HeaderIndex points to the last Header in grid
                    If HeaderIndex <> -1 Then
                        DrawCurrentHeaderRow()

                        'After paint last header initialize values for next
                        HeaderIndex = -1
                        CurrentHeaderColor = HeaderYellow
                        CurrentHeaderAlarmsIcon = AlarmsIconValues.None
                    End If
                End If

            Else
                'Refresh mode
                Dim myRefreshRowsAffected As New List(Of Integer) 'Grid rows affected for refresh (IMPORTANT it must be sorted ASC by column Index!!!!!)
                Dim affectedGridRows As New List(Of ExecutionsDS.vwksWSExecutionsMonitorRow)
                Dim orderIDsWithCALCTestsAffected As New List(Of String) 'When a OrderTests changes to CLOSED it is possible that the calculated tests of this patient were already calculated!!!
                Dim otDlg As New OrderTestsDelegate
                Dim resultData As GlobalDataTO

                Dim childPosition As Integer = -1
                Dim headerPosition As Integer = -1

                'Loop the refresh DS data and update the local screen DS and prepare data for refresh the grid
                For Each refreshRow As UIRefreshDS.ExecutionStatusChangedRow In pRefreshDS.ExecutionStatusChanged
                    '1- Search the CHILD test row in the DS
                    affectedGridRows = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In myWSGridDS.vwksWSExecutionsMonitor _
                               Where a.OrderTestID = refreshRow.OrderTestID AndAlso a.RerunNumber = refreshRow.RerunNumber AndAlso _
                               a.IsHeader = False Select a).ToList

                    '2- If found update his status values!!! (only 1 row can be matched by OrderTest - Rerun)
                    If affectedGridRows.Count > 0 Then
                        childPosition = affectedGridRows(0).Index
                        With myWSGridDS.vwksWSExecutionsMonitor(childPosition)
                            .BeginEdit()
                            If Not .IsExecutionStatusNull AndAlso Not refreshRow.IsExecutionStatusNull AndAlso .ExecutionStatus <> refreshRow.ExecutionStatus Then .ExecutionStatus = refreshRow.ExecutionStatus
                            If Not .IsOrderTestStatusNull AndAlso Not refreshRow.IsOrderTestStatusNull AndAlso .OrderTestStatus <> refreshRow.OrderTestStatus Then .OrderTestStatus = refreshRow.OrderTestStatus
                            If Not .IsOrderStatusNull AndAlso Not refreshRow.IsOrderStatusNull AndAlso .OrderStatus <> refreshRow.OrderStatus Then .OrderStatus = refreshRow.OrderStatus
                            .EndEdit()
                            .AcceptChanges()

                            'Inform the OrderID list which calculated tests STATUS could have change to CLOSED
                            If Not refreshRow.IsOrderTestStatusNull AndAlso refreshRow.OrderTestStatus = "CLOSED" AndAlso _
                                Not refreshRow.IsOrderIDNull AndAlso Not orderIDsWithCALCTestsAffected.Contains(refreshRow.OrderID) Then
                                orderIDsWithCALCTestsAffected.Add(refreshRow.OrderID)
                            End If
                        End With


                        '3- Then search the HEADER row and update value (OrderStatus) if required  (note that the HEADER is the nearest isHeader = TRUE but with index lower)
                        affectedGridRows = _
                            (From row As ExecutionsDS.vwksWSExecutionsMonitorRow In myWSGridDS.vwksWSExecutionsMonitor _
                             Where row.OrderID = refreshRow.OrderID AndAlso _
                             row.IsHeader = True AndAlso _
                             row.SampleType = row.SampleType AndAlso _
                             row.Index < affectedGridRows(0).Index _
                             Select row).ToList()

                        If affectedGridRows.Count > 0 Then
                            headerPosition = affectedGridRows(affectedGridRows.Count - 1).Index
                            With myWSGridDS.vwksWSExecutionsMonitor(headerPosition)
                                .BeginEdit()
                                If Not .IsOrderStatusNull AndAlso Not refreshRow.IsOrderStatusNull AndAlso .OrderStatus <> refreshRow.OrderStatus Then .OrderStatus = refreshRow.OrderStatus
                                .EndEdit()
                                .AcceptChanges()
                            End With

                            '4- Prepare the grid rows to be updated (header and required children)
                            'If ExecutionStatus = INPROCESS then refresh only the affected child tests row in pRefreshDS and his header (1 execution inprocess means header inprocess)
                            'Else: We must send to refresh the header and all his children (because the header is calculated using all their children)
                            '- ExecutionStatus PENDING/LOCKED/CLOSED: 1 execution pending/locked/closed does not mean header pending/locked/closed
                            '- OrderTestStatus CLOSED: 1 test closed does not mean test/header GREEN (besides when autorerun is launch the ordertest continues INPROCESS)
                            '- OrderStatus <> CLOSED: does not mean header is not GREEN (automatic reruns)

                            If Not myRefreshRowsAffected.Contains(headerPosition) Then myRefreshRowsAffected.Add(headerPosition) 'Add header into list to refresh
                            If Not refreshRow.IsExecutionStatusNull AndAlso refreshRow.ExecutionStatus = "INPROCESS" Then
                                If Not myRefreshRowsAffected.Contains(childPosition) Then myRefreshRowsAffected.Add(childPosition) 'Add child into list to refresh
                            Else
                                'Add ALL children tests into list to refresh
                                For i As Integer = headerPosition + 1 To myWSGridDS.vwksWSExecutionsMonitor.Count - 1
                                    If Not myWSGridDS.vwksWSExecutionsMonitor(i).IsHeader Then
                                        If Not myRefreshRowsAffected.Contains(i) Then
                                            myRefreshRowsAffected.Add(i)

                                            'If the orderId has been marked as possible changes into CALC tests then read the current CALC test status
                                            'and update myWSGridDS
                                            If myWSGridDS.vwksWSExecutionsMonitor(i).TestType = "CALC" _
                                                AndAlso Not myWSGridDS.vwksWSExecutionsMonitor(i).IsOrderIDNull _
                                                AndAlso orderIDsWithCALCTestsAffected.Contains(myWSGridDS.vwksWSExecutionsMonitor(i).OrderID) Then
                                                resultData = otDlg.GetOrderTest(Nothing, myWSGridDS.vwksWSExecutionsMonitor(i).OrderTestID)
                                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                                    If DirectCast(resultData.SetDatos, OrderTestsDS).twksOrderTests.Rows.Count > 0 AndAlso _
                                                        DirectCast(resultData.SetDatos, OrderTestsDS).twksOrderTests(0).OrderTestStatus = "CLOSED" Then
                                                        myWSGridDS.vwksWSExecutionsMonitor(i).BeginEdit()
                                                        myWSGridDS.vwksWSExecutionsMonitor(i).OrderTestStatus = "CLOSED"
                                                        myWSGridDS.vwksWSExecutionsMonitor(i).EndEdit()
                                                    End If
                                                End If
                                            End If
                                        End If

                                    Else
                                        'Exit loop when next header is found!!!
                                        Exit For
                                    End If
                                Next
                            End If

                        End If

                    End If
                    myWSGridDS.vwksWSExecutionsMonitor.AcceptChanges()
                Next

                'Finally call the headers and child tests rows affected for refresh in the grid
                'myRefreshRowsAffected.Sort()
                For Each rowToRefresh As Integer In myRefreshRowsAffected
                    If myWSGridDS.vwksWSExecutionsMonitor(rowToRefresh).IsHeader Then
                        HeaderIndex = -1
                        CurrentHeaderColor = HeaderYellow
                        CurrentHeaderAlarmsIcon = AlarmsIconValues.None
                    End If

                    'Refresh affected grid row
                    DrawRefreshRow(dgv, myWSGridDS.vwksWSExecutionsMonitor(rowToRefresh), rowToRefresh) 'Fill it
                Next

                'At this moment HeaderIndex points to the last Header in grid
                'If HeaderIndex <> -1 AndAlso bsWSExecutionsDataGridView.Rows(HeaderIndex).DefaultCellStyle.BackColor <> CurrentHeaderColor Then 
                If HeaderIndex <> -1 Then 'AG 14/03/2014
                    DrawCurrentHeaderRow()

                    'After paint last header initialize values for next
                    HeaderIndex = -1
                    CurrentHeaderColor = HeaderYellow
                    currentHeaderAlarmsIcon = AlarmsIconValues.None
                End If

                'Release elements
                affectedGridRows.Clear()
                myRefreshRowsAffected.Clear()
                orderIDsWithCALCTestsAffected.Clear()
                affectedGridRows = Nothing
                myRefreshRowsAffected = Nothing
                orderIDsWithCALCTestsAffected = Nothing
            End If

            ' XB 14/03/2014 - Delete Collapse functionality due System Out of memory cause errors - Task #1543
            ''Set the rows count for the Collapse Column
            'DirectCast(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = dgv.Rows.Count


            'CreateLogActivity("TIME: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Name & ".UpdateExecutionsDataGridView", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateExecutionsDataGridView", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpdateExecutionsDataGridView", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
            'Finally
            '    bsWSExecutionsDataGridView.Enabled = True
        End Try
    End Sub


    ''' <summary>
    ''' Updates the statistics gauges
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 05/04/2011 
    ''' AG 03/03/2014 - #1524 make code easier
    ''' </remarks>
    Private Sub UpdateStatisticsCharts()
        Try
            'AG 03/03/2014 - #1524
            For i As Integer = 0 To 5
                TotalTestsChart.Series("Series1").Points(i).YValues(0) = CInt(Statistics(0)(i))
            Next

            TotalTestsChart.ResetAutoValues()
            'AG 03/03/2014 
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateStatistics", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpdateStatistics", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub


    ''' <summary>
    ''' Resets (puts to zero) the statistics gauges
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 06/04/2011 
    ''' </remarks>
    Private Sub ResetStatisticsCharts()
        Try
            For i As Integer = 0 To 5
                'STDTestsChart.Series("Series1").Points(i).YValues(0) = 0
                'CALCTestsChart.Series("Series1").Points(i).YValues(0) = 0
                'ISETestsChart.Series("Series1").Points(i).YValues(0) = 0
                'OffSysTestsChart.Series("Series1").Points(i).YValues(0) = 0
                TotalTestsChart.Series("Series1").Points(i).YValues(0) = 0
            Next

            'STDTestsChart.ResetAutoValues()
            'CALCTestsChart.ResetAutoValues()
            'ISETestsChart.ResetAutoValues()
            'OffSysTestsChart.ResetAutoValues()
            TotalTestsChart.ResetAutoValues()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResetStatistics", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResetStatistics", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub

    ''' <summary>
    ''' Sets a bsWSExecutionsDataGridView row back color
    ''' </summary>
    ''' <param name="RowIndex">The Row Index to be setted</param>
    ''' <param name="pColor">The Color value for the Row back color</param>
    ''' <remarks>
    ''' Created by: RH 13/05/2011
    ''' </remarks>
    Private Sub SetRowColor(ByVal RowIndex As Integer, ByVal pColor As Color)
        Try

            If RowIndex >= 0 AndAlso RowIndex <= bsWSExecutionsDataGridView.Rows.Count - 1 Then 'DL 22/06/2012
                bsWSExecutionsDataGridView.Rows(RowIndex).DefaultCellStyle.BackColor = pColor
                bsWSExecutionsDataGridView.Rows(RowIndex).DefaultCellStyle.SelectionBackColor = pColor
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetRowColor", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetRowColor", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub

    ''' <summary>
    ''' Initializes a bsWSExecutionsDataGridView row selected fields
    ''' </summary>
    ''' <param name="RowIndex">The Row Index to be initialized</param>
    ''' <remarks>
    ''' Created by: RH 24/01/2011
    ''' </remarks>
    Private Sub InitWSExecutionsDataGridRow(ByVal RowIndex As Integer)
        Try
            Dim dgv As BSDataGridView = bsWSExecutionsDataGridView

            'dgv.Rows(RowIndex).DefaultCellStyle.BackColor = RowGreen
            'dgv.Rows(RowIndex).DefaultCellStyle.SelectionBackColor = RowGreen
            'CurrentRowColor = RowGreen

            dgv("PausedFlag", RowIndex).Value = NoImage
            dgv("PausedFlag", RowIndex).ToolTipText = String.Empty

            dgv("Alarms", RowIndex).Value = NoImage 'LockedImage
            dgv("Alarms", RowIndex).ToolTipText = String.Empty 'labelLocked ' "*Locked*"

            dgv("PrintAvailable", RowIndex).Value = NoImage
            dgv("PrintAvailable", RowIndex).ToolTipText = String.Empty

            dgv("Exported", RowIndex).Value = NoImage
            'dgv("TestType", RowIndex).Value = NoImage

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitWSExecutionsDataGridRow", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitWSExecutionsDataGridRow", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub


    ''' <summary>
    ''' Returns True if there is an error in the Execution result, False otherwise
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 03/12/2010
    ''' Modified by: RH 28/01/2011 OrderTestID and TestType parameters for recovering OffSystem test alarms
    '''              RH 26/07/2011 Recovering of CALC, STD and ISE tests alarms
    '''              SGM 22/07/2013 - Include RerunNumber
    ''' AG 28/02/2014 - #1524 do not use ExecutionID, apply link in myWSGridDS.vwksWSExecutionsAlarms using OrderTestID, RerunNumber
    ''' </remarks>
    Private Function IsExecutionWithAlarms(ByVal ExecutionID As Integer, ByVal OrderTestID As Integer, ByVal TestType As String, ByVal RerunNumber As Integer) As Boolean
        'Private Function IsExecutionWithAlarms(ByVal ExecutionID As Integer, ByVal OrderTestID As Integer, ByVal TestType As String) As Boolean
        Try
            Dim Errors As Integer = 0

            'AG 05/03/2014 - #1524 - the calculated and offs test has always rerun = 1
            If ((TestType = "CALC") OrElse (TestType = "OFFS")) Then RerunNumber = 1

            Errors = (From row In AverageResultsDS.vwksResultsAlarms _
                      Where row.OrderTestID = OrderTestID _
                      And row.RerunNumber = RerunNumber _
                      Select row).Count()

            'Add Executions Alarms for STD and ISE tests
            If Errors = 0 AndAlso ((TestType = "STD") OrElse (TestType = "ISE")) Then
                'AG 28/02/2014 - #1524
                'If (TestType = "STD") OrElse (TestType = "ISE") Then
                'Errors += (From row In myWSGridDS.vwksWSExecutionsAlarms _
                '          Where row.ExecutionID = ExecutionID _
                '          Select row).Count()
                Errors += (From row In myWSGridDS.vwksWSExecutionsAlarms _
                         Where row.OrderTestID = OrderTestID And row.RerunNumber = RerunNumber _
                          Select row).Count()

            End If

            Return (Errors > 0)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " IsExecutionWithAlarms ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
            Return False

        End Try

    End Function

    ''' <summary>
    ''' Loads all the icon images for the form
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 29/11/2010
    ''' </remarks>
    Private Sub LoadImages()
        Try
            Dim preloadedDataConfig As New PreloadedMasterDataDelegate

            Dim auxIconName As String = String.Empty
            Dim iconPath As String = IconsPath

            SampleIconList = New ImageList()

            auxIconName = GetIconName("STATS")
            If Not String.IsNullOrEmpty(auxIconName) Then
                AddIconToImageList(SampleIconList, auxIconName)
            End If

            auxIconName = GetIconName("ROUTINES")
            If Not String.IsNullOrEmpty(auxIconName) Then
                AddIconToImageList(SampleIconList, auxIconName)
            End If

            auxIconName = GetIconName("FREECELL")
            If Not String.IsNullOrEmpty(auxIconName) Then
                NoImage = preloadedDataConfig.GetIconImage("FREECELL")
                NoSatImage = preloadedDataConfig.GetIconImage("FREECELL")
                PlayImage = preloadedDataConfig.GetIconImage("FREECELL")
            End If

            auxIconName = GetIconName("ACCEPT")
            If Not String.IsNullOrEmpty(auxIconName) Then
                SatImage = preloadedDataConfig.GetIconImage("ACCEPT")
            End If

            auxIconName = GetIconName("STUS_FINISH")
            If Not String.IsNullOrEmpty(auxIconName) Then
                FinishedImage = preloadedDataConfig.GetIconImage("STUS_FINISH")
                ResultsReadyImage.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("STUS_WITHERRS")
            If Not String.IsNullOrEmpty(auxIconName) Then
                FinishedWithErrorsImage = preloadedDataConfig.GetIconImage("STUS_WITHERRS")
                ResultsWarningImage.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("STUS_LOCKED")
            If Not String.IsNullOrEmpty(auxIconName) Then
                LockedImage = preloadedDataConfig.GetIconImage("STUS_LOCKED")
                LockedTestImage.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("EXEC_PAUSED")
            If Not String.IsNullOrEmpty(auxIconName) Then
                PausedImage = preloadedDataConfig.GetIconImage("EXEC_PAUSED")
                PausePictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("PRINTL")
            If Not String.IsNullOrEmpty(auxIconName) Then
                PrintAvailableImage = preloadedDataConfig.GetIconImage("PRINTL")
                PrintPictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                FinalReportImage.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("MANUAL_EXP")
            If Not String.IsNullOrEmpty(auxIconName) Then
                HISSentImage = preloadedDataConfig.GetIconImage("MANUAL_EXP")
                HISSentPictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                ExportLISImage.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'AG 28/02/2014 - #1524 The test type images are not used
            'auxIconName = GetIconName("TESTICON") 'STD tests icon
            'If Not String.IsNullOrEmpty(auxIconName) Then
            '    STDTestIcon = preloadedDataConfig.GetIconImage("TESTICON")
            'End If

            'auxIconName = GetIconName("TCALC") 'CALC tests icon
            'If Not String.IsNullOrEmpty(auxIconName) Then
            '    CALCTestIcon = preloadedDataConfig.GetIconImage("TCALC")
            'End If

            'auxIconName = GetIconName("TISE_SYS") 'ISE tests icon
            'If Not String.IsNullOrEmpty(auxIconName) Then
            '    ISETestIcon = preloadedDataConfig.GetIconImage("TISE_SYS")
            'End If

            'auxIconName = GetIconName("TOFF_SYS") 'OFFS tests icon
            'If Not String.IsNullOrEmpty(auxIconName) Then
            '    OFFSTestIcon = preloadedDataConfig.GetIconImage("TOFF_SYS")
            'End If
            'AG 28/02/2014 - #1524

            auxIconName = GetIconName("BLANK")
            If Not String.IsNullOrEmpty(auxIconName) Then
                BlankIcon = preloadedDataConfig.GetIconImage("BLANK")
            End If

            auxIconName = GetIconName("CALIB")
            If Not String.IsNullOrEmpty(auxIconName) Then
                CalibratorIcon = preloadedDataConfig.GetIconImage("CALIB")
            End If

            auxIconName = GetIconName("CTRL")
            If Not String.IsNullOrEmpty(auxIconName) Then
                ControlIcon = preloadedDataConfig.GetIconImage("CTRL")
            End If

            'OPEN CURVE GRAPH Button
            auxIconName = GetIconName("ABS_GRAPH") ' "CURVE"
            If (auxIconName <> "") Then
                GraphImage = preloadedDataConfig.GetIconImage("ABS_GRAPH") ' CURVE
                GraphPictureBox.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
                ResultsAbsImage.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("EXEC_PAUSED") ' "CURVE"
            If (auxIconName <> "") Then
                PausedTestImage.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("MON_LEG_PENDIN") ' "CURVE"
            If (auxIconName <> "") Then
                LegendPendingImage0.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("MON_LEG_INPROG") ' "CURVE"
            If (auxIconName <> "") Then
                LegendInProgressImage0.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("MON_LEG_FINISH") ' "CURVE"
            If (auxIconName <> "") Then
                LegendFinishedImage0.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " LoadImages ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Opens a ResultForm and shows SampleClass and SampleOrTestName info
    ''' </summary>
    ''' <param name="SampleClass">SampleClass to show</param>
    ''' <param name="SampleOrTestName">Sample or Test name to show</param>
    ''' <remarks>
    ''' Created by: RH 13/01/2011
    ''' Modified by: XB 26/02/2014 - Change the way to open the screen using OpenMDIChildForm generic method to avoid OutOfMem errors when back to Monitor - task #1529
    ''' </remarks>
    Public Sub OpenResultForm(ByVal SampleClass As String, ByVal SampleOrTestName As String)
        Try
            'TRAZA DE APERTURA DE FORMULARIO
            CreateLogActivity("Monito WSGrid ---> IResult", Name & " .OpenResultForm", EventLogEntryType.Information, False)

            ' XB 24/02/2014 BT #1499
            Dim PCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage)
            PCounters.GetAllCounters()
            PCounters = Nothing
            ' XB 24/02/2014 BT #1499

            ' XB 26/02/2014 - task #1529
            'Dim myResultForm As New IResults(SampleClass, SampleOrTestName)
            'myResultForm.MdiParent = IAx00MainMDI
            'myResultForm.ActiveWSStatus = IAx00MainMDI.ActiveStatus  'AG 20/03/2012 - inform the WS status (used to disable some buttons)
            'myResultForm.Show()
            'Application.DoEvents()
            'Close()

            IResults.ActiveWSStatus = IAx00MainMDI.ActiveStatus
            IResults.AnalyzerModel = IAx00MainMDI.ActiveAnalyzerModel
            IResults.SampleClass = SampleClass
            IResults.SampleOrTestName = SampleOrTestName
            IAx00MainMDI.OpenMDIChildForm(IResults)
            ' XB 26/02/2014 - task #1529

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OpenResultForm", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenResultForm", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub


    ''' <summary>
    ''' Displays the result chart
    ''' </summary>
    ''' <param name="pOrderTestID">order test id</param> 
    ''' <param name="pMultiItemNumber">multiitem number</param> 
    ''' <param name="pRerunNumber">rerun number</param>
    ''' <remarks>
    ''' Created by dl 18/02/2011
    ''' </remarks>
    Private Sub ShowResultsChart(ByVal pOrderTestID As Integer, _
                                 ByVal pRerunNumber As Integer, _
                                 ByVal pMultiItemNumber As Integer, _
                                 ByVal pTestName As String, _
                                 ByVal pSampleID As String, _
                                 ByVal pSampleClass As String)

        Try

            Using myForm As New IResultsAbsCurve
                myForm.AnalyzerID = AnalyzerIDField
                myForm.WorkSessionID = WorkSessionIDField
                myForm.MultiItemNumber = pMultiItemNumber
                myForm.ReRun = pRerunNumber
                myForm.Replicate = -1
                '
                myForm.TestName = pTestName
                myForm.SampleID = pSampleID
                myForm.SampleClass = pSampleClass
                '
                myForm.OrderTestID = pOrderTestID
                myForm.SourceForm = GlobalEnumerates.ScreenCallsGraphical.WS_STATES
                'myForm.SourceCalled = GraphicalAbsScreenCallMode.WS_STATES_MULTIPLE
                myForm.ListExecutions = myExecutions

                IAx00MainMDI.AddNoMDIChildForm = myForm 'Inform the MDI the curve calib results is shown
                myForm.ShowDialog()
                IAx00MainMDI.RemoveNoMDIChildForm = myForm 'Inform the MDI the curve calib results is closed

            End Using


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " ShowResultsChart ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

#Region "Events"

    Private Sub bsWSExecutionsDataGridView_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
            Handles bsWSExecutionsDataGridView.CellMouseEnter
        Try
            Dim dgv As BSDataGridView = CType(sender, BSDataGridView)
            Dim ColumnName As String = dgv.Columns(e.ColumnIndex).Name
            'Dim executionRow As ExecutionsDS.vwksWSExecutionsMonitorRow

            Select Case ColumnName
                Case "ElementName"
                    If e.RowIndex < 0 Then
                        dgv.Cursor = Cursors.Default
                    Else
                        Dim BkColor As Color = dgv.Rows(e.RowIndex).DefaultCellStyle.BackColor
                        If BkColor = RowGreen OrElse BkColor = HeaderGreen Then
                            dgv.Cursor = Cursors.Hand
                        Else
                            dgv.Cursor = Cursors.Default
                        End If
                    End If

                Case "PausedFlag"
                    If e.RowIndex < 0 Then
                        dgv.Cursor = Cursors.Default
                    Else
                        Dim BkColor As Color = dgv.Rows(e.RowIndex).DefaultCellStyle.BackColor
                        If dgv("PausedFlag", e.RowIndex).Value.Equals(NoImage) Then
                            dgv.Cursor = Cursors.Default
                        Else
                            If (BkColor <> RowGreen) AndAlso (BkColor <> HeaderGreen) Then
                                dgv.Cursor = Cursors.Hand
                            End If
                        End If
                    End If

                Case "Graph"
                    If e.RowIndex < 0 Then
                        dgv.Cursor = Cursors.Default
                    Else
                        If dgv(e.ColumnIndex, e.RowIndex).Value.Equals(GraphImage) Then
                            dgv.Cursor = Cursors.Hand
                        Else
                            dgv.Cursor = Cursors.Default
                        End If
                    End If

                Case Else
                    dgv.Cursor = Cursors.Default
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsWSExecutionsDataGridView_CellMouseEnter ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    Private Sub bsWSExecutionsDataGridView_CellMouseLeave(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
            Handles bsWSExecutionsDataGridView.CellMouseLeave

        Try
            Dim dgv As BSDataGridView = CType(sender, BSDataGridView)

            'If e.RowIndex > 0 Then
            dgv.Cursor = Cursors.Default
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsWSExecutionsDataGridView_CellMouseLeave ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Grid functionality:
    ''' - Paused column
    ''' - Element Name column
    ''' - Graph absorbance versus time column
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Modified AG 03/03/2014 - #1524</remarks>
    Private Sub bsWSExecutionsDataGridView_CellMouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) _
            Handles bsWSExecutionsDataGridView.CellMouseClick
        Try
            If (IsDisposed) Then Return 'IT 03/06/2014 - #1644 No refresh if screen is disposed

            If e.RowIndex < 0 Then Return
            If sender Is Nothing Then Return

            Dim dgv As BSDataGridView = CType(sender, BSDataGridView)
            Dim ColumnName As String = dgv.Columns(e.ColumnIndex).Name
            Dim executionRow As ExecutionsDS.vwksWSExecutionsMonitorRow

            Select Case ColumnName
                Case "PausedFlag"
                    If myWSGridDS.vwksWSExecutionsMonitor.Rows.Count >= e.RowIndex AndAlso dgv.Cursor = Cursors.Hand Then
                        executionRow = DirectCast(myWSGridDS.vwksWSExecutionsMonitor(e.RowIndex), ExecutionsDS.vwksWSExecutionsMonitorRow)

                        Dim resultData As New GlobalDataTO
                        Dim myOT As New List(Of Integer) 'Affected orderTestID
                        Dim myRerun As New List(Of Integer) 'Affected Rerun for the previous orderTestID list
                        Dim myRefreshRowsAffected As New List(Of Integer) 'Grid rows affected for refresh (IMPORTANT it must be sorted ASC by column Index!!!!!)
                        Dim myNewValue As Boolean = False
                        Dim PendingRows As New List(Of ExecutionsDS.vwksWSExecutionsMonitorRow)

                        If executionRow.IsHeader Then
                            'Click on the HEADER row
                            If Not executionRow.IsPausedNull Then myNewValue = Not executionRow.Paused

                            'Get all OT - Reruns with Values <> from new value
                            PendingRows = _
                               (From row As ExecutionsDS.vwksWSExecutionsMonitorRow In myWSGridDS.vwksWSExecutionsMonitor _
                                Where row.OrderID = executionRow.OrderID AndAlso _
                                row.TestType <> "OFFS" AndAlso row.TestType <> "CALC" AndAlso _
                                Not row.IsExecutionStatusNull AndAlso _
                                (row.ExecutionStatus = "PENDING" OrElse row.ExecutionStatus = "LOCKED") AndAlso _
                                Not row.IsPausedNull AndAlso row.Paused <> myNewValue AndAlso _
                                row.SampleType = executionRow.SampleType _
                                Select row).ToList()

                            If PendingRows.Count > 0 Then
                                'Set Pause value for every execution with the same OrderID and other criteria
                                For Each row As ExecutionsDS.vwksWSExecutionsMonitorRow In PendingRows
                                    If Not row.IsOrderTestIDNull AndAlso Not myOT.Contains(row.OrderTestID) Then
                                        myOT.Add(row.OrderTestID)
                                        If Not row.IsRerunNumberNull Then myRerun.Add(row.RerunNumber)
                                    End If
                                    If Not row.IsIndexNull AndAlso Not myRefreshRowsAffected.Contains(row.Index) Then myRefreshRowsAffected.Add(row.Index)
                                Next
                            End If

                        Else
                            'Click on the CHILD row
                            If executionRow.TestType <> "OFFS" AndAlso executionRow.TestType <> "CALC" Then 'Work only with STD and ISE tests
                                'Get the values: paused, OT, Rerun
                                If Not executionRow.IsExecutionStatusNull AndAlso (executionRow.ExecutionStatus = "PENDING" OrElse executionRow.ExecutionStatus = "LOCKED") Then

                                    If Not executionRow.IsOrderTestIDNull Then myOT.Add(executionRow.OrderTestID)
                                    If Not executionRow.IsRerunNumberNull Then myRerun.Add(executionRow.RerunNumber)
                                    If Not executionRow.IsPausedNull Then myNewValue = Not executionRow.Paused

                                    'If NO child paused -> Header not paused, if AT LEAST ONE child paused -> Header paused
                                    'Current test child is going to be update to Paused = myNewValue
                                    'Search if exists at least 1 "brother" child test with Paused = FALSE (header icon = "") 
                                    PendingRows.Clear()
                                    If Not myNewValue Then
                                        'Search if exists at least 1 "brother" child test with Paused = TRUE (header must continue with icon = PAUSED) 
                                        PendingRows = _
                                            (From row As ExecutionsDS.vwksWSExecutionsMonitorRow In myWSGridDS.vwksWSExecutionsMonitor _
                                             Where row.OrderID = executionRow.OrderID AndAlso _
                                             row.OrderTestID <> executionRow.OrderTestID AndAlso _
                                             row.TestType <> "OFFS" AndAlso row.TestType <> "CALC" AndAlso _
                                             row.IsHeader = False AndAlso _
                                             row.Paused = True AndAlso _
                                             row.SampleType = executionRow.SampleType _
                                             Select row).ToList()
                                    End If

                                    Dim checkHeaderFlag As Boolean = False
                                    If Not myNewValue AndAlso PendingRows.Count = 0 Then
                                        checkHeaderFlag = True
                                    ElseIf myNewValue Then
                                        checkHeaderFlag = True
                                    End If

                                    If checkHeaderFlag Then
                                        'Search if the header has the wrong Paused value, in this case update it
                                        PendingRows = _
                                            (From row As ExecutionsDS.vwksWSExecutionsMonitorRow In myWSGridDS.vwksWSExecutionsMonitor _
                                             Where row.OrderID = executionRow.OrderID AndAlso _
                                             row.IsHeader = True AndAlso _
                                             Not row.IsPausedNull AndAlso row.Paused = Not myNewValue AndAlso _
                                             row.SampleType = executionRow.SampleType AndAlso _
                                             row.Index < executionRow.Index _
                                             Select row).ToList()

                                        'Add the header row into the grid rows that have to be refreshed!!
                                        If PendingRows.Count > 0 Then
                                            If Not PendingRows(PendingRows.Count - 1).IsIndexNull Then myRefreshRowsAffected.Add(PendingRows(PendingRows.Count - 1).Index)
                                        End If
                                    End If

                                    'Finally add the selected by user row into the list of rows to refresh
                                    If Not executionRow.IsIndexNull Then myRefreshRowsAffected.Add(executionRow.Index)
                                End If

                            End If
                        End If

                        PendingRows.Clear()
                        PendingRows = Nothing

                        'If exists data to update --> call the method for update it
                        Dim updateCalledFlag As Boolean = False
                        If myOT.Count > 0 AndAlso myOT.Count = myRerun.Count Then
                            updateCalledFlag = True
                            'Update Paused values in DB
                            Dim myExecutionsDelegate As New ExecutionsDelegate
                            'AG 20/03/2014 - #1545 add parameters analyzerID, worksessionID
                            'resultData = myExecutionsDelegate.UpdatePaused(Nothing, myOT, myRerun, myNewValue)
                            resultData = myExecutionsDelegate.UpdatePaused(Nothing, myOT, myRerun, myNewValue, AnalyzerIDField, WorkSessionIDField)
                            myOT.Clear()
                            myRerun.Clear()
                            myOT = Nothing
                            myRerun = Nothing
                        End If

                        'Finally refresh internal DS and grid
                        If updateCalledFlag AndAlso Not resultData.HasError Then
                            For Each rowToRefresh As Integer In myRefreshRowsAffected
                                myWSGridDS.vwksWSExecutionsMonitor(rowToRefresh).BeginEdit()
                                myWSGridDS.vwksWSExecutionsMonitor(rowToRefresh).Paused = myNewValue
                                myWSGridDS.vwksWSExecutionsMonitor(rowToRefresh).EndEdit()
                                myWSGridDS.vwksWSExecutionsMonitor.AcceptChanges()

                                'When a header is refreshed we must inform (HeaderIndex, CurrentHeaderColor) that will be used in the child refreshment
                                If myWSGridDS.vwksWSExecutionsMonitor(rowToRefresh).IsHeader Then
                                    HeaderIndex = rowToRefresh
                                    CurrentHeaderColor = dgv.Rows(rowToRefresh).DefaultCellStyle.BackColor
                                End If

                                'Refresh affected grid row
                                DrawRefreshRow(dgv, myWSGridDS.vwksWSExecutionsMonitor(rowToRefresh), rowToRefresh) 'Fill it
                            Next
                        End If

                    End If

                Case "ElementName"
                    'AG 03/03/2014 - #1524
                    If myWSGridDS.vwksWSExecutionsMonitor.Rows.Count >= e.RowIndex Then
                        executionRow = DirectCast(myWSGridDS.vwksWSExecutionsMonitor(e.RowIndex), ExecutionsDS.vwksWSExecutionsMonitorRow)
                        Dim BkColor As Color = dgv.Rows(e.RowIndex).DefaultCellStyle.BackColor

                        ' XB 14/03/2014 - Delete Collapse functionality due System Out of memory cause errors - Task #1543
                        'If CType(dgv(CollapseColName, e.RowIndex), bsDataGridViewCollapseCell).IsSubHeader Then
                        '    If BkColor = HeaderGreen Then
                        '        If executionRow.SampleClass = "PATIENT" Then
                        '            'AG 03/03/2014 - Use ElementName instead of PatientID
                        '            'OpenResultForm(executionRow.SampleClass, executionRow.PatientID)
                        '            OpenResultForm(executionRow.SampleClass, executionRow.ElementName)
                        '        Else
                        '            OpenResultForm(executionRow.SampleClass, executionRow.TestName)
                        '        End If
                        '    End If
                        'Else
                        'Normal row
                        If BkColor = RowGreen Then
                            If executionRow.SampleClass = "PATIENT" Then
                                'AG 03/03/2014 - Use ElementName instead of PatientID
                                'OpenResultForm(executionRow.SampleClass, executionRow.PatientID)
                                OpenResultForm(executionRow.SampleClass, executionRow.ElementName)
                            Else
                                OpenResultForm(executionRow.SampleClass, executionRow.TestName)
                            End If
                        End If
                        'End If


                    End If
                    'AG 03/03/2014 - #1524

                Case "Graph"
                    'AG 03/03/2014 - #1524
                    If myWSGridDS.vwksWSExecutionsMonitor.Rows.Count >= e.RowIndex Then
                        If dgv(e.ColumnIndex, e.RowIndex).Value.Equals(GraphImage) Then
                            executionRow = DirectCast(myWSGridDS.vwksWSExecutionsMonitor(e.RowIndex), ExecutionsDS.vwksWSExecutionsMonitorRow)

                            Dim myTestName As String = ""
                            Dim mySampleID As String = ""
                            Dim mySampleClass As String = ""

                            If Not executionRow.IsTestNameNull Then myTestName = executionRow.TestName
                            If executionRow.SampleClass <> "" Then mySampleClass = executionRow.SampleClass

                            'AG 03/03/2014 - Use elementName instead of sampleID (new view does not use this column)
                            'If Not executionRow.IsSampleIDNull Then mySampleID = executionRow.SampleID
                            If mySampleClass = "PATIENT" Then
                                If Not executionRow.IsElementNameNull Then mySampleID = executionRow.ElementName
                            End If

                            ShowResultsChart(executionRow.OrderTestID, _
                                             executionRow.RerunNumber, _
                                             1, _
                                             myTestName, _
                                             mySampleID, _
                                             mySampleClass)
                        End If
                    End If
                    'AG 03/03/2014 - #1524

            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsWSExecutionsDataGridView_CellMouseClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub


#End Region

#Region "MultiLanguage"

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 04/04/2011
    ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
    ''' </remarks>
    Private Sub GetStateTabLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            StatText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Stat", LanguageID)
            RoutineText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Routine", LanguageID)
            labelReportPrintAvailable = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Final_Print", LanguageID)
            labelReportPrintNOTAvailable = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Not_Final_Print", LanguageID)
            labelHISSent = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_HIS_Sent", LanguageID)
            labelHISNOTSent = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Not_HIS_Sent", LanguageID)

            'RH 02/05/2011
            'EF 29/08/2013 - Bugtracking 1272 - Change label text by 'Sample' in v2.1.1
            'labelName = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", LanguageID)
            labelName = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", LanguageID)
            'EF 29/08/2013
            labelType = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", LanguageID)
            labelState = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_State", LanguageID)
            labelAbsorbanceCurve = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ABSORBANCE_CURVE", LanguageID)
            labelLocked = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LOCKED", LanguageID)
            labelFinishedRemarks = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FINISHED_REMARKS", LanguageID)
            labelFinishedOpticalKO = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FINISHED_OPTICAL_KO", LanguageID)
            labelSamplePosStatusFinished = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_STATUS_FINISHED", LanguageID)
            labelWSActionPause = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_WSAction_Pause", LanguageID)
            labelTitleResult = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Results", LanguageID)

            'RH 18/04/2012
            labelBLANK = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Blanks", LanguageID)    '.ToUpper()

            'RH 25/07/2011
            labelWSActionResume = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Resume", LanguageID)

            'RH 25/07/2011 Grid Texts
            bsTestStatusLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WORKSESSION_STATUS", LanguageID)
            TotalTestsChart.Titles(0).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Total", LanguageID)
            TotalTestsChart.Titles(1).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_STATUS_FINISHED", LanguageID) 'Finished
            TotalTestsChart.Titles(2).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_STATUS_INPROGRESS", LanguageID) 'In Process
            TotalTestsChart.Titles(3).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_STATUS_PENDING", LanguageID) 'Pending
            TotalTestsChart.Titles(4).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LOCKED", LanguageID) 'Locked
            TotalTestsChart.Titles(5).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WithAlarms", LanguageID) 'With Alarms

            bsTimeAvailableLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Time_Available_Rotors", LanguageID)

            'Legend
            ResultsStatusLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ResultsStatus", LanguageID)
            ResultsReadyLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ResultsReady", LanguageID)
            ResultsWarningLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ResultsWarning", LanguageID)
            ResultsAbsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Abst", LanguageID)
            FinalReportLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Final_Print", LanguageID)
            ExportLISLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExportLIS", LanguageID)
            TestStatusLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestsStatus", LanguageID)
            LockedTestLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LockedTest", LanguageID)
            PausedTestLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PausedTest", LanguageID)
            bsWorksessionLegendLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Legend", LanguageID)

            PendingLabel0.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_STATUS_PENDING", LanguageID)
            InProgressLabel0.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_STATUS_INPROGRESS", LanguageID)
            FinishedLabel0.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_POS_STATUS_FINISHED", LanguageID)

            'SGM 12/04/2013 - get LIS name for header
            Dim myParams As New SwParametersDelegate
            Dim myParametersDS As New ParametersDS
            Dim myGlobal As New GlobalDataTO
            myGlobal = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.LIS_NAME.ToString, Nothing)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                If myParametersDS.tfmwSwParameters.Rows.Count > 0 Then
                    MyClass.LISNameForColumnHeaders = myParametersDS.tfmwSwParameters.Item(0).ValueText
                End If
            End If
            'end SGM 12/04/2013

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

        End Try
    End Sub



#End Region

#Region "NEW FUNCTIONS TO IMPROVE PERFORMANCE OF MONITOR SCREEN - SA (in process)"

    Private Sub InitializeStatesGrid_NEW()
        Try
            Dim columnName As String
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsWSExecutionsDataGridView.AutoGenerateColumns = False
            bsWSExecutionsDataGridView.AllowUserToAddRows = False
            bsWSExecutionsDataGridView.AllowUserToDeleteRows = False
            bsWSExecutionsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically

            bsWSExecutionsDataGridView.Rows.Clear()
            bsWSExecutionsDataGridView.Columns.Clear()

            ' XB 14/03/2014 - Delete Collapse functionality due System Out of memory cause errors - Task #1543
            ''Collapse column
            'Dim CollapseColumn As New bsDataGridViewCollapseColumn()
            'CollapseColumn.Name = CollapseColName
            'bsWSExecutionsDataGridView.Columns.Add(CollapseColumn)

            'SampleClass column (icon)
            Dim iconSampleClassColumn As New DataGridViewImageColumn
            columnName = "SampleClassIcon"
            iconSampleClassColumn.Name = columnName

            bsWSExecutionsDataGridView.Columns.Add(iconSampleClassColumn)
            bsWSExecutionsDataGridView.Columns(columnName).Width = 33
            bsWSExecutionsDataGridView.Columns(columnName).ReadOnly = True
            bsWSExecutionsDataGridView.Columns(columnName).HeaderText = String.Empty
            bsWSExecutionsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsWSExecutionsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            bsWSExecutionsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Paused column (icon)
            Dim iconPausedColumn As New DataGridViewImageColumn
            columnName = "PauseIcon"
            iconPausedColumn.Name = columnName

            bsWSExecutionsDataGridView.Columns.Add(iconPausedColumn)
            bsWSExecutionsDataGridView.Columns(columnName).Width = 33
            bsWSExecutionsDataGridView.Columns(columnName).ReadOnly = True
            bsWSExecutionsDataGridView.Columns(columnName).HeaderText = String.Empty
            bsWSExecutionsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsWSExecutionsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            bsWSExecutionsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'ElementNameToShown column
            columnName = "ElementNameToShown"
            bsWSExecutionsDataGridView.Columns.Add(columnName, labelName)

            ' XB 14/03/2014 - Delete Collapse functionality due System Out of memory cause errors - Task #1543
            'bsWSExecutionsDataGridView.Columns(columnName).Width = 180
            bsWSExecutionsDataGridView.Columns(columnName).Width = 205
            ' XB 14/03/2014 

            bsWSExecutionsDataGridView.Columns(columnName).ReadOnly = True
            bsWSExecutionsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsWSExecutionsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            bsWSExecutionsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            'SampleType column
            columnName = "SampleType"
            bsWSExecutionsDataGridView.Columns.Add(columnName, labelType)
            bsWSExecutionsDataGridView.Columns(columnName).Width = 45
            bsWSExecutionsDataGridView.Columns(columnName).ReadOnly = True
            bsWSExecutionsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsWSExecutionsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            bsWSExecutionsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Alarms column (icon)
            Dim iconStatusColumn As New DataGridViewImageColumn
            columnName = "Alarms"
            iconStatusColumn.Name = columnName

            bsWSExecutionsDataGridView.Columns.Add(iconStatusColumn)
            bsWSExecutionsDataGridView.Columns(columnName).Width = 50
            bsWSExecutionsDataGridView.Columns(columnName).ReadOnly = True
            bsWSExecutionsDataGridView.Columns(columnName).HeaderText = labelState
            bsWSExecutionsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsWSExecutionsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            bsWSExecutionsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Printed column (icon)
            Dim iconPrintedColumn As New DataGridViewImageColumn
            columnName = "PrintedIcon"
            iconPrintedColumn.Name = columnName

            bsWSExecutionsDataGridView.Columns.Add(iconPrintedColumn)
            bsWSExecutionsDataGridView.Columns(columnName).Width = 33
            bsWSExecutionsDataGridView.Columns(columnName).ReadOnly = True
            bsWSExecutionsDataGridView.Columns(columnName).HeaderText = String.Empty
            bsWSExecutionsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsWSExecutionsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            bsWSExecutionsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Exported column (icon)
            Dim iconExportedColumn As New DataGridViewImageColumn
            columnName = "ExportedIcon"
            iconExportedColumn.Name = columnName

            bsWSExecutionsDataGridView.Columns.Add(iconExportedColumn)
            bsWSExecutionsDataGridView.Columns(columnName).Width = 33
            bsWSExecutionsDataGridView.Columns(columnName).ReadOnly = True
            bsWSExecutionsDataGridView.Columns(columnName).HeaderText = LISNameForColumnHeaders
            bsWSExecutionsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsWSExecutionsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            bsWSExecutionsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'ABS Graph column (icon)
            Dim iconGraphColumn As New DataGridViewImageColumn
            columnName = "GraphIcon"
            iconGraphColumn.Name = columnName

            bsWSExecutionsDataGridView.Columns.Add(iconGraphColumn)
            bsWSExecutionsDataGridView.Columns(columnName).Width = 33
            bsWSExecutionsDataGridView.Columns(columnName).ReadOnly = True
            bsWSExecutionsDataGridView.Columns(columnName).HeaderText = String.Empty
            bsWSExecutionsDataGridView.Columns(columnName).DataPropertyName = columnName
            bsWSExecutionsDataGridView.Columns(columnName).SortMode = DataGridViewColumnSortMode.Programmatic
            bsWSExecutionsDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            bsWSExecutionsDataGridView.ScrollBars = ScrollBars.Both

            Const PicTop As Integer = 8
            Const PicLeft As Integer = 9
            Const HeadImageSide As Integer = 16

            'Header Icon for Paused column
            PausePictureBox.Name = "PausePictureBox"
            PausePictureBox.SizeMode = PictureBoxSizeMode.StretchImage
            PausePictureBox.Size = New Size(HeadImageSide, HeadImageSide)
            PausePictureBox.TabStop = False

            Dim X As Integer = 0
            For i As Integer = 0 To iconPausedColumn.Index - 1
                X += bsWSExecutionsDataGridView.Columns(i).Width
            Next

            PausePictureBox.Left = X + PicLeft
            PausePictureBox.Top = PicTop
            bsWSExecutionsDataGridView.Controls.Add(PausePictureBox)

            'Header Icon for Printed column
            PrintPictureBox.Name = "PrintPictureBox"
            PrintPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
            PrintPictureBox.Size = New Size(HeadImageSide, HeadImageSide)
            PrintPictureBox.TabStop = False

            X = 0
            For i As Integer = 0 To iconPrintedColumn.Index - 1
                X += bsWSExecutionsDataGridView.Columns(i).Width
            Next

            PrintPictureBox.Left = X + PicLeft
            PrintPictureBox.Top = PicTop
            bsWSExecutionsDataGridView.Controls.Add(PrintPictureBox)

            'Header Icon for ABS Graph column
            GraphPictureBox.Name = "GraphPictureBox"
            GraphPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
            GraphPictureBox.Size = New Size(HeadImageSide, HeadImageSide)
            GraphPictureBox.TabStop = False

            X = 0
            For i As Integer = 0 To iconGraphColumn.Index - 1
                X += bsWSExecutionsDataGridView.Columns(i).Width
            Next

            GraphPictureBox.Left = X + PicLeft
            GraphPictureBox.Top = PicTop
            bsWSExecutionsDataGridView.Controls.Add(GraphPictureBox)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeStatesGrid_NEW", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeStatesGrid_NEW", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    Private Sub LoadExecutionsDS_NEW()
        Dim resultData As New GlobalDataTO
        Try
            resultData = myExecutionsDelegate.GetDataForMonitorTabWS_NEW(Nothing, ActiveAnalyzer, ActiveWorkSession, labelBLANK)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                If (myWSGridDS Is Nothing) Then
                    'Create the global DS
                    myWSGridDS = New ExecutionsDS
                Else
                    'Clear data tables in the global DS
                    myWSGridDS.vwksWSExecutionsMonitor.Clear()
                    myWSGridDS.vwksWSExecutionsAlarms.Clear()
                End If

                myWSGridDS = DirectCast(resultData.SetDatos, ExecutionsDS)
                bsWSExecutionsDataGridView.DataSource = myWSGridDS.vwksWSExecutionsMonitor

                'Load the Alarms DS
                LoadAlarms()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadExecutionsDS_NEW", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadExecutionsDS_NEW", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    Private Sub UpdateExecutionsDataGridView_NEW(ByVal pCreateDGWRows As Boolean, ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            Dim dgv As BSDataGridView = bsWSExecutionsDataGridView

            'Initialize variables!!
            MaxRow = -1 'MaxRow is used in mode creation 'dgv.Rows.Count - 1
            CurrentRowColor = RowYellow
            HeaderIndex = -1 'Header position for the current child test (creation or update)
            CurrentHeaderColor = HeaderYellow
            currentHeaderAlarmsIcon = AlarmsIconValues.None

            'Statistics Frame
            InitializeStatisticsTable()
            If (myWSGridDS.vwksWSExecutionsMonitor.Rows.Count > 0) Then
                CalculateStatisticsCharts()
                UpdateStatisticsCharts()
            Else
                ResetStatisticsCharts()
            End If

            'If mode CreateDGWRows or fake refresh then create grid!!!
            If (pCreateDGWRows OrElse (Not pRefreshDS Is Nothing AndAlso pRefreshDS.ExecutionStatusChanged.Rows.Count = 0)) Then
                If (myWSGridDS.vwksWSExecutionsMonitor.Rows.Count > 0) Then
                    For Each wsGridRow As ExecutionsDS.vwksWSExecutionsMonitorRow In myWSGridDS.vwksWSExecutionsMonitor
                        MaxRow += 1
                        DrawNewRow_NEW(dgv, wsGridRow) 'Fill it
                    Next

                    ''At this moment HeaderIndex points to the last Header in grid
                    'If HeaderIndex <> -1 Then
                    '    DrawCurrentHeaderRow()

                    '    'After paint last header initialize values for next
                    '    HeaderIndex = -1
                    '    CurrentHeaderColor = HeaderYellow
                    '    CurrentHeaderAlarmsIcon = AlarmsIconValues.None
                    'End If
                End If
            Else
                'Refresh mode
                Dim myRefreshRowsAffected As New List(Of Integer) 'Grid rows affected for refresh (IMPORTANT it must be sorted ASC by column Index!!!!!)
                Dim affectedGridRows As New List(Of ExecutionsDS.vwksWSExecutionsMonitorRow)
                Dim orderIDsWithCALCTestsAffected As New List(Of String) 'When a OrderTests changes to CLOSED it is possible that the calculated tests of this patient were already calculated!!!
                Dim otDlg As New OrderTestsDelegate
                Dim resultData As GlobalDataTO

                Dim childPosition As Integer = -1
                Dim headerPosition As Integer = -1

                'Loop the refresh DS data and update the local screen DS and prepare data for refresh the grid
                For Each refreshRow As UIRefreshDS.ExecutionStatusChangedRow In pRefreshDS.ExecutionStatusChanged
                    '1- Search the CHILD test row in the DS
                    affectedGridRows = (From a As ExecutionsDS.vwksWSExecutionsMonitorRow In myWSGridDS.vwksWSExecutionsMonitor _
                               Where a.OrderTestID = refreshRow.OrderTestID AndAlso a.RerunNumber = refreshRow.RerunNumber AndAlso _
                               a.IsHeader = False Select a).ToList

                    '2- If found update his status values!!! (only 1 row can be matched by OrderTest - Rerun)
                    If affectedGridRows.Count > 0 Then
                        childPosition = affectedGridRows(0).Index
                        With myWSGridDS.vwksWSExecutionsMonitor(childPosition)
                            .BeginEdit()
                            If Not .IsExecutionStatusNull AndAlso Not refreshRow.IsExecutionStatusNull AndAlso .ExecutionStatus <> refreshRow.ExecutionStatus Then .ExecutionStatus = refreshRow.ExecutionStatus
                            If Not .IsOrderTestStatusNull AndAlso Not refreshRow.IsOrderTestStatusNull AndAlso .OrderTestStatus <> refreshRow.OrderTestStatus Then .OrderTestStatus = refreshRow.OrderTestStatus
                            If Not .IsOrderStatusNull AndAlso Not refreshRow.IsOrderStatusNull AndAlso .OrderStatus <> refreshRow.OrderStatus Then .OrderStatus = refreshRow.OrderStatus
                            .EndEdit()
                            .AcceptChanges()

                            'Inform the OrderID list which calculated tests STATUS could have change to CLOSED
                            If Not refreshRow.IsOrderTestStatusNull AndAlso refreshRow.OrderTestStatus = "CLOSED" AndAlso _
                                Not refreshRow.IsOrderIDNull AndAlso Not orderIDsWithCALCTestsAffected.Contains(refreshRow.OrderID) Then
                                orderIDsWithCALCTestsAffected.Add(refreshRow.OrderID)
                            End If
                        End With


                        '3- Then search the HEADER row and update value (OrderStatus) if required  (note that the HEADER is the nearest isHeader = TRUE but with index lower)
                        affectedGridRows = _
                            (From row As ExecutionsDS.vwksWSExecutionsMonitorRow In myWSGridDS.vwksWSExecutionsMonitor _
                             Where row.OrderID = refreshRow.OrderID AndAlso _
                             row.IsHeader = True AndAlso _
                             row.SampleType = row.SampleType AndAlso _
                             row.Index < affectedGridRows(0).Index _
                             Select row).ToList()

                        If affectedGridRows.Count > 0 Then
                            headerPosition = affectedGridRows(affectedGridRows.Count - 1).Index
                            With myWSGridDS.vwksWSExecutionsMonitor(headerPosition)
                                .BeginEdit()
                                If Not .IsOrderStatusNull AndAlso Not refreshRow.IsOrderStatusNull AndAlso .OrderStatus <> refreshRow.OrderStatus Then .OrderStatus = refreshRow.OrderStatus
                                .EndEdit()
                                .AcceptChanges()
                            End With

                            '4- Prepare the grid rows to be updated (header and required children)
                            'If ExecutionStatus = INPROCESS then refresh only the affected child tests row in pRefreshDS and his header (1 execution inprocess means header inprocess)
                            'Else: We must send to refresh the header and all his children (because the header is calculated using all their children)
                            '- ExecutionStatus PENDING/LOCKED/CLOSED: 1 execution pending/locked/closed does not mean header pending/locked/closed
                            '- OrderTestStatus CLOSED: 1 test closed does not mean test/header GREEN (besides when autorerun is launch the ordertest continues INPROCESS)
                            '- OrderStatus <> CLOSED: does not mean header is not GREEN (automatic reruns)

                            If Not myRefreshRowsAffected.Contains(headerPosition) Then myRefreshRowsAffected.Add(headerPosition) 'Add header into list to refresh
                            If Not refreshRow.IsExecutionStatusNull AndAlso refreshRow.ExecutionStatus = "INPROCESS" Then
                                If Not myRefreshRowsAffected.Contains(childPosition) Then myRefreshRowsAffected.Add(childPosition) 'Add child into list to refresh
                            Else
                                'Add ALL children tests into list to refresh
                                For i As Integer = headerPosition + 1 To myWSGridDS.vwksWSExecutionsMonitor.Count - 1
                                    If Not myWSGridDS.vwksWSExecutionsMonitor(i).IsHeader Then
                                        If Not myRefreshRowsAffected.Contains(i) Then
                                            myRefreshRowsAffected.Add(i)

                                            'If the orderId has been marked as possible changes into CALC tests then read the current CALC test status
                                            'and update myWSGridDS
                                            If myWSGridDS.vwksWSExecutionsMonitor(i).TestType = "CALC" _
                                                AndAlso Not myWSGridDS.vwksWSExecutionsMonitor(i).IsOrderIDNull _
                                                AndAlso orderIDsWithCALCTestsAffected.Contains(myWSGridDS.vwksWSExecutionsMonitor(i).OrderID) Then
                                                resultData = otDlg.GetOrderTest(Nothing, myWSGridDS.vwksWSExecutionsMonitor(i).OrderTestID)
                                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                                    If DirectCast(resultData.SetDatos, OrderTestsDS).twksOrderTests.Rows.Count > 0 AndAlso _
                                                        DirectCast(resultData.SetDatos, OrderTestsDS).twksOrderTests(0).OrderTestStatus = "CLOSED" Then
                                                        myWSGridDS.vwksWSExecutionsMonitor(i).BeginEdit()
                                                        myWSGridDS.vwksWSExecutionsMonitor(i).OrderTestStatus = "CLOSED"
                                                        myWSGridDS.vwksWSExecutionsMonitor(i).EndEdit()
                                                    End If
                                                End If
                                            End If
                                        End If

                                    Else
                                        'Exit loop when next header is found!!!
                                        Exit For
                                    End If
                                Next
                            End If

                        End If

                    End If
                    myWSGridDS.vwksWSExecutionsMonitor.AcceptChanges()
                Next

                'Finally call the headers and child tests rows affected for refresh in the grid
                'myRefreshRowsAffected.Sort()
                For Each rowToRefresh As Integer In myRefreshRowsAffected
                    If myWSGridDS.vwksWSExecutionsMonitor(rowToRefresh).IsHeader Then
                        HeaderIndex = -1
                        CurrentHeaderColor = HeaderYellow
                        currentHeaderAlarmsIcon = AlarmsIconValues.None
                    End If

                    'Refresh affected grid row
                    'DrawRefreshRow(dgv, myWSGridDS.vwksWSExecutionsMonitor(rowToRefresh), rowToRefresh) 'Fill it
                Next

                'At this moment HeaderIndex points to the last Header in grid
                'If HeaderIndex <> -1 AndAlso bsWSExecutionsDataGridView.Rows(HeaderIndex).DefaultCellStyle.BackColor <> CurrentHeaderColor Then
                If HeaderIndex <> -1 Then 'AG 14/03/2014
                    DrawCurrentHeaderRow()

                    'After paint last header initialize values for next
                    HeaderIndex = -1
                    CurrentHeaderColor = HeaderYellow
                    currentHeaderAlarmsIcon = AlarmsIconValues.None
                End If

                'Release elements
                affectedGridRows.Clear()
                myRefreshRowsAffected.Clear()
                orderIDsWithCALCTestsAffected.Clear()
                affectedGridRows = Nothing
                myRefreshRowsAffected = Nothing
                orderIDsWithCALCTestsAffected = Nothing
            End If

            ' XB 14/03/2014 - Delete Collapse functionality due System Out of memory cause errors - Task #1543
            ''Set the rows count for the Collapse Column
            'DirectCast(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = dgv.Rows.Count

            'CreateLogActivity("TIME: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Name & ".UpdateExecutionsDataGridView", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateExecutionsDataGridView_NEW", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpdateExecutionsDataGridView_NEW", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
            'Finally
            '    bsWSExecutionsDataGridView.Enabled = True
        End Try
    End Sub

    Private Sub DrawNewRow_NEW(ByVal dgv As BSDataGridView, ByVal pNewRow As ExecutionsDS.vwksWSExecutionsMonitorRow)
        Try
            Dim auxString As String = String.Empty
            CurrentRowColor = AssignRowColor(pNewRow)
            SetRowColor(MaxRow, CurrentRowColor)

            ' XB 14/03/2014 - Delete Collapse functionality due System Out of memory cause errors - Task #1543
            ''Collapse column
            'If pNewRow.IsHeader Then HeaderIndex = MaxRow 'Set the index (in grid and in myWSGridDS) of the last header
            'CType(dgv(CollapseColName, MaxRow), bsDataGridViewCollapseCell).IsSubHeader = pNewRow.IsHeader

            'SampleClass column
            If (pNewRow.IsHeader) Then
                If (pNewRow.SampleClass <> "BLANK") Then
                    If (pNewRow.StatFlag) Then
                        dgv("SampleClassIcon", MaxRow).ToolTipText = StatText
                    Else
                        dgv("SampleClassIcon", MaxRow).ToolTipText = RoutineText
                    End If
                End If
            Else
                dgv("SampleClassIcon", MaxRow).ToolTipText = String.Empty
            End If

            'Pause column 
            If (CurrentRowColor = RowYellow OrElse CurrentRowColor = HeaderYellow) Then
                If (pNewRow.SampleClass = "PATIENT") AndAlso (pNewRow.TestType = "STD" OrElse pNewRow.TestType = "ISE") Then
                    If (Not pNewRow.IsPausedNull AndAlso pNewRow.Paused) Then 'First replicate
                        dgv("PauseIcon", MaxRow).ToolTipText = labelWSActionResume

                        ''If one child paused then the header is paused too!!!
                        'If Not dgv("PausedFlag", HeaderIndex).Value.Equals(PausedImage) Then
                        '    dgv("PausedIcon", HeaderIndex).ToolTipText = labelWSActionResume

                        '    'Update the dataset myWSGridDS
                        '    myWSGridDS.vwksWSExecutionsMonitor(HeaderIndex).BeginEdit()
                        '    myWSGridDS.vwksWSExecutionsMonitor(HeaderIndex).Paused = True
                        '    myWSGridDS.vwksWSExecutionsMonitor(HeaderIndex).EndEdit()
                        '    myWSGridDS.vwksWSExecutionsMonitor.AcceptChanges()
                        'End If

                    Else
                        dgv("PauseIcon", MaxRow).ToolTipText = labelWSActionPause
                    End If
                End If
            End If

            'ElementNameToShown column
            If (pNewRow.IsHeader) Then
                dgv("ElementNameToShown", MaxRow).Style.Font = HeaderFont
            Else
                dgv("ElementNameToShown", MaxRow).Style.Font = RowFont
            End If

            'ElementNameToShown column - ToolTip
            If (CurrentRowColor = RowGreen OrElse CurrentRowColor = HeaderGreen) Then
                dgv("ElementNameToShown", MaxRow).ToolTipText = labelTitleResult

                If (pNewRow.SampleClass = "PATIENT") Then
                    If (Not pNewRow.IsElementNameNull) Then auxString = pNewRow.ElementName
                    If (Not pNewRow.IsSpecimenIDListNull) Then
                        dgv("ElementNameToShown", MaxRow).ToolTipText = String.Format("{0} ({1}) - {2}", pNewRow.SpecimenIDList, auxString, pNewRow.PatientName)
                    Else
                        dgv("ElementNameToShown", MaxRow).ToolTipText = String.Format("{0} - {1}", auxString, pNewRow.PatientName)
                    End If
                End If
            Else
                dgv("ElementNameToShown", MaxRow).ToolTipText = String.Empty
            End If

            'SampleType column
            If (pNewRow.IsHeader) Then
                dgv("SampleType", MaxRow).Style.Font = HeaderFont
            Else
                dgv("SampleType", MaxRow).Style.Font = RowFont
            End If

            'Status column
            If (pNewRow.IsHeader) Then
                dgv("Alarms", HeaderIndex).ToolTipText = String.Empty
            Else
                If (CurrentRowColor = RowGreen OrElse CurrentRowColor = HeaderGreen) Then 'Finished
                    Dim myRerun As Integer = -1
                    Dim myExecutionID As Integer = -1

                    If (Not pNewRow.IsExecutionIDNull) Then myExecutionID = pNewRow.ExecutionID 'This column is NULL for CALC/OFFS
                    If (Not pNewRow.IsRerunNumberNull) Then myRerun = pNewRow.RerunNumber 'This column is NULL for CALC/OFFS

                    If (IsExecutionWithAlarms(myExecutionID, pNewRow.OrderTestID, pNewRow.TestType, myRerun)) Then
                        dgv("Alarms", MaxRow).Value = FinishedWithErrorsImage
                        dgv("Alarms", MaxRow).ToolTipText = labelFinishedRemarks '"*Finished With Remarks*"
                    ElseIf (Not pNewRow.IsExecutionStatusNull AndAlso pNewRow.ExecutionStatus = "CLOSEDNOK") Then
                        dgv("Alarms", MaxRow).Value = FinishedWithErrorsImage
                        dgv("Alarms", MaxRow).ToolTipText = labelFinishedOpticalKO  '"*Finished With Optical Errors*"
                    ElseIf (Not pNewRow.IsExecutionStatusNull AndAlso pNewRow.ExecutionStatus = "LOCKED") Then
                        dgv("Alarms", MaxRow).Value = LockedImage
                        dgv("Alarms", MaxRow).ToolTipText = labelLocked 'Locked
                    End If

                ElseIf (Not pNewRow.IsExecutionStatusNull AndAlso pNewRow.ExecutionStatus <> "LOCKED") Then 'Evaluate using the replicate 1 ExecutionStatus
                    dgv("Alarms", MaxRow).Value = NoImage
                    dgv("Alarms", MaxRow).ToolTipText = String.Empty

                ElseIf (Not pNewRow.IsExecutionStatusNull AndAlso pNewRow.ExecutionStatus = "LOCKED") Then 'Evaluate using the replicate 1 ExecutionStatus
                    dgv("Alarms", MaxRow).Value = LockedImage
                    dgv("Alarms", MaxRow).ToolTipText = labelLocked
                End If

                'AG 12/03/2014 - #1524 Calculate value for header column
                Select Case dgv("Alarms", MaxRow).ToolTipText
                    Case labelLocked 'child LOCKED
                        currentHeaderAlarmsIcon = AlarmsIconValues.Locked  'At least 1 child locked then header shows lock icon

                    Case labelFinishedOpticalKO 'child CLOSED NOK
                        'If there is no child locked-> show optical errors
                        If currentHeaderAlarmsIcon <> AlarmsIconValues.Locked Then
                            currentHeaderAlarmsIcon = AlarmsIconValues.With_OpticalErrors
                        End If

                    Case labelFinishedRemarks 'child CLOSED OK with remarks
                        'If there is no child locked, with optical error and with remarks -> show OK
                        If currentHeaderAlarmsIcon <> AlarmsIconValues.Locked AndAlso currentHeaderAlarmsIcon <> AlarmsIconValues.With_OpticalErrors Then
                            currentHeaderAlarmsIcon = AlarmsIconValues.With_Remarks
                        End If

                    Case labelSamplePosStatusFinished 'child CLOSED OK withour remarks
                        'If there is no child locked, with optical error and with remarks -> show OK
                        If currentHeaderAlarmsIcon <> AlarmsIconValues.Locked AndAlso currentHeaderAlarmsIcon <> AlarmsIconValues.With_Remarks AndAlso _
                            currentHeaderAlarmsIcon <> AlarmsIconValues.With_OpticalErrors Then
                            currentHeaderAlarmsIcon = AlarmsIconValues.Finished_OK
                        End If
                End Select
                'AG 12/03/2014 - #1524

            End If

            'AG 12/03/2014 - #1524 - PrintAvailable
            If pNewRow.SampleClass = "PATIENT" Then
                If pNewRow.IsHeader Then
                    'Initialize empty, it will be calculated using their children
                    dgv("PrintAvailable", MaxRow).Value = NoImage
                    dgv("PrintAvailable", MaxRow).ToolTipText = labelReportPrintNOTAvailable
                    currentHeaderShowPrintedIconFlag = False

                Else
                    If pNewRow.IsPrintedNull Then pNewRow.Printed = False 'AG 19/03/2014 - Protection against DBNULL
                    If CurrentRowColor = RowGreen AndAlso Not pNewRow.Printed Then
                        dgv("PrintAvailable", MaxRow).Value = PrintAvailableImage
                        dgv("PrintAvailable", MaxRow).ToolTipText = labelReportPrintAvailable
                        currentHeaderShowPrintedIconFlag = True 'There is at least one child available to print

                    Else
                        dgv("PrintAvailable", MaxRow).Value = NoImage
                        dgv("PrintAvailable", MaxRow).ToolTipText = labelReportPrintNOTAvailable
                    End If
                End If

            Else 'Blanks, Calibs, Ctrols do not show available print icon
                currentHeaderShowPrintedIconFlag = False
            End If

            'Export icons columns
            If pNewRow.SampleClass = "PATIENT" OrElse pNewRow.SampleClass = "CTRL" Then
                If pNewRow.IsHeader Then
                    'Initialize empty, it will be calculated using their children
                    dgv("Exported", MaxRow).Value = NoImage
                    dgv("Exported", MaxRow).ToolTipText = labelHISNOTSent
                    currentHeaderShowExportIconFlag = False

                Else
                    If CurrentRowColor = RowGreen AndAlso pNewRow.ExportStatus = "SENT" Then
                        dgv("Exported", MaxRow).Value = HISSentImage
                        dgv("Exported", MaxRow).ToolTipText = labelHISSent
                        currentHeaderShowExportIconFlag = True 'There is at least one child sent to LIS

                    Else
                        dgv("Exported", MaxRow).Value = NoImage
                        dgv("Exported", MaxRow).ToolTipText = labelHISNOTSent
                    End If
                End If

            Else 'Blanks, Calibs do not show the sent to LIS icon
                currentHeaderShowExportIconFlag = False
            End If
            'AG 12/03/2014 - #1524

            'Absorbance time graph
            If (pNewRow.TestType = "STD") Then
                dgv("GraphIcon", MaxRow).ToolTipText = labelAbsorbanceCurve '"*Open Absorbance Curve*"
            Else
                dgv("GraphIcon", MaxRow).ToolTipText = String.Empty
            End If

            auxString = String.Empty
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DrawNewRow_NEW", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DrawNewRow_NEW", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

#End Region


End Class
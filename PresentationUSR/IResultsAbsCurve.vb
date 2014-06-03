Option Explicit On
Option Strict Off

Imports System.Threading
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
'Imports Biosystems.Ax00.DAL
Imports DevExpress.Utils
Imports DevExpress.XtraCharts
Imports DevExpress.XtraGrid.Views.Grid
Imports Biosystems.Ax00.CommunicationsSwFw
Imports DevExpress
Imports DevExpress.XtraGrid.Columns
Imports Biosystems.Ax00.Types.vwksWSAbsorbanceDS


Public Class IResultsAbsCurve

#Region "Declarations"

    Private Structure strOrderTest
        Public OrderTestID As Integer
        Public MultiItemNumber As Integer
        Public RerunNumber As Integer
    End Structure

    Private Const AllowDecimals As Integer = 4
    Private IntervalABS As Single = -1
    Private OrderTestGrouped As List(Of strOrderTest) 'RH 24/02/2012
    Private ReplicateDS As GraphDS

    '//Changes for TASK + BUGS Tracking  #1331
    '// CF - 14/10/2013 - V3.0.0 A new property is added in order to 
    '//calculate the value of the maximum cycle number in the graph. This value will be used to make the 
    '//graph's X axis longer according to the number of reads performed during the session. 
    Private MaxValueForXAxis As Integer = 75 '//We use the 75 value that we've had for versions < 2.1.1

    'Global variables for names of Icons needed 
    Private PATIENT_IconName As String = ""
    Private BLANK_IconName As String = ""
    Private CTRL_IconName As String = ""
    Private CALIB_IconName As String = ""

    'Global variables for names of Labels needed
    Private PATIENTID_Label As String = ""
    Private CTRL_Label As String = ""
    Private CALIB_Label As String = ""
    Private myReadingMode As String = ""
    Private CurrentID As Integer
    '
    'Global variables for chart control
    Private ReadOnly SizeMarker = 4
    Private ReadOnly SizeLineSelected = 3
    Private ReadOnly SizeLineNormal = 1
    Private ReadOnly KindMarker = MarkerKind.Cross
    '
    Private LBL_CYCLE As String = ""
    Private LBL_ABS As String = ""
    Private IsReplicateGroup As Boolean = True
    Private SerieNameSeleceted As String = ""
    Private ReadingSelected As Integer = -1

    'RH 28/02/2012
    Private Expanded As Boolean = False
    Private ExpandImage As Image = Nothing
    Private CollapseImage As Image = Nothing
    Private ReadOnly ExpandedSize As Size = New Size(974, 483)
    Private ReadOnly CollapsedSize As Size = New Size(693, 483)
    Private LanguageID As String
    Private myMultiLangResourcesDelegate As MultilanguageResourcesDelegate
    'END RH 28/02/2012

    Private AbsDS As vwksWSAbsorbanceDS
    Private SourceReplicate As Integer = -1

    'TR 16/03/2012 -Valiable used to store the initial position for controls.
    Private bsTestLabelLocation As Point
    Private bsTestTextLocation As Point
    Private bsRerunLabelLocation As Point
    Private bsRerunTextLocation As Point
    'TR 16/03/2012 -END

    'RH 19/03/2012
    Private refreshScreenResults As GlobalDataTO
    Private GettingDataForAbsCurve As Boolean = False
    Private Executions As List(Of vwksWSAbsorbanceDS.vwksWSAbsorbanceRow)
    Private ReadOnly myResultsFileDelegate As New ResultsFileDelegate()
    'END RH 19/03/2012
#End Region

#Region "Atributes"

    Private SourceCalledAttribute As GlobalEnumerates.GraphicalAbsScreenCallMode
    Private ExecutionIDAttribute As Integer
    Private OrderTestIDAttribute As Integer
    Private SampleClassAttribute As String
    Private SampleIDAttribute As String
    Private TestNameAttribute As String
    Private SampleTypeAttribute As String

    Private SourceFormAttribute As GlobalEnumerates.ScreenCallsGraphical
    Private ExecutionsAttribute As List(Of ExecutionsDS.vwksWSExecutionsMonitorRow)

    Private ReplicateAttribute As Integer = -1
    Private MultiItemNumberAttribute As Integer
    Private RerunAttribute As Integer
    Private WorkSessionIDAttribute As String
    Private AnalyzerIDAttribute As String

#End Region

#Region "Properties"

    Public WriteOnly Property Replicate() As Integer
        Set(ByVal value As Integer)
            ReplicateAttribute = value
        End Set
    End Property


    Public WriteOnly Property WorkSessionID() As String
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property

    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    Public WriteOnly Property TestName() As String
        Set(ByVal value As String)
            TestNameAttribute = value
        End Set
    End Property

    Public WriteOnly Property SampleClass() As String
        Set(ByVal value As String)
            SampleClassAttribute = value
        End Set
    End Property

    Public WriteOnly Property SampleID() As String
        Set(ByVal value As String)
            SampleIDAttribute = value
        End Set
    End Property

    Public WriteOnly Property SampleType() As String
        Set(ByVal value As String)
            SampleTypeAttribute = value
        End Set
    End Property

    Public WriteOnly Property OrderTestID() As Integer
        Set(ByVal value As Integer)
            OrderTestIDAttribute = value
        End Set
    End Property

    Public WriteOnly Property SourceForm() As GlobalEnumerates.ScreenCallsGraphical
        Set(ByVal value As GlobalEnumerates.ScreenCallsGraphical)
            SourceFormAttribute = value
        End Set
    End Property

    Public WriteOnly Property MultiItemNumber() As Integer
        Set(ByVal value As Integer)
            MultiItemNumberAttribute = value
        End Set
    End Property

    Public WriteOnly Property ReRun() As Integer
        Set(ByVal value As Integer)
            RerunAttribute = value
        End Set
    End Property

    Public WriteOnly Property ExecutionID() As Integer
        Set(ByVal value As Integer)
            ExecutionIDAttribute = value
        End Set
    End Property

    Public WriteOnly Property ListExecutions() As List(Of ExecutionsDS.vwksWSExecutionsMonitorRow)
        Set(ByVal value As List(Of ExecutionsDS.vwksWSExecutionsMonitorRow))
            ExecutionsAttribute = value
        End Set
    End Property

    Public WriteOnly Property SourceCalled() As GraphicalAbsScreenCallMode
        Set(ByVal value As GraphicalAbsScreenCallMode)
            SourceCalledAttribute = value
        End Set
    End Property
#End Region

#Region "Events"

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


    '''' <summary>
    '''' Draw points
    '''' </summary>
    '''' <remarks>
    '''' Created by: DL 10/02/2011
    '''' </remarks>
    Private Sub ResultChartControl_CustomDrawSeriesPoint(ByVal sender As Object, ByVal e As CustomDrawSeriesPointEventArgs) Handles ResultChartControl.CustomDrawSeriesPoint
        Try
            If Not IsReplicateGroup AndAlso SerieNameSeleceted <> "" AndAlso ReadingSelected > 0 Then

                'DL 18/11/2011
                ' BRING TO FRONT SERIE FROM POINT SELECTED
                Dim s As Series = ResultChartControl.Series(SerieNameSeleceted)
                ResultChartControl.SuspendLayout()
                Dim i As Integer = ResultChartControl.Series.IndexOf(s)

                If i > -1 AndAlso i <> ResultChartControl.Series.Count - 1 Then
                    ResultChartControl.Series.Swap(ResultChartControl.Series.Count - 1, i)
                End If
                '
                ResultChartControl.ResumeLayout()
                '
                'DL 18/11/2011

                If e.Series.Name = SerieNameSeleceted Then
                    If e.SeriesPoint.Argument = ReadingSelected Then
                        e.SeriesDrawOptions.Color = Color.Blue

                        'RH 24/02/2012
                        Dim ldo As LineDrawOptions = CType(e.SeriesDrawOptions, LineDrawOptions)
                        ldo.Marker.FillStyle.FillMode = FillMode.Solid
                        ldo.Marker.Kind = KindMarker
                        ldo.Marker.Size = SizeMarker
                    End If

                End If
            End If

            '//Changes for TASK + BUGS Tracking  #1331
            '//15/10/2013 - CF -v3 We check the "TAG" attribute, if it's true, then we change the marker kind for this point

            Dim drawOptions As PointDrawOptions = e.SeriesDrawOptions
            If (IsNothing(drawOptions)) Then
                Exit Sub
            End If

            If (e.SeriesPoint.Tag = True) Then
                DirectCast(e.SeriesDrawOptions, PointDrawOptions).Marker.Kind = MarkerKind.Triangle
                DirectCast(e.SeriesDrawOptions, PointDrawOptions).Marker.BorderColor = Color.Black
                'DirectCast(e.SeriesDrawOptions, PointDrawOptions).Marker.Kind = Kind.Diamond
            Else
                DirectCast(e.SeriesDrawOptions, PointDrawOptions).Marker.Kind = MarkerKind.Circle
            End If
            '//End modifications by CF

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChartControlSingle_CustomDrawSeriesPoint", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChartControlSingle_CustomDrawSeriesPoint", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    '''' <summary>
    '''' Click over grid control
    '''' </summary>
    '''' <remarks>
    '''' Created by: DL 27/10/2011
    '''' </remarks>
    Private Sub ReplicatesGridControl_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim View As GridView = ReplicatesGridView

        Try
            Dim rowHandle As Integer = View.FocusedRowHandle
            Dim mySerieName As String
            Dim myReplicate As String

            If View.RowCount > 0 Then
                'Get related data rows 
                If View.IsGroupRow(rowHandle) Then
                    Dim childHandle As Integer = View.GetChildRowHandle(rowHandle, 0)
                    myReplicate = ReplicatesGridView.GetRowCellValue(childHandle, "Replicate").ToString
                    mySerieName = myReplicate & ". " & FilterComboBox.Text

                    IsReplicateGroup = True
                    SerieNameSeleceted = ""
                    ReadingSelected = -1
                Else
                    myReplicate = View.GetRowCellValue(rowHandle, "Replicate").ToString
                    mySerieName = myReplicate & ". " & FilterComboBox.Text

                    IsReplicateGroup = False
                    SerieNameSeleceted = mySerieName
                    ReadingSelected = CInt(View.GetRowCellValue(rowHandle, "Cycle").ToString)
                End If

                SourceReplicate = CInt(myReplicate)
                RemoveHandler ReplicateUpDown.ValueChanged, AddressOf ReplicateUpDown_ValueChanged
                ReplicateUpDown.Value = SourceReplicate
                AddHandler ReplicateUpDown.ValueChanged, AddressOf ReplicateUpDown_ValueChanged

                If Not ResultChartControl.Series(mySerieName) Is Nothing Then
                    If CType(ResultChartControl.Series(mySerieName).View, LineSeriesView).LineStyle.Thickness <> SizeLineSelected Then
                        'unselect all series
                        For Each nSerie As Series In ResultChartControl.Series
                            CType(nSerie.View, LineSeriesView).LineStyle.Thickness = SizeLineNormal
                        Next nSerie

                        'select serie selected in replicate grid
                        CType(ResultChartControl.Series(mySerieName).View, LineSeriesView).LineStyle.Thickness = SizeLineSelected

                    End If
                End If

                If SerieNameSeleceted <> "" Then
                    ResultChartControl.Refresh()
                    ResultChartControl.RefreshData()
                    ResultChartControl.Update()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ReplicatesGridControl_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub ReplicatesGridControl_ProcessGridKey(ByVal e As System.Windows.Forms.KeyEventArgs)

        If ReplicatesGridView.IsGroupRow(ReplicatesGridView.FocusedRowHandle) Then Exit Sub

        If e.KeyCode = Keys.Down Then
            If ReplicatesGridView.FocusedRowHandle < ReplicatesGridView.DataRowCount - 1 Then
                ReplicatesGridView.FocusedRowHandle += 1
                ReplicatesGridControl_Click(Nothing, Nothing)
                e.Handled = True
            End If
        End If
        If e.KeyCode = Keys.Up Then
            If ReplicatesGridView.FocusedRowHandle > 0 Then
                ReplicatesGridView.FocusedRowHandle -= 1
                ReplicatesGridControl_Click(Nothing, Nothing)
                e.Handled = True
            End If
        End If
    End Sub

    ''' <summary>
    ''' Show tooltip when select a point in the chart
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/10/2011
    ''' Modified by RH 28/02/2012 Code optimization.
    ''' </remarks>
    Private Sub ResultChartControl_ObjectHotTracked(ByVal sender As Object, ByVal e As HotTrackEventArgs) Handles ResultChartControl.ObjectHotTracked
        Try
            Dim myPoint As SeriesPoint = TryCast(e.AdditionalObject, SeriesPoint)

            If Not myPoint Is Nothing Then
                Cursor = Cursors.Hand

                Dim mySerie As Series = TryCast(e.Object, Series)
                Dim SerieToArray() As String = mySerie.Name.Split(".")
                Dim Replicate As String = SerieToArray(0)
                Dim Cycle As String = myPoint.NumericalArgument.ToString("#0.####")
                Dim Absorbance As String = myPoint.Values(0).ToString("#0.####")
                Dim ToolTip As New System.Text.StringBuilder()

                ToolTip.AppendFormat("{0} Replicate: {1}{2}", SerieToArray(1).Trim(), Replicate, vbCrLf)
                ToolTip.AppendFormat("Cycle: {0} Absorbance: {1}", Cycle, Absorbance)

                ToolTipController1.ShowHint(ToolTip.ToString())
            Else
                Cursor = Cursors.Default
                ToolTipController1.HideHint()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChartControlSingle_ObjectHotTracked", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChartControlSingle_ObjectHotTracked", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Show char when change value
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 24/10/2011
    ''' </remarks>
    Private Sub ChangeValue(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AllReplicatesRadioButton.CheckedChanged, ReplicateRadioButton.CheckedChanged
        Try
            If AllReplicatesRadioButton.Checked Then
                ReplicateUpDown.Enabled = False
            Else
                If Not ReplicateDS Is Nothing AndAlso ReplicateDS.tReplicates.Count > 0 Then
                    ' Calculate min and max value for replicate updown
                    'Dim Min As Integer = (From row In ReplicateDS.tReplicates Select CType(row.Replicate, Integer?)).Min
                    'Dim Max As Integer = (From row In ReplicateDS.tReplicates Select CType(row.Replicate, Integer?)).Max

                    'RH 24/02/2012
                    Dim Min As Integer = (From row In ReplicateDS.tReplicates Select row.Replicate).Min()
                    Dim Max As Integer = (From row In ReplicateDS.tReplicates Select row.Replicate).Max()

                    ReplicateUpDown.Minimum = Min
                    ReplicateUpDown.Maximum = Max

                    If SourceReplicate = -1 Then
                        SourceReplicate = Min
                    ElseIf SourceReplicate > Max Then 'DL 30/05/2012
                        SourceReplicate = ReplicateUpDown.Maximum 'DL 30/05/2012
                    ElseIf SourceReplicate < Min Then 'DL 30/05/2012
                        SourceReplicate = ReplicateUpDown.Minimum 'DL 30/05/2012
                    End If

                    ReplicateUpDown.Value = SourceReplicate '1 'ReplicateAttribute 'descomentar
                    ReplicateUpDown.Enabled = True
                End If
            End If

            RefreshGraphType()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeValue", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeValue", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '''' <summary>
    '''' Expand chart and hide cgrid control
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>DL 21/10/2011</remarks>
    'Private Sub bsExpandButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExpandButton.Click
    '    Try
    '        'Resize chart control
    '        ResultChartControl.Size = New Size(974, 483)

    '        New location for a navaigation button
    '        bsLastButton.Location = New Point(892, 650)
    '        bsNextButton.Location = New Point(855, 650)
    '        bsPreviousButton.Location = New Point(818, 650)
    '        bsFirstButton.Location = New Point(781, 650)
    '        bsPrintButton.Location = New Point(744, 650)

    '        ReplicatesGridControl.Visible = False
    '        bsCollapseButton.Visible = True
    '        bsExpandButton.Visible = False

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExpandButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".bsExpandButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub

    '''' <summary>
    '''' Collapse chart. Set original size and show grid control
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks>DL 21/10/2011</remarks>
    'Private Sub bsCollapseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCollapseButton.Click
    '    Try
    '        'Resize chart control
    '        ResultChartControl.Size = New Size(693, 483)

    '        New location for a navaigation button
    '        bsLastButton.Location = New Point(643, 650)
    '        bsNextButton.Location = New Point(606, 650)
    '        bsPreviousButton.Location = New Point(569, 650)
    '        bsFirstButton.Location = New Point(532, 650)
    '        bsPrintButton.Location = New Point(495, 650)

    '        ReplicatesGridControl.Visible = True
    '        bsCollapseButton.Visible = False
    '        bsExpandButton.Visible = True

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCollapseButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".bsCollapseButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
    '    End Try
    'End Sub

    ''' <summary>
    ''' Expands or collapses the chart, and hides or shows the grid control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by RH 28/02/2012
    ''' </remarks>
    Private Sub bsExpandButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExpandButton.Click
        Try
            Expanded = Not Expanded

            If Expanded Then
                ResultChartControl.Size = ExpandedSize
                bsExpandButton.Image = CollapseImage
                ReplicatesGridControl.Visible = False
            Else
                ResultChartControl.Size = CollapsedSize
                bsExpandButton.Image = ExpandImage
                ReplicatesGridControl.Visible = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsExpandButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsExpandButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Double click in chart control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>DL 21/10/2011</remarks>
    Private Sub ResultChartControl_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResultChartControl.DoubleClick
        Try
            'If bsExpandButton.Visible = True Then
            '    bsExpandButton_Click(Nothing, Nothing)
            'Else
            '    bsCollapseButton_Click(Nothing, Nothing)
            'End If

            'RH 28/02/2012
            bsExpandButton_Click(Nothing, Nothing)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ResultChartControl_DoubleClick", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ResultChartControl_DoubleClick", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Double click in chart control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>DL 21/10/2011</remarks>
    Private Sub FilterComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FilterComboBox.SelectedIndexChanged

        Try
            Dim mySerieName As String
            Dim mySerie As Series
            Dim Min As Single
            Dim Max As Single
            Dim FilterAbs As List(Of GraphDS.tReplicatesRow)

            If Not ReplicateDS Is Nothing AndAlso ReplicateDS.tReplicates.Count > 0 Then
                Dim qReplicateFilter As List(Of GraphDS.tReplicatesRow)

                CreateChartControl()

                If AllReplicatesRadioButton.Checked Then

                    ReplicatesGridView.Columns("Replicate").FilterInfo = New ColumnFilterInfo("[Replicate] = *")

                    Dim myGraph As String = FilterComboBox.Text
                    Dim qReplicates As List(Of Integer) = (From row As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                                           Select row.Replicate).Distinct.ToList()


                    ' Calculate min and max value for replicate updown
                    Select Case myGraph
                        Case "Abs1"

                            FilterAbs = (From row In ReplicateDS.tReplicates Where Not row.IsAbs1Null AndAlso row.Abs1 <> "Error" Select row).ToList()

                            If FilterAbs.Count > 0 Then
                                Min = (From row In ReplicateDS.tReplicates _
                                       Where Not row.IsAbs1Null AndAlso row.Abs1 <> "Error" _
                                       Select CType(row.Abs1, Single)).Min

                                Max = (From row In ReplicateDS.tReplicates _
                                       Where Not row.IsAbs1Null AndAlso row.Abs1 <> "Error" _
                                       Select CType(row.Abs1, Single)).Max
                            End If

                        Case "Abs2"

                            FilterAbs = (From row In ReplicateDS.tReplicates Where Not row.IsAbs2Null AndAlso row.Abs2 <> "Error" Select row).ToList()

                            If FilterAbs.Count > 0 Then
                                Min = (From row In ReplicateDS.tReplicates _
                                       Where Not row.IsAbs2Null AndAlso row.Abs2 <> "Error" _
                                       Select CType(row.Abs2, Single)).Min

                                Max = (From row In ReplicateDS.tReplicates _
                                       Where Not row.IsAbs2Null AndAlso row.Abs2 <> "Error" _
                                       Select CType(row.Abs2, Single)).Max
                            End If


                    End Select

                    For iReplicate As Integer = 0 To qReplicates.Count - 1
                        mySerieName = qReplicates(iReplicate) & ". " & myGraph

                        Select Case myGraph
                            Case "Abs1"
                                mySerie = CreateSerie(mySerieName, "Abs1")

                                'qReplicateFilter = (From row As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                '                    Where row.Replicate = CType(qReplicates(iReplicate), Integer) AndAlso Not row.IsAbs1Null _
                                '                    Select row).ToList

                                'RH 24/02/2012 Do not convert apples into apples
                                qReplicateFilter = (From row As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                                    Where row.Replicate = qReplicates(iReplicate) AndAlso Not row.IsAbs1Null _
                                                    Select row).ToList

                            Case "Abs2"
                                mySerie = CreateSerie(mySerieName, "Abs2")

                                'qReplicateFilter = (From row As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                '                    Where row.Replicate = CType(qReplicates(iReplicate), Integer) AndAlso Not row.IsAbs2Null _
                                '                    Select row).ToList

                                'RH 24/02/2012 Do not convert apples into apples
                                qReplicateFilter = (From row As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                                    Where row.Replicate = qReplicates(iReplicate) AndAlso Not row.IsAbs2Null _
                                                    Select row).ToList

                        End Select

                        For i As Integer = 0 To qReplicateFilter.Count - 1
                            Dim tmpSeriesPoint As SeriesPoint
                            '//Changes for TASK + BUGS Tracking  #1331
                            '//15/10/2013 CF V3. Change the way that the points are initialized in order to store the 
                            '// "Pause" value so that the graph draws these points differently. 
                            '//tmpSeriesPoint will be used to set the TAG and then add it to the collection of points. 
                            Select Case myGraph
                                Case "Abs1"
                                    If String.Equals(qReplicateFilter(i).Abs1, "Error") Then
                                        tmpSeriesPoint = New SeriesPoint(qReplicateFilter(i).Cycle)
                                        'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle))
                                        'mySerie.Points.Add(tmpSeriesPoint)
                                    Else
                                        tmpSeriesPoint = New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs1)
                                        'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs1))
                                        'mySerie.Points.Add(tmpSeriesPoint)
                                    End If
                                    tmpSeriesPoint.Tag = qReplicateFilter(i).Pause
                                    mySerie.Points.Add(tmpSeriesPoint)

                                Case "Abs2"
                                    If String.Equals(qReplicateFilter(i).Abs2, "Error") Then
                                        tmpSeriesPoint = New SeriesPoint(qReplicateFilter(i).Cycle)
                                        'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle).Tag = qReplicateFilter(i).Pause)
                                    Else
                                        tmpSeriesPoint = New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs2)
                                        'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs2))
                                    End If
                                    tmpSeriesPoint.Tag = qReplicateFilter(i).Pause
                                    mySerie.Points.Add(tmpSeriesPoint)

                            End Select

                        Next i

                        ResultChartControl.Series.AddRange(New Series() {mySerie})

                    Next iReplicate

                    CreateDiagram(Min, Max)

                ElseIf ReplicateRadioButton.Checked Then

                    'DL 31/01/2012. Check if exist the replicate 
                    Dim existReplicate As Boolean = (From row In ReplicateDS.tReplicates _
                                          Where row.Replicate = SourceReplicate _
                                          Select row.Replicate).Any

                    If Not existReplicate Then
                        If ReplicateDS.tReplicates.Count > 0 Then
                            'SourceReplicate 
                            'SourceReplicate = (From row In ReplicateDS.tReplicates _
                            '                   Select CType(row.Replicate, Integer?)).Max

                            'RH 24/02/2012 Do not convert apples into apples
                            SourceReplicate = (From row In ReplicateDS.tReplicates _
                                               Select row.Replicate).Max()

                        Else
                            SourceReplicate = -1
                        End If
                    End If
                    'DL 31/01/2012

                    RemoveHandler ReplicateUpDown.ValueChanged, AddressOf ReplicateUpDown_ValueChanged
                    ReplicateUpDown.Value = SourceReplicate
                    AddHandler ReplicateUpDown.ValueChanged, AddressOf ReplicateUpDown_ValueChanged

                    ReplicatesGridView.Columns("Replicate").FilterInfo = New ColumnFilterInfo("[Replicate] = " & SourceReplicate)

                    mySerieName = CStr(SourceReplicate)

                    Select Case FilterComboBox.Text()

                        Case "Abs1"

                            FilterAbs = (From row In ReplicateDS.tReplicates Where Not row.IsAbs1Null AndAlso row.Abs1 <> "Error" Select row).ToList()

                            If FilterAbs.Count > 0 Then
                                Min = (From row In ReplicateDS.tReplicates _
                                       Where Not row.IsAbs1Null AndAlso row.Abs1 <> "Error" AndAlso row.Replicate = ReplicateUpDown.Value _
                                       Select CType(row.Abs1, Single?)).Min

                                Max = (From row In ReplicateDS.tReplicates _
                                       Where Not row.IsAbs1Null AndAlso row.Abs1 <> "Error" AndAlso row.Replicate = ReplicateUpDown.Value _
                                       Select CType(row.Abs1, Single?)).Max
                            End If


                            qReplicateFilter = (From row As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                                Where row.Replicate = ReplicateUpDown.Value AndAlso Not row.IsAbs1Null _
                                                Select row).ToList

                            mySerieName &= ". Abs1"
                            mySerie = CreateSerie(mySerieName, "Abs1")
                            Dim tmpSeriePoint As SeriesPoint
                            '//Changes for TASK + BUGS Tracking  #1331
                            '//15/10/2013 CF V3. Change the way that the points are initialized in order to store the 
                            '// "Pause" value so that the graph draws these points differently. 
                            '//tmpSeriesPoint will be used to set the TAG and then add it to the collection of points. 
                            For i As Integer = 0 To qReplicateFilter.Count - 1
                                If qReplicateFilter(i).Abs1 = "Error" Then
                                    tmpSeriePoint = New SeriesPoint(qReplicateFilter(i).Cycle)
                                    'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle))
                                Else
                                    tmpSeriePoint = New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs1)
                                    'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs1))
                                End If
                                tmpSeriePoint.Tag = qReplicateFilter(i).Pause
                                mySerie.Points.Add(tmpSeriePoint)
                            Next i

                            ResultChartControl.Series.AddRange(New Series() {mySerie})

                        Case "Abs2"

                            FilterAbs = (From row In ReplicateDS.tReplicates Where Not row.IsAbs2Null AndAlso row.Abs2 <> "Error" Select row).ToList()

                            If FilterAbs.Count > 0 Then
                                Min = (From row In ReplicateDS.tReplicates _
                                       Where Not row.IsAbs2Null AndAlso row.Abs2 <> "Error" AndAlso row.Replicate = ReplicateUpDown.Value _
                                       Select CType(row.Abs2, Single?)).Min

                                Max = (From row In ReplicateDS.tReplicates _
                                       Where Not row.IsAbs2Null AndAlso row.Abs2 <> "Error" AndAlso row.Replicate = ReplicateUpDown.Value _
                                       Select CType(row.Abs2, Single?)).Max
                            End If

                            qReplicateFilter = (From row As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                                Where row.Replicate = ReplicateUpDown.Value AndAlso Not row.IsAbs2Null _
                                                Select row).ToList

                            mySerieName &= ". Abs2"
                            mySerie = CreateSerie(mySerieName, "Abs2")
                            '//Changes for TASK + BUGS Tracking  #1331
                            '//15/10/2013 CF V3. Change the way that the points are initialized in order to store the 
                            '// "Pause" value so that the graph draws these points differently. 
                            '//tmpSeriesPoint will be used to set the TAG and then add it to the collection of points. 
                            Dim tmpSeriePoint As SeriesPoint
                            For i As Integer = 0 To qReplicateFilter.Count - 1
                                If qReplicateFilter(i).Abs2 = "Error" Then
                                    tmpSeriePoint = New SeriesPoint(qReplicateFilter(i).Cycle)
                                    'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle))
                                Else
                                    tmpSeriePoint = New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs2)
                                    'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs2))
                                End If
                                tmpSeriePoint.Tag = qReplicateFilter(i).Pause
                                mySerie.Points.Add(tmpSeriePoint)
                            Next i

                            ResultChartControl.Series.AddRange(New Series() {mySerie})

                        Case "Abs1 & Abs2"

                            FilterAbs = (From row In ReplicateDS.tReplicates Where Not row.IsAbs1Null AndAlso row.Abs1 <> "Error" Select row).ToList()

                            If FilterAbs.Count > 0 Then
                                Min = (From row In ReplicateDS.tReplicates _
                                       Where Not row.IsAbs1Null AndAlso row.Abs1 <> "Error" AndAlso row.Replicate = ReplicateUpDown.Value _
                                       Select CType(row.Abs1, Single?)).Min

                                Max = (From row In ReplicateDS.tReplicates _
                                       Where Not row.IsAbs1Null AndAlso row.Abs1 <> "Error" AndAlso row.Replicate = ReplicateUpDown.Value _
                                       Select CType(row.Abs1, Single?)).Max
                            End If


                            FilterAbs = (From row In ReplicateDS.tReplicates Where Not row.IsAbs2Null AndAlso row.Abs2 <> "Error" Select row).ToList()

                            Dim Min1 As Single
                            Dim Max1 As Single

                            If FilterAbs.Count > 0 Then
                                Min1 = (From row In ReplicateDS.tReplicates _
                                        Where Not row.IsAbs2Null AndAlso row.Abs2 <> "Error" AndAlso row.Replicate = ReplicateUpDown.Value _
                                        Select CType(row.Abs2, Single?)).Min

                                Max1 = (From row In ReplicateDS.tReplicates _
                                        Where Not row.IsAbs2Null AndAlso row.Abs2 <> "Error" AndAlso row.Replicate = ReplicateUpDown.Value _
                                        Select CType(row.Abs2, Single?)).Max
                            End If

                            If Min > Min1 Then Min = Min1
                            If Max < Max1 Then Max = Max1

                            qReplicateFilter = (From row As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                                Where row.Replicate = ReplicateUpDown.Value AndAlso Not row.IsAbs1Null _
                                                Select row).ToList

                            mySerieName &= ". Abs1"
                            mySerie = CreateSerie(mySerieName, "Abs1")
                            Dim tmpSeriesPoint As SeriesPoint
                            '//Changes for TASK + BUGS Tracking  #1331
                            '//15/10/2013 CF V3. Change the way that the points are initialized in order to store the 
                            '// "Pause" value so that the graph draws these points differently. 
                            '//tmpSeriesPoint will be used to set the TAG and then add it to the collection of points. 
                            For i As Integer = 0 To qReplicateFilter.Count - 1
                                If qReplicateFilter(i).Abs1 = "Error" Then
                                    tmpSeriesPoint = New SeriesPoint(qReplicateFilter(i).Cycle)
                                    'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle))
                                Else
                                    tmpSeriesPoint = New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs1)
                                    'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs1))
                                End If
                                tmpSeriesPoint.Tag = qReplicateFilter(i).Pause
                                mySerie.Points.Add(tmpSeriesPoint)
                            Next i
                            '
                            qReplicateFilter = (From row As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                                Where row.Replicate = ReplicateUpDown.Value AndAlso Not row.IsAbs2Null _
                                                Select row).ToList

                            Dim mySerieAux As Series
                            mySerieName &= ". Abs2"
                            mySerieAux = CreateSerie(mySerieName, "Abs2")
                            '//Changes for TASK + BUGS Tracking  #1331
                            '//15/10/2013 CF V3. Change the way that the points are initialized in order to store the 
                            '// "Pause" value so that the graph draws these points differently. 
                            '//tmpSeriesPoint will be used to set the TAG and then add it to the collection of points. 
                            For i As Integer = 0 To qReplicateFilter.Count - 1
                                tmpSeriesPoint = New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs2)
                                tmpSeriesPoint.Tag = qReplicateFilter(i).Pause
                                mySerieAux.Points.Add(tmpSeriesPoint)
                                'mySerieAux.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Abs2))
                            Next i

                            ResultChartControl.Series.AddRange(New Series() {mySerie, mySerieAux})

                        Case "Abs1 - Abs2"

                            Min = (From row In ReplicateDS.tReplicates _
                                   Where Not row.IsDiffNull AndAlso row.Replicate = ReplicateUpDown.Value _
                                   Select CType(row.Diff, Single?)).Min

                            Max = (From row In ReplicateDS.tReplicates _
                                   Where Not row.IsDiffNull AndAlso row.Replicate = ReplicateUpDown.Value _
                                   Select CType(row.Diff, Single?)).Max

                            qReplicateFilter = (From row As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                                Where row.Replicate = ReplicateUpDown.Value AndAlso Not row.IsDiffNull _
                                                Select row).ToList

                            mySerieName &= ". Diff"
                            mySerie = CreateSerie(mySerieName, "Diff")
                            '//Changes for TASK + BUGS Tracking  #1331
                            '//15/10/2013 CF V3. Change the way that the points are initialized in order to store the 
                            '// "Pause" value so that the graph draws these points differently. 
                            '//tmpSeriesPoint will be used to set the TAG and then add it to the collection of points. 
                            Dim tmpSeriesPoint As SeriesPoint
                            For i As Integer = 0 To qReplicateFilter.Count - 1
                                tmpSeriesPoint = New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Diff)
                                tmpSeriesPoint.Tag = qReplicateFilter(i).Pause
                                mySerie.Points.Add(tmpSeriesPoint)
                                'mySerie.Points.Add(New SeriesPoint(qReplicateFilter(i).Cycle, qReplicateFilter(i).Diff))
                            Next i

                            ResultChartControl.Series.AddRange(New Series() {mySerie})
                    End Select
                    CreateDiagram(Min, Max)

                End If

            Else
                ResultChartControl.Series.Clear()

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FilterComboBox_SelectedIndexChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FilterComboBox_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub ReplicateUpDown_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReplicateUpDown.ValueChanged

        If ReplicateUpDown.Value >= ReplicateUpDown.Minimum And ReplicateUpDown.Value <= ReplicateUpDown.Maximum Then
            SourceReplicate = ReplicateUpDown.Value

            FilterComboBox_SelectedIndexChanged(Nothing, Nothing)
        End If

    End Sub
    ''' <summary>
    ''' Changes for TASK + BUGS Tracking  #1331
    ''' This method will format the cells in the grid right before it is displayed. 
    ''' This event is fired for every cell in the grid.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by CF - 14/10/2013 - v3.0.0</remarks>
    Private Sub ReplicatesGridView_CustomDrawCell(ByVal sender As System.Object, ByVal e As DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs) Handles ReplicatesGridView.CustomDrawCell
        Dim currentView As GridView = CType(sender, GridView)

        '//CF: Add this "IF" statement if only one of the row's cells has to change colour. Use the FieldName property to filter. 
        If (e.RowHandle = currentView.FocusedRowHandle) Then Exit Sub
        Dim r As Rectangle = e.Bounds '//Get the cell's size
        Dim paused As Boolean = currentView.GetRowCellValue(e.RowHandle, currentView.Columns("Pause")) '//Check the pause column for true or false
        If (paused) Then '//if true-> change the colour of the current cell. 
            Dim colorBrush = Brushes.LightGray
            e.Graphics.FillRectangle(colorBrush, r)
            e.Appearance.DrawString(e.Cache, e.DisplayText, r) '//This line is required in order to display the cell value correctly. 
            e.Handled = True
        End If
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 02/11/2011
    ''' </remarks>
    Private Sub IResultsAbsCurve_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown

        Try
            If (e.KeyCode = Keys.Escape) Then
                bsCloseButton.PerformClick()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IResultsAbsCurve_KeyDown", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IResultsAbsCurve_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    Private Sub GraphAbsorbance_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim StartTime As DateTime = Now

        Try

            ReplicatesGridControl.GetType.InvokeMember("DoubleBuffered", _
                                                        Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.SetProperty, _
                                                        Nothing, _
                                                        ReplicatesGridControl, _
                                                        New Object() {True})

            'TR 16/03/2012
            bsTestLabelLocation = bsTestLabel.Location
            bsTestTextLocation = bsTestText.Location
            bsRerunLabelLocation = bsRerunLabel.Location
            bsRerunTextLocation = bsRerunText.Location

            'RH 29/02/2012 Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            LanguageID = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

            'RH 29/02/2012 Initialize myMultiLangResourcesDelegate
            myMultiLangResourcesDelegate = New MultilanguageResourcesDelegate()

            Cursor = Cursors.WaitCursor

            PrepareButtons()                    'Load images
            GetScreenLabels()    'Load the multilanguage texts for all Screen Labels
            InitializeABSInterval()

            InitializeGridControl()             'Initialize Grid Control

            GetData()

            If ReplicateAttribute = -1 Then
                If AllReplicatesRadioButton.Checked Then
                    ChangeValue(Nothing, Nothing)
                Else
                    AllReplicatesRadioButton.Checked = True
                End If

            Else
                SourceReplicate = ReplicateAttribute
                ReplicateRadioButton.Checked = True
            End If

            CheckButtons()

        Catch ex As Exception
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GraphAbsorbance_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GraphAbsorbance_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        Finally
            Cursor = Cursors.Default

            'Dim ElapsedTime As Double = Now.Subtract(StartTime).TotalMilliseconds
            'MessageBox.Show("GraphAbsorbance_Load Elapsed Time: " & ElapsedTime.ToStringWithDecimals(0))

        End Try
    End Sub

    ''' <summary>
    ''' Move to last chart
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 10/02/2011
    ''' </remarks>
    Private Sub LastButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsLastButton.Click
        Try
            Cursor = Cursors.WaitCursor

            'CurrentID = UBound(OrderTestGrouped)
            'RH 24/02/2012
            CurrentID = OrderTestGrouped.Count - 1

            CheckButtons()
            FindData()

        Catch ex As Exception
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".LastButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".LastButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub

    ''' <summary>
    ''' Navigate to Next Execution Identifier
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 10/02/2011
    ''' </remarks>
    Private Sub NextButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNextButton.Click
        Try
            Cursor = Cursors.WaitCursor
            CurrentID += 1
            CheckButtons()
            FindData()

        Catch ex As Exception
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".NextButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".NextButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try

    End Sub

    ''' <summary>
    ''' Navigate to Previous Execution Identifier
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 10/02/2011
    ''' </remarks>
    Private Sub PreviousButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPreviousButton.Click
        Try
            Cursor = Cursors.WaitCursor

            CurrentID -= 1
            CheckButtons()
            FindData()

        Catch ex As Exception
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PreviousButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PreviousButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub

    ''' <summary>
    ''' Navigate to first chart
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 10/02/2011
    ''' </remarks>
    Private Sub FirstButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsFirstButton.Click
        Try
            Cursor = Cursors.WaitCursor

            CurrentID = 0
            CheckButtons()
            FindData()

        Catch ex As Exception
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FirstButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FirstButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try

    End Sub

    Private Sub PrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            '' Preview
            '' Check whether the ChartControl can be previewed.
            'If Not DXChartControl.IsPrintingAvailable Then
            '    MessageBox.Show("The 'DevExpress.XtraPrinting.v7.2.dll' is not found", "Error")
            '    Return

            'Else
            '    'Dim ps As New DevExpress.XtraPrinting.PrintingSystem()
            '    Dim composLink As New CompositeLink(New PrintingSystem())
            '    Dim pcLink1 As New PrintableComponentLink()
            '    Dim pcLink2 As New PrintableComponentLink()

            '    pcLink1.Component = bsPointsList
            '    pcLink2.Component = DXChartControl

            '    composLink.Links.Add(pcLink2)
            '    composLink.Links.Add(pcLink1)
            '    composLink.Margins.Left = 10
            '    composLink.Margins.Right = 10
            '    composLink.ShowPreviewDialog()
            'End If

            '' Opens the Preview window.
            ''DXChartControl.ShowPrintPreview()

            '' print

            ' '' Check whether the ChartControl can be printed.
            ''If Not DXChartControl.IsPrintingAvailable Then
            ''    MessageBox.Show("The 'DevExpress.XtraPrinting.v7.2.dll' is not found", "Error")
            ''    Return
            ''End If

            ' '' Print.
            ''DXChartControl.Print()

        Catch ex As Exception
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrintButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Close form
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 10/02/2011
    ''' </remarks>
    Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCloseButton.Click
        Try
            Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CloseButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CloseButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Updates and refreshes the chart control in real time
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 28/02/2012 based on a previous version by DL.
    '''             Code optimization. Convert most method code into a delegate method.
    '''             RH 19/03/2012 Parallel gathering of data for Abs curve
    '''             AG 13/06/2012 - simplify and try improve code performance
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), _
                                       ByVal pRefreshDS As UIRefreshDS)
        Try
            If isClosingFlag Then Return 'AG 03/08/2012

            Dim myLogAcciones As New ApplicationLogManager()
            Dim StartTime As DateTime = Now 'AG 13/06/2012 - time estimation
            Dim qUIRefresh As List(Of vwksWSAbsorbanceDS.vwksWSAbsorbanceRow)
            Dim workingThread As New Threading.Thread(AddressOf GetDataForAbsCurve)

            Executions = New List(Of vwksWSAbsorbanceDS.vwksWSAbsorbanceRow)

            For Each rowRefresh As UIRefreshDS.ReceivedReadingsRow In pRefreshDS.ReceivedReadings.Rows
                qUIRefresh = (From row In AbsDS.vwksWSAbsorbance _
                              Where row.ExecutionID = rowRefresh.ExecutionID _
                              AndAlso row.OrderTestID = OrderTestIDAttribute _
                              AndAlso row.RerunNumber = RerunAttribute _
                              AndAlso row.MultiItemNumber = MultiItemNumberAttribute _
                              Order By row.ReplicateNumber _
                              Select row Distinct).ToList()

                If qUIRefresh.Count > 0 Then
                    Executions.Add(qUIRefresh.First())
                End If
            Next rowRefresh

            GettingDataForAbsCurve = True

            workingThread.Start()

            While GettingDataForAbsCurve
                Application.DoEvents() 'Make the User Interface be responsiveness
                Thread.Sleep(100)
            End While

            If Not refreshScreenResults.HasError AndAlso Not refreshScreenResults.SetDatos Is Nothing Then
                Dim myReplicateDS As GraphDS = CType(refreshScreenResults.SetDatos, GraphDS)

                If myReplicateDS.tReplicates.Count > 0 Then
                    Dim changes As Boolean = False 'AG 13/06/2012
                    If Not ReplicateDS Is Nothing Then
                        'AG 13/06/2012 - This loop is very wide (max = 68 x Replicates number in screenx 1 linq by iteration)
                        ''DL CODE
                        'Dim Replicates As Integer = 0
                        'For Each row As GraphDS.tReplicatesRow In myReplicateDS.tReplicates
                        '    'Search this point inside the current values in ReplicateDS
                        '    Replicates = (From qRow As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                        '                  Where qRow.ExecutionID = row.ExecutionID _
                        '                  AndAlso qRow.Replicate = row.Replicate _
                        '                  AndAlso qRow.Cycle = row.Cycle _
                        '                  Select qRow).Count

                        '    If Replicates = 0 Then 'Insert the new point
                        '        ReplicateDS.tReplicates.ImportRow(row)
                        '        'AG 14/06/2012
                        '        'ReplicatesGridControl.RefreshDataSource()
                        '        'Application.DoEvents() 'Make the User Interface be responsiveness
                        '        changes = True
                        '    End If
                        'Next

                        'If changes Then 'AG 14/06/2012
                        '    ReplicatesGridControl.RefreshDataSource()
                        '    Application.DoEvents() 'Make the User Interface be responsiveness
                        'End If
                        ''DL CODE

                        'AG CODE - To test
                        'Search only the new points and insert them (max items in loop 68)
                        Dim executionCurrentLastReading As Integer = 0
                        Dim toInsertRow As List(Of GraphDS.tReplicatesRow)
                        Dim maxReadings As List(Of GraphDS.tReplicatesRow)
                        For Each GraphRow As vwksWSAbsorbanceDS.vwksWSAbsorbanceRow In Executions
                            If ReplicateDS.tReplicates.Rows.Count > 0 Then
                                maxReadings = (From qRow As GraphDS.tReplicatesRow In ReplicateDS.tReplicates _
                                                Where qRow.ExecutionID = GraphRow.ExecutionID _
                                                AndAlso qRow.Replicate = GraphRow.ReplicateNumber _
                                                Select qRow Order By qRow.Cycle Descending).ToList

                                If maxReadings.Count > 0 Then
                                    'Get the current last reading cycle in graph (max value)
                                    executionCurrentLastReading = maxReadings(0).Cycle

                                    'Search for the next reading cycle
                                    toInsertRow = (From a As GraphDS.tReplicatesRow In myReplicateDS.tReplicates _
                                                    Where a.ExecutionID = GraphRow.ExecutionID _
                                                    AndAlso a.Replicate = GraphRow.ReplicateNumber _
                                                    AndAlso a.Cycle = executionCurrentLastReading + 1 Select a).ToList

                                Else
                                    toInsertRow = (From a As GraphDS.tReplicatesRow In myReplicateDS.tReplicates _
                                                    Where a.ExecutionID = GraphRow.ExecutionID _
                                                    AndAlso a.Replicate = GraphRow.ReplicateNumber Select a).ToList
                                End If

                                'If exists then Import it
                                If toInsertRow.Count > 0 Then
                                    ReplicateDS.tReplicates.ImportRow(toInsertRow(0))
                                    changes = True
                                End If

                            Else
                                ReplicateDS = myReplicateDS
                                ReplicatesGridControl.DataSource = ReplicateDS.tReplicates
                                changes = True
                            End If
                        Next

                        If changes Then

                            ReplicatesGridControl.RefreshDataSource()
                            Application.DoEvents() 'Make the User Interface be responsiveness
                        End If
                        'AG 13/06/2012
                        'AG CODE

                    Else
                        ReplicateDS = myReplicateDS
                        ReplicatesGridControl.DataSource = ReplicateDS.tReplicates
                    End If
                    GetMaxValueForXAxisFromReplicatesDS(ReplicateDS)
                    FilterComboBox_SelectedIndexChanged(Nothing, Nothing)
                End If

            Else
                'AG 15/10/2012 - when error show the message error to presentation
                If refreshScreenResults.HasError AndAlso refreshScreenResults.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString Then
                    ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, refreshScreenResults.ErrorMessage.ToString)
                End If

            End If

            myLogAcciones.CreateLogActivity("Refresh Abs(t) Graph screen: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "IResultsAbsCurve.RefreshScreen", EventLogEntryType.Information, False) 'AG 04/07/2012

        Catch ex As Exception
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".RefreshScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Cursor = Cursors.Default

        End Try
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Gets data for Abs curve
    ''' </summary>
    ''' <remarks>RH 19/03/2012</remarks>
    Private Sub GetDataForAbsCurve()
        refreshScreenResults = myResultsFileDelegate.GetDataForAbsCurve(Nothing,
                                                                        OrderTestIDAttribute,
                                                                        RerunAttribute, _
                                                                        MultiItemNumberAttribute,
                                                                        Executions,
                                                                        AllowDecimals)

        GettingDataForAbsCurve = False
    End Sub

    ''' <summary>
    ''' Create chart control
    ''' </summary>
    ''' <remarks>DL 21/10/2011</remarks>
    Private Sub CreateChartControl()
        Try
            'If bsExpandButton.Visible Then
            '    ResultChartControl.Size = New Size(693, 483)            'Change the Graphic control size
            'Else
            '    ResultChartControl.Size = New Size(974, 483)
            'End If

            'RH 28/02/2012
            If Expanded Then
                ResultChartControl.Size = ExpandedSize            'Change the Graphic control size
            Else
                ResultChartControl.Size = CollapsedSize
            End If

            ResultChartControl.Location = New Point(8, 15)          'Set the location 

            ' Specify data members to bind the chart's series template.
            ResultChartControl.RefreshDataOnRepaint = False
            ResultChartControl.CacheToMemory = True
            ResultChartControl.RuntimeHitTesting = False
            ResultChartControl.Legend.Visible = False

            ResultChartControl.ClearCache()
            ResultChartControl.Series.Clear()
            ResultChartControl.BackColor = Color.White
            ResultChartControl.AppearanceName = "Light"


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CreateChartControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CreateChartControl", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Create new Serie
    ''' </summary>
    ''' <param name="pName">Serie name</param>
    ''' <param name="pDataMember">Data member name</param>
    ''' <remarks>DL 25/10/2011</remarks>
    Private Function CreateSerie(ByVal pName As String, ByVal pDataMember As String) As Series
        Dim mySerie As Series = Nothing

        Try
            mySerie = New Series(pName, ViewType.Line)
            mySerie.Name = pName
            mySerie.Label.Visible = False
            mySerie.PointOptions.PointView = PointView.ArgumentAndValues
            mySerie.ArgumentDataMember = "Cycle"
            mySerie.ArgumentScaleType = ScaleType.Numerical
            mySerie.ValueScaleType = ScaleType.Numerical
            mySerie.ShowInLegend = False
            mySerie.Label.Antialiasing = False
            mySerie.ValueDataMembers.AddRange(New String() {pDataMember})  'Absorbance

            Dim myView As XtraCharts.LineSeriesView
            myView = TryCast(mySerie.View, XtraCharts.LineSeriesView)
            myView.LineMarkerOptions.Size = SizeMarker

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CreateSerie ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CreateSerie ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        Return mySerie
    End Function

    ''' <summary>
    ''' Create Diagram
    ''' </summary>
    ''' <param name="Min">Min value</param>
    ''' <param name="Max">Max value</param>
    ''' <remarks>DL 25/10/2011</remarks>
    Private Sub CreateDiagram(ByVal Min As Single, ByVal Max As Single)
        Try

            'Dim myDiagram As New XYDiagram
            Dim myDiagram As XYDiagram

            myDiagram = CType(ResultChartControl.Diagram, XYDiagram)

            With myDiagram
                'my Customize the appearance of the Y-axis title.
                .AxisY.Title.Text = LBL_ABS
                .AxisY.Title.Visible = True
                .AxisY.Title.Alignment = StringAlignment.Center
                .AxisY.Title.TextColor = Color.Black ' Color.Blue
                .AxisY.Title.Antialiasing = True
                .AxisY.Title.Font = New Font("Verdana", 8, FontStyle.Regular)

                ' Customize the appearance of the X-axis title.
                .AxisX.Title.Text = LBL_CYCLE
                .AxisX.Title.Visible = True
                .AxisX.Title.Alignment = StringAlignment.Center
                .AxisX.Title.TextColor = Color.Black 'Color.Red
                .AxisX.Title.Antialiasing = True
                .AxisX.Title.Font = New Font("Verdana", 8, FontStyle.Regular)
                '
                If IntervalABS = -1 Then
                    .AxisY.Range.Auto = True
                Else
                    .AxisY.Range.SetMinMaxValues(Min - IntervalABS, Max + IntervalABS)
                End If

                '//Changes for TASK + BUGS Tracking  #1331
                '//14/10/2013 - cf - v3.0.0 -  Added the MaxValueForXAxis variable to adjust the graph's limits. 

                .AxisX.Range.SetMinMaxValues(0, MaxValueForXAxis + 5)

                .AxisY.Visible = True
            End With

            ' Set some properties to get a nice-looking chart.
            'CType(ResultChartControl.Diagram, XYDiagram).AxisY.Visible = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CreateDiagram ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CreateDiagram ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

    ''' <summary>
    ''' Initialize ABS Interval
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 24/02/2011
    ''' </remarks>
    Private Sub InitializeABSInterval()
        Try
            'Get the decimal separator.
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

            'Get value of General Setting containing the maximum number of Patient Order Tests that can be created

            Dim myUserSettingsDelegate As New GeneralSettingsDelegate
            Dim myGlobalDataTO As GlobalDataTO
            myGlobalDataTO = myUserSettingsDelegate.GetGeneralSettingValue(Nothing, GlobalEnumerates.GeneralSettingsEnum.INTERVAL_ABS_T.ToString)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                'Save value in global variable maxPatientOrderTests
                Dim myNewValue As String = DirectCast(myGlobalDataTO.SetDatos, String)

                If myDecimalSeparator = "." Then
                    If myNewValue.ToString.Contains(",") Then
                        myNewValue = myNewValue.ToString.Replace(",", myDecimalSeparator)
                        IntervalABS = CType(myNewValue, Single)
                    Else
                        IntervalABS = CType(myNewValue, Single)
                    End If

                ElseIf myDecimalSeparator = "," Then
                    If myNewValue.ToString.Contains(".") Then
                        myNewValue = myNewValue.ToString.Replace(".", myDecimalSeparator)
                        IntervalABS = CType(myNewValue, Single)
                    Else
                        IntervalABS = CType(myNewValue, Single)
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeABSInterval", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeABSInterval", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Modify navigations buttons states
    ''' </summary>
    ''' <remarks>
    ''' Created by : DL 10/02/2011
    ''' Modified by: DL 14/02/2011
    ''' </remarks>
    Private Sub CheckButtons()
        Try
            Dim LimitMax As Integer
            Dim CurrentIndex As Integer = CurrentID

            If AbsDS Is Nothing Then
                LimitMax = 0
            Else
                'LimitMax = UBound(OrderTestGrouped)
                LimitMax = OrderTestGrouped.Count - 1 'RH 24/02/2012

            End If

            Select Case CurrentIndex
                Case 0
                    bsPreviousButton.Enabled = False
                    bsFirstButton.Enabled = False

                    If LimitMax = 0 Or LimitMax = 1 Then
                        bsNextButton.Enabled = False
                        bsLastButton.Enabled = False
                    ElseIf LimitMax > 1 Then
                        bsNextButton.Enabled = True
                        bsLastButton.Enabled = True
                    End If

                Case (LimitMax)
                    bsNextButton.Enabled = False
                    bsLastButton.Enabled = False
                    bsPreviousButton.Enabled = False
                    bsFirstButton.Enabled = False

                    If LimitMax > 1 Then
                        bsPreviousButton.Enabled = True
                        bsFirstButton.Enabled = True
                    End If

                Case Else
                    bsNextButton.Enabled = True
                    bsPreviousButton.Enabled = True
                    bsFirstButton.Enabled = True
                    bsLastButton.Enabled = True
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CheckButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' '<param name="pLanguageID"> The current Language of Application </param>
    ''' Created by:  DL 21/02/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels.....
            bsLotLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", LanguageID)
            bsGraphResultLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_RESULTSABSCURVE", LanguageID)
            bsGraphTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Graph_Type", LanguageID)
            ReplicateRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Rep", LanguageID)
            BsShowLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Graph", LanguageID)
            bsSampleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleClass_Short", LanguageID) + ":"
            bsTestLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", LanguageID) + ":"
            bsSampleTypeLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", LanguageID) + ":"
            bsRerunLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rerun", LanguageID) + ":"
            BsReplicateLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Rep", LanguageID) + ":"

            BsWellLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Well", LanguageID) + ":"
            bsMultiItemLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Kit", LanguageID)

            BsShowLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SHOW", LanguageID)

            bsGraphToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", LanguageID))
            bsGraphToolTips.SetToolTip(bsFirstButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToFirst", LanguageID))
            bsGraphToolTips.SetToolTip(bsNextButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToNext", LanguageID))
            bsGraphToolTips.SetToolTip(bsPreviousButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToPrevious", LanguageID))
            bsGraphToolTips.SetToolTip(bsLastButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_GoToLast", LanguageID))

            'EF 29/08/2013 - Bugtracking 1272 - Change label text by 'Sample' in v2.1.1
            'PATIENTID_Label = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", LanguageID) + _
            '                    "/" + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorPos_SampleID", LanguageID) + ":"
            PATIENTID_Label = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", LanguageID) + _
                               "/" + myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_SampleVolume", LanguageID) + ":"
            'EF 29/08/2013

            CTRL_Label = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", LanguageID) + ":"
            CALIB_Label = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibratorName", LanguageID) + ":"

            AllReplicatesRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AllReplicates", LanguageID)

            LBL_ABS = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", LanguageID)
            LBL_CYCLE = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Cycles", LanguageID)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Method incharge to load the image for each graphical button 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 05/05/2010
    ''' Modified by: SA 03/11/2010 - Set value of Image Property instead of BackgroundImage Property; 
    '''                              removed Icon in button Search Tests
    '''              SA 18/01/2011 - Get OffSystem Tests Icon for the new button to opening the auxiliary screen 
    '''                              to add results for this type of Tests
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = IconsPath

            ' close Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsCloseButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            ' print button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            ' next button
            auxIconName = GetIconName("RIGHT")
            If auxIconName <> "" Then
                bsNextButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            ' previous button
            auxIconName = GetIconName("LEFT")
            If auxIconName <> "" Then
                bsPreviousButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            ' Last Button
            auxIconName = GetIconName("FORWARDL")
            If auxIconName <> "" Then
                bsLastButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'First Button
            auxIconName = GetIconName("BACKWARDL")
            If auxIconName <> "" Then
                bsFirstButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'Expand Button
            auxIconName = GetIconName("FORWARD")
            If auxIconName <> "" Then
                bsExpandButton.Image = Image.FromFile(iconPath & auxIconName)
                ExpandImage = Image.FromFile(iconPath & auxIconName)
            End If

            'Collapse Button
            auxIconName = GetIconName("BACKWARD")
            If auxIconName <> "" Then
                bsCollapseButton.Image = Image.FromFile(iconPath & auxIconName)
                CollapseImage = Image.FromFile(iconPath & auxIconName)
            End If

            PATIENT_IconName = GetIconName("ROUTINES")
            CALIB_IconName = GetIconName("CALIB")
            CTRL_IconName = GetIconName("CTRL")
            BLANK_IconName = GetIconName("BLANK")

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub



    ''' <summary>
    ''' Initialize grid control columns
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 24/10/2011
    ''' </remarks>
    Private Sub InitializeGridControl()
        Try
            ReplicatesGridView.Columns.Clear()

            Dim ReplicateColumn As New GridColumn()
            Dim CycleColumn As New GridColumn()
            Dim Abs1Column As New GridColumn()
            Dim Abs2Column As New GridColumn()
            Dim DiffColumn As New GridColumn()
            Dim ExecutionIDColumn As New GridColumn()
            '//Changes for TASK + BUGS Tracking  #1331
            '//14/10/2013 - CF - v3.0.0 Added the new hidden Pause column which will be used to draw the cells 
            '//in a different colour
            Dim PauseColumn As New GridColumn()

            ReplicatesGridView.Columns.AddRange(New GridColumn() _
                        {ReplicateColumn, CycleColumn, Abs1Column, _
                         Abs2Column, DiffColumn, ExecutionIDColumn, PauseColumn})

            ReplicatesGridView.OptionsView.AllowCellMerge = False
            ReplicatesGridView.OptionsView.GroupDrawMode = GroupDrawMode.Default
            ReplicatesGridView.OptionsView.ShowGroupedColumns = False
            ReplicatesGridView.OptionsView.ColumnAutoWidth = True
            ReplicatesGridView.OptionsView.RowAutoHeight = True
            ReplicatesGridView.OptionsView.ShowIndicator = False

            ReplicatesGridView.Appearance.Row.TextOptions.VAlignment = VertAlignment.Center
            ReplicatesGridView.Appearance.Row.TextOptions.HAlignment = HorzAlignment.Far

            'GridView1.Appearance.FocusedRow.ForeColor = Color.White
            'GridView1.Appearance.FocusedRow.BackColor = Color.LightSlateGray
            'SamplesXtraGridView.Appearance.GroupRow.BackColor = Color.WhiteSmoke
            'SamplesXtraGridView.Appearance.GroupRow.ForeColor = Color.Black
            'SamplesXtraGridView.Appearance.FocusedCell.BackColor = Color.Transparent
            'SamplesXtraGridView.Appearance.GroupButton.BackColor = Color.Transparent

            ReplicatesGridView.OptionsHint.ShowColumnHeaderHints = False
            ReplicatesGridView.OptionsBehavior.Editable = False
            ReplicatesGridView.OptionsBehavior.ReadOnly = True
            ReplicatesGridView.OptionsCustomization.AllowFilter = False
            ReplicatesGridView.OptionsCustomization.AllowSort = False

            'GridView1.OptionsSelection.EnableAppearanceFocusedRow = True
            'GridView1.OptionsSelection.MultiSelect = False
            'ReplicatesGridView.ColumnPanelRowHeight = 30

            ReplicatesGridView.GroupCount = 1

            'Replicate column
            ReplicateColumn.FieldName = "Replicate"
            ReplicateColumn.Name = "Replicate"
            ReplicateColumn.GroupIndex = 0

            'Cycle column
            CycleColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Tests_Cycle", LanguageID)
            CycleColumn.FieldName = "Cycle"
            CycleColumn.Name = "Cycle"
            CycleColumn.Visible = True

            'Abs1 column
            Abs1Column.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Abs1_Short", LanguageID)
            Abs1Column.FieldName = "Abs1"
            Abs1Column.Name = "Abs1"
            Abs1Column.Visible = True

            'Abs2 column
            Abs2Column.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Abs2_Short", LanguageID)
            Abs2Column.FieldName = "Abs2"
            Abs2Column.Name = "Abs2"
            Abs2Column.Visible = True

            'Diff column
            DiffColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveReplicate_Diff_Short", LanguageID)
            DiffColumn.FieldName = "Diff"
            DiffColumn.Name = "Diff"
            DiffColumn.Visible = True

            'Diff column
            ExecutionIDColumn.FieldName = "ExecutionID"
            ExecutionIDColumn.Name = "ExecutionID"

            'Pause column
            PauseColumn.FieldName = "Pause"
            PauseColumn.Name = "Pause"



        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "InitializeGridControl", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Show header data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/10/2011
    ''' Modified by: SA 14/06/2012 - Call function GetByOrderTestID instead of GetByTestIDAndOrderTestID (both in ControlsDelegate)
    ''' </remarks>
    Private Sub ShowHeaderData(ByVal pAbsRow As vwksWSAbsorbanceRow)
        Try
            If Not pAbsRow.SampleClass = "BLANK" Then
                'TR 16/03/2012 -Change the locations for controls.
                bsTestLabel.Location = New Point(bsTestLabelLocation)
                bsTestText.Location = New Point(bsTestTextLocation)
                bsRerunLabel.Location = New Point(bsRerunLabelLocation)
                bsRerunText.Location = New Point(bsRerunTextLocation)
                'TR 16/03/2012 -END
            End If

            'Set head panel data
            Select Case pAbsRow.SampleClass
                Case "PATIENT"

                    'Set Visible or not header fields
                    bsComodinLabel.Visible = True
                    bsComodinText.Visible = True
                    bsLotLabel.Visible = False
                    bsLotText.Visible = False
                    bsMultiItemLabel.Visible = False
                    bsMultiItemText.Visible = False
                    bsSampleTypeLabel.Visible = True
                    bsSampleTypeText.Visible = True
                    bsRerunText.Visible = True
                    bsRerunLabel.Visible = True

                    'Load Data
                    bsComodinLabel.Text = PATIENTID_Label
                    If Not pAbsRow.IsSampleIDNull Then
                        bsComodinText.Text = pAbsRow.SampleID
                    Else
                        bsComodinText.Text = String.Empty
                    End If

                    bsLotText.Text = String.Empty
                    bsMultiItemText.Text = String.Empty 'CType(.MultiItemNumber, String)
                    bsTestText.Text = pAbsRow.TestName
                    bsSampleTypeText.Text = pAbsRow.SampleType
                    bsRerunText.Text = pAbsRow.RerunNumber.ToString()

                    'Load image in sample class
                    bsClassPictureBox.ImageLocation = IconsPath & PATIENT_IconName

                Case "BLANK"

                    'TR 16/03/2012 -Change the locations for controls.
                    bsTestLabel.Location = bsComodinLabel.Location
                    bsTestText.Location = bsComodinText.Location

                    bsRerunLabel.Location = bsLotLabel.Location
                    bsRerunText.Location = bsLotText.Location
                    'TR 16/03/2012 -END

                    'Set Visible or not header fields
                    bsComodinLabel.Visible = False
                    bsComodinText.Visible = False
                    bsLotLabel.Visible = False
                    bsLotText.Visible = False
                    bsMultiItemLabel.Visible = False
                    bsMultiItemText.Visible = False
                    bsSampleTypeLabel.Visible = False
                    bsSampleTypeText.Visible = False
                    bsRerunText.Visible = True
                    bsRerunLabel.Visible = True

                    'Load Data
                    bsComodinLabel.Text = PATIENTID_Label
                    If Not pAbsRow.IsSampleIDNull Then
                        bsComodinText.Text = pAbsRow.SampleID
                    Else
                        bsComodinText.Text = String.Empty
                    End If

                    bsLotText.Text = String.Empty
                    bsMultiItemText.Text = String.Empty 'CType(.MultiItemNumber, String)
                    bsTestText.Text = pAbsRow.TestName
                    bsSampleTypeText.Text = String.Empty
                    bsRerunText.Text = pAbsRow.RerunNumber.ToString()


                    'Load image in sample class
                    bsClassPictureBox.ImageLocation = IconsPath & BLANK_IconName

                Case "CALIB"
                    'Set Visible or not header fields
                    bsComodinLabel.Visible = True
                    bsComodinText.Visible = True
                    bsLotLabel.Visible = True
                    bsLotText.Visible = True
                    bsMultiItemLabel.Visible = True
                    bsMultiItemText.Visible = True
                    bsSampleTypeLabel.Visible = True
                    bsSampleTypeText.Visible = True
                    bsRerunText.Visible = True
                    bsRerunLabel.Visible = True

                    'Load Data

                    ' Load Calibrator Name and lot 
                    bsComodinLabel.Text = CALIB_Label
                    bsComodinText.Text = String.Empty
                    bsLotText.Text = String.Empty

                    Dim myGlobalDataTO As GlobalDataTO
                    Dim myTestCalibratorsDelgate As New TestCalibratorsDelegate
                    Dim myTestCalibratorsDS As TestCalibratorsDS

                    myGlobalDataTO = myTestCalibratorsDelgate.GetTestCalibratorByTestID(Nothing, pAbsRow.TestID, pAbsRow.SampleType)

                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        myTestCalibratorsDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)
                        If myTestCalibratorsDS.tparTestCalibrators.Count > 0 Then
                            bsComodinText.Text = myTestCalibratorsDS.tparTestCalibrators.First.CalibratorName
                            bsLotText.Text = myTestCalibratorsDS.tparTestCalibrators.First.LotNumber
                        End If
                    End If
                    '
                    bsMultiItemText.Text = pAbsRow.MultiItemNumber.ToString()
                    bsTestText.Text = pAbsRow.TestName
                    bsSampleTypeText.Text = pAbsRow.SampleType
                    bsRerunText.Text = pAbsRow.RerunNumber.ToString()

                    'Load image in sample class
                    bsClassPictureBox.ImageLocation = IconsPath & CALIB_IconName

                Case "CTRL"
                    'Set Visible or not header fields
                    bsComodinLabel.Visible = True
                    bsComodinText.Visible = True
                    bsLotLabel.Visible = True
                    bsLotText.Visible = True
                    bsMultiItemLabel.Visible = False
                    bsMultiItemText.Visible = False
                    bsSampleTypeLabel.Visible = True
                    bsSampleTypeText.Visible = True
                    bsRerunText.Visible = True
                    bsRerunLabel.Visible = True

                    'Load Data
                    bsComodinLabel.Text = CTRL_Label
                    bsComodinText.Text = String.Empty
                    bsLotText.Text = String.Empty

                    Dim myGlobalDataTO As GlobalDataTO
                    Dim myControlsDelegate As New ControlsDelegate
                    Dim myControlsDS As ControlsDS

                    myGlobalDataTO = myControlsDelegate.GetByOrderTestID(Nothing, pAbsRow.OrderTestID)
                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        myControlsDS = DirectCast(myGlobalDataTO.SetDatos, ControlsDS)

                        If myControlsDS.tparControls.Count > 0 Then
                            bsComodinText.Text = myControlsDS.tparControls.First.ControlName
                            bsLotText.Text = myControlsDS.tparControls.First.LotNumber
                        End If
                    End If

                    bsMultiItemText.Text = String.Empty 'CType(.MultiItemNumber, String)
                    bsTestText.Text = pAbsRow.TestName
                    bsSampleTypeText.Text = pAbsRow.SampleType
                    bsRerunText.Text = pAbsRow.RerunNumber.ToString()

                    'Load image in sample class
                    bsClassPictureBox.ImageLocation = IconsPath & CTRL_IconName
            End Select

            myReadingMode = pAbsRow.ReadingMode
            RefreshGraphType()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ShowHeaderdata", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub


    ''' <summary>
    ''' Show header data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 21/10/2011
    ''' Modified by: SA 14/06/2012 - Call function GetByOrderTestID instead of GetByTestIDAndOrderTestID (both in ControlsDelegate)
    ''' </remarks>
    Private Sub ShowHeaderWithoutData(ByVal rowExecution As ExecutionsDS.vwksWSExecutionsMonitorRow)
        Try
            'Set head panel data
            Select Case rowExecution.SampleClass
                Case "PATIENT"

                    'Set Visible or not header fields
                    bsComodinLabel.Visible = True
                    bsComodinText.Visible = True
                    bsLotLabel.Visible = False
                    bsLotText.Visible = False
                    bsMultiItemLabel.Visible = False
                    bsMultiItemText.Visible = False
                    bsSampleTypeLabel.Visible = True
                    bsSampleTypeText.Visible = True
                    bsRerunText.Visible = True
                    bsRerunLabel.Visible = True

                    'Load Data
                    bsComodinLabel.Text = PATIENTID_Label
                    If Not rowExecution.IsSampleIDNull Then
                        bsComodinText.Text = rowExecution.SampleID
                    Else
                        bsComodinText.Text = String.Empty
                    End If

                    bsLotText.Text = String.Empty
                    bsMultiItemText.Text = String.Empty 'CType(.MultiItemNumber, String)
                    bsTestText.Text = rowExecution.TestName
                    bsSampleTypeText.Text = rowExecution.SampleType
                    bsRerunText.Text = CStr(rowExecution.RerunNumber)

                    'Load image in sample class
                    bsClassPictureBox.ImageLocation = IconsPath & PATIENT_IconName

                Case "BLANK"
                    'Set Visible or not header fields
                    bsComodinLabel.Visible = False
                    bsComodinText.Visible = False
                    bsLotLabel.Visible = False
                    bsLotText.Visible = False
                    bsMultiItemLabel.Visible = False
                    bsMultiItemText.Visible = False
                    bsSampleTypeLabel.Visible = False
                    bsSampleTypeText.Visible = False
                    bsRerunText.Visible = True
                    bsRerunLabel.Visible = True

                    'Load Data
                    bsComodinLabel.Text = PATIENTID_Label
                    If Not rowExecution.IsSampleIDNull Then
                        bsComodinText.Text = rowExecution.SampleID
                    Else
                        bsComodinText.Text = String.Empty
                    End If

                    bsLotText.Text = String.Empty
                    bsMultiItemText.Text = String.Empty 'CType(.MultiItemNumber, String)
                    bsTestText.Text = rowExecution.TestName
                    bsSampleTypeText.Text = String.Empty
                    bsRerunText.Text = CStr(rowExecution.RerunNumber)

                    'Load image in sample class
                    bsClassPictureBox.ImageLocation = IconsPath & BLANK_IconName

                Case "CALIB"
                    'Set Visible or not header fields
                    bsComodinLabel.Visible = True
                    bsComodinText.Visible = True
                    bsLotLabel.Visible = True
                    bsLotText.Visible = True
                    bsMultiItemLabel.Visible = True
                    bsMultiItemText.Visible = True
                    bsSampleTypeLabel.Visible = True
                    bsSampleTypeText.Visible = True
                    bsRerunText.Visible = True
                    bsRerunLabel.Visible = True

                    'Load Data

                    ' Load Calibrator Name and lot 
                    bsComodinLabel.Text = CALIB_Label
                    bsComodinText.Text = String.Empty
                    bsLotText.Text = String.Empty

                    Dim myGlobalDataTO As New GlobalDataTO
                    Dim myTestCalibratorsDelgate As New TestCalibratorsDelegate
                    Dim myTestCalibratorsDS As New TestCalibratorsDS

                    myGlobalDataTO = myTestCalibratorsDelgate.GetTestCalibratorByTestID(Nothing, rowExecution.TestID, rowExecution.SampleType)

                    If Not myGlobalDataTO.HasError Then
                        myTestCalibratorsDS = DirectCast(myGlobalDataTO.SetDatos, TestCalibratorsDS)
                        If myTestCalibratorsDS.tparTestCalibrators.Count > 0 Then
                            bsComodinText.Text = myTestCalibratorsDS.tparTestCalibrators.First.CalibratorName
                            bsLotText.Text = myTestCalibratorsDS.tparTestCalibrators.First.LotNumber
                        End If
                    End If
                    '
                    bsMultiItemText.Text = CType(rowExecution.MultiItemNumber, String)
                    bsTestText.Text = rowExecution.TestName
                    bsSampleTypeText.Text = rowExecution.SampleType
                    bsRerunText.Text = CStr(rowExecution.RerunNumber)

                    'Load image in sample class
                    bsClassPictureBox.ImageLocation = IconsPath & CALIB_IconName

                Case "CTRL"
                    'Set Visible or not header fields
                    bsComodinLabel.Visible = True
                    bsComodinText.Visible = True
                    bsLotLabel.Visible = True
                    bsLotText.Visible = True
                    bsMultiItemLabel.Visible = False
                    bsMultiItemText.Visible = False
                    bsSampleTypeLabel.Visible = True
                    bsSampleTypeText.Visible = True
                    bsRerunText.Visible = True
                    bsRerunLabel.Visible = True

                    'Load Data
                    bsComodinLabel.Text = CTRL_Label
                    bsComodinText.Text = String.Empty
                    bsLotText.Text = String.Empty

                    Dim myGlobalDataTO As New GlobalDataTO
                    Dim myControlsDelegate As New ControlsDelegate
                    Dim myControlsDS As New ControlsDS

                    myGlobalDataTO = myControlsDelegate.GetByOrderTestID(Nothing, rowExecution.OrderTestID)
                    If Not myGlobalDataTO.HasError Then
                        myControlsDS = DirectCast(myGlobalDataTO.SetDatos, ControlsDS)

                        If myControlsDS.tparControls.Count > 0 Then
                            bsComodinText.Text = myControlsDS.tparControls.First.ControlName
                            bsLotText.Text = myControlsDS.tparControls.First.LotNumber
                        End If
                    End If

                    bsMultiItemText.Text = String.Empty 'CType(.MultiItemNumber, String)
                    bsTestText.Text = rowExecution.TestName
                    bsSampleTypeText.Text = rowExecution.SampleType
                    bsRerunText.Text = CStr(rowExecution.RerunNumber)

                    'Load image in sample class
                    bsClassPictureBox.ImageLocation = IconsPath & CTRL_IconName
            End Select

            ''myReadingMode = rowExecution.ReadingMode
            'ReplicateDS = New GraphDS

            ''DL 13/02/2012
            'If Not ReplicateDS Is Nothing AndAlso ReplicateDS.tReplicates.Rows.Count > 0 Then
            '    ReplicatesGridControl.DataSource = ReplicateDS.tReplicates
            '    RefreshGraphType()
            'End If
            ''DL 13/02/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "ShowHeaderdata", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    ''' <summary>
    ''' Refredh graph type
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 21/10/2011
    ''' </remarks>
    Private Sub RefreshGraphType()
        Try
            'Set values in filter combo 
            FilterComboBox.Items.Clear()
            FilterComboBox.Enabled = True

            If myReadingMode = "BIC" Then
                FilterComboBox.Items.Add("Abs1")
                FilterComboBox.Items.Add("Abs2")

                If ReplicateRadioButton.Checked Then
                    FilterComboBox.Items.Add("Abs1 & Abs2")
                    FilterComboBox.Items.Add("Abs1 - Abs2")
                End If

            ElseIf myReadingMode = "MONO" Then
                FilterComboBox.Items.Add("Abs1")
            End If

            If FilterComboBox.Items.Count > 0 Then FilterComboBox.SelectedIndex = 0

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "RefreshaGraphType", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub



    ''' <summary>
    ''' Show data
    ''' </summary>
    ''' <param name="pOrderTestID">Order test identifier</param>
    ''' <param name="pRerunNumber">Rerun Number</param>
    ''' <param name="pMultiItemNumber">MultiItem Number</param>
    ''' <remarks>
    ''' DL 24/10/2011
    ''' Modified by: RH 27/02/2012 Code optimization. Convert most method code into a delegate method.
    ''' </remarks>
    Private Sub ShowData(ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, ByVal pMultiItemNumber As Integer)
        Try
            'Dim StartTime As DateTime = Now
            Dim myResultsFileDelegate As New ResultsFileDelegate
            Dim myGlobalDataTO As GlobalDataTO

            IsReplicateGroup = True
            SerieNameSeleceted = String.Empty
            ReadingSelected = -1

            ResultChartControl.Series.Clear()

            Dim qExecutions As List(Of vwksWSAbsorbanceDS.vwksWSAbsorbanceRow)

            'DL 15/05/2012
            'Select Case SourceFormAttribute
            '    Case GlobalEnumerates.ScreenCallsGraphical.RESULTSFRM
            'qExecutions = (From row As vwksWSAbsorbanceDS.vwksWSAbsorbanceRow In AbsDS.vwksWSAbsorbance _
            '               Where row.OrderTestID = pOrderTestID _
            '               AndAlso row.RerunNumber = pRerunNumber _
            '               AndAlso row.MultiItemNumber = pMultiItemNumber _
            '               AndAlso (row.ExecutionStatus = "CLOSED" OrElse row.ExecutionStatus = "CLOSEDNOK") _
            '               Select row).ToList()

            'Case ScreenCallsGraphical.WS_STATES
            'Case Else
            qExecutions = (From row As vwksWSAbsorbanceDS.vwksWSAbsorbanceRow In AbsDS.vwksWSAbsorbance _
                           Where row.OrderTestID = pOrderTestID _
                           AndAlso row.RerunNumber = pRerunNumber _
                           AndAlso row.MultiItemNumber = pMultiItemNumber _
                           AndAlso (row.ExecutionStatus = "CLOSED" OrElse row.ExecutionStatus = "CLOSEDNOK" OrElse row.ExecutionStatus = "INPROCESS") _
                           Select row).ToList()
            'End Select
            'DL 15/05/2012

            myGlobalDataTO = myResultsFileDelegate.GetDataForAbsCurve(Nothing, pOrderTestID, pRerunNumber, _
                                                                      pMultiItemNumber, qExecutions, AllowDecimals)

            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                ReplicateDS = CType(myGlobalDataTO.SetDatos, GraphDS)
                ReplicatesGridControl.DataSource = ReplicateDS.tReplicates
            Else
                ReplicatesGridControl.DataSource = Nothing

                'DL 18/07/2013. Bug #1187
                ReplicateDS = Nothing
                ResultChartControl.Series.Clear()
                'DL 18/07/2013

                If myGlobalDataTO.HasError AndAlso myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString Then
                    ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, myGlobalDataTO.ErrorMessage.ToString)
                End If

            End If
            '//Changes for TASK + BUGS Tracking  #1331
            '//14/10/2013 - CF - v3.0.0 -Call this method to get the X Axis MAX value
            GetMaxValueForXAxisFromReplicatesDS(ReplicateDS)
            'Dim ElapsedTime As Double = Now.Subtract(StartTime).TotalMilliseconds
            'MessageBox.Show("ShowData Elapsed Time: " & ElapsedTime.ToStringWithDecimals(0))

        Catch ex As Exception
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ShowData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowData", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
    ''' <summary>
    ''' Changes for TASK + BUGS Tracking  #1331
    ''' 14/10/2013 - CF - v3.0.0 - Added this sub to get the max cycle count from a replicates DS, this will be used to adjust
    ''' the length of the XAxis in the graph.
    ''' </summary>
    ''' <param name="ReplicateDS">Replicate dataset of type GraphDS</param>
    ''' <remarks>Will set the property MaxValueForXAxis to 75 if it fails or if the DS is empty</remarks>
    Private Sub GetMaxValueForXAxisFromReplicatesDS(ByVal ReplicateDS As GraphDS)
        Try
            If (Not IsNothing(ReplicateDS)) Then
                Dim dr As DataRow = ReplicateDS.Tables(0).Select("Cycle = MAX(Cycle)").FirstOrDefault()
                If (dr IsNot Nothing) Then
                    MaxValueForXAxis = dr("Cycle")
                End If
            Else
                MaxValueForXAxis = 75
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetMaxValueForXAxisFromReplicatesDS", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MaxValueForXAxis = 75
        End Try
    End Sub
    ''' <summary>
    ''' Find current Order
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 24/02/2012 Optimization on a previous version by DL.
    ''' </remarks>
    Private Sub FindData()
        Try
            'Search in ds the information about saved ordesrtestid
            Dim DataFound As Boolean = False
            Dim RowList As List(Of vwksWSAbsorbanceDS.vwksWSAbsorbanceRow)

            Dim OrderTestID As Integer = OrderTestGrouped(CurrentID).OrderTestID
            Dim MultiItemNumber As Integer = OrderTestGrouped(CurrentID).MultiItemNumber
            Dim RerunNumber As Integer = OrderTestGrouped(CurrentID).RerunNumber

            Select Case SourceFormAttribute
                Case ScreenCallsGraphical.RESULTSFRM

                    RowList = (From row As vwksWSAbsorbanceDS.vwksWSAbsorbanceRow In AbsDS.vwksWSAbsorbance _
                               Where row.OrderTestID = OrderTestID _
                               AndAlso row.MultiItemNumber = MultiItemNumber _
                               AndAlso row.RerunNumber = RerunNumber _
                               Select row).ToList()

                Case ScreenCallsGraphical.WS_STATES, ScreenCallsGraphical.CURVEFRM

                    RowList = (From row As vwksWSAbsorbanceDS.vwksWSAbsorbanceRow In AbsDS.vwksWSAbsorbance _
                               Where row.OrderTestID = OrderTestID _
                               AndAlso row.MultiItemNumber = MultiItemNumber _
                               AndAlso row.RerunNumber = RerunNumber _
                               Select row).ToList()

                Case Else

                    RowList = (From row As vwksWSAbsorbanceDS.vwksWSAbsorbanceRow In AbsDS.vwksWSAbsorbance _
                               Where row.OrderTestID = OrderTestID _
                               AndAlso row.MultiItemNumber = MultiItemNumber _
                               Select row).ToList()

            End Select

            DataFound = RowList.Count > 0

            'Update current OrderTestIDAttribute
            OrderTestIDAttribute = OrderTestID
            MultiItemNumberAttribute = MultiItemNumber
            RerunAttribute = RerunNumber

            If DataFound Then
                ShowHeaderData(RowList.First())
                ShowData(OrderTestID, RerunNumber, MultiItemNumber)
                FilterComboBox_SelectedIndexChanged(Nothing, Nothing)
            Else
                FindWithOutData()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "FindData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Find current Order
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 24/10/2011
    ''' </remarks>
    Private Sub FindWithOutData()
        Try
            If Not ExecutionsAttribute Is Nothing Then

                Dim p As List(Of ExecutionsDS.vwksWSExecutionsMonitorRow) = _
                        (From row In ExecutionsAttribute _
                         Where row.OrderTestID = OrderTestIDAttribute _
                         AndAlso row.RerunNumber = RerunAttribute _
                         AndAlso row.MultiItemNumber = MultiItemNumberAttribute _
                         Select row).ToList()

                If p.Count > 0 Then
                    ShowHeaderWithoutData(p.First())
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "FindWithOutData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get data
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 24/02/2012 Optimization on a previous version by DL.
    ''' </remarks>
    Private Sub GetData()
        Try
            Dim myGlobalDataTO As GlobalDataTO
            Dim myExecutionsDelegate As New ExecutionsDelegate

            'Get all different order testid with executions closed
            myGlobalDataTO = myExecutionsDelegate.GetOrderTestWithExecutionStatus( _
                                    Nothing, SourceFormAttribute, RerunAttribute, _
                                    OrderTestIDAttribute, AnalyzerIDAttribute, WorkSessionIDAttribute)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                AbsDS = CType(myGlobalDataTO.SetDatos, vwksWSAbsorbanceDS)

                If AbsDS.vwksWSAbsorbance.Count > 0 Then
                    'Group ds in lists by ordertestid, rerunnumber, multitemnumber

                    Dim Rows As IList = Nothing

                    Select Case SourceFormAttribute
                        Case ScreenCallsGraphical.RESULTSFRM

                            Rows = (From row As vwksWSAbsorbanceDS.vwksWSAbsorbanceRow In AbsDS.vwksWSAbsorbance _
                                    Group row By row.OrderTestID, row.MultiItemNumber, row.RerunNumber _
                                    Into grp = Group Select grp Distinct).ToList()

                        Case ScreenCallsGraphical.WS_STATES

                            Rows = (From row As ExecutionsDS.vwksWSExecutionsMonitorRow In ExecutionsAttribute _
                                    Where row.ExecutionType = "PREP_STD" AndAlso row.TestType = "STD" _
                                    Group row By row.OrderTestID, row.MultiItemNumber, row.RerunNumber _
                                    Into grp = Group Select grp Distinct).ToList()

                            'DL 15/05/2012
                        Case ScreenCallsGraphical.CURVEFRM

                            Rows = (From row As vwksWSAbsorbanceDS.vwksWSAbsorbanceRow In AbsDS.vwksWSAbsorbance _
                                    Group row By row.OrderTestID, row.MultiItemNumber, row.RerunNumber _
                                    Into grp = Group Select grp Distinct).ToList()

                            'DL 15/05/2012

                        Case Else

                            Rows = (From row As vwksWSAbsorbanceDS.vwksWSAbsorbanceRow In AbsDS.vwksWSAbsorbance _
                                    Where row.OrderTestID = OrderTestIDAttribute _
                                    Group row By row.OrderTestID, row.MultiItemNumber _
                                    Into grp = Group Select grp Distinct).ToList()

                    End Select

                    OrderTestGrouped = New List(Of strOrderTest)

                    'Search in DS the source data
                    For i As Integer = 0 To Rows.Count - 1
                        Dim NewRow As strOrderTest
                        NewRow.OrderTestID = (Rows(i)(0)).OrderTestID
                        NewRow.MultiItemNumber = (Rows(i)(0)).MultiItemNumber
                        NewRow.RerunNumber = (Rows(i)(0)).RerunNumber

                        OrderTestGrouped.Add(NewRow)

                        If (NewRow.OrderTestID = OrderTestIDAttribute) AndAlso _
                           (NewRow.MultiItemNumber = MultiItemNumberAttribute) AndAlso _
                           (NewRow.RerunNumber = RerunAttribute) Then

                            CurrentID = OrderTestGrouped.Count - 1

                        End If
                    Next

                    FindData()

                Else
                    FindWithOutData()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & "GetData", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

#End Region

    Private Sub BsButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton2.Click
        'Simulate instruction reception
        If TextBox5.Text.Trim <> "" Then
            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                Dim myGlobal As New GlobalDataTO

                Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
                If myAnalyzerManager.CommThreadsStarted Then
                    'Short instructions
                    myGlobal = myAnalyzerManager.SimulateInstructionReception(TextBox5.Text.Trim)

                    'Me.DataGridView1.DataSource = DirectCast (myGlobal.SetDatos, 
                End If
            End If

        End If
    End Sub

    Private Sub BsButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton3.Click
        TextBox5.Text = String.Empty
    End Sub


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.DoubleBuffer Or ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)

    End Sub
End Class

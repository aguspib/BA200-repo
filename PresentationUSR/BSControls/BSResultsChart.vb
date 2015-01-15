Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Public Class bsResultsChart

#Region "Enumerates"
    Private Enum DisplayModes
        ABS1 = 0
        ABS2 = 1
        BOTH = 2
        DIFF = 3
    End Enum
#End Region

#Region "Classes"
    ''' <summary>
    ''' Object that define the values of each point in the sequence
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' </remarks>
    Public Class ResultItem
        Public Cycle As Integer
        Public Absorbance1 As Double
        Public Absorbance2 As Double
        Public Difference As Double
        Public Visible As Boolean
    End Class

    ''' <summary>
    ''' Object that define the chart area related to each ResultItem
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' </remarks>
    Public Class PointArea
        Public Item As ResultItem
        Public Type As String
        Public R As Rectangle
    End Class
#End Region

#Region "Attributes"
    Private IconAttribute As Image
    Private IsBicromaticAttribute As Boolean = False

    Private ChartVerticalAxisNameAttribute As String = "ABSORBANCE"
    Private ChartHorizontalAxisNameAttribute As String = "CYCLES"

    Private ChartVerticalMarginAttribute As Integer = 40
    Private ChartHorizontalMarginAttribute As Integer = 50

    Private ChartAxisPenWidthAttribute As Integer = 1
    Private ChartPlotPenWidthAttribute As Integer = 1
    Private ChartGridPenWidthAttribute As Integer = 1

    Private ChartBackGroundColorAttribute As System.Drawing.Color = Color.White
    Private ChartAxesForeColorAttribute As System.Drawing.Color = Color.Black
    Private ChartAbs1ForeColorAttribute As System.Drawing.Color = Color.DarkGreen
    Private ChartAbs2ForeColorAttribute As System.Drawing.Color = Color.Blue
    Private ChartDiffForeColorAttribute As System.Drawing.Color = Color.Olive
    Private ChartGridForeColorAttribute As System.Drawing.Color = Color.DarkGray

    Private ChartAbs1SelColorAttribute As System.Drawing.Brush = Brushes.DarkGreen
    Private ChartAbs2SelColorAttribute As System.Drawing.Brush = Brushes.Blue
    Private ChartDiffSelColorAttribute As System.Drawing.Brush = Brushes.Olive

    Private ChartAbs1TitleAttribute As String = "Absorbance1:"
    Private ChartAbs2TitleAttribute As String = "Absorbance2:"
    Private ChartDiffTitleAttribute As String = "Abs1 - Abs2:"

    Private ChartDisplayModeAttribute As DisplayModes = DisplayModes.ABS1
    Private DrawPointsAttribute As Boolean = True
    Private ShowCalibratorNumberAttribute As Boolean 'SG 13/09/2010
    Private DecimalSeparatorAttribute As String
#End Region

#Region "Public Properties"
    'Load image and ToolTip for Exit Button (Close Graph)
    Public WriteOnly Property ExitButtonImage() As Image
        Set(ByVal value As Image)
            bsExitButton.Text = ""
            bsExitButton.Image = value
        End Set
    End Property
    Public WriteOnly Property ExitButtonToolTip() As String
        Set(ByVal value As String)
            bsChartToolTip.SetToolTip(bsExitButton, value)
        End Set
    End Property

    'Character set in the OSCulture as decimal separator
    Public WriteOnly Property DecimalSeparator() As String
        Set(ByVal value As String)
            DecimalSeparatorAttribute = value.Trim
        End Set
    End Property

    'Properties to set the Control margins
    Public WriteOnly Property FormLeft() As Integer
        Set(ByVal value As Integer)
            Me.Left = value
        End Set
    End Property
    Public WriteOnly Property FormTop() As Integer
        Set(ByVal value As Integer)
            Me.Top = value
        End Set
    End Property
    Public WriteOnly Property FormWidth() As Integer
        Set(ByVal value As Integer)
            Me.Width = value
        End Set
    End Property
    Public WriteOnly Property FormHeight() As Integer
        Set(ByVal value As Integer)
            Me.Height = value
        End Set
    End Property

    'Properties for multilanguage texts of all labels shown in the Control
    Public WriteOnly Property CalibratorNumberCaption() As String
        Set(ByVal value As String)
            Me.bsCalibNumTitle.Text = value
        End Set
    End Property
    Public WriteOnly Property SampleCaption() As String
        Set(ByVal value As String)
            Me.bsSampleTitle.Text = value
        End Set
    End Property
    Public WriteOnly Property TestCaption() As String
        Set(ByVal value As String)
            Me.bsTestTitle.Text = value
        End Set
    End Property
    Public WriteOnly Property WellCaption() As String
        Set(ByVal value As String)
            Me.bsWellTitle.Text = value
        End Set
    End Property
    Public WriteOnly Property ReplicateCaption() As String
        Set(ByVal value As String)
            Me.bsReplicateTitle.Text = value
        End Set
    End Property

    'Properties to set values of all fields shown in the Control
    Public WriteOnly Property TestIcon() As Image
        Set(ByVal value As Image)
            IconAttribute = value
        End Set
    End Property
    Public WriteOnly Property CalibNumber() As String
        Set(ByVal value As String)
            bsCalibNumLabel.Text = value
        End Set
    End Property
    Public WriteOnly Property SampleType() As String
        Set(ByVal value As String)
            bsSampleLabel.Text = value
        End Set
    End Property
    Public WriteOnly Property TestID() As String
        Set(ByVal value As String)
            bsTestLabel.Text = value
        End Set
    End Property
    Public WriteOnly Property WellNumber() As String
        Set(ByVal value As String)
            bsWellLabel.Text = value
        End Set
    End Property
    Public WriteOnly Property ReplicateNumber() As String
        Set(ByVal value As String)
            bsReplicateLabel.Text = value
        End Set
    End Property

    'Property to set if field Calibrator Number has to be shown 
    Public WriteOnly Property ShowCalibratorNumber() As Boolean
        Set(ByVal value As Boolean)
            ShowCalibratorNumberAttribute = value

            Me.bsCalibNumLabel.Visible = value
            Me.bsCalibNumTitle.Visible = value

            If (value) Then
                'Relocate controls 
                Dim num As Integer = 23
                bsTestLabel.Width -= num
                bsTestLabel.Location = New System.Drawing.Point(bsTestLabel.Location.X + num, bsTestLabel.Location.Y)
                bsSampleLabel.Location = New System.Drawing.Point(bsSampleLabel.Location.X + num, bsSampleLabel.Location.Y)
                bsTestTitle.Location = New System.Drawing.Point(bsTestTitle.Location.X + num, bsTestTitle.Location.Y)
                bsSampleTitle.Location = New System.Drawing.Point(bsSampleTitle.Location.X + num, bsSampleTitle.Location.Y)
                bsCalibNumLabel.Location = New System.Drawing.Point(bsCalibNumLabel.Location.X + num, bsCalibNumLabel.Location.Y)
                bsCalibNumTitle.Location = New System.Drawing.Point(bsCalibNumTitle.Location.X + num, bsCalibNumTitle.Location.Y)
            End If
        End Set
    End Property

    'Properties for multilanguage texts shown as grid headers
    Public WriteOnly Property ChartCycleGrid() As String
        Set(ByVal value As String)
            bsResultDataGridView.Columns("Cycle").HeaderText = value
            bsResultDataGridView.Columns("Cycle").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        End Set
    End Property
    Public WriteOnly Property ChartAbs1Grid() As String
        Set(ByVal value As String)
            bsResultDataGridView.Columns("Absorbance1").HeaderText = value
            bsResultDataGridView.Columns("Absorbance1").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End Set
    End Property
    Public WriteOnly Property ChartAbs2Grid() As String
        Set(ByVal value As String)
            bsResultDataGridView.Columns("Absorbance2").HeaderText = value
            bsResultDataGridView.Columns("Absorbance2").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End Set
    End Property
    Public WriteOnly Property ChartDiffGrid() As String
        Set(ByVal value As String)
            bsResultDataGridView.Columns("Difference").HeaderText = value
            bsResultDataGridView.Columns("Difference").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End Set
    End Property

    'Properties to set the Chart margins
    Public Property ChartVerticalMargin() As Integer
        Get
            Return ChartVerticalMarginAttribute
        End Get
        Set(ByVal value As Integer)
            ChartVerticalMarginAttribute = value
        End Set
    End Property
    Public Property ChartHorizontalMargin() As Integer
        Get
            Return ChartHorizontalMarginAttribute
        End Get
        Set(ByVal value As Integer)
            ChartHorizontalMarginAttribute = value
        End Set
    End Property

    'Other Properties needed to configure the Graph 
    Public Property ChartAxisPenWidth() As Integer
        Get
            Return ChartAxisPenWidthAttribute
        End Get
        Set(ByVal value As Integer)
            If value < 1 Then value = 1
            ChartAxisPenWidthAttribute = value
        End Set
    End Property
    Public Property ChartPlotPenWidth() As Integer
        Get
            Return ChartPlotPenWidthAttribute
        End Get
        Set(ByVal value As Integer)
            If value < 1 Then value = 1
            ChartPlotPenWidthAttribute = value
        End Set
    End Property
    Public Property ChartBackGroundColor() As System.Drawing.Color
        Get
            Return ChartBackGroundColorAttribute
        End Get
        Set(ByVal value As System.Drawing.Color)
            ChartBackGroundColorAttribute = value
            Me.bsChartPanel.BackColor = value
            Me.bsChartPictureBox.BackColor = value
        End Set
    End Property
    Public Property ChartAxesForeColor() As System.Drawing.Color
        Get
            Return ChartAxesForeColorAttribute
        End Get
        Set(ByVal value As System.Drawing.Color)
            ChartAxesForeColorAttribute = value
        End Set
    End Property
    Public Property ChartAbs1ForeColor() As System.Drawing.Color
        Get
            Return ChartAbs1ForeColorAttribute
        End Get
        Set(ByVal value As System.Drawing.Color)
            ChartAbs1ForeColorAttribute = value
            Me.BsAbs1PictureBox.BackColor = value
        End Set
    End Property
    Public Property ChartAbs2ForeColor() As System.Drawing.Color
        Get
            Return ChartAbs2ForeColorAttribute
        End Get
        Set(ByVal value As System.Drawing.Color)
            ChartAbs2ForeColorAttribute = value
            Me.bsAbs2PictureBox.BackColor = value
        End Set
    End Property
    Public Property ChartDiffForeColor() As System.Drawing.Color
        Get
            Return ChartDiffForeColorAttribute
        End Get
        Set(ByVal value As System.Drawing.Color)
            ChartDiffForeColorAttribute = value
            Me.BsDiffPictureBox.BackColor = value
        End Set
    End Property
    Public Property ChartAbs1SelColor() As System.Drawing.Brush
        Get
            Return ChartAbs1SelColorAttribute
        End Get
        Set(ByVal value As System.Drawing.Brush)
            ChartAbs1SelColorAttribute = value
        End Set
    End Property
    Public Property ChartAbs2SelColor() As System.Drawing.Brush
        Get
            Return ChartAbs2SelColorAttribute
        End Get
        Set(ByVal value As System.Drawing.Brush)
            ChartAbs2SelColorAttribute = value
        End Set
    End Property
    Public Property ChartDiffSelColor() As System.Drawing.Brush
        Get
            Return ChartDiffSelColorAttribute
        End Get
        Set(ByVal value As System.Drawing.Brush)
            ChartDiffSelColorAttribute = value
        End Set
    End Property
    Public Property DrawPoints() As Boolean
        Get
            Return DrawPointsAttribute
        End Get
        Set(ByVal value As Boolean)
            DrawPointsAttribute = value
        End Set
    End Property

    'Properties to set the Title to be shown for the Graph according
    'the option selected 
    Public Property ChartAbs1Title() As String
        Get
            Return ChartAbs1TitleAttribute
        End Get
        Set(ByVal value As String)
            ChartAbs1TitleAttribute = value
            Me.bsAbs1Label.Text = value
        End Set
    End Property
    Public Property ChartAbs2Title() As String
        Get
            Return ChartAbs2TitleAttribute
        End Get
        Set(ByVal value As String)
            ChartAbs2TitleAttribute = value
            Me.bsAbs2Label.Text = value
        End Set
    End Property
    Public Property ChartDiffTitle() As String
        Get
            Return ChartDiffTitleAttribute
        End Get
        Set(ByVal value As String)
            ChartDiffTitleAttribute = value
            Me.bsDiffLabel.Text = value
        End Set
    End Property

    'Other Properties to configure the Grid
    Public Property ChartGridPenWidth() As Integer
        Get
            Return ChartGridPenWidthAttribute
        End Get
        Set(ByVal value As Integer)
            If value < 1 Then value = 1
            ChartGridPenWidthAttribute = value
        End Set
    End Property
    Public Property ChartGridForeColor() As System.Drawing.Color
        Get
            Return ChartGridForeColorAttribute
        End Get
        Set(ByVal value As System.Drawing.Color)
            ChartGridForeColorAttribute = value
        End Set
    End Property
    Public Property ChartVerticalAxisName() As String
        Get
            Return ChartVerticalAxisNameAttribute
        End Get
        Set(ByVal value As String)
            ChartVerticalAxisNameAttribute = value
        End Set
    End Property
    Public Property ChartHorizontalAxisName() As String
        Get
            Return ChartHorizontalAxisNameAttribute
        End Get
        Set(ByVal value As String)
            ChartHorizontalAxisNameAttribute = value
        End Set
    End Property
#End Region

#Region "Private Properties"
    Private Property IsBicromatic() As Boolean
        Get
            Return IsBicromaticAttribute
        End Get
        Set(ByVal value As Boolean)
            IsBicromaticAttribute = value
            If Me.bsModoComboBox.SelectedIndex < 0 Then Me.bsModoComboBox.SelectedIndex = 0

            Me.bsResultDataGridView.Columns(3).Visible = value
            Me.bsResultDataGridView.Columns(4).Visible = value
        End Set
    End Property

    Private Property ChartDisplayMode() As DisplayModes
        Get
            Return ChartDisplayModeAttribute
        End Get
        Set(ByVal value As DisplayModes)
            ChartDisplayModeAttribute = value
        End Set
    End Property
#End Region

#Region "Events"
    Public Event ExitRequest()
    'Public Event FocusLost(ByVal sender As System.Object, ByVal e As System.EventArgs)
#End Region

#Region "Declarations"
    'Absorbance DataSets
    Private InitialResults As AbsorbanceDS
    'Private AcceptedResults As AbsorbanceDS

    Private ScaleX As Double
    Private ScaleY As Double

    'Width and Height of Chart Area
    Private FrameX As Integer
    Private FrameY As Integer

    Private ZeroY As Integer = -1
    Private LeftX As Integer
    Private TopY As Integer
    Private BottomY As Integer
    Private RightX As Integer

    Private NumCycles As Integer
    Private ResultItems As New List(Of ResultItem)
    Private SelectedCycles As New List(Of Integer)

    Private ChartLowerLimit As Double
    Private ChartUpperLimit As Double

    Private myGraphics As System.Drawing.Graphics

    Private PointAreas As New List(Of PointArea)
    Private RemovingPointArea As PointArea

    Private CurrentChartMousePoint As Point

    Private isMonitor As Boolean = False
    Private isDataGridLoading As Boolean
    Private isEven As Boolean 'par
    Private ABS_FORMAT As String = GlobalConstants.ABSORBANCE_FORMAT
#End Region

#Region "Constructor"
    Public Sub New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call.
        FillModeCombo()
    End Sub
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' 1º overload: analysis
    ''' Initializes the common parameters of the chart
    ''' </summary>
    ''' <param name="pExecutions">ExecutionsDS typed dataset</param>
    ''' <remarks>Created by SG 04/07/2010</remarks>
    Public Sub ShowData(ByVal pExecutions As ExecutionsDS, ByVal pSampleType As String, ByVal pTestID As String, ByVal pCalibNumber As String, Optional ByVal pMonitor As Boolean = False)
        Try
            isMonitor = pMonitor

            'Load the data from the absorbances dataset
            InitialResults = GetAbsorbances(pExecutions)

            With Me
                .SampleType = pSampleType
                .TestID = pTestID
                .CalibNumber = pCalibNumber
            End With

            ManageInitialData()
            Refresh()
            If bsModoComboBox.Visible And bsModoComboBox.Enabled Then bsModoComboBox.Focus() 'AG 16/12/2010

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    '''' <summary>
    ''''PROVISIONAL
    '''' Initializes the common parameters of the chart
    '''' </summary>
    '''' <param name="pAbsorbances">AbsorbanceDS typed dataset</param>
    '''' <remarks>Created by SG 04/07/2010</remarks>
    'Public Sub ShowDataMonitor(ByVal pAbsorbances As AbsorbanceDS, ByVal pSampleType As String, ByVal pTestID As String)
    '    Try
    '        Me.isMonitor = True

    '        InitialResults = pAbsorbances 'load the data from the absorbances dataset
    '        With Me
    '            .SampleType = pSampleType
    '            .TestID = pTestID
    '        End With

    '        ManageInitialData()
    '        Me.Refresh()
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Prepares the input data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 04/07/2010
    ''' </remarks>
    Private Sub ManageInitialData()
        Try
            FrameX = Me.bsChartPictureBox.Width - 2 * Me.ChartHorizontalMarginAttribute
            FrameY = Me.bsChartPictureBox.Height - 2 * Me.ChartVerticalMarginAttribute

            LeftX = Me.ChartHorizontalMarginAttribute
            TopY = Me.ChartVerticalMarginAttribute
            BottomY = Me.ChartVerticalMarginAttribute + FrameY
            RightX = Me.ChartHorizontalMarginAttribute + FrameX

            Me.bsAbsorbanceLabel.Text = ChartVerticalAxisNameAttribute
            Me.bsCyclesLabel.Text = ChartHorizontalAxisNameAttribute

            If (InitialResults IsNot Nothing AndAlso InitialResults.twksAbsorbances.Rows.Count > 0) Then
                LoadResultItems()
                If (Me.IsBicromatic) Then bsModoComboBox.Visible = True

                PrepareResultItems()
                BindResultGrid()
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Obtains the absorbances table corresponding to the executions
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 26/08/2010
    ''' </remarks>
    Private Function GetAbsorbances(ByVal pExecutions As ExecutionsDS) As AbsorbanceDS
        Dim myAbsorbancesDS As New AbsorbanceDS
        Dim resultdata As New Biosystems.Ax00.Global.GlobalDataTO

        Try
            Dim myExecutionsDelegate As New Biosystems.Ax00.BL.ExecutionsDelegate
            For Each dr As ExecutionsDS.twksWSExecutionsRow In pExecutions.twksWSExecutions.Rows
                Dim myExportCalculations As New Biosystems.Ax00.BL.ResultsFileDelegate
                With Me
                    '.WellNumber = dr.WellUsed.ToString
                    '.ReplicateNumber = dr.ReplicateNumber.ToString
                    If Not dr.IsWellUsedNull Then .WellNumber = dr.WellUsed.ToString
                    If Not dr.IsReplicateNumberNull Then .ReplicateNumber = dr.ReplicateNumber.ToString
                End With

                resultdata = myExportCalculations.GetReadingAbsorbancesByExecution(Nothing, dr.ExecutionID, dr.AnalyzerID, dr.WorkSessionID, False) 'AG 09/03/2011 - change True for False
                If (Not resultdata.HasError) Then
                    myAbsorbancesDS = CType(resultdata.SetDatos, AbsorbanceDS)
                    Exit For
                Else
                    Return Nothing
                End If
            Next
        Catch ex As Exception
            Throw ex
        End Try
        Return myAbsorbancesDS
    End Function

    ''' <summary>
    ''' Drawn an empty chart
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 27/08/2010
    ''' </remarks>
    Private Sub DrawEmptyChart()
        Try
            Dim XAxisPositions As New List(Of Integer)
            Dim YAxisPositions As New List(Of Integer)

            Dim intervalX As Double = Me.FrameX / 10
            Dim intervalY As Double = Me.FrameY / 5

            For x As Integer = 0 To 10 Step 1
                XAxisPositions.Add(Me.LeftX + x * CInt(intervalX))
            Next x

            For y As Integer = 0 To 5 Step 1
                YAxisPositions.Add(Me.TopY + y * CInt(intervalY))
            Next y

            'Pens presettings
            Dim myAxisPen As New System.Drawing.Pen(Me.ChartAxesForeColorAttribute, Me.ChartAxisPenWidthAttribute)
            Dim myGridPen As New System.Drawing.Pen(Me.ChartGridForeColorAttribute, Me.ChartGridPenWidthAttribute)

            'Selecting brushes
            Dim myAbs1SelBrush As System.Drawing.Brush = Me.ChartAbs1SelColorAttribute
            Dim myAbs2SelBrush As System.Drawing.Brush = Me.ChartAbs2SelColorAttribute
            Dim myDiffSelBrush As System.Drawing.Brush = Me.ChartDiffSelColorAttribute

            'Vertical Axes and Grids
            Dim i As Integer = 0
            For Each x As Integer In XAxisPositions
                If (i = 0) Then
                    myGraphics.DrawLine(myAxisPen, LeftX, BottomY, LeftX, TopY)
                ElseIf (i = XAxisPositions.Count - 1) Then
                    myGraphics.DrawLine(myAxisPen, RightX, BottomY, RightX, TopY)
                Else
                    myGraphics.DrawLine(myGridPen, x, BottomY, x, TopY) 'grid
                End If
                i = i + 1
            Next

            'Horizontal Axes and grids
            i = 0
            Dim format As String = FormatChartValue(Me.ChartUpperLimit - Me.ChartLowerLimit)
            For Each y As Integer In YAxisPositions
                If (i = 0) Then
                    myGraphics.DrawLine(myAxisPen, LeftX, TopY, RightX, TopY)
                ElseIf (i = YAxisPositions.Count - 1) Then
                    myGraphics.DrawLine(myAxisPen, LeftX, BottomY, RightX, BottomY)
                Else
                    myGraphics.DrawLine(myGridPen, LeftX, y, RightX, y)
                End If
                i = i + 1
            Next
            Me.Enabled = False
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Draws the chart with the loaded data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' Modified by: SA 11/11/2010 - Removed the replace of decimal separator when format the ABS values.
    '''                              Besides, shown top labels for Vertical Grid Lines horizontally instead 
    '''                              of vertically and remove the min/max limits characters 
    ''' </remarks>
    Private Sub DrawResultChart()
        Dim XAxisValues As New List(Of Double)     'Values of the X axis ticks
        Dim YAxisValues As New List(Of Double)     'Values of the Y axis ticks
        Dim XAxisPositions As New List(Of Integer) 'Positions of the X axis ticks
        Dim YAxisPositions As New List(Of Integer) 'Positions of the Y axis ticks

        Dim XDataPositions As New List(Of Integer)     'X positions
        Dim YDataPositions As New List(Of Integer)     'Y positions

        Dim XDiffDataPositions As New List(Of Integer) 'X positions
        Dim YDiffDataPositions As New List(Of Integer) 'Y positions

        Try
            'Pens presettings
            Dim myAxisPen As New System.Drawing.Pen(Me.ChartAxesForeColorAttribute, Me.ChartAxisPenWidthAttribute)
            Dim myGridPen As New System.Drawing.Pen(Me.ChartGridForeColorAttribute, Me.ChartGridPenWidthAttribute)

            'Selecting brushes
            Dim myAbs1SelBrush As System.Drawing.Brush = Me.ChartAbs1SelColorAttribute
            Dim myAbs2SelBrush As System.Drawing.Brush = Me.ChartAbs2SelColorAttribute
            Dim myDiffSelBrush As System.Drawing.Brush = Me.ChartDiffSelColorAttribute

            'CALCULATING AREA
            'Cycles axis
            SetCyclesAxisPositions(XAxisPositions, XAxisValues)

            'Absorbance axis
            SetAbsorbanceAxisPositions(YAxisPositions, YAxisValues)

            'Data
            If (Not Me.ChartDisplayMode = DisplayModes.DIFF) Then
                SetDataPositions(XDataPositions, YDataPositions)
            Else
                XDiffDataPositions = SetDiffDataPositions(XAxisPositions, YDiffDataPositions)
            End If

            'DRAWING AREA
            'Vertical Axes and grids
            Dim i As Integer = 0
            For Each x As Integer In XAxisPositions
                myGraphics.DrawLine(myGridPen, x, BottomY, x, TopY) 'Grid

                If (i = 0 Or i = XAxisPositions.Count - 1) Then
                    myGraphics.DrawLine(myAxisPen, x, BottomY, x, TopY)
                Else
                    myGraphics.DrawLine(myAxisPen, x, TopY - 5, x, TopY)       'tick sup
                    myGraphics.DrawLine(myAxisPen, x, BottomY, x, BottomY + 5) 'tick inf
                    'myGraphics.DrawString(XAxisValues(i).ToString.PadLeft(2), Me.Font, Brushes.Black, x - 8, BottomY + 8) 'text
                End If
                i = i + 1
            Next

            'Horizontal Axes and grids
            i = 0
            Dim format As String = FormatChartValue(Me.ChartUpperLimit - Me.ChartLowerLimit)
            For Each y As Integer In YAxisPositions
                myGraphics.DrawLine(myGridPen, LeftX, y, RightX, y)
                If (i = 0 Or i = YAxisPositions.Count - 1) Then
                    myGraphics.DrawLine(myAxisPen, LeftX, y, RightX, y)
                End If

                myGraphics.DrawLine(myAxisPen, LeftX + 5, y, LeftX, y)   'tick left
                myGraphics.DrawLine(myAxisPen, RightX, y, RightX - 5, y) 'tick right

                Dim text As String = YAxisValues(i).ToString(ABS_FORMAT) 'AG 16/12/2010 - add the 4th decimal
                myGraphics.DrawString(text.PadLeft(6), Me.Font, Brushes.Black, LeftX - 40, y - 6) 'text
                myGraphics.DrawString(text.PadLeft(6), Me.Font, Brushes.Black, RightX + 2, y - 6) 'text
                i = i + 1
            Next

            'Data points and lines
            PointAreas.Clear()

            'Assign the visible result items to point areas
            For Each RI As ResultItem In ResultItems
                If (RI.Visible) Then
                    Dim myPointArea As New PointArea
                    myPointArea.Item = RI
                    PointAreas.Add(myPointArea)
                End If
            Next

            'Draw on the top the original cycle index
            'Dim k As Integer = 0
            'For Each x As Integer In XAxisPositions
            '    If (k >= 1 And k < XAxisPositions.Count - 1) Then
            '        If (PointAreas.Count > 0) Then
            '            Dim sf As New Drawing.StringFormat() '(StringFormatFlags.DirectionVertical)
            '            sf.Alignment = StringAlignment.Near
            '            myGraphics.DrawString(PointAreas(k - 1).Item.Cycle.ToString, Me.Font, Brushes.Black, x - 5, TopY - 20, sf)
            '            'myGraphics.DrawString("<" & PointAreas(k - 1).Item.Cycle.ToString & ">", Me.Font, Brushes.Black, x - 12, TopY - 20, sf)
            '        Else
            '            myGraphics.DrawString(XAxisValues(k).ToString.PadLeft(2), Me.Font, Brushes.Black, x - 10, TopY - 20)
            '        End If
            '    End If
            '    k = k + 1
            'Next

            If (Me.ChartDisplayMode <> DisplayModes.DIFF) Then
                DrawDataPoints(XAxisPositions, YDataPositions)
            Else
                DrawDiffDataPoints(XDiffDataPositions, YDiffDataPositions)
            End If

            'Selected points
            i = 0
            Dim pX1 As Integer
            Dim pY1 As Integer
            If (SelectedCycles.Count > 0) Then
                If (Me.ChartDisplayMode <> DisplayModes.DIFF) Then
                    For Each x As Integer In XAxisPositions
                        If (i > 0 And i < XAxisPositions.Count - 1) Then
                            If (SelectedCycles.First = i) Then
                                pX1 = x
                                pY1 = YDataPositions(i - 1)
                                Dim selArea As Rectangle = New Rectangle(pX1 - 6, pY1 - 6, 12, 12)
                                If (Not Me.IsBicromatic) Then
                                    myGraphics.FillEllipse(myAbs1SelBrush, selArea)
                                Else
                                    If (Me.ChartDisplayMode = DisplayModes.ABS1 Or Me.ChartDisplayMode = DisplayModes.BOTH) Then
                                        If (Me.isEven) Then
                                            If ((i Mod 2) = 0) Then
                                                myGraphics.FillEllipse(myAbs1SelBrush, selArea)
                                            End If
                                        Else
                                            If ((i Mod 2) = 1) Then
                                                myGraphics.FillEllipse(myAbs1SelBrush, selArea)
                                            End If
                                        End If
                                    End If
                                    If (Me.ChartDisplayMode = DisplayModes.ABS2 Or Me.ChartDisplayMode = DisplayModes.BOTH) Then
                                        If (Me.isEven) Then
                                            If ((i Mod 2) = 1) Then
                                                myGraphics.FillEllipse(myAbs2SelBrush, selArea)
                                            End If
                                        Else
                                            If ((i Mod 2) = 0) Then
                                                myGraphics.FillEllipse(myAbs2SelBrush, selArea)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        i = i + 1
                    Next
                Else
                    'Difference
                    Dim j As Integer = 0
                    For Each x As Integer In XDiffDataPositions
                        If (XAxisPositions(SelectedCycles.First) = x) Then
                            pX1 = x
                            pY1 = YDiffDataPositions(j)

                            Dim selArea As Rectangle = New Rectangle(pX1 - 6, pY1 - 6, 12, 12)
                            myGraphics.FillEllipse(myDiffSelBrush, selArea)
                        End If
                        j = j + 1
                    Next
                End If
            End If

            Me.Enabled = True
            bsResultDataGridView.Focus()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Once all the positions have been calculated, this method draws the result lines
    ''' </summary>
    ''' <param name="pXAxisPositions">Positions in X axis</param>
    ''' <param name="pYDataPositions">Positions in Y axis (absorbance)</param>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' </remarks>
    Private Sub DrawDataPoints(ByRef pXAxisPositions As List(Of Integer), ByRef pYDataPositions As List(Of Integer))
        Try
            Dim myAbs1Pen As New System.Drawing.Pen(Me.ChartAbs1ForeColorAttribute, Me.ChartPlotPenWidthAttribute)
            Dim myAbs1Brush As New System.Drawing.SolidBrush(Me.ChartAbs1ForeColorAttribute)
            Dim myAbs2Pen As New System.Drawing.Pen(Me.ChartAbs2ForeColorAttribute, Me.ChartPlotPenWidthAttribute)
            Dim myAbs2Brush As New System.Drawing.SolidBrush(Me.ChartAbs2ForeColorAttribute)

            'Polygon brushes with transparency
            Dim myAbs1PolyBrush As New SolidBrush(Color.FromArgb(50, Me.ChartAbs1ForeColorAttribute))
            Dim myAbs2PolyBrush As New SolidBrush(Color.FromArgb(50, Me.ChartAbs2ForeColorAttribute))

            Dim i As Integer = 0

            Dim pAbs1Xo As Integer
            Dim pAbs1Yo As Integer
            Dim pAbs1X1 As Integer
            Dim pAbs1Y1 As Integer

            Dim pAbs2Xo As Integer
            Dim pAbs2Yo As Integer
            Dim pAbs2X1 As Integer
            Dim pAbs2Y1 As Integer

            For Each x As Integer In pXAxisPositions
                If (Not Me.IsBicromatic) Then
                    If (i > 0 And i < pXAxisPositions.Count - 1) Then
                        pAbs1X1 = x
                        pAbs1Y1 = pYDataPositions(i - 1)

                        'Points
                        PointAreas(i - 1).R = CreateCircle(pAbs1X1, pAbs1Y1)
                        PointAreas(i - 1).Type = "Abs1"

                        If (Me.DrawPoints) Then
                            myGraphics.FillEllipse(myAbs1Brush, PointAreas(i - 1).R)
                        End If

                        'Lines
                        If (i > 1) Then
                            DrawPolygon4(myGraphics, myAbs1PolyBrush, pAbs1Xo, pAbs1Yo, pAbs1X1, pAbs1Y1)
                            myGraphics.DrawLine(myAbs1Pen, pAbs1Xo, pAbs1Yo, pAbs1X1, pAbs1Y1)
                        End If

                        pAbs1Xo = pAbs1X1
                        pAbs1Yo = pAbs1Y1
                    End If
                Else
                    If (i > 0 And i < pXAxisPositions.Count - 1) Then
                        If (Me.ChartDisplayMode = DisplayModes.ABS1 Or Me.ChartDisplayMode = DisplayModes.BOTH) Then
                            Dim take As Boolean = False

                            If (Me.isEven) Then
                                take = (i Mod 2 = 0)
                            Else
                                take = (i Mod 2 = 1)
                            End If

                            If (take) Then
                                pAbs1X1 = x
                                pAbs1Y1 = pYDataPositions(i - 1)

                                'Points
                                PointAreas(i - 1).R = CreateCircle(pAbs1X1, pAbs1Y1)
                                PointAreas(i - 1).Type = "Abs1"

                                If (Me.DrawPoints) Then
                                    myGraphics.FillEllipse(myAbs1Brush, PointAreas(i - 1).R)
                                End If

                                'Lines
                                If (Me.isEven And (i > 2)) Or (Not Me.isEven And (i > 1)) Then
                                    DrawPolygon4(myGraphics, myAbs1PolyBrush, pAbs1Xo, pAbs1Yo, pAbs1X1, pAbs1Y1)
                                    myGraphics.DrawLine(myAbs1Pen, pAbs1Xo, pAbs1Yo, pAbs1X1, pAbs1Y1)
                                End If

                                pAbs1Xo = pAbs1X1
                                pAbs1Yo = pAbs1Y1
                            End If
                        End If

                        If (Me.ChartDisplayMode = DisplayModes.ABS2 Or Me.ChartDisplayMode = DisplayModes.BOTH) Then
                            Dim take As Boolean = False

                            If (Me.isEven) Then
                                take = (i Mod 2 = 1)
                            Else
                                take = (i Mod 2 = 0)
                            End If

                            If (take) Then
                                pAbs2X1 = x
                                pAbs2Y1 = pYDataPositions(i - 1)

                                'Points
                                PointAreas(i - 1).R = CreateCircle(pAbs2X1, pAbs2Y1)
                                PointAreas(i - 1).Type = "Abs2"

                                If (Me.DrawPoints) Then
                                    myGraphics.FillEllipse(myAbs2Brush, PointAreas(i - 1).R)
                                End If

                                'Lines
                                If (Not Me.isEven And (i > 2)) Or (Me.isEven And (i > 1)) Then
                                    DrawPolygon4(myGraphics, myAbs2PolyBrush, pAbs2Xo, pAbs2Yo, pAbs2X1, pAbs2Y1)
                                    myGraphics.DrawLine(myAbs2Pen, pAbs2Xo, pAbs2Yo, pAbs2X1, pAbs2Y1)
                                End If

                                pAbs2Xo = pAbs2X1
                                pAbs2Yo = pAbs2Y1
                            End If
                        End If
                    End If
                End If
                i = i + 1
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub DrawDiffDataPoints(ByRef pXDiffDataPositions As List(Of Integer), ByRef pYDiffDataPositions As List(Of Integer))
        Try
            Dim myDiffPen As New System.Drawing.Pen(Me.ChartDiffForeColorAttribute, Me.ChartPlotPenWidthAttribute)
            Dim myDiffBrush As New System.Drawing.SolidBrush(Me.ChartDiffForeColorAttribute)

            'Polygon brushes with transparency
            Dim myDiffPolyBrush As New SolidBrush(Color.FromArgb(50, Me.ChartDiffForeColorAttribute))

            Dim i As Integer = 0

            Dim pDiffXo As Integer
            Dim pDiffYo As Integer
            Dim pDiffX1 As Integer
            Dim pDiffY1 As Integer

            'Draw diff datapoints
            Dim j As Integer = 0
            For Each x As Integer In pXDiffDataPositions
                pDiffX1 = x
                pDiffY1 = pYDiffDataPositions(j)

                'Points
                Dim k As Integer = 1
                If (Me.isEven) Then
                    k = 1
                Else
                    k = 2
                    If (j >= pXDiffDataPositions.Count - 1) Then Exit For
                End If

                PointAreas(k + 2 * j).R = CreateCircle(pDiffX1, pDiffY1)
                PointAreas(k + 2 * j).Type = "Diff"

                If (Me.DrawPoints) Then
                    myGraphics.FillEllipse(myDiffBrush, PointAreas(k + 2 * j).R)
                End If

                'Lines
                If (j > 0) Then
                    DrawPolygon4(myGraphics, myDiffPolyBrush, pDiffXo, pDiffYo, pDiffX1, pDiffY1, True)
                    myGraphics.DrawLine(myDiffPen, pDiffXo, pDiffYo, pDiffX1, pDiffY1)
                End If

                pDiffXo = pDiffX1
                pDiffYo = pDiffY1

                j = j + 1
            Next
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Sets the values and positions of the Y axis ticks
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' </remarks>
    Private Sub SetAbsorbanceAxisPositions(ByRef pPositions As List(Of Integer), ByRef pValues As List(Of Double))
        Try
            Dim middle As Double = (Me.ChartUpperLimit + Me.ChartLowerLimit) / 2
            Dim range As Double = Me.ChartUpperLimit - Me.ChartLowerLimit

            Dim min As Double
            Dim max As Double
            Dim interval As Double

            'Define Y interval value
            If (range > 0 And range < 1) Then
                Dim posDecimalRange As Integer = GetFirstDecimalPosition(range)
                Dim size As Integer = CInt((range Mod 1) * Math.Pow(10, posDecimalRange))
                interval = size * (2 / Math.Pow(10, posDecimalRange + 1))
            ElseIf (range = 0) Then
                interval = Math.Abs(Me.ChartUpperLimit) * 2
            Else
                Dim posIntegerRange As Integer = GetFirstIntegerPosition(range)
                Dim size As Integer = CInt(range.ToString.First.ToString)
                interval = size * (2 * Math.Pow(10, posIntegerRange - 2))
            End If

            If (Math.Abs(interval) < 0.01) Then
                If (range > 0) Then
                    interval = Math.Abs(range)
                Else
                    interval = 1
                End If
            End If

            Dim p As Integer
            Dim base As Double
            If (Me.ChartLowerLimit < 0 And Me.ChartUpperLimit > 0) Then
                Dim min1 As Double
                Dim max1 As Double = 0

                'Define the lower range
                base = 0
                Do While (base - interval > (middle - range * 1.5 / 2 - interval / 2))
                    base = base - interval
                Loop
                min1 = base

                p = 0
                Dim value1 As Double
                While ((min1 + p * interval) < 0)
                    value1 = min1 + p * interval
                    pValues.Add(value1)
                    p = p + 1
                End While
                min = min1

                'Define the upper range
                min1 = 0
                p = 0

                Dim value2 As Double
                While ((min1 + p * interval) <= (middle + range * 1.5 / 2 + interval))
                    value2 = min1 + p * interval
                    pValues.Add(value2)
                    p = p + 1
                End While

            ElseIf (Me.ChartLowerLimit = Me.ChartUpperLimit) Then
                'Define the lower limit
                base = -1000
                Do While (base + interval < (middle))
                    base = base + interval
                Loop
                min = base

                p = 0
                Dim value As Double
                While ((min + p * interval) <= (middle + 2 * interval))
                    value = min + p * interval
                    pValues.Add(value)
                    p = p + 1
                End While

            Else
                'Define the lower limit
                base = -1000
                Do While (base + interval < (middle - range * 1.5 / 2 - interval / 2))
                    base = base + interval
                Loop
                min = base

                p = 0
                Dim value As Double
                While ((min + p * interval) <= (middle + range * 1.5 / 2 + interval / 2))
                    value = min + p * interval
                    pValues.Add(value)
                    p = p + 1
                End While
            End If

            'Define the upper limit
            max = pValues(pValues.Count - 1)

            'Resort the values in the list because the lower values will be at the upper positions (pixels) in the chart
            pValues = ReOrderDoubleList(pValues)
            Dim valueRange As Double = max - min
            Dim myScale As Double = Me.FrameY / valueRange

            'Calculate the positions according to the obtained values
            Dim pos As Double
            For Each v As Double In pValues
                pos = Me.TopY + FrameY - myScale * (v - min)
                pPositions.Add(CInt(pos))

                If v = 0 Then Me.ZeroY = CInt(pos)
            Next

            If (Me.ZeroY = -1) Then
                Me.ZeroY = Me.TopY + FrameY
            End If

            Me.ChartLowerLimit = min
            Me.ChartUpperLimit = max

            Me.ScaleY = myScale
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Sets the values and positions of the X axis ticks
    ''' </summary>
    ''' <remarks> 
    ''' Created by:  SG 25/08/2010
    ''' </remarks>
    Private Sub SetCyclesAxisPositions(ByRef pPositions As List(Of Integer), ByRef pValues As List(Of Double))
        Try
            Dim ValueRange As Double = Me.NumCycles + 1
            Dim myScale As Double = Me.FrameX / ValueRange
            Dim myGridLines As Integer = Me.NumCycles + 1

            For p As Integer = 0 To Me.NumCycles + 1
                Dim val As Double = p * ValueRange / myGridLines
                Dim pos As Double = Me.LeftX + p * Me.FrameX / myGridLines

                pValues.Add(val)
                pPositions.Add(CInt(pos))
            Next

            Me.ScaleX = myScale
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Calculates the positions of the points according of the data values
    ''' </summary>
    ''' <remarks> 
    ''' Created by:  SG 25/08/2010
    ''' </remarks>
    Private Sub SetDataPositions(ByRef pXPositions As List(Of Integer), ByRef pYPositions As List(Of Integer))
        Try
            If (ResultItems.Count > 0) Then
                Dim i As Integer = 0
                For Each r As ResultItem In ResultItems
                    Dim posX As Integer
                    Dim posY As Integer

                    If (r.Visible) Then
                        posX = CInt(Me.ScaleX * r.Cycle)
                        If (r.Absorbance1 > -1000) Then
                            posY = CInt(Me.ScaleY * (r.Absorbance1 - Me.ChartLowerLimit))
                        ElseIf (r.Absorbance2 > -1000) Then
                            posY = CInt(Me.ScaleY * (r.Absorbance2 - Me.ChartLowerLimit))
                        Else
                            posY = 0
                        End If
                        posY = TopY + FrameY - posY

                        pXPositions.Add(posX)
                        pYPositions.Add(posY)
                        i = i + 1
                    End If
                Next
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Calculates the positions in pixels according to the differences
    ''' </summary>
    ''' <remarks>
    '''  Created by:  SG 25/08/2010
    ''' </remarks>
    Private Function SetDiffDataPositions(ByRef pXPositions As List(Of Integer), ByRef pYPositions As List(Of Integer)) As List(Of Integer)
        Dim XDataPos As New List(Of Integer)

        Try
            If (ResultItems.Count > 0) Then
                Dim i As Integer = 0
                For Each r As ResultItem In ResultItems
                    Dim posX As Integer
                    Dim posY As Integer

                    If (r.Difference > -1000) Then
                        posX = pXPositions(i + 1)
                        If (i > 0 And i < ResultItems.Count) Then
                            posY = CInt(Me.ScaleY * (r.Difference - Me.ChartLowerLimit))
                        Else
                            posY = 0
                        End If
                        posY = TopY + FrameY - posY

                        XDataPos.Add(posX)
                        pYPositions.Add(posY)
                    End If

                    i = i + 1
                Next
            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return XDataPos
    End Function

    ''' <summary>
    ''' Bind the data into the datagrid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' Modified by: SA 11/11/2010 - Remove the replacing when apply the format to ABS values
    ''' </remarks>
    Private Sub BindResultGrid()
        Try
            isDataGridLoading = True

            Me.bsResultDataGridView.Rows.Clear()
            For Each R As ResultItem In ResultItems
                Dim i As Integer = Me.bsResultDataGridView.Rows.Add

                With Me.bsResultDataGridView.Rows(i)
                    .Cells("CycleVisible").Value = R.Visible
                    .Cells("Cycle").Value = "<" & R.Cycle.ToString & ">"

                    If (R.Absorbance1 = -1000) Then
                        .Cells("Absorbance1").Value = "-"
                    Else
                        .Cells("Absorbance1").Value = R.Absorbance1.ToString(ABS_FORMAT) 'AG 16/12/2010 - add the 4th decimal
                    End If

                    If (R.Absorbance2 = -1000) Then
                        .Cells("Absorbance2").Value = "-"
                    Else
                        .Cells("Absorbance2").Value = R.Absorbance2.ToString(ABS_FORMAT) 'AG 16/21/2010 - add 4th decimal
                    End If

                    If (Me.IsBicromatic) Then
                        If (R.Difference = -1000) Then
                            .Cells("Difference").Value = "-"
                        Else
                            .Cells("Difference").Value = R.Difference.ToString(ABS_FORMAT) 'AG 16/21/2010 - add 4th decimal
                        End If
                    Else
                        .Cells("Difference").Value = "-"
                    End If

                    .Cells("CycleVisible").ReadOnly = False
                    .Cells("Cycle").ReadOnly = True
                    .Cells("Absorbance1").ReadOnly = True
                    .Cells("Absorbance2").ReadOnly = True
                    .Cells("Difference").ReadOnly = True

                    .Cells("CycleVisible").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    .Cells("Cycle").Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                    .Cells("Absorbance1").Style.Alignment = DataGridViewContentAlignment.MiddleRight
                    .Cells("Absorbance2").Style.Alignment = DataGridViewContentAlignment.MiddleRight
                    .Cells("Difference").Style.Alignment = DataGridViewContentAlignment.MiddleRight

                    If (R.Visible) Then
                        .DefaultCellStyle.BackColor = Color.White
                        .DefaultCellStyle.ForeColor = Color.Black
                    Else
                        .DefaultCellStyle.BackColor = Color.White
                        .DefaultCellStyle.ForeColor = Color.Gray
                    End If
                End With
            Next

            bsResultDataGridView.Columns("CycleVisible").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsResultDataGridView.Columns("Cycle").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            If (Me.bsResultDataGridView.SelectedRows.Count > 0) Then
                Me.bsResultDataGridView.SelectedRows(0).Selected = False
            End If
            isDataGridLoading = False
        Catch ex As Exception
            isDataGridLoading = False
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Load the data in an internal structure ResultItems. Gets the minimum and maximum values of the data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG SG 25/08/2010
    ''' </remarks>
    Private Sub LoadResultItems()
        Try
            Me.NumCycles = 0
            ResultItems = New List(Of ResultItem)

            Me.IsBicromatic = False

            'Determine if it is monochromatic or bichromatic
            Dim i As Integer = 0
            Dim firstWaveLength As Double

            For Each result As AbsorbanceDS.twksAbsorbancesRow In Me.InitialResults.twksAbsorbances.Rows
                If (i = 0) Then firstWaveLength = result.WaveLength
                If (i = 1) Then
                    If (result.WaveLength <> firstWaveLength) Then
                        Me.IsBicromatic = True
                        Exit For
                    End If
                End If
                i = i + 1
            Next

            Me.bsModoComboBox.Visible = Me.IsBicromatic
            Me.isEven = Me.InitialResults.twksAbsorbances.Rows.Count Mod 2 = 0

            'If it's bicromatic the last cycle must be ALWAYS normal waveLength
            If (Me.IsBicromatic) Then
                If (Not Me.isEven) Then
                    If (Me.InitialResults.twksAbsorbances.First.WaveLength <> Me.InitialResults.twksAbsorbances.Last.WaveLength) Then
                        Throw New Exception("Incorrect data")
                    End If
                Else
                    If (Me.InitialResults.twksAbsorbances.First.WaveLength = Me.InitialResults.twksAbsorbances.Last.WaveLength) Then
                        Throw New Exception("Incorrect data")
                    End If
                End If
            End If

            Dim j As Integer = 0
            For Each result As AbsorbanceDS.twksAbsorbancesRow In Me.InitialResults.twksAbsorbances.Rows
                Dim myResultItem As New ResultItem
                With myResultItem
                    .Visible = True
                    .Cycle = result.ReadingNumber

                    If (Not Me.IsBicromatic) Then
                        'MONOCHROMATIC
                        .Absorbance1 = result.Absorbance
                        .Absorbance2 = -1000
                    Else
                        'BICHROMATIC
                        If (Me.isEven) Then
                            If (j Mod 2 = 0) Then
                                .Absorbance1 = -1000
                                .Absorbance2 = result.Absorbance
                            ElseIf (j Mod 2 = 1) Then
                                .Absorbance1 = result.Absorbance
                                .Absorbance2 = -1000
                            End If
                        Else
                            If (j = 0) Then
                                .Absorbance1 = result.Absorbance
                                .Absorbance2 = -1000
                            Else
                                If (j Mod 2 = 1) Then
                                    .Absorbance1 = -1000
                                    .Absorbance2 = result.Absorbance
                                ElseIf (j Mod 2 = 0) Then
                                    .Absorbance1 = result.Absorbance
                                    .Absorbance2 = -1000
                                End If
                            End If
                        End If
                    End If
                End With

                ResultItems.Add(myResultItem)
                j = j + 1
            Next

            'Calculate the differences
            If (Me.IsBicromatic) Then CalculateDifferences()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Calculates the differences between cycles for bichromatic tests
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' </remarks>
    Private Sub CalculateDifferences()
        Try
            If (ResultItems.Count > 1) Then
                Dim odd As Integer
                Dim since As Integer
                Dim until As Integer

                If (isEven) Then
                    since = 1
                    until = ResultItems.Count - 1
                    odd = 1
                Else
                    since = 2
                    until = ResultItems.Count - 1
                    odd = 0
                End If

                ResultItems(0).Difference = -1000
                If (Not Me.isEven) Then
                    ResultItems(1).Difference = -1000
                End If

                For c As Integer = since To until Step 1
                    If (c Mod 2 = odd) Then
                        ResultItems(c).Difference = ResultItems(c).Absorbance1 - ResultItems(c - 1).Absorbance2
                    Else
                        ResultItems(c).Difference = -1000
                    End If
                Next
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the data before calculating the scales, axes, etc
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' </remarks>
    Private Sub PrepareResultItems()
        Try
            Dim min1 As Double
            Dim max1 As Double

            Me.NumCycles = 0

            min1 = 999
            max1 = -999

            Dim j As Integer = 0
            For Each R As ResultItem In ResultItems
                If (R.Visible) Then
                    If (Not Me.IsBicromatic) Then
                        If (j = 0 Or min1 > R.Absorbance1) Then
                            min1 = R.Absorbance1
                        End If
                        If (j = 0 Or max1 < R.Absorbance1) Then
                            max1 = R.Absorbance1
                        End If
                    Else
                        'AG 16/12/2010
                        'If (Me.ChartDisplayMode <> DisplayModes.DIFF) Then
                        '    If (R.Absorbance1 > -1000) Then
                        '        If (j = 0 Or min1 > R.Absorbance1) Then
                        '            min1 = R.Absorbance1
                        '        End If
                        '        If (j = 0 Or max1 < R.Absorbance1) Then
                        '            max1 = R.Absorbance1
                        '        End If
                        '    ElseIf (R.Absorbance2 > -1000) Then
                        '        If (j = 0 Or min1 > R.Absorbance2) Then
                        '            min1 = R.Absorbance2
                        '        End If
                        '        If (j = 0 Or max1 < R.Absorbance2) Then
                        '            max1 = R.Absorbance2
                        '        End If
                        '    End If
                        'Else
                        '    If (R.Difference <> -1000) Then
                        '        If (j = 0 Or min1 > R.Difference) Then
                        '            min1 = R.Difference
                        '        End If
                        '        If (j = 0 Or max1 < R.Difference) Then
                        '            max1 = R.Difference
                        '        End If
                        '    End If
                        'End If
                        If ChartDisplayMode = DisplayModes.ABS1 Then
                            If R.Absorbance1 <> -1000 Then
                                If min1 > R.Absorbance1 Then min1 = R.Absorbance1
                                If max1 < R.Absorbance1 Then max1 = R.Absorbance1
                            End If

                        ElseIf Me.ChartDisplayMode = DisplayModes.ABS2 Then
                            If R.Absorbance2 <> -1000 Then
                                If min1 > R.Absorbance2 Then min1 = R.Absorbance2
                                If max1 < R.Absorbance2 Then max1 = R.Absorbance2
                            End If

                        ElseIf Me.ChartDisplayMode = DisplayModes.BOTH Then
                            If R.Absorbance1 <> -1000 Then
                                If min1 > R.Absorbance1 Then min1 = R.Absorbance1
                                If max1 < R.Absorbance1 Then max1 = R.Absorbance1
                            End If

                            If R.Absorbance2 <> -1000 Then
                                If min1 > R.Absorbance2 Then min1 = R.Absorbance2
                                If max1 < R.Absorbance2 Then max1 = R.Absorbance2
                            End If

                        ElseIf Me.ChartDisplayMode = DisplayModes.DIFF Then
                            If (R.Difference <> -1000) Then
                                If (j = 0 Or min1 > R.Difference) Then
                                    min1 = R.Difference
                                End If
                                If (j = 0 Or max1 < R.Difference) Then
                                    max1 = R.Difference
                                End If
                            End If

                        End If
                        'END AG 16/12/2010

                    End If

                    Me.NumCycles = Me.NumCycles + 1
                End If

                j = j + 1
            Next

            Me.ChartLowerLimit = min1
            Me.ChartUpperLimit = max1
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Fills the Mode ComboBox
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG SG 25/08/2010
    ''' </remarks>
    Private Sub FillModeCombo()
        Try
            Me.bsModoComboBox.Items.Clear()
            Me.bsModoComboBox.Items.Add("Abs1")
            Me.bsModoComboBox.Items.Add("Abs2")
            Me.bsModoComboBox.Items.Add("Abs1 & Abs2")
            Me.bsModoComboBox.Items.Add("Abs1 - Abs2")
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Make a result item invisible
    ''' </summary>
    ''' <param name="pCycle"></param>
    ''' <remarks>
    ''' Created by: SG SG 25/08/2010
    ''' </remarks>
    Private Sub RemoveResultItem(ByVal pCycle As Integer)
        Try
            For Each R As ResultItem In ResultItems
                If (R.Cycle = pCycle) Then
                    R.Visible = False
                    Exit For
                End If
            Next

            PrepareResultItems()
            For Each dgr As DataGridViewRow In Me.bsResultDataGridView.Rows
                If (CStr(dgr.Cells("Cycle").Value) = "<" & pCycle.ToString & ">") Then
                    dgr.Cells("CycleVisible").Value = False

                    dgr.DefaultCellStyle.BackColor = Color.LightGray
                    dgr.DefaultCellStyle.ForeColor = Color.Black
                    dgr.DefaultCellStyle.SelectionBackColor = Color.DarkGray
                    dgr.DefaultCellStyle.SelectionForeColor = Color.White
                    SelectedCycles.Clear()
                    Exit For
                End If
            Next

            bsChartPictureBox.Refresh()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Make a result item visible
    ''' </summary>
    ''' <param name="pCycle"></param>
    ''' <remarks>
    ''' Created by: SG 25/08/2010
    ''' </remarks>
    Private Sub RecoverResultItem(ByVal pCycle As Integer)
        Try
            For Each R As ResultItem In ResultItems
                If (R.Cycle = pCycle) Then
                    R.Visible = True
                    Exit For
                End If
            Next

            PrepareResultItems()
            For Each dgr As DataGridViewRow In Me.bsResultDataGridView.Rows
                If (CStr(dgr.Cells("Cycle").Value) = "<" & pCycle.ToString & ">") Then
                    dgr.Cells("CycleVisible").Value = True

                    dgr.DefaultCellStyle.BackColor = Color.White
                    dgr.DefaultCellStyle.ForeColor = Color.Black
                    dgr.DefaultCellStyle.SelectionBackColor = Color.LightSlateGray
                    dgr.DefaultCellStyle.SelectionForeColor = Color.White
                    SelectedCycles.Clear()
                    Exit For
                End If
            Next

            bsChartPictureBox.Refresh()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Reverse the order of a collection of double variables
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 25/08/2010
    ''' </remarks>
    Private Function ReOrderDoubleList(ByVal pList As List(Of Double)) As List(Of Double)
        Try
            Dim c As Integer = pList.Count - 1
            Dim pList_Copy As New List(Of Double)

            For Each y As Double In pList
                pList_Copy.Add(y)
            Next

            For Each y As Double In pList_Copy
                pList(c) = y
                c = c - 1
            Next

            Return pList
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Returns the first decimal number after the comma
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' Modified by: SA 11/11/2010 - Search position of currently used Decimal Separator instead of search ","
    ''' </remarks>
    Private Function GetFirstDecimalPosition(ByVal pNum As Double) As Integer
        Try
            Dim number As String = pNum.ToString
            Dim posDecimal As Integer = 0
            Dim posComa As Integer = number.IndexOf(DecimalSeparatorAttribute)

            If (posComa <> -1) Then
                If (number.First = "0") Then
                    Dim c As Integer = posComa + 1
                    While number(c) = "0"
                        c = c + 1
                    End While
                    posDecimal = c - posComa
                Else
                    posDecimal = 0
                End If
            End If

            Return posDecimal
        Catch ex As Exception
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Returns the first integer position after the comma
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' Modified by: SA 11/11/2010 - Search position of currently used Decimal Separator instead of search ","
    ''' </remarks>
    Private Function GetFirstIntegerPosition(ByVal pNum As Double) As Integer
        Try
            Dim number As String = pNum.ToString
            Dim posInteger As Integer = 1
            Dim posComa As Integer = number.IndexOf(DecimalSeparatorAttribute)

            If (posComa <> -1) Then
                posInteger = posComa
            Else
                posInteger = number.Length
            End If

            Return posInteger
        Catch ex As Exception
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Formats a number according to it's number of decimals and integers
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 25/08/2010
    ''' </remarks>
    Private Function FormatChartValue(ByVal pNum As Double) As String
        Dim numDecimals As Integer
        Dim format As String = "0."

        Try
            If (pNum < 1) Then
                Dim posDecimal As Integer = GetFirstDecimalPosition(pNum)
                numDecimals = posDecimal + 1
            Else
                numDecimals = 2
            End If

            If (numDecimals < 0) Then numDecimals = 0
            If (numDecimals > 0) Then
                For f As Integer = 1 To numDecimals
                    format = format & "0"
                Next
            Else
                format = ""
            End If

            Return format
        Catch ex As Exception
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Draw a polygon between a line defined by 2 points and a horizontal line
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 25/08/2010
    ''' </remarks>
    Private Sub DrawPolygon4(ByRef myGraphics As Graphics, ByVal myBrush As Brush, ByVal Xo As Integer, ByVal Yo As Integer, ByVal X1 As Integer, ByVal Y1 As Integer, Optional ByVal pRelatedToZero As Boolean = False)
        Try
            Dim myPoints As New List(Of Point)

            myPoints.Add(New Point(Xo, Yo))
            myPoints.Add(New Point(X1, Y1))

            If (pRelatedToZero) Then
                myPoints.Add(New Point(X1, ZeroY))
                myPoints.Add(New Point(Xo, ZeroY))
            Else
                myPoints.Add(New Point(X1, TopY + FrameY))
                myPoints.Add(New Point(Xo, TopY + FrameY))
            End If

            myGraphics.FillPolygon(myBrush, myPoints.ToArray)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Returns a rectangle with a specific origin and size
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 25/08/2010
    ''' </remarks>
    Private Function CreateCircle(ByVal X As Integer, ByVal Y As Integer) As Rectangle
        Try
            Return New Rectangle(X - 3 * Me.ChartPlotPenWidthAttribute, Y - 3 * Me.ChartPlotPenWidthAttribute, 6 * Me.ChartPlotPenWidthAttribute, 6 * Me.ChartPlotPenWidthAttribute)
        Catch ex As Exception
            Throw ex
            Return Nothing
        End Try
    End Function
#End Region

#Region "Events"
    Private Sub ChartPictureBox_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles bsChartPictureBox.Paint
        Try
            myGraphics = e.Graphics

            If (InitialResults IsNot Nothing AndAlso InitialResults.twksAbsorbances.Rows.Count > 0) Then
                If ResultItems.Count > 0 Then
                    'PrepareResultItems()
                    DrawResultChart()
                End If
            Else
                DrawEmptyChart()
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsResultChart_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Me.bsAbsorbanceLabel.Text = ChartVerticalAxisNameAttribute
            Me.bsCyclesLabel.Text = ChartHorizontalAxisNameAttribute
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub BsModoComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsModoComboBox.SelectedIndexChanged
        Try
            Me.bsAbs1Label.Text = Me.ChartAbs1Title
            Me.bsAbs2Label.Text = Me.ChartAbs2Title
            Me.bsDiffLabel.Text = Me.ChartDiffTitle

            Me.BsAbs1PictureBox.BackColor = Me.ChartAbs1ForeColor
            Me.bsAbs2PictureBox.BackColor = Me.ChartAbs2ForeColor
            Me.BsDiffPictureBox.BackColor = Me.ChartDiffForeColor

            If (Me.IsBicromatic) Then
                Select Case (bsModoComboBox.SelectedIndex)
                    Case 0
                        ChartDisplayMode = DisplayModes.ABS1
                        Me.bsAbs1Panel.Location = New Point(5, 7)
                        Me.bsAbs1Panel.Visible = True
                        Me.bsAbs2Panel.Visible = False
                        Me.bsDiffPanel.Visible = False

                    Case 1
                        ChartDisplayMode = DisplayModes.ABS2
                        Me.bsAbs2Panel.Location = New Point(5, 7)
                        Me.bsAbs2Panel.Visible = True
                        Me.bsAbs1Panel.Visible = False
                        Me.bsDiffPanel.Visible = False

                    Case 2
                        ChartDisplayMode = DisplayModes.BOTH
                        Me.bsAbs1Panel.Location = New Point(5, 7)
                        Me.bsAbs2Panel.Location = New Point(161, 7)
                        Me.bsAbs1Panel.Visible = True
                        Me.bsAbs2Panel.Visible = True
                        Me.bsDiffPanel.Visible = False

                    Case 3
                        ChartDisplayMode = DisplayModes.DIFF
                        Me.bsDiffPanel.Location = New Point(5, 7)
                        Me.bsDiffPanel.Visible = True
                        Me.bsAbs1Panel.Visible = False
                        Me.bsAbs2Panel.Visible = False

                    Case Else
                        ChartDisplayMode = DisplayModes.ABS1
                        Me.bsAbs1Panel.Location = New Point(5, 7)
                        Me.bsAbs1Panel.Visible = False
                        Me.bsAbs2Panel.Visible = False
                        Me.bsDiffPanel.Visible = False
                End Select

                PrepareResultItems()
                Me.bsChartPictureBox.Refresh()
                If bsModoComboBox.Visible And bsModoComboBox.Enabled Then bsModoComboBox.Focus() 'AG 16/12/2010

            Else
                ChartDisplayMode = DisplayModes.ABS1
                Me.bsAbs1Panel.Location = New Point(5, 7)
                Me.bsAbs1Panel.Visible = True
                Me.bsAbs2Panel.Visible = False
                Me.bsDiffPanel.Visible = False
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub ResultDataGridView_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsResultDataGridView.SelectionChanged
        Try
            If (Not isDataGridLoading) Then
                If (Me.bsResultDataGridView.Rows.Count > 0 And Me.bsResultDataGridView.SelectedRows.Count > 0) Then
                    SelectedCycles.Clear()

                    Dim pt As Integer = 0
                    For Each P As PointArea In PointAreas
                        If (CStr(Me.bsResultDataGridView.SelectedRows(0).Cells("Cycle").Value) = "<" & P.Item.Cycle.ToString & ">") Then
                            SelectedCycles.Add(pt + 1)
                        End If
                        pt = pt + 1
                    Next

                    If (PointAreas.Count > 0) Then
                        PrepareResultItems()
                        Me.bsChartPictureBox.Refresh()
                    End If
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Show a ToolTip with informartion of the ABS point when the Mouse passes over it
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' Modified by: SA 11/11/2010 - Removed the replace of decimal separator when format the ABS values. Show
    '''                              min and max limit characters also when the graph is not showing Difference
    ''' </remarks>
    Private Sub BsChartPictureBox_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsChartPictureBox.MouseMove
        Try
            Dim isInside As Boolean = False

            Dim pt As Integer
            Dim X As Integer = e.X
            Dim Y As Integer = e.Y
            If Me.CurrentChartMousePoint <> New Point(X, Y) Then
                For Each P As PointArea In PointAreas
                    If (X > P.R.X And X < (P.R.X + P.R.Width)) Then
                        If (Y > P.R.Y And Y < (P.R.Y + P.R.Height)) Then
                            Dim text As String = ""
                            If (P.Type = "Abs1") Then
                                text = "Cycle <" & ResultItems(pt).Cycle.ToString & ">: Absorbance: " & ResultItems(pt).Absorbance1.ToString(ABS_FORMAT) 'AG 16/12/2010 - add the 4th decimal
                            ElseIf (P.Type = "Abs2") Then
                                text = "Cycle <" & ResultItems(pt).Cycle.ToString & ">: Absorbance: " & ResultItems(pt).Absorbance2.ToString(ABS_FORMAT) 'AG 16/12/2010 - add the 4th decimal
                            ElseIf (P.Type = "Diff") Then
                                text = "Cycles <" & ResultItems(pt).Cycle.ToString & "> - <" & ResultItems(pt - 1).Cycle.ToString & ">: Difference: " & ResultItems(pt).Difference.ToString(ABS_FORMAT) 'AG 16/12/2010 - add the 4th decimal
                            End If

                            Me.bsChartToolTip.Show(text, Me, New Point(X + Me.bsChartPanel.Left + 35, Y + Me.bsChartPanel.Top - 10), 5000)
                            isInside = True
                            Exit For
                        End If
                    End If
                    pt = pt + 1
                Next
                If (Not isInside) Then Me.bsChartToolTip.Hide(Me)
            End If

            Me.CurrentChartMousePoint = New Point(X, Y)
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' MouseUp to PopUp the context menu
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' Modified by: SA 11/11/2010 - ContextMenu is hidden temporarily (while the related functionality is not defined)
    ''' </remarks>
    Private Sub BsChartPictureBox_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles bsChartPictureBox.MouseUp
        Try
            If e.Button = Windows.Forms.MouseButtons.Right Then
                'Dim isInside As Boolean = False
                'Dim pt As Integer
                'If Not Me.IsBicromatic Then
                '    If PointAreas.Count > 2 Then
                '        For Each P As PointArea In PointAreas
                '            If e.X > P.R.X And e.X < (P.R.X + P.R.Width) Then
                '                If e.Y > P.R.Y And e.Y < (P.R.Y + P.R.Height) Then
                '                    Me.BsChartToolTip.Hide(Me)
                '                    'Me.BsChartContextMenu.Items(1).Enabled = False
                '                    Me.BsChartContextMenu.Show(BsChartPictureBox, New Point(e.X, e.Y))
                '                    isInside = True
                '                    RemovingPointArea = P
                '                    If RemovingPointArea.Item.Visible Then
                '                        Me.BsChartContextMenu.Items(0).Enabled = True
                '                        Me.BsChartContextMenu.Items(1).Enabled = False
                '                    Else
                '                        Me.BsChartContextMenu.Items(0).Enabled = False
                '                        Me.BsChartContextMenu.Items(1).Enabled = True
                '                    End If

                '                    Exit For
                '                End If
                '            End If
                '            pt = pt + 1
                '        Next
                '    End If
                'End If
                'If Not isInside Then
                '    Me.BsChartContextMenu.Hide()
                '    RemovingPointArea = Nothing
                'End If
            Else
                If (PointAreas.Count > 0) Then
                    For Each P As PointArea In PointAreas
                        If (e.X > P.R.X And e.X < (P.R.X + P.R.Width)) Then
                            If (e.Y > P.R.Y And e.Y < (P.R.Y + P.R.Height)) Then
                                SelectedCycles.Clear()
                                SelectedCycles.Add(P.Item.Cycle)
                                For Each dgr As DataGridViewRow In Me.bsResultDataGridView.Rows
                                    If (CStr(dgr.Cells("Cycle").Value) = "<" & P.Item.Cycle.ToString & ">") Then
                                        If (Me.bsResultDataGridView.SelectedRows.Count > 0) Then
                                            Me.bsResultDataGridView.SelectedRows(0).Selected = False
                                        End If
                                        dgr.Selected = True
                                        Exit For
                                    End If
                                Next
                                Exit For
                            End If
                        End If
                    Next

                    PrepareResultItems()
                    Me.bsChartPictureBox.Refresh()
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Click in option REMOVE in the Context Menu
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 25/08/2010
    ''' </remarks>
    Private Sub RemoveMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveMenuItem.Click
        Try
            If (RemovingPointArea IsNot Nothing And PointAreas.Count > 2) Then
                If (Not Me.IsBicromatic) Then
                    Me.RemoveResultItem(RemovingPointArea.Item.Cycle)
                Else
                    If (Not Me.isEven And RemovingPointArea.Item.Cycle = 1) Then
                        Me.RemoveResultItem(1)
                    Else
                        If (ResultItems(RemovingPointArea.Item.Cycle - 2).Absorbance1 = -1000) Then
                            Me.RemoveResultItem(RemovingPointArea.Item.Cycle)
                            Me.RemoveResultItem(RemovingPointArea.Item.Cycle - 1)
                        Else
                            Me.RemoveResultItem(RemovingPointArea.Item.Cycle)
                            Me.RemoveResultItem(RemovingPointArea.Item.Cycle + 1)
                        End If
                        If (Not Me.isEven And RemovingPointArea.Item.Cycle = 3) Then
                            Me.RemoveResultItem(1)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Click in option RECOVER in the Context Menu
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 25/08/2010
    ''' </remarks>
    Private Sub RecoverMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RecoverCycleToolStripMenuItem.Click
        Try
            If (RemovingPointArea IsNot Nothing) Then
                If (Not Me.IsBicromatic) Then
                    Me.RecoverResultItem(RemovingPointArea.Item.Cycle)
                Else
                    If (Not Me.isEven And RemovingPointArea.Item.Cycle = 1) Then
                        Me.RecoverResultItem(1)
                    Else
                        If (ResultItems(RemovingPointArea.Item.Cycle - 2).Absorbance1 = -1000) Then
                            Me.RecoverResultItem(RemovingPointArea.Item.Cycle)
                            Me.RecoverResultItem(RemovingPointArea.Item.Cycle - 1)
                        Else
                            Me.RecoverResultItem(RemovingPointArea.Item.Cycle)
                            Me.RecoverResultItem(RemovingPointArea.Item.Cycle + 1)
                        End If
                        If (Not Me.isEven And RemovingPointArea.Item.Cycle = 3) Then
                            Me.RecoverResultItem(1)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' MouseUp to PopUp the context menu
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 25/08/2010
    ''' Modified by: SA 11/11/2010 - ContextMenu is hidden temporarily (while the related functionality is not defined)
    ''' </remarks>
    Private Sub BsResultDataGridView_CellMouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsResultDataGridView.CellMouseUp
        Try
            'If e.Button = Windows.Forms.MouseButtons.Right Then
            '    If e.RowIndex >= 0 Then
            '        RemovingPointArea = New PointArea
            '        RemovingPointArea.Item = ResultItems(e.RowIndex)
            '        Dim h As Integer = BsResultDataGridView.RowTemplate.Height * e.RowIndex + e.Y - BsResultDataGridView.VerticalScrollingOffset
            '        If RemovingPointArea.Item.Visible Then
            '            Me.BsChartContextMenu.Items(0).Enabled = True
            '            Me.BsChartContextMenu.Items(1).Enabled = False
            '        Else
            '            Me.BsChartContextMenu.Items(0).Enabled = False
            '            Me.BsChartContextMenu.Items(1).Enabled = True
            '        End If

            '        Me.BsChartContextMenu.Show(BsResultDataGridView, New Point(e.X, h))

            '    End If

            'End If


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Resize the chart when double clicking in it
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 25/08/2010
    ''' </remarks>
    Private Sub BsChartPictureBox_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsChartPictureBox.DoubleClick
        Try
            If (Me.bsResultDataGridView.Visible) Then
                Me.bsChartInfoPanel.Width = Me.Width - 2 * Me.bsChartInfoPanel.Left
                Me.bsChartPanel.Width = Me.Width - 2 * Me.bsChartPanel.Left
                Me.bsResultDataGridView.Visible = False
            Else
                Me.bsChartInfoPanel.Width = CInt(2 * Me.Width / 3)
                Me.bsChartPanel.Width = CInt(2 * Me.Width / 3)
                Me.bsResultDataGridView.Width = Me.bsHeaderPanel.Width - Me.bsChartPanel.Width - Me.bsChartPanel.Left
                Me.bsResultDataGridView.Left = Me.bsChartPanel.Left + Me.bsChartPanel.Width + 3
                Me.bsResultDataGridView.Visible = True
            End If

            FrameX = Me.bsChartPictureBox.Width - 2 * Me.ChartHorizontalMarginAttribute
            FrameY = Me.bsChartPictureBox.Height - 2 * Me.ChartVerticalMarginAttribute

            LeftX = Me.ChartHorizontalMarginAttribute
            TopY = Me.ChartVerticalMarginAttribute
            BottomY = Me.ChartVerticalMarginAttribute + FrameY
            RightX = Me.ChartHorizontalMarginAttribute + FrameX

            PrepareResultItems()
            Me.Refresh()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            RaiseEvent ExitRequest()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region

#Region "TO DELETE??"
    'Private AnalyzerIDAttribute As String
    'Private WorkSessionIDAttribute As String
    'Private CalibNumAttribute As String
    'Private SampleTypeAttribute As String
    'Private TestIDAttribute As String
    'Private WellNumberAttribute As String
    'Private ReplicateNumberAttribute As String
    'Private SampleCaptionAttribute As String = "Sample:"
    'Private TestCaptionAttribute As String = "Test:"
    'Private WellCaptionAttribute As String = "Well/Rotor:"
    'Private ReplicateCaptionAttribute As String = "Replicate:"
    'Private ChartGridHeightAttribute As Integer = 7
    'Private ChartCycleVisibleGridAttribute As String = "CycleVisible"
    'Private ChartCycleGridAttribute As String = "Cycle"
    'Private ChartAbs1GridAttribute As String = "Abs1"
    'Private ChartAbs2GridAttribute As String = "Abs2"
    'Private ChartDiffGridAttribute As String = "Diff"

    'Private Sub UpdateAbsorbancesDS()
    '    Try
    '        AcceptedResults = New AbsorbanceDS

    '        Dim initialDS As AbsorbanceDS = Me.InitialResults

    '        Dim i As Integer = 0
    '        For Each R As ResultItem In ResultItems
    '            If R.Visible Then
    '                Dim dr As AbsorbanceDS.twksAbsorbancesRow = AcceptedResults.twksAbsorbances.NewtwksAbsorbancesRow
    '                dr = initialDS.twksAbsorbances(i)
    '                AcceptedResults.twksAbsorbances.ImportRow(dr)
    '            End If
    '            i = i + 1
    '        Next

    '        AcceptedResults.AcceptChanges()

    '        Dim b As Integer = InitialResults.twksAbsorbances.Count
    '        Dim c As Integer = AcceptedResults.twksAbsorbances.Count

    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub

    'Private Function AnyHasFocus(ByRef pControl As Control) As Boolean
    '    Try
    '        For Each C As Control In pControl.Controls
    '            If AnyHasFocus(C) Then
    '                Return True
    '                Exit For
    '            End If
    '        Next
    '        Return False
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Function
#End Region

End Class


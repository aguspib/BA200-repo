Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Controls.UserControls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IPositionsAdjustments
    Inherits PesentationLayer.BSAdjustmentBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim XyDiagram1 As DevExpress.XtraCharts.XYDiagram = New DevExpress.XtraCharts.XYDiagram
        Dim ConstantLine1 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine2 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine3 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine4 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine5 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine6 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine7 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine8 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine9 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine10 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine11 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine12 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine13 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine14 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine15 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine16 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine17 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim Series1 As DevExpress.XtraCharts.Series = New DevExpress.XtraCharts.Series
        Dim PointSeriesLabel1 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel
        Dim SplineSeriesView1 As DevExpress.XtraCharts.SplineSeriesView = New DevExpress.XtraCharts.SplineSeriesView
        Dim Series2 As DevExpress.XtraCharts.Series = New DevExpress.XtraCharts.Series
        Dim PointSeriesLabel2 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel
        Dim SplineSeriesView2 As DevExpress.XtraCharts.SplineSeriesView = New DevExpress.XtraCharts.SplineSeriesView
        Dim PointSeriesLabel3 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel
        Dim SplineSeriesView3 As DevExpress.XtraCharts.SplineSeriesView = New DevExpress.XtraCharts.SplineSeriesView
        Dim ChartTitle1 As DevExpress.XtraCharts.ChartTitle = New DevExpress.XtraCharts.ChartTitle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IPositionsAdjustments))
        Dim SwiftPlotDiagram1 As DevExpress.XtraCharts.SwiftPlotDiagram = New DevExpress.XtraCharts.SwiftPlotDiagram
        Dim ConstantLine18 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine19 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine20 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine21 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine22 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine23 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine24 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine25 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine26 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine27 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine28 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine29 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim Series3 As DevExpress.XtraCharts.Series = New DevExpress.XtraCharts.Series
        Dim SwiftPlotSeriesView1 As DevExpress.XtraCharts.SwiftPlotSeriesView = New DevExpress.XtraCharts.SwiftPlotSeriesView
        Dim SwiftPlotSeriesView2 As DevExpress.XtraCharts.SwiftPlotSeriesView = New DevExpress.XtraCharts.SwiftPlotSeriesView
        Dim ChartTitle2 As DevExpress.XtraCharts.ChartTitle = New DevExpress.XtraCharts.ChartTitle
        Me.BsTabPagesControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl
        Me.TabOpticCentering = New System.Windows.Forms.TabPage
        Me.BsOpticInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoOptXPSViewer = New BsXPSViewer
        Me.BsOpticInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsOpticAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsLEDCurrentLabel = New System.Windows.Forms.Label
        Me.BsLEDCurrentTrackBar = New System.Windows.Forms.TrackBar
        Me.BsMinusLabel = New System.Windows.Forms.Label
        Me.BsPlusLabel = New System.Windows.Forms.Label
        Me.BsAbsorbancePanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.AbsorbanceChart = New DevExpress.XtraCharts.ChartControl
        Me.BsOpticAdjustGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.BsOpticAdjustButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsOpticStopButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsOpticCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustOptic = New Biosystems.Ax00.Controls.UserControls.BSAdjustControl
        Me.BsOpticAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BSOpticWSGroupBox = New System.Windows.Forms.GroupBox
        Me.BsOpticWSLabel = New System.Windows.Forms.Label
        Me.BsButton4 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsUpDownWSButton1 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsEncoderAdjustmentTitle = New System.Windows.Forms.Label
        Me.BsEncoderAdjustmentLabel = New System.Windows.Forms.Label
        Me.BsOpticAdjustmentTitle = New System.Windows.Forms.Label
        Me.BsOpticAdjustmentLabel = New System.Windows.Forms.Label
        Me.TabWashingStation = New System.Windows.Forms.TabPage
        Me.BsWashingInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoWsXPSViewer = New BsXPSViewer
        Me.BsWashingInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsWashingAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsWashingAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BSWSWSGroupBox = New System.Windows.Forms.GroupBox
        Me.BsWSWSLabel = New System.Windows.Forms.Label
        Me.BsUpDownWSButton2 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsButton3 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsWSAdjustGroupBox = New System.Windows.Forms.GroupBox
        Me.BsWSAdjustButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsWSCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustWashing = New Biosystems.Ax00.Controls.UserControls.BSAdjustControl
        Me.BsWashingAdjustmentTitle = New System.Windows.Forms.Label
        Me.BsWashingAdjustmentLabel = New System.Windows.Forms.Label
        Me.TabArmsPositions = New System.Windows.Forms.TabPage
        Me.BsArmsInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoArmsXPSViewer = New BsXPSViewer
        Me.BsArmsInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsArmsAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsArmsAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsArmsOkButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsArmsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsArmsWSGroupBox = New System.Windows.Forms.GroupBox
        Me.BsArmsWSLabel = New System.Windows.Forms.Label
        Me.BsUpDownWSButton3 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsButton6 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustRotor = New Biosystems.Ax00.Controls.UserControls.BSAdjustControl
        Me.BsAdjustZ = New Biosystems.Ax00.Controls.UserControls.BSAdjustControl
        Me.BsAdjustPolar = New Biosystems.Ax00.Controls.UserControls.BSAdjustControl
        Me.BsTabArmsControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl
        Me.TabSample = New System.Windows.Forms.TabPage
        Me.BsGridSample = New BSGridControl
        Me.TabReagent1 = New System.Windows.Forms.TabPage
        Me.BsGridReagent1 = New BSGridControl
        Me.TabReagent2 = New System.Windows.Forms.TabPage
        Me.BsGridReagent2 = New BSGridControl
        Me.TabMixer1 = New System.Windows.Forms.TabPage
        Me.BsStirrer1Button = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsGridMixer1 = New BSGridControl
        Me.TabMixer2 = New System.Windows.Forms.TabPage
        Me.BsStirrer2Button = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsGridMixer2 = New BSGridControl
        Me.TabPageTODELETE = New System.Windows.Forms.TabPage
        Me.BsPanel1 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsButton1 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.TrackBar1 = New System.Windows.Forms.TrackBar
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.BsPanel2 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ChartControl1 = New DevExpress.XtraCharts.ChartControl
        Me.BsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsAdjustControl1 = New Biosystems.Ax00.Controls.UserControls.BSAdjustControl
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsResponse = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.ToolTipController1 = New DevExpress.Utils.ToolTipController(Me.components)
        Me.BsTabPagesControl.SuspendLayout()
        Me.TabOpticCentering.SuspendLayout()
        Me.BsOpticInfoPanel.SuspendLayout()
        Me.BsOpticAdjustPanel.SuspendLayout()
        CType(Me.BsLEDCurrentTrackBar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsAbsorbancePanel.SuspendLayout()
        CType(Me.AbsorbanceChart, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(SplineSeriesView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Series2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(SplineSeriesView2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(SplineSeriesView3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsOpticAdjustGroupBox.SuspendLayout()
        Me.BSOpticWSGroupBox.SuspendLayout()
        Me.TabWashingStation.SuspendLayout()
        Me.BsWashingInfoPanel.SuspendLayout()
        Me.BsWashingAdjustPanel.SuspendLayout()
        Me.BSWSWSGroupBox.SuspendLayout()
        Me.BsWSAdjustGroupBox.SuspendLayout()
        Me.TabArmsPositions.SuspendLayout()
        Me.BsArmsInfoPanel.SuspendLayout()
        Me.BsArmsAdjustPanel.SuspendLayout()
        Me.BsArmsWSGroupBox.SuspendLayout()
        Me.BsTabArmsControl.SuspendLayout()
        Me.TabSample.SuspendLayout()
        Me.TabReagent1.SuspendLayout()
        Me.TabReagent2.SuspendLayout()
        Me.TabMixer1.SuspendLayout()
        Me.TabMixer2.SuspendLayout()
        Me.TabPageTODELETE.SuspendLayout()
        Me.BsPanel1.SuspendLayout()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsPanel2.SuspendLayout()
        CType(Me.ChartControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(SwiftPlotDiagram1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Series3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(SwiftPlotSeriesView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(SwiftPlotSeriesView2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsButtonsPanel.SuspendLayout()
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsTabPagesControl
        '
        Me.BsTabPagesControl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTabPagesControl.Controls.Add(Me.TabOpticCentering)
        Me.BsTabPagesControl.Controls.Add(Me.TabWashingStation)
        Me.BsTabPagesControl.Controls.Add(Me.TabArmsPositions)
        Me.BsTabPagesControl.Controls.Add(Me.TabPageTODELETE)
        Me.BsTabPagesControl.Location = New System.Drawing.Point(0, 0)
        Me.BsTabPagesControl.Name = "BsTabPagesControl"
        Me.BsTabPagesControl.SelectedIndex = 0
        Me.BsTabPagesControl.Size = New System.Drawing.Size(978, 558)
        Me.BsTabPagesControl.TabIndex = 14
        '
        'TabOpticCentering
        '
        Me.TabOpticCentering.Controls.Add(Me.BsOpticInfoPanel)
        Me.TabOpticCentering.Controls.Add(Me.BsOpticAdjustPanel)
        Me.TabOpticCentering.Location = New System.Drawing.Point(4, 22)
        Me.TabOpticCentering.Name = "TabOpticCentering"
        Me.TabOpticCentering.Padding = New System.Windows.Forms.Padding(3)
        Me.TabOpticCentering.Size = New System.Drawing.Size(970, 532)
        Me.TabOpticCentering.TabIndex = 0
        Me.TabOpticCentering.Text = "Optic Centering"
        Me.TabOpticCentering.UseVisualStyleBackColor = True
        '
        'BsOpticInfoPanel
        '
        Me.BsOpticInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsOpticInfoPanel.Controls.Add(Me.BsInfoOptXPSViewer)
        Me.BsOpticInfoPanel.Controls.Add(Me.BsOpticInfoTitle)
        Me.BsOpticInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsOpticInfoPanel.Name = "BsOpticInfoPanel"
        Me.BsOpticInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsOpticInfoPanel.TabIndex = 25
        '
        'BsInfoOptXPSViewer
        '
        Me.BsInfoOptXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoOptXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoOptXPSViewer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsInfoOptXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoOptXPSViewer.CopyButtonVisible = True
        Me.BsInfoOptXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoOptXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoOptXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoOptXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoOptXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoOptXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoOptXPSViewer.HorizontalPageMargin = 10
        Me.BsInfoOptXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoOptXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoOptXPSViewer.IsScrollable = False
        Me.BsInfoOptXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoOptXPSViewer.MenuBarVisible = False
        Me.BsInfoOptXPSViewer.Name = "BsInfoOptXPSViewer"
        Me.BsInfoOptXPSViewer.PopupMenuEnabled = True
        Me.BsInfoOptXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoOptXPSViewer.PrintButtonVisible = True
        Me.BsInfoOptXPSViewer.SearchBarVisible = False
        Me.BsInfoOptXPSViewer.Size = New System.Drawing.Size(230, 509)
        Me.BsInfoOptXPSViewer.TabIndex = 34
        Me.BsInfoOptXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoOptXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoOptXPSViewer.VerticalPageMargin = 10
        Me.BsInfoOptXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoOptXPSViewer.WholePageButtonVisible = True
        '
        'BsOpticInfoTitle
        '
        Me.BsOpticInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsOpticInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsOpticInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsOpticInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsOpticInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsOpticInfoTitle.Name = "BsOpticInfoTitle"
        Me.BsOpticInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsOpticInfoTitle.TabIndex = 22
        Me.BsOpticInfoTitle.Text = "Information"
        Me.BsOpticInfoTitle.Title = True
        '
        'BsOpticAdjustPanel
        '
        Me.BsOpticAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsOpticAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsOpticAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsLEDCurrentLabel)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsLEDCurrentTrackBar)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsMinusLabel)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsPlusLabel)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsAbsorbancePanel)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsOpticAdjustGroupBox)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsOpticAdjustTitle)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BSOpticWSGroupBox)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsEncoderAdjustmentTitle)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsEncoderAdjustmentLabel)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsOpticAdjustmentTitle)
        Me.BsOpticAdjustPanel.Controls.Add(Me.BsOpticAdjustmentLabel)
        Me.BsOpticAdjustPanel.Location = New System.Drawing.Point(231, 0)
        Me.BsOpticAdjustPanel.Name = "BsOpticAdjustPanel"
        Me.BsOpticAdjustPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsOpticAdjustPanel.TabIndex = 21
        '
        'BsLEDCurrentLabel
        '
        Me.BsLEDCurrentLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsLEDCurrentLabel.ForeColor = System.Drawing.Color.Black
        Me.BsLEDCurrentLabel.Location = New System.Drawing.Point(31, 419)
        Me.BsLEDCurrentLabel.Name = "BsLEDCurrentLabel"
        Me.BsLEDCurrentLabel.Size = New System.Drawing.Size(120, 20)
        Me.BsLEDCurrentLabel.TabIndex = 44
        Me.BsLEDCurrentLabel.Text = "LED Current"
        Me.BsLEDCurrentLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'BsLEDCurrentTrackBar
        '
        Me.BsLEDCurrentTrackBar.AutoSize = False
        Me.BsLEDCurrentTrackBar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BsLEDCurrentTrackBar.Enabled = False
        Me.BsLEDCurrentTrackBar.LargeChange = 1000
        Me.BsLEDCurrentTrackBar.Location = New System.Drawing.Point(31, 430)
        Me.BsLEDCurrentTrackBar.Margin = New System.Windows.Forms.Padding(0)
        Me.BsLEDCurrentTrackBar.Maximum = 30000
        Me.BsLEDCurrentTrackBar.Name = "BsLEDCurrentTrackBar"
        Me.BsLEDCurrentTrackBar.Size = New System.Drawing.Size(120, 28)
        Me.BsLEDCurrentTrackBar.TabIndex = 39
        Me.BsLEDCurrentTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        Me.BsLEDCurrentTrackBar.Value = 8000
        '
        'BsMinusLabel
        '
        Me.BsMinusLabel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BsMinusLabel.Font = New System.Drawing.Font("Verdana", 12.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsMinusLabel.ForeColor = System.Drawing.Color.DimGray
        Me.BsMinusLabel.Location = New System.Drawing.Point(13, 431)
        Me.BsMinusLabel.Name = "BsMinusLabel"
        Me.BsMinusLabel.Size = New System.Drawing.Size(20, 20)
        Me.BsMinusLabel.TabIndex = 42
        Me.BsMinusLabel.Text = "-"
        Me.BsMinusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BsPlusLabel
        '
        Me.BsPlusLabel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BsPlusLabel.Font = New System.Drawing.Font("Verdana", 12.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsPlusLabel.ForeColor = System.Drawing.Color.DimGray
        Me.BsPlusLabel.Location = New System.Drawing.Point(150, 430)
        Me.BsPlusLabel.Name = "BsPlusLabel"
        Me.BsPlusLabel.Size = New System.Drawing.Size(20, 20)
        Me.BsPlusLabel.TabIndex = 43
        Me.BsPlusLabel.Text = "+"
        Me.BsPlusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BsAbsorbancePanel
        '
        Me.BsAbsorbancePanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAbsorbancePanel.Controls.Add(Me.AbsorbanceChart)
        Me.BsAbsorbancePanel.Location = New System.Drawing.Point(7, 68)
        Me.BsAbsorbancePanel.Name = "BsAbsorbancePanel"
        Me.BsAbsorbancePanel.Size = New System.Drawing.Size(723, 339)
        Me.BsAbsorbancePanel.TabIndex = 34
        '
        'AbsorbanceChart
        '
        Me.AbsorbanceChart.AppearanceName = "Dark Flat"
        Me.AbsorbanceChart.BackColor = System.Drawing.Color.Gainsboro
        Me.AbsorbanceChart.BorderOptions.Color = System.Drawing.Color.White
        XyDiagram1.AxisX.Color = System.Drawing.Color.Gray
        ConstantLine1.AxisValueSerializable = "0"
        ConstantLine1.Color = System.Drawing.Color.DimGray
        ConstantLine1.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.Solid
        ConstantLine1.LineStyle.Thickness = 8
        ConstantLine1.Name = "OldWall1"
        ConstantLine1.ShowBehind = True
        ConstantLine1.ShowInLegend = False
        ConstantLine1.Title.Visible = False
        ConstantLine1.Visible = False
        ConstantLine2.AxisValueSerializable = "80"
        ConstantLine2.Color = System.Drawing.Color.DimGray
        ConstantLine2.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.Solid
        ConstantLine2.LineStyle.Thickness = 8
        ConstantLine2.Name = "OldWall2"
        ConstantLine2.ShowBehind = True
        ConstantLine2.ShowInLegend = False
        ConstantLine2.Title.Visible = False
        ConstantLine2.Visible = False
        ConstantLine3.AxisValueSerializable = "160"
        ConstantLine3.Color = System.Drawing.Color.DimGray
        ConstantLine3.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.Solid
        ConstantLine3.LineStyle.Thickness = 8
        ConstantLine3.Name = "OldWall3"
        ConstantLine3.ShowBehind = True
        ConstantLine3.ShowInLegend = False
        ConstantLine3.Title.Visible = False
        ConstantLine3.Visible = False
        ConstantLine4.AxisValueSerializable = "240"
        ConstantLine4.Color = System.Drawing.Color.DimGray
        ConstantLine4.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.Solid
        ConstantLine4.LineStyle.Thickness = 8
        ConstantLine4.Name = "OldWall4"
        ConstantLine4.ShowBehind = True
        ConstantLine4.ShowInLegend = False
        ConstantLine4.Title.Visible = False
        ConstantLine4.Visible = False
        ConstantLine5.AxisValueSerializable = "320"
        ConstantLine5.Color = System.Drawing.Color.DimGray
        ConstantLine5.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.Solid
        ConstantLine5.LineStyle.Thickness = 8
        ConstantLine5.Name = "OldWall5"
        ConstantLine5.ShowBehind = True
        ConstantLine5.ShowInLegend = False
        ConstantLine5.Title.Visible = False
        ConstantLine5.Visible = False
        ConstantLine6.AxisValueSerializable = "400"
        ConstantLine6.Color = System.Drawing.Color.DimGray
        ConstantLine6.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.Solid
        ConstantLine6.LineStyle.Thickness = 8
        ConstantLine6.Name = "OldWall6"
        ConstantLine6.ShowBehind = True
        ConstantLine6.ShowInLegend = False
        ConstantLine6.Title.Visible = False
        ConstantLine6.Visible = False
        ConstantLine7.AxisValueSerializable = "0"
        ConstantLine7.Color = System.Drawing.Color.LightGray
        ConstantLine7.LineStyle.Thickness = 4
        ConstantLine7.Name = "NewWall1"
        ConstantLine7.ShowBehind = True
        ConstantLine7.ShowInLegend = False
        ConstantLine7.Title.Visible = False
        ConstantLine7.Visible = False
        ConstantLine8.AxisValueSerializable = "80"
        ConstantLine8.Color = System.Drawing.Color.LightGray
        ConstantLine8.LineStyle.Thickness = 4
        ConstantLine8.Name = "NewWall2"
        ConstantLine8.ShowBehind = True
        ConstantLine8.ShowInLegend = False
        ConstantLine8.Title.Visible = False
        ConstantLine8.Visible = False
        ConstantLine9.AxisValueSerializable = "160"
        ConstantLine9.Color = System.Drawing.Color.LightGray
        ConstantLine9.LineStyle.Thickness = 4
        ConstantLine9.Name = "NewWall3"
        ConstantLine9.ShowBehind = True
        ConstantLine9.ShowInLegend = False
        ConstantLine9.Title.Visible = False
        ConstantLine9.Visible = False
        ConstantLine10.AxisValueSerializable = "240"
        ConstantLine10.Color = System.Drawing.Color.LightGray
        ConstantLine10.LineStyle.Thickness = 4
        ConstantLine10.Name = "NewWall4"
        ConstantLine10.ShowBehind = True
        ConstantLine10.ShowInLegend = False
        ConstantLine10.Title.Visible = False
        ConstantLine10.Visible = False
        ConstantLine11.AxisValueSerializable = "320"
        ConstantLine11.Color = System.Drawing.Color.LightGray
        ConstantLine11.LineStyle.Thickness = 4
        ConstantLine11.Name = "NewWall5"
        ConstantLine11.ShowBehind = True
        ConstantLine11.ShowInLegend = False
        ConstantLine11.Title.Visible = False
        ConstantLine11.Visible = False
        ConstantLine12.AxisValueSerializable = "400"
        ConstantLine12.Color = System.Drawing.Color.Silver
        ConstantLine12.LineStyle.Thickness = 4
        ConstantLine12.Name = "NewWall6"
        ConstantLine12.ShowBehind = True
        ConstantLine12.ShowInLegend = False
        ConstantLine12.Title.Visible = False
        ConstantLine12.Visible = False
        ConstantLine13.AxisValueSerializable = "40"
        ConstantLine13.Color = System.Drawing.Color.Black
        ConstantLine13.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.DashDot
        ConstantLine13.Name = "CenterWell1"
        ConstantLine13.ShowBehind = True
        ConstantLine13.ShowInLegend = False
        ConstantLine13.Title.Visible = False
        ConstantLine14.AxisValueSerializable = "120"
        ConstantLine14.Color = System.Drawing.Color.Black
        ConstantLine14.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.DashDot
        ConstantLine14.Name = "CenterWell2"
        ConstantLine14.ShowBehind = True
        ConstantLine14.ShowInLegend = False
        ConstantLine14.Title.Visible = False
        ConstantLine15.AxisValueSerializable = "200"
        ConstantLine15.Color = System.Drawing.Color.Black
        ConstantLine15.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.DashDot
        ConstantLine15.Name = "CenterWell3"
        ConstantLine15.ShowBehind = True
        ConstantLine15.ShowInLegend = False
        ConstantLine15.Title.Visible = False
        ConstantLine16.AxisValueSerializable = "280"
        ConstantLine16.Color = System.Drawing.Color.Black
        ConstantLine16.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.DashDot
        ConstantLine16.Name = "CenterWell4"
        ConstantLine16.ShowBehind = True
        ConstantLine16.ShowInLegend = False
        ConstantLine16.Title.Visible = False
        ConstantLine17.AxisValueSerializable = "360"
        ConstantLine17.Color = System.Drawing.Color.Black
        ConstantLine17.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.DashDot
        ConstantLine17.Name = "CenterWell5"
        ConstantLine17.ShowBehind = True
        ConstantLine17.ShowInLegend = False
        ConstantLine17.Title.Visible = False
        XyDiagram1.AxisX.ConstantLines.AddRange(New DevExpress.XtraCharts.ConstantLine() {ConstantLine1, ConstantLine2, ConstantLine3, ConstantLine4, ConstantLine5, ConstantLine6, ConstantLine7, ConstantLine8, ConstantLine9, ConstantLine10, ConstantLine11, ConstantLine12, ConstantLine13, ConstantLine14, ConstantLine15, ConstantLine16, ConstantLine17})
        XyDiagram1.AxisX.GridSpacing = 40
        XyDiagram1.AxisX.GridSpacingAuto = False
        XyDiagram1.AxisX.Label.Font = New System.Drawing.Font("Verdana", 8.0!)
        XyDiagram1.AxisX.Label.TextColor = System.Drawing.Color.Black
        XyDiagram1.AxisX.Range.Auto = False
        XyDiagram1.AxisX.Range.MaxValueSerializable = "400"
        XyDiagram1.AxisX.Range.MinValueSerializable = "0"
        XyDiagram1.AxisX.Range.ScrollingRange.SideMarginsEnabled = True
        XyDiagram1.AxisX.Range.SideMarginsEnabled = True
        XyDiagram1.AxisX.Title.Font = New System.Drawing.Font("Verdana", 8.0!)
        XyDiagram1.AxisX.Title.Text = "steps"
        XyDiagram1.AxisX.Title.TextColor = System.Drawing.Color.Black
        XyDiagram1.AxisX.Title.Visible = True
        XyDiagram1.AxisX.VisibleInPanesSerializable = "-1"
        XyDiagram1.AxisY.Color = System.Drawing.Color.Gray
        XyDiagram1.AxisY.GridLines.Color = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        XyDiagram1.AxisY.Label.Antialiasing = True
        XyDiagram1.AxisY.Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        XyDiagram1.AxisY.Label.TextColor = System.Drawing.Color.Black
        XyDiagram1.AxisY.Range.Auto = False
        XyDiagram1.AxisY.Range.MaxValueSerializable = "1100000"
        XyDiagram1.AxisY.Range.MinValueSerializable = "500000"
        XyDiagram1.AxisY.Range.ScrollingRange.SideMarginsEnabled = True
        XyDiagram1.AxisY.Range.SideMarginsEnabled = True
        XyDiagram1.AxisY.Title.Font = New System.Drawing.Font("Verdana", 8.0!)
        XyDiagram1.AxisY.Title.Text = "counts"
        XyDiagram1.AxisY.Title.TextColor = System.Drawing.Color.Black
        XyDiagram1.AxisY.Title.Visible = True
        XyDiagram1.AxisY.VisibleInPanesSerializable = "-1"
        XyDiagram1.DefaultPane.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        XyDiagram1.DefaultPane.BorderColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.AbsorbanceChart.Diagram = XyDiagram1
        Me.AbsorbanceChart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AbsorbanceChart.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Right
        Me.AbsorbanceChart.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Bottom
        Me.AbsorbanceChart.Legend.Antialiasing = True
        Me.AbsorbanceChart.Legend.BackColor = System.Drawing.Color.Transparent
        Me.AbsorbanceChart.Legend.BackImage.Stretch = True
        Me.AbsorbanceChart.Legend.Border.Visible = False
        Me.AbsorbanceChart.Legend.Font = New System.Drawing.Font("Verdana", 8.0!)
        Me.AbsorbanceChart.Legend.TextColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.AbsorbanceChart.Location = New System.Drawing.Point(0, 0)
        Me.AbsorbanceChart.Name = "AbsorbanceChart"
        Me.AbsorbanceChart.Padding.Bottom = 10
        Me.AbsorbanceChart.Padding.Left = 10
        Me.AbsorbanceChart.Padding.Right = 10
        Me.AbsorbanceChart.Padding.Top = 10
        Me.AbsorbanceChart.PaletteBaseColorNumber = 2
        Me.AbsorbanceChart.PaletteName = "Nature Colors"
        Series1.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Numerical
        PointSeriesLabel1.LineVisible = True
        PointSeriesLabel1.Visible = False
        Series1.Label = PointSeriesLabel1
        Series1.LegendText = "Absorbance"
        Series1.Name = "Absorbance"
        SplineSeriesView1.Color = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        SplineSeriesView1.LineMarkerOptions.Size = 3
        SplineSeriesView1.LineStyle.Thickness = 3
        Series1.View = SplineSeriesView1
        Series2.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Numerical
        PointSeriesLabel2.BackColor = System.Drawing.Color.SteelBlue
        PointSeriesLabel2.Border.Color = System.Drawing.Color.SteelBlue
        PointSeriesLabel2.FillStyle.FillMode = DevExpress.XtraCharts.FillMode.Hatch
        PointSeriesLabel2.LineColor = System.Drawing.Color.White
        PointSeriesLabel2.LineVisible = True
        PointSeriesLabel2.TextColor = System.Drawing.Color.White
        Series2.Label = PointSeriesLabel2
        Series2.LegendText = "Encoder"
        Series2.Name = "Encoder"
        SplineSeriesView2.Color = System.Drawing.Color.Silver
        SplineSeriesView2.LineMarkerOptions.Size = 2
        SplineSeriesView2.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.Dot
        SplineSeriesView2.LineStyle.Thickness = 1
        Series2.View = SplineSeriesView2
        Me.AbsorbanceChart.SeriesSerializable = New DevExpress.XtraCharts.Series() {Series1, Series2}
        PointSeriesLabel3.LineVisible = True
        Me.AbsorbanceChart.SeriesTemplate.Label = PointSeriesLabel3
        SplineSeriesView3.LineStyle.Thickness = 1
        Me.AbsorbanceChart.SeriesTemplate.View = SplineSeriesView3
        Me.AbsorbanceChart.Size = New System.Drawing.Size(723, 339)
        Me.AbsorbanceChart.SmallChartText.Font = New System.Drawing.Font("Verdana", 12.0!)
        Me.AbsorbanceChart.SmallChartText.Text = ""
        Me.AbsorbanceChart.TabIndex = 2
        ChartTitle1.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ChartTitle1.Text = "Absorbance"
        ChartTitle1.TextColor = System.Drawing.Color.Black
        ChartTitle1.Visible = False
        Me.AbsorbanceChart.Titles.AddRange(New DevExpress.XtraCharts.ChartTitle() {ChartTitle1})
        '
        'BsOpticAdjustGroupBox
        '
        Me.BsOpticAdjustGroupBox.Controls.Add(Me.BsOpticAdjustButton)
        Me.BsOpticAdjustGroupBox.Controls.Add(Me.BsOpticStopButton)
        Me.BsOpticAdjustGroupBox.Controls.Add(Me.BsOpticCancelButton)
        Me.BsOpticAdjustGroupBox.Controls.Add(Me.BsAdjustOptic)
        Me.BsOpticAdjustGroupBox.ForeColor = System.Drawing.Color.Black
        Me.BsOpticAdjustGroupBox.Location = New System.Drawing.Point(524, 404)
        Me.BsOpticAdjustGroupBox.Name = "BsOpticAdjustGroupBox"
        Me.BsOpticAdjustGroupBox.Size = New System.Drawing.Size(208, 122)
        Me.BsOpticAdjustGroupBox.TabIndex = 76
        Me.BsOpticAdjustGroupBox.TabStop = False
        '
        'BsOpticAdjustButton
        '
        Me.BsOpticAdjustButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsOpticAdjustButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsOpticAdjustButton.Enabled = False
        Me.BsOpticAdjustButton.Location = New System.Drawing.Point(167, 15)
        Me.BsOpticAdjustButton.Name = "BsOpticAdjustButton"
        Me.BsOpticAdjustButton.Size = New System.Drawing.Size(32, 32)
        Me.BsOpticAdjustButton.TabIndex = 0
        Me.BsOpticAdjustButton.UseVisualStyleBackColor = True
        '
        'BsOpticStopButton
        '
        Me.BsOpticStopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsOpticStopButton.Location = New System.Drawing.Point(167, 15)
        Me.BsOpticStopButton.Name = "BsOpticStopButton"
        Me.BsOpticStopButton.Size = New System.Drawing.Size(30, 30)
        Me.BsOpticStopButton.TabIndex = 4
        Me.BsOpticStopButton.UseVisualStyleBackColor = True
        Me.BsOpticStopButton.Visible = False
        '
        'BsOpticCancelButton
        '
        Me.BsOpticCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsOpticCancelButton.Enabled = False
        Me.BsOpticCancelButton.Location = New System.Drawing.Point(167, 51)
        Me.BsOpticCancelButton.Name = "BsOpticCancelButton"
        Me.BsOpticCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.BsOpticCancelButton.TabIndex = 2
        Me.BsOpticCancelButton.UseVisualStyleBackColor = True
        '
        'BsAdjustOptic
        '
        Me.BsAdjustOptic.AdjustButtonMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.AdjustButtonModes.LeftRight
        Me.BsAdjustOptic.AdjustingEnabled = True
        Me.BsAdjustOptic.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustOptic.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustOptic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.BsAdjustOptic.CurrentStepValue = 1
        Me.BsAdjustOptic.CurrentValue = 0.0!
        Me.BsAdjustOptic.DisplayBackColor = System.Drawing.Color.Black
        Me.BsAdjustOptic.DisplayEditingForeColor = System.Drawing.Color.White
        Me.BsAdjustOptic.DisplayForeColor = System.Drawing.Color.LightGreen
        Me.BsAdjustOptic.EditingEnabled = True
        Me.BsAdjustOptic.EditionMode = False
        Me.BsAdjustOptic.Enabled = False
        Me.BsAdjustOptic.FocusedBackColor = System.Drawing.Color.Blue
        Me.BsAdjustOptic.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!)
        Me.BsAdjustOptic.HomingEnabled = True
        Me.BsAdjustOptic.IncreaseMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.IncreaseModes.Direct
        Me.BsAdjustOptic.InfoBackColor = System.Drawing.Color.LightSteelBlue
        Me.BsAdjustOptic.InfoTitlesForeColor = System.Drawing.Color.Black
        Me.BsAdjustOptic.InfoValuesForeColor = System.Drawing.Color.Black
        Me.BsAdjustOptic.IsFocused = False
        Me.BsAdjustOptic.LastValueSavedTitle = "Last:"
        Me.BsAdjustOptic.Location = New System.Drawing.Point(9, 12)
        Me.BsAdjustOptic.MaximumLimit = 50000.0!
        Me.BsAdjustOptic.MaximumSize = New System.Drawing.Size(153, 127)
        Me.BsAdjustOptic.MaxNumDecimals = 0
        Me.BsAdjustOptic.MinimumLimit = 0.0!
        Me.BsAdjustOptic.MinimumSize = New System.Drawing.Size(153, 87)
        Me.BsAdjustOptic.Name = "BsAdjustOptic"
        Me.BsAdjustOptic.RangeTitle = "Range:"
        Me.BsAdjustOptic.SimulationMode = False
        Me.BsAdjustOptic.Size = New System.Drawing.Size(153, 106)
        Me.BsAdjustOptic.StepValues = CType(resources.GetObject("BsAdjustOptic.StepValues"), System.Collections.Generic.List(Of Integer))
        Me.BsAdjustOptic.TabIndex = 3
        Me.BsAdjustOptic.UnFocusedBackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustOptic.UnitsCaption = "steps"
        '
        'BsOpticAdjustTitle
        '
        Me.BsOpticAdjustTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsOpticAdjustTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsOpticAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.BsOpticAdjustTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsOpticAdjustTitle.Name = "BsOpticAdjustTitle"
        Me.BsOpticAdjustTitle.Size = New System.Drawing.Size(739, 20)
        Me.BsOpticAdjustTitle.TabIndex = 31
        Me.BsOpticAdjustTitle.Text = "Optic Centering Adjustment"
        Me.BsOpticAdjustTitle.Title = True
        '
        'BSOpticWSGroupBox
        '
        Me.BSOpticWSGroupBox.Controls.Add(Me.BsOpticWSLabel)
        Me.BSOpticWSGroupBox.Controls.Add(Me.BsButton4)
        Me.BSOpticWSGroupBox.Controls.Add(Me.BsUpDownWSButton1)
        Me.BSOpticWSGroupBox.Location = New System.Drawing.Point(7, 17)
        Me.BSOpticWSGroupBox.Name = "BSOpticWSGroupBox"
        Me.BSOpticWSGroupBox.Size = New System.Drawing.Size(725, 45)
        Me.BSOpticWSGroupBox.TabIndex = 75
        Me.BSOpticWSGroupBox.TabStop = False
        '
        'BsOpticWSLabel
        '
        Me.BsOpticWSLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsOpticWSLabel.Location = New System.Drawing.Point(6, 18)
        Me.BsOpticWSLabel.Name = "BsOpticWSLabel"
        Me.BsOpticWSLabel.Size = New System.Drawing.Size(679, 19)
        Me.BsOpticWSLabel.TabIndex = 108
        Me.BsOpticWSLabel.Text = "Place the Reactions Rotor"
        Me.BsOpticWSLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BsButton4
        '
        Me.BsButton4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton4.Location = New System.Drawing.Point(690, 228)
        Me.BsButton4.Name = "BsButton4"
        Me.BsButton4.Size = New System.Drawing.Size(30, 30)
        Me.BsButton4.TabIndex = 106
        Me.BsButton4.UseVisualStyleBackColor = True
        '
        'BsUpDownWSButton1
        '
        Me.BsUpDownWSButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsUpDownWSButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsUpDownWSButton1.Location = New System.Drawing.Point(690, 10)
        Me.BsUpDownWSButton1.Name = "BsUpDownWSButton1"
        Me.BsUpDownWSButton1.Size = New System.Drawing.Size(32, 32)
        Me.BsUpDownWSButton1.TabIndex = 45
        Me.BsUpDownWSButton1.UseVisualStyleBackColor = True
        '
        'BsEncoderAdjustmentTitle
        '
        Me.BsEncoderAdjustmentTitle.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsEncoderAdjustmentTitle.Location = New System.Drawing.Point(202, 492)
        Me.BsEncoderAdjustmentTitle.Name = "BsEncoderAdjustmentTitle"
        Me.BsEncoderAdjustmentTitle.Size = New System.Drawing.Size(174, 30)
        Me.BsEncoderAdjustmentTitle.TabIndex = 59
        Me.BsEncoderAdjustmentTitle.Text = "Encoder:"
        Me.BsEncoderAdjustmentTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BsEncoderAdjustmentLabel
        '
        Me.BsEncoderAdjustmentLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsEncoderAdjustmentLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsEncoderAdjustmentLabel.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.BsEncoderAdjustmentLabel.ForeColor = System.Drawing.Color.Black
        Me.BsEncoderAdjustmentLabel.Location = New System.Drawing.Point(382, 492)
        Me.BsEncoderAdjustmentLabel.Name = "BsEncoderAdjustmentLabel"
        Me.BsEncoderAdjustmentLabel.Size = New System.Drawing.Size(120, 34)
        Me.BsEncoderAdjustmentLabel.TabIndex = 46
        Me.BsEncoderAdjustmentLabel.Text = "0"
        Me.BsEncoderAdjustmentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BsOpticAdjustmentTitle
        '
        Me.BsOpticAdjustmentTitle.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsOpticAdjustmentTitle.Location = New System.Drawing.Point(170, 453)
        Me.BsOpticAdjustmentTitle.Name = "BsOpticAdjustmentTitle"
        Me.BsOpticAdjustmentTitle.Size = New System.Drawing.Size(206, 30)
        Me.BsOpticAdjustmentTitle.TabIndex = 38
        Me.BsOpticAdjustmentTitle.Text = "Reactions Rotor Position Saved Adjustment:"
        Me.BsOpticAdjustmentTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BsOpticAdjustmentLabel
        '
        Me.BsOpticAdjustmentLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsOpticAdjustmentLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsOpticAdjustmentLabel.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.BsOpticAdjustmentLabel.ForeColor = System.Drawing.Color.Black
        Me.BsOpticAdjustmentLabel.Location = New System.Drawing.Point(382, 453)
        Me.BsOpticAdjustmentLabel.Name = "BsOpticAdjustmentLabel"
        Me.BsOpticAdjustmentLabel.Size = New System.Drawing.Size(120, 34)
        Me.BsOpticAdjustmentLabel.TabIndex = 37
        Me.BsOpticAdjustmentLabel.Text = "0"
        Me.BsOpticAdjustmentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TabWashingStation
        '
        Me.TabWashingStation.Controls.Add(Me.BsWashingInfoPanel)
        Me.TabWashingStation.Controls.Add(Me.BsWashingAdjustPanel)
        Me.TabWashingStation.Location = New System.Drawing.Point(4, 22)
        Me.TabWashingStation.Name = "TabWashingStation"
        Me.TabWashingStation.Padding = New System.Windows.Forms.Padding(3)
        Me.TabWashingStation.Size = New System.Drawing.Size(970, 532)
        Me.TabWashingStation.TabIndex = 1
        Me.TabWashingStation.Text = "Washing Station"
        Me.TabWashingStation.UseVisualStyleBackColor = True
        '
        'BsWashingInfoPanel
        '
        Me.BsWashingInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsWashingInfoPanel.Controls.Add(Me.BsInfoWsXPSViewer)
        Me.BsWashingInfoPanel.Controls.Add(Me.BsWashingInfoTitle)
        Me.BsWashingInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsWashingInfoPanel.Name = "BsWashingInfoPanel"
        Me.BsWashingInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsWashingInfoPanel.TabIndex = 23
        '
        'BsInfoWsXPSViewer
        '
        Me.BsInfoWsXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoWsXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoWsXPSViewer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsInfoWsXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoWsXPSViewer.CopyButtonVisible = True
        Me.BsInfoWsXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoWsXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoWsXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoWsXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoWsXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoWsXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoWsXPSViewer.HorizontalPageMargin = 10
        Me.BsInfoWsXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoWsXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoWsXPSViewer.IsScrollable = False
        Me.BsInfoWsXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoWsXPSViewer.MenuBarVisible = False
        Me.BsInfoWsXPSViewer.Name = "BsInfoWsXPSViewer"
        Me.BsInfoWsXPSViewer.PopupMenuEnabled = True
        Me.BsInfoWsXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoWsXPSViewer.PrintButtonVisible = True
        Me.BsInfoWsXPSViewer.SearchBarVisible = False
        Me.BsInfoWsXPSViewer.Size = New System.Drawing.Size(230, 509)
        Me.BsInfoWsXPSViewer.TabIndex = 35
        Me.BsInfoWsXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoWsXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoWsXPSViewer.VerticalPageMargin = 10
        Me.BsInfoWsXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoWsXPSViewer.WholePageButtonVisible = True
        '
        'BsWashingInfoTitle
        '
        Me.BsWashingInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsWashingInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsWashingInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsWashingInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsWashingInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsWashingInfoTitle.Name = "BsWashingInfoTitle"
        Me.BsWashingInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsWashingInfoTitle.TabIndex = 29
        Me.BsWashingInfoTitle.Text = "Information"
        Me.BsWashingInfoTitle.Title = True
        '
        'BsWashingAdjustPanel
        '
        Me.BsWashingAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsWashingAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsWashingAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsWashingAdjustPanel.Controls.Add(Me.BsWashingAdjustTitle)
        Me.BsWashingAdjustPanel.Controls.Add(Me.BSWSWSGroupBox)
        Me.BsWashingAdjustPanel.Controls.Add(Me.BsWSAdjustGroupBox)
        Me.BsWashingAdjustPanel.Location = New System.Drawing.Point(231, 0)
        Me.BsWashingAdjustPanel.Name = "BsWashingAdjustPanel"
        Me.BsWashingAdjustPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsWashingAdjustPanel.TabIndex = 22
        '
        'BsWashingAdjustTitle
        '
        Me.BsWashingAdjustTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsWashingAdjustTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsWashingAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.BsWashingAdjustTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsWashingAdjustTitle.Name = "BsWashingAdjustTitle"
        Me.BsWashingAdjustTitle.Size = New System.Drawing.Size(739, 20)
        Me.BsWashingAdjustTitle.TabIndex = 31
        Me.BsWashingAdjustTitle.Text = "Washind Station Positioning Adjustment"
        Me.BsWashingAdjustTitle.Title = True
        '
        'BSWSWSGroupBox
        '
        Me.BSWSWSGroupBox.Controls.Add(Me.BsWSWSLabel)
        Me.BSWSWSGroupBox.Controls.Add(Me.BsUpDownWSButton2)
        Me.BSWSWSGroupBox.Controls.Add(Me.BsButton3)
        Me.BSWSWSGroupBox.Location = New System.Drawing.Point(7, 17)
        Me.BSWSWSGroupBox.Name = "BSWSWSGroupBox"
        Me.BSWSWSGroupBox.Size = New System.Drawing.Size(725, 45)
        Me.BSWSWSGroupBox.TabIndex = 74
        Me.BSWSWSGroupBox.TabStop = False
        '
        'BsWSWSLabel
        '
        Me.BsWSWSLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsWSWSLabel.Location = New System.Drawing.Point(6, 18)
        Me.BsWSWSLabel.Name = "BsWSWSLabel"
        Me.BsWSWSLabel.Size = New System.Drawing.Size(679, 19)
        Me.BsWSWSLabel.TabIndex = 108
        Me.BsWSWSLabel.Text = "Place the Reactions Rotor"
        Me.BsWSWSLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BsUpDownWSButton2
        '
        Me.BsUpDownWSButton2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsUpDownWSButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsUpDownWSButton2.Location = New System.Drawing.Point(690, 10)
        Me.BsUpDownWSButton2.Name = "BsUpDownWSButton2"
        Me.BsUpDownWSButton2.Size = New System.Drawing.Size(32, 32)
        Me.BsUpDownWSButton2.TabIndex = 41
        Me.BsUpDownWSButton2.UseVisualStyleBackColor = True
        '
        'BsButton3
        '
        Me.BsButton3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton3.Location = New System.Drawing.Point(690, 228)
        Me.BsButton3.Name = "BsButton3"
        Me.BsButton3.Size = New System.Drawing.Size(30, 30)
        Me.BsButton3.TabIndex = 106
        Me.BsButton3.UseVisualStyleBackColor = True
        '
        'BsWSAdjustGroupBox
        '
        Me.BsWSAdjustGroupBox.Controls.Add(Me.BsWSAdjustButton)
        Me.BsWSAdjustGroupBox.Controls.Add(Me.BsWSCancelButton)
        Me.BsWSAdjustGroupBox.Controls.Add(Me.BsAdjustWashing)
        Me.BsWSAdjustGroupBox.Controls.Add(Me.BsWashingAdjustmentTitle)
        Me.BsWSAdjustGroupBox.Controls.Add(Me.BsWashingAdjustmentLabel)
        Me.BsWSAdjustGroupBox.Location = New System.Drawing.Point(6, 62)
        Me.BsWSAdjustGroupBox.Name = "BsWSAdjustGroupBox"
        Me.BsWSAdjustGroupBox.Size = New System.Drawing.Size(727, 197)
        Me.BsWSAdjustGroupBox.TabIndex = 75
        Me.BsWSAdjustGroupBox.TabStop = False
        '
        'BsWSAdjustButton
        '
        Me.BsWSAdjustButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsWSAdjustButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsWSAdjustButton.Enabled = False
        Me.BsWSAdjustButton.Location = New System.Drawing.Point(666, 47)
        Me.BsWSAdjustButton.Name = "BsWSAdjustButton"
        Me.BsWSAdjustButton.Size = New System.Drawing.Size(32, 32)
        Me.BsWSAdjustButton.TabIndex = 41
        Me.BsWSAdjustButton.UseVisualStyleBackColor = True
        '
        'BsWSCancelButton
        '
        Me.BsWSCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsWSCancelButton.Enabled = False
        Me.BsWSCancelButton.Location = New System.Drawing.Point(666, 83)
        Me.BsWSCancelButton.Name = "BsWSCancelButton"
        Me.BsWSCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.BsWSCancelButton.TabIndex = 42
        Me.BsWSCancelButton.UseVisualStyleBackColor = True
        '
        'BsAdjustWashing
        '
        Me.BsAdjustWashing.AdjustButtonMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.AdjustButtonModes.UpDown
        Me.BsAdjustWashing.AdjustingEnabled = True
        Me.BsAdjustWashing.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustWashing.CurrentStepValue = 1
        Me.BsAdjustWashing.CurrentValue = 0.0!
        Me.BsAdjustWashing.DisplayBackColor = System.Drawing.Color.Black
        Me.BsAdjustWashing.DisplayEditingForeColor = System.Drawing.Color.White
        Me.BsAdjustWashing.DisplayForeColor = System.Drawing.Color.LightGreen
        Me.BsAdjustWashing.EditingEnabled = True
        Me.BsAdjustWashing.EditionMode = False
        Me.BsAdjustWashing.Enabled = False
        Me.BsAdjustWashing.FocusedBackColor = System.Drawing.Color.Blue
        Me.BsAdjustWashing.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAdjustWashing.HomingEnabled = True
        Me.BsAdjustWashing.IncreaseMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.IncreaseModes.Direct
        Me.BsAdjustWashing.InfoBackColor = System.Drawing.Color.LightSteelBlue
        Me.BsAdjustWashing.InfoTitlesForeColor = System.Drawing.Color.Black
        Me.BsAdjustWashing.InfoValuesForeColor = System.Drawing.Color.Black
        Me.BsAdjustWashing.IsFocused = False
        Me.BsAdjustWashing.LastValueSavedTitle = "Last:"
        Me.BsAdjustWashing.Location = New System.Drawing.Point(507, 47)
        Me.BsAdjustWashing.MaximumLimit = 0.999!
        Me.BsAdjustWashing.MaximumSize = New System.Drawing.Size(153, 127)
        Me.BsAdjustWashing.MaxNumDecimals = 3
        Me.BsAdjustWashing.MinimumLimit = -0.999!
        Me.BsAdjustWashing.MinimumSize = New System.Drawing.Size(153, 87)
        Me.BsAdjustWashing.Name = "BsAdjustWashing"
        Me.BsAdjustWashing.RangeTitle = "Range:"
        Me.BsAdjustWashing.SimulationMode = False
        Me.BsAdjustWashing.Size = New System.Drawing.Size(153, 106)
        Me.BsAdjustWashing.StepValues = CType(resources.GetObject("BsAdjustWashing.StepValues"), System.Collections.Generic.List(Of Integer))
        Me.BsAdjustWashing.TabIndex = 3
        Me.BsAdjustWashing.UnFocusedBackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustWashing.UnitsCaption = "steps"
        '
        'BsWashingAdjustmentTitle
        '
        Me.BsWashingAdjustmentTitle.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsWashingAdjustmentTitle.Location = New System.Drawing.Point(25, 47)
        Me.BsWashingAdjustmentTitle.Name = "BsWashingAdjustmentTitle"
        Me.BsWashingAdjustmentTitle.Size = New System.Drawing.Size(318, 44)
        Me.BsWashingAdjustmentTitle.TabIndex = 40
        Me.BsWashingAdjustmentTitle.Text = "Washing Station Vertical Position Saved Adjustment:"
        Me.BsWashingAdjustmentTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BsWashingAdjustmentLabel
        '
        Me.BsWashingAdjustmentLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsWashingAdjustmentLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsWashingAdjustmentLabel.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.BsWashingAdjustmentLabel.Location = New System.Drawing.Point(348, 52)
        Me.BsWashingAdjustmentLabel.Name = "BsWashingAdjustmentLabel"
        Me.BsWashingAdjustmentLabel.Size = New System.Drawing.Size(120, 34)
        Me.BsWashingAdjustmentLabel.TabIndex = 39
        Me.BsWashingAdjustmentLabel.Text = "0"
        Me.BsWashingAdjustmentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TabArmsPositions
        '
        Me.TabArmsPositions.Controls.Add(Me.BsArmsInfoPanel)
        Me.TabArmsPositions.Controls.Add(Me.BsArmsAdjustPanel)
        Me.TabArmsPositions.Location = New System.Drawing.Point(4, 22)
        Me.TabArmsPositions.Name = "TabArmsPositions"
        Me.TabArmsPositions.Size = New System.Drawing.Size(970, 532)
        Me.TabArmsPositions.TabIndex = 2
        Me.TabArmsPositions.Text = "Arms Positions"
        Me.TabArmsPositions.UseVisualStyleBackColor = True
        '
        'BsArmsInfoPanel
        '
        Me.BsArmsInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsArmsInfoPanel.Controls.Add(Me.BsInfoArmsXPSViewer)
        Me.BsArmsInfoPanel.Controls.Add(Me.BsArmsInfoTitle)
        Me.BsArmsInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsArmsInfoPanel.Name = "BsArmsInfoPanel"
        Me.BsArmsInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsArmsInfoPanel.TabIndex = 26
        '
        'BsInfoArmsXPSViewer
        '
        Me.BsInfoArmsXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoArmsXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoArmsXPSViewer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsInfoArmsXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoArmsXPSViewer.CopyButtonVisible = True
        Me.BsInfoArmsXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoArmsXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoArmsXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoArmsXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoArmsXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoArmsXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoArmsXPSViewer.HorizontalPageMargin = 10
        Me.BsInfoArmsXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoArmsXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoArmsXPSViewer.IsScrollable = False
        Me.BsInfoArmsXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoArmsXPSViewer.MenuBarVisible = False
        Me.BsInfoArmsXPSViewer.Name = "BsInfoArmsXPSViewer"
        Me.BsInfoArmsXPSViewer.PopupMenuEnabled = True
        Me.BsInfoArmsXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoArmsXPSViewer.PrintButtonVisible = True
        Me.BsInfoArmsXPSViewer.SearchBarVisible = False
        Me.BsInfoArmsXPSViewer.Size = New System.Drawing.Size(230, 509)
        Me.BsInfoArmsXPSViewer.TabIndex = 36
        Me.BsInfoArmsXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoArmsXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoArmsXPSViewer.VerticalPageMargin = 10
        Me.BsInfoArmsXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoArmsXPSViewer.WholePageButtonVisible = True
        '
        'BsArmsInfoTitle
        '
        Me.BsArmsInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsArmsInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsArmsInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsArmsInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsArmsInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsArmsInfoTitle.Name = "BsArmsInfoTitle"
        Me.BsArmsInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsArmsInfoTitle.TabIndex = 25
        Me.BsArmsInfoTitle.Text = "Information"
        Me.BsArmsInfoTitle.Title = True
        '
        'BsArmsAdjustPanel
        '
        Me.BsArmsAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsArmsAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsArmsAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsArmsAdjustPanel.Controls.Add(Me.BsArmsAdjustTitle)
        Me.BsArmsAdjustPanel.Controls.Add(Me.BsArmsOkButton)
        Me.BsArmsAdjustPanel.Controls.Add(Me.BsArmsCancelButton)
        Me.BsArmsAdjustPanel.Controls.Add(Me.BsArmsWSGroupBox)
        Me.BsArmsAdjustPanel.Controls.Add(Me.BsAdjustRotor)
        Me.BsArmsAdjustPanel.Controls.Add(Me.BsAdjustZ)
        Me.BsArmsAdjustPanel.Controls.Add(Me.BsAdjustPolar)
        Me.BsArmsAdjustPanel.Controls.Add(Me.BsTabArmsControl)
        Me.BsArmsAdjustPanel.Location = New System.Drawing.Point(231, 0)
        Me.BsArmsAdjustPanel.Name = "BsArmsAdjustPanel"
        Me.BsArmsAdjustPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsArmsAdjustPanel.TabIndex = 25
        '
        'BsArmsAdjustTitle
        '
        Me.BsArmsAdjustTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsArmsAdjustTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsArmsAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.BsArmsAdjustTitle.Location = New System.Drawing.Point(0, -1)
        Me.BsArmsAdjustTitle.Name = "BsArmsAdjustTitle"
        Me.BsArmsAdjustTitle.Size = New System.Drawing.Size(739, 20)
        Me.BsArmsAdjustTitle.TabIndex = 77
        Me.BsArmsAdjustTitle.Text = "Arms Positions Adjustment"
        Me.BsArmsAdjustTitle.Title = True
        '
        'BsArmsOkButton
        '
        Me.BsArmsOkButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsArmsOkButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsArmsOkButton.Enabled = False
        Me.BsArmsOkButton.Location = New System.Drawing.Point(614, 452)
        Me.BsArmsOkButton.Name = "BsArmsOkButton"
        Me.BsArmsOkButton.Size = New System.Drawing.Size(32, 32)
        Me.BsArmsOkButton.TabIndex = 76
        Me.BsArmsOkButton.UseVisualStyleBackColor = True
        '
        'BsArmsCancelButton
        '
        Me.BsArmsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsArmsCancelButton.Enabled = False
        Me.BsArmsCancelButton.Location = New System.Drawing.Point(616, 416)
        Me.BsArmsCancelButton.Name = "BsArmsCancelButton"
        Me.BsArmsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.BsArmsCancelButton.TabIndex = 75
        Me.BsArmsCancelButton.UseVisualStyleBackColor = True
        '
        'BsArmsWSGroupBox
        '
        Me.BsArmsWSGroupBox.Controls.Add(Me.BsArmsWSLabel)
        Me.BsArmsWSGroupBox.Controls.Add(Me.BsUpDownWSButton3)
        Me.BsArmsWSGroupBox.Controls.Add(Me.BsButton6)
        Me.BsArmsWSGroupBox.Location = New System.Drawing.Point(7, 17)
        Me.BsArmsWSGroupBox.Name = "BsArmsWSGroupBox"
        Me.BsArmsWSGroupBox.Size = New System.Drawing.Size(725, 45)
        Me.BsArmsWSGroupBox.TabIndex = 73
        Me.BsArmsWSGroupBox.TabStop = False
        '
        'BsArmsWSLabel
        '
        Me.BsArmsWSLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsArmsWSLabel.Location = New System.Drawing.Point(6, 18)
        Me.BsArmsWSLabel.Name = "BsArmsWSLabel"
        Me.BsArmsWSLabel.Size = New System.Drawing.Size(679, 19)
        Me.BsArmsWSLabel.TabIndex = 108
        Me.BsArmsWSLabel.Text = "Place the Reactions Rotor"
        Me.BsArmsWSLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BsUpDownWSButton3
        '
        Me.BsUpDownWSButton3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsUpDownWSButton3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsUpDownWSButton3.Location = New System.Drawing.Point(690, 10)
        Me.BsUpDownWSButton3.Name = "BsUpDownWSButton3"
        Me.BsUpDownWSButton3.Size = New System.Drawing.Size(32, 32)
        Me.BsUpDownWSButton3.TabIndex = 5
        Me.BsUpDownWSButton3.UseVisualStyleBackColor = True
        '
        'BsButton6
        '
        Me.BsButton6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton6.Location = New System.Drawing.Point(690, 228)
        Me.BsButton6.Name = "BsButton6"
        Me.BsButton6.Size = New System.Drawing.Size(30, 30)
        Me.BsButton6.TabIndex = 106
        Me.BsButton6.UseVisualStyleBackColor = True
        '
        'BsAdjustRotor
        '
        Me.BsAdjustRotor.AdjustButtonMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.AdjustButtonModes.LeftRight
        Me.BsAdjustRotor.AdjustingEnabled = True
        Me.BsAdjustRotor.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustRotor.CurrentStepValue = 1
        Me.BsAdjustRotor.CurrentValue = 0.0!
        Me.BsAdjustRotor.DisplayBackColor = System.Drawing.Color.Black
        Me.BsAdjustRotor.DisplayEditingForeColor = System.Drawing.Color.White
        Me.BsAdjustRotor.DisplayForeColor = System.Drawing.Color.LightGreen
        Me.BsAdjustRotor.EditingEnabled = True
        Me.BsAdjustRotor.EditionMode = False
        Me.BsAdjustRotor.Enabled = False
        Me.BsAdjustRotor.FocusedBackColor = System.Drawing.Color.Blue
        Me.BsAdjustRotor.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAdjustRotor.HomingEnabled = True
        Me.BsAdjustRotor.IncreaseMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.IncreaseModes.Direct
        Me.BsAdjustRotor.InfoBackColor = System.Drawing.Color.LightSteelBlue
        Me.BsAdjustRotor.InfoTitlesForeColor = System.Drawing.Color.Black
        Me.BsAdjustRotor.InfoValuesForeColor = System.Drawing.Color.Black
        Me.BsAdjustRotor.IsFocused = False
        Me.BsAdjustRotor.LastValueSavedTitle = "Last:"
        Me.BsAdjustRotor.Location = New System.Drawing.Point(456, 416)
        Me.BsAdjustRotor.MaximumLimit = 0.999!
        Me.BsAdjustRotor.MaximumSize = New System.Drawing.Size(153, 127)
        Me.BsAdjustRotor.MaxNumDecimals = 3
        Me.BsAdjustRotor.MinimumLimit = -0.999!
        Me.BsAdjustRotor.MinimumSize = New System.Drawing.Size(153, 87)
        Me.BsAdjustRotor.Name = "BsAdjustRotor"
        Me.BsAdjustRotor.RangeTitle = "Range:"
        Me.BsAdjustRotor.SimulationMode = False
        Me.BsAdjustRotor.Size = New System.Drawing.Size(153, 106)
        Me.BsAdjustRotor.StepValues = CType(resources.GetObject("BsAdjustRotor.StepValues"), System.Collections.Generic.List(Of Integer))
        Me.BsAdjustRotor.TabIndex = 4
        Me.BsAdjustRotor.UnFocusedBackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustRotor.UnitsCaption = "steps"
        '
        'BsAdjustZ
        '
        Me.BsAdjustZ.AdjustButtonMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.AdjustButtonModes.UpDown
        Me.BsAdjustZ.AdjustingEnabled = True
        Me.BsAdjustZ.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustZ.CurrentStepValue = 1
        Me.BsAdjustZ.CurrentValue = 0.0!
        Me.BsAdjustZ.DisplayBackColor = System.Drawing.Color.Black
        Me.BsAdjustZ.DisplayEditingForeColor = System.Drawing.Color.White
        Me.BsAdjustZ.DisplayForeColor = System.Drawing.Color.LightGreen
        Me.BsAdjustZ.EditingEnabled = True
        Me.BsAdjustZ.EditionMode = False
        Me.BsAdjustZ.Enabled = False
        Me.BsAdjustZ.FocusedBackColor = System.Drawing.Color.Blue
        Me.BsAdjustZ.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAdjustZ.HomingEnabled = True
        Me.BsAdjustZ.IncreaseMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.IncreaseModes.Direct
        Me.BsAdjustZ.InfoBackColor = System.Drawing.Color.LightSteelBlue
        Me.BsAdjustZ.InfoTitlesForeColor = System.Drawing.Color.Black
        Me.BsAdjustZ.InfoValuesForeColor = System.Drawing.Color.Black
        Me.BsAdjustZ.IsFocused = False
        Me.BsAdjustZ.LastValueSavedTitle = "Last:"
        Me.BsAdjustZ.Location = New System.Drawing.Point(297, 416)
        Me.BsAdjustZ.MaximumLimit = 0.999!
        Me.BsAdjustZ.MaximumSize = New System.Drawing.Size(153, 127)
        Me.BsAdjustZ.MaxNumDecimals = 3
        Me.BsAdjustZ.MinimumLimit = -0.999!
        Me.BsAdjustZ.MinimumSize = New System.Drawing.Size(153, 87)
        Me.BsAdjustZ.Name = "BsAdjustZ"
        Me.BsAdjustZ.RangeTitle = "Range:"
        Me.BsAdjustZ.SimulationMode = False
        Me.BsAdjustZ.Size = New System.Drawing.Size(153, 106)
        Me.BsAdjustZ.StepValues = CType(resources.GetObject("BsAdjustZ.StepValues"), System.Collections.Generic.List(Of Integer))
        Me.BsAdjustZ.TabIndex = 3
        Me.BsAdjustZ.UnFocusedBackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustZ.UnitsCaption = "steps"
        '
        'BsAdjustPolar
        '
        Me.BsAdjustPolar.AdjustButtonMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.AdjustButtonModes.LeftRight
        Me.BsAdjustPolar.AdjustingEnabled = True
        Me.BsAdjustPolar.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustPolar.CurrentStepValue = 1
        Me.BsAdjustPolar.CurrentValue = 0.0!
        Me.BsAdjustPolar.DisplayBackColor = System.Drawing.Color.Black
        Me.BsAdjustPolar.DisplayEditingForeColor = System.Drawing.Color.White
        Me.BsAdjustPolar.DisplayForeColor = System.Drawing.Color.LightGreen
        Me.BsAdjustPolar.EditingEnabled = True
        Me.BsAdjustPolar.EditionMode = False
        Me.BsAdjustPolar.Enabled = False
        Me.BsAdjustPolar.FocusedBackColor = System.Drawing.Color.Blue
        Me.BsAdjustPolar.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAdjustPolar.HomingEnabled = True
        Me.BsAdjustPolar.IncreaseMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.IncreaseModes.Direct
        Me.BsAdjustPolar.InfoBackColor = System.Drawing.Color.LightSteelBlue
        Me.BsAdjustPolar.InfoTitlesForeColor = System.Drawing.Color.Black
        Me.BsAdjustPolar.InfoValuesForeColor = System.Drawing.Color.Black
        Me.BsAdjustPolar.IsFocused = False
        Me.BsAdjustPolar.LastValueSavedTitle = "Last:"
        Me.BsAdjustPolar.Location = New System.Drawing.Point(137, 416)
        Me.BsAdjustPolar.MaximumLimit = 0.999!
        Me.BsAdjustPolar.MaximumSize = New System.Drawing.Size(153, 127)
        Me.BsAdjustPolar.MaxNumDecimals = 3
        Me.BsAdjustPolar.MinimumLimit = -0.999!
        Me.BsAdjustPolar.MinimumSize = New System.Drawing.Size(153, 87)
        Me.BsAdjustPolar.Name = "BsAdjustPolar"
        Me.BsAdjustPolar.RangeTitle = "Range:"
        Me.BsAdjustPolar.SimulationMode = False
        Me.BsAdjustPolar.Size = New System.Drawing.Size(153, 106)
        Me.BsAdjustPolar.StepValues = CType(resources.GetObject("BsAdjustPolar.StepValues"), System.Collections.Generic.List(Of Integer))
        Me.BsAdjustPolar.TabIndex = 2
        Me.BsAdjustPolar.UnFocusedBackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustPolar.UnitsCaption = "steps"
        '
        'BsTabArmsControl
        '
        Me.BsTabArmsControl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTabArmsControl.Controls.Add(Me.TabSample)
        Me.BsTabArmsControl.Controls.Add(Me.TabReagent1)
        Me.BsTabArmsControl.Controls.Add(Me.TabReagent2)
        Me.BsTabArmsControl.Controls.Add(Me.TabMixer1)
        Me.BsTabArmsControl.Controls.Add(Me.TabMixer2)
        Me.BsTabArmsControl.Location = New System.Drawing.Point(7, 71)
        Me.BsTabArmsControl.Name = "BsTabArmsControl"
        Me.BsTabArmsControl.SelectedIndex = 0
        Me.BsTabArmsControl.Size = New System.Drawing.Size(723, 332)
        Me.BsTabArmsControl.TabIndex = 0
        '
        'TabSample
        '
        Me.TabSample.BackColor = System.Drawing.Color.Gainsboro
        Me.TabSample.Controls.Add(Me.BsGridSample)
        Me.TabSample.Location = New System.Drawing.Point(4, 22)
        Me.TabSample.Name = "TabSample"
        Me.TabSample.Padding = New System.Windows.Forms.Padding(3)
        Me.TabSample.Size = New System.Drawing.Size(715, 306)
        Me.TabSample.TabIndex = 0
        Me.TabSample.Text = "SAMPLE"
        '
        'BsGridSample
        '
        Me.BsGridSample.AdjustButtonImage = Nothing
        Me.BsGridSample.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsGridSample.BackColor = System.Drawing.Color.Gainsboro
        Me.BsGridSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsGridSample.Location = New System.Drawing.Point(3, 3)
        Me.BsGridSample.Name = "BsGridSample"
        Me.BsGridSample.nameColButton1 = Nothing
        Me.BsGridSample.nameColButton2 = Nothing
        Me.BsGridSample.nameColImage = Nothing
        Me.BsGridSample.nameIdentificator = Nothing
        Me.BsGridSample.NoValidationImage = ""
        Me.BsGridSample.numParams = 0
        Me.BsGridSample.OkImage = Nothing
        Me.BsGridSample.SelectedRow = 0
        Me.BsGridSample.SelectedValue = Nothing
        Me.BsGridSample.Size = New System.Drawing.Size(706, 297)
        Me.BsGridSample.TabIndex = 0
        Me.BsGridSample.TestButtonImage = Nothing
        Me.BsGridSample.ValidationImage = ""
        '
        'TabReagent1
        '
        Me.TabReagent1.BackColor = System.Drawing.Color.Gainsboro
        Me.TabReagent1.Controls.Add(Me.BsGridReagent1)
        Me.TabReagent1.Location = New System.Drawing.Point(4, 22)
        Me.TabReagent1.Name = "TabReagent1"
        Me.TabReagent1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabReagent1.Size = New System.Drawing.Size(717, 307)
        Me.TabReagent1.TabIndex = 1
        Me.TabReagent1.Text = "REAGENT1"
        '
        'BsGridReagent1
        '
        Me.BsGridReagent1.AdjustButtonImage = Nothing
        Me.BsGridReagent1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsGridReagent1.BackColor = System.Drawing.Color.Gainsboro
        Me.BsGridReagent1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsGridReagent1.Location = New System.Drawing.Point(4, 5)
        Me.BsGridReagent1.Name = "BsGridReagent1"
        Me.BsGridReagent1.nameColButton1 = Nothing
        Me.BsGridReagent1.nameColButton2 = Nothing
        Me.BsGridReagent1.nameColImage = Nothing
        Me.BsGridReagent1.nameIdentificator = Nothing
        Me.BsGridReagent1.NoValidationImage = ""
        Me.BsGridReagent1.numParams = 0
        Me.BsGridReagent1.OkImage = Nothing
        Me.BsGridReagent1.SelectedRow = 0
        Me.BsGridReagent1.SelectedValue = Nothing
        Me.BsGridReagent1.Size = New System.Drawing.Size(708, 259)
        Me.BsGridReagent1.TabIndex = 4
        Me.BsGridReagent1.TestButtonImage = Nothing
        Me.BsGridReagent1.ValidationImage = ""
        '
        'TabReagent2
        '
        Me.TabReagent2.BackColor = System.Drawing.Color.Gainsboro
        Me.TabReagent2.Controls.Add(Me.BsGridReagent2)
        Me.TabReagent2.Location = New System.Drawing.Point(4, 22)
        Me.TabReagent2.Name = "TabReagent2"
        Me.TabReagent2.Size = New System.Drawing.Size(717, 307)
        Me.TabReagent2.TabIndex = 2
        Me.TabReagent2.Text = "REAGENT2"
        '
        'BsGridReagent2
        '
        Me.BsGridReagent2.AdjustButtonImage = Nothing
        Me.BsGridReagent2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsGridReagent2.BackColor = System.Drawing.Color.Gainsboro
        Me.BsGridReagent2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsGridReagent2.Location = New System.Drawing.Point(4, 5)
        Me.BsGridReagent2.Name = "BsGridReagent2"
        Me.BsGridReagent2.nameColButton1 = Nothing
        Me.BsGridReagent2.nameColButton2 = Nothing
        Me.BsGridReagent2.nameColImage = Nothing
        Me.BsGridReagent2.nameIdentificator = Nothing
        Me.BsGridReagent2.NoValidationImage = ""
        Me.BsGridReagent2.numParams = 0
        Me.BsGridReagent2.OkImage = Nothing
        Me.BsGridReagent2.SelectedRow = 0
        Me.BsGridReagent2.SelectedValue = Nothing
        Me.BsGridReagent2.Size = New System.Drawing.Size(708, 259)
        Me.BsGridReagent2.TabIndex = 4
        Me.BsGridReagent2.TestButtonImage = Nothing
        Me.BsGridReagent2.ValidationImage = ""
        '
        'TabMixer1
        '
        Me.TabMixer1.BackColor = System.Drawing.Color.Gainsboro
        Me.TabMixer1.Controls.Add(Me.BsStirrer1Button)
        Me.TabMixer1.Controls.Add(Me.BsGridMixer1)
        Me.TabMixer1.Location = New System.Drawing.Point(4, 22)
        Me.TabMixer1.Name = "TabMixer1"
        Me.TabMixer1.Size = New System.Drawing.Size(717, 307)
        Me.TabMixer1.TabIndex = 3
        Me.TabMixer1.Text = "MIXER1"
        '
        'BsStirrer1Button
        '
        Me.BsStirrer1Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsStirrer1Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsStirrer1Button.Location = New System.Drawing.Point(682, 270)
        Me.BsStirrer1Button.Name = "BsStirrer1Button"
        Me.BsStirrer1Button.Size = New System.Drawing.Size(32, 32)
        Me.BsStirrer1Button.TabIndex = 5
        Me.BsStirrer1Button.UseVisualStyleBackColor = True
        Me.BsStirrer1Button.Visible = False
        '
        'BsGridMixer1
        '
        Me.BsGridMixer1.AdjustButtonImage = Nothing
        Me.BsGridMixer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsGridMixer1.BackColor = System.Drawing.Color.Gainsboro
        Me.BsGridMixer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsGridMixer1.Location = New System.Drawing.Point(4, 5)
        Me.BsGridMixer1.Name = "BsGridMixer1"
        Me.BsGridMixer1.nameColButton1 = Nothing
        Me.BsGridMixer1.nameColButton2 = Nothing
        Me.BsGridMixer1.nameColImage = Nothing
        Me.BsGridMixer1.nameIdentificator = Nothing
        Me.BsGridMixer1.NoValidationImage = ""
        Me.BsGridMixer1.numParams = 0
        Me.BsGridMixer1.OkImage = Nothing
        Me.BsGridMixer1.SelectedRow = 0
        Me.BsGridMixer1.SelectedValue = Nothing
        Me.BsGridMixer1.Size = New System.Drawing.Size(708, 259)
        Me.BsGridMixer1.TabIndex = 4
        Me.BsGridMixer1.TestButtonImage = Nothing
        Me.BsGridMixer1.ValidationImage = ""
        '
        'TabMixer2
        '
        Me.TabMixer2.BackColor = System.Drawing.Color.Gainsboro
        Me.TabMixer2.Controls.Add(Me.BsStirrer2Button)
        Me.TabMixer2.Controls.Add(Me.BsGridMixer2)
        Me.TabMixer2.Location = New System.Drawing.Point(4, 22)
        Me.TabMixer2.Name = "TabMixer2"
        Me.TabMixer2.Size = New System.Drawing.Size(717, 307)
        Me.TabMixer2.TabIndex = 4
        Me.TabMixer2.Text = "MIXER2"
        '
        'BsStirrer2Button
        '
        Me.BsStirrer2Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsStirrer2Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsStirrer2Button.Location = New System.Drawing.Point(682, 270)
        Me.BsStirrer2Button.Name = "BsStirrer2Button"
        Me.BsStirrer2Button.Size = New System.Drawing.Size(32, 32)
        Me.BsStirrer2Button.TabIndex = 6
        Me.BsStirrer2Button.UseVisualStyleBackColor = True
        Me.BsStirrer2Button.Visible = False
        '
        'BsGridMixer2
        '
        Me.BsGridMixer2.AdjustButtonImage = Nothing
        Me.BsGridMixer2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsGridMixer2.BackColor = System.Drawing.Color.Gainsboro
        Me.BsGridMixer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsGridMixer2.Location = New System.Drawing.Point(4, 5)
        Me.BsGridMixer2.Name = "BsGridMixer2"
        Me.BsGridMixer2.nameColButton1 = Nothing
        Me.BsGridMixer2.nameColButton2 = Nothing
        Me.BsGridMixer2.nameColImage = Nothing
        Me.BsGridMixer2.nameIdentificator = Nothing
        Me.BsGridMixer2.NoValidationImage = ""
        Me.BsGridMixer2.numParams = 0
        Me.BsGridMixer2.OkImage = Nothing
        Me.BsGridMixer2.SelectedRow = 0
        Me.BsGridMixer2.SelectedValue = Nothing
        Me.BsGridMixer2.Size = New System.Drawing.Size(708, 259)
        Me.BsGridMixer2.TabIndex = 4
        Me.BsGridMixer2.TestButtonImage = Nothing
        Me.BsGridMixer2.ValidationImage = ""
        '
        'TabPageTODELETE
        '
        Me.TabPageTODELETE.Controls.Add(Me.BsPanel1)
        Me.TabPageTODELETE.Location = New System.Drawing.Point(4, 22)
        Me.TabPageTODELETE.Name = "TabPageTODELETE"
        Me.TabPageTODELETE.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageTODELETE.Size = New System.Drawing.Size(970, 532)
        Me.TabPageTODELETE.TabIndex = 3
        Me.TabPageTODELETE.Text = "TabPageTODELETE"
        Me.TabPageTODELETE.UseVisualStyleBackColor = True
        '
        'BsPanel1
        '
        Me.BsPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsPanel1.BackColor = System.Drawing.Color.Gainsboro
        Me.BsPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsPanel1.Controls.Add(Me.BsButton1)
        Me.BsPanel1.Controls.Add(Me.Label1)
        Me.BsPanel1.Controls.Add(Me.Label2)
        Me.BsPanel1.Controls.Add(Me.Label3)
        Me.BsPanel1.Controls.Add(Me.TrackBar1)
        Me.BsPanel1.Controls.Add(Me.Label4)
        Me.BsPanel1.Controls.Add(Me.Label5)
        Me.BsPanel1.Controls.Add(Me.Label6)
        Me.BsPanel1.Controls.Add(Me.BsPanel2)
        Me.BsPanel1.Controls.Add(Me.BsLabel1)
        Me.BsPanel1.Controls.Add(Me.BsAdjustControl1)
        Me.BsPanel1.Location = New System.Drawing.Point(229, 0)
        Me.BsPanel1.Name = "BsPanel1"
        Me.BsPanel1.Size = New System.Drawing.Size(741, 533)
        Me.BsPanel1.TabIndex = 22
        '
        'BsButton1
        '
        Me.BsButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton1.Location = New System.Drawing.Point(702, 495)
        Me.BsButton1.Name = "BsButton1"
        Me.BsButton1.Size = New System.Drawing.Size(32, 32)
        Me.BsButton1.TabIndex = 45
        Me.BsButton1.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.Font = New System.Drawing.Font("Verdana", 12.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.DimGray
        Me.Label1.Location = New System.Drawing.Point(165, 400)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(124, 20)
        Me.Label1.TabIndex = 44
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Label2.Font = New System.Drawing.Font("Verdana", 12.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.DimGray
        Me.Label2.Location = New System.Drawing.Point(288, 419)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(20, 20)
        Me.Label2.TabIndex = 43
        Me.Label2.Text = "+"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label3
        '
        Me.Label3.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Label3.Font = New System.Drawing.Font("Verdana", 12.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.DimGray
        Me.Label3.Location = New System.Drawing.Point(151, 420)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(20, 20)
        Me.Label3.TabIndex = 42
        Me.Label3.Text = "-"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TrackBar1
        '
        Me.TrackBar1.AutoSize = False
        Me.TrackBar1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.TrackBar1.Enabled = False
        Me.TrackBar1.LargeChange = 1000
        Me.TrackBar1.Location = New System.Drawing.Point(169, 408)
        Me.TrackBar1.Margin = New System.Windows.Forms.Padding(0)
        Me.TrackBar1.Maximum = 30000
        Me.TrackBar1.Name = "TrackBar1"
        Me.TrackBar1.Size = New System.Drawing.Size(120, 39)
        Me.TrackBar1.TabIndex = 39
        Me.TrackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        Me.TrackBar1.Value = 8000
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(39, 442)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(259, 44)
        Me.Label4.TabIndex = 38
        Me.Label4.Text = "Reactions Rotor Position Saved Adjustment:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(39, 405)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(100, 44)
        Me.Label5.TabIndex = 40
        Me.Label5.Text = "LED Current:"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label6
        '
        Me.Label6.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label6.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.Location = New System.Drawing.Point(328, 447)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(120, 34)
        Me.Label6.TabIndex = 37
        Me.Label6.Text = "0"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BsPanel2
        '
        Me.BsPanel2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsPanel2.Controls.Add(Me.ChartControl1)
        Me.BsPanel2.Location = New System.Drawing.Point(7, 33)
        Me.BsPanel2.Name = "BsPanel2"
        Me.BsPanel2.Size = New System.Drawing.Size(725, 360)
        Me.BsPanel2.TabIndex = 34
        '
        'ChartControl1
        '
        Me.ChartControl1.AppearanceName = "Dark Flat"
        Me.ChartControl1.BackColor = System.Drawing.Color.Gainsboro
        Me.ChartControl1.BorderOptions.Color = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        SwiftPlotDiagram1.AxisX.Color = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        ConstantLine18.AxisValueSerializable = "0"
        ConstantLine18.Color = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        ConstantLine18.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.Solid
        ConstantLine18.LineStyle.Thickness = 8
        ConstantLine18.Name = "OldWall1"
        ConstantLine18.ShowBehind = True
        ConstantLine18.ShowInLegend = False
        ConstantLine18.Title.Visible = False
        ConstantLine19.AxisValueSerializable = "80"
        ConstantLine19.Color = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        ConstantLine19.LineStyle.Thickness = 4
        ConstantLine19.Name = "OldWall2"
        ConstantLine19.ShowBehind = True
        ConstantLine19.ShowInLegend = False
        ConstantLine19.Title.Visible = False
        ConstantLine20.AxisValueSerializable = "160"
        ConstantLine20.Color = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        ConstantLine20.LineStyle.Thickness = 4
        ConstantLine20.Name = "OldWall3"
        ConstantLine20.ShowBehind = True
        ConstantLine20.ShowInLegend = False
        ConstantLine20.Title.Visible = False
        ConstantLine21.AxisValueSerializable = "240"
        ConstantLine21.Color = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        ConstantLine21.LineStyle.Thickness = 4
        ConstantLine21.Name = "OldWall4"
        ConstantLine21.ShowBehind = True
        ConstantLine21.ShowInLegend = False
        ConstantLine21.Title.Visible = False
        ConstantLine22.AxisValueSerializable = "320"
        ConstantLine22.Color = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        ConstantLine22.LineStyle.Thickness = 4
        ConstantLine22.Name = "OldWall5"
        ConstantLine22.ShowBehind = True
        ConstantLine22.ShowInLegend = False
        ConstantLine22.Title.Visible = False
        ConstantLine23.AxisValueSerializable = "400"
        ConstantLine23.Color = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer))
        ConstantLine23.LineStyle.Thickness = 8
        ConstantLine23.Name = "OldWall6"
        ConstantLine23.ShowBehind = True
        ConstantLine23.ShowInLegend = False
        ConstantLine24.AxisValueSerializable = "0"
        ConstantLine24.Color = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        ConstantLine24.LineStyle.Thickness = 4
        ConstantLine24.Name = "NewWall1"
        ConstantLine24.ShowBehind = True
        ConstantLine24.ShowInLegend = False
        ConstantLine24.Title.Visible = False
        ConstantLine25.AxisValueSerializable = "80"
        ConstantLine25.Color = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        ConstantLine25.LineStyle.Thickness = 4
        ConstantLine25.Name = "NewWall2"
        ConstantLine25.ShowBehind = True
        ConstantLine25.ShowInLegend = False
        ConstantLine25.Title.Visible = False
        ConstantLine26.AxisValueSerializable = "160"
        ConstantLine26.Color = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        ConstantLine26.LineStyle.Thickness = 4
        ConstantLine26.Name = "NewWall3"
        ConstantLine26.ShowBehind = True
        ConstantLine26.ShowInLegend = False
        ConstantLine26.Title.Visible = False
        ConstantLine27.AxisValueSerializable = "240"
        ConstantLine27.Color = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        ConstantLine27.LineStyle.Thickness = 4
        ConstantLine27.Name = "NewWall4"
        ConstantLine27.ShowBehind = True
        ConstantLine27.ShowInLegend = False
        ConstantLine27.Title.Visible = False
        ConstantLine28.AxisValueSerializable = "320"
        ConstantLine28.Color = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        ConstantLine28.LineStyle.Thickness = 4
        ConstantLine28.Name = "NewWall5"
        ConstantLine28.ShowBehind = True
        ConstantLine28.ShowInLegend = False
        ConstantLine28.Title.Visible = False
        ConstantLine29.AxisValueSerializable = "400"
        ConstantLine29.Color = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        ConstantLine29.LineStyle.DashStyle = DevExpress.XtraCharts.DashStyle.Solid
        ConstantLine29.LineStyle.Thickness = 4
        ConstantLine29.Name = "NewWall6"
        ConstantLine29.ShowBehind = True
        ConstantLine29.ShowInLegend = False
        ConstantLine29.Title.Visible = False
        SwiftPlotDiagram1.AxisX.ConstantLines.AddRange(New DevExpress.XtraCharts.ConstantLine() {ConstantLine18, ConstantLine19, ConstantLine20, ConstantLine21, ConstantLine22, ConstantLine23, ConstantLine24, ConstantLine25, ConstantLine26, ConstantLine27, ConstantLine28, ConstantLine29})
        SwiftPlotDiagram1.AxisX.GridLines.MinorVisible = True
        SwiftPlotDiagram1.AxisX.GridLines.Visible = True
        SwiftPlotDiagram1.AxisX.GridSpacing = 80
        SwiftPlotDiagram1.AxisX.GridSpacingAuto = False
        SwiftPlotDiagram1.AxisX.Label.Antialiasing = True
        SwiftPlotDiagram1.AxisX.Label.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        SwiftPlotDiagram1.AxisX.Label.TextColor = System.Drawing.Color.Black
        SwiftPlotDiagram1.AxisX.Range.Auto = False
        SwiftPlotDiagram1.AxisX.Range.MaxValueSerializable = "400"
        SwiftPlotDiagram1.AxisX.Range.MinValueSerializable = "0"
        SwiftPlotDiagram1.AxisX.Range.ScrollingRange.SideMarginsEnabled = True
        SwiftPlotDiagram1.AxisX.Range.SideMarginsEnabled = True
        SwiftPlotDiagram1.AxisX.Title.Alignment = System.Drawing.StringAlignment.Far
        SwiftPlotDiagram1.AxisX.Title.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        SwiftPlotDiagram1.AxisX.Title.Text = "steps"
        SwiftPlotDiagram1.AxisX.Title.TextColor = System.Drawing.Color.DimGray
        SwiftPlotDiagram1.AxisX.Title.Visible = True
        SwiftPlotDiagram1.AxisX.VisibleInPanesSerializable = "-1"
        SwiftPlotDiagram1.AxisY.Color = System.Drawing.Color.Black
        SwiftPlotDiagram1.AxisY.Label.Antialiasing = True
        SwiftPlotDiagram1.AxisY.Label.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        SwiftPlotDiagram1.AxisY.Label.TextColor = System.Drawing.Color.Black
        SwiftPlotDiagram1.AxisY.NumericOptions.Precision = 0
        SwiftPlotDiagram1.AxisY.Range.Auto = False
        SwiftPlotDiagram1.AxisY.Range.MaxValueSerializable = "1100000"
        SwiftPlotDiagram1.AxisY.Range.MinValueSerializable = "500000"
        SwiftPlotDiagram1.AxisY.Range.ScrollingRange.SideMarginsEnabled = True
        SwiftPlotDiagram1.AxisY.Range.SideMarginsEnabled = False
        SwiftPlotDiagram1.AxisY.Title.Alignment = System.Drawing.StringAlignment.Far
        SwiftPlotDiagram1.AxisY.Title.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        SwiftPlotDiagram1.AxisY.Title.Text = "counts"
        SwiftPlotDiagram1.AxisY.Title.TextColor = System.Drawing.Color.DimGray
        SwiftPlotDiagram1.AxisY.Title.Visible = True
        SwiftPlotDiagram1.AxisY.VisibleInPanesSerializable = "-1"
        SwiftPlotDiagram1.DefaultPane.BorderColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        SwiftPlotDiagram1.ZoomingOptions.UseKeyboard = False
        SwiftPlotDiagram1.ZoomingOptions.UseKeyboardWithMouse = False
        SwiftPlotDiagram1.ZoomingOptions.UseMouseWheel = False
        Me.ChartControl1.Diagram = SwiftPlotDiagram1
        Me.ChartControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ChartControl1.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Right
        Me.ChartControl1.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Bottom
        Me.ChartControl1.Legend.Antialiasing = True
        Me.ChartControl1.Legend.BackColor = System.Drawing.Color.Transparent
        Me.ChartControl1.Legend.BackImage.Stretch = True
        Me.ChartControl1.Legend.Border.Visible = False
        Me.ChartControl1.Legend.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.ChartControl1.Legend.TextColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.ChartControl1.Legend.Visible = False
        Me.ChartControl1.Location = New System.Drawing.Point(0, 0)
        Me.ChartControl1.Name = "ChartControl1"
        Me.ChartControl1.Padding.Bottom = 10
        Me.ChartControl1.Padding.Left = 10
        Me.ChartControl1.Padding.Right = 10
        Me.ChartControl1.Padding.Top = 10
        Me.ChartControl1.PaletteBaseColorNumber = 2
        Me.ChartControl1.PaletteName = "Nature Colors"
        Series3.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Numerical
        Series3.LegendText = "Absorbance"
        Series3.Name = "Absorbance"
        SwiftPlotSeriesView1.Color = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        SwiftPlotSeriesView1.LineStyle.Thickness = 3
        Series3.View = SwiftPlotSeriesView1
        Me.ChartControl1.SeriesSerializable = New DevExpress.XtraCharts.Series() {Series3}
        Me.ChartControl1.SeriesTemplate.View = SwiftPlotSeriesView2
        Me.ChartControl1.Size = New System.Drawing.Size(725, 360)
        Me.ChartControl1.SmallChartText.Text = ""
        Me.ChartControl1.TabIndex = 2
        ChartTitle2.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ChartTitle2.Text = "Absorbance"
        ChartTitle2.TextColor = System.Drawing.Color.Black
        ChartTitle2.Visible = False
        Me.ChartControl1.Titles.AddRange(New DevExpress.XtraCharts.ChartTitle() {ChartTitle2})
        '
        'BsLabel1
        '
        Me.BsLabel1.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsLabel1.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsLabel1.ForeColor = System.Drawing.Color.Black
        Me.BsLabel1.Location = New System.Drawing.Point(0, 0)
        Me.BsLabel1.Name = "BsLabel1"
        Me.BsLabel1.Size = New System.Drawing.Size(739, 20)
        Me.BsLabel1.TabIndex = 31
        Me.BsLabel1.Text = "Optic Centering Adjustment"
        Me.BsLabel1.Title = True
        '
        'BsAdjustControl1
        '
        Me.BsAdjustControl1.AdjustButtonMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.AdjustButtonModes.LeftRight
        Me.BsAdjustControl1.AdjustingEnabled = True
        Me.BsAdjustControl1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustControl1.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustControl1.CurrentStepValue = 1
        Me.BsAdjustControl1.CurrentValue = 0.0!
        Me.BsAdjustControl1.DisplayBackColor = System.Drawing.Color.Black
        Me.BsAdjustControl1.DisplayEditingForeColor = System.Drawing.Color.White
        Me.BsAdjustControl1.DisplayForeColor = System.Drawing.Color.LimeGreen
        Me.BsAdjustControl1.EditingEnabled = True
        Me.BsAdjustControl1.EditionMode = False
        Me.BsAdjustControl1.Enabled = False
        Me.BsAdjustControl1.FocusedBackColor = System.Drawing.Color.Blue
        Me.BsAdjustControl1.Font = New System.Drawing.Font("Verdana", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAdjustControl1.HomingEnabled = True
        Me.BsAdjustControl1.IncreaseMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.IncreaseModes.Direct
        Me.BsAdjustControl1.InfoBackColor = System.Drawing.Color.LightSteelBlue
        Me.BsAdjustControl1.InfoTitlesForeColor = System.Drawing.Color.Black
        Me.BsAdjustControl1.InfoValuesForeColor = System.Drawing.Color.Black
        Me.BsAdjustControl1.IsFocused = False
        Me.BsAdjustControl1.LastValueSavedTitle = "Last:"
        Me.BsAdjustControl1.Location = New System.Drawing.Point(521, 420)
        Me.BsAdjustControl1.MaximumLimit = 50000.0!
        Me.BsAdjustControl1.MaximumSize = New System.Drawing.Size(153, 127)
        Me.BsAdjustControl1.MaxNumDecimals = 0
        Me.BsAdjustControl1.MinimumLimit = 0.0!
        Me.BsAdjustControl1.MinimumSize = New System.Drawing.Size(153, 87)
        Me.BsAdjustControl1.Name = "BsAdjustControl1"
        Me.BsAdjustControl1.RangeTitle = "Range:"
        Me.BsAdjustControl1.SimulationMode = False
        Me.BsAdjustControl1.Size = New System.Drawing.Size(153, 106)
        Me.BsAdjustControl1.StepValues = CType(resources.GetObject("BsAdjustControl1.StepValues"), System.Collections.Generic.List(Of Integer))
        Me.BsAdjustControl1.TabIndex = 3
        Me.BsAdjustControl1.UnFocusedBackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustControl1.UnitsCaption = "steps"
        '
        'BsButtonsPanel
        '
        Me.BsButtonsPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButtonsPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsButtonsPanel.Controls.Add(Me.BsExitButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsSaveButton)
        Me.BsButtonsPanel.Location = New System.Drawing.Point(810, 557)
        Me.BsButtonsPanel.Name = "BsButtonsPanel"
        Me.BsButtonsPanel.Size = New System.Drawing.Size(168, 35)
        Me.BsButtonsPanel.TabIndex = 13
        '
        'BsExitButton
        '
        Me.BsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsExitButton.Enabled = False
        Me.BsExitButton.Location = New System.Drawing.Point(134, 1)
        Me.BsExitButton.Name = "BsExitButton"
        Me.BsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.BsExitButton.TabIndex = 3
        Me.BsExitButton.UseVisualStyleBackColor = True
        '
        'BsSaveButton
        '
        Me.BsSaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsSaveButton.Enabled = False
        Me.BsSaveButton.Location = New System.Drawing.Point(98, 1)
        Me.BsSaveButton.Name = "BsSaveButton"
        Me.BsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.BsSaveButton.TabIndex = 1
        Me.BsSaveButton.UseVisualStyleBackColor = True
        '
        'BsMessagesPanel
        '
        Me.BsMessagesPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessagesPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsMessagesPanel.Controls.Add(Me.ProgressBar1)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageImage)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageLabel)
        Me.BsMessagesPanel.Location = New System.Drawing.Point(0, 557)
        Me.BsMessagesPanel.Name = "BsMessagesPanel"
        Me.BsMessagesPanel.Size = New System.Drawing.Size(811, 35)
        Me.BsMessagesPanel.TabIndex = 12
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(494, 9)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(180, 18)
        Me.ProgressBar1.TabIndex = 5
        Me.ProgressBar1.Visible = False
        '
        'BsMessageImage
        '
        Me.BsMessageImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.BsMessageImage.Location = New System.Drawing.Point(3, 1)
        Me.BsMessageImage.Name = "BsMessageImage"
        Me.BsMessageImage.PositionNumber = 0
        Me.BsMessageImage.Size = New System.Drawing.Size(32, 32)
        Me.BsMessageImage.TabIndex = 3
        Me.BsMessageImage.TabStop = False
        '
        'BsMessageLabel
        '
        Me.BsMessageLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsMessageLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsMessageLabel.ForeColor = System.Drawing.Color.Black
        Me.BsMessageLabel.Location = New System.Drawing.Point(41, 11)
        Me.BsMessageLabel.Name = "BsMessageLabel"
        Me.BsMessageLabel.Size = New System.Drawing.Size(762, 13)
        Me.BsMessageLabel.TabIndex = 1
        Me.BsMessageLabel.Title = False
        '
        'BsResponse
        '
        Me.BsResponse.BackColor = System.Drawing.Color.White
        Me.BsResponse.DecimalsValues = False
        Me.BsResponse.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsResponse.IsNumeric = False
        Me.BsResponse.Location = New System.Drawing.Point(52, 57)
        Me.BsResponse.Mandatory = False
        Me.BsResponse.Multiline = True
        Me.BsResponse.Name = "BsResponse"
        Me.BsResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.BsResponse.Size = New System.Drawing.Size(836, 130)
        Me.BsResponse.TabIndex = 43
        '
        'IPositionsAdjustments
        '
        Me.AcceptButton = Me.BsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 593)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.Controls.Add(Me.BsTabPagesControl)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "IPositionsAdjustments"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = ""
        Me.BsTabPagesControl.ResumeLayout(False)
        Me.TabOpticCentering.ResumeLayout(False)
        Me.BsOpticInfoPanel.ResumeLayout(False)
        Me.BsOpticAdjustPanel.ResumeLayout(False)
        CType(Me.BsLEDCurrentTrackBar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsAbsorbancePanel.ResumeLayout(False)
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(SplineSeriesView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(SplineSeriesView2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Series2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(SplineSeriesView3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AbsorbanceChart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsOpticAdjustGroupBox.ResumeLayout(False)
        Me.BSOpticWSGroupBox.ResumeLayout(False)
        Me.TabWashingStation.ResumeLayout(False)
        Me.BsWashingInfoPanel.ResumeLayout(False)
        Me.BsWashingAdjustPanel.ResumeLayout(False)
        Me.BSWSWSGroupBox.ResumeLayout(False)
        Me.BsWSAdjustGroupBox.ResumeLayout(False)
        Me.TabArmsPositions.ResumeLayout(False)
        Me.BsArmsInfoPanel.ResumeLayout(False)
        Me.BsArmsAdjustPanel.ResumeLayout(False)
        Me.BsArmsWSGroupBox.ResumeLayout(False)
        Me.BsTabArmsControl.ResumeLayout(False)
        Me.TabSample.ResumeLayout(False)
        Me.TabReagent1.ResumeLayout(False)
        Me.TabReagent2.ResumeLayout(False)
        Me.TabMixer1.ResumeLayout(False)
        Me.TabMixer2.ResumeLayout(False)
        Me.TabPageTODELETE.ResumeLayout(False)
        Me.BsPanel1.ResumeLayout(False)
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsPanel2.ResumeLayout(False)
        CType(SwiftPlotDiagram1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(SwiftPlotSeriesView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Series3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(SwiftPlotSeriesView2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ChartControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsButtonsPanel.ResumeLayout(False)
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsResponse As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsOpticCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsOpticAdjustButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsTabPagesControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents TabOpticCentering As System.Windows.Forms.TabPage
    Friend WithEvents TabWashingStation As System.Windows.Forms.TabPage
    Friend WithEvents BsOpticInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsOpticAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents TabArmsPositions As System.Windows.Forms.TabPage
    Friend WithEvents BsArmsAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsTabArmsControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents TabSample As System.Windows.Forms.TabPage
    Friend WithEvents BsGridSample As BSGridControl
    Friend WithEvents TabReagent1 As System.Windows.Forms.TabPage
    Friend WithEvents BsGridReagent1 As BSGridControl
    Friend WithEvents TabReagent2 As System.Windows.Forms.TabPage
    Friend WithEvents BsGridReagent2 As BSGridControl
    Friend WithEvents TabMixer1 As System.Windows.Forms.TabPage
    Friend WithEvents BsGridMixer1 As BSGridControl
    Friend WithEvents TabMixer2 As System.Windows.Forms.TabPage
    Friend WithEvents BsGridMixer2 As BSGridControl
    Friend WithEvents BsAdjustRotor As BSAdjustControl
    Friend WithEvents BsAdjustZ As BSAdjustControl
    Friend WithEvents BsAdjustPolar As BSAdjustControl
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsAdjustOptic As BSAdjustControl
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsOpticAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsWashingAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsWashingAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsAdjustWashing As BSAdjustControl
    Friend WithEvents BsArmsInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsArmsInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsOpticInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsWashingInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsWashingInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsAbsorbancePanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents AbsorbanceChart As DevExpress.XtraCharts.ChartControl
    Friend WithEvents BsOpticAdjustmentLabel As System.Windows.Forms.Label
    Friend WithEvents BsOpticAdjustmentTitle As System.Windows.Forms.Label
    Friend WithEvents BsWashingAdjustmentTitle As System.Windows.Forms.Label
    Friend WithEvents BsWashingAdjustmentLabel As System.Windows.Forms.Label
    Friend WithEvents BsLEDCurrentTrackBar As System.Windows.Forms.TrackBar
    Friend WithEvents BsMinusLabel As System.Windows.Forms.Label
    Friend WithEvents BsPlusLabel As System.Windows.Forms.Label
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents BsStirrer1Button As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsStirrer2Button As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsLEDCurrentLabel As System.Windows.Forms.Label
    Friend WithEvents BsInfoOptXPSViewer As BsXPSViewer
    Friend WithEvents BsInfoWsXPSViewer As BsXPSViewer
    Friend WithEvents BsInfoArmsXPSViewer As BsXPSViewer
    Friend WithEvents BsUpDownWSButton1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsUpDownWSButton2 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsUpDownWSButton3 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsOpticStopButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents TabPageTODELETE As System.Windows.Forms.TabPage
    Friend WithEvents BsPanel1 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsButton1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TrackBar1 As System.Windows.Forms.TrackBar
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents BsPanel2 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents ChartControl1 As DevExpress.XtraCharts.ChartControl
    Friend WithEvents BsLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsAdjustControl1 As Biosystems.Ax00.Controls.UserControls.BSAdjustControl
    Friend WithEvents BsEncoderAdjustmentLabel As System.Windows.Forms.Label
    Friend WithEvents BsEncoderAdjustmentTitle As System.Windows.Forms.Label
    Friend WithEvents BsArmsWSGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsArmsWSLabel As System.Windows.Forms.Label
    Friend WithEvents BsButton6 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsWSAdjustGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BSWSWSGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsWSWSLabel As System.Windows.Forms.Label
    Friend WithEvents BsButton3 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BSOpticWSGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsOpticWSLabel As System.Windows.Forms.Label
    Friend WithEvents BsButton4 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsOpticAdjustGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsWSAdjustButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsWSCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsArmsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsArmsOkButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ToolTipController1 As DevExpress.Utils.ToolTipController
    Friend WithEvents BsArmsAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents Label7 As System.Windows.Forms.Label
End Class

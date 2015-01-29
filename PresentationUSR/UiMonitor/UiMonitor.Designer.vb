Imports Biosystems.Ax00.Global

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiMonitor
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            GlobalBase.CreateLogActivity("Initial - Dispose", Me.Name & ".Dispose", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            ReleaseElements()
        Finally
            MyBase.Dispose(disposing)
            GlobalBase.CreateLogActivity("Final - Dispose", Me.Name & ".Dispose", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiMonitor))
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim DataPoint1 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(0R, 100R)
        Dim DataPoint2 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(24R, 50R)
        Dim DataPoint3 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(74R, 100R)
        Dim DataPoint4 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(100R, 0R)
        Dim ChartArea2 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series2 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim DataPoint5 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(15R, 50R)
        Dim DataPoint6 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(40R, 75R)
        Dim DataPoint7 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(65R, 100R)
        Dim DataPoint8 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(90R, 20R)
        Dim DataPoint9 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(115R, 10R)
        Dim DataPoint10 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(140R, 90R)
        Dim DataPoint11 As System.Windows.Forms.DataVisualization.Charting.DataPoint = New System.Windows.Forms.DataVisualization.Charting.DataPoint(155R, 0R)
        Dim Title1 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim Title2 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim Title3 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim Title4 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim Title5 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim Title6 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.MonitorTabs = New DevExpress.XtraTab.XtraTabControl()
        Me.MainTab = New DevExpress.XtraTab.XtraTabPage()
        Me.chart2 = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.WasteLabel = New System.Windows.Forms.Label()
        Me.WashingLabel = New System.Windows.Forms.Label()
        Me.CoverSamplesPicture = New DevExpress.XtraEditors.PictureEdit()
        Me.CoverReactionsPicture = New BSGlyphPictureEdit()
        Me.CoverReagentsPicture = New DevExpress.XtraEditors.PictureEdit()
        Me.CoverOnPicture = New DevExpress.XtraEditors.PictureEdit()
        Me.CoverOffPicture = New DevExpress.XtraEditors.PictureEdit()
        Me.StatesTab = New DevExpress.XtraTab.XtraTabPage()
        Me.LegendWSGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.SampleStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.PendingLabel0 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.InProgressLabel0 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.FinishedLabel0 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LegendPendingImage0 = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LegendFinishedImage0 = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LegendInProgressImage0 = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.ExportLISLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.ExportLISImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.ResultsStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.PausedTestImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.PausedTestLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LockedTestImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.ResultsAbsImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LockedTestLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.ResultsAbsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.FinalReportLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.ResultsWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.ResultsReadyLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.TestStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.FinalReportImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.ResultsWarningImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.ResultsReadyImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsWorksessionLegendLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsGroupBox9 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.TotalTestsChart = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.bsTestStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsWSExecutionsDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.SamplesTab = New DevExpress.XtraTab.XtraTabPage()
        Me.PanelControl10 = New DevExpress.XtraEditors.PanelControl()
        Me.PanelControl6 = New DevExpress.XtraEditors.PanelControl()
        Me.PanelControl7 = New DevExpress.XtraEditors.PanelControl()
        Me.LegendSamplesGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BarcodeErrorLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.PendingLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.InProgressLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesLegendLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.FinishedLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LegendPendingImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LegendBarCodeErrorImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LegendFinishedImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LegendDepletedImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LegendInProgressImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LegendSelectedImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LegendNotInUseImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.DepletedLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.SelectedLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.NotInUseLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsGroupBox11 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsSampleStatusTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSamplesStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesMoveFirstPositionButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSamplesCellLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesPositionInfoLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesDiskNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesContentLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesNumberLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleCellTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSamplesDecreaseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSampleDiskNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsTubeSizeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsSampleContentTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSamplesIncreaseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsTubeSizeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleNumberTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSamplesMoveLastPositionButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSamplesBarcodeTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSampleIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDiluteStatusTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleTypeTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsDiluteStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSamplesBarcodeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.Sam3106 = New BSRImage()
        Me.Sam3128 = New BSRImage()
        Me.Sam3129 = New BSRImage()
        Me.Sam395 = New BSRImage()
        Me.Sam396 = New BSRImage()
        Me.Sam397 = New BSRImage()
        Me.Sam3107 = New BSRImage()
        Me.Sam3134 = New BSRImage()
        Me.Sam3133 = New BSRImage()
        Me.Sam3132 = New BSRImage()
        Me.Sam3131 = New BSRImage()
        Me.Sam3127 = New BSRImage()
        Me.Sam3126 = New BSRImage()
        Me.Sam3125 = New BSRImage()
        Me.Sam3124 = New BSRImage()
        Me.Sam3123 = New BSRImage()
        Me.Sam3122 = New BSRImage()
        Me.Sam3121 = New BSRImage()
        Me.Sam3120 = New BSRImage()
        Me.Sam3119 = New BSRImage()
        Me.Sam3118 = New BSRImage()
        Me.Sam3117 = New BSRImage()
        Me.Sam3116 = New BSRImage()
        Me.Sam3115 = New BSRImage()
        Me.Sam3114 = New BSRImage()
        Me.Sam3113 = New BSRImage()
        Me.Sam3112 = New BSRImage()
        Me.Sam3111 = New BSRImage()
        Me.Sam3110 = New BSRImage()
        Me.Sam3109 = New BSRImage()
        Me.Sam3108 = New BSRImage()
        Me.Sam3105 = New BSRImage()
        Me.Sam3104 = New BSRImage()
        Me.Sam3103 = New BSRImage()
        Me.Sam3102 = New BSRImage()
        Me.Sam3101 = New BSRImage()
        Me.Sam3100 = New BSRImage()
        Me.Sam3135 = New BSRImage()
        Me.Sam399 = New BSRImage()
        Me.Sam398 = New BSRImage()
        Me.Sam394 = New BSRImage()
        Me.Sam393 = New BSRImage()
        Me.Sam392 = New BSRImage()
        Me.Sam391 = New BSRImage()
        Me.Sam290 = New BSRImage()
        Me.Sam289 = New BSRImage()
        Me.Sam288 = New BSRImage()
        Me.Sam287 = New BSRImage()
        Me.Sam286 = New BSRImage()
        Me.Sam285 = New BSRImage()
        Me.Sam284 = New BSRImage()
        Me.Sam283 = New BSRImage()
        Me.Sam282 = New BSRImage()
        Me.Sam281 = New BSRImage()
        Me.Sam280 = New BSRImage()
        Me.Sam279 = New BSRImage()
        Me.Sam278 = New BSRImage()
        Me.Sam277 = New BSRImage()
        Me.Sam276 = New BSRImage()
        Me.Sam275 = New BSRImage()
        Me.Sam274 = New BSRImage()
        Me.Sam273 = New BSRImage()
        Me.Sam272 = New BSRImage()
        Me.Sam271 = New BSRImage()
        Me.Sam270 = New BSRImage()
        Me.Sam269 = New BSRImage()
        Me.Sam268 = New BSRImage()
        Me.Sam267 = New BSRImage()
        Me.Sam266 = New BSRImage()
        Me.Sam265 = New BSRImage()
        Me.Sam264 = New BSRImage()
        Me.Sam263 = New BSRImage()
        Me.Sam262 = New BSRImage()
        Me.Sam261 = New BSRImage()
        Me.Sam260 = New BSRImage()
        Me.Sam259 = New BSRImage()
        Me.Sam258 = New BSRImage()
        Me.Sam257 = New BSRImage()
        Me.Sam256 = New BSRImage()
        Me.Sam255 = New BSRImage()
        Me.Sam254 = New BSRImage()
        Me.Sam253 = New BSRImage()
        Me.Sam252 = New BSRImage()
        Me.Sam251 = New BSRImage()
        Me.Sam250 = New BSRImage()
        Me.Sam249 = New BSRImage()
        Me.Sam248 = New BSRImage()
        Me.Sam247 = New BSRImage()
        Me.Sam246 = New BSRImage()
        Me.Sam145 = New BSRImage()
        Me.Sam144 = New BSRImage()
        Me.Sam143 = New BSRImage()
        Me.Sam142 = New BSRImage()
        Me.Sam11 = New BSRImage()
        Me.Sam141 = New BSRImage()
        Me.Sam140 = New BSRImage()
        Me.Sam139 = New BSRImage()
        Me.Sam138 = New BSRImage()
        Me.Sam137 = New BSRImage()
        Me.Sam136 = New BSRImage()
        Me.Sam135 = New BSRImage()
        Me.Sam134 = New BSRImage()
        Me.Sam133 = New BSRImage()
        Me.Sam132 = New BSRImage()
        Me.Sam131 = New BSRImage()
        Me.Sam130 = New BSRImage()
        Me.Sam129 = New BSRImage()
        Me.Sam128 = New BSRImage()
        Me.Sam127 = New BSRImage()
        Me.Sam126 = New BSRImage()
        Me.Sam125 = New BSRImage()
        Me.Sam124 = New BSRImage()
        Me.Sam123 = New BSRImage()
        Me.Sam122 = New BSRImage()
        Me.Sam121 = New BSRImage()
        Me.Sam120 = New BSRImage()
        Me.Sam119 = New BSRImage()
        Me.Sam118 = New BSRImage()
        Me.Sam117 = New BSRImage()
        Me.Sam116 = New BSRImage()
        Me.Sam115 = New BSRImage()
        Me.Sam114 = New BSRImage()
        Me.Sam113 = New BSRImage()
        Me.Sam112 = New BSRImage()
        Me.Sam111 = New BSRImage()
        Me.Sam110 = New BSRImage()
        Me.Sam19 = New BSRImage()
        Me.Sam18 = New BSRImage()
        Me.Sam17 = New BSRImage()
        Me.Sam16 = New BSRImage()
        Me.Sam15 = New BSRImage()
        Me.Sam14 = New BSRImage()
        Me.Sam13 = New BSRImage()
        Me.Sam12 = New BSRImage()
        Me.Sam3130 = New BSRImage()
        Me.ReagentsTab = New DevExpress.XtraTab.XtraTabPage()
        Me.PanelControl9 = New DevExpress.XtraEditors.PanelControl()
        Me.Reag11 = New BSRImage()
        Me.Reag12 = New BSRImage()
        Me.Reag13 = New BSRImage()
        Me.Reag14 = New BSRImage()
        Me.Reag15 = New BSRImage()
        Me.Reag16 = New BSRImage()
        Me.Reag17 = New BSRImage()
        Me.Reag18 = New BSRImage()
        Me.Reag19 = New BSRImage()
        Me.Reag110 = New BSRImage()
        Me.Reag111 = New BSRImage()
        Me.Reag112 = New BSRImage()
        Me.Reag113 = New BSRImage()
        Me.Reag114 = New BSRImage()
        Me.Reag115 = New BSRImage()
        Me.Reag116 = New BSRImage()
        Me.Reag117 = New BSRImage()
        Me.Reag118 = New BSRImage()
        Me.Reag119 = New BSRImage()
        Me.Reag120 = New BSRImage()
        Me.Reag121 = New BSRImage()
        Me.Reag122 = New BSRImage()
        Me.Reag123 = New BSRImage()
        Me.Reag124 = New BSRImage()
        Me.Reag125 = New BSRImage()
        Me.Reag126 = New BSRImage()
        Me.Reag127 = New BSRImage()
        Me.Reag128 = New BSRImage()
        Me.Reag129 = New BSRImage()
        Me.Reag130 = New BSRImage()
        Me.Reag131 = New BSRImage()
        Me.Reag132 = New BSRImage()
        Me.Reag133 = New BSRImage()
        Me.Reag134 = New BSRImage()
        Me.Reag135 = New BSRImage()
        Me.Reag136 = New BSRImage()
        Me.Reag137 = New BSRImage()
        Me.Reag138 = New BSRImage()
        Me.Reag139 = New BSRImage()
        Me.Reag140 = New BSRImage()
        Me.Reag141 = New BSRImage()
        Me.Reag142 = New BSRImage()
        Me.Reag143 = New BSRImage()
        Me.Reag144 = New BSRImage()
        Me.PanelControl4 = New DevExpress.XtraEditors.PanelControl()
        Me.PanelControl3 = New DevExpress.XtraEditors.PanelControl()
        Me.LegendReagentsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.LegendUnknownImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.UnknownLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LegendBarCodeErrorRGImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.BarcodeErrorRGLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LowVolPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.ReagentPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LegReagLowVolLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LegReagentLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LegReagAdditionalSol = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LegReagNoInUseLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LegReagentSelLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LegReagDepleteLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.AdditionalSolPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.NoInUsePictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.SelectedPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsDepletedPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsReagentsLegendLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsPositionInfoGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsReagentsStatusTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsCellTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsCellLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTeststLeftTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsCurrentVolTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsTestsLeftLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsCurrentVolLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsBottleSizeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsBottleSizeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsExpirationDateTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsPositionInfoLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsMoveLastPositionButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsReagentsBarCodeTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsTestNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsIncreaseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsReagentsNumberTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsDecreaseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsReagentNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsContentTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsMoveFirstPositionButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsReagentsDiskNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsExpirationDateLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsBarCodeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTestNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsNumberLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsContentLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsDiskNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.Reag288 = New BSRImage()
        Me.Reag287 = New BSRImage()
        Me.Reag286 = New BSRImage()
        Me.Reag285 = New BSRImage()
        Me.Reag284 = New BSRImage()
        Me.Reag283 = New BSRImage()
        Me.Reag282 = New BSRImage()
        Me.Reag281 = New BSRImage()
        Me.Reag280 = New BSRImage()
        Me.Reag279 = New BSRImage()
        Me.Reag278 = New BSRImage()
        Me.Reag277 = New BSRImage()
        Me.Reag276 = New BSRImage()
        Me.Reag275 = New BSRImage()
        Me.Reag274 = New BSRImage()
        Me.Reag273 = New BSRImage()
        Me.Reag272 = New BSRImage()
        Me.Reag271 = New BSRImage()
        Me.Reag270 = New BSRImage()
        Me.Reag269 = New BSRImage()
        Me.Reag268 = New BSRImage()
        Me.Reag267 = New BSRImage()
        Me.Reag266 = New BSRImage()
        Me.Reag265 = New BSRImage()
        Me.Reag264 = New BSRImage()
        Me.Reag263 = New BSRImage()
        Me.Reag262 = New BSRImage()
        Me.Reag261 = New BSRImage()
        Me.Reag260 = New BSRImage()
        Me.Reag259 = New BSRImage()
        Me.Reag258 = New BSRImage()
        Me.Reag257 = New BSRImage()
        Me.Reag256 = New BSRImage()
        Me.Reag255 = New BSRImage()
        Me.Reag254 = New BSRImage()
        Me.Reag253 = New BSRImage()
        Me.Reag252 = New BSRImage()
        Me.Reag251 = New BSRImage()
        Me.Reag250 = New BSRImage()
        Me.Reag249 = New BSRImage()
        Me.Reag248 = New BSRImage()
        Me.Reag247 = New BSRImage()
        Me.Reag246 = New BSRImage()
        Me.Reag245 = New BSRImage()
        Me.ReactionsTab = New DevExpress.XtraTab.XtraTabPage()
        Me.Reac2 = New BSRImage()
        Me.Reac39 = New BSRImage()
        Me.Reac29 = New BSRImage()
        Me.Reac28 = New BSRImage()
        Me.Reac27 = New BSRImage()
        Me.Reac16 = New BSRImage()
        Me.Reac15 = New BSRImage()
        Me.Reac14 = New BSRImage()
        Me.Reac13 = New BSRImage()
        Me.Reac12 = New BSRImage()
        Me.Reac11 = New BSRImage()
        Me.Reac10 = New BSRImage()
        Me.Reac9 = New BSRImage()
        Me.Reac8 = New BSRImage()
        Me.Reac7 = New BSRImage()
        Me.Reac6 = New BSRImage()
        Me.Reac5 = New BSRImage()
        Me.Reac4 = New BSRImage()
        Me.Reac3 = New BSRImage()
        Me.Reac1 = New BSRImage()
        Me.Reac66 = New BSRImage()
        Me.Reac67 = New BSRImage()
        Me.Reac117 = New BSRImage()
        Me.Reac77 = New BSRImage()
        Me.Reac78 = New BSRImage()
        Me.Reac26 = New BSRImage()
        Me.Reac37 = New BSRImage()
        Me.Reac38 = New BSRImage()
        Me.Reac120 = New BSRImage()
        Me.Reac119 = New BSRImage()
        Me.Reac118 = New BSRImage()
        Me.Reac116 = New BSRImage()
        Me.Reac115 = New BSRImage()
        Me.Reac114 = New BSRImage()
        Me.Reac113 = New BSRImage()
        Me.Reac112 = New BSRImage()
        Me.Reac111 = New BSRImage()
        Me.Reac110 = New BSRImage()
        Me.Reac109 = New BSRImage()
        Me.Reac108 = New BSRImage()
        Me.Reac107 = New BSRImage()
        Me.Reac106 = New BSRImage()
        Me.Reac105 = New BSRImage()
        Me.Reac104 = New BSRImage()
        Me.Reac103 = New BSRImage()
        Me.Reac102 = New BSRImage()
        Me.Reac100 = New BSRImage()
        Me.Reac99 = New BSRImage()
        Me.Reac98 = New BSRImage()
        Me.Reac97 = New BSRImage()
        Me.Reac96 = New BSRImage()
        Me.Reac95 = New BSRImage()
        Me.Reac94 = New BSRImage()
        Me.Reac93 = New BSRImage()
        Me.Reac92 = New BSRImage()
        Me.Reac91 = New BSRImage()
        Me.Reac90 = New BSRImage()
        Me.Reac88 = New BSRImage()
        Me.Reac86 = New BSRImage()
        Me.Reac85 = New BSRImage()
        Me.Reac84 = New BSRImage()
        Me.Reac83 = New BSRImage()
        Me.Reac82 = New BSRImage()
        Me.Reac81 = New BSRImage()
        Me.Reac80 = New BSRImage()
        Me.Reac79 = New BSRImage()
        Me.Reac76 = New BSRImage()
        Me.Reac74 = New BSRImage()
        Me.Reac73 = New BSRImage()
        Me.Reac72 = New BSRImage()
        Me.Reac71 = New BSRImage()
        Me.Reac70 = New BSRImage()
        Me.Reac69 = New BSRImage()
        Me.Reac68 = New BSRImage()
        Me.Reac65 = New BSRImage()
        Me.Reac64 = New BSRImage()
        Me.Reac63 = New BSRImage()
        Me.Reac62 = New BSRImage()
        Me.Reac61 = New BSRImage()
        Me.Reac60 = New BSRImage()
        Me.Reac59 = New BSRImage()
        Me.Reac58 = New BSRImage()
        Me.Reac57 = New BSRImage()
        Me.Reac56 = New BSRImage()
        Me.Reac55 = New BSRImage()
        Me.Reac54 = New BSRImage()
        Me.Reac53 = New BSRImage()
        Me.Reac52 = New BSRImage()
        Me.Reac51 = New BSRImage()
        Me.Reac50 = New BSRImage()
        Me.Reac49 = New BSRImage()
        Me.Reac48 = New BSRImage()
        Me.Reac47 = New BSRImage()
        Me.Reac46 = New BSRImage()
        Me.Reac45 = New BSRImage()
        Me.Reac44 = New BSRImage()
        Me.Reac43 = New BSRImage()
        Me.Reac42 = New BSRImage()
        Me.Reac41 = New BSRImage()
        Me.Reac40 = New BSRImage()
        Me.Reac36 = New BSRImage()
        Me.Reac35 = New BSRImage()
        Me.Reac34 = New BSRImage()
        Me.Reac33 = New BSRImage()
        Me.Reac32 = New BSRImage()
        Me.Reac31 = New BSRImage()
        Me.Reac30 = New BSRImage()
        Me.Reac25 = New BSRImage()
        Me.Reac24 = New BSRImage()
        Me.Reac23 = New BSRImage()
        Me.Reac22 = New BSRImage()
        Me.Reac21 = New BSRImage()
        Me.Reac20 = New BSRImage()
        Me.Reac19 = New BSRImage()
        Me.Reac17 = New BSRImage()
        Me.PanelControl14 = New DevExpress.XtraEditors.PanelControl()
        Me.PanelControl15 = New DevExpress.XtraEditors.PanelControl()
        Me.LegendReactionsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsOpticalLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsContaminatedLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsOpticalPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsContaminatedPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsFinishLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsR1SampleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsR1SampleR2 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReactionsLegendLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDilutionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsR1SamplePictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsFinishPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsDilutionPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsR1PictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsR1SampleR2PictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsWashingPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsNotInUsePictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsR1Label = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsWashingLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsNotInUseLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsGroupBox13 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsReactionsMoveLastPositionButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsReactionsDecreaseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsReactionsIncreaseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsReactionsMoveFirstPositionButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsOrderTestIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsExecutionIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReactionsOpenGraph = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsReacTestTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReacTestNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsRerunTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReplicateTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsRerunLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReplicateLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReacStatusTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReacStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReactionsPositionInfoLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsWellNumberLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleClassLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsCalibNumLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsWellNrTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSampleClassTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsCalibNrTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReacSampleIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDilutionTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReacSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReacSampleTypeTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReacDilutionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsPatientIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.Reac18 = New BSRImage()
        Me.Reac75 = New BSRImage()
        Me.Reac87 = New BSRImage()
        Me.Reac89 = New BSRImage()
        Me.Reac101 = New BSRImage()
        Me.ISETab = New DevExpress.XtraTab.XtraTabPage()
        Me.BsIseMonitor = New Biosystems.Ax00.Controls.UserControls.BSISEMonitorPanel()
        Me.AlarmsTab = New DevExpress.XtraTab.XtraTabPage()
        Me.BsGroupBox4 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.AlarmsXtraGrid = New DevExpress.XtraGrid.GridControl()
        Me.AlarmsXtraGridView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.ToolTipController1 = New DevExpress.Utils.ToolTipController(Me.components)
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.ElapsedTimeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.TaskListProgressBar = New DevExpress.XtraEditors.ProgressBarControl()
        Me.PanelControl2 = New DevExpress.XtraEditors.PanelControl()
        Me.bsWamUpGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsEndWarmUp = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.TimeWarmUpProgressBar = New DevExpress.XtraEditors.ProgressBarControl()
        Me.bsTimeWUpLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsGroupBox14 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsTimeAvailableLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.AccessRRTimeTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.AccessRMTimeTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.AccessRMTimeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.AccessRRTimeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsGroupBox3 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.PanelControl11 = New DevExpress.XtraEditors.PanelControl()
        Me.RemainingTimeTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.ElapsedTimeTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.OverallTimeTextEdit = New DevExpress.XtraEditors.TextEdit()
        Me.OverallTimeTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.RemainingTimeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTimeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsGroupBox10 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsISELongTermDeactivated = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.TestRefresh = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.StateCfgLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsFridgeStatusLed = New Biosystems.Ax00.Controls.UserControls.bsLed()
        Me.bsConnectedLed = New Biosystems.Ax00.Controls.UserControls.bsLed()
        Me.bsISEStatusLed = New Biosystems.Ax00.Controls.UserControls.bsLed()
        Me.bsSamplesLegendGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsSensorsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReactionsTemperatureLed = New Biosystems.Ax00.Controls.UserControls.bsLed()
        Me.bsFridgeTemperatureLed = New Biosystems.Ax00.Controls.UserControls.bsLed()
        Me.BsTimer1 = New Biosystems.Ax00.Controls.UserControls.BSTimer()
        Me.BsGroupBox1 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsDataGridView1 = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        CType(Me.MonitorTabs,System.ComponentModel.ISupportInitialize).BeginInit
        Me.MonitorTabs.SuspendLayout
        Me.MainTab.SuspendLayout
        CType(Me.chart2,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.CoverSamplesPicture.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.CoverReactionsPicture.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.CoverReagentsPicture.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.CoverOnPicture.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.CoverOffPicture.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        Me.StatesTab.SuspendLayout
        Me.LegendWSGroupBox.SuspendLayout
        CType(Me.LegendPendingImage0,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LegendFinishedImage0,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LegendInProgressImage0,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.ExportLISImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PausedTestImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LockedTestImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.ResultsAbsImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.FinalReportImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.ResultsWarningImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.ResultsReadyImage,System.ComponentModel.ISupportInitialize).BeginInit
        Me.BsGroupBox9.SuspendLayout
        CType(Me.TotalTestsChart,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsWSExecutionsDataGridView,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SamplesTab.SuspendLayout
        CType(Me.PanelControl10,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PanelControl6,System.ComponentModel.ISupportInitialize).BeginInit
        Me.PanelControl6.SuspendLayout
        CType(Me.PanelControl7,System.ComponentModel.ISupportInitialize).BeginInit
        Me.PanelControl7.SuspendLayout
        Me.LegendSamplesGroupBox.SuspendLayout
        CType(Me.LegendPendingImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LegendBarCodeErrorImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LegendFinishedImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LegendDepletedImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LegendInProgressImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LegendSelectedImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LegendNotInUseImage,System.ComponentModel.ISupportInitialize).BeginInit
        Me.BsGroupBox11.SuspendLayout
        CType(Me.Sam3106,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3128,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3129,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam395,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam396,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam397,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3107,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3134,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3133,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3132,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3131,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3127,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3126,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3125,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3124,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3123,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3122,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3121,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3120,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3119,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3118,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3117,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3116,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3115,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3114,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3113,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3112,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3111,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3110,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3109,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3108,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3105,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3104,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3103,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3102,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3101,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3100,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3135,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam399,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam398,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam394,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam393,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam392,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam391,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam290,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam289,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam288,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam287,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam286,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam285,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam284,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam283,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam282,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam281,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam280,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam279,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam278,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam277,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam276,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam275,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam274,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam273,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam272,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam271,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam270,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam269,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam268,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam267,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam266,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam265,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam264,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam263,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam262,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam261,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam260,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam259,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam258,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam257,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam256,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam255,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam254,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam253,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam252,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam251,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam250,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam249,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam248,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam247,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam246,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam145,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam144,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam143,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam142,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam11,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam141,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam140,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam139,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam138,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam137,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam136,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam135,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam134,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam133,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam132,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam131,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam130,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam129,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam128,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam127,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam126,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam125,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam124,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam123,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam122,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam121,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam120,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam119,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam118,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam117,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam116,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam115,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam114,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam113,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam112,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam111,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam110,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam19,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam18,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam17,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam16,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam15,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam14,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam13,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam12,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Sam3130,System.ComponentModel.ISupportInitialize).BeginInit
        Me.ReagentsTab.SuspendLayout
        CType(Me.PanelControl9,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag11,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag12,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag13,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag14,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag15,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag16,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag17,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag18,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag19,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag110,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag111,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag112,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag113,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag114,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag115,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag116,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag117,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag118,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag119,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag120,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag121,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag122,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag123,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag124,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag125,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag126,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag127,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag128,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag129,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag130,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag131,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag132,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag133,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag134,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag135,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag136,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag137,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag138,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag139,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag140,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag141,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag142,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag143,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag144,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PanelControl4,System.ComponentModel.ISupportInitialize).BeginInit
        Me.PanelControl4.SuspendLayout
        CType(Me.PanelControl3,System.ComponentModel.ISupportInitialize).BeginInit
        Me.PanelControl3.SuspendLayout
        Me.LegendReagentsGroupBox.SuspendLayout
        CType(Me.LegendUnknownImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LegendBarCodeErrorRGImage,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.LowVolPictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.ReagentPictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.AdditionalSolPictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.NoInUsePictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.SelectedPictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsDepletedPictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        Me.bsReagentsPositionInfoGroupBox.SuspendLayout
        CType(Me.Reag288,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag287,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag286,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag285,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag284,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag283,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag282,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag281,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag280,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag279,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag278,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag277,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag276,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag275,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag274,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag273,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag272,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag271,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag270,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag269,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag268,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag267,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag266,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag265,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag264,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag263,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag262,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag261,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag260,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag259,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag258,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag257,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag256,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag255,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag254,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag253,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag252,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag251,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag250,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag249,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag248,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag247,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag246,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reag245,System.ComponentModel.ISupportInitialize).BeginInit
        Me.ReactionsTab.SuspendLayout
        CType(Me.Reac2,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac39,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac29,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac28,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac27,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac16,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac15,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac14,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac13,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac12,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac11,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac10,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac9,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac8,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac7,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac6,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac5,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac4,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac3,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac1,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac66,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac67,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac117,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac77,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac78,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac26,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac37,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac38,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac120,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac119,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac118,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac116,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac115,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac114,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac113,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac112,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac111,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac110,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac109,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac108,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac107,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac106,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac105,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac104,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac103,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac102,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac100,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac99,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac98,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac97,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac96,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac95,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac94,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac93,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac92,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac91,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac90,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac88,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac86,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac85,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac84,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac83,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac82,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac81,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac80,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac79,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac76,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac74,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac73,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac72,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac71,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac70,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac69,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac68,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac65,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac64,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac63,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac62,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac61,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac60,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac59,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac58,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac57,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac56,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac55,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac54,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac53,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac52,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac51,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac50,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac49,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac48,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac47,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac46,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac45,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac44,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac43,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac42,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac41,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac40,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac36,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac35,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac34,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac33,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac32,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac31,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac30,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac25,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac24,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac23,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac22,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac21,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac20,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac19,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac17,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PanelControl14,System.ComponentModel.ISupportInitialize).BeginInit
        Me.PanelControl14.SuspendLayout
        CType(Me.PanelControl15,System.ComponentModel.ISupportInitialize).BeginInit
        Me.PanelControl15.SuspendLayout
        Me.LegendReactionsGroupBox.SuspendLayout
        CType(Me.bsOpticalPictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsContaminatedPictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsR1SamplePictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsFinishPictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsDilutionPictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsR1PictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsR1SampleR2PictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsWashingPictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsNotInUsePictureBox,System.ComponentModel.ISupportInitialize).BeginInit
        Me.BsGroupBox13.SuspendLayout
        CType(Me.Reac18,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac75,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac87,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac89,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.Reac101,System.ComponentModel.ISupportInitialize).BeginInit
        Me.ISETab.SuspendLayout
        Me.AlarmsTab.SuspendLayout
        Me.BsGroupBox4.SuspendLayout
        CType(Me.AlarmsXtraGrid,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.AlarmsXtraGridView,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.TaskListProgressBar.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.PanelControl2,System.ComponentModel.ISupportInitialize).BeginInit
        Me.PanelControl2.SuspendLayout
        Me.bsWamUpGroupBox.SuspendLayout
        CType(Me.TimeWarmUpProgressBar.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        Me.BsGroupBox14.SuspendLayout
        CType(Me.AccessRRTimeTextEdit.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.AccessRMTimeTextEdit.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        Me.BsGroupBox3.SuspendLayout
        CType(Me.PanelControl11,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.RemainingTimeTextEdit.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.ElapsedTimeTextEdit.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.OverallTimeTextEdit.Properties,System.ComponentModel.ISupportInitialize).BeginInit
        Me.BsGroupBox10.SuspendLayout
        CType(Me.BsISELongTermDeactivated,System.ComponentModel.ISupportInitialize).BeginInit
        Me.bsSamplesLegendGroupBox.SuspendLayout
        Me.BsGroupBox1.SuspendLayout
        CType(Me.BsDataGridView1,System.ComponentModel.ISupportInitialize).BeginInit
        CType(Me.bsErrorProvider1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'MonitorTabs
        '
        Me.MonitorTabs.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
        Me.MonitorTabs.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.MonitorTabs.Appearance.Options.UseBackColor = true
        Me.MonitorTabs.Appearance.Options.UseFont = true
        Me.MonitorTabs.AppearancePage.Header.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.MonitorTabs.AppearancePage.Header.ForeColor = System.Drawing.Color.Black
        Me.MonitorTabs.AppearancePage.Header.Options.UseFont = true
        Me.MonitorTabs.AppearancePage.Header.Options.UseForeColor = true
        Me.MonitorTabs.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.MonitorTabs.Location = New System.Drawing.Point(208, 1)
        Me.MonitorTabs.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003
        Me.MonitorTabs.LookAndFeel.UseDefaultLookAndFeel = false
        Me.MonitorTabs.LookAndFeel.UseWindowsXPTheme = true
        Me.MonitorTabs.Name = "MonitorTabs"
        Me.MonitorTabs.SelectedTabPage = Me.MainTab
        Me.MonitorTabs.Size = New System.Drawing.Size(770, 652)
        Me.MonitorTabs.TabIndex = 17
        Me.MonitorTabs.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.MainTab, Me.StatesTab, Me.SamplesTab, Me.ReagentsTab, Me.ReactionsTab, Me.ISETab, Me.AlarmsTab})
        '
        'MainTab
        '
        Me.MainTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.MainTab.Appearance.PageClient.Image = CType(resources.GetObject("MainTab.Appearance.PageClient.Image"),System.Drawing.Image)
        Me.MainTab.Appearance.PageClient.Options.UseBackColor = true
        Me.MainTab.Appearance.PageClient.Options.UseImage = true
        Me.MainTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.MainTab.Controls.Add(Me.chart2)
        Me.MainTab.Controls.Add(Me.WasteLabel)
        Me.MainTab.Controls.Add(Me.WashingLabel)
        Me.MainTab.Controls.Add(Me.CoverSamplesPicture)
        Me.MainTab.Controls.Add(Me.CoverReactionsPicture)
        Me.MainTab.Controls.Add(Me.CoverReagentsPicture)
        Me.MainTab.Controls.Add(Me.CoverOnPicture)
        Me.MainTab.Controls.Add(Me.CoverOffPicture)
        Me.MainTab.Name = "MainTab"
        Me.MainTab.Size = New System.Drawing.Size(762, 623)
        Me.MainTab.Text = "Main"
        '
        'chart2
        '
        Me.chart2.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.chart2.BackColor = System.Drawing.Color.Transparent
        Me.chart2.BackImageTransparentColor = System.Drawing.Color.Transparent
        Me.chart2.BackSecondaryColor = System.Drawing.Color.Transparent
        Me.chart2.BorderlineColor = System.Drawing.Color.Transparent
        Me.chart2.BorderSkin.BackColor = System.Drawing.Color.Transparent
        Me.chart2.BorderSkin.BackImageTransparentColor = System.Drawing.Color.Transparent
        Me.chart2.BorderSkin.BackSecondaryColor = System.Drawing.Color.Transparent
        Me.chart2.BorderSkin.BorderColor = System.Drawing.Color.Transparent
        Me.chart2.BorderSkin.BorderWidth = 0
        Me.chart2.BorderSkin.PageColor = System.Drawing.Color.Transparent
        ChartArea1.AlignmentOrientation = System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.None
        ChartArea1.AlignmentStyle = System.Windows.Forms.DataVisualization.Charting.AreaAlignmentStyles.None
        ChartArea1.Area3DStyle.Inclination = 15
        ChartArea1.Area3DStyle.IsClustered = true
        ChartArea1.Area3DStyle.IsRightAngleAxes = false
        ChartArea1.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.None
        ChartArea1.Area3DStyle.Rotation = 10
        ChartArea1.Area3DStyle.WallWidth = 0
        ChartArea1.AxisX.IsLabelAutoFit = false
        ChartArea1.AxisX.IsMarginVisible = false
        ChartArea1.AxisX.IsStartedFromZero = false
        ChartArea1.AxisX.LabelStyle.Font = New System.Drawing.Font("Trebuchet MS", 10!, System.Drawing.FontStyle.Bold)
        ChartArea1.AxisX.LabelStyle.Format = "F2"
        ChartArea1.AxisX.LineColor = System.Drawing.Color.Transparent
        ChartArea1.AxisX.MajorGrid.Enabled = false
        ChartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer))
        ChartArea1.AxisX.MajorTickMark.Enabled = false
        ChartArea1.AxisX.Minimum = 0R
        ChartArea1.AxisX.ScaleView.Zoomable = false
        ChartArea1.AxisX.ScrollBar.LineColor = System.Drawing.Color.Black
        ChartArea1.AxisX.ScrollBar.Size = 10R
        ChartArea1.AxisX2.LineColor = System.Drawing.Color.Transparent
        ChartArea1.AxisY.IsMarginVisible = false
        ChartArea1.AxisY.LabelStyle.Font = New System.Drawing.Font("Trebuchet MS", 8.25!, System.Drawing.FontStyle.Bold)
        ChartArea1.AxisY.LabelStyle.Format = "F4"
        ChartArea1.AxisY.LineColor = System.Drawing.Color.Transparent
        ChartArea1.AxisY.MajorGrid.Enabled = false
        ChartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer))
        ChartArea1.AxisY.MajorTickMark.Enabled = false
        ChartArea1.AxisY.ScrollBar.LineColor = System.Drawing.Color.Black
        ChartArea1.AxisY.ScrollBar.Size = 10R
        ChartArea1.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Stacked
        ChartArea1.AxisY2.LineColor = System.Drawing.Color.Transparent
        ChartArea1.BackColor = System.Drawing.Color.Transparent
        ChartArea1.BackSecondaryColor = System.Drawing.Color.White
        ChartArea1.BorderColor = System.Drawing.Color.FromArgb(CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer))
        ChartArea1.BorderWidth = 0
        ChartArea1.InnerPlotPosition.Auto = false
        ChartArea1.InnerPlotPosition.Height = 100!
        ChartArea1.InnerPlotPosition.Width = 100!
        ChartArea1.IsSameFontSizeForAllAxes = true
        ChartArea1.Name = "Default"
        ChartArea1.Position.Auto = false
        ChartArea1.Position.Height = 100!
        ChartArea1.Position.Width = 100!
        ChartArea1.ShadowColor = System.Drawing.Color.Transparent
        Me.chart2.ChartAreas.Add(ChartArea1)
        Me.chart2.IsSoftShadows = false
        Me.chart2.Location = New System.Drawing.Point(312, 254)
        Me.chart2.Name = "chart2"
        Series1.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.NarrowHorizontal
        Series1.BackSecondaryColor = System.Drawing.Color.Transparent
        Series1.BorderColor = System.Drawing.Color.Transparent
        Series1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet
        Series1.BorderWidth = 0
        Series1.ChartArea = "Default"
        Series1.Color = System.Drawing.Color.SteelBlue
        Series1.CustomProperties = "PixelPointWidth=40, DrawSideBySide=True"
        Series1.EmptyPointStyle.BorderColor = System.Drawing.Color.Transparent
        Series1.EmptyPointStyle.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet
        Series1.EmptyPointStyle.BorderWidth = 0
        Series1.EmptyPointStyle.Color = System.Drawing.Color.Transparent
        Series1.EmptyPointStyle.MarkerBorderColor = System.Drawing.Color.Transparent
        Series1.EmptyPointStyle.MarkerColor = System.Drawing.Color.Transparent
        Series1.Font = New System.Drawing.Font("Verdana", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Series1.LabelForeColor = System.Drawing.Color.FromArgb(CType(CType(200,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer))
        Series1.LabelFormat = "{0}%"
        Series1.MarkerSize = 1
        Series1.Name = "Series1"
        DataPoint1.Color = System.Drawing.Color.Transparent
        Series1.Points.Add(DataPoint1)
        Series1.Points.Add(DataPoint2)
        Series1.Points.Add(DataPoint3)
        Series1.Points.Add(DataPoint4)
        Series1.ShadowColor = System.Drawing.Color.Transparent
        Series1.SmartLabelStyle.AllowOutsidePlotArea = System.Windows.Forms.DataVisualization.Charting.LabelOutsidePlotAreaStyle.Yes
        Series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32
        Series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32
        Me.chart2.Series.Add(Series1)
        Me.chart2.Size = New System.Drawing.Size(145, 63)
        Me.chart2.TabIndex = 278
        Me.chart2.TabStop = false
        '
        'WasteLabel
        '
        Me.WasteLabel.BackColor = System.Drawing.Color.Transparent
        Me.WasteLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.WasteLabel.ForeColor = System.Drawing.Color.Gray
        Me.WasteLabel.Location = New System.Drawing.Point(398, 210)
        Me.WasteLabel.Name = "WasteLabel"
        Me.WasteLabel.Size = New System.Drawing.Size(42, 16)
        Me.WasteLabel.TabIndex = 32
        Me.WasteLabel.Text = "100%"
        Me.WasteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'WashingLabel
        '
        Me.WashingLabel.BackColor = System.Drawing.Color.Transparent
        Me.WashingLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.WashingLabel.ForeColor = System.Drawing.Color.Gray
        Me.WashingLabel.Location = New System.Drawing.Point(326, 210)
        Me.WashingLabel.Name = "WashingLabel"
        Me.WashingLabel.Size = New System.Drawing.Size(44, 16)
        Me.WashingLabel.TabIndex = 31
        Me.WashingLabel.Text = "100%"
        Me.WashingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CoverSamplesPicture
        '
        Me.CoverSamplesPicture.EditValue = CType(resources.GetObject("CoverSamplesPicture.EditValue"),Object)
        Me.CoverSamplesPicture.Location = New System.Drawing.Point(413, 447)
        Me.CoverSamplesPicture.Name = "CoverSamplesPicture"
        Me.CoverSamplesPicture.Properties.AllowFocused = false
        Me.CoverSamplesPicture.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.CoverSamplesPicture.Properties.Appearance.Options.UseBackColor = true
        Me.CoverSamplesPicture.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.CoverSamplesPicture.Properties.ShowMenu = false
        Me.CoverSamplesPicture.Size = New System.Drawing.Size(124, 124)
        Me.CoverSamplesPicture.TabIndex = 281
        '
        'CoverReactionsPicture
        '
        Me.CoverReactionsPicture.EditValue = CType(resources.GetObject("CoverReactionsPicture.EditValue"),Object)
        Me.CoverReactionsPicture.Location = New System.Drawing.Point(345, 411)
        Me.CoverReactionsPicture.Name = "CoverReactionsPicture"
        Me.CoverReactionsPicture.Properties.AllowFocused = false
        Me.CoverReactionsPicture.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.CoverReactionsPicture.Properties.Appearance.Options.UseBackColor = true
        Me.CoverReactionsPicture.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.CoverReactionsPicture.Properties.ShowMenu = false
        Me.CoverReactionsPicture.Properties.UseParentBackground = true
        Me.CoverReactionsPicture.Size = New System.Drawing.Size(64, 66)
        Me.CoverReactionsPicture.TabIndex = 280
        '
        'CoverReagentsPicture
        '
        Me.CoverReagentsPicture.EditValue = CType(resources.GetObject("CoverReagentsPicture.EditValue"),Object)
        Me.CoverReagentsPicture.Location = New System.Drawing.Point(236, 447)
        Me.CoverReagentsPicture.Name = "CoverReagentsPicture"
        Me.CoverReagentsPicture.Properties.AllowFocused = false
        Me.CoverReagentsPicture.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.CoverReagentsPicture.Properties.Appearance.Options.UseBackColor = true
        Me.CoverReagentsPicture.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.CoverReagentsPicture.Properties.ShowMenu = false
        Me.CoverReagentsPicture.Properties.UseParentBackground = true
        Me.CoverReagentsPicture.Size = New System.Drawing.Size(124, 124)
        Me.CoverReagentsPicture.TabIndex = 279
        '
        'CoverOnPicture
        '
        Me.CoverOnPicture.EditValue = CType(resources.GetObject("CoverOnPicture.EditValue"),Object)
        Me.CoverOnPicture.Location = New System.Drawing.Point(218, 0)
        Me.CoverOnPicture.Name = "CoverOnPicture"
        Me.CoverOnPicture.Properties.AllowFocused = false
        Me.CoverOnPicture.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.CoverOnPicture.Properties.Appearance.Options.UseBackColor = true
        Me.CoverOnPicture.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.CoverOnPicture.Properties.ShowMenu = false
        Me.CoverOnPicture.Size = New System.Drawing.Size(332, 127)
        Me.CoverOnPicture.TabIndex = 277
        '
        'CoverOffPicture
        '
        Me.CoverOffPicture.EditValue = CType(resources.GetObject("CoverOffPicture.EditValue"),Object)
        Me.CoverOffPicture.Location = New System.Drawing.Point(218, 0)
        Me.CoverOffPicture.Name = "CoverOffPicture"
        Me.CoverOffPicture.Properties.AllowFocused = false
        Me.CoverOffPicture.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.CoverOffPicture.Properties.Appearance.Options.UseBackColor = true
        Me.CoverOffPicture.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.CoverOffPicture.Properties.ShowMenu = false
        Me.CoverOffPicture.Size = New System.Drawing.Size(332, 127)
        Me.CoverOffPicture.TabIndex = 276
        Me.CoverOffPicture.Visible = false
        '
        'StatesTab
        '
        Me.StatesTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.StatesTab.Appearance.PageClient.Options.UseBackColor = true
        Me.StatesTab.Controls.Add(Me.LegendWSGroupBox)
        Me.StatesTab.Controls.Add(Me.BsGroupBox9)
        Me.StatesTab.Controls.Add(Me.bsWSExecutionsDataGridView)
        Me.StatesTab.Name = "StatesTab"
        Me.StatesTab.Size = New System.Drawing.Size(762, 623)
        Me.StatesTab.Text = "Worksession"
        '
        'LegendWSGroupBox
        '
        Me.LegendWSGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.LegendWSGroupBox.Controls.Add(Me.SampleStatusLabel)
        Me.LegendWSGroupBox.Controls.Add(Me.PendingLabel0)
        Me.LegendWSGroupBox.Controls.Add(Me.InProgressLabel0)
        Me.LegendWSGroupBox.Controls.Add(Me.FinishedLabel0)
        Me.LegendWSGroupBox.Controls.Add(Me.LegendPendingImage0)
        Me.LegendWSGroupBox.Controls.Add(Me.LegendFinishedImage0)
        Me.LegendWSGroupBox.Controls.Add(Me.LegendInProgressImage0)
        Me.LegendWSGroupBox.Controls.Add(Me.ExportLISLabel)
        Me.LegendWSGroupBox.Controls.Add(Me.ExportLISImage)
        Me.LegendWSGroupBox.Controls.Add(Me.ResultsStatusLabel)
        Me.LegendWSGroupBox.Controls.Add(Me.PausedTestImage)
        Me.LegendWSGroupBox.Controls.Add(Me.PausedTestLabel)
        Me.LegendWSGroupBox.Controls.Add(Me.LockedTestImage)
        Me.LegendWSGroupBox.Controls.Add(Me.ResultsAbsImage)
        Me.LegendWSGroupBox.Controls.Add(Me.LockedTestLabel)
        Me.LegendWSGroupBox.Controls.Add(Me.ResultsAbsLabel)
        Me.LegendWSGroupBox.Controls.Add(Me.FinalReportLabel)
        Me.LegendWSGroupBox.Controls.Add(Me.ResultsWarningLabel)
        Me.LegendWSGroupBox.Controls.Add(Me.ResultsReadyLabel)
        Me.LegendWSGroupBox.Controls.Add(Me.TestStatusLabel)
        Me.LegendWSGroupBox.Controls.Add(Me.FinalReportImage)
        Me.LegendWSGroupBox.Controls.Add(Me.ResultsWarningImage)
        Me.LegendWSGroupBox.Controls.Add(Me.ResultsReadyImage)
        Me.LegendWSGroupBox.Controls.Add(Me.bsWorksessionLegendLabel)
        Me.LegendWSGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.LegendWSGroupBox.ForeColor = System.Drawing.Color.Black
        Me.LegendWSGroupBox.Location = New System.Drawing.Point(490, 284)
        Me.LegendWSGroupBox.Name = "LegendWSGroupBox"
        Me.LegendWSGroupBox.Size = New System.Drawing.Size(270, 338)
        Me.LegendWSGroupBox.TabIndex = 180
        Me.LegendWSGroupBox.TabStop = false
        Me.LegendWSGroupBox.Visible = false
        '
        'SampleStatusLabel
        '
        Me.SampleStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.SampleStatusLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.SampleStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.SampleStatusLabel.Location = New System.Drawing.Point(6, 247)
        Me.SampleStatusLabel.Name = "SampleStatusLabel"
        Me.SampleStatusLabel.Size = New System.Drawing.Size(258, 13)
        Me.SampleStatusLabel.TabIndex = 49
        Me.SampleStatusLabel.Text = "*Sample Status"
        Me.SampleStatusLabel.Title = false
        '
        'PendingLabel0
        '
        Me.PendingLabel0.BackColor = System.Drawing.Color.Transparent
        Me.PendingLabel0.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.PendingLabel0.ForeColor = System.Drawing.Color.Black
        Me.PendingLabel0.Location = New System.Drawing.Point(52, 270)
        Me.PendingLabel0.Name = "PendingLabel0"
        Me.PendingLabel0.Size = New System.Drawing.Size(213, 13)
        Me.PendingLabel0.TabIndex = 48
        Me.PendingLabel0.Text = "Pending"
        Me.PendingLabel0.Title = false
        '
        'InProgressLabel0
        '
        Me.InProgressLabel0.BackColor = System.Drawing.Color.Transparent
        Me.InProgressLabel0.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.InProgressLabel0.ForeColor = System.Drawing.Color.Black
        Me.InProgressLabel0.Location = New System.Drawing.Point(52, 294)
        Me.InProgressLabel0.Name = "InProgressLabel0"
        Me.InProgressLabel0.Size = New System.Drawing.Size(213, 13)
        Me.InProgressLabel0.TabIndex = 46
        Me.InProgressLabel0.Text = "In Progress"
        Me.InProgressLabel0.Title = false
        '
        'FinishedLabel0
        '
        Me.FinishedLabel0.BackColor = System.Drawing.Color.Transparent
        Me.FinishedLabel0.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.FinishedLabel0.ForeColor = System.Drawing.Color.Black
        Me.FinishedLabel0.Location = New System.Drawing.Point(52, 316)
        Me.FinishedLabel0.Name = "FinishedLabel0"
        Me.FinishedLabel0.Size = New System.Drawing.Size(213, 13)
        Me.FinishedLabel0.TabIndex = 45
        Me.FinishedLabel0.Text = "Finished"
        Me.FinishedLabel0.Title = false
        '
        'LegendPendingImage0
        '
        Me.LegendPendingImage0.BackColor = System.Drawing.Color.Transparent
        Me.LegendPendingImage0.InitialImage = CType(resources.GetObject("LegendPendingImage0.InitialImage"),System.Drawing.Image)
        Me.LegendPendingImage0.Location = New System.Drawing.Point(30, 267)
        Me.LegendPendingImage0.Name = "LegendPendingImage0"
        Me.LegendPendingImage0.PositionNumber = 0
        Me.LegendPendingImage0.Size = New System.Drawing.Size(20, 20)
        Me.LegendPendingImage0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.LegendPendingImage0.TabIndex = 47
        Me.LegendPendingImage0.TabStop = false
        '
        'LegendFinishedImage0
        '
        Me.LegendFinishedImage0.BackColor = System.Drawing.Color.Transparent
        Me.LegendFinishedImage0.InitialImage = CType(resources.GetObject("LegendFinishedImage0.InitialImage"),System.Drawing.Image)
        Me.LegendFinishedImage0.Location = New System.Drawing.Point(30, 313)
        Me.LegendFinishedImage0.Name = "LegendFinishedImage0"
        Me.LegendFinishedImage0.PositionNumber = 0
        Me.LegendFinishedImage0.Size = New System.Drawing.Size(20, 20)
        Me.LegendFinishedImage0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.LegendFinishedImage0.TabIndex = 44
        Me.LegendFinishedImage0.TabStop = false
        '
        'LegendInProgressImage0
        '
        Me.LegendInProgressImage0.BackColor = System.Drawing.Color.Transparent
        Me.LegendInProgressImage0.InitialImage = CType(resources.GetObject("LegendInProgressImage0.InitialImage"),System.Drawing.Image)
        Me.LegendInProgressImage0.Location = New System.Drawing.Point(30, 290)
        Me.LegendInProgressImage0.Name = "LegendInProgressImage0"
        Me.LegendInProgressImage0.PositionNumber = 0
        Me.LegendInProgressImage0.Size = New System.Drawing.Size(20, 20)
        Me.LegendInProgressImage0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.LegendInProgressImage0.TabIndex = 43
        Me.LegendInProgressImage0.TabStop = false
        '
        'ExportLISLabel
        '
        Me.ExportLISLabel.BackColor = System.Drawing.Color.Transparent
        Me.ExportLISLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.ExportLISLabel.ForeColor = System.Drawing.Color.Black
        Me.ExportLISLabel.Location = New System.Drawing.Point(52, 151)
        Me.ExportLISLabel.Name = "ExportLISLabel"
        Me.ExportLISLabel.Size = New System.Drawing.Size(213, 13)
        Me.ExportLISLabel.TabIndex = 42
        Me.ExportLISLabel.Text = "*Exported to LIS"
        Me.ExportLISLabel.Title = false
        '
        'ExportLISImage
        '
        Me.ExportLISImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExportLISImage.ErrorImage = Nothing
        Me.ExportLISImage.InitialImage = CType(resources.GetObject("ExportLISImage.InitialImage"),System.Drawing.Image)
        Me.ExportLISImage.Location = New System.Drawing.Point(30, 147)
        Me.ExportLISImage.Name = "ExportLISImage"
        Me.ExportLISImage.PositionNumber = 0
        Me.ExportLISImage.Size = New System.Drawing.Size(20, 20)
        Me.ExportLISImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.ExportLISImage.TabIndex = 41
        Me.ExportLISImage.TabStop = false
        '
        'ResultsStatusLabel
        '
        Me.ResultsStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.ResultsStatusLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.ResultsStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.ResultsStatusLabel.Location = New System.Drawing.Point(6, 37)
        Me.ResultsStatusLabel.Name = "ResultsStatusLabel"
        Me.ResultsStatusLabel.Size = New System.Drawing.Size(258, 13)
        Me.ResultsStatusLabel.TabIndex = 40
        Me.ResultsStatusLabel.Text = "*Estados Resultados"
        Me.ResultsStatusLabel.Title = false
        '
        'PausedTestImage
        '
        Me.PausedTestImage.InitialImage = CType(resources.GetObject("PausedTestImage.InitialImage"),System.Drawing.Image)
        Me.PausedTestImage.Location = New System.Drawing.Point(29, 218)
        Me.PausedTestImage.Name = "PausedTestImage"
        Me.PausedTestImage.PositionNumber = 0
        Me.PausedTestImage.Size = New System.Drawing.Size(20, 20)
        Me.PausedTestImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.PausedTestImage.TabIndex = 37
        Me.PausedTestImage.TabStop = false
        '
        'PausedTestLabel
        '
        Me.PausedTestLabel.BackColor = System.Drawing.Color.Transparent
        Me.PausedTestLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.PausedTestLabel.ForeColor = System.Drawing.Color.Black
        Me.PausedTestLabel.Location = New System.Drawing.Point(52, 222)
        Me.PausedTestLabel.Name = "PausedTestLabel"
        Me.PausedTestLabel.Size = New System.Drawing.Size(213, 13)
        Me.PausedTestLabel.TabIndex = 36
        Me.PausedTestLabel.Text = "*Paused test"
        Me.PausedTestLabel.Title = false
        '
        'LockedTestImage
        '
        Me.LockedTestImage.InitialImage = CType(resources.GetObject("LockedTestImage.InitialImage"),System.Drawing.Image)
        Me.LockedTestImage.Location = New System.Drawing.Point(29, 196)
        Me.LockedTestImage.Name = "LockedTestImage"
        Me.LockedTestImage.PositionNumber = 0
        Me.LockedTestImage.Size = New System.Drawing.Size(20, 20)
        Me.LockedTestImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.LockedTestImage.TabIndex = 35
        Me.LockedTestImage.TabStop = false
        '
        'ResultsAbsImage
        '
        Me.ResultsAbsImage.InitialImage = CType(resources.GetObject("ResultsAbsImage.InitialImage"),System.Drawing.Image)
        Me.ResultsAbsImage.Location = New System.Drawing.Point(30, 101)
        Me.ResultsAbsImage.Name = "ResultsAbsImage"
        Me.ResultsAbsImage.PositionNumber = 0
        Me.ResultsAbsImage.Size = New System.Drawing.Size(20, 20)
        Me.ResultsAbsImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.ResultsAbsImage.TabIndex = 34
        Me.ResultsAbsImage.TabStop = false
        '
        'LockedTestLabel
        '
        Me.LockedTestLabel.BackColor = System.Drawing.Color.Transparent
        Me.LockedTestLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LockedTestLabel.ForeColor = System.Drawing.Color.Black
        Me.LockedTestLabel.Location = New System.Drawing.Point(52, 199)
        Me.LockedTestLabel.Name = "LockedTestLabel"
        Me.LockedTestLabel.Size = New System.Drawing.Size(213, 13)
        Me.LockedTestLabel.TabIndex = 33
        Me.LockedTestLabel.Text = "*Locked test"
        Me.LockedTestLabel.Title = false
        '
        'ResultsAbsLabel
        '
        Me.ResultsAbsLabel.BackColor = System.Drawing.Color.Transparent
        Me.ResultsAbsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.ResultsAbsLabel.ForeColor = System.Drawing.Color.Black
        Me.ResultsAbsLabel.Location = New System.Drawing.Point(52, 104)
        Me.ResultsAbsLabel.Name = "ResultsAbsLabel"
        Me.ResultsAbsLabel.Size = New System.Drawing.Size(213, 13)
        Me.ResultsAbsLabel.TabIndex = 32
        Me.ResultsAbsLabel.Text = "*Abs/t graph"
        Me.ResultsAbsLabel.Title = false
        '
        'FinalReportLabel
        '
        Me.FinalReportLabel.BackColor = System.Drawing.Color.Transparent
        Me.FinalReportLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.FinalReportLabel.ForeColor = System.Drawing.Color.Black
        Me.FinalReportLabel.Location = New System.Drawing.Point(52, 127)
        Me.FinalReportLabel.Name = "FinalReportLabel"
        Me.FinalReportLabel.Size = New System.Drawing.Size(213, 13)
        Me.FinalReportLabel.TabIndex = 31
        Me.FinalReportLabel.Text = "*Final report available"
        Me.FinalReportLabel.Title = false
        '
        'ResultsWarningLabel
        '
        Me.ResultsWarningLabel.BackColor = System.Drawing.Color.Transparent
        Me.ResultsWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.ResultsWarningLabel.ForeColor = System.Drawing.Color.Black
        Me.ResultsWarningLabel.Location = New System.Drawing.Point(52, 81)
        Me.ResultsWarningLabel.Name = "ResultsWarningLabel"
        Me.ResultsWarningLabel.Size = New System.Drawing.Size(213, 13)
        Me.ResultsWarningLabel.TabIndex = 30
        Me.ResultsWarningLabel.Text = "*Results with Warnings"
        Me.ResultsWarningLabel.Title = false
        '
        'ResultsReadyLabel
        '
        Me.ResultsReadyLabel.BackColor = System.Drawing.Color.Transparent
        Me.ResultsReadyLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.ResultsReadyLabel.ForeColor = System.Drawing.Color.Black
        Me.ResultsReadyLabel.Location = New System.Drawing.Point(52, 59)
        Me.ResultsReadyLabel.Name = "ResultsReadyLabel"
        Me.ResultsReadyLabel.Size = New System.Drawing.Size(213, 13)
        Me.ResultsReadyLabel.TabIndex = 29
        Me.ResultsReadyLabel.Text = "*Results Ready"
        Me.ResultsReadyLabel.Title = false
        '
        'TestStatusLabel
        '
        Me.TestStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.TestStatusLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.TestStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.TestStatusLabel.Location = New System.Drawing.Point(6, 177)
        Me.TestStatusLabel.Name = "TestStatusLabel"
        Me.TestStatusLabel.Size = New System.Drawing.Size(258, 13)
        Me.TestStatusLabel.TabIndex = 26
        Me.TestStatusLabel.Text = "*Estado técnicas"
        Me.TestStatusLabel.Title = false
        '
        'FinalReportImage
        '
        Me.FinalReportImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.FinalReportImage.ErrorImage = Nothing
        Me.FinalReportImage.InitialImage = CType(resources.GetObject("FinalReportImage.InitialImage"),System.Drawing.Image)
        Me.FinalReportImage.Location = New System.Drawing.Point(30, 124)
        Me.FinalReportImage.Name = "FinalReportImage"
        Me.FinalReportImage.PositionNumber = 0
        Me.FinalReportImage.Size = New System.Drawing.Size(20, 20)
        Me.FinalReportImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.FinalReportImage.TabIndex = 27
        Me.FinalReportImage.TabStop = false
        '
        'ResultsWarningImage
        '
        Me.ResultsWarningImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ResultsWarningImage.ErrorImage = Nothing
        Me.ResultsWarningImage.InitialImage = CType(resources.GetObject("ResultsWarningImage.InitialImage"),System.Drawing.Image)
        Me.ResultsWarningImage.Location = New System.Drawing.Point(30, 78)
        Me.ResultsWarningImage.Name = "ResultsWarningImage"
        Me.ResultsWarningImage.PositionNumber = 0
        Me.ResultsWarningImage.Size = New System.Drawing.Size(20, 20)
        Me.ResultsWarningImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.ResultsWarningImage.TabIndex = 28
        Me.ResultsWarningImage.TabStop = false
        '
        'ResultsReadyImage
        '
        Me.ResultsReadyImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ResultsReadyImage.ErrorImage = Nothing
        Me.ResultsReadyImage.InitialImage = CType(resources.GetObject("ResultsReadyImage.InitialImage"),System.Drawing.Image)
        Me.ResultsReadyImage.Location = New System.Drawing.Point(30, 55)
        Me.ResultsReadyImage.Name = "ResultsReadyImage"
        Me.ResultsReadyImage.PositionNumber = 0
        Me.ResultsReadyImage.Size = New System.Drawing.Size(20, 20)
        Me.ResultsReadyImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.ResultsReadyImage.TabIndex = 27
        Me.ResultsReadyImage.TabStop = false
        '
        'bsWorksessionLegendLabel
        '
        Me.bsWorksessionLegendLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsWorksessionLegendLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsWorksessionLegendLabel.ForeColor = System.Drawing.Color.Black
        Me.bsWorksessionLegendLabel.Location = New System.Drawing.Point(5, 12)
        Me.bsWorksessionLegendLabel.Name = "bsWorksessionLegendLabel"
        Me.bsWorksessionLegendLabel.Size = New System.Drawing.Size(260, 19)
        Me.bsWorksessionLegendLabel.TabIndex = 25
        Me.bsWorksessionLegendLabel.Text = "Legend"
        Me.bsWorksessionLegendLabel.Title = true
        '
        'BsGroupBox9
        '
        Me.BsGroupBox9.BackColor = System.Drawing.Color.Transparent
        Me.BsGroupBox9.Controls.Add(Me.TotalTestsChart)
        Me.BsGroupBox9.Controls.Add(Me.bsTestStatusLabel)
        Me.BsGroupBox9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.BsGroupBox9.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox9.Location = New System.Drawing.Point(490, -4)
        Me.BsGroupBox9.Name = "BsGroupBox9"
        Me.BsGroupBox9.Size = New System.Drawing.Size(270, 288)
        Me.BsGroupBox9.TabIndex = 177
        Me.BsGroupBox9.TabStop = false
        '
        'TotalTestsChart
        '
        Me.TotalTestsChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.DiagonalLeft
        Me.TotalTestsChart.BackSecondaryColor = System.Drawing.Color.Gainsboro
        Me.TotalTestsChart.BorderlineColor = System.Drawing.Color.FromArgb(CType(CType(26,Byte),Integer), CType(CType(59,Byte),Integer), CType(CType(105,Byte),Integer))
        Me.TotalTestsChart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid
        Me.TotalTestsChart.BorderSkin.PageColor = System.Drawing.Color.Gainsboro
        ChartArea2.AlignmentOrientation = System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.None
        ChartArea2.AlignmentStyle = System.Windows.Forms.DataVisualization.Charting.AreaAlignmentStyles.None
        ChartArea2.Area3DStyle.Inclination = 15
        ChartArea2.Area3DStyle.IsClustered = true
        ChartArea2.Area3DStyle.IsRightAngleAxes = false
        ChartArea2.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.None
        ChartArea2.Area3DStyle.Rotation = 10
        ChartArea2.Area3DStyle.WallWidth = 0
        ChartArea2.AxisX.IsLabelAutoFit = false
        ChartArea2.AxisX.IsMarginVisible = false
        ChartArea2.AxisX.IsStartedFromZero = false
        ChartArea2.AxisX.LabelStyle.Font = New System.Drawing.Font("Trebuchet MS", 10!, System.Drawing.FontStyle.Bold)
        ChartArea2.AxisX.LabelStyle.Format = "F2"
        ChartArea2.AxisX.LineColor = System.Drawing.Color.FromArgb(CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer))
        ChartArea2.AxisX.MajorGrid.Enabled = false
        ChartArea2.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer))
        ChartArea2.AxisX.MajorTickMark.Enabled = false
        ChartArea2.AxisX.Minimum = 0R
        ChartArea2.AxisX.ScaleView.Zoomable = false
        ChartArea2.AxisX.ScrollBar.LineColor = System.Drawing.Color.Black
        ChartArea2.AxisX.ScrollBar.Size = 10R
        ChartArea2.AxisY.IsMarginVisible = false
        ChartArea2.AxisY.LabelStyle.Font = New System.Drawing.Font("Trebuchet MS", 8.25!, System.Drawing.FontStyle.Bold)
        ChartArea2.AxisY.LabelStyle.Format = "F4"
        ChartArea2.AxisY.LineColor = System.Drawing.Color.FromArgb(CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer))
        ChartArea2.AxisY.MajorGrid.Enabled = false
        ChartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer))
        ChartArea2.AxisY.MajorTickMark.Enabled = false
        ChartArea2.AxisY.ScrollBar.LineColor = System.Drawing.Color.Black
        ChartArea2.AxisY.ScrollBar.Size = 10R
        ChartArea2.AxisY.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Stacked
        ChartArea2.BackColor = System.Drawing.Color.Transparent
        ChartArea2.BackSecondaryColor = System.Drawing.Color.White
        ChartArea2.BorderColor = System.Drawing.Color.FromArgb(CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer), CType(CType(64,Byte),Integer))
        ChartArea2.BorderWidth = 0
        ChartArea2.InnerPlotPosition.Auto = false
        ChartArea2.InnerPlotPosition.Height = 100!
        ChartArea2.InnerPlotPosition.Width = 100!
        ChartArea2.IsSameFontSizeForAllAxes = true
        ChartArea2.Name = "Default"
        ChartArea2.Position.Auto = false
        ChartArea2.Position.Height = 100!
        ChartArea2.Position.Width = 60!
        ChartArea2.Position.X = 37!
        ChartArea2.ShadowColor = System.Drawing.Color.Transparent
        Me.TotalTestsChart.ChartAreas.Add(ChartArea2)
        Me.TotalTestsChart.IsSoftShadows = false
        Legend1.BackColor = System.Drawing.Color.Transparent
        Legend1.Enabled = false
        Legend1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Legend1.IsTextAutoFit = false
        Legend1.Name = "Default"
        Me.TotalTestsChart.Legends.Add(Legend1)
        Me.TotalTestsChart.Location = New System.Drawing.Point(5, 36)
        Me.TotalTestsChart.Name = "TotalTestsChart"
        Series2.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.DiagonalLeft
        Series2.BackSecondaryColor = System.Drawing.Color.Transparent
        Series2.BorderColor = System.Drawing.Color.FromArgb(CType(CType(180,Byte),Integer), CType(CType(26,Byte),Integer), CType(CType(59,Byte),Integer), CType(CType(105,Byte),Integer))
        Series2.ChartArea = "Default"
        Series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar
        Series2.Color = System.Drawing.Color.SteelBlue
        Series2.CustomProperties = "PixelPointWidth=25, BarLabelStyle=Right, EmptyPointValue=Zero, PointWidth=1"
        Series2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Series2.IsValueShownAsLabel = true
        Series2.LabelBackColor = System.Drawing.Color.Transparent
        Series2.LabelBorderColor = System.Drawing.Color.Black
        Series2.LabelBorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet
        Series2.Legend = "Default"
        Series2.MarkerSize = 1
        Series2.Name = "Series1"
        DataPoint5.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.None
        DataPoint5.Color = System.Drawing.Color.FromArgb(CType(CType(237,Byte),Integer), CType(CType(28,Byte),Integer), CType(CType(36,Byte),Integer))
        DataPoint5.LabelBackColor = System.Drawing.Color.Transparent
        DataPoint5.LabelBorderColor = System.Drawing.Color.Black
        DataPoint6.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.None
        DataPoint6.Color = System.Drawing.Color.FromArgb(CType(CType(127,Byte),Integer), CType(CType(127,Byte),Integer), CType(CType(127,Byte),Integer))
        DataPoint6.LabelBackColor = System.Drawing.Color.Transparent
        DataPoint6.LabelBorderColor = System.Drawing.Color.Black
        DataPoint7.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.None
        DataPoint7.Color = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(255,Byte),Integer), CType(CType(153,Byte),Integer))
        DataPoint7.LabelBackColor = System.Drawing.Color.Transparent
        DataPoint7.LabelBorderColor = System.Drawing.Color.Black
        DataPoint7.MarkerSize = 1
        DataPoint8.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.None
        DataPoint8.Color = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(201,Byte),Integer), CType(CType(14,Byte),Integer))
        DataPoint8.LabelBackColor = System.Drawing.Color.Transparent
        DataPoint8.LabelBorderColor = System.Drawing.Color.Black
        DataPoint9.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.None
        DataPoint9.Color = System.Drawing.Color.FromArgb(CType(CType(96,Byte),Integer), CType(CType(233,Byte),Integer), CType(CType(75,Byte),Integer))
        DataPoint9.LabelBackColor = System.Drawing.Color.Transparent
        DataPoint9.LabelBorderColor = System.Drawing.Color.Black
        DataPoint10.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.None
        DataPoint10.LabelBackColor = System.Drawing.Color.Transparent
        DataPoint10.LabelBorderColor = System.Drawing.Color.Black
        DataPoint11.IsEmpty = true
        DataPoint11.IsValueShownAsLabel = true
        Series2.Points.Add(DataPoint5)
        Series2.Points.Add(DataPoint6)
        Series2.Points.Add(DataPoint7)
        Series2.Points.Add(DataPoint8)
        Series2.Points.Add(DataPoint9)
        Series2.Points.Add(DataPoint10)
        Series2.Points.Add(DataPoint11)
        Series2.ShadowColor = System.Drawing.Color.Empty
        Series2.SmartLabelStyle.AllowOutsidePlotArea = System.Windows.Forms.DataVisualization.Charting.LabelOutsidePlotAreaStyle.Yes
        Series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32
        Series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32
        Me.TotalTestsChart.Series.Add(Series2)
        Me.TotalTestsChart.Size = New System.Drawing.Size(260, 246)
        Me.TotalTestsChart.TabIndex = 26
        Me.TotalTestsChart.TabStop = false
        Title1.Alignment = System.Drawing.ContentAlignment.MiddleLeft
        Title1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Title1.Name = "Title1"
        Title1.Position.Auto = false
        Title1.Position.Height = 4.609879!
        Title1.Position.Width = 94!
        Title1.Position.X = 1!
        Title1.Position.Y = 8!
        Title1.ShadowColor = System.Drawing.Color.FromArgb(CType(CType(32,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer))
        Title1.Text = "TOTALS"
        Title1.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal
        Title2.Alignment = System.Drawing.ContentAlignment.MiddleLeft
        Title2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Title2.Name = "Title2"
        Title2.Position.Auto = false
        Title2.Position.Height = 4.609879!
        Title2.Position.Width = 94!
        Title2.Position.X = 1!
        Title2.Position.Y = 24!
        Title2.ShadowColor = System.Drawing.Color.FromArgb(CType(CType(32,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer))
        Title2.Text = "Finished"
        Title2.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal
        Title3.Alignment = System.Drawing.ContentAlignment.MiddleLeft
        Title3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Title3.Name = "Title3"
        Title3.Position.Auto = false
        Title3.Position.Width = 94!
        Title3.Position.X = 1!
        Title3.Position.Y = 42!
        Title3.ShadowColor = System.Drawing.Color.FromArgb(CType(CType(32,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer))
        Title3.Text = "In Process"
        Title3.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal
        Title4.Alignment = System.Drawing.ContentAlignment.MiddleLeft
        Title4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Title4.Name = "Title4"
        Title4.Position.Auto = false
        Title4.Position.Height = 5.180103!
        Title4.Position.Width = 94!
        Title4.Position.X = 1!
        Title4.Position.Y = 55!
        Title4.ShadowColor = System.Drawing.Color.FromArgb(CType(CType(32,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer))
        Title4.Text = "Pending"
        Title4.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal
        Title5.Alignment = System.Drawing.ContentAlignment.MiddleLeft
        Title5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Title5.Name = "Title5"
        Title5.Position.Auto = false
        Title5.Position.Height = 5.180103!
        Title5.Position.Width = 94!
        Title5.Position.X = 1!
        Title5.Position.Y = 72!
        Title5.ShadowColor = System.Drawing.Color.FromArgb(CType(CType(32,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer))
        Title5.Text = "Locked"
        Title5.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal
        Title6.Alignment = System.Drawing.ContentAlignment.MiddleLeft
        Title6.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Title6.Name = "Title6"
        Title6.Position.Auto = false
        Title6.Position.Height = 5.180103!
        Title6.Position.Width = 94!
        Title6.Position.X = 1!
        Title6.Position.Y = 88!
        Title6.ShadowColor = System.Drawing.Color.FromArgb(CType(CType(32,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer), CType(CType(0,Byte),Integer))
        Title6.Text = "With Alarms"
        Title6.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal
        Me.TotalTestsChart.Titles.Add(Title1)
        Me.TotalTestsChart.Titles.Add(Title2)
        Me.TotalTestsChart.Titles.Add(Title3)
        Me.TotalTestsChart.Titles.Add(Title4)
        Me.TotalTestsChart.Titles.Add(Title5)
        Me.TotalTestsChart.Titles.Add(Title6)
        '
        'bsTestStatusLabel
        '
        Me.bsTestStatusLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTestStatusLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsTestStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestStatusLabel.Location = New System.Drawing.Point(5, 12)
        Me.bsTestStatusLabel.Name = "bsTestStatusLabel"
        Me.bsTestStatusLabel.Size = New System.Drawing.Size(260, 20)
        Me.bsTestStatusLabel.TabIndex = 25
        Me.bsTestStatusLabel.Text = "Tests Status"
        Me.bsTestStatusLabel.Title = true
        '
        'bsWSExecutionsDataGridView
        '
        Me.bsWSExecutionsDataGridView.AllowUserToAddRows = false
        Me.bsWSExecutionsDataGridView.AllowUserToDeleteRows = false
        Me.bsWSExecutionsDataGridView.AllowUserToResizeColumns = false
        Me.bsWSExecutionsDataGridView.AllowUserToResizeRows = false
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.bsWSExecutionsDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsWSExecutionsDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsWSExecutionsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsWSExecutionsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsWSExecutionsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsWSExecutionsDataGridView.ColumnHeadersHeight = 30
        Me.bsWSExecutionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 10!)
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsWSExecutionsDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsWSExecutionsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsWSExecutionsDataGridView.EnterToTab = false
        Me.bsWSExecutionsDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsWSExecutionsDataGridView.Location = New System.Drawing.Point(1, 3)
        Me.bsWSExecutionsDataGridView.MultiSelect = false
        Me.bsWSExecutionsDataGridView.Name = "bsWSExecutionsDataGridView"
        Me.bsWSExecutionsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsWSExecutionsDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsWSExecutionsDataGridView.RowHeadersVisible = false
        Me.bsWSExecutionsDataGridView.RowHeadersWidth = 20
        Me.bsWSExecutionsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black
        Me.bsWSExecutionsDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsWSExecutionsDataGridView.RowTemplate.Height = 28
        Me.bsWSExecutionsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsWSExecutionsDataGridView.Size = New System.Drawing.Size(485, 618)
        Me.bsWSExecutionsDataGridView.TabIndex = 172
        Me.bsWSExecutionsDataGridView.TabToEnter = false
        '
        'SamplesTab
        '
        Me.SamplesTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.SamplesTab.Appearance.PageClient.Image = CType(resources.GetObject("SamplesTab.Appearance.PageClient.Image"),System.Drawing.Image)
        Me.SamplesTab.Appearance.PageClient.Options.UseBackColor = true
        Me.SamplesTab.Appearance.PageClient.Options.UseImage = true
        Me.SamplesTab.Controls.Add(Me.PanelControl10)
        Me.SamplesTab.Controls.Add(Me.PanelControl6)
        Me.SamplesTab.Controls.Add(Me.Sam3106)
        Me.SamplesTab.Controls.Add(Me.Sam3128)
        Me.SamplesTab.Controls.Add(Me.Sam3129)
        Me.SamplesTab.Controls.Add(Me.Sam395)
        Me.SamplesTab.Controls.Add(Me.Sam396)
        Me.SamplesTab.Controls.Add(Me.Sam397)
        Me.SamplesTab.Controls.Add(Me.Sam3107)
        Me.SamplesTab.Controls.Add(Me.Sam3134)
        Me.SamplesTab.Controls.Add(Me.Sam3133)
        Me.SamplesTab.Controls.Add(Me.Sam3132)
        Me.SamplesTab.Controls.Add(Me.Sam3131)
        Me.SamplesTab.Controls.Add(Me.Sam3127)
        Me.SamplesTab.Controls.Add(Me.Sam3126)
        Me.SamplesTab.Controls.Add(Me.Sam3125)
        Me.SamplesTab.Controls.Add(Me.Sam3124)
        Me.SamplesTab.Controls.Add(Me.Sam3123)
        Me.SamplesTab.Controls.Add(Me.Sam3122)
        Me.SamplesTab.Controls.Add(Me.Sam3121)
        Me.SamplesTab.Controls.Add(Me.Sam3120)
        Me.SamplesTab.Controls.Add(Me.Sam3119)
        Me.SamplesTab.Controls.Add(Me.Sam3118)
        Me.SamplesTab.Controls.Add(Me.Sam3117)
        Me.SamplesTab.Controls.Add(Me.Sam3116)
        Me.SamplesTab.Controls.Add(Me.Sam3115)
        Me.SamplesTab.Controls.Add(Me.Sam3114)
        Me.SamplesTab.Controls.Add(Me.Sam3113)
        Me.SamplesTab.Controls.Add(Me.Sam3112)
        Me.SamplesTab.Controls.Add(Me.Sam3111)
        Me.SamplesTab.Controls.Add(Me.Sam3110)
        Me.SamplesTab.Controls.Add(Me.Sam3109)
        Me.SamplesTab.Controls.Add(Me.Sam3108)
        Me.SamplesTab.Controls.Add(Me.Sam3105)
        Me.SamplesTab.Controls.Add(Me.Sam3104)
        Me.SamplesTab.Controls.Add(Me.Sam3103)
        Me.SamplesTab.Controls.Add(Me.Sam3102)
        Me.SamplesTab.Controls.Add(Me.Sam3101)
        Me.SamplesTab.Controls.Add(Me.Sam3100)
        Me.SamplesTab.Controls.Add(Me.Sam3135)
        Me.SamplesTab.Controls.Add(Me.Sam399)
        Me.SamplesTab.Controls.Add(Me.Sam398)
        Me.SamplesTab.Controls.Add(Me.Sam394)
        Me.SamplesTab.Controls.Add(Me.Sam393)
        Me.SamplesTab.Controls.Add(Me.Sam392)
        Me.SamplesTab.Controls.Add(Me.Sam391)
        Me.SamplesTab.Controls.Add(Me.Sam290)
        Me.SamplesTab.Controls.Add(Me.Sam289)
        Me.SamplesTab.Controls.Add(Me.Sam288)
        Me.SamplesTab.Controls.Add(Me.Sam287)
        Me.SamplesTab.Controls.Add(Me.Sam286)
        Me.SamplesTab.Controls.Add(Me.Sam285)
        Me.SamplesTab.Controls.Add(Me.Sam284)
        Me.SamplesTab.Controls.Add(Me.Sam283)
        Me.SamplesTab.Controls.Add(Me.Sam282)
        Me.SamplesTab.Controls.Add(Me.Sam281)
        Me.SamplesTab.Controls.Add(Me.Sam280)
        Me.SamplesTab.Controls.Add(Me.Sam279)
        Me.SamplesTab.Controls.Add(Me.Sam278)
        Me.SamplesTab.Controls.Add(Me.Sam277)
        Me.SamplesTab.Controls.Add(Me.Sam276)
        Me.SamplesTab.Controls.Add(Me.Sam275)
        Me.SamplesTab.Controls.Add(Me.Sam274)
        Me.SamplesTab.Controls.Add(Me.Sam273)
        Me.SamplesTab.Controls.Add(Me.Sam272)
        Me.SamplesTab.Controls.Add(Me.Sam271)
        Me.SamplesTab.Controls.Add(Me.Sam270)
        Me.SamplesTab.Controls.Add(Me.Sam269)
        Me.SamplesTab.Controls.Add(Me.Sam268)
        Me.SamplesTab.Controls.Add(Me.Sam267)
        Me.SamplesTab.Controls.Add(Me.Sam266)
        Me.SamplesTab.Controls.Add(Me.Sam265)
        Me.SamplesTab.Controls.Add(Me.Sam264)
        Me.SamplesTab.Controls.Add(Me.Sam263)
        Me.SamplesTab.Controls.Add(Me.Sam262)
        Me.SamplesTab.Controls.Add(Me.Sam261)
        Me.SamplesTab.Controls.Add(Me.Sam260)
        Me.SamplesTab.Controls.Add(Me.Sam259)
        Me.SamplesTab.Controls.Add(Me.Sam258)
        Me.SamplesTab.Controls.Add(Me.Sam257)
        Me.SamplesTab.Controls.Add(Me.Sam256)
        Me.SamplesTab.Controls.Add(Me.Sam255)
        Me.SamplesTab.Controls.Add(Me.Sam254)
        Me.SamplesTab.Controls.Add(Me.Sam253)
        Me.SamplesTab.Controls.Add(Me.Sam252)
        Me.SamplesTab.Controls.Add(Me.Sam251)
        Me.SamplesTab.Controls.Add(Me.Sam250)
        Me.SamplesTab.Controls.Add(Me.Sam249)
        Me.SamplesTab.Controls.Add(Me.Sam248)
        Me.SamplesTab.Controls.Add(Me.Sam247)
        Me.SamplesTab.Controls.Add(Me.Sam246)
        Me.SamplesTab.Controls.Add(Me.Sam145)
        Me.SamplesTab.Controls.Add(Me.Sam144)
        Me.SamplesTab.Controls.Add(Me.Sam143)
        Me.SamplesTab.Controls.Add(Me.Sam142)
        Me.SamplesTab.Controls.Add(Me.Sam11)
        Me.SamplesTab.Controls.Add(Me.Sam141)
        Me.SamplesTab.Controls.Add(Me.Sam140)
        Me.SamplesTab.Controls.Add(Me.Sam139)
        Me.SamplesTab.Controls.Add(Me.Sam138)
        Me.SamplesTab.Controls.Add(Me.Sam137)
        Me.SamplesTab.Controls.Add(Me.Sam136)
        Me.SamplesTab.Controls.Add(Me.Sam135)
        Me.SamplesTab.Controls.Add(Me.Sam134)
        Me.SamplesTab.Controls.Add(Me.Sam133)
        Me.SamplesTab.Controls.Add(Me.Sam132)
        Me.SamplesTab.Controls.Add(Me.Sam131)
        Me.SamplesTab.Controls.Add(Me.Sam130)
        Me.SamplesTab.Controls.Add(Me.Sam129)
        Me.SamplesTab.Controls.Add(Me.Sam128)
        Me.SamplesTab.Controls.Add(Me.Sam127)
        Me.SamplesTab.Controls.Add(Me.Sam126)
        Me.SamplesTab.Controls.Add(Me.Sam125)
        Me.SamplesTab.Controls.Add(Me.Sam124)
        Me.SamplesTab.Controls.Add(Me.Sam123)
        Me.SamplesTab.Controls.Add(Me.Sam122)
        Me.SamplesTab.Controls.Add(Me.Sam121)
        Me.SamplesTab.Controls.Add(Me.Sam120)
        Me.SamplesTab.Controls.Add(Me.Sam119)
        Me.SamplesTab.Controls.Add(Me.Sam118)
        Me.SamplesTab.Controls.Add(Me.Sam117)
        Me.SamplesTab.Controls.Add(Me.Sam116)
        Me.SamplesTab.Controls.Add(Me.Sam115)
        Me.SamplesTab.Controls.Add(Me.Sam114)
        Me.SamplesTab.Controls.Add(Me.Sam113)
        Me.SamplesTab.Controls.Add(Me.Sam112)
        Me.SamplesTab.Controls.Add(Me.Sam111)
        Me.SamplesTab.Controls.Add(Me.Sam110)
        Me.SamplesTab.Controls.Add(Me.Sam19)
        Me.SamplesTab.Controls.Add(Me.Sam18)
        Me.SamplesTab.Controls.Add(Me.Sam17)
        Me.SamplesTab.Controls.Add(Me.Sam16)
        Me.SamplesTab.Controls.Add(Me.Sam15)
        Me.SamplesTab.Controls.Add(Me.Sam14)
        Me.SamplesTab.Controls.Add(Me.Sam13)
        Me.SamplesTab.Controls.Add(Me.Sam12)
        Me.SamplesTab.Controls.Add(Me.Sam3130)
        Me.SamplesTab.Name = "SamplesTab"
        Me.SamplesTab.Size = New System.Drawing.Size(762, 623)
        Me.SamplesTab.Text = "Samples"
        '
        'PanelControl10
        '
        Me.PanelControl10.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelControl10.Appearance.Options.UseBackColor = true
        Me.PanelControl10.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl10.Location = New System.Drawing.Point(0, 593)
        Me.PanelControl10.Name = "PanelControl10"
        Me.PanelControl10.Size = New System.Drawing.Size(569, 34)
        Me.PanelControl10.TabIndex = 319
        '
        'PanelControl6
        '
        Me.PanelControl6.Appearance.BackColor = System.Drawing.Color.White
        Me.PanelControl6.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl6.Appearance.Options.UseBackColor = true
        Me.PanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl6.Controls.Add(Me.PanelControl7)
        Me.PanelControl6.Location = New System.Drawing.Point(575, -4)
        Me.PanelControl6.Name = "PanelControl6"
        Me.PanelControl6.Size = New System.Drawing.Size(190, 627)
        Me.PanelControl6.TabIndex = 318
        '
        'PanelControl7
        '
        Me.PanelControl7.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.PanelControl7.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl7.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.PanelControl7.Appearance.Options.UseBackColor = true
        Me.PanelControl7.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.PanelControl7.Controls.Add(Me.LegendSamplesGroupBox)
        Me.PanelControl7.Controls.Add(Me.BsGroupBox11)
        Me.PanelControl7.Location = New System.Drawing.Point(3, 4)
        Me.PanelControl7.Name = "PanelControl7"
        Me.PanelControl7.Size = New System.Drawing.Size(183, 620)
        Me.PanelControl7.TabIndex = 262
        '
        'LegendSamplesGroupBox
        '
        Me.LegendSamplesGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.LegendSamplesGroupBox.Controls.Add(Me.BarcodeErrorLabel)
        Me.LegendSamplesGroupBox.Controls.Add(Me.PendingLabel)
        Me.LegendSamplesGroupBox.Controls.Add(Me.InProgressLabel)
        Me.LegendSamplesGroupBox.Controls.Add(Me.bsSamplesLegendLabel)
        Me.LegendSamplesGroupBox.Controls.Add(Me.FinishedLabel)
        Me.LegendSamplesGroupBox.Controls.Add(Me.LegendPendingImage)
        Me.LegendSamplesGroupBox.Controls.Add(Me.LegendBarCodeErrorImage)
        Me.LegendSamplesGroupBox.Controls.Add(Me.LegendFinishedImage)
        Me.LegendSamplesGroupBox.Controls.Add(Me.LegendDepletedImage)
        Me.LegendSamplesGroupBox.Controls.Add(Me.LegendInProgressImage)
        Me.LegendSamplesGroupBox.Controls.Add(Me.LegendSelectedImage)
        Me.LegendSamplesGroupBox.Controls.Add(Me.LegendNotInUseImage)
        Me.LegendSamplesGroupBox.Controls.Add(Me.DepletedLabel)
        Me.LegendSamplesGroupBox.Controls.Add(Me.SelectedLabel)
        Me.LegendSamplesGroupBox.Controls.Add(Me.NotInUseLabel)
        Me.LegendSamplesGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.LegendSamplesGroupBox.ForeColor = System.Drawing.Color.Black
        Me.LegendSamplesGroupBox.Location = New System.Drawing.Point(4, 333)
        Me.LegendSamplesGroupBox.Name = "LegendSamplesGroupBox"
        Me.LegendSamplesGroupBox.Size = New System.Drawing.Size(175, 282)
        Me.LegendSamplesGroupBox.TabIndex = 36
        Me.LegendSamplesGroupBox.TabStop = false
        Me.LegendSamplesGroupBox.Visible = false
        '
        'BarcodeErrorLabel
        '
        Me.BarcodeErrorLabel.BackColor = System.Drawing.Color.Transparent
        Me.BarcodeErrorLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BarcodeErrorLabel.ForeColor = System.Drawing.Color.Black
        Me.BarcodeErrorLabel.Location = New System.Drawing.Point(24, 215)
        Me.BarcodeErrorLabel.Name = "BarcodeErrorLabel"
        Me.BarcodeErrorLabel.Size = New System.Drawing.Size(148, 13)
        Me.BarcodeErrorLabel.TabIndex = 17
        Me.BarcodeErrorLabel.Text = "BarCode Error"
        Me.BarcodeErrorLabel.Title = false
        '
        'PendingLabel
        '
        Me.PendingLabel.BackColor = System.Drawing.Color.Transparent
        Me.PendingLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.PendingLabel.ForeColor = System.Drawing.Color.Black
        Me.PendingLabel.Location = New System.Drawing.Point(24, 128)
        Me.PendingLabel.Name = "PendingLabel"
        Me.PendingLabel.Size = New System.Drawing.Size(148, 13)
        Me.PendingLabel.TabIndex = 19
        Me.PendingLabel.Text = "Pending"
        Me.PendingLabel.Title = false
        '
        'InProgressLabel
        '
        Me.InProgressLabel.BackColor = System.Drawing.Color.Transparent
        Me.InProgressLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.InProgressLabel.ForeColor = System.Drawing.Color.Black
        Me.InProgressLabel.Location = New System.Drawing.Point(24, 157)
        Me.InProgressLabel.Name = "InProgressLabel"
        Me.InProgressLabel.Size = New System.Drawing.Size(148, 13)
        Me.InProgressLabel.TabIndex = 16
        Me.InProgressLabel.Text = "In Progress"
        Me.InProgressLabel.Title = false
        '
        'bsSamplesLegendLabel
        '
        Me.bsSamplesLegendLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSamplesLegendLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsSamplesLegendLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesLegendLabel.Location = New System.Drawing.Point(4, 12)
        Me.bsSamplesLegendLabel.Name = "bsSamplesLegendLabel"
        Me.bsSamplesLegendLabel.Size = New System.Drawing.Size(167, 19)
        Me.bsSamplesLegendLabel.TabIndex = 10
        Me.bsSamplesLegendLabel.Text = "Legend"
        Me.bsSamplesLegendLabel.Title = true
        '
        'FinishedLabel
        '
        Me.FinishedLabel.BackColor = System.Drawing.Color.Transparent
        Me.FinishedLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.FinishedLabel.ForeColor = System.Drawing.Color.Black
        Me.FinishedLabel.Location = New System.Drawing.Point(24, 185)
        Me.FinishedLabel.Name = "FinishedLabel"
        Me.FinishedLabel.Size = New System.Drawing.Size(148, 13)
        Me.FinishedLabel.TabIndex = 15
        Me.FinishedLabel.Text = "Finished"
        Me.FinishedLabel.Title = false
        '
        'LegendPendingImage
        '
        Me.LegendPendingImage.BackColor = System.Drawing.Color.Transparent
        Me.LegendPendingImage.InitialImage = CType(resources.GetObject("LegendPendingImage.InitialImage"),System.Drawing.Image)
        Me.LegendPendingImage.Location = New System.Drawing.Point(3, 125)
        Me.LegendPendingImage.Name = "LegendPendingImage"
        Me.LegendPendingImage.PositionNumber = 0
        Me.LegendPendingImage.Size = New System.Drawing.Size(20, 20)
        Me.LegendPendingImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.LegendPendingImage.TabIndex = 18
        Me.LegendPendingImage.TabStop = false
        '
        'LegendBarCodeErrorImage
        '
        Me.LegendBarCodeErrorImage.BackColor = System.Drawing.Color.Transparent
        Me.LegendBarCodeErrorImage.InitialImage = CType(resources.GetObject("LegendBarCodeErrorImage.InitialImage"),System.Drawing.Image)
        Me.LegendBarCodeErrorImage.Location = New System.Drawing.Point(3, 212)
        Me.LegendBarCodeErrorImage.Name = "LegendBarCodeErrorImage"
        Me.LegendBarCodeErrorImage.PositionNumber = 0
        Me.LegendBarCodeErrorImage.Size = New System.Drawing.Size(20, 20)
        Me.LegendBarCodeErrorImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.LegendBarCodeErrorImage.TabIndex = 11
        Me.LegendBarCodeErrorImage.TabStop = false
        '
        'LegendFinishedImage
        '
        Me.LegendFinishedImage.BackColor = System.Drawing.Color.Transparent
        Me.LegendFinishedImage.InitialImage = CType(resources.GetObject("LegendFinishedImage.InitialImage"),System.Drawing.Image)
        Me.LegendFinishedImage.Location = New System.Drawing.Point(3, 183)
        Me.LegendFinishedImage.Name = "LegendFinishedImage"
        Me.LegendFinishedImage.PositionNumber = 0
        Me.LegendFinishedImage.Size = New System.Drawing.Size(20, 20)
        Me.LegendFinishedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.LegendFinishedImage.TabIndex = 9
        Me.LegendFinishedImage.TabStop = false
        '
        'LegendDepletedImage
        '
        Me.LegendDepletedImage.BackColor = System.Drawing.Color.Transparent
        Me.LegendDepletedImage.InitialImage = CType(resources.GetObject("LegendDepletedImage.InitialImage"),System.Drawing.Image)
        Me.LegendDepletedImage.Location = New System.Drawing.Point(3, 96)
        Me.LegendDepletedImage.Name = "LegendDepletedImage"
        Me.LegendDepletedImage.PositionNumber = 0
        Me.LegendDepletedImage.Size = New System.Drawing.Size(20, 20)
        Me.LegendDepletedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.LegendDepletedImage.TabIndex = 5
        Me.LegendDepletedImage.TabStop = false
        '
        'LegendInProgressImage
        '
        Me.LegendInProgressImage.BackColor = System.Drawing.Color.Transparent
        Me.LegendInProgressImage.InitialImage = CType(resources.GetObject("LegendInProgressImage.InitialImage"),System.Drawing.Image)
        Me.LegendInProgressImage.Location = New System.Drawing.Point(3, 154)
        Me.LegendInProgressImage.Name = "LegendInProgressImage"
        Me.LegendInProgressImage.PositionNumber = 0
        Me.LegendInProgressImage.Size = New System.Drawing.Size(20, 20)
        Me.LegendInProgressImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.LegendInProgressImage.TabIndex = 8
        Me.LegendInProgressImage.TabStop = false
        '
        'LegendSelectedImage
        '
        Me.LegendSelectedImage.BackColor = System.Drawing.Color.Transparent
        Me.LegendSelectedImage.InitialImage = CType(resources.GetObject("LegendSelectedImage.InitialImage"),System.Drawing.Image)
        Me.LegendSelectedImage.Location = New System.Drawing.Point(3, 38)
        Me.LegendSelectedImage.Name = "LegendSelectedImage"
        Me.LegendSelectedImage.PositionNumber = 0
        Me.LegendSelectedImage.Size = New System.Drawing.Size(20, 20)
        Me.LegendSelectedImage.TabIndex = 6
        Me.LegendSelectedImage.TabStop = false
        '
        'LegendNotInUseImage
        '
        Me.LegendNotInUseImage.BackColor = System.Drawing.Color.Transparent
        Me.LegendNotInUseImage.InitialImage = CType(resources.GetObject("LegendNotInUseImage.InitialImage"),System.Drawing.Image)
        Me.LegendNotInUseImage.Location = New System.Drawing.Point(3, 67)
        Me.LegendNotInUseImage.Name = "LegendNotInUseImage"
        Me.LegendNotInUseImage.PositionNumber = 0
        Me.LegendNotInUseImage.Size = New System.Drawing.Size(20, 20)
        Me.LegendNotInUseImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.LegendNotInUseImage.TabIndex = 7
        Me.LegendNotInUseImage.TabStop = false
        '
        'DepletedLabel
        '
        Me.DepletedLabel.BackColor = System.Drawing.Color.Transparent
        Me.DepletedLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.DepletedLabel.ForeColor = System.Drawing.Color.Black
        Me.DepletedLabel.Location = New System.Drawing.Point(24, 99)
        Me.DepletedLabel.Name = "DepletedLabel"
        Me.DepletedLabel.Size = New System.Drawing.Size(148, 13)
        Me.DepletedLabel.TabIndex = 14
        Me.DepletedLabel.Text = "Terminado"
        Me.DepletedLabel.Title = false
        '
        'SelectedLabel
        '
        Me.SelectedLabel.BackColor = System.Drawing.Color.Transparent
        Me.SelectedLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.SelectedLabel.ForeColor = System.Drawing.Color.Black
        Me.SelectedLabel.Location = New System.Drawing.Point(24, 41)
        Me.SelectedLabel.Name = "SelectedLabel"
        Me.SelectedLabel.Size = New System.Drawing.Size(148, 13)
        Me.SelectedLabel.TabIndex = 12
        Me.SelectedLabel.Text = "Selected"
        Me.SelectedLabel.Title = false
        '
        'NotInUseLabel
        '
        Me.NotInUseLabel.BackColor = System.Drawing.Color.Transparent
        Me.NotInUseLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.NotInUseLabel.ForeColor = System.Drawing.Color.Black
        Me.NotInUseLabel.Location = New System.Drawing.Point(24, 70)
        Me.NotInUseLabel.Name = "NotInUseLabel"
        Me.NotInUseLabel.Size = New System.Drawing.Size(148, 13)
        Me.NotInUseLabel.TabIndex = 13
        Me.NotInUseLabel.Text = "Not In Use"
        Me.NotInUseLabel.Title = false
        '
        'BsGroupBox11
        '
        Me.BsGroupBox11.BackColor = System.Drawing.Color.Transparent
        Me.BsGroupBox11.Controls.Add(Me.bsSampleStatusTextBox)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesStatusLabel)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesMoveFirstPositionButton)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesCellLabel)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesPositionInfoLabel)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesDiskNameLabel)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesContentLabel)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesNumberLabel)
        Me.BsGroupBox11.Controls.Add(Me.bsSampleCellTextBox)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesDecreaseButton)
        Me.BsGroupBox11.Controls.Add(Me.bsSampleDiskNameTextBox)
        Me.BsGroupBox11.Controls.Add(Me.bsTubeSizeComboBox)
        Me.BsGroupBox11.Controls.Add(Me.bsSampleContentTextBox)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesIncreaseButton)
        Me.BsGroupBox11.Controls.Add(Me.bsTubeSizeLabel)
        Me.BsGroupBox11.Controls.Add(Me.bsSampleNumberTextBox)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesMoveLastPositionButton)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesBarcodeTextBox)
        Me.BsGroupBox11.Controls.Add(Me.bsSampleIDLabel)
        Me.BsGroupBox11.Controls.Add(Me.bsDiluteStatusTextBox)
        Me.BsGroupBox11.Controls.Add(Me.bsSampleTypeLabel)
        Me.BsGroupBox11.Controls.Add(Me.bsSampleTypeTextBox)
        Me.BsGroupBox11.Controls.Add(Me.bsDiluteStatusLabel)
        Me.BsGroupBox11.Controls.Add(Me.bsSampleIDTextBox)
        Me.BsGroupBox11.Controls.Add(Me.bsSamplesBarcodeLabel)
        Me.BsGroupBox11.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.BsGroupBox11.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox11.Location = New System.Drawing.Point(4, -3)
        Me.BsGroupBox11.Name = "BsGroupBox11"
        Me.BsGroupBox11.Size = New System.Drawing.Size(175, 335)
        Me.BsGroupBox11.TabIndex = 4
        Me.BsGroupBox11.TabStop = false
        '
        'bsSampleStatusTextBox
        '
        Me.bsSampleStatusTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleStatusTextBox.DecimalsValues = false
        Me.bsSampleStatusTextBox.Enabled = false
        Me.bsSampleStatusTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleStatusTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleStatusTextBox.IsNumeric = false
        Me.bsSampleStatusTextBox.Location = New System.Drawing.Point(5, 276)
        Me.bsSampleStatusTextBox.Mandatory = false
        Me.bsSampleStatusTextBox.Name = "bsSampleStatusTextBox"
        Me.bsSampleStatusTextBox.ReadOnly = true
        Me.bsSampleStatusTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsSampleStatusTextBox.TabIndex = 33
        Me.bsSampleStatusTextBox.TabStop = false
        '
        'bsSamplesStatusLabel
        '
        Me.bsSamplesStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesStatusLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesStatusLabel.Location = New System.Drawing.Point(2, 262)
        Me.bsSamplesStatusLabel.Name = "bsSamplesStatusLabel"
        Me.bsSamplesStatusLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsSamplesStatusLabel.TabIndex = 34
        Me.bsSamplesStatusLabel.Text = "Status"
        Me.bsSamplesStatusLabel.Title = false
        '
        'bsSamplesMoveFirstPositionButton
        '
        Me.bsSamplesMoveFirstPositionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSamplesMoveFirstPositionButton.Enabled = false
        Me.bsSamplesMoveFirstPositionButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsSamplesMoveFirstPositionButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsSamplesMoveFirstPositionButton.Location = New System.Drawing.Point(5, 301)
        Me.bsSamplesMoveFirstPositionButton.Name = "bsSamplesMoveFirstPositionButton"
        Me.bsSamplesMoveFirstPositionButton.Size = New System.Drawing.Size(26, 26)
        Me.bsSamplesMoveFirstPositionButton.TabIndex = 45
        Me.bsSamplesMoveFirstPositionButton.UseVisualStyleBackColor = true
        '
        'bsSamplesCellLabel
        '
        Me.bsSamplesCellLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesCellLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesCellLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesCellLabel.Location = New System.Drawing.Point(120, 33)
        Me.bsSamplesCellLabel.Name = "bsSamplesCellLabel"
        Me.bsSamplesCellLabel.Size = New System.Drawing.Size(52, 13)
        Me.bsSamplesCellLabel.TabIndex = 27
        Me.bsSamplesCellLabel.Text = "Cell"
        Me.bsSamplesCellLabel.Title = false
        '
        'bsSamplesPositionInfoLabel
        '
        Me.bsSamplesPositionInfoLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSamplesPositionInfoLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsSamplesPositionInfoLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesPositionInfoLabel.Location = New System.Drawing.Point(4, 12)
        Me.bsSamplesPositionInfoLabel.Name = "bsSamplesPositionInfoLabel"
        Me.bsSamplesPositionInfoLabel.Size = New System.Drawing.Size(167, 19)
        Me.bsSamplesPositionInfoLabel.TabIndex = 25
        Me.bsSamplesPositionInfoLabel.Text = "Position Information"
        Me.bsSamplesPositionInfoLabel.Title = true
        '
        'bsSamplesDiskNameLabel
        '
        Me.bsSamplesDiskNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesDiskNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesDiskNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesDiskNameLabel.Location = New System.Drawing.Point(2, 33)
        Me.bsSamplesDiskNameLabel.Name = "bsSamplesDiskNameLabel"
        Me.bsSamplesDiskNameLabel.Size = New System.Drawing.Size(119, 13)
        Me.bsSamplesDiskNameLabel.TabIndex = 28
        Me.bsSamplesDiskNameLabel.Text = "Disk Name/Num"
        Me.bsSamplesDiskNameLabel.Title = false
        '
        'bsSamplesContentLabel
        '
        Me.bsSamplesContentLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesContentLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesContentLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesContentLabel.Location = New System.Drawing.Point(2, 71)
        Me.bsSamplesContentLabel.Name = "bsSamplesContentLabel"
        Me.bsSamplesContentLabel.Size = New System.Drawing.Size(119, 13)
        Me.bsSamplesContentLabel.TabIndex = 30
        Me.bsSamplesContentLabel.Text = "Content"
        Me.bsSamplesContentLabel.Title = false
        '
        'bsSamplesNumberLabel
        '
        Me.bsSamplesNumberLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesNumberLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesNumberLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesNumberLabel.Location = New System.Drawing.Point(120, 71)
        Me.bsSamplesNumberLabel.Name = "bsSamplesNumberLabel"
        Me.bsSamplesNumberLabel.Size = New System.Drawing.Size(52, 13)
        Me.bsSamplesNumberLabel.TabIndex = 33
        Me.bsSamplesNumberLabel.Text = "Num"
        Me.bsSamplesNumberLabel.Title = false
        '
        'bsSampleCellTextBox
        '
        Me.bsSampleCellTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.bsSampleCellTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleCellTextBox.DecimalsValues = false
        Me.bsSampleCellTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleCellTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleCellTextBox.IsNumeric = false
        Me.bsSampleCellTextBox.Location = New System.Drawing.Point(123, 47)
        Me.bsSampleCellTextBox.Mandatory = false
        Me.bsSampleCellTextBox.MaxLength = 3
        Me.bsSampleCellTextBox.Name = "bsSampleCellTextBox"
        Me.bsSampleCellTextBox.ReadOnly = true
        Me.bsSampleCellTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsSampleCellTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsSampleCellTextBox.TabIndex = 29
        Me.bsSampleCellTextBox.TabStop = false
        Me.bsSampleCellTextBox.WordWrap = false
        '
        'bsSamplesDecreaseButton
        '
        Me.bsSamplesDecreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSamplesDecreaseButton.Enabled = false
        Me.bsSamplesDecreaseButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsSamplesDecreaseButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsSamplesDecreaseButton.Location = New System.Drawing.Point(30, 301)
        Me.bsSamplesDecreaseButton.Name = "bsSamplesDecreaseButton"
        Me.bsSamplesDecreaseButton.Size = New System.Drawing.Size(26, 26)
        Me.bsSamplesDecreaseButton.TabIndex = 46
        Me.bsSamplesDecreaseButton.UseVisualStyleBackColor = true
        '
        'bsSampleDiskNameTextBox
        '
        Me.bsSampleDiskNameTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.bsSampleDiskNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleDiskNameTextBox.DecimalsValues = false
        Me.bsSampleDiskNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleDiskNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleDiskNameTextBox.IsNumeric = false
        Me.bsSampleDiskNameTextBox.Location = New System.Drawing.Point(5, 47)
        Me.bsSampleDiskNameTextBox.Mandatory = false
        Me.bsSampleDiskNameTextBox.Name = "bsSampleDiskNameTextBox"
        Me.bsSampleDiskNameTextBox.ReadOnly = true
        Me.bsSampleDiskNameTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsSampleDiskNameTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsSampleDiskNameTextBox.TabIndex = 26
        Me.bsSampleDiskNameTextBox.TabStop = false
        Me.bsSampleDiskNameTextBox.WordWrap = false
        '
        'bsTubeSizeComboBox
        '
        Me.bsTubeSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsTubeSizeComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsTubeSizeComboBox.FormattingEnabled = true
        Me.bsTubeSizeComboBox.Location = New System.Drawing.Point(5, 237)
        Me.bsTubeSizeComboBox.MaxLength = 25
        Me.bsTubeSizeComboBox.Name = "bsTubeSizeComboBox"
        Me.bsTubeSizeComboBox.Size = New System.Drawing.Size(164, 21)
        Me.bsTubeSizeComboBox.TabIndex = 43
        '
        'bsSampleContentTextBox
        '
        Me.bsSampleContentTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleContentTextBox.DecimalsValues = false
        Me.bsSampleContentTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleContentTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleContentTextBox.IsNumeric = false
        Me.bsSampleContentTextBox.Location = New System.Drawing.Point(5, 85)
        Me.bsSampleContentTextBox.Mandatory = false
        Me.bsSampleContentTextBox.MaxLength = 20
        Me.bsSampleContentTextBox.Name = "bsSampleContentTextBox"
        Me.bsSampleContentTextBox.ReadOnly = true
        Me.bsSampleContentTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsSampleContentTextBox.TabIndex = 31
        Me.bsSampleContentTextBox.TabStop = false
        Me.bsSampleContentTextBox.WordWrap = false
        '
        'bsSamplesIncreaseButton
        '
        Me.bsSamplesIncreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSamplesIncreaseButton.Enabled = false
        Me.bsSamplesIncreaseButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsSamplesIncreaseButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsSamplesIncreaseButton.Location = New System.Drawing.Point(55, 301)
        Me.bsSamplesIncreaseButton.Name = "bsSamplesIncreaseButton"
        Me.bsSamplesIncreaseButton.Size = New System.Drawing.Size(26, 26)
        Me.bsSamplesIncreaseButton.TabIndex = 47
        Me.bsSamplesIncreaseButton.UseVisualStyleBackColor = true
        '
        'bsTubeSizeLabel
        '
        Me.bsTubeSizeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTubeSizeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTubeSizeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTubeSizeLabel.Location = New System.Drawing.Point(2, 223)
        Me.bsTubeSizeLabel.Name = "bsTubeSizeLabel"
        Me.bsTubeSizeLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsTubeSizeLabel.TabIndex = 44
        Me.bsTubeSizeLabel.Text = "Tube Size"
        Me.bsTubeSizeLabel.Title = false
        '
        'bsSampleNumberTextBox
        '
        Me.bsSampleNumberTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.bsSampleNumberTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleNumberTextBox.DecimalsValues = false
        Me.bsSampleNumberTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleNumberTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleNumberTextBox.IsNumeric = false
        Me.bsSampleNumberTextBox.Location = New System.Drawing.Point(123, 85)
        Me.bsSampleNumberTextBox.Mandatory = false
        Me.bsSampleNumberTextBox.Name = "bsSampleNumberTextBox"
        Me.bsSampleNumberTextBox.ReadOnly = true
        Me.bsSampleNumberTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsSampleNumberTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsSampleNumberTextBox.TabIndex = 32
        Me.bsSampleNumberTextBox.TabStop = false
        Me.bsSampleNumberTextBox.WordWrap = false
        '
        'bsSamplesMoveLastPositionButton
        '
        Me.bsSamplesMoveLastPositionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSamplesMoveLastPositionButton.Enabled = false
        Me.bsSamplesMoveLastPositionButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsSamplesMoveLastPositionButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsSamplesMoveLastPositionButton.Location = New System.Drawing.Point(80, 301)
        Me.bsSamplesMoveLastPositionButton.Name = "bsSamplesMoveLastPositionButton"
        Me.bsSamplesMoveLastPositionButton.Size = New System.Drawing.Size(26, 26)
        Me.bsSamplesMoveLastPositionButton.TabIndex = 48
        Me.bsSamplesMoveLastPositionButton.UseVisualStyleBackColor = true
        '
        'bsSamplesBarcodeTextBox
        '
        Me.bsSamplesBarcodeTextBox.BackColor = System.Drawing.Color.White
        Me.bsSamplesBarcodeTextBox.DecimalsValues = false
        Me.bsSamplesBarcodeTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesBarcodeTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesBarcodeTextBox.IsNumeric = false
        Me.bsSamplesBarcodeTextBox.Location = New System.Drawing.Point(5, 199)
        Me.bsSamplesBarcodeTextBox.Mandatory = false
        Me.bsSamplesBarcodeTextBox.Name = "bsSamplesBarcodeTextBox"
        Me.bsSamplesBarcodeTextBox.ReadOnly = true
        Me.bsSamplesBarcodeTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsSamplesBarcodeTextBox.TabIndex = 41
        Me.bsSamplesBarcodeTextBox.WordWrap = false
        '
        'bsSampleIDLabel
        '
        Me.bsSampleIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleIDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleIDLabel.Location = New System.Drawing.Point(2, 109)
        Me.bsSampleIDLabel.Name = "bsSampleIDLabel"
        Me.bsSampleIDLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsSampleIDLabel.TabIndex = 35
        Me.bsSampleIDLabel.Text = "Sample ID"
        Me.bsSampleIDLabel.Title = false
        '
        'bsDiluteStatusTextBox
        '
        Me.bsDiluteStatusTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.bsDiluteStatusTextBox.BackColor = System.Drawing.Color.White
        Me.bsDiluteStatusTextBox.DecimalsValues = false
        Me.bsDiluteStatusTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDiluteStatusTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsDiluteStatusTextBox.IsNumeric = false
        Me.bsDiluteStatusTextBox.Location = New System.Drawing.Point(123, 161)
        Me.bsDiluteStatusTextBox.Mandatory = false
        Me.bsDiluteStatusTextBox.Name = "bsDiluteStatusTextBox"
        Me.bsDiluteStatusTextBox.ReadOnly = true
        Me.bsDiluteStatusTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsDiluteStatusTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsDiluteStatusTextBox.TabIndex = 40
        Me.bsDiluteStatusTextBox.TabStop = false
        Me.bsDiluteStatusTextBox.WordWrap = false
        '
        'bsSampleTypeLabel
        '
        Me.bsSampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeLabel.Location = New System.Drawing.Point(2, 147)
        Me.bsSampleTypeLabel.Name = "bsSampleTypeLabel"
        Me.bsSampleTypeLabel.Size = New System.Drawing.Size(115, 13)
        Me.bsSampleTypeLabel.TabIndex = 37
        Me.bsSampleTypeLabel.Text = "Sample Type"
        Me.bsSampleTypeLabel.Title = false
        '
        'bsSampleTypeTextBox
        '
        Me.bsSampleTypeTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleTypeTextBox.DecimalsValues = false
        Me.bsSampleTypeTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypeTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeTextBox.IsNumeric = false
        Me.bsSampleTypeTextBox.Location = New System.Drawing.Point(5, 161)
        Me.bsSampleTypeTextBox.Mandatory = false
        Me.bsSampleTypeTextBox.MaxLength = 20
        Me.bsSampleTypeTextBox.Name = "bsSampleTypeTextBox"
        Me.bsSampleTypeTextBox.ReadOnly = true
        Me.bsSampleTypeTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsSampleTypeTextBox.TabIndex = 38
        Me.bsSampleTypeTextBox.TabStop = false
        Me.bsSampleTypeTextBox.WordWrap = false
        '
        'bsDiluteStatusLabel
        '
        Me.bsDiluteStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDiluteStatusLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDiluteStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDiluteStatusLabel.Location = New System.Drawing.Point(120, 147)
        Me.bsDiluteStatusLabel.Name = "bsDiluteStatusLabel"
        Me.bsDiluteStatusLabel.Size = New System.Drawing.Size(54, 13)
        Me.bsDiluteStatusLabel.TabIndex = 39
        Me.bsDiluteStatusLabel.Text = "Diluted"
        Me.bsDiluteStatusLabel.Title = false
        '
        'bsSampleIDTextBox
        '
        Me.bsSampleIDTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleIDTextBox.DecimalsValues = false
        Me.bsSampleIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleIDTextBox.IsNumeric = false
        Me.bsSampleIDTextBox.Location = New System.Drawing.Point(5, 123)
        Me.bsSampleIDTextBox.Mandatory = false
        Me.bsSampleIDTextBox.Name = "bsSampleIDTextBox"
        Me.bsSampleIDTextBox.ReadOnly = true
        Me.bsSampleIDTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsSampleIDTextBox.TabIndex = 36
        Me.bsSampleIDTextBox.TabStop = false
        Me.bsSampleIDTextBox.WordWrap = false
        '
        'bsSamplesBarcodeLabel
        '
        Me.bsSamplesBarcodeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesBarcodeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesBarcodeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesBarcodeLabel.Location = New System.Drawing.Point(2, 185)
        Me.bsSamplesBarcodeLabel.Name = "bsSamplesBarcodeLabel"
        Me.bsSamplesBarcodeLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsSamplesBarcodeLabel.TabIndex = 42
        Me.bsSamplesBarcodeLabel.Text = "Sample Barcode"
        Me.bsSamplesBarcodeLabel.Title = false
        '
        'Sam3106
        '
        Me.Sam3106.BackColor = System.Drawing.Color.Transparent
        Me.Sam3106.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3106.Image = Nothing
        Me.Sam3106.ImagePath = ""
        Me.Sam3106.IsTransparentImage = false
        Me.Sam3106.Location = New System.Drawing.Point(438, 169)
        Me.Sam3106.Name = "Sam3106"
        Me.Sam3106.Rotation = 0
        Me.Sam3106.ShowThrough = false
        Me.Sam3106.Size = New System.Drawing.Size(24, 23)
        Me.Sam3106.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3106.TabIndex = 289
        Me.Sam3106.TabStop = false
        Me.Sam3106.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3128
        '
        Me.Sam3128.BackColor = System.Drawing.Color.Transparent
        Me.Sam3128.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3128.Image = Nothing
        Me.Sam3128.ImagePath = ""
        Me.Sam3128.IsTransparentImage = true
        Me.Sam3128.Location = New System.Drawing.Point(109, 387)
        Me.Sam3128.Name = "Sam3128"
        Me.Sam3128.Rotation = 0
        Me.Sam3128.ShowThrough = false
        Me.Sam3128.Size = New System.Drawing.Size(24, 23)
        Me.Sam3128.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3128.TabIndex = 311
        Me.Sam3128.TabStop = false
        Me.Sam3128.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3129
        '
        Me.Sam3129.BackColor = System.Drawing.Color.Transparent
        Me.Sam3129.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3129.Image = Nothing
        Me.Sam3129.ImagePath = ""
        Me.Sam3129.IsTransparentImage = true
        Me.Sam3129.Location = New System.Drawing.Point(125, 410)
        Me.Sam3129.Name = "Sam3129"
        Me.Sam3129.Rotation = 0
        Me.Sam3129.ShowThrough = false
        Me.Sam3129.Size = New System.Drawing.Size(24, 23)
        Me.Sam3129.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3129.TabIndex = 312
        Me.Sam3129.TabStop = false
        Me.Sam3129.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam395
        '
        Me.Sam395.BackColor = System.Drawing.Color.Transparent
        Me.Sam395.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam395.Image = Nothing
        Me.Sam395.ImagePath = ""
        Me.Sam395.IsTransparentImage = true
        Me.Sam395.Location = New System.Drawing.Point(396, 443)
        Me.Sam395.Name = "Sam395"
        Me.Sam395.Rotation = 0
        Me.Sam395.ShowThrough = false
        Me.Sam395.Size = New System.Drawing.Size(24, 24)
        Me.Sam395.TabIndex = 277
        Me.Sam395.TabStop = false
        Me.Sam395.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam396
        '
        Me.Sam396.BackColor = System.Drawing.Color.Transparent
        Me.Sam396.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam396.Image = Nothing
        Me.Sam396.ImagePath = ""
        Me.Sam396.IsTransparentImage = false
        Me.Sam396.Location = New System.Drawing.Point(417, 424)
        Me.Sam396.Name = "Sam396"
        Me.Sam396.Rotation = 0
        Me.Sam396.ShowThrough = false
        Me.Sam396.Size = New System.Drawing.Size(24, 24)
        Me.Sam396.TabIndex = 278
        Me.Sam396.TabStop = false
        Me.Sam396.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam397
        '
        Me.Sam397.BackColor = System.Drawing.Color.Transparent
        Me.Sam397.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam397.Image = Nothing
        Me.Sam397.ImagePath = ""
        Me.Sam397.IsTransparentImage = false
        Me.Sam397.Location = New System.Drawing.Point(435, 403)
        Me.Sam397.Name = "Sam397"
        Me.Sam397.Rotation = 0
        Me.Sam397.ShowThrough = false
        Me.Sam397.Size = New System.Drawing.Size(24, 23)
        Me.Sam397.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam397.TabIndex = 279
        Me.Sam397.TabStop = false
        Me.Sam397.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3107
        '
        Me.Sam3107.BackColor = System.Drawing.Color.Transparent
        Me.Sam3107.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3107.Image = Nothing
        Me.Sam3107.ImagePath = ""
        Me.Sam3107.IsTransparentImage = true
        Me.Sam3107.Location = New System.Drawing.Point(421, 147)
        Me.Sam3107.Name = "Sam3107"
        Me.Sam3107.Rotation = 0
        Me.Sam3107.ShowThrough = false
        Me.Sam3107.Size = New System.Drawing.Size(24, 23)
        Me.Sam3107.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3107.TabIndex = 290
        Me.Sam3107.TabStop = false
        Me.Sam3107.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3134
        '
        Me.Sam3134.BackColor = System.Drawing.Color.Transparent
        Me.Sam3134.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3134.Image = Nothing
        Me.Sam3134.ImagePath = ""
        Me.Sam3134.IsTransparentImage = true
        Me.Sam3134.Location = New System.Drawing.Point(240, 480)
        Me.Sam3134.Name = "Sam3134"
        Me.Sam3134.Rotation = 0
        Me.Sam3134.ShowThrough = false
        Me.Sam3134.Size = New System.Drawing.Size(24, 24)
        Me.Sam3134.TabIndex = 317
        Me.Sam3134.TabStop = false
        Me.Sam3134.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3133
        '
        Me.Sam3133.BackColor = System.Drawing.Color.Transparent
        Me.Sam3133.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3133.Image = Nothing
        Me.Sam3133.ImagePath = ""
        Me.Sam3133.IsTransparentImage = true
        Me.Sam3133.Location = New System.Drawing.Point(214, 472)
        Me.Sam3133.Name = "Sam3133"
        Me.Sam3133.Rotation = 0
        Me.Sam3133.ShowThrough = false
        Me.Sam3133.Size = New System.Drawing.Size(24, 24)
        Me.Sam3133.TabIndex = 316
        Me.Sam3133.TabStop = false
        Me.Sam3133.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3132
        '
        Me.Sam3132.BackColor = System.Drawing.Color.Transparent
        Me.Sam3132.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3132.Image = Nothing
        Me.Sam3132.ImagePath = ""
        Me.Sam3132.IsTransparentImage = false
        Me.Sam3132.Location = New System.Drawing.Point(189, 462)
        Me.Sam3132.Name = "Sam3132"
        Me.Sam3132.Rotation = 0
        Me.Sam3132.ShowThrough = false
        Me.Sam3132.Size = New System.Drawing.Size(24, 23)
        Me.Sam3132.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3132.TabIndex = 315
        Me.Sam3132.TabStop = false
        Me.Sam3132.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3131
        '
        Me.Sam3131.BackColor = System.Drawing.Color.Transparent
        Me.Sam3131.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3131.Image = Nothing
        Me.Sam3131.ImagePath = ""
        Me.Sam3131.IsTransparentImage = false
        Me.Sam3131.Location = New System.Drawing.Point(165, 448)
        Me.Sam3131.Name = "Sam3131"
        Me.Sam3131.Rotation = 0
        Me.Sam3131.ShowThrough = false
        Me.Sam3131.Size = New System.Drawing.Size(24, 23)
        Me.Sam3131.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3131.TabIndex = 314
        Me.Sam3131.TabStop = false
        Me.Sam3131.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3127
        '
        Me.Sam3127.BackColor = System.Drawing.Color.Transparent
        Me.Sam3127.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3127.Image = Nothing
        Me.Sam3127.ImagePath = ""
        Me.Sam3127.IsTransparentImage = true
        Me.Sam3127.Location = New System.Drawing.Point(97, 362)
        Me.Sam3127.Name = "Sam3127"
        Me.Sam3127.Rotation = 0
        Me.Sam3127.ShowThrough = false
        Me.Sam3127.Size = New System.Drawing.Size(24, 23)
        Me.Sam3127.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3127.TabIndex = 310
        Me.Sam3127.TabStop = false
        Me.Sam3127.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3126
        '
        Me.Sam3126.BackColor = System.Drawing.Color.Transparent
        Me.Sam3126.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3126.Image = Nothing
        Me.Sam3126.ImagePath = ""
        Me.Sam3126.IsTransparentImage = false
        Me.Sam3126.Location = New System.Drawing.Point(88, 335)
        Me.Sam3126.Name = "Sam3126"
        Me.Sam3126.Rotation = 0
        Me.Sam3126.ShowThrough = false
        Me.Sam3126.Size = New System.Drawing.Size(24, 24)
        Me.Sam3126.TabIndex = 309
        Me.Sam3126.TabStop = false
        Me.Sam3126.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3125
        '
        Me.Sam3125.BackColor = System.Drawing.Color.Transparent
        Me.Sam3125.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3125.Image = Nothing
        Me.Sam3125.ImagePath = ""
        Me.Sam3125.IsTransparentImage = false
        Me.Sam3125.Location = New System.Drawing.Point(83, 308)
        Me.Sam3125.Name = "Sam3125"
        Me.Sam3125.Rotation = 0
        Me.Sam3125.ShowThrough = false
        Me.Sam3125.Size = New System.Drawing.Size(24, 24)
        Me.Sam3125.TabIndex = 308
        Me.Sam3125.TabStop = false
        Me.Sam3125.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3124
        '
        Me.Sam3124.BackColor = System.Drawing.Color.Transparent
        Me.Sam3124.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3124.Image = Nothing
        Me.Sam3124.ImagePath = ""
        Me.Sam3124.IsTransparentImage = false
        Me.Sam3124.Location = New System.Drawing.Point(81, 280)
        Me.Sam3124.Name = "Sam3124"
        Me.Sam3124.Rotation = 0
        Me.Sam3124.ShowThrough = false
        Me.Sam3124.Size = New System.Drawing.Size(24, 24)
        Me.Sam3124.TabIndex = 307
        Me.Sam3124.TabStop = false
        Me.Sam3124.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3123
        '
        Me.Sam3123.BackColor = System.Drawing.Color.Transparent
        Me.Sam3123.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3123.Image = Nothing
        Me.Sam3123.ImagePath = ""
        Me.Sam3123.IsTransparentImage = false
        Me.Sam3123.Location = New System.Drawing.Point(83, 252)
        Me.Sam3123.Name = "Sam3123"
        Me.Sam3123.Rotation = 0
        Me.Sam3123.ShowThrough = false
        Me.Sam3123.Size = New System.Drawing.Size(24, 24)
        Me.Sam3123.TabIndex = 306
        Me.Sam3123.TabStop = false
        Me.Sam3123.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3122
        '
        Me.Sam3122.BackColor = System.Drawing.Color.Transparent
        Me.Sam3122.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3122.Image = Nothing
        Me.Sam3122.ImagePath = ""
        Me.Sam3122.IsTransparentImage = false
        Me.Sam3122.Location = New System.Drawing.Point(89, 225)
        Me.Sam3122.Name = "Sam3122"
        Me.Sam3122.Rotation = 0
        Me.Sam3122.ShowThrough = false
        Me.Sam3122.Size = New System.Drawing.Size(24, 24)
        Me.Sam3122.TabIndex = 305
        Me.Sam3122.TabStop = false
        Me.Sam3122.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3121
        '
        Me.Sam3121.BackColor = System.Drawing.Color.Transparent
        Me.Sam3121.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3121.Image = Nothing
        Me.Sam3121.ImagePath = ""
        Me.Sam3121.IsTransparentImage = false
        Me.Sam3121.Location = New System.Drawing.Point(99, 199)
        Me.Sam3121.Name = "Sam3121"
        Me.Sam3121.Rotation = 0
        Me.Sam3121.ShowThrough = false
        Me.Sam3121.Size = New System.Drawing.Size(24, 24)
        Me.Sam3121.TabIndex = 304
        Me.Sam3121.TabStop = false
        Me.Sam3121.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3120
        '
        Me.Sam3120.BackColor = System.Drawing.Color.Transparent
        Me.Sam3120.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3120.Image = Nothing
        Me.Sam3120.ImagePath = ""
        Me.Sam3120.IsTransparentImage = false
        Me.Sam3120.Location = New System.Drawing.Point(113, 175)
        Me.Sam3120.Name = "Sam3120"
        Me.Sam3120.Rotation = 0
        Me.Sam3120.ShowThrough = false
        Me.Sam3120.Size = New System.Drawing.Size(24, 24)
        Me.Sam3120.TabIndex = 303
        Me.Sam3120.TabStop = false
        Me.Sam3120.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3119
        '
        Me.Sam3119.BackColor = System.Drawing.Color.Transparent
        Me.Sam3119.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3119.Image = Nothing
        Me.Sam3119.ImagePath = ""
        Me.Sam3119.IsTransparentImage = false
        Me.Sam3119.Location = New System.Drawing.Point(129, 153)
        Me.Sam3119.Name = "Sam3119"
        Me.Sam3119.Rotation = 0
        Me.Sam3119.ShowThrough = false
        Me.Sam3119.Size = New System.Drawing.Size(24, 23)
        Me.Sam3119.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3119.TabIndex = 302
        Me.Sam3119.TabStop = false
        Me.Sam3119.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3118
        '
        Me.Sam3118.BackColor = System.Drawing.Color.Transparent
        Me.Sam3118.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3118.Image = Nothing
        Me.Sam3118.ImagePath = ""
        Me.Sam3118.IsTransparentImage = false
        Me.Sam3118.Location = New System.Drawing.Point(149, 133)
        Me.Sam3118.Name = "Sam3118"
        Me.Sam3118.Rotation = 0
        Me.Sam3118.ShowThrough = false
        Me.Sam3118.Size = New System.Drawing.Size(24, 23)
        Me.Sam3118.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3118.TabIndex = 301
        Me.Sam3118.TabStop = false
        Me.Sam3118.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3117
        '
        Me.Sam3117.BackColor = System.Drawing.Color.Transparent
        Me.Sam3117.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3117.Image = Nothing
        Me.Sam3117.ImagePath = ""
        Me.Sam3117.IsTransparentImage = false
        Me.Sam3117.Location = New System.Drawing.Point(171, 116)
        Me.Sam3117.Name = "Sam3117"
        Me.Sam3117.Rotation = 0
        Me.Sam3117.ShowThrough = false
        Me.Sam3117.Size = New System.Drawing.Size(24, 23)
        Me.Sam3117.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3117.TabIndex = 300
        Me.Sam3117.TabStop = false
        Me.Sam3117.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3116
        '
        Me.Sam3116.BackColor = System.Drawing.Color.Transparent
        Me.Sam3116.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3116.Image = Nothing
        Me.Sam3116.ImagePath = ""
        Me.Sam3116.IsTransparentImage = false
        Me.Sam3116.Location = New System.Drawing.Point(195, 102)
        Me.Sam3116.Name = "Sam3116"
        Me.Sam3116.Rotation = 0
        Me.Sam3116.ShowThrough = false
        Me.Sam3116.Size = New System.Drawing.Size(24, 23)
        Me.Sam3116.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3116.TabIndex = 299
        Me.Sam3116.TabStop = false
        Me.Sam3116.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3115
        '
        Me.Sam3115.BackColor = System.Drawing.Color.Transparent
        Me.Sam3115.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3115.Image = Nothing
        Me.Sam3115.ImagePath = ""
        Me.Sam3115.IsTransparentImage = false
        Me.Sam3115.Location = New System.Drawing.Point(220, 92)
        Me.Sam3115.Name = "Sam3115"
        Me.Sam3115.Rotation = 0
        Me.Sam3115.ShowThrough = false
        Me.Sam3115.Size = New System.Drawing.Size(24, 24)
        Me.Sam3115.TabIndex = 298
        Me.Sam3115.TabStop = false
        Me.Sam3115.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3114
        '
        Me.Sam3114.BackColor = System.Drawing.Color.Transparent
        Me.Sam3114.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3114.Image = Nothing
        Me.Sam3114.ImagePath = ""
        Me.Sam3114.IsTransparentImage = false
        Me.Sam3114.Location = New System.Drawing.Point(246, 85)
        Me.Sam3114.Name = "Sam3114"
        Me.Sam3114.Rotation = 0
        Me.Sam3114.ShowThrough = false
        Me.Sam3114.Size = New System.Drawing.Size(24, 24)
        Me.Sam3114.TabIndex = 297
        Me.Sam3114.TabStop = false
        Me.Sam3114.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3113
        '
        Me.Sam3113.BackColor = System.Drawing.Color.Transparent
        Me.Sam3113.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3113.Image = Nothing
        Me.Sam3113.ImagePath = ""
        Me.Sam3113.IsTransparentImage = false
        Me.Sam3113.Location = New System.Drawing.Point(274, 83)
        Me.Sam3113.Name = "Sam3113"
        Me.Sam3113.Rotation = 0
        Me.Sam3113.ShowThrough = false
        Me.Sam3113.Size = New System.Drawing.Size(24, 24)
        Me.Sam3113.TabIndex = 296
        Me.Sam3113.TabStop = false
        Me.Sam3113.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3112
        '
        Me.Sam3112.BackColor = System.Drawing.Color.Transparent
        Me.Sam3112.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3112.Image = Nothing
        Me.Sam3112.ImagePath = ""
        Me.Sam3112.IsTransparentImage = false
        Me.Sam3112.Location = New System.Drawing.Point(301, 84)
        Me.Sam3112.Name = "Sam3112"
        Me.Sam3112.Rotation = 0
        Me.Sam3112.ShowThrough = false
        Me.Sam3112.Size = New System.Drawing.Size(24, 24)
        Me.Sam3112.TabIndex = 295
        Me.Sam3112.TabStop = false
        Me.Sam3112.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3111
        '
        Me.Sam3111.BackColor = System.Drawing.Color.Transparent
        Me.Sam3111.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3111.Image = Nothing
        Me.Sam3111.ImagePath = ""
        Me.Sam3111.IsTransparentImage = false
        Me.Sam3111.Location = New System.Drawing.Point(328, 89)
        Me.Sam3111.Name = "Sam3111"
        Me.Sam3111.Rotation = 0
        Me.Sam3111.ShowThrough = false
        Me.Sam3111.Size = New System.Drawing.Size(24, 24)
        Me.Sam3111.TabIndex = 294
        Me.Sam3111.TabStop = false
        Me.Sam3111.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3110
        '
        Me.Sam3110.BackColor = System.Drawing.Color.Transparent
        Me.Sam3110.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3110.Image = Nothing
        Me.Sam3110.ImagePath = ""
        Me.Sam3110.IsTransparentImage = false
        Me.Sam3110.Location = New System.Drawing.Point(354, 98)
        Me.Sam3110.Name = "Sam3110"
        Me.Sam3110.Rotation = 0
        Me.Sam3110.ShowThrough = false
        Me.Sam3110.Size = New System.Drawing.Size(24, 24)
        Me.Sam3110.TabIndex = 293
        Me.Sam3110.TabStop = false
        Me.Sam3110.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3109
        '
        Me.Sam3109.BackColor = System.Drawing.Color.Transparent
        Me.Sam3109.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3109.Image = Nothing
        Me.Sam3109.ImagePath = ""
        Me.Sam3109.IsTransparentImage = false
        Me.Sam3109.Location = New System.Drawing.Point(378, 111)
        Me.Sam3109.Name = "Sam3109"
        Me.Sam3109.Rotation = 0
        Me.Sam3109.ShowThrough = false
        Me.Sam3109.Size = New System.Drawing.Size(24, 24)
        Me.Sam3109.TabIndex = 292
        Me.Sam3109.TabStop = false
        Me.Sam3109.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3108
        '
        Me.Sam3108.BackColor = System.Drawing.Color.Transparent
        Me.Sam3108.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3108.Image = Nothing
        Me.Sam3108.ImagePath = ""
        Me.Sam3108.IsTransparentImage = false
        Me.Sam3108.Location = New System.Drawing.Point(401, 127)
        Me.Sam3108.Name = "Sam3108"
        Me.Sam3108.Rotation = 0
        Me.Sam3108.ShowThrough = false
        Me.Sam3108.Size = New System.Drawing.Size(24, 24)
        Me.Sam3108.TabIndex = 291
        Me.Sam3108.TabStop = false
        Me.Sam3108.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3105
        '
        Me.Sam3105.BackColor = System.Drawing.Color.Transparent
        Me.Sam3105.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3105.Image = Nothing
        Me.Sam3105.ImagePath = ""
        Me.Sam3105.IsTransparentImage = false
        Me.Sam3105.Location = New System.Drawing.Point(453, 192)
        Me.Sam3105.Name = "Sam3105"
        Me.Sam3105.Rotation = 0
        Me.Sam3105.ShowThrough = false
        Me.Sam3105.Size = New System.Drawing.Size(24, 24)
        Me.Sam3105.TabIndex = 288
        Me.Sam3105.TabStop = false
        Me.Sam3105.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3104
        '
        Me.Sam3104.BackColor = System.Drawing.Color.Transparent
        Me.Sam3104.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3104.Image = Nothing
        Me.Sam3104.ImagePath = ""
        Me.Sam3104.IsTransparentImage = false
        Me.Sam3104.Location = New System.Drawing.Point(463, 217)
        Me.Sam3104.Name = "Sam3104"
        Me.Sam3104.Rotation = 0
        Me.Sam3104.ShowThrough = false
        Me.Sam3104.Size = New System.Drawing.Size(24, 24)
        Me.Sam3104.TabIndex = 287
        Me.Sam3104.TabStop = false
        Me.Sam3104.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3103
        '
        Me.Sam3103.BackColor = System.Drawing.Color.Transparent
        Me.Sam3103.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3103.Image = Nothing
        Me.Sam3103.ImagePath = ""
        Me.Sam3103.IsTransparentImage = false
        Me.Sam3103.Location = New System.Drawing.Point(471, 244)
        Me.Sam3103.Name = "Sam3103"
        Me.Sam3103.Rotation = 0
        Me.Sam3103.ShowThrough = false
        Me.Sam3103.Size = New System.Drawing.Size(24, 24)
        Me.Sam3103.TabIndex = 286
        Me.Sam3103.TabStop = false
        Me.Sam3103.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3102
        '
        Me.Sam3102.BackColor = System.Drawing.Color.Transparent
        Me.Sam3102.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3102.Image = Nothing
        Me.Sam3102.ImagePath = ""
        Me.Sam3102.IsTransparentImage = false
        Me.Sam3102.Location = New System.Drawing.Point(474, 272)
        Me.Sam3102.Name = "Sam3102"
        Me.Sam3102.Rotation = 0
        Me.Sam3102.ShowThrough = false
        Me.Sam3102.Size = New System.Drawing.Size(24, 24)
        Me.Sam3102.TabIndex = 285
        Me.Sam3102.TabStop = false
        Me.Sam3102.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3101
        '
        Me.Sam3101.BackColor = System.Drawing.Color.Transparent
        Me.Sam3101.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3101.Image = Nothing
        Me.Sam3101.ImagePath = ""
        Me.Sam3101.IsTransparentImage = false
        Me.Sam3101.Location = New System.Drawing.Point(473, 300)
        Me.Sam3101.Name = "Sam3101"
        Me.Sam3101.Rotation = 0
        Me.Sam3101.ShowThrough = false
        Me.Sam3101.Size = New System.Drawing.Size(24, 24)
        Me.Sam3101.TabIndex = 284
        Me.Sam3101.TabStop = false
        Me.Sam3101.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3100
        '
        Me.Sam3100.BackColor = System.Drawing.Color.Transparent
        Me.Sam3100.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3100.Image = Nothing
        Me.Sam3100.ImagePath = ""
        Me.Sam3100.IsTransparentImage = false
        Me.Sam3100.Location = New System.Drawing.Point(469, 328)
        Me.Sam3100.Name = "Sam3100"
        Me.Sam3100.Rotation = 0
        Me.Sam3100.ShowThrough = false
        Me.Sam3100.Size = New System.Drawing.Size(24, 24)
        Me.Sam3100.TabIndex = 283
        Me.Sam3100.TabStop = false
        Me.Sam3100.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3135
        '
        Me.Sam3135.BackColor = System.Drawing.Color.Transparent
        Me.Sam3135.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3135.Image = Nothing
        Me.Sam3135.ImagePath = ""
        Me.Sam3135.IsTransparentImage = true
        Me.Sam3135.Location = New System.Drawing.Point(268, 483)
        Me.Sam3135.Name = "Sam3135"
        Me.Sam3135.Rotation = 0
        Me.Sam3135.ShowThrough = false
        Me.Sam3135.Size = New System.Drawing.Size(24, 24)
        Me.Sam3135.TabIndex = 282
        Me.Sam3135.TabStop = false
        Me.Sam3135.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam399
        '
        Me.Sam399.BackColor = System.Drawing.Color.Transparent
        Me.Sam399.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam399.Image = Nothing
        Me.Sam399.ImagePath = ""
        Me.Sam399.IsTransparentImage = false
        Me.Sam399.Location = New System.Drawing.Point(461, 354)
        Me.Sam399.Name = "Sam399"
        Me.Sam399.Rotation = 0
        Me.Sam399.ShowThrough = false
        Me.Sam399.Size = New System.Drawing.Size(24, 24)
        Me.Sam399.TabIndex = 281
        Me.Sam399.TabStop = false
        Me.Sam399.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam398
        '
        Me.Sam398.BackColor = System.Drawing.Color.Transparent
        Me.Sam398.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam398.Image = Nothing
        Me.Sam398.ImagePath = ""
        Me.Sam398.IsTransparentImage = false
        Me.Sam398.Location = New System.Drawing.Point(450, 379)
        Me.Sam398.Name = "Sam398"
        Me.Sam398.Rotation = 0
        Me.Sam398.ShowThrough = false
        Me.Sam398.Size = New System.Drawing.Size(24, 24)
        Me.Sam398.TabIndex = 280
        Me.Sam398.TabStop = false
        Me.Sam398.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam394
        '
        Me.Sam394.BackColor = System.Drawing.Color.Transparent
        Me.Sam394.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam394.Image = Nothing
        Me.Sam394.ImagePath = ""
        Me.Sam394.IsTransparentImage = false
        Me.Sam394.Location = New System.Drawing.Point(373, 458)
        Me.Sam394.Name = "Sam394"
        Me.Sam394.Rotation = 0
        Me.Sam394.ShowThrough = false
        Me.Sam394.Size = New System.Drawing.Size(24, 24)
        Me.Sam394.TabIndex = 276
        Me.Sam394.TabStop = false
        Me.Sam394.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam393
        '
        Me.Sam393.BackColor = System.Drawing.Color.Transparent
        Me.Sam393.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam393.Image = Nothing
        Me.Sam393.ImagePath = ""
        Me.Sam393.IsTransparentImage = false
        Me.Sam393.Location = New System.Drawing.Point(349, 470)
        Me.Sam393.Name = "Sam393"
        Me.Sam393.Rotation = 0
        Me.Sam393.ShowThrough = false
        Me.Sam393.Size = New System.Drawing.Size(24, 23)
        Me.Sam393.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam393.TabIndex = 275
        Me.Sam393.TabStop = false
        Me.Sam393.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam392
        '
        Me.Sam392.BackColor = System.Drawing.Color.Transparent
        Me.Sam392.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam392.Image = Nothing
        Me.Sam392.ImagePath = ""
        Me.Sam392.IsTransparentImage = false
        Me.Sam392.Location = New System.Drawing.Point(322, 478)
        Me.Sam392.Name = "Sam392"
        Me.Sam392.Rotation = 0
        Me.Sam392.ShowThrough = false
        Me.Sam392.Size = New System.Drawing.Size(24, 24)
        Me.Sam392.TabIndex = 274
        Me.Sam392.TabStop = false
        Me.Sam392.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam391
        '
        Me.Sam391.BackColor = System.Drawing.Color.Transparent
        Me.Sam391.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam391.Image = Nothing
        Me.Sam391.ImagePath = ""
        Me.Sam391.IsTransparentImage = false
        Me.Sam391.Location = New System.Drawing.Point(295, 482)
        Me.Sam391.Name = "Sam391"
        Me.Sam391.Rotation = 0
        Me.Sam391.ShowThrough = false
        Me.Sam391.Size = New System.Drawing.Size(24, 24)
        Me.Sam391.TabIndex = 273
        Me.Sam391.TabStop = false
        Me.Sam391.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam290
        '
        Me.Sam290.BackColor = System.Drawing.Color.Transparent
        Me.Sam290.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam290.Image = Nothing
        Me.Sam290.ImagePath = ""
        Me.Sam290.IsTransparentImage = false
        Me.Sam290.Location = New System.Drawing.Point(258, 523)
        Me.Sam290.Name = "Sam290"
        Me.Sam290.Rotation = 0
        Me.Sam290.ShowThrough = false
        Me.Sam290.Size = New System.Drawing.Size(24, 24)
        Me.Sam290.TabIndex = 272
        Me.Sam290.TabStop = false
        Me.Sam290.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam289
        '
        Me.Sam289.BackColor = System.Drawing.Color.Transparent
        Me.Sam289.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam289.Image = Nothing
        Me.Sam289.ImagePath = ""
        Me.Sam289.IsTransparentImage = false
        Me.Sam289.Location = New System.Drawing.Point(225, 517)
        Me.Sam289.Name = "Sam289"
        Me.Sam289.Rotation = 0
        Me.Sam289.ShowThrough = false
        Me.Sam289.Size = New System.Drawing.Size(24, 24)
        Me.Sam289.TabIndex = 271
        Me.Sam289.TabStop = false
        Me.Sam289.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam288
        '
        Me.Sam288.BackColor = System.Drawing.Color.Transparent
        Me.Sam288.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam288.Image = Nothing
        Me.Sam288.ImagePath = ""
        Me.Sam288.IsTransparentImage = false
        Me.Sam288.Location = New System.Drawing.Point(193, 508)
        Me.Sam288.Name = "Sam288"
        Me.Sam288.Rotation = 0
        Me.Sam288.ShowThrough = false
        Me.Sam288.Size = New System.Drawing.Size(24, 24)
        Me.Sam288.TabIndex = 270
        Me.Sam288.TabStop = false
        Me.Sam288.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam287
        '
        Me.Sam287.BackColor = System.Drawing.Color.Transparent
        Me.Sam287.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam287.Image = Nothing
        Me.Sam287.ImagePath = ""
        Me.Sam287.IsTransparentImage = false
        Me.Sam287.Location = New System.Drawing.Point(164, 493)
        Me.Sam287.Name = "Sam287"
        Me.Sam287.Rotation = 0
        Me.Sam287.ShowThrough = false
        Me.Sam287.Size = New System.Drawing.Size(24, 24)
        Me.Sam287.TabIndex = 269
        Me.Sam287.TabStop = false
        Me.Sam287.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam286
        '
        Me.Sam286.BackColor = System.Drawing.Color.Transparent
        Me.Sam286.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam286.Image = Nothing
        Me.Sam286.ImagePath = ""
        Me.Sam286.IsTransparentImage = false
        Me.Sam286.Location = New System.Drawing.Point(136, 475)
        Me.Sam286.Name = "Sam286"
        Me.Sam286.Rotation = 0
        Me.Sam286.ShowThrough = false
        Me.Sam286.Size = New System.Drawing.Size(24, 24)
        Me.Sam286.TabIndex = 268
        Me.Sam286.TabStop = false
        Me.Sam286.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam285
        '
        Me.Sam285.BackColor = System.Drawing.Color.Transparent
        Me.Sam285.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam285.Image = Nothing
        Me.Sam285.ImagePath = ""
        Me.Sam285.IsTransparentImage = false
        Me.Sam285.Location = New System.Drawing.Point(111, 453)
        Me.Sam285.Name = "Sam285"
        Me.Sam285.Rotation = 0
        Me.Sam285.ShowThrough = false
        Me.Sam285.Size = New System.Drawing.Size(24, 24)
        Me.Sam285.TabIndex = 267
        Me.Sam285.TabStop = false
        Me.Sam285.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam284
        '
        Me.Sam284.BackColor = System.Drawing.Color.Transparent
        Me.Sam284.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam284.Image = Nothing
        Me.Sam284.ImagePath = ""
        Me.Sam284.IsTransparentImage = false
        Me.Sam284.Location = New System.Drawing.Point(89, 428)
        Me.Sam284.Name = "Sam284"
        Me.Sam284.Rotation = 0
        Me.Sam284.ShowThrough = false
        Me.Sam284.Size = New System.Drawing.Size(24, 24)
        Me.Sam284.TabIndex = 266
        Me.Sam284.TabStop = false
        Me.Sam284.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam283
        '
        Me.Sam283.BackColor = System.Drawing.Color.Transparent
        Me.Sam283.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam283.Image = Nothing
        Me.Sam283.ImagePath = ""
        Me.Sam283.IsTransparentImage = false
        Me.Sam283.Location = New System.Drawing.Point(71, 400)
        Me.Sam283.Name = "Sam283"
        Me.Sam283.Rotation = 0
        Me.Sam283.ShowThrough = false
        Me.Sam283.Size = New System.Drawing.Size(24, 24)
        Me.Sam283.TabIndex = 265
        Me.Sam283.TabStop = false
        Me.Sam283.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam282
        '
        Me.Sam282.BackColor = System.Drawing.Color.Transparent
        Me.Sam282.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam282.Image = Nothing
        Me.Sam282.ImagePath = ""
        Me.Sam282.IsTransparentImage = false
        Me.Sam282.Location = New System.Drawing.Point(58, 370)
        Me.Sam282.Name = "Sam282"
        Me.Sam282.Rotation = 0
        Me.Sam282.ShowThrough = false
        Me.Sam282.Size = New System.Drawing.Size(24, 24)
        Me.Sam282.TabIndex = 264
        Me.Sam282.TabStop = false
        Me.Sam282.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam281
        '
        Me.Sam281.BackColor = System.Drawing.Color.Transparent
        Me.Sam281.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam281.Image = Nothing
        Me.Sam281.ImagePath = ""
        Me.Sam281.IsTransparentImage = false
        Me.Sam281.Location = New System.Drawing.Point(48, 338)
        Me.Sam281.Name = "Sam281"
        Me.Sam281.Rotation = 0
        Me.Sam281.ShowThrough = false
        Me.Sam281.Size = New System.Drawing.Size(24, 24)
        Me.Sam281.TabIndex = 263
        Me.Sam281.TabStop = false
        Me.Sam281.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam280
        '
        Me.Sam280.BackColor = System.Drawing.Color.Transparent
        Me.Sam280.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam280.Image = Nothing
        Me.Sam280.ImagePath = ""
        Me.Sam280.IsTransparentImage = false
        Me.Sam280.Location = New System.Drawing.Point(43, 305)
        Me.Sam280.Name = "Sam280"
        Me.Sam280.Rotation = 0
        Me.Sam280.ShowThrough = false
        Me.Sam280.Size = New System.Drawing.Size(24, 24)
        Me.Sam280.TabIndex = 262
        Me.Sam280.TabStop = false
        Me.Sam280.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam279
        '
        Me.Sam279.BackColor = System.Drawing.Color.Transparent
        Me.Sam279.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam279.Image = Nothing
        Me.Sam279.ImagePath = ""
        Me.Sam279.IsTransparentImage = false
        Me.Sam279.Location = New System.Drawing.Point(42, 271)
        Me.Sam279.Name = "Sam279"
        Me.Sam279.Rotation = 0
        Me.Sam279.ShowThrough = false
        Me.Sam279.Size = New System.Drawing.Size(24, 24)
        Me.Sam279.TabIndex = 261
        Me.Sam279.TabStop = false
        Me.Sam279.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam278
        '
        Me.Sam278.BackColor = System.Drawing.Color.Transparent
        Me.Sam278.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam278.Image = Nothing
        Me.Sam278.ImagePath = ""
        Me.Sam278.IsTransparentImage = false
        Me.Sam278.Location = New System.Drawing.Point(46, 238)
        Me.Sam278.Name = "Sam278"
        Me.Sam278.Rotation = 0
        Me.Sam278.ShowThrough = false
        Me.Sam278.Size = New System.Drawing.Size(24, 24)
        Me.Sam278.TabIndex = 260
        Me.Sam278.TabStop = false
        Me.Sam278.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam277
        '
        Me.Sam277.BackColor = System.Drawing.Color.Transparent
        Me.Sam277.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam277.Image = Nothing
        Me.Sam277.ImagePath = ""
        Me.Sam277.IsTransparentImage = false
        Me.Sam277.Location = New System.Drawing.Point(54, 205)
        Me.Sam277.Name = "Sam277"
        Me.Sam277.Rotation = 0
        Me.Sam277.ShowThrough = false
        Me.Sam277.Size = New System.Drawing.Size(24, 24)
        Me.Sam277.TabIndex = 259
        Me.Sam277.TabStop = false
        Me.Sam277.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam276
        '
        Me.Sam276.BackColor = System.Drawing.Color.Transparent
        Me.Sam276.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam276.Image = Nothing
        Me.Sam276.ImagePath = ""
        Me.Sam276.IsTransparentImage = false
        Me.Sam276.Location = New System.Drawing.Point(67, 174)
        Me.Sam276.Name = "Sam276"
        Me.Sam276.Rotation = 0
        Me.Sam276.ShowThrough = false
        Me.Sam276.Size = New System.Drawing.Size(24, 24)
        Me.Sam276.TabIndex = 258
        Me.Sam276.TabStop = false
        Me.Sam276.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam275
        '
        Me.Sam275.BackColor = System.Drawing.Color.Transparent
        Me.Sam275.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam275.Image = Nothing
        Me.Sam275.ImagePath = ""
        Me.Sam275.IsTransparentImage = false
        Me.Sam275.Location = New System.Drawing.Point(84, 145)
        Me.Sam275.Name = "Sam275"
        Me.Sam275.Rotation = 0
        Me.Sam275.ShowThrough = false
        Me.Sam275.Size = New System.Drawing.Size(24, 24)
        Me.Sam275.TabIndex = 257
        Me.Sam275.TabStop = false
        Me.Sam275.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam274
        '
        Me.Sam274.BackColor = System.Drawing.Color.Transparent
        Me.Sam274.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam274.Image = Nothing
        Me.Sam274.ImagePath = ""
        Me.Sam274.IsTransparentImage = false
        Me.Sam274.Location = New System.Drawing.Point(105, 119)
        Me.Sam274.Name = "Sam274"
        Me.Sam274.Rotation = 0
        Me.Sam274.ShowThrough = false
        Me.Sam274.Size = New System.Drawing.Size(24, 24)
        Me.Sam274.TabIndex = 256
        Me.Sam274.TabStop = false
        Me.Sam274.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam273
        '
        Me.Sam273.BackColor = System.Drawing.Color.Transparent
        Me.Sam273.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam273.Image = Nothing
        Me.Sam273.ImagePath = ""
        Me.Sam273.IsTransparentImage = false
        Me.Sam273.Location = New System.Drawing.Point(129, 96)
        Me.Sam273.Name = "Sam273"
        Me.Sam273.Rotation = 0
        Me.Sam273.ShowThrough = false
        Me.Sam273.Size = New System.Drawing.Size(24, 24)
        Me.Sam273.TabIndex = 255
        Me.Sam273.TabStop = false
        Me.Sam273.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam272
        '
        Me.Sam272.BackColor = System.Drawing.Color.Transparent
        Me.Sam272.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam272.Image = Nothing
        Me.Sam272.ImagePath = ""
        Me.Sam272.IsTransparentImage = false
        Me.Sam272.Location = New System.Drawing.Point(156, 77)
        Me.Sam272.Name = "Sam272"
        Me.Sam272.Rotation = 0
        Me.Sam272.ShowThrough = false
        Me.Sam272.Size = New System.Drawing.Size(24, 24)
        Me.Sam272.TabIndex = 254
        Me.Sam272.TabStop = false
        Me.Sam272.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam271
        '
        Me.Sam271.BackColor = System.Drawing.Color.Transparent
        Me.Sam271.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam271.Image = Nothing
        Me.Sam271.ImagePath = ""
        Me.Sam271.IsTransparentImage = false
        Me.Sam271.Location = New System.Drawing.Point(185, 61)
        Me.Sam271.Name = "Sam271"
        Me.Sam271.Rotation = 0
        Me.Sam271.ShowThrough = false
        Me.Sam271.Size = New System.Drawing.Size(24, 24)
        Me.Sam271.TabIndex = 253
        Me.Sam271.TabStop = false
        Me.Sam271.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam270
        '
        Me.Sam270.BackColor = System.Drawing.Color.Transparent
        Me.Sam270.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam270.Image = Nothing
        Me.Sam270.ImagePath = ""
        Me.Sam270.IsTransparentImage = false
        Me.Sam270.Location = New System.Drawing.Point(216, 50)
        Me.Sam270.Name = "Sam270"
        Me.Sam270.Rotation = 0
        Me.Sam270.ShowThrough = false
        Me.Sam270.Size = New System.Drawing.Size(24, 24)
        Me.Sam270.TabIndex = 252
        Me.Sam270.TabStop = false
        Me.Sam270.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam269
        '
        Me.Sam269.BackColor = System.Drawing.Color.Transparent
        Me.Sam269.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam269.Image = Nothing
        Me.Sam269.ImagePath = ""
        Me.Sam269.IsTransparentImage = false
        Me.Sam269.Location = New System.Drawing.Point(248, 44)
        Me.Sam269.Name = "Sam269"
        Me.Sam269.Rotation = 0
        Me.Sam269.ShowThrough = false
        Me.Sam269.Size = New System.Drawing.Size(24, 24)
        Me.Sam269.TabIndex = 251
        Me.Sam269.TabStop = false
        Me.Sam269.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam268
        '
        Me.Sam268.BackColor = System.Drawing.Color.Transparent
        Me.Sam268.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam268.Image = Nothing
        Me.Sam268.ImagePath = ""
        Me.Sam268.IsTransparentImage = false
        Me.Sam268.Location = New System.Drawing.Point(281, 42)
        Me.Sam268.Name = "Sam268"
        Me.Sam268.Rotation = 0
        Me.Sam268.ShowThrough = false
        Me.Sam268.Size = New System.Drawing.Size(24, 24)
        Me.Sam268.TabIndex = 250
        Me.Sam268.TabStop = false
        Me.Sam268.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam267
        '
        Me.Sam267.BackColor = System.Drawing.Color.Transparent
        Me.Sam267.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam267.Image = Nothing
        Me.Sam267.ImagePath = ""
        Me.Sam267.IsTransparentImage = false
        Me.Sam267.Location = New System.Drawing.Point(314, 45)
        Me.Sam267.Name = "Sam267"
        Me.Sam267.Rotation = 0
        Me.Sam267.ShowThrough = false
        Me.Sam267.Size = New System.Drawing.Size(24, 24)
        Me.Sam267.TabIndex = 249
        Me.Sam267.TabStop = false
        Me.Sam267.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam266
        '
        Me.Sam266.BackColor = System.Drawing.Color.Transparent
        Me.Sam266.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam266.Image = Nothing
        Me.Sam266.ImagePath = ""
        Me.Sam266.IsTransparentImage = false
        Me.Sam266.Location = New System.Drawing.Point(346, 52)
        Me.Sam266.Name = "Sam266"
        Me.Sam266.Rotation = 0
        Me.Sam266.ShowThrough = false
        Me.Sam266.Size = New System.Drawing.Size(24, 24)
        Me.Sam266.TabIndex = 248
        Me.Sam266.TabStop = false
        Me.Sam266.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam265
        '
        Me.Sam265.BackColor = System.Drawing.Color.Transparent
        Me.Sam265.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam265.Image = Nothing
        Me.Sam265.ImagePath = ""
        Me.Sam265.IsTransparentImage = false
        Me.Sam265.Location = New System.Drawing.Point(377, 64)
        Me.Sam265.Name = "Sam265"
        Me.Sam265.Rotation = 0
        Me.Sam265.ShowThrough = false
        Me.Sam265.Size = New System.Drawing.Size(24, 24)
        Me.Sam265.TabIndex = 247
        Me.Sam265.TabStop = false
        Me.Sam265.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam264
        '
        Me.Sam264.BackColor = System.Drawing.Color.Transparent
        Me.Sam264.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam264.Image = Nothing
        Me.Sam264.ImagePath = ""
        Me.Sam264.IsTransparentImage = false
        Me.Sam264.Location = New System.Drawing.Point(406, 81)
        Me.Sam264.Name = "Sam264"
        Me.Sam264.Rotation = 0
        Me.Sam264.ShowThrough = false
        Me.Sam264.Size = New System.Drawing.Size(24, 24)
        Me.Sam264.TabIndex = 246
        Me.Sam264.TabStop = false
        Me.Sam264.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam263
        '
        Me.Sam263.BackColor = System.Drawing.Color.Transparent
        Me.Sam263.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam263.Image = Nothing
        Me.Sam263.ImagePath = ""
        Me.Sam263.IsTransparentImage = false
        Me.Sam263.Location = New System.Drawing.Point(433, 101)
        Me.Sam263.Name = "Sam263"
        Me.Sam263.Rotation = 0
        Me.Sam263.ShowThrough = false
        Me.Sam263.Size = New System.Drawing.Size(24, 24)
        Me.Sam263.TabIndex = 245
        Me.Sam263.TabStop = false
        Me.Sam263.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam262
        '
        Me.Sam262.BackColor = System.Drawing.Color.Transparent
        Me.Sam262.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam262.Image = Nothing
        Me.Sam262.ImagePath = ""
        Me.Sam262.IsTransparentImage = false
        Me.Sam262.Location = New System.Drawing.Point(456, 124)
        Me.Sam262.Name = "Sam262"
        Me.Sam262.Rotation = 0
        Me.Sam262.ShowThrough = false
        Me.Sam262.Size = New System.Drawing.Size(24, 24)
        Me.Sam262.TabIndex = 244
        Me.Sam262.TabStop = false
        Me.Sam262.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam261
        '
        Me.Sam261.BackColor = System.Drawing.Color.Transparent
        Me.Sam261.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam261.Image = Nothing
        Me.Sam261.ImagePath = ""
        Me.Sam261.IsTransparentImage = false
        Me.Sam261.Location = New System.Drawing.Point(475, 151)
        Me.Sam261.Name = "Sam261"
        Me.Sam261.Rotation = 0
        Me.Sam261.ShowThrough = false
        Me.Sam261.Size = New System.Drawing.Size(24, 24)
        Me.Sam261.TabIndex = 243
        Me.Sam261.TabStop = false
        Me.Sam261.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam260
        '
        Me.Sam260.BackColor = System.Drawing.Color.Transparent
        Me.Sam260.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam260.Image = Nothing
        Me.Sam260.ImagePath = ""
        Me.Sam260.IsTransparentImage = false
        Me.Sam260.Location = New System.Drawing.Point(492, 181)
        Me.Sam260.Name = "Sam260"
        Me.Sam260.Rotation = 0
        Me.Sam260.ShowThrough = false
        Me.Sam260.Size = New System.Drawing.Size(24, 24)
        Me.Sam260.TabIndex = 242
        Me.Sam260.TabStop = false
        Me.Sam260.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam259
        '
        Me.Sam259.BackColor = System.Drawing.Color.Transparent
        Me.Sam259.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam259.Image = Nothing
        Me.Sam259.ImagePath = ""
        Me.Sam259.IsTransparentImage = false
        Me.Sam259.Location = New System.Drawing.Point(504, 212)
        Me.Sam259.Name = "Sam259"
        Me.Sam259.Rotation = 0
        Me.Sam259.ShowThrough = false
        Me.Sam259.Size = New System.Drawing.Size(24, 24)
        Me.Sam259.TabIndex = 241
        Me.Sam259.TabStop = false
        Me.Sam259.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam258
        '
        Me.Sam258.BackColor = System.Drawing.Color.Transparent
        Me.Sam258.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam258.Image = Nothing
        Me.Sam258.ImagePath = ""
        Me.Sam258.IsTransparentImage = false
        Me.Sam258.Location = New System.Drawing.Point(511, 244)
        Me.Sam258.Name = "Sam258"
        Me.Sam258.Rotation = 0
        Me.Sam258.ShowThrough = false
        Me.Sam258.Size = New System.Drawing.Size(24, 24)
        Me.Sam258.TabIndex = 240
        Me.Sam258.TabStop = false
        Me.Sam258.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam257
        '
        Me.Sam257.BackColor = System.Drawing.Color.Transparent
        Me.Sam257.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam257.Image = Nothing
        Me.Sam257.ImagePath = ""
        Me.Sam257.IsTransparentImage = false
        Me.Sam257.Location = New System.Drawing.Point(514, 278)
        Me.Sam257.Name = "Sam257"
        Me.Sam257.Rotation = 0
        Me.Sam257.ShowThrough = false
        Me.Sam257.Size = New System.Drawing.Size(24, 24)
        Me.Sam257.TabIndex = 239
        Me.Sam257.TabStop = false
        Me.Sam257.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam256
        '
        Me.Sam256.BackColor = System.Drawing.Color.Transparent
        Me.Sam256.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam256.Image = Nothing
        Me.Sam256.ImagePath = ""
        Me.Sam256.IsTransparentImage = false
        Me.Sam256.Location = New System.Drawing.Point(512, 312)
        Me.Sam256.Name = "Sam256"
        Me.Sam256.Rotation = 0
        Me.Sam256.ShowThrough = false
        Me.Sam256.Size = New System.Drawing.Size(24, 24)
        Me.Sam256.TabIndex = 238
        Me.Sam256.TabStop = false
        Me.Sam256.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam255
        '
        Me.Sam255.BackColor = System.Drawing.Color.Transparent
        Me.Sam255.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam255.Image = Nothing
        Me.Sam255.ImagePath = ""
        Me.Sam255.IsTransparentImage = false
        Me.Sam255.Location = New System.Drawing.Point(506, 345)
        Me.Sam255.Name = "Sam255"
        Me.Sam255.Rotation = 0
        Me.Sam255.ShowThrough = false
        Me.Sam255.Size = New System.Drawing.Size(24, 24)
        Me.Sam255.TabIndex = 237
        Me.Sam255.TabStop = false
        Me.Sam255.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam254
        '
        Me.Sam254.BackColor = System.Drawing.Color.Transparent
        Me.Sam254.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam254.Image = Nothing
        Me.Sam254.ImagePath = ""
        Me.Sam254.IsTransparentImage = false
        Me.Sam254.Location = New System.Drawing.Point(496, 376)
        Me.Sam254.Name = "Sam254"
        Me.Sam254.Rotation = 0
        Me.Sam254.ShowThrough = false
        Me.Sam254.Size = New System.Drawing.Size(24, 24)
        Me.Sam254.TabIndex = 236
        Me.Sam254.TabStop = false
        Me.Sam254.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam253
        '
        Me.Sam253.BackColor = System.Drawing.Color.Transparent
        Me.Sam253.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam253.Image = Nothing
        Me.Sam253.ImagePath = ""
        Me.Sam253.IsTransparentImage = false
        Me.Sam253.Location = New System.Drawing.Point(481, 406)
        Me.Sam253.Name = "Sam253"
        Me.Sam253.Rotation = 0
        Me.Sam253.ShowThrough = false
        Me.Sam253.Size = New System.Drawing.Size(24, 24)
        Me.Sam253.TabIndex = 235
        Me.Sam253.TabStop = false
        Me.Sam253.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam252
        '
        Me.Sam252.BackColor = System.Drawing.Color.Transparent
        Me.Sam252.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam252.Image = Nothing
        Me.Sam252.ImagePath = ""
        Me.Sam252.IsTransparentImage = false
        Me.Sam252.Location = New System.Drawing.Point(462, 434)
        Me.Sam252.Name = "Sam252"
        Me.Sam252.Rotation = 0
        Me.Sam252.ShowThrough = false
        Me.Sam252.Size = New System.Drawing.Size(24, 24)
        Me.Sam252.TabIndex = 234
        Me.Sam252.TabStop = false
        Me.Sam252.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam251
        '
        Me.Sam251.BackColor = System.Drawing.Color.Transparent
        Me.Sam251.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam251.Image = Nothing
        Me.Sam251.ImagePath = ""
        Me.Sam251.IsTransparentImage = false
        Me.Sam251.Location = New System.Drawing.Point(439, 459)
        Me.Sam251.Name = "Sam251"
        Me.Sam251.Rotation = 0
        Me.Sam251.ShowThrough = false
        Me.Sam251.Size = New System.Drawing.Size(24, 24)
        Me.Sam251.TabIndex = 233
        Me.Sam251.TabStop = false
        Me.Sam251.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam250
        '
        Me.Sam250.BackColor = System.Drawing.Color.Transparent
        Me.Sam250.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam250.Image = Nothing
        Me.Sam250.ImagePath = ""
        Me.Sam250.IsTransparentImage = false
        Me.Sam250.Location = New System.Drawing.Point(414, 480)
        Me.Sam250.Name = "Sam250"
        Me.Sam250.Rotation = 0
        Me.Sam250.ShowThrough = false
        Me.Sam250.Size = New System.Drawing.Size(24, 24)
        Me.Sam250.TabIndex = 232
        Me.Sam250.TabStop = false
        Me.Sam250.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam249
        '
        Me.Sam249.BackColor = System.Drawing.Color.Transparent
        Me.Sam249.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam249.Image = Nothing
        Me.Sam249.ImagePath = ""
        Me.Sam249.IsTransparentImage = false
        Me.Sam249.Location = New System.Drawing.Point(386, 497)
        Me.Sam249.Name = "Sam249"
        Me.Sam249.Rotation = 0
        Me.Sam249.ShowThrough = false
        Me.Sam249.Size = New System.Drawing.Size(24, 24)
        Me.Sam249.TabIndex = 231
        Me.Sam249.TabStop = false
        Me.Sam249.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam248
        '
        Me.Sam248.BackColor = System.Drawing.Color.Transparent
        Me.Sam248.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam248.Image = Nothing
        Me.Sam248.ImagePath = ""
        Me.Sam248.IsTransparentImage = false
        Me.Sam248.Location = New System.Drawing.Point(355, 511)
        Me.Sam248.Name = "Sam248"
        Me.Sam248.Rotation = 0
        Me.Sam248.ShowThrough = false
        Me.Sam248.Size = New System.Drawing.Size(24, 24)
        Me.Sam248.TabIndex = 230
        Me.Sam248.TabStop = false
        Me.Sam248.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam247
        '
        Me.Sam247.BackColor = System.Drawing.Color.Transparent
        Me.Sam247.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam247.Image = Nothing
        Me.Sam247.ImagePath = ""
        Me.Sam247.IsTransparentImage = false
        Me.Sam247.Location = New System.Drawing.Point(323, 519)
        Me.Sam247.Name = "Sam247"
        Me.Sam247.Rotation = 0
        Me.Sam247.ShowThrough = false
        Me.Sam247.Size = New System.Drawing.Size(24, 24)
        Me.Sam247.TabIndex = 229
        Me.Sam247.TabStop = false
        Me.Sam247.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam246
        '
        Me.Sam246.BackColor = System.Drawing.Color.Transparent
        Me.Sam246.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam246.Image = Nothing
        Me.Sam246.ImagePath = ""
        Me.Sam246.IsTransparentImage = false
        Me.Sam246.Location = New System.Drawing.Point(291, 523)
        Me.Sam246.Name = "Sam246"
        Me.Sam246.Rotation = 0
        Me.Sam246.ShowThrough = false
        Me.Sam246.Size = New System.Drawing.Size(24, 24)
        Me.Sam246.TabIndex = 228
        Me.Sam246.TabStop = false
        Me.Sam246.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam145
        '
        Me.Sam145.BackColor = System.Drawing.Color.Transparent
        Me.Sam145.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam145.Image = Nothing
        Me.Sam145.ImagePath = ""
        Me.Sam145.IsTransparentImage = false
        Me.Sam145.Location = New System.Drawing.Point(269, 553)
        Me.Sam145.Name = "Sam145"
        Me.Sam145.Rotation = 0
        Me.Sam145.ShowThrough = false
        Me.Sam145.Size = New System.Drawing.Size(24, 24)
        Me.Sam145.TabIndex = 181
        Me.Sam145.TabStop = false
        Me.Sam145.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam144
        '
        Me.Sam144.BackColor = System.Drawing.Color.Transparent
        Me.Sam144.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam144.Image = Nothing
        Me.Sam144.ImagePath = ""
        Me.Sam144.IsTransparentImage = false
        Me.Sam144.Location = New System.Drawing.Point(232, 549)
        Me.Sam144.Name = "Sam144"
        Me.Sam144.Rotation = 0
        Me.Sam144.ShowThrough = false
        Me.Sam144.Size = New System.Drawing.Size(24, 24)
        Me.Sam144.TabIndex = 180
        Me.Sam144.TabStop = false
        Me.Sam144.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam143
        '
        Me.Sam143.BackColor = System.Drawing.Color.Transparent
        Me.Sam143.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam143.Image = Nothing
        Me.Sam143.ImagePath = ""
        Me.Sam143.IsTransparentImage = false
        Me.Sam143.Location = New System.Drawing.Point(196, 540)
        Me.Sam143.Name = "Sam143"
        Me.Sam143.Rotation = 0
        Me.Sam143.ShowThrough = false
        Me.Sam143.Size = New System.Drawing.Size(24, 24)
        Me.Sam143.TabIndex = 179
        Me.Sam143.TabStop = false
        Me.Sam143.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam142
        '
        Me.Sam142.BackColor = System.Drawing.Color.Transparent
        Me.Sam142.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam142.Image = Nothing
        Me.Sam142.ImagePath = ""
        Me.Sam142.IsTransparentImage = false
        Me.Sam142.Location = New System.Drawing.Point(162, 526)
        Me.Sam142.Name = "Sam142"
        Me.Sam142.Rotation = 0
        Me.Sam142.ShowThrough = false
        Me.Sam142.Size = New System.Drawing.Size(24, 24)
        Me.Sam142.TabIndex = 178
        Me.Sam142.TabStop = false
        Me.Sam142.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam11
        '
        Me.Sam11.BackColor = System.Drawing.Color.Transparent
        Me.Sam11.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam11.Image = Nothing
        Me.Sam11.ImagePath = ""
        Me.Sam11.IsTransparentImage = false
        Me.Sam11.Location = New System.Drawing.Point(306, 552)
        Me.Sam11.Name = "Sam11"
        Me.Sam11.Rotation = 0
        Me.Sam11.ShowThrough = false
        Me.Sam11.Size = New System.Drawing.Size(24, 24)
        Me.Sam11.TabIndex = 177
        Me.Sam11.TabStop = false
        Me.Sam11.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam141
        '
        Me.Sam141.BackColor = System.Drawing.Color.Transparent
        Me.Sam141.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam141.Image = Nothing
        Me.Sam141.ImagePath = ""
        Me.Sam141.IsTransparentImage = false
        Me.Sam141.Location = New System.Drawing.Point(130, 507)
        Me.Sam141.Name = "Sam141"
        Me.Sam141.Rotation = 0
        Me.Sam141.ShowThrough = false
        Me.Sam141.Size = New System.Drawing.Size(24, 24)
        Me.Sam141.TabIndex = 176
        Me.Sam141.TabStop = false
        Me.Sam141.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam140
        '
        Me.Sam140.BackColor = System.Drawing.Color.Transparent
        Me.Sam140.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam140.Image = Nothing
        Me.Sam140.ImagePath = ""
        Me.Sam140.IsTransparentImage = false
        Me.Sam140.Location = New System.Drawing.Point(101, 484)
        Me.Sam140.Name = "Sam140"
        Me.Sam140.Rotation = 0
        Me.Sam140.ShowThrough = false
        Me.Sam140.Size = New System.Drawing.Size(24, 24)
        Me.Sam140.TabIndex = 175
        Me.Sam140.TabStop = false
        Me.Sam140.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam139
        '
        Me.Sam139.BackColor = System.Drawing.Color.Transparent
        Me.Sam139.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam139.Image = Nothing
        Me.Sam139.ImagePath = ""
        Me.Sam139.IsTransparentImage = false
        Me.Sam139.Location = New System.Drawing.Point(75, 457)
        Me.Sam139.Name = "Sam139"
        Me.Sam139.Rotation = 0
        Me.Sam139.ShowThrough = false
        Me.Sam139.Size = New System.Drawing.Size(24, 24)
        Me.Sam139.TabIndex = 174
        Me.Sam139.TabStop = false
        Me.Sam139.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam138
        '
        Me.Sam138.BackColor = System.Drawing.Color.Transparent
        Me.Sam138.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam138.Image = Nothing
        Me.Sam138.ImagePath = ""
        Me.Sam138.IsTransparentImage = false
        Me.Sam138.Location = New System.Drawing.Point(53, 426)
        Me.Sam138.Name = "Sam138"
        Me.Sam138.Rotation = 0
        Me.Sam138.ShowThrough = false
        Me.Sam138.Size = New System.Drawing.Size(24, 24)
        Me.Sam138.TabIndex = 173
        Me.Sam138.TabStop = false
        Me.Sam138.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam137
        '
        Me.Sam137.BackColor = System.Drawing.Color.Transparent
        Me.Sam137.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam137.Image = Nothing
        Me.Sam137.ImagePath = ""
        Me.Sam137.IsTransparentImage = false
        Me.Sam137.Location = New System.Drawing.Point(35, 393)
        Me.Sam137.Name = "Sam137"
        Me.Sam137.Rotation = 0
        Me.Sam137.ShowThrough = false
        Me.Sam137.Size = New System.Drawing.Size(24, 24)
        Me.Sam137.TabIndex = 172
        Me.Sam137.TabStop = false
        Me.Sam137.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam136
        '
        Me.Sam136.BackColor = System.Drawing.Color.Transparent
        Me.Sam136.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam136.Image = Nothing
        Me.Sam136.ImagePath = ""
        Me.Sam136.IsTransparentImage = false
        Me.Sam136.Location = New System.Drawing.Point(23, 358)
        Me.Sam136.Name = "Sam136"
        Me.Sam136.Rotation = 0
        Me.Sam136.ShowThrough = false
        Me.Sam136.Size = New System.Drawing.Size(24, 24)
        Me.Sam136.TabIndex = 171
        Me.Sam136.TabStop = false
        Me.Sam136.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam135
        '
        Me.Sam135.BackColor = System.Drawing.Color.Transparent
        Me.Sam135.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam135.Image = Nothing
        Me.Sam135.ImagePath = ""
        Me.Sam135.IsTransparentImage = false
        Me.Sam135.Location = New System.Drawing.Point(15, 321)
        Me.Sam135.Name = "Sam135"
        Me.Sam135.Rotation = 0
        Me.Sam135.ShowThrough = false
        Me.Sam135.Size = New System.Drawing.Size(24, 24)
        Me.Sam135.TabIndex = 170
        Me.Sam135.TabStop = false
        Me.Sam135.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam134
        '
        Me.Sam134.BackColor = System.Drawing.Color.Transparent
        Me.Sam134.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam134.Image = Nothing
        Me.Sam134.ImagePath = ""
        Me.Sam134.IsTransparentImage = false
        Me.Sam134.Location = New System.Drawing.Point(12, 284)
        Me.Sam134.Name = "Sam134"
        Me.Sam134.Rotation = 0
        Me.Sam134.ShowThrough = false
        Me.Sam134.Size = New System.Drawing.Size(24, 24)
        Me.Sam134.TabIndex = 169
        Me.Sam134.TabStop = false
        Me.Sam134.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam133
        '
        Me.Sam133.BackColor = System.Drawing.Color.Transparent
        Me.Sam133.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam133.Image = Nothing
        Me.Sam133.ImagePath = ""
        Me.Sam133.IsTransparentImage = false
        Me.Sam133.Location = New System.Drawing.Point(15, 246)
        Me.Sam133.Name = "Sam133"
        Me.Sam133.Rotation = 0
        Me.Sam133.ShowThrough = false
        Me.Sam133.Size = New System.Drawing.Size(24, 24)
        Me.Sam133.TabIndex = 168
        Me.Sam133.TabStop = false
        Me.Sam133.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam132
        '
        Me.Sam132.BackColor = System.Drawing.Color.Transparent
        Me.Sam132.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam132.Image = Nothing
        Me.Sam132.ImagePath = ""
        Me.Sam132.IsTransparentImage = false
        Me.Sam132.Location = New System.Drawing.Point(22, 209)
        Me.Sam132.Name = "Sam132"
        Me.Sam132.Rotation = 0
        Me.Sam132.ShowThrough = false
        Me.Sam132.Size = New System.Drawing.Size(24, 24)
        Me.Sam132.TabIndex = 167
        Me.Sam132.TabStop = false
        Me.Sam132.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam131
        '
        Me.Sam131.BackColor = System.Drawing.Color.Transparent
        Me.Sam131.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam131.Image = Nothing
        Me.Sam131.ImagePath = ""
        Me.Sam131.IsTransparentImage = false
        Me.Sam131.Location = New System.Drawing.Point(35, 174)
        Me.Sam131.Name = "Sam131"
        Me.Sam131.Rotation = 0
        Me.Sam131.ShowThrough = false
        Me.Sam131.Size = New System.Drawing.Size(24, 24)
        Me.Sam131.TabIndex = 166
        Me.Sam131.TabStop = false
        Me.Sam131.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam130
        '
        Me.Sam130.BackColor = System.Drawing.Color.Transparent
        Me.Sam130.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam130.Image = Nothing
        Me.Sam130.ImagePath = ""
        Me.Sam130.IsTransparentImage = false
        Me.Sam130.Location = New System.Drawing.Point(52, 140)
        Me.Sam130.Name = "Sam130"
        Me.Sam130.Rotation = 0
        Me.Sam130.ShowThrough = false
        Me.Sam130.Size = New System.Drawing.Size(24, 24)
        Me.Sam130.TabIndex = 165
        Me.Sam130.TabStop = false
        Me.Sam130.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam129
        '
        Me.Sam129.BackColor = System.Drawing.Color.Transparent
        Me.Sam129.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam129.Image = Nothing
        Me.Sam129.ImagePath = ""
        Me.Sam129.IsTransparentImage = false
        Me.Sam129.Location = New System.Drawing.Point(74, 110)
        Me.Sam129.Name = "Sam129"
        Me.Sam129.Rotation = 0
        Me.Sam129.ShowThrough = false
        Me.Sam129.Size = New System.Drawing.Size(24, 24)
        Me.Sam129.TabIndex = 164
        Me.Sam129.TabStop = false
        Me.Sam129.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam128
        '
        Me.Sam128.BackColor = System.Drawing.Color.Transparent
        Me.Sam128.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam128.Image = Nothing
        Me.Sam128.ImagePath = ""
        Me.Sam128.IsTransparentImage = false
        Me.Sam128.Location = New System.Drawing.Point(100, 82)
        Me.Sam128.Name = "Sam128"
        Me.Sam128.Rotation = 0
        Me.Sam128.ShowThrough = false
        Me.Sam128.Size = New System.Drawing.Size(24, 24)
        Me.Sam128.TabIndex = 163
        Me.Sam128.TabStop = false
        Me.Sam128.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam127
        '
        Me.Sam127.BackColor = System.Drawing.Color.Transparent
        Me.Sam127.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam127.Image = Nothing
        Me.Sam127.ImagePath = ""
        Me.Sam127.IsTransparentImage = false
        Me.Sam127.Location = New System.Drawing.Point(129, 59)
        Me.Sam127.Name = "Sam127"
        Me.Sam127.Rotation = 0
        Me.Sam127.ShowThrough = false
        Me.Sam127.Size = New System.Drawing.Size(24, 24)
        Me.Sam127.TabIndex = 162
        Me.Sam127.TabStop = false
        Me.Sam127.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam126
        '
        Me.Sam126.BackColor = System.Drawing.Color.Transparent
        Me.Sam126.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam126.Image = Nothing
        Me.Sam126.ImagePath = ""
        Me.Sam126.IsTransparentImage = false
        Me.Sam126.Location = New System.Drawing.Point(161, 40)
        Me.Sam126.Name = "Sam126"
        Me.Sam126.Rotation = 0
        Me.Sam126.ShowThrough = false
        Me.Sam126.Size = New System.Drawing.Size(24, 24)
        Me.Sam126.TabIndex = 160
        Me.Sam126.TabStop = false
        Me.Sam126.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam125
        '
        Me.Sam125.BackColor = System.Drawing.Color.Transparent
        Me.Sam125.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam125.Image = Nothing
        Me.Sam125.ImagePath = ""
        Me.Sam125.IsTransparentImage = false
        Me.Sam125.Location = New System.Drawing.Point(195, 26)
        Me.Sam125.Name = "Sam125"
        Me.Sam125.Rotation = 0
        Me.Sam125.ShowThrough = false
        Me.Sam125.Size = New System.Drawing.Size(24, 24)
        Me.Sam125.TabIndex = 159
        Me.Sam125.TabStop = false
        Me.Sam125.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam124
        '
        Me.Sam124.BackColor = System.Drawing.Color.Transparent
        Me.Sam124.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam124.Image = Nothing
        Me.Sam124.ImagePath = ""
        Me.Sam124.IsTransparentImage = false
        Me.Sam124.Location = New System.Drawing.Point(231, 17)
        Me.Sam124.Name = "Sam124"
        Me.Sam124.Rotation = 0
        Me.Sam124.ShowThrough = false
        Me.Sam124.Size = New System.Drawing.Size(24, 24)
        Me.Sam124.TabIndex = 158
        Me.Sam124.TabStop = false
        Me.Sam124.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam123
        '
        Me.Sam123.BackColor = System.Drawing.Color.Transparent
        Me.Sam123.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam123.Image = Nothing
        Me.Sam123.ImagePath = ""
        Me.Sam123.IsTransparentImage = false
        Me.Sam123.Location = New System.Drawing.Point(268, 13)
        Me.Sam123.Name = "Sam123"
        Me.Sam123.Rotation = 0
        Me.Sam123.ShowThrough = false
        Me.Sam123.Size = New System.Drawing.Size(24, 24)
        Me.Sam123.TabIndex = 157
        Me.Sam123.TabStop = false
        Me.Sam123.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam122
        '
        Me.Sam122.BackColor = System.Drawing.Color.Transparent
        Me.Sam122.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam122.Image = Nothing
        Me.Sam122.ImagePath = ""
        Me.Sam122.IsTransparentImage = false
        Me.Sam122.Location = New System.Drawing.Point(305, 14)
        Me.Sam122.Name = "Sam122"
        Me.Sam122.Rotation = 0
        Me.Sam122.ShowThrough = false
        Me.Sam122.Size = New System.Drawing.Size(24, 24)
        Me.Sam122.TabIndex = 156
        Me.Sam122.TabStop = false
        Me.Sam122.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam121
        '
        Me.Sam121.BackColor = System.Drawing.Color.Transparent
        Me.Sam121.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam121.Image = Nothing
        Me.Sam121.ImagePath = ""
        Me.Sam121.IsTransparentImage = false
        Me.Sam121.Location = New System.Drawing.Point(342, 20)
        Me.Sam121.Name = "Sam121"
        Me.Sam121.Rotation = 0
        Me.Sam121.ShowThrough = false
        Me.Sam121.Size = New System.Drawing.Size(24, 24)
        Me.Sam121.TabIndex = 155
        Me.Sam121.TabStop = false
        Me.Sam121.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam120
        '
        Me.Sam120.BackColor = System.Drawing.Color.Transparent
        Me.Sam120.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam120.Image = Nothing
        Me.Sam120.ImagePath = ""
        Me.Sam120.IsTransparentImage = false
        Me.Sam120.Location = New System.Drawing.Point(377, 32)
        Me.Sam120.Name = "Sam120"
        Me.Sam120.Rotation = 0
        Me.Sam120.ShowThrough = false
        Me.Sam120.Size = New System.Drawing.Size(24, 24)
        Me.Sam120.TabIndex = 154
        Me.Sam120.TabStop = false
        Me.Sam120.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam119
        '
        Me.Sam119.BackColor = System.Drawing.Color.Transparent
        Me.Sam119.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam119.Image = Nothing
        Me.Sam119.ImagePath = ""
        Me.Sam119.IsTransparentImage = false
        Me.Sam119.Location = New System.Drawing.Point(410, 48)
        Me.Sam119.Name = "Sam119"
        Me.Sam119.Rotation = 0
        Me.Sam119.ShowThrough = false
        Me.Sam119.Size = New System.Drawing.Size(24, 24)
        Me.Sam119.TabIndex = 153
        Me.Sam119.TabStop = false
        Me.Sam119.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam118
        '
        Me.Sam118.BackColor = System.Drawing.Color.Transparent
        Me.Sam118.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam118.Image = Nothing
        Me.Sam118.ImagePath = ""
        Me.Sam118.IsTransparentImage = false
        Me.Sam118.Location = New System.Drawing.Point(441, 69)
        Me.Sam118.Name = "Sam118"
        Me.Sam118.Rotation = 0
        Me.Sam118.ShowThrough = false
        Me.Sam118.Size = New System.Drawing.Size(24, 24)
        Me.Sam118.TabIndex = 152
        Me.Sam118.TabStop = false
        Me.Sam118.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam117
        '
        Me.Sam117.BackColor = System.Drawing.Color.Transparent
        Me.Sam117.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam117.Image = Nothing
        Me.Sam117.ImagePath = ""
        Me.Sam117.IsTransparentImage = false
        Me.Sam117.Location = New System.Drawing.Point(469, 95)
        Me.Sam117.Name = "Sam117"
        Me.Sam117.Rotation = 0
        Me.Sam117.ShowThrough = false
        Me.Sam117.Size = New System.Drawing.Size(24, 24)
        Me.Sam117.TabIndex = 151
        Me.Sam117.TabStop = false
        Me.Sam117.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam116
        '
        Me.Sam116.BackColor = System.Drawing.Color.Transparent
        Me.Sam116.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam116.Image = Nothing
        Me.Sam116.ImagePath = ""
        Me.Sam116.IsTransparentImage = false
        Me.Sam116.Location = New System.Drawing.Point(492, 124)
        Me.Sam116.Name = "Sam116"
        Me.Sam116.Rotation = 0
        Me.Sam116.ShowThrough = false
        Me.Sam116.Size = New System.Drawing.Size(24, 24)
        Me.Sam116.TabIndex = 150
        Me.Sam116.TabStop = false
        Me.Sam116.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam115
        '
        Me.Sam115.BackColor = System.Drawing.Color.Transparent
        Me.Sam115.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam115.Image = Nothing
        Me.Sam115.ImagePath = ""
        Me.Sam115.IsTransparentImage = false
        Me.Sam115.Location = New System.Drawing.Point(512, 155)
        Me.Sam115.Name = "Sam115"
        Me.Sam115.Rotation = 0
        Me.Sam115.ShowThrough = false
        Me.Sam115.Size = New System.Drawing.Size(24, 24)
        Me.Sam115.TabIndex = 149
        Me.Sam115.TabStop = false
        Me.Sam115.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam114
        '
        Me.Sam114.BackColor = System.Drawing.Color.Transparent
        Me.Sam114.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam114.Image = Nothing
        Me.Sam114.ImagePath = ""
        Me.Sam114.IsTransparentImage = false
        Me.Sam114.Location = New System.Drawing.Point(527, 190)
        Me.Sam114.Name = "Sam114"
        Me.Sam114.Rotation = 0
        Me.Sam114.ShowThrough = false
        Me.Sam114.Size = New System.Drawing.Size(24, 24)
        Me.Sam114.TabIndex = 148
        Me.Sam114.TabStop = false
        Me.Sam114.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam113
        '
        Me.Sam113.BackColor = System.Drawing.Color.Transparent
        Me.Sam113.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam113.Image = Nothing
        Me.Sam113.ImagePath = ""
        Me.Sam113.IsTransparentImage = false
        Me.Sam113.Location = New System.Drawing.Point(537, 226)
        Me.Sam113.Name = "Sam113"
        Me.Sam113.Rotation = 0
        Me.Sam113.ShowThrough = false
        Me.Sam113.Size = New System.Drawing.Size(24, 24)
        Me.Sam113.TabIndex = 147
        Me.Sam113.TabStop = false
        Me.Sam113.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam112
        '
        Me.Sam112.BackColor = System.Drawing.Color.Transparent
        Me.Sam112.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam112.Image = Nothing
        Me.Sam112.ImagePath = ""
        Me.Sam112.IsTransparentImage = false
        Me.Sam112.Location = New System.Drawing.Point(543, 264)
        Me.Sam112.Name = "Sam112"
        Me.Sam112.Rotation = 0
        Me.Sam112.ShowThrough = false
        Me.Sam112.Size = New System.Drawing.Size(24, 24)
        Me.Sam112.TabIndex = 146
        Me.Sam112.TabStop = false
        Me.Sam112.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam111
        '
        Me.Sam111.BackColor = System.Drawing.Color.Transparent
        Me.Sam111.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam111.Image = Nothing
        Me.Sam111.ImagePath = ""
        Me.Sam111.IsTransparentImage = false
        Me.Sam111.Location = New System.Drawing.Point(543, 301)
        Me.Sam111.Name = "Sam111"
        Me.Sam111.Rotation = 0
        Me.Sam111.ShowThrough = false
        Me.Sam111.Size = New System.Drawing.Size(24, 24)
        Me.Sam111.TabIndex = 145
        Me.Sam111.TabStop = false
        Me.Sam111.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam110
        '
        Me.Sam110.BackColor = System.Drawing.Color.Transparent
        Me.Sam110.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam110.Image = Nothing
        Me.Sam110.ImagePath = ""
        Me.Sam110.IsTransparentImage = false
        Me.Sam110.Location = New System.Drawing.Point(537, 339)
        Me.Sam110.Name = "Sam110"
        Me.Sam110.Rotation = 0
        Me.Sam110.ShowThrough = false
        Me.Sam110.Size = New System.Drawing.Size(24, 24)
        Me.Sam110.TabIndex = 144
        Me.Sam110.TabStop = false
        Me.Sam110.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam19
        '
        Me.Sam19.BackColor = System.Drawing.Color.Transparent
        Me.Sam19.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam19.Image = Nothing
        Me.Sam19.ImagePath = ""
        Me.Sam19.IsTransparentImage = false
        Me.Sam19.Location = New System.Drawing.Point(527, 375)
        Me.Sam19.Name = "Sam19"
        Me.Sam19.Rotation = 0
        Me.Sam19.ShowThrough = false
        Me.Sam19.Size = New System.Drawing.Size(24, 24)
        Me.Sam19.TabIndex = 143
        Me.Sam19.TabStop = false
        Me.Sam19.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam18
        '
        Me.Sam18.BackColor = System.Drawing.Color.Transparent
        Me.Sam18.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam18.Image = Nothing
        Me.Sam18.ImagePath = ""
        Me.Sam18.IsTransparentImage = false
        Me.Sam18.Location = New System.Drawing.Point(513, 409)
        Me.Sam18.Name = "Sam18"
        Me.Sam18.Rotation = 0
        Me.Sam18.ShowThrough = false
        Me.Sam18.Size = New System.Drawing.Size(24, 24)
        Me.Sam18.TabIndex = 142
        Me.Sam18.TabStop = false
        Me.Sam18.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam17
        '
        Me.Sam17.BackColor = System.Drawing.Color.Transparent
        Me.Sam17.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam17.Image = Nothing
        Me.Sam17.ImagePath = ""
        Me.Sam17.IsTransparentImage = false
        Me.Sam17.Location = New System.Drawing.Point(493, 441)
        Me.Sam17.Name = "Sam17"
        Me.Sam17.Rotation = 0
        Me.Sam17.ShowThrough = false
        Me.Sam17.Size = New System.Drawing.Size(24, 24)
        Me.Sam17.TabIndex = 140
        Me.Sam17.TabStop = false
        Me.Sam17.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam16
        '
        Me.Sam16.BackColor = System.Drawing.Color.Transparent
        Me.Sam16.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam16.Image = Nothing
        Me.Sam16.ImagePath = ""
        Me.Sam16.IsTransparentImage = false
        Me.Sam16.Location = New System.Drawing.Point(469, 470)
        Me.Sam16.Name = "Sam16"
        Me.Sam16.Rotation = 0
        Me.Sam16.ShowThrough = false
        Me.Sam16.Size = New System.Drawing.Size(24, 24)
        Me.Sam16.TabIndex = 139
        Me.Sam16.TabStop = false
        Me.Sam16.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam15
        '
        Me.Sam15.BackColor = System.Drawing.Color.Transparent
        Me.Sam15.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam15.Image = Nothing
        Me.Sam15.ImagePath = ""
        Me.Sam15.IsTransparentImage = false
        Me.Sam15.Location = New System.Drawing.Point(442, 496)
        Me.Sam15.Name = "Sam15"
        Me.Sam15.Rotation = 0
        Me.Sam15.ShowThrough = false
        Me.Sam15.Size = New System.Drawing.Size(24, 24)
        Me.Sam15.TabIndex = 138
        Me.Sam15.TabStop = false
        Me.Sam15.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam14
        '
        Me.Sam14.BackColor = System.Drawing.Color.Transparent
        Me.Sam14.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam14.Image = Nothing
        Me.Sam14.ImagePath = ""
        Me.Sam14.IsTransparentImage = false
        Me.Sam14.Location = New System.Drawing.Point(412, 516)
        Me.Sam14.Name = "Sam14"
        Me.Sam14.Rotation = 0
        Me.Sam14.ShowThrough = false
        Me.Sam14.Size = New System.Drawing.Size(24, 24)
        Me.Sam14.TabIndex = 137
        Me.Sam14.TabStop = false
        Me.Sam14.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam13
        '
        Me.Sam13.BackColor = System.Drawing.Color.Transparent
        Me.Sam13.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam13.Image = Nothing
        Me.Sam13.ImagePath = ""
        Me.Sam13.IsTransparentImage = false
        Me.Sam13.Location = New System.Drawing.Point(378, 533)
        Me.Sam13.Name = "Sam13"
        Me.Sam13.Rotation = 0
        Me.Sam13.ShowThrough = false
        Me.Sam13.Size = New System.Drawing.Size(24, 24)
        Me.Sam13.TabIndex = 136
        Me.Sam13.TabStop = false
        Me.Sam13.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam12
        '
        Me.Sam12.BackColor = System.Drawing.Color.Transparent
        Me.Sam12.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam12.Image = Nothing
        Me.Sam12.ImagePath = ""
        Me.Sam12.IsTransparentImage = false
        Me.Sam12.Location = New System.Drawing.Point(343, 545)
        Me.Sam12.Name = "Sam12"
        Me.Sam12.Rotation = 0
        Me.Sam12.ShowThrough = false
        Me.Sam12.Size = New System.Drawing.Size(24, 24)
        Me.Sam12.TabIndex = 135
        Me.Sam12.TabStop = false
        Me.Sam12.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3130
        '
        Me.Sam3130.BackColor = System.Drawing.Color.Transparent
        Me.Sam3130.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3130.Image = Nothing
        Me.Sam3130.ImagePath = ""
        Me.Sam3130.IsTransparentImage = false
        Me.Sam3130.Location = New System.Drawing.Point(144, 430)
        Me.Sam3130.Name = "Sam3130"
        Me.Sam3130.Rotation = 0
        Me.Sam3130.ShowThrough = false
        Me.Sam3130.Size = New System.Drawing.Size(24, 23)
        Me.Sam3130.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3130.TabIndex = 313
        Me.Sam3130.TabStop = false
        Me.Sam3130.TransparentColor = System.Drawing.Color.LightPink
        '
        'ReagentsTab
        '
        Me.ReagentsTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ReagentsTab.Appearance.PageClient.Image = CType(resources.GetObject("ReagentsTab.Appearance.PageClient.Image"),System.Drawing.Image)
        Me.ReagentsTab.Appearance.PageClient.Options.UseBackColor = true
        Me.ReagentsTab.Appearance.PageClient.Options.UseImage = true
        Me.ReagentsTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ReagentsTab.Controls.Add(Me.PanelControl9)
        Me.ReagentsTab.Controls.Add(Me.Reag11)
        Me.ReagentsTab.Controls.Add(Me.Reag12)
        Me.ReagentsTab.Controls.Add(Me.Reag13)
        Me.ReagentsTab.Controls.Add(Me.Reag14)
        Me.ReagentsTab.Controls.Add(Me.Reag15)
        Me.ReagentsTab.Controls.Add(Me.Reag16)
        Me.ReagentsTab.Controls.Add(Me.Reag17)
        Me.ReagentsTab.Controls.Add(Me.Reag18)
        Me.ReagentsTab.Controls.Add(Me.Reag19)
        Me.ReagentsTab.Controls.Add(Me.Reag110)
        Me.ReagentsTab.Controls.Add(Me.Reag111)
        Me.ReagentsTab.Controls.Add(Me.Reag112)
        Me.ReagentsTab.Controls.Add(Me.Reag113)
        Me.ReagentsTab.Controls.Add(Me.Reag114)
        Me.ReagentsTab.Controls.Add(Me.Reag115)
        Me.ReagentsTab.Controls.Add(Me.Reag116)
        Me.ReagentsTab.Controls.Add(Me.Reag117)
        Me.ReagentsTab.Controls.Add(Me.Reag118)
        Me.ReagentsTab.Controls.Add(Me.Reag119)
        Me.ReagentsTab.Controls.Add(Me.Reag120)
        Me.ReagentsTab.Controls.Add(Me.Reag121)
        Me.ReagentsTab.Controls.Add(Me.Reag122)
        Me.ReagentsTab.Controls.Add(Me.Reag123)
        Me.ReagentsTab.Controls.Add(Me.Reag124)
        Me.ReagentsTab.Controls.Add(Me.Reag125)
        Me.ReagentsTab.Controls.Add(Me.Reag126)
        Me.ReagentsTab.Controls.Add(Me.Reag127)
        Me.ReagentsTab.Controls.Add(Me.Reag128)
        Me.ReagentsTab.Controls.Add(Me.Reag129)
        Me.ReagentsTab.Controls.Add(Me.Reag130)
        Me.ReagentsTab.Controls.Add(Me.Reag131)
        Me.ReagentsTab.Controls.Add(Me.Reag132)
        Me.ReagentsTab.Controls.Add(Me.Reag133)
        Me.ReagentsTab.Controls.Add(Me.Reag134)
        Me.ReagentsTab.Controls.Add(Me.Reag135)
        Me.ReagentsTab.Controls.Add(Me.Reag136)
        Me.ReagentsTab.Controls.Add(Me.Reag137)
        Me.ReagentsTab.Controls.Add(Me.Reag138)
        Me.ReagentsTab.Controls.Add(Me.Reag139)
        Me.ReagentsTab.Controls.Add(Me.Reag140)
        Me.ReagentsTab.Controls.Add(Me.Reag141)
        Me.ReagentsTab.Controls.Add(Me.Reag142)
        Me.ReagentsTab.Controls.Add(Me.Reag143)
        Me.ReagentsTab.Controls.Add(Me.Reag144)
        Me.ReagentsTab.Controls.Add(Me.PanelControl4)
        Me.ReagentsTab.Controls.Add(Me.Reag288)
        Me.ReagentsTab.Controls.Add(Me.Reag287)
        Me.ReagentsTab.Controls.Add(Me.Reag286)
        Me.ReagentsTab.Controls.Add(Me.Reag285)
        Me.ReagentsTab.Controls.Add(Me.Reag284)
        Me.ReagentsTab.Controls.Add(Me.Reag283)
        Me.ReagentsTab.Controls.Add(Me.Reag282)
        Me.ReagentsTab.Controls.Add(Me.Reag281)
        Me.ReagentsTab.Controls.Add(Me.Reag280)
        Me.ReagentsTab.Controls.Add(Me.Reag279)
        Me.ReagentsTab.Controls.Add(Me.Reag278)
        Me.ReagentsTab.Controls.Add(Me.Reag277)
        Me.ReagentsTab.Controls.Add(Me.Reag276)
        Me.ReagentsTab.Controls.Add(Me.Reag275)
        Me.ReagentsTab.Controls.Add(Me.Reag274)
        Me.ReagentsTab.Controls.Add(Me.Reag273)
        Me.ReagentsTab.Controls.Add(Me.Reag272)
        Me.ReagentsTab.Controls.Add(Me.Reag271)
        Me.ReagentsTab.Controls.Add(Me.Reag270)
        Me.ReagentsTab.Controls.Add(Me.Reag269)
        Me.ReagentsTab.Controls.Add(Me.Reag268)
        Me.ReagentsTab.Controls.Add(Me.Reag267)
        Me.ReagentsTab.Controls.Add(Me.Reag266)
        Me.ReagentsTab.Controls.Add(Me.Reag265)
        Me.ReagentsTab.Controls.Add(Me.Reag264)
        Me.ReagentsTab.Controls.Add(Me.Reag263)
        Me.ReagentsTab.Controls.Add(Me.Reag262)
        Me.ReagentsTab.Controls.Add(Me.Reag261)
        Me.ReagentsTab.Controls.Add(Me.Reag260)
        Me.ReagentsTab.Controls.Add(Me.Reag259)
        Me.ReagentsTab.Controls.Add(Me.Reag258)
        Me.ReagentsTab.Controls.Add(Me.Reag257)
        Me.ReagentsTab.Controls.Add(Me.Reag256)
        Me.ReagentsTab.Controls.Add(Me.Reag255)
        Me.ReagentsTab.Controls.Add(Me.Reag254)
        Me.ReagentsTab.Controls.Add(Me.Reag253)
        Me.ReagentsTab.Controls.Add(Me.Reag252)
        Me.ReagentsTab.Controls.Add(Me.Reag251)
        Me.ReagentsTab.Controls.Add(Me.Reag250)
        Me.ReagentsTab.Controls.Add(Me.Reag249)
        Me.ReagentsTab.Controls.Add(Me.Reag248)
        Me.ReagentsTab.Controls.Add(Me.Reag247)
        Me.ReagentsTab.Controls.Add(Me.Reag246)
        Me.ReagentsTab.Controls.Add(Me.Reag245)
        Me.ReagentsTab.Name = "ReagentsTab"
        Me.ReagentsTab.Size = New System.Drawing.Size(762, 623)
        Me.ReagentsTab.Text = "Reagents"
        '
        'PanelControl9
        '
        Me.PanelControl9.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
        Me.PanelControl9.Appearance.Options.UseBackColor = true
        Me.PanelControl9.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl9.Location = New System.Drawing.Point(0, 593)
        Me.PanelControl9.Name = "PanelControl9"
        Me.PanelControl9.Size = New System.Drawing.Size(576, 34)
        Me.PanelControl9.TabIndex = 266
        '
        'Reag11
        '
        Me.Reag11.BackColor = System.Drawing.Color.Transparent
        Me.Reag11.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag11.Image = Nothing
        Me.Reag11.ImagePath = ""
        Me.Reag11.IsTransparentImage = false
        Me.Reag11.Location = New System.Drawing.Point(287, 547)
        Me.Reag11.Name = "Reag11"
        Me.Reag11.Rotation = 355
        Me.Reag11.ShowThrough = true
        Me.Reag11.Size = New System.Drawing.Size(37, 37)
        Me.Reag11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Reag11.TabIndex = 174
        Me.Reag11.TabStop = false
        Me.Reag11.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag11.WaitOnLoad = true
        '
        'Reag12
        '
        Me.Reag12.BackColor = System.Drawing.Color.Transparent
        Me.Reag12.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag12.Image = Nothing
        Me.Reag12.ImagePath = ""
        Me.Reag12.IsTransparentImage = false
        Me.Reag12.Location = New System.Drawing.Point(324, 541)
        Me.Reag12.Name = "Reag12"
        Me.Reag12.Rotation = 349
        Me.Reag12.ShowThrough = true
        Me.Reag12.Size = New System.Drawing.Size(37, 37)
        Me.Reag12.TabIndex = 175
        Me.Reag12.TabStop = false
        Me.Reag12.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag12.WaitOnLoad = true
        '
        'Reag13
        '
        Me.Reag13.BackColor = System.Drawing.Color.Transparent
        Me.Reag13.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag13.Image = Nothing
        Me.Reag13.ImagePath = ""
        Me.Reag13.IsTransparentImage = false
        Me.Reag13.Location = New System.Drawing.Point(360, 531)
        Me.Reag13.Name = "Reag13"
        Me.Reag13.Rotation = 341
        Me.Reag13.ShowThrough = true
        Me.Reag13.Size = New System.Drawing.Size(37, 37)
        Me.Reag13.TabIndex = 176
        Me.Reag13.TabStop = false
        Me.Reag13.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag13.WaitOnLoad = true
        '
        'Reag14
        '
        Me.Reag14.BackColor = System.Drawing.Color.Transparent
        Me.Reag14.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag14.Image = Nothing
        Me.Reag14.ImagePath = ""
        Me.Reag14.IsTransparentImage = false
        Me.Reag14.Location = New System.Drawing.Point(394, 514)
        Me.Reag14.Name = "Reag14"
        Me.Reag14.Rotation = 332
        Me.Reag14.ShowThrough = true
        Me.Reag14.Size = New System.Drawing.Size(37, 37)
        Me.Reag14.TabIndex = 177
        Me.Reag14.TabStop = false
        Me.Reag14.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag14.WaitOnLoad = true
        '
        'Reag15
        '
        Me.Reag15.BackColor = System.Drawing.Color.Transparent
        Me.Reag15.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag15.Image = Nothing
        Me.Reag15.ImagePath = ""
        Me.Reag15.IsTransparentImage = false
        Me.Reag15.Location = New System.Drawing.Point(426, 495)
        Me.Reag15.Name = "Reag15"
        Me.Reag15.Rotation = 325
        Me.Reag15.ShowThrough = true
        Me.Reag15.Size = New System.Drawing.Size(37, 37)
        Me.Reag15.TabIndex = 183
        Me.Reag15.TabStop = false
        Me.Reag15.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag15.WaitOnLoad = true
        '
        'Reag16
        '
        Me.Reag16.BackColor = System.Drawing.Color.Transparent
        Me.Reag16.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag16.Image = Nothing
        Me.Reag16.ImagePath = ""
        Me.Reag16.IsTransparentImage = false
        Me.Reag16.Location = New System.Drawing.Point(455, 470)
        Me.Reag16.Name = "Reag16"
        Me.Reag16.Rotation = 316
        Me.Reag16.ShowThrough = true
        Me.Reag16.Size = New System.Drawing.Size(37, 37)
        Me.Reag16.TabIndex = 184
        Me.Reag16.TabStop = false
        Me.Reag16.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag16.WaitOnLoad = true
        '
        'Reag17
        '
        Me.Reag17.BackColor = System.Drawing.Color.Transparent
        Me.Reag17.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag17.Image = Nothing
        Me.Reag17.ImagePath = ""
        Me.Reag17.IsTransparentImage = false
        Me.Reag17.Location = New System.Drawing.Point(481, 440)
        Me.Reag17.Name = "Reag17"
        Me.Reag17.Rotation = 306
        Me.Reag17.ShowThrough = true
        Me.Reag17.Size = New System.Drawing.Size(37, 37)
        Me.Reag17.TabIndex = 185
        Me.Reag17.TabStop = false
        Me.Reag17.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag17.WaitOnLoad = true
        '
        'Reag18
        '
        Me.Reag18.BackColor = System.Drawing.Color.Transparent
        Me.Reag18.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag18.Image = Nothing
        Me.Reag18.ImagePath = ""
        Me.Reag18.IsTransparentImage = false
        Me.Reag18.Location = New System.Drawing.Point(501, 408)
        Me.Reag18.Name = "Reag18"
        Me.Reag18.Rotation = 302
        Me.Reag18.ShowThrough = true
        Me.Reag18.Size = New System.Drawing.Size(37, 37)
        Me.Reag18.TabIndex = 186
        Me.Reag18.TabStop = false
        Me.Reag18.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag18.WaitOnLoad = true
        '
        'Reag19
        '
        Me.Reag19.BackColor = System.Drawing.Color.Transparent
        Me.Reag19.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag19.Image = Nothing
        Me.Reag19.ImagePath = ""
        Me.Reag19.IsTransparentImage = false
        Me.Reag19.Location = New System.Drawing.Point(517, 374)
        Me.Reag19.Name = "Reag19"
        Me.Reag19.Rotation = 292
        Me.Reag19.ShowThrough = true
        Me.Reag19.Size = New System.Drawing.Size(37, 37)
        Me.Reag19.TabIndex = 187
        Me.Reag19.TabStop = false
        Me.Reag19.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag19.WaitOnLoad = true
        '
        'Reag110
        '
        Me.Reag110.BackColor = System.Drawing.Color.Transparent
        Me.Reag110.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag110.Image = Nothing
        Me.Reag110.ImagePath = ""
        Me.Reag110.IsTransparentImage = false
        Me.Reag110.Location = New System.Drawing.Point(529, 337)
        Me.Reag110.Name = "Reag110"
        Me.Reag110.Rotation = 285
        Me.Reag110.ShowThrough = true
        Me.Reag110.Size = New System.Drawing.Size(37, 37)
        Me.Reag110.TabIndex = 188
        Me.Reag110.TabStop = false
        Me.Reag110.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag110.WaitOnLoad = true
        '
        'Reag111
        '
        Me.Reag111.BackColor = System.Drawing.Color.Transparent
        Me.Reag111.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag111.Image = Nothing
        Me.Reag111.ImagePath = ""
        Me.Reag111.IsTransparentImage = false
        Me.Reag111.Location = New System.Drawing.Point(534, 298)
        Me.Reag111.Name = "Reag111"
        Me.Reag111.Rotation = 277
        Me.Reag111.ShowThrough = true
        Me.Reag111.Size = New System.Drawing.Size(37, 37)
        Me.Reag111.TabIndex = 189
        Me.Reag111.TabStop = false
        Me.Reag111.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag111.WaitOnLoad = true
        '
        'Reag112
        '
        Me.Reag112.BackColor = System.Drawing.Color.Transparent
        Me.Reag112.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag112.Image = Nothing
        Me.Reag112.ImagePath = ""
        Me.Reag112.IsTransparentImage = false
        Me.Reag112.Location = New System.Drawing.Point(535, 261)
        Me.Reag112.Name = "Reag112"
        Me.Reag112.Rotation = 266
        Me.Reag112.ShowThrough = true
        Me.Reag112.Size = New System.Drawing.Size(37, 37)
        Me.Reag112.TabIndex = 190
        Me.Reag112.TabStop = false
        Me.Reag112.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag112.WaitOnLoad = true
        '
        'Reag113
        '
        Me.Reag113.BackColor = System.Drawing.Color.Transparent
        Me.Reag113.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag113.Image = Nothing
        Me.Reag113.ImagePath = ""
        Me.Reag113.IsTransparentImage = false
        Me.Reag113.Location = New System.Drawing.Point(530, 222)
        Me.Reag113.Name = "Reag113"
        Me.Reag113.Rotation = 257
        Me.Reag113.ShowThrough = true
        Me.Reag113.Size = New System.Drawing.Size(37, 37)
        Me.Reag113.TabIndex = 191
        Me.Reag113.TabStop = false
        Me.Reag113.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag113.WaitOnLoad = true
        '
        'Reag114
        '
        Me.Reag114.BackColor = System.Drawing.Color.Transparent
        Me.Reag114.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag114.Image = Nothing
        Me.Reag114.ImagePath = ""
        Me.Reag114.IsTransparentImage = false
        Me.Reag114.Location = New System.Drawing.Point(520, 184)
        Me.Reag114.Name = "Reag114"
        Me.Reag114.Rotation = 252
        Me.Reag114.ShowThrough = true
        Me.Reag114.Size = New System.Drawing.Size(37, 37)
        Me.Reag114.TabIndex = 192
        Me.Reag114.TabStop = false
        Me.Reag114.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag114.WaitOnLoad = true
        '
        'Reag115
        '
        Me.Reag115.BackColor = System.Drawing.Color.Transparent
        Me.Reag115.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag115.Image = Nothing
        Me.Reag115.ImagePath = ""
        Me.Reag115.IsTransparentImage = false
        Me.Reag115.Location = New System.Drawing.Point(502, 149)
        Me.Reag115.Name = "Reag115"
        Me.Reag115.Rotation = 241
        Me.Reag115.ShowThrough = true
        Me.Reag115.Size = New System.Drawing.Size(37, 37)
        Me.Reag115.TabIndex = 193
        Me.Reag115.TabStop = false
        Me.Reag115.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag115.WaitOnLoad = true
        '
        'Reag116
        '
        Me.Reag116.BackColor = System.Drawing.Color.Transparent
        Me.Reag116.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag116.Image = Nothing
        Me.Reag116.ImagePath = ""
        Me.Reag116.IsTransparentImage = false
        Me.Reag116.Location = New System.Drawing.Point(484, 116)
        Me.Reag116.Name = "Reag116"
        Me.Reag116.Rotation = 234
        Me.Reag116.ShowThrough = true
        Me.Reag116.Size = New System.Drawing.Size(37, 37)
        Me.Reag116.TabIndex = 194
        Me.Reag116.TabStop = false
        Me.Reag116.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag116.WaitOnLoad = true
        '
        'Reag117
        '
        Me.Reag117.BackColor = System.Drawing.Color.Transparent
        Me.Reag117.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag117.Image = Nothing
        Me.Reag117.ImagePath = ""
        Me.Reag117.IsTransparentImage = false
        Me.Reag117.Location = New System.Drawing.Point(460, 88)
        Me.Reag117.Name = "Reag117"
        Me.Reag117.Rotation = 228
        Me.Reag117.ShowThrough = true
        Me.Reag117.Size = New System.Drawing.Size(37, 37)
        Me.Reag117.TabIndex = 195
        Me.Reag117.TabStop = false
        Me.Reag117.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag117.WaitOnLoad = true
        '
        'Reag118
        '
        Me.Reag118.BackColor = System.Drawing.Color.Transparent
        Me.Reag118.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag118.Image = Nothing
        Me.Reag118.ImagePath = ""
        Me.Reag118.IsTransparentImage = false
        Me.Reag118.Location = New System.Drawing.Point(430, 62)
        Me.Reag118.Name = "Reag118"
        Me.Reag118.Rotation = 218
        Me.Reag118.ShowThrough = true
        Me.Reag118.Size = New System.Drawing.Size(37, 37)
        Me.Reag118.TabIndex = 196
        Me.Reag118.TabStop = false
        Me.Reag118.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag118.WaitOnLoad = true
        '
        'Reag119
        '
        Me.Reag119.BackColor = System.Drawing.Color.Transparent
        Me.Reag119.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag119.Image = Nothing
        Me.Reag119.ImagePath = ""
        Me.Reag119.IsTransparentImage = false
        Me.Reag119.Location = New System.Drawing.Point(399, 42)
        Me.Reag119.Name = "Reag119"
        Me.Reag119.Rotation = 212
        Me.Reag119.ShowThrough = true
        Me.Reag119.Size = New System.Drawing.Size(37, 37)
        Me.Reag119.TabIndex = 197
        Me.Reag119.TabStop = false
        Me.Reag119.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag119.WaitOnLoad = true
        '
        'Reag120
        '
        Me.Reag120.BackColor = System.Drawing.Color.Transparent
        Me.Reag120.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag120.Image = Nothing
        Me.Reag120.ImagePath = ""
        Me.Reag120.IsTransparentImage = false
        Me.Reag120.Location = New System.Drawing.Point(365, 24)
        Me.Reag120.Name = "Reag120"
        Me.Reag120.Rotation = 204
        Me.Reag120.ShowThrough = true
        Me.Reag120.Size = New System.Drawing.Size(37, 37)
        Me.Reag120.TabIndex = 198
        Me.Reag120.TabStop = false
        Me.Reag120.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag120.WaitOnLoad = true
        '
        'Reag121
        '
        Me.Reag121.BackColor = System.Drawing.Color.Transparent
        Me.Reag121.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag121.Image = Nothing
        Me.Reag121.ImagePath = ""
        Me.Reag121.IsTransparentImage = false
        Me.Reag121.Location = New System.Drawing.Point(329, 14)
        Me.Reag121.Name = "Reag121"
        Me.Reag121.Rotation = 195
        Me.Reag121.ShowThrough = true
        Me.Reag121.Size = New System.Drawing.Size(37, 37)
        Me.Reag121.TabIndex = 199
        Me.Reag121.TabStop = false
        Me.Reag121.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag121.WaitOnLoad = true
        '
        'Reag122
        '
        Me.Reag122.BackColor = System.Drawing.Color.Transparent
        Me.Reag122.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag122.Image = Nothing
        Me.Reag122.ImagePath = ""
        Me.Reag122.IsTransparentImage = false
        Me.Reag122.Location = New System.Drawing.Point(291, 8)
        Me.Reag122.Name = "Reag122"
        Me.Reag122.Rotation = 187
        Me.Reag122.ShowThrough = true
        Me.Reag122.Size = New System.Drawing.Size(37, 37)
        Me.Reag122.TabIndex = 200
        Me.Reag122.TabStop = false
        Me.Reag122.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag122.WaitOnLoad = true
        '
        'Reag123
        '
        Me.Reag123.BackColor = System.Drawing.Color.Transparent
        Me.Reag123.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag123.Image = Nothing
        Me.Reag123.ImagePath = ""
        Me.Reag123.IsTransparentImage = false
        Me.Reag123.Location = New System.Drawing.Point(254, 7)
        Me.Reag123.Name = "Reag123"
        Me.Reag123.Rotation = 179
        Me.Reag123.ShowThrough = true
        Me.Reag123.Size = New System.Drawing.Size(37, 37)
        Me.Reag123.TabIndex = 201
        Me.Reag123.TabStop = false
        Me.Reag123.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag123.WaitOnLoad = true
        '
        'Reag124
        '
        Me.Reag124.BackColor = System.Drawing.Color.Transparent
        Me.Reag124.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag124.Image = Nothing
        Me.Reag124.ImagePath = ""
        Me.Reag124.IsTransparentImage = false
        Me.Reag124.Location = New System.Drawing.Point(217, 12)
        Me.Reag124.Name = "Reag124"
        Me.Reag124.Rotation = 169
        Me.Reag124.ShowThrough = true
        Me.Reag124.Size = New System.Drawing.Size(37, 37)
        Me.Reag124.TabIndex = 202
        Me.Reag124.TabStop = false
        Me.Reag124.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag124.WaitOnLoad = true
        '
        'Reag125
        '
        Me.Reag125.BackColor = System.Drawing.Color.Transparent
        Me.Reag125.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag125.Image = Nothing
        Me.Reag125.ImagePath = ""
        Me.Reag125.IsTransparentImage = false
        Me.Reag125.Location = New System.Drawing.Point(179, 23)
        Me.Reag125.Name = "Reag125"
        Me.Reag125.Rotation = 162
        Me.Reag125.ShowThrough = true
        Me.Reag125.Size = New System.Drawing.Size(37, 37)
        Me.Reag125.TabIndex = 203
        Me.Reag125.TabStop = false
        Me.Reag125.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag125.WaitOnLoad = true
        '
        'Reag126
        '
        Me.Reag126.BackColor = System.Drawing.Color.Transparent
        Me.Reag126.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag126.Image = Nothing
        Me.Reag126.ImagePath = ""
        Me.Reag126.IsTransparentImage = false
        Me.Reag126.Location = New System.Drawing.Point(146, 38)
        Me.Reag126.Name = "Reag126"
        Me.Reag126.Rotation = 154
        Me.Reag126.ShowThrough = true
        Me.Reag126.Size = New System.Drawing.Size(37, 37)
        Me.Reag126.TabIndex = 204
        Me.Reag126.TabStop = false
        Me.Reag126.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag126.WaitOnLoad = true
        '
        'Reag127
        '
        Me.Reag127.BackColor = System.Drawing.Color.Transparent
        Me.Reag127.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag127.Image = Nothing
        Me.Reag127.ImagePath = ""
        Me.Reag127.IsTransparentImage = false
        Me.Reag127.Location = New System.Drawing.Point(113, 59)
        Me.Reag127.Name = "Reag127"
        Me.Reag127.Rotation = 145
        Me.Reag127.ShowThrough = true
        Me.Reag127.Size = New System.Drawing.Size(37, 37)
        Me.Reag127.TabIndex = 205
        Me.Reag127.TabStop = false
        Me.Reag127.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag127.WaitOnLoad = true
        '
        'Reag128
        '
        Me.Reag128.BackColor = System.Drawing.Color.Transparent
        Me.Reag128.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag128.Image = Nothing
        Me.Reag128.ImagePath = ""
        Me.Reag128.IsTransparentImage = false
        Me.Reag128.Location = New System.Drawing.Point(85, 84)
        Me.Reag128.Name = "Reag128"
        Me.Reag128.Rotation = 138
        Me.Reag128.ShowThrough = true
        Me.Reag128.Size = New System.Drawing.Size(37, 37)
        Me.Reag128.TabIndex = 206
        Me.Reag128.TabStop = false
        Me.Reag128.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag128.WaitOnLoad = true
        '
        'Reag129
        '
        Me.Reag129.BackColor = System.Drawing.Color.Transparent
        Me.Reag129.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag129.Image = Nothing
        Me.Reag129.ImagePath = ""
        Me.Reag129.IsTransparentImage = false
        Me.Reag129.Location = New System.Drawing.Point(60, 112)
        Me.Reag129.Name = "Reag129"
        Me.Reag129.Rotation = 126
        Me.Reag129.ShowThrough = true
        Me.Reag129.Size = New System.Drawing.Size(37, 37)
        Me.Reag129.TabIndex = 207
        Me.Reag129.TabStop = false
        Me.Reag129.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag129.WaitOnLoad = true
        '
        'Reag130
        '
        Me.Reag130.BackColor = System.Drawing.Color.Transparent
        Me.Reag130.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag130.Image = Nothing
        Me.Reag130.ImagePath = ""
        Me.Reag130.IsTransparentImage = false
        Me.Reag130.Location = New System.Drawing.Point(38, 145)
        Me.Reag130.Name = "Reag130"
        Me.Reag130.Rotation = 120
        Me.Reag130.ShowThrough = true
        Me.Reag130.Size = New System.Drawing.Size(37, 37)
        Me.Reag130.TabIndex = 208
        Me.Reag130.TabStop = false
        Me.Reag130.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag130.WaitOnLoad = true
        '
        'Reag131
        '
        Me.Reag131.BackColor = System.Drawing.Color.Transparent
        Me.Reag131.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag131.Image = Nothing
        Me.Reag131.ImagePath = ""
        Me.Reag131.IsTransparentImage = false
        Me.Reag131.Location = New System.Drawing.Point(23, 180)
        Me.Reag131.Name = "Reag131"
        Me.Reag131.Rotation = 111
        Me.Reag131.ShowThrough = true
        Me.Reag131.Size = New System.Drawing.Size(37, 37)
        Me.Reag131.TabIndex = 209
        Me.Reag131.TabStop = false
        Me.Reag131.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag131.WaitOnLoad = true
        '
        'Reag132
        '
        Me.Reag132.BackColor = System.Drawing.Color.Transparent
        Me.Reag132.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag132.Image = Nothing
        Me.Reag132.ImagePath = ""
        Me.Reag132.IsTransparentImage = false
        Me.Reag132.Location = New System.Drawing.Point(11, 217)
        Me.Reag132.Name = "Reag132"
        Me.Reag132.Rotation = 101
        Me.Reag132.ShowThrough = true
        Me.Reag132.Size = New System.Drawing.Size(37, 37)
        Me.Reag132.TabIndex = 210
        Me.Reag132.TabStop = false
        Me.Reag132.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag132.WaitOnLoad = true
        '
        'Reag133
        '
        Me.Reag133.BackColor = System.Drawing.Color.Transparent
        Me.Reag133.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag133.Image = Nothing
        Me.Reag133.ImagePath = ""
        Me.Reag133.IsTransparentImage = false
        Me.Reag133.Location = New System.Drawing.Point(6, 255)
        Me.Reag133.Name = "Reag133"
        Me.Reag133.Rotation = 95
        Me.Reag133.ShowThrough = true
        Me.Reag133.Size = New System.Drawing.Size(37, 37)
        Me.Reag133.TabIndex = 211
        Me.Reag133.TabStop = false
        Me.Reag133.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag133.WaitOnLoad = true
        '
        'Reag134
        '
        Me.Reag134.BackColor = System.Drawing.Color.Transparent
        Me.Reag134.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag134.Image = Nothing
        Me.Reag134.ImagePath = ""
        Me.Reag134.IsTransparentImage = false
        Me.Reag134.Location = New System.Drawing.Point(7, 294)
        Me.Reag134.Name = "Reag134"
        Me.Reag134.Rotation = 86
        Me.Reag134.ShowThrough = true
        Me.Reag134.Size = New System.Drawing.Size(37, 37)
        Me.Reag134.TabIndex = 212
        Me.Reag134.TabStop = false
        Me.Reag134.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag134.WaitOnLoad = true
        '
        'Reag135
        '
        Me.Reag135.BackColor = System.Drawing.Color.Transparent
        Me.Reag135.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag135.Image = Nothing
        Me.Reag135.ImagePath = ""
        Me.Reag135.IsTransparentImage = false
        Me.Reag135.Location = New System.Drawing.Point(10, 332)
        Me.Reag135.Name = "Reag135"
        Me.Reag135.Rotation = 78
        Me.Reag135.ShowThrough = true
        Me.Reag135.Size = New System.Drawing.Size(37, 37)
        Me.Reag135.TabIndex = 213
        Me.Reag135.TabStop = false
        Me.Reag135.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag135.WaitOnLoad = true
        '
        'Reag136
        '
        Me.Reag136.BackColor = System.Drawing.Color.Transparent
        Me.Reag136.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag136.Image = Nothing
        Me.Reag136.ImagePath = ""
        Me.Reag136.IsTransparentImage = false
        Me.Reag136.Location = New System.Drawing.Point(21, 368)
        Me.Reag136.Name = "Reag136"
        Me.Reag136.Rotation = 70
        Me.Reag136.ShowThrough = true
        Me.Reag136.Size = New System.Drawing.Size(37, 37)
        Me.Reag136.TabIndex = 214
        Me.Reag136.TabStop = false
        Me.Reag136.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag136.WaitOnLoad = true
        '
        'Reag137
        '
        Me.Reag137.BackColor = System.Drawing.Color.Transparent
        Me.Reag137.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag137.Image = Nothing
        Me.Reag137.ImagePath = ""
        Me.Reag137.IsTransparentImage = false
        Me.Reag137.Location = New System.Drawing.Point(36, 403)
        Me.Reag137.Name = "Reag137"
        Me.Reag137.Rotation = 62
        Me.Reag137.ShowThrough = true
        Me.Reag137.Size = New System.Drawing.Size(37, 37)
        Me.Reag137.TabIndex = 215
        Me.Reag137.TabStop = false
        Me.Reag137.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag137.WaitOnLoad = true
        '
        'Reag138
        '
        Me.Reag138.BackColor = System.Drawing.Color.Transparent
        Me.Reag138.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag138.Image = Nothing
        Me.Reag138.ImagePath = ""
        Me.Reag138.IsTransparentImage = false
        Me.Reag138.Location = New System.Drawing.Point(57, 436)
        Me.Reag138.Name = "Reag138"
        Me.Reag138.Rotation = 55
        Me.Reag138.ShowThrough = true
        Me.Reag138.Size = New System.Drawing.Size(37, 37)
        Me.Reag138.TabIndex = 216
        Me.Reag138.TabStop = false
        Me.Reag138.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag138.WaitOnLoad = true
        '
        'Reag139
        '
        Me.Reag139.BackColor = System.Drawing.Color.Transparent
        Me.Reag139.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag139.Image = Nothing
        Me.Reag139.ImagePath = ""
        Me.Reag139.IsTransparentImage = false
        Me.Reag139.Location = New System.Drawing.Point(81, 466)
        Me.Reag139.Name = "Reag139"
        Me.Reag139.Rotation = 46
        Me.Reag139.ShowThrough = true
        Me.Reag139.Size = New System.Drawing.Size(37, 37)
        Me.Reag139.TabIndex = 217
        Me.Reag139.TabStop = false
        Me.Reag139.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag139.WaitOnLoad = true
        '
        'Reag140
        '
        Me.Reag140.BackColor = System.Drawing.Color.Transparent
        Me.Reag140.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag140.Image = Nothing
        Me.Reag140.ImagePath = ""
        Me.Reag140.IsTransparentImage = false
        Me.Reag140.Location = New System.Drawing.Point(109, 492)
        Me.Reag140.Name = "Reag140"
        Me.Reag140.Rotation = 36
        Me.Reag140.ShowThrough = true
        Me.Reag140.Size = New System.Drawing.Size(37, 37)
        Me.Reag140.TabIndex = 218
        Me.Reag140.TabStop = false
        Me.Reag140.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag140.WaitOnLoad = true
        '
        'Reag141
        '
        Me.Reag141.BackColor = System.Drawing.Color.Transparent
        Me.Reag141.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag141.Image = Nothing
        Me.Reag141.ImagePath = ""
        Me.Reag141.IsTransparentImage = false
        Me.Reag141.Location = New System.Drawing.Point(141, 513)
        Me.Reag141.Name = "Reag141"
        Me.Reag141.Rotation = 32
        Me.Reag141.ShowThrough = true
        Me.Reag141.Size = New System.Drawing.Size(37, 37)
        Me.Reag141.TabIndex = 219
        Me.Reag141.TabStop = false
        Me.Reag141.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag141.WaitOnLoad = true
        '
        'Reag142
        '
        Me.Reag142.BackColor = System.Drawing.Color.Transparent
        Me.Reag142.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag142.Image = Nothing
        Me.Reag142.ImagePath = ""
        Me.Reag142.IsTransparentImage = false
        Me.Reag142.Location = New System.Drawing.Point(175, 529)
        Me.Reag142.Name = "Reag142"
        Me.Reag142.Rotation = 23
        Me.Reag142.ShowThrough = true
        Me.Reag142.Size = New System.Drawing.Size(37, 37)
        Me.Reag142.TabIndex = 220
        Me.Reag142.TabStop = false
        Me.Reag142.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag142.WaitOnLoad = true
        '
        'Reag143
        '
        Me.Reag143.BackColor = System.Drawing.Color.Transparent
        Me.Reag143.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag143.Image = Nothing
        Me.Reag143.ImagePath = ""
        Me.Reag143.IsTransparentImage = false
        Me.Reag143.Location = New System.Drawing.Point(212, 541)
        Me.Reag143.Name = "Reag143"
        Me.Reag143.Rotation = 14
        Me.Reag143.ShowThrough = true
        Me.Reag143.Size = New System.Drawing.Size(37, 37)
        Me.Reag143.TabIndex = 221
        Me.Reag143.TabStop = false
        Me.Reag143.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag143.WaitOnLoad = true
        '
        'Reag144
        '
        Me.Reag144.BackColor = System.Drawing.Color.Transparent
        Me.Reag144.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag144.Image = Nothing
        Me.Reag144.ImagePath = ""
        Me.Reag144.IsTransparentImage = false
        Me.Reag144.Location = New System.Drawing.Point(249, 546)
        Me.Reag144.Name = "Reag144"
        Me.Reag144.Rotation = 6
        Me.Reag144.ShowThrough = true
        Me.Reag144.Size = New System.Drawing.Size(37, 37)
        Me.Reag144.TabIndex = 222
        Me.Reag144.TabStop = false
        Me.Reag144.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag144.WaitOnLoad = true
        '
        'PanelControl4
        '
        Me.PanelControl4.Appearance.BackColor = System.Drawing.Color.White
        Me.PanelControl4.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl4.Appearance.Options.UseBackColor = true
        Me.PanelControl4.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl4.Controls.Add(Me.PanelControl3)
        Me.PanelControl4.Location = New System.Drawing.Point(575, -4)
        Me.PanelControl4.Name = "PanelControl4"
        Me.PanelControl4.Size = New System.Drawing.Size(190, 627)
        Me.PanelControl4.TabIndex = 263
        '
        'PanelControl3
        '
        Me.PanelControl3.Appearance.BackColor = System.Drawing.Color.White
        Me.PanelControl3.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl3.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.PanelControl3.Appearance.Options.UseBackColor = true
        Me.PanelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.PanelControl3.Controls.Add(Me.LegendReagentsGroupBox)
        Me.PanelControl3.Controls.Add(Me.bsReagentsPositionInfoGroupBox)
        Me.PanelControl3.Location = New System.Drawing.Point(3, 4)
        Me.PanelControl3.Name = "PanelControl3"
        Me.PanelControl3.Size = New System.Drawing.Size(183, 620)
        Me.PanelControl3.TabIndex = 262
        '
        'LegendReagentsGroupBox
        '
        Me.LegendReagentsGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.LegendReagentsGroupBox.Controls.Add(Me.LegendUnknownImage)
        Me.LegendReagentsGroupBox.Controls.Add(Me.UnknownLabel)
        Me.LegendReagentsGroupBox.Controls.Add(Me.LegendBarCodeErrorRGImage)
        Me.LegendReagentsGroupBox.Controls.Add(Me.BarcodeErrorRGLabel)
        Me.LegendReagentsGroupBox.Controls.Add(Me.LowVolPictureBox)
        Me.LegendReagentsGroupBox.Controls.Add(Me.ReagentPictureBox)
        Me.LegendReagentsGroupBox.Controls.Add(Me.LegReagLowVolLabel)
        Me.LegendReagentsGroupBox.Controls.Add(Me.LegReagentLabel)
        Me.LegendReagentsGroupBox.Controls.Add(Me.LegReagAdditionalSol)
        Me.LegendReagentsGroupBox.Controls.Add(Me.LegReagNoInUseLabel)
        Me.LegendReagentsGroupBox.Controls.Add(Me.LegReagentSelLabel)
        Me.LegendReagentsGroupBox.Controls.Add(Me.LegReagDepleteLabel)
        Me.LegendReagentsGroupBox.Controls.Add(Me.AdditionalSolPictureBox)
        Me.LegendReagentsGroupBox.Controls.Add(Me.NoInUsePictureBox)
        Me.LegendReagentsGroupBox.Controls.Add(Me.SelectedPictureBox)
        Me.LegendReagentsGroupBox.Controls.Add(Me.bsDepletedPictureBox)
        Me.LegendReagentsGroupBox.Controls.Add(Me.bsReagentsLegendLabel)
        Me.LegendReagentsGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.LegendReagentsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.LegendReagentsGroupBox.Location = New System.Drawing.Point(4, 367)
        Me.LegendReagentsGroupBox.Name = "LegendReagentsGroupBox"
        Me.LegendReagentsGroupBox.Size = New System.Drawing.Size(175, 248)
        Me.LegendReagentsGroupBox.TabIndex = 28
        Me.LegendReagentsGroupBox.TabStop = false
        Me.LegendReagentsGroupBox.Visible = false
        '
        'LegendUnknownImage
        '
        Me.LegendUnknownImage.InitialImage = CType(resources.GetObject("LegendUnknownImage.InitialImage"),System.Drawing.Image)
        Me.LegendUnknownImage.Location = New System.Drawing.Point(3, 194)
        Me.LegendUnknownImage.Name = "LegendUnknownImage"
        Me.LegendUnknownImage.PositionNumber = 0
        Me.LegendUnknownImage.Size = New System.Drawing.Size(20, 20)
        Me.LegendUnknownImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.LegendUnknownImage.TabIndex = 39
        Me.LegendUnknownImage.TabStop = false
        '
        'UnknownLabel
        '
        Me.UnknownLabel.BackColor = System.Drawing.Color.Transparent
        Me.UnknownLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.UnknownLabel.ForeColor = System.Drawing.Color.Black
        Me.UnknownLabel.Location = New System.Drawing.Point(24, 197)
        Me.UnknownLabel.Name = "UnknownLabel"
        Me.UnknownLabel.Size = New System.Drawing.Size(149, 13)
        Me.UnknownLabel.TabIndex = 38
        Me.UnknownLabel.Text = "*Unknown"
        Me.UnknownLabel.Title = false
        '
        'LegendBarCodeErrorRGImage
        '
        Me.LegendBarCodeErrorRGImage.InitialImage = CType(resources.GetObject("LegendBarCodeErrorRGImage.InitialImage"),System.Drawing.Image)
        Me.LegendBarCodeErrorRGImage.Location = New System.Drawing.Point(3, 168)
        Me.LegendBarCodeErrorRGImage.Name = "LegendBarCodeErrorRGImage"
        Me.LegendBarCodeErrorRGImage.PositionNumber = 0
        Me.LegendBarCodeErrorRGImage.Size = New System.Drawing.Size(20, 20)
        Me.LegendBarCodeErrorRGImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.LegendBarCodeErrorRGImage.TabIndex = 37
        Me.LegendBarCodeErrorRGImage.TabStop = false
        '
        'BarcodeErrorRGLabel
        '
        Me.BarcodeErrorRGLabel.BackColor = System.Drawing.Color.Transparent
        Me.BarcodeErrorRGLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BarcodeErrorRGLabel.ForeColor = System.Drawing.Color.Black
        Me.BarcodeErrorRGLabel.Location = New System.Drawing.Point(24, 171)
        Me.BarcodeErrorRGLabel.Name = "BarcodeErrorRGLabel"
        Me.BarcodeErrorRGLabel.Size = New System.Drawing.Size(149, 13)
        Me.BarcodeErrorRGLabel.TabIndex = 36
        Me.BarcodeErrorRGLabel.Text = "*BarCode Error"
        Me.BarcodeErrorRGLabel.Title = false
        '
        'LowVolPictureBox
        '
        Me.LowVolPictureBox.InitialImage = CType(resources.GetObject("LowVolPictureBox.InitialImage"),System.Drawing.Image)
        Me.LowVolPictureBox.Location = New System.Drawing.Point(3, 116)
        Me.LowVolPictureBox.Name = "LowVolPictureBox"
        Me.LowVolPictureBox.PositionNumber = 0
        Me.LowVolPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.LowVolPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.LowVolPictureBox.TabIndex = 35
        Me.LowVolPictureBox.TabStop = false
        '
        'ReagentPictureBox
        '
        Me.ReagentPictureBox.InitialImage = CType(resources.GetObject("ReagentPictureBox.InitialImage"),System.Drawing.Image)
        Me.ReagentPictureBox.Location = New System.Drawing.Point(3, 38)
        Me.ReagentPictureBox.Name = "ReagentPictureBox"
        Me.ReagentPictureBox.PositionNumber = 0
        Me.ReagentPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.ReagentPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.ReagentPictureBox.TabIndex = 34
        Me.ReagentPictureBox.TabStop = false
        '
        'LegReagLowVolLabel
        '
        Me.LegReagLowVolLabel.BackColor = System.Drawing.Color.Transparent
        Me.LegReagLowVolLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegReagLowVolLabel.ForeColor = System.Drawing.Color.Black
        Me.LegReagLowVolLabel.Location = New System.Drawing.Point(24, 119)
        Me.LegReagLowVolLabel.Name = "LegReagLowVolLabel"
        Me.LegReagLowVolLabel.Size = New System.Drawing.Size(149, 13)
        Me.LegReagLowVolLabel.TabIndex = 33
        Me.LegReagLowVolLabel.Text = "Low Volume"
        Me.LegReagLowVolLabel.Title = false
        '
        'LegReagentLabel
        '
        Me.LegReagentLabel.BackColor = System.Drawing.Color.Transparent
        Me.LegReagentLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegReagentLabel.ForeColor = System.Drawing.Color.Black
        Me.LegReagentLabel.Location = New System.Drawing.Point(24, 41)
        Me.LegReagentLabel.Name = "LegReagentLabel"
        Me.LegReagentLabel.Size = New System.Drawing.Size(149, 13)
        Me.LegReagentLabel.TabIndex = 32
        Me.LegReagentLabel.Text = "Reagent"
        Me.LegReagentLabel.Title = false
        '
        'LegReagAdditionalSol
        '
        Me.LegReagAdditionalSol.BackColor = System.Drawing.Color.Transparent
        Me.LegReagAdditionalSol.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegReagAdditionalSol.ForeColor = System.Drawing.Color.Black
        Me.LegReagAdditionalSol.Location = New System.Drawing.Point(24, 67)
        Me.LegReagAdditionalSol.Name = "LegReagAdditionalSol"
        Me.LegReagAdditionalSol.Size = New System.Drawing.Size(149, 13)
        Me.LegReagAdditionalSol.TabIndex = 31
        Me.LegReagAdditionalSol.Text = "Additional Solutions"
        Me.LegReagAdditionalSol.Title = false
        '
        'LegReagNoInUseLabel
        '
        Me.LegReagNoInUseLabel.BackColor = System.Drawing.Color.Transparent
        Me.LegReagNoInUseLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegReagNoInUseLabel.ForeColor = System.Drawing.Color.Black
        Me.LegReagNoInUseLabel.Location = New System.Drawing.Point(24, 145)
        Me.LegReagNoInUseLabel.Name = "LegReagNoInUseLabel"
        Me.LegReagNoInUseLabel.Size = New System.Drawing.Size(149, 13)
        Me.LegReagNoInUseLabel.TabIndex = 30
        Me.LegReagNoInUseLabel.Text = "No In Use"
        Me.LegReagNoInUseLabel.Title = false
        '
        'LegReagentSelLabel
        '
        Me.LegReagentSelLabel.BackColor = System.Drawing.Color.Transparent
        Me.LegReagentSelLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegReagentSelLabel.ForeColor = System.Drawing.Color.Black
        Me.LegReagentSelLabel.Location = New System.Drawing.Point(24, 223)
        Me.LegReagentSelLabel.Name = "LegReagentSelLabel"
        Me.LegReagentSelLabel.Size = New System.Drawing.Size(149, 13)
        Me.LegReagentSelLabel.TabIndex = 29
        Me.LegReagentSelLabel.Text = "Selected"
        Me.LegReagentSelLabel.Title = false
        '
        'LegReagDepleteLabel
        '
        Me.LegReagDepleteLabel.BackColor = System.Drawing.Color.Transparent
        Me.LegReagDepleteLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegReagDepleteLabel.ForeColor = System.Drawing.Color.Black
        Me.LegReagDepleteLabel.Location = New System.Drawing.Point(24, 93)
        Me.LegReagDepleteLabel.Name = "LegReagDepleteLabel"
        Me.LegReagDepleteLabel.Size = New System.Drawing.Size(149, 13)
        Me.LegReagDepleteLabel.TabIndex = 26
        Me.LegReagDepleteLabel.Text = "Depleted"
        Me.LegReagDepleteLabel.Title = false
        '
        'AdditionalSolPictureBox
        '
        Me.AdditionalSolPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.AdditionalSolPictureBox.ErrorImage = Nothing
        Me.AdditionalSolPictureBox.InitialImage = CType(resources.GetObject("AdditionalSolPictureBox.InitialImage"),System.Drawing.Image)
        Me.AdditionalSolPictureBox.Location = New System.Drawing.Point(3, 64)
        Me.AdditionalSolPictureBox.Name = "AdditionalSolPictureBox"
        Me.AdditionalSolPictureBox.PositionNumber = 0
        Me.AdditionalSolPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.AdditionalSolPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.AdditionalSolPictureBox.TabIndex = 27
        Me.AdditionalSolPictureBox.TabStop = false
        '
        'NoInUsePictureBox
        '
        Me.NoInUsePictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.NoInUsePictureBox.ErrorImage = Nothing
        Me.NoInUsePictureBox.InitialImage = CType(resources.GetObject("NoInUsePictureBox.InitialImage"),System.Drawing.Image)
        Me.NoInUsePictureBox.Location = New System.Drawing.Point(3, 142)
        Me.NoInUsePictureBox.Name = "NoInUsePictureBox"
        Me.NoInUsePictureBox.PositionNumber = 0
        Me.NoInUsePictureBox.Size = New System.Drawing.Size(20, 20)
        Me.NoInUsePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.NoInUsePictureBox.TabIndex = 28
        Me.NoInUsePictureBox.TabStop = false
        '
        'SelectedPictureBox
        '
        Me.SelectedPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.SelectedPictureBox.ErrorImage = Nothing
        Me.SelectedPictureBox.InitialImage = CType(resources.GetObject("SelectedPictureBox.InitialImage"),System.Drawing.Image)
        Me.SelectedPictureBox.Location = New System.Drawing.Point(3, 220)
        Me.SelectedPictureBox.Name = "SelectedPictureBox"
        Me.SelectedPictureBox.PositionNumber = 0
        Me.SelectedPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.SelectedPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.SelectedPictureBox.TabIndex = 27
        Me.SelectedPictureBox.TabStop = false
        '
        'bsDepletedPictureBox
        '
        Me.bsDepletedPictureBox.InitialImage = CType(resources.GetObject("bsDepletedPictureBox.InitialImage"),System.Drawing.Image)
        Me.bsDepletedPictureBox.Location = New System.Drawing.Point(3, 90)
        Me.bsDepletedPictureBox.Name = "bsDepletedPictureBox"
        Me.bsDepletedPictureBox.PositionNumber = 0
        Me.bsDepletedPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsDepletedPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsDepletedPictureBox.TabIndex = 26
        Me.bsDepletedPictureBox.TabStop = false
        '
        'bsReagentsLegendLabel
        '
        Me.bsReagentsLegendLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsReagentsLegendLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsReagentsLegendLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsLegendLabel.Location = New System.Drawing.Point(4, 12)
        Me.bsReagentsLegendLabel.Name = "bsReagentsLegendLabel"
        Me.bsReagentsLegendLabel.Size = New System.Drawing.Size(167, 19)
        Me.bsReagentsLegendLabel.TabIndex = 25
        Me.bsReagentsLegendLabel.Text = "Legend"
        Me.bsReagentsLegendLabel.Title = true
        '
        'bsReagentsPositionInfoGroupBox
        '
        Me.bsReagentsPositionInfoGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsStatusTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsStatusLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsCellTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsCellLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsTeststLeftTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsCurrentVolTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsTestsLeftLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsCurrentVolLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsBottleSizeComboBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsBottleSizeLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsExpirationDateTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsPositionInfoLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsMoveLastPositionButton)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsBarCodeTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsTestNameTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsIncreaseButton)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsNumberTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsDecreaseButton)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentNameTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsContentTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsMoveFirstPositionButton)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsDiskNameTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsExpirationDateLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsBarCodeLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsTestNameLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsNumberLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentNameLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsContentLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsDiskNameLabel)
        Me.bsReagentsPositionInfoGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsReagentsPositionInfoGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsPositionInfoGroupBox.Location = New System.Drawing.Point(4, -3)
        Me.bsReagentsPositionInfoGroupBox.Name = "bsReagentsPositionInfoGroupBox"
        Me.bsReagentsPositionInfoGroupBox.Size = New System.Drawing.Size(175, 370)
        Me.bsReagentsPositionInfoGroupBox.TabIndex = 4
        Me.bsReagentsPositionInfoGroupBox.TabStop = false
        '
        'bsReagentsStatusTextBox
        '
        Me.bsReagentsStatusTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsStatusTextBox.DecimalsValues = false
        Me.bsReagentsStatusTextBox.Enabled = false
        Me.bsReagentsStatusTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsStatusTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsStatusTextBox.IsNumeric = false
        Me.bsReagentsStatusTextBox.Location = New System.Drawing.Point(5, 314)
        Me.bsReagentsStatusTextBox.Mandatory = false
        Me.bsReagentsStatusTextBox.Name = "bsReagentsStatusTextBox"
        Me.bsReagentsStatusTextBox.ReadOnly = true
        Me.bsReagentsStatusTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsReagentsStatusTextBox.TabIndex = 33
        Me.bsReagentsStatusTextBox.TabStop = false
        '
        'bsReagentsStatusLabel
        '
        Me.bsReagentsStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsStatusLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsStatusLabel.Location = New System.Drawing.Point(2, 299)
        Me.bsReagentsStatusLabel.Name = "bsReagentsStatusLabel"
        Me.bsReagentsStatusLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsReagentsStatusLabel.TabIndex = 34
        Me.bsReagentsStatusLabel.Text = "Status"
        Me.bsReagentsStatusLabel.Title = false
        '
        'bsReagentsCellTextBox
        '
        Me.bsReagentsCellTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsCellTextBox.DecimalsValues = false
        Me.bsReagentsCellTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsCellTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsCellTextBox.IsNumeric = false
        Me.bsReagentsCellTextBox.Location = New System.Drawing.Point(123, 47)
        Me.bsReagentsCellTextBox.Mandatory = false
        Me.bsReagentsCellTextBox.MaxLength = 3
        Me.bsReagentsCellTextBox.Name = "bsReagentsCellTextBox"
        Me.bsReagentsCellTextBox.ReadOnly = true
        Me.bsReagentsCellTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsReagentsCellTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsReagentsCellTextBox.TabIndex = 32
        Me.bsReagentsCellTextBox.TabStop = false
        Me.bsReagentsCellTextBox.WordWrap = false
        '
        'bsReagentsCellLabel
        '
        Me.bsReagentsCellLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsCellLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsCellLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsCellLabel.Location = New System.Drawing.Point(120, 33)
        Me.bsReagentsCellLabel.Name = "bsReagentsCellLabel"
        Me.bsReagentsCellLabel.Size = New System.Drawing.Size(52, 13)
        Me.bsReagentsCellLabel.TabIndex = 31
        Me.bsReagentsCellLabel.Text = "Cell"
        Me.bsReagentsCellLabel.Title = false
        '
        'bsTeststLeftTextBox
        '
        Me.bsTeststLeftTextBox.BackColor = System.Drawing.Color.White
        Me.bsTeststLeftTextBox.DecimalsValues = false
        Me.bsTeststLeftTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTeststLeftTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsTeststLeftTextBox.IsNumeric = false
        Me.bsTeststLeftTextBox.Location = New System.Drawing.Point(85, 275)
        Me.bsTeststLeftTextBox.Mandatory = false
        Me.bsTeststLeftTextBox.Name = "bsTeststLeftTextBox"
        Me.bsTeststLeftTextBox.ReadOnly = true
        Me.bsTeststLeftTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsTeststLeftTextBox.Size = New System.Drawing.Size(83, 21)
        Me.bsTeststLeftTextBox.TabIndex = 10
        Me.bsTeststLeftTextBox.TabStop = false
        Me.bsTeststLeftTextBox.WordWrap = false
        '
        'bsCurrentVolTextBox
        '
        Me.bsCurrentVolTextBox.BackColor = System.Drawing.Color.White
        Me.bsCurrentVolTextBox.DecimalsValues = false
        Me.bsCurrentVolTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCurrentVolTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsCurrentVolTextBox.IsNumeric = false
        Me.bsCurrentVolTextBox.Location = New System.Drawing.Point(5, 275)
        Me.bsCurrentVolTextBox.Mandatory = false
        Me.bsCurrentVolTextBox.MaxLength = 20
        Me.bsCurrentVolTextBox.Name = "bsCurrentVolTextBox"
        Me.bsCurrentVolTextBox.ReadOnly = true
        Me.bsCurrentVolTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsCurrentVolTextBox.Size = New System.Drawing.Size(76, 21)
        Me.bsCurrentVolTextBox.TabIndex = 9
        Me.bsCurrentVolTextBox.TabStop = false
        Me.bsCurrentVolTextBox.WordWrap = false
        '
        'bsTestsLeftLabel
        '
        Me.bsTestsLeftLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTestsLeftLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestsLeftLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestsLeftLabel.Location = New System.Drawing.Point(82, 261)
        Me.bsTestsLeftLabel.Name = "bsTestsLeftLabel"
        Me.bsTestsLeftLabel.Size = New System.Drawing.Size(91, 13)
        Me.bsTestsLeftLabel.TabIndex = 30
        Me.bsTestsLeftLabel.Text = "Tests Left"
        Me.bsTestsLeftLabel.Title = false
        '
        'bsCurrentVolLabel
        '
        Me.bsCurrentVolLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsCurrentVolLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCurrentVolLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCurrentVolLabel.Location = New System.Drawing.Point(2, 261)
        Me.bsCurrentVolLabel.Name = "bsCurrentVolLabel"
        Me.bsCurrentVolLabel.Size = New System.Drawing.Size(81, 13)
        Me.bsCurrentVolLabel.TabIndex = 29
        Me.bsCurrentVolLabel.Text = "Current Vol."
        Me.bsCurrentVolLabel.Title = false
        '
        'bsBottleSizeComboBox
        '
        Me.bsBottleSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsBottleSizeComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsBottleSizeComboBox.FormattingEnabled = true
        Me.bsBottleSizeComboBox.Location = New System.Drawing.Point(85, 237)
        Me.bsBottleSizeComboBox.MaxLength = 25
        Me.bsBottleSizeComboBox.Name = "bsBottleSizeComboBox"
        Me.bsBottleSizeComboBox.Size = New System.Drawing.Size(83, 21)
        Me.bsBottleSizeComboBox.TabIndex = 8
        '
        'bsBottleSizeLabel
        '
        Me.bsBottleSizeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsBottleSizeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsBottleSizeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsBottleSizeLabel.Location = New System.Drawing.Point(82, 223)
        Me.bsBottleSizeLabel.Name = "bsBottleSizeLabel"
        Me.bsBottleSizeLabel.Size = New System.Drawing.Size(91, 13)
        Me.bsBottleSizeLabel.TabIndex = 27
        Me.bsBottleSizeLabel.Text = "Bottle Size"
        Me.bsBottleSizeLabel.Title = false
        '
        'bsExpirationDateTextBox
        '
        Me.bsExpirationDateTextBox.BackColor = System.Drawing.Color.White
        Me.bsExpirationDateTextBox.DecimalsValues = false
        Me.bsExpirationDateTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsExpirationDateTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsExpirationDateTextBox.IsNumeric = false
        Me.bsExpirationDateTextBox.Location = New System.Drawing.Point(5, 237)
        Me.bsExpirationDateTextBox.Mandatory = false
        Me.bsExpirationDateTextBox.MaxLength = 20
        Me.bsExpirationDateTextBox.Name = "bsExpirationDateTextBox"
        Me.bsExpirationDateTextBox.ReadOnly = true
        Me.bsExpirationDateTextBox.Size = New System.Drawing.Size(75, 21)
        Me.bsExpirationDateTextBox.TabIndex = 7
        Me.bsExpirationDateTextBox.TabStop = false
        Me.bsExpirationDateTextBox.WordWrap = false
        '
        'bsReagentsPositionInfoLabel
        '
        Me.bsReagentsPositionInfoLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsReagentsPositionInfoLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsReagentsPositionInfoLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsPositionInfoLabel.Location = New System.Drawing.Point(4, 12)
        Me.bsReagentsPositionInfoLabel.Name = "bsReagentsPositionInfoLabel"
        Me.bsReagentsPositionInfoLabel.Size = New System.Drawing.Size(167, 19)
        Me.bsReagentsPositionInfoLabel.TabIndex = 25
        Me.bsReagentsPositionInfoLabel.Text = "Position Information"
        Me.bsReagentsPositionInfoLabel.Title = true
        '
        'bsReagentsMoveLastPositionButton
        '
        Me.bsReagentsMoveLastPositionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReagentsMoveLastPositionButton.Enabled = false
        Me.bsReagentsMoveLastPositionButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsReagentsMoveLastPositionButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReagentsMoveLastPositionButton.Location = New System.Drawing.Point(80, 338)
        Me.bsReagentsMoveLastPositionButton.Name = "bsReagentsMoveLastPositionButton"
        Me.bsReagentsMoveLastPositionButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReagentsMoveLastPositionButton.TabIndex = 14
        Me.bsReagentsMoveLastPositionButton.UseVisualStyleBackColor = true
        '
        'bsReagentsBarCodeTextBox
        '
        Me.bsReagentsBarCodeTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsBarCodeTextBox.DecimalsValues = false
        Me.bsReagentsBarCodeTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsBarCodeTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsBarCodeTextBox.IsNumeric = false
        Me.bsReagentsBarCodeTextBox.Location = New System.Drawing.Point(5, 199)
        Me.bsReagentsBarCodeTextBox.Mandatory = false
        Me.bsReagentsBarCodeTextBox.Name = "bsReagentsBarCodeTextBox"
        Me.bsReagentsBarCodeTextBox.ReadOnly = true
        Me.bsReagentsBarCodeTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsReagentsBarCodeTextBox.TabIndex = 6
        Me.bsReagentsBarCodeTextBox.TabStop = false
        Me.bsReagentsBarCodeTextBox.WordWrap = false
        '
        'bsTestNameTextBox
        '
        Me.bsTestNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsTestNameTextBox.DecimalsValues = false
        Me.bsTestNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestNameTextBox.IsNumeric = false
        Me.bsTestNameTextBox.Location = New System.Drawing.Point(5, 161)
        Me.bsTestNameTextBox.Mandatory = false
        Me.bsTestNameTextBox.MaxLength = 20
        Me.bsTestNameTextBox.Name = "bsTestNameTextBox"
        Me.bsTestNameTextBox.ReadOnly = true
        Me.bsTestNameTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsTestNameTextBox.TabIndex = 5
        Me.bsTestNameTextBox.TabStop = false
        Me.bsTestNameTextBox.WordWrap = false
        '
        'bsReagentsIncreaseButton
        '
        Me.bsReagentsIncreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReagentsIncreaseButton.Enabled = false
        Me.bsReagentsIncreaseButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsReagentsIncreaseButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReagentsIncreaseButton.Location = New System.Drawing.Point(55, 338)
        Me.bsReagentsIncreaseButton.Name = "bsReagentsIncreaseButton"
        Me.bsReagentsIncreaseButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReagentsIncreaseButton.TabIndex = 13
        Me.bsReagentsIncreaseButton.UseVisualStyleBackColor = true
        '
        'bsReagentsNumberTextBox
        '
        Me.bsReagentsNumberTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsNumberTextBox.DecimalsValues = false
        Me.bsReagentsNumberTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsNumberTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsNumberTextBox.IsNumeric = false
        Me.bsReagentsNumberTextBox.Location = New System.Drawing.Point(123, 85)
        Me.bsReagentsNumberTextBox.Mandatory = false
        Me.bsReagentsNumberTextBox.Name = "bsReagentsNumberTextBox"
        Me.bsReagentsNumberTextBox.ReadOnly = true
        Me.bsReagentsNumberTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsReagentsNumberTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsReagentsNumberTextBox.TabIndex = 3
        Me.bsReagentsNumberTextBox.TabStop = false
        Me.bsReagentsNumberTextBox.WordWrap = false
        '
        'bsReagentsDecreaseButton
        '
        Me.bsReagentsDecreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReagentsDecreaseButton.Enabled = false
        Me.bsReagentsDecreaseButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsReagentsDecreaseButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReagentsDecreaseButton.Location = New System.Drawing.Point(30, 338)
        Me.bsReagentsDecreaseButton.Name = "bsReagentsDecreaseButton"
        Me.bsReagentsDecreaseButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReagentsDecreaseButton.TabIndex = 12
        Me.bsReagentsDecreaseButton.UseVisualStyleBackColor = true
        '
        'bsReagentNameTextBox
        '
        Me.bsReagentNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentNameTextBox.DecimalsValues = false
        Me.bsReagentNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentNameTextBox.IsNumeric = false
        Me.bsReagentNameTextBox.Location = New System.Drawing.Point(5, 123)
        Me.bsReagentNameTextBox.Mandatory = false
        Me.bsReagentNameTextBox.Name = "bsReagentNameTextBox"
        Me.bsReagentNameTextBox.ReadOnly = true
        Me.bsReagentNameTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsReagentNameTextBox.TabIndex = 4
        Me.bsReagentNameTextBox.TabStop = false
        Me.bsReagentNameTextBox.WordWrap = false
        '
        'bsReagentsContentTextBox
        '
        Me.bsReagentsContentTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsContentTextBox.DecimalsValues = false
        Me.bsReagentsContentTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsContentTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsContentTextBox.IsNumeric = false
        Me.bsReagentsContentTextBox.Location = New System.Drawing.Point(5, 85)
        Me.bsReagentsContentTextBox.Mandatory = false
        Me.bsReagentsContentTextBox.MaxLength = 20
        Me.bsReagentsContentTextBox.Name = "bsReagentsContentTextBox"
        Me.bsReagentsContentTextBox.ReadOnly = true
        Me.bsReagentsContentTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsReagentsContentTextBox.TabIndex = 2
        Me.bsReagentsContentTextBox.TabStop = false
        Me.bsReagentsContentTextBox.WordWrap = false
        '
        'bsReagentsMoveFirstPositionButton
        '
        Me.bsReagentsMoveFirstPositionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReagentsMoveFirstPositionButton.Enabled = false
        Me.bsReagentsMoveFirstPositionButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsReagentsMoveFirstPositionButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReagentsMoveFirstPositionButton.Location = New System.Drawing.Point(5, 338)
        Me.bsReagentsMoveFirstPositionButton.Name = "bsReagentsMoveFirstPositionButton"
        Me.bsReagentsMoveFirstPositionButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReagentsMoveFirstPositionButton.TabIndex = 11
        Me.bsReagentsMoveFirstPositionButton.UseVisualStyleBackColor = true
        '
        'bsReagentsDiskNameTextBox
        '
        Me.bsReagentsDiskNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsDiskNameTextBox.DecimalsValues = false
        Me.bsReagentsDiskNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsDiskNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsDiskNameTextBox.IsNumeric = false
        Me.bsReagentsDiskNameTextBox.Location = New System.Drawing.Point(5, 47)
        Me.bsReagentsDiskNameTextBox.Mandatory = false
        Me.bsReagentsDiskNameTextBox.Name = "bsReagentsDiskNameTextBox"
        Me.bsReagentsDiskNameTextBox.ReadOnly = true
        Me.bsReagentsDiskNameTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsReagentsDiskNameTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsReagentsDiskNameTextBox.TabIndex = 1
        Me.bsReagentsDiskNameTextBox.TabStop = false
        Me.bsReagentsDiskNameTextBox.WordWrap = false
        '
        'bsExpirationDateLabel
        '
        Me.bsExpirationDateLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsExpirationDateLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsExpirationDateLabel.ForeColor = System.Drawing.Color.Black
        Me.bsExpirationDateLabel.Location = New System.Drawing.Point(2, 223)
        Me.bsExpirationDateLabel.Name = "bsExpirationDateLabel"
        Me.bsExpirationDateLabel.Size = New System.Drawing.Size(81, 13)
        Me.bsExpirationDateLabel.TabIndex = 8
        Me.bsExpirationDateLabel.Text = "Exp. Date"
        Me.bsExpirationDateLabel.Title = false
        '
        'bsReagentsBarCodeLabel
        '
        Me.bsReagentsBarCodeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsBarCodeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsBarCodeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsBarCodeLabel.Location = New System.Drawing.Point(2, 185)
        Me.bsReagentsBarCodeLabel.Name = "bsReagentsBarCodeLabel"
        Me.bsReagentsBarCodeLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsReagentsBarCodeLabel.TabIndex = 7
        Me.bsReagentsBarCodeLabel.Text = "Reagent Barcode"
        Me.bsReagentsBarCodeLabel.Title = false
        '
        'bsTestNameLabel
        '
        Me.bsTestNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTestNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestNameLabel.Location = New System.Drawing.Point(2, 147)
        Me.bsTestNameLabel.Name = "bsTestNameLabel"
        Me.bsTestNameLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsTestNameLabel.TabIndex = 5
        Me.bsTestNameLabel.Text = "Test Name"
        Me.bsTestNameLabel.Title = false
        '
        'bsReagentsNumberLabel
        '
        Me.bsReagentsNumberLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsNumberLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsNumberLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsNumberLabel.Location = New System.Drawing.Point(120, 71)
        Me.bsReagentsNumberLabel.Name = "bsReagentsNumberLabel"
        Me.bsReagentsNumberLabel.Size = New System.Drawing.Size(52, 13)
        Me.bsReagentsNumberLabel.TabIndex = 4
        Me.bsReagentsNumberLabel.Text = "Num"
        Me.bsReagentsNumberLabel.Title = false
        '
        'bsReagentNameLabel
        '
        Me.bsReagentNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentNameLabel.Location = New System.Drawing.Point(2, 109)
        Me.bsReagentNameLabel.Name = "bsReagentNameLabel"
        Me.bsReagentNameLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsReagentNameLabel.TabIndex = 3
        Me.bsReagentNameLabel.Text = "Reagent Name"
        Me.bsReagentNameLabel.Title = false
        '
        'bsReagentsContentLabel
        '
        Me.bsReagentsContentLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsContentLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsContentLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsContentLabel.Location = New System.Drawing.Point(2, 71)
        Me.bsReagentsContentLabel.Name = "bsReagentsContentLabel"
        Me.bsReagentsContentLabel.Size = New System.Drawing.Size(119, 13)
        Me.bsReagentsContentLabel.TabIndex = 2
        Me.bsReagentsContentLabel.Text = "Content"
        Me.bsReagentsContentLabel.Title = false
        '
        'bsReagentsDiskNameLabel
        '
        Me.bsReagentsDiskNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsDiskNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsDiskNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsDiskNameLabel.Location = New System.Drawing.Point(2, 33)
        Me.bsReagentsDiskNameLabel.Name = "bsReagentsDiskNameLabel"
        Me.bsReagentsDiskNameLabel.Size = New System.Drawing.Size(119, 13)
        Me.bsReagentsDiskNameLabel.TabIndex = 1
        Me.bsReagentsDiskNameLabel.Text = "Disk Name/Num"
        Me.bsReagentsDiskNameLabel.Title = false
        '
        'Reag288
        '
        Me.Reag288.BackColor = System.Drawing.Color.Transparent
        Me.Reag288.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag288.Image = Nothing
        Me.Reag288.ImagePath = ""
        Me.Reag288.IsTransparentImage = false
        Me.Reag288.Location = New System.Drawing.Point(199, 426)
        Me.Reag288.Name = "Reag288"
        Me.Reag288.Rotation = 9
        Me.Reag288.ShowThrough = true
        Me.Reag288.Size = New System.Drawing.Size(122, 122)
        Me.Reag288.TabIndex = 178
        Me.Reag288.TabStop = false
        Me.Reag288.TransparentColor = System.Drawing.Color.Transparent
        '
        'Reag287
        '
        Me.Reag287.BackColor = System.Drawing.Color.Transparent
        Me.Reag287.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag287.Image = Nothing
        Me.Reag287.ImagePath = ""
        Me.Reag287.IsTransparentImage = false
        Me.Reag287.Location = New System.Drawing.Point(173, 420)
        Me.Reag287.Name = "Reag287"
        Me.Reag287.Rotation = 17
        Me.Reag287.ShowThrough = true
        Me.Reag287.Size = New System.Drawing.Size(122, 122)
        Me.Reag287.TabIndex = 179
        Me.Reag287.TabStop = false
        Me.Reag287.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag287.WaitOnLoad = true
        '
        'Reag286
        '
        Me.Reag286.BackColor = System.Drawing.Color.Transparent
        Me.Reag286.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag286.Image = Nothing
        Me.Reag286.ImagePath = ""
        Me.Reag286.IsTransparentImage = false
        Me.Reag286.Location = New System.Drawing.Point(147, 410)
        Me.Reag286.Name = "Reag286"
        Me.Reag286.Rotation = 25
        Me.Reag286.ShowThrough = true
        Me.Reag286.Size = New System.Drawing.Size(122, 122)
        Me.Reag286.TabIndex = 180
        Me.Reag286.TabStop = false
        Me.Reag286.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag286.WaitOnLoad = true
        '
        'Reag285
        '
        Me.Reag285.BackColor = System.Drawing.Color.Transparent
        Me.Reag285.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag285.Image = Nothing
        Me.Reag285.ImagePath = ""
        Me.Reag285.IsTransparentImage = false
        Me.Reag285.Location = New System.Drawing.Point(124, 397)
        Me.Reag285.Name = "Reag285"
        Me.Reag285.Rotation = 34
        Me.Reag285.ShowThrough = true
        Me.Reag285.Size = New System.Drawing.Size(122, 122)
        Me.Reag285.TabIndex = 181
        Me.Reag285.TabStop = false
        Me.Reag285.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag285.WaitOnLoad = true
        '
        'Reag284
        '
        Me.Reag284.BackColor = System.Drawing.Color.Transparent
        Me.Reag284.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag284.Image = Nothing
        Me.Reag284.ImagePath = ""
        Me.Reag284.IsTransparentImage = false
        Me.Reag284.Location = New System.Drawing.Point(102, 380)
        Me.Reag284.Name = "Reag284"
        Me.Reag284.Rotation = 41
        Me.Reag284.ShowThrough = true
        Me.Reag284.Size = New System.Drawing.Size(122, 122)
        Me.Reag284.TabIndex = 182
        Me.Reag284.TabStop = false
        Me.Reag284.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag284.WaitOnLoad = true
        '
        'Reag283
        '
        Me.Reag283.BackColor = System.Drawing.Color.Transparent
        Me.Reag283.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag283.Image = Nothing
        Me.Reag283.ImagePath = ""
        Me.Reag283.IsTransparentImage = false
        Me.Reag283.Location = New System.Drawing.Point(84, 361)
        Me.Reag283.Name = "Reag283"
        Me.Reag283.Rotation = 50
        Me.Reag283.ShowThrough = true
        Me.Reag283.Size = New System.Drawing.Size(122, 122)
        Me.Reag283.TabIndex = 261
        Me.Reag283.TabStop = false
        Me.Reag283.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag283.WaitOnLoad = true
        '
        'Reag282
        '
        Me.Reag282.BackColor = System.Drawing.Color.Transparent
        Me.Reag282.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag282.Image = Nothing
        Me.Reag282.ImagePath = ""
        Me.Reag282.IsTransparentImage = false
        Me.Reag282.Location = New System.Drawing.Point(67, 339)
        Me.Reag282.Name = "Reag282"
        Me.Reag282.Rotation = 57
        Me.Reag282.ShowThrough = true
        Me.Reag282.Size = New System.Drawing.Size(122, 122)
        Me.Reag282.TabIndex = 260
        Me.Reag282.TabStop = false
        Me.Reag282.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag282.WaitOnLoad = true
        '
        'Reag281
        '
        Me.Reag281.BackColor = System.Drawing.Color.Transparent
        Me.Reag281.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag281.Image = Nothing
        Me.Reag281.ImagePath = ""
        Me.Reag281.IsTransparentImage = false
        Me.Reag281.Location = New System.Drawing.Point(55, 313)
        Me.Reag281.Name = "Reag281"
        Me.Reag281.Rotation = 66
        Me.Reag281.ShowThrough = true
        Me.Reag281.Size = New System.Drawing.Size(122, 122)
        Me.Reag281.TabIndex = 259
        Me.Reag281.TabStop = false
        Me.Reag281.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag281.WaitOnLoad = true
        '
        'Reag280
        '
        Me.Reag280.BackColor = System.Drawing.Color.Transparent
        Me.Reag280.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag280.Image = Nothing
        Me.Reag280.ImagePath = ""
        Me.Reag280.IsTransparentImage = false
        Me.Reag280.Location = New System.Drawing.Point(46, 288)
        Me.Reag280.Name = "Reag280"
        Me.Reag280.Rotation = 75
        Me.Reag280.ShowThrough = true
        Me.Reag280.Size = New System.Drawing.Size(122, 122)
        Me.Reag280.TabIndex = 258
        Me.Reag280.TabStop = false
        Me.Reag280.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag280.WaitOnLoad = true
        '
        'Reag279
        '
        Me.Reag279.BackColor = System.Drawing.Color.Transparent
        Me.Reag279.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag279.Image = Nothing
        Me.Reag279.ImagePath = ""
        Me.Reag279.IsTransparentImage = false
        Me.Reag279.Location = New System.Drawing.Point(39, 261)
        Me.Reag279.Name = "Reag279"
        Me.Reag279.Rotation = 83
        Me.Reag279.ShowThrough = true
        Me.Reag279.Size = New System.Drawing.Size(122, 122)
        Me.Reag279.TabIndex = 257
        Me.Reag279.TabStop = false
        Me.Reag279.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag279.WaitOnLoad = true
        '
        'Reag278
        '
        Me.Reag278.BackColor = System.Drawing.Color.Transparent
        Me.Reag278.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag278.Image = Nothing
        Me.Reag278.ImagePath = ""
        Me.Reag278.IsTransparentImage = false
        Me.Reag278.Location = New System.Drawing.Point(38, 233)
        Me.Reag278.Name = "Reag278"
        Me.Reag278.Rotation = 91
        Me.Reag278.ShowThrough = true
        Me.Reag278.Size = New System.Drawing.Size(122, 122)
        Me.Reag278.TabIndex = 256
        Me.Reag278.TabStop = false
        Me.Reag278.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag278.WaitOnLoad = true
        '
        'Reag277
        '
        Me.Reag277.BackColor = System.Drawing.Color.Transparent
        Me.Reag277.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag277.Image = Nothing
        Me.Reag277.ImagePath = ""
        Me.Reag277.IsTransparentImage = false
        Me.Reag277.Location = New System.Drawing.Point(40, 205)
        Me.Reag277.Name = "Reag277"
        Me.Reag277.Rotation = 99
        Me.Reag277.ShowThrough = true
        Me.Reag277.Size = New System.Drawing.Size(122, 122)
        Me.Reag277.TabIndex = 255
        Me.Reag277.TabStop = false
        Me.Reag277.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag277.WaitOnLoad = true
        '
        'Reag276
        '
        Me.Reag276.BackColor = System.Drawing.Color.Transparent
        Me.Reag276.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag276.Image = Nothing
        Me.Reag276.ImagePath = ""
        Me.Reag276.IsTransparentImage = false
        Me.Reag276.Location = New System.Drawing.Point(46, 179)
        Me.Reag276.Name = "Reag276"
        Me.Reag276.Rotation = 108
        Me.Reag276.ShowThrough = true
        Me.Reag276.Size = New System.Drawing.Size(122, 122)
        Me.Reag276.TabIndex = 254
        Me.Reag276.TabStop = false
        Me.Reag276.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag276.WaitOnLoad = true
        '
        'Reag275
        '
        Me.Reag275.BackColor = System.Drawing.Color.Transparent
        Me.Reag275.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag275.Image = Nothing
        Me.Reag275.ImagePath = ""
        Me.Reag275.IsTransparentImage = false
        Me.Reag275.Location = New System.Drawing.Point(55, 153)
        Me.Reag275.Name = "Reag275"
        Me.Reag275.Rotation = 116
        Me.Reag275.ShowThrough = true
        Me.Reag275.Size = New System.Drawing.Size(122, 122)
        Me.Reag275.TabIndex = 253
        Me.Reag275.TabStop = false
        Me.Reag275.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag275.WaitOnLoad = true
        '
        'Reag274
        '
        Me.Reag274.BackColor = System.Drawing.Color.Transparent
        Me.Reag274.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag274.Image = Nothing
        Me.Reag274.ImagePath = ""
        Me.Reag274.IsTransparentImage = false
        Me.Reag274.Location = New System.Drawing.Point(68, 129)
        Me.Reag274.Name = "Reag274"
        Me.Reag274.Rotation = 124
        Me.Reag274.ShowThrough = true
        Me.Reag274.Size = New System.Drawing.Size(122, 122)
        Me.Reag274.TabIndex = 252
        Me.Reag274.TabStop = false
        Me.Reag274.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag274.WaitOnLoad = true
        '
        'Reag273
        '
        Me.Reag273.BackColor = System.Drawing.Color.Transparent
        Me.Reag273.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag273.Image = Nothing
        Me.Reag273.ImagePath = ""
        Me.Reag273.IsTransparentImage = false
        Me.Reag273.Location = New System.Drawing.Point(84, 107)
        Me.Reag273.Name = "Reag273"
        Me.Reag273.Rotation = 132
        Me.Reag273.ShowThrough = true
        Me.Reag273.Size = New System.Drawing.Size(122, 122)
        Me.Reag273.TabIndex = 251
        Me.Reag273.TabStop = false
        Me.Reag273.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag273.WaitOnLoad = true
        '
        'Reag272
        '
        Me.Reag272.BackColor = System.Drawing.Color.Transparent
        Me.Reag272.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag272.Image = Nothing
        Me.Reag272.ImagePath = ""
        Me.Reag272.IsTransparentImage = false
        Me.Reag272.Location = New System.Drawing.Point(103, 87)
        Me.Reag272.Name = "Reag272"
        Me.Reag272.Rotation = 140
        Me.Reag272.ShowThrough = true
        Me.Reag272.Size = New System.Drawing.Size(122, 122)
        Me.Reag272.TabIndex = 250
        Me.Reag272.TabStop = false
        Me.Reag272.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag272.WaitOnLoad = true
        '
        'Reag271
        '
        Me.Reag271.BackColor = System.Drawing.Color.Transparent
        Me.Reag271.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag271.Image = Nothing
        Me.Reag271.ImagePath = ""
        Me.Reag271.IsTransparentImage = false
        Me.Reag271.Location = New System.Drawing.Point(125, 71)
        Me.Reag271.Name = "Reag271"
        Me.Reag271.Rotation = 149
        Me.Reag271.ShowThrough = true
        Me.Reag271.Size = New System.Drawing.Size(122, 122)
        Me.Reag271.TabIndex = 249
        Me.Reag271.TabStop = false
        Me.Reag271.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag271.WaitOnLoad = true
        '
        'Reag270
        '
        Me.Reag270.BackColor = System.Drawing.Color.Transparent
        Me.Reag270.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag270.Image = Nothing
        Me.Reag270.ImagePath = ""
        Me.Reag270.IsTransparentImage = false
        Me.Reag270.Location = New System.Drawing.Point(149, 58)
        Me.Reag270.Name = "Reag270"
        Me.Reag270.Rotation = 157
        Me.Reag270.ShowThrough = true
        Me.Reag270.Size = New System.Drawing.Size(122, 122)
        Me.Reag270.TabIndex = 248
        Me.Reag270.TabStop = false
        Me.Reag270.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag270.WaitOnLoad = true
        '
        'Reag269
        '
        Me.Reag269.BackColor = System.Drawing.Color.Transparent
        Me.Reag269.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag269.Image = Nothing
        Me.Reag269.ImagePath = ""
        Me.Reag269.IsTransparentImage = false
        Me.Reag269.Location = New System.Drawing.Point(175, 48)
        Me.Reag269.Name = "Reag269"
        Me.Reag269.Rotation = 165
        Me.Reag269.ShowThrough = true
        Me.Reag269.Size = New System.Drawing.Size(122, 122)
        Me.Reag269.TabIndex = 247
        Me.Reag269.TabStop = false
        Me.Reag269.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag269.WaitOnLoad = true
        '
        'Reag268
        '
        Me.Reag268.BackColor = System.Drawing.Color.Transparent
        Me.Reag268.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag268.Image = Nothing
        Me.Reag268.ImagePath = ""
        Me.Reag268.IsTransparentImage = false
        Me.Reag268.Location = New System.Drawing.Point(201, 42)
        Me.Reag268.Name = "Reag268"
        Me.Reag268.Rotation = 173
        Me.Reag268.ShowThrough = true
        Me.Reag268.Size = New System.Drawing.Size(122, 122)
        Me.Reag268.TabIndex = 246
        Me.Reag268.TabStop = false
        Me.Reag268.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag268.WaitOnLoad = true
        '
        'Reag267
        '
        Me.Reag267.BackColor = System.Drawing.Color.Transparent
        Me.Reag267.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag267.Image = Nothing
        Me.Reag267.ImagePath = ""
        Me.Reag267.IsTransparentImage = false
        Me.Reag267.Location = New System.Drawing.Point(227, 41)
        Me.Reag267.Name = "Reag267"
        Me.Reag267.Rotation = 181
        Me.Reag267.ShowThrough = true
        Me.Reag267.Size = New System.Drawing.Size(122, 122)
        Me.Reag267.TabIndex = 245
        Me.Reag267.TabStop = false
        Me.Reag267.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag267.WaitOnLoad = true
        '
        'Reag266
        '
        Me.Reag266.BackColor = System.Drawing.Color.Transparent
        Me.Reag266.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag266.Image = Nothing
        Me.Reag266.ImagePath = ""
        Me.Reag266.IsTransparentImage = false
        Me.Reag266.Location = New System.Drawing.Point(255, 42)
        Me.Reag266.Name = "Reag266"
        Me.Reag266.Rotation = 188
        Me.Reag266.ShowThrough = true
        Me.Reag266.Size = New System.Drawing.Size(122, 122)
        Me.Reag266.TabIndex = 244
        Me.Reag266.TabStop = false
        Me.Reag266.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag266.WaitOnLoad = true
        '
        'Reag265
        '
        Me.Reag265.BackColor = System.Drawing.Color.Transparent
        Me.Reag265.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag265.Image = Nothing
        Me.Reag265.ImagePath = ""
        Me.Reag265.IsTransparentImage = false
        Me.Reag265.Location = New System.Drawing.Point(281, 48)
        Me.Reag265.Name = "Reag265"
        Me.Reag265.Rotation = 197
        Me.Reag265.ShowThrough = true
        Me.Reag265.Size = New System.Drawing.Size(122, 122)
        Me.Reag265.TabIndex = 243
        Me.Reag265.TabStop = false
        Me.Reag265.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag265.WaitOnLoad = true
        '
        'Reag264
        '
        Me.Reag264.BackColor = System.Drawing.Color.Transparent
        Me.Reag264.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag264.Image = Nothing
        Me.Reag264.ImagePath = ""
        Me.Reag264.IsTransparentImage = false
        Me.Reag264.Location = New System.Drawing.Point(306, 58)
        Me.Reag264.Name = "Reag264"
        Me.Reag264.Rotation = 205
        Me.Reag264.ShowThrough = true
        Me.Reag264.Size = New System.Drawing.Size(122, 122)
        Me.Reag264.TabIndex = 242
        Me.Reag264.TabStop = false
        Me.Reag264.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag264.WaitOnLoad = true
        '
        'Reag263
        '
        Me.Reag263.BackColor = System.Drawing.Color.Transparent
        Me.Reag263.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag263.Image = Nothing
        Me.Reag263.ImagePath = ""
        Me.Reag263.IsTransparentImage = false
        Me.Reag263.Location = New System.Drawing.Point(331, 72)
        Me.Reag263.Name = "Reag263"
        Me.Reag263.Rotation = 212
        Me.Reag263.ShowThrough = true
        Me.Reag263.Size = New System.Drawing.Size(122, 122)
        Me.Reag263.TabIndex = 241
        Me.Reag263.TabStop = false
        Me.Reag263.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag263.WaitOnLoad = true
        '
        'Reag262
        '
        Me.Reag262.BackColor = System.Drawing.Color.Transparent
        Me.Reag262.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag262.Image = Nothing
        Me.Reag262.ImagePath = ""
        Me.Reag262.IsTransparentImage = false
        Me.Reag262.Location = New System.Drawing.Point(353, 88)
        Me.Reag262.Name = "Reag262"
        Me.Reag262.Rotation = 221
        Me.Reag262.ShowThrough = true
        Me.Reag262.Size = New System.Drawing.Size(122, 122)
        Me.Reag262.TabIndex = 240
        Me.Reag262.TabStop = false
        Me.Reag262.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag262.WaitOnLoad = true
        '
        'Reag261
        '
        Me.Reag261.BackColor = System.Drawing.Color.Transparent
        Me.Reag261.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag261.Image = Nothing
        Me.Reag261.ImagePath = ""
        Me.Reag261.IsTransparentImage = false
        Me.Reag261.Location = New System.Drawing.Point(371, 107)
        Me.Reag261.Name = "Reag261"
        Me.Reag261.Rotation = 229
        Me.Reag261.ShowThrough = true
        Me.Reag261.Size = New System.Drawing.Size(122, 122)
        Me.Reag261.TabIndex = 239
        Me.Reag261.TabStop = false
        Me.Reag261.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag261.WaitOnLoad = true
        '
        'Reag260
        '
        Me.Reag260.BackColor = System.Drawing.Color.Transparent
        Me.Reag260.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag260.Image = Nothing
        Me.Reag260.ImagePath = ""
        Me.Reag260.IsTransparentImage = false
        Me.Reag260.Location = New System.Drawing.Point(388, 129)
        Me.Reag260.Name = "Reag260"
        Me.Reag260.Rotation = 237
        Me.Reag260.ShowThrough = true
        Me.Reag260.Size = New System.Drawing.Size(122, 122)
        Me.Reag260.TabIndex = 238
        Me.Reag260.TabStop = false
        Me.Reag260.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag260.WaitOnLoad = true
        '
        'Reag259
        '
        Me.Reag259.BackColor = System.Drawing.Color.Transparent
        Me.Reag259.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag259.Image = Nothing
        Me.Reag259.ImagePath = ""
        Me.Reag259.IsTransparentImage = false
        Me.Reag259.Location = New System.Drawing.Point(400, 154)
        Me.Reag259.Name = "Reag259"
        Me.Reag259.Rotation = 246
        Me.Reag259.ShowThrough = true
        Me.Reag259.Size = New System.Drawing.Size(122, 122)
        Me.Reag259.TabIndex = 237
        Me.Reag259.TabStop = false
        Me.Reag259.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag259.WaitOnLoad = true
        '
        'Reag258
        '
        Me.Reag258.BackColor = System.Drawing.Color.Transparent
        Me.Reag258.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag258.Image = Nothing
        Me.Reag258.ImagePath = ""
        Me.Reag258.IsTransparentImage = false
        Me.Reag258.Location = New System.Drawing.Point(409, 180)
        Me.Reag258.Name = "Reag258"
        Me.Reag258.Rotation = 254
        Me.Reag258.ShowThrough = true
        Me.Reag258.Size = New System.Drawing.Size(122, 122)
        Me.Reag258.TabIndex = 236
        Me.Reag258.TabStop = false
        Me.Reag258.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag258.WaitOnLoad = true
        '
        'Reag257
        '
        Me.Reag257.BackColor = System.Drawing.Color.Transparent
        Me.Reag257.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag257.Image = Nothing
        Me.Reag257.ImagePath = ""
        Me.Reag257.IsTransparentImage = false
        Me.Reag257.Location = New System.Drawing.Point(415, 207)
        Me.Reag257.Name = "Reag257"
        Me.Reag257.Rotation = 262
        Me.Reag257.ShowThrough = true
        Me.Reag257.Size = New System.Drawing.Size(122, 122)
        Me.Reag257.TabIndex = 235
        Me.Reag257.TabStop = false
        Me.Reag257.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag257.WaitOnLoad = true
        '
        'Reag256
        '
        Me.Reag256.BackColor = System.Drawing.Color.Transparent
        Me.Reag256.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag256.Image = Nothing
        Me.Reag256.ImagePath = ""
        Me.Reag256.IsTransparentImage = false
        Me.Reag256.Location = New System.Drawing.Point(417, 235)
        Me.Reag256.Name = "Reag256"
        Me.Reag256.Rotation = 271
        Me.Reag256.ShowThrough = true
        Me.Reag256.Size = New System.Drawing.Size(122, 122)
        Me.Reag256.TabIndex = 234
        Me.Reag256.TabStop = false
        Me.Reag256.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag256.WaitOnLoad = true
        '
        'Reag255
        '
        Me.Reag255.BackColor = System.Drawing.Color.Transparent
        Me.Reag255.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag255.Image = Nothing
        Me.Reag255.ImagePath = ""
        Me.Reag255.IsTransparentImage = false
        Me.Reag255.Location = New System.Drawing.Point(415, 262)
        Me.Reag255.Name = "Reag255"
        Me.Reag255.Rotation = 279
        Me.Reag255.ShowThrough = true
        Me.Reag255.Size = New System.Drawing.Size(122, 122)
        Me.Reag255.TabIndex = 233
        Me.Reag255.TabStop = false
        Me.Reag255.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag255.WaitOnLoad = true
        '
        'Reag254
        '
        Me.Reag254.BackColor = System.Drawing.Color.Transparent
        Me.Reag254.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag254.Image = Nothing
        Me.Reag254.ImagePath = ""
        Me.Reag254.IsTransparentImage = false
        Me.Reag254.Location = New System.Drawing.Point(409, 289)
        Me.Reag254.Name = "Reag254"
        Me.Reag254.Rotation = 287
        Me.Reag254.ShowThrough = true
        Me.Reag254.Size = New System.Drawing.Size(122, 122)
        Me.Reag254.TabIndex = 232
        Me.Reag254.TabStop = false
        Me.Reag254.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag254.WaitOnLoad = true
        '
        'Reag253
        '
        Me.Reag253.BackColor = System.Drawing.Color.Transparent
        Me.Reag253.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag253.Image = Nothing
        Me.Reag253.ImagePath = ""
        Me.Reag253.IsTransparentImage = false
        Me.Reag253.Location = New System.Drawing.Point(399, 316)
        Me.Reag253.Name = "Reag253"
        Me.Reag253.Rotation = 296
        Me.Reag253.ShowThrough = true
        Me.Reag253.Size = New System.Drawing.Size(122, 122)
        Me.Reag253.TabIndex = 231
        Me.Reag253.TabStop = false
        Me.Reag253.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag253.WaitOnLoad = true
        '
        'Reag252
        '
        Me.Reag252.BackColor = System.Drawing.Color.Transparent
        Me.Reag252.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag252.Image = Nothing
        Me.Reag252.ImagePath = ""
        Me.Reag252.IsTransparentImage = false
        Me.Reag252.Location = New System.Drawing.Point(387, 339)
        Me.Reag252.Name = "Reag252"
        Me.Reag252.Rotation = 304
        Me.Reag252.ShowThrough = true
        Me.Reag252.Size = New System.Drawing.Size(122, 122)
        Me.Reag252.TabIndex = 230
        Me.Reag252.TabStop = false
        Me.Reag252.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag252.WaitOnLoad = true
        '
        'Reag251
        '
        Me.Reag251.BackColor = System.Drawing.Color.Transparent
        Me.Reag251.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag251.Image = Nothing
        Me.Reag251.ImagePath = ""
        Me.Reag251.IsTransparentImage = false
        Me.Reag251.Location = New System.Drawing.Point(371, 362)
        Me.Reag251.Name = "Reag251"
        Me.Reag251.Rotation = 312
        Me.Reag251.ShowThrough = true
        Me.Reag251.Size = New System.Drawing.Size(122, 122)
        Me.Reag251.TabIndex = 230
        Me.Reag251.TabStop = false
        Me.Reag251.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag251.WaitOnLoad = true
        '
        'Reag250
        '
        Me.Reag250.BackColor = System.Drawing.Color.Transparent
        Me.Reag250.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag250.Image = Nothing
        Me.Reag250.ImagePath = ""
        Me.Reag250.IsTransparentImage = false
        Me.Reag250.Location = New System.Drawing.Point(352, 381)
        Me.Reag250.Name = "Reag250"
        Me.Reag250.Rotation = 320
        Me.Reag250.ShowThrough = true
        Me.Reag250.Size = New System.Drawing.Size(122, 122)
        Me.Reag250.TabIndex = 228
        Me.Reag250.TabStop = false
        Me.Reag250.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag250.WaitOnLoad = true
        '
        'Reag249
        '
        Me.Reag249.BackColor = System.Drawing.Color.Transparent
        Me.Reag249.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag249.Image = Nothing
        Me.Reag249.ImagePath = ""
        Me.Reag249.IsTransparentImage = false
        Me.Reag249.Location = New System.Drawing.Point(330, 398)
        Me.Reag249.Name = "Reag249"
        Me.Reag249.Rotation = 328
        Me.Reag249.ShowThrough = true
        Me.Reag249.Size = New System.Drawing.Size(122, 122)
        Me.Reag249.TabIndex = 228
        Me.Reag249.TabStop = false
        Me.Reag249.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag249.WaitOnLoad = true
        '
        'Reag248
        '
        Me.Reag248.BackColor = System.Drawing.Color.Transparent
        Me.Reag248.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag248.Image = Nothing
        Me.Reag248.ImagePath = ""
        Me.Reag248.IsTransparentImage = false
        Me.Reag248.Location = New System.Drawing.Point(306, 411)
        Me.Reag248.Name = "Reag248"
        Me.Reag248.Rotation = 337
        Me.Reag248.ShowThrough = true
        Me.Reag248.Size = New System.Drawing.Size(122, 122)
        Me.Reag248.TabIndex = 227
        Me.Reag248.TabStop = false
        Me.Reag248.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag248.WaitOnLoad = true
        '
        'Reag247
        '
        Me.Reag247.BackColor = System.Drawing.Color.Transparent
        Me.Reag247.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag247.Image = Nothing
        Me.Reag247.ImagePath = ""
        Me.Reag247.IsTransparentImage = false
        Me.Reag247.Location = New System.Drawing.Point(280, 420)
        Me.Reag247.Name = "Reag247"
        Me.Reag247.Rotation = 344
        Me.Reag247.ShowThrough = true
        Me.Reag247.Size = New System.Drawing.Size(122, 122)
        Me.Reag247.TabIndex = 225
        Me.Reag247.TabStop = false
        Me.Reag247.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag247.WaitOnLoad = true
        '
        'Reag246
        '
        Me.Reag246.BackColor = System.Drawing.Color.Transparent
        Me.Reag246.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag246.Image = Nothing
        Me.Reag246.ImagePath = ""
        Me.Reag246.IsTransparentImage = false
        Me.Reag246.Location = New System.Drawing.Point(254, 426)
        Me.Reag246.Name = "Reag246"
        Me.Reag246.Rotation = 352
        Me.Reag246.ShowThrough = true
        Me.Reag246.Size = New System.Drawing.Size(122, 122)
        Me.Reag246.TabIndex = 224
        Me.Reag246.TabStop = false
        Me.Reag246.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag246.WaitOnLoad = true
        '
        'Reag245
        '
        Me.Reag245.BackColor = System.Drawing.Color.Transparent
        Me.Reag245.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag245.Image = Nothing
        Me.Reag245.ImagePath = ""
        Me.Reag245.IsTransparentImage = false
        Me.Reag245.Location = New System.Drawing.Point(227, 428)
        Me.Reag245.Name = "Reag245"
        Me.Reag245.Rotation = 1
        Me.Reag245.ShowThrough = true
        Me.Reag245.Size = New System.Drawing.Size(122, 122)
        Me.Reag245.TabIndex = 223
        Me.Reag245.TabStop = false
        Me.Reag245.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag245.WaitOnLoad = true
        '
        'ReactionsTab
        '
        Me.ReactionsTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ReactionsTab.Appearance.PageClient.Image = CType(resources.GetObject("ReactionsTab.Appearance.PageClient.Image"),System.Drawing.Image)
        Me.ReactionsTab.Appearance.PageClient.Options.UseBackColor = true
        Me.ReactionsTab.Appearance.PageClient.Options.UseImage = true
        Me.ReactionsTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.ReactionsTab.Controls.Add(Me.Reac2)
        Me.ReactionsTab.Controls.Add(Me.Reac39)
        Me.ReactionsTab.Controls.Add(Me.Reac29)
        Me.ReactionsTab.Controls.Add(Me.Reac28)
        Me.ReactionsTab.Controls.Add(Me.Reac27)
        Me.ReactionsTab.Controls.Add(Me.Reac16)
        Me.ReactionsTab.Controls.Add(Me.Reac15)
        Me.ReactionsTab.Controls.Add(Me.Reac14)
        Me.ReactionsTab.Controls.Add(Me.Reac13)
        Me.ReactionsTab.Controls.Add(Me.Reac12)
        Me.ReactionsTab.Controls.Add(Me.Reac11)
        Me.ReactionsTab.Controls.Add(Me.Reac10)
        Me.ReactionsTab.Controls.Add(Me.Reac9)
        Me.ReactionsTab.Controls.Add(Me.Reac8)
        Me.ReactionsTab.Controls.Add(Me.Reac7)
        Me.ReactionsTab.Controls.Add(Me.Reac6)
        Me.ReactionsTab.Controls.Add(Me.Reac5)
        Me.ReactionsTab.Controls.Add(Me.Reac4)
        Me.ReactionsTab.Controls.Add(Me.Reac3)
        Me.ReactionsTab.Controls.Add(Me.Reac1)
        Me.ReactionsTab.Controls.Add(Me.Reac66)
        Me.ReactionsTab.Controls.Add(Me.Reac67)
        Me.ReactionsTab.Controls.Add(Me.Reac117)
        Me.ReactionsTab.Controls.Add(Me.Reac77)
        Me.ReactionsTab.Controls.Add(Me.Reac78)
        Me.ReactionsTab.Controls.Add(Me.Reac26)
        Me.ReactionsTab.Controls.Add(Me.Reac37)
        Me.ReactionsTab.Controls.Add(Me.Reac38)
        Me.ReactionsTab.Controls.Add(Me.Reac120)
        Me.ReactionsTab.Controls.Add(Me.Reac119)
        Me.ReactionsTab.Controls.Add(Me.Reac118)
        Me.ReactionsTab.Controls.Add(Me.Reac116)
        Me.ReactionsTab.Controls.Add(Me.Reac115)
        Me.ReactionsTab.Controls.Add(Me.Reac114)
        Me.ReactionsTab.Controls.Add(Me.Reac113)
        Me.ReactionsTab.Controls.Add(Me.Reac112)
        Me.ReactionsTab.Controls.Add(Me.Reac111)
        Me.ReactionsTab.Controls.Add(Me.Reac110)
        Me.ReactionsTab.Controls.Add(Me.Reac109)
        Me.ReactionsTab.Controls.Add(Me.Reac108)
        Me.ReactionsTab.Controls.Add(Me.Reac107)
        Me.ReactionsTab.Controls.Add(Me.Reac106)
        Me.ReactionsTab.Controls.Add(Me.Reac105)
        Me.ReactionsTab.Controls.Add(Me.Reac104)
        Me.ReactionsTab.Controls.Add(Me.Reac103)
        Me.ReactionsTab.Controls.Add(Me.Reac102)
        Me.ReactionsTab.Controls.Add(Me.Reac100)
        Me.ReactionsTab.Controls.Add(Me.Reac99)
        Me.ReactionsTab.Controls.Add(Me.Reac98)
        Me.ReactionsTab.Controls.Add(Me.Reac97)
        Me.ReactionsTab.Controls.Add(Me.Reac96)
        Me.ReactionsTab.Controls.Add(Me.Reac95)
        Me.ReactionsTab.Controls.Add(Me.Reac94)
        Me.ReactionsTab.Controls.Add(Me.Reac93)
        Me.ReactionsTab.Controls.Add(Me.Reac92)
        Me.ReactionsTab.Controls.Add(Me.Reac91)
        Me.ReactionsTab.Controls.Add(Me.Reac90)
        Me.ReactionsTab.Controls.Add(Me.Reac88)
        Me.ReactionsTab.Controls.Add(Me.Reac86)
        Me.ReactionsTab.Controls.Add(Me.Reac85)
        Me.ReactionsTab.Controls.Add(Me.Reac84)
        Me.ReactionsTab.Controls.Add(Me.Reac83)
        Me.ReactionsTab.Controls.Add(Me.Reac82)
        Me.ReactionsTab.Controls.Add(Me.Reac81)
        Me.ReactionsTab.Controls.Add(Me.Reac80)
        Me.ReactionsTab.Controls.Add(Me.Reac79)
        Me.ReactionsTab.Controls.Add(Me.Reac76)
        Me.ReactionsTab.Controls.Add(Me.Reac74)
        Me.ReactionsTab.Controls.Add(Me.Reac73)
        Me.ReactionsTab.Controls.Add(Me.Reac72)
        Me.ReactionsTab.Controls.Add(Me.Reac71)
        Me.ReactionsTab.Controls.Add(Me.Reac70)
        Me.ReactionsTab.Controls.Add(Me.Reac69)
        Me.ReactionsTab.Controls.Add(Me.Reac68)
        Me.ReactionsTab.Controls.Add(Me.Reac65)
        Me.ReactionsTab.Controls.Add(Me.Reac64)
        Me.ReactionsTab.Controls.Add(Me.Reac63)
        Me.ReactionsTab.Controls.Add(Me.Reac62)
        Me.ReactionsTab.Controls.Add(Me.Reac61)
        Me.ReactionsTab.Controls.Add(Me.Reac60)
        Me.ReactionsTab.Controls.Add(Me.Reac59)
        Me.ReactionsTab.Controls.Add(Me.Reac58)
        Me.ReactionsTab.Controls.Add(Me.Reac57)
        Me.ReactionsTab.Controls.Add(Me.Reac56)
        Me.ReactionsTab.Controls.Add(Me.Reac55)
        Me.ReactionsTab.Controls.Add(Me.Reac54)
        Me.ReactionsTab.Controls.Add(Me.Reac53)
        Me.ReactionsTab.Controls.Add(Me.Reac52)
        Me.ReactionsTab.Controls.Add(Me.Reac51)
        Me.ReactionsTab.Controls.Add(Me.Reac50)
        Me.ReactionsTab.Controls.Add(Me.Reac49)
        Me.ReactionsTab.Controls.Add(Me.Reac48)
        Me.ReactionsTab.Controls.Add(Me.Reac47)
        Me.ReactionsTab.Controls.Add(Me.Reac46)
        Me.ReactionsTab.Controls.Add(Me.Reac45)
        Me.ReactionsTab.Controls.Add(Me.Reac44)
        Me.ReactionsTab.Controls.Add(Me.Reac43)
        Me.ReactionsTab.Controls.Add(Me.Reac42)
        Me.ReactionsTab.Controls.Add(Me.Reac41)
        Me.ReactionsTab.Controls.Add(Me.Reac40)
        Me.ReactionsTab.Controls.Add(Me.Reac36)
        Me.ReactionsTab.Controls.Add(Me.Reac35)
        Me.ReactionsTab.Controls.Add(Me.Reac34)
        Me.ReactionsTab.Controls.Add(Me.Reac33)
        Me.ReactionsTab.Controls.Add(Me.Reac32)
        Me.ReactionsTab.Controls.Add(Me.Reac31)
        Me.ReactionsTab.Controls.Add(Me.Reac30)
        Me.ReactionsTab.Controls.Add(Me.Reac25)
        Me.ReactionsTab.Controls.Add(Me.Reac24)
        Me.ReactionsTab.Controls.Add(Me.Reac23)
        Me.ReactionsTab.Controls.Add(Me.Reac22)
        Me.ReactionsTab.Controls.Add(Me.Reac21)
        Me.ReactionsTab.Controls.Add(Me.Reac20)
        Me.ReactionsTab.Controls.Add(Me.Reac19)
        Me.ReactionsTab.Controls.Add(Me.Reac17)
        Me.ReactionsTab.Controls.Add(Me.PanelControl14)
        Me.ReactionsTab.Controls.Add(Me.Reac18)
        Me.ReactionsTab.Controls.Add(Me.Reac75)
        Me.ReactionsTab.Controls.Add(Me.Reac87)
        Me.ReactionsTab.Controls.Add(Me.Reac89)
        Me.ReactionsTab.Controls.Add(Me.Reac101)
        Me.ReactionsTab.Name = "ReactionsTab"
        Me.ReactionsTab.Size = New System.Drawing.Size(762, 623)
        Me.ReactionsTab.Text = "Reactions"
        '
        'Reac2
        '
        Me.Reac2.BackColor = System.Drawing.Color.Transparent
        Me.Reac2.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac2.Image = Nothing
        Me.Reac2.ImagePath = ""
        Me.Reac2.IsTransparentImage = false
        Me.Reac2.Location = New System.Drawing.Point(280, 41)
        Me.Reac2.Name = "Reac2"
        Me.Reac2.Rotation = 3
        Me.Reac2.ShowThrough = true
        Me.Reac2.Size = New System.Drawing.Size(44, 44)
        Me.Reac2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac2.TabIndex = 321
        Me.Reac2.TabStop = false
        Me.Reac2.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac2.WaitOnLoad = true
        '
        'Reac39
        '
        Me.Reac39.BackColor = System.Drawing.Color.Transparent
        Me.Reac39.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac39.Image = Nothing
        Me.Reac39.ImagePath = ""
        Me.Reac39.IsTransparentImage = false
        Me.Reac39.Location = New System.Drawing.Point(493, 392)
        Me.Reac39.Name = "Reac39"
        Me.Reac39.Rotation = 113
        Me.Reac39.ShowThrough = true
        Me.Reac39.Size = New System.Drawing.Size(44, 44)
        Me.Reac39.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac39.TabIndex = 363
        Me.Reac39.TabStop = false
        Me.Reac39.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac39.WaitOnLoad = true
        '
        'Reac29
        '
        Me.Reac29.BackColor = System.Drawing.Color.Transparent
        Me.Reac29.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac29.Image = Nothing
        Me.Reac29.ImagePath = ""
        Me.Reac29.IsTransparentImage = false
        Me.Reac29.Location = New System.Drawing.Point(513, 264)
        Me.Reac29.Name = "Reac29"
        Me.Reac29.Rotation = 83
        Me.Reac29.ShowThrough = true
        Me.Reac29.Size = New System.Drawing.Size(44, 44)
        Me.Reac29.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac29.TabIndex = 353
        Me.Reac29.TabStop = false
        Me.Reac29.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac29.WaitOnLoad = true
        '
        'Reac28
        '
        Me.Reac28.BackColor = System.Drawing.Color.Transparent
        Me.Reac28.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac28.Image = Nothing
        Me.Reac28.ImagePath = ""
        Me.Reac28.IsTransparentImage = false
        Me.Reac28.Location = New System.Drawing.Point(512, 251)
        Me.Reac28.Name = "Reac28"
        Me.Reac28.Rotation = 79
        Me.Reac28.ShowThrough = true
        Me.Reac28.Size = New System.Drawing.Size(44, 44)
        Me.Reac28.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac28.TabIndex = 352
        Me.Reac28.TabStop = false
        Me.Reac28.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac28.WaitOnLoad = true
        '
        'Reac27
        '
        Me.Reac27.BackColor = System.Drawing.Color.Transparent
        Me.Reac27.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac27.Image = Nothing
        Me.Reac27.ImagePath = ""
        Me.Reac27.IsTransparentImage = false
        Me.Reac27.Location = New System.Drawing.Point(509, 239)
        Me.Reac27.Name = "Reac27"
        Me.Reac27.Rotation = 76
        Me.Reac27.ShowThrough = true
        Me.Reac27.Size = New System.Drawing.Size(44, 44)
        Me.Reac27.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac27.TabIndex = 351
        Me.Reac27.TabStop = false
        Me.Reac27.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac27.WaitOnLoad = true
        '
        'Reac16
        '
        Me.Reac16.BackColor = System.Drawing.Color.Transparent
        Me.Reac16.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac16.Image = Nothing
        Me.Reac16.ImagePath = ""
        Me.Reac16.IsTransparentImage = false
        Me.Reac16.Location = New System.Drawing.Point(443, 114)
        Me.Reac16.Name = "Reac16"
        Me.Reac16.Rotation = 43
        Me.Reac16.ShowThrough = true
        Me.Reac16.Size = New System.Drawing.Size(44, 44)
        Me.Reac16.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac16.TabIndex = 339
        Me.Reac16.TabStop = false
        Me.Reac16.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac16.WaitOnLoad = true
        '
        'Reac15
        '
        Me.Reac15.BackColor = System.Drawing.Color.Transparent
        Me.Reac15.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac15.Image = Nothing
        Me.Reac15.ImagePath = ""
        Me.Reac15.IsTransparentImage = false
        Me.Reac15.Location = New System.Drawing.Point(433, 105)
        Me.Reac15.Name = "Reac15"
        Me.Reac15.Rotation = 42
        Me.Reac15.ShowThrough = true
        Me.Reac15.Size = New System.Drawing.Size(44, 44)
        Me.Reac15.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac15.TabIndex = 337
        Me.Reac15.TabStop = false
        Me.Reac15.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac15.WaitOnLoad = true
        '
        'Reac14
        '
        Me.Reac14.BackColor = System.Drawing.Color.Transparent
        Me.Reac14.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac14.Image = Nothing
        Me.Reac14.ImagePath = ""
        Me.Reac14.IsTransparentImage = false
        Me.Reac14.Location = New System.Drawing.Point(422, 97)
        Me.Reac14.Name = "Reac14"
        Me.Reac14.Rotation = 39
        Me.Reac14.ShowThrough = true
        Me.Reac14.Size = New System.Drawing.Size(44, 44)
        Me.Reac14.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac14.TabIndex = 335
        Me.Reac14.TabStop = false
        Me.Reac14.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac14.WaitOnLoad = true
        '
        'Reac13
        '
        Me.Reac13.BackColor = System.Drawing.Color.Transparent
        Me.Reac13.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac13.Image = Nothing
        Me.Reac13.ImagePath = ""
        Me.Reac13.IsTransparentImage = false
        Me.Reac13.Location = New System.Drawing.Point(413, 88)
        Me.Reac13.Name = "Reac13"
        Me.Reac13.Rotation = 36
        Me.Reac13.ShowThrough = true
        Me.Reac13.Size = New System.Drawing.Size(44, 44)
        Me.Reac13.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac13.TabIndex = 333
        Me.Reac13.TabStop = false
        Me.Reac13.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac13.WaitOnLoad = true
        '
        'Reac12
        '
        Me.Reac12.BackColor = System.Drawing.Color.Transparent
        Me.Reac12.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac12.Image = Nothing
        Me.Reac12.ImagePath = ""
        Me.Reac12.IsTransparentImage = false
        Me.Reac12.Location = New System.Drawing.Point(402, 81)
        Me.Reac12.Name = "Reac12"
        Me.Reac12.Rotation = 34
        Me.Reac12.ShowThrough = true
        Me.Reac12.Size = New System.Drawing.Size(44, 44)
        Me.Reac12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac12.TabIndex = 331
        Me.Reac12.TabStop = false
        Me.Reac12.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac12.WaitOnLoad = true
        '
        'Reac11
        '
        Me.Reac11.BackColor = System.Drawing.Color.Transparent
        Me.Reac11.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac11.Image = Nothing
        Me.Reac11.ImagePath = ""
        Me.Reac11.IsTransparentImage = false
        Me.Reac11.Location = New System.Drawing.Point(391, 74)
        Me.Reac11.Name = "Reac11"
        Me.Reac11.Rotation = 29
        Me.Reac11.ShowThrough = true
        Me.Reac11.Size = New System.Drawing.Size(44, 44)
        Me.Reac11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac11.TabIndex = 330
        Me.Reac11.TabStop = false
        Me.Reac11.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac11.WaitOnLoad = true
        '
        'Reac10
        '
        Me.Reac10.BackColor = System.Drawing.Color.Transparent
        Me.Reac10.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac10.Image = Nothing
        Me.Reac10.ImagePath = ""
        Me.Reac10.IsTransparentImage = false
        Me.Reac10.Location = New System.Drawing.Point(379, 68)
        Me.Reac10.Name = "Reac10"
        Me.Reac10.Rotation = 27
        Me.Reac10.ShowThrough = true
        Me.Reac10.Size = New System.Drawing.Size(44, 44)
        Me.Reac10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac10.TabIndex = 329
        Me.Reac10.TabStop = false
        Me.Reac10.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac10.WaitOnLoad = true
        '
        'Reac9
        '
        Me.Reac9.BackColor = System.Drawing.Color.Transparent
        Me.Reac9.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac9.Image = Nothing
        Me.Reac9.ImagePath = ""
        Me.Reac9.IsTransparentImage = false
        Me.Reac9.Location = New System.Drawing.Point(367, 63)
        Me.Reac9.Name = "Reac9"
        Me.Reac9.Rotation = 24
        Me.Reac9.ShowThrough = true
        Me.Reac9.Size = New System.Drawing.Size(44, 44)
        Me.Reac9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac9.TabIndex = 328
        Me.Reac9.TabStop = false
        Me.Reac9.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac9.WaitOnLoad = true
        '
        'Reac8
        '
        Me.Reac8.BackColor = System.Drawing.Color.Transparent
        Me.Reac8.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac8.Image = Nothing
        Me.Reac8.ImagePath = ""
        Me.Reac8.IsTransparentImage = false
        Me.Reac8.Location = New System.Drawing.Point(355, 58)
        Me.Reac8.Name = "Reac8"
        Me.Reac8.Rotation = 21
        Me.Reac8.ShowThrough = true
        Me.Reac8.Size = New System.Drawing.Size(44, 44)
        Me.Reac8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac8.TabIndex = 327
        Me.Reac8.TabStop = false
        Me.Reac8.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac8.WaitOnLoad = true
        '
        'Reac7
        '
        Me.Reac7.BackColor = System.Drawing.Color.Transparent
        Me.Reac7.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac7.Image = Nothing
        Me.Reac7.ImagePath = ""
        Me.Reac7.IsTransparentImage = false
        Me.Reac7.Location = New System.Drawing.Point(343, 53)
        Me.Reac7.Name = "Reac7"
        Me.Reac7.Rotation = 18
        Me.Reac7.ShowThrough = true
        Me.Reac7.Size = New System.Drawing.Size(44, 44)
        Me.Reac7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac7.TabIndex = 326
        Me.Reac7.TabStop = false
        Me.Reac7.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac7.WaitOnLoad = true
        '
        'Reac6
        '
        Me.Reac6.BackColor = System.Drawing.Color.Transparent
        Me.Reac6.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac6.Image = Nothing
        Me.Reac6.ImagePath = ""
        Me.Reac6.IsTransparentImage = false
        Me.Reac6.Location = New System.Drawing.Point(331, 49)
        Me.Reac6.Name = "Reac6"
        Me.Reac6.Rotation = 14
        Me.Reac6.ShowThrough = true
        Me.Reac6.Size = New System.Drawing.Size(44, 44)
        Me.Reac6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac6.TabIndex = 325
        Me.Reac6.TabStop = false
        Me.Reac6.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac6.WaitOnLoad = true
        '
        'Reac5
        '
        Me.Reac5.BackColor = System.Drawing.Color.Transparent
        Me.Reac5.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac5.Image = Nothing
        Me.Reac5.ImagePath = ""
        Me.Reac5.IsTransparentImage = false
        Me.Reac5.Location = New System.Drawing.Point(318, 46)
        Me.Reac5.Name = "Reac5"
        Me.Reac5.Rotation = 12
        Me.Reac5.ShowThrough = true
        Me.Reac5.Size = New System.Drawing.Size(44, 44)
        Me.Reac5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac5.TabIndex = 324
        Me.Reac5.TabStop = false
        Me.Reac5.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac5.WaitOnLoad = true
        '
        'Reac4
        '
        Me.Reac4.BackColor = System.Drawing.Color.Transparent
        Me.Reac4.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac4.Image = Nothing
        Me.Reac4.ImagePath = ""
        Me.Reac4.IsTransparentImage = false
        Me.Reac4.Location = New System.Drawing.Point(306, 44)
        Me.Reac4.Name = "Reac4"
        Me.Reac4.Rotation = 7
        Me.Reac4.ShowThrough = true
        Me.Reac4.Size = New System.Drawing.Size(44, 44)
        Me.Reac4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac4.TabIndex = 323
        Me.Reac4.TabStop = false
        Me.Reac4.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac4.WaitOnLoad = true
        '
        'Reac3
        '
        Me.Reac3.BackColor = System.Drawing.Color.Transparent
        Me.Reac3.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac3.Image = Nothing
        Me.Reac3.ImagePath = ""
        Me.Reac3.IsTransparentImage = false
        Me.Reac3.Location = New System.Drawing.Point(293, 42)
        Me.Reac3.Name = "Reac3"
        Me.Reac3.Rotation = 5
        Me.Reac3.ShowThrough = true
        Me.Reac3.Size = New System.Drawing.Size(44, 44)
        Me.Reac3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac3.TabIndex = 322
        Me.Reac3.TabStop = false
        Me.Reac3.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac3.WaitOnLoad = true
        '
        'Reac1
        '
        Me.Reac1.BackColor = System.Drawing.Color.Transparent
        Me.Reac1.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac1.Image = Nothing
        Me.Reac1.ImagePath = ""
        Me.Reac1.IsTransparentImage = false
        Me.Reac1.Location = New System.Drawing.Point(267, 41)
        Me.Reac1.Name = "Reac1"
        Me.Reac1.Rotation = 0
        Me.Reac1.ShowThrough = true
        Me.Reac1.Size = New System.Drawing.Size(44, 44)
        Me.Reac1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac1.TabIndex = 320
        Me.Reac1.TabStop = false
        Me.Reac1.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac1.WaitOnLoad = true
        '
        'Reac66
        '
        Me.Reac66.BackColor = System.Drawing.Color.Transparent
        Me.Reac66.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac66.Image = Nothing
        Me.Reac66.ImagePath = ""
        Me.Reac66.IsTransparentImage = false
        Me.Reac66.Location = New System.Drawing.Point(202, 531)
        Me.Reac66.Name = "Reac66"
        Me.Reac66.Rotation = 195
        Me.Reac66.ShowThrough = true
        Me.Reac66.Size = New System.Drawing.Size(44, 44)
        Me.Reac66.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac66.TabIndex = 388
        Me.Reac66.TabStop = false
        Me.Reac66.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac66.WaitOnLoad = true
        '
        'Reac67
        '
        Me.Reac67.BackColor = System.Drawing.Color.Transparent
        Me.Reac67.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac67.ErrorImage = Nothing
        Me.Reac67.Image = Nothing
        Me.Reac67.ImagePath = ""
        Me.Reac67.IsTransparentImage = false
        Me.Reac67.Location = New System.Drawing.Point(189, 527)
        Me.Reac67.Name = "Reac67"
        Me.Reac67.Rotation = 197
        Me.Reac67.ShowThrough = true
        Me.Reac67.Size = New System.Drawing.Size(44, 44)
        Me.Reac67.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac67.TabIndex = 389
        Me.Reac67.TabStop = false
        Me.Reac67.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac67.WaitOnLoad = true
        '
        'Reac117
        '
        Me.Reac117.BackColor = System.Drawing.Color.Transparent
        Me.Reac117.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac117.Image = Nothing
        Me.Reac117.ImagePath = ""
        Me.Reac117.IsTransparentImage = false
        Me.Reac117.Location = New System.Drawing.Point(216, 47)
        Me.Reac117.Name = "Reac117"
        Me.Reac117.Rotation = 347
        Me.Reac117.ShowThrough = true
        Me.Reac117.Size = New System.Drawing.Size(44, 44)
        Me.Reac117.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac117.TabIndex = 437
        Me.Reac117.TabStop = false
        Me.Reac117.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac117.WaitOnLoad = true
        '
        'Reac77
        '
        Me.Reac77.BackColor = System.Drawing.Color.Transparent
        Me.Reac77.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac77.Image = Nothing
        Me.Reac77.ImagePath = ""
        Me.Reac77.IsTransparentImage = false
        Me.Reac77.Location = New System.Drawing.Point(82, 456)
        Me.Reac77.Name = "Reac77"
        Me.Reac77.Rotation = 228
        Me.Reac77.ShowThrough = true
        Me.Reac77.Size = New System.Drawing.Size(44, 44)
        Me.Reac77.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac77.TabIndex = 399
        Me.Reac77.TabStop = false
        Me.Reac77.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac77.WaitOnLoad = true
        '
        'Reac78
        '
        Me.Reac78.BackColor = System.Drawing.Color.Transparent
        Me.Reac78.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac78.Image = Nothing
        Me.Reac78.ImagePath = ""
        Me.Reac78.IsTransparentImage = false
        Me.Reac78.Location = New System.Drawing.Point(74, 446)
        Me.Reac78.Name = "Reac78"
        Me.Reac78.Rotation = 230
        Me.Reac78.ShowThrough = true
        Me.Reac78.Size = New System.Drawing.Size(44, 44)
        Me.Reac78.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac78.TabIndex = 400
        Me.Reac78.TabStop = false
        Me.Reac78.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac78.WaitOnLoad = true
        '
        'Reac26
        '
        Me.Reac26.BackColor = System.Drawing.Color.Transparent
        Me.Reac26.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac26.Image = Nothing
        Me.Reac26.ImagePath = ""
        Me.Reac26.IsTransparentImage = false
        Me.Reac26.Location = New System.Drawing.Point(506, 226)
        Me.Reac26.Name = "Reac26"
        Me.Reac26.Rotation = 74
        Me.Reac26.ShowThrough = true
        Me.Reac26.Size = New System.Drawing.Size(44, 44)
        Me.Reac26.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac26.TabIndex = 350
        Me.Reac26.TabStop = false
        Me.Reac26.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac26.WaitOnLoad = true
        '
        'Reac37
        '
        Me.Reac37.BackColor = System.Drawing.Color.Transparent
        Me.Reac37.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac37.Image = Nothing
        Me.Reac37.ImagePath = ""
        Me.Reac37.IsTransparentImage = false
        Me.Reac37.Location = New System.Drawing.Point(502, 368)
        Me.Reac37.Name = "Reac37"
        Me.Reac37.Rotation = 108
        Me.Reac37.ShowThrough = true
        Me.Reac37.Size = New System.Drawing.Size(44, 44)
        Me.Reac37.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac37.TabIndex = 361
        Me.Reac37.TabStop = false
        Me.Reac37.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac37.WaitOnLoad = true
        '
        'Reac38
        '
        Me.Reac38.BackColor = System.Drawing.Color.Transparent
        Me.Reac38.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac38.Image = Nothing
        Me.Reac38.ImagePath = ""
        Me.Reac38.IsTransparentImage = false
        Me.Reac38.Location = New System.Drawing.Point(498, 380)
        Me.Reac38.Name = "Reac38"
        Me.Reac38.Rotation = 109
        Me.Reac38.ShowThrough = true
        Me.Reac38.Size = New System.Drawing.Size(44, 44)
        Me.Reac38.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac38.TabIndex = 362
        Me.Reac38.TabStop = false
        Me.Reac38.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac38.WaitOnLoad = true
        '
        'Reac120
        '
        Me.Reac120.BackColor = System.Drawing.Color.Transparent
        Me.Reac120.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac120.Image = Nothing
        Me.Reac120.ImagePath = ""
        Me.Reac120.IsTransparentImage = false
        Me.Reac120.Location = New System.Drawing.Point(254, 42)
        Me.Reac120.Name = "Reac120"
        Me.Reac120.Rotation = 356
        Me.Reac120.ShowThrough = true
        Me.Reac120.Size = New System.Drawing.Size(44, 44)
        Me.Reac120.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac120.TabIndex = 332
        Me.Reac120.TabStop = false
        Me.Reac120.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac120.WaitOnLoad = true
        '
        'Reac119
        '
        Me.Reac119.BackColor = System.Drawing.Color.Transparent
        Me.Reac119.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac119.Image = Nothing
        Me.Reac119.ImagePath = ""
        Me.Reac119.IsTransparentImage = false
        Me.Reac119.Location = New System.Drawing.Point(241, 43)
        Me.Reac119.Name = "Reac119"
        Me.Reac119.Rotation = 352
        Me.Reac119.ShowThrough = true
        Me.Reac119.Size = New System.Drawing.Size(44, 44)
        Me.Reac119.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac119.TabIndex = 439
        Me.Reac119.TabStop = false
        Me.Reac119.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac119.WaitOnLoad = true
        '
        'Reac118
        '
        Me.Reac118.BackColor = System.Drawing.Color.Transparent
        Me.Reac118.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac118.Image = Nothing
        Me.Reac118.ImagePath = ""
        Me.Reac118.IsTransparentImage = false
        Me.Reac118.Location = New System.Drawing.Point(228, 45)
        Me.Reac118.Name = "Reac118"
        Me.Reac118.Rotation = 350
        Me.Reac118.ShowThrough = true
        Me.Reac118.Size = New System.Drawing.Size(44, 44)
        Me.Reac118.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac118.TabIndex = 438
        Me.Reac118.TabStop = false
        Me.Reac118.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac118.WaitOnLoad = true
        '
        'Reac116
        '
        Me.Reac116.BackColor = System.Drawing.Color.Transparent
        Me.Reac116.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac116.Image = Nothing
        Me.Reac116.ImagePath = ""
        Me.Reac116.IsTransparentImage = false
        Me.Reac116.Location = New System.Drawing.Point(203, 50)
        Me.Reac116.Name = "Reac116"
        Me.Reac116.Rotation = 344
        Me.Reac116.ShowThrough = true
        Me.Reac116.Size = New System.Drawing.Size(44, 44)
        Me.Reac116.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac116.TabIndex = 436
        Me.Reac116.TabStop = false
        Me.Reac116.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac116.WaitOnLoad = true
        '
        'Reac115
        '
        Me.Reac115.BackColor = System.Drawing.Color.Transparent
        Me.Reac115.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac115.Image = Nothing
        Me.Reac115.ImagePath = ""
        Me.Reac115.IsTransparentImage = false
        Me.Reac115.Location = New System.Drawing.Point(191, 54)
        Me.Reac115.Name = "Reac115"
        Me.Reac115.Rotation = 342
        Me.Reac115.ShowThrough = true
        Me.Reac115.Size = New System.Drawing.Size(44, 44)
        Me.Reac115.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac115.TabIndex = 435
        Me.Reac115.TabStop = false
        Me.Reac115.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac115.WaitOnLoad = true
        '
        'Reac114
        '
        Me.Reac114.BackColor = System.Drawing.Color.Transparent
        Me.Reac114.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac114.Image = Nothing
        Me.Reac114.ImagePath = ""
        Me.Reac114.IsTransparentImage = false
        Me.Reac114.Location = New System.Drawing.Point(179, 58)
        Me.Reac114.Name = "Reac114"
        Me.Reac114.Rotation = 339
        Me.Reac114.ShowThrough = true
        Me.Reac114.Size = New System.Drawing.Size(44, 44)
        Me.Reac114.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac114.TabIndex = 434
        Me.Reac114.TabStop = false
        Me.Reac114.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac114.WaitOnLoad = true
        '
        'Reac113
        '
        Me.Reac113.BackColor = System.Drawing.Color.Transparent
        Me.Reac113.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac113.Image = Nothing
        Me.Reac113.ImagePath = ""
        Me.Reac113.IsTransparentImage = false
        Me.Reac113.Location = New System.Drawing.Point(167, 63)
        Me.Reac113.Name = "Reac113"
        Me.Reac113.Rotation = 335
        Me.Reac113.ShowThrough = true
        Me.Reac113.Size = New System.Drawing.Size(44, 44)
        Me.Reac113.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac113.TabIndex = 433
        Me.Reac113.TabStop = false
        Me.Reac113.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac113.WaitOnLoad = true
        '
        'Reac112
        '
        Me.Reac112.BackColor = System.Drawing.Color.Transparent
        Me.Reac112.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac112.Image = Nothing
        Me.Reac112.ImagePath = ""
        Me.Reac112.IsTransparentImage = false
        Me.Reac112.Location = New System.Drawing.Point(155, 68)
        Me.Reac112.Name = "Reac112"
        Me.Reac112.Rotation = 331
        Me.Reac112.ShowThrough = true
        Me.Reac112.Size = New System.Drawing.Size(44, 44)
        Me.Reac112.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac112.TabIndex = 432
        Me.Reac112.TabStop = false
        Me.Reac112.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac112.WaitOnLoad = true
        '
        'Reac111
        '
        Me.Reac111.BackColor = System.Drawing.Color.Transparent
        Me.Reac111.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac111.Image = Nothing
        Me.Reac111.ImagePath = ""
        Me.Reac111.IsTransparentImage = false
        Me.Reac111.Location = New System.Drawing.Point(143, 74)
        Me.Reac111.Name = "Reac111"
        Me.Reac111.Rotation = 328
        Me.Reac111.ShowThrough = true
        Me.Reac111.Size = New System.Drawing.Size(44, 44)
        Me.Reac111.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac111.TabIndex = 431
        Me.Reac111.TabStop = false
        Me.Reac111.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac111.WaitOnLoad = true
        '
        'Reac110
        '
        Me.Reac110.BackColor = System.Drawing.Color.Transparent
        Me.Reac110.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac110.Image = Nothing
        Me.Reac110.ImagePath = ""
        Me.Reac110.IsTransparentImage = false
        Me.Reac110.Location = New System.Drawing.Point(132, 81)
        Me.Reac110.Name = "Reac110"
        Me.Reac110.Rotation = 327
        Me.Reac110.ShowThrough = true
        Me.Reac110.Size = New System.Drawing.Size(44, 44)
        Me.Reac110.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac110.TabIndex = 430
        Me.Reac110.TabStop = false
        Me.Reac110.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac110.WaitOnLoad = true
        '
        'Reac109
        '
        Me.Reac109.BackColor = System.Drawing.Color.Transparent
        Me.Reac109.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac109.Image = Nothing
        Me.Reac109.ImagePath = ""
        Me.Reac109.IsTransparentImage = false
        Me.Reac109.Location = New System.Drawing.Point(122, 89)
        Me.Reac109.Name = "Reac109"
        Me.Reac109.Rotation = 323
        Me.Reac109.ShowThrough = true
        Me.Reac109.Size = New System.Drawing.Size(44, 44)
        Me.Reac109.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac109.TabIndex = 429
        Me.Reac109.TabStop = false
        Me.Reac109.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac109.WaitOnLoad = true
        '
        'Reac108
        '
        Me.Reac108.BackColor = System.Drawing.Color.Transparent
        Me.Reac108.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac108.Image = Nothing
        Me.Reac108.ImagePath = ""
        Me.Reac108.IsTransparentImage = false
        Me.Reac108.Location = New System.Drawing.Point(111, 97)
        Me.Reac108.Name = "Reac108"
        Me.Reac108.Rotation = 320
        Me.Reac108.ShowThrough = true
        Me.Reac108.Size = New System.Drawing.Size(44, 44)
        Me.Reac108.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac108.TabIndex = 428
        Me.Reac108.TabStop = false
        Me.Reac108.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac108.WaitOnLoad = true
        '
        'Reac107
        '
        Me.Reac107.BackColor = System.Drawing.Color.Transparent
        Me.Reac107.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac107.Image = Nothing
        Me.Reac107.ImagePath = ""
        Me.Reac107.IsTransparentImage = false
        Me.Reac107.Location = New System.Drawing.Point(101, 105)
        Me.Reac107.Name = "Reac107"
        Me.Reac107.Rotation = 317
        Me.Reac107.ShowThrough = true
        Me.Reac107.Size = New System.Drawing.Size(44, 44)
        Me.Reac107.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac107.TabIndex = 427
        Me.Reac107.TabStop = false
        Me.Reac107.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac107.WaitOnLoad = true
        '
        'Reac106
        '
        Me.Reac106.BackColor = System.Drawing.Color.Transparent
        Me.Reac106.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac106.Image = Nothing
        Me.Reac106.ImagePath = ""
        Me.Reac106.IsTransparentImage = false
        Me.Reac106.Location = New System.Drawing.Point(92, 114)
        Me.Reac106.Name = "Reac106"
        Me.Reac106.Rotation = 314
        Me.Reac106.ShowThrough = true
        Me.Reac106.Size = New System.Drawing.Size(44, 44)
        Me.Reac106.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac106.TabIndex = 426
        Me.Reac106.TabStop = false
        Me.Reac106.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac106.WaitOnLoad = true
        '
        'Reac105
        '
        Me.Reac105.BackColor = System.Drawing.Color.Transparent
        Me.Reac105.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac105.Image = Nothing
        Me.Reac105.ImagePath = ""
        Me.Reac105.IsTransparentImage = false
        Me.Reac105.Location = New System.Drawing.Point(84, 124)
        Me.Reac105.Name = "Reac105"
        Me.Reac105.Rotation = 311
        Me.Reac105.ShowThrough = true
        Me.Reac105.Size = New System.Drawing.Size(44, 44)
        Me.Reac105.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac105.TabIndex = 425
        Me.Reac105.TabStop = false
        Me.Reac105.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac105.WaitOnLoad = true
        '
        'Reac104
        '
        Me.Reac104.BackColor = System.Drawing.Color.Transparent
        Me.Reac104.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac104.Image = Nothing
        Me.Reac104.ImagePath = ""
        Me.Reac104.IsTransparentImage = false
        Me.Reac104.Location = New System.Drawing.Point(75, 134)
        Me.Reac104.Name = "Reac104"
        Me.Reac104.Rotation = 306
        Me.Reac104.ShowThrough = true
        Me.Reac104.Size = New System.Drawing.Size(44, 44)
        Me.Reac104.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac104.TabIndex = 424
        Me.Reac104.TabStop = false
        Me.Reac104.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac104.WaitOnLoad = true
        '
        'Reac103
        '
        Me.Reac103.BackColor = System.Drawing.Color.Transparent
        Me.Reac103.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac103.Image = Nothing
        Me.Reac103.ImagePath = ""
        Me.Reac103.IsTransparentImage = false
        Me.Reac103.Location = New System.Drawing.Point(67, 144)
        Me.Reac103.Name = "Reac103"
        Me.Reac103.Rotation = 303
        Me.Reac103.ShowThrough = true
        Me.Reac103.Size = New System.Drawing.Size(44, 44)
        Me.Reac103.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac103.TabIndex = 423
        Me.Reac103.TabStop = false
        Me.Reac103.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac103.WaitOnLoad = true
        '
        'Reac102
        '
        Me.Reac102.BackColor = System.Drawing.Color.Transparent
        Me.Reac102.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac102.Image = Nothing
        Me.Reac102.ImagePath = ""
        Me.Reac102.IsTransparentImage = false
        Me.Reac102.Location = New System.Drawing.Point(59, 154)
        Me.Reac102.Name = "Reac102"
        Me.Reac102.Rotation = 301
        Me.Reac102.ShowThrough = true
        Me.Reac102.Size = New System.Drawing.Size(44, 44)
        Me.Reac102.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac102.TabIndex = 422
        Me.Reac102.TabStop = false
        Me.Reac102.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac102.WaitOnLoad = true
        '
        'Reac100
        '
        Me.Reac100.BackColor = System.Drawing.Color.Transparent
        Me.Reac100.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac100.Image = Nothing
        Me.Reac100.ImagePath = ""
        Me.Reac100.IsTransparentImage = false
        Me.Reac100.Location = New System.Drawing.Point(46, 176)
        Me.Reac100.Name = "Reac100"
        Me.Reac100.Rotation = 296
        Me.Reac100.ShowThrough = true
        Me.Reac100.Size = New System.Drawing.Size(44, 44)
        Me.Reac100.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac100.TabIndex = 346
        Me.Reac100.TabStop = false
        Me.Reac100.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac100.WaitOnLoad = true
        '
        'Reac99
        '
        Me.Reac99.BackColor = System.Drawing.Color.Transparent
        Me.Reac99.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac99.Image = Nothing
        Me.Reac99.ImagePath = ""
        Me.Reac99.IsTransparentImage = false
        Me.Reac99.Location = New System.Drawing.Point(41, 188)
        Me.Reac99.Name = "Reac99"
        Me.Reac99.Rotation = 291
        Me.Reac99.ShowThrough = true
        Me.Reac99.Size = New System.Drawing.Size(44, 44)
        Me.Reac99.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac99.TabIndex = 420
        Me.Reac99.TabStop = false
        Me.Reac99.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac99.WaitOnLoad = true
        '
        'Reac98
        '
        Me.Reac98.BackColor = System.Drawing.Color.Transparent
        Me.Reac98.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac98.Image = Nothing
        Me.Reac98.ImagePath = ""
        Me.Reac98.IsTransparentImage = false
        Me.Reac98.Location = New System.Drawing.Point(35, 201)
        Me.Reac98.Name = "Reac98"
        Me.Reac98.Rotation = 289
        Me.Reac98.ShowThrough = true
        Me.Reac98.Size = New System.Drawing.Size(44, 44)
        Me.Reac98.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac98.TabIndex = 419
        Me.Reac98.TabStop = false
        Me.Reac98.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac98.WaitOnLoad = true
        '
        'Reac97
        '
        Me.Reac97.BackColor = System.Drawing.Color.Transparent
        Me.Reac97.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac97.ErrorImage = Nothing
        Me.Reac97.Image = Nothing
        Me.Reac97.ImagePath = ""
        Me.Reac97.IsTransparentImage = false
        Me.Reac97.Location = New System.Drawing.Point(31, 213)
        Me.Reac97.Name = "Reac97"
        Me.Reac97.Rotation = 288
        Me.Reac97.ShowThrough = true
        Me.Reac97.Size = New System.Drawing.Size(44, 44)
        Me.Reac97.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac97.TabIndex = 418
        Me.Reac97.TabStop = false
        Me.Reac97.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac97.WaitOnLoad = true
        '
        'Reac96
        '
        Me.Reac96.BackColor = System.Drawing.Color.Transparent
        Me.Reac96.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac96.Image = Nothing
        Me.Reac96.ImagePath = ""
        Me.Reac96.IsTransparentImage = false
        Me.Reac96.Location = New System.Drawing.Point(27, 225)
        Me.Reac96.Name = "Reac96"
        Me.Reac96.Rotation = 285
        Me.Reac96.ShowThrough = true
        Me.Reac96.Size = New System.Drawing.Size(44, 44)
        Me.Reac96.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac96.TabIndex = 417
        Me.Reac96.TabStop = false
        Me.Reac96.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac96.WaitOnLoad = true
        '
        'Reac95
        '
        Me.Reac95.BackColor = System.Drawing.Color.Transparent
        Me.Reac95.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac95.Image = Nothing
        Me.Reac95.ImagePath = ""
        Me.Reac95.IsTransparentImage = false
        Me.Reac95.Location = New System.Drawing.Point(24, 238)
        Me.Reac95.Name = "Reac95"
        Me.Reac95.Rotation = 283
        Me.Reac95.ShowThrough = true
        Me.Reac95.Size = New System.Drawing.Size(44, 44)
        Me.Reac95.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac95.TabIndex = 416
        Me.Reac95.TabStop = false
        Me.Reac95.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac95.WaitOnLoad = true
        '
        'Reac94
        '
        Me.Reac94.BackColor = System.Drawing.Color.Transparent
        Me.Reac94.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac94.ErrorImage = Nothing
        Me.Reac94.Image = Nothing
        Me.Reac94.ImagePath = ""
        Me.Reac94.IsTransparentImage = false
        Me.Reac94.Location = New System.Drawing.Point(22, 250)
        Me.Reac94.Name = "Reac94"
        Me.Reac94.Rotation = 278
        Me.Reac94.ShowThrough = true
        Me.Reac94.Size = New System.Drawing.Size(44, 44)
        Me.Reac94.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac94.TabIndex = 415
        Me.Reac94.TabStop = false
        Me.Reac94.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac94.WaitOnLoad = true
        '
        'Reac93
        '
        Me.Reac93.BackColor = System.Drawing.Color.Transparent
        Me.Reac93.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac93.Image = Nothing
        Me.Reac93.ImagePath = ""
        Me.Reac93.IsTransparentImage = false
        Me.Reac93.Location = New System.Drawing.Point(20, 263)
        Me.Reac93.Name = "Reac93"
        Me.Reac93.Rotation = 276
        Me.Reac93.ShowThrough = true
        Me.Reac93.Size = New System.Drawing.Size(44, 44)
        Me.Reac93.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac93.TabIndex = 414
        Me.Reac93.TabStop = false
        Me.Reac93.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac93.WaitOnLoad = true
        '
        'Reac92
        '
        Me.Reac92.BackColor = System.Drawing.Color.Transparent
        Me.Reac92.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac92.Image = Nothing
        Me.Reac92.ImagePath = ""
        Me.Reac92.IsTransparentImage = false
        Me.Reac92.Location = New System.Drawing.Point(19, 276)
        Me.Reac92.Name = "Reac92"
        Me.Reac92.Rotation = 272
        Me.Reac92.ShowThrough = true
        Me.Reac92.Size = New System.Drawing.Size(44, 44)
        Me.Reac92.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac92.TabIndex = 413
        Me.Reac92.TabStop = false
        Me.Reac92.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac92.WaitOnLoad = true
        '
        'Reac91
        '
        Me.Reac91.BackColor = System.Drawing.Color.Transparent
        Me.Reac91.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac91.ErrorImage = Nothing
        Me.Reac91.Image = Nothing
        Me.Reac91.ImagePath = ""
        Me.Reac91.IsTransparentImage = false
        Me.Reac91.Location = New System.Drawing.Point(19, 289)
        Me.Reac91.Name = "Reac91"
        Me.Reac91.Rotation = 269
        Me.Reac91.ShowThrough = true
        Me.Reac91.Size = New System.Drawing.Size(44, 44)
        Me.Reac91.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac91.TabIndex = 412
        Me.Reac91.TabStop = false
        Me.Reac91.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac91.WaitOnLoad = true
        '
        'Reac90
        '
        Me.Reac90.BackColor = System.Drawing.Color.Transparent
        Me.Reac90.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac90.Image = Nothing
        Me.Reac90.ImagePath = ""
        Me.Reac90.IsTransparentImage = false
        Me.Reac90.Location = New System.Drawing.Point(19, 302)
        Me.Reac90.Name = "Reac90"
        Me.Reac90.Rotation = 266
        Me.Reac90.ShowThrough = true
        Me.Reac90.Size = New System.Drawing.Size(44, 44)
        Me.Reac90.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac90.TabIndex = 411
        Me.Reac90.TabStop = false
        Me.Reac90.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac90.WaitOnLoad = true
        '
        'Reac88
        '
        Me.Reac88.BackColor = System.Drawing.Color.Transparent
        Me.Reac88.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac88.ErrorImage = Nothing
        Me.Reac88.Image = Nothing
        Me.Reac88.ImagePath = ""
        Me.Reac88.IsTransparentImage = false
        Me.Reac88.Location = New System.Drawing.Point(22, 328)
        Me.Reac88.Name = "Reac88"
        Me.Reac88.Rotation = 259
        Me.Reac88.ShowThrough = true
        Me.Reac88.Size = New System.Drawing.Size(44, 44)
        Me.Reac88.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac88.TabIndex = 409
        Me.Reac88.TabStop = false
        Me.Reac88.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac88.WaitOnLoad = true
        '
        'Reac86
        '
        Me.Reac86.BackColor = System.Drawing.Color.Transparent
        Me.Reac86.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac86.Image = Nothing
        Me.Reac86.ImagePath = ""
        Me.Reac86.IsTransparentImage = false
        Me.Reac86.Location = New System.Drawing.Point(27, 354)
        Me.Reac86.Name = "Reac86"
        Me.Reac86.Rotation = 253
        Me.Reac86.ShowThrough = true
        Me.Reac86.Size = New System.Drawing.Size(44, 44)
        Me.Reac86.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac86.TabIndex = 407
        Me.Reac86.TabStop = false
        Me.Reac86.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac86.WaitOnLoad = true
        '
        'Reac85
        '
        Me.Reac85.BackColor = System.Drawing.Color.Transparent
        Me.Reac85.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac85.Image = Nothing
        Me.Reac85.ImagePath = ""
        Me.Reac85.IsTransparentImage = false
        Me.Reac85.Location = New System.Drawing.Point(31, 366)
        Me.Reac85.Name = "Reac85"
        Me.Reac85.Rotation = 250
        Me.Reac85.ShowThrough = true
        Me.Reac85.Size = New System.Drawing.Size(44, 44)
        Me.Reac85.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac85.TabIndex = 406
        Me.Reac85.TabStop = false
        Me.Reac85.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac85.WaitOnLoad = true
        '
        'Reac84
        '
        Me.Reac84.BackColor = System.Drawing.Color.Transparent
        Me.Reac84.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac84.ErrorImage = Nothing
        Me.Reac84.Image = Nothing
        Me.Reac84.ImagePath = ""
        Me.Reac84.IsTransparentImage = false
        Me.Reac84.Location = New System.Drawing.Point(35, 378)
        Me.Reac84.Name = "Reac84"
        Me.Reac84.Rotation = 246
        Me.Reac84.ShowThrough = true
        Me.Reac84.Size = New System.Drawing.Size(44, 44)
        Me.Reac84.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac84.TabIndex = 405
        Me.Reac84.TabStop = false
        Me.Reac84.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac84.WaitOnLoad = true
        '
        'Reac83
        '
        Me.Reac83.BackColor = System.Drawing.Color.Transparent
        Me.Reac83.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac83.Image = Nothing
        Me.Reac83.ImagePath = ""
        Me.Reac83.IsTransparentImage = false
        Me.Reac83.Location = New System.Drawing.Point(40, 391)
        Me.Reac83.Name = "Reac83"
        Me.Reac83.Rotation = 243
        Me.Reac83.ShowThrough = true
        Me.Reac83.Size = New System.Drawing.Size(44, 44)
        Me.Reac83.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac83.TabIndex = 404
        Me.Reac83.TabStop = false
        Me.Reac83.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac83.WaitOnLoad = true
        '
        'Reac82
        '
        Me.Reac82.BackColor = System.Drawing.Color.Transparent
        Me.Reac82.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac82.Image = Nothing
        Me.Reac82.ImagePath = ""
        Me.Reac82.IsTransparentImage = false
        Me.Reac82.Location = New System.Drawing.Point(45, 403)
        Me.Reac82.Name = "Reac82"
        Me.Reac82.Rotation = 240
        Me.Reac82.ShowThrough = true
        Me.Reac82.Size = New System.Drawing.Size(44, 44)
        Me.Reac82.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac82.TabIndex = 403
        Me.Reac82.TabStop = false
        Me.Reac82.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac82.WaitOnLoad = true
        '
        'Reac81
        '
        Me.Reac81.BackColor = System.Drawing.Color.Transparent
        Me.Reac81.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac81.Image = Nothing
        Me.Reac81.ImagePath = ""
        Me.Reac81.IsTransparentImage = false
        Me.Reac81.Location = New System.Drawing.Point(52, 414)
        Me.Reac81.Name = "Reac81"
        Me.Reac81.Rotation = 238
        Me.Reac81.ShowThrough = true
        Me.Reac81.Size = New System.Drawing.Size(44, 44)
        Me.Reac81.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac81.TabIndex = 402
        Me.Reac81.TabStop = false
        Me.Reac81.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac81.WaitOnLoad = true
        '
        'Reac80
        '
        Me.Reac80.BackColor = System.Drawing.Color.Transparent
        Me.Reac80.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac80.ErrorImage = Nothing
        Me.Reac80.Image = Nothing
        Me.Reac80.ImagePath = ""
        Me.Reac80.IsTransparentImage = false
        Me.Reac80.Location = New System.Drawing.Point(59, 425)
        Me.Reac80.Name = "Reac80"
        Me.Reac80.Rotation = 236
        Me.Reac80.ShowThrough = true
        Me.Reac80.Size = New System.Drawing.Size(44, 44)
        Me.Reac80.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac80.TabIndex = 336
        Me.Reac80.TabStop = false
        Me.Reac80.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac80.WaitOnLoad = true
        '
        'Reac79
        '
        Me.Reac79.BackColor = System.Drawing.Color.Transparent
        Me.Reac79.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac79.Image = Nothing
        Me.Reac79.ImagePath = ""
        Me.Reac79.IsTransparentImage = false
        Me.Reac79.Location = New System.Drawing.Point(66, 436)
        Me.Reac79.Name = "Reac79"
        Me.Reac79.Rotation = 234
        Me.Reac79.ShowThrough = true
        Me.Reac79.Size = New System.Drawing.Size(44, 44)
        Me.Reac79.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac79.TabIndex = 401
        Me.Reac79.TabStop = false
        Me.Reac79.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac79.WaitOnLoad = true
        '
        'Reac76
        '
        Me.Reac76.BackColor = System.Drawing.Color.Transparent
        Me.Reac76.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac76.ErrorImage = Nothing
        Me.Reac76.Image = Nothing
        Me.Reac76.ImagePath = ""
        Me.Reac76.IsTransparentImage = false
        Me.Reac76.Location = New System.Drawing.Point(91, 466)
        Me.Reac76.Name = "Reac76"
        Me.Reac76.Rotation = 224
        Me.Reac76.ShowThrough = true
        Me.Reac76.Size = New System.Drawing.Size(44, 44)
        Me.Reac76.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac76.TabIndex = 398
        Me.Reac76.TabStop = false
        Me.Reac76.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac76.WaitOnLoad = true
        '
        'Reac74
        '
        Me.Reac74.BackColor = System.Drawing.Color.Transparent
        Me.Reac74.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac74.Image = Nothing
        Me.Reac74.ImagePath = ""
        Me.Reac74.IsTransparentImage = false
        Me.Reac74.Location = New System.Drawing.Point(110, 483)
        Me.Reac74.Name = "Reac74"
        Me.Reac74.Rotation = 218
        Me.Reac74.ShowThrough = true
        Me.Reac74.Size = New System.Drawing.Size(44, 44)
        Me.Reac74.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac74.TabIndex = 396
        Me.Reac74.TabStop = false
        Me.Reac74.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac74.WaitOnLoad = true
        '
        'Reac73
        '
        Me.Reac73.BackColor = System.Drawing.Color.Transparent
        Me.Reac73.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac73.ErrorImage = Nothing
        Me.Reac73.Image = Nothing
        Me.Reac73.ImagePath = ""
        Me.Reac73.IsTransparentImage = false
        Me.Reac73.Location = New System.Drawing.Point(121, 491)
        Me.Reac73.Name = "Reac73"
        Me.Reac73.Rotation = 215
        Me.Reac73.ShowThrough = true
        Me.Reac73.Size = New System.Drawing.Size(44, 44)
        Me.Reac73.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac73.TabIndex = 395
        Me.Reac73.TabStop = false
        Me.Reac73.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac73.WaitOnLoad = true
        '
        'Reac72
        '
        Me.Reac72.BackColor = System.Drawing.Color.Transparent
        Me.Reac72.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac72.Image = Nothing
        Me.Reac72.ImagePath = ""
        Me.Reac72.IsTransparentImage = false
        Me.Reac72.Location = New System.Drawing.Point(131, 499)
        Me.Reac72.Name = "Reac72"
        Me.Reac72.Rotation = 211
        Me.Reac72.ShowThrough = true
        Me.Reac72.Size = New System.Drawing.Size(44, 44)
        Me.Reac72.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac72.TabIndex = 394
        Me.Reac72.TabStop = false
        Me.Reac72.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac72.WaitOnLoad = true
        '
        'Reac71
        '
        Me.Reac71.BackColor = System.Drawing.Color.Transparent
        Me.Reac71.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac71.Image = Nothing
        Me.Reac71.ImagePath = ""
        Me.Reac71.IsTransparentImage = false
        Me.Reac71.Location = New System.Drawing.Point(142, 505)
        Me.Reac71.Name = "Reac71"
        Me.Reac71.Rotation = 208
        Me.Reac71.ShowThrough = true
        Me.Reac71.Size = New System.Drawing.Size(44, 44)
        Me.Reac71.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac71.TabIndex = 393
        Me.Reac71.TabStop = false
        Me.Reac71.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac71.WaitOnLoad = true
        '
        'Reac70
        '
        Me.Reac70.BackColor = System.Drawing.Color.Transparent
        Me.Reac70.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac70.ErrorImage = Nothing
        Me.Reac70.Image = Nothing
        Me.Reac70.ImagePath = ""
        Me.Reac70.IsTransparentImage = false
        Me.Reac70.Location = New System.Drawing.Point(153, 512)
        Me.Reac70.Name = "Reac70"
        Me.Reac70.Rotation = 205
        Me.Reac70.ShowThrough = true
        Me.Reac70.Size = New System.Drawing.Size(44, 44)
        Me.Reac70.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac70.TabIndex = 392
        Me.Reac70.TabStop = false
        Me.Reac70.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac70.WaitOnLoad = true
        '
        'Reac69
        '
        Me.Reac69.BackColor = System.Drawing.Color.Transparent
        Me.Reac69.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac69.Image = Nothing
        Me.Reac69.ImagePath = ""
        Me.Reac69.IsTransparentImage = false
        Me.Reac69.Location = New System.Drawing.Point(165, 518)
        Me.Reac69.Name = "Reac69"
        Me.Reac69.Rotation = 202
        Me.Reac69.ShowThrough = true
        Me.Reac69.Size = New System.Drawing.Size(44, 44)
        Me.Reac69.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac69.TabIndex = 391
        Me.Reac69.TabStop = false
        Me.Reac69.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac69.WaitOnLoad = true
        '
        'Reac68
        '
        Me.Reac68.BackColor = System.Drawing.Color.Transparent
        Me.Reac68.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac68.Image = Nothing
        Me.Reac68.ImagePath = ""
        Me.Reac68.IsTransparentImage = false
        Me.Reac68.Location = New System.Drawing.Point(177, 523)
        Me.Reac68.Name = "Reac68"
        Me.Reac68.Rotation = 199
        Me.Reac68.ShowThrough = true
        Me.Reac68.Size = New System.Drawing.Size(44, 44)
        Me.Reac68.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac68.TabIndex = 390
        Me.Reac68.TabStop = false
        Me.Reac68.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac68.WaitOnLoad = true
        '
        'Reac65
        '
        Me.Reac65.BackColor = System.Drawing.Color.Transparent
        Me.Reac65.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac65.Image = Nothing
        Me.Reac65.ImagePath = ""
        Me.Reac65.IsTransparentImage = false
        Me.Reac65.Location = New System.Drawing.Point(215, 534)
        Me.Reac65.Name = "Reac65"
        Me.Reac65.Rotation = 194
        Me.Reac65.ShowThrough = true
        Me.Reac65.Size = New System.Drawing.Size(44, 44)
        Me.Reac65.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac65.TabIndex = 387
        Me.Reac65.TabStop = false
        Me.Reac65.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac65.WaitOnLoad = true
        '
        'Reac64
        '
        Me.Reac64.BackColor = System.Drawing.Color.Transparent
        Me.Reac64.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac64.Image = Nothing
        Me.Reac64.ImagePath = ""
        Me.Reac64.IsTransparentImage = false
        Me.Reac64.Location = New System.Drawing.Point(227, 536)
        Me.Reac64.Name = "Reac64"
        Me.Reac64.Rotation = 188
        Me.Reac64.ShowThrough = true
        Me.Reac64.Size = New System.Drawing.Size(44, 44)
        Me.Reac64.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac64.TabIndex = 386
        Me.Reac64.TabStop = false
        Me.Reac64.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac64.WaitOnLoad = true
        '
        'Reac63
        '
        Me.Reac63.BackColor = System.Drawing.Color.Transparent
        Me.Reac63.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac63.ErrorImage = Nothing
        Me.Reac63.Image = Nothing
        Me.Reac63.ImagePath = ""
        Me.Reac63.IsTransparentImage = false
        Me.Reac63.Location = New System.Drawing.Point(240, 538)
        Me.Reac63.Name = "Reac63"
        Me.Reac63.Rotation = 185
        Me.Reac63.ShowThrough = true
        Me.Reac63.Size = New System.Drawing.Size(44, 44)
        Me.Reac63.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac63.TabIndex = 385
        Me.Reac63.TabStop = false
        Me.Reac63.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac63.WaitOnLoad = true
        '
        'Reac62
        '
        Me.Reac62.BackColor = System.Drawing.Color.Transparent
        Me.Reac62.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac62.Image = Nothing
        Me.Reac62.ImagePath = ""
        Me.Reac62.IsTransparentImage = false
        Me.Reac62.Location = New System.Drawing.Point(253, 539)
        Me.Reac62.Name = "Reac62"
        Me.Reac62.Rotation = 182
        Me.Reac62.ShowThrough = true
        Me.Reac62.Size = New System.Drawing.Size(44, 44)
        Me.Reac62.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac62.TabIndex = 384
        Me.Reac62.TabStop = false
        Me.Reac62.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac62.WaitOnLoad = true
        '
        'Reac61
        '
        Me.Reac61.BackColor = System.Drawing.Color.Transparent
        Me.Reac61.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac61.Image = Nothing
        Me.Reac61.ImagePath = ""
        Me.Reac61.IsTransparentImage = false
        Me.Reac61.Location = New System.Drawing.Point(267, 540)
        Me.Reac61.Name = "Reac61"
        Me.Reac61.Rotation = 180
        Me.Reac61.ShowThrough = true
        Me.Reac61.Size = New System.Drawing.Size(44, 44)
        Me.Reac61.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac61.TabIndex = 383
        Me.Reac61.TabStop = false
        Me.Reac61.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac61.WaitOnLoad = true
        '
        'Reac60
        '
        Me.Reac60.BackColor = System.Drawing.Color.Transparent
        Me.Reac60.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac60.Image = Nothing
        Me.Reac60.ImagePath = ""
        Me.Reac60.IsTransparentImage = false
        Me.Reac60.Location = New System.Drawing.Point(279, 539)
        Me.Reac60.Name = "Reac60"
        Me.Reac60.Rotation = 178
        Me.Reac60.ShowThrough = true
        Me.Reac60.Size = New System.Drawing.Size(44, 44)
        Me.Reac60.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac60.TabIndex = 334
        Me.Reac60.TabStop = false
        Me.Reac60.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac60.WaitOnLoad = true
        '
        'Reac59
        '
        Me.Reac59.BackColor = System.Drawing.Color.Transparent
        Me.Reac59.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac59.ErrorImage = Nothing
        Me.Reac59.Image = Nothing
        Me.Reac59.ImagePath = ""
        Me.Reac59.IsTransparentImage = false
        Me.Reac59.Location = New System.Drawing.Point(292, 538)
        Me.Reac59.Name = "Reac59"
        Me.Reac59.Rotation = 174
        Me.Reac59.ShowThrough = true
        Me.Reac59.Size = New System.Drawing.Size(44, 44)
        Me.Reac59.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac59.TabIndex = 382
        Me.Reac59.TabStop = false
        Me.Reac59.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac59.WaitOnLoad = true
        '
        'Reac58
        '
        Me.Reac58.BackColor = System.Drawing.Color.Transparent
        Me.Reac58.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac58.Image = Nothing
        Me.Reac58.ImagePath = ""
        Me.Reac58.IsTransparentImage = false
        Me.Reac58.Location = New System.Drawing.Point(305, 537)
        Me.Reac58.Name = "Reac58"
        Me.Reac58.Rotation = 169
        Me.Reac58.ShowThrough = true
        Me.Reac58.Size = New System.Drawing.Size(44, 44)
        Me.Reac58.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac58.TabIndex = 381
        Me.Reac58.TabStop = false
        Me.Reac58.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac58.WaitOnLoad = true
        '
        'Reac57
        '
        Me.Reac57.BackColor = System.Drawing.Color.Transparent
        Me.Reac57.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac57.Image = Nothing
        Me.Reac57.ImagePath = ""
        Me.Reac57.IsTransparentImage = false
        Me.Reac57.Location = New System.Drawing.Point(318, 534)
        Me.Reac57.Name = "Reac57"
        Me.Reac57.Rotation = 166
        Me.Reac57.ShowThrough = true
        Me.Reac57.Size = New System.Drawing.Size(44, 44)
        Me.Reac57.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac57.TabIndex = 380
        Me.Reac57.TabStop = false
        Me.Reac57.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac57.WaitOnLoad = true
        '
        'Reac56
        '
        Me.Reac56.BackColor = System.Drawing.Color.Transparent
        Me.Reac56.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac56.ErrorImage = Nothing
        Me.Reac56.Image = Nothing
        Me.Reac56.ImagePath = ""
        Me.Reac56.IsTransparentImage = false
        Me.Reac56.Location = New System.Drawing.Point(330, 531)
        Me.Reac56.Name = "Reac56"
        Me.Reac56.Rotation = 163
        Me.Reac56.ShowThrough = true
        Me.Reac56.Size = New System.Drawing.Size(44, 44)
        Me.Reac56.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac56.TabIndex = 379
        Me.Reac56.TabStop = false
        Me.Reac56.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac56.WaitOnLoad = true
        '
        'Reac55
        '
        Me.Reac55.BackColor = System.Drawing.Color.Transparent
        Me.Reac55.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac55.ErrorImage = Nothing
        Me.Reac55.Image = Nothing
        Me.Reac55.ImagePath = ""
        Me.Reac55.IsTransparentImage = false
        Me.Reac55.Location = New System.Drawing.Point(343, 527)
        Me.Reac55.Name = "Reac55"
        Me.Reac55.Rotation = 161
        Me.Reac55.ShowThrough = true
        Me.Reac55.Size = New System.Drawing.Size(44, 44)
        Me.Reac55.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac55.TabIndex = 378
        Me.Reac55.TabStop = false
        Me.Reac55.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac55.WaitOnLoad = true
        '
        'Reac54
        '
        Me.Reac54.BackColor = System.Drawing.Color.Transparent
        Me.Reac54.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac54.Image = Nothing
        Me.Reac54.ImagePath = ""
        Me.Reac54.IsTransparentImage = false
        Me.Reac54.Location = New System.Drawing.Point(355, 523)
        Me.Reac54.Name = "Reac54"
        Me.Reac54.Rotation = 158
        Me.Reac54.ShowThrough = true
        Me.Reac54.Size = New System.Drawing.Size(44, 44)
        Me.Reac54.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac54.TabIndex = 377
        Me.Reac54.TabStop = false
        Me.Reac54.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac54.WaitOnLoad = true
        '
        'Reac53
        '
        Me.Reac53.BackColor = System.Drawing.Color.Transparent
        Me.Reac53.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac53.ErrorImage = Nothing
        Me.Reac53.Image = Nothing
        Me.Reac53.ImagePath = ""
        Me.Reac53.IsTransparentImage = false
        Me.Reac53.Location = New System.Drawing.Point(367, 518)
        Me.Reac53.Name = "Reac53"
        Me.Reac53.Rotation = 157
        Me.Reac53.ShowThrough = true
        Me.Reac53.Size = New System.Drawing.Size(44, 44)
        Me.Reac53.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac53.TabIndex = 376
        Me.Reac53.TabStop = false
        Me.Reac53.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac53.WaitOnLoad = true
        '
        'Reac52
        '
        Me.Reac52.BackColor = System.Drawing.Color.Transparent
        Me.Reac52.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac52.Image = Nothing
        Me.Reac52.ImagePath = ""
        Me.Reac52.IsTransparentImage = false
        Me.Reac52.Location = New System.Drawing.Point(379, 512)
        Me.Reac52.Name = "Reac52"
        Me.Reac52.Rotation = 152
        Me.Reac52.ShowThrough = true
        Me.Reac52.Size = New System.Drawing.Size(44, 44)
        Me.Reac52.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac52.TabIndex = 375
        Me.Reac52.TabStop = false
        Me.Reac52.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac52.WaitOnLoad = true
        '
        'Reac51
        '
        Me.Reac51.BackColor = System.Drawing.Color.Transparent
        Me.Reac51.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac51.Image = Nothing
        Me.Reac51.ImagePath = ""
        Me.Reac51.IsTransparentImage = false
        Me.Reac51.Location = New System.Drawing.Point(390, 506)
        Me.Reac51.Name = "Reac51"
        Me.Reac51.Rotation = 150
        Me.Reac51.ShowThrough = true
        Me.Reac51.Size = New System.Drawing.Size(44, 44)
        Me.Reac51.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac51.TabIndex = 374
        Me.Reac51.TabStop = false
        Me.Reac51.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac51.WaitOnLoad = true
        '
        'Reac50
        '
        Me.Reac50.BackColor = System.Drawing.Color.Transparent
        Me.Reac50.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac50.Image = Nothing
        Me.Reac50.ImagePath = ""
        Me.Reac50.IsTransparentImage = false
        Me.Reac50.Location = New System.Drawing.Point(401, 499)
        Me.Reac50.Name = "Reac50"
        Me.Reac50.Rotation = 147
        Me.Reac50.ShowThrough = true
        Me.Reac50.Size = New System.Drawing.Size(44, 44)
        Me.Reac50.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac50.TabIndex = 369
        Me.Reac50.TabStop = false
        Me.Reac50.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac50.WaitOnLoad = true
        '
        'Reac49
        '
        Me.Reac49.BackColor = System.Drawing.Color.Transparent
        Me.Reac49.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac49.Image = Nothing
        Me.Reac49.ImagePath = ""
        Me.Reac49.IsTransparentImage = false
        Me.Reac49.Location = New System.Drawing.Point(412, 492)
        Me.Reac49.Name = "Reac49"
        Me.Reac49.Rotation = 143
        Me.Reac49.ShowThrough = true
        Me.Reac49.Size = New System.Drawing.Size(44, 44)
        Me.Reac49.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac49.TabIndex = 373
        Me.Reac49.TabStop = false
        Me.Reac49.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac49.WaitOnLoad = true
        '
        'Reac48
        '
        Me.Reac48.BackColor = System.Drawing.Color.Transparent
        Me.Reac48.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac48.Image = Nothing
        Me.Reac48.ImagePath = ""
        Me.Reac48.IsTransparentImage = false
        Me.Reac48.Location = New System.Drawing.Point(422, 484)
        Me.Reac48.Name = "Reac48"
        Me.Reac48.Rotation = 141
        Me.Reac48.ShowThrough = true
        Me.Reac48.Size = New System.Drawing.Size(44, 44)
        Me.Reac48.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac48.TabIndex = 372
        Me.Reac48.TabStop = false
        Me.Reac48.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac48.WaitOnLoad = true
        '
        'Reac47
        '
        Me.Reac47.BackColor = System.Drawing.Color.Transparent
        Me.Reac47.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac47.Image = Nothing
        Me.Reac47.ImagePath = ""
        Me.Reac47.IsTransparentImage = false
        Me.Reac47.Location = New System.Drawing.Point(432, 475)
        Me.Reac47.Name = "Reac47"
        Me.Reac47.Rotation = 139
        Me.Reac47.ShowThrough = true
        Me.Reac47.Size = New System.Drawing.Size(44, 44)
        Me.Reac47.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac47.TabIndex = 371
        Me.Reac47.TabStop = false
        Me.Reac47.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac47.WaitOnLoad = true
        '
        'Reac46
        '
        Me.Reac46.BackColor = System.Drawing.Color.Transparent
        Me.Reac46.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac46.Image = Nothing
        Me.Reac46.ImagePath = ""
        Me.Reac46.IsTransparentImage = false
        Me.Reac46.Location = New System.Drawing.Point(441, 466)
        Me.Reac46.Name = "Reac46"
        Me.Reac46.Rotation = 136
        Me.Reac46.ShowThrough = true
        Me.Reac46.Size = New System.Drawing.Size(44, 44)
        Me.Reac46.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac46.TabIndex = 370
        Me.Reac46.TabStop = false
        Me.Reac46.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac46.WaitOnLoad = true
        '
        'Reac45
        '
        Me.Reac45.BackColor = System.Drawing.Color.Transparent
        Me.Reac45.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac45.Image = Nothing
        Me.Reac45.ImagePath = ""
        Me.Reac45.IsTransparentImage = false
        Me.Reac45.Location = New System.Drawing.Point(450, 457)
        Me.Reac45.Name = "Reac45"
        Me.Reac45.Rotation = 132
        Me.Reac45.ShowThrough = true
        Me.Reac45.Size = New System.Drawing.Size(44, 44)
        Me.Reac45.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac45.TabIndex = 368
        Me.Reac45.TabStop = false
        Me.Reac45.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac45.WaitOnLoad = true
        '
        'Reac44
        '
        Me.Reac44.BackColor = System.Drawing.Color.Transparent
        Me.Reac44.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac44.Image = Nothing
        Me.Reac44.ImagePath = ""
        Me.Reac44.IsTransparentImage = false
        Me.Reac44.Location = New System.Drawing.Point(459, 447)
        Me.Reac44.Name = "Reac44"
        Me.Reac44.Rotation = 128
        Me.Reac44.ShowThrough = true
        Me.Reac44.Size = New System.Drawing.Size(44, 44)
        Me.Reac44.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac44.TabIndex = 367
        Me.Reac44.TabStop = false
        Me.Reac44.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac44.WaitOnLoad = true
        '
        'Reac43
        '
        Me.Reac43.BackColor = System.Drawing.Color.Transparent
        Me.Reac43.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac43.Image = Nothing
        Me.Reac43.ImagePath = ""
        Me.Reac43.IsTransparentImage = false
        Me.Reac43.Location = New System.Drawing.Point(467, 437)
        Me.Reac43.Name = "Reac43"
        Me.Reac43.Rotation = 125
        Me.Reac43.ShowThrough = true
        Me.Reac43.Size = New System.Drawing.Size(44, 44)
        Me.Reac43.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac43.TabIndex = 366
        Me.Reac43.TabStop = false
        Me.Reac43.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac43.WaitOnLoad = true
        '
        'Reac42
        '
        Me.Reac42.BackColor = System.Drawing.Color.Transparent
        Me.Reac42.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac42.Image = Nothing
        Me.Reac42.ImagePath = ""
        Me.Reac42.IsTransparentImage = false
        Me.Reac42.Location = New System.Drawing.Point(474, 426)
        Me.Reac42.Name = "Reac42"
        Me.Reac42.Rotation = 122
        Me.Reac42.ShowThrough = true
        Me.Reac42.Size = New System.Drawing.Size(44, 44)
        Me.Reac42.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac42.TabIndex = 365
        Me.Reac42.TabStop = false
        Me.Reac42.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac42.WaitOnLoad = true
        '
        'Reac41
        '
        Me.Reac41.BackColor = System.Drawing.Color.Transparent
        Me.Reac41.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac41.Image = Nothing
        Me.Reac41.ImagePath = ""
        Me.Reac41.IsTransparentImage = false
        Me.Reac41.Location = New System.Drawing.Point(481, 415)
        Me.Reac41.Name = "Reac41"
        Me.Reac41.Rotation = 118
        Me.Reac41.ShowThrough = true
        Me.Reac41.Size = New System.Drawing.Size(44, 44)
        Me.Reac41.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac41.TabIndex = 364
        Me.Reac41.TabStop = false
        Me.Reac41.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac41.WaitOnLoad = true
        '
        'Reac40
        '
        Me.Reac40.BackColor = System.Drawing.Color.Transparent
        Me.Reac40.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac40.Image = Nothing
        Me.Reac40.ImagePath = ""
        Me.Reac40.IsTransparentImage = false
        Me.Reac40.Location = New System.Drawing.Point(488, 404)
        Me.Reac40.Name = "Reac40"
        Me.Reac40.Rotation = 116
        Me.Reac40.ShowThrough = true
        Me.Reac40.Size = New System.Drawing.Size(44, 44)
        Me.Reac40.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac40.TabIndex = 345
        Me.Reac40.TabStop = false
        Me.Reac40.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac40.WaitOnLoad = true
        '
        'Reac36
        '
        Me.Reac36.BackColor = System.Drawing.Color.Transparent
        Me.Reac36.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac36.Image = Nothing
        Me.Reac36.ImagePath = ""
        Me.Reac36.IsTransparentImage = false
        Me.Reac36.Location = New System.Drawing.Point(506, 355)
        Me.Reac36.Name = "Reac36"
        Me.Reac36.Rotation = 105
        Me.Reac36.ShowThrough = true
        Me.Reac36.Size = New System.Drawing.Size(44, 44)
        Me.Reac36.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac36.TabIndex = 360
        Me.Reac36.TabStop = false
        Me.Reac36.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac36.WaitOnLoad = true
        '
        'Reac35
        '
        Me.Reac35.BackColor = System.Drawing.Color.Transparent
        Me.Reac35.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac35.Image = Nothing
        Me.Reac35.ImagePath = ""
        Me.Reac35.IsTransparentImage = false
        Me.Reac35.Location = New System.Drawing.Point(509, 343)
        Me.Reac35.Name = "Reac35"
        Me.Reac35.Rotation = 103
        Me.Reac35.ShowThrough = true
        Me.Reac35.Size = New System.Drawing.Size(44, 44)
        Me.Reac35.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac35.TabIndex = 359
        Me.Reac35.TabStop = false
        Me.Reac35.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac35.WaitOnLoad = true
        '
        'Reac34
        '
        Me.Reac34.BackColor = System.Drawing.Color.Transparent
        Me.Reac34.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac34.Image = Nothing
        Me.Reac34.ImagePath = ""
        Me.Reac34.IsTransparentImage = false
        Me.Reac34.Location = New System.Drawing.Point(512, 330)
        Me.Reac34.Name = "Reac34"
        Me.Reac34.Rotation = 100
        Me.Reac34.ShowThrough = true
        Me.Reac34.Size = New System.Drawing.Size(44, 44)
        Me.Reac34.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac34.TabIndex = 358
        Me.Reac34.TabStop = false
        Me.Reac34.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac34.WaitOnLoad = true
        '
        'Reac33
        '
        Me.Reac33.BackColor = System.Drawing.Color.Transparent
        Me.Reac33.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac33.Image = Nothing
        Me.Reac33.ImagePath = ""
        Me.Reac33.IsTransparentImage = false
        Me.Reac33.Location = New System.Drawing.Point(513, 316)
        Me.Reac33.Name = "Reac33"
        Me.Reac33.Rotation = 96
        Me.Reac33.ShowThrough = true
        Me.Reac33.Size = New System.Drawing.Size(44, 44)
        Me.Reac33.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac33.TabIndex = 357
        Me.Reac33.TabStop = false
        Me.Reac33.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac33.WaitOnLoad = true
        '
        'Reac32
        '
        Me.Reac32.BackColor = System.Drawing.Color.Transparent
        Me.Reac32.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac32.Image = Nothing
        Me.Reac32.ImagePath = ""
        Me.Reac32.IsTransparentImage = false
        Me.Reac32.Location = New System.Drawing.Point(514, 303)
        Me.Reac32.Name = "Reac32"
        Me.Reac32.Rotation = 92
        Me.Reac32.ShowThrough = true
        Me.Reac32.Size = New System.Drawing.Size(44, 44)
        Me.Reac32.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac32.TabIndex = 356
        Me.Reac32.TabStop = false
        Me.Reac32.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac32.WaitOnLoad = true
        '
        'Reac31
        '
        Me.Reac31.BackColor = System.Drawing.Color.Transparent
        Me.Reac31.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac31.Image = Nothing
        Me.Reac31.ImagePath = ""
        Me.Reac31.IsTransparentImage = false
        Me.Reac31.Location = New System.Drawing.Point(515, 290)
        Me.Reac31.Name = "Reac31"
        Me.Reac31.Rotation = 89
        Me.Reac31.ShowThrough = true
        Me.Reac31.Size = New System.Drawing.Size(44, 44)
        Me.Reac31.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac31.TabIndex = 355
        Me.Reac31.TabStop = false
        Me.Reac31.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac31.WaitOnLoad = true
        '
        'Reac30
        '
        Me.Reac30.BackColor = System.Drawing.Color.Transparent
        Me.Reac30.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac30.Image = Nothing
        Me.Reac30.ImagePath = ""
        Me.Reac30.IsTransparentImage = false
        Me.Reac30.Location = New System.Drawing.Point(514, 277)
        Me.Reac30.Name = "Reac30"
        Me.Reac30.Rotation = 85
        Me.Reac30.ShowThrough = true
        Me.Reac30.Size = New System.Drawing.Size(44, 44)
        Me.Reac30.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac30.TabIndex = 354
        Me.Reac30.TabStop = false
        Me.Reac30.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac30.WaitOnLoad = true
        '
        'Reac25
        '
        Me.Reac25.BackColor = System.Drawing.Color.Transparent
        Me.Reac25.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac25.Image = Nothing
        Me.Reac25.ImagePath = ""
        Me.Reac25.IsTransparentImage = false
        Me.Reac25.Location = New System.Drawing.Point(502, 213)
        Me.Reac25.Name = "Reac25"
        Me.Reac25.Rotation = 72
        Me.Reac25.ShowThrough = true
        Me.Reac25.Size = New System.Drawing.Size(44, 44)
        Me.Reac25.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac25.TabIndex = 349
        Me.Reac25.TabStop = false
        Me.Reac25.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac25.WaitOnLoad = true
        '
        'Reac24
        '
        Me.Reac24.BackColor = System.Drawing.Color.Transparent
        Me.Reac24.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac24.Image = Nothing
        Me.Reac24.ImagePath = ""
        Me.Reac24.IsTransparentImage = false
        Me.Reac24.Location = New System.Drawing.Point(497, 201)
        Me.Reac24.Name = "Reac24"
        Me.Reac24.Rotation = 69
        Me.Reac24.ShowThrough = true
        Me.Reac24.Size = New System.Drawing.Size(44, 44)
        Me.Reac24.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac24.TabIndex = 348
        Me.Reac24.TabStop = false
        Me.Reac24.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac24.WaitOnLoad = true
        '
        'Reac23
        '
        Me.Reac23.BackColor = System.Drawing.Color.Transparent
        Me.Reac23.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac23.Image = Nothing
        Me.Reac23.ImagePath = ""
        Me.Reac23.IsTransparentImage = false
        Me.Reac23.Location = New System.Drawing.Point(493, 189)
        Me.Reac23.Name = "Reac23"
        Me.Reac23.Rotation = 66
        Me.Reac23.ShowThrough = true
        Me.Reac23.Size = New System.Drawing.Size(44, 44)
        Me.Reac23.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac23.TabIndex = 347
        Me.Reac23.TabStop = false
        Me.Reac23.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac23.WaitOnLoad = true
        '
        'Reac22
        '
        Me.Reac22.BackColor = System.Drawing.Color.Transparent
        Me.Reac22.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac22.Image = Nothing
        Me.Reac22.ImagePath = ""
        Me.Reac22.IsTransparentImage = false
        Me.Reac22.Location = New System.Drawing.Point(488, 177)
        Me.Reac22.Name = "Reac22"
        Me.Reac22.Rotation = 62
        Me.Reac22.ShowThrough = true
        Me.Reac22.Size = New System.Drawing.Size(44, 44)
        Me.Reac22.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac22.TabIndex = 344
        Me.Reac22.TabStop = false
        Me.Reac22.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac22.WaitOnLoad = true
        '
        'Reac21
        '
        Me.Reac21.BackColor = System.Drawing.Color.Transparent
        Me.Reac21.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac21.Image = Nothing
        Me.Reac21.ImagePath = ""
        Me.Reac21.InitialImage = Nothing
        Me.Reac21.IsTransparentImage = false
        Me.Reac21.Location = New System.Drawing.Point(481, 166)
        Me.Reac21.Name = "Reac21"
        Me.Reac21.Rotation = 60
        Me.Reac21.ShowThrough = true
        Me.Reac21.Size = New System.Drawing.Size(44, 44)
        Me.Reac21.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac21.TabIndex = 343
        Me.Reac21.TabStop = false
        Me.Reac21.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac21.WaitOnLoad = true
        '
        'Reac20
        '
        Me.Reac20.BackColor = System.Drawing.Color.Transparent
        Me.Reac20.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac20.Image = Nothing
        Me.Reac20.ImagePath = ""
        Me.Reac20.IsTransparentImage = false
        Me.Reac20.Location = New System.Drawing.Point(474, 155)
        Me.Reac20.Name = "Reac20"
        Me.Reac20.Rotation = 56
        Me.Reac20.ShowThrough = true
        Me.Reac20.Size = New System.Drawing.Size(44, 44)
        Me.Reac20.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac20.TabIndex = 342
        Me.Reac20.TabStop = false
        Me.Reac20.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac20.WaitOnLoad = true
        '
        'Reac19
        '
        Me.Reac19.BackColor = System.Drawing.Color.Transparent
        Me.Reac19.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac19.Image = Nothing
        Me.Reac19.ImagePath = ""
        Me.Reac19.IsTransparentImage = false
        Me.Reac19.Location = New System.Drawing.Point(467, 144)
        Me.Reac19.Name = "Reac19"
        Me.Reac19.Rotation = 53
        Me.Reac19.ShowThrough = true
        Me.Reac19.Size = New System.Drawing.Size(44, 44)
        Me.Reac19.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac19.TabIndex = 338
        Me.Reac19.TabStop = false
        Me.Reac19.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac19.WaitOnLoad = true
        '
        'Reac17
        '
        Me.Reac17.BackColor = System.Drawing.Color.Transparent
        Me.Reac17.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac17.Image = Nothing
        Me.Reac17.ImagePath = ""
        Me.Reac17.IsTransparentImage = false
        Me.Reac17.Location = New System.Drawing.Point(450, 124)
        Me.Reac17.Name = "Reac17"
        Me.Reac17.Rotation = 47
        Me.Reac17.ShowThrough = true
        Me.Reac17.Size = New System.Drawing.Size(44, 44)
        Me.Reac17.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac17.TabIndex = 340
        Me.Reac17.TabStop = false
        Me.Reac17.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac17.WaitOnLoad = true
        '
        'PanelControl14
        '
        Me.PanelControl14.Appearance.BackColor = System.Drawing.Color.White
        Me.PanelControl14.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl14.Appearance.Options.UseBackColor = true
        Me.PanelControl14.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl14.Controls.Add(Me.PanelControl15)
        Me.PanelControl14.Location = New System.Drawing.Point(575, -4)
        Me.PanelControl14.Name = "PanelControl14"
        Me.PanelControl14.Size = New System.Drawing.Size(190, 627)
        Me.PanelControl14.TabIndex = 319
        '
        'PanelControl15
        '
        Me.PanelControl15.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.PanelControl15.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl15.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.PanelControl15.Appearance.Options.UseBackColor = true
        Me.PanelControl15.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.PanelControl15.Controls.Add(Me.LegendReactionsGroupBox)
        Me.PanelControl15.Controls.Add(Me.BsGroupBox13)
        Me.PanelControl15.Location = New System.Drawing.Point(3, 4)
        Me.PanelControl15.Name = "PanelControl15"
        Me.PanelControl15.Size = New System.Drawing.Size(183, 620)
        Me.PanelControl15.TabIndex = 262
        '
        'LegendReactionsGroupBox
        '
        Me.LegendReactionsGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsOpticalLabel)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsContaminatedLabel)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsOpticalPictureBox)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsContaminatedPictureBox)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsFinishLabel)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsR1SampleLabel)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsR1SampleR2)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsReactionsLegendLabel)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsDilutionLabel)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsR1SamplePictureBox)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsFinishPictureBox)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsDilutionPictureBox)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsR1PictureBox)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsR1SampleR2PictureBox)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsWashingPictureBox)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsNotInUsePictureBox)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsR1Label)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsWashingLabel)
        Me.LegendReactionsGroupBox.Controls.Add(Me.bsNotInUseLabel)
        Me.LegendReactionsGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.LegendReactionsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.LegendReactionsGroupBox.Location = New System.Drawing.Point(4, 333)
        Me.LegendReactionsGroupBox.Name = "LegendReactionsGroupBox"
        Me.LegendReactionsGroupBox.Size = New System.Drawing.Size(175, 282)
        Me.LegendReactionsGroupBox.TabIndex = 36
        Me.LegendReactionsGroupBox.TabStop = false
        Me.LegendReactionsGroupBox.Visible = false
        '
        'bsOpticalLabel
        '
        Me.bsOpticalLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsOpticalLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsOpticalLabel.ForeColor = System.Drawing.Color.Black
        Me.bsOpticalLabel.Location = New System.Drawing.Point(25, 243)
        Me.bsOpticalLabel.Name = "bsOpticalLabel"
        Me.bsOpticalLabel.Size = New System.Drawing.Size(147, 13)
        Me.bsOpticalLabel.TabIndex = 23
        Me.bsOpticalLabel.Text = "Optical Rejection"
        Me.bsOpticalLabel.Title = false
        '
        'bsContaminatedLabel
        '
        Me.bsContaminatedLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsContaminatedLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsContaminatedLabel.ForeColor = System.Drawing.Color.Black
        Me.bsContaminatedLabel.Location = New System.Drawing.Point(25, 217)
        Me.bsContaminatedLabel.Name = "bsContaminatedLabel"
        Me.bsContaminatedLabel.Size = New System.Drawing.Size(147, 13)
        Me.bsContaminatedLabel.TabIndex = 22
        Me.bsContaminatedLabel.Text = "Contaminated"
        Me.bsContaminatedLabel.Title = false
        '
        'bsOpticalPictureBox
        '
        Me.bsOpticalPictureBox.BackColor = System.Drawing.Color.Transparent
        Me.bsOpticalPictureBox.InitialImage = CType(resources.GetObject("bsOpticalPictureBox.InitialImage"),System.Drawing.Image)
        Me.bsOpticalPictureBox.Location = New System.Drawing.Point(4, 240)
        Me.bsOpticalPictureBox.Name = "bsOpticalPictureBox"
        Me.bsOpticalPictureBox.PositionNumber = 0
        Me.bsOpticalPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsOpticalPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsOpticalPictureBox.TabIndex = 21
        Me.bsOpticalPictureBox.TabStop = false
        '
        'bsContaminatedPictureBox
        '
        Me.bsContaminatedPictureBox.BackColor = System.Drawing.Color.Transparent
        Me.bsContaminatedPictureBox.InitialImage = CType(resources.GetObject("bsContaminatedPictureBox.InitialImage"),System.Drawing.Image)
        Me.bsContaminatedPictureBox.Location = New System.Drawing.Point(4, 215)
        Me.bsContaminatedPictureBox.Name = "bsContaminatedPictureBox"
        Me.bsContaminatedPictureBox.PositionNumber = 0
        Me.bsContaminatedPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsContaminatedPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsContaminatedPictureBox.TabIndex = 20
        Me.bsContaminatedPictureBox.TabStop = false
        '
        'bsFinishLabel
        '
        Me.bsFinishLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsFinishLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFinishLabel.ForeColor = System.Drawing.Color.Black
        Me.bsFinishLabel.Location = New System.Drawing.Point(25, 192)
        Me.bsFinishLabel.Name = "bsFinishLabel"
        Me.bsFinishLabel.Size = New System.Drawing.Size(147, 13)
        Me.bsFinishLabel.TabIndex = 17
        Me.bsFinishLabel.Text = "Finished"
        Me.bsFinishLabel.Title = false
        '
        'bsR1SampleLabel
        '
        Me.bsR1SampleLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsR1SampleLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsR1SampleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsR1SampleLabel.Location = New System.Drawing.Point(25, 117)
        Me.bsR1SampleLabel.Name = "bsR1SampleLabel"
        Me.bsR1SampleLabel.Size = New System.Drawing.Size(147, 13)
        Me.bsR1SampleLabel.TabIndex = 19
        Me.bsR1SampleLabel.Text = "R1 + Sample "
        Me.bsR1SampleLabel.Title = false
        '
        'bsR1SampleR2
        '
        Me.bsR1SampleR2.BackColor = System.Drawing.Color.Transparent
        Me.bsR1SampleR2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsR1SampleR2.ForeColor = System.Drawing.Color.Black
        Me.bsR1SampleR2.Location = New System.Drawing.Point(25, 142)
        Me.bsR1SampleR2.Name = "bsR1SampleR2"
        Me.bsR1SampleR2.Size = New System.Drawing.Size(147, 13)
        Me.bsR1SampleR2.TabIndex = 16
        Me.bsR1SampleR2.Text = "R1 + Sample + R2 "
        Me.bsR1SampleR2.Title = false
        '
        'bsReactionsLegendLabel
        '
        Me.bsReactionsLegendLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsReactionsLegendLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsReactionsLegendLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReactionsLegendLabel.Location = New System.Drawing.Point(4, 12)
        Me.bsReactionsLegendLabel.Name = "bsReactionsLegendLabel"
        Me.bsReactionsLegendLabel.Size = New System.Drawing.Size(167, 19)
        Me.bsReactionsLegendLabel.TabIndex = 10
        Me.bsReactionsLegendLabel.Text = "Legend"
        Me.bsReactionsLegendLabel.Title = true
        '
        'bsDilutionLabel
        '
        Me.bsDilutionLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDilutionLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDilutionLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDilutionLabel.Location = New System.Drawing.Point(25, 166)
        Me.bsDilutionLabel.Name = "bsDilutionLabel"
        Me.bsDilutionLabel.Size = New System.Drawing.Size(147, 13)
        Me.bsDilutionLabel.TabIndex = 15
        Me.bsDilutionLabel.Text = "Sample Dilution "
        Me.bsDilutionLabel.Title = false
        '
        'bsR1SamplePictureBox
        '
        Me.bsR1SamplePictureBox.BackColor = System.Drawing.Color.Transparent
        Me.bsR1SamplePictureBox.InitialImage = CType(resources.GetObject("bsR1SamplePictureBox.InitialImage"),System.Drawing.Image)
        Me.bsR1SamplePictureBox.Location = New System.Drawing.Point(4, 114)
        Me.bsR1SamplePictureBox.Name = "bsR1SamplePictureBox"
        Me.bsR1SamplePictureBox.PositionNumber = 0
        Me.bsR1SamplePictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsR1SamplePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsR1SamplePictureBox.TabIndex = 18
        Me.bsR1SamplePictureBox.TabStop = false
        '
        'bsFinishPictureBox
        '
        Me.bsFinishPictureBox.BackColor = System.Drawing.Color.Transparent
        Me.bsFinishPictureBox.InitialImage = CType(resources.GetObject("bsFinishPictureBox.InitialImage"),System.Drawing.Image)
        Me.bsFinishPictureBox.Location = New System.Drawing.Point(4, 189)
        Me.bsFinishPictureBox.Name = "bsFinishPictureBox"
        Me.bsFinishPictureBox.PositionNumber = 0
        Me.bsFinishPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsFinishPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsFinishPictureBox.TabIndex = 11
        Me.bsFinishPictureBox.TabStop = false
        '
        'bsDilutionPictureBox
        '
        Me.bsDilutionPictureBox.BackColor = System.Drawing.Color.Transparent
        Me.bsDilutionPictureBox.InitialImage = CType(resources.GetObject("bsDilutionPictureBox.InitialImage"),System.Drawing.Image)
        Me.bsDilutionPictureBox.Location = New System.Drawing.Point(4, 164)
        Me.bsDilutionPictureBox.Name = "bsDilutionPictureBox"
        Me.bsDilutionPictureBox.PositionNumber = 0
        Me.bsDilutionPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsDilutionPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsDilutionPictureBox.TabIndex = 9
        Me.bsDilutionPictureBox.TabStop = false
        '
        'bsR1PictureBox
        '
        Me.bsR1PictureBox.BackColor = System.Drawing.Color.Transparent
        Me.bsR1PictureBox.InitialImage = CType(resources.GetObject("bsR1PictureBox.InitialImage"),System.Drawing.Image)
        Me.bsR1PictureBox.Location = New System.Drawing.Point(4, 89)
        Me.bsR1PictureBox.Name = "bsR1PictureBox"
        Me.bsR1PictureBox.PositionNumber = 0
        Me.bsR1PictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsR1PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsR1PictureBox.TabIndex = 5
        Me.bsR1PictureBox.TabStop = false
        '
        'bsR1SampleR2PictureBox
        '
        Me.bsR1SampleR2PictureBox.BackColor = System.Drawing.Color.Transparent
        Me.bsR1SampleR2PictureBox.InitialImage = CType(resources.GetObject("bsR1SampleR2PictureBox.InitialImage"),System.Drawing.Image)
        Me.bsR1SampleR2PictureBox.Location = New System.Drawing.Point(4, 139)
        Me.bsR1SampleR2PictureBox.Name = "bsR1SampleR2PictureBox"
        Me.bsR1SampleR2PictureBox.PositionNumber = 0
        Me.bsR1SampleR2PictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsR1SampleR2PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsR1SampleR2PictureBox.TabIndex = 8
        Me.bsR1SampleR2PictureBox.TabStop = false
        '
        'bsWashingPictureBox
        '
        Me.bsWashingPictureBox.BackColor = System.Drawing.Color.Transparent
        Me.bsWashingPictureBox.InitialImage = CType(resources.GetObject("bsWashingPictureBox.InitialImage"),System.Drawing.Image)
        Me.bsWashingPictureBox.Location = New System.Drawing.Point(4, 39)
        Me.bsWashingPictureBox.Name = "bsWashingPictureBox"
        Me.bsWashingPictureBox.PositionNumber = 0
        Me.bsWashingPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsWashingPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsWashingPictureBox.TabIndex = 6
        Me.bsWashingPictureBox.TabStop = false
        '
        'bsNotInUsePictureBox
        '
        Me.bsNotInUsePictureBox.BackColor = System.Drawing.Color.Transparent
        Me.bsNotInUsePictureBox.InitialImage = CType(resources.GetObject("bsNotInUsePictureBox.InitialImage"),System.Drawing.Image)
        Me.bsNotInUsePictureBox.Location = New System.Drawing.Point(4, 64)
        Me.bsNotInUsePictureBox.Name = "bsNotInUsePictureBox"
        Me.bsNotInUsePictureBox.PositionNumber = 0
        Me.bsNotInUsePictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsNotInUsePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsNotInUsePictureBox.TabIndex = 7
        Me.bsNotInUsePictureBox.TabStop = false
        '
        'bsR1Label
        '
        Me.bsR1Label.BackColor = System.Drawing.Color.Transparent
        Me.bsR1Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsR1Label.ForeColor = System.Drawing.Color.Black
        Me.bsR1Label.Location = New System.Drawing.Point(25, 92)
        Me.bsR1Label.Name = "bsR1Label"
        Me.bsR1Label.Size = New System.Drawing.Size(147, 13)
        Me.bsR1Label.TabIndex = 14
        Me.bsR1Label.Text = "R1"
        Me.bsR1Label.Title = false
        '
        'bsWashingLabel
        '
        Me.bsWashingLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsWashingLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsWashingLabel.ForeColor = System.Drawing.Color.Black
        Me.bsWashingLabel.Location = New System.Drawing.Point(25, 42)
        Me.bsWashingLabel.Name = "bsWashingLabel"
        Me.bsWashingLabel.Size = New System.Drawing.Size(147, 13)
        Me.bsWashingLabel.TabIndex = 12
        Me.bsWashingLabel.Text = "Washing"
        Me.bsWashingLabel.Title = false
        '
        'bsNotInUseLabel
        '
        Me.bsNotInUseLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNotInUseLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNotInUseLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNotInUseLabel.Location = New System.Drawing.Point(25, 67)
        Me.bsNotInUseLabel.Name = "bsNotInUseLabel"
        Me.bsNotInUseLabel.Size = New System.Drawing.Size(147, 13)
        Me.bsNotInUseLabel.TabIndex = 13
        Me.bsNotInUseLabel.Text = "Not In Use"
        Me.bsNotInUseLabel.Title = false
        '
        'BsGroupBox13
        '
        Me.BsGroupBox13.BackColor = System.Drawing.Color.Transparent
        Me.BsGroupBox13.Controls.Add(Me.bsReactionsMoveLastPositionButton)
        Me.BsGroupBox13.Controls.Add(Me.bsReactionsDecreaseButton)
        Me.BsGroupBox13.Controls.Add(Me.bsReactionsIncreaseButton)
        Me.BsGroupBox13.Controls.Add(Me.bsReactionsMoveFirstPositionButton)
        Me.BsGroupBox13.Controls.Add(Me.bsOrderTestIDTextBox)
        Me.BsGroupBox13.Controls.Add(Me.BsExecutionIDTextBox)
        Me.BsGroupBox13.Controls.Add(Me.bsReactionsOpenGraph)
        Me.BsGroupBox13.Controls.Add(Me.bsReacTestTextBox)
        Me.BsGroupBox13.Controls.Add(Me.bsReacTestNameLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsRerunTextBox)
        Me.BsGroupBox13.Controls.Add(Me.bsReplicateTextBox)
        Me.BsGroupBox13.Controls.Add(Me.bsRerunLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsReplicateLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsReacStatusTextBox)
        Me.BsGroupBox13.Controls.Add(Me.bsReacStatusLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsReactionsPositionInfoLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsWellNumberLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsSampleClassLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsCalibNumLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsWellNrTextBox)
        Me.BsGroupBox13.Controls.Add(Me.bsSampleClassTextBox)
        Me.BsGroupBox13.Controls.Add(Me.bsCalibNrTextBox)
        Me.BsGroupBox13.Controls.Add(Me.bsReacSampleIDLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsDilutionTextBox)
        Me.BsGroupBox13.Controls.Add(Me.bsReacSampleTypeLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsReacSampleTypeTextBox)
        Me.BsGroupBox13.Controls.Add(Me.bsReacDilutionLabel)
        Me.BsGroupBox13.Controls.Add(Me.bsPatientIDTextBox)
        Me.BsGroupBox13.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.BsGroupBox13.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox13.Location = New System.Drawing.Point(4, -3)
        Me.BsGroupBox13.Name = "BsGroupBox13"
        Me.BsGroupBox13.Size = New System.Drawing.Size(175, 333)
        Me.BsGroupBox13.TabIndex = 4
        Me.BsGroupBox13.TabStop = false
        '
        'bsReactionsMoveLastPositionButton
        '
        Me.bsReactionsMoveLastPositionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReactionsMoveLastPositionButton.Enabled = false
        Me.bsReactionsMoveLastPositionButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsReactionsMoveLastPositionButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReactionsMoveLastPositionButton.Location = New System.Drawing.Point(79, 299)
        Me.bsReactionsMoveLastPositionButton.Name = "bsReactionsMoveLastPositionButton"
        Me.bsReactionsMoveLastPositionButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReactionsMoveLastPositionButton.TabIndex = 48
        Me.bsReactionsMoveLastPositionButton.UseVisualStyleBackColor = true
        '
        'bsReactionsDecreaseButton
        '
        Me.bsReactionsDecreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReactionsDecreaseButton.Enabled = false
        Me.bsReactionsDecreaseButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsReactionsDecreaseButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReactionsDecreaseButton.Location = New System.Drawing.Point(29, 299)
        Me.bsReactionsDecreaseButton.Name = "bsReactionsDecreaseButton"
        Me.bsReactionsDecreaseButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReactionsDecreaseButton.TabIndex = 46
        Me.bsReactionsDecreaseButton.UseVisualStyleBackColor = true
        '
        'bsReactionsIncreaseButton
        '
        Me.bsReactionsIncreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReactionsIncreaseButton.Enabled = false
        Me.bsReactionsIncreaseButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsReactionsIncreaseButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReactionsIncreaseButton.Location = New System.Drawing.Point(54, 299)
        Me.bsReactionsIncreaseButton.Name = "bsReactionsIncreaseButton"
        Me.bsReactionsIncreaseButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReactionsIncreaseButton.TabIndex = 47
        Me.bsReactionsIncreaseButton.UseVisualStyleBackColor = true
        '
        'bsReactionsMoveFirstPositionButton
        '
        Me.bsReactionsMoveFirstPositionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReactionsMoveFirstPositionButton.Enabled = false
        Me.bsReactionsMoveFirstPositionButton.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsReactionsMoveFirstPositionButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReactionsMoveFirstPositionButton.Location = New System.Drawing.Point(4, 299)
        Me.bsReactionsMoveFirstPositionButton.Name = "bsReactionsMoveFirstPositionButton"
        Me.bsReactionsMoveFirstPositionButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReactionsMoveFirstPositionButton.TabIndex = 45
        Me.bsReactionsMoveFirstPositionButton.UseVisualStyleBackColor = true
        '
        'bsOrderTestIDTextBox
        '
        Me.bsOrderTestIDTextBox.BackColor = System.Drawing.Color.White
        Me.bsOrderTestIDTextBox.DecimalsValues = false
        Me.bsOrderTestIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsOrderTestIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsOrderTestIDTextBox.IsNumeric = false
        Me.bsOrderTestIDTextBox.Location = New System.Drawing.Point(5, 290)
        Me.bsOrderTestIDTextBox.Mandatory = false
        Me.bsOrderTestIDTextBox.Name = "bsOrderTestIDTextBox"
        Me.bsOrderTestIDTextBox.ReadOnly = true
        Me.bsOrderTestIDTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsOrderTestIDTextBox.Size = New System.Drawing.Size(59, 21)
        Me.bsOrderTestIDTextBox.TabIndex = 60
        Me.bsOrderTestIDTextBox.TabStop = false
        Me.bsOrderTestIDTextBox.Visible = false
        Me.bsOrderTestIDTextBox.WordWrap = false
        '
        'BsExecutionIDTextBox
        '
        Me.BsExecutionIDTextBox.BackColor = System.Drawing.Color.White
        Me.BsExecutionIDTextBox.DecimalsValues = false
        Me.BsExecutionIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsExecutionIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsExecutionIDTextBox.IsNumeric = false
        Me.BsExecutionIDTextBox.Location = New System.Drawing.Point(5, 274)
        Me.BsExecutionIDTextBox.Mandatory = false
        Me.BsExecutionIDTextBox.Name = "BsExecutionIDTextBox"
        Me.BsExecutionIDTextBox.ReadOnly = true
        Me.BsExecutionIDTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.BsExecutionIDTextBox.Size = New System.Drawing.Size(59, 21)
        Me.BsExecutionIDTextBox.TabIndex = 58
        Me.BsExecutionIDTextBox.TabStop = false
        Me.BsExecutionIDTextBox.Visible = false
        Me.BsExecutionIDTextBox.WordWrap = false
        '
        'bsReactionsOpenGraph
        '
        Me.bsReactionsOpenGraph.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReactionsOpenGraph.Enabled = false
        Me.bsReactionsOpenGraph.Font = New System.Drawing.Font("Verdana", 6!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsReactionsOpenGraph.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReactionsOpenGraph.Location = New System.Drawing.Point(141, 299)
        Me.bsReactionsOpenGraph.Name = "bsReactionsOpenGraph"
        Me.bsReactionsOpenGraph.Size = New System.Drawing.Size(28, 28)
        Me.bsReactionsOpenGraph.TabIndex = 57
        Me.bsReactionsOpenGraph.UseVisualStyleBackColor = true
        '
        'bsReacTestTextBox
        '
        Me.bsReacTestTextBox.BackColor = System.Drawing.Color.White
        Me.bsReacTestTextBox.DecimalsValues = false
        Me.bsReacTestTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReacTestTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReacTestTextBox.IsNumeric = false
        Me.bsReacTestTextBox.Location = New System.Drawing.Point(5, 237)
        Me.bsReacTestTextBox.Mandatory = false
        Me.bsReacTestTextBox.MaxLength = 20
        Me.bsReacTestTextBox.Name = "bsReacTestTextBox"
        Me.bsReacTestTextBox.ReadOnly = true
        Me.bsReacTestTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsReacTestTextBox.TabIndex = 56
        Me.bsReacTestTextBox.TabStop = false
        Me.bsReacTestTextBox.WordWrap = false
        '
        'bsReacTestNameLabel
        '
        Me.bsReacTestNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReacTestNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReacTestNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReacTestNameLabel.Location = New System.Drawing.Point(2, 223)
        Me.bsReacTestNameLabel.Name = "bsReacTestNameLabel"
        Me.bsReacTestNameLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsReacTestNameLabel.TabIndex = 55
        Me.bsReacTestNameLabel.Text = "Test Name"
        Me.bsReacTestNameLabel.Title = false
        '
        'bsRerunTextBox
        '
        Me.bsRerunTextBox.BackColor = System.Drawing.Color.White
        Me.bsRerunTextBox.DecimalsValues = false
        Me.bsRerunTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRerunTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsRerunTextBox.IsNumeric = false
        Me.bsRerunTextBox.Location = New System.Drawing.Point(107, 199)
        Me.bsRerunTextBox.Mandatory = false
        Me.bsRerunTextBox.Name = "bsRerunTextBox"
        Me.bsRerunTextBox.ReadOnly = true
        Me.bsRerunTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsRerunTextBox.Size = New System.Drawing.Size(62, 21)
        Me.bsRerunTextBox.TabIndex = 50
        Me.bsRerunTextBox.TabStop = false
        Me.bsRerunTextBox.WordWrap = false
        '
        'bsReplicateTextBox
        '
        Me.bsReplicateTextBox.BackColor = System.Drawing.Color.White
        Me.bsReplicateTextBox.DecimalsValues = false
        Me.bsReplicateTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReplicateTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReplicateTextBox.IsNumeric = false
        Me.bsReplicateTextBox.Location = New System.Drawing.Point(5, 199)
        Me.bsReplicateTextBox.Mandatory = false
        Me.bsReplicateTextBox.MaxLength = 20
        Me.bsReplicateTextBox.Name = "bsReplicateTextBox"
        Me.bsReplicateTextBox.ReadOnly = true
        Me.bsReplicateTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsReplicateTextBox.Size = New System.Drawing.Size(100, 21)
        Me.bsReplicateTextBox.TabIndex = 49
        Me.bsReplicateTextBox.TabStop = false
        Me.bsReplicateTextBox.WordWrap = false
        '
        'bsRerunLabel
        '
        Me.bsRerunLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRerunLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRerunLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRerunLabel.Location = New System.Drawing.Point(104, 185)
        Me.bsRerunLabel.Name = "bsRerunLabel"
        Me.bsRerunLabel.Size = New System.Drawing.Size(68, 13)
        Me.bsRerunLabel.TabIndex = 52
        Me.bsRerunLabel.Text = "Rerun"
        Me.bsRerunLabel.Title = false
        '
        'bsReplicateLabel
        '
        Me.bsReplicateLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReplicateLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReplicateLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReplicateLabel.Location = New System.Drawing.Point(2, 185)
        Me.bsReplicateLabel.Name = "bsReplicateLabel"
        Me.bsReplicateLabel.Size = New System.Drawing.Size(104, 13)
        Me.bsReplicateLabel.TabIndex = 51
        Me.bsReplicateLabel.Text = "Replicate"
        Me.bsReplicateLabel.Title = false
        '
        'bsReacStatusTextBox
        '
        Me.bsReacStatusTextBox.BackColor = System.Drawing.Color.White
        Me.bsReacStatusTextBox.DecimalsValues = false
        Me.bsReacStatusTextBox.Enabled = false
        Me.bsReacStatusTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReacStatusTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReacStatusTextBox.IsNumeric = false
        Me.bsReacStatusTextBox.Location = New System.Drawing.Point(5, 274)
        Me.bsReacStatusTextBox.Mandatory = false
        Me.bsReacStatusTextBox.Name = "bsReacStatusTextBox"
        Me.bsReacStatusTextBox.ReadOnly = true
        Me.bsReacStatusTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsReacStatusTextBox.TabIndex = 33
        Me.bsReacStatusTextBox.TabStop = false
        '
        'bsReacStatusLabel
        '
        Me.bsReacStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReacStatusLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReacStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReacStatusLabel.Location = New System.Drawing.Point(2, 260)
        Me.bsReacStatusLabel.Name = "bsReacStatusLabel"
        Me.bsReacStatusLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsReacStatusLabel.TabIndex = 34
        Me.bsReacStatusLabel.Text = "Status"
        Me.bsReacStatusLabel.Title = false
        '
        'bsReactionsPositionInfoLabel
        '
        Me.bsReactionsPositionInfoLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsReactionsPositionInfoLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsReactionsPositionInfoLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReactionsPositionInfoLabel.Location = New System.Drawing.Point(4, 12)
        Me.bsReactionsPositionInfoLabel.Name = "bsReactionsPositionInfoLabel"
        Me.bsReactionsPositionInfoLabel.Size = New System.Drawing.Size(167, 19)
        Me.bsReactionsPositionInfoLabel.TabIndex = 25
        Me.bsReactionsPositionInfoLabel.Text = "Position Information"
        Me.bsReactionsPositionInfoLabel.Title = true
        '
        'bsWellNumberLabel
        '
        Me.bsWellNumberLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsWellNumberLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsWellNumberLabel.ForeColor = System.Drawing.Color.Black
        Me.bsWellNumberLabel.Location = New System.Drawing.Point(2, 44)
        Me.bsWellNumberLabel.Name = "bsWellNumberLabel"
        Me.bsWellNumberLabel.Size = New System.Drawing.Size(104, 13)
        Me.bsWellNumberLabel.TabIndex = 28
        Me.bsWellNumberLabel.Text = "Well Number"
        Me.bsWellNumberLabel.Title = false
        '
        'bsSampleClassLabel
        '
        Me.bsSampleClassLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleClassLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleClassLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleClassLabel.Location = New System.Drawing.Point(2, 71)
        Me.bsSampleClassLabel.Name = "bsSampleClassLabel"
        Me.bsSampleClassLabel.Size = New System.Drawing.Size(120, 13)
        Me.bsSampleClassLabel.TabIndex = 30
        Me.bsSampleClassLabel.Text = "Sample Class"
        Me.bsSampleClassLabel.Title = false
        '
        'bsCalibNumLabel
        '
        Me.bsCalibNumLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsCalibNumLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCalibNumLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCalibNumLabel.Location = New System.Drawing.Point(120, 71)
        Me.bsCalibNumLabel.Name = "bsCalibNumLabel"
        Me.bsCalibNumLabel.Size = New System.Drawing.Size(52, 13)
        Me.bsCalibNumLabel.TabIndex = 33
        Me.bsCalibNumLabel.Text = "Num"
        Me.bsCalibNumLabel.Title = false
        '
        'bsWellNrTextBox
        '
        Me.bsWellNrTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.bsWellNrTextBox.BackColor = System.Drawing.Color.White
        Me.bsWellNrTextBox.DecimalsValues = false
        Me.bsWellNrTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsWellNrTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsWellNrTextBox.IsNumeric = false
        Me.bsWellNrTextBox.Location = New System.Drawing.Point(107, 40)
        Me.bsWellNrTextBox.Mandatory = false
        Me.bsWellNrTextBox.Name = "bsWellNrTextBox"
        Me.bsWellNrTextBox.ReadOnly = true
        Me.bsWellNrTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsWellNrTextBox.Size = New System.Drawing.Size(62, 21)
        Me.bsWellNrTextBox.TabIndex = 26
        Me.bsWellNrTextBox.TabStop = false
        Me.bsWellNrTextBox.WordWrap = false
        '
        'bsSampleClassTextBox
        '
        Me.bsSampleClassTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleClassTextBox.DecimalsValues = false
        Me.bsSampleClassTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleClassTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleClassTextBox.IsNumeric = false
        Me.bsSampleClassTextBox.Location = New System.Drawing.Point(5, 85)
        Me.bsSampleClassTextBox.Mandatory = false
        Me.bsSampleClassTextBox.MaxLength = 20
        Me.bsSampleClassTextBox.Name = "bsSampleClassTextBox"
        Me.bsSampleClassTextBox.ReadOnly = true
        Me.bsSampleClassTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsSampleClassTextBox.TabIndex = 31
        Me.bsSampleClassTextBox.TabStop = false
        Me.bsSampleClassTextBox.WordWrap = false
        '
        'bsCalibNrTextBox
        '
        Me.bsCalibNrTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.bsCalibNrTextBox.BackColor = System.Drawing.Color.White
        Me.bsCalibNrTextBox.DecimalsValues = false
        Me.bsCalibNrTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCalibNrTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsCalibNrTextBox.IsNumeric = false
        Me.bsCalibNrTextBox.Location = New System.Drawing.Point(123, 85)
        Me.bsCalibNrTextBox.Mandatory = false
        Me.bsCalibNrTextBox.Name = "bsCalibNrTextBox"
        Me.bsCalibNrTextBox.ReadOnly = true
        Me.bsCalibNrTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsCalibNrTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsCalibNrTextBox.TabIndex = 32
        Me.bsCalibNrTextBox.TabStop = false
        Me.bsCalibNrTextBox.WordWrap = false
        '
        'bsReacSampleIDLabel
        '
        Me.bsReacSampleIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReacSampleIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReacSampleIDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReacSampleIDLabel.Location = New System.Drawing.Point(2, 109)
        Me.bsReacSampleIDLabel.Name = "bsReacSampleIDLabel"
        Me.bsReacSampleIDLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsReacSampleIDLabel.TabIndex = 35
        Me.bsReacSampleIDLabel.Text = "Sample ID"
        Me.bsReacSampleIDLabel.Title = false
        '
        'bsDilutionTextBox
        '
        Me.bsDilutionTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.bsDilutionTextBox.BackColor = System.Drawing.Color.White
        Me.bsDilutionTextBox.DecimalsValues = false
        Me.bsDilutionTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDilutionTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsDilutionTextBox.IsNumeric = false
        Me.bsDilutionTextBox.Location = New System.Drawing.Point(107, 161)
        Me.bsDilutionTextBox.Mandatory = false
        Me.bsDilutionTextBox.Name = "bsDilutionTextBox"
        Me.bsDilutionTextBox.ReadOnly = true
        Me.bsDilutionTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsDilutionTextBox.Size = New System.Drawing.Size(62, 21)
        Me.bsDilutionTextBox.TabIndex = 40
        Me.bsDilutionTextBox.TabStop = false
        Me.bsDilutionTextBox.WordWrap = false
        '
        'bsReacSampleTypeLabel
        '
        Me.bsReacSampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReacSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReacSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReacSampleTypeLabel.Location = New System.Drawing.Point(2, 147)
        Me.bsReacSampleTypeLabel.Name = "bsReacSampleTypeLabel"
        Me.bsReacSampleTypeLabel.Size = New System.Drawing.Size(104, 13)
        Me.bsReacSampleTypeLabel.TabIndex = 37
        Me.bsReacSampleTypeLabel.Text = "Sample Type"
        Me.bsReacSampleTypeLabel.Title = false
        '
        'bsReacSampleTypeTextBox
        '
        Me.bsReacSampleTypeTextBox.BackColor = System.Drawing.Color.White
        Me.bsReacSampleTypeTextBox.DecimalsValues = false
        Me.bsReacSampleTypeTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReacSampleTypeTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReacSampleTypeTextBox.IsNumeric = false
        Me.bsReacSampleTypeTextBox.Location = New System.Drawing.Point(5, 161)
        Me.bsReacSampleTypeTextBox.Mandatory = false
        Me.bsReacSampleTypeTextBox.MaxLength = 20
        Me.bsReacSampleTypeTextBox.Name = "bsReacSampleTypeTextBox"
        Me.bsReacSampleTypeTextBox.ReadOnly = true
        Me.bsReacSampleTypeTextBox.Size = New System.Drawing.Size(100, 21)
        Me.bsReacSampleTypeTextBox.TabIndex = 38
        Me.bsReacSampleTypeTextBox.TabStop = false
        Me.bsReacSampleTypeTextBox.WordWrap = false
        '
        'bsReacDilutionLabel
        '
        Me.bsReacDilutionLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReacDilutionLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReacDilutionLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReacDilutionLabel.Location = New System.Drawing.Point(104, 147)
        Me.bsReacDilutionLabel.Name = "bsReacDilutionLabel"
        Me.bsReacDilutionLabel.Size = New System.Drawing.Size(68, 13)
        Me.bsReacDilutionLabel.TabIndex = 39
        Me.bsReacDilutionLabel.Text = "Dilution"
        Me.bsReacDilutionLabel.Title = false
        '
        'bsPatientIDTextBox
        '
        Me.bsPatientIDTextBox.BackColor = System.Drawing.Color.White
        Me.bsPatientIDTextBox.DecimalsValues = false
        Me.bsPatientIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPatientIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsPatientIDTextBox.IsNumeric = false
        Me.bsPatientIDTextBox.Location = New System.Drawing.Point(5, 123)
        Me.bsPatientIDTextBox.Mandatory = false
        Me.bsPatientIDTextBox.Name = "bsPatientIDTextBox"
        Me.bsPatientIDTextBox.ReadOnly = true
        Me.bsPatientIDTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsPatientIDTextBox.TabIndex = 36
        Me.bsPatientIDTextBox.TabStop = false
        Me.bsPatientIDTextBox.WordWrap = false
        '
        'Reac18
        '
        Me.Reac18.BackColor = System.Drawing.Color.Transparent
        Me.Reac18.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac18.Image = Nothing
        Me.Reac18.ImagePath = ""
        Me.Reac18.IsTransparentImage = false
        Me.Reac18.Location = New System.Drawing.Point(459, 134)
        Me.Reac18.Name = "Reac18"
        Me.Reac18.Rotation = 50
        Me.Reac18.ShowThrough = true
        Me.Reac18.Size = New System.Drawing.Size(44, 44)
        Me.Reac18.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac18.TabIndex = 341
        Me.Reac18.TabStop = false
        Me.Reac18.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac18.WaitOnLoad = true
        '
        'Reac75
        '
        Me.Reac75.BackColor = System.Drawing.Color.Transparent
        Me.Reac75.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac75.Image = Nothing
        Me.Reac75.ImagePath = ""
        Me.Reac75.IsTransparentImage = false
        Me.Reac75.Location = New System.Drawing.Point(100, 475)
        Me.Reac75.Name = "Reac75"
        Me.Reac75.Rotation = 221
        Me.Reac75.ShowThrough = true
        Me.Reac75.Size = New System.Drawing.Size(44, 44)
        Me.Reac75.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac75.TabIndex = 397
        Me.Reac75.TabStop = false
        Me.Reac75.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac75.WaitOnLoad = true
        '
        'Reac87
        '
        Me.Reac87.BackColor = System.Drawing.Color.Transparent
        Me.Reac87.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac87.Image = Nothing
        Me.Reac87.ImagePath = ""
        Me.Reac87.IsTransparentImage = false
        Me.Reac87.Location = New System.Drawing.Point(25, 341)
        Me.Reac87.Name = "Reac87"
        Me.Reac87.Rotation = 256
        Me.Reac87.ShowThrough = true
        Me.Reac87.Size = New System.Drawing.Size(44, 44)
        Me.Reac87.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac87.TabIndex = 408
        Me.Reac87.TabStop = false
        Me.Reac87.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac87.WaitOnLoad = true
        '
        'Reac89
        '
        Me.Reac89.BackColor = System.Drawing.Color.Transparent
        Me.Reac89.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac89.Image = Nothing
        Me.Reac89.ImagePath = ""
        Me.Reac89.IsTransparentImage = false
        Me.Reac89.Location = New System.Drawing.Point(20, 315)
        Me.Reac89.Name = "Reac89"
        Me.Reac89.Rotation = 262
        Me.Reac89.ShowThrough = true
        Me.Reac89.Size = New System.Drawing.Size(44, 44)
        Me.Reac89.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac89.TabIndex = 410
        Me.Reac89.TabStop = false
        Me.Reac89.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac89.WaitOnLoad = true
        '
        'Reac101
        '
        Me.Reac101.BackColor = System.Drawing.Color.Transparent
        Me.Reac101.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reac101.ErrorImage = Nothing
        Me.Reac101.Image = Nothing
        Me.Reac101.ImagePath = ""
        Me.Reac101.IsTransparentImage = false
        Me.Reac101.Location = New System.Drawing.Point(52, 165)
        Me.Reac101.Name = "Reac101"
        Me.Reac101.Rotation = 298
        Me.Reac101.ShowThrough = true
        Me.Reac101.Size = New System.Drawing.Size(44, 44)
        Me.Reac101.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.Reac101.TabIndex = 421
        Me.Reac101.TabStop = false
        Me.Reac101.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(255,Byte),Integer), CType(CType(192,Byte),Integer), CType(CType(255,Byte),Integer))
        Me.Reac101.WaitOnLoad = true
        '
        'ISETab
        '
        Me.ISETab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ISETab.Appearance.PageClient.Options.UseBackColor = true
        Me.ISETab.Controls.Add(Me.BsIseMonitor)
        Me.ISETab.Name = "ISETab"
        Me.ISETab.Size = New System.Drawing.Size(762, 623)
        Me.ISETab.Text = "ISE Module"
        '
        'BsIseMonitor
        '
        Me.BsIseMonitor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsIseMonitor.Location = New System.Drawing.Point(0, 0)
        Me.BsIseMonitor.MaximumSize = New System.Drawing.Size(978, 623)
        Me.BsIseMonitor.MinimumSize = New System.Drawing.Size(762, 593)
        Me.BsIseMonitor.Name = "BsIseMonitor"
        Me.BsIseMonitor.Size = New System.Drawing.Size(762, 623)
        Me.BsIseMonitor.TabIndex = 0
        '
        'AlarmsTab
        '
        Me.AlarmsTab.Controls.Add(Me.BsGroupBox4)
        Me.AlarmsTab.Name = "AlarmsTab"
        Me.AlarmsTab.Size = New System.Drawing.Size(762, 623)
        Me.AlarmsTab.Text = "Alarms"
        '
        'BsGroupBox4
        '
        Me.BsGroupBox4.BackColor = System.Drawing.Color.Transparent
        Me.BsGroupBox4.Controls.Add(Me.AlarmsXtraGrid)
        Me.BsGroupBox4.Controls.Add(Me.bsTitleLabel)
        Me.BsGroupBox4.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox4.Location = New System.Drawing.Point(2, -4)
        Me.BsGroupBox4.Margin = New System.Windows.Forms.Padding(4, 3, 3, 3)
        Me.BsGroupBox4.Name = "BsGroupBox4"
        Me.BsGroupBox4.Size = New System.Drawing.Size(757, 624)
        Me.BsGroupBox4.TabIndex = 52
        Me.BsGroupBox4.TabStop = false
        '
        'AlarmsXtraGrid
        '
        Me.AlarmsXtraGrid.Location = New System.Drawing.Point(5, 34)
        Me.AlarmsXtraGrid.LookAndFeel.UseWindowsXPTheme = true
        Me.AlarmsXtraGrid.MainView = Me.AlarmsXtraGridView
        Me.AlarmsXtraGrid.Name = "AlarmsXtraGrid"
        Me.AlarmsXtraGrid.Size = New System.Drawing.Size(746, 584)
        Me.AlarmsXtraGrid.TabIndex = 14
        Me.AlarmsXtraGrid.ToolTipController = Me.ToolTipController1
        Me.AlarmsXtraGrid.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.AlarmsXtraGridView})
        '
        'AlarmsXtraGridView
        '
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButton.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButton.Options.UseBorderColor = true
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButton.Options.UseForeColor = true
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = true
        Me.AlarmsXtraGridView.Appearance.ColumnFilterButtonActive.Options.UseForeColor = true
        Me.AlarmsXtraGridView.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.AlarmsXtraGridView.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.AlarmsXtraGridView.Appearance.Empty.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.AlarmsXtraGridView.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black
        Me.AlarmsXtraGridView.Appearance.EvenRow.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.EvenRow.Options.UseForeColor = true
        Me.AlarmsXtraGridView.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.AlarmsXtraGridView.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.AlarmsXtraGridView.Appearance.FilterCloseButton.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.FilterCloseButton.Options.UseBorderColor = true
        Me.AlarmsXtraGridView.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.AlarmsXtraGridView.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.AlarmsXtraGridView.Appearance.FilterPanel.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.FilterPanel.Options.UseForeColor = true
        Me.AlarmsXtraGridView.Appearance.FocusedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.AlarmsXtraGridView.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.AlarmsXtraGridView.Appearance.FocusedRow.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.FocusedRow.Options.UseForeColor = true
        Me.AlarmsXtraGridView.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.AlarmsXtraGridView.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.AlarmsXtraGridView.Appearance.FooterPanel.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.FooterPanel.Options.UseBorderColor = true
        Me.AlarmsXtraGridView.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.AlarmsXtraGridView.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.AlarmsXtraGridView.Appearance.GroupButton.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.GroupButton.Options.UseBorderColor = true
        Me.AlarmsXtraGridView.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.AlarmsXtraGridView.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.AlarmsXtraGridView.Appearance.GroupFooter.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.GroupFooter.Options.UseBorderColor = true
        Me.AlarmsXtraGridView.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.AlarmsXtraGridView.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.AlarmsXtraGridView.Appearance.GroupPanel.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.GroupPanel.Options.UseForeColor = true
        Me.AlarmsXtraGridView.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.AlarmsXtraGridView.Appearance.GroupRow.Font = New System.Drawing.Font("Tahoma", 8!, System.Drawing.FontStyle.Bold)
        Me.AlarmsXtraGridView.Appearance.GroupRow.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.GroupRow.Options.UseFont = true
        Me.AlarmsXtraGridView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.AlarmsXtraGridView.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.AlarmsXtraGridView.Appearance.HeaderPanel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.AlarmsXtraGridView.Appearance.HeaderPanel.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.HeaderPanel.Options.UseBorderColor = true
        Me.AlarmsXtraGridView.Appearance.HeaderPanel.Options.UseFont = true
        Me.AlarmsXtraGridView.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.AlarmsXtraGridView.Appearance.HorzLine.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.OddRow.BackColor = System.Drawing.Color.White
        Me.AlarmsXtraGridView.Appearance.OddRow.ForeColor = System.Drawing.Color.Black
        Me.AlarmsXtraGridView.Appearance.OddRow.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.OddRow.Options.UseForeColor = true
        Me.AlarmsXtraGridView.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.AlarmsXtraGridView.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.AlarmsXtraGridView.Appearance.Preview.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.Preview.Options.UseForeColor = true
        Me.AlarmsXtraGridView.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.AlarmsXtraGridView.Appearance.Row.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.AlarmsXtraGridView.Appearance.Row.ForeColor = System.Drawing.Color.Black
        Me.AlarmsXtraGridView.Appearance.Row.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.Row.Options.UseFont = true
        Me.AlarmsXtraGridView.Appearance.Row.Options.UseForeColor = true
        Me.AlarmsXtraGridView.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.AlarmsXtraGridView.Appearance.RowSeparator.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.SelectedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.AlarmsXtraGridView.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White
        Me.AlarmsXtraGridView.Appearance.SelectedRow.Options.UseBackColor = true
        Me.AlarmsXtraGridView.Appearance.SelectedRow.Options.UseForeColor = true
        Me.AlarmsXtraGridView.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.AlarmsXtraGridView.Appearance.VertLine.Options.UseBackColor = true
        Me.AlarmsXtraGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.AlarmsXtraGridView.GridControl = Me.AlarmsXtraGrid
        Me.AlarmsXtraGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.AlarmsXtraGridView.Name = "AlarmsXtraGridView"
        Me.AlarmsXtraGridView.OptionsCustomization.AllowColumnMoving = false
        Me.AlarmsXtraGridView.OptionsCustomization.AllowFilter = false
        Me.AlarmsXtraGridView.OptionsCustomization.AllowQuickHideColumns = false
        Me.AlarmsXtraGridView.OptionsFind.AllowFindPanel = false
        Me.AlarmsXtraGridView.OptionsSelection.EnableAppearanceFocusedCell = false
        Me.AlarmsXtraGridView.OptionsView.ShowGroupPanel = false
        Me.AlarmsXtraGridView.PaintStyleName = "WindowsXP"
        '
        'bsTitleLabel
        '
        Me.bsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitleLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTitleLabel.Location = New System.Drawing.Point(5, 12)
        Me.bsTitleLabel.Name = "bsTitleLabel"
        Me.bsTitleLabel.Size = New System.Drawing.Size(746, 19)
        Me.bsTitleLabel.TabIndex = 13
        Me.bsTitleLabel.Text = "Alarms List"
        Me.bsTitleLabel.Title = true
        '
        'ElapsedTimeLabel
        '
        Me.ElapsedTimeLabel.BackColor = System.Drawing.Color.Transparent
        Me.ElapsedTimeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.ElapsedTimeLabel.ForeColor = System.Drawing.Color.Black
        Me.ElapsedTimeLabel.Location = New System.Drawing.Point(7, 116)
        Me.ElapsedTimeLabel.Name = "ElapsedTimeLabel"
        Me.ElapsedTimeLabel.Size = New System.Drawing.Size(120, 13)
        Me.ElapsedTimeLabel.TabIndex = 18
        Me.ElapsedTimeLabel.Text = "Elapsed Time"
        Me.ElapsedTimeLabel.Title = false
        '
        'TaskListProgressBar
        '
        Me.TaskListProgressBar.Location = New System.Drawing.Point(9, 69)
        Me.TaskListProgressBar.Name = "TaskListProgressBar"
        Me.TaskListProgressBar.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.TaskListProgressBar.Properties.DisplayFormat.FormatString = "Task List Progress: {0}%"
        Me.TaskListProgressBar.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
        Me.TaskListProgressBar.Properties.LookAndFeel.SkinName = "Money Twins"
        Me.TaskListProgressBar.Properties.LookAndFeel.UseDefaultLookAndFeel = false
        Me.TaskListProgressBar.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid
        Me.TaskListProgressBar.Size = New System.Drawing.Size(179, 20)
        Me.TaskListProgressBar.TabIndex = 17
        '
        'PanelControl2
        '
        Me.PanelControl2.Appearance.BackColor = System.Drawing.Color.White
        Me.PanelControl2.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl2.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.PanelControl2.Appearance.Options.UseBackColor = true
        Me.PanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.PanelControl2.Controls.Add(Me.bsWamUpGroupBox)
        Me.PanelControl2.Controls.Add(Me.BsGroupBox14)
        Me.PanelControl2.Controls.Add(Me.BsGroupBox3)
        Me.PanelControl2.Controls.Add(Me.BsGroupBox10)
        Me.PanelControl2.Controls.Add(Me.bsSamplesLegendGroupBox)
        Me.PanelControl2.Location = New System.Drawing.Point(2, 2)
        Me.PanelControl2.Name = "PanelControl2"
        Me.PanelControl2.Size = New System.Drawing.Size(203, 650)
        Me.PanelControl2.TabIndex = 19
        '
        'bsWamUpGroupBox
        '
        Me.bsWamUpGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.bsWamUpGroupBox.Controls.Add(Me.bsEndWarmUp)
        Me.bsWamUpGroupBox.Controls.Add(Me.TimeWarmUpProgressBar)
        Me.bsWamUpGroupBox.Controls.Add(Me.bsTimeWUpLabel)
        Me.bsWamUpGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsWamUpGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsWamUpGroupBox.Location = New System.Drawing.Point(3, 488)
        Me.bsWamUpGroupBox.Name = "bsWamUpGroupBox"
        Me.bsWamUpGroupBox.Size = New System.Drawing.Size(197, 96)
        Me.bsWamUpGroupBox.TabIndex = 42
        Me.bsWamUpGroupBox.TabStop = false
        '
        'bsEndWarmUp
        '
        Me.bsEndWarmUp.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.bsEndWarmUp.AutoSize = true
        Me.bsEndWarmUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsEndWarmUp.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsEndWarmUp.ForeColor = System.Drawing.Color.SteelBlue
        Me.bsEndWarmUp.Location = New System.Drawing.Point(91, 66)
        Me.bsEndWarmUp.Name = "bsEndWarmUp"
        Me.bsEndWarmUp.Size = New System.Drawing.Size(99, 24)
        Me.bsEndWarmUp.TabIndex = 36
        Me.bsEndWarmUp.Text = "Terminate"
        Me.bsEndWarmUp.UseVisualStyleBackColor = true
        Me.bsEndWarmUp.Visible = false
        '
        'TimeWarmUpProgressBar
        '
        Me.TimeWarmUpProgressBar.Location = New System.Drawing.Point(9, 40)
        Me.TimeWarmUpProgressBar.Name = "TimeWarmUpProgressBar"
        Me.TimeWarmUpProgressBar.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.TimeWarmUpProgressBar.Properties.LookAndFeel.SkinName = "Money Twins"
        Me.TimeWarmUpProgressBar.Properties.LookAndFeel.UseDefaultLookAndFeel = false
        Me.TimeWarmUpProgressBar.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid
        Me.TimeWarmUpProgressBar.Properties.Step = 1
        Me.TimeWarmUpProgressBar.Size = New System.Drawing.Size(179, 20)
        Me.TimeWarmUpProgressBar.TabIndex = 26
        '
        'bsTimeWUpLabel
        '
        Me.bsTimeWUpLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTimeWUpLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsTimeWUpLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTimeWUpLabel.Location = New System.Drawing.Point(5, 12)
        Me.bsTimeWUpLabel.Name = "bsTimeWUpLabel"
        Me.bsTimeWUpLabel.Size = New System.Drawing.Size(187, 19)
        Me.bsTimeWUpLabel.TabIndex = 25
        Me.bsTimeWUpLabel.Text = "Time Warm Up"
        Me.bsTimeWUpLabel.Title = true
        '
        'BsGroupBox14
        '
        Me.BsGroupBox14.BackColor = System.Drawing.Color.Transparent
        Me.BsGroupBox14.Controls.Add(Me.bsTimeAvailableLabel)
        Me.BsGroupBox14.Controls.Add(Me.AccessRRTimeTextEdit)
        Me.BsGroupBox14.Controls.Add(Me.AccessRMTimeTextEdit)
        Me.BsGroupBox14.Controls.Add(Me.AccessRMTimeLabel)
        Me.BsGroupBox14.Controls.Add(Me.AccessRRTimeLabel)
        Me.BsGroupBox14.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.BsGroupBox14.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox14.Location = New System.Drawing.Point(3, 391)
        Me.BsGroupBox14.Name = "BsGroupBox14"
        Me.BsGroupBox14.Size = New System.Drawing.Size(197, 96)
        Me.BsGroupBox14.TabIndex = 35
        Me.BsGroupBox14.TabStop = false
        '
        'bsTimeAvailableLabel
        '
        Me.bsTimeAvailableLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTimeAvailableLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsTimeAvailableLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTimeAvailableLabel.Location = New System.Drawing.Point(5, 12)
        Me.bsTimeAvailableLabel.Name = "bsTimeAvailableLabel"
        Me.bsTimeAvailableLabel.Size = New System.Drawing.Size(187, 19)
        Me.bsTimeAvailableLabel.TabIndex = 25
        Me.bsTimeAvailableLabel.Text = "Time available rotors"
        Me.bsTimeAvailableLabel.Title = true
        '
        'AccessRRTimeTextEdit
        '
        Me.AccessRRTimeTextEdit.CausesValidation = false
        Me.AccessRRTimeTextEdit.EditValue = ""
        Me.AccessRRTimeTextEdit.Location = New System.Drawing.Point(128, 68)
        Me.AccessRRTimeTextEdit.Name = "AccessRRTimeTextEdit"
        Me.AccessRRTimeTextEdit.Properties.AllowFocused = false
        Me.AccessRRTimeTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.AccessRRTimeTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
        Me.AccessRRTimeTextEdit.Properties.Appearance.Options.UseFont = true
        Me.AccessRRTimeTextEdit.Properties.Appearance.Options.UseForeColor = true
        Me.AccessRRTimeTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
        Me.AccessRRTimeTextEdit.Properties.ReadOnly = true
        Me.AccessRRTimeTextEdit.ShowToolTips = false
        Me.AccessRRTimeTextEdit.Size = New System.Drawing.Size(60, 22)
        Me.AccessRRTimeTextEdit.TabIndex = 40
        '
        'AccessRMTimeTextEdit
        '
        Me.AccessRMTimeTextEdit.CausesValidation = false
        Me.AccessRMTimeTextEdit.EditValue = ""
        Me.AccessRMTimeTextEdit.Location = New System.Drawing.Point(128, 39)
        Me.AccessRMTimeTextEdit.Name = "AccessRMTimeTextEdit"
        Me.AccessRMTimeTextEdit.Properties.AllowFocused = false
        Me.AccessRMTimeTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.AccessRMTimeTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
        Me.AccessRMTimeTextEdit.Properties.Appearance.Options.UseFont = true
        Me.AccessRMTimeTextEdit.Properties.Appearance.Options.UseForeColor = true
        Me.AccessRMTimeTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
        Me.AccessRMTimeTextEdit.Properties.ReadOnly = true
        Me.AccessRMTimeTextEdit.ShowToolTips = false
        Me.AccessRMTimeTextEdit.Size = New System.Drawing.Size(60, 22)
        Me.AccessRMTimeTextEdit.TabIndex = 39
        '
        'AccessRMTimeLabel
        '
        Me.AccessRMTimeLabel.BackColor = System.Drawing.Color.Transparent
        Me.AccessRMTimeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.AccessRMTimeLabel.ForeColor = System.Drawing.Color.Black
        Me.AccessRMTimeLabel.Location = New System.Drawing.Point(7, 43)
        Me.AccessRMTimeLabel.Name = "AccessRMTimeLabel"
        Me.AccessRMTimeLabel.Size = New System.Drawing.Size(120, 13)
        Me.AccessRMTimeLabel.TabIndex = 28
        Me.AccessRMTimeLabel.Text = "Sample Rotor"
        Me.AccessRMTimeLabel.Title = false
        '
        'AccessRRTimeLabel
        '
        Me.AccessRRTimeLabel.BackColor = System.Drawing.Color.Transparent
        Me.AccessRRTimeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.AccessRRTimeLabel.ForeColor = System.Drawing.Color.Black
        Me.AccessRRTimeLabel.Location = New System.Drawing.Point(7, 72)
        Me.AccessRRTimeLabel.Name = "AccessRRTimeLabel"
        Me.AccessRRTimeLabel.Size = New System.Drawing.Size(120, 13)
        Me.AccessRRTimeLabel.TabIndex = 29
        Me.AccessRRTimeLabel.Text = "Reagent Rotor"
        Me.AccessRRTimeLabel.Title = false
        '
        'BsGroupBox3
        '
        Me.BsGroupBox3.BackColor = System.Drawing.Color.Transparent
        Me.BsGroupBox3.Controls.Add(Me.PanelControl11)
        Me.BsGroupBox3.Controls.Add(Me.RemainingTimeTextEdit)
        Me.BsGroupBox3.Controls.Add(Me.ElapsedTimeTextEdit)
        Me.BsGroupBox3.Controls.Add(Me.OverallTimeTextEdit)
        Me.BsGroupBox3.Controls.Add(Me.OverallTimeTitleLabel)
        Me.BsGroupBox3.Controls.Add(Me.TaskListProgressBar)
        Me.BsGroupBox3.Controls.Add(Me.RemainingTimeLabel)
        Me.BsGroupBox3.Controls.Add(Me.ElapsedTimeLabel)
        Me.BsGroupBox3.Controls.Add(Me.bsTimeLabel)
        Me.BsGroupBox3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.BsGroupBox3.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox3.Location = New System.Drawing.Point(3, 217)
        Me.BsGroupBox3.Name = "BsGroupBox3"
        Me.BsGroupBox3.Size = New System.Drawing.Size(197, 173)
        Me.BsGroupBox3.TabIndex = 34
        Me.BsGroupBox3.TabStop = false
        '
        'PanelControl11
        '
        Me.PanelControl11.Location = New System.Drawing.Point(4, 100)
        Me.PanelControl11.Name = "PanelControl11"
        Me.PanelControl11.Size = New System.Drawing.Size(191, 2)
        Me.PanelControl11.TabIndex = 41
        '
        'RemainingTimeTextEdit
        '
        Me.RemainingTimeTextEdit.CausesValidation = false
        Me.RemainingTimeTextEdit.EditValue = ""
        Me.RemainingTimeTextEdit.Location = New System.Drawing.Point(128, 141)
        Me.RemainingTimeTextEdit.Name = "RemainingTimeTextEdit"
        Me.RemainingTimeTextEdit.Properties.AllowFocused = false
        Me.RemainingTimeTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.White
        Me.RemainingTimeTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.RemainingTimeTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
        Me.RemainingTimeTextEdit.Properties.Appearance.Options.UseBackColor = true
        Me.RemainingTimeTextEdit.Properties.Appearance.Options.UseFont = true
        Me.RemainingTimeTextEdit.Properties.Appearance.Options.UseForeColor = true
        Me.RemainingTimeTextEdit.Properties.Appearance.Options.UseTextOptions = true
        Me.RemainingTimeTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.RemainingTimeTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
        Me.RemainingTimeTextEdit.Properties.ReadOnly = true
        Me.RemainingTimeTextEdit.ShowToolTips = false
        Me.RemainingTimeTextEdit.Size = New System.Drawing.Size(60, 22)
        Me.RemainingTimeTextEdit.TabIndex = 38
        '
        'ElapsedTimeTextEdit
        '
        Me.ElapsedTimeTextEdit.CausesValidation = false
        Me.ElapsedTimeTextEdit.EditValue = ""
        Me.ElapsedTimeTextEdit.Location = New System.Drawing.Point(128, 112)
        Me.ElapsedTimeTextEdit.Name = "ElapsedTimeTextEdit"
        Me.ElapsedTimeTextEdit.Properties.AllowFocused = false
        Me.ElapsedTimeTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.White
        Me.ElapsedTimeTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.ElapsedTimeTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
        Me.ElapsedTimeTextEdit.Properties.Appearance.Options.UseBackColor = true
        Me.ElapsedTimeTextEdit.Properties.Appearance.Options.UseFont = true
        Me.ElapsedTimeTextEdit.Properties.Appearance.Options.UseForeColor = true
        Me.ElapsedTimeTextEdit.Properties.Appearance.Options.UseTextOptions = true
        Me.ElapsedTimeTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.ElapsedTimeTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
        Me.ElapsedTimeTextEdit.Properties.ReadOnly = true
        Me.ElapsedTimeTextEdit.ShowToolTips = false
        Me.ElapsedTimeTextEdit.Size = New System.Drawing.Size(60, 22)
        Me.ElapsedTimeTextEdit.TabIndex = 37
        '
        'OverallTimeTextEdit
        '
        Me.OverallTimeTextEdit.CausesValidation = false
        Me.OverallTimeTextEdit.EditValue = ""
        Me.OverallTimeTextEdit.Location = New System.Drawing.Point(128, 40)
        Me.OverallTimeTextEdit.Name = "OverallTimeTextEdit"
        Me.OverallTimeTextEdit.Properties.AllowFocused = false
        Me.OverallTimeTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.White
        Me.OverallTimeTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.OverallTimeTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
        Me.OverallTimeTextEdit.Properties.Appearance.Options.UseBackColor = true
        Me.OverallTimeTextEdit.Properties.Appearance.Options.UseFont = true
        Me.OverallTimeTextEdit.Properties.Appearance.Options.UseForeColor = true
        Me.OverallTimeTextEdit.Properties.Appearance.Options.UseTextOptions = true
        Me.OverallTimeTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.OverallTimeTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
        Me.OverallTimeTextEdit.Properties.ReadOnly = true
        Me.OverallTimeTextEdit.ShowToolTips = false
        Me.OverallTimeTextEdit.Size = New System.Drawing.Size(60, 22)
        Me.OverallTimeTextEdit.TabIndex = 36
        '
        'OverallTimeTitleLabel
        '
        Me.OverallTimeTitleLabel.BackColor = System.Drawing.Color.Transparent
        Me.OverallTimeTitleLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.OverallTimeTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.OverallTimeTitleLabel.Location = New System.Drawing.Point(7, 44)
        Me.OverallTimeTitleLabel.Name = "OverallTimeTitleLabel"
        Me.OverallTimeTitleLabel.Size = New System.Drawing.Size(119, 13)
        Me.OverallTimeTitleLabel.TabIndex = 36
        Me.OverallTimeTitleLabel.Text = "Overall Time"
        Me.OverallTimeTitleLabel.Title = false
        '
        'RemainingTimeLabel
        '
        Me.RemainingTimeLabel.BackColor = System.Drawing.Color.Transparent
        Me.RemainingTimeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.RemainingTimeLabel.ForeColor = System.Drawing.Color.Black
        Me.RemainingTimeLabel.Location = New System.Drawing.Point(7, 145)
        Me.RemainingTimeLabel.Name = "RemainingTimeLabel"
        Me.RemainingTimeLabel.Size = New System.Drawing.Size(120, 13)
        Me.RemainingTimeLabel.TabIndex = 26
        Me.RemainingTimeLabel.Text = "Remaining Time"
        Me.RemainingTimeLabel.Title = false
        '
        'bsTimeLabel
        '
        Me.bsTimeLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTimeLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsTimeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTimeLabel.Location = New System.Drawing.Point(5, 12)
        Me.bsTimeLabel.Name = "bsTimeLabel"
        Me.bsTimeLabel.Size = New System.Drawing.Size(187, 19)
        Me.bsTimeLabel.TabIndex = 25
        Me.bsTimeLabel.Text = "Time"
        Me.bsTimeLabel.Title = true
        '
        'BsGroupBox10
        '
        Me.BsGroupBox10.BackColor = System.Drawing.Color.Transparent
        Me.BsGroupBox10.Controls.Add(Me.BsISELongTermDeactivated)
        Me.BsGroupBox10.Controls.Add(Me.TestRefresh)
        Me.BsGroupBox10.Controls.Add(Me.StateCfgLabel)
        Me.BsGroupBox10.Controls.Add(Me.bsFridgeStatusLed)
        Me.BsGroupBox10.Controls.Add(Me.bsConnectedLed)
        Me.BsGroupBox10.Controls.Add(Me.bsISEStatusLed)
        Me.BsGroupBox10.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.BsGroupBox10.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox10.Location = New System.Drawing.Point(3, -4)
        Me.BsGroupBox10.Name = "BsGroupBox10"
        Me.BsGroupBox10.Size = New System.Drawing.Size(197, 124)
        Me.BsGroupBox10.TabIndex = 33
        Me.BsGroupBox10.TabStop = false
        '
        'BsISELongTermDeactivated
        '
        Me.BsISELongTermDeactivated.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.BsISELongTermDeactivated.Location = New System.Drawing.Point(141, 93)
        Me.BsISELongTermDeactivated.Name = "BsISELongTermDeactivated"
        Me.BsISELongTermDeactivated.PositionNumber = 0
        Me.BsISELongTermDeactivated.Size = New System.Drawing.Size(23, 23)
        Me.BsISELongTermDeactivated.TabIndex = 36
        Me.BsISELongTermDeactivated.TabStop = false
        Me.BsISELongTermDeactivated.Visible = false
        '
        'TestRefresh
        '
        Me.TestRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.TestRefresh.Location = New System.Drawing.Point(121, 11)
        Me.TestRefresh.Name = "TestRefresh"
        Me.TestRefresh.Size = New System.Drawing.Size(60, 19)
        Me.TestRefresh.TabIndex = 35
        Me.TestRefresh.Text = "Refresh"
        Me.TestRefresh.UseVisualStyleBackColor = true
        Me.TestRefresh.Visible = false
        '
        'StateCfgLabel
        '
        Me.StateCfgLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.StateCfgLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.StateCfgLabel.ForeColor = System.Drawing.Color.Black
        Me.StateCfgLabel.Location = New System.Drawing.Point(5, 12)
        Me.StateCfgLabel.Name = "StateCfgLabel"
        Me.StateCfgLabel.Size = New System.Drawing.Size(187, 19)
        Me.StateCfgLabel.TabIndex = 25
        Me.StateCfgLabel.Text = "State/Config"
        Me.StateCfgLabel.Title = true
        '
        'bsFridgeStatusLed
        '
        Me.bsFridgeStatusLed.BackColor = System.Drawing.Color.Transparent
        Me.bsFridgeStatusLed.Location = New System.Drawing.Point(7, 64)
        Me.bsFridgeStatusLed.Name = "bsFridgeStatusLed"
        Me.bsFridgeStatusLed.Size = New System.Drawing.Size(185, 23)
        Me.bsFridgeStatusLed.StateColor = Biosystems.Ax00.Controls.UserControls.bsLed.LedColors.GRAY
        Me.bsFridgeStatusLed.StateIndex = 0
        Me.bsFridgeStatusLed.TabIndex = 0
        Me.bsFridgeStatusLed.Title = "Fridge"
        '
        'bsConnectedLed
        '
        Me.bsConnectedLed.BackColor = System.Drawing.Color.Transparent
        Me.bsConnectedLed.Location = New System.Drawing.Point(7, 37)
        Me.bsConnectedLed.Name = "bsConnectedLed"
        Me.bsConnectedLed.Size = New System.Drawing.Size(185, 23)
        Me.bsConnectedLed.StateColor = Biosystems.Ax00.Controls.UserControls.bsLed.LedColors.GRAY
        Me.bsConnectedLed.StateIndex = 0
        Me.bsConnectedLed.TabIndex = 3
        Me.bsConnectedLed.Title = "PC Connected"
        '
        'bsISEStatusLed
        '
        Me.bsISEStatusLed.BackColor = System.Drawing.Color.Transparent
        Me.bsISEStatusLed.Location = New System.Drawing.Point(7, 93)
        Me.bsISEStatusLed.Name = "bsISEStatusLed"
        Me.bsISEStatusLed.Size = New System.Drawing.Size(185, 23)
        Me.bsISEStatusLed.StateColor = Biosystems.Ax00.Controls.UserControls.bsLed.LedColors.GRAY
        Me.bsISEStatusLed.StateIndex = 0
        Me.bsISEStatusLed.TabIndex = 2
        Me.bsISEStatusLed.Title = "ISE Module"
        '
        'bsSamplesLegendGroupBox
        '
        Me.bsSamplesLegendGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsSensorsLabel)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsReactionsTemperatureLed)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsFridgeTemperatureLed)
        Me.bsSamplesLegendGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.bsSamplesLegendGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesLegendGroupBox.Location = New System.Drawing.Point(3, 120)
        Me.bsSamplesLegendGroupBox.Name = "bsSamplesLegendGroupBox"
        Me.bsSamplesLegendGroupBox.Size = New System.Drawing.Size(197, 96)
        Me.bsSamplesLegendGroupBox.TabIndex = 32
        Me.bsSamplesLegendGroupBox.TabStop = false
        '
        'bsSensorsLabel
        '
        Me.bsSensorsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSensorsLabel.Font = New System.Drawing.Font("Verdana", 10!)
        Me.bsSensorsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSensorsLabel.Location = New System.Drawing.Point(5, 12)
        Me.bsSensorsLabel.Name = "bsSensorsLabel"
        Me.bsSensorsLabel.Size = New System.Drawing.Size(187, 19)
        Me.bsSensorsLabel.TabIndex = 25
        Me.bsSensorsLabel.Text = "Temperatures"
        Me.bsSensorsLabel.Title = true
        '
        'bsReactionsTemperatureLed
        '
        Me.bsReactionsTemperatureLed.BackColor = System.Drawing.Color.Transparent
        Me.bsReactionsTemperatureLed.Location = New System.Drawing.Point(7, 37)
        Me.bsReactionsTemperatureLed.Name = "bsReactionsTemperatureLed"
        Me.bsReactionsTemperatureLed.Size = New System.Drawing.Size(185, 23)
        Me.bsReactionsTemperatureLed.StateColor = Biosystems.Ax00.Controls.UserControls.bsLed.LedColors.GRAY
        Me.bsReactionsTemperatureLed.StateIndex = 0
        Me.bsReactionsTemperatureLed.TabIndex = 0
        Me.bsReactionsTemperatureLed.Title = "Reactions Rotor"
        '
        'bsFridgeTemperatureLed
        '
        Me.bsFridgeTemperatureLed.BackColor = System.Drawing.Color.Transparent
        Me.bsFridgeTemperatureLed.Location = New System.Drawing.Point(7, 64)
        Me.bsFridgeTemperatureLed.Name = "bsFridgeTemperatureLed"
        Me.bsFridgeTemperatureLed.Size = New System.Drawing.Size(185, 23)
        Me.bsFridgeTemperatureLed.StateColor = Biosystems.Ax00.Controls.UserControls.bsLed.LedColors.GRAY
        Me.bsFridgeTemperatureLed.StateIndex = 0
        Me.bsFridgeTemperatureLed.TabIndex = 1
        Me.bsFridgeTemperatureLed.Title = "Fridge"
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.BsGroupBox1.Controls.Add(Me.BsLabel1)
        Me.BsGroupBox1.Controls.Add(Me.BsDataGridView1)
        Me.BsGroupBox1.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox1.Location = New System.Drawing.Point(2, -4)
        Me.BsGroupBox1.Margin = New System.Windows.Forms.Padding(4, 3, 3, 3)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New System.Drawing.Size(757, 624)
        Me.BsGroupBox1.TabIndex = 51
        Me.BsGroupBox1.TabStop = false
        '
        'BsLabel1
        '
        Me.BsLabel1.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsLabel1.Font = New System.Drawing.Font("Verdana", 10!)
        Me.BsLabel1.ForeColor = System.Drawing.Color.Black
        Me.BsLabel1.Location = New System.Drawing.Point(5, 12)
        Me.BsLabel1.Name = "BsLabel1"
        Me.BsLabel1.Size = New System.Drawing.Size(746, 19)
        Me.BsLabel1.TabIndex = 13
        Me.BsLabel1.Text = "Alarms List"
        Me.BsLabel1.Title = true
        '
        'BsDataGridView1
        '
        Me.BsDataGridView1.AllowUserToAddRows = false
        Me.BsDataGridView1.AllowUserToDeleteRows = false
        DataGridViewCellStyle6.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White
        Me.BsDataGridView1.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle6
        Me.BsDataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.BsDataGridView1.BackgroundColor = System.Drawing.Color.LightGray
        Me.BsDataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.BsDataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle7.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BsDataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.BsDataGridView1.ColumnHeadersHeight = 30
        Me.BsDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BsDataGridView1.DefaultCellStyle = DataGridViewCellStyle8
        Me.BsDataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.BsDataGridView1.EnterToTab = false
        Me.BsDataGridView1.GridColor = System.Drawing.Color.Silver
        Me.BsDataGridView1.Location = New System.Drawing.Point(5, 34)
        Me.BsDataGridView1.Name = "BsDataGridView1"
        Me.BsDataGridView1.ReadOnly = true
        Me.BsDataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle9.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BsDataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.BsDataGridView1.RowHeadersVisible = false
        Me.BsDataGridView1.RowHeadersWidth = 20
        Me.BsDataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle10.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White
        Me.BsDataGridView1.RowsDefaultCellStyle = DataGridViewCellStyle10
        Me.BsDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.BsDataGridView1.Size = New System.Drawing.Size(746, 584)
        Me.BsDataGridView1.TabIndex = 9
        Me.BsDataGridView1.TabToEnter = false
        '
        'bsErrorProvider1
        '
        Me.bsErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsErrorProvider1.ContainerControl = Me
        '
        'IMonitor
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = true
        Me.Appearance.Options.UseFont = true
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.Controls.Add(Me.PanelControl2)
        Me.Controls.Add(Me.MonitorTabs)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = false
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "UiMonitor"
        Me.ShowIcon = false
        Me.ShowInTaskbar = false
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "UiMonitor"
        CType(Me.MonitorTabs,System.ComponentModel.ISupportInitialize).EndInit
        Me.MonitorTabs.ResumeLayout(false)
        Me.MainTab.ResumeLayout(false)
        CType(Me.chart2,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.CoverSamplesPicture.Properties,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.CoverReactionsPicture.Properties,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.CoverReagentsPicture.Properties,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.CoverOnPicture.Properties,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.CoverOffPicture.Properties,System.ComponentModel.ISupportInitialize).EndInit
        Me.StatesTab.ResumeLayout(false)
        Me.LegendWSGroupBox.ResumeLayout(false)
        CType(Me.LegendPendingImage0,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LegendFinishedImage0,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LegendInProgressImage0,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.ExportLISImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PausedTestImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LockedTestImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.ResultsAbsImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.FinalReportImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.ResultsWarningImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.ResultsReadyImage,System.ComponentModel.ISupportInitialize).EndInit
        Me.BsGroupBox9.ResumeLayout(false)
        CType(Me.TotalTestsChart,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsWSExecutionsDataGridView,System.ComponentModel.ISupportInitialize).EndInit
        Me.SamplesTab.ResumeLayout(false)
        CType(Me.PanelControl10,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PanelControl6,System.ComponentModel.ISupportInitialize).EndInit
        Me.PanelControl6.ResumeLayout(false)
        CType(Me.PanelControl7,System.ComponentModel.ISupportInitialize).EndInit
        Me.PanelControl7.ResumeLayout(false)
        Me.LegendSamplesGroupBox.ResumeLayout(false)
        CType(Me.LegendPendingImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LegendBarCodeErrorImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LegendFinishedImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LegendDepletedImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LegendInProgressImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LegendSelectedImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LegendNotInUseImage,System.ComponentModel.ISupportInitialize).EndInit
        Me.BsGroupBox11.ResumeLayout(false)
        Me.BsGroupBox11.PerformLayout
        CType(Me.Sam3106,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3128,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3129,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam395,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam396,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam397,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3107,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3134,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3133,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3132,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3131,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3127,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3126,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3125,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3124,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3123,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3122,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3121,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3120,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3119,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3118,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3117,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3116,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3115,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3114,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3113,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3112,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3111,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3110,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3109,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3108,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3105,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3104,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3103,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3102,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3101,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3100,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3135,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam399,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam398,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam394,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam393,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam392,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam391,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam290,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam289,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam288,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam287,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam286,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam285,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam284,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam283,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam282,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam281,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam280,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam279,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam278,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam277,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam276,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam275,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam274,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam273,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam272,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam271,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam270,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam269,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam268,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam267,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam266,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam265,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam264,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam263,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam262,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam261,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam260,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam259,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam258,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam257,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam256,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam255,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam254,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam253,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam252,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam251,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam250,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam249,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam248,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam247,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam246,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam145,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam144,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam143,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam142,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam11,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam141,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam140,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam139,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam138,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam137,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam136,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam135,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam134,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam133,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam132,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam131,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam130,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam129,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam128,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam127,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam126,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam125,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam124,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam123,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam122,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam121,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam120,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam119,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam118,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam117,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam116,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam115,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam114,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam113,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam112,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam111,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam110,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam19,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam18,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam17,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam16,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam15,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam14,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam13,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam12,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Sam3130,System.ComponentModel.ISupportInitialize).EndInit
        Me.ReagentsTab.ResumeLayout(false)
        CType(Me.PanelControl9,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag11,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag12,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag13,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag14,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag15,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag16,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag17,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag18,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag19,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag110,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag111,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag112,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag113,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag114,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag115,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag116,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag117,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag118,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag119,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag120,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag121,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag122,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag123,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag124,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag125,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag126,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag127,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag128,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag129,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag130,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag131,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag132,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag133,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag134,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag135,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag136,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag137,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag138,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag139,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag140,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag141,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag142,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag143,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag144,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PanelControl4,System.ComponentModel.ISupportInitialize).EndInit
        Me.PanelControl4.ResumeLayout(false)
        CType(Me.PanelControl3,System.ComponentModel.ISupportInitialize).EndInit
        Me.PanelControl3.ResumeLayout(false)
        Me.LegendReagentsGroupBox.ResumeLayout(false)
        CType(Me.LegendUnknownImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LegendBarCodeErrorRGImage,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.LowVolPictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.ReagentPictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.AdditionalSolPictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.NoInUsePictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.SelectedPictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsDepletedPictureBox,System.ComponentModel.ISupportInitialize).EndInit
        Me.bsReagentsPositionInfoGroupBox.ResumeLayout(false)
        Me.bsReagentsPositionInfoGroupBox.PerformLayout
        CType(Me.Reag288,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag287,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag286,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag285,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag284,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag283,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag282,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag281,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag280,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag279,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag278,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag277,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag276,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag275,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag274,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag273,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag272,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag271,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag270,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag269,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag268,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag267,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag266,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag265,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag264,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag263,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag262,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag261,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag260,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag259,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag258,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag257,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag256,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag255,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag254,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag253,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag252,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag251,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag250,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag249,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag248,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag247,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag246,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reag245,System.ComponentModel.ISupportInitialize).EndInit
        Me.ReactionsTab.ResumeLayout(false)
        CType(Me.Reac2,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac39,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac29,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac28,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac27,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac16,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac15,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac14,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac13,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac12,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac11,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac10,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac9,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac8,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac7,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac6,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac5,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac4,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac3,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac1,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac66,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac67,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac117,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac77,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac78,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac26,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac37,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac38,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac120,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac119,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac118,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac116,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac115,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac114,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac113,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac112,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac111,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac110,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac109,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac108,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac107,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac106,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac105,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac104,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac103,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac102,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac100,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac99,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac98,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac97,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac96,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac95,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac94,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac93,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac92,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac91,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac90,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac88,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac86,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac85,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac84,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac83,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac82,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac81,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac80,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac79,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac76,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac74,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac73,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac72,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac71,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac70,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac69,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac68,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac65,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac64,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac63,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac62,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac61,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac60,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac59,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac58,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac57,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac56,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac55,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac54,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac53,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac52,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac51,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac50,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac49,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac48,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac47,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac46,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac45,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac44,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac43,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac42,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac41,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac40,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac36,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac35,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac34,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac33,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac32,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac31,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac30,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac25,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac24,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac23,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac22,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac21,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac20,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac19,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac17,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PanelControl14,System.ComponentModel.ISupportInitialize).EndInit
        Me.PanelControl14.ResumeLayout(false)
        CType(Me.PanelControl15,System.ComponentModel.ISupportInitialize).EndInit
        Me.PanelControl15.ResumeLayout(false)
        Me.LegendReactionsGroupBox.ResumeLayout(false)
        CType(Me.bsOpticalPictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsContaminatedPictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsR1SamplePictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsFinishPictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsDilutionPictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsR1PictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsR1SampleR2PictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsWashingPictureBox,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsNotInUsePictureBox,System.ComponentModel.ISupportInitialize).EndInit
        Me.BsGroupBox13.ResumeLayout(false)
        Me.BsGroupBox13.PerformLayout
        CType(Me.Reac18,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac75,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac87,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac89,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.Reac101,System.ComponentModel.ISupportInitialize).EndInit
        Me.ISETab.ResumeLayout(false)
        Me.AlarmsTab.ResumeLayout(false)
        Me.BsGroupBox4.ResumeLayout(false)
        CType(Me.AlarmsXtraGrid,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.AlarmsXtraGridView,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.TaskListProgressBar.Properties,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.PanelControl2,System.ComponentModel.ISupportInitialize).EndInit
        Me.PanelControl2.ResumeLayout(false)
        Me.bsWamUpGroupBox.ResumeLayout(false)
        Me.bsWamUpGroupBox.PerformLayout
        CType(Me.TimeWarmUpProgressBar.Properties,System.ComponentModel.ISupportInitialize).EndInit
        Me.BsGroupBox14.ResumeLayout(false)
        CType(Me.AccessRRTimeTextEdit.Properties,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.AccessRMTimeTextEdit.Properties,System.ComponentModel.ISupportInitialize).EndInit
        Me.BsGroupBox3.ResumeLayout(false)
        CType(Me.PanelControl11,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.RemainingTimeTextEdit.Properties,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.ElapsedTimeTextEdit.Properties,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.OverallTimeTextEdit.Properties,System.ComponentModel.ISupportInitialize).EndInit
        Me.BsGroupBox10.ResumeLayout(false)
        CType(Me.BsISELongTermDeactivated,System.ComponentModel.ISupportInitialize).EndInit
        Me.bsSamplesLegendGroupBox.ResumeLayout(false)
        Me.BsGroupBox1.ResumeLayout(false)
        CType(Me.BsDataGridView1,System.ComponentModel.ISupportInitialize).EndInit
        CType(Me.bsErrorProvider1,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)

End Sub

    Friend WithEvents MonitorTabs As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents MainTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents StatesTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents SamplesTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents ReagentsTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents ReactionsTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents ISETab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents WasteLabel As System.Windows.Forms.Label
    Friend WithEvents WashingLabel As System.Windows.Forms.Label
    Friend WithEvents BsGroupBox9 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Private WithEvents TotalTestsChart As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents bsTestStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsWSExecutionsDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents ElapsedTimeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents TaskListProgressBar As DevExpress.XtraEditors.ProgressBarControl
    Friend WithEvents PanelControl2 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BsGroupBox3 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTimeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsFridgeStatusLed As Biosystems.Ax00.Controls.UserControls.bsLed
    Friend WithEvents BsGroupBox10 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents StateCfgLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsConnectedLed As Biosystems.Ax00.Controls.UserControls.bsLed
    Friend WithEvents bsISEStatusLed As Biosystems.Ax00.Controls.UserControls.bsLed
    Friend WithEvents bsSamplesLegendGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsSensorsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReactionsTemperatureLed As Biosystems.Ax00.Controls.UserControls.bsLed
    Friend WithEvents bsFridgeTemperatureLed As Biosystems.Ax00.Controls.UserControls.bsLed
    Friend WithEvents bsSampleNumberTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleContentTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleDiskNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleCellTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSamplesNumberLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesContentLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesDiskNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesCellLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesBarcodeTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsDiluteStatusTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleTypeTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSamplesBarcodeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDiluteStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSampleIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesMoveLastPositionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSamplesIncreaseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSamplesDecreaseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSamplesMoveFirstPositionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTubeSizeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsTubeSizeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents Sam17 As BSRImage
    Friend WithEvents Sam16 As BSRImage
    Friend WithEvents Sam15 As BSRImage
    Friend WithEvents Sam14 As BSRImage
    Friend WithEvents Sam13 As BSRImage
    Friend WithEvents Sam12 As BSRImage
    Friend WithEvents Sam114 As BSRImage
    Friend WithEvents Sam113 As BSRImage
    Friend WithEvents Sam112 As BSRImage
    Friend WithEvents Sam111 As BSRImage
    Friend WithEvents Sam110 As BSRImage
    Friend WithEvents Sam19 As BSRImage
    Friend WithEvents Sam18 As BSRImage
    Friend WithEvents Sam120 As BSRImage
    Friend WithEvents Sam119 As BSRImage
    Friend WithEvents Sam118 As BSRImage
    Friend WithEvents Sam117 As BSRImage
    Friend WithEvents Sam116 As BSRImage
    Friend WithEvents Sam115 As BSRImage
    Friend WithEvents Sam126 As BSRImage
    Friend WithEvents Sam125 As BSRImage
    Friend WithEvents Sam124 As BSRImage
    Friend WithEvents Sam123 As BSRImage
    Friend WithEvents Sam122 As BSRImage
    Friend WithEvents Sam121 As BSRImage
    Friend WithEvents Sam134 As BSRImage
    Friend WithEvents Sam133 As BSRImage
    Friend WithEvents Sam132 As BSRImage
    Friend WithEvents Sam131 As BSRImage
    Friend WithEvents Sam130 As BSRImage
    Friend WithEvents Sam129 As BSRImage
    Friend WithEvents Sam128 As BSRImage
    Friend WithEvents Sam127 As BSRImage
    Friend WithEvents Sam141 As BSRImage
    Friend WithEvents Sam140 As BSRImage
    Friend WithEvents Sam139 As BSRImage
    Friend WithEvents Sam138 As BSRImage
    Friend WithEvents Sam137 As BSRImage
    Friend WithEvents Sam136 As BSRImage
    Friend WithEvents Sam135 As BSRImage
    Friend WithEvents Sam145 As BSRImage
    Friend WithEvents Sam144 As BSRImage
    Friend WithEvents Sam143 As BSRImage
    Friend WithEvents Sam142 As BSRImage
    Friend WithEvents Sam11 As BSRImage
    Friend WithEvents Sam255 As BSRImage
    Friend WithEvents Sam254 As BSRImage
    Friend WithEvents Sam253 As BSRImage
    Friend WithEvents Sam252 As BSRImage
    Friend WithEvents Sam251 As BSRImage
    Friend WithEvents Sam250 As BSRImage
    Friend WithEvents Sam249 As BSRImage
    Friend WithEvents Sam248 As BSRImage
    Friend WithEvents Sam247 As BSRImage
    Friend WithEvents Sam246 As BSRImage
    Friend WithEvents Sam265 As BSRImage
    Friend WithEvents Sam264 As BSRImage
    Friend WithEvents Sam263 As BSRImage
    Friend WithEvents Sam262 As BSRImage
    Friend WithEvents Sam261 As BSRImage
    Friend WithEvents Sam260 As BSRImage
    Friend WithEvents Sam259 As BSRImage
    Friend WithEvents Sam258 As BSRImage
    Friend WithEvents Sam257 As BSRImage
    Friend WithEvents Sam256 As BSRImage
    Friend WithEvents Sam274 As BSRImage
    Friend WithEvents Sam273 As BSRImage
    Friend WithEvents Sam272 As BSRImage
    Friend WithEvents Sam271 As BSRImage
    Friend WithEvents Sam270 As BSRImage
    Friend WithEvents Sam269 As BSRImage
    Friend WithEvents Sam268 As BSRImage
    Friend WithEvents Sam267 As BSRImage
    Friend WithEvents Sam266 As BSRImage
    Friend WithEvents Sam281 As BSRImage
    Friend WithEvents Sam280 As BSRImage
    Friend WithEvents Sam279 As BSRImage
    Friend WithEvents Sam278 As BSRImage
    Friend WithEvents Sam277 As BSRImage
    Friend WithEvents Sam276 As BSRImage
    Friend WithEvents Sam275 As BSRImage
    Friend WithEvents Sam289 As BSRImage
    Friend WithEvents Sam288 As BSRImage
    Friend WithEvents Sam287 As BSRImage
    Friend WithEvents Sam286 As BSRImage
    Friend WithEvents Sam285 As BSRImage
    Friend WithEvents Sam284 As BSRImage
    Friend WithEvents Sam283 As BSRImage
    Friend WithEvents Sam282 As BSRImage
    Friend WithEvents Sam290 As BSRImage
    Friend WithEvents Sam3135 As BSRImage
    Friend WithEvents Sam399 As BSRImage
    Friend WithEvents Sam398 As BSRImage
    Friend WithEvents Sam397 As BSRImage
    Friend WithEvents Sam396 As BSRImage
    Friend WithEvents Sam395 As BSRImage
    Friend WithEvents Sam394 As BSRImage
    Friend WithEvents Sam393 As BSRImage
    Friend WithEvents Sam392 As BSRImage
    Friend WithEvents Sam391 As BSRImage
    Friend WithEvents Sam3109 As BSRImage
    Friend WithEvents Sam3108 As BSRImage
    Friend WithEvents Sam3107 As BSRImage
    Friend WithEvents Sam3106 As BSRImage
    Friend WithEvents Sam3105 As BSRImage
    Friend WithEvents Sam3104 As BSRImage
    Friend WithEvents Sam3103 As BSRImage
    Friend WithEvents Sam3102 As BSRImage
    Friend WithEvents Sam3101 As BSRImage
    Friend WithEvents Sam3100 As BSRImage
    Friend WithEvents Sam3115 As BSRImage
    Friend WithEvents Sam3114 As BSRImage
    Friend WithEvents Sam3113 As BSRImage
    Friend WithEvents Sam3112 As BSRImage
    Friend WithEvents Sam3111 As BSRImage
    Friend WithEvents Sam3110 As BSRImage
    Friend WithEvents Sam3125 As BSRImage
    Friend WithEvents Sam3124 As BSRImage
    Friend WithEvents Sam3123 As BSRImage
    Friend WithEvents Sam3122 As BSRImage
    Friend WithEvents Sam3121 As BSRImage
    Friend WithEvents Sam3120 As BSRImage
    Friend WithEvents Sam3119 As BSRImage
    Friend WithEvents Sam3118 As BSRImage
    Friend WithEvents Sam3117 As BSRImage
    Friend WithEvents Sam3116 As BSRImage
    Friend WithEvents Sam3134 As BSRImage
    Friend WithEvents Sam3133 As BSRImage
    Friend WithEvents Sam3132 As BSRImage
    Friend WithEvents Sam3131 As BSRImage
    Friend WithEvents Sam3130 As BSRImage
    Friend WithEvents Sam3129 As BSRImage
    Friend WithEvents Sam3128 As BSRImage
    Friend WithEvents Sam3127 As BSRImage
    Friend WithEvents Sam3126 As BSRImage
    Friend WithEvents bsReagentsPositionInfoGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsReagentsCellTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsCellLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTeststLeftTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsCurrentVolTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsTestsLeftLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsCurrentVolLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsBottleSizeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsBottleSizeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsExpirationDateTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsPositionInfoLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsMoveLastPositionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReagentsIncreaseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReagentsDecreaseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReagentsMoveFirstPositionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReagentsBarCodeTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsTestNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsNumberTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsContentTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsDiskNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsExpirationDateLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsBarCodeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTestNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsNumberLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsContentLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsDiskNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents Reag11 As BSRImage
    Friend WithEvents Reag12 As BSRImage
    Public WithEvents Reag13 As BSRImage
    Friend WithEvents Reag14 As BSRImage
    Friend WithEvents Reag15 As BSRImage
    Friend WithEvents Reag16 As BSRImage
    Friend WithEvents Reag17 As BSRImage
    Friend WithEvents Reag18 As BSRImage
    Friend WithEvents Reag19 As BSRImage
    Friend WithEvents Reag110 As BSRImage
    Friend WithEvents Reag111 As BSRImage
    Friend WithEvents Reag112 As BSRImage
    Friend WithEvents Reag113 As BSRImage
    Friend WithEvents Reag114 As BSRImage
    Friend WithEvents Reag115 As BSRImage
    Friend WithEvents Reag116 As BSRImage
    Friend WithEvents Reag117 As BSRImage
    Friend WithEvents Reag118 As BSRImage
    Friend WithEvents Reag119 As BSRImage
    Friend WithEvents Reag120 As BSRImage
    Friend WithEvents Reag121 As BSRImage
    Friend WithEvents Reag122 As BSRImage
    Friend WithEvents Reag123 As BSRImage
    Friend WithEvents Reag124 As BSRImage
    Friend WithEvents Reag125 As BSRImage
    Friend WithEvents Reag126 As BSRImage
    Friend WithEvents Reag127 As BSRImage
    Friend WithEvents Reag128 As BSRImage
    Friend WithEvents Reag129 As BSRImage
    Friend WithEvents Reag130 As BSRImage
    Friend WithEvents Reag131 As BSRImage
    Friend WithEvents Reag132 As BSRImage
    Friend WithEvents Reag133 As BSRImage
    Friend WithEvents Reag134 As BSRImage
    Friend WithEvents Reag135 As BSRImage
    Friend WithEvents Reag136 As BSRImage
    Friend WithEvents Reag137 As BSRImage
    Friend WithEvents Reag138 As BSRImage
    Friend WithEvents Reag139 As BSRImage
    Friend WithEvents Reag140 As BSRImage
    Friend WithEvents Reag141 As BSRImage
    Friend WithEvents Reag142 As BSRImage
    Friend WithEvents Reag143 As BSRImage
    Friend WithEvents Reag144 As BSRImage
    Friend WithEvents PanelControl3 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LegendReagentsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents LegReagAdditionalSol As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegReagNoInUseLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegReagentSelLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegReagDepleteLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents AdditionalSolPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents NoInUsePictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents SelectedPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsDepletedPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsReagentsLegendLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents PanelControl4 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents bsReagentsStatusTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents PanelControl6 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents PanelControl7 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BsGroupBox11 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsSampleStatusTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSamplesStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesPositionInfoLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegendDepletedImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsSamplesLegendLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegendFinishedImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents LegendInProgressImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents LegendNotInUseImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents LegendBarCodeErrorImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BarcodeErrorLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents InProgressLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents FinishedLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents DepletedLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents NotInUseLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents SelectedLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents PendingLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegendPendingImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents RemainingTimeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents AccessRRTimeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents AccessRMTimeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegReagentLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegReagLowVolLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LowVolPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents ReagentPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents LegendSelectedImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents TestRefresh As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents LegendSamplesGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents OverallTimeTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents OverallTimeTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents ElapsedTimeTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents RemainingTimeTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents AccessRRTimeTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents AccessRMTimeTextEdit As DevExpress.XtraEditors.TextEdit
    Friend WithEvents PanelControl11 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents PanelControl14 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents PanelControl15 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LegendReactionsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsFinishLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsR1SampleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsR1SampleR2 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReactionsLegendLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDilutionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsR1SamplePictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsFinishPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsDilutionPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsR1PictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsR1SampleR2PictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsWashingPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsNotInUsePictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsR1Label As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsWashingLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsNotInUseLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsGroupBox13 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsReacStatusTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReacStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReactionsMoveFirstPositionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReactionsPositionInfoLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsWellNumberLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSampleClassLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReactionsDecreaseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCalibNumLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReactionsIncreaseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReactionsMoveLastPositionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsWellNrTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleClassTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsCalibNrTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReacSampleIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDilutionTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReacSampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReacSampleTypeTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReacDilutionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsPatientIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsRerunTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReplicateTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsRerunLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReplicateLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsOpticalLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsContaminatedLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsOpticalPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsContaminatedPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsGroupBox14 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTimeAvailableLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReacTestTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReacTestNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReactionsOpenGraph As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Reac1 As BSRImage
    Friend WithEvents Reac2 As BSRImage
    Friend WithEvents Reac3 As BSRImage
    Friend WithEvents Reac4 As BSRImage
    Friend WithEvents Reac7 As BSRImage
    Friend WithEvents Reac6 As BSRImage
    Friend WithEvents Reac5 As BSRImage
    Friend WithEvents Reac8 As BSRImage
    Friend WithEvents Reac9 As BSRImage
    Friend WithEvents Reac11 As BSRImage
    Friend WithEvents Reac10 As BSRImage
    Friend WithEvents Reac60 As BSRImage
    Friend WithEvents Reac13 As BSRImage
    Friend WithEvents Reac120 As BSRImage
    Friend WithEvents Reac12 As BSRImage
    Friend WithEvents Reac80 As BSRImage
    Friend WithEvents Reac14 As BSRImage
    Friend WithEvents Reac19 As BSRImage
    Friend WithEvents Reac15 As BSRImage
    Friend WithEvents Reac17 As BSRImage
    Friend WithEvents Reac16 As BSRImage
    Friend WithEvents Reac18 As BSRImage
    Friend WithEvents Reac20 As BSRImage
    Friend WithEvents Reac100 As BSRImage
    Friend WithEvents Reac40 As BSRImage
    Friend WithEvents Reac22 As BSRImage
    Friend WithEvents Reac21 As BSRImage
    Friend WithEvents Reac24 As BSRImage
    Friend WithEvents Reac23 As BSRImage
    Friend WithEvents Reac26 As BSRImage
    Friend WithEvents Reac25 As BSRImage
    Friend WithEvents Reac30 As BSRImage
    Friend WithEvents Reac29 As BSRImage
    Friend WithEvents Reac28 As BSRImage
    Friend WithEvents Reac27 As BSRImage
    Friend WithEvents Reac31 As BSRImage
    Friend WithEvents Reac37 As BSRImage
    Friend WithEvents Reac36 As BSRImage
    Friend WithEvents Reac35 As BSRImage
    Friend WithEvents Reac34 As BSRImage
    Friend WithEvents Reac33 As BSRImage
    Friend WithEvents Reac32 As BSRImage
    Friend WithEvents Reac43 As BSRImage
    Friend WithEvents Reac42 As BSRImage
    Friend WithEvents Reac41 As BSRImage
    Friend WithEvents Reac39 As BSRImage
    Friend WithEvents Reac38 As BSRImage
    Friend WithEvents Reac44 As BSRImage
    Friend WithEvents Reac50 As BSRImage
    Friend WithEvents Reac45 As BSRImage
    Friend WithEvents Reac48 As BSRImage
    Friend WithEvents Reac47 As BSRImage
    Friend WithEvents Reac46 As BSRImage
    Friend WithEvents Reac49 As BSRImage
    Friend WithEvents Reac55 As BSRImage
    Friend WithEvents Reac54 As BSRImage
    Friend WithEvents Reac53 As BSRImage
    Friend WithEvents Reac52 As BSRImage
    Friend WithEvents Reac51 As BSRImage
    Friend WithEvents Reac59 As BSRImage
    Friend WithEvents Reac58 As BSRImage
    Friend WithEvents Reac57 As BSRImage
    Friend WithEvents Reac56 As BSRImage
    Friend WithEvents Reac65 As BSRImage
    Friend WithEvents Reac64 As BSRImage
    Friend WithEvents Reac63 As BSRImage
    Friend WithEvents Reac62 As BSRImage
    Friend WithEvents Reac61 As BSRImage
    Friend WithEvents Reac68 As BSRImage
    Friend WithEvents Reac67 As BSRImage
    Friend WithEvents Reac66 As BSRImage
    Friend WithEvents Reac73 As BSRImage
    Friend WithEvents Reac72 As BSRImage
    Friend WithEvents Reac71 As BSRImage
    Friend WithEvents Reac70 As BSRImage
    Friend WithEvents Reac69 As BSRImage
    Friend WithEvents Reac76 As BSRImage
    Friend WithEvents Reac75 As BSRImage
    Friend WithEvents Reac74 As BSRImage
    Friend WithEvents Reac79 As BSRImage
    Friend WithEvents Reac78 As BSRImage
    Friend WithEvents Reac77 As BSRImage
    Friend WithEvents Reac85 As BSRImage
    Friend WithEvents Reac84 As BSRImage
    Friend WithEvents Reac83 As BSRImage
    Friend WithEvents Reac82 As BSRImage
    Friend WithEvents Reac81 As BSRImage
    Friend WithEvents Reac89 As BSRImage
    Friend WithEvents Reac88 As BSRImage
    Friend WithEvents Reac87 As BSRImage
    Friend WithEvents Reac86 As BSRImage
    Friend WithEvents Reac94 As BSRImage
    Friend WithEvents Reac93 As BSRImage
    Friend WithEvents Reac92 As BSRImage
    Friend WithEvents Reac91 As BSRImage
    Friend WithEvents Reac90 As BSRImage
    Friend WithEvents Reac99 As BSRImage
    Friend WithEvents Reac98 As BSRImage
    Friend WithEvents Reac97 As BSRImage
    Friend WithEvents Reac96 As BSRImage
    Friend WithEvents Reac95 As BSRImage
    Friend WithEvents Reac104 As BSRImage
    Friend WithEvents Reac103 As BSRImage
    Friend WithEvents Reac102 As BSRImage
    Friend WithEvents Reac101 As BSRImage
    Friend WithEvents Reac111 As BSRImage
    Friend WithEvents Reac110 As BSRImage
    Friend WithEvents Reac109 As BSRImage
    Friend WithEvents Reac108 As BSRImage
    Friend WithEvents Reac107 As BSRImage
    Friend WithEvents Reac106 As BSRImage
    Friend WithEvents Reac105 As BSRImage
    Friend WithEvents Reac112 As BSRImage
    Friend WithEvents Reac114 As BSRImage
    Friend WithEvents Reac113 As BSRImage
    Friend WithEvents Reac117 As BSRImage
    Friend WithEvents Reac116 As BSRImage
    Friend WithEvents Reac115 As BSRImage
    Friend WithEvents Reac119 As BSRImage
    Friend WithEvents Reac118 As BSRImage
    Friend WithEvents BsExecutionIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsOrderTestIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsTimer1 As Biosystems.Ax00.Controls.UserControls.BSTimer
    Friend WithEvents PanelControl10 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LegendBarCodeErrorRGImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BarcodeErrorRGLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegendUnknownImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents UnknownLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsWamUpGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsEndWarmUp As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents TimeWarmUpProgressBar As DevExpress.XtraEditors.ProgressBarControl
    Friend WithEvents bsTimeWUpLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents Reag245 As BSRImage
    Friend WithEvents Reag246 As BSRImage
    Friend WithEvents Reag247 As BSRImage
    Friend WithEvents Reag248 As BSRImage
    Friend WithEvents Reag249 As BSRImage
    Friend WithEvents Reag250 As BSRImage
    Friend WithEvents Reag251 As BSRImage
    Friend WithEvents Reag252 As BSRImage
    Friend WithEvents Reag253 As BSRImage
    Friend WithEvents Reag254 As BSRImage
    Friend WithEvents Reag255 As BSRImage
    Friend WithEvents Reag256 As BSRImage
    Friend WithEvents Reag257 As BSRImage
    Friend WithEvents Reag258 As BSRImage
    Friend WithEvents Reag259 As BSRImage
    Friend WithEvents Reag260 As BSRImage
    Friend WithEvents Reag261 As BSRImage
    Friend WithEvents Reag262 As BSRImage
    Friend WithEvents Reag263 As BSRImage
    Friend WithEvents Reag264 As BSRImage
    Friend WithEvents Reag265 As BSRImage
    Friend WithEvents Reag266 As BSRImage
    Friend WithEvents Reag267 As BSRImage
    Friend WithEvents Reag268 As BSRImage
    Friend WithEvents Reag269 As BSRImage
    Friend WithEvents Reag270 As BSRImage
    Friend WithEvents Reag271 As BSRImage
    Friend WithEvents Reag272 As BSRImage
    Friend WithEvents Reag273 As BSRImage
    Friend WithEvents Reag274 As BSRImage
    Friend WithEvents Reag275 As BSRImage
    Friend WithEvents Reag276 As BSRImage
    Friend WithEvents Reag277 As BSRImage
    Friend WithEvents Reag278 As BSRImage
    Friend WithEvents Reag279 As BSRImage
    Friend WithEvents Reag280 As BSRImage
    Friend WithEvents Reag281 As BSRImage
    Friend WithEvents Reag282 As BSRImage
    Friend WithEvents Reag283 As BSRImage
    Friend WithEvents Reag284 As BSRImage
    Friend WithEvents Reag285 As BSRImage
    Friend WithEvents Reag286 As BSRImage
    Friend WithEvents Reag287 As BSRImage
    Friend WithEvents Reag288 As BSRImage
    Friend WithEvents PanelControl9 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LegendWSGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents ResultsStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents PausedTestImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents PausedTestLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LockedTestImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents ResultsAbsImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents LockedTestLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents ResultsAbsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents FinalReportLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents ResultsWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents ResultsReadyLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents TestStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents FinalReportImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents ResultsWarningImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents ResultsReadyImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsWorksessionLegendLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents ExportLISLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents ExportLISImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents PendingLabel0 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents InProgressLabel0 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents FinishedLabel0 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegendPendingImage0 As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents LegendFinishedImage0 As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents LegendInProgressImage0 As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents SampleStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents AlarmsTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents BsGroupBox4 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsGroupBox1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsDataGridView1 As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents AlarmsXtraGrid As DevExpress.XtraGrid.GridControl
    Friend WithEvents AlarmsXtraGridView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents BsIseMonitor As Biosystems.Ax00.Controls.UserControls.BSISEMonitorPanel
    Friend WithEvents BsISELongTermDeactivated As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents CoverOffPicture As DevExpress.XtraEditors.PictureEdit
    Private WithEvents chart2 As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents CoverOnPicture As DevExpress.XtraEditors.PictureEdit
    Friend WithEvents CoverReagentsPicture As DevExpress.XtraEditors.PictureEdit
    Friend WithEvents CoverSamplesPicture As DevExpress.XtraEditors.PictureEdit
    Friend WithEvents CoverReactionsPicture As BSGlyphPictureEdit
    Friend WithEvents ToolTipController1 As DevExpress.Utils.ToolTipController
End Class

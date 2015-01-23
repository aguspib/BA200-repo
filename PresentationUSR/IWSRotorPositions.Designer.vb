<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiWSRotorPositions
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            CreateLogActivity("Initial - Dispose", Me.Name & ".Dispose", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            ReleaseElements()
        Finally
            MyBase.Dispose(disposing)
            isClosingFlag = False
            CreateLogActivity("Final - Dispose", Me.Name & ".Dispose", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiWSRotorPositions))
        Me.bsScreenTimer = New System.Windows.Forms.Timer(Me.components)
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.FunctionalityArea = New System.Windows.Forms.Panel()
        Me.BarcodeWarningButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsScanningButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsCheckRotorVolumeButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsWarningsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSaveVRotorButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsLoadVRotorButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsResetRotorButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsReagentAutoPosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSamplesAutoPosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.RotorsTabs = New DevExpress.XtraTab.XtraTabControl()
        Me.SamplesTab = New DevExpress.XtraTab.XtraTabPage()
        Me.Sam3127 = New BSRImage()
        Me.Sam3128 = New BSRImage()
        Me.Sam3129 = New BSRImage()
        Me.Sam3130 = New BSRImage()
        Me.Sam3131 = New BSRImage()
        Me.Sam3132 = New BSRImage()
        Me.Sam3133 = New BSRImage()
        Me.Sam3135 = New BSRImage()
        Me.PanelControl1 = New DevExpress.XtraEditors.PanelControl()
        Me.PanelControl2 = New DevExpress.XtraEditors.PanelControl()
        Me.bsSamplesLegendGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsTubeAddSolLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTubeAddSolPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsLegendDilutedLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDilutedPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsLegendRoutineLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLegendStatLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLegendControlsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLegendCalibratorsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsRoutinePictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsStatPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsControlPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsCalibratorPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsSamplesLegendLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesPositionInfoGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.SamplesStatusTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSamplesStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesPositionInfoLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesDeletePosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSamplesMoveLastPositionButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSamplesRefillPosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSamplesIncreaseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsTubeSizeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsSamplesBarcodeTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSamplesDecreaseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsDiluteStatusTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSampleTypeTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSamplesMoveFirstPositionButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSampleNumberTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSampleIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSampleContentTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSampleRingNumTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSampleCellTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsTubeSizeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesBarcodeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDiluteStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesNumberLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesContentLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesRingNumLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSamplesCellLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.Sam3106 = New BSRImage()
        Me.Sam395 = New BSRImage()
        Me.Sam396 = New BSRImage()
        Me.Sam397 = New BSRImage()
        Me.Sam3107 = New BSRImage()
        Me.Sam3134 = New BSRImage()
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
        Me.ReagentsTab = New DevExpress.XtraTab.XtraTabPage()
        Me.PanelControl6 = New DevExpress.XtraEditors.PanelControl()
        Me.PanelControl7 = New DevExpress.XtraEditors.PanelControl()
        Me.bsReagentsLegendGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.LegReagentSelLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.SelectedPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.LegendUnknownImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsUnknownLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LegendBarCodeErrorRGImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsBarcodeErrorRGLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.LowVolPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.ReagentPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsLegReagLowVolLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLegReagentLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLegReagAdditionalSol = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLegReagNoInUseLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLegReagDepleteLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.AdditionalSolPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.NoInUsePictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsDepletedPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsReagentsLegendLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsPositionInfoGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsReagStatusLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.ReagStatusTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsCellTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsCellLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsDeletePosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsTeststLeftTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsRefillPosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsCurrentVolTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsCheckVolumePosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
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
        Me.bsReagentsMoveFirstPositionButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsReagentsContentTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsReagentsRingNumTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsExpirationDateLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsBarCodeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTestNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsNumberLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsContentLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReagentsRingNumLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
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
        Me.Reag245 = New BSRImage()
        Me.Reag246 = New BSRImage()
        Me.Reag247 = New BSRImage()
        Me.Reag248 = New BSRImage()
        Me.Reag249 = New BSRImage()
        Me.Reag250 = New BSRImage()
        Me.Reag251 = New BSRImage()
        Me.Reag252 = New BSRImage()
        Me.Reag253 = New BSRImage()
        Me.Reag254 = New BSRImage()
        Me.Reag255 = New BSRImage()
        Me.Reag256 = New BSRImage()
        Me.Reag257 = New BSRImage()
        Me.Reag258 = New BSRImage()
        Me.Reag259 = New BSRImage()
        Me.Reag260 = New BSRImage()
        Me.Reag261 = New BSRImage()
        Me.Reag262 = New BSRImage()
        Me.Reag263 = New BSRImage()
        Me.Reag264 = New BSRImage()
        Me.Reag265 = New BSRImage()
        Me.Reag266 = New BSRImage()
        Me.Reag267 = New BSRImage()
        Me.Reag268 = New BSRImage()
        Me.Reag269 = New BSRImage()
        Me.Reag270 = New BSRImage()
        Me.Reag271 = New BSRImage()
        Me.Reag272 = New BSRImage()
        Me.Reag273 = New BSRImage()
        Me.Reag274 = New BSRImage()
        Me.Reag275 = New BSRImage()
        Me.Reag276 = New BSRImage()
        Me.Reag277 = New BSRImage()
        Me.Reag278 = New BSRImage()
        Me.Reag279 = New BSRImage()
        Me.Reag280 = New BSRImage()
        Me.Reag281 = New BSRImage()
        Me.Reag282 = New BSRImage()
        Me.Reag283 = New BSRImage()
        Me.Reag284 = New BSRImage()
        Me.Reag285 = New BSRImage()
        Me.Reag286 = New BSRImage()
        Me.Reag287 = New BSRImage()
        Me.Reag288 = New BSRImage()
        Me.BsGroupBox1 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsRefresh = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsLabel3 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsLabel2 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsRotationAngle = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsLeft = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsRotate2 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsTop1 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsRotate1 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsLeft2 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsTop2 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsLeft1 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsTop = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsElementsTreeView = New Biosystems.Ax00.Controls.UserControls.BSTreeView()
        Me.bsRequiredElementsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FunctionalityArea.SuspendLayout()
        CType(Me.RotorsTabs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RotorsTabs.SuspendLayout()
        Me.SamplesTab.SuspendLayout()
        CType(Me.Sam3127, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3128, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3129, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3130, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3131, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3132, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3133, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3135, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl1.SuspendLayout()
        CType(Me.PanelControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl2.SuspendLayout()
        Me.bsSamplesLegendGroupBox.SuspendLayout()
        CType(Me.bsTubeAddSolPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsDilutedPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsRoutinePictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsStatPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsControlPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsCalibratorPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsSamplesPositionInfoGroupBox.SuspendLayout()
        CType(Me.Sam3106, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam395, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam396, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam397, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3107, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3134, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3126, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3125, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3124, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3123, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3122, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3121, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3120, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3119, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3118, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3117, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3116, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3115, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3114, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3113, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3112, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3111, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3110, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3109, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3108, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3105, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3104, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3103, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3102, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3101, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam3100, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam399, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam398, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam394, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam393, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam392, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam391, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam290, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam289, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam288, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam287, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam286, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam285, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam284, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam283, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam282, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam281, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam280, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam279, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam278, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam277, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam276, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam275, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam274, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam273, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam272, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam271, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam270, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam269, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam268, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam267, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam266, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam265, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam264, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam263, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam262, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam261, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam260, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam259, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam258, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam257, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam256, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam255, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam254, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam253, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam252, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam251, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam250, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam249, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam248, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam247, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam246, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam145, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam144, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam143, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam142, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam11, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam141, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam140, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam139, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam138, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam137, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam136, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam135, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam134, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam133, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam132, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam131, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam130, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam129, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam128, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam127, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam126, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam125, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam124, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam123, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam122, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam121, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam120, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam119, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam118, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam117, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam116, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam115, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam114, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam113, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam112, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam111, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam110, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam19, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam18, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam17, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam16, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam15, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam14, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam13, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Sam12, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ReagentsTab.SuspendLayout()
        CType(Me.PanelControl6, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl6.SuspendLayout()
        CType(Me.PanelControl7, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl7.SuspendLayout()
        Me.bsReagentsLegendGroupBox.SuspendLayout()
        CType(Me.SelectedPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LegendUnknownImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LegendBarCodeErrorRGImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LowVolPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ReagentPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AdditionalSolPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NoInUsePictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsDepletedPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsReagentsPositionInfoGroupBox.SuspendLayout()
        CType(Me.Reag11, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag12, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag13, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag14, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag15, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag16, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag17, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag18, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag19, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag110, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag111, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag112, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag113, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag114, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag115, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag116, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag117, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag118, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag119, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag120, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag121, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag122, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag123, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag124, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag125, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag126, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag127, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag128, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag129, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag130, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag131, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag132, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag133, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag134, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag135, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag136, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag137, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag138, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag139, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag140, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag141, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag142, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag143, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag144, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag245, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag246, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag247, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag248, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag249, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag250, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag251, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag252, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag253, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag254, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag255, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag256, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag257, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag258, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag259, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag260, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag261, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag262, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag263, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag264, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag265, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag266, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag267, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag268, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag269, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag270, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag271, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag272, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag273, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag274, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag275, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag276, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag277, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag278, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag279, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag280, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag281, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag282, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag283, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag284, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag285, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag286, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag287, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Reag288, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsGroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsScreenTimer
        '
        Me.bsScreenTimer.Interval = 50
        '
        'bsErrorProvider1
        '
        Me.bsErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsErrorProvider1.ContainerControl = Me
        '
        'FunctionalityArea
        '
        Me.FunctionalityArea.Controls.Add(Me.BarcodeWarningButton)
        Me.FunctionalityArea.Controls.Add(Me.bsAcceptButton)
        Me.FunctionalityArea.Controls.Add(Me.bsScanningButton)
        Me.FunctionalityArea.Controls.Add(Me.bsCheckRotorVolumeButton)
        Me.FunctionalityArea.Controls.Add(Me.bsWarningsButton)
        Me.FunctionalityArea.Controls.Add(Me.bsPrintButton)
        Me.FunctionalityArea.Controls.Add(Me.bsSaveVRotorButton)
        Me.FunctionalityArea.Controls.Add(Me.bsLoadVRotorButton)
        Me.FunctionalityArea.Controls.Add(Me.bsResetRotorButton)
        Me.FunctionalityArea.Controls.Add(Me.bsReagentAutoPosButton)
        Me.FunctionalityArea.Controls.Add(Me.bsSamplesAutoPosButton)
        Me.FunctionalityArea.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.FunctionalityArea.Location = New System.Drawing.Point(0, 616)
        Me.FunctionalityArea.Name = "FunctionalityArea"
        Me.FunctionalityArea.Size = New System.Drawing.Size(978, 38)
        Me.FunctionalityArea.TabIndex = 31
        '
        'BarcodeWarningButton
        '
        Me.BarcodeWarningButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BarcodeWarningButton.Location = New System.Drawing.Point(525, 4)
        Me.BarcodeWarningButton.Name = "BarcodeWarningButton"
        Me.BarcodeWarningButton.Size = New System.Drawing.Size(32, 32)
        Me.BarcodeWarningButton.TabIndex = 14
        Me.BarcodeWarningButton.UseVisualStyleBackColor = True
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAcceptButton.ForeColor = System.Drawing.Color.Green
        Me.bsAcceptButton.Location = New System.Drawing.Point(944, 4)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 9
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'bsScanningButton
        '
        Me.bsScanningButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsScanningButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsScanningButton.Location = New System.Drawing.Point(459, 4)
        Me.bsScanningButton.Name = "bsScanningButton"
        Me.bsScanningButton.Size = New System.Drawing.Size(32, 32)
        Me.bsScanningButton.TabIndex = 2
        Me.bsScanningButton.UseVisualStyleBackColor = True
        '
        'bsCheckRotorVolumeButton
        '
        Me.bsCheckRotorVolumeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsCheckRotorVolumeButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCheckRotorVolumeButton.Location = New System.Drawing.Point(644, 4)
        Me.bsCheckRotorVolumeButton.Name = "bsCheckRotorVolumeButton"
        Me.bsCheckRotorVolumeButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCheckRotorVolumeButton.TabIndex = 5
        Me.bsCheckRotorVolumeButton.UseVisualStyleBackColor = True
        Me.bsCheckRotorVolumeButton.Visible = False
        '
        'bsWarningsButton
        '
        Me.bsWarningsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsWarningsButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsWarningsButton.Location = New System.Drawing.Point(559, 4)
        Me.bsWarningsButton.Name = "bsWarningsButton"
        Me.bsWarningsButton.Size = New System.Drawing.Size(32, 32)
        Me.bsWarningsButton.TabIndex = 2
        Me.bsWarningsButton.UseVisualStyleBackColor = True
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsPrintButton.Location = New System.Drawing.Point(593, 4)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 3
        Me.bsPrintButton.UseVisualStyleBackColor = True
        Me.bsPrintButton.Visible = False
        '
        'bsSaveVRotorButton
        '
        Me.bsSaveVRotorButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSaveVRotorButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSaveVRotorButton.Location = New System.Drawing.Point(695, 4)
        Me.bsSaveVRotorButton.Name = "bsSaveVRotorButton"
        Me.bsSaveVRotorButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveVRotorButton.TabIndex = 6
        Me.bsSaveVRotorButton.UseVisualStyleBackColor = True
        '
        'bsLoadVRotorButton
        '
        Me.bsLoadVRotorButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsLoadVRotorButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLoadVRotorButton.Location = New System.Drawing.Point(728, 4)
        Me.bsLoadVRotorButton.Name = "bsLoadVRotorButton"
        Me.bsLoadVRotorButton.Size = New System.Drawing.Size(32, 32)
        Me.bsLoadVRotorButton.TabIndex = 7
        Me.bsLoadVRotorButton.UseVisualStyleBackColor = True
        '
        'bsResetRotorButton
        '
        Me.bsResetRotorButton.BackColor = System.Drawing.Color.Transparent
        Me.bsResetRotorButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsResetRotorButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsResetRotorButton.Location = New System.Drawing.Point(761, 4)
        Me.bsResetRotorButton.Name = "bsResetRotorButton"
        Me.bsResetRotorButton.Size = New System.Drawing.Size(32, 32)
        Me.bsResetRotorButton.TabIndex = 8
        Me.bsResetRotorButton.UseVisualStyleBackColor = False
        '
        'bsReagentAutoPosButton
        '
        Me.bsReagentAutoPosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReagentAutoPosButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReagentAutoPosButton.Location = New System.Drawing.Point(37, 4)
        Me.bsReagentAutoPosButton.Name = "bsReagentAutoPosButton"
        Me.bsReagentAutoPosButton.Size = New System.Drawing.Size(32, 32)
        Me.bsReagentAutoPosButton.TabIndex = 1
        Me.bsReagentAutoPosButton.UseVisualStyleBackColor = True
        '
        'bsSamplesAutoPosButton
        '
        Me.bsSamplesAutoPosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSamplesAutoPosButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSamplesAutoPosButton.Location = New System.Drawing.Point(3, 4)
        Me.bsSamplesAutoPosButton.Name = "bsSamplesAutoPosButton"
        Me.bsSamplesAutoPosButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSamplesAutoPosButton.TabIndex = 0
        Me.bsSamplesAutoPosButton.UseVisualStyleBackColor = True
        '
        'RotorsTabs
        '
        Me.RotorsTabs.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
        Me.RotorsTabs.Appearance.Options.UseBackColor = True
        Me.RotorsTabs.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.RotorsTabs.Location = New System.Drawing.Point(208, 1)
        Me.RotorsTabs.LookAndFeel.UseDefaultLookAndFeel = False
        Me.RotorsTabs.LookAndFeel.UseWindowsXPTheme = True
        Me.RotorsTabs.Name = "RotorsTabs"
        Me.RotorsTabs.SelectedTabPage = Me.SamplesTab
        Me.RotorsTabs.Size = New System.Drawing.Size(770, 617)
        Me.RotorsTabs.TabIndex = 32
        Me.RotorsTabs.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.SamplesTab, Me.ReagentsTab})
        '
        'SamplesTab
        '
        Me.SamplesTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.SamplesTab.Appearance.PageClient.Image = CType(resources.GetObject("SamplesTab.Appearance.PageClient.Image"), System.Drawing.Image)
        Me.SamplesTab.Appearance.PageClient.Options.UseBackColor = True
        Me.SamplesTab.Appearance.PageClient.Options.UseImage = True
        Me.SamplesTab.Controls.Add(Me.Sam3127)
        Me.SamplesTab.Controls.Add(Me.Sam3128)
        Me.SamplesTab.Controls.Add(Me.Sam3129)
        Me.SamplesTab.Controls.Add(Me.Sam3130)
        Me.SamplesTab.Controls.Add(Me.Sam3131)
        Me.SamplesTab.Controls.Add(Me.Sam3132)
        Me.SamplesTab.Controls.Add(Me.Sam3133)
        Me.SamplesTab.Controls.Add(Me.Sam3135)
        Me.SamplesTab.Controls.Add(Me.PanelControl1)
        Me.SamplesTab.Controls.Add(Me.Sam3106)
        Me.SamplesTab.Controls.Add(Me.Sam395)
        Me.SamplesTab.Controls.Add(Me.Sam396)
        Me.SamplesTab.Controls.Add(Me.Sam397)
        Me.SamplesTab.Controls.Add(Me.Sam3107)
        Me.SamplesTab.Controls.Add(Me.Sam3134)
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
        Me.SamplesTab.Name = "SamplesTab"
        Me.SamplesTab.Padding = New System.Windows.Forms.Padding(3)
        Me.SamplesTab.Size = New System.Drawing.Size(762, 588)
        Me.SamplesTab.Text = "Samples"
        '
        'Sam3127
        '
        Me.Sam3127.BackColor = System.Drawing.Color.Transparent
        Me.Sam3127.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3127.Image = Nothing
        Me.Sam3127.ImagePath = ""
        Me.Sam3127.IsTransparentImage = True
        Me.Sam3127.Location = New System.Drawing.Point(97, 362)
        Me.Sam3127.Name = "Sam3127"
        Me.Sam3127.Rotation = 0
        Me.Sam3127.ShowThrough = False
        Me.Sam3127.Size = New System.Drawing.Size(24, 23)
        Me.Sam3127.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3127.TabIndex = 310
        Me.Sam3127.TabStop = False
        Me.Sam3127.TransparentColor = System.Drawing.Color.Transparent
        '
        'Sam3128
        '
        Me.Sam3128.BackColor = System.Drawing.Color.Transparent
        Me.Sam3128.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3128.Image = Nothing
        Me.Sam3128.ImagePath = ""
        Me.Sam3128.IsTransparentImage = True
        Me.Sam3128.Location = New System.Drawing.Point(109, 387)
        Me.Sam3128.Name = "Sam3128"
        Me.Sam3128.Rotation = 0
        Me.Sam3128.ShowThrough = False
        Me.Sam3128.Size = New System.Drawing.Size(24, 23)
        Me.Sam3128.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3128.TabIndex = 311
        Me.Sam3128.TabStop = False
        Me.Sam3128.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3129
        '
        Me.Sam3129.BackColor = System.Drawing.Color.Transparent
        Me.Sam3129.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3129.Image = Nothing
        Me.Sam3129.ImagePath = ""
        Me.Sam3129.IsTransparentImage = True
        Me.Sam3129.Location = New System.Drawing.Point(125, 410)
        Me.Sam3129.Name = "Sam3129"
        Me.Sam3129.Rotation = 0
        Me.Sam3129.ShowThrough = False
        Me.Sam3129.Size = New System.Drawing.Size(24, 23)
        Me.Sam3129.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3129.TabIndex = 312
        Me.Sam3129.TabStop = False
        Me.Sam3129.TransparentColor = System.Drawing.Color.Gainsboro
        '
        'Sam3130
        '
        Me.Sam3130.BackColor = System.Drawing.Color.Transparent
        Me.Sam3130.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3130.Image = Nothing
        Me.Sam3130.ImagePath = ""
        Me.Sam3130.IsTransparentImage = False
        Me.Sam3130.Location = New System.Drawing.Point(144, 430)
        Me.Sam3130.Name = "Sam3130"
        Me.Sam3130.Rotation = 0
        Me.Sam3130.ShowThrough = False
        Me.Sam3130.Size = New System.Drawing.Size(24, 23)
        Me.Sam3130.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3130.TabIndex = 313
        Me.Sam3130.TabStop = False
        Me.Sam3130.TransparentColor = System.Drawing.Color.Gainsboro
        '
        'Sam3131
        '
        Me.Sam3131.BackColor = System.Drawing.Color.Transparent
        Me.Sam3131.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3131.Image = Nothing
        Me.Sam3131.ImagePath = ""
        Me.Sam3131.IsTransparentImage = False
        Me.Sam3131.Location = New System.Drawing.Point(165, 448)
        Me.Sam3131.Name = "Sam3131"
        Me.Sam3131.Rotation = 0
        Me.Sam3131.ShowThrough = False
        Me.Sam3131.Size = New System.Drawing.Size(24, 23)
        Me.Sam3131.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3131.TabIndex = 314
        Me.Sam3131.TabStop = False
        Me.Sam3131.TransparentColor = System.Drawing.Color.Gainsboro
        '
        'Sam3132
        '
        Me.Sam3132.BackColor = System.Drawing.Color.Transparent
        Me.Sam3132.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3132.Image = Nothing
        Me.Sam3132.ImagePath = ""
        Me.Sam3132.IsTransparentImage = False
        Me.Sam3132.Location = New System.Drawing.Point(189, 462)
        Me.Sam3132.Name = "Sam3132"
        Me.Sam3132.Rotation = 0
        Me.Sam3132.ShowThrough = False
        Me.Sam3132.Size = New System.Drawing.Size(24, 23)
        Me.Sam3132.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3132.TabIndex = 315
        Me.Sam3132.TabStop = False
        Me.Sam3132.TransparentColor = System.Drawing.Color.Gainsboro
        '
        'Sam3133
        '
        Me.Sam3133.BackColor = System.Drawing.Color.Transparent
        Me.Sam3133.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3133.Image = Nothing
        Me.Sam3133.ImagePath = ""
        Me.Sam3133.IsTransparentImage = True
        Me.Sam3133.Location = New System.Drawing.Point(214, 472)
        Me.Sam3133.Name = "Sam3133"
        Me.Sam3133.Rotation = 0
        Me.Sam3133.ShowThrough = False
        Me.Sam3133.Size = New System.Drawing.Size(24, 24)
        Me.Sam3133.TabIndex = 316
        Me.Sam3133.TabStop = False
        Me.Sam3133.TransparentColor = System.Drawing.Color.Gainsboro
        '
        'Sam3135
        '
        Me.Sam3135.BackColor = System.Drawing.Color.Transparent
        Me.Sam3135.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3135.Image = Nothing
        Me.Sam3135.ImagePath = ""
        Me.Sam3135.IsTransparentImage = True
        Me.Sam3135.Location = New System.Drawing.Point(268, 483)
        Me.Sam3135.Name = "Sam3135"
        Me.Sam3135.Rotation = 0
        Me.Sam3135.ShowThrough = False
        Me.Sam3135.Size = New System.Drawing.Size(24, 24)
        Me.Sam3135.TabIndex = 282
        Me.Sam3135.TabStop = False
        Me.Sam3135.TransparentColor = System.Drawing.Color.White
        '
        'PanelControl1
        '
        Me.PanelControl1.Appearance.BackColor = System.Drawing.Color.White
        Me.PanelControl1.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl1.Appearance.Options.UseBackColor = True
        Me.PanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl1.Controls.Add(Me.PanelControl2)
        Me.PanelControl1.Location = New System.Drawing.Point(575, -4)
        Me.PanelControl1.Name = "PanelControl1"
        Me.PanelControl1.Size = New System.Drawing.Size(190, 598)
        Me.PanelControl1.TabIndex = 320
        '
        'PanelControl2
        '
        Me.PanelControl2.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.PanelControl2.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl2.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.PanelControl2.Appearance.Options.UseBackColor = True
        Me.PanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.PanelControl2.Controls.Add(Me.bsSamplesLegendGroupBox)
        Me.PanelControl2.Controls.Add(Me.bsSamplesPositionInfoGroupBox)
        Me.PanelControl2.Location = New System.Drawing.Point(3, 4)
        Me.PanelControl2.LookAndFeel.UseDefaultLookAndFeel = False
        Me.PanelControl2.Name = "PanelControl2"
        Me.PanelControl2.Size = New System.Drawing.Size(183, 588)
        Me.PanelControl2.TabIndex = 262
        '
        'bsSamplesLegendGroupBox
        '
        Me.bsSamplesLegendGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsTubeAddSolLabel)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsTubeAddSolPictureBox)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsLegendDilutedLabel)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsDilutedPictureBox)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsLegendRoutineLabel)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsLegendStatLabel)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsLegendControlsLabel)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsLegendCalibratorsLabel)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsRoutinePictureBox)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsStatPictureBox)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsControlPictureBox)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsCalibratorPictureBox)
        Me.bsSamplesLegendGroupBox.Controls.Add(Me.bsSamplesLegendLabel)
        Me.bsSamplesLegendGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSamplesLegendGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesLegendGroupBox.Location = New System.Drawing.Point(4, 370)
        Me.bsSamplesLegendGroupBox.Name = "bsSamplesLegendGroupBox"
        Me.bsSamplesLegendGroupBox.Size = New System.Drawing.Size(175, 210)
        Me.bsSamplesLegendGroupBox.TabIndex = 26
        Me.bsSamplesLegendGroupBox.TabStop = False
        '
        'bsTubeAddSolLabel
        '
        Me.bsTubeAddSolLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTubeAddSolLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTubeAddSolLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTubeAddSolLabel.Location = New System.Drawing.Point(23, 167)
        Me.bsTubeAddSolLabel.Name = "bsTubeAddSolLabel"
        Me.bsTubeAddSolLabel.Size = New System.Drawing.Size(151, 13)
        Me.bsTubeAddSolLabel.TabIndex = 35
        Me.bsTubeAddSolLabel.Text = "*Additional Solution"
        Me.bsTubeAddSolLabel.Title = False
        '
        'bsTubeAddSolPictureBox
        '
        Me.bsTubeAddSolPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsTubeAddSolPictureBox.InitialImage = CType(resources.GetObject("bsTubeAddSolPictureBox.InitialImage"), System.Drawing.Image)
        Me.bsTubeAddSolPictureBox.Location = New System.Drawing.Point(2, 164)
        Me.bsTubeAddSolPictureBox.Name = "bsTubeAddSolPictureBox"
        Me.bsTubeAddSolPictureBox.PositionNumber = 0
        Me.bsTubeAddSolPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsTubeAddSolPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsTubeAddSolPictureBox.TabIndex = 34
        Me.bsTubeAddSolPictureBox.TabStop = False
        '
        'bsLegendDilutedLabel
        '
        Me.bsLegendDilutedLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLegendDilutedLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLegendDilutedLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLegendDilutedLabel.Location = New System.Drawing.Point(23, 142)
        Me.bsLegendDilutedLabel.Name = "bsLegendDilutedLabel"
        Me.bsLegendDilutedLabel.Size = New System.Drawing.Size(151, 13)
        Me.bsLegendDilutedLabel.TabIndex = 33
        Me.bsLegendDilutedLabel.Text = "*Diluted Sample"
        Me.bsLegendDilutedLabel.Title = False
        '
        'bsDilutedPictureBox
        '
        Me.bsDilutedPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsDilutedPictureBox.InitialImage = CType(resources.GetObject("bsDilutedPictureBox.InitialImage"), System.Drawing.Image)
        Me.bsDilutedPictureBox.Location = New System.Drawing.Point(2, 139)
        Me.bsDilutedPictureBox.Name = "bsDilutedPictureBox"
        Me.bsDilutedPictureBox.PositionNumber = 0
        Me.bsDilutedPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsDilutedPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsDilutedPictureBox.TabIndex = 32
        Me.bsDilutedPictureBox.TabStop = False
        '
        'bsLegendRoutineLabel
        '
        Me.bsLegendRoutineLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLegendRoutineLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLegendRoutineLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLegendRoutineLabel.Location = New System.Drawing.Point(23, 117)
        Me.bsLegendRoutineLabel.Name = "bsLegendRoutineLabel"
        Me.bsLegendRoutineLabel.Size = New System.Drawing.Size(151, 13)
        Me.bsLegendRoutineLabel.TabIndex = 31
        Me.bsLegendRoutineLabel.Text = "*Routine"
        Me.bsLegendRoutineLabel.Title = False
        '
        'bsLegendStatLabel
        '
        Me.bsLegendStatLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLegendStatLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLegendStatLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLegendStatLabel.Location = New System.Drawing.Point(23, 92)
        Me.bsLegendStatLabel.Name = "bsLegendStatLabel"
        Me.bsLegendStatLabel.Size = New System.Drawing.Size(151, 13)
        Me.bsLegendStatLabel.TabIndex = 30
        Me.bsLegendStatLabel.Text = "*Stat"
        Me.bsLegendStatLabel.Title = False
        '
        'bsLegendControlsLabel
        '
        Me.bsLegendControlsLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLegendControlsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLegendControlsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLegendControlsLabel.Location = New System.Drawing.Point(23, 67)
        Me.bsLegendControlsLabel.Name = "bsLegendControlsLabel"
        Me.bsLegendControlsLabel.Size = New System.Drawing.Size(151, 13)
        Me.bsLegendControlsLabel.TabIndex = 29
        Me.bsLegendControlsLabel.Text = "*Control"
        Me.bsLegendControlsLabel.Title = False
        '
        'bsLegendCalibratorsLabel
        '
        Me.bsLegendCalibratorsLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLegendCalibratorsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLegendCalibratorsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLegendCalibratorsLabel.Location = New System.Drawing.Point(23, 42)
        Me.bsLegendCalibratorsLabel.Name = "bsLegendCalibratorsLabel"
        Me.bsLegendCalibratorsLabel.Size = New System.Drawing.Size(151, 13)
        Me.bsLegendCalibratorsLabel.TabIndex = 26
        Me.bsLegendCalibratorsLabel.Text = "*Calibrator"
        Me.bsLegendCalibratorsLabel.Title = False
        '
        'bsRoutinePictureBox
        '
        Me.bsRoutinePictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsRoutinePictureBox.InitialImage = CType(resources.GetObject("bsRoutinePictureBox.InitialImage"), System.Drawing.Image)
        Me.bsRoutinePictureBox.Location = New System.Drawing.Point(2, 114)
        Me.bsRoutinePictureBox.Name = "bsRoutinePictureBox"
        Me.bsRoutinePictureBox.PositionNumber = 0
        Me.bsRoutinePictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsRoutinePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsRoutinePictureBox.TabIndex = 27
        Me.bsRoutinePictureBox.TabStop = False
        '
        'bsStatPictureBox
        '
        Me.bsStatPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsStatPictureBox.InitialImage = CType(resources.GetObject("bsStatPictureBox.InitialImage"), System.Drawing.Image)
        Me.bsStatPictureBox.Location = New System.Drawing.Point(2, 89)
        Me.bsStatPictureBox.Name = "bsStatPictureBox"
        Me.bsStatPictureBox.PositionNumber = 0
        Me.bsStatPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsStatPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsStatPictureBox.TabIndex = 28
        Me.bsStatPictureBox.TabStop = False
        '
        'bsControlPictureBox
        '
        Me.bsControlPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsControlPictureBox.InitialImage = CType(resources.GetObject("bsControlPictureBox.InitialImage"), System.Drawing.Image)
        Me.bsControlPictureBox.Location = New System.Drawing.Point(2, 64)
        Me.bsControlPictureBox.Name = "bsControlPictureBox"
        Me.bsControlPictureBox.PositionNumber = 0
        Me.bsControlPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsControlPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsControlPictureBox.TabIndex = 27
        Me.bsControlPictureBox.TabStop = False
        '
        'bsCalibratorPictureBox
        '
        Me.bsCalibratorPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCalibratorPictureBox.InitialImage = CType(resources.GetObject("bsCalibratorPictureBox.InitialImage"), System.Drawing.Image)
        Me.bsCalibratorPictureBox.Location = New System.Drawing.Point(2, 39)
        Me.bsCalibratorPictureBox.Name = "bsCalibratorPictureBox"
        Me.bsCalibratorPictureBox.PositionNumber = 0
        Me.bsCalibratorPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsCalibratorPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsCalibratorPictureBox.TabIndex = 26
        Me.bsCalibratorPictureBox.TabStop = False
        '
        'bsSamplesLegendLabel
        '
        Me.bsSamplesLegendLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSamplesLegendLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsSamplesLegendLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesLegendLabel.Location = New System.Drawing.Point(4, 12)
        Me.bsSamplesLegendLabel.Name = "bsSamplesLegendLabel"
        Me.bsSamplesLegendLabel.Size = New System.Drawing.Size(167, 19)
        Me.bsSamplesLegendLabel.TabIndex = 25
        Me.bsSamplesLegendLabel.Text = "Legend"
        Me.bsSamplesLegendLabel.Title = True
        '
        'bsSamplesPositionInfoGroupBox
        '
        Me.bsSamplesPositionInfoGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.SamplesStatusTextBox)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesStatusLabel)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesPositionInfoLabel)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesDeletePosButton)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesMoveLastPositionButton)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesRefillPosButton)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesIncreaseButton)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsTubeSizeComboBox)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesBarcodeTextBox)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesDecreaseButton)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsDiluteStatusTextBox)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSampleTypeTextBox)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesMoveFirstPositionButton)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSampleNumberTextBox)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSampleIDTextBox)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSampleContentTextBox)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSampleRingNumTextBox)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSampleCellTextBox)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsTubeSizeLabel)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesBarcodeLabel)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsDiluteStatusLabel)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSampleTypeLabel)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesNumberLabel)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSampleIDLabel)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesContentLabel)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesRingNumLabel)
        Me.bsSamplesPositionInfoGroupBox.Controls.Add(Me.bsSamplesCellLabel)
        Me.bsSamplesPositionInfoGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSamplesPositionInfoGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesPositionInfoGroupBox.Location = New System.Drawing.Point(4, -3)
        Me.bsSamplesPositionInfoGroupBox.Name = "bsSamplesPositionInfoGroupBox"
        Me.bsSamplesPositionInfoGroupBox.Size = New System.Drawing.Size(175, 370)
        Me.bsSamplesPositionInfoGroupBox.TabIndex = 2
        Me.bsSamplesPositionInfoGroupBox.TabStop = False
        '
        'SamplesStatusTextBox
        '
        Me.SamplesStatusTextBox.BackColor = System.Drawing.Color.White
        Me.SamplesStatusTextBox.DecimalsValues = False
        Me.SamplesStatusTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.SamplesStatusTextBox.ForeColor = System.Drawing.Color.Black
        Me.SamplesStatusTextBox.IsNumeric = False
        Me.SamplesStatusTextBox.Location = New System.Drawing.Point(5, 276)
        Me.SamplesStatusTextBox.Mandatory = False
        Me.SamplesStatusTextBox.Name = "SamplesStatusTextBox"
        Me.SamplesStatusTextBox.ReadOnly = True
        Me.SamplesStatusTextBox.Size = New System.Drawing.Size(164, 21)
        Me.SamplesStatusTextBox.TabIndex = 27
        Me.SamplesStatusTextBox.WordWrap = False
        '
        'bsSamplesStatusLabel
        '
        Me.bsSamplesStatusLabel.AutoSize = True
        Me.bsSamplesStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesStatusLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesStatusLabel.Location = New System.Drawing.Point(2, 262)
        Me.bsSamplesStatusLabel.Name = "bsSamplesStatusLabel"
        Me.bsSamplesStatusLabel.Size = New System.Drawing.Size(55, 13)
        Me.bsSamplesStatusLabel.TabIndex = 26
        Me.bsSamplesStatusLabel.Text = "*Status:"
        Me.bsSamplesStatusLabel.Title = False
        '
        'bsSamplesPositionInfoLabel
        '
        Me.bsSamplesPositionInfoLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSamplesPositionInfoLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsSamplesPositionInfoLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesPositionInfoLabel.Location = New System.Drawing.Point(4, 12)
        Me.bsSamplesPositionInfoLabel.Name = "bsSamplesPositionInfoLabel"
        Me.bsSamplesPositionInfoLabel.Size = New System.Drawing.Size(167, 19)
        Me.bsSamplesPositionInfoLabel.TabIndex = 25
        Me.bsSamplesPositionInfoLabel.Text = "Position Information"
        Me.bsSamplesPositionInfoLabel.Title = True
        '
        'bsSamplesDeletePosButton
        '
        Me.bsSamplesDeletePosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSamplesDeletePosButton.Location = New System.Drawing.Point(136, 335)
        Me.bsSamplesDeletePosButton.Name = "bsSamplesDeletePosButton"
        Me.bsSamplesDeletePosButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSamplesDeletePosButton.TabIndex = 11
        Me.bsSamplesDeletePosButton.UseVisualStyleBackColor = True
        '
        'bsSamplesMoveLastPositionButton
        '
        Me.bsSamplesMoveLastPositionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSamplesMoveLastPositionButton.Enabled = False
        Me.bsSamplesMoveLastPositionButton.Font = New System.Drawing.Font("Verdana", 6.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSamplesMoveLastPositionButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsSamplesMoveLastPositionButton.Location = New System.Drawing.Point(80, 301)
        Me.bsSamplesMoveLastPositionButton.Name = "bsSamplesMoveLastPositionButton"
        Me.bsSamplesMoveLastPositionButton.Size = New System.Drawing.Size(26, 26)
        Me.bsSamplesMoveLastPositionButton.TabIndex = 12
        Me.bsSamplesMoveLastPositionButton.UseVisualStyleBackColor = True
        '
        'bsSamplesRefillPosButton
        '
        Me.bsSamplesRefillPosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsSamplesRefillPosButton.Location = New System.Drawing.Point(6, 335)
        Me.bsSamplesRefillPosButton.Name = "bsSamplesRefillPosButton"
        Me.bsSamplesRefillPosButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSamplesRefillPosButton.TabIndex = 9
        Me.bsSamplesRefillPosButton.UseVisualStyleBackColor = True
        '
        'bsSamplesIncreaseButton
        '
        Me.bsSamplesIncreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSamplesIncreaseButton.Enabled = False
        Me.bsSamplesIncreaseButton.Font = New System.Drawing.Font("Verdana", 6.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSamplesIncreaseButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsSamplesIncreaseButton.Location = New System.Drawing.Point(55, 301)
        Me.bsSamplesIncreaseButton.Name = "bsSamplesIncreaseButton"
        Me.bsSamplesIncreaseButton.Size = New System.Drawing.Size(26, 26)
        Me.bsSamplesIncreaseButton.TabIndex = 11
        Me.bsSamplesIncreaseButton.UseVisualStyleBackColor = True
        '
        'bsTubeSizeComboBox
        '
        Me.bsTubeSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsTubeSizeComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsTubeSizeComboBox.FormattingEnabled = True
        Me.bsTubeSizeComboBox.Location = New System.Drawing.Point(5, 237)
        Me.bsTubeSizeComboBox.MaxLength = 25
        Me.bsTubeSizeComboBox.Name = "bsTubeSizeComboBox"
        Me.bsTubeSizeComboBox.Size = New System.Drawing.Size(164, 21)
        Me.bsTubeSizeComboBox.TabIndex = 8
        '
        'bsSamplesBarcodeTextBox
        '
        Me.bsSamplesBarcodeTextBox.BackColor = System.Drawing.Color.White
        Me.bsSamplesBarcodeTextBox.DecimalsValues = False
        Me.bsSamplesBarcodeTextBox.Enabled = False
        Me.bsSamplesBarcodeTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesBarcodeTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesBarcodeTextBox.IsNumeric = False
        Me.bsSamplesBarcodeTextBox.Location = New System.Drawing.Point(5, 199)
        Me.bsSamplesBarcodeTextBox.Mandatory = False
        Me.bsSamplesBarcodeTextBox.Name = "bsSamplesBarcodeTextBox"
        Me.bsSamplesBarcodeTextBox.ReadOnly = True
        Me.bsSamplesBarcodeTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsSamplesBarcodeTextBox.TabIndex = 7
        Me.bsSamplesBarcodeTextBox.WordWrap = False
        '
        'bsSamplesDecreaseButton
        '
        Me.bsSamplesDecreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSamplesDecreaseButton.Enabled = False
        Me.bsSamplesDecreaseButton.Font = New System.Drawing.Font("Verdana", 6.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSamplesDecreaseButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsSamplesDecreaseButton.Location = New System.Drawing.Point(30, 301)
        Me.bsSamplesDecreaseButton.Name = "bsSamplesDecreaseButton"
        Me.bsSamplesDecreaseButton.Size = New System.Drawing.Size(26, 26)
        Me.bsSamplesDecreaseButton.TabIndex = 10
        Me.bsSamplesDecreaseButton.UseVisualStyleBackColor = True
        '
        'bsDiluteStatusTextBox
        '
        Me.bsDiluteStatusTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsDiluteStatusTextBox.BackColor = System.Drawing.Color.White
        Me.bsDiluteStatusTextBox.DecimalsValues = False
        Me.bsDiluteStatusTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDiluteStatusTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsDiluteStatusTextBox.IsNumeric = False
        Me.bsDiluteStatusTextBox.Location = New System.Drawing.Point(123, 161)
        Me.bsDiluteStatusTextBox.Mandatory = False
        Me.bsDiluteStatusTextBox.Name = "bsDiluteStatusTextBox"
        Me.bsDiluteStatusTextBox.ReadOnly = True
        Me.bsDiluteStatusTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsDiluteStatusTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsDiluteStatusTextBox.TabIndex = 6
        Me.bsDiluteStatusTextBox.TabStop = False
        Me.bsDiluteStatusTextBox.WordWrap = False
        '
        'bsSampleTypeTextBox
        '
        Me.bsSampleTypeTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleTypeTextBox.DecimalsValues = False
        Me.bsSampleTypeTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypeTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeTextBox.IsNumeric = False
        Me.bsSampleTypeTextBox.Location = New System.Drawing.Point(5, 161)
        Me.bsSampleTypeTextBox.Mandatory = False
        Me.bsSampleTypeTextBox.MaxLength = 20
        Me.bsSampleTypeTextBox.Name = "bsSampleTypeTextBox"
        Me.bsSampleTypeTextBox.ReadOnly = True
        Me.bsSampleTypeTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsSampleTypeTextBox.TabIndex = 5
        Me.bsSampleTypeTextBox.TabStop = False
        Me.bsSampleTypeTextBox.WordWrap = False
        '
        'bsSamplesMoveFirstPositionButton
        '
        Me.bsSamplesMoveFirstPositionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSamplesMoveFirstPositionButton.Enabled = False
        Me.bsSamplesMoveFirstPositionButton.Font = New System.Drawing.Font("Verdana", 6.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSamplesMoveFirstPositionButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsSamplesMoveFirstPositionButton.Location = New System.Drawing.Point(5, 301)
        Me.bsSamplesMoveFirstPositionButton.Name = "bsSamplesMoveFirstPositionButton"
        Me.bsSamplesMoveFirstPositionButton.Size = New System.Drawing.Size(26, 26)
        Me.bsSamplesMoveFirstPositionButton.TabIndex = 9
        Me.bsSamplesMoveFirstPositionButton.UseVisualStyleBackColor = True
        '
        'bsSampleNumberTextBox
        '
        Me.bsSampleNumberTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsSampleNumberTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleNumberTextBox.DecimalsValues = False
        Me.bsSampleNumberTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleNumberTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleNumberTextBox.IsNumeric = False
        Me.bsSampleNumberTextBox.Location = New System.Drawing.Point(123, 85)
        Me.bsSampleNumberTextBox.Mandatory = False
        Me.bsSampleNumberTextBox.Name = "bsSampleNumberTextBox"
        Me.bsSampleNumberTextBox.ReadOnly = True
        Me.bsSampleNumberTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsSampleNumberTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsSampleNumberTextBox.TabIndex = 3
        Me.bsSampleNumberTextBox.TabStop = False
        Me.bsSampleNumberTextBox.WordWrap = False
        '
        'bsSampleIDTextBox
        '
        Me.bsSampleIDTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleIDTextBox.DecimalsValues = False
        Me.bsSampleIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleIDTextBox.IsNumeric = False
        Me.bsSampleIDTextBox.Location = New System.Drawing.Point(5, 123)
        Me.bsSampleIDTextBox.Mandatory = False
        Me.bsSampleIDTextBox.Name = "bsSampleIDTextBox"
        Me.bsSampleIDTextBox.ReadOnly = True
        Me.bsSampleIDTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsSampleIDTextBox.TabIndex = 4
        Me.bsSampleIDTextBox.TabStop = False
        Me.bsSampleIDTextBox.WordWrap = False
        '
        'bsSampleContentTextBox
        '
        Me.bsSampleContentTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleContentTextBox.DecimalsValues = False
        Me.bsSampleContentTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleContentTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleContentTextBox.IsNumeric = False
        Me.bsSampleContentTextBox.Location = New System.Drawing.Point(5, 85)
        Me.bsSampleContentTextBox.Mandatory = False
        Me.bsSampleContentTextBox.MaxLength = 20
        Me.bsSampleContentTextBox.Name = "bsSampleContentTextBox"
        Me.bsSampleContentTextBox.ReadOnly = True
        Me.bsSampleContentTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsSampleContentTextBox.TabIndex = 2
        Me.bsSampleContentTextBox.TabStop = False
        Me.bsSampleContentTextBox.WordWrap = False
        '
        'bsSampleRingNumTextBox
        '
        Me.bsSampleRingNumTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsSampleRingNumTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleRingNumTextBox.DecimalsValues = False
        Me.bsSampleRingNumTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleRingNumTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleRingNumTextBox.IsNumeric = False
        Me.bsSampleRingNumTextBox.Location = New System.Drawing.Point(5, 47)
        Me.bsSampleRingNumTextBox.Mandatory = False
        Me.bsSampleRingNumTextBox.Name = "bsSampleRingNumTextBox"
        Me.bsSampleRingNumTextBox.ReadOnly = True
        Me.bsSampleRingNumTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsSampleRingNumTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsSampleRingNumTextBox.TabIndex = 0
        Me.bsSampleRingNumTextBox.TabStop = False
        Me.bsSampleRingNumTextBox.WordWrap = False
        '
        'bsSampleCellTextBox
        '
        Me.bsSampleCellTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsSampleCellTextBox.BackColor = System.Drawing.Color.White
        Me.bsSampleCellTextBox.DecimalsValues = False
        Me.bsSampleCellTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleCellTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleCellTextBox.IsNumeric = False
        Me.bsSampleCellTextBox.Location = New System.Drawing.Point(123, 47)
        Me.bsSampleCellTextBox.Mandatory = False
        Me.bsSampleCellTextBox.MaxLength = 3
        Me.bsSampleCellTextBox.Name = "bsSampleCellTextBox"
        Me.bsSampleCellTextBox.ReadOnly = True
        Me.bsSampleCellTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsSampleCellTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsSampleCellTextBox.TabIndex = 1
        Me.bsSampleCellTextBox.TabStop = False
        Me.bsSampleCellTextBox.WordWrap = False
        '
        'bsTubeSizeLabel
        '
        Me.bsTubeSizeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTubeSizeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTubeSizeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTubeSizeLabel.Location = New System.Drawing.Point(2, 223)
        Me.bsTubeSizeLabel.Name = "bsTubeSizeLabel"
        Me.bsTubeSizeLabel.Size = New System.Drawing.Size(164, 13)
        Me.bsTubeSizeLabel.TabIndex = 8
        Me.bsTubeSizeLabel.Text = "*Tube Size:"
        Me.bsTubeSizeLabel.Title = False
        '
        'bsSamplesBarcodeLabel
        '
        Me.bsSamplesBarcodeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesBarcodeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesBarcodeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesBarcodeLabel.Location = New System.Drawing.Point(2, 185)
        Me.bsSamplesBarcodeLabel.Name = "bsSamplesBarcodeLabel"
        Me.bsSamplesBarcodeLabel.Size = New System.Drawing.Size(165, 13)
        Me.bsSamplesBarcodeLabel.TabIndex = 7
        Me.bsSamplesBarcodeLabel.Text = "*Sample Barcode:"
        Me.bsSamplesBarcodeLabel.Title = False
        '
        'bsDiluteStatusLabel
        '
        Me.bsDiluteStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDiluteStatusLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDiluteStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDiluteStatusLabel.Location = New System.Drawing.Point(120, 147)
        Me.bsDiluteStatusLabel.Name = "bsDiluteStatusLabel"
        Me.bsDiluteStatusLabel.Size = New System.Drawing.Size(55, 13)
        Me.bsDiluteStatusLabel.TabIndex = 6
        Me.bsDiluteStatusLabel.Text = "*Dilution:"
        Me.bsDiluteStatusLabel.Title = False
        '
        'bsSampleTypeLabel
        '
        Me.bsSampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeLabel.Location = New System.Drawing.Point(2, 147)
        Me.bsSampleTypeLabel.Name = "bsSampleTypeLabel"
        Me.bsSampleTypeLabel.Size = New System.Drawing.Size(115, 13)
        Me.bsSampleTypeLabel.TabIndex = 5
        Me.bsSampleTypeLabel.Text = "*Sample Type:"
        Me.bsSampleTypeLabel.Title = False
        '
        'bsSamplesNumberLabel
        '
        Me.bsSamplesNumberLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesNumberLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesNumberLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesNumberLabel.Location = New System.Drawing.Point(120, 71)
        Me.bsSamplesNumberLabel.Name = "bsSamplesNumberLabel"
        Me.bsSamplesNumberLabel.Size = New System.Drawing.Size(55, 13)
        Me.bsSamplesNumberLabel.TabIndex = 4
        Me.bsSamplesNumberLabel.Text = "*Num:"
        Me.bsSamplesNumberLabel.Title = False
        '
        'bsSampleIDLabel
        '
        Me.bsSampleIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleIDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleIDLabel.Location = New System.Drawing.Point(2, 109)
        Me.bsSampleIDLabel.Name = "bsSampleIDLabel"
        Me.bsSampleIDLabel.Size = New System.Drawing.Size(172, 13)
        Me.bsSampleIDLabel.TabIndex = 3
        Me.bsSampleIDLabel.Text = "*Sample ID:"
        Me.bsSampleIDLabel.Title = False
        '
        'bsSamplesContentLabel
        '
        Me.bsSamplesContentLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesContentLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesContentLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesContentLabel.Location = New System.Drawing.Point(2, 71)
        Me.bsSamplesContentLabel.Name = "bsSamplesContentLabel"
        Me.bsSamplesContentLabel.Size = New System.Drawing.Size(115, 13)
        Me.bsSamplesContentLabel.TabIndex = 2
        Me.bsSamplesContentLabel.Text = "*Content:"
        Me.bsSamplesContentLabel.Title = False
        '
        'bsSamplesRingNumLabel
        '
        Me.bsSamplesRingNumLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesRingNumLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesRingNumLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesRingNumLabel.Location = New System.Drawing.Point(2, 33)
        Me.bsSamplesRingNumLabel.Name = "bsSamplesRingNumLabel"
        Me.bsSamplesRingNumLabel.Size = New System.Drawing.Size(115, 13)
        Me.bsSamplesRingNumLabel.TabIndex = 1
        Me.bsSamplesRingNumLabel.Text = "*Ring Num:"
        Me.bsSamplesRingNumLabel.Title = False
        '
        'bsSamplesCellLabel
        '
        Me.bsSamplesCellLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSamplesCellLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesCellLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesCellLabel.Location = New System.Drawing.Point(120, 33)
        Me.bsSamplesCellLabel.Name = "bsSamplesCellLabel"
        Me.bsSamplesCellLabel.Size = New System.Drawing.Size(55, 13)
        Me.bsSamplesCellLabel.TabIndex = 0
        Me.bsSamplesCellLabel.Text = "*Cell:"
        Me.bsSamplesCellLabel.Title = False
        '
        'Sam3106
        '
        Me.Sam3106.BackColor = System.Drawing.Color.Transparent
        Me.Sam3106.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3106.Image = Nothing
        Me.Sam3106.ImagePath = ""
        Me.Sam3106.IsTransparentImage = False
        Me.Sam3106.Location = New System.Drawing.Point(438, 169)
        Me.Sam3106.Name = "Sam3106"
        Me.Sam3106.Rotation = 0
        Me.Sam3106.ShowThrough = False
        Me.Sam3106.Size = New System.Drawing.Size(24, 23)
        Me.Sam3106.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3106.TabIndex = 289
        Me.Sam3106.TabStop = False
        Me.Sam3106.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam395
        '
        Me.Sam395.BackColor = System.Drawing.Color.Transparent
        Me.Sam395.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam395.Image = Nothing
        Me.Sam395.ImagePath = ""
        Me.Sam395.IsTransparentImage = True
        Me.Sam395.Location = New System.Drawing.Point(396, 443)
        Me.Sam395.Name = "Sam395"
        Me.Sam395.Rotation = 0
        Me.Sam395.ShowThrough = False
        Me.Sam395.Size = New System.Drawing.Size(24, 24)
        Me.Sam395.TabIndex = 277
        Me.Sam395.TabStop = False
        Me.Sam395.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam396
        '
        Me.Sam396.BackColor = System.Drawing.Color.Transparent
        Me.Sam396.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam396.Image = Nothing
        Me.Sam396.ImagePath = ""
        Me.Sam396.IsTransparentImage = False
        Me.Sam396.Location = New System.Drawing.Point(417, 424)
        Me.Sam396.Name = "Sam396"
        Me.Sam396.Rotation = 0
        Me.Sam396.ShowThrough = False
        Me.Sam396.Size = New System.Drawing.Size(24, 24)
        Me.Sam396.TabIndex = 278
        Me.Sam396.TabStop = False
        Me.Sam396.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam397
        '
        Me.Sam397.BackColor = System.Drawing.Color.Transparent
        Me.Sam397.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam397.Image = Nothing
        Me.Sam397.ImagePath = ""
        Me.Sam397.IsTransparentImage = False
        Me.Sam397.Location = New System.Drawing.Point(435, 403)
        Me.Sam397.Name = "Sam397"
        Me.Sam397.Rotation = 0
        Me.Sam397.ShowThrough = False
        Me.Sam397.Size = New System.Drawing.Size(24, 23)
        Me.Sam397.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam397.TabIndex = 279
        Me.Sam397.TabStop = False
        Me.Sam397.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3107
        '
        Me.Sam3107.BackColor = System.Drawing.Color.Transparent
        Me.Sam3107.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3107.Image = Nothing
        Me.Sam3107.ImagePath = ""
        Me.Sam3107.IsTransparentImage = True
        Me.Sam3107.Location = New System.Drawing.Point(421, 147)
        Me.Sam3107.Name = "Sam3107"
        Me.Sam3107.Rotation = 0
        Me.Sam3107.ShowThrough = False
        Me.Sam3107.Size = New System.Drawing.Size(24, 23)
        Me.Sam3107.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3107.TabIndex = 290
        Me.Sam3107.TabStop = False
        Me.Sam3107.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3134
        '
        Me.Sam3134.BackColor = System.Drawing.Color.Transparent
        Me.Sam3134.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3134.Image = Nothing
        Me.Sam3134.ImagePath = ""
        Me.Sam3134.IsTransparentImage = True
        Me.Sam3134.Location = New System.Drawing.Point(240, 480)
        Me.Sam3134.Name = "Sam3134"
        Me.Sam3134.Rotation = 0
        Me.Sam3134.ShowThrough = False
        Me.Sam3134.Size = New System.Drawing.Size(24, 24)
        Me.Sam3134.TabIndex = 317
        Me.Sam3134.TabStop = False
        Me.Sam3134.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3126
        '
        Me.Sam3126.BackColor = System.Drawing.Color.Transparent
        Me.Sam3126.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3126.Image = Nothing
        Me.Sam3126.ImagePath = ""
        Me.Sam3126.IsTransparentImage = False
        Me.Sam3126.Location = New System.Drawing.Point(88, 335)
        Me.Sam3126.Name = "Sam3126"
        Me.Sam3126.Rotation = 0
        Me.Sam3126.ShowThrough = False
        Me.Sam3126.Size = New System.Drawing.Size(24, 24)
        Me.Sam3126.TabIndex = 309
        Me.Sam3126.TabStop = False
        Me.Sam3126.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3125
        '
        Me.Sam3125.BackColor = System.Drawing.Color.Transparent
        Me.Sam3125.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3125.Image = Nothing
        Me.Sam3125.ImagePath = ""
        Me.Sam3125.IsTransparentImage = False
        Me.Sam3125.Location = New System.Drawing.Point(83, 308)
        Me.Sam3125.Name = "Sam3125"
        Me.Sam3125.Rotation = 0
        Me.Sam3125.ShowThrough = False
        Me.Sam3125.Size = New System.Drawing.Size(24, 24)
        Me.Sam3125.TabIndex = 308
        Me.Sam3125.TabStop = False
        Me.Sam3125.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3124
        '
        Me.Sam3124.BackColor = System.Drawing.Color.Transparent
        Me.Sam3124.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3124.Image = Nothing
        Me.Sam3124.ImagePath = ""
        Me.Sam3124.IsTransparentImage = False
        Me.Sam3124.Location = New System.Drawing.Point(81, 280)
        Me.Sam3124.Name = "Sam3124"
        Me.Sam3124.Rotation = 0
        Me.Sam3124.ShowThrough = False
        Me.Sam3124.Size = New System.Drawing.Size(24, 24)
        Me.Sam3124.TabIndex = 307
        Me.Sam3124.TabStop = False
        Me.Sam3124.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3123
        '
        Me.Sam3123.BackColor = System.Drawing.Color.Transparent
        Me.Sam3123.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3123.Image = Nothing
        Me.Sam3123.ImagePath = ""
        Me.Sam3123.IsTransparentImage = False
        Me.Sam3123.Location = New System.Drawing.Point(83, 252)
        Me.Sam3123.Name = "Sam3123"
        Me.Sam3123.Rotation = 0
        Me.Sam3123.ShowThrough = False
        Me.Sam3123.Size = New System.Drawing.Size(24, 24)
        Me.Sam3123.TabIndex = 306
        Me.Sam3123.TabStop = False
        Me.Sam3123.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3122
        '
        Me.Sam3122.BackColor = System.Drawing.Color.Transparent
        Me.Sam3122.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3122.Image = Nothing
        Me.Sam3122.ImagePath = ""
        Me.Sam3122.IsTransparentImage = False
        Me.Sam3122.Location = New System.Drawing.Point(89, 225)
        Me.Sam3122.Name = "Sam3122"
        Me.Sam3122.Rotation = 0
        Me.Sam3122.ShowThrough = False
        Me.Sam3122.Size = New System.Drawing.Size(24, 24)
        Me.Sam3122.TabIndex = 305
        Me.Sam3122.TabStop = False
        Me.Sam3122.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3121
        '
        Me.Sam3121.BackColor = System.Drawing.Color.Transparent
        Me.Sam3121.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3121.Image = Nothing
        Me.Sam3121.ImagePath = ""
        Me.Sam3121.IsTransparentImage = False
        Me.Sam3121.Location = New System.Drawing.Point(99, 199)
        Me.Sam3121.Name = "Sam3121"
        Me.Sam3121.Rotation = 0
        Me.Sam3121.ShowThrough = False
        Me.Sam3121.Size = New System.Drawing.Size(24, 24)
        Me.Sam3121.TabIndex = 304
        Me.Sam3121.TabStop = False
        Me.Sam3121.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3120
        '
        Me.Sam3120.BackColor = System.Drawing.Color.Transparent
        Me.Sam3120.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3120.Image = Nothing
        Me.Sam3120.ImagePath = ""
        Me.Sam3120.IsTransparentImage = False
        Me.Sam3120.Location = New System.Drawing.Point(113, 175)
        Me.Sam3120.Name = "Sam3120"
        Me.Sam3120.Rotation = 0
        Me.Sam3120.ShowThrough = False
        Me.Sam3120.Size = New System.Drawing.Size(24, 24)
        Me.Sam3120.TabIndex = 303
        Me.Sam3120.TabStop = False
        Me.Sam3120.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3119
        '
        Me.Sam3119.BackColor = System.Drawing.Color.Transparent
        Me.Sam3119.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3119.Image = Nothing
        Me.Sam3119.ImagePath = ""
        Me.Sam3119.IsTransparentImage = False
        Me.Sam3119.Location = New System.Drawing.Point(129, 153)
        Me.Sam3119.Name = "Sam3119"
        Me.Sam3119.Rotation = 0
        Me.Sam3119.ShowThrough = False
        Me.Sam3119.Size = New System.Drawing.Size(24, 23)
        Me.Sam3119.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3119.TabIndex = 302
        Me.Sam3119.TabStop = False
        Me.Sam3119.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3118
        '
        Me.Sam3118.BackColor = System.Drawing.Color.Transparent
        Me.Sam3118.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3118.Image = Nothing
        Me.Sam3118.ImagePath = ""
        Me.Sam3118.IsTransparentImage = False
        Me.Sam3118.Location = New System.Drawing.Point(149, 133)
        Me.Sam3118.Name = "Sam3118"
        Me.Sam3118.Rotation = 0
        Me.Sam3118.ShowThrough = False
        Me.Sam3118.Size = New System.Drawing.Size(24, 23)
        Me.Sam3118.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3118.TabIndex = 301
        Me.Sam3118.TabStop = False
        Me.Sam3118.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3117
        '
        Me.Sam3117.BackColor = System.Drawing.Color.Transparent
        Me.Sam3117.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3117.Image = Nothing
        Me.Sam3117.ImagePath = ""
        Me.Sam3117.IsTransparentImage = False
        Me.Sam3117.Location = New System.Drawing.Point(171, 116)
        Me.Sam3117.Name = "Sam3117"
        Me.Sam3117.Rotation = 0
        Me.Sam3117.ShowThrough = False
        Me.Sam3117.Size = New System.Drawing.Size(24, 23)
        Me.Sam3117.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3117.TabIndex = 300
        Me.Sam3117.TabStop = False
        Me.Sam3117.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3116
        '
        Me.Sam3116.BackColor = System.Drawing.Color.Transparent
        Me.Sam3116.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3116.Image = Nothing
        Me.Sam3116.ImagePath = ""
        Me.Sam3116.IsTransparentImage = False
        Me.Sam3116.Location = New System.Drawing.Point(195, 102)
        Me.Sam3116.Name = "Sam3116"
        Me.Sam3116.Rotation = 0
        Me.Sam3116.ShowThrough = False
        Me.Sam3116.Size = New System.Drawing.Size(24, 23)
        Me.Sam3116.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam3116.TabIndex = 299
        Me.Sam3116.TabStop = False
        Me.Sam3116.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3115
        '
        Me.Sam3115.BackColor = System.Drawing.Color.Transparent
        Me.Sam3115.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3115.Image = Nothing
        Me.Sam3115.ImagePath = ""
        Me.Sam3115.IsTransparentImage = False
        Me.Sam3115.Location = New System.Drawing.Point(220, 92)
        Me.Sam3115.Name = "Sam3115"
        Me.Sam3115.Rotation = 0
        Me.Sam3115.ShowThrough = False
        Me.Sam3115.Size = New System.Drawing.Size(24, 24)
        Me.Sam3115.TabIndex = 298
        Me.Sam3115.TabStop = False
        Me.Sam3115.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3114
        '
        Me.Sam3114.BackColor = System.Drawing.Color.Transparent
        Me.Sam3114.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3114.Image = Nothing
        Me.Sam3114.ImagePath = ""
        Me.Sam3114.IsTransparentImage = False
        Me.Sam3114.Location = New System.Drawing.Point(246, 85)
        Me.Sam3114.Name = "Sam3114"
        Me.Sam3114.Rotation = 0
        Me.Sam3114.ShowThrough = False
        Me.Sam3114.Size = New System.Drawing.Size(24, 24)
        Me.Sam3114.TabIndex = 297
        Me.Sam3114.TabStop = False
        Me.Sam3114.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3113
        '
        Me.Sam3113.BackColor = System.Drawing.Color.Transparent
        Me.Sam3113.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3113.Image = Nothing
        Me.Sam3113.ImagePath = ""
        Me.Sam3113.IsTransparentImage = False
        Me.Sam3113.Location = New System.Drawing.Point(274, 83)
        Me.Sam3113.Name = "Sam3113"
        Me.Sam3113.Rotation = 0
        Me.Sam3113.ShowThrough = False
        Me.Sam3113.Size = New System.Drawing.Size(24, 24)
        Me.Sam3113.TabIndex = 296
        Me.Sam3113.TabStop = False
        Me.Sam3113.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3112
        '
        Me.Sam3112.BackColor = System.Drawing.Color.Transparent
        Me.Sam3112.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3112.Image = Nothing
        Me.Sam3112.ImagePath = ""
        Me.Sam3112.IsTransparentImage = False
        Me.Sam3112.Location = New System.Drawing.Point(301, 84)
        Me.Sam3112.Name = "Sam3112"
        Me.Sam3112.Rotation = 0
        Me.Sam3112.ShowThrough = False
        Me.Sam3112.Size = New System.Drawing.Size(24, 24)
        Me.Sam3112.TabIndex = 295
        Me.Sam3112.TabStop = False
        Me.Sam3112.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3111
        '
        Me.Sam3111.BackColor = System.Drawing.Color.Transparent
        Me.Sam3111.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3111.Image = Nothing
        Me.Sam3111.ImagePath = ""
        Me.Sam3111.IsTransparentImage = False
        Me.Sam3111.Location = New System.Drawing.Point(328, 89)
        Me.Sam3111.Name = "Sam3111"
        Me.Sam3111.Rotation = 0
        Me.Sam3111.ShowThrough = False
        Me.Sam3111.Size = New System.Drawing.Size(24, 24)
        Me.Sam3111.TabIndex = 294
        Me.Sam3111.TabStop = False
        Me.Sam3111.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3110
        '
        Me.Sam3110.BackColor = System.Drawing.Color.Transparent
        Me.Sam3110.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3110.Image = Nothing
        Me.Sam3110.ImagePath = ""
        Me.Sam3110.IsTransparentImage = False
        Me.Sam3110.Location = New System.Drawing.Point(354, 98)
        Me.Sam3110.Name = "Sam3110"
        Me.Sam3110.Rotation = 0
        Me.Sam3110.ShowThrough = False
        Me.Sam3110.Size = New System.Drawing.Size(24, 24)
        Me.Sam3110.TabIndex = 293
        Me.Sam3110.TabStop = False
        Me.Sam3110.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3109
        '
        Me.Sam3109.BackColor = System.Drawing.Color.Transparent
        Me.Sam3109.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3109.Image = Nothing
        Me.Sam3109.ImagePath = ""
        Me.Sam3109.IsTransparentImage = False
        Me.Sam3109.Location = New System.Drawing.Point(378, 111)
        Me.Sam3109.Name = "Sam3109"
        Me.Sam3109.Rotation = 0
        Me.Sam3109.ShowThrough = False
        Me.Sam3109.Size = New System.Drawing.Size(24, 24)
        Me.Sam3109.TabIndex = 292
        Me.Sam3109.TabStop = False
        Me.Sam3109.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3108
        '
        Me.Sam3108.BackColor = System.Drawing.Color.Transparent
        Me.Sam3108.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3108.Image = Nothing
        Me.Sam3108.ImagePath = ""
        Me.Sam3108.IsTransparentImage = False
        Me.Sam3108.Location = New System.Drawing.Point(401, 127)
        Me.Sam3108.Name = "Sam3108"
        Me.Sam3108.Rotation = 0
        Me.Sam3108.ShowThrough = False
        Me.Sam3108.Size = New System.Drawing.Size(24, 24)
        Me.Sam3108.TabIndex = 291
        Me.Sam3108.TabStop = False
        Me.Sam3108.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3105
        '
        Me.Sam3105.BackColor = System.Drawing.Color.Transparent
        Me.Sam3105.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3105.Image = Nothing
        Me.Sam3105.ImagePath = ""
        Me.Sam3105.IsTransparentImage = False
        Me.Sam3105.Location = New System.Drawing.Point(453, 192)
        Me.Sam3105.Name = "Sam3105"
        Me.Sam3105.Rotation = 0
        Me.Sam3105.ShowThrough = False
        Me.Sam3105.Size = New System.Drawing.Size(24, 24)
        Me.Sam3105.TabIndex = 288
        Me.Sam3105.TabStop = False
        Me.Sam3105.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3104
        '
        Me.Sam3104.BackColor = System.Drawing.Color.Transparent
        Me.Sam3104.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3104.Image = Nothing
        Me.Sam3104.ImagePath = ""
        Me.Sam3104.IsTransparentImage = False
        Me.Sam3104.Location = New System.Drawing.Point(463, 217)
        Me.Sam3104.Name = "Sam3104"
        Me.Sam3104.Rotation = 0
        Me.Sam3104.ShowThrough = False
        Me.Sam3104.Size = New System.Drawing.Size(24, 24)
        Me.Sam3104.TabIndex = 287
        Me.Sam3104.TabStop = False
        Me.Sam3104.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3103
        '
        Me.Sam3103.BackColor = System.Drawing.Color.Transparent
        Me.Sam3103.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3103.Image = Nothing
        Me.Sam3103.ImagePath = ""
        Me.Sam3103.IsTransparentImage = False
        Me.Sam3103.Location = New System.Drawing.Point(471, 244)
        Me.Sam3103.Name = "Sam3103"
        Me.Sam3103.Rotation = 0
        Me.Sam3103.ShowThrough = False
        Me.Sam3103.Size = New System.Drawing.Size(24, 24)
        Me.Sam3103.TabIndex = 286
        Me.Sam3103.TabStop = False
        Me.Sam3103.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3102
        '
        Me.Sam3102.BackColor = System.Drawing.Color.Transparent
        Me.Sam3102.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3102.Image = Nothing
        Me.Sam3102.ImagePath = ""
        Me.Sam3102.IsTransparentImage = False
        Me.Sam3102.Location = New System.Drawing.Point(474, 272)
        Me.Sam3102.Name = "Sam3102"
        Me.Sam3102.Rotation = 0
        Me.Sam3102.ShowThrough = False
        Me.Sam3102.Size = New System.Drawing.Size(24, 24)
        Me.Sam3102.TabIndex = 285
        Me.Sam3102.TabStop = False
        Me.Sam3102.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3101
        '
        Me.Sam3101.BackColor = System.Drawing.Color.Transparent
        Me.Sam3101.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3101.Image = Nothing
        Me.Sam3101.ImagePath = ""
        Me.Sam3101.IsTransparentImage = False
        Me.Sam3101.Location = New System.Drawing.Point(473, 300)
        Me.Sam3101.Name = "Sam3101"
        Me.Sam3101.Rotation = 0
        Me.Sam3101.ShowThrough = False
        Me.Sam3101.Size = New System.Drawing.Size(24, 24)
        Me.Sam3101.TabIndex = 284
        Me.Sam3101.TabStop = False
        Me.Sam3101.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam3100
        '
        Me.Sam3100.BackColor = System.Drawing.Color.Transparent
        Me.Sam3100.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam3100.Image = Nothing
        Me.Sam3100.ImagePath = ""
        Me.Sam3100.IsTransparentImage = False
        Me.Sam3100.Location = New System.Drawing.Point(469, 328)
        Me.Sam3100.Name = "Sam3100"
        Me.Sam3100.Rotation = 0
        Me.Sam3100.ShowThrough = False
        Me.Sam3100.Size = New System.Drawing.Size(24, 24)
        Me.Sam3100.TabIndex = 283
        Me.Sam3100.TabStop = False
        Me.Sam3100.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam399
        '
        Me.Sam399.BackColor = System.Drawing.Color.Transparent
        Me.Sam399.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam399.Image = Nothing
        Me.Sam399.ImagePath = ""
        Me.Sam399.IsTransparentImage = False
        Me.Sam399.Location = New System.Drawing.Point(461, 354)
        Me.Sam399.Name = "Sam399"
        Me.Sam399.Rotation = 0
        Me.Sam399.ShowThrough = False
        Me.Sam399.Size = New System.Drawing.Size(24, 24)
        Me.Sam399.TabIndex = 281
        Me.Sam399.TabStop = False
        Me.Sam399.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam398
        '
        Me.Sam398.BackColor = System.Drawing.Color.Transparent
        Me.Sam398.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam398.Image = Nothing
        Me.Sam398.ImagePath = ""
        Me.Sam398.IsTransparentImage = False
        Me.Sam398.Location = New System.Drawing.Point(450, 379)
        Me.Sam398.Name = "Sam398"
        Me.Sam398.Rotation = 0
        Me.Sam398.ShowThrough = False
        Me.Sam398.Size = New System.Drawing.Size(24, 24)
        Me.Sam398.TabIndex = 280
        Me.Sam398.TabStop = False
        Me.Sam398.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam394
        '
        Me.Sam394.BackColor = System.Drawing.Color.Transparent
        Me.Sam394.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam394.Image = Nothing
        Me.Sam394.ImagePath = ""
        Me.Sam394.IsTransparentImage = False
        Me.Sam394.Location = New System.Drawing.Point(373, 458)
        Me.Sam394.Name = "Sam394"
        Me.Sam394.Rotation = 0
        Me.Sam394.ShowThrough = False
        Me.Sam394.Size = New System.Drawing.Size(24, 24)
        Me.Sam394.TabIndex = 276
        Me.Sam394.TabStop = False
        Me.Sam394.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam393
        '
        Me.Sam393.BackColor = System.Drawing.Color.Transparent
        Me.Sam393.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam393.Image = Nothing
        Me.Sam393.ImagePath = ""
        Me.Sam393.IsTransparentImage = False
        Me.Sam393.Location = New System.Drawing.Point(349, 470)
        Me.Sam393.Name = "Sam393"
        Me.Sam393.Rotation = 0
        Me.Sam393.ShowThrough = False
        Me.Sam393.Size = New System.Drawing.Size(24, 23)
        Me.Sam393.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Sam393.TabIndex = 275
        Me.Sam393.TabStop = False
        Me.Sam393.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam392
        '
        Me.Sam392.BackColor = System.Drawing.Color.Transparent
        Me.Sam392.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam392.Image = Nothing
        Me.Sam392.ImagePath = ""
        Me.Sam392.IsTransparentImage = False
        Me.Sam392.Location = New System.Drawing.Point(322, 478)
        Me.Sam392.Name = "Sam392"
        Me.Sam392.Rotation = 0
        Me.Sam392.ShowThrough = False
        Me.Sam392.Size = New System.Drawing.Size(24, 24)
        Me.Sam392.TabIndex = 274
        Me.Sam392.TabStop = False
        Me.Sam392.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam391
        '
        Me.Sam391.BackColor = System.Drawing.Color.Transparent
        Me.Sam391.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam391.Image = Nothing
        Me.Sam391.ImagePath = ""
        Me.Sam391.IsTransparentImage = False
        Me.Sam391.Location = New System.Drawing.Point(295, 482)
        Me.Sam391.Name = "Sam391"
        Me.Sam391.Rotation = 0
        Me.Sam391.ShowThrough = False
        Me.Sam391.Size = New System.Drawing.Size(24, 24)
        Me.Sam391.TabIndex = 273
        Me.Sam391.TabStop = False
        Me.Sam391.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam290
        '
        Me.Sam290.BackColor = System.Drawing.Color.Transparent
        Me.Sam290.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam290.Image = Nothing
        Me.Sam290.ImagePath = ""
        Me.Sam290.IsTransparentImage = False
        Me.Sam290.Location = New System.Drawing.Point(258, 523)
        Me.Sam290.Name = "Sam290"
        Me.Sam290.Rotation = 0
        Me.Sam290.ShowThrough = False
        Me.Sam290.Size = New System.Drawing.Size(24, 24)
        Me.Sam290.TabIndex = 272
        Me.Sam290.TabStop = False
        Me.Sam290.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam289
        '
        Me.Sam289.BackColor = System.Drawing.Color.Transparent
        Me.Sam289.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam289.Image = Nothing
        Me.Sam289.ImagePath = ""
        Me.Sam289.IsTransparentImage = False
        Me.Sam289.Location = New System.Drawing.Point(225, 517)
        Me.Sam289.Name = "Sam289"
        Me.Sam289.Rotation = 0
        Me.Sam289.ShowThrough = False
        Me.Sam289.Size = New System.Drawing.Size(24, 24)
        Me.Sam289.TabIndex = 271
        Me.Sam289.TabStop = False
        Me.Sam289.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam288
        '
        Me.Sam288.BackColor = System.Drawing.Color.Transparent
        Me.Sam288.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam288.Image = Nothing
        Me.Sam288.ImagePath = ""
        Me.Sam288.IsTransparentImage = False
        Me.Sam288.Location = New System.Drawing.Point(193, 508)
        Me.Sam288.Name = "Sam288"
        Me.Sam288.Rotation = 0
        Me.Sam288.ShowThrough = False
        Me.Sam288.Size = New System.Drawing.Size(24, 24)
        Me.Sam288.TabIndex = 270
        Me.Sam288.TabStop = False
        Me.Sam288.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam287
        '
        Me.Sam287.BackColor = System.Drawing.Color.Transparent
        Me.Sam287.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam287.Image = Nothing
        Me.Sam287.ImagePath = ""
        Me.Sam287.IsTransparentImage = False
        Me.Sam287.Location = New System.Drawing.Point(164, 493)
        Me.Sam287.Name = "Sam287"
        Me.Sam287.Rotation = 0
        Me.Sam287.ShowThrough = False
        Me.Sam287.Size = New System.Drawing.Size(24, 24)
        Me.Sam287.TabIndex = 269
        Me.Sam287.TabStop = False
        Me.Sam287.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam286
        '
        Me.Sam286.BackColor = System.Drawing.Color.Transparent
        Me.Sam286.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam286.Image = Nothing
        Me.Sam286.ImagePath = ""
        Me.Sam286.IsTransparentImage = False
        Me.Sam286.Location = New System.Drawing.Point(136, 475)
        Me.Sam286.Name = "Sam286"
        Me.Sam286.Rotation = 0
        Me.Sam286.ShowThrough = False
        Me.Sam286.Size = New System.Drawing.Size(24, 24)
        Me.Sam286.TabIndex = 268
        Me.Sam286.TabStop = False
        Me.Sam286.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam285
        '
        Me.Sam285.BackColor = System.Drawing.Color.Transparent
        Me.Sam285.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam285.Image = Nothing
        Me.Sam285.ImagePath = ""
        Me.Sam285.IsTransparentImage = False
        Me.Sam285.Location = New System.Drawing.Point(111, 453)
        Me.Sam285.Name = "Sam285"
        Me.Sam285.Rotation = 0
        Me.Sam285.ShowThrough = False
        Me.Sam285.Size = New System.Drawing.Size(24, 24)
        Me.Sam285.TabIndex = 267
        Me.Sam285.TabStop = False
        Me.Sam285.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam284
        '
        Me.Sam284.BackColor = System.Drawing.Color.Transparent
        Me.Sam284.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam284.Image = Nothing
        Me.Sam284.ImagePath = ""
        Me.Sam284.IsTransparentImage = False
        Me.Sam284.Location = New System.Drawing.Point(89, 428)
        Me.Sam284.Name = "Sam284"
        Me.Sam284.Rotation = 0
        Me.Sam284.ShowThrough = False
        Me.Sam284.Size = New System.Drawing.Size(24, 24)
        Me.Sam284.TabIndex = 266
        Me.Sam284.TabStop = False
        Me.Sam284.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam283
        '
        Me.Sam283.BackColor = System.Drawing.Color.Transparent
        Me.Sam283.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam283.Image = Nothing
        Me.Sam283.ImagePath = ""
        Me.Sam283.IsTransparentImage = False
        Me.Sam283.Location = New System.Drawing.Point(71, 400)
        Me.Sam283.Name = "Sam283"
        Me.Sam283.Rotation = 0
        Me.Sam283.ShowThrough = False
        Me.Sam283.Size = New System.Drawing.Size(24, 24)
        Me.Sam283.TabIndex = 265
        Me.Sam283.TabStop = False
        Me.Sam283.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam282
        '
        Me.Sam282.BackColor = System.Drawing.Color.Transparent
        Me.Sam282.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam282.Image = Nothing
        Me.Sam282.ImagePath = ""
        Me.Sam282.IsTransparentImage = False
        Me.Sam282.Location = New System.Drawing.Point(58, 370)
        Me.Sam282.Name = "Sam282"
        Me.Sam282.Rotation = 0
        Me.Sam282.ShowThrough = False
        Me.Sam282.Size = New System.Drawing.Size(24, 24)
        Me.Sam282.TabIndex = 264
        Me.Sam282.TabStop = False
        Me.Sam282.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam281
        '
        Me.Sam281.BackColor = System.Drawing.Color.Transparent
        Me.Sam281.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam281.Image = Nothing
        Me.Sam281.ImagePath = ""
        Me.Sam281.IsTransparentImage = False
        Me.Sam281.Location = New System.Drawing.Point(48, 338)
        Me.Sam281.Name = "Sam281"
        Me.Sam281.Rotation = 0
        Me.Sam281.ShowThrough = False
        Me.Sam281.Size = New System.Drawing.Size(24, 24)
        Me.Sam281.TabIndex = 263
        Me.Sam281.TabStop = False
        Me.Sam281.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam280
        '
        Me.Sam280.BackColor = System.Drawing.Color.Transparent
        Me.Sam280.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam280.Image = Nothing
        Me.Sam280.ImagePath = ""
        Me.Sam280.IsTransparentImage = False
        Me.Sam280.Location = New System.Drawing.Point(43, 305)
        Me.Sam280.Name = "Sam280"
        Me.Sam280.Rotation = 0
        Me.Sam280.ShowThrough = False
        Me.Sam280.Size = New System.Drawing.Size(24, 24)
        Me.Sam280.TabIndex = 262
        Me.Sam280.TabStop = False
        Me.Sam280.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam279
        '
        Me.Sam279.BackColor = System.Drawing.Color.Transparent
        Me.Sam279.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam279.Image = Nothing
        Me.Sam279.ImagePath = ""
        Me.Sam279.IsTransparentImage = False
        Me.Sam279.Location = New System.Drawing.Point(42, 271)
        Me.Sam279.Name = "Sam279"
        Me.Sam279.Rotation = 0
        Me.Sam279.ShowThrough = False
        Me.Sam279.Size = New System.Drawing.Size(24, 24)
        Me.Sam279.TabIndex = 261
        Me.Sam279.TabStop = False
        Me.Sam279.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam278
        '
        Me.Sam278.BackColor = System.Drawing.Color.Transparent
        Me.Sam278.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam278.Image = Nothing
        Me.Sam278.ImagePath = ""
        Me.Sam278.IsTransparentImage = False
        Me.Sam278.Location = New System.Drawing.Point(46, 238)
        Me.Sam278.Name = "Sam278"
        Me.Sam278.Rotation = 0
        Me.Sam278.ShowThrough = False
        Me.Sam278.Size = New System.Drawing.Size(24, 24)
        Me.Sam278.TabIndex = 260
        Me.Sam278.TabStop = False
        Me.Sam278.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam277
        '
        Me.Sam277.BackColor = System.Drawing.Color.Transparent
        Me.Sam277.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam277.Image = Nothing
        Me.Sam277.ImagePath = ""
        Me.Sam277.IsTransparentImage = False
        Me.Sam277.Location = New System.Drawing.Point(54, 205)
        Me.Sam277.Name = "Sam277"
        Me.Sam277.Rotation = 0
        Me.Sam277.ShowThrough = False
        Me.Sam277.Size = New System.Drawing.Size(24, 24)
        Me.Sam277.TabIndex = 259
        Me.Sam277.TabStop = False
        Me.Sam277.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam276
        '
        Me.Sam276.BackColor = System.Drawing.Color.Transparent
        Me.Sam276.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam276.Image = Nothing
        Me.Sam276.ImagePath = ""
        Me.Sam276.IsTransparentImage = False
        Me.Sam276.Location = New System.Drawing.Point(67, 174)
        Me.Sam276.Name = "Sam276"
        Me.Sam276.Rotation = 0
        Me.Sam276.ShowThrough = False
        Me.Sam276.Size = New System.Drawing.Size(24, 24)
        Me.Sam276.TabIndex = 258
        Me.Sam276.TabStop = False
        Me.Sam276.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam275
        '
        Me.Sam275.BackColor = System.Drawing.Color.Transparent
        Me.Sam275.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam275.Image = Nothing
        Me.Sam275.ImagePath = ""
        Me.Sam275.IsTransparentImage = False
        Me.Sam275.Location = New System.Drawing.Point(84, 145)
        Me.Sam275.Name = "Sam275"
        Me.Sam275.Rotation = 0
        Me.Sam275.ShowThrough = False
        Me.Sam275.Size = New System.Drawing.Size(24, 24)
        Me.Sam275.TabIndex = 257
        Me.Sam275.TabStop = False
        Me.Sam275.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam274
        '
        Me.Sam274.BackColor = System.Drawing.Color.Transparent
        Me.Sam274.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam274.Image = Nothing
        Me.Sam274.ImagePath = ""
        Me.Sam274.IsTransparentImage = False
        Me.Sam274.Location = New System.Drawing.Point(105, 119)
        Me.Sam274.Name = "Sam274"
        Me.Sam274.Rotation = 0
        Me.Sam274.ShowThrough = False
        Me.Sam274.Size = New System.Drawing.Size(24, 24)
        Me.Sam274.TabIndex = 256
        Me.Sam274.TabStop = False
        Me.Sam274.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam273
        '
        Me.Sam273.BackColor = System.Drawing.Color.Transparent
        Me.Sam273.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam273.Image = Nothing
        Me.Sam273.ImagePath = ""
        Me.Sam273.IsTransparentImage = False
        Me.Sam273.Location = New System.Drawing.Point(129, 96)
        Me.Sam273.Name = "Sam273"
        Me.Sam273.Rotation = 0
        Me.Sam273.ShowThrough = False
        Me.Sam273.Size = New System.Drawing.Size(24, 24)
        Me.Sam273.TabIndex = 255
        Me.Sam273.TabStop = False
        Me.Sam273.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam272
        '
        Me.Sam272.BackColor = System.Drawing.Color.Transparent
        Me.Sam272.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam272.Image = Nothing
        Me.Sam272.ImagePath = ""
        Me.Sam272.IsTransparentImage = False
        Me.Sam272.Location = New System.Drawing.Point(156, 77)
        Me.Sam272.Name = "Sam272"
        Me.Sam272.Rotation = 0
        Me.Sam272.ShowThrough = False
        Me.Sam272.Size = New System.Drawing.Size(24, 24)
        Me.Sam272.TabIndex = 254
        Me.Sam272.TabStop = False
        Me.Sam272.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam271
        '
        Me.Sam271.BackColor = System.Drawing.Color.Transparent
        Me.Sam271.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam271.Image = Nothing
        Me.Sam271.ImagePath = ""
        Me.Sam271.IsTransparentImage = False
        Me.Sam271.Location = New System.Drawing.Point(185, 61)
        Me.Sam271.Name = "Sam271"
        Me.Sam271.Rotation = 0
        Me.Sam271.ShowThrough = False
        Me.Sam271.Size = New System.Drawing.Size(24, 24)
        Me.Sam271.TabIndex = 253
        Me.Sam271.TabStop = False
        Me.Sam271.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam270
        '
        Me.Sam270.BackColor = System.Drawing.Color.Transparent
        Me.Sam270.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam270.Image = Nothing
        Me.Sam270.ImagePath = ""
        Me.Sam270.IsTransparentImage = False
        Me.Sam270.Location = New System.Drawing.Point(216, 50)
        Me.Sam270.Name = "Sam270"
        Me.Sam270.Rotation = 0
        Me.Sam270.ShowThrough = False
        Me.Sam270.Size = New System.Drawing.Size(24, 24)
        Me.Sam270.TabIndex = 252
        Me.Sam270.TabStop = False
        Me.Sam270.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam269
        '
        Me.Sam269.BackColor = System.Drawing.Color.Transparent
        Me.Sam269.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam269.Image = Nothing
        Me.Sam269.ImagePath = ""
        Me.Sam269.IsTransparentImage = False
        Me.Sam269.Location = New System.Drawing.Point(248, 44)
        Me.Sam269.Name = "Sam269"
        Me.Sam269.Rotation = 0
        Me.Sam269.ShowThrough = False
        Me.Sam269.Size = New System.Drawing.Size(24, 24)
        Me.Sam269.TabIndex = 251
        Me.Sam269.TabStop = False
        Me.Sam269.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam268
        '
        Me.Sam268.BackColor = System.Drawing.Color.Transparent
        Me.Sam268.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam268.Image = Nothing
        Me.Sam268.ImagePath = ""
        Me.Sam268.IsTransparentImage = False
        Me.Sam268.Location = New System.Drawing.Point(281, 42)
        Me.Sam268.Name = "Sam268"
        Me.Sam268.Rotation = 0
        Me.Sam268.ShowThrough = False
        Me.Sam268.Size = New System.Drawing.Size(24, 24)
        Me.Sam268.TabIndex = 250
        Me.Sam268.TabStop = False
        Me.Sam268.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam267
        '
        Me.Sam267.BackColor = System.Drawing.Color.Transparent
        Me.Sam267.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam267.Image = Nothing
        Me.Sam267.ImagePath = ""
        Me.Sam267.IsTransparentImage = False
        Me.Sam267.Location = New System.Drawing.Point(314, 45)
        Me.Sam267.Name = "Sam267"
        Me.Sam267.Rotation = 0
        Me.Sam267.ShowThrough = False
        Me.Sam267.Size = New System.Drawing.Size(24, 24)
        Me.Sam267.TabIndex = 249
        Me.Sam267.TabStop = False
        Me.Sam267.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam266
        '
        Me.Sam266.BackColor = System.Drawing.Color.Transparent
        Me.Sam266.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam266.Image = Nothing
        Me.Sam266.ImagePath = ""
        Me.Sam266.IsTransparentImage = False
        Me.Sam266.Location = New System.Drawing.Point(346, 52)
        Me.Sam266.Name = "Sam266"
        Me.Sam266.Rotation = 0
        Me.Sam266.ShowThrough = False
        Me.Sam266.Size = New System.Drawing.Size(24, 24)
        Me.Sam266.TabIndex = 248
        Me.Sam266.TabStop = False
        Me.Sam266.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam265
        '
        Me.Sam265.BackColor = System.Drawing.Color.Transparent
        Me.Sam265.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam265.Image = Nothing
        Me.Sam265.ImagePath = ""
        Me.Sam265.IsTransparentImage = False
        Me.Sam265.Location = New System.Drawing.Point(377, 64)
        Me.Sam265.Name = "Sam265"
        Me.Sam265.Rotation = 0
        Me.Sam265.ShowThrough = False
        Me.Sam265.Size = New System.Drawing.Size(24, 24)
        Me.Sam265.TabIndex = 247
        Me.Sam265.TabStop = False
        Me.Sam265.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam264
        '
        Me.Sam264.BackColor = System.Drawing.Color.Transparent
        Me.Sam264.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam264.Image = Nothing
        Me.Sam264.ImagePath = ""
        Me.Sam264.IsTransparentImage = False
        Me.Sam264.Location = New System.Drawing.Point(406, 81)
        Me.Sam264.Name = "Sam264"
        Me.Sam264.Rotation = 0
        Me.Sam264.ShowThrough = False
        Me.Sam264.Size = New System.Drawing.Size(24, 24)
        Me.Sam264.TabIndex = 246
        Me.Sam264.TabStop = False
        Me.Sam264.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam263
        '
        Me.Sam263.BackColor = System.Drawing.Color.Transparent
        Me.Sam263.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam263.Image = Nothing
        Me.Sam263.ImagePath = ""
        Me.Sam263.IsTransparentImage = False
        Me.Sam263.Location = New System.Drawing.Point(433, 101)
        Me.Sam263.Name = "Sam263"
        Me.Sam263.Rotation = 0
        Me.Sam263.ShowThrough = False
        Me.Sam263.Size = New System.Drawing.Size(24, 24)
        Me.Sam263.TabIndex = 245
        Me.Sam263.TabStop = False
        Me.Sam263.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam262
        '
        Me.Sam262.BackColor = System.Drawing.Color.Transparent
        Me.Sam262.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam262.Image = Nothing
        Me.Sam262.ImagePath = ""
        Me.Sam262.IsTransparentImage = False
        Me.Sam262.Location = New System.Drawing.Point(456, 124)
        Me.Sam262.Name = "Sam262"
        Me.Sam262.Rotation = 0
        Me.Sam262.ShowThrough = False
        Me.Sam262.Size = New System.Drawing.Size(24, 24)
        Me.Sam262.TabIndex = 244
        Me.Sam262.TabStop = False
        Me.Sam262.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam261
        '
        Me.Sam261.BackColor = System.Drawing.Color.Transparent
        Me.Sam261.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam261.Image = Nothing
        Me.Sam261.ImagePath = ""
        Me.Sam261.IsTransparentImage = False
        Me.Sam261.Location = New System.Drawing.Point(475, 151)
        Me.Sam261.Name = "Sam261"
        Me.Sam261.Rotation = 0
        Me.Sam261.ShowThrough = False
        Me.Sam261.Size = New System.Drawing.Size(24, 24)
        Me.Sam261.TabIndex = 243
        Me.Sam261.TabStop = False
        Me.Sam261.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam260
        '
        Me.Sam260.BackColor = System.Drawing.Color.Transparent
        Me.Sam260.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam260.Image = Nothing
        Me.Sam260.ImagePath = ""
        Me.Sam260.IsTransparentImage = False
        Me.Sam260.Location = New System.Drawing.Point(492, 181)
        Me.Sam260.Name = "Sam260"
        Me.Sam260.Rotation = 0
        Me.Sam260.ShowThrough = False
        Me.Sam260.Size = New System.Drawing.Size(24, 24)
        Me.Sam260.TabIndex = 242
        Me.Sam260.TabStop = False
        Me.Sam260.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam259
        '
        Me.Sam259.BackColor = System.Drawing.Color.Transparent
        Me.Sam259.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam259.Image = Nothing
        Me.Sam259.ImagePath = ""
        Me.Sam259.IsTransparentImage = False
        Me.Sam259.Location = New System.Drawing.Point(504, 212)
        Me.Sam259.Name = "Sam259"
        Me.Sam259.Rotation = 0
        Me.Sam259.ShowThrough = False
        Me.Sam259.Size = New System.Drawing.Size(24, 24)
        Me.Sam259.TabIndex = 241
        Me.Sam259.TabStop = False
        Me.Sam259.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam258
        '
        Me.Sam258.BackColor = System.Drawing.Color.Transparent
        Me.Sam258.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam258.Image = Nothing
        Me.Sam258.ImagePath = ""
        Me.Sam258.IsTransparentImage = False
        Me.Sam258.Location = New System.Drawing.Point(511, 244)
        Me.Sam258.Name = "Sam258"
        Me.Sam258.Rotation = 0
        Me.Sam258.ShowThrough = False
        Me.Sam258.Size = New System.Drawing.Size(24, 24)
        Me.Sam258.TabIndex = 240
        Me.Sam258.TabStop = False
        Me.Sam258.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam257
        '
        Me.Sam257.BackColor = System.Drawing.Color.Transparent
        Me.Sam257.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam257.Image = Nothing
        Me.Sam257.ImagePath = ""
        Me.Sam257.IsTransparentImage = False
        Me.Sam257.Location = New System.Drawing.Point(514, 278)
        Me.Sam257.Name = "Sam257"
        Me.Sam257.Rotation = 0
        Me.Sam257.ShowThrough = False
        Me.Sam257.Size = New System.Drawing.Size(24, 24)
        Me.Sam257.TabIndex = 239
        Me.Sam257.TabStop = False
        Me.Sam257.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam256
        '
        Me.Sam256.BackColor = System.Drawing.Color.Transparent
        Me.Sam256.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam256.Image = Nothing
        Me.Sam256.ImagePath = ""
        Me.Sam256.IsTransparentImage = False
        Me.Sam256.Location = New System.Drawing.Point(512, 312)
        Me.Sam256.Name = "Sam256"
        Me.Sam256.Rotation = 0
        Me.Sam256.ShowThrough = False
        Me.Sam256.Size = New System.Drawing.Size(24, 24)
        Me.Sam256.TabIndex = 238
        Me.Sam256.TabStop = False
        Me.Sam256.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam255
        '
        Me.Sam255.BackColor = System.Drawing.Color.Transparent
        Me.Sam255.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam255.Image = Nothing
        Me.Sam255.ImagePath = ""
        Me.Sam255.IsTransparentImage = False
        Me.Sam255.Location = New System.Drawing.Point(506, 345)
        Me.Sam255.Name = "Sam255"
        Me.Sam255.Rotation = 0
        Me.Sam255.ShowThrough = False
        Me.Sam255.Size = New System.Drawing.Size(24, 24)
        Me.Sam255.TabIndex = 237
        Me.Sam255.TabStop = False
        Me.Sam255.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam254
        '
        Me.Sam254.BackColor = System.Drawing.Color.Transparent
        Me.Sam254.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam254.Image = Nothing
        Me.Sam254.ImagePath = ""
        Me.Sam254.IsTransparentImage = False
        Me.Sam254.Location = New System.Drawing.Point(496, 376)
        Me.Sam254.Name = "Sam254"
        Me.Sam254.Rotation = 0
        Me.Sam254.ShowThrough = False
        Me.Sam254.Size = New System.Drawing.Size(24, 24)
        Me.Sam254.TabIndex = 236
        Me.Sam254.TabStop = False
        Me.Sam254.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam253
        '
        Me.Sam253.BackColor = System.Drawing.Color.Transparent
        Me.Sam253.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam253.Image = Nothing
        Me.Sam253.ImagePath = ""
        Me.Sam253.IsTransparentImage = False
        Me.Sam253.Location = New System.Drawing.Point(481, 406)
        Me.Sam253.Name = "Sam253"
        Me.Sam253.Rotation = 0
        Me.Sam253.ShowThrough = False
        Me.Sam253.Size = New System.Drawing.Size(24, 24)
        Me.Sam253.TabIndex = 235
        Me.Sam253.TabStop = False
        Me.Sam253.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam252
        '
        Me.Sam252.BackColor = System.Drawing.Color.Transparent
        Me.Sam252.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam252.Image = Nothing
        Me.Sam252.ImagePath = ""
        Me.Sam252.IsTransparentImage = False
        Me.Sam252.Location = New System.Drawing.Point(462, 434)
        Me.Sam252.Name = "Sam252"
        Me.Sam252.Rotation = 0
        Me.Sam252.ShowThrough = False
        Me.Sam252.Size = New System.Drawing.Size(24, 24)
        Me.Sam252.TabIndex = 234
        Me.Sam252.TabStop = False
        Me.Sam252.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam251
        '
        Me.Sam251.BackColor = System.Drawing.Color.Transparent
        Me.Sam251.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam251.Image = Nothing
        Me.Sam251.ImagePath = ""
        Me.Sam251.IsTransparentImage = False
        Me.Sam251.Location = New System.Drawing.Point(439, 459)
        Me.Sam251.Name = "Sam251"
        Me.Sam251.Rotation = 0
        Me.Sam251.ShowThrough = False
        Me.Sam251.Size = New System.Drawing.Size(24, 24)
        Me.Sam251.TabIndex = 233
        Me.Sam251.TabStop = False
        Me.Sam251.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam250
        '
        Me.Sam250.BackColor = System.Drawing.Color.Transparent
        Me.Sam250.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam250.Image = Nothing
        Me.Sam250.ImagePath = ""
        Me.Sam250.IsTransparentImage = False
        Me.Sam250.Location = New System.Drawing.Point(414, 480)
        Me.Sam250.Name = "Sam250"
        Me.Sam250.Rotation = 0
        Me.Sam250.ShowThrough = False
        Me.Sam250.Size = New System.Drawing.Size(24, 24)
        Me.Sam250.TabIndex = 232
        Me.Sam250.TabStop = False
        Me.Sam250.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam249
        '
        Me.Sam249.BackColor = System.Drawing.Color.Transparent
        Me.Sam249.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam249.Image = Nothing
        Me.Sam249.ImagePath = ""
        Me.Sam249.IsTransparentImage = False
        Me.Sam249.Location = New System.Drawing.Point(386, 497)
        Me.Sam249.Name = "Sam249"
        Me.Sam249.Rotation = 0
        Me.Sam249.ShowThrough = False
        Me.Sam249.Size = New System.Drawing.Size(24, 24)
        Me.Sam249.TabIndex = 231
        Me.Sam249.TabStop = False
        Me.Sam249.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam248
        '
        Me.Sam248.BackColor = System.Drawing.Color.Transparent
        Me.Sam248.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam248.Image = Nothing
        Me.Sam248.ImagePath = ""
        Me.Sam248.IsTransparentImage = False
        Me.Sam248.Location = New System.Drawing.Point(355, 511)
        Me.Sam248.Name = "Sam248"
        Me.Sam248.Rotation = 0
        Me.Sam248.ShowThrough = False
        Me.Sam248.Size = New System.Drawing.Size(24, 24)
        Me.Sam248.TabIndex = 230
        Me.Sam248.TabStop = False
        Me.Sam248.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam247
        '
        Me.Sam247.BackColor = System.Drawing.Color.Transparent
        Me.Sam247.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam247.Image = Nothing
        Me.Sam247.ImagePath = ""
        Me.Sam247.IsTransparentImage = False
        Me.Sam247.Location = New System.Drawing.Point(323, 519)
        Me.Sam247.Name = "Sam247"
        Me.Sam247.Rotation = 0
        Me.Sam247.ShowThrough = False
        Me.Sam247.Size = New System.Drawing.Size(24, 24)
        Me.Sam247.TabIndex = 229
        Me.Sam247.TabStop = False
        Me.Sam247.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam246
        '
        Me.Sam246.BackColor = System.Drawing.Color.Transparent
        Me.Sam246.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam246.Image = Nothing
        Me.Sam246.ImagePath = ""
        Me.Sam246.IsTransparentImage = False
        Me.Sam246.Location = New System.Drawing.Point(291, 523)
        Me.Sam246.Name = "Sam246"
        Me.Sam246.Rotation = 0
        Me.Sam246.ShowThrough = False
        Me.Sam246.Size = New System.Drawing.Size(24, 24)
        Me.Sam246.TabIndex = 228
        Me.Sam246.TabStop = False
        Me.Sam246.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam145
        '
        Me.Sam145.BackColor = System.Drawing.Color.Transparent
        Me.Sam145.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam145.Image = Nothing
        Me.Sam145.ImagePath = ""
        Me.Sam145.IsTransparentImage = False
        Me.Sam145.Location = New System.Drawing.Point(269, 553)
        Me.Sam145.Name = "Sam145"
        Me.Sam145.Rotation = 0
        Me.Sam145.ShowThrough = False
        Me.Sam145.Size = New System.Drawing.Size(24, 24)
        Me.Sam145.TabIndex = 181
        Me.Sam145.TabStop = False
        Me.Sam145.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam144
        '
        Me.Sam144.BackColor = System.Drawing.Color.Transparent
        Me.Sam144.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam144.Image = Nothing
        Me.Sam144.ImagePath = ""
        Me.Sam144.IsTransparentImage = False
        Me.Sam144.Location = New System.Drawing.Point(232, 549)
        Me.Sam144.Name = "Sam144"
        Me.Sam144.Rotation = 0
        Me.Sam144.ShowThrough = False
        Me.Sam144.Size = New System.Drawing.Size(24, 24)
        Me.Sam144.TabIndex = 180
        Me.Sam144.TabStop = False
        Me.Sam144.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam143
        '
        Me.Sam143.BackColor = System.Drawing.Color.Transparent
        Me.Sam143.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam143.Image = Nothing
        Me.Sam143.ImagePath = ""
        Me.Sam143.IsTransparentImage = False
        Me.Sam143.Location = New System.Drawing.Point(196, 540)
        Me.Sam143.Name = "Sam143"
        Me.Sam143.Rotation = 0
        Me.Sam143.ShowThrough = False
        Me.Sam143.Size = New System.Drawing.Size(24, 24)
        Me.Sam143.TabIndex = 179
        Me.Sam143.TabStop = False
        Me.Sam143.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam142
        '
        Me.Sam142.BackColor = System.Drawing.Color.Transparent
        Me.Sam142.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam142.Image = Nothing
        Me.Sam142.ImagePath = ""
        Me.Sam142.IsTransparentImage = False
        Me.Sam142.Location = New System.Drawing.Point(162, 526)
        Me.Sam142.Name = "Sam142"
        Me.Sam142.Rotation = 0
        Me.Sam142.ShowThrough = False
        Me.Sam142.Size = New System.Drawing.Size(24, 24)
        Me.Sam142.TabIndex = 178
        Me.Sam142.TabStop = False
        Me.Sam142.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam11
        '
        Me.Sam11.BackColor = System.Drawing.Color.Transparent
        Me.Sam11.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam11.Image = Nothing
        Me.Sam11.ImagePath = ""
        Me.Sam11.IsTransparentImage = False
        Me.Sam11.Location = New System.Drawing.Point(306, 552)
        Me.Sam11.Name = "Sam11"
        Me.Sam11.Rotation = 0
        Me.Sam11.ShowThrough = False
        Me.Sam11.Size = New System.Drawing.Size(24, 24)
        Me.Sam11.TabIndex = 177
        Me.Sam11.TabStop = False
        Me.Sam11.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam141
        '
        Me.Sam141.BackColor = System.Drawing.Color.Transparent
        Me.Sam141.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam141.Image = Nothing
        Me.Sam141.ImagePath = ""
        Me.Sam141.IsTransparentImage = False
        Me.Sam141.Location = New System.Drawing.Point(130, 507)
        Me.Sam141.Name = "Sam141"
        Me.Sam141.Rotation = 0
        Me.Sam141.ShowThrough = False
        Me.Sam141.Size = New System.Drawing.Size(24, 24)
        Me.Sam141.TabIndex = 176
        Me.Sam141.TabStop = False
        Me.Sam141.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam140
        '
        Me.Sam140.BackColor = System.Drawing.Color.Transparent
        Me.Sam140.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam140.Image = Nothing
        Me.Sam140.ImagePath = ""
        Me.Sam140.IsTransparentImage = False
        Me.Sam140.Location = New System.Drawing.Point(101, 484)
        Me.Sam140.Name = "Sam140"
        Me.Sam140.Rotation = 0
        Me.Sam140.ShowThrough = False
        Me.Sam140.Size = New System.Drawing.Size(24, 24)
        Me.Sam140.TabIndex = 175
        Me.Sam140.TabStop = False
        Me.Sam140.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam139
        '
        Me.Sam139.BackColor = System.Drawing.Color.Transparent
        Me.Sam139.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam139.Image = Nothing
        Me.Sam139.ImagePath = ""
        Me.Sam139.IsTransparentImage = False
        Me.Sam139.Location = New System.Drawing.Point(75, 457)
        Me.Sam139.Name = "Sam139"
        Me.Sam139.Rotation = 0
        Me.Sam139.ShowThrough = False
        Me.Sam139.Size = New System.Drawing.Size(24, 24)
        Me.Sam139.TabIndex = 174
        Me.Sam139.TabStop = False
        Me.Sam139.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam138
        '
        Me.Sam138.BackColor = System.Drawing.Color.Transparent
        Me.Sam138.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam138.Image = Nothing
        Me.Sam138.ImagePath = ""
        Me.Sam138.IsTransparentImage = False
        Me.Sam138.Location = New System.Drawing.Point(53, 426)
        Me.Sam138.Name = "Sam138"
        Me.Sam138.Rotation = 0
        Me.Sam138.ShowThrough = False
        Me.Sam138.Size = New System.Drawing.Size(24, 24)
        Me.Sam138.TabIndex = 173
        Me.Sam138.TabStop = False
        Me.Sam138.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam137
        '
        Me.Sam137.BackColor = System.Drawing.Color.Transparent
        Me.Sam137.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam137.Image = Nothing
        Me.Sam137.ImagePath = ""
        Me.Sam137.IsTransparentImage = False
        Me.Sam137.Location = New System.Drawing.Point(35, 393)
        Me.Sam137.Name = "Sam137"
        Me.Sam137.Rotation = 0
        Me.Sam137.ShowThrough = False
        Me.Sam137.Size = New System.Drawing.Size(24, 24)
        Me.Sam137.TabIndex = 172
        Me.Sam137.TabStop = False
        Me.Sam137.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam136
        '
        Me.Sam136.BackColor = System.Drawing.Color.Transparent
        Me.Sam136.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam136.Image = Nothing
        Me.Sam136.ImagePath = ""
        Me.Sam136.IsTransparentImage = False
        Me.Sam136.Location = New System.Drawing.Point(23, 358)
        Me.Sam136.Name = "Sam136"
        Me.Sam136.Rotation = 0
        Me.Sam136.ShowThrough = False
        Me.Sam136.Size = New System.Drawing.Size(24, 24)
        Me.Sam136.TabIndex = 171
        Me.Sam136.TabStop = False
        Me.Sam136.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam135
        '
        Me.Sam135.BackColor = System.Drawing.Color.Transparent
        Me.Sam135.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam135.Image = Nothing
        Me.Sam135.ImagePath = ""
        Me.Sam135.IsTransparentImage = False
        Me.Sam135.Location = New System.Drawing.Point(15, 321)
        Me.Sam135.Name = "Sam135"
        Me.Sam135.Rotation = 0
        Me.Sam135.ShowThrough = False
        Me.Sam135.Size = New System.Drawing.Size(24, 24)
        Me.Sam135.TabIndex = 170
        Me.Sam135.TabStop = False
        Me.Sam135.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam134
        '
        Me.Sam134.BackColor = System.Drawing.Color.Transparent
        Me.Sam134.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam134.Image = Nothing
        Me.Sam134.ImagePath = ""
        Me.Sam134.IsTransparentImage = False
        Me.Sam134.Location = New System.Drawing.Point(12, 284)
        Me.Sam134.Name = "Sam134"
        Me.Sam134.Rotation = 0
        Me.Sam134.ShowThrough = False
        Me.Sam134.Size = New System.Drawing.Size(24, 24)
        Me.Sam134.TabIndex = 169
        Me.Sam134.TabStop = False
        Me.Sam134.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam133
        '
        Me.Sam133.BackColor = System.Drawing.Color.Transparent
        Me.Sam133.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam133.Image = Nothing
        Me.Sam133.ImagePath = ""
        Me.Sam133.IsTransparentImage = False
        Me.Sam133.Location = New System.Drawing.Point(15, 246)
        Me.Sam133.Name = "Sam133"
        Me.Sam133.Rotation = 0
        Me.Sam133.ShowThrough = False
        Me.Sam133.Size = New System.Drawing.Size(24, 24)
        Me.Sam133.TabIndex = 168
        Me.Sam133.TabStop = False
        Me.Sam133.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam132
        '
        Me.Sam132.BackColor = System.Drawing.Color.Transparent
        Me.Sam132.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam132.Image = Nothing
        Me.Sam132.ImagePath = ""
        Me.Sam132.IsTransparentImage = False
        Me.Sam132.Location = New System.Drawing.Point(22, 209)
        Me.Sam132.Name = "Sam132"
        Me.Sam132.Rotation = 0
        Me.Sam132.ShowThrough = False
        Me.Sam132.Size = New System.Drawing.Size(24, 24)
        Me.Sam132.TabIndex = 167
        Me.Sam132.TabStop = False
        Me.Sam132.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam131
        '
        Me.Sam131.BackColor = System.Drawing.Color.Transparent
        Me.Sam131.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam131.Image = Nothing
        Me.Sam131.ImagePath = ""
        Me.Sam131.IsTransparentImage = False
        Me.Sam131.Location = New System.Drawing.Point(35, 174)
        Me.Sam131.Name = "Sam131"
        Me.Sam131.Rotation = 0
        Me.Sam131.ShowThrough = False
        Me.Sam131.Size = New System.Drawing.Size(24, 24)
        Me.Sam131.TabIndex = 166
        Me.Sam131.TabStop = False
        Me.Sam131.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam130
        '
        Me.Sam130.BackColor = System.Drawing.Color.Transparent
        Me.Sam130.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam130.Image = Nothing
        Me.Sam130.ImagePath = ""
        Me.Sam130.IsTransparentImage = False
        Me.Sam130.Location = New System.Drawing.Point(52, 140)
        Me.Sam130.Name = "Sam130"
        Me.Sam130.Rotation = 0
        Me.Sam130.ShowThrough = False
        Me.Sam130.Size = New System.Drawing.Size(24, 24)
        Me.Sam130.TabIndex = 165
        Me.Sam130.TabStop = False
        Me.Sam130.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam129
        '
        Me.Sam129.BackColor = System.Drawing.Color.Transparent
        Me.Sam129.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam129.Image = Nothing
        Me.Sam129.ImagePath = ""
        Me.Sam129.IsTransparentImage = False
        Me.Sam129.Location = New System.Drawing.Point(74, 110)
        Me.Sam129.Name = "Sam129"
        Me.Sam129.Rotation = 0
        Me.Sam129.ShowThrough = False
        Me.Sam129.Size = New System.Drawing.Size(24, 24)
        Me.Sam129.TabIndex = 164
        Me.Sam129.TabStop = False
        Me.Sam129.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam128
        '
        Me.Sam128.BackColor = System.Drawing.Color.Transparent
        Me.Sam128.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam128.Image = Nothing
        Me.Sam128.ImagePath = ""
        Me.Sam128.IsTransparentImage = False
        Me.Sam128.Location = New System.Drawing.Point(100, 82)
        Me.Sam128.Name = "Sam128"
        Me.Sam128.Rotation = 0
        Me.Sam128.ShowThrough = False
        Me.Sam128.Size = New System.Drawing.Size(24, 24)
        Me.Sam128.TabIndex = 163
        Me.Sam128.TabStop = False
        Me.Sam128.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam127
        '
        Me.Sam127.BackColor = System.Drawing.Color.Transparent
        Me.Sam127.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam127.Image = Nothing
        Me.Sam127.ImagePath = ""
        Me.Sam127.IsTransparentImage = False
        Me.Sam127.Location = New System.Drawing.Point(129, 59)
        Me.Sam127.Name = "Sam127"
        Me.Sam127.Rotation = 0
        Me.Sam127.ShowThrough = False
        Me.Sam127.Size = New System.Drawing.Size(24, 24)
        Me.Sam127.TabIndex = 162
        Me.Sam127.TabStop = False
        Me.Sam127.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam126
        '
        Me.Sam126.BackColor = System.Drawing.Color.Transparent
        Me.Sam126.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam126.Image = Nothing
        Me.Sam126.ImagePath = ""
        Me.Sam126.IsTransparentImage = False
        Me.Sam126.Location = New System.Drawing.Point(161, 40)
        Me.Sam126.Name = "Sam126"
        Me.Sam126.Rotation = 0
        Me.Sam126.ShowThrough = False
        Me.Sam126.Size = New System.Drawing.Size(24, 24)
        Me.Sam126.TabIndex = 160
        Me.Sam126.TabStop = False
        Me.Sam126.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam125
        '
        Me.Sam125.BackColor = System.Drawing.Color.Transparent
        Me.Sam125.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam125.Image = Nothing
        Me.Sam125.ImagePath = ""
        Me.Sam125.IsTransparentImage = False
        Me.Sam125.Location = New System.Drawing.Point(195, 26)
        Me.Sam125.Name = "Sam125"
        Me.Sam125.Rotation = 0
        Me.Sam125.ShowThrough = False
        Me.Sam125.Size = New System.Drawing.Size(24, 24)
        Me.Sam125.TabIndex = 159
        Me.Sam125.TabStop = False
        Me.Sam125.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam124
        '
        Me.Sam124.BackColor = System.Drawing.Color.Transparent
        Me.Sam124.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam124.Image = Nothing
        Me.Sam124.ImagePath = ""
        Me.Sam124.IsTransparentImage = False
        Me.Sam124.Location = New System.Drawing.Point(231, 17)
        Me.Sam124.Name = "Sam124"
        Me.Sam124.Rotation = 0
        Me.Sam124.ShowThrough = False
        Me.Sam124.Size = New System.Drawing.Size(24, 24)
        Me.Sam124.TabIndex = 158
        Me.Sam124.TabStop = False
        Me.Sam124.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam123
        '
        Me.Sam123.BackColor = System.Drawing.Color.Transparent
        Me.Sam123.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam123.Image = Nothing
        Me.Sam123.ImagePath = ""
        Me.Sam123.IsTransparentImage = False
        Me.Sam123.Location = New System.Drawing.Point(268, 13)
        Me.Sam123.Name = "Sam123"
        Me.Sam123.Rotation = 0
        Me.Sam123.ShowThrough = False
        Me.Sam123.Size = New System.Drawing.Size(24, 24)
        Me.Sam123.TabIndex = 157
        Me.Sam123.TabStop = False
        Me.Sam123.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam122
        '
        Me.Sam122.BackColor = System.Drawing.Color.Transparent
        Me.Sam122.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam122.Image = Nothing
        Me.Sam122.ImagePath = ""
        Me.Sam122.IsTransparentImage = False
        Me.Sam122.Location = New System.Drawing.Point(305, 14)
        Me.Sam122.Name = "Sam122"
        Me.Sam122.Rotation = 0
        Me.Sam122.ShowThrough = False
        Me.Sam122.Size = New System.Drawing.Size(24, 24)
        Me.Sam122.TabIndex = 156
        Me.Sam122.TabStop = False
        Me.Sam122.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam121
        '
        Me.Sam121.BackColor = System.Drawing.Color.Transparent
        Me.Sam121.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam121.Image = Nothing
        Me.Sam121.ImagePath = ""
        Me.Sam121.IsTransparentImage = False
        Me.Sam121.Location = New System.Drawing.Point(342, 20)
        Me.Sam121.Name = "Sam121"
        Me.Sam121.Rotation = 0
        Me.Sam121.ShowThrough = False
        Me.Sam121.Size = New System.Drawing.Size(24, 24)
        Me.Sam121.TabIndex = 155
        Me.Sam121.TabStop = False
        Me.Sam121.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam120
        '
        Me.Sam120.BackColor = System.Drawing.Color.Transparent
        Me.Sam120.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam120.Image = Nothing
        Me.Sam120.ImagePath = ""
        Me.Sam120.IsTransparentImage = False
        Me.Sam120.Location = New System.Drawing.Point(377, 32)
        Me.Sam120.Name = "Sam120"
        Me.Sam120.Rotation = 0
        Me.Sam120.ShowThrough = False
        Me.Sam120.Size = New System.Drawing.Size(24, 24)
        Me.Sam120.TabIndex = 154
        Me.Sam120.TabStop = False
        Me.Sam120.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam119
        '
        Me.Sam119.BackColor = System.Drawing.Color.Transparent
        Me.Sam119.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam119.Image = Nothing
        Me.Sam119.ImagePath = ""
        Me.Sam119.IsTransparentImage = False
        Me.Sam119.Location = New System.Drawing.Point(410, 48)
        Me.Sam119.Name = "Sam119"
        Me.Sam119.Rotation = 0
        Me.Sam119.ShowThrough = False
        Me.Sam119.Size = New System.Drawing.Size(24, 24)
        Me.Sam119.TabIndex = 153
        Me.Sam119.TabStop = False
        Me.Sam119.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam118
        '
        Me.Sam118.BackColor = System.Drawing.Color.Transparent
        Me.Sam118.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam118.Image = Nothing
        Me.Sam118.ImagePath = ""
        Me.Sam118.IsTransparentImage = False
        Me.Sam118.Location = New System.Drawing.Point(441, 69)
        Me.Sam118.Name = "Sam118"
        Me.Sam118.Rotation = 0
        Me.Sam118.ShowThrough = False
        Me.Sam118.Size = New System.Drawing.Size(24, 24)
        Me.Sam118.TabIndex = 152
        Me.Sam118.TabStop = False
        Me.Sam118.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam117
        '
        Me.Sam117.BackColor = System.Drawing.Color.Transparent
        Me.Sam117.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam117.Image = Nothing
        Me.Sam117.ImagePath = ""
        Me.Sam117.IsTransparentImage = False
        Me.Sam117.Location = New System.Drawing.Point(469, 95)
        Me.Sam117.Name = "Sam117"
        Me.Sam117.Rotation = 0
        Me.Sam117.ShowThrough = False
        Me.Sam117.Size = New System.Drawing.Size(24, 24)
        Me.Sam117.TabIndex = 151
        Me.Sam117.TabStop = False
        Me.Sam117.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam116
        '
        Me.Sam116.BackColor = System.Drawing.Color.Transparent
        Me.Sam116.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam116.Image = Nothing
        Me.Sam116.ImagePath = ""
        Me.Sam116.IsTransparentImage = False
        Me.Sam116.Location = New System.Drawing.Point(492, 124)
        Me.Sam116.Name = "Sam116"
        Me.Sam116.Rotation = 0
        Me.Sam116.ShowThrough = False
        Me.Sam116.Size = New System.Drawing.Size(24, 24)
        Me.Sam116.TabIndex = 150
        Me.Sam116.TabStop = False
        Me.Sam116.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam115
        '
        Me.Sam115.BackColor = System.Drawing.Color.Transparent
        Me.Sam115.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam115.Image = Nothing
        Me.Sam115.ImagePath = ""
        Me.Sam115.IsTransparentImage = False
        Me.Sam115.Location = New System.Drawing.Point(512, 155)
        Me.Sam115.Name = "Sam115"
        Me.Sam115.Rotation = 0
        Me.Sam115.ShowThrough = False
        Me.Sam115.Size = New System.Drawing.Size(24, 24)
        Me.Sam115.TabIndex = 149
        Me.Sam115.TabStop = False
        Me.Sam115.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam114
        '
        Me.Sam114.BackColor = System.Drawing.Color.Transparent
        Me.Sam114.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam114.Image = Nothing
        Me.Sam114.ImagePath = ""
        Me.Sam114.IsTransparentImage = False
        Me.Sam114.Location = New System.Drawing.Point(527, 190)
        Me.Sam114.Name = "Sam114"
        Me.Sam114.Rotation = 0
        Me.Sam114.ShowThrough = False
        Me.Sam114.Size = New System.Drawing.Size(24, 24)
        Me.Sam114.TabIndex = 148
        Me.Sam114.TabStop = False
        Me.Sam114.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam113
        '
        Me.Sam113.BackColor = System.Drawing.Color.Transparent
        Me.Sam113.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam113.Image = Nothing
        Me.Sam113.ImagePath = ""
        Me.Sam113.IsTransparentImage = False
        Me.Sam113.Location = New System.Drawing.Point(537, 226)
        Me.Sam113.Name = "Sam113"
        Me.Sam113.Rotation = 0
        Me.Sam113.ShowThrough = False
        Me.Sam113.Size = New System.Drawing.Size(24, 24)
        Me.Sam113.TabIndex = 147
        Me.Sam113.TabStop = False
        Me.Sam113.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam112
        '
        Me.Sam112.BackColor = System.Drawing.Color.Transparent
        Me.Sam112.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam112.Image = Nothing
        Me.Sam112.ImagePath = ""
        Me.Sam112.IsTransparentImage = False
        Me.Sam112.Location = New System.Drawing.Point(543, 264)
        Me.Sam112.Name = "Sam112"
        Me.Sam112.Rotation = 0
        Me.Sam112.ShowThrough = False
        Me.Sam112.Size = New System.Drawing.Size(24, 24)
        Me.Sam112.TabIndex = 146
        Me.Sam112.TabStop = False
        Me.Sam112.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam111
        '
        Me.Sam111.BackColor = System.Drawing.Color.Transparent
        Me.Sam111.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam111.Image = Nothing
        Me.Sam111.ImagePath = ""
        Me.Sam111.IsTransparentImage = False
        Me.Sam111.Location = New System.Drawing.Point(543, 301)
        Me.Sam111.Name = "Sam111"
        Me.Sam111.Rotation = 0
        Me.Sam111.ShowThrough = False
        Me.Sam111.Size = New System.Drawing.Size(24, 24)
        Me.Sam111.TabIndex = 145
        Me.Sam111.TabStop = False
        Me.Sam111.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam110
        '
        Me.Sam110.BackColor = System.Drawing.Color.Transparent
        Me.Sam110.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam110.Image = Nothing
        Me.Sam110.ImagePath = ""
        Me.Sam110.IsTransparentImage = False
        Me.Sam110.Location = New System.Drawing.Point(537, 339)
        Me.Sam110.Name = "Sam110"
        Me.Sam110.Rotation = 0
        Me.Sam110.ShowThrough = False
        Me.Sam110.Size = New System.Drawing.Size(24, 24)
        Me.Sam110.TabIndex = 144
        Me.Sam110.TabStop = False
        Me.Sam110.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam19
        '
        Me.Sam19.BackColor = System.Drawing.Color.Transparent
        Me.Sam19.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam19.Image = Nothing
        Me.Sam19.ImagePath = ""
        Me.Sam19.IsTransparentImage = False
        Me.Sam19.Location = New System.Drawing.Point(527, 375)
        Me.Sam19.Name = "Sam19"
        Me.Sam19.Rotation = 0
        Me.Sam19.ShowThrough = False
        Me.Sam19.Size = New System.Drawing.Size(24, 24)
        Me.Sam19.TabIndex = 143
        Me.Sam19.TabStop = False
        Me.Sam19.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam18
        '
        Me.Sam18.BackColor = System.Drawing.Color.Transparent
        Me.Sam18.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam18.Image = Nothing
        Me.Sam18.ImagePath = ""
        Me.Sam18.IsTransparentImage = False
        Me.Sam18.Location = New System.Drawing.Point(513, 409)
        Me.Sam18.Name = "Sam18"
        Me.Sam18.Rotation = 0
        Me.Sam18.ShowThrough = False
        Me.Sam18.Size = New System.Drawing.Size(24, 24)
        Me.Sam18.TabIndex = 142
        Me.Sam18.TabStop = False
        Me.Sam18.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam17
        '
        Me.Sam17.BackColor = System.Drawing.Color.Transparent
        Me.Sam17.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam17.Image = Nothing
        Me.Sam17.ImagePath = ""
        Me.Sam17.IsTransparentImage = False
        Me.Sam17.Location = New System.Drawing.Point(493, 441)
        Me.Sam17.Name = "Sam17"
        Me.Sam17.Rotation = 0
        Me.Sam17.ShowThrough = False
        Me.Sam17.Size = New System.Drawing.Size(24, 24)
        Me.Sam17.TabIndex = 140
        Me.Sam17.TabStop = False
        Me.Sam17.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam16
        '
        Me.Sam16.BackColor = System.Drawing.Color.Transparent
        Me.Sam16.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam16.Image = Nothing
        Me.Sam16.ImagePath = ""
        Me.Sam16.IsTransparentImage = False
        Me.Sam16.Location = New System.Drawing.Point(469, 470)
        Me.Sam16.Name = "Sam16"
        Me.Sam16.Rotation = 0
        Me.Sam16.ShowThrough = False
        Me.Sam16.Size = New System.Drawing.Size(24, 24)
        Me.Sam16.TabIndex = 139
        Me.Sam16.TabStop = False
        Me.Sam16.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam15
        '
        Me.Sam15.BackColor = System.Drawing.Color.Transparent
        Me.Sam15.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam15.Image = Nothing
        Me.Sam15.ImagePath = ""
        Me.Sam15.IsTransparentImage = False
        Me.Sam15.Location = New System.Drawing.Point(442, 496)
        Me.Sam15.Name = "Sam15"
        Me.Sam15.Rotation = 0
        Me.Sam15.ShowThrough = False
        Me.Sam15.Size = New System.Drawing.Size(24, 24)
        Me.Sam15.TabIndex = 138
        Me.Sam15.TabStop = False
        Me.Sam15.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam14
        '
        Me.Sam14.BackColor = System.Drawing.Color.Transparent
        Me.Sam14.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam14.Image = Nothing
        Me.Sam14.ImagePath = ""
        Me.Sam14.IsTransparentImage = False
        Me.Sam14.Location = New System.Drawing.Point(412, 516)
        Me.Sam14.Name = "Sam14"
        Me.Sam14.Rotation = 0
        Me.Sam14.ShowThrough = False
        Me.Sam14.Size = New System.Drawing.Size(24, 24)
        Me.Sam14.TabIndex = 137
        Me.Sam14.TabStop = False
        Me.Sam14.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam13
        '
        Me.Sam13.BackColor = System.Drawing.Color.Transparent
        Me.Sam13.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam13.Image = Nothing
        Me.Sam13.ImagePath = ""
        Me.Sam13.IsTransparentImage = False
        Me.Sam13.Location = New System.Drawing.Point(378, 533)
        Me.Sam13.Name = "Sam13"
        Me.Sam13.Rotation = 0
        Me.Sam13.ShowThrough = False
        Me.Sam13.Size = New System.Drawing.Size(24, 24)
        Me.Sam13.TabIndex = 136
        Me.Sam13.TabStop = False
        Me.Sam13.TransparentColor = System.Drawing.Color.LightPink
        '
        'Sam12
        '
        Me.Sam12.BackColor = System.Drawing.Color.Transparent
        Me.Sam12.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Sam12.Image = Nothing
        Me.Sam12.ImagePath = ""
        Me.Sam12.IsTransparentImage = False
        Me.Sam12.Location = New System.Drawing.Point(343, 545)
        Me.Sam12.Name = "Sam12"
        Me.Sam12.Rotation = 0
        Me.Sam12.ShowThrough = False
        Me.Sam12.Size = New System.Drawing.Size(24, 24)
        Me.Sam12.TabIndex = 135
        Me.Sam12.TabStop = False
        Me.Sam12.TransparentColor = System.Drawing.Color.LightPink
        '
        'ReagentsTab
        '
        Me.ReagentsTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ReagentsTab.Appearance.PageClient.Image = CType(resources.GetObject("ReagentsTab.Appearance.PageClient.Image"), System.Drawing.Image)
        Me.ReagentsTab.Appearance.PageClient.Options.UseBackColor = True
        Me.ReagentsTab.Appearance.PageClient.Options.UseImage = True
        Me.ReagentsTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ReagentsTab.Controls.Add(Me.PanelControl6)
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
        Me.ReagentsTab.Controls.Add(Me.Reag245)
        Me.ReagentsTab.Controls.Add(Me.Reag246)
        Me.ReagentsTab.Controls.Add(Me.Reag247)
        Me.ReagentsTab.Controls.Add(Me.Reag248)
        Me.ReagentsTab.Controls.Add(Me.Reag249)
        Me.ReagentsTab.Controls.Add(Me.Reag250)
        Me.ReagentsTab.Controls.Add(Me.Reag251)
        Me.ReagentsTab.Controls.Add(Me.Reag252)
        Me.ReagentsTab.Controls.Add(Me.Reag253)
        Me.ReagentsTab.Controls.Add(Me.Reag254)
        Me.ReagentsTab.Controls.Add(Me.Reag255)
        Me.ReagentsTab.Controls.Add(Me.Reag256)
        Me.ReagentsTab.Controls.Add(Me.Reag257)
        Me.ReagentsTab.Controls.Add(Me.Reag258)
        Me.ReagentsTab.Controls.Add(Me.Reag259)
        Me.ReagentsTab.Controls.Add(Me.Reag260)
        Me.ReagentsTab.Controls.Add(Me.Reag261)
        Me.ReagentsTab.Controls.Add(Me.Reag262)
        Me.ReagentsTab.Controls.Add(Me.Reag263)
        Me.ReagentsTab.Controls.Add(Me.Reag264)
        Me.ReagentsTab.Controls.Add(Me.Reag265)
        Me.ReagentsTab.Controls.Add(Me.Reag266)
        Me.ReagentsTab.Controls.Add(Me.Reag267)
        Me.ReagentsTab.Controls.Add(Me.Reag268)
        Me.ReagentsTab.Controls.Add(Me.Reag269)
        Me.ReagentsTab.Controls.Add(Me.Reag270)
        Me.ReagentsTab.Controls.Add(Me.Reag271)
        Me.ReagentsTab.Controls.Add(Me.Reag272)
        Me.ReagentsTab.Controls.Add(Me.Reag273)
        Me.ReagentsTab.Controls.Add(Me.Reag274)
        Me.ReagentsTab.Controls.Add(Me.Reag275)
        Me.ReagentsTab.Controls.Add(Me.Reag276)
        Me.ReagentsTab.Controls.Add(Me.Reag277)
        Me.ReagentsTab.Controls.Add(Me.Reag278)
        Me.ReagentsTab.Controls.Add(Me.Reag279)
        Me.ReagentsTab.Controls.Add(Me.Reag280)
        Me.ReagentsTab.Controls.Add(Me.Reag281)
        Me.ReagentsTab.Controls.Add(Me.Reag282)
        Me.ReagentsTab.Controls.Add(Me.Reag283)
        Me.ReagentsTab.Controls.Add(Me.Reag284)
        Me.ReagentsTab.Controls.Add(Me.Reag285)
        Me.ReagentsTab.Controls.Add(Me.Reag286)
        Me.ReagentsTab.Controls.Add(Me.Reag287)
        Me.ReagentsTab.Controls.Add(Me.Reag288)
        Me.ReagentsTab.Name = "ReagentsTab"
        Me.ReagentsTab.Size = New System.Drawing.Size(762, 588)
        Me.ReagentsTab.Text = "Reagents"
        '
        'PanelControl6
        '
        Me.PanelControl6.Appearance.BackColor = System.Drawing.Color.White
        Me.PanelControl6.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl6.Appearance.Options.UseBackColor = True
        Me.PanelControl6.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl6.Controls.Add(Me.PanelControl7)
        Me.PanelControl6.Location = New System.Drawing.Point(575, -4)
        Me.PanelControl6.Name = "PanelControl6"
        Me.PanelControl6.Size = New System.Drawing.Size(190, 598)
        Me.PanelControl6.TabIndex = 319
        '
        'PanelControl7
        '
        Me.PanelControl7.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.PanelControl7.Appearance.BackColor2 = System.Drawing.Color.Gainsboro
        Me.PanelControl7.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.PanelControl7.Appearance.Options.UseBackColor = True
        Me.PanelControl7.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.PanelControl7.Controls.Add(Me.bsReagentsLegendGroupBox)
        Me.PanelControl7.Controls.Add(Me.bsReagentsPositionInfoGroupBox)
        Me.PanelControl7.Location = New System.Drawing.Point(3, 4)
        Me.PanelControl7.LookAndFeel.UseDefaultLookAndFeel = False
        Me.PanelControl7.Name = "PanelControl7"
        Me.PanelControl7.Size = New System.Drawing.Size(183, 588)
        Me.PanelControl7.TabIndex = 262
        '
        'bsReagentsLegendGroupBox
        '
        Me.bsReagentsLegendGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.LegReagentSelLabel)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.SelectedPictureBox)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.LegendUnknownImage)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.bsUnknownLabel)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.LegendBarCodeErrorRGImage)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.bsBarcodeErrorRGLabel)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.LowVolPictureBox)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.ReagentPictureBox)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.bsLegReagLowVolLabel)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.bsLegReagentLabel)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.bsLegReagAdditionalSol)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.bsLegReagNoInUseLabel)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.bsLegReagDepleteLabel)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.AdditionalSolPictureBox)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.NoInUsePictureBox)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.bsDepletedPictureBox)
        Me.bsReagentsLegendGroupBox.Controls.Add(Me.bsReagentsLegendLabel)
        Me.bsReagentsLegendGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReagentsLegendGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsLegendGroupBox.Location = New System.Drawing.Point(4, 395)
        Me.bsReagentsLegendGroupBox.Name = "bsReagentsLegendGroupBox"
        Me.bsReagentsLegendGroupBox.Size = New System.Drawing.Size(175, 188)
        Me.bsReagentsLegendGroupBox.TabIndex = 27
        Me.bsReagentsLegendGroupBox.TabStop = False
        '
        'LegReagentSelLabel
        '
        Me.LegReagentSelLabel.BackColor = System.Drawing.Color.Transparent
        Me.LegReagentSelLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegReagentSelLabel.ForeColor = System.Drawing.Color.Black
        Me.LegReagentSelLabel.Location = New System.Drawing.Point(20, 170)
        Me.LegReagentSelLabel.Name = "LegReagentSelLabel"
        Me.LegReagentSelLabel.Size = New System.Drawing.Size(153, 13)
        Me.LegReagentSelLabel.TabIndex = 55
        Me.LegReagentSelLabel.Text = "*Selected"
        Me.LegReagentSelLabel.Title = False
        '
        'SelectedPictureBox
        '
        Me.SelectedPictureBox.InitialImage = CType(resources.GetObject("SelectedPictureBox.InitialImage"), System.Drawing.Image)
        Me.SelectedPictureBox.Location = New System.Drawing.Point(3, 168)
        Me.SelectedPictureBox.Name = "SelectedPictureBox"
        Me.SelectedPictureBox.PositionNumber = 0
        Me.SelectedPictureBox.Size = New System.Drawing.Size(16, 16)
        Me.SelectedPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.SelectedPictureBox.TabIndex = 54
        Me.SelectedPictureBox.TabStop = False
        '
        'LegendUnknownImage
        '
        Me.LegendUnknownImage.InitialImage = CType(resources.GetObject("LegendUnknownImage.InitialImage"), System.Drawing.Image)
        Me.LegendUnknownImage.Location = New System.Drawing.Point(3, 149)
        Me.LegendUnknownImage.Name = "LegendUnknownImage"
        Me.LegendUnknownImage.PositionNumber = 0
        Me.LegendUnknownImage.Size = New System.Drawing.Size(16, 16)
        Me.LegendUnknownImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.LegendUnknownImage.TabIndex = 53
        Me.LegendUnknownImage.TabStop = False
        '
        'bsUnknownLabel
        '
        Me.bsUnknownLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsUnknownLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsUnknownLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUnknownLabel.Location = New System.Drawing.Point(20, 151)
        Me.bsUnknownLabel.Name = "bsUnknownLabel"
        Me.bsUnknownLabel.Size = New System.Drawing.Size(153, 13)
        Me.bsUnknownLabel.TabIndex = 52
        Me.bsUnknownLabel.Text = "*Unknown"
        Me.bsUnknownLabel.Title = False
        '
        'LegendBarCodeErrorRGImage
        '
        Me.LegendBarCodeErrorRGImage.InitialImage = CType(resources.GetObject("LegendBarCodeErrorRGImage.InitialImage"), System.Drawing.Image)
        Me.LegendBarCodeErrorRGImage.Location = New System.Drawing.Point(3, 130)
        Me.LegendBarCodeErrorRGImage.Name = "LegendBarCodeErrorRGImage"
        Me.LegendBarCodeErrorRGImage.PositionNumber = 0
        Me.LegendBarCodeErrorRGImage.Size = New System.Drawing.Size(16, 16)
        Me.LegendBarCodeErrorRGImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.LegendBarCodeErrorRGImage.TabIndex = 51
        Me.LegendBarCodeErrorRGImage.TabStop = False
        '
        'bsBarcodeErrorRGLabel
        '
        Me.bsBarcodeErrorRGLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsBarcodeErrorRGLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsBarcodeErrorRGLabel.ForeColor = System.Drawing.Color.Black
        Me.bsBarcodeErrorRGLabel.Location = New System.Drawing.Point(20, 132)
        Me.bsBarcodeErrorRGLabel.Name = "bsBarcodeErrorRGLabel"
        Me.bsBarcodeErrorRGLabel.Size = New System.Drawing.Size(153, 13)
        Me.bsBarcodeErrorRGLabel.TabIndex = 50
        Me.bsBarcodeErrorRGLabel.Text = "*Barcode Error"
        Me.bsBarcodeErrorRGLabel.Title = False
        '
        'LowVolPictureBox
        '
        Me.LowVolPictureBox.InitialImage = CType(resources.GetObject("LowVolPictureBox.InitialImage"), System.Drawing.Image)
        Me.LowVolPictureBox.Location = New System.Drawing.Point(3, 92)
        Me.LowVolPictureBox.Name = "LowVolPictureBox"
        Me.LowVolPictureBox.PositionNumber = 0
        Me.LowVolPictureBox.Size = New System.Drawing.Size(16, 16)
        Me.LowVolPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.LowVolPictureBox.TabIndex = 49
        Me.LowVolPictureBox.TabStop = False
        '
        'ReagentPictureBox
        '
        Me.ReagentPictureBox.InitialImage = CType(resources.GetObject("ReagentPictureBox.InitialImage"), System.Drawing.Image)
        Me.ReagentPictureBox.Location = New System.Drawing.Point(3, 35)
        Me.ReagentPictureBox.Name = "ReagentPictureBox"
        Me.ReagentPictureBox.PositionNumber = 0
        Me.ReagentPictureBox.Size = New System.Drawing.Size(16, 16)
        Me.ReagentPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.ReagentPictureBox.TabIndex = 48
        Me.ReagentPictureBox.TabStop = False
        '
        'bsLegReagLowVolLabel
        '
        Me.bsLegReagLowVolLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLegReagLowVolLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLegReagLowVolLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLegReagLowVolLabel.Location = New System.Drawing.Point(20, 94)
        Me.bsLegReagLowVolLabel.Name = "bsLegReagLowVolLabel"
        Me.bsLegReagLowVolLabel.Size = New System.Drawing.Size(153, 13)
        Me.bsLegReagLowVolLabel.TabIndex = 47
        Me.bsLegReagLowVolLabel.Text = "*Low Volume"
        Me.bsLegReagLowVolLabel.Title = False
        '
        'bsLegReagentLabel
        '
        Me.bsLegReagentLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLegReagentLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLegReagentLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLegReagentLabel.Location = New System.Drawing.Point(20, 37)
        Me.bsLegReagentLabel.Name = "bsLegReagentLabel"
        Me.bsLegReagentLabel.Size = New System.Drawing.Size(153, 13)
        Me.bsLegReagentLabel.TabIndex = 46
        Me.bsLegReagentLabel.Text = "*Reagent"
        Me.bsLegReagentLabel.Title = False
        '
        'bsLegReagAdditionalSol
        '
        Me.bsLegReagAdditionalSol.BackColor = System.Drawing.Color.Transparent
        Me.bsLegReagAdditionalSol.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLegReagAdditionalSol.ForeColor = System.Drawing.Color.Black
        Me.bsLegReagAdditionalSol.Location = New System.Drawing.Point(20, 56)
        Me.bsLegReagAdditionalSol.Name = "bsLegReagAdditionalSol"
        Me.bsLegReagAdditionalSol.Size = New System.Drawing.Size(153, 13)
        Me.bsLegReagAdditionalSol.TabIndex = 45
        Me.bsLegReagAdditionalSol.Text = "*Additional Solution"
        Me.bsLegReagAdditionalSol.Title = False
        '
        'bsLegReagNoInUseLabel
        '
        Me.bsLegReagNoInUseLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLegReagNoInUseLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLegReagNoInUseLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLegReagNoInUseLabel.Location = New System.Drawing.Point(20, 113)
        Me.bsLegReagNoInUseLabel.Name = "bsLegReagNoInUseLabel"
        Me.bsLegReagNoInUseLabel.Size = New System.Drawing.Size(153, 13)
        Me.bsLegReagNoInUseLabel.TabIndex = 44
        Me.bsLegReagNoInUseLabel.Text = "*Not In Use"
        Me.bsLegReagNoInUseLabel.Title = False
        '
        'bsLegReagDepleteLabel
        '
        Me.bsLegReagDepleteLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLegReagDepleteLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLegReagDepleteLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLegReagDepleteLabel.Location = New System.Drawing.Point(20, 75)
        Me.bsLegReagDepleteLabel.Name = "bsLegReagDepleteLabel"
        Me.bsLegReagDepleteLabel.Size = New System.Drawing.Size(153, 13)
        Me.bsLegReagDepleteLabel.TabIndex = 40
        Me.bsLegReagDepleteLabel.Text = "*Depleted/Insufficient"
        Me.bsLegReagDepleteLabel.Title = False
        '
        'AdditionalSolPictureBox
        '
        Me.AdditionalSolPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.AdditionalSolPictureBox.ErrorImage = Nothing
        Me.AdditionalSolPictureBox.InitialImage = CType(resources.GetObject("AdditionalSolPictureBox.InitialImage"), System.Drawing.Image)
        Me.AdditionalSolPictureBox.Location = New System.Drawing.Point(3, 54)
        Me.AdditionalSolPictureBox.Name = "AdditionalSolPictureBox"
        Me.AdditionalSolPictureBox.PositionNumber = 0
        Me.AdditionalSolPictureBox.Size = New System.Drawing.Size(16, 16)
        Me.AdditionalSolPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.AdditionalSolPictureBox.TabIndex = 42
        Me.AdditionalSolPictureBox.TabStop = False
        '
        'NoInUsePictureBox
        '
        Me.NoInUsePictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.NoInUsePictureBox.ErrorImage = Nothing
        Me.NoInUsePictureBox.InitialImage = CType(resources.GetObject("NoInUsePictureBox.InitialImage"), System.Drawing.Image)
        Me.NoInUsePictureBox.Location = New System.Drawing.Point(3, 111)
        Me.NoInUsePictureBox.Name = "NoInUsePictureBox"
        Me.NoInUsePictureBox.PositionNumber = 0
        Me.NoInUsePictureBox.Size = New System.Drawing.Size(16, 16)
        Me.NoInUsePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.NoInUsePictureBox.TabIndex = 43
        Me.NoInUsePictureBox.TabStop = False
        '
        'bsDepletedPictureBox
        '
        Me.bsDepletedPictureBox.InitialImage = CType(resources.GetObject("bsDepletedPictureBox.InitialImage"), System.Drawing.Image)
        Me.bsDepletedPictureBox.Location = New System.Drawing.Point(3, 73)
        Me.bsDepletedPictureBox.Name = "bsDepletedPictureBox"
        Me.bsDepletedPictureBox.PositionNumber = 0
        Me.bsDepletedPictureBox.Size = New System.Drawing.Size(16, 16)
        Me.bsDepletedPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsDepletedPictureBox.TabIndex = 41
        Me.bsDepletedPictureBox.TabStop = False
        '
        'bsReagentsLegendLabel
        '
        Me.bsReagentsLegendLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsReagentsLegendLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsReagentsLegendLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsLegendLabel.Location = New System.Drawing.Point(4, 12)
        Me.bsReagentsLegendLabel.Name = "bsReagentsLegendLabel"
        Me.bsReagentsLegendLabel.Size = New System.Drawing.Size(167, 19)
        Me.bsReagentsLegendLabel.TabIndex = 25
        Me.bsReagentsLegendLabel.Text = "Legend"
        Me.bsReagentsLegendLabel.Title = True
        '
        'bsReagentsPositionInfoGroupBox
        '
        Me.bsReagentsPositionInfoGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagStatusLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.ReagStatusTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsCellTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsCellLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsDeletePosButton)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsTeststLeftTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsRefillPosButton)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsCurrentVolTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsCheckVolumePosButton)
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
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsMoveFirstPositionButton)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsContentTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsRingNumTextBox)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsExpirationDateLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsBarCodeLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsTestNameLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsNumberLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentNameLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsContentLabel)
        Me.bsReagentsPositionInfoGroupBox.Controls.Add(Me.bsReagentsRingNumLabel)
        Me.bsReagentsPositionInfoGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReagentsPositionInfoGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsPositionInfoGroupBox.Location = New System.Drawing.Point(4, -3)
        Me.bsReagentsPositionInfoGroupBox.Name = "bsReagentsPositionInfoGroupBox"
        Me.bsReagentsPositionInfoGroupBox.Size = New System.Drawing.Size(175, 399)
        Me.bsReagentsPositionInfoGroupBox.TabIndex = 3
        Me.bsReagentsPositionInfoGroupBox.TabStop = False
        '
        'bsReagStatusLabel
        '
        Me.bsReagStatusLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagStatusLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagStatusLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagStatusLabel.Location = New System.Drawing.Point(3, 298)
        Me.bsReagStatusLabel.Name = "bsReagStatusLabel"
        Me.bsReagStatusLabel.Size = New System.Drawing.Size(163, 13)
        Me.bsReagStatusLabel.TabIndex = 34
        Me.bsReagStatusLabel.Text = "*Status:"
        Me.bsReagStatusLabel.Title = False
        '
        'ReagStatusTextBox
        '
        Me.ReagStatusTextBox.BackColor = System.Drawing.Color.White
        Me.ReagStatusTextBox.DecimalsValues = False
        Me.ReagStatusTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.ReagStatusTextBox.ForeColor = System.Drawing.Color.Black
        Me.ReagStatusTextBox.IsNumeric = False
        Me.ReagStatusTextBox.Location = New System.Drawing.Point(5, 314)
        Me.ReagStatusTextBox.Mandatory = False
        Me.ReagStatusTextBox.Name = "ReagStatusTextBox"
        Me.ReagStatusTextBox.ReadOnly = True
        Me.ReagStatusTextBox.Size = New System.Drawing.Size(164, 21)
        Me.ReagStatusTextBox.TabIndex = 33
        Me.ReagStatusTextBox.WordWrap = False
        '
        'bsReagentsCellTextBox
        '
        Me.bsReagentsCellTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsCellTextBox.DecimalsValues = False
        Me.bsReagentsCellTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsCellTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsCellTextBox.IsNumeric = False
        Me.bsReagentsCellTextBox.Location = New System.Drawing.Point(123, 47)
        Me.bsReagentsCellTextBox.Mandatory = False
        Me.bsReagentsCellTextBox.MaxLength = 3
        Me.bsReagentsCellTextBox.Name = "bsReagentsCellTextBox"
        Me.bsReagentsCellTextBox.ReadOnly = True
        Me.bsReagentsCellTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsReagentsCellTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsReagentsCellTextBox.TabIndex = 32
        Me.bsReagentsCellTextBox.TabStop = False
        Me.bsReagentsCellTextBox.WordWrap = False
        '
        'bsReagentsCellLabel
        '
        Me.bsReagentsCellLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsCellLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsCellLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsCellLabel.Location = New System.Drawing.Point(120, 33)
        Me.bsReagentsCellLabel.Name = "bsReagentsCellLabel"
        Me.bsReagentsCellLabel.Size = New System.Drawing.Size(42, 13)
        Me.bsReagentsCellLabel.TabIndex = 31
        Me.bsReagentsCellLabel.Text = "*Cell:"
        Me.bsReagentsCellLabel.Title = False
        '
        'bsReagentsDeletePosButton
        '
        Me.bsReagentsDeletePosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReagentsDeletePosButton.Location = New System.Drawing.Point(135, 364)
        Me.bsReagentsDeletePosButton.Name = "bsReagentsDeletePosButton"
        Me.bsReagentsDeletePosButton.Size = New System.Drawing.Size(32, 32)
        Me.bsReagentsDeletePosButton.TabIndex = 14
        Me.bsReagentsDeletePosButton.UseVisualStyleBackColor = True
        '
        'bsTeststLeftTextBox
        '
        Me.bsTeststLeftTextBox.BackColor = System.Drawing.Color.White
        Me.bsTeststLeftTextBox.DecimalsValues = False
        Me.bsTeststLeftTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTeststLeftTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsTeststLeftTextBox.IsNumeric = False
        Me.bsTeststLeftTextBox.Location = New System.Drawing.Point(85, 274)
        Me.bsTeststLeftTextBox.Mandatory = False
        Me.bsTeststLeftTextBox.Name = "bsTeststLeftTextBox"
        Me.bsTeststLeftTextBox.ReadOnly = True
        Me.bsTeststLeftTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsTeststLeftTextBox.Size = New System.Drawing.Size(83, 21)
        Me.bsTeststLeftTextBox.TabIndex = 10
        Me.bsTeststLeftTextBox.TabStop = False
        Me.bsTeststLeftTextBox.WordWrap = False
        '
        'bsReagentsRefillPosButton
        '
        Me.bsReagentsRefillPosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsReagentsRefillPosButton.Location = New System.Drawing.Point(6, 364)
        Me.bsReagentsRefillPosButton.Name = "bsReagentsRefillPosButton"
        Me.bsReagentsRefillPosButton.Size = New System.Drawing.Size(32, 32)
        Me.bsReagentsRefillPosButton.TabIndex = 12
        Me.bsReagentsRefillPosButton.UseVisualStyleBackColor = True
        '
        'bsCurrentVolTextBox
        '
        Me.bsCurrentVolTextBox.BackColor = System.Drawing.Color.White
        Me.bsCurrentVolTextBox.DecimalsValues = False
        Me.bsCurrentVolTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCurrentVolTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsCurrentVolTextBox.IsNumeric = False
        Me.bsCurrentVolTextBox.Location = New System.Drawing.Point(5, 274)
        Me.bsCurrentVolTextBox.Mandatory = False
        Me.bsCurrentVolTextBox.MaxLength = 20
        Me.bsCurrentVolTextBox.Name = "bsCurrentVolTextBox"
        Me.bsCurrentVolTextBox.ReadOnly = True
        Me.bsCurrentVolTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsCurrentVolTextBox.Size = New System.Drawing.Size(76, 21)
        Me.bsCurrentVolTextBox.TabIndex = 9
        Me.bsCurrentVolTextBox.TabStop = False
        Me.bsCurrentVolTextBox.WordWrap = False
        '
        'bsReagentsCheckVolumePosButton
        '
        Me.bsReagentsCheckVolumePosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsReagentsCheckVolumePosButton.Enabled = False
        Me.bsReagentsCheckVolumePosButton.Location = New System.Drawing.Point(40, 364)
        Me.bsReagentsCheckVolumePosButton.Name = "bsReagentsCheckVolumePosButton"
        Me.bsReagentsCheckVolumePosButton.Size = New System.Drawing.Size(32, 32)
        Me.bsReagentsCheckVolumePosButton.TabIndex = 13
        Me.bsReagentsCheckVolumePosButton.UseVisualStyleBackColor = True
        Me.bsReagentsCheckVolumePosButton.Visible = False
        '
        'bsTestsLeftLabel
        '
        Me.bsTestsLeftLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTestsLeftLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestsLeftLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestsLeftLabel.Location = New System.Drawing.Point(82, 260)
        Me.bsTestsLeftLabel.Name = "bsTestsLeftLabel"
        Me.bsTestsLeftLabel.Size = New System.Drawing.Size(82, 13)
        Me.bsTestsLeftLabel.TabIndex = 30
        Me.bsTestsLeftLabel.Text = "*Tests Left:"
        Me.bsTestsLeftLabel.Title = False
        '
        'bsCurrentVolLabel
        '
        Me.bsCurrentVolLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsCurrentVolLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCurrentVolLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCurrentVolLabel.Location = New System.Drawing.Point(2, 260)
        Me.bsCurrentVolLabel.Name = "bsCurrentVolLabel"
        Me.bsCurrentVolLabel.Size = New System.Drawing.Size(80, 13)
        Me.bsCurrentVolLabel.TabIndex = 29
        Me.bsCurrentVolLabel.Text = "*Current Vol:"
        Me.bsCurrentVolLabel.Title = False
        '
        'bsBottleSizeComboBox
        '
        Me.bsBottleSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsBottleSizeComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsBottleSizeComboBox.FormattingEnabled = True
        Me.bsBottleSizeComboBox.Location = New System.Drawing.Point(85, 236)
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
        Me.bsBottleSizeLabel.Location = New System.Drawing.Point(82, 222)
        Me.bsBottleSizeLabel.Name = "bsBottleSizeLabel"
        Me.bsBottleSizeLabel.Size = New System.Drawing.Size(84, 13)
        Me.bsBottleSizeLabel.TabIndex = 27
        Me.bsBottleSizeLabel.Text = "*Bottle Size:"
        Me.bsBottleSizeLabel.Title = False
        '
        'bsExpirationDateTextBox
        '
        Me.bsExpirationDateTextBox.BackColor = System.Drawing.Color.White
        Me.bsExpirationDateTextBox.DecimalsValues = False
        Me.bsExpirationDateTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsExpirationDateTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsExpirationDateTextBox.IsNumeric = False
        Me.bsExpirationDateTextBox.Location = New System.Drawing.Point(5, 236)
        Me.bsExpirationDateTextBox.Mandatory = False
        Me.bsExpirationDateTextBox.MaxLength = 20
        Me.bsExpirationDateTextBox.Name = "bsExpirationDateTextBox"
        Me.bsExpirationDateTextBox.ReadOnly = True
        Me.bsExpirationDateTextBox.Size = New System.Drawing.Size(75, 21)
        Me.bsExpirationDateTextBox.TabIndex = 7
        Me.bsExpirationDateTextBox.TabStop = False
        Me.bsExpirationDateTextBox.WordWrap = False
        '
        'bsReagentsPositionInfoLabel
        '
        Me.bsReagentsPositionInfoLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsReagentsPositionInfoLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsReagentsPositionInfoLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsPositionInfoLabel.Location = New System.Drawing.Point(4, 12)
        Me.bsReagentsPositionInfoLabel.Name = "bsReagentsPositionInfoLabel"
        Me.bsReagentsPositionInfoLabel.Size = New System.Drawing.Size(167, 19)
        Me.bsReagentsPositionInfoLabel.TabIndex = 25
        Me.bsReagentsPositionInfoLabel.Text = "Position Information"
        Me.bsReagentsPositionInfoLabel.Title = True
        '
        'bsReagentsMoveLastPositionButton
        '
        Me.bsReagentsMoveLastPositionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReagentsMoveLastPositionButton.Enabled = False
        Me.bsReagentsMoveLastPositionButton.Font = New System.Drawing.Font("Verdana", 6.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReagentsMoveLastPositionButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReagentsMoveLastPositionButton.Location = New System.Drawing.Point(81, 337)
        Me.bsReagentsMoveLastPositionButton.Name = "bsReagentsMoveLastPositionButton"
        Me.bsReagentsMoveLastPositionButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReagentsMoveLastPositionButton.TabIndex = 14
        Me.bsReagentsMoveLastPositionButton.UseVisualStyleBackColor = True
        '
        'bsReagentsBarCodeTextBox
        '
        Me.bsReagentsBarCodeTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsBarCodeTextBox.DecimalsValues = False
        Me.bsReagentsBarCodeTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsBarCodeTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsBarCodeTextBox.IsNumeric = False
        Me.bsReagentsBarCodeTextBox.Location = New System.Drawing.Point(5, 198)
        Me.bsReagentsBarCodeTextBox.Mandatory = False
        Me.bsReagentsBarCodeTextBox.Name = "bsReagentsBarCodeTextBox"
        Me.bsReagentsBarCodeTextBox.ReadOnly = True
        Me.bsReagentsBarCodeTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsReagentsBarCodeTextBox.TabIndex = 6
        Me.bsReagentsBarCodeTextBox.TabStop = False
        Me.bsReagentsBarCodeTextBox.WordWrap = False
        '
        'bsTestNameTextBox
        '
        Me.bsTestNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsTestNameTextBox.DecimalsValues = False
        Me.bsTestNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestNameTextBox.IsNumeric = False
        Me.bsTestNameTextBox.Location = New System.Drawing.Point(5, 160)
        Me.bsTestNameTextBox.Mandatory = False
        Me.bsTestNameTextBox.MaxLength = 20
        Me.bsTestNameTextBox.Name = "bsTestNameTextBox"
        Me.bsTestNameTextBox.ReadOnly = True
        Me.bsTestNameTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsTestNameTextBox.TabIndex = 5
        Me.bsTestNameTextBox.TabStop = False
        Me.bsTestNameTextBox.WordWrap = False
        '
        'bsReagentsIncreaseButton
        '
        Me.bsReagentsIncreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReagentsIncreaseButton.Enabled = False
        Me.bsReagentsIncreaseButton.Font = New System.Drawing.Font("Verdana", 6.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReagentsIncreaseButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReagentsIncreaseButton.Location = New System.Drawing.Point(56, 337)
        Me.bsReagentsIncreaseButton.Name = "bsReagentsIncreaseButton"
        Me.bsReagentsIncreaseButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReagentsIncreaseButton.TabIndex = 13
        Me.bsReagentsIncreaseButton.UseVisualStyleBackColor = True
        '
        'bsReagentsNumberTextBox
        '
        Me.bsReagentsNumberTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsNumberTextBox.DecimalsValues = False
        Me.bsReagentsNumberTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsNumberTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsNumberTextBox.IsNumeric = False
        Me.bsReagentsNumberTextBox.Location = New System.Drawing.Point(123, 84)
        Me.bsReagentsNumberTextBox.Mandatory = False
        Me.bsReagentsNumberTextBox.Name = "bsReagentsNumberTextBox"
        Me.bsReagentsNumberTextBox.ReadOnly = True
        Me.bsReagentsNumberTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsReagentsNumberTextBox.Size = New System.Drawing.Size(46, 21)
        Me.bsReagentsNumberTextBox.TabIndex = 3
        Me.bsReagentsNumberTextBox.TabStop = False
        Me.bsReagentsNumberTextBox.WordWrap = False
        '
        'bsReagentsDecreaseButton
        '
        Me.bsReagentsDecreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReagentsDecreaseButton.Enabled = False
        Me.bsReagentsDecreaseButton.Font = New System.Drawing.Font("Verdana", 6.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReagentsDecreaseButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReagentsDecreaseButton.Location = New System.Drawing.Point(31, 337)
        Me.bsReagentsDecreaseButton.Name = "bsReagentsDecreaseButton"
        Me.bsReagentsDecreaseButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReagentsDecreaseButton.TabIndex = 12
        Me.bsReagentsDecreaseButton.UseVisualStyleBackColor = True
        '
        'bsReagentNameTextBox
        '
        Me.bsReagentNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentNameTextBox.DecimalsValues = False
        Me.bsReagentNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentNameTextBox.IsNumeric = False
        Me.bsReagentNameTextBox.Location = New System.Drawing.Point(5, 122)
        Me.bsReagentNameTextBox.Mandatory = False
        Me.bsReagentNameTextBox.Name = "bsReagentNameTextBox"
        Me.bsReagentNameTextBox.ReadOnly = True
        Me.bsReagentNameTextBox.Size = New System.Drawing.Size(164, 21)
        Me.bsReagentNameTextBox.TabIndex = 4
        Me.bsReagentNameTextBox.TabStop = False
        Me.bsReagentNameTextBox.WordWrap = False
        '
        'bsReagentsMoveFirstPositionButton
        '
        Me.bsReagentsMoveFirstPositionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsReagentsMoveFirstPositionButton.Enabled = False
        Me.bsReagentsMoveFirstPositionButton.Font = New System.Drawing.Font("Verdana", 6.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReagentsMoveFirstPositionButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsReagentsMoveFirstPositionButton.Location = New System.Drawing.Point(6, 337)
        Me.bsReagentsMoveFirstPositionButton.Name = "bsReagentsMoveFirstPositionButton"
        Me.bsReagentsMoveFirstPositionButton.Size = New System.Drawing.Size(26, 26)
        Me.bsReagentsMoveFirstPositionButton.TabIndex = 11
        Me.bsReagentsMoveFirstPositionButton.UseVisualStyleBackColor = True
        '
        'bsReagentsContentTextBox
        '
        Me.bsReagentsContentTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsContentTextBox.DecimalsValues = False
        Me.bsReagentsContentTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsContentTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsContentTextBox.IsNumeric = False
        Me.bsReagentsContentTextBox.Location = New System.Drawing.Point(5, 84)
        Me.bsReagentsContentTextBox.Mandatory = False
        Me.bsReagentsContentTextBox.MaxLength = 20
        Me.bsReagentsContentTextBox.Name = "bsReagentsContentTextBox"
        Me.bsReagentsContentTextBox.ReadOnly = True
        Me.bsReagentsContentTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsReagentsContentTextBox.TabIndex = 2
        Me.bsReagentsContentTextBox.TabStop = False
        Me.bsReagentsContentTextBox.WordWrap = False
        '
        'bsReagentsRingNumTextBox
        '
        Me.bsReagentsRingNumTextBox.BackColor = System.Drawing.Color.White
        Me.bsReagentsRingNumTextBox.DecimalsValues = False
        Me.bsReagentsRingNumTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsRingNumTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsRingNumTextBox.IsNumeric = False
        Me.bsReagentsRingNumTextBox.Location = New System.Drawing.Point(5, 47)
        Me.bsReagentsRingNumTextBox.Mandatory = False
        Me.bsReagentsRingNumTextBox.Name = "bsReagentsRingNumTextBox"
        Me.bsReagentsRingNumTextBox.ReadOnly = True
        Me.bsReagentsRingNumTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.bsReagentsRingNumTextBox.Size = New System.Drawing.Size(115, 21)
        Me.bsReagentsRingNumTextBox.TabIndex = 1
        Me.bsReagentsRingNumTextBox.TabStop = False
        Me.bsReagentsRingNumTextBox.WordWrap = False
        '
        'bsExpirationDateLabel
        '
        Me.bsExpirationDateLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsExpirationDateLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsExpirationDateLabel.ForeColor = System.Drawing.Color.Black
        Me.bsExpirationDateLabel.Location = New System.Drawing.Point(2, 222)
        Me.bsExpirationDateLabel.Name = "bsExpirationDateLabel"
        Me.bsExpirationDateLabel.Size = New System.Drawing.Size(74, 13)
        Me.bsExpirationDateLabel.TabIndex = 8
        Me.bsExpirationDateLabel.Text = "*Exp Date:"
        Me.bsExpirationDateLabel.Title = False
        '
        'bsReagentsBarCodeLabel
        '
        Me.bsReagentsBarCodeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsBarCodeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsBarCodeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsBarCodeLabel.Location = New System.Drawing.Point(2, 184)
        Me.bsReagentsBarCodeLabel.Name = "bsReagentsBarCodeLabel"
        Me.bsReagentsBarCodeLabel.Size = New System.Drawing.Size(163, 13)
        Me.bsReagentsBarCodeLabel.TabIndex = 7
        Me.bsReagentsBarCodeLabel.Text = "*Reagent Barcode:"
        Me.bsReagentsBarCodeLabel.Title = False
        '
        'bsTestNameLabel
        '
        Me.bsTestNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTestNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestNameLabel.Location = New System.Drawing.Point(2, 146)
        Me.bsTestNameLabel.Name = "bsTestNameLabel"
        Me.bsTestNameLabel.Size = New System.Drawing.Size(162, 13)
        Me.bsTestNameLabel.TabIndex = 5
        Me.bsTestNameLabel.Text = "*Test Name:"
        Me.bsTestNameLabel.Title = False
        '
        'bsReagentsNumberLabel
        '
        Me.bsReagentsNumberLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsNumberLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsNumberLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsNumberLabel.Location = New System.Drawing.Point(120, 70)
        Me.bsReagentsNumberLabel.Name = "bsReagentsNumberLabel"
        Me.bsReagentsNumberLabel.Size = New System.Drawing.Size(49, 13)
        Me.bsReagentsNumberLabel.TabIndex = 4
        Me.bsReagentsNumberLabel.Text = "*Num:"
        Me.bsReagentsNumberLabel.Title = False
        '
        'bsReagentNameLabel
        '
        Me.bsReagentNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentNameLabel.Location = New System.Drawing.Point(2, 108)
        Me.bsReagentNameLabel.Name = "bsReagentNameLabel"
        Me.bsReagentNameLabel.Size = New System.Drawing.Size(162, 13)
        Me.bsReagentNameLabel.TabIndex = 3
        Me.bsReagentNameLabel.Text = "*Reagent Name:"
        Me.bsReagentNameLabel.Title = False
        '
        'bsReagentsContentLabel
        '
        Me.bsReagentsContentLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsContentLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsContentLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsContentLabel.Location = New System.Drawing.Point(2, 70)
        Me.bsReagentsContentLabel.Name = "bsReagentsContentLabel"
        Me.bsReagentsContentLabel.Size = New System.Drawing.Size(113, 13)
        Me.bsReagentsContentLabel.TabIndex = 2
        Me.bsReagentsContentLabel.Text = "*Content:"
        Me.bsReagentsContentLabel.Title = False
        '
        'bsReagentsRingNumLabel
        '
        Me.bsReagentsRingNumLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsReagentsRingNumLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReagentsRingNumLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsRingNumLabel.Location = New System.Drawing.Point(2, 33)
        Me.bsReagentsRingNumLabel.Name = "bsReagentsRingNumLabel"
        Me.bsReagentsRingNumLabel.Size = New System.Drawing.Size(115, 13)
        Me.bsReagentsRingNumLabel.TabIndex = 1
        Me.bsReagentsRingNumLabel.Text = "*Ring Num:"
        Me.bsReagentsRingNumLabel.Title = False
        '
        'Reag11
        '
        Me.Reag11.BackColor = System.Drawing.Color.Transparent
        Me.Reag11.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag11.Image = Nothing
        Me.Reag11.ImagePath = ""
        Me.Reag11.IsTransparentImage = False
        Me.Reag11.Location = New System.Drawing.Point(287, 547)
        Me.Reag11.Name = "Reag11"
        Me.Reag11.Rotation = 355
        Me.Reag11.ShowThrough = True
        Me.Reag11.Size = New System.Drawing.Size(37, 37)
        Me.Reag11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.Reag11.TabIndex = 174
        Me.Reag11.TabStop = False
        Me.Reag11.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag11.WaitOnLoad = True
        '
        'Reag12
        '
        Me.Reag12.BackColor = System.Drawing.Color.Transparent
        Me.Reag12.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag12.Image = Nothing
        Me.Reag12.ImagePath = ""
        Me.Reag12.IsTransparentImage = False
        Me.Reag12.Location = New System.Drawing.Point(324, 541)
        Me.Reag12.Name = "Reag12"
        Me.Reag12.Rotation = 349
        Me.Reag12.ShowThrough = True
        Me.Reag12.Size = New System.Drawing.Size(37, 37)
        Me.Reag12.TabIndex = 175
        Me.Reag12.TabStop = False
        Me.Reag12.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag12.WaitOnLoad = True
        '
        'Reag13
        '
        Me.Reag13.BackColor = System.Drawing.Color.Transparent
        Me.Reag13.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag13.Image = Nothing
        Me.Reag13.ImagePath = ""
        Me.Reag13.IsTransparentImage = False
        Me.Reag13.Location = New System.Drawing.Point(360, 531)
        Me.Reag13.Name = "Reag13"
        Me.Reag13.Rotation = 341
        Me.Reag13.ShowThrough = True
        Me.Reag13.Size = New System.Drawing.Size(37, 37)
        Me.Reag13.TabIndex = 176
        Me.Reag13.TabStop = False
        Me.Reag13.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag13.WaitOnLoad = True
        '
        'Reag14
        '
        Me.Reag14.BackColor = System.Drawing.Color.Transparent
        Me.Reag14.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag14.Image = Nothing
        Me.Reag14.ImagePath = ""
        Me.Reag14.IsTransparentImage = False
        Me.Reag14.Location = New System.Drawing.Point(394, 514)
        Me.Reag14.Name = "Reag14"
        Me.Reag14.Rotation = 332
        Me.Reag14.ShowThrough = True
        Me.Reag14.Size = New System.Drawing.Size(37, 37)
        Me.Reag14.TabIndex = 177
        Me.Reag14.TabStop = False
        Me.Reag14.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag14.WaitOnLoad = True
        '
        'Reag15
        '
        Me.Reag15.BackColor = System.Drawing.Color.Transparent
        Me.Reag15.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag15.Image = Nothing
        Me.Reag15.ImagePath = ""
        Me.Reag15.IsTransparentImage = False
        Me.Reag15.Location = New System.Drawing.Point(426, 495)
        Me.Reag15.Name = "Reag15"
        Me.Reag15.Rotation = 325
        Me.Reag15.ShowThrough = True
        Me.Reag15.Size = New System.Drawing.Size(37, 37)
        Me.Reag15.TabIndex = 183
        Me.Reag15.TabStop = False
        Me.Reag15.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag15.WaitOnLoad = True
        '
        'Reag16
        '
        Me.Reag16.BackColor = System.Drawing.Color.Transparent
        Me.Reag16.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag16.Image = Nothing
        Me.Reag16.ImagePath = ""
        Me.Reag16.IsTransparentImage = False
        Me.Reag16.Location = New System.Drawing.Point(455, 470)
        Me.Reag16.Name = "Reag16"
        Me.Reag16.Rotation = 316
        Me.Reag16.ShowThrough = True
        Me.Reag16.Size = New System.Drawing.Size(37, 37)
        Me.Reag16.TabIndex = 184
        Me.Reag16.TabStop = False
        Me.Reag16.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag16.WaitOnLoad = True
        '
        'Reag17
        '
        Me.Reag17.BackColor = System.Drawing.Color.Transparent
        Me.Reag17.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag17.Image = Nothing
        Me.Reag17.ImagePath = ""
        Me.Reag17.IsTransparentImage = False
        Me.Reag17.Location = New System.Drawing.Point(481, 440)
        Me.Reag17.Name = "Reag17"
        Me.Reag17.Rotation = 306
        Me.Reag17.ShowThrough = True
        Me.Reag17.Size = New System.Drawing.Size(37, 37)
        Me.Reag17.TabIndex = 185
        Me.Reag17.TabStop = False
        Me.Reag17.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag17.WaitOnLoad = True
        '
        'Reag18
        '
        Me.Reag18.BackColor = System.Drawing.Color.Transparent
        Me.Reag18.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag18.Image = Nothing
        Me.Reag18.ImagePath = ""
        Me.Reag18.IsTransparentImage = False
        Me.Reag18.Location = New System.Drawing.Point(501, 408)
        Me.Reag18.Name = "Reag18"
        Me.Reag18.Rotation = 302
        Me.Reag18.ShowThrough = True
        Me.Reag18.Size = New System.Drawing.Size(37, 37)
        Me.Reag18.TabIndex = 186
        Me.Reag18.TabStop = False
        Me.Reag18.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag18.WaitOnLoad = True
        '
        'Reag19
        '
        Me.Reag19.BackColor = System.Drawing.Color.Transparent
        Me.Reag19.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag19.Image = Nothing
        Me.Reag19.ImagePath = ""
        Me.Reag19.IsTransparentImage = False
        Me.Reag19.Location = New System.Drawing.Point(517, 374)
        Me.Reag19.Name = "Reag19"
        Me.Reag19.Rotation = 292
        Me.Reag19.ShowThrough = True
        Me.Reag19.Size = New System.Drawing.Size(37, 37)
        Me.Reag19.TabIndex = 187
        Me.Reag19.TabStop = False
        Me.Reag19.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag19.WaitOnLoad = True
        '
        'Reag110
        '
        Me.Reag110.BackColor = System.Drawing.Color.Transparent
        Me.Reag110.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag110.Image = Nothing
        Me.Reag110.ImagePath = ""
        Me.Reag110.IsTransparentImage = False
        Me.Reag110.Location = New System.Drawing.Point(529, 337)
        Me.Reag110.Name = "Reag110"
        Me.Reag110.Rotation = 285
        Me.Reag110.ShowThrough = True
        Me.Reag110.Size = New System.Drawing.Size(37, 37)
        Me.Reag110.TabIndex = 188
        Me.Reag110.TabStop = False
        Me.Reag110.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag110.WaitOnLoad = True
        '
        'Reag111
        '
        Me.Reag111.BackColor = System.Drawing.Color.Transparent
        Me.Reag111.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag111.Image = Nothing
        Me.Reag111.ImagePath = ""
        Me.Reag111.IsTransparentImage = False
        Me.Reag111.Location = New System.Drawing.Point(534, 298)
        Me.Reag111.Name = "Reag111"
        Me.Reag111.Rotation = 277
        Me.Reag111.ShowThrough = True
        Me.Reag111.Size = New System.Drawing.Size(37, 37)
        Me.Reag111.TabIndex = 189
        Me.Reag111.TabStop = False
        Me.Reag111.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag111.WaitOnLoad = True
        '
        'Reag112
        '
        Me.Reag112.BackColor = System.Drawing.Color.Transparent
        Me.Reag112.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag112.Image = Nothing
        Me.Reag112.ImagePath = ""
        Me.Reag112.IsTransparentImage = False
        Me.Reag112.Location = New System.Drawing.Point(535, 261)
        Me.Reag112.Name = "Reag112"
        Me.Reag112.Rotation = 266
        Me.Reag112.ShowThrough = True
        Me.Reag112.Size = New System.Drawing.Size(37, 37)
        Me.Reag112.TabIndex = 190
        Me.Reag112.TabStop = False
        Me.Reag112.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag112.WaitOnLoad = True
        '
        'Reag113
        '
        Me.Reag113.BackColor = System.Drawing.Color.Transparent
        Me.Reag113.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag113.Image = Nothing
        Me.Reag113.ImagePath = ""
        Me.Reag113.IsTransparentImage = False
        Me.Reag113.Location = New System.Drawing.Point(530, 222)
        Me.Reag113.Name = "Reag113"
        Me.Reag113.Rotation = 257
        Me.Reag113.ShowThrough = True
        Me.Reag113.Size = New System.Drawing.Size(37, 37)
        Me.Reag113.TabIndex = 191
        Me.Reag113.TabStop = False
        Me.Reag113.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag113.WaitOnLoad = True
        '
        'Reag114
        '
        Me.Reag114.BackColor = System.Drawing.Color.Transparent
        Me.Reag114.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag114.Image = Nothing
        Me.Reag114.ImagePath = ""
        Me.Reag114.IsTransparentImage = False
        Me.Reag114.Location = New System.Drawing.Point(520, 184)
        Me.Reag114.Name = "Reag114"
        Me.Reag114.Rotation = 252
        Me.Reag114.ShowThrough = True
        Me.Reag114.Size = New System.Drawing.Size(37, 37)
        Me.Reag114.TabIndex = 192
        Me.Reag114.TabStop = False
        Me.Reag114.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag114.WaitOnLoad = True
        '
        'Reag115
        '
        Me.Reag115.BackColor = System.Drawing.Color.Transparent
        Me.Reag115.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag115.Image = Nothing
        Me.Reag115.ImagePath = ""
        Me.Reag115.IsTransparentImage = False
        Me.Reag115.Location = New System.Drawing.Point(502, 149)
        Me.Reag115.Name = "Reag115"
        Me.Reag115.Rotation = 241
        Me.Reag115.ShowThrough = True
        Me.Reag115.Size = New System.Drawing.Size(37, 37)
        Me.Reag115.TabIndex = 193
        Me.Reag115.TabStop = False
        Me.Reag115.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag115.WaitOnLoad = True
        '
        'Reag116
        '
        Me.Reag116.BackColor = System.Drawing.Color.Transparent
        Me.Reag116.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag116.Image = Nothing
        Me.Reag116.ImagePath = ""
        Me.Reag116.IsTransparentImage = False
        Me.Reag116.Location = New System.Drawing.Point(484, 116)
        Me.Reag116.Name = "Reag116"
        Me.Reag116.Rotation = 234
        Me.Reag116.ShowThrough = True
        Me.Reag116.Size = New System.Drawing.Size(37, 37)
        Me.Reag116.TabIndex = 194
        Me.Reag116.TabStop = False
        Me.Reag116.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag116.WaitOnLoad = True
        '
        'Reag117
        '
        Me.Reag117.BackColor = System.Drawing.Color.Transparent
        Me.Reag117.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag117.Image = Nothing
        Me.Reag117.ImagePath = ""
        Me.Reag117.IsTransparentImage = False
        Me.Reag117.Location = New System.Drawing.Point(460, 88)
        Me.Reag117.Name = "Reag117"
        Me.Reag117.Rotation = 228
        Me.Reag117.ShowThrough = True
        Me.Reag117.Size = New System.Drawing.Size(37, 37)
        Me.Reag117.TabIndex = 195
        Me.Reag117.TabStop = False
        Me.Reag117.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag117.WaitOnLoad = True
        '
        'Reag118
        '
        Me.Reag118.BackColor = System.Drawing.Color.Transparent
        Me.Reag118.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag118.Image = Nothing
        Me.Reag118.ImagePath = ""
        Me.Reag118.IsTransparentImage = False
        Me.Reag118.Location = New System.Drawing.Point(430, 62)
        Me.Reag118.Name = "Reag118"
        Me.Reag118.Rotation = 218
        Me.Reag118.ShowThrough = True
        Me.Reag118.Size = New System.Drawing.Size(37, 37)
        Me.Reag118.TabIndex = 196
        Me.Reag118.TabStop = False
        Me.Reag118.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag118.WaitOnLoad = True
        '
        'Reag119
        '
        Me.Reag119.BackColor = System.Drawing.Color.Transparent
        Me.Reag119.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag119.Image = Nothing
        Me.Reag119.ImagePath = ""
        Me.Reag119.IsTransparentImage = False
        Me.Reag119.Location = New System.Drawing.Point(399, 42)
        Me.Reag119.Name = "Reag119"
        Me.Reag119.Rotation = 212
        Me.Reag119.ShowThrough = True
        Me.Reag119.Size = New System.Drawing.Size(37, 37)
        Me.Reag119.TabIndex = 197
        Me.Reag119.TabStop = False
        Me.Reag119.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag119.WaitOnLoad = True
        '
        'Reag120
        '
        Me.Reag120.BackColor = System.Drawing.Color.Transparent
        Me.Reag120.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag120.Image = Nothing
        Me.Reag120.ImagePath = ""
        Me.Reag120.IsTransparentImage = False
        Me.Reag120.Location = New System.Drawing.Point(365, 24)
        Me.Reag120.Name = "Reag120"
        Me.Reag120.Rotation = 204
        Me.Reag120.ShowThrough = True
        Me.Reag120.Size = New System.Drawing.Size(37, 37)
        Me.Reag120.TabIndex = 198
        Me.Reag120.TabStop = False
        Me.Reag120.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag120.WaitOnLoad = True
        '
        'Reag121
        '
        Me.Reag121.BackColor = System.Drawing.Color.Transparent
        Me.Reag121.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag121.Image = Nothing
        Me.Reag121.ImagePath = ""
        Me.Reag121.IsTransparentImage = False
        Me.Reag121.Location = New System.Drawing.Point(329, 14)
        Me.Reag121.Name = "Reag121"
        Me.Reag121.Rotation = 195
        Me.Reag121.ShowThrough = True
        Me.Reag121.Size = New System.Drawing.Size(37, 37)
        Me.Reag121.TabIndex = 199
        Me.Reag121.TabStop = False
        Me.Reag121.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag121.WaitOnLoad = True
        '
        'Reag122
        '
        Me.Reag122.BackColor = System.Drawing.Color.Transparent
        Me.Reag122.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag122.Image = Nothing
        Me.Reag122.ImagePath = ""
        Me.Reag122.IsTransparentImage = False
        Me.Reag122.Location = New System.Drawing.Point(291, 8)
        Me.Reag122.Name = "Reag122"
        Me.Reag122.Rotation = 187
        Me.Reag122.ShowThrough = True
        Me.Reag122.Size = New System.Drawing.Size(37, 37)
        Me.Reag122.TabIndex = 200
        Me.Reag122.TabStop = False
        Me.Reag122.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag122.WaitOnLoad = True
        '
        'Reag123
        '
        Me.Reag123.BackColor = System.Drawing.Color.Transparent
        Me.Reag123.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag123.Image = Nothing
        Me.Reag123.ImagePath = ""
        Me.Reag123.IsTransparentImage = False
        Me.Reag123.Location = New System.Drawing.Point(254, 7)
        Me.Reag123.Name = "Reag123"
        Me.Reag123.Rotation = 179
        Me.Reag123.ShowThrough = True
        Me.Reag123.Size = New System.Drawing.Size(37, 37)
        Me.Reag123.TabIndex = 201
        Me.Reag123.TabStop = False
        Me.Reag123.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag123.WaitOnLoad = True
        '
        'Reag124
        '
        Me.Reag124.BackColor = System.Drawing.Color.Transparent
        Me.Reag124.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag124.Image = Nothing
        Me.Reag124.ImagePath = ""
        Me.Reag124.IsTransparentImage = False
        Me.Reag124.Location = New System.Drawing.Point(217, 12)
        Me.Reag124.Name = "Reag124"
        Me.Reag124.Rotation = 169
        Me.Reag124.ShowThrough = True
        Me.Reag124.Size = New System.Drawing.Size(37, 37)
        Me.Reag124.TabIndex = 202
        Me.Reag124.TabStop = False
        Me.Reag124.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag124.WaitOnLoad = True
        '
        'Reag125
        '
        Me.Reag125.BackColor = System.Drawing.Color.Transparent
        Me.Reag125.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag125.Image = Nothing
        Me.Reag125.ImagePath = ""
        Me.Reag125.IsTransparentImage = False
        Me.Reag125.Location = New System.Drawing.Point(179, 23)
        Me.Reag125.Name = "Reag125"
        Me.Reag125.Rotation = 162
        Me.Reag125.ShowThrough = True
        Me.Reag125.Size = New System.Drawing.Size(37, 37)
        Me.Reag125.TabIndex = 203
        Me.Reag125.TabStop = False
        Me.Reag125.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag125.WaitOnLoad = True
        '
        'Reag126
        '
        Me.Reag126.BackColor = System.Drawing.Color.Transparent
        Me.Reag126.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag126.Image = Nothing
        Me.Reag126.ImagePath = ""
        Me.Reag126.IsTransparentImage = False
        Me.Reag126.Location = New System.Drawing.Point(146, 38)
        Me.Reag126.Name = "Reag126"
        Me.Reag126.Rotation = 154
        Me.Reag126.ShowThrough = True
        Me.Reag126.Size = New System.Drawing.Size(37, 37)
        Me.Reag126.TabIndex = 204
        Me.Reag126.TabStop = False
        Me.Reag126.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag126.WaitOnLoad = True
        '
        'Reag127
        '
        Me.Reag127.BackColor = System.Drawing.Color.Transparent
        Me.Reag127.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag127.Image = Nothing
        Me.Reag127.ImagePath = ""
        Me.Reag127.IsTransparentImage = False
        Me.Reag127.Location = New System.Drawing.Point(113, 59)
        Me.Reag127.Name = "Reag127"
        Me.Reag127.Rotation = 145
        Me.Reag127.ShowThrough = True
        Me.Reag127.Size = New System.Drawing.Size(37, 37)
        Me.Reag127.TabIndex = 205
        Me.Reag127.TabStop = False
        Me.Reag127.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag127.WaitOnLoad = True
        '
        'Reag128
        '
        Me.Reag128.BackColor = System.Drawing.Color.Transparent
        Me.Reag128.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag128.Image = Nothing
        Me.Reag128.ImagePath = ""
        Me.Reag128.IsTransparentImage = False
        Me.Reag128.Location = New System.Drawing.Point(85, 84)
        Me.Reag128.Name = "Reag128"
        Me.Reag128.Rotation = 138
        Me.Reag128.ShowThrough = True
        Me.Reag128.Size = New System.Drawing.Size(37, 37)
        Me.Reag128.TabIndex = 206
        Me.Reag128.TabStop = False
        Me.Reag128.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag128.WaitOnLoad = True
        '
        'Reag129
        '
        Me.Reag129.BackColor = System.Drawing.Color.Transparent
        Me.Reag129.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag129.Image = Nothing
        Me.Reag129.ImagePath = ""
        Me.Reag129.IsTransparentImage = False
        Me.Reag129.Location = New System.Drawing.Point(60, 112)
        Me.Reag129.Name = "Reag129"
        Me.Reag129.Rotation = 126
        Me.Reag129.ShowThrough = True
        Me.Reag129.Size = New System.Drawing.Size(37, 37)
        Me.Reag129.TabIndex = 207
        Me.Reag129.TabStop = False
        Me.Reag129.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag129.WaitOnLoad = True
        '
        'Reag130
        '
        Me.Reag130.BackColor = System.Drawing.Color.Transparent
        Me.Reag130.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag130.Image = Nothing
        Me.Reag130.ImagePath = ""
        Me.Reag130.IsTransparentImage = False
        Me.Reag130.Location = New System.Drawing.Point(38, 145)
        Me.Reag130.Name = "Reag130"
        Me.Reag130.Rotation = 120
        Me.Reag130.ShowThrough = True
        Me.Reag130.Size = New System.Drawing.Size(37, 37)
        Me.Reag130.TabIndex = 208
        Me.Reag130.TabStop = False
        Me.Reag130.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag130.WaitOnLoad = True
        '
        'Reag131
        '
        Me.Reag131.BackColor = System.Drawing.Color.Transparent
        Me.Reag131.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag131.Image = Nothing
        Me.Reag131.ImagePath = ""
        Me.Reag131.IsTransparentImage = False
        Me.Reag131.Location = New System.Drawing.Point(23, 180)
        Me.Reag131.Name = "Reag131"
        Me.Reag131.Rotation = 111
        Me.Reag131.ShowThrough = True
        Me.Reag131.Size = New System.Drawing.Size(37, 37)
        Me.Reag131.TabIndex = 209
        Me.Reag131.TabStop = False
        Me.Reag131.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag131.WaitOnLoad = True
        '
        'Reag132
        '
        Me.Reag132.BackColor = System.Drawing.Color.Transparent
        Me.Reag132.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag132.Image = Nothing
        Me.Reag132.ImagePath = ""
        Me.Reag132.IsTransparentImage = False
        Me.Reag132.Location = New System.Drawing.Point(11, 217)
        Me.Reag132.Name = "Reag132"
        Me.Reag132.Rotation = 101
        Me.Reag132.ShowThrough = True
        Me.Reag132.Size = New System.Drawing.Size(37, 37)
        Me.Reag132.TabIndex = 210
        Me.Reag132.TabStop = False
        Me.Reag132.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag132.WaitOnLoad = True
        '
        'Reag133
        '
        Me.Reag133.BackColor = System.Drawing.Color.Transparent
        Me.Reag133.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag133.Image = Nothing
        Me.Reag133.ImagePath = ""
        Me.Reag133.IsTransparentImage = False
        Me.Reag133.Location = New System.Drawing.Point(6, 255)
        Me.Reag133.Name = "Reag133"
        Me.Reag133.Rotation = 95
        Me.Reag133.ShowThrough = True
        Me.Reag133.Size = New System.Drawing.Size(37, 37)
        Me.Reag133.TabIndex = 211
        Me.Reag133.TabStop = False
        Me.Reag133.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag133.WaitOnLoad = True
        '
        'Reag134
        '
        Me.Reag134.BackColor = System.Drawing.Color.Transparent
        Me.Reag134.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag134.Image = Nothing
        Me.Reag134.ImagePath = ""
        Me.Reag134.IsTransparentImage = False
        Me.Reag134.Location = New System.Drawing.Point(7, 294)
        Me.Reag134.Name = "Reag134"
        Me.Reag134.Rotation = 86
        Me.Reag134.ShowThrough = True
        Me.Reag134.Size = New System.Drawing.Size(37, 37)
        Me.Reag134.TabIndex = 212
        Me.Reag134.TabStop = False
        Me.Reag134.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag134.WaitOnLoad = True
        '
        'Reag135
        '
        Me.Reag135.BackColor = System.Drawing.Color.Transparent
        Me.Reag135.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag135.Image = Nothing
        Me.Reag135.ImagePath = ""
        Me.Reag135.IsTransparentImage = False
        Me.Reag135.Location = New System.Drawing.Point(10, 332)
        Me.Reag135.Name = "Reag135"
        Me.Reag135.Rotation = 78
        Me.Reag135.ShowThrough = True
        Me.Reag135.Size = New System.Drawing.Size(37, 37)
        Me.Reag135.TabIndex = 213
        Me.Reag135.TabStop = False
        Me.Reag135.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag135.WaitOnLoad = True
        '
        'Reag136
        '
        Me.Reag136.BackColor = System.Drawing.Color.Transparent
        Me.Reag136.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag136.Image = Nothing
        Me.Reag136.ImagePath = ""
        Me.Reag136.IsTransparentImage = False
        Me.Reag136.Location = New System.Drawing.Point(21, 368)
        Me.Reag136.Name = "Reag136"
        Me.Reag136.Rotation = 70
        Me.Reag136.ShowThrough = True
        Me.Reag136.Size = New System.Drawing.Size(37, 37)
        Me.Reag136.TabIndex = 214
        Me.Reag136.TabStop = False
        Me.Reag136.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag136.WaitOnLoad = True
        '
        'Reag137
        '
        Me.Reag137.BackColor = System.Drawing.Color.Transparent
        Me.Reag137.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag137.Image = Nothing
        Me.Reag137.ImagePath = ""
        Me.Reag137.IsTransparentImage = False
        Me.Reag137.Location = New System.Drawing.Point(36, 403)
        Me.Reag137.Name = "Reag137"
        Me.Reag137.Rotation = 62
        Me.Reag137.ShowThrough = True
        Me.Reag137.Size = New System.Drawing.Size(37, 37)
        Me.Reag137.TabIndex = 215
        Me.Reag137.TabStop = False
        Me.Reag137.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag137.WaitOnLoad = True
        '
        'Reag138
        '
        Me.Reag138.BackColor = System.Drawing.Color.Transparent
        Me.Reag138.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag138.Image = Nothing
        Me.Reag138.ImagePath = ""
        Me.Reag138.IsTransparentImage = False
        Me.Reag138.Location = New System.Drawing.Point(57, 436)
        Me.Reag138.Name = "Reag138"
        Me.Reag138.Rotation = 55
        Me.Reag138.ShowThrough = True
        Me.Reag138.Size = New System.Drawing.Size(37, 37)
        Me.Reag138.TabIndex = 216
        Me.Reag138.TabStop = False
        Me.Reag138.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag138.WaitOnLoad = True
        '
        'Reag139
        '
        Me.Reag139.BackColor = System.Drawing.Color.Transparent
        Me.Reag139.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag139.Image = Nothing
        Me.Reag139.ImagePath = ""
        Me.Reag139.IsTransparentImage = False
        Me.Reag139.Location = New System.Drawing.Point(81, 466)
        Me.Reag139.Name = "Reag139"
        Me.Reag139.Rotation = 46
        Me.Reag139.ShowThrough = True
        Me.Reag139.Size = New System.Drawing.Size(37, 37)
        Me.Reag139.TabIndex = 217
        Me.Reag139.TabStop = False
        Me.Reag139.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag139.WaitOnLoad = True
        '
        'Reag140
        '
        Me.Reag140.BackColor = System.Drawing.Color.Transparent
        Me.Reag140.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag140.Image = Nothing
        Me.Reag140.ImagePath = ""
        Me.Reag140.IsTransparentImage = False
        Me.Reag140.Location = New System.Drawing.Point(109, 492)
        Me.Reag140.Name = "Reag140"
        Me.Reag140.Rotation = 36
        Me.Reag140.ShowThrough = True
        Me.Reag140.Size = New System.Drawing.Size(37, 37)
        Me.Reag140.TabIndex = 218
        Me.Reag140.TabStop = False
        Me.Reag140.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag140.WaitOnLoad = True
        '
        'Reag141
        '
        Me.Reag141.BackColor = System.Drawing.Color.Transparent
        Me.Reag141.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag141.Image = Nothing
        Me.Reag141.ImagePath = ""
        Me.Reag141.IsTransparentImage = False
        Me.Reag141.Location = New System.Drawing.Point(141, 513)
        Me.Reag141.Name = "Reag141"
        Me.Reag141.Rotation = 32
        Me.Reag141.ShowThrough = True
        Me.Reag141.Size = New System.Drawing.Size(37, 37)
        Me.Reag141.TabIndex = 219
        Me.Reag141.TabStop = False
        Me.Reag141.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag141.WaitOnLoad = True
        '
        'Reag142
        '
        Me.Reag142.BackColor = System.Drawing.Color.Transparent
        Me.Reag142.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag142.Image = Nothing
        Me.Reag142.ImagePath = ""
        Me.Reag142.IsTransparentImage = False
        Me.Reag142.Location = New System.Drawing.Point(175, 529)
        Me.Reag142.Name = "Reag142"
        Me.Reag142.Rotation = 23
        Me.Reag142.ShowThrough = True
        Me.Reag142.Size = New System.Drawing.Size(37, 37)
        Me.Reag142.TabIndex = 220
        Me.Reag142.TabStop = False
        Me.Reag142.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag142.WaitOnLoad = True
        '
        'Reag143
        '
        Me.Reag143.BackColor = System.Drawing.Color.Transparent
        Me.Reag143.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag143.Image = Nothing
        Me.Reag143.ImagePath = ""
        Me.Reag143.IsTransparentImage = False
        Me.Reag143.Location = New System.Drawing.Point(212, 541)
        Me.Reag143.Name = "Reag143"
        Me.Reag143.Rotation = 14
        Me.Reag143.ShowThrough = True
        Me.Reag143.Size = New System.Drawing.Size(37, 37)
        Me.Reag143.TabIndex = 221
        Me.Reag143.TabStop = False
        Me.Reag143.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag143.WaitOnLoad = True
        '
        'Reag144
        '
        Me.Reag144.BackColor = System.Drawing.Color.Transparent
        Me.Reag144.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag144.Image = Nothing
        Me.Reag144.ImagePath = ""
        Me.Reag144.IsTransparentImage = False
        Me.Reag144.Location = New System.Drawing.Point(249, 546)
        Me.Reag144.Name = "Reag144"
        Me.Reag144.Rotation = 6
        Me.Reag144.ShowThrough = True
        Me.Reag144.Size = New System.Drawing.Size(37, 37)
        Me.Reag144.TabIndex = 222
        Me.Reag144.TabStop = False
        Me.Reag144.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag144.WaitOnLoad = True
        '
        'Reag245
        '
        Me.Reag245.BackColor = System.Drawing.Color.Transparent
        Me.Reag245.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag245.Image = Nothing
        Me.Reag245.ImagePath = ""
        Me.Reag245.IsTransparentImage = False
        Me.Reag245.Location = New System.Drawing.Point(227, 428)
        Me.Reag245.Name = "Reag245"
        Me.Reag245.Rotation = 1
        Me.Reag245.ShowThrough = True
        Me.Reag245.Size = New System.Drawing.Size(122, 122)
        Me.Reag245.TabIndex = 223
        Me.Reag245.TabStop = False
        Me.Reag245.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag245.WaitOnLoad = True
        '
        'Reag246
        '
        Me.Reag246.BackColor = System.Drawing.Color.Transparent
        Me.Reag246.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag246.Image = Nothing
        Me.Reag246.ImagePath = ""
        Me.Reag246.IsTransparentImage = False
        Me.Reag246.Location = New System.Drawing.Point(254, 426)
        Me.Reag246.Name = "Reag246"
        Me.Reag246.Rotation = 352
        Me.Reag246.ShowThrough = True
        Me.Reag246.Size = New System.Drawing.Size(122, 122)
        Me.Reag246.TabIndex = 224
        Me.Reag246.TabStop = False
        Me.Reag246.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag246.WaitOnLoad = True
        '
        'Reag247
        '
        Me.Reag247.BackColor = System.Drawing.Color.Transparent
        Me.Reag247.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag247.Image = Nothing
        Me.Reag247.ImagePath = ""
        Me.Reag247.IsTransparentImage = False
        Me.Reag247.Location = New System.Drawing.Point(280, 420)
        Me.Reag247.Name = "Reag247"
        Me.Reag247.Rotation = 344
        Me.Reag247.ShowThrough = True
        Me.Reag247.Size = New System.Drawing.Size(122, 122)
        Me.Reag247.TabIndex = 225
        Me.Reag247.TabStop = False
        Me.Reag247.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag247.WaitOnLoad = True
        '
        'Reag248
        '
        Me.Reag248.BackColor = System.Drawing.Color.Transparent
        Me.Reag248.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag248.Image = Nothing
        Me.Reag248.ImagePath = ""
        Me.Reag248.IsTransparentImage = False
        Me.Reag248.Location = New System.Drawing.Point(306, 411)
        Me.Reag248.Name = "Reag248"
        Me.Reag248.Rotation = 336
        Me.Reag248.ShowThrough = True
        Me.Reag248.Size = New System.Drawing.Size(122, 122)
        Me.Reag248.TabIndex = 227
        Me.Reag248.TabStop = False
        Me.Reag248.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag248.WaitOnLoad = True
        '
        'Reag249
        '
        Me.Reag249.BackColor = System.Drawing.Color.Transparent
        Me.Reag249.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag249.Image = Nothing
        Me.Reag249.ImagePath = ""
        Me.Reag249.IsTransparentImage = False
        Me.Reag249.Location = New System.Drawing.Point(330, 398)
        Me.Reag249.Name = "Reag249"
        Me.Reag249.Rotation = 328
        Me.Reag249.ShowThrough = True
        Me.Reag249.Size = New System.Drawing.Size(122, 122)
        Me.Reag249.TabIndex = 228
        Me.Reag249.TabStop = False
        Me.Reag249.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag249.WaitOnLoad = True
        '
        'Reag250
        '
        Me.Reag250.BackColor = System.Drawing.Color.Transparent
        Me.Reag250.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag250.Image = Nothing
        Me.Reag250.ImagePath = ""
        Me.Reag250.IsTransparentImage = False
        Me.Reag250.Location = New System.Drawing.Point(352, 381)
        Me.Reag250.Name = "Reag250"
        Me.Reag250.Rotation = 320
        Me.Reag250.ShowThrough = True
        Me.Reag250.Size = New System.Drawing.Size(122, 122)
        Me.Reag250.TabIndex = 228
        Me.Reag250.TabStop = False
        Me.Reag250.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag250.WaitOnLoad = True
        '
        'Reag251
        '
        Me.Reag251.BackColor = System.Drawing.Color.Transparent
        Me.Reag251.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag251.Image = Nothing
        Me.Reag251.ImagePath = ""
        Me.Reag251.IsTransparentImage = False
        Me.Reag251.Location = New System.Drawing.Point(371, 362)
        Me.Reag251.Name = "Reag251"
        Me.Reag251.Rotation = 312
        Me.Reag251.ShowThrough = True
        Me.Reag251.Size = New System.Drawing.Size(122, 122)
        Me.Reag251.TabIndex = 230
        Me.Reag251.TabStop = False
        Me.Reag251.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag251.WaitOnLoad = True
        '
        'Reag252
        '
        Me.Reag252.BackColor = System.Drawing.Color.Transparent
        Me.Reag252.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag252.Image = Nothing
        Me.Reag252.ImagePath = ""
        Me.Reag252.IsTransparentImage = False
        Me.Reag252.Location = New System.Drawing.Point(387, 339)
        Me.Reag252.Name = "Reag252"
        Me.Reag252.Rotation = 304
        Me.Reag252.ShowThrough = True
        Me.Reag252.Size = New System.Drawing.Size(122, 122)
        Me.Reag252.TabIndex = 230
        Me.Reag252.TabStop = False
        Me.Reag252.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag252.WaitOnLoad = True
        '
        'Reag253
        '
        Me.Reag253.BackColor = System.Drawing.Color.Transparent
        Me.Reag253.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag253.Image = Nothing
        Me.Reag253.ImagePath = ""
        Me.Reag253.IsTransparentImage = False
        Me.Reag253.Location = New System.Drawing.Point(399, 316)
        Me.Reag253.Name = "Reag253"
        Me.Reag253.Rotation = 296
        Me.Reag253.ShowThrough = True
        Me.Reag253.Size = New System.Drawing.Size(122, 122)
        Me.Reag253.TabIndex = 231
        Me.Reag253.TabStop = False
        Me.Reag253.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag253.WaitOnLoad = True
        '
        'Reag254
        '
        Me.Reag254.BackColor = System.Drawing.Color.Transparent
        Me.Reag254.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag254.Image = Nothing
        Me.Reag254.ImagePath = ""
        Me.Reag254.IsTransparentImage = False
        Me.Reag254.Location = New System.Drawing.Point(409, 289)
        Me.Reag254.Name = "Reag254"
        Me.Reag254.Rotation = 287
        Me.Reag254.ShowThrough = True
        Me.Reag254.Size = New System.Drawing.Size(122, 122)
        Me.Reag254.TabIndex = 232
        Me.Reag254.TabStop = False
        Me.Reag254.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag254.WaitOnLoad = True
        '
        'Reag255
        '
        Me.Reag255.BackColor = System.Drawing.Color.Transparent
        Me.Reag255.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag255.Image = Nothing
        Me.Reag255.ImagePath = ""
        Me.Reag255.IsTransparentImage = False
        Me.Reag255.Location = New System.Drawing.Point(415, 262)
        Me.Reag255.Name = "Reag255"
        Me.Reag255.Rotation = 279
        Me.Reag255.ShowThrough = True
        Me.Reag255.Size = New System.Drawing.Size(122, 122)
        Me.Reag255.TabIndex = 233
        Me.Reag255.TabStop = False
        Me.Reag255.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag255.WaitOnLoad = True
        '
        'Reag256
        '
        Me.Reag256.BackColor = System.Drawing.Color.Transparent
        Me.Reag256.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag256.Image = Nothing
        Me.Reag256.ImagePath = ""
        Me.Reag256.IsTransparentImage = False
        Me.Reag256.Location = New System.Drawing.Point(417, 235)
        Me.Reag256.Name = "Reag256"
        Me.Reag256.Rotation = 271
        Me.Reag256.ShowThrough = True
        Me.Reag256.Size = New System.Drawing.Size(122, 122)
        Me.Reag256.TabIndex = 234
        Me.Reag256.TabStop = False
        Me.Reag256.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag256.WaitOnLoad = True
        '
        'Reag257
        '
        Me.Reag257.BackColor = System.Drawing.Color.Transparent
        Me.Reag257.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag257.Image = Nothing
        Me.Reag257.ImagePath = ""
        Me.Reag257.IsTransparentImage = False
        Me.Reag257.Location = New System.Drawing.Point(415, 207)
        Me.Reag257.Name = "Reag257"
        Me.Reag257.Rotation = 262
        Me.Reag257.ShowThrough = True
        Me.Reag257.Size = New System.Drawing.Size(122, 122)
        Me.Reag257.TabIndex = 235
        Me.Reag257.TabStop = False
        Me.Reag257.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag257.WaitOnLoad = True
        '
        'Reag258
        '
        Me.Reag258.BackColor = System.Drawing.Color.Transparent
        Me.Reag258.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag258.Image = Nothing
        Me.Reag258.ImagePath = ""
        Me.Reag258.IsTransparentImage = False
        Me.Reag258.Location = New System.Drawing.Point(409, 180)
        Me.Reag258.Name = "Reag258"
        Me.Reag258.Rotation = 254
        Me.Reag258.ShowThrough = True
        Me.Reag258.Size = New System.Drawing.Size(122, 122)
        Me.Reag258.TabIndex = 236
        Me.Reag258.TabStop = False
        Me.Reag258.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag258.WaitOnLoad = True
        '
        'Reag259
        '
        Me.Reag259.BackColor = System.Drawing.Color.Transparent
        Me.Reag259.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag259.Image = Nothing
        Me.Reag259.ImagePath = ""
        Me.Reag259.IsTransparentImage = False
        Me.Reag259.Location = New System.Drawing.Point(400, 154)
        Me.Reag259.Name = "Reag259"
        Me.Reag259.Rotation = 246
        Me.Reag259.ShowThrough = True
        Me.Reag259.Size = New System.Drawing.Size(122, 122)
        Me.Reag259.TabIndex = 237
        Me.Reag259.TabStop = False
        Me.Reag259.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag259.WaitOnLoad = True
        '
        'Reag260
        '
        Me.Reag260.BackColor = System.Drawing.Color.Transparent
        Me.Reag260.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag260.Image = Nothing
        Me.Reag260.ImagePath = ""
        Me.Reag260.IsTransparentImage = False
        Me.Reag260.Location = New System.Drawing.Point(388, 129)
        Me.Reag260.Name = "Reag260"
        Me.Reag260.Rotation = 237
        Me.Reag260.ShowThrough = True
        Me.Reag260.Size = New System.Drawing.Size(122, 122)
        Me.Reag260.TabIndex = 238
        Me.Reag260.TabStop = False
        Me.Reag260.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag260.WaitOnLoad = True
        '
        'Reag261
        '
        Me.Reag261.BackColor = System.Drawing.Color.Transparent
        Me.Reag261.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag261.Image = Nothing
        Me.Reag261.ImagePath = ""
        Me.Reag261.IsTransparentImage = False
        Me.Reag261.Location = New System.Drawing.Point(371, 107)
        Me.Reag261.Name = "Reag261"
        Me.Reag261.Rotation = 229
        Me.Reag261.ShowThrough = True
        Me.Reag261.Size = New System.Drawing.Size(122, 122)
        Me.Reag261.TabIndex = 239
        Me.Reag261.TabStop = False
        Me.Reag261.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag261.WaitOnLoad = True
        '
        'Reag262
        '
        Me.Reag262.BackColor = System.Drawing.Color.Transparent
        Me.Reag262.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag262.Image = Nothing
        Me.Reag262.ImagePath = ""
        Me.Reag262.IsTransparentImage = False
        Me.Reag262.Location = New System.Drawing.Point(353, 88)
        Me.Reag262.Name = "Reag262"
        Me.Reag262.Rotation = 221
        Me.Reag262.ShowThrough = True
        Me.Reag262.Size = New System.Drawing.Size(122, 122)
        Me.Reag262.TabIndex = 240
        Me.Reag262.TabStop = False
        Me.Reag262.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag262.WaitOnLoad = True
        '
        'Reag263
        '
        Me.Reag263.BackColor = System.Drawing.Color.Transparent
        Me.Reag263.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag263.Image = Nothing
        Me.Reag263.ImagePath = ""
        Me.Reag263.IsTransparentImage = False
        Me.Reag263.Location = New System.Drawing.Point(331, 72)
        Me.Reag263.Name = "Reag263"
        Me.Reag263.Rotation = 212
        Me.Reag263.ShowThrough = True
        Me.Reag263.Size = New System.Drawing.Size(122, 122)
        Me.Reag263.TabIndex = 241
        Me.Reag263.TabStop = False
        Me.Reag263.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag263.WaitOnLoad = True
        '
        'Reag264
        '
        Me.Reag264.BackColor = System.Drawing.Color.Transparent
        Me.Reag264.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag264.Image = Nothing
        Me.Reag264.ImagePath = ""
        Me.Reag264.IsTransparentImage = False
        Me.Reag264.Location = New System.Drawing.Point(306, 58)
        Me.Reag264.Name = "Reag264"
        Me.Reag264.Rotation = 205
        Me.Reag264.ShowThrough = True
        Me.Reag264.Size = New System.Drawing.Size(122, 122)
        Me.Reag264.TabIndex = 242
        Me.Reag264.TabStop = False
        Me.Reag264.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag264.WaitOnLoad = True
        '
        'Reag265
        '
        Me.Reag265.BackColor = System.Drawing.Color.Transparent
        Me.Reag265.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag265.Image = Nothing
        Me.Reag265.ImagePath = ""
        Me.Reag265.IsTransparentImage = False
        Me.Reag265.Location = New System.Drawing.Point(281, 48)
        Me.Reag265.Name = "Reag265"
        Me.Reag265.Rotation = 197
        Me.Reag265.ShowThrough = True
        Me.Reag265.Size = New System.Drawing.Size(122, 122)
        Me.Reag265.TabIndex = 243
        Me.Reag265.TabStop = False
        Me.Reag265.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag265.WaitOnLoad = True
        '
        'Reag266
        '
        Me.Reag266.BackColor = System.Drawing.Color.Transparent
        Me.Reag266.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag266.Image = Nothing
        Me.Reag266.ImagePath = ""
        Me.Reag266.IsTransparentImage = False
        Me.Reag266.Location = New System.Drawing.Point(255, 42)
        Me.Reag266.Name = "Reag266"
        Me.Reag266.Rotation = 188
        Me.Reag266.ShowThrough = True
        Me.Reag266.Size = New System.Drawing.Size(122, 122)
        Me.Reag266.TabIndex = 244
        Me.Reag266.TabStop = False
        Me.Reag266.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag266.WaitOnLoad = True
        '
        'Reag267
        '
        Me.Reag267.BackColor = System.Drawing.Color.Transparent
        Me.Reag267.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag267.Image = Nothing
        Me.Reag267.ImagePath = ""
        Me.Reag267.IsTransparentImage = False
        Me.Reag267.Location = New System.Drawing.Point(227, 41)
        Me.Reag267.Name = "Reag267"
        Me.Reag267.Rotation = 181
        Me.Reag267.ShowThrough = True
        Me.Reag267.Size = New System.Drawing.Size(122, 122)
        Me.Reag267.TabIndex = 245
        Me.Reag267.TabStop = False
        Me.Reag267.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag267.WaitOnLoad = True
        '
        'Reag268
        '
        Me.Reag268.BackColor = System.Drawing.Color.Transparent
        Me.Reag268.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag268.Image = Nothing
        Me.Reag268.ImagePath = ""
        Me.Reag268.IsTransparentImage = False
        Me.Reag268.Location = New System.Drawing.Point(201, 42)
        Me.Reag268.Name = "Reag268"
        Me.Reag268.Rotation = 173
        Me.Reag268.ShowThrough = True
        Me.Reag268.Size = New System.Drawing.Size(122, 122)
        Me.Reag268.TabIndex = 246
        Me.Reag268.TabStop = False
        Me.Reag268.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag268.WaitOnLoad = True
        '
        'Reag269
        '
        Me.Reag269.BackColor = System.Drawing.Color.Transparent
        Me.Reag269.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag269.Image = Nothing
        Me.Reag269.ImagePath = ""
        Me.Reag269.IsTransparentImage = False
        Me.Reag269.Location = New System.Drawing.Point(175, 48)
        Me.Reag269.Name = "Reag269"
        Me.Reag269.Rotation = 165
        Me.Reag269.ShowThrough = True
        Me.Reag269.Size = New System.Drawing.Size(122, 122)
        Me.Reag269.TabIndex = 247
        Me.Reag269.TabStop = False
        Me.Reag269.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag269.WaitOnLoad = True
        '
        'Reag270
        '
        Me.Reag270.BackColor = System.Drawing.Color.Transparent
        Me.Reag270.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag270.Image = Nothing
        Me.Reag270.ImagePath = ""
        Me.Reag270.IsTransparentImage = False
        Me.Reag270.Location = New System.Drawing.Point(149, 58)
        Me.Reag270.Name = "Reag270"
        Me.Reag270.Rotation = 157
        Me.Reag270.ShowThrough = True
        Me.Reag270.Size = New System.Drawing.Size(122, 122)
        Me.Reag270.TabIndex = 248
        Me.Reag270.TabStop = False
        Me.Reag270.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag270.WaitOnLoad = True
        '
        'Reag271
        '
        Me.Reag271.BackColor = System.Drawing.Color.Transparent
        Me.Reag271.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag271.Image = Nothing
        Me.Reag271.ImagePath = ""
        Me.Reag271.IsTransparentImage = False
        Me.Reag271.Location = New System.Drawing.Point(125, 71)
        Me.Reag271.Name = "Reag271"
        Me.Reag271.Rotation = 149
        Me.Reag271.ShowThrough = True
        Me.Reag271.Size = New System.Drawing.Size(122, 122)
        Me.Reag271.TabIndex = 249
        Me.Reag271.TabStop = False
        Me.Reag271.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag271.WaitOnLoad = True
        '
        'Reag272
        '
        Me.Reag272.BackColor = System.Drawing.Color.Transparent
        Me.Reag272.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag272.Image = Nothing
        Me.Reag272.ImagePath = ""
        Me.Reag272.IsTransparentImage = False
        Me.Reag272.Location = New System.Drawing.Point(103, 87)
        Me.Reag272.Name = "Reag272"
        Me.Reag272.Rotation = 140
        Me.Reag272.ShowThrough = True
        Me.Reag272.Size = New System.Drawing.Size(122, 122)
        Me.Reag272.TabIndex = 250
        Me.Reag272.TabStop = False
        Me.Reag272.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag272.WaitOnLoad = True
        '
        'Reag273
        '
        Me.Reag273.BackColor = System.Drawing.Color.Transparent
        Me.Reag273.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag273.Image = Nothing
        Me.Reag273.ImagePath = ""
        Me.Reag273.IsTransparentImage = False
        Me.Reag273.Location = New System.Drawing.Point(84, 107)
        Me.Reag273.Name = "Reag273"
        Me.Reag273.Rotation = 132
        Me.Reag273.ShowThrough = True
        Me.Reag273.Size = New System.Drawing.Size(122, 122)
        Me.Reag273.TabIndex = 251
        Me.Reag273.TabStop = False
        Me.Reag273.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag273.WaitOnLoad = True
        '
        'Reag274
        '
        Me.Reag274.BackColor = System.Drawing.Color.Transparent
        Me.Reag274.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag274.Image = Nothing
        Me.Reag274.ImagePath = ""
        Me.Reag274.IsTransparentImage = False
        Me.Reag274.Location = New System.Drawing.Point(68, 129)
        Me.Reag274.Name = "Reag274"
        Me.Reag274.Rotation = 124
        Me.Reag274.ShowThrough = True
        Me.Reag274.Size = New System.Drawing.Size(122, 122)
        Me.Reag274.TabIndex = 252
        Me.Reag274.TabStop = False
        Me.Reag274.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag274.WaitOnLoad = True
        '
        'Reag275
        '
        Me.Reag275.BackColor = System.Drawing.Color.Transparent
        Me.Reag275.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag275.Image = Nothing
        Me.Reag275.ImagePath = ""
        Me.Reag275.IsTransparentImage = False
        Me.Reag275.Location = New System.Drawing.Point(55, 153)
        Me.Reag275.Name = "Reag275"
        Me.Reag275.Rotation = 116
        Me.Reag275.ShowThrough = True
        Me.Reag275.Size = New System.Drawing.Size(122, 122)
        Me.Reag275.TabIndex = 253
        Me.Reag275.TabStop = False
        Me.Reag275.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag275.WaitOnLoad = True
        '
        'Reag276
        '
        Me.Reag276.BackColor = System.Drawing.Color.Transparent
        Me.Reag276.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag276.Image = Nothing
        Me.Reag276.ImagePath = ""
        Me.Reag276.IsTransparentImage = False
        Me.Reag276.Location = New System.Drawing.Point(46, 179)
        Me.Reag276.Name = "Reag276"
        Me.Reag276.Rotation = 108
        Me.Reag276.ShowThrough = True
        Me.Reag276.Size = New System.Drawing.Size(122, 122)
        Me.Reag276.TabIndex = 254
        Me.Reag276.TabStop = False
        Me.Reag276.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag276.WaitOnLoad = True
        '
        'Reag277
        '
        Me.Reag277.BackColor = System.Drawing.Color.Transparent
        Me.Reag277.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag277.Image = Nothing
        Me.Reag277.ImagePath = ""
        Me.Reag277.IsTransparentImage = False
        Me.Reag277.Location = New System.Drawing.Point(40, 205)
        Me.Reag277.Name = "Reag277"
        Me.Reag277.Rotation = 99
        Me.Reag277.ShowThrough = True
        Me.Reag277.Size = New System.Drawing.Size(122, 122)
        Me.Reag277.TabIndex = 255
        Me.Reag277.TabStop = False
        Me.Reag277.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag277.WaitOnLoad = True
        '
        'Reag278
        '
        Me.Reag278.BackColor = System.Drawing.Color.Transparent
        Me.Reag278.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag278.Image = Nothing
        Me.Reag278.ImagePath = ""
        Me.Reag278.IsTransparentImage = False
        Me.Reag278.Location = New System.Drawing.Point(38, 233)
        Me.Reag278.Name = "Reag278"
        Me.Reag278.Rotation = 91
        Me.Reag278.ShowThrough = True
        Me.Reag278.Size = New System.Drawing.Size(122, 122)
        Me.Reag278.TabIndex = 256
        Me.Reag278.TabStop = False
        Me.Reag278.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag278.WaitOnLoad = True
        '
        'Reag279
        '
        Me.Reag279.BackColor = System.Drawing.Color.Transparent
        Me.Reag279.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag279.Image = Nothing
        Me.Reag279.ImagePath = ""
        Me.Reag279.IsTransparentImage = False
        Me.Reag279.Location = New System.Drawing.Point(39, 261)
        Me.Reag279.Name = "Reag279"
        Me.Reag279.Rotation = 83
        Me.Reag279.ShowThrough = True
        Me.Reag279.Size = New System.Drawing.Size(122, 122)
        Me.Reag279.TabIndex = 257
        Me.Reag279.TabStop = False
        Me.Reag279.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag279.WaitOnLoad = True
        '
        'Reag280
        '
        Me.Reag280.BackColor = System.Drawing.Color.Transparent
        Me.Reag280.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag280.Image = Nothing
        Me.Reag280.ImagePath = ""
        Me.Reag280.IsTransparentImage = False
        Me.Reag280.Location = New System.Drawing.Point(46, 288)
        Me.Reag280.Name = "Reag280"
        Me.Reag280.Rotation = 75
        Me.Reag280.ShowThrough = True
        Me.Reag280.Size = New System.Drawing.Size(122, 122)
        Me.Reag280.TabIndex = 258
        Me.Reag280.TabStop = False
        Me.Reag280.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag280.WaitOnLoad = True
        '
        'Reag281
        '
        Me.Reag281.BackColor = System.Drawing.Color.Transparent
        Me.Reag281.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag281.Image = Nothing
        Me.Reag281.ImagePath = ""
        Me.Reag281.IsTransparentImage = False
        Me.Reag281.Location = New System.Drawing.Point(55, 313)
        Me.Reag281.Name = "Reag281"
        Me.Reag281.Rotation = 66
        Me.Reag281.ShowThrough = True
        Me.Reag281.Size = New System.Drawing.Size(122, 122)
        Me.Reag281.TabIndex = 259
        Me.Reag281.TabStop = False
        Me.Reag281.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag281.WaitOnLoad = True
        '
        'Reag282
        '
        Me.Reag282.BackColor = System.Drawing.Color.Transparent
        Me.Reag282.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag282.Image = Nothing
        Me.Reag282.ImagePath = ""
        Me.Reag282.IsTransparentImage = False
        Me.Reag282.Location = New System.Drawing.Point(67, 339)
        Me.Reag282.Name = "Reag282"
        Me.Reag282.Rotation = 57
        Me.Reag282.ShowThrough = True
        Me.Reag282.Size = New System.Drawing.Size(122, 122)
        Me.Reag282.TabIndex = 260
        Me.Reag282.TabStop = False
        Me.Reag282.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag282.WaitOnLoad = True
        '
        'Reag283
        '
        Me.Reag283.BackColor = System.Drawing.Color.Transparent
        Me.Reag283.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag283.Image = Nothing
        Me.Reag283.ImagePath = ""
        Me.Reag283.IsTransparentImage = False
        Me.Reag283.Location = New System.Drawing.Point(84, 361)
        Me.Reag283.Name = "Reag283"
        Me.Reag283.Rotation = 50
        Me.Reag283.ShowThrough = True
        Me.Reag283.Size = New System.Drawing.Size(122, 122)
        Me.Reag283.TabIndex = 261
        Me.Reag283.TabStop = False
        Me.Reag283.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag283.WaitOnLoad = True
        '
        'Reag284
        '
        Me.Reag284.BackColor = System.Drawing.Color.Transparent
        Me.Reag284.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag284.Image = Nothing
        Me.Reag284.ImagePath = ""
        Me.Reag284.IsTransparentImage = False
        Me.Reag284.Location = New System.Drawing.Point(102, 380)
        Me.Reag284.Name = "Reag284"
        Me.Reag284.Rotation = 41
        Me.Reag284.ShowThrough = True
        Me.Reag284.Size = New System.Drawing.Size(122, 122)
        Me.Reag284.TabIndex = 182
        Me.Reag284.TabStop = False
        Me.Reag284.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag284.WaitOnLoad = True
        '
        'Reag285
        '
        Me.Reag285.BackColor = System.Drawing.Color.Transparent
        Me.Reag285.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag285.Image = Nothing
        Me.Reag285.ImagePath = ""
        Me.Reag285.IsTransparentImage = False
        Me.Reag285.Location = New System.Drawing.Point(124, 397)
        Me.Reag285.Name = "Reag285"
        Me.Reag285.Rotation = 34
        Me.Reag285.ShowThrough = True
        Me.Reag285.Size = New System.Drawing.Size(122, 122)
        Me.Reag285.TabIndex = 181
        Me.Reag285.TabStop = False
        Me.Reag285.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag285.WaitOnLoad = True
        '
        'Reag286
        '
        Me.Reag286.BackColor = System.Drawing.Color.Transparent
        Me.Reag286.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag286.Image = Nothing
        Me.Reag286.ImagePath = ""
        Me.Reag286.IsTransparentImage = False
        Me.Reag286.Location = New System.Drawing.Point(147, 410)
        Me.Reag286.Name = "Reag286"
        Me.Reag286.Rotation = 25
        Me.Reag286.ShowThrough = True
        Me.Reag286.Size = New System.Drawing.Size(122, 122)
        Me.Reag286.TabIndex = 180
        Me.Reag286.TabStop = False
        Me.Reag286.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag286.WaitOnLoad = True
        '
        'Reag287
        '
        Me.Reag287.BackColor = System.Drawing.Color.Transparent
        Me.Reag287.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag287.Image = Nothing
        Me.Reag287.ImagePath = ""
        Me.Reag287.IsTransparentImage = False
        Me.Reag287.Location = New System.Drawing.Point(173, 420)
        Me.Reag287.Name = "Reag287"
        Me.Reag287.Rotation = 17
        Me.Reag287.ShowThrough = True
        Me.Reag287.Size = New System.Drawing.Size(122, 122)
        Me.Reag287.TabIndex = 179
        Me.Reag287.TabStop = False
        Me.Reag287.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag287.WaitOnLoad = True
        '
        'Reag288
        '
        Me.Reag288.BackColor = System.Drawing.Color.Transparent
        Me.Reag288.Direction = BSRImage.DirectionEnum.Clockwise
        Me.Reag288.Image = Nothing
        Me.Reag288.ImagePath = ""
        Me.Reag288.IsTransparentImage = False
        Me.Reag288.Location = New System.Drawing.Point(199, 426)
        Me.Reag288.Name = "Reag288"
        Me.Reag288.Rotation = 9
        Me.Reag288.ShowThrough = True
        Me.Reag288.Size = New System.Drawing.Size(122, 122)
        Me.Reag288.TabIndex = 178
        Me.Reag288.TabStop = False
        Me.Reag288.TransparentColor = System.Drawing.Color.Transparent
        Me.Reag288.WaitOnLoad = True
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.BackColor = System.Drawing.Color.Gainsboro
        Me.BsGroupBox1.Controls.Add(Me.BsRefresh)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel3)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel2)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel1)
        Me.BsGroupBox1.Controls.Add(Me.BsRotationAngle)
        Me.BsGroupBox1.Controls.Add(Me.BsLeft)
        Me.BsGroupBox1.Controls.Add(Me.BsRotate2)
        Me.BsGroupBox1.Controls.Add(Me.BsTop1)
        Me.BsGroupBox1.Controls.Add(Me.BsRotate1)
        Me.BsGroupBox1.Controls.Add(Me.BsLeft2)
        Me.BsGroupBox1.Controls.Add(Me.BsTop2)
        Me.BsGroupBox1.Controls.Add(Me.BsLeft1)
        Me.BsGroupBox1.Controls.Add(Me.BsTop)
        Me.BsGroupBox1.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox1.Location = New System.Drawing.Point(4, 445)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New System.Drawing.Size(200, 170)
        Me.BsGroupBox1.TabIndex = 35
        Me.BsGroupBox1.TabStop = False
        Me.BsGroupBox1.Text = "Size & rotate images"
        Me.BsGroupBox1.Visible = False
        '
        'BsRefresh
        '
        Me.BsRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsRefresh.Location = New System.Drawing.Point(6, 130)
        Me.BsRefresh.Name = "BsRefresh"
        Me.BsRefresh.Size = New System.Drawing.Size(74, 30)
        Me.BsRefresh.TabIndex = 29
        Me.BsRefresh.Text = "Refresh"
        Me.BsRefresh.UseVisualStyleBackColor = True
        '
        'BsLabel3
        '
        Me.BsLabel3.AutoSize = True
        Me.BsLabel3.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel3.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel3.ForeColor = System.Drawing.Color.Black
        Me.BsLabel3.Location = New System.Drawing.Point(105, 101)
        Me.BsLabel3.Name = "BsLabel3"
        Me.BsLabel3.Size = New System.Drawing.Size(89, 13)
        Me.BsLabel3.TabIndex = 28
        Me.BsLabel3.Text = "Rotation value"
        Me.BsLabel3.Title = False
        '
        'BsLabel2
        '
        Me.BsLabel2.AutoSize = True
        Me.BsLabel2.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel2.ForeColor = System.Drawing.Color.Black
        Me.BsLabel2.Location = New System.Drawing.Point(105, 67)
        Me.BsLabel2.Name = "BsLabel2"
        Me.BsLabel2.Size = New System.Drawing.Size(49, 13)
        Me.BsLabel2.TabIndex = 27
        Me.BsLabel2.Text = "Y value"
        Me.BsLabel2.Title = False
        '
        'BsLabel1
        '
        Me.BsLabel1.AutoSize = True
        Me.BsLabel1.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel1.ForeColor = System.Drawing.Color.Black
        Me.BsLabel1.Location = New System.Drawing.Point(105, 34)
        Me.BsLabel1.Name = "BsLabel1"
        Me.BsLabel1.Size = New System.Drawing.Size(50, 13)
        Me.BsLabel1.TabIndex = 26
        Me.BsLabel1.Text = "X value"
        Me.BsLabel1.Title = False
        '
        'BsRotationAngle
        '
        Me.BsRotationAngle.BackColor = System.Drawing.Color.White
        Me.BsRotationAngle.DecimalsValues = False
        Me.BsRotationAngle.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsRotationAngle.ForeColor = System.Drawing.Color.Black
        Me.BsRotationAngle.IsNumeric = False
        Me.BsRotationAngle.Location = New System.Drawing.Point(50, 98)
        Me.BsRotationAngle.Mandatory = False
        Me.BsRotationAngle.Name = "BsRotationAngle"
        Me.BsRotationAngle.ReadOnly = True
        Me.BsRotationAngle.Size = New System.Drawing.Size(52, 21)
        Me.BsRotationAngle.TabIndex = 13
        Me.BsRotationAngle.TabStop = False
        Me.BsRotationAngle.WordWrap = False
        '
        'BsLeft
        '
        Me.BsLeft.BackColor = System.Drawing.Color.White
        Me.BsLeft.DecimalsValues = False
        Me.BsLeft.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLeft.ForeColor = System.Drawing.Color.Black
        Me.BsLeft.IsNumeric = False
        Me.BsLeft.Location = New System.Drawing.Point(50, 31)
        Me.BsLeft.Mandatory = False
        Me.BsLeft.Name = "BsLeft"
        Me.BsLeft.ReadOnly = True
        Me.BsLeft.Size = New System.Drawing.Size(52, 21)
        Me.BsLeft.TabIndex = 19
        Me.BsLeft.TabStop = False
        Me.BsLeft.WordWrap = False
        '
        'BsRotate2
        '
        Me.BsRotate2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsRotate2.Location = New System.Drawing.Point(25, 92)
        Me.BsRotate2.Name = "BsRotate2"
        Me.BsRotate2.Size = New System.Drawing.Size(21, 30)
        Me.BsRotate2.TabIndex = 12
        Me.BsRotate2.Text = "-"
        Me.BsRotate2.UseVisualStyleBackColor = True
        '
        'BsTop1
        '
        Me.BsTop1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsTop1.Location = New System.Drawing.Point(2, 58)
        Me.BsTop1.Name = "BsTop1"
        Me.BsTop1.Size = New System.Drawing.Size(21, 30)
        Me.BsTop1.TabIndex = 14
        Me.BsTop1.Text = "+"
        Me.BsTop1.UseVisualStyleBackColor = True
        '
        'BsRotate1
        '
        Me.BsRotate1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsRotate1.Location = New System.Drawing.Point(2, 92)
        Me.BsRotate1.Name = "BsRotate1"
        Me.BsRotate1.Size = New System.Drawing.Size(21, 30)
        Me.BsRotate1.TabIndex = 11
        Me.BsRotate1.Text = "+"
        Me.BsRotate1.UseVisualStyleBackColor = True
        '
        'BsLeft2
        '
        Me.BsLeft2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsLeft2.Location = New System.Drawing.Point(25, 25)
        Me.BsLeft2.Name = "BsLeft2"
        Me.BsLeft2.Size = New System.Drawing.Size(21, 30)
        Me.BsLeft2.TabIndex = 18
        Me.BsLeft2.Text = "-"
        Me.BsLeft2.UseVisualStyleBackColor = True
        '
        'BsTop2
        '
        Me.BsTop2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsTop2.Location = New System.Drawing.Point(25, 58)
        Me.BsTop2.Name = "BsTop2"
        Me.BsTop2.Size = New System.Drawing.Size(21, 30)
        Me.BsTop2.TabIndex = 15
        Me.BsTop2.Text = "-"
        Me.BsTop2.UseVisualStyleBackColor = True
        '
        'BsLeft1
        '
        Me.BsLeft1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsLeft1.Location = New System.Drawing.Point(2, 25)
        Me.BsLeft1.Name = "BsLeft1"
        Me.BsLeft1.Size = New System.Drawing.Size(21, 30)
        Me.BsLeft1.TabIndex = 17
        Me.BsLeft1.Text = "+"
        Me.BsLeft1.UseVisualStyleBackColor = True
        '
        'BsTop
        '
        Me.BsTop.BackColor = System.Drawing.Color.White
        Me.BsTop.DecimalsValues = False
        Me.BsTop.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsTop.ForeColor = System.Drawing.Color.Black
        Me.BsTop.IsNumeric = False
        Me.BsTop.Location = New System.Drawing.Point(50, 66)
        Me.BsTop.Mandatory = False
        Me.BsTop.Name = "BsTop"
        Me.BsTop.ReadOnly = True
        Me.BsTop.Size = New System.Drawing.Size(52, 21)
        Me.BsTop.TabIndex = 16
        Me.BsTop.TabStop = False
        Me.BsTop.WordWrap = False
        '
        'bsElementsTreeView
        '
        Me.bsElementsTreeView.AllowDrop = True
        Me.bsElementsTreeView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsElementsTreeView.BackColor = System.Drawing.Color.White
        Me.bsElementsTreeView.Font = New System.Drawing.Font("Arial", 6.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsElementsTreeView.ForeColor = System.Drawing.Color.Black
        Me.bsElementsTreeView.FullRowSelect = True
        Me.bsElementsTreeView.Location = New System.Drawing.Point(2, 29)
        Me.bsElementsTreeView.Name = "bsElementsTreeView"
        Me.bsElementsTreeView.ShowNodeToolTips = True
        Me.bsElementsTreeView.Size = New System.Drawing.Size(201, 588)
        Me.bsElementsTreeView.TabIndex = 34
        '
        'bsRequiredElementsLabel
        '
        Me.bsRequiredElementsLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsRequiredElementsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsRequiredElementsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsRequiredElementsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRequiredElementsLabel.Location = New System.Drawing.Point(4, 5)
        Me.bsRequiredElementsLabel.Name = "bsRequiredElementsLabel"
        Me.bsRequiredElementsLabel.Size = New System.Drawing.Size(202, 20)
        Me.bsRequiredElementsLabel.TabIndex = 33
        Me.bsRequiredElementsLabel.Text = "Required Elements"
        Me.bsRequiredElementsLabel.Title = True
        '
        'IWSRotorPositions
        '
        Me.AcceptButton = Me.bsAcceptButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.Controls.Add(Me.BsGroupBox1)
        Me.Controls.Add(Me.bsElementsTreeView)
        Me.Controls.Add(Me.bsRequiredElementsLabel)
        Me.Controls.Add(Me.RotorsTabs)
        Me.Controls.Add(Me.FunctionalityArea)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "DevExpress Dark Style"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IWSRotorPositions"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "A400 Reagent & Sample Positioning"
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FunctionalityArea.ResumeLayout(False)
        CType(Me.RotorsTabs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RotorsTabs.ResumeLayout(False)
        Me.SamplesTab.ResumeLayout(False)
        CType(Me.Sam3127, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3128, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3129, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3130, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3131, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3132, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3133, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3135, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl1.ResumeLayout(False)
        CType(Me.PanelControl2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl2.ResumeLayout(False)
        Me.bsSamplesLegendGroupBox.ResumeLayout(False)
        CType(Me.bsTubeAddSolPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsDilutedPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsRoutinePictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsStatPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsControlPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsCalibratorPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsSamplesPositionInfoGroupBox.ResumeLayout(False)
        Me.bsSamplesPositionInfoGroupBox.PerformLayout()
        CType(Me.Sam3106, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam395, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam396, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam397, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3107, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3134, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3126, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3125, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3124, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3123, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3122, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3121, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3120, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3119, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3118, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3117, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3116, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3115, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3114, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3113, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3112, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3111, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3110, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3109, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3108, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3105, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3104, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3103, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3102, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3101, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam3100, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam399, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam398, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam394, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam393, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam392, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam391, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam290, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam289, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam288, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam287, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam286, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam285, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam284, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam283, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam282, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam281, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam280, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam279, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam278, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam277, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam276, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam275, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam274, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam273, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam272, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam271, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam270, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam269, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam268, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam267, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam266, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam265, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam264, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam263, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam262, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam261, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam260, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam259, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam258, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam257, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam256, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam255, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam254, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam253, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam252, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam251, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam250, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam249, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam248, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam247, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam246, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam145, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam144, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam143, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam142, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam11, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam141, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam140, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam139, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam138, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam137, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam136, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam135, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam134, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam133, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam132, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam131, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam130, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam129, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam128, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam127, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam126, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam125, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam124, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam123, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam122, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam121, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam120, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam119, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam118, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam117, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam116, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam115, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam114, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam113, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam112, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam111, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam110, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam19, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam18, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam17, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam16, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam15, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam14, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam13, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Sam12, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ReagentsTab.ResumeLayout(False)
        CType(Me.PanelControl6, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl6.ResumeLayout(False)
        CType(Me.PanelControl7, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl7.ResumeLayout(False)
        Me.bsReagentsLegendGroupBox.ResumeLayout(False)
        CType(Me.SelectedPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LegendUnknownImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LegendBarCodeErrorRGImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LowVolPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ReagentPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AdditionalSolPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NoInUsePictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsDepletedPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsReagentsPositionInfoGroupBox.ResumeLayout(False)
        Me.bsReagentsPositionInfoGroupBox.PerformLayout()
        CType(Me.Reag11, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag12, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag13, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag14, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag15, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag16, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag17, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag18, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag19, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag110, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag111, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag112, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag113, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag114, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag115, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag116, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag117, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag118, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag119, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag120, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag121, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag122, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag123, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag124, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag125, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag126, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag127, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag128, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag129, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag130, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag131, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag132, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag133, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag134, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag135, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag136, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag137, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag138, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag139, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag140, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag141, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag142, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag143, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag144, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag245, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag246, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag247, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag248, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag249, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag250, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag251, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag252, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag253, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag254, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag255, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag256, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag257, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag258, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag259, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag260, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag261, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag262, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag263, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag264, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag265, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag266, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag267, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag268, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag269, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag270, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag271, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag272, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag273, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag274, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag275, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag276, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag277, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag278, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag279, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag280, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag281, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag282, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag283, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag284, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag285, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag286, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag287, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Reag288, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsGroupBox1.ResumeLayout(False)
        Me.BsGroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsScreenTimer As System.Windows.Forms.Timer
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents FunctionalityArea As System.Windows.Forms.Panel
    Friend WithEvents BarcodeWarningButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScanningButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCheckRotorVolumeButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsWarningsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSaveVRotorButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLoadVRotorButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsResetRotorButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReagentAutoPosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSamplesAutoPosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents RotorsTabs As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents SamplesTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents Sam3127 As BSRImage
    Friend WithEvents Sam3128 As BSRImage
    Friend WithEvents Sam3129 As BSRImage
    Friend WithEvents Sam3130 As BSRImage
    Friend WithEvents Sam3131 As BSRImage
    Friend WithEvents Sam3132 As BSRImage
    Friend WithEvents Sam3133 As BSRImage
    Friend WithEvents Sam3135 As BSRImage
    Friend WithEvents PanelControl1 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents PanelControl2 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents bsSamplesLegendGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTubeAddSolLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTubeAddSolPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsLegendDilutedLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDilutedPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsLegendRoutineLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLegendStatLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLegendControlsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLegendCalibratorsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRoutinePictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsStatPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsControlPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsCalibratorPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsSamplesLegendLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesPositionInfoGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents SamplesStatusTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSamplesStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesPositionInfoLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesDeletePosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSamplesMoveLastPositionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSamplesRefillPosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSamplesIncreaseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTubeSizeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsSamplesBarcodeTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSamplesDecreaseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDiluteStatusTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleTypeTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSamplesMoveFirstPositionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSampleNumberTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleContentTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleRingNumTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSampleCellTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsTubeSizeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesBarcodeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDiluteStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesNumberLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSampleIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesContentLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesRingNumLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSamplesCellLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents Sam3106 As BSRImage
    Friend WithEvents Sam395 As BSRImage
    Friend WithEvents Sam396 As BSRImage
    Friend WithEvents Sam397 As BSRImage
    Friend WithEvents Sam3107 As BSRImage
    Friend WithEvents Sam3134 As BSRImage
    Friend WithEvents Sam3126 As BSRImage
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
    Friend WithEvents Sam3115 As BSRImage
    Friend WithEvents Sam3114 As BSRImage
    Friend WithEvents Sam3113 As BSRImage
    Friend WithEvents Sam3112 As BSRImage
    Friend WithEvents Sam3111 As BSRImage
    Friend WithEvents Sam3110 As BSRImage
    Friend WithEvents Sam3109 As BSRImage
    Friend WithEvents Sam3108 As BSRImage
    Friend WithEvents Sam3105 As BSRImage
    Friend WithEvents Sam3104 As BSRImage
    Friend WithEvents Sam3103 As BSRImage
    Friend WithEvents Sam3102 As BSRImage
    Friend WithEvents Sam3101 As BSRImage
    Friend WithEvents Sam3100 As BSRImage
    Friend WithEvents Sam399 As BSRImage
    Friend WithEvents Sam398 As BSRImage
    Friend WithEvents Sam394 As BSRImage
    Friend WithEvents Sam393 As BSRImage
    Friend WithEvents Sam392 As BSRImage
    Friend WithEvents Sam391 As BSRImage
    Friend WithEvents Sam290 As BSRImage
    Friend WithEvents Sam289 As BSRImage
    Friend WithEvents Sam288 As BSRImage
    Friend WithEvents Sam287 As BSRImage
    Friend WithEvents Sam286 As BSRImage
    Friend WithEvents Sam285 As BSRImage
    Friend WithEvents Sam284 As BSRImage
    Friend WithEvents Sam283 As BSRImage
    Friend WithEvents Sam282 As BSRImage
    Friend WithEvents Sam281 As BSRImage
    Friend WithEvents Sam280 As BSRImage
    Friend WithEvents Sam279 As BSRImage
    Friend WithEvents Sam278 As BSRImage
    Friend WithEvents Sam277 As BSRImage
    Friend WithEvents Sam276 As BSRImage
    Friend WithEvents Sam275 As BSRImage
    Friend WithEvents Sam274 As BSRImage
    Friend WithEvents Sam273 As BSRImage
    Friend WithEvents Sam272 As BSRImage
    Friend WithEvents Sam271 As BSRImage
    Friend WithEvents Sam270 As BSRImage
    Friend WithEvents Sam269 As BSRImage
    Friend WithEvents Sam268 As BSRImage
    Friend WithEvents Sam267 As BSRImage
    Friend WithEvents Sam266 As BSRImage
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
    Friend WithEvents Sam145 As BSRImage
    Friend WithEvents Sam144 As BSRImage
    Friend WithEvents Sam143 As BSRImage
    Friend WithEvents Sam142 As BSRImage
    Friend WithEvents Sam11 As BSRImage
    Friend WithEvents Sam141 As BSRImage
    Friend WithEvents Sam140 As BSRImage
    Friend WithEvents Sam139 As BSRImage
    Friend WithEvents Sam138 As BSRImage
    Friend WithEvents Sam137 As BSRImage
    Friend WithEvents Sam136 As BSRImage
    Friend WithEvents Sam135 As BSRImage
    Friend WithEvents Sam134 As BSRImage
    Friend WithEvents Sam133 As BSRImage
    Friend WithEvents Sam132 As BSRImage
    Friend WithEvents Sam131 As BSRImage
    Friend WithEvents Sam130 As BSRImage
    Friend WithEvents Sam129 As BSRImage
    Friend WithEvents Sam128 As BSRImage
    Friend WithEvents Sam127 As BSRImage
    Friend WithEvents Sam126 As BSRImage
    Friend WithEvents Sam125 As BSRImage
    Friend WithEvents Sam124 As BSRImage
    Friend WithEvents Sam123 As BSRImage
    Friend WithEvents Sam122 As BSRImage
    Friend WithEvents Sam121 As BSRImage
    Friend WithEvents Sam120 As BSRImage
    Friend WithEvents Sam119 As BSRImage
    Friend WithEvents Sam118 As BSRImage
    Friend WithEvents Sam117 As BSRImage
    Friend WithEvents Sam116 As BSRImage
    Friend WithEvents Sam115 As BSRImage
    Friend WithEvents Sam114 As BSRImage
    Friend WithEvents Sam113 As BSRImage
    Friend WithEvents Sam112 As BSRImage
    Friend WithEvents Sam111 As BSRImage
    Friend WithEvents Sam110 As BSRImage
    Friend WithEvents Sam19 As BSRImage
    Friend WithEvents Sam18 As BSRImage
    Friend WithEvents Sam17 As BSRImage
    Friend WithEvents Sam16 As BSRImage
    Friend WithEvents Sam15 As BSRImage
    Friend WithEvents Sam14 As BSRImage
    Friend WithEvents Sam13 As BSRImage
    Friend WithEvents Sam12 As BSRImage
    Friend WithEvents ReagentsTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents PanelControl6 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents PanelControl7 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents bsReagentsLegendGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents LegReagentSelLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents SelectedPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents LegendUnknownImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsUnknownLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LegendBarCodeErrorRGImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsBarcodeErrorRGLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LowVolPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents ReagentPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsLegReagLowVolLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLegReagentLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLegReagAdditionalSol As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLegReagNoInUseLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLegReagDepleteLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents AdditionalSolPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents NoInUsePictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsDepletedPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsReagentsLegendLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsPositionInfoGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsReagStatusLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents ReagStatusTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsCellTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsCellLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsDeletePosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTeststLeftTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsRefillPosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCurrentVolTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsCheckVolumePosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTestsLeftLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsCurrentVolLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsBottleSizeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsBottleSizeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsExpirationDateTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsPositionInfoLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsMoveLastPositionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReagentsBarCodeTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsTestNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsIncreaseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReagentsNumberTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsDecreaseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReagentNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsMoveFirstPositionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsReagentsContentTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsReagentsRingNumTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsExpirationDateLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsBarCodeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTestNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsNumberLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsContentLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReagentsRingNumLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
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
    Friend WithEvents BsGroupBox1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsRefresh As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsLabel3 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel2 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsRotationAngle As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsLeft As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsRotate2 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTop1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsRotate1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsLeft2 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTop2 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsLeft1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTop As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsElementsTreeView As Biosystems.Ax00.Controls.UserControls.BSTreeView
    Friend WithEvents bsRequiredElementsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
End Class

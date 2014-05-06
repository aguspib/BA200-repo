<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ISE_Test
    Inherits System.Windows.Forms.Form

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
        Me.tabMain = New Biosystems.Ax00.Controls.UserControls.BSTabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.grpISE = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.cmbISEAction = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.grpReceived = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.grpCalib2 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.txtCalib2Cl = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblCalib2Cl = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtCalib2K = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblCalib2K = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtCalib2Na = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblCalib2Na = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtCalib2Res = New Biosystems.Ax00.Controls.UserControls.BSMaskedTextBox
        Me.lblCalib2Res = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtCalib2Li = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblCalib2Li = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.chkCalib2 = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.grpCalib1 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.txtCalib1Cl = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblCalib1Cl = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtCalib1K = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.LblCalib1K = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtCalib1Na = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblCalib1Na = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtCalib1Res = New Biosystems.Ax00.Controls.UserControls.BSMaskedTextBox
        Me.lblCalib1Res = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtCalib1Li = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblCalib1Li = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.chkCalib1 = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.butGenerateProcess = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.grpPumps = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.txtPumpsW = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblPumpsW = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtPumpsB = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblPumpsB = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtPumpsA = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblPumpsA = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.chkPumps = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.grpERC = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.chkERC = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.lblERCCode = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.cmbERCCode = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.txtERCRes = New Biosystems.Ax00.Controls.UserControls.BSMaskedTextBox
        Me.lblERCRes = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.grpBubbles = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.txtBubblesL = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblBubblesL = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtBubblesM = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblBubblesM = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.txtBubblesA = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.lblBubblesA = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.chkBubbles = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.lblISEAction = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.SendReceiveTab = New System.Windows.Forms.TabPage
        Me.grpProcessed = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.chkOnlyTreated = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.txtProcessedData = New System.Windows.Forms.TextBox
        Me.butClearProcessed = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.grpReceivedMessage = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.txtReceivedData = New System.Windows.Forms.TextBox
        Me.butProcess = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.butClearReceived = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.butTestGrid = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.butHistAlarms = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.chkCombo = New DevExpress.XtraEditors.CheckedComboBoxEdit
        Me.comboLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.comboProperties = New System.Windows.Forms.PropertyGrid
        Me.tabMain.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.grpISE.SuspendLayout()
        Me.grpReceived.SuspendLayout()
        Me.grpCalib2.SuspendLayout()
        Me.grpCalib1.SuspendLayout()
        Me.grpPumps.SuspendLayout()
        Me.grpERC.SuspendLayout()
        Me.grpBubbles.SuspendLayout()
        Me.SendReceiveTab.SuspendLayout()
        Me.grpProcessed.SuspendLayout()
        Me.grpReceivedMessage.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        CType(Me.chkCombo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.TabPage1)
        Me.tabMain.Controls.Add(Me.SendReceiveTab)
        Me.tabMain.Controls.Add(Me.TabPage2)
        Me.tabMain.Controls.Add(Me.TabPage3)
        Me.tabMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabMain.Location = New System.Drawing.Point(0, 0)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(1052, 303)
        Me.tabMain.TabIndex = 52
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.grpISE)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(1044, 277)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "ISE Calibrations"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'grpISE
        '
        Me.grpISE.Controls.Add(Me.cmbISEAction)
        Me.grpISE.Controls.Add(Me.grpReceived)
        Me.grpISE.Controls.Add(Me.lblISEAction)
        Me.grpISE.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpISE.ForeColor = System.Drawing.Color.Black
        Me.grpISE.Location = New System.Drawing.Point(3, 3)
        Me.grpISE.Name = "grpISE"
        Me.grpISE.Size = New System.Drawing.Size(1038, 271)
        Me.grpISE.TabIndex = 51
        Me.grpISE.TabStop = False
        '
        'cmbISEAction
        '
        Me.cmbISEAction.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.cmbISEAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbISEAction.FormattingEnabled = True
        Me.cmbISEAction.Items.AddRange(New Object() {"None", "CalibrateElectrodes", "CalibratePumps", "CalibrateBubbles", "Clean"})
        Me.cmbISEAction.Location = New System.Drawing.Point(479, 19)
        Me.cmbISEAction.Name = "cmbISEAction"
        Me.cmbISEAction.Size = New System.Drawing.Size(182, 21)
        Me.cmbISEAction.TabIndex = 48
        '
        'grpReceived
        '
        Me.grpReceived.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpReceived.Controls.Add(Me.grpCalib2)
        Me.grpReceived.Controls.Add(Me.grpCalib1)
        Me.grpReceived.Controls.Add(Me.butGenerateProcess)
        Me.grpReceived.Controls.Add(Me.grpPumps)
        Me.grpReceived.Controls.Add(Me.grpERC)
        Me.grpReceived.Controls.Add(Me.grpBubbles)
        Me.grpReceived.ForeColor = System.Drawing.Color.Black
        Me.grpReceived.Location = New System.Drawing.Point(6, 46)
        Me.grpReceived.Name = "grpReceived"
        Me.grpReceived.Size = New System.Drawing.Size(1026, 204)
        Me.grpReceived.TabIndex = 49
        Me.grpReceived.TabStop = False
        Me.grpReceived.Text = "Received Data"
        '
        'grpCalib2
        '
        Me.grpCalib2.Controls.Add(Me.txtCalib2Cl)
        Me.grpCalib2.Controls.Add(Me.lblCalib2Cl)
        Me.grpCalib2.Controls.Add(Me.txtCalib2K)
        Me.grpCalib2.Controls.Add(Me.lblCalib2K)
        Me.grpCalib2.Controls.Add(Me.txtCalib2Na)
        Me.grpCalib2.Controls.Add(Me.lblCalib2Na)
        Me.grpCalib2.Controls.Add(Me.txtCalib2Res)
        Me.grpCalib2.Controls.Add(Me.lblCalib2Res)
        Me.grpCalib2.Controls.Add(Me.txtCalib2Li)
        Me.grpCalib2.Controls.Add(Me.lblCalib2Li)
        Me.grpCalib2.Controls.Add(Me.chkCalib2)
        Me.grpCalib2.ForeColor = System.Drawing.Color.Black
        Me.grpCalib2.Location = New System.Drawing.Point(516, 19)
        Me.grpCalib2.Name = "grpCalib2"
        Me.grpCalib2.Size = New System.Drawing.Size(504, 73)
        Me.grpCalib2.TabIndex = 52
        Me.grpCalib2.TabStop = False
        Me.grpCalib2.Text = "Calibration 2"
        Me.grpCalib2.Visible = False
        '
        'txtCalib2Cl
        '
        Me.txtCalib2Cl.BackColor = System.Drawing.Color.White
        Me.txtCalib2Cl.DecimalsValues = False
        Me.txtCalib2Cl.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtCalib2Cl.IsNumeric = False
        Me.txtCalib2Cl.Location = New System.Drawing.Point(449, 17)
        Me.txtCalib2Cl.Mandatory = False
        Me.txtCalib2Cl.Name = "txtCalib2Cl"
        Me.txtCalib2Cl.Size = New System.Drawing.Size(47, 21)
        Me.txtCalib2Cl.TabIndex = 7
        Me.txtCalib2Cl.Text = "42.31"
        '
        'lblCalib2Cl
        '
        Me.lblCalib2Cl.AutoSize = True
        Me.lblCalib2Cl.BackColor = System.Drawing.Color.Transparent
        Me.lblCalib2Cl.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblCalib2Cl.ForeColor = System.Drawing.Color.Black
        Me.lblCalib2Cl.Location = New System.Drawing.Point(422, 20)
        Me.lblCalib2Cl.Name = "lblCalib2Cl"
        Me.lblCalib2Cl.Size = New System.Drawing.Size(24, 13)
        Me.lblCalib2Cl.TabIndex = 6
        Me.lblCalib2Cl.Text = "Cl:"
        Me.lblCalib2Cl.Title = False
        '
        'txtCalib2K
        '
        Me.txtCalib2K.BackColor = System.Drawing.Color.White
        Me.txtCalib2K.DecimalsValues = False
        Me.txtCalib2K.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtCalib2K.IsNumeric = False
        Me.txtCalib2K.Location = New System.Drawing.Point(360, 17)
        Me.txtCalib2K.Mandatory = False
        Me.txtCalib2K.Name = "txtCalib2K"
        Me.txtCalib2K.Size = New System.Drawing.Size(47, 21)
        Me.txtCalib2K.TabIndex = 5
        Me.txtCalib2K.Text = "55.41"
        '
        'lblCalib2K
        '
        Me.lblCalib2K.AutoSize = True
        Me.lblCalib2K.BackColor = System.Drawing.Color.Transparent
        Me.lblCalib2K.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblCalib2K.ForeColor = System.Drawing.Color.Black
        Me.lblCalib2K.Location = New System.Drawing.Point(333, 20)
        Me.lblCalib2K.Name = "lblCalib2K"
        Me.lblCalib2K.Size = New System.Drawing.Size(20, 13)
        Me.lblCalib2K.TabIndex = 1
        Me.lblCalib2K.Text = "K:"
        Me.lblCalib2K.Title = False
        '
        'txtCalib2Na
        '
        Me.txtCalib2Na.BackColor = System.Drawing.Color.White
        Me.txtCalib2Na.DecimalsValues = False
        Me.txtCalib2Na.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtCalib2Na.IsNumeric = False
        Me.txtCalib2Na.Location = New System.Drawing.Point(273, 17)
        Me.txtCalib2Na.Mandatory = False
        Me.txtCalib2Na.Name = "txtCalib2Na"
        Me.txtCalib2Na.Size = New System.Drawing.Size(47, 21)
        Me.txtCalib2Na.TabIndex = 4
        Me.txtCalib2Na.Text = "53.23"
        '
        'lblCalib2Na
        '
        Me.lblCalib2Na.AutoSize = True
        Me.lblCalib2Na.BackColor = System.Drawing.Color.Transparent
        Me.lblCalib2Na.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblCalib2Na.ForeColor = System.Drawing.Color.Black
        Me.lblCalib2Na.Location = New System.Drawing.Point(246, 20)
        Me.lblCalib2Na.Name = "lblCalib2Na"
        Me.lblCalib2Na.Size = New System.Drawing.Size(27, 13)
        Me.lblCalib2Na.TabIndex = 3
        Me.lblCalib2Na.Text = "Na:"
        Me.lblCalib2Na.Title = False
        '
        'txtCalib2Res
        '
        Me.txtCalib2Res.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.txtCalib2Res.BackColor = System.Drawing.Color.White
        Me.txtCalib2Res.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtCalib2Res.Location = New System.Drawing.Point(295, 44)
        Me.txtCalib2Res.Mask = "AAAAAAA"
        Me.txtCalib2Res.Name = "txtCalib2Res"
        Me.txtCalib2Res.Size = New System.Drawing.Size(66, 21)
        Me.txtCalib2Res.TabIndex = 8
        Me.txtCalib2Res.Text = "0000000"
        '
        'lblCalib2Res
        '
        Me.lblCalib2Res.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lblCalib2Res.AutoSize = True
        Me.lblCalib2Res.BackColor = System.Drawing.Color.Transparent
        Me.lblCalib2Res.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblCalib2Res.ForeColor = System.Drawing.Color.Black
        Me.lblCalib2Res.Location = New System.Drawing.Point(153, 47)
        Me.lblCalib2Res.Name = "lblCalib2Res"
        Me.lblCalib2Res.Size = New System.Drawing.Size(136, 13)
        Me.lblCalib2Res.TabIndex = 1
        Me.lblCalib2Res.Text = "Alarm codes (7 hexa):"
        Me.lblCalib2Res.Title = False
        '
        'txtCalib2Li
        '
        Me.txtCalib2Li.BackColor = System.Drawing.Color.White
        Me.txtCalib2Li.DecimalsValues = False
        Me.txtCalib2Li.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtCalib2Li.IsNumeric = False
        Me.txtCalib2Li.Location = New System.Drawing.Point(185, 17)
        Me.txtCalib2Li.Mandatory = False
        Me.txtCalib2Li.Name = "txtCalib2Li"
        Me.txtCalib2Li.Size = New System.Drawing.Size(47, 21)
        Me.txtCalib2Li.TabIndex = 2
        Me.txtCalib2Li.Text = "22.31"
        '
        'lblCalib2Li
        '
        Me.lblCalib2Li.AutoSize = True
        Me.lblCalib2Li.BackColor = System.Drawing.Color.Transparent
        Me.lblCalib2Li.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblCalib2Li.ForeColor = System.Drawing.Color.Black
        Me.lblCalib2Li.Location = New System.Drawing.Point(158, 20)
        Me.lblCalib2Li.Name = "lblCalib2Li"
        Me.lblCalib2Li.Size = New System.Drawing.Size(21, 13)
        Me.lblCalib2Li.TabIndex = 1
        Me.lblCalib2Li.Text = "Li:"
        Me.lblCalib2Li.Title = False
        '
        'chkCalib2
        '
        Me.chkCalib2.AutoSize = True
        Me.chkCalib2.Checked = True
        Me.chkCalib2.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCalib2.Location = New System.Drawing.Point(6, 19)
        Me.chkCalib2.Name = "chkCalib2"
        Me.chkCalib2.Size = New System.Drawing.Size(146, 17)
        Me.chkCalib2.TabIndex = 0
        Me.chkCalib2.Text = "Include Calibration Result"
        Me.chkCalib2.UseVisualStyleBackColor = True
        '
        'grpCalib1
        '
        Me.grpCalib1.Controls.Add(Me.txtCalib1Cl)
        Me.grpCalib1.Controls.Add(Me.lblCalib1Cl)
        Me.grpCalib1.Controls.Add(Me.txtCalib1K)
        Me.grpCalib1.Controls.Add(Me.LblCalib1K)
        Me.grpCalib1.Controls.Add(Me.txtCalib1Na)
        Me.grpCalib1.Controls.Add(Me.lblCalib1Na)
        Me.grpCalib1.Controls.Add(Me.txtCalib1Res)
        Me.grpCalib1.Controls.Add(Me.lblCalib1Res)
        Me.grpCalib1.Controls.Add(Me.txtCalib1Li)
        Me.grpCalib1.Controls.Add(Me.lblCalib1Li)
        Me.grpCalib1.Controls.Add(Me.chkCalib1)
        Me.grpCalib1.ForeColor = System.Drawing.Color.Black
        Me.grpCalib1.Location = New System.Drawing.Point(6, 19)
        Me.grpCalib1.Name = "grpCalib1"
        Me.grpCalib1.Size = New System.Drawing.Size(504, 73)
        Me.grpCalib1.TabIndex = 52
        Me.grpCalib1.TabStop = False
        Me.grpCalib1.Text = "Calibration 1"
        Me.grpCalib1.Visible = False
        '
        'txtCalib1Cl
        '
        Me.txtCalib1Cl.BackColor = System.Drawing.Color.White
        Me.txtCalib1Cl.DecimalsValues = False
        Me.txtCalib1Cl.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtCalib1Cl.IsNumeric = False
        Me.txtCalib1Cl.Location = New System.Drawing.Point(449, 17)
        Me.txtCalib1Cl.Mandatory = False
        Me.txtCalib1Cl.Name = "txtCalib1Cl"
        Me.txtCalib1Cl.Size = New System.Drawing.Size(47, 21)
        Me.txtCalib1Cl.TabIndex = 2
        Me.txtCalib1Cl.Text = "42.30"
        '
        'lblCalib1Cl
        '
        Me.lblCalib1Cl.AutoSize = True
        Me.lblCalib1Cl.BackColor = System.Drawing.Color.Transparent
        Me.lblCalib1Cl.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblCalib1Cl.ForeColor = System.Drawing.Color.Black
        Me.lblCalib1Cl.Location = New System.Drawing.Point(422, 20)
        Me.lblCalib1Cl.Name = "lblCalib1Cl"
        Me.lblCalib1Cl.Size = New System.Drawing.Size(24, 13)
        Me.lblCalib1Cl.TabIndex = 4
        Me.lblCalib1Cl.Text = "Cl:"
        Me.lblCalib1Cl.Title = False
        '
        'txtCalib1K
        '
        Me.txtCalib1K.BackColor = System.Drawing.Color.White
        Me.txtCalib1K.DecimalsValues = False
        Me.txtCalib1K.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtCalib1K.IsNumeric = False
        Me.txtCalib1K.Location = New System.Drawing.Point(360, 17)
        Me.txtCalib1K.Mandatory = False
        Me.txtCalib1K.Name = "txtCalib1K"
        Me.txtCalib1K.Size = New System.Drawing.Size(47, 21)
        Me.txtCalib1K.TabIndex = 3
        Me.txtCalib1K.Text = "55.40"
        '
        'LblCalib1K
        '
        Me.LblCalib1K.AutoSize = True
        Me.LblCalib1K.BackColor = System.Drawing.Color.Transparent
        Me.LblCalib1K.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LblCalib1K.ForeColor = System.Drawing.Color.Black
        Me.LblCalib1K.Location = New System.Drawing.Point(333, 20)
        Me.LblCalib1K.Name = "LblCalib1K"
        Me.LblCalib1K.Size = New System.Drawing.Size(20, 13)
        Me.LblCalib1K.TabIndex = 1
        Me.LblCalib1K.Text = "K:"
        Me.LblCalib1K.Title = False
        '
        'txtCalib1Na
        '
        Me.txtCalib1Na.BackColor = System.Drawing.Color.White
        Me.txtCalib1Na.DecimalsValues = False
        Me.txtCalib1Na.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtCalib1Na.IsNumeric = False
        Me.txtCalib1Na.Location = New System.Drawing.Point(273, 17)
        Me.txtCalib1Na.Mandatory = False
        Me.txtCalib1Na.Name = "txtCalib1Na"
        Me.txtCalib1Na.Size = New System.Drawing.Size(47, 21)
        Me.txtCalib1Na.TabIndex = 2
        Me.txtCalib1Na.Text = "53.22"
        '
        'lblCalib1Na
        '
        Me.lblCalib1Na.AutoSize = True
        Me.lblCalib1Na.BackColor = System.Drawing.Color.Transparent
        Me.lblCalib1Na.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblCalib1Na.ForeColor = System.Drawing.Color.Black
        Me.lblCalib1Na.Location = New System.Drawing.Point(246, 20)
        Me.lblCalib1Na.Name = "lblCalib1Na"
        Me.lblCalib1Na.Size = New System.Drawing.Size(27, 13)
        Me.lblCalib1Na.TabIndex = 1
        Me.lblCalib1Na.Text = "Na:"
        Me.lblCalib1Na.Title = False
        '
        'txtCalib1Res
        '
        Me.txtCalib1Res.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.txtCalib1Res.BackColor = System.Drawing.Color.White
        Me.txtCalib1Res.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtCalib1Res.Location = New System.Drawing.Point(295, 44)
        Me.txtCalib1Res.Mask = "AAAAAAA"
        Me.txtCalib1Res.Name = "txtCalib1Res"
        Me.txtCalib1Res.Size = New System.Drawing.Size(66, 21)
        Me.txtCalib1Res.TabIndex = 5
        Me.txtCalib1Res.Text = "0000000"
        '
        'lblCalib1Res
        '
        Me.lblCalib1Res.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lblCalib1Res.AutoSize = True
        Me.lblCalib1Res.BackColor = System.Drawing.Color.Transparent
        Me.lblCalib1Res.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblCalib1Res.ForeColor = System.Drawing.Color.Black
        Me.lblCalib1Res.Location = New System.Drawing.Point(153, 47)
        Me.lblCalib1Res.Name = "lblCalib1Res"
        Me.lblCalib1Res.Size = New System.Drawing.Size(136, 13)
        Me.lblCalib1Res.TabIndex = 1
        Me.lblCalib1Res.Text = "Alarm codes (7 hexa):"
        Me.lblCalib1Res.Title = False
        '
        'txtCalib1Li
        '
        Me.txtCalib1Li.BackColor = System.Drawing.Color.White
        Me.txtCalib1Li.DecimalsValues = False
        Me.txtCalib1Li.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtCalib1Li.IsNumeric = False
        Me.txtCalib1Li.Location = New System.Drawing.Point(185, 17)
        Me.txtCalib1Li.Mandatory = False
        Me.txtCalib1Li.Name = "txtCalib1Li"
        Me.txtCalib1Li.Size = New System.Drawing.Size(47, 21)
        Me.txtCalib1Li.TabIndex = 1
        Me.txtCalib1Li.Text = "22.30"
        '
        'lblCalib1Li
        '
        Me.lblCalib1Li.AutoSize = True
        Me.lblCalib1Li.BackColor = System.Drawing.Color.Transparent
        Me.lblCalib1Li.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblCalib1Li.ForeColor = System.Drawing.Color.Black
        Me.lblCalib1Li.Location = New System.Drawing.Point(158, 20)
        Me.lblCalib1Li.Name = "lblCalib1Li"
        Me.lblCalib1Li.Size = New System.Drawing.Size(21, 13)
        Me.lblCalib1Li.TabIndex = 1
        Me.lblCalib1Li.Text = "Li:"
        Me.lblCalib1Li.Title = False
        '
        'chkCalib1
        '
        Me.chkCalib1.AutoSize = True
        Me.chkCalib1.Checked = True
        Me.chkCalib1.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCalib1.Location = New System.Drawing.Point(6, 19)
        Me.chkCalib1.Name = "chkCalib1"
        Me.chkCalib1.Size = New System.Drawing.Size(146, 17)
        Me.chkCalib1.TabIndex = 0
        Me.chkCalib1.Text = "Include Calibration Result"
        Me.chkCalib1.UseVisualStyleBackColor = True
        '
        'butGenerateProcess
        '
        Me.butGenerateProcess.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.butGenerateProcess.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butGenerateProcess.Location = New System.Drawing.Point(414, 156)
        Me.butGenerateProcess.Name = "butGenerateProcess"
        Me.butGenerateProcess.Size = New System.Drawing.Size(199, 35)
        Me.butGenerateProcess.TabIndex = 1
        Me.butGenerateProcess.Text = "Generate Received message"
        Me.butGenerateProcess.UseVisualStyleBackColor = True
        '
        'grpPumps
        '
        Me.grpPumps.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.grpPumps.Controls.Add(Me.txtPumpsW)
        Me.grpPumps.Controls.Add(Me.lblPumpsW)
        Me.grpPumps.Controls.Add(Me.txtPumpsB)
        Me.grpPumps.Controls.Add(Me.lblPumpsB)
        Me.grpPumps.Controls.Add(Me.txtPumpsA)
        Me.grpPumps.Controls.Add(Me.lblPumpsA)
        Me.grpPumps.Controls.Add(Me.chkPumps)
        Me.grpPumps.ForeColor = System.Drawing.Color.Black
        Me.grpPumps.Location = New System.Drawing.Point(303, 19)
        Me.grpPumps.Name = "grpPumps"
        Me.grpPumps.Size = New System.Drawing.Size(421, 72)
        Me.grpPumps.TabIndex = 52
        Me.grpPumps.TabStop = False
        Me.grpPumps.Text = "Pumps"
        Me.grpPumps.Visible = False
        '
        'txtPumpsW
        '
        Me.txtPumpsW.BackColor = System.Drawing.Color.White
        Me.txtPumpsW.DecimalsValues = False
        Me.txtPumpsW.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtPumpsW.IsNumeric = False
        Me.txtPumpsW.Location = New System.Drawing.Point(364, 19)
        Me.txtPumpsW.Mandatory = False
        Me.txtPumpsW.Name = "txtPumpsW"
        Me.txtPumpsW.Size = New System.Drawing.Size(47, 21)
        Me.txtPumpsW.TabIndex = 2
        Me.txtPumpsW.Text = "2333"
        '
        'lblPumpsW
        '
        Me.lblPumpsW.AutoSize = True
        Me.lblPumpsW.BackColor = System.Drawing.Color.Transparent
        Me.lblPumpsW.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblPumpsW.ForeColor = System.Drawing.Color.Black
        Me.lblPumpsW.Location = New System.Drawing.Point(337, 22)
        Me.lblPumpsW.Name = "lblPumpsW"
        Me.lblPumpsW.Size = New System.Drawing.Size(23, 13)
        Me.lblPumpsW.TabIndex = 1
        Me.lblPumpsW.Text = "W:"
        Me.lblPumpsW.Title = False
        '
        'txtPumpsB
        '
        Me.txtPumpsB.BackColor = System.Drawing.Color.White
        Me.txtPumpsB.DecimalsValues = False
        Me.txtPumpsB.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtPumpsB.IsNumeric = False
        Me.txtPumpsB.Location = New System.Drawing.Point(277, 19)
        Me.txtPumpsB.Mandatory = False
        Me.txtPumpsB.Name = "txtPumpsB"
        Me.txtPumpsB.Size = New System.Drawing.Size(47, 21)
        Me.txtPumpsB.TabIndex = 2
        Me.txtPumpsB.Text = "2222"
        '
        'lblPumpsB
        '
        Me.lblPumpsB.AutoSize = True
        Me.lblPumpsB.BackColor = System.Drawing.Color.Transparent
        Me.lblPumpsB.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblPumpsB.ForeColor = System.Drawing.Color.Black
        Me.lblPumpsB.Location = New System.Drawing.Point(250, 22)
        Me.lblPumpsB.Name = "lblPumpsB"
        Me.lblPumpsB.Size = New System.Drawing.Size(20, 13)
        Me.lblPumpsB.TabIndex = 1
        Me.lblPumpsB.Text = "B:"
        Me.lblPumpsB.Title = False
        '
        'txtPumpsA
        '
        Me.txtPumpsA.BackColor = System.Drawing.Color.White
        Me.txtPumpsA.DecimalsValues = False
        Me.txtPumpsA.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtPumpsA.IsNumeric = False
        Me.txtPumpsA.Location = New System.Drawing.Point(189, 19)
        Me.txtPumpsA.Mandatory = False
        Me.txtPumpsA.Name = "txtPumpsA"
        Me.txtPumpsA.Size = New System.Drawing.Size(47, 21)
        Me.txtPumpsA.TabIndex = 2
        Me.txtPumpsA.Text = "2111"
        '
        'lblPumpsA
        '
        Me.lblPumpsA.AutoSize = True
        Me.lblPumpsA.BackColor = System.Drawing.Color.Transparent
        Me.lblPumpsA.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblPumpsA.ForeColor = System.Drawing.Color.Black
        Me.lblPumpsA.Location = New System.Drawing.Point(162, 22)
        Me.lblPumpsA.Name = "lblPumpsA"
        Me.lblPumpsA.Size = New System.Drawing.Size(20, 13)
        Me.lblPumpsA.TabIndex = 1
        Me.lblPumpsA.Text = "A:"
        Me.lblPumpsA.Title = False
        '
        'chkPumps
        '
        Me.chkPumps.AutoSize = True
        Me.chkPumps.Checked = True
        Me.chkPumps.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkPumps.Location = New System.Drawing.Point(6, 23)
        Me.chkPumps.Name = "chkPumps"
        Me.chkPumps.Size = New System.Drawing.Size(146, 17)
        Me.chkPumps.TabIndex = 0
        Me.chkPumps.Text = "Include Calibration Result"
        Me.chkPumps.UseVisualStyleBackColor = True
        '
        'grpERC
        '
        Me.grpERC.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.grpERC.Controls.Add(Me.chkERC)
        Me.grpERC.Controls.Add(Me.lblERCCode)
        Me.grpERC.Controls.Add(Me.cmbERCCode)
        Me.grpERC.Controls.Add(Me.txtERCRes)
        Me.grpERC.Controls.Add(Me.lblERCRes)
        Me.grpERC.ForeColor = System.Drawing.Color.Black
        Me.grpERC.Location = New System.Drawing.Point(199, 98)
        Me.grpERC.Name = "grpERC"
        Me.grpERC.Size = New System.Drawing.Size(644, 52)
        Me.grpERC.TabIndex = 0
        Me.grpERC.TabStop = False
        Me.grpERC.Text = "ERC"
        '
        'chkERC
        '
        Me.chkERC.AutoSize = True
        Me.chkERC.Location = New System.Drawing.Point(26, 18)
        Me.chkERC.Name = "chkERC"
        Me.chkERC.Size = New System.Drawing.Size(132, 17)
        Me.chkERC.TabIndex = 0
        Me.chkERC.Text = "Include ERC Message"
        Me.chkERC.UseVisualStyleBackColor = True
        '
        'lblERCCode
        '
        Me.lblERCCode.AutoSize = True
        Me.lblERCCode.BackColor = System.Drawing.Color.Transparent
        Me.lblERCCode.Enabled = False
        Me.lblERCCode.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblERCCode.ForeColor = System.Drawing.Color.Black
        Me.lblERCCode.Location = New System.Drawing.Point(186, 19)
        Me.lblERCCode.Name = "lblERCCode"
        Me.lblERCCode.Size = New System.Drawing.Size(42, 13)
        Me.lblERCCode.TabIndex = 1
        Me.lblERCCode.Text = "Code:"
        Me.lblERCCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblERCCode.Title = False
        '
        'cmbERCCode
        '
        Me.cmbERCCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbERCCode.Enabled = False
        Me.cmbERCCode.FormattingEnabled = True
        Me.cmbERCCode.Items.AddRange(New Object() {"0 - No Error", "S - Air in  Sample/Urine", "A - Air in Calibrant A", "B - Air in Calibrant B", "C - Air in Cleaner", "M - Air in Segment", "P - Pump Cal", "F - No Flow", "D - Bubble Detector", "R - Dallas Read", "W - Dallas Write", "T - Invalid Command", "N - No detecta xip quan vol llegir o escriure"})
        Me.cmbERCCode.Location = New System.Drawing.Point(236, 16)
        Me.cmbERCCode.Name = "cmbERCCode"
        Me.cmbERCCode.Size = New System.Drawing.Size(158, 21)
        Me.cmbERCCode.TabIndex = 2
        '
        'txtERCRes
        '
        Me.txtERCRes.BackColor = System.Drawing.Color.White
        Me.txtERCRes.Enabled = False
        Me.txtERCRes.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtERCRes.Location = New System.Drawing.Point(566, 16)
        Me.txtERCRes.Mask = "AAAAAA"
        Me.txtERCRes.Name = "txtERCRes"
        Me.txtERCRes.Size = New System.Drawing.Size(66, 21)
        Me.txtERCRes.TabIndex = 4
        Me.txtERCRes.Text = "000000"
        '
        'lblERCRes
        '
        Me.lblERCRes.AutoSize = True
        Me.lblERCRes.BackColor = System.Drawing.Color.Transparent
        Me.lblERCRes.Enabled = False
        Me.lblERCRes.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblERCRes.ForeColor = System.Drawing.Color.Black
        Me.lblERCRes.Location = New System.Drawing.Point(424, 19)
        Me.lblERCRes.Name = "lblERCRes"
        Me.lblERCRes.Size = New System.Drawing.Size(136, 13)
        Me.lblERCRes.TabIndex = 3
        Me.lblERCRes.Text = "Alarm codes (6 hexa):"
        Me.lblERCRes.Title = False
        '
        'grpBubbles
        '
        Me.grpBubbles.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.grpBubbles.Controls.Add(Me.txtBubblesL)
        Me.grpBubbles.Controls.Add(Me.lblBubblesL)
        Me.grpBubbles.Controls.Add(Me.txtBubblesM)
        Me.grpBubbles.Controls.Add(Me.lblBubblesM)
        Me.grpBubbles.Controls.Add(Me.txtBubblesA)
        Me.grpBubbles.Controls.Add(Me.lblBubblesA)
        Me.grpBubbles.Controls.Add(Me.chkBubbles)
        Me.grpBubbles.ForeColor = System.Drawing.Color.Black
        Me.grpBubbles.Location = New System.Drawing.Point(304, 19)
        Me.grpBubbles.Name = "grpBubbles"
        Me.grpBubbles.Size = New System.Drawing.Size(419, 72)
        Me.grpBubbles.TabIndex = 52
        Me.grpBubbles.TabStop = False
        Me.grpBubbles.Text = "Bubbles"
        Me.grpBubbles.Visible = False
        '
        'txtBubblesL
        '
        Me.txtBubblesL.BackColor = System.Drawing.Color.White
        Me.txtBubblesL.DecimalsValues = False
        Me.txtBubblesL.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtBubblesL.IsNumeric = False
        Me.txtBubblesL.Location = New System.Drawing.Point(363, 19)
        Me.txtBubblesL.Mandatory = False
        Me.txtBubblesL.Name = "txtBubblesL"
        Me.txtBubblesL.Size = New System.Drawing.Size(47, 21)
        Me.txtBubblesL.TabIndex = 2
        Me.txtBubblesL.Text = "99"
        '
        'lblBubblesL
        '
        Me.lblBubblesL.AutoSize = True
        Me.lblBubblesL.BackColor = System.Drawing.Color.Transparent
        Me.lblBubblesL.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblBubblesL.ForeColor = System.Drawing.Color.Black
        Me.lblBubblesL.Location = New System.Drawing.Point(336, 22)
        Me.lblBubblesL.Name = "lblBubblesL"
        Me.lblBubblesL.Size = New System.Drawing.Size(18, 13)
        Me.lblBubblesL.TabIndex = 1
        Me.lblBubblesL.Text = "L:"
        Me.lblBubblesL.Title = False
        '
        'txtBubblesM
        '
        Me.txtBubblesM.BackColor = System.Drawing.Color.White
        Me.txtBubblesM.DecimalsValues = False
        Me.txtBubblesM.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtBubblesM.IsNumeric = False
        Me.txtBubblesM.Location = New System.Drawing.Point(276, 19)
        Me.txtBubblesM.Mandatory = False
        Me.txtBubblesM.Name = "txtBubblesM"
        Me.txtBubblesM.Size = New System.Drawing.Size(47, 21)
        Me.txtBubblesM.TabIndex = 2
        Me.txtBubblesM.Text = "109"
        '
        'lblBubblesM
        '
        Me.lblBubblesM.AutoSize = True
        Me.lblBubblesM.BackColor = System.Drawing.Color.Transparent
        Me.lblBubblesM.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblBubblesM.ForeColor = System.Drawing.Color.Black
        Me.lblBubblesM.Location = New System.Drawing.Point(249, 22)
        Me.lblBubblesM.Name = "lblBubblesM"
        Me.lblBubblesM.Size = New System.Drawing.Size(21, 13)
        Me.lblBubblesM.TabIndex = 1
        Me.lblBubblesM.Text = "M:"
        Me.lblBubblesM.Title = False
        '
        'txtBubblesA
        '
        Me.txtBubblesA.BackColor = System.Drawing.Color.White
        Me.txtBubblesA.DecimalsValues = False
        Me.txtBubblesA.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.txtBubblesA.IsNumeric = False
        Me.txtBubblesA.Location = New System.Drawing.Point(188, 19)
        Me.txtBubblesA.Mandatory = False
        Me.txtBubblesA.Name = "txtBubblesA"
        Me.txtBubblesA.Size = New System.Drawing.Size(47, 21)
        Me.txtBubblesA.TabIndex = 2
        Me.txtBubblesA.Text = "199"
        '
        'lblBubblesA
        '
        Me.lblBubblesA.AutoSize = True
        Me.lblBubblesA.BackColor = System.Drawing.Color.Transparent
        Me.lblBubblesA.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblBubblesA.ForeColor = System.Drawing.Color.Black
        Me.lblBubblesA.Location = New System.Drawing.Point(161, 22)
        Me.lblBubblesA.Name = "lblBubblesA"
        Me.lblBubblesA.Size = New System.Drawing.Size(20, 13)
        Me.lblBubblesA.TabIndex = 1
        Me.lblBubblesA.Text = "A:"
        Me.lblBubblesA.Title = False
        '
        'chkBubbles
        '
        Me.chkBubbles.AutoSize = True
        Me.chkBubbles.Checked = True
        Me.chkBubbles.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkBubbles.Location = New System.Drawing.Point(9, 21)
        Me.chkBubbles.Name = "chkBubbles"
        Me.chkBubbles.Size = New System.Drawing.Size(146, 17)
        Me.chkBubbles.TabIndex = 0
        Me.chkBubbles.Text = "Include Calibration Result"
        Me.chkBubbles.UseVisualStyleBackColor = True
        '
        'lblISEAction
        '
        Me.lblISEAction.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lblISEAction.AutoSize = True
        Me.lblISEAction.BackColor = System.Drawing.Color.Transparent
        Me.lblISEAction.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblISEAction.ForeColor = System.Drawing.Color.Black
        Me.lblISEAction.Location = New System.Drawing.Point(378, 22)
        Me.lblISEAction.Name = "lblISEAction"
        Me.lblISEAction.Size = New System.Drawing.Size(98, 13)
        Me.lblISEAction.TabIndex = 40
        Me.lblISEAction.Text = "ISE Calibration:"
        Me.lblISEAction.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblISEAction.Title = False
        '
        'SendReceiveTab
        '
        Me.SendReceiveTab.Controls.Add(Me.grpProcessed)
        Me.SendReceiveTab.Controls.Add(Me.grpReceivedMessage)
        Me.SendReceiveTab.Location = New System.Drawing.Point(4, 22)
        Me.SendReceiveTab.Name = "SendReceiveTab"
        Me.SendReceiveTab.Padding = New System.Windows.Forms.Padding(3)
        Me.SendReceiveTab.Size = New System.Drawing.Size(1044, 277)
        Me.SendReceiveTab.TabIndex = 1
        Me.SendReceiveTab.Text = "Messages"
        Me.SendReceiveTab.UseVisualStyleBackColor = True
        '
        'grpProcessed
        '
        Me.grpProcessed.BackColor = System.Drawing.SystemColors.Control
        Me.grpProcessed.Controls.Add(Me.chkOnlyTreated)
        Me.grpProcessed.Controls.Add(Me.txtProcessedData)
        Me.grpProcessed.Controls.Add(Me.butClearProcessed)
        Me.grpProcessed.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpProcessed.ForeColor = System.Drawing.Color.Black
        Me.grpProcessed.Location = New System.Drawing.Point(3, 105)
        Me.grpProcessed.Name = "grpProcessed"
        Me.grpProcessed.Size = New System.Drawing.Size(1038, 169)
        Me.grpProcessed.TabIndex = 51
        Me.grpProcessed.TabStop = False
        Me.grpProcessed.Text = "Processed Data"
        '
        'chkOnlyTreated
        '
        Me.chkOnlyTreated.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkOnlyTreated.AutoSize = True
        Me.chkOnlyTreated.Location = New System.Drawing.Point(6, 137)
        Me.chkOnlyTreated.Name = "chkOnlyTreated"
        Me.chkOnlyTreated.Size = New System.Drawing.Size(137, 17)
        Me.chkOnlyTreated.TabIndex = 42
        Me.chkOnlyTreated.Text = "Only Treated messages"
        Me.chkOnlyTreated.UseVisualStyleBackColor = True
        '
        'txtProcessedData
        '
        Me.txtProcessedData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtProcessedData.BackColor = System.Drawing.Color.DimGray
        Me.txtProcessedData.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtProcessedData.ForeColor = System.Drawing.Color.GreenYellow
        Me.txtProcessedData.Location = New System.Drawing.Point(6, 19)
        Me.txtProcessedData.Multiline = True
        Me.txtProcessedData.Name = "txtProcessedData"
        Me.txtProcessedData.ReadOnly = True
        Me.txtProcessedData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtProcessedData.Size = New System.Drawing.Size(1026, 112)
        Me.txtProcessedData.TabIndex = 39
        '
        'butClearProcessed
        '
        Me.butClearProcessed.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butClearProcessed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butClearProcessed.Enabled = False
        Me.butClearProcessed.Location = New System.Drawing.Point(977, 137)
        Me.butClearProcessed.Name = "butClearProcessed"
        Me.butClearProcessed.Size = New System.Drawing.Size(55, 23)
        Me.butClearProcessed.TabIndex = 41
        Me.butClearProcessed.Text = "Clear"
        Me.butClearProcessed.UseVisualStyleBackColor = True
        '
        'grpReceivedMessage
        '
        Me.grpReceivedMessage.Controls.Add(Me.txtReceivedData)
        Me.grpReceivedMessage.Controls.Add(Me.butProcess)
        Me.grpReceivedMessage.Controls.Add(Me.butClearReceived)
        Me.grpReceivedMessage.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpReceivedMessage.ForeColor = System.Drawing.Color.Black
        Me.grpReceivedMessage.Location = New System.Drawing.Point(3, 3)
        Me.grpReceivedMessage.Name = "grpReceivedMessage"
        Me.grpReceivedMessage.Size = New System.Drawing.Size(1038, 102)
        Me.grpReceivedMessage.TabIndex = 0
        Me.grpReceivedMessage.TabStop = False
        Me.grpReceivedMessage.Text = "Received Message"
        '
        'txtReceivedData
        '
        Me.txtReceivedData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtReceivedData.Location = New System.Drawing.Point(6, 19)
        Me.txtReceivedData.Multiline = True
        Me.txtReceivedData.Name = "txtReceivedData"
        Me.txtReceivedData.Size = New System.Drawing.Size(1026, 45)
        Me.txtReceivedData.TabIndex = 42
        '
        'butProcess
        '
        Me.butProcess.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.butProcess.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butProcess.Enabled = False
        Me.butProcess.Location = New System.Drawing.Point(6, 70)
        Me.butProcess.Name = "butProcess"
        Me.butProcess.Size = New System.Drawing.Size(111, 23)
        Me.butProcess.TabIndex = 44
        Me.butProcess.Text = "Process Reception"
        Me.butProcess.UseVisualStyleBackColor = True
        '
        'butClearReceived
        '
        Me.butClearReceived.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butClearReceived.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butClearReceived.Enabled = False
        Me.butClearReceived.Location = New System.Drawing.Point(977, 70)
        Me.butClearReceived.Name = "butClearReceived"
        Me.butClearReceived.Size = New System.Drawing.Size(55, 23)
        Me.butClearReceived.TabIndex = 43
        Me.butClearReceived.Text = "Clear"
        Me.butClearReceived.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.butTestGrid)
        Me.TabPage2.Controls.Add(Me.butHistAlarms)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(1044, 277)
        Me.TabPage2.TabIndex = 2
        Me.TabPage2.Text = "Testing"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'butTestGrid
        '
        Me.butTestGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butTestGrid.Location = New System.Drawing.Point(8, 35)
        Me.butTestGrid.Name = "butTestGrid"
        Me.butTestGrid.Size = New System.Drawing.Size(137, 23)
        Me.butTestGrid.TabIndex = 45
        Me.butTestGrid.Text = "Test binding Data Grids"
        Me.butTestGrid.UseVisualStyleBackColor = True
        '
        'butHistAlarms
        '
        Me.butHistAlarms.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butHistAlarms.Location = New System.Drawing.Point(8, 6)
        Me.butHistAlarms.Name = "butHistAlarms"
        Me.butHistAlarms.Size = New System.Drawing.Size(137, 23)
        Me.butHistAlarms.TabIndex = 45
        Me.butHistAlarms.Text = "Alarm History (old form)"
        Me.butHistAlarms.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.comboProperties)
        Me.TabPage3.Controls.Add(Me.comboLabel)
        Me.TabPage3.Controls.Add(Me.chkCombo)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(1044, 277)
        Me.TabPage3.TabIndex = 3
        Me.TabPage3.Text = "Testing Combos"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'chkCombo
        '
        Me.chkCombo.Location = New System.Drawing.Point(8, 36)
        Me.chkCombo.Name = "chkCombo"
        Me.chkCombo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.chkCombo.Size = New System.Drawing.Size(160, 20)
        Me.chkCombo.TabIndex = 0
        '
        'comboLabel
        '
        Me.comboLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold)
        Me.comboLabel.Location = New System.Drawing.Point(8, 17)
        Me.comboLabel.Name = "comboLabel"
        Me.comboLabel.Size = New System.Drawing.Size(160, 16)
        Me.comboLabel.TabIndex = 1
        Me.comboLabel.Text = "Testing Combo:"
        Me.comboLabel.Title = False
        '
        'comboProperties
        '
        Me.comboProperties.Dock = System.Windows.Forms.DockStyle.Right
        Me.comboProperties.Location = New System.Drawing.Point(527, 3)
        Me.comboProperties.Name = "comboProperties"
        Me.comboProperties.SelectedObject = Me.chkCombo
        Me.comboProperties.Size = New System.Drawing.Size(514, 271)
        Me.comboProperties.TabIndex = 2
        '
        'Form_JBL
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1052, 303)
        Me.Controls.Add(Me.tabMain)
        Me.Name = "Form_JBL"
        Me.Text = "Form_JBL"
        Me.tabMain.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.grpISE.ResumeLayout(False)
        Me.grpISE.PerformLayout()
        Me.grpReceived.ResumeLayout(False)
        Me.grpCalib2.ResumeLayout(False)
        Me.grpCalib2.PerformLayout()
        Me.grpCalib1.ResumeLayout(False)
        Me.grpCalib1.PerformLayout()
        Me.grpPumps.ResumeLayout(False)
        Me.grpPumps.PerformLayout()
        Me.grpERC.ResumeLayout(False)
        Me.grpERC.PerformLayout()
        Me.grpBubbles.ResumeLayout(False)
        Me.grpBubbles.PerformLayout()
        Me.SendReceiveTab.ResumeLayout(False)
        Me.grpProcessed.ResumeLayout(False)
        Me.grpProcessed.PerformLayout()
        Me.grpReceivedMessage.ResumeLayout(False)
        Me.grpReceivedMessage.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        CType(Me.chkCombo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cmbISEAction As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents lblISEAction As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents grpReceived As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents grpISE As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents grpCalib1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents txtCalib1Cl As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblCalib1Cl As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtCalib1K As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents LblCalib1K As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtCalib1Na As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblCalib1Na As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtCalib1Li As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblCalib1Li As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents chkCalib1 As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents txtCalib1Res As Biosystems.Ax00.Controls.UserControls.BSMaskedTextBox
    Friend WithEvents lblCalib1Res As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents grpCalib2 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents txtCalib2Cl As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblCalib2Cl As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtCalib2K As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblCalib2K As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtCalib2Na As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblCalib2Na As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtCalib2Res As Biosystems.Ax00.Controls.UserControls.BSMaskedTextBox
    Friend WithEvents lblCalib2Res As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtCalib2Li As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblCalib2Li As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents chkCalib2 As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents grpERC As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents cmbERCCode As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents lblERCCode As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents butGenerateProcess As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents txtERCRes As Biosystems.Ax00.Controls.UserControls.BSMaskedTextBox
    Friend WithEvents lblERCRes As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents chkERC As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents grpPumps As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents txtPumpsW As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblPumpsW As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtPumpsB As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblPumpsB As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtPumpsA As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblPumpsA As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents grpBubbles As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents txtBubblesL As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblBubblesL As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtBubblesM As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblBubblesM As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents txtBubblesA As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents lblBubblesA As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents chkPumps As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents chkBubbles As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents tabMain As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents SendReceiveTab As System.Windows.Forms.TabPage
    Friend WithEvents grpProcessed As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents txtProcessedData As System.Windows.Forms.TextBox
    Friend WithEvents butClearProcessed As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents grpReceivedMessage As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents txtReceivedData As System.Windows.Forms.TextBox
    Friend WithEvents butProcess As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents butClearReceived As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents chkOnlyTreated As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents butHistAlarms As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents butTestGrid As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents comboProperties As System.Windows.Forms.PropertyGrid
    Friend WithEvents comboLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents chkCombo As DevExpress.XtraEditors.CheckedComboBoxEdit
End Class

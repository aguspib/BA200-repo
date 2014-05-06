<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class bsReception
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.BsGroupBox1 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsXmlButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsButton2 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsImport = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsErrorCodes = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsClear = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsSendNext = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsGroupBox2 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsInstructionComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.BsRestore = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsInstruction = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsClearReception = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsReceivedTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsLabel2 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsCommTestings = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.BsDataGridView1 = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.BsReceive = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsSendPrep = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.TextBox5 = New System.Windows.Forms.TextBox()
        Me.BsTextBoxToSend = New System.Windows.Forms.TextBox()
        Me.BsLockProcess = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsExecution = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsExecuteCalc = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsShortAction = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsDecodeEnBase2 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsNewGUID = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsHistoricCalibCurve = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsGroupBox1.SuspendLayout()
        Me.BsGroupBox2.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BsDataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.Controls.Add(Me.bsXmlButton)
        Me.BsGroupBox1.Controls.Add(Me.BsButton2)
        Me.BsGroupBox1.Controls.Add(Me.BsImport)
        Me.BsGroupBox1.Controls.Add(Me.bsErrorCodes)
        Me.BsGroupBox1.Controls.Add(Me.BsClear)
        Me.BsGroupBox1.Controls.Add(Me.BsSendNext)
        Me.BsGroupBox1.Controls.Add(Me.BsGroupBox2)
        Me.BsGroupBox1.Controls.Add(Me.DataGridView1)
        Me.BsGroupBox1.Controls.Add(Me.BsDataGridView1)
        Me.BsGroupBox1.Controls.Add(Me.BsReceive)
        Me.BsGroupBox1.Controls.Add(Me.BsLabel1)
        Me.BsGroupBox1.Controls.Add(Me.BsSendPrep)
        Me.BsGroupBox1.Controls.Add(Me.TextBox5)
        Me.BsGroupBox1.Controls.Add(Me.BsTextBoxToSend)
        Me.BsGroupBox1.Controls.Add(Me.BsLockProcess)
        Me.BsGroupBox1.Controls.Add(Me.BsExecution)
        Me.BsGroupBox1.Controls.Add(Me.BsExecuteCalc)
        Me.BsGroupBox1.Controls.Add(Me.BsShortAction)
        Me.BsGroupBox1.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox1.Location = New System.Drawing.Point(12, 11)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New System.Drawing.Size(1104, 545)
        Me.BsGroupBox1.TabIndex = 10
        Me.BsGroupBox1.TabStop = False
        Me.BsGroupBox1.Text = "AG Testings"
        '
        'bsXmlButton
        '
        Me.bsXmlButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsXmlButton.Location = New System.Drawing.Point(959, 50)
        Me.bsXmlButton.Name = "bsXmlButton"
        Me.bsXmlButton.Size = New System.Drawing.Size(130, 24)
        Me.bsXmlButton.TabIndex = 54
        Me.bsXmlButton.Text = "Tasks 11-03-2013"
        Me.bsXmlButton.UseVisualStyleBackColor = True
        '
        'BsButton2
        '
        Me.BsButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton2.Location = New System.Drawing.Point(521, 20)
        Me.BsButton2.Name = "BsButton2"
        Me.BsButton2.Size = New System.Drawing.Size(130, 23)
        Me.BsButton2.TabIndex = 53
        Me.BsButton2.Text = "Calculations NEW"
        Me.BsButton2.UseVisualStyleBackColor = True
        '
        'BsImport
        '
        Me.BsImport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsImport.Location = New System.Drawing.Point(959, 20)
        Me.BsImport.Name = "BsImport"
        Me.BsImport.Size = New System.Drawing.Size(130, 24)
        Me.BsImport.TabIndex = 52
        Me.BsImport.Text = "Import"
        Me.BsImport.UseVisualStyleBackColor = True
        '
        'bsErrorCodes
        '
        Me.bsErrorCodes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsErrorCodes.Location = New System.Drawing.Point(553, 148)
        Me.bsErrorCodes.Name = "bsErrorCodes"
        Me.bsErrorCodes.Size = New System.Drawing.Size(179, 24)
        Me.bsErrorCodes.TabIndex = 51
        Me.bsErrorCodes.Text = "Decode Alarm Error Codes"
        Me.bsErrorCodes.UseVisualStyleBackColor = True
        '
        'BsClear
        '
        Me.BsClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsClear.Location = New System.Drawing.Point(993, 149)
        Me.BsClear.Name = "BsClear"
        Me.BsClear.Size = New System.Drawing.Size(96, 21)
        Me.BsClear.TabIndex = 50
        Me.BsClear.Text = "Clear LOG"
        Me.BsClear.UseVisualStyleBackColor = True
        '
        'BsSendNext
        '
        Me.BsSendNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsSendNext.Location = New System.Drawing.Point(7, 49)
        Me.BsSendNext.Name = "BsSendNext"
        Me.BsSendNext.Size = New System.Drawing.Size(100, 24)
        Me.BsSendNext.TabIndex = 49
        Me.BsSendNext.Text = "Send Next"
        Me.BsSendNext.UseVisualStyleBackColor = True
        '
        'BsGroupBox2
        '
        Me.BsGroupBox2.Controls.Add(Me.BsInstructionComboBox)
        Me.BsGroupBox2.Controls.Add(Me.BsRestore)
        Me.BsGroupBox2.Controls.Add(Me.BsInstruction)
        Me.BsGroupBox2.Controls.Add(Me.BsClearReception)
        Me.BsGroupBox2.Controls.Add(Me.BsReceivedTextBox)
        Me.BsGroupBox2.Controls.Add(Me.BsLabel2)
        Me.BsGroupBox2.Controls.Add(Me.BsCommTestings)
        Me.BsGroupBox2.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox2.Location = New System.Drawing.Point(7, 107)
        Me.BsGroupBox2.Name = "BsGroupBox2"
        Me.BsGroupBox2.Size = New System.Drawing.Size(404, 424)
        Me.BsGroupBox2.TabIndex = 45
        Me.BsGroupBox2.TabStop = False
        Me.BsGroupBox2.Text = "Communications Testings"
        '
        'BsInstructionComboBox
        '
        Me.BsInstructionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.BsInstructionComboBox.FormattingEnabled = True
        Me.BsInstructionComboBox.Items.AddRange(New Object() {"CONNECT", "STATE", "STANDBY", "SLEEP", "RUNNING", "START", "TEST", "ENDRUN", "INFO ON", "INFO OFF", "CONFIG", "POLLRD"})
        Me.BsInstructionComboBox.Location = New System.Drawing.Point(161, 18)
        Me.BsInstructionComboBox.Name = "BsInstructionComboBox"
        Me.BsInstructionComboBox.Size = New System.Drawing.Size(125, 21)
        Me.BsInstructionComboBox.TabIndex = 47
        '
        'BsRestore
        '
        Me.BsRestore.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsRestore.Location = New System.Drawing.Point(300, 16)
        Me.BsRestore.Name = "BsRestore"
        Me.BsRestore.Size = New System.Drawing.Size(96, 21)
        Me.BsRestore.TabIndex = 46
        Me.BsRestore.Text = "Restore Comms"
        Me.BsRestore.UseVisualStyleBackColor = True
        '
        'BsInstruction
        '
        Me.BsInstruction.BackColor = System.Drawing.Color.White
        Me.BsInstruction.DecimalsValues = False
        Me.BsInstruction.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsInstruction.IsNumeric = False
        Me.BsInstruction.Location = New System.Drawing.Point(161, 41)
        Me.BsInstruction.Mandatory = False
        Me.BsInstruction.Name = "BsInstruction"
        Me.BsInstruction.Size = New System.Drawing.Size(125, 21)
        Me.BsInstruction.TabIndex = 14
        Me.BsInstruction.Text = "CONNECT"
        Me.BsInstruction.Visible = False
        '
        'BsClearReception
        '
        Me.BsClearReception.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsClearReception.Location = New System.Drawing.Point(300, 40)
        Me.BsClearReception.Name = "BsClearReception"
        Me.BsClearReception.Size = New System.Drawing.Size(96, 21)
        Me.BsClearReception.TabIndex = 13
        Me.BsClearReception.Text = "Clear LOG"
        Me.BsClearReception.UseVisualStyleBackColor = True
        '
        'BsReceivedTextBox
        '
        Me.BsReceivedTextBox.BackColor = System.Drawing.Color.White
        Me.BsReceivedTextBox.DecimalsValues = False
        Me.BsReceivedTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsReceivedTextBox.IsNumeric = False
        Me.BsReceivedTextBox.Location = New System.Drawing.Point(10, 64)
        Me.BsReceivedTextBox.Mandatory = False
        Me.BsReceivedTextBox.Multiline = True
        Me.BsReceivedTextBox.Name = "BsReceivedTextBox"
        Me.BsReceivedTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.BsReceivedTextBox.Size = New System.Drawing.Size(386, 354)
        Me.BsReceivedTextBox.TabIndex = 12
        '
        'BsLabel2
        '
        Me.BsLabel2.AutoSize = True
        Me.BsLabel2.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel2.ForeColor = System.Drawing.Color.Black
        Me.BsLabel2.Location = New System.Drawing.Point(6, 48)
        Me.BsLabel2.Name = "BsLabel2"
        Me.BsLabel2.Size = New System.Drawing.Size(106, 13)
        Me.BsLabel2.TabIndex = 1
        Me.BsLabel2.Text = "Instruction's Flow"
        Me.BsLabel2.Title = False
        '
        'BsCommTestings
        '
        Me.BsCommTestings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsCommTestings.Location = New System.Drawing.Point(10, 16)
        Me.BsCommTestings.Name = "BsCommTestings"
        Me.BsCommTestings.Size = New System.Drawing.Size(134, 21)
        Me.BsCommTestings.TabIndex = 0
        Me.BsCommTestings.Text = "Send instruction"
        Me.BsCommTestings.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(785, 376)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(304, 149)
        Me.DataGridView1.TabIndex = 44
        '
        'BsDataGridView1
        '
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.BsDataGridView1.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.BsDataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BsDataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.BsDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BsDataGridView1.DefaultCellStyle = DataGridViewCellStyle3
        Me.BsDataGridView1.EnterToTab = False
        Me.BsDataGridView1.GridColor = System.Drawing.Color.Silver
        Me.BsDataGridView1.Location = New System.Drawing.Point(428, 376)
        Me.BsDataGridView1.Name = "BsDataGridView1"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BsDataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.BsDataGridView1.Size = New System.Drawing.Size(318, 149)
        Me.BsDataGridView1.TabIndex = 43
        Me.BsDataGridView1.TabToEnter = False
        '
        'BsReceive
        '
        Me.BsReceive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsReceive.Location = New System.Drawing.Point(428, 146)
        Me.BsReceive.Name = "BsReceive"
        Me.BsReceive.Size = New System.Drawing.Size(119, 24)
        Me.BsReceive.TabIndex = 42
        Me.BsReceive.Text = "Process Reception"
        Me.BsReceive.UseVisualStyleBackColor = True
        '
        'BsLabel1
        '
        Me.BsLabel1.AutoSize = True
        Me.BsLabel1.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel1.ForeColor = System.Drawing.Color.Black
        Me.BsLabel1.Location = New System.Drawing.Point(535, 99)
        Me.BsLabel1.Name = "BsLabel1"
        Me.BsLabel1.Size = New System.Drawing.Size(111, 13)
        Me.BsLabel1.TabIndex = 41
        Me.BsLabel1.Text = "Execution Number"
        Me.BsLabel1.Title = False
        '
        'BsSendPrep
        '
        Me.BsSendPrep.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsSendPrep.Location = New System.Drawing.Point(428, 115)
        Me.BsSendPrep.Name = "BsSendPrep"
        Me.BsSendPrep.Size = New System.Drawing.Size(101, 25)
        Me.BsSendPrep.TabIndex = 40
        Me.BsSendPrep.Text = "Preparation"
        Me.BsSendPrep.UseVisualStyleBackColor = True
        '
        'TextBox5
        '
        Me.TextBox5.Location = New System.Drawing.Point(428, 176)
        Me.TextBox5.Multiline = True
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(661, 194)
        Me.TextBox5.TabIndex = 38
        '
        'BsTextBoxToSend
        '
        Me.BsTextBoxToSend.Location = New System.Drawing.Point(538, 115)
        Me.BsTextBoxToSend.Name = "BsTextBoxToSend"
        Me.BsTextBoxToSend.Size = New System.Drawing.Size(122, 21)
        Me.BsTextBoxToSend.TabIndex = 37
        '
        'BsLockProcess
        '
        Me.BsLockProcess.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsLockProcess.Location = New System.Drawing.Point(7, 78)
        Me.BsLockProcess.Name = "BsLockProcess"
        Me.BsLockProcess.Size = New System.Drawing.Size(100, 23)
        Me.BsLockProcess.TabIndex = 16
        Me.BsLockProcess.Text = "Lock process"
        Me.BsLockProcess.UseVisualStyleBackColor = True
        '
        'BsExecution
        '
        Me.BsExecution.BackColor = System.Drawing.Color.White
        Me.BsExecution.DecimalsValues = False
        Me.BsExecution.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsExecution.IsNumeric = False
        Me.BsExecution.Location = New System.Drawing.Point(275, 19)
        Me.BsExecution.Mandatory = False
        Me.BsExecution.Multiline = True
        Me.BsExecution.Name = "BsExecution"
        Me.BsExecution.Size = New System.Drawing.Size(100, 23)
        Me.BsExecution.TabIndex = 15
        '
        'BsExecuteCalc
        '
        Me.BsExecuteCalc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsExecuteCalc.Location = New System.Drawing.Point(385, 19)
        Me.BsExecuteCalc.Name = "BsExecuteCalc"
        Me.BsExecuteCalc.Size = New System.Drawing.Size(130, 23)
        Me.BsExecuteCalc.TabIndex = 14
        Me.BsExecuteCalc.Text = "Calculations"
        Me.BsExecuteCalc.UseVisualStyleBackColor = True
        '
        'BsShortAction
        '
        Me.BsShortAction.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsShortAction.Location = New System.Drawing.Point(6, 20)
        Me.BsShortAction.Name = "BsShortAction"
        Me.BsShortAction.Size = New System.Drawing.Size(129, 23)
        Me.BsShortAction.TabIndex = 12
        Me.BsShortAction.Text = "Positions InProcess"
        Me.BsShortAction.UseVisualStyleBackColor = True
        '
        'bsDecodeEnBase2
        '
        Me.bsDecodeEnBase2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsDecodeEnBase2.Location = New System.Drawing.Point(12, 562)
        Me.bsDecodeEnBase2.Name = "bsDecodeEnBase2"
        Me.bsDecodeEnBase2.Size = New System.Drawing.Size(151, 23)
        Me.bsDecodeEnBase2.TabIndex = 54
        Me.bsDecodeEnBase2.Text = "Decodificar en base2"
        Me.bsDecodeEnBase2.UseVisualStyleBackColor = True
        '
        'bsNewGUID
        '
        Me.bsNewGUID.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsNewGUID.Location = New System.Drawing.Point(12, 591)
        Me.bsNewGUID.Name = "bsNewGUID"
        Me.bsNewGUID.Size = New System.Drawing.Size(151, 23)
        Me.bsNewGUID.TabIndex = 55
        Me.bsNewGUID.Text = "Generate GUID"
        Me.bsNewGUID.UseVisualStyleBackColor = True
        '
        'BsHistoricCalibCurve
        '
        Me.BsHistoricCalibCurve.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsHistoricCalibCurve.Location = New System.Drawing.Point(169, 562)
        Me.BsHistoricCalibCurve.Name = "BsHistoricCalibCurve"
        Me.BsHistoricCalibCurve.Size = New System.Drawing.Size(118, 23)
        Me.BsHistoricCalibCurve.TabIndex = 56
        Me.BsHistoricCalibCurve.Text = "Hist CalibCurve"
        Me.BsHistoricCalibCurve.UseVisualStyleBackColor = True
        '
        'bsReception
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1136, 736)
        Me.Controls.Add(Me.BsHistoricCalibCurve)
        Me.Controls.Add(Me.bsNewGUID)
        Me.Controls.Add(Me.bsDecodeEnBase2)
        Me.Controls.Add(Me.BsGroupBox1)
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "bsReception"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "Form3"
        Me.BsGroupBox1.ResumeLayout(False)
        Me.BsGroupBox1.PerformLayout()
        Me.BsGroupBox2.ResumeLayout(False)
        Me.BsGroupBox2.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BsDataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsGroupBox1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsShortAction As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsExecution As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsExecuteCalc As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsLockProcess As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsReceive As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsSendPrep As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents TextBox5 As System.Windows.Forms.TextBox
    Friend WithEvents BsTextBoxToSend As System.Windows.Forms.TextBox
    Friend WithEvents BsDataGridView1 As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents BsGroupBox2 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsCommTestings As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsClearReception As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsReceivedTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsLabel2 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsInstruction As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsSendNext As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsInstructionComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsClear As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsErrorCodes As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsImport As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsButton2 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsRestore As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDecodeEnBase2 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsNewGUID As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsHistoricCalibCurve As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsXmlButton As Biosystems.Ax00.Controls.UserControls.BSButton
End Class

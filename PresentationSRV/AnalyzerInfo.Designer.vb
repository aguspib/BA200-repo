<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AnalyzerInfo
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


    'Const WM_WINDOWPOSCHANGING As Int32 = &H46

    'Not allow move form
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_WINDOWPOSCHANGING Then
            Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, _
                                                                                             GetType(WINDOWPOS)),  _
                                                                                             WINDOWPOS)
            If Not Me.EnableMoveWindow Then
                pos.x = Me.Left
                pos.y = Me.Top
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

        End If

        MyBase.WndProc(m)

    End Sub
    'End XBC 27/07/2011

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AnalyzerInfo))
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsRichTextBox1 = New Biosystems.Ax00.Controls.UserControls.BSRichTextBox
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsInfoDetailsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.MoreInfoButton = New System.Windows.Forms.Button
        Me.BsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bwPreload = New System.ComponentModel.BackgroundWorker
        Me.TestProcessTimer = New Biosystems.Ax00.Controls.UserControls.BSTimer
        Me.PrintDialog1 = New System.Windows.Forms.PrintDialog
        Me.PrintDocument1 = New System.Drawing.Printing.PrintDocument
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar
        Me.bsLanguageSelectionGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsFwVersionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsModelTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsSerialTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsSerialLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsEditSNButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsSaveSNButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsCancelSNButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.DetailsButton = New System.Windows.Forms.Button
        Me.BsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.bsFwVersionLabel2 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsInfoDetailsGroupBox.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsMessagesPanel.SuspendLayout()
        Me.bsLanguageSelectionGroupBox.SuspendLayout()
        CType(Me.BsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsExitButton
        '
        Me.BsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsExitButton.Location = New System.Drawing.Point(493, 430)
        Me.BsExitButton.Name = "BsExitButton"
        Me.BsExitButton.Size = New System.Drawing.Size(30, 30)
        Me.BsExitButton.TabIndex = 4
        Me.BsExitButton.UseVisualStyleBackColor = True
        '
        'BsRichTextBox1
        '
        Me.BsRichTextBox1.BackColor = System.Drawing.SystemColors.Info
        Me.BsRichTextBox1.Location = New System.Drawing.Point(6, 19)
        Me.BsRichTextBox1.Name = "BsRichTextBox1"
        Me.BsRichTextBox1.ReadOnly = True
        Me.BsRichTextBox1.Size = New System.Drawing.Size(499, 166)
        Me.BsRichTextBox1.TabIndex = 5
        Me.BsRichTextBox1.Text = resources.GetString("BsRichTextBox1.Text")
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsPrintButton.ForeColor = System.Drawing.Color.Black
        Me.bsPrintButton.Location = New System.Drawing.Point(12, 430)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 26
        Me.bsPrintButton.UseVisualStyleBackColor = True
        '
        'bsInfoDetailsGroupBox
        '
        Me.bsInfoDetailsGroupBox.Controls.Add(Me.MoreInfoButton)
        Me.bsInfoDetailsGroupBox.Controls.Add(Me.BsRichTextBox1)
        Me.bsInfoDetailsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsInfoDetailsGroupBox.Location = New System.Drawing.Point(12, 204)
        Me.bsInfoDetailsGroupBox.Name = "bsInfoDetailsGroupBox"
        Me.bsInfoDetailsGroupBox.Size = New System.Drawing.Size(511, 220)
        Me.bsInfoDetailsGroupBox.TabIndex = 40
        Me.bsInfoDetailsGroupBox.TabStop = False
        Me.bsInfoDetailsGroupBox.Text = "Information of the Analyzer"
        '
        'MoreInfoButton
        '
        Me.MoreInfoButton.Location = New System.Drawing.Point(9, 191)
        Me.MoreInfoButton.Name = "MoreInfoButton"
        Me.MoreInfoButton.Size = New System.Drawing.Size(108, 23)
        Me.MoreInfoButton.TabIndex = 67
        Me.MoreInfoButton.Text = "+ Request Info"
        Me.MoreInfoButton.UseVisualStyleBackColor = True
        Me.MoreInfoButton.Visible = False
        '
        'BsTitleLabel
        '
        Me.BsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTitleLabel.Location = New System.Drawing.Point(6, 17)
        Me.BsTitleLabel.Name = "BsTitleLabel"
        Me.BsTitleLabel.Size = New System.Drawing.Size(491, 20)
        Me.BsTitleLabel.TabIndex = 60
        Me.BsTitleLabel.Text = "Analyzer Info"
        Me.BsTitleLabel.Title = True
        '
        'BsMessageImage
        '
        Me.BsMessageImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsMessageImage.Location = New System.Drawing.Point(2, 1)
        Me.BsMessageImage.Name = "BsMessageImage"
        Me.BsMessageImage.PositionNumber = 0
        Me.BsMessageImage.Size = New System.Drawing.Size(32, 32)
        Me.BsMessageImage.TabIndex = 5
        Me.BsMessageImage.TabStop = False
        '
        'BsMessageLabel
        '
        Me.BsMessageLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsMessageLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsMessageLabel.ForeColor = System.Drawing.Color.Black
        Me.BsMessageLabel.Location = New System.Drawing.Point(37, 11)
        Me.BsMessageLabel.Name = "BsMessageLabel"
        Me.BsMessageLabel.Size = New System.Drawing.Size(338, 15)
        Me.BsMessageLabel.TabIndex = 4
        Me.BsMessageLabel.Title = False
        '
        'bwPreload
        '
        '
        'TestProcessTimer
        '
        '
        'PrintDialog1
        '
        Me.PrintDialog1.UseEXDialog = True
        '
        'PrintDocument1
        '
        '
        'BsMessagesPanel
        '
        Me.BsMessagesPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessagesPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsMessagesPanel.Controls.Add(Me.ProgressBar1)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageLabel)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageImage)
        Me.BsMessagesPanel.Location = New System.Drawing.Point(12, 107)
        Me.BsMessagesPanel.Name = "BsMessagesPanel"
        Me.BsMessagesPanel.Size = New System.Drawing.Size(485, 35)
        Me.BsMessagesPanel.TabIndex = 64
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(297, 9)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(180, 18)
        Me.ProgressBar1.TabIndex = 67
        Me.ProgressBar1.Visible = False
        '
        'bsLanguageSelectionGroupBox
        '
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsFwVersionLabel2)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsFwVersionLabel)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsModelTextBox)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.BsMessagesPanel)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsSerialTextBox)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsSerialLabel)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.BsTitleLabel)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsEditSNButton)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsSaveSNButton)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsCancelSNButton)
        Me.bsLanguageSelectionGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLanguageSelectionGroupBox.Location = New System.Drawing.Point(12, 4)
        Me.bsLanguageSelectionGroupBox.Name = "bsLanguageSelectionGroupBox"
        Me.bsLanguageSelectionGroupBox.Size = New System.Drawing.Size(511, 153)
        Me.bsLanguageSelectionGroupBox.TabIndex = 64
        Me.bsLanguageSelectionGroupBox.TabStop = False
        '
        'bsFwVersionLabel
        '
        Me.bsFwVersionLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsFwVersionLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFwVersionLabel.ForeColor = System.Drawing.Color.Black
        Me.bsFwVersionLabel.Location = New System.Drawing.Point(11, 85)
        Me.bsFwVersionLabel.Name = "bsFwVersionLabel"
        Me.bsFwVersionLabel.Size = New System.Drawing.Size(138, 19)
        Me.bsFwVersionLabel.TabIndex = 86
        Me.bsFwVersionLabel.Text = "Firmware Version:"
        Me.bsFwVersionLabel.Title = False
        '
        'bsModelTextBox
        '
        Me.bsModelTextBox.BackColor = System.Drawing.Color.White
        Me.bsModelTextBox.DecimalsValues = False
        Me.bsModelTextBox.Enabled = False
        Me.bsModelTextBox.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsModelTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsModelTextBox.IsNumeric = False
        Me.bsModelTextBox.Location = New System.Drawing.Point(112, 47)
        Me.bsModelTextBox.Mandatory = False
        Me.bsModelTextBox.MaxLength = 9
        Me.bsModelTextBox.Name = "bsModelTextBox"
        Me.bsModelTextBox.ReadOnly = True
        Me.bsModelTextBox.Size = New System.Drawing.Size(37, 27)
        Me.bsModelTextBox.TabIndex = 85
        Me.bsModelTextBox.WordWrap = False
        '
        'bsSerialTextBox
        '
        Me.bsSerialTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsSerialTextBox.DecimalsValues = False
        Me.bsSerialTextBox.Enabled = False
        Me.bsSerialTextBox.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSerialTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSerialTextBox.IsNumeric = False
        Me.bsSerialTextBox.Location = New System.Drawing.Point(155, 47)
        Me.bsSerialTextBox.Mandatory = True
        Me.bsSerialTextBox.MaxLength = 6
        Me.bsSerialTextBox.Name = "bsSerialTextBox"
        Me.bsSerialTextBox.Size = New System.Drawing.Size(203, 27)
        Me.bsSerialTextBox.TabIndex = 62
        Me.bsSerialTextBox.WordWrap = False
        '
        'bsSerialLabel
        '
        Me.bsSerialLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSerialLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSerialLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSerialLabel.Location = New System.Drawing.Point(9, 54)
        Me.bsSerialLabel.Name = "bsSerialLabel"
        Me.bsSerialLabel.Size = New System.Drawing.Size(112, 13)
        Me.bsSerialLabel.TabIndex = 61
        Me.bsSerialLabel.Text = "Serial number:"
        Me.bsSerialLabel.Title = False
        '
        'bsEditSNButton
        '
        Me.bsEditSNButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsEditSNButton.Location = New System.Drawing.Point(465, 44)
        Me.bsEditSNButton.Name = "bsEditSNButton"
        Me.bsEditSNButton.Size = New System.Drawing.Size(32, 32)
        Me.bsEditSNButton.TabIndex = 63
        Me.bsEditSNButton.UseVisualStyleBackColor = True
        '
        'bsSaveSNButton
        '
        Me.bsSaveSNButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSaveSNButton.Enabled = False
        Me.bsSaveSNButton.Location = New System.Drawing.Point(427, 44)
        Me.bsSaveSNButton.Name = "bsSaveSNButton"
        Me.bsSaveSNButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveSNButton.TabIndex = 64
        Me.bsSaveSNButton.UseVisualStyleBackColor = True
        Me.bsSaveSNButton.Visible = False
        '
        'bsCancelSNButton
        '
        Me.bsCancelSNButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelSNButton.Location = New System.Drawing.Point(465, 44)
        Me.bsCancelSNButton.Name = "bsCancelSNButton"
        Me.bsCancelSNButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelSNButton.TabIndex = 65
        Me.bsCancelSNButton.UseVisualStyleBackColor = True
        Me.bsCancelSNButton.Visible = False
        '
        'DetailsButton
        '
        Me.DetailsButton.Location = New System.Drawing.Point(21, 167)
        Me.DetailsButton.Name = "DetailsButton"
        Me.DetailsButton.Size = New System.Drawing.Size(102, 23)
        Me.DetailsButton.TabIndex = 66
        Me.DetailsButton.Text = "Show Details >"
        Me.DetailsButton.UseVisualStyleBackColor = True
        Me.DetailsButton.Visible = False
        '
        'BsErrorProvider1
        '
        Me.BsErrorProvider1.ContainerControl = Me
        '
        'bsFwVersionLabel2
        '
        Me.bsFwVersionLabel2.BackColor = System.Drawing.Color.Transparent
        Me.bsFwVersionLabel2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFwVersionLabel2.ForeColor = System.Drawing.Color.Black
        Me.bsFwVersionLabel2.Location = New System.Drawing.Point(155, 85)
        Me.bsFwVersionLabel2.Name = "bsFwVersionLabel2"
        Me.bsFwVersionLabel2.Size = New System.Drawing.Size(85, 19)
        Me.bsFwVersionLabel2.TabIndex = 87
        Me.bsFwVersionLabel2.Text = "v.0.0.0"
        Me.bsFwVersionLabel2.Title = False
        '
        'AnalyzerInfo
        '
        Me.AcceptButton = Me.BsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(535, 467)
        Me.ControlBox = False
        Me.Controls.Add(Me.DetailsButton)
        Me.Controls.Add(Me.bsPrintButton)
        Me.Controls.Add(Me.BsExitButton)
        Me.Controls.Add(Me.bsLanguageSelectionGroupBox)
        Me.Controls.Add(Me.bsInfoDetailsGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AnalyzerInfo"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = ""
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        Me.bsInfoDetailsGroupBox.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsMessagesPanel.ResumeLayout(False)
        Me.bsLanguageSelectionGroupBox.ResumeLayout(False)
        Me.bsLanguageSelectionGroupBox.PerformLayout()
        CType(Me.BsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsRichTextBox1 As Biosystems.Ax00.Controls.UserControls.BSRichTextBox
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsInfoDetailsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bwPreload As System.ComponentModel.BackgroundWorker
    Friend WithEvents TestProcessTimer As Biosystems.Ax00.Controls.UserControls.BSTimer
    Friend WithEvents PrintDialog1 As System.Windows.Forms.PrintDialog
    Friend WithEvents PrintDocument1 As System.Drawing.Printing.PrintDocument
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsLanguageSelectionGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsCancelSNButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSerialTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSerialLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsEditSNButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSaveSNButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents DetailsButton As System.Windows.Forms.Button
    Friend WithEvents MoreInfoButton As System.Windows.Forms.Button
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents BsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsModelTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsFwVersionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsFwVersionLabel2 As Biosystems.Ax00.Controls.UserControls.BSLabel
End Class

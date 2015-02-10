<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiInstrumentInfo
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiInstrumentInfo))
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.BsBorderedPanel1 = New bsBorderedPanel
        Me.OKButton = New System.Windows.Forms.Button
        Me.BsPanel2 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.GroupControl1 = New DevExpress.XtraEditors.GroupControl
        Me.BsPanel1 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.FirmwareLabel = New System.Windows.Forms.Label
        Me.SerialNumberLabel = New System.Windows.Forms.Label
        Me.LogoPictureBox = New System.Windows.Forms.PictureBox
        Me.BsGroupBox1 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.BsPanel2.SuspendLayout()
        CType(Me.GroupControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupControl1.SuspendLayout()
        Me.BsPanel1.SuspendLayout()
        CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsBorderedPanel1
        '
        Me.BsBorderedPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsBorderedPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsBorderedPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsBorderedPanel1.Name = "BsBorderedPanel1"
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(374, 176)
        Me.BsBorderedPanel1.TabIndex = 19
        '
        'OKButton
        '
        Me.OKButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.OKButton.Location = New System.Drawing.Point(332, 132)
        Me.OKButton.Name = "OKButton"
        Me.OKButton.Size = New System.Drawing.Size(32, 32)
        Me.OKButton.TabIndex = 20
        Me.OKButton.UseVisualStyleBackColor = True
        '
        'BsPanel2
        '
        Me.BsPanel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(223, Byte), Integer), CType(CType(237, Byte), Integer), CType(CType(251, Byte), Integer))
        Me.BsPanel2.Controls.Add(Me.GroupControl1)
        Me.BsPanel2.Controls.Add(Me.LogoPictureBox)
        Me.BsPanel2.Location = New System.Drawing.Point(2, 2)
        Me.BsPanel2.Name = "BsPanel2"
        Me.BsPanel2.Size = New System.Drawing.Size(370, 118)
        Me.BsPanel2.TabIndex = 22
        '
        'GroupControl1
        '
        Me.GroupControl1.Appearance.BackColor = System.Drawing.Color.White
        Me.GroupControl1.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupControl1.Appearance.Options.UseBackColor = True
        Me.GroupControl1.Appearance.Options.UseFont = True
        Me.GroupControl1.Controls.Add(Me.BsPanel1)
        Me.GroupControl1.Location = New System.Drawing.Point(50, 8)
        Me.GroupControl1.LookAndFeel.SkinName = "Money Twins"
        Me.GroupControl1.LookAndFeel.UseDefaultLookAndFeel = False
        Me.GroupControl1.Name = "GroupControl1"
        Me.GroupControl1.Size = New System.Drawing.Size(310, 102)
        Me.GroupControl1.TabIndex = 19
        '
        'BsPanel1
        '
        Me.BsPanel1.BackColor = System.Drawing.Color.White
        Me.BsPanel1.Controls.Add(Me.FirmwareLabel)
        Me.BsPanel1.Controls.Add(Me.SerialNumberLabel)
        Me.BsPanel1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsPanel1.Location = New System.Drawing.Point(1, 21)
        Me.BsPanel1.Name = "BsPanel1"
        Me.BsPanel1.Size = New System.Drawing.Size(308, 80)
        Me.BsPanel1.TabIndex = 12
        '
        'FirmwareLabel
        '
        Me.FirmwareLabel.AutoSize = True
        Me.FirmwareLabel.BackColor = System.Drawing.Color.Transparent
        Me.FirmwareLabel.Location = New System.Drawing.Point(8, 14)
        Me.FirmwareLabel.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
        Me.FirmwareLabel.MaximumSize = New System.Drawing.Size(0, 17)
        Me.FirmwareLabel.Name = "FirmwareLabel"
        Me.FirmwareLabel.Size = New System.Drawing.Size(120, 16)
        Me.FirmwareLabel.TabIndex = 10
        Me.FirmwareLabel.Text = "Firmware Version"
        Me.FirmwareLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'SerialNumberLabel
        '
        Me.SerialNumberLabel.AutoSize = True
        Me.SerialNumberLabel.BackColor = System.Drawing.Color.Transparent
        Me.SerialNumberLabel.Location = New System.Drawing.Point(8, 41)
        Me.SerialNumberLabel.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
        Me.SerialNumberLabel.MaximumSize = New System.Drawing.Size(0, 17)
        Me.SerialNumberLabel.Name = "SerialNumberLabel"
        Me.SerialNumberLabel.Size = New System.Drawing.Size(98, 16)
        Me.SerialNumberLabel.TabIndex = 6
        Me.SerialNumberLabel.Text = "Serial Number"
        Me.SerialNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LogoPictureBox
        '
        Me.LogoPictureBox.BackColor = System.Drawing.Color.Transparent
        Me.LogoPictureBox.InitialImage = CType(resources.GetObject("LogoPictureBox.InitialImage"), System.Drawing.Image)
        Me.LogoPictureBox.Location = New System.Drawing.Point(10, 9)
        Me.LogoPictureBox.Name = "LogoPictureBox"
        Me.LogoPictureBox.Size = New System.Drawing.Size(30, 30)
        Me.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.LogoPictureBox.TabIndex = 18
        Me.LogoPictureBox.TabStop = False
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.BackColor = System.Drawing.Color.FromArgb(CType(CType(223, Byte), Integer), CType(CType(237, Byte), Integer), CType(CType(251, Byte), Integer))
        Me.BsGroupBox1.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox1.Location = New System.Drawing.Point(2, 120)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New System.Drawing.Size(370, 3)
        Me.BsGroupBox1.TabIndex = 21
        Me.BsGroupBox1.TabStop = False
        '
        'IInstrumentInfo
        '
        Me.AcceptButton = Me.OKButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(374, 176)
        Me.ControlBox = False
        Me.Controls.Add(Me.BsPanel2)
        Me.Controls.Add(Me.BsGroupBox1)
        Me.Controls.Add(Me.OKButton)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IInstrumentInfo"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        Me.BsPanel2.ResumeLayout(False)
        CType(Me.GroupControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupControl1.ResumeLayout(False)
        Me.BsPanel1.ResumeLayout(False)
        Me.BsPanel1.PerformLayout()
        CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
    Friend WithEvents OKButton As System.Windows.Forms.Button
    Friend WithEvents BsPanel2 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents GroupControl1 As DevExpress.XtraEditors.GroupControl
    Friend WithEvents BsPanel1 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents FirmwareLabel As System.Windows.Forms.Label
    Friend WithEvents SerialNumberLabel As System.Windows.Forms.Label
    Friend WithEvents LogoPictureBox As System.Windows.Forms.PictureBox
    Friend WithEvents BsGroupBox1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox

End Class

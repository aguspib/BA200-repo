Namespace Biosystems.Ax00.Controls.UserControls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class bsAlert
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
            Me.AlertPanel = New System.Windows.Forms.TableLayoutPanel
            Me.BsPanel1 = New System.Windows.Forms.Panel
            Me.Panel2 = New System.Windows.Forms.Panel
            Me.DescriptionLabel = New System.Windows.Forms.Label
            Me.Panel1 = New System.Windows.Forms.Panel
            Me.bsAlertIconPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.TitleLabel = New System.Windows.Forms.Label
            Me.Panel3 = New System.Windows.Forms.Panel
            Me.Panel4 = New System.Windows.Forms.Panel
            Me.AlertPanel.SuspendLayout()
            Me.BsPanel1.SuspendLayout()
            Me.Panel2.SuspendLayout()
            Me.Panel1.SuspendLayout()
            CType(Me.bsAlertIconPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'AlertPanel
            '
            Me.AlertPanel.ColumnCount = 1
            Me.AlertPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.AlertPanel.Controls.Add(Me.BsPanel1, 0, 1)
            Me.AlertPanel.Controls.Add(Me.Panel3, 0, 0)
            Me.AlertPanel.Controls.Add(Me.Panel4, 0, 2)
            Me.AlertPanel.Cursor = System.Windows.Forms.Cursors.Default
            Me.AlertPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
            Me.AlertPanel.Location = New System.Drawing.Point(0, 0)
            Me.AlertPanel.Margin = New System.Windows.Forms.Padding(0)
            Me.AlertPanel.Name = "AlertPanel"
            Me.AlertPanel.RowCount = 3
            Me.AlertPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5.0!))
            Me.AlertPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.AlertPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 7.0!))
            Me.AlertPanel.Size = New System.Drawing.Size(190, 268)
            Me.AlertPanel.TabIndex = 3
            '
            'BsPanel1
            '
            Me.BsPanel1.BackgroundImage = Global.My.Resources.Resources.balloonmiddle
            Me.BsPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.BsPanel1.Controls.Add(Me.Panel2)
            Me.BsPanel1.Controls.Add(Me.Panel1)
            Me.BsPanel1.ForeColor = System.Drawing.Color.White
            Me.BsPanel1.Location = New System.Drawing.Point(0, 5)
            Me.BsPanel1.Margin = New System.Windows.Forms.Padding(0)
            Me.BsPanel1.Name = "BsPanel1"
            Me.BsPanel1.Size = New System.Drawing.Size(190, 256)
            Me.BsPanel1.TabIndex = 3
            '
            'Panel2
            '
            Me.Panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.Panel2.BackColor = System.Drawing.Color.Transparent
            Me.Panel2.Controls.Add(Me.DescriptionLabel)
            Me.Panel2.Cursor = System.Windows.Forms.Cursors.Default
            Me.Panel2.ForeColor = System.Drawing.Color.Black
            Me.Panel2.Location = New System.Drawing.Point(0, 16)
            Me.Panel2.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel2.Name = "Panel2"
            Me.Panel2.Size = New System.Drawing.Size(190, 240)
            Me.Panel2.TabIndex = 1
            '
            'DescriptionLabel
            '
            Me.DescriptionLabel.AutoEllipsis = True
            Me.DescriptionLabel.AutoSize = True
            Me.DescriptionLabel.BackColor = System.Drawing.Color.Transparent
            Me.DescriptionLabel.Font = New System.Drawing.Font("Verdana", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.DescriptionLabel.ForeColor = System.Drawing.Color.Black
            Me.DescriptionLabel.Location = New System.Drawing.Point(2, 0)
            Me.DescriptionLabel.MaximumSize = New System.Drawing.Size(188, 0)
            Me.DescriptionLabel.MinimumSize = New System.Drawing.Size(188, 0)
            Me.DescriptionLabel.Name = "DescriptionLabel"
            Me.DescriptionLabel.Size = New System.Drawing.Size(188, 12)
            Me.DescriptionLabel.TabIndex = 5
            Me.DescriptionLabel.Text = "Description"
            '
            'Panel1
            '
            Me.Panel1.BackColor = System.Drawing.Color.Transparent
            Me.Panel1.Controls.Add(Me.bsAlertIconPictureBox)
            Me.Panel1.Controls.Add(Me.TitleLabel)
            Me.Panel1.Location = New System.Drawing.Point(0, 0)
            Me.Panel1.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel1.MinimumSize = New System.Drawing.Size(0, 16)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(190, 16)
            Me.Panel1.TabIndex = 0
            '
            'bsAlertIconPictureBox
            '
            Me.bsAlertIconPictureBox.BackColor = System.Drawing.Color.Transparent
            Me.bsAlertIconPictureBox.Cursor = System.Windows.Forms.Cursors.Default
            Me.bsAlertIconPictureBox.Image = Global.My.Resources.Resources.AlertWarning
            Me.bsAlertIconPictureBox.Location = New System.Drawing.Point(3, 0)
            Me.bsAlertIconPictureBox.Margin = New System.Windows.Forms.Padding(0)
            Me.bsAlertIconPictureBox.Name = "bsAlertIconPictureBox"
            Me.bsAlertIconPictureBox.PositionNumber = 0
            Me.bsAlertIconPictureBox.Size = New System.Drawing.Size(16, 16)
            Me.bsAlertIconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
            Me.bsAlertIconPictureBox.TabIndex = 4
            Me.bsAlertIconPictureBox.TabStop = False
            '
            'TitleLabel
            '
            Me.TitleLabel.AutoEllipsis = True
            Me.TitleLabel.BackColor = System.Drawing.Color.Transparent
            Me.TitleLabel.Cursor = System.Windows.Forms.Cursors.Default
            Me.TitleLabel.Font = New System.Drawing.Font("Verdana", 6.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TitleLabel.ForeColor = System.Drawing.Color.Black
            Me.TitleLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.TitleLabel.Location = New System.Drawing.Point(22, 1)
            Me.TitleLabel.MinimumSize = New System.Drawing.Size(166, 16)
            Me.TitleLabel.Name = "TitleLabel"
            Me.TitleLabel.Size = New System.Drawing.Size(166, 16)
            Me.TitleLabel.TabIndex = 5
            Me.TitleLabel.Text = "Title"
            Me.TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Panel3
            '
            Me.Panel3.BackgroundImage = Global.My.Resources.Resources.balloontop
            Me.Panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Panel3.Cursor = System.Windows.Forms.Cursors.Default
            Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel3.Location = New System.Drawing.Point(0, 0)
            Me.Panel3.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel3.Name = "Panel3"
            Me.Panel3.Size = New System.Drawing.Size(190, 5)
            Me.Panel3.TabIndex = 0
            '
            'Panel4
            '
            Me.Panel4.BackgroundImage = Global.My.Resources.Resources.balloonbottom
            Me.Panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Panel4.Cursor = System.Windows.Forms.Cursors.Default
            Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel4.Location = New System.Drawing.Point(0, 261)
            Me.Panel4.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel4.Name = "Panel4"
            Me.Panel4.Size = New System.Drawing.Size(190, 7)
            Me.Panel4.TabIndex = 2
            '
            'bsAlert
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
            Me.BackColor = System.Drawing.Color.MistyRose
            Me.ClientSize = New System.Drawing.Size(190, 268)
            Me.Controls.Add(Me.AlertPanel)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Name = "bsAlert"
            Me.Opacity = 0.7
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
            Me.Text = "bsAlert"
            Me.TransparencyKey = System.Drawing.Color.MistyRose
            Me.AlertPanel.ResumeLayout(False)
            Me.BsPanel1.ResumeLayout(False)
            Me.Panel2.ResumeLayout(False)
            Me.Panel2.PerformLayout()
            Me.Panel1.ResumeLayout(False)
            CType(Me.bsAlertIconPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents AlertPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents BsPanel1 As System.Windows.Forms.Panel
        Friend WithEvents Panel2 As System.Windows.Forms.Panel
        Friend WithEvents DescriptionLabel As System.Windows.Forms.Label
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents TitleLabel As System.Windows.Forms.Label
        Friend WithEvents bsAlertIconPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents Panel3 As System.Windows.Forms.Panel
        Friend WithEvents Panel4 As System.Windows.Forms.Panel
    End Class

End Namespace
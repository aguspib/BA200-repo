Namespace Biosystems.Ax00.Controls.UserControls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class bsAlertArrowForm
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(bsAlertArrowForm))
            Me.AlertPanel = New System.Windows.Forms.TableLayoutPanel
            Me.BsPanel1 = New System.Windows.Forms.Panel
            Me.Panel2 = New System.Windows.Forms.Panel
            Me.DescriptionLabel = New System.Windows.Forms.Label
            Me.Panel1 = New System.Windows.Forms.Panel
            Me.TitleLabel = New System.Windows.Forms.Label
            Me.Panel3 = New System.Windows.Forms.Panel
            Me.Panel4 = New System.Windows.Forms.Panel
            Me.BsPictureBox1 = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.AlertPanel.SuspendLayout()
            Me.BsPanel1.SuspendLayout()
            Me.Panel2.SuspendLayout()
            Me.Panel1.SuspendLayout()
            CType(Me.BsPictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'AlertPanel
            '
            Me.AlertPanel.ColumnCount = 1
            Me.AlertPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.AlertPanel.Controls.Add(Me.BsPanel1, 0, 1)
            Me.AlertPanel.Controls.Add(Me.Panel3, 0, 0)
            Me.AlertPanel.Controls.Add(Me.Panel4, 0, 2)
            Me.AlertPanel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.AlertPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
            Me.AlertPanel.Location = New System.Drawing.Point(0, 0)
            Me.AlertPanel.Margin = New System.Windows.Forms.Padding(0)
            Me.AlertPanel.Name = "AlertPanel"
            Me.AlertPanel.RowCount = 3
            Me.AlertPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5.0!))
            Me.AlertPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.AlertPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 7.0!))
            Me.AlertPanel.Size = New System.Drawing.Size(208, 68)
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
            Me.BsPanel1.Size = New System.Drawing.Size(208, 56)
            Me.BsPanel1.TabIndex = 3
            '
            'Panel2
            '
            Me.Panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.Panel2.BackColor = System.Drawing.Color.Transparent
            Me.Panel2.Controls.Add(Me.DescriptionLabel)
            Me.Panel2.ForeColor = System.Drawing.Color.Black
            Me.Panel2.Location = New System.Drawing.Point(0, 16)
            Me.Panel2.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel2.Name = "Panel2"
            Me.Panel2.Size = New System.Drawing.Size(208, 40)
            Me.Panel2.TabIndex = 1
            '
            'DescriptionLabel
            '
            Me.DescriptionLabel.AutoEllipsis = True
            Me.DescriptionLabel.AutoSize = True
            Me.DescriptionLabel.BackColor = System.Drawing.Color.Transparent
            Me.DescriptionLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.DescriptionLabel.ForeColor = System.Drawing.Color.Black
            Me.DescriptionLabel.Location = New System.Drawing.Point(0, 0)
            Me.DescriptionLabel.MaximumSize = New System.Drawing.Size(212, 0)
            Me.DescriptionLabel.MinimumSize = New System.Drawing.Size(212, 13)
            Me.DescriptionLabel.Name = "DescriptionLabel"
            Me.DescriptionLabel.Size = New System.Drawing.Size(212, 13)
            Me.DescriptionLabel.TabIndex = 5
            Me.DescriptionLabel.Text = "Description"
            '
            'Panel1
            '
            Me.Panel1.BackColor = System.Drawing.Color.Transparent
            Me.Panel1.Controls.Add(Me.TitleLabel)
            Me.Panel1.Controls.Add(Me.BsPictureBox1)
            Me.Panel1.Location = New System.Drawing.Point(0, 0)
            Me.Panel1.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel1.MinimumSize = New System.Drawing.Size(0, 16)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(208, 16)
            Me.Panel1.TabIndex = 0
            '
            'TitleLabel
            '
            Me.TitleLabel.AutoEllipsis = True
            Me.TitleLabel.BackColor = System.Drawing.Color.Transparent
            Me.TitleLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TitleLabel.ForeColor = System.Drawing.Color.Black
            Me.TitleLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.TitleLabel.Location = New System.Drawing.Point(24, 0)
            Me.TitleLabel.MinimumSize = New System.Drawing.Size(184, 16)
            Me.TitleLabel.Name = "TitleLabel"
            Me.TitleLabel.Size = New System.Drawing.Size(184, 16)
            Me.TitleLabel.TabIndex = 5
            Me.TitleLabel.Text = "Title"
            '
            'Panel3
            '
            Me.Panel3.BackgroundImage = Global.My.Resources.Resources.balloontop
            Me.Panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel3.Location = New System.Drawing.Point(0, 0)
            Me.Panel3.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel3.Name = "Panel3"
            Me.Panel3.Size = New System.Drawing.Size(208, 5)
            Me.Panel3.TabIndex = 0
            '
            'Panel4
            '
            Me.Panel4.BackgroundImage = Global.My.Resources.Resources.balloonbottom
            Me.Panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel4.Location = New System.Drawing.Point(0, 61)
            Me.Panel4.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel4.Name = "Panel4"
            Me.Panel4.Size = New System.Drawing.Size(208, 7)
            Me.Panel4.TabIndex = 2
            '
            'BsPictureBox1
            '
            Me.BsPictureBox1.BackColor = System.Drawing.Color.Transparent
            Me.BsPictureBox1.Image = CType(resources.GetObject("BsPictureBox1.Image"), System.Drawing.Image)
            Me.BsPictureBox1.Location = New System.Drawing.Point(0, -2)
            Me.BsPictureBox1.Margin = New System.Windows.Forms.Padding(0)
            Me.BsPictureBox1.Name = "BsPictureBox1"
            Me.BsPictureBox1.PositionNumber = 0
            Me.BsPictureBox1.Size = New System.Drawing.Size(24, 20)
            Me.BsPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.BsPictureBox1.TabIndex = 4
            Me.BsPictureBox1.TabStop = False
            '
            'bsAlertArrowForm
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
            Me.BackColor = System.Drawing.Color.MistyRose
            Me.ClientSize = New System.Drawing.Size(449, 257)
            Me.Controls.Add(Me.AlertPanel)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.Name = "bsAlertArrowForm"
            Me.Opacity = 0.7
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
            Me.Text = "bsAlertArrowForm"
            Me.TransparencyKey = System.Drawing.Color.MistyRose
            Me.AlertPanel.ResumeLayout(False)
            Me.BsPanel1.ResumeLayout(False)
            Me.Panel2.ResumeLayout(False)
            Me.Panel2.PerformLayout()
            Me.Panel1.ResumeLayout(False)
            CType(Me.BsPictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents AlertPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents BsPanel1 As System.Windows.Forms.Panel
        Friend WithEvents Panel2 As System.Windows.Forms.Panel
        Friend WithEvents DescriptionLabel As System.Windows.Forms.Label
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents TitleLabel As System.Windows.Forms.Label
        Friend WithEvents BsPictureBox1 As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents Panel3 As System.Windows.Forms.Panel
        Friend WithEvents Panel4 As System.Windows.Forms.Panel
    End Class

End Namespace
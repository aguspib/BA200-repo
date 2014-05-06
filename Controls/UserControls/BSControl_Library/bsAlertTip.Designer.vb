Namespace Biosystems.Ax00.Controls.UserControls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class bsAlertTip
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(bsAlertTip))
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.Panel1 = New System.Windows.Forms.Panel
            Me.Panel3 = New System.Windows.Forms.Panel
            Me.LeftPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
            Me.Panel2 = New System.Windows.Forms.Panel
            Me.DescriptionLabel = New System.Windows.Forms.Label
            Me.Panel4 = New System.Windows.Forms.Panel
            Me.TitleLabel = New System.Windows.Forms.Label
            Me.BsPictureBox1 = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.TableLayoutPanel1.SuspendLayout()
            Me.LeftPanel.SuspendLayout()
            Me.Panel2.SuspendLayout()
            Me.Panel4.SuspendLayout()
            CType(Me.BsPictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'TableLayoutPanel1
            '
            Me.TableLayoutPanel1.AutoSize = True
            Me.TableLayoutPanel1.ColumnCount = 1
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.TableLayoutPanel1.Controls.Add(Me.LeftPanel, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.Panel1, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Panel3, 0, 2)
            Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
            Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            Me.TableLayoutPanel1.RowCount = 3
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
            Me.TableLayoutPanel1.Size = New System.Drawing.Size(212, 106)
            Me.TableLayoutPanel1.TabIndex = 0
            '
            'Panel1
            '
            Me.Panel1.BackgroundImage = CType(resources.GetObject("Panel1.BackgroundImage"), System.Drawing.Image)
            Me.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel1.Location = New System.Drawing.Point(0, 0)
            Me.Panel1.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(212, 10)
            Me.Panel1.TabIndex = 0
            '
            'Panel3
            '
            Me.Panel3.BackgroundImage = CType(resources.GetObject("Panel3.BackgroundImage"), System.Drawing.Image)
            Me.Panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel3.Location = New System.Drawing.Point(0, 76)
            Me.Panel3.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel3.Name = "Panel3"
            Me.Panel3.Size = New System.Drawing.Size(212, 30)
            Me.Panel3.TabIndex = 2
            '
            'LeftPanel
            '
            Me.LeftPanel.BackColor = System.Drawing.Color.Transparent
            Me.LeftPanel.BackgroundImage = CType(resources.GetObject("LeftPanel.BackgroundImage"), System.Drawing.Image)
            Me.LeftPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.LeftPanel.Controls.Add(Me.Panel2)
            Me.LeftPanel.Controls.Add(Me.Panel4)
            Me.LeftPanel.Cursor = System.Windows.Forms.Cursors.Hand
            Me.LeftPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.LeftPanel.ForeColor = System.Drawing.Color.White
            Me.LeftPanel.Location = New System.Drawing.Point(0, 10)
            Me.LeftPanel.Margin = New System.Windows.Forms.Padding(0)
            Me.LeftPanel.Name = "LeftPanel"
            Me.LeftPanel.Padding = New System.Windows.Forms.Padding(2)
            Me.LeftPanel.Size = New System.Drawing.Size(212, 66)
            Me.LeftPanel.TabIndex = 3
            '
            'Panel2
            '
            Me.Panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.Panel2.BackColor = System.Drawing.Color.Transparent
            Me.Panel2.Controls.Add(Me.DescriptionLabel)
            Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.Panel2.ForeColor = System.Drawing.Color.Black
            Me.Panel2.Location = New System.Drawing.Point(2, 24)
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
            Me.DescriptionLabel.MaximumSize = New System.Drawing.Size(204, 0)
            Me.DescriptionLabel.MinimumSize = New System.Drawing.Size(0, 13)
            Me.DescriptionLabel.Name = "DescriptionLabel"
            Me.DescriptionLabel.Size = New System.Drawing.Size(71, 13)
            Me.DescriptionLabel.TabIndex = 5
            Me.DescriptionLabel.Text = "Description"
            '
            'Panel4
            '
            Me.Panel4.BackColor = System.Drawing.Color.Transparent
            Me.Panel4.Controls.Add(Me.TitleLabel)
            Me.Panel4.Controls.Add(Me.BsPictureBox1)
            Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
            Me.Panel4.Location = New System.Drawing.Point(2, 2)
            Me.Panel4.Margin = New System.Windows.Forms.Padding(0)
            Me.Panel4.MinimumSize = New System.Drawing.Size(0, 20)
            Me.Panel4.Name = "Panel4"
            Me.Panel4.Size = New System.Drawing.Size(208, 20)
            Me.Panel4.TabIndex = 0
            '
            'TitleLabel
            '
            Me.TitleLabel.AutoEllipsis = True
            Me.TitleLabel.BackColor = System.Drawing.Color.Transparent
            Me.TitleLabel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TitleLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TitleLabel.ForeColor = System.Drawing.Color.Black
            Me.TitleLabel.Location = New System.Drawing.Point(24, 0)
            Me.TitleLabel.Name = "TitleLabel"
            Me.TitleLabel.Size = New System.Drawing.Size(184, 20)
            Me.TitleLabel.TabIndex = 5
            Me.TitleLabel.Text = "Title"
            Me.TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'BsPictureBox1
            '
            Me.BsPictureBox1.BackColor = System.Drawing.Color.Transparent
            Me.BsPictureBox1.Dock = System.Windows.Forms.DockStyle.Left
            Me.BsPictureBox1.Image = CType(resources.GetObject("BsPictureBox1.Image"), System.Drawing.Image)
            Me.BsPictureBox1.Location = New System.Drawing.Point(0, 0)
            Me.BsPictureBox1.Name = "BsPictureBox1"
            Me.BsPictureBox1.PositionNumber = 0
            Me.BsPictureBox1.Size = New System.Drawing.Size(24, 20)
            Me.BsPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            Me.BsPictureBox1.TabIndex = 4
            Me.BsPictureBox1.TabStop = False
            '
            'bsAlertTip
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
            Me.BackColor = System.Drawing.Color.Transparent
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Name = "bsAlertTip"
            Me.Size = New System.Drawing.Size(212, 106)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.LeftPanel.ResumeLayout(False)
            Me.Panel2.ResumeLayout(False)
            Me.Panel2.PerformLayout()
            Me.Panel4.ResumeLayout(False)
            CType(Me.BsPictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents Panel3 As System.Windows.Forms.Panel
        Friend WithEvents LeftPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
        Friend WithEvents Panel2 As System.Windows.Forms.Panel
        Friend WithEvents DescriptionLabel As System.Windows.Forms.Label
        Friend WithEvents Panel4 As System.Windows.Forms.Panel
        Friend WithEvents TitleLabel As System.Windows.Forms.Label
        Friend WithEvents BsPictureBox1 As Biosystems.Ax00.Controls.UserControls.BSPictureBox

    End Class

End Namespace
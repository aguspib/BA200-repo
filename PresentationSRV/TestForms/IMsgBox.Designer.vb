<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IMsgBox
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IMsgBox))
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsMessageIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.ButtonsLayoutPanel = New System.Windows.Forms.TableLayoutPanel
        Me.BsImageList = New System.Windows.Forms.ImageList(Me.components)
        CType(Me.BsMessageIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsMessageLabel
        '
        Me.BsMessageLabel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessageLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsMessageLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsMessageLabel.ForeColor = System.Drawing.Color.Black
        Me.BsMessageLabel.Location = New System.Drawing.Point(49, 9)
        Me.BsMessageLabel.Name = "BsMessageLabel"
        Me.BsMessageLabel.Size = New System.Drawing.Size(381, 16)
        Me.BsMessageLabel.TabIndex = 1
        Me.BsMessageLabel.Text = "BsLabel1"
        Me.BsMessageLabel.Title = False
        '
        'BsMessageIcon
        '
        Me.BsMessageIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsMessageIcon.Location = New System.Drawing.Point(11, 7)
        Me.BsMessageIcon.Name = "BsMessageIcon"
        Me.BsMessageIcon.PositionNumber = 0
        Me.BsMessageIcon.Size = New System.Drawing.Size(32, 32)
        Me.BsMessageIcon.TabIndex = 4
        Me.BsMessageIcon.TabStop = False
        '
        'ButtonsLayoutPanel
        '
        Me.ButtonsLayoutPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonsLayoutPanel.ColumnCount = 3
        Me.ButtonsLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.ButtonsLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.ButtonsLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.ButtonsLayoutPanel.Location = New System.Drawing.Point(198, 48)
        Me.ButtonsLayoutPanel.Name = "ButtonsLayoutPanel"
        Me.ButtonsLayoutPanel.RowCount = 1
        Me.ButtonsLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.ButtonsLayoutPanel.Size = New System.Drawing.Size(232, 29)
        Me.ButtonsLayoutPanel.TabIndex = 0
        '
        'BsImageList
        '
        Me.BsImageList.ImageStream = CType(resources.GetObject("BsImageList.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.BsImageList.TransparentColor = System.Drawing.Color.Transparent
        Me.BsImageList.Images.SetKeyName(0, "IMG_HOME")
        Me.BsImageList.Images.SetKeyName(1, "IMG_HOME_DIS")
        Me.BsImageList.Images.SetKeyName(2, "IMG_LEFT")
        Me.BsImageList.Images.SetKeyName(3, "IMG_LEFT_DIS")
        Me.BsImageList.Images.SetKeyName(4, "IMG_RIGHT")
        Me.BsImageList.Images.SetKeyName(5, "IMG_RIGHT_DIS")
        Me.BsImageList.Images.SetKeyName(6, "IMG_DOWN")
        Me.BsImageList.Images.SetKeyName(7, "IMG_DOWN_DIS")
        Me.BsImageList.Images.SetKeyName(8, "IMG_UP")
        Me.BsImageList.Images.SetKeyName(9, "IMG_UP_DIS")
        Me.BsImageList.Images.SetKeyName(10, "IMG_ENTER")
        Me.BsImageList.Images.SetKeyName(11, "IMG_ENTER_DIS")
        '
        'IMsgBox
        '
        Me.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(435, 83)
        Me.Controls.Add(Me.BsMessageIcon)
        Me.Controls.Add(Me.BsMessageLabel)
        Me.Controls.Add(Me.ButtonsLayoutPanel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IMsgBox"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "IMsgBox"
        CType(Me.BsMessageIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsMessageIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents ButtonsLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents BsImageList As System.Windows.Forms.ImageList

End Class

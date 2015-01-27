Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.PresentationCOM
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class UiMsgBox
    Inherits BSBaseForm

    'Form overrides dispose to clean up the component list.
    <DebuggerNonUserCode()> _
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
    Private components As IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New Container
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(UiMsgBox))
        Me.BsMessageLabel = New BSLabel
        Me.BsMessageIcon = New BSPictureBox
        Me.ButtonsLayoutPanel = New TableLayoutPanel
        Me.BsImageList = New ImageList(Me.components)
        CType(Me.BsMessageIcon, ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsMessageLabel
        '
        Me.BsMessageLabel.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
                    Or AnchorStyles.Left) _
                    Or AnchorStyles.Right), AnchorStyles)
        Me.BsMessageLabel.BackColor = Color.Transparent
        Me.BsMessageLabel.Font = New Font("Verdana", 8.25!)
        Me.BsMessageLabel.ForeColor = Color.Black
        Me.BsMessageLabel.Location = New Point(49, 9)
        Me.BsMessageLabel.Name = "BsMessageLabel"
        Me.BsMessageLabel.Size = New Size(381, 16)
        Me.BsMessageLabel.TabIndex = 1
        Me.BsMessageLabel.Text = "BsLabel1"
        Me.BsMessageLabel.Title = False
        '
        'BsMessageIcon
        '
        Me.BsMessageIcon.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsMessageIcon.Location = New Point(11, 7)
        Me.BsMessageIcon.Name = "BsMessageIcon"
        Me.BsMessageIcon.PositionNumber = 0
        Me.BsMessageIcon.Size = New Size(32, 32)
        Me.BsMessageIcon.TabIndex = 4
        Me.BsMessageIcon.TabStop = False
        '
        'ButtonsLayoutPanel
        '
        Me.ButtonsLayoutPanel.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
        Me.ButtonsLayoutPanel.ColumnCount = 3
        Me.ButtonsLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33333!))
        Me.ButtonsLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33333!))
        Me.ButtonsLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33333!))
        Me.ButtonsLayoutPanel.Location = New Point(198, 48)
        Me.ButtonsLayoutPanel.Name = "ButtonsLayoutPanel"
        Me.ButtonsLayoutPanel.RowCount = 1
        Me.ButtonsLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!))
        Me.ButtonsLayoutPanel.Size = New Size(232, 29)
        Me.ButtonsLayoutPanel.TabIndex = 0
        '
        'BsImageList
        '
        Me.BsImageList.ImageStream = CType(resources.GetObject("BsImageList.ImageStream"), ImageListStreamer)
        Me.BsImageList.TransparentColor = Color.Transparent
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
        Me.Appearance.BackColor = Color.WhiteSmoke
        Me.Appearance.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(435, 83)
        Me.Controls.Add(Me.BsMessageIcon)
        Me.Controls.Add(Me.BsMessageLabel)
        Me.Controls.Add(Me.ButtonsLayoutPanel)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IMsgBox"
        Me.ShowInTaskbar = False
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "IMsgBox"
        CType(Me.BsMessageIcon, ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsMessageLabel As BSLabel
    Friend WithEvents BsMessageIcon As BSPictureBox
    Friend WithEvents ButtonsLayoutPanel As TableLayoutPanel
    Friend WithEvents BsImageList As ImageList

End Class

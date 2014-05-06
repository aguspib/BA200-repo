<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class XRDesignMasterForm
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.BsPanel1 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsPanel2 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsPanel3 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsToolTips1 = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.BsPanel1.SuspendLayout()
        Me.bsPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'BsPanel1
        '
        Me.BsPanel1.BackColor = System.Drawing.Color.Gainsboro
        Me.BsPanel1.Controls.Add(Me.bsPanel2)
        Me.BsPanel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.BsPanel1.Location = New System.Drawing.Point(0, 702)
        Me.BsPanel1.Name = "BsPanel1"
        Me.BsPanel1.Size = New System.Drawing.Size(1018, 40)
        Me.BsPanel1.TabIndex = 0
        '
        'bsPanel2
        '
        Me.bsPanel2.Controls.Add(Me.ExitButton)
        Me.bsPanel2.Location = New System.Drawing.Point(932, 4)
        Me.bsPanel2.Name = "bsPanel2"
        Me.bsPanel2.Size = New System.Drawing.Size(69, 32)
        Me.bsPanel2.TabIndex = 35
        '
        'ExitButton
        '
        Me.ExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExitButton.Location = New System.Drawing.Point(37, 0)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(32, 32)
        Me.ExitButton.TabIndex = 167
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'BsPanel3
        '
        Me.BsPanel3.BackColor = System.Drawing.Color.Transparent
        Me.BsPanel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.BsPanel3.Location = New System.Drawing.Point(0, 0)
        Me.BsPanel3.Name = "BsPanel3"
        Me.BsPanel3.Size = New System.Drawing.Size(1018, 620)
        Me.BsPanel3.TabIndex = 1
        '
        'XRDesignMasterForm
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.ClientSize = New System.Drawing.Size(1018, 742)
        Me.ControlBox = False
        Me.Controls.Add(Me.BsPanel3)
        Me.Controls.Add(Me.BsPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "XRDesignMasterForm"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Preview Report"
        Me.BsPanel1.ResumeLayout(False)
        Me.bsPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsPanel1 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsPanel2 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents ExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsPanel3 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsToolTips1 As Biosystems.Ax00.Controls.UserControls.BSToolTip

End Class

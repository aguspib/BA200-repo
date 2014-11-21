<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UpdateProcessValidation
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
        Me.bsUpdateOFFSTestsButton = New System.Windows.Forms.Button()
        Me.bsUpdateCALCTestsButton = New System.Windows.Forms.Button()
        Me.bsUpdateISETestsButton = New System.Windows.Forms.Button()
        Me.bsUpdateSTDTestsButton = New System.Windows.Forms.Button()
        Me.bsUpdateISETestsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.bsUpdateISETestsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.XMLViewer = New System.Windows.Forms.RichTextBox()
        Me.bsUpdateSTDTestsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.bsUpdateSTDTestsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsUpdateOFFSTestsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.bsUpdateOFFSTestsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsUpdateCALCTestsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.bsUpdateCALCTestsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsExitButton = New System.Windows.Forms.Button()
        Me.bsUpdateISETestsPanel.SuspendLayout()
        Me.bsUpdateSTDTestsPanel.SuspendLayout()
        Me.bsUpdateOFFSTestsPanel.SuspendLayout()
        Me.bsUpdateCALCTestsPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsUpdateOFFSTestsButton
        '
        Me.bsUpdateOFFSTestsButton.Location = New System.Drawing.Point(30, 32)
        Me.bsUpdateOFFSTestsButton.Name = "bsUpdateOFFSTestsButton"
        Me.bsUpdateOFFSTestsButton.Size = New System.Drawing.Size(100, 39)
        Me.bsUpdateOFFSTestsButton.TabIndex = 32
        Me.bsUpdateOFFSTestsButton.Text = "UPDATE OFFS"
        Me.bsUpdateOFFSTestsButton.UseVisualStyleBackColor = True
        '
        'bsUpdateCALCTestsButton
        '
        Me.bsUpdateCALCTestsButton.Location = New System.Drawing.Point(30, 32)
        Me.bsUpdateCALCTestsButton.Name = "bsUpdateCALCTestsButton"
        Me.bsUpdateCALCTestsButton.Size = New System.Drawing.Size(100, 39)
        Me.bsUpdateCALCTestsButton.TabIndex = 31
        Me.bsUpdateCALCTestsButton.Text = "UPDATE CALC"
        Me.bsUpdateCALCTestsButton.UseVisualStyleBackColor = True
        '
        'bsUpdateISETestsButton
        '
        Me.bsUpdateISETestsButton.Location = New System.Drawing.Point(30, 32)
        Me.bsUpdateISETestsButton.Name = "bsUpdateISETestsButton"
        Me.bsUpdateISETestsButton.Size = New System.Drawing.Size(100, 39)
        Me.bsUpdateISETestsButton.TabIndex = 30
        Me.bsUpdateISETestsButton.Text = "UPDATE ISE"
        Me.bsUpdateISETestsButton.UseVisualStyleBackColor = True
        '
        'bsUpdateSTDTestsButton
        '
        Me.bsUpdateSTDTestsButton.Location = New System.Drawing.Point(30, 32)
        Me.bsUpdateSTDTestsButton.Name = "bsUpdateSTDTestsButton"
        Me.bsUpdateSTDTestsButton.Size = New System.Drawing.Size(100, 39)
        Me.bsUpdateSTDTestsButton.TabIndex = 29
        Me.bsUpdateSTDTestsButton.Text = "UPDATE STD"
        Me.bsUpdateSTDTestsButton.UseVisualStyleBackColor = True
        '
        'bsUpdateISETestsPanel
        '
        Me.bsUpdateISETestsPanel.Controls.Add(Me.bsUpdateISETestsLabel)
        Me.bsUpdateISETestsPanel.Controls.Add(Me.bsUpdateISETestsButton)
        Me.bsUpdateISETestsPanel.Location = New System.Drawing.Point(12, 12)
        Me.bsUpdateISETestsPanel.Name = "bsUpdateISETestsPanel"
        Me.bsUpdateISETestsPanel.Size = New System.Drawing.Size(171, 79)
        Me.bsUpdateISETestsPanel.TabIndex = 33
        '
        'bsUpdateISETestsLabel
        '
        Me.bsUpdateISETestsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsUpdateISETestsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsUpdateISETestsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUpdateISETestsLabel.Location = New System.Drawing.Point(10, 7)
        Me.bsUpdateISETestsLabel.Name = "bsUpdateISETestsLabel"
        Me.bsUpdateISETestsLabel.Size = New System.Drawing.Size(150, 20)
        Me.bsUpdateISETestsLabel.TabIndex = 31
        Me.bsUpdateISETestsLabel.Text = "Update ISE Tests"
        Me.bsUpdateISETestsLabel.Title = True
        '
        'XMLViewer
        '
        Me.XMLViewer.Location = New System.Drawing.Point(189, 12)
        Me.XMLViewer.Name = "XMLViewer"
        Me.XMLViewer.ReadOnly = True
        Me.XMLViewer.Size = New System.Drawing.Size(607, 334)
        Me.XMLViewer.TabIndex = 34
        Me.XMLViewer.Text = ""
        '
        'bsUpdateSTDTestsPanel
        '
        Me.bsUpdateSTDTestsPanel.Controls.Add(Me.bsUpdateSTDTestsLabel)
        Me.bsUpdateSTDTestsPanel.Controls.Add(Me.bsUpdateSTDTestsButton)
        Me.bsUpdateSTDTestsPanel.Location = New System.Drawing.Point(12, 97)
        Me.bsUpdateSTDTestsPanel.Name = "bsUpdateSTDTestsPanel"
        Me.bsUpdateSTDTestsPanel.Size = New System.Drawing.Size(171, 79)
        Me.bsUpdateSTDTestsPanel.TabIndex = 34
        '
        'bsUpdateSTDTestsLabel
        '
        Me.bsUpdateSTDTestsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsUpdateSTDTestsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsUpdateSTDTestsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUpdateSTDTestsLabel.Location = New System.Drawing.Point(10, 7)
        Me.bsUpdateSTDTestsLabel.Name = "bsUpdateSTDTestsLabel"
        Me.bsUpdateSTDTestsLabel.Size = New System.Drawing.Size(150, 20)
        Me.bsUpdateSTDTestsLabel.TabIndex = 31
        Me.bsUpdateSTDTestsLabel.Text = "Update STD Tests"
        Me.bsUpdateSTDTestsLabel.Title = True
        '
        'bsUpdateOFFSTestsPanel
        '
        Me.bsUpdateOFFSTestsPanel.Controls.Add(Me.bsUpdateOFFSTestsLabel)
        Me.bsUpdateOFFSTestsPanel.Controls.Add(Me.bsUpdateOFFSTestsButton)
        Me.bsUpdateOFFSTestsPanel.Location = New System.Drawing.Point(12, 182)
        Me.bsUpdateOFFSTestsPanel.Name = "bsUpdateOFFSTestsPanel"
        Me.bsUpdateOFFSTestsPanel.Size = New System.Drawing.Size(171, 79)
        Me.bsUpdateOFFSTestsPanel.TabIndex = 35
        '
        'bsUpdateOFFSTestsLabel
        '
        Me.bsUpdateOFFSTestsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsUpdateOFFSTestsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsUpdateOFFSTestsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUpdateOFFSTestsLabel.Location = New System.Drawing.Point(10, 7)
        Me.bsUpdateOFFSTestsLabel.Name = "bsUpdateOFFSTestsLabel"
        Me.bsUpdateOFFSTestsLabel.Size = New System.Drawing.Size(150, 20)
        Me.bsUpdateOFFSTestsLabel.TabIndex = 31
        Me.bsUpdateOFFSTestsLabel.Text = "Update OFFS Tests"
        Me.bsUpdateOFFSTestsLabel.Title = True
        '
        'bsUpdateCALCTestsPanel
        '
        Me.bsUpdateCALCTestsPanel.Controls.Add(Me.bsUpdateCALCTestsLabel)
        Me.bsUpdateCALCTestsPanel.Controls.Add(Me.bsUpdateCALCTestsButton)
        Me.bsUpdateCALCTestsPanel.Location = New System.Drawing.Point(12, 267)
        Me.bsUpdateCALCTestsPanel.Name = "bsUpdateCALCTestsPanel"
        Me.bsUpdateCALCTestsPanel.Size = New System.Drawing.Size(171, 79)
        Me.bsUpdateCALCTestsPanel.TabIndex = 36
        '
        'bsUpdateCALCTestsLabel
        '
        Me.bsUpdateCALCTestsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsUpdateCALCTestsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsUpdateCALCTestsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUpdateCALCTestsLabel.Location = New System.Drawing.Point(10, 7)
        Me.bsUpdateCALCTestsLabel.Name = "bsUpdateCALCTestsLabel"
        Me.bsUpdateCALCTestsLabel.Size = New System.Drawing.Size(150, 20)
        Me.bsUpdateCALCTestsLabel.TabIndex = 31
        Me.bsUpdateCALCTestsLabel.Text = "Update CALC Tests"
        Me.bsUpdateCALCTestsLabel.Title = True
        '
        'bsExitButton
        '
        Me.bsExitButton.Location = New System.Drawing.Point(721, 352)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(75, 34)
        Me.bsExitButton.TabIndex = 32
        Me.bsExitButton.Text = "CLOSE"
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'UpdateProcessValidation
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.ClientSize = New System.Drawing.Size(811, 393)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsUpdateCALCTestsPanel)
        Me.Controls.Add(Me.bsUpdateOFFSTestsPanel)
        Me.Controls.Add(Me.bsUpdateSTDTestsPanel)
        Me.Controls.Add(Me.XMLViewer)
        Me.Controls.Add(Me.bsUpdateISETestsPanel)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "UpdateProcessValidation"
        Me.bsUpdateISETestsPanel.ResumeLayout(False)
        Me.bsUpdateSTDTestsPanel.ResumeLayout(False)
        Me.bsUpdateOFFSTestsPanel.ResumeLayout(False)
        Me.bsUpdateCALCTestsPanel.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsUpdateOFFSTestsButton As System.Windows.Forms.Button
    Friend WithEvents bsUpdateCALCTestsButton As System.Windows.Forms.Button
    Friend WithEvents bsUpdateISETestsButton As System.Windows.Forms.Button
    Friend WithEvents bsUpdateSTDTestsButton As System.Windows.Forms.Button
    Friend WithEvents bsUpdateISETestsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsUpdateISETestsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents XMLViewer As System.Windows.Forms.RichTextBox
    Friend WithEvents bsUpdateSTDTestsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsUpdateSTDTestsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsUpdateOFFSTestsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsUpdateOFFSTestsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsUpdateCALCTestsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsUpdateCALCTestsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsExitButton As System.Windows.Forms.Button

End Class

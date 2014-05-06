<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TestISEMonitor
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
        Me.BsiseMonitorPanel1 = New Biosystems.Ax00.Controls.UserControls.BSISEMonitorPanel
        Me.SuspendLayout()
        '
        'BsiseMonitorPanel1
        '
        Me.BsiseMonitorPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsiseMonitorPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsiseMonitorPanel1.MaximumSize = New System.Drawing.Size(978, 623)
        Me.BsiseMonitorPanel1.MinimumSize = New System.Drawing.Size(762, 593)
        Me.BsiseMonitorPanel1.Name = "BsiseMonitorPanel1"
        Me.BsiseMonitorPanel1.Size = New System.Drawing.Size(762, 623)
        Me.BsiseMonitorPanel1.TabIndex = 0
        '
        'TestISEMonitor
        '
        Me.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(762, 623)
        Me.Controls.Add(Me.BsiseMonitorPanel1)
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "TestISEMonitor"
        Me.Text = "TestISEMonitor"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsiseMonitorPanel1 As Biosystems.Ax00.Controls.UserControls.BSISEMonitorPanel
End Class

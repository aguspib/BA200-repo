Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.PresentationCOM
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class TestISEMonitor
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
        Me.BsiseMonitorPanel1 = New BSISEMonitorPanel
        Me.SuspendLayout()
        '
        'BsiseMonitorPanel1
        '
        Me.BsiseMonitorPanel1.BorderStyle = BorderStyle.FixedSingle
        Me.BsiseMonitorPanel1.Location = New Point(0, 0)
        Me.BsiseMonitorPanel1.MaximumSize = New Size(978, 623)
        Me.BsiseMonitorPanel1.MinimumSize = New Size(762, 593)
        Me.BsiseMonitorPanel1.Name = "BsiseMonitorPanel1"
        Me.BsiseMonitorPanel1.Size = New Size(762, 623)
        Me.BsiseMonitorPanel1.TabIndex = 0
        '
        'TestISEMonitor
        '
        Me.Appearance.BackColor = Color.WhiteSmoke
        Me.Appearance.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(762, 623)
        Me.Controls.Add(Me.BsiseMonitorPanel1)
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "TestISEMonitor"
        Me.Text = "TestISEMonitor"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsiseMonitorPanel1 As BSISEMonitorPanel
End Class

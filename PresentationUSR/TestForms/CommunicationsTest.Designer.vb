<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CommunicationsTest
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
        Me.BsGroupBox1 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsReceive = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.SimulatedFrameReceived = New System.Windows.Forms.TextBox()
        Me.BsGroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.Controls.Add(Me.BsReceive)
        Me.BsGroupBox1.Controls.Add(Me.SimulatedFrameReceived)
        Me.BsGroupBox1.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox1.Location = New System.Drawing.Point(12, 11)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New System.Drawing.Size(1104, 648)
        Me.BsGroupBox1.TabIndex = 10
        Me.BsGroupBox1.TabStop = False
        Me.BsGroupBox1.Text = "Frame Reception Simulation"
        '
        'BsReceive
        '
        Me.BsReceive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsReceive.Location = New System.Drawing.Point(15, 26)
        Me.BsReceive.Name = "BsReceive"
        Me.BsReceive.Size = New System.Drawing.Size(119, 24)
        Me.BsReceive.TabIndex = 42
        Me.BsReceive.Text = "Process Reception"
        Me.BsReceive.UseVisualStyleBackColor = True
        '
        'SimulatedFrameReceived
        '
        Me.SimulatedFrameReceived.Location = New System.Drawing.Point(15, 56)
        Me.SimulatedFrameReceived.Multiline = True
        Me.SimulatedFrameReceived.Name = "SimulatedFrameReceived"
        Me.SimulatedFrameReceived.Size = New System.Drawing.Size(661, 194)
        Me.SimulatedFrameReceived.TabIndex = 38
        '
        'CommunicationsTest
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1136, 736)
        Me.Controls.Add(Me.BsGroupBox1)
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "CommunicationsTest"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "Communications Test"
        Me.BsGroupBox1.ResumeLayout(False)
        Me.BsGroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsGroupBox1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsReceive As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents SimulatedFrameReceived As System.Windows.Forms.TextBox
End Class
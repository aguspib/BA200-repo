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
        Me.btnWaterDepositErrAlarm = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsReceive = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.SimulatedFrameReceived = New System.Windows.Forms.TextBox()
        Me.BsGroupBox2 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.BsGroupBox1.SuspendLayout()
        Me.BsGroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.Controls.Add(Me.BsReceive)
        Me.BsGroupBox1.Controls.Add(Me.SimulatedFrameReceived)
        Me.BsGroupBox1.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox1.Location = New System.Drawing.Point(12, 11)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New System.Drawing.Size(1104, 330)
        Me.BsGroupBox1.TabIndex = 10
        Me.BsGroupBox1.TabStop = False
        Me.BsGroupBox1.Text = "Frame Reception Simulation"
        '
        'btnWaterDepositErrAlarm
        '
        Me.btnWaterDepositErrAlarm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnWaterDepositErrAlarm.Location = New System.Drawing.Point(164, 33)
        Me.btnWaterDepositErrAlarm.Name = "btnWaterDepositErrAlarm"
        Me.btnWaterDepositErrAlarm.Size = New System.Drawing.Size(167, 24)
        Me.btnWaterDepositErrAlarm.TabIndex = 42
        Me.btnWaterDepositErrAlarm.Text = "Enable Simulate Alarm"
        Me.btnWaterDepositErrAlarm.UseVisualStyleBackColor = True
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
        'BsGroupBox2
        '
        Me.BsGroupBox2.Controls.Add(Me.Label2)
        Me.BsGroupBox2.Controls.Add(Me.Label1)
        Me.BsGroupBox2.Controls.Add(Me.btnWaterDepositErrAlarm)
        Me.BsGroupBox2.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox2.Location = New System.Drawing.Point(12, 361)
        Me.BsGroupBox2.Name = "BsGroupBox2"
        Me.BsGroupBox2.Size = New System.Drawing.Size(1104, 330)
        Me.BsGroupBox2.TabIndex = 43
        Me.BsGroupBox2.TabStop = False
        Me.BsGroupBox2.Text = "Alarm Reception Simulation"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(142, 13)
        Me.Label1.TabIndex = 43
        Me.Label1.Text = "WATER_DEPOSIT_ERR:"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(337, 33)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(629, 37)
        Me.Label2.TabIndex = 44
        Me.Label2.Text = "In every ANSINF reception from Analyzer, the  water deposit sensor (field7:HS) is" & _
    " set to 0 to simulate Alarm. Alarm is informed to user after some time (default:" & _
    " 5 minuts) receiving the error."
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
        Me.Controls.Add(Me.BsGroupBox2)
        Me.Controls.Add(Me.BsGroupBox1)
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "CommunicationsTest"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "Communications Test"
        Me.BsGroupBox1.ResumeLayout(False)
        Me.BsGroupBox1.PerformLayout()
        Me.BsGroupBox2.ResumeLayout(False)
        Me.BsGroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsGroupBox1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsReceive As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents SimulatedFrameReceived As System.Windows.Forms.TextBox
    Friend WithEvents btnWaterDepositErrAlarm As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsGroupBox2 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
Namespace Biosystems.Ax00.Controls.UserControls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class bsLed
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(bsLed))
            Me.TextLabel = New System.Windows.Forms.Label
            Me.BsMonitorLED = New Biosystems.Ax00.Controls.UserControls.BSMonitorLED
            Me.SuspendLayout()
            '
            'TextLabel
            '
            Me.TextLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.TextLabel.BackColor = System.Drawing.Color.Transparent
            Me.TextLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TextLabel.ForeColor = System.Drawing.Color.Black
            Me.TextLabel.Location = New System.Drawing.Point(0, 0)
            Me.TextLabel.Name = "TextLabel"
            Me.TextLabel.Size = New System.Drawing.Size(108, 23)
            Me.TextLabel.TabIndex = 1
            Me.TextLabel.Text = "bsLed"
            Me.TextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'BsMonitorLED
            '
            Me.BsMonitorLED.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsMonitorLED.BackColor = System.Drawing.Color.Transparent
            Me.BsMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status._OFF
            Me.BsMonitorLED.LightColor = CType(resources.GetObject("BsMonitorLED.LightColor"), System.Collections.Generic.List(Of System.Drawing.Color))
            Me.BsMonitorLED.Location = New System.Drawing.Point(116, 0)
            Me.BsMonitorLED.MaxLimit = 100
            Me.BsMonitorLED.MinLimit = 0
            Me.BsMonitorLED.Name = "BsMonitorLED"
            Me.BsMonitorLED.Size = New System.Drawing.Size(21, 21)
            Me.BsMonitorLED.TabIndex = 2
            Me.BsMonitorLED.TitleAlignment = System.Drawing.ContentAlignment.BottomCenter
            Me.BsMonitorLED.TitleFont = Nothing
            Me.BsMonitorLED.TitleForeColor = System.Drawing.Color.Black
            Me.BsMonitorLED.TitleHeight = 0
            Me.BsMonitorLED.TitleText = ""
            '
            'bsLed
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.Transparent
            Me.Controls.Add(Me.BsMonitorLED)
            Me.Controls.Add(Me.TextLabel)
            Me.Name = "bsLed"
            Me.Size = New System.Drawing.Size(139, 23)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents TextLabel As System.Windows.Forms.Label
        Friend WithEvents BsMonitorLED As Biosystems.Ax00.Controls.UserControls.BSMonitorLED

    End Class

End Namespace

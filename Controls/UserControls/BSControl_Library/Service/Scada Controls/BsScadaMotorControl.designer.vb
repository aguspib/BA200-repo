

Namespace Biosystems.Ax00.Controls.UserControls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BsScadaMotorControl
        Inherits BsScadaControl

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BsScadaMotorControl))
            Me.ControlWidget = New PerpetuumSoft.Instrumentation.Windows.Forms.IndicatorWidget
            Me.ActuatorWidget = New PerpetuumSoft.Instrumentation.Windows.Forms.IndicatorWidget
            Me.SuspendLayout()
            '
            'ControlWidget
            '
            Me.ControlWidget.Cursor = System.Windows.Forms.Cursors.Default
            Me.ControlWidget.Dock = System.Windows.Forms.DockStyle.Fill
            Me.ControlWidget.HideFocusRectangle = True
            Me.ControlWidget.InstrumentationStream = resources.GetString("ControlWidget.InstrumentationStream")
            Me.ControlWidget.Location = New System.Drawing.Point(0, 0)
            Me.ControlWidget.Margin = New System.Windows.Forms.Padding(0)
            Me.ControlWidget.Maximum = 0
            Me.ControlWidget.Minimum = 0
            Me.ControlWidget.Name = "ControlWidget"
            Me.ControlWidget.RightToLeft = System.Windows.Forms.RightToLeft.Yes
            Me.ControlWidget.Size = New System.Drawing.Size(60, 60)
            Me.ControlWidget.TabIndex = 5
            '
            'ActuatorWidget
            '
            Me.ActuatorWidget.Cursor = System.Windows.Forms.Cursors.Default
            Me.ActuatorWidget.HideFocusRectangle = True
            Me.ActuatorWidget.InstrumentationStream = resources.GetString("ActuatorWidget.InstrumentationStream")
            Me.ActuatorWidget.Location = New System.Drawing.Point(0, 0)
            Me.ActuatorWidget.Margin = New System.Windows.Forms.Padding(0)
            Me.ActuatorWidget.Maximum = 0
            Me.ActuatorWidget.Minimum = 0
            Me.ActuatorWidget.Name = "ActuatorWidget"
            Me.ActuatorWidget.Size = New System.Drawing.Size(70, 30)
            Me.ActuatorWidget.TabIndex = 6
            '
            'BsScadaMotorControl
            '
            Me.ActivatedColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(102, Byte), Integer), CType(CType(204, Byte), Integer))
            Me.ActivationState = Biosystems.Ax00.Controls.UserControls.BsScadaControl.States._ON
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.Transparent
            Me.Controls.Add(Me.ControlWidget)
            Me.Controls.Add(Me.ActuatorWidget)
            Me.Cursor = System.Windows.Forms.Cursors.Hand
            Me.Name = "BsScadaMotorControl"
            Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
            Me.Size = New System.Drawing.Size(60, 60)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents ControlWidget As PerpetuumSoft.Instrumentation.Windows.Forms.IndicatorWidget
        Friend WithEvents ActuatorWidget As PerpetuumSoft.Instrumentation.Windows.Forms.IndicatorWidget

    End Class
End Namespace
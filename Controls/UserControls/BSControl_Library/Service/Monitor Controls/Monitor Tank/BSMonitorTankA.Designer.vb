Namespace Biosystems.Ax00.Controls.UserControls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BSMonitorTankA
        Inherits BSMonitorTank

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BSMonitorTankA))
            Me.IndicatorWidget1 = New PerpetuumSoft.Instrumentation.Windows.Forms.IndicatorWidget
            Me.myInstrumentPanel = New System.Windows.Forms.Panel
            Me.myInstrumentPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            'IndicatorWidget1
            '
            Me.IndicatorWidget1.Cursor = System.Windows.Forms.Cursors.Default
            Me.IndicatorWidget1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.IndicatorWidget1.HideFocusRectangle = True
            Me.IndicatorWidget1.InstrumentationStream = resources.GetString("IndicatorWidget1.InstrumentationStream")
            Me.IndicatorWidget1.Location = New System.Drawing.Point(0, 0)
            Me.IndicatorWidget1.Maximum = 0
            Me.IndicatorWidget1.Minimum = 0
            Me.IndicatorWidget1.Name = "IndicatorWidget1"
            Me.IndicatorWidget1.Size = New System.Drawing.Size(232, 233)
            Me.IndicatorWidget1.TabIndex = 4
            '
            'myInstrumentPanel
            '
            Me.myInstrumentPanel.Controls.Add(Me.IndicatorWidget1)
            Me.myInstrumentPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me.myInstrumentPanel.Location = New System.Drawing.Point(0, 0)
            Me.myInstrumentPanel.Name = "myInstrumentPanel"
            Me.myInstrumentPanel.Size = New System.Drawing.Size(232, 233)
            Me.myInstrumentPanel.TabIndex = 5
            '
            'BSMonitorTankA
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.myInstrumentPanel)
            Me.Name = "BSMonitorTankA"
            Me.Controls.SetChildIndex(Me.myInstrumentPanel, 0)
            Me.myInstrumentPanel.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents IndicatorWidget1 As PerpetuumSoft.Instrumentation.Windows.Forms.IndicatorWidget
        Friend WithEvents myInstrumentPanel As System.Windows.Forms.Panel

    End Class

End Namespace
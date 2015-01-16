Namespace Biosystems.Ax00.Controls.UserControls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BSMonitorPercentBar
        Inherits BSMonitorControlBase


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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BSMonitorPercentBar))
            Me.IndicatorWidget1 = New PerpetuumSoft.Instrumentation.Windows.Forms.IndicatorWidget
            Me.InstrumentPanel = New System.Windows.Forms.Panel
            Me.InstrumentPanel.SuspendLayout()
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
            Me.IndicatorWidget1.Size = New System.Drawing.Size(926, 285)
            Me.IndicatorWidget1.TabIndex = 0
            '
            'InstrumentPanel
            '
            Me.InstrumentPanel.Controls.Add(Me.IndicatorWidget1)
            Me.InstrumentPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me.InstrumentPanel.Location = New System.Drawing.Point(0, 0)
            Me.InstrumentPanel.Name = "InstrumentPanel"
            Me.InstrumentPanel.Size = New System.Drawing.Size(926, 285)
            Me.InstrumentPanel.TabIndex = 4
            '
            'BSMonitorPercentBar
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.Transparent
            Me.Controls.Add(Me.InstrumentPanel)
            Me.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status._ON
            Me.Name = "BSMonitorPercentBar"
            Me.Size = New System.Drawing.Size(926, 335)
            Me.Controls.SetChildIndex(Me.InstrumentPanel, 0)
            Me.InstrumentPanel.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents IndicatorWidget1 As PerpetuumSoft.Instrumentation.Windows.Forms.IndicatorWidget
        Friend WithEvents InstrumentPanel As System.Windows.Forms.Panel

        Protected Friend Overrides Sub RefreshControl()
            UpdateContentsSize(InstrumentPanel)
        End Sub
    End Class
End Namespace

Namespace Biosystems.Ax00.Controls.UserControls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BsScadaWashControl
        Inherits BsScadaControl

        'UserControl reemplaza a Dispose para limpiar la lista de componentes.
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

        'Requerido por el Diseñador de Windows Forms
        Private components As System.ComponentModel.IContainer

        'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
        'Se puede modificar usando el Diseñador de Windows Forms.  
        'No lo modifique con el editor de código.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BsScadaWashControl))
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
            Me.ControlWidget.Size = New System.Drawing.Size(100, 150)
            Me.ControlWidget.TabIndex = 6
            '
            'ActuatorWidget
            '
            Me.ActuatorWidget.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ActuatorWidget.Cursor = System.Windows.Forms.Cursors.Default
            Me.ActuatorWidget.HideFocusRectangle = True
            Me.ActuatorWidget.InstrumentationStream = resources.GetString("ActuatorWidget.InstrumentationStream")
            Me.ActuatorWidget.Location = New System.Drawing.Point(0, 0)
            Me.ActuatorWidget.Margin = New System.Windows.Forms.Padding(0)
            Me.ActuatorWidget.Maximum = 0
            Me.ActuatorWidget.Minimum = 0
            Me.ActuatorWidget.Name = "ActuatorWidget"
            Me.ActuatorWidget.Size = New System.Drawing.Size(100, 30)
            Me.ActuatorWidget.TabIndex = 7
            Me.ActuatorWidget.Visible = False
            '
            'BsScadaWashControl
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.ControlWidget)
            Me.Controls.Add(Me.ActuatorWidget)
            Me.Name = "BsScadaWashControl"
            Me.Size = New System.Drawing.Size(100, 150)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents ActuatorWidget As PerpetuumSoft.Instrumentation.Windows.Forms.IndicatorWidget
        Friend WithEvents ControlWidget As PerpetuumSoft.Instrumentation.Windows.Forms.IndicatorWidget
    End Class
End Namespace
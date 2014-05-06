Namespace Biosystems.Ax00.Controls.UserControls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    <Drawing.ToolboxBitmap(GetType(System.Windows.Forms.DateTimePicker))> _
    Partial Class BSDateTimePicker
        Inherits System.Windows.Forms.DateTimePicker

        'Control overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Control Designer
        Private components As System.ComponentModel.IContainer

        ' NOTE: The following procedure is required by the Component Designer
        ' It can be modified using the Component Designer.  Do not modify it
        ' using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.SuspendLayout()
            '
            'BSDateTimePicker
            '
            Me.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
            Me.CalendarForeColor = System.Drawing.Color.Black
            Me.CalendarMonthBackground = System.Drawing.Color.White
            Me.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
            Me.CalendarTitleForeColor = System.Drawing.Color.Black
            Me.CalendarTrailingForeColor = System.Drawing.Color.Silver
            Me.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
            Me.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
            Me.ResumeLayout(False)

        End Sub

    End Class
End Namespace

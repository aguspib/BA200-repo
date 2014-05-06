Namespace Biosystems.Ax00.Controls.UserControls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    <Drawing.ToolboxBitmap(GetType(System.Windows.Forms.Button))> _
    Partial Class BSButton
        Inherits System.Windows.Forms.Button

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
            'BSButton
            '
            Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.Size = New System.Drawing.Size(32, 32)
            Me.ResumeLayout(False)

        End Sub

    End Class
End Namespace

Namespace Biosystems.Ax00.Controls.UserControls
    Public Class BSTreeView

        Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(pe)

            'Add your custom paint code here
        End Sub

        'RH 16/02/2012
        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            MyBase.WndProc(m)

            'http://msdn.microsoft.com/en-us/library/windows/desktop/bb787577(v=vs.85).aspx

            'If scrolling (m.Msg = &H115) and scroll stops (m.WParam = 8),
            'redraw the control to fix a native bug.
            'If m.Msg = &H115 AndAlso m.WParam = 8 Then
            '    Me.Invalidate()
            'End If

            'Solution 2: If scrolling (m.Msg = &H115), redraw the control to fix a native bug.
            If m.Msg = &H115 Then
                'Me.FindForm().ParentForm.Text = m.WParam.ToString() 'Just for watching the values when debugging
                Me.Invalidate()
            End If

        End Sub

    End Class

End Namespace
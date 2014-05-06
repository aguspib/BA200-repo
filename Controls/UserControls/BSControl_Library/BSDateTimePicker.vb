Namespace Biosystems.Ax00.Controls.UserControls
    Public Class BSDateTimePicker

        Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(pe)

            'Add your custom paint code here
        End Sub

        'JB 12/07/2012 -Verify the MaxDate and MinDate are in correct Range
        Public Shadows Property MinDate() As Date
            Get
                Return MyBase.MinDate
            End Get
            Set(ByVal value As Date)
                Dim oldCheck As Boolean = MyBase.Checked 'JB 28/09/2012: To keep the check
                If value > MaxDate Then
                    MaxDate = value
                End If
                MyBase.MinDate = value
                MyBase.Checked = oldCheck 'JB 28/09/2012: Keep the check
            End Set
        End Property

        Public Shadows Property MaxDate() As Date
            Get
                Return MyBase.MaxDate
            End Get
            Set(ByVal value As Date)
                Dim oldCheck As Boolean = MyBase.Checked 'JB 28/09/2012: To keep the check
                If value < MinDate Then
                    MinDate = value
                End If
                MyBase.MaxDate = value
                MyBase.Checked = oldCheck 'JB 28/09/2012: Keep the check
            End Set
        End Property
        'JB 12/07/2012 -END
    End Class
End Namespace
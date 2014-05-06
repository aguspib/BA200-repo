Namespace Biosystems.Ax00.Controls.UserControls
    Public Class BSNumericUpDown
        'Attribute to indicate that is Required 
        Private MandatoryAttribute As Boolean

        Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(pe)

            'Add your custom paint code here
        End Sub

        'Public Shadows Property Value() As Decimal
        '    Get
        '        Return MyBase.Value
        '    End Get
        '    Set(ByVal value As Decimal)
        '        If MyBase.Value <> value Then
        '            Dim e As New NumericUpDownExValueChangingEventArgs(MyBase.Value, value)

        '            Me.OnValueChanging(e)

        '            If Not e.Cancel Then
        '                MyBase.Value = value
        '            End If
        '        End If
        '    End Set
        'End Property


        'Protected Overrides Sub OnValueChanged(ByVal e As System.EventArgs)
        '    'Raise the ValueChanged event through the base class.
        '    MyBase.OnValueChanged(e)
        '    'Explicitly raise the ValueChanging event.
        '    RaiseEvent ValueChanging(Me, New NumericUpDownExValueChangingEventArgs(Me.oldValue, Me.Value))

        '    'Update the old value for ready for the next event.
        '    Me.oldValue = Me.Value

        'End Sub

    End Class


    'Public Class NumericUpDownExValueChangingEventArgs
    '    Inherits System.ComponentModel.CancelEventArgs

    '    Private _currentValue As Decimal
    '    Private _newValue As Decimal
    '    Private _oldValue As Decimal

    '    Public ReadOnly Property CurrentValue() As Decimal
    '        Get
    '            Return Me._currentValue
    '        End Get
    '    End Property

    '    Public ReadOnly Property OldValue() As Decimal
    '        Get
    '            Return Me._oldValue
    '        End Get
    '    End Property


    '    Public ReadOnly Property NewValue() As Decimal
    '        Get
    '            Return Me._newValue
    '        End Get
    '    End Property

    '    Public Sub New(ByVal currentValue As Decimal, ByVal newValue As Decimal)
    '        Me._currentValue = currentValue
    '        Me._newValue = newValue
    '        Me._oldValue = OldValue
    '    End Sub

    'End Class

End Namespace
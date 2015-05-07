Namespace Biosystems.Ax00.CC

    ''' <summary>
    ''' This structure represents a minimum and maximum pair of comparable elements. Notice it's imutable.
    ''' </summary>
    ''' <typeparam name="TComparable">The comparable parameter type</typeparam>
    ''' <remarks></remarks>
    Public Structure Range(Of TComparable As {IComparable})
        Public ReadOnly Minimum As TComparable
        Public ReadOnly Maximum As TComparable

        ''' <summary>
        ''' This is the default structure constructor. 
        ''' </summary>
        ''' <param name="minimum"></param>
        ''' <param name="maximum"></param>
        ''' <remarks></remarks>
        Sub New(minimum As TComparable, maximum As TComparable)
            If minimum.CompareTo(maximum) <= 0 Then
                Me.Minimum = minimum
                Me.Maximum = maximum
            Else
                Throw New Exception("Range values are not valid. Minimum can't be greater than maximum")
            End If
        End Sub

        ''' <summary>
        ''' Tell if a given value is between minimum and maximum range.Integer This is an inclusive check.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsValueOnRange(value As TComparable) As Boolean
            If value.CompareTo(Minimum) >= 0 AndAlso value.CompareTo(Maximum) <= 0 Then
                Return True
            Else
                Return False
            End If
        End Function

    End Structure
End Namespace
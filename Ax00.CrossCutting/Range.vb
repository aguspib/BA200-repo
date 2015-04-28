Namespace Biosystems.Ax00.CC


    ''' <summary>
    ''' This structure represents a minimum and maximum pair of comparable elements
    ''' </summary>
    ''' <typeparam name="TComparable">The comparable parameter type</typeparam>
    ''' <remarks></remarks>
    Public Structure Range(Of TComparable As {IComparable})
        Public minimum As TComparable
        Public maximum As TComparable

        Sub New(minimum As TComparable, maximum As TComparable)
            If minimum.CompareTo(maximum) <= 0 Then
                Me.minimum = minimum
                Me.maximum = maximum
            Else
                Throw New Exception("Range values are not valid. Minimum can't be greater than maximum")
            End If
        End Sub

        Public Function IsValueOnRange(value As TComparable) As Boolean
            If value.CompareTo(minimum) >= 0 AndAlso value.CompareTo(maximum) <= 0 Then
                Return True
            Else
                Return False
            End If
        End Function

    End Structure
End Namespace
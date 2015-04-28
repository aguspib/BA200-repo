Namespace Biosystems.Ax00.CC

    Public Class RangedCollection(Of T)
        Implements IEnumerable(Of T)

        Dim internalList As New List(Of T)
        Public ReadOnly WorkingRange As Range(Of Integer)

        Public Property AllowOutOfRange As Boolean = True

        Sub New(workingRange As Range(Of Integer))
            Me.WorkingRange = workingRange
        End Sub

        Sub New(minimum As Integer, maximum As Integer)
            Me.New(New Range(Of Integer)(minimum, maximum))
        End Sub

        Default Property Item(index As Integer) As T
            Get
                index = convertIndex(index)
                If index < 0 Then
                    Throw New IndexOutOfRangeException("Index can't be smaller than minimum range")
                ElseIf index >= internalList.Count Then
                    Throw New IndexOutOfRangeException("Index out of bounds")
                End If
                Return internalList(index)
            End Get
            Set(value As T)
                index = convertIndex(index)
                If index < 0 Then
                    Throw New IndexOutOfRangeException("Index can't be smaller than minimum range")
                ElseIf index >= internalList.Count Then
                    Throw New IndexOutOfRangeException("Index out of bounds")
                End If
                internalList(index) = value
            End Set
        End Property

        Function Add(item As T) As Boolean
            If (Not AllowOutOfRange) AndAlso (GetMaxIndex() < internalList.Count) Then
                Return False
            Else
                internalList.Add(item)
                Return True
            End If
        End Function

        Function RemoveLast() As T
            Dim last = internalList.Last()
            internalList.RemoveAt(internalList.Count - 1)
            Return last
        End Function

        Sub RemoveOutOfRangeItems()
            Dim maxIndex = GetMaxIndex()
            While internalList.Count - 1 > maxIndex
                RemoveLast()
            End While
        End Sub

        Function RemoveFirst() As T
            Dim first = internalList.First()
            internalList.RemoveAt(0)
            Return first
        End Function

        Private Function convertIndex(index As Integer) As Integer
            Return index - WorkingRange.minimum
        End Function

        Private Function GetMaxIndex() As Integer
            Return (WorkingRange.maximum - WorkingRange.minimum)
        End Function

        Public Function GetTypedEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Return internalList.GetEnumerator
        End Function

        Public Function GetUntypedEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return internalList.GetEnumerator
        End Function
    End Class
End Namespace

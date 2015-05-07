Namespace Biosystems.Ax00.CC

    Public Class RangedCollection(Of T)
        Implements IEnumerable(Of T)
        ''' <summary>
        ''' This represents the minimum and maximum indexes this collection can have.
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Range As Range(Of Integer)

        ''' <summary>
        ''' Enable this setting to allow the collection to grow past its maximum.
        ''' <para>If at any point, we need to shrink the collection to its actual range, we can call the RemoveOutOfRangeItems method.</para>
        ''' </summary>
        ''' <value></value>
        ''' <returns>True if the collection allows for items to be added past its maximum range.</returns>
        ''' <remarks></remarks>
        Public Property AllowOutOfRange As Boolean = True

        Public Sub New(workingRange As Range(Of Integer))
            Range = workingRange
        End Sub

        Public Sub New(minimum As Integer, maximum As Integer)
            Me.New(New Range(Of Integer)(minimum, maximum))
        End Sub

        Public Sub Preallocate()
            While _internalList.Count < GetZeroBasedMaxIndex() + 1
                _internalList.Add(Nothing)
            End While
        End Sub

        ''' <summary>
        ''' This allows any item into the collection to be accessed by its index. <para>Notice that the index starts in Range.Minimum </para>
        ''' </summary>
        ''' <param name="index"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public _
        Property Item(index As Integer) As T
            Get
                index = ConvertToZeroBasedIndex(index)
                If index < 0 Then
                    Throw New IndexOutOfRangeException("Index can't be smaller than minimum range")
                ElseIf index >= _internalList.Count Then
                    Throw New IndexOutOfRangeException("Index out of bounds")
                End If
                Return _internalList(index)
            End Get
            Set(value As T)
                index = ConvertToZeroBasedIndex(index)
                If index < 0 Then
                    Throw New IndexOutOfRangeException("Index can't be smaller than minimum range")
                ElseIf index >= _internalList.Count Then
                    Throw New IndexOutOfRangeException("Index out of bounds")
                End If
                _internalList(index) = value
            End Set
        End Property

        ''' <summary>
        ''' Appends an item at the end of this collection
        ''' </summary>
        ''' <param name="item">The item to be added</param>
        ''' <returns>Returns TRUE if the item was added successfully. Otherwise returns false.</returns>
        ''' <remarks></remarks>
        Public Function Add(item As T) As Boolean
            If (Not AllowOutOfRange) AndAlso (GetZeroBasedMaxIndex() < _internalList.Count) Then
                Return False
            Else
                _internalList.Add(item)
                Return True
            End If
        End Function

        ''' <summary>
        ''' Removes and returns the last item in the collection.
        ''' </summary>
        ''' <returns>The item removed, or null/nothing if there are no items to remove.</returns>
        ''' <remarks></remarks>
        Public Function RemoveLast() As T
            If _internalList.Any = False Then Return Nothing
            Dim last = _internalList.Last()
            _internalList.RemoveAt(_internalList.Count - 1)
            Return last
        End Function

        ''' <summary>
        ''' This method will remove all items that have an index greater than the Range.Maximim value.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub RemoveOutOfRangeItems()
            Dim maxIndex = GetZeroBasedMaxIndex()
            While _internalList.Count - 1 > maxIndex
                RemoveLast()
            End While
        End Sub

        ''' <summary>
        ''' Removes and returns the first item in the collection and reasigns all other items idexes accordingly, so all items in the collection see their index decreased by 1
        ''' </summary>
        ''' <returns>The item removed, if any. Otherwise returns null/nothing</returns>
        ''' <remarks></remarks>
        Public Function RemoveFirst() As T
            If _internalList.Any = False Then Return Nothing
            Dim first = _internalList.First()
            _internalList.RemoveAt(0)
            Return first
        End Function

        ''' <summary>
        ''' Sort the list items using the given comparer
        ''' </summary>
        ''' <param name="comparer"></param>
        ''' <remarks></remarks>
        Public Sub Sort(comparer As IComparer(Of T))
            _internalList.Sort(comparer)
        End Sub

        ''' <summary>
        ''' Sort the list items using the default comparer. Notice that this method will throw an exception if list items are not comparable.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Sort()
            _internalList.Sort()
        End Sub

#Region "Internal zero-based indexes conversion helpers"

        ReadOnly _internalList As New List(Of T)

        Private Function ConvertToZeroBasedIndex(index As Integer) As Integer
            Return index - Range.minimum
        End Function

        Private Function GetZeroBasedMaxIndex() As Integer
            Return ConvertToZeroBasedIndex(Range.maximum) '(Range.maximum - Range.minimum)
        End Function
#End Region

#Region "LINQ supprt implementation"
        Public Function GetTypedEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Return _internalList.GetEnumerator
        End Function

        Public Function GetUntypedEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _internalList.GetEnumerator
        End Function
#End Region
    End Class

End Namespace

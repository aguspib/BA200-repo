Imports Telerik.JustMock.AutoMock.Ninject.Planning.Bindings.Resolvers

Public Class Context

    ''' <summary>
    ''' We will define a context range from [-Persistence to + Persitence] we'll considere index 0 the current reagent, that is, the context "center"
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly ContextRange As Range(Of Integer)

    Public ReadOnly Reagents As RangedList(Of Reagent)

    'TODO: Define which parameters need to be considered for maximum and minimum range values
    Sub New(persistence As Integer, predilutionCycles As Integer)
        ContextRange = New Range(Of Integer)(-persistence, predilutionCycles)

    End Sub



End Class


Public Class Reagent

End Class

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

    Function IsValueOnRange(value As TComparable) As Boolean
        If value.CompareTo(minimum) >= 0 AndAlso value.CompareTo(maximum) <= 0 Then
            Return True
        Else
            Return False
        End If
    End Function

End Structure

Public Class RangedList(Of T)
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
        While internalList.Count > maxIndex + 1
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

    Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
        Return New Enumerator(Of T)(Me)
    End Function

    Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
        Return New Enumerator(Of T)(Me)
    End Function

    Public Class Enumerator(Of TEnumerator)
        Implements IEnumerator(Of TEnumerator)
        Sub New(owner As RangedList(Of TEnumerator))
            parent = owner
            index = parent.WorkingRange.minimum
        End Sub
        Dim index As Integer = 0
        Dim parent As RangedList(Of TEnumerator)

        Public ReadOnly Property Current As TEnumerator Implements IEnumerator(Of TEnumerator).Current
            Get
                Return parent(index)
            End Get
        End Property

        Public ReadOnly Property CurrentObj As Object Implements IEnumerator.Current
            Get
                Return Current
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            If index < parent.internalList.Count Then
                index += 1
                Return True
            Else
                Return False
            End If
        End Function

        Public Sub Reset() Implements IEnumerator.Reset
            index = parent.WorkingRange.minimum
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Class


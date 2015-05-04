﻿Imports Biosystems.Ax00.CC

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class ContextStep
        Implements IEnumerable(Of IReagentDispensing)



        Sub New(dispensingPerStep As Integer)
            'ReDim _dispensings(dispensingPerStep - 1)
            _dispensings = New RangedCollection(Of IReagentDispensing)(1, dispensingPerStep)
            _dispensings.Preallocate()
        End Sub

        Public ReadOnly Property DispensingPerStep As Integer
            Get
                Return _dispensings.Count
            End Get
        End Property

        Default Public Property Dispensing(index As Integer) As IReagentDispensing
            Get
                Return _dispensings(index)
            End Get
            Set(value As IReagentDispensing)
                _dispensings(index) = value
            End Set
        End Property


        Private ReadOnly _dispensings As RangedCollection(Of IReagentDispensing)  'R1 are dispenses(0), R2 are dispenses(1), etc.

        Public Function GetEnumerator() As IEnumerator(Of IReagentDispensing) Implements IEnumerable(Of IReagentDispensing).GetEnumerator
            Return _dispensings.GetTypedEnumerator()
        End Function

        Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _dispensings.GetUntypedEnumerator()
        End Function
    End Class

End Namespace
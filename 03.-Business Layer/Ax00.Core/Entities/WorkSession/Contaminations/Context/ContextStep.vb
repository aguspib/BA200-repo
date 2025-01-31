﻿
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.WorkSession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.CC

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context
    Public Class ContextStep
        Implements IContextStep

        Sub New(dispensingPerStep As Integer)
            'ReDim _dispensings(dispensingPerStep - 1)
            _dispensings = New RangedCollection(Of IDispensing)(1, dispensingPerStep)
            _dispensings.Preallocate()
        End Sub

        Public ReadOnly Property DispensingPerStep As Integer Implements IContextStep.DispensingPerStep
            Get
                Return _dispensings.Count
            End Get
        End Property

        Default Public Property Dispensing(index As Integer) As IDispensing Implements IContextStep.Dispensing
            Get
                Return _dispensings(index)
            End Get
            Set(value As IDispensing)
                _dispensings(index) = value
            End Set
        End Property


        Private ReadOnly _dispensings As RangedCollection(Of IDispensing)  'R1 are dispenses(0), R2 are dispenses(1), etc.

        Public Function GetEnumerator() As IEnumerator(Of IDispensing) Implements IEnumerable(Of IDispensing).GetEnumerator
            Return _dispensings.GetTypedEnumerator()
        End Function

        Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _dispensings.GetUntypedEnumerator()
        End Function
    End Class

End Namespace
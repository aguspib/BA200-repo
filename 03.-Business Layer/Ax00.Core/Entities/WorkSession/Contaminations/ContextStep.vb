Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class ContextStep

        Sub New(dispensingPerStep As Integer)
            ReDim _dispensings(dispensingPerStep)
        End Sub

        Public ReadOnly Property DispensingPerStep As Integer
            Get
                Return _dispensings.Length
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

        Private ReadOnly _dispensings() As IReagentDispensing  'R1 are dispenses(0), R2 are dispenses(1), etc.

    End Class

End Namespace
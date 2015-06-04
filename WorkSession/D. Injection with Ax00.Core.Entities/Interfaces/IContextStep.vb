Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces


Namespace Biosystems.Ax00.Core.Entities.Worksession.Interfaces
    Public Interface IContextStep
        Inherits IEnumerable(Of IDispensing)
        ReadOnly Property DispensingPerStep As Integer
        Default Property Dispensing(index As Integer) As IDispensing
    End Interface
End Namespace
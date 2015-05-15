Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces


Namespace Biosystems.Ax00.Core.Entities.Worksession.Interfaces
    Public Interface IContextStep
        Inherits IEnumerable(Of IReagentDispensing)
        ReadOnly Property DispensingPerStep As Integer
        Default Property Dispensing(index As Integer) As IReagentDispensing
    End Interface
End Namespace
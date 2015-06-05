Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.Worksession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context

    Public Class ActionRequiredForDispensing
        Implements IActionRequiredForDispensing

        Public Property Action As IContaminationsAction.RequiredAction Implements IActionRequiredForDispensing.Action
        Public Property InvolvedWashes As New List(Of IWashingDescription) Implements IActionRequiredForDispensing.InvolvedWashes

    End Class
End Namespace

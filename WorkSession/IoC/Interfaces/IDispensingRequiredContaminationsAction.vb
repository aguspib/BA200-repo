Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces
    Public Interface IContaminationsAction
        Property Action As RequiredAction
        Property InvolvedWash As IWashingDescription

        Enum RequiredAction
            GoAhead
            Wash
            Skip
            RemoveRequiredWashing
        End Enum

    End Interface

End Namespace


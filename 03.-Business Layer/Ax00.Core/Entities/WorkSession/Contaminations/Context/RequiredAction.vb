Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context
    Public Class RequiredAction
        Implements IContaminationsAction

        Public Property Action As IContaminationsAction.RequiredAction Implements IContaminationsAction.Action

        Public Property InvolvedWash As Core.Interfaces.IWashingDescription Implements IContaminationsAction.InvolvedWash

    End Class
End Namespace
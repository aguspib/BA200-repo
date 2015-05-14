Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class ContaminationsAction
        Implements IContaminationsAction


        Public Property Action As IContaminationsAction.RequiredAction Implements IContaminationsAction.Action

        Public Property InvolvedWash As Biosystems.Ax00.Core.Interfaces.IWashingDescription Implements IContaminationsAction.InvolvedWash

    End Class
End Namespace
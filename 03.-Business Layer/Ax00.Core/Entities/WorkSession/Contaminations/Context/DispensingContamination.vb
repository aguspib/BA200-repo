Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context

    Public Class DispensingContamination
        Implements IDispensingContaminationDescription

        Public Property ContaminedReagent As Integer Implements IDispensingContaminationDescription.ContaminedReagent

        Public Property RequiredWashing As IWashingDescription Implements IDispensingContaminationDescription.RequiredWashing
    End Class


End Namespace
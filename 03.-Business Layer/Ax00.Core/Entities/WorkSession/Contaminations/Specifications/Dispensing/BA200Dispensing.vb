Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context
Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Specifications.Dispensing
    Public Class BA200Dispensing
        Inherits Ax00DispensingBase

        Protected Overrides Function ReagentRequiresWashingOrSkip(ByVal scope As Integer, ByVal dispensingBeingSent As IDispensing, ByVal reagentNumber As Integer) As IContaminationsAction

            Dim result = MyBase.ReagentRequiresWashingOrSkip(scope, dispensingBeingSent, reagentNumber)

            'If (result.Action <> IContaminationsAction.RequiredAction.Wash AndAlso
            '    scope = -1 AndAlso reagentNumber = 1 AndAlso
            '    Contamines IsNot Nothing AndAlso
            '    Contamines.Any AndAlso
            '    AnalysisMode = Global.AnalysisMode.BiReactive AndAlso
            '    dispensingBeingSent.AnalysisMode = Global.AnalysisMode.MonoReactive) Then

            '    result.Action = IContaminationsAction.RequiredAction.Wash
            '    result.InvolvedWash = New WashingDescription(1, Context.WashingDescription.RegularWaterWashingID)
            'End If

            Return result

        End Function
    End Class
End Namespace
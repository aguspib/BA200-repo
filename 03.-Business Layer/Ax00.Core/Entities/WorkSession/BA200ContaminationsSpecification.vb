Imports Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
Imports Biosystems.Ax00.CC

Public Class BA200ContaminationsSpecification
    Implements IAnalyzerContaminationsSpecification

    Dim _range As New Range(Of Integer)(-2, 7)
    Public Property ContaminationsContextRange As Biosystems.Ax00.CC.Range(Of Integer) Implements IAnalyzerContaminationsSpecification.ContaminationsContextRange
        Get
            Return _range
        End Get
        Set(value As Biosystems.Ax00.CC.Range(Of Integer))
            _range = value
        End Set
    End Property

    Dim _dispensesPerStep As Integer = 2
    Public Property DispensesPerStep As Integer Implements IAnalyzerContaminationsSpecification.DispensesPerStep
        Get
            Return _dispensesPerStep
        End Get
        Set(value As Integer)
            _dispensesPerStep = value
        End Set
    End Property


    Public Function DispensingFactory() As IReagentDispensing Implements IAnalyzerContaminationsSpecification.DispensingFactory
        Return New BA200ReagentDispensing
    End Function
End Class

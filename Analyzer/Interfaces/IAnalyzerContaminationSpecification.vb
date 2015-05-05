Imports Biosystems.Ax00.CC
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces

Namespace Biosystems.Ax00.Core.Entities.Worksession.Interfaces
    Public Interface IAnalyzerContaminationsSpecification
        ''' <summary>
        ''' This represents the amount of dispenses per step (that is, how many reagents are dispenses in a running cycle. In BA200 and BA400 that is always 2, R1 and R2)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property DispensesPerStep As Integer

        ''' <summary>
        ''' This represents the range fo steps or cycles that have to be examined in order to calculate contaminations.
        ''' if 0 is current "cycle", this range usually goes from -2 to + persistence in the worst case.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property ContaminationsContextRange As Range(Of Integer)

        ''' <summary>
        ''' This method provides Dispensing instances. Those instances will be responsible to return the contamination they generate when they're placed in the reactions rotor
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function CreateDispensing() As IReagentDispensing

        Property AdditionalPredilutionSteps As Integer

    End Interface
End Namespace

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces
Imports Biosystems.Ax00.Core.Entities.Worksession.Interfaces
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities.Worksession.Contaminations.Interfaces

    Public Interface IDispensing


        'Property ReagentNumber As Integer   '1 for R1, 2 for R2, etc. If any non-cycle based R3 or whatever is added, it should be informed here!

        Property R1ReagentID As Integer

        Property WashingID As Integer

        Property ExecutionID As Integer

        Property KindOfLiquid As KindOfDispensedLiquid

        Property SampleClass As String

        Property OrderTestID As Integer

        Property TestID As Integer

        ReadOnly Property AnalysisMode As Integer 'OptimizationPolicyApplier.AnalysisMode

        ReadOnly Property Contamines As Dictionary(Of Integer, IDispensingContaminationDescription)

        Function RequiredActionForDispensing(Reagent As IDispensing, scope As Integer, ReagentNumber As Integer) As IContaminationsAction

        ''' <summary>
        ''' This indicates the number of cycles that will take between the dispense is programmed and the analyzer actually makes the dispensing. 
        ''' <para>On analyzers with automatic predilution this will contain the number of cycles that take from the analyzer to actually dispense the reagent in the prediluted well</para>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property DelayCyclesForDispensing As Integer

        Sub FillDispense(analyzerContaminationsSpecification As IAnalyzerContaminationsSpecification, ByVal row As ExecutionsDS.twksWSExecutionsRow)

        Enum KindOfDispensedLiquid
            Reagent
            Ise
            Dummy
            Washing
        End Enum

    End Interface



End Namespace
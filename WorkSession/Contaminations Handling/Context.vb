Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.CC

Public Class ContaminationsContext

    Public ReadOnly Reagents As RangedCollection(Of ContextStep)

    Sub New(indexesRange As Range(Of Integer), dispensesPerStep As Integer)
        Reagents = New RangedCollection(Of ContextStep)(indexesRange)
        Reagents.AllowOutOfRange = False
        For i = indexesRange.minimum To indexesRange.maximum
            Reagents(i) = New ContextStep(dispensesPerStep) 'Cantidad máxima de reactivos que se pueden dispensar por ciclo
        Next
    End Sub

End Class


Public Class ContextStep

    Public ReadOnly DispensingPerStep As Integer = 2

    Sub New(dispensingPerStep As Integer)
        Me.DispensingPerStep = dispensingPerStep
        ReDim _dispensings(dispensingPerStep)
    End Sub

    Default Public Property Dispensing(index As Integer) As ReagentDispensing
        Get
            Return _dispensings(index)
        End Get
        Set(value As ReagentDispensing)
            _dispensings(index) = value
        End Set
    End Property

    Private ReadOnly _dispensings() As ReagentDispensing  'R1 are dispenses(0), R2 are dispenses(1), etc.

End Class


Public Class ReagentDispensing
    Public Property Dispensing As Integer   '1 for R1, 2 for R2, etc. If any non-cycle based R3 or whatever is added, it should be informed here!
    Public Property TechniqueID As Integer  'ID of the associated technique
    Public Property R1ReagentID As Integer
    Public Property AnalysisMode As OptimizationPolicyApplier.AnalysisMode
    Public Contamines As Dictionary(Of Integer, ContaminationDescription)
End Class

Public Class ContaminationDescription
    Public ContaminedTechnique As Integer
    Public RequiredWashing As WashingDescription
End Class

Public Class WashingDescription
    ''' <summary>
    ''' Contamination persistence cycles this washing liquid can clean. Water is 1, washing is 2, etc.
    ''' </summary>
    Public Property CleaningPower As Integer = 1      ' 1 is water
    Public Property WashingSolutionID As Integer = -1 '-1 is water

End Class


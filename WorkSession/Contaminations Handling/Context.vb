Imports Biosystems.Ax00.CC

Public Class ContaminationsContext


    Public ReadOnly Reagents As RangedCollection(Of ReagentDispensing)

    'TODO: Define which parameters need to be considered for maximum and minimum range values
    Sub New(indexesRange As Range(Of Integer))


    End Sub

End Class


Public Class ContextStep
    ''' <summary>
    ''' Instantiates a new ContextStep
    ''' </summary>
    ''' <param name="DispensesByStep"></param>
    ''' <remarks></remarks>
    Sub New(DispensesByStep As Integer)

    End Sub
End Class


Public Class ReagentDispensing

End Class


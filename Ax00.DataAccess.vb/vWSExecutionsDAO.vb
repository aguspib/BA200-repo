

Public Class vWSExecutionsDAO

    Public Sub New()

    End Sub

    Public Function GetInfoExecutions() As vWSExecutionsDS
        Dim result As New vWSExecutionsDS()
        Dim aux As New vWSExecutionsDSTableAdapters.vWSExecutionsSELECTTableAdapter()
        aux.Fill(result.vWSExecutionsSELECT)

        Return result
    End Function

    Public Function GetInfoExecutionByExecutionID(executionID As Integer) As vWSExecutionsDS
        Dim result As New vWSExecutionsDS()
        Dim aux As New vWSExecutionsDSTableAdapters.vWSExecutionsSELECTTableAdapter()
        aux.FillByExecutionID(result.vWSExecutionsSELECT, executionID)

        Return result
    End Function

End Class

﻿

Public Class vWSExecutionsDAO

    Public Sub New()

    End Sub

    Public Function GetInfoExecutions() As vWSExecutionsDS
        Dim result As New vWSExecutionsDS()
        Dim aux As New vWSExecutionsDSTableAdapters.vWSExecutionsSELECTTableAdapter()

        'AJG. If this command throws and exception, it needs to be catched up by the upper layer
        aux.Fill(result.vWSExecutionsSELECT)

        Return result
    End Function

    Public Function GetInfoExecutionByExecutionID(executionID As Integer) As vWSExecutionsDS
        Dim result As New vWSExecutionsDS()
        Dim aux As New vWSExecutionsDSTableAdapters.vWSExecutionsSELECTTableAdapter()

        'AJG. If this command throws and exception, it needs to be catched up by the upper layer
        aux.FillByExecutionID(result.vWSExecutionsSELECT, executionID)

        Return result
    End Function

End Class

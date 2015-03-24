Module Utils
    Public Function GetInstructionBuilt(ByVal analyzerModel As String, ByVal typeInstruction As String, ByVal parametersInstruction As String) As List(Of InstructionParameterTO)
        Dim newInstructionBuilt = New List(Of InstructionParameterTO)
        Dim paramPosition = 1
        Dim parametersSplitted = Split(parametersInstruction, ";")

        newInstructionBuilt.Add(New InstructionParameterTO With {.InstructionType = "A", .ParameterIndex = paramPosition, .ParameterValue = analyzerModel})
        paramPosition += 1
        newInstructionBuilt.Add(New InstructionParameterTO With {.InstructionType = "TYPE", .ParameterIndex = paramPosition, .ParameterValue = typeInstruction})
        paramPosition += 1
        For Each miniSplit In From newParam In parametersSplitted Select Split(newParam, ":")
            newInstructionBuilt.Add(New InstructionParameterTO With {.InstructionType = miniSplit(0), .ParameterIndex = paramPosition, .ParameterValue = miniSplit(1)})
            paramPosition += 1
        Next
        Return newInstructionBuilt
    End Function

    Public Function GetInstructionInString(ByVal constructedInstruction As List(Of InstructionParameterTO)) As String
        Return constructedInstruction.Aggregate("", Function(current, instructionParam) current + (instructionParam.InstructionType + ":" + instructionParam.ParameterValue + ";"))
    End Function
End Module

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations
    Public Class WashingDescription
        ''' <summary>
        ''' Contamination persistence cycles this washing liquid can clean. Water is 1, washing is 2, etc.
        ''' </summary>
        Public ReadOnly CleaningPower As Integer '= 0    '0 means no washing
        Public ReadOnly WashingSolutionID As Integer

        Protected Const NoWashingIDRequired = -1

        Sub New(cleaningPower As Integer, washingSolution As Integer)
            Me.CleaningPower = cleaningPower
            If Me.CleaningPower <> 0 Then
                WashingSolutionID = washingSolution
            ElseIf washingSolution <> -1 Then
                Throw New Exception("Data integrity. Washing solution of 0 power can't have a Washing solution ID")
            Else
                WashingSolutionID = NoWashingIDRequired
            End If
        End Sub

    End Class

End Namespace
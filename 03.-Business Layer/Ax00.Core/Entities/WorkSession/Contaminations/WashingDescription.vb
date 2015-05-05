Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations

    Public Class WashingDescription
        Implements IWashingDescription

        ''' <summary>
        ''' Contamination persistence cycles this washing liquid can clean. Water is 1, washing is 2, etc.
        ''' </summary>
        Public Property WashingStrength As Integer Implements IWashingDescription.WashingStrength '= 0    '0 means no washing
        Public Property WashingSolutionID As String Implements IWashingDescription.WashingSolutionID

        Protected Const NoWashingIDRequired = ""
        Protected Const RegularWaterWashingID = "WATER"

        Public Sub New(cleaningPower As Integer, washingSolution As String)
            Me.WashingStrength = cleaningPower
            If Me.WashingStrength <> 0 Then
                WashingSolutionID = washingSolution
            ElseIf washingSolution <> NoWashingIDRequired Then
                Throw New Exception("Data integrity. Washing solution of 0 power can't have a Washing solution ID")
            Else
                WashingSolutionID = NoWashingIDRequired
            End If
        End Sub

    End Class

End Namespace
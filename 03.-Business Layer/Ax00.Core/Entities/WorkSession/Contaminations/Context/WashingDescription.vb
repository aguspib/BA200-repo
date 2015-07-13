Imports System.Xml.Serialization
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities.WorkSession.Contaminations.Context
    <Serializable()>
    Public Class WashingDescription
        Implements IWashingDescription


        ''' <summary>
        ''' Contamination persistence cycles this washing liquid can clean. Water is 1, washing is 2, etc.
        ''' </summary>
        <XmlAttribute>
        Public Property WashingStrength As Integer Implements IWashingDescription.WashingStrength '= 0    '0 means no washing
        <XmlAttribute>
        Public Property WashingSolutionID As String Implements IWashingDescription.WashingSolutionCode

        Public Const NoWashingIDRequired = ""
        Public Const RegularWaterWashingID = "DISTW"

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

        Public Sub New()

        End Sub

    End Class

End Namespace
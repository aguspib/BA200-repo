Namespace Biosystems.Ax00.Core.Interfaces

    Public Interface IWashingDescription
        ''' <summary>
        ''' Contamination persistence cycles this washing liquid can clean. Water is 1, washing is 2, etc.
        ''' </summary>
        Property WashingStrength As Integer

        Property WashingSolutionCode As String

    End Interface

End Namespace
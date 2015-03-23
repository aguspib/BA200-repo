Namespace Biosystems.Ax00.Global
    Module StatusParameters
        Public Structure SavedRotorStatus
            Public State As RotorStates
            Public LastSaved As DateTime
        End Structure

        Public Enum RotorStates
            None = 0
            CheckedRotorFull = 551
            LeeryRotorFull = 552
        End Enum
    End Module
End Namespace

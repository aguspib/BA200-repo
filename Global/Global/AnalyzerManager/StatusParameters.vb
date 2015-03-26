Namespace Biosystems.Ax00.Global
    Public Module StatusParameters

        Public Structure SavedRotorStatus
            Public Shared IsActive As Boolean
            Public Shared State As RotorStates
            Public Shared LastSaved As DateTime
        End Structure

        Public Enum RotorStates
            None = 0
            CheckedRotorFull = 551
            LeeryRotorFull = 552
        End Enum


        Public IsActive As Boolean
        Public State As RotorStates
        Public LastSaved As DateTime


    End Module
End Namespace

﻿Namespace Biosystems.Ax00.Global
    Public Module StatusParameters

        Public Structure SavedRotorStatus
            Public Shared IsActive As Boolean
            Public Shared State As RotorStates
            Public Shared LastSaved As DateTime
        End Structure

        Public Enum RotorStates
            None = 0
            FBLD_ROTOR_FULL = 551
            UNKNOW_ROTOR_FULL = 552
        End Enum
    End Module
End Namespace

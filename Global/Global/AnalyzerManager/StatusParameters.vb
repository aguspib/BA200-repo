Namespace Biosystems.Ax00.Global
    Public Class StatusParameters

        'Public Class SavedRotorStatus
        Public Shared Property IsActive As Boolean
            Get
                Return _isActive
            End Get
            Set(value As Boolean)
                If _isActive <> value Then
                    _isActive = value
                    'Debug.WriteLine("StatusParameters.IsActive = " & _isActive)
                End If
            End Set
        End Property

        Private Shared _state As RotorStates
        Private Shared _isActive As Boolean

        Public Shared Property State As RotorStates
            Get
                Return _state
            End Get
            Set(value As RotorStates)
                If _state <> value Then
                    _state = value
                    'Debug.WriteLine("StatusParameters.State = " & _state)
                End If
            End Set
        End Property

        Public Shared LastSaved As DateTime
        'End end

        Public Enum RotorStates
            None = 0
            FBLD_ROTOR_FULL = 551
            UNKNOW_ROTOR_FULL = 552
        End Enum
    End Class
End Namespace

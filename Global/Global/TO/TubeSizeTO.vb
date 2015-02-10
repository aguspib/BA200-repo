Option Explicit On
Option Strict On

#Region "Libraries Used by this Class"


#End Region

Namespace Biosystems.Ax00.Global.TO


    Public Class TubeSizeTO


#Region "Attributes"
        Private TubeCodeAttribute As String
        Private FixedTubeNameAttribute As String
        Private RotorTypeAttribute As String
        Private VolumeAttribute As Integer
        Private PositionAttribute As Integer
        Private RingNumberAttribute As Integer
        Private ManualUseFlagAttribute As Boolean

#End Region

#Region "Properties"

        Public Property TubeCode() As String
            Get
                Return TubeCodeAttribute
            End Get

            Set(ByVal Value As String)
                TubeCodeAttribute = Value
            End Set
        End Property

        Public Property FixedTubeName() As String
            Get
                Return FixedTubeNameAttribute
            End Get

            Set(ByVal Value As String)
                FixedTubeNameAttribute = Value
            End Set
        End Property

        Public Property RotorType() As String
            Get
                Return RotorTypeAttribute
            End Get

            Set(ByVal Value As String)
                RotorTypeAttribute = Value
            End Set
        End Property

        Public Property Volume() As Integer
            Get
                Return VolumeAttribute
            End Get

            Set(ByVal Value As Integer)
                VolumeAttribute = Value
            End Set
        End Property

        Public Property Position() As Integer
            Get
                Return PositionAttribute
            End Get

            Set(ByVal Value As Integer)
                PositionAttribute = Value
            End Set
        End Property

        Public Property RingNumber() As Integer
            Get
                Return RingNumberAttribute
            End Get
            Set(ByVal value As Integer)
                RingNumberAttribute = value
            End Set
        End Property

        Public Property ManualUseFlag() As Boolean
            Get
                Return ManualUseFlagAttribute
            End Get
            Set(ByVal value As Boolean)
                ManualUseFlagAttribute = value
            End Set
        End Property
        

#End Region

#Region "Constructor"

        Public Sub New()
            TubeCodeAttribute = ""
            FixedTubeNameAttribute = ""
            RotorTypeAttribute = ""
            VolumeAttribute = 0
            PositionAttribute = 0
            RingNumberAttribute = 0
            ManualUseFlagAttribute = False
        End Sub

#End Region

#Region "Methods"


#End Region


    End Class

End Namespace

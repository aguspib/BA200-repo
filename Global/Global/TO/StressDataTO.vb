Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Global.TO

    Public Class StressDataTO

#Region "Attributes"
        Private StatusAttr As STRESS_STATUS
        Private NumCyclesAttr As Integer
        Private NumCyclesCompletedAttr As Integer
        Private StartDatetimeAttr As DateTime
        Private StartHourAttr As Integer
        Private StartMinuteAttr As Integer
        Private StartSecondAttr As Integer
        Private TypeAttr As STRESS_TYPE
        Private NumResetsAttr As Integer
        Private CyclesResetsAttr As List(Of Long)
        Private NumErrorsAttr As Integer
        Private CodeErrorsAttr As List(Of STRESS_ERRORS)
#End Region

#Region "Properties"
        Public Property Status() As STRESS_STATUS
            Get
                Return StatusAttr
            End Get
            Set(ByVal value As STRESS_STATUS)
                StatusAttr = value
            End Set
        End Property

        Public Property NumCycles() As Integer
            Get
                Return NumCyclesAttr
            End Get
            Set(ByVal value As Integer)
                NumCyclesAttr = value
            End Set
        End Property

        Public Property NumCyclesCompleted() As Integer
            Get
                Return NumCyclesCompletedAttr
            End Get
            Set(ByVal value As Integer)
                NumCyclesCompletedAttr = value
            End Set
        End Property

        Public Property StartHour() As Integer
            Get
                Return StartHourAttr
            End Get
            Set(ByVal value As Integer)
                StartHourAttr = value
            End Set
        End Property

        Public Property StartMinute() As Integer
            Get
                Return StartMinuteAttr
            End Get
            Set(ByVal value As Integer)
                StartMinuteAttr = value
            End Set
        End Property

        Public Property StartSecond() As Integer
            Get
                Return StartSecondAttr
            End Get
            Set(ByVal value As Integer)
                StartSecondAttr = value
            End Set
        End Property

        Public Property StartDatetime() As DateTime
            Get
                Return StartDatetimeAttr
            End Get
            Set(ByVal value As DateTime)
                StartDatetimeAttr = value
            End Set
        End Property

        Public Property Type() As STRESS_TYPE
            Get
                Return TypeAttr
            End Get
            Set(ByVal value As STRESS_TYPE)
                TypeAttr = value
            End Set
        End Property

        Public Property NumResets() As Integer
            Get
                Return NumResetsAttr
            End Get
            Set(ByVal value As Integer)
                NumResetsAttr = value
            End Set
        End Property

        Public Property CyclesResets() As List(Of Long)
            Get
                Return CyclesResetsAttr
            End Get
            Set(ByVal value As List(Of Long))
                CyclesResetsAttr = value
            End Set
        End Property

        Public Property NumErrors() As Integer
            Get
                Return NumErrorsAttr
            End Get
            Set(ByVal value As Integer)
                NumErrorsAttr = value
            End Set
        End Property

        Public Property CodeErrors() As List(Of STRESS_ERRORS)
            Get
                Return CodeErrorsAttr
            End Get
            Set(ByVal value As List(Of STRESS_ERRORS))
                CodeErrorsAttr = value
            End Set
        End Property
#End Region

#Region "Constructor"
        Public Sub New()
            StatusAttr = STRESS_STATUS.NOT_STARTED
            NumCyclesAttr = 0
            NumCyclesCompletedAttr = 0
            StartHourAttr = 0
            StartMinuteAttr = 0
            StartSecondAttr = 0
            StartDatetimeAttr = Nothing
            TypeAttr = STRESS_TYPE.COMPLETE
            NumResetsAttr = 0
            CyclesResetsAttr = New List(Of Long)
            NumErrorsAttr = 0
            CodeErrorsAttr = New List(Of STRESS_ERRORS)
        End Sub
#End Region

    End Class

End Namespace

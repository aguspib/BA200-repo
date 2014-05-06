Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global

    Public Class ISEErrorTO

#Region "public Enums"

        Public Enum ErrorCycles
            None
            Calibration 'CAL x000000
            Sample 'SER x000000>
            Urine 'URN x000000>
            Clean 'CLE x000000>
            PumpCalibration 'PMC x000000>
            BubbleDetCalibration 'BBC x000000>
            SipCycle 'SIP x000000>
            PurgeA 'PGA x000000>
            PurgeB 'PGB x000000>
            DallasReadWrite 'DAL x000000>
            Maintenance 'MAT x000000>
            Communication 'COM x000000>

        End Enum

        Public Enum ISECancelErrorCodes
            None = 0
            S = 1  'Air in Sample/Urine
            A = 2 'Air in Calibrant A
            B = 3  'Air in Calibrant B
            C = 4  'Air in Cleaner
            M = 5  'Air in Segment
            P = 6 'Pump Cal
            F = 7 'No Flow
            D = 8  'Bubble Detector
            R = 9  'Dallas Read
            W = 10  'Dallas Write
            T = 11   'Invalid Command
            N = 12  'Chip missing
        End Enum

        Public Enum ISEResultErrorCodes
            None = 0
            mvOut_CalBSample = 2               'digit #2
            mvOut_CalASample_CalBUrine = 3     'digit #3
            mvNoise_CalBSample = 4            'digit #4
            mvNoise_CalBSample_CalBUrine = 5   'digit #5
            Drift_CalASample = 6             'digit #6
            OutOfSlope_MachineRanges = 7       'digit #7
        End Enum

#End Region

#Region "Attribute"
        Private DigitNumberAttribute As Integer = 0
        Private DigitValueAttr As String = ""
        Private AffectedAttr As String = ""
        'Private MessageAttr As String = ""

        Private ErrorDataStringAttr As String = ""
        Private IsCancelErrorAttr As Boolean = False
        Private ErrorCycleAttr As ErrorCycles = ErrorCycles.None

        'SGM 20/07/2012
        Private CancelErrorCodeAttr As ISECancelErrorCodes = ISECancelErrorCodes.None
        Private ResultErrorCodeAttr As ISEResultErrorCodes = ISEResultErrorCodes.None
        Private ErrorDescAttr As String = ""
        'Private CancelErrorDescAttr As String = ""
        'Private ResultErrorDescAttr As String = ""

#End Region

#Region "Properties"

        Public Property IsCancelError() As Boolean
            Get
                Return IsCancelErrorAttr
            End Get
            Set(ByVal value As Boolean)
                IsCancelErrorAttr = value
            End Set
        End Property

        Public Property ErrorCycle() As ErrorCycles
            Get
                Return ErrorCycleAttr
            End Get
            Set(ByVal value As ErrorCycles)
                ErrorCycleAttr = value
            End Set
        End Property

        Public Property DigitNumber() As Integer
            Get
                Return DigitNumberAttribute
            End Get
            Set(ByVal value As Integer)
                DigitNumberAttribute = value
            End Set
        End Property

        Public Property DigitValue() As String
            Get
                Return DigitValueAttr
            End Get
            Set(ByVal value As String)
                DigitValueAttr = value
            End Set
        End Property

        Public Property Affected() As String
            Get
                Return AffectedAttr
            End Get
            Set(ByVal value As String)
                AffectedAttr = value
            End Set
        End Property

        Public Property ErrorDesc() As String
            Get

                Return ErrorDescAttr
            End Get
            Set(ByVal value As String)
                ErrorDescAttr = value
            End Set
        End Property

        'Public Property Message() As String
        '    Get
        '        Return MessageAttr
        '    End Get
        '    Set(ByVal value As String)
        '        MessageAttr = value
        '    End Set
        'End Property

        Public Property CancelErrorCode() As ISECancelErrorCodes
            Get
                Return CancelErrorCodeAttr
            End Get
            Set(ByVal value As ISECancelErrorCodes)
                CancelErrorCodeAttr = value
            End Set
        End Property

        Public Property ResultErrorCode() As ISEResultErrorCodes
            Get
                Return ResultErrorCodeAttr
            End Get
            Set(ByVal value As ISEResultErrorCodes)
                ResultErrorCodeAttr = value
            End Set
        End Property

        'Public Property CancelErrorDesc() As String
        '    Get

        '        Return CancelErrorDescAttr
        '    End Get
        '    Set(ByVal value As String)
        '        CancelErrorDescAttr = value
        '    End Set
        'End Property

        'Public Property ResultErrorDesc() As String
        '    Get
        '        Return ResultErrorDescAttr
        '    End Get
        '    Set(ByVal value As String)
        '        ResultErrorDescAttr = value
        '    End Set
        'End Property


#End Region

#Region "Constructor"

        Public Sub New()

        End Sub
#End Region

        Public Overrides Function ToString() As String
            Return ErrorDataStringAttr
        End Function

        Public Shared Function FormatAffected(ByVal pAffected As String) As String
            Dim res As String = ""
            Try
                'Cl-, K+, Na+, Li+

                Dim Li As String = "Li+"
                Dim Na As String = "Na+"
                Dim K As String = "K+"
                Dim Cl As String = "Cl-"

                If pAffected.Contains("Cl") Then res &= Cl & ", "
                If pAffected.Contains("K") Then res &= K & ", "
                If pAffected.Contains("Na") Then res &= Na & ", "
                If pAffected.Contains("Li") Then res &= Li & ", "

                If res.EndsWith(", ") Then res = res.Substring(0, res.Length - 2)

            Catch ex As Exception
                res = ""
            End Try
            Return res
        End Function
    End Class
End Namespace
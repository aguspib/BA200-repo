Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global

    Public Class ISEResultTO

#Region "Public Structures"


        Public Structure LiNaKCl
            Public Li As Single
            Public Na As Single
            Public K As Single
            Public Cl As Single

            Public Sub New(ByVal pLi As Single, ByVal pNa As Single, ByVal pK As Single, ByVal pCl As Single)
                Me.Li = pLi
                Me.Na = pNa
                Me.K = pK
                Me.Cl = pCl
            End Sub

            Public ReadOnly Property HasData() As Boolean
                Get
                    ' XBC 03/07/2012
                    'Return ((Me.Li >= 0) Or (Me.Na >= 0) Or (Me.K >= 0) Or (Me.Cl >= 0))
                    Return ((Me.Li > 0) Or (Me.Na > 0) Or (Me.K > 0) Or (Me.Cl > 0))
                End Get
            End Property

        End Structure


        Public Structure PumpCalibrationValues
            Public PumpA As Single
            Public PumpB As Single
            Public PumpW As Single

            Public Sub New(ByVal pA As Single, ByVal pB As Single, ByVal pW As Single)
                Me.PumpA = pA
                Me.PumpB = pB
                Me.PumpW = pW
            End Sub

            Public ReadOnly Property HasData() As Boolean
                Get
                    Return ((Me.PumpA >= 0) And (Me.PumpB >= 0) And (Me.PumpW >= 0))
                End Get
            End Property

        End Structure


        Public Structure BubbleCalibrationValues
            Public ValueA As Single
            Public ValueM As Single
            Public ValueL As Single

            Public Sub New(ByVal pA As Single, ByVal pM As Single, ByVal pL As Single)
                Me.ValueA = pA
                Me.ValueM = pM
                Me.ValueL = pL
            End Sub

            Public ReadOnly Property HasData() As Boolean
                Get
                    Return ((Me.ValueA >= 0) And (Me.ValueM >= 0) And (Me.ValueL >= 0))
                End Get
            End Property

        End Structure
#End Region

#Region "Public Enumerates"
        'SGM 11/01/2012
        Public Enum ISEResultItemTypes
            None
            SerumConcentration
            UrineConcentration
            SerumMilivolts
            UrineMilivolts
            CalAMilivolts
            CalBMilivolts
            Calibration1
            Calibration2
            PumpsCalibration
            BubbleCalibration
            Dallas_SN
            Dallas_Page0
            Dallas_Page1
            Checksum
            Acknoledge
            CancelError
        End Enum

        Public Enum ISEResultTypes
            None
            SwError 'error because of sofware exception
            ComError 'error because of communications error, timeout
            OK
            SER 'serum
            URN 'urine
            CAL 'calibration
            PMC 'pump calibration
            BBC 'bubble calibration
            ISV 'checksum
            AMV 'read mv A
            BMV 'read mv B
            DDT00 'card page 0
            DDT01 'card page 1
            ERC 'error
            SIP 'sipping
        End Enum

        Public Enum ISEResultBlockTypes
            None
            OK
            SER 'serum
            URN 'urine
            CAL 'calibration
            PMC 'pump calibration
            BBC 'bubble calibration
            ISV 'checksum
            AMV 'read mv
            BMV
            DSN
            DDT_00 'card page 0
            DDT_01 'card page 1
            ERC 'error
            SIP 'sipping
            UMV
            SMV
        End Enum

        
#End Region

#Region "Attribute"
        Private ISEResultTypeAttribute As ISEResultTypes
        'Private LithiumAttribute As Single
        'Private SodiumAttribute As Single
        'Private PotassiumAttribute As Single
        'Private ClorineAttribute As Single
        'Private ErrorsStringAttribute As String
        Private ReceivedResultLineAttribute As String 'Use for debug mode reults.

        'SGM 11/01/2012
        Private WorkSessionIDAttr As String
        Private PatientIDAttr As String
        Private ReceivedDatetimeAttr As DateTime
        Private ConcentrationValuesAttr As LiNaKCl
        Private SerumMilivoltsAttr As LiNaKCl
        Private UrineMilivoltsAttr As LiNaKCl
        Private CalibratorAMilivoltsAttr As LiNaKCl
        Private CalibratorBMilivoltsAttr As LiNaKCl
        Private CalibrationResults1Attr As LiNaKCl
        Private CalibrationResults2Attr As LiNaKCl
        Private PumpsCalibrationValuesAttr As PumpCalibrationValues
        Private BubbleDetCalibrationValuesAttr As BubbleCalibrationValues
        Private ChecksumValueAttr As String
        Private DallasSNDataAttr As ISEDallasSNTO
        Private DallasPage00DataAttr As ISEDallasPage00TO
        Private DallasPage01DataAttr As ISEDallasPage01TO
        Private IsCancelErrorAttr As Boolean
        Private ErrorsStringAttr As String
        Private ErrorsAttr As List(Of ISEErrorTO)
        'end 11/01/2012
#End Region

#Region "Properties"

        Public Property ISEResultType() As ISEResultTypes
            Get
                Return ISEResultTypeAttribute
            End Get
            Set(ByVal value As ISEResultTypes)
                ISEResultTypeAttribute = value
            End Set
        End Property

        Public Property IsCancelError() As Boolean
            Get
                Return IsCancelErrorAttr
            End Get
            Set(ByVal value As Boolean)
                IsCancelErrorAttr = value
            End Set
        End Property

        Public Property WorkSessionID() As String
            Get
                Return WorkSessionIDAttr
            End Get
            Set(ByVal value As String)
                WorkSessionIDAttr = value
            End Set
        End Property

        Public Property PatientID() As String
            Get
                Return PatientIDAttr
            End Get
            Set(ByVal value As String)
                PatientIDAttr = value
            End Set
        End Property

        Public Property ReceivedResults() As String
            Get
                Return ReceivedResultLineAttribute
            End Get
            Set(ByVal value As String)
                ReceivedResultLineAttribute = value
            End Set
        End Property

        'SGM 11/01/2012
        Public Property ConcentrationValues() As LiNaKCl
            Get
                Return ConcentrationValuesAttr
            End Get
            Set(ByVal value As LiNaKCl)
                ConcentrationValuesAttr = value
            End Set
        End Property

        Public Property SerumMilivolts() As LiNaKCl
            Get
                Return SerumMilivoltsAttr
            End Get
            Set(ByVal value As LiNaKCl)
                SerumMilivoltsAttr = value
            End Set
        End Property

        Public Property UrineMilivolts() As LiNaKCl
            Get
                Return UrineMilivoltsAttr
            End Get
            Set(ByVal value As LiNaKCl)
                UrineMilivoltsAttr = value
            End Set
        End Property

        Public Property CalibratorAMilivolts() As LiNaKCl
            Get
                Return CalibratorAMilivoltsAttr
            End Get
            Set(ByVal value As LiNaKCl)
                CalibratorAMilivoltsAttr = value
            End Set
        End Property

        Public Property CalibratorBMilivolts() As LiNaKCl
            Get
                Return CalibratorBMilivoltsAttr
            End Get
            Set(ByVal value As LiNaKCl)
                CalibratorBMilivoltsAttr = value
            End Set
        End Property

        Public Property CalibrationResults1() As LiNaKCl
            Get
                Return CalibrationResults1Attr
            End Get
            Set(ByVal value As LiNaKCl)
                CalibrationResults1Attr = value
            End Set
        End Property

        Public Property CalibrationResults2() As LiNaKCl
            Get
                Return CalibrationResults2Attr
            End Get
            Set(ByVal value As LiNaKCl)
                CalibrationResults2Attr = value
            End Set
        End Property

        Public Property PumpsCalibrationValues() As PumpCalibrationValues
            Get
                Return PumpsCalibrationValuesAttr
            End Get
            Set(ByVal value As PumpCalibrationValues)
                PumpsCalibrationValuesAttr = value
            End Set
        End Property

        Public Property BubbleDetCalibrationValues() As BubbleCalibrationValues
            Get
                Return BubbleDetCalibrationValuesAttr
            End Get
            Set(ByVal value As BubbleCalibrationValues)
                BubbleDetCalibrationValuesAttr = value
            End Set
        End Property

        Public Property ChecksumValue() As String
            Get
                Return ChecksumValueAttr
            End Get
            Set(ByVal value As String)
                ChecksumValueAttr = value
            End Set
        End Property

        Public Property DallasSNData() As ISEDallasSNTO
            Get
                Return DallasSNDataAttr
            End Get
            Set(ByVal value As ISEDallasSNTO)
                DallasSNDataAttr = value
            End Set
        End Property

        Public Property DallasPage00Data() As ISEDallasPage00TO
            Get
                Return DallasPage00DataAttr
            End Get
            Set(ByVal value As ISEDallasPage00TO)
                DallasPage00DataAttr = value
            End Set
        End Property

        Public Property DallasPage01Data() As ISEDallasPage01TO
            Get
                Return DallasPage01DataAttr
            End Get
            Set(ByVal value As ISEDallasPage01TO)
                DallasPage01DataAttr = value
            End Set
        End Property
        'end 11/01/2012

        Public Property Errors() As List(Of ISEErrorTO)
            Get
                Return ErrorsAttr
            End Get
            Set(ByVal value As List(Of ISEErrorTO))
                ErrorsAttr = value
            End Set
        End Property

        Public Property ErrorsString() As String
            Get
                Return ErrorsStringAttr
            End Get
            Set(ByVal value As String)
                ErrorsStringAttr = value
            End Set
        End Property
        'SGM 16/01/2012



#End Region

#Region "Constructor"

        Public Sub New()
            ISEResultType = ISEResultTypes.None
            'Li = 0
            'Na = 0
            'K = 0
            'Cl = 0
            ErrorsString = ""
            ReceivedResults = ""

            WorkSessionID = ""
            PatientID = ""

            ConcentrationValues = New LiNaKCl(-1, -1, -1, -1)
            SerumMilivolts = New LiNaKCl(-1, -1, -1, -1)
            UrineMilivolts = New LiNaKCl(-1, -1, -1, -1)
            CalibratorAMilivolts = New LiNaKCl(-1, -1, -1, -1)
            CalibratorBMilivolts = New LiNaKCl(-1, -1, -1, -1)
            CalibrationResults1 = New LiNaKCl(-1, -1, -1, -1)
            CalibrationResults2 = New LiNaKCl(-1, -1, -1, -1)
            PumpsCalibrationValues = New PumpCalibrationValues(-1, -1, -1)
            BubbleDetCalibrationValues = New BubbleCalibrationValues(-1, -1, -1)
            ChecksumValue = ""
            DallasSNData = Nothing
            DallasPage00Data = Nothing
            DallasPage01Data = Nothing

            Errors = New List(Of ISEErrorTO)
            ErrorsString = ""

        End Sub

#End Region

        ''' <summary>
        ''' Override the method to return the recived result line from ISE Module.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 14/01/2011
        ''' </remarks>
        Public Overrides Function ToString() As String
            Return ReceivedResultLineAttribute
        End Function

    End Class
End Namespace
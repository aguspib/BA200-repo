Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global

    Public Class ISEDallasPage00TO


#Region "Attributes"
        Private Page00DataStringAttr As String = ""
        Private LotNumberAttr As String = ""
        Private ExpirationDayAttr As Integer = -1
        Private ExpirationMonthAttr As Integer = -1
        Private ExpirationYearAttr As Integer = -1
        Private InitialCalibAVolumeAttr As Integer = -1
        Private InitialCalibBVolumeAttr As Integer = -1
        Private DistributorCodeAttr As String = ""
        Private SecurityCodeAttr As String = ""
        Private CRCAttr As String = ""
        Private ValidationErrorAttr As Boolean = False 'SGM 06/06/2012
#End Region

#Region "Properties"

        Public Property Page00DataString() As String
            Get
                Return Page00DataStringAttr
            End Get
            Set(ByVal value As String)
                Page00DataStringAttr = value
            End Set
        End Property

        Public Property LotNumber() As String
            Get
                Return LotNumberAttr
            End Get
            Set(ByVal value As String)
                LotNumberAttr = value
            End Set
        End Property

        Public Property ExpirationDay() As Integer
            Get
                Return ExpirationDayAttr
            End Get
            Set(ByVal value As Integer)
                If value > 0 And value <= 31 Then
                    ExpirationDayAttr = value
                Else
                    ExpirationDayAttr = -1
                    Me.ValidationError = True
                End If
            End Set
        End Property

        Public Property ExpirationMonth() As Integer
            Get
                Return ExpirationMonthAttr
            End Get
            Set(ByVal value As Integer)
                If value > 0 And value <= 31 Then
                    ExpirationMonthAttr = value
                Else
                    ExpirationMonthAttr = -1
                    Me.ValidationError = True
                End If
            End Set
        End Property

        Public Property ExpirationYear() As Integer
            Get
                Return ExpirationYearAttr
            End Get
            Set(ByVal value As Integer)
                If value > 2000 And value <= 2100 Then
                    ExpirationYearAttr = value
                Else
                    ExpirationYearAttr = -1
                    Me.ValidationError = True
                End If
            End Set
        End Property

        Public ReadOnly Property ExpirationDate() As DateTime
            Get
                If Me.ExpirationYearAttr >= 0 And Me.ExpirationMonthAttr >= 1 And Me.ExpirationDayAttr >= 1 Then
                    Dim myDate As New DateTime(Me.ExpirationYear, Me.ExpirationMonthAttr, Me.ExpirationDayAttr)
                    Return myDate 'new DateTime(2011,05,30) for testing
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' consumption value in mililitres
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InitialCalibAVolume() As Integer
            Get
                Return InitialCalibAVolumeAttr
            End Get
            Set(ByVal value As Integer)
                InitialCalibAVolumeAttr = value
            End Set
        End Property

        ''' <summary>
        ''' consumption value in mililitres
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InitialCalibBVolume() As Integer
            Get
                Return InitialCalibBVolumeAttr
            End Get
            Set(ByVal value As Integer)
                InitialCalibBVolumeAttr = value
            End Set
        End Property

        Public Property DistributorCode() As String
            Get
                Return DistributorCodeAttr
            End Get
            Set(ByVal value As String)
                DistributorCodeAttr = value
            End Set
        End Property

        Public Property SecurityCode() As String
            Get
                Return SecurityCodeAttr
            End Get
            Set(ByVal value As String)
                SecurityCodeAttr = value
            End Set
        End Property

        Public Property CRC() As String
            Get
                Return CRCAttr
            End Get
            Set(ByVal value As String)
                CRCAttr = value
            End Set
        End Property

        'error because of wrong mapping of the data - is not Biosystems pack
        Public Property ValidationError() As Boolean
            Get
                Return ValidationErrorAttr
            End Get
            Set(ByVal value As Boolean)
                If ValidationErrorAttr <> value Then
                    ValidationErrorAttr = value
                End If
            End Set
        End Property

#End Region

#Region "Constructor"

        Public Sub New()

        End Sub


#End Region

        Public Overrides Function ToString() As String
            Return Me.Page00DataStringAttr
        End Function

    End Class
End Namespace
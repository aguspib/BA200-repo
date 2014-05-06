Option Explicit On
Option Strict On

Namespace Biosystems.Ax00.Global

    Public Class ISEDallasSNTO

#Region "Attribute"
        Private SNDataStringAttr As String = ""
        Private SerialSerialNumberAttr As String = ""
        Private CRCAttr As String = ""
        Private ValidationErrorAttr As Boolean = False 'SGM 06/06/2012
#End Region

#Region "Properties"
        Public Property SNDataString() As String
            Get
                Return SNDataStringAttr
            End Get
            Set(ByVal value As String)
                SNDataStringAttr = value
            End Set
        End Property

        Public Property SerialNumber() As String
            Get
                Return SerialSerialNumberAttr
            End Get
            Set(ByVal value As String)
                SerialSerialNumberAttr = value
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
                ValidationErrorAttr = value
            End Set
        End Property

#End Region

        Public Overrides Function ToString() As String
            Return SNDataStringAttr
        End Function


    End Class
End Namespace
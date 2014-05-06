Option Strict On
Option Explicit On


Namespace Biosystems.Ax00.Global
    Public Class ISECalibratorResultTO

#Region "Attribute"
        'Calibrator
        Private CalibratorResultStringAttr As String = ""
        Private CalibratorNumberAttribute As String = ""
        Private Cal_LitAttribute As Single = 0
        Private Cal_NaAttribute As Single = 0
        Private Cal_KAttribute As Single = 0
        Private Cal_ClAttribute As Single = 0

        Private ISECalRemarkAttribute As String = ""
        Private RecivedResultLineAttribute As String = "" 'Use for debug mode reults.

#End Region

#Region "Properties"

        Public Property CalibratorResultString() As String
            Get
                Return CalibratorResultStringAttr
            End Get
            Set(ByVal value As String)
                CalibratorResultStringAttr = value
            End Set
        End Property

        Public Property CalNumber() As String
            Get
                Return CalibratorNumberAttribute
            End Get
            Set(ByVal value As String)
                CalibratorNumberAttribute = value
            End Set
        End Property

        'Calibrator
        Public Property Cal_Lit() As Single
            Get
                Return Cal_LitAttribute
            End Get
            Set(ByVal value As Single)
                Cal_LitAttribute = value
            End Set
        End Property

        Public Property Cal_Na() As Single
            Get
                Return Cal_NaAttribute
            End Get
            Set(ByVal value As Single)
                Cal_NaAttribute = value
            End Set
        End Property

        Public Property Cal_K() As Single
            Get
                Return Cal_KAttribute
            End Get
            Set(ByVal value As Single)
                Cal_KAttribute = value
            End Set
        End Property

        Public Property Cal_Cl() As Single
            Get
                Return Cal_ClAttribute
            End Get
            Set(ByVal value As Single)
                Cal_ClAttribute = value
            End Set
        End Property

        Public Property ISECalRemark() As String
            Get
                Return ISECalRemarkAttribute
            End Get
            Set(ByVal value As String)
                ISECalRemarkAttribute = value
            End Set
        End Property

        Public Property RecivedResultLine() As String
            Get
                Return RecivedResultLineAttribute
            End Get
            Set(ByVal value As String)
                RecivedResultLineAttribute = value
            End Set
        End Property

#End Region

#Region "Constructor"

        Public Sub New()

        End Sub

        Public Sub New(ByVal pResultString As String)
            CalibratorResultStringAttr = pResultString
        End Sub
#End Region



    End Class
End Namespace
Namespace Biosystems.Ax00.Global.TO

    Public Class DynamicBaseLineTO

#Region "Attributes"
        Private WavelengthAttr As Integer
        Private WellUsedAttr As New List(Of Integer)
        Private MainLightAttr As New List(Of Integer)
        Private RefLightAttr As New List(Of Integer)
        Private MainDarkAttr As Integer
        Private RefDarkAttr As Integer
        Private MainBaseLineAttr As Integer 'Defined in instruction but not used by software
        Private RefBaseLineAttr As Integer 'Defined in instruction but not used by software
        Private IntegrationTimeAttr As Single
        Private DACAttr As Single

#End Region


#Region "Properties"

        Public Property Wavelength() As Integer
            Get
                Return WavelengthAttr
            End Get
            Set(ByVal value As Integer)
                WavelengthAttr = value
            End Set
        End Property

        Public Property WellUsed() As List(Of Integer)
            Get
                Return WellUsedAttr
            End Get
            Set(ByVal value As List(Of Integer))
                WellUsedAttr = value
            End Set
        End Property

        Public Property MainLight() As List(Of Integer)
            Get
                Return MainLightAttr
            End Get
            Set(ByVal value As List(Of Integer))
                MainLightAttr = value
            End Set
        End Property

        Public Property RefLight() As List(Of Integer)
            Get
                Return RefLightAttr
            End Get
            Set(ByVal value As List(Of Integer))
                RefLightAttr = value
            End Set
        End Property

        Public Property MainDark() As Integer
            Get
                Return MainDarkAttr
            End Get
            Set(ByVal value As Integer)
                MainDarkAttr = value
            End Set
        End Property

        Public Property RefDark() As Integer
            Get
                Return RefDarkAttr
            End Get
            Set(ByVal value As Integer)
                RefDarkAttr = value
            End Set
        End Property

        Public Property MainBaseLine() As Integer
            Get
                Return MainBaseLineAttr
            End Get
            Set(ByVal value As Integer)
                MainBaseLineAttr = value
            End Set
        End Property

        Public Property RefBaseLine() As Integer
            Get
                Return RefBaseLineAttr
            End Get
            Set(ByVal value As Integer)
                RefBaseLineAttr = value
            End Set
        End Property

        Public Property IntegrationTime() As Single
            Get
                Return IntegrationTimeAttr
            End Get
            Set(ByVal value As Single)
                IntegrationTimeAttr = value
            End Set
        End Property

        Public Property DAC() As Single
            Get
                Return DACAttr
            End Get
            Set(ByVal value As Single)
                DACAttr = value
            End Set
        End Property

#End Region


#Region "Public methods"

        Public Sub Add(ByVal pWell As Integer, ByVal pMainLight As Integer, ByVal pRefLight As Integer)
            If Not WellUsedAttr Is Nothing AndAlso Not MainLightAttr Is Nothing AndAlso Not RefLightAttr Is Nothing Then
                WellUsedAttr.Add(pWell)
                MainLightAttr.Add(pMainLight)
                RefLightAttr.Add(pRefLight)
            End If
        End Sub

#End Region


#Region "Constructor"

        Public Sub New()
            WavelengthAttr = 0
            WellUsedAttr.Clear()
            MainLightAttr.Clear()
            RefLightAttr.Clear()
            MainDarkAttr = 0
            RefDarkAttr = 0
            MainBaseLineAttr = 0
            RefBaseLineAttr = 0
            IntegrationTimeAttr = 0
            DACAttr = 0
        End Sub

        Public Sub dispose()
            WellUsedAttr.Clear()
            WellUsedAttr = Nothing

            MainLightAttr.Clear()
            MainLightAttr = Nothing

            RefLightAttr.Clear()
            RefLightAttr = Nothing
        End Sub

#End Region

    End Class

End Namespace

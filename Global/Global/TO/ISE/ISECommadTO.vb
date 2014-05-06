Option Explicit On
Option Strict On


Namespace Biosystems.Ax00.Global
    Public Class ISECommandTO

#Region "Constructor"

        Public Sub New()

        End Sub

        Public Sub New(ByVal pISETestType As GlobalEnumerates.ISECycles)
            MyClass.TestTypeAttr = pISETestType
        End Sub
#End Region

#Region "Attribute"
        Private CommandDataStringAttr As String = ""
        Private SentDatetimeAttr As DateTime
        Private ISEModeAttr As GlobalEnumerates.ISEModes
        Private ISECommandIDAttr As GlobalEnumerates.ISECommands
        Private P1Attr As String  ' XBC 12/01/2012 
        Private P2Attr As String  ' XBC 12/01/2012 
        Private P3Attr As String  ' XBC 12/01/2012 
        Private SampleTubePosAttr As Integer
        Private SampleTubeTypeAttr As String 'AG 11/01/2012 GlobalEnumerates.ISESampleTubeTypes
        Private SampleRotorTypeAttr As Integer
        Private SampleVolumeAttr As Integer 'microlitres

        Private TestTypeAttr As GlobalEnumerates.ISECycles = GlobalEnumerates.ISECycles.NONE

#End Region

#Region "Properties"
        Public ReadOnly Property CommandDataString() As String
            Get
                Return CommandDataStringAttr
            End Get
        End Property

        Public Property SentDatetime() As DateTime
            Get
                Return SentDatetimeAttr
            End Get
            Set(ByVal value As DateTime)
                SentDatetimeAttr = value
            End Set
        End Property
        Public Property ISEMode() As GlobalEnumerates.ISEModes
            Get
                Return ISEModeAttr
            End Get
            Set(ByVal value As GlobalEnumerates.ISEModes)
                ISEModeAttr = value
            End Set
        End Property

        Public Property ISECommandID() As GlobalEnumerates.ISECommands
            Get
                Return ISECommandIDAttr
            End Get
            Set(ByVal value As GlobalEnumerates.ISECommands)
                ISECommandIDAttr = value
            End Set
        End Property

        Public Property P1() As String  ' XBC 12/01/2012 
            Get
                Return P1Attr
            End Get
            Set(ByVal value As String)
                P1Attr = value
            End Set
        End Property

        Public Property P2() As String  ' XBC 12/01/2012 
            Get
                Return P2Attr
            End Get
            Set(ByVal value As String)
                P2Attr = value
            End Set
        End Property

        Public Property P3() As String  ' XBC 12/01/2012 
            Get
                Return P3Attr
            End Get
            Set(ByVal value As String)
                P3Attr = value
            End Set
        End Property

        Public Property SampleTubePos() As Integer
            Get
                Return SampleTubePosAttr
            End Get
            Set(ByVal value As Integer)
                SampleTubePosAttr = value
            End Set
        End Property

        Public Property SampleTubeType() As String 'AG 11/01/2012 GlobalEnumerates.ISESampleTubeTypes
            Get
                Return SampleTubeTypeAttr
            End Get
            Set(ByVal value As String) 'AG 11/01/2012 GlobalEnumerates.ISESampleTubeTypes)
                SampleTubeTypeAttr = value
            End Set
        End Property

        Public Property SampleRotorType() As Integer
            Get
                Return SampleRotorTypeAttr
            End Get
            Set(ByVal value As Integer)
                SampleRotorTypeAttr = value
            End Set
        End Property

        Public Property SampleVolume() As Integer
            Get
                Return SampleVolumeAttr
            End Get
            Set(ByVal value As Integer)
                SampleVolumeAttr = value
            End Set
        End Property

        Public ReadOnly Property TestType() As GlobalEnumerates.ISECycles
            Get
                Return TestTypeAttr
            End Get
        End Property

#End Region


        ''' <summary>
        ''' Override the method to return the recived result line from ISE Module.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 14/01/2011
        ''' </remarks>
        Public Overrides Function ToString() As String
            Return CommandDataStringAttr
        End Function


    End Class
End Namespace
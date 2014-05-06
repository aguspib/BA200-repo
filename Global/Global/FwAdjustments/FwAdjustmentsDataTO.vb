Option Explicit On
Option Strict On

Public Class FwAdjustmentsDataTO

#Region "Attributes"
    Private AnalyzerIDAttr As String
    Private CodeFwAttr As String
    Private ValueAttr As String
    Private CanSaveAttr As Boolean
    Private CanMoveAttr As Boolean
    Private AxisIDAttr As String
    Private GroupIDAttr As String
    Private InFileAttr As Boolean
#End Region

#Region "Properties"
    Public Property AnalyzerID() As String
        Get
            Return AnalyzerIDAttr
        End Get
        Set(ByVal value As String)
            AnalyzerIDAttr = value
        End Set
    End Property

    Public Property CodeFw() As String
        Get
            Return CodeFwAttr
        End Get
        Set(ByVal value As String)
            CodeFwAttr = value
        End Set
    End Property

    Public Property Value() As String
        Get
            Return ValueAttr
        End Get
        Set(ByVal value As String)
            ValueAttr = value
        End Set
    End Property

    Public Property CanSave() As Boolean
        Get
            Return CanSaveAttr
        End Get
        Set(ByVal value As Boolean)
            CanSaveAttr = value
        End Set
    End Property

    Public Property CanMove() As Boolean
        Get
            Return CanMoveAttr
        End Get
        Set(ByVal value As Boolean)
            CanMoveAttr = value
        End Set
    End Property

    Public Property AxisID() As String
        Get
            Return AxisIDAttr
        End Get
        Set(ByVal value As String)
            AxisIDAttr = value
        End Set
    End Property

    Public Property GroupID() As String
        Get
            Return GroupIDAttr
        End Get
        Set(ByVal value As String)
            GroupIDAttr = value
        End Set
    End Property

    Public Property InFile() As Boolean
        Get
            Return InFileAttr
        End Get
        Set(ByVal value As Boolean)
            InFileAttr = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New(ByVal pCodeFw As String)
        CodeFw = pCodeFw
        AnalyzerID = ""
        Value = ""
        CanSave = False
        CanMove = False
        GroupID = ""
        AxisID = ""
        InFile = False
    End Sub
#End Region

#Region "Public Methods"
    Public Sub Clear()
        AnalyzerID = ""
        CodeFw = ""
        Value = ""
        CanSave = False
        CanMove = False
        GroupID = ""
        AxisID = ""
        InFile = False
    End Sub
#End Region

End Class

#Region "TO DELETE"

''Imports Biosystems.Ax00.Global

'Public Class FwAdjustmentsDataTO

'    '#Region "Attributes"
'    '    'QUITAR
'    '    'Private OpticCenteringAttr As ArmTO
'    '    'Private WashingStationAttr As ArmTO

'    '    'Private SampleArmAttr As ArmTO
'    '    'Private Reagent1ArmAttr As ArmTO
'    '    'Private Reagent2ArmAttr As ArmTO
'    '    'Private Mixer1ArmAttr As ArmTO
'    '    'Private Mixer2ArmAttr As ArmTO
'    '#End Region

'    '#Region "Attributes"
'    '    'Private PositionArmAtrr As List(Of PositionTO)
'    '#End Region

'    '#Region "Properties"
'    '    'Public Property PositionArm() As List(Of PositionTO)
'    '    '    Get
'    '    '        Return Me.PositionArmAtrr
'    '    '    End Get
'    '    '    Set(ByVal value As List(Of PositionTO))
'    '    '        Me.PositionArmAtrr = value
'    '    '    End Set
'    '    'End Property


'    '    ''QUITAR
'    '    'Public Property OpticCentering() As ArmTO
'    '    '    Get
'    '    '        Return Me.OpticCenteringAttr
'    '    '    End Get
'    '    '    Set(ByVal value As ArmTO)
'    '    '        Me.OpticCenteringAttr = value
'    '    '    End Set
'    '    'End Property
'    '    'Public Property WashingStation() As ArmTO
'    '    '    Get
'    '    '        Return Me.WashingStationAttr
'    '    '    End Get
'    '    '    Set(ByVal value As ArmTO)
'    '    '        Me.WashingStationAttr = value
'    '    '    End Set
'    '    'End Property



'    '    'Public Property SampleArm() As ArmTO
'    '    '    Get
'    '    '        Return Me.SampleArmAttr
'    '    '    End Get
'    '    '    Set(ByVal value As ArmTO)
'    '    '        Me.SampleArmAttr = value
'    '    '    End Set
'    '    'End Property

'    '    'Public Property Reagent1Arm() As ArmTO
'    '    '    Get
'    '    '        Return Me.Reagent1ArmAttr
'    '    '    End Get
'    '    '    Set(ByVal value As ArmTO)
'    '    '        Me.Reagent1ArmAttr = value
'    '    '    End Set
'    '    'End Property

'    '    'Public Property Reagent2Arm() As ArmTO
'    '    '    Get
'    '    '        Return Me.Reagent2ArmAttr
'    '    '    End Get
'    '    '    Set(ByVal value As ArmTO)
'    '    '        Me.Reagent2ArmAttr = value
'    '    '    End Set
'    '    'End Property

'    '    'Public Property Mixer1Arm() As ArmTO
'    '    '    Get
'    '    '        Return Me.Mixer1ArmAttr
'    '    '    End Get
'    '    '    Set(ByVal value As ArmTO)
'    '    '        Me.Mixer1ArmAttr = value
'    '    '    End Set
'    '    'End Property

'    '    'Public Property Mixer2Arm() As ArmTO
'    '    '    Get
'    '    '        Return Me.Mixer2ArmAttr
'    '    '    End Get
'    '    '    Set(ByVal value As ArmTO)
'    '    '        Me.Mixer2ArmAttr = value
'    '    '    End Set
'    '    'End Property
'    '#End Region

'    '#Region "Constructor"
'    '    'Public Sub New()
'    '    '    'SGX
'    '    '    Me.OpticCentering = New ArmTO
'    '    '    Me.WashingStation = New ArmTO

'    '    '    Me.SampleArmAttr = New ArmTO
'    '    '    Me.Reagent1ArmAttr = New ArmTO
'    '    '    Me.Reagent2ArmAttr = New ArmTO
'    '    '    Me.Mixer1ArmAttr = New ArmTO
'    '    '    Me.Mixer2ArmAttr = New ArmTO
'    '    'End Sub
'    '#End Region

'End Class

#End Region

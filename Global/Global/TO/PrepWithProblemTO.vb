Option Explicit On
Option Strict On


Namespace Biosystems.Ax00.Global.TO

    Public Class PrepWithProblemTO

#Region "TO Structures"
        'Level detection fails when communications error during Running
        Private Structure LevelDetectionMissing
            Dim Sample As Boolean 'True: level detection failed on samples rotor tube
            Dim SamplePosition As Integer

            Dim R1 As Boolean 'True: level detection failed on reagents rotor R1 bottle
            Dim R1Position As Integer

            Dim R2 As Boolean 'True: level detection failed on reagents rotor R2 bottle
            Dim R2Position As Integer

            Dim Diluent As Boolean 'True: level detection failed on reagents rotor diluent bottle (R1 arm)
            Dim DiluentPosition As Integer

            Dim SampleToDilute As Boolean 'True: level detection failed on samples rotor 
            Dim SampleToDilutePosition As Integer

            Dim DilutedSample As Boolean 'True: level detection failed on reactions rotor. In this case no position is required

            Dim R1ContaminationRisk As Boolean 'True: level detection failed on a washing solution. Next R1 preparation has contamination risk
            Dim R2ContaminationRisk As Boolean 'True: level detection failed on a washing solution. Next R2 preparation has contamination risk
        End Structure

        'Clot detection warnings when communications error during Running
        Private Structure ClotDetectionWarnings
            Dim ClotDetected As Boolean
            Dim ClotPossible As Boolean
        End Structure

        'Arm collision errors when communications error during Running
        Private Structure ArmCollisionErrors
            Dim Sample As Boolean 'Collision in Sample Arm
            Dim R1 As Boolean 'Collision in R1 Arm
            Dim R2 As Boolean 'Collision in R2 Arm
        End Structure

#End Region

#Region "Attributes"
        Private PreparationIDAttribute As Integer
        Private ExecutionIDAttribute As Integer
        Private LevelDetectionAttribute As LevelDetectionMissing
        Private ClotDetectionAttribute As ClotDetectionWarnings
        Private ArmCollisionAttribute As ArmCollisionErrors

#End Region

#Region "Properties"
        'Define a property Get and Set for each Attribute

        Public Property ExecutionID() As Integer
            Get
                Return ExecutionIDAttribute
            End Get
            Set(ByVal value As Integer)
                ExecutionIDAttribute = value
            End Set
        End Property

        Public Property PreparationID() As Integer
            Get
                Return PreparationIDAttribute
            End Get
            Set(ByVal value As Integer)
                PreparationIDAttribute = value
            End Set
        End Property


#Region "SubRegion Level Detection"

        Public Property SampleLevelDetectionKO() As Boolean
            Get
                Return LevelDetectionAttribute.Sample
            End Get
            Set(ByVal value As Boolean)
                LevelDetectionAttribute.Sample = value
            End Set
        End Property

        Public Property SamplePositionKO() As Integer
            Get
                Return LevelDetectionAttribute.SamplePosition
            End Get
            Set(ByVal value As Integer)
                LevelDetectionAttribute.SamplePosition = value
            End Set
        End Property

        Public Property R1LevelDetectionKO() As Boolean
            Get
                Return LevelDetectionAttribute.R1
            End Get
            Set(ByVal value As Boolean)
                LevelDetectionAttribute.R1 = value
            End Set
        End Property

        Public Property R1PositionKO() As Integer
            Get
                Return LevelDetectionAttribute.R1Position
            End Get
            Set(ByVal value As Integer)
                LevelDetectionAttribute.R1Position = value
            End Set
        End Property

        Public Property R2LevelDetectionKO() As Boolean
            Get
                Return LevelDetectionAttribute.R2
            End Get
            Set(ByVal value As Boolean)
                LevelDetectionAttribute.R2 = value
            End Set
        End Property

        Public Property R2PositionKO() As Integer
            Get
                Return LevelDetectionAttribute.R2Position
            End Get
            Set(ByVal value As Integer)
                LevelDetectionAttribute.R2Position = value
            End Set
        End Property

        Public Property DiluentLevelDetectionKO() As Boolean
            Get
                Return LevelDetectionAttribute.Diluent
            End Get
            Set(ByVal value As Boolean)
                LevelDetectionAttribute.Diluent = value
            End Set
        End Property

        Public Property DiluentPositionKO() As Integer
            Get
                Return LevelDetectionAttribute.DiluentPosition
            End Get
            Set(ByVal value As Integer)
                LevelDetectionAttribute.DiluentPosition = value
            End Set
        End Property

        Public Property SampleToDiluteLevelDetectionKO() As Boolean
            Get
                Return LevelDetectionAttribute.SampleToDilute
            End Get
            Set(ByVal value As Boolean)
                LevelDetectionAttribute.SampleToDilute = value
            End Set
        End Property

        Public Property SampleToDilutePositionKO() As Integer
            Get
                Return LevelDetectionAttribute.SampleToDilutePosition
            End Get
            Set(ByVal value As Integer)
                LevelDetectionAttribute.SampleToDilutePosition = value
            End Set
        End Property

        Public Property DilutedSampleLevelDetectionKO() As Boolean
            Get
                Return LevelDetectionAttribute.DilutedSample
            End Get
            Set(ByVal value As Boolean)
                LevelDetectionAttribute.DilutedSample = value
            End Set
        End Property

        Public Property R1ContaminationRiskKO() As Boolean
            Get
                Return LevelDetectionAttribute.R1ContaminationRisk
            End Get
            Set(ByVal value As Boolean)
                LevelDetectionAttribute.R1ContaminationRisk = value
            End Set
        End Property

        Public Property R2ContaminationRiskKO() As Boolean
            Get
                Return LevelDetectionAttribute.R2ContaminationRisk
            End Get
            Set(ByVal value As Boolean)
                LevelDetectionAttribute.R2ContaminationRisk = value
            End Set
        End Property

#End Region

#Region "SubRegion Clot Detection"

        Public Property ClotDetectionKO() As Boolean
            Get
                Return ClotDetectionAttribute.ClotDetected
            End Get
            Set(ByVal value As Boolean)
                ClotDetectionAttribute.ClotDetected = value
            End Set
        End Property

        Public Property ClotPossibleKO() As Boolean
            Get
                Return ClotDetectionAttribute.ClotPossible
            End Get
            Set(ByVal value As Boolean)
                ClotDetectionAttribute.ClotPossible = value
            End Set
        End Property

#End Region

#Region "SubRegion Arm Collision Detection"
        Public Property SampleCollisionKO() As Boolean
            Get
                Return ArmCollisionAttribute.Sample
            End Get
            Set(ByVal value As Boolean)
                ArmCollisionAttribute.Sample = value
            End Set
        End Property

        Public Property R1CollisionKO() As Boolean
            Get
                Return ArmCollisionAttribute.R1
            End Get
            Set(ByVal value As Boolean)
                ArmCollisionAttribute.R1 = value
            End Set
        End Property

        Public Property R2CollisionKO() As Boolean
            Get
                Return ArmCollisionAttribute.R2
            End Get
            Set(ByVal value As Boolean)
                ArmCollisionAttribute.R2 = value
            End Set
        End Property

#End Region

#End Region

#Region "Constructor"
        Public Sub New()
            ExecutionIDAttribute = -1
            PreparationIDAttribute = -1
            With LevelDetectionAttribute
                .Sample = False
                .SamplePosition = 0
                .R1 = False
                .R1Position = 0
                .R2 = False
                .R2Position = 0
                .Diluent = False
                .DiluentPosition = 0
                .SampleToDilute = False
                .SampleToDilutePosition = 0
                .DilutedSample = False
                .R1ContaminationRisk = False
                .R2ContaminationRisk = False
            End With

            With ClotDetectionAttribute
                .ClotDetected = False
                .ClotPossible = False
            End With

            With ArmCollisionAttribute
                .Sample = False
                .R1 = False
                .R2 = False
            End With

        End Sub

#End Region

    End Class

End Namespace


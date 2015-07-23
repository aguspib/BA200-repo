Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.App

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class ThermosAdjustmentsDelegate
        Inherits BaseFwScriptDelegate

#Region "Attributes"
        Private CurrentOperationAttr As OPERATIONS      ' controls the operation which is currently executed in each time
        Private CurrentTestAttr As ADJUSTMENT_GROUPS    ' Test currently executed
        Private pValueAdjustAttr As String

        Private FillModeAttr As FILL_MODE ' indicates if the system must be filled automatically or by user

        Private ProbeWellsTempsAttr As New List(Of Single)
        'Private ProbeWellsTempsDoneAttr As New List(Of Boolean)
        Private ReagentMeasuredTempAttr As Single
        Private HeaterMeasuredTempAttr As Single

        Private MonitorThermoRotorAttr As Single
        Private MonitorThermoR1Attr As Single
        Private MonitorThermoR2Attr As Single
        Private MonitorThermoHeaterAttr As Single

        Private ProposalCorrectionAttr As Single

        Private ReactionsRotorConditioningDoneAttr As Boolean
        Private ReactionsRotorConditioningDoingAttr As Boolean
        Private ReactionsRotorHomesDoneAttr As Boolean
        Private ReactionsRotorConditioningManualFirstStepDoneAttr As Boolean
        Private ReactionsRotorConditioningManualSecondStepDoneAttr As Boolean
        Private ReactionsRotorWellFilledAttr As New List(Of Boolean)
        Private ReactionsRotorPrepareWellDoneAttr As New List(Of Boolean)
        Private ReactionsRotorRotatingDoneAttr As Boolean 'SGM 19/10/2011
        Private ReactionsRotorRotatingDoingAttr As New List(Of Boolean) 'SGM 19/10/2011
        Private ReactionsRotorRotationsDoneAttr As New List(Of Boolean) 'SGM 19/10/2011
        Private ReactionsRotorRotationsNumAttr As Integer 'SGM 19/10/2011
        Private ReactionsRotorCurrentRotationAttr As Integer 'SGM 19/10/2011
        Private ReactionsRotorMeasuredTempDoneAttr As Boolean
        Private ReactionsRotorMeasuredTempDoingAttr As Boolean
        Private ReactionsRotorCurrentWellAttr As Integer
        Private ReactionsRotorTestAdjustmentDoneAttr As Boolean
        Private ReactionsRotorTCOAttr As String
        'SGM 13/10/2011
        Private Reagent1NeedlePrimingDoneAttr As Boolean
        Private Reagent2NeedlePrimingDoneAttr As Boolean
        'Private ReagentNeedleConditioningDoneAttr As Boolean
        'SGM 13/10/2011
        Private ReagentNeedleConditioningNumDispAttr As Integer
        Private ReagentNeedleConditioningDoingAttr As Boolean
        Private ReagentNeedleMaxDispensationsTemperAttr As Integer
        Private ReagentNeedleMaxPrimeOperationsAttr As Integer 'SGM 19/10/2011
        Private ReagentNeedleDispensingDoneAttr As Boolean 'SGM 19/10/2011
        Private ReagentNeedleDispensingDoingAttr As New List(Of Boolean) 'SGM 19/10/2011
        Private ReagentNeedleDispensationsDoneAttr As New List(Of Boolean) 'SGM 19/10/2011
        Private ReagentNeedleCurrentDispensationAttr As Integer 'SGM 19/10/2011
        Private ReagentNeedleDispensingNumAttr As Integer 'SGM 19/10/2011
        Private ReagentNeedleMeasuredTempDoneAttr As Boolean
        Private ReagentNeedleMeasuredTempDoingAttr As Boolean
        Private ReagentNeedleTestAdjustmentDoneAttr As Boolean
        Private ReagentNeedleTCOAttr As String



        'Needles 
        Public IsReagentArm1InParking As Boolean
        Public IsReagentArm2InParking As Boolean

        'Reagent 1
        Private R1WSVerticalRefPosAttr As Integer
        Private R1RotorRing1HorPosAttr As Integer
        Private R1RotorRing1DetMaxVerPosAttr As Integer
        Private R1VerticalSafetyPosAttr As Integer
        Private R1WSHorizontalPosAttr As Integer
        Private R1RotorPosAttr As Integer
        Private Reagent1ParkingZAttr As Single
        Private Reagent1ParkingPolarAttr As Single
        Private R1InitialPointLevelDetAttr As Integer
        Private R2InitialPointLevelDetAttr As Integer

        'Reagent 2
        Private R2WSVerticalRefPosAttr As Integer
        Private R2RotorRing1HorPosAttr As Integer
        Private R2RotorRing1DetMaxVerPosAttr As Integer
        Private R2VerticalSafetyPosAttr As Integer
        Private R2WSHorizontalPosAttr As Integer
        Private R2RotorPosAttr As Integer
        Private Reagent2ParkingZAttr As Single
        Private Reagent2ParkingPolarAttr As Single




        'WS Heater
        Private WSReadyRefPosAttr As Integer
        Private WSFinalPosAttr As Integer
        Private WSReferencePosAttr As Integer

        'Rotor
        Private RotorWellsToFillAttr As New List(Of Integer)
        Public RotorConditioningCancelRequested As Boolean = False 'SGM 02/12/2011
        Public RotorRotatingCancelRequested As Boolean = False 'SGM 02/12/2011



        Private HeaterConditioningDoneAttr As Boolean
        Private HeaterConditioningNumDispAttr As Integer
        Private HeaterConditioningDoingAttr As List(Of Boolean)
        Private HeaterTestAdjustmentDoneAttr As Boolean
        Private HeaterTCOAttr As String

        Private LoadAdjDoneAttr As Boolean

        'Private MeasuredTempFridgeDoneAttr As Boolean

        Private CurrentTimeOperationAttr As Integer                 ' Time expected for current operation

        Private NoneInstructionToSendAttr As Boolean

        ' Absolute Movements parameters

        ' Reagents1 Arm
        Private ArmReagent1DispPolarAttr As String
        Private ArmReagent1DispZAttr As String
        Private ArmReagent1WSPolarAttr As String
        Private ArmReagent1Ring1PolarAttr As String
        Private ArmReagent1Ring1ZAttr As String
        ' Reagents2 Arm
        Private ArmReagent2DispPolarAttr As String
        Private ArmReagent2DispZAttr As String
        Private ArmReagent2WSPolarAttr As String
        Private ArmReagent2Ring1PolarAttr As String
        Private ArmReagent2Ring1ZAttr As String
        ' Reactions Rotor
        Private ReactionsRotorDispWellAttr As String
        Private ReactionsRotorDispPreviousWellAttr As String
        Private ReactionsRotorDispNextWellAttr As String

        ' Parameters
        Private ReactionsRotorMaxPrimesAttr As Integer
        Private ReactionsRotorRotatingTimesAttr As Integer
        Private ReactionsRotorMaxWellsAttr As Integer
        Private ReactionsRotorWellsFilledCountAttr As Integer
        Private ReactionsRotorWellsToFillAttr As List(Of Integer)
        Private ReactionsRotorMeasuredWellsCountAttr As Integer
        Private ReactionsRotorMeasuredWellsAttr As List(Of Integer)
        Private ReagentNeedleHeatMaxDispensationsAttr As Integer
        Private ReagentNeedleSourceWellAttr As Integer
        Private HeaterMaxActionsTemperAttr As Integer
        Private RotorSoundSecondsAttr As Integer = 10 'SGM 18/09/2012 - Seconds that takes the buzzer sound when Rotor conditioning has been finished 

        Private FinalResultAttr() As Integer



        Private ConditioningExecutedAttr() As Integer
        Private MeasuringTempExecutedAttr() As Integer
        Private TestTempExecutedAttr() As Integer
        Private AssignedNewValuesAttr() As Integer
        Private RotatingExecutedAttr As Integer 'SGM 19/10/2011
        Private DispensingExecutedAttr() As Integer 'SGM 19/10/2011

        Private PrimeAmountAttr As Integer

        Private IsKeepRotatingAttr As Boolean
        Private IsKeepRotatingStoppedAttr As Boolean

        ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics
        Private NeedlesDispensationsCountAttr As Integer

        ' XBC 20/04/2012
        Private IsWashingStationUpAttr As Boolean

        'SGM 18/09/2012 - Timer for stopping the buzzer sound after parameterized interval (SRV_SOUND_THERMO)
        Private RotorSoundTimerCallBack As System.Threading.TimerCallback
        Private WithEvents RotorSoundTimer As System.Threading.Timer


#End Region

#Region "Properties"
        Public Property CurrentOperation() As OPERATIONS
            Get
                Return MyClass.CurrentOperationAttr
            End Get
            Set(ByVal value As OPERATIONS)
                MyClass.CurrentOperationAttr = value
            End Set
        End Property

        Public Property CurrentTest() As ADJUSTMENT_GROUPS
            Get
                Return CurrentTestAttr
            End Get
            Set(ByVal value As ADJUSTMENT_GROUPS)
                CurrentTestAttr = value
            End Set
        End Property

        Public Property FillMode() As FILL_MODE
            Get
                Return FillModeAttr
            End Get
            Set(ByVal value As FILL_MODE)
                FillModeAttr = value
            End Set
        End Property

        Public Property ReactionsRotorConditioningDone() As Boolean
            Get
                Return MyClass.ReactionsRotorConditioningDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReactionsRotorConditioningDoneAttr = value
            End Set
        End Property

        Public Property ReactionsRotorConditioningDoing() As Boolean
            Get
                Return MyClass.ReactionsRotorConditioningDoingAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReactionsRotorConditioningDoingAttr = value
            End Set
        End Property

        Public Property ReactionsRotorHomesDone() As Boolean
            Get
                Return MyClass.ReactionsRotorHomesDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReactionsRotorHomesDoneAttr = value
            End Set
        End Property

        Public Property ReactionsRotorConditioningManualFirstStepDone() As Boolean
            Get
                Return MyClass.ReactionsRotorConditioningManualFirstStepDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReactionsRotorConditioningManualFirstStepDoneAttr = value
            End Set
        End Property

        Public Property ReactionsRotorConditioningManualSecondStepDone() As Boolean
            Get
                Return MyClass.ReactionsRotorConditioningManualSecondStepDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReactionsRotorConditioningManualSecondStepDoneAttr = value
            End Set
        End Property

        Public Property ReactionsRotorRotatingDone() As Boolean
            Get
                Return MyClass.ReactionsRotorRotatingDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReactionsRotorRotatingDoneAttr = value
            End Set
        End Property

        Public Property ReactionsRotorRotatingDoing() As List(Of Boolean)
            Get
                Return MyClass.ReactionsRotorRotatingDoingAttr
            End Get
            Set(ByVal value As List(Of Boolean))
                MyClass.ReactionsRotorRotatingDoingAttr = value
            End Set
        End Property

        Public Property IsKeepRotating() As Boolean
            Get
                Return IsKeepRotatingAttr
            End Get
            Set(ByVal value As Boolean)
                IsKeepRotatingAttr = value
            End Set
        End Property

        Public Property IsKeepRotatingStopped() As Boolean
            Get
                Return IsKeepRotatingStoppedAttr
            End Get
            Set(ByVal value As Boolean)
                IsKeepRotatingStoppedAttr = value
            End Set
        End Property


        'SGM 13/10/2011
        Public Property ReagentNeedleConditioningDone() As Boolean
            Get
                Select Case MyClass.CurrentTest
                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1 : Return MyClass.Reagent1NeedlePrimingDoneAttr
                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2 : Return MyClass.Reagent2NeedlePrimingDoneAttr
                End Select
            End Get
            Set(ByVal value As Boolean)
                Select Case MyClass.CurrentTest
                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1 : MyClass.Reagent1NeedlePrimingDoneAttr = value
                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2 : MyClass.Reagent2NeedlePrimingDoneAttr = value
                End Select

            End Set
        End Property

        Public Property HeaterConditioningDone() As Boolean
            Get
                Return MyClass.HeaterConditioningDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.HeaterConditioningDoneAttr = value
            End Set
        End Property

        Public Property ReagentNeedleMaxDispensationsTemper() As Integer
            Get
                Return ReagentNeedleMaxDispensationsTemperAttr
            End Get
            Set(ByVal value As Integer)
                ReagentNeedleMaxDispensationsTemperAttr = value
            End Set
        End Property

        Public Property ReagentNeedleMaxPrimeOperations() As Integer
            Get
                Return ReagentNeedleMaxPrimeOperationsAttr
            End Get
            Set(ByVal value As Integer)
                ReagentNeedleMaxPrimeOperationsAttr = value
            End Set
        End Property

        Public Property ReagentNeedleDispensingDone() As Boolean
            Get
                Return MyClass.ReagentNeedleDispensingDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReagentNeedleDispensingDoneAttr = value
            End Set
        End Property

        Public Property ReagentNeedleDispensingDoing() As List(Of Boolean)
            Get
                Return MyClass.ReagentNeedleDispensingDoingAttr
            End Get
            Set(ByVal value As List(Of Boolean))
                MyClass.ReagentNeedleDispensingDoingAttr = value
            End Set
        End Property

        Public Property ReactionsRotorMeasuredTempDone() As Boolean
            Get
                Return MyClass.ReactionsRotorMeasuredTempDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReactionsRotorMeasuredTempDoneAttr = value
            End Set
        End Property

        Public Property ReactionsRotorMeasuredTempDoing() As Boolean
            Get
                Return MyClass.ReactionsRotorMeasuredTempDoingAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReactionsRotorMeasuredTempDoingAttr = value
            End Set
        End Property

        Public Property ReagentNeedleMeasuredTempDone() As Boolean
            Get
                Return MyClass.ReagentNeedleMeasuredTempDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReagentNeedleMeasuredTempDoneAttr = value
            End Set
        End Property

        Public Property ReactionsRotorPrepareWellDone() As List(Of Boolean)
            Get
                Return MyClass.ReactionsRotorPrepareWellDoneAttr
            End Get
            Set(ByVal value As List(Of Boolean))
                MyClass.ReactionsRotorPrepareWellDoneAttr = value
            End Set
        End Property

        Public Property ReactionsRotorWellFilled() As List(Of Boolean)
            Get
                Return MyClass.ReactionsRotorWellFilledAttr
            End Get
            Set(ByVal value As List(Of Boolean))
                MyClass.ReactionsRotorWellFilledAttr = value
            End Set
        End Property

        Public Property ReactionsRotorCurrentWell() As Integer
            Get
                Return MyClass.ReactionsRotorCurrentWellAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.ReactionsRotorCurrentWellAttr = value
            End Set
        End Property

        Public ReadOnly Property AreReactionsRotorAllPrepared() As Boolean
            Get
                Dim returnValue As Boolean
                If MyClass.ReactionsRotorPrepareWellDoneAttr.Contains(False) Then
                    returnValue = False
                Else
                    returnValue = True
                End If
                Return returnValue
            End Get
        End Property

        Public ReadOnly Property AreReactionsRotorAllFilled() As Boolean
            Get
                Dim returnValue As Boolean
                If MyClass.ReactionsRotorWellFilledAttr.Contains(False) Then
                    returnValue = False
                Else
                    returnValue = True
                End If
                Return returnValue
            End Get
        End Property

        Public Property ReactionsRotorRotationsDone() As List(Of Boolean)
            Get
                Return MyClass.ReactionsRotorRotationsDoneAttr
            End Get
            Set(ByVal value As List(Of Boolean))
                MyClass.ReactionsRotorRotationsDoneAttr = value
            End Set
        End Property

        Public Property ReactionsRotorCurrentRotation() As Integer
            Get
                Return ReactionsRotorCurrentRotationAttr
            End Get
            Set(ByVal value As Integer)
                ReactionsRotorCurrentRotationAttr = value
            End Set
        End Property

        Public ReadOnly Property NextReactionsRotorWellToEdit() As Integer
            Get
                Dim returnValue As Integer = 0

                For i As Integer = 0 To MyClass.ReactionsRotorPrepareWellDoneAttr.Count - 1
                    If Not MyClass.ReactionsRotorPrepareWellDoneAttr(i) Then
                        returnValue = i + 1
                        Exit For
                    End If
                Next

                Return returnValue
            End Get
        End Property

        Public ReadOnly Property CurrentTimeOperation() As Integer
            Get
                Return CurrentTimeOperationAttr
            End Get
        End Property

        Public Property ReactionsRotorTestAdjustmentDone() As Boolean
            Get
                Return MyClass.ReactionsRotorTestAdjustmentDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReactionsRotorTestAdjustmentDoneAttr = value
            End Set
        End Property

        Public Property pValueAdjust() As String
            Get
                Return MyClass.pValueAdjustAttr
            End Get
            Set(ByVal value As String)
                MyClass.pValueAdjustAttr = value
            End Set
        End Property

        Public Property LoadAdjDone() As Boolean
            Get
                Return MyClass.LoadAdjDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.LoadAdjDoneAttr = value
            End Set
        End Property

        Public Property ReagentNeedleConditioningDoing() As Boolean
            Get
                Return MyClass.ReagentNeedleConditioningDoingAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReagentNeedleConditioningDoingAttr = value
            End Set
        End Property

        Public Property ReagentNeedleConditioningNumDisp() As Integer
            Get
                Return MyClass.ReagentNeedleConditioningNumDispAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.ReagentNeedleConditioningNumDispAttr = value
            End Set
        End Property

        Public ReadOnly Property ReactionsRotorRotatingTimes() As Integer
            Get
                Return MyClass.ReactionsRotorRotatingTimesAttr
            End Get
        End Property

        Public ReadOnly Property ReagentNeedleHeatMaxDispensations() As Integer
            Get
                Return MyClass.ReagentNeedleHeatMaxDispensationsAttr
            End Get
        End Property

        Public Property ReagentNeedleMeasuredTempDoing() As Boolean
            Get
                Return MyClass.ReagentNeedleMeasuredTempDoingAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReagentNeedleMeasuredTempDoingAttr = value
            End Set
        End Property

        Public Property ReagentNeedleTestAdjustmentDone() As Boolean
            Get
                Return MyClass.ReagentNeedleTestAdjustmentDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.ReagentNeedleTestAdjustmentDoneAttr = value
            End Set
        End Property

        Public Property AllReagentNeedleDispensationsDone() As List(Of Boolean)
            Get
                Return MyClass.ReagentNeedleDispensationsDoneAttr
            End Get
            Set(ByVal value As List(Of Boolean))
                MyClass.ReagentNeedleDispensationsDoneAttr = value
            End Set
        End Property

        Public Property ReagentNeedleCurrentDispensation() As Integer
            Get
                Return ReagentNeedleCurrentDispensationAttr
            End Get
            Set(ByVal value As Integer)
                ReagentNeedleCurrentDispensationAttr = value
            End Set
        End Property

        Public Property ReagentNeedleDispensingNum() As Integer
            Get
                Return ReagentNeedleDispensingNumAttr
            End Get
            Set(ByVal value As Integer)
                ReagentNeedleDispensingNumAttr = value
            End Set
        End Property

        Public Property HeaterConditioningDoing() As List(Of Boolean)
            Get
                Return MyClass.HeaterConditioningDoingAttr
            End Get
            Set(ByVal value As List(Of Boolean))
                MyClass.HeaterConditioningDoingAttr = value
            End Set
        End Property

        Public Property HeaterConditioningNumDisp() As Integer
            Get
                Return MyClass.HeaterConditioningNumDispAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.HeaterConditioningNumDispAttr = value
            End Set
        End Property

        Public Property HeaterTestAdjustmentDone() As Boolean
            Get
                Return MyClass.HeaterTestAdjustmentDoneAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.HeaterTestAdjustmentDoneAttr = value
            End Set
        End Property

        Public Property NoneInstructionToSend() As Boolean
            Get
                Return NoneInstructionToSendAttr
            End Get
            Set(ByVal value As Boolean)
                NoneInstructionToSendAttr = value
            End Set
        End Property

        Public Property ArmReagent1DispPolar() As String
            Get
                Return MyClass.ArmReagent1DispPolarAttr
            End Get
            Set(ByVal value As String)
                MyClass.ArmReagent1DispPolarAttr = value
            End Set
        End Property

        Public Property ArmReagent1DispZ() As String
            Get
                Return MyClass.ArmReagent1DispZAttr
            End Get
            Set(ByVal value As String)
                MyClass.ArmReagent1DispZAttr = value
            End Set
        End Property

        Public Property ArmReagent1WSPolar() As String
            Get
                Return MyClass.ArmReagent1WSPolarAttr
            End Get
            Set(ByVal value As String)
                MyClass.ArmReagent1WSPolarAttr = value
            End Set
        End Property

        Public Property ArmReagent1Ring1Polar() As String
            Get
                Return MyClass.ArmReagent1Ring1PolarAttr
            End Get
            Set(ByVal value As String)
                MyClass.ArmReagent1Ring1PolarAttr = value
            End Set
        End Property

        Public Property ArmReagent1Ring1Z() As String
            Get
                Return MyClass.ArmReagent1Ring1ZAttr
            End Get
            Set(ByVal value As String)
                MyClass.ArmReagent1Ring1ZAttr = value
            End Set
        End Property

        Public Property ArmReagent2Ring1Polar() As String
            Get
                Return MyClass.ArmReagent2Ring1PolarAttr
            End Get
            Set(ByVal value As String)
                MyClass.ArmReagent2Ring1PolarAttr = value
            End Set
        End Property

        Public Property ArmReagent2Ring1Z() As String
            Get
                Return MyClass.ArmReagent2Ring1ZAttr
            End Get
            Set(ByVal value As String)
                MyClass.ArmReagent2Ring1ZAttr = value
            End Set
        End Property

        Public Property ArmReagent2DispPolar() As String
            Get
                Return MyClass.ArmReagent2DispPolarAttr
            End Get
            Set(ByVal value As String)
                MyClass.ArmReagent2DispPolarAttr = value
            End Set
        End Property

        Public Property ArmReagent2DispZ() As String
            Get
                Return MyClass.ArmReagent2DispZAttr
            End Get
            Set(ByVal value As String)
                MyClass.ArmReagent2DispZAttr = value
            End Set
        End Property

        Public Property ArmReagent2WSPolar() As String
            Get
                Return MyClass.ArmReagent2WSPolarAttr
            End Get
            Set(ByVal value As String)
                MyClass.ArmReagent2WSPolarAttr = value
            End Set
        End Property

        Public Property ReactionsRotorDispWell() As String
            Get
                Return MyClass.ReactionsRotorDispWellAttr
            End Get
            Set(ByVal value As String)
                MyClass.ReactionsRotorDispWellAttr = value
            End Set
        End Property

        Public Property ReactionsRotorDispPreviousWell() As String
            Get
                Return MyClass.ReactionsRotorDispPreviousWellAttr
            End Get
            Set(ByVal value As String)
                MyClass.ReactionsRotorDispPreviousWellAttr = value
            End Set
        End Property

        Public Property ReactionsRotorDispNextWell() As String
            Get
                Return MyClass.ReactionsRotorDispNextWellAttr
            End Get
            Set(ByVal value As String)
                MyClass.ReactionsRotorDispNextWellAttr = value
            End Set
        End Property

        Public ReadOnly Property ReactionsRotorMeasuredWellsCount() As Integer
            Get
                Return MyClass.ReactionsRotorMeasuredWellsCountAttr
            End Get
        End Property

        Public ReadOnly Property ReactionsRotorWellsFilledCount() As Integer
            Get
                Return MyClass.ReactionsRotorWellsFilledCountAttr
            End Get
        End Property

        Public Property ReactionsRotorTCO() As String
            Get
                Return MyClass.ReactionsRotorTCOAttr
            End Get
            Set(ByVal value As String)
                MyClass.ReactionsRotorTCOAttr = value
            End Set
        End Property

        Public Property ReagentNeedleTCO() As String
            Get
                Return MyClass.ReagentNeedleTCOAttr
            End Get
            Set(ByVal value As String)
                MyClass.ReagentNeedleTCOAttr = value
            End Set
        End Property

        Public Property HeaterTCO() As String
            Get
                Return MyClass.HeaterTCOAttr
            End Get
            Set(ByVal value As String)
                MyClass.HeaterTCOAttr = value
            End Set
        End Property

        Public ReadOnly Property HeaterMaxActionsTemper() As Integer
            Get
                Return MyClass.HeaterMaxActionsTemperAttr
            End Get
        End Property


        Public Property FinalResult(ByVal pElement As Integer) As Integer
            Get
                Return MyClass.FinalResultAttr(pElement)
            End Get
            Set(ByVal value As Integer)
                MyClass.FinalResultAttr(pElement) = value
            End Set
        End Property

        Public Property AssignedNewValues(ByVal pElement As Integer) As Integer
            Get
                Return MyClass.AssignedNewValuesAttr(pElement)
            End Get
            Set(ByVal value As Integer)
                MyClass.AssignedNewValuesAttr(pElement) = value
            End Set
        End Property

        Public Property ConditioningExecuted(ByVal pElement As Integer) As Integer
            Get
                Return MyClass.ConditioningExecutedAttr(pElement)
            End Get
            Set(ByVal value As Integer)
                MyClass.ConditioningExecutedAttr(pElement) = value
            End Set
        End Property

        Public Property MeasuringTempExecuted(ByVal pElement As Integer) As Integer
            Get
                Return MyClass.MeasuringTempExecutedAttr(pElement)
            End Get
            Set(ByVal value As Integer)
                MyClass.MeasuringTempExecutedAttr(pElement) = value
            End Set
        End Property

        Public Property TestTempExecuted(ByVal pElement As Integer) As Integer
            Get
                Return MyClass.TestTempExecutedAttr(pElement)
            End Get
            Set(ByVal value As Integer)
                MyClass.TestTempExecutedAttr(pElement) = value
            End Set
        End Property

        'SGM 19/10/2011
        Public Property RotatingExecuted() As Integer
            Get
                Return MyClass.RotatingExecutedAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.RotatingExecutedAttr = value
            End Set
        End Property

        'SGM 19/10/2011
        Public Property DispensingExecuted(ByVal pArm As Integer) As Integer
            Get
                Return MyClass.DispensingExecutedAttr(pArm)
            End Get
            Set(ByVal value As Integer)
                MyClass.DispensingExecutedAttr(pArm) = value
            End Set
        End Property

        Public Property ProbeWellsTemps() As List(Of Single)
            Get
                Return MyClass.ProbeWellsTempsAttr
            End Get
            Set(ByVal value As List(Of Single))
                MyClass.ProbeWellsTempsAttr = value
            End Set
        End Property

        Public Property MonitorThermoRotor() As Single
            Get
                Return MyClass.MonitorThermoRotorAttr
            End Get
            Set(ByVal value As Single)
                MyClass.MonitorThermoRotorAttr = value
            End Set
        End Property

        Public Property MonitorThermoR1() As Single
            Get
                Return MyClass.MonitorThermoR1Attr
            End Get
            Set(ByVal value As Single)
                MyClass.MonitorThermoR1Attr = value
            End Set
        End Property

        Public Property MonitorThermoR2() As Single
            Get
                Return MyClass.MonitorThermoR2Attr
            End Get
            Set(ByVal value As Single)
                MyClass.MonitorThermoR2Attr = value
            End Set
        End Property

        Public Property MonitorThermoHeater() As Single
            Get
                Return MyClass.MonitorThermoHeaterAttr
            End Get
            Set(ByVal value As Single)
                MyClass.MonitorThermoHeaterAttr = value
            End Set
        End Property

        Public Property ProposalCorrection() As Single
            Get
                Return MyClass.ProposalCorrectionAttr
            End Get
            Set(ByVal value As Single)
                MyClass.ProposalCorrectionAttr = value
            End Set
        End Property

        Public Property ReagentMeasuredTemp() As Single
            Get
                Return MyClass.ReagentMeasuredTempAttr
            End Get
            Set(ByVal value As Single)
                MyClass.ReagentMeasuredTempAttr = value
            End Set
        End Property

        Public Property HeaterMeasuredTemp() As Single
            Get
                Return MyClass.HeaterMeasuredTempAttr
            End Get
            Set(ByVal value As Single)
                MyClass.HeaterMeasuredTempAttr = value
            End Set
        End Property

        Public Property PrimeAmount() As Integer
            Get
                Return PrimeAmountAttr
            End Get
            Set(ByVal value As Integer)
                PrimeAmountAttr = value
            End Set
        End Property

        Public Property IsWashingStationUp() As Boolean
            Get
                Return MyClass.IsWashingStationUpAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.IsWashingStationUpAttr = value
            End Set
        End Property


#Region "WS Heater Test"

        ''' <summary>
        ''' WSRR WASHING_STATION Z
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property WSReadyRefPos() As Integer
            Get
                Return WSReadyRefPosAttr
            End Get
            Set(ByVal value As Integer)
                WSReadyRefPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' WSEV WASHING_STATION Z
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property WSFinalPos() As Integer
            Get
                Return WSFinalPosAttr
            End Get
            Set(ByVal value As Integer)
                WSFinalPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' WSRV WASHING_STATION_PARK Z
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property WSReferencePos() As Integer
            Get
                Return WSReferencePosAttr
            End Get
            Set(ByVal value As Integer)
                WSReferencePosAttr = value
            End Set
        End Property

#End Region

#Region "Rotor Test"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Property RotorWellsToFill() As List(Of Integer)
            Get
                Return RotorWellsToFillAttr
            End Get
            Set(ByVal value As List(Of Integer))
                RotorWellsToFillAttr = value
            End Set
        End Property

#End Region

#Region "Needles Test"

        ''' <summary>
        ''' R1PH1 REAGENT1_ARM_RING1 Polar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R1RotorRing1HorPos() As Integer
            Get
                Return R1RotorRing1HorPosAttr
            End Get
            Set(ByVal value As Integer)
                R1RotorRing1HorPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' R1PV1 REAGENT1_ARM_RING1 Z
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R1RotorRing1DetMaxVerPos() As Integer
            Get
                Return R1RotorRing1DetMaxVerPosAttr
            End Get
            Set(ByVal value As Integer)
                R1RotorRing1DetMaxVerPosAttr = value
            End Set
        End Property

        Public Property R1InitialPointLevelDet() As Integer
            Get
                Return R1InitialPointLevelDetAttr
            End Get
            Set(ByVal value As Integer)
                R1InitialPointLevelDetAttr = value
            End Set
        End Property

        Public Property R2InitialPointLevelDet() As Integer
            Get
                Return R2InitialPointLevelDetAttr
            End Get
            Set(ByVal value As Integer)
                R2InitialPointLevelDetAttr = value
            End Set
        End Property

        ''' <summary>
        ''' R1SV REAGENT1_ARM_VSEC Z
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R1VerticalSafetyPos() As Integer
            Get
                Return R1VerticalSafetyPosAttr
            End Get
            Set(ByVal value As Integer)
                R1VerticalSafetyPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' R1WH REAGENT1_ARM_WASH Polar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R1WSHorizontalPos() As Integer
            Get
                Return R1WSHorizontalPosAttr
            End Get
            Set(ByVal value As Integer)
                R1WSHorizontalPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' RR1P1 REAGENT1_ARM_RING1 Polar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R1RotorPos() As Integer
            Get
                Return R1RotorPosAttr
            End Get
            Set(ByVal value As Integer)
                R1RotorPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' R1WVR REAGENT1_ARM_WASH Z
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R1WSVerticalRefPos() As Integer
            Get
                Return R1WSVerticalRefPosAttr
            End Get
            Set(ByVal value As Integer)
                R1WSVerticalRefPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' R2PH1 REAGENT2_ARM_RING1 Polar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R2RotorRing1HorPos() As Integer
            Get
                Return R2RotorRing1HorPosAttr
            End Get
            Set(ByVal value As Integer)
                R2RotorRing1HorPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' R2PV1 REAGENT2_ARM_RING1 Z
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R2RotorRing1DetMaxVerPos() As Integer
            Get
                Return R2RotorRing1DetMaxVerPosAttr
            End Get
            Set(ByVal value As Integer)
                R2RotorRing1DetMaxVerPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' R2SV REAGENT2_ARM_VSEC Z
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R2VerticalSafetyPos() As Integer
            Get
                Return R2VerticalSafetyPosAttr
            End Get
            Set(ByVal value As Integer)
                R2VerticalSafetyPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' R2WH REAGENT2_ARM_WASH Polar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R2WSHorizontalPos() As Integer
            Get
                Return R2WSHorizontalPosAttr
            End Get
            Set(ByVal value As Integer)
                R2WSHorizontalPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' R2PH1 REAGENT2_ARM_RING1 Polar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R2RotorPos() As Integer
            Get
                Return R2RotorPosAttr
            End Get
            Set(ByVal value As Integer)
                R2RotorPosAttr = value
            End Set
        End Property

        ''' <summary>
        ''' R2WVR REAGENT2_ARM_WASH Z
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property R2WSVerticalRefPos() As Integer
            Get
                Return R2WSVerticalRefPosAttr
            End Get
            Set(ByVal value As Integer)
                R2WSVerticalRefPosAttr = value
            End Set
        End Property

        ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics
        Public Property NeedlesDispensationsCount() As Integer
            Get
                Return NeedlesDispensationsCountAttr

            End Get
            Set(ByVal value As Integer)
                NeedlesDispensationsCountAttr = value
            End Set
        End Property



        ''SGM 01/12/2011
        'Public Property Reagent1ParkingZ() As Single
        '    Get
        '        Return Reagent1ParkingZAttr
        '    End Get
        '    Set(ByVal value As Single)
        '        Reagent1ParkingZAttr = value
        '    End Set
        'End Property

        'Public Property Reagent2ParkingZ() As Single
        '    Get
        '        Return Reagent2ParkingZAttr
        '    End Get
        '    Set(ByVal value As Single)
        '        Reagent2ParkingZAttr = value
        '    End Set
        'End Property

        'Public Property Reagent1ParkingPolar() As Single
        '    Get
        '        Return Reagent1ParkingPolarAttr
        '    End Get
        '    Set(ByVal value As Single)
        '        Reagent1ParkingPolarAttr = value
        '    End Set
        'End Property

        'Public Property Reagent2ParkingPolar() As Single
        '    Get
        '        Return Reagent2ParkingPolarAttr
        '    End Get
        '    Set(ByVal value As Single)
        '        Reagent2ParkingPolarAttr = value
        '    End Set
        'End Property
        ''end SGM 01/12/2011



#End Region

#End Region

#Region "Declarations"
        Private RecommendationsReport() As HISTORY_RECOMMENDATIONS
        Private ReportCountTimeout As Integer
#End Region

#Region "Enumerations"

        Public Enum OPERATIONS
            _NONE
            HOMES
            DISPENSING_NEEDLE
            CONDITIONING_NEEDLE
            'NEEDLE_TO_PARKING
            'NEEDLE_BACK_TO_WASHING
            CONDITIONING_HEATER
            CONDITIONING_ROTOR
            ROTATING_ROTOR
            CONDITIONING
            CONDITIONING_MANUAL_UP
            CONDITIONING_MANUAL_DOWN
            MEASURE_TEMPERATURE
            TEST_ADJUSTMENT
            SAVE_ADJUSTMENT
            'WASH_STATION_CTRL
            WASHING_STATION_UP
            WASHING_STATION_DOWN
        End Enum

        Public Enum ResultsOperations
            Unrealized = 0
            OK = 1
            NOK = 2
        End Enum

#End Region

#Region "Constructor"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID) 'SGM 20/01/2012
            myFwScriptDelegate = pFwScriptsDelegate

            MyClass.Initializations()
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub
#End Region

#Region "Event Handlers"
        ''' <summary>
        ''' manages the responses of the Analyzer
        ''' The response can be OK, NG, Timeout or Exception
        ''' </summary>
        ''' <param name="pResponse">response type</param>
        ''' <param name="pData">data received</param>
        ''' <remarks>Created by XBC 17/06/2011</remarks>
        Private Sub ScreenReceptionLastFwScriptEvent(ByVal pResponse As RESPONSE_TYPES, ByVal pData As Object) Handles Me.ReceivedLastFwScriptEvent
            Dim myGlobal As New GlobalDataTO
            Try
                'manage special operations according to the screen characteristics

                ' XBC 05/05/2011 - timeout limit repetitions
                If pResponse = RESPONSE_TYPES.TIMEOUT Or _
                   pResponse = RESPONSE_TYPES.EXCEPTION Then

                    If MyClass.ReportCountTimeout = 0 Then
                        MyClass.ReportCountTimeout += 1
                        ' registering the incidence in historical reports activity
                        If MyClass.RecommendationsReport Is Nothing Then
                            ReDim MyClass.RecommendationsReport(0)
                        Else
                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                        End If
                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_COMM
                    End If

                    Exit Sub
                End If
                ' XBC 05/05/2011 - timeout limit repetitions


                Select Case MyClass.CurrentTestAttr

                    Case ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                        '
                        ' THERMOS PHOTOMETRY
                        '

                        Select Case CurrentOperation

                            ' XBC 20/04/2012
                            Case OPERATIONS.WASHING_STATION_UP
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        MyClass.IsWashingStationUpAttr = True
                                End Select

                                ' XBC 20/04/2012
                            Case OPERATIONS.WASHING_STATION_DOWN
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        MyClass.IsWashingStationUpAttr = False
                                End Select

                            Case OPERATIONS.CONDITIONING_ROTOR
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        'MyClass.ReactionsRotorConditioningDoingAttr = True
                                        'CurrentTimeOperationAttr = myFwScriptDelegate.CurrentFwScriptItem.TimeExpected

                                    Case RESPONSE_TYPES.OK
                                        'MyClass.ReactionsRotorConditioningDoingAttr = False
                                        MyClass.ReactionsRotorHomesDoneAttr = True
                                        If MyClass.RotorConditioningCancelRequested Then 'SGM 02/12/2011
                                            MyClass.IsRotorPrimedAttr = False
                                            MyClass.ReactionsRotorConditioningDoneAttr = False
                                        Else

                                            If MyClass.FillMode = FILL_MODE.AUTOMATIC Then
                                                MyClass.IsRotorPrimedAttr = True
                                                MyClass.ConditioningExecutedAttr(0) = ResultsOperations.OK
                                            End If
                                            'MyClass.ReactionsRotorWellFilledAttr(MyClass.ReactionsRotorCurrentWellAttr - 1) = True
                                            'If AreReactionsRotorAllFilled Then
                                            MyClass.ReactionsRotorConditioningDoneAttr = True
                                            'End If
                                        End If

                                        'start rotating
                                        'MyClass.CurrentOperation = OPERATIONS.ROTATING_ROTOR
                                        'MyClass.SendFwScriptsQueueList(ADJUSTMENT_MODES.TESTING)

                                    Case Else
                                        ' registering the incidence in historical reports activity
                                        MyClass.ConditioningExecutedAttr(0) = ResultsOperations.NOK
                                        If MyClass.RecommendationsReport Is Nothing Then
                                            ReDim MyClass.RecommendationsReport(0)
                                        Else
                                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                        End If
                                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_CONDITIONING_GLF_THERMO
                                End Select


                                'Case OPERATIONS.HOMES
                                '    Select Case pResponse
                                '        Case RESPONSE_TYPES.START
                                '            ' Nothing by now
                                '        Case RESPONSE_TYPES.OK
                                '            MyClass.ReactionsRotorHomesDoneAttr = True
                                '    End Select

                                ' FILL MANUAL MODE
                            Case OPERATIONS.CONDITIONING_MANUAL_UP
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        If MyClass.RotorConditioningCancelRequested Then 'SGM 02/12/2011
                                            MyClass.IsRotorPrimedAttr = False
                                            MyClass.ReactionsRotorConditioningDoneAttr = False
                                        Else
                                            MyClass.ReactionsRotorConditioningManualFirstStepDoneAttr = True
                                        End If

                                    Case Else
                                        ' registering the incidence in historical reports activity
                                        MyClass.ConditioningExecutedAttr(0) = ResultsOperations.NOK
                                        If MyClass.RecommendationsReport Is Nothing Then
                                            ReDim MyClass.RecommendationsReport(0)
                                        Else
                                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                        End If
                                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_CONDITIONING_GLF_THERMO
                                End Select

                                ' FILL MANUAL MODE
                            Case OPERATIONS.CONDITIONING_MANUAL_DOWN
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        If MyClass.RotorConditioningCancelRequested Then 'SGM 02/12/2011
                                            MyClass.IsRotorPrimedAttr = False
                                            MyClass.ReactionsRotorConditioningDoneAttr = False
                                        Else
                                            MyClass.ReactionsRotorConditioningManualSecondStepDoneAttr = True
                                            MyClass.ReactionsRotorConditioningDoneAttr = True
                                            MyClass.ConditioningExecutedAttr(0) = ResultsOperations.OK
                                        End If

                                    Case Else
                                        ' registering the incidence in historical reports activity
                                        MyClass.ConditioningExecutedAttr(0) = ResultsOperations.NOK
                                        If MyClass.RecommendationsReport Is Nothing Then
                                            ReDim MyClass.RecommendationsReport(0)
                                        Else
                                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                        End If
                                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_CONDITIONING_GLF_THERMO
                                End Select

                            Case OPERATIONS.ROTATING_ROTOR
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        'MyClass.ReactionsRotorRotatingDoingAttr(MyClass.ReactionsRotorRotationsNumAttr - 1) = True
                                        'CurrentTimeOperationAttr = myFwScriptDelegate.CurrentFwScriptItem.TimeExpected

                                    Case RESPONSE_TYPES.OK
                                        If MyClass.RotorRotatingCancelRequested Then 'SGM 02/12/2011
                                            MyClass.IsKeepRotating = False
                                            MyClass.IsKeepRotatingStoppedAttr = True
                                        Else
                                            If Not MyClass.ReactionsRotorRotatingDoneAttr And Not MyClass.IsKeepRotatingAttr Then
                                                MyClass.ReactionsRotorRotationsDoneAttr(MyClass.ReactionsRotorCurrentRotationAttr - 1) = True
                                                If MyClass.ReactionsRotorCurrentRotationAttr = MyClass.ReactionsRotorRotatingTimesAttr Then
                                                    MyClass.ReactionsRotorRotatingDoneAttr = True
                                                End If
                                            ElseIf MyClass.ReactionsRotorRotatingDoneAttr And MyClass.IsKeepRotatingAttr Then

                                                MyClass.SendQueueForROTATE_ROTOR()

                                                If MyClass.NoneInstructionToSend Then
                                                    ' Send FwScripts
                                                    myGlobal = myFwScriptDelegate.StartFwScriptQueue
                                                End If

                                            ElseIf MyClass.ReactionsRotorRotatingDoneAttr And Not MyClass.IsKeepRotating Then
                                                MyClass.IsKeepRotatingStoppedAttr = True
                                            End If
                                        End If
                                        'MyClass.RotatingExecutedAttr = ResultsOperations.OK
                                        'MyClass.ConditioningExecutedAttr(0) = ResultsOperations.OK 

                                    Case Else
                                        ' registering the incidence in historical reports activity
                                        MyClass.RotatingExecutedAttr = ResultsOperations.NOK
                                        If MyClass.RecommendationsReport Is Nothing Then
                                            ReDim MyClass.RecommendationsReport(0)
                                        Else
                                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                        End If
                                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_CONDITIONING_GLF_THERMO
                                End Select

                                'Case OPERATIONS.MEASURE_TEMPERATURE
                                '    Select Case pResponse
                                '        Case RESPONSE_TYPES.START
                                '            MyClass.ReactionsRotorMeasuredTempDoingAttr = True
                                '            CurrentTimeOperationAttr = myFwScriptDelegate.CurrentFwScriptItem.TimeExpected
                                '        Case RESPONSE_TYPES.OK
                                '            MyClass.ReactionsRotorMeasuredTempDoingAttr = False
                                '            MyClass.ReactionsRotorPrepareWellDoneAttr(MyClass.ReactionsRotorCurrentWellAttr - 1) = True
                                '            If AreReactionsRotorAllPrepared Then
                                '                MyClass.ReactionsRotorMeasuredTempDoneAttr = True
                                '            End If
                                '            MyClass.FinalResultAttr(0) = ResultsOperations.NOK
                                '            MyClass.MeasuringTempExecutedAttr(0) = ResultsOperations.OK
                                '        Case Else
                                '            ' registering the incidence in historical reports activity
                                '            MyClass.MeasuringTempExecutedAttr(0) = ResultsOperations.NOK
                                '            If MyClass.RecommendationsReport Is Nothing Then
                                '                ReDim MyClass.RecommendationsReport(0)
                                '            Else
                                '                ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                '            End If
                                '            MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_MEASURING_GLF_THERMO
                                '    End Select

                            Case OPERATIONS.TEST_ADJUSTMENT
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        MyClass.ReactionsRotorTestAdjustmentDoneAttr = True
                                        MyClass.TestTempExecutedAttr(0) = ResultsOperations.OK
                                    Case Else
                                        MyClass.TestTempExecutedAttr(0) = ResultsOperations.NOK
                                End Select


                            Case OPERATIONS.SAVE_ADJUSTMENT
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        MyClass.LoadAdjDoneAttr = True
                                        MyClass.AssignedNewValuesAttr(0) = ResultsOperations.OK
                                End Select

                        End Select

                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1, _
                         ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                        '
                        ' THERMOS REAGENT1 or REAGENT2
                        '

                        Select Case CurrentOperation

                            ''SGM 01/12/2011
                            'Case OPERATIONS.NEEDLE_TO_PARKING
                            '    Select Case pResponse
                            '        Case RESPONSE_TYPES.START
                            '            'nothing by now

                            '        Case RESPONSE_TYPES.OK
                            '            'the needle is in parking are conditioned (primed)
                            '            Select Case MyClass.CurrentTest
                            '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1 : MyClass.IsReagentArm1InParking = True
                            '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2 : MyClass.IsReagentArm1InParking = True
                            '            End Select

                            '    End Select


                            'Case OPERATIONS.NEEDLE_BACK_TO_WASHING
                            '    Select Case pResponse
                            '        Case RESPONSE_TYPES.START
                            '            'nothing by now

                            '        Case RESPONSE_TYPES.OK
                            '            'the needle is in parking are conditioned (primed)
                            '            Select Case MyClass.CurrentTest
                            '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1 : MyClass.IsReagentArm1InParking = False
                            '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2 : MyClass.IsReagentArm1InParking = False
                            '            End Select

                            '    End Select

                            '    'end SGM 01/12/2011

                            Case OPERATIONS.CONDITIONING_NEEDLE
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        'nothing by now

                                    Case RESPONSE_TYPES.OK
                                        'the needles are conditioned (primed)
                                        'MyClass.ReagentNeedleConditioningDoneAttr = True
                                        Select Case MyClass.CurrentTest
                                            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1 : MyClass.Reagent1NeedlePrimingDoneAttr = True
                                            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2 : MyClass.Reagent2NeedlePrimingDoneAttr = True
                                        End Select

                                        ' registering the incidence in historical reports activity
                                        MyClass.ConditioningExecutedAttr(1) = ResultsOperations.OK

                                    Case Else
                                        'error while conditioning needles
                                        ' registering the incidence in historical reports activity
                                        MyClass.ConditioningExecutedAttr(1) = ResultsOperations.NOK
                                        If MyClass.RecommendationsReport Is Nothing Then
                                            ReDim MyClass.RecommendationsReport(0)
                                        Else
                                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                        End If
                                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_CONDITIONING_REA_THERMO

                                End Select

                            Case OPERATIONS.DISPENSING_NEEDLE
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        'Nothing by now

                                    Case RESPONSE_TYPES.OK
                                        'a dispensation into the reagent washing station has been performed
                                        MyClass.ReagentNeedleDispensingDoneAttr = True

                                        ' registering the incidence in historical reports activity
                                        MyClass.ReagentNeedleMeasuredTempDoneAttr = True
                                        MyClass.MeasuringTempExecutedAttr(1) = ResultsOperations.OK

                                    Case Else
                                        'error while a dispensation into the reagent washing station has been performed
                                        ' registering the incidence in historical reports activity
                                        MyClass.MeasuringTempExecutedAttr(1) = ResultsOperations.NOK
                                        If MyClass.RecommendationsReport Is Nothing Then
                                            ReDim MyClass.RecommendationsReport(0)
                                        Else
                                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                        End If
                                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_MEASURING_REA_THERMO
                                End Select


                                'Case OPERATIONS.MEASURE_TEMPERATURE
                                '    Select Case pResponse
                                '        Case RESPONSE_TYPES.START
                                '            MyClass.ReagentNeedleMeasuredTempDoingAttr = True
                                '            CurrentTimeOperationAttr = myFwScriptDelegate.CurrentFwScriptItem.TimeExpected
                                '        Case RESPONSE_TYPES.OK
                                '            MyClass.ReagentNeedleMeasuredTempDoingAttr = False
                                '            MyClass.ReagentNeedleMeasuredTempDoneAttr = True
                                '            MyClass.MeasuringTempExecutedAttr(1) = ResultsOperations.OK
                                '        Case Else
                                '            MyClass.MeasuringTempExecutedAttr(1) = ResultsOperations.NOK
                                '            If MyClass.RecommendationsReport Is Nothing Then
                                '                ReDim MyClass.RecommendationsReport(0)
                                '            Else
                                '                ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                '            End If
                                '            MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_MEASURING_REA_THERMO
                                '    End Select

                            Case OPERATIONS.TEST_ADJUSTMENT
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        MyClass.ReagentNeedleTestAdjustmentDoneAttr = True
                                        MyClass.TestTempExecutedAttr(1) = ResultsOperations.OK
                                    Case Else
                                        MyClass.TestTempExecutedAttr(1) = ResultsOperations.NOK
                                End Select


                            Case OPERATIONS.SAVE_ADJUSTMENT
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        MyClass.LoadAdjDoneAttr = True
                                        MyClass.AssignedNewValuesAttr(1) = ResultsOperations.OK
                                End Select


                        End Select


                    Case ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                        '
                        ' THERMOS WASHING STATION HEATER
                        '

                        Select Case CurrentOperation


                            ' XBC 20/04/2012
                            Case OPERATIONS.WASHING_STATION_UP
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        MyClass.IsWashingStationUpAttr = True
                                End Select

                                ' XBC 20/04/2012
                            Case OPERATIONS.WASHING_STATION_DOWN
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        MyClass.IsWashingStationUpAttr = False
                                End Select

                            Case OPERATIONS.CONDITIONING_HEATER
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        'Nothing by Now

                                    Case RESPONSE_TYPES.OK
                                        'The WS heater has been conditioned
                                        MyClass.HeaterConditioningDoingAttr(MyClass.HeaterConditioningNumDispAttr - 1) = True
                                        If MyClass.HeaterConditioningNumDispAttr = MyClass.HeaterMaxActionsTemperAttr Then
                                            MyClass.HeaterConditioningDoneAttr = True
                                            ' registering the incidence in historical reports activity
                                            MyClass.ConditioningExecutedAttr(2) = ResultsOperations.OK
                                        End If


                                    Case Else
                                        'error while conditioning WS heater
                                        ' registering the incidence in historical reports activity
                                        MyClass.ConditioningExecutedAttr(2) = ResultsOperations.NOK
                                        If MyClass.RecommendationsReport Is Nothing Then
                                            ReDim MyClass.RecommendationsReport(0)
                                        Else
                                            ReDim Preserve MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport) + 1)
                                        End If
                                        MyClass.RecommendationsReport(UBound(MyClass.RecommendationsReport)) = HISTORY_RECOMMENDATIONS.ERR_CONDITIONING_HEA_THERMO
                                End Select

                                'Case OPERATIONS.MEASURE_TEMPERATURE
                                '    ' Heater no need Measurement operations

                            Case OPERATIONS.TEST_ADJUSTMENT
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        MyClass.HeaterTestAdjustmentDoneAttr = True
                                        MyClass.TestTempExecutedAttr(2) = ResultsOperations.OK
                                    Case Else
                                        MyClass.TestTempExecutedAttr(2) = ResultsOperations.NOK
                                End Select


                            Case OPERATIONS.SAVE_ADJUSTMENT
                                Select Case pResponse
                                    Case RESPONSE_TYPES.START
                                        ' Nothing by now
                                    Case RESPONSE_TYPES.OK
                                        MyClass.LoadAdjDoneAttr = True
                                        MyClass.AssignedNewValuesAttr(2) = ResultsOperations.OK
                                End Select


                        End Select


                End Select

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.ScreenReceptionLastFwScriptEvent", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Create the corresponding Script list according to the Screen Mode
        ''' </summary>
        ''' <param name="pMode">Screen mode</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: XBC 15/06/11
        ''' </remarks>
        Public Function SendFwScriptsQueueList(ByVal pMode As ADJUSTMENT_MODES) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                ' Create the list of Scripts which are need to initialize this Adjustment
                Select Case pMode
                    Case ADJUSTMENT_MODES.ADJUSTMENTS_READING
                        myResultData = MyBase.SendQueueForREADINGADJUSTMENTS()


                    Case ADJUSTMENT_MODES.TESTING

                        Select Case MyClass.CurrentOperation


                            'Case OPERATIONS.CONDITIONING
                            '    ', _
                            '    'OPERATIONS.CONDITIONING_MANUAL_ONE, _
                            '    'OPERATIONS.CONDITIONING_MANUAL_TWO()

                            '    myResultData = MyClass.SendQueueForCONDIGIONING_SYSTEM(MyClass.CurrentTest)

                            Case OPERATIONS.CONDITIONING_HEATER
                                myResultData = MyClass.SendQueueForWS_HEATER_TEST

                            Case OPERATIONS.CONDITIONING_ROTOR
                                myResultData = MyClass.SendQueueForCONDITIONING_ROTOR

                            Case OPERATIONS.ROTATING_ROTOR
                                myResultData = MyClass.SendQueueForROTATE_ROTOR

                            Case OPERATIONS.CONDITIONING_NEEDLE
                                myResultData = MyClass.SendQueueForPRIME_NEEDLE()

                            Case OPERATIONS.DISPENSING_NEEDLE
                                myResultData = MyClass.SendQueueForDISPENSE_NEEDLE()

                                'Case OPERATIONS.MEASURE_TEMPERATURE
                                '    myResultData = MyClass.SendQueueForMEASURE_TEMPERATURE(MyClass.CurrentTest)

                            Case OPERATIONS.TEST_ADJUSTMENT
                                myResultData = MyClass.SendQueueForTEST_ADJUSTMENT(MyClass.CurrentTest)

                                '    'SGM 01/12/2011
                                'Case OPERATIONS.NEEDLE_TO_PARKING
                                '    myResultData = MyClass.SendQueueForNeedleToParking

                                'Case OPERATIONS.NEEDLE_BACK_TO_WASHING
                                '    myResultData = MyClass.SendQueueForNeedleToWashing
                                '    'end SGM 01/12/2011

                        End Select


                End Select
            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentsDelegate.SendFwScriptsQueueList", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Get the Thermo Parameters
        ''' </summary>
        ''' <param name="pAnalyzerModel">Identifier of the Analyzer model which the data will be obtained</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 29/06/2011</remarks>
        Public Function GetParameters(ByVal pAnalyzerModel As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            Dim myParametersDS As New ParametersDS
            Dim myWell As Integer
            Try
                ' Max Rotor Wells
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_WELLS_REACTIONS_ROTOR.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.ReactionsRotorMaxWellsAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If
                ' Num Wells to Measure in Reactions Rotor
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_WELLS_TO_MEASURE_GLF_THERMO.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.ReactionsRotorMeasuredWellsCountAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Wells to Fill in self mode Reactions Rotor
                MyClass.ReactionsRotorMeasuredWellsAttr = New List(Of Integer)

                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_WELL1_FILL_GLF_THERMO.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    'IT 23/07/2015 - BA-2649
                    If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                        MyClass.ReactionsRotorMeasuredWellsAttr.Add(CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric))
                    End If
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_WELL2_FILL_GLF_THERMO.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    'IT 23/07/2015 - BA-2649
                    If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                        MyClass.ReactionsRotorMeasuredWellsAttr.Add(CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric))
                    End If
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_WELL3_FILL_GLF_THERMO.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    'IT 23/07/2015 - BA-2649
                    If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                        MyClass.ReactionsRotorMeasuredWellsAttr.Add(CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric))
                    End If
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_WELL4_FILL_GLF_THERMO.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    'IT 23/07/2015 - BA-2649
                    If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                        MyClass.ReactionsRotorMeasuredWellsAttr.Add(CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric))
                    End If
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Define wells to fill - for each specified well, always 2 wells besides
                MyClass.ReactionsRotorWellsToFillAttr = New List(Of Integer)
                For i As Integer = 0 To MyClass.ReactionsRotorMeasuredWellsCountAttr - 1
                    ' Well to fill
                    myWell = MyClass.ReactionsRotorMeasuredWellsAttr(i)
                    MyClass.ReactionsRotorWellsToFillAttr.Add(myWell)
                Next

                MyClass.ReactionsRotorWellsFilledCountAttr = ReactionsRotorMeasuredWellsCountAttr

                'times that the rotor must be rotating
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MIN_ROTOR_ROTATING_TIMES.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.ReactionsRotorRotatingTimesAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                'number of primes needed for conditioninng rotor
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_PRIME_GLF_THERMO.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.ReactionsRotorMaxPrimesAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If


                ' Max Number of dispensations to heat R1 and R2 Needles
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_DISP_CONDITIONING_REAGENT.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.ReagentNeedleHeatMaxDispensationsAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Well source where to take distilled water to dispense on external auxiliary device
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_WELL_SOURCE_REAGENT_THERMO.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.ReagentNeedleSourceWellAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                ' Max Number of "manchadas" of a pump to temper Washing Station Heater
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_MAX_ACT_CONDITIONING_HEATER.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.HeaterMaxActionsTemperAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

                'SGM 18/09/2012 - Seconds that takes the buzzer sound when Rotor conditioning has been finished
                myResultData = myParams.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SRV_SOUND_THERMO.ToString, pAnalyzerModel)
                If Not myResultData.HasError And Not myResultData.SetDatos Is Nothing Then
                    myParametersDS = CType(myResultData.SetDatos, ParametersDS)
                    MyClass.RotorSoundSecondsAttr = CInt(myParametersDS.tfmwSwParameters.Item(0).ValueNumeric)
                Else
                    myResultData.HasError = True
                    Exit Try
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.GetParameters", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Refresh this specified delegate with the information received from the Instrument
        ''' </summary>
        ''' <param name="pRefreshEventType"></param>
        ''' <param name="pRefreshDS"></param>
        ''' <remarks>Created by XBC 29/06/2011</remarks>
        Public Sub RefreshDelegate(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS)
            Dim myResultData As New GlobalDataTO
            Try
                MyClass.ScreenReceptionLastFwScriptEvent(RESPONSE_TYPES.OK, Nothing)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.RefreshDelegate", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Load Adjustments High Level Instruction to save values into the instrument
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 20/06/2011</remarks>
        Public Function SendLOAD_ADJUSTMENTS() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                MyClass.CurrentOperation = OPERATIONS.SAVE_ADJUSTMENT
                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.LOADADJ, True, Nothing, MyClass.pValueAdjustAttr) '#REFACTORING

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendLOAD_ADJUSTMENTS", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Load Adjustments High Level Instruction to move Washing Station
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 28/06/2011</remarks>
        Public Function SendWASH_STATION_CTRL(ByVal pAction As Ax00WashStationControlModes) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Select Case pAction
                    Case Ax00WashStationControlModes.UP
                        MyClass.CurrentOperation = OPERATIONS.CONDITIONING_MANUAL_UP
                    Case Ax00WashStationControlModes.DOWN
                        MyClass.CurrentOperation = OPERATIONS.CONDITIONING_MANUAL_DOWN
                End Select
                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, pAction) '#REFACTORING

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendWASH_STATION_CTRL", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Load Adjustments High Level Instruction to move Washing Station
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 20/04/2012</remarks>
        Public Function SendWASH_STATION_CTRL2(ByVal pAction As Ax00WashStationControlModes) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Select Case pAction
                    Case Ax00WashStationControlModes.UP
                        MyClass.CurrentOperation = OPERATIONS.WASHING_STATION_UP
                    Case Ax00WashStationControlModes.DOWN
                        MyClass.CurrentOperation = OPERATIONS.WASHING_STATION_DOWN
                End Select
                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, pAction) '#REFACTORING

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendWASH_STATION_CTRL2", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' NRotor High Level Instruction
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XB 14/10/2014 - Use NROTOR instead WSCTRL when Wash Station is down - BA-2004</remarks>
        Public Function SendNEW_ROTOR(ByVal pCurrentOperation As OPERATIONS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myParams As New List(Of String)
            Try

                CurrentOperation = pCurrentOperation

                myResultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.NROTOR, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendNEW_ROTOR", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        '''' <summary>
        '''' Needles arms to Parking
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SGM 01/12/2011</remarks>
        'Private Function SendQueueForNeedleToParking() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myListFwScript As New List(Of FwScriptQueueItem)
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Dim myFwScript2 As New FwScriptQueueItem
        '    Dim myMovZ As Single
        '    Dim myMovPolar As Single

        '    Try
        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        With myFwScript1
        '            Select Case MyClass.CurrentTest
        '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
        '                    .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_UP.ToString
        '                    myMovZ = CSng(Me.Reagent1ParkingZ)

        '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
        '                    .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_UP.ToString
        '                    myMovZ = CSng(Me.Reagent2ParkingZ)
        '            End Select

        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = myFwScript2
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            ' expects 2 params
        '            .ParamList = New List(Of String)
        '            .ParamList.Add(myMovZ.ToString)

        '            myListFwScript.Add(myFwScript1)

        '        End With

        '        With myFwScript2
        '            Select Case MyClass.CurrentTest
        '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
        '                    .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_POLAR.ToString
        '                    myMovPolar = CSng(Me.Reagent1ParkingPolar)

        '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
        '                    .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_POLAR.ToString
        '                    myMovPolar = CSng(Me.Reagent2ParkingPolar)
        '            End Select

        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            ' expects 2 params
        '            .ParamList = New List(Of String)
        '            .ParamList.Add(myMovPolar.ToString)

        '            myListFwScript.Add(myFwScript2)

        '        End With



        '        'add to the queue list
        '        If myListFwScript.Count > 0 Then
        '            For i As Integer = 0 To myListFwScript.Count - 1
        '                If i = 0 Then
        '                    ' First Script
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
        '                Else
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
        '                End If
        '            Next
        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
        '        Else
        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '        End If


        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendQueueForNeedleToParking", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function


        '''' <summary>
        '''' Needles arms back to Washing
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SGM 01/12/2011</remarks>
        'Private Function SendQueueForNeedleToWashing() As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myListFwScript As New List(Of FwScriptQueueItem)
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Dim myFwScript2 As New FwScriptQueueItem
        '    Dim myMovZ As Single
        '    Dim myMovPolar As Single

        '    Try
        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        With myFwScript1
        '            Select Case MyClass.CurrentTest
        '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
        '                    .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_UP.ToString
        '                    myMovPolar = CSng(Me.R1WSHorizontalPos)

        '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
        '                    .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_UP.ToString
        '                    myMovPolar = CSng(Me.R2WSHorizontalPos)
        '            End Select

        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = myFwScript2
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            ' expects 2 params
        '            .ParamList = New List(Of String)
        '            .ParamList.Add(myMovPolar.ToString)

        '            myListFwScript.Add(myFwScript1)

        '        End With

        '        With myFwScript2
        '            Select Case MyClass.CurrentTest
        '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
        '                    .FwScriptID = FwSCRIPTS_IDS.REAGENT1_ARM_ABS_UP.ToString
        '                    myMovZ = CSng(Me.R1VerticalSafetyPos)

        '                Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
        '                    .FwScriptID = FwSCRIPTS_IDS.REAGENT2_ARM_ABS_UP.ToString
        '                    myMovZ = CSng(Me.R2VerticalSafetyPos)
        '            End Select

        '            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '            .EvaluateValue = 1
        '            .NextOnResultOK = Nothing
        '            .NextOnResultNG = Nothing
        '            .NextOnTimeOut = Nothing
        '            .NextOnError = Nothing
        '            ' expects 2 params
        '            .ParamList = New List(Of String)
        '            .ParamList.Add(myMovZ.ToString)

        '            myListFwScript.Add(myFwScript2)

        '        End With





        '        'add to the queue list
        '        If myListFwScript.Count > 0 Then
        '            For i As Integer = 0 To myListFwScript.Count - 1
        '                If i = 0 Then
        '                    ' First Script
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
        '                Else
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
        '                End If
        '            Next
        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
        '        Else
        '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '        End If


        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendQueueForNeedleToWashing", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function


        Public Function CalculateThermoCorrection(ByVal pSetPointTemp As Single, ByVal pTargetTemp As Single) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myCalc As New CalculationsDelegate()
            Try

                myResultData = myCalc.CalculateThermoCorrection(pSetPointTemp, pTargetTemp)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.CalculateThermoCorrection", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Public Function CalculateThermoSetPoint(ByVal pSetPointTemp As Single, ByVal pTargetTemp As Single, ByVal pMeasuredTemp As Single) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myCalc As New CalculationsDelegate()
            Try

                myResultData = myCalc.CalculateThermoSetPoint(pSetPointTemp, pTargetTemp, pMeasuredTemp)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.CalculateThermoSetPoint", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        Public Sub Initializations()
            ' Next parameters to control operations in each thermo option (0=Photometry 1=Reagents 2=Heater)
            ReDim MyClass.FinalResultAttr(2)
            For i As Integer = 0 To MyClass.FinalResultAttr.Length - 1
                MyClass.FinalResultAttr(i) = ResultsOperations.NOK
            Next
            ReDim MyClass.AssignedNewValuesAttr(2)
            For i As Integer = 0 To MyClass.AssignedNewValuesAttr.Length - 1
                MyClass.AssignedNewValuesAttr(i) = ResultsOperations.NOK
            Next
            ReDim MyClass.ConditioningExecutedAttr(2)
            For i As Integer = 0 To MyClass.ConditioningExecutedAttr.Length - 1
                MyClass.ConditioningExecutedAttr(i) = ResultsOperations.Unrealized
            Next
            ReDim MyClass.TestTempExecutedAttr(2)
            For i As Integer = 0 To MyClass.TestTempExecutedAttr.Length - 1
                MyClass.TestTempExecutedAttr(i) = ResultsOperations.Unrealized
            Next
            ReDim MyClass.MeasuringTempExecutedAttr(2)
            For i As Integer = 0 To MyClass.MeasuringTempExecutedAttr.Length - 1
                MyClass.MeasuringTempExecutedAttr(i) = ResultsOperations.NOK
            Next

            MyClass.RecommendationsReport = Nothing
            MyClass.ReportCountTimeout = 0

            ' XBC 22/11/2011 - Counter placed to Delegate to register it into Historics
            MyClass.NeedlesDispensationsCountAttr = 0
        End Sub

        ''' <summary>
        ''' Method to decode the data information of the this screen from a String format source and obtain the data information easily legible
        ''' </summary>
        ''' <param name="pTask">task identifier</param>
        ''' <param name="pAction">task's action identifier</param>
        ''' <param name="pData">content data with the information to format</param>
        ''' <param name="pcurrentLanguage">language identifier to localize contents</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 29/08/2011</remarks>
        Public Function DecodeDataReport(ByVal pTask As String, ByVal pAction As String, ByVal pData As String, ByVal pcurrentLanguage As String) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                'Dim myUtility As New Utilities()
                Dim text1 As String
                Dim text As String = ""

                Select Case pTask
                    Case "TEST"

                        Select Case pAction
                            Case "GLF_TER"

                                Dim j As Integer = 0
                                ' Final Result
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FinalResult", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Filling option used for the test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FILL_OPTION", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_AUTOMATIC_FILL", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MANUALLY_FILL", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Conditioning option executed
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ConditioningOperation", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                ElseIf pData.Substring(j, 1) = "2" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NotExecuted", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' SetPoint temperature test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TestOperation", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                ElseIf pData.Substring(j, 1) = "2" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NotExecuted", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                text += Environment.NewLine
                                ' Temperature measured points 1 to 4
                                For i As Integer = 0 To 3
                                    ' Check if temperature has been measured
                                    Dim measuredTemp As Integer
                                    measuredTemp = CInt(pData.Substring(j, 1))
                                    j += 1

                                    If measuredTemp = 1 Then
                                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEMP_POINT", pcurrentLanguage) + (i + 1).ToString + ": "

                                        ' alignment...
                                        text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                        text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                        text += Utilities.FormatLineHistorics(text1)
                                    End If
                                    j += 5
                                Next

                                text += Environment.NewLine
                                ' Mean Temperature measured 
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MeanTemp", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)
                                j += 5

                                ' Sensor Temperature received
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SensorTemp", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)

                            Case "REG1_TER", "REG2_TER"

                                Dim j As Integer = 0
                                ' Final Result
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FinalResult", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Selected Arm
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SelectedArm", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Conditioning option executed
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ConditioningOperation", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                ElseIf pData.Substring(j, 1) = "2" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NotExecuted", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' SetPoint temperature test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TestOperation", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                ElseIf pData.Substring(j, 1) = "2" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NotExecuted", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Check if temperature has been measured
                                Dim measuredTemp As Integer
                                measuredTemp = CInt(pData.Substring(j, 1))
                                j += 1

                                text += Environment.NewLine
                                ' Temperature measured
                                If measuredTemp = 1 Then
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEMP", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                    text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                    text += Utilities.FormatLineHistorics(text1)
                                End If
                                j += 5

                                ' Sensor Temperature received
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SensorTemp", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)

                                ' XBC 22/11/2011 - register Counter into Historics
                                j += 5

                                ' Number of Dispesations executed
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NumDispensations", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 2)).ToString().Length)
                                text1 += CSng(pData.Substring(j, 2)).ToString()

                                text += Utilities.FormatLineHistorics(text1)
                                j += 2
                                ' XBC 22/11/2011 - register Counter into Historics

                            Case "HEAT_TER"

                                Dim j As Integer = 0
                                ' Final Result
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FinalResult", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Conditioning option executed
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ConditioningOperation", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                ElseIf pData.Substring(j, 1) = "2" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NotExecuted", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' SetPoint temperature test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TestOperation", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                ElseIf pData.Substring(j, 1) = "2" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NotExecuted", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Check if temperature has been measured
                                Dim measuredTemp As Integer
                                measuredTemp = CInt(pData.Substring(j, 1))
                                j += 1

                                text += Environment.NewLine
                                ' Temperature measured
                                If measuredTemp = 1 Then
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEMP", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                    text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                    text += Utilities.FormatLineHistorics(text1)
                                End If
                                j += 5

                                ' Sensor Temperature received
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SensorTemp", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)

                        End Select


                    Case "ADJUST"

                        Select Case pAction
                            Case "GLF_TER"

                                Dim j As Integer = 0
                                ' Final Result
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FinalResult", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' New Assigned values
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NewAssigValues", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_YES", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NO", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Filling option used for the test
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FILL_OPTION", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_AUTOMATIC_FILL", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MANUALLY_FILL", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Conditioning option executed
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ConditioningOperation", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                ElseIf pData.Substring(j, 1) = "2" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NotExecuted", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                text += Environment.NewLine
                                ' Temperature measured points 1 to 4
                                For i As Integer = 0 To 3
                                    ' Check if temperature has been measured
                                    Dim measuredTemp As Integer
                                    measuredTemp = CInt(pData.Substring(j, 1))
                                    j += 1

                                    If measuredTemp = 1 Then
                                        text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEMP_POINT", pcurrentLanguage) + (i + 1).ToString + ": "

                                        ' alignment...
                                        text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                        text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                        text += Utilities.FormatLineHistorics(text1)
                                    End If
                                    j += 5
                                Next

                                text += Environment.NewLine
                                ' Mean Temperature measured 
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_MeanTemp", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)
                                j += 5

                                ' Sensor Temperature received
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SensorTemp", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)
                                j += 5

                                ' Final Correction Adjustment
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CorrectionAdj", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 6)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 6)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)

                            Case "REG1_TER", "REG2_TER"

                                Dim j As Integer = 0
                                ' Final Result
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FinalResult", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Selected Arm
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SelectedArm", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent1", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Reagent2", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' New Assigned values
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NewAssigValues", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_YES", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NO", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Conditioning option executed
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ConditioningOperation", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                ElseIf pData.Substring(j, 1) = "2" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NotExecuted", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Check if temperature has been measured
                                Dim measuredTemp As Integer
                                measuredTemp = CInt(pData.Substring(j, 1))
                                j += 1

                                text += Environment.NewLine
                                ' Temperature measured
                                If measuredTemp = 1 Then
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEMP", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                    text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                    text += Utilities.FormatLineHistorics(text1)
                                End If
                                j += 5

                                ' Sensor Temperature received
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SensorTemp", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)
                                j += 5

                                ' Final Correction Adjustment
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CorrectionAdj", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 6)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 6)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)

                                ' XBC 22/11/2011 - register Counter into Historics
                                j += 6

                                ' Number of Dispesations executed
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NumDispensations", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 2)).ToString().Length)
                                text1 += CSng(pData.Substring(j, 2)).ToString()

                                text += Utilities.FormatLineHistorics(text1)
                                j += 2
                                ' XBC 22/11/2011 - register Counter into Historics

                            Case "HEAT_TER"

                                Dim j As Integer = 0
                                ' Final Result
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_FinalResult", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' New Assigned values
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NewAssigValues", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_YES", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NO", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Conditioning option executed
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_ConditioningOperation", pcurrentLanguage) + ": "
                                If pData.Substring(j, 1) = "1" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_OK", pcurrentLanguage)
                                ElseIf pData.Substring(j, 1) = "2" Then
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_RES_KO", pcurrentLanguage)
                                Else
                                    text1 += myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_NotExecuted", pcurrentLanguage)
                                End If
                                text += Utilities.FormatLineHistorics(text1)
                                j += 1

                                ' Check if temperature has been measured
                                Dim measuredTemp As Integer
                                measuredTemp = CInt(pData.Substring(j, 1))
                                j += 1

                                text += Environment.NewLine
                                ' Temperature measured
                                If measuredTemp = 1 Then
                                    text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TEMP", pcurrentLanguage) + ": "

                                    ' alignment...
                                    text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                    text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                    text += Utilities.FormatLineHistorics(text1)
                                End If
                                j += 5

                                ' Sensor Temperature received
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SensorTemp", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 5)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 5)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)
                                j += 5

                                ' Final Correction Adjustment
                                text1 = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_CorrectionAdj", pcurrentLanguage) + ": "

                                ' alignment...
                                text1 += Utilities.SetSpaces(40 - text1.Length - 1 - CSng(pData.Substring(j, 6)).ToString("00.00").Length)
                                text1 += CSng(pData.Substring(j, 6)).ToString("00.00")

                                text += Utilities.FormatLineHistorics(text1)

                        End Select

                End Select

                myResultData.SetDatos = text

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.DecodeDataReport", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function
#End Region

#Region "Private Methods"
        '''' <summary>
        '''' Creates the Script List for Screen Conditioning System operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 16/06/2011</remarks>
        'Private Function SendQueueForCONDIGIONING_SYSTEM(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myListFwScript As New List(Of FwScriptQueueItem)
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Dim myFwScript2 As New FwScriptQueueItem
        '    Try
        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        'get the pending Homes  
        '        Dim myHomes As New tadjPreliminaryHomesDAO
        '        Dim myHomesDS As SRVPreliminaryHomesDS
        '        myResultData = myHomes.GetPreliminaryHomes(Nothing, pAdjustment.ToString)
        '        If myResultData IsNot Nothing AndAlso Not myResultData.HasError Then
        '            myHomesDS = CType(myResultData.SetDatos, SRVPreliminaryHomesDS)

        '            Dim myPendingHomesList As List(Of SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow) = _
        '                            (From a As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myHomesDS.srv_tadjPreliminaryHomes _
        '                            Where a.AdjustmentGroupID = pAdjustment.ToString And a.Done = False Select a).ToList

        '            For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
        '                Dim myFwScript As New FwScriptQueueItem
        '                myListFwScript.Add(myFwScript)
        '            Next

        '            Dim i As Integer = 0
        '            For Each H As SRVPreliminaryHomesDS.srv_tadjPreliminaryHomesRow In myPendingHomesList
        '                'GET EACH PENDING HOME'S FWSCRIPT FROM FWSCRIPT DATA AND ADD TO THE FWSCRIPT QUEUE
        '                If i = myListFwScript.Count - 1 Then
        '                    'Last index
        '                    With myListFwScript(i)
        '                        .FwScriptID = H.RequiredHomeID.ToString
        '                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                        .EvaluateValue = 1
        '                        .NextOnResultOK = myFwScript1
        '                        .NextOnResultNG = Nothing
        '                        .NextOnTimeOut = myFwScript1
        '                        .NextOnError = Nothing
        '                        .ParamList = Nothing
        '                    End With
        '                Else
        '                    With myListFwScript(i)
        '                        .FwScriptID = H.RequiredHomeID.ToString
        '                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                        .EvaluateValue = 1
        '                        .NextOnResultOK = myListFwScript(i + 1)
        '                        .NextOnResultNG = Nothing
        '                        .NextOnTimeOut = myListFwScript(i + 1)
        '                        .NextOnError = Nothing
        '                        .ParamList = Nothing
        '                    End With
        '                End If
        '                i += 1
        '            Next

        '        End If


        '        Select Case pAdjustment

        '            Case ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
        '                ' 
        '                ' THERMOS PHOTOMETRY
        '                '

        '                Select Case MyClass.FillModeAttr
        '                    Case FILL_MODE.AUTOMATIC
        '                        ' Well filled by Instrument With Washing Station

        '                        ' Well to fill
        '                        Dim myWell As Integer
        '                        myWell = MyClass.ReactionsRotorWellsToFillAttr(MyClass.ReactionsRotorCurrentWellAttr - 1)
        '                        MyClass.ReactionsRotorDispWellAttr = myWell.ToString

        '                        ' 1. Place Reactions Rotor align with the specified well (Absolute positioning)
        '                        ' 2. Z Move Reagents1 Arm to dispense liquid as well as to fill the well (Absolute positioning)
        '                        ' 3. Fill the well
        '                        ' 4. Z Place Reagents1 Arm outside Reactions Rotor (Absolute positioning)
        '                        With myFwScript1
        '                            .FwScriptID = FwSCRIPTS_IDS.FILL_WELL_GLF.ToString
        '                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                            .EvaluateValue = 1
        '                            .NextOnResultOK = Nothing
        '                            .NextOnResultNG = Nothing
        '                            .NextOnTimeOut = Nothing
        '                            .NextOnError = Nothing
        '                            ' expects 5 params
        '                            .ParamList = New List(Of String)
        '                            ' Place Rotor on specified Well to dispense
        '                            .ParamList.Add(MyClass.ReactionsRotorDispWellAttr)
        '                            ' Move Reagent1 Arm to its washing station
        '                            .ParamList.Add(MyClass.ArmReagent1WSPolarAttr)
        '                            ' Move Reagent1 Arm to Dispensation Point
        '                            .ParamList.Add(MyClass.ArmReagent1DispPolarAttr)
        '                            .ParamList.Add(MyClass.ArmReagent1DispZAttr)
        '                            ' Move Reagent1 Arm to its washing station
        '                            .ParamList.Add(MyClass.ArmReagent1WSPolarAttr)
        '                        End With

        '                        If myListFwScript.Count > 0 Then
        '                            For i As Integer = 0 To myListFwScript.Count - 1
        '                                If i = 0 Then
        '                                    ' First Script
        '                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
        '                                Else
        '                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
        '                                End If
        '                            Next
        '                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
        '                        Else
        '                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '                        End If


        '                    Case FILL_MODE.MANUAL
        '                        ' presuppose that the Rotor is filled by user - only need execute homes
        '                        MyClass.CurrentOperation = OPERATIONS.HOMES
        '                        If myListFwScript.Count > 0 Then
        '                            With myListFwScript(myListFwScript.Count - 1)
        '                                .NextOnResultOK = Nothing
        '                                .NextOnTimeOut = Nothing
        '                            End With

        '                            For i As Integer = 0 To myListFwScript.Count - 1
        '                                If i = 0 Then
        '                                    ' First Script
        '                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
        '                                Else
        '                                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
        '                                End If
        '                            Next
        '                        Else
        '                            MyClass.ReactionsRotorHomesDoneAttr = True
        '                            MyClass.NoneInstructionToSendAttr = False
        '                        End If

        '                End Select

        '            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
        '                ' 
        '                ' THERMOS REAGENT1
        '                '

        '                ' Conditioning for Reagent 1 
        '                With myFwScript1
        '                    .FwScriptID = FwSCRIPTS_IDS.TEMPER_REAGENT1_NEEDLE.ToString
        '                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                    .EvaluateValue = 1
        '                    .NextOnResultOK = Nothing
        '                    .NextOnResultNG = Nothing
        '                    .NextOnTimeOut = Nothing
        '                    .NextOnError = Nothing
        '                    ' expects 1 param
        '                    .ParamList = New List(Of String)
        '                    ' Move Reagent1 Arm to its washing station
        '                    .ParamList.Add(MyClass.ArmReagent1WSPolarAttr)
        '                End With

        '                If myListFwScript.Count > 0 Then
        '                    For i As Integer = 0 To myListFwScript.Count - 1
        '                        If i = 0 Then
        '                            ' First Script
        '                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
        '                        Else
        '                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
        '                        End If
        '                    Next
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
        '                Else
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '                End If

        '            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
        '                ' 
        '                ' THERMOS REAGENT2
        '                '

        '                ' Conditioning for Reagent 2 
        '                With myFwScript1
        '                    .FwScriptID = FwSCRIPTS_IDS.TEMPER_REAGENT2_NEEDLE.ToString
        '                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                    .EvaluateValue = 1
        '                    .NextOnResultOK = Nothing
        '                    .NextOnResultNG = Nothing
        '                    .NextOnTimeOut = Nothing
        '                    .NextOnError = Nothing
        '                    ' expects 1 param
        '                    .ParamList = New List(Of String)
        '                    ' Move Reagent1 Arm to its washing station
        '                    .ParamList.Add(MyClass.ArmReagent2WSPolarAttr)
        '                End With

        '                If myListFwScript.Count > 0 Then
        '                    For i As Integer = 0 To myListFwScript.Count - 1
        '                        If i = 0 Then
        '                            ' First Script
        '                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
        '                        Else
        '                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
        '                        End If
        '                    Next
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
        '                Else
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '                End If

        '            Case ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
        '                ' 
        '                ' THERMOS WASHING STATION HEATER 
        '                '

        '                ' Conditioning for Washing Station Heater
        '                With myFwScript1
        '                    .FwScriptID = FwSCRIPTS_IDS.TEMPER_HEATER.ToString
        '                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                    .EvaluateValue = 1
        '                    .NextOnResultOK = Nothing
        '                    .NextOnResultNG = Nothing
        '                    .NextOnTimeOut = Nothing
        '                    .NextOnError = Nothing
        '                    .ParamList = Nothing
        '                End With

        '                If myListFwScript.Count > 0 Then
        '                    For i As Integer = 0 To myListFwScript.Count - 1
        '                        If i = 0 Then
        '                            ' First Script
        '                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), True)
        '                        Else
        '                            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myListFwScript(i), False)
        '                        End If
        '                    Next
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
        '                Else
        '                    If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
        '                End If

        '        End Select

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendQueueForCONDIGIONING_SYSTEM", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function

        '''' <summary>
        '''' Creates the Script List for Screen Measure Temperature operation
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 17/06/2011</remarks>
        'Private Function SendQueueForMEASURE_TEMPERATURE(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
        '    Dim myResultData As New GlobalDataTO
        '    Dim myFwScript1 As New FwScriptQueueItem
        '    Try
        '        Select Case pAdjustment
        '            Case ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
        '                Dim myWell As Integer
        '                ' Well to measure
        '                myWell = MyClass.ReactionsRotorMeasuredWellsAttr(MyClass.ReactionsRotorCurrentWellAttr - 1)

        '                ' Place Rotor align with the specified well to measure (Absolute positioning)
        '                With myFwScript1
        '                    .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ABS_ROTOR.ToString
        '                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                    .EvaluateValue = 1
        '                    .NextOnResultOK = Nothing
        '                    .NextOnResultNG = Nothing
        '                    .NextOnTimeOut = Nothing
        '                    .NextOnError = Nothing
        '                    ' expects 1 param
        '                    .ParamList = New List(Of String)
        '                    .ParamList.Add(myWell.ToString)
        '                End With

        '                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

        '            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1

        '                ' Take Distilled water from Reagents Rotor and dispense it on auxiliary device placed on Reagents Arm's washing station
        '                With myFwScript1
        '                    .FwScriptID = FwSCRIPTS_IDS.TAKE_TEMP_REAGENT1_NEEDLE.ToString
        '                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                    .EvaluateValue = 1
        '                    .NextOnResultOK = Nothing
        '                    .NextOnResultNG = Nothing
        '                    .NextOnTimeOut = Nothing
        '                    .NextOnError = Nothing
        '                    ' expects 5 params
        '                    .ParamList = New List(Of String)
        '                    ' well to place reagents rotor
        '                    .ParamList.Add(MyClass.ReagentNeedleSourceWellAttr.ToString)
        '                    ' Reagents Rotor Ring 1 - Horizontal
        '                    .ParamList.Add(MyClass.ArmReagent1Ring1PolarAttr)
        '                    ' Rotor de Reactivos Corona 1 Punto Aproximacio ???? 
        '                    .ParamList.Add("500") ' PDT !!! ????  
        '                    ' Reagents Rotor Ring 1 - Vertical
        '                    .ParamList.Add(MyClass.ArmReagent1Ring1ZAttr)
        '                    ' Move Reagent1 Arm to its washing station
        '                    .ParamList.Add(MyClass.ArmReagent1WSPolarAttr)
        '                End With

        '                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

        '            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2

        '                ' Take Distilled water from Reagents Rotor and dispense it on auxiliary device placed on Reagents Arm's washing station
        '                With myFwScript1
        '                    .FwScriptID = FwSCRIPTS_IDS.TAKE_TEMP_REAGENT2_NEEDLE.ToString
        '                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
        '                    .EvaluateValue = 1
        '                    .NextOnResultOK = Nothing
        '                    .NextOnResultNG = Nothing
        '                    .NextOnTimeOut = Nothing
        '                    .NextOnError = Nothing
        '                    ' expects 5 params
        '                    .ParamList = New List(Of String)
        '                    ' well to place reagents rotor
        '                    .ParamList.Add(MyClass.ReagentNeedleSourceWellAttr.ToString)
        '                    ' Reagents Rotor Ring 1 - Horizontal
        '                    .ParamList.Add(MyClass.ArmReagent2Ring1PolarAttr)
        '                    ' Rotor de Reactivos Corona 1 Punto Aproximacio ????
        '                    .ParamList.Add("500") ' PDT !!! ???? 
        '                    ' Reagents Rotor Ring 1 - Vertical
        '                    .ParamList.Add(MyClass.ArmReagent2Ring1ZAttr)
        '                    ' Move Reagent1 Arm to its washing station
        '                    .ParamList.Add(MyClass.ArmReagent2WSPolarAttr)
        '                End With

        '                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

        '        End Select

        '    Catch ex As Exception
        '        myResultData.HasError = True
        '        myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myResultData.ErrorMessage = ex.Message

        '        If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
        '            myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
        '        End If

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendQueueForMEASURE_TEMPERATURE", EventLogEntryType.Error, False)
        '    End Try
        '    Return myResultData
        'End Function


        ''' <summary>
        ''' Creates the Script List for Screen Test Adjustment operation Without save it 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 20/06/2011</remarks>
        Private Function SendQueueForTEST_ADJUSTMENT(ByVal pAdjustment As ADJUSTMENT_GROUPS) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myFwScript1 As New FwScriptQueueItem
            Try
                Select Case pAdjustment
                    Case ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY
                        ' Assign a new Consigna's Temperature to the Reaction Rotor's System
                        With myFwScript1
                            .FwScriptID = FwSCRIPTS_IDS.SET_TCO_REACTIONS_ROTOR.ToString
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(MyClass.ReactionsRotorTCOAttr)
                        End With

                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)


                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                        ' Assign a new Consigna's Temperature to the Reagent1 Needle's System
                        With myFwScript1
                            .FwScriptID = FwSCRIPTS_IDS.SET_TCO_REAGENT1.ToString
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(MyClass.ReagentNeedleTCOAttr)
                        End With

                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)


                    Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                        ' Assign a new Consigna's Temperature to the Reagent2 Needle's System
                        With myFwScript1
                            .FwScriptID = FwSCRIPTS_IDS.SET_TCO_REAGENT2.ToString
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(MyClass.ReagentNeedleTCOAttr)
                        End With

                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)


                    Case ADJUSTMENT_GROUPS.THERMOS_WS_HEATER
                        ' Assign a new Consigna's Temperature to the Washing Station Heater's System
                        With myFwScript1
                            .FwScriptID = FwSCRIPTS_IDS.SET_TCO_HEATER.ToString
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            ' expects 1 param
                            .ParamList = New List(Of String)
                            .ParamList.Add(MyClass.HeaterTCOAttr)
                        End With

                        If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

                End Select

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendQueueForTEST_ADJUSTMENT", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Prime
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 14/10/11
        ''' </remarks>
        Private Function SendQueueForPRIME_NEEDLE() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myHomeScriptsList As List(Of FwScriptQueueItem) = Nothing
            Dim myFwScript1 As New FwScriptQueueItem
            Dim myFwScript2 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CurrentOperation = OPERATIONS.CONDITIONING_NEEDLE

                myResultData = MyBase.GetPendingPreliminaryHomeScripts(MyClass.CurrentTest, myFwScript1)
                If Not myResultData.HasError AndAlso myResultData.SetDatos IsNot Nothing Then
                    myHomeScriptsList = CType(myResultData.SetDatos, List(Of FwScriptQueueItem))
                End If

                If Not myResultData.HasError Then
                    With myFwScript1
                        Select Case MyClass.CurrentTest
                            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                                .FwScriptID = FwSCRIPTS_IDS.NEEDLES_PRIME_R1.ToString
                                .ParamList = New List(Of String)
                                .ParamList.Add(MyClass.R1VerticalSafetyPosAttr.ToString)
                                .ParamList.Add(MyClass.R1WSHorizontalPosAttr.ToString)
                                .ParamList.Add(MyClass.R1WSVerticalRefPosAttr.ToString)
                                .ParamList.Add(MyClass.R1VerticalSafetyPosAttr.ToString)

                            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                                .FwScriptID = FwSCRIPTS_IDS.NEEDLES_PRIME_R2.ToString
                                .ParamList = New List(Of String)
                                .ParamList.Add(MyClass.R2VerticalSafetyPosAttr.ToString)
                                .ParamList.Add(MyClass.R2WSHorizontalPosAttr.ToString)
                                .ParamList.Add(MyClass.R2WSVerticalRefPosAttr.ToString)
                                .ParamList.Add(MyClass.R2VerticalSafetyPosAttr.ToString)

                        End Select

                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = Nothing
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = Nothing
                        .NextOnError = Nothing



                    End With

                    'add to the queue list
                    If myHomeScriptsList IsNot Nothing And myHomeScriptsList.Count > 0 Then
                        For i As Integer = 0 To myHomeScriptsList.Count - 1
                            If i = 0 Then
                                ' First Script
                                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myHomeScriptsList(i), True)
                            Else
                                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myHomeScriptsList(i), False)
                            End If
                        Next
                        If myFwScript1 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                    Else
                        If myFwScript1 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                    End If
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendQueueForPRIME", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List for Prime
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 14/10/11
        ''' </remarks>
        Private Function SendQueueForDISPENSE_NEEDLE() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myHomeScriptsList As List(Of FwScriptQueueItem) = Nothing
            Dim myFwScript1 As New FwScriptQueueItem
            Dim myFwScript2 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CurrentOperation = OPERATIONS.DISPENSING_NEEDLE

                myResultData = MyBase.GetPendingPreliminaryHomeScripts(MyClass.CurrentTest, myFwScript1)
                If Not myResultData.HasError AndAlso myResultData.SetDatos IsNot Nothing Then
                    myHomeScriptsList = CType(myResultData.SetDatos, List(Of FwScriptQueueItem))
                End If

                If Not myResultData.HasError Then
                    ' Assign a new Consigna's Temperature to the Washing Station Heater's System
                    With myFwScript1
                        .FwScriptID = FwSCRIPTS_IDS.REAGENTS_ABS_ROTOR.ToString
                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = myFwScript2
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = myFwScript2
                        .NextOnError = Nothing
                        ' expects 1 param
                        .ParamList = New List(Of String)

                        Select Case MyClass.CurrentTest
                            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1 : .ParamList.Add(MyClass.R1RotorPosAttr.ToString)
                            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2 : .ParamList.Add(MyClass.R2RotorPosAttr.ToString)
                        End Select
                    End With


                    With myFwScript2
                        Select Case MyClass.CurrentTest
                            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1 : .FwScriptID = FwSCRIPTS_IDS.NEEDLES_DISPENSE_R1.ToString
                            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2 : .FwScriptID = FwSCRIPTS_IDS.NEEDLES_DISPENSE_R2.ToString
                        End Select

                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = Nothing
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = Nothing
                        .NextOnError = Nothing

                        .ParamList = New List(Of String)
                        Select Case MyClass.CurrentTest
                            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT1
                                ' XBC 19/04/2012 - Change maneuver
                                '.ParamList.Add(MyClass.R1RotorPosAttr.ToString)
                                '.ParamList.Add(MyClass.R1RotorRing1HorPosAttr.ToString)
                                '.ParamList.Add(MyClass.R1RotorRing1DetMaxVerPosAttr.ToString)
                                '.ParamList.Add(MyClass.R1VerticalSafetyPosAttr.ToString)
                                '.ParamList.Add(MyClass.R1WSHorizontalPosAttr.ToString)

                                .ParamList.Add(MyClass.R1VerticalSafetyPosAttr.ToString)
                                .ParamList.Add(MyClass.R1RotorRing1HorPosAttr.ToString)
                                .ParamList.Add(MyClass.R1InitialPointLevelDetAttr.ToString)
                                .ParamList.Add(MyClass.R1RotorRing1DetMaxVerPosAttr.ToString)
                                .ParamList.Add(MyClass.R1VerticalSafetyPosAttr.ToString)
                                .ParamList.Add(MyClass.R1WSHorizontalPosAttr.ToString)
                                ' XBC 19/04/2012 - Change maneuver

                            Case ADJUSTMENT_GROUPS.THERMOS_REAGENT2
                                ' XBC 19/04/2012 - Change maneuver
                                '.ParamList.Add(MyClass.R2RotorPosAttr.ToString)
                                '.ParamList.Add(MyClass.R2RotorRing1HorPosAttr.ToString)
                                '.ParamList.Add(MyClass.R2RotorRing1DetMaxVerPosAttr.ToString)
                                '.ParamList.Add(MyClass.R2VerticalSafetyPosAttr.ToString)
                                '.ParamList.Add(MyClass.R2WSHorizontalPosAttr.ToString)

                                .ParamList.Add(MyClass.R2VerticalSafetyPosAttr.ToString)
                                .ParamList.Add(MyClass.R2RotorRing1HorPosAttr.ToString)
                                .ParamList.Add(MyClass.R2InitialPointLevelDetAttr.ToString)
                                .ParamList.Add(MyClass.R2RotorRing1DetMaxVerPosAttr.ToString)
                                .ParamList.Add(MyClass.R2VerticalSafetyPosAttr.ToString)
                                .ParamList.Add(MyClass.R2WSHorizontalPosAttr.ToString)
                                ' XBC 19/04/2012 - Change maneuver

                        End Select
                    End With

                    'add to the queue list
                    If myHomeScriptsList IsNot Nothing And myHomeScriptsList.Count > 0 Then
                        For i As Integer = 0 To myHomeScriptsList.Count - 1
                            If i = 0 Then
                                ' First Script
                                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myHomeScriptsList(i), True)
                            Else
                                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myHomeScriptsList(i), False)
                            End If
                        Next
                        If myFwScript1 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                        If myFwScript2 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
                    Else
                        If myFwScript1 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                        If myFwScript2 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript2, False)
                    End If
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendQueueForDISPENSE_NEEDLE", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        ''' <summary>
        ''' Creates the Script List WS Heater Test
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 14/10/11
        ''' </remarks>
        Private Function SendQueueForWS_HEATER_TEST() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myHomeScriptsList As List(Of FwScriptQueueItem) = Nothing
            Dim myFwScript1 As New FwScriptQueueItem
            Dim myFwScript2 As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CurrentOperation = OPERATIONS.CONDITIONING_HEATER

                myResultData = MyBase.GetPendingPreliminaryHomeScripts(MyClass.CurrentTest, myFwScript1)
                If Not myResultData.HasError AndAlso myResultData.SetDatos IsNot Nothing Then
                    myHomeScriptsList = CType(myResultData.SetDatos, List(Of FwScriptQueueItem))
                End If

                If Not myResultData.HasError Then
                    With myFwScript1
                        .FwScriptID = FwSCRIPTS_IDS.WS_HEATER_TEST.ToString    ' .FwScriptID = FwSCRIPTS_IDS.WS_HEATER_TEST_OLD.ToString
                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = Nothing
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = Nothing
                        .NextOnError = Nothing

                        .ParamList = New List(Of String)
                        .ParamList.Add(MyClass.WSReadyRefPosAttr.ToString)
                        .ParamList.Add(MyClass.WSFinalPosAttr.ToString)
                        .ParamList.Add(MyClass.WSReferencePos.ToString)

                    End With

                    'add to the queue list
                    If myHomeScriptsList IsNot Nothing And myHomeScriptsList.Count > 0 Then
                        For i As Integer = 0 To myHomeScriptsList.Count - 1
                            If i = 0 Then
                                ' First Script
                                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myHomeScriptsList(i), True)
                            Else
                                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myHomeScriptsList(i), False)
                            End If
                        Next
                        If myFwScript1 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                    Else
                        If myFwScript1 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                    End If
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendQueueForWS_HEATER_TEST", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Creates the Script List WS Heater Test
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 14/10/11
        ''' </remarks>
        Private Function SendQueueForROTATE_ROTOR() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            'Dim myHomeScriptsList As List(Of FwScriptQueueItem)
            Dim myFwScript1 As New FwScriptQueueItem

            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CurrentOperation = OPERATIONS.ROTATING_ROTOR

                'myResultData = MyBase.GetPendingPreliminaryHomeScripts(MyClass.CurrentTest, myFwScript1)
                'If Not myResultData.HasError AndAlso myResultData.SetDatos IsNot Nothing Then
                '    myHomeScriptsList = CType(myResultData.SetDatos, List(Of FwScriptQueueItem))
                'End If

                If Not myResultData.HasError Then
                    With myFwScript1
                        .FwScriptID = FwSCRIPTS_IDS.ROTOR_ROTATE.ToString
                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                        .EvaluateValue = 1
                        .NextOnResultOK = Nothing
                        .NextOnResultNG = Nothing
                        .NextOnTimeOut = Nothing
                        .NextOnError = Nothing

                        .ParamList = Nothing

                    End With

                    'add to the queue list
                    'If myHomeScriptsList IsNot Nothing And myHomeScriptsList.Count > 0 Then
                    '    For i As Integer = 0 To myHomeScriptsList.Count - 1
                    '        If i = 0 Then
                    '            ' First Script
                    '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myHomeScriptsList(i), True)
                    '        Else
                    '            If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myHomeScriptsList(i), False)
                    '        End If
                    '    Next
                    '    If myFwScript1 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, False)
                    'Else
                    If myFwScript1 IsNot Nothing AndAlso Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)
                    'End If
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendQueueForROTATE_ROTOR", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        Public Property IsWSHeaterConditioning() As Boolean
            Get
                Return IsWSHeaterConditioningAttr
            End Get
            Set(ByVal value As Boolean)
                IsWSHeaterConditioningAttr = value
            End Set
        End Property
        Private IsWSHeaterConditioningAttr As Boolean = False
        Public Event OnWSHeaterIsStartConditioning()

        Public Property IsWSHeaterConditioned() As Boolean
            Get
                Return IsWSHeaterConditionedAttr
            End Get
            Set(ByVal value As Boolean)
                IsWSHeaterConditionedAttr = value
            End Set
        End Property
        Private IsWSHeaterConditionedAttr As Boolean = False
        Public Event OnWSHeaterIsConditioned()

        Public Property IsRotorPriming() As Boolean
            Get
                Return IsRotorPrimingAttr
            End Get
            Set(ByVal value As Boolean)
                IsRotorPrimingAttr = value
            End Set
        End Property
        Private IsRotorPrimingAttr As Boolean = False
        Public Event OnRotorIsStartPriming()

        Public Property IsRotorPrimed() As Boolean
            Get
                Return IsRotorPrimedAttr
            End Get
            Set(ByVal value As Boolean)
                IsRotorPrimedAttr = value
            End Set
        End Property
        Private IsRotorPrimedAttr As Boolean = False
        Public Event OnRotorIsPrimed()

        Public Property IsRotorConditioning() As Boolean
            Get
                Return IsRotorConditioningAttr
            End Get
            Set(ByVal value As Boolean)
                IsRotorConditioningAttr = value
            End Set
        End Property
        Private IsRotorConditioningAttr As Boolean = False
        Public Event OnRotorIsStartConditioning()

        Public Property IsRotorConditioned() As Boolean
            Get
                Return IsRotorConditionedAttr
            End Get
            Set(ByVal value As Boolean)
                IsRotorConditionedAttr = value
            End Set
        End Property
        Private IsRotorConditionedAttr As Boolean = False
        Public Event OnRotorIsConditioned()


        ''' <summary>
        ''' Creates the Script List WS Heater Test
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 14/10/11
        ''' </remarks>
        Private Function SendQueueForCONDITIONING_ROTOR() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Dim myHomeScriptsList As List(Of FwScriptQueueItem) = Nothing
            Dim myFwPrimeStartScript As New FwScriptQueueItem
            Dim myFwPrimeScriptsList As New List(Of FwScriptQueueItem)
            Dim myFwPrimeEndScript As New FwScriptQueueItem
            Dim myFwAutoFillScriptsList As New List(Of FwScriptQueueItem)
            Dim myFwRotorInWell1Script As New FwScriptQueueItem
            Try
                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                CurrentOperation = OPERATIONS.CONDITIONING_ROTOR

                If MyClass.FillMode = FILL_MODE.AUTOMATIC Then
                    myResultData = MyBase.GetPendingPreliminaryHomeScripts(MyClass.CurrentTest, myFwPrimeStartScript)
                ElseIf MyClass.FillMode = FILL_MODE.MANUAL Then
                    myResultData = MyBase.GetPendingPreliminaryHomeScripts(MyClass.CurrentTest, Nothing)
                End If
                If Not myResultData.HasError AndAlso myResultData.SetDatos IsNot Nothing Then
                    myHomeScriptsList = CType(myResultData.SetDatos, List(Of FwScriptQueueItem))
                End If

                If Not myResultData.HasError Then

                    If MyClass.FillMode = FILL_MODE.AUTOMATIC Then
                        For F As Integer = 1 To MyClass.ReactionsRotorMeasuredWellsCountAttr Step 1
                            myFwAutoFillScriptsList.Add(New FwScriptQueueItem)
                        Next F


                        'if the Rotor is not primed yet
                        If Not IsRotorPrimedAttr Then

                            For P As Integer = 1 To MyClass.ReactionsRotorMaxPrimesAttr Step 1
                                myFwPrimeScriptsList.Add(New FwScriptQueueItem)
                            Next P

                            'PRIME INIT
                            With myFwPrimeStartScript
                                .FwScriptID = FwSCRIPTS_IDS.ROTOR_PRIME_INI.ToString
                                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                .EvaluateValue = 1
                                .NextOnResultOK = myFwPrimeScriptsList(0)
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = Nothing
                                .NextOnError = Nothing

                                .ParamList = New List(Of String)
                                .ParamList.Add(MyClass.WSFinalPosAttr.ToString)

                            End With

                            'PRIME STEP
                            For P As Integer = 1 To MyClass.ReactionsRotorMaxPrimesAttr
                                If P < MyClass.ReactionsRotorMaxPrimesAttr Then
                                    With myFwPrimeScriptsList(P - 1)
                                        .FwScriptID = FwSCRIPTS_IDS.ROTOR_PRIME_STEP.ToString
                                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                        .EvaluateValue = 1
                                        .NextOnResultOK = myFwPrimeScriptsList(P)
                                        .NextOnResultNG = Nothing
                                        .NextOnTimeOut = Nothing
                                        .NextOnError = Nothing

                                        .ParamList = Nothing
                                    End With
                                Else
                                    With myFwPrimeScriptsList(P - 1)
                                        .FwScriptID = FwSCRIPTS_IDS.ROTOR_PRIME_STEP.ToString
                                        .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                        .EvaluateValue = 1
                                        .NextOnResultOK = myFwPrimeEndScript
                                        .NextOnResultNG = Nothing
                                        .NextOnTimeOut = Nothing
                                        .NextOnError = Nothing

                                        .ParamList = Nothing
                                    End With
                                End If

                            Next

                            'PRIME END
                            With myFwPrimeEndScript
                                .FwScriptID = FwSCRIPTS_IDS.ROTOR_PRIME_END.ToString
                                .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                .EvaluateValue = 1
                                If MyClass.FillMode = FILL_MODE.AUTOMATIC Then
                                    .NextOnResultOK = myFwAutoFillScriptsList(0)
                                Else
                                    .NextOnResultOK = Nothing
                                End If
                                .NextOnResultNG = Nothing
                                .NextOnTimeOut = Nothing
                                .NextOnError = Nothing

                                .ParamList = New List(Of String)
                                .ParamList.Add(MyClass.WSReferencePosAttr.ToString)

                            End With

                        End If



                        'If MyClass.FillMode = FILL_MODE.AUTOMATIC Then

                        'AUTOFILL STEP
                        For F As Integer = 1 To MyClass.ReactionsRotorMeasuredWellsCountAttr
                            If F < MyClass.ReactionsRotorMeasuredWellsCountAttr Then
                                With myFwAutoFillScriptsList(F - 1)
                                    .FwScriptID = FwSCRIPTS_IDS.ROTOR_FILL_STEP.ToString
                                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                    .EvaluateValue = 1
                                    .NextOnResultOK = myFwAutoFillScriptsList(F)
                                    .NextOnResultNG = Nothing
                                    .NextOnTimeOut = Nothing
                                    .NextOnError = Nothing

                                    .ParamList = New List(Of String)
                                    .ParamList.Add(MyClass.ReactionsRotorWellsToFillAttr(F - 1).ToString)
                                    .ParamList.Add(MyClass.WSFinalPosAttr.ToString)
                                    .ParamList.Add(MyClass.WSReferencePosAttr.ToString)

                                End With
                            Else
                                With myFwAutoFillScriptsList(F - 1)
                                    .FwScriptID = FwSCRIPTS_IDS.ROTOR_FILL_STEP.ToString
                                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                                    .EvaluateValue = 1
                                    .NextOnResultOK = myFwRotorInWell1Script
                                    .NextOnResultNG = Nothing
                                    .NextOnTimeOut = Nothing
                                    .NextOnError = Nothing

                                    .ParamList = New List(Of String)
                                    .ParamList.Add(MyClass.ReactionsRotorWellsToFillAttr(F - 1).ToString)
                                    .ParamList.Add(MyClass.WSFinalPosAttr.ToString)
                                    .ParamList.Add(MyClass.WSReferencePosAttr.ToString)

                                End With
                            End If

                        Next F

                        With myFwRotorInWell1Script
                            .FwScriptID = FwSCRIPTS_IDS.REACTIONS_ROTOR_HOME_WELL1.ToString
                            .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                            .EvaluateValue = 1
                            .NextOnResultOK = Nothing
                            .NextOnResultNG = Nothing
                            .NextOnTimeOut = Nothing
                            .NextOnError = Nothing
                            .ParamList = Nothing
                        End With

                        'add to the queue list
                        If Not myResultData.HasError Then
                            If myHomeScriptsList IsNot Nothing Then
                                For Each Q As FwScriptQueueItem In myHomeScriptsList
                                    myResultData = myFwScriptDelegate.AddToFwScriptQueue(Q, False)
                                    If myResultData.HasError Then Exit For
                                Next
                            End If
                            If Not myResultData.HasError Then
                                If Not IsRotorPrimedAttr Then
                                    If myFwPrimeStartScript IsNot Nothing Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwPrimeStartScript, False)
                                    If Not myResultData.HasError Then
                                        If myFwPrimeScriptsList IsNot Nothing Then
                                            For Each S As FwScriptQueueItem In myFwPrimeScriptsList
                                                myResultData = myFwScriptDelegate.AddToFwScriptQueue(S, False)
                                                If myResultData.HasError Then Exit For
                                            Next
                                        End If
                                        If Not myResultData.HasError Then
                                            If myFwPrimeEndScript IsNot Nothing Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwPrimeEndScript, False)
                                        End If
                                    End If
                                End If
                                If MyClass.FillMode = FILL_MODE.AUTOMATIC Then
                                    If myFwAutoFillScriptsList IsNot Nothing Then
                                        For Each S As FwScriptQueueItem In myFwAutoFillScriptsList
                                            myResultData = myFwScriptDelegate.AddToFwScriptQueue(S, False)
                                            If myResultData.HasError Then Exit For
                                        Next
                                    End If
                                    If Not myResultData.HasError Then
                                        If myFwRotorInWell1Script IsNot Nothing Then
                                            myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwRotorInWell1Script, False)
                                        End If
                                    End If
                                End If
                            End If
                        End If

                    ElseIf MyClass.FillMode = FILL_MODE.MANUAL Then
                        If Not myResultData.HasError Then
                            If myHomeScriptsList IsNot Nothing Then
                                For Each Q As FwScriptQueueItem In myHomeScriptsList
                                    myResultData = myFwScriptDelegate.AddToFwScriptQueue(Q, False)
                                    If myResultData.HasError Then Exit For
                                Next
                            End If

                            If myHomeScriptsList Is Nothing OrElse myHomeScriptsList.Count = 0 Then
                                MyClass.ReactionsRotorHomesDoneAttr = True
                                MyClass.NoneInstructionToSendAttr = False
                            End If
                        End If
                    End If

                    If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing AndAlso myFwScriptDelegate.CurrentFwScriptsQueue.Count > 0 Then
                        myFwScriptDelegate.SetQueueItemAsFirst(myFwScriptDelegate.CurrentFwScriptsQueue.First)
                    End If

                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendQueueForCONDITIONING_ROTOR", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Activates Sound for a controlled interval of time
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 18/09/2012</remarks>
        Public Overrides Function SendSOUND() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                myResultData = MyBase.SendSOUND()

                If Not myResultData.HasError And MyClass.RotorSoundSecondsAttr > 0 Then
                    'Deactivate timer
                    If MyClass.RotorSoundTimer IsNot Nothing Then
                        MyClass.RotorSoundTimer.Dispose()
                        MyClass.RotorSoundTimer = Nothing
                        MyClass.RotorSoundTimerCallBack = Nothing
                    End If

                    'Activate timer
                    MyClass.RotorSoundTimerCallBack = New System.Threading.TimerCallback(AddressOf OnRotorSoundTimerTick)
                    MyClass.RotorSoundTimer = New System.Threading.Timer(MyClass.RotorSoundTimerCallBack, New Object, 1000 * MyClass.RotorSoundSecondsAttr, 0)

                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "BaseFwScriptDelegate.SendSOUND", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function

        ''' <summary>
        ''' Deactivates Sound for a controlled interval of time
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 18/09/2012</remarks>
        Public Overrides Function SendENDSOUND() As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try
                'Deactivate timer
                If MyClass.RotorSoundTimer IsNot Nothing Then
                    MyClass.RotorSoundTimer.Dispose()
                    MyClass.RotorSoundTimer = Nothing
                    MyClass.RotorSoundTimerCallBack = Nothing

                    myResultData = MyBase.SendENDSOUND()
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.SendENDSOUND", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function


        Private Sub OnRotorSoundTimerTick(ByVal stateInfo As Object)
            Try
                MyClass.SendENDSOUND()

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.waitingTimer_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region

#Region "Historical reports"
        Public Function InsertReport(ByVal pTaskID As String, ByVal pActionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim myHistoricalReportsDelegate As New HistoricalReportsDelegate
                Dim myHistoricReport As New SRVResultsServiceDS
                Dim myHistoricReportRow As SRVResultsServiceDS.srv_thrsResultsServiceRow

                myHistoricReportRow = myHistoricReport.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                myHistoricReportRow.TaskID = pTaskID
                myHistoricReportRow.ActionID = pActionID
                myHistoricReportRow.Data = MyClass.GenerateDataReport(myHistoricReportRow.TaskID, myHistoricReportRow.ActionID)
                myHistoricReportRow.AnalyzerID = AnalyzerId

                resultData = myHistoricalReportsDelegate.Add(Nothing, myHistoricReportRow)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    'Get the generated ID from the dataset returned 
                    Dim generatedID As Integer = -1
                    generatedID = DirectCast(resultData.SetDatos, SRVResultsServiceDS.srv_thrsResultsServiceRow).ResultServiceID

                    ' Insert recommendations if existing
                    If MyClass.RecommendationsReport IsNot Nothing Then
                        Dim myRecommendationsList As New SRVRecommendationsServiceDS
                        Dim myRecommendationsRow As SRVRecommendationsServiceDS.srv_thrsRecommendationsServiceRow

                        For i As Integer = 0 To MyClass.RecommendationsReport.Length - 1
                            myRecommendationsRow = myRecommendationsList.srv_thrsRecommendationsService.Newsrv_thrsRecommendationsServiceRow
                            myRecommendationsRow.ResultServiceID = generatedID
                            myRecommendationsRow.RecommendationID = CInt(MyClass.RecommendationsReport(i))
                            myRecommendationsList.srv_thrsRecommendationsService.Rows.Add(myRecommendationsRow)
                        Next

                        resultData = myHistoricalReportsDelegate.AddRecommendations(Nothing, myRecommendationsList)
                        If resultData.HasError Then
                            resultData.HasError = True
                        End If
                        MyClass.RecommendationsReport = Nothing
                    End If
                Else
                    resultData.HasError = True
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.InsertReport", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Information of every test in this screen with its corresponding codification to be inserted in the common database of Historics
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 29/08/2011
        ''' 
        ''' Data Format : 
        ''' -----------
        ''' Final Result for the test (1) - OK=1 NOK=2
        ''' New assigned values (1) - OK=1 NOK=2
        ''' Filling option used for the test (1) - Automatic=0 Manual=1
        ''' Conditioning execution (1) - Unrealized=0 OK=1 NOK=2
        ''' Temperature measured point 1 (1) - NO=0 YES=1
        ''' Temperature measured point 1 (5)
        ''' Temperature measured point 2 (1) - NO=0 YES=1
        ''' Temperature measured point 2 (5)
        ''' Temperature measured point 3 (1) - NO=0 YES=1
        ''' Temperature measured point 3 (5)
        ''' Temperature measured point 4 (1) - NO=0 YES=1
        ''' Temperature measured point 4 (5)
        ''' Temperature measured point 5 (1) - NO=0 YES=1
        ''' Temperature measured point 5 (5)
        ''' Temperature measured point 6 (1) - NO=0 YES=1
        ''' Temperature measured point 6 (5)
        ''' Temperature measured point 7 (1) - NO=0 YES=1
        ''' Temperature measured point 7 (5)
        ''' Temperature measured point 8 (1) - NO=0 YES=1
        ''' Temperature measured point 8 (5)
        ''' Final Mean Temperature measured (5)
        ''' Final Sensor Temperature (5)
        ''' Final Correction Adjustment (6)
        ''' </remarks>
        Private Function GenerateDataReport(ByVal pTask As String, ByVal pAction As String) As String
            Dim returnValue As String = ""
            Try
                Select Case pTask
                    Case "TEST"

                        Select Case pAction
                            Case "GLF_TER"

                                returnValue = ""
                                ' Final Result obtained
                                If MyClass.FinalResultAttr(0) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Filling option used for the test
                                returnValue += CInt(MyClass.FillModeAttr).ToString
                                ' Conditioning execution
                                If MyClass.ConditioningExecutedAttr(0) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.ConditioningExecutedAttr(0) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.ConditioningExecutedAttr(0) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If
                                ' SetPoint temperature test
                                If MyClass.TestTempExecutedAttr(0) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.TestTempExecutedAttr(0) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.TestTempExecutedAttr(0) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If

                                ' Temperature measured points 1 to 4
                                For i As Integer = 0 To ReactionsRotorPrepareWellDoneAttr.Count - 1 'IT 21/07/2015 - BA-2649
                                    If Not MyClass.ReactionsRotorPrepareWellDoneAttr(i) Then
                                        returnValue += "0"
                                    Else
                                        returnValue += "1"
                                    End If
                                    ' Temperature measured value point i
                                    returnValue += MyClass.ProbeWellsTempsAttr(i).ToString("00.00")
                                Next

                                ' Mean Temperature measured 
                                returnValue += MyClass.ProbeWellsTempsAttr.Average.ToString("00.00")

                                ' Sensor Temperature received
                                returnValue += MyClass.MonitorThermoRotorAttr.ToString("00.00")

                            Case "REG1_TER"

                                returnValue = ""
                                ' Final Result obtained
                                If MyClass.FinalResultAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Selected Arm
                                returnValue += "1"
                                ' Conditioning execution
                                If MyClass.ConditioningExecutedAttr(1) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.ConditioningExecutedAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.ConditioningExecutedAttr(1) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If
                                ' SetPoint temperature test
                                If MyClass.TestTempExecutedAttr(1) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.TestTempExecutedAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.TestTempExecutedAttr(1) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If

                                ' Temperature measured 
                                If Not MyClass.ReagentNeedleMeasuredTempDoneAttr Then
                                    returnValue += "0"
                                Else
                                    returnValue += "1"
                                End If
                                ' Temperature measured value
                                returnValue += MyClass.ReagentMeasuredTempAttr.ToString("00.00")

                                ' Sensor Temperature received
                                returnValue += MyClass.MonitorThermoR1Attr.ToString("00.00")

                                ' XBC 22/11/2011 - register Counter into Historics
                                returnValue += MyClass.NeedlesDispensationsCountAttr.ToString("00")


                            Case "REG2_TER"

                                returnValue = ""
                                ' Final Result obtained
                                If MyClass.FinalResultAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Selected Arm
                                returnValue += "2"
                                ' Conditioning execution
                                If MyClass.ConditioningExecutedAttr(1) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.ConditioningExecutedAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.ConditioningExecutedAttr(1) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If
                                ' SetPoint temperature test
                                If MyClass.TestTempExecutedAttr(1) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.TestTempExecutedAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.TestTempExecutedAttr(1) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If

                                ' Temperature measured 
                                If Not MyClass.ReagentNeedleMeasuredTempDoneAttr Then
                                    returnValue += "0"
                                Else
                                    returnValue += "1"
                                End If
                                ' Temperature measured value
                                returnValue += MyClass.ReagentMeasuredTempAttr.ToString("00.00")

                                ' Sensor Temperature received
                                returnValue += MyClass.MonitorThermoR2Attr.ToString("00.00")

                                ' XBC 22/11/2011 - register Counter into Historics
                                returnValue += MyClass.NeedlesDispensationsCountAttr.ToString("00")

                            Case "HEAT_TER"

                                returnValue = ""
                                ' Final Result obtained
                                If MyClass.FinalResultAttr(2) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Conditioning execution
                                If MyClass.ConditioningExecutedAttr(2) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.ConditioningExecutedAttr(2) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.ConditioningExecutedAttr(2) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If
                                ' SetPoint temperature test
                                If MyClass.TestTempExecutedAttr(2) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.TestTempExecutedAttr(2) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.TestTempExecutedAttr(2) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If

                                ' Temperature measured 
                                If Not MyClass.HeaterTestAdjustmentDoneAttr Then
                                    returnValue += "0"
                                Else
                                    returnValue += "1"
                                End If
                                ' Temperature measured value
                                returnValue += MyClass.HeaterMeasuredTempAttr.ToString("00.00")

                                ' Sensor Temperature received
                                returnValue += MyClass.MonitorThermoHeaterAttr.ToString("00.00")

                        End Select

                    Case "ADJUST"

                        Select Case pAction
                            Case "GLF_TER"

                                returnValue = ""
                                ' Final Result obtained
                                If MyClass.FinalResultAttr(0) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' New assigned values
                                If MyClass.AssignedNewValuesAttr(0) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Filling option used for the test
                                returnValue += CInt(MyClass.FillModeAttr).ToString
                                ' Conditioning execution
                                If MyClass.ConditioningExecutedAttr(0) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.ConditioningExecutedAttr(0) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.ConditioningExecutedAttr(0) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If

                                ' Temperature measured points 1 to 4
                                For i As Integer = 0 To ReactionsRotorPrepareWellDoneAttr.Count - 1 'IT 21/07/2015 - BA-2649
                                    If Not MyClass.ReactionsRotorPrepareWellDoneAttr(i) Then
                                        returnValue += "0"
                                    Else
                                        returnValue += "1"
                                    End If
                                    ' Temperature measured value point i
                                    returnValue += MyClass.ProbeWellsTempsAttr(i).ToString("00.00")
                                Next

                                ' Mean Temperature measured 
                                returnValue += MyClass.ProbeWellsTempsAttr.Average.ToString("00.00")

                                ' Sensor Temperature received
                                returnValue += MyClass.MonitorThermoRotorAttr.ToString("00.00")

                                ' Final Correction Adjustment
                                If MyClass.ProposalCorrectionAttr < 0 Then
                                    returnValue += MyClass.ProposalCorrectionAttr.ToString("00.00")
                                Else
                                    returnValue += MyClass.ProposalCorrectionAttr.ToString("+00.00")
                                End If

                            Case "REG1_TER"

                                returnValue = ""
                                ' Final Result obtained
                                If MyClass.FinalResultAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Selected Arm
                                returnValue += "1"
                                ' New assigned values
                                If MyClass.AssignedNewValuesAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Conditioning execution
                                If MyClass.ConditioningExecutedAttr(1) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.ConditioningExecutedAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.ConditioningExecutedAttr(1) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If

                                ' Temperature measured 
                                If Not MyClass.ReagentNeedleMeasuredTempDoneAttr Then
                                    returnValue += "0"
                                Else
                                    returnValue += "1"
                                End If
                                ' Temperature measured value
                                returnValue += MyClass.ReagentMeasuredTempAttr.ToString("00.00")

                                ' Sensor Temperature received
                                returnValue += MyClass.MonitorThermoR1Attr.ToString("00.00")

                                ' Final Correction Adjustment
                                If MyClass.ProposalCorrectionAttr < 0 Then
                                    returnValue += MyClass.ProposalCorrectionAttr.ToString("00.00")
                                Else
                                    returnValue += MyClass.ProposalCorrectionAttr.ToString("+00.00")
                                End If

                                ' XBC 22/11/2011 - register Counter into Historics
                                returnValue += MyClass.NeedlesDispensationsCountAttr.ToString("00")


                            Case "REG2_TER"

                                returnValue = ""
                                ' Final Result obtained
                                If MyClass.FinalResultAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Selected Arm
                                returnValue += "2"
                                ' New assigned values
                                If MyClass.AssignedNewValuesAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Conditioning execution
                                If MyClass.ConditioningExecutedAttr(1) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.ConditioningExecutedAttr(1) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.ConditioningExecutedAttr(1) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If

                                ' Temperature measured 
                                If Not MyClass.ReagentNeedleMeasuredTempDoneAttr Then
                                    returnValue += "0"
                                Else
                                    returnValue += "1"
                                End If
                                ' Temperature measured value
                                returnValue += MyClass.ReagentMeasuredTempAttr.ToString("00.00")

                                ' Sensor Temperature received
                                returnValue += MyClass.MonitorThermoR2Attr.ToString("00.00")

                                ' Final Correction Adjustment
                                If MyClass.ProposalCorrectionAttr < 0 Then
                                    returnValue += MyClass.ProposalCorrectionAttr.ToString("00.00")
                                Else
                                    returnValue += MyClass.ProposalCorrectionAttr.ToString("+00.00")
                                End If

                                ' XBC 22/11/2011 - register Counter into Historics
                                returnValue += MyClass.NeedlesDispensationsCountAttr.ToString("00")


                            Case "HEAT_TER"

                                returnValue = ""
                                ' Final Result obtained
                                If MyClass.FinalResultAttr(2) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' New assigned values
                                If MyClass.AssignedNewValuesAttr(2) = ResultsOperations.OK Then
                                    returnValue += "1"
                                Else
                                    returnValue += "2"
                                End If
                                ' Conditioning execution
                                If MyClass.ConditioningExecutedAttr(2) = ResultsOperations.Unrealized Then
                                    returnValue += "0"
                                ElseIf MyClass.ConditioningExecutedAttr(2) = ResultsOperations.OK Then
                                    returnValue += "1"
                                ElseIf MyClass.ConditioningExecutedAttr(2) = ResultsOperations.NOK Then
                                    returnValue += "2"
                                End If

                                ' Temperature measured 
                                If Not MyClass.HeaterTestAdjustmentDoneAttr Then
                                    returnValue += "0"
                                Else
                                    returnValue += "1"
                                End If
                                ' Temperature measured value
                                returnValue += MyClass.HeaterMeasuredTempAttr.ToString("00.00")

                                ' Sensor Temperature received
                                returnValue += MyClass.MonitorThermoHeaterAttr.ToString("00.00")

                                ' Final Correction Adjustment
                                If MyClass.ProposalCorrectionAttr < 0 Then
                                    returnValue += MyClass.ProposalCorrectionAttr.ToString("00.00")
                                Else
                                    returnValue += MyClass.ProposalCorrectionAttr.ToString("+00.00")
                                End If

                        End Select

                End Select

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ThermosAdjustmentDelegate.GenerateDataReport", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function
#End Region

    End Class

End Namespace

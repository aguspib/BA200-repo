Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Services

    Public Class DynamicBaseLineService

        Public Sub New(analyzer As IAnalyzerManager)
            _analyzer = analyzer
        End Sub


        Public Enum DynamicBaseLineStepsEnum
            ConditioningWashing
            StaticBaseLine
            DynamicBaseLineFill
            DynamicBaseLineRead
            DynamicBaseLineEmpty
            Finalize
            None
        End Enum

#Region "Attributes"

        Private WithEvents _analyzer As IAnalyzerManager
        Private _forceEmptyAndFinalize As Boolean = False
        Private _staticBaseLineFinished As Boolean = False
        Private _dynamicBaseLineValid As Boolean = False

#End Region

#Region "Event Handlers"

        Public Sub OnReceivedStatusInformationEvent() Handles _analyzer.ReceivedStatusInformationEventHandler
            TryToStartNextStep()
        End Sub

        Public Sub OnProcessFlagEvent(ByVal pFlagCode As AnalyzerManagerFlags) Handles _analyzer.ProcessFlagEventHandler
            Select Case pFlagCode
                Case AnalyzerManagerFlags.BaseLine
                    ProcessStaticBaseLine()

                Case AnalyzerManagerFlags.DynamicBL_Fill,
                    AnalyzerManagerFlags.DynamicBL_Empty
                    ValidateProcess()

                Case AnalyzerManagerFlags.DynamicBL_Read
                    ProcessDynamicBaseLine()

            End Select
        End Sub

#End Region

        Public Function StartProcess() As Boolean

            'Previous conditions: (no flow here)
            If _analyzer.Connected = False Then Return False

            'Method flow:
            Dim StartProcessSuccess As Boolean = False

            Dim resultData As GlobalDataTO
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            Initialize()

            If _analyzer.ExistBottleAlarms Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "CANCELED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "CANCELED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, "") 'AG + IT 10/02/2015 BA-2246 apply same rules in Change Rotor and in StartInstr
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "CANCELED")
                StartProcessSuccess = False
            Else
                If _analyzer.Connected Then
                    _analyzer.StopAnalyzerRinging()
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, "")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "")
                    _staticBaseLineFinished = False

                    resultData = _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, Ax00WashStationControlModes.UP, "")

                    If resultData.HasError Then
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "")
                        Throw New Exception(resultData.ErrorCode)
                    Else
                        'Update analyzer session flags into DataBase
                        If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                        End If
                        StartProcessSuccess = True
                    End If
                Else
                    StartProcessSuccess = False
                End If
            End If

            Return StartProcessSuccess

        End Function

#Region "Private methods"
        Private Sub Initialize()

            _forceEmptyAndFinalize = False
            _staticBaseLineFinished = False
            _dynamicBaseLineValid = False
            _analyzer.DynamicBaselineInitializationFailures = 0
            _analyzer.CurrentInstructionAction = InstructionActions.None

            If _analyzer.Alarms.Contains(Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_INIT_ERR)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub TryToStartNextStep()

            Dim nextStep As DynamicBaseLineStepsEnum
            nextStep = GetNextStep()

            Select Case nextStep
                Case DynamicBaseLineStepsEnum.ConditioningWashing,
                     DynamicBaseLineStepsEnum.StaticBaseLine,
                     DynamicBaseLineStepsEnum.DynamicBaseLineFill,
                     DynamicBaseLineStepsEnum.DynamicBaseLineEmpty
                    ValidateProcess()
            End Select

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextStep() As DynamicBaseLineStepsEnum

            Dim nextStep = DynamicBaseLineStepsEnum.None

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") Then
                nextStep = GetNextStepWhileInProcess()

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED") Then

                If (_analyzer.SessionFlag(AnalyzerManagerFlags.NewRotor) = "END") Then
                    If (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) = "CANCELED") Then
                        nextStep = DynamicBaseLineStepsEnum.ConditioningWashing

                    ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                        nextStep = DynamicBaseLineStepsEnum.StaticBaseLine

                    ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "CANCELED") Then
                        nextStep = DynamicBaseLineStepsEnum.DynamicBaseLineFill

                    ElseIf ((_forceEmptyAndFinalize) Or (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED")) Then
                        nextStep = DynamicBaseLineStepsEnum.DynamicBaseLineEmpty

                    End If
                End If

            End If
            Return nextStep

        End Function

        Private Function GetNextStepWhileInProcess() As DynamicBaseLineStepsEnum

            Dim nextStep = DynamicBaseLineStepsEnum.None

            Select Case _analyzer.SessionFlag(AnalyzerManagerFlags.Washing)
                Case "", "CANCELED"
                    nextStep = DynamicBaseLineStepsEnum.ConditioningWashing

                Case "END"
                    If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "") OrElse (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                        nextStep = DynamicBaseLineStepsEnum.StaticBaseLine

                    Else

                        If (_analyzer.BaseLineTypeForCalculations = BaseLineType.DYNAMIC) Then

                            If (_analyzer.CurrentInstructionAction = InstructionActions.None) Then

                                If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "END") And
                                   ((_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "") OrElse
                                    (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "CANCELED")) Then
                                    nextStep = DynamicBaseLineStepsEnum.DynamicBaseLineFill

                                ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "END") And
                                       (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "") Then
                                    nextStep = DynamicBaseLineStepsEnum.DynamicBaseLineRead

                                ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "END") And
                                       ((_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "") OrElse
                                        (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED")) Then
                                    nextStep = DynamicBaseLineStepsEnum.DynamicBaseLineEmpty

                                ElseIf (_forceEmptyAndFinalize) AndAlso (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) <> "END") Then
                                    nextStep = DynamicBaseLineStepsEnum.DynamicBaseLineEmpty

                                End If

                            End If

                            If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "END") Then
                                nextStep = DynamicBaseLineStepsEnum.Finalize
                            End If

                        Else
                            If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "END") OrElse
                               (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                                nextStep = DynamicBaseLineStepsEnum.Finalize
                            End If
                        End If

                    End If
            End Select

            Return nextStep

        End Function
        Private Sub ProcessStaticBaseLine()
            Throw New NotImplementedException
        End Sub

        Private Sub ValidateProcess()
            Throw New NotImplementedException
        End Sub

        Private Sub ProcessDynamicBaseLine()
            Throw New NotImplementedException
        End Sub

#End Region


    End Class

End Namespace


Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Services

    Public Class DynamicBaseLineService
        Inherits AsyncService

        Sub New(analyzer As IAnalyzerManager)
            MyBase.New(analyzer)
        End Sub

        Public Overrides Function StartService() As Boolean


            'Previous conditions: (no flow here)
            If _analyzer.Connected = False Then Return False
            If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then Return False

            'Method flow:
            Dim startProcessSuccess As Boolean = False

            Dim resultData As GlobalDataTO
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            Initialize()

            If _analyzer.ExistBottleAlarms Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "CANCELED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "CANCELED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, "") 'AG + IT 10/02/2015 BA-2246 apply same rules in Change Rotor and in StartInstr
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "CANCELED")
                startProcessSuccess = False
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
                        '_analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "")
                        Throw New Exception(resultData.ErrorCode)
                    Else
                        'Update analyzer session flags into DataBase
                        If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                        End If
                        startProcessSuccess = True
                    End If
                Else
                    startProcessSuccess = False
                End If
            End If

            Return startProcessSuccess

        End Function

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

        Private _forceEmptyAndFinalize As Boolean = False
        Private _staticBaseLineFinished As Boolean = False
        Private _dynamicBaseLineValid As Boolean = False
        Private _currentStep As DynamicBaseLineStepsEnum
        Private _alreadyFinalized As Boolean = False

#End Region

#Region "Event Handlers"

        Private Sub OnReceivedStatusInformationEvent() Handles _analyzer.ReceivedStatusInformationEventHandler
            TryToStartNextStep()
        End Sub

        Private Sub OnProcessFlagEvent(ByVal pFlagCode As AnalyzerManagerFlags) Handles _analyzer.ProcessFlagEventHandler
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


#Region "Private methods"
        Private Sub Initialize()

            _forceEmptyAndFinalize = False
            _staticBaseLineFinished = False
            _dynamicBaseLineValid = False
            _analyzer.DynamicBaselineInitializationFailures = 0
            _analyzer.CurrentInstructionAction = InstructionActions.None
            _alreadyFinalized = False

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
            Debug.WriteLine("Nexi step is " & nextStep.ToString)
            Select Case nextStep
                Case DynamicBaseLineStepsEnum.ConditioningWashing,
                     DynamicBaseLineStepsEnum.StaticBaseLine,
                     DynamicBaseLineStepsEnum.DynamicBaseLineFill,
                     DynamicBaseLineStepsEnum.DynamicBaseLineEmpty
                    ValidateProcess()
            End Select

        End Sub

        Private Function GetNextStep() As DynamicBaseLineStepsEnum

            Dim nextStep = DynamicBaseLineStepsEnum.None
            If _alreadyFinalized Then Return DynamicBaseLineStepsEnum.None

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
            If nextStep <> DynamicBaseLineStepsEnum.None Then
                Debug.Print("Current doing: " & nextStep.ToString)
            End If
            Return nextStep

        End Function
        Private Sub ProcessStaticBaseLine()
            _staticBaseLineFinished = True

            If (_analyzer.BaseLineTypeForCalculations <> BaseLineType.DYNAMIC) Then
                ValidateProcess()
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ValidateProcess()

            _currentStep = GetNextStep()

            Select Case _currentStep


                Case DynamicBaseLineStepsEnum.ConditioningWashing
                    If (_analyzer.CheckIfWashingIsPossible()) Then
                        'RestartProcess() '--> This only has sense inside a ChangeRotor scenario
                        ExecuteWashingStep()
                    Else
                        CancelWashingStep()
                    End If

                Case DynamicBaseLineStepsEnum.StaticBaseLine
                    If (Not _staticBaseLineFinished) Then
                        If (_analyzer.CheckIfWashingIsPossible()) Then
                            'RestartProcess()   '--> Indica en la base de datos que se inicia el proceso de cambio de rotor. N/A aquí
                            ExecuteStaticBaseLineStep()
                        Else
                            CancelStaticBaseLineStep()
                        End If
                    Else
                        If (Not IsValidStaticBaseLine()) Then
                            CancelStaticBaseLineStep()
                            FinalizeProcess()
                        End If
                    End If


                Case DynamicBaseLineStepsEnum.DynamicBaseLineFill
                    If (_analyzer.CheckIfWashingIsPossible()) Then
                        'RestartProcess()
                        ExecuteDynamicBaseLineFillStep()
                    Else
                        CancelDynamicBaseLineFillStep()
                    End If

                Case DynamicBaseLineStepsEnum.DynamicBaseLineRead
                    'RestartProcess()
                    ExecuteDynamicBaseLineReadStep()

                Case DynamicBaseLineStepsEnum.DynamicBaseLineEmpty
                    If (IsEmptyingAllowed()) Then
                        If (_analyzer.CheckIfWashingIsPossible()) Then
                            'RestartProcess()
                            ExecuteDynamicBaseLineEmptyStep()
                        Else
                            CancelDynamicBaseLineEmptyStep()
                        End If
                    Else
                        If (IsReadingAllowed()) Then
                            'RestartProcess()
                            ExecuteDynamicBaseLineReadStep()
                        Else
                            CancelDynamicBaseLineReadStep()
                        End If
                    End If

                Case DynamicBaseLineStepsEnum.Finalize
                    FinalizeProcess()

            End Select

            Return
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteWashingStep()
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            'Case Start instrument failed before wash was performed (for example no reactions missing)
            'User execute the change reaction rotor: Sw instructions NRotor + Wash + Alight
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, "INI")
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.WASH, True) 'Send a WASH instruction (Conditioning complete)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready

            'Update analyzer session flags into DataBase
            If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CancelWashingStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess) <> "PAUSED") Or
                (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, "CANCELED")

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                End If

                _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Modified AG 15/01/2015 BA-2212
        ''' </remarks>
        Private Sub ExecuteStaticBaseLineStep()
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            'Alight
            'User execute the change reaction rotor when start instrument OK: Sw instructions NRotor + Alight

            'Delete the current reactions rotor status (configuration) and crete a new one
            Dim cfgAnReactionsRotor As New AnalyzerReactionsRotorDelegate
            Dim resultData As GlobalDataTO = cfgAnReactionsRotor.ChangeRotorPerformed(Nothing, _analyzer.ActiveAnalyzer)

            'Before send ALIGHT process ... delete the all ALIGHT/FLIGHT results
            If Not resultData.HasError Then
                Dim aLightDelg As New WSBLinesDelegate
                resultData = aLightDelg.DeleteBLinesValues(Nothing, _analyzer.ActiveAnalyzer, _analyzer.ActiveWorkSession, "", _analyzer.BaseLineTypeForCalculations.ToString) 'AG 15/01/2015 BA-2212 inform new parameter BaseLineTypeForCalculations)
                If Not resultData.HasError Then
                    'Once the conditioning is finished the Sw send an ALIGHT instruction 
                    _analyzer.ResetBaseLineFailuresCounters() 'AG 27/11/2014 BA-2066
                    'AG 12/09/2011 - when change rotor is performed the ALIGHt well starts in 1, ignore the well field in status instruction (RPalazon, JGelabert, STortosa)
                    'myGlobal = Me.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, pNextWell)
                    resultData = _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, 1)

                    'When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                    If Not resultData.HasError AndAlso _analyzer.Connected Then
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "INI")
                        _analyzer.SetAnalyzerNotReady()
                    End If
                End If
            End If

            'Update analyzer session flags into DataBase
            If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
            End If

        End Sub


        Private Sub ProcessDynamicBaseLine()
            _dynamicBaseLineValid = _analyzer.ProcessFlightReadAction()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CancelStaticBaseLineStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "CANCELED")

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                End If

                _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsValidStaticBaseLine() As Boolean
            If (_staticBaseLineFinished) Then
                Return (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) <> "CANCELED")
            Else
                Return False
            End If

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub FinalizeProcess()

            'Reentrance check:
            If _alreadyFinalized Then Return
            _alreadyFinalized = True

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            '_analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "CLOSED")

            'Remove alarm change rotor recommend
            If _analyzer.Alarms.Contains(Alarms.BASELINE_WELL_WARN) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_WELL_WARN)
            End If

            '_analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PERFORMED, 1, True) 'Inform the FALSE sensor the new rotor process is finished for UI refresh

            'AG 26/03/2012 - Special case: maybe the user was starting the instrument and the process has been
            'paused because there is not reactions rotor ... in this case when a valid alight is received
            'Sw must inform the start instrument process is OK
            If _analyzer.SessionFlag(AnalyzerManagerFlags.WUPprocess) = "PAUSED" Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WUPprocess, "INPROCESS")
                _analyzer.ValidateWarmUpProcess(myAnalyzerFlagsDs, WarmUpProcessFlag.Finalize) 'BA-2075
            End If

            'Update analyzer session flags into DataBase
            If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
            End If

            ServiceStatusCallback.Invoke(Me, ServiceStatusEnum.ServiceSuccess)

            'Clean up event listening:
            '_analyzer = Nothing
            GC.Collect()

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteDynamicBaseLineFillStep()

            _analyzer.CurrentInstructionAction = InstructionActions.FlightFilling
            Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.FillRotor), "0"})
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, myParams, String.Empty, Nothing)
            _analyzer.SetAnalyzerNotReady()

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CancelDynamicBaseLineFillStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) <> "CANCELED") Then
                '_analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "CANCELED")

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                End If

                '_analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteDynamicBaseLineReadStep()

            _analyzer.CurrentInstructionAction = InstructionActions.FlightReading
            Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.Perform), "0"})
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, myParams, String.Empty, Nothing)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready

            _dynamicBaseLineValid = False
            If _analyzer.Alarms.Contains(Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_INIT_ERR)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsEmptyingAllowed() As Boolean
            Return (_dynamicBaseLineValid Or _forceEmptyAndFinalize)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteDynamicBaseLineEmptyStep()

            _analyzer.CurrentInstructionAction = InstructionActions.FlightEmptying
            Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.EmptyRotor), "0"})
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, myParams, String.Empty, Nothing)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready
            _forceEmptyAndFinalize = False

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CancelDynamicBaseLineEmptyStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "CANCELED")

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                End If

                _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsReadingAllowed() As Boolean
            Return (_analyzer.DynamicBaselineInitializationFailures < _analyzer.FlightInitFailures)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CancelDynamicBaseLineReadStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, "CANCELED")

                _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.DYNAMIC_BASELINE_ERROR, 1, True)
                '_analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.Alarms.BASELINE_INIT_ERR, 1, True)
                _analyzer.Alarms.Add(Alarms.BASELINE_INIT_ERR)

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                End If

                _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub
#End Region



    End Class

End Namespace


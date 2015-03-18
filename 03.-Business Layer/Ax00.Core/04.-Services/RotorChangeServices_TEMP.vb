Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Services.TEMP

    Public Enum RotorChangeStepsEnum
        WashStationControl
        NewRotor
        Washing
        StaticBaseLine
        DynamicBaseLineFill
        DynamicBaseLineRead
        DynamicBaseLineEmpty
        Finalize
        None
    End Enum

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  IT 19/12/2014 - BA-2143
    ''' Modified by: IT 30/01/2015 - BA-2216
    ''' </remarks>
    Public Class RotorChangeServices_TEMP
        Inherits AsyncService

        Public Sub New(analyzer As IAnalyzerManager)
            MyBase.New(analyzer)
            '_analyzer = analyzer
        End Sub

#Region "Attributes"

        'Private WithEvents _analyzer As IAnalyzerManager
        Private _currentStep As RotorChangeStepsEnum
        Private _forceEmptyAndFinalize As Boolean = False
        Private _staticBaseLineFinished As Boolean = False
        Private _dynamicBaseLineValid As Boolean = False
        Private _isInRecovering As Boolean = False

#End Region

#Region "Event Handlers"

        Public Sub OnReceivedStatusInformationEvent() Handles _analyzer.ReceivedStatusInformationEventHandler
            CheckIfCanContinue()
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

#Region "Public Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function StartService(ByVal isInRecovering As Boolean) As Boolean
            Dim resultData As GlobalDataTO
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (Not isInRecovering) Then
                Initialize()

                If _analyzer.ExistBottleAlarms Then
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NewRotor, "CANCELED")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "CANCELED")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "CANCELED")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, "") 'AG + IT 10/02/2015 BA-2246 apply same rules in Change Rotor and in StartInstr
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "CANCELED")
                    Return False
                Else
                    'TR 28/10/2011 -Turn off Sound alarm
                    _analyzer.StopAnalyzerRinging()
                    If _analyzer.Connected Then
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "INPROCESS")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NewRotor, "")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, "")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "")
                        _staticBaseLineFinished = False

                        resultData = _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, Ax00WashStationControlModes.UP, "")

                        If resultData.HasError Then
                            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "")
                            Throw New Exception(resultData.ErrorCode)
                        End If
                    Else
                        Return False
                    End If
                End If

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                End If
            Else
                InitializeRecover()
            End If

            Return True

        End Function

        ''' <summary>
        ''' Starts the change rotor service, assuming it's not in recovery mode.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function StartService() As Boolean
            Return Me.StartService(False)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContinueProcess(ByVal isInRecovering As Boolean) As Boolean
            If (_analyzer.Connected) Then 'AG 06/02/2012 - add AnalyzerController.Instance.Analyzer.Connected to the activation rule
                If (isInRecovering) Then
                    RecoverProcess()
                Else
                    ExecuteNewRotorStep()
                End If
            Else
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub RepeatDynamicBaseLineReadStep()

            _analyzer.DynamicBaselineInitializationFailures = 0

            If _analyzer.Alarms.Contains(Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_INIT_ERR)
            End If

            RestartProcess()
            ExecuteDynamicBaseLineReadStep()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub EmptyAndFinalizeProcess()
            _forceEmptyAndFinalize = True
        End Sub


        ''' <summary>
        ''' Recovers the system to a stable point after close and start application during change rotor process 'in course'
        ''' </summary>
        ''' <returns>TRUE process recovered | FALSE process could not be recovered</returns>
        ''' <remarks>
        ''' Modified by:  AG 20/01/2015 - BA-2216
        ''' </remarks>
        Public Function RecoverProcess() As Boolean
            Try
                _isInRecovering = True
                '_analyzer.CurrentInstructionAction = InstructionActions.None 'AG 04/02/2015 BA-2246 (informed in the event of USB disconnection AnalyzerManager.ProcessUSBCableDisconnection)

                Dim nextStep As RotorChangeStepsEnum
                nextStep = GetNextStep()

                Select Case nextStep
                    Case RotorChangeStepsEnum.NewRotor,
                         RotorChangeStepsEnum.DynamicBaseLineRead,
                         RotorChangeStepsEnum.Finalize
                        ValidateProcess()
                    Case RotorChangeStepsEnum.DynamicBaseLineEmpty
                        ProcessDynamicBaseLine()
                End Select

                _isInRecovering = False

                Return True

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "RotorChangeServices.RecoverProcess", EventLogEntryType.Error, False)
                Return False
            End Try
        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CheckIfCanContinue()

            If (Not _isInRecovering) Then
                Dim nextStep As RotorChangeStepsEnum
                nextStep = GetNextStep()

                Select Case nextStep
                    Case RotorChangeStepsEnum.Washing,
                         RotorChangeStepsEnum.StaticBaseLine,
                         RotorChangeStepsEnum.DynamicBaseLineFill,
                         RotorChangeStepsEnum.DynamicBaseLineEmpty
                        ValidateProcess()
                End Select
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ValidateProcess()

            _currentStep = GetNextStep()

            Select Case _currentStep

                Case RotorChangeStepsEnum.NewRotor
                    ExecuteNewRotorStep()

                Case RotorChangeStepsEnum.Washing
                    If (_analyzer.CheckIfWashingIsPossible()) Then
                        RestartProcess()
                        ExecuteWashingStep()
                    Else
                        CancelWashingStep()
                    End If

                Case RotorChangeStepsEnum.StaticBaseLine
                    If (Not _staticBaseLineFinished) Then
                        If (_analyzer.CheckIfWashingIsPossible()) Then
                            RestartProcess()
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


                Case RotorChangeStepsEnum.DynamicBaseLineFill
                    If (_analyzer.CheckIfWashingIsPossible()) Then
                        RestartProcess()
                        ExecuteDynamicBaseLineFillStep()
                    Else
                        CancelDynamicBaseLineFillStep()
                    End If

                Case RotorChangeStepsEnum.DynamicBaseLineRead
                    RestartProcess()
                    ExecuteDynamicBaseLineReadStep()

                Case RotorChangeStepsEnum.DynamicBaseLineEmpty
                    If (IsEmptyingAllowed()) Then
                        If (_analyzer.CheckIfWashingIsPossible()) Then
                            RestartProcess()
                            ExecuteDynamicBaseLineEmptyStep()
                        Else
                            CancelDynamicBaseLineEmptyStep()
                        End If
                    Else
                        If (IsReadingAllowed()) Then
                            RestartProcess()
                            ExecuteDynamicBaseLineReadStep()
                        Else
                            CancelDynamicBaseLineReadStep()
                        End If
                    End If

                Case RotorChangeStepsEnum.Finalize
                    FinalizeProcess()

            End Select

            Return
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextStep() As RotorChangeStepsEnum

            Dim nextStep = RotorChangeStepsEnum.None

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") Then
                nextStep = GetNextStepWhileInProcess()

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED") Then

                If (_analyzer.SessionFlag(AnalyzerManagerFlags.NewRotor) = "END") Then
                    If (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) = "CANCELED") Then
                        nextStep = RotorChangeStepsEnum.Washing

                    ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                        nextStep = RotorChangeStepsEnum.StaticBaseLine

                    ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "CANCELED") Then
                        nextStep = RotorChangeStepsEnum.DynamicBaseLineFill

                    ElseIf ((_isInRecovering) And (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "CANCELED") And (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "")) Then
                        nextStep = RotorChangeStepsEnum.DynamicBaseLineRead

                    ElseIf ((_forceEmptyAndFinalize) Or (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED")) Then
                        nextStep = RotorChangeStepsEnum.DynamicBaseLineEmpty

                    End If
                End If

            End If

            Return nextStep

        End Function

        Private Function GetNextStepWhileInProcess() As RotorChangeStepsEnum

            Dim nextStep As RotorChangeStepsEnum = RotorChangeStepsEnum.None

            Select Case _analyzer.SessionFlag(AnalyzerManagerFlags.NewRotor)
                Case "", "INI"
                    nextStep = RotorChangeStepsEnum.NewRotor

                Case "END"

                    Select Case _analyzer.SessionFlag(AnalyzerManagerFlags.Washing)
                        Case "", "CANCELED"
                            nextStep = RotorChangeStepsEnum.Washing

                        Case "END"
                            If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "") OrElse (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                                nextStep = RotorChangeStepsEnum.StaticBaseLine

                            Else

                                If (_analyzer.BaseLineTypeForCalculations = BaseLineType.DYNAMIC) Then

                                    If (_analyzer.CurrentInstructionAction = InstructionActions.None) Then

                                        If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "END") And
                                           ((_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "") OrElse
                                            (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "CANCELED")) Then
                                            nextStep = RotorChangeStepsEnum.DynamicBaseLineFill

                                        ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "END") And
                                               (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "") Then
                                            nextStep = RotorChangeStepsEnum.DynamicBaseLineRead

                                        ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "END") And
                                               ((_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "") OrElse
                                                (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED")) Then
                                            nextStep = RotorChangeStepsEnum.DynamicBaseLineEmpty

                                        ElseIf (_forceEmptyAndFinalize) AndAlso (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) <> "END") Then
                                            nextStep = RotorChangeStepsEnum.DynamicBaseLineEmpty

                                        End If

                                    End If

                                    If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "END") Then
                                        nextStep = RotorChangeStepsEnum.Finalize
                                    End If

                                Else
                                    If (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "END") OrElse
                                       (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                                        nextStep = RotorChangeStepsEnum.Finalize
                                    End If
                                End If

                            End If
                    End Select


            End Select

            Return nextStep

        End Function

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
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsReadingAllowed() As Boolean
            Return (_analyzer.DynamicBaselineInitializationFailures < _analyzer.FlightInitFailures)
        End Function

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
        Private Sub FinalizeProcess()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "CLOSED")

            'Remove alarm change rotor recommend
            If _analyzer.Alarms.Contains(Alarms.BASELINE_WELL_WARN) Then
                _analyzer.Alarms.Remove(Alarms.BASELINE_WELL_WARN)
            End If

            _analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.NEW_ROTOR_PERFORMED, 1, True) 'Inform the FALSE sensor the new rotor process is finished for UI refresh

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
        ''' <remarks></remarks>
        Private Sub CancelDynamicBaseLineFillStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "CANCELED")

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
        ''' <remarks></remarks>
        Private Sub RestartProcess()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "INPROCESS")

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
        ''' <remarks></remarks>
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
        Private Sub ProcessDynamicBaseLine()
            _dynamicBaseLineValid = _analyzer.ProcessFlightReadAction()
        End Sub

        ''' <summary>
        ''' Set the flags into a stable value for repeat last action and recover the process
        ''' </summary>
        ''' <remarks>Created by:  AG 20/01/2015 BA-2216
        '''          Modified by: IT 16/02/2015 BA-2266
        '''  </remarks>
        Private Sub InitializeRecover()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            Initialize()

            'NEWROTORprocess in INPROCESS status
            'If (_analyzer.SessionFlag(AnalyzerManagerFlags.NewRotor) = "") Then
            '_analyzer.UpdateSensorValuesAttribute(AnalyzerSensors.WASHSTATION_CTRL_PERFORMED, 1, True)
            If (_analyzer.SessionFlag(AnalyzerManagerFlags.NewRotor) = "INI") Then
                '_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "" 'Re-send NROTOR
            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) = "INI") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, "") 'Re-send Washing

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = "INI") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, "") 'Re-send ALIGHT

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "INI") Then
                _staticBaseLineFinished = True
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, "") 'Re-send FLIGHT mode fill

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "INI") Then
                _staticBaseLineFinished = True
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, "") 'Re-send FLIGHT mode read

            ElseIf ((_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "") Or (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "INI")) And
                (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "CANCELED") Then
                _dynamicBaseLineValid = False
                _forceEmptyAndFinalize = True
                _analyzer.Alarms.Add(Alarms.BASELINE_INIT_ERR)
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "") 'Re-send FLIGHT mode read

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "INI") Then
                _staticBaseLineFinished = True
                _dynamicBaseLineValid = True
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, "") 'Re-send FLIGHT mode empty


                'NEWROTORprocess in PAUSED status
            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED") And
                (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "CANCELED") Then
                _forceEmptyAndFinalize = True
                _dynamicBaseLineValid = False
                _analyzer.Alarms.Add(Alarms.BASELINE_INIT_ERR)

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED") And
               (_analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "END") Then
                _dynamicBaseLineValid = True


            End If

            If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
            End If

        End Sub

        ''' <summary>
        ''' Sends instruction NROTOR again
        ''' </summary>
        ''' <remarks>AG 20/01/2015 BA-2216</remarks>
        Private Sub ExecuteNewRotorStep()
            Dim resultData As GlobalDataTO
            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            resultData = _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.NROTOR, True, Nothing, Nothing, Nothing)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready

            If Not resultData.HasError Then
                _analyzer.SetSensorValue(AnalyzerSensors.NEW_ROTOR_PERFORMED) = 0 'Once instruction has been sent clear sensor
            Else
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.NEWROTORprocess, "")
                Throw New Exception(resultData.ErrorCode)
            End If

            'Update analyzer session flags into DataBase
            If myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
            End If

        End Sub

#End Region

    End Class

End Namespace
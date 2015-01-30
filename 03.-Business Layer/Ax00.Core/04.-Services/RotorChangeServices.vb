Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Services

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
    Public Class RotorChangeServices

        Public Sub New(analyzer As IAnalyzerEntity)
            _analyzer = analyzer
        End Sub

#Region "Attributes"

        Private WithEvents _analyzer As IAnalyzerEntity
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

        Public Sub OnProcessFlagEvent(ByVal pFlagCode As GlobalEnumerates.AnalyzerManagerFlags) Handles _analyzer.ProcessFlagEventHandler
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
        Public Function StartProcess(ByVal isInRecovering As Boolean) As Boolean
            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            If (Not isInRecovering) Then
                Initialize()

                If _analyzer.ExistBottleAlarms Then
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NewRotor, "CANCELED")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "CANCELED")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "CANCELED")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read, "CANCELED")
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty, "CANCELED")
                    Return False
                Else
                    'TR 28/10/2011 -Turn off Sound alarm
                    _analyzer.StopAnalyzerRinging()
                    If _analyzer.Connected Then
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "INPROCESS")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NewRotor, "")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read, "")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty, "")
                        _staticBaseLineFinished = False

                        resultData = _analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH_STATION_CTRL, True, Nothing, GlobalEnumerates.Ax00WashStationControlModes.UP, "")

                        If resultData.HasError Then
                            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "")
                            Throw New Exception(resultData.ErrorCode)
                        End If
                    Else
                        Return False
                    End If
                End If

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If
            Else
                InitializeRecover()
            End If

            Return True

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContinueProcess(ByVal isInRecovering As Boolean) As Boolean
            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

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

            If _analyzer.Alarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)
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

                Dim nextStep As RotorChangeStepsEnum
                nextStep = GetNextStep()

                Select nextStep
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
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "RotorChangeServices.RecoverProcess", EventLogEntryType.Error, False)
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
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ValidateProcess() As Boolean

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

            Return True

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextStep() As RotorChangeStepsEnum

            If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS") Then

                'AG 20/01/2015 BA-2216
                If ((_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "") Or
                    (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "INI")) Then
                    Return RotorChangeStepsEnum.NewRotor
                End If

                If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "END") Then

                    If ((_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "") OrElse
                        (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "CANCELED")) Then 'Send Wash
                        Return RotorChangeStepsEnum.Washing
                    End If

                    If ((_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "END") AndAlso
                        ((_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "") OrElse _
                        (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "CANCELED"))) Then
                        Return RotorChangeStepsEnum.StaticBaseLine
                    End If

                End If

                If (_analyzer.BaseLineTypeForCalculations = BaseLineType.DYNAMIC) Then

                    If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "END") And
                        ((_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "") OrElse (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "CANCELED")) And
                        (_analyzer.CurrentInstructionAction = InstructionActions.None) Then
                        Return RotorChangeStepsEnum.DynamicBaseLineFill
                    End If

                    If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "END") And
                        (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "") And
                        (_analyzer.CurrentInstructionAction = InstructionActions.None) Then
                        Return RotorChangeStepsEnum.DynamicBaseLineRead
                    End If

                    If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "END") And
                        ((_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "") OrElse (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED")) And
                        (_analyzer.CurrentInstructionAction = InstructionActions.None) Then
                        Return RotorChangeStepsEnum.DynamicBaseLineEmpty
                    End If

                    If (_forceEmptyAndFinalize) And (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) <> "END") And (_analyzer.CurrentInstructionAction = InstructionActions.None) Then
                        Return RotorChangeStepsEnum.DynamicBaseLineEmpty
                    End If

                    If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "END") Then
                        Return RotorChangeStepsEnum.Finalize
                    End If
                Else
                    If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "END") OrElse
                        (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                        Return RotorChangeStepsEnum.Finalize
                    End If
                End If
            End If

            If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED") Then

                If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "END") Then
                    If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "CANCELED") Then
                        Return RotorChangeStepsEnum.Washing
                    End If

                    If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "CANCELED") Then
                        Return RotorChangeStepsEnum.StaticBaseLine
                    End If

                    If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "CANCELED") Then
                        Return RotorChangeStepsEnum.DynamicBaseLineFill
                    End If

                    If ((_isInRecovering) And (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "CANCELED") And (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "")) Then
                        Return RotorChangeStepsEnum.DynamicBaseLineRead
                    End If

                    If ((_forceEmptyAndFinalize) Or (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED")) Then
                        Return RotorChangeStepsEnum.DynamicBaseLineEmpty
                    End If
                End If

            End If

            Return RotorChangeStepsEnum.None

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsEmptyingAllowed()
            Return (_dynamicBaseLineValid Or _forceEmptyAndFinalize)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsReadingAllowed()
            Return (_analyzer.DynamicBaselineInitializationFailures < _analyzer.FlightInitFailures)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsValidStaticBaseLine()
            If (_staticBaseLineFinished) Then
                Return (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) <> "CANCELED")
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
            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            'Alight
            'User execute the change reaction rotor when start instrument OK: Sw instructions NRotor + Alight

            'Delete the current reactions rotor status (configuration) and crete a new one
            Dim cfgAnReactionsRotor As New AnalyzerReactionsRotorDelegate
            resultData = cfgAnReactionsRotor.ChangeRotorPerformed(Nothing, _analyzer.ActiveAnalyzer)

            'Before send ALIGHT process ... delete the all ALIGHT/FLIGHT results
            If Not resultData.HasError Then
                Dim ALightDelg As New WSBLinesDelegate
                resultData = ALightDelg.DeleteBLinesValues(Nothing, _analyzer.ActiveAnalyzer, _analyzer.ActiveWorkSession, "", _analyzer.BaseLineTypeForCalculations.ToString) 'AG 15/01/2015 BA-2212 inform new parameter BaseLineTypeForCalculations)
                If Not resultData.HasError Then
                    'Once the conditioning is finished the Sw send an ALIGHT instruction 
                    _analyzer.ResetBaseLineFailuresCounters() 'AG 27/11/2014 BA-2066
                    'AG 12/09/2011 - when change rotor is performed the ALIGHt well starts in 1, ignore the well field in status instruction (RPalazon, JGelabert, STortosa)
                    'myGlobal = Me.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, pNextWell)
                    resultData = _analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_LIGHT, True, Nothing, 1)

                    'When a process involve an instruction sending sequence automatic (for instance STANDBY (end) + WASH) change the AnalyzerIsReady value
                    If Not resultData.HasError AndAlso _analyzer.Connected Then
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "INI")
                        _analyzer.SetAnalyzerNotReady()
                    End If
                End If
            End If

            'Update analyzer session flags into DataBase
            If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteWashingStep()
            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            'Case Start instrument failed before wash was performed (for example no reactions missing)
            'User execute the change reaction rotor: Sw instructions NRotor + Wash + Alight
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "INI")
            resultData = _analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.WASH, True) 'Send a WASH instruction (Conditioning complete)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready

            'Update analyzer session flags into DataBase
            If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteDynamicBaseLineEmptyStep()

            _analyzer.CurrentInstructionAction = InstructionActions.FlightEmptying
            Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.EmptyRotor), "0"})
            _analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, Nothing, String.Empty, myParams)
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
            _analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, Nothing, String.Empty, myParams)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready

            _dynamicBaseLineValid = False

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteDynamicBaseLineFillStep()

            _analyzer.CurrentInstructionAction = InstructionActions.FlightFilling
            Dim myParams As New List(Of String)(New String() {CStr(Ax00FlightAction.FillRotor), "0"})
            _analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ADJUST_FLIGHT, True, Nothing, Nothing, String.Empty, myParams)
            _analyzer.SetAnalyzerNotReady()

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub FinalizeProcess()
            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "CLOSED")

            'Remove alarm change rotor recommend
            If _analyzer.Alarms.Contains(GlobalEnumerates.Alarms.BASELINE_WELL_WARN) Then
                _analyzer.Alarms.Remove(GlobalEnumerates.Alarms.BASELINE_WELL_WARN)
            End If

            _analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PERFORMED, 1, True) 'Inform the FALSE sensor the new rotor process is finished for UI refresh

            'AG 26/03/2012 - Special case: maybe the user was starting the instrument and the process has been
            'paused because there is not reactions rotor ... in this case when a valid alight is received
            'Sw must inform the start instrument process is OK
            If _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "PAUSED" Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.WUPprocess, "INPROCESS")
                _analyzer.ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.Finalize) 'BA-2075
            End If

            'Update analyzer session flags into DataBase
            If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CancelWashingStep()

            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) <> "PAUSED") Or
                (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "CANCELED")

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If

                _analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CancelStaticBaseLineStep()

            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "CANCELED")

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If

                _analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CancelDynamicBaseLineFillStep()

            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "CANCELED")

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If

                _analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CancelDynamicBaseLineReadStep()

            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read, "CANCELED")

                _analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.DYNAMIC_BASELINE_ERROR, 1, True)
                '_analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.Alarms.BASELINE_INIT_ERR, 1, True)
                _analyzer.Alarms.Add(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If

                _analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CancelDynamicBaseLineEmptyStep()

            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty, "CANCELED")

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If

                _analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RestartProcess()

            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "INPROCESS")

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If

                _analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PROCESS_STATUS_CHANGED, 1, True)
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

            If _analyzer.Alarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) Then
                _analyzer.Alarms.Remove(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)
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
        ''' <remarks>AG 20/01/2015 BA-2216</remarks>
        Private Sub InitializeRecover()

            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            Initialize()

            _analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WASHSTATION_CTRL_PERFORMED, 1, True)

            'NEWROTORprocess in INPROCESS status
            If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "INI") Then
                '_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NewRotor) = "" 'Re-send NROTOR
            ElseIf (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "INI") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.Washing, "") 'Re-send Washing

            ElseIf (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "INI") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.BaseLine, "") 'Re-send ALIGHT

            ElseIf (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "INI") Then
                _staticBaseLineFinished = True
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill, "") 'Re-send FLIGHT mode fill

            ElseIf (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "INI") Then
                _staticBaseLineFinished = True
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read, "") 'Re-send FLIGHT mode read

            ElseIf ((_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "") Or (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "INI")) And
                (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "CANCELED") Then
                _dynamicBaseLineValid = False
                _forceEmptyAndFinalize = True
                _analyzer.Alarms.Add(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty, "") 'Re-send FLIGHT mode read

            ElseIf (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "INI") Then
                _staticBaseLineFinished = True
                _dynamicBaseLineValid = True
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty, "") 'Re-send FLIGHT mode empty


                'NEWROTORprocess in PAUSED status
            ElseIf (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED") And
                (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "CANCELED") Then
                _forceEmptyAndFinalize = True
                _dynamicBaseLineValid = False
                _analyzer.Alarms.Add(GlobalEnumerates.Alarms.BASELINE_INIT_ERR)

            ElseIf (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED") And
               (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "END") Then
                _dynamicBaseLineValid = True


            End If

            If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
            End If

        End Sub

        ''' <summary>
        ''' Sends instruction NROTOR again
        ''' </summary>
        ''' <remarks>AG 20/01/2015 BA-2216</remarks>
        Private Sub ExecuteNewRotorStep()
            Dim resultData As New GlobalDataTO
            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            resultData = _analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.NROTOR, True, Nothing, Nothing, Nothing)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready

            If Not resultData.HasError Then
                _analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.NEW_ROTOR_PERFORMED) = 0 'Once instruction has been sent clear sensor
            Else
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess, "")
                Throw New Exception(resultData.ErrorCode)
            End If

            'Update analyzer session flags into DataBase
            If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
            End If

        End Sub

#End Region

    End Class

End Namespace
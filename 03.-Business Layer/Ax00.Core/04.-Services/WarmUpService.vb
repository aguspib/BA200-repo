Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.Core.Entities
Imports System.Globalization

Namespace Biosystems.Ax00.Core.Services

    Public Enum WarmUpStepsEnum
        StartInstrument
        Washing
        Barcode
        BaseLine
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
    Public Class WarmUpService
        Inherits AsyncService

#Region "Constructors"
        Public Sub New(analyzer As IAnalyzerManager)
            MyBase.New(analyzer)
            _baseLineService = New BaseLineService(_analyzer)
            _baseLineService.OnServiceStatusChange = AddressOf BaseLineStatusChanged

        End Sub
#End Region

#Region "Attributes"

        Private _currentStep As WarmUpStepsEnum
        Private _isInRecovering As Boolean = False
        Private _baseLineService As BaseLineService

#End Region

#Region "Event Handlers"

        Public Sub OnReceivedStatusInformationEvent() Handles _analyzer.ReceivedStatusInformationEventHandler
            TryToResume()
        End Sub

        Public Sub OnProcessFlagEvent(ByVal pFlagCode As AnalyzerManagerFlags) Handles _analyzer.ProcessFlagEventHandler
            Select Case pFlagCode
                Case AnalyzerManagerFlags.Washing
                    ValidateProcess()
                Case AnalyzerManagerFlags.Barcode
                    FinalizeBarcodeStep()
            End Select

        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function StartService() As Boolean


            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

            If (Not _analyzer Is Nothing) Then
                _analyzer.StopAnalyzerRinging() 'AG 29/05/2012 - Stop Analyzer sound
                _analyzer.ISEAnalyzer.IsAnalyzerWarmUp = True
                'DL 17/05/2012

                'If (CheckStatusWarmUp()) Then
                '_analyzer.ValidateWarmUpProcess(myAnalyzerFlagsDS, GlobalEnumerates.WarmUpProcessFlag.StartInstrument)
                'If (Not myGlobal.HasError) Then

                If _analyzer.Connected Then

                    If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "") Then
                        Initialize()
                        _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
                    End If

                    ValidateProcess()

                    'Start instrument instruction send OK (initialize wup UI flags)
                    'myGlobal = InitializeStartInstrument()
                Else
                    '_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = StepStringStatus.Empty
                    Return False
                End If

                'End If
            End If

            UpdateFlags(myAnalyzerFlagsDS)

            Return True

        End Function

        ' ''' <summary>
        ' ''' Starts the change rotor service, assuming it's not in recovery mode.
        ' ''' </summary>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Overrides Function StartService() As Boolean
        '    Return Me.StartService(False)
        'End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub RepeatDynamicBaseLineReadStep()

            'RestartProcess()

            _baseLineService.RepeatDynamicBaseLineReadStep()

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub EmptyAndFinalizeProcess()
            _baseLineService.EmptyAndFinalizeProcess()
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

                InitializeRecover()
                ''_analyzer.CurrentInstructionAction = InstructionActions.None 'AG 04/02/2015 BA-2246 (informed in the event of USB disconnection AnalyzerManager.ProcessUSBCableDisconnection)

                Dim nextStep As WarmUpStepsEnum
                nextStep = GetNextStep()

                Select Case nextStep
                    Case WarmUpStepsEnum.StartInstrument,
                        WarmUpStepsEnum.Washing,
                        WarmUpStepsEnum.Barcode,
                        WarmUpStepsEnum.Finalize
                        ValidateProcess()
                    Case Else
                        _baseLineService.RecoverProcess()
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

        Private Sub BaseLineStatusChanged(callback As IServiceStatusCallback)
            Select Case callback.Sender.Status
                Case ServiceStatusEnum.Paused
                    PauseProcess()
                Case ServiceStatusEnum.Running
                    RestartProcess()
                Case ServiceStatusEnum.EndError, ServiceStatusEnum.EndSuccess
                    FinalizeProcess()
            End Select
        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub TryToResume()

            If (Not _isInRecovering) Then
                Dim nextStep As WarmUpStepsEnum
                nextStep = GetNextStep()

                Select Case nextStep
                    Case WarmUpStepsEnum.Washing
                        ValidateProcess()
                    Case WarmUpStepsEnum.BaseLine
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

                Case WarmUpStepsEnum.StartInstrument
                    ExecuteStartInstrumentStep()

                Case WarmUpStepsEnum.Washing
                    If (_analyzer.CheckIfWashingIsPossible()) Then
                        RestartProcess()
                        ExecuteWashingStep()
                    Else
                        PauseProcess()
                        WashingStepPaused()
                    End If

                Case WarmUpStepsEnum.Barcode
                    ExecuteBarcodeStep()

                Case WarmUpStepsEnum.BaseLine
                    ExecuteBaseLineStep()

                Case WarmUpStepsEnum.Finalize
                    FinalizeProcess()

            End Select

        End Sub

        Private Sub ExecuteStartInstrumentStep()

            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.STANDBY, True)

        End Sub

        Private Sub ExecuteWashingStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            'Case Start instrument failed before wash was performed (for example no reactions missing)
            'User execute the change reaction rotor: Sw instructions NRotor + Wash + Alight
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, "INI")
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.WASH, True) 'Send a WASH instruction (Conditioning complete)
            _analyzer.SetAnalyzerNotReady() 'AG 20/01/2014 after send a instruction set the analyzer as not ready

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        Private Sub ExecuteBarcodeStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            ' XBC 13/02/2012 - CODEBR Configuration instruction
            'myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONFIG, True) 'AG 24/11/2011 - If Wup process FINISH sent again the config instruction (maybe user has changed something)

            Dim BarCodeDS As New AnalyzerManagerDS
            Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow
            rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
            With rowBarCode
                .RotorType = "SAMPLES"
                .Action = GlobalEnumerates.Ax00CodeBarAction.CONFIG
                .Position = 0
            End With
            BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
            BarCodeDS.AcceptChanges()

            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Barcode, "INI")
            _analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS)
            ' XBC 13/02/2012 - CODEBR Configuration instruction

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        Private Sub ExecuteBaseLineStep()
            _baseLineService.StartService()
        End Sub

        Private Sub ExecuteConfigStep()

            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.CONFIG, True) 'AG 24/11/2011 - If Wup process canceled sent again the config instruction (maybe user has changed something)
            'analyzerReadyFlagMustBeSetToFALSE =True 'AG 16/01/2015 BA-2170 Not required! It is an inmediate instruction

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextStep() As WarmUpStepsEnum

            Dim nextStep As WarmUpStepsEnum

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.WUPprocess) = "INPROCESS") Then
                nextStep = GetNextStepWhileInProcess()

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.WUPprocess) = "PAUSED") Then
                nextStep = GetNextStepWhilePaused()
            Else
                nextStep = WarmUpStepsEnum.None
            End If

            Return nextStep

            '-----------------------------------------------------------







        End Function

        Private Function GetNextStepWhilePaused() As WarmUpStepsEnum

            Dim nextStep As WarmUpStepsEnum = WarmUpStepsEnum.None

            Select Case _analyzer.SessionFlag(AnalyzerManagerFlags.Washing)
                Case StepStringStatus.Canceled
                    nextStep = WarmUpStepsEnum.Washing
            End Select

            Return nextStep

        End Function

        Private Function GetNextStepWhileInProcess() As WarmUpStepsEnum

            Dim nextStep As WarmUpStepsEnum = WarmUpStepsEnum.None

            Select Case _analyzer.SessionFlag(AnalyzerManagerFlags.StartInstrument)
                Case StepStringStatus.Empty
                    nextStep = WarmUpStepsEnum.StartInstrument

                Case StepStringStatus.Ended
                    Select Case _analyzer.SessionFlag(AnalyzerManagerFlags.Washing)
                        Case StepStringStatus.Empty
                            nextStep = WarmUpStepsEnum.Washing

                        Case StepStringStatus.Ended

                            Select Case _analyzer.SessionFlag(AnalyzerManagerFlags.Barcode)
                                Case StepStringStatus.Empty
                                    nextStep = WarmUpStepsEnum.Barcode

                                Case StepStringStatus.Ended
                                    Select Case _baseLineService.Status
                                        Case ServiceStatusEnum.NotYetStarted, ServiceStatusEnum.EndError
                                            nextStep = WarmUpStepsEnum.BaseLine

                                        Case ServiceStatusEnum.EndSuccess
                                            nextStep = WarmUpStepsEnum.Finalize

                                        Case ServiceStatusEnum.Running
                                            nextStep = WarmUpStepsEnum.None

                                    End Select
                            End Select
                    End Select
            End Select

            Return nextStep

        End Function


        Private Function CheckStatusWarmUp() As Boolean

            If (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "PAUSED") Then

                If _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.StartInstrument) = "END" AndAlso
                    _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "CANCELED" Then
                    _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
                    _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = ""
                    _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
                End If

                If _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "END" AndAlso
                    _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "CANCELED" Then
                    _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
                    _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = ""
                    _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
                End If

                'If _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "END" AndAlso
                ' _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = "CANCELED" Then
                '_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
                '_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = ""
                'analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
                'End If

                'If _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read) = "END" AndAlso
                '_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = "CANCELED" Then
                '_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
                '_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty) = ""
                '_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = ""
                'End If

                Return False

            Else
                _analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 0 'clear sensor
                _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "" 'Reset flag
                _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "" 'Reset flag
                _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine) = "" 'Reset flag
                _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"

                Return True
            End If

        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub FinalizeProcess()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
            Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow
            Dim myAlarm As Alarms
            Dim wupManeuversFinishFlag As Boolean = False 'AG 23/05/2012

            'WUPCOMPLETEFLAG
            myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            With myAnalyzerSettingsRow
                .AnalyzerID = _analyzer.ActiveAnalyzer
                .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
                .CurrentValue = "1"
            End With
            myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

            Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
            myGlobalDataTO = myAnalyzerSettings.Save(Nothing, _analyzer.ActiveAnalyzer, myAnalyzerSettingsDS, Nothing)

            wupManeuversFinishFlag = True

            '' XBC 13/02/2012 - CODEBR Configuration instruction
            If (_analyzer.SessionFlag(AnalyzerManagerFlags.Barcode) = StepStringStatus.Empty) Then
                ExecuteBarcodeStep()
            End If
            '' XBC 13/02/2012 - CODEBR Configuration instruction

            'AG 23/05/2012
            If wupManeuversFinishFlag Then
                'Inform the presentation layer to activate the STOP wup button
                _analyzer.UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED, 1, True)
                _analyzer.ISEAnalyzer.IsAnalyzerWarmUp = False 'AG 22/05/2012 - ISE alarms ready to be shown

                'AG 23/05/2012 - Evaluate if Sw has to recommend to change reactions rotor (remember all alarms are remove when Start Instruments starts)
                Dim analyzerReactRotor As New AnalyzerReactionsRotorDelegate
                myGlobalDataTO = analyzerReactRotor.ChangeReactionsRotorRecommended(Nothing, _analyzer.ActiveAnalyzer, _analyzer.Model)
                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                    If CType(myGlobalDataTO.SetDatos, String) <> "" Then
                        myAlarm = AlarmEnumerates.Alarms.BASELINE_WELL_WARN
                        'Update internal alarm list if exists alarm but not saved it into database!!!
                        'But generate refresh
                        If Not _analyzer.Alarms.Contains(myAlarm) Then
                            _analyzer.Alarms.Add(myAlarm)
                            myGlobalDataTO = _analyzer.PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, myAlarm.ToString, True)
                        End If
                    End If
                End If
            End If
            'AG 23/05/2012

            CheckIseAlarms()

            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WUPprocess, "CLOSED")
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        'Private Function StartProcess() As Boolean

        '    Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

        '    If (Not _analyzer Is Nothing) Then
        '        _analyzer.StopAnalyzerRinging() 'AG 29/05/2012 - Stop Analyzer sound
        '        _analyzer.ISEAnalyzer.IsAnalyzerWarmUp = True
        '        'DL 17/05/2012

        '        'If (CheckStatusWarmUp()) Then
        '        '_analyzer.ValidateWarmUpProcess(myAnalyzerFlagsDS, GlobalEnumerates.WarmUpProcessFlag.StartInstrument)
        '        'If (Not myGlobal.HasError) Then

        '        If _analyzer.Connected Then

        '            _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS"
        '            Initialize()
        '            'ValidateProcess()
        '            'Start instrument instruction send OK (initialize wup UI flags)
        '            'myGlobal = InitializeStartInstrument()
        '        Else
        '            '_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = StepStringStatus.Empty
        '            Return False
        '        End If

        '        'End If
        '    End If

        '    UpdateFlags(myAnalyzerFlagsDS)

        '    Return True


        'End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RestartProcess()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.WUPprocess) = "PAUSED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WUPprocess, "INPROCESS")
                Status = ServiceStatusEnum.Running

                'Update analyzer session flags into DataBase
                UpdateFlags(myAnalyzerFlagsDs)

            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Initialize()

            Dim myGlobal As GlobalDataTO = Nothing
            'Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'DL 09/09/2011
                'Set Enable (or set visible meeting 12/09/2011 ??) frame time W-Up in common Monitor
                'IMonitor.bsWamUpGroupBox.Enabled = True
                Dim swParams As New SwParametersDelegate

                ' Read W-Up full time configuration
                myGlobal = swParams.ReadByAnalyzerModel(Nothing, _analyzer.Model)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    Dim myParametersDS As New ParametersDS

                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    Dim myList As New List(Of ParametersDS.tfmwSwParametersRow)

                    myList = (From a As ParametersDS.tfmwSwParametersRow In myParametersDS.tfmwSwParameters _
                              Where String.Equals(a.ParameterName, GlobalEnumerates.SwParameters.WUPFULLTIME.ToString) _
                              Select a).ToList

                    Dim WUPFullTime As Single
                    If myList.Count > 0 Then WUPFullTime = myList(0).ValueNumeric '= DateTime.Now.ToString("yyyyMMdd hh-mm") & "_" & myList(0).ValueText
                End If

                ' Save initial states when press over w-up
                If Not myGlobal.HasError Then
                    Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
                    Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

                    'WUPCOMPLETEFLAG
                    myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                    With myAnalyzerSettingsRow
                        .AnalyzerID = _analyzer.ActiveAnalyzer
                        .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
                        .CurrentValue = "0"
                    End With
                    myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                    'WUPCOMPLETEFLAG
                    myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                    With myAnalyzerSettingsRow
                        .AnalyzerID = _analyzer.ActiveAnalyzer
                        .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString()
                        ''.CurrentValue = Now.ToString 'AG + SA 05/10/2012
                        '.CurrentValue = Now.ToString("yyyy/MM/dd HH:mm:ss")
                        .CurrentValue = Now.ToString(CultureInfo.InvariantCulture)
                    End With
                    myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                    Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
                    myGlobal = myAnalyzerSettings.Save(Nothing, _analyzer.ActiveAnalyzer, myAnalyzerSettingsDS, Nothing)

                End If

                _analyzer.ISEAnalyzer.IsAnalyzerWarmUp = True 'SGM 13/04/2012 

            Catch ex As Exception
                Throw ex
            End Try

            'Return myGlobal
            '-------------------------------------------
        End Sub


        ''' <summary>
        ''' Set the flags into a stable value for repeat last action and recover the process
        ''' </summary>
        ''' <remarks>Created by:  AG 20/01/2015 BA-2216
        '''          Modified by: IT 16/02/2015 BA-2266
        '''  </remarks>
        Private Sub InitializeRecover()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
            'Dim stableSetupAchieved As Boolean = True   '/True means that is not necessary any action to reachs a stable setup 
            '/False means that some actions to reach stable setup has been started

            Initialize()

            '-----------------------------------------------------------------------------------
            If (_analyzer.SessionFlag(AnalyzerManagerFlags.StartInstrument) = StepStringStatus.Initialized) Then
                '1.	Re-send STANDBY instruction. Requires analyzer status SLEEP
                If (_analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING) Then
                    'AG 20/01/2015 BA-2216
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.StartInstrument, StepStringStatus.Ended)

                    'AG 20/01/2015 BA-2216 - new conditions
                ElseIf _analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                    'stableSetupAchieved = False
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.StartInstrument, StepStringStatus.Empty)
                    '_analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, StepStringStatus.Empty)
                    'ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.Wash) 'AG 06/02/2015 This step will be called during liquid levels validation in the ANSINF treatment, method UserSwANSINFTreatment
                    'AG 20/01/2015 BA-2216
                End If

                'AG 20/01/2015 BA-2216 - changes conditions
            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) = StepStringStatus.Initialized) Then
                '2.	Re-send WASH instruction. Requires analyzer status STANDBY
                If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                    'stableSetupAchieved = False
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, StepStringStatus.Empty)
                    'ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.Wash) 'AG 06/02/2015 This step will be called during liquid levels validation in the ANSINF treatment, method UserSwANSINFTreatment
                End If

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.Barcode) = StepStringStatus.Initialized) Then
                '2.	Re-send WASH instruction. Requires analyzer status STANDBY
                If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                    'stableSetupAchieved = False
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Barcode, StepStringStatus.Empty)
                    'ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.Wash) 'AG 06/02/2015 This step will be called during liquid levels validation in the ANSINF treatment, method UserSwANSINFTreatment
                End If

                'ElseIf _analyzer.SessionFlag(AnalyzerManagerFlags.BaseLine) = StepStringStatus.Initialized Then
                '    '3.	Re-send ALIGHT instruction (well CurrentWellAttribute). Requires analyzer status STANDBY
                '    If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                '        'stableSetupAchieved = False
                '        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.BaseLine, StepStringStatus.Empty)
                '        'ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.ProcessStaticBaseLine) 'AG 06/02/2015 This step will be called during liquid levels validation in the ANSINF treatment, method UserSwANSINFTreatment
                '    End If

                '    'AG 06/02/2015 BA-2246 previous step is set to END using instruction <> STATUS. Add also case new step flag = ""
                'ElseIf _analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill) = StepStringStatus.Initialized OrElse _analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = StepStringStatus.Empty Then
                '    '3.	Re-send FLIGHT instruction in mode fill
                '    If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                '        'stableSetupAchieved = False
                '        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Fill, StepStringStatus.Empty)
                '        'ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.ProcessDynamicBaseLine) 'AG 06/02/2015 This step will be called during liquid levels validation in the ANSINF treatment, method UserSwANSINFTreatment
                '    End If
                'ElseIf _analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = StepStringStatus.Initialized Then
                '    '3.	Re-send FLIGHT instruction in mode read
                '    If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                '        'stableSetupAchieved = False
                '        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Read, StepStringStatus.Empty)
                '        'ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.ProcessDynamicBaseLine) 'AG 06/02/2015 This step will be called during liquid levels validation in the ANSINF treatment, method UserSwANSINFTreatment
                '    End If
                'ElseIf _analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = StepStringStatus.Initialized Then
                '    '3.	Re-send FLIGHT instruction in mode empty
                '    If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                '        'stableSetupAchieved = False
                '        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.DynamicBL_Empty, StepStringStatus.Empty)
                '        'ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.ProcessDynamicBaseLine) 'AG 06/02/2015 This step will be called during liquid levels validation in the ANSINF treatment, method UserSwANSINFTreatment
                '    End If


            End If

            UpdateFlags(myAnalyzerFlagsDs)

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

        Private Sub FinalizeBarcodeStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            ExecuteConfigStep()

            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Barcode, "END")
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        Private Sub PauseProcess()

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.WUPprocess) <> "PAUSED") Then
                Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WUPprocess, "PAUSED")
                Status = ServiceStatusEnum.Paused

                'Update analyzer session flags into DataBase
                UpdateFlags(myAnalyzerFlagsDs)
            End If

        End Sub

        Private Sub WashingStepPaused()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) <> "CANCELED") Then

                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, "CANCELED")
                ExecuteConfigStep()

                'Update analyzer session flags into DataBase
                UpdateFlags(myAnalyzerFlagsDs)
            End If

        End Sub


        Private Sub CheckIseAlarms()

            Dim myGlobalDataTO As New GlobalDataTO
            Dim AlarmList As New List(Of AlarmEnumerates.Alarms)
            Dim AlarmStatusList As New List(Of Boolean)

            'AG 16/05/2012 - If warm up maneuvers are finished check for the ise alarms 
            If ((_analyzer.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) <> Nothing) AndAlso _analyzer.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 1) Then

                Dim tempISEAlarmList As New List(Of AlarmEnumerates.Alarms)
                Dim tempISEAlarmStatusList As New List(Of Boolean)
                myGlobalDataTO = _analyzer.ISEAnalyzer.CheckAlarms(_analyzer.Connected, tempISEAlarmList, tempISEAlarmStatusList)

                AlarmList.Clear()
                AlarmStatusList.Clear()
                For i As Integer = 0 To tempISEAlarmList.Count - 1
                    _analyzer.PrepareLocalAlarmList(tempISEAlarmList(i), tempISEAlarmStatusList(i), AlarmList, AlarmStatusList)
                Next

                If AlarmList.Count > 0 Then
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ' XBC 16/10/2012 - Alarms treatment for Service
                        ' Not Apply
                        'myGlobalDataTO = ManageAlarms_SRV(dbConnection, AlarmList, AlarmStatusList)
                    Else
                        Dim currentAlarms = New CurrentAlarms(_analyzer)
                        myGlobalDataTO = currentAlarms.Manage(AlarmList, AlarmStatusList)
                    End If
                End If
            End If
            'AG 16/05/2012

        End Sub

#End Region

    End Class

End Namespace
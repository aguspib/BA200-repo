﻿Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.Core.Entities
Imports System.Globalization
Imports Biosystems.Ax00.Core.Services.Enums
Imports Biosystems.Ax00.Core.Services.Interfaces

Namespace Biosystems.Ax00.Core.Services
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  IT 19/12/2014 - BA-2406
    ''' </remarks>
    Public Class WarmUpService
        Inherits AsyncService
        Implements IWarmUpService

#Region "Constructors"

        Public Sub New(analyzer As IAnalyzerManager, myFlagsDelg As IAnalyzerManagerFlagsDelegate)
            Me.New(analyzer, New BaseLineService(analyzer, myFlagsDelg), myFlagsDelg)
        End Sub

        Public Sub New(analyzer As IAnalyzerManager, baseLineService As IBaseLineService, myFlagsDelg As IAnalyzerManagerFlagsDelegate)
            MyBase.New(analyzer, myFlagsDelg)
            _baseLineService = baseLineService
            _baseLineService.OnServiceStatusChange = AddressOf BaseLineStatusChanged
        End Sub

#End Region

#Region "Attributes"

        Private _currentStep As WarmUpStepsEnum
        Private _isInRecovering As Boolean = False
        Private _baseLineService As IBaseLineService
        Private _eventHandlersAdded As Boolean = False

#End Region

#Region "Event Handlers"

        Public Sub OnReceivedStatusInformationEvent()
            TryToResume()
        End Sub

        Public Sub OnProcessFlagEvent(ByVal pFlagCode As AnalyzerManagerFlags)
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

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (Not _analyzer Is Nothing) Then
                _analyzer.StopAnalyzerRinging() 'AG 29/05/2012 - Stop Analyzer sound
                _analyzer.ISEAnalyzer.IsAnalyzerWarmUp = True

                If _analyzer.Connected Then
                    If ((_analyzer.SessionFlag(AnalyzerManagerFlags.WUPprocess) = String.Empty) OrElse (_analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "CLOSED")) Then

                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WUPprocess, "INPROCESS")
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.StartInstrument, StepStringStatus.Empty)
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, StepStringStatus.Empty)
                        _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Barcode, StepStringStatus.Empty)

                        'Update analyzer session flags into DataBase
                        UpdateFlags(myAnalyzerFlagsDs)

                        Status = ServiceStatusEnum.Running
                        Initialize()
                        AddRequiredEventHandlers()
                    End If

                    ValidateProcess()
                Else
                    Return False
                End If

            End If

            Return True

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub PauseService()

            If (Status <> ServiceStatusEnum.Running) Then
                RemoveRequiredEventHandlers()
                _baseLineService.PauseService()
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub RestartService()

            AddRequiredEventHandlers()
            _baseLineService.RestartService()

        End Sub

        ''' <summary>
        ''' Recovers the system to a stable point after close and start application during change rotor process 'in course'
        ''' </summary>
        ''' <returns>TRUE process recovered | FALSE process could not be recovered</returns>
        ''' <remarks>
        ''' </remarks>
        Public Function RecoverProcess() As Boolean Implements IWarmUpService.RecoverProcess
            Try
                _isInRecovering = True

                InitializeRecover()

                Dim nextStep As WarmUpStepsEnum
                nextStep = GetNextStep()

                Select Case nextStep
                    Case WarmUpStepsEnum.StartInstrument,
                        WarmUpStepsEnum.Barcode,
                        WarmUpStepsEnum.Finalize
                        ValidateProcess()
                    Case Else
                        If (nextStep <> WarmUpStepsEnum.Washing) Then
                            _baseLineService.DecideToReuseRotorContents = ReuseContentsForBaseLineCallback
                            _baseLineService.RecoverProcess()
                        End If
                End Select

                _isInRecovering = False
                AddRequiredEventHandlers()

                Return True

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "RotorChangeServices.RecoverProcess", EventLogEntryType.Error, False)
                Return False
            End Try
        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="callback"></param>
        ''' <remarks></remarks>
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
                        If Status <> ServiceStatusEnum.Paused Then
                            ValidateProcess()
                        End If
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
                    If (Not ExistsBottleAlarmsOrRotorIsMissing()) Then
                        RestartProcess()
                        ExecuteWashingStep()
                    Else
                        PauseProcess()
                        WashingStepPaused()
                    End If

                Case WarmUpStepsEnum.Barcode
                    ExecuteBarcodeStep()

                Case WarmUpStepsEnum.BaseLine
#If DEBUG Then
                    If Not StatusParameters.IsActive Then
                        Dim result = System.Windows.Forms.MessageBox.Show("Do you want to perform the whole baseline process?", "Baseline shortcut question", System.Windows.Forms.MessageBoxButtons.YesNo)
                        If result = System.Windows.Forms.DialogResult.Yes Then
                            ExecuteBaseLineStep()
                        Else
                            _baseLineService.EmptyAndFinalizeProcess()
                            _baseLineService.Status = ServiceStatusEnum.EndSuccess
                            _analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Fill) = "END"
                            _analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Read) = "END"
                            _analyzer.SessionFlag(AnalyzerManagerFlags.DynamicBL_Empty) = "END"
                            _currentStep = WarmUpStepsEnum.Finalize
                            FinalizeProcess()
                        End If
                    Else
                        ExecuteBaseLineStep()
                    End If
#Else
                                        ExecuteBaseLineStep()
#End If

                Case WarmUpStepsEnum.Finalize
                    FinalizeProcess()

            End Select

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteStartInstrumentStep()
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.STANDBY, True)
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
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteBarcodeStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            ' XBC 13/02/2012 - CODEBR Configuration instruction
            'myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONFIG, True) 'AG 24/11/2011 - If Wup process FINISH sent again the config instruction (maybe user has changed something)

            Dim barCodeDs As New AnalyzerManagerDS
            Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow
            rowBarCode = barCodeDs.barCodeRequests.NewbarCodeRequestsRow
            With rowBarCode
                .RotorType = "SAMPLES"
                .Action = GlobalEnumerates.Ax00CodeBarAction.CONFIG
                .Position = 0
            End With
            barCodeDs.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
            barCodeDs.AcceptChanges()

            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Barcode, "INI")
            _analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, barCodeDs)
            _analyzer.SetAnalyzerNotReady()
            ' XBC 13/02/2012 - CODEBR Configuration instruction

            'Update analyzer session flags into DataBase
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteBaseLineStep()
            _baseLineService.DecideToReuseRotorContents = ReuseContentsForBaseLineCallback
            _baseLineService.StartService()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub ExecuteConfigStep()
            _analyzer.ManageAnalyzer(AnalyzerManagerSwActionList.CONFIG, True) 'AG 24/11/2011 - If Wup process canceled sent again the config instruction (maybe user has changed something)
            _analyzer.SetAnalyzerNotReady()
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

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextStepWhilePaused() As WarmUpStepsEnum

            Dim nextStep As WarmUpStepsEnum = WarmUpStepsEnum.None

            Select Case _analyzer.SessionFlag(AnalyzerManagerFlags.Washing)
                Case StepStringStatus.Canceled
                    nextStep = WarmUpStepsEnum.Washing
            End Select

            Return nextStep

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub FinalizeProcess() Implements IWarmUpService.FinalizeProcess

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
            Dim myGlobalDataTo As New GlobalDataTO
            Dim myAnalyzerSettingsDs As New AnalyzerSettingsDS
            Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow
            Dim myAlarm As Alarms
            Dim wupManeuversFinishFlag As Boolean = False 'AG 23/05/2012

            'WUPCOMPLETEFLAG
            myAnalyzerSettingsRow = myAnalyzerSettingsDs.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            With myAnalyzerSettingsRow
                .AnalyzerID = _analyzer.ActiveAnalyzer
                .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
                .CurrentValue = "1"
            End With
            myAnalyzerSettingsDs.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

            myGlobalDataTo = AnalyzerSettingsDelegate.Save(Nothing, _analyzer.ActiveAnalyzer, myAnalyzerSettingsDs, Nothing)

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
                myGlobalDataTo = analyzerReactRotor.ChangeReactionsRotorRecommended(Nothing, _analyzer.ActiveAnalyzer, _analyzer.Model)
                If Not myGlobalDataTo.HasError AndAlso Not myGlobalDataTo.SetDatos Is Nothing Then
                    If CType(myGlobalDataTo.SetDatos, String) <> "" Then
                        myAlarm = AlarmEnumerates.Alarms.BASELINE_WELL_WARN
                        'Update internal alarm list if exists alarm but not saved it into database!!!
                        'But generate refresh
                        If Not _analyzer.Alarms.Contains(myAlarm) Then
                            _analyzer.Alarms.Add(myAlarm)
                            myGlobalDataTo = _analyzer.PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, myAlarm.ToString, True)
                        End If
                    End If
                End If
            End If
            'AG 23/05/2012

            CheckIseAlarms()

            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WUPprocess, "CLOSED")
            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

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

            Try
                'DL 09/09/2011
                'Set Enable (or set visible meeting 12/09/2011 ??) frame time W-Up in common Monitor
                Dim swParams As New SwParametersDelegate

                ' Read W-Up full time configuration
                myGlobal = swParams.ReadByAnalyzerModel(Nothing, _analyzer.Model)

                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then

                    Dim myParametersDs = CType(myGlobal.SetDatos, ParametersDS)

                    Dim myList = (From a As ParametersDS.tfmwSwParametersRow In myParametersDs.tfmwSwParameters _
                              Where String.Equals(a.ParameterName, SwParameters.WUPFULLTIME.ToString) _
                              Select a).ToList

                    Dim wUpFullTime As Single
                    If myList.Count > 0 Then wUpFullTime = myList(0).ValueNumeric
                End If

                ' Save initial states when press over w-up
                If Not myGlobal.HasError Then
                    Dim myAnalyzerSettingsDs As New AnalyzerSettingsDS
                    Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

                    'WUPCOMPLETEFLAG
                    myAnalyzerSettingsRow = myAnalyzerSettingsDs.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                    With myAnalyzerSettingsRow
                        .AnalyzerID = _analyzer.ActiveAnalyzer
                        .SettingID = AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
                        .CurrentValue = "0"
                    End With
                    myAnalyzerSettingsDs.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                    'WUPCOMPLETEFLAG
                    myAnalyzerSettingsRow = myAnalyzerSettingsDs.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
                    With myAnalyzerSettingsRow
                        .AnalyzerID = _analyzer.ActiveAnalyzer
                        .SettingID = AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString()
                        .CurrentValue = Now.ToString(CultureInfo.InvariantCulture)
                    End With
                    myAnalyzerSettingsDs.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

                    myGlobal = AnalyzerSettingsDelegate.Save(Nothing, _analyzer.ActiveAnalyzer, myAnalyzerSettingsDs, Nothing)

                End If

                _analyzer.ISEAnalyzer.IsAnalyzerWarmUp = True 'SGM 13/04/2012 

            Catch ex As Exception
                Throw
            End Try

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

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.StartInstrument) = StepStringStatus.Initialized) Then
                '1.	Re-send STANDBY instruction. Requires analyzer status SLEEP
                If (_analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING) Then
                    'AG 20/01/2015 BA-2216
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.StartInstrument, StepStringStatus.Ended)

                    'AG 20/01/2015 BA-2216 - new conditions
                ElseIf _analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.StartInstrument, StepStringStatus.Empty)
                    'AG 20/01/2015 BA-2216
                End If

                'AG 20/01/2015 BA-2216 - changes conditions
            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) = StepStringStatus.Initialized) Then
                '2.	Re-send WASH instruction. Requires analyzer status STANDBY
                If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, StepStringStatus.Empty)
                End If

            ElseIf (_analyzer.SessionFlag(AnalyzerManagerFlags.Barcode) = StepStringStatus.Initialized) Then
                '2.	Re-send WASH instruction. Requires analyzer status STANDBY
                If _analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                    _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Barcode, StepStringStatus.Empty)
                End If

            End If

            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub FinalizeBarcodeStep()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            ExecuteConfigStep()
            _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Barcode, "END")

            UpdateFlags(myAnalyzerFlagsDs)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub PauseProcess()

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.WUPprocess) <> "PAUSED") Then
                Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.WUPprocess, "PAUSED")
                Status = ServiceStatusEnum.Paused

                'Update analyzer session flags into DataBase
                UpdateFlags(myAnalyzerFlagsDs)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub WashingStepPaused()

            Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS

            If (_analyzer.SessionFlag(AnalyzerManagerFlags.Washing) <> "CANCELED") Then
                _analyzer.UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.Washing, "CANCELED")
                ExecuteConfigStep()

                'Update analyzer session flags into DataBase
                UpdateFlags(myAnalyzerFlagsDs)
            End If

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CheckIseAlarms()


            Dim alarmList As New List(Of AlarmEnumerates.Alarms)
            Dim alarmStatusList As New List(Of Boolean)

            'AG 16/05/2012 - If warm up maneuvers are finished check for the ise alarms 
            If ((_analyzer.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) <> Nothing) AndAlso _analyzer.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 1) Then

                Dim tempIseAlarmList As New List(Of AlarmEnumerates.Alarms)
                Dim tempIseAlarmStatusList As New List(Of Boolean)
                _analyzer.ISEAnalyzer.CheckAlarms(_analyzer.Connected, tempIseAlarmList, tempIseAlarmStatusList)

                alarmList.Clear()
                alarmStatusList.Clear()
                For i As Integer = 0 To tempIseAlarmList.Count - 1
                    _analyzer.PrepareLocalAlarmList(tempIseAlarmList(i), tempIseAlarmStatusList(i), alarmList, alarmStatusList)
                Next

                If alarmList.Count > 0 Then
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ' XBC 16/10/2012 - Alarms treatment for Service
                        ' Not Apply
                        'myGlobalDataTO = ManageAlarms_SRV(dbConnection, AlarmList, AlarmStatusList)
                    Else
                        Dim currentAlarms = New AnalyzerAlarms(_analyzer)
                        currentAlarms.Manage(alarmList, alarmStatusList)
                    End If
                End If
            End If
            'AG 16/05/2012

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub RemoveRequiredEventHandlers()
            If _eventHandlersAdded Then
                Try
                    RemoveHandler _analyzer.ReceivedStatusInformationEventHandler, AddressOf OnReceivedStatusInformationEvent
                    RemoveHandler _analyzer.ProcessFlagEventHandler, AddressOf OnProcessFlagEvent
                    _eventHandlersAdded = False
                Catch : End Try
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub AddRequiredEventHandlers()
            If Not _eventHandlersAdded Then
                AddHandler _analyzer.ReceivedStatusInformationEventHandler, AddressOf OnReceivedStatusInformationEvent
                AddHandler _analyzer.ProcessFlagEventHandler, AddressOf OnProcessFlagEvent
                _eventHandlersAdded = True
            End If
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            RemoveRequiredEventHandlers()
            MyBase.Dispose(disposing)
        End Sub

#End Region

#Region "Properties"
        Public Property ReuseContentsForBaseLineCallback As Action(Of BaseLineService.ReuseRotorResponse) Implements IWarmUpService.ReuseContentsForBaseLineCallback

        Public ReadOnly Property NextStep As WarmUpStepsEnum Implements IWarmUpService.NextStep
            Get
                Return _currentStep
            End Get
        End Property
#End Region

    End Class

End Namespace

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Core.Entities
    Partial Public Class AnalyzerManager

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ConnectSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            ' Set Waiting Timer Current Instruction OFF
            If GlobalConstants.REAL_DEVELOPMENT_MODE > 0 Then
                ClearQueueToSend()
            Else
                myGlobal = ProcessConnection(pSwAdditionalParameters, False)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <param name="pFwScriptId"></param>
        ''' <param name="pServiceParams"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RunningSendEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal pSwAdditionalParameters As Object, ByVal pFwScriptId As String, ByVal pServiceParams As List(Of String)) As GlobalDataTO
            Dim myGlobal As GlobalDataTO

            If Not sendingRepetitions Then
                numRepetitionsTimeout = 0
            End If
            InitializeTimerStartTaskControl(WAITING_TIME_FAST, True)
            StoreStartTaskinQueue(pAction, pSwAdditionalParameters, pFwScriptID, pServiceParams)

            myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.RUNNING)
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function PauseSendEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If AnalyzerCurrentActionAttribute <> AnalyzerManagerAx00Actions.ABORT_START AndAlso Not abortAlreadySentFlagAttribute AndAlso _
               AnalyzerCurrentActionAttribute <> AnalyzerManagerAx00Actions.PAUSE_START AndAlso Not PauseAlreadySentFlagAttribute Then

                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    If Not myInstructionsQueue.Contains(pAction) Then
                        myInstructionsQueue.Add(pAction)
                        myParamsQueue.Add(pSwAdditionalParameters)
                    End If
                Else
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.PAUSE)
                    If Not myGlobal.HasError Then useRequestFlag = False
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function StartSendEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                If Not ContinueAlreadySentFlagAttribute AndAlso _
                   (mySessionFlags(AnalyzerManagerFlags.PAUSEprocess.ToString) <> "INPROCESS") AndAlso Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.PAUSE) Then
                    If Not myInstructionsQueue.Contains(pAction) Then
                        myInstructionsQueue.Add(pAction)
                        myParamsQueue.Add(pSwAdditionalParameters)
                    End If
                End If

            Else
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.START)
                If Not myGlobal.HasError Then
                    endRunAlreadySentFlagAttribute = False
                    abortAlreadySentFlagAttribute = False
                    PauseAlreadySentFlagAttribute = False
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function EndRunSendEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If AnalyzerCurrentActionAttribute <> AnalyzerManagerAx00Actions.END_RUN_START AndAlso Not endRunAlreadySentFlagAttribute AndAlso _
               AnalyzerCurrentActionAttribute <> AnalyzerManagerAx00Actions.ABORT_START AndAlso Not abortAlreadySentFlagAttribute AndAlso _
               AnalyzerCurrentActionAttribute <> AnalyzerManagerAx00Actions.PAUSE_START AndAlso Not PauseAlreadySentFlagAttribute AndAlso _
               AnalyzerCurrentActionAttribute <> AnalyzerManagerAx00Actions.STANDBY_START Then

                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    If AllowScanInRunningAttribute Then
                        If Not stopRequestedByUserInPauseModeFlag AndAlso Not pSwAdditionalParameters Is Nothing Then
                            If TypeOf (pSwAdditionalParameters) Is Boolean Then
                                stopRequestedByUserInPauseModeFlag = DirectCast(pSwAdditionalParameters, Boolean)

                                If stopRequestedByUserInPauseModeFlag AndAlso _
                                   (AnalyzerCurrentActionAttribute = AnalyzerManagerAx00Actions.START_INSTRUCTION_START OrElse _
                                    AnalyzerCurrentActionAttribute = AnalyzerManagerAx00Actions.STANDBY_END OrElse _
                                    mySessionFlags(AnalyzerManagerFlags.StartRunning.ToString) = "INI") Then
                                    stopRequestedByUserInPauseModeFlag = False
                                End If
                            End If
                        End If
                    Else
                        stopRequestedByUserInPauseModeFlag = False
                    End If

                    If Not stopRequestedByUserInPauseModeFlag Then 'Code until v2.1.1
                        If Not myInstructionsQueue.Contains(pAction) Then
                            myInstructionsQueue.Add(pAction) 'END instruction added to queue
                            myParamsQueue.Add(Nothing)
                        End If
                    Else
                        GlobalBase.CreateLogActivity("In paused mode user decides stop the Worksession, 1st add START instruction into queue (or recovery results while analyzer is in pause mode)", "AnalyzerManager.ManageAnalyzer", EventLogEntryType.Information, False)
                        If Not myInstructionsQueue.Contains(AnalyzerManagerSwActionList.START) Then
                            myInstructionsQueue.Add(AnalyzerManagerSwActionList.START)
                            myParamsQueue.Add(Nothing)
                        End If
                    End If

                Else
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.ENDRUN)
                    If Not myGlobal.HasError Then useRequestFlag = False
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AbortSendEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If AnalyzerCurrentActionAttribute <> AnalyzerManagerAx00Actions.ABORT_START AndAlso Not abortAlreadySentFlagAttribute Then
                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    If Not myInstructionsQueue.Contains(pAction) Then
                        myInstructionsQueue.Add(pAction)
                        myParamsQueue.Add(pSwAdditionalParameters)
                    End If
                Else
                    SetAllowScanInRunningValue(False) 'AllowScanInRunningAttribute = False

                    RemoveItemFromQueue(AnalyzerManagerSwActionList.ENDRUN)

                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.ABORT)
                    If Not myGlobal.HasError Then useRequestFlag = False 'AG 02/11/2010
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SoundSendEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            'AG 26/01/2012 - In Running the SOUND instruction is sent only after receive a STATUS instruction
            If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING AndAlso Not AnalyzerIsRingingAttribute Then
                If Not myInstructionsQueue.Contains(pAction) Then
                    myInstructionsQueue.Add(pAction)
                    myParamsQueue.Add(pSwAdditionalParameters)
                End If
            Else
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.SOUND)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function EndSoundSendEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            'AG 26/01/2012 - In Running the SOUND instruction is sent only after receive a STATUS instruction
            'AG 03/09/2012 - during conection (running) force sent always endsound, do not use queue
            If (AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING AndAlso AnalyzerIsRingingAttribute) _
               AndAlso mySessionFlags(AnalyzerManagerFlags.CONNECTprocess.ToString) <> "INPROCESS" Then
                If Not myInstructionsQueue.Contains(pAction) Then
                    myInstructionsQueue.Add(pAction)
                    myParamsQueue.Add(pSwAdditionalParameters)
                End If
            Else
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.ENDSOUND)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function NextPreparationSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If IsNumeric(pSwAdditionalParameters) Then
                Dim nextWell = Integer.Parse(pSwAdditionalParameters.ToString)

                'The pauseSendingTestPreparationsFlag becomes TRUE with several alarms appears and becomes FALSE when solved
                'AG 29/06/2012 - Add 'AndAlso Not endRunAlreadySentFlagAttribute AndAlso Not abortAlreadySentFlagAttribute'
                ' XB 15/10/2013 - Add PauseAlreadySentFlagAttribute - BT #1318
                If Not pauseSendingTestPreparationsFlag AndAlso _
                   Not endRunAlreadySentFlagAttribute AndAlso _
                   Not abortAlreadySentFlagAttribute AndAlso _
                   Not PauseAlreadySentFlagAttribute Then
                    myGlobal = ManageSendAndSearchNext(nextWell)
                End If

            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <param name="pServiceParams"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AdjustLightSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object, ByVal pServiceParams As List(Of String)) As GlobalDataTO

            If ConnectedAttribute Then
                If IsNumeric(pSwAdditionalParameters) Then
                    Dim nextWell = Integer.Parse(pSwAdditionalParameters.ToString)
                    myGlobal = SendAdjustLightInstruction(nextWell)
                ElseIf Not pServiceParams Is Nothing Then
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.ALIGHT, Nothing, "", "", pServiceParams)
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AdjustFLightSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                If Not pSwAdditionalParameters Is Nothing Then
                    Dim myParams = DirectCast(pSwAdditionalParameters, List(Of String))
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.FLIGHT, myParams, String.Empty, String.Empty, Nothing)
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InfoSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim queryMode As Integer = Ax00InfoInstructionModes.ALR
                If IsNumeric(pSwAdditionalParameters) Then
                    queryMode = CInt(pSwAdditionalParameters)
                End If

                Dim allowToSendFlag As Boolean = True

                'AG 21/03/2012 - the following code try to optimize the INFO str or stp sending instructions depending
                'if sw has activated or deactivated it ... problem new Fw versions deactivates ansinf automatically so
                'this code not works ok always
                'If AnalyzerIsInfoActivatedAttribute = 1 AndAlso queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STR Then allowToSendFlag = False
                'If AnalyzerIsInfoActivatedAttribute = 0 AndAlso queryMode = GlobalEnumerates.Ax00InfoInstructionModes.STP Then allowToSendFlag = False

                'SGM 24/10/2012 - Not to allow send INFO Q:3 in case of Alarm details requested
                If GlobalBase.IsServiceAssembly Then

                    Dim myInstructionsQueueTemp As New List(Of AnalyzerManagerSwActionList)
                    Dim myParamsQueueTemp As New List(Of Object)

                    For Each a As AnalyzerManagerSwActionList In myInstructionsQueue
                        Dim j As Integer = myInstructionsQueue.IndexOf(a)
                        myInstructionsQueueTemp.Add(a)
                        myParamsQueueTemp.Add(myParamsQueue(j))
                    Next
                    For Each a As AnalyzerManagerSwActionList In myInstructionsQueueTemp
                        If a = AnalyzerManagerSwActionList.INFO Then
                            Dim myInfoType As Ax00InfoInstructionModes = CType(queryMode, Ax00InfoInstructionModes)
                            Dim j As Integer = myInstructionsQueueTemp.IndexOf(a)
                            If CType(myParamsQueueTemp(j), Ax00InfoInstructionModes) = myInfoType Then
                                myInstructionsQueue.RemoveAt(j)
                                myParamsQueue.RemoveAt(j)
                            End If
                        End If
                    Next

                    If queryMode = Ax00InfoInstructionModes.STR Or queryMode = Ax00InfoInstructionModes.STP Then
                        allowToSendFlag = Not IsAlarmInfoRequested
                        If Not allowToSendFlag Then
                            myInstructionsQueue.Add(AnalyzerManagerSwActionList.INFO)
                            myParamsQueue.Add(CInt(pSwAdditionalParameters))
                        End If
                    End If
                End If

                If allowToSendFlag Then

                    ' SGM 06/11/2012 - this instruction don't expect answer
                    If GlobalBase.IsServiceAssembly Then
                        If queryMode = Ax00InfoInstructionModes.STP Then
                            ClearQueueToSend()
                        End If
                    End If

                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.INFO, CInt(queryMode))

                    If Not myGlobal.HasError Then

                        If queryMode = Ax00InfoInstructionModes.STR Then
                            AnalyzerIsInfoActivatedAttribute = 1
                            IsAlarmInfoRequested = False
                        End If

                        If queryMode = Ax00InfoInstructionModes.STP Then
                            AnalyzerIsInfoActivatedAttribute = 0
                            IsAlarmInfoRequested = False
                        End If

                        If queryMode = Ax00InfoInstructionModes.ALR Then
                            AnalyzerIsInfoActivatedAttribute = 0
                            If GlobalBase.IsServiceAssembly Then
                                IsAlarmInfoRequested = True
                            End If
                        End If

                    Else
                        IsAlarmInfoRequested = False
                    End If

                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function WashStationControlSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim queryMode As Integer = Ax00WashStationControlModes.UP
                If IsNumeric(pSwAdditionalParameters) Then
                    queryMode = CInt(pSwAdditionalParameters)
                End If
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.WSCTRL, queryMode)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function BarCodeSendEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                'AG 16/10/2013 - In Running add the barcoderequest and the BarcodeRequestDS into queue, the instruction will be sent after STATUS reception

                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    If (AnalyzerCurrentActionAttribute <> AnalyzerManagerAx00Actions.ABORT_START AndAlso Not abortAlreadySentFlagAttribute) _
                       AndAlso (AllowScanInRunning) Then
                        If Not myInstructionsQueue.Contains(pAction) Then
                            myInstructionsQueue.Add(pAction)
                            myParamsQueue.Add(pSwAdditionalParameters)

                            ' XB 29/01/2014 - Deactivate WatchDog timer - Task #1438
                            RaiseEvent WatchDogEvent(False)
                        End If
                    End If
                Else
                    myBarcodeRequestDS = CType(pSwAdditionalParameters, AnalyzerManagerDS)
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.BARCODE_REQUEST, myBarcodeRequestDS)

                    BarcodeStartInstrExpected = True
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ConfigSendEvent(ByVal myGlobal As GlobalDataTO) As GlobalDataTO

            If ConnectedAttribute Then
                Dim anSetts As New AnalyzerSettingsDelegate
                myGlobal = anSetts.ReadAll(Nothing, AnalyzerIDAttribute)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    Dim mySettingsDs = CType(myGlobal.SetDatos, AnalyzerSettingsDS)
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.CONFIG, mySettingsDs)
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ReadAdjSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim queryMode As String = ""
                If Not pSwAdditionalParameters Is Nothing Then
                    queryMode = pSwAdditionalParameters.ToString
                End If
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.READADJ, queryMode)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <param name="pFwScriptId"></param>
        ''' <param name="pServiceParams"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IseCmdSendEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object, ByVal pFwScriptId As String, ByVal pServiceParams As List(Of String)) As GlobalDataTO

            If ConnectedAttribute Then
                Dim myIseCommand As ISECommandTO
                myIseCommand = CType(pSwAdditionalParameters, ISECommandTO)
                If myIseCommand.ISEMode <> ISEModes.None Then
                    ' XB 03/04/2014 - Avoid send ISECMD out of running cycle - task #1573
                    If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                        If Not myInstructionsQueue.Contains(pAction) Then
                            myInstructionsQueue.Add(pAction)
                            myParamsQueue.Add(pSwAdditionalParameters)
                            Debug.Print("ISECMD TO THE QUEUE !!! [" & pAction.ToString & "] [" & pSwAdditionalParameters.ToString & "]")
                        End If
                    Else
                        timeISEOffsetFirstTime = False
                        If Not sendingRepetitions Then
                            numRepetitionsTimeout = 0
                        End If
                        InitializeTimerStartTaskControl(WAITING_TIME_FAST, True)
                        StoreStartTaskinQueue(pAction, pSwAdditionalParameters, pFwScriptId, pServiceParams)

                        myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.ISE_CMD, myIseCommand)

                    End If

                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function FwUtilSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim myFwAction As FWUpdateRequestTO
                myFwAction = CType(pSwAdditionalParameters, FWUpdateRequestTO)
                If myFwAction.ActionType <> FwUpdateActions.None Then
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.FW_UTIL, myFwAction)
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RecoverSendEvent() As GlobalDataTO
            Dim myGlobal As GlobalDataTO

            If GlobalBase.IsServiceAssembly Then
                SensorValuesAttribute.Clear()
            End If
            ClearQueueToSend() 'Before sent recover instruction clear the instructions in queue to be sent
            myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.RECOVER)
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function PollRdSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim actionMode As Integer = Ax00PollRDAction.Biochemical
                If IsNumeric(pSwAdditionalParameters) Then
                    actionMode = CInt(pSwAdditionalParameters)
                End If
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.POLLRD, actionMode)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <param name="pFwScriptId"></param>
        ''' <param name="pServiceParams"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CommandSendEvent(ByVal pAction As AnalyzerManagerSwActionList, ByVal pSwAdditionalParameters As Object, ByVal pFwScriptId As String, ByVal pServiceParams As List(Of String)) As GlobalDataTO
            Dim myGlobal As GlobalDataTO

            myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.COMMAND, pSwAdditionalParameters, "", pFwScriptId, pServiceParams)

            If GlobalBase.IsServiceAssembly Then
                If Not myGlobal.HasError Then
                    If Not sendingRepetitions Then
                        numRepetitionsTimeout = 0
                    End If
                    InitializeTimerStartTaskControl(WAITING_TIME_DEFAULT)
                    StoreStartTaskinQueue(pAction, pSwAdditionalParameters, pFwScriptId, pServiceParams)
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function LoadAdjSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim queryMode As String = ""
                If Not pSwAdditionalParameters Is Nothing Then
                    queryMode = pSwAdditionalParameters.ToString
                    'queryMode = "{Brazo Muestra}" & vbCrLf & _
                    '"M1PI:4400;   {REF Punto Inicial Detección de nivel - Vertical}" & vbCrLf & _
                    '"M1RPI:4000;  {REF Punto Inicial Detección de nivel - Vertical - En Rotor de Reacciones}" '& vbCrLf & _

                    '"CLOT:0;      {Detección de clot desactivada}" & vbCrLf & _
                    '"M1DH1:16635; {Dispensación en Rotor Posicion 1 - Horizontal}" & vbCrLf & _
                    '"M1DH2:16188; {Dispensación en Rotor Posicion 2 - Horizontal}" & vbCrLf & _
                    '"M1DV1:7474;  {REF Dispensación en Rotor Posicion 1 - Vertical}" & vbCrLf & _
                    '"M1DV2:8050;  {REF Dispensación en Rotor Posicion 2 - Vertical}" & vbCrLf & _
                    '"M1H:1763;    {Posición Parking - Horizontal}" & vbCrLf & _
                    '"M1ISEH:-60;  {Dispensación en ISE - Horizontal}" & vbCrLf & _
                    '"M1ISEV:16000;{Dispensación en ISE - Vertical}" & vbCrLf & _
                    '"M1PH1:10632; {Rotor de Muestras Corona 1 - Horizontal}" & vbCrLf & _
                    '"M1PH2:9970;  {Rotor de Muestras Corona 2 - Horizontal}" & vbCrLf & _
                    '"M1PH3:8710;  {Rotor de Muestras Corona 3 - Horizontal}" & vbCrLf & _
                    '"M1PI:4400;   {REF Punto Inicial Detección de nivel - Vertical}" & vbCrLf & _
                    '"M1PV1p:16300;{Rotor de Muestras Corona 1 Punto máximo detección pediátrico - Vertical}" & vbCrLf & _
                    '"M1PV1t:16400;{Rotor de Muestras Corona 1 Punto máximo detección tubo - Vertical}" & vbCrLf & _
                    '"M1PV2p:16200;{Rotor de Muestras Corona 2 Punto máximo detección pediátrico - Vertical}" & vbCrLf & _
                    '"M1PV2t:16400;{Rotor de Muestras Corona 2 Punto máximo detección tubo - Vertical}" & vbCrLf & _
                    '"M1PV3p:16200;{Rotor de Muestras Corona 3 Punto máximo detección pediátrico - Vertical}" & vbCrLf & _
                    '"M1PV3t:16400;{Rotor de Muestras Corona 3 Punto máximo detección tubo - Vertical}" & vbCrLf & _
                    '"M1RPI:4000;  {REF Punto Inicial Detección de nivel - Vertical - En Rotor de Reacciones}" & vbCrLf & _
                    '"M1RV:200;    {Posicion referencia - Vertical}" & vbCrLf & _
                    '"M1SV:580;    {Posición Segura para movimiento Horizontal - Vertical}" & vbCrLf & _
                    '"M1V:16332;   {REF Posición Parking - Vertical}" & vbCrLf & _
                    '"M1WH:12620;  {Estación de Lavado - Horizontal}" & vbCrLf & _
                    '"M1WVR:4250;  {Estación de Lavado - Vertical Relativo a R1SV}"

                End If
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.LOADADJ, queryMode)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function TanksTestSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO
            If ConnectedAttribute Then
                Dim queryMode As String = ""
                If Not pSwAdditionalParameters Is Nothing Then
                    queryMode = pSwAdditionalParameters.ToString
                End If
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.TANKSTEST, queryMode)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function PollFwSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim queryMode As POLL_IDs
                If Not pSwAdditionalParameters Is Nothing Then
                    queryMode = CType(pSwAdditionalParameters, POLL_IDs)
                End If
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.POLLFW, queryMode)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function PollHwSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim queryMode As POLL_IDs
                If Not pSwAdditionalParameters Is Nothing Then
                    queryMode = CType(pSwAdditionalParameters, POLL_IDs)
                End If
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.POLLHW, queryMode)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function UpdateFwSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO
            If ConnectedAttribute Then
                Dim queryMode() As Byte = Nothing
                If Not pSwAdditionalParameters Is Nothing Then
                    queryMode = CType(pSwAdditionalParameters, Byte())
                End If
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.UPDATE_FIRMWARE, queryMode)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ReadCyclesSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim queryMode As String = ""
                If Not pSwAdditionalParameters Is Nothing Then
                    queryMode = pSwAdditionalParameters.ToString
                End If
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.READCYC, queryMode)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function WriteCyclesSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim queryMode As String = ""
                If Not pSwAdditionalParameters Is Nothing Then
                    queryMode = pSwAdditionalParameters.ToString
                End If
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.WRITECYC, queryMode)
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <param name="pSwAdditionalParameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function UtilSendEvent(ByVal myGlobal As GlobalDataTO, ByVal pSwAdditionalParameters As Object) As GlobalDataTO

            If ConnectedAttribute Then
                Dim myUtilCommand As UTILCommandTO
                myUtilCommand = CType(pSwAdditionalParameters, UTILCommandTO)
                If myUtilCommand.ActionType <> UTILInstructionTypes.None Then
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.UTIL, myUtilCommand)
                End If
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function PollSnSendEvent(ByVal myGlobal As GlobalDataTO) As GlobalDataTO

            If ConnectedAttribute Then
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.POLLSN)
                runningConnectionPollSnSent = True
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function WaitingTimeExpiredSendEvent(ByVal myGlobal As GlobalDataTO) As GlobalDataTO

            If myStartTaskInstructionsQueue.Contains(AnalyzerManagerSwActionList.ISE_CMD) Then
                ISECMDLost = True
                GlobalBase.CreateLogActivity("ISE CMD lost !!! [WAITING_TIME_EXPIRED] ... sending STATE ...", "AnalyzerManager.ManagerAnalyzer", EventLogEntryType.Error, False)
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.STATE)

            ElseIf myStartTaskInstructionsQueue.Contains(AnalyzerManagerSwActionList.RUNNING) Then
                Debug.Print("RUNNING lost !!! [WAITING_TIME_EXPIRED] ... sending STATE ...")
                RUNNINGLost = True
                GlobalBase.CreateLogActivity("RUNNING lost !!! [WAITING_TIME_EXPIRED] ... sending STATE ...", "AnalyzerManager.ManagerAnalyzer", EventLogEntryType.Error, False)
                myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.STATE)

            Else
                RaiseEvent SendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)

                If ConnectedAttribute Then
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.STATE)
                End If

            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="myGlobal"></param>
        ''' <remarks></remarks>
        Private Function StartTaskTimeoutSendEvent(ByRef myGlobal As GlobalDataTO) As Boolean

            If myStartTaskInstructionsQueue.Contains(AnalyzerManagerSwActionList.ISE_CMD) Then
                ' ISE COMMAND Instructions
                ISECMDLost = True
                GlobalBase.CreateLogActivity("ISE CMD lost !!! [START_TASK_TIMEOUT] ... sending STATE ...", "AnalyzerManager.ManagerAnalyzer", EventLogEntryType.Error, False)

                If numRepetitionsSTATE > GlobalBase.MaxRepetitionsTimeout Then
                    GlobalBase.CreateLogActivity("Num of Repetitions for STATE timeout excedeed !!!", "AnalyzerManager.ManagerAnalyzer", EventLogEntryType.Error, False)

                    ' Activates Alarm begin
                    Dim myAlarmList As New List(Of Alarms)
                    Dim myAlarmStatusList As New List(Of Boolean)

                    Const alarmId As Alarms = AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR
                    Const alarmStatus As Boolean = True
                    ISEAnalyzer.IsTimeOut = True

                    PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)
                    If myAlarmList.Count > 0 Then
                        ' Note that this alarm is common on User and Service !
                        Dim currentAlarms = New CurrentAlarms(Me)
                        myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                    End If
                    ' Activates Alarm end

                    RaiseEvent SendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                Else
                    ' Instruction has not started by Fw, so is need to send it again
                    GlobalBase.CreateLogActivity("Repeat STATE Instruction [" & numRepetitionsSTATE.ToString & "]", "AnalyzerManager.ManagerAnalyzer", EventLogEntryType.Error, False)
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.STATE)
                    InitializeTimerSTATEControl(WAITING_TIME_FAST)
                End If


            ElseIf myStartTaskInstructionsQueue.Contains(AnalyzerManagerSwActionList.RUNNING) Then
                ' RUNNING Instruction
                RUNNINGLost = True
                GlobalBase.CreateLogActivity("RUNNING lost !!! [START_TASK_TIMEOUT] ... sending STATE ...", "AnalyzerManager.ManagerAnalyzer", EventLogEntryType.Error, False)

                If numRepetitionsSTATE > GlobalBase.MaxRepetitionsTimeout Then
                    GlobalBase.CreateLogActivity("Num of Repetitions for RUNNING timeout excedeed !!!", "AnalyzerManager.ManagerAnalyzer", EventLogEntryType.Error, False)

                    ' Activates Alarm begin
                    Dim myAlarmList As New List(Of Alarms)
                    Dim myAlarmStatusList As New List(Of Boolean)

                    Const alarmId As Alarms = AlarmEnumerates.Alarms.COMMS_TIMEOUT_ERR
                    Const alarmStatus As Boolean = True
                    ISEAnalyzer.IsTimeOut = True

                    PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)
                    If myAlarmList.Count > 0 Then
                        Dim currentAlarms = New CurrentAlarms(Me)
                        myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                    End If
                    ' Activates Alarm end

                    Dim myAnalyzerFlagsDs As New AnalyzerManagerFlagsDS
                    UpdateSessionFlags(myAnalyzerFlagsDs, AnalyzerManagerFlags.RUNNINGprocess, "CLOSED")

                    'Update internal flags. Basically used by the running normal business
                    If (Not myGlobal.HasError AndAlso ConnectedAttribute) Then
                        'Update analyzer session flags into DataBase
                        If (myAnalyzerFlagsDs.tcfgAnalyzerManagerFlags.Rows.Count > 0) Then
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDs)
                        End If
                    End If

                    RaiseEvent SendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                Else
                    ' Instruction has not started by Fw, so is need to send it again
                    GlobalBase.CreateLogActivity("Repeat STATE Instruction [" & numRepetitionsSTATE.ToString & "]", "AnalyzerManager.ManagerAnalyzer", EventLogEntryType.Error, False)
                    myGlobal = AppLayer.ActivateProtocol(AppLayerEventList.STATE)
                    InitializeTimerSTATEControl(WAITING_TIME_FAST)
                End If

            Else
                ' Another Instructions

                If GlobalBase.IsServiceAssembly Then
                    sendingRepetitions = True
                    ClearQueueToSend()
                    numRepetitionsTimeout += 1
                    If numRepetitionsTimeout > GlobalBase.MaxRepetitionsTimeout Then
                        waitingStartTaskTimer.Enabled = False
                        sendingRepetitions = False

                        ConnectedAttribute = False
                        InfoRefreshFirstTime = True
                        UpdateSensorValuesAttribute(AnalyzerSensors.CONNECTED, CSng(ConnectedAttribute), True)

                        RaiseEvent SendEvent(AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString)
                        Return False

                    Else
                        ' Instruction has not started by Fw, so is need to send it again
                        GlobalBase.CreateLogActivity("Repeat Start Task Instruction [" & numRepetitionsTimeout.ToString & "]", "AnalyzerManager.ManageAnalyzer", EventLogEntryType.Error, False)
                        myGlobal = SendStartTaskinQueue()
                    End If
                End If

            End If
            Return True
        End Function
    End Class
End Namespace

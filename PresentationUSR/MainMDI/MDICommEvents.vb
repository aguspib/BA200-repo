Option Strict On
Option Explicit On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.BL.UpdateVersion
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Controls.UserControls
Imports System.Globalization
Imports LIS.Biosystems.Ax00.LISCommunications 'AG 25/02/2013 - for LIS communications
Imports System.Xml 'AG 25/02/2013 - for LIS communications
Imports System.Threading 'AG 25/02/2013 - for LIS communications (release MDILISManager object in MDI closing event)
Imports System.Timers
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.App


'Refactoring code in CommEvents partial class inherits form MDI (specially method ManageReceptionEvent)
Partial Public Class IAx00MainMDI

#Region "Communications events Main Methods"

    ''' <summary>
    ''' Executes ManageReceptionEvent() method in the main thread
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 27/05/2011
    ''' AG 25/02/2013 - declare as private instead of public
    ''' </remarks>
    Private Sub OnManageReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                      ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) Handles MDIAnalyzerManager.ReceptionEvent

        Me.UIThread(Function() ManageReceptionEvent(pInstructionReceived, pTreated, pRefreshEvent, pRefreshDS, pMainThread))

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pInstructionReceived"></param>
    ''' <remarks>
    ''' AG 02/11/2010
    ''' Modified by: RH - 27/05/2011
    ''' modified by: DL - 16/09/2011 Lock prefreshDS copy to send to different functions
    '''              AG - 25/02/2013 - declare as private instead of public
    '''              XB - 23/07/2013 - Analyzer starts ringing everytime the Start Session process pauses and requires an user action. Stops ringing after the user chooses option
    '''              AG - 28/11/2013 - BT #1397 changes for recovery results if re-connection in running pause mode
    '''              XB - 16/12/2013 - Add ISE exception error cases management (like timeout, E:61) - Task #1441
    '''              AG - 07/01/2014 - refactoring code in CommEvents partial class inherits form
    '''              XB - 06/02/2014 - Improve WDOG BARCODE_SCAN - Task #1438
    '''              XB - 28/04/2014 - Improve Initial Purges sends before StartWS - Task #1587
    '''              XB - 23/05/2014 - Do not shows ISE warnings if there are no ISE preparations into the WS - task #1638
    '''              XB - 20/06/2014 - improve the ISE Timeouts control (E:61) - Task #1441
    '''              IT - 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Function ManageReceptionEvent(ByVal pInstructionReceived As String, _
                                         ByVal pTreated As Boolean, _
                                         ByVal pRefreshEvent As List(Of GlobalEnumerates.UI_RefreshEvents), _
                                         ByVal pRefreshDS As UIRefreshDS, ByVal pMainThread As Boolean) As Boolean
        Dim myLogAcciones As New ApplicationLogManager()
        'RH Save UIRefreshDS for recording generated UIRefreshDS data and reproduce bugs in Debug sessions
        'pRefreshDS.WriteXml(String.Format("UIRefreshDS{0}.xml", DSNumber))

        'RH Load UIRefreshDS for simulating generated UIRefreshDS data and reproduce bugs in Debug sessions
        'pRefreshDS.ReadXml(String.Format("UIRefreshDS{0}.xml", DSNumber))

        'DSNumber += 1

        ' XB 20/06/2014 - #1441
        Dim ISENotReady As Boolean = False

        Try
            'Dim StartTime As DateTime = Now 'AG 12/06/2012 - time estimation

            'DL 16/09/2011
            'SyncLock ensures that multiple threads do not execute the statement block at the same time. 
            'SyncLock prevents each thread from entering the block until no other thread is executing it.
            'Protect pRefreshDS data from being updated by more than one thread simultaneously. 
            'If the statements that manipulate the data must go to completion without interruption, put them inside a SyncLock block.
            Dim copyRefreshDS As UIRefreshDS = Nothing
            Dim copyRefreshEventList As New List(Of GlobalEnumerates.UI_RefreshEvents)

            ''TR 18/09/2012 -Log to trace incase Exception error is race.
            'myLogAcciones.CreateLogActivity("START BLOCK 0.", Me.Name & ".ManageReceptionEvent ", _
            '                                EventLogEntryType.FailureAudit, False)

            ' XB 30/08/2013
            If AnalyzerController.Instance.Analyzer.InstructionTypeReceived = AnalyzerManagerSwActionList.ANSFCP_RECEIVED Then
                myLogAcciones.CreateLogActivity("START ANSFCP received with pTreated value as [" & pTreated & "] ", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False)
            End If

            'AG 04/04/2012 - This code is needed because the incomplete samples screen is open using ShowDialog
            'Check if the refresh event involves open the incomple samples screen
            Dim barcode_Samples_Warnings As Boolean = False
            If pRefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                Dim lnqRes As List(Of UIRefreshDS.SensorValueChangedRow)
                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In pRefreshDS.SensorValueChanged _
                          Where String.Equals(a.SensorID, GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS.ToString) _
                          Select a).ToList

                If lnqRes.Count > 0 Then
                    If lnqRes(0).Value = 1 Then 'Samples barcode warnings
                        barcode_Samples_Warnings = True
                        incompleteSamplesOpenFlag = True
                    ElseIf lnqRes(0).Value = 2 Then 'No pending executions to be sent
                        processingBeforeRunning = "2" 'Finished Nok (no executions pending to be sent)
                        ScreenWorkingProcess = False
                    End If

                    'Once treated reset flag (This flag will be removed after treat flags barcode_Samples_Warnings or incompleteSamplesOpenFlag 
                    'or Warning message caused by value 2 in method ShowAlarmsOrSensorsWarningMessages)
                    'AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS) = 0
                End If
            End If
            'AG 04/04/2012

            SyncLock lockThis
                copyRefreshDS = CType(pRefreshDS.Copy(), UIRefreshDS)
                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("BEFORE pRefreshEvent FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)
                For Each item As GlobalEnumerates.UI_RefreshEvents In pRefreshEvent
                    'copyRefreshEventList.Add(item)
                    If item = UI_RefreshEvents.BARCODE_POSITION_READ AndAlso incompleteSamplesOpenFlag Then
                        'In this case do not add item
                    Else
                        copyRefreshEventList.Add(item)
                    End If
                Next
                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("AFTER pRefreshEvent FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)

                If pRefreshEvent.Count > 0 Then AnalyzerController.Instance.Analyzer.ReadyToClearUIRefreshDS(pMainThread) 'Inform the ui refresh dataset can be cleared so they are already copied
            End SyncLock
            'END DL 16/09/2011

            ''TR 18/09/2012 -Log to trace incase Exception error is race.
            'myLogAcciones.CreateLogActivity("END BLOCK 0.", Me.Name & ".ManageReceptionEvent ", _
            '                                EventLogEntryType.FailureAudit, False)

            If pTreated Then '(1) 
                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("START BLOCK 1.", Me.Name & ".ManageReceptionEvent ", _
                '                                EventLogEntryType.FailureAudit, False)
                '////////////////////////////
                'REFRESH MDI BUTTONS & STATUS
                '////////////////////////////

                'Refresh button bar the Ax00 and the connection Status 

                'AG 27/02/2012 - previous code generate uirefresh event every instruction received. New code 24/02/2012 generate uirefresh events
                'in running only when receive the ansphr readings so add the 2on condition part
                'If AnalyzerController.Instance.Analyzer.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED Then '(2)
                If AnalyzerController.Instance.Analyzer.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED OrElse _
                   AnalyzerController.Instance.Analyzer.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.ANSERR_RECEIVED OrElse _
                   (AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.RUNNING AndAlso pMainThread) Then '(2)
                    SetActionButtonsEnableProperty(True) 'AG 27/02/2012

                    ShowLabelInStatusBar() 'AG 23/10/2013 - move next code into this method

                    ' XB 15/01/2014 - Set Interval WatchDog for Barcode Actions with the value received from the Instrument - Task #1438
                    If AnalyzerController.Instance.Analyzer.AnalyzerCurrentAction = AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED AndAlso _
                       AnalyzerController.Instance.Analyzer.BarcodeStartInstrExpected Then
                        AnalyzerController.Instance.Analyzer.BarcodeStartInstrExpected = False

                        Dim NewIntervalValue As Double = (AnalyzerController.Instance.Analyzer.MaxWaitTime) * 1000
                        Debug.Print("******************************* BARCODE_ACTION_RECEIVED !!!")
                        watchDogTimer.Interval = NewIntervalValue
                        Debug.Print("******************************* WATCHDOG INTERVAL CHANGED TO [" & NewIntervalValue.ToString & "]")

                        Dim MdiChildIsDisabled As Boolean = False
                        Dim refreshTriggeredFlag As Boolean = False
                        If DisabledMDIChildAttribute.Count > 0 Then
                            MdiChildIsDisabled = True
                        End If

                        Dim myCurrentMDIForm As Form = Nothing
                        If Not ActiveMdiChild Is Nothing Then
                            myCurrentMDIForm = ActiveMdiChild
                        ElseIf MdiChildIsDisabled Then
                            myCurrentMDIForm = DisabledMDIChildAttribute(0)
                        End If

                        If Not myCurrentMDIForm Is Nothing AndAlso TypeOf myCurrentMDIForm Is IWSRotorPositions Then
                            Dim CurrentMdiChild As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                            CurrentMdiChild.RefreshwatchDogTimer_Interval(NewIntervalValue)
                        End If
                    End If
                    ' XB 15/01/2014


                    'AG 22/03/2012 - exception case: when connection is performed in StandBy keep the status text "Connection Process"
                    'until the ISE module is initialized (use the ANSISE instruction)

                    ' XBC 03/10/2012 - Correction : some case didn't actives the menu
                    ' ElseIf AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY AndAlso _
                    ' AnalyzerController.Instance.Analyzer.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED Then


                ElseIf AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY AndAlso _
                       AnalyzerController.Instance.Analyzer.AdjustmentsRead Then
                    ' XBC 03/10/2012

                    '(ISE instaled and (initiated or not SwitchedON)) Or not instaled
                    If AnalyzerController.Instance.Analyzer.ISEAnalyzer IsNot Nothing AndAlso _
                       AnalyzerController.Instance.Analyzer.ISEAnalyzer.ConnectionTasksCanContinue Then

                        If AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS" OrElse AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "PAUSED" Then
                            ShowStatus(Messages.STARTING_INSTRUMENT)
                        Else
                            'SGM 10/04/2012 not to show Standby in case of ISE utilities open
                            If Not ActiveMdiChild Is Nothing AndAlso TypeOf ActiveMdiChild Is IISEUtilities Then
                            Else
                                If Not ProcessingBusinessInCourse() AndAlso Not Me.ShutDownisPending AndAlso Not Me.StartSessionisPending Then ' AG 08/11/2012 - PENDING Validate - Execute only when the working process progress bar is activate
                                    'If Not Me.ShutDownisPending And Not Me.StartSessionisPending Then ' XBC 23/07/2012
                                    ShowStatus(Messages.STANDBY)

                                    ' XB 06/11/2013 - Force Refresh screen just the first time when Analyzer is connected on StandBy
                                    If String.Compare(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "CLOSED", False) = 0 Then
                                        If ForceRefreshOnFirstStandBy Then
                                            ForceRefreshOnFirstStandBy = False
                                            EnableButtonAndMenus(True, True)
                                        End If
                                    End If


                                    ' AG 07/11/2012 - Avoid blinking every 2 seconds when analyzer is connected and we are in StandBy
                                    '                 Do not call this method because is also called in EnableButtonAndMenus
                                    'SetActionButtonsEnableProperty(True) 'Call to activate vertical action buttons
                                    EnableButtonAndMenus(True) 'TR 20/09/2012 -uncommented by TR.  ('SA 07/09/2012 commented by SA.)
                                End If
                            End If
                        End If
                    End If

                End If '(2)

                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("END BLOCK 1.", Me.Name & ".ManageReceptionEvent ", _
                '                                EventLogEntryType.FailureAudit, False)

                ChangeErrorStatusLabel(False) 'AG 18/05/2012 - When some process is paused by some error set the error providers text to 'Not Finished'

                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("START BLOCK 2.", Me.Name & ".ManageReceptionEvent ", _
                '                                EventLogEntryType.FailureAudit, False)
                '////////////////
                ' REFRESH SCREENS
                '////////////////

                Dim monitorTreated As Boolean = False
                Dim resultTreated As Boolean = False
                Dim wsRotorPositionTreated As Boolean = False
                Dim changeRotorTreated As Boolean = False
                Dim conditioningTreated As Boolean = False
                Dim configSettingsLISTreated As Boolean = False    ' XB 24/04/2013
                'Refreh UI screens depending the current screen and the pRefreshEvent received
                'use the method RefreshScreen

                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS) Then
                    PerformNewExecutionStatusDone(copyRefreshEventList, copyRefreshDS, monitorTreated)
                End If

                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED) Then
                    PerformNewCalculationsDone(copyRefreshEventList, copyRefreshDS, monitorTreated, resultTreated)
                End If

                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.READINGS_RECEIVED) Then
                    PerformNewReadingsReception(copyRefreshEventList, copyRefreshDS)
                End If


                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED) Then

                    ' XB 20/06/2014 - #1441
                    If AnalyzerController.Instance.Analyzer.InstructionTypeReceived = GlobalEnumerates.AnalyzerManagerSwActionList.STATUS_RECEIVED Then
                        Dim sensorValue As Single = 0
                        sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_SWITCHON_CHANGED)
                        If sensorValue = 1 Then
                            ISENotReady = True
                        End If
                    End If
                    ' XB 20/06/2014 - #1441

                    PerformNewAlarmsReception(copyRefreshEventList, copyRefreshDS, monitorTreated, changeRotorTreated, conditioningTreated)
                End If

                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then


                    PerformNewSensorValueChanged(copyRefreshEventList, copyRefreshDS, monitorTreated, changeRotorTreated, conditioningTreated, wsRotorPositionTreated, configSettingsLISTreated)
                End If

                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.REACTIONS_WELL_STATUS_CHANGED) Then
                    PerformNewWashingStationPositionDone(copyRefreshEventList, copyRefreshDS, monitorTreated)
                End If


                'If pRefreshEvent.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED) Then
                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED) OrElse _
                    copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.BARCODE_POSITION_READ) Then
                    PerformNewWRotorPositionChange(copyRefreshEventList, copyRefreshDS, monitorTreated, wsRotorPositionTreated)
                End If
                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("END BLOCK 2.", Me.Name & ".ManageReceptionEvent ", _
                '                                EventLogEntryType.FailureAudit, False)

                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("START BLOCK 3.", Me.Name & ".ManageReceptionEvent ", _
                '                                EventLogEntryType.FailureAudit, False)

                'LAST EVENT REFRESH TREATMENT DUE some PopUp screen can be opened
                '================================================================

                'AG 05/09/2011 - Finally open popup screen  when barcode warnings
                'When sensor BARCODE_WARNINGS = 1 then screens Monitor, WSRotorPositions & WSPreparation (open the auxiliary screen IWSIncompleteSamplesAuxScreen)
                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) AndAlso _
                   barcode_Samples_Warnings Then 'AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS) = 1 Then
                    PerformNewBarcodeWarnings(barcode_Samples_Warnings)
                End If

                ' XB 20/06/2014 - #1441
                'SGM 12/03/2012
                ' XB 24/10/2013 - Specific ISE commands are allowed in RUNNING (pause mode) - BT #1343
                'If AnalyzerController.Instance.Analyzer.InstructionTypeReceived = AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED Then
                If (AnalyzerController.Instance.Analyzer.InstructionTypeReceived = AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED) Or _
                   (pInstructionReceived.Contains("A400;ANSISE;")) Or _
                   (ISENotReady AndAlso (ShutDownisPending Or StartSessionisPending)) Then
                    ' XB 24/10/2013
                    ' XB 20/06/2014 - #1441
                    '
                    ' ISE RECEIVED
                    '

                    ' XB 03/12/2013 - protection : Ensure that connected Analyzer is fine registered on ISE table DB - task #1410
                    If CheckAnalyzerIDOnFirstConnectionForISE Then
                        If AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then
                            If String.Compare(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "CLOSED", False) = 0 Then
                                If Not AnalyzerController.Instance.Analyzer.ISEAnalyzer Is Nothing Then
                                    AnalyzerController.Instance.Analyzer.ISEAnalyzer.UpdateAnalyzerInformation(AnalyzerIDAttribute, AnalyzerModelAttribute)
                                    CheckAnalyzerIDOnFirstConnectionForISE = False
                                End If
                            End If
                        End If
                    End If

                    ' Refresh BarCode Info screen
                    If Not ActiveMdiChild Is Nothing Then

                        If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                            Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        ElseIf (TypeOf ActiveMdiChild Is IMonitor AndAlso Not monitorTreated) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                            CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                            monitorTreated = True
                        Else ' XBC 10/12/2012 - Correction : Allow ISE receptions also on another active screens, not only IMonitor and IISEUtilities
                            'ISE procedure finished
                            Dim sensorValue As Single = 0
                            sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED)
                            If sensorValue = 1 Then
                                ScreenWorkingProcess = False
                                AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED) = 0 'Once updated UI clear sensor
                                ISEProcedureFinished = True
                            End If
                        End If

                    Else

                        'if no screen is active
                        If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED) Then
                            Dim sensorValue As Single = 0


                            ''ISE switch on changed
                            'sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_SWITCHON_CHANGED)
                            'If sensorValue = 1 Then
                            '    ScreenWorkingProcess = False
                            '    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_SWITCHON_CHANGED) = 0 'Once updated UI clear sensor

                            'End If

                            ''ISE initiated
                            'sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED)
                            'If sensorValue >= 1 Then
                            '    ScreenWorkingProcess = False

                            '    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_CONNECTION_FINISHED) = 0 'Once updated UI clear sensor

                            'End If

                            ''ISE ready changed
                            'sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_READY_CHANGED)
                            'If sensorValue = 1 Then
                            '    ScreenWorkingProcess = False
                            '    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_READY_CHANGED) = 0 'Once updated UI clear sensor

                            'End If

                            'ANSISE received SGM 12/06/2012
                            sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISECMD_ANSWER_RECEIVED)
                            If sensorValue = 1 Then
                                ScreenWorkingProcess = False
                                AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISECMD_ANSWER_RECEIVED) = 0 'Once updated UI clear sensor
                            End If

                            'ISE procedure finished
                            sensorValue = AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED)
                            If sensorValue = 1 Then
                                ScreenWorkingProcess = False
                                AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_PROCEDURE_FINISHED) = 0 'Once updated UI clear sensor
                                ISEProcedureFinished = True ' XBC 27/09/2012 
                            End If


                        End If

                    End If


                    ' XBC 26/06/2012 - ISE final self-maintenance 

                    ' XBC 04/09/2012 - Correction : MDI send can't send next instruction until the last operation has been whole completed
                    'If Not AnalyzerController.Instance.Analyzer.ISEAnalyzer Is Nothing AndAlso AnalyzerController.Instance.Analyzer.ISEAnalyzer.CurrentProcedureIsFinished Then
                    'JBL 06/09/2012 - Correction: Undo XBC correction: Now is not needed
                    If Not AnalyzerController.Instance.Analyzer.ISEAnalyzer Is Nothing Then

                        ' XBC 27/09/2012 - Correction : This code just must be executed when the whole ISE procedure has finished
                        If ISEProcedureFinished Then
                            ISEProcedureFinished = False

                            If ShutDownisPending Then
                                ShutDownisPending = False
                                EnableButtonAndMenus(True)
                                SetEnableMainTab(True)
                                If AnalyzerController.Instance.Analyzer.ISEAnalyzer.LastProcedureResult = ISEAnalyzerEntity.ISEProcedureResult.OK Then
                                    ' Continues with Shut Down
                                    Me.NotDisplayShutDownConfirmMsg = True
                                    Me.bsTSShutdownButton.Enabled = True

                                    Me.bsTSShutdownButton.PerformClick()
                                Else
                                    'ActivateActionButtonBarOrSendNewAction()    ' XBC 10/12/2012 - Correction : activate again Shutdown button
                                    SetActionButtonsEnableProperty(True) 'AG 31/10/2013 - use this method instead of ActivateActionButtonBarOrSendNewAction (it has previous conditions to buttons activation)
                                    ShowMessage("Error", GlobalEnumerates.Messages.ISE_CLEAN_WRONG.ToString)
                                    Cursor = Cursors.Default
                                End If
                            ElseIf Me.StartSessionisPending Then
                                StopMarqueeProgressBar()
                                EnableButtonAndMenus(True)
                                SetEnableMainTab(True)
                                If AnalyzerController.Instance.Analyzer.ISEAnalyzer.LastProcedureResult = ISEAnalyzerEntity.ISEProcedureResult.OK Then
                                    ' Continues with Work Session
                                    Me.StartSession(True)

                                    ' XB 16/12/2013 - Manage Exceptions of ISE module - Task #1441
                                    'ElseIf Me.StartSessionisInitialPUGsent AndAlso _
                                    '       (Not Me.StartSessionisCALBsent And _
                                    '        Not Me.StartSessionisPMCLsent And _
                                    '        Not Me.StartSessionisBMCLsent) Then

                                    ' XB 28/04/2014 - Task #1587
                                    'ElseIf Me.StartSessionisInitialPUGsent AndAlso _
                                    '       (Not Me.StartSessionisCALBsent And Not Me.StartSessionisPMCLsent And Not Me.StartSessionisBMCLsent) AndAlso _
                                    '       AnalyzerController.Instance.Analyzer.ISEAnalyzer.LastProcedureResult <> ISEManager.ISEProcedureResult.Exception Then

                                ElseIf (Me.StartSessionisInitialPUGAsent Or Me.StartSessionisInitialPUGBsent) AndAlso _
                                       (Not Me.StartSessionisCALBsent And Not Me.StartSessionisPMCLsent And Not Me.StartSessionisBMCLsent) AndAlso _
                                       AnalyzerController.Instance.Analyzer.ISEAnalyzer.LastProcedureResult <> ISEAnalyzerEntity.ISEProcedureResult.Exception Then
                                    ' XB 28/04/2014 - Task #1587

                                    ' XB 16/12/2013

                                    ' Errors into initial Purges are ommited
                                    ' Continues with Work Session
                                    Me.StartSession(True)

                                    ' XB 16/12/2013 - Manage Exceptions of ISE module - Task #1441
                                    'ElseIf Me.StartSessionisCALBsent Or Me.StartSessionisPMCLsent Or Me.StartSessionisBMCLsent Then
                                ElseIf (Me.StartSessionisCALBsent Or Me.StartSessionisPMCLsent Or Me.StartSessionisBMCLsent) OrElse _
                                       (AnalyzerController.Instance.Analyzer.ISEAnalyzer.LastProcedureResult = ISEAnalyzerEntity.ISEProcedureResult.Exception) Then
                                    ' XB 16/12/2013

                                    If AnalyzerController.Instance.Analyzer.ISEAnalyzer.LastProcedureResult = ISEAnalyzerEntity.ISEProcedureResult.NOK Then
                                        ' Results Error
                                        If Me.StartSessionisCALBsent Then
                                            Me.SkipCALB = True
                                        ElseIf Me.StartSessionisPMCLsent Then
                                            Me.SkipPMCL = True
                                        ElseIf Me.StartSessionisBMCLsent Then
                                            Me.SkipBMCL = True
                                        End If

                                        Me.StartSession(True)
                                    Else
                                        ' ERC Error
                                        StartingSession = False     ' XB 31/10/2013
                                        Me.StartSessionisPending = False

                                        If StartSessionisCALBsent Then
                                            StartSessionisCALBsent = False
                                        ElseIf StartSessionisPMCLsent Then
                                            StartSessionisPMCLsent = False
                                        ElseIf StartSessionisBMCLsent Then
                                            StartSessionisBMCLsent = False
                                        End If

                                        Dim myGlobal As New GlobalDataTO
                                        Dim myOrderTestsDelegate As New OrderTestsDelegate
                                        executeSTD = False
                                        LockISE = False
                                        myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "STD")
                                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then

                                            '  XB 23/05/2014 - #1638
                                            Dim TestsByTypeIntoWS As String = ""
                                            If (CType(myGlobal.SetDatos, Boolean)) Then
                                                TestsByTypeIntoWS = "STD"
                                                myGlobal = myOrderTestsDelegate.IsThereAnyTestByType(Nothing, WorkSessionIDAttribute, "ISE")
                                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                                    If (CType(myGlobal.SetDatos, Boolean)) Then
                                                        TestsByTypeIntoWS = "ALL"
                                                    End If
                                                End If
                                            Else
                                                TestsByTypeIntoWS = "ISE"
                                            End If

                                            If TestsByTypeIntoWS = "STD" Then
                                                ' Only STD
                                                executeSTD = True
                                                EnableButtonAndMenus(False)
                                                Me.SkipPMCL = True
                                            Else
                                                '  XB 23/05/2014 - #1638

                                                ' XB 23/07/2013 - Activate buzzer
                                                If (AnalyzerController.IsAnalyzerInstantiated) Then
                                                    AnalyzerController.Instance.Analyzer.StartAnalyzerRinging()
                                                    System.Threading.Thread.Sleep(500)
                                                End If
                                                ' XB 23/07/2013

                                                If TestsByTypeIntoWS = "ALL" Then
                                                    'ISE Calibrations are wrong so Module is not ready to be used but there are STD Tests pending of execution; a question Message is shown, and the User 
                                                    'can choose between stop the WS Start/Continue process to solve the problem or to continue with the execution despite of the problem
                                                    Dim userAnswer As DialogResult

                                                    ' XB 20/11/2013 - No ISE warnings can appear when ISE module is not installed
                                                    'userAnswer = ShowMessage(Me.Name & "ManageReceptionEvent", GlobalEnumerates.Messages.ISE_MODULE_NOT_AVAILABLE.ToString)
                                                    If AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsISEModuleInstalled Then
                                                        userAnswer = ShowMessage(Me.Name & "ManageReceptionEvent", GlobalEnumerates.Messages.ISE_MODULE_NOT_AVAILABLE.ToString)
                                                    Else
                                                        userAnswer = Windows.Forms.DialogResult.Yes
                                                    End If
                                                    ' XB 20/11/2013

                                                    Application.DoEvents()
                                                    If (userAnswer = DialogResult.Yes) Then
                                                        'User has selected continue WS without ISE Tests; all pending ISE Tests will be blocked
                                                        executeSTD = True
                                                        LockISE = True
                                                        EnableButtonAndMenus(False)
                                                        Me.SkipPMCL = True ' XB 22/10/2013
                                                    Else
                                                        LockISE = True '  XB 23/05/2014 - Lock ISE preparations always #1638
                                                        EnableButtonAndMenus(True, True)   ' XB 07/11/2013
                                                        Application.DoEvents()
                                                    End If

                                                Else
                                                    'ISE Calibrations are wrong so Module is not ready and there are not STD Tests pending of execution; an error Message is shown due to 
                                                    'the WS can not be started
                                                    ShowMessage(Me.Name & "ManageReceptionEvent", GlobalEnumerates.Messages.ONLY_ISE_WS_NOT_STARTED.ToString)
                                                    Application.DoEvents()
                                                    'All ISE Pending Tests will be blocked
                                                    LockISE = True
                                                End If

                                                ' XB 23/07/2013 - Close buzzer
                                                If (AnalyzerController.IsAnalyzerInstantiated) Then
                                                    AnalyzerController.Instance.Analyzer.StopAnalyzerRinging()
                                                    System.Threading.Thread.Sleep(500)
                                                End If
                                                ' XB 22/07/2013

                                            End If    '  XB 23/05/2014 - #1638

                                        End If

                                        If (LockISE) Then
                                            If (Me.ActiveMdiChild Is Nothing) OrElse (Not TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                                                Dim myExecutionsDelegate As New ExecutionsDelegate
                                                myGlobal = myExecutionsDelegate.UpdateStatusByExecutionTypeAndStatus(Nothing, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "PENDING", "LOCKED")
                                            End If

                                            If (Not Me.ActiveMdiChild Is Nothing) AndAlso (TypeOf ActiveMdiChild Is IMonitor) Then
                                                'Refresh the status of ISE Preparations in Monitor Screen if it is the active screen
                                                Dim myDummyUIRefresh As New UIRefreshDS
                                                IMonitor.UpdateWSState(myDummyUIRefresh)
                                            End If
                                        End If

                                        If executeSTD Then
                                            ' Continues with Work Session
                                            Me.StartSession(True)
                                        Else
                                            ShowStatus(Messages.STANDBY)
                                            Cursor = Cursors.Default
                                            ' XBC 29/08/2012 - Enable Monitor Panel
                                            'If (Not ActiveMdiChild Is Nothing) Then
                                            '    ActiveMdiChild.Enabled = True
                                            'End If
                                            ' XB 22/07/2013 - Auto WS Process
                                            SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                                            InitializeAutoWSFlags()
                                            'ActivateActionButtonBarOrSendNewAction()
                                            SetActionButtonsEnableProperty(True) 'AG 31/10/2013 - use this method instead of ActivateActionButtonBarOrSendNewAction (it has previous conditions to buttons activation)
                                        End If

                                    End If

                                End If
                            End If

                        End If ' XBC 27/09/2012 

                    End If
                    ' XBC 26/06/2012


                End If
                'end SGM 12/03/2012

                ' XBC 15/06/2012
                If AnalyzerController.Instance.Analyzer.InstructionTypeReceived = AnalyzerManagerSwActionList.ANSFCP_RECEIVED Then
                    ' XB 30/08/2013
                    myLogAcciones.CreateLogActivity("(Analyzer Change) ANSFCP received (get it !) ", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False)

                    If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.FWCPUVALUE_CHANGED) Then
                        ' XB 30/08/2013
                        myLogAcciones.CreateLogActivity("(Analyzer Change) ANSFCP received (manage it !) ", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False)

                        Dim myLogAccionesAux As New ApplicationLogManager()
                        myLogAccionesAux.CreateLogActivity("(Analyzer Change) calling function ManageAnalyzerConnected ... ", Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False)

                        ManageAnalyzerConnected(True, False)
                        AnalyzerController.Instance.Analyzer.InstructionTypeReceived = AnalyzerManagerSwActionList.NONE

                        '' XB 17/09/2013 - Additional protection - commented by now
                        'AnalyzerController.Instance.Analyzer.WaitingGUI = False

                    End If
                End If
                ' XBC 15/06/2012

                ' XBC 02/08/2012
                If AnalyzerController.Instance.Analyzer.InstructionTypeReceived = AnalyzerManagerSwActionList.ANSPSN_RECEIVED Then
                    AnalyzerController.Instance.Analyzer.InstructionTypeReceived = AnalyzerManagerSwActionList.NONE

                    Dim myGlobal As New GlobalDataTO
                    myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDSOUND, True) 'Send SOUND OFF instruction

                    If Not myGlobal.HasError Then

                        Select Case AnalyzerController.Instance.Analyzer.DifferentWSType
                            Case "1"
                                'the WS can not be started
                                ShowMessage(Me.Name & "ManageReceptionEvent", GlobalEnumerates.Messages.WS_NO_MATCH1.ToString)
                            Case "2"
                                'the WS can not be started
                                ShowMessage(Me.Name & "ManageReceptionEvent", GlobalEnumerates.Messages.WS_NO_MATCH2.ToString)
                        End Select

                        ' XB 10/01/2014 - Initialize flag - Task #1445
                        AnalyzerController.Instance.Analyzer.DifferentWSType = ""

                        If AnalyzerController.Instance.Analyzer.ForceAbortSession Then
                            NotDisplayAbortMsg = True
                            bsTSAbortSessionButton.PerformClick()
                            NotDisplayAbortMsg = False
                            'AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS"
                            'AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConsumption) = ""
                            'AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = ""
                            'myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ABORT, True)
                        End If
                    End If

                End If
                ' XBC 02/08/2012

                'AG 02/04/2013 - Upload results to LIS if any
                If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED) Then
                    Dim onlineExportResults As New ExecutionsDS
                    'Get the last results automatically exported (from AnalyzerManager object)
                    onlineExportResults = AnalyzerController.Instance.Analyzer.LastExportedResults

                    'Add into local structure with pending results to be uploaded and upload them (assynchronous call to a synchronous process)
                    If onlineExportResults.twksWSExecutions.Rows.Count > 0 Then
                        AddResultsIntoQueueToUpload(onlineExportResults)

                        CreateLogActivity("Current results automatic upload (STD, ISE, CALC)", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False) 'AG 02/01/2014 - BT #1433 (v211 patch2)

                        InvokeUploadResultsLIS(False)

                        'Clear the last results automatically exported (in AnalyzerManager object)
                        AnalyzerController.Instance.Analyzer.ClearLastExportedResults()
                    End If
                End If
                'AG 02/04/2013

                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("END BLOCK 3.", Me.Name & ".ManageReceptionEvent ", _
                '                                 EventLogEntryType.FailureAudit, False)
            End If

            'AG 17/07/2013 - Commented and moved until StartEnterInRunningMode to avoid create 2 threads calling EnterRunning method
            ''AG 08/07/2013 - Special mode for work with LIS with automatic actions
            'If autoWSCreationWithLISModeAttribute AndAlso (automateProcessCurrentState = LISautomateProcessSteps.subProcessDownloadOrders OrElse _
            '                                               automateProcessCurrentState = LISautomateProcessSteps.ExitHostQueryNotAvailableButGoToRunning OrElse automateProcessCurrentState = LISautomateProcessSteps.ExitNoWorkOrders) Then
            '    CreateAutomaticWSWithLIS()
            'End If
            ''AG 08/07/2013


            'Debug.Print("IAx00MainMDI.ManageReceptionEvent: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0)) 'AG 12/06/2012 - time estimation

            ' XB 30/08/2013 protection : Ensure that connected Analyzer is registered on DB
            Dim myAx00Action As GlobalEnumerates.AnalyzerManagerAx00Actions
            myAx00Action = AnalyzerController.Instance.Analyzer.AnalyzerCurrentAction
            'If myAx00Action = GlobalEnumerates.AnalyzerManagerAx00Actions.SOUND_DONE Then  ' XB 03/12/2013 - Do not works when Analyzer on StandBy mode
            If CheckAnalyzerIDOnFirstConnection Then                                        ' XB 03/12/2013 - Works fine !- task #1410
                If AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then
                    If String.Compare(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "CLOSED", False) = 0 Then
                        'Get the connected Analyzer
                        Dim myAnalyzerDelegate As New AnalyzersDelegate
                        Dim myAnalyzerData As New AnalyzersDS
                        Dim myGlobal As New GlobalDataTO
                        myGlobal = myAnalyzerDelegate.GetAnalyzer(Nothing)
                        If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                            myAnalyzerData = DirectCast(myGlobal.SetDatos, AnalyzersDS)
                            If (myAnalyzerData.tcfgAnalyzers.Rows.Count > 0) Then
                                Dim myTmpAnalyzerConnected As String = ""
                                myTmpAnalyzerConnected = AnalyzerController.Instance.Analyzer.TemporalAnalyzerConnected
                                If myAnalyzerData.tcfgAnalyzers(0).Generic OrElse myAnalyzerData.tcfgAnalyzers(0).AnalyzerID <> myTmpAnalyzerConnected Then
                                    myLogAcciones.CreateLogActivity("(Analyzer Change) Activate protection to register the Connected Analyzer... ", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.Information, False)
                                    ManageAnalyzerConnected(True, False)
                                    CheckAnalyzerIDOnFirstConnection = False
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            '//JVV 20/09/2013 Si el evento de Auto_Report está activo, se tratará según este if
            If copyRefreshEventList.Contains(GlobalEnumerates.UI_RefreshEvents.AUTO_REPORT) Then
                'Dim sOrderIDs As String = String.Empty
                'With copyRefreshDS
                '    For Each dr As DataRow In .AutoReport.Rows
                '        sOrderIDs &= dr("OrderID").ToString & " / "
                '    Next
                'End With
                'MessageBox.Show("AutoReport per: " & sOrderIDs)

                '//1- marcar las ordenes como impresas una vez lo estén (campo Printed=1) -> twksResults ??
                '//2- generar alarmas si ha habido un error en la impresión (impresora no disponible, etc.) ??
                Dim orderList As List(Of String)
                orderList = Enumerable.Cast(Of String)(From l In copyRefreshDS.AutoReport.AsEnumerable() Select l("OrderID")).Distinct().ToList()

                Dim myGlobalDataTO As New GlobalDataTO
                Dim myReportFrecuency As String = String.Empty, myReportType As String = String.Empty
                Dim myUserSettingDelegate As New UserSettingsDelegate()

                myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_FREQ.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myReportFrecuency = myGlobalDataTO.SetDatos.ToString()
                End If
                myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUT_RESULTS_TYPE.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myReportType = myGlobalDataTO.SetDatos.ToString()
                End If

                '//Impresión del Report Automático en función de la frecuencia y tipo de informe escogidos por el usuario.
                If (myReportFrecuency <> String.Empty) Then
                    Select Case (myReportFrecuency)
                        Case "END_WS", "ORDER"
                            If (String.Equals(myReportType, "COMPACT")) Then
                                XRManager.PrintCompactPatientsReport(copyRefreshDS.AutoReport.Rows(0)("AnalyzerID").ToString, copyRefreshDS.AutoReport.Rows(0)("WorkSessionID").ToString, orderList, True)
                            ElseIf (String.Equals(myReportType, "INDIVIDUAL")) Then
                                XRManager.PrintPatientsFinalReport(copyRefreshDS.AutoReport.Rows(0)("AnalyzerID").ToString, copyRefreshDS.AutoReport.Rows(0)("WorkSessionID").ToString, orderList)
                            End If
                        Case Else
                            'Opción Reset: el informe se lanzará al pulsar el botón de Reset. No hacemos nada aquí.
                    End Select
                End If
            End If
            '//JVV 20/09/2013

            'AG 04/12/2013 - BT #1397 - Show the message here, not when the sensor changes because it could change when the analyzer
            'is scanning and the incomplete samples screen is shown (this screen is TopMost)
            If recoveryResultsWarnFlag AndAlso Not incompleteSamplesOpenFlag Then
                recoveryResultsWarnFlag = False 'Set to false in order to show it only 1 time
                recoveryResultsMessageIsShown = True 'Set to true in order to show it only 1 time

                'Pause running ... show message to user and he has to choose between EXIT pause or ABORT
                Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                Dim recoverInPauseMessage As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_RES_RECOVER_INPAUSE", CurrentLanguageAttribute)

                'Dim exitPauseText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_Exit", CurrentLanguageAttribute) 'Exit
                Dim exitPauseText As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_RecoverResults", CurrentLanguageAttribute) 'Recover results
                Dim userAnswer As DialogResult = DialogResult.No
                userAnswer = BSCustomMessageBox.Show(Me, recoverInPauseMessage, My.Application.Info.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, BSCustomMessageBox.BSMessageBoxDefaultButton.LeftButton, "", exitPauseText, abortButtonText)
                Dim resultData As New GlobalDataTO
                If userAnswer = Windows.Forms.DialogResult.Yes Then
                    'Exit pause
                    resultData = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ENDRUN, True, Nothing, True) 'Last parameter means: START + END

                Else 'No
                    'Abort
                    AbortSession(resultData)
                End If
                recoveryResultsMessageIsShown = False

                'Once the user has answered open the auxiliary screen for recovery results
                CloseActiveMdiChild(True)  'AG 07/01/2014 - BT #1436 call this method
                OpenMDIChildForm(IResultsRecover)
                EnableButtonAndMenus(False)
            End If
            'AG 04/12/2013

        Catch ex As Exception
            'Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.StackTrace + " - " + ex.HResult.ToString + "))" & ". SOURCE --> " & ex.Source, "ManageReceptionEvent", _
                                            EventLogEntryType.Error, False)

            myLogAcciones.CreateLogActivity("Instruction Recived --> " & pInstructionReceived, "ManageReceptionEvent", _
                                            EventLogEntryType.Error, False)
        End Try

        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pCheckFwSwCompatible"></param>
    ''' <param name="pisReportSATLoading"></param>
    ''' <remarks>XBC 15/06/2012
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub ManageAnalyzerConnected(ByVal pCheckFwSwCompatible As Boolean, ByVal pisReportSATLoading As Boolean)
        Dim myGlobal As New GlobalDataTO

        Try
            Dim myLogAccionesAux As New ApplicationLogManager()
            myLogAccionesAux.CreateLogActivity("(Analyzer Change) Manage Analyzer Connection... ", Name & ".ManageAnalyzerConnected ", EventLogEntryType.Information, False)

            If (pisReportSATLoading) Then Exit Sub

            Dim RefreshScreen As Boolean = False
            Dim BlockCommunications As Boolean = False

            If (pCheckFwSwCompatible AndAlso Not AnalyzerController.Instance.Analyzer.IsFwSwCompatible) Then
                NotDisplayErrorCommsMsg = True

                'Obtain needed fw version
                Dim mySwVersion As String
                Dim myUtil As New Utilities

                myGlobal = myUtil.GetSoftwareVersion()
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    mySwVersion = myGlobal.SetDatos.ToString

                    myGlobal = AnalyzerController.Instance.Analyzer.GetFwVersionNeeded(mySwVersion)
                    If (Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing) Then
                        Dim myNeededFwVersion As String = CStr(myGlobal.SetDatos)

                        Dim TextPar As New List(Of String)
                        TextPar.Add(myNeededFwVersion)

                        Dim myMsgList As New List(Of String)
                        myMsgList.Add(Messages.FW_VERSION_NOT_VALID2.ToString)
                        ShowMultipleMessage("Warning", myMsgList, Nothing, TextPar)
                    End If
                End If
                BlockCommunications = True
            Else
                'Get the worksession status in case exists
                If (ActiveWorkSession.Length > 0) Then
                    If (AnalyzerController.Instance.Analyzer.IsAnalyzerIDNotLikeWS) Then
                        RefreshScreen = True
                        AnalyzerController.Instance.Analyzer.IsAnalyzerIDNotLikeWS = False

                        Dim myResult As DialogResult
                        Dim myMessageIDList As New List(Of String)
                        myMessageIDList.Add(GlobalEnumerates.Messages.DIFFERENT_SERIALNUM_1.ToString)
                        myMessageIDList.Add("")
                        myMessageIDList.Add(GlobalEnumerates.Messages.DIFFERENT_SERIALNUM_2.ToString)
                        myMessageIDList.Add("")
                        myMessageIDList.Add(GlobalEnumerates.Messages.DIFFERENT_SERIALNUM_3.ToString)
                        myResult = ShowMultipleMessage("Warning", myMessageIDList)

                        If (myResult = Windows.Forms.DialogResult.Cancel) Then
                            NotDisplayErrorCommsMsg = True
                            BlockCommunications = True
                        End If
                    End If
                End If
            End If

            If (BlockCommunications) Then
                'Cut off communications channel
                If (AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY) Then
                    myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP)
                End If

                If (Not myGlobal.HasError) Then
                    If (AnalyzerController.Instance.Analyzer.StopComm()) Then
                        DisconnectComms()
                    End If
                End If

                'XBC 06/09/2012 - Refresh status as Disconnected
                ShowStatus(Messages.NO_CONNECTED)
                Exit Try
            End If

            myLogAccionesAux.CreateLogActivity("(Analyzer Change) ActiveWorkSession [ " & ActiveWorkSession & " ] ", Name & ".ManageAnalyzerConnected ", EventLogEntryType.Information, False)

            If (ActiveWorkSession.Length > 0) Then
                ' Execute process to update the corresponding Work Session tables 
                AnalyzerController.Instance.Analyzer.ForceAbortSession = False
                myLogAccionesAux.CreateLogActivity("(Analyzer Change) Calling function ProcessUpdateWSByAnalyzerID ... ", Name & ".ManageAnalyzerConnected ", EventLogEntryType.Information, False)
                myGlobal = AnalyzerController.Instance.Analyzer.ProcessUpdateWSByAnalyzerID(Nothing)
                If (Not myGlobal.HasError) Then
                    'If (AnalyzerController.Instance.Analyzer.ForceAbortSession) Then
                    ' XBC 21/06/2012 - DISCARDED !!!
                    'bsTSAbortSessionButton.Enabled = True
                    'NotDisplayAbortMsg = True
                    'bsTSAbortSessionButton.PerformClick()
                    'NotDisplayAbortMsg = False
                    'bsTSAbortSessionButton.Enabled = False
                    'End If

                    AnalyzerIDAttribute = AnalyzerController.Instance.Analyzer.ActiveAnalyzer
                    AnalyzerModelAttribute = AnalyzerController.Instance.Analyzer.GetModelValue(AnalyzerIDAttribute)
                    WorkSessionIDAttribute = AnalyzerController.Instance.Analyzer.ActiveWorkSession
                    WSStatusAttribute = AnalyzerController.Instance.Analyzer.WorkSessionStatus

                    If (RefreshScreen) Then
                        CloseActiveMdiChild()

                        'Open the Monitor Screen an update status of the Vertical Buttons Bar
                        OpenMonitorForm(Nothing)

                    ElseIf (Not ActiveMdiChild Is Nothing) Then
                        'If monitor is the current screen inform analyzer and model
                        If (TypeOf ActiveMdiChild Is IMonitor) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)

                            CurrentMdiChild.ActiveAnalyzer = AnalyzerIDAttribute
                            CurrentMdiChild.AnalyzerModel = AnalyzerModelAttribute
                            CurrentMdiChild.ActiveWorkSession = WorkSessionIDAttribute
                            CurrentMdiChild.CurrentWorkSessionStatus = WSStatusAttribute
                        End If
                    End If

                Else
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity("Analyzer ID checking", "ManageAnalyzerConnected", EventLogEntryType.Error, False)
                    ShowMessage("Error", GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString)
                End If
            End If
        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ManageAnalyzerConnected", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare GUI for Disconnected Communications
    ''' </summary>
    ''' <remarks>Created by XBC 18/06/2012
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub DisconnectComms()
        Try
            ForceRefreshOnFirstStandBy = True               ' XB 06/11/2013
            CheckAnalyzerIDOnFirstConnection = True         ' XB 03/12/2013
            CheckAnalyzerIDOnFirstConnectionForISE = True   ' XB 03/12/2013

            Dim myGlobal As New GlobalDataTO
            'Inform the analyzer manager
            myGlobal = AnalyzerController.Instance.Analyzer.ProcessUSBCableDisconnection(True)
            SetActionButtonsEnableProperty(False)
            If Not myGlobal.HasError Then
                ''does not call the method ManageAlarms
                'AnalyzerController.Instance.Analyzer.Alarms.Add(GlobalEnumerates.Alarms.COMMS_ERR)
                'Dim myWSAnalyzerAlarmDS As New WSAnalyzerAlarmsDS
                'myWSAnalyzerAlarmDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow( _
                '        GlobalEnumerates.Alarms.COMMS_ERR.ToString(), AnalyzerIDAttribute, _
                '        DateTime.Now, 1, WorkSessionIDAttribute, Nothing, True, Nothing)

                'Dim myWSAlarmDelegate As New WSAnalyzerAlarmsDelegate
                'myGlobal = myWSAlarmDelegate.Create(Nothing, myWSAnalyzerAlarmDS) 'AG 24/07/2012 - keep using Create, do not use Save, it is not necessary


                If AnalyzerController.Instance.Analyzer.ISEAnalyzer IsNot Nothing AndAlso _
                   AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsISEModuleInstalled AndAlso _
                   AnalyzerController.Instance.Analyzer.ISEAnalyzer.CurrentProcedure <> ISEAnalyzerEntity.ISEProcedures.None Then
                    Dim myISEResultWithComErrors As ISEResultTO = New ISEResultTO
                    myISEResultWithComErrors.ISEResultType = ISEResultTO.ISEResultTypes.ComError
                    AnalyzerController.Instance.Analyzer.ISEAnalyzer.LastISEResult = myISEResultWithComErrors

                    If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                        Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                        CurrentMdiChild.PrepareErrorMode()
                    End If

                End If

            Else
                ShowMessage("Error", myGlobal.ErrorMessage)
            End If
            SetActionButtonsEnableProperty(True)
            ControlBox = True

            If Not ActiveMdiChild Is Nothing Then
                '- Configuration screen (close screen without save changes)
                ' The 1st idea was update the ports combo but it was CANCELED due a system error was triggered and it was difficult to solve
                Dim myRefreshDS As UIRefreshDS = Nothing
                Dim myRefreshEventList As New List(Of GlobalEnumerates.UI_RefreshEvents)
                If (TypeOf ActiveMdiChild Is IConfigGeneral) Then
                    Dim CurrentMdiChild As IConfigGeneral = CType(ActiveMdiChild, IConfigGeneral)
                    CurrentMdiChild.RefreshScreen(myRefreshEventList, myRefreshDS)

                ElseIf (TypeOf ActiveMdiChild Is IMonitor) Then
                    Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                    myRefreshEventList.Add(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED)
                    CurrentMdiChild.RefreshScreen(myRefreshEventList, myRefreshDS)
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DisconnectComms ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DisconnectComms ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub
    ' XBC 15/06/2012


    Private Sub OnManageSentEvent(ByVal pInstructionSent As String) Handles MDIAnalyzerManager.SendEvent

        Me.UIThread(Function() ManageSentEvent(pInstructionSent))

    End Sub


    ''' <summary>
    ''' Special Event sent from Communications Layer
    ''' </summary>
    ''' <param name="pInstructionSent"></param>
    ''' <remarks>
    ''' Created by XB-SG 11/05/2012
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    ''' 
    Private Function ManageSentEvent(ByVal pInstructionSent As String) As Boolean
        Try
            If String.Compare(pInstructionSent, AnalyzerManagerSwActionList.WAITING_TIME_EXPIRED.ToString, False) = 0 Then

                'SGM 11/05/2012
                If AnalyzerController.Instance.Analyzer.ISEAnalyzer IsNot Nothing AndAlso _
               AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsISEModuleInstalled AndAlso _
               AnalyzerController.Instance.Analyzer.ISEAnalyzer.CurrentProcedure <> ISEAnalyzerEntity.ISEProcedures.None Then
                    Dim myISEResultWithComErrors As ISEResultTO = New ISEResultTO
                    myISEResultWithComErrors.ISEResultType = ISEResultTO.ISEResultTypes.ComError
                    AnalyzerController.Instance.Analyzer.ISEAnalyzer.LastISEResult = myISEResultWithComErrors
                End If

                If Not ActiveMdiChild Is Nothing Then

                    If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                        Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                        CurrentMdiChild.PrepareErrorMode()
                    End If

                End If
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ManageSentEvent", EventLogEntryType.Error, False)
        End Try
        Return True
    End Function


    ' XB 29/01/2014 - Task #1438
    Private Sub OnManageWatchDogEvent(ByVal pEnable As Boolean) Handles MDIAnalyzerManager.WatchDogEvent

        Me.UIThread(Function() ManageWatchDogEvent(pEnable))

    End Sub

    ''' <summary>
    ''' Special Event to activate/deactivate WatchDog timer
    ''' </summary>
    ''' <param name="pEnable"></param>
    ''' <remarks>
    ''' Created by XB 29/01/2014 - Task #1438
    ''' </remarks>
    Private Function ManageWatchDogEvent(ByVal pEnable As Boolean) As Boolean
        Try
            If pEnable Then AnalyzerController.Instance.Analyzer.BarcodeStartInstrExpected = True '#REFACTORING
            ' Change MDI timer enable value
            watchDogTimer.Enabled = pEnable

            ' Change IWSRotorPositions timer enable value
            Dim MdiChildIsDisabled As Boolean = False
            Dim refreshTriggeredFlag As Boolean = False
            If DisabledMDIChildAttribute.Count > 0 Then
                MdiChildIsDisabled = True
            End If

            Dim myCurrentMDIForm As Form = Nothing
            If Not ActiveMdiChild Is Nothing Then
                myCurrentMDIForm = ActiveMdiChild
            ElseIf MdiChildIsDisabled Then
                myCurrentMDIForm = DisabledMDIChildAttribute(0)
            End If

            If Not myCurrentMDIForm Is Nothing AndAlso TypeOf myCurrentMDIForm Is IWSRotorPositions Then
                Dim CurrentMdiChild As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                CurrentMdiChild.RefreshwatchDogTimer_Enable(pEnable)
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ManageWatchDogEvent", EventLogEntryType.Error, False)
        End Try
        Return True
    End Function
    ' XB 29/01/2014


    ''' <summary>
    ''' Event incharge to show information if Ax00 Is disconected.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>CREATE BY: 'TR 20/10/2011
    ''' Modified by: AG 28/11/2013 - BT #1397
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub OnDeviceRemoved(ByVal sender As Object, ByVal e As Biosystems.Ax00.PresentationCOM.DetectorForm.DriveDetectorEventArgs)
        Try
            ForceRefreshOnFirstStandBy = True               ' XB 06/11/2013
            CheckAnalyzerIDOnFirstConnection = True         ' XB 03/12/2013
            CheckAnalyzerIDOnFirstConnectionForISE = True   ' XB 03/12/2013

            'Get the open port.
            Dim myConnectedPort As String = ""
            Dim isConnected As Boolean = False
            If (AnalyzerController.IsAnalyzerInstantiated) Then
                isConnected = AnalyzerController.Instance.Analyzer.Connected
                myConnectedPort = AnalyzerController.Instance.Analyzer.PortName
                'Remove special character in case Port Number is Greather than 9 
                myConnectedPort = myConnectedPort.Replace("\\.\", String.Empty)
            End If

            If isConnected AndAlso Not String.Compare(myConnectedPort, String.Empty, False) = 0 Then
                Dim MyPortList As New List(Of String)
                MyPortList = IO.Ports.SerialPort.GetPortNames().ToList
                'Search on myPortList if the current port is on the list.
                If MyPortList.Where(Function(a) String.Compare(a, myConnectedPort, False) = 0).Count = 0 Then

                    'TR 10/11/2011 -Create a log entry to inform the desconnection.
                    CreateLogActivity(GetMessageText(e.Message, CurrentLanguageAttribute), Name & ".OnDeviceRemoved", EventLogEntryType.Information, False) 'AG 25/03/2014 - Information instead of Error

                    'XB + AG 09/09/2013 - error comms while automatic WS creation process is reading barcode
                    If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState = LISautomateProcessSteps.subProcessReadBarcode Then
                        'processingBeforeRunning = "2" 'Aborted
                        SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                        InitializeAutoWSFlags()
                        StopMarqueeProgressBar()
                    End If
                    'XB + AG 09/09/2013

                    'AG 24/10/2011
                    'Inform the analyzer manager
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = AnalyzerController.Instance.Analyzer.ProcessUSBCableDisconnection()
                    SetActionButtonsEnableProperty(False)
                    If Not myGlobal.HasError Then
                        recoveryResultsWarnFlag = False 'AG 28/11/2013 - BT #1397

                        'AG 13/02/2012 - Insert a COMMS_ERR alarm in DB (this is required because the ProcessUSBCableDisconnection
                        'does not call the method ManageAlarms
                        'AnalyzerController.Instance.Analyzer.Alarms.Clear() 'AG 21/05/2012 - if not communications do not remove analyzermanager alarms, only do not show them
                        AnalyzerController.Instance.Analyzer.Alarms.Add(GlobalEnumerates.Alarms.COMMS_ERR)
                        Dim myWSAnalyzerAlarmDS As New WSAnalyzerAlarmsDS
                        myWSAnalyzerAlarmDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow( _
                                GlobalEnumerates.Alarms.COMMS_ERR.ToString(), AnalyzerIDAttribute, _
                                DateTime.Now, 1, WorkSessionIDAttribute, Nothing, True, Nothing)

                        Dim myWSAlarmDelegate As New WSAnalyzerAlarmsDelegate
                        myGlobal = myWSAlarmDelegate.Create(Nothing, myWSAnalyzerAlarmDS) 'AG 24/07/2012 - keep using Create, do not use Save, it is not necessary
                        'AG 13/02/2012

                        ShowMessage("Warning", GlobalEnumerates.Messages.ERROR_COMM.ToString)
                        Debug.Print("ERROR-COMM IN Ax00MainMDI.OnDeviceRemoved")

                        'SGM 11/05/2012
                        If AnalyzerController.Instance.Analyzer.ISEAnalyzer IsNot Nothing AndAlso _
                           AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsISEModuleInstalled AndAlso _
                           AnalyzerController.Instance.Analyzer.ISEAnalyzer.CurrentProcedure <> ISEAnalyzerEntity.ISEProcedures.None Then

                            Dim myISEResultWithComErrors As ISEResultTO = New ISEResultTO
                            myISEResultWithComErrors.ISEResultType = ISEResultTO.ISEResultTypes.ComError
                            AnalyzerController.Instance.Analyzer.ISEAnalyzer.LastISEResult = myISEResultWithComErrors

                            If (TypeOf ActiveMdiChild Is IISEUtilities) Then
                                Dim CurrentMdiChild As IISEUtilities = CType(ActiveMdiChild, IISEUtilities)
                                CurrentMdiChild.PrepareErrorMode()
                            End If

                        End If
                        'SGM 11/05/2012

                    Else
                        ShowMessage("Error", myGlobal.ErrorMessage)
                    End If
                    SetActionButtonsEnableProperty(True)

                    If Not ActiveMdiChild Is Nothing Then
                        '- Configuration screen (close screen without save changes)
                        ' The 1st idea was update the ports combo but it was CANCELED due a system error was triggered and it was difficult to solve
                        Dim myRefreshDS As UIRefreshDS = Nothing
                        Dim myRefreshEventList As New List(Of GlobalEnumerates.UI_RefreshEvents)
                        If (TypeOf ActiveMdiChild Is IConfigGeneral) Then
                            Dim CurrentMdiChild As IConfigGeneral = CType(ActiveMdiChild, IConfigGeneral)
                            CurrentMdiChild.RefreshScreen(myRefreshEventList, myRefreshDS) 'DL16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)

                            'AG 13/02/2012 - Refresh monitor (new alarm)
                        ElseIf (TypeOf ActiveMdiChild Is IMonitor) Then
                            Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                            myRefreshEventList.Add(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED)
                            'TR 25/09/2012 -Add the sensor value change to update the pc no connected Led.
                            myRefreshEventList.Add(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED)
                            CurrentMdiChild.RefreshScreen(myRefreshEventList, myRefreshDS) 'DL16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                            'AG 13/02/2012

                            'DL 26/09/2012
                            'ElseIf (TypeOf ActiveMdiChild Is IChangeRotor) Then
                            'SetActionButtonsEnableProperty(False)
                            '   EnableButtonAndMenus(False)

                            'DL 26/09/2012
                        End If
                    End If
                    'AG 24/10/2011

                End If
            End If

            ' XBC 04/09/2012 - Cut off communications channel
            'AnalyzerController.Instance.Analyzer.StopComm()
            ' ALBERT !!!!!!! HAURIA DE SER AIXO PERO NO FUNCIONA !!!!!!!!!!!!!!!!!!!!!

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OnDeviceRemoved ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OnDeviceRemoved ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub


    ''' <summary>
    ''' Activation and deactivation event to be called from outher layer
    ''' </summary>
    ''' <param name="pEnable"></param>
    ''' <remarks>
    ''' Created by XBC 08/02/2012
    ''' Modified by XBC 11/07/2012 - add protection while Wup is processing
    ''' </remarks>
    Public Sub OnManageActivateScreenEvent(ByVal pEnable As Boolean, ByVal pMessageID As GlobalEnumerates.Messages) Handles myISEUtilities.ActivateScreenEvent
        Try
            If String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS") Then '#REFACTORING
                Exit Sub  ' XBC 11/07/2012
            End If

            Me.EnableButtonAndMenus(pEnable)
            Me.ControlBox = pEnable

            If Not pEnable Then
                Me.InitializeMarqueeProgreesBar()
            Else
                Me.StopMarqueeProgressBar()
            End If
            Application.DoEvents()

            Me.ShowStatus(pMessageID)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".OnManageActivateScreenEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".OnManageActivateScreenEvent ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pEnable"></param>
    ''' <remarks>Created by SGM 14/05/2012</remarks>
    Public Sub OnActivateVerticalButtonsEvent(ByVal pEnable As Boolean) Handles myISEUtilities.ActivateVerticalButtonsEvent
        Try
            MyClass.SetActionButtonsEnableProperty(pEnable)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".OnActivateVerticalButtonsEvent ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".OnActivateVerticalButtonsEvent ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

#Region "ManageReceptionEvents subMethods"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' AG 23/10/2013 creation - moved from ManageEventReception
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub ShowLabelInStatusBar()
        Try
            If Not bsAnalyzerStatus Is Nothing Then
                'bsAnalyzerStatus.DisplayStyle = ToolStripItemDisplayStyle.Text
                If AnalyzerController.Instance.Analyzer.Connected Then
                    'AG 03/10/2011 - Sleeping + analyzer ready: CONNECTED // Sleeping + NO analyzer ready: SLEEPING
                    'bsAnalyzerStatus.Text = AnalyzerController.Instance.Analyzer.AnalyzerStatus.ToString()

                    If AnalyzerController.Instance.Analyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING AndAlso AnalyzerController.Instance.Analyzer.AnalyzerIsReady Then
                        'bsAnalyzerStatus.Text = "CONNECTED"
                        'TR 20/09/2012 -enable buttons and menu elements.
                        EnableButtonAndMenus(True)
                        ShowStatus(Messages.CONNECTED)

                    ElseIf AnalyzerController.Instance.Analyzer.AnalyzerStatus <> AnalyzerManagerStatus.SLEEPING Then
                        'bsAnalyzerStatus.Text = AnalyzerController.Instance.Analyzer.AnalyzerStatus.ToString()
                        Select Case AnalyzerController.Instance.Analyzer.AnalyzerStatus
                            'Case GlobalEnumerates.AnalyzerManagerStatus.MONITOR
                            'Case GlobalEnumerates.AnalyzerManagerStatus.SLEEPING

                            Case GlobalEnumerates.AnalyzerManagerStatus.STANDBY
                                ' XBC 13/07/2012
                                Dim ISEOperating As Boolean = False
                                If AnalyzerController.Instance.Analyzer.ISEAnalyzer IsNot Nothing AndAlso _
                                   AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsISEModuleInstalled AndAlso _
                                   AnalyzerController.Instance.Analyzer.ISEAnalyzer.CurrentProcedure <> ISEAnalyzerEntity.ISEProcedures.None Then
                                    'Do not change the status text
                                    ISEOperating = True
                                End If
                                ' XBC 13/07/2012

                                '1st Start recover instrument
                                If AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) = "INPROCESS" OrElse _
                                   AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) = "PAUSED" Then

                                    ShowStatus(Messages.RECOVERING_INSTRUMENT)

                                    '2on Light adjustment (1st light adjustments because it can be performed: when wupprocess is closed or paused, sdownprocess is closed or paused, 
                                ElseIf AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "INPROCESS" OrElse AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess) = "PAUSED" Then
                                    ShowStatus(Messages.LIGHT_ADJUSTMENT)

                                    '3th Shut down instrument
                                ElseIf AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "INPROCESS" OrElse AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "PAUSED" Then
                                    ShowStatus(Messages.SHUTDOWN)


                                    '4rd Start instrument process Activate
                                ElseIf (AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS" OrElse AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "PAUSED") OrElse Not WarmUpFinishedAttribute Then
                                    'ShowStatus(Messages.STARTING_INSTRUMENT)
                                    If Not WarmUpFinishedAttribute Then
                                        Dim myAnalyzerSettingsDS As AnalyzerSettingsDS
                                        Dim myGlobal As GlobalDataTO

                                        'Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
                                        myGlobal = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, _
                                                               AnalyzerIDAttribute, _
                                                               GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString())

                                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                            myAnalyzerSettingsDS = DirectCast(myGlobal.SetDatos, AnalyzerSettingsDS)

                                            If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                                                Dim myDate As String = ""
                                                myDate = myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue
                                                If myDate = "" Then
                                                    WarmUpFinishedAttribute = True
                                                    If AnalyzerController.Instance.Analyzer.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 0 Then
                                                        AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 1 'set sensor to 1
                                                        AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsAnalyzerWarmUp = False 'AG 22/05/2012 -Ise alarms ready to be shown
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If

                                    If Not WarmUpFinishedAttribute And Not ISEOperating Then
                                        If Not BsTimerWUp.Enabled Then BsTimerWUp.Enabled = True
                                        ShowStatus(Messages.STARTING_INSTRUMENT)
                                        'Else
                                        'Do not change the status text
                                    End If

                                    '5th Abort session (once analyzer in Standby the abort process sends a complete wash)
                                ElseIf AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS" OrElse AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "PAUSED" Then
                                    ShowStatus(Messages.ABORTING_SESSION)

                                    '6th (a) Start Work session Reading bar code before Start(these flags has no the PAUSED value)
                                ElseIf AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess) = "INPROCESS" Then
                                    ShowStatus(Messages.BARCODE_READING)

                                    '6th (b) Start Work session (these flags has no the PAUSED value)
                                ElseIf String.Compare(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess), "INPROCESS", False) = 0 _
                                    OrElse AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess) = "INPROCESS" Then
                                    ShowStatus(Messages.STARTING_SESSION)

                                    '7th Connecting                                   
                                ElseIf AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then
                                    'Do not change the status text

                                    '8th ISE initialitation (connection in StandBy)                                   
                                ElseIf AnalyzerController.Instance.Analyzer.ISEAnalyzer IsNot Nothing AndAlso AnalyzerController.Instance.Analyzer.ISEAnalyzer.IsISEInitiating Then
                                    'Do not change the status text

                                    'Other  cases if analyzer is working do not change the status text (for example bar code read from WSPreparation and RotorPosition)
                                ElseIf Not AnalyzerController.Instance.Analyzer.AnalyzerIsReady Then
                                    'Do not change the status text

                                Else
                                    If StartSessionisPending Or StartingSession Then  ' XB 30/10/2013 
                                        ' Nothing to do
                                    Else
                                        'TR 20/09/2012 -enable buttons and menu elements.
                                        EnableButtonAndMenus(True)
                                        ShowStatus(Messages.STANDBY)
                                    End If
                                End If

                                'AG 19/04/2012 - WS is aborted and user do not change screen
                                If String.Equals(WSStatusAttribute, "ABORTED") AndAlso Not ActiveMdiChild Is Nothing Then
                                    '- Monitor (WSStates ... (pRefreshDS.ExecutionStatusChanged contains the information to refresh)
                                    If (TypeOf ActiveMdiChild Is IMonitor) Then
                                        Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                                        CurrentMdiChild.bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
                                    ElseIf (TypeOf ActiveMdiChild Is IResults) Then
                                        Dim CurrentMdiChild As IResults = CType(ActiveMdiChild, IResults)
                                        CurrentMdiChild.bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
                                    ElseIf (TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                                        Dim CurrentMdiChild As IWSRotorPositions = CType(ActiveMdiChild, IWSRotorPositions)
                                        CurrentMdiChild.bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
                                    ElseIf (TypeOf ActiveMdiChild Is IWSSampleRequest) Then
                                        Dim CurrentMdiChild As IWSSampleRequest = CType(ActiveMdiChild, IWSSampleRequest)
                                        CurrentMdiChild.bsErrorProvider1.SetError(Nothing, GetMessageText(GlobalEnumerates.Messages.WS_ABORTED.ToString))
                                    End If
                                End If
                                'AG 19/04/2012

                            Case GlobalEnumerates.AnalyzerManagerStatus.RUNNING
                                'Start Work session (these flags has no the PAUSED value)
                                'Aborting session
                                If AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS" OrElse AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "PAUSED" Then
                                    ShowStatus(Messages.ABORTING_SESSION)

                                    'Normal case
                                ElseIf AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess) = "INPROCESS" OrElse AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess) = "INPROCESS" _
                                    OrElse AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RUNNINGprocess) = "INPROCESS" Then
                                    'Do not change the status text
                                ElseIf AnalyzerController.Instance.Analyzer.AllowScanInRunning Then ' TR 28/01/2014 -Validate only if analyzer enter on PAUSE MODE not when INPROCESS
                                    ShowStatus(Messages.PAUSE_IN_RUNNING)
                                    '//25/10/2013 - CF BT#1348 Adds the 'paused' status in the statusbar.

                                Else
                                    ShowStatus(Messages.RUNNING)
                                End If

                            Case GlobalEnumerates.AnalyzerManagerStatus.SLEEPING    'DL 04/06/2012
                                If flagExitWithShutDown Then                        'DL 04/06/2012
                                    'Disable all buttons until Ax00 accept the new instruction
                                    SetActionButtonsEnableProperty(False)
                                    Me.Close() 'DL 04/06/2012
                                End If                                              'DL 04/06/2012  

                        End Select

                    End If
                    'AG 03/10/2011

                    'RH + AG 30/03/2012
                    Dim Flag1 As String = AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess)
                    Dim Flag2 As String = AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess)

                    If Not String.Equals(Flag1, "INPROCESS") AndAlso _
                       Not String.Equals(Flag1, "PAUSED") AndAlso _
                       Not String.Equals(Flag2, "INPROCESS") AndAlso _
                       Not String.Equals(Flag2, "PAUSED") Then

                        SetEnableMainTab(True)

                    End If
                    'RH + AG 30/03/2012
                Else

                    If flagExitWithShutDown AndAlso AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess) = "CLOSED" Then                        'DL 04/06/2012
                        'Disable all buttons until Ax00 accept the new instruction
                        SetActionButtonsEnableProperty(False)
                        Me.Close() 'DL 04/06/2012

                    ElseIf AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then
                        ShowStatus(Messages.CONNECTING)
                    Else
                        'bsAnalyzerStatus.Text = "NO CONNECTED"
                        ShowStatus(Messages.NO_CONNECTED)
                        SetEnableMainTab(False) 'RH 27/03/2012
                    End If

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ShowLabelInStatusBar", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ShowLabelInStatusBar", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Communications layer informs about execution status change
    ''' </summary>
    ''' <param name="copyRefreshEventList"></param>
    ''' <param name="copyRefreshDS"></param>
    ''' <param name="monitorTreated"></param>
    ''' <remarks>AG 07/01/2014</remarks>
    Private Sub PerformNewExecutionStatusDone(ByVal copyRefreshEventList As List(Of GlobalEnumerates.UI_RefreshEvents), copyRefreshDS As UIRefreshDS, ByRef MonitorTreated As Boolean)
        Try
            'Call the RefreshScreen method when the current screen is:
            '- Monitor (WSStates) ... (pRefreshDS.ExecutionStatusChanged contains the information to refresh)

            'RH 10/10/2011
            If Not ActiveMdiChild Is Nothing Then
                If (TypeOf ActiveMdiChild Is IMonitor AndAlso Not MonitorTreated) Then
                    Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                    If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                        MonitorTreated = True
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PerformNewExecutionStatusDone ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PerformNewExecutionStatusDone ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Communications layer informs about new calculation has been done
    ''' </summary>
    ''' <param name="copyRefreshEventList"></param>
    ''' <param name="copyRefreshDS"></param>
    ''' <param name="monitorTreated"></param>
    ''' <param name="resultTreated"></param>
    ''' <remarks>AG 07/01/2014</remarks>
    Private Sub PerformNewCalculationsDone(ByVal copyRefreshEventList As List(Of GlobalEnumerates.UI_RefreshEvents), copyRefreshDS As UIRefreshDS, ByRef MonitorTreated As Boolean, ByRef resultTreated As Boolean)
        Try
            '[This case means new results calculated + new readings received!!]
            'Call the RefreshScreen method when the current screen is:

            'RH 10/10/2011
            If Not ActiveMdiChild Is Nothing Then
                '- Monitor (WSStates ... (pRefreshDS.ExecutionStatusChanged contains the information to refresh)
                If (TypeOf ActiveMdiChild Is IMonitor AndAlso Not MonitorTreated) Then
                    Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                    CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                    MonitorTreated = True

                    '- ResultForm ... (pRefreshDS.ExecutionStatusChanged contains the information to refresh)
                ElseIf (TypeOf ActiveMdiChild Is IResults AndAlso Not resultTreated) Then
                    Dim CurrentMdiChild As IResults = CType(ActiveMdiChild, IResults)
                    CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)

                    resultTreated = True

                    'CurveResultForm ... (pRefreshDS.ExecutionStatusChanged contains the information to refresh)
                    'BE CAREFULL: CurveResultForm & Graphical Abs(t) screen are open as popup not as MDIChild
                    ' DL 24/05/2011
                    Dim found As Boolean = False
                    Dim index As Integer
                    ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                    'myLogAcciones.CreateLogActivity("1- BEFORE NoMDIChildActiveFormsAttribute.Count FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)
                    For index = 0 To NoMDIChildActiveFormsAttribute.Count - 1
                        If String.Compare(NoMDIChildActiveFormsAttribute.Item(index).Name, IResultsCalibCurve.Name, False) = 0 Then
                            found = True
                            Exit For
                        End If
                    Next index
                    ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                    'myLogAcciones.CreateLogActivity("1- AFTER NoMDIChildActiveFormsAttribute.Count FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)

                    If found Then
                        Dim CurrentNoMdiChild As IResultsCalibCurve
                        CurrentNoMdiChild = CType(NoMDIChildActiveFormsAttribute.Item(index), IResultsCalibCurve)

                        With CurrentNoMdiChild
                            .ActiveAnalyzer = AnalyzerIDAttribute
                            .ActiveWorkSession = WorkSessionIDAttribute
                            .AnalyzerModel = AnalyzerModelAttribute
                            .AverageResults = CurrentMdiChild.AverageResults
                            .ExecutionResults = CurrentMdiChild.ExecutionResults
                            '.TestSelectedName = CurrentMdiChild.SelectedTestName
                            .AcceptedRerunNumber = CurrentMdiChild.AcceptedRerunNumber
                        End With

                        CurrentNoMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentNoMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                    End If
                    ' END DL 24/05/2011

                End If

            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PerformNewCalculationsDone ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PerformNewCalculationsDone ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub


    ''' <summary>
    ''' Communications layer informs about biochemical readings reception
    ''' </summary>
    ''' <param name="copyRefreshEventList"></param>
    ''' <param name="copyRefreshDS"></param>
    ''' <remarks>AG 07/01/2014</remarks>
    Private Sub PerformNewReadingsReception(ByVal copyRefreshEventList As List(Of GlobalEnumerates.UI_RefreshEvents), copyRefreshDS As UIRefreshDS)
        Try
            '[This case means new results calculated + new readings received!!]
            'Call the RefreshScreen method when the current screen is:

            If Not ActiveMdiChild Is Nothing Then
                'Graphical Abs(t) screen ... (pRefreshDS.ReceivedReadings contains the information to refresh)
                'BE CAREFULL: Graphical Abs(t) screen are open as popup not as MDIChild

                ' DL 24/05/2011
                Dim found As Boolean = False
                Dim index As Integer
                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("2- BEFORE NoMDIChildActiveFormsAttribute.Count FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)
                For index = 0 To NoMDIChildActiveFormsAttribute.Count - 1
                    If NoMDIChildActiveFormsAttribute.Item(index).Name = IResultsAbsCurve.Name Then
                        found = True
                        Exit For
                    End If
                Next index
                ''TR 18/09/2012 -Log to trace incase Exception error is race TO DELETE.
                'myLogAcciones.CreateLogActivity("2- After NoMDIChildActiveFormsAttribute.Count FOR.", Me.Name & ".ManageReceptionEvent ", EventLogEntryType.FailureAudit, False)
                If found Then
                    Dim CurrentNoMdiChild As IResultsAbsCurve
                    CurrentNoMdiChild = CType(NoMDIChildActiveFormsAttribute.Item(index), IResultsAbsCurve)
                    CurrentNoMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentNoMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                End If
                ' END DL 24/05/2011

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PerformNewReadingsReception ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PerformNewReadingsReception ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Communications layer informs about new alarms reception (alarm active or solved)
    ''' </summary>
    ''' <param name="copyRefreshEventList"></param>
    ''' <param name="copyRefreshDS"></param>
    ''' <param name="monitorTreated"></param>
    ''' <param name="changeRotorTreated"></param>
    ''' <param name="conditioningTreated"></param>
    ''' <remarks>AG 08/01/2014</remarks>
    Private Sub PerformNewAlarmsReception(ByVal copyRefreshEventList As List(Of GlobalEnumerates.UI_RefreshEvents), copyRefreshDS As UIRefreshDS, _
                                          ByRef monitorTreated As Boolean, ByRef changeRotorTreated As Boolean, ByRef conditioningTreated As Boolean)
        Try
            '[This case means new alarms received (alarm active or alarm solved!!]
            'Call the RefreshScreen method when the current screen is:

            'AG 17/10/2011 - during the application load and autoconnect the ActiveMdiChild is nothing but the current screen is monitor
            Dim MdiChildIsDisabled As Boolean = False
            Dim refreshTriggeredFlag As Boolean = False 'AG 23/01/2012 - Indicates if the refresh method is performed or not
            If DisabledMDIChildAttribute.Count > 0 Then
                MdiChildIsDisabled = True
            End If

            Dim myCurrentMDIForm As Form = Nothing
            If Not ActiveMdiChild Is Nothing Then
                myCurrentMDIForm = ActiveMdiChild
            ElseIf MdiChildIsDisabled Then
                myCurrentMDIForm = DisabledMDIChildAttribute(0)
            End If
            'AG 17/10/2011

            If Not myCurrentMDIForm Is Nothing Then
                '- Monitor (Main or Alarms) screen ... (pRefreshDS.ReceivedAlarms contains the information to refresh)
                '- Change rotor ... (pRefreshDS.SensorValueChanged contains the information to refresh)

                If (TypeOf myCurrentMDIForm Is IMonitor AndAlso Not monitorTreated) Then
                    Dim CurrentMdiChild As IMonitor = CType(myCurrentMDIForm, IMonitor)
                    If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                        monitorTreated = True
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                    End If
                    'AG 16/03/2012 - If no reactions rotor alarm appears while the UI is disabled we must reactivated it
                    'Change rotor
                ElseIf (TypeOf myCurrentMDIForm Is IChangeRotor AndAlso Not changeRotorTreated) Then
                    Dim CurrentMdiChild As IChangeRotor = CType(myCurrentMDIForm, IChangeRotor)
                    If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        changeRotorTreated = True
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                        'AG 16/03/2012
                    End If
                    'AG 28/03/2012 - When cover open and enabled alarms appears it disables Scan barcode button. Else enable it
                    'Sample request

                ElseIf (TypeOf myCurrentMDIForm Is IConditioning AndAlso Not conditioningTreated) Then
                    Dim CurrentMdiChild As IConditioning = CType(myCurrentMDIForm, IConditioning)
                    If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        'conditioningTreated = True
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'DL 05/06/2012
                        'DL 05/06/2012
                    End If
                ElseIf (TypeOf myCurrentMDIForm Is IWSSampleRequest) Then
                    Dim CurrentMdiChild As IWSSampleRequest = CType(myCurrentMDIForm, IWSSampleRequest)
                    If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'AG + TR 23/09/2011
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                    End If
                    'AG 28/03/2012 - When cover open and enabled alarms appears it disables Scan barcode /Check bottle volume button. Else enable it
                    'Rotor positions
                ElseIf (TypeOf myCurrentMDIForm Is IWSRotorPositions) Then
                    Dim CurrentMdiChild As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                    If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'AG + TR 23/09/2011
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                    End If
                    'AG 28/03/2012 - When cover open and enabled alarms appears it disables ISE command button. Else enable it
                ElseIf (TypeOf myCurrentMDIForm Is IISEUtilities) Then
                    Dim CurrentMdiChild As IISEUtilities = CType(myCurrentMDIForm, IISEUtilities)
                    If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                    End If
                End If

            End If

            'AG 17/10/2011
            If processingBeforeRunning = "0" Then refreshTriggeredFlag = False 'AG 23/01/2012 - special case: enter running process in progress
            If MdiChildIsDisabled AndAlso refreshTriggeredFlag Then
                'Activate the current mdi child who is disabled once the refresh method is finished
                If DisabledMDIChildAttribute.Count > 0 Then
                    DisabledMDIChildAttribute(0).Enabled = True
                    If DisabledMDIChildAttribute.Count = 1 Then
                        DisabledMDIChildAttribute.Clear()
                    Else
                        DisabledMDIChildAttribute.Remove(DisabledMDIChildAttribute(0))
                    End If
                End If
            End If
            'AG 17/10/2011

            'DL 31/07/2012. Begin 
            Dim linq As New List(Of UIRefreshDS.ReceivedAlarmsRow)

            'DisconnectComms GUI 
            linq = (From a As UIRefreshDS.ReceivedAlarmsRow In copyRefreshDS.ReceivedAlarms _
                     Where (String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_CPU_ERR.ToString) _
                    OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_DISTRIBUTED_ERR.ToString) _
                    OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_REPOSITORY_ERR.ToString) _
                    OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_CHECKSUM_ERR.ToString) _
                    OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_MAN_ERR.ToString)) _
                       And a.AlarmStatus = True _
                    Select a).ToList

            If linq.Count > 0 Then DisconnectComms()
            'DL 31/07/2012. End 
            linq = Nothing

            'Alarm Messages (Messages do not required ActiveMdiChild) 
            '--------------------------------------------------------
            '(All screens) Finally show message depending the alarms received and update vertical button bar depending the current alarms
            ShowAlarmsOrSensorsWarningMessages(GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, copyRefreshDS) 'DL 16/09/2011 ShowAlarmWarningMessages(pRefreshDS) 
            SetActionButtonsEnableProperty(True)
            'Debug.Print("ShowAlarmsOrSensorsWarningMessages called from ManageReceptionEvent-1") 
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PerformNewAlarmsReception ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PerformNewAlarmsReception ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub


    ''' <summary>
    ''' Communications layer informs about biochemical readings reception
    ''' </summary>
    ''' <param name="copyRefreshEventList"></param>
    ''' <param name="copyRefreshDS"></param>
    ''' <param name="monitorTreated"></param>
    ''' <param name="changeRotorTreated"></param>
    ''' <param name="conditioningTreated"></param>
    ''' <param name="wsRotorPositionTreated"></param>
    ''' <param name="configSettingsLISTreated"></param>
    ''' <remarks>AG 08/01/2014
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub PerformNewSensorValueChanged(ByVal copyRefreshEventList As List(Of GlobalEnumerates.UI_RefreshEvents), copyRefreshDS As UIRefreshDS, _
                                          ByRef monitorTreated As Boolean, ByRef changeRotorTreated As Boolean, ByRef conditioningTreated As Boolean, _
                                          ByRef wsRotorPositionTreated As Boolean, ByRef configSettingsLISTreated As Boolean)
        Try
            '[This case means internal sensors changed (analyzer sensors ANSINF, or false sensors like processes finishes,...!!]
            'Call the RefreshScreen method when the current screen is:

            'AG 20/01/2012 - during the application load and autoconnect the ActiveMdiChild is nothing but the current screen is monitor
            Dim MdiChildIsDisabled As Boolean = False
            Dim refreshTriggeredFlag As Boolean = False 'AG 23/01/2012 - Indicates if the refresh method is performed or not
            If DisabledMDIChildAttribute.Count > 0 Then
                MdiChildIsDisabled = True
            End If

            Dim myCurrentMDIForm As System.Windows.Forms.Form = Nothing
            If Not ActiveMdiChild Is Nothing Then
                myCurrentMDIForm = ActiveMdiChild
            ElseIf MdiChildIsDisabled Then
                myCurrentMDIForm = DisabledMDIChildAttribute(0)
            End If
            'AG 12/01/2012

            If Not myCurrentMDIForm Is Nothing Then

                'AG 27/08/2012 - Results recovery process has an special screen 
                If AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) = "INPROCESS" AndAlso _
                   AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVERY_RESULTS_STATUS) = 1 AndAlso _
                   Not (TypeOf myCurrentMDIForm Is IResultsRecover) Then

                    'AG 28/11/2013 - BT #1397
                    'Comment the old code
                    ''Open the results recover popup screen
                    'OpenMDIChildForm(IResultsRecover)
                    'EnableButtonAndMenus(False)
                    If Not AnalyzerController.Instance.Analyzer.AllowScanInRunning Then
                        'Normal running ... same code as in previous versions
                        'Open the results recover popup screen
                        recoveryResultsWarnFlag = False

                        CloseActiveMdiChild(True)  'AG 07/01/2014 - BT #1436 call this method
                        OpenMDIChildForm(IResultsRecover)
                        EnableButtonAndMenus(False)


                        'Only show message one time
                        'Do not shown when the incomplete samples (or HQMonitor) screen are open because they are TopMost
                    ElseIf Not recoveryResultsWarnFlag AndAlso Not recoveryResultsMessageIsShown Then

                        'Pause running ... show message to user and he has to choose between EXIT pause or ABORT
                        'The message is market to be shown when the auxiliary screen for incomplete samples is closed (it has TopMost = TRUE)
                        recoveryResultsWarnFlag = True
                    End If
                    'AG 28/11/2013

                ElseIf AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) <> "INPROCESS" AndAlso _
                       AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVERY_RESULTS_STATUS) = 0 AndAlso _
                       (TypeOf myCurrentMDIForm Is IResultsRecover) Then

                    'Close the results recover popup screen and open the monitor screen
                    'WSStatusAttribute = "ABORTED" 'AG 07/03/2014 - integrate patches 'AG 18/02/2014 - #1513 do not abort WS after recover results!! 'After recovery results the worksession becomes aborted
                    'Debug.Print("ManageReceptionEvent -> ABORTED -> CURRENT MDI FORM = " & myCurrentMDIForm.Name)
                    OpenMonitorForm(myCurrentMDIForm) 'myCurrentMDIForm = IResultsRecover = form to close
                    EnableButtonAndMenus(True)


                Else 'Normal code

                    '- Monitor (Main) ... (pRefreshDS.SensorValueChanged contains the information to refresh)
                    '  <or maybe all monitor TAB's (LED's area)>
                    '- Change rotor ... (pRefreshDS.SensorValueChanged contains the information to refresh)
                    'NOTE: All sensor values are available using the property AnalyzerManager.GetSensorValue
                    If (TypeOf myCurrentMDIForm Is IMonitor AndAlso Not monitorTreated) Then
                        Dim CurrentMdiChild As IMonitor = CType(myCurrentMDIForm, IMonitor)
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                        monitorTreated = True
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012

                    ElseIf (TypeOf myCurrentMDIForm Is IChangeRotor AndAlso Not changeRotorTreated) Then
                        Dim CurrentMdiChild As IChangeRotor = CType(myCurrentMDIForm, IChangeRotor)
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                        changeRotorTreated = True
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012

                        ' XBC 08/02/2012 ISE Utilities placed into PresetationCOM layer
                        'AG 12/01/2012


                    ElseIf (TypeOf myCurrentMDIForm Is IConditioning AndAlso Not conditioningTreated) Then
                        Dim CurrentMdiChild As IConditioning = CType(myCurrentMDIForm, IConditioning)
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        conditioningTreated = True
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone

                        'DL 05/06/2012 ISE Utilities placed into PresetationCOM layer

                    ElseIf (TypeOf myCurrentMDIForm Is IISEUtilities) Then
                        Dim CurrentMdiChild As IISEUtilities = CType(myCurrentMDIForm, IISEUtilities)
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                        'ElseIf (TypeOf myCurrentMDIForm Is Biosystems.Ax00.PresentationCOM.IISEUtilities) Then
                        '    Dim CurrentMdiChild As Biosystems.Ax00.PresentationCOM.IISEUtilities = CType(myCurrentMDIForm, Biosystems.Ax00.PresentationCOM.IISEUtilities)
                        '    CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        '    refreshTriggeredFlag = True
                        'AG 12/01/2012
                        ' XBC 08/02/2012 ISE Utilities placed into PresetationCOM layer

                        'AG 16/03/2012 - If freeze appears while this screen is disable Sw must re-activate it
                        'Sample request
                    ElseIf (TypeOf myCurrentMDIForm Is IWSSampleRequest) Then
                        Dim CurrentMdiChild As IWSSampleRequest = CType(myCurrentMDIForm, IWSSampleRequest)
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'AG + TR 23/09/2011
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                        'Rotor positioning
                    ElseIf (TypeOf myCurrentMDIForm Is IWSRotorPositions AndAlso Not wsRotorPositionTreated) Then
                        Dim CurrentMdiChild As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                        wsRotorPositionTreated = True
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                        'AG 16/03/2012

                        ' XB 24/04/2013
                    ElseIf (TypeOf myCurrentMDIForm Is IConfigLIS AndAlso Not configSettingsLISTreated) Then
                        Dim CurrentMdiChild As IConfigLIS = CType(myCurrentMDIForm, IConfigLIS)
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS)
                        configSettingsLISTreated = True
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone

                    End If


                End If
                'AG 27/08/2012


                'AG 23/09/2011 - Check if analyzer status has changed when is opened a screen witch business depends on analyzer status
                If AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ANALYZER_STATUS_CHANGED) = 1 Then
                    'Several screens have business witch depends on the analyzer status ... so if the analyzer status changes when screen is opened
                    'they has to be notified (IWSRotorPositions, IConfigAnalyzer, IBarcodeCondig, IChangeRotor, ISatReport_Load, ISettings, IWSSampleRequest, ISatReport ...)
                    TreatScreenDueAnalyzerStatusChanged()
                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ANALYZER_STATUS_CHANGED) = 0 'Once UI refresh ... clear the sensor
                End If
                'AG 23/09/2011

                'AG 20/01/2012
                If String.Compare(processingBeforeRunning, "0", False) = 0 Then refreshTriggeredFlag = False 'AG 23/01/2012 - special case: enter running process in process
                If MdiChildIsDisabled AndAlso refreshTriggeredFlag Then
                    'Activate the current mdi child who is disabled once the refresh method is finished
                    If DisabledMDIChildAttribute.Count > 0 Then
                        DisabledMDIChildAttribute(0).Enabled = True
                        If DisabledMDIChildAttribute.Count = 1 Then
                            DisabledMDIChildAttribute.Clear()
                        Else
                            DisabledMDIChildAttribute.Remove(DisabledMDIChildAttribute(0))
                        End If
                    End If
                End If
                'AG 20/01/2012

            End If

            'AG 23/03/2012 - If errors while status changing Sw must assure the UI becomes enabled again
            If AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ERROR_IN_STATUS_CHANGING) = 1 Then
                If ScreenWorkingProcess Then
                    ScreenWorkingProcess = False
                    processingBeforeRunning = "1"

                    'clear the possible sensor values affected
                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 0
                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 0
                    AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_WARNINGS) = 0
                End If

                'Finally clear the error in change status sensor
                AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ERROR_IN_STATUS_CHANGING) = 0
            End If

            'DL 18/01/2012 - Check if process before running is finished or aborted and then enable UI again
            If AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 1 Then
                'AG 13/02/2012 - these 2 lines must be performed only when BAx00 becomes in RUNNING, not before
                'Time Ago it works because the running instruction was inmediate but in latest Fw versions Running requires 1 minut or more
                'That is why I have to move them into method ActivateActionButtonBarOrSendNewAction (case Running)
                'AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 0 'clear sensor
                'processBeforeRunningFinished = "1" 'Finished ok
                'AG 13/02/2012
                If AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_WARNINGS) = 1 Then
                    processingBeforeRunning = "2" 'Aborted due ise alarms
                    ScreenWorkingProcess = False
                End If
            End If

            'AG 03/10/2012 - I think the 2on part of condition was wrong!!
            'AG 26/03/2012 - Stop sound alarm button disabled when analyzer not sound and when sound off instruction is in queue to be sent ('TR 27/10/2011)
            If AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ANALYZER_SOUND_CHANGED) = 0 OrElse AnalyzerController.Instance.Analyzer.QueueContains(AnalyzerManagerSwActionList.SOUND) Then
                'If AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.ANALYZER_SOUND_CHANGED) = 0 OrElse AnalyzerController.Instance.Analyzer.QueueContains(AnalyzerManagerSwActionList.ENDSOUND) Then
                bsTSSoundOff.Enabled = False 'DL 22/03/212
            Else
                bsTSSoundOff.Enabled = True 'DL 22/03/212
            End If
            'TR 27/10/2011 -END

            'AG 07/03/2012
            If AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 1 Then
                ScreenWorkingProcess = False
                processingRecover = False
            End If

            'AG 15/05/2012
            If AnalyzerController.Instance.Analyzer.GetSensorValue(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM) = 1 Then
                'Clear the sensor once performed the business and stops the timer
                If ElapsedTimeTimer.Enabled Then
                    ElapsedTimeTimer.Stop()
                    'TR 15/05/2012 clean the valible that handle the time because initialization required.
                    TotalSecs = 0
                    TotalSecs2 = 0
                    'TR 15/05/2012 -END.
                End If

                AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.AUTO_PAUSE_BY_ALARM) = 0
            End If

            'AG 14/03/2012 - Business only when specific sensors changes (are informed in copyRefreshDS) (code is moved from ShowAlarmsOrSensorsWarningMessages)
            Dim lnqRes As List(Of UIRefreshDS.SensorValueChangedRow)

            'WarmUp canceled due bottle alarms
            lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In copyRefreshDS.SensorValueChanged _
                     Where String.Compare(a.SensorID, GlobalEnumerates.AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS.ToString, False) = 0 _
                     Select a).ToList
            If lnqRes.Count > 0 Then
                If lnqRes(0).Value = 1 Then
                    If String.Compare(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "PAUSED", False) = 0 Then
                        FinishWarmUp(True)
                    End If
                End If
            End If

            'Freeze while analyzer is processing before running
            lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In copyRefreshDS.SensorValueChanged _
                      Where String.Compare(a.SensorID, GlobalEnumerates.AnalyzerSensors.FREEZE.ToString, False) = 0 _
                      Select a).ToList
            If lnqRes.Count > 0 Then
                If processingBeforeRunning = "0" Then processingBeforeRunning = "2"
                If ScreenWorkingProcess Then ScreenWorkingProcess = False

                StopMarqueeProgressBar()
                EnableButtonAndMenus(True)
                SetActionButtonsEnableProperty(True)
                Cursor = Cursors.Default
            End If
            'AG 14/03/2012

            'AG 23/03/2012 No connection while screen is working leave the loop and activate screen
            lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In copyRefreshDS.SensorValueChanged _
                      Where String.Equals(a.SensorID, GlobalEnumerates.AnalyzerSensors.CONNECTED.ToString) _
                      Select a).ToList
            If lnqRes.Count > 0 Then
                If lnqRes(0).Value = 0 Then 'Note this sensor works different (1 connected (ok), 0 (no connected, connection fails, error))
                    If ScreenWorkingProcess Then
                        ScreenWorkingProcess = False
                        processingBeforeRunning = "1"

                        'clear the possible sensor values affected
                        AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.RECOVER_PROCESS_FINISHED) = 0
                        AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 0
                        AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.ISE_WARNINGS) = 0
                    End If

                Else 'AG 23/05/2012 Connection establisment ok refresh monitor.main
                    If Not myCurrentMDIForm Is Nothing Then
                        If (TypeOf myCurrentMDIForm Is IMonitor) Then
                            Dim CurrentMdiChild As IMonitor = CType(myCurrentMDIForm, IMonitor)
                            If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                                CurrentMdiChild.RefreshAlarmsGlobes(Nothing)
                            End If
                        End If
                    End If
                    'AG 23/05/2012

                End If
                'TR 20/09/2012 Commented by TR. 
                'EnableButtonAndMenus(True)
                Cursor = Cursors.Default
            End If
            lnqRes = Nothing

            'Sensor Messages (messages do not required ActiveMdiChild)
            '---------------------------------------------------------
            ShowAlarmsOrSensorsWarningMessages(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED, copyRefreshDS) 'AG 07/03/2012
            'Debug.Print("ShowAlarmsOrSensorsWarningMessages called from ManageReceptionEvent-2")

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PerformNewSensorValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PerformNewSensorValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub


    ''' <summary>
    ''' Communications layer informs about the new position (reactions rotor well) of the washing station
    ''' </summary>
    ''' <param name="copyRefreshEventList"></param>
    ''' <param name="copyRefreshDS"></param>
    ''' <param name="monitorTreated"></param>
    ''' <remarks>AG 07/01/2014</remarks>
    Private Sub PerformNewWashingStationPositionDone(ByVal copyRefreshEventList As List(Of GlobalEnumerates.UI_RefreshEvents), copyRefreshDS As UIRefreshDS, ByRef MonitorTreated As Boolean)
        Try
            '[This case means washing station in reactions rotor achieves a new position!!]
            'Call the RefreshScreen method when the current screen is:

            If Not ActiveMdiChild Is Nothing Then
                '- Monitor (Reactions Rotor) screen ... (pRefreshDS.ReactionWellStatusChanged contains the information to refresh)
                If (TypeOf ActiveMdiChild Is IMonitor AndAlso Not MonitorTreated) Then
                    Dim CurrentMdiChild As IMonitor = CType(ActiveMdiChild, IMonitor)
                    CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                    MonitorTreated = True
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PerformNewWashingStationPositionDone ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PerformNewWashingStationPositionDone ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Communications layer informs about the new position change (element, status, ...) inside the reagents/samples rotor
    ''' </summary>
    ''' <param name="copyRefreshEventList"></param>
    ''' <param name="copyRefreshDS"></param>
    ''' <remarks>
    ''' Created by  AG 07/01/2014
    ''' Modified by XB 06/02/2014 - Improve WDOG BARCODE_SCAN - Task #1438
    ''' </remarks>
    Private Sub PerformNewWRotorPositionChange(ByVal copyRefreshEventList As List(Of GlobalEnumerates.UI_RefreshEvents), copyRefreshDS As UIRefreshDS, _
                                               ByRef monitorTreated As Boolean, ByRef wsRotorPositionTreated As Boolean)
        Try
            '[This case means new rotor positions changed (element, status, ...)!!]

            ' XB 06/02/2014 - WatchDog timer must False when the LAST scan rotor (SAMPLES) will completed - Task #1438
            'If watchDogTimer.Enabled Then watchDogTimer.Enabled = False
            Dim resultData As GlobalDataTO = Nothing
            Dim myAnalyzerSettings As New AnalyzerSettingsDelegate
            Dim barcodeReagentDisabled As Boolean
            Dim barcodeSampleDisabled As Boolean
            Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS

            resultData = myAnalyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString)
            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                myAnalyzerSettingsDS = CType(resultData.SetDatos, AnalyzerSettingsDS)
                If myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count > 0 Then
                    barcodeReagentDisabled = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                End If
            End If

            resultData = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString())
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                myAnalyzerSettingsDS = CType(resultData.SetDatos, AnalyzerSettingsDS)

                If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count > 0) Then
                    barcodeSampleDisabled = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                End If
            End If

            If Not barcodeReagentDisabled AndAlso Not barcodeSampleDisabled Then
                If watchDogTimer.Enabled Then
                    If Not copyRefreshDS Is Nothing Then
                        If copyRefreshDS.RotorPositionsChanged.Rows.Count > 0 Then
                            If copyRefreshDS.RotorPositionsChanged.First().RotorType = "SAMPLES" Then
                                watchDogTimer.Enabled = False
                                Debug.Print("******************************* WATCHDOG ENABLE CHANGED TO [FALSE]")
                            End If
                        End If
                    End If
                End If
            Else
                If watchDogTimer.Enabled Then watchDogTimer.Enabled = False
            End If
            ' XB 06/02/2014

            'AG 26/09/2011 - during the barcode read the current screen is disable (son ActiveMdiChild is nothing)
            Dim MdiChildIsDisabled As Boolean = False
            Dim refreshTriggeredFlag As Boolean = False 'AG 23/01/2012 - Indicates if the refresh method is performed or not
            If DisabledMDIChildAttribute.Count > 0 Then
                MdiChildIsDisabled = True
            End If

            Dim myCurrentMDIForm As System.Windows.Forms.Form = Nothing

            If Not ActiveMdiChild Is Nothing Then
                myCurrentMDIForm = ActiveMdiChild
            ElseIf MdiChildIsDisabled Then
                myCurrentMDIForm = DisabledMDIChildAttribute(0)
            End If
            'AG 26/09/2011

            'Call the RefreshScreen method when the current screen is:
            If Not myCurrentMDIForm Is Nothing Then
                '- Monitor (Reagents or Sample Rotor) screen ... (pRefreshDS.RotorPositionChanged contains the information to refresh)
                If (TypeOf myCurrentMDIForm Is IMonitor AndAlso Not monitorTreated) Then
                    Dim CurrentMdiChild As IMonitor = CType(myCurrentMDIForm, IMonitor)
                    If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                        monitorTreated = True
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                    End If
                    '- Rotor position screen ... (pRefreshDS.RotorPositionChanged contains the information to refresh)
                ElseIf (TypeOf myCurrentMDIForm Is IWSRotorPositions AndAlso Not wsRotorPositionTreated) Then
                    Dim CurrentMdiChild As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                    If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'DL 16/09/2011 CurrentMdiChild.RefreshScreen(pRefreshEvent, pRefreshDS)
                        wsRotorPositionTreated = True
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                    End If

                ElseIf (TypeOf myCurrentMDIForm Is IWSSampleRequest) Then
                    Dim CurrentMdiChild As IWSSampleRequest = CType(myCurrentMDIForm, IWSSampleRequest)
                    If (Not CurrentMdiChild Is Nothing) Then 'IT #1644
                        CurrentMdiChild.RefreshScreen(copyRefreshEventList, copyRefreshDS) 'AG + TR 23/09/2011
                        refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                    End If
                End If
            End If

            If String.Compare(processingBeforeRunning, "0", False) = 0 Then refreshTriggeredFlag = False 'AG 23/01/2012 - special case: enter running process in process
            If MdiChildIsDisabled AndAlso refreshTriggeredFlag Then
                'Activate the current mdi child who is disabled once the refresh method is finished
                If DisabledMDIChildAttribute.Count > 0 Then

                    'DL 19/01/2012 Only activate the monitor if not scanning
                    If String.Compare(processingBeforeRunning, "1", False) = 0 Then
                        DisabledMDIChildAttribute(0).Enabled = True

                        If DisabledMDIChildAttribute.Count = 1 Then
                            DisabledMDIChildAttribute.Clear()
                        Else
                            DisabledMDIChildAttribute.Remove(DisabledMDIChildAttribute(0))
                        End If
                    End If
                    'DL 19/01/2012

                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PerformNewWRotorPositionChange ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PerformNewWRotorPositionChange ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Scanning barcode finish and there are some warnings (tubes with no tests) to be shown using a popup screen
    ''' </summary>
    ''' <param name="barcode_Samples_Warnings"></param>
    ''' <remarks>AG 08/01/2014
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub PerformNewBarcodeWarnings(ByRef barcode_Samples_Warnings As Boolean)
        Try
            If watchDogTimer.Enabled Then watchDogTimer.Enabled = False 'AG 03/02/2014 - BT #1488 (disable watchdog when barcode warnings, ELSE STARTWS button is enabled before the process finishes)

            Dim MdiChildIsDisabled As Boolean = False
            Dim refreshTriggeredFlag As Boolean = False 'AG 23/01/2012 - Indicates if the refresh method is performed or not
            If DisabledMDIChildAttribute.Count > 0 Then
                MdiChildIsDisabled = True
            End If

            Dim myCurrentMDIForm As Form = Nothing

            If Not ActiveMdiChild Is Nothing Then
                myCurrentMDIForm = ActiveMdiChild
            ElseIf MdiChildIsDisabled Then
                myCurrentMDIForm = DisabledMDIChildAttribute(0)
            End If

            If Not myCurrentMDIForm Is Nothing Then
                processingBeforeRunning = "2" 'Start instrument failed because some barcode alerts must be solved

                'AG 03/04/2012 - In these case stop the mdi bar progress bar and enabled menus ... a new popup screen is opened
                ScreenWorkingProcess = False
                StopMarqueeProgressBar()
                EnableButtonAndMenus(True)
                ShowStatus(Messages.STANDBY)
                Cursor = Cursors.Default

                'Once treated reset flag
                AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS) = 0
                'AG 03/04/2012 

                Dim userAns As DialogResult = Windows.Forms.DialogResult.Yes 'By default show the incomplete samples screen

                'AG 07/01/2014 - BT #1436 protection, if recover results INPROCESS do not open the HQ barcode neither the incomplete samples screens
                '                If current screen is disable activate it!!
                If AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) = "INPROCESS" Then
                    barcode_Samples_Warnings = False
                    incompleteSamplesOpenFlag = False
                    'AG 07/01/2014

                Else 'Normal code (code before add condition for recovery results)
                    Dim myAction As BarcodeWorksessionActionsEnum = BarcodeWorksessionActionsEnum.NO_RUNNING_REQUEST
                    myAction = AnalyzerController.Instance.Analyzer.BarCodeProcessBeforeRunning

                    If myAction = BarcodeWorksessionActionsEnum.ENTER_RUNNING Then
                        'Ask question only in start ws process
                        'userAns = ShowMessage("Question", GlobalEnumerates.Messages.CONFIRM_BARCODE_WARNING.ToString)
                        'AG 09/07/2013 - In automate mode do not ask the user
                        'XB 23/07/2013 - auto HQ
                        'If Not autoWSCreationWithLISModeAttribute Then
                        If Not autoWSCreationWithLISModeAttribute And Not HQProcessByUserFlag Then
                            ' XB 06/11/2013 - Activate buzzer
                            If (AnalyzerController.IsAnalyzerInstantiated) Then
                                AnalyzerController.Instance.Analyzer.StartAnalyzerRinging()
                            End If

                            'XB 23/07/2013
                            userAns = ShowMessage("Question", GlobalEnumerates.Messages.CONFIRM_BARCODE_WARNING.ToString)

                            ' XB 06/11/2013 - Close buzzer
                            If (AnalyzerController.IsAnalyzerInstantiated) Then
                                AnalyzerController.Instance.Analyzer.StopAnalyzerRinging()
                            End If
                        End If
                        AnalyzerController.Instance.Analyzer.BarCodeProcessBeforeRunning = BarcodeWorksessionActionsEnum.BARCODE_AVAILABLE
                    End If

                    If (userAns = Windows.Forms.DialogResult.Yes) Then
                        EnableButtonAndMenus(True, True)   ' XB 07/11/2013

                        'AG 03/04/2013 - Open the incomplete samples or HostQuery screen depending this setting
                        Dim lisWithFilesMode As Boolean = False
                        Dim resultData As New GlobalDataTO
                        Dim userSettings As New UserSettingsDelegate
                        resultData = userSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WITHFILES_MODE.ToString)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            lisWithFilesMode = CType(resultData.SetDatos, Boolean)
                        End If

                        If lisWithFilesMode Then
                            'Open popup the IWSIncompleteSamplesAuxScreen 
                            Using MyForm As New IWSIncompleteSamplesAuxScreen()
                                MyForm.AnalyzerID = AnalyzerIDAttribute
                                MyForm.WorkSessionID = WorkSessionIDAttribute
                                MyForm.WorkSessionStatus = WSStatusAttribute
                                MyForm.SourceScreen = SourceScreen.START_BUTTON
                                'Validate if active screen is Sample request, 'cause requires the WorkSessionResultDS.
                                If TypeOf myCurrentMDIForm Is IWSSampleRequest Then
                                    MyForm.SourceScreen = SourceScreen.SAMPLE_REQUEST
                                    MyForm.WSOrderTests = CType(myCurrentMDIForm, IWSSampleRequest).myWorkSessionResultDS
                                End If

                                MyForm.ShowDialog()
                                incompleteSamplesOpenFlag = False

                                'TR 14/09/2011 -Set worksession status and set property ChangesMade when WS Samples Request is open
                                WSStatusAttribute = MyForm.WorkSessionStatus
                                If TypeOf myCurrentMDIForm Is IWSSampleRequest Then
                                    CType(myCurrentMDIForm, IWSSampleRequest).ChangesMade = MyForm.ChangesMade
                                End If
                            End Using

                        Else 'HostQuery screen
                            'Open popup the HostQuery screen 
                            Dim myOriginalStatus As String = bsAnalyzerStatus.Text 'DL 17/07/2013
                            'DL 18/07/2013
                            'bsAnalyzerStatus.Text = myAutoLISWaitingOrder
                            ShowStatus(Messages.AUTOLIS_WAITING_ORDERS)

                            Using MyForm As New HQBarcode
                                MyForm.AnalyzerID = AnalyzerIDAttribute
                                MyForm.WorkSessionID = WorkSessionIDAttribute
                                MyForm.WorkSessionStatus = WSStatusAttribute
                                MyForm.SourceScreen = SourceScreen.START_BUTTON
                                MyForm.applicationMaxMemoryUsage = myApplicationMaxMemoryUsage 'AG 24/02/2014 - #1520 inform new property
                                MyForm.SQLMaxMemoryUsage = mySQLMaxMemoryUsage 'AG 24/02/2014 - #1520 inform new property

                                'JC-SG 10/05/2013 Screen sometimes appear on bacground of MDIForm, and app looks like has hang out
                                '#If Not Debug Then
                                MyForm.TopMost = True
                                '#End If

                                'AG 08/07/2013 - Special mode for work with LIS with automatic actions
                                'XB 23/07/2013 - auto HQ
                                'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                    'XB 23/07/2013
                                    MyForm.OpenByAutomaticProcess = True
                                    'XB 23/07/2013 - auto HQ
                                    'MyForm.AutoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute   ' XB 17/07/2013 - Auto WS process
                                    MyForm.AutoWSCreationWithLISMode = autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag
                                    'XB 23/07/2013
                                    EnableLISWaitTimer(False)
                                    SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessAskBySpecimen)
                                End If
                                'AG 08/07/2013

                                AddNoMDIChildForm = MyForm 'Inform the MDI the HostQuery monitor screen is shown

                                MyForm.ShowDialog()
                                RemoveNoMDIChildForm = MyForm 'Inform the MDI the HostQuery monitor screen is closed
                                incompleteSamplesOpenFlag = False

                                'TR 14/09/2011 -Set worksession status and set property ChangesMade when WS Samples Request is open
                                WSStatusAttribute = MyForm.WorkSessionStatus
                            End Using

                            bsAnalyzerStatus.Text = myOriginalStatus 'DL 17/07/2013

                            'AG 08/07/2013 - Special mode for work with LIS with automatic actions
                            'XB 23/07/2013 - auto HQ
                            'If autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                            If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                                'XB 23/07/2013
                                CreateLogActivity("AutoCreate WS with LIS: HostQuery monitor screen closed", "IAx00MainMDI.ManageReceptionEvent", EventLogEntryType.Information, False)

                                'Check if there is something received from LIS pending to be add to WS (see conditions instead of button status)
                                'If bsTSOrdersDownloadButton.Enabled Then 'Exists lis workorders pending to be added
                                Dim myESRulesDlg As New ESBusiness
                                If myESRulesDlg.AllowLISAction(Nothing, LISActions.OrdersDownload_MDIButton, False, False, MDILISManager.Status, MDILISManager.Storage) Then
                                    'OK - normal case
                                    SetAutomateProcessStatusValue(LISautomateProcessSteps.subProcessDownloadOrders)

                                ElseIf automateProcessCurrentState = LISautomateProcessSteps.subProcessAskBySpecimen Then
                                    'Exception: No workorders received from LIS
                                    SetAutomateProcessStatusValue(LISautomateProcessSteps.ExitNoWorkOrders)
                                    InitializeAutoWSFlags()

                                ElseIf automateProcessCurrentState = LISautomateProcessSteps.ExitAutomaticProcesAndStop Then
                                    'Exception: Impossible ask for specimen and user decides stop
                                    EnableButtonAndMenus(True, True) 'Enable buttons before update attribute!!
                                    SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                                    InitializeAutoWSFlags()
                                End If
                            End If
                            'AG 08/07/2013

                        End If

                    Else 'Skip incomplete samples and continue enter Running
                        Cursor = Cursors.WaitCursor
                        SetActionButtonsEnableProperty(False)
                        EnableButtonAndMenus(False)
                        readBarcodeIfConfigured = False 'Enter running without barcode read neither warnings
                        StartEnterInRunningMode()

                        'AG 03/02/2014 - BT #1488 (in scenario barcode before running without LIS after leave PAUSE the menu remains disable)
                        incompleteSamplesOpenFlag = False
                        If AnalyzerController.Instance.Analyzer.AllowScanInRunning Then EnableButtonAndMenus(True)
                        'AG 03/02/2014
                    End If

                End If 'AG 07/01/2014

                'TR 14/09/2011 -Refresh screen
                If TypeOf myCurrentMDIForm Is IWSRotorPositions Then
                    Dim CurrentMdiChild As IWSRotorPositions = CType(myCurrentMDIForm, IWSRotorPositions)
                    CurrentMdiChild.ScreenWorkingProcess = False

                    'AG 08/01/2014 - BT #1436 Do not activate the screen buttons during recover results process
                    'CurrentMdiChild.RefreshAfterSamplesWithoutRequest(WSStatusAttribute)
                    If AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess) <> "INPROCESS" Then
                        CurrentMdiChild.RefreshAfterSamplesWithoutRequest(WSStatusAttribute)
                    End If

                    refreshTriggeredFlag = CurrentMdiChild.RefreshDone 'RH 28/03/2012
                ElseIf TypeOf myCurrentMDIForm Is IWSSampleRequest Then
                    'TR 16/09/2011
                    CType(myCurrentMDIForm, IWSSampleRequest).ScreenWorkingProcess = False
                    CType(myCurrentMDIForm, IWSSampleRequest).ActiveWSStatus = WSStatusAttribute
                    CType(myCurrentMDIForm, IWSSampleRequest).ActiveWorkSession = WorkSessionIDAttribute
                    CType(myCurrentMDIForm, IWSSampleRequest).ChangesMade = True
                    refreshTriggeredFlag = True
                    Cursor = Cursors.Default
                Else
                    'AG 11/03/2013 - Open rotor positions screen (only if the auxiliary screen for complete samples has been shown and closed)
                    'OpenRotorPositionsForm(myCurrentMDIForm) 'AG 03/04/2012 - Parameter is nothing using myCurrentMDIForm a system error is triggered (myCurrentMDIForm)
                    'XB 23/07/2013 - auto HQ
                    'If userAns = Windows.Forms.DialogResult.Yes AndAlso Not autoWSCreationWithLISModeAttribute AndAlso automateProcessCurrentState = LISautomateProcessSteps.notStarted Then
                    If userAns = Windows.Forms.DialogResult.Yes AndAlso (Not autoWSCreationWithLISModeAttribute And Not HQProcessByUserFlag) AndAlso automateProcessCurrentState = LISautomateProcessSteps.notStarted Then
                        'XB 23/07/2013
                        'AG 09/07/2013 - add condition not AUTO_WS_WITH_LIS_MODE
                        OpenRotorPositionsForm(myCurrentMDIForm)
                    End If
                End If
                'TR 14/09/2011 -END
            End If

            If String.Equals(processingBeforeRunning, "0") Then refreshTriggeredFlag = False 'AG 23/01/2012 - special case: enter running process in process
            If MdiChildIsDisabled AndAlso refreshTriggeredFlag Then
                'Activate the current mdi child who is disabled once the refresh method is finished
                If DisabledMDIChildAttribute.Count > 0 Then
                    DisabledMDIChildAttribute(0).Enabled = True
                    If DisabledMDIChildAttribute.Count = 1 Then
                        DisabledMDIChildAttribute.Clear()
                    Else
                        DisabledMDIChildAttribute.Remove(DisabledMDIChildAttribute(0))
                    End If
                End If
            End If
            'AG 05/09/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PerformNewBarcodeWarnings ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PerformNewBarcodeWarnings ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When some alarms appears the Sw show a warning message to the user  (only 1st time ... when the alarm appears)
    ''' When some sensors changes values Sw must show a warning message to the user (only 1st time ... when the sensors changes the value)
    ''' 
    ''' </summary>
    ''' <param name="pRefreshEvent">If alarms check alarms for show messages, if sensors check sensors for show messages</param>
    ''' <param name="pRefreshDS"></param>
    ''' <remarks>
    ''' AG 01/04/2011 - Creation
    ''' Modified by: AG 09/03/2012
    '''              IT 23/10/2014 - REFACTORING (BA-2016)
    '''              IT 01/12/2014 - BA-2075
    '''              IT 19/12/2014 - BA-2143
    ''' </remarks>
    Private Sub ShowAlarmsOrSensorsWarningMessages(ByVal pRefreshEvent As GlobalEnumerates.UI_RefreshEvents, ByVal pRefreshDS As UIRefreshDS)

        Try
            Dim myMessageIDList As New List(Of String)
            Dim messageType As String = Name

            'Messages when some new alarms appears
            If pRefreshEvent = UI_RefreshEvents.ALARMS_RECEIVED Then
                Dim linq As New List(Of UIRefreshDS.ReceivedAlarmsRow)

                '"Reactions Rotor must be changed"
                'Condition: Base line initializations fails
                linq = (From a As UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
                         Where (String.Equals(a.AlarmID, GlobalEnumerates.Alarms.BASELINE_INIT_ERR.ToString)) _
                           And a.AlarmStatus = True _
                        Select a).ToList

                If linq.Count > 0 Then
                    If AnalyzerController.Instance.Analyzer.ShowBaseLineInitializationFailedMessage Then
                        myMessageIDList.Add(GlobalEnumerates.Messages.CHANGE_REACTROTOR_REQUIRED.ToString)
                        messageType = "Error"
                    End If
                End If

                'AG 12/03/2012 - Reactions rotor missing messages
                linq = (From a As UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
                        Where (String.Equals(a.AlarmID, GlobalEnumerates.Alarms.REACT_MISSING_ERR.ToString.ToString)) _
                        And a.AlarmStatus = True _
                        Select a).ToList

                If linq.Count > 0 Then
                    myMessageIDList.Add(GlobalEnumerates.Messages.MISS_REACT_ROTOR.ToString) 'Reactions rotor not found
                    messageType = "Error"

                    'If current screen <> Change rotor add a new message line
                    Dim addMessageLine As Boolean = True
                    If Not ActiveMdiChild Is Nothing Then
                        If (TypeOf ActiveMdiChild Is IChangeRotor) Then addMessageLine = False
                    End If

                    If addMessageLine Then
                        myMessageIDList.Add("")
                        myMessageIDList.Add(GlobalEnumerates.Messages.MISS_REACT_ROTOR2.ToString) 'Execute the change rotor utility
                    End If

                End If
                'AG 12/03/2012

                'DL 31/07/2012 
                'FW_CPU_ERR: CPU Firmware Error
                'FW_DISTRIBUTED_ERR: Distributed Firmware Error
                'FW_REPOSITORY_ERR: Repository Firmware Error
                'FW_CHECKSUM_ERR: Checksum Firmware Error
                'FW_MAN_ERR: Maneuvers or Adjustments Firmware Error

                linq = (From a As UIRefreshDS.ReceivedAlarmsRow In pRefreshDS.ReceivedAlarms _
                        Where (String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_CPU_ERR.ToString) _
                        OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_DISTRIBUTED_ERR.ToString) _
                        OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_REPOSITORY_ERR.ToString) _
                        OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_CHECKSUM_ERR.ToString) _
                        OrElse String.Equals(a.AlarmID, GlobalEnumerates.Alarms.FW_MAN_ERR.ToString)) _
                           And a.AlarmStatus = True _
                        Select a).ToList

                If linq.Count > 0 Then
                    myMessageIDList.Add(GlobalEnumerates.Messages.FW_UPDATE.ToString) 'Reactions rotor not found
                    messageType = "Error"
                End If
                'DL 31/07/2012
                linq = Nothing

                'Messages when some sensors change value
            ElseIf pRefreshEvent = UI_RefreshEvents.SENSORVALUE_CHANGED Then

                'AG 07/03/2012 - Moved from ManageReceptionEvent
                Dim lnqRes As List(Of UIRefreshDS.SensorValueChangedRow)

                'AG 30/09/2011 - WarmUp canceled due bottle alarms
                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In pRefreshDS.SensorValueChanged _
                          Where String.Equals(a.SensorID, GlobalEnumerates.AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS.ToString) _
                          Select a).ToList

                If lnqRes.Count > 0 Then
                    If lnqRes(0).Value = 1 Then
                        'Message ) Wup aborted due bottle alars - "You must solve the level alarms in bottle and tank before continue"
                        'ShowAlarmWarningMessages(GlobalEnumerates.UI_RefreshEvents.SENSORVALUE_CHANGED, copyRefreshDS) 'DL 16/09/2011 ShowAlarmWarningMessages(pRefreshDS)
                        'BA-2075 & BA-2143
                        If String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing), "CANCELED") OrElse _
                           String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine), "CANCELED") OrElse _
                           String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Fill), "CANCELED") OrElse _
                           String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Empty), "CANCELED") OrElse _
                            String.Equals(AnalyzerController.Instance.Analyzer.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read), "CANCELED") Then
                            myMessageIDList.Add(GlobalEnumerates.Messages.NOT_LEVEL_AVAILABLE.ToString)
                            If Not String.Equals(messageType, "Error") Then messageType = "Warning"
                        End If

                    End If
                End If

                'AG 06/03/2012 - Freeze messages
                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In pRefreshDS.SensorValueChanged _
                          Where String.Equals(a.SensorID, GlobalEnumerates.AnalyzerSensors.FREEZE.ToString) _
                          Select a).ToList

                If lnqRes.Count > 0 Then
                    If lnqRes(0).Value = 1 Then
                        If String.Equals(AnalyzerController.Instance.Analyzer.AnalyzerFreezeMode, "TOTAL") Then
                            'Partial Freeze is form by 3 different messages
                            If AnalyzerController.Instance.Analyzer.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_AUTO.ToString)
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC0.ToString)
                            Else
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_TOTAL.ToString)
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC1.ToString)
                                WSStatusAttribute = "ABORTED" 'This is business ... maybe we can move outside the method by I leave here to do not duplicate the same condition
                                ElapsedTimeTimer.Stop()
                            End If

                            myMessageIDList.Add("")
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC2.ToString)
                            messageType = "Error"

                        ElseIf String.Compare(AnalyzerController.Instance.Analyzer.AnalyzerFreezeMode, "PARTIAL", False) = 0 Then
                            'Partial Freeze is form with 3 different messages
                            If AnalyzerController.Instance.Analyzer.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_AUTO.ToString)
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC0.ToString)
                            Else
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_PARTIAL.ToString)
                                myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC1.ToString)
                            End If

                            myMessageIDList.Add("")
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC2.ToString)
                            messageType = "Error"

                        ElseIf String.Compare(AnalyzerController.Instance.Analyzer.AnalyzerFreezeMode, "RESET", False) = 0 Then
                            'Reset Freeze is form with 1 different messages
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_RESET.ToString)
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC_RESET.ToString)
                            messageType = "Error"

                        ElseIf String.Compare(AnalyzerController.Instance.Analyzer.AnalyzerFreezeMode, "AUTO", False) = 0 Then
                            'Auto Freeze is form with 2 different messages
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_AUTO.ToString)
                            myMessageIDList.Add(GlobalEnumerates.Messages.FREEZE_GENERIC_AUTO.ToString)
                            messageType = "Warning"

                        End If
                    End If
                End If
                'AG 06/03/2012

                'AG 14/10/2011 - TimeOut Communication error message
                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In pRefreshDS.SensorValueChanged _
                          Where String.Equals(a.SensorID, GlobalEnumerates.AnalyzerSensors.CONNECTED.ToString) _
                          Select a).ToList

                If lnqRes.Count > 0 Then
                    If lnqRes(0).Value = 0 Then 'Note this sensor works different (1 connected (ok), 0 (no connected, connection fails, error))
                        If NotDisplayErrorCommsMsg Then ' XBC 19/06/2012
                            NotDisplayErrorCommsMsg = False ' XBC 19/06/2012
                        Else
                            myMessageIDList.Add(GlobalEnumerates.Messages.ERROR_COMM.ToString)
                            Debug.Print("Added ERROR_COMM to Message list in IAx00MainMDI.ShowAlarmsOrSensorsWarningMessages")
                        End If
                        messageType = "Error"
                    End If
                End If
                'AG 07/03/2012

                'AG 09/05/2012 - No Running instruction sent because no pending executions
                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In pRefreshDS.SensorValueChanged _
                         Where String.Compare(a.SensorID, GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS.ToString, False) = 0 _
                         Select a).ToList
                If lnqRes.Count > 0 Then
                    If lnqRes(0).Value = 2 Then
                        myMessageIDList.Add(GlobalEnumerates.Messages.NO_PENDING_EXECUTIONS.ToString)
                        messageType = "Warning"

                        'Once treated reset
                        AnalyzerController.Instance.Analyzer.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BARCODE_WARNINGS) = 0
                    End If

                End If
                'AG 09/05/2012

                lnqRes = Nothing

            End If

            'Finally show single or multiple message
            If myMessageIDList.Count = 1 Then
                ShowMessage(messageType, myMessageIDList(0))
            ElseIf myMessageIDList.Count > 1 Then
                ShowMultipleMessage(messageType, myMessageIDList)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ShowAlarmsOrSensorsWarningMessages ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & " ShowAlarmWarningMessages, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

#End Region


End Class

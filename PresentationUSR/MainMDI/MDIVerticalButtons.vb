Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.CommunicationsSwFw
'AG 25/02/2013 - for LIS communications
'AG 25/02/2013 - for LIS communications
'AG 25/02/2013 - for LIS communications (release MDILISManager object in MDI closing event)


'Refactoring code in VericalButtons partial class inherits form MDI (specially method ActivateActionButtonBarOrSendNewAction)
Partial Public Class UiAx00MainMDI

#Region "Action Buttons managenement business"

    ''' <summary>
    ''' Activates or not the vertical buttons (action buttons) depending the 
    ''' analyzer state (free or busy), the analyzer status, current alarms 
    ''' and so on
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 01/06/2010 (Tested)
    ''' Modified by: AG 16/11/2010 - define as public method to call it from WS Rotor Positions
    '''              XB 15/10/2013 - Implement mode when Analyzer allows Scan Rotors in RUNNING (PAUSE mode) - Change ENDprocess instead of PAUSEprocess - BT #1318
    '''              AG 06/11/2013 - Task #1375
    '''              AG 09/01/2014 - refactoring code in VerticalButtons partial class inherits form
    ''' </remarks>
    Private Sub ActivateActionButtonBarOrSendNewAction()
        Try
            Dim myGlobal As New GlobalDataTO
            If Not MDIAnalyzerManager Is Nothing Then
                If Not MDIAnalyzerManager.classInitializationError Then

                    '1st differentiate if connection established or not
                    Dim myConnected As Boolean = False
                    If MDIAnalyzerManager.CommThreadsStarted Then myConnected = MDIAnalyzerManager.Connected

                    'Me.SetActionButtonsEnableProperty(False)    'All buttons are already disable. We only need to active the required ones

                    If Not myConnected Then 'Only button Connect enabled
                        SetActionButtonsEnableProperty(False) 'AG 09/05/2012 - Disable all buttons
                        bsTSConnectButton.Enabled = True 'But not the Connect button

                        'ShowStatus(Messages.NO_CONNECTED)
                        If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "INPROCESS", False) = 0 Then
                            ShowStatus(Messages.CONNECTING)
                        Else
                            ShowStatus(Messages.NO_CONNECTED)
                        End If

                        SetEnableMainTab(False) 'RH 27/03/2012

                    Else
                        'Activate or deactivate buttons depending the analyzer status and if it's ready

                        ' AG+XBC 24/05/2012
                        'Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING
                        Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.NONE
                        ' AG+XBC 24/05/2012

                        Dim myAx00Ready As Boolean = True
                        Dim myAx00Action As GlobalEnumerates.AnalyzerManagerAx00Actions
                        Dim myAx00InstructionReceived As GlobalEnumerates.AnalyzerManagerSwActionList
                        'Dim myAx00CurrentAlarms As New List(Of GlobalEnumerates.Alarms) 'AG 01/12/11

                        myAx00Status = MDIAnalyzerManager.AnalyzerStatus
                        myAx00Ready = MDIAnalyzerManager.AnalyzerIsReady
                        myAx00Action = MDIAnalyzerManager.AnalyzerCurrentAction
                        myAx00InstructionReceived = MDIAnalyzerManager.InstructionTypeReceived
                        'myAx00CurrentAlarms = MDIAnalyzerManager.Alarms 'AG 01/12/11

                        'AG 31/03/2011 - If analyzer not is in FREEZE mode take care about status, alarms, user actions,...
                        If Not MDIAnalyzerManager.AnalyzerIsFreeze Then
                            bsTSConnectButton.Enabled = False 'AG 12/03/2012
                            bsTSRecover.Enabled = False 'AG 12/03/2012

                            'AG 20/06/2012 - If analyzer is in STANDBY  but there are no alight results and start instrument is NOT IN PROCESS -> activate button Start instrument 
                            If myAx00Status = AnalyzerManagerStatus.STANDBY AndAlso Not MDIAnalyzerManager.ExistsALIGHT AndAlso _
                            (String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "INPROCESS", False) <> 0 AndAlso String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess), "PAUSED", False) <> 0) Then
                                'Means we are in Standby from Service Sw or we have restore database and connected with an analyzer already started
                                'Solution: Repeat the Start Instrument Process
                                ApplyRulesStandByWithOutALIGHT(myAx00Ready)

                            Else 'Apply the normal business: Select cases

                                Select Case myAx00Status
                                    Case GlobalEnumerates.AnalyzerManagerStatus.MONITOR 'All buttons disable
                                        'TR 15/05/2012 -Stop the worksession timer
                                        If ElapsedTimeTimer.Enabled Then ElapsedTimeTimer.Stop()
                                        'Already done (all disable)

                                    Case GlobalEnumerates.AnalyzerManagerStatus.SLEEPING  'Only Start instrument button enabled if analyzer is ready 
                                        'TR 15/05/2012 -Stop the worksession timer
                                        If ElapsedTimeTimer.Enabled Then ElapsedTimeTimer.Stop()

                                        'Instrument buttons
                                        If myAx00Ready Then
                                            If Not MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then ' XBC 06/09/2012 - Fix : button is enabled while Connect process is in in process
                                                bsTSStartInstrumentButton.Enabled = True
                                                bsTSStartInstrumentButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_INSTRUMENT) 'AG 12/03/2012
                                            End If
                                            WarmUpFinishedAttribute = False
                                        End If

                                    Case GlobalEnumerates.AnalyzerManagerStatus.STANDBY
                                        ApplyRulesForStandBy(myAx00Ready, myAx00Action, myAx00Status)


                                    Case GlobalEnumerates.AnalyzerManagerStatus.RUNNING    'Only Wash, Pause and Abort buttons enabled
                                        ApplyRulesForRunning(myAx00Action)

                                    Case Else
                                End Select
                                'JV + AG revision 18/10/2013 task # 1341
                                ChangeStatusImageMultipleSessionButton()
                                'JV + AG revision 18/10/2013 task # 1341
                            End If 'AG 20/06/2012

                        Else
                            ApplyRulesForFreeze(myAx00Status)

                        End If

                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ActivateActionButtonBarOrSendNewAction ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & " ActivateActionButtonBarOrSendNewAction, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try


    End Sub


    ''' <summary>
    ''' Activate buttons depending the current alarms
    ''' 
    ''' NOTE: Sw has to check if the cover are disable before consider this cover alarm treatment
    ''' </summary>
    ''' <param name="pButton"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  AG
    ''' Modified by: AG 28/03/2012 - Define as public and add actions buttons in other screens (utilities, bar code read, ...)
    '''              AG 14/05/2012 - When ISE utilities screen is active the only vertical buttons that can be enabled are: ChangeBottle confirm, Recover and Sound Off
    '''              SA 15/10/2013 - BG #1334 ==> Changes in the availability of BARCODE Button when Analyzer Status is RUNNING: 
    '''                                            ** If it is not in PAUSE Mode: Barcode Button is disabled
    '''                                            ** If it is in PAUSE Mode but the Analyzer is FREEZE due to an Alarm: Barcode Button is disabled
    '''                                            ** Otherwise: Barcode Button is enabled  
    '''              AG 06/11/2013 - Task #1375 (take into account the start/continue WS is also allowed in running paused mode)
    ''' </remarks>
    Public Function ActivateButtonWithAlarms(ByVal pButton As GlobalEnumerates.ActionButton) As Boolean
        Dim myStatus As Boolean = True
        Try
            Dim resultData As New GlobalDataTO
            Dim myAlarms As List(Of GlobalEnumerates.Alarms) 'AG 21/01/2014 - Do not use New in definition New List(Of GlobalEnumerates.Alarms)

            ' AG+XBC 24/05/2012
            'Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING
            Dim myAx00Status As GlobalEnumerates.AnalyzerManagerStatus = GlobalEnumerates.AnalyzerManagerStatus.NONE
            ' AG+XBC 24/05/2012

            myAlarms = MDIAnalyzerManager.Alarms
            myAx00Status = MDIAnalyzerManager.AnalyzerStatus

            'AG 25/10/2011 - Before treat the cover alarms read if they are deactivated (0 disabled, 1 enabled)
            Dim mainCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.MAIN_COVER_WARN), 1, 0), Boolean)
            Dim reactionsCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.REACT_COVER_WARN), 1, 0), Boolean)
            Dim fridgeCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.FRIDGE_COVER_WARN), 1, 0), Boolean)
            Dim samplesCoverAlarm As Boolean = CType(IIf(myAlarms.Contains(GlobalEnumerates.Alarms.S_COVER_WARN), 1, 0), Boolean)

            resultData = MDIAnalyzerManager.ReadFwAdjustmentsDS
            If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                'Dim myAdjDS As SRVAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS) 'Causes system error in develop mode
                Dim myAdjDS As New SRVAdjustmentsDS
                myAdjDS = CType(resultData.SetDatos, SRVAdjustmentsDS)

                Dim linqRes As List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                'Main cover disabled (0 disabled, 1 enabled)
                linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                           Where String.Equals(a.CodeFw, GlobalEnumerates.Ax00Adjustsments.MCOV.ToString) _
                           Select a).ToList
                If linqRes.Count > 0 AndAlso Not String.Equals(linqRes(0).Value, String.Empty) Then
                    If Not CType(linqRes(0).Value, Boolean) Then mainCoverAlarm = False
                End If
                'Reactions cover disabled (0 disabled, 1 enabled)
                linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                           Where String.Equals(a.CodeFw, GlobalEnumerates.Ax00Adjustsments.PHCOV.ToString) _
                           Select a).ToList
                If linqRes.Count > 0 AndAlso Not String.Equals(linqRes(0).Value, String.Empty) Then
                    If Not CType(linqRes(0).Value, Boolean) Then reactionsCoverAlarm = False
                End If
                'Reagents cover disabled (0 disabled, 1 enabled)
                linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                           Where String.Equals(a.CodeFw, GlobalEnumerates.Ax00Adjustsments.RCOV.ToString) _
                           Select a).ToList
                If linqRes.Count > 0 AndAlso Not String.Equals(linqRes(0).Value, String.Empty) Then
                    If Not CType(linqRes(0).Value, Boolean) Then fridgeCoverAlarm = False
                End If
                'Samples cover disabled (0 disabled, 1 enabled)
                linqRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                           Where String.Equals(a.CodeFw, GlobalEnumerates.Ax00Adjustsments.SCOV.ToString) _
                           Select a).ToList
                If linqRes.Count > 0 AndAlso Not String.Equals(linqRes(0).Value, String.Empty) Then
                    If Not CType(linqRes(0).Value, Boolean) Then samplesCoverAlarm = False
                End If
                linqRes = Nothing
            End If
            'AG 25/10/2011

            'AG 02/04/2012
            '(ISE instaled and (initiated or not SwitchedON)) Or not instaled
            Dim iseInitiatedFinishedFlag As Boolean = True
            If MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso _
               (MDIAnalyzerManager.ISE_Manager.IsISEInitiating OrElse Not MDIAnalyzerManager.ISE_Manager.ConnectionTasksCanContinue) Then
                iseInitiatedFinishedFlag = False
            End If
            'AG 02/04/2012

            'AG 14/05/2012 - When ISE utilities screen is active the only vertical buttons that can be enabled are: ChangeBottle confirm, Recover & Sound Off
            Dim iSEUtilitiesActiveFlag As Boolean = False
            If Not ActiveMdiChild Is Nothing AndAlso TypeOf ActiveMdiChild Is IISEUtilities Then
                iSEUtilitiesActiveFlag = True
            End If
            'AG 14/05/2012

            Select Case pButton
                'AG 14/09/2011 - Use the adjustment for treat or not the covers alarms (tested ok 26/10/2011 AG with cover detection disabled)
                'START INSTRUMENT button
                Case ActionButton.START_INSTRUMENT
                    If myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) OrElse _
                       mainCoverAlarm OrElse reactionsCoverAlarm OrElse fridgeCoverAlarm OrElse samplesCoverAlarm OrElse iSEUtilitiesActiveFlag Then
                        myStatus = False
                    Else
                        myStatus = True
                    End If

                    'SHUT DOWN INSTRUMENT button
                Case ActionButton.SHUT_DOWN
                    If myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) OrElse _
                       mainCoverAlarm OrElse reactionsCoverAlarm OrElse fridgeCoverAlarm OrElse samplesCoverAlarm OrElse iSEUtilitiesActiveFlag Then
                        myStatus = False
                    Else
                        If StartSessionisPending Or StartingSession Then  ' XB 30/10/2013 
                            myStatus = False
                        End If
                    End If

                    'START running button
                Case GlobalEnumerates.ActionButton.START_WS
                    'AG 06/11/2013 - Task #1375
                    'If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                    'JV 23/01/2014 #1467
                    'If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY OrElse (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso MDIAnalyzerManager.AllowScanInRunning) Then
                    '    'AG 06/11/2013 - Task #1375

                    '    If mainCoverAlarm OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) OrElse _
                    '        reactionsCoverAlarm OrElse _
                    '        fridgeCoverAlarm OrElse _
                    '        samplesCoverAlarm OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.REACT_ENCODER_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.R1_DETECT_SYSTEM_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.R2_DETECT_SYSTEM_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.S_DETECT_SYSTEM_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.WS_COLLISION_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse _
                    '        myAlarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) OrElse _
                    '        Not MDIAnalyzerManager.ValidALIGHT OrElse iSEUtilitiesActiveFlag Then

                    '        myStatus = False
                    '    End If
                    'Else
                    '    myStatus = False
                    'End If
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        'AG 06/11/2013 - Task #1375

                        If mainCoverAlarm OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) OrElse _
                            reactionsCoverAlarm OrElse _
                            fridgeCoverAlarm OrElse _
                            samplesCoverAlarm OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.REACT_ENCODER_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.R1_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.R2_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.S_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WS_COLLISION_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) OrElse _
                            Not MDIAnalyzerManager.ValidALIGHT OrElse iSEUtilitiesActiveFlag Then

                            myStatus = False
                        End If
                    ElseIf (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso MDIAnalyzerManager.AllowScanInRunning) Then
                        If mainCoverAlarm OrElse _
                           reactionsCoverAlarm OrElse _
                           fridgeCoverAlarm OrElse _
                           samplesCoverAlarm Then 'OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.REACT_ENCODER_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.R1_DETECT_SYSTEM_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.R2_DETECT_SYSTEM_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.S_DETECT_SYSTEM_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WS_COLLISION_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) OrElse _
                            'Not MDIAnalyzerManager.ValidALIGHT OrElse iSEUtilitiesActiveFlag Then

                            myStatus = False
                        End If
                    Else
                        myStatus = False
                    End If
                    'JV 23/01/2014 #1467

                    'CONTINUE running button
                Case GlobalEnumerates.ActionButton.CONTINUE_WS
                    'AG 06/11/2013 - Task #1375
                    'If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                    'JV 23/01/2014 #1467
                    'If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY OrElse (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso MDIAnalyzerManager.AllowScanInRunning) Then
                    ''AG 06/11/2013 - Task #1375
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        If mainCoverAlarm OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) OrElse _
                            reactionsCoverAlarm OrElse _
                            fridgeCoverAlarm OrElse _
                            samplesCoverAlarm OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.REACT_ENCODER_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.R1_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.R2_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.S_DETECT_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WS_COLLISION_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse _
                            myAlarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) OrElse _
                            Not MDIAnalyzerManager.ValidALIGHT OrElse iSEUtilitiesActiveFlag Then

                            myStatus = False
                        End If

                    ElseIf (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso MDIAnalyzerManager.AllowScanInRunning) Then
                        If mainCoverAlarm OrElse _
                            reactionsCoverAlarm OrElse _
                            fridgeCoverAlarm OrElse _
                            samplesCoverAlarm Then
                            'myAlarms.Contains(GlobalEnumerates.Alarms.REACT_ENCODER_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.R1_DETECT_SYSTEM_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.R2_DETECT_SYSTEM_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.S_DETECT_SYSTEM_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WS_COLLISION_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WATER_SYSTEM_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_SYSTEM_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WATER_DEPOSIT_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WASTE_DEPOSIT_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse _
                            'myAlarms.Contains(GlobalEnumerates.Alarms.BASELINE_INIT_ERR) OrElse _
                            'Not MDIAnalyzerManager.ValidALIGHT OrElse iSEUtilitiesActiveFlag Then

                            myStatus = False
                        End If

                    Else
                        myStatus = False
                    End If
                    'JV 23/01/2014 #1467

                    'CHANGE BOTTLE CONFIRMATION
                Case ActionButton.CHANGE_BOTTLES_CONFIRM
                    myStatus = False
                    'JV 23/01/2014 #1467
                    'If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY OrElse (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.RUNNING AndAlso MDIAnalyzerManager.AllowScanInRunning) Then
                        'JV 23/01/2014 #1467
                        'Depending the analyzer alarms activate or not the button (myStatus = True or False)
                        'If myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_ERR) OrElse myAlarms.Contains(GlobalEnumerates.Alarms.WASH_CONTAINER_WARN) OrElse _
                        '   myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_ERR) OrElse myAlarms.Contains(GlobalEnumerates.Alarms.HIGH_CONTAMIN_WARN) Then
                        If MDIAnalyzerManager.ExistBottleAlarms Then
                            myStatus = True

                        End If
                    End If

                    'AG 28/03/2012 - READ BARCODE FROM SAMPLE REQUEST OR ROTOR POSITIONING SCREENS
                Case ActionButton.READ_BARCODE
                    If (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY) Then
                        If (mainCoverAlarm OrElse reactionsCoverAlarm OrElse fridgeCoverAlarm OrElse _
                           samplesCoverAlarm OrElse Not iseInitiatedFinishedFlag) Then
                            myStatus = False
                        End If

                        'SA 15/10/2013 - ADDED NEW CASES FOR BUTTON AVAILABILITY IN RUNNING 
                    ElseIf (myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.RUNNING) Then
                        If (Not MDIAnalyzerManager.AllowScanInRunning) Then
                            myStatus = False
                        Else
                            'If Analyzer is in PAUSE during RUNNING, Barcode is available excepting when the Analyzer is FREEZE 
                            myStatus = (Not MDIAnalyzerManager.AnalyzerIsFreeze)
                        End If
                    Else
                        myStatus = False
                    End If

                    'CHECK BOTTLE VOLUMES FROM ROTOR POSITIONING SCREEN
                Case ActionButton.CHECK_BOTTLE_VOLUME
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        If mainCoverAlarm OrElse _
                           reactionsCoverAlarm OrElse _
                           fridgeCoverAlarm OrElse _
                           samplesCoverAlarm OrElse Not iseInitiatedFinishedFlag Then

                            myStatus = False
                        End If
                    Else
                        myStatus = False
                    End If

                    'RAISE WASHING STATION FROM CHANGE REACTIONS ROTOR UTILITY SCREEN
                Case ActionButton.RAISE_WASH_STATION
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        'AG 03/05/2012 - do not allowed when ise not initiated and reactions rotor is found
                        'If Not iseInitiatedFinishedFlag Then
                        If Not iseInitiatedFinishedFlag AndAlso Not myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR) Then
                            myStatus = False
                        End If

                    Else
                        myStatus = False
                    End If

                    'CHANGE REACTIONS ROTOR FROM CHANGE REACTIONS ROTOR UTILITY SCREEN
                Case ActionButton.CHANGE_REACTIONS_ROTOR
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        'AG 03/05/2012 - do not allowed when reactions rotor cover error or (ise not initiated and reactions rotor is found)
                        'If reactionsCoverAlarm OrElse Not iseInitiatedFinishedFlag Then
                        If reactionsCoverAlarm OrElse (Not iseInitiatedFinishedFlag AndAlso Not myAlarms.Contains(GlobalEnumerates.Alarms.REACT_MISSING_ERR)) Then
                            myStatus = False
                        End If
                    Else
                        myStatus = False
                    End If

                    'ISE COMMAND FROM ISE UTILITIES SCREEN
                Case ActionButton.ISE_COMMAND
                    If myAx00Status = GlobalEnumerates.AnalyzerManagerStatus.STANDBY Then
                        If mainCoverAlarm OrElse _
                           reactionsCoverAlarm OrElse _
                           fridgeCoverAlarm OrElse _
                           samplesCoverAlarm OrElse Not iseInitiatedFinishedFlag Then

                            myStatus = False
                        End If
                    Else
                        myStatus = False
                    End If
                    'AG 28/03/2012

            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ActivateButtonWithAlarms ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & " ActivateButtonWithAlarms, ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myStatus
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pForceActionIcon"></param>
    ''' <remarks>
    ''' Created by  JV + AG revision 18/10/2013 New Button Play/Pause/Continue image and visibility task # 1341
    ''' Modified by AG 30/10/2013 - in some cases we need set a specific icon without buttons activation (for example ResetWS) so use the optional parameter
    '''             XB 30/10/2013 - Pause button must disable when user press pause button while Auto WS process or auto ISE maintenance is processing
    ''' Modified by AG 05/11/2013 - modified during validation of task #1375
    ''' </remarks>
    Private Sub ChangeStatusImageMultipleSessionButton(Optional ByVal pForceActionIcon As WorkSessionUserActions = WorkSessionUserActions.NO_WS)
        Try
            multisessionButtonEnableValue = (showPAUSEWSiconFlag Or showSTARTWSiconFlag)
            If multisessionButtonEnableValue Then
                'Change image
                If showPAUSEWSiconFlag Then
                    bsTSMultiFunctionSessionButton.Image = imagePause
                    bsTSMultiFunctionSessionButton.ToolTipText = tooltipPause
                Else
                    bsTSMultiFunctionSessionButton.Image = imagePlay
                    bsTSMultiFunctionSessionButton.ToolTipText = tooltipPlay
                End If

                'Set enabled true
                'AG 05/11/2013 - Task #1375 - change business and take into account the automatic WS creation with STARTWS icon. Change 'XB 30/10/2013' failed
                'If Not pausingAutomateProcessFlag And Not StartSessionisPending AndAlso Not StartingSession Then  ' XB 30/10/2013
                Dim myLocalStartingSession As Boolean = StartingSession

                'In automatic creation WS process (StandBy) the pause button must be enabled except just before go to running
                'In automatic creation WS process (running paused) the pause button must be enabled except just before leave the paused mode
                If autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag Then
                    myLocalStartingSession = False
                    If automateProcessCurrentState = LISautomateProcessSteps.subProcessEnterRunning Then myLocalStartingSession = True
                End If
                If Not pausingAutomateProcessFlag And Not StartSessionisPending AndAlso Not myLocalStartingSession Then
                    'AG 05/11/2013

                    If Not MDIAnalyzerManager Is Nothing AndAlso _
                       MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess) = "INPROCESS" Then ' XB 07/11/2013
                        ' Nothing to do 
                    Else
                        bsTSMultiFunctionSessionButton.Enabled = True
                    End If
                Else
                    bsTSMultiFunctionSessionButton.Enabled = False
                End If
            Else
                'No change image. Only Set enabled false
                bsTSMultiFunctionSessionButton.Enabled = False

                'AG 22/10/2013 - Exception, change icon when
                If MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY AndAlso automateProcessCurrentState = LISautomateProcessSteps.notStarted Then
                    'STARTWS icon
                    bsTSMultiFunctionSessionButton.Image = imagePlay
                    bsTSMultiFunctionSessionButton.ToolTipText = tooltipPlay
                    'AG 30/10/2013
                ElseIf pForceActionIcon = WorkSessionUserActions.START_WS Then
                    bsTSMultiFunctionSessionButton.Image = imagePlay
                    bsTSMultiFunctionSessionButton.ToolTipText = tooltipPlay

                ElseIf pForceActionIcon = WorkSessionUserActions.PAUSE_WS Then
                    bsTSMultiFunctionSessionButton.Image = imagePause
                    bsTSMultiFunctionSessionButton.ToolTipText = tooltipPause
                    'AG 30/10/2013
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ChangeStatusImageMultipleSessionButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ChangeStatusImageMultipleSessionButton ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

#Region "Refactoring ActivateActionButtonBarOrSendNewAction submethods"

    ''' <summary>
    ''' Means we are in Standby from Service Sw or we have restore database and connected with an analyzer already started
    ''' Solution: Repeat the Start Instrument Process
    ''' </summary>
    ''' <param name="myAx00Ready"></param>
    ''' <remarks>AG 09/01/2014 - refactoring</remarks>
    Private Sub ApplyRulesStandByWithOutALIGHT(ByVal myAx00Ready As Boolean)
        Try

            'Active only the start instrument button
            SetActionButtonsEnableProperty(False)

            If myAx00Ready Then
                ' XBC 04/07/2012
                If Not MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess) = "INPROCESS" Then
                    If Not MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "INPROCESS" Then
                        If MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso Not MDIAnalyzerManager.ISE_Manager.IsISEInitiating Then
                            If Not isStartInstrumentActivating Then

                                bsTSStartInstrumentButton.Enabled = True
                                bsTSStartInstrumentButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_INSTRUMENT) 'AG 12/03/2012
                                WarmUpFinishedAttribute = False

                                ' XBC 04/07/2012
                                If Not ActiveMdiChild Is Nothing Then
                                    If (TypeOf ActiveMdiChild Is UiMonitor) Then
                                        Dim CurrentMdiChild As UiMonitor = CType(ActiveMdiChild, UiMonitor)
                                        CurrentMdiChild.TimeWarmUpProgressBar.Position = 0
                                        ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                                        ErrorStatusLabel.Text = GetMessageText(GlobalEnumerates.Messages.ALIGHT_REQUIRED.ToString)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If


            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ApplyRulesStandByWithOutALIGHT ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ApplyRulesStandByWithOutALIGHT ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' StandBy (normal case) activations action buttons rules
    ''' </summary>
    ''' <param name="myAx00Ready"></param>
    ''' <param name="myAx00Action"></param>
    ''' <param name="myAx00Status"></param>
    ''' <remarks>AG 09/01/2013 - refactoring</remarks>
    Private Sub ApplyRulesForStandBy(ByVal myAx00Ready As Boolean, ByVal myAx00Action As GlobalEnumerates.AnalyzerManagerAx00Actions, ByVal myAx00Status As GlobalEnumerates.AnalyzerManagerStatus)
        Try
            Dim myGlobal As New GlobalDataTO

            'TR 15/05/2012 -Stop the worksession timer
            'If ElapsedTimeTimer.Enabled Then ElapsedTimeTimer.Stop() TR 18/052012 commented

            'AG 23/03/2012 - ISE initialitation not finished (connection in StandBy) 
            If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "INPROCESS", False) = 0 Then
                'Do nothing

                'AG 04/09/2012 - ABORT session button:
                'Recovery results starts in Running and allow Abort but it must be disabled once the analyzer becomes in StandBy
                'Note that recovery results will be closed once the complete connection finishes
                If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess), "INPROCESS", False) = 0 AndAlso bsTSAbortSessionButton.Enabled Then
                    bsTSAbortSessionButton.Enabled = False
                End If
                'AG 04/09/2012

            ElseIf MDIAnalyzerManager.ISE_Manager IsNot Nothing AndAlso MDIAnalyzerManager.ISE_Manager.IsISEInitiating Then
                '    'Do nothing, no activate buttons until the ISE initialitazion finishes

            ElseIf String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess), "INPROCESS", False) = 0 Then
                'AG 27/08/2012 - STANDBY: All buttons Disabled | RUNNING: Abort button enabled
                SetActionButtonsEnableProperty(False)

            Else

                'Recover wash NOT in process, and analyzer is in active mode
                'If Abort NOT in process
                If MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) <> "INPROCESS" AndAlso _
                   MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess) <> "PAUSED" AndAlso _
                   MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) <> "INPROCESS" AndAlso _
                   MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) <> "PAUSED" Then

                    'AG 30/09/2011 - check if warm up has been canceled ... then activate start instrument and shut down
                    Dim warmUpPaused As Boolean = False
                    Dim abortPaused As Boolean = False
                    Dim shutDownPaused As Boolean = False
                    If MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "PAUSED" AndAlso _
                       (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing) = "CANCELED" OrElse _
                       String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BaseLine), "CANCELED", False) = 0) Then
                        warmUpPaused = True
                    ElseIf String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "PAUSED", False) = 0 AndAlso _
                       String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.Washing), "CANCELED", False) = 0 Then
                        abortPaused = True
                    ElseIf String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.SDOWNprocess), "PAUSED", False) = 0 Then
                        shutDownPaused = True
                    End If
                    'AG 30/09/2011

                    'AG 12/09/2011 - check if instrument has finished his warm up process
                    If Not WarmUpFinishedAttribute AndAlso Not warmUpPaused Then
                        'Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
                        Dim myAnalyzerSettingsDS As AnalyzerSettingsDS
                        'Dim myAnalyzerSettingsDelegate As New AnalyzerSettingsDelegate
                        myGlobal = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, _
                                               AnalyzerIDAttribute, _
                                               GlobalEnumerates.AnalyzerSettingsEnum.WUPSTARTDATETIME.ToString())

                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            myAnalyzerSettingsDS = DirectCast(myGlobal.SetDatos, AnalyzerSettingsDS)

                            If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                                Dim myDate As String = ""
                                myDate = myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue
                                If String.Compare(myDate, "", False) = 0 Then
                                    WarmUpFinishedAttribute = True

                                    'AG 22/03/2012
                                    If MDIAnalyzerManager.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 0 Then
                                        MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 1 'set sensor to 1
                                        MyClass.MDIAnalyzerManager.ISE_Manager.IsAnalyzerWarmUp = False 'AG 22/05/2012 - Ise alarms ready to be shown
                                    End If
                                    'AG 22/03/2012

                                Else
                                    If Not BsTimerWUp.Enabled Then BsTimerWUp.Enabled = True
                                End If
                            End If
                        End If

                        'AG 14/03/2012- If not finished, not paused, not in process and analyzer ready maybe it has been aborted by freeze
                        If Not WarmUpFinished AndAlso Not warmUpPaused AndAlso myAx00Ready AndAlso _
                           MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.WUPprocess) = "" Then
                            myGlobal = myAnalyzerSettingsDelegate.GetAnalyzerSetting(Nothing, _
                                       AnalyzerIDAttribute, _
                                       GlobalEnumerates.AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString())

                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                myAnalyzerSettingsDS = DirectCast(myGlobal.SetDatos, AnalyzerSettingsDS)

                                If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                                    If myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue = "0" Then
                                        warmUpPaused = True
                                    End If
                                End If
                            End If

                        End If
                        'AG 14/03/2012

                    End If

                    'If myAx00Ready Then
                    Dim enableChangeBottlesConfirmFlag As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHANGE_BOTTLES_CONFIRM)
                    If myAx00Ready AndAlso warmUpPaused Then
                        bsTSShutdownButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.SHUT_DOWN) 'AG 14/05/2012 True 'AG - shut down with out washings
                        WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012
                        bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag

                        'AG 12/03/2012
                        'bsTSStartInstrumentButton.Enabled = Not bsTSChangeBottlesConfirm.Enabled
                        If Not bsTSChangeBottlesConfirm.Enabled Then
                            bsTSStartInstrumentButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_INSTRUMENT)
                        Else
                            bsTSStartInstrumentButton.Enabled = False
                        End If
                        'AG 12/03/2012

                        WarmUpFinishedAttribute = False

                    ElseIf myAx00Ready AndAlso abortPaused Then
                        bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag

                        'AG 12/03/2012 - 'When abort paused user can not shut down because there is a conditioning wash pending
                        'bsTSShutdownButton.Enabled = Not bsTSChangeBottlesConfirm.Enabled
                        If Not bsTSChangeBottlesConfirm.Enabled Then
                            bsTSShutdownButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.SHUT_DOWN)
                        Else
                            bsTSShutdownButton.Enabled = False
                        End If
                        'AG 12/03/2012
                        WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012


                    ElseIf myAx00Ready AndAlso shutDownPaused Then
                        bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag

                        'AG 12/03/2012
                        'bsTSShutdownButton.Enabled = Not bsTSChangeBottlesConfirm.Enabled
                        If Not bsTSChangeBottlesConfirm.Enabled Then
                            bsTSShutdownButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.SHUT_DOWN)
                        Else
                            bsTSShutdownButton.Enabled = False
                        End If

                        WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012
                        'AG 12/03/2012


                    ElseIf myAx00Ready AndAlso WarmUpFinishedAttribute AndAlso Not warmUpPaused Then

                        'XB 23/07/2013 - auto HQ
                        'If autoWSCreationWithLISModeAttribute AndAlso (automateProcessCurrentState <> LISautomateProcessSteps.notStarted _
                        '                                               AndAlso automateProcessCurrentState <> LISautomateProcessSteps.ExitAutomaticProcesAndStop) Then
                        If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) _
                                        AndAlso (automateProcessCurrentState <> LISautomateProcessSteps.notStarted _
                                        AndAlso automateProcessCurrentState <> LISautomateProcessSteps.ExitAutomaticProcesAndStop) Then
                            'XB 23/07/2013

                            'Automate process in course (PAUSE enabled)
                            'bsTSStartSessionButton.Enabled = False

                            'JV + AG 18/10/2013 revision task # 1341
                            'bsTSMultiFunctionSessionButton.Image = imagePause
                            showSTARTWSiconFlag = False
                            'JV + AG 18/10/2013 revision task # 1341

                            'AG 19/07/2013 v2.1.1- when enter in running the pause button must remain disabled until BA400 becomes in Running or until the process fails
                            '                else evaluate same condition as previously
                            'If Not pausingAutomateProcessFlag Then bsTSPauseSessionButton.Enabled = True Else bsTSPauseSessionButton.Enabled = False
                            If automateProcessCurrentState <> LISautomateProcessSteps.subProcessEnterRunning Then
                                'AG 30/07/2013 - During the process automatic for HQ the pause button is disabled
                                'If Not pausingAutomateProcessFlag Then bsTSPauseSessionButton.Enabled = True Else bsTSPauseSessionButton.Enabled = False
                                'If Not pausingAutomateProcessFlag AndAlso Not HQProcessByUserFlag Then bsTSPauseSessionButton.Enabled = True Else bsTSPauseSessionButton.Enabled = False
                                If Not pausingAutomateProcessFlag AndAlso Not HQProcessByUserFlag Then showPAUSEWSiconFlag = True Else showPAUSEWSiconFlag = False 'JV + AG 18/10/2013 revision task # 1341
                            Else
                                'bsTSPauseSessionButton.Enabled = False
                                'JV + AG 18/10/2013 revision task # 1341
                                'bsTSMultiFunctionSessionButton.Image = imagePlay
                                showPAUSEWSiconFlag = False
                                'JV + AG 18/10/2013 revision task # 1341
                            End If

                            'bsTSContinueSessionButton.Enabled = False
                            'JV + AG 18/10/2013 - revision task # 1341
                            'bsTSMultiFunctionSessionButton.Image = imagePause
                            showSTARTWSiconFlag = False
                            'JV + AG 18/10/2013 task # 1341

                        Else 'Move here the code previous to the automate Creation of WS with LIS mode
                            'AG 12/09/2011
                            'Instrument buttons
                            'AG 12/03/2012
                            'bsTSShutdownButton.Enabled = True
                            bsTSShutdownButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.SHUT_DOWN)
                            WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012
                            'AG 12/03/2012

                            bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag
                            'bsTSPauseSessionButton.Enabled = False
                            'JV + AG 18/10/2013 revision task # 1341
                            'bsTSMultiFunctionSessionButton.Image = imagePlay
                            showPAUSEWSiconFlag = False
                            'JV + AG 18/10/2013 task # 1341
                            bsTSAbortSessionButton.Enabled = False

                            'Session buttons depending if exist PendingExecutionsLeft or not (AG 09/03/2012 add condition WS not aborted
                            If String.Compare(WorkSessionIDAttribute, "", False) <> 0 AndAlso AnalyzerIDAttribute <> "" AndAlso String.Compare(WSStatusAttribute, "EMPTY", False) <> 0 _
                              AndAlso String.Compare(WSStatusAttribute, "ABORTED", False) <> 0 AndAlso myAx00Action <> AnalyzerManagerAx00Actions.RUNNING_START Then
                                Dim myWSDelegate As New WorkSessionsDelegate
                                Dim pendingExecutionsLeft As Boolean = False
                                Dim executionsNumber As Integer = 0
                                myGlobal = myWSDelegate.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, executionsNumber, pendingExecutionsLeft)

                                If executionsNumber > 0 Then
                                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                        Dim wsStarted As Boolean = CType(myGlobal.SetDatos, Boolean)

                                        ' XB 29/10/2013 - Enable START_WS if there are only ISE preparations pending but locked by any required calibration - BT #1070
                                        If Not pendingExecutionsLeft Then
                                            pendingExecutionsLeft = IsTherePendingOnlyISEExecutions()
                                        End If
                                        ' XB 29/10/2013

                                        If Not wsStarted AndAlso pendingExecutionsLeft Then
                                            'bsTSStartSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                            'JV + AG 18/10/2013 revision task # 1341
                                            Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                            showSTARTWSiconFlag = bReturn 'AG 23/01/2014 #1467 (showSTARTWSiconFlag Or bReturn)
                                            'JV + AG 18/10/2013 revision task # 1341
                                        ElseIf pendingExecutionsLeft Then
                                            'JV + AG 18/10/2013 revision task # 1341
                                            'bsTSContinueSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CONTINUE_WS)
                                            Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CONTINUE_WS)
                                            showSTARTWSiconFlag = bReturn 'AG 23/01/2014 #1467 (showSTARTWSiconFlag Or bReturn)
                                            'JV + AG 18/10/2013 revision task # 1341
                                        Else 'All Executions pending are paused or locked
                                            'bsTSStartSessionButton.Enabled = False
                                            'bsTSContinueSessionButton.Enabled = False
                                            'JV + AG 18/10/2013 revision task # 1341
                                            'bsTSMultiFunctionSessionButton.Image = imagePause
                                            showSTARTWSiconFlag = False
                                            'JV + AG 18/10/2013 revision task # 1341
                                            'AG 09/07/2013
                                            'XB 23/07/2013 - auto HQ
                                            'If autoWSCreationWithLISModeAttribute Then
                                            If autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag Then
                                                'XB 23/07/2013
                                                If Not wsStarted Then
                                                    'bsTSStartSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                                    'JV + AG 18/10/2013 revision task # 1341
                                                    Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                                    'If bReturn Then bsTSMultiFunctionSessionButton.Image = imagePlay Else bsTSMultiFunctionSessionButton.Image = imagePause
                                                    showSTARTWSiconFlag = bReturn 'AG 23/01/2014 #1467 (showSTARTWSiconFlag Or bReturn)
                                                    'JV + AG 18/10/2013 revision task # 1341
                                                Else
                                                    'JV + AG 18/10/2013 revision task # 1341
                                                    'bsTSContinueSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CONTINUE_WS)
                                                    Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CONTINUE_WS)
                                                    showSTARTWSiconFlag = bReturn 'AG 23/01/2014 #1467 (showSTARTWSiconFlag Or bReturn)
                                                    'JV + AG 18/10/2013 revision task # 1341
                                                End If
                                            End If
                                            'AG 09/07/2013
                                        End If
                                    End If

                                    'AG 08/07/2013 - Special code for auto WS creation with LIS
                                Else
                                    'XB 23/07/2013 - auto HQ
                                    'If autoWSCreationWithLISModeAttribute Then
                                    If autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag Then
                                        'XB 23/07/2013
                                        'JV + AG 18/10/2013 revision task # 1341
                                        'bsTSStartSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                        Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                        showSTARTWSiconFlag = bReturn 'AG 23/01/2014 #1467 (showSTARTWSiconFlag Or bReturn)
                                        'JV + AG 18/10/2013 revision task # 1341
                                    End If
                                    'AG 08/07/2013
                                End If

                            Else
                                'bsTSStartSessionButton.Enabled = False
                                'bsTSContinueSessionButton.Enabled = False
                                'AG 18/10/2013 revision
                                'bsTSMultiFunctionSessionButton.Image = imagePause
                                showSTARTWSiconFlag = False
                                'AG 18/10/2013 revision
                                'AG 08/07/2013 - Special code for auto WS creation with LIS
                                'XB 23/07/2013 - auto HQ
                                ' If autoWSCreationWithLISModeAttribute AndAlso String.Compare(WSStatusAttribute, "EMPTY", False) = 0 Then
                                If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso String.Compare(WSStatusAttribute, "EMPTY", False) = 0 Then
                                    'XB 23/07/2013
                                    'JV + AG 18/10/2013 revision task # 1341
                                    'bsTSStartSessionButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                    Dim bReturn As Boolean = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.START_WS)
                                    showSTARTWSiconFlag = bReturn 'AG 23/01/2014 #1467 (showSTARTWSiconFlag Or bReturn)
                                    'JV + AG 18/10/2013 revision task # 1341
                                End If
                                'AG 08/07/2013 

                            End If 'If WorkSessionIDAttribute <> "" And AnalyzerIDAttribute <> "" Then

                        End If
                        'TODO
                        'AG 14/05/2012
                        'Ax00 is ready and wup no ha acabado y boton FinWup visible
                    ElseIf myAx00Ready AndAlso Not WarmUpFinishedAttribute Then
                        bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag 'AG 23/07/2012

                        If ActiveMdiChild Is UiMonitor Then
                            Dim myMonitor As UiMonitor = CType(ActiveMdiChild, UiMonitor)
                            If myMonitor.IsEndWarmUpButtonVisible Then
                                bsTSShutdownButton.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.SHUT_DOWN) 'AG 14/05/2012 True 
                                WithShutToolStripMenuItem.Enabled = bsTSShutdownButton.Enabled  'DL 04/06/2012
                            End If
                        End If

                    End If 'If myAx00Ready Then


                    'AG 03/10/2011 - Buttons over analyzer not in Vertical Buttons Bar
                    'Barcode button
                    Dim bcButtonStatus As Boolean = False 'bar code button status
                    Dim sensorValue As Single = MDIAnalyzerManager.GetSensorValue(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED)

                    If myAx00Status = AnalyzerManagerStatus.STANDBY AndAlso myAx00Ready AndAlso _
                        MDIAnalyzerManager.BarCodeProcessBeforeRunning = AnalyzerManager.BarcodeWorksessionActions.BARCODE_AVAILABLE AndAlso _
                        sensorValue = 1 Then
                        bcButtonStatus = ActivateButtonWithAlarms(ActionButton.READ_BARCODE) 'AG 28/03/2012 - buttonStatus = True
                    End If

                    'Similar business for check bottle volume (in RotorPositions screen) but using (ActionButton.CHECK_BOTTLE_VOLUME)
                    Dim cvbButtonStatus As Boolean = False 'check volume bottle button status
                    'TODO


                    If Not ActiveMdiChild Is Nothing Then
                        Dim analyzerSettings As New AnalyzerSettingsDelegate
                        Dim resultData As New GlobalDataTO

                        If (TypeOf ActiveMdiChild Is IWSRotorPositions) Then
                            Dim CurrentMdiChild As IWSRotorPositions = CType(ActiveMdiChild, IWSRotorPositions)
                            If Not CurrentMdiChild.bsScanningButton Is Nothing AndAlso CurrentMdiChild.bsScanningButton.Enabled <> bcButtonStatus Then
                                'CurrentMdiChild.bsScanningButton.Enabled = buttonStatus
                                If bcButtonStatus Then
                                    'A part from Analyzer status check if barcode reader is enabled
                                    If String.Compare(CurrentMdiChild.CurrentRotorType, "SAMPLES", False) = 0 Then
                                        resultData = analyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
                                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                            Dim myDataSet As New AnalyzerSettingsDS
                                            myDataSet = CType(resultData.SetDatos, AnalyzerSettingsDS)
                                            If myDataSet.tcfgAnalyzerSettings.Rows.Count > 0 Then
                                                If CType(myDataSet.tcfgAnalyzerSettings(0).CurrentValue, Boolean) Then bcButtonStatus = False
                                            End If
                                        End If

                                    Else
                                        resultData = analyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString)
                                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                            Dim myDataSet As New AnalyzerSettingsDS
                                            myDataSet = CType(resultData.SetDatos, AnalyzerSettingsDS)
                                            If myDataSet.tcfgAnalyzerSettings.Rows.Count > 0 Then
                                                If CType(myDataSet.tcfgAnalyzerSettings(0).CurrentValue, Boolean) Then bcButtonStatus = False
                                            End If
                                        End If

                                    End If
                                End If

                                CurrentMdiChild.bsScanningButton.Enabled = bcButtonStatus
                            End If

                            'AG 28/03/2012 - check bottle volume (only for reagents)
                            If Not CurrentMdiChild.bsCheckRotorVolumeButton Is Nothing AndAlso CurrentMdiChild.bsCheckRotorVolumeButton.Enabled <> cvbButtonStatus Then
                                'TODO

                                CurrentMdiChild.bsCheckRotorVolumeButton.Enabled = cvbButtonStatus
                                CurrentMdiChild.bsReagentsCheckVolumePosButton.Enabled = cvbButtonStatus
                                'CurrentMdiChild.bsSamplesCheckVolumePosButton.Enabled = False 'Always disabled for samples
                            End If
                            'AG 28/03/2012

                        ElseIf (TypeOf ActiveMdiChild Is IWSSampleRequest) Then
                            Dim CurrentMdiChild As IWSSampleRequest = CType(ActiveMdiChild, IWSSampleRequest)
                            If Not CurrentMdiChild.bsScanningButton Is Nothing AndAlso CurrentMdiChild.bsScanningButton.Enabled <> bcButtonStatus Then
                                If bcButtonStatus Then
                                    'A part from Analyzer status check if barcode reader is enabled
                                    resultData = analyzerSettings.GetAnalyzerSetting(Nothing, AnalyzerIDAttribute, GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString)
                                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                        Dim myDataSet As New AnalyzerSettingsDS
                                        myDataSet = CType(resultData.SetDatos, AnalyzerSettingsDS)
                                        If myDataSet.tcfgAnalyzerSettings.Rows.Count > 0 Then
                                            If CType(myDataSet.tcfgAnalyzerSettings(0).CurrentValue, Boolean) Then bcButtonStatus = False
                                        End If
                                    End If
                                End If

                                CurrentMdiChild.bsScanningButton.Enabled = bcButtonStatus
                            End If

                            'AG 22/02/2012 - Update button in change rotor utility
                        ElseIf (TypeOf ActiveMdiChild Is IChangeRotor) Then
                            Dim CurrentMdiChild As IChangeRotor = CType(ActiveMdiChild, IChangeRotor)
                            If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.NEWROTORprocess), "PAUSED", False) = 0 Then
                                bsTSChangeBottlesConfirm.Enabled = enableChangeBottlesConfirmFlag
                                'AG 28/03/2012
                                'CurrentMdiChild.bsChangeRotortButton.Enabled = Not bsTSChangeBottlesConfirm.Enabled
                                If Not bsTSChangeBottlesConfirm.Enabled Then
                                    CurrentMdiChild.bsChangeRotortButton.Enabled = ActivateButtonWithAlarms(ActionButton.CHANGE_REACTIONS_ROTOR)
                                End If

                            End If
                            'AG 22/02/2012

                        End If
                    End If

                    'TODO
                    'bsCheckRotorVolumeButton, bsReagentsCheckVolumePosButton, bsSamplesCheckVolumePosButton
                    'AG 03/10/2011

                Else 'If recover wash in process
                    If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess), "PAUSED", False) = 0 OrElse _
                       String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "PAUSED", False) = 0 Then
                        'Activate the change bottle confirm
                        bsTSChangeBottlesConfirm.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHANGE_BOTTLES_CONFIRM)
                    End If
                End If

            End If 'AG 23/03/2012 - ISE initialitation not finished (connection in StandBy)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ApplyRulesForStandBy ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ApplyRulesForStandBy ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Running activations action buttons rules
    ''' </summary>
    ''' <remarks>AG 09/01/2013 - refactoring
    ''' AG 15/04/2014 - #1591 do not send START while analyzer is starting pause
    ''' </remarks>
    Private Sub ApplyRulesForRunning(ByVal myAx00Action As GlobalEnumerates.AnalyzerManagerAx00Actions)
        Try
            Dim myGlobal As New GlobalDataTO

            ' XB 31/10/2013
            bsTSStartInstrumentButton.Enabled = False
            bsTSShutdownButton.Enabled = False

            If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess), "INPROCESS", False) = 0 Then
                'AG 27/08/2012 - STANDBY: All buttons Disabled | RUNNING: Abort button enabled
                SetActionButtonsEnableProperty(False)
                If MDIAnalyzerManager.AbortInstructionSent OrElse _
                   String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS") Then
                    bsTSAbortSessionButton.Enabled = False

                    'When analyzer finishes readings and the recovery results instructions flow starts disabled Abort button
                    'AG 04/12/2013 - BT #1397 while analyzer is scanning the abort is not available too (add also action PAUSE_START)
                ElseIf myAx00Action = AnalyzerManagerAx00Actions.END_RUN_END OrElse _
                       myAx00Action = AnalyzerManagerAx00Actions.NO_ACTION OrElse _
                       myAx00Action = AnalyzerManagerAx00Actions.BARCODE_ACTION_RECEIVED OrElse _
                       myAx00Action = AnalyzerManagerAx00Actions.PAUSE_START OrElse _
                       (myAx00Action = AnalyzerManagerAx00Actions.SOUND_DONE AndAlso String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess), "INPROCESS")) OrElse _
                       myAx00Action = AnalyzerManagerAx00Actions.POLLRD_RECEIVED Then
                    bsTSAbortSessionButton.Enabled = False

                    'Analyzer still perform readings
                Else
                    bsTSAbortSessionButton.Enabled = True
                End If

            Else
                'AG 21/03/2012 (add abort condition) + AG 02/09/2011
                If (WSStatusAttribute <> "ABORTED") AndAlso (WSStatusAttribute <> "INPROCESS" OrElse Not ExecuteSessionAttribute) Then
                    If Not MDIAnalyzerManager.AbortInstructionSent Then
                        SetWorkSessionUserCommandFlags(WorkSessionUserActions.START_WS)
                        WSStatusAttribute = "INPROCESS"
                    Else
                        WSStatusAttribute = "ABORTED"
                        ElapsedTimeTimer.Stop() 'Stop the elapsed time timer
                    End If
                End If
                'AG 02/09/2011

                'AG 24/02/2012 - when enter in running mark process before running finished
                'AG 13/02/2012 - Execute only when Sw enters in Running mode
                'If processBeforeRunningFinished = "0" AndAlso MDIAnalyzerManager.GetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 1 Then
                '    MDIAnalyzerManager.SetSensorValue(GlobalEnumerates.AnalyzerSensors.BEFORE_ENTER_RUNNING) = 0 'clear sensor
                '    processBeforeRunningFinished = "1" 'Finished ok
                'End If

                'AG 05/11/2013 - Task #1375 add new conditions required for the paused mode (in Running)
                'If String.Compare(processingBeforeRunning, "0", False) = 0 Then
                Dim finishFlag As Boolean = True
                If MDIAnalyzerManager.AllowScanInRunning Then
                    If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.subProcessEnterRunning Then
                        finishFlag = False

                        'AG 03/02/2014 - BT #1488 (in scenario barcode before running without LIS after leave PAUSE the menu remains disable)
                    ElseIf Not MDIAnalyzerManager Is Nothing AndAlso MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess) = "INPROCESS" Then
                        finishFlag = False
                    End If
                End If
                If String.Compare(processingBeforeRunning, "0", False) = 0 AndAlso finishFlag Then
                    'AG 05/11/2013 - Task #1375

                    processingBeforeRunning = "1" 'Finished ok
                    ScreenWorkingProcess = False
                End If
                'AG 24/02/2012

                'Instrument buttons
                'AG 24/01/2014 - #1467
                'bsTSChangeBottlesConfirm.Enabled = False
                'During running initialization or exiting pause mode (change bottle confirmation disable)
                If (myAx00Action = AnalyzerManagerAx00Actions.START_INSTRUCTION_START OrElse myAx00Action = AnalyzerManagerAx00Actions.START_INSTRUCTION_END) Then
                    bsTSChangeBottlesConfirm.Enabled = False

                    'Automatic process of auto worksession creation (change bottle confirmation disable)
                ElseIf (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                    bsTSChangeBottlesConfirm.Enabled = False

                    'Other scenarios
                Else
                    bsTSChangeBottlesConfirm.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHANGE_BOTTLES_CONFIRM)
                End If
                'AG 24/01/2014 - #1467

                '...
                '...

                'Session buttons
                'bsTSStartSessionButton.Enabled = False
                'JV + AG 18/10/2013 revision task # 1341
                'bsTSMultiFunctionSessionButton.Image = imagePause
                showSTARTWSiconFlag = False
                'JV + AG 18/10/2013 task # 1341

                'AG + XB 15/10/2013 - BT #1333
                'bsTSContinueSessionButton.Enabled = False
                'bsTSContinueSessionButton.Enabled = MDIAnalyzerManager.AllowScanInRunning AndAlso Not MDIAnalyzerManager.ContinueAlreadySentFlag And _
                '                                    Not (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS")
                'JV + AG 18/10/2013 revision task # 1341
                Dim bReturn As Boolean = MDIAnalyzerManager.AllowScanInRunning AndAlso Not MDIAnalyzerManager.ContinueAlreadySentFlag And _
                                                    Not (MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess) = "INPROCESS")
                showSTARTWSiconFlag = (showSTARTWSiconFlag Or bReturn)
                'JV + AG 18/10/2013 task # 1341


                'AG 21/12/2011
                'Option A: Buttons PAUSE & ABORT become enabled when analyzer enter in running mode
                'Are disabled when user press action button or when Sw sends instruction because an alarm

                ' XB 15/10/2013 - Implement mode when Analyzer allows Scan Rotors in RUNNING (PAUSE mode) - BT #1333
                'Pause disable when pause or abort requested
                'If (MDIAnalyzerManager.EndRunInstructionSent OrElse _
                '    String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess), "INPROCESS") OrElse _
                '    MDIAnalyzerManager.AbortInstructionSent OrElse _
                '    String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS")) Then bsTSPauseSessionButton.Enabled = False Else bsTSPauseSessionButton.Enabled = True
                If (MDIAnalyzerManager.PauseInstructionSent OrElse _
                    Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess), "") OrElse _
                    MDIAnalyzerManager.AbortInstructionSent OrElse _
                    String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS")) _
                    Then showPAUSEWSiconFlag = False Else showPAUSEWSiconFlag = True 'bsTSPauseSessionButton.Enabled = True String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS")) Then bsTSPauseSessionButton.Enabled = False Else bsTSPauseSessionButton.Enabled = True
                ' XB 15/10/2013 - BT #1333

                'AG 24/01/2014 - #1467 if automatic WS creation process is in course the pause button must be enabled in order to not return to normal running
                If Not showPAUSEWSiconFlag AndAlso MDIAnalyzerManager.AllowScanInRunning Then
                    If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                        showSTARTWSiconFlag = False
                        If Not pausingAutomateProcessFlag AndAlso Not HQProcessByUserFlag Then showPAUSEWSiconFlag = True
                    End If
                End If
                'AG 24/01/2014

                'AG 05/11/2013 - Task #1375 - new activation rule. In running paused mode once the STARTWS button has been clicked
                'during the automatic WS creation with LIS the active button must be the PAUSEWS not the STARTWS
                If showSTARTWSiconFlag AndAlso MDIAnalyzerManager.AllowScanInRunning Then
                    If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                        showSTARTWSiconFlag = False
                        'showPAUSEWSiconFlag = True
                        If Not pausingAutomateProcessFlag AndAlso Not HQProcessByUserFlag Then showPAUSEWSiconFlag = True Else showPAUSEWSiconFlag = False

                        'AG 06/11/2013 - Task #1375 another new condition in paused mode: User goes to rotor position screen and read barcode -> STARTWS button disabled
                    ElseIf isSinglecanningRequestedAttribute Then
                        showSTARTWSiconFlag = False

                        'AG 12/11/2013 - Task #1375 :Else read barcode before running functionality
                        'AG 19/11/2013 - Task #1396-e (add also flag StartRunning)
                    ElseIf String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess), "INPROCESS") OrElse _
                        String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess), "INPROCESS") OrElse _
                        String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.StartRunning), "INI") Then
                        showSTARTWSiconFlag = False

                    End If
                End If
                'AG 05/11/2013

                'AG 24/01/2014 - comment this line JV 23/01/2014 #1467
                'In pause mode analyzer do not freeze due cover opens, Sw evaluate the same conditions just before go running in method FinishAutomaticWSWithLIS - In automatic WS creation mode
                '(In manual mode button must be disable if cover enabled and opened)
                If (showSTARTWSiconFlag AndAlso MDIAnalyzerManager.AllowScanInRunning) Then
                    If Not autoWSCreationWithLISModeAttribute Then
                        showSTARTWSiconFlag = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CONTINUE_WS)
                    End If
                End If
                'JV 23/01/2014 #1467

                'Abort disable when abort requested
                If MDIAnalyzerManager.AbortInstructionSent OrElse _
                   String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS") Then
                    bsTSAbortSessionButton.Enabled = False
                Else
                    'AG 06/11/2013 - Task #1375 - new activation rule. In running paused mode once the STARTWS button has been clicked
                    'during the automatic WS creation with LIS the abort WS button must be disabled
                    'bsTSAbortSessionButton.Enabled = True
                    Dim enabledValue As Boolean = True 'By default ABORT is enabled in running but when in pause mode Sw has started the autoCreation WS with LIS process
                    If MDIAnalyzerManager.AllowScanInRunning Then
                        If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                            enabledValue = False

                            'Task #1375 another new condition in paused mode: User goes to rotor position screen and read barcode -> STARTWS and ABORTWS buttons disabled
                        ElseIf isSinglecanningRequestedAttribute Then
                            enabledValue = False

                            'AG 12/11/2013 - Task #1375 :Else read barcode before running functionality
                        ElseIf String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.BarcodeSTARTWSProcess), "INPROCESS") OrElse _
                            String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ISEConditioningProcess), "INPROCESS") Then
                            enabledValue = False

                        End If
                    End If
                    bsTSAbortSessionButton.Enabled = enabledValue
                    'AG 06/11/2013

                End If


                'Session management ... when new executions are added in Running (for instance repetitions) and the A400;END  or ABORT has
                'been already sent (but not using the PAUSE or ABORT button) Sw has to send the A400;START; instruction

                'AG 06/03/2012 - remove condition "AndAlso myAx00InstructionReceived = AnalyzerManagerSwActionList.STATUS_RECEIVED" because now the reception event is triggered
                'only with the photometric results instruction
                'NOTE: (AG 06/11/2013) - Next code wont be executed (that's OK) in pause mode because in paused mode EndRunInstructionSent = FALSE!!!
                'AG 19/11/2013 - #1396-d Do not start automatically in pause mode (condition: 'Not MDIAnalyzerManager.AllowScanInRunning')
                'AG 15/04/2014 - #1591 do not send START while analyzer is starting pause (flag PAUSEprocess = INPROCESS)
                If Not MDIAnalyzerManager.AllowScanInRunning AndAlso ExecuteSessionAttribute AndAlso MDIAnalyzerManager.EndRunInstructionSent AndAlso _
                   Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ENDprocess), "INPROCESS") AndAlso _
                   Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS") AndAlso _
                   Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.PAUSEprocess), "INPROCESS") Then

                    'AG 30/11/2011 - check if no exists alarms whose treatment in RUNNING is sent ENDRUN instruction
                    Dim endRunDueAlarms As Boolean = False 'This variable takes TRUE value when exists alarms whose running treatment is send ENDRUN
                    endRunDueAlarms = MDIAnalyzerManager.ExistSomeAlarmThatRequiresStopWS() 'endRunDueAlarms = ExistSomeAlarmThatRequiresSendENDRUN(myAx00CurrentAlarms)

                    If Not endRunDueAlarms AndAlso String.Compare(WorkSessionIDAttribute, "", False) <> 0 AndAlso Not String.Equals(AnalyzerIDAttribute, String.Empty) Then
                        Dim myWSDelegate As New WorkSessionsDelegate
                        Dim pendingExecutionsLeft As Boolean = False
                        Dim executionsNumber As Integer = 0
                        myGlobal = myWSDelegate.StartedWorkSessionFlag(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, executionsNumber, pendingExecutionsLeft)
                        If pendingExecutionsLeft Then
                            myGlobal = MDIAnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.START, True, Nothing, Nothing, Nothing, Nothing)
                        End If
                    End If 'If WorkSessionIDAttribute <> "" And AnalyzerIDAttribute <> "" Then

                End If 'If myAx00Action = AnalyzerManagerAx00Actions.END_RUN_START Then
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ApplyRulesForRunning ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ApplyRulesForRunning ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Freeze activations action buttons rules
    ''' </summary>
    ''' <param name="myAx00Status"></param>
    ''' <remarks>AG 09/01/2013 - refactoring</remarks>
    Private Sub ApplyRulesForFreeze(ByVal myAx00Status As GlobalEnumerates.AnalyzerManagerStatus)
        Try

            'Stop the worksession timer
            If ElapsedTimeTimer.Enabled Then ElapsedTimeTimer.Stop()
            If BsTimerWUp.Enabled Then BsTimerWUp.Enabled = False
            flagExitWithShutDown = False 'DL 04/06/2012

            'Freeze RESET leave only connect button
            If String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "RESET") Then
                SetActionButtonsEnableProperty(False)
                bsTSConnectButton.Enabled = True

                'If analyzer is in FREEZE mode recover and connect buttons are enabled in standy (when freeze mode <> AUTOrecover)
            ElseIf myAx00Status = AnalyzerManagerStatus.STANDBY AndAlso _
                   Not String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "AUTO") AndAlso _
                   Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess), "INPROCESS") AndAlso _
                   Not String.Equals(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.RECOVERprocess), "PAUSED") Then

                'bsTSPauseSessionButton.Enabled = False 'AG 01/10/2012 - force disable this button
                'JV + AG 18/10/2013 task # 1341
                'bsTSMultiFunctionSessionButton.Image = imagePlay
                showPAUSEWSiconFlag = False
                'JV + AG 18/10/2013 task # 1341
                bsTSAbortSessionButton.Enabled = False 'AG 01/10/2012 - force disable this button
                'bsTSConnectButton.Enabled = True    ' XB 07/11/2013 - Don't enable Connect button, just  Recover button must Enable - BT #1155
                bsTSRecover.Enabled = True

            ElseIf String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "AUTO") Then
                SetActionButtonsEnableProperty(False)
                'bsTSConnectButton.Enabled = True
                If myAx00Status <> AnalyzerManagerStatus.RUNNING Then bsTSConnectButton.Enabled = True

            ElseIf String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "PARTIAL") Then
                SetActionButtonsEnableProperty(False)
                'AG 16/05/2012 - when freeze the connect button only if not running
                'bsTSConnectButton.Enabled = True
                If MDIAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.RUNNING Then
                    If String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "INPROCESS", False) <> 0 AndAlso _
                       String.Compare(MDIAnalyzerManager.SessionFlag(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess), "PAUSED", False) <> 0 Then

                        bsTSAbortSessionButton.Enabled = True 'AG 19/04/2012 - Abort (with out wash) is allowed while Partial Freeze

                    End If
                Else
                    bsTSConnectButton.Enabled = True
                End If

            Else
                SetActionButtonsEnableProperty(False)
                If myAx00Status = AnalyzerManagerStatus.STANDBY Then
                    bsTSChangeBottlesConfirm.Enabled = ActivateButtonWithAlarms(GlobalEnumerates.ActionButton.CHANGE_BOTTLES_CONFIRM)
                End If
            End If

            'Total freeze means WS is aborted
            If myAx00Status = AnalyzerManagerStatus.RUNNING AndAlso _
               String.Equals(MDIAnalyzerManager.AnalyzerFreezeMode, "TOTAL") Then

                WSStatusAttribute = "ABORTED"
                'TR 15/05/2012 -Commented because time is previously stopped.
                'ElapsedTimeTimer.Stop() 'Stop the elapsed time timer
            End If

            'AG 24/01/2014 - #1467 If analyzer freezes during process set all variables to their original value
            If (autoWSCreationWithLISModeAttribute OrElse HQProcessByUserFlag) AndAlso automateProcessCurrentState <> LISautomateProcessSteps.notStarted Then
                MDIAnalyzerManager.BarCodeProcessBeforeRunning = AnalyzerManager.BarcodeWorksessionActions.BARCODE_AVAILABLE
                SetAutomateProcessStatusValue(LISautomateProcessSteps.notStarted)
                InitializeAutoWSFlags()
            End If
            'AG 24/01/2014

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ApplyRulesForFreeze ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ApplyRulesForFreeze ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

#End Region

End Class

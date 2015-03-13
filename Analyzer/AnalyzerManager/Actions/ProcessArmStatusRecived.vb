Imports System.Data.SqlClient
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Core.Interfaces
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Namespace Biosystems.Ax00.Core.Entities
    Public Class ProcessArmStatusRecived

        Private _analyzerManager As IAnalyzerManager
        Private ReadOnly _instructionReceived As List(Of InstructionParameterTO)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="analyzerMan"></param>
        ''' <param name="instructionReceived"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef analyzerMan As IAnalyzerManager, ByVal instructionReceived As List(Of InstructionParameterTO))
            _analyzerManager = analyzerMan
            _instructionReceived = instructionReceived
        End Sub

        ''' <summary>
        ''' Sw has received an ANSBR1 or ANSBR2 or ANSBM1 instruction 
        ''' Do business depending the instruction parameter values
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 15/03/2011
        ''' Modified by: TR 27/09/2012 - Implement funcitionality of reagents on board, when the bottle status change to LOCKED
        '''              SA 15/11/2012 - Before calling function ReagentBottleManagement, if field BarcodeInfo is not NULL, check 
        '''                              also it is informed (different of an empty string)
        '''              AG 18/11/2013 - (#1385) Decrement the number of tests using the positions for sample or reagent2 or arm r2 washing
        '''              JV 09/01/2014 - BT #1443 ==> Added Status value in his
        '''              SA 20/01/2014 - BT #1443 ==> Changes made previously for this same item did not work due to the Position Status passed when call function 
        '''                                           ReagentsOnBoardDelegate.ReagentBottleManagement was the current Position Status instead of the re-calculated 
        '''                                           Position Status (according value of the LevelControl informed for the Analyzer in the received Instruction).
        '''                                           Code included in section labelled as (2.2) has been re-written to make it more clear. 
        '''                                           Added a new call to ReagentsOnBoardDelegate.ReagentBottleManagement in section labelled as (2.3), to allow
        '''                                           save the DEPLETED Status when the Instruction indicates the Detection Level has failed
        '''              SA 28/05/2014 - BT #1627 ==> Added changes to avoid call functions that are specific for REAGENTS (functions of ReagentsOnBoardManagement)  
        '''                                           when DILUTION and/or WASHING SOLUTIONS are dispensed. Current code just verify the Rotor Type, but not the 
        '''                                           Bottle content. Changes have been made in section labelled (2.2) 
        '''              AG 04/06/2014 - #1653 Check if WRUN (reagents washings) could not be completed remove the last WRUN for wash reagents sent
        '''              AG 13/06/2014 #1662 - Protection against change positions in pause when S / R2 arm still have not been finished with this position
        '''                                    If there not information on the position indicated andalso prepID is 0 --> Do nothing
        ''' </remarks>
        Public Function InstructionProcessing() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing

            Try
                'Dim Utilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO
                Dim StartTime As DateTime = Now 'AG 11/06/2012 - time estimation

                '*******************************************************'
                '(1) GET ALL FIELDS INFORMED IN THE RECEIVED INSTRUCTION'
                '*******************************************************'
                'Get Instruction field (parameter index 2)
                If Not GetItem(_instructionReceived, myGlobal, myInstParamTO, 2) Then
                    Return myGlobal
                End If
                Dim myInst = myInstParamTO.ParameterValue

                'Get Preparation Identifier field (ID, parameter index 3)
                If Not GetItem(_instructionReceived, myGlobal, myInstParamTO, 3) Then
                    Return myGlobal
                End If
                Dim myPrepID As Integer = 0
                If (IsNumeric(myInstParamTO.ParameterValue)) Then
                    myPrepID = CInt(myInstParamTO.ParameterValue)
                End If

                'Get Well Number field (W, parameter index 4)
                If Not GetItem(_instructionReceived, myGlobal, myInstParamTO, 4) Then
                    Return myGlobal
                End If
                Dim myWell As Integer = 0
                If (IsNumeric(myInstParamTO.ParameterValue)) Then
                    myWell = CInt(myInstParamTO.ParameterValue)
                End If

                'Get Well Status field (S, parameter index 5)
                If Not GetItem(_instructionReceived, myGlobal, myInstParamTO, 5) Then
                    Return myGlobal
                End If
                Dim myWellStatus = myInstParamTO.ParameterValue

                'Get Bottle Position  field (P, parameter index 6)            
                If Not GetItem(_instructionReceived, myGlobal, myInstParamTO, 6) Then
                    Return myGlobal
                End If
                Dim myBottlePos As Integer = 0
                If (IsNumeric(myInstParamTO.ParameterValue)) Then
                    myBottlePos = CInt(myInstParamTO.ParameterValue)
                End If

                'Get Level Control field (L, parameter index 7)            
                If Not GetItem(_instructionReceived, myGlobal, myInstParamTO, 7) Then
                    Return myGlobal
                End If
                Dim myLevelControl As Integer = 0
                If (IsNumeric(myInstParamTO.ParameterValue)) Then
                    myLevelControl = CInt(myInstParamTO.ParameterValue)
                End If

                Dim myClotStatus As String = ""
                Dim myRotorName As String = "REAGENTS"
                If (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString) Then
                    myRotorName = "SAMPLES"

                    'Get Clot Status field (C, parameter index 8)
                    If Not GetItem(_instructionReceived, myGlobal, myInstParamTO, 8) Then
                        Return myGlobal
                    End If
                    myClotStatus = myInstParamTO.ParameterValue
                End If

                '**********************************************************'
                '(2) BUSINESS LOGIC FOR PROCESSING THE RECEIVED INSTRUCTION'
                '**********************************************************'

                '********************************'
                '(2.1) MONITORING REACTIONS ROTOR'
                '********************************'
                Dim execDelg As New ExecutionsDelegate
                Dim uiRefreshMyGlobal As New GlobalDataTO 'Use an auxiliar GlobalDataTO
                Dim myReactionsRotor As New ReactionsRotorDelegate

                'Using the variables: myPrepID, myWell & myWellStatus create or update a record into twksWSReactionsRotor table for monitoring the Reactions Rotor
                myGlobal = myReactionsRotor.UpdateWellByArmStatus(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, myWell, myWellStatus, myPrepID, _
                                                                  _analyzerManager.AnalyzerCurrentAction, myRotorName)

                'Prepare UIRefresh Dataset (NEW_WELLSTATUS_RECEIVED) for refresh screen when needed
                If (Not myGlobal.HasError AndAlso myPrepID > 0) Then
                    Dim reactionsDS As ReactionsRotorDS = DirectCast(myGlobal.SetDatos, ReactionsRotorDS)
                    uiRefreshMyGlobal = _analyzerManager.PrepareUIRefreshEventNum3(dbConnection, GlobalEnumerates.UI_RefreshEvents.REACTIONS_WELL_STATUS_CHANGED, reactionsDS, True)
                End If

                'Prepare internal variables for Alarms Management
                Dim alarmStatusList As New List(Of Boolean)
                Dim alarmAdditionalInfoList As New List(Of String)
                Dim alarmList As New List(Of GlobalEnumerates.Alarms)

                Dim reagentNumberWithNoVolume As Integer = 0 'NOTE: 0=Sample, 1=Reagent1, 2=Reagent2
                Dim alarmLevelDetectionEnum As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                Dim alarmCollisionEnum As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE

                Select Case (myInst)
                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSBR1.ToString
                        alarmLevelDetectionEnum = GlobalEnumerates.Alarms.R1_NO_VOLUME_WARN
                        alarmCollisionEnum = GlobalEnumerates.Alarms.R1_COLLISION_WARN
                        reagentNumberWithNoVolume = 1

                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSBR2.ToString
                        alarmLevelDetectionEnum = GlobalEnumerates.Alarms.R2_NO_VOLUME_WARN
                        alarmCollisionEnum = GlobalEnumerates.Alarms.R2_COLLISION_WARN
                        reagentNumberWithNoVolume = 2

                    Case GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString
                        alarmLevelDetectionEnum = GlobalEnumerates.Alarms.S_NO_VOLUME_WARN
                        alarmCollisionEnum = GlobalEnumerates.Alarms.S_COLLISION_WARN
                        reagentNumberWithNoVolume = 0
                End Select

                'BT #1385 ==> Decrement the Number of Tests using the Position for Sample or Reagent2 or Arm R2 Washing
                If (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString OrElse myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBR2.ToString) Then
                    Dim inProcessDlg As New WSRotorPositionsInProcessDelegate
                    myGlobal = inProcessDlg.DecrementInProcessTestsNumber(dbConnection, _analyzerManager.ActiveAnalyzer, myRotorName, myBottlePos)
                End If

                '*********************************************************************************************************************************'
                '(2.2) UPDATE INFORMATION OF THE ROTOR POSITION (FOR ALL ARMS, ALTHOUGH THE BUSINESS IS DIFFERENT FOR SAMPLES AND REAGENTS ROTORS)' 
                '*********************************************************************************************************************************'
                Dim newElementStatus As String = "POS"
                Dim moreVolumeAvailable As Boolean = True
                Dim myBottleStatus As String = String.Empty
                Dim newPositionStatus As String = String.Empty

                'Read the current Rotor Position Status and assign it to initialRotorPositionStatus
                Dim rcpDs As New WSRotorContentByPositionDS
                Dim rcpDel As New WSRotorContentByPositionDelegate
                Dim initialRotorPositionStatus As String = String.Empty
                Dim elementTubeContent As String = String.Empty 'AG 30/05/2014 #1627

                myGlobal = rcpDel.ReadByRotorTypeAndCellNumber(dbConnection, myRotorName, myBottlePos, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    rcpDs = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS)
                    If (rcpDs.twksWSRotorContentByPosition.Rows.Count > 0) Then
                        If (Not rcpDs.twksWSRotorContentByPosition(0).IsStatusNull) Then initialRotorPositionStatus = rcpDs.twksWSRotorContentByPosition(0).Status
                        If (Not rcpDs.twksWSRotorContentByPosition(0).IsTubeContentNull) Then elementTubeContent = rcpDs.twksWSRotorContentByPosition(0).TubeContent 'AG 30/05/2014 #1627
                    End If
                End If


                Dim testLeft As Integer = 0
                Dim realVolume As Single = 0

                'If Reagent1 or WashingSolution or Predilution or Sample or ISE or Reagent2 dispensed
                If (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.R1.ToString Or myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.WS.ToString Or _
                    myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.PD.ToString Or myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.S1.ToString Or _
                    myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.DI.ToString Or myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.R2.ToString) Then
                    'Calculate the remaining Bottle Volume
                    Dim changesInPosition As Boolean = False
                    realVolume = 60
                    testLeft = 0

                    'Calculate the real volume test left for the bottle in position myBottlePos (using LevelControl value myLevelControl)
                    If (myRotorName = "REAGENTS") Then 'For reagents
                        Dim reagOnBoard As New ReagentsOnBoardDelegate
                        myGlobal = reagOnBoard.CalculateBottleVolumeTestLeft(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, myBottlePos, myLevelControl, realVolume, testLeft)

                        'BT #1443 - Calculate the new Position Status according the value of the Level Control returned by the Analyzer
                        Dim limitList As List(Of FieldLimitsDS.tfmwFieldLimitsRow) = (From a In _analyzerManager.FieldLimits _
                                                                                     Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.REAGENT_LEVELCONTROL_LIMIT.ToString _
                                                                                    Select a).ToList
                        If (limitList.Count > 0) Then
                            Debug.Print("=====================================")
                            Debug.Print("LEVEL CONTROL --> " & myLevelControl.ToString())

                            If (myLevelControl = 0) Then
                                newPositionStatus = "DEPLETED"

                            ElseIf (myLevelControl < CInt(limitList(0).MinValue)) Then
                                newPositionStatus = "FEW"

                            ElseIf (myLevelControl < CInt(limitList(0).MaxValue)) Then
                                newPositionStatus = "FEW"
                            Else
                                newPositionStatus = "INUSE"
                            End If

                            Debug.Print("NEW POS STATUS --> " & newPositionStatus.ToString())
                        End If
                        limitList = Nothing

                        'BT #1627 - Execute this block only when Reagent1 or Reagent2 have been dispensed, skip it for Dilution and Washing Solutions
                        If (elementTubeContent = "REAGENT") Then
                            'Validate and update Volumes in the Reagents Historic Table (verify if the Bottle has to be locked due to an invalid refill
                            If (Not myGlobal.HasError) Then
                                If (rcpDs.twksWSRotorContentByPosition.Rows.Count > 0) AndAlso (Not rcpDs.twksWSRotorContentByPosition(0).IsBarCodeInfoNull) AndAlso _
                                   (rcpDs.twksWSRotorContentByPosition(0).BarCodeInfo <> String.Empty) Then
                                    'BT #1443 - Added the new calculated Position Status to save it Reagents Historic Table (newPositionStatus, not current Rotor Position) 
                                    myGlobal = reagOnBoard.ReagentBottleManagement(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, myBottlePos, _
                                                                                   rcpDs.twksWSRotorContentByPosition(0).BarCodeInfo, _
                                                                                   newPositionStatus, realVolume)

                                    If (Not myGlobal.HasError) Then
                                        myBottleStatus = myGlobal.SetDatos.ToString()

                                        If (myBottleStatus = "LOCKED") Then
                                            newPositionStatus = myBottleStatus

                                            'Add the Alarm of Bottle blocked due to an invalid refill was detected only if the previous Position Status was not LOCKED
                                            If (initialRotorPositionStatus <> "LOCKED") Then
                                                Dim myExecution As Integer = 0
                                                Dim addInfoInvalidRefill As String = String.Empty

                                                myGlobal = execDelg.GetExecutionByPreparationID(dbConnection, myPrepID, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                                    Dim lockedBottleExecDS As ExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)

                                                    If (lockedBottleExecDS.twksWSExecutions.Rows.Count > 0) AndAlso (Not lockedBottleExecDS.twksWSExecutions(0).IsExecutionIDNull) Then
                                                        myExecution = lockedBottleExecDS.twksWSExecutions(0).ExecutionID
                                                    End If

                                                    'Get the information needed and add the Alarm to the Alarms List
                                                    myGlobal = execDelg.EncodeAdditionalInfo(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, myExecution, myBottlePos, _
                                                                                              GlobalEnumerates.Alarms.BOTTLE_LOCKED_WARN, reagentNumberWithNoVolume)
                                                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                                        addInfoInvalidRefill = CType(myGlobal.SetDatos, String)
                                                        _analyzerManager.PrepareLocalAlarmList(GlobalEnumerates.Alarms.BOTTLE_LOCKED_WARN, True, alarmList, alarmStatusList, addInfoInvalidRefill, alarmAdditionalInfoList, False)
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            If (Not myGlobal.HasError AndAlso (realVolume <> 60 OrElse testLeft <> 0)) Then
                                changesInPosition = True
                            End If
                        End If
                    Else
                        realVolume = 0
                        testLeft = 0

                        'NOTE: Is required implement similar business for SAMPLES or not? By now do not implement it
                    End If

                    If (Not myGlobal.HasError AndAlso (newPositionStatus <> String.Empty OrElse changesInPosition)) Then 'AG 05/12/2011 - Add changesInPosition (by now december 2011 only for REAGENTS ROTOR)
                        myGlobal = rcpDel.UpdateByRotorTypeAndCellNumber(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, myRotorName, _
                                                                          myBottlePos, newPositionStatus, realVolume, testLeft, False, False)

                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            rcpDs = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS)
                            If (rcpDs.twksWSRotorContentByPosition.Rows.Count > 0) Then
                                If (Not rcpDs.twksWSRotorContentByPosition(0).IsElementStatusNull) Then newElementStatus = rcpDs.twksWSRotorContentByPosition(0).ElementStatus
                            End If

                            'If the Position Status is DEPLETED or LOCKED (no more volume available in this Position), search if there is another Position 
                            'containing a not DEPLETED or LOCKED Bottle for the same Element 
                            If (newPositionStatus = "DEPLETED" OrElse newPositionStatus = "LOCKED") Then
                                myGlobal = rcpDel.SearchOtherPosition(dbConnection, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer, myRotorName, myBottlePos)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    rcpDs = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS)
                                    If (rcpDs.twksWSRotorContentByPosition.Count = 0) Then
                                        'There is not another available Bottle of the same Element
                                        moreVolumeAvailable = False
                                        If (newElementStatus = "POS") Then newElementStatus = "NOPOS"
                                    Else
                                        'Special business for Reagent2 (MTEST) -> For v2
                                        If (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBR2.ToString) Then
                                            'Be carefull: Only one instruction can be sent at a time
                                            'Special case: R2 current bottle R2 volume lower critical volume but exists other R2 bottles ... Sw has to inform Fw for use the new bottle from now
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If

                    'Only when there has been a change in the Position Status
                    If (newPositionStatus <> String.Empty) Then
                        'Prepare UIRefresh Dataset (NEW_ROTORPOSITION_STATUS) for refresh screen when needed
                        uiRefreshMyGlobal = _analyzerManager.PrepareUIRefreshEventNum2(dbConnection, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, myRotorName, myBottlePos, newPositionStatus, _
                                                                      newElementStatus, realVolume, testLeft, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                    End If
                End If

                '********************************************************'
                '(2.3) LEVEL DETECTION FAILS (Block Executions, all Arms)' 
                '********************************************************'
                Dim myExecutionId As Integer = -1
                Dim lockedExecutionsDs As ExecutionsDS = Nothing
                Dim addInfoVolMissingOrClot As String = String.Empty 'Additional info for volume missing or clot detection

                'AG 04/10/2012 - R1 or S or R2 level detection failed but exists additional position, then the Execution Status changes from INPROCESS to PENDING. 
                'AG 04/06/2014 - also used when a WRUN (reagents contamination) detects no volume in arm R1 (rename variable to 'reEvaluateNextToSend' instead of 'executionTurnToPendingAgainFlag')
                Dim reEvaluateNextToSend As Boolean = False

                'If level detection fail has been detected or volume detected but bottle exhausted and no move volume available -> call the  volume missing process
                If (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LD.ToString OrElse myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LP.ToString OrElse _
                    Not moreVolumeAvailable) Then
                    'AG 28/02/2012 - The Level Control is used only to mark the bottle as DEPLETED (remove condition ... OrElse myLevelControl = 0)
                    'It is possible that the Fw detects but informs L:0 (It cannot descend more)

                    'Get affected ExecutionID
                    Dim affectedExecutionId As Integer = -1
                    myGlobal = execDelg.GetExecutionByPreparationID(dbConnection, myPrepID, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        lockedExecutionsDs = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                        If (lockedExecutionsDs.twksWSExecutions.Rows.Count > 0) AndAlso (Not lockedExecutionsDs.twksWSExecutions(0).IsExecutionIDNull) Then
                            affectedExecutionId = lockedExecutionsDs.twksWSExecutions(0).ExecutionID
                        End If
                    End If

                    'Level detection fails (no Bottle or Sample Tube volume) in Reagents or Samples Rotors 
                    '(or <only for Bottles in Reagents Rotor> when detection was OK but Tests Left calculation says there are no more volume for other test preparations)

                    If (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LD.ToString Or Not moreVolumeAvailable) Then 'AG 02/02/2012
                        lockedExecutionsDs.Clear() 'AG 07/02/2012 Clear the DS. The executions that must be informed to be locked are calculated later

                        'The volume missing alarms are only generated when each position becomes DEPLETED (initial position status <> 'DEPLETED' but current status = 'DEPLETED')
                        'AG 16/06/2014 #1662 (add also protection against empty position (only for WRUN!!! If prepID <> 0 we must call LOCKING executions process)
                        Dim runningWashUsingEmptyPositionFlag = (myPrepID = 0 AndAlso initialRotorPositionStatus = "")

                        If (initialRotorPositionStatus <> "DEPLETED" AndAlso Not runningWashUsingEmptyPositionFlag) Then

                            'XBC 17/07/2012 - Estimated ISE Consumption by Firmware
                            'Every Samples volume alarm when ISE test operation increases PurgeA by firmware counter
                            If (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString) Then

                                myGlobal = execDelg.GetExecutionByPreparationID(dbConnection, myPrepID, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    Dim myExecutionsDs As ExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)

                                    If (myExecutionsDs.twksWSExecutions.Rows.Count > 0) Then
                                        Dim myExecutionType As String = myExecutionsDs.twksWSExecutions(0).ExecutionType

                                        If (myExecutionType = "PREP_ISE") Then
                                            If (Not _analyzerManager.ISEAnalyzer Is Nothing) Then
                                                _analyzerManager.ISEAnalyzer.PurgeAbyFirmware += 1
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            'TR 01/10/2012 - If the Bottle Status is LOCKED jump this
                            If (myBottleStatus <> "LOCKED") Then
                                'AG 23/05/2012 - Remove condition because when WashSolution fails, affectedExecutionID = -1 
                                addInfoVolMissingOrClot = ""
                                myGlobal = execDelg.EncodeAdditionalInfo(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, affectedExecutionId, myBottlePos, _
                                                                          alarmLevelDetectionEnum, reagentNumberWithNoVolume)

                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    addInfoVolMissingOrClot = CType(myGlobal.SetDatos, String)
                                End If
                                _analyzerManager.PrepareLocalAlarmList(alarmLevelDetectionEnum, True, alarmList, alarmStatusList, addInfoVolMissingOrClot, alarmAdditionalInfoList, False) 'Inform about the volume missing warm
                            End If

                            'AG 14/09/2012 v052 - previous versions used "Not moreVolumeAvailable" as parameter pPreparationIDPerformedOK, it works well but was confused
                            'More readable code for v052 
                            Dim prepPerformedOkFlag = (myWellStatus <> GlobalEnumerates.Ax00ArmWellStatusValues.LD.ToString AndAlso myWellStatus <> GlobalEnumerates.Ax00ArmWellStatusValues.LP.ToString)

                            'TR 28/09/2012
                            Dim isBottleLocked As Boolean = False
                            If (myBottleStatus = "LOCKED") Then
                                isBottleLocked = True
                            ElseIf (myBottleStatus = String.Empty) Then
                                myBottleStatus = "DEPLETED"
                            End If

                            'BT #1443 - Only when Bottle Status is DEPLETED, call the function to update the Status in the Reagents Historic Table
                            '#1627 - Add elementTubeContent = "REAGENT" 
                            If (myRotorName = "REAGENTS" AndAlso myBottleStatus = "DEPLETED") AndAlso elementTubeContent = "REAGENT" Then
                                Debug.Print("DETECTION FAILS: BOTTLE STATUS --> " & myBottleStatus.ToString())

                                Dim reagOnBoard As New ReagentsOnBoardDelegate
                                myGlobal = reagOnBoard.ReagentBottleManagement(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, myBottlePos, _
                                                                               rcpDs.twksWSRotorContentByPosition(0).BarCodeInfo, myBottleStatus, realVolume)
                                Debug.Print("CHECK THE TABLE NOW!!!")
                            End If


                            myGlobal = execDelg.ProcessVolumeMissing(dbConnection, myPrepID, myRotorName, myBottlePos, prepPerformedOkFlag, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer, _
                                                                      reEvaluateNextToSend, isBottleLocked, realVolume, testLeft)
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                lockedExecutionsDs = DirectCast(myGlobal.SetDatos, ExecutionsDS) 'Executions locked due to the volume missing alarm 
                            End If
                            'AG 23/01/2012 - Prepare UIRefresh into rotor position
                            If (myRotorName = "SAMPLES") Then
                                newElementStatus = "NOPOS" 'Samples rotor does not allow multiple positions of the same element
                            ElseIf (newElementStatus = String.Empty) Then '¿??
                                'Assign  value: POS or NOPOS
                                If (moreVolumeAvailable) Then newElementStatus = "POS" Else newElementStatus = "NOPOS"
                            End If

                            'TR 28/09/2012 - Implement the variable myBottleStatus instead of DEPLETED.
                            uiRefreshMyGlobal = _analyzerManager.PrepareUIRefreshEventNum2(dbConnection, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, myRotorName, _
                                                                          myBottlePos, myBottleStatus, newElementStatus, 0, 0, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                            'AG 04/06/2014 - #1653 Check if WRUN could not be completed and remove the last WRUN (for wash reagents) sent because it was not performed
                            '(Apply only for ANSBR1 + prepID =0 + status = LD)
                            If myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBR1.ToString AndAlso myPrepID = 0 AndAlso myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LD.ToString Then
                                Dim wrunSentLinq As List(Of AnalyzerManagerDS.sentPreparationsRow) = (From a As AnalyzerManagerDS.sentPreparationsRow In _analyzerManager.SentPreparations _
                                                                                                   Where a.ReagentWashFlag = True _
                                                                                                   Select a).ToList
                                If wrunSentLinq.Count > 0 Then
                                    wrunSentLinq(wrunSentLinq.Count - 1).Delete()
                                    _analyzerManager.SentPreparations.AcceptChanges()
                                    reEvaluateNextToSend = True 'Search for the next instruction to send again
                                End If
                                wrunSentLinq = Nothing 'Release memory
                            End If
                            'AG 04/06/2014 -

                            'AG 04/10/2012 V053 Remove moreVolumeAvailable from condition - when LD is received Sw do not search for more position so variable moreVolumeAvailable has his
                            'initial declaration value (TRUE).
                            '1st time LD is received and no more positions Sw works well because the position is not still market as DEPLETED
                            '2on, 3rd, ... time Sw works bad because the "If initialRotorPositionStatus <> "DEPLETED" Then" condition not is valid and Sw enters here
                            '
                            'Required change 04/10/2012: 
                            '          -> Search another position if exists then mark execution as PENDING
                            ''AG 01/10/2012 - Temporally code until the MTEST instruction is not implemented
                            ''R2 bottle has been set to DEPLETED before executing code in previous IF (If initialRotorPositionStatus <> "DEPLETED" Then)
                            ''It works well only for the 1st execution with R2 missing and change his status from INPROCESS to PENDING
                            ''but it does not work well for the others executions using this position because they do not change their status
                            ''(add also case samples because duplicated sample tubes is possible with samples barcode)
                            ''
                            ''IMPORTANT NOTE: This code AG 01/10/2012 must be commented or redesigned once the MTEST instruction will be developped

                            'When LD but previous condition If initialRotorPositionStatus <> "DEPLETED" is FALSE execute this code:
                        ElseIf (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBR2.ToString OrElse myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString) _
                                AndAlso myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LD.ToString Then

                            'When no more volume available (no more bottles with volume) set the flag moreVolumeAvailable = False
                            myGlobal = rcpDel.SearchOtherPosition(dbConnection, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer, myRotorName, myBottlePos)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                rcpDs = CType(myGlobal.SetDatos, WSRotorContentByPositionDS)
                                If rcpDs.twksWSRotorContentByPosition.Count = 0 Then
                                    moreVolumeAvailable = False
                                Else
                                    moreVolumeAvailable = True
                                    If affectedExecutionId > -1 Then
                                        myGlobal = execDelg.GetExecutionByPreparationID(dbConnection, myPrepID, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                                        If Not myGlobal.HasError Then
                                            lockedExecutionsDs = CType(myGlobal.SetDatos, ExecutionsDS)
                                            For Each execRow As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDs.twksWSExecutions.Rows 'Implements loop for ISE executions
                                                If Not execRow.IsExecutionIDNull Then
                                                    'Change execution status from INPROCESS -> PENDING
                                                    reEvaluateNextToSend = True
                                                    myGlobal = execDelg.UpdateStatusByExecutionID(dbConnection, "PENDING", execRow.ExecutionID, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                                                    'Prepare UIRefresh Dataset (EXECUTION_STATUS) for refresh screen when needed
                                                    If Not myGlobal.HasError Then
                                                        uiRefreshMyGlobal = _analyzerManager.PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, execRow.ExecutionID, 0, Nothing, False)
                                                    End If
                                                End If
                                            Next
                                        End If
                                        lockedExecutionsDs.Clear()
                                    End If
                                End If
                            End If
                        End If

                        'Level detection fails (no diluted sample) in reactions rotors
                    ElseIf (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.LP.ToString()) Then
                        'Mark the execution (PTEST instruction) as locked
                        If (Not lockedExecutionsDs Is Nothing) Then
                            If (lockedExecutionsDs.twksWSExecutions.Rows.Count > 0) Then
                                'Inform Diluted Sample Volume missing Alarm
                                _analyzerManager.PrepareLocalAlarmList(GlobalEnumerates.Alarms.DS_NO_VOLUME_WARN, True, alarmList, alarmStatusList, "", alarmAdditionalInfoList, False) 'Inform about the volume missing warm as "" in this case (with additionalinfoList)

                                myGlobal = execDelg.UpdateStatusByExecutionID(dbConnection, "LOCKED", affectedExecutionId, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                            End If
                        End If
                    End If

                    'AG 07/06/2012 - Bottle or tube empty but no executions locked (exists more volume in other positions or wash the last execution)
                    '                Despite that the software has to inform about the depleted position
                ElseIf (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.R1.ToString OrElse myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.S1.ToString _
                        OrElse myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.R2.ToString) AndAlso myLevelControl = 0 Then
                    'AG 13/07/2012 
                    'The volume missing alarms are only generated when each position becomes DEPLETED (initial position status <> 'DEPLETED' but current status = 'DEPLETED')
                    'If analyzer is bad adjusted Fw can send several instructions for the same position with S:R1 or S or R2 amd LevelControl=0. Sw not has to duplicate bottle depleted alarm!!!
                    If (initialRotorPositionStatus <> "DEPLETED") Then
                        'XBC 17/07/2012 - Estimated ISE Consumption by Firmware
                        'Every Samples volume alarm when ISE test operation increases PurgeA by firmware counter
                        If (myInst = GlobalEnumerates.AppLayerInstrucionReception.ANSBM1.ToString) Then

                            Dim myExecutionsDs As New ExecutionsDS
                            Dim myExecutionType As String
                            myGlobal = execDelg.GetExecutionByPreparationID(dbConnection, myPrepID, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                myExecutionsDs = CType(myGlobal.SetDatos, ExecutionsDS)
                                If myExecutionsDs.twksWSExecutions.Rows.Count > 0 Then
                                    myExecutionType = myExecutionsDs.twksWSExecutions(0).TestType

                                    If myExecutionType = "PREP_ISE" Then
                                        If Not _analyzerManager.ISEAnalyzer Is Nothing Then
                                            _analyzerManager.ISEAnalyzer.PurgeAbyFirmware += 1
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        Dim affectedExecutionId As Integer = -1
                        addInfoVolMissingOrClot = ""
                        myGlobal = execDelg.GetExecutionByPreparationID(dbConnection, myPrepID, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                        If Not myGlobal.HasError Then
                            lockedExecutionsDs = CType(myGlobal.SetDatos, ExecutionsDS)
                            If (lockedExecutionsDs.twksWSExecutions.Rows.Count > 0) AndAlso (Not lockedExecutionsDs.twksWSExecutions(0).IsExecutionIDNull) Then
                                affectedExecutionId = lockedExecutionsDs.twksWSExecutions(0).ExecutionID

                                myGlobal = execDelg.EncodeAdditionalInfo(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, affectedExecutionId, myBottlePos, _
                                                                          alarmLevelDetectionEnum, reagentNumberWithNoVolume)
                                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                    addInfoVolMissingOrClot = CType(myGlobal.SetDatos, String)
                                End If
                            End If
                            lockedExecutionsDs.Clear() 'Clear dataset, because in this case no executions have been locked
                        End If

                        _analyzerManager.PrepareLocalAlarmList(alarmLevelDetectionEnum, True, alarmList, alarmStatusList, addInfoVolMissingOrClot, alarmAdditionalInfoList, False) 'Inform about the volume missing warm (with additionalinfoList)

                    End If
                    'AG 07/06/2012

                    'AG 20/07/2012 when the samples arm goes to photometric rotor to get diluted sample and dispense it into another well in photometric rotor
                    '                new FW has to return PS instead of S1, in this way until L:0 Sw knows the position value not applies to the samples rotor
                ElseIf myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.PS.ToString AndAlso myLevelControl = 0 Then
                    'Nothing to do

                End If

                '***********************************'
                '(2.4) COLLISION DETECTED (ALL ARMS)'
                '***********************************'
                Dim collisionAffectedExecutionsDS As New ExecutionsDS
                If (myWellStatus = GlobalEnumerates.Ax00ArmWellStatusValues.KO.ToString) Then
                    'Implement business using the variables: myWellStatus, myPrepID,...
                    'If not already read it ... read the execution related to the status arm now
                    If (myExecutionId = -1) Then
                        myGlobal = execDelg.GetExecutionByPreparationID(dbConnection, myPrepID, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            collisionAffectedExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                            If (collisionAffectedExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                myExecutionId = collisionAffectedExecutionsDS.twksWSExecutions(0).ExecutionID
                            End If
                        End If
                    End If

                    'Mark execution as PENDING (or LOCKED if not volume)
                    If myExecutionId <> -1 AndAlso collisionAffectedExecutionsDS.twksWSExecutions(0).ExecutionStatus = "INPROCESS" Then
                        'AG 27/03/2012 - do not use myExecutionID ... when ise test several executions can share the same preparationID
                        For Each execRow As ExecutionsDS.twksWSExecutionsRow In collisionAffectedExecutionsDS.twksWSExecutions.Rows
                            myGlobal = execDelg.ChangeINPROCESSStatusByCollision(dbConnection, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession, execRow.ExecutionID)

                            'AG 08/03/2012 - Prepare UIRefresh Dataset (EXECUTION_STATUS, ROTORPOSITION_CHANGED) for refresh screen when needed
                            If Not myGlobal.HasError Then
                                myGlobal = _analyzerManager.PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, execRow.ExecutionID, 0, Nothing, False)
                            End If

                            'Update rotor position status (SAMPLES: PENDING, INPROCESS, FINISHED)
                            If Not myGlobal.HasError Then
                                myGlobal = rcpDel.UpdateSamplePositionStatus(dbConnection, execRow.ExecutionID, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                                If Not myGlobal.HasError Then
                                    rcpDs = CType(myGlobal.SetDatos, WSRotorContentByPositionDS)
                                    For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rcpDs.twksWSRotorContentByPosition.Rows
                                        myGlobal = _analyzerManager.PrepareUIRefreshEventNum2(dbConnection, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, "SAMPLES", row.CellNumber, _
                                                                             row.Status, "", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                                        'AG 09/03/2012 - do not inform about ElementStatus (old code: '... row.Status, "POS", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")'

                                        If myGlobal.HasError Then Exit For
                                    Next
                                End If
                            End If

                            If myGlobal.HasError Then Exit For
                        Next
                        'AG 27/03/2012

                    End If
                    'In method ProcessREadingsReceived implements:
                    '... (OK) Do not save reading neither call calculations if ExecutionStatus = PENDING or LOCKED

                    'AG 20/07/2012 - Collision Arm alarm (Warning level) has not to be saved. Fw sends both Warning (ansbxx) / Error (Status)
                    'If Sw generates both alarms it seems like repeated alarm, so we save only the error level (but keep doing the alarm warning treatment)
                    'PrepareLocalAlarmList(alarmCollisionEnum, True, alarmList, alarmStatusList)
                End If


                '*********************************************'
                '(2.5) CLOT DETECTED (ONLY FOR SAMPLES ROTOR) '
                '*********************************************'
                Dim alarmClotDetectionEnum As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                Dim clotAffectedExecutionsDS As New ExecutionsDS
                'AG 27/10/2011 - NOTE when required save CLOT alarm always but when CLOT detection is disabled do not treat it in ManageAlarms
                'If Not clotDetectionDisabled Then 'Treat field clot status only when clot detection is enabled
                If (myRotorName = "SAMPLES" AndAlso myClotStatus <> GlobalEnumerates.Ax00ArmClotDetectionValues.OK.ToString) Then
                    If (Not _analyzerManager.Alarms.Contains(GlobalEnumerates.Alarms.CLOT_SYSTEM_ERR)) Then 'Ignore ANSBM clot warnings if clot system err alarm exists 
                        'Implement business using the variables: myWellStatus, myPrepID, myClotStatus,...
                        'If not already read it ... read the execution related to the status arm now
                        If (myExecutionId = -1) Then
                            myGlobal = execDelg.GetExecutionByPreparationID(dbConnection, myPrepID, _analyzerManager.ActiveWorkSession, _analyzerManager.ActiveAnalyzer)
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                clotAffectedExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                                If (clotAffectedExecutionsDS.twksWSExecutions.Rows.Count > 0) Then
                                    myExecutionId = clotAffectedExecutionsDS.twksWSExecutions(0).ExecutionID
                                End If
                            End If
                        End If

                        'Mark the execution with ClotValue = myClotStatus
                        If (myExecutionId <> -1) Then
                            'AG 27/03/2012 - do not use myExecutionID ... when ise test several executions can share the same preparationID
                            For Each execRow As ExecutionsDS.twksWSExecutionsRow In clotAffectedExecutionsDS.twksWSExecutions.Rows
                                With execRow
                                    .BeginEdit()
                                    .ClotValue = myClotStatus
                                    .EndEdit()
                                End With
                            Next
                            'AG 27/03/2012
                            clotAffectedExecutionsDS.AcceptChanges()
                            myGlobal = execDelg.UpdateClotValue(dbConnection, clotAffectedExecutionsDS)
                        End If

                        If (myClotStatus = GlobalEnumerates.Ax00ArmClotDetectionValues.BS.ToString) Then 'Blocked system 
                            alarmClotDetectionEnum = GlobalEnumerates.Alarms.CLOT_DETECTION_ERR
                        ElseIf (myClotStatus = GlobalEnumerates.Ax00ArmClotDetectionValues.CD.ToString Or myClotStatus = GlobalEnumerates.Ax00ArmClotDetectionValues.CP.ToString) Then 'Clot Detected or Clot possible
                            alarmClotDetectionEnum = GlobalEnumerates.Alarms.CLOT_DETECTION_WARN
                        End If
                    End If

                ElseIf myRotorName = "SAMPLES" Then 'No clot found: If exists clear alarms 
                    'AG 25/07/2012 - these alarms has not OK
                    'alarmClotDetectionEnum = GlobalEnumerates.Alarms.CLOT_DETECTION_ERR
                    'PrepareLocalAlarmList(alarmClotDetectionEnum, False, alarmList, alarmStatusList)
                    '
                    'alarmClotDetectionEnum = GlobalEnumerates.Alarms.CLOT_DETECTION_WARN
                    'PrepareLocalAlarmList(alarmClotDetectionEnum, False, alarmList, alarmStatusList)
                End If
                'End If 'If Not clotDetectionDisabled Then

                If alarmList.Count > 0 Then
                    'Finally call manage all alarms detected (new or fixed)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If Not GlobalBase.IsServiceAssembly Then
                        Dim currentAlarms = New CurrentAlarms(_analyzerManager)
                        myGlobal = currentAlarms.Manage(dbConnection, alarmList, alarmStatusList, alarmAdditionalInfoList)
                    End If
                End If

                'Finally if exists executions locked due volume missing inform them into database after inform the other alarms!!
                'Inform the locked executions list (prepLockedDS)
                Dim wsAnAlarmsDelg As New WSAnalyzerAlarmsDelegate
                If Not myGlobal.HasError AndAlso Not lockedExecutionsDs Is Nothing Then
                    If lockedExecutionsDs.twksWSExecutions.Rows.Count > 0 Then
                        Dim alarmPrepLockedDS As New WSAnalyzerAlarmsDS  'AG 19/04/2011 - New DS with all preparation locked due this instruction
                        Dim i As Integer = 2 'AG 16/02/2012 - The prep_locked_warn starts with AlarmItem = 2 not 1 '1
                        Dim distinctOTlocked As List(Of Integer) = (From a As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDs.twksWSExecutions _
                                                                  Select a.OrderTestID Distinct).ToList
                        Dim firstExecutionLockedByOT As List(Of ExecutionsDS.twksWSExecutionsRow)
                        For Each itemOT As Integer In distinctOTlocked
                            firstExecutionLockedByOT = (From a As ExecutionsDS.twksWSExecutionsRow In lockedExecutionsDs.twksWSExecutions _
                                                        Where a.OrderTestID = itemOT Select a).ToList
                            If firstExecutionLockedByOT.Count > 0 Then
                                Dim alarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow
                                alarmRow = alarmPrepLockedDS.twksWSAnalyzerAlarms.NewtwksWSAnalyzerAlarmsRow
                                With alarmRow
                                    .AlarmID = GlobalEnumerates.Alarms.PREP_LOCKED_WARN.ToString
                                    .AnalyzerID = _analyzerManager.ActiveAnalyzer
                                    .AlarmDateTime = Now
                                    If _analyzerManager.ActiveWorkSession <> "" Then
                                        .WorkSessionID = _analyzerManager.ActiveWorkSession
                                    Else
                                        .SetWorkSessionIDNull()
                                    End If
                                    .AlarmStatus = True
                                    .AlarmItem = i

                                    'Create DATA with information of the locked execution (sample class, sampleid, test name, replicates,...)
                                    myGlobal = execDelg.EncodeAdditionalInfo(dbConnection, firstExecutionLockedByOT.First.AnalyzerID, firstExecutionLockedByOT.First.WorkSessionID, _
                                                                              firstExecutionLockedByOT.First.ExecutionID, myBottlePos, _
                                                                              GlobalEnumerates.Alarms.NONE, reagentNumberWithNoVolume)

                                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                        .AdditionalInfo = CType(myGlobal.SetDatos, String)
                                    Else
                                        .SetAdditionalInfoNull()
                                    End If

                                    .EndEdit()
                                End With
                                alarmPrepLockedDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow(alarmRow)

                                'WS Status refresh updates only the OrderTest affected, so we have to generate 1 uiRefreshrow for every ordertest affected
                                'AG 24/01/2012 - Prepare UIRefresh DS for WS status
                                uiRefreshMyGlobal = _analyzerManager.PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, firstExecutionLockedByOT.First.ExecutionID, 0, Nothing, False)

                                If i = 2 Then 'Alarms tab reload always all alarms, only inform the first 
                                    'AG 26/01/2012 - Prepare UIRefresh Dataset (NEW_ALARMS_RECEIVED) for refresh screen when needed
                                    uiRefreshMyGlobal = _analyzerManager.PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmRow.AlarmID.ToString, True)
                                End If

                                i = i + 1
                            End If
                        Next
                        firstExecutionLockedByOT = Nothing
                        distinctOTlocked = Nothing
                        'AG 18/06/2012

                        alarmPrepLockedDS.AcceptChanges()
                        myGlobal = wsAnAlarmsDelg.Save(dbConnection, alarmPrepLockedDS, alarmsDefintionTableDS) 'AG 24/07/2012 - change Create for Save
                    End If
                End If 'If Not myGlobal.HasError Then

                'AG 26/07/2012 - clot affected executions - Biochemical 1 execution for each ANSBM1 instruction, ISE several executions for each ANSBM1 instruction
                If Not myGlobal.HasError AndAlso Not clotAffectedExecutionsDS Is Nothing AndAlso clotAffectedExecutionsDS.twksWSExecutions.Count > 0 Then
                    Dim alarmPrepWithClotDS As New WSAnalyzerAlarmsDS  'AG 19/04/2011 - New DS with all preparation locked due this instruction
                    Dim i As Integer = 1 'AG 16/02/2012 - The clot_detection_err or clot_detection_warn starts with AlarmItem = 1

                    For Each item As ExecutionsDS.twksWSExecutionsRow In clotAffectedExecutionsDS.twksWSExecutions.Rows
                        Dim alarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow
                        alarmRow = alarmPrepWithClotDS.twksWSAnalyzerAlarms.NewtwksWSAnalyzerAlarmsRow
                        With alarmRow
                            .AlarmID = alarmClotDetectionEnum.ToString
                            .AnalyzerID = _analyzerManager.ActiveAnalyzer
                            .AlarmDateTime = Now
                            If _analyzerManager.ActiveWorkSession <> "" Then
                                .WorkSessionID = _analyzerManager.ActiveWorkSession
                            Else
                                .SetWorkSessionIDNull()
                            End If
                            .AlarmStatus = True
                            .AlarmItem = i

                            'Create DATA with information of the locked execution (sample class, sampleid, test name, replicates,...)
                            myGlobal = execDelg.EncodeAdditionalInfo(dbConnection, item.AnalyzerID, item.WorkSessionID, _
                                                                      item.ExecutionID, myBottlePos, _
                                                                      GlobalEnumerates.Alarms.NONE, reagentNumberWithNoVolume)
                            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                .AdditionalInfo = CType(myGlobal.SetDatos, String)
                            Else
                                .SetAdditionalInfoNull()
                            End If
                            .EndEdit()
                        End With
                        alarmPrepWithClotDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow(alarmRow)

                        'Prepare UIRefresh Dataset (NEW_ALARMS_RECEIVED) for refresh screen when needed
                        If i = 1 Then 'Alarms tab reload always all alarms, only inform the first 
                            uiRefreshMyGlobal = _analyzerManager.PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmRow.AlarmID.ToString, True)
                        End If

                        i = i + 1
                    Next
                    alarmPrepWithClotDS.AcceptChanges()
                    myGlobal = wsAnAlarmsDelg.Save(dbConnection, alarmPrepWithClotDS, alarmsDefintionTableDS) 'AG 24/07/2012 - change Create for Save
                End If
                'AG 26/07/2012


                'AG 08/06/2012 - Finally if Sw has the new WS instruction to be sent to analyzer in Running. Evaluate if is a test preparation (test, ptest or isetest)
                'In this case check the execution found continues PENDING, otherwise look for another execution

                If _analyzerManager.NextPreparationsToSend.Rows.Count > 0 Then

                    'Set to FALSE again when next preparation found belong to a NOT PENDING execution
                    Dim lookForNewInstructionToBeSent As Boolean = False
                    If reEvaluateNextToSend Then
                        'AG 04/10/2012 - R1 or S or R2 level detection failed but exists additional position the execution status changes from INPROCESS to PENDING. 
                        '                So remove previous search next estimation a searc again
                        lookForNewInstructionToBeSent = True

                    Else
                        With _analyzerManager.NextPreparationsAnalyzerManagerDS

                            'Priority: (1) rejected well, (2) contaminated well, (3) ise test, (4) reagent contamination, (5) std test
                            If Not .nextPreparation(0).IsCuvetteOpticallyRejectedFlagNull AndAlso Not .nextPreparation(0).CuvetteOpticallyRejectedFlag AndAlso _
                            Not .nextPreparation(0).IsCuvetteContaminationFlagNull AndAlso Not .nextPreparation(0).CuvetteContaminationFlag Then 'No cuvette rejected neither contaminated
                                Dim lookForExecutionStatus As Boolean = False

                                If Not .nextPreparation(0).IsExecutionTypeNull AndAlso .nextPreparation(0).ExecutionType = "PREP_ISE" Then
                                    lookForExecutionStatus = True
                                ElseIf Not .nextPreparation(0).IsReagentContaminationFlagNull AndAlso .nextPreparation(0).ReagentContaminationFlag Then
                                    lookForExecutionStatus = False
                                ElseIf Not .nextPreparation(0).IsExecutionTypeNull AndAlso .nextPreparation(0).ExecutionType = "PREP_STD" Then
                                    lookForExecutionStatus = True
                                End If

                                If lookForExecutionStatus Then
                                    If Not .nextPreparation(0).IsExecutionIDNull AndAlso .nextPreparation(0).ExecutionID <> GlobalConstants.NO_PENDING_PREPARATION_FOUND Then
                                        Dim myExecutionsDelg As New ExecutionsDelegate
                                        myGlobal = myExecutionsDelg.GetExecution(dbConnection, .nextPreparation(0).ExecutionID, _analyzerManager.ActiveAnalyzer, _analyzerManager.ActiveWorkSession)
                                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                            Dim localDS As New ExecutionsDS
                                            localDS = CType(myGlobal.SetDatos, ExecutionsDS)
                                            If localDS.twksWSExecutions.Rows.Count > 0 Then
                                                If localDS.twksWSExecutions(0).ExecutionStatus <> "PENDING" Then
                                                    lookForNewInstructionToBeSent = True 'Look for new exectuton
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End With
                    End If

                    If lookForNewInstructionToBeSent Then
                        Dim wellOffset As Integer = 0
                        If InStr(_analyzerManager.InstructionSent, "PTEST") > 0 Then 'Last instruction sent was a ptest
                            wellOffset = WELL_OFFSET_FOR_PREDILUTION
                        ElseIf InStr(_analyzerManager.InstructionSent, "ISETEST") > 0 Then 'Last instruction sent was an isetest
                            'Different offset depending the last execution sample type
                            If InStr(_analyzerManager.InstructionSent, "ISETEST;TI:1") > 0 Then
                                'SER or PLM
                                wellOffset = WELL_OFFSET_FOR_ISETEST_SERPLM

                            ElseIf InStr(_analyzerManager.InstructionSent, "ISETEST;TI:2") > 0 Then
                                'URI
                                wellOffset = WELL_OFFSET_FOR_ISETEST_URI
                            End If
                        End If
                        Dim reactRotorDlg As New ReactionsRotorDelegate
                        _analyzerManager.FutureRequestNextWellValue = reactRotorDlg.GetRealWellNumber(_analyzerManager.CurrentWell + 1 + wellOffset, MAX_REACTROTOR_WELLS) 'Estimation of future next well (last well received with Request + 1)
                        myGlobal = _analyzerManager.SearchNextPreparation(dbConnection, _analyzerManager.FutureRequestNextWellValue) 'Search for next instruction to be sent ... and sent it!!
                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then '(1)
                            _analyzerManager.NextPreparationsAnalyzerManagerDS = CType(myGlobal.SetDatos, AnalyzerManagerDS)
                        End If
                    End If

                End If

                GlobalBase.CreateLogActivity("Treat ARM STATUS Received (" & myInst.ToString & "): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessArmStatusRecived", EventLogEntryType.Information, False)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(String.Format("Message: {0} InnerException: {1}", ex.Message, ex.InnerException.Message), "AnalyzerManager.ProcessArmStatusRecived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        Private Function GetItem(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef myGlobal As GlobalDataTO, ByRef myInstParamTo As InstructionParameterTO, _
                                 ByVal itemPosition As Integer) As Boolean

            myGlobal = GetItemByParameterIndex(pInstructionReceived, itemPosition)
            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Return True
            End If
            Return False
        End Function
    End Class
End Namespace
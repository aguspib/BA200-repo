Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.CommunicationsSwFw

    Partial Public Class AnalyzerManager

#Region "Instructions to send Flow"

        ''' <summary>
        ''' Send the instruction and update flag for recovery results depending the entry parameter
        ''' </summary>
        ''' <param name="pMode"></param>
        ''' <returns></returns>
        ''' <remarks>AG 27/08/2012</remarks>
        Private Function RecoveryResultsAndStatus(ByVal pMode As GlobalEnumerates.Ax00PollRDAction) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                'AG 25/09/2012 - Disable waiting time control. Not use method because in running always leave active
                'InitializeTimerControl(WAITING_TIME_OFF)
                waitingTimer.Enabled = False

                'Send the next instruction
                myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.POLLRD, True, Nothing, pMode)

                'Update flags
                If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                    Select Case pMode
                        Case GlobalEnumerates.Ax00PollRDAction.PreparationsWithProblem
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.SoftwareWSonRUNNING, "CLOSED") 'AG 03/09/2012 current Work Session in Software is not RUNNING yet
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ResRecoverPrepProblems, "INI")
                        Case GlobalEnumerates.Ax00PollRDAction.Biochemical
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ResRecoverReadings, "INI")
                        Case GlobalEnumerates.Ax00PollRDAction.ISE
                            UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ResRecoverISE, "INI")
                    End Select

                    'Update analyzer session flags into DataBase
                    If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.RecoveryResultsAndStatus", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

#End Region

#Region "Treat recovery results"

        ''' <summary>
        ''' Current recovery results mode has finished. Sw has to evaluate received information and continue with next recovery result step
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 27/08/2012
        ''' Modified by: SA 19/12/2014 - Replaced sentences DirectCast(CInt(myInstParamTO.ParameterValue), Integer) by CInt(myInstParamTO.ParameterValue)
        '''                              due to the first one is redundant and produces building warnings</remarks>
        Private Function ProcessANSPRDReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Dim utilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO

                '''''''
                '(1) Get instruction fields
                '''''''
                ' Get the preparation identifier ID) field (parameter index 3)
                Dim recoveryMode As Integer = 0
                myGlobal = utilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If
                If IsNumeric(myInstParamTO.ParameterValue) Then
                    recoveryMode = CInt(myInstParamTO.ParameterValue)
                End If

                'AG 04/12/2013 - BT #1397 protection because when recovery is started in pause mode Softw has to send
                'START + END and at this point the flag analyzerIsReady is false, that causes the recover instruction flow stops
                If Not AnalyzerIsReadyAttribute Then
                    AnalyzerIsReadyAttribute = True
                End If
                'AG 04/12/2013

                'Tread recovered results
                myGlobal = ProcessResultsRecoveryInstructions(recoveryMode)

                'Update flags and continue with next step
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                'If Not myGlobal.HasError Then 'Comment, if ProcessResultsRecoveryInstructions continue with next recovery results step 
                Select Case recoveryMode
                    Case GlobalEnumerates.Ax00PollRDAction.PreparationsWithProblem
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ResRecoverPrepProblems, "END")
                        myGlobal = RecoveryResultsAndStatus(GlobalEnumerates.Ax00PollRDAction.Biochemical) 'Next recovery results step

                    Case GlobalEnumerates.Ax00PollRDAction.Biochemical
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ResRecoverReadings, "END")
                        myGlobal = RecoveryResultsAndStatus(GlobalEnumerates.Ax00PollRDAction.ISE) 'Next recovery results step

                    Case GlobalEnumerates.Ax00PollRDAction.ISE
                        UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.ResRecoverISE, "END")

                        'AG 03/09/2012- Previous version: Recovery instructions POLLRD were sent in STANDBY
                        'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess, "CLOSED")

                        ''Mark work session as aborted
                        'Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                        'myGlobal = myWSAnalyzerDelegate.UpdateWSStatus(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, "ABORTED")

                        ' ''Evaluate if ISE consumption must be updated. After activate ANSINF
                        ''myGlobal = ManageStandByStatus(GlobalEnumerates.AnalyzerManagerAx00Actions.STANDBY_END, CurrentWellAttribute)

                        ''Generate UI refresh for presentation - Inform the recovery results has finished!!
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.RECOVERY_RESULTS_STATUS, 0, True)

                        'Correction: Recovery instruction POLLRD are sent in RUNNING
                        myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STANDBY, True)
                        'AG 03/09/2012
                End Select
                'End If

                'Update analyzer session flags into DataBase
                If Not myGlobal.HasError AndAlso myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessANSPRDReceived", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' When each recovery mode (preparations with problems, readings missing, ISE results missing) finishes this method is called to treat the recovered information
        ''' </summary>
        ''' <param name="pMode"></param>
        ''' <returns></returns>
        ''' <remarks>AG Creation DD/09/2012 </remarks>
        Private Function ProcessResultsRecoveryInstructions(ByVal pMode As Integer) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                For i As Integer = 0 To bufferInstructionsRESULTSRECOVERYProcess.Count - 1
                    Select Case pMode
                        Case GlobalEnumerates.Ax00PollRDAction.PreparationsWithProblem
                            myGlobal = ProcessANSTINInstructionReceived(bufferInstructionsRESULTSRECOVERYProcess(i))

                        Case GlobalEnumerates.Ax00PollRDAction.Biochemical
                            myGlobal = ProcessANSPHRInstruction(bufferInstructionsRESULTSRECOVERYProcess(i))

                        Case GlobalEnumerates.Ax00PollRDAction.ISE
                            myGlobal = ProcessRecivedISEResult(bufferInstructionsRESULTSRECOVERYProcess(i))
                    End Select
                    'If myglobal.HasError  Then Exit For 'If error not exit. Treat all recovered results
                Next

                If pMode = GlobalEnumerates.Ax00PollRDAction.Biochemical Then
                    myGlobal = MarkContaminatedWellsAfterRecoveryResults()
                End If

                bufferInstructionsRESULTSRECOVERYProcess.Clear()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessResultsRecoveryInstructions", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Evaluate which preparations performed after communications error have left the used well (cuvette) contaminated for the next turn
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>AG 25/09/2012</remarks>
        Private Function MarkContaminatedWellsAfterRecoveryResults() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                '1 - Look for minimum ID (but higher than 0) in bufferInstructionsRESULTSRECOVERYProcess(0) --> minPrepID
                Dim minPrepID As Integer = 0
                If recoveredPrepIDList.Count > 0 Then
                    minPrepID = recoveredPrepIDList.Min

                    If minPrepID > 0 Then
                        '2 - Look for contaminator cuvette tests with ID highter or equal than minPrepID
                        Dim myPrepDelegate As New WSPreparationsDelegate
                        myGlobal = myPrepDelegate.ReadCuvettesContaminatedAfterRecoveryResults(Nothing, minPrepID)
                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                            Dim cuvettesContaminatedDS As New WSPreparationsDS
                            cuvettesContaminatedDS = CType(myGlobal.SetDatos, WSPreparationsDS)

                            If cuvettesContaminatedDS.twksWSPreparations.Rows.Count > 0 Then
                                '3 - Mark the affected cuvettes as contaminated for the next turn
                                Dim myReactionsDelegate As New ReactionsRotorDelegate
                                myGlobal = myReactionsDelegate.GetAllWellsLastTurn(Nothing, AnalyzerIDAttribute)
                                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                    Dim wellsDS As ReactionsRotorDS
                                    wellsDS = CType(myGlobal.SetDatos, ReactionsRotorDS)

                                    Dim resLinq As List(Of ReactionsRotorDS.twksWSReactionsRotorRow)
                                    Dim toUpdateDS As New ReactionsRotorDS
                                    For Each contaminatedRow As WSPreparationsDS.twksWSPreparationsRow In cuvettesContaminatedDS.twksWSPreparations.Rows
                                        If Not contaminatedRow.IsWellUsedNull Then
                                            resLinq = (From a As ReactionsRotorDS.twksWSReactionsRotorRow In wellsDS.twksWSReactionsRotor _
                                                       Where a.WellNumber = contaminatedRow.WellUsed Select a Order By a.RotorTurn Descending).ToList 'Get the last rotor turn of the affected well

                                            'Mark the well as contaminated
                                            If resLinq.Count > 0 Then
                                                Dim newRow As ReactionsRotorDS.twksWSReactionsRotorRow
                                                newRow = toUpdateDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow
                                                newRow.BeginEdit()
                                                newRow.AnalyzerID = AnalyzerIDAttribute
                                                newRow.WellNumber = contaminatedRow.WellUsed
                                                newRow.WellContent = "C"
                                                newRow.WashedFlag = False
                                                newRow.WashRequiredFlag = True
                                                newRow.TestID = contaminatedRow.TestID
                                                If Not contaminatedRow.IsWashingSolutionR1Null Then newRow.WashingSolutionR1 = contaminatedRow.WashingSolutionR1 Else newRow.SetWashingSolutionR1Null()
                                                If Not contaminatedRow.IsWashingSolutionR2Null Then newRow.WashingSolutionR2 = contaminatedRow.WashingSolutionR2 Else newRow.SetWashingSolutionR2Null()
                                                newRow.EndEdit()
                                                toUpdateDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(newRow)
                                            End If
                                            toUpdateDS.AcceptChanges()
                                        End If
                                    Next
                                    resLinq = Nothing

                                    'Update only last turn
                                    If toUpdateDS.twksWSReactionsRotor.Rows.Count > 0 Then
                                        myGlobal = myReactionsDelegate.Update(Nothing, toUpdateDS, True)
                                    End If

                                End If 'If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                            End If 'If cuvettesContaminatedDS.twksWSPreparations.Rows.Count > 0 Then

                        End If 'If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                    End If 'If minPrepID > 0 Then

                End If 'If recoveredPrepIDList.Count > 0 Then

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.MarkContaminatedWellsAfterRecoveryResults", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#End Region

#Region "Preparations with problems treatment"

        ''' <summary>
        ''' Treat each instruction received with preparations with problems data:
        ''' - Volume missing treatment
        ''' - Clot warnings treatment
        ''' - Arm collision errors treatment
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>AG 27/08/2012</remarks>
        Private Function ProcessANSTINInstructionReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Decode instruction and prepare a PrepWithProblemTO
                myGlobal = DecodeANSTINInstruction(pInstructionReceived)

                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                    Dim problematicPrep As New List(Of PrepWithProblemTO)
                    problematicPrep = CType(myGlobal.SetDatos, List(Of PrepWithProblemTO))

                    For Each item As PrepWithProblemTO In problematicPrep
                        If Not myGlobal.HasError Then
                            myGlobal = ProcessLevelDetectionDuringRecover(item)
                        Else
                            Exit For
                        End If

                        If Not myGlobal.HasError Then
                            myGlobal = ProcessClotDetectionDuringRecover(item)
                        Else
                            Exit For
                        End If

                        If Not myGlobal.HasError Then
                            myGlobal = ProcessArmCollisionDuringRecover(item)
                        Else
                            Exit For
                        End If
                    Next
                    problematicPrep = Nothing 'AG 25/02/2014 - #1521
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessANSTINInstructionReceived", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function



        ''' <summary>
        ''' Decode the ANSTIN instruction, fill a PrepWithProblemTO and return data
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns>GlobalDataTo with data as List (of PrepWithProblemTO))</returns>
        ''' <remarks>
        ''' Created by:  AG 19/09/2012
        ''' Modified by: SA 19/12/2014 - Replaced sentences DirectCast(CInt(myInstParamTO.ParameterValue), Integer) by CInt(myInstParamTO.ParameterValue)
        '''                              due to the first one is redundant and produces building warnings
        ''' </remarks>
        Private Function DecodeANSTINInstruction(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim dataToReturn As New List(Of PrepWithProblemTO)

                'Dim Utilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO

                '1) Get the number of preparations with problems ID) field (parameter index 3)
                Dim valueN As Integer = 0
                myGlobal = Utilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        valueN = CInt(myInstParamTO.ParameterValue)
                    Else
                        myGlobal.HasError = True
                    End If
                End If

                If Not myGlobal.HasError Then

                    Dim myParametersDlg As New SwParametersDelegate
                    Dim sampleLevelDetectionKO As Long = 0 'Level detection KOs
                    Dim r1LevelDetectionKO As Long = 0
                    Dim r2LevelDetectionKO As Long = 0
                    Dim diluentLevelDetectionKO As Long = 0
                    Dim sampleToDiluteLevelDetectionKO As Long = 0
                    Dim dilutedSampleLevelDetectionKO As Long = 0
                    Dim r1ContaminationRiskKO As Long = 0
                    Dim r2ContaminationRiskKO As Long = 0

                    Dim clotDetectedKO As Long = 0 'Clot KOs
                    Dim clotPossibleKO As Long = 0

                    Dim sampleArmCollsionKO As Long = 0 'Collision KOs
                    Dim R1ArmCollsionKO As Long = 0
                    Dim R2ArmCollsionKO As Long = 0

                    '2) Read the Sw parameters with the preparations KO possible error codes
                    myGlobal = myParametersDlg.ReadByAnalyzerModel(Nothing, String.Empty, AnalyzerIDAttribute)
                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        Dim localParameters As New ParametersDS
                        localParameters = CType(myGlobal.SetDatos, ParametersDS)

                        Dim resLinQ As List(Of ParametersDS.tfmwSwParametersRow)
                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_SAMPLE_FAILED.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            sampleLevelDetectionKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_R1_FAILED.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            r1LevelDetectionKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_R2_FAILED.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            r2LevelDetectionKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_DILUTIONSOL_FAILED.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            diluentLevelDetectionKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_SAMPLETODILUTE_FAILED.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            sampleToDiluteLevelDetectionKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_DILUTEDSAMPLE_FAILED.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            dilutedSampleLevelDetectionKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_R1_CONTAMINATIONRISK.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            r1ContaminationRiskKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_R2_CONTAMINATIONRISK.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            r2ContaminationRiskKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_CLOT_DETECTED.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            clotDetectedKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_CLOT_POSSIBLE.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            clotPossibleKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_SAMPLE_COLLISION.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            sampleArmCollsionKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_R1_COLLISION.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            R1ArmCollsionKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = (From a As ParametersDS.tfmwSwParametersRow In localParameters.tfmwSwParameters _
                                   Where a.ParameterName = GlobalEnumerates.SwParameters.RECOVERY_R2_COLLISION.ToString Select a).ToList
                        If resLinQ.Count > 0 Then
                            R2ArmCollsionKO = CLng(resLinQ(0).ValueNumeric)
                        Else
                            myGlobal.HasError = True
                        End If

                        resLinQ = Nothing
                    End If


                    '3) Get the preparations with problems and decode them
                    If Not myGlobal.HasError Then
                        Dim loopOffset As Integer = 2 'ANSTIN instruction has a loop of 2 fields ID and EC
                        Dim execDlg As New ExecutionsDelegate
                        Dim prepDlg As New WSPreparationsDelegate

                        For index As Integer = 0 To valueN - 1

                            '3.1) Get the preparationID with problems field (parameter index 4)
                            Dim prepID As Integer = 0
                            myGlobal = Utilities.GetItemByParameterIndex(pInstructionReceived, 4 + index * loopOffset)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

                                If IsNumeric(myInstParamTO.ParameterValue) Then
                                    prepID = CInt(myInstParamTO.ParameterValue)
                                Else
                                    Exit For
                                End If
                            Else
                                Exit For
                            End If

                            '3.2) Get the error code field (parameter index 5)
                            Dim errorCode As Long = 0
                            myGlobal = Utilities.GetItemByParameterIndex(pInstructionReceived, 5 + index * loopOffset)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

                                If IsNumeric(myInstParamTO.ParameterValue) Then
                                    errorCode = CLng(myInstParamTO.ParameterValue)
                                Else
                                    Exit For
                                End If
                            Else
                                Exit For
                            End If

                            '3.4) Get affected executions related with ID (preparationID), evaluate warnings and prepare data to return (list of (PrepWithProblemTO))
                            myGlobal = execDlg.GetExecutionByPreparationID(Nothing, prepID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                Dim dsExec As New ExecutionsDS
                                dsExec = CType(myGlobal.SetDatos, ExecutionsDS)

                                For Each row As ExecutionsDS.twksWSExecutionsRow In dsExec.twksWSExecutions.Rows
                                    If Not row.IsExecutionIDNull Then
                                        'Create new element for list to return
                                        Dim prepKO As New PrepWithProblemTO
                                        prepKO.PreparationID = prepID
                                        prepKO.ExecutionID = row.ExecutionID

                                        'Look for level detection KOs
                                        If (errorCode And sampleLevelDetectionKO) = sampleLevelDetectionKO Then
                                            prepKO.SampleLevelDetectionKO = True
                                            'Look for DEPLETED sample tube position
                                            myGlobal = prepDlg.GetDepletedPositionDuringRecoveryResults(Nothing, prepID, "SAMPLE")
                                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                                prepKO.SamplePositionKO = CType(myGlobal.SetDatos, Integer)
                                            End If
                                        End If

                                        If (errorCode And r1LevelDetectionKO) = r1LevelDetectionKO Then
                                            prepKO.R1LevelDetectionKO = True
                                            'Look for DEPLETED reagent bottle position
                                            myGlobal = prepDlg.GetDepletedPositionDuringRecoveryResults(Nothing, prepID, "R1")
                                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                                prepKO.R1PositionKO = CType(myGlobal.SetDatos, Integer)
                                            End If
                                        End If

                                        If (errorCode And r2LevelDetectionKO) = r2LevelDetectionKO Then
                                            prepKO.R2LevelDetectionKO = True
                                            'Look for DEPLETED reagent bottle position
                                            myGlobal = prepDlg.GetDepletedPositionDuringRecoveryResults(Nothing, prepID, "R2")
                                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                                prepKO.R2PositionKO = CType(myGlobal.SetDatos, Integer)
                                            End If
                                        End If

                                        If (errorCode And diluentLevelDetectionKO) = diluentLevelDetectionKO Then
                                            prepKO.DiluentLevelDetectionKO = True
                                            'Look for DEPLETED diluent bottle position
                                            myGlobal = prepDlg.GetDepletedPositionDuringRecoveryResults(Nothing, prepID, "DILUENT")
                                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                                prepKO.DiluentPositionKO = CType(myGlobal.SetDatos, Integer)
                                            End If
                                        End If

                                        If (errorCode And sampleToDiluteLevelDetectionKO) = sampleToDiluteLevelDetectionKO Then
                                            prepKO.SampleToDiluteLevelDetectionKO = True
                                            'Look for DEPLETED sample tube position to dilute
                                            myGlobal = prepDlg.GetDepletedPositionDuringRecoveryResults(Nothing, prepID, "SAMPLETODILUTE")
                                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                                prepKO.SampleToDilutePositionKO = CType(myGlobal.SetDatos, Integer)
                                            End If
                                        End If

                                        If (errorCode And dilutedSampleLevelDetectionKO) = dilutedSampleLevelDetectionKO Then
                                            prepKO.DilutedSampleLevelDetectionKO = True
                                            'This KO is on the reactions rotor, so no position has to be found
                                        End If

                                        If (errorCode And r1ContaminationRiskKO) = r1ContaminationRiskKO Then
                                            prepKO.R1ContaminationRiskKO = True
                                            'v1.0 Impossible to know the washsolution position
                                        End If

                                        If (errorCode And r2ContaminationRiskKO) = r2ContaminationRiskKO Then
                                            prepKO.R2ContaminationRiskKO = True
                                            'v1.0 Impossible to know the washsolution position
                                        End If

                                        'Look for clot detection KOs
                                        If (errorCode And clotDetectedKO) = clotDetectedKO Then prepKO.ClotDetectionKO = True
                                        If (errorCode And clotPossibleKO) = clotPossibleKO Then prepKO.ClotPossibleKO = True

                                        'Loop for arm collision KOs
                                        If (errorCode And sampleArmCollsionKO) = sampleArmCollsionKO Then prepKO.SampleCollisionKO = True
                                        If (errorCode And R1ArmCollsionKO) = R1ArmCollsionKO Then prepKO.R1CollisionKO = True
                                        If (errorCode And R2ArmCollsionKO) = R2ArmCollsionKO Then prepKO.R2CollisionKO = True

                                        'Finally add to list to return
                                        dataToReturn.Add(prepKO)


                                    End If 'If Not row.IsExecutionIDNull Then
                                Next 'For Each row As ExecutionsDS.twksWSExecutionsRow In dsExec.twksWSExecutions.Rows
                                dsExec.Clear()
                                dsExec = Nothing
                            End If

                        Next 'For index As Integer = 0 To valueN - 1

                    End If

                End If

                If Not myGlobal.HasError Then
                    myGlobal.SetDatos = dataToReturn
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.DecodeANSTINInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Preparation with problems decoded (error flags and when required also the positions)
        ''' </summary>
        ''' <param name="pPrepWithKO"></param>
        ''' <returns>GlobalDataTo with data</returns>
        ''' <remarks>IR 20/09/2012</remarks>
        Private Function ProcessLevelDetectionDuringRecover(ByVal pPrepWithKO As PrepWithProblemTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim myExecutions As New ExecutionsDelegate

            Try
                Dim turnToPendingFlag As Boolean = False

                'Preparation KO: R1 failed
                If pPrepWithKO.R1LevelDetectionKO Then
                    myGlobal = myExecutions.ProcessVolumeMissing(Nothing, pPrepWithKO.PreparationID, "REAGENTS", pPrepWithKO.R1PositionKO, False, WorkSessionIDAttribute, AnalyzerIDAttribute, turnToPendingFlag)
                End If

                'Preparation KO: Diluent failed
                If pPrepWithKO.DiluentLevelDetectionKO Then
                    myGlobal = myExecutions.ProcessVolumeMissing(Nothing, pPrepWithKO.PreparationID, "REAGENTS", pPrepWithKO.DiluentPositionKO, False, WorkSessionIDAttribute, AnalyzerIDAttribute, turnToPendingFlag)
                End If

                'Preparation KO: Sample failed
                If pPrepWithKO.SampleLevelDetectionKO Then
                    myGlobal = myExecutions.ProcessVolumeMissing(Nothing, pPrepWithKO.PreparationID, "SAMPLES", pPrepWithKO.SamplePositionKO, False, WorkSessionIDAttribute, AnalyzerIDAttribute, turnToPendingFlag)
                End If

                'Preparation KO: Sample to dilute failed
                If pPrepWithKO.SampleToDiluteLevelDetectionKO Then
                    myGlobal = myExecutions.ProcessVolumeMissing(Nothing, pPrepWithKO.PreparationID, "SAMPLES", pPrepWithKO.SampleToDilutePositionKO, False, WorkSessionIDAttribute, AnalyzerIDAttribute, turnToPendingFlag)
                End If

                'Preparation KO: R2 failed
                If (pPrepWithKO.R2LevelDetectionKO) Then
                    myGlobal = myExecutions.ProcessVolumeMissing(Nothing, pPrepWithKO.PreparationID, "REAGENTS", pPrepWithKO.R2PositionKO, False, WorkSessionIDAttribute, AnalyzerIDAttribute, turnToPendingFlag)
                End If

                'Preparation KO: Diluted sample in reactions rotor failed
                If (pPrepWithKO.DilutedSampleLevelDetectionKO) Then
                    myGlobal = myExecutions.UpdateStatusByExecutionID(Nothing, "LOCKED", pPrepWithKO.ExecutionID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessLevelDetectionDuringRecover", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' Preparation with clot detections problems decoded (error flags and when required also the positions)
        ''' </summary>
        ''' <param name="pPrepWithKO"></param>
        ''' <returns>GlobalDataTo with data</returns>
        ''' <remarks>IR 20/09/2012</remarks>
        Private Function ProcessClotDetectionDuringRecover(ByVal pPrepWithKO As PrepWithProblemTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim myExecutions As New ExecutionsDelegate
            Dim clotAffectedExecutionsDS As New ExecutionsDS
            Dim myClotStatus As String = ""

            Try
                If ((pPrepWithKO.ClotDetectionKO) OrElse (pPrepWithKO.ClotPossibleKO)) Then
                    myGlobal = myExecutions.GetExecutionByPreparationID(Nothing, pPrepWithKO.PreparationID, WorkSessionIDAttribute, AnalyzerIDAttribute)

                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        clotAffectedExecutionsDS = CType(myGlobal.SetDatos, ExecutionsDS)
                        If clotAffectedExecutionsDS.twksWSExecutions.Rows.Count > 0 Then

                            If pPrepWithKO.ClotDetectionKO Then
                                myClotStatus = GlobalEnumerates.Ax00ArmClotDetectionValues.CD.ToString
                            Else
                                myClotStatus = GlobalEnumerates.Ax00ArmClotDetectionValues.CP.ToString
                            End If

                            'Mark the execution with ClotValue = myClotStatus
                            For Each execRow As ExecutionsDS.twksWSExecutionsRow In clotAffectedExecutionsDS.twksWSExecutions.Rows
                                With execRow
                                    .BeginEdit()
                                    .ClotValue = myClotStatus
                                    .EndEdit()
                                End With
                            Next

                            clotAffectedExecutionsDS.AcceptChanges()
                            myGlobal = myExecutions.UpdateClotValue(Nothing, clotAffectedExecutionsDS)

                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessClotDetectionDuringRecover", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' Preparation with arm collision problems decoded (error flags and when required also the positions)
        ''' </summary>
        ''' <param name="pPrepWithKO"></param>
        ''' <returns>GlobalDataTo with data</returns>
        ''' <remarks>IR 20/09/2012</remarks>
        Private Function ProcessArmCollisionDuringRecover(ByVal pPrepWithKO As PrepWithProblemTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim myExecutions As New ExecutionsDelegate
            Dim collisionAffectedExecutionsDS As New ExecutionsDS
            Dim myExecutionID As Integer = -1
            Dim rcp_del As New WSRotorContentByPositionDelegate

            Try
                If ((pPrepWithKO.R1CollisionKO) OrElse (pPrepWithKO.R2CollisionKO) OrElse (pPrepWithKO.SampleCollisionKO)) Then
                    myGlobal = myExecutions.GetExecutionByPreparationID(Nothing, pPrepWithKO.PreparationID, WorkSessionIDAttribute, AnalyzerIDAttribute)

                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        collisionAffectedExecutionsDS = CType(myGlobal.SetDatos, ExecutionsDS)
                        If collisionAffectedExecutionsDS.twksWSExecutions.Rows.Count > 0 Then
                            'Mark the affected execution status as LOCKED or PENDING
                            For Each execRow As ExecutionsDS.twksWSExecutionsRow In collisionAffectedExecutionsDS.twksWSExecutions.Rows
                                If Not execRow.IsExecutionIDNull AndAlso Not execRow.IsExecutionStatusNull AndAlso execRow.ExecutionStatus = "INPROCESS" Then
                                    myGlobal = myExecutions.ChangeINPROCESSStatusByCollision(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, execRow.ExecutionID)

                                    If Not myGlobal.HasError Then
                                        myGlobal = rcp_del.UpdateSamplePositionStatus(Nothing, execRow.ExecutionID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                                    End If
                                End If

                                If myGlobal.HasError Then
                                    Exit For
                                End If
                            Next

                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessArmCollisionDuringRecover", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

#End Region

    End Class

End Namespace

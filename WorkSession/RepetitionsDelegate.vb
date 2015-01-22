'Created by: AG 30/04/2010

Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types.ExecutionsDS
Imports Biosystems.Ax00.Global.GlobalEnumerates


Namespace Biosystems.Ax00.BL
    Public Class RepetitionsDelegate

#Region "Private Structures"

        Private Structure RepetitionCriterion
            'General information
            Dim TestID As Integer
            Dim SampleClass As String
            Dim SampleType As String

            'Results data
            Dim Conc As Single
            Dim Abs As Single
            Dim ConcErrors As String
            Dim AbsErrors As String
            Dim SubstrateDepletion As Integer
            Dim LinealKinetics As Boolean

            'Test programming data
            Dim KineticTest As Boolean
            Dim LinearityLimit As Single
            Dim DetectionLimit As Single
            Dim RepetitionLimitMax As Single
            Dim RepetitionLimitMin As Single
            Dim AutomaticRerun As Boolean
        End Structure

        Private Enum CriterionResult
            NO_REP   ' NO REPETITION
            NONE     'Criterion equal (postdilution NONE)
            INC      'Criterion increased
            RED      'Criterion reduced
        End Enum
#End Region

#Region "Private variables"
        Private myRepetitionCriterion As RepetitionCriterion
#End Region

#Region "Public methods"
        ''' <summary>
        ''' Verify if it is needed to add a new repetition for the specified OrderTest in the WorkSession, and in that case, add the rerun.
        ''' Once the Rerun has been added, parameter pNewSampleStatus is updated with the value set to the related Rotor Positions
        ''' ** For Automatic Reruns:
        '''       - Get value of flag Automatic Rerun from the programming of the correspondent Test
        '''       - If AutomaticRerun = True, calculate the criteria for automatic repetition of the Test
        '''       - If it is needed, add a new request for a Test Rerun in the WorkSession using the proper Rerun criteria
        ''' ** For Manual Reruns (requested from WSResults Screen): 
        '''       -Add a new request for Test Rerun in the WorkSession using the proper Rerun criteria
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Value of the last Rerun requested for the informed Order Test</param>
        ''' <param name="pRepetitionCreatedFlag">By Reference parameter set to TRUE when the Executions for the new Rerun of the informed OrderTest have been created</param>
        ''' <param name="pNewSampleStatus">By Reference parameter used to return the status of the Rotor Positions in which the related tubes are placed</param>
        ''' <param name="pRunningMode">TRUE: analyzer is in running mode, FALSE: not</param>
        ''' <param name="pManualRepetitionFlag">Flag indicating the rerun was requested manually in the Results Review screen</param>
        ''' <param name="pManualRepetitionCriterium">For manual reruns, this parameter indicates the rerun criteria selected</param>
        ''' <param name="pIsISEModuleReady">Flag indicating if the ISE Module is ready to be used. Optional parameter</param>
        ''' <param name="pLISDownloadProcess"></param>
        ''' <returns>GlobalDataTO indicating if an error has occurred or not. SetDatos as List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)</returns>
        ''' <remarks>
        ''' Created by:  AG 30/04/2010 
        ''' Modified by: DL 06/05/2010
        '''              AG 07/05/2010 - Added AnalyzerID and WorkSessionID as parameters
        '''              AG 10/05/2010 - Added DB Connection parameter and use the METHOD TEMPLATE
        '''              AG 05/07/2011 - Update status of Rotor Positions to PENDING for the added rerun
        '''              AG 18/07/2011 - Inform new Executions have been created by setting parameter pRepetitionCreatedFlag to TRUE
        '''              SA 27/08/2010 - Call to function CreateWSExecutionsByOrderTest replaced by call to CreateWSExecutions
        '''              SA 30/01/2012 - Rollback and Connection closing were missing in the Catch block. 
        '''                            - If the Required Element needed for the OrderTestID is marked as FINISHED, unmark it (set ElementFinished = False)
        '''              AG 03/07/2012 - Do not open Transaction and define local dbConnection as Nothing
        '''              SA 26/07/2012 - Added optional parameter to indicate if the ISE Module is ready to be used; when it is not ready, the status 
        '''                              of all pending ISE Executions has to be changed to LOCKED
        '''              AG 03/06/2013 - BT #1170 ==> Added code to ensure the function returns always a List of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow
        '''                                           in the SetDatos component of the GlobalDataTO
        '''              SA 17/03/2014 - BT #1536 ==> Added new optional parameter pLISDownloadProcess with default value FALSE. When value of this parameter 
        '''                                           is TRUE, the DBConnection received as parameter is used instead of Nothing (to solve the SQL TimeOut Expired
        '''                                           error that is raised in function OrdersDelegate.UpdateOrderStatus when this function is called inside the process
        '''                                           of Orders download from LIS)
        '''              AG 31/03/2014 - #1565 add new required parameter pRunningMode
        ''' </remarks>
        Public Function ManageRepetitions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                          ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer, ByRef pRepetitionCreatedFlag As Boolean, _
                                          ByRef pNewSampleStatus As String, ByVal pRunningMode As Boolean, Optional ByVal pManualRepetitionFlag As Boolean = False, _
                                          Optional ByVal pManualRepetitionCriterium As String = "NONE", Optional ByVal pIsISEModuleReady As Boolean = False, _
                                          Optional ByVal pLISDownloadProcess As Boolean = False) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'BT #1536 - When value of parameter pLISDownloadProcess is TRUE, the DBConnection received as parameter is used instead of Nothing
                If (pLISDownloadProcess) Then dbConnection = pDBConnection

                pNewSampleStatus = ""
                Dim myAutoRepCriterion As String = CriterionResult.NO_REP.ToString

                If (Not pManualRepetitionFlag) Then
                    'Calculate the criteria for automatic Rerun
                    Me.Init(dbConnection, pOrderTestID, pRerunNumber)
                    If (myRepetitionCriterion.AutomaticRerun AndAlso pRerunNumber = 1) Then
                        myGlobal = Me.CalculateAutoRepetitionCriterion()

                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            myAutoRepCriterion = DirectCast(myGlobal.SetDatos, String)
                        End If
                    End If
                Else
                    'Use as Rerun criteria the informed by user when the Manual Rerun was requested
                    myAutoRepCriterion = pManualRepetitionCriterium
                End If

                If (pManualRepetitionFlag OrElse myAutoRepCriterion <> CriterionResult.NO_REP.ToString) Then
                    'Update field SentNewRerunPostdilution with value of the Rerun Criteria in the related Executions
                    Dim myExecutionDelegate As New ExecutionsDelegate
                    myGlobal = myExecutionDelegate.UpdateNewRerunSentPostdilutionType(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, myAutoRepCriterion)

                    If (Not myGlobal.HasError) Then

                        'AG 31/03/2014 - #1565 add trace + inform the analyzer status!!
                        'Create Executions for the Rerun (without deleting the not in curse ones --> set parameter pWorkInRunningMode to FALSE)
                        'myGlobal = myExecutionDelegate.CreateWSExecutions(dbConnection, pAnalyzerID, pWorkSessionID, False, pOrderTestID, myAutoRepCriterion, pIsISEModuleReady)
                        'Dim myLogAcciones As New ApplicationLogManager()
                        GlobalBase.CreateLogActivity("Launch CreateWSExecutions !", "AnalyzerManager.ManageRepetitions", EventLogEntryType.Information, False)
                        Debug.Print("Manage Repetitions!!! - RunningMode: " & pRunningMode)
                        'NOTE: AG 30/05/2014 #1644 - No changes made here! When add reruns the new parameter pauseMode is not required

                        'Use NOTHING as ISEElectrodes, False as pauseMode (do not affect for reruns and  pManualRerun parameter)
                        myGlobal = myExecutionDelegate.CreateWSExecutions(dbConnection, pAnalyzerID, pWorkSessionID, pRunningMode, pOrderTestID, myAutoRepCriterion, pIsISEModuleReady, Nothing, False, pManualRepetitionFlag)
                        'AG 31/03/2014 - #1565

                        'If the Required Element needed for the OrderTestID is marked as FINISHED, unmark it
                        If (Not myGlobal.HasError) Then
                            Dim myOrderTestsDelegate As New OrderTestsDelegate
                            myGlobal = myOrderTestsDelegate.GetElementByOrderTestID(dbConnection, pWorkSessionID, pAnalyzerID, pOrderTestID)

                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                Dim myReqElementDS As WSRequiredElementsDS = DirectCast(myGlobal.SetDatos, WSRequiredElementsDS)

                                If (myReqElementDS.twksWSRequiredElements.Count > 0) AndAlso (myReqElementDS.twksWSRequiredElements.First.ElementFinished) Then
                                    Dim myRequiredElemDelegate As New WSRequiredElementsDelegate
                                    myGlobal = myRequiredElemDelegate.UpdateElementFinished(dbConnection, myReqElementDS.twksWSRequiredElements.First.ElementID, False)
                                End If
                            End If
                        End If

                        'Change the status of all SAMPLES Rotor Positions containing tubes of the sample needed to repeat the Test
                        If (Not myGlobal.HasError) Then
                            Dim rcpDelegate As New WSRotorContentByPositionDelegate
                            myGlobal = rcpDelegate.GetRotorPositionsByOrderTestID(dbConnection, pOrderTestID, True, True)

                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                Dim rcpDS As WSRotorContentByPositionDS = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS)

                                'Set to PENDING all positions marked as FINISHED
                                For Each rotorPos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rcpDS.twksWSRotorContentByPosition
                                    rotorPos.BeginEdit()
                                    If (rotorPos.Status = "FINISHED") Then rotorPos.Status = "PENDING"
                                    pNewSampleStatus = rotorPos.Status
                                    rotorPos.EndEdit()
                                Next
                                rcpDS.AcceptChanges()

                                'Get all positions marked as PENDING
                                Dim linqList As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                linqList = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rcpDS.twksWSRotorContentByPosition _
                                           Where a.Status = "PENDING" _
                                          Select a).ToList

                                'Update to PENDING the status of the Rotor Cell
                                For Each rotorPos As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In linqList
                                    myGlobal = rcpDelegate.UpdateByRotorTypeAndCellNumber(dbConnection, pAnalyzerID, pWorkSessionID, "SAMPLES", _
                                                                                          rotorPos.CellNumber, rotorPos.Status, 0, 0, True, False)
                                    If (myGlobal.HasError) Then Exit For
                                Next

                                If (Not myGlobal.HasError) Then
                                    'Return the list of Rotor Positions with Status PENDING and inform that new Executions have been created by
                                    'setting value of parameter pRepetitionCreatedFlag to TRUE
                                    myGlobal.SetDatos = linqList
                                    pRepetitionCreatedFlag = True
                                End If
                                linqList = Nothing
                            End If
                        End If
                    End If

                Else 'AG 03/06/2013 - fix bug #1170 (add protection method returns only one data type list of RotorPositionsDS.RotorPositionsRow)
                    Dim emptyList As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                    myGlobal.SetDatos = emptyList
                    emptyList = Nothing
                End If
                '    End If
                'End If

                If (Not myGlobal.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    'If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    'If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                'If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "RepetitionsDelegate.ManageRepetitions", EventLogEntryType.Error, False)
            Finally
                'If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Add all the Repetitions pending to be added
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pRunningMode">TRUE analyzer is in running mode</param>
        ''' <param name="pIsISEModuleReady">Flag indicating if the ISE Module is ready to be used. Optional parameter</param>
        ''' <returns>GlobalDataTO indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by:  SG 16/07/2010
        ''' Modified by: SA 30/01/2012 - Rollback and Connection closing were missing in the Catch block
        '''              SA 26/07/2012 - Added optional parameter to indicate if the ISE Module is ready to be used; when it is not ready, the status 
        '''                              of all pending ISE Executions has to be changed to LOCKED 
        '''              AG 31/03/2014 - #1565 add new required parameter pRunningMode      
        ''' </remarks>
        Public Function AddManualRepetitions(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                             ByVal pRunningMode As Boolean, Optional ByVal pIsISEModuleReady As Boolean = False) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim StartTime As DateTime = Now
                'Dim myLogAcciones1 As New ApplicationLogManager()
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                Dim myWSRepetitionsToAddDelegate As New WSRepetitionsToAddDelegate

                myGlobal = myWSRepetitionsToAddDelegate.ReadAll(dbConnection, pAnalyzerID, pWorkSessionID)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    Dim pendingRepToAdd As WSRepetitionsToAddDS = DirectCast(myGlobal.SetDatos, WSRepetitionsToAddDS)

                    'TR 19/07/2012 - Verify if there is at least a Control requested in the WS
                    Dim myCtrlSendingGroupNumber As Integer = 0
                    Dim myWSOrderTestDelegate As New WSOrderTestsDelegate
                    Dim myPendingRepToAddList As List(Of WSRepetitionsToAddDS.twksWSRepetitionsToAddRow) = (From a In pendingRepToAdd.twksWSRepetitionsToAdd _
                                                                                                       Where Not a.IsSampleClassNull _
                                                                                                         AndAlso a.SampleClass = "CTRL" _
                                                                                                          Select a).ToList()
                    If (myPendingRepToAddList.Count > 0) Then
                        'Get Max CtrlSendingGroup and add one to it to obtain the next CtrlSendingGroup
                        myGlobal = myWSOrderTestDelegate.GetMaxCtrlsSendingGroup(pDBConnection, pWorkSessionID)
                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            myCtrlSendingGroupNumber = CInt(myGlobal.SetDatos) + 1
                        End If
                    End If
                    'TR 19/07/2012 -END

                    Dim newSampleRotorStatus As String = ""
                    Dim repCreatedFlag As Boolean = True 'Manual repetitions always are created
                    For Each rep As WSRepetitionsToAddDS.twksWSRepetitionsToAddRow In pendingRepToAdd.twksWSRepetitionsToAdd.Rows
                        If (rep.SampleClass = "CTRL" AndAlso myCtrlSendingGroupNumber > 0) Then
                            'TR 19/07/2012 - Update value of field CtrlSendingGroup on twksWSOrderTests
                            myGlobal = myWSOrderTestDelegate.UpdateCtrlsSendingGroup(dbConnection, pWorkSessionID, rep.OrderTestID, myCtrlSendingGroupNumber)
                            If (myGlobal.HasError) Then Exit For
                        End If

                        'AG 31/03/2014 - #1565 add parameter pRunningMode in the proper position
                        myGlobal = ManageRepetitions(dbConnection, pAnalyzerID, pWorkSessionID, rep.OrderTestID, 1, repCreatedFlag, newSampleRotorStatus, pRunningMode, _
                                                     True, rep.PostDilutionType, pIsISEModuleReady)
                        If (myGlobal.HasError) Then Exit For
                    Next

                    'Delete pending
                    If (Not myGlobal.HasError) Then
                        myGlobal = myWSRepetitionsToAddDelegate.DeleteAll(dbConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                GlobalBase.CreateLogActivity("AddManualRepetitions (Complete): " & _
                                                Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                "RepetititonsDelegate.AddManualRepetitions", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                If (Not myGlobal.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    'If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    'If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                'If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "RepetitionsDelegate.AddManualRepetitions", EventLogEntryType.Error, False)
            Finally
                'If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function

#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Method who initializes the internal structure
        ''' </summary>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pRerunNumber">Number of the last rerun requested for the specifier OrderTest</param>
        ''' <returns>GlobalDataTO indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by:  DL 03/05/2010
        ''' Modified by: AG 10/05/2010 - Add pDBConnection
        '''              SA 28/10/2010 - Call to TestsDelegate.AnalysisMode replaced by call to TestsDelegate.Read
        '''              SA 30/01/2012 - Call to TestSamplesDelegate.GetTestSampleDataByTestIDAndSampleType replaced by call to TestSamplesDelegate.GetDefinition
        '''                              Call to OrderTestsDelegate.GetTestID replaced by call to OrderTestsDelegate.GetOrderTest
        '''                              Error control improved: if an error is raised (GlobalDataTO.HasError), do not continue with the rest of the process
        ''' </remarks>
        Private Function Init(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestID As Integer, ByVal pRerunNumber As Integer) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                'Initialize default values in the global structure
                myRepetitionCriterion.SubstrateDepletion = 0
                myRepetitionCriterion.LinearityLimit = -1
                myRepetitionCriterion.DetectionLimit = -1
                myRepetitionCriterion.RepetitionLimitMax = -1
                myRepetitionCriterion.RepetitionLimitMin = -1
                myRepetitionCriterion.AutomaticRerun = False

                myGlobal = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOrderTestDelegate As New OrderTestsDelegate
                        myGlobal = myOrderTestDelegate.GetOrderTest(dbConnection, pOrderTestID)

                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            Dim myOrderTestDS As OrderTestsDS = DirectCast(myGlobal.SetDatos, OrderTestsDS)

                            'General Data 
                            myRepetitionCriterion.TestID = myOrderTestDS.twksOrderTests.Item(0).TestID
                            myRepetitionCriterion.SampleType = myOrderTestDS.twksOrderTests.Item(0).SampleType

                            'Result Data 
                            Dim myTestsDelegate As New TestsDelegate

                            myGlobal = myTestsDelegate.Read(dbConnection, myOrderTestDS.twksOrderTests.Item(0).TestID)
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                Dim myTestDS As TestsDS = DirectCast(myGlobal.SetDatos, TestsDS)

                                If (myTestDS.tparTests.Item(0).AnalysisMode.Trim = "BRK" OrElse myTestDS.tparTests.Item(0).AnalysisMode.Trim = "MRK") Then
                                    myRepetitionCriterion.KineticTest = True
                                Else
                                    myRepetitionCriterion.KineticTest = False
                                End If
                            End If

                            If (Not myGlobal.HasError) Then
                                Dim myExecutionsDelegate As New ExecutionsDelegate

                                myGlobal = myExecutionsDelegate.ReadByOrderTestIDandRerunNumber(dbConnection, pOrderTestID, pRerunNumber)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    Dim myExecutionsDS As ExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)

                                    If (Not myExecutionsDS.twksWSExecutions(0).IsSampleClassNull) Then
                                        myRepetitionCriterion.SampleClass = myExecutionsDS.twksWSExecutions.Item(0).SampleClass
                                    End If

                                    myRepetitionCriterion.LinealKinetics = True
                                    For Each myRow As twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                        If (Not myRow.IsKineticsLinearNull) Then
                                            If (Not myRow.KineticsLinear) Then
                                                myRepetitionCriterion.LinealKinetics = False
                                                Exit For
                                            End If
                                        End If
                                    Next myRow
                                End If
                            End If

                            If (Not myGlobal.HasError) Then
                                Dim myResultsDelegate As New ResultsDelegate

                                myGlobal = myResultsDelegate.ReadByOrderTestIDandRerunNumber(dbConnection, pOrderTestID, pRerunNumber)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    Dim myResultsDS As ResultsDS = DirectCast(myGlobal.SetDatos, ResultsDS)

                                    If (myResultsDS.twksResults.Count > 0) Then
                                        If (Not myResultsDS.twksResults(0).IsCONC_ValueNull) Then myRepetitionCriterion.Conc = myResultsDS.twksResults.Item(0).CONC_Value
                                        If (Not myResultsDS.twksResults(0).IsABSValueNull) Then myRepetitionCriterion.Abs = myResultsDS.twksResults.Item(0).ABSValue
                                        If (Not myResultsDS.twksResults(0).IsSubstrateDepletionNull) Then myRepetitionCriterion.SubstrateDepletion = myResultsDS.twksResults.Item(0).SubstrateDepletion
                                        If (Not myResultsDS.twksResults(0).IsCONC_ErrorNull) Then myRepetitionCriterion.ConcErrors = myResultsDS.twksResults.Item(0).CONC_Error
                                        If (Not myResultsDS.twksResults(0).IsABS_ErrorNull) Then myRepetitionCriterion.AbsErrors = myResultsDS.twksResults.Item(0).ABS_Error
                                    End If
                                End If
                            End If

                            'Test programming data 
                            If (Not myGlobal.HasError) Then
                                Dim myTestsSampleDelegate As New TestSamplesDelegate()
                                Dim myTestID As Integer = CType(myOrderTestDS.twksOrderTests.Item(0).TestID, Integer)

                                myGlobal = myTestsSampleDelegate.GetDefinition(dbConnection, myTestID, myRepetitionCriterion.SampleType)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    Dim myTestSamplesDS As TestSamplesDS = DirectCast(myGlobal.SetDatos, TestSamplesDS)
                                    If (myTestSamplesDS.tparTestSamples.Count > 0) Then
                                        If (Not myTestSamplesDS.tparTestSamples(0).IsLinearityLimitNull) Then myRepetitionCriterion.LinearityLimit = myTestSamplesDS.tparTestSamples(0).LinearityLimit
                                        If (Not myTestSamplesDS.tparTestSamples(0).IsDetectionLimitNull) Then myRepetitionCriterion.DetectionLimit = myTestSamplesDS.tparTestSamples(0).DetectionLimit
                                        If (Not myTestSamplesDS.tparTestSamples(0).IsRerunUpperLimitNull) Then myRepetitionCriterion.RepetitionLimitMax = myTestSamplesDS.tparTestSamples(0).RerunUpperLimit
                                        If (Not myTestSamplesDS.tparTestSamples(0).IsRerunLowerLimitNull) Then myRepetitionCriterion.RepetitionLimitMin = myTestSamplesDS.tparTestSamples(0).RerunLowerLimit

                                        'FIRST: If AutomaticRerun is TRUE for the Test/SampleType...
                                        If (myTestSamplesDS.tparTestSamples(0).AutomaticRerun) Then
                                            'Verify if the User Setting for Automatic Repetitions is also set
                                            Dim myUserSettingsDelegate As New UserSettingsDelegate

                                            myGlobal = myUserSettingsDelegate.ReadBySettingID(dbConnection, UserSettingsEnum.AUTOMATIC_RERUN.ToString)
                                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                                Dim myUserSettingDS As UserSettingDS = DirectCast(myGlobal.SetDatos, UserSettingDS)

                                                'SECOND: Get the Status and CurrentValue of the AUTOMATIC_RERUN User Setting
                                                'If (Status = TRUE And CurrentValue = 1) Then set field AutomaticRerun = TRUE; otherwise, set AutomaticRerun = FALSE
                                                'AG 10/04/2012 - Use CBool instead of Convert.ToBoolean
                                                If (myUserSettingDS.tcfgUserSettings(0).Status AndAlso CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)) Then
                                                    'AG 10/04/2012
                                                    myRepetitionCriterion.AutomaticRerun = True
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "RepetitionsDelegate.Init", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Method that calculate the criteria for automatic repetition
        ''' </summary>
        ''' <returns>GlobalDataTO containing an String value with the criteria to apply for the Automatic Rerun</returns>
        ''' <remarks>
        ''' Created by:  DL 03/05/2010 
        ''' Modified by: AG 10/04/2012 - corrections
        ''' Modified by: SG 17/04/2013 - If LIS_WORKING_MODE_RERUNS = "LIS" returns always 'CriterionResult.NO_REP'
        ''' Modified by  AG 30/04/2013 - fix issues in code changes 17/04/2013
        ''' Modified by  JC 14/05/2013 - When LIS_WORKING_MODE_RERUNS = "LIS" ONLY return 'CriterionResult.NO_REP' if SampleClass is PATIENT
        ''' </remarks>
        Private Function CalculateAutoRepetitionCriterion() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones1 As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Try
                Dim resultData As String = CriterionResult.NO_REP.ToString

                'SGM 17/04/2013 - 
                Dim myRerunLISMode As String = "BOTH" 'AG - Change default value from LIS to BOTH
                Dim myUsersSettingsDelegate As New UserSettingsDelegate
                myGlobal = myUsersSettingsDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WORKING_MODE_RERUNS.ToString)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    myRerunLISMode = TryCast(myGlobal.SetDatos, String)
                End If 'AG 30/04/2013

                ' JC 15/05/2013
                ' PATIENT ReRUN from LIS is NOT_ALLOWED
                If (myRepetitionCriterion.SampleClass = "PATIENT" AndAlso myRerunLISMode <> "LIS") Then
                    If (myRepetitionCriterion.SubstrateDepletion = 2) Then
                        'Priority1: All Patient replicates with Substrate Depletion		
                        'OrderTestID belongs to a Patient Internal Structure field SubstrateDepletion = 2
                        resultData = CriterionResult.RED.ToString

                    ElseIf (myRepetitionCriterion.ConcErrors = "OUT_HIGH") Then
                        'Priority2: Patient Absorbance outside (HIGH) the Calibration Curve		
                        'OrderTestID belongs to a Patient Internal structure field TestID belongs a test how calibrates with curve (calibration multi item)
                        'Internal Structure field ConcError = OUT_HIGH"
                        resultData = CriterionResult.RED.ToString

                    ElseIf (myRepetitionCriterion.Conc < 0) Then
                        'Priority3: Patient Absorbance is minor (LOW) than Blank Absorbance		
                        'OrderTestID belongs to a patient Internal structure field TestID belongs a test how calibrates with curve (calibration multi item)
                        'Internal Structure field ConcValue < 0"
                        resultData = CriterionResult.INC.ToString

                    ElseIf (myRepetitionCriterion.LinearityLimit <> -1 AndAlso myRepetitionCriterion.Conc > myRepetitionCriterion.LinearityLimit) Then
                        'Priority4: Patient Concentration value is higher than Linearity Limit
                        'OrderTestID belongs to a patient Internal Structure fields LinearityLimit <> -1 and ConcValue > LineartityLimit"
                        resultData = CriterionResult.RED.ToString

                        'AG 10/04/2012
                    ElseIf (myRepetitionCriterion.DetectionLimit <> -1 AndAlso myRepetitionCriterion.Conc < myRepetitionCriterion.DetectionLimit) Then
                        'ElseIf (myRepetitionCriterion.LinearityLimit <> -1 AndAlso myRepetitionCriterion.Conc < myRepetitionCriterion.LinearityLimit) Then
                        'AG 10/04/2012

                        'Priority5: Patient Concentration value is lower than Detection Limit		
                        'OrderTestID belongs to a patient Internal Structure fields DetectionLimit <> -1 and ConcValue < LineartityLimit
                        resultData = CriterionResult.INC.ToString

                        'AG 10/04/2012
                        'TR 29/09/2012 -Change the validation use AndAlso instead of OrElse
                    ElseIf (myRepetitionCriterion.RepetitionLimitMax <> -1) AndAlso _
                           (myRepetitionCriterion.RepetitionLimitMin <> -1) AndAlso _
                           (myRepetitionCriterion.RepetitionLimitMin < myRepetitionCriterion.Conc AndAlso myRepetitionCriterion.Conc < myRepetitionCriterion.RepetitionLimitMax) Then

                        'ElseIf (myRepetitionCriterion.RepetitionLimitMax <> -1) AndAlso _
                        '(myRepetitionCriterion.RepetitionLimitMin <> -1) AndAlso _
                        '(CSng(myRepetitionCriterion.RepetitionLimitMin < myRepetitionCriterion.Conc) < myRepetitionCriterion.RepetitionLimitMax) Then
                        'AG 10/04/2012

                        'Priority6: Rerun Limit Minimum < Patient Concentration < Rerun Limit Maximum		
                        'OrderTestID belongs to a patient Internal Structure fields RepetitionLimitMax <> -1 and RepetitionLimitMin <> -1
                        'RepetitionLimitMin < ConcValue < RepetitionLimitMax"
                        resultData = CriterionResult.NONE.ToString  'NONE = Equal
                    End If
                End If

                'End If
                'End If
                ''end SGM 17/04/2013 - AG 30/04/2013 - Comment these 2 'End If', they must be place after the next If ... End If block (it is another auto rerun criteria!!)

                If (resultData = CriterionResult.NO_REP.ToString AndAlso myRepetitionCriterion.SampleClass <> "BLANK") Then
                    If Not (myRepetitionCriterion.SampleClass = "PATIENT" AndAlso myRerunLISMode = "LIS") Then

                        If (myRepetitionCriterion.KineticTest AndAlso Not myRepetitionCriterion.LinealKinetics) Then
                            '7: Non linear kinetics (except Blanks)
                            'OrderTestID don't belongs to a blank Internal Structure fields KineticTest = TRUE and LinealKinetics = FALSE
                            resultData = CriterionResult.NONE.ToString  'NONE = EQUAL
                        End If

                    End If
                End If
                'End If 'AG 30/04/2013


                myGlobal.SetDatos = resultData
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "RepetitionsDelegate.CalculateAutoRepetitionCriterion", EventLogEntryType.Error, False)
            End Try

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("CalculateAutoRepetitionCriterion (Complete): " & _
                                            Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "RepetitionDelegate.CalculateAutoRepetitionCriterion", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***


            Return myGlobal
        End Function
#End Region
    End Class
End Namespace
